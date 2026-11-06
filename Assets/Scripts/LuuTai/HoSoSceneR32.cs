// Chọn đường save riêng cho scene R32.
// Không gắn trực tiếp trong r92; checkpoint JourneyRepair_v48_r32.unity: Lake_Market_Story_GDD21.
// Reference public để đối chiếu Inspector (có nhánh tùy chọn): story.
// Thứ tự chạy khai báo: -31800; giữ quan hệ khởi tạo/pose với các owner khác.
using System;
using System.IO;
using UnityEngine;
[DefaultExecutionOrder(-31800)] [UnityEngine.Scripting.APIUpdating.MovedFrom(true, null, "Assembly-CSharp", "R32SceneProfile")]
public sealed class HoSoSceneR32:MonoBehaviour {
 public DieuPhoiHanhTrinh story;
 void Awake(){
  var args=Environment.GetCommandLineArgs();int i=Array.IndexOf(args,"--r30-review-output");
  string dir=i>=0&&i+1<args.Length?Path.GetFullPath(args[i+1]):Path.Combine(Application.persistentDataPath,"r32-playtest");
  Directory.CreateDirectory(dir);story.SavePathOverride=Path.Combine(dir,"journey-v48.json");
  // Copy only; original saves are never written. Continue accepts the existing state schema.
  if(i<0&&!File.Exists(story.SavePathOverride))foreach(var profile in new[]{"r30-playtest","r26-playtest"}){
   string src=Path.Combine(Application.persistentDataPath,profile,"journey-v48.json");if(!File.Exists(src))continue;
   try{File.Copy(src,story.SavePathOverride,false);}catch(IOException e){Debug.LogWarning("r32 profile copy: "+e.Message);}break;
  }
 }
}
