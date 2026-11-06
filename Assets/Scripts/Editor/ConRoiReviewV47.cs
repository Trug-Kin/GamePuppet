using System;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using Newtonsoft.Json.Linq;

public static class ConRoiReviewV47
{
 public const string Scene="Assets/Scenes/JourneyAcceptance_v47.unity",Report="Docs/Reviews/journey_v47";
 public static void AssembleDiagnostic()
 {
  Directory.CreateDirectory(Report);EditorSceneManager.OpenScene(ConRoiReviewV46.Scene);
  var story=UnityEngine.Object.FindAnyObjectByType<DieuPhoiHanhTrinh>();story.gameObject.AddComponent<DoChiPhiTacVuV47>();
  story.player.GetComponentInChildren<Camera>().gameObject.AddComponent<DoHieuNangRenderV47>();
  EditorSceneManager.SaveScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene(),Scene);AssetDatabase.SaveAssets();
  BuildTo("Windows_v47_diagnostic","build-diagnostic.json");
 }
 public static void BuildTrace(){BuildTo("Windows_v47_trace2","build-trace2.json");}
 public static void BuildTraceAndCandidate(){BuildTrace();Build();}
 public static void Build()
 {
  EditorSceneManager.OpenScene(Scene);var story=UnityEngine.Object.FindAnyObjectByType<DieuPhoiHanhTrinh>();
  // The inherited scene omitted this existing GDD cue, so PlayEffect was a no-op.
  story.notification=AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/ConRoiGame/Audio/Thông báo.wav");
  if(!story.notification)throw new Exception("Missing notification audio asset");
  if(!story.GetComponent<PhongTrongMoV47>())
  {
   var room=story.gameObject.AddComponent<PhongTrongMoV47>();room.story=story;
   var desk=story.hub.GetComponentsInChildren<Transform>(true).Single(t=>t.name=="Desk_Puppet_Broken_Strings");
   room.brokenStrings=desk.GetComponentsInChildren<Renderer>(true).Where(r=>r.name.StartsWith("ConRoi_String_")).ToArray();
   if(room.brokenStrings.Length!=5)throw new Exception("Expected five original desk strings");
   room.repairedStrings=room.brokenStrings.Select(r=>{
    var g=UnityEngine.Object.Instantiate(r.gameObject,r.transform.parent);g.name=r.name+"_Dream_Repaired";
    g.transform.localPosition=r.transform.localPosition;g.transform.localRotation=r.transform.localRotation*Quaternion.Euler(0,0,-55);
    var scale=r.transform.localScale;scale.y/=.14f;g.transform.localScale=scale;
    var renderer=g.GetComponent<Renderer>();renderer.enabled=false;return renderer;
   }).ToArray();
   story.presentation.strings=story.presentation.strings.Concat(room.repairedStrings).ToArray();
  }
  // The transition render experiment did not improve the matched workload.
  // Remove its serialized component before shipping the acceptance candidate.
  var rejectedGate=story.GetComponent("RegionRenderGateV47");if(rejectedGate)UnityEngine.Object.DestroyImmediate(rejectedGate);
  EditorSceneManager.SaveScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene(),Scene);AssetDatabase.SaveAssets();
  BuildTo("Windows_v47_rc2","build.json");
 }
 static void BuildTo(string directory,string report)
 {
  Directory.CreateDirectory(Report);EditorSceneManager.OpenScene(Scene);EditorBuildSettings.scenes=new[]{new EditorBuildSettingsScene(Scene,true)};PlayerSettings.bundleVersion="0.47.2";
  var r=BuildPipeline.BuildPlayer(new[]{Scene},"Builds/"+directory+"/ConRoi.exe",BuildTarget.StandaloneWindows64,BuildOptions.Development);
  File.WriteAllText(Report+"/"+report,new JObject{["result"]=r.summary.result.ToString(),["errors"]=r.summary.totalErrors,["warnings"]=r.summary.totalWarnings,["path"]=r.summary.outputPath,["messages"]=new JArray(r.steps.SelectMany(s=>s.messages).Where(m=>m.type==LogType.Error||m.type==LogType.Warning).Select(m=>m.content))}.ToString());
  if(r.summary.totalErrors>0)throw new Exception("Build failed");
 }
}
