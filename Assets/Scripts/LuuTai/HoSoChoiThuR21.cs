// Chọn cấu hình scene/profile cho lượt chơi thử R21.
// Không gắn trực tiếp trong r92; checkpoint JourneyRepair_v48_r21_playtest.unity: Lake_Market_Story_GDD21.
// Reference public để đối chiếu Inspector (có nhánh tùy chọn): story.
// Thứ tự chạy khai báo: -32000; giữ quan hệ khởi tạo/pose với các owner khác.
using System.IO;
using UnityEngine;

// Attached only to the product-review derivative. Never imports or replaces a live save.
[DefaultExecutionOrder(-32000)]
[UnityEngine.Scripting.APIUpdating.MovedFrom(true, null, "Assembly-CSharp", "R21PlaytestProfile")]
public sealed class HoSoChoiThuR21 : MonoBehaviour
{
    public DieuPhoiHanhTrinh story;
    public string SavePath { get; private set; }
    void Awake()
    {
        SavePath = Path.Combine(Application.persistentDataPath, "r21-playtest", "journey-v48.json");
        Directory.CreateDirectory(Path.GetDirectoryName(SavePath));
        story.SavePathOverride = SavePath;
    }
}
