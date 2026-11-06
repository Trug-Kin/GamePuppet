// Tương tác vùng dốc 3: cửa cuối hoặc ký ức theo cấu hình.
// Component trong scene r92: Last_Door, "\u0110\u1ECAA CH\u1EC8 VI\u1EBET TAY", Tung_Interaction, "V\xC9 XE C\u0168".
// Reference public để đối chiếu Inspector (có nhánh tùy chọn): chapter.
using UnityEngine;
[UnityEngine.Scripting.APIUpdating.MovedFrom(true, null, "Assembly-CSharp", "SlopeInteractionV39")]
public sealed class TuongTacDocV39 : MonoBehaviour,IPlayerInteractable
{
 public ChuongDocV39 chapter;
 public bool finalDoor;
 public int memoryIndex=-1;
 public string memory;
 public void Interact()
 {
  if(chapter.story.data.region!=3||chapter.story.Paused||chapter.story.PhoneOpen)return;
  if(finalDoor)chapter.BeginFinal();else if(memoryIndex>=0){chapter.story.data.slopeReadMask|=1<<memoryIndex;chapter.story.Say(memory);if(chapter.story.polish)chapter.story.polish.Read("GIẤY TỜ CỦA TÙNG",memory);}else chapter.Talk();
 }
}
