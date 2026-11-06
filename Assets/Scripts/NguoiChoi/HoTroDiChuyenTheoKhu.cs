// Hiện hướng dẫn di chuyển cho scene môi trường khi owner hành trình chưa chạy.
// Component trong scene r92: Minh_RegionalWalk_180cm.
// Code tự tìm/tạo: DieuPhoiHanhTrinh.
using UnityEngine;

// Navigation help for the integrated environment scene; no recorder or test driver.
[UnityEngine.Scripting.APIUpdating.MovedFrom(true, null, "Assembly-CSharp", "RegionalWalkHelp")]
public sealed class HoTroDiChuyenTheoKhu : MonoBehaviour
{
    DieuPhoiHanhTrinh story;
    void Awake(){story=FindAnyObjectByType<DieuPhoiHanhTrinh>();}
    void OnGUI()
    {
        if(story&&story.enabled)return;
        var style=new GUIStyle(GUI.skin.label){fontSize=15,wordWrap=true};
        GUI.Box(new Rect(12,12,440,52),GUIContent.none);
        GUI.Label(new Rect(24,18,416,44),"WASD / phím mũi tên: di chuyển • Chuột: nhìn\nBấm chuột: giữ con trỏ • Esc: thả con trỏ",style);
    }
}
