#if UNITY_EDITOR || CON_ROI_BUILD_REVIEW
using System.Collections;
using System.IO;
using Newtonsoft.Json.Linq;
using UnityEngine;

public sealed partial class C3ActualPlayRun
{
 IEnumerator CapturePlayerImage(string path)
 {
  if(System.Array.IndexOf(System.Environment.GetCommandLineArgs(),"--no-captures")>=0)yield break;
  double captureStarted=Time.realtimeSinceStartupAsDouble;int captureFrame=Time.frameCount;
  UnityEngine.Profiling.Profiler.BeginSample("V45.CapturePlayerImage");
  // A hidden Windows window may not present a swapchain image. Render the live
  // player camera to a real GPU target instead of accepting a black screenshot.
  var camera=player.GetComponentInChildren<Camera>();var prior=camera.targetTexture;var active=RenderTexture.active;
  var target=RenderTexture.GetTemporary(1280,720,24,RenderTextureFormat.ARGB32);camera.targetTexture=target;
  // Keep the render request/readback/release in one frame. The former coroutine
  // retained a temporary GPU target across a yield and manually rendered twice.
  UnityEngine.Rendering.RenderPipeline.SubmitRenderRequest(camera,new UnityEngine.Rendering.Universal.UniversalRenderPipeline.SingleCameraRequest{destination=target});RenderTexture.active=target;
  var texture=new Texture2D(1280,720,TextureFormat.RGB24,false);texture.ReadPixels(new Rect(0,0,1280,720),0,0);texture.Apply();
  File.WriteAllBytes(path,texture.EncodeToPNG());var pixels=texture.GetPixels32();int low=255,high=0,nonBlack=0,white=0;
  for(int i=0;i<pixels.Length;i+=16){var p=pixels[i];int v=Mathf.Max(p.r,Mathf.Max(p.g,p.b));low=Mathf.Min(low,v);high=Mathf.Max(high,v);if(v>5)nonBlack++;if(p.r>=250&&p.g>=250&&p.b>=250)white++;}
  var volume=UnityEngine.Rendering.VolumeManager.instance.stack;
  var grade=volume.GetComponent<UnityEngine.Rendering.Universal.ColorAdjustments>();var bloom=volume.GetComponent<UnityEngine.Rendering.Universal.Bloom>();
  File.AppendAllText(Path.Combine(folder,"visual_stats.jsonl"),new JObject{["image"]=path,["capture"]="Live player camera rendered to GPU texture",["minimum_channel"]=low,["maximum_channel"]=high,["non_black_fraction"]=nonBlack/(pixels.Length/16f),["near_white_fraction"]=white/(pixels.Length/16f),["region"]=walkingStory?walkingStory.data.region:-99,["eye"]=camera.transform.position.ToString("F3"),["view"]=camera.transform.eulerAngles.ToString("F3"),["sun"]=RenderSettings.sun?RenderSettings.sun.intensity:0,["fog"]=RenderSettings.fog,["fog_color"]=RenderSettings.fogColor.ToString(),["fog_end"]=RenderSettings.fogEndDistance,["post_exposure"]=grade?grade.postExposure.value:0,["bloom_intensity"]=bloom?bloom.intensity.value:0}.ToString(Newtonsoft.Json.Formatting.None)+"\n");
  Object.Destroy(texture);RenderTexture.active=active;camera.targetTexture=prior;RenderTexture.ReleaseTemporary(target);
  UnityEngine.Profiling.Profiler.EndSample();
  if(walkingStory&&walkingStory.GetComponent<DienXuatHanhTrinhV45>())File.AppendAllText(Path.Combine(VerificationOutput,"evidence-capture-cost.csv"),System.FormattableString.Invariant($"{captureFrame},{captureStarted:F6},{(Time.realtimeSinceStartupAsDouble-captureStarted)*1000:F4},{path}\n"));
 }
}

#endif
