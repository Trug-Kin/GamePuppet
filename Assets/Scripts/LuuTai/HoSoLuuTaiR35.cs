// Chọn profile save sớm; seed legacy một lần khi chưa có save và không có đối số review riêng.
// Component trong scene r92: Lake_Market_Story_GDD21.
// Reference public để đối chiếu Inspector (có nhánh tùy chọn): story.
// Thứ tự chạy khai báo: -31800; giữ quan hệ khởi tạo/pose với các owner khác.
// Awake -31800 chạy sau profile R26 -31900; không seed khi có --r30-review-output, không ghi đè save có sẵn.
using System;
using System.IO;
using UnityEngine;
[DefaultExecutionOrder(-31800)] [UnityEngine.Scripting.APIUpdating.MovedFrom(true, null, "Assembly-CSharp", "R35SceneProfile")]
public sealed class HoSoLuuTaiR35:MonoBehaviour {
 public DieuPhoiHanhTrinh story;
 public string saveProfile="r35-market";
 public bool seedLegacySave=true;
 void Awake(){
  var args=Environment.GetCommandLineArgs();int i=Array.IndexOf(args,"--r30-review-output");
  string dir=i>=0&&i+1<args.Length?Path.GetFullPath(args[i+1]):Path.Combine(Application.persistentDataPath,saveProfile);Directory.CreateDirectory(dir);story.SavePathOverride=Path.Combine(dir,"journey-v48.json");
  if(seedLegacySave&&i<0&&!File.Exists(story.SavePathOverride))foreach(var profile in new[]{"r34-class203","r33-chair-playtest","r32-playtest","r30-playtest","r26-playtest"}){
   string src=Path.Combine(Application.persistentDataPath,profile,"journey-v48.json");if(!File.Exists(src))continue;try{File.Copy(src,story.SavePathOverride,false);}catch(IOException e){Debug.LogWarning(e.Message);}break;
  }
 }
}
