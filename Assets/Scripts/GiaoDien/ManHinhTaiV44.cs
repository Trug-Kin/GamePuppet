// Giữ màn hình tải tới khi kiểm GPU readback hoàn tất; không thay bằng chờ thời gian cố định.
// Component trong scene r92: Lake_Market_Story_GDD21.
// Reference public để đối chiếu Inspector (có nhánh tùy chọn): story.
// Thứ tự chạy khai báo: -32000; giữ quan hệ khởi tạo/pose với các owner khác.
using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Rendering;

[DefaultExecutionOrder(-32000)]
[UnityEngine.Scripting.APIUpdating.MovedFrom(true, null, "Assembly-CSharp", "JourneyLoadingV44")]
public sealed class ManHinhTaiV44:MonoBehaviour
{
 public DieuPhoiHanhTrinh story;
 public bool Ready {get;private set;}
 public int ReleaseFrame {get;private set;}
 public double ReleaseTime {get;private set;}
 public bool GpuCompletionConfirmed {get;private set;}
 void Awake(){story.SetLoading(true);}
 IEnumerator Start()
 {
  // Render the actual room, Canvas, postprocessing and RenderGraph before input.
  // EndOfFrame includes the first UI/IMGUI pass. No elapsed-time threshold.
  yield return new WaitForEndOfFrame();
  // D3D11 advertises fences but rejects async-compute fence polling. Read back
  // a one-pixel submission ordered after the actual first frame instead.
  var pixel=RenderTexture.GetTemporary(1,1,0,RenderTextureFormat.ARGB32);
  var previousActive=RenderTexture.active;Graphics.Blit(Texture2D.blackTexture,pixel);RenderTexture.active=previousActive;
  bool synchronous=!SystemInfo.supportsAsyncGPUReadback;
  if(!synchronous)
  {
   var readback=AsyncGPUReadback.Request(pixel,0);
   while(!readback.done)yield return null;
   synchronous=readback.hasError;
  }
  if(synchronous){var old=RenderTexture.active;RenderTexture.active=pixel;var cpu=new Texture2D(1,1);cpu.ReadPixels(new Rect(0,0,1,1),0,0);RenderTexture.active=old;Destroy(cpu);}
  RenderTexture.ReleaseTemporary(pixel);GpuCompletionConfirmed=true;
  Ready=true;ReleaseFrame=Time.frameCount;ReleaseTime=Time.realtimeSinceStartupAsDouble;
  story.SetLoading(false);
  var args=Environment.GetCommandLineArgs();int i=Array.IndexOf(args,"--verification-output");
  if(i>=0)File.WriteAllText(Path.Combine(args[i+1],"loading-gate.json"),JsonUtility.ToJson(new Evidence{frame=ReleaseFrame,time=ReleaseTime,gpuReadback=GpuCompletionConfirmed,synchronousFallback=synchronous,reason="Actual first frame, UI and ordered GPU readback completed; no fixed wait"},true));
 }
 [Serializable]class Evidence{public int frame;public double time;public bool gpuReadback,synchronousFallback;public string reason;}
 void OnGUI(){if(!Ready)GUI.Box(new Rect(Screen.width*.5f-120,Screen.height*.5f-24,240,48),"\u0110ang t\u1ea3i khung c\u1ea3nh...");}
}
