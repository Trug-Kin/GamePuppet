using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine;

// Packaging repair only. Never saves scene or changes project settings/assets.
public static class ConRoiR30StartupFix {
 const string Out="Docs/Reviews/journey_v48/r30-windows-fix";
 public static void Build(){BuildRun(1);}
 public static void Build2(){BuildRun(2);}
 public static void Build3(){BuildRun(3);}
 public static void Build4(){BuildRun(4);}
 public static void Build5(){BuildRun(5);}
 public static void Build6(){BuildRun(6);}
 static void BuildRun(int attempt){
  Directory.CreateDirectory(Out);
  if(new DriveInfo(Path.GetPathRoot(Path.GetFullPath("."))).AvailableFreeSpace<2L*1024*1024*1024)throw new Exception("Less than 2 GiB free on project drive; refusing serialized-data build");
  if(new DriveInfo(Path.GetPathRoot(Path.GetTempPath())).AvailableFreeSpace<4L*1024*1024*1024)throw new Exception("Less than 4 GiB free on output/cache drive; refusing build copy");
  EditorSceneManager.OpenScene(ConRoiR30.Scene);
  if(EditorBuildSettings.scenes.Count(x=>x.enabled)!=1||EditorBuildSettings.scenes.Single(x=>x.enabled).path!=ConRoiR30.Scene)throw new Exception("Wrong scene input");
  var cb=new JObject();
  foreach(var kind in new[]{typeof(IPreprocessBuildWithReport),typeof(IPostprocessBuildWithReport),typeof(IProcessSceneWithReport)})cb[kind.Name]=new JArray(TypeCache.GetTypesDerivedFrom(kind).Select(t=>new JObject{["type"]=t.FullName,["assembly"]=t.Assembly.GetName().Name}));
  File.WriteAllText(Out+"/build-callbacks.json",cb.ToString());
  string exe=Path.Combine(Path.GetTempPath(),"ConRoi-r30-startupfix-build"+attempt,"ConRoi.exe");
  if(File.Exists(exe))throw new Exception("Output exists; refuse overwrite");Directory.CreateDirectory(Path.GetDirectoryName(exe));
  var report=BuildPipeline.BuildPlayer(new[]{ConRoiR30.Scene},exe,BuildTarget.StandaloneWindows64,BuildOptions.Development|BuildOptions.DetailedBuildReport|BuildOptions.CleanBuildCache);
  var packed=new JArray(report.packedAssets.Select(p=>new JObject{["file"]=p.shortPath,["contents"]=new JArray(p.contents.Select(c=>new JObject{["source"]=c.sourceAssetPath,["bytes"]=c.packedSize}))}));
  File.WriteAllText(Out+"/build"+attempt+"-packed-assets.json",packed.ToString());
  File.WriteAllText(Out+"/build"+attempt+".json",new JObject{["result"]=report.summary.result.ToString(),["errors"]=report.summary.totalErrors,["warnings"]=report.summary.totalWarnings,["scene"]=ConRoiR30.Scene,["binary"]=exe,["bytes"]=report.summary.totalSize,["utc"]=DateTime.UtcNow.ToString("o"),["settings_changed"]=false,["models_or_scene_changed"]=false}.ToString());
  if(report.summary.result!=BuildResult.Succeeded)throw new Exception("Build failed");
 }
}
