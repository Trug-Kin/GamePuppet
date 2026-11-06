using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;

// Packaging-only diagnostic. All original project settings restored in finally.
public static class ConRoiR30BuildCompatibility {
 public static void Build(){
  EditorSceneManager.OpenScene(ConRoiR30.Scene);
  if(EditorBuildSettings.scenes.Count(x=>x.enabled)!=1||EditorBuildSettings.scenes.Single(x=>x.enabled).path!=ConRoiR30.Scene)throw new Exception("Wrong build input");
  var flags=BindingFlags.Public|BindingFlags.NonPublic|BindingFlags.Static;
  var settings=new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/ProjectSettings.asset")[0]);
  var compression=settings.FindProperty("VertexChannelCompressionMask");
  if(compression==null)throw new Exception("Compression setting unavailable; refusing guessed settings");
  int original=compression.intValue;
  string exe=Path.Combine(Path.GetTempPath(),"ConRoi-r30-Windows-compatible","ConRoi.exe");
  Directory.CreateDirectory(Path.GetDirectoryName(exe));
  try {
   compression.intValue=0;settings.ApplyModifiedPropertiesWithoutUndo();
   var report=BuildPipeline.BuildPlayer(new[]{ConRoiR30.Scene},exe,BuildTarget.StandaloneWindows64,BuildOptions.DetailedBuildReport|BuildOptions.CleanBuildCache);
   File.WriteAllText(ConRoiR30.Out+"/build-compatibility.json",new JObject{["result"]=report.summary.result.ToString(),["errors"]=report.summary.totalErrors,["warnings"]=report.summary.totalWarnings,["scene"]=ConRoiR30.Scene,["binary"]=exe,["bytes"]=report.summary.totalSize,["utc"]=DateTime.UtcNow.ToString("o"),["development"]=false,["vertex_compression_mask"]=0,["original_compression_mask"]=Convert.ToInt32(original),["source_geometry_changed"]=false}.ToString());
   if(report.summary.result!=BuildResult.Succeeded)throw new Exception("Build failed");
  } finally {settings.Update();settings.FindProperty("VertexChannelCompressionMask").intValue=original;settings.ApplyModifiedPropertiesWithoutUndo();AssetDatabase.SaveAssets();}
 }
}
