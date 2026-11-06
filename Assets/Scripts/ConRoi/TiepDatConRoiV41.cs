// Căn tiếp đất rối theo cấu hình trình bày hiện có.
// Component trong scene r92: ConRoi_Minh_1_2, ConRoi_Minh_1_1, ConRoi_Minh_GD3, ConRoi_Minh_1_0, ConRoi_Minh_1_3, ConRoi_Minh_GD4_FinalOnly (các instance khác xem bảng tra cứu).
// Reference public để đối chiếu Inspector (có nhánh tùy chọn): supportR46.
// Code tự tìm/tạo: DieuKhienDiChuyen.
// Thứ tự chạy khai báo: 20550; giữ quan hệ khởi tạo/pose với các owner khác.
using System.Linq;
using UnityEngine;
// Presentation follows the existing route collision, without moving the player
// or changing GD1/GD2 timing. This removes hovering as the terrain grade changes.
[DefaultExecutionOrder(20550)]
[UnityEngine.Scripting.APIUpdating.MovedFrom(true, null, "Assembly-CSharp", "PuppetGroundingV41")]
public sealed class TiepDatConRoiV41:MonoBehaviour
{
 // R46 explicitly binds the walkable source surface. Decorative facade/counter
 // colliders must not become support; older scenes keep their existing policy.
 public Collider supportR46;
 float footOffset;Vector3 footLocal;bool ready,snap;
 void OnEnable(){snap=true;}
 void Start()
 {
  var legs=GetComponentsInChildren<Renderer>(true).Where(r=>r.name=="ConRoi_Leg_L"||r.name=="ConRoi_Leg_R").ToArray();
  if(legs.Length==0)return;footOffset=transform.position.y-legs.Min(r=>r.bounds.min.y);
  var center=(legs[0].bounds.center+legs[legs.Length-1].bounds.center)*.5f;center.y=legs.Min(r=>r.bounds.min.y);footLocal=transform.InverseTransformPoint(center);ready=true;
 }
 void LateUpdate()
 {
  if(!ready||Time.timeScale==0)return;
  Vector3 p=transform.position;float foot=p.y-footOffset;var support=transform.TransformPoint(footLocal);
  // Imported puppet roots sit roughly 3 m beside their visible feet. Sample
  // under the feet, rather than under the FBX origin on a different surface.
  var hits=Physics.RaycastAll(new Vector3(support.x,foot+1.2f,support.z),Vector3.down,4,~0,QueryTriggerInteraction.Ignore).OrderBy(h=>h.distance);
  if(supportR46){
   if(supportR46.Raycast(new Ray(new Vector3(support.x,foot+1.2f,support.z),Vector3.down),out var contact,4)){
    p.y=contact.point.y+footOffset+.012f;transform.position=p;snap=false;
   }
   return;
  }
  foreach(var h in hits)
  {
   if(h.normal.y<.55f||h.transform.IsChildOf(transform)||h.collider.GetComponentInParent<DieuKhienDiChuyen>())continue;
   float desired=h.point.y+footOffset+.012f;p.y=snap?desired:Mathf.Lerp(p.y,desired,1-Mathf.Exp(-Time.deltaTime*16));transform.position=p;snap=false;break;
  }
 }
}
