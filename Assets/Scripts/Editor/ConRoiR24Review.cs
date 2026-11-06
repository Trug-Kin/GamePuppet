using System;
using System.IO;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.InputSystem;

[InitializeOnLoad]
public static class ConRoiR24Review
{
 const string Out=ConRoiR24Reference.Out;static IEnumerator routine;static double deadline;static Stack<IEnumerator> pending;
 static JArray checks,errors,frames;static DieuPhoiHanhTrinh s;static Camera eye;static DieuKhienGocNhin mouse;static MoDauBenHoR23 encounter;static int serial;
 static ConRoiR24Review(){EditorApplication.playModeStateChanged+=state=>{if(state==PlayModeStateChange.EnteredPlayMode&&SessionState.GetBool("r24.review",false)){checks=new JArray();errors=new JArray();frames=new JArray();routine=Run();deadline=EditorApplication.timeSinceStartup+900;Application.logMessageReceived+=Log;EditorApplication.update+=Tick;}};}
 public static void Begin(){EditorSceneManager.OpenScene(ConRoiR24Reference.Scene);SessionState.SetBool("r24.review",true);EditorApplication.EnterPlaymode();}
 public static void AuthorAndBegin(){ConRoiR24Reference.Author();Begin();}
 static void Log(string message,string trace,LogType type){if(type==LogType.Error||type==LogType.Exception||type==LogType.Assert)errors.Add(new JObject{["message"]=message,["trace"]=trace});}
 static void Check(string name,bool pass,JToken detail=null){checks.Add(new JObject{["name"]=name,["pass"]=pass,["detail"]=detail});Debug.Log("R23_CHECK "+name+" "+pass);}
 static void Tick(){try{
  EditorApplication.QueuePlayerLoopUpdate();if(EditorApplication.timeSinceStartup>deadline)throw new Exception("Timeout");
  if(pending==null){pending=new Stack<IEnumerator>();pending.Push(routine);}
  while(pending.Count>0){var top=pending.Peek();if(!top.MoveNext()){pending.Pop();continue;}if(top.Current is IEnumerator nested){pending.Push(nested);continue;}return;}
  Finish(null);
 }catch(Exception e){Finish(e.ToString());}}
 static void Finish(string error){bool complete=error==null&&errors.Count==0&&checks.All(c=>(bool)c["pass"]);File.WriteAllText(Out+"/playmode.json",new JObject{["complete"]=complete,["failure"]=error,["checks"]=checks,["errors"]=errors,["method"]="Editor Play Mode, direct story interaction APIs; real PlayerMovement filter for normal walking, URP render requests from player camera. Initial loading coroutine bypassed because WaitForEndOfFrame stalls in batch. Not Windows input or first-time comprehension certification.",["native"]="NOT_RUN; diagnostics PAUSED; INTERRUPTED/NOT_RUN retained"}.ToString());File.WriteAllText(Out+"/film-cameras.json",frames.ToString());SessionState.SetBool("r24.review",false);EditorApplication.update-=Tick;EditorApplication.Exit(complete?0:1);}
 static IEnumerator Wait(float seconds){float until=Time.time+seconds;while(Time.time<until)yield return null;}
 static IEnumerator Do(IEnumerator it){while(it.MoveNext())yield return null;}
 static void Shot(string name){ConRoiR22Scenery.Capture(eye,Out+"/play/"+name+".png");}
 static JArray V(Vector3 v)=>new JArray(v.x,v.y,v.z);
 static void Look(Vector3 target){if(mouse)mouse.enabled=false;var d=target-eye.transform.position;var flat=d;flat.y=0;s.player.transform.rotation=Quaternion.LookRotation(flat);eye.transform.rotation=Quaternion.LookRotation(d);}
 static IEnumerator Film(float seconds,Vector2 input,Vector3? target=null){
  s.player.MovementFilter=raw=>input;int n=Mathf.RoundToInt(seconds*12);
  for(int i=0;i<n;i++){
   if(mouse)mouse.enabled=false;if(target.HasValue){var next=Quaternion.RotateTowards(eye.transform.rotation,Quaternion.LookRotation(target.Value-eye.transform.position),90*Time.deltaTime);s.player.transform.rotation=Quaternion.Euler(0,next.eulerAngles.y,0);eye.transform.rotation=next;}
   yield return null;
   ConRoiR22Scenery.Capture(eye,Out+"/frames/frame-"+serial.ToString("D5")+".png");
   frames.Add(new JObject{["frame"]=serial++,["t"]=Time.unscaledTime,["position"]=V(s.player.transform.position),["eye"]=V(eye.transform.position),["rotation"]=V(eye.transform.eulerAngles),["input"]=new JArray(input.x,input.y),["region"]=s.data.region,["observed"]=encounter.Observed});
  }
  s.player.MovementFilter=null;
 }
 static IEnumerator Run(){
  s=UnityEngine.Object.FindAnyObjectByType<DieuPhoiHanhTrinh>();eye=s.player.GetComponentInChildren<Camera>();mouse=s.player.GetComponentInChildren<DieuKhienGocNhin>();encounter=s.GetComponent<MoDauBenHoR23>();
  InputSystem.settings.updateMode=InputSettings.UpdateMode.ProcessEventsManually;Application.runInBackground=true;
  Check("isolated_r24_profile",s.SavePathOverride.Contains("r24-playtest"));
  var profile=s.GetComponent<HoSoSceneR24>();if(!string.IsNullOrEmpty(profile.SeededFrom))Check("first_profile_is_readonly_copy",File.ReadAllText(profile.SeededFrom)==File.ReadAllText(s.SavePathOverride));
  s.SavePathOverride=Path.GetFullPath(Out+"/fixture-save.json");s.SetMenu(false);s.Restart();
  yield return null;yield return null;var loader=s.GetComponent<ManHinhTaiV44>();loader.StopAllCoroutines();Shot("boarding-house");s.SetLoading(false);loader.enabled=false;
  Check("boarding_house_retained",s.data.region==-1&&!s.DreamHub);s.Sleep();Check("dream_room_retained",s.DreamHub);yield return Do(Wait(4));
  if(mouse)mouse.enabled=false;Check("new_game_lake_entry",s.data.region==0&&Vector3.Distance(s.player.transform.position,s.lakeSpawn.position)<.15f,V(s.player.transform.position));
  Check("spawn_supported",s.GetComponent<PhucHoiTaiBienR21>().Supported(s.player.transform.position,.5f));
  Check("arrival_not_observed_or_trace",!s.data.witnessed[0]&&!s.data.traces[0]&&!encounter.Observed);
  Check("arrival_no_stacked_reveal_audio",s.WoodEffectsPlayed==0&&!s.data.faces[0]);
  Shot("arrival");
  // Miss the first opportunity deliberately. No partial or stale motion credit.
  Look(eye.transform.position-Vector3.forward*10);s.player.MovementFilter=raw=>new Vector2(.18f,0);yield return Do(Wait(.6f));s.player.MovementFilter=null;yield return Do(Wait(.7f));
  Check("offscreen_step_stop_not_observation",!encounter.Observed&&!s.data.witnessed[0]);
  var saved=s.player.transform.position;s.Save();s.Place(s.lakeSpawn.position,s.lakeSpawn.rotation);s.Load();yield return Do(Wait(.3f));if(mouse)mouse.enabled=false;
  Check("valid_loaded_pose_retained",Vector3.Distance(saved,s.player.transform.position)<.10f&&Vector3.Distance(saved,s.lakeSpawn.position)>.15f,V(s.player.transform.position));
  Check("partial_observation_not_invented_on_load",!encounter.Observed);
  // New Game -> real three-second dream transition once more, now one
  // continuous first-person sequence from actual lake arrival onward.
  s.Restart();s.Sleep();yield return Do(Wait(3.3f));if(mouse)mouse.enabled=false;
  Time.captureDeltaTime=1f/12;
  yield return Do(Film(3,Vector2.zero));
  var bao=s.GetComponent<TuTheBaoBenCauV48>().actorPosition+Vector3.up*1.2f;Look(bao);
  yield return Do(Film(.55f,new Vector2(0,.25f),bao));yield return Do(Film(.5f,Vector2.zero,bao));
  var interaction=s.player.GetComponent<TuongTacNguoiChoi>();Check("bao_reachable_by_view_ray",interaction.Target is TuongTacHoCho b&&b.kind==TuongTacHoCho.Kind.Bao, new JValue(interaction.Target?.ToString()));
  interaction.Target?.Interact();Check("bao_dialogue",s.Interactions>0&&!string.IsNullOrEmpty(s.Dialogue));
  yield return Do(Film(5.5f,Vector2.zero,bao));Shot("bao-meeting");
  var p=s.puppets[0];var chest=p.dauTransform.position-Vector3.up*.55f;
  yield return Do(Film(2.5f,Vector2.zero,chest));Shot("neutral-invitation");
  Check("invitation_before_conclusion",!encounter.Observed&&!s.GetComponent<TraiNghiemHanhTrinhR23>().Observation.Contains("vừa bước"));
  // Small steps parallel to rail, back and forth; physical controller support.
  yield return Do(Film(.6f,new Vector2(0,.24f),chest));yield return Do(Film(1,Vector2.zero,chest));
  yield return Do(Film(.6f,new Vector2(0,-.24f),chest));yield return Do(Film(1,Vector2.zero,chest));
  Shot("gd1-feedback");Check("visible_step_stop_observed",encounter.Observed&&s.data.witnessed[0],new JObject{["travel"]=encounter.SeenTravel,["opportunity"]=encounter.Opportunity});
  Check("puppet_not_E_target",!p.GetComponentInChildren<TuongTacHoCho>());
  Check("normal_controller_still_supported",s.player.controller.isGrounded&&s.GetComponent<PhucHoiTaiBienR21>().Supported(s.player.transform.position,.5f));
  yield return Do(Film(4,Vector2.zero,chest));s.OpenPhone();if(mouse)mouse.enabled=false;yield return Do(Film(4,Vector2.zero,chest));Shot("phone-note-next-goal");
  var experience=s.GetComponent<TraiNghiemHanhTrinhR23>();Check("note_and_next_goal",experience.PhoneText.Contains("hình người gỗ")&&experience.Objective.Contains("TỈNH DẬY"));
  var phone= s.polish.GetComponentsInChildren<UnityEngine.UI.Text>(true).Single(t=>t.transform.parent&&t.transform.parent.name=="Phone");Check("phone_fits",phone.preferredHeight<=phone.rectTransform.rect.height,new JObject{["height"]=phone.preferredHeight,["available"]=phone.rectTransform.rect.height});
  // Load closes the phone through existing Controls/ApplyRegion behavior.
  s.Save();saved=s.player.transform.position;s.Load();yield return Do(Wait(.3f));if(mouse)mouse.enabled=false;Check("completed_note_survives_load",encounter.Observed&&Vector3.Distance(saved,s.player.transform.position)<.1f);
  var reflection=UnityEngine.Object.FindAnyObjectByType<PhanChieuMatHo>();Check("reflection_active",reflection&&reflection.HasRendered);
  var rt=typeof(PhanChieuMatHo).GetField("texture",System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Instance).GetValue(reflection) as RenderTexture;
  if(rt){var prev=RenderTexture.active;RenderTexture.active=rt;var tex=new Texture2D(rt.width,rt.height,TextureFormat.RGB24,false);tex.ReadPixels(new Rect(0,0,rt.width,rt.height),0,0);tex.Apply();File.WriteAllBytes(Out+"/play/reflection-texture.png",tex.EncodeToPNG());RenderTexture.active=prev;UnityEngine.Object.DestroyImmediate(tex);}
  var contact=BaoContactProbeV48.Sample(s.GetComponent<TuTheBaoBenCauV48>());Check("bao_contact_preserved",contact["shoes"].All(q=>q["gap"].Type!=JTokenType.Null&&Mathf.Abs((float)q["gap"])<.025f)&&contact["sleeves"].All(q=>(float)q["triangleToTopRailSurfaceMin"]>=-.003f&&(float)q["triangleToTopRailSurfaceMin"]<.025f),contact);
  var userSave=Path.Combine(Application.persistentDataPath,"r22-playtest/journey-v48.json");
  if(File.Exists(userSave)){
   var original=File.ReadAllText(userSave);var old=JsonUtility.FromJson<DieuPhoiHanhTrinh.State>(original);var path=s.SavePathOverride;File.WriteAllText(path,original);s.Load();yield return Do(Wait(.2f));
   Check("r22_save_private_copy_retains_valid_pose",Vector3.Distance(s.player.transform.position,old.position)<.12f,new JObject{["expected"]=V(old.position),["actual"]=V(s.player.transform.position),["region"]=s.data.region});
  }
  Time.captureDeltaTime=0;
  // Missed encounter has no gate: wake/sleep continues to market.
  s.Restart();s.Sleep();yield return Do(Wait(3.5f));Check("fresh_trial_unobserved",!encounter.Observed);s.Wake();s.Sleep();yield return Do(Wait(3.5f));Check("missed_encounter_still_reaches_market",s.data.region==1);
  Check("market_multiple_delayed_puppets",s.puppets.Count(ai=>ai.gameObject.activeInHierarchy)>=2&&s.puppets.Where(ai=>ai.gameObject.activeInHierarchy).All(ai=>ai.giaiDoanHienTai==DieuKhienConRoi.GiaiDoan.GD2&&ai.doTreGD2>=.4f&&ai.doTreGD2<=.6f));
  var marketP=s.puppets.First(ai=>ai.gameObject.activeInHierarchy);Look(marketP.dauTransform.position-Vector3.up*.4f);Shot("market-context");
  // Separate chapter inspection, outside the continuous lake film.
  var anchor=marketP.dauTransform.position-Vector3.up*1.58f;bool placed=false;
  for(int k=0;k<8;k++){
   var candidate=anchor+Quaternion.Euler(0,k*45,0)*Vector3.forward*3.5f;
   foreach(var h in Physics.RaycastAll(candidate+Vector3.up*4,Vector3.down,8,8,QueryTriggerInteraction.Ignore).OrderBy(h=>h.distance)){
    candidate.y=h.point.y+.93f;if(s.GetComponent<PhucHoiTaiBienR21>().Supported(candidate,.5f)){s.Place(candidate,Quaternion.identity);placed=true;break;}
   }if(placed)break;
  }
  Check("market_inspection_on_supported_surface",placed);
  if(mouse)mouse.enabled=false;Look(marketP.dauTransform.position-Vector3.up*.55f);yield return Do(Wait(2));
  Time.captureDeltaTime=1f/30;var rootBefore=marketP.transform.position;float firstPlayer=-1,firstPuppet=-1;var priorPlayer=s.player.transform.position;var series=new JArray();
  float start=Time.time;while(Time.time-start<1.4f){
   s.player.MovementFilter=raw=>Time.time-start<.40f?new Vector2(.15f,0):Vector2.zero;
   yield return null;var pd=s.player.transform.position-priorPlayer;pd.y=0;priorPlayer=s.player.transform.position;
   if(firstPlayer<0&&pd.magnitude>.005f)firstPlayer=Time.time;
   var outDelta=marketP.transform.position-rootBefore;outDelta.y=0;
   if(firstPuppet<0&&outDelta.magnitude>.005f)firstPuppet=Time.time;
   series.Add(new JObject{["t"]=Time.time-start,["player_delta"]=pd.magnitude,["puppet_planar_delta"]=outDelta.magnitude});
  }
  s.player.MovementFilter=null;yield return Do(Wait(.8f));Shot("market-delayed-observation");
  Check("gd2_measured_delay",firstPuppet>0&&firstPlayer>0&&firstPuppet-firstPlayer>=.39f&&firstPuppet-firstPlayer<=.64f,new JValue(firstPuppet-firstPlayer));
  Check("gd2_note_distinct_from_lake",encounter.Observed&&s.GetComponent<TraiNghiemHanhTrinhR23>().Observation.Contains("nhiều"));
  File.WriteAllText(Out+"/market-delay.json",series.ToString());Time.captureDeltaTime=0;
  s.Wake();s.Sleep();yield return Do(Wait(3.5f));Check("school_still_GD3",s.data.region==2&&s.schoolChapter.puppet.giaiDoanHienTai==DieuKhienConRoi.GiaiDoan.GD3);
  Shot("school-gameplay");
  s.Wake();s.Sleep();yield return Do(Wait(3.5f));Check("slope_still_GD4",s.data.region==3&&s.slopeChapter.puppet.giaiDoanHienTai==DieuKhienConRoi.GiaiDoan.GD4);
  Shot("slope-gameplay");ConRoiR24Reference.CheckGeometry(s);
  // Continuous walking with the retained controller. Steer toward the centre of the curved ramp.
  var walk=new JArray();Time.captureDeltaTime=1f/12;int walkFrame=0;float walkStart=s.player.transform.position.z;
  while(s.player.transform.position.z<-17.5f&&walkFrame<420){
   float localZ=s.player.transform.position.z+60;float ahead=localZ+2;float cx=-180+Mathf.Clamp01((ahead-18)/12)*2+.45f;
   var target=new Vector3(cx,Mathf.Min(ahead,30)*.2f+1.65f,-60+ahead);Look(target);s.player.MovementFilter=raw=>new Vector2(0,.65f);
   yield return null;
   ConRoiR22Scenery.Capture(eye,Out+"/slope-walk/frame-"+walkFrame.ToString("D5")+".png");
   walk.Add(new JObject{["frame"]=walkFrame++,["position"]=V(s.player.transform.position),["eye"]=V(eye.transform.position),["grounded"]=s.player.controller.isGrounded,["region"]=s.data.region});
  }
  s.player.MovementFilter=null;Time.captureDeltaTime=0;
  Check("slope_ordinary_walk_reaches_tung",s.player.transform.position.z>=-17.5f&&s.player.transform.position.z-walkStart>35,V(s.player.transform.position));
  Check("slope_trace_retained",s.data.slopeTrace);
  File.WriteAllText(Out+"/slope-walk.json",walk.ToString());Shot("slope-walk-finish");
 }
}
