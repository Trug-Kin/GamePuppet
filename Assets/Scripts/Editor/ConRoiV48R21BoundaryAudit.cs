using System.IO;
using System.Linq;
using UnityEngine;
using UnityEditor.SceneManagement;
using Newtonsoft.Json.Linq;

public static class ConRoiV48R21BoundaryAudit
{
 static JArray V(Vector3 p)=>new JArray(p.x,p.y,p.z);
 static string PathOf(Transform t)=>t.parent?PathOf(t.parent)+"/"+t.name:t.name;
 public static void Audit()
 {
  EditorSceneManager.OpenScene("Assets/Scenes/JourneyRepair_v48_r21.unity");Physics.SyncTransforms();
  var story=Object.FindAnyObjectByType<DieuPhoiHanhTrinh>();var colliders=new JArray();var interactions=new JArray();
  foreach(var c in Object.FindObjectsByType<Collider>(FindObjectsInactive.Include))
  {
   Bounds world=new Bounds(c.transform.position,Vector3.zero);
   var local=c is BoxCollider bx?new Bounds(bx.center,bx.size):c is MeshCollider mc&&mc.sharedMesh?mc.sharedMesh.bounds:new Bounds(Vector3.zero,Vector3.zero);
   bool first=true;for(int x=0;x<2;x++)for(int y=0;y<2;y++)for(int z=0;z<2;z++){var p=c.transform.TransformPoint(local.center+Vector3.Scale(local.extents,new Vector3(x*2-1,y*2-1,z*2-1)));if(first){world=new Bounds(p,Vector3.zero);first=false;}else world.Encapsulate(p);}
   string path=PathOf(c.transform);bool water=c is MeshCollider m&&m.sharedMesh&&world.size.y<.01f&&world.size.x>50&&world.size.z>50;
   if(!water&&!path.Contains("Fence")&&!path.Contains("Boundary")&&!path.Contains("Gate")&&!path.Contains("Wall")&&!path.Contains("Envelope")&&!path.Contains("Closed_Outer_Streets")&&!path.Contains("Lan_")&&!path.Contains("House_"))continue;
   colliders.Add(new JObject{["path"]=path,["type"]=c.GetType().Name,["enabled"]=c.enabled,["active_self"]=c.gameObject.activeSelf,["hierarchy_active_at_authoring"]=c.gameObject.activeInHierarchy,["position"]=V(c.transform.position),["world_min"]=V(world.min),["world_max"]=V(world.max),["trigger"]=c.isTrigger,["possible_large_flat_water_mesh"]=water,["renderers"]=new JArray(c.GetComponentsInChildren<Renderer>(true).Select(r=>r.name))});
  }
  foreach(var behaviour in Object.FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include).Where(b=>b is IPlayerInteractable))
   interactions.Add(new JObject{["path"]=PathOf(behaviour.transform),["type"]=behaviour.GetType().Name,["position"]=V(behaviour.transform.position)});
  File.WriteAllText("Docs/Reviews/journey_v48/r21/boundary-audit-draft.json",new JObject{["status"]="READ_ONLY_AUTHORING_AUDIT_NOT_NATIVE_ESCAPE_VERIFICATION",["jump_height_m"]=story.player.jumpHeight,["speed"]=story.player.speed,["hub_spawn"]=V(story.hubSpawn.position),["lake_spawn"]=V(story.lakeSpawn.position),["market_spawn"]=V(story.marketSpawn.position),["school_spawn"]=V(story.schoolChapter.spawn.position),["slope_spawn"]=V(story.slopeChapter.spawn.position),["colliders"]=colliders,["interactions"]=interactions}.ToString());
 }
}
