// Đạo cụ kể chuyện của lớp trải nghiệm V40.
// Component trong scene r92: "Document_BI\xCAN B\u1EA2N H\u1ECCP KHOA", "Document_T\u1EDC GI\u1EA4Y C\u1EE6A M\u1EB8", "Document_TH\u01AF CH\u01AFA G\u1EECI(Clone)", "Document_TH\u01AF CH\u01AFA G\u1EECI", "Document_NH\u1EACT K\xDD TH\u1EF0C T\u1EACP(Clone)", "Document_BI\xCAN B\u1EA2N H\u1ECCP KHOA(Clone)" (các instance khác xem bảng tra cứu).
// Reference public để đối chiếu Inspector (có nhánh tùy chọn): story.
using UnityEngine;

[UnityEngine.Scripting.APIUpdating.MovedFrom(true, null, "Assembly-CSharp", "NarrativePropV40")]
public sealed class DaoCuKeChuyenV40 : MonoBehaviour,IPlayerInteractable
{
 public DieuPhoiHanhTrinh story;
 public string heading;
 [TextArea(5,20)]public string text;
 public void Interact(){if(story.polish&&!story.Paused)story.polish.Read(heading,text);}
}
