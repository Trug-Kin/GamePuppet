#if UNITY_EDITOR || CON_ROI_BUILD_REVIEW
using System.IO;
using UnityEngine;
// Opt-in evidence: PCM from the game's listener, never synthesized or added in post.
public sealed class R42AudioCapture:MonoBehaviour
{
 public string output;
 readonly object gate=new object();BinaryWriter writer;int samples,channels,rate;double energy;float peak;float started;
 void Start(){started=Time.realtimeSinceStartup;rate=AudioSettings.outputSampleRate;writer=new BinaryWriter(File.Create(output));writer.Write(new byte[44]);}
 void OnAudioFilterRead(float[] data,int count){lock(gate){if(writer==null)return;channels=count;foreach(float x in data){float v=Mathf.Clamp(x,-1,1);writer.Write((short)(v*32767));samples++;energy+=v*v;peak=Mathf.Max(peak,Mathf.Abs(v));}}}
 void OnApplicationQuit(){OnDestroy();}
 void OnDestroy(){lock(gate){if(writer==null)return;writer.Seek(0,SeekOrigin.Begin);writer.Write(System.Text.Encoding.ASCII.GetBytes("RIFF"));writer.Write(36+samples*2);writer.Write(System.Text.Encoding.ASCII.GetBytes("WAVEfmt "));writer.Write(16);writer.Write((short)1);writer.Write((short)Mathf.Max(1,channels));writer.Write(rate);writer.Write(rate*Mathf.Max(1,channels)*2);writer.Write((short)(Mathf.Max(1,channels)*2));writer.Write((short)16);writer.Write(System.Text.Encoding.ASCII.GetBytes("data"));writer.Write(samples*2);writer.Dispose();writer=null;File.WriteAllText(output+".json","{\"startRealtime\":"+started.ToString(System.Globalization.CultureInfo.InvariantCulture)+",\"samples\":"+samples+",\"rate\":"+rate+",\"channels\":"+channels+",\"peak\":"+peak.ToString(System.Globalization.CultureInfo.InvariantCulture)+",\"rms\":"+System.Math.Sqrt(energy/System.Math.Max(1,samples)).ToString(System.Globalization.CultureInfo.InvariantCulture)+",\"source\":\"Unity AudioListener OnAudioFilterRead; device listening NOT_VERIFIED\"}");}}
}

#endif
