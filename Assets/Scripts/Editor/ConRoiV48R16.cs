using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.Build.Reporting;

public static class ConRoiV48R16
{
 public static void Build()
 {BuildRevision(16);}
 public static void BuildR17()
 {BuildRevision(17);}
 static void BuildRevision(int revision)
 {
  string output="Docs/Reviews/journey_v48/r"+revision;Directory.CreateDirectory(output);
  var distance=typeof(BaoContactProbeV48).GetMethod("SegmentTriangle",BindingFlags.NonPublic|BindingFlags.Static);
  var p=Vector3.zero;var q=Vector3.right;var r=Vector3.up;
  float D(Vector3 a,Vector3 b)=>(float)distance.Invoke(null,new object[]{a,b,p,q,r});
  if(Mathf.Abs(D(new Vector3(.2f,.2f,1),new Vector3(.2f,.2f,-1)))>1e-6f||Mathf.Abs(D(new Vector3(.2f,.2f,1),new Vector3(.2f,.2f,2))-1)>1e-6f||Mathf.Abs(D(new Vector3(2,0,0),new Vector3(3,0,0))-1)>1e-6f||Mathf.Abs(D(new Vector3(-1,.2f,1),new Vector3(2,.2f,1))-1)>1e-6f)throw new Exception("Contact diagnostic geometry failed known intersection/face/edge/parallel cases");
  File.WriteAllText(output+"/contact-probe-check.json",new JObject{["intersection_face_edge_parallel_cases"]=true,["distance_units"]="squared metres"}.ToString());
  EditorSceneManager.OpenScene("Assets/Scenes/JourneyRepair_v48_r15.unity");
  string scene="Assets/Scenes/JourneyRepair_v48_r"+revision+".unity";EditorSceneManager.SaveScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene(),scene);
  var version=PlayerSettings.bundleVersion;
  try
  {
   PlayerSettings.bundleVersion="0.48."+revision;
   var report=BuildPipeline.BuildPlayer(new[]{scene},"Builds/Windows_v48_r"+revision+"/ConRoi.exe",BuildTarget.StandaloneWindows64,BuildOptions.Development);
   File.WriteAllText(output+"/build.json",new JObject{["result"]=report.summary.result.ToString(),["errors"]=report.summary.totalErrors,["warnings"]=report.summary.totalWarnings}.ToString());
   File.WriteAllText(output+"/build-assets.json",new JObject{["assets"]=new JArray(report.packedAssets.SelectMany(packed=>packed.contents).Select(c=>c.sourceAssetPath).Distinct().OrderBy(path=>path))}.ToString());
   if(report.summary.result!=BuildResult.Succeeded)throw new Exception("r16 build failed");
  }
  finally{PlayerSettings.bundleVersion=version;AssetDatabase.SaveAssets();}
 }
}
