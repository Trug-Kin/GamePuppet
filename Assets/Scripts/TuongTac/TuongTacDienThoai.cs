// Tương tác điện thoại theo tiến trình hiện có.
// Không thấy attachment trực tiếp trong YAML scene; xem consumer AddComponent và prefab trong bảng tra cứu.
using UnityEngine;

[UnityEngine.Scripting.APIUpdating.MovedFrom(true, null, "Assembly-CSharp", "PhoneInteractable")]
public class TuongTacDienThoai : MonoBehaviour, IPlayerInteractable
{
    public enum HanhDongDienThoai { DangBai, NgheMay, BoQua }
    public HanhDongDienThoai hanhDong = HanhDongDienThoai.DangBai;

    private bool daTuongTac;

    public void Interact()
    {
        if (TienTrinhTroChoi.Instance == null || (QuanLyTrangThaiGame.Instance != null && QuanLyTrangThaiGame.Instance.TrangThaiHienTai != QuanLyTrangThaiGame.TrangThai.Thuc)) return;

        // Moi doi tuong chi tinh diem Hanh dong MOT lan (truoc day bam E lien tuc
        // van cong diem, khien ending C chi can spam mot dien thoai).
        if (daTuongTac) return;
        daTuongTac = true;

        switch (hanhDong)
        {
            case HanhDongDienThoai.DangBai:
                TienTrinhTroChoi.Instance.GhiNhanTuongTacDoiThuc();
                TienTrinhTroChoi.Instance.DangBaiTrend();
                break;
            case HanhDongDienThoai.NgheMay:
                TienTrinhTroChoi.Instance.GhiNhanTuongTacDoiThuc();
                TienTrinhTroChoi.Instance.NgheDienThoai();
                break;
            case HanhDongDienThoai.BoQua:
                TienTrinhTroChoi.Instance.BoQuaTinNhan();
                break;
        }
    }
}
