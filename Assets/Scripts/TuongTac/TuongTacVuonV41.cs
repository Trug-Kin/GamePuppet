// Tương tác đạo cụ vườn theo owner hành trình.
// Component trong scene r92: Window_Garden_Latch.
// Reference public để đối chiếu Inspector (có nhánh tùy chọn): chapter.
using UnityEngine;
[UnityEngine.Scripting.APIUpdating.MovedFrom(true, null, "Assembly-CSharp", "GardenInteractionV41")]
public sealed class TuongTacVuonV41 : MonoBehaviour,IPlayerInteractable
{
 public HoanTatHanhTrinhV41 chapter;
 public void Interact(){if(!chapter.story.Paused)chapter.VisitLan();}
}
