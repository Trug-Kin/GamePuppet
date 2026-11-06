// Áp profile và cấu hình riêng cho scene R24.
// Không gắn trực tiếp trong r92; checkpoint JourneyRepair_v48_r24.unity: Lake_Market_Story_GDD21.
// Reference public để đối chiếu Inspector (có nhánh tùy chọn): story.
// Thứ tự chạy khai báo: -31900; giữ quan hệ khởi tạo/pose với các owner khác.
using System.IO;
using UnityEngine;
using UnityEngine.Rendering;

[DefaultExecutionOrder(-31900)]
[UnityEngine.Scripting.APIUpdating.MovedFrom(true, null, "Assembly-CSharp", "R24SceneProfile")]
public sealed class HoSoSceneR24:MonoBehaviour
{
 public DieuPhoiHanhTrinh story;public RenderPipelineAsset pipeline;
 public string SeededFrom{get;private set;}
 public static Vector3 MapSlope(Vector3 world){var p=world-new Vector3(-180,0,-60);p.z=p.z<=60?p.z*.5f:30+p.z-60;p.x=p.x*.45f+Mathf.Clamp01((p.z-18)/12)*2;return p+new Vector3(-180,0,-60);}
 void Awake(){
  story.SavePathOverride=Path.Combine(Application.persistentDataPath,"r24-playtest","journey-v48.json");Directory.CreateDirectory(Path.GetDirectoryName(story.SavePathOverride));
  // A new profile starts a new game; old geometry/save coordinates stay with r23.
  // Explicit import/migration is outside this pass, so no automatic pose remapping is hidden from players.
  if(pipeline){GraphicsSettings.defaultRenderPipeline=pipeline;QualitySettings.renderPipeline=pipeline;}
 }
}
