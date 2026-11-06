#if UNITY_EDITOR || CON_ROI_BUILD_REVIEW
using System.Collections;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public sealed partial class C3ActualPlayRun
{
 IEnumerator SlopeGo(Vector3 p,string name)
 {yield return RegionalGo(p,name,12);WalkCheck(name,regionalReached);}
 IEnumerator SlopeUse(TuongTacDocV39 target)
 {
  var eye=player.GetComponentInChildren<Camera>();eye.transform.LookAt(target.GetComponent<Collider>().bounds.center);yield return null;yield return WalkPress(Key.E);lookController.enabled=false;
 }
 IEnumerator ContinueSlopeIntegration()
 {
  var slope=walkingStory.slopeChapter;var origin=slope.spawn.position-new Vector3(0,.94f,1);
  yield return SchoolCheckpoint("before_slope_transition",-1,3);
  int interactions=walkingStory.data.interactions;
  yield return WalkPress(Key.P);yield return WalkPress(Key.Digit1);yield return WalkPress(Key.Digit2);
  WalkCheck("real_world_actions_before_slope",walkingStory.data.interactions==interactions+2&&walkingStory.data.actionPoints==4);yield return WalkPress(Key.P);lookController.enabled=false;
  yield return SlopeGo(new Vector3(-35,.94f,-34.6f),"hub_fourth_bed");yield return WalkUse(TuongTacHoCho.Kind.Bed);yield return new WaitForSeconds(.25f);
  WalkCheck("normal_bed_enters_slope_without_GD4",walkingStory.data.region==3&&slope.environment.activeSelf&&!school.environment.activeSelf&&walkingStory.puppets.All(p=>!p.gameObject.activeInHierarchy)&&!slope.puppet.gameObject.activeSelf&&!walkingStory.data.finalStarted);
  yield return SchoolCheckpoint("after_slope_transition",3,3);
  var eye=player.GetComponentInChildren<Camera>();
  for(int z=8;z<=40;z+=8)
  {
   yield return SlopeGo(origin+new Vector3(0,z*.1f+.94f,z),"slope_climb_"+z);
   if(z==16){var memory=slope.environment.GetComponentsInChildren<TuongTacDocV39>().First(i=>i.memoryIndex==0);yield return SlopeGo(origin+new Vector3(-2.5f,2.57f,16.3f),"slope_ticket_approach");yield return SlopeUse(memory);WalkCheck("slope_ticket_E",(walkingStory.data.slopeReadMask&1)!=0);yield return SlopeGo(origin+new Vector3(0,2.54f,16),"slope_ticket_return");}
  }
  int awareness=walkingStory.data.awareness;yield return SlopeGo(origin+new Vector3(1.3f,5.34f,44),"slope_suitcase_approach");yield return new WaitForSeconds(.25f);
  WalkCheck("suitcase_optional_once",walkingStory.data.slopeTrace&&walkingStory.data.awareness==awareness+1);
  eye.transform.LookAt(slope.suitcase.position);yield return CapturePlayerImage(Path.GetFullPath(folder+"/slope_suitcase.png"));
  yield return SchoolCheckpoint("slope_trace_save_load",3,3);WalkCheck("suitcase_restore_without_duplicate",walkingStory.data.awareness==awareness+1&&walkingStory.data.slopeTrace&&!slope.puppet.gameObject.activeSelf);
  yield return SlopeGo(origin+new Vector3(0,5.74f,48),"slope_back_to_center");
  var address=slope.environment.GetComponentsInChildren<TuongTacDocV39>().First(i=>i.memoryIndex==1);yield return SlopeGo(origin+new Vector3(-2.5f,6.17f,52.3f),"slope_address_approach");yield return SlopeUse(address);WalkCheck("slope_address_E",walkingStory.data.slopeReadMask==3);yield return SchoolCheckpoint("slope_memories_save_load",3,3);WalkCheck("slope_memories_restored",walkingStory.data.slopeReadMask==3);
  foreach(int z in new[]{56,64,71})yield return SlopeGo(origin+new Vector3(0,Mathf.Min(z,60)*.1f+.94f,z),"slope_upper_"+z);
  var talk=slope.environment.GetComponentsInChildren<TuongTacDocV39>().First(i=>!i.finalDoor&&i.memoryIndex<0);
  yield return SlopeUse(talk);WalkCheck("Tung_dialogue_before_final_face_hidden",walkingStory.Dialogue.Contains("Anh Tùng")&&!walkingStory.data.slopeFace&&!walkingStory.data.finalStarted);
  var bounds=SchoolModelBounds(slope.ghost);WalkCheck("Tung_upright_real_scale",bounds.size.y>1.7f&&bounds.size.y<1.85f&&bounds.min.y>5.98f&&slope.face.ghostHead.position.y>7.2f&&slope.face.ghostHead.position.y<7.8f,new JObject{["bounds"]=bounds.ToString(),["head"]=slope.face.ghostHead.position.ToString()});
  eye.transform.LookAt(slope.face.ghostHead.position);yield return CapturePlayerImage(Path.GetFullPath(folder+"/Tung_before_final.png"));
  yield return SlopeGo(origin+new Vector3(0,6.94f,74),"slope_final_door_approach");yield return SchoolCheckpoint("before_GD4",3,3);
  var door=slope.environment.GetComponentsInChildren<TuongTacDocV39>().First(i=>i.finalDoor);yield return SlopeUse(door);
  WalkCheck("GD4_starts_once_at_final_door",walkingStory.data.finalStarted&&walkingStory.data.finalStarts==1&&slope.puppet.gameObject.activeInHierarchy&&!slope.puppet.enabled);
  var puppetHead=SchoolModelBounds(slope.puppet.dauTransform);var puppetBody=slope.puppet.GetComponentsInChildren<Renderer>().First(r=>r.name=="ConRoi_Body_Wood").bounds;
  WalkCheck("GD4_head_attached_to_body",Mathf.Abs(puppetHead.center.x-puppetBody.center.x)<.05f&&puppetHead.min.y-puppetBody.max.y>-.05f&&puppetHead.min.y-puppetBody.max.y<.08f,new JObject{["head_body_gap_m"]=puppetHead.min.y-puppetBody.max.y});
  var fixedPosition=slope.puppet.transform.position;eye.transform.LookAt(slope.puppet.dauTransform.position);yield return new WaitForSeconds(.6f);
  WalkCheck("GD4_stationary_and_witnessed",Vector3.Distance(slope.puppet.transform.position,fixedPosition)<.001f&&walkingStory.data.slopeWitnessed);
  yield return WalkPress(Key.F5);float elapsed=(float)JObject.Parse(File.ReadAllText(walkingStory.SavePathOverride))["finalElapsed"];yield return WalkPress(Key.F6);yield return new WaitForSeconds(.2f);lookController.enabled=false;
  WalkCheck("GD4_save_load_resumes_same_encounter",walkingStory.data.finalStarts==1&&walkingStory.data.finalStarted&&walkingStory.data.finalElapsed>=elapsed&&walkingStory.data.finalElapsed<elapsed+.5f);
  yield return SlopeUse(door);WalkCheck("GD4_door_cannot_restart",walkingStory.data.finalStarts==1&&walkingStory.data.finalElapsed>=elapsed);
  int woodBefore=walkingStory.WoodEffectsPlayed;eye.transform.LookAt(slope.face.ghostHead.position);yield return new WaitForSeconds(.2f);
  WalkCheck("Tung_face_after_GD4",walkingStory.data.slopeFace&&slope.face.Revealed&&slope.face.RevealCount==1);
  WalkCheck("Tung_face_fade_and_wood_sync",slope.face.SawIntermediateAlpha&&slope.face.LastFadeSeconds>=.075f&&slope.face.LastFadeSeconds<.15f&&walkingStory.WoodEffectsPlayed==woodBefore+1);
  yield return CapturePlayerImage(Path.GetFullPath(folder+"/GD4_Tung_face.png"));
  while(walkingStory.data.region==3&&walkingStory.data.finalElapsed<9.2f)yield return null;
  var fifth=slope.puppet.GetComponentsInChildren<Renderer>().First(r=>r.name.Contains("String_5_"));var block=new MaterialPropertyBlock();fifth.GetPropertyBlock(block);var endingColor=block.GetColor("_BaseColor");
  WalkCheck("GD4_fifth_string_changes_at_ending",walkingStory.data.region==3&&endingColor.r>.8f&&endingColor.g>.7f&&endingColor.b<.5f,new JObject{["color"]=endingColor.ToString()});
  eye.transform.LookAt(slope.puppet.dauTransform.position);yield return CapturePlayerImage(Path.GetFullPath(folder+"/GD4_all_strings.png"));
  float deadline=Time.realtimeSinceStartup+13;while(string.IsNullOrEmpty(walkingStory.data.ending)&&Time.realtimeSinceStartup<deadline)yield return null;
  WalkCheck("ending_from_actual_two_axes",walkingStory.data.ending==DieuPhoiHanhTrinh.ResolveEnding(walkingStory.data.awareness,walkingStory.data.interactions)&&walkingStory.data.ending=="C"&&walkingStory.data.region==-1&&walkingStory.data.nextRegion==4);
  WalkCheck("all_phases_off_after_ending",!slope.environment.activeSelf&&!school.environment.activeSelf&&walkingStory.puppets.All(p=>!p.gameObject.activeInHierarchy)&&!player.enabled);
  for(int beat=1;beat<=3;beat++){yield return WalkPress(Key.E);WalkCheck("ending_narrative_beat_"+beat,walkingStory.data.endingBeat==beat);}
  yield return SchoolCheckpoint("ending_save_load",-1,4);WalkCheck("ending_beat_restored",walkingStory.data.endingBeat==3&&walkingStory.data.ending=="C"&&walkingStory.data.finalStarts==1);
  yield return CapturePlayerImage(Path.GetFullPath(folder+"/ending_C.png"));
  WalkCheck("four_ending_boundary_matrix",DieuPhoiHanhTrinh.ResolveEnding(2,1)=="A"&&DieuPhoiHanhTrinh.ResolveEnding(2,2)=="A2"&&DieuPhoiHanhTrinh.ResolveEnding(3,1)=="B"&&DieuPhoiHanhTrinh.ResolveEnding(3,2)=="C");
 }
}

#endif
