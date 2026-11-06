// Lưới dự phòng thời gian ngủ/mơ của graybox; giữ khoảng chờ để không cướp lượt tỉnh chủ động.
// Không thấy attachment trực tiếp trong YAML scene; xem consumer AddComponent và prefab trong bảng tra cứu.
using UnityEngine;

[UnityEngine.Scripting.APIUpdating.MovedFrom(true, null, "Assembly-CSharp", "SleepFallback")]
public class DuPhongNguTinh : MonoBehaviour
{
    public float thoiGianFallback = 90f;

    // Luoi an toan cho graybox: trong Mo hien chua co cach thoat nao khac neu nguoi choi
    // bo lo het dau vet lan Wake_Trigger. De rong rai de khong "cuop" vong lap chinh
    // (tinh day chu dong = hoan tat khu vuc / Wake_Trigger).
    public float thoiGianToiDaTrongMo = 120f;

    private float thoiGianCho;
    private float thoiGianTrongMo;

    private void Update()
    {
        if (QuanLyTrangThaiGame.Instance == null) return;
        if (QuanLyTrangThaiGame.Instance.DaKetThuc) return;

        if (QuanLyTrangThaiGame.Instance.TrangThaiHienTai == QuanLyTrangThaiGame.TrangThai.Thuc)
        {
            thoiGianTrongMo = 0f;
            thoiGianCho += Time.deltaTime;
            if (thoiGianCho >= thoiGianFallback)
            {
                thoiGianCho = 0f;
                QuanLyTrangThaiGame.Instance.Ngu();
            }
            return;
        }

        thoiGianCho = 0f;
        thoiGianTrongMo += Time.deltaTime;
        if (thoiGianTrongMo >= thoiGianToiDaTrongMo)
        {
            thoiGianTrongMo = 0f;
            QuanLyTrangThaiGame.Instance.TinhDay();
        }
    }
}