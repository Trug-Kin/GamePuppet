using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class ConRoiR23Build
{
 public static void Build(){
  const string output=ConRoiR23Opening.Out,binary="Builds/Windows_v48_r23_playtest/ConRoi.exe";
  var review=JObject.Parse(File.ReadAllText(output+"/playmode.json"));if(!(bool)review["complete"]||review["errors"].Any())throw new Exception("Complete Play Mode review required");
  if(File.Exists("Builds/Windows_v48_r23_playtest.zip"))throw new Exception("Retained handoff is immutable");
  EditorSceneManager.OpenScene(ConRoiR23Opening.Scene);
  var deps=AssetDatabase.GetDependencies(ConRoiR23Opening.Scene,true);File.WriteAllText(output+"/scene-dependencies.json",new JArray(deps).ToString());
  var report=BuildPipeline.BuildPlayer(new[]{ConRoiR23Opening.Scene},binary,BuildTarget.StandaloneWindows64,BuildOptions.Development|BuildOptions.DetailedBuildReport);
  var packed=report.packedAssets.SelectMany(p=>p.contents).Select(p=>p.sourceAssetPath).Where(p=>!string.IsNullOrEmpty(p)).Distinct().OrderBy(p=>p).ToArray();
  var required=new[]{"Assets/Art/Characters/R23/WoodenStep.json","Assets/Art/UI/R23/ReadableUI.shader","Assets/Art/UI/R23/ReadableUI.mat","Assets/Art/Environment/JourneyV48/SceneryR22/Flowers_Red_0.asset","Assets/Art/Environment/JourneyV48/SceneryR22/Flowers_Yellow_2.asset","Assets/Art/Environment/JourneyV48/SceneryR22/MarketAwning.fbx","Assets/Art/Environment/JourneyV48/SceneryR22/WorldMist.shader","Assets/Art/Environment/JourneyV48/SceneryR22/Pipeline.asset","Assets/Art/Environment/JourneyV48/SceneryR22/IslandDeckCollider.asset"};
  var evidence=new JObject();foreach(var n in required)evidence[n]=packed.Contains(n);bool present=evidence.Properties().All(p=>(bool)p.Value);
  var messages=new JArray();foreach(var step in report.steps)foreach(var m in step.messages)if(m.type==LogType.Warning||m.type==LogType.Error||m.type==LogType.Exception)messages.Add(new JObject{["step"]=step.name,["type"]=m.type.ToString(),["message"]=m.content});
  File.WriteAllText(output+"/build-packed-assets.json",new JArray(packed).ToString());File.WriteAllText(output+"/build.json",new JObject{["result"]=report.summary.result.ToString(),["errors"]=report.summary.totalErrors,["warnings"]=report.summary.totalWarnings,["messages"]=messages,["scene"]=ConRoiR23Opening.Scene,["binary"]=binary,["required_assets_present"]=present,["required_asset_evidence"]=evidence,["native_input"]="NOT_RUN; diagnosis PAUSED; existing INTERRUPTED/NOT_RUN unchanged",["method"]="Windows64 Development Build and packed-asset audit; Editor Play Mode functional/visual checks. No Windows-input or executable loading-gate certification."}.ToString());
  if(report.summary.result!=BuildResult.Succeeded||!present)throw new Exception("Build/asset audit failed");
 }
}
