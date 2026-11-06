#if UNITY_EDITOR || CON_ROI_BUILD_REVIEW
using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

// Explicit review only. No component in the saved scene; never runs in ordinary play.
[DefaultExecutionOrder(30000)]
public sealed class R33ChairReview:MonoBehaviour
{
 public static string Output=>Path.Combine(Path.GetTempPath(),"ConRoiR33Review");
 public string output;
 TuongTacGiayLop203R32 activity;
 DieuPhoiHanhTrinh s;TraiNghiemTruongR28 ex;Camera eye;DieuKhienGocNhin mouse;TuongTacNguoiChoi pi;
 Stack<IEnumerator> stack=new Stack<IEnumerator>();JArray checks=new JArray(),errors=new JArray();StreamWriter timeline;
 int frame;bool filming=true,finished;string phase="startup";double deadline;
 static bool Flag(string name)=>Array.IndexOf(Environment.GetCommandLineArgs(),name)>=0;
 [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)] static void Boot(){
  if(Application.isEditor)return;var args=Environment.GetCommandLineArgs();int i=Array.IndexOf(args,"--r33-chair");if(i<0)return;
  var s=FindAnyObjectByType<DieuPhoiHanhTrinh>();s.gameObject.AddComponent<R33ChairReview>().output=i+1<args.Length?args[i+1]:Output;
 }
 void Start(){
  if(string.IsNullOrEmpty(output))output=Output;Directory.CreateDirectory(output);Directory.CreateDirectory(output+"/frames");
  s=FindAnyObjectByType<DieuPhoiHanhTrinh>();activity=s.GetComponent<TuongTacGiayLop203R32>();ex=s.GetComponent<TraiNghiemTruongR28>();eye=s.player.GetComponentInChildren<Camera>();mouse=s.player.GetComponentInChildren<DieuKhienGocNhin>();pi=s.player.GetComponent<TuongTacNguoiChoi>();
  s.SavePathOverride=output+"/isolated-save.json";Application.runInBackground=true;
  InputSystem.settings=Instantiate(InputSystem.settings);InputSystem.settings.updateMode=Flag("--r31-realtime")?InputSettings.UpdateMode.ProcessEventsInDynamicUpdate:InputSettings.UpdateMode.ProcessEventsManually;
  InputSystem.settings.backgroundBehavior=InputSettings.BackgroundBehavior.IgnoreFocus;
  InputSystem.settings.editorInputBehaviorInPlayMode=InputSettings.EditorInputBehaviorInPlayMode.AllDeviceInputAlwaysGoesToGameView;
  if(Keyboard.current==null)InputSystem.AddDevice<Keyboard>();
  if(Flag("--r31-realtime")){var reviewKeyboard=InputSystem.AddDevice<Keyboard>("R31ReviewKeyboard");reviewKeyboard.MakeCurrent();}
  timeline=new StreamWriter(output+"/timeline.jsonl"){AutoFlush=true};Application.logMessageReceived+=Log;
  Time.captureDeltaTime=Flag("--r31-realtime")?0:1f/12;
  if(Flag("--r31-realtime")){QualitySettings.vSyncCount=0;Application.targetFrameRate=12;}
  File.WriteAllText(output+"/clock-scope.json",new JObject{["captureDeltaTime"]=Time.captureDeltaTime,["realtime"]=Flag("--r31-realtime"),["targetFrameRate"]=Application.targetFrameRate,["renderRequest"]=Application.isBatchMode||Flag("--r31-render-request"),["automatedInputAndCamera"]=true}.ToString());
  deadline=Time.realtimeSinceStartupAsDouble+2400;stack.Push(Run());if(!Application.isBatchMode)StartCoroutine(RecordFrames());
 }
 void Log(string m,string t,LogType k){if(k==LogType.Error||k==LogType.Exception)errors.Add(new JObject{["message"]=m,["trace"]=t});}
 void LateUpdate(){
  if(finished)return;try{
   if(Time.realtimeSinceStartupAsDouble>deadline)throw new Exception("Review timeout");
   if(mouse)mouse.enabled=false;
   while(stack.Count>0){var e=stack.Peek();if(!e.MoveNext()){stack.Pop();continue;}if(e.Current is IEnumerator n){stack.Push(n);continue;}break;}
   if(Application.isBatchMode&&filming&&!s.Loading)Capture();if(stack.Count==0)Finish(null);
  }catch(Exception e){Finish(e.ToString());}
 }
 void Finish(string failure){
  finished=true;Stop();Time.captureDeltaTime=0;timeline?.Dispose();
  bool pass=failure==null&&errors.Count==0&&checks.All(c=>(bool)c["pass"]);
  File.WriteAllText(output+"/result.json",new JObject{["complete"]=pass,["failure"]=failure,["scene"]=gameObject.scene.path,["editor"]=Application.isEditor,["checks"]=checks,["errors"]=errors,["frames"]=frame,["method"]="Focused controller exploration loaded from actual r32 full-route school-before save. No teleport or progress seeding during route. Synthetic MovementFilter + keyboard, normal gameplay eye/FOV steered by harness; offscreen, not physical input. Ending resolver assertions are logic fixtures, not four-ending walkthroughs."}.ToString());
  Debug.Log("R33_FINISH "+pass+" "+failure);
#if UNITY_EDITOR
  if(Application.isEditor){UnityEditor.EditorApplication.Exit(pass?0:1);return;}
#endif
  Application.Quit(pass?0:1);
 }
 void Check(string n,bool p,JToken d=null){checks.Add(new JObject{["name"]=n,["pass"]=p,["detail"]=d});File.WriteAllText(output+"/progress.json",checks.ToString());Debug.Log("R33_CHECK "+n+" "+p);}
 static JArray V(Vector3 p)=>new JArray(p.x,p.y,p.z);
 static Vector3 P(JToken p)=>new Vector3((float)p[0],(float)p[1],(float)p[2]);
 static Vector3 W(float x,float y,float z)=>new Vector3(-95+x,y,-45+z);
 void Stop(){if(s)s.player.MovementFilter=r=>Vector2.zero;}
 void Look(Vector3 at){mouse.enabled=false;var d=at-eye.transform.position;var flat=d;flat.y=0;if(flat.sqrMagnitude>.0001f)s.player.transform.rotation=Quaternion.LookRotation(flat);eye.transform.rotation=Quaternion.LookRotation(d);}
 void Capture(){
  if(Application.isBatchMode||Flag("--r31-render-request")){
   var rt=RenderTexture.GetTemporary(1280,720,24);var old=RenderTexture.active;var t=new Texture2D(1280,720,TextureFormat.RGB24,false);
   try{RenderPipeline.SubmitRenderRequest(eye,new UniversalRenderPipeline.SingleCameraRequest{destination=rt});if(!Flag("--r31-no-images")&&(!Flag("--r31-gd3-video")||phase=="class203_entry"||phase.StartsWith("GD3_")||phase.StartsWith("PAPER_")||phase.StartsWith("SKIP_")||(Flag("--r32-skip")&&(s.data.region==2||phase=="school_wake")))){RenderTexture.active=rt;t.ReadPixels(new Rect(0,0,1280,720),0,0);t.Apply();File.WriteAllBytes(output+"/frames/"+frame.ToString("D6")+".jpg",t.EncodeToJPG(86));}}
   finally{RenderTexture.active=old;RenderTexture.ReleaseTemporary(rt);Destroy(t);}
  }else if(!Flag("--r31-no-images")){var t=ScreenCapture.CaptureScreenshotAsTexture();try{File.WriteAllBytes(output+"/frames/"+frame.ToString("D6")+".jpg",t.EncodeToJPG(86));}finally{Destroy(t);}}
  timeline.WriteLine(new JObject{["chairDistance"]=Vector3.Distance(s.player.transform.position,ex.traceChair.position),["schoolTrace"]=s.data.schoolTrace,["awareness"]=s.data.awareness,["actionPoints"]=s.data.actionPoints,["interactions"]=s.data.interactions,["chairRotation"]=V(ex.traceChair.eulerAngles),["chairPosition"]=V(ex.traceChair.position),["frame"]=frame++,["seconds"]=(frame-1)/12f,["phase"]=phase,["region"]=s.data.region,["position"]=V(s.player.transform.position),["camera"]=V(eye.transform.eulerAngles),["input"]=new JArray(s.player.LastMovementInput.x,s.player.LastMovementInput.y),["initial"]=ex.InitialSeconds,["lead"]=ex.LeadSeconds,["observed"]=ex.ObservedSeconds,["response"]=ex.ResponseProgress,["witnessed"]=s.data.schoolWitnessed,["paperMemory"]=JObject.Parse(JsonUtility.ToJson(activity.memory)),["inspecting"]=activity.Inspecting,["realtime"]=Time.realtimeSinceStartup,["puppetPaper"]=V(ex.puppetPaper.position),["minhPaper"]=V(ex.responsePaper.position),["puppets"]=new JArray(s.puppets.Select(p=>V(p.dauTransform.position)))}.ToString(Newtonsoft.Json.Formatting.None));
 }
 IEnumerator RecordFrames(){while(!finished){yield return new WaitForEndOfFrame();if(!finished&&filming&&!s.Loading)Capture();}}
 IEnumerator Wait(float sec,string p=null){if(p!=null)phase=p;float end=Time.time+sec;while(Time.time<end)yield return null;}
 IEnumerator Until(Func<bool> ready,float seconds,string name){phase=name;float end=Time.realtimeSinceStartup+seconds;while(!ready()){if(Time.realtimeSinceStartup>end)throw new Exception(name+" timeout");yield return null;}}
 IEnumerator Walk(string name,Vector3 target,Vector3? look=null){
  phase=name;int frames=0,unsupported=0;Vector3 start=s.player.transform.position;
  while(new Vector2(target.x-s.player.transform.position.x,target.z-s.player.transform.position.z).magnitude>.18f&&frames++<1200){
   Look(look??(target+Vector3.up*1.55f));var d=target-s.player.transform.position;d.y=0;
   float amount=Mathf.Min(.85f,d.magnitude/(s.player.speed*Mathf.Max(Time.deltaTime,1f/12)));
   var local=s.player.transform.InverseTransformDirection(d.normalized)*amount;s.player.MovementFilter=r=>new Vector2(local.x,local.z);
   yield return null;
   var b=s.player.controller.bounds;if(!Physics.Raycast(new Vector3(b.center.x,b.min.y+.25f,b.center.z),Vector3.down,.85f,8,QueryTriggerInteraction.Ignore))unsupported++;
  }
  Stop();Check(name,frames<1200&&unsupported==0,new JObject{["position"]=V(s.player.transform.position),["from"]=V(start),["frames"]=frames,["unsupported"]=unsupported});
  if(frames>=1200)throw new Exception("Blocked route "+name+" at "+s.player.transform.position);
 }
 void UpdateInteraction(){typeof(TuongTacNguoiChoi).GetMethod("Update",System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.NonPublic).Invoke(pi,null);}
 static void ManualInput(){typeof(InputSystem).GetMethod("Update",System.Reflection.BindingFlags.Static|System.Reflection.BindingFlags.NonPublic,null,new[]{typeof(InputUpdateType)},null).Invoke(null,new object[]{InputUpdateType.Manual});}
 IEnumerator Press(IPlayerInteractable target,Vector3 aim,string name){
  phase=name;Stop();Look(aim);yield return null;UpdateInteraction();
  if(pi.Target!=target){
   var component=target as MonoBehaviour;var box=component.GetComponent<Collider>();
   if(box){foreach(var candidate in new[]{box.bounds.center+Vector3.up*(box.bounds.extents.y*.95f),box.bounds.center+Vector3.left*box.bounds.extents.x*.9f,box.bounds.center+Vector3.right*box.bounds.extents.x*.9f}){Look(candidate);UpdateInteraction();if(pi.Target==target)break;}}
   if(pi.Target!=target){File.WriteAllText(output+"/blocked-ray.json",new JArray(Physics.RaycastAll(eye.transform.position,eye.transform.forward,4,~0,QueryTriggerInteraction.Ignore).OrderBy(h=>h.distance).Select(h=>new JObject{["name"]=h.collider.name,["distance"]=h.distance,["point"]=V(h.point),["center"]=V(h.collider.bounds.center),["size"]=V(h.collider.bounds.size)})).ToString());throw new Exception("Wrong ray target "+name+" actual="+pi.Target);}
  }
  Check(name+"_ray",pi.Target==target,new JValue(pi.Target?.GetType().Name));int count=pi.CompletedInteractions;
  InputSystem.QueueStateEvent(Keyboard.current,new KeyboardState(Key.E));
  if(Flag("--r31-realtime")){yield return null;yield return null;}
  else{ManualInput();UpdateInteraction();}
  InputSystem.QueueStateEvent(Keyboard.current,new KeyboardState());if(!Flag("--r31-realtime"))ManualInput();yield return null;
  Check(name+"_E",pi.CompletedInteractions==count+1,new JObject{["keyboard"]=Keyboard.current.name,["enabled"]=Keyboard.current.enabled,["focused"]=Application.isFocused,["updateMode"]=InputSystem.settings.updateMode.ToString(),["completedBefore"]=count,["completedAfter"]=pi.CompletedInteractions});
 }
 TuongTacHoCho Item(TuongTacHoCho.Kind kind)=>FindObjectsByType<TuongTacHoCho>().First(i=>i.kind==kind&&i.GetComponent<Collider>());
 IEnumerator Bed(int expected){yield return Walk("hub_to_bed_"+expected,new Vector3(-35,.94f,-34.6f));var bed=Item(TuongTacHoCho.Kind.Bed);yield return Press(bed,bed.transform.position,"bed_"+expected);yield return Until(()=>s.data.region==expected&&!s.Loading,15,"dream_transition_"+expected);Check("entered_"+expected,s.data.region==expected);}
 IEnumerator Motion(int region){
  var p=s.puppets[region==0?0:2];var at=p.dauTransform.position-Vector3.up*.55f;Look(at);Stop();yield return Wait(3.6f,"GD"+(region+1)+"_initial");
  for(int i=0;i<4&&!s.data.witnessed[region];i++){
   Look(p.dauTransform.position-Vector3.up*.55f);
   phase="GD"+(region+1)+"_Minh_step_"+i;s.player.MovementFilter=r=>new Vector2(0,i%2==0?.24f:-.24f);yield return Wait(.65f);
   Stop();yield return Wait(1.1f,"GD"+(region+1)+"_stop_"+i);
  }
  Check("GD"+(region+1)+"_observed",s.data.witnessed[region],new JObject{["visible_seconds"]=s.GetComponent<MoDauBenHoR23>().VisibleSeconds,["seen_travel"]=s.GetComponent<MoDauBenHoR23>().SeenTravel,["delay"]=p.doTreGD2});
 }
 void Snapshot(string name){s.Save();File.Copy(s.SavePathOverride,output+"/"+name+".json",true);}
 IEnumerator Restore(string name){File.Copy(output+"/"+name+".json",s.SavePathOverride,true);s.Load();yield return null;Stop();mouse.enabled=false;}
 IEnumerator Run(){
  yield return Until(()=>!s.Loading,25,"startup_loading");
  s.GetComponent<MenuVaoGameV47>().NewGame();Stop();yield return null;
  File.Copy("Docs/Reviews/journey_v48/r32/native-main/school-before.json",output+"/school-entry.json",true);
  yield return Restore("school-entry");yield return Wait(.5f,"loaded_real_route_class_entry");
  int aw=s.data.awareness,ap=s.data.actionPoints,ac=s.data.interactions;
  Check("entry_no_award",!s.data.schoolTrace&&s.data.awareness==aw);
  Snapshot("before-explore");
  yield return Walk("CHAIR_aisle",W(9.7f,3.6f,9),ex.traceChair.position+Vector3.up*.65f);
  yield return Walk("CHAIR_Mai_approach",W(9.7f,3.6f,10.6f),ex.traceChair.position+Vector3.up*.65f);
  Check("Mai_area_no_award",!s.data.schoolTrace&&s.data.awareness==aw);
  var mai=s.schoolChapter.environment.GetComponentsInChildren<TuongTacTruongV37>().First(i=>i.name=="Mai_R28_Interaction");
  yield return Press(mai,mai.transform.position+Vector3.up*.55f,"Mai_no_clue_award");yield return Wait(2);
  Check("Mai_dialogue_no_award",s.data.schoolTalks>0&&!s.data.schoolTrace&&s.data.awareness==aw);
  Look(ex.traceChair.position+Vector3.up*.65f);yield return Wait(1.5f,"CHAIR_initial_window_facing");
  yield return Walk("CHAIR_front_approach",W(9.4f,3.6f,11.9f),ex.traceChair.position+Vector3.up*.65f);
  yield return Walk("CHAIR_behind_teacher_desk",W(9.4f,3.6f,14.0f),ex.traceChair.position+Vector3.up*.65f);
  yield return Walk("CHAIR_discovery",W(11.7f,3.6f,14.0f),ex.traceChair.position+Vector3.up*.65f);
  yield return Wait(2.5f,"CHAIR_result_held");
  bool expected=!Flag("--r33-baseline");
  Check("reachable_original_detection_radius",Vector3.Distance(s.player.transform.position,ex.traceChair.position)<2.4f);
  Check("chair_expected_detection",s.data.schoolTrace==expected&&s.data.awareness==aw+(expected?1:0));
  Snapshot("chair-after");yield return Restore("chair-after");yield return Wait(1);
  Check("load_same_flag_score_pose",s.data.schoolTrace==expected&&s.data.awareness==aw+(expected?1:0)&&Quaternion.Angle(ex.traceChair.rotation,expected?ex.chairBoard:ex.chairWindow)<.1f);
  yield return Walk("CHAIR_leave_radius",W(9.4f,3.6f,14.0f),ex.traceChair.position+Vector3.up*.65f);
  yield return Walk("CHAIR_revisit",W(11.7f,3.6f,14.0f),ex.traceChair.position+Vector3.up*.65f);
  yield return Wait(1);Check("revisit_no_duplicate",s.data.awareness==aw+(expected?1:0));
  Check("other_axis_unchanged",s.data.actionPoints==ap&&s.data.interactions==ac);
  Snapshot("explore-final");
  yield return Restore("before-explore");yield return Wait(.5f,"SKIP_entry");
  yield return Walk("SKIP_aisle",W(9.7f,3.6f,9));yield return Walk("SKIP_Mai",W(9.7f,3.6f,10.6f),s.schoolChapter.face.ghostHead.position);
  yield return Press(mai,mai.transform.position+Vector3.up*.55f,"SKIP_Mai_talk");yield return Wait(2);
  Check("skip_talk_no_award",!s.data.schoolTrace&&s.data.awareness==aw);
  Snapshot("skip-before-exit");yield return Restore("skip-before-exit");yield return Wait(.4f);
  Check("skip_load_no_award",!s.data.schoolTrace&&s.data.awareness==aw);
  yield return Walk("SKIP_return_aisle",W(9.7f,3.6f,8.5f));yield return Walk("SKIP_exit",W(4.5f,3.6f,7),W(2.1f,4.75f,5.12f));
  var exit=s.schoolChapter.environment.GetComponentsInChildren<TuongTacTruongV37>().First(i=>i.exit&&i.name=="203_Return");yield return Press(exit,exit.transform.position,"SKIP_school_wake");yield return Wait(.5f);
  Check("skip_continues_no_award",s.data.region==-1&&s.data.nextRegion==3&&!s.data.schoolTrace&&s.data.awareness==aw);Snapshot("skip-final");
  foreach(var t in new[]{(0,0,"A"),(0,2,"A2"),(3,0,"B"),(3,2,"C"),(2,1,"A"),(2,2,"A2"),(3,1,"B")})Check("FIXTURE_resolver_"+t.Item1+"_"+t.Item2,DieuPhoiHanhTrinh.ResolveEnding(t.Item1,t.Item2)==t.Item3);
 }
}

#endif
