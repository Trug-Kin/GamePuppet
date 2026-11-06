using System.IO;
using UnityEngine;
using Unity.Collections;
using Newtonsoft.Json.Linq;

public sealed class AudioProbeV40:MonoBehaviour
{
 bool started;float until;StreamWriter log;
 void Start(){Directory.CreateDirectory("Docs/Reviews/journey_v40");log=new StreamWriter("Docs/Reviews/journey_v40/audio-probe.jsonl"){AutoFlush=true};started=AudioRenderer.Start();until=Time.realtimeSinceStartup+4;}
 void Update()
 {
  int count=AudioRenderer.GetSampleCountForCaptureFrame();int requested=count>0?count:Mathf.Max(1,Mathf.RoundToInt(Time.unscaledDeltaTime*AudioSettings.outputSampleRate));bool rendered=false;float peak=0;
  using(var data=new NativeArray<float>(requested*2,Allocator.Temp)){rendered=AudioRenderer.Render(data);for(int i=0;i<data.Length;i++)peak=Mathf.Max(peak,Mathf.Abs(data[i]));}
  var output=new float[1024];AudioListener.GetOutputData(output,0);float listenerPeak=0;foreach(var value in output)listenerPeak=Mathf.Max(listenerPeak,Mathf.Abs(value));
  log.WriteLine(new JObject{["started"]=started,["reported_samples"]=count,["requested"]=requested,["rendered"]=rendered,["peak"]=peak,["listener_peak"]=listenerPeak,["dsp_time"]=AudioSettings.dspTime,["sample_rate"]=AudioSettings.outputSampleRate,["speaker_mode"]=AudioSettings.speakerMode.ToString(),["pause"]=AudioListener.pause,["volume"]=AudioListener.volume,["dt"]=Time.unscaledDeltaTime}.ToString(Newtonsoft.Json.Formatting.None));
  if(Time.realtimeSinceStartup>until){if(started)AudioRenderer.Stop();log.Dispose();enabled=false;
#if UNITY_EDITOR
   UnityEditor.EditorApplication.isPlaying=false;
#endif
  }
 }
}
