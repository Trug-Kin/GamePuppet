// Đích thao tác chợ theo cấu hình của activity.
// Component trong scene r92: Phieu_ghi_hang, O_giao_hang.
// Reference public để đối chiếu Inspector (có nhánh tùy chọn): activity.
using UnityEngine;
[UnityEngine.Scripting.APIUpdating.MovedFrom(true, null, "Assembly-CSharp", "MarketTargetR35")]
public sealed class DichTuongTacChoR35:MonoBehaviour,IPlayerInteractable {
 public QuanLyGoiHangChoR35 activity;
 public bool slip;
 public string Prompt=>slip?"E · Xem phiếu ghi hàng":activity.Held>=0?"E · Đặt thử lên ô giao hàng":activity.Candidate>=0?"Gói đang ở ô giao · E trên gói để lấy lại":"Ô giao hàng · Có thể đặt một gói ở đây";
 public void Interact(){if(slip)activity.ReadSlip();else activity.Stage();}
}
