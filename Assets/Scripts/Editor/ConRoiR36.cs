using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.Build.Reporting;

// Editor authoring/audit only. No repair component is added to gameplay.
[InitializeOnLoad] public static class ConRoiR36 {
 public const string Scene="Assets/Scenes/JourneyRepair_v48_r36.unity", Out="Docs/Reviews/journey_v48/r36";
 static ConRoiR36(){EditorApplication.playModeStateChanged+=State;}
 static void State(PlayModeStateChange p){if(p==PlayModeStateChange.EnteredPlayMode&&SessionState.GetBool("r36.review",false)){SessionState.SetBool("r36.review",false);UnityEngine.Object.FindAnyObjectByType<DieuPhoiHanhTrinh>().gameObject.AddComponent<R36IntegrationReview>().output=Out+"/editor-main";}}
 public static void Begin(){EditorSceneManager.OpenScene(Scene);SessionState.SetBool("r36.review",true);EditorApplication.EnterPlaymode();}
 static string PathOf(Transform t)=>t.parent?PathOf(t.parent)+"/"+t.name:t.name;
 static JArray V(Vector3 v)=>new JArray(v.x,v.y,v.z);
 public static void Inspect(){EditorSceneManager.OpenScene(ConRoiR35.Scene);Audit("source-r35");}
 public static void Audit(string name){
  var scene=UnityEngine.SceneManagement.SceneManager.GetActiveScene();var all=scene.GetRootGameObjects().SelectMany(g=>g.GetComponentsInChildren<Transform>(true)).ToArray();
  var missing=new JArray();var broken=new JArray();var prefab=new JArray();var required=new JArray();var texts=new JArray();
  foreach(var t in all){int count=GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(t.gameObject);if(count>0)missing.Add(new JObject{["path"]=PathOf(t),["count"]=count});
   if(PrefabUtility.IsAnyPrefabInstanceRoot(t.gameObject))prefab.Add(new JObject{["path"]=PathOf(t),["status"]=PrefabUtility.GetPrefabInstanceStatus(t.gameObject).ToString(),["source"]=PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(t.gameObject)});
   foreach(var c in t.GetComponents<Component>().Where(c=>c!=null)){var so=new SerializedObject(c);var p=so.GetIterator();while(p.NextVisible(true))if(p.propertyType==SerializedPropertyType.ObjectReference&&p.objectReferenceValue==null&&!p.objectReferenceEntityIdValue.Equals(default(UnityEngine.EntityId)))broken.Add(new JObject{["path"]=PathOf(t),["component"]=c.GetType().Name,["field"]=p.propertyPath});}
  }
  var s=UnityEngine.Object.FindAnyObjectByType<DieuPhoiHanhTrinh>();var a=s.GetComponent<QuanLyGoiHangChoR35>();
  Action<string,UnityEngine.Object> need=(n,o)=>required.Add(new JObject{["field"]=n,["valid"]=o!=null,["target"]=o?o.name:"NULL"});
  need("activity.story",a.story);foreach(var f in new[]{("root",a.root),("delivery",a.delivery),("slip",a.slip),("puppet",a.puppet),("gesture",a.gesture),("mimicPackage",a.mimicPackage),("mimicLid",a.mimicLid)})need(f.Item1,f.Item2);need("preview",a.placementPreview);
  need("story.player",s.player);need("story.hub",s.hub);need("story.market",s.market);need("story.lake",s.lake);need("story.polish",s.polish);need("story.schoolChapter",s.schoolChapter);need("story.slopeChapter",s.slopeChapter);
  need("player.controller",s.player.controller);need("player.groundCheck",s.player.groundCheck);need("interaction.camera",s.player.GetComponent<TuongTacNguoiChoi>().playerCamera);need("polish.eye",s.polish.eye);need("profile.story",s.GetComponent<HoSoLuuTaiR35>().story);
  foreach(var p in a.parcels){need(p.name+".activity",p.activity);need(p.name+".lid",p.lid);need(p.name+".scarf",p.scarf);need(p.name+".label",p.label);need(p.name+".inside",p.inside);need(p.name+".collider",p.GetComponent<BoxCollider>());}
  foreach(var p in new[]{a.slip,a.delivery})need(p.name+".activity",p.GetComponent<DichTuongTacChoR35>().activity);
  foreach(var t in all.Select(t=>t.GetComponent<TextMesh>()).Where(t=>t&&Vector3.Distance(t.transform.position,a.delivery.position)<16)){
   var r=t.GetComponent<Renderer>();var m=r.sharedMaterial;texts.Add(new JObject{["path"]=PathOf(t.transform),["text"]=t.text,["material"]=AssetDatabase.GetAssetPath(m),["shader"]=m?m.shader.name:"NULL",["queue"]=m?m.renderQueue:0,["sortingLayer"]=r.sortingLayerName,["sortingOrder"]=r.sortingOrder,["layer"]=t.gameObject.layer,["position"]=V(t.transform.position),["font"]=t.font?t.font.name:"default",["atlas"]=m&&m.mainTexture?m.mainTexture.name:"NULL"});
  }
  var colliders=new JArray(a.root.GetComponentsInChildren<Collider>(true).Select(c=>(JToken)new JObject{["path"]=PathOf(c.transform),["type"]=c.GetType().Name,["enabled"]=c.enabled,["trigger"]=c.isTrigger,["layer"]=c.gameObject.layer,["scale"]=V(c.transform.lossyScale),["rigidbody"]=c.attachedRigidbody!=null}));
  var j=new JObject{["scene"]=scene.path,["missingScripts"]=missing,["brokenSerializedReferences"]=broken,["required"]=required,["prefabs"]=prefab,["nearbyWorldText"]=texts,["colliders"]=colliders,["players"]=all.Count(t=>t.GetComponent<DieuKhienDiChuyen>()),["cameras"]=new JArray(all.Where(t=>t.GetComponent<Camera>()).Select(t=>PathOf(t))),["listeners"]=new JArray(all.Where(t=>t.GetComponent<AudioListener>()).Select(t=>PathOf(t))),["eventSystems"]=all.Count(t=>t.GetComponent<EventSystem>()),["serializedHarnesses"]=new JArray(all.SelectMany(t=>t.GetComponents<MonoBehaviour>()).Where(c=>c&&(c.GetType().Name.Contains("Review")||c.GetType().Name.Contains("Harness"))).Select(c=>c.GetType().Name)),["buildScenes"]=new JArray(EditorBuildSettings.scenes.Select(x=>x.path))};
  File.WriteAllText(Out+"/"+name+".json",j.ToString());
  if(missing.Count>0||broken.Count>0||required.Any(x=>!(bool)x["valid"])||prefab.Any(x=>(string)x["status"]=="MissingAsset"))throw new Exception("Integration audit failed; see "+name);
 }
 public static void Author(){
  if(File.Exists(Scene))throw new Exception("Refuse overwrite checkpoint");
  EditorSceneManager.OpenScene(ConRoiR35.Scene);Audit("source-r35");
  if(!AssetDatabase.CopyAsset(ConRoiR35.Scene,Scene))throw new Exception("Copy failed");EditorSceneManager.OpenScene(Scene);
  var a=UnityEngine.Object.FindAnyObjectByType<QuanLyGoiHangChoR35>();Directory.CreateDirectory("Assets/Art/Gameplay/MarketR36");AssetDatabase.Refresh();var changed=new JArray();
  // Scene-local world lettering, including package labels. Leave screen UI and shared font material untouched.
  var texts=UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects().SelectMany(g=>g.GetComponentsInChildren<TextMesh>(true)).Where(t=>Vector3.Distance(t.transform.position,a.delivery.position)<16).ToArray();
  foreach(var t in texts){var r=t.GetComponent<Renderer>();var old=r.sharedMaterial;if(!old||old.shader.name=="ConRoi/WorldText")continue;
   string path="Assets/Art/Gameplay/MarketR36/DepthTestedFont.mat";var m=AssetDatabase.LoadAssetAtPath<Material>(path);if(!m){m=new Material(old){name="Market_DepthTestedFont"};m.shader=Shader.Find("ConRoi/WorldText");m.renderQueue=3000;AssetDatabase.CreateAsset(m,path);}
   if(m.mainTexture!=old.mainTexture)throw new Exception("Different font atlas requires own material: "+t.name);
   changed.Add(new JObject{["path"]=PathOf(t.transform),["oldShader"]=old.shader.name,["oldMaterial"]=AssetDatabase.GetAssetPath(old),["newMaterial"]=path});r.sharedMaterial=m;EditorUtility.SetDirty(r);
  }
  EditorBuildSettings.scenes=new[]{new EditorBuildSettingsScene(Scene,true)};EditorSceneManager.SaveScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene());AssetDatabase.SaveAssets();
  EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);EditorSceneManager.OpenScene(Scene);Audit("reopened-r36");File.WriteAllText(Out+"/material-changes.json",changed.ToString());
 }
 public static void Build(){
  EditorSceneManager.OpenScene(Scene);Audit("prebuild-r36");EditorBuildSettings.scenes=new[]{new EditorBuildSettingsScene(Scene,true)};
  string exe=Path.GetFullPath("Builds/Windows_v48_r36_integrated/ConRoi.exe");Directory.CreateDirectory(Path.GetDirectoryName(exe));var r=BuildPipeline.BuildPlayer(new[]{Scene},exe,BuildTarget.StandaloneWindows64,BuildOptions.Development|BuildOptions.DetailedBuildReport);
  File.WriteAllText(Out+"/build.json",new JObject{["result"]=r.summary.result.ToString(),["errors"]=r.summary.totalErrors,["warnings"]=r.summary.totalWarnings,["scene"]=Scene,["binary"]=exe}.ToString());if(r.summary.result!=BuildResult.Succeeded)throw new Exception("Build failed");
 }
}
