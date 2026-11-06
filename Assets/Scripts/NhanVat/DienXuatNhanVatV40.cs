// Điều khiển nhìn và clip thân actor; không tranh kênh với crossfade mặt.
// Component trong scene r92: AnhTung_Cousin, CoMai_Former_Teacher, CR_Bao, CR_Ha.
// Reference public để đối chiếu Inspector (có nhánh tùy chọn): story, gaze, body.
// Thứ tự chạy khai báo: 20500; giữ quan hệ khởi tạo/pose với các owner khác.
using UnityEngine;

// A centered gaze pivot and authored body clips share no channel with face crossfades.
[DefaultExecutionOrder(20500)]
[UnityEngine.Scripting.APIUpdating.MovedFrom(true, null, "Assembly-CSharp", "ActorPerformanceV40")]
public sealed class DienXuatNhanVatV40 : MonoBehaviour
{
 public DieuPhoiHanhTrinh story;
 public Transform gaze;
 public string speaker;
 public Animation body;
 Quaternion rest;
 Vector3 forward;
 float engagedUntil;
 string previous="";
 void Start(){rest=gaze.rotation;forward=Vector3.back;}
 void LateUpdate()
 {
  if(story.Paused||!gaze||!gameObject.activeInHierarchy)return;
  if(story.Dialogue!=previous){previous=story.Dialogue;if(previous.StartsWith(speaker)){engagedUntil=Time.time+Mathf.Max(5,previous.Length/14f);if(body&&body.GetClip("speak"))body.CrossFade("speak",.35f);}}
  bool speaking=Time.time<engagedUntil;
  var delta=story.player.transform.position+Vector3.up*.65f-gaze.position;
  float proximity=delta.magnitude;
  // Natural attention within a conversation; never spin a whole character through a wall.
  var flat=new Vector3(delta.x,0,delta.z).normalized;
  float yaw=Mathf.Clamp(Vector3.SignedAngle(forward,flat,Vector3.up),-38,38);
  float pitch=Mathf.Clamp(Mathf.Atan2(delta.y,new Vector2(delta.x,delta.z).magnitude)*Mathf.Rad2Deg,-12,12);
  var desired=proximity<5?Quaternion.AngleAxis(yaw,Vector3.up)*Quaternion.AngleAxis(pitch+(speaking?Mathf.Sin(Time.time*2.2f)*1.7f:0),Vector3.right)*rest:rest;
  gaze.rotation=Quaternion.Slerp(gaze.rotation,desired,1-Mathf.Exp(-Time.deltaTime*2.3f));
  if(!speaking&&body&&body.GetClip("idle")&&!body.IsPlaying("idle"))body.CrossFade("idle",.5f);
 }
}
