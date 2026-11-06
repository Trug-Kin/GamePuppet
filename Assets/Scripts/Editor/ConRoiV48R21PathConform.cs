using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Split local path triangles at the actual terrain triangle boundaries before
// height projection. Merely sampling sparse path vertices lets their interiors
// cut through a curved bank even when every sampled vertex clears the ground.
public static class ConRoiV48R21PathConform
{
 // R50: clipping/densification can produce distinct samples that collapse
 // during the producer's 0.1mm weld. Validate AFTER the weld/projection.
 // Positions and every face outside the audited bridge patch are preserved.
 public static void RemoveInvalidLocalFaces(Mesh mesh,Transform owner)
 {
  var v=mesh.vertices.Select(owner.TransformPoint).ToArray();
  bool Local(Vector3 p)=>p.x>=626&&p.x<=709&&p.z>=484&&p.z<=557;
  string Key(Vector3 p)=>System.Math.Round(p.x,5)+","+System.Math.Round(p.y,5)+","+System.Math.Round(p.z,5);
  for(int sub=0;sub<mesh.subMeshCount;sub++){
   var src=mesh.GetTriangles(sub);var dst=new List<int>(src.Length);var seen=new HashSet<string>();
   for(int i=0;i<src.Length;i+=3){int a=src[i],b=src[i+1],c=src[i+2];
    if(Local(v[a])&&Local(v[b])&&Local(v[c])){
     if(Vector3.Cross(v[b]-v[a],v[c]-v[a]).sqrMagnitude<4e-18f)continue;
     var keys=new[]{Key(v[a]),Key(v[b]),Key(v[c])};System.Array.Sort(keys,System.StringComparer.Ordinal);
     if(!seen.Add(string.Join("|",keys)))continue;
    }
    dst.Add(a);dst.Add(b);dst.Add(c);
   }
   mesh.SetTriangles(dst,sub);
  }
 }
 struct P {public Vector3 p;public Vector2 uv;public P(Vector3 p,Vector2 uv){this.p=p;this.uv=uv;}}
 public static Mesh Split(Mesh path,Transform owner,MeshFilter ground)
 {
  var gv=ground.sharedMesh.vertices;for(int i=0;i<gv.Length;i++)gv[i]=ground.transform.TransformPoint(gv[i]);var gt=ground.sharedMesh.triangles;
  var bins=new Dictionary<(int,int),List<int>>();const float size=2;
  for(int i=0;i<gt.Length;i+=3)
  {
   var a=gv[gt[i]];var b=gv[gt[i+1]];var c=gv[gt[i+2]];float minX=Mathf.Min(a.x,Mathf.Min(b.x,c.x)),maxX=Mathf.Max(a.x,Mathf.Max(b.x,c.x)),minZ=Mathf.Min(a.z,Mathf.Min(b.z,c.z)),maxZ=Mathf.Max(a.z,Mathf.Max(b.z,c.z));
   if(maxX<610||minX>725||maxZ<470||minZ>565||Vector3.Cross(b-a,c-a).y<=1e-8f)continue;
   for(int x=Mathf.FloorToInt(Mathf.Max(minX,610)/size);x<=Mathf.FloorToInt(Mathf.Min(maxX,725)/size);x++)for(int z=Mathf.FloorToInt(Mathf.Max(minZ,470)/size);z<=Mathf.FloorToInt(Mathf.Min(maxZ,565)/size);z++)
   {var key=(x,z);if(!bins.TryGetValue(key,out var list)){list=new List<int>();bins.Add(key,list);}list.Add(i);}
  }
  var pv=path.vertices;var uv=path.uv;var pt=path.triangles;var vertices=new List<Vector3>();var uvs=new List<Vector2>();var faces=new List<int>();
  var weld=new Dictionary<(int,int,int),int>();
  int Add(P p)
  {
   var key=(Mathf.RoundToInt(p.p.x*10000),Mathf.RoundToInt(p.p.y*10000),Mathf.RoundToInt(p.p.z*10000));if(weld.TryGetValue(key,out int id))return id;
   id=vertices.Count;vertices.Add(owner.InverseTransformPoint(p.p));uvs.Add(p.uv);weld.Add(key,id);return id;
  }
  float Cross(Vector3 a,Vector3 b,Vector3 p)=>(b.x-a.x)*(p.z-a.z)-(b.z-a.z)*(p.x-a.x);
  void Emit(List<P> polygon)
  {for(int j=1;j<polygon.Count-1;j++){if(Vector3.Cross(polygon[j].p-polygon[0].p,polygon[j+1].p-polygon[0].p).sqrMagnitude<1e-12f)continue;faces.Add(Add(polygon[0]));faces.Add(Add(polygon[j]));faces.Add(Add(polygon[j+1]));}}
  for(int i=0;i<pt.Length;i+=3)
  {
   var polygon=new List<P>();for(int j=0;j<3;j++){int n=pt[i+j];polygon.Add(new P(owner.TransformPoint(pv[n]),uv.Length==pv.Length?uv[n]:Vector2.zero));}
   float minX=725,maxX=610,minZ=565,maxZ=470;bool local=true;
   foreach(var p in polygon){if(p.p.x<610||p.p.x>725||p.p.z<470||p.p.z>565)local=false;minX=Mathf.Min(minX,p.p.x);maxX=Mathf.Max(maxX,p.p.x);minZ=Mathf.Min(minZ,p.p.z);maxZ=Mathf.Max(maxZ,p.p.z);}
   if(!local){Emit(polygon);continue;}
   var candidates=new HashSet<int>();for(int x=Mathf.FloorToInt(minX/size);x<=Mathf.FloorToInt(maxX/size);x++)for(int z=Mathf.FloorToInt(minZ/size);z<=Mathf.FloorToInt(maxZ/size);z++)if(bins.TryGetValue((x,z),out var list))foreach(int t in list)candidates.Add(t);
   bool emitted=false;
   foreach(int t in candidates)
   {
    var clip=new List<P>(polygon);var triangle=new[]{gv[gt[t]],gv[gt[t+1]],gv[gt[t+2]]};float sign=Mathf.Sign(Cross(triangle[0],triangle[1],triangle[2]));
    for(int edge=0;edge<3&&clip.Count>=3;edge++)
    {
     var next=new List<P>();var a=triangle[edge];var b=triangle[(edge+1)%3];
     for(int j=0;j<clip.Count;j++)
     {var p=clip[j];var q=clip[(j+1)%clip.Count];float dp=Cross(a,b,p.p)*sign,dq=Cross(a,b,q.p)*sign;bool ip=dp>=0,iq=dq>=0;if(ip)next.Add(p);if(ip!=iq){float f=dp/(dp-dq);next.Add(new P(Vector3.Lerp(p.p,q.p,f),Vector2.Lerp(p.uv,q.uv,f)));}}
     clip=next;
    }
    if(clip.Count>=3){Emit(clip);emitted=true;}
   }
   if(!emitted)Emit(polygon);
  }
  var result=new Mesh{name="TerrainAlignedPath",indexFormat=UnityEngine.Rendering.IndexFormat.UInt32};result.SetVertices(vertices);result.SetUVs(0,uvs);result.SetTriangles(faces,0);RemoveInvalidLocalFaces(result,owner);result.RecalculateNormals();result.RecalculateBounds();return result;
 }
}
