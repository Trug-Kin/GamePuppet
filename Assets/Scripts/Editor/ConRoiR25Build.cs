using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine;
public static class ConRoiR25Build
{
 public static void Build(){
  const string output=ConRoiR25Slope.Out,binary="Builds/Windows_v48_r25_playtest/ConRoi.exe";
  var review=JObject.Parse(File.ReadAllText(output+"/playmode.json"));if(!(bool)review["complete"]||review["errors"].Any())throw new Exception("Passing review required");
  if(File.Exists("Builds/Windows_v48_r25_playtest.zip"))throw new Exception("Retained handoff immutable");
  if(!(bool)JObject.Parse(File.ReadAllText(output+"/geometry-validation.json"))["pass"])throw new Exception("Passing geometry required");
  EditorSceneManager.OpenScene(ConRoiR25Slope.Scene);
  File.WriteAllText(output+"/scene-dependencies.json",new JArray(AssetDatabase.GetDependencies(ConRoiR25Slope.Scene,true)).ToString());
  var report=BuildPipeline.BuildPlayer(new[]{ConRoiR25Slope.Scene},binary,BuildTarget.StandaloneWindows64,BuildOptions.Development|BuildOptions.DetailedBuildReport);
  var packed=report.packedAssets.SelectMany(p=>p.contents).Select(p=>p.sourceAssetPath).Where(p=>!string.IsNullOrEmpty(p)).Distinct().OrderBy(p=>p).ToArray();
  var required=new[]{ConRoiR25Slope.Art+"/R25_Concrete_True.asset",ConRoiR25Slope.Art+"/R25_Earth_True.asset",ConRoiR25Slope.Art+"/R25_TealPaint_True.asset",ConRoiR25Slope.Art+"/Textures/TealPaint.png",ConRoiR25Slope.Art+"/Textures/Concrete.png","Assets/Art/Environment/JourneyV48/ReferenceR24/school_Peach_Plaster_True.asset","Assets/Art/Characters/R23/WoodenStep.json","Assets/Art/UI/R23/ReadableUI.shader","Assets/Art/Environment/JourneyV48/SceneryR22/WorldMist.shader"};
  var evidence=new JObject();foreach(var n in required)evidence[n]=packed.Contains(n);bool present=evidence.Properties().All(p=>(bool)p.Value);
  var messages=new JArray();foreach(var step in report.steps)foreach(var m in step.messages)if(m.type==LogType.Warning||m.type==LogType.Error||m.type==LogType.Exception)messages.Add(new JObject{["step"]=step.name,["type"]=m.type.ToString(),["message"]=m.content});
  File.WriteAllText(output+"/build-packed-assets.json",new JArray(packed).ToString());File.WriteAllText(output+"/build.json",new JObject{["result"]=report.summary.result.ToString(),["errors"]=report.summary.totalErrors,["warnings"]=report.summary.totalWarnings,["messages"]=messages,["scene"]=ConRoiR25Slope.Scene,["binary"]=binary,["required_assets_present"]=present,["required_asset_evidence"]=evidence,["native_input"]="NOT_RUN; existing diagnosis PAUSED retained",["method"]="Windows64 build + packed-asset audit; Editor Play Mode controller/visual checks. Launcher not opened."}.ToString());
  if(report.summary.result!=BuildResult.Succeeded||!present)throw new Exception("Build/asset audit failed");
 }
}
