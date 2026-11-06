using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.Build.Reporting;
using UnityEngine;

public static class ConRoiR30
{
 public const string Scene="Assets/Scenes/JourneyRepair_v48_r30.unity",Out="Docs/Reviews/journey_v48/r30";
 public static void Author(){
  if(File.Exists(Scene))throw new Exception("r30 exists; checkpoint copy refused");
  if(!AssetDatabase.CopyAsset(ConRoiR29School.Scene,Scene))throw new Exception("copy failed");
  EditorSceneManager.OpenScene(Scene);var s=UnityEngine.Object.FindAnyObjectByType<DieuPhoiHanhTrinh>();
  s.GetComponent<TraiNghiemTruongR28>().stagedSchoolR30=true;
  s.gameObject.AddComponent<HoSoSceneR30>().story=s;
  var root=s.schoolChapter.environment.transform.Find("R28_School");
  foreach(var t in root.GetComponentsInChildren<TextMesh>(true)){
   if(t.name=="203_BoardWriting")t.text="203\nBÀI LÀM ĐÚNG KHUÔN\nMỗi bài đều giống nhau.";
  }
  EditorSceneManager.SaveScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
  EditorBuildSettings.scenes=new[]{new EditorBuildSettingsScene(Scene,true)};AssetDatabase.SaveAssets();Dump();
 }
 public static void Dump(){
  EditorSceneManager.OpenScene(Scene);var s=UnityEngine.Object.FindAnyObjectByType<DieuPhoiHanhTrinh>();
  var items=UnityEngine.Object.FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include).Where(x=>x is IPlayerInteractable).Select(x=>new JObject{["name"]=x.name,["type"]=x.GetType().Name,["kind"]=x is TuongTacHoCho l?l.kind.ToString():"",["position"]=V(x.transform.position),["collider_center"]=x.GetComponent<Collider>()?V(x.GetComponent<Collider>().bounds.center):null});
  var actors=s.puppets.Select(p=>new JObject{["name"]=p.name,["position"]=V(p.transform.position),["head"]=V(p.dauTransform.position)});
  File.WriteAllText(Out+"/route-targets.json",new JObject{["hub"]=V(s.hubSpawn.position),["lake"]=V(s.lakeSpawn.position),["market"]=V(s.marketSpawn.position),["school"]=V(s.schoolChapter.spawn.position),["slope"]=V(s.slopeChapter.spawn.position),["actors"]=new JArray(actors),["targets"]=new JArray(items)}.ToString());
  File.WriteAllText(Out+"/build-input.json",new JObject{["scene"]=Path.GetFullPath(Scene),["guid"]=AssetDatabase.AssetPathToGUID(Scene),["enabled_scenes"]=new JArray(EditorBuildSettings.scenes.Where(x=>x.enabled).Select(x=>x.path)),["load_flow"]="Single integrated scene; normal Sleep/DreamRoom/Wake/ApplyRegion roots. No scene redirect.",["geometry"]="No mesh, collider, placement, lights or material changes. Blender source and REVIEW geometry remain exactly r29. TextMesh board text, staging opt-in/profile and r30-only renderer depth snapshot fix changed.",["renderer"]=AssetDatabase.GetAssetPath(s.GetComponent<HoSoSceneR26>().pipeline)}.ToString());
 }
 public static JArray V(Vector3 p)=>new JArray(p.x,p.y,p.z);
 public static void FixOverlay(){
  const string art="Assets/Art/Environment/JourneyV48/IntegrationR30";
  Directory.CreateDirectory(art);AssetDatabase.Refresh();
  string renderer=art+"/Renderer.asset",pipeline=art+"/Pipeline.asset";
  if(!File.Exists(renderer))AssetDatabase.CopyAsset("Assets/Art/Environment/JourneyV48/SceneryR22/Renderer.asset",renderer);
  if(!File.Exists(pipeline))AssetDatabase.CopyAsset("Assets/Art/Environment/JourneyV48/SceneryR22/Pipeline.asset",pipeline);
  AssetDatabase.Refresh();
  foreach(var feature in AssetDatabase.LoadAllAssetsAtPath(renderer).OfType<TinhNangSuongKhongGianR22>()){feature.isolateDepthForOverlayR30=true;EditorUtility.SetDirty(feature);}
  var pipe=AssetDatabase.LoadAssetAtPath<UnityEngine.Rendering.Universal.UniversalRenderPipelineAsset>(pipeline);
  var serialized=new SerializedObject(pipe);serialized.FindProperty("m_RendererDataList").GetArrayElementAtIndex(0).objectReferenceValue=AssetDatabase.LoadAssetAtPath<UnityEngine.Rendering.Universal.UniversalRendererData>(renderer);serialized.ApplyModifiedPropertiesWithoutUndo();
  EditorSceneManager.OpenScene(Scene);UnityEngine.Object.FindAnyObjectByType<HoSoSceneR26>().pipeline=pipe;
  EditorSceneManager.SaveScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene());AssetDatabase.SaveAssets();Dump();
 }
 public static void Build(){
  EditorSceneManager.OpenScene(Scene);
  if(EditorBuildSettings.scenes.Count(x=>x.enabled)!=1||EditorBuildSettings.scenes.Single(x=>x.enabled).path!=Scene)throw new Exception("Build Settings mismatch");
  string exe=Path.Combine(Path.GetTempPath(),"ConRoi-r30-Windows-clean","ConRoi.exe");Directory.CreateDirectory(Path.GetDirectoryName(exe));
  var report=BuildPipeline.BuildPlayer(new[]{Scene},exe,BuildTarget.StandaloneWindows64,BuildOptions.Development|BuildOptions.DetailedBuildReport|BuildOptions.CleanBuildCache);
  File.WriteAllText(Out+"/build.json",new JObject{["result"]=report.summary.result.ToString(),["errors"]=report.summary.totalErrors,["warnings"]=report.summary.totalWarnings,["scene"]=Scene,["binary"]=exe,["bytes"]=report.summary.totalSize,["utc"]=DateTime.UtcNow.ToString("o")}.ToString());
  if(report.summary.result!=BuildResult.Succeeded)throw new Exception("Build failed");
 }
}
