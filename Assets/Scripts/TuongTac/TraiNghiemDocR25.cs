// Lớp trải nghiệm dốc R25, giữ cho checkpoint cũ.
// Component trong scene r92: Lake_Market_Story_GDD21.
// Reference public để đối chiếu Inspector (có nhánh tùy chọn): story.
// Code tự tìm/tạo: Camera, TuongTacNguoiChoi.
// Thứ tự chạy khai báo: 28000; giữ quan hệ khởi tạo/pose với các owner khác.
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

// r25 only: observation timing and presentation; no chapter/ending changes.
[DefaultExecutionOrder(28000)]
[UnityEngine.Scripting.APIUpdating.MovedFrom(true, null, "Assembly-CSharp", "R25SlopeExperience")]
public sealed class TraiNghiemDocR25:MonoBehaviour
{
 public DieuPhoiHanhTrinh story;public bool clearReview;
 public float BaselineVisibleSeconds{get;private set;}
 public float AfterVisibleSeconds{get;private set;}
 public bool DoorPromptVisible{get;private set;}
 Camera eye;Text[] labels;int previousRegion=-99,previousLoads=-1;
 void Start(){eye=story.player.GetComponentInChildren<Camera>();story.slopeChapter.CanObserveTrace=()=>BaselineVisibleSeconds>=1.1f&&SuitcaseVisible();}
 void OnDestroy(){if(story&&story.slopeChapter)story.slopeChapter.CanObserveTrace=null;}
 bool SuitcaseVisible(){
  var t=story.slopeChapter.suitcase;var at=t.position+Vector3.up*.25f;var p=eye.WorldToViewportPoint(at);var delta=at-eye.transform.position;
  if(p.z<=0||p.z>8||p.x<.12f||p.x>.88f||p.y<.1f||p.y>.9f)return false;
  foreach(var hit in Physics.RaycastAll(eye.transform.position,delta.normalized,delta.magnitude,~0,QueryTriggerInteraction.Ignore).OrderBy(h=>h.distance)){
   if(hit.transform.IsChildOf(story.player.transform))continue;
   return hit.transform.IsChildOf(t);
  }
  return true;
 }
 void LateUpdate(){
  if(!story||story.data==null||!eye)return;
  if(previousRegion!=story.data.region||previousLoads!=story.SuccessfulLoads){previousRegion=story.data.region;previousLoads=story.SuccessfulLoads;BaselineVisibleSeconds=AfterVisibleSeconds=0;}
  DoorPromptVisible=false;if(story.data.region!=3)return;
  if(!story.Paused&&!story.PhoneOpen&&!story.Loading&&SuitcaseVisible()){
   if(!story.data.slopeTrace)BaselineVisibleSeconds+=Time.deltaTime;else AfterVisibleSeconds+=Time.deltaTime;
  }
  // Keep the immediate lane clear; atmosphere only softens the distant continuation.
  RenderSettings.fog=!clearReview;RenderSettings.fogMode=FogMode.Linear;RenderSettings.fogStartDistance=38;RenderSettings.fogEndDistance=100;RenderSettings.fogColor=new Color(.43f,.49f,.48f);
  if(labels==null||labels.Length==0)labels=story.GetComponentsInChildren<Text>(true);
  var target=story.player.GetComponent<TuongTacNguoiChoi>().Target as TuongTacDocV39;
  foreach(var label in labels){
   if(label.text.Contains("Nhìn cửa, nhấn E để tiếp tục."))label.text=label.text.Replace("Nhìn cửa, nhấn E để tiếp tục.","Lối vào nằm cuối dốc.");
   if(label.transform.parent&&label.transform.parent.name=="Dialogue"&&label.text=="E: Mở cửa"&&(!target||!target.finalDoor||story.data.finalStarted||story.PhoneOpen||story.Paused))label.transform.parent.gameObject.SetActive(false);
   if(label.text.Contains("Một chi tiết quen thuộc đã thay đổi"))label.text="Có thể dừng lại quan sát đồ đạc bên đường.\nP · Xem lại việc trước mắt";
   if(label.transform.parent&&label.transform.parent.name=="Dialogue"&&target&&target.finalDoor&&!story.data.finalStarted&&!story.PhoneOpen&&!story.Paused&&!story.polish.IsReading&&!story.polish.DialogueVisible){label.text="E: Mở cửa";label.transform.parent.gameObject.SetActive(true);DoorPromptVisible=true;}
  }
 }
}
