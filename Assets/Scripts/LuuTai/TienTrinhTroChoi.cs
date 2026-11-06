// Giữ hai trục tiến trình và snapshot; ngưỡng ending đọc số tương tác đời thực, không thay bằng tổng điểm có trọng số.
// Không thấy attachment trực tiếp trong YAML scene; xem consumer AddComponent và prefab trong bảng tra cứu.
using UnityEngine;

[UnityEngine.Scripting.APIUpdating.MovedFrom(true, null, "Assembly-CSharp", "GameProgress")]
public class TienTrinhTroChoi : MonoBehaviour
{
    [System.Serializable] public class Snapshot
    { public int awareness, actionPoints, evidence, interactions, ignoredMessages; }
    public Snapshot Capture() => new Snapshot { awareness=NhanThuc, actionPoints=HanhDong, evidence=SoDauVet, interactions=SoLanTuongTacDoiThuc, ignoredMessages=SoTinNhanBoQua };
    public void Restore(Snapshot data)
    {
        NhanThuc=Mathf.Clamp(data.awareness,0,4); HanhDong=data.actionPoints;
        SoDauVet=Mathf.Clamp(data.evidence,0,4); SoLanTuongTacDoiThuc=Mathf.Max(0,data.interactions);
        SoTinNhanBoQua=Mathf.Max(0,data.ignoredMessages);
    }
    public void ResetProgress() { Restore(new Snapshot()); }
    void OnDestroy() { if(Instance==this)Instance=null; }
    public static TienTrinhTroChoi Instance { get; private set; }

    public int NhanThuc { get; private set; }
    public int HanhDong { get; private set; }
    public int SoDauVet { get; private set; }

    // GDD 5.3: ending uses number of positive real-world interactions, not weighted points.
    public int SoLanTuongTacDoiThuc { get; private set; }
    public int SoTinNhanBoQua { get; private set; }

    [Header("Nguong ending (conroi.txt 5.3 / 5.4)")]
    [Tooltip("Diem Hanh dong duoc coi la CAO khi >= nguong nay (+1 dang bai, +3 nghe may, -1 bo qua).")]
    [SerializeField] private int nguongHanhDong = 2;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void PhatHienDauVet()
    {
        SoDauVet++;
        NhanThuc++;
    }

    public void DangBaiTrend()
    {
        HanhDong++;
    }

    public void NgheDienThoai()
    {
        HanhDong += 3;
    }

    public void BoQuaTinNhan()
    {
        HanhDong--;
        SoTinNhanBoQua++;
    }

    /// Tang bo dem tuong tac doi thuc. Moi doi tuong tu bao ve bang co daTuongTac
    /// cua chinh no, nen khong the spam mot doi tuong de cong diem.
    public void GhiNhanTuongTacDoiThuc()
    {
        SoLanTuongTacDoiThuc++;
    }

    public string LayEnding(int tongDauVet)
    {
        bool nhanThucCao = NhanThuc >= Mathf.Max(1, tongDauVet * 3 / 4);
        bool hanhDongCao = SoLanTuongTacDoiThuc >= nguongHanhDong;

        if (nhanThucCao && hanhDongCao) return "C";
        if (nhanThucCao) return "B";
        if (hanhDongCao) return "A2";
        return "A";
    }
}
