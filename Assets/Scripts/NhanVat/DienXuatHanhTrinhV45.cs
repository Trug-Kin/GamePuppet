// Diễn xuất và bước chân nhân vật theo vùng, phối hợp rig/clip hiện có.
// Component trong scene r92: Lake_Market_Story_GDD21.
// Reference public để đối chiếu Inspector (có nhánh tùy chọn): actor, frame, head, torso, upper, fore, palm, eyes, brows, thighs, shins, shoes, story, driven.
// Code tự tìm/tạo: TraiNghiemHanhTrinhV42, DieuChinhHanhTrinhV43, Camera, TuTheBaoBenCauV48, DieuKhienDiChuyen.
// Thứ tự chạy khai báo: 25000; giữ quan hệ khởi tạo/pose với các owner khác.
using System;
using System.IO;
using UnityEngine;
using Unity.Profiling;

// NPCs only. No ConRoiAI, puppet pivot or puppet gesture is registered here.
[DefaultExecutionOrder(25000)]
[UnityEngine.Scripting.APIUpdating.MovedFrom(true, null, "Assembly-CSharp", "JourneyActingV45")]
public sealed class DienXuatHanhTrinhV45:MonoBehaviour
{
 [Serializable] public class Rig
 {
  public string role,speaker;public Transform actor,frame,head,torso;
  public Transform[] upper,fore,palm,eyes,brows;public Transform[] thighs,shins,shoes;
  public Vector3[] shoulder;public Vector3[] eyeRest,browRest;public Quaternion headRest,torsoRest;
  public float shoulderY;public bool customArms;public Quaternion actorRest,frameInActor;
  [NonSerialized] public float attention,phrase,clock,gesture;[NonSerialized] public string line="";
 }
 [Serializable] public class Pose {public Vector3 p,s;public Quaternion q;}
 [Serializable] public class Foot {public Vector3 planted,start,end,previousVisual;public bool swing,wasStance;}
 [Serializable] public class Saved
 {
  public int version=2,region;public float vyTime=-1,walked,lanTime=-1;public int steps;
  public Foot[] feet;public Pose[] poses;public float[] attention,phrase,clock,gesture;
  public TraiNghiemHanhTrinhV42.SavedAct observed;public DieuChinhHanhTrinhV43.SavedAct sorting;
 }
 public DieuPhoiHanhTrinh story;public Rig[] rigs;public Transform[] driven;
 public AudioClip footClip;public Transform can,spout,rod;
 public Vector3 vyOrigin;public Quaternion vyWalkingRotation;public float vyRootGroundOffset;
 public int Footsteps {get;private set;}public float PlantSlip {get;private set;}public float GroundError {get;private set;}
 public float Walked=>walked;public float VyTime=>vyTime;public bool VyMoving=>vyTime>2&&vyTime<7;
 public float MaxHandBodyOverlap {get;private set;}
 public bool Ready=>ready;
 float vyTime=-1,walked,lanTime=-1;int region=-99;bool ready;
 Foot[] feet={new Foot(),new Foot()};Vector3 previousVy;bool previousVisible;
 AudioSource footVoice;Camera eye;TraiNghiemHanhTrinhV42 observed;DieuChinhHanhTrinhV43 sorting;
 StreamWriter telemetry;float nextSample;int frameSteps;
 TuTheBaoBenCauV48 baoBridge;
 readonly RaycastHit[] hits=new RaycastHit[32];
 static readonly ProfilerMarker actorMarker=new ProfilerMarker("V45.NPCActing");
 Rig Vy=>rigs[5];
 void Start()
 {
  observed=GetComponent<TraiNghiemHanhTrinhV42>();sorting=GetComponent<DieuChinhHanhTrinhV43>();eye=story.player.GetComponentInChildren<Camera>();baoBridge=GetComponent<TuTheBaoBenCauV48>();
  foreach(var r in rigs){r.headRest=r.head.localRotation;if(r.torso)r.torsoRest=r.torso.localRotation;}
  var args=Environment.GetCommandLineArgs();int n=Array.IndexOf(args,"--verification-output");if(n>=0){telemetry=new StreamWriter(Path.Combine(args[n+1],"acting-v45.csv"));telemetry.WriteLine("frame,time,region,vy_visible,vy_elapsed,walk_m,footsteps,foot_audio,plant_slip_m,ground_error_m,hand_overlap_m,lan_attention,tung_attention");}
  ready=true;Applied(true);
 }
 public void Applied(bool relocate)
 {
  if(!ready)return;
  StopFoot();
  if(!relocate&&!string.IsNullOrEmpty(story.data.npcPerformance))
  {
   Saved s=null;try{s=JsonUtility.FromJson<Saved>(story.data.npcPerformance);}catch(ArgumentException){}
   if(s!=null&&s.version==2&&s.poses?.Length==driven.Length&&s.attention?.Length==rigs.Length&&s.gesture?.Length==rigs.Length&&s.phrase?.Length==rigs.Length&&s.clock?.Length==rigs.Length&&s.observed!=null&&s.sorting!=null&&s.feet?.Length==2)
   {
    region=s.region;vyTime=s.vyTime;walked=s.walked;lanTime=s.lanTime;Footsteps=s.steps;feet=s.feet;
    for(int i=0;i<driven.Length;i++){driven[i].localPosition=s.poses[i].p;driven[i].localRotation=s.poses[i].q;driven[i].localScale=s.poses[i].s;}
    for(int i=0;i<rigs.Length;i++){rigs[i].attention=s.attention[i];rigs[i].phrase=s.phrase[i];rigs[i].clock=s.clock[i];rigs[i].gesture=s.gesture[i];rigs[i].line=story.Dialogue;}
    observed.RestoreAct(s.observed);sorting.RestoreAct(s.sorting);previousVisible=region==1&&vyTime>=0;Vy.actor.gameObject.SetActive(previousVisible);previousVy=Vy.actor.position;return;
   }
  }
  region=story.data.region;vyTime=lanTime=-1;walked=0;Footsteps=0;previousVisible=false;Vy.actor.gameObject.SetActive(false);observed.ResetAct();sorting.ResetAct();
  foreach(var r in rigs){r.attention=r.phrase=r.clock=r.gesture=0;r.line="";r.head.localRotation=r.headRest;if(r.torso)r.torso.localRotation=r.torsoRest;}
 }
 public string Capture()
 {
  if(!ready)return "";
  var s=new Saved{region=region,vyTime=vyTime,walked=walked,lanTime=lanTime,steps=Footsteps,feet=feet,poses=new Pose[driven.Length],attention=new float[rigs.Length],phrase=new float[rigs.Length],clock=new float[rigs.Length],gesture=new float[rigs.Length],observed=observed.CaptureAct(),sorting=sorting.CaptureAct()};
  for(int i=0;i<driven.Length;i++)s.poses[i]=new Pose{p=driven[i].localPosition,q=driven[i].localRotation,s=driven[i].localScale};
  for(int i=0;i<rigs.Length;i++){s.attention[i]=rigs[i].attention;s.phrase[i]=rigs[i].phrase;s.clock[i]=rigs[i].clock;s.gesture[i]=rigs[i].gesture;}
  return JsonUtility.ToJson(s);
 }
 float Ground(Vector3 p)
 {
  int count=Physics.RaycastNonAlloc(p+Vector3.up*.8f,Vector3.down,hits,2,~0,QueryTriggerInteraction.Ignore);float best=float.PositiveInfinity,y=p.y;
  for(int i=0;i<count;i++){var h=hits[i];if(h.normal.y<.7f||h.collider.GetComponentInParent<DieuKhienDiChuyen>()||h.transform.IsChildOf(Vy.actor))continue;if(h.distance<best){best=h.distance;y=h.point.y;}}
  return y;
 }
 static float Travel(float t)
 {
  // Integral of a trapezoidal velocity profile, eased at both ends. 1.8 m/5 s.
  t=Mathf.Clamp(t-2,0,5);const float ramp=.5f;const float speed=.4f;
  if(t<ramp)return speed*t*t/(2*ramp);if(t>5-ramp){float u=5-t;return 1.8f-speed*u*u/(2*ramp);}return speed*(t-ramp*.5f);
 }
 void UpdateVy(float dt)
 {
  bool market=story.data.region==1&&string.IsNullOrEmpty(story.data.ending);
  if(!market){Vy.actor.gameObject.SetActive(false);StopFoot();previousVisible=false;return;}
  if(vyTime<0&&!story.data.vyEncountered&&Vector3.Distance(story.player.transform.position,vyOrigin)<12)
  {
   story.data.vyEncountered=true;vyTime=0;walked=0;Vy.actor.rotation=vyWalkingRotation;
   Vy.actor.position=vyOrigin+Vector3.back*(2-Mathf.Clamp(story.data.ignored,0,2))*2;Vy.actor.gameObject.SetActive(true);
   for(int i=0;i<2;i++){var p=Vy.frame.TransformPoint(new Vector3(i==0?-.101f:.101f,0,0));p.y=Ground(p);feet[i]=new Foot{planted=p,start=p,end=p};}
   previousVy=Vy.actor.position;previousVisible=true;story.polish.Event("Vy_market_once","ordinary silhouette; contact-driven gait; no awareness reward");story.Save();
  }
  bool visible=vyTime>=0;
  if(visible&&vyTime>=7){var v=eye.WorldToViewportPoint(Vy.head.position);if(v.z<0||v.x<-.1f||v.x>1.1f||v.y<-.1f||v.y>1.1f){vyTime=-1;visible=false;}}
  Vy.actor.gameObject.SetActive(visible);
  if(!visible){StopFoot();previousVisible=false;return;}
  vyTime+=dt;float old=walked;walked=Travel(vyTime);var pRoot=vyOrigin+Vector3.back*(2-Mathf.Clamp(story.data.ignored,0,2))*2+Vector3.right*walked;pRoot.y=Ground(pRoot)+vyRootGroundOffset;Vy.actor.position=pRoot;
  PlantSlip=GroundError=0;
  for(int i=0;i<2;i++)
  {
   float phase=Mathf.Repeat(walked/.60f+i*.5f,1);var f=feet[i];Vector3 target;
   if(phase>=.60f&&walked<1.79999f)
   {
    if(!f.swing){f.swing=true;f.start=f.planted;f.end=Vy.frame.TransformPoint(new Vector3(i==0?-.101f:.101f,0,-.18f));f.end.y=Ground(f.end);}
    float u=(phase-.60f)/.40f;target=Vector3.Lerp(f.start,f.end,u*u*(3-2*u))+Vector3.up*(Mathf.Sin(u*Mathf.PI)*.075f);
   }
   else
   {
    if(f.swing){f.swing=false;f.planted=f.end;Footsteps++;frameSteps++;if(dt>0&&!story.Paused&&visible)footVoice=story.PlayContactFootstep(footClip,f.planted);story.polish.Event("Vy_foot_contact",i+" / "+walked.ToString("F3",System.Globalization.CultureInfo.InvariantCulture));}
    target=f.planted;GroundError=Mathf.Max(GroundError,Mathf.Abs(target.y-Ground(target)));
   }
   // Analytic two-bone knee solve: the stance target is a fixed world point.
   var hip=Vy.frame.TransformPoint(new Vector3(i==0?-.101f:.101f,.79f,0));var ankle=target+Vector3.up*.13f;var axis=(ankle-hip).normalized;float dist=Vector3.Distance(hip,ankle),a=.37f,b=.37f;
   float along=(a*a-b*b+dist*dist)/(2*Mathf.Max(.001f,dist));float bend=Mathf.Sqrt(Mathf.Max(0,a*a-along*along));var pole=Vector3.ProjectOnPlane(Vector3.right,axis).normalized;var knee=hip+axis*along+pole*bend;
   DieuChinhHanhTrinhV43.Segment(Vy.thighs[i],hip,knee);DieuChinhHanhTrinhV43.Segment(Vy.shins[i],knee,ankle);Vy.shoes[i].SetPositionAndRotation(target+Vector3.up*.067f,Vy.frame.rotation);
   if(!f.swing&&f.wasStance&&old>0&&previousVisible)PlantSlip=Mathf.Max(PlantSlip,Vector3.Distance(f.previousVisual,Vy.shoes[i].position-Vector3.up*.067f));
   f.previousVisual=Vy.shoes[i].position-Vector3.up*.067f;f.wasStance=!f.swing;
  }
  previousVy=pRoot;previousVisible=true;
 }
 void PoseRig(Rig r,float dt)
 {
  if(!r.actor.gameObject.activeInHierarchy)return;r.clock+=dt;
  bool own=story.polish.DialogueVisible&&story.Dialogue.StartsWith(r.speaker,StringComparison.Ordinal)&&r.role!="Vy";
  if(own&&r.line!=story.Dialogue){r.line=story.Dialogue;r.phrase=0;}
  r.phrase+=dt;float distance=Vector3.Distance(eye.transform.position,r.head.position);float target=own?1:distance<3.5f?.22f:0;
  r.attention=Mathf.MoveTowards(r.attention,target,dt*(target>r.attention?1.8f:1.35f));
  var delta=eye.transform.position-r.head.position;var local=r.frame.InverseTransformDirection(delta.normalized);
  float yaw=Mathf.Clamp(Mathf.Atan2(-local.x,-local.z)*Mathf.Rad2Deg,-32,32)*r.attention;
  float pitch=Mathf.Clamp(Mathf.Asin(Mathf.Clamp(local.y,-1,1))*Mathf.Rad2Deg,-10,10)*r.attention;
  // One finite nod and small turn per subtitle, no silent mouth loop.
  r.gesture=Mathf.MoveTowards(r.gesture,own?Mathf.Sin(Mathf.Clamp01(r.phrase/2.8f)*Mathf.PI):0,dt*1.4f);float beat=r.gesture;
  var desired=Quaternion.AngleAxis(yaw,Vector3.up)*Quaternion.AngleAxis(pitch+beat*(r.role=="Tung"?-3:2),r.frame.right)*(r.head.parent.rotation*r.headRest);
  r.head.rotation=Quaternion.Slerp(r.head.rotation,desired,1-Mathf.Exp(-dt*4));
  if(r.torso)r.torso.localRotation=Quaternion.Slerp(r.torso.localRotation,r.torsoRest*Quaternion.Euler(beat*1.3f,0,beat*(r.role=="Mai"?-1:1)),1-Mathf.Exp(-dt*3));
  float blinkPhase=Mathf.Repeat(r.clock+(r.role.Length*.31f),4.3f),blink=blinkPhase<.16f?Mathf.Abs(blinkPhase/.08f-1):1;
  for(int i=0;i<r.eyes.Length;i++){r.eyes[i].localPosition=r.eyeRest[i]+new Vector3(-yaw*.00013f,pitch*.00010f,0);var scale=r.eyes[i].localScale;scale.y=(r.role=="Lan"?.009f:.013f)*Mathf.Max(.10f,blink);r.eyes[i].localScale=scale;}
  for(int i=0;i<r.brows.Length;i++)r.brows[i].localPosition=r.browRest[i]+Vector3.up*(beat*(r.role=="Tung"?-.003f:.002f));
  if(!r.customArms)return;
  if(r.role=="Bao"&&baoBridge&&baoBridge.leaningOnly)return;
  for(int i=0;i<2;i++)
  {
   int side=i==0?-1:1;var sh=r.shoulder[i]+Vector3.up*(beat*.009f);float gesture=(i==1?beat*r.attention:0);
   var wrist=new Vector3(side*(r.role=="Tung"?.29f:.265f),r.shoulderY-.54f+gesture*.17f,-.10f-gesture*.18f);
   if(r.role=="Vy"&&i==1)wrist=new Vector3(.17f,1.19f,-.19f);
   if(r.role=="Lan"&&i==0){float water=(1-r.attention)*(.5f+.5f*Mathf.Sin(r.clock*.8f));wrist=new Vector3(-.34f,.88f+water*.035f,-.29f);}
   if(r.role=="Vy"&&i==0)wrist+=new Vector3(0,0,Mathf.Sin(walked/.60f*Mathf.PI*2)*.045f);
   var elbow=new Vector3(side*(Mathf.Abs(sh.x)+.08f),r.shoulderY-.30f,-.035f-gesture*.025f);
   var a=r.frame.TransformPoint(sh);var b=r.frame.TransformPoint(elbow);var c=r.frame.TransformPoint(wrist);
   if(r.role=="Bao"&&i==0&&rod){c=rod.TransformPoint(new Vector3(0,-.90f,0));b=Vector3.Lerp(a,c,.5f)+r.frame.TransformVector(new Vector3(-.08f,-.035f,.07f));}
   DieuChinhHanhTrinhV43.Segment(r.upper[i],a,b);DieuChinhHanhTrinhV43.Segment(r.fore[i],b,c);r.palm[i].SetPositionAndRotation(c,r.frame.rotation*Quaternion.Euler(-gesture*18,0,side*gesture*12));
   float penetration=Mathf.Max(0,.155f-Mathf.Abs(wrist.x));MaxHandBodyOverlap=Mathf.Max(MaxHandBodyOverlap,penetration);
   if(r.role=="Lan"&&i==0&&can){can.rotation=r.frame.rotation*Quaternion.Euler(0,0,-(1-r.attention)*8);can.position=c-can.TransformVector(new Vector3(0,.11f,0));}
  }
 }
 void LateUpdate()
 {
  if(!ready)return;if(region!=story.data.region)Applied(true);
  if(story.Paused||story.Loading){StopFoot();return;}
  using(actorMarker.Auto())
  {
   float dt=Time.deltaTime;frameSteps=0;UpdateVy(dt);MaxHandBodyOverlap=0;
   var lan=rigs[4];bool lanTalk=story.polish.DialogueVisible&&story.Dialogue.StartsWith(lan.speaker,StringComparison.Ordinal);var flat=eye.transform.position-lan.actor.position;flat.y=0;
   var lanQ=lanTalk&&flat.sqrMagnitude>.01f?Quaternion.LookRotation(-flat.normalized)*Quaternion.Inverse(lan.frameInActor):lan.actorRest;
   lan.actor.rotation=Quaternion.RotateTowards(lan.actor.rotation,lanQ,dt*65);
   foreach(var r in rigs)PoseRig(r,dt);
  }
  if(telemetry!=null&&Time.unscaledTime>=nextSample){nextSample=Time.unscaledTime+1f/30;telemetry.WriteLine(FormattableString.Invariant($"{Time.frameCount},{Time.realtimeSinceStartup:F6},{region},{Vy.actor.gameObject.activeInHierarchy},{vyTime:F5},{walked:F6},{Footsteps},{FootAudio},{PlantSlip:F7},{GroundError:F6},{MaxHandBodyOverlap:F6},{rigs[4].attention:F4},{rigs[3].attention:F4}"));if(Time.frameCount%300==0)telemetry.Flush();}
 }
 void OnDestroy(){telemetry?.Dispose();}
 void OnApplicationQuit(){telemetry?.Flush();}
 public bool FootAudio=>footVoice&&footVoice.clip==footClip&&footVoice.isPlaying;
 public bool FootPlanted(int i)=>!feet[i].swing;
 void StopFoot(){if(FootAudio)footVoice.Stop();footVoice=null;}
}
