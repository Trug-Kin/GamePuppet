using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
public static class ConRoiR31Review {
 public static void BuildDiagnostic(){Build("diagnostic");}
 public static void BuildFinal(){Build("final");}
 public static void BuildFinal2(){Build("final2");}
 static void Build(string suffix){
  const string scene="Assets/Scenes/JourneyRepair_v48_r30.unity";
  if(EditorBuildSettings.scenes.Count(x=>x.enabled)!=1||EditorBuildSettings.scenes.Single(x=>x.enabled).path!=scene)throw new Exception("Wrong scene input");
  foreach(var drive in new[]{"C:\\","D:\\"})if(new DriveInfo(drive).AvailableFreeSpace<2L*1024*1024*1024)throw new Exception("Insufficient staging space: "+drive);
  EditorSceneManager.OpenScene(scene);
  string exe=Path.Combine(Path.GetTempPath(),"ConRoi-r31-"+suffix,"ConRoi.exe");
  if(File.Exists(exe))throw new Exception("Refuse overwrite");Directory.CreateDirectory(Path.GetDirectoryName(exe));
  var report=BuildPipeline.BuildPlayer(new[]{scene},exe,BuildTarget.StandaloneWindows64,BuildOptions.Development|BuildOptions.DetailedBuildReport);
  Directory.CreateDirectory("Docs/Reviews/journey_v48/r31-gd2");
  File.WriteAllText("Docs/Reviews/journey_v48/r31-gd2/build-"+suffix+".json",new JObject{["result"]=report.summary.result.ToString(),["errors"]=report.summary.totalErrors,["warnings"]=report.summary.totalWarnings,["scene"]=scene,["binary"]=exe,["utc"]=DateTime.UtcNow.ToString("o")}.ToString());
  if(report.summary.result!=BuildResult.Succeeded)throw new Exception("Build failed");
 }
}
