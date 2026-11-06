using System;using System.IO;using System.Linq;using Newtonsoft.Json.Linq;using UnityEngine;using UnityEditor;using UnityEditor.SceneManagement;
[InitializeOnLoad] public static class ConRoiR40 {
 public const string Scene="Assets/Scenes/JourneyRepair_v48_r40.unity",Out="Docs/Reviews/journey_v48/r40";
 static ConRoiR40(){EditorApplication.playModeStateChanged+=Mode;}
 static string PathOf(Transform t)=>t.parent?PathOf(t.parent)+"/"+t.name:t.name;
 static JArray V(Vector3 v)=>new JArray(v.x,v.y,v.z);
 static void Mode(PlayModeStateChange p){if(p==PlayModeStateChange.EnteredPlayMode&&SessionState.GetBool("r40.run",false)){SessionState.SetBool("r40.run",false);if(SessionState.GetBool("r40.edge",false)){SessionState.SetBool("r40.edge",false);new GameObject("R40_Edge_OptIn").AddComponent<R38LakeReview>().output=Out+"/editor-edge";}else new GameObject("R40_OptInAudit").AddComponent<R40LakeAudit>();}}
 public static void Author(){
  Directory.CreateDirectory(Out);if(File.Exists(Scene))throw new Exception("Preserve derivative");
  // Restore the exact r38 serialized environment AND actor/prop/collider placements;
  // then apply the independent verified reflection sampling fix. No mesh rebuilding.
  AssetDatabase.CopyAsset(ConRoiR38.Scene,Scene);EditorSceneManager.OpenScene(Scene);
  var s=UnityEngine.Object.FindAnyObjectByType<DieuPhoiHanhTrinh>();s.GetComponent<PhanChieuMatHo>().resolvedRequestUvR39=true;s.GetComponent<TuTheBaoBenCauV48>().restoreSourceLayoutR40=true;
  EditorSceneManager.SaveScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene());AssetDatabase.SaveAssets();EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);EditorSceneManager.OpenScene(Scene);Validate();
  // Working scene only. No new player build or acceptance candidate is produced.
  EditorBuildSettings.scenes=new[]{new EditorBuildSettingsScene(Scene,true)};
  SessionState.SetBool("r40.run",true);EditorApplication.EnterPlaymode();
 }
 public static void Begin(){EditorSceneManager.OpenScene(Scene);Validate();SessionState.SetBool("r40.run",true);EditorApplication.EnterPlaymode();}
 public static void Edge(){ExportSceneDelta();Validate();SessionState.SetBool("r40.edge",true);SessionState.SetBool("r40.run",true);EditorApplication.EnterPlaymode();}
 public static void Validate(){
  var sc=UnityEngine.SceneManagement.SceneManager.GetActiveScene();var all=sc.GetRootGameObjects().SelectMany(g=>g.GetComponentsInChildren<Transform>(true)).ToArray();var s=UnityEngine.Object.FindAnyObjectByType<DieuPhoiHanhTrinh>();var n=s.GetComponent<TuongTacSoTayHoR37>();var r=s.GetComponent<PhanChieuMatHo>();
  var missing=all.Where(t=>GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(t.gameObject)>0).Select(PathOf).ToArray();var deps=AssetDatabase.GetDependencies(sc.path,true);bool retired=deps.Any(p=>p.Contains("LakeR39/"));
  var j=new JObject{["scene"]=sc.path,["missingScripts"]=new JArray(missing),["retiredAssetDependency"]=retired,["originalBridge"]=all.Any(t=>t.name=="OngDao_V35_Photo_Batch"),["unsupportedRoot"]=all.Any(t=>t.name.Contains("Protected_Reflection_Overlook")),["players"]=all.Count(t=>t.GetComponent<DieuKhienDiChuyen>()),["listeners"]=all.Count(t=>t.GetComponent<AudioListener>()),["references"]=n.book&&n.socket&&n.gesture&&n.leaf&&n.leftPage&&r.playerCamera&&r.water&&r.notebook==n,["resolvedY"]=r.resolvedRequestUvR39,["migration"]=s.GetComponent<TuTheBaoBenCauV48>().restoreSourceLayoutR40,["sourceSavedReopened"]=true,["harnesses"]=new JArray(all.Where(t=>t.GetComponent<R40LakeAudit>()||t.GetComponent<R39LakeReview>()||t.GetComponent<R39GeometryProbe>()).Select(PathOf))};File.WriteAllText(Out+"/source-reopened.json",j.ToString());
  if(missing.Length>0||retired||!(bool)j["originalBridge"]||(bool)j["unsupportedRoot"]||!(bool)j["references"]||j["harnesses"].Count()!=0)throw new Exception("r40 source audit");
 }
 public static void ExportSceneDelta(){
  var rows=new JArray();foreach(var path in new[]{ConRoiR38.Scene,ConRoiR39.Scene,Scene}){EditorSceneManager.OpenScene(path);var all=UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects().SelectMany(g=>g.GetComponentsInChildren<Transform>(true)).ToArray();
   var bridge=all.First(t=>t.name=="OngDao_V35_Photo_Batch"||t.name=="OngDao_R39_Protected_Reflection_Overlook");var s=UnityEngine.Object.FindAnyObjectByType<DieuPhoiHanhTrinh>();var n=s.GetComponent<TuongTacSoTayHoR37>();var b=s.GetComponent<TuTheBaoBenCauV48>();
   rows.Add(new JObject{["scene"]=path,["bridge"]=PathOf(bridge),["source"]=PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(bridge.gameObject),["bridgePosition"]=V(bridge.position),["bridgeScale"]=V(bridge.lossyScale),["colliders"]=new JArray(bridge.GetComponentsInChildren<Collider>(true).Select(c=>new JObject{["path"]=PathOf(c.transform),["type"]=c.GetType().Name,["layer"]=c.gameObject.layer,["trigger"]=c.isTrigger,["mesh"]=c is MeshCollider m&&m.sharedMesh?m.sharedMesh.name:"",["asset"]=c is MeshCollider mc?AssetDatabase.GetAssetPath(mc.sharedMesh):""})),["bao"]=V(b.actorPosition),["rail"]=V(b.railContact),["spawn"]=V(s.lakeSpawn.position),["spawnRotation"]=new JArray(s.lakeSpawn.rotation.x,s.lakeSpawn.rotation.y,s.lakeSpawn.rotation.z,s.lakeSpawn.rotation.w),["notebookRoot"]=V(n.root.position),["socket"]=V(n.socket.position),["calendar"]=V(b.calendar.position),["puppetRoot"]=V(s.puppets[0].transform.position),["puppetRotation"]=V(s.puppets[0].transform.eulerAngles),["footAnchor"]=V(s.GetComponent<MoDauBenHoR23>().lakeFootAnchor)});
  }File.WriteAllText(Out+"/scene-exact-delta.json",rows.ToString());EditorSceneManager.OpenScene(Scene);
 }
}
