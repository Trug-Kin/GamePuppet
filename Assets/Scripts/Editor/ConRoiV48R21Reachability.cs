using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEditor.SceneManagement;

// Supplemental static exploration audit, not a player traversal test. Uses
// ground normals, headroom and obstacle sweeps; never moves the player/save.
public static class ConRoiV48R21Reachability
{
 public static void Audit()
 {
  EditorSceneManager.OpenScene("Assets/Scenes/JourneyRepair_v48_r21.unity");
  var story=UnityEngine.Object.FindAnyObjectByType<DieuPhoiHanhTrinh>();story.player.gameObject.SetActive(false);
  story.hub.SetActive(false);story.lake.SetActive(true);story.market.SetActive(true);story.schoolChapter.environment.SetActive(false);story.slopeChapter.environment.SetActive(false);
  foreach(var g in story.GetComponent<CanhQuanChoV48>().scenery)g.SetActive(true);Physics.SyncTransforms();
  // Include a margin outside both authored terrain meshes. The old Z=130
  // cut crossed real terrain ending at Z=58.76, so reaching that grid edge
  // alone did not demonstrate an escape from the actual map.
  const int minX=448,maxX=866,minZ=54,maxZ=606,step=2;int nx=(maxX-minX)/step+1,nz=(maxZ-minZ)/step+1;
  var points=new Vector3[nx,nz];var valid=new bool[nx,nz];var seen=new bool[nx,nz];var parent=new Dictionary<(int,int),(int,int)>();int walkable=0;
  for(int x=0;x<nx;x++)for(int z=0;z<nz;z++)
  {
   var ray=new Ray(new Vector3(minX+x*step,45,minZ+z*step),Vector3.down);
   var hits=Physics.RaycastAll(ray,100,8,QueryTriggerInteraction.Ignore).Where(h=>h.normal.y>.7071f).OrderByDescending(h=>h.point.y);
   foreach(var hit in hits)
   {
    var p=hit.point;bool blocked=Physics.CheckCapsule(p+Vector3.up*.34f,p+Vector3.up*1.52f,.28f,8,QueryTriggerInteraction.Ignore);if(blocked)continue;
    points[x,z]=p;valid[x,z]=true;walkable++;break;
   }
  }
  var spawn=story.lakeSpawn.position;int sx=Mathf.RoundToInt((spawn.x-minX)/step),sz=Mathf.RoundToInt((spawn.z-minZ)/step);
  if(!valid[sx,sz])throw new Exception("Static grid has no headroom at lake start; do not infer inaccessible route");
  var queue=new Queue<(int,int)>();queue.Enqueue((sx,sz));seen[sx,sz]=true;int reached=0;var edges=new List<(int,int)>();
  var directions=new[]{(1,0),(-1,0),(0,1),(0,-1)};
  while(queue.Count>0)
  {
   var current=queue.Dequeue();reached++;int x=current.Item1,z=current.Item2;
   if(x<=2||x>=nx-3||z<=2||z>=nz-3)edges.Add(current);
   foreach(var delta in directions)
   {
    int xx=x+delta.Item1,zz=z+delta.Item2;if(xx<0||xx>=nx||zz<0||zz>=nz||seen[xx,zz]||!valid[xx,zz])continue;
    var a=points[x,z];var b=points[xx,zz];if(Mathf.Abs(b.y-a.y)>step*.9f)continue;
    var offset=b-a;bool blocked=Physics.SphereCast(a+Vector3.up*.95f,.28f,offset.normalized,out var obstacle,offset.magnitude,8,QueryTriggerInteraction.Ignore);
    if(blocked)continue;seen[xx,zz]=true;parent[(xx,zz)]=current;queue.Enqueue((xx,zz));
   }
  }
  JArray V(Vector3 p)=>new JArray(p.x,p.y,p.z);
  var paths=new JArray();foreach(var end in edges.Where((p,i)=>i%Mathf.Max(1,edges.Count/24)==0).Take(24))
  {
   var path=new List<(int,int)>{end};var p=end;while(parent.TryGetValue(p,out var previous)){path.Add(previous);p=previous;}path.Reverse();
   paths.Add(new JObject{["edge_point"]=V(points[end.Item1,end.Item2]),["sampled_path"]=new JArray(path.Where((v,i)=>i%5==0||i==path.Count-1).Select(v=>V(points[v.Item1,v.Item2])))});
  }
  var interactions=new JArray();foreach(var i in UnityEngine.Object.FindObjectsByType<TuongTacHoCho>().Where(i=>i.kind==TuongTacHoCho.Kind.Bao||i.kind==TuongTacHoCho.Kind.Ha||i.kind==TuongTacHoCho.Kind.Wake))
  {
   var p=i.transform.position;bool near=false;for(int x=0;x<nx;x++)for(int z=0;z<nz;z++)if(seen[x,z]&&Vector2.Distance(new Vector2(points[x,z].x,points[x,z].z),new Vector2(p.x,p.z))<3)near=true;
   interactions.Add(new JObject{["kind"]=i.kind.ToString(),["position"]=V(p),["grid_nearby_reachable"]=near});
  }
  File.WriteAllText("Docs/Reviews/journey_v48/r21/static-reachability-audit.json",new JObject{["status"]="SUPPLEMENTAL_STATIC_GRID_NOT_NATIVE_MOVEMENT_OR_ESCAPE_PROOF",["grid_step_m"]=step,["grid_bounds"]=new JArray(minX,maxX,minZ,maxZ),["previous_bounds_correction"]="Old bounds cut through authored terrain; old outer-grid count alone is not an actual escape count",["limits"]="2 m grid may miss narrow paths or barrier endpoints and does not test falling off terrain; no jumping, progression, dynamic poses, camera, visual framing or save/load tested",["walkable_samples"]=walkable,["reachable_samples"]=reached,["reachable_outer_grid_samples"]=edges.Count,["possible_outer_paths"]=paths,["interactions"]=interactions}.ToString());
 }
}
