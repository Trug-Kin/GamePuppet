// Đếm dấu vết của khu mơ và phát sự kiện hoàn tất khu.
// Không thấy attachment trực tiếp trong YAML scene; xem consumer AddComponent và prefab trong bảng tra cứu.
using UnityEngine;

[UnityEngine.Scripting.APIUpdating.MovedFrom(true, null, "Assembly-CSharp", "DreamRegionController")]
public class DieuKhienKhuMo : MonoBehaviour
{
    public int soDauVetCanPhatHien = 1;
    public bool tuDongTinhDayKhiHoanTat = true;
    public UnityEngine.Events.UnityEvent OnRegionCompleted;

    private int soDauVetDaPhatHien;
    private bool daHoanTat;

    public void GhiNhanDauVet()
    {
        if (daHoanTat) return;
        soDauVetDaPhatHien++;
        if (soDauVetDaPhatHien < soDauVetCanPhatHien) return;

        daHoanTat = true;
        OnRegionCompleted?.Invoke();
        if (tuDongTinhDayKhiHoanTat) QuanLyTrangThaiGame.Instance?.TinhDay();
    }
}