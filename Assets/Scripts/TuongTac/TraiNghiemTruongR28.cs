// Điều phối diễn xuất trường/lớp 203; partial R30 dùng chung owner staging và trạng thái lưu.
// Component trong scene r92: Lake_Market_Story_GDD21.
// Reference public để đối chiếu Inspector (có nhánh tùy chọn): story, puppetPaper, responsePaper, responseDesk, puppetDesk, traceChair, leadUpper, leadFore, leadHand, leadShoulder, pushPivotR91.
// Code tự tìm/tạo: TuongTacGiayLop203R32, Camera, MeshFilter.
// Thứ tự chạy khai báo: 21000; giữ quan hệ khởi tạo/pose với các owner khác.
using UnityEngine;

// Opt-in only on r28. Uses existing school save fields; no controller/camera writes.
[DefaultExecutionOrder(21000)]
[UnityEngine.Scripting.APIUpdating.MovedFrom(true, null, "Assembly-CSharp", "R28SchoolExperience")]
public sealed partial class TraiNghiemTruongR28:MonoBehaviour
{
 public DieuPhoiHanhTrinh story;
 // Serialized opt-in: r28 checkpoint keeps its original behavior.
 public bool blenderSchoolR29;
 float responseBlend=1;
 public bool CanRespond=>stagedSchoolR30?((Leading&&elapsed>=1.8f)||r30.finished):!blenderSchoolR29||(Leading&&elapsed>=1.35f);
 public Transform puppetPaper,responsePaper,responseDesk,puppetDesk,traceChair;
 public Vector3 puppetPaperRest,responsePaperRest;
 public Quaternion chairWindow,chairBoard;
 public Transform leadUpper,leadFore,leadHand,leadShoulder;
 public Transform pushPivotR91;
 public TextAsset pushSourceR91;
 [System.Serializable] sealed class PushSource {public float duration;public Vector3 axis;public float[] angles;public float upperLength,foreLength;}
 PushSource push;
 public bool Leading=>story.data.region==2&&elapsed>=0;
 public float LeadSeconds=>Mathf.Max(0,elapsed);
 public float ObservedSeconds {get;private set;}
 public int Cycles {get;private set;}
 public int Responses {get;private set;}
 public float LastResponseDelay {get;private set;}=-1;
 public string Objective=>GetComponent<TuongTacGiayLop203R32>()?GetComponent<TuongTacGiayLop203R32>().Objective:blenderSchoolR29?(story.player.transform.position.y<2.8f?"Lên cầu thang trái → lớp 203.":"Vào lớp 203. Có thể xem giấy, gặp Cô Mai hoặc về phòng trọ."):story.player.transform.position.y<2.8f?"Qua sân đến sảnh. Cầu thang bên trái dẫn lên hành lang tầng 2 → lớp 203.\nCó thể trở về phòng trọ ở bảng TỈNH DẬY cạnh cổng.":"Theo biển 203 vào lớp. Cô Mai đứng cạnh bảng; giấy trên bàn có thể dịch sang khay bên phải.\nTrò chuyện và quan sát là tùy chọn. Biển TỈNH DẬY ở cửa lớp dẫn về phòng trọ.";
 float elapsed=-1,idle;int loads=-1;bool active,responded;
 Quaternion armRest,headRest;bool initialized;
 Camera eye;
 void Init(){if(initialized)return;initialized=true;eye=story.player.GetComponentInChildren<Camera>();armRest=story.schoolChapter.puppet.gesturePart.localRotation;headRest=story.schoolChapter.puppet.dauTransform.localRotation;}
 public void Applied(bool enabled){Init();if(stagedSchoolR30){RestoreR30(enabled);return;}active=enabled;loads=story.SuccessfulLoads;idle=0;responded=false;responseBlend=1;ObservedSeconds=0;elapsed=enabled&&story.data.schoolLeading?Mathf.Clamp(story.data.schoolLeadElapsed,0,8):-1;Pose();}
 public void Capture(){if(stagedSchoolR30){CaptureR30();return;}story.data.schoolLeading=Leading;story.data.schoolLeadElapsed=Leading?Mathf.Min(elapsed,8):0;}
 bool Visible(Transform t){
  var p=eye.WorldToViewportPoint(t.position);if(p.z<0||p.z>12||p.x<.04f||p.x>.96f||p.y<.06f||p.y>.94f)return false;
  var d=t.position-eye.transform.position;
  foreach(var h in Physics.RaycastAll(eye.transform.position,d.normalized,d.magnitude-.06f,~0,QueryTriggerInteraction.Ignore))if(!h.transform.IsChildOf(story.player.transform)&&!h.transform.IsChildOf(story.schoolChapter.puppet.transform)&&!h.transform.IsChildOf(puppetDesk))return false;
  return true;
 }
 void Update(){
  if(stagedSchoolR30){UpdateR30();return;}
  if(!initialized)Init();if(story.data.region!=2){if(active){active=false;elapsed=-1;}return;}
  if(!active||loads!=story.SuccessfulLoads)Applied(true);
  if(story.Paused||story.Loading||story.MenuOpen||story.PhoneOpen||story.polish.IsReading)return;
  var ch=story.schoolChapter;
  if(ch.clockHand)ch.clockHand.Rotate(0,0,-Time.deltaTime*6,Space.Self);
  bool near=Vector3.Distance(story.player.transform.position,responseDesk.position)<6.2f;
  bool moving=story.player.LastMovementInput.sqrMagnitude>.01f;
  if(!Leading){idle=near&&!moving?idle+Time.deltaTime:0;if(idle>=1.2f){elapsed=0;ObservedSeconds=0;responded=false;Cycles++;}}
  else {
   elapsed+=Time.deltaTime;
   if(elapsed<1.9f&&Visible(puppetPaper)&&Visible(ch.puppet.dauTransform))ObservedSeconds+=Time.deltaTime;
   // Hold indefinitely while the player stands. Movement freely resets the attempt.
   if(moving&&elapsed>1.9f){elapsed=-1;idle=0;}
  }
  if(!story.data.schoolTrace&&Vector3.Distance(story.player.transform.position,traceChair.position)<2.4f){story.data.schoolTrace=true;story.data.awareness++;story.PlayEffect(story.wood);}
  traceChair.rotation=story.data.schoolTrace?chairBoard:chairWindow;
  if(blenderSchoolR29&&responded)responseBlend=Mathf.Min(1,responseBlend+Time.deltaTime/.65f);
  Pose();
 }
 void Pose(){
  if(!initialized)return;
  float t=Leading?Mathf.SmoothStep(0,1,Mathf.Clamp01(elapsed/(stagedSchoolR30?1.8f:1.35f))):0;
  var ch=story.schoolChapter;
  if(pushPivotR91&&pushSourceR91){
   if(push==null)push=JsonUtility.FromJson<PushSource>(pushSourceR91.text);
   float sample=Leading?Mathf.Clamp01(elapsed/push.duration)*(push.angles.Length-1):0;int k=Mathf.Min((int)sample,push.angles.Length-2);
   pushPivotR91.localRotation=Quaternion.AngleAxis(Mathf.Lerp(push.angles[k],push.angles[k+1],sample-k),push.axis);
  }
  ch.puppet.gesturePart.localRotation=armRest*Quaternion.Euler(-58*t,0,-48*t);
  if(Leading){var d=story.player.transform.position+Vector3.up*.65f-ch.puppet.dauTransform.position;if(d.sqrMagnitude>.001f)ch.puppet.dauTransform.rotation=Quaternion.LookRotation(d);}
  else ch.puppet.dauTransform.localRotation=headRest;
  puppetPaper.localPosition=puppetPaperRest+Vector3.right*(.46f*t);
  // Paper stays on the desk; the hand reaches, makes contact, then slides with it.
  if(leadUpper&&leadFore&&leadHand&&leadShoulder){
   var a=leadShoulder.position;var rest=a+new Vector3(.04f,-.53f,-.08f);
   float reach=Leading?Mathf.SmoothStep(0,1,Mathf.Clamp01(stagedSchoolR30?(elapsed-.30f)/.60f:elapsed/.65f)):0;
   float slide=Leading?Mathf.SmoothStep(0,1,Mathf.Clamp01(stagedSchoolR30?(elapsed-.90f)/.90f:(elapsed-.65f)/.70f)):0;
   if(stagedSchoolR30&&r30.finished){slide=1;if(!Leading)reach=0;}
   puppetPaper.localPosition=puppetPaperRest+new Vector3(.46f,.016f,0)*slide;
   var wrist=Vector3.Lerp(rest,puppetPaper.position+new Vector3(push!=null?-.12f:-.06f,push!=null?.025f:.045f,push!=null?.10f:.03f),reach);
   var elbow=Vector3.Lerp(a,wrist,.51f)+new Vector3(.10f,-.06f,.04f);
   if(push!=null){
    var delta=wrist-a;float distance=Mathf.Clamp(delta.magnitude,.025f,push.upperLength+push.foreLength-.002f);var axis=delta.normalized;wrist=a+axis*distance;
    float along=(push.upperLength*push.upperLength-push.foreLength*push.foreLength+distance*distance)/(2*distance);
    var pole=Vector3.ProjectOnPlane(Vector3.forward+Vector3.up*.15f,axis).normalized;
    elbow=a+axis*along+pole*Mathf.Sqrt(Mathf.Max(0,push.upperLength*push.upperLength-along*along));
    FitLinkR91(leadUpper,a,elbow);FitLinkR91(leadFore,elbow,wrist);
    leadHand.SetPositionAndRotation(wrist,Quaternion.Slerp(Quaternion.identity,Quaternion.LookRotation(Vector3.up,Vector3.left),reach));
   }else{Segment(leadUpper,a,elbow);Segment(leadFore,elbow,wrist);leadHand.position=wrist;}
  }
  float responseAmount=(responded||(story.data.schoolReadMask&64)!=0)?(blenderSchoolR29?Mathf.SmoothStep(0,1,responseBlend):1):0;
  if(!paperOwnsResponseR34)responsePaper.localPosition=responsePaperRest+new Vector3(.46f,.016f,0)*responseAmount;
 }
 static void Segment(Transform part,Vector3 a,Vector3 b){part.position=(a+b)*.5f;part.up=b-a;var sc=part.localScale;part.localScale=new Vector3(sc.x,(a-b).magnitude,sc.z);}
 static void FitLinkR91(Transform part,Vector3 a,Vector3 b){
  part.up=b-a;var bounds=part.GetComponent<MeshFilter>().sharedMesh.bounds;
  float parentAxis=part.parent?part.parent.TransformVector(part.localRotation*Vector3.up).magnitude:1;
  var scale=part.localScale;scale.y=Vector3.Distance(a,b)/(bounds.size.y*parentAxis);part.localScale=scale;
  part.position=(a+b)*.5f-part.TransformVector(bounds.center);
 }
 public void Respond(){
  if(stagedSchoolR30){RespondR30();return;}
  if(story.data.region!=2||story.Paused||story.PhoneOpen)return;
  if(!CanRespond)return; // Object is not ready; walking/looking/exiting remain fully available.
  if((story.data.schoolReadMask&64)!=0)return; // A repeated E without a new physical paper move cannot earn witness.
  Responses++;responded=true;responseBlend=blenderSchoolR29?0:1;story.data.schoolReadMask|=64;if(!blenderSchoolR29)responsePaper.localPosition=responsePaperRest+new Vector3(.46f,.016f,0);
  if(Leading&&elapsed>=1.35f&&ObservedSeconds>=.75f){story.data.schoolWitnessed=true;LastResponseDelay=elapsed;}
  story.PlayEffectAt(story.wood,responsePaper.position);
 }
}
