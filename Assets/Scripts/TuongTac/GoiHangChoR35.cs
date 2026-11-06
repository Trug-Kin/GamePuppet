// Component gói hàng: ID, nắp, khăn, nhãn và thao tác nhặt qua activity.
// Component trong scene r92: Goi_khoang_ben, Goi_canh_phieu.
// Reference public để đối chiếu Inspector (có nhánh tùy chọn): activity, lid, scarf, label, inside.
using UnityEngine;
[UnityEngine.Scripting.APIUpdating.MovedFrom(true, null, "Assembly-CSharp", "MarketParcelR35")]
public sealed class GoiHangChoR35:MonoBehaviour,IPlayerInteractable {
 public QuanLyGoiHangChoR35 activity;
 public int id;
 public Transform lid;
 public KhanTrongGoiHangR35 scarf;
 public TextMesh label,inside;
 public string Prompt=>activity.Held>=0?(activity.Candidate==id?"E · Thay gói trên ô bằng gói đang cầm":"E · Đổi gói đang cầm (gói cũ về chỗ)"):"E · Cầm gói để kiểm bên trong";
 public void Interact()=>activity.Pick(id);
}
