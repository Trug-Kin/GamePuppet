using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.Build.Reporting;

[InitializeOnLoad] public static class ConRoiR34 {
 public const string Scene="Assets/Scenes/JourneyRepair_v48_r34.unity",Out="Docs/Reviews/journey_v48/r34";
 static ConRoiR34(){EditorApplication.playModeStateChanged+=p=>{if(p==PlayModeStateChange.EnteredPlayMode&&SessionState.GetBool("r34.walk",false)){SessionState.SetBool("r34.walk",false);UnityEngine.Object.FindAnyObjectByType<DieuPhoiHanhTrinh>().gameObject.AddComponent<R34WalkReview>().output=Out+"/editor-main";}};}
 public static void Begin(){EditorSceneManager.OpenScene(Scene);SessionState.SetBool("r34.walk",true);EditorApplication.EnterPlaymode();}
 public static void EndingFixtures(){
  var rows=new JArray();foreach(var t in new[]{(0,0,"A"),(0,2,"A2"),(3,0,"B"),(3,2,"C"),(2,1,"A"),(2,2,"A2"),(3,1,"B"),(4,2,"C")}){
   string got=DieuPhoiHanhTrinh.ResolveEnding(t.Item1,t.Item2);rows.Add(new JObject{["awareness"]=t.Item1,["interactions"]=t.Item2,["expected"]=t.Item3,["actual"]=got,["pass"]=got==t.Item3});
  }
  File.WriteAllText(Out+"/ending-fixtures.json",rows.ToString());if(rows.Any(r=>!(bool)r["pass"]))throw new Exception("Ending fixture failed");
 }
 public static void Author(){
  if(File.Exists(Scene))throw new Exception("Refuse overwrite checkpoint/derivative");
  if(!AssetDatabase.CopyAsset(ConRoiR33Chair.Scene,Scene))throw new Exception("Scene copy failed");
  EditorSceneManager.OpenScene(Scene);
  var s=UnityEngine.Object.FindAnyObjectByType<DieuPhoiHanhTrinh>();var p=s.GetComponent<TuongTacGiayLop203R32>();var ex=p.staging;
  p.completeR34=true;ex.paperOwnsResponseR34=true;
  var tops=s.schoolChapter.environment.GetComponentsInChildren<Transform>(true).Where(t=>t.name=="Desk_Top"&&!t.IsChildOf(ex.responseDesk)&&!t.IsChildOf(ex.puppetDesk)).ToArray();
  var top=tops.OrderBy(t=>Vector3.Distance(t.position,ex.responseDesk.position)).First();
  var bounds=top.GetComponent<Renderer>().bounds;
  p.papers[1].position=new Vector3(bounds.center.x,bounds.max.y+.003f,bounds.center.z);
  var label=new GameObject("R34_ModelDesk_Label").AddComponent<TextMesh>();label.transform.SetParent(top.parent,true);
  label.transform.position=p.papers[1].position+new Vector3(0,.001f,-.24f);label.transform.rotation=Quaternion.Euler(90,0,0);
  label.text="BÀI MẪU · ĐÚNG DÀN Ý";label.characterSize=.014f;label.fontSize=48;label.anchor=TextAnchor.MiddleCenter;label.color=new Color(.16f,.12f,.08f);
  var old=s.GetComponent<HoSoSceneR33>();UnityEngine.Object.DestroyImmediate(old);s.gameObject.AddComponent<HoSoSceneR34>().story=s;
  EditorBuildSettings.scenes=new[]{new EditorBuildSettingsScene(Scene,true)};
  EditorSceneManager.SaveScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene());AssetDatabase.SaveAssets();
  File.WriteAllText(Out+"/layout.json",new JObject{["scene"]=Scene,["Minh"]=V(p.papers[0].position),["model"]=V(p.papers[1].position),["model_desk"]=top.parent.name,["model_bounds_min"]=V(bounds.min),["model_bounds_max"]=V(bounds.max),["tray"]=V(p.tray.position),["outside"]=V(p.outside.position),["geometry_changed"]=false,["chair_restoration"]=ex.restoreChairTraceR33,["source"]=Path.GetFullPath("Docs/Reviews/journey_v48/r32/school-r32-source.blend"),["source_meshes"]=new JArray(p.papers.Select(t=>t.GetComponent<MeshFilter>().sharedMesh.name))}.ToString());
 }
 static JArray V(Vector3 p)=>new JArray(p.x,p.y,p.z);
 public static void Build(){
  EndingFixtures();
  EditorSceneManager.OpenScene(Scene);EditorBuildSettings.scenes=new[]{new EditorBuildSettingsScene(Scene,true)};
  string exe=Path.GetFullPath("Builds/Windows_v48_r34_integrated/ConRoi.exe");if(File.Exists(exe))throw new Exception("Refuse overwrite binary");
  Directory.CreateDirectory(Path.GetDirectoryName(exe));
  var r=BuildPipeline.BuildPlayer(new[]{Scene},exe,BuildTarget.StandaloneWindows64,BuildOptions.Development|BuildOptions.DetailedBuildReport);
  File.WriteAllText(Out+"/build.json",new JObject{["result"]=r.summary.result.ToString(),["errors"]=r.summary.totalErrors,["warnings"]=r.summary.totalWarnings,["scene"]=Scene,["binary"]=exe,["utc"]=DateTime.UtcNow.ToString("o")}.ToString());
  if(r.summary.result!=BuildResult.Succeeded)throw new Exception("Build failed");
 }
}
