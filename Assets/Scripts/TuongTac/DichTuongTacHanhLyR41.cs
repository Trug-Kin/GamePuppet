// Chuyển thao tác hành lý về activity theo item/arrange.
// Component trong scene r92: To_chuan_bi_gap, Dat_the_canh_quai, The_hanh_ly_cu.
// Reference public để đối chiếu Inspector (có nhánh tùy chọn): activity.
using UnityEngine;
[UnityEngine.Scripting.APIUpdating.MovedFrom(true, null, "Assembly-CSharp", "SlopePreparationTargetR41")]
public sealed class DichTuongTacHanhLyR41:MonoBehaviour,IPlayerInteractable {
 public ChuanBiHanhLyR41 activity;
 public int item;
 public bool arrange;
 public string Prompt=>activity.Prompt(item,arrange);
 public void Interact()=>activity.Interact(item,arrange);
}
