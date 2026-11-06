// Đo chi phí tác vụ V46 khi profiling được bật.
// Component trong scene r92: Lake_Market_Story_GDD21.
// Thứ tự chạy khai báo: -29999; giữ quan hệ khởi tạo/pose với các owner khác.
using System;
using System.IO;
using UnityEngine;
using Unity.Profiling;

// Previous completed-frame CPU samples aligned with FrameHitchV45's previous frame wall time.
[DefaultExecutionOrder(-29999)]
[UnityEngine.Scripting.APIUpdating.MovedFrom(true, null, "Assembly-CSharp", "TaskAttributionV46")]
public sealed class DoChiPhiTacVuV46:MonoBehaviour
{
 string[] names={"V46.Player.Movement","V46.Player.InteractionRaycast","V46.Lake.Reflection","V45.NPCActing","V46.HumanSurface.Deform","Main Thread","Render Thread","GC.Collect","GC.Alloc","Gfx.WaitForPresentOnGfxThread"};
 ProfilerRecorder[] recorders;StreamWriter file;
 void Start()
 {
  var args=Environment.GetCommandLineArgs();int n=Array.IndexOf(args,"--verification-output");if(n<0){enabled=false;return;}
  file=new StreamWriter(Path.Combine(args[n+1],"task-attribution-v46.csv"));file.WriteLine("observed_frame,completed_frame,"+string.Join(",",names));
  recorders=new ProfilerRecorder[names.Length];for(int i=0;i<names.Length;i++)recorders[i]=ProfilerRecorder.StartNew(i==8?ProfilerCategory.Memory:ProfilerCategory.Internal,names[i],1);
  // Custom markers are in Scripts; the engine counters keep their native categories.
  for(int i=0;i<5;i++){recorders[i].Dispose();recorders[i]=ProfilerRecorder.StartNew(ProfilerCategory.Scripts,names[i],1);}
 }
 void Update(){if(file==null)return;file.Write(Time.frameCount+","+(Time.frameCount-1));foreach(var r in recorders)file.Write(","+(r.Valid?r.LastValue:-1));file.WriteLine();if(Time.frameCount%300==0)file.Flush();}
 void OnDestroy(){file?.Dispose();if(recorders!=null)foreach(var r in recorders)r.Dispose();}
 void OnApplicationQuit(){file?.Flush();}
}
