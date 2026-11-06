using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.Build.Reporting;
[InitializeOnLoad] public static class ConRoiR42
{
 static ConRoiR42(){EditorApplication.playModeStateChanged+=Mode;}
 static void Mode(PlayModeStateChange p){if(p==PlayModeStateChange.EnteredPlayMode&&SessionState.GetBool("r42.run",false)){SessionState.SetBool("r42.run",false);new GameObject("R42_OptInReview").AddComponent<R42JourneyReview>().output=Out+"/editor-full";}}
 public static void Begin(){EditorSceneManager.OpenScene(Scene);Validate();SessionState.SetBool("r42.run",true);EditorApplication.EnterPlaymode();}
 public const string Scene="Assets/Scenes/JourneyRepair_v48_r42.unity",Out="Docs/Reviews/journey_v48/r42";
 public static void Author()
 {
  Directory.CreateDirectory(Out);
  if(File.Exists(Scene))throw new Exception("Preserve derivative");
  AssetDatabase.CopyAsset(ConRoiR41.Scene,Scene);EditorSceneManager.OpenScene(Scene);
  var s=UnityEngine.Object.FindAnyObjectByType<DieuPhoiHanhTrinh>();s.gameObject.AddComponent<DieuPhoiGiaoDienR42>().story=s;
  EditorSceneManager.SaveScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene());AssetDatabase.SaveAssets();
  EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);EditorSceneManager.OpenScene(Scene);Validate();
  EditorBuildSettings.scenes=new[]{new EditorBuildSettingsScene(Scene,true)};AssetDatabase.SaveAssets();EditorApplication.Exit(0);
 }
 public static void Validate()
 {
  var sc=UnityEngine.SceneManagement.SceneManager.GetActiveScene();var all=sc.GetRootGameObjects().SelectMany(g=>g.GetComponentsInChildren<Transform>(true)).ToArray();
  var missing=all.Where(t=>GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(t.gameObject)>0).Select(t=>t.name).ToArray();
  var refs=new JArray();foreach(var b in all.SelectMany(t=>t.GetComponents<MonoBehaviour>()).Where(b=>b)){
   var so=new SerializedObject(b);var p=so.GetIterator();while(p.NextVisible(true))if(p.propertyType==SerializedPropertyType.ObjectReference&&p.objectReferenceValue==null&&!p.objectReferenceEntityIdValue.Equals(default(UnityEngine.EntityId)))refs.Add(b.GetType().Name+"."+p.propertyPath);
  }
  var s=UnityEngine.Object.FindAnyObjectByType<DieuPhoiHanhTrinh>();
  var required=new JArray();Action<string,UnityEngine.Object> need=(name,value)=>required.Add(new JObject{["field"]=name,["valid"]=value!=null,["target"]=value?value.name:"NULL"});
  need("story.player",s.player);need("story.hub",s.hub);need("story.lake",s.lake);need("story.market",s.market);need("story.polish",s.polish);need("story.school",s.schoolChapter);need("story.slope",s.slopeChapter);need("interaction.eye",s.player.GetComponent<TuongTacNguoiChoi>().playerCamera);need("polish.eye",s.polish.eye);
  foreach(var type in new[]{typeof(ChuanBiHanhLyR41),typeof(TuongTacSoTayHoR37),typeof(QuanLyGoiHangChoR35)}){var activity=s.GetComponent(type);foreach(var field in type.GetFields().Where(f=>typeof(UnityEngine.Object).IsAssignableFrom(f.FieldType)))need(type.Name+"."+field.Name,field.GetValue(activity) as UnityEngine.Object);}
  var papers=s.GetComponent<TuongTacGiayLop203R32>();need("papers.story",papers.story);need("papers.staging",papers.staging);need("papers.tray",papers.tray);need("papers.outside",papers.outside);foreach(var p in papers.papers)need("paper",p);foreach(var p in papers.pages)need("page texture",p);
  var prefabs=new JArray();foreach(var path in all.Where(t=>PrefabUtility.IsAnyPrefabInstanceRoot(t.gameObject)).Select(t=>PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(t.gameObject)).Distinct()){
   for(int i=0;i<2;i++){bool editable=path.EndsWith(".prefab");var root=editable?PrefabUtility.LoadPrefabContents(path):AssetDatabase.LoadAssetAtPath<GameObject>(path);int count=root.GetComponentsInChildren<Transform>(true).Sum(t=>GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(t.gameObject));prefabs.Add(new JObject{["path"]=path,["open"]=i+1,["kind"]=editable?"editable prefab contents":"imported model prefab; source asset read only",["missingScripts"]=count,["modified"]=false});if(editable)PrefabUtility.UnloadPrefabContents(root);if(count>0)throw new Exception("Prefab missing script");}
  }
  var report=new JObject{["scene"]=sc.path,["savedClosedReopened"]=true,["missingScripts"]=new JArray(missing),["brokenReferences"]=refs,["players"]=all.Count(t=>t.GetComponent<DieuKhienDiChuyen>()),["listeners"]=all.Count(t=>t.GetComponent<AudioListener>()),["eventSystems"]=all.Count(t=>t.GetComponent<UnityEngine.EventSystems.EventSystem>()),["inputModules"]=all.Count(t=>t.GetComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>()),["uiStoryReference"]=s.GetComponent<DieuPhoiGiaoDienR42>().story==s,["prefabInstances"]=all.Count(t=>PrefabUtility.IsAnyPrefabInstanceRoot(t.gameObject)),["serializedReview"]=all.Any(t=>t.GetComponent<R42JourneyReview>()),["audioClips"]=new JArray(s.ambience.Select(c=>c?AssetDatabase.GetAssetPath(c):"NULL"))};
  report["requiredReferences"]=required;report["prefabsLoadedClosedReopened"]=prefabs;report["buildScenes"]=new JArray(EditorBuildSettings.scenes.Select(x=>x.path));
  File.WriteAllText(Out+"/source-reopened.json",report.ToString());
  if(required.Any(x=>!(bool)x["valid"]))throw new Exception("Missing required reference");
  if(missing.Length>0||refs.Count>0||(bool)report["serializedReview"]||!(bool)report["uiStoryReference"])throw new Exception("R42 references");
 }
 public static void Build(){BuildTo("Builds/Windows_v48_r42_integrated/ConRoi.exe",false);}
 public static void BuildFinal(){BuildTo("Builds/Windows_v48_r42_clean/ConRoi.exe",true);}
 static void BuildTo(string path,bool clean){EditorSceneManager.OpenScene(Scene);Validate();EditorBuildSettings.scenes=new[]{new EditorBuildSettingsScene(Scene,true)};if(clean&&Directory.Exists(Path.GetDirectoryName(path)))throw new Exception("Preserve existing clean build");Directory.CreateDirectory(Path.GetDirectoryName(path));var options=BuildOptions.Development|BuildOptions.DetailedBuildReport;if(clean)options|=BuildOptions.CleanBuildCache;var r=BuildPipeline.BuildPlayer(new[]{Scene},path,BuildTarget.StandaloneWindows64,options);File.WriteAllText(Out+"/build.json",new JObject{["scene"]=Scene,["output"]=path,["cleanBuildCache"]=clean,["result"]=r.summary.result.ToString(),["errors"]=r.summary.totalErrors,["warnings"]=r.summary.totalWarnings}.ToString());EditorApplication.Exit(r.summary.result==BuildResult.Succeeded?0:1);}
}
