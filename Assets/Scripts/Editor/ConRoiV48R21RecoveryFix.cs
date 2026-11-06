using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public static class ConRoiV48R21RecoveryFix
{
 public const string Source="Assets/Scenes/JourneyRepair_v48_r21.unity", Scene="Assets/Scenes/JourneyRepair_v48_r21_recoveryfix.unity";
 public const string Out="Docs/Reviews/journey_v48/r21/recovery-fix", Art="Assets/Art/Environment/JourneyV48/RecoveryFixR21";
 static JArray V(Vector3 p)=>new JArray(p.x,p.y,p.z);
 static string P(Transform t)=>t.parent?P(t.parent)+"/"+t.name:t.name;
 static DieuPhoiHanhTrinh Open(string scene){Directory.CreateDirectory(Out);EditorSceneManager.OpenScene(scene);var s=UnityEngine.Object.FindAnyObjectByType<DieuPhoiHanhTrinh>();s.lake.SetActive(true);s.market.SetActive(true);s.hub.SetActive(false);foreach(var g in s.GetComponent<CanhQuanChoV48>().scenery)if(g)g.SetActive(true);Physics.SyncTransforms();return s;}
 static JObject Hit(RaycastHit h)=>new JObject{["point"]=V(h.point),["normal"]=V(h.normal),["triangle"]=h.triangleIndex,["path"]=P(h.collider.transform),["mesh"]=h.collider is MeshCollider m?AssetDatabase.GetAssetPath(m.sharedMesh):""};
 public static void Author()
 {
  Directory.CreateDirectory(Out);if(File.Exists(Scene))throw new Exception("Derivative already exists; use explicit reauthor, never replace baseline");
  File.Copy(Source,Out+"/baseline-scene.unity",false);AssetDatabase.CopyAsset(Source,Scene);AssetDatabase.Refresh();
  Reauthor();
 }
 public static void Reauthor(){ConRoiV48R21RecoveryGeometry.Author();ConRoiV48R21RecoveryGeometry.OverlayFootways();SmoothToeTerrain();AttachRecovery();}
 public static void ResetAndVerify(){EditorSceneManager.OpenScene(Source);EditorSceneManager.SaveScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene(),Scene);Reauthor();Verify();}
 public static void ReauthorAndVerify(){Reauthor();Verify();}
 static void SmoothToeTerrain()
 {
  var s=Open(Scene);var root=GameObject.Find("R21_Boundary_Landscape_Finish");var author=JObject.Parse(File.ReadAllText(Out+"/authoring.json"));var toes=author["trails"].SelectMany(t=>t["sections"]).SelectMany(q=>new[]{q["toe_left"],q["toe_right"]}).Select(q=>new Vector3((float)q[0],(float)q[1]-.025f,(float)q[2])).ToArray();
  var bins=new Dictionary<Vector2Int,List<Vector3>>();foreach(var q in toes){var key=new Vector2Int(Mathf.FloorToInt(q.x/2),Mathf.FloorToInt(q.z/2));if(!bins.ContainsKey(key))bins[key]=new List<Vector3>();bins[key].Add(q);}
  var rows=new JArray();var terrainRoot=GameObject.Find("R20_Whole_Source_Export");var context=GameObject.Find("LakeGateMarketV32");var targets=terrainRoot.GetComponentsInChildren<MeshFilter>(true).Where(f=>f.name=="Surface_142"||f.name=="Surface_176").Concat(context.GetComponentsInChildren<MeshFilter>(true).Where(f=>f.name=="Context_106"));
  foreach(var f in targets){var mesh=UnityEngine.Object.Instantiate(f.sharedMesh);mesh.name="ToeGrade_"+f.name;var vs=mesh.vertices;int changed=0;
   for(int i=0;i<vs.Length;i++){var p=f.transform.TransformPoint(vs[i]);float y=p.y;var key=new Vector2Int(Mathf.FloorToInt(p.x/2),Mathf.FloorToInt(p.z/2));for(int x=-4;x<=4;x++)for(int z=-4;z<=4;z++)if(bins.TryGetValue(key+new Vector2Int(x,z),out var points))foreach(var q in points){float d=new Vector2(q.x-p.x,q.z-p.z).magnitude;if(d<=8)y=Mathf.Max(y,q.y-.32f*d);}if(y>p.y+.0001f){p.y=y;vs[i]=f.transform.InverseTransformPoint(p);changed++;}}
   mesh.vertices=vs;mesh.RecalculateNormals();mesh.RecalculateBounds();mesh.SetPreBakeCollisionMesh(false,true);string path=Art+"/"+mesh.name+".asset";var asset=AssetDatabase.LoadAssetAtPath<Mesh>(path);if(asset){EditorUtility.CopySerialized(mesh,asset);UnityEngine.Object.DestroyImmediate(mesh);mesh=asset;EditorUtility.SetDirty(asset);}else AssetDatabase.CreateAsset(mesh,path);f.sharedMesh=mesh;var collider=f.GetComponent<MeshCollider>();if(collider)collider.sharedMesh=mesh;rows.Add(new JObject{["path"]=P(f.transform),["raised_vertices"]=changed,["mesh"]=path});
  }
  // Tiles are a 25mm cosmetic overlay. The continuously graded bank is the
  // structural walking collider, avoiding thousands of overlapping tile edges.
  foreach(var f in root.GetComponentsInChildren<MeshFilter>().Where(f=>f.name.StartsWith("FootwayOverShoulder_"))){var c=f.GetComponent<MeshCollider>();if(c)UnityEngine.Object.DestroyImmediate(c);}
  s.lake.SetActive(false);s.market.SetActive(false);EditorSceneManager.SaveScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene());AssetDatabase.SaveAssets();File.WriteAllText(Out+"/toe-terrain-grade.json",rows.ToString());
 }
 static void AttachRecovery()
 {
  var s=Open(Scene);var recovery=s.GetComponent<PhucHoiTaiBienR21>();if(!recovery)recovery=s.gameObject.AddComponent<PhucHoiTaiBienR21>();recovery.story=s;
  var panels=JObject.Parse(File.ReadAllText("Docs/Reviews/journey_v48/r21/asset-pass/actual-terrain-boundary.json"))["panels"];
  recovery.perimeterEdges=panels.SelectMany(p=>new[]{new Vector2((float)p["a"][0],(float)p["a"][2]),new Vector2((float)p["b"][0],(float)p["b"][2])}).ToArray();
  recovery.verifiedFallback=new Vector3(627.923584f,2.077146f,546.096924f);recovery.fallbackRotation=Quaternion.Euler(0,130,0);
  recovery.minimumWorldY=-25;s.lake.SetActive(false);s.market.SetActive(false);EditorSceneManager.SaveScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene());AssetDatabase.SaveAssets();
 }
 public static void Diagnose()
 {
  var s=Open(Source);var rows=new JArray();
  foreach(var p in new[]{new Vector3(652.26062f,.656438f,502.47f),new Vector3(698.950012f,-5.450142f,588.416931f),new Vector3(698.92572f,-2.349008f,588.628784f)}){
   var grid=new JArray();for(float x=-1;x<=1.01f;x+=.25f)for(float z=-1;z<=1.01f;z+=.25f){var q=p+new Vector3(x,0,z);grid.Add(new JObject{["xz"]=new JArray(q.x,q.z),["hits"]=new JArray(Physics.RaycastAll(new Vector3(q.x,20,q.z),Vector3.down,60,8,QueryTriggerInteraction.Ignore).OrderByDescending(h=>h.point.y).Select(Hit))});}
   rows.Add(new JObject{["position"]=V(p),["grid"]=grid,["nearby"]=new JArray(Physics.OverlapBox(p,new Vector3(2,5,2),Quaternion.identity,8,QueryTriggerInteraction.Ignore).Select(c=>new JObject{["path"]=P(c.transform),["bounds"]=c.bounds.ToString("F5")}))});
  }
  var replays=new JArray();foreach(float hz in new[]{30f,60f,120f})replays.Add(Replay(s.player,new Vector3(698.950012f,-5.450142f,588.416931f),Vector3.forward,new[]{1.4f,2f,1.4f,1f},hz,true));
  File.WriteAllText(Out+"/diagnosis-before.json",new JObject{["coordinate_system"]="Unity world XYZ, metres; telemetry and save use player.transform.position, camera yaw 0 = +Z",["player_world"]=V(s.player.transform.position),["player_local"]=V(s.player.transform.localPosition),["parent"]=s.player.transform.parent?P(s.player.transform.parent):"NONE",["controller_center"]=V(s.player.controller.center),["probes"]=rows,["two_jump_replays"]=replays}.ToString());
  Render(s,"before");
 }
 public static void Verify()
 {
  var s=Open(Scene);var recovery=s.GetComponent<PhucHoiTaiBienR21>();var author=JObject.Parse(File.ReadAllText(Out+"/authoring.json"));var walks=new JArray();
  foreach(var t in author["trails"]){var route=t["route"].Select(q=>new Vector3((float)q[0],(float)q[1],(float)q[2])).ToArray();walks.Add(Walk(s.player,route[0]+Vector3.up*.94f,route,"forward_"+t["side"]));walks.Add(Walk(s.player,route[^1]+Vector3.up*.94f,route.Reverse().ToArray(),"reverse_"+t["side"]));}
  // Native failed approach, with no jump and the unchanged original start.
  walks.Add(Walk(s.player,new Vector3(656.096435f,.081721f,498.773804f),new[]{new Vector3(652.26f,0,502.47f),new Vector3(654.8f,0,505.3f),new Vector3(638.815f,0,518.832f),new Vector3(627.923584f,0,546.096924f)},"A17_A18_then_SE_no_jump"));
  walks.Add(Walk(s.player,new Vector3(656.096435f,.081721f,498.773804f),new[]{new Vector3(652.26f,0,502.47f),new Vector3(654.8f,0,505.3f),new Vector3(681.412f,0,482.798f),new Vector3(690.702f,0,476.177f),new Vector3(698.958f,0,485.638f)},"A17_A18_then_NW_no_jump"));
  var jumps=new JArray();foreach(float hz in new[]{30f,60f,120f})jumps.Add(Replay(s.player,new Vector3(698.950012f,-5.450142f,588.416931f),Vector3.forward,new[]{1.4f,2f,1.4f,1f},hz,true));
  var oldJump=Replay(s.player,new Vector3(663.562927f,2.070060f,506.849640f),Quaternion.Euler(0,-141.8f,0)*Vector3.forward,new[]{2.2f,1f},60,true);
  var crossEntries=new JArray();foreach(var trail in author["trails"]){var sections=trail["sections"].ToArray();for(int i=12;i<sections.Length-12;i+=35){Vector3 Vec(JToken q)=>new Vector3((float)q[0],(float)q[1],(float)q[2]);var fullA=Vec(sections[i]["toe_left"]);var fullB=Vec(sections[i]["toe_right"]);var centre=Vec(sections[i]["centre"]);var a=centre+Vector3.ClampMagnitude(new Vector3(fullA.x-centre.x,0,fullA.z-centre.z),3);var b=centre+Vector3.ClampMagnitude(new Vector3(fullB.x-centre.x,0,fullB.z-centre.z),3);Vector3 Stand(Vector3 q){var hits=Physics.RaycastAll(q+Vector3.up*6,Vector3.down,12,8,QueryTriggerInteraction.Ignore).Where(h=>h.normal.y>.7f).OrderByDescending(h=>h.point.y).ToArray();return hits.Length>0?hits[0].point+Vector3.up*.94f:q+Vector3.up*.94f;}crossEntries.Add(Walk(s.player,Stand(a),new[]{centre,b},"cross_left_"+trail["side"]+"_"+i));crossEntries.Add(Walk(s.player,Stand(b),new[]{centre,a},"cross_right_"+trail["side"]+"_"+i));var exterior=(float)trail["side"]>0?fullB:fullA;crossEntries.Add(Walk(s.player,Stand(exterior),new[]{centre},"exterior_to_path_"+trail["side"]+"_"+i));}}
  var hedge=GameObject.Find("R21_Boundary_Landscape_Finish").transform.Find("Grounded_Cypress_Boundary");var casts=new JArray();var misses=new JArray();var unsupportedOrigins=new JArray();var footholds=new JArray();var replaySamples=new JArray();int index=0;
  foreach(var f in hedge.GetComponentsInChildren<MeshFilter>()){
   var bounds=f.GetComponent<Collider>().bounds;var mesh=f.sharedMesh;var vertices=mesh.vertices;int ring=7;var a=vertices.Take(ring).Aggregate(Vector3.zero,(v,p)=>v+p)/ring;var b=vertices.Skip(vertices.Length-ring).Aggregate(Vector3.zero,(v,p)=>v+p)/ring;var dir=b-a;dir.y=0;dir.Normalize();var inward=-Vector3.Cross(Vector3.up,dir);
   foreach(float fraction in new[]{0f,.5f,1f}){
    var at=Vector3.Lerp(a,b,fraction);var floorHits=Physics.RaycastAll(new Vector3(at.x,40,at.z)+inward*2,Vector3.down,100,8,QueryTriggerInteraction.Ignore).Where(h=>h.normal.y>.7f&&!h.collider.transform.IsChildOf(hedge)).OrderByDescending(h=>h.point.y).ToArray();float ground=floorHits.Length>0?floorHits[0].point.y:bounds.min.y+.2f;if(floorHits.Length==0)unsupportedOrigins.Add(new JObject{["panel"]=f.name,["fraction"]=fraction,["xz"]=V(at),["method"]="No standing origin at 2m inward. Geometry cast from mesh-base height only; native traversal NOT_RUN."});
    foreach(float rise in new[]{0f,1.5f,2.95f}){var centre=new Vector3(at.x,ground+.93f+rise,at.z)+inward*2;bool blocked=Physics.CapsuleCast(centre-Vector3.up*.6f,centre+Vector3.up*.6f,.3f,-inward,out var hit,4,8,QueryTriggerInteraction.Ignore);var row=new JObject{["panel"]=f.name,["fraction"]=fraction,["rise"]=rise,["blocked"]=blocked,["hit"]=blocked?Hit(hit):null};casts.Add(row);if(!blocked)misses.Add(row);}
    // Walkable surfaces, including visible objects, within the horizontal jump reach.
    foreach(float distance in new[]{1.5f,3f,4.5f})foreach(float along in new[]{-2f,0f,2f}){var q=at+inward*distance+dir*along;foreach(var h in Physics.RaycastAll(new Vector3(q.x,40,q.z),Vector3.down,100,8,QueryTriggerInteraction.Ignore).Where(h=>h.normal.y>.7f&&!h.collider.transform.IsChildOf(hedge)&&h.point.y>ground+.3f&&h.point.y<ground+6.1f))if(bounds.max.y-h.point.y<3.3f)footholds.Add(new JObject{["panel"]=f.name,["crown_y"]=bounds.max.y,["support"]=Hit(h)});}
   }
   if(index++%35==0){var at=(a+b)*.5f;var h=Physics.RaycastAll(new Vector3(at.x,40,at.z)+inward*2,Vector3.down,100,8,QueryTriggerInteraction.Ignore).Where(q=>q.normal.y>.7f&&!q.collider.transform.IsChildOf(hedge)).OrderByDescending(q=>q.point.y).FirstOrDefault();if(h.collider)replaySamples.Add(new JObject{["panel"]=f.name,["inward"]=V(inward),["replay"]=Replay(s.player,h.point+Vector3.up*.94f,-inward,new[]{1.4f,2f,1.4f,1f},60,true)});}
  }
  var meshIssues=new JArray();foreach(var f in GameObject.Find("R21_Boundary_Landscape_Finish").GetComponentsInChildren<MeshFilter>()){var m=f.sharedMesh;var vs=m.vertices;var ts=m.triangles;int degenerate=0;for(int i=0;i<ts.Length;i+=3)if(Vector3.Cross(vs[ts[i+1]]-vs[ts[i]],vs[ts[i+2]]-vs[ts[i]]).sqrMagnitude<1e-12f)degenerate++;var c=f.GetComponent<MeshCollider>();if(degenerate>0||(!c&&!f.name.StartsWith("FootwayOverShoulder_"))||(c&&c.sharedMesh!=m))meshIssues.Add(new JObject{["mesh"]=m.name,["degenerate"]=degenerate});}
  var recoveryTests=RecoveryTests(s,recovery);
  File.WriteAllText(Out+"/verification.json",new JObject{["method"]="Fresh Unity Editor physics, sequential settled two-jump replays and real collider queries; NOT native acceptance",["walks"]=walks,["cross_entries"]=crossEntries,["old_jump"]=oldJump,["two_jump_regression"]=jumps,["boundary_cast_count"]=casts.Count,["boundary_misses"]=misses,["unsupported_cast_origins"]=unsupportedOrigins,["nearby_footholds"]=footholds,["same_structure_replays"]=replaySamples,["mesh_issues"]=meshIssues,["recovery_tests"]=recoveryTests,["native_status"]="NOT_RUN"}.ToString());
  Render(s,"after");
 }
 static JObject Walk(DieuKhienDiChuyen p,Vector3 start,Vector3[] targets,string name)
 {
  var g=new GameObject("Temporary_NoJump_Walk");var c=g.AddComponent<CharacterController>();var probe=g.AddComponent<R21RecoveryContactProbe>();probe.controller=c;var o=p.controller;c.height=o.height;c.radius=o.radius;c.center=o.center;c.skinWidth=o.skinWidth;c.slopeLimit=o.slopeLimit;c.stepOffset=o.stepOffset;c.minMoveDistance=o.minMoveDistance;g.transform.position=start;Physics.SyncTransforms();float vy=-2;Vector3 pending=Vector3.zero,support=Vector3.up;var failures=new JArray();int reached=0;
  foreach(var target in targets){bool arrived=false;for(int i=0;i<1500;i++){var delta=target-g.transform.position;delta.y=0;if(delta.magnitude<.24f){arrived=true;break;}if(c.isGrounded&&vy<0)vy=-2;vy+=p.gravity/60;var motion=delta.normalized*p.speed/60;if(c.isGrounded&&support.y>.001f){float downhill=-(support.x*motion.x+support.z*motion.z)/support.y;if(downhill<0)motion.y=downhill;}pending+=motion+Vector3.up*vy/60;if(pending.sqrMagnitude>=c.minMoveDistance*c.minMoveDistance&&Mathf.Abs(pending.y)>=c.minMoveDistance){probe.normal=Vector3.zero;c.Move(pending);support=c.isGrounded&&probe.normal!=Vector3.zero?probe.normal:Vector3.up;pending=Vector3.zero;}}
   if(!arrived){failures.Add(new JObject{["target"]=V(target),["stopped"]=V(g.transform.position),["nearby"]=new JArray(Physics.OverlapCapsule(g.transform.position-Vector3.up*.58f,g.transform.position+Vector3.up*.58f,.4f,8,QueryTriggerInteraction.Ignore).Select(q=>P(q.transform))),["target_hits"]=new JArray(Physics.RaycastAll(target+Vector3.up*5,Vector3.down,10,8,QueryTriggerInteraction.Ignore).Select(Hit))});break;}reached++;}
  var end=g.transform.position;UnityEngine.Object.DestroyImmediate(g);return new JObject{["name"]=name,["start"]=V(start),["end"]=V(end),["targets"]=targets.Length,["reached"]=reached,["failures"]=failures};
 }
 static JObject RecoveryTests(DieuPhoiHanhTrinh s,PhucHoiTaiBienR21 r)
 {
  s.data.region=0;s.data.nextRegion=1;s.data.baoTalks=7;s.data.traces[0]=true;s.data.witnessed[0]=true;var before=JsonUtility.ToJson(s.data);var anchor=r.Supported(r.verifiedFallback,1);var inside=r.Inside(r.verifiedFallback,2);var outside=!r.Inside(new Vector3(698.9256f,-76.998f,595.8746f));
  var path=Path.GetFullPath(Out+"/recovery-good-save-test.json");s.SavePathOverride=path;s.Place(r.verifiedFallback,Quaternion.identity);s.Save();var good=File.ReadAllBytes(path);s.Place(new Vector3(698.9256f,-76.998f,595.8746f),Quaternion.identity);bool refused=!r.AllowSave();s.Save();bool blockedUnchanged=good.SequenceEqual(File.ReadAllBytes(path));bool recovered=r.Recover("editor_regression_void");bool progress=s.data.baoTalks==7&&s.data.traces[0]&&s.data.witnessed[0]&&s.data.nextRegion==1;bool position=Vector3.Distance(s.player.transform.position,r.verifiedFallback)<.001f;s.Save();bool afterRecoveryUnchanged=good.SequenceEqual(File.ReadAllBytes(path));
  var stateBefore=JsonUtility.ToJson(s.data);s.Place(new Vector3(698.9256f,-76.998f,595.8746f),Quaternion.identity);r.AfterLoad();typeof(PhucHoiTaiBienR21).GetMethod("LateUpdate",System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Instance).Invoke(r,null);bool loadedRecovery=Vector3.Distance(s.player.transform.position,r.verifiedFallback)<.001f&&r.Recoveries==2;bool entireStateRetained=stateBefore==JsonUtility.ToJson(s.data);bool loadedSaveUnchanged=good.SequenceEqual(File.ReadAllBytes(path));
  s.Place(r.verifiedFallback,Quaternion.Euler(0,31,0));r.AfterLoad();typeof(PhucHoiTaiBienR21).GetMethod("LateUpdate",System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Instance).Invoke(r,null);bool validLoadUnchanged=Quaternion.Angle(s.player.transform.rotation,Quaternion.Euler(0,31,0))<.001f&&r.Recoveries==2;
  var result=new JObject{["anchor_supported"]=anchor,["anchor_inside"]=inside,["void_outside"]=outside,["invalid_save_refused"]=refused,["good_save_bytes_unchanged_while_invalid"]=blockedUnchanged,["recovered"]=recovered,["safe_position"]=position,["progress_retained"]=progress,["good_save_bytes_unchanged_after_recovery"]=afterRecoveryUnchanged,["loaded_invalid_pose_recovers"]=loadedRecovery,["entire_state_retained"]=entireStateRetained,["good_save_bytes_unchanged_after_invalid_load_recovery"]=loadedSaveUnchanged,["valid_loaded_pose_orientation_unchanged"]=validLoadUnchanged,["test_save_path"]=path,["method"]="Direct Editor component integration, Save/Place/Recover/AfterLoad callback; isolated save under workspace; native F5/F6 still NOT_RUN"};s.SavePathOverride=null;return result;
 }
 public static void VerifyAndBuild(){Verify();Build();}
 public static void Build()
 {
  var verification=JObject.Parse(File.ReadAllText(Out+"/verification.json"));
  if(verification["walks"].Concat(verification["cross_entries"]).Any(w=>w["failures"].Any())||verification["mesh_issues"].Any()||verification["boundary_misses"].Any()||verification["nearby_footholds"].Any()||verification["recovery_tests"].Children<JProperty>().Any(p=>p.Value.Type==JTokenType.Boolean&&!(bool)p.Value))throw new Exception("Resolve derivative verification issues before build");
  EditorSceneManager.OpenScene(Scene);var s=UnityEngine.Object.FindAnyObjectByType<DieuPhoiHanhTrinh>();var old=JObject.Parse(File.ReadAllText("Docs/Reviews/journey_v48/r21/boundary-finish/survey.json"))["player"];var c=s.player.controller;
  if(c.height!=(float)old["height"]||c.radius!=(float)old["radius"]||c.stepOffset!=(float)old["step"]||c.slopeLimit!=(float)old["slope"]||s.player.jumpHeight!=(float)old["jump"]||s.player.gravity!=(float)old["gravity"]||s.player.speed!=(float)old["speed"])throw new Exception("Player parameters changed");
  var reflection=UnityEngine.Object.FindAnyObjectByType<PhanChieuMatHo>();if(!reflection.water.enabled||P(reflection.water.transform)!="R20_Whole_Source_Export/Surface_051")throw new Exception("Reflection binding changed");
  foreach(var light in UnityEngine.Object.FindObjectsByType<Light>(FindObjectsInactive.Include).Where(l=>l.type!=LightType.Directional&&l.shadows!=LightShadows.None)){var data=light.GetComponent<UnityEngine.Rendering.Universal.UniversalAdditionalLightData>();if(!data||new SerializedObject(data).FindProperty("m_AdditionalLightsShadowResolutionTier").intValue!=UnityEngine.Rendering.Universal.UniversalAdditionalLightData.AdditionalLightsShadowResolutionTierLow)throw new Exception("Practical shadow tier changed");}
  var report=BuildPipeline.BuildPlayer(new[]{Scene},"Builds/Windows_v48_r21_recoveryfix/ConRoi.exe",BuildTarget.StandaloneWindows64,BuildOptions.Development);var messages=new JArray();foreach(var step in report.steps)foreach(var message in step.messages)if(message.type==LogType.Warning||message.type==LogType.Error||message.type==LogType.Exception)messages.Add(new JObject{["step"]=step.name,["type"]=message.type.ToString(),["message"]=message.content});
  File.WriteAllText(Out+"/build.json",new JObject{["result"]=report.summary.result.ToString(),["errors"]=report.summary.totalErrors,["warnings"]=report.summary.totalWarnings,["messages"]=messages,["scene"]=Scene,["binary"]="Builds/Windows_v48_r21_recoveryfix/ConRoi.exe",["status"]="DERIVATIVE_INTERNAL_NATIVE_NOT_RUN_NOT_CANDIDATE"}.ToString());
  if(report.summary.result!=UnityEditor.Build.Reporting.BuildResult.Succeeded)throw new Exception("Derivative build failed");
 }
 // Matches movement's accumulated combined sweep and grounded jump gate; Editor evidence only.
 static JObject Replay(DieuKhienDiChuyen p,Vector3 start,Vector3 direction,float[] durations,float hz,bool jumps)
 {
  var g=new GameObject("Temporary_Sequential_Controller");var c=g.AddComponent<CharacterController>();var o=p.controller;c.height=o.height;c.radius=o.radius;c.center=o.center;c.skinWidth=o.skinWidth;c.slopeLimit=o.slopeLimit;c.stepOffset=o.stepOffset;c.minMoveDistance=o.minMoveDistance;g.transform.position=start;Physics.SyncTransforms();
  float vy=-2,dt=1/hz;Vector3 pending=Vector3.zero;var rows=new JArray();int jumpCount=0;
  // Settle first, then preserve grounded/vertical state across both jumps and waits.
  for(int i=0;i<hz;i++)c.Move(Vector3.down*2*dt);
  for(int phase=0;phase<durations.Length;phase++){
   bool walk=phase%2==0;bool requested=false;
   for(int frame=0;frame<Mathf.RoundToInt(durations[phase]*hz);frame++){
    bool grounded=c.isGrounded;if(grounded&&vy<0)vy=-2;
    if(walk&&jumps&&!requested){requested=true;if(grounded){vy=Mathf.Sqrt(p.jumpHeight*-2*p.gravity);jumpCount++;}}
    vy+=p.gravity*dt;pending+=((walk?direction*p.speed:Vector3.zero)+Vector3.up*vy)*dt;
    if(pending.sqrMagnitude>=c.minMoveDistance*c.minMoveDistance&&(vy>=0||Mathf.Abs(pending.y)>=c.minMoveDistance)){c.Move(pending);pending=Vector3.zero;}
   }
   var pos=g.transform.position;rows.Add(new JObject{["phase"]=phase,["position"]=V(pos),["grounded"]=c.isGrounded,["support"]=new JArray(Physics.RaycastAll(pos+Vector3.up*.1f,Vector3.down,2,8,QueryTriggerInteraction.Ignore).OrderBy(h=>h.distance).Take(4).Select(Hit))});
  }
  UnityEngine.Object.DestroyImmediate(g);return new JObject{["hz"]=hz,["jumps"]=jumpCount,["phases"]=rows,["method"]="Editor sequential CharacterController replay; no native acceptance"};
 }
 static void Render(DieuPhoiHanhTrinh s,string stage)
 {
  Directory.CreateDirectory(Out+"/"+stage);var eye=UnityEngine.Object.Instantiate(s.player.GetComponentInChildren<Camera>());eye.transform.SetParent(null);eye.gameObject.SetActive(true);eye.enabled=false;eye.fieldOfView=65;RenderSettings.fog=false;
  var views=new[]{( "shore",new Vector3(649,3.7f,500),new Vector3(654,.3f,506)),("boundary",new Vector3(699,-3.8f,580),new Vector3(699,-2,590)),("boundary-side",new Vector3(692,0,586),new Vector3(700,-3,590))};
  foreach(var v in views){eye.transform.SetPositionAndRotation(v.Item2,Quaternion.LookRotation(v.Item3-v.Item2));var rt=RenderTexture.GetTemporary(1280,720,24);var tex=new Texture2D(1280,720,TextureFormat.RGB24,false);for(int i=0;i<3;i++)UnityEngine.Rendering.RenderPipeline.SubmitRenderRequest(eye,new UnityEngine.Rendering.Universal.UniversalRenderPipeline.SingleCameraRequest{destination=rt});var old=RenderTexture.active;RenderTexture.active=rt;tex.ReadPixels(new Rect(0,0,1280,720),0,0);tex.Apply();RenderTexture.active=old;File.WriteAllBytes(Out+"/"+stage+"/"+v.Item1+".png",tex.EncodeToPNG());RenderTexture.ReleaseTemporary(rt);UnityEngine.Object.DestroyImmediate(tex);}
  UnityEngine.Object.DestroyImmediate(eye.gameObject);
 }
}
public sealed class R21RecoveryContactProbe:MonoBehaviour
{
 public CharacterController controller;public Vector3 normal;
 void OnControllerColliderHit(ControllerColliderHit hit){if(hit.normal.y>=Mathf.Cos(controller.slopeLimit*Mathf.Deg2Rad)&&hit.point.y<=controller.bounds.min.y+controller.radius+controller.skinWidth&&hit.normal.y>normal.y)normal=hit.normal;}
}
