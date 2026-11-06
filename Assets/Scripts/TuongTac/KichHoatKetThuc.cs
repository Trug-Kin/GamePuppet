// Kích hoạt nhánh kết thúc khi đủ điều kiện trigger hiện có.
// Không thấy attachment trực tiếp trong YAML scene; xem consumer AddComponent và prefab trong bảng tra cứu.
using UnityEngine;

[UnityEngine.Scripting.APIUpdating.MovedFrom(true, null, "Assembly-CSharp", "EndingTrigger")]
public class KichHoatKetThuc : MonoBehaviour, IPlayerInteractable
{
    public int soKhuVucCanHoanTat = 4;

    public void Interact()
    {
        if (QuanLyTrangThaiGame.Instance == null) return;
        if (QuanLyTrangThaiGame.Instance.KhuVucHienTai < soKhuVucCanHoanTat) return;
        QuanLyTrangThaiGame.Instance.KichHoatEnding();
    }
}