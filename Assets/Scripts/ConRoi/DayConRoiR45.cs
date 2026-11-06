// Lớp dây rối R45 giữ cho checkpoint cũ.
// Component trong scene r92: ConRoi_Minh_0_0, ConRoi_Minh_1_0, ConRoi_Minh_1_1, ConRoi_Minh_1_2, ConRoi_Minh_1_3, R35_Roi_tai_quay.
// Reference public để đối chiếu Inspector (có nhánh tùy chọn): threads, head.
// Thứ tự chạy khai báo: 21100; giữ quan hệ khởi tạo/pose với các owner khác.
using UnityEngine;

// Five finite strings, authored only on the r45 lake and market puppets.
// Unit thread geometry is exported from the Blender identity source.
[DefaultExecutionOrder(21100)]
[UnityEngine.Scripting.APIUpdating.MovedFrom(true, null, "Assembly-CSharp", "PuppetThreadsR45")]
public sealed class DayConRoiR45 : MonoBehaviour
{
 public MeshFilter[] attachments;
 public Transform[] threads;
 public Renderer head;
 readonly float[] lengths={1.8f,2.65f,2.65f,3.45f,3.45f};
 Vector3[] upper;
 Vector3 Bottom(int i)
 {
  if(i==0)return new Vector3(head.bounds.center.x,head.bounds.max.y,head.bounds.center.z);
  // The source's five lower ends are crown, two wrists and two ankles.
  // Legacy body meshes use local Z as their long axis, including after gait.
  var mesh=attachments[i];var bounds=mesh.sharedMesh.bounds;
  var local=bounds.center;local.z=bounds.min.z;
  var p=mesh.transform.TransformPoint(local);
  if(i>=3)p+=transform.right*(i==3?-.035f:.035f);
  return p;
 }
 void Start()
 {
  upper=new Vector3[5];
  for(int i=0;i<5;i++)
  {
   var spread=i>=3?transform.right*(i==3?-.45f:.45f):Vector3.zero;
   upper[i]=transform.InverseTransformPoint(Bottom(i)+(Vector3.up+spread).normalized*lengths[i]);
  }
 }
 void LateUpdate()
 {
  if(upper==null)return;
  for(int i=0;i<5;i++)
  {
   var a=Bottom(i);var b=transform.TransformPoint(upper[i]);var delta=b-a;
   var t=threads[i];t.SetPositionAndRotation((a+b)*.5f,Quaternion.FromToRotation(Vector3.forward,delta.normalized));
   var s=t.parent.lossyScale;t.localScale=new Vector3(1/Mathf.Abs(s.x),1/Mathf.Abs(s.y),delta.magnitude/Mathf.Abs(s.z));
  }
 }
}
