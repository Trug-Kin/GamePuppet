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
 public string journeySuite;
 int peakEffects;
 IEnumerator PrepareJourneyCheck(string name)
 {
  previousBackground=Application.runInBackground;Application.runInBackground=true;player=GetComponent<DieuKhienDiChuyen>();cc=player.controller;player.DisableTestInputHook();walkingStory=FindAnyObjectByType<DieuPhoiHanhTrinh>();school=walkingStory.schoolChapter;
  if(string.IsNullOrEmpty(VerificationOutput))VerificationOutput="Docs/Reviews/journey_v39";
  folder=VerificationFolder(name);Directory.CreateDirectory(folder);walkingStory.SavePathOverride=Path.GetFullPath(folder+"/test-save.json");
  frames=new StreamWriter(folder+"/frames.csv"){AutoFlush=true};frames.WriteLine("case,phase,time,dt,x,y,z,foot_y,controller_grounded,ground_check,input_x,input_y,cc_vx,cc_vy,cc_vz,top_support_y,support_names,vertical_speed,move_flags,move_flags_copy,grounded_after_move,frame_contacts");
  lookController=player.GetComponent<DieuKhienGocNhin>();previousLookEnabled=lookController.enabled;lookController.enabled=false;contacts=gameObject.AddComponent<C3PlayContacts>();
  previousKeyboard=Keyboard.current;previousMouse=Mouse.current;previousInputBackground=InputSystem.settings.backgroundBehavior;previousEditorInputBehavior=InputSystem.settings.editorInputBehaviorInPlayMode;InputSystem.settings.backgroundBehavior=InputSettings.BackgroundBehavior.IgnoreFocus;InputSystem.settings.editorInputBehaviorInPlayMode=InputSettings.EditorInputBehaviorInPlayMode.AllDeviceInputAlwaysGoesToGameView;regionalInputSettingsChanged=true;regionKeyboard=InputSystem.AddDevice<Keyboard>("JourneyCheckKeyboard");regionMouse=InputSystem.AddDevice<Mouse>("JourneyCheckMouse");regionKeyboard.MakeCurrent();regionMouse.MakeCurrent();
  regionalRoute=Resources.Load<TextAsset>("VerificationV36/route");yield return null;
  if(name=="experience"||name.StartsWith("acceptance_"))
  {walkingStory.SetMenu(true);yield return new WaitForSecondsRealtime(1);yield return WalkPress(Key.Enter);lookController.enabled=false;WalkCheck("entry_New_Game",!walkingStory.MenuOpen&&walkingStory.data.region==-1&&walkingStory.data.nextRegion==0);}
 }
 void LoadJourneySeed(string name)
 {
  File.WriteAllText(walkingStory.SavePathOverride,Resources.Load<TextAsset>("VerificationV39/"+name).text);walkingStory.Load();lookController.enabled=false;
 }
 IEnumerator RunJourneyStability()
 {
  yield return PrepareJourneyCheck("stability");LoadJourneySeed("school_entry");yield return new WaitForSeconds(.3f);
  var origin=school.spawn.position-new Vector3(0,.94f,-7);
  yield return SchoolGo(origin+new Vector3(-10,.94f,-5),"planter_diagonal_graze_regression");yield return RegionalHold("planter_graze_hold");
  contacts.hits.Clear();yield return RegionalGo(origin+new Vector3(-6,.94f,-7),"planter_direct_push",3);
  WalkCheck("visible_planter_blocks_capsule",!regionalReached&&contacts.hits.Contains("Courtyard_Planter_Base"));yield return RegionalHold("planter_direct_hold");
  phase="jump_under_walkway_roof";float baseY=player.transform.position.y,maxY=baseY;contacts.hits.Clear();RegionKey(Key.Space);yield return null;yield return null;RegionKey(null);
  float until=Time.realtimeSinceStartup+2.5f;while(Time.realtimeSinceStartup<until){maxY=Mathf.Max(maxY,player.transform.position.y);yield return null;}
  WalkCheck("jump_against_planter_hits_visible_walkway_roof",maxY-baseY>1.5f&&maxY-baseY<1.75f&&contacts.hits.Contains("Covered_Link_Roof")&&cc.isGrounded,new JObject{["apex_m"]=maxY-baseY,["ceiling_contact"]=contacts.hits.Contains("Covered_Link_Roof")});
  yield return SchoolGo(origin+new Vector3(-10,.94f,-5),"planter_retreat");
  yield return SchoolGo(origin+new Vector3(0,.94f,-4),"open_courtyard_jump_position");phase="open_courtyard_jump";baseY=player.transform.position.y;maxY=baseY;RegionKey(Key.Space);yield return null;yield return null;RegionKey(null);until=Time.realtimeSinceStartup+2.5f;while(Time.realtimeSinceStartup<until){maxY=Mathf.Max(maxY,player.transform.position.y);yield return null;}
  WalkCheck("open_courtyard_jump_and_land",maxY-baseY>2.7f&&maxY-baseY<3.2f&&cc.isGrounded,new JObject{["apex_m"]=maxY-baseY});
  // High compliance delays lateral action only, using live actions and a measured onset.
  walkingStory.data.compliance=2;RegionKey(null);yield return new WaitForSeconds(.3f);float begin=Time.time,onset=-1;
  RegionKey(Key.D);until=Time.realtimeSinceStartup+.3f;while(Time.realtimeSinceStartup<until){if(onset<0&&player.LastMovementInput.x>.5f)onset=Time.time-begin;yield return null;}RegionKey(null);yield return new WaitForSeconds(.25f);
  WalkCheck("high_compliance_lateral_delay",onset>=.15f&&onset<=.25f,new JObject{["measured_onset_s"]=onset});walkingStory.data.compliance=0;
  walkingStory.data.compliance=1;RegionKey(Key.W);yield return new WaitForSeconds(.1f);RegionKey(null);yield return new WaitForSeconds(.1f);var stillPosition=player.transform.position;
  yield return new WaitForSeconds(4.2f);var eye=player.GetComponentInChildren<Camera>();
  WalkCheck("medium_compliance_idle_nudge_after_4s",Mathf.Abs(eye.transform.localPosition.x)>.001f&&Vector3.Distance(stillPosition,player.transform.position)<.01f&&player.enabled);walkingStory.data.compliance=0;
  yield return SchoolGo(origin+new Vector3(0,.94f,5),"held_GD3_save_position");RegionKey(null);eye.transform.LookAt(school.puppet.gesturePart.position);float leadDeadline=Time.realtimeSinceStartup+6;while((!school.Leading||school.LeadSeconds<1.3f)&&Time.realtimeSinceStartup<leadDeadline)yield return null;
  yield return WalkPress(Key.F5);float held=(float)JObject.Parse(File.ReadAllText(walkingStory.SavePathOverride))["schoolLeadElapsed"];yield return WalkPress(Key.F6);lookController.enabled=false;
  WalkCheck("held_GD3_save_load_pose",school.Leading&&school.LeadSeconds>=held&&school.LeadSeconds<held+.3f&&Quaternion.Angle(school.puppet.gesturePart.localRotation,Quaternion.identity)>10&&!walkingStory.data.schoolWitnessed);
  // Invalid input must leave the current world/state intact rather than half apply it.
  yield return WalkPress(Key.F5);string valid=File.ReadAllText(walkingStory.SavePathOverride);var state=walkingStory.data;var pose=player.transform.position;
  File.WriteAllText(walkingStory.SavePathOverride,"{\"version\":1,\"region\":2,\"traces\":[]}");walkingStory.Load();
  WalkCheck("malformed_save_rejected_atomically",ReferenceEquals(state,walkingStory.data)&&Vector3.Distance(pose,player.transform.position)<.01f);File.WriteAllText(walkingStory.SavePathOverride,valid);
  WalkCheck("controller_profile_and_declared_walk_speed",cc.height==1.8f&&cc.radius==.3f&&cc.stepOffset==.3f&&cc.slopeLimit==45&&cc.skinWidth==.03f&&cc.minMoveDistance==.001f&&player.speed==(walkingStory.polish?3.6f:12)&&player.gravity==-19.62f);
  if(walkingStory.completion)
  {
   int awareness=walkingStory.data.awareness,actions=walkingStory.data.interactions;
   walkingStory.data.baoTalks=walkingStory.data.haTalks=0;
   var lines=new System.Collections.Generic.HashSet<string>();
   for(int region=0;region<2;region++)for(int n=0;n<3;n++)lines.Add(walkingStory.completion.NextTalk(region));
   WalkCheck("v41_finite_distinct_Bao_Ha_dialogue",lines.Count==6&&walkingStory.completion.NextTalk(0)==null&&walkingStory.completion.NextTalk(1)==null);
   walkingStory.data.lanVisitMask=15;walkingStory.data.vyEncountered=true;
   yield return WalkPress(Key.F5);yield return WalkPress(Key.F6);lookController.enabled=false;
   WalkCheck("v41_optional_content_persists_without_axis_reward",walkingStory.data.baoTalks==3&&walkingStory.data.haTalks==3&&walkingStory.data.lanVisitMask==15&&walkingStory.data.vyEncountered&&walkingStory.data.awareness==awareness&&walkingStory.data.interactions==actions);
   WalkCheck("v41_Vy_absent_outside_market",!walkingStory.completion.VyVisible);
   WalkCheck("v41_triggered_sound_limit_includes_child_voices",peakEffects<=2,new JObject{["peak"]=peakEffects});
  }
  yield return FinishWalk();
 }
 IEnumerator RunEndingBranches()
 {
  yield return PrepareJourneyCheck("ending_branches");string seed=Resources.Load<TextAsset>("VerificationV39/school_completed").text;
  // Four declared boundary fixtures, derived from the actually executed school exit.
  // These are not presented as four complete fresh-start playthroughs.
  foreach(var branch in new[]{("A",2,1),("A2",2,2),("B",3,1),("C",3,2)})
  {
   var json=JObject.Parse(seed);json["awareness"]=branch.Item2;json["interactions"]=branch.Item3;json["actionPoints"]=branch.Item3==2?4:1;json["traces"]=new JArray(branch.Item2>=3,true);json["compliance"]=2;
   File.WriteAllText(walkingStory.SavePathOverride,json.ToString());walkingStory.Load();lookController.enabled=false;
   yield return SlopeGo(new Vector3(-35,.94f,-34.6f),branch.Item1+"_bed");yield return WalkUse(TuongTacHoCho.Kind.Bed);yield return new WaitForSeconds(.2f);
   var slope=walkingStory.slopeChapter;var origin=slope.spawn.position-new Vector3(0,.94f,1);
   for(int z=9;z<74;z+=8)yield return SlopeGo(origin+new Vector3(0,Mathf.Min(z,60)*.1f+.94f,z),branch.Item1+"_slope_"+z);
   yield return SlopeGo(origin+new Vector3(0,6.94f,74),branch.Item1+"_door");
   WalkCheck(branch.Item1+"_can_skip_vali_and_Tung",!walkingStory.data.slopeTrace&&walkingStory.data.slopeTalks==0&&!walkingStory.data.finalStarted);
   var door=slope.environment.GetComponentsInChildren<TuongTacDocV39>().First(i=>i.finalDoor);yield return SlopeUse(door);var eye=player.GetComponentInChildren<Camera>();eye.transform.LookAt(slope.puppet.dauTransform.position);
   yield return new WaitForSeconds(.4f);yield return WalkPress(Key.F5);float elapsed=(float)JObject.Parse(File.ReadAllText(walkingStory.SavePathOverride))["finalElapsed"];yield return WalkPress(Key.F6);lookController.enabled=false;eye.transform.LookAt(slope.puppet.dauTransform.position);
   WalkCheck(branch.Item1+"_final_load_once",walkingStory.data.finalStarts==1&&walkingStory.data.finalElapsed>=elapsed);
   float deadline=Time.realtimeSinceStartup+12;while(string.IsNullOrEmpty(walkingStory.data.ending)&&Time.realtimeSinceStartup<deadline)yield return null;
   WalkCheck(branch.Item1+"_ending_from_fixture_axes",walkingStory.data.ending==branch.Item1&&walkingStory.data.region==-1&&walkingStory.data.nextRegion==4&&walkingStory.data.finalStarts==1);
   for(int beat=1;beat<=3;beat++)yield return WalkPress(Key.E);
   yield return WalkPress(Key.F5);yield return WalkPress(Key.F6);lookController.enabled=false;
   WalkCheck(branch.Item1+"_ending_restores",walkingStory.data.ending==branch.Item1&&walkingStory.data.endingBeat==3&&!player.enabled);
   yield return CapturePlayerImage(Path.GetFullPath(folder+"/ending_"+branch.Item1+".png"));File.Copy(walkingStory.SavePathOverride,folder+"/ending_"+branch.Item1+"-save.json");
  }
  WalkCheck("sound_effects_at_most_two",peakEffects<=2,new JObject{["observed_peak"]=peakEffects});
  yield return FinishWalk();
 }
}

#endif
