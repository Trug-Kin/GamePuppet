// Chỉ hiện mặt chữ đang hướng về camera vì vật liệu font legacy vẽ hai phía.
// Component trong scene r92: "V\xD2NG XOAY / CH\u1EE2 \u2192", "T\u1EC8NH D\u1EACY\nE \xB7 V\u1EC1 ph\xF2ng tr\u1ECD", "\u2190 C\u1EA6U \xD4NG \u0110\u1EA0O\nL\u1ED0I VEN H\u1ED2", "M\u1EB6T CH\u1EE2 \u2191\nC\xF3 th\u1EC3 \u0111i ti\u1EBFp r\u1ED3i quay l\u1EA1i", "CH\u1EE2 \u0110\xC0 L\u1EA0T \u2191\nTheo ph\u1ED1 t\u1EDBi m\u1EB7t ch\u1EE3", "C\u1EA6U \xD4NG \u0110\u1EA0O \u2192\n\xD4ng B\u1EA3o b\xEAn lan can".
// Reference public để đối chiếu Inspector (có nhánh tùy chọn): eye, front, back.
using UnityEngine;

// Legacy dynamic font material draws both sides. Show only the readable face.
[UnityEngine.Scripting.APIUpdating.MovedFrom(true, null, "Assembly-CSharp", "WorldSignFacesV48")]
public sealed class MatBienHieuV48:MonoBehaviour
{
 public Camera eye;
 public Renderer front,back;
 void LateUpdate()
 {
  if(!eye)return;
  bool ahead=Vector3.Dot(-transform.forward,eye.transform.position-transform.position)>=0;
  front.enabled=ahead;back.enabled=!ahead;
 }
}
