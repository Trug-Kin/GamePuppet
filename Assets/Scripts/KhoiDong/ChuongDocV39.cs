// Chương dốc; GĐ4 chỉ thuộc cuộc gặp cuối một lần, không thêm AI bắt chước khi khám phá.
// Component trong scene r92: Lake_Market_Story_GDD21.
// Reference public để đối chiếu Inspector (có nhánh tùy chọn): story, environment, spawn, suitcase, ghost, face, puppet, rollingWood, rollingSound.
// Code tự tìm/tạo: Camera, TuongTacDocV39, ChuanBiHanhLyR41.
using UnityEngine;

// Slope exploration has no imitation AI. GD4 exists only in the single final encounter.
[UnityEngine.Scripting.APIUpdating.MovedFrom(true, null, "Assembly-CSharp", "SlopeChapterV39")]
public sealed class ChuongDocV39 : MonoBehaviour
{
 public DieuPhoiHanhTrinh story;
 public GameObject environment;
 public Transform spawn,suitcase,ghost;
 public HienGuongMatTheoKhu face;
 public DieuKhienConRoi puppet;
 public Transform rollingWood;
 public AudioClip rollingSound;
 Quaternion caseRest,headRest;
 Quaternion ghostHeadRest;
 Vector3 woodRest;
 bool initialized;
 float rollTime;
 // Optional scene-owned observation rule. Legacy scenes retain proximity behavior.
 public System.Func<bool> CanObserveTrace;
 public bool FinalActive=>story.data.region==3&&story.data.finalStarted;
 public float FinalElapsed=>story.data.finalElapsed;
 public void Apply(bool active)
 {
  if(!initialized){initialized=true;caseRest=suitcase.localRotation;headRest=puppet.dauTransform.localRotation;ghostHeadRest=face.ghostHead.localRotation;woodRest=rollingWood.position;}
  StopAllCoroutines();face.Restore(story.data.slopeFace);environment.SetActive(active);
  puppet.enabled=false;puppet.gameObject.SetActive(active&&story.data.finalStarted);
  puppet.dauTransform.localRotation=headRest;
  suitcase.localRotation=caseRest*(story.data.slopeTrace?Quaternion.Euler(0,-12,0):Quaternion.identity);
  rollTime=story.data.slopeRollTime;rollingWood.position=woodRest+new Vector3(0,-.10f,-1)*Mathf.Min(rollTime%14,8)*2;
  rollingWood.gameObject.SetActive(rollTime%14<8);face.ghostHead.localRotation=ghostHeadRest;
 }
 bool InView(Transform target)
 {
  var camera=story.player.GetComponentInChildren<Camera>();var p=camera.WorldToViewportPoint(target.position);var d=target.position-camera.transform.position;
  if(p.z<=0||p.z>20||p.x<0||p.x>1||p.y<0||p.y>1)return false;
  foreach(var h in Physics.RaycastAll(camera.transform.position,d.normalized,d.magnitude,~0,QueryTriggerInteraction.Ignore))
  {
   if(h.collider.transform.IsChildOf(story.player.transform)||h.collider.transform.IsChildOf(puppet.transform)||h.collider.transform.IsChildOf(ghost))continue;
   var proxy=h.collider.GetComponent<TuongTacDocV39>();
   if(target.IsChildOf(ghost)&&proxy&&proxy.chapter==this&&!proxy.finalDoor&&proxy.memoryIndex<0)continue;
   return false;
  }
  return true;
 }
 void Update()
 {
  if(story.data.region!=3||story.Paused||Time.timeScale==0)return;
  if(!story.polish)face.ghostHead.localRotation=ghostHeadRest*Quaternion.Euler(Mathf.Sin(Time.time*1.3f)*.6f,0,0);
  if(!story.data.slopeTrace&&Vector3.Distance(story.player.transform.position,suitcase.position)<2.4f&&(CanObserveTrace==null||CanObserveTrace()))
  {story.data.slopeTrace=true;story.data.awareness++;suitcase.localRotation=caseRest*Quaternion.Euler(0,-12,0);story.PlayEffect(story.wood);StartCoroutine(Glance());}
  if(FinalActive)
  {
   var d=story.player.transform.position+Vector3.up*.65f-puppet.dauTransform.position;
   if(d.sqrMagnitude>.001f)puppet.dauTransform.rotation=Quaternion.Slerp(puppet.dauTransform.rotation,puppet.dauTransform.name=="Minh_Head_Pivot"?Quaternion.LookRotation(d):Quaternion.FromToRotation(puppet.dauTransform.up,d.normalized)*puppet.dauTransform.rotation,Time.deltaTime*2);
   story.data.finalElapsed+=Time.deltaTime;
   if(InView(puppet.dauTransform))story.data.slopeWitnessed=true;
   if(story.data.slopeWitnessed&&!story.data.slopeFace&&InView(face.ghostHead)){story.data.slopeFace=true;face.Reveal();story.PlayEffect(story.wood);}
   // The player can remain silent; this encounter never asks for a matching key.
   if(story.data.finalElapsed>=10)story.FinishJourney();
  }
  else
  {
   rollTime+=Time.deltaTime;
   // A distant wooden spool on its own gutter; its motion never blocks the walking lane.
   float cycle=rollTime%14;rollingWood.position=woodRest+new Vector3(0,-.10f,-1)*Mathf.Min(cycle,8)*2;
   rollingWood.gameObject.SetActive(cycle<8);rollingWood.Rotate(Vector3.right,Time.deltaTime*382,Space.World);
   if(cycle<Time.deltaTime)story.PlayEffect(rollingSound);
  }
 }
 System.Collections.IEnumerator Glance()
 {
  var eye=story.player.GetComponentInChildren<Camera>().transform;
  for(float t=0;t<.18f;t+=Time.deltaTime){if(story.data.region!=3)yield break;eye.rotation=Quaternion.RotateTowards(eye.rotation,Quaternion.LookRotation(suitcase.position-eye.position),Time.deltaTime*9);yield return null;}
 }
 public void Talk()
 {
  if(story.data.region!=3||story.Paused)return;
  var preparations=story.GetComponent<ChuanBiHanhLyR41>();if(preparations&&preparations.TryRespond())return;
  string[] lines={"Anh Tùng: Vali anh xếp xong lâu rồi. Cứ sáng mai lại tính.","Anh Tùng: Ở đây anh biết mọi ngã rẽ. Đi chỗ khác, nhỡ không làm được thì sao?","Anh Tùng: Không ai khóa cửa. Anh vẫn đứng chỗ này."};
  if(story.completion&&story.data.slopeTalks>=lines.Length)return;
  story.Say(lines[Mathf.Min(story.data.slopeTalks++,lines.Length-1)]);
 }
 public void BeginFinal()
 {
  if(story.data.region!=3||story.Paused||story.data.finalStarted)return;
  story.data.finalStarted=true;story.data.finalStarts=1;story.data.finalElapsed=0;
  puppet.gameObject.SetActive(true);puppet.enabled=false;story.Save();
 }
 public void Capture() {if(story.data.region==3)story.data.slopeRollTime=rollTime;}
}
