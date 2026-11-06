using System.IO;
using System.Linq;
using UnityEditor.Build.Reporting;
using Newtonsoft.Json.Linq;

// Reads the last completed build; never builds or saves a scene.
public static class ConRoiV48PackageAudit
{
 public static void Inspect()
 {
  var report=UnityEditorInternal.InternalEditorUtility.LoadSerializedFileAndForget("Library/LastBuild.buildreport").OfType<BuildReport>().First();
  File.WriteAllText("Docs/Reviews/journey_v48/r11/build-assets.json",new JObject{
   ["output"]=report.summary.outputPath,["result"]=report.summary.result.ToString(),["started"]=report.summary.buildStartedAt.ToString("o"),
   ["assets"]=new JArray(report.packedAssets.SelectMany(p=>p.contents).Select(c=>c.sourceAssetPath).Distinct().OrderBy(p=>p))
  }.ToString());
 }
}
