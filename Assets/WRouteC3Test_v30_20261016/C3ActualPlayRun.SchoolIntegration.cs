#if UNITY_EDITOR || CON_ROI_BUILD_REVIEW
using System.Collections;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public sealed partial class C3ActualPlayRun
{
 IEnumerator SchoolCheckpoint(string label,int region,int next)
 {
  yield return WalkPress(Key.F5);var pos=player.transform.position;
  string snapshot=folder+"/"+label+"-save.json";File.Copy(folder+"/test-save.json",snapshot,true);
  yield return WalkPress(Key.F6);yield return new WaitForSeconds(.2f);lookController.enabled=false;
  WalkCheck(label,walkingStory.data.region==region&&walkingStory.data.nextRegion==next&&Vector3.Distance(pos,player.transform.position)<.15f);
 }
 IEnumerator ContinueSchoolIntegration()
 {
  school=walkingStory.schoolChapter;var origin=school.spawn.position-new Vector3(0,.94f,-7);
  yield return SchoolCheckpoint("before_school_transition",-1,2);
  yield return SchoolGo(new Vector3(-35,.94f,-34.6f),"hub_third_bed");yield return WalkUse(TuongTacHoCho.Kind.Bed);
  WalkCheck("normal_bed_enters_school",walkingStory.data.region==2&&school.environment.activeInHierarchy&&!walkingStory.lake.activeSelf&&!walkingStory.market.activeSelf&&!walkingStory.hub.activeSelf);
  yield return SchoolCheckpoint("after_school_transition",2,2);
  var old=walkingStory.puppets.Select(p=>p.transform.position).ToArray();yield return new WaitForSeconds(.7f);
  WalkCheck("GD1_GD2_inactive_in_school",walkingStory.puppets.All(p=>!p.gameObject.activeInHierarchy)&&walkingStory.puppets.Select((p,i)=>Vector3.Distance(p.transform.position,old[i])<.001f).All(x=>x)&&!school.puppet.enabled);
  var headBounds=SchoolModelBounds(school.puppet.dauTransform);var bodyBounds=school.puppet.GetComponentsInChildren<Renderer>().First(r=>r.name=="ConRoi_Body_Wood").bounds;
  WalkCheck("GD3_head_attached_to_body",headBounds.min.y-bodyBounds.max.y>=0&&headBounds.min.y-bodyBounds.max.y<.08f,new JObject{["head_body_gap_m"]=headBounds.min.y-bodyBounds.max.y});
  var eye=player.GetComponentInChildren<Camera>();
  // Explore the linked west wing using actual movement, return through the courtyard doorway.
  foreach(var p in new[]{new Vector3(-10,.94f,-5),new Vector3(-22,.94f,-5),new Vector3(-22,.94f,0),new Vector3(-22,.94f,10)})yield return SchoolGo(origin+p,"archive_approach_"+p.z);
  eye.transform.LookAt(origin+new Vector3(-27,1.5f,12.5f));yield return CapturePlayerImage(Path.GetFullPath(folder+"/school_archive.png"));
  foreach(int index in new[]{2,1,3,0})
  {
   var m=school.environment.GetComponentsInChildren<TuongTacTruongV37>().First(i=>i.memoryIndex==index);
   Vector3 approach=m.transform.position+Vector3.back*1.7f;approach.y=origin.y+.94f;
   if(index==0){yield return SchoolGo(origin+new Vector3(-22,.94f,0),"archive_return_door");yield return SchoolGo(origin+new Vector3(-22,.94f,-5),"archive_return_court");}
   yield return SchoolGo(approach,"memory_approach_"+index);eye.transform.LookAt(m.transform.position);yield return null;yield return WalkPress(Key.E);
   WalkCheck("memory_E_"+index,(walkingStory.data.schoolReadMask&(1<<index))!=0);
  }
  eye.transform.LookAt(origin+new Vector3(-22,1.5f,8));yield return CapturePlayerImage(Path.GetFullPath(folder+"/school_west_wing.png"));
  foreach(var p in new[]{new Vector3(-10,.94f,-5),new Vector3(0,.94f,-5)})yield return SchoolGo(origin+p,"school_classroom_"+p.z);
  var notice=school.environment.GetComponentsInChildren<TuongTacTruongV37>().First(i=>i.memoryIndex==4);yield return SchoolGo(origin+new Vector3(-4.8f,.94f,-1.9f),"school_notice_approach");eye.transform.LookAt(notice.transform.position);yield return null;yield return WalkPress(Key.E);WalkCheck("memory_E_4",(walkingStory.data.schoolReadMask&16)!=0);
  foreach(var p in new[]{new Vector3(0,.94f,-3),new Vector3(0,.94f,1),new Vector3(0,.94f,5),new Vector3(1.2f,.94f,9)})yield return SchoolGo(origin+p,"school_classroom_"+p.z);
  yield return SchoolUse(false);WalkCheck("Mai_before_GD3_face_hidden",school.Talks>0&&!walkingStory.data.schoolFace);
  yield return SchoolGo(origin+new Vector3(0,.94f,5),"school_observe_GD3");eye.transform.LookAt(school.puppet.gesturePart.position);RegionKey(null);
  float begin=Time.realtimeSinceStartup;
  while((!school.Leading||school.LeadSeconds<1.35f)&&Time.realtimeSinceStartup-begin<6)yield return null;
  int leads=school.LeadCount;var pose=school.puppet.gesturePart.localRotation;yield return new WaitForSeconds(.25f);
  WalkCheck("GD3_holds_without_forcing_input",school.Leading&&school.LeadCount==leads&&Quaternion.Angle(pose,school.puppet.gesturePart.localRotation)<1&&player.enabled&&!walkingStory.data.schoolWitnessed);
  yield return WalkPress(Key.D);yield return new WaitForSeconds(.1f);
  WalkCheck("GD3_visible_matching_response",walkingStory.data.schoolWitnessed&&school.LastResponseDelay>=1&&school.LastResponseDelay<2,new JObject{["delay_s"]=school.LastResponseDelay});
  yield return SchoolGo(origin+new Vector3(1.2f,.94f,9),"school_Mai_after_GD3");eye.transform.LookAt(school.face.ghostHead.position);yield return CapturePlayerImage(Path.GetFullPath(folder+"/Mai_before.png"));
  int woodBefore=walkingStory.WoodEffectsPlayed;yield return SchoolUse(false);yield return new WaitForSeconds(.15f);
  WalkCheck("Mai_face_08s_and_dialogue",walkingStory.data.schoolFace&&school.face.Revealed&&school.face.SawIntermediateAlpha&&school.face.LastFadeSeconds>=.075f&&school.face.LastFadeSeconds<.15f&&walkingStory.Dialogue.Contains("Cô Mai"));
  WalkCheck("Mai_face_wood_sync",walkingStory.WoodEffectsPlayed==woodBefore+1);
  yield return CapturePlayerImage(Path.GetFullPath(folder+"/Mai_after.png"));yield return SchoolUse(false);WalkCheck("Mai_face_once",school.face.RevealCount==1);
  int aware=walkingStory.data.awareness;yield return SchoolGo(origin+new Vector3(4.5f,.94f,11),"school_chair_trace");yield return new WaitForSeconds(.3f);
  WalkCheck("chair_changes_once",walkingStory.data.schoolTrace&&walkingStory.data.awareness==aware+1);
  var lesson=school.environment.GetComponentsInChildren<TuongTacTruongV37>().First(i=>i.memoryIndex==5);yield return SchoolGo(origin+new Vector3(-4,.94f,12.1f),"school_lesson_plan_approach");eye.transform.LookAt(lesson.transform.position);yield return null;yield return WalkPress(Key.E);WalkCheck("memory_E_5",(walkingStory.data.schoolReadMask&32)!=0);
  yield return SchoolCheckpoint("school_progress_save_load",2,2);
  WalkCheck("school_progress_restored",walkingStory.data.schoolTrace&&walkingStory.data.schoolFace&&walkingStory.data.schoolWitnessed&&walkingStory.data.schoolReadMask==63&&school.face.Revealed);
  yield return WalkPress(Key.Escape);WalkCheck("school_pause",walkingStory.Paused&&Time.timeScale==0);yield return WalkPress(Key.Escape);lookController.enabled=false;
  yield return SchoolGo(origin+new Vector3(0,.94f,10),"school_exit_to_aisle");
  foreach(float z in new[]{5f,1f,-3f,-7f})yield return SchoolGo(origin+new Vector3(0,.94f,z),"school_exit_"+z);
  yield return SchoolCheckpoint("before_school_wake",2,2);yield return SchoolUse(true);
  WalkCheck("school_completion_returns_hub",walkingStory.data.region==-1&&walkingStory.data.nextRegion==3&&!school.environment.activeSelf);
  yield return SchoolCheckpoint("after_school_wake",-1,3);
  WalkCheck("all_dream_phases_off_in_hub",walkingStory.puppets.All(p=>!p.gameObject.activeInHierarchy)&&!school.puppet.gameObject.activeInHierarchy&&!school.Leading);
 }
}

#endif
