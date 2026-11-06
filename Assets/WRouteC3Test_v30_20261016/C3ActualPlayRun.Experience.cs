#if UNITY_EDITOR || CON_ROI_BUILD_REVIEW
using System;
using System.Collections;
using System.Linq;
using System.IO;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public sealed partial class C3ActualPlayRun
{
 IEnumerator ExperiencePhaseObservation(Camera eye)
 {
  if(!walkingStory.completion)yield break;
  var puppet=walkingStory.puppets.First(p=>p.gameObject.activeInHierarchy);
  walkingStory.polish.Event("phase_observation_begin",walkingStory.data.region==0?"GD1":"GD2");
  RegionKey(null);eye.transform.LookAt(puppet.dauTransform.position);yield return new WaitForSeconds(1);
  foreach(var key in new[]{Key.D,Key.A})
  {
   RegionKey(key);float until=Time.realtimeSinceStartup+.65f;
   while(Time.realtimeSinceStartup<until){eye.transform.LookAt(puppet.dauTransform.position);yield return null;}
   RegionKey(null);until=Time.realtimeSinceStartup+1.25f;
   while(Time.realtimeSinceStartup<until){eye.transform.LookAt(puppet.dauTransform.position);yield return null;}
  }
  walkingStory.polish.Event("phase_observation_end",walkingStory.data.region.ToString());
 }
 IEnumerator ExperienceGarden(Camera eye)
 {
  if(!walkingStory.completion)yield break;
  yield return RegionalGo(new Vector3(-35,.94f,-36),"Lan_window",10);
  var window=walkingStory.hub.GetComponentInChildren<TuongTacVuonV41>();eye.transform.LookAt(window.transform.position);
  yield return WalkPress(Key.E);yield return ExperienceRead(walkingStory.Dialogue);
  WalkCheck("Lan_optional_visit_"+walkingStory.data.nextRegion,(walkingStory.data.lanVisitMask&(1<<walkingStory.data.nextRegion))!=0);
 }
 // A separate, openly scripted exploration route. Reading pauses belong to this
 // input driver, never to game progression. No fixtures, score overrides or teleports.
 IEnumerator ExperienceRead(string text)
 {
  RegionKey(null);float seconds=Mathf.Max(4,text.Split(new[]{' ','\n'},StringSplitOptions.RemoveEmptyEntries).Length/2.8f);
  walkingStory.polish.Event("automation_reading_schedule",seconds.ToString("F2",System.Globalization.CultureInfo.InvariantCulture)+" seconds; 168 words/min; not a human playtest");
  yield return new WaitForSeconds(seconds);walkingStory.polish.DismissReading();
 }
 IEnumerator ExperienceTalk(TuongTacHoCho.Kind kind)
 {
  yield return WalkUse(kind);yield return ExperienceRead(walkingStory.Dialogue);
 }
 IEnumerator ExperienceSceneObservation(Camera eye,Transform actor,float seconds)
 {
  // Capture the whole body/prop action before the face-to-face conversation.
  // This pause belongs to the optional observer route, never to game rules.
  yield return RegionalGo(actor.position+new Vector3(-2.4f,0,-3.2f),"scene_observation_"+actor.name,12);
  RegionKey(null);eye.transform.LookAt(actor.GetComponent<HienGuongMatTheoKhu>().ghostHead.position+Vector3.down*.4f);
  walkingStory.polish.Event("automation_scene_observation",actor.name+" / "+seconds+" seconds; optional camera framing, not a mandatory game wait");
  yield return new WaitForSeconds(seconds);
 }
 IEnumerator ExperienceDocument(DaoCuKeChuyenV40 prop,Vector3 approach)
 {
  yield return RegionalGo(approach,"document_"+prop.heading,20);var eye=player.GetComponentInChildren<Camera>();eye.transform.LookAt(prop.transform.position);yield return null;yield return WalkPress(Key.E);
  var hits=Physics.RaycastAll(eye.transform.position,eye.transform.forward,3,~0,QueryTriggerInteraction.Ignore).OrderBy(h=>h.distance);
  WalkCheck("read_"+prop.heading,walkingStory.polish.IsReading,new JObject{["ray_hits"]=new JArray(hits.Select(h=>h.collider.name+":"+h.distance.ToString("F3"))),["eye"]=eye.transform.position.ToString("F4"),["prop"]=prop.transform.position.ToString("F4")});
  if(walkingStory.polish.IsReading)yield return ExperienceRead(prop.text);
 }
 IEnumerator RunExperienceWalkthrough()
 {
  yield return PrepareJourneyCheck("experience");
  var eye=player.GetComponentInChildren<Camera>();
  File.WriteAllText(Path.Combine(VerificationOutput,"experience-method.json"),new JObject{["input"]="Virtual Input System keyboard -> live actions; E raycast; automated camera steering",["time_scale"]=1,["reading_schedule_words_per_minute"]=168,["driver_changes_game_waits"]=false,["dream_room_transition_seconds"]=walkingStory.GetComponent<PhongTrongMoV47>()?3:0,["teleports"]="Only game sleep/wake transitions",["human_playtest"]=false,["os_input"]="NOT_VERIFIED by this suite"}.ToString());
  var route=JObject.Parse(regionalRoute.text)["route"].Select(P).ToArray();
  WalkCheck("new_game_hub",walkingStory.data.region==-1&&walkingStory.data.nextRegion==0);
  // The room can be explored without touching the phone. Here the route elects
  // to read the family note and make one call, decisions that remain optional.
  var homeNote=walkingStory.hub.GetComponentsInChildren<DaoCuKeChuyenV40>().First();
  yield return ExperienceDocument(homeNote,new Vector3(-34,.94f,-35.2f));
  yield return ExperienceGarden(eye);
  yield return WalkPress(Key.P);yield return WalkPress(Key.Digit2);yield return ExperienceRead(walkingStory.Dialogue);yield return WalkPress(Key.P);lookController.enabled=false;
  yield return RegionalGo(new Vector3(-35,.94f,-34.6f),"sleep_first",10);yield return WalkUse(TuongTacHoCho.Kind.Bed);yield return new WaitForSeconds(1);
  yield return ExperiencePhaseObservation(eye);
  yield return WalkRoute(route,0,3,"lake_arrival");
  var bao=walkingStory.ghosts[0].position;if(walkingStory.GetComponent<TraiNghiemHanhTrinhV42>())yield return ExperienceSceneObservation(eye,walkingStory.ghosts[0],4.5f);
  yield return RegionalGo(bao+new Vector3(-1.1f,0,-.9f),"Bao_conversation",12);yield return ExperienceTalk(TuongTacHoCho.Kind.Bao);
  if(walkingStory.completion)
  {
   for(int n=0;n<2;n++)yield return ExperienceTalk(TuongTacHoCho.Kind.Bao);
   var calendar=walkingStory.traces[0].position+Vector3.back*1.7f;calendar.y=player.transform.position.y;
   yield return RegionalGo(calendar,"calendar_unchanged_date",12);eye.transform.LookAt(walkingStory.traces[0].position);yield return new WaitForSeconds(2);
   WalkCheck("calendar_trace_observed",walkingStory.data.traces[0]);
   var v42=walkingStory.GetComponent<TraiNghiemHanhTrinhV42>();if(v42)WalkCheck("Bao_scene_performance_moves_rod",v42.BaoEvents==1&&v42.BaoPoseDegrees>20,new JObject{["peak_degrees"]=v42.BaoPoseDegrees});
  }
  foreach(var puppet in walkingStory.puppets.Where(p=>p.gameObject.activeInHierarchy))
  {eye.transform.LookAt(puppet.dauTransform.position);yield return WalkPress(Key.D);yield return new WaitForSeconds(1);break;}
  yield return RegionalGo(route[3],"lake_path_return",12);yield return WalkRoute(route,4,31,"lake_departure");yield return WalkUse(TuongTacHoCho.Kind.Wake);
  yield return ExperienceGarden(eye);
  yield return RegionalGo(new Vector3(-35,.94f,-34.6f),"sleep_second",10);yield return WalkUse(TuongTacHoCho.Kind.Bed);yield return new WaitForSeconds(1);
  yield return ExperiencePhaseObservation(eye);
  foreach(var puppet in walkingStory.puppets.Where(p=>p.gameObject.activeInHierarchy))
  {eye.transform.LookAt(puppet.dauTransform.position);yield return WalkPress(Key.D);yield return new WaitForSeconds(.8f);break;}
  yield return WalkRoute(route,165,route.Length-1,"market_stalls");
  var ha=walkingStory.ghosts[1].position;if(walkingStory.GetComponent<TraiNghiemHanhTrinhV42>())yield return ExperienceSceneObservation(eye,walkingStory.ghosts[1],8.5f);
  yield return RegionalGo(ha+new Vector3(-1.3f,0,-1.7f),"Ha_conversation",14);yield return ExperienceTalk(TuongTacHoCho.Kind.Ha);
  if(walkingStory.completion)
  {
   for(int n=0;n<2;n++)yield return ExperienceTalk(TuongTacHoCho.Kind.Ha);
   WalkCheck("Vy_once_market",walkingStory.data.vyEncountered);
   var v42=walkingStory.GetComponent<TraiNghiemHanhTrinhV42>();if(v42)WalkCheck("Ha_scene_performance_moves_parcel",v42.HaEvents==1&&v42.ParcelTravel>.20f,new JObject{["peak_travel_m"]=v42.ParcelTravel});
   var v43=walkingStory.GetComponent<DieuChinhHanhTrinhV43>();if(v43){WalkCheck("Ha_v43_two_complete_replace_cycles",v43.Cycles==2);WalkCheck("Ha_v43_both_hands_contact",v43.ContactError<.005f&&v43.PeakTravel>.25f,new JObject{["contact_error_m"]=v43.ContactError,["peak_travel_m"]=v43.PeakTravel});}
  }
  yield return RegionalGo(walkingStory.traces[1].position+new Vector3(-1.7f,0,-1.1f),"unopened_books",12);eye.transform.LookAt(walkingStory.traces[1].position);yield return new WaitForSeconds(3);
  yield return RegionalGo(route.Last(),"market_exit",14);yield return WalkUse(TuongTacHoCho.Kind.Wake);
  yield return ExperienceGarden(eye);
  yield return WalkPress(Key.P);yield return WalkPress(Key.Digit1);yield return ExperienceRead(walkingStory.Dialogue);yield return WalkPress(Key.P);lookController.enabled=false;
  yield return RegionalGo(new Vector3(-35,.94f,-34.6f),"sleep_third",10);yield return WalkUse(TuongTacHoCho.Kind.Bed);yield return new WaitForSeconds(1);
  school=walkingStory.schoolChapter;var origin=school.spawn.position-new Vector3(0,.94f,-7);
  foreach(var point in new[]{new Vector3(-10,.94f,-5),new Vector3(-22,.94f,-5),new Vector3(-22,.94f,0)})yield return SchoolGo(origin+point,"school_archive");
  foreach(var document in school.environment.GetComponentsInChildren<DaoCuKeChuyenV40>().OrderBy(p=>p.transform.position.z))
  {
   var approach=document.transform.position+Vector3.back*1.6f;approach.y=origin.y+.94f;
   yield return SchoolGo(new Vector3(origin.x-22,origin.y+.94f,approach.z),"archive_central_aisle");yield return ExperienceDocument(document,approach);
  }
  yield return SchoolGo(origin+new Vector3(-22,.94f,5.4f),"archive_return_aisle");yield return SchoolGo(origin+new Vector3(-22,.94f,10),"archive_registers");
  foreach(int index in new[]{1,2,3,0})
  {
   var memory=school.environment.GetComponentsInChildren<TuongTacTruongV37>().First(i=>i.memoryIndex==index);
   if(index==0){yield return SchoolGo(origin+new Vector3(-22,.94f,0),"archive_aisle");yield return SchoolGo(origin+new Vector3(-22,.94f,-5),"archive_door");}
   var approach=memory.transform.position+Vector3.back*1.7f;approach.y=origin.y+.94f;yield return SchoolGo(approach,"memory_"+index);eye.transform.LookAt(memory.transform.position);yield return WalkPress(Key.E);yield return ExperienceRead(memory.memory);
  }
  foreach(var point in new[]{new Vector3(-10,.94f,-5),new Vector3(0,.94f,-5),new Vector3(0,.94f,1),new Vector3(0,.94f,5)})yield return SchoolGo(origin+point,"classroom_entry");
  eye.transform.LookAt(school.puppet.gesturePart.position);RegionKey(null);float deadline=Time.realtimeSinceStartup+5;
  while((!school.Leading||school.LeadSeconds<1.35f)&&Time.realtimeSinceStartup<deadline)yield return null;
  if(walkingStory.completion){walkingStory.polish.Event("GD3_hold_observation","automation elects to watch; player may move at any time");yield return new WaitForSeconds(3);WalkCheck("GD3_holds_without_input",school.Leading);}
  yield return WalkPress(Key.D);WalkCheck("GD3_visible_response",walkingStory.data.schoolWitnessed);
  yield return SchoolGo(origin+new Vector3(1.2f,.94f,9),"Mai_conversation");
  for(int n=0;n<4;n++){yield return SchoolUse(false);yield return ExperienceRead(walkingStory.Dialogue);}
  yield return SchoolGo(origin+new Vector3(4.5f,.94f,11),"chair_window");eye.transform.LookAt(school.chair.position);yield return new WaitForSeconds(2);
  foreach(var point in new[]{new Vector3(0,.94f,10),new Vector3(0,.94f,5),new Vector3(0,.94f,1),new Vector3(0,.94f,-3),new Vector3(0,.94f,-7)})yield return SchoolGo(origin+point,"school_departure");
  yield return SchoolUse(true);
  yield return ExperienceGarden(eye);
  yield return RegionalGo(new Vector3(-35,.94f,-34.6f),"sleep_fourth",10);yield return WalkUse(TuongTacHoCho.Kind.Bed);yield return new WaitForSeconds(1);
  var slope=walkingStory.slopeChapter;origin=slope.spawn.position-new Vector3(0,.94f,1);
  for(int z=8;z<=72;z+=8)
  {
   yield return SlopeGo(origin+new Vector3(0,Mathf.Min(z,60)*.1f+.94f,z),"slope_"+z);
   if(z==16||z==48)
   {
    var memory=slope.environment.GetComponentsInChildren<TuongTacDocV39>().First(i=>i.memoryIndex==(z==16?0:1));
    var approach=memory.transform.position+Vector3.back*1.7f;approach.y=origin.y+Mathf.Min(approach.z-origin.z,60)*.1f+.94f;
    yield return SlopeGo(approach,"Tung_paper");yield return SlopeUse(memory);yield return ExperienceRead(memory.memory);
   }
   if(z==40){yield return SlopeGo(origin+new Vector3(1.3f,5.34f,44),"packed_suitcase");eye.transform.LookAt(slope.suitcase.position);yield return new WaitForSeconds(3);}
  }
  var talk=slope.environment.GetComponentsInChildren<TuongTacDocV39>().First(i=>!i.finalDoor&&i.memoryIndex<0);
  for(int n=0;n<3;n++){yield return SlopeUse(talk);yield return ExperienceRead(walkingStory.Dialogue);}
  yield return SlopeGo(origin+new Vector3(0,6.94f,74),"final_entrance");yield return SlopeUse(slope.environment.GetComponentsInChildren<TuongTacDocV39>().First(i=>i.finalDoor));
  eye.transform.LookAt(slope.puppet.dauTransform.position);while(walkingStory.data.region==3)yield return null;
  WalkCheck("complete_journey",!string.IsNullOrEmpty(walkingStory.data.ending)&&walkingStory.data.nextRegion==4);
  for(int beat=0;beat<4;beat++){yield return ExperienceRead(HoanThienTrinhBayV40.EndingText(walkingStory.data.ending,beat));if(beat<3)yield return WalkPress(Key.E);}
  yield return FinishWalk();
 }
}

#endif
