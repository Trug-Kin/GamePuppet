using System;
using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.Build.Reporting;
using System.Linq;
using Newtonsoft.Json.Linq;
[InitializeOnLoad] public static class ConRoiR39 {
 public const string Scene="Assets/Scenes/JourneyRepair_v48_r39.unity",Out="Docs/Reviews/journey_v48/r39";
 static ConRoiR39(){EditorApplication.playModeStateChanged+=Mode;}
 static void Mode(PlayModeStateChange p){if(p==PlayModeStateChange.EnteredPlayMode&&SessionState.GetBool("r39.review",false)){SessionState.SetBool("r39.review",false);new GameObject("R39_OptInReview").AddComponent<R39LakeReview>().output=Out+"/"+SessionState.GetString("r39.output","editor-final");}if(p==PlayModeStateChange.EnteredPlayMode&&SessionState.GetBool("r39.probe",false)){SessionState.SetBool("r39.probe",false);new GameObject("R39_Diagnostic_Only").AddComponent<R39GeometryProbe>();}}
 public static void Diagnose(){Directory.CreateDirectory(Out);EditorSceneManager.OpenScene(ConRoiR38.Scene);SessionState.SetBool("r39.probe",true);EditorApplication.EnterPlaymode();}
 public static void ExportGeometry(){
  Directory.CreateDirectory(Out);EditorSceneManager.OpenScene(ConRoiR38.Scene);var r=UnityEngine.Object.FindAnyObjectByType<PhanChieuMatHo>();var b=r.GetComponent<TuTheBaoBenCauV48>();
  var meshes=UnityEngine.Object.FindObjectsByType<MeshFilter>(FindObjectsInactive.Include,FindObjectsSortMode.None).Where(f=>f.sharedMesh&&f.GetComponent<Renderer>()&&Vector3.Distance(f.GetComponent<Renderer>().bounds.ClosestPoint(b.actorPosition),b.actorPosition)<25&&!f.transform.IsChildOf(b.actor)&&!f.transform.IsChildOf(r.GetComponent<DieuPhoiHanhTrinh>().player.transform));var rows=new JArray();
  foreach(var f in meshes){using(var data=MeshUtility.AcquireReadOnlyMeshData(f.sharedMesh)){
   var verts=new Unity.Collections.NativeArray<Vector3>(data[0].vertexCount,Unity.Collections.Allocator.Temp);data[0].GetVertices(verts);var v=new JArray(verts.ToArray().Select(p=>{var w=f.transform.TransformPoint(p);return new JArray(w.x,w.y,w.z);}));verts.Dispose();var tris=new JArray();
   for(int sub=0;sub<data[0].subMeshCount;sub++){if(data[0].GetSubMesh(sub).topology!=MeshTopology.Triangles)continue;var a=new Unity.Collections.NativeArray<int>(data[0].GetSubMesh(sub).indexCount,Unity.Collections.Allocator.Temp);data[0].GetIndices(a,sub);foreach(int j in a)tris.Add(j);a.Dispose();}
   rows.Add(new JObject{["name"]=f.name,["path"]=f.transform.parent?f.transform.parent.name+"/"+f.name:f.name,["water"]=f.gameObject==r.water.gameObject,["enabled"]=f.GetComponent<Renderer>().enabled,["active"]=f.gameObject.activeInHierarchy,["vertices"]=v,["triangles"]=tris});
  }}File.WriteAllText(Out+"/source-world-geometry.json",rows.ToString(Newtonsoft.Json.Formatting.None));Debug.Log("R39_EXPORTED "+rows.Count);
 }

 static Vector3 Vec(JToken a)=>new Vector3((float)a[0],(float)a[1],(float)a[2]);
 static JArray V(Vector3 a)=>new JArray(a.x,a.y,a.z);
 public static void Author(){
  if(!SessionState.GetBool("r39.archival-authoring-explicitly-approved",false))throw new Exception("r39 U overlook retired: use r40 source-backed layout. Historical tools must not recreate unsupported construction.");
  if(File.Exists(Scene))throw new Exception("Keep derivative");Directory.CreateDirectory(Out);AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
  const string path="Assets/Art/Environment/LakeR39/OngDaoReflectionOverlook.fbx";
  var imp=(ModelImporter)AssetImporter.GetAtPath(path);imp.isReadable=true;imp.addCollider=false;imp.materialImportMode=ModelImporterMaterialImportMode.None;imp.SaveAndReimport();
  AssetDatabase.CopyAsset(ConRoiR38.Scene,Scene);EditorSceneManager.OpenScene(Scene);
  var s=UnityEngine.Object.FindAnyObjectByType<DieuPhoiHanhTrinh>();var b=s.GetComponent<TuTheBaoBenCauV48>();var n=s.GetComponent<TuongTacSoTayHoR37>();var r=s.GetComponent<PhanChieuMatHo>();
  var old=GameObject.Find("OngDao_V35_Photo_Batch");if(!old)throw new Exception("Expected bridge prefab");var position=old.transform.position;var rotation=old.transform.rotation;var scale=old.transform.localScale;UnityEngine.Object.DestroyImmediate(old);
  var model=(GameObject)PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>(path));model.name="OngDao_R39_Protected_Reflection_Overlook";model.transform.SetPositionAndRotation(position,rotation);model.transform.localScale=scale;
  var materials=JObject.Parse(File.ReadAllText("Assets/Art/Environment/OngDaoV35/bridge_record.json"))["materials"];
  foreach(var mf in model.GetComponentsInChildren<MeshFilter>()){
   if(mf.name.StartsWith("OD35_COLLIDER_")){mf.GetComponent<Renderer>().enabled=false;mf.gameObject.layer=3;mf.gameObject.AddComponent<MeshCollider>().sharedMesh=mf.sharedMesh;}
   else{string index=mf.name.Substring("OD35_Surface_".Length);mf.GetComponent<Renderer>().sharedMaterial=AssetDatabase.LoadAssetAtPath<Material>("Assets/Art/Environment/OngDaoV35/"+(string)materials[index]["name"]+".mat");}
  }
  var l=JObject.Parse(File.ReadAllText("Assets/Art/Environment/LakeR39/layout.json"));var target=Vec(l["bao"]);var delta=target-b.actorPosition;
  b.actor.position+=delta;b.actorPosition=target;b.railContact+=delta;
  foreach(var t in new[]{b.leftElbowTarget,b.leftHandTarget,b.rightElbowTarget,b.rightHandTarget,b.reflectionLight})if(t&&!t.IsChildOf(b.actor))t.position+=delta;
  // Keep the old rail's source height and lean chain, aligning new end rail to the same 1.02m contact.
  var stand=n.root.GetComponentsInChildren<Collider>(true).First(c=>c.GetComponent<DichTuongTacSoTayR37>()&&c.transform!=n.book).transform;
  var standTarget=target-b.waterDirection*1.5f;var shift=standTarget-stand.position;n.root.position+=shift;
  s.lakeSpawn.position=Vec(l["observer"])+Vector3.up*.91204f;s.lakeSpawn.rotation=Quaternion.LookRotation(target-s.lakeSpawn.position+Vector3.up*.91204f);
  var foot=Vec(l["puppet"])+Vector3.up*.0121f;var puppet=s.puppets[0];puppet.transform.position+=foot-(puppet.dauTransform.position-Vector3.up*1.58f);s.GetComponent<MoDauBenHoR23>().lakeFootAnchor=foot;s.GetComponent<MoDauBenHoR23>().stripHalfWidth=.25f;
  r.resolvedRequestUvR39=true;
  File.WriteAllText(Out+"/authored-layout.json",new JObject{["bridge"]=path,["bridgePosition"]=V(position),["bridgeScale"]=V(scale),["bao"]=V(target),["rail"]=V(b.railContact),["socket"]=V(n.socket.position),["spawn"]=V(s.lakeSpawn.position),["puppetFoot"]=V(foot),["waterRenderer"]=r.water.name,["resolvedUv"]=r.resolvedRequestUvR39}.ToString());
  EditorSceneManager.SaveScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene());AssetDatabase.SaveAssets();EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);EditorSceneManager.OpenScene(Scene);Validate();
 }
 public static void Validate(){
  var scene=UnityEngine.SceneManagement.SceneManager.GetActiveScene();var all=scene.GetRootGameObjects().SelectMany(g=>g.GetComponentsInChildren<Transform>(true)).ToArray();var s=UnityEngine.Object.FindAnyObjectByType<DieuPhoiHanhTrinh>();var r=s.GetComponent<PhanChieuMatHo>();var n=s.GetComponent<TuongTacSoTayHoR37>();
  var missing=all.Where(t=>GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(t.gameObject)>0).Select(t=>t.name).ToArray();
  var j=new JObject{["scene"]=scene.path,["missingScripts"]=new JArray(missing),["players"]=all.Count(t=>t.GetComponent<DieuKhienDiChuyen>()),["listeners"]=all.Count(t=>t.GetComponent<AudioListener>()),["reflectionRefs"]=r.water&&r.playerCamera&&r.notebook==n,["resolvedUv"]=r.resolvedRequestUvR39,["bookRefs"]=n.root&&n.book&&n.gesture&&n.socket&&n.leaf&&n.leftPage,["harnesses"]=new JArray(all.SelectMany(t=>t.GetComponents<MonoBehaviour>()).Where(c=>c&&(c is R39GeometryProbe||c is R38LakeReview)).Select(c=>c.name)),["sourceReopened"]=true};File.WriteAllText(Out+"/source-reopened.json",j.ToString());
  if(missing.Length>0||!(bool)j["reflectionRefs"]||!(bool)j["bookRefs"]||(int)j["players"]!=1||(int)j["listeners"]!=1||j["harnesses"].Count()!=0)throw new Exception("r39 source audit");
 }
 public static void Begin(){EditorSceneManager.OpenScene(Scene);SessionState.SetBool("r39.review",true);EditorApplication.EnterPlaymode();}
 public static void Build(){
  if(!SessionState.GetBool("r39.archival-authoring-explicitly-approved",false))throw new Exception("r39 U overlook retired: use r40 source-backed layout. Historical tools must not recreate unsupported construction.");EditorSceneManager.OpenScene(Scene);Validate();EditorBuildSettings.scenes=new[]{new EditorBuildSettingsScene(Scene,true)};string exe=Path.GetFullPath("Builds/Windows_v48_r39_integrated/ConRoi.exe");Directory.CreateDirectory(Path.GetDirectoryName(exe));var result=BuildPipeline.BuildPlayer(new[]{Scene},exe,BuildTarget.StandaloneWindows64,BuildOptions.Development|BuildOptions.DetailedBuildReport);File.WriteAllText(Out+"/build.json",new JObject{["result"]=result.summary.result.ToString(),["errors"]=result.summary.totalErrors,["warnings"]=result.summary.totalWarnings,["scene"]=Scene,["binary"]=exe}.ToString());if(result.summary.result!=BuildResult.Succeeded)throw new Exception("Build failed");}

 public static void Tune(){
  if(!SessionState.GetBool("r39.archival-authoring-explicitly-approved",false))throw new Exception("r39 U overlook retired: use r40 source-backed layout. Historical tools must not recreate unsupported construction.");
  AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);EditorSceneManager.OpenScene(Scene);var s=UnityEngine.Object.FindAnyObjectByType<DieuPhoiHanhTrinh>();var b=s.GetComponent<TuTheBaoBenCauV48>();var n=s.GetComponent<TuongTacSoTayHoR37>();var l=JObject.Parse(File.ReadAllText("Assets/Art/Environment/LakeR39/layout.json"));var target=Vec(l["bao"]);var delta=target-b.actorPosition;
  b.actor.position+=delta;b.actorPosition=target;b.railContact+=delta;foreach(var t in new[]{b.leftElbowTarget,b.leftHandTarget,b.rightElbowTarget,b.rightHandTarget,b.reflectionLight})if(t&&!t.IsChildOf(b.actor))t.position+=delta;
  foreach(var t in s.lake.GetComponentsInChildren<TuongTacHoCho>(true).Where(i=>i.kind==TuongTacHoCho.Kind.Bao))if(!t.transform.IsChildOf(b.actor))t.transform.position+=delta;
  var stand=n.root.GetComponentsInChildren<Collider>(true).First(c=>c.GetComponent<DichTuongTacSoTayR37>()&&c.transform!=n.book).transform;n.root.position+=target-b.waterDirection*1.5f-stand.position;
  s.lakeSpawn.position=Vec(l["observer"])+Vector3.up*.91204f;s.lakeSpawn.rotation=Quaternion.LookRotation((target-s.lakeSpawn.position)+Vector3.up*.91204f);
  var foot=Vec(l["puppet"])+Vector3.up*.0121f;var puppet=s.puppets[0];puppet.transform.position+=foot-(puppet.dauTransform.position-Vector3.up*1.58f);var opening=s.GetComponent<MoDauBenHoR23>();opening.lakeFootAnchor=foot;opening.stripHalfWidth=.25f;
  var cal=s.lake.GetComponentsInChildren<Transform>(true).First(t=>t.name=="R11_Calendar_Complete_Assembly");b.calendar.position=cal.position+new Vector3(.035536f,1.349932f,.041842f);cal.position=Vec(l["entrance"])+Vec(l["along"])*-4.05f+Vec(l["waterward"])*12.2f+Vector3.up*.00207f;
  Physics.SyncTransforms();var ent=Vec(l["entrance"]);var direction=Vec(l["waterward"]);var rays=new JArray();
  for(float h=.35f;h<1.8f;h+=.35f)foreach(var hit in Physics.RaycastAll(ent+direction*2+Vector3.up*h,-direction,4,~0,QueryTriggerInteraction.Ignore))rays.Add(new JObject{["height"]=h,["name"]=hit.collider.name,["path"]=hit.transform.parent?hit.transform.parent.name+"/"+hit.collider.name:hit.collider.name,["point"]=V(hit.point),["mesh"]=hit.collider is MeshCollider mc&&mc.sharedMesh?AssetDatabase.GetAssetPath(mc.sharedMesh):hit.collider.GetType().Name});
  File.WriteAllText(Out+"/entrance-ray-diagnostic.json",rays.ToString());
  EditorSceneManager.SaveScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene());AssetDatabase.SaveAssets();EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);EditorSceneManager.OpenScene(Scene);Validate();SessionState.SetString("r39.output","editor-final");SessionState.SetBool("r39.review",true);EditorApplication.EnterPlaymode();
 }

 public static void FinalizeAndBuild(){
  if(!SessionState.GetBool("r39.archival-authoring-explicitly-approved",false))throw new Exception("r39 U overlook retired: use r40 source-backed layout. Historical tools must not recreate unsupported construction.");
  EditorSceneManager.OpenScene(Scene);var n=UnityEngine.Object.FindAnyObjectByType<TuongTacSoTayHoR37>();var r=n.story.GetComponent<PhanChieuMatHo>();n.gestureLiftDegrees=60;
  var puppet=n.story.puppets[0];var foot=n.story.GetComponent<MoDauBenHoR23>().lakeFootAnchor;var face=n.story.lakeSpawn.position-foot;face.y=0;
  // The original pose faced the old inland spawn; rotate the complete existing rig around its supported anchor.
  puppet.transform.RotateAround(foot,Vector3.up,180);
  string path="Assets/Art/Environment/LakeR39/WaterReflection.mat";if(AssetDatabase.LoadAssetAtPath<Material>(path))throw new Exception("Refuse repeat finalization");
  var material=new Material(r.water.sharedMaterial);material.SetFloat("_ReflectionGain",2.6f);AssetDatabase.CreateAsset(material,path);r.water.sharedMaterial=material;
  EditorSceneManager.SaveScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene());AssetDatabase.SaveAssets();EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);EditorSceneManager.OpenScene(Scene);Validate();Build();
 }
}
