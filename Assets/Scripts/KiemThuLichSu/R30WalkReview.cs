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
public sealed class R30WalkReview:MonoBehaviour
{
 public static string Output=>Path.Combine(Path.GetTempPath(),"ConRoiR30Review");
 public string output;
 DieuPhoiHanhTrinh s;TraiNghiemTruongR28 ex;Camera eye;DieuKhienGocNhin mouse;TuongTacNguoiChoi pi;
 Stack<IEnumerator> stack=new Stack<IEnumerator>();JArray checks=new JArray(),errors=new JArray();StreamWriter timeline;
 int frame;bool filming=true,finished;string phase="startup";double deadline;
 static bool Flag(string name)=>Array.IndexOf(Environment.GetCommandLineArgs(),name)>=0;
 [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)] static void Boot(){
  if(Application.isEditor)return;var args=Environment.GetCommandLineArgs();int i=Array.IndexOf(args,"--r30-walk");if(i<0)return;
  var s=FindAnyObjectByType<DieuPhoiHanhTrinh>();s.gameObject.AddComponent<R30WalkReview>().output=i+1<args.Length?args[i+1]:Output;
 }
 void Start(){
  if(string.IsNullOrEmpty(output))output=Output;Directory.CreateDirectory(output);Directory.CreateDirectory(output+"/frames");
  s=FindAnyObjectByType<DieuPhoiHanhTrinh>();ex=s.GetComponent<TraiNghiemTruongR28>();eye=s.player.GetComponentInChildren<Camera>();mouse=s.player.GetComponentInChildren<DieuKhienGocNhin>();pi=s.player.GetComponent<TuongTacNguoiChoi>();
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
  File.WriteAllText(output+"/result.json",new JObject{["complete"]=pass,["failure"]=failure,["scene"]=gameObject.scene.path,["editor"]=Application.isEditor,["checks"]=checks,["errors"]=errors,["frames"]=frame,["method"]="New Game entry API; actual CharacterController movement via synthetic MovementFilter, E via real interaction ray and queued InputSystem. Camera steered by harness at unchanged player eye/FOV. No teleport or progress flag assignment during walkthrough. Branch fixtures reload saved school states after walkthrough. Not human/OS input acceptance."}.ToString());
  Debug.Log("R30_FINISH "+pass+" "+failure);
#if UNITY_EDITOR
  if(Application.isEditor){UnityEditor.EditorApplication.Exit(pass?0:1);return;}
#endif
  Application.Quit(pass?0:1);
 }
 void Check(string n,bool p,JToken d=null){checks.Add(new JObject{["name"]=n,["pass"]=p,["detail"]=d});File.WriteAllText(output+"/progress.json",checks.ToString());Debug.Log("R30_CHECK "+n+" "+p);}
 static JArray V(Vector3 p)=>new JArray(p.x,p.y,p.z);
 static Vector3 P(JToken p)=>new Vector3((float)p[0],(float)p[1],(float)p[2]);
 static Vector3 W(float x,float y,float z)=>new Vector3(-95+x,y,-45+z);
 void Stop(){if(s)s.player.MovementFilter=r=>Vector2.zero;}
 void Look(Vector3 at){mouse.enabled=false;var d=at-eye.transform.position;var flat=d;flat.y=0;if(flat.sqrMagnitude>.0001f)s.player.transform.rotation=Quaternion.LookRotation(flat);eye.transform.rotation=Quaternion.LookRotation(d);}
 void Capture(){
  if(Application.isBatchMode||Flag("--r31-render-request")){
   var rt=RenderTexture.GetTemporary(1280,720,24);var old=RenderTexture.active;var t=new Texture2D(1280,720,TextureFormat.RGB24,false);
   try{RenderPipeline.SubmitRenderRequest(eye,new UniversalRenderPipeline.SingleCameraRequest{destination=rt});if(!Flag("--r31-no-images")&&(!Flag("--r31-gd3-video")||phase=="class203_entry"||phase.StartsWith("GD3_"))){RenderTexture.active=rt;t.ReadPixels(new Rect(0,0,1280,720),0,0);t.Apply();File.WriteAllBytes(output+"/frames/"+frame.ToString("D6")+".jpg",t.EncodeToJPG(86));}}
   finally{RenderTexture.active=old;RenderTexture.ReleaseTemporary(rt);Destroy(t);}
  }else if(!Flag("--r31-no-images")){var t=ScreenCapture.CaptureScreenshotAsTexture();try{File.WriteAllBytes(output+"/frames/"+frame.ToString("D6")+".jpg",t.EncodeToJPG(86));}finally{Destroy(t);}}
  timeline.WriteLine(new JObject{["frame"]=frame++,["seconds"]=(frame-1)/12f,["phase"]=phase,["region"]=s.data.region,["position"]=V(s.player.transform.position),["camera"]=V(eye.transform.eulerAngles),["input"]=new JArray(s.player.LastMovementInput.x,s.player.LastMovementInput.y),["initial"]=ex.InitialSeconds,["lead"]=ex.LeadSeconds,["observed"]=ex.ObservedSeconds,["response"]=ex.ResponseProgress,["witnessed"]=s.data.schoolWitnessed,["puppetPaper"]=V(ex.puppetPaper.position),["minhPaper"]=V(ex.responsePaper.position),["puppets"]=new JArray(s.puppets.Select(p=>V(p.dauTransform.position)))}.ToString(Newtonsoft.Json.Formatting.None));
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
  if(Application.isBatchMode){var loader=s.GetComponent<ManHinhTaiV44>();loader.StopAllCoroutines();loader.enabled=false;s.SetLoading(false);File.WriteAllText(output+"/startup-scope.txt","NOT_RUN in batch: WaitForEndOfFrame loading gate bypassed. See separate normal Editor / native startup evidence. Gameplay walkthrough begins at New Game; no region progress seeded.");}
  else{yield return Until(()=>!s.Loading,25,"startup_loading");Check("real_loading_gate",s.GetComponent<ManHinhTaiV44>().Ready);}
  s.GetComponent<MenuVaoGameV47>().NewGame();Stop();yield return null;Check("new_game_hub",s.data.region==-1&&s.data.nextRegion==0&&!s.MenuOpen);
  yield return Bed(0);yield return Motion(0);
  var bao=Item(TuongTacHoCho.Kind.Bao);yield return Press(bao,s.ghosts[0].GetComponent<HienGuongMatTheoKhu>().ghostHead.position,"Bao");yield return Wait(2);
  // Retained bridge footway from the actual r23+ arrival, then its paved land link.
  yield return Walk("bridge_to_southeast",new Vector3(682.23f,1.14f,492.19f));
  yield return Walk("bridge_end",new Vector3(688.34f,1.14f,487.02f));
  yield return Walk("bridge_land_link",new Vector3(703.95f,.95f,484.53f));
  var bridge=Resources.Load<TextAsset>("VerificationV36/bridge");var link=JObject.Parse(bridge.text)["connections"][0].Select(P).Reverse().ToArray();
  foreach(var p in link.Where((p,i)=>i%4==0||i==link.Length-1))yield return Walk("lake_land_ramp",p);
  yield return Walk("lake_exit",new Vector3(718.7f,0,481.5f));var wake=Item(TuongTacHoCho.Kind.Wake);yield return Press(wake,wake.transform.position,"lake_wake");Check("lake_to_hub",s.data.region==-1&&s.data.nextRegion==1);
  yield return Bed(1);if(Flag("--r31-branches"))Snapshot("market-before");yield return Motion(1);
  if(Flag("--r31-branches"))Snapshot("market-after");
  if(Flag("--r31-stop-after-gd2"))yield break;
  var route=JObject.Parse(Resources.Load<TextAsset>("VerificationV36/route").text)["route"].Select(P).ToArray();
  for(int i=166;i<route.Length;i+=3)yield return Walk("market_street_"+i,route[i]);
  yield return Walk("market_Ha_approach",new Vector3(784.2f,5.5f,271.5f));var ha=Item(TuongTacHoCho.Kind.Ha);yield return Press(ha,s.ghosts[1].GetComponent<HienGuongMatTheoKhu>().ghostHead.position,"Ha");yield return Wait(2);
  yield return Walk("market_exit",new Vector3(783.35f,5.5f,275.15f));wake=Item(TuongTacHoCho.Kind.Wake);yield return Press(wake,wake.transform.position,"market_wake");Check("market_to_hub",s.data.region==-1&&s.data.nextRegion==2);
  yield return Bed(2);
  yield return Walk("school_gate",W(0,0,-25),W(-4,2,-17));yield return Walk("school_court",W(-6.5f,0,-5),W(0,5,1));
  yield return Walk("stair_approach",W(-6.5f,0,-3.45f),W(-6.5f,4.4f,4.7f));yield return Walk("stairs_up",W(-6.5f,3.6f,4.7f),W(-6.5f,5.2f,5));
  yield return Walk("gallery_start",W(-3.5f,3.6f,4.7f),W(4.6f,5.3f,5.4f));yield return Walk("gallery",W(4.6f,3.6f,3),W(4.6f,5.2f,6));
  yield return Walk("class203_entry",W(4.6f,3.6f,7),W(8,5,12));yield return Walk("GD3_approach",W(7.2f,3.6f,8.1f),W(6.4f,4.85f,10.6f));
  Stop();Look(W(6.4f,4.85f,10.6f));Snapshot("school-before");yield return Wait(4.8f,"GD3_initial_then_action");
  Check("GD3_first_without_E",ex.InitialSeen&&ex.Leading&&ex.LeadSeconds>=1.8f&&ex.ObservedSeconds>=1.1f&&(s.data.schoolReadMask&64)==0,new JObject{["initial"]=ex.InitialSeconds,["lead"]=ex.LeadSeconds,["seen"]=ex.ObservedSeconds});
  yield return Wait(Flag("--r31-gd3-video")?6:1,"GD3_result_hold");Snapshot("school-result");yield return Press(ex.responsePaper.GetComponent<TuongTacTruongR28>(),ex.responsePaper.position,"GD3_Minh_choice");yield return Wait(.25f,"GD3_Minh_slide");Snapshot("school-response-mid");yield return Wait(1,"GD3_Minh_result");Snapshot("school-after");Check("GD3_matching_choice",s.data.schoolWitnessed&&ex.ResponseProgress>=.99f);
  yield return Walk("class_aisle",W(9.7f,3.6f,9),W(10.4f,5,12.1f));yield return Walk("Mai_approach",W(9.7f,3.6f,10.6f),s.schoolChapter.face.ghostHead.position);
  var mai=s.schoolChapter.environment.GetComponentsInChildren<TuongTacTruongV37>().First(i=>i.name=="Mai_R28_Interaction");yield return Press(mai,mai.transform.position+Vector3.up*.55f,"Mai");Check("Mai_dialogue",s.data.schoolTalks>0);yield return Wait(2);
  yield return Walk("class_aisle_return",W(9.7f,3.6f,8.5f));yield return Walk("class_return",W(4.5f,3.6f,7),W(2.1f,4.75f,5.12f));
  var exit=s.schoolChapter.environment.GetComponentsInChildren<TuongTacTruongV37>().First(i=>i.exit&&i.name=="203_Return");yield return Press(exit,exit.transform.position,"school_wake");Check("school_to_hub",s.data.region==-1&&s.data.nextRegion==3);
  yield return Bed(3);Check("GD4_hidden_before_final",!s.slopeChapter.puppet.gameObject.activeInHierarchy);
  for(float z=2;z<=42;z+=2){float x=z<12?0:z<25?(z-12)*.10f:1.3f;yield return Walk("slope_up_"+z,new Vector3(-180+x,0,-60+z),new Vector3(-180+x,Mathf.Min(z*.2f,6)+1.6f,-56+z));}
  yield return Walk("slope_door",new Vector3(-178,0,-15.5f));var door=s.slopeChapter.environment.GetComponentsInChildren<TuongTacDocV39>().Single(i=>i.finalDoor);yield return Press(door,door.transform.position,"final_door");
  Look(s.slopeChapter.puppet.dauTransform.position-Vector3.up*.3f);Stop();var puppetPos=s.slopeChapter.puppet.transform.position;
  yield return Wait(3,"GD4_still");yield return Walk("GD4_Minh_step",s.player.transform.position+Vector3.left*.5f,s.slopeChapter.puppet.dauTransform.position);yield return Wait(2,"GD4_watch");
  Check("GD4_watches_once",s.data.finalStarts==1&&Vector3.Distance(puppetPos,s.slopeChapter.puppet.transform.position)<.001f&&s.data.slopeWitnessed);
  yield return Until(()=>!string.IsNullOrEmpty(s.data.ending),15,"ending_transition");Check("ending_reached",s.data.region==-1&&s.data.nextRegion==4);Snapshot("walkthrough-ending");
  filming=false;if(Flag("--r31-branches"))yield return MarketBranches();yield return Branches();
 }
 IEnumerator MarketBranches(){
  yield return Restore("market-after");var awareness=s.data.awareness;yield return Wait(.5f,"GD2_loaded_observed");
  Check("GD2_load_keeps_witness_without_reward",s.data.witnessed[1]&&(s.data.experienceR22Flags&16)!=0&&s.data.awareness==awareness);
  yield return Restore("market-before");awareness=s.data.awareness;Look(eye.transform.position+Vector3.forward*20);yield return Wait(3.6f,"GD2_missed_lookaway");
  s.player.MovementFilter=r=>new Vector2(.24f,0);yield return Wait(.3f);Stop();Snapshot("market-missed-pending");yield return Restore("market-missed-pending");Look(eye.transform.position+Vector3.forward*20);yield return Wait(2);
  Check("GD2_missed_load_no_invented_observation",!s.data.witnessed[1]&&(s.data.experienceR22Flags&16)==0&&s.data.awareness==awareness&&s.GetComponent<MoDauBenHoR23>().VisibleSeconds==0);
  var route=JObject.Parse(Resources.Load<TextAsset>("VerificationV36/route").text)["route"].Select(P).ToArray();
  for(int i=166;i<route.Length;i+=3)yield return Walk("missed_market_street_"+i,route[i],s.player.transform.position+Vector3.forward*20);
  yield return Walk("missed_market_exit",new Vector3(783.35f,5.5f,275.15f),s.player.transform.position+Vector3.forward*20);
  var wake=Item(TuongTacHoCho.Kind.Wake);yield return Press(wake,wake.transform.position,"missed_market_wake");
  Check("GD2_missed_can_continue_no_witness",s.data.region==-1&&s.data.nextRegion==2&&!s.data.witnessed[1]&&(s.data.experienceR22Flags&16)==0);
 }
 IEnumerator Branches(){
  yield return Restore("school-before");var aw=s.data.awareness;var q=eye.transform.rotation;Stop();Look(W(14,5,7));yield return Wait(3);Check("lookaway_no_action_or_awareness",!ex.Leading&&s.data.awareness==aw&&!s.data.schoolWitnessed);
  int beforeCycles=ex.Cycles;ex.Respond();yield return Wait(1);Check("early_E_never_triggers_puppet",!ex.Leading&&ex.Cycles==beforeCycles&&!s.data.schoolWitnessed&&(s.data.schoolReadMask&64)==0);
  yield return Restore("school-before");Stop();Look(W(6.4f,4.85f,10.6f));yield return Wait(1.8f);Snapshot("school-action-mid");float lead=ex.LeadSeconds,seen=ex.ObservedSeconds;var paper=ex.puppetPaper.position;int sounds=s.WoodEffectsPlayed;
  yield return Restore("school-action-mid");Check("mid_action_load_pose_observation",Mathf.Abs(ex.LeadSeconds-lead)<.18f&&Mathf.Abs(ex.ObservedSeconds-seen)<.18f&&Vector3.Distance(paper,ex.puppetPaper.position)<.08f);yield return Wait(3);Check("no_false_witness_without_response",!s.data.schoolWitnessed&&s.data.awareness==aw);
  Snapshot("school-no-response");var pose=eye.transform.rotation;var pos=s.player.transform.position;yield return Wait(4);Check("idle_no_penalty_or_camera",Quaternion.Angle(pose,eye.transform.rotation)<.01f&&Vector3.Distance(pos,s.player.transform.position)<.03f&&s.data.awareness==aw);
  yield return Restore("school-response-mid");Check("response_load_keeps_progress",ex.ResponseProgress>.05f&&ex.ResponseProgress<.9f);yield return Wait(1);Check("response_load_finishes",ex.ResponseProgress==1&&s.data.schoolWitnessed);
  int responses=ex.Responses;aw=s.data.awareness;ex.Respond();Check("duplicate_E_no_reward",ex.Responses==responses&&s.data.awareness==aw);
  yield return Restore("school-after");var finalPaper=ex.puppetPaper.position;seen=ex.ObservedSeconds;sounds=s.WoodEffectsPlayed;yield return Wait(.4f);Check("terminal_load_no_replay",Vector3.Distance(finalPaper,ex.puppetPaper.position)<.001f&&Mathf.Abs(ex.ObservedSeconds-seen)<.01f&&s.WoodEffectsPlayed==sounds);
  yield return Restore("school-before");Look(W(14,5,7));yield return Walk("missed_exit_aisle",W(9.7f,3.6f,8.5f));yield return Walk("missed_exit",W(4.5f,3.6f,7),W(2.1f,4.75f,5.12f));
  var exit=s.schoolChapter.environment.GetComponentsInChildren<TuongTacTruongV37>().First(i=>i.name=="203_Return");yield return Press(exit,exit.transform.position,"missed_school_wake");Check("missed_can_continue_no_witness",s.data.region==-1&&s.data.nextRegion==3&&!s.data.schoolWitnessed);
  Check("school_hidden_after_leave",!s.schoolChapter.environment.activeInHierarchy&&!s.slopeChapter.environment.activeInHierarchy&&s.hub.activeInHierarchy);
  // Compatibility fixture, separate from the completed on-foot walkthrough.
  var legacy=JObject.Parse(File.ReadAllText(output+"/school-before.json"));legacy.Remove("schoolStagingR30");legacy["schoolLeading"]=true;legacy["schoolLeadElapsed"]=1f;
  File.WriteAllText(s.SavePathOverride,legacy.ToString());s.Load();Stop();Look(W(14,5,7));
  var expectedPaper=ex.puppetPaperRest+new Vector3(.46f,.016f,0)*.5f;
  Check("legacy_r29_mid_action_preserves_paper_without_invented_observation",Vector3.Distance(ex.puppetPaper.localPosition,expectedPaper)<.001f&&ex.ObservedSeconds==0&&!s.data.schoolWitnessed&&(s.data.schoolReadMask&64)==0);
  yield return Wait(1);Check("legacy_missing_observation_no_false_reward",!s.data.schoolWitnessed);
  foreach(var tuple in new[]{(0,0,"A"),(0,2,"A2"),(3,0,"B"),(3,2,"C")})Check("ending_resolver_"+tuple.Item3,DieuPhoiHanhTrinh.ResolveEnding(tuple.Item1,tuple.Item2)==tuple.Item3);
 }
}

#endif
