using UnityEngine;
using UnityEditor;
namespace ConRoi.Gameplay.EditorChecks {
public sealed class FullCaseSandboxWindow : EditorWindow {
    FullCaseSandboxPanel panel;
    [SerializeField] bool sessionWasOpened;
    internal FullCaseSandboxController ControllerForChecks=>panel?.ControllerForChecks;
    internal string GeometryForChecks(string label)=>panel?.GeometryForChecks(label)??"Session interrupted";
    internal bool RevealControl(string label,out Rect rect){rect=default;return panel!=null&&panel.RevealControl(label,out rect);}
    [MenuItem("Con Roi/Gameplay/Toan tuyen TEST ONLY")]
    public static void Open()=>GetWindow<FullCaseSandboxWindow>("Con Rối • TEST ONLY");
    void OnEnable(){if(!sessionWasOpened)StartSession();}
    void StartSession(){panel?.Dispose();panel=new FullCaseSandboxPanel(this);sessionWasOpened=true;}
    void OnDisable(){panel?.Dispose();panel=null;}
    void OnGUI(){if(panel!=null){panel.DrawEditorUI();return;}
        EditorGUILayout.HelpBox("TEST ONLY: phiên công cụ đã ngắt do compile/domain reload. Không tự tạo lại tiến trình hoặc ghi đè save. Mở phiên mới rồi tải đúng profile thử đã lưu nếu cần.",MessageType.Warning);
        if(GUILayout.Button("Bắt đầu phiên thử mới"))StartSession();
    }
}
}
