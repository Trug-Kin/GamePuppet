#if UNITY_EDITOR || CON_ROI_BUILD_REVIEW
using System.Collections;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public sealed partial class C3ActualPlayRun
{
 IEnumerator RunGddV47()
 {
  yield return PrepareJourneyCheck("gdd_v47");var room=walkingStory.GetComponent<PhongTrongMoV47>();var eye=player.GetComponentInChildren<Camera>();
  WalkCheck("dream_room_component_and_five_strings",room&&room.brokenStrings.Length==5&&room.repairedStrings.Length==5);
  if(!room){yield return FinishWalk();yield break;}
  // Actual elapsed time, with no timescale acceleration or modified fallback threshold.
  float start=Time.realtimeSinceStartup;yield return new WaitForSeconds(80);
  var latch=walkingStory.hub.GetComponentInChildren<TuongTacVuonV41>();eye.transform.LookAt(latch.transform.position);yield return WalkPress(Key.E);
  WalkCheck("fallback_reset_by_real_E",walkingStory.data.region==-1&&(walkingStory.data.lanVisitMask&1)!=0);
  float lastInteraction=Time.realtimeSinceStartup;yield return new WaitForSeconds(12);
  WalkCheck("no_sleep_at_90s_since_entry_after_interaction",Time.realtimeSinceStartup-start>=90&&!room.DreamHub&&walkingStory.data.region==-1);
  yield return WalkPress(Key.Escape);yield return new WaitForSecondsRealtime(2);WalkCheck("pause_does_not_enter_dream",walkingStory.Paused&&!room.DreamHub);
  yield return WalkPress(Key.Escape);lookController.enabled=false;
  float deadline=Time.realtimeSinceStartup+85;while(!room.DreamHub&&Time.realtimeSinceStartup<deadline)yield return null;
  float delay=Time.realtimeSinceStartup-lastInteraction;
  WalkCheck("fallback_90s_after_interaction_excludes_pause",room.DreamHub&&delay>=91.8f&&delay<94,new JObject{["elapsed_real_seconds"]=delay,["pause_seconds"]=2});
  if(!room.DreamHub){yield return FinishWalk();yield break;}
  // Native audio state can lag a PlayOneShot call made earlier in this frame.
  // Observe the playing source after an audio update, while the cue is still audible.
  yield return new WaitForSeconds(.04f);
  var voices=walkingStory.GetComponentsInChildren<AudioSource>();
  WalkCheck("new_message_notification_survives_region_apply",walkingStory.data.vyMessages==1&&voices.Any(s=>s.clip==walkingStory.notification&&s.isPlaying),new JObject{["message_count"]=walkingStory.data.vyMessages,["notification"]=walkingStory.notification?walkingStory.notification.name:"missing",["sources"]=new JArray(voices.Select(s=>new JObject{["name"]=s.name,["clip"]=s.clip?s.clip.name:"none",["playing"]=s.isPlaying,["time"]=s.time}))});
  var clock=walkingStory.clockHand.localRotation;var volume=walkingStory.GetComponent<Volume>();var grade=volume.profile.components.OfType<ColorAdjustments>().Single();
  WalkCheck("dream_hub_world_and_grading",walkingStory.data.region==-1&&walkingStory.hub.activeSelf&&!walkingStory.lake.activeSelf&&volume.weight==1&&Mathf.Abs(grade.saturation.value+30)<.01f);
  WalkCheck("dream_hub_repaired_strings_and_drone",room.repairedStrings.All(r=>r.enabled)&&room.brokenStrings.All(r=>!r.enabled)&&walkingStory.GetComponents<AudioSource>().Any(s=>s.clip==walkingStory.drone&&s.isPlaying));
  var desk=room.repairedStrings[0].transform.parent;eye.transform.LookAt(desk.position+Vector3.up*.16f);yield return new WaitForSeconds(.3f);
  WalkCheck("dream_clock_stopped",Quaternion.Angle(clock,walkingStory.clockHand.localRotation)<.001f);
  yield return CapturePlayerImage(Path.Combine(folder,"dream-room.png"));
  yield return WalkPress(Key.F5);float elapsed=(float)JObject.Parse((string)JObject.Parse(File.ReadAllText(walkingStory.SavePathOverride))["dreamRoom"])["elapsed"];int messages=walkingStory.data.vyMessages;
  yield return WalkPress(Key.F6);lookController.enabled=false;
  WalkCheck("dream_hub_save_load_resumes_once",room.DreamHub&&room.Elapsed>=elapsed&&room.Elapsed<elapsed+.5f&&walkingStory.data.vyMessages==messages&&walkingStory.data.awareness==0);
  deadline=Time.realtimeSinceStartup+8;while((walkingStory.data.region<0||walkingStory.Loading)&&Time.realtimeSinceStartup<deadline)yield return null;
  WalkCheck("dream_hub_advances_to_lake_without_extra_task",walkingStory.data.region==0&&!room.DreamHub&&!walkingStory.hub.activeSelf&&walkingStory.data.nextRegion==0&&walkingStory.data.interactions==0);
  yield return WalkPress(Key.F5);string valid=File.ReadAllText(walkingStory.SavePathOverride);var original=walkingStory.data;var pos=player.transform.position;
  var malformed=JObject.Parse(valid);malformed["dreamRoom"]="{\"dreaming\":true,\"elapsed\":-2}";File.WriteAllText(walkingStory.SavePathOverride,malformed.ToString());walkingStory.Load();
  WalkCheck("invalid_dream_save_rejected_atomically",ReferenceEquals(original,walkingStory.data)&&Vector3.Distance(pos,player.transform.position)<.01f);File.WriteAllText(walkingStory.SavePathOverride,valid);
  walkingStory.Restart();lookController.enabled=false;yield return null;
  WalkCheck("restart_awake_room_restored",walkingStory.data.region==-1&&!room.DreamHub&&room.brokenStrings.All(r=>r.enabled)&&room.repairedStrings.All(r=>!r.enabled)&&volume.weight==0);
  clock=walkingStory.clockHand.localRotation;yield return new WaitForSeconds(.4f);WalkCheck("awake_clock_moves",Quaternion.Angle(clock,walkingStory.clockHand.localRotation)>1);
  eye.transform.LookAt(desk.position+Vector3.up*.16f);yield return CapturePlayerImage(Path.Combine(folder,"awake-room.png"));
  yield return FinishWalk();
 }
}

#endif
