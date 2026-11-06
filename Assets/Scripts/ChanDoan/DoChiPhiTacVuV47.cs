// Đo chi phí tác vụ V47 khi profiling được bật.
// Component trong scene r92: Lake_Market_Story_GDD21.
// Thứ tự chạy khai báo: -29998; giữ quan hệ khởi tạo/pose với các owner khác.
using System;
using System.IO;
using UnityEngine;
using Unity.Profiling;
using Unity.Profiling.LowLevel.Unsafe;
using System.Collections.Generic;

// Additional scopes, without changing v46's marker names or frame alignment.
[DefaultExecutionOrder(-29998)]
[UnityEngine.Scripting.APIUpdating.MovedFrom(true, null, "Assembly-CSharp", "TaskAttributionV47")]
public sealed class DoChiPhiTacVuV47:MonoBehaviour
{
 readonly string[] names={"V47.Reflection.StoryLookup","V47.Reflection.Prepare","V47.Reflection.Render","V47.Reflection.Bind","V47.Story.ApplyRegion","V47.Story.Save","V47.Story.Load","V47.Profile.PlayerRender","V47.Transition.PlayerRender","PostLateUpdate.BatchModeUpdate","Semaphore.WaitForSignal","PostLateUpdate.UpdateResolution","CoroutinesDelayedCalls"};
 ProfilerRecorder[] recorders;StreamWriter file;
 void Start()
 {
  var args=Environment.GetCommandLineArgs();int n=Array.IndexOf(args,"--verification-output");if(n<0){enabled=false;return;}
  file=new StreamWriter(Path.Combine(args[n+1],"task-attribution-v47.csv"));file.WriteLine("observed_frame,completed_frame,"+string.Join(",",names));
  var handles=new List<ProfilerRecorderHandle>();ProfilerRecorderHandle.GetAvailable(handles);
  recorders=new ProfilerRecorder[names.Length];
  for(int i=0;i<names.Length;i++)
  {
   if(names[i].StartsWith("V47.")){recorders[i]=ProfilerRecorder.StartNew(ProfilerCategory.Scripts,names[i],1);continue;}
   foreach(var handle in handles){var description=ProfilerRecorderHandle.GetDescription(handle);if(description.Name==names[i]){recorders[i]=ProfilerRecorder.StartNew(description.Category,names[i],1);break;}}
  }
 }
 void Update(){if(file==null)return;file.Write(Time.frameCount+","+(Time.frameCount-1));foreach(var r in recorders)file.Write(","+(r.Valid?r.LastValue:-1));file.WriteLine();if(Time.frameCount%300==0)file.Flush();}
 void OnDestroy(){file?.Dispose();if(recorders!=null)foreach(var r in recorders)r.Dispose();}
 void OnApplicationQuit(){file?.Flush();}
}
