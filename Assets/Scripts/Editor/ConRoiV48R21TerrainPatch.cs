using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;

// Replace terrain triangles in two bounded approach earthwork areas. The
// replacement belongs to the original terrain mesh and its collider; it is
// neither an overlay floor nor a foundation box. Preserve source height before
// the separate grading operation, including all clipped boundary intersections.
public static class ConRoiV48R21TerrainPatch
{
 struct Point
 {
  public Vector3 p;public Vector2 uv;
  public Point(Vector3 p,Vector2 uv){this.p=p;this.uv=uv;}
 }
 public static Mesh Remesh(Mesh source,Transform owner,Rect[] regions=null,string auditPath="Docs/Reviews/journey_v48/r21/local-terrain-remesh-draft.json")
 {
  if(source.subMeshCount!=1)throw new Exception("Terrain patch requires one source material");
  var points=source.vertices.Select(owner.TransformPoint).ToArray();var uv=source.uv;var triangles=source.triangles;
  var polygons=new List<List<Point>>();
  for(int i=0;i<triangles.Length;i+=3)polygons.Add(new List<Point>(new[]{triangles[i],triangles[i+1],triangles[i+2]}.Select(j=>new Point(points[j],uv.Length==points.Length?uv[j]:Vector2.zero))));
  var temporary=new GameObject("Temporary_Shore_Height_Sampler");temporary.transform.SetPositionAndRotation(owner.position,owner.rotation);temporary.transform.localScale=owner.lossyScale;
  var sampler=temporary.AddComponent<MeshCollider>();sampler.sharedMesh=source;Physics.SyncTransforms();
  var rows=new JArray();
  // Bounds include the whole footprint plus the five-metre graded shoulder.
  var rectangles=regions??new[]{new Rect(672,467,53,50),new Rect(610,518,55,49),new Rect(640,418,45,102)};
  foreach(var rect in rectangles)
  {
   var kept=new List<List<Point>>();var boundary=new List<Point>();int removed=0;
   float Plane(Point p,int edge)=>edge==0?p.p.x-rect.xMin:edge==1?rect.xMax-p.p.x:edge==2?p.p.z-rect.yMin:rect.yMax-p.p.z;
   List<Point> Clip(List<Point> input,int edge,bool inside)
   {
    var output=new List<Point>();if(input.Count==0)return output;
    for(int j=0;j<input.Count;j++)
    {
     var a=input[j];var b=input[(j+1)%input.Count];float da=Plane(a,edge),db=Plane(b,edge);bool ia=inside?da>=0:da<=0,ib=inside?db>=0:db<=0;
     if(ia)output.Add(a);
     if(ia!=ib){float t=da/(da-db);var q=new Point(Vector3.Lerp(a.p,b.p,t),Vector2.Lerp(a.uv,b.uv,t));output.Add(q);boundary.Add(q);}
    }
    return output;
   }
   foreach(var polygon in polygons)
   {
    if(polygon.All(p=>p.p.x<=rect.xMin)||polygon.All(p=>p.p.x>=rect.xMax)||polygon.All(p=>p.p.z<=rect.yMin)||polygon.All(p=>p.p.z>=rect.yMax)){kept.Add(polygon);continue;}
    var remaining=polygon;
    for(int edge=0;edge<4&&remaining.Count>=3;edge++)
    {
     var outside=Clip(remaining,edge,false);if(outside.Count>=3)kept.Add(outside);
     remaining=Clip(remaining,edge,true);
    }
    if(remaining.Count>=3)removed++;
   }
   boundary=boundary.Where(q=>q.p.x>=rect.xMin-.001f&&q.p.x<=rect.xMax+.001f&&q.p.z>=rect.yMin-.001f&&q.p.z<=rect.yMax+.001f).ToList();
   int fallback=0;
   Point Sample(float x,float z)
   {
    Vector3 p=new Vector3(x,0,z);
    if(sampler.Raycast(new Ray(new Vector3(x,40,z),Vector3.down),out var hit,100))p.y=hit.point.y;
    else
    {
     // Original terrain was trimmed beneath some road polygons. Interpolate
     // nearby source ground, then let footprint grading establish its support.
     fallback++;float weight=0,height=0;foreach(var near in points.OrderBy(v=>(v.x-x)*(v.x-x)+(v.z-z)*(v.z-z)).Take(4))
     {float w=1/Mathf.Max(.00001f,(near.x-x)*(near.x-x)+(near.z-z)*(near.z-z));weight+=w;height+=near.y*w;}
     p.y=height/weight;
    }
    return new Point(p,new Vector2(x*.1f,z*.1f));
   }
   const float step=.5f;int nx=Mathf.CeilToInt(rect.width/step),nz=Mathf.CeilToInt(rect.height/step);
   var grid=new Point[nx+1,nz+1];for(int x=0;x<=nx;x++)for(int z=0;z<=nz;z++)grid[x,z]=Sample(Mathf.Lerp(rect.xMin,rect.xMax,x/(float)nx),Mathf.Lerp(rect.yMin,rect.yMax,z/(float)nz));
   for(int x=0;x<nx;x++)for(int z=0;z<nz;z++)
   {
    var a=grid[x,z];var b=grid[x,z+1];var c=grid[x+1,z+1];var d=grid[x+1,z];var ring=new List<Point>();
    void Edge(Point u,Point v,bool outer)
    {
     ring.Add(u);if(!outer)return;
     var delta=v.p-u.p;delta.y=0;
     foreach(var q in boundary.Where(q=>Mathf.Abs((q.p.x-u.p.x)*delta.z-(q.p.z-u.p.z)*delta.x)<.0001f).Select(q=>(q,t:Vector3.Dot(new Vector3(q.p.x-u.p.x,0,q.p.z-u.p.z),delta)/delta.sqrMagnitude)).Where(v=>v.t>.0001f&&v.t<.9999f).OrderBy(v=>v.t))ring.Add(q.q);
    }
    Edge(a,b,x==0);Edge(b,c,z==nz-1);Edge(c,d,x==nx-1);Edge(d,a,z==0);
    var centre=Sample((a.p.x+c.p.x)*.5f,(a.p.z+c.p.z)*.5f);
    for(int i=0;i<ring.Count;i++)kept.Add(new List<Point>{centre,ring[i],ring[(i+1)%ring.Count]});
   }
   rows.Add(new JObject{["min_x"]=rect.xMin,["max_x"]=rect.xMax,["min_z"]=rect.yMin,["max_z"]=rect.yMax,["removed_source_polygons"]=removed,["trimmed_ground_samples_interpolated"]=fallback,["grid_spacing_m"]=step,["boundary_intersections_preserved"]=boundary.Count});
   polygons=kept;
  }
  UnityEngine.Object.DestroyImmediate(temporary);
  var vertices=new List<Vector3>();var uvs=new List<Vector2>();var indices=new List<int>();var weld=new Dictionary<(int,int,int),int>();
  int Add(Point p)
  {
   var k=(Mathf.RoundToInt(p.p.x*10000),Mathf.RoundToInt(p.p.y*10000),Mathf.RoundToInt(p.p.z*10000));if(weld.TryGetValue(k,out var found))return found;
   int id=vertices.Count;weld.Add(k,id);vertices.Add(owner.InverseTransformPoint(p.p));uvs.Add(p.uv);return id;
  }
  foreach(var polygon in polygons)for(int i=1;i<polygon.Count-1;i++)
  {
   var a=polygon[0];var b=polygon[i];var c=polygon[i+1];if(Vector3.Cross(b.p-a.p,c.p-a.p).sqrMagnitude<1e-12f)continue;
   indices.Add(Add(a));indices.Add(Add(b));indices.Add(Add(c));
  }
  var mesh=new Mesh{name="R21_ReplacedApproachTerrain",indexFormat=UnityEngine.Rendering.IndexFormat.UInt32};mesh.SetVertices(vertices);mesh.SetUVs(0,uvs);mesh.SetTriangles(indices,0);ConRoiV48R21PathConform.RemoveInvalidLocalFaces(mesh,owner);mesh.RecalculateNormals();mesh.RecalculateBounds();
  File.WriteAllText(auditPath,new JObject{["status"]="DRAFT_REQUIRES_BUILD_VISUAL_CHECK",["source_triangles"]=triangles.Length/3,["result_triangles"]=indices.Count/3,["patches"]=rows,["source_outside_patches_preserved"]=true,["water_modified"]=false}.ToString());return mesh;
 }
}
