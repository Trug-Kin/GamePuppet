#if UNITY_EDITOR || CON_ROI_BUILD_REVIEW
using System;
using System.Collections;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
public sealed partial class C3ActualPlayRun
{
 public bool walkthroughRun;
 DieuPhoiHanhTrinh walkingStory;
 void WalkCheck(string name,bool pass,JObject details=null){var row=details??new JObject();row["case"]=name;row["status"]=pass?"PASS":"FAIL";row["region"]=walkingStory.data.region;row["position"]=player.transform.position.ToString("F4");results.Add(row);File.WriteAllText(folder+"/progress.json",results.ToString());}
 IEnumerator WalkPress(Key key){RegionKey(key);yield return null;yield return null;RegionKey(null);yield return null;yield return null;float deadline=Time.realtimeSinceStartup+15;while(walkingStory&&walkingStory.Loading&&Time.realtimeSinceStartup<deadline)yield return null;}
 IEnumerator WalkUse(TuongTacHoCho.Kind kind)
 {
  var target=FindObjectsByType<TuongTacHoCho>(FindObjectsInactive.Exclude).FirstOrDefault(x=>x.kind==kind&&x.GetComponent<Collider>());if(!target){WalkCheck("missing_"+kind,false);yield break;}
  var eye=player.GetComponentInChildren<Camera>();lookController.enabled=false;var aim=target.GetComponent<Collider>().bounds.center;
  if(kind==TuongTacHoCho.Kind.Ha||kind==TuongTacHoCho.Kind.Bao)aim=walkingStory.ghosts[kind==TuongTacHoCho.Kind.Ha?1:0].GetComponent<HienGuongMatTheoKhu>().ghostHead.position;
  eye.transform.LookAt(aim);yield return null;if(kind==TuongTacHoCho.Kind.Ha){yield return CapturePlayerImage(Path.GetFullPath(folder+"/Ha_before_face.png"));}yield return WalkPress(Key.E);
  if(kind==TuongTacHoCho.Kind.Bed){float deadline=Time.realtimeSinceStartup+15;while((walkingStory.DreamHub||walkingStory.Loading)&&Time.realtimeSinceStartup<deadline)yield return null;}
  WalkCheck("E_"+kind,kind==TuongTacHoCho.Kind.Bed?walkingStory.data.region>=0:kind==TuongTacHoCho.Kind.Wake?walkingStory.data.region<0:walkingStory.Interactions>0);lookController.enabled=false;
  if(kind==TuongTacHoCho.Kind.Bed&&walkingStory.GetComponent<HuongDanHanhTrinhV48>())yield return V48GuidanceCheck();
 }
 IEnumerator WalkRoute(Vector3[] route,int first,int last,string name)
 {
  caseId=name;int reached=0;for(int i=first;i<=last;i++){yield return RegionalGo(route[i],name+"_"+i,10);if(!regionalReached)break;reached++;}WalkCheck(name,reached==last-first+1,new JObject{["reached"]=reached,["total"]=last-first+1});
 }
 IEnumerator RunStoryWalkthrough()
 {
  previousBackground=Application.runInBackground;Application.runInBackground=true;player=GetComponent<DieuKhienDiChuyen>();cc=player.controller;player.DisableTestInputHook();walkingStory=FindAnyObjectByType<DieuPhoiHanhTrinh>();
  if(walkingStory.schoolChapter&&string.IsNullOrEmpty(VerificationOutput))VerificationOutput=walkingStory.slopeChapter?"Docs/Reviews/journey_v39":"Docs/Reviews/school_v38";
  folder=VerificationFolder("walkthrough");Directory.CreateDirectory(folder);walkingStory.SavePathOverride=Path.GetFullPath(folder+"/test-save.json");
  frames=new StreamWriter(folder+"/frames.csv"){AutoFlush=true};frames.WriteLine("case,phase,time,dt,x,y,z,foot_y,controller_grounded,ground_check,input_x,input_y,cc_vx,cc_vy,cc_vz,top_support_y,support_names,vertical_speed,move_flags,move_flags_copy,grounded_after_move,frame_contacts");
  lookController=player.GetComponent<DieuKhienGocNhin>();previousLookEnabled=lookController.enabled;lookController.enabled=false;contacts=gameObject.AddComponent<C3PlayContacts>();
  previousKeyboard=Keyboard.current;previousMouse=Mouse.current;previousInputBackground=InputSystem.settings.backgroundBehavior;previousEditorInputBehavior=InputSystem.settings.editorInputBehaviorInPlayMode;InputSystem.settings.backgroundBehavior=InputSettings.BackgroundBehavior.IgnoreFocus;InputSystem.settings.editorInputBehaviorInPlayMode=InputSettings.EditorInputBehaviorInPlayMode.AllDeviceInputAlwaysGoesToGameView;regionalInputSettingsChanged=true;regionKeyboard=InputSystem.AddDevice<Keyboard>("WalkthroughKeyboard");regionMouse=InputSystem.AddDevice<Mouse>("WalkthroughMouse");regionKeyboard.MakeCurrent();regionMouse.MakeCurrent();
  yield return new WaitForSeconds(.5f);var route=JObject.Parse(regionalRoute.text)["route"].Select(P).ToArray();
  if(journeySuite=="feedback-v48"){yield return RunFeedbackV48(route);yield break;}
  if(walkingStory.GetComponent<HuongDanHanhTrinhV48>()){walkingStory.SetMenu(true);yield return new WaitForSecondsRealtime(15);yield return WalkPress(Key.Enter);yield return new WaitForSeconds(.25f);lookController.enabled=false;WalkCheck("entry_New_Game",!walkingStory.MenuOpen&&walkingStory.data.region==-1);WalkCheck("objective_after_long_menu_wait",walkingStory.GetComponent<HuongDanHanhTrinhV48>().ReminderVisible);}
  WalkCheck("start_in_hub",walkingStory.data.region==-1&&cc.isGrounded);
  if(walkingStory.GetComponent<HuongDanHanhTrinhV48>())yield return V48DeskView();
  yield return RegionalGo(new Vector3(-35,.94f,-34.6f),"hub_to_bed",5);yield return WalkUse(TuongTacHoCho.Kind.Bed);
  if(walkingStory.data.region!=0){yield return FinishWalk();yield break;}
  if(walkingStory.GetComponent<HuongDanHanhTrinhV48>())yield return ExperiencePhaseObservation(player.GetComponentInChildren<Camera>());
  yield return WalkRoute(route,0,3,"lake_to_Bao");
  var bao=walkingStory.ghosts[0].position;yield return RegionalGo(new Vector3(bao.x-1.1f,bao.y,bao.z-.9f),"Bao_approach",8);yield return WalkUse(TuongTacHoCho.Kind.Bao);WalkCheck("Bao_dialogue",walkingStory.Dialogue.Contains("Bảo"));
  yield return RegionalGo(route[3],"return_to_lake_path",8);yield return WalkRoute(route,4,31,"lake_to_exit");
  if(Array.IndexOf(Environment.GetCommandLineArgs(),"--lake-only")>=0){yield return FinishWalk();yield break;}
  yield return WalkUse(TuongTacHoCho.Kind.Wake);
  if(walkingStory.data.region!=-1){yield return FinishWalk();yield break;}
  yield return RegionalGo(new Vector3(-35,.94f,-34.6f),"hub_second_bed",5);yield return WalkUse(TuongTacHoCho.Kind.Bed);
  if(walkingStory.data.region!=1){yield return FinishWalk();yield break;}
  if(walkingStory.GetComponent<HuongDanHanhTrinhV48>())yield return ExperiencePhaseObservation(player.GetComponentInChildren<Camera>());
  var witnessCamera=player.GetComponentInChildren<Camera>();
  foreach(var ai in walkingStory.puppets.Where(x=>x.gameObject.activeInHierarchy)){witnessCamera.transform.LookAt(ai.dauTransform?ai.dauTransform.position:ai.transform.position+Vector3.up);yield return WalkPress(Key.W);yield return new WaitForSeconds(.75f);if(walkingStory.data.witnessed[1])break;}
  yield return WalkRoute(route,165,route.Length-1,"market_continuous");
  var ha=walkingStory.ghosts[1].position;yield return RegionalGo(ha+new Vector3(-1.3f,0,-1.7f),"Ha_approach",10);
  var eye=player.GetComponentInChildren<Camera>();
  // Witness actual active GD2 motion using the player camera, never setting story flags.
  foreach(var ai in walkingStory.puppets.Where(x=>x.gameObject.activeInHierarchy)){eye.transform.LookAt(ai.dauTransform?ai.dauTransform.position:ai.transform.position+Vector3.up);yield return new WaitForSeconds(.7f);if(walkingStory.data.witnessed[1])break;}
  WalkCheck("GD2_witnessed_in_view",walkingStory.data.witnessed[1]);yield return WalkUse(TuongTacHoCho.Kind.Ha);WalkCheck("Ha_dialogue",walkingStory.Dialogue.Contains("Chị Hà"));yield return new WaitForSeconds(.15f);
  var face=walkingStory.ghosts[1].GetComponent<HienGuongMatTheoKhu>();WalkCheck("face_crossfade",walkingStory.data.faces[1]&&face.Revealed&&face.SawIntermediateAlpha&&face.LastFadeSeconds>=.075f&&face.LastFadeSeconds<.15f,new JObject{["seconds"]=face.LastFadeSeconds,["samples"]=face.FadeSamples,["intermediate_alpha"]=face.SawIntermediateAlpha});
  yield return CapturePlayerImage(Path.GetFullPath(folder+"/Ha_after_face.png"));yield return WalkPress(Key.E);WalkCheck("Ha_face_only_once",face.RevealCount==1);
  yield return RegionalGo(walkingStory.traces[1].position+new Vector3(-1.7f,0,-1.1f),"books_approach",8);yield return new WaitForSeconds(.2f);WalkCheck("market_books_discovered",walkingStory.data.traces[1]);
  // Walk around the post; the front-facing books approach cannot exercise a rear graze.
  // The right route intersects Ha's solid interaction proxy. Use the open
  // street side to the left, with capsule clearance from both actor and post.
  yield return RegionalGo(new Vector3(783.6f,player.transform.position.y,272f),"post_bypass_front",5);bool frontBypassReached=regionalReached;
  yield return RegionalGo(new Vector3(783.6f,player.transform.position.y,274.1815f),"post_bypass_rear",5);bool rearBypassReached=regionalReached;
  yield return RegionalGo(new Vector3(785.1028f,player.transform.position.y,274.1815f),"post_rear_approach",5);
  bool rearReached=frontBypassReached&&rearBypassReached&&regionalReached;contacts.hits.Clear();
  yield return RegionalGo(new Vector3(785.1028f,player.transform.position.y,273.50f),"post_contact_prerequisite",3);
  bool postContact=rearReached&&contacts.hits.Contains("Context_142");
  WalkCheck("save_post_contact_prerequisite",postContact,new JObject{["rear_reached"]=rearReached,["contacts"]=new JArray(contacts.hits)});
  if(!postContact){WalkCheck("save_post_graze_contacts_and_releases",false,new JObject{["reason"]="Contact prerequisite absent; release and save/load deliberately not credited"});yield return FinishWalk();yield break;}
  yield return RegionalGo(new Vector3(785.1028f,player.transform.position.y,272.1815f),"post_release_after_contact",5);
  bool postReleased=regionalReached;WalkCheck("save_post_graze_contacts_and_releases",postContact&&postReleased);
  if(!postReleased){yield return FinishWalk();yield break;}
  yield return WalkPress(Key.F5);var saved=player.transform.position;int awareness=walkingStory.data.awareness;bool trace=walkingStory.data.traces[1];
  yield return RegionalGo(saved+Vector3.back*.7f,"move_after_save",4);
  yield return RegionalGo(saved,"return_to_saved_position",4);WalkCheck("return_from_post_graze",regionalReached);
  contacts.hits.Clear();yield return RegionalGo(new Vector3(784.8827f,saved.y,273.6947f),"save_post_direct_push",4);
  WalkCheck("save_move_wall_blocks",!regionalReached&&contacts.hits.Contains("Context_142"));yield return RegionalHold("hold_after_save_wall");
  yield return RegionalGo(saved+Vector3.back*.7f,"retreat_from_save_wall",4);WalkCheck("save_pose_actually_left",postContact&&postReleased&&regionalReached&&Vector3.Distance(saved,player.transform.position)>.4f);
  yield return WalkPress(Key.F6);yield return new WaitForSeconds(.2f);lookController.enabled=false;
  WalkCheck("F5_F6_pose_face_trace",Vector3.Distance(saved,player.transform.position)<.15f&&walkingStory.data.awareness==awareness&&walkingStory.data.traces[1]==trace&&face.Revealed&&walkingStory.data.faces[1]);
  yield return RegionalGo(route[route.Length-1],"return_from_Ha",10);yield return WalkUse(TuongTacHoCho.Kind.Wake);WalkCheck("returned_hub",walkingStory.data.region==-1&&walkingStory.data.nextRegion==2);
  if(walkingStory.schoolChapter)yield return ContinueSchoolIntegration();
  if(walkingStory.slopeChapter)yield return ContinueSlopeIntegration();
  yield return FinishWalk();
 }
 IEnumerator FinishWalk()
 {
  RegionKey(null);yield return null;File.WriteAllText(folder+"/result.json",new JObject{["status"]=results.All(x=>(string)x["status"]=="PASS")&&regionalPenetrations==0&&regionalMissingSupport==0?"PASS":"EXECUTED_REVIEW_FAILURES",["scene"]=gameObject.scene.path,["input"]="Virtual Input System -> live actions -> controller / E raycast; OS NOT_VERIFIED",["fixture_teleports"]=journeySuite=="acting"||journeySuite=="hitches"?actingPlacements:0,["relocations"]=journeySuite=="acting"||journeySuite=="hitches"?"Explicit staged region/save fixtures; see inspection-placements.json":"Only runtime sleep/wake and F6 load",["floor_penetration_frames"]=regionalPenetrations,["missing_support_frames"]=regionalMissingSupport,["results"]=results}.ToString());frames.Dispose();frames=null;complete=true;CleanupRegionalInput();
#if UNITY_EDITOR
  UnityEditor.EditorApplication.isPlaying=false;
#else
        Application.Quit(results.All(r=>(string)r["status"]=="PASS")?0:2);
#endif
 }
}


#endif
