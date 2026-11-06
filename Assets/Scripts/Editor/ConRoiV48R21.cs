using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.Build.Reporting;

// Working derivative only. No candidate packaging until geometry and native
// verification are complete; r20, RC3 and the immutable save remain untouched.
public static class ConRoiV48R21
{
 const string Out="Docs/Reviews/journey_v48/r21";
 const string Scene="Assets/Scenes/JourneyRepair_v48_r21.unity";
 static JArray V(Vector3 p)=>new JArray(p.x,p.y,p.z);
 static string P(Transform t)=>t.parent?P(t.parent)+"/"+t.name:t.name;
 public static void AssembleDraft()
 {
  Directory.CreateDirectory(Out);
  EditorSceneManager.OpenScene("Assets/Scenes/JourneyRepair_v48_r20.unity");
  var b=UnityEngine.Object.FindAnyObjectByType<TuTheBaoBenCauV48>();
  b.leaningOnly=true;
  foreach(var prop in new[]{b.rod,b.line,b.bag})if(prop)prop.gameObject.SetActive(false);
  // Retain the measured rail and foot contact targets. Equal hand heights remove
  // the last rod-grip asymmetry; head attention can still follow a conversation.
  var side=Vector3.Cross(Vector3.up,-b.waterDirection).normalized;
  b.rightHandTarget.name="RightHandRestingOverRail";
  b.rightHandTarget.position=b.railContact+side*.20f+b.waterDirection*.23f+Vector3.up*.095f;
  var mist=b.gameObject.AddComponent<SuongDaLatV48>();mist.story=b.story;
  EditorSceneManager.SaveScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene(),Scene);
  AssetDatabase.SaveAssets();
  Survey();
 }
 public static void Survey()
 {
  EditorSceneManager.OpenScene(Scene);Physics.SyncTransforms();
  var rows=new JArray();
  foreach(var f in UnityEngine.Object.FindObjectsByType<MeshFilter>(FindObjectsInactive.Include))
  {
   var r=f.GetComponent<Renderer>();if(!r||!f.sharedMesh)continue;
   string p=P(f.transform);var bounds=r.bounds;
   bool local=bounds.Intersects(new Bounds(new Vector3(669,1,507),new Vector3(135,24,180)));
   if(!local&&!p.Contains("Boundary")&&!p.Contains("Fence")&&!p.Contains("Wall"))continue;
   rows.Add(new JObject{["path"]=p,["mesh"]=AssetDatabase.GetAssetPath(f.sharedMesh),["active"]=f.gameObject.activeInHierarchy,["renderer"]=r.enabled,["min"]=V(bounds.min),["max"]=V(bounds.max),["position"]=V(f.transform.position),["scale"]=V(f.transform.lossyScale),["collider"]=f.GetComponent<Collider>()!=null,["material"]=r.sharedMaterial?r.sharedMaterial.name:""});
  }
  File.WriteAllText(Out+"/draft-scene-survey.json",rows.ToString());
  var b=UnityEngine.Object.FindAnyObjectByType<TuTheBaoBenCauV48>();
  File.WriteAllText(Out+"/draft-bao-authoring.json",new JObject{["scene"]=Scene,["leaning_only"]=b.leaningOnly,["layout_revision"]=b.LayoutRevision,["actor"]=V(b.actorPosition),["rail"]=V(b.railContact),["left_elbow"]=V(b.leftElbowTarget.position),["right_elbow"]=V(b.rightElbowTarget.position),["left_hand"]=V(b.leftHandTarget.position),["right_hand"]=V(b.rightHandTarget.position),["fishing_props_active"]=new JArray(new[]{b.rod,b.line,b.bag}.Select(t=>t&&t.gameObject.activeSelf)),["runtime_verified"]=false}.ToString());
 }
 public static void BuildWorking()
 {
  EditorSceneManager.OpenScene(Scene);var version=PlayerSettings.bundleVersion;
  try
  {
   PlayerSettings.bundleVersion="0.48.21";
   var report=BuildPipeline.BuildPlayer(new[]{Scene},"Builds/Windows_v48_r21/ConRoi.exe",BuildTarget.StandaloneWindows64,BuildOptions.Development);
   File.WriteAllText(Out+"/working-build.json",new JObject{["status"]="INTERNAL_WORKING_BUILD_NOT_ACCEPTANCE_CANDIDATE",["result"]=report.summary.result.ToString(),["errors"]=report.summary.totalErrors,["warnings"]=report.summary.totalWarnings,["fog_disabled_for_geometry_review"]=UnityEngine.Object.FindAnyObjectByType<SuongDaLatV48>().clearGeometryReview,["new_photo_matching_pending"]=true,["full_route_boundaries_pending"]=true}.ToString());
   if(report.summary.result!=BuildResult.Succeeded)throw new Exception("Working build failed");
  }
  finally{PlayerSettings.bundleVersion=version;AssetDatabase.SaveAssets();}
 }
 public static void RefineBaoLeanDraft()
 {
  EditorSceneManager.OpenScene(Scene);var b=UnityEngine.Object.FindAnyObjectByType<TuTheBaoBenCauV48>();
  var old=b.actorPosition;var side=Vector3.Cross(Vector3.up,-b.waterDirection).normalized;
  b.actorPosition=b.railContact-b.waterDirection*.30f;b.actorPosition=new Vector3(b.actorPosition.x,old.y,b.actorPosition.z);
  b.actor.SetPositionAndRotation(b.actorPosition,b.actorRotation);
  // Elbows near the torso, wrists just beyond the rail: bent arms rather than
  // the almost straight reach inherited from the fishing grip.
  b.leftElbowTarget.position=b.railContact-side*.22f-b.waterDirection*.25f+Vector3.up*.083f;
  b.rightElbowTarget.position=b.railContact+side*.20f-b.waterDirection*.25f+Vector3.up*.083f;
  b.leftHandTarget.position=b.railContact-side*.22f+b.waterDirection*.08f+Vector3.up*.095f;
  b.rightHandTarget.position=b.railContact+side*.20f+b.waterDirection*.08f+Vector3.up*.095f;
  foreach(var i in b.story.lake.GetComponentsInChildren<TuongTacHoCho>(true).Where(i=>i.kind==TuongTacHoCho.Kind.Bao))if(!i.transform.IsChildOf(b.actor))i.transform.position+=b.actorPosition-old;
  EditorSceneManager.SaveScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene());AssetDatabase.SaveAssets();Survey();
 }
 public static void AdvanceIndependentDraft()
 {
  RefineBaoLeanDraft();
  ConRoiV48R21Footpaths.MatchChangedLocalGradeDraft();
  ConRoiV48R21Boundaries.SchoolAndSlopeDraft();
 }
 public static void EnableAtmosphereDraftAndBuild()
 {
  EditorSceneManager.OpenScene(Scene);var mist=UnityEngine.Object.FindAnyObjectByType<SuongDaLatV48>();mist.clearGeometryReview=false;
  // Serialize EXP2 in the built scene so automatic fog-variant stripping keeps
  // the variant used by the runtime regional profile.
  RenderSettings.fog=true;RenderSettings.fogMode=FogMode.ExponentialSquared;RenderSettings.fogDensity=.006f;RenderSettings.fogColor=new Color(.36f,.43f,.47f);
  EditorSceneManager.SaveScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene());AssetDatabase.SaveAssets();BuildWorking();
 }
}
