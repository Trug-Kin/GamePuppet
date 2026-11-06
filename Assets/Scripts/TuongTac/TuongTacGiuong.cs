// Tương tác giường để chuyển sang nhánh ngủ hiện có.
// Không thấy attachment trực tiếp trong YAML scene; xem consumer AddComponent và prefab trong bảng tra cứu.
using UnityEngine;

[UnityEngine.Scripting.APIUpdating.MovedFrom(true, null, "Assembly-CSharp", "BedInteractable")]
public class TuongTacGiuong : MonoBehaviour, IPlayerInteractable
{
    public void Interact()
    {
        if (QuanLyTrangThaiGame.Instance == null) return;
        if (QuanLyTrangThaiGame.Instance.DaKetThuc) return;
        QuanLyTrangThaiGame.Instance.Ngu();
    }
}