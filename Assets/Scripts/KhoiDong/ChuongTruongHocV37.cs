// Chương trường tùy chọn; khi không có component, checkpoint cũ chỉ gồm hồ/chợ.
// Component trong scene r92: Lake_Market_Story_GDD21.
// Reference public để đối chiếu Inspector (có nhánh tùy chọn): story, environment, spawn, chair, ghost, clockHand, puppet, face, tick.
// Code tự tìm/tạo: TraiNghiemTruongR28, Camera, TuongTacGiayLop203R32.
using UnityEngine;

// Optional continuation, serialized only in the v37 scene. The v36 milestone
// remains limited to the lake and market when this component is absent.
[UnityEngine.Scripting.APIUpdating.MovedFrom(true, null, "Assembly-CSharp", "SchoolChapterV37")]
public sealed class ChuongTruongHocV37 : MonoBehaviour
{
 public DieuPhoiHanhTrinh story;
 public GameObject environment;
 public Transform spawn, chair, ghost, clockHand;
 public DieuKhienConRoi puppet;
 public HienGuongMatTheoKhu face;
 public AudioClip tick;
 public bool Leading => leadAt>=0;
 public float LeadSeconds => Leading?Time.time-leadAt:0;
 public int LeadCount {get;private set;}
 public int Talks {get;private set;}
 public float LastResponseDelay {get;private set;}=-1;
 Vector3 chairRestPosition;
 Quaternion chairRestRotation;
 float idle,leadAt=-1;
 Quaternion armRest;
 Quaternion headRest,maiHeadRest;
 float talkUntil;
 bool initialized;
 public void Apply(bool active)
 {
  if(!initialized)
  {
   chairRestPosition=chair.localPosition;chairRestRotation=chair.localRotation;
   armRest=puppet.gesturePart.localRotation;
   headRest=puppet.dauTransform.localRotation;maiHeadRest=face.ghostHead.localRotation;
   initialized=true;
  }
  StopAllCoroutines();idle=0;leadAt=-1;talkUntil=0;
  puppet.gesturePart.localRotation=armRest;puppet.dauTransform.localRotation=headRest;
  face.ghostHead.localRotation=maiHeadRest;face.Restore(story.data.schoolFace);
  environment.SetActive(active);
  if(active)
  {
   puppet.enabled=false;
   if(story.data.schoolLeading){leadAt=Time.time-story.data.schoolLeadElapsed;idle=1.5f;puppet.gesturePart.localRotation=armRest*Quaternion.Euler(story.completion?-62:-30,0,story.completion?-48:-25);}
   chair.localPosition=chairRestPosition;chair.localRotation=(story.data.schoolTrace?Quaternion.Euler(0,90,0):Quaternion.identity)*chairRestRotation;
  }
  var r28=GetComponent<TraiNghiemTruongR28>();if(r28&&r28.enabled)r28.Applied(active);
 }
 void Update()
 {
  var r28=GetComponent<TraiNghiemTruongR28>();if(r28&&r28.enabled)return;
  if(story.data.region!=2||!environment.activeInHierarchy||story.Paused||Time.timeScale==0)return;
  if(!story.polish)face.ghostHead.localRotation=maiHeadRest*Quaternion.Euler(Mathf.Sin(Time.time*3)*(Time.time<talkUntil?3:.3f),0,0);
  if(clockHand)clockHand.Rotate(0,0,-Time.deltaTime*6,Space.Self);
  var input=story.player.LastMovementInput;
  if(input.sqrMagnitude>.01f)
  {
   // A matching rightward response after the visible lead; early movement is
   // always allowed and simply ends this attempt. No delayed or forced input.
   if(Leading&&Time.time-leadAt>=1&&input.x>.5f&&Visible(puppet.gesturePart))
   {story.data.schoolWitnessed=true;LastResponseDelay=Time.time-leadAt;}
   idle=0;leadAt=-1;
   puppet.gesturePart.localRotation=Quaternion.RotateTowards(puppet.gesturePart.localRotation,armRest,Time.deltaTime*120);
  }
  else
  {
   idle+=Time.deltaTime;
   if(!Leading)puppet.gesturePart.localRotation=Quaternion.RotateTowards(puppet.gesturePart.localRotation,armRest,Time.deltaTime*120);
   if(idle>=1.5f&&!Leading){leadAt=Time.time;LeadCount++;}
   if(Leading)
   {
    puppet.gesturePart.localRotation=Quaternion.RotateTowards(puppet.gesturePart.localRotation,armRest*Quaternion.Euler(story.completion?-62:-30,0,story.completion?-48:-25),Time.deltaTime*75);
    var d=story.player.transform.position+Vector3.up*.7f-puppet.dauTransform.position;
    if(d.sqrMagnitude>.001f)puppet.dauTransform.rotation=puppet.dauTransform.name=="Minh_Head_Pivot"?Quaternion.LookRotation(d):Quaternion.FromToRotation(puppet.dauTransform.up,d.normalized)*puppet.dauTransform.rotation;
   }
  }
  if(!story.data.schoolTrace&&Vector3.Distance(story.player.transform.position,chair.position)<2.4f)
  {
   story.data.schoolTrace=true;story.data.awareness++;
   chair.localRotation=Quaternion.Euler(0,90,0)*chairRestRotation;
   story.PlayEffect(story.wood);StartCoroutine(Glance());
  }
 }
 bool Visible(Transform target)
 {
  var camera=story.player.GetComponentInChildren<Camera>();var p=camera.WorldToViewportPoint(target.position);
  if(p.z<=0||p.x<0||p.x>1||p.y<0||p.y>1||p.z>14)return false;
  var origin=camera.transform.position;var delta=target.position-origin;
  foreach(var hit in Physics.RaycastAll(origin,delta.normalized,delta.magnitude,~0,QueryTriggerInteraction.Ignore))
   if(!hit.collider.transform.IsChildOf(story.player.transform)&&!hit.collider.transform.IsChildOf(puppet.transform))return false;
  return true;
 }
 System.Collections.IEnumerator Glance()
 {
  var eye=story.player.GetComponentInChildren<Camera>().transform;
  for(float t=0;t<.18f;t+=Time.deltaTime){if(story.data.region!=2)yield break;eye.rotation=Quaternion.RotateTowards(eye.rotation,Quaternion.LookRotation(chair.position-eye.position),Time.deltaTime*9);yield return null;}
 }
 public void Talk()
 {
  Talks++;talkUntil=Time.time+4;
  int n=story.data.schoolTalks++;if(story.completion&&n>=4){story.data.schoolTalks=4;return;}
  string[] words={"Cô Mai: Cứ theo đáp án mẫu. Cô cũng từng nghĩ như thế là đủ.","Cô Mai: Hồi ấy em hay hỏi vì sao. Cô bảo để cuối giờ, rồi chuông reo.","Cô Mai: Trong sổ chỉ có một cột điểm. Những câu hỏi khác không có chỗ để ghi.","Cô Mai: Cô đã quay ghế ra cửa sổ. Sáng hôm sau, nó lại hướng về bảng."};
  story.Say(words[Mathf.Min(n,words.Length-1)]);
   var papers=GetComponent<TuongTacGiayLop203R32>();if(papers)story.Say(papers.completeR34?papers.MaiTalkR34():papers.MaiResponse+"\n"+words[Mathf.Min(n,words.Length-1)]);
  if(story.data.schoolWitnessed&&!story.data.schoolFace)
  {story.data.schoolFace=true;face.Reveal();story.PlayEffect(story.wood);}
 }
 public void Read(int index,string text)
 {
  if(index<0||index>=6)return;
  story.data.schoolReadMask|=1<<index;story.Say(text);
  if(story.polish)story.polish.Read("TƯ LIỆU GIẢNG ĐƯỜNG",text);
 }
 public void Capture()
 {
  var r28=GetComponent<TraiNghiemTruongR28>();if(r28&&r28.enabled){r28.Capture();return;}
  story.data.schoolLeading=story.data.region==2&&Leading;
  story.data.schoolLeadElapsed=Leading?Mathf.Min(LeadSeconds,30):0;
 }
}

