using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[InitializeOnLoad]
public static class ConRoiR27Review
{
 const string Out=ConRoiR27Slope.Out;static IEnumerator routine;static Stack<IEnumerator> stack;static double deadline;
 static JArray checks,errors,walk;static DieuPhoiHanhTrinh s;static Camera eye;static DieuKhienGocNhin mouse;static TraiNghiemDocR25 exp;static int frame;
 static ConRoiR27Review(){EditorApplication.playModeStateChanged+=state=>{if(state==PlayModeStateChange.EnteredPlayMode&&SessionState.GetBool("r27.review",false)){checks=new JArray();errors=new JArray();walk=new JArray();stack=new Stack<IEnumerator>();stack.Push(Run());deadline=EditorApplication.timeSinceStartup+900;Application.logMessageReceived+=Log;EditorApplication.update+=Tick;}};}
 public static void Begin(){EditorSceneManager.OpenScene(ConRoiR27Slope.Scene);SessionState.SetBool("r27.review",true);EditorApplication.EnterPlaymode();}
 public static void AuthorAndBegin(){ConRoiR27Slope.Author();Begin();}
 public static void RefineAndBegin(){ConRoiR27Slope.Refine();Begin();}
 static void Log(string m,string t,LogType k){if(k==LogType.Error||k==LogType.Exception||k==LogType.Assert)errors.Add(new JObject{["message"]=m,["trace"]=t});}
 static void Tick(){try{EditorApplication.QueuePlayerLoopUpdate();if(EditorApplication.timeSinceStartup>deadline)throw new Exception("Review timeout");while(stack.Count>0){var r=stack.Peek();if(!r.MoveNext()){stack.Pop();continue;}if(r.Current is IEnumerator nested){stack.Push(nested);continue;}return;}Finish(null);}catch(Exception e){Finish(e.ToString());}}
 static void Finish(string error){bool pass=error==null&&errors.Count==0&&checks.All(c=>(bool)c["pass"]);File.WriteAllText(Out+"/playmode.json",new JObject{["complete"]=pass,["failure"]=error,["checks"]=checks,["errors"]=errors,["method"]="Editor Play Mode; story transition APIs outside film, real PlayerMovement controller/filter throughout continuous film. Loading coroutine bypassed in batch. No native Windows input certification."}.ToString());File.WriteAllText(Out+"/slope-walk.json",walk.ToString());SessionState.SetBool("r27.review",false);EditorApplication.update-=Tick;EditorApplication.Exit(pass?0:1);}
 static void Check(string n,bool p,JToken d=null){checks.Add(new JObject{["name"]=n,["pass"]=p,["detail"]=d});Debug.Log("R25_CHECK "+n+" "+p);}
 static JArray V(Vector3 p)=>new JArray(p.x,p.y,p.z);
 static IEnumerator Wait(float seconds){float end=Time.time+seconds;while(Time.time<end)yield return null;}
 static void Look(Vector3 at){if(mouse)mouse.enabled=false;var d=at-eye.transform.position;var flat=d;flat.y=0;s.player.transform.rotation=Quaternion.LookRotation(flat);eye.transform.rotation=Quaternion.LookRotation(d);}
 static void Shot(string n){Canvas.ForceUpdateCanvases();ConRoiR22Scenery.Capture(eye,Out+"/play/"+n+".png");}
 static float C(float z)=>Mathf.Clamp01((z-18)/12)*2;
 static void Drive(bool moving){float z=s.player.transform.position.z+60,ahead=z+2;var p=new Vector3(-180+C(ahead)+.45f,Mathf.Min(ahead,30)*.2f+1.65f,-60+ahead);var d=p-s.player.transform.position;d.y=0;var local=s.player.transform.InverseTransformDirection(d.normalized)*.65f;s.player.MovementFilter=raw=>moving?new Vector2(local.x,local.z):Vector2.zero;}
 static void Capture(string phase){ConRoiR22Scenery.Capture(eye,Out+"/slope-walk/frame-"+frame.ToString("D5")+".png");walk.Add(new JObject{["frame"]=frame++,["position"]=V(s.player.transform.position),["eye"]=V(eye.transform.position),["rotation"]=V(eye.transform.eulerAngles),["grounded"]=s.player.controller.isGrounded,["region"]=s.data.region,["phase"]=phase,["trace"]=s.data.slopeTrace,["case_rotation"]=V(s.slopeChapter.suitcase.eulerAngles),["baseline_seconds"]=exp.BaselineVisibleSeconds,["after_seconds"]=exp.AfterVisibleSeconds,["door_prompt"]=exp.DoorPromptVisible});}
 static void Geometry(){
  Physics.SyncTransforms();var samples=new JArray();bool pass=true;
  for(int i=0;i<=88;i++){float z=i*.5f;foreach(float dx in new[]{-.5f,0f,.4f}){
   var at=new Vector3(-180+C(z)+dx,12,-60+z);var hits=Physics.RaycastAll(at,Vector3.down,16,8,QueryTriggerInteraction.Ignore).Where(h=>!h.transform.IsChildOf(s.player.transform)).OrderBy(h=>h.distance).ToArray();
   bool found=hits.Length>0;float expected=Mathf.Min(z,30)*.2f;bool good=found&&Mathf.Abs(hits[0].point.y-expected)<.025f&&hits[0].normal.y>.9f;pass&=good;
   samples.Add(new JObject{["z"]=z,["dx"]=dx,["height"]=found?(JToken)hits[0].point.y:null,["first_hit"]=found?hits[0].transform.name:null,["pass"]=good});
  }}
  bool stairs=true;var stairSamples=new JArray();
  for(int i=0;i<38;i++){float z=i*.8f+.4f;foreach(float dx in new[]{-1.82f,-1.45f,-1.08f}){var at=new Vector3(-180+C(z)+dx,12,-60+z);bool found=Physics.Raycast(at,Vector3.down,out var hit,16,8,QueryTriggerInteraction.Ignore);float expected=Mathf.Min((i+1)*.16f,6);bool good=found&&Mathf.Abs(hit.point.y-expected)<.025f&&hit.normal.y>.95f;stairs&=good;stairSamples.Add(new JObject{["z"]=z,["dx"]=dx,["height"]=found?(JToken)hit.point.y:null,["pass"]=good});}}
  File.WriteAllText(Out+"/stair-validation.json",new JObject{["pass"]=stairs,["samples"]=stairSamples}.ToString());Check("114_stair_tread_support_samples",stairs);
  var meshes=s.slopeChapter.environment.GetComponentsInChildren<MeshFilter>().Where(f=>(f.name.StartsWith("R25_")||f.name.StartsWith("R26_")||f.name.StartsWith("R27_")));bool uv=meshes.Any()&&meshes.All(f=>f.sharedMesh.uv.Length==f.sharedMesh.vertexCount&&f.sharedMesh.normals.All(n=>n.sqrMagnitude>.9f));
  File.WriteAllText(Out+"/geometry-validation.json",new JObject{["pass"]=pass&&uv,["samples"]=samples,["uv_normals"]=uv,["scope"]="267 first-hit layer-3 support samples. Does not filter out blocking architecture. Individual solids audited in Blender; no whole-scene intersection-free claim."}.ToString());Check("267_ramp_court_support_samples",pass);Check("new_mesh_uv_normals",uv);
 }
 static IEnumerator Run(){
  s=UnityEngine.Object.FindAnyObjectByType<DieuPhoiHanhTrinh>();eye=s.player.GetComponentInChildren<Camera>();mouse=s.player.GetComponentInChildren<DieuKhienGocNhin>();exp=s.GetComponent<TraiNghiemDocR25>();
  InputSystem.settings.updateMode=InputSettings.UpdateMode.ProcessEventsManually;Application.runInBackground=true;
  Check("private_r26_save_profile",s.SavePathOverride.Contains("r26-playtest"));s.SavePathOverride=Path.GetFullPath(Out+"/fixture-save.json");s.SetMenu(false);s.Restart();yield return null;yield return null;
  var loader=s.GetComponent<ManHinhTaiV44>();loader.StopAllCoroutines();s.SetLoading(false);loader.enabled=false;
  Check("boarding_house_retained",s.data.region==-1&&!s.DreamHub);s.Sleep();yield return Wait(3.5f);
  Check("bao_spawn_preserved",s.data.region==0&&Vector3.Distance(s.lakeSpawn.position,new Vector3(656.05f,2.0521f,514.35f))<.001f);
  Check("lake_GD1_retained",s.puppets.Where(p=>p.gameObject.activeInHierarchy).All(p=>p.giaiDoanHienTai==DieuKhienConRoi.GiaiDoan.GD1));
  s.Wake();s.Sleep();yield return Wait(3.5f);Check("market_GD2_retained",s.data.region==1&&s.puppets.Where(p=>p.gameObject.activeInHierarchy).All(p=>p.giaiDoanHienTai==DieuKhienConRoi.GiaiDoan.GD2));
  s.Wake();s.Sleep();yield return Wait(3.5f);Check("school_CoMai_GD3_retained",s.data.region==2&&s.schoolChapter.ghost.name.Contains("CoMai")&&s.schoolChapter.puppet.giaiDoanHienTai==DieuKhienConRoi.GiaiDoan.GD3);
  Check("fictional_school_sign_retained",s.schoolChapter.environment.GetComponentsInChildren<TextMesh>(true).Any(t=>t.name=="R24_MainSchoolName"&&t.text=="TRƯỜNG ĐÀ LẠT"));
  s.Wake();s.Sleep();yield return Wait(3.5f);if(mouse)mouse.enabled=false;
  Check("slope_GD4_retained",s.data.region==3&&s.slopeChapter.puppet.giaiDoanHienTai==DieuKhienConRoi.GiaiDoan.GD4);Check("single_door_proxy_preserved",s.slopeChapter.environment.GetComponentsInChildren<TuongTacDocV39>().Count(t=>t.finalDoor)==1);
  Check("technical_signs_disabled",!UnityEngine.Object.FindObjectsByType<TextMesh>(FindObjectsInactive.Exclude).Any(t=>t.text.Contains("CỬA CUỐI DỐC")));
  var memories=s.slopeChapter.environment.GetComponentsInChildren<TuongTacDocV39>().Where(m=>m.memoryIndex>=0).ToArray();Check("two_optional_papers_retained",memories.Length==2&&memories.Select(m=>m.memoryIndex).Distinct().Count()==2);
  RenderSettings.fog=false;Shader.SetGlobalVector("_MistProfile",Vector4.zero);
  Geometry();Shot("entry-no-fog");
  yield return Traversal("ramp_down",42,1,.45f);
  yield return Traversal("stairs_up",1,29,-1.4f);
  yield return Traversal("stairs_down",29,1,-1.4f);
  yield return Traversal("stair_lane_edge_up",1,29,-.68f);
  yield return WallContact();
  foreach(var memory in memories){var p=memory.transform.position;float approachZ=p.z+60-1.2f;s.Place(new Vector3(-180+C(approachZ)-1.4f,Mathf.Min(approachZ,30)*.2f+.10f,p.z-1.2f),Quaternion.identity);Look(p);yield return Wait(.3f);Look(p);yield return null;Check("paper_"+memory.memoryIndex+"_ray_reachable",s.player.GetComponent<TuongTacNguoiChoi>().Target==memory,new JObject{["player"]=V(s.player.transform.position),["paper"]=V(p),["method"]="Approach from stair centre at tread height, not perched on retaining wall"});memory.Interact();Check("paper_"+memory.memoryIndex+"_reading_preserved",(s.data.slopeReadMask&(1<<memory.memoryIndex))!=0);s.polish.DismissReading();s.Say("");}
  // Separate negative trial: proximity without seeing the suitcase earns no trace.
  var caseAt=s.slopeChapter.suitcase.position;var spawn=s.slopeChapter.spawn;
  s.Place(new Vector3(caseAt.x-.8f,caseAt.y+.93f,caseAt.z-1),Quaternion.identity);Look(eye.transform.position-Vector3.forward*10);yield return Wait(1.4f);
  Check("unseen_proximity_does_not_invent_change",!s.data.slopeTrace&&exp.BaselineVisibleSeconds<.05f);Check("door_prompt_absent_far",!exp.DoorPromptVisible);
  s.Place(spawn.position,spawn.rotation);Look(new Vector3(-179,5,-35));yield return Wait(.3f);
  // Continuous controller walk from entry to final approach. No Place calls in this segment.
  Time.captureDeltaTime=1f/12;bool baseline=false,after=false;int held=0;float start=s.player.transform.position.z;var rest=s.slopeChapter.suitcase.rotation;
  while(s.player.transform.position.z< -15.75f&&frame<500){
   float z=s.player.transform.position.z+60;string phase="walk";
   if(z>=17.5f&&!baseline){Look(caseAt+Vector3.up*.25f);Drive(false);phase="observe-before";if(++held>=18){baseline=true;held=0;}}
   else if(baseline&&!s.data.slopeTrace&&z<25){Look(caseAt+Vector3.up*.25f);Drive(true);phase="approach-case";}
   else if(s.data.slopeTrace&&!after){Look(caseAt+Vector3.up*.25f);Drive(false);phase="observe-after";if(++held>=18){after=true;held=0;}}
   else{float ahead=z+3;float glance=0;if(z>4&&z<9){glance=-3.5f*Mathf.Sin((z-4)/5*Mathf.PI);phase="walk-look-left";}if(z>11&&z<16){glance=4*Mathf.Sin((z-11)/5*Mathf.PI);phase="walk-look-right";}if(z>28&&z<33){glance=-3*Mathf.Sin((z-28)/5*Mathf.PI);phase="walk-look-left-upper";}Look(new Vector3(-180+C(ahead)+.45f+glance,Mathf.Min(ahead,30)*.2f+1.65f,-60+ahead));Drive(true);}
   yield return null;Capture(phase);
   if(phase=="observe-before"&&held==12)Shot("case-before");if(phase=="observe-after"&&held==12)Shot("case-after");
  }
  Drive(false);Check("continuous_walk_reaches_door_approach",s.player.transform.position.z>=-15.75f&&s.player.transform.position.z-start>40,V(s.player.transform.position));
  Check("case_seen_before_change",exp.BaselineVisibleSeconds>=1.1f,new JValue(exp.BaselineVisibleSeconds));Check("case_after_state_visible",exp.AfterVisibleSeconds>=1.1f,new JValue(exp.AfterVisibleSeconds));Check("native_case_twist_retained",s.data.slopeTrace&&Quaternion.Angle(rest,s.slopeChapter.suitcase.rotation)>11.9f);
  var door=s.slopeChapter.environment.GetComponentsInChildren<TuongTacDocV39>().Single(t=>t.finalDoor);var interaction=s.player.GetComponent<TuongTacNguoiChoi>();
  Look(door.transform.position);for(int i=0;i<18;i++){yield return null;Capture("door-near-look");}Shot("door-near-look");
  Check("door_actual_ray_target",interaction.Target==door);Check("door_exact_context_prompt",exp.DoorPromptVisible&&s.GetComponentsInChildren<Text>().Any(t=>t.text=="E: Mở cửa"));
  Look(eye.transform.position+Vector3.right*10);for(int i=0;i<12;i++){yield return null;Capture("door-look-away");}Check("lookaway_hides_prompt",!exp.DoorPromptVisible&&!s.GetComponentsInChildren<Text>().Any(t=>t.text=="E: Mở cửa"));
  Look(door.transform.position);for(int i=0;i<12;i++){yield return null;Capture("door-return-look");}
  Time.captureDeltaTime=0;s.player.MovementFilter=null;var pose=s.player.transform.position;s.Save();s.Load();yield return Wait(.3f);if(mouse)mouse.enabled=false;
  Check("slope_save_load_retains_pose_trace",s.data.slopeTrace&&Vector3.Distance(pose,s.player.transform.position)<.1f);Check("load_does_not_invent_baseline",exp.BaselineVisibleSeconds==0);
  Check("no_assertive_change_hint",!s.GetComponentsInChildren<Text>().Any(t=>t.text.Contains("Một chi tiết quen thuộc đã thay đổi")));
  door.Interact();door.Interact();yield return Wait(.2f);Check("final_starts_once",s.data.finalStarted&&s.data.finalStarts==1&&s.slopeChapter.puppet.gameObject.activeInHierarchy);Shot("final-GD4");
  yield return Wait(10.2f);Check("editor_ending_still_completes",s.data.region==-1&&s.data.nextRegion==4&&!string.IsNullOrEmpty(s.data.ending));
 }
 static IEnumerator Traversal(string name,float start,float end,float offset){
  float y=offset<-.8f?Mathf.Min((Mathf.Floor(start/.8f)+1)*.16f,6):Mathf.Min(start,30)*.2f;
  float capsuleLift=s.player.controller.height*.5f-s.player.controller.center.y+.06f;
  s.Place(new Vector3(-180+C(start)+offset,y+capsuleLift,-60+start),Quaternion.identity);s.player.MovementFilter=raw=>Vector2.zero;yield return Wait(.25f);
  Time.captureDeltaTime=1f/30;float sign=Mathf.Sign(end-start);int frames=0,unsupported=0,sideHits=0;var samples=new JArray();
  while(sign*(s.player.transform.position.z+60-end)<-.12f&&frames++<1000){
   float z=s.player.transform.position.z+60,ahead=z+sign*.8f;var target=new Vector3(-180+C(ahead)+offset,0,-60+ahead);var d=target-s.player.transform.position;d.y=0;var local=s.player.transform.InverseTransformDirection(d.normalized)*.65f;s.player.MovementFilter=raw=>new Vector2(local.x,local.z);
   yield return null;
   var feet=s.player.controller.bounds;bool support=Physics.Raycast(new Vector3(feet.center.x,feet.min.y+.3f,feet.center.z),Vector3.down,out var hit,.62f,8,QueryTriggerInteraction.Ignore);
   if(!support)unsupported++;if((s.player.HorizontalFlags&CollisionFlags.Sides)!=0)sideHits++;
   if(frames%15==0)samples.Add(new JObject{["position"]=V(s.player.transform.position),["support"]=support,["grounded"]=s.player.controller.isGrounded});
  }
  s.player.MovementFilter=raw=>Vector2.zero;Time.captureDeltaTime=0;
  Check(name+"_controller_traversal",frames<1000&&unsupported==0,new JObject{["end"]=V(s.player.transform.position),["frames"]=frames,["unsupported"]=unsupported,["side_contact_frames"]=sideHits,["samples"]=samples});
  var pose=s.player.transform.position;s.Save();s.Load();yield return Wait(.2f);Check(name+"_save_load",Vector3.Distance(pose,s.player.transform.position)<.12f);
 }
 static IEnumerator WallContact(){
  float z=6;float tread=(Mathf.Floor(z/.8f)+1)*.16f;float capsuleLift=s.player.controller.height*.5f-s.player.controller.center.y+.06f;
  s.Place(new Vector3(-180+C(z)-1.45f,tread+capsuleLift,-60+z),Quaternion.identity);s.player.MovementFilter=raw=>Vector2.zero;yield return Wait(.25f);
  float oldX=s.player.transform.position.x;Time.captureDeltaTime=1f/30;s.player.MovementFilter=raw=>new Vector2(-.65f,0);for(int i=0;i<35;i++)yield return null;
  s.player.MovementFilter=raw=>Vector2.zero;Time.captureDeltaTime=0;
  Check("left_retaining_wall_blocks_capsule",s.player.transform.position.x> -182.15f&&s.player.transform.position.x<oldX,new JObject{["position"]=V(s.player.transform.position),["wall_face"]= -182.12f});
 }
}
