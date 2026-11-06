using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public static class ConRoiReviewV44
{
 public const string Report="Docs/Reviews/journey_v44",Scene="Assets/Scenes/JourneyExperience_v44.unity",Art="Assets/Art/Environment/JourneyV44";
 public static void Assemble()
 {
  if(File.Exists(Scene))throw new Exception("v44 exists; preserve checkpoint");
  Directory.CreateDirectory(Art);Directory.CreateDirectory(Report+"/comparison");
  EditorSceneManager.OpenScene(ConRoiReviewV43.Scene);Capture("before",true);
  var s=UnityEngine.Object.FindAnyObjectByType<DieuPhoiHanhTrinh>();
  // The room's low sill was traversable by jumping. Close the actual opening
  // with a fitted structural grille and ceiling, both rendered and collidable.
  var timber=AssetDatabase.LoadAssetAtPath<Material>("Assets/Art/Environment/JourneyV42/Timber_Worn_Warm.mat");
  var root=new GameObject("V44_Room_Structural_Envelope").transform;root.SetParent(s.hub.transform,true);
  for(int n=0;n<8;n++)Solid(root,"Window_Structural_Mullion_"+n,new Vector3(-36.55f+n*.443f,1.70f,-37.5f),new Vector3(.075f,1.60f,.14f),timber);
  Solid(root,"Room_Ceiling_Support",new Vector3(-35,3.075f,-35),new Vector3(6.16f,.15f,5.16f),timber);
  CharacterCraftV44.All(s);File.WriteAllText(Report+"/character-authoring.json",CharacterCraftV44.Evidence.ToString());
  s.gameObject.AddComponent<ManHinhTaiV44>().story=s;
  EditorSceneManager.SaveScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene(),Scene);AssetDatabase.SaveAssets();Capture("after",false);
 }
 static void Solid(Transform p,string name,Vector3 pos,Vector3 size,Material mat)
 {
  var g=GameObject.CreatePrimitive(PrimitiveType.Cube);g.name=name;g.layer=3;g.transform.SetParent(p,true);g.transform.position=pos;g.transform.localScale=size;g.GetComponent<Renderer>().sharedMaterial=mat;
 }
 public static void Capture(string suffix,bool define)
 {
  var s=UnityEngine.Object.FindAnyObjectByType<DieuPhoiHanhTrinh>();var camera=s.player.GetComponentInChildren<Camera>();
  var actors=new[]{s.slopeChapter.ghost,s.completion.lan,s.completion.vy,s.schoolChapter.ghost,s.ghosts[0],s.ghosts[1]};var roles=new[]{"Tung","Lan","Vy","Mai","Bao","Ha"};
  var areas=new[]{s.slopeChapter.environment,s.hub,s.market,s.schoolChapter.environment,s.lake,s.market};var all=new[]{s.hub,s.lake,s.market,s.schoolChapter.environment,s.slopeChapter.environment};
  var active=all.Select(g=>g.activeSelf).ToArray();var actorActive=actors.Select(g=>g.gameObject.activeSelf).ToArray();var old=camera.transform.position;var oldq=camera.transform.rotation;float fov=camera.fieldOfView;var poses=define?new JArray():JArray.Parse(File.ReadAllText(Report+"/comparison/cameras_before.json"));
  var rt=RenderTexture.GetTemporary(1280,960,24);var tex=new Texture2D(1280,960,TextureFormat.RGB24,false);
  for(int i=0;i<actors.Length;i++)
  {
   foreach(var a in all)a.SetActive(a==areas[i]);actors[i].gameObject.SetActive(true);
   for(int view=0;view<2;view++)
   {
    Vector3 pos,target;
    if(define)
    {
     var parts=actors[i].GetComponentsInChildren<MeshRenderer>(true);var face=parts.First(r=>r.name.EndsWith("_Face"));var nose=parts.FirstOrDefault(r=>r.name.EndsWith("_Nose"));var front=nose?nose.bounds.center-face.bounds.center:Vector3.back;front.y=0;var q=Quaternion.LookRotation(-front.normalized);
     var floor=parts.Where(r=>r.enabled).Min(r=>r.bounds.min.y);var origin=new Vector3(face.bounds.center.x,floor,face.bounds.center.z);
     target=view==0?origin+Vector3.up*.94f:face.bounds.center;pos=view==0?origin+q*new Vector3(-1.2f,1.24f,-2.5f):target+q*new Vector3(-.34f,.04f,-.82f);
     if(i==1&&view==0)pos=origin+q*new Vector3(1.25f,1.18f,-1.30f);
     poses.Add(new JObject{["role"]=roles[i],["view"]=view==0?"body":"face",["position"]=new JArray(pos.x,pos.y,pos.z),["target"]=new JArray(target.x,target.y,target.z),["fov"]=view==0?42:32,["width"]=1280,["height"]=960});
     if(i==1&&view==0)poses.Last["fov"]=58;
    }
    else{var p=poses[i*2+view];pos=V(p["position"]);target=V(p["target"]);}
    camera.fieldOfView=(float)poses[i*2+view]["fov"];camera.transform.SetPositionAndRotation(pos,Quaternion.LookRotation(target-pos));
    for(int warm=0;warm<3;warm++)RenderPipeline.SubmitRenderRequest(camera,new UniversalRenderPipeline.SingleCameraRequest{destination=rt});
    var previous=RenderTexture.active;RenderTexture.active=rt;tex.ReadPixels(new Rect(0,0,1280,960),0,0);tex.Apply();RenderTexture.active=previous;
    File.WriteAllBytes(Report+"/comparison/"+roles[i]+"_"+(view==0?"body":"face")+"_"+suffix+".png",tex.EncodeToPNG());
   }
  }
  File.WriteAllText(Report+"/comparison/cameras_"+suffix+".json",poses.ToString());RenderTexture.ReleaseTemporary(rt);UnityEngine.Object.DestroyImmediate(tex);
  for(int i=0;i<all.Length;i++)all[i].SetActive(active[i]);for(int i=0;i<actors.Length;i++)actors[i].gameObject.SetActive(actorActive[i]);camera.transform.SetPositionAndRotation(old,oldq);camera.fieldOfView=fov;
 }
 static Vector3 V(JToken t)=>new Vector3((float)t[0],(float)t[1],(float)t[2]);
 public static void AssembleAndBuild()
 {
  if(!File.Exists(Scene))Assemble();
  Build();
 }
 public static void Build()
 {
  EditorSceneManager.OpenScene(Scene);EditorBuildSettings.scenes=new[]{new EditorBuildSettingsScene(Scene,true)};PlayerSettings.bundleVersion="0.44.0";PlayerSettings.runInBackground=true;PlayerSettings.enableFrameTimingStats=true;
  PlayerSettings.SetUseDefaultGraphicsAPIs(BuildTarget.StandaloneWindows64,false);PlayerSettings.SetGraphicsAPIs(BuildTarget.StandaloneWindows64,new[]{GraphicsDeviceType.Direct3D11});
  Capture("after",false);var r=BuildPipeline.BuildPlayer(new[]{Scene},"Builds/Windows_v44_r4/ConRoi.exe",BuildTarget.StandaloneWindows64,BuildOptions.Development);
  File.WriteAllText(Report+"/build.json",new JObject{["result"]=r.summary.result.ToString(),["errors"]=r.summary.totalErrors,["warnings"]=r.summary.totalWarnings,["path"]=r.summary.outputPath,["messages"]=new JArray(r.steps.SelectMany(s=>s.messages).Where(m=>m.type==LogType.Error||m.type==LogType.Warning).Select(m=>m.content))}.ToString());
 }
 public static void RefineAndBuild()
 {
  File.Copy(Report+"/build.json",Report+"/build-r1.json",false);
  EditorSceneManager.OpenScene(ConRoiReviewV43.Scene);Capture("before",true);
  var baseline=BuildPipeline.BuildPlayer(new[]{ConRoiReviewV43.Scene},"Builds/Windows_v44_boundary_baseline/ConRoi.exe",BuildTarget.StandaloneWindows64,BuildOptions.Development);
  File.WriteAllText(Report+"/boundary-baseline-build.json",new JObject{["result"]=baseline.summary.result.ToString(),["purpose"]="v43 scene, unchanged physical envelope; v44 diagnostics and virtual-action boundary regression only"}.ToString());
  EditorSceneManager.OpenScene(Scene);var s=UnityEngine.Object.FindAnyObjectByType<DieuPhoiHanhTrinh>();CharacterCraftV44.Refine(s);
  EditorSceneManager.SaveScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene());AssetDatabase.SaveAssets();Build();
 }
 public static void BuildBoth()
 {
  File.Copy(Report+"/build.json",Report+"/build-r2-failed.json",false);
  var baseline=BuildPipeline.BuildPlayer(new[]{ConRoiReviewV43.Scene},"Builds/Windows_v44_boundary_baseline_r3/ConRoi.exe",BuildTarget.StandaloneWindows64,BuildOptions.Development);
  File.WriteAllText(Report+"/boundary-baseline-build-r3.json",new JObject{["result"]=baseline.summary.result.ToString(),["errors"]=baseline.summary.totalErrors,["path"]=baseline.summary.outputPath,["purpose"]="Unchanged v43 scene and controller geometry; new input-only boundary diagnostic; no loading gate"}.ToString());
  Build();
 }
 public static void FinishProportions()
 {
  File.Copy(Report+"/build.json",Report+"/build-r3.json",false);
  File.Copy(Scene,Report+"/scene-v44-r3.unity.snapshot",false);
  EditorSceneManager.OpenScene(Scene);var s=UnityEngine.Object.FindAnyObjectByType<DieuPhoiHanhTrinh>();
  var head=s.slopeChapter.ghost.GetComponentsInChildren<Transform>(true).First(t=>t.name=="V44_Tung_SculptedHead");
  head.localScale=Vector3.Scale(head.localScale,new Vector3(1,.80f,1));head.position+=Vector3.down*.04f;
  EditorSceneManager.SaveScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene());AssetDatabase.SaveAssets();
  var authored=JArray.Parse(File.ReadAllText(Report+"/character-authoring.json"));var entry=authored.First(x=>(string)x["role"]=="Tung");entry["final_head_vertical_scale"]=.8;entry["final_head_center"]=head.position.ToString("F4");File.WriteAllText(Report+"/character-authoring.json",authored.ToString());
  Build();
 }
 public static void Inspect()
 {
  EditorSceneManager.OpenScene(ConRoiReviewV43.Scene);var s=UnityEngine.Object.FindAnyObjectByType<DieuPhoiHanhTrinh>();
  File.WriteAllText(Report+"/additional-inspection.json",new JObject{["hub_renderers"]=new JArray(s.hub.GetComponentsInChildren<MeshRenderer>(true).Select(r=>new JObject{["name"]=r.name,["bounds"]=r.bounds.ToString("F3")})),["actors"]=new JArray(new[]{s.completion.lan,s.completion.vy,s.slopeChapter.ghost,s.schoolChapter.ghost}.Select(a=>new JObject{["name"]=a.name,["position"]=a.position.ToString("F3"),["rotation"]=a.eulerAngles.ToString(),["parts"]=new JArray(a.GetComponentsInChildren<MeshRenderer>(true).Select(r=>new JObject{["name"]=r.name,["enabled"]=r.enabled,["bounds"]=r.bounds.ToString("F3")}))}))}.ToString());
  StartupProfileReviewV44.Export();
 }
 public static void Diagnostic()
 {
  Directory.CreateDirectory(Report);EditorSceneManager.OpenScene(ConRoiReviewV43.Scene);
  var s=UnityEngine.Object.FindAnyObjectByType<DieuPhoiHanhTrinh>();
  var report=new JObject();report["colliders"]=new JArray(s.hub.GetComponentsInChildren<Collider>(true).Select(c=>new JObject{["name"]=c.name,["type"]=c.GetType().Name,["position"]=c.transform.position.ToString("F4"),["bounds"]=c.bounds.ToString("F4"),["enabled"]=c.enabled,["active"]=c.gameObject.activeSelf,["trigger"]=c.isTrigger}));
  report["actors"]=new JArray(UnityEngine.Object.FindObjectsByType<HienGuongMatTheoKhu>(FindObjectsInactive.Include,FindObjectsSortMode.None).Select(a=>new JObject{["actor"]=a.name,["position"]=a.transform.position.ToString("F4"),["scale"]=a.transform.lossyScale.ToString("F4"),["parts"]=new JArray(a.GetComponentsInChildren<MeshRenderer>(true).Select(r=>new JObject{["name"]=r.name,["enabled"]=r.enabled,["bounds"]=r.bounds.ToString("F4"),["localScale"]=r.transform.localScale.ToString("F4"),["mesh"]=AssetDatabase.GetAssetPath(r.GetComponent<MeshFilter>().sharedMesh)}))}));
  File.WriteAllText(Report+"/baseline-inspection.json",report.ToString());
  var r=BuildPipeline.BuildPlayer(new[]{ConRoiReviewV43.Scene},"Builds/Windows_v44_diagnostic/ConRoi.exe",BuildTarget.StandaloneWindows64,BuildOptions.Development);
  File.WriteAllText(Report+"/diagnostic-build.txt",r.summary.result.ToString());
 }
}
