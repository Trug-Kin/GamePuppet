// Tương tác trường theo vùng 2; chặn khi pause hoặc mở điện thoại.
// Component trong scene r92: "S\u1ED4 NH\u1EACN X\xC9T", "TRANG VI\u1EBET D\u1EDE", 203_Return, "TH\xD4NG B\xC1O", Gate_Return, "L\u1ECACH H\u1ECCC" (các instance khác xem bảng tra cứu).
// Reference public để đối chiếu Inspector (có nhánh tùy chọn): chapter.
// Code tự tìm/tạo: TuongTacGiayLop203R32.
using UnityEngine;

[UnityEngine.Scripting.APIUpdating.MovedFrom(true, null, "Assembly-CSharp", "SchoolInteractionV37")]
public sealed class TuongTacTruongV37 : MonoBehaviour,IPlayerInteractable
{
 public ChuongTruongHocV37 chapter;
 public bool exit;
 public string memory;
 public int memoryIndex=-1;
 public void Interact()
 {
  if(chapter.story.data.region!=2||chapter.story.Paused||chapter.story.PhoneOpen)return;
  if(exit){var papers=chapter.GetComponent<TuongTacGiayLop203R32>();if(papers&&papers.completeR34){papers.LeaveR34();chapter.story.Wake();chapter.story.Say(papers.MaiResponse);}else chapter.story.Wake();}else if(memoryIndex>=0)chapter.Read(memoryIndex,memory);else chapter.Talk();
 }
}
