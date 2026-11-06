using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.Build.Reporting;
using UnityEngine;

public static class ConRoiV48R14
{
 const string Out="Docs/Reviews/journey_v48/r14";
 public static void Build()
 {
  Directory.CreateDirectory(Out);
  EditorSceneManager.OpenScene("Assets/Scenes/JourneyRepair_v48_r12.unity");
  Physics.SyncTransforms();
  var rows=new JArray();
  foreach(var c in UnityEngine.Object.FindObjectsByType<Collider>(FindObjectsInactive.Include,FindObjectsSortMode.None))
  {
   if(c.transform.root.name!="V48_Closed_Outer_Streets")continue;
   rows.Add(new JObject{["name"]=c.name,["active"]=c.gameObject.activeInHierarchy,["min"]=V(c.bounds.min),["max"]=V(c.bounds.max)});
  }
  File.WriteAllText(Out+"/boundary-colliders.json",rows.ToString());
  var sections=new JArray();
  var nw=new Vector3(685.9787f,1.14f,496.6172f);var se=new Vector3(642.2553f,1.14f,533.6046f);var axis=(se-nw).normalized;var side=Vector3.Cross(Vector3.up,axis);
  for(float along=-18;along<=76;along+=2)for(float lateral=-10;lateral<=10;lateral+=2)
  {
   var p=nw+axis*along+side*lateral;
   var hits=Physics.RaycastAll(p+Vector3.up*40,Vector3.down,80,8,QueryTriggerInteraction.Ignore).Where(h=>h.normal.y>.6f).OrderByDescending(h=>h.point.y);
   sections.Add(new JObject{["along"]=along,["lateral"]=lateral,["x"]=p.x,["z"]=p.z,["hits"]=new JArray(hits.Select(h=>new JObject{["name"]=h.collider.name,["y"]=h.point.y,["root"]=h.collider.transform.root.name}))});
  }
  File.WriteAllText(Out+"/bridge-cross-sections.json",sections.ToString());
  const string scene="Assets/Scenes/JourneyRepair_v48_r14.unity";
  EditorSceneManager.SaveScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene(),scene);
  var version=PlayerSettings.bundleVersion;
  try
  {
   PlayerSettings.bundleVersion="0.48.14";
   var report=BuildPipeline.BuildPlayer(new[]{scene},"Builds/Windows_v48_r14/ConRoi.exe",BuildTarget.StandaloneWindows64,BuildOptions.Development);
   File.WriteAllText(Out+"/build.json",new JObject{["result"]=report.summary.result.ToString(),["errors"]=report.summary.totalErrors,["warnings"]=report.summary.totalWarnings}.ToString());
   File.WriteAllText(Out+"/build-assets.json",new JObject{["assets"]=new JArray(report.packedAssets.SelectMany(p=>p.contents).Select(c=>c.sourceAssetPath).Distinct().OrderBy(p=>p))}.ToString());
   if(report.summary.result!=BuildResult.Succeeded)throw new Exception("r14 build failed");
  }
  finally{PlayerSettings.bundleVersion=version;AssetDatabase.SaveAssets();}
 }
 static JArray V(Vector3 v)=>new JArray(v.x,v.y,v.z);
}
