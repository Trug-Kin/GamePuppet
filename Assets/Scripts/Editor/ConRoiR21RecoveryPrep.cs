using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

// A new derivative for the API-warning fix. Neither frozen binary is a build target.
public static class ConRoiR21RecoveryPrep
{
 const string Out="Docs/Reviews/journey_v48/r21/recovery-prep";
 const string Scene="Assets/Scenes/JourneyRepair_v48_r21_recoveryprep.unity";
 public static void AuthorAndVerify()
 {
  Directory.CreateDirectory(Out);
  if(File.Exists(Scene))throw new Exception("Retain existing derivative scene");
  if(!AssetDatabase.CopyAsset(ConRoiV48R21RecoveryFix.Scene,Scene))throw new Exception("New derivative scene copy failed");
  ConRoiR21HedgeContinuity.Author();ConRoiR21Preflight.VerifyPrep();
 }
 static JObject RecoveryTests(DieuPhoiHanhTrinh s,PhucHoiTaiBienR21 r)
 {
  s.data.region=0;s.data.nextRegion=1;s.data.baoTalks=7;s.data.traces[0]=true;s.data.witnessed[0]=true;var before=JsonUtility.ToJson(s.data);var anchor=r.Supported(r.verifiedFallback,1);var inside=r.Inside(r.verifiedFallback,2);var outside=!r.Inside(new Vector3(698.9256f,-76.998f,595.8746f));
  var path=Path.GetFullPath(Out+"/recovery-good-save-test.json");s.SavePathOverride=path;s.Place(r.verifiedFallback,Quaternion.identity);s.Save();var good=File.ReadAllBytes(path);s.Place(new Vector3(698.9256f,-76.998f,595.8746f),Quaternion.identity);bool refused=!r.AllowSave();s.Save();bool blockedUnchanged=good.SequenceEqual(File.ReadAllBytes(path));bool recovered=r.Recover("editor_regression_void");bool progress=s.data.baoTalks==7&&s.data.traces[0]&&s.data.witnessed[0]&&s.data.nextRegion==1;bool position=Vector3.Distance(s.player.transform.position,r.verifiedFallback)<.001f;s.Save();bool afterRecoveryUnchanged=good.SequenceEqual(File.ReadAllBytes(path));
  var stateBefore=JsonUtility.ToJson(s.data);s.Place(new Vector3(698.9256f,-76.998f,595.8746f),Quaternion.identity);r.AfterLoad();typeof(PhucHoiTaiBienR21).GetMethod("LateUpdate",System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Instance).Invoke(r,null);bool loadedRecovery=Vector3.Distance(s.player.transform.position,r.verifiedFallback)<.001f&&r.Recoveries==2;bool entireStateRetained=stateBefore==JsonUtility.ToJson(s.data);bool loadedSaveUnchanged=good.SequenceEqual(File.ReadAllBytes(path));
  s.Place(r.verifiedFallback,Quaternion.Euler(0,31,0));r.AfterLoad();typeof(PhucHoiTaiBienR21).GetMethod("LateUpdate",System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Instance).Invoke(r,null);bool validLoadUnchanged=Quaternion.Angle(s.player.transform.rotation,Quaternion.Euler(0,31,0))<.001f&&r.Recoveries==2;
  var result=new JObject{["anchor_supported"]=anchor,["anchor_inside"]=inside,["void_outside"]=outside,["invalid_save_refused"]=refused,["good_save_bytes_unchanged_while_invalid"]=blockedUnchanged,["recovered"]=recovered,["safe_position"]=position,["progress_retained"]=progress,["good_save_bytes_unchanged_after_recovery"]=afterRecoveryUnchanged,["loaded_invalid_pose_recovers"]=loadedRecovery,["entire_state_retained"]=entireStateRetained,["good_save_bytes_unchanged_after_invalid_load_recovery"]=loadedSaveUnchanged,["valid_loaded_pose_orientation_unchanged"]=validLoadUnchanged,["test_save_path"]=path,["method"]="Direct Editor component integration, Save/Place/Recover/AfterLoad callback; isolated save under workspace; native F5/F6 still NOT_RUN"};s.SavePathOverride=null;return result;
 }
 public static void VerifyRecovery()
 {
  EditorSceneManager.OpenScene(Scene);var s=UnityEngine.Object.FindAnyObjectByType<DieuPhoiHanhTrinh>();s.hub.SetActive(false);s.lake.SetActive(true);s.market.SetActive(true);foreach(var g in s.GetComponent<CanhQuanChoV48>().scenery)if(g)g.SetActive(true);Physics.SyncTransforms();
  var r=s.GetComponent<PhucHoiTaiBienR21>();var result=RecoveryTests(s,r);int count=r.Recoveries;var callback=typeof(PhucHoiTaiBienR21).GetMethod("LateUpdate",System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Instance);
  for(int i=0;i<120;i++)callback.Invoke(r,null);
  result["no_repeat_in_120_editor_callbacks"]=r.Recoveries==count;
  result["repeat_test_limit"]="Editor callbacks, not 120 native frames; 15s foreground watch still NOT_RUN";
  File.WriteAllText(Out+"/preflight/recovery-integration.json",result.ToString());
 }
 public static void RecoveryAndBuild(){VerifyRecovery();Build();}
 public static void ReviseAndVerify(){ConRoiR21HedgeContinuity.Author();ConRoiR21Preflight.FinalAuditAndImages();}
 public static void Build()
 {
  Directory.CreateDirectory(Out);
  var audit=JObject.Parse(File.ReadAllText(Out+"/preflight/corrected-boundary-audit.json"));
  if(audit["casts"].Any(c=>(string)c["status"]!="BLOCKED_BY_VISIBLE_HEDGE"))throw new Exception("Unresolved corrected boundary casts; no build");
  var routes=JObject.Parse(File.ReadAllText(Out+"/preflight/route-regression.json"));
  if(routes["walks"].Any(w=>(int)w["reached"]!=(int)w["targets"]))throw new Exception("Route regression failed");
  var integration=JObject.Parse(File.ReadAllText(Out+"/preflight/recovery-integration.json"));if(integration.Children<JProperty>().Any(p=>p.Value.Type==JTokenType.Boolean&&!(bool)p.Value))throw new Exception("Recovery integration failed");
  var approaches=JObject.Parse(File.ReadAllText(Out+"/preflight/connected-approaches.json"));if(approaches["rows"].Where(r=>(bool)r["inner_supported_margin2"]).SelectMany(r=>r["tests"]).Any(t=>(float)t["end"][1]<-25))throw new Exception("Reachable unsupported fall remains");
  if(Directory.Exists("Builds/Windows_v48_r21_recoveryprep"))throw new Exception("Never overwrite a retained binary");
  EditorSceneManager.OpenScene(Scene);
  var s=UnityEngine.Object.FindAnyObjectByType<DieuPhoiHanhTrinh>();
  if(s.player.controller.stepOffset!=.3f||s.player.controller.height!=1.8f||s.player.controller.radius!=.3f||s.player.controller.slopeLimit!=45||s.player.jumpHeight!=3||s.player.gravity!=-19.62f)throw new Exception("Controller changed");
  var testComponents=UnityEngine.Object.FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include).Where(c=>c&&(c is DaLatRayMissProbe||c is C3ActualPlayRun)).Select(c=>c.GetType().Name).ToArray();
  if(testComponents.Length>0)throw new Exception("Test driver in derivative scene");
  var meshAudit=new JArray();foreach(var f in s.GetComponent<CanhQuanChoV48>().scenery.Where(g=>g&&g.name=="R21_Boundary_Landscape_Finish").SelectMany(g=>g.GetComponentsInChildren<MeshFilter>(true))){var collider=f.GetComponent<MeshCollider>();var mesh=f.sharedMesh;if(f.name.StartsWith("FootwayOverShoulder_"))continue;var v=mesh.vertices;var t=mesh.triangles;int degenerate=0;for(int i=0;i<t.Length;i+=3)if(Vector3.Cross(v[t[i+1]]-v[t[i]],v[t[i+2]]-v[t[i]]).sqrMagnitude<1e-12f)degenerate++;bool ok=collider&&collider.sharedMesh==mesh&&mesh.HasPreBakeCollisionMesh(false)&&degenerate==0;meshAudit.Add(new JObject{["name"]=f.name,["mesh"]=AssetDatabase.GetAssetPath(mesh),["prebake"]=mesh.HasPreBakeCollisionMesh(false),["degenerate"]=degenerate,["ok"]=ok});}
  File.WriteAllText(Out+"/preflight/mesh-audit.json",meshAudit.ToString());if(meshAudit.Any(m=>!(bool)m["ok"]))throw new Exception("Mesh/collider issues");
  var report=BuildPipeline.BuildPlayer(new[]{Scene},"Builds/Windows_v48_r21_recoveryprep/ConRoi.exe",BuildTarget.StandaloneWindows64,BuildOptions.Development);
  var messages=new JArray();foreach(var step in report.steps)foreach(var m in step.messages)if(m.type==LogType.Warning||m.type==LogType.Error||m.type==LogType.Exception)messages.Add(new JObject{["step"]=step.name,["type"]=m.type.ToString(),["message"]=m.content});
  File.WriteAllText(Out+"/build.json",new JObject{["result"]=report.summary.result.ToString(),["errors"]=report.summary.totalErrors,["warnings"]=report.summary.totalWarnings,["messages"]=messages,["scene"]=Scene,["binary"]="Builds/Windows_v48_r21_recoveryprep/ConRoi.exe",["native_status"]="NOT_RUN_NOT_CANDIDATE"}.ToString());
  if(report.summary.result!=UnityEditor.Build.Reporting.BuildResult.Succeeded)throw new Exception("Build failed");
 }
}
