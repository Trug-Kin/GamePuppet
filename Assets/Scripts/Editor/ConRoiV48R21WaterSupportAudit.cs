using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public static class ConRoiV48R21WaterSupportAudit
{
 static string PathOf(Transform t)=>t.parent?PathOf(t.parent)+"/"+t.name:t.name;
 public static void Audit()
 {
  EditorSceneManager.OpenScene("Assets/Scenes/JourneyRepair_v48_r21.unity");
  var story=UnityEngine.Object.FindAnyObjectByType<DieuPhoiHanhTrinh>();story.player.gameObject.SetActive(false);story.hub.SetActive(false);story.lake.SetActive(true);story.market.SetActive(true);
  foreach(var g in story.GetComponent<CanhQuanChoV48>().scenery)g.SetActive(true);
  Physics.SyncTransforms();var rows=new JArray();
  foreach(var p in new[]{new Vector3(600,0,534),new Vector3(460,0,480),new Vector3(570,0,490),new Vector3(480,0,512),new Vector3(612,0,538),new Vector3(627,0,556),new Vector3(626,0,556)})
  {
   var hits=Physics.RaycastAll(new Ray(p+Vector3.up*45,Vector3.down),100,8,QueryTriggerInteraction.Ignore).OrderByDescending(h=>h.point.y);
   rows.Add(new JObject{["x"]=p.x,["z"]=p.z,["hits"]=new JArray(hits.Select(h=>new JObject{["path"]=PathOf(h.collider.transform),["y"]=h.point.y,["normal_y"]=h.normal.y,["mesh"]=h.collider is MeshCollider mc?AssetDatabase.GetAssetPath(mc.sharedMesh):h.collider.GetType().Name,["renderer_enabled"]=h.collider.GetComponent<Renderer>()&&h.collider.GetComponent<Renderer>().enabled}))});
  }
  File.WriteAllText("Docs/Reviews/journey_v48/r21/water-support-attribution.json",new JObject{["status"]="READ_ONLY_EDITOR_PHYSICS_ATTRIBUTION_NOT_NATIVE_EVIDENCE",["points"]=rows}.ToString());
 }
 public static void AuditThenRepairLinks()
 {
  Audit();ConRoiV48R21BridgeLinks.RepairAndBuildDraft();
 }
}
