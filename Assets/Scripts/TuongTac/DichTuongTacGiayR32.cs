// Chuyển thao tác trên giấy/khay cho activity theo ID và cờ inTray.
// Component trong scene r92: Player_Paper, Desk_Top, Right_PaperTray_Base, Model_Answer_Paper_R32.
// Reference public để đối chiếu Inspector (có nhánh tùy chọn): activity.
using UnityEngine;
[UnityEngine.Scripting.APIUpdating.MovedFrom(true, null, "Assembly-CSharp", "Class203PaperTargetR32")]
public sealed class DichTuongTacGiayR32:MonoBehaviour,IPlayerInteractable {
 public TuongTacGiayLop203R32 activity;
 public int paper=-1;
 public bool inTray;
 public string Prompt=>paper>=0?activity.ItemPrompt(paper):activity.PlacePrompt(inTray);
 public void Interact(){if(paper>=0)activity.Inspect(paper);else activity.Place(inTray);}
}
