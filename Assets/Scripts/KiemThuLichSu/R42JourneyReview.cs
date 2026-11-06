#if UNITY_EDITOR || CON_ROI_BUILD_REVIEW
using System;using System.IO;using System.Linq;using System.Collections;using System.Collections.Generic;using Newtonsoft.Json.Linq;using UnityEngine;using UnityEngine.InputSystem;using UnityEngine.InputSystem.LowLevel;using UnityEngine.Rendering;using UnityEngine.Rendering.Universal;

// Opt-in diagnostics only. Never serialized into product scenes.
[DefaultExecutionOrder(30000)] public sealed class R42JourneyReview:MonoBehaviour {
 public string output;DieuPhoiHanhTrinh s;ChuanBiHanhLyR41 a;Camera eye;DieuKhienGocNhin mouse;TuongTacNguoiChoi pi;
 readonly Stack<IEnumerator> stack=new Stack<IEnumerator>();readonly JArray checks=new JArray(),errors=new JArray();StreamWriter timeline;string phase="startup",branch;bool finished,filming;int frame;double deadline;
 static string Option(string key,string fallback){var args=Environment.GetCommandLineArgs();int i=Array.IndexOf(args,key);return i>=0&&i+1<args.Length?args[i+1]:fallback;}
 [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)] static void Boot(){if(Application.isEditor)return;string p=Option("--r42-journey","");if(p.Length>0)new GameObject("R42_OptInReview").AddComponent<R42JourneyReview>().output=p;}
 bool NoCapture=>Array.IndexOf(Environment.GetCommandLineArgs(),"--r42-no-capture")>=0;
 void Bind(){s=FindAnyObjectByType<DieuPhoiHanhTrinh>();a=s.GetComponent<ChuanBiHanhLyR41>();eye=s.player.GetComponentInChildren<Camera>();mouse=s.player.GetComponentInChildren<DieuKhienGocNhin>();pi=s.player.GetComponent<TuongTacNguoiChoi>();}
 void Start(){Bind();branch=Option("--r42-branch","main");Directory.CreateDirectory(output);Directory.CreateDirectory(output+"/frames");s.SavePathOverride=output+"/isolated-save.json";Application.runInBackground=true;InputSystem.settings=Instantiate(InputSystem.settings);InputSystem.settings.updateMode=InputSettings.UpdateMode.ProcessEventsInDynamicUpdate;InputSystem.settings.backgroundBehavior=InputSettings.BackgroundBehavior.IgnoreFocus;InputSystem.settings.editorInputBehaviorInPlayMode=InputSettings.EditorInputBehaviorInPlayMode.AllDeviceInputAlwaysGoesToGameView;InputSystem.AddDevice<Keyboard>("R41ReviewKeyboard").MakeCurrent();InputSystem.AddDevice<Mouse>("R41ReviewMouse").MakeCurrent();QualitySettings.vSyncCount=0;Application.targetFrameRate=NoCapture?60:12;Time.captureDeltaTime=0;timeline=new StreamWriter(output+"/timeline.jsonl"){AutoFlush=true};Application.logMessageReceived+=Log;deadline=Time.realtimeSinceStartupAsDouble+2400;if(!NoCapture)eye.gameObject.AddComponent<R42AudioCapture>().output=output+"/game-audio.wav";stack.Push(Run());StartCoroutine(Record());}
 void OnDestroy(){Application.logMessageReceived-=Log;timeline?.Dispose();}
 void Log(string m,string trace,LogType t){if(t==LogType.Error||t==LogType.Exception)errors.Add(new JObject{["message"]=m,["trace"]=trace});}
 void LateUpdate(){if(finished)return;try{if(Time.realtimeSinceStartupAsDouble>deadline)throw new Exception("R41 timeout");if(mouse)mouse.enabled=false;while(stack.Count>0){var e=stack.Peek();if(!e.MoveNext()){stack.Pop();continue;}if(e.Current is IEnumerator nested){stack.Push(nested);continue;}break;}if(stack.Count==0)Finish(null);}catch(Exception e){Finish(e.ToString());}}
 void Finish(string failure){finished=true;Stop();timeline?.Dispose();bool pass=failure==null&&errors.Count==0&&checks.All(c=>(bool)c["pass"]);File.WriteAllText(output+"/result.json",new JObject{["complete"]=pass,["captureEnabled"]=!NoCapture,["failure"]=failure,["scene"]=s.gameObject.scene.path,["editor"]=Application.isEditor,["branch"]=branch,["checks"]=checks,["errors"]=errors,["frames"]=frame,["scope"]="Branch full starts at New Game, other branches are explicitly isolated fixtures; original CharacterController, real E ray, synthetic input/aim, realtime captureDeltaTime=0, URP offscreen; audio listener PCM captured separately if callbacks available. No state injection in recorded slope-to-ending sequence. Isolated saves only. Physical Windows NOT_VERIFIED."}.ToString());
#if UNITY_EDITOR
 if(Application.isEditor){UnityEditor.EditorApplication.Exit(pass?0:1);return;}
#endif
 Application.Quit(pass?0:1);}
 void Check(string n,bool value,JToken detail=null){checks.Add(new JObject{["name"]=n,["pass"]=value,["detail"]=detail});File.WriteAllText(output+"/progress.json",checks.ToString());if(!value)Debug.LogWarning("R41_CHECK_FAILED "+n);}
 static JArray V(Vector3 v)=>new JArray(v.x,v.y,v.z);
 void Stop(){if(s)s.player.MovementFilter=r=>Vector2.zero;}
 void Look(Vector3 at){mouse.enabled=false;var d=at-eye.transform.position;var flat=d;flat.y=0;if(flat.sqrMagnitude>.0001f)s.player.transform.rotation=Quaternion.LookRotation(flat);eye.transform.rotation=Quaternion.LookRotation(d);}
 IEnumerator Wait(float seconds,string p=null){if(p!=null)phase=p;double end=Time.realtimeSinceStartupAsDouble+seconds;while(Time.realtimeSinceStartupAsDouble<end)yield return null;}
 IEnumerator Until(Func<bool> f,float seconds,string p){phase=p;double end=Time.realtimeSinceStartupAsDouble+seconds;while(!f()){if(Time.realtimeSinceStartupAsDouble>end)throw new Exception(p+" timeout");yield return null;}}
 IEnumerator KeyPress(Key k){InputSystem.QueueStateEvent(Keyboard.current,new KeyboardState(k));yield return null;yield return null;InputSystem.QueueStateEvent(Keyboard.current,new KeyboardState());yield return null;yield return null;}
 IEnumerator Chord(Key a,Key b){InputSystem.QueueStateEvent(Keyboard.current,new KeyboardState(a,b));yield return null;yield return null;InputSystem.QueueStateEvent(Keyboard.current,new KeyboardState());yield return null;yield return null;}
 void Ray(){typeof(TuongTacNguoiChoi).GetMethod("Update",System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Instance).Invoke(pi,null);}
 IEnumerator Press(IPlayerInteractable target,Vector3 pos,string p){phase=p;Stop();Look(pos);yield return null;Ray();if(pi.Target!=target){var box=(target as Component).GetComponent<Collider>();foreach(var at in new[]{box.bounds.center,box.bounds.center+Vector3.up*box.bounds.extents.y*.9f,box.bounds.center+Vector3.left*box.bounds.extents.x*.9f}){Look(at);Ray();if(pi.Target==target)break;}}
  if(pi.Target!=target){File.WriteAllText(output+"/blocked-"+p+".json",new JArray(Physics.RaycastAll(eye.transform.position,eye.transform.forward,4,~0,QueryTriggerInteraction.Ignore).OrderBy(h=>h.distance).Select(h=>new JObject{["path"]=h.collider.name,["distance"]=h.distance})).ToString());throw new Exception("Wrong ray "+p+" actual="+pi.Target);}
  int count=pi.CompletedInteractions;yield return KeyPress(Key.E);Check(p+"_actual_ray",pi.CompletedInteractions==count+1);
 }
 IEnumerator Walk(Vector3 at,string p,Vector3? gaze=null){phase=p;int frames=0;double walkAt=Time.realtimeSinceStartupAsDouble;while(new Vector2(at.x-s.player.transform.position.x,at.z-s.player.transform.position.z).magnitude>.16f&&Time.realtimeSinceStartupAsDouble-walkAt<30&&frames++<12000){var ahead=at+Vector3.up*1.6f;if(!gaze.HasValue)ahead.y=eye.transform.position.y;Look(gaze??ahead);var d=at-s.player.transform.position;d.y=0;float amt=Mathf.Min(.8f,d.magnitude/(s.player.speed*Mathf.Max(Time.deltaTime,.0833f)));var local=s.player.transform.InverseTransformDirection(d.normalized)*amt;s.player.MovementFilter=r=>new Vector2(local.x,local.z);yield return null;}Stop();bool reached=new Vector2(at.x-s.player.transform.position.x,at.z-s.player.transform.position.z).magnitude<=.16f;Check(p+"_walk",reached,new JObject{["position"]=V(s.player.transform.position),["frames"]=frames});if(!reached)throw new Exception("Blocked route "+p);}
 void Save(string n){s.Save();File.Copy(s.SavePathOverride,output+"/"+n+".json",true);}
 IEnumerator Load(string n){File.Copy(output+"/"+n+".json",s.SavePathOverride,true);yield return KeyPress(Key.F6);Stop();yield return Wait(.25f);}
 void Audit(string label){var all=s.gameObject.scene.GetRootGameObjects().SelectMany(g=>g.GetComponentsInChildren<Transform>(true)).ToArray();Check(label+"_single_objects",all.Count(t=>t.GetComponent<DieuKhienDiChuyen>())==1&&all.Count(t=>t.GetComponent<AudioListener>())==1&&all.Count(t=>t.GetComponent<ChuanBiHanhLyR41>())==1&&all.Count(t=>t.name=="The_hanh_ly_cu")==1&&all.Count(t=>t.name=="To_chuan_bi_gap")==1&&all.Count(t=>t.name=="R41_PreparationContext")==1);Check(label+"_no_U",!all.Any(t=>t.name.Contains("Protected_Reflection_Overlook")));}
 void Still(string n){UiAudit(n);if(NoCapture)return;int width=Screen.width,height=Screen.height;var rt=RenderTexture.GetTemporary(width,height,24,RenderTextureFormat.ARGB32);var old=RenderTexture.active;var t=new Texture2D(width,height,TextureFormat.RGB24,false);try{RenderPipeline.SubmitRenderRequest(eye,new UniversalRenderPipeline.SingleCameraRequest{destination=rt});RenderTexture.active=rt;t.ReadPixels(new Rect(0,0,width,height),0,0);t.Apply();File.WriteAllBytes(output+"/"+n+".png",t.EncodeToPNG());}finally{RenderTexture.active=old;RenderTexture.ReleaseTemporary(rt);Destroy(t);}}
 IEnumerator Record(){while(!finished){yield return new WaitForEndOfFrame();if(filming&&!finished){if(!NoCapture){var rt=RenderTexture.GetTemporary(1280,720,24,RenderTextureFormat.ARGB32);var old=RenderTexture.active;var t=new Texture2D(1280,720,TextureFormat.RGB24,false);try{RenderPipeline.SubmitRenderRequest(eye,new UniversalRenderPipeline.SingleCameraRequest{destination=rt});RenderTexture.active=rt;t.ReadPixels(new Rect(0,0,1280,720),0,0);t.Apply();File.WriteAllBytes(output+"/frames/"+frame.ToString("D6")+".jpg",t.EncodeToJPG(86));}finally{RenderTexture.active=old;RenderTexture.ReleaseTemporary(rt);Destroy(t);}
}
   timeline.WriteLine(new JObject{["frame"]=frame++,["phase"]=phase,["realtime"]=Time.realtimeSinceStartup,["uiMode"]=s.GetComponent<DieuPhoiGiaoDienR42>().Mode,["audioLayers"]=s.presentation.TriggeredLayers,["phone"]=s.PhoneOpen,["paused"]=s.Paused,["reading"]=s.polish.IsReading,["inspection"]=s.SupplementInspection,["region"]=s.data.region,["position"]=V(s.player.transform.position),["camera"]=V(eye.transform.eulerAngles),["eyePosition"]=V(eye.transform.position),["puppetHead"]=V(s.slopeChapter.puppet.dauTransform.position),["headViewport"]=V(eye.WorldToViewportPoint(s.slopeChapter.puppet.dauTransform.position)),["memory"]=JObject.Parse(JsonUtility.ToJson(a.memory)),["trace"]=s.data.slopeTrace,["awareness"]=s.data.awareness,["action"]=s.data.actionPoints,["interactions"]=s.data.interactions,["finalStarts"]=s.data.finalStarts,["elapsed"]=s.data.finalElapsed,["witnessed"]=s.data.slopeWitnessed,["puppet"]=V(s.slopeChapter.puppet.transform.position),["dialogue"]=s.Dialogue,["ending"]=s.data.ending}.ToString(Newtonsoft.Json.Formatting.None));}}
 }
 IEnumerator Run(){yield return Until(()=>!s.Loading,60,"bootstrap");yield return MenuPair();yield return KeyPress(Key.Enter);Stop();Audit("startup");
  if(branch=="startup"){Check("startup_correct_scene",s.gameObject.scene.path.Contains("r42"));yield break;}
  if(branch=="matrix"){yield return Matrix();yield break;}
  if(branch=="full"){yield return Full();yield break;}
  if(branch=="regression"){yield return Regression();yield break;}
  // A compatible old version-1 fixture at the existing pre-slope hub, before filming.
  var fixture=new DieuPhoiHanhTrinh.State{nextRegion=3,region=-1,position=s.hubSpawn.position,rotation=s.hubSpawn.rotation,mapLayoutRevision=21,posted=new bool[4],called=new bool[4],dismissed=new bool[4],vyMessage="Mấy hôm rồi không thấy cậu kể. Cậu vẫn ổn chứ? Không muốn đăng cũng được."};
  string old=JsonUtility.ToJson(fixture,true);File.WriteAllText(s.SavePathOverride,old);yield return KeyPress(Key.F6);Check("old_save_retains_chapter_no_new_memory",s.data.nextRegion==3&&s.data.region==-1&&a.memory.viewed==0&&File.ReadAllText(s.SavePathOverride)==old);
  yield return Walk(new Vector3(-35,.94f,-34.6f),"hub_bed");var bed=FindObjectsByType<TuongTacHoCho>().First(i=>i.kind==TuongTacHoCho.Kind.Bed&&i.GetComponent<Collider>());yield return Press(bed,bed.transform.position,"sleep");yield return Until(()=>s.data.region==3&&!s.Loading,20,"dream_transition");Stop();Check("GD4_hidden_on_entry",!s.slopeChapter.puppet.gameObject.activeInHierarchy&&!s.data.finalStarted);
  filming=true;phase="ARRIVE_Tung_neighborhood";Look(new Vector3(-180,5,-36));yield return Wait(1);Save("slope-entry");
  for(float z=2;z<=20;z+=2){float x=z<12?0:(z-12)*.1f;yield return Walk(new Vector3(-180+x,0,-60+z),"approach_"+z,branch=="skip"?new Vector3(-190,4,-40):(Vector3?)null);}
  if(branch=="main"){
   for(float z=22;z<=42;z+=2){float x=z<25?(z-12)*.1f:1.3f;yield return Walk(new Vector3(-180+x,0,-60+z),"ARRIVE_Tung_"+z);}
   var greeting=s.slopeChapter.environment.GetComponentsInChildren<TuongTacDocV39>().First(t=>!t.finalDoor&&t.memoryIndex<0);yield return Press(greeting,greeting.transform.position,"GREET_Tung_original");yield return Wait(3,"Tung_original_invitation");
   for(float z=40;z>=22;z-=2){float x=z<25?(z-12)*.1f:1.3f;yield return Walk(new Vector3(-180+x,0,-60+z),"RETURN_to_case_"+z);}
  }
  if(branch=="skip"){yield return ContinueSlope();Check("skip_no_items_no_trace",a.memory.viewed==0&&!s.data.slopeTrace);yield break;}
  yield return Walk(new Vector3(-178.8f,0,-38.9f),"FIND_case_and_tag",a.tagRest.position);Look(a.tagRest.position);yield return Wait(1.5f,"SEE_original_dusty_case");Still("case-before");int before=s.data.awareness;
  if(branch!="paper"){
  yield return Press(a.tag.GetComponent<DichTuongTacHanhLyR41>(),a.tag.position,"PICK_tag");Check("tag_memory_only",a.memory.held==0&&a.memory.viewed==1&&s.data.awareness==before);Look(eye.transform.position+Vector3.forward*10);yield return Wait(1,"READ_tag_front");Still("held-tag");yield return KeyPress(Key.E);yield return Wait(.7f,"READ_tag_reverse");Check("tag_real_back",a.memory.tagBack);Still("tag-back");
  InputSystem.QueueDeltaStateEvent(Mouse.current.scroll,new Vector2(0,120));yield return null;yield return null;Check("tag_rotates",a.memory.tagTurn!=0);
  Save("tag-held");
  if(branch=="edge"){yield return Load("tag-held");Check("tag_held_face_turn_load",a.memory.held==0&&a.memory.tagBack&&a.memory.tagTurn!=0);}
  if(branch=="one"){yield return KeyPress(Key.R);Check("only_tag_recorded",a.memory.viewed==1);yield return ContinueSlope();yield break;}
  var spot=a.root.GetComponentsInChildren<DichTuongTacHanhLyR41>(true).First(t=>t.arrange);Look(spot.transform.position);yield return Wait(.5f,"ARRANGE_preview");Still("preview-tag");yield return Press(spot,spot.transform.position,"ARRANGE_tag");Check("tag_slot_actual",a.memory.held==-1&&a.memory.tagSlot==1);Still("tag-arranged");
  }
  yield return Press(a.paper.GetComponent<DichTuongTacHanhLyR41>(),a.paper.position+Vector3.right*.07f,"PICK_checklist");Look(eye.transform.position+Vector3.forward*10);yield return null;yield return KeyPress(Key.E);yield return Wait(1.4f,"READ_revised_checklist");Check("paper_open_seen",a.memory.paperOpen&&a.memory.viewed==(branch=="paper"?2:3));Still("held-open-paper");Save("open-held");
  if(branch=="edge"){
   yield return KeyPress(Key.Escape);var pos=s.player.transform.position;bool open=a.memory.paperOpen;yield return KeyPress(Key.E);yield return Wait(.4f);Check("pause_no_item_or_movement",s.Paused&&a.memory.paperOpen==open&&Vector3.Distance(pos,s.player.transform.position)<.001f);yield return KeyPress(Key.F5);yield return KeyPress(Key.F6);if(s.Paused)yield return KeyPress(Key.Escape);yield return Wait(.3f);Check("pause_load_restores_one_held",!s.Paused&&a.memory.held==1&&a.memory.paperOpen);Audit("held_load");
   Look(eye.transform.position+Vector3.forward*10);yield return null;yield return KeyPress(Key.E);yield return Wait(.5f);Save("closed-held");yield return Load("closed-held");Check("closed_held_load",a.Held&&!a.memory.paperOpen);Look(eye.transform.position+Vector3.forward*10);yield return null;yield return KeyPress(Key.E);yield return Wait(.5f);yield return Load("open-held");Check("open_held_load",a.Held&&a.memory.paperOpen);}
  // R returns to its authored safe surface, retaining open state; no free dropping.
  yield return KeyPress(Key.R);yield return Wait(.7f,"COMPARE_side_by_side_on_case");Look(a.paperRest.position);yield return Wait(1);Still("placed-open");Check("placed_open_contact",!a.Held&&a.memory.paperOpen&&Vector3.Distance(a.paper.position,a.paperRest.position)<.0001f);Save("placed-open");
  if(branch=="edge"){yield return Load("placed-open");Check("placed_load_choice",a.memory.tagSlot==1&&a.memory.paperOpen&&!a.Held);Audit("placed_load");int awareness=s.data.awareness;yield return Wait(2);Check("original_case_no_repeat",s.data.awareness==awareness&&s.data.slopeTrace);
   yield return Press(a.paper.GetComponent<DichTuongTacHanhLyR41>(),a.paper.position+Vector3.forward*.07f,"RETRY_paper");Look(eye.transform.position+Vector3.forward*10);yield return KeyPress(Key.E);yield return Wait(.5f);yield return KeyPress(Key.R);Check("placed_closed",!a.Held&&!a.memory.paperOpen);Save("placed-closed");yield return Load("placed-closed");Check("placed_closed_load",!a.Held&&!a.memory.paperOpen);var reclaim=a.root.GetComponentsInChildren<DichTuongTacHanhLyR41>(true).First(t=>t.arrange);yield return Press(reclaim,reclaim.transform.position,"ABANDON_pick_tag");}
  Check("items_no_action_points",s.data.actionPoints==0&&s.data.interactions==0);yield return ContinueSlope();
 }

 static Vector3 P(JToken p)=>new Vector3((float)p[0],(float)p[1],(float)p[2]);
 static Vector3 W(float x,float y,float z)=>new Vector3(-95+x,y,-45+z);
 TuongTacHoCho Item(TuongTacHoCho.Kind k)=>FindObjectsByType<TuongTacHoCho>().First(i=>i.kind==k&&i.GetComponent<Collider>());
 IEnumerator Bed(int r){yield return Walk(new Vector3(-35,.94f,-34.6f),"hub_bed_"+r);var b=Item(TuongTacHoCho.Kind.Bed);yield return Press(b,b.transform.position,"bed_"+r);yield return Until(()=>s.data.region==r&&!s.Loading,20,"sleep_"+r);Stop();}
 IEnumerator Hub(string label){
  Check(label+"_really_awake",s.data.region==-1&&!s.DreamHub&&string.IsNullOrEmpty(s.data.ending));
  int actions=s.data.actionPoints,calls=s.data.interactions;yield return KeyPress(Key.P);yield return Wait(.7f,"AWAKE_"+label+"_phone");Still(label+"-phone");
  yield return KeyPress(Key.N);yield return Wait(.7f,"AWAKE_"+label+"_memories");Still(label+"-memories");
  yield return KeyPress(Key.P);yield return KeyPress(Key.F5);string snapshot=JsonUtility.ToJson(s.data);yield return KeyPress(Key.F6);yield return Wait(.3f);
  Check(label+"_readonly_no_actions",s.data.actionPoints==actions&&s.data.interactions==calls);
  var saved=JsonUtility.FromJson<DieuPhoiHanhTrinh.State>(snapshot);Check(label+"_memory_save_load",saved.lakeNotebookR37==s.data.lakeNotebookR37&&saved.marketPackagesR35==s.data.marketPackagesR35&&saved.class203R32==s.data.class203R32&&saved.slopePreparationsR41==s.data.slopePreparationsR41);
 }
 IEnumerator Full(){
  filming=true;Save("new-game");Check("new_game",s.data.region==-1&&s.data.nextRegion==0);yield return Bed(0);
  var n=s.GetComponent<TuongTacSoTayHoR37>();var bao=s.GetComponent<TuTheBaoBenCauV48>();
  yield return Press(Item(TuongTacHoCho.Kind.Bao),s.ghosts[0].GetComponent<HienGuongMatTheoKhu>().ghostHead.position,"Bao");
  yield return Walk(n.socket.position-bao.waterDirection*.82f,"notebook",n.socket.position);
  yield return Press(n.book.GetComponent<DichTuongTacSoTayR37>(),n.book.position+Vector3.right*.04f,"take_book");
  Look(eye.transform.position+Vector3.forward*10);yield return null;yield return KeyPress(Key.E);yield return Wait(.8f,"READ_lake_book");
  yield return KeyPress(Key.Escape);bool open=n.memory.open;yield return KeyPress(Key.E);Check("pause_keeps_book",s.Paused&&n.Held&&n.memory.open==open);yield return Chord(Key.Escape,Key.R);Check("resume_R_does_not_drop_book",n.Held);yield return KeyPress(Key.R);
  yield return Walk(s.traces[0].position-bao.waterDirection*1.3f,"calendar",s.traces[0].position);Look(s.traces[0].position);yield return Until(()=>s.data.traces[0],10,"calendar_observation");
  Check("calendar_original",s.data.traces[0]);
  yield return Walk(n.socket.position-bao.waterDirection*1.05f,"leave_stand");
  yield return Walk(new Vector3(682.23f,1.14f,492.19f),"bridge");yield return Walk(new Vector3(688.34f,1.14f,487.02f),"bridge_end");yield return Walk(new Vector3(703.95f,.95f,484.53f),"land_link");
  var link=JObject.Parse(Resources.Load<TextAsset>("VerificationV36/bridge").text)["connections"][0].Select(P).Reverse().ToArray();foreach(var v in link.Where((v,i)=>i%4==0||i==link.Length-1))yield return Walk(v,"ramp");yield return Walk(new Vector3(718.7f,0,481.5f),"lake_exit");var wake=Item(TuongTacHoCho.Kind.Wake);yield return Press(wake,wake.transform.position,"wake_lake");yield return Hub("after-lake");
  Check("no_real_book",!n.Held&&!n.book.gameObject.activeInHierarchy);yield return Bed(1);
  var route=JObject.Parse(Resources.Load<TextAsset>("VerificationV36/route").text)["route"].Select(P).ToArray();for(int i=166;i<244;i+=3)yield return Walk(route[i],"market_street_"+i);
  yield return Walk(new Vector3(779.5f,5.5f,274.25f),"cross_market");yield return Walk(new Vector3(779.5f,5.5f,271.1f),"front_aisle");yield return Walk(new Vector3(785.3f,5.5f,271.25f),"slip");
  var m=s.GetComponent<QuanLyGoiHangChoR35>();yield return Press(m.slip.GetComponent<DichTuongTacChoR35>(),m.slip.position,"view_slip");yield return Wait(1,"READ_slip");yield return KeyPress(Key.Tab);
  yield return Press(m.parcels[0],m.parcels[0].transform.position+Vector3.up*.05f,"take_package");Look(eye.transform.position+Vector3.forward*10);yield return null;yield return KeyPress(Key.E);yield return Wait(.8f,"READ_red_package");yield return KeyPress(Key.P);yield return KeyPress(Key.R);Check("phone_keeps_package",m.Held==0);yield return Chord(Key.P,Key.R);Check("close_phone_R_keeps_package",m.Held==0);yield return KeyPress(Key.R);
  yield return Walk(new Vector3(786.8f,5.5f,271.1f),"meet_Ha");yield return Press(Item(TuongTacHoCho.Kind.Ha),s.ghosts[1].GetComponent<HienGuongMatTheoKhu>().ghostHead.position,"Ha_unselected");
  yield return Walk(new Vector3(782.8f,5.5f,271.1f),"market_aisle");yield return Walk(new Vector3(781.1f,5.5f,273.2f),"market_exit");wake=Item(TuongTacHoCho.Kind.Wake);yield return Press(wake,wake.transform.position+Vector3.up*.7f,"wake_market");yield return Hub("after-market");
  Check("only_viewed_package",m.memory.viewed==3&&m.Held<0);yield return Bed(2);
  // Original school approach: the existing courtyard, left stairs and classroom doorway.
  var schoolRoute=JObject.Parse(Resources.Load<TextAsset>("VerificationV36/route").text);
  yield return SchoolApproach();
  var papers=s.GetComponent<TuongTacGiayLop203R32>();yield return Walk(W(4.6f,3.6f,7),"class203");yield return Walk(W(9.7f,3.6f,8.5f),"class_aisle");yield return Walk(W(7.2f,3.6f,8.1f),"Minh_desk",papers.papers[0].position);
  yield return Press(papers.papers[0].GetComponent<DichTuongTacGiayR32>(),papers.papers[0].position,"view_Minh");yield return Wait(1,"READ_Minh");Still("school-inspection");yield return KeyPress(Key.Escape);yield return KeyPress(Key.E);Check("pause_does_not_take_paper",papers.Inspecting&&papers.Selected<0);yield return Chord(Key.Escape,Key.E);Check("resume_E_does_not_take_paper",papers.Inspecting);yield return KeyPress(Key.E);yield return null;Check("E_takes_paper",!papers.Inspecting&&papers.Selected==0);yield return KeyPress(Key.R);
  yield return Walk(W(9.7f,3.6f,9),"Mai_aisle");yield return Walk(W(9.7f,3.6f,10.6f),"Mai_approach",s.schoolChapter.face.ghostHead.position);
  var mai=FindObjectsByType<TuongTacTruongV37>().First(t=>t.name=="Mai_R28_Interaction");yield return Press(mai,s.schoolChapter.face.ghostHead.position,"Mai");
  yield return Walk(W(9.7f,3.6f,8.5f),"school_return_aisle");yield return Walk(W(4.5f,3.6f,7),"school_door",W(2.1f,4.75f,5.12f));
  yield return Walk(W(4.6f,3.6f,3),"exit_through_door");yield return Walk(W(2.1f,3.6f,3),"exit_sign_corridor");
  var back=FindObjectsByType<TuongTacTruongV37>().First(t=>t.name=="203_Return");yield return Press(back,back.transform.position,"wake_school");yield return Hub("after-school");
  Check("only_Minh_seen",papers.memory.viewed==1);yield return Bed(3);
  for(float z=2;z<=20;z+=2){float x=z<12?0:(z-12)*.1f;yield return Walk(new Vector3(-180+x,0,-60+z),"slope_"+z);}
  yield return Walk(new Vector3(-178.8f,0,-38.9f),"case",a.tagRest.position);yield return Press(a.tag.GetComponent<DichTuongTacHanhLyR41>(),a.tag.position,"tag");Look(eye.transform.position+Vector3.forward*10);yield return Wait(.8f,"READ_tag_only");yield return KeyPress(Key.R);
  Check("only_tag_memory",a.memory.viewed==1);yield return ContinueSlope();
  yield return KeyPress(Key.P);yield return Wait(.7f,"AWAKE_ending_memory");Still("ending-memory");yield return KeyPress(Key.E);Check("memory_does_not_advance_ending",s.data.endingBeat==0);yield return KeyPress(Key.P);
  for(int i=0;i<4;i++){Still("ending-page-"+i);yield return Wait(.8f,"ending_page_"+i);if(i<3)yield return KeyPress(Key.E);}
  Check("full_no_item_action_points",s.data.actionPoints==0&&s.data.interactions==0);
 }
 IEnumerator SchoolApproach(){
  yield return Walk(W(0,0,-25),"school_gate");yield return Walk(W(-6.5f,0,-5),"court");yield return Walk(W(-6.5f,0,-3.45f),"stair_bottom");yield return Walk(W(-6.5f,3.6f,4.7f),"stairs_up");yield return Walk(W(-3.5f,3.6f,4.7f),"gallery_start");yield return Walk(W(4.6f,3.6f,3),"gallery");
 }

 void UiAudit(string name){
  Canvas.ForceUpdateCanvases();var rows=new JArray();var bad=new JArray();
  foreach(var t in s.GetComponentsInChildren<UnityEngine.UI.Text>()){
   if(!t.isActiveAndEnabled||t.GetComponentsInParent<CanvasGroup>().Any(g=>g.alpha<.01f)||t.GetComponentsInParent<Canvas>().Any(c=>!c.enabled))continue;
   var r=t.rectTransform;var canvas=t.GetComponentInParent<Canvas>();var canvasRect=((RectTransform)canvas.transform).rect;var bounds=RectTransformUtility.CalculateRelativeRectTransformBounds(canvas.transform,t.transform);bool scroll=t.GetComponent<UnityEngine.UI.ContentSizeFitter>();
   var settings=t.GetGenerationSettings(r.rect.size);t.cachedTextGenerator.Populate(t.text,settings);int font=t.resizeTextForBestFit?t.cachedTextGenerator.fontSizeUsedForBestFit:t.fontSize;settings.resizeTextForBestFit=false;settings.fontSize=t.resizeTextForBestFit?Mathf.RoundToInt(font/t.pixelsPerUnit):font;settings.verticalOverflow=VerticalWrapMode.Overflow;
   float preferred=new TextGenerator().GetPreferredHeight(t.text,settings)/t.pixelsPerUnit;bool clipped=!scroll&&preferred>r.rect.height+2;bool outside=!scroll&&(bounds.min.x<canvasRect.xMin-2||bounds.max.x>canvasRect.xMax+2||bounds.min.y<canvasRect.yMin-2||bounds.max.y>canvasRect.yMax+2);
   var row=new JObject{["panel"]=t.transform.parent.name,["text"]=t.text,["font"]=font,["logicalFont"]=settings.fontSize,["pixelsPerUnit"]=t.pixelsPerUnit,["preferredHeight"]=preferred,["height"]=r.rect.height,["clipped"]=clipped,["outside"]=outside,["scrollContent"]=scroll,["canvasMin"]=V(bounds.min),["canvasMax"]=V(bounds.max)};rows.Add(row);if(clipped||outside)bad.Add(row);
  }
  File.WriteAllText(output+"/ui-"+name+".json",new JObject{["width"]=Screen.width,["height"]=Screen.height,["mode"]=s.GetComponent<DieuPhoiGiaoDienR42>().Mode,["texts"]=rows,["problems"]=bad}.ToString());
  Check("layout_"+name,bad.Count==0);
 }
 IEnumerator MenuPair(){
  if(NoCapture)yield break;
  yield return new WaitForEndOfFrame();var t=ScreenCapture.CaptureScreenshotAsTexture();if(t){File.WriteAllBytes(output+"/menu-ScreenCapture.png",t.EncodeToPNG());Destroy(t);}Still("menu-URP");
  File.WriteAllText(output+"/menu-scope.json",new JObject{["menuOpen"]=s.MenuOpen,["focused"]=Application.isFocused,["batchMode"]=Application.isBatchMode,["nativeWindowInput"]=false,["conclusion"]="ScreenCapture includes final frame/IMGUI; SingleCameraRequest only camera/Canvas. Physical Windows remains OPEN."}.ToString());
 }
 IEnumerator Matrix(){
  string seed=File.ReadAllText("Docs/Reviews/journey_v48/r42/binary-full/isolated-save.json");
  foreach(int height in new[]{720,1080}){
   Screen.SetResolution(height==720?1280:1920,height,false);yield return Wait(.4f);
   foreach(string end in new[]{"A","A2","B","C"}){
    var state=JsonUtility.FromJson<DieuPhoiHanhTrinh.State>(seed);state.ending=end;state.endingBeat=0;state.awareness=end=="B"||end=="C"?3:2;state.interactions=end=="A2"||end=="C"?2:1;
    Check("threshold_"+end,DieuPhoiHanhTrinh.ResolveEnding(state.awareness,state.interactions)==end);File.WriteAllText(s.SavePathOverride,JsonUtility.ToJson(state));yield return KeyPress(Key.F6);yield return Wait(.2f);Stop();
    for(int i=0;i<4;i++){Still(end+"-"+height+"-"+i);if(i<3)yield return KeyPress(Key.E);}
    int beat=s.data.endingBeat;yield return KeyPress(Key.P);yield return Wait(.3f);Still(end+"-"+height+"-memory");yield return KeyPress(Key.E);Check(end+"_memory_blocks_E",s.data.endingBeat==beat);yield return KeyPress(Key.R);Check(end+"_memory_blocks_restart",s.data.ending==end);yield return KeyPress(Key.P);
    yield return KeyPress(Key.Escape);yield return KeyPress(Key.E);Check(end+"_pause_blocks_E",s.Paused&&s.data.endingBeat==beat);yield return KeyPress(Key.Escape);
   }
  }
 }
 IEnumerator Regression(){
  foreach(var path in new[]{"Docs/Reviews/journey_v48/r40/editor-main/placed.json","Docs/Reviews/journey_v48/r35/native-main/confirmed.json","Docs/Reviews/journey_v48/r34/native-main/choice-final.json"}){
   string original=File.ReadAllText(path);var old=JsonUtility.FromJson<DieuPhoiHanhTrinh.State>(original);File.WriteAllText(s.SavePathOverride,original);yield return KeyPress(Key.F6);yield return Wait(.4f);Stop();int awareness=s.data.awareness,actions=s.data.actionPoints;Check("legacy_region_"+old.region,s.data.region==old.region&&s.data.nextRegion==old.nextRegion&&s.data.awareness==old.awareness&&s.data.interactions==old.interactions&&a.memory.viewed==0);Audit("legacy_"+old.region);
   var target=old.region==0?(IPlayerInteractable)s.GetComponent<TuongTacSoTayHoR37>().book.GetComponent<DichTuongTacSoTayR37>():old.region==1?FindObjectsByType<GoiHangChoR35>().First():null;
   if(target!=null)Check("legacy_prompt_"+old.region,HuongDanHanhTrinhV48.Prompt(target).Length>0);
   s.Save();var saved=JsonUtility.FromJson<DieuPhoiHanhTrinh.State>(File.ReadAllText(s.SavePathOverride));Check("legacy_save_no_points_"+old.region,saved.awareness==awareness&&saved.actionPoints==actions&&saved.slopePreparationsR41.Contains("\"viewed\":0"));yield return KeyPress(Key.F6);yield return Wait(.3f);Check("legacy_source_untouched_"+old.region,File.ReadAllText(path)==original);
  }
 }
 IEnumerator ContinueSlope(){
  for(float z=22;z<=42;z+=2){float x=z<25?(z-12)*.1f:1.3f;yield return Walk(new Vector3(-180+x,0,-60+z),"CONTINUE_slope_"+z,branch=="skip"?new Vector3(-190,6,-16):(Vector3?)null);}
  var talk=s.slopeChapter.environment.GetComponentsInChildren<TuongTacDocV39>(true).First(t=>!t.finalDoor&&t.memoryIndex<0);yield return Walk(new Vector3(-178.7f,0,-17.5f),"MEET_Tung",talk.transform.position);
  if(branch=="edge")Check("abandon_returned_safe",!a.Held&&a.memory.abandoned&&a.memory.tagSlot==1);
  yield return Press(talk,talk.transform.position,"TALK_Tung");yield return Wait(3,"RESULT_Tung_response");Still("Tung-response");Check("content_response_or_original",a.memory.viewed==0?s.data.slopeTalks>0:a.memory.response>=0);Save("before-final");
  yield return Walk(new Vector3(-178,0,-15.5f),"FINAL_door_approach");var door=s.slopeChapter.environment.GetComponentsInChildren<TuongTacDocV39>(true).First(t=>t.finalDoor);yield return Press(door,door.transform.position,"FINAL_door");Look(branch=="skip"?eye.transform.position+Vector3.right*10:s.slopeChapter.puppet.dauTransform.position-Vector3.up*.15f);Stop();var puppet=s.slopeChapter.puppet.transform.position;yield return Wait(2,"GD4_stands_looks_no_imitation");Still("GD4");
  Check("GD4_once_props_returned",s.data.finalStarts==1&&!a.Ready&&!a.Held&&Vector3.Distance(puppet,s.slopeChapter.puppet.transform.position)<.001f);
  if(branch=="edge"){yield return KeyPress(Key.Escape);float elapsed=s.data.finalElapsed;yield return Wait(.4f);Check("GD4_pause_stops_elapsed",s.Paused&&Mathf.Abs(s.data.finalElapsed-elapsed)<.05f);yield return KeyPress(Key.F5);yield return KeyPress(Key.F6);yield return Wait(.4f);Check("GD4_pause_load_once",!s.Paused&&s.data.finalStarts==1&&s.data.finalElapsed>=elapsed&&s.data.finalElapsed<elapsed+2);Look(s.slopeChapter.puppet.dauTransform.position);}
  yield return Until(()=>!string.IsNullOrEmpty(s.data.ending),18,"ENDING_existing_transition");Check("ending_original_threshold",s.data.finalStarts==1&&s.data.ending==DieuPhoiHanhTrinh.ResolveEnding(s.data.awareness,s.data.interactions)&&s.data.nextRegion==4);if(branch=="skip")Check("GD4_missed_no_auto_witness",!s.data.slopeWitnessed);
  Check("dream_items_not_real_hub",!a.root.gameObject.activeInHierarchy&&a.memory.held==-1);yield return Wait(1,"HUB_existing_ending");if(a.memory.viewed!=0){yield return KeyPress(Key.P);yield return Wait(2,"HUB_memory_next_to_existing_message");Still("hub-memory");Check("readonly_memory_no_actions",s.data.actionPoints==0&&s.data.interactions==0&&a.Notes.Length>0);yield return KeyPress(Key.P);}
  Audit("ending");phase="DONE";yield return Wait(.5f);
 }
}

#endif
