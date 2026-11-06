// Áp profile/save riêng cho scene R30.
// Không gắn trực tiếp trong r92; checkpoint JourneyRepair_v48_r30.unity: Lake_Market_Story_GDD21.
// Reference public để đối chiếu Inspector (có nhánh tùy chọn): story.
// Thứ tự chạy khai báo: -31800; giữ quan hệ khởi tạo/pose với các owner khác.
using System;
using System.IO;
using UnityEngine;

// Separate review profile. Historical r26/user profiles are never opened for writing.
[DefaultExecutionOrder(-31800)]
[UnityEngine.Scripting.APIUpdating.MovedFrom(true, null, "Assembly-CSharp", "R30SceneProfile")]
public sealed class HoSoSceneR30:MonoBehaviour
{
 public DieuPhoiHanhTrinh story;
 void Awake(){
  var args=Environment.GetCommandLineArgs();int i=Array.IndexOf(args,"--r30-review-output");
  string dir=i>=0&&i+1<args.Length?Path.GetFullPath(args[i+1]):Path.Combine(Application.persistentDataPath,"r30-playtest");
  Directory.CreateDirectory(dir);story.SavePathOverride=Path.Combine(dir,"journey-v48.json");
  // r28/r29 used the retained r26 profile. Continue gets a read-only copy once;
  // all subsequent writes/New Game stay in r30. Explicit test output never seeds.
  if(i<0&&!File.Exists(story.SavePathOverride)){
   string source=Path.Combine(Application.persistentDataPath,"r26-playtest","journey-v48.json");
   if(File.Exists(source))try{File.Copy(source,story.SavePathOverride,false);}catch(IOException e){Debug.LogWarning("r30 save copy: "+e.Message);}
  }
 }
}
