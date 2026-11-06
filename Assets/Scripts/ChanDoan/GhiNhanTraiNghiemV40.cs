// Ghi nhận trải nghiệm theo cấu hình quan sát hiện có; không dùng làm driver thao tác.
// Component trong scene r92: Main Camera.
// Reference public để đối chiếu Inspector (có nhánh tùy chọn): recorder.
// Code tự tìm/tạo: Camera, AudioListener, ExperienceAudioTapV40.
using System;
using System.Collections;
using System.IO;
using UnityEngine;

// Records the running player's camera, including its camera-space UI, and the
// actual AudioListener mix. Enabled only by --record-gameplay <folder>.
[UnityEngine.Scripting.APIUpdating.MovedFrom(true, null, "Assembly-CSharp", "ExperienceRecorderV40")]
public sealed class GhiNhanTraiNghiemV40 : MonoBehaviour
{
 Camera cameraSource;
 RenderTexture target;
 Texture2D pixels;
 string folder;
 StreamWriter index;
 BinaryWriter audio;
 readonly object audioLock=new object();
 bool closing;
 bool engineAudio;
 int frame;
 double next;
 double audioAt,audioFraction;
 public const int Fps=12;
 int captureFps=Fps;
 void Start()
 {
  var args=Environment.GetCommandLineArgs();int i=Array.IndexOf(args,"--record-gameplay");
  int suite=Array.IndexOf(args,"--verify-suite");if(suite>=0&&suite+1<args.Length&&args[suite+1]=="acting")captureFps=24;
  string requested=i>=0&&i+1<args.Length?args[i+1]:"";
#if UNITY_EDITOR
  if(string.IsNullOrEmpty(requested)){var session=UnityEditor.SessionState.GetString("V40Record","");if(!string.IsNullOrEmpty(session))requested=Path.Combine(session,"recording");}
#endif
  if(string.IsNullOrEmpty(requested)){enabled=false;return;}
  folder=Path.GetFullPath(requested);Directory.CreateDirectory(folder);
  cameraSource=GetComponent<Camera>();target=new RenderTexture(1280,720,24,RenderTextureFormat.ARGB32);target.Create();if(Array.IndexOf(args,"--visible-validation")<0)cameraSource.targetTexture=target;
  pixels=new Texture2D(1280,720,TextureFormat.RGB24,false);
  index=new StreamWriter(Path.Combine(folder,"frames.csv")){AutoFlush=true};index.WriteLine("frame,realtime_seconds");
  audio=new BinaryWriter(File.Open(Path.Combine(folder,"listener.f32"),FileMode.Create,FileAccess.Write,FileShare.Read));
  engineAudio=AudioRenderer.Start();
  audioAt=Time.realtimeSinceStartupAsDouble;
  if(!engineAudio){var listener=UnityEngine.Object.FindAnyObjectByType<AudioListener>();if(!listener)listener=gameObject.AddComponent<AudioListener>();listener.gameObject.AddComponent<ExperienceAudioTapV40>().recorder=this;}
  File.WriteAllText(Path.Combine(folder,"capture.json"),"{\"source\":\"live player GPU camera + camera-space UI; Unity main mixer output\",\"editor\":"+(Application.isEditor?"true":"false")+",\"audio_renderer_started\":"+(engineAudio?"true":"false")+",\"fps\":12,\"sample_rate\":"+AudioSettings.outputSampleRate+",\"channels\":2,\"input\":\"See verification result; recording does not imply OS input\",\"time_scale\":1}");
  var metaPath=Path.Combine(folder,"capture.json");File.WriteAllText(metaPath,File.ReadAllText(metaPath).Replace("\"fps\":12","\"fps\":"+captureFps));
  next=Time.realtimeSinceStartupAsDouble;StartCoroutine(Record());
 }
 void LateUpdate()
 {
  if(!engineAudio||closing||audio==null)return;
  double now=Time.realtimeSinceStartupAsDouble,samples=(now-audioAt)*AudioSettings.outputSampleRate+audioFraction;audioAt=now;
  int count=AudioRenderer.GetSampleCountForCaptureFrame();
  if(count<=0){count=(int)samples;audioFraction=samples-count;}else audioFraction=0;
  if(count<=0)return;
  using(var buffer=new Unity.Collections.NativeArray<float>(count*2,Unity.Collections.Allocator.Temp))
  {if(AudioRenderer.Render(buffer)){var bytes=buffer.Reinterpret<byte>(sizeof(float)).ToArray();lock(audioLock)audio.Write(bytes);}}
 }
 IEnumerator Record()
 {
  while(!closing)
  {
   yield return new WaitForEndOfFrame();
   double now=Time.realtimeSinceStartupAsDouble;if(now<next)continue;
   var previous=RenderTexture.active;
   try{UnityEngine.Rendering.RenderPipeline.SubmitRenderRequest(cameraSource,new UnityEngine.Rendering.Universal.UniversalRenderPipeline.SingleCameraRequest{destination=target});RenderTexture.active=target;pixels.ReadPixels(new Rect(0,0,1280,720),0,0);pixels.Apply(false);File.WriteAllBytes(Path.Combine(folder,"frame_"+frame.ToString("D6")+".jpg"),pixels.EncodeToJPG(85));index.WriteLine(frame+","+now.ToString("F6",System.Globalization.CultureInfo.InvariantCulture));frame++;}
   finally{RenderTexture.active=previous;}
   next=now+1.0/captureFps;
  }
 }
 public void AppendAudio(float[] data,int channels)
 {
  lock(audioLock){if(audio==null||closing)return;var bytes=new byte[data.Length*sizeof(float)];Buffer.BlockCopy(data,0,bytes,0,bytes.Length);audio.Write(bytes);}
 }
 void OnApplicationQuit(){Close();}
 void OnDestroy(){Close();}
 void Close()
 {
  if(closing)return;closing=true;if(engineAudio)AudioRenderer.Stop();index?.Dispose();lock(audioLock){audio?.Dispose();audio=null;}
  if(cameraSource&&cameraSource.targetTexture==target)cameraSource.targetTexture=null;
  if(target){target.Release();Destroy(target);}if(pixels)Destroy(pixels);
 }
}

public sealed class ExperienceAudioTapV40:MonoBehaviour
{
 public GhiNhanTraiNghiemV40 recorder;
 void OnAudioFilterRead(float[] data,int channels){if(recorder)recorder.AppendAudio(data,channels);}
}
