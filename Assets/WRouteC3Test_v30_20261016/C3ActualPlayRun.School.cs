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
 public bool schoolRun;
 ChuongTruongHocV37 school;
 Bounds SchoolModelBounds(Transform model)
 {
  var renderers=model.GetComponentsInChildren<Renderer>(true).Where(r=>r.enabled&&r.gameObject.activeInHierarchy).ToArray();var bounds=renderers[0].bounds;foreach(var renderer in renderers)bounds.Encapsulate(renderer.bounds);return bounds;
 }
 IEnumerator SchoolUse(bool exit)
 {
  var target=exit?school.environment.GetComponentsInChildren<TuongTacTruongV37>().First(i=>i.exit).transform:school.ghost;
  var eye=player.GetComponentInChildren<Camera>();lookController.enabled=false;
  eye.transform.LookAt(exit?target.position:school.face.ghostHead.position);yield return null;yield return WalkPress(Key.E);lookController.enabled=false;
 }
 IEnumerator SchoolGo(Vector3 p,string label)
 {
  yield return RegionalGo(p,label,10);WalkCheck(label,regionalReached);
 }
 IEnumerator RunSchool()
 {
  previousBackground=Application.runInBackground;Application.runInBackground=true;player=GetComponent<DieuKhienDiChuyen>();cc=player.controller;player.DisableTestInputHook();walkingStory=FindAnyObjectByType<DieuPhoiHanhTrinh>();school=walkingStory.schoolChapter;
  folder="Docs/Reviews/school_v37/run_"+DateTime.UtcNow.ToString("yyyyMMdd_HHmmss");Directory.CreateDirectory(folder);
  // Continue from an actually executed lake-market walkthrough, preserving its
  // evidence file and all historical state; tests own a copy of that save.
  var previous=new DirectoryInfo("Docs/Reviews/lake_gate_market_current/v36").GetDirectories("walkthrough_*").OrderByDescending(d=>d.Name).First(d=>File.Exists(d.FullName+"/result.json")&&(string)JObject.Parse(File.ReadAllText(d.FullName+"/result.json"))["status"]=="PASS");
  string seed=Path.GetFullPath(folder+"/seed-from-v36.json");File.Copy(previous.FullName+"/test-save.json",seed);string save=Path.GetFullPath(folder+"/school-save.json");File.Copy(seed,save);walkingStory.SavePathOverride=save;walkingStory.Load();
  frames=new StreamWriter(folder+"/frames.csv"){AutoFlush=true};frames.WriteLine("case,phase,time,dt,x,y,z,foot_y,controller_grounded,ground_check,input_x,input_y,cc_vx,cc_vy,cc_vz,top_support_y,support_names,vertical_speed,move_flags,move_flags_copy,grounded_after_move,frame_contacts");
  lookController=player.GetComponent<DieuKhienGocNhin>();previousLookEnabled=lookController.enabled;lookController.enabled=false;contacts=gameObject.AddComponent<C3PlayContacts>();
  previousKeyboard=Keyboard.current;previousMouse=Mouse.current;previousInputBackground=InputSystem.settings.backgroundBehavior;previousEditorInputBehavior=InputSystem.settings.editorInputBehaviorInPlayMode;InputSystem.settings.backgroundBehavior=InputSettings.BackgroundBehavior.IgnoreFocus;InputSystem.settings.editorInputBehaviorInPlayMode=InputSettings.EditorInputBehaviorInPlayMode.AllDeviceInputAlwaysGoesToGameView;regionalInputSettingsChanged=true;regionKeyboard=InputSystem.AddDevice<Keyboard>("SchoolKeyboard");regionMouse=InputSystem.AddDevice<Mouse>("SchoolMouse");regionKeyboard.MakeCurrent();regionMouse.MakeCurrent();
  yield return new WaitForSeconds(.5f);WalkCheck("continued_actual_v36_save",walkingStory.data.region==-1&&walkingStory.data.nextRegion==2);
  yield return SchoolGo(new Vector3(-35,.94f,-34.6f),"hub_to_bed");yield return WalkUse(TuongTacHoCho.Kind.Bed);WalkCheck("bed_enters_school",walkingStory.data.region==2&&school.environment.activeSelf);yield return new WaitForSeconds(.3f);
  var ghostBounds=SchoolModelBounds(school.face.ghostHead);
  WalkCheck("Mai_model_real_scale",ghostBounds.size.y>.2f&&ghostBounds.size.y<.7f&&ghostBounds.center.y>1.2f,new JObject{["head_height_m"]=ghostBounds.size.y});
  var eyeAtEntry=player.GetComponentInChildren<Camera>();eyeAtEntry.transform.LookAt(school.puppet.dauTransform.position);yield return CapturePlayerImage(Path.GetFullPath(folder+"/classroom_entry.png"));
  var origin=school.spawn.position-new Vector3(0,.94f,-7);
  foreach(float z in new[]{-3,1,5})yield return SchoolGo(origin+new Vector3(0,.94f,z),"walk_school_aisle_"+z);
  yield return SchoolGo(origin+new Vector3(1.2f,.94f,9),"first_Mai_approach");yield return SchoolUse(false);WalkCheck("Mai_before_witness_no_face",school.Talks>0&&!walkingStory.data.schoolFace);
  yield return SchoolGo(origin+new Vector3(0,.94f,5),"GD3_observation_position");
  var eye=player.GetComponentInChildren<Camera>();eye.transform.LookAt(school.puppet.gesturePart.position);RegionKey(null);var position=player.transform.position;float begin=Time.realtimeSinceStartup;
  while((!school.Leading||school.LeadSeconds<1.35f)&&Time.realtimeSinceStartup-begin<6)yield return null;
  var pose=school.puppet.gesturePart.localRotation;int leads=school.LeadCount;yield return new WaitForSeconds(.25f);
  WalkCheck("GD3_wait_keeps_pose_and_control",school.Leading&&school.LeadCount==leads&&Quaternion.Angle(pose,school.puppet.gesturePart.localRotation)<1&&Vector3.Distance(position,player.transform.position)<.02f&&player.enabled&&!walkingStory.data.schoolWitnessed,new JObject{["lead_elapsed_s"]=school.LeadSeconds});
  yield return WalkPress(Key.D);yield return new WaitForSeconds(.1f);WalkCheck("GD3_matching_response_after_lead",walkingStory.data.schoolWitnessed&&school.LastResponseDelay>=1&&school.LastResponseDelay<2,new JObject{["response_delay_s"]=school.LastResponseDelay});
  yield return SchoolGo(origin+new Vector3(1.2f,.94f,9),"second_Mai_approach");eye.transform.LookAt(school.face.ghostHead.position);yield return CapturePlayerImage(Path.GetFullPath(folder+"/Mai_before.png"));yield return SchoolUse(false);yield return new WaitForSeconds(.15f);
  WalkCheck("Mai_dialogue_and_face",walkingStory.Dialogue.Contains("Cô Mai")&&walkingStory.data.schoolFace&&school.face.Revealed&&school.face.SawIntermediateAlpha&&school.face.LastFadeSeconds>=.075f&&school.face.LastFadeSeconds<.15f);
  yield return CapturePlayerImage(Path.GetFullPath(folder+"/Mai_after.png"));yield return SchoolUse(false);WalkCheck("Mai_face_once",school.face.RevealCount==1);
  int awareness=walkingStory.data.awareness;yield return SchoolGo(origin+new Vector3(4.5f,.94f,11),"chair_by_window_approach");yield return new WaitForSeconds(.25f);var rotation=school.chair.localRotation;
  WalkCheck("chair_optional_trace",walkingStory.data.schoolTrace&&walkingStory.data.awareness==awareness+1&&SchoolModelBounds(school.chair).size.y>.8f&&school.chair.GetComponentsInChildren<Renderer>().Length>=7);eye.transform.LookAt(SchoolModelBounds(school.chair).center);yield return CapturePlayerImage(Path.GetFullPath(folder+"/chair_after.png"));yield return new WaitForSeconds(.3f);WalkCheck("chair_not_counted_twice",walkingStory.data.awareness==awareness+1);
  yield return WalkPress(Key.F5);var saved=player.transform.position;yield return SchoolGo(saved+Vector3.back*.8f,"move_after_school_save");yield return WalkPress(Key.F6);yield return new WaitForSeconds(.25f);lookController.enabled=false;
  WalkCheck("school_F5_F6",Vector3.Distance(saved,player.transform.position)<.15f&&walkingStory.data.schoolTrace&&walkingStory.data.schoolFace&&walkingStory.data.schoolWitnessed&&Quaternion.Angle(rotation,school.chair.localRotation)<.1f&&school.face.Revealed);
  File.Copy(save,Path.GetFullPath(folder+"/verified-school-save.json"));
  yield return WalkPress(Key.Escape);WalkCheck("school_pause",walkingStory.Paused&&Time.timeScale==0);yield return WalkPress(Key.Escape);lookController.enabled=false;WalkCheck("school_resume",!walkingStory.Paused&&Time.timeScale==1);
  yield return SchoolGo(origin+new Vector3(0,.94f,10),"return_to_center_aisle");foreach(float z in new[]{5,1,-3,-7})yield return SchoolGo(origin+new Vector3(0,.94f,z),"school_return_"+z);
  yield return SchoolUse(true);WalkCheck("school_returns_hub_next_slope",walkingStory.data.region==-1&&walkingStory.data.nextRegion==3&&!school.environment.activeSelf);
  // Independent bypass branch from the retained v36 save, through runtime Load.
  string bypass=Path.GetFullPath(folder+"/bypass-save.json");File.Copy(seed,bypass);walkingStory.SavePathOverride=bypass;walkingStory.Load();yield return new WaitForSeconds(.2f);lookController.enabled=false;
  yield return SchoolGo(new Vector3(-35,.94f,-34.6f),"bypass_hub_to_bed");yield return WalkUse(TuongTacHoCho.Kind.Bed);yield return new WaitForSeconds(.2f);lookController.enabled=false;yield return SchoolUse(true);WalkCheck("skip_chair_and_Mai_still_progresses",walkingStory.data.region==-1&&walkingStory.data.nextRegion==3&&!walkingStory.data.schoolTrace&&!walkingStory.data.schoolFace);
  RegionKey(null);yield return null;
  File.WriteAllText(folder+"/result.json",new JObject{["status"]=results.All(r=>(string)r["status"]=="PASS")&&regionalPenetrations==0&&regionalMissingSupport==0?"PASS":"EXECUTED_REVIEW_FAILURES",["scene"]=gameObject.scene.path,["input"]="Virtual Input System actions; E actual raycast; OS NOT_VERIFIED",["seed_evidence"]=previous.FullName,["fixture_teleports_during_traversal"]=0,["floor_penetration_frames"]=regionalPenetrations,["missing_support_frames"]=regionalMissingSupport,["results"]=results}.ToString());frames.Dispose();frames=null;complete=true;CleanupRegionalInput();
#if UNITY_EDITOR
  UnityEditor.EditorApplication.isPlaying=false;
#endif
 }
}

#endif
