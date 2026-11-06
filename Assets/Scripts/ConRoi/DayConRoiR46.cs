// Dây rối theo anchor mesh chuyển động; giữ đăng ký dây đứt/sửa của phòng mơ.
// Component trong scene r92: Desk_Puppet_Broken_Strings, ConRoi_Minh_1_1, ConRoi_Minh_GD3, R35_Roi_tai_quay, ConRoi_Minh_1_2, ConRoi_Minh_1_0 (các instance khác xem bảng tra cứu).
// Reference public để đối chiếu Inspector (có nhánh tùy chọn): segments.
// Thứ tự chạy khai báo: 21110; giữ quan hệ khởi tạo/pose với các owner khác.
using System;
using UnityEngine;

// Only R46 opts in. Geometry and attachment/routing coordinates are authored in
// puppet-presentation-r46-source.blend; this synchronizes retained rigid pieces.
[DefaultExecutionOrder(21110)]
[UnityEngine.Scripting.APIUpdating.MovedFrom(true, null, "Assembly-CSharp", "PuppetThreadsR46")]
public sealed class DayConRoiR46 : MonoBehaviour
{
 [Serializable] public sealed class Binding
 {
  public Vector3[] attachmentLocal, bendLocal, routeLocal;
 }
 public TextAsset sourceBindings;
 public TextAsset fittedRoutingR91;
 [Serializable] public sealed class FittedRouting {public Vector3[] anchors,clearance;public float topClearance;}
 FittedRouting fitted;
 public MeshFilter[] attachments;
 public Transform[] segments; // two finite segments for each of five strings
 Binding binding;
 void Awake(){binding=JsonUtility.FromJson<Binding>(sourceBindings.text);if(fittedRoutingR91)fitted=JsonUtility.FromJson<FittedRouting>(fittedRoutingR91.text);}
 void LateUpdate()
 {
  if(binding==null)return;
  if(fitted!=null){
   var hb=attachments[0].sharedMesh.bounds;var fittedCrown=transform.InverseTransformPoint(attachments[0].transform.TransformPoint(hb.center+Vector3.forward*hb.extents.z));
   for(int i=0;i<5;i++){
    var bounds=attachments[i].sharedMesh.bounds;var a=attachments[i].transform.TransformPoint(bounds.min+Vector3.Scale(bounds.size,fitted.anchors[i]));
    var fittedBend=transform.InverseTransformPoint(a)+fitted.clearance[i];var fittedEnd=new Vector3(fittedBend.x,fittedBend.y,Mathf.Max(fittedBend.z+.03f,fittedCrown.z+fitted.topClearance));
    Place(segments[i*2],a,transform.TransformPoint(fittedBend));Place(segments[i*2+1],transform.TransformPoint(fittedBend),transform.TransformPoint(fittedEnd));
   }
   return;
  }
  var crown=attachments[0].transform.TransformPoint(binding.attachmentLocal[0]);
  var offset=transform.InverseTransformPoint(crown)-new Vector3(-2.987f,0,1.7256f);
  for(int i=0;i<5;i++){
   var a=attachments[i].transform.TransformPoint(binding.attachmentLocal[i]);
   var bend=transform.TransformPoint(binding.bendLocal[i]+offset);
   var end=transform.TransformPoint(binding.routeLocal[i]+offset);
   Place(segments[i*2],a,bend);Place(segments[i*2+1],bend,end);
  }
 }
 static void Place(Transform t,Vector3 a,Vector3 b)
 {
  var d=b-a;t.SetPositionAndRotation((a+b)*.5f,Quaternion.FromToRotation(Vector3.forward,d.normalized));
  var s=t.parent.lossyScale;t.localScale=new Vector3(1/Mathf.Abs(s.x),1/Mathf.Abs(s.y),d.magnitude/Mathf.Abs(s.z));
 }
}
