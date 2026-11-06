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
 // Fresh-start runs: no save seed, axis assignment, discovery override or fixture placement.
 IEnumerator RunAcceptanceV47()
 {
  string target=journeySuite.Substring("acceptance-".Length);
  yield return PrepareJourneyCheck("acceptance_"+target);
  bool highAwareness=target=="B"||target=="C",highAction=target=="A2"||target=="C";
  var route=JObject.Parse(regionalRoute.text)["route"].Select(P).ToArray();var eye=player.GetComponentInChildren<Camera>();
  WalkCheck("fresh_start_zero_axes",walkingStory.data.region==-1&&walkingStory.data.nextRegion==0&&walkingStory.data.awareness==0&&walkingStory.data.interactions==0);
  yield return WalkPress(Key.Escape);var pos=player.transform.position;
  yield return new WaitForSecondsRealtime(.6f);WalkCheck("pause_freezes_and_releases_cursor",walkingStory.Paused&&Time.timeScale==0&&Vector3.Distance(pos,player.transform.position)<.001f&&Cursor.lockState==CursorLockMode.None);
  yield return WalkPress(Key.Escape);lookController.enabled=false;WalkCheck("resume_reenables_actions",!walkingStory.Paused&&player.enabled&&player.GameplayActionsEnabled);
  if(highAction)
  {
   yield return WalkPress(Key.P);yield return WalkPress(Key.Digit1);yield return WalkPress(Key.Digit2);
   yield return WalkPress(Key.Digit1);yield return WalkPress(Key.Digit2);
   WalkCheck("two_distinct_actions_no_farm",walkingStory.data.interactions==2&&walkingStory.data.actionPoints==4);
   yield return WalkPress(Key.P);lookController.enabled=false;
  }
  yield return RegionalGo(new Vector3(-35,.94f,-34.6f),"acceptance_bed_lake",8);yield return WalkUse(TuongTacHoCho.Kind.Bed);
  yield return WalkRoute(route,0,3,"acceptance_lake_first_path");
  var bao=walkingStory.ghosts[0].position;yield return RegionalGo(bao+new Vector3(-1.1f,0,-.9f),"acceptance_Bao",8);yield return WalkUse(TuongTacHoCho.Kind.Bao);
  yield return RegionalGo(route[3],"acceptance_lake_return",8);yield return WalkRoute(route,4,31,"acceptance_lake_exit");yield return WalkUse(TuongTacHoCho.Kind.Wake);
  yield return SchoolCheckpoint("acceptance_hub_after_lake",-1,1);
  yield return RegionalGo(new Vector3(-35,.94f,-34.6f),"acceptance_bed_market",8);yield return WalkUse(TuongTacHoCho.Kind.Bed);
  yield return WalkRoute(route,165,route.Length-1,"acceptance_market_route");
  if(highAwareness)
  {
   var ha=walkingStory.ghosts[1].position;yield return RegionalGo(ha+new Vector3(-1.3f,0,-1.7f),"acceptance_Ha",10);yield return WalkUse(TuongTacHoCho.Kind.Ha);
   yield return RegionalGo(walkingStory.traces[1].position+new Vector3(-1.7f,0,-1.1f),"acceptance_books",10);yield return new WaitForSeconds(.2f);
   WalkCheck("books_found_by_proximity",walkingStory.data.traces[1]);
  }
  yield return RegionalGo(route.Last(),"acceptance_market_exit",12);yield return WalkUse(TuongTacHoCho.Kind.Wake);
  yield return SchoolCheckpoint("acceptance_hub_after_market",-1,2);
  yield return RegionalGo(new Vector3(-35,.94f,-34.6f),"acceptance_bed_school",8);yield return WalkUse(TuongTacHoCho.Kind.Bed);
  var origin=school.spawn.position-new Vector3(0,.94f,-7);
  foreach(var p in new[]{new Vector3(0,.94f,1),new Vector3(0,.94f,5)})yield return SchoolGo(origin+p,"acceptance_classroom");
  RegionKey(null);eye.transform.LookAt(school.puppet.gesturePart.position);float deadline=Time.realtimeSinceStartup+6;
  while((!school.Leading||school.LeadSeconds<1.35f)&&Time.realtimeSinceStartup<deadline)yield return null;
  yield return new WaitForSeconds(.4f);WalkCheck("GD3_no_input_no_failure",school.Leading&&walkingStory.data.region==2&&!walkingStory.data.schoolWitnessed);
  yield return WalkPress(Key.D);WalkCheck("GD3_voluntary_response",walkingStory.data.schoolWitnessed);
  yield return SchoolGo(origin+new Vector3(1.2f,.94f,9),"acceptance_Mai");yield return SchoolUse(false);
  if(highAwareness){yield return SchoolGo(origin+new Vector3(4.5f,.94f,11),"acceptance_chair");yield return new WaitForSeconds(.2f);WalkCheck("chair_found_by_proximity",walkingStory.data.schoolTrace);}
  foreach(var p in new[]{new Vector3(0,.94f,5),new Vector3(0,.94f,1),new Vector3(0,.94f,-3),new Vector3(0,.94f,-7)})yield return SchoolGo(origin+p,"acceptance_school_exit");
  yield return SchoolUse(true);yield return SchoolCheckpoint("acceptance_hub_after_school",-1,3);
  yield return RegionalGo(new Vector3(-35,.94f,-34.6f),"acceptance_bed_slope",8);yield return WalkUse(TuongTacHoCho.Kind.Bed);
  var slope=walkingStory.slopeChapter;origin=slope.spawn.position-new Vector3(0,.94f,1);
  for(int z=8;z<=72;z+=8)
  {
   yield return SlopeGo(origin+new Vector3(0,Mathf.Min(z,60)*.1f+.94f,z),"acceptance_slope_"+z);
   if(z==40&&highAwareness){yield return SlopeGo(origin+new Vector3(1.3f,5.34f,44),"acceptance_vali");yield return new WaitForSeconds(.2f);WalkCheck("vali_found_by_proximity",walkingStory.data.slopeTrace);}
  }
  yield return SlopeGo(origin+new Vector3(0,6.94f,74),"acceptance_final_door");
  WalkCheck("natural_axes_before_final",(walkingStory.data.awareness>=3)==highAwareness&&(walkingStory.data.interactions>=2)==highAction,new JObject{["awareness"]=walkingStory.data.awareness,["interactions"]=walkingStory.data.interactions,["weighted_action_points"]=walkingStory.data.actionPoints});
  yield return SlopeUse(slope.environment.GetComponentsInChildren<TuongTacDocV39>().First(i=>i.finalDoor));eye.transform.LookAt(slope.puppet.dauTransform.position);
  yield return new WaitForSeconds(.7f);yield return WalkPress(Key.F5);yield return WalkPress(Key.F6);lookController.enabled=false;eye.transform.LookAt(slope.puppet.dauTransform.position);
  deadline=Time.realtimeSinceStartup+15;while(walkingStory.data.region==3&&Time.realtimeSinceStartup<deadline)yield return null;
  WalkCheck("natural_ending_"+target,walkingStory.data.ending==target&&walkingStory.data.finalStarts==1&&walkingStory.data.nextRegion==4&&!player.enabled);
  for(int beat=1;beat<=3;beat++){yield return WalkPress(Key.E);WalkCheck("ending_beat_"+beat,walkingStory.data.endingBeat==beat);}
  yield return WalkPress(Key.F5);string valid=File.ReadAllText(walkingStory.SavePathOverride);yield return WalkPress(Key.F6);lookController.enabled=false;
  WalkCheck("ending_save_load",walkingStory.data.ending==target&&walkingStory.data.endingBeat==3&&walkingStory.data.finalStarts==1);
  File.WriteAllText(Path.Combine(folder,"natural-ending-save.json"),valid);yield return CapturePlayerImage(Path.Combine(folder,"ending_"+target+".png"));
  yield return WalkPress(Key.R);lookController.enabled=false;WalkCheck("replay_resets_axes_and_optional_flags",walkingStory.data.region==-1&&walkingStory.data.nextRegion==0&&walkingStory.data.awareness==0&&walkingStory.data.interactions==0&&!walkingStory.data.vyEncountered&&!walkingStory.data.finalStarted);
  File.WriteAllText(Path.Combine(VerificationOutput,"acceptance-method.json"),new JObject{["target"]=target,["fresh_start"]=true,["fixture_teleports"]=0,["axis_overrides"]=0,["input"]="Virtual Input System -> live actions/E raycast; scripted camera and route; not physical-input or human playtest"}.ToString());
  yield return FinishWalk();
 }
}

#endif
