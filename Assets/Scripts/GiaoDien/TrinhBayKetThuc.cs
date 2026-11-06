// Trình bày kết thúc theo sự kiện từ game manager.
// Không thấy attachment trực tiếp trong YAML scene; xem consumer AddComponent và prefab trong bảng tra cứu.
using UnityEngine;

/// Trinh bay ending. Truoc day EndingTriggered khong co nguoi nghe nen ket qua
/// chi di ra Console (ConRoiGameManager.cs) va game van chay tiep sau ending.
/// Graybox nay dung OnGUI de khong phu thuoc package UI / TextMeshPro.
[UnityEngine.Scripting.APIUpdating.MovedFrom(true, null, "Assembly-CSharp", "EndingPresenter")]
public class TrinhBayKetThuc : MonoBehaviour
{
    [TextArea] public string[] noiDungEnding =
    {
        "A - Tro thanh roi: Minh thuc day va vo thuc dang nhat ky giac mo.",
        "A2 - Hanh dong rong: Minh goi ve nha nhung chi lap lai nhung cau tra loi an toan.",
        "B - Nhan ra, chua hanh dong: Minh nhan ra van de nhung dien thoai nam im.",
        "C - Chap nhan trach nhiem: Minh goi cho bo me va noi ra mot dieu chua tung dam noi."
    };

    private string endingHienTai;
    private bool daHien;

    private void Start()
    {
        if (QuanLyTrangThaiGame.Instance == null)
        {
            Debug.LogWarning("EndingPresenter: ConRoiGameManager.Instance == null, khong dang ky duoc ending.");
            return;
        }

        QuanLyTrangThaiGame.Instance.EndingTriggered += HienEnding;
    }

    private void OnDestroy()
    {
        if (QuanLyTrangThaiGame.Instance != null) QuanLyTrangThaiGame.Instance.EndingTriggered -= HienEnding;
    }

    private void HienEnding(string ending)
    {
        endingHienTai = ending;
        daHien = true;

        // Tra tu do chuot de nguoi choi doc duoc ending.
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Debug.Log("Ending hien thi: " + LayNoiDung(ending));
    }

    private string LayNoiDung(string ending)
    {
        int index = -1;
        if (ending == "A") index = 0;
        else if (ending == "A2") index = 1;
        else if (ending == "B") index = 2;
        else if (ending == "C") index = 3;

        if (index >= 0 && noiDungEnding != null && index < noiDungEnding.Length) return noiDungEnding[index];
        return "Ending: " + ending;
    }

    private void OnGUI()
    {
        if (!daHien) return;

        GUIStyle style = new GUIStyle(GUI.skin.label);
        style.fontSize = 22;
        style.alignment = TextAnchor.MiddleCenter;
        style.wordWrap = true;

        Rect panel = new Rect(Screen.width * 0.15f, Screen.height * 0.35f, Screen.width * 0.7f, Screen.height * 0.3f);
        GUI.Box(panel, GUIContent.none);
        GUI.Label(new Rect(panel.x + 20f, panel.y + 20f, panel.width - 40f, panel.height - 40f),
            "CON ROI - KET THUC (" + endingHienTai + ")\n\n" + LayNoiDung(endingHienTai), style);
    }
}