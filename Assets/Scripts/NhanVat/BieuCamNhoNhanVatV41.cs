// Chớp mắt/miệng nhỏ sau diễn xuất chính; tìm các phần mắt/miệng trong con của actor.
// Component trong scene r92: AnhTung_Cousin, CoMai_Former_Teacher, Lan_Garden_World_Rig, CR_Bao, CR_Ha.
// Reference public để đối chiếu Inspector (có nhánh tùy chọn): story.
// Thứ tự chạy khai báo: 22500; giữ quan hệ khởi tạo/pose với các owner khác.
using System.Linq;
using UnityEngine;
[DefaultExecutionOrder(22500)]
[UnityEngine.Scripting.APIUpdating.MovedFrom(true, null, "Assembly-CSharp", "ActorMicroPerformanceV41")]
public sealed class BieuCamNhoNhanVatV41:MonoBehaviour
{
 public DieuPhoiHanhTrinh story;
 Transform[] eyes;Vector3[] eyeScale;Transform mouth;Vector3 mouthScale;
 void Start()
 {
  eyes=GetComponentsInChildren<Transform>(true).Where(t=>t.name.Contains("_Eye")).ToArray();eyeScale=eyes.Select(t=>t.localScale).ToArray();
  mouth=GetComponentsInChildren<Transform>(true).FirstOrDefault(t=>t.name=="Closed_Mouth");if(mouth)mouthScale=mouth.localScale;
 }
 void LateUpdate()
 {
  if(story.Paused)return;
  bool blink=Mathf.Repeat(Time.time+transform.position.x,4.7f)<.13f;
  for(int n=0;n<eyes.Length;n++)if(eyes[n])eyes[n].localScale=Vector3.Scale(eyeScale[n],new Vector3(1,blink?.12f:1,1));
  if(mouth)
  {
   bool near=Vector3.Distance(story.player.transform.position,transform.position)<4;
   float opening=near&&story.polish.DialogueVisible?1+Mathf.Abs(Mathf.Sin(Time.time*8))*1.8f:1;
   mouth.localScale=Vector3.Scale(mouthScale,new Vector3(1,opening,1));
  }
 }
}
