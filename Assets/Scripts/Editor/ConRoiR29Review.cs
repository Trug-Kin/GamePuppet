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
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
[InitializeOnLoad]
public static class ConRoiR29Review {
 const string O=ConRoiR29School.Out;static Stack<IEnumerator> stack;static double deadline;static JArray checks,errors,film;static DieuPhoiHanhTrinh s;static TraiNghiemTruongR28 ex;static Camera eye;static DieuKhienGocNhin mouse;static TuongTacNguoiChoi pi;static int frame;static bool filming;static Func<Vector2,Vector2> productionFilter;
 static ConRoiR29Review(){EditorApplication.playModeStateChanged+=p=>{if(p==PlayModeStateChange.EnteredPlayMode&&SessionState.GetBool("r29.review",false)){checks=new JArray();errors=new JArray();film=new JArray();stack=new Stack<IEnumerator>();stack.Push(Run());deadline=EditorApplication.timeSinceStartup+1100;Application.logMessageReceived+=Log;EditorApplication.update+=Tick;}};}
 public static void RefineAndBegin(){ConRoiR29School.Import();ConRoiR29School.Validate();Begin();}
 public static void Begin(){EditorSceneManager.OpenScene(ConRoiR29School.Scene);SessionState.SetBool("r29.review",true);EditorApplication.EnterPlaymode();}
 static void Log(string m,string t,LogType k){if(k==LogType.Error||k==LogType.Exception)errors.Add(new JObject{["message"]=m,["trace"]=t});}
 static void Tick(){try{EditorApplication.QueuePlayerLoopUpdate();if(EditorApplication.timeSinceStartup>deadline)throw new Exception("Timeout");while(stack.Count>0){var r=stack.Peek();if(!r.MoveNext()){stack.Pop();continue;}if(r.Current is IEnumerator e){stack.Push(e);continue;}return;}Finish(null);}catch(Exception e){Finish(e.ToString());}}
 static void Finish(string error){bool pass=error==null&&errors.Count==0&&checks.All(t=>(bool)t["pass"]);File.WriteAllText(O+"/playmode.json",new JObject{["complete"]=pass,["failure"]=error,["checks"]=checks,["errors"]=errors,["method"]="Unity Editor Play Mode. Controller driven by synthetic MovementFilter; interactions use raycast and queued E with synchronous PlayerInteraction.Update after manual InputSystem.Update (batch focus). Startup loading bypassed for batch; school transition uses Sleep/Wake. Camera directed by review harness only. No Windows binary certification."}.ToString());File.WriteAllText(O+"/school-walk.json",film.ToString());SessionState.SetBool("r29.review",false);EditorApplication.update-=Tick;EditorApplication.Exit(pass?0:1);}
 static void Check(string name,bool pass,JToken d=null){checks.Add(new JObject{["name"]=name,["pass"]=pass,["detail"]=d});Debug.Log("R29_CHECK "+name+" "+pass);}
 static JArray V(Vector3 p)=>new JArray(p.x,p.y,p.z);static Vector3 W(float x,float y,float z)=>ConRoiR28School.W(x,y,z);
 static IEnumerator Wait(float sec){float end=Time.time+sec;while(Time.time<end){yield return null;if(filming)Capture("pause");}}
 static void Look(Vector3 at){mouse.enabled=false;var d=at-eye.transform.position;var f=d;f.y=0;if(f.sqrMagnitude>.001f)s.player.transform.rotation=Quaternion.LookRotation(f);eye.transform.rotation=Quaternion.LookRotation(d);}
 static void Stop(){s.player.MovementFilter=r=>Vector2.zero;}
 static void Capture(string phase){var rt=RenderTexture.GetTemporary(800,500,24);var prev=RenderTexture.active;var t=new Texture2D(800,500,TextureFormat.RGB24,false);try{RenderPipeline.SubmitRenderRequest(eye,new UniversalRenderPipeline.SingleCameraRequest{destination=rt});RenderTexture.active=rt;t.ReadPixels(new Rect(0,0,800,500),0,0);t.Apply();File.WriteAllBytes(System.IO.Path.GetTempPath()+"ConRoiR29Film/frame-"+frame.ToString("D5")+".jpg",t.EncodeToJPG(85));}finally{RenderTexture.active=prev;RenderTexture.ReleaseTemporary(rt);UnityEngine.Object.DestroyImmediate(t);}film.Add(new JObject{["frame"]=frame++,["phase"]=phase,["position"]=V(s.player.transform.position),["camera"]=V(eye.transform.eulerAngles),["region"]=s.data.region,["lead"]=ex.LeadSeconds,["observed"]=ex.ObservedSeconds,["paper"]=V(ex.puppetPaper.position),["response"]=V(ex.responsePaper.position),["witnessed"]=s.data.schoolWitnessed});}
 static void Shot(string n)=>ConRoiR22Scenery.Capture(eye,O+"/play/"+n+".png");
 static IEnumerator Walk(string n,Vector3 target,Vector3 look){int count=0,unsupported=0;float oldTime=Time.captureDeltaTime;Time.captureDeltaTime=1f/12;while(new Vector2(target.x-s.player.transform.position.x,target.z-s.player.transform.position.z).magnitude>.18f&&count++<240){Look(look);var d=target-s.player.transform.position;d.y=0;var local=s.player.transform.InverseTransformDirection(d.normalized)*.55f;s.player.MovementFilter=r=>new Vector2(local.x,local.z);yield return null;if(filming)Capture(n);var b=s.player.controller.bounds;if(!Physics.Raycast(new Vector3(b.center.x,b.min.y+.22f,b.center.z),Vector3.down,.65f,8,QueryTriggerInteraction.Ignore))unsupported++;}Stop();Time.captureDeltaTime=oldTime;Check(n,count<240&&unsupported==0,new JObject{["frames"]=count,["unsupported"]=unsupported,["position"]=V(s.player.transform.position)});if(count>=240)throw new Exception("Blocked route: "+n);}
 static IEnumerator Press(IPlayerInteractable target,Vector3 at,string n){Stop();Look(at);yield return null;if(filming)Capture(n+"_aim");Check(n+"_ray",pi.Target==target,new JValue(pi.Target?.GetType().Name));int before=pi.CompletedInteractions;InputSystem.QueueStateEvent(Keyboard.current,new KeyboardState(Key.E));ManualInput();pi.SendMessage("Update");if(filming)Capture(n+"_E");InputSystem.QueueStateEvent(Keyboard.current,new KeyboardState());ManualInput();yield return null;if(filming)Capture(n+"_release");Check(n+"_E",pi.CompletedInteractions==before+1);}
 static void ManualInput(){typeof(InputSystem).GetMethod("Update",System.Reflection.BindingFlags.Static|System.Reflection.BindingFlags.NonPublic,null,new[]{typeof(InputUpdateType)},null).Invoke(null,new object[]{InputUpdateType.Manual});}
 static float Lift=>s.player.controller.height*.5f-s.player.controller.center.y+.06f;
 static IEnumerator At(float x,float y,float z){s.Place(W(x,y+Lift,z),Quaternion.identity);Stop();yield return Wait(.25f);}
 static void Geometry(){
  Physics.SyncTransforms();bool support=true,clear=true;var rows=new JArray();
  for(int i=0;i<21;i++)foreach(float dx in new[]{-.7f,0,.7f}){
   float top=.24f+(i+1)*.16f;var p=W(-6.5f+dx,top,-3+(i+.5f)*.34f);
   bool hit=Physics.Raycast(p+Vector3.up*.18f,Vector3.down,out var h,.4f,8,QueryTriggerInteraction.Ignore);bool good=hit&&Mathf.Abs(h.point.y-top)<.015f;support&=good;
   var hits=Physics.OverlapCapsule(p+Vector3.up*.6f,p+Vector3.up*1.5f,.27f,8,QueryTriggerInteraction.Ignore).Where(c=>!c.transform.IsChildOf(s.player.transform)).ToArray();bool free=hits.Length==0;clear&=free;
   rows.Add(new JObject{["tread"]=i,["dx"]=dx,["support"]=good,["headroom"]=free,["overlaps"]=new JArray(hits.Select(c=>c.name))});
  }
  Check("63_stair_support_points",support);Check("63_stair_body_headroom",clear);
  var r=s.schoolChapter.environment.transform.Find("R28_School");var furniture=new JArray();bool grounded=true;
  foreach(Transform t in r)if(t.name.StartsWith("Ban_")||t.name.StartsWith("Ghe_")){float bottom=t.GetComponentsInChildren<Renderer>().Min(x=>x.bounds.min.y);bool good=Mathf.Abs(bottom-3.6f)<.025f;grounded&=good;furniture.Add(new JObject{["name"]=t.name,["bottom"]=bottom,["pass"]=good});}
  Check("furniture_ground_contacts",grounded);File.WriteAllText(O+"/geometry-validation.json",new JObject{["support"]=support,["headroom"]=clear,["stairs"]=rows,["furniture"]=furniture}.ToString());
 }
 static IEnumerator Contacts(){
  yield return At(23,0,-15);Look(W(30,1.6f,-15));float x=s.player.transform.position.x;Time.captureDeltaTime=1f/30;s.player.MovementFilter=r=>new Vector2(0,.6f);for(int i=0;i<45;i++)yield return null;Stop();Check("court_boundary_blocks",s.player.transform.position.x>x&&s.player.transform.position.x< -71.3f,V(s.player.transform.position));
  yield return At(0,3.6f,1.2f);Look(W(0,5.2f,-4));s.player.MovementFilter=r=>new Vector2(0,.6f);for(int i=0;i<45;i++)yield return null;Stop();Check("gallery_guard_blocks",s.player.transform.position.z> -44.6f&&s.player.transform.position.y>4.4f,V(s.player.transform.position));
  Time.captureDeltaTime=0;
 }

