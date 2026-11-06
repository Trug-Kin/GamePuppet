// Trigger cho Player tỉnh dậy và chuyển khu theo các cờ Inspector.
// Không thấy attachment trực tiếp trong YAML scene; xem consumer AddComponent và prefab trong bảng tra cứu.
// Code tự tìm/tạo: Collider.
using UnityEngine;

[RequireComponent(typeof(Collider))]
[UnityEngine.Scripting.APIUpdating.MovedFrom(true, null, "Assembly-CSharp", "DreamWakeTrigger")]
public class VungKichHoatTinhDay : MonoBehaviour
{
    public bool chiKhiDangMo = true;
    public bool chuyenKhuVucSauKhiTinh = true;

    private void Reset()
    {
        GetComponent<Collider>().isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (chiKhiDangMo && (QuanLyTrangThaiGame.Instance == null || QuanLyTrangThaiGame.Instance.TrangThaiHienTai != QuanLyTrangThaiGame.TrangThai.Mo)) return;

        QuanLyTrangThaiGame.Instance?.TinhDay();
        if (chuyenKhuVucSauKhiTinh) QuanLyTrangThaiGame.Instance?.ChuyenKhuVucTiepTheo();
        gameObject.SetActive(false);
    }
}