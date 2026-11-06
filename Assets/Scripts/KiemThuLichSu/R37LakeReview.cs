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

// Explicit review only. Product scene contains no fixture; input/aim automation is not physical OS input.
[DefaultExecutionOrder(30000)] public sealed class R37LakeReview:MonoBehaviour {
 public string output;DieuPhoiHanhTrinh s;TuongTacSoTayHoR37 n;Camera eye;DieuKhienGocNhin mouse;TuongTacNguoiChoi pi;
 readonly Stack<IEnumerator> stack=new Stack<IEnumerator>();readonly JArray checks=new JArray(),errors=new JArray();StreamWriter timeline;
 string phase="startup",branch;bool finished,filming;int frame;double deadline;
 static string Option(string key,string fallback){var a=Environment.GetCommandLineArgs();int i=Array.IndexOf(a,key);return i>=0&&i+1<a.Length?a[i+1]:fallback;}
 [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)] static void Boot(){if(Application.isEditor||FindAnyObjectByType<R37LakeReview>())return;string p=Option("--r37-lake","");if(p.Length>0)new GameObject("R37_OptInReview").AddComponent<R37LakeReview>().output=p;}
 void Bind(){s=FindAnyObjectByType<DieuPhoiHanhTrinh>();n=s.GetComponent<TuongTacSoTayHoR37>();eye=s.player.GetComponentInChildren<Camera>();mouse=s.player.GetComponentInChildren<DieuKhienGocNhin>();pi=s.player.GetComponent<TuongTacNguoiChoi>();}
 void Start(){
  Bind();branch=Option("--r37-branch","main");Directory.CreateDirectory(output);Directory.CreateDirectory(output+"/frames");s.SavePathOverride=output+"/isolated-save.json";
  Application.runInBackground=true;InputSystem.settings=Instantiate(InputSystem.settings);InputSystem.settings.updateMode=InputSettings.UpdateMode.ProcessEventsInDynamicUpdate;InputSystem.settings.backgroundBehavior=InputSettings.BackgroundBehavior.IgnoreFocus;InputSystem.settings.editorInputBehaviorInPlayMode=InputSettings.EditorInputBehaviorInPlayMode.AllDeviceInputAlwaysGoesToGameView;
  InputSystem.AddDevice<Keyboard>("R37ReviewKeyboard").MakeCurrent();QualitySettings.vSyncCount=0;Application.targetFrameRate=12;Time.captureDeltaTime=0;timeline=new StreamWriter(output+"/timeline.jsonl"){AutoFlush=true};Application.logMessageReceived+=Log;deadline=Time.realtimeSinceStartupAsDouble+1200;stack.Push(Run());StartCoroutine(Record());
 }
 void OnDestroy(){Application.logMessageReceived-=Log;timeline?.Dispose();}
 void Log(string m,string trace,LogType t){if(t==LogType.Error||t==LogType.Exception)errors.Add(new JObject{["message"]=m,["trace"]=trace});}
 void LateUpdate(){if(finished)return;try{if(Time.realtimeSinceStartupAsDouble>deadline)throw new Exception("Review timeout");if(mouse)mouse.enabled=false;while(stack.Count>0){var e=stack.Peek();if(!e.MoveNext()){stack.Pop();continue;}if(e.Current is IEnumerator nested){stack.Push(nested);continue;}break;}if(stack.Count==0)Finish(null);}catch(Exception e){Finish(e.ToString());}}
 void Finish(string failure){finished=true;Stop();timeline?.Dispose();bool pass=failure==null&&errors.Count==0&&checks.All(c=>(bool)c["pass"]);File.WriteAllText(output+"/result.json",new JObject{["complete"]=pass,["failure"]=failure,["scene"]=s?s.gameObject.scene.path:"reloading",["editor"]=Application.isEditor,["branch"]=branch,["checks"]=checks,["errors"]=errors,["frames"]=frame,["scope"]="Dynamic synthetic keyboard, original CharacterController and actual E ray. Real-time captureDeltaTime=0; automated camera, explicit URP offscreen, silent. No item/progress injection during recorded walkthrough. Old-save fixtures are separate from continuous sequence. Physical Windows input/display NOT_VERIFIED."}.ToString());Debug.Log("R37_FINISH "+pass+" "+failure);
#if UNITY_EDITOR
  if(Application.isEditor){UnityEditor.EditorApplication.Exit(pass?0:1);return;}
#endif
  Application.Quit(pass?0:1);
 }
 void Check(string name,bool pass,JToken detail=null){checks.Add(new JObject{["name"]=name,["pass"]=pass,["detail"]=detail});File.WriteAllText(output+"/progress.json",checks.ToString());Debug.Log("R37_CHECK "+name+" "+pass);}
 static JArray V(Vector3 v)=>new JArray(v.x,v.y,v.z);
 static Vector3 P(JToken p)=>new Vector3((float)p[0],(float)p[1],(float)p[2]);
 void Stop(){if(s)s.player.MovementFilter=r=>Vector2.zero;}
 void Look(Vector3 at){mouse.enabled=false;var d=at-eye.transform.position;var flat=d;flat.y=0;if(flat.sqrMagnitude>.0001f)s.player.transform.rotation=Quaternion.LookRotation(flat);eye.transform.rotation=Quaternion.LookRotation(d);}
 IEnumerator Wait(float seconds,string name=null){if(name!=null)phase=name;double end=Time.realtimeSinceStartupAsDouble+seconds;while(Time.realtimeSinceStartupAsDouble<end)yield return null;}
 IEnumerator Until(Func<bool> condition,float seconds,string name){phase=name;double end=Time.realtimeSinceStartupAsDouble+seconds;while(!condition()){if(Time.realtimeSinceStartupAsDouble>end)throw new Exception(name+" timeout");yield return null;}}
 IEnumerator KeyPress(Key key){InputSystem.QueueStateEvent(Keyboard.current,new KeyboardState(key));yield return null;yield return null;InputSystem.QueueStateEvent(Keyboard.current,new KeyboardState());yield return null;yield return null;}
 void UpdateRay(){typeof(TuongTacNguoiChoi).GetMethod("Update",System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Instance).Invoke(pi,null);}
 IEnumerator Press(IPlayerInteractable target,Vector3 aim,string name){phase=name;Stop();Look(aim);yield return null;UpdateRay();if(pi.Target!=target){var box=(target as Component).GetComponent<Collider>();if(box){foreach(var p in new[]{box.bounds.center,box.bounds.center+Vector3.up*box.bounds.extents.y*.95f,box.bounds.center+Vector3.right*box.bounds.extents.x*.85f}){Look(p);UpdateRay();if(pi.Target==target)break;}}}if(pi.Target!=target){File.WriteAllText(output+"/blocked-ray.json",new JArray(Physics.RaycastAll(eye.transform.position,eye.transform.forward,4,~0,QueryTriggerInteraction.Ignore).OrderBy(h=>h.distance).Select(h=>new JObject{["name"]=h.collider.name,["distance"]=h.distance,["point"]=V(h.point)})).ToString());throw new Exception("Wrong E target "+name+" actual="+pi.Target);}int before=pi.CompletedInteractions;yield return KeyPress(Key.E);Check(name+"_one_ray_E",pi.CompletedInteractions==before+1);}
 IEnumerator Walk(Vector3 target,string name,Vector3? look=null){phase=name;int count=0;while(new Vector2(target.x-s.player.transform.position.x,target.z-s.player.transform.position.z).magnitude>.16f&&count++<320){Look(look??(target+Vector3.up*1.6f));var d=target-s.player.transform.position;d.y=0;float amount=Mathf.Min(.8f,d.magnitude/(s.player.speed*Mathf.Max(Time.deltaTime,.0833f)));var local=s.player.transform.InverseTransformDirection(d.normalized)*amount;s.player.MovementFilter=r=>new Vector2(local.x,local.z);yield return null;}Stop();Check(name+"_walk",count<320,new JObject{["position"]=V(s.player.transform.position),["frames"]=count});if(count>=320)throw new Exception("Blocked path "+name);}
 TuongTacHoCho Item(TuongTacHoCho.Kind kind)=>FindObjectsByType<TuongTacHoCho>().First(i=>i.kind==kind&&i.GetComponent<Collider>());
 void Save(string name){s.Save();File.Copy(s.SavePathOverride,output+"/"+name+".json",true);}
 IEnumerator Load(string name){File.Copy(output+"/"+name+".json",s.SavePathOverride,true);yield return KeyPress(Key.F6);Stop();yield return Wait(.3f);}
 void Still(string name){Render(output+"/"+name+".png",false);}
 void Render(string path,bool jpg){var rt=RenderTexture.GetTemporary(1280,720,24);var old=RenderTexture.active;var t=new Texture2D(1280,720,TextureFormat.RGB24,false);try{RenderPipeline.SubmitRenderRequest(eye,new UniversalRenderPipeline.SingleCameraRequest{destination=rt});RenderTexture.active=rt;t.ReadPixels(new Rect(0,0,1280,720),0,0);t.Apply();File.WriteAllBytes(path,jpg?t.EncodeToJPG(86):t.EncodeToPNG());}finally{RenderTexture.active=old;RenderTexture.ReleaseTemporary(rt);Destroy(t);}}
 IEnumerator Record(){while(!finished){yield return new WaitForEndOfFrame();if(!finished&&filming&&!s.Loading){Render(output+"/frames/"+(frame).ToString("D6")+".jpg",true);timeline.WriteLine(new JObject{["frame"]=frame++,["phase"]=phase,["realtime"]=Time.realtimeSinceStartup,["region"]=s.data.region,["position"]=V(s.player.transform.position),["camera"]=V(eye.transform.eulerAngles),["memory"]=JObject.Parse(JsonUtility.ToJson(n.memory)),["dialogue"]=s.Dialogue,["awareness"]=s.data.awareness,["actionPoints"]=s.data.actionPoints,["interactions"]=s.data.interactions,["witnessed"]=s.data.witnessed[0],["gestureSeen"]=n.SeenGesture,["actionClock"]=n.LastInput,["mimicClock"]=n.LastMimic,["clock"]=n.Clock}.ToString(Newtonsoft.Json.Formatting.None));}}}
 void Audit(string label){
  var all=s.gameObject.scene.GetRootGameObjects().SelectMany(g=>g.GetComponentsInChildren<Transform>(true)).ToArray();var cams=all.Select(t=>t.GetComponent<Camera>()).Where(c=>c).ToArray();
  var j=new JObject{["players"]=all.Count(t=>t.GetComponent<DieuKhienDiChuyen>()),["listeners"]=all.Count(t=>t.GetComponent<AudioListener>()),["stories"]=all.Count(t=>t.GetComponent<DieuPhoiHanhTrinh>()),["books"]=all.Count(t=>t.name=="So_cua_Minh"),["panels"]=all.Count(t=>t.name=="R37_NotebookContext"),["eventSystems"]=all.Count(t=>t.GetComponent<UnityEngine.EventSystems.EventSystem>()),["inputModules"]=all.Sum(t=>t.GetComponents<UnityEngine.EventSystems.BaseInputModule>().Length),["cameras"]=new JArray(cams.Select(c=>new JObject{["name"]=c.name,["enabled"]=c.enabled,["mask"]=c.cullingMask})),["mapEnabled"]=s.player.GameplayActionsEnabled,["ready"]=n.Ready,["memory"]=JObject.Parse(JsonUtility.ToJson(n.memory))};File.WriteAllText(output+"/audit-"+label+".json",j.ToString());
  Check(label+"_single_objects",(int)j["players"]==1&&(int)j["listeners"]==1&&(int)j["stories"]==1&&(int)j["books"]==1&&(int)j["panels"]==1&&(int)j["eventSystems"]==1&&(int)j["inputModules"]==1);
  Check(label+"_intentional_cameras",cams.Count(c=>c.enabled&&c.gameObject.activeInHierarchy)==1&&cams.All(c=>c==eye||!c.enabled&&(c.name=="Lake_Reflection_Camera"||c.name=="R32_inspection_camera")));
 }
 IEnumerator Run(){
  yield return Until(()=>!s.Loading,40,"loading");Check("loading_gate_real",s.GetComponent<ManHinhTaiV44>().Ready);Audit("startup");yield return KeyPress(Key.Enter);Stop();yield return null;Check("new_game_hub_retained",s.data.region==-1&&s.data.nextRegion==0);
  yield return Walk(new Vector3(-35,.94f,-34.6f),"hub_bed");var bed=Item(TuongTacHoCho.Kind.Bed);yield return Press(bed,bed.transform.position,"sleep");yield return Until(()=>s.data.region==0&&!s.Loading,15,"dream_transition");Stop();
  filming=true;phase="ARRIVE_lake";var bao=s.GetComponent<TuTheBaoBenCauV48>();Look(bao.actorPosition+Vector3.up*1.15f);yield return Wait(2);Save("lake-entry");Check("lake_entry_near_Bao_no_award",Vector3.Distance(s.player.transform.position,bao.actorPosition)<4&&!s.data.traces[0]&&!s.data.witnessed[0]&&n.memory.viewed==0);
  yield return Press(Item(TuongTacHoCho.Kind.Bao),s.ghosts[0].GetComponent<HienGuongMatTheoKhu>().ghostHead.position,"MEET_Bao");yield return Wait(3,"BAO_original_dialogue");Check("Bao_leaning_no_fishing",bao.leaningOnly&&!bao.rod.gameObject.activeInHierarchy&&!bao.line.gameObject.activeInHierarchy);File.WriteAllText(output+"/Bao-contact.json",BaoContactProbeV48.Sample(bao).ToString());
  if(branch=="skip"){Check("skip_untouched",!n.memory.touched);yield return ExitLake();Check("skip_continues_without_notebook_or_points",s.data.nextRegion==1&&n.memory.viewed==0&&s.data.awareness==(s.data.traces[0]?1:0));yield break;}
  var approach=n.socket.position-bao.waterDirection*.82f;yield return Walk(approach,"FIND_notebook",n.socket.position);yield return Wait(1.2f,"SEE_notebook");
  yield return Press(n.book.GetComponent<DichTuongTacSoTayR37>(),n.book.position+Vector3.right*.04f,"PICK_notebook");Check("pickup_not_automatic_observation",n.Held&&!n.memory.handObserved);Save("closed-held");
  Look(branch=="edge"?eye.transform.position-bao.waterDirection*10:n.gesture.position+Vector3.up*.05f);yield return null;UpdateRay();yield return KeyPress(Key.E);yield return Wait(2.4f,"READ_plan_crossed");Check("opened_plan_no_points",n.Held&&n.memory.open&&n.memory.page==0&&(n.memory.viewed&1)!=0&&s.data.awareness==0&&s.data.interactions==0);Save("open-held");Still("held-open-plan");
  yield return KeyPress(Key.E);yield return Wait(2,"READ_blank_page");Check("both_pages_seen",n.memory.viewed==3&&n.memory.page==1);Still("held-open-blank");
  if(branch=="edge"){
   Check("missed_hand_does_not_observe",!n.memory.handObserved&&!s.data.witnessed[0]);yield return KeyPress(Key.Escape);float clock=n.Clock;var lid=n.leaf.localRotation;yield return Wait(1.1f,"PAUSE_open_book");Check("pause_freezes_book_and_input",s.Paused&&!n.Ready&&!s.player.GameplayActionsEnabled&&n.Clock==clock&&n.leaf.localRotation==lid);yield return KeyPress(Key.E);Check("paused_E_no_cycle",n.memory.open&&n.memory.page==1);yield return KeyPress(Key.F5);yield return KeyPress(Key.F6);yield return Wait(.3f);Check("paused_save_load_resumes_original_flow",!s.Paused&&n.Held&&n.memory.open&&n.memory.page==1);Audit("held_load");
   int sound=n.SoundRequests;yield return Load("open-held");Check("open_held_load_no_sound_or_observe_replay",n.Held&&n.memory.open&&n.SoundRequests==sound&&!n.memory.handObserved);yield return Load("closed-held");Check("closed_held_load",n.Held&&!n.memory.open);yield return KeyPress(Key.R);Check("R_places_closed",!n.Held&&!n.memory.open);yield return PickAgain();Look(eye.transform.position-bao.waterDirection*10);yield return null;UpdateRay();yield return KeyPress(Key.E);yield return KeyPress(Key.R);Check("R_places_open",!n.Held&&n.memory.open);Save("placed-open");yield return Load("placed-open");Check("placed_open_load_one_instance",!n.Held&&n.memory.open&&Vector3.Distance(n.book.position,n.socket.position)<.001f);Audit("placed_load");
   yield return PickAgain();yield return Walk(s.lakeSpawn.position,"ABANDON_away");yield return Walk(s.lakeSpawn.position-bao.waterDirection*6,"ABANDON_far");Check("abandon_returns_safe",!n.Held&&n.memory.abandoned);yield return Load("lake-entry");Check("old_entry_retains_position",s.data.region==0&&Vector3.Distance(s.player.transform.position,s.lakeSpawn.position)<.2f);
   yield return Walk(s.traces[0].position-bao.waterDirection*1.3f,"FIND_calendar",s.traces[0].position);Look(s.traces[0].position);yield return Wait(10,"CALENDAR_view");Check("calendar_awareness_once",s.data.traces[0]&&s.data.awareness==1);Save("calendar-seen");yield return Load("calendar-seen");yield return Wait(1);Check("calendar_no_repeat_on_load",s.data.awareness==1);yield return PickAgain();
   yield return ExitLake();Check("dream_book_absent_hub",!n.Held&&!n.book.gameObject.activeInHierarchy&&s.data.awareness==1);filming=false;
   // Compatibility fixtures are after recorded walkthrough, no progress injected into video.
   yield return Compatibility();yield break;
  }
  Check("GD1_hand_seen_input_linked",n.memory.handObserved&&s.data.witnessed[0]&&n.SeenGesture>=.25f,new JObject{["delaySeconds"]=n.LastMimic-n.LastInput,["visibleSeconds"]=n.SeenGesture,["captureDeltaTime"]=Time.captureDeltaTime});
   Look(n.socket.position+bao.waterDirection*8+Vector3.down*1.8f);yield return Wait(2,"VIEW_water_reflection");Still("water-context");SaveReflection("reflection-water-view");
  yield return KeyPress(Key.E);yield return Wait(.5f,"CLOSE_notebook");Check("closed_again",!n.memory.open);if(branch!="closed"){yield return KeyPress(Key.E);yield return Wait(.7f,"REOPEN_try_again");}
  Look(n.socket.position);yield return null;UpdateRay();yield return Wait(.6f,"PREVIEW_safe_placement");Check("placement_preview_visible",n.preview.activeSelf);yield return Press(n.socket.parent.GetComponentsInChildren<DichTuongTacSoTayR37>(true).First(t=>t.transform!=n.book),n.socket.position-Vector3.up*.012f,"PLACE_on_stand");yield return Wait(3,"RESULT_Bao_response");Check("chosen_state_and_response",!n.Held&&n.memory.open==(branch!="closed")&&n.LastResponse.Contains(branch=="closed"?"Khép lại":"Trang kia"));
  // Same camera for source contact screenshots of both real interactable states.
  Still(branch=="closed"?"placed-closed":"placed-open");Check("support_contact_safe",Mathf.Abs(n.book.position.y-(1.1400611f+.741f))<.002f);
  filming=false;Save("placed");int played=n.SoundRequests;yield return Load("placed");Check("placed_load_no_replay",!n.Held&&n.memory.open==(branch!="closed")&&n.LastResponse==""&&n.SoundRequests==played);Audit("placed");
  var refl=s.GetComponent<PhanChieuMatHo>();Check("reflection_rendered",refl.HasRendered);SaveReflection("reflection-placed");
  Check("notebook_no_ending_points",s.data.awareness==0&&s.data.actionPoints==0&&s.data.interactions==0);yield return ExitLake();Check("hub_only_memory",!n.book.gameObject.activeInHierarchy&&!n.Held&&n.memory.viewed==3&&s.GetComponent<HuongDanHanhTrinhV48>().PhoneText.Contains("CUỐN SỔ"));
  yield return Load("placed");Audit("return_lake");Check("return_lake_state_restored",s.data.region==0&&!n.Held&&n.memory.viewed==3);yield return ReloadScene();
 }
 IEnumerator PickAgain(){var b=s.GetComponent<TuTheBaoBenCauV48>();yield return Walk(n.socket.position-b.waterDirection*.82f,"return_to_stand",n.socket.position);yield return Press(n.book.GetComponent<DichTuongTacSoTayR37>(),n.book.position+Vector3.right*.04f,"take_again");}
 void SaveReflection(string name){var t=typeof(PhanChieuMatHo).GetField("texture",System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Instance).GetValue(s.GetComponent<PhanChieuMatHo>()) as RenderTexture;if(!t)return;var old=RenderTexture.active;RenderTexture.active=t;var im=new Texture2D(t.width,t.height,TextureFormat.RGB24,false);im.ReadPixels(new Rect(0,0,t.width,t.height),0,0);im.Apply();File.WriteAllBytes(output+"/"+name+".png",im.EncodeToPNG());RenderTexture.active=old;Destroy(im);}
 IEnumerator ExitLake(){
  // Follow the inner footway around the physical stand before the long bridge segment.
  var bao=s.GetComponent<TuTheBaoBenCauV48>();
  if(Vector3.Distance(s.player.transform.position,n.socket.position)<5)
   yield return Walk(n.socket.position-bao.waterDirection*1.05f,"LEAVE_inner_footway");
  yield return Walk(new Vector3(682.23f,1.14f,492.19f),"LEAVE_bridge_footway");yield return Walk(new Vector3(688.34f,1.14f,487.02f),"LEAVE_bridge_end");yield return Walk(new Vector3(703.95f,.95f,484.53f),"LEAVE_land_link");var link=JObject.Parse(Resources.Load<TextAsset>("VerificationV36/bridge").text)["connections"][0].Select(P).Reverse().ToArray();foreach(var p in link.Where((p,i)=>i%4==0||i==link.Length-1))yield return Walk(p,"LEAVE_ramp");yield return Walk(new Vector3(718.7f,0,481.5f),"LEAVE_exit");var wake=Item(TuongTacHoCho.Kind.Wake);yield return Press(wake,wake.transform.position,"LEAVE_wake");yield return Wait(.7f,"CONTINUE_hub");Check("main_flow_continues",s.data.region==-1&&s.data.nextRegion==1);}
 IEnumerator Compatibility(){
  foreach(var spec in new[]{("Docs/Reviews/journey_v48/r36/native-main/confirmed.json",1),("Docs/Reviews/journey_v48/r35/school-chair/chair-after.json",2)}){if(!File.Exists(spec.Item1)){Check("legacy_fixture_not_found_"+spec.Item2,false,new JValue(spec.Item1));continue;}File.Copy(spec.Item1,s.SavePathOverride,true);yield return KeyPress(Key.F6);Check("legacy_region_retained_"+spec.Item2,s.data.region==spec.Item2&&!n.Active&&!n.Held);Audit("legacy_"+spec.Item2);
   int aw=s.data.awareness,ap=s.data.actionPoints;
   if(spec.Item2==1){
    var m=s.GetComponent<QuanLyGoiHangChoR35>();var parcel=m.parcels[0];
    yield return Walk(new Vector3(parcel.transform.position.x,5.5f,271.25f),"legacy_market_pick",parcel.transform.position);
    yield return Press(parcel,parcel.transform.position+Vector3.up*.05f,"legacy_market_E");
    Check("legacy_market_holds_without_lake_input",m.Held==0&&!n.Held);
    bool before=m.memory.opened[0];Look(eye.transform.position+Vector3.up*3);yield return null;UpdateRay();yield return KeyPress(Key.E);yield return Wait(.6f);
    Check("legacy_market_E_opens",m.memory.opened[0]!=before);yield return KeyPress(Key.R);
    Check("legacy_market_R_returns",m.Held<0&&!n.Held);
   }else{
    var papers=s.GetComponent<TuongTacGiayLop203R32>();
    yield return Walk(new Vector3(-85.3f,3.6f,-36.5f),"legacy_school_aisle");
    yield return Walk(new Vector3(-87.8f,3.6f,-36.9f),"legacy_school_desk",papers.papers[0].position);
    yield return Press(papers.papers[0].GetComponent<DichTuongTacGiayR32>(),papers.papers[0].position,"legacy_school_view_E");
    Check("legacy_school_inspection",papers.Inspecting&&!n.Held);yield return KeyPress(Key.Tab);yield return Wait(.3f);
    Check("legacy_school_close_restores_input",!papers.Inspecting&&s.player.GameplayActionsEnabled);
   }
   Check("legacy_no_new_points_"+spec.Item2,s.data.awareness==aw&&s.data.actionPoints==ap);
  }
 }
 IEnumerator ReloadScene(){string path=s.gameObject.scene.path,save=s.SavePathOverride;DontDestroyOnLoad(gameObject);s=null;n=null;mouse=null;eye=null;pi=null;var op=UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(path);while(!op.isDone)yield return null;Bind();s.SavePathOverride=save;yield return Until(()=>!s.Loading,40,"reload_loading");s.GetComponent<MenuVaoGameV47>().Continue();yield return null;Stop();Audit("scene_reload");Check("reload_keeps_notebook_choice",s.data.region==0&&!n.Held&&n.memory.viewed==3);}
}

#endif
