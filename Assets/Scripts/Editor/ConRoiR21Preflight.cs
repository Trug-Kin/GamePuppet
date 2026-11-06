using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

// Read-only investigation of the original 23 retained origins. Never saves a scene.
public static class ConRoiR21Preflight
{
 static string Out="Docs/Reviews/journey_v48/r21/recovery-fix/preflight";
 static string Scene=ConRoiV48R21RecoveryFix.Scene;
 public static void VerifyPrep()
 {
  Out="Docs/Reviews/journey_v48/r21/recovery-prep/preflight";Scene="Assets/Scenes/JourneyRepair_v48_r21_recoveryprep.unity";Directory.CreateDirectory(Out);
  foreach(var f in new[]{"origins-survey.json","invalid-origin-approaches.json"})File.Copy(ConRoiV48R21RecoveryFix.Out+"/preflight/"+f,Out+"/"+f,true);
  CorrectedAudit();DetailInvalid();ConnectedApproach();RouteRegression();
 }
 static JArray V(Vector3 p)=>new JArray(p.x,p.y,p.z);
 static string P(Transform t)=>t.parent?P(t.parent)+"/"+t.name:t.name;
 static JObject Hit(RaycastHit h)=>new JObject{["point"]=V(h.point),["normal"]=V(h.normal),["slope_degrees"]=Vector3.Angle(h.normal,Vector3.up),["triangle"]=h.triangleIndex,["path"]=P(h.collider.transform),["mesh"]=h.collider is MeshCollider c?AssetDatabase.GetAssetPath(c.sharedMesh):""};
 public static void Survey()
 {
  Directory.CreateDirectory(Out);EditorSceneManager.OpenScene(Scene);var s=UnityEngine.Object.FindAnyObjectByType<DieuPhoiHanhTrinh>();s.hub.SetActive(false);s.lake.SetActive(true);s.market.SetActive(true);foreach(var g in s.GetComponent<CanhQuanChoV48>().scenery)if(g)g.SetActive(true);Physics.SyncTransforms();var recovery=s.GetComponent<PhucHoiTaiBienR21>();var hedge=GameObject.Find("R21_Boundary_Landscape_Finish").transform.Find("Grounded_Cypress_Boundary");
  var old=JObject.Parse(File.ReadAllText("Docs/Reviews/journey_v48/r21/recovery-fix/verification.json"));var panels=JObject.Parse(File.ReadAllText("Docs/Reviews/journey_v48/r21/asset-pass/actual-terrain-boundary.json"))["panels"];var rows=new JArray();
  foreach(var original in old["unsupported_cast_origins"]){
   string name=(string)original["panel"];float fraction=(float)original["fraction"];var f=hedge.Find(name).GetComponent<MeshFilter>();var vertices=f.sharedMesh.vertices;
   var a=vertices.Take(7).Aggregate(Vector3.zero,(v,p)=>v+p)/7;var b=vertices.Skip(vertices.Length-7).Aggregate(Vector3.zero,(v,p)=>v+p)/7;var direction=b-a;direction.y=0;direction.Normalize();var inward=-Vector3.Cross(Vector3.up,direction);var at=Vector3.Lerp(a,b,fraction);var origin=at+inward*2;
   var all=Physics.RaycastAll(new Vector3(origin.x,100,origin.z),Vector3.down,200,8,QueryTriggerInteraction.Ignore).Where(h=>!h.collider.transform.IsChildOf(hedge)&&!h.collider.GetComponentInParent<DieuKhienDiChuyen>()).OrderByDescending(h=>h.point.y).ToArray();
   var grid=new JArray();for(float x=-4;x<=4.01f;x+=.5f)for(float z=-4;z<=4.01f;z+=.5f){var q=origin+new Vector3(x,0,z);var hits=Physics.RaycastAll(new Vector3(q.x,100,q.z),Vector3.down,200,8,QueryTriggerInteraction.Ignore).Where(h=>!h.collider.transform.IsChildOf(hedge)&&!h.collider.GetComponentInParent<DieuKhienDiChuyen>()).OrderByDescending(h=>h.point.y).ToArray();grid.Add(new JObject{["xz"]=new JArray(q.x,q.z),["inside"]=recovery.Inside(new Vector3(q.x,0,q.z)),["hits"]=new JArray(hits.Select(Hit))});}
   JToken canonical=null;
   if(name.StartsWith("CypressBoundary_")){int index=int.Parse(name.Substring("CypressBoundary_".Length));var panel=panels[index];var ca=new Vector3((float)panel["a"][0],0,(float)panel["a"][2]);var cb=new Vector3((float)panel["b"][0],0,(float)panel["b"][2]);var ci=-Vector3.Cross(Vector3.up,(cb-ca).normalized);var cp=Vector3.Lerp(ca,cb,fraction)+ci*2.8f;canonical=new JObject{["a"]=V(ca),["b"]=V(cb),["inward"]=V(ci),["inward_error_degrees"]=Vector3.Angle(inward,ci),["reference_xz_only_not_replacement"]=V(cp)};}
   rows.Add(new JObject{["retained_record"]=original.DeepClone(),["exact_original_origin"]=V(origin),["old_computed_axis"]=V(direction),["old_computed_inward"]=V(inward),["reconstruction_error_m"]=Vector3.Distance(at,new Vector3((float)original["xz"][0],(float)original["xz"][1],(float)original["xz"][2])),["inside_terrain_union"]=recovery.Inside(new Vector3(origin.x,0,origin.z)),["all_ground_hits_at_unchanged_origin"]=new JArray(all.Select(Hit)),["canonical"]=canonical,["grid"]=grid});
  }
  var runtime=new JArray();foreach(var c in UnityEngine.Object.FindObjectsByType<MeshCollider>(FindObjectsInactive.Include).Where(c=>c.sharedMesh&&AssetDatabase.GetAssetPath(c.sharedMesh).StartsWith(ConRoiV48R21RecoveryFix.Art))){runtime.Add(new JObject{["object"]=P(c.transform),["mesh"]=AssetDatabase.GetAssetPath(c.sharedMesh),["prebake"]=c.sharedMesh.HasPreBakeCollisionMesh(false)});}
  File.WriteAllText(Out+"/origins-survey.json",new JObject{["method"]="Read-only fresh Editor; exact historical origins reconstructed without moving them. Full normal hits retained, local 8x8m grids. No native claim.",["rows"]=rows,["new_mesh_prebake"]=runtime}.ToString());
 }
 public static void CorrectedAudit()
 {
  EditorSceneManager.OpenScene(Scene);var s=UnityEngine.Object.FindAnyObjectByType<DieuPhoiHanhTrinh>();s.hub.SetActive(false);s.lake.SetActive(true);s.market.SetActive(true);foreach(var g in s.GetComponent<CanhQuanChoV48>().scenery)if(g)g.SetActive(true);Physics.SyncTransforms();
  var hedge=GameObject.Find("R21_Boundary_Landscape_Finish").transform.Find("Grounded_Cypress_Boundary");var panels=JObject.Parse(File.ReadAllText("Docs/Reviews/journey_v48/r21/asset-pass/actual-terrain-boundary.json"))["panels"];var casts=new JArray();var invalid=new JArray();int index=0;
  if(Scene.Contains("recoveryprep")){foreach(var join in JObject.Parse(File.ReadAllText("Docs/Reviews/journey_v48/r21/recovery-prep/hedge-authoring.json"))["connectors"]){var p=(JObject)join.DeepClone();p["name"]="CypressConnector_after_"+(string)p["after_panel"];((JArray)panels).Add(p);}}
  foreach(var p in panels){string name=(string)p["name"]??"CypressBoundary_"+index++;var a=new Vector3((float)p["a"][0],0,(float)p["a"][2]);var b=new Vector3((float)p["b"][0],0,(float)p["b"][2]);var inward=-Vector3.Cross(Vector3.up,(b-a).normalized);
   foreach(float fraction in new[]{0f,.5f,1f}){var at=Vector3.Lerp(a,b,fraction);var origin=at+inward*2.8f;var floors=Physics.RaycastAll(new Vector3(origin.x,100,origin.z),Vector3.down,200,8,QueryTriggerInteraction.Ignore).Where(h=>h.normal.y>=Mathf.Cos(45*Mathf.Deg2Rad)&&!h.collider.transform.IsChildOf(hedge)&&!h.collider.GetComponentInParent<DieuKhienDiChuyen>()).OrderByDescending(h=>h.point.y).ToArray();
    if(floors.Length==0){invalid.Add(new JObject{["panel"]=name,["fraction"]=fraction,["origin_xz"]=new JArray(origin.x,origin.z),["status"]="NO_WALKABLE_SUPPORT_NOT_A_PASS"});continue;}
    foreach(float rise in new[]{0f,1.5f,2.95f}){var centre=floors[0].point+Vector3.up*(.93f+rise);var hits=Physics.CapsuleCastAll(centre-Vector3.up*.6f,centre+Vector3.up*.6f,.3f,-inward,6,8,QueryTriggerInteraction.Ignore);var fence=hits.Where(h=>h.collider.transform.IsChildOf(hedge)).OrderBy(h=>h.distance).ToArray();casts.Add(new JObject{["panel"]=name,["fraction"]=fraction,["origin"]=V(centre),["rise"]=rise,["support"]=Hit(floors[0]),["hedge_hit"]=fence.Length>0?Hit(fence[0]):null,["status"]=fence.Length>0?"BLOCKED_BY_VISIBLE_HEDGE":"UNRESOLVED"});}
   }
  }
  var testGuids=new[]{AssetDatabase.AssetPathToGUID("Assets/DaLatMovementTest_20261013/DaLatRayMissProbe.cs"),AssetDatabase.AssetPathToGUID("Assets/WRouteC3Test_v30_20261016/C3ActualPlayRun.cs")};var source=File.ReadAllText(ConRoiV48R21RecoveryFix.Scene);
  File.WriteAllText(Out+"/corrected-boundary-audit.json",new JObject{["method"]="Authoritative terrain perimeter endpoints; fixed 2.8m inward, no sample search or relocation; capsule casts must hit visible hedge specifically. Editor only, not native acceptance. Original 23 retained separately.",["casts"]=casts,["invalid_origins"]=invalid,["test_script_scene_references"]=new JArray(testGuids.Select(g=>new JObject{["guid"]=g,["present"]=source.Contains(g)}))}.ToString());
 }
 public static void DetailInvalid()
 {
  EditorSceneManager.OpenScene(Scene);var s=UnityEngine.Object.FindAnyObjectByType<DieuPhoiHanhTrinh>();s.hub.SetActive(false);s.lake.SetActive(true);s.market.SetActive(true);foreach(var g in s.GetComponent<CanhQuanChoV48>().scenery)if(g)g.SetActive(true);Physics.SyncTransforms();var recovery=s.GetComponent<PhucHoiTaiBienR21>();var hedge=GameObject.Find("R21_Boundary_Landscape_Finish").transform.Find("Grounded_Cypress_Boundary");
  var old=JObject.Parse(File.ReadAllText(Out+"/origins-survey.json"));var corrected=JObject.Parse(File.ReadAllText(Out+"/corrected-boundary-audit.json"));var rows=new JArray();
  var points=old["rows"].Select(r=>new{label="historical:"+(string)r["retained_record"]["panel"]+":"+(string)r["retained_record"]["fraction"],p=new Vector3((float)r["exact_original_origin"][0],0,(float)r["exact_original_origin"][2])}).Concat(corrected["invalid_origins"].Select(r=>new{label="corrected:"+(string)r["panel"]+":"+(string)r["fraction"],p=new Vector3((float)r["origin_xz"][0],0,(float)r["origin_xz"][1])}));
  foreach(var item in points){var p=item.p;var hits=Physics.RaycastAll(p+Vector3.up*100,Vector3.down,200,8,QueryTriggerInteraction.Ignore).Where(h=>!h.collider.transform.IsChildOf(hedge)&&!h.collider.GetComponentInParent<DieuKhienDiChuyen>()).OrderByDescending(h=>h.point.y).ToArray();var approaches=new JArray();int originsInHedge=0;
   for(float x=-4;x<=4.01f;x+=.5f)for(float z=-4;z<=4.01f;z+=.5f){var q=p+new Vector3(x,0,z);if(!recovery.Inside(q))continue;var floors=Physics.RaycastAll(q+Vector3.up*100,Vector3.down,200,8,QueryTriggerInteraction.Ignore).Where(h=>h.normal.y>=Mathf.Cos(45*Mathf.Deg2Rad)&&!h.collider.transform.IsChildOf(hedge)&&!h.collider.GetComponentInParent<DieuKhienDiChuyen>()).OrderByDescending(h=>h.point.y).ToArray();if(floors.Length==0)continue;var centre=floors[0].point+Vector3.up*.93f;
    if(Physics.OverlapCapsule(centre-Vector3.up*.6f,centre+Vector3.up*.6f,.3f,8,QueryTriggerInteraction.Ignore).Any(c=>c.transform.IsChildOf(hedge))){originsInHedge++;continue;}
    var delta=p-q;delta.y=0;if(delta.magnitude<.01f)continue;var barriers=Physics.CapsuleCastAll(centre-Vector3.up*.6f,centre+Vector3.up*.6f,.3f,delta.normalized,delta.magnitude,8,QueryTriggerInteraction.Ignore).Where(h=>h.collider.transform.IsChildOf(hedge)).ToArray();approaches.Add(new JObject{["start"]=V(centre),["support"]=Hit(floors[0]),["hedge_separates"]=barriers.Length>0,["barrier"]=barriers.Length>0?Hit(barriers.OrderBy(h=>h.distance).First()):null});
   }
   rows.Add(new JObject{["label"]=item.label,["unchanged_xz"]=new JArray(p.x,p.z),["inside"]=recovery.Inside(p),["ground"]=new JArray(hits.Select(Hit)),["grid_candidates_inside_hedge"]=originsInHedge,["approaches"]=approaches});
  }
  File.WriteAllText(Out+"/invalid-origin-detail.json",new JObject{["method"]="Exact retained coordinates, independent fixed local support grid. Approach rays are geometry witnesses only, not native traversal. Samples never relocated.",["rows"]=rows}.ToString());
 }
 public static void ApproachInvalid()
 {
  EditorSceneManager.OpenScene(Scene);var s=UnityEngine.Object.FindAnyObjectByType<DieuPhoiHanhTrinh>();s.hub.SetActive(false);s.lake.SetActive(true);s.market.SetActive(true);foreach(var g in s.GetComponent<CanhQuanChoV48>().scenery)if(g)g.SetActive(true);Physics.SyncTransforms();
  var input=JObject.Parse(File.ReadAllText(Out+"/invalid-origin-detail.json"));var rows=new JArray();var walk=typeof(ConRoiV48R21RecoveryFix).GetMethod("Walk",System.Reflection.BindingFlags.Static|System.Reflection.BindingFlags.NonPublic);
  foreach(var r in input["rows"]){var target=new Vector3((float)r["unchanged_xz"][0],0,(float)r["unchanged_xz"][1]);var tests=new JArray();foreach(var a in r["approaches"].Where(a=>!(bool)a["hedge_separates"])){var q=a["start"];var start=new Vector3((float)q[0],(float)q[1],(float)q[2]);var result=(JObject)walk.Invoke(null,new object[]{s.player,start,new[]{target},(string)r["label"]});var e=result["end"];result["end_inside"]=s.GetComponent<PhucHoiTaiBienR21>().Inside(new Vector3((float)e[0],(float)e[1],(float)e[2]));tests.Add(result);}rows.Add(new JObject{["label"]=r["label"],["approach_tests"]=tests});}
  File.WriteAllText(Out+"/invalid-origin-approaches.json",new JObject{["method"]="No-jump Editor controller solver using unchanged Player parameters, fixed retained target and every local support start without a hedge cast hit. No recovery active, no native claim.",["rows"]=rows}.ToString());
 }
 public static void ConnectedApproach()
 {
  EditorSceneManager.OpenScene(Scene);var s=UnityEngine.Object.FindAnyObjectByType<DieuPhoiHanhTrinh>();s.hub.SetActive(false);s.lake.SetActive(true);s.market.SetActive(true);foreach(var g in s.GetComponent<CanhQuanChoV48>().scenery)if(g)g.SetActive(true);Physics.SyncTransforms();
  var hedge=GameObject.Find("R21_Boundary_Landscape_Finish").transform.Find("Grounded_Cypress_Boundary");var input=JObject.Parse(File.ReadAllText(Out+"/invalid-origin-approaches.json"));var panels=JObject.Parse(File.ReadAllText("Docs/Reviews/journey_v48/r21/asset-pass/actual-terrain-boundary.json"))["panels"];var walk=typeof(ConRoiV48R21RecoveryFix).GetMethod("Walk",System.Reflection.BindingFlags.Static|System.Reflection.BindingFlags.NonPublic);var rows=new JArray();
  foreach(var r in input["rows"]){string label=(string)r["label"];var bits=label.Split(':');Vector3 a,b;if(bits[1].StartsWith("CypressBoundary_")){int index=int.Parse(bits[1].Substring("CypressBoundary_".Length));var p=panels[index];a=new Vector3((float)p["a"][0],0,(float)p["a"][2]);b=new Vector3((float)p["b"][0],0,(float)p["b"][2]);}else{var gap=JObject.Parse(File.ReadAllText("Docs/Reviews/journey_v48/r21/asset-pass/actual-terrain-boundary.json"))["unsupported_midpoints"];var centre=gap.Select(q=>new Vector3((float)q[0],0,(float)q[1])).Aggregate(Vector3.zero,(v,p)=>v+p)/gap.Count();a=centre-Vector3.right*.8f;b=centre+Vector3.right*.8f;}
   var inward=-Vector3.Cross(Vector3.up,(b-a).normalized);var mid=(a+b)*.5f;var q=mid+inward*6;var floors=Physics.RaycastAll(q+Vector3.up*100,Vector3.down,200,8,QueryTriggerInteraction.Ignore).Where(h=>h.normal.y>=Mathf.Cos(45*Mathf.Deg2Rad)&&!h.collider.transform.IsChildOf(hedge)&&!h.collider.GetComponentInParent<DieuKhienDiChuyen>()).OrderByDescending(h=>h.point.y).ToArray();var tests=new JArray();
   if(floors.Length>0){var start=floors[0].point+Vector3.up*.94f;foreach(var t in r["approach_tests"]){var e=t["end"];var target=new Vector3((float)e[0],(float)e[1],(float)e[2]);var original=t["start"];var waypoint=new Vector3((float)original[0],(float)original[1],(float)original[2]);var result=(JObject)walk.Invoke(null,new object[]{s.player,start,new[]{waypoint,target},label});result["outer_seed_signed_inward_m"]=Vector3.Dot(waypoint-mid,inward);result["inner_start_signed_inward_m"]=Vector3.Dot(start-mid,inward);tests.Add(result);}}
   rows.Add(new JObject{["label"]=label,["fixed_inner_xz"]=V(q),["inner_floor"]=floors.Length>0?Hit(floors[0]):null,["inner_supported_margin2"]=floors.Length>0&&s.GetComponent<PhucHoiTaiBienR21>().Supported(floors[0].point+Vector3.up*.94f,2),["tests"]=tests});
  }
  File.WriteAllText(Out+"/connected-approaches.json",new JObject{["method"]="Two-stage no-jump walks from fixed 6m inward support via each suspect local seed to its previous endpoint. Seed is not assumed reachable. No search or relocation of historical targets. Editor only.",["rows"]=rows}.ToString());
 }
 static void RouteRegression()
 {
  var s=UnityEngine.Object.FindAnyObjectByType<DieuPhoiHanhTrinh>();var walk=typeof(ConRoiV48R21RecoveryFix).GetMethod("Walk",System.Reflection.BindingFlags.Static|System.Reflection.BindingFlags.NonPublic);var replay=typeof(ConRoiV48R21RecoveryFix).GetMethod("Replay",System.Reflection.BindingFlags.Static|System.Reflection.BindingFlags.NonPublic);var author=JObject.Parse(File.ReadAllText(ConRoiV48R21RecoveryFix.Out+"/authoring.json"));var walks=new JArray();
  foreach(var t in author["trails"]){var route=t["route"].Select(q=>new Vector3((float)q[0],(float)q[1],(float)q[2])).ToArray();walks.Add((JObject)walk.Invoke(null,new object[]{s.player,route[0]+Vector3.up*.94f,route,"forward_"+t["side"]}));walks.Add((JObject)walk.Invoke(null,new object[]{s.player,route[^1]+Vector3.up*.94f,route.Reverse().ToArray(),"reverse_"+t["side"]}));}
  foreach(var endpoint in new[]{new Vector3(627.923584f,0,546.096924f),new Vector3(698.958f,0,485.638f)})walks.Add((JObject)walk.Invoke(null,new object[]{s.player,new Vector3(656.096435f,.081721f,498.773804f),new[]{new Vector3(652.26f,0,502.47f),new Vector3(654.8f,0,505.3f),endpoint},"old_stuck_approach"}));
  var jumps=new JArray();foreach(float hz in new[]{30f,60f,120f})jumps.Add((JObject)replay.Invoke(null,new object[]{s.player,new Vector3(698.950012f,-5.450142f,588.416931f),Vector3.forward,new[]{1.4f,2f,1.4f,1f},hz,true}));
  File.WriteAllText(Out+"/route-regression.json",new JObject{["method"]="Editor solver only, unchanged player parameters; native NOT_RUN",["walks"]=walks,["two_jump_replays"]=jumps}.ToString());
 }
 public static void FinalAuditAndImages(){VerifyPrep();CompareImages();}
 static void CompareImages()
 {
  string images="Docs/Reviews/journey_v48/r21/recovery-prep/images";Directory.CreateDirectory(images);
  foreach(var entry in new[]{new[]{"before",ConRoiV48R21RecoveryFix.Scene},new[]{"after",Scene}}){EditorSceneManager.OpenScene(entry[1]);var s=UnityEngine.Object.FindAnyObjectByType<DieuPhoiHanhTrinh>();s.hub.SetActive(false);s.lake.SetActive(true);s.market.SetActive(true);foreach(var g in s.GetComponent<CanhQuanChoV48>().scenery)if(g)g.SetActive(true);Physics.SyncTransforms();var eye=UnityEngine.Object.Instantiate(s.player.GetComponentInChildren<Camera>());eye.transform.SetParent(null);eye.gameObject.SetActive(true);eye.enabled=false;eye.fieldOfView=65;RenderSettings.fog=false;
   foreach(var view in new[]{("panel633",new Vector3(480,20,577),new Vector3(468,14,588)),("panel676",new Vector3(482,5,501),new Vector3(472,0,508)),("panel676-low",new Vector3(479,-6,504),new Vector3(471,-5,508))}){eye.transform.SetPositionAndRotation(view.Item2,Quaternion.LookRotation(view.Item3-view.Item2));var rt=RenderTexture.GetTemporary(1280,720,24);var tex=new Texture2D(1280,720,TextureFormat.RGB24,false);for(int i=0;i<3;i++)UnityEngine.Rendering.RenderPipeline.SubmitRenderRequest(eye,new UnityEngine.Rendering.Universal.UniversalRenderPipeline.SingleCameraRequest{destination=rt});var old=RenderTexture.active;RenderTexture.active=rt;tex.ReadPixels(new Rect(0,0,1280,720),0,0);tex.Apply();RenderTexture.active=old;File.WriteAllBytes(images+"/"+entry[0]+"-"+view.Item1+".png",tex.EncodeToPNG());RenderTexture.ReleaseTemporary(rt);UnityEngine.Object.DestroyImmediate(tex);}
   UnityEngine.Object.DestroyImmediate(eye.gameObject);
  }
 }
}
