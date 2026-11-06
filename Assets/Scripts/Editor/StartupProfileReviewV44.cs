using System.IO;
using Newtonsoft.Json.Linq;
using UnityEditorInternal;
public static class StartupProfileReviewV44
{
 public static void Export()
 {
  var file=File.ReadAllText("Docs/Reviews/journey_v44/profile-path.txt").Trim();
  ProfilerDriver.LoadProfile(file,false);
  var frames=new JArray();
  for(int f=ProfilerDriver.firstFrameIndex;f<=ProfilerDriver.lastFrameIndex;f++)
  {
   var threads=new JArray();
   for(int t=0;t<256;t++)using(var view=ProfilerDriver.GetRawFrameDataView(f,t))
   {
    if(!view.valid)break;var samples=new JArray();
    for(int n=0;n<view.sampleCount;n++)if(view.GetSampleTimeMs(n)>=.5f)samples.Add(new JObject{["index"]=n,["name"]=view.GetSampleName(n),["start_ms"]=view.GetSampleStartTimeMs(n),["duration_ms"]=view.GetSampleTimeMs(n),["children"]=view.GetSampleChildrenCountRecursive(n)});
    if(samples.Count>0)threads.Add(new JObject{["thread"]=view.threadName,["samples"]=samples});
   }
   frames.Add(new JObject{["frame"]=f,["threads"]=threads});
  }
  File.WriteAllText(Path.ChangeExtension(file,"json"),frames.ToString());
 }
}
