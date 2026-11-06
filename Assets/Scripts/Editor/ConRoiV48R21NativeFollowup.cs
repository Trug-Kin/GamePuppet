using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEditor;
using UnityEditor.SceneManagement;

public static class ConRoiV48R21NativeFollowup
{
 const string Out="Docs/Reviews/journey_v48/r21/asset-pass";
 static string BuildFolder="Builds/Windows_v48_r21_nativefix";
 static string P(Transform t)=>t.parent?P(t.parent)+"/"+t.name:t.name;
 public static void Audit()
 {
  EditorSceneManager.OpenScene("Assets/Scenes/JourneyRepair_v48_r21.unity");
  var reflection=UnityEngine.Object.FindAnyObjectByType<PhanChieuMatHo>();
  var renderers=UnityEngine.Object.FindObjectsByType<MeshRenderer>(FindObjectsInactive.Include);
  JObject R(Renderer r)=>new JObject{["path"]=P(r.transform),["active"]=r.gameObject.activeInHierarchy,["enabled"]=r.enabled,["bounds"]=r.bounds.ToString("F4"),["material"]=r.sharedMaterial?AssetDatabase.GetAssetPath(r.sharedMaterial):"",["shader"]=r.sharedMaterial?r.sharedMaterial.shader.name:""};
  File.WriteAllText(Out+"/native-followup-audit.json",new JObject{
   ["reflection_bound_water"]=R(reflection.water),
   ["flat_large_surfaces"]=new JArray(renderers.Where(r=>r.bounds.size.x>50&&r.bounds.size.z>50&&r.bounds.size.y<.2f).Select(R)),
   ["shadow_lights"]=new JArray(UnityEngine.Object.FindObjectsByType<Light>(FindObjectsInactive.Include).Where(l=>l.type!=LightType.Directional&&l.shadows!=LightShadows.None).Select(l=>new JObject{["path"]=P(l.transform),["resolution"]=l.shadowResolution.ToString(),["custom"]=l.shadowCustomResolution,["tier"]=l.GetUniversalAdditionalLightData().additionalLightsShadowResolutionTier}))
  }.ToString());
 }
 public static void RepairAndBuild()
 {
  Audit();
  File.Copy("Assets/Scenes/JourneyRepair_v48_r21.unity",Out+"/scene-before-native-runtime-fix.unity",false);
  var reflection=UnityEngine.Object.FindAnyObjectByType<PhanChieuMatHo>();
  var water=GameObject.Find("R20_Whole_Source_Export").GetComponentsInChildren<MeshRenderer>(true).Single(r=>r.name=="Surface_051");
  if(!water.enabled||!water.sharedMaterial||water.sharedMaterial.shader.name!="ConRoi/LakeStoryV34/Water")throw new Exception("Unexpected current water renderer");
  reflection.water=water;
  var lights=new JArray();
  foreach(var light in UnityEngine.Object.FindObjectsByType<Light>(FindObjectsInactive.Include).Where(l=>l.type!=LightType.Directional&&l.shadows!=LightShadows.None))
  {
   lights.Add(new JObject{["path"]=P(light.transform),["previous_custom"]=light.shadowCustomResolution,["previous_tier"]=light.GetUniversalAdditionalLightData().additionalLightsShadowResolutionTier});
   light.shadowCustomResolution=-1;
   var serialized=new SerializedObject(light.GetUniversalAdditionalLightData());
   serialized.FindProperty("m_AdditionalLightsShadowResolutionTier").intValue=UniversalAdditionalLightData.AdditionalLightsShadowResolutionTierLow;
   serialized.ApplyModifiedPropertiesWithoutUndo();EditorUtility.SetDirty(light);
  }
  EditorSceneManager.SaveScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene());AssetDatabase.SaveAssets();
  File.WriteAllText(Out+"/native-runtime-fixes.json",new JObject{["reflection_water"]=P(water.transform),["reason"]="Reflection was bound to retired disabled r20 source renderer; bind current visible water with existing reflection shader",["punctual_shadow_resolution"]=256,["lights"]=lights,["controller_and_gameplay_unchanged"]=true}.ToString());
  BakeAndBuild();
 }
 public static void BakeAndBuild()
 {
  EditorSceneManager.OpenScene("Assets/Scenes/JourneyRepair_v48_r21.unity");
  var meshes=UnityEngine.Object.FindObjectsByType<MeshCollider>(FindObjectsInactive.Include).Where(c=>c.sharedMesh).Select(c=>c.sharedMesh).Distinct().ToArray();
  var baked=new JArray();var deferred=new JArray();
  foreach(var mesh in meshes)
  {
   var path=AssetDatabase.GetAssetPath(mesh);
   if(!path.StartsWith("Assets/Art/Environment/JourneyV48/")||!path.EndsWith(".asset")){if(!mesh.HasPreBakeCollisionMesh(false))deferred.Add(path+"::"+mesh.name);continue;}
   if(mesh.HasPreBakeCollisionMesh(false))continue;
   mesh.SetPreBakeCollisionMesh(false,true);EditorUtility.SetDirty(mesh);baked.Add(path);
  }
  AssetDatabase.SaveAssets();
  if(baked.Count>0||!File.Exists(Out+"/native-collision-prebake.json"))File.WriteAllText(Out+"/native-collision-prebake.json",new JObject{["enabled_triangle_prebake"]=baked,["historical_imported_deferred"]=deferred,["vertex_index_geometry_unchanged"]=true}.ToString());
  var version=PlayerSettings.bundleVersion;
  try{
   PlayerSettings.bundleVersion="0.48.21";
   var report=UnityEditor.BuildPipeline.BuildPlayer(new[]{"Assets/Scenes/JourneyRepair_v48_r21.unity"},BuildFolder+"/ConRoi.exe",BuildTarget.StandaloneWindows64,BuildOptions.Development);
   var messages=new JArray();
   foreach(var step in report.steps)foreach(var message in step.messages)if(message.type==LogType.Warning||message.type==LogType.Error||message.type==LogType.Exception)messages.Add(new JObject{["step"]=step.name,["type"]=message.type.ToString(),["message"]=message.content});
   File.WriteAllText(Out+"/nativefix-build.json",new JObject{["result"]=report.summary.result.ToString(),["errors"]=report.summary.totalErrors,["warnings"]=report.summary.totalWarnings,["messages"]=messages,["status"]="INTERNAL_NOT_ACCEPTED"}.ToString());
   if(report.summary.result!=UnityEditor.Build.Reporting.BuildResult.Succeeded)throw new Exception("Native fix build failed");
  }finally{PlayerSettings.bundleVersion=version;AssetDatabase.SaveAssets();}
 }
 public static void CorrectShadowTierAndBuild()
 {
  BuildFolder="Builds/Windows_v48_r21_nativefix2";
  EditorSceneManager.OpenScene("Assets/Scenes/JourneyRepair_v48_r21.unity");
  foreach(var light in UnityEngine.Object.FindObjectsByType<Light>(FindObjectsInactive.Include).Where(l=>l.type!=LightType.Directional&&l.shadows!=LightShadows.None))
  {
   var serialized=new SerializedObject(light.GetUniversalAdditionalLightData());
   serialized.FindProperty("m_AdditionalLightsShadowResolutionTier").intValue=UniversalAdditionalLightData.AdditionalLightsShadowResolutionTierLow;
   serialized.ApplyModifiedPropertiesWithoutUndo();light.shadowCustomResolution=-1;EditorUtility.SetDirty(light);
  }
  EditorSceneManager.SaveScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene());AssetDatabase.SaveAssets();
  var record=JObject.Parse(File.ReadAllText(Out+"/native-runtime-fixes.json"));record["punctual_shadow_mode"]="Pipeline Low tier = 256px, matching URP 17.6 implementation; supersedes rejected custom-tier trial";File.WriteAllText(Out+"/native-runtime-fixes.json",record.ToString());
  BakeAndBuild();
 }
}
