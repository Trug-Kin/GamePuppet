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
public sealed class R34WalkReview:MonoBehaviour
{
 public static string Output=>Path.Combine(Path.GetTempPath(),"ConRoiR34Review");
 public string output;
 TuongTacGiayLop203R32 activity;
 DieuPhoiHanhTrinh s;TraiNghiemTruongR28 ex;Camera eye;DieuKhienGocNhin mouse;TuongTacNguoiChoi pi;
 Stack<IEnumerator> stack=new Stack<IEnumerator>();JArray checks=new JArray(),errors=new JArray();StreamWriter timeline;
 int frame;bool filming=true,finished;string phase="startup";double deadline;
 static bool Flag(string name)=>Array.IndexOf(Environment.GetCommandLineArgs(),name)>=0;
 [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)] static void Boot(){
  if(Application.isEditor)return;var args=Environment.GetCommandLineArgs();int i=Array.IndexOf(args,"--r34-walk");if(i<0)return;
  var s=FindAnyObjectByType<DieuPhoiHanhTrinh>();s.gameObject.AddComponent<R34WalkReview>().output=i+1<args.Length?args[i+1]:Output;
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
  File.WriteAllText(output+"/result.json",new JObject{["complete"]=pass,["failure"]=failure,["scene"]=gameObject.scene.path,["editor"]=Application.isEditor,["checks"]=checks,["errors"]=errors,["frames"]=frame,["method"]="Focused continuous school routes begin from a real r32 walkthrough save, then walk outside classroom before filming. Automated CharacterController movement, real ray E, synthetic keyboard; gameplay camera aim automated, offscreen, silent. No progress/position assignment in route. Edge fixtures load real route snapshots; ending resolver tested separately. Not human input or newcomer acceptance."}.ToString());
  Debug.Log("R34_FINISH "+pass+" "+failure);
#if UNITY_EDITOR
  if(Application.isEditor){UnityEditor.EditorApplication.Exit(pass?0:1);return;}
#endif
  Application.Quit(pass?0:1);
 }
 void Check(string n,bool p,JToken d=null){checks.Add(new JObject{["name"]=n,["pass"]=p,["detail"]=d});File.WriteAllText(output+"/progress.json",checks.ToString());Debug.Log("R34_CHECK "+n+" "+p);}
 static JArray V(Vector3 p)=>new JArray(p.x,p.y,p.z);
 static Vector3 P(JToken p)=>new Vector3((float)p[0],(float)p[1],(float)p[2]);
 static Vector3 W(float x,float y,float z)=>new Vector3(-95+x,y,-45+z);
 void Stop(){if(s)s.player.MovementFilter=r=>Vector2.zero;}
 void Look(Vector3 at){mouse.enabled=false;var d=at-eye.transform.position;var flat=d;flat.y=0;if(flat.sqrMagnitude>.0001f)s.player.transform.rotation=Quaternion.LookRotation(flat);eye.transform.rotation=Quaternion.LookRotation(d);}
 void Capture(){
  if(Application.isBatchMode||Flag("--r31-render-request")){
   var rt=RenderTexture.GetTemporary(1280,720,24);var old=RenderTexture.active;var t=new Texture2D(1280,720,TextureFormat.RGB24,false);
   try{RenderPipeline.SubmitRenderRequest(eye,new UniversalRenderPipeline.SingleCameraRequest{destination=rt});if(!Flag("--r31-no-images")){RenderTexture.active=rt;t.ReadPixels(new Rect(0,0,1280,720),0,0);t.Apply();File.WriteAllBytes(output+"/frames/"+frame.ToString("D6")+".jpg",t.EncodeToJPG(86));}}
   finally{RenderTexture.active=old;RenderTexture.ReleaseTemporary(rt);Destroy(t);}
  }else if(!Flag("--r31-no-images")){var t=ScreenCapture.CaptureScreenshotAsTexture();try{File.WriteAllBytes(output+"/frames/"+frame.ToString("D6")+".jpg",t.EncodeToJPG(86));}finally{Destroy(t);}}
  timeline.WriteLine(new JObject{["chairDistance"]=Vector3.Distance(s.player.transform.position,ex.traceChair.position),["schoolTrace"]=s.data.schoolTrace,["awareness"]=s.data.awareness,["actionPoints"]=s.data.actionPoints,["interactions"]=s.data.interactions,["placementPreview"]=activity.PlacementVisible,["dialogue"]=s.Dialogue,["frame"]=frame++,["seconds"]=(frame-1)/12f,["phase"]=phase,["region"]=s.data.region,["position"]=V(s.player.transform.position),["camera"]=V(eye.transform.eulerAngles),["input"]=new JArray(s.player.LastMovementInput.x,s.player.LastMovementInput.y),["initial"]=ex.InitialSeconds,["lead"]=ex.LeadSeconds,["observed"]=ex.ObservedSeconds,["response"]=ex.ResponseProgress,["witnessed"]=s.data.schoolWitnessed,["paperMemory"]=JObject.Parse(JsonUtility.ToJson(activity.memory)),["inspecting"]=activity.Inspecting,["realtime"]=Time.realtimeSinceStartup,["puppetPaper"]=V(ex.puppetPaper.position),["minhPaper"]=V(ex.responsePaper.position),["puppets"]=new JArray(s.puppets.Select(p=>V(p.dauTransform.position)))}.ToString(Newtonsoft.Json.Formatting.None));
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

 string Option(string name,string fallback){var a=Environment.GetCommandLineArgs();int i=Array.IndexOf(a,name);return i>=0&&i+1<a.Length?a[i+1]:fallback;}
 IEnumerator KeyPress(Key key){InputSystem.QueueStateEvent(Keyboard.current,new KeyboardState(key));yield return null;yield return null;InputSystem.QueueStateEvent(Keyboard.current,new KeyboardState());yield return null;yield return null;}
 IEnumerator Run(){
  yield return Until(()=>!s.Loading,35,"startup_loading");Check("real_startup_gate",s.GetComponent<ManHinhTaiV44>().Ready);
  s.GetComponent<MenuVaoGameV47>().NewGame();Stop();yield return null;
  filming=false;
  File.Copy(Option("--r34-seed","Docs/Reviews/journey_v48/r32/native-main/school-before.json"),output+"/entry.json",true);
  yield return Restore("entry");yield return Wait(.5f);
  Check("r34_integrated_scene",activity.completeR34&&ex.restoreChairTraceR33&&ex.paperOwnsResponseR34&&(s.gameObject.scene.path.EndsWith("r34.unity")||(Flag("--r35-school-regression")&&s.gameObject.scene.path.EndsWith("r35.unity"))));
  yield return Walk("setup_to_door",W(4.6f,3.6f,7));yield return Walk("setup_outside",W(4.6f,3.6f,3),W(4.6f,5.1f,6));
  Snapshot("outside");int aw=s.data.awareness,ap=s.data.actionPoints,ac=s.data.interactions;
  string branch=Option("--r34-branch","main");
  if(branch=="edge"){yield return Edges();yield break;}
  if(branch=="chair"){yield return Chair();yield break;}
  filming=true;yield return Wait(1,"OUTSIDE_class203");
  yield return Walk("ENTER_class203",W(4.6f,3.6f,7),W(8,5,12));
  if(branch=="skip"){
   Look(W(14,5,7));yield return Wait(1,"SKIP_leave_papers_untouched");
   yield return MeetMai("SKIP_Mai");Check("invitation_without_spoiler",s.Dialogue.Contains("có tên em"));
   yield return Exit("SKIP");
   Check("skip_no_points_or_witness",s.data.awareness==aw&&s.data.actionPoints==ap&&s.data.interactions==ac&&!s.data.schoolTrace&&!s.data.schoolWitnessed&&activity.memory.viewed==0&&activity.memory.places.All(x=>x==0));
   Check("skip_response_specific",s.Dialogue.Contains("Dòng cô gạch")&&activity.memory.choice==3);Snapshot("skip-final");yield break;
  }
  // Invitation comes from Mai, without revealing the model's location or the choice.
  yield return MeetMai("INVITATION_Mai");yield return Wait(2,"INVITATION_read");
  yield return Walk("FIND_aisle_back",W(9.7f,3.6f,8.5f));
  yield return Walk("FIND_Minh",W(7.2f,3.6f,8.1f),activity.papers[0].position);
  yield return Press(activity.papers[0].GetComponent<DichTuongTacGiayR32>(),activity.papers[0].position,"VIEW_Minh");yield return Wait(3,"READ_Minh");
  Check("view_does_not_take",activity.Inspecting&&activity.memory.places[0]==0&&activity.papers[0].gameObject.activeSelf);
  yield return KeyPress(Key.Tab);yield return null;yield return null;
  Check("view_close_returns_controls",s.player.enabled&&s.player.GameplayActionsEnabled);
  yield return Walk("FIND_model_aisle",W(9.7f,3.6f,8.5f),activity.papers[1].position);
  var m=activity.papers[1].position;
  // Keep to the side aisle; the stand point is outside the model desk collider.
  yield return Walk("FIND_model_desk",new Vector3(m.x-1.25f,3.6f,m.z-1.05f),m);
  yield return Press(activity.papers[1].GetComponent<DichTuongTacGiayR32>(),m,"VIEW_model");yield return Wait(3,"READ_model");
  Check("both_notes_seen",activity.memory.viewed==3);
  yield return KeyPress(Key.C);yield return Wait(4,"COMPARE_two_papers");
  var related=FindObjectsByType<UnityEngine.UI.Button>().First(b=>b.name.StartsWith("Đánh dấu:"));related.onClick.Invoke();yield return Wait(1,"COMPARE_manual_mark");
  Check("relation_records_no_points",activity.memory.related&&s.data.awareness==aw&&s.data.interactions==ac&&s.data.actionPoints==ap);
  yield return KeyPress(Key.C);
  if(branch=="main")yield return KeyPress(Key.F);else yield return KeyPress(Key.Tab);
  yield return null;yield return null;
  if(branch=="personal"){
   yield return Walk("KEEP_return_aisle",W(9.7f,3.6f,8.5f));yield return Walk("KEEP_Minh_desk",W(7.2f,3.6f,8.1f),activity.papers[0].position);
   Look(activity.papers[0].position);yield return null;UpdateInteraction();yield return KeyPress(Key.F);
   Check("direct_take_without_UI",activity.memory.places[0]==1&&!activity.Inspecting);
  }else{
   Check("take_model_from_reading",activity.memory.places[1]==1&&!activity.Inspecting);
   yield return Wait(2,"HELD_model_visible");yield return Walk("RETURN_model_aisle",W(9.7f,3.6f,8.5f));yield return Walk("RETURN_Minh_desk",W(7.2f,3.6f,8.1f));
  }
  Stop();Look(W(6.4f,4.85f,10.6f));yield return Wait(5.2f,"GD3_observe_initial_reach_slide");
  Check("puppet_first_no_forced_input",ex.InitialSeen&&ex.ObservedSeconds>=1.1f&&ex.ActionFinished&&!s.data.schoolWitnessed);
  yield return Wait(2,"GD3_hold_before_choice");
  var target=activity.tray.parent.GetComponentsInChildren<DichTuongTacGiayR32>().First(x=>x.paper<0&&x.inTray==(branch=="main"));
  Look(branch=="main"?activity.tray.position:activity.outside.position+new Vector3(-.23f,0,0));yield return null;UpdateInteraction();yield return Wait(1.5f,"CHOICE_placement_preview");
  Check("preview_at_valid_surface",activity.PlacementVisible);
  yield return Press(target,branch=="main"?activity.tray.position:activity.outside.position+new Vector3(-.23f,0,0),"CHOICE_"+branch);
  yield return Wait(4,"RESULT_Mai_and_physical_paper");
  Check("branch_choice",activity.memory.choice==(branch=="main"?1:2)&&activity.memory.places[branch=="main"?1:0]==(branch=="main"?2:3));
  Check("branch_response_content",s.Dialogue.Contains(branch=="main"?"Câu em muốn thử":"thử một tháng"));
  Check("choice_preserves_axes_chair",s.data.awareness==aw&&s.data.actionPoints==ap&&s.data.interactions==ac&&!s.data.schoolTrace);
  Check("witness_only_matching_response",s.data.schoolWitnessed==(branch=="main"));
  Snapshot("choice-final");yield return Restore("choice-final");yield return Wait(.5f);
  Check("saved_choice_and_pose",activity.memory.choice==(branch=="main"?1:2)&&Vector3.Distance(activity.papers[branch=="main"?1:0].position,branch=="main"?activity.tray.position:activity.outside.position)<.005f);
  filming=false;
 }
 IEnumerator MeetMai(string phaseName){
  yield return Walk(phaseName+"_aisle",W(9.7f,3.6f,9));yield return Walk(phaseName+"_approach",W(9.7f,3.6f,10.6f),s.schoolChapter.face.ghostHead.position);
  var mai=s.schoolChapter.environment.GetComponentsInChildren<TuongTacTruongV37>().First(i=>i.name=="Mai_R28_Interaction");
  yield return Press(mai,mai.transform.position+Vector3.up*.55f,phaseName);yield return Wait(2,phaseName+"_response");
 }
 IEnumerator Exit(string n){
  yield return Walk(n+"_return_aisle",W(9.7f,3.6f,8.5f));yield return Walk(n+"_door",W(4.5f,3.6f,7),W(2.1f,4.75f,5.12f));
  var exit=s.schoolChapter.environment.GetComponentsInChildren<TuongTacTruongV37>().First(i=>i.exit&&i.name=="203_Return");yield return Press(exit,exit.transform.position,n+"_wake");yield return Wait(2,n+"_continued");
  Check(n+"_continues",s.data.region==-1&&s.data.nextRegion==3);
 }
 IEnumerator Chair(){
  int aw=s.data.awareness;yield return Walk("chair_entry",W(4.6f,3.6f,7));yield return MeetMai("chair_Mai");Check("Mai_no_award",!s.data.schoolTrace&&s.data.awareness==aw);
  yield return Walk("chair_front",W(9.4f,3.6f,11.9f),ex.traceChair.position+Vector3.up*.65f);
  yield return Walk("chair_behind",W(9.4f,3.6f,14),ex.traceChair.position+Vector3.up*.65f);
  yield return Walk("chair_discover",W(11.7f,3.6f,14),ex.traceChair.position+Vector3.up*.65f);yield return Wait(1);
  Check("chair_plus_one",s.data.schoolTrace&&s.data.awareness==aw+1);Snapshot("chair-after");yield return Restore("chair-after");yield return Wait(.5f);
  Check("chair_load_keeps_pose_score",s.data.schoolTrace&&s.data.awareness==aw+1&&Quaternion.Angle(ex.traceChair.rotation,ex.chairBoard)<.1f);
  yield return Walk("chair_leave",W(9.4f,3.6f,14));yield return Walk("chair_revisit",W(11.7f,3.6f,14));Check("chair_exactly_once",s.data.awareness==aw+1);
 }
 IEnumerator Edges(){
  int aw=s.data.awareness,ap=s.data.actionPoints,ac=s.data.interactions;
  yield return Walk("edge_entry",W(4.6f,3.6f,7));yield return Walk("edge_Minh",W(7.2f,3.6f,8.1f));Snapshot("edge-start");
  yield return Press(activity.papers[0].GetComponent<DichTuongTacGiayR32>(),activity.papers[0].position,"edge_view");yield return Wait(.6f);yield return KeyPress(Key.F5);
  yield return KeyPress(Key.Escape);Check("pause_reading",s.Paused&&activity.Inspecting);float lead=ex.LeadSeconds;yield return null;yield return null;Check("pause_stops_staging",ex.LeadSeconds==lead);
  yield return KeyPress(Key.Escape);Check("resume_reading",!s.Paused&&activity.Inspecting&&!s.player.enabled);
  yield return KeyPress(Key.F6);yield return null;yield return null;Check("load_view_not_taken",!activity.Inspecting&&activity.memory.places[0]==0&&activity.papers[0].gameObject.activeSelf&&s.player.enabled);
  Look(activity.papers[0].position);yield return null;UpdateInteraction();yield return KeyPress(Key.F);yield return KeyPress(Key.F5);yield return KeyPress(Key.F6);yield return null;yield return null;
  Check("load_held_exactly_one",activity.memory.places[0]==1&&!activity.papers[0].gameObject.activeSelf&&activity.Selected==0);
  Check("wrong_tray_keeps_item",!activity.Place(true)&&activity.memory.places[0]==1);
  yield return KeyPress(Key.R);Check("return_held_to_source",activity.memory.places[0]==0&&activity.papers[0].gameObject.activeSelf);
  yield return Restore("edge-start");
  var m=activity.papers[1].position;yield return Walk("edge_model_aisle",W(9.7f,3.6f,8.5f));yield return Walk("edge_model",new Vector3(m.x-1.25f,3.6f,m.z-1.05f),m);
  Look(m);yield return null;UpdateInteraction();yield return KeyPress(Key.F);Check("edge_take_model",activity.memory.places[1]==1);
  yield return Walk("edge_back_aisle",W(9.7f,3.6f,8.5f));yield return Walk("edge_back_Minh",W(7.2f,3.6f,8.1f),W(14,5,7));
  var tray=activity.tray.parent.GetComponentsInChildren<DichTuongTacGiayR32>().First(x=>x.inTray);yield return Press(tray,activity.tray.position,"early_submit");
  Check("early_allowed_no_credit",activity.memory.early&&activity.memory.choice==1&&!s.data.schoolWitnessed&&!activity.memory.puppetSeen);
  var chosen=activity.papers[1].position;Look(W(6.4f,4.85f,10.6f));yield return Wait(5.2f);
  Check("later_observation_does_not_backdate",!s.data.schoolWitnessed&&Vector3.Distance(chosen,activity.papers[1].position)<.001f);
  int cycles=ex.Cycles;yield return Walk("after_choice_step",W(7.4f,3.6f,8.1f));Look(W(6.4f,4.85f,10.6f));yield return Wait(4);
  Check("no_repeat_or_paper_reset",ex.Cycles==cycles&&Vector3.Distance(chosen,activity.papers[1].position)<.001f);
  yield return Restore("edge-start");Look(W(6.4f,4.85f,10.6f));yield return Wait(2.1f);Snapshot("mid-animation");float mid=ex.LeadSeconds;yield return Restore("mid-animation");Check("mid_animation_load",Mathf.Abs(ex.LeadSeconds-mid)<.3f);Look(W(14,5,7));yield return Wait(3);
  Check("missed_event_not_seen",!activity.memory.puppetSeen&&!s.data.schoolWitnessed);cycles=ex.Cycles;
  yield return Walk("missed_move",W(7.4f,3.6f,8.1f));Look(W(6.4f,4.85f,10.6f));yield return Wait(4);Check("missed_no_replay",ex.Cycles==cycles&&!s.data.schoolWitnessed);
  yield return Restore("edge-start");Look(activity.papers[0].position);yield return null;UpdateInteraction();yield return KeyPress(Key.F);yield return Exit("abandon");
  Check("abandon_no_dream_item_in_hub",activity.memory.places.All(x=>x!=1)&&activity.memory.abandoned&&!activity.papers.Any(t=>t.gameObject.activeSelf));
  Check("edges_no_axis_points",s.data.awareness==aw&&s.data.actionPoints==ap&&s.data.interactions==ac);
 }
}

#endif
