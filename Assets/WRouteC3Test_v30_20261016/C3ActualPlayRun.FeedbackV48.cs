#if UNITY_EDITOR || CON_ROI_BUILD_REVIEW
using System;
using System.IO;
using System.Linq;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using Newtonsoft.Json.Linq;

public sealed partial class C3ActualPlayRun
{
 IEnumerator V48GuidanceCheck()
 {
  var guide=walkingStory.GetComponent<HuongDanHanhTrinhV48>();if(!guide)yield break;
  yield return new WaitForSeconds(.8f);
  int region=walkingStory.data.region;var objective=guide.Objective;int actions=walkingStory.data.interactions;
  yield return WalkPress(Key.P);WalkCheck("phone_objective_region_"+region,walkingStory.PhoneOpen&&!string.IsNullOrEmpty(guide.Objective));yield return FeedbackShot("objective_phone_region_"+region);
  if(region>=0){yield return WalkPress(Key.Digit1);WalkCheck("dream_phone_cannot_post_"+region,walkingStory.data.interactions==actions);}
  yield return WalkPress(Key.P);lookController.enabled=false;
  yield return WalkPress(Key.F5);var at=player.transform.position;yield return WalkPress(Key.F6);lookController.enabled=false;
  yield return new WaitForSeconds(.8f);WalkCheck("objective_reminder_after_reload_"+region,guide.ReminderVisible);
  WalkCheck("objective_after_reload_region_"+region,guide.Objective==objective&&Vector3.Distance(at,player.transform.position)<.15f);
  var eye=player.GetComponentInChildren<Camera>();var rotation=eye.transform.rotation;
  for(int yaw=0;yaw<360;yaw+=90){eye.transform.rotation=Quaternion.Euler(0,yaw,0);yield return new WaitForSeconds(.15f);yield return FeedbackShot("entry_rotate_"+region+"_"+yaw);}
  foreach(int pitch in new[]{-70,70}){eye.transform.rotation=Quaternion.Euler(pitch,0,0);yield return new WaitForSeconds(.15f);yield return FeedbackShot("entry_pitch_"+region+"_"+pitch);}
  eye.transform.rotation=rotation;
 }
 IEnumerator V48DeskView()
 {
  var eye=player.GetComponentInChildren<Camera>();var rotation=eye.transform.rotation;
  var desk=walkingStory.hub.GetComponentsInChildren<Transform>(true).First(t=>t.name=="Desk_Puppet_Broken_Strings");
  var body=desk.GetComponentsInChildren<Renderer>(true).First(r=>r.name=="ConRoi_Body_Wood");
  eye.transform.LookAt(body.bounds.center);yield return new WaitForSeconds(1);yield return FeedbackShot("desk_waking_player");eye.transform.rotation=rotation;
 }
 IEnumerator FeedbackShot(string name){yield return CapturePlayerImage(Path.GetFullPath(folder+"/"+name+".png"));}
 IEnumerator RunFeedbackV48(Vector3[] route)
 {
  var entry=walkingStory.GetComponent<MenuVaoGameV47>();walkingStory.SetMenu(true);yield return new WaitForSecondsRealtime(2);
  yield return FeedbackShot("00-menu");yield return WalkPress(Key.Enter);lookController.enabled=false;
  WalkCheck("New_Game_from_entry",!walkingStory.MenuOpen&&walkingStory.data.region==-1&&walkingStory.data.nextRegion==0);
  yield return FeedbackShot("01-hub-objective");yield return WalkPress(Key.P);yield return FeedbackShot("02-phone");yield return WalkPress(Key.P);lookController.enabled=false;yield return V48DeskView();
  yield return RegionalGo(new Vector3(-35,.94f,-34.6f),"v48_bed",8);yield return WalkUse(TuongTacHoCho.Kind.Bed);
  yield return FeedbackShot("03-lake-opening");
  var eye=player.GetComponentInChildren<Camera>();var ai=walkingStory.puppets[0];
  // Ordinary lateral steps, held camera at the visible wooden figure, then rest.
  eye.transform.LookAt(ai.dauTransform.position-Vector3.up*.3f);yield return new WaitForSeconds(2);
  yield return WalkPress(Key.D);RegionKey(Key.D);yield return new WaitForSeconds(.6f);RegionKey(null);yield return new WaitForSeconds(1);yield return FeedbackShot("04-immediate-stopped");
  RegionKey(Key.A);yield return new WaitForSeconds(.6f);RegionKey(null);yield return new WaitForSeconds(1);
  for(int i=0;i<route.Length;i++)
  {
   yield return RegionalGo(route[i],"v48_full_route_"+i,12);
   if(!regionalReached){WalkCheck("full_route_reached_"+i,false);yield return FeedbackShot("blocked_"+i);yield return FinishWalk();yield break;}
   if(new[]{0,3,31,45,60,80,100,120,140,164,165,200,278}.Contains(i))
   {
    if(i<route.Length-1){var target=route[Mathf.Min(i+4,route.Length-1)];target.y=eye.transform.position.y;eye.transform.LookAt(target);}
    yield return new WaitForSeconds(.4f);yield return FeedbackShot("route_"+i.ToString("D3"));
    if(i==45||i==140){for(int yaw=0;yaw<4;yaw++){eye.transform.Rotate(0,90,0,Space.World);yield return FeedbackShot("rotate_"+i+"_"+yaw);}}
   }
  }
  WalkCheck("lake_roundabout_market_walk_complete",true);
  yield return WalkPress(Key.F5);var pos=player.transform.position;yield return WalkPress(Key.F6);lookController.enabled=false;
  WalkCheck("far_market_load_continuity",Vector3.Distance(player.transform.position,pos)<.15f);yield return FeedbackShot("market_reload");
  for(int i=route.Length-1;i>=31;i--){yield return RegionalGo(route[i],"v48_return_"+i,12);if(!regionalReached){WalkCheck("return_reached_"+i,false);yield return FinishWalk();yield break;}}
  WalkCheck("market_roundabout_lake_return_complete",true);yield return FeedbackShot("lake_return");
  if(walkingStory.GetComponent<HuongDanHanhTrinhV48>())
  {
   // Reach the actual bridge from the lake path; no fixture placement.
   yield return WalkRoute(route,32,61,"v48_bridge_approach");
   var record=JObject.Parse(bridgeRecord.text);var south=record["connections"][1].Select(P).ToArray();var deck=record["paths"]["sidewalk_-1"].Select(P).ToArray();var north=record["connections"][0].Select(P).Reverse().ToArray();
   foreach(var p in south){yield return RegionalGo(p,"v48_south_bridge_link",15);if(!regionalReached)break;}
   yield return RegionalGo(deck[deck.Length-2],"v48_bridge_south_end",15);WalkCheck("bridge_reached_on_foot",regionalReached);yield return FeedbackShot("bridge_south");
   yield return RegionalGo(deck[1],"v48_cross_bridge",35);WalkCheck("bridge_crossed_on_foot",regionalReached);yield return FeedbackShot("bridge_north");
   foreach(var p in north){yield return RegionalGo(p,"v48_north_bridge_link",15);if(!regionalReached)break;}
   yield return RegionalGo(route[122],"v48_rejoin_city_paving",15);yield return WalkRoute(route,123,165,"v48_bridge_to_gate");yield return FeedbackShot("bridge_gate");
  }
  File.WriteAllText(Path.Combine(VerificationOutput,"experience-method.json"),new JObject{["new_game"]=true,["input"]="Virtual keyboard through existing gameplay actions; automated camera steering",["fixture_teleports"]=0,["seeded_progress"]=false,["route"]="Every waypoint 0..278 then 278..31 on foot, including previously untested 32..164",["human_playtest"]=false}.ToString());
  yield return FinishWalk();
 }
}

#endif