 static void FootSupport(){
  Physics.SyncTransforms();var rows=new JArray();
  foreach(var m in s.schoolChapter.puppet.GetComponentsInChildren<MeshFilter>(true).Where(m=>m.name=="ConRoi_Leg_L"||m.name=="ConRoi_Leg_R")){
   var b=m.GetComponent<Renderer>().bounds;var p=new Vector3(b.center.x,b.min.y+.025f,b.center.z);
   bool hit=Physics.Raycast(p,Vector3.down,out var h,.15f,8,QueryTriggerInteraction.Ignore);
   Check("puppet_foot_floor_"+m.name,hit&&Mathf.Abs(h.point.y-3.6f)<.02f&&Mathf.Abs(b.min.y-h.point.y)<.02f);
   rows.Add(new JObject{["leg"]=m.name,["foot_bottom"]=b.min.y,["support_y"]=hit?h.point.y:-999,["support"]=hit?h.collider.name:"NONE"});
  }
  File.WriteAllText(O+"/foot-support.json",rows.ToString());
 }
 static IEnumerator Run(){
  Directory.CreateDirectory(System.IO.Path.GetTempPath()+"ConRoiR29Film");s=UnityEngine.Object.FindAnyObjectByType<DieuPhoiHanhTrinh>();ex=s.GetComponent<TraiNghiemTruongR28>();eye=s.player.GetComponentInChildren<Camera>();mouse=s.player.GetComponentInChildren<DieuKhienGocNhin>();pi=s.player.GetComponent<TuongTacNguoiChoi>();
  InputSystem.settings=UnityEngine.Object.Instantiate(InputSystem.settings);InputSystem.settings.updateMode=InputSettings.UpdateMode.ProcessEventsManually;InputSystem.settings.editorInputBehaviorInPlayMode=InputSettings.EditorInputBehaviorInPlayMode.AllDeviceInputAlwaysGoesToGameView;InputSystem.settings.backgroundBehavior=InputSettings.BackgroundBehavior.IgnoreFocus;if(Keyboard.current==null)InputSystem.AddDevice<Keyboard>();Application.runInBackground=true;s.SavePathOverride=Path.GetFullPath(O+"/fixture-save.json");s.SetMenu(false);s.Restart();yield return null;yield return null;var loader=s.GetComponent<ManHinhTaiV44>();loader.StopAllCoroutines();loader.enabled=false;s.SetLoading(false);
  s.data.nextRegion=2;s.Sleep();yield return Wait(3.8f);mouse.enabled=false;RenderSettings.fog=false;Shader.SetGlobalVector("_MistProfile",Vector4.zero);
  Check("school_entry",s.data.region==2&&s.schoolChapter.environment.activeInHierarchy);FootSupport();Check("GD3",s.schoolChapter.puppet.giaiDoanHienTai==DieuKhienConRoi.GiaiDoan.GD3);Check("fictional_name",s.schoolChapter.environment.GetComponentsInChildren<TextMesh>().Any(t=>t.text=="TRƯỜNG ĐÀ LẠT"));
  Geometry();productionFilter=s.player.MovementFilter;Check("production_input_immediate",productionFilter!=null&&Vector2.Distance(productionFilter(new Vector2(.2f,.7f)),new Vector2(.2f,.7f))<.0001f);Stop();
  // Explicit synthetic inputs below exercise the production capsule; no teleport in the film.
  filming=true;Time.captureDeltaTime=1f/12;Look(W(0,5,-1));Capture("entry");Shot("entry");
  yield return Walk("gate",W(0,0,-25),W(-4,2,-17));yield return Walk("court",W(-6.5f,0,-5),W(0,5,1));Shot("court");
  yield return Walk("stair_approach",W(-6.5f,0,-3.45f),W(-6.5f,4.4f,4.7f));yield return Walk("stairs_up",W(-6.5f,3.6f,4.7f),W(-6.5f,5.2f,5));Shot("landing");
  yield return Walk("landing_to_gallery",W(-3.5f,3.6f,4.7f),W(4.6f,5.3f,5.4f));yield return Walk("gallery",W(4.6f,3.6f,3),W(4.6f,5.2f,6));yield return Walk("room203_entry",W(4.6f,3.6f,7),W(8,5,12));Shot("class203");
  yield return Walk("response_approach",W(8,3.6f,9.0f),W(5.5f,4.9f,11));Stop();Look(W(5.0f,4.8f,10.8f));
  var before=ex.puppetPaper.position;yield return Wait(3.1f);Shot("GD3_lead");Check("visible_lead_before_response",ex.Leading&&ex.LeadSeconds>=1.35f&&ex.ObservedSeconds>=.75f&&!s.data.schoolWitnessed,new JObject{["lead"]=ex.LeadSeconds,["observed"]=ex.ObservedSeconds,["paper_travel"]=Vector3.Distance(before,ex.puppetPaper.position)});
  yield return Press(ex.responsePaper.GetComponent<TuongTacTruongR28>(),ex.responsePaper.position,"paper");yield return Wait(.8f);Check("voluntary_matching_response",s.data.schoolWitnessed&&ex.LastResponseDelay>=1.35f);Shot("GD3_response");
  yield return Walk("desk_aisle",W(9.7f,3.6f,9),W(10.4f,5,12.1f));yield return Walk("mai_approach",W(9.7f,3.6f,10.6f),s.schoolChapter.face.ghostHead.position);var mai=s.schoolChapter.environment.GetComponentsInChildren<TuongTacTruongV37>().First(i=>i.name=="Mai_R28_Interaction");yield return Press(mai,mai.transform.position+Vector3.up*.55f,"mai");Check("Mai_talk",s.schoolChapter.Talks>0);Shot("mai");yield return Wait(1.2f);
  yield return Walk("desk_aisle_return",W(9.7f,3.6f,8.5f),W(4.5f,5,7));yield return Walk("class_return",W(4.5f,3.6f,7),W(2.1f,4.75f,5.12f));var exit=s.schoolChapter.environment.GetComponentsInChildren<TuongTacTruongV37>().First(i=>i.exit&&i.name=="203_Return");yield return Press(exit,exit.transform.position,"wake");yield return Wait(.6f);Check("film_transition",s.data.region==-1&&s.data.nextRegion==3);filming=false;Time.captureDeltaTime=0;
  // Re-enter with saved state for contact, descent, persistence and no-pressure branches.
  s.data.nextRegion=2;s.Sleep();yield return Wait(3.8f);mouse.enabled=false;Stop();
  yield return Contacts();yield return At(-6.5f,3.6f,4.7f);yield return Walk("stairs_down",W(-6.5f,0,-4.1f),W(-6.5f,1.4f,-6));
  yield return At(8,3.6f,9);Look(W(5.0f,4.8f,10.8f));yield return Wait(3);var q=eye.transform.rotation;var pos=s.player.transform.position;int aw=s.data.awareness;float lead=ex.LeadSeconds;yield return Wait(4);Check("idle_holds_no_penalty_camera",ex.Leading&&ex.LeadSeconds>lead&&Quaternion.Angle(q,eye.transform.rotation)<.01f&&Vector3.Distance(pos,s.player.transform.position)<.03f&&s.data.awareness==aw);
  s.Save();int reads=s.data.schoolReadMask;var pp=ex.puppetPaper.position;s.Load();yield return Wait(.3f);Stop();mouse.enabled=false;Check("upper_floor_save_load",Vector3.Distance(pos,s.player.transform.position)<.12f&&s.data.schoolReadMask==reads&&Vector3.Distance(pp,ex.puppetPaper.position)<.03f&&s.data.schoolWitnessed&&ex.ObservedSeconds==0);
  yield return At(12,3.6f,12.2f);yield return Wait(.3f);Check("optional_chair_trace",s.data.schoolTrace);s.Save();s.Load();yield return Wait(.2f);Check("chair_trace_saved",s.data.schoolTrace&&Quaternion.Angle(ex.traceChair.rotation,ex.chairBoard)<.1f);
  // Early response and missed observation must not award witness or gate exit.
  s.data.schoolWitnessed=false;s.data.schoolLeading=false;s.data.schoolReadMask=0;ex.Applied(true);yield return At(8,3.6f,9);Look(W(14,5,7));ex.Respond();Check("early_response_no_false_witness",!s.data.schoolWitnessed);yield return Wait(3);Check("missed_observation_no_witness",!s.data.schoolWitnessed&&ex.ObservedSeconds<.01f);Look(W(5.0f,4.8f,10.8f));s.data.schoolLeading=false;ex.Applied(true);yield return Wait(3);ex.Respond();yield return Wait(.8f);Check("late_response_after_early_attempt",s.data.schoolWitnessed);int responseCount=ex.Responses;ex.Respond();Check("repeat_E_no_duplicate_response",ex.Responses==responseCount);s.Wake();Check("missed_observation_can_continue",s.data.region==-1&&s.data.nextRegion==3);
  // Mid-action reload must preserve ordering and cannot consume the only response early.
  s.data.schoolReadMask&=~64;s.data.schoolWitnessed=false;s.data.schoolLeading=false;ex.Applied(true);
  s.data.nextRegion=2;s.Sleep();yield return Wait(3.8f);mouse.enabled=false;Stop();
  yield return At(8,3.6f,9);Look(W(5.0f,4.8f,10.8f));yield return Wait(1.5f);
  Check("mid_action_fixture",ex.Leading&&ex.LeadSeconds<1.35f);s.Save();float savedLead=ex.LeadSeconds;s.Load();yield return Wait(.1f);Stop();mouse.enabled=false;
  Check("mid_action_reload_order",ex.Leading&&ex.LeadSeconds>=savedLead&&ex.LeadSeconds<1.35f&&!ex.CanRespond);
  int earlyCount=ex.Responses;ex.Respond();Check("early_E_does_not_consume_sheet",ex.Responses==earlyCount&&(s.data.schoolReadMask&64)==0&&!s.data.schoolWitnessed);
  yield return Wait(2);ex.Respond();yield return Wait(.8f);s.Save();int terminalCount=ex.Responses,terminalAwareness=s.data.awareness;bool terminalWitness=s.data.schoolWitnessed;s.Load();yield return Wait(.2f);Stop();mouse.enabled=false;ex.Respond();
  Check("terminal_reload_no_duplicate",ex.Responses==terminalCount&&s.data.awareness==terminalAwareness&&s.data.schoolWitnessed==terminalWitness&&(s.data.schoolReadMask&64)!=0);
  s.Wake();
  // Prove all unchanged chapters still activate; this fixture never overwrites previous saves.
  foreach(int r in new[]{0,1,3}){s.data.nextRegion=r;s.Sleep();yield return Wait(3.8f);Stop();Check("retained_region_"+r,s.data.region==r);s.Save();var at=s.player.transform.position;s.Load();yield return Wait(.2f);Check("retained_save_"+r,Vector3.Distance(at,s.player.transform.position)<.12f);s.Wake();}
 }
}
