#if UNITY_EDITOR || CON_ROI_BUILD_REVIEW
using System.Collections;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public sealed partial class C3ActualPlayRun
{
 int actingPlacements;
 void ActingSeed(int region,Vector3 position)
 {
  var state=new DieuPhoiHanhTrinh.State{region=region,nextRegion=Mathf.Max(0,region),position=position,rotation=Quaternion.identity,posted=new bool[4],called=new bool[4],dismissed=new bool[4],cameraSaved=false};
  File.WriteAllText(walkingStory.SavePathOverride,JsonUtility.ToJson(state));walkingStory.Load();lookController.enabled=false;actingPlacements++;
 }
 void ActingEvent(string label){walkingStory.polish.Event("v45_inspection_clip",label);phase=label;}
 IEnumerator ActingReload(DienXuatHanhTrinhV45 acting,string label)
 {
  walkingStory.Save();var poses=acting.driven.Select(t=>new{p=t.position,q=t.rotation}).ToArray();float time=acting.VyTime;int steps=acting.Footsteps;
  yield return new WaitForSeconds(.2f);walkingStory.Load();lookController.enabled=false;
  float delta=acting.driven.Select((t,i)=>Vector3.Distance(t.position,poses[i].p)).Max();float angle=acting.driven.Select((t,i)=>Quaternion.Angle(t.rotation,poses[i].q)).Max();
  WalkCheck(label+"_exact_pose_at_load",delta<.0001f&&angle<.05f,new JObject{["max_position_error_m"]=delta,["max_angle_error_deg"]=angle});
  WalkCheck(label+"_timeline_restored",Mathf.Abs(acting.VyTime-time)<.001f&&acting.Footsteps==steps);
  yield return null;var deltas=acting.driven.Select((t,i)=>new{distance=Vector3.Distance(t.position,poses[i].p),name=t.name}).OrderByDescending(x=>x.distance).ToArray();float immediate=deltas[0].distance;WalkCheck(label+"_no_next_frame_snap",immediate<.08f,new JObject{["maximum_next_frame_travel_m"]=immediate,["worst_transform"]=deltas[0].name,["frame_dt"]=Time.deltaTime});
 }
 IEnumerator RunActingV45()
 {
  yield return PrepareJourneyCheck("acting");var acting=walkingStory.GetComponent<DienXuatHanhTrinhV45>();var eye=player.GetComponentInChildren<Camera>();var inspect=eye.gameObject.AddComponent<InspectionCameraV45>();
  File.WriteAllText(Path.Combine(VerificationOutput,"acting-method.json"),new JObject{["mode"]="Staged runtime inspection; region/load fixture placements and inspection camera cuts. Not a player walkthrough or OS input.",["puppet_control"]="Existing four-stage components only; NPC rig excludes puppets",["audio"]="Actual mixer, contact cues; no VO or lip sync"}.ToString());
  foreach(int index in new[]{4,0,1,2,3})
  {
   var rig=acting.rigs[index];int area=index==4?-1:index;
   var floor=rig.frame.position;var front=-rig.frame.forward;var pos=floor+front*2;pos.y=floor.y+.94f;
   if(index==4)pos=new Vector3(-35,.94f,-36);
   ActingSeed(area,pos);yield return new WaitForSeconds(.3f);inspect.rig=rig;inspect.close=false;
   ActingEvent(rig.role+"_body_idle");yield return new WaitForSeconds(2);
   if(index==0)walkingStory.Talk(0);else if(index==1)walkingStory.Talk(1);else if(index==2)walkingStory.schoolChapter.Talk();else if(index==3)walkingStory.slopeChapter.Talk();else walkingStory.completion.VisitLan();
   ActingEvent(rig.role+"_body_interaction");yield return new WaitForSeconds(2);
   float weight=rig.attention;WalkCheck(rig.role+"_subtitle_attention_blends_in",weight>.8f,new JObject{["attention"]=weight});
   WalkCheck(rig.role+"_hands_outside_body_envelope",acting.MaxHandBodyOverlap<.001f);
   yield return ActingReload(acting,rig.role);
   inspect.close=true;ActingEvent(rig.role+"_face_recover");yield return new WaitForSeconds(2.5f);
   WalkCheck(rig.role+"_no_silent_lips",rig.actor.GetComponentsInChildren<BieuCamNhoNhanVatV41>(true).All(x=>!x.enabled));
   if(index==1){inspect.close=false;ActingEvent("Ha_two_cycles_close");yield return new WaitForSeconds(8);var ha=walkingStory.GetComponent<DieuChinhHanhTrinhV43>();WalkCheck("Ha_hands_follow_parcel_after_reload",ha.ContactError<.005f&&ha.Cycles==2,new JObject{["contact_m"]=ha.ContactError,["cycles"]=ha.Cycles});}
  }
  var vy=acting.rigs[5];var entry=acting.vyOrigin+Vector3.back*5;entry.y=acting.vyOrigin.y+.94f;
  ActingSeed(1,entry);inspect.rig=vy;inspect.close=false;ActingEvent("Vy_feet_full_stride");
  float deadline=Time.realtimeSinceStartup+10,maxSlip=0,maxGround=0,renderSlip=0;bool movingSeen=false,reloaded=false,pausedTest=false;int observedSteps=0,supportedSamples=0,unsupportedSamples=0;Vector3[] soles=new Vector3[2];bool[] stance=new bool[2];
  while(Time.realtimeSinceStartup<deadline)
  {
   maxSlip=Mathf.Max(maxSlip,acting.PlantSlip);maxGround=Mathf.Max(maxGround,acting.GroundError);observedSteps=Mathf.Max(observedSteps,acting.Footsteps);movingSeen|=acting.VyMoving;
   if(!reloaded&&acting.VyTime>3){reloaded=true;yield return ActingReload(acting,"Vy_mid_stride");stance[0]=stance[1]=false;}
   if(!pausedTest&&acting.VyTime>4)
   {
    pausedTest=true;yield return WalkPress(Key.Escape);float heldTime=acting.VyTime;int heldSteps=acting.Footsteps;var heldFoot=vy.shoes[0].position;
    yield return new WaitForSecondsRealtime(.4f);WalkCheck("Vy_pause_freezes_pose_and_contacts",walkingStory.Paused&&Mathf.Abs(acting.VyTime-heldTime)<.001f&&acting.Footsteps==heldSteps&&!acting.FootAudio&&Vector3.Distance(heldFoot,vy.shoes[0].position)<.001f);
    yield return WalkPress(Key.Escape);lookController.enabled=false;stance[0]=stance[1]=false;
   }
   yield return new WaitForEndOfFrame();
   // A load fixture can now spend frames in the render gate. Hidden poses from
   // the previous encounter are not rendered stance samples of the next one.
   if(walkingStory.Loading||!vy.actor.gameObject.activeInHierarchy){stance[0]=stance[1]=false;yield return null;continue;}
   for(int i=0;i<2;i++)
   {
    bool planted=acting.FootPlanted(i);if(planted&&stance[i])renderSlip=Mathf.Max(renderSlip,Vector3.Distance(soles[i],vy.shoes[i].position));soles[i]=vy.shoes[i].position;stance[i]=planted;
    if(planted){var sole=vy.shoes[i].position-Vector3.up*.067f;bool has=Physics.RaycastAll(sole+Vector3.up*.05f,Vector3.down,.10f,~0,QueryTriggerInteraction.Ignore).Any(h=>h.normal.y>.7f&&!h.transform.IsChildOf(vy.actor)&&Mathf.Abs(h.point.y-sole.y)<.015f);if(has)supportedSamples++;else unsupportedSamples++;}
   }
   yield return null;
  }
  WalkCheck("Vy_actual_motion_and_multiple_contacts",movingSeen&&acting.Walked>1.79f&&observedSteps>=4,new JObject{["distance_m"]=acting.Walked,["contacts"]=observedSteps});
  WalkCheck("Vy_world_planted_feet_no_slide",maxSlip<.002f,new JObject{["max_stance_slip_m"]=maxSlip});WalkCheck("Vy_contact_support",maxGround<.015f,new JObject{["max_support_error_m"]=maxGround});
  WalkCheck("Vy_rendered_stance_sample_no_slide",renderSlip<.002f,new JObject{["after_end_of_frame_stance_delta_m"]=renderSlip});
  WalkCheck("Vy_rendered_feet_hit_actual_support",supportedSamples>30&&unsupportedSamples==0,new JObject{["supported_samples"]=supportedSamples,["unsupported_samples"]=unsupportedSamples});
  inspect.close=true;ActingEvent("Vy_face_should_remain_ambiguous");yield return new WaitForSeconds(1);
  inspect.rig=null;eye.transform.rotation=Quaternion.LookRotation(Vector3.up);yield return new WaitForSeconds(.2f);
  WalkCheck("Vy_hidden_stops_contact_audio",!walkingStory.completion.VyVisible&&!acting.FootAudio);
  var stepsHidden=acting.Footsteps;yield return new WaitForSeconds(.5f);WalkCheck("Vy_no_hidden_contact_events",acting.Footsteps==stepsHidden);
  walkingStory.Wake();yield return null;WalkCheck("Vy_no_foot_audio_outside_market",!acting.FootAudio&&!walkingStory.completion.VyVisible);
  var puppetScripts=acting.driven.Any(t=>t.GetComponentInParent<DieuKhienConRoi>());WalkCheck("NPC_channels_exclude_all_puppets",!puppetScripts);
  UnityEngine.Object.Destroy(inspect);File.WriteAllText(Path.Combine(VerificationOutput,"inspection-placements.json"),new JObject{["count"]=actingPlacements,["description"]="Explicit region/save fixture positions for close inspection. No claim of uninterrupted traversal; separate walkthrough required."}.ToString());yield return FinishWalk();
 }
}

[DefaultExecutionOrder(29000)]
public sealed class InspectionCameraV45:MonoBehaviour
{
 public DienXuatHanhTrinhV45.Rig rig;public bool close;
 void LateUpdate()
 {
  if(rig==null)return;var frame=rig.frame;Vector3 target=close?rig.head.position:frame.position+Vector3.up*.94f;
  var p=close?target+frame.TransformDirection(new Vector3(-.30f,.035f,-.80f)):frame.position+frame.TransformDirection(new Vector3(-1.05f,1.12f,-2.05f));
  transform.SetPositionAndRotation(p,Quaternion.LookRotation(target-p));GetComponent<Camera>().fieldOfView=close?34:50;
 }
}

#endif
