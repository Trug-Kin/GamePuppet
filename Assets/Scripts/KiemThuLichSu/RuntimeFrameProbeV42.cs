using System;
using System.IO;
using System.Globalization;
using UnityEngine;
using UnityEngine.Rendering;

// Wall-clock intervals from consecutive live player frames. Video rate is unrelated.
public sealed class RuntimeFrameProbeV42:MonoBehaviour
{
 public DieuPhoiHanhTrinh story;
 StreamWriter output;double previous;int rows;bool recording;
 Camera mainCamera;int cameraRenders;
 readonly FrameTiming[] timings=new FrameTiming[1];
 void Start()
 {
  var args=Environment.GetCommandLineArgs();int i=Array.IndexOf(args,"--verification-output");
  string dir=i>=0&&i+1<args.Length?args[i+1]:Path.Combine(Application.persistentDataPath,"v42-perf",DateTime.UtcNow.ToString("yyyyMMdd_HHmmss"));
  Directory.CreateDirectory(dir);recording=Array.IndexOf(args,"--record-gameplay")>=0;
  mainCamera=story.player.GetComponentInChildren<Camera>();RenderPipelineManager.endCameraRendering+=Rendered;
  output=new StreamWriter(Path.Combine(dir,"runtime-frame-times.csv"));output.WriteLine("frame,time,area,wall_ms,cpu_ms,gpu_ms,frame_timing_samples,recording,width,height,graphics_api,main_camera_renders_previous_frame");
  File.WriteAllText(Path.Combine(dir,"runtime-device.txt"),SystemInfo.deviceModel+"\n"+SystemInfo.processorType+"\n"+SystemInfo.graphicsDeviceName+"\n"+SystemInfo.graphicsDeviceVersion+"\nDevelopment="+Debug.isDebugBuild+"\nGPU zero/missing means unavailable; wall intervals include telemetry/verification overhead.\n");
 }
 void Update()
 {
  double now=Time.realtimeSinceStartupAsDouble,dt=previous>0?(now-previous)*1000:0;previous=now;
  FrameTimingManager.CaptureFrameTimings();uint count=FrameTimingManager.GetLatestTimings(1,timings);
  var d=story.data;string area=!string.IsNullOrEmpty(d.ending)?"Ending":d.region<0?"Hub":new[]{"Lake","Market","School","Slope"}[d.region];
  if(dt>0)output.WriteLine(string.Format(CultureInfo.InvariantCulture,"{0},{1:F5},{2},{3:F4},{4:F4},{5:F4},{6},{7},{8},{9},{10},{11}",Time.frameCount,now,area,dt,count>0?timings[0].cpuFrameTime:0,count>0?timings[0].gpuFrameTime:0,count,recording,Screen.width,Screen.height,SystemInfo.graphicsDeviceType,cameraRenders));
  cameraRenders=0;
  if(++rows%300==0)output.Flush();
 }
 void Rendered(ScriptableRenderContext context,Camera camera){if(camera==mainCamera)cameraRenders++;}
 void OnDestroy(){RenderPipelineManager.endCameraRendering-=Rendered;output?.Dispose();}
 void OnApplicationQuit(){output?.Flush();}
}
