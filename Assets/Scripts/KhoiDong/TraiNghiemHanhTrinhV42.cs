// Bổ sung lớp trải nghiệm hành trình V42 trên các owner hiện hữu.
// Component trong scene r92: Lake_Market_Story_GDD21.
// Reference public để đối chiếu Inspector (có nhánh tùy chọn): story, sortedParcel.
// Code tự tìm/tạo: TuTheBaoBenCauV48, Camera, RuntimeFrameProbeV42, HienGuongMatTheoKhu.
// Thứ tự chạy khai báo: 23000; giữ quan hệ khởi tạo/pose với các owner khác.
using System;
using System.IO;
using UnityEngine;

// Optional authoring layer, present only in the v42 scene. No progression gates.
[DefaultExecutionOrder(23000)]
[UnityEngine.Scripting.APIUpdating.MovedFrom(true, null, "Assembly-CSharp", "JourneyExperienceV42")]
public sealed class TraiNghiemHanhTrinhV42:MonoBehaviour
{
 public DieuPhoiHanhTrinh story;
 public Transform sortedParcel;
 Quaternion baoRest,haRest,calendarRest,booksRest;
 Vector3 parcelRest;
 Vector3 parcelInArm;
 Quaternion parcelRestRotation,parcelInArmRotation,baoEventRest,haEventRest;
 Camera eye;
 TuTheBaoBenCauV48 baoBridge;
 float baoAt=-1,haAt=-1;
 int region=-99;
 float baoPoseElapsed=-1,haPoseElapsed=-1;
 public int BaoEvents {get;private set;}
 public int HaEvents {get;private set;}
 public float BaoPoseDegrees {get;private set;}
 public float BaoHabitLift=>!(baoBridge&&baoBridge.leaningOnly)&&region==0&&baoAt>=0&&baoPoseElapsed>=0&&baoPoseElapsed<4?Mathf.Sin(Mathf.Clamp01(baoPoseElapsed/4)*Mathf.PI):0;
 public float ParcelTravel {get;private set;}
 public bool externalSorting;
 public void RecordSortingTravel(float value){ParcelTravel=Mathf.Max(ParcelTravel,value);}
 [Serializable] public class SavedAct {public float bao=-1,ha=-1;public int region,baoEvents,haEvents;public Quaternion baoRest,haRest,parcelRotation;public Vector3 parcelInArm;public float baoPeak,parcelPeak;}
 public SavedAct CaptureAct()=>new SavedAct{bao=baoAt<0?-1:Mathf.Max(0,baoPoseElapsed),ha=haAt<0?-1:Mathf.Max(0,haPoseElapsed),region=region,baoEvents=BaoEvents,haEvents=HaEvents,baoRest=baoEventRest,haRest=haEventRest,parcelInArm=parcelInArm,parcelRotation=parcelInArmRotation,baoPeak=BaoPoseDegrees,parcelPeak=ParcelTravel};
 public void RestoreAct(SavedAct s){region=s.region;baoPoseElapsed=s.bao;haPoseElapsed=s.ha;baoAt=s.bao<0?-1:Time.time-s.bao;haAt=s.ha<0?-1:Time.time-s.ha;BaoEvents=s.baoEvents;HaEvents=s.haEvents;baoEventRest=s.baoRest;haEventRest=s.haRest;parcelInArm=s.parcelInArm;parcelInArmRotation=s.parcelRotation;BaoPoseDegrees=s.baoPeak;ParcelTravel=s.parcelPeak;}
 public void ResetAct(){region=-99;BaoEvents=HaEvents=0;baoAt=haAt=baoPoseElapsed=haPoseElapsed=-1;}
 void Start()
 {
  baoBridge=GetComponent<TuTheBaoBenCauV48>();
  baoRest=story.completion.baoArm.localRotation;haRest=story.completion.haArm.localRotation;
  calendarRest=story.traces[0].localRotation;booksRest=story.traces[1].localRotation;parcelRest=sortedParcel.position;parcelRestRotation=sortedParcel.rotation;eye=story.player.GetComponentInChildren<Camera>();
  gameObject.AddComponent<RuntimeFrameProbeV42>().story=story;
 }
 bool Observable(int index)
 {
  var actor=story.ghosts[index];if(Vector3.Distance(story.player.transform.position,actor.position)>4.5f)return false;
  var target=actor.GetComponent<HienGuongMatTheoKhu>().ghostHead.position;
  var view=eye.WorldToViewportPoint(target);if(view.z<=0||view.x<.12f||view.x>.88f||view.y<.12f||view.y>.88f)return false;
  var delta=target-eye.transform.position;
  if(Physics.Raycast(eye.transform.position,delta.normalized,out var hit,delta.magnitude,~0,QueryTriggerInteraction.Ignore))
   return hit.collider.transform.IsChildOf(actor)||hit.collider.name==(index==0?"CR_Bao_Interaction":"CR_Ha_Interaction");
  return true;
 }
 void LateUpdate()
 {
  if(story.Paused)return;var d=story.data;float now=Time.time;
  if(d.region!=region){region=d.region;baoAt=haAt=-1;sortedParcel.position=parcelRest;sortedParcel.rotation=parcelRestRotation;}
  if(region==0&&baoAt<0&&Observable(0))
  {baoAt=now;baoEventRest=story.completion.baoArm.localRotation;BaoEvents++;story.polish.Event(baoBridge&&baoBridge.leaningOnly?"Bao_observed_rail_rest":"Bao_observed_rod_habit","camera visible and unoccluded; optional; no reward");}
  if(region==1&&haAt<0&&Observable(1))
  {haAt=now;haEventRest=story.completion.haArm.localRotation;parcelInArm=story.completion.haArm.InverseTransformPoint(sortedParcel.position);parcelInArmRotation=Quaternion.Inverse(story.completion.haArm.rotation)*sortedParcel.rotation;HaEvents++;story.polish.Event("Ha_observed_sorting","camera visible and unoccluded; parcel follows hand for two finite cycles; optional; no reward");}
  // Bao lifts an empty line, then returns to the same habitual pose.
  if(region==0&&baoAt>=0)
  {
   float t=now-baoAt;baoPoseElapsed=t;float lift=t<4?Mathf.Sin(Mathf.Clamp01(t/4)*Mathf.PI):0;
   if(!(baoBridge&&baoBridge.leaningOnly))story.completion.baoArm.localRotation=baoEventRest*Quaternion.Euler(lift*27,0,-lift*4);
   BaoPoseDegrees=Mathf.Max(BaoPoseDegrees,Quaternion.Angle(baoEventRest,story.completion.baoArm.localRotation));
  }
  // Ha's hand and the parcel move together; repetition is witnessed in-scene.
  if(!externalSorting&&region==1&&haAt>=0)
  {
   float t=now-haAt;haPoseElapsed=t;float cycle=t<8?Mathf.Repeat(t,4)/4:0;float lift=Mathf.Sin(cycle*Mathf.PI);
   story.completion.haArm.localRotation=haEventRest*Quaternion.Euler(lift*32,0,lift*8);
   sortedParcel.position=story.completion.haArm.TransformPoint(parcelInArm);
   sortedParcel.rotation=story.completion.haArm.rotation*parcelInArmRotation;
   ParcelTravel=Mathf.Max(ParcelTravel,Vector3.Distance(parcelRest,sortedParcel.position));
   // Discrete foley uses the existing two-voice pool, not another audio layer.
   if(t<8&&cycle<Time.deltaTime/4)story.PlayEffectAt(story.completion.cloth,sortedParcel.position);
  }
  story.traces[0].localRotation=Quaternion.Slerp(story.traces[0].localRotation,calendarRest*Quaternion.Euler(0,0,d.traces[0]?-14:0),1-Mathf.Exp(-Time.deltaTime*2));
  story.traces[1].localRotation=Quaternion.Slerp(story.traces[1].localRotation,booksRest*Quaternion.Euler(0,d.traces[1]?12:0,0),1-Mathf.Exp(-Time.deltaTime*2));
 }
}
