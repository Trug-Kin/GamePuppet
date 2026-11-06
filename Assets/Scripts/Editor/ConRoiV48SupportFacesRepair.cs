using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.Build.Reporting;

public static class ConRoiV48SupportFacesRepair
{
 public static void BuildFinal()
 {
  const string output="Docs/Reviews/journey_v48/r13";Directory.CreateDirectory(output);
  var version=PlayerSettings.bundleVersion;
  try
  {
   PlayerSettings.bundleVersion="0.48.13";
   var report=BuildPipeline.BuildPlayer(new[]{"Assets/Scenes/JourneyRepair_v48_r12.unity"},"Builds/Windows_v48_r13/ConRoi.exe",BuildTarget.StandaloneWindows64,BuildOptions.Development);
   File.WriteAllText(output+"/build.json",new JObject{["result"]=report.summary.result.ToString(),["errors"]=report.summary.totalErrors,["warnings"]=report.summary.totalWarnings}.ToString());
   File.WriteAllText(output+"/build-assets.json",new JObject{["output"]=report.summary.outputPath,["assets"]=new JArray(report.packedAssets.SelectMany(p=>p.contents).Select(c=>c.sourceAssetPath).Distinct().OrderBy(p=>p))}.ToString());
   if(report.summary.result!=BuildResult.Succeeded)throw new Exception("Final build failed");
  }
  finally{PlayerSettings.bundleVersion=version;AssetDatabase.SaveAssets();}
 }
 const string Out="Docs/Reviews/journey_v48/r12";
 public static void Build()
 {
  Directory.CreateDirectory(Out);
  const string asset="Assets/Art/Environment/JourneyV48/BridgeRepairR12/BridgeLocalSupport.fbx";
  AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
  var importer=(ModelImporter)AssetImporter.GetAtPath(asset);importer.isReadable=true;importer.addCollider=false;importer.materialImportMode=ModelImporterMaterialImportMode.None;importer.SaveAndReimport();
  EditorSceneManager.OpenScene(ConRoiV48AcceptanceRepair.Scene);
  var meshes=AssetDatabase.LoadAllAssetsAtPath(asset).OfType<Mesh>().ToDictionary(m=>m.name);
  var root=GameObject.Find("R11_Local_Bridge_Supports");var rows=new JArray();
  foreach(var f in root.GetComponentsInChildren<MeshFilter>(true))
  {
   var before=f.sharedMesh;var next=meshes[before.name];
   if(Vector3.Distance(before.bounds.center,next.bounds.center)>.001f||Vector3.Distance(before.bounds.size,next.bounds.size)>.001f)throw new Exception("Unexpected support geometry bounds change "+f.name);
   f.sharedMesh=next;var collider=f.GetComponent<MeshCollider>();if(collider)collider.sharedMesh=next;
   rows.Add(new JObject{["name"]=f.name,["before"]=AssetDatabase.GetAssetPath(before),["after"]=asset,["bounds_retained"]=true,["collider_matches"]=!collider||collider.sharedMesh==next});
  }
  File.WriteAllText(Out+"/support-mesh-swap.json",rows.ToString());
  const string scene="Assets/Scenes/JourneyRepair_v48_r12.unity";
  EditorSceneManager.SaveScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene(),scene);AssetDatabase.SaveAssets();
  var version=PlayerSettings.bundleVersion;
  try
  {
   PlayerSettings.bundleVersion="0.48.12";
   var report=BuildPipeline.BuildPlayer(new[]{scene},"Builds/Windows_v48_r12/ConRoi.exe",BuildTarget.StandaloneWindows64,BuildOptions.Development);
   File.WriteAllText(Out+"/build.json",new JObject{["result"]=report.summary.result.ToString(),["errors"]=report.summary.totalErrors,["warnings"]=report.summary.totalWarnings}.ToString());
   File.WriteAllText(Out+"/build-assets.json",new JObject{["output"]=report.summary.outputPath,["assets"]=new JArray(report.packedAssets.SelectMany(p=>p.contents).Select(c=>c.sourceAssetPath).Distinct().OrderBy(p=>p))}.ToString());
   if(report.summary.result!=BuildResult.Succeeded)throw new Exception("R12 build failed");
  }
  finally{PlayerSettings.bundleVersion=version;AssetDatabase.SaveAssets();}
 }
}
