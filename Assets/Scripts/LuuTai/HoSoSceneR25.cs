// Áp profile và cấu hình riêng cho scene R25.
// Không gắn trực tiếp trong r92; checkpoint JourneyRepair_v48_r25.unity: Lake_Market_Story_GDD21.
// Reference public để đối chiếu Inspector (có nhánh tùy chọn): story.
// Thứ tự chạy khai báo: -31900; giữ quan hệ khởi tạo/pose với các owner khác.
using System.IO;
using UnityEngine;
using UnityEngine.Rendering;
[DefaultExecutionOrder(-31900)]
[UnityEngine.Scripting.APIUpdating.MovedFrom(true, null, "Assembly-CSharp", "R25SceneProfile")]
public sealed class HoSoSceneR25:MonoBehaviour
{
 public DieuPhoiHanhTrinh story;public RenderPipelineAsset pipeline;
 void Awake(){story.SavePathOverride=Path.Combine(Application.persistentDataPath,"r25-playtest","journey-v48.json");Directory.CreateDirectory(Path.GetDirectoryName(story.SavePathOverride));if(pipeline){GraphicsSettings.defaultRenderPipeline=pipeline;QualitySettings.renderPipeline=pipeline;}}
}
