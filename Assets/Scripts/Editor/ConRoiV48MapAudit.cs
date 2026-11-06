using System;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using Newtonsoft.Json.Linq;

public static class ConRoiV48MapAudit
{
 const string Out="Docs/Reviews/journey_v48/r11";
 static string PathOf(Transform t)=>t.parent?PathOf(t.parent)+"/"+t.name:t.name;
 static JArray V(Vector3 p)=>new JArray(p.x,p.y,p.z);
 public static void Inspect()
 {
  Directory.CreateDirectory(Out);EditorSceneManager.OpenScene("Assets/Scenes/JourneyRepair_v48_r10.unity");
  var all=UnityEngine.Object.FindObjectsByType<Transform>(FindObjectsInactive.Include,FindObjectsSortMode.None);
  var s=UnityEngine.Object.FindAnyObjectByType<DieuPhoiHanhTrinh>();
  var renderers=all.SelectMany(t=>t.GetComponents<Renderer>()).ToArray();
  File.WriteAllText(Out+"/r10-runtime-scene-geometry.json",new JObject{
   ["renderers"]=new JArray(renderers.Select(r=>new JObject{["path"]=PathOf(r.transform),["enabled"]=r.enabled,["active"]=r.gameObject.activeInHierarchy,["min"]=V(r.bounds.min),["max"]=V(r.bounds.max),["position"]=V(r.transform.position),["scale"]=V(r.transform.lossyScale),["mesh"]=r.GetComponent<MeshFilter>()?AssetDatabase.GetAssetPath(r.GetComponent<MeshFilter>().sharedMesh):"",["materials"]=new JArray(r.sharedMaterials.Select(m=>m?AssetDatabase.GetAssetPath(m):"null"))})),
   ["bao"]=new JArray(s.lake.GetComponentsInChildren<Transform>(true).Where(t=>PathOf(t).Contains("Bao")||t.name.Contains("Fishing")||t.name.Contains("Reflection")).Select(t=>new JObject{["path"]=PathOf(t),["position"]=V(t.position),["rotation"]=V(t.eulerAngles),["scale"]=V(t.lossyScale),["colliders"]=new JArray(t.GetComponents<Collider>().Select(c=>c.bounds.ToString("F4")))}))
  }.ToString());
  Physics.SyncTransforms();var probes=new JArray();
  // Local support survey at both bridge ends, island and park routes, never move assets.
  for(float x=575;x<=760;x+=2.5f)for(float z=420;z<=580;z+=2.5f)
  {
   var hits=Physics.RaycastAll(new Vector3(x,80,z),Vector3.down,120,~0,QueryTriggerInteraction.Ignore).Where(h=>h.normal.y>.4f&&!h.collider.transform.IsChildOf(s.player.transform)).OrderByDescending(h=>h.point.y).ToArray();
   if(hits.Length>0)probes.Add(new JObject{["x"]=x,["z"]=z,["hits"]=new JArray(hits.Select(h=>new JObject{["name"]=h.collider.name,["y"]=h.point.y,["mesh"]=h.collider is MeshCollider c?AssetDatabase.GetAssetPath(c.sharedMesh):"primitive"}))});
  }
  File.WriteAllText(Out+"/r10-support-survey.json",probes.ToString());
  Debug.Log("V48_MAP_AUDIT_COMPLETE");
 }
}
