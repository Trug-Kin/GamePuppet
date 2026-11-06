// Quan sát hiển thị/cửa sổ và ghi bằng chứng thụ động; chỉ bật với --verification-output và --visible-validation.
// Component trong scene r92: Lake_Market_Story_GDD21.
// Reference public để đối chiếu Inspector (có nhánh tùy chọn): story.
// Code tự tìm/tạo: Camera, C3ActualPlayRun.
// Thứ tự chạy khai báo: 30000; giữ quan hệ khởi tạo/pose với các owner khác.
using System;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Rendering;
using Newtonsoft.Json.Linq;

// Passive evidence only, opt-in; does not move, aim, load saves or inject input.
[DefaultExecutionOrder(30000)]
[UnityEngine.Scripting.APIUpdating.MovedFrom(true, null, "Assembly-CSharp", "VisibleEvidenceV47")]
public sealed class GhiBangChungHienThiV47:MonoBehaviour
{
 [DllImport("user32.dll")]static extern IntPtr GetForegroundWindow();
 [DllImport("user32.dll")]static extern uint GetWindowThreadProcessId(IntPtr hwnd,out uint pid);
 public DieuPhoiHanhTrinh story;StreamWriter file;Camera eye;int renders;double previous;string output;bool handoff;
 void Start()
 {
  var args=Environment.GetCommandLineArgs();int n=Array.IndexOf(args,"--verification-output");
  if(n<0||Array.IndexOf(args,"--visible-validation")<0){enabled=false;return;}
  output=args[n+1];handoff=Array.IndexOf(args,"--handoff-probe")>=0;eye=story.player.GetComponentInChildren<Camera>();
  file=new StreamWriter(Path.Combine(output,"visible-frames.csv"));file.WriteLine("frame,time,wall_ms,region,x,y,z,focused,foreground,camera_enabled,screen_target,camera_renders,menu,paused,loading,loads,hook,runner");
  RenderPipelineManager.endCameraRendering+=Rendered;
  File.WriteAllText(Path.Combine(output,"visible-startup.json"),new JObject{["args"]=new JArray(args),["pid"]=System.Diagnostics.Process.GetCurrentProcess().Id,["mode"]=handoff?"normal player launch with passive observer; native OS input":"opt-in route driver; live presented gameplay camera",["capture"]=Array.IndexOf(args,"--record-gameplay")>=0,["startup_readback"]="Existing loading gate does one 1x1 readback; exclude startup from lake measurement"}.ToString());
 }
 void Rendered(ScriptableRenderContext c,Camera cam){if(cam==eye)renders++;}
 void LateUpdate()
 {
  if(file==null)return;double now=Time.realtimeSinceStartupAsDouble;var p=story.player.transform.position;uint pid;GetWindowThreadProcessId(GetForegroundWindow(),out pid);
  file.WriteLine(FormattableString.Invariant($"{Time.frameCount},{now:F6},{(previous>0?(now-previous)*1000:0):F4},{story.data.region},{p.x:F5},{p.y:F5},{p.z:F5},{Application.isFocused},{pid==System.Diagnostics.Process.GetCurrentProcess().Id},{eye.enabled},{eye.targetTexture==null},{renders},{story.MenuOpen},{story.Paused},{story.Loading},{story.SuccessfulLoads},{story.player.TestInputHookActive},{ReviewRunnerPresent}"));previous=now;renders=0;
  if(Time.frameCount%120==0)file.Flush();
  if(handoff&&now>65)Application.Quit();
 }
#if UNITY_EDITOR || CON_ROI_BUILD_REVIEW
 bool ReviewRunnerPresent => story.player.GetComponent<C3ActualPlayRun>()!=null;
#else
 bool ReviewRunnerPresent => false;
#endif
 void OnApplicationQuit(){file?.Flush();}
 void OnDestroy(){file?.Dispose();RenderPipelineManager.endCameraRendering-=Rendered;}
}
