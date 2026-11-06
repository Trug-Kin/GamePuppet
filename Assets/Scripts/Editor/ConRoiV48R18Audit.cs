using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

// Read-only scene inspection: no transforms, meshes, colliders or scene saves.
public static class ConRoiV48R18Audit
{
 public static void Run()
 {
  const string output="Docs/Reviews/journey_v48/r18";Directory.CreateDirectory(output);
  var source=JObject.Parse(File.ReadAllText("Docs/Reviews/journey_v48/r15/planter-local-repair.json"));
  var scenes=new JArray();
  foreach(int revision in new[]{14,17})
  {
   EditorSceneManager.OpenScene("Assets/Scenes/JourneyRepair_v48_r"+revision+".unity");Physics.SyncTransforms();
   var colliders=UnityEngine.Object.FindObjectsByType<Collider>(FindObjectsInactive.Include,FindObjectsSortMode.None);
   var bases=new JArray();
   foreach(var row in source["bases"])
   {
    var footprint=row["footprint_source"].Select(p=>new Vector3(-(float)p[0],(float)row["base_top"],-(float)p[1])).ToArray();
    var center=footprint.Aggregate(Vector3.zero,(sum,p)=>sum+p)/footprint.Length;
    var samples=new JArray();
    // Interior probes avoid edge ambiguity and measure the collider visible in
    // the inherited plinth's exact footprint. Source corner samples stay linked.
    foreach(var p in footprint.Select(p=>Vector3.Lerp(p,center,.1f)).Append(center))
    {
     var hits=Physics.RaycastAll(p+Vector3.up*.01f,Vector3.down,10,~0,QueryTriggerInteraction.Ignore).Where(h=>h.normal.y>.5f).OrderByDescending(h=>h.point.y).ToArray();
     samples.Add(new JObject{["point"]=V(p),["first_support_y"]=hits.Length>0?new JValue(hits[0].point.y):JValue.CreateNull(),["gap_from_original_plinth_bottom"]=hits.Length>0?new JValue(p.y+.002f-hits[0].point.y):JValue.CreateNull(),["support"]=hits.Length>0?Path(hits[0].collider.transform):"none",["all_hits"]=new JArray(hits.Select(h=>new JObject{["path"]=Path(h.collider.transform),["y"]=h.point.y,["normal"]=V(h.normal)}))});
    }
    string meshName=(string)row["mesh"];
    var added=colliders.OfType<MeshCollider>().Where(c=>c.name==meshName).ToArray();
    bases.Add(new JObject{["source_object"]=(string)row["source_planter"],["center"]=V(center),["footprint"]=new JArray(footprint.Select(V)),["source_max_unsupported_height"]=(float)row["max_height"],["samples"]=samples,["matching_foundation_colliders"]=new JArray(added.Select(c=>new JObject{["path"]=Path(c.transform),["enabled"]=c.enabled,["active"]=c.gameObject.activeInHierarchy,["same_render_mesh"]=c.GetComponent<MeshFilter>()&&c.sharedMesh==c.GetComponent<MeshFilter>().sharedMesh,["bounds_min"]=V(c.bounds.min),["bounds_max"]=V(c.bounds.max)}))});
   }
   var roots=UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
   var landmarks=new JArray(roots.SelectMany(r=>r.GetComponentsInChildren<Transform>(true)).Where(t=>t.name.Contains("Outside_Play")||t.name.Contains("Wake")||t.name.Contains("Bao")||t.name.Contains("Direction")||t.name.Contains("Sign")).Select(t=>new JObject{["path"]=Path(t),["position"]=V(t.position),["active"]=t.gameObject.activeInHierarchy}));
   var interactionObjects=roots.SelectMany(r=>r.GetComponentsInChildren<TuongTacHoCho>(true)).Where(i=>i.kind==TuongTacHoCho.Kind.Ha||i.kind==TuongTacHoCho.Kind.Wake).ToArray();
   var interactions=new JArray(interactionObjects.Select(i=>new JObject{["path"]=Path(i.transform),["kind"]=i.kind.ToString(),["position"]=V(i.transform.position),["colliders"]=new JArray(i.GetComponentsInChildren<Collider>(true).Select(c=>new JObject{["path"]=Path(c.transform),["min"]=V(c.bounds.min),["max"]=V(c.bounds.max),["enabled"]=c.enabled}))}));
   var ha=interactionObjects.FirstOrDefault(i=>i.kind==TuongTacHoCho.Kind.Ha);
   var nearHa=ha?new JArray(colliders.Where(c=>Vector3.Distance(c.transform.position,ha.transform.position)<10).Select(c=>new JObject{["path"]=Path(c.transform),["position"]=V(c.transform.position),["min"]=V(c.bounds.min),["max"]=V(c.bounds.max),["enabled"]=c.enabled})):new JArray();
   scenes.Add(new JObject{["revision"]=revision,["bases"]=bases,["landmarks"]=landmarks,["interactions"]=interactions,["near_ha_colliders"]=nearHa,["scene_dirty"]=UnityEngine.SceneManagement.SceneManager.GetActiveScene().isDirty});
  }
  File.WriteAllText(output+"/scene-contact-audit.json",new JObject{["method"]="Read-only imported scene collider raycasts, r14 before/r17 after; measured model footprints, not attribution to unavailable original photographs. Build image evidence recorded separately.",["scenes"]=scenes}.ToString());
 }
 static JArray V(Vector3 p)=>new JArray(p.x,p.y,p.z);
 static string Path(Transform t)=>t.parent?Path(t.parent)+"/"+t.name:t.name;
}
