// Quan sát input/control runtime và ghi CSV; không thay controller.
// Component trong scene r92: Lake_Market_Story_GDD21.
// Reference public để đối chiếu Inspector (có nhánh tùy chọn): story.
// Thứ tự chạy khai báo: 31000; giữ quan hệ khởi tạo/pose với các owner khác.
using System;
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;

[DefaultExecutionOrder(31000)]
[UnityEngine.Scripting.APIUpdating.MovedFrom(true, null, "Assembly-CSharp", "RuntimeControlProbeV43")]
public sealed class ChanDoanDieuKhienV43:MonoBehaviour
{
 public DieuPhoiHanhTrinh story;
 StreamWriter file;double previous,entered;int region=-99;bool previousControl;
 void Start()
 {
  var a=Environment.GetCommandLineArgs();int i=Array.IndexOf(a,"--verification-output");if(i<0)return;
  file=new StreamWriter(Path.Combine(a[i+1],"control-frame-times.csv"));file.WriteLine("frame,time,wall_ms,region,region_age_s,control_enabled,previous_control_enabled,focused,move_x,move_y,movement_frames,keyboard_native,keyboard_name,paused,phone,ending");
 }
 void LateUpdate()
 {
  double now=Time.realtimeSinceStartupAsDouble;bool controls=story.player.enabled&&story.player.GameplayActionsEnabled;
  if(region!=story.data.region){region=story.data.region;entered=now;}
  var k=Keyboard.current;var m=story.player.LastMovementInput;
  if(file!=null)file.WriteLine(FormattableString.Invariant($"{Time.frameCount},{now:F6},{(previous>0?(now-previous)*1000:0):F4},{region},{now-entered:F5},{controls},{previousControl},{Application.isFocused},{m.x:F4},{m.y:F4},{story.player.MovementFrames},{(k!=null&&k.native)},{k?.name},{story.Paused},{story.PhoneOpen},{story.data.ending}"));
  previous=now;previousControl=controls;if(Time.frameCount%300==0)file?.Flush();
 }
 void OnDestroy(){file?.Dispose();}
 void OnApplicationQuit(){file?.Flush();}
}
