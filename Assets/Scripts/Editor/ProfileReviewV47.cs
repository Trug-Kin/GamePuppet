using System.IO;
using Newtonsoft.Json.Linq;
using UnityEditorInternal;
public static class ProfileReviewV47
{
 public static void ExportAll()
 {
  foreach(var path in File.ReadAllLines("Docs/Reviews/journey_v47/profile-paths.txt"))
  {if(string.IsNullOrWhiteSpace(path))continue;File.WriteAllText("Docs/Reviews/journey_v47/profile-path.txt",path.Trim());Export();}
 }
 public static void Export()
 {
  var file=File.ReadAllText("Docs/Reviews/journey_v47/profile-path.txt").Trim();
  var flags=System.Reflection.BindingFlags.Static|System.Reflection.BindingFlags.Public|System.Reflection.BindingFlags.NonPublic;
  var capacity=typeof(ProfilerDriver).GetProperty("maxHistoryLength",flags);if(capacity!=null&&capacity.CanWrite)capacity.SetValue(null,20000);
  File.WriteAllText(Path.ChangeExtension(file,"history.txt"),"capacity_property="+(capacity!=null)+"\n"+string.Join("\n",System.Array.ConvertAll(typeof(ProfilerDriver).GetProperties(flags),p=>p.Name)));
  ProfilerDriver.LoadProfile(file,false);var frames=new JArray();
  for(int f=ProfilerDriver.firstFrameIndex;f<=ProfilerDriver.lastFrameIndex;f++)
  {
   var threads=new JArray();
   for(int t=0;t<256;t++)using(var view=ProfilerDriver.GetRawFrameDataView(f,t))
   {
    if(!view.valid)break;var samples=new JArray();
    for(int n=0;n<view.sampleCount;n++)if(view.GetSampleTimeMs(n)>=1)samples.Add(new JObject{["index"]=n,["name"]=view.GetSampleName(n),["start_ms"]=view.GetSampleStartTimeMs(n),["duration_ms"]=view.GetSampleTimeMs(n),["children"]=view.GetSampleChildrenCountRecursive(n)});
    if(samples.Count>0)threads.Add(new JObject{["thread"]=view.threadName,["samples"]=samples});
   }
   frames.Add(new JObject{["frame"]=f,["threads"]=threads});
  }
  File.WriteAllText(Path.ChangeExtension(file,"json"),frames.ToString());
 }
}
