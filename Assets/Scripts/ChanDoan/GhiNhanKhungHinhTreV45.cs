// Ghi thời gian frame/GC và vị trí vào CSV khi được bật để điều tra spike.
// Component trong scene r92: Lake_Market_Story_GDD21.
// Reference public để đối chiếu Inspector (có nhánh tùy chọn): story.
// Code tự tìm/tạo: TuongTacNguoiChoi.
// Thứ tự chạy khai báo: -30000; giữ quan hệ khởi tạo/pose với các owner khác.
using System;
using System.IO;
using UnityEngine;
using UnityEngine.Profiling;
using Unity.Profiling;

// Position, action and allocation evidence remains active without opt-in profiler.
[DefaultExecutionOrder(-30000)]
[UnityEngine.Scripting.APIUpdating.MovedFrom(true, null, "Assembly-CSharp", "FrameHitchV45")]
public sealed class GhiNhanKhungHinhTreV45:MonoBehaviour
{
 public DieuPhoiHanhTrinh story;
 StreamWriter file;double previous;long allocation;int collections;Vector3 oldPosition;string oldTarget="";int oldRegion;
 ProfilerRecorder gc;bool profile,started,closed;float until;string directory;
 void Start()
 {
  var args=Environment.GetCommandLineArgs();int i=Array.IndexOf(args,"--verification-output");if(i<0){enabled=false;return;}
  directory=args[i+1];file=new StreamWriter(Path.Combine(directory,"hitch-context-v45.csv"));file.WriteLine("frame,time,wall_ms,previous_region,region,previous_x,previous_y,previous_z,previous_target,moved_m,gc_allocated_bytes,gc_collections,controls,dialogue");
  gc=ProfilerRecorder.StartNew(ProfilerCategory.Memory,"GC Allocated In Frame");profile=Array.IndexOf(args,"--profile-acting")>=0;oldPosition=story.player.transform.position;
 }
 void Update()
 {
  double now=Time.realtimeSinceStartupAsDouble;var pos=story.player.transform.position;var target=story.player.GetComponent<TuongTacNguoiChoi>().Target;string name=target==null?"":target.GetType().Name;
  int current=GC.CollectionCount(0);
  if(previous>0)file.WriteLine(FormattableString.Invariant($"{Time.frameCount},{now:F6},{(now-previous)*1000:F4},{oldRegion},{story.data.region},{oldPosition.x:F5},{oldPosition.y:F5},{oldPosition.z:F5},{oldTarget},{Vector3.Distance(pos,oldPosition):F6},{(gc.Valid?gc.LastValue:0)},{current-collections},{story.player.enabled},{story.polish.DialogueVisible}"));
  previous=now;oldPosition=pos;oldTarget=name;oldRegion=story.data.region;collections=current;if(Time.frameCount%300==0)file.Flush();
  if(profile&&!started&&story.data.region==2&&pos.x< -100&&pos.z< -44){started=true;until=Time.realtimeSinceStartup+12;Profiler.logFile=Path.Combine(directory,"school-motion.raw");Profiler.enableBinaryLog=true;Profiler.enabled=true;}
  if(started&&!closed&&Time.realtimeSinceStartup>until){closed=true;Profiler.enabled=false;Profiler.enableBinaryLog=false;}
 }
 void OnDestroy(){file?.Dispose();if(gc.Valid)gc.Dispose();if(profile&&started){Profiler.enabled=false;Profiler.enableBinaryLog=false;}}
 void OnApplicationQuit(){file?.Flush();}
}
