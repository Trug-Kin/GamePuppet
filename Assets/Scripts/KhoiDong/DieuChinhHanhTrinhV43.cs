// Điều chỉnh lớp trình bày hành trình V43; không thay controller người chơi.
// Component trong scene r92: Lake_Market_Story_GDD21.
// Reference public để đối chiếu Inspector (có nhánh tùy chọn): story, upper, lower, hands, haHead, parcel.
// Code tự tìm/tạo: TraiNghiemHanhTrinhV42.
// Thứ tự chạy khai báo: 24000; giữ quan hệ khởi tạo/pose với các owner khác.
using System;
using System.IO;
using UnityEngine;
using Newtonsoft.Json.Linq;

// Present only in the v43 scene. Optional finite action, no progression or waits.
[DefaultExecutionOrder(24000)]
[UnityEngine.Scripting.APIUpdating.MovedFrom(true, null, "Assembly-CSharp", "JourneyRevisionV43")]
public sealed class DieuChinhHanhTrinhV43:MonoBehaviour
{
 public DieuPhoiHanhTrinh story;
 public Vector3 haOrigin;
 public Vector3[] shoulders;
 public Transform[] upper,lower,hands;
 public Transform haHead,parcel;
 public bool externalGaze;
 float began=-1,poseElapsed=-1;int region=-99,completedCycles;Quaternion headRest;
 StreamWriter motion;
 public int Cycles=>completedCycles;
 public float ContactError {get;private set;}
 public float PeakTravel {get;private set;}
 [Serializable] public class SavedAct {public float elapsed=-1;public int region,cycles;public float peak;}
 public SavedAct CaptureAct()=>new SavedAct{elapsed=began<0?-1:Mathf.Max(0,poseElapsed),region=region,cycles=completedCycles,peak=PeakTravel};
 public void RestoreAct(SavedAct s){region=s.region;poseElapsed=s.elapsed;began=s.elapsed<0?-1:Time.time-s.elapsed;completedCycles=s.cycles;PeakTravel=s.peak;}
 public void ResetAct(){region=-99;began=poseElapsed=-1;completedCycles=0;PeakTravel=0;}
 public static void Segment(Transform part,Vector3 a,Vector3 b)
 {part.position=(a+b)*.5f;part.up=b-a;var scale=part.localScale;part.localScale=new Vector3(scale.x,(b-a).magnitude*.5f/Mathf.Abs(part.parent.lossyScale.y),scale.z);}
 public void PreviewHa(){Pose(0);}
 void Start()
 {
  headRest=haHead.localRotation;var args=Environment.GetCommandLineArgs();int i=Array.IndexOf(args,"--verification-output");
  if(i>=0){motion=new StreamWriter(Path.Combine(args[i+1],"ha-motion.csv"));motion.WriteLine("time,cycle,phase,parcel_x,parcel_y,parcel_z,left_contact_error,right_contact_error,control_enabled");}
 }
 void Pose(float t)
 {
  // Identical pick -> carry -> replace path twice. The hands stay on the parcel;
  // elbow planes and a small torso/head attention change make the task legible.
  float phase=t<8?Mathf.Repeat(t,4):0;
  float lift=Mathf.SmoothStep(0,1,Mathf.Clamp01((phase-.35f)/.7f))*(1-Mathf.SmoothStep(0,1,Mathf.Clamp01((phase-2.8f)/.8f)));
  float across=Mathf.Sin(Mathf.Clamp01(phase/4)*Mathf.PI);across=across*across*.25f;
  var rest=haOrigin+new Vector3(0,.82f,-.32f);parcel.position=rest+new Vector3(across,lift*.26f,-lift*.08f);parcel.rotation=Quaternion.Euler(0,0,-lift*7);
  ContactError=0;for(int i=0;i<2;i++)
  {
   int side=i==0?-1:1;var target=parcel.TransformPoint(new Vector3(side*.49f,0,0));var shoulder=shoulders[i];var mid=(shoulder+target)*.5f;
   var elbow=mid+new Vector3(side*.095f,-.09f,.12f);Segment(upper[i],shoulder,elbow);Segment(lower[i],elbow,target);
   hands[i].SetPositionAndRotation(target,parcel.rotation);ContactError=Mathf.Max(ContactError,Vector3.Distance(hands[i].position,target));
  }
  PeakTravel=Mathf.Max(PeakTravel,Vector3.Distance(rest,parcel.position));
  if(Application.isPlaying)
  {
   if(!externalGaze)haHead.localRotation=headRest*Quaternion.Euler(lift*7,-lift*5,0);
   if(externalGaze)story.GetComponent<TraiNghiemHanhTrinhV42>().RecordSortingTravel(Vector3.Distance(rest,parcel.position));
   if(motion!=null&&story.data.region==1&&Time.frameCount%4==0)motion.WriteLine(FormattableString.Invariant($"{Time.realtimeSinceStartup:F5},{Mathf.Min(2,(int)(t/4))},{phase:F3},{parcel.position.x:F5},{parcel.position.y:F5},{parcel.position.z:F5},{ContactError:F6},{ContactError:F6},{story.player.enabled}"));
  }
 }
 void LateUpdate()
 {
  if(story.Paused)return;
  if(region!=story.data.region){region=story.data.region;began=-1;completedCycles=0;}
  if(region!=1)return;
  var source=story.GetComponent<TraiNghiemHanhTrinhV42>();
  if(began<0&&source.HaEvents>0){began=Time.time;story.polish.Event("Ha_v43_action_begin","two handed pick-carry-replace; two identical cycles; optional no reward");}
  float t=began<0?0:Time.time-began;Pose(t);poseElapsed=t;
  int done=began<0?0:Mathf.Min(2,(int)(t/4));
  if(done>completedCycles){completedCycles=done;story.PlayEffectAt(story.completion.cloth,parcel.position);story.polish.Event("Ha_v43_cycle_replaced",done.ToString());}
 }
 void OnDestroy(){motion?.Dispose();}
 void OnApplicationQuit(){motion?.Flush();}
}
