// Nội suy vertex fog URP theo vùng hồ/chợ; không dùng overlay che hình học gần.
// Component trong scene r92: Lake_Market_Story_GDD21.
// Reference public để đối chiếu Inspector (có nhánh tùy chọn): story.
// Code tự tìm/tạo: Camera.
// Thứ tự chạy khai báo: 26000; giữ quan hệ khởi tạo/pose với các owner khác.
using UnityEngine;

// URP's built-in vertex fog: no fullscreen pass, depth texture, particles or UI
// overlay. Profiles interpolate continuously while walking between lake/market.
[DefaultExecutionOrder(26000)]
[UnityEngine.Scripting.APIUpdating.MovedFrom(true, null, "Assembly-CSharp", "DalatMistV48")]
public sealed class SuongDaLatV48:MonoBehaviour
{
 public DieuPhoiHanhTrinh story;
 public bool clearGeometryReview;
 float density;
 Color color=new Color(.36f,.43f,.47f);
 Camera eye;
 void Awake(){eye=story.player.GetComponentInChildren<Camera>();}
 public void ApplyRegion()
 {
  if(!eye)eye=story.player.GetComponentInChildren<Camera>();
  // Keep geometry available behind fog; boundaries must be physical scenery.
  eye.farClipPlane=1000;
 }
 void LateUpdate()
 {
  if(!story||story.data==null)return;
  var d=story.data;bool room=d.region<0;
  float market=Mathf.SmoothStep(0,1,Mathf.InverseLerp(455,345,story.player.transform.position.z));
  float target=room?0:d.region==0?Mathf.Lerp(.0060f,.0080f,market):d.region==1?Mathf.Lerp(.0070f,.0100f,market):d.region==2?.012f:.013f;
  var targetColor=d.region==1?new Color(.045f,.064f,.085f):d.region>=2?new Color(.32f,.38f,.40f):Color.Lerp(new Color(.36f,.43f,.47f),new Color(.40f,.43f,.45f),market);
  float blend=1-Mathf.Exp(-Time.deltaTime*.8f);
  density=Mathf.Lerp(density,target,blend);color=Color.Lerp(color,targetColor,blend);
  RenderSettings.fog=!clearGeometryReview;
  RenderSettings.fogMode=FogMode.ExponentialSquared;
  RenderSettings.fogDensity=density;RenderSettings.fogColor=color;
 }
}
