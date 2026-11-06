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
public sealed class R36IntegrationReview:MonoBehaviour
{
 public static string Output=>Path.Combine(Path.GetTempPath(),"ConRoiR35Review");
 public string output;
 QuanLyGoiHangChoR35 activity;
 DieuPhoiHanhTrinh s;TraiNghiemTruongR28 ex;Camera eye;DieuKhienGocNhin mouse;TuongTacNguoiChoi pi;
 Stack<IEnumerator> stack=new Stack<IEnumerator>();JArray checks=new JArray(),errors=new JArray();StreamWriter timeline;
 int frame;bool filming=true,finished;string phase="startup";double deadline;
 static bool Flag(string name)=>name=="--r31-realtime"||name=="--r31-render-request"||Array.IndexOf(Environment.GetCommandLineArgs(),name)>=0;
 [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)] static void Boot(){
  if(Application.isEditor||FindAnyObjectByType<R36IntegrationReview>())return;var args=Environment.GetCommandLineArgs();int i=Array.IndexOf(args,"--r36-integration");if(i<0)return;
  var s=FindAnyObjectByType<DieuPhoiHanhTrinh>();new GameObject("R36_OptInReview").AddComponent<R36IntegrationReview>().output=i+1<args.Length?args[i+1]:Output;
 }
 void Start(){
  if(string.IsNullOrEmpty(output))output=Output;Directory.CreateDirectory(output);Directory.CreateDirectory(output+"/frames");
  s=FindAnyObjectByType<DieuPhoiHanhTrinh>();activity=s.GetComponent<QuanLyGoiHangChoR35>();ex=s.GetComponent<TraiNghiemTruongR28>();eye=s.player.GetComponentInChildren<Camera>();mouse=s.player.GetComponentInChildren<DieuKhienGocNhin>();pi=s.player.GetComponent<TuongTacNguoiChoi>();
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
 void OnDestroy(){Application.logMessageReceived-=Log;timeline?.Dispose();}
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
  File.WriteAllText(output+"/result.json",new JObject{["complete"]=pass,["failure"]=failure,["scene"]=s.gameObject.scene.path,["editor"]=Application.isEditor,["checks"]=checks,["errors"]=errors,["frames"]=frame,["method"]="Continuous market routes load an actual r31 market-entry walkthrough save, then walk the original street before filming. Automated CharacterController movement, real ray E, synthetic keyboard; gameplay camera aim automated, offscreen, silent. No progress/position assignment in route. Edge fixtures load real route snapshots; ending resolver tested separately. Not human input or newcomer acceptance."}.ToString());
  Debug.Log("R35_FINISH "+pass+" "+failure);
#if UNITY_EDITOR
  if(Application.isEditor){UnityEditor.EditorApplication.Exit(pass?0:1);return;}
#endif
  Application.Quit(pass?0:1);
 }
 void Check(string n,bool p,JToken d=null){checks.Add(new JObject{["name"]=n,["pass"]=p,["detail"]=d});File.WriteAllText(output+"/progress.json",checks.ToString());Debug.Log("R35_CHECK "+n+" "+p);}
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
  timeline.WriteLine(new JObject{["frame"]=frame++,["phase"]=phase,["region"]=s.data.region,["position"]=V(s.player.transform.position),["camera"]=V(eye.transform.eulerAngles),["realtime"]=Time.realtimeSinceStartup,["memory"]=JObject.Parse(JsonUtility.ToJson(activity.memory)),["dialogue"]=s.Dialogue,["heldLocal"]=activity.Held>=0?V(activity.parcels[activity.Held].transform.localPosition):null,["heldScale"]=activity.Held>=0?V(activity.parcels[activity.Held].transform.lossyScale):null,["fov"]=eye.fieldOfView,["preview"]=activity.PreviewVisible,["actionClock"]=activity.LastAction,["mimicClock"]=activity.LastMimic,["clock"]=activity.Clock,["witnessed"]=s.data.witnessed[1],["awareness"]=s.data.awareness,["actionPoints"]=s.data.actionPoints,["interactions"]=s.data.interactions}.ToString(Newtonsoft.Json.Formatting.None));
 }
 IEnumerator RecordFrames(){while(!finished){yield return new WaitForEndOfFrame();if(!finished&&filming&&!s.Loading){if(pendingPair!=null){CapturePair(pendingPair);pendingPair=null;}Capture();}}}
 IEnumerator Wait(float sec,string p=null){if(p!=null)phase=p;float end=Time.time+sec;while(Time.time<end)yield return null;}
 IEnumerator Until(Func<bool> ready,float seconds,string name){phase=name;float end=Time.realtimeSinceStartup+seconds;while(!ready()){if(Time.realtimeSinceStartup>end)throw new Exception(name+" timeout");yield return null;}}
 IEnumerator Walk(string name,Vector3 target,Vector3? look=null){
  phase=name;int frames=0,unsupported=0;Vector3 start=s.player.transform.position;
  while(new Vector2(target.x-s.player.transform.position.x,target.z-s.player.transform.position.z).magnitude>.18f&&frames++<240){
   Look(look??(target+Vector3.up*1.55f));var d=target-s.player.transform.position;d.y=0;
   float amount=Mathf.Min(.85f,d.magnitude/(s.player.speed*Mathf.Max(Time.deltaTime,1f/12)));
   var local=s.player.transform.InverseTransformDirection(d.normalized)*amount;s.player.MovementFilter=r=>new Vector2(local.x,local.z);
   yield return null;
   var b=s.player.controller.bounds;if(!Physics.Raycast(new Vector3(b.center.x,b.min.y+.25f,b.center.z),Vector3.down,.85f,8,QueryTriggerInteraction.Ignore))unsupported++;
  }
  Stop();Check(name,frames<240&&unsupported==0,new JObject{["position"]=V(s.player.transform.position),["from"]=V(start),["frames"]=frames,["unsupported"]=unsupported});
  if(frames>=240){Snapshot("blocked-walk");File.WriteAllText(output+"/blocking-colliders.json",new JArray(Physics.OverlapSphere(s.player.transform.position+Vector3.up,2,~0,QueryTriggerInteraction.Ignore).Select(c=>new JObject{["name"]=c.name,["center"]=V(c.bounds.center),["size"]=V(c.bounds.size)})).ToString());}else if(s.data.region==1)Snapshot("last-walk");if(frames>=240)throw new Exception("Blocked route "+name+" at "+s.player.transform.position);
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
 string pendingPair;
 void CapturePair(string name){
  var rows=new JArray();var shot=ScreenCapture.CaptureScreenshotAsTexture();
  if(shot){File.WriteAllBytes(output+"/"+name+"-screen.png",shot.EncodeToPNG());rows.Add(Pixels("ScreenCapture",shot));Destroy(shot);}
  var rt=RenderTexture.GetTemporary(1280,720,24);var old=RenderTexture.active;var tex=new Texture2D(1280,720,TextureFormat.RGB24,false);
  try{RenderPipeline.SubmitRenderRequest(eye,new UniversalRenderPipeline.SingleCameraRequest{destination=rt});RenderTexture.active=rt;tex.ReadPixels(new Rect(0,0,1280,720),0,0);tex.Apply();File.WriteAllBytes(output+"/"+name+"-URP.png",tex.EncodeToPNG());rows.Add(Pixels("ExplicitURP_NoOnGUI",tex));}finally{RenderTexture.active=old;RenderTexture.ReleaseTemporary(rt);Destroy(tex);}
  File.WriteAllText(output+"/"+name+"-capture-comparison.json",new JObject{["menu"]=s.MenuOpen,["editor"]=Application.isEditor,["focused"]=Application.isFocused,["methods"]=rows,["physicalWindowsPresentationVerified"]=false,["note"]="ScreenCapture and explicit URP use different capture paths. OnGUI menu is composed outside SingleCameraRequest. Hidden/offscreen samples do not certify a visible Windows window."}.ToString());
 }
 JObject Pixels(string method,Texture2D t){var px=t.GetPixels32();int lit=0;for(int i=0;i<px.Length;i+=97)if(px[i].r+px[i].g+px[i].b>6)lit++;return new JObject{["method"]=method,["width"]=t.width,["height"]=t.height,["nonblackSamples"]=lit,["sampleCount"]=(px.Length+96)/97};}
 IEnumerator Run(){
  yield return Until(()=>!s.Loading,40,"startup_loading");Check("real_loading",s.GetComponent<ManHinhTaiV44>().Ready);
  pendingPair="menu";yield return Until(()=>pendingPair==null,10,"menu_capture_endframe");
  
  RuntimeAudit("menu_clean");
  if(Option("--r35-branch","main")=="lifecycle"){yield return Lifecycle();yield break;}
  s.GetComponent<MenuVaoGameV47>().NewGame();Stop();yield return null;filming=false;
  File.Copy(Option("--r35-seed","Docs/Reviews/journey_v48/r35/editor-transform-blocked/last-walk.json"),output+"/entry.json",true);
  yield return Restore("entry");yield return Wait(.5f);
  Check("r35_scene_and_r34_school",activity&&s.gameObject.scene.path.EndsWith("r36.unity")&&s.GetComponent<TuongTacGiayLop203R32>().completeR34);
  Check("legacy_progress_retained",activity.memory.viewed==0&&activity.Held==-1&&s.data.nextRegion==1&&s.data.awareness==1);
  var route=JObject.Parse(Resources.Load<TextAsset>("VerificationV36/route").text)["route"].Select(P).ToArray();
  if(Vector3.Distance(s.player.transform.position,new Vector3(784.8f,5.7f,271.1f))>20){
   for(int i=166;i<244;i+=3)yield return Walk("setup_market_street_"+i,route[i]);
   yield return Walk("setup_cross_market",new Vector3(779.5f,5.5f,274.25f));
   yield return Walk("setup_front_aisle",new Vector3(779.5f,5.5f,271.1f));
  }
  if(s.player.transform.position.z>272.5f){yield return Walk("setup_outside_tables",new Vector3(781.1f,5.5f,274.25f));yield return Walk("setup_around_tables",new Vector3(781.1f,5.5f,271.1f));}
  yield return Walk("setup_before_stall",new Vector3(781.1f,5.5f,271.1f));
  Snapshot("before-stall");int aw=s.data.awareness,ap=s.data.actionPoints,ac=s.data.interactions;
  string branch=Option("--r35-branch","main");
  filming=true;Look(activity.slip.position);yield return Wait(2,"ARRIVE_Ha_stall");pendingPair="gameplay";yield return Until(()=>pendingPair==null,10,"ARRIVE_capture_endframe");
  if(branch=="skip"){
   yield return ExitMarket("SKIP");Check("skip_no_activity_award",activity.memory.viewed==0&&activity.Held<0&&s.data.awareness==aw&&s.data.interactions==ac);yield break;
  }
  yield return Walk("APPROACH_slip",new Vector3(785.3f,5.5f,271.25f),activity.slip.position);
  yield return Press(activity.slip.GetComponent<DichTuongTacChoR35>(),activity.slip.position,"VIEW_slip");yield return Wait(3,"READ_slip");yield return KeyPress(Key.R);
  Check("slip_optional_no_inventory",!s.polish.IsReading&&activity.Held<0&&(activity.memory.viewed&1)!=0);
  yield return Pick(0,"FIRST_red");Look(eye.transform.position+Vector3.forward*10);yield return null;UpdateInteraction();yield return KeyPress(Key.E);yield return Wait(2,"OPEN_red_actual_scarf");
  Check("red_open_visible_content",activity.Held==0&&activity.memory.opened[0]&&activity.parcels[0].scarf.colour==KhanTrongGoiHangR35.Colour.BrickRed&&activity.parcels[0].transform.IsChildOf(eye.transform));
  Snapshot("holding-open");yield return KeyPress(Key.F6);yield return Wait(.3f,"LOAD_open_held");Check("load_open_held",activity.Held==0&&activity.memory.opened[0]);RuntimeAudit("held_after_load");
  if(branch=="edge"){
   yield return KeyPress(Key.Escape);float clock=activity.Clock;var lid=activity.parcels[0].lid.localRotation;
   phase="PAUSE_real_time";float end=Time.realtimeSinceStartup+1.2f;while(Time.realtimeSinceStartup<end)yield return null;
   RuntimeAudit("paused");Check("pause_clock_and_lid_frozen",s.Paused&&activity.Clock==clock&&activity.parcels[0].lid.localRotation==lid);
   yield return KeyPress(Key.F5);yield return KeyPress(Key.F6);Check("save_load_while_paused_resumes_original_flow",!s.Paused&&s.player.enabled&&activity.Held==0&&activity.memory.opened[0]);
   Look(eye.transform.position+Vector3.back*10);yield return null;UpdateInteraction();yield return KeyPress(Key.E);yield return Wait(1.2f,"MISSED_GD2_look_away");
   Check("missed_does_not_award",!s.data.witnessed[1]);Snapshot("closed-held");yield return KeyPress(Key.F6);yield return Wait(.3f);Check("load_closed_held",activity.Held==0&&!activity.memory.opened[0]);
   yield return KeyPress(Key.R);Check("return_safe_home",activity.Held<0&&activity.memory.places[0]==0);
   yield return Pick(1,"EARLY_green");yield return Stage("EARLY_stage");Snapshot("staged");yield return KeyPress(Key.F6);yield return Wait(.3f);Check("load_staged_no_duplicates",activity.Candidate==1&&activity.Held<0);
   yield return Press(activity.parcels[1],activity.parcels[1].transform.position+Vector3.up*.05f,"RECLAIM_staged");Check("reclaimed",activity.Held==1&&activity.Candidate<0);
   yield return Walk("ABANDON_leave_stall",new Vector3(778.8f,5.5f,271.1f));yield return Wait(.4f);Check("leave_returns_dream_item",activity.Held<0&&activity.memory.abandoned);
   yield return Pick(0,"LEAVE_holding_red");yield return ExitMarket("EDGE");Check("wake_returns_held_item",activity.Held<0&&activity.parcels.All(p=>!p.gameObject.activeInHierarchy));Check("edge_no_points",s.data.awareness==aw&&s.data.actionPoints==ap&&s.data.interactions==ac);
   File.Copy("Docs/Reviews/journey_v48/r32/native-main/school-before.json",s.SavePathOverride,true);s.Load();yield return null;Check("old_school_save_stays_school",s.data.region==2&&s.data.schoolTalks>=0&&activity.Held<0&&!activity.root.gameObject.activeInHierarchy);yield break;
  }
  yield return KeyPress(Key.R);yield return Pick(1,"SECOND_green");
  Look(eye.transform.position+Vector3.forward*10);yield return null;UpdateInteraction();yield return KeyPress(Key.E);yield return Wait(2,"OPEN_green_actual_scarf");
  Check("both_contents_seen",activity.memory.viewed==7&&activity.memory.opened[1]);
  if(branch=="notchosen"){
   yield return KeyPress(Key.R);yield return MeetHa("NOT_CHOSEN_Ha");yield return Wait(4,"NOT_CHOSEN_response");
   Check("not_chosen_both_home",activity.memory.notChosen&&activity.memory.places.All(p=>p==0)&&activity.LastResponse.Contains("giữ phiếu"));yield return ExitMarket("NOT_CHOSEN");yield break;
  }
  // Repeat supported gesture while the visible puppet can be seen, then stop.
  yield return Wait(1);Look(activity.gesture.position+Vector3.down*.10f);yield return null;UpdateInteraction();
  Check("gesture_context_is_clear",pi.Target==null);
  float previous=activity.LastMimic;yield return KeyPress(Key.E);
  Check("mimic_delay_respected",activity.LastMimic==previous||activity.LastMimic-activity.LastAction>=.49f);yield return Wait(1.25f,"GD2_real_delay_and_tail");
  Check("GD2_observed_with_visible_tail",s.data.witnessed[1]&&activity.SeenMotion>=.30f,new JObject{["delaySeconds"]=activity.LastMimic-activity.LastAction,["visibleMotion"]=activity.SeenMotion,["captureDeltaTime"]=Time.captureDeltaTime});
  Check("real_half_second",activity.LastMimic-activity.LastAction>=.49f&&activity.LastMimic-activity.LastAction<.8f&&Time.captureDeltaTime==0);
  // Change a staged candidate through world interaction before final confirmation.
  yield return Stage("STAGE_green_preview");yield return Wait(1,"PLACED_green");
  yield return Pick(0,"CHANGE_pick_red");yield return Stage("CHANGE_stage_red");Check("replace_returns_green_home",activity.Candidate==0&&activity.memory.places[1]==0);
  if(branch!="red"){
   yield return Press(activity.parcels[0],activity.parcels[0].transform.position+Vector3.up*.05f,"RECLAIM_red");yield return KeyPress(Key.R);
   yield return Pick(1,"FINAL_pick_green");yield return Stage("FINAL_stage_green");
  }
  yield return MeetHa("CONFIRM_Ha");yield return Wait(4,"RESULT_content_specific_Ha");
  int expected=branch=="red"?0:1;Check("confirmed_actual_content",activity.memory.confirmed==expected&&activity.Candidate==expected&&activity.LastResponse.Contains(expected==0?"đỏ gạch":"Xanh rêu"));
  Check("face_original_once",s.data.faces[1]);Snapshot("confirmed");yield return KeyPress(Key.F6);yield return Wait(.4f,"LOAD_confirmed_no_dialogue_replay");
  Check("confirmed_load_stable",activity.memory.confirmed==expected&&activity.Candidate==expected&&activity.Held<0&&activity.LastResponse=="");
  Check("no_new_ending_points",s.data.awareness==aw&&s.data.actionPoints==ap&&s.data.interactions==ac);
  yield return ExitMarket("DELIVERED");RuntimeAudit("hub_after_market");Check("dream_items_stay_in_market",activity.Held<0&&activity.parcels.All(p=>!p.gameObject.activeInHierarchy));
 }
 IEnumerator Pick(int id,string name){
  var p=activity.parcels[id];yield return Walk(name+"_approach",new Vector3(p.transform.position.x,5.5f,271.25f),p.transform.position);
  yield return Press(p,p.transform.position+Vector3.up*.05f,name);Check(name+"_held",activity.Held==id);yield return Wait(.5f);
 }
 IEnumerator Stage(string name){
  yield return Walk(name+"_approach",new Vector3(785.8f,5.5f,271.25f),activity.delivery.position);Look(activity.delivery.position);yield return null;UpdateInteraction();yield return Wait(.4f,name);
  Check(name+"_preview",activity.PreviewVisible);if(activity.Candidate>=0)yield return Press(activity.parcels[activity.Candidate],activity.parcels[activity.Candidate].transform.position+Vector3.up*.05f,name+"_replace_E");else yield return Press(activity.delivery.GetComponent<DichTuongTacChoR35>(),activity.delivery.position,name+"_E");
 }
 IEnumerator MeetHa(string name){
  yield return Walk(name+"_side",new Vector3(786.8f,5.5f,271.1f));
  var ha=Item(TuongTacHoCho.Kind.Ha);yield return Press(ha,s.ghosts[1].GetComponent<HienGuongMatTheoKhu>().ghostHead.position,name);
 }
 IEnumerator ExitMarket(string name){
  yield return Walk(name+"_aisle",new Vector3(782.8f,5.5f,271.1f));yield return Walk(name+"_exit",new Vector3(781.1f,5.5f,273.2f));
  var wake=Item(TuongTacHoCho.Kind.Wake);yield return Press(wake,wake.transform.position+Vector3.up*.7f,name+"_wake");yield return Wait(.5f,name+"_continued");Check(name+"_main_flow_continues",s.data.region==-1&&s.data.nextRegion==2);Snapshot(name+"-final");
 }
 void RuntimeAudit(string label){
  var all=s.gameObject.scene.GetRootGameObjects().SelectMany(g=>g.GetComponentsInChildren<Transform>(true)).ToArray();
  var actions=typeof(QuanLyGoiHangChoR35).GetFields(System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Instance).Where(f=>f.FieldType==typeof(InputAction)).Select(f=>(InputAction)f.GetValue(activity)).Where(a=>a!=null).ToArray();
  var canvases=all.Select(t=>t.GetComponent<Canvas>()).Where(c=>c!=null).ToArray();
  var cameras=all.Select(t=>t.GetComponent<Camera>()).Where(c=>c!=null).ToArray();
  var j=new JObject{["label"]=label,["region"]=s.data.region,["menu"]=s.MenuOpen,["pause"]=s.Paused,["phone"]=s.PhoneOpen,["movementEnabled"]=s.player.GameplayActionsEnabled,["interactionEnabled"]=pi.enabled,["parcelReady"]=activity.Ready,["parcelPollingActions"]=new JArray(actions.Select(a=>new JObject{["name"]=a.name,["enabled"]=a.enabled})),["canvases"]=new JArray(canvases.Select(c=>new JObject{["name"]=c.name,["mode"]=c.renderMode.ToString(),["camera"]=c.worldCamera?c.worldCamera.name:"NULL",["sorting"]=c.sortingOrder})),["eventSystems"]=all.Count(t=>t.GetComponent<UnityEngine.EventSystems.EventSystem>()),["inputModules"]=all.Sum(t=>t.GetComponents<UnityEngine.EventSystems.BaseInputModule>().Length),["playerInputs"]=all.Count(t=>t.GetComponent<PlayerInput>()),["players"]=all.Count(t=>t.GetComponent<DieuKhienDiChuyen>()),["cameras"]=all.Count(t=>t.GetComponent<Camera>()),["listeners"]=all.Count(t=>t.GetComponent<AudioListener>()),["stories"]=all.Count(t=>t.GetComponent<DieuPhoiHanhTrinh>()),["parcelInstances"]=all.Count(t=>t.GetComponent<GoiHangChoR35>()),["contextPanels"]=all.Count(t=>t.name=="R35_ParcelContext"),["requiredScenePresent"]=activity.root&&activity.slip&&activity.delivery&&activity.parcels.All(p=>p&&p.activity==activity&&p.scarf&&p.lid)};
  j["cameraDetails"]=new JArray(cameras.Select(c=>new JObject{["name"]=c.name,["enabled"]=c.enabled,["active"]=c.gameObject.activeInHierarchy,["mask"]=c.cullingMask,["targetTexture"]=c.targetTexture?c.targetTexture.name:"NULL",["reflectionOwner"]=c.GetComponentInParent<PhanChieuMatHo>()!=null}));
  File.WriteAllText(output+"/integration-"+label+".json",j.ToString());
  Check(label+"_one_player_camera_listener_manager",(int)j["players"]==1&&cameras.Count(c=>c.enabled&&c.gameObject.activeInHierarchy)==1&&cameras.All(c=>c==eye||!c.enabled&&(c.name=="Lake_Reflection_Camera"&&c.GetComponentInParent<PhanChieuMatHo>()||c.name=="R32_inspection_camera"&&c.targetTexture&&c.cullingMask==(1<<30)))&&(int)j["listeners"]==1&&(int)j["stories"]==1,j["cameraDetails"]);
  Check(label+"_two_packages_one_panel",(int)j["parcelInstances"]==2&&(int)j["contextPanels"]==1&&(bool)j["requiredScenePresent"]);
  Check(label+"_display_only_UI_no_duplicate_module",canvases.All(c=>c.renderMode!=RenderMode.ScreenSpaceCamera||c.worldCamera==eye)&&(int)j["inputModules"]<=1&&(int)j["playerInputs"]==0);
  Check(label+"_controls_match_story",s.player.GameplayActionsEnabled==(!s.MenuOpen&&!s.Loading&&!s.Paused&&!s.PhoneOpen&&!s.SupplementInspection&&string.IsNullOrEmpty(s.data.ending)));
 }
 void RenderStill(string name){
  var rt=RenderTexture.GetTemporary(1280,720,24);var old=RenderTexture.active;var tex=new Texture2D(1280,720,TextureFormat.RGB24,false);
  try{RenderPipeline.SubmitRenderRequest(eye,new UniversalRenderPipeline.SingleCameraRequest{destination=rt});RenderTexture.active=rt;tex.ReadPixels(new Rect(0,0,1280,720),0,0);tex.Apply();File.WriteAllBytes(output+"/"+name+".png",tex.EncodeToPNG());}finally{RenderTexture.active=old;RenderTexture.ReleaseTemporary(rt);Destroy(tex);}
 }
 IEnumerator Lifecycle(){
  filming=false;s.GetComponent<MenuVaoGameV47>().NewGame();yield return null;RuntimeAudit("new_game");Check("new_game_is_hub",s.data.region==-1&&activity.Held<0);
  File.Copy(Option("--r35-seed","Docs/Reviews/journey_v48/r35/editor-main/before-stall.json"),s.SavePathOverride,true);
  // Exercise the real menu Continue route, not an independent test bootstrap.
  s.SetMenu(true);s.GetComponent<MenuVaoGameV47>().Continue();yield return Wait(.5f);Stop();RuntimeAudit("menu_continue_market");Check("continue_retains_market_save",s.data.region==1&&s.data.nextRegion==1);
  yield return Walk("fixture_approach",new Vector3(785.0f,5.5f,271.25f));
  yield return Pick(0,"fixture_red");Look(eye.transform.position+Vector3.forward*10);yield return null;UpdateInteraction();yield return KeyPress(Key.E);yield return Wait(.6f);
  // Same frame, camera, geometry and lid state. Only world text materials differ.
  var ts=s.gameObject.scene.GetRootGameObjects().SelectMany(g=>g.GetComponentsInChildren<TextMesh>(true)).Where(t=>Vector3.Distance(t.transform.position,activity.delivery.position)<16).ToArray();
  var rs=ts.Select(t=>t.GetComponent<Renderer>()).ToArray();var mats=rs.Select(r=>r.sharedMaterial).ToArray();
  RenderStill("after-open");for(int i=0;i<ts.Length;i++)rs[i].sharedMaterial=(ts[i].font?ts[i].font:Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf")).material;
  RenderStill("before-open");for(int i=0;i<rs.Length;i++)rs[i].sharedMaterial=mats[i];
  File.WriteAllText(output+"/comparison-state.json",new JObject{["cameraPosition"]=V(eye.transform.position),["cameraEuler"]=V(eye.transform.eulerAngles),["fov"]=eye.fieldOfView,["heldPosition"]=V(activity.parcels[0].transform.position),["lidEuler"]=V(activity.parcels[0].lid.localEulerAngles),["memory"]=JObject.Parse(JsonUtility.ToJson(activity.memory)),["method"]="Controlled rendering fixture in one frame; before restores source r35 built-in font material only, after uses saved r36 scene material. No geometry/camera/light changes."}.ToString());
  yield return KeyPress(Key.R);Look(eye.transform.position+Vector3.forward*10);yield return null;RenderStill("after-unblocked");
  for(int i=0;i<ts.Length;i++)rs[i].sharedMaterial=(ts[i].font?ts[i].font:Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf")).material;
  RenderStill("before-unblocked");for(int i=0;i<rs.Length;i++)rs[i].sharedMaterial=mats[i];
  yield return KeyPress(Key.P);RuntimeAudit("phone_open");yield return KeyPress(Key.P);RuntimeAudit("phone_closed");
  Snapshot("reload-market");int loads=s.SuccessfulLoads;for(int i=0;i<3;i++){yield return KeyPress(Key.F6);yield return null;}Check("three_loads_once_each",s.SuccessfulLoads==loads+3);RuntimeAudit("three_loads");
  // One normal E must still produce exactly one interaction after UI/load cycles.
  yield return Press(activity.slip.GetComponent<DichTuongTacChoR35>(),activity.slip.position,"after_load_read");yield return KeyPress(Key.R);
  yield return ExitMarket("lifecycle");RuntimeAudit("transition_hub");
  File.Copy(output+"/reload-market.json",s.SavePathOverride,true);s.SetMenu(true);s.GetComponent<MenuVaoGameV47>().Continue();yield return null;Stop();RuntimeAudit("return_market");
  // Test runner survives; all product objects are destroyed and reconstructed by SceneManager.
  string scene=s.gameObject.scene.path,savePath=s.SavePathOverride;DontDestroyOnLoad(gameObject);s=null;activity=null;eye=null;mouse=null;pi=null;
  var loading=UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(scene);while(!loading.isDone)yield return null;
  s=FindAnyObjectByType<DieuPhoiHanhTrinh>();activity=s.GetComponent<QuanLyGoiHangChoR35>();eye=s.player.GetComponentInChildren<Camera>();mouse=s.player.GetComponentInChildren<DieuKhienGocNhin>();pi=s.player.GetComponent<TuongTacNguoiChoi>();s.SavePathOverride=savePath;
  yield return Until(()=>!s.Loading,40,"clean_scene_reload");yield return null;RuntimeAudit("scene_reloaded_menu");
  s.GetComponent<MenuVaoGameV47>().Continue();yield return null;Stop();RuntimeAudit("scene_reloaded_continue");Check("reload_then_continue_market",s.data.region==1&&activity.Held<0);
 }

}

#endif
