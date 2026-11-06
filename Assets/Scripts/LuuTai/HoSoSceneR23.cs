// Áp profile và cấu hình riêng cho scene R23.
// Không gắn trực tiếp trong r92; checkpoint JourneyRepair_v48_r23.unity: Lake_Market_Story_GDD21.
// Reference public để đối chiếu Inspector (có nhánh tùy chọn): story.
// Thứ tự chạy khai báo: -31900; giữ quan hệ khởi tạo/pose với các owner khác.
using System.IO;
using UnityEngine;
using UnityEngine.Rendering;
[DefaultExecutionOrder(-31900)]
[UnityEngine.Scripting.APIUpdating.MovedFrom(true, null, "Assembly-CSharp", "R23SceneProfile")]
public sealed class HoSoSceneR23:MonoBehaviour
{
 public DieuPhoiHanhTrinh story;public RenderPipelineAsset pipeline;
 public string SeededFrom{get;private set;}
 void Awake(){
  story.SavePathOverride=Path.Combine(Application.persistentDataPath,"r23-playtest","journey-v48.json");Directory.CreateDirectory(Path.GetDirectoryName(story.SavePathOverride));
  var source=Path.Combine(Application.persistentDataPath,"r22-playtest","journey-v48.json");
  if(!File.Exists(story.SavePathOverride)&&File.Exists(source)){
   try{File.Copy(source,story.SavePathOverride,false);SeededFrom=source;}
   catch(IOException ex){Debug.LogWarning("R23 save copy unavailable; New Game remains available: "+ex.Message);}
   catch(System.UnauthorizedAccessException ex){Debug.LogWarning("R23 save copy unavailable; New Game remains available: "+ex.Message);}
  }
  if(pipeline){GraphicsSettings.defaultRenderPipeline=pipeline;QualitySettings.renderPipeline=pipeline;}
 }
}
