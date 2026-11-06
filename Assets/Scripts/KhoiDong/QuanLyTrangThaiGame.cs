// Giữ singleton trạng thái thức/mơ/kết thúc và phát sự kiện đổi khu; tồn tại qua chuyển scene.
// Không thấy attachment trực tiếp trong YAML scene; xem consumer AddComponent và prefab trong bảng tra cứu.
using System;
using UnityEngine;

[UnityEngine.Scripting.APIUpdating.MovedFrom(true, null, "Assembly-CSharp", "ConRoiGameManager")]
public class QuanLyTrangThaiGame : MonoBehaviour
{
    public void RestoreState(int region, TrangThai state)
    {
        KhuVucHienTai=Mathf.Clamp(region,0,4); TrangThaiHienTai=state; TongDauVet=4;
        KhuVucChanged?.Invoke(KhuVucHienTai); TrangThaiChanged?.Invoke(state);
    }
    void OnDestroy() { if(Instance==this)Instance=null; }
    public enum TrangThai { Thuc, Mo, KetThuc }

    public static QuanLyTrangThaiGame Instance { get; private set; }
    public TrangThai TrangThaiHienTai { get; private set; } = TrangThai.Thuc;
    public int KhuVucHienTai { get; private set; }
    public int TongDauVet { get; private set; }
    public event Action<TrangThai> TrangThaiChanged;
    public event Action<int> KhuVucChanged;
    public event Action<string> EndingTriggered;

    [SerializeField] private int tongDauVetMacDinh = 4;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        TongDauVet = tongDauVetMacDinh;
        DontDestroyOnLoad(gameObject);
    }

    public void Ngu()
    {
        if (TrangThaiHienTai == TrangThai.KetThuc) return;
        TrangThaiHienTai = TrangThai.Mo;
        TrangThaiChanged?.Invoke(TrangThaiHienTai);
    }

    public void TinhDay()
    {
        if (TrangThaiHienTai != TrangThai.Mo) return;
        TrangThaiHienTai = TrangThai.Thuc;
        TrangThaiChanged?.Invoke(TrangThaiHienTai);
    }

    public void ChuyenKhuVucTiepTheo()
    {
        KhuVucHienTai++;
        KhuVucChanged?.Invoke(KhuVucHienTai);
    }

    public bool DaKetThuc => TrangThaiHienTai == TrangThai.KetThuc;

    public void GhiNhanDauVet()
    {
        TienTrinhTroChoi.Instance?.PhatHienDauVet();
    }

    /// Do so dau vet thuc te co trong scene thay vi dung gia tri mac dinh trong Inspector.
    public void DatTongDauVet(int soLuong)
    {
        if (DaKetThuc) return;
        TongDauVet = Mathf.Max(1, soLuong);
    }

    public void KichHoatEnding()
    {
        if (TrangThaiHienTai == TrangThai.KetThuc) return;
        TrangThaiHienTai = TrangThai.KetThuc;
        string ending = TienTrinhTroChoi.Instance == null ? "A" : TienTrinhTroChoi.Instance.LayEnding(TongDauVet);
        EndingTriggered?.Invoke(ending);
        TrangThaiChanged?.Invoke(TrangThaiHienTai);
        Debug.Log("CON ROI ending: " + ending);
    }
}
