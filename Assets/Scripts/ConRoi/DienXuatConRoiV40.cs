// Diễn xuất rối trên owner hiện hữu.
// Component trong scene r92: ConRoi_Minh_1_2, ConRoi_Minh_1_1, ConRoi_Minh_1_0, ConRoi_Minh_1_3, ConRoi_Minh_0_0.
// Reference public để đối chiếu Inspector (có nhánh tùy chọn): puppet, articulation.
// Thứ tự chạy khai báo: 20600; giữ quan hệ khởi tạo/pose với các owner khác.
using UnityEngine;

// Authored transform clips articulate the retained wooden segments. GD3's lead
// and GD4's stillness keep exclusive control of their own pose channels.
[DefaultExecutionOrder(20600)]
[UnityEngine.Scripting.APIUpdating.MovedFrom(true, null, "Assembly-CSharp", "PuppetPerformanceV40")]
public sealed class DienXuatConRoiV40 : MonoBehaviour
{
 public DieuKhienConRoi puppet;
 public Animation articulation;
 Vector3 previous;
 float stride;
 void OnEnable(){previous=transform.position;stride=0;}
 void LateUpdate()
 {
  var delta=transform.position-previous;previous=transform.position;
  if(!puppet.enabled||puppet.giaiDoanHienTai==DieuKhienConRoi.GiaiDoan.GD3||puppet.giaiDoanHienTai==DieuKhienConRoi.GiaiDoan.GD4)return;
  if(delta.magnitude>2)return;
  if(delta.sqrMagnitude>.000001f)stride+=delta.magnitude/.9f;
  else stride=Mathf.MoveTowards(stride,Mathf.Round(stride*2)/2,Time.deltaTime*2);
  var state=articulation["walk"];if(state==null)return;
  state.enabled=true;state.weight=1;state.time=stride;articulation.Sample();state.enabled=false;
 }
}
