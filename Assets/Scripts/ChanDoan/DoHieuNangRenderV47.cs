// Đo render player theo cấu hình profiling opt-in hiện có.
// Component trong scene r92: Main Camera.
// Code tự tìm/tạo: Camera.
using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

// Opt-in offscreen GPU presentation, with no readback, screenshots or encoding.
// This verifies a rendered workload in a hidden process, not desktop presentation.
[UnityEngine.Scripting.APIUpdating.MovedFrom(true, null, "Assembly-CSharp", "ProfilePlayerRenderV47")]
public sealed class DoHieuNangRenderV47:MonoBehaviour
{
 Camera eye;RenderTexture target;RenderTexture previous;
 static readonly Unity.Profiling.ProfilerMarker marker=new Unity.Profiling.ProfilerMarker("V47.Profile.PlayerRender");
 IEnumerator Start()
 {
  var args=Environment.GetCommandLineArgs();if(Array.IndexOf(args,"--profile-player-render")<0){enabled=false;yield break;}
  eye=GetComponent<Camera>();previous=eye.targetTexture;target=new RenderTexture(1280,720,24,RenderTextureFormat.ARGB32);target.Create();eye.targetTexture=target;
  int n=Array.IndexOf(args,"--verification-output");if(n>=0)File.WriteAllText(Path.Combine(args[n+1],"render-workload.json"),"{\"mode\":\"Explicit live player camera GPU render every frame; no pixel readback or encoder; hidden window, not presented swapchain\",\"width\":1280,\"height\":720}");
  while(true){yield return new WaitForEndOfFrame();var active=RenderTexture.active;try{using(marker.Auto())RenderPipeline.SubmitRenderRequest(eye,new UniversalRenderPipeline.SingleCameraRequest{destination=target});}finally{RenderTexture.active=active;}}
 }
 void OnDestroy(){if(eye&&eye.targetTexture==target)eye.targetTexture=previous;if(target){target.Release();Destroy(target);}}
}
