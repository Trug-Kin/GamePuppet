// Chọn đường save riêng cho scene R33.
// Không gắn trực tiếp trong r92; checkpoint JourneyRepair_v48_r33.unity: Lake_Market_Story_GDD21.
// Reference public để đối chiếu Inspector (có nhánh tùy chọn): story.
// Thứ tự chạy khai báo: -31800; giữ quan hệ khởi tạo/pose với các owner khác.
using System;
using System.IO;
using UnityEngine;
[DefaultExecutionOrder(-31800)] [UnityEngine.Scripting.APIUpdating.MovedFrom(true, null, "Assembly-CSharp", "R33SceneProfile")]
public sealed class HoSoSceneR33:MonoBehaviour {
 public DieuPhoiHanhTrinh story;
 void Awake(){
  var args=Environment.GetCommandLineArgs();int i=Array.IndexOf(args,"--r30-review-output");
  string dir=i>=0&&i+1<args.Length?Path.GetFullPath(args[i+1]):Path.Combine(Application.persistentDataPath,"r33-chair-playtest");
  Directory.CreateDirectory(dir);story.SavePathOverride=Path.Combine(dir,"journey-v48.json");
  if(i<0&&!File.Exists(story.SavePathOverride))foreach(var profile in new[]{"r32-playtest","r30-playtest","r26-playtest"}){
   string src=Path.Combine(Application.persistentDataPath,profile,"journey-v48.json");if(!File.Exists(src))continue;
   try{File.Copy(src,story.SavePathOverride,false);}catch(IOException e){Debug.LogWarning("r33 profile copy: "+e.Message);}break;
  }
 }
}
