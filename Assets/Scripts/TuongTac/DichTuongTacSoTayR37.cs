// Đích tương tác sổ, chuyển thao tác về activity đã gán.
// Component trong scene r92: Mat_ke_an_toan, So_cua_Minh.
// Reference public để đối chiếu Inspector (có nhánh tùy chọn): activity.
using UnityEngine;
[UnityEngine.Scripting.APIUpdating.MovedFrom(true, null, "Assembly-CSharp", "LakeNotebookTargetR37")]
public sealed class DichTuongTacSoTayR37:MonoBehaviour,IPlayerInteractable {
 public TuongTacSoTayHoR37 activity;
 public string Prompt=>activity.Prompt;
 public void Interact()=>activity.PickOrPlace();
}
