// Kích hoạt dấu vết một lần, phát UnityEvent và đổi giai đoạn rối nếu được cấu hình.
// Không gắn trực tiếp trong r92; checkpoint SampleScene.unity: DauVet_Test.
// Reference public để đối chiếu Inspector (có nhánh tùy chọn): conRoi.
// Code tự tìm/tạo: SphereCollider.
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(SphereCollider))]
[UnityEngine.Scripting.APIUpdating.MovedFrom(true, null, "Assembly-CSharp", "TriggerVungDauVet")]
public class VungKichHoatDauVet : MonoBehaviour
{
    public enum PhaseSauKhiPhatHien { KhongDoi, GD1, GD2, GD3 }

    public UnityEvent OnActivated = new UnityEvent();
    public bool chiKhiDangMo = true;
    public PhaseSauKhiPhatHien phaseSauKhiPhatHien = PhaseSauKhiPhatHien.KhongDoi;
    public DieuKhienConRoi conRoi;
    public bool DaKichHoat { get; private set; } = false;

    private void Reset()
    {
        GetComponent<SphereCollider>().isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (DaKichHoat) return;
        if (!other.CompareTag("Player")) return;
        if (chiKhiDangMo && (QuanLyTrangThaiGame.Instance == null || QuanLyTrangThaiGame.Instance.TrangThaiHienTai != QuanLyTrangThaiGame.TrangThai.Mo)) return;

        DaKichHoat = true;
        QuanLyTrangThaiGame.Instance?.GhiNhanDauVet();
        if (conRoi != null)
        {
            if (phaseSauKhiPhatHien == PhaseSauKhiPhatHien.GD1) conRoi.ChuyenSangGD1();
            if (phaseSauKhiPhatHien == PhaseSauKhiPhatHien.GD2) conRoi.ChuyenSangGD2();
            if (phaseSauKhiPhatHien == PhaseSauKhiPhatHien.GD3) conRoi.ChuyenSangGD3();
        }
        Debug.Log("Da kich hoat dau vet!");
        OnActivated?.Invoke();
    }
}