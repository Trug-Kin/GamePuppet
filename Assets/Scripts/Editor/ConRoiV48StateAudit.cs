using System;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.Rendering;
using Newtonsoft.Json.Linq;

public static class ConRoiV48StateAudit
{
 static bool repair;
 static string Out=>"Docs/Reviews/journey_v48/"+(repair?"r10":"r8-isolation");
 public static void BuildRepaired(){repair=true;Build();}
 static string PathOf(Transform t)=>t.parent?PathOf(t.parent)+"/"+t.name:t.name;
 static JObject RendererRow(Renderer r)=>new JObject{["path"]=PathOf(r.transform),["enabled"]=r.enabled,["activeSelf"]=r.gameObject.activeSelf,["activeHierarchy"]=r.gameObject.activeInHierarchy,["position"]=r.transform.position.ToString("F5"),["matrix"]=r.transform.localToWorldMatrix.ToString("F5"),["bounds"]=r.bounds.ToString("F5"),["mesh"]=r.GetComponent<MeshFilter>()?AssetDatabase.GetAssetPath(r.GetComponent<MeshFilter>().sharedMesh):"",["materials"]=new JArray(r.sharedMaterials.Select(m=>m?AssetDatabase.GetAssetPath(m):"null"))};
 public static void Build()
 {
  Directory.CreateDirectory(Out);
  EditorSceneManager.OpenScene("Assets/Scenes/JourneyAcceptance_v47_rc3.unity");
  var s=UnityEngine.Object.FindAnyObjectByType<DieuPhoiHanhTrinh>();
  var groups=new JArray(s.market.GetComponentsInChildren<Transform>(true).Where(t=>t.parent==s.market.transform).Select(t=>new JObject{["path"]=PathOf(t),["models"]=new JArray(t.GetComponentsInChildren<Renderer>(true).Select(RendererRow)),["components"]=new JArray(t.GetComponents<Component>().Select(c=>c?c.GetType().Name:"missing"))}));
  File.WriteAllText(Out+"/rc3-market-exact-inventory.json",groups.ToString());
  var old=UnityEngine.Object.FindObjectsByType<MeshRenderer>(FindObjectsInactive.Include,FindObjectsSortMode.None).Where(r=>r.name.StartsWith("Surface_")||r.name.StartsWith("Context_")).Select(RendererRow).ToArray();
  EditorSceneManager.OpenScene(ConRoiRepairV48.Scene);
  s=UnityEngine.Object.FindAnyObjectByType<DieuPhoiHanhTrinh>();
  var current=UnityEngine.Object.FindObjectsByType<MeshRenderer>(FindObjectsInactive.Include,FindObjectsSortMode.None).Where(r=>r.name.StartsWith("Surface_")||r.name.StartsWith("Context_")).Select(RendererRow).ToDictionary(r=>(string)r["path"]);
  File.WriteAllText(Out+"/architecture-comparison.json",new JArray(old.Select(r=>new JObject{["before"]=r,["after"]=current.TryGetValue((string)r["path"],out var after)?after:null,["identical"]=after!=null&&JToken.DeepEquals(r,after)})).ToString());
  var mats=UnityEngine.Object.FindObjectsByType<Renderer>(FindObjectsInactive.Include,FindObjectsSortMode.None).SelectMany(r=>r.sharedMaterials).Where(m=>m).Distinct();
  File.WriteAllText(Out+"/material-emission-audit.json",new JArray(mats.Select(m=>new JObject{["asset"]=AssetDatabase.GetAssetPath(m),["shader"]=m.shader.name,["emission"]=m.HasProperty("_EmissionColor")?m.GetColor("_EmissionColor").ToString("F5"):"none",["emissionEnabled"]=m.IsKeywordEnabled("_EMISSION"),["surface"]=m.HasProperty("_Surface")?m.GetFloat("_Surface"):0})).ToString());
  File.WriteAllText(Out+"/static-effects.json",new JObject{["lights"]=new JArray(UnityEngine.Object.FindObjectsByType<Light>(FindObjectsInactive.Include,FindObjectsSortMode.None).Select(l=>new JObject{["path"]=PathOf(l.transform),["position"]=l.transform.position.ToString("F3"),["intensity"]=l.intensity,["range"]=l.range,["type"]=l.type.ToString()})),["volumes"]=new JArray(UnityEngine.Object.FindObjectsByType<Volume>(FindObjectsInactive.Include,FindObjectsSortMode.None).Select(v=>new JObject{["path"]=PathOf(v.transform),["profile"]=v.sharedProfile?AssetDatabase.GetAssetPath(v.sharedProfile):"none",["weight"]=v.weight})),["lodGroups"]=UnityEngine.Object.FindObjectsByType<LODGroup>(FindObjectsInactive.Include,FindObjectsSortMode.None).Length}.ToString());
  if(repair)ConRoiV48Normals.Repair(Out);
  s.gameObject.AddComponent<GhiTrangThaiHinhAnhV48>().story=s;
  if(!repair)s.gameObject.AddComponent<CoLapHinhAnhV48>().story=s;
  string scene=repair?"Assets/Scenes/JourneyRepair_v48_r10.unity":"Assets/Scenes/JourneyRepair_v48_r8_isolation.unity";
  EditorSceneManager.SaveScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene(),scene);
  string version=PlayerSettings.bundleVersion;
  try{
   PlayerSettings.bundleVersion=repair?"0.48.10":"0.48.8";
   var b=BuildPipeline.BuildPlayer(new[]{scene},repair?"Builds/Windows_v48_r10/ConRoi.exe":"Builds/Windows_v48_r8_isolation/ConRoi.exe",BuildTarget.StandaloneWindows64,BuildOptions.Development);
   File.WriteAllText(Out+"/build.json",new JObject{["result"]=b.summary.result.ToString(),["errors"]=b.summary.totalErrors,["warnings"]=b.summary.totalWarnings,["scene"]=scene}.ToString());
   if(b.summary.result!=UnityEditor.Build.Reporting.BuildResult.Succeeded)throw new Exception("Build failed");
  }finally{PlayerSettings.bundleVersion=version;AssetDatabase.SaveAssets();}
 }
}
