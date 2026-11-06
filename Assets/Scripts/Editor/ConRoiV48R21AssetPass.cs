using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public static class ConRoiV48R21AssetPass
{
 const string Scene="Assets/Scenes/JourneyRepair_v48_r21.unity",Out="Docs/Reviews/journey_v48/r21/asset-pass";
 public static void Survey()
 {
  Directory.CreateDirectory(Out);var backup=Out+"/scene-before-asset-pass.unity";
  if(!File.Exists(backup))File.Copy(Scene,backup);
  EditorSceneManager.OpenScene(Scene);
  var rows=new JArray();
  foreach(var f in UnityEngine.Object.FindObjectsByType<MeshFilter>(FindObjectsInactive.Include))
  {
   var r=f.GetComponent<Renderer>();if(!r||!r.enabled||!f.gameObject.activeInHierarchy||!f.sharedMesh)continue;
   if(!r.sharedMaterials.Any(m=>m&&(m.name.Contains("OD35_Soil")||m.name.Contains("OD35_Concrete"))))continue;
   var mesh=f.sharedMesh;var vs=mesh.vertices.Select(p=>f.transform.TransformPoint(p)).ToArray();var ts=mesh.triangles;
   var parent=Enumerable.Range(0,vs.Length).ToArray();
   int Find(int x){while(parent[x]!=x){parent[x]=parent[parent[x]];x=parent[x];}return x;}
   void Union(int a,int b){parent[Find(a)]=Find(b);}
   var weld=new Dictionary<(int,int,int),int>();
   for(int i=0;i<vs.Length;i++){var p=vs[i];var key=(Mathf.RoundToInt(p.x*1000),Mathf.RoundToInt(p.y*1000),Mathf.RoundToInt(p.z*1000));if(weld.TryGetValue(key,out int j))Union(i,j);else weld[key]=i;}
   for(int i=0;i<ts.Length;i+=3){Union(ts[i],ts[i+1]);Union(ts[i],ts[i+2]);}
   var groups=new JArray();
   foreach(var group in Enumerable.Range(0,vs.Length).GroupBy(Find))
   {
    var ids=group.ToArray();var b=new Bounds(vs[ids[0]],Vector3.zero);foreach(int i in ids)b.Encapsulate(vs[i]);
    if(b.center.x<600||b.center.x>740||b.center.z<465||b.center.z>565)continue;
    JArray V(Vector3 v)=>new JArray(v.x,v.y,v.z);
    groups.Add(new JObject{["min"]=V(b.min),["max"]=V(b.max),["vertices"]=ids.Length,["indices"]=new JArray(ids)});
   }
   rows.Add(new JObject{["name"]=f.name,["asset"]=AssetDatabase.GetAssetPath(mesh),["material"]=r.sharedMaterial.name,["components"]=groups});
  }
  File.WriteAllText(Out+"/planter-components.json",rows.ToString());
  Debug.Log("R21_ASSET_SURVEY_COMPLETE");
 }
 public static void PlaceSample()
 {
  const string art="Assets/Art/Environment/JourneyV48/FlowersR21";
  EditorSceneManager.OpenScene(Scene);AssetDatabase.Refresh();
  var importer=(ModelImporter)AssetImporter.GetAtPath(art+"/FlowerModule.fbx");importer.importAnimation=false;importer.addCollider=false;importer.globalScale=1;importer.useFileScale=true;importer.SaveAndReimport();
  var mesh=AssetDatabase.LoadAssetAtPath<GameObject>(art+"/FlowerModule.fbx").GetComponentInChildren<MeshFilter>().sharedMesh;
  var source=JArray.Parse(File.ReadAllText(Out+"/planter-components.json"));
  var soilRow=source.Single(r=>(string)r["material"]=="OD35_Soil");
  var component=soilRow["components"].OrderBy(c=>Mathf.Abs(((float)c["min"][0]+(float)c["max"][0])*.5f-641)+Mathf.Abs(((float)c["min"][2]+(float)c["max"][2])*.5f-547)).First();
  var soil=UnityEngine.Object.FindObjectsByType<MeshFilter>(FindObjectsInactive.Include).Single(f=>f.name==(string)soilRow["name"]&&AssetDatabase.GetAssetPath(f.sharedMesh)==(string)soilRow["asset"]);
  var vs=soil.sharedMesh.vertices;var pts=component["indices"].Values<int>().Select(i=>soil.transform.TransformPoint(vs[i])).ToArray();float top=pts.Max(p=>p.y);
  var corners=pts.Where(p=>p.y>top-.003f).GroupBy(p=>(Mathf.RoundToInt(p.x*1000),Mathf.RoundToInt(p.z*1000))).Select(g=>g.First()).ToArray();
  var centre=corners.Aggregate(Vector3.zero,(a,b)=>a+b)/corners.Length;
  var sorted=corners.OrderBy(p=>Mathf.Atan2(p.z-centre.z,p.x-centre.x)).ToArray();
  var direction=Enumerable.Range(0,sorted.Length).Select(i=>sorted[(i+1)%sorted.Length]-sorted[i]).OrderByDescending(v=>v.sqrMagnitude).First().normalized;direction.y=0;direction.Normalize();
  var across=Vector3.Cross(Vector3.up,direction);float halfLength=pts.Max(p=>Mathf.Abs(Vector3.Dot(p-centre,direction))),halfWidth=pts.Max(p=>Mathf.Abs(Vector3.Dot(p-centre,across)));
  bool Inside(Vector3 p)=>Mathf.Abs(Vector3.Dot(p-centre,direction))<halfLength+.17f&&Mathf.Abs(Vector3.Dot(p-centre,across))<halfWidth+.17f&&p.y>top-.04f&&p.y<top+.8f;
  var removed=new JArray();
  foreach(var f in UnityEngine.Object.FindObjectsByType<MeshFilter>(FindObjectsInactive.Include))
  {
   var r=f.GetComponent<Renderer>();if(!r||!r.enabled||!r.sharedMaterial||!r.sharedMaterial.name.Contains("Flowers"))continue;
   var old=f.sharedMesh;var vertices=old.vertices;var triangles=old.triangles;var kept=new List<int>();
   for(int i=0;i<triangles.Length;i+=3){var p=f.transform.TransformPoint((vertices[triangles[i]]+vertices[triangles[i+1]]+vertices[triangles[i+2]])/3);if(!Inside(p)){kept.Add(triangles[i]);kept.Add(triangles[i+1]);kept.Add(triangles[i+2]);}}
   if(kept.Count==triangles.Length)continue;
   var copy=UnityEngine.Object.Instantiate(old);copy.name="R21_SampleRemoved_"+f.name+"_"+removed.Count;copy.triangles=kept.ToArray();copy.RecalculateBounds();var path=art+"/"+copy.name+".asset";
   if(AssetDatabase.LoadAssetAtPath<Mesh>(path))throw new Exception("Sample already placed; preserve prior derivative");
   AssetDatabase.CreateAsset(copy,path);f.sharedMesh=copy;removed.Add(new JObject{["object"]=f.name,["removed_triangles"]=(triangles.Length-kept.Count)/3,["source"]=AssetDatabase.GetAssetPath(old),["derived"]=path});
  }
  if(GameObject.Find("R21_RealPlanterFlowerSample"))throw new Exception("Sample root exists");
  var root=new GameObject("R21_RealPlanterFlowerSample");
  var colors=new[]{new Color(.075f,.19f,.035f),new Color(.32f,.16f,.025f),new Color(.48f,.048f,.052f)};
  var module=AssetDatabase.LoadAssetAtPath<GameObject>(art+"/FlowerModule.fbx").GetComponentInChildren<Renderer>();
  Material Make(string name,Color color){var p=art+"/"+name+".mat";var m=AssetDatabase.LoadAssetAtPath<Material>(p);if(!m){m=new Material(Shader.Find("Universal Render Pipeline/Lit"));AssetDatabase.CreateAsset(m,p);}m.SetColor("_BaseColor",color);m.SetFloat("_Smoothness",.12f);m.enableInstancing=true;return m;}
  var green=Make("Foliage",colors[0]);var core=Make("Centre",colors[1]);var red=Make("PetalRed",colors[2]);var yellow=Make("PetalYellow",new Color(.72f,.47f,.07f));
  int count=0;int columns=Mathf.CeilToInt((halfLength*2-.26f)/.32f);var objects=new JArray();
  for(int row=0;row<2;row++)for(int i=0;i<columns;i++)
  {
   float along=Mathf.Lerp(-halfLength+.14f,halfLength-.14f,(i+.5f)/columns);float side=(row==0?-1:1)*halfWidth*.42f;
   var p=centre+direction*along+across*side; p.y=top-.012f;
   var g=new GameObject("FlowerModule_"+count);g.transform.SetParent(root.transform);g.transform.position=p;g.transform.rotation=Quaternion.Euler(0,(i*137+row*71)%360,0);g.transform.localScale=Vector3.one*(.92f+.04f*(i%4));
   g.AddComponent<MeshFilter>().sharedMesh=mesh;var r=g.AddComponent<MeshRenderer>();var petals=(i%4==0)?yellow:red;
   r.sharedMaterials=module.sharedMaterials.Select(m=>m.name.Contains("Foliage")?green:m.name.Contains("Centre")?core:petals).ToArray();r.shadowCastingMode=ShadowCastingMode.Off;
   objects.Add(new JObject{["position"]=new JArray(p.x,p.y,p.z)});count++;
  }
  EditorSceneManager.SaveScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene());AssetDatabase.SaveAssets();
  File.WriteAllText(Out+"/real-planter-sample.json",new JObject{["status"]="SINGLE_REAL_PLANTER_REQUIRES_VISUAL_REVIEW_BEFORE_PROPAGATION",["centre"]=new JArray(centre.x,centre.y,centre.z),["axis"]=new JArray(direction.x,direction.y,direction.z),["length_m"]=halfLength*2,["width_m"]=halfWidth*2,["modules"]=count,["triangles"]=count*mesh.triangles.Length/3,["shared_mesh"]=AssetDatabase.GetAssetPath(mesh),["shared_materials"]=4,["old_flower_triangles"]=removed,["instances"]=objects}.ToString());
  PreviewSample();
 }
 public static void PreviewSample()
 {
  EditorSceneManager.OpenScene(Scene);var story=UnityEngine.Object.FindAnyObjectByType<DieuPhoiHanhTrinh>();story.hub.SetActive(false);story.lake.SetActive(true);story.market.SetActive(true);foreach(var g in story.GetComponent<CanhQuanChoV48>().scenery)g.SetActive(true);
  var root=GameObject.Find("R21_RealPlanterFlowerSample");var all=root.GetComponentsInChildren<Renderer>();var bounds=all[0].bounds;foreach(var r in all)bounds.Encapsulate(r.bounds);
  var eye=UnityEngine.Object.Instantiate(story.player.GetComponentInChildren<Camera>());eye.gameObject.SetActive(true);eye.enabled=false;eye.fieldOfView=60;
  var views=new[]{("flower-real-overall",bounds.center+new Vector3(4,1.1f,-5),bounds.center),("flower-real-player",bounds.center+new Vector3(1.5f,1.22f,-2),bounds.center),("flower-real-close",all[5].bounds.center+new Vector3(.6f,.65f,-.7f),all[5].bounds.center)};
  bool fog=RenderSettings.fog;RenderSettings.fog=false;var rows=new JArray();
  foreach(var v in views){eye.transform.SetPositionAndRotation(v.Item2,Quaternion.LookRotation(v.Item3-v.Item2));var rt=RenderTexture.GetTemporary(1400,900,24);var tex=new Texture2D(1400,900,TextureFormat.RGB24,false);for(int i=0;i<3;i++)RenderPipeline.SubmitRenderRequest(eye,new UniversalRenderPipeline.SingleCameraRequest{destination=rt});var old=RenderTexture.active;RenderTexture.active=rt;tex.ReadPixels(new Rect(0,0,1400,900),0,0);tex.Apply();RenderTexture.active=old;File.WriteAllBytes(Out+"/"+v.Item1+".png",tex.EncodeToPNG());RenderTexture.ReleaseTemporary(rt);UnityEngine.Object.DestroyImmediate(tex);rows.Add(new JObject{["image"]=v.Item1+".png",["eye"]=new JArray(v.Item2.x,v.Item2.y,v.Item2.z)});}
  File.WriteAllText(Out+"/sample-preview-method.json",new JObject{["method"]="Editor URP actual derived r21 scene, fog disabled, not foreground gameplay",["views"]=rows}.ToString());RenderSettings.fog=fog;EditorSceneManager.OpenScene(Scene);
 }
 public static void FixModuleScale()
 {
  EditorSceneManager.OpenScene(Scene);const string art="Assets/Art/Environment/JourneyV48/FlowersR21";
  var imported=AssetDatabase.LoadAssetAtPath<GameObject>(art+"/FlowerModule.fbx").GetComponentInChildren<MeshFilter>();var original=imported.sharedMesh;
  var mesh=UnityEngine.Object.Instantiate(original);mesh.name="R21_FlowerModule_Metres";var matrix=imported.transform.localToWorldMatrix;
  mesh.vertices=original.vertices.Select(matrix.MultiplyPoint3x4).ToArray();mesh.normals=original.normals.Select(n=>matrix.inverse.transpose.MultiplyVector(n).normalized).ToArray();mesh.RecalculateBounds();
  if(mesh.bounds.size.y<.20f||mesh.bounds.size.y>.35f)throw new Exception("Baked module height unexpected: "+mesh.bounds.size);
  AssetDatabase.CreateAsset(mesh,art+"/FlowerModuleMetres.asset");
  foreach(var f in GameObject.Find("R21_RealPlanterFlowerSample").GetComponentsInChildren<MeshFilter>())f.sharedMesh=mesh;
  EditorSceneManager.SaveScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene());AssetDatabase.SaveAssets();
  File.WriteAllText(Out+"/flower-import-scale.json",new JObject{["fbx_mesh_bounds"]=original.bounds.size.ToString("F6"),["imported_transform"]=matrix.ToString(),["baked_metre_bounds"]=mesh.bounds.size.ToString("F6"),["reason"]="Direct sharedMesh reuse must include FBX child transform; trial prefab instantiation had retained it"}.ToString());PreviewSample();
 }
 public static void BuildReview()
 {
  ConRoiV48R21GeometryAudit.Audit();File.Copy("Docs/Reviews/journey_v48/r21/geometry-contact-audit.json",Out+"/geometry-contact-audit.json",true);
  EditorSceneManager.OpenScene(Scene);var version=PlayerSettings.bundleVersion;
  try {
   PlayerSettings.bundleVersion="0.48.21";
   var report=BuildPipeline.BuildPlayer(new[]{Scene},"Builds/Windows_v48_r21_assetpass/ConRoi.exe",BuildTarget.StandaloneWindows64,BuildOptions.Development);
   File.WriteAllText(Out+"/build.json",new JObject{["status"]="INTERNAL_REVIEW_NOT_ACCEPTANCE",["result"]=report.summary.result.ToString(),["errors"]=report.summary.totalErrors,["warnings"]=report.summary.totalWarnings}.ToString());
   if(report.summary.result!=UnityEditor.Build.Reporting.BuildResult.Succeeded)throw new Exception("Asset review build failed");
  } finally {PlayerSettings.bundleVersion=version;AssetDatabase.SaveAssets();}
 }
 public static void SurveyEnvelope()
 {
  EditorSceneManager.OpenScene(Scene);var story=UnityEngine.Object.FindAnyObjectByType<DieuPhoiHanhTrinh>();story.hub.SetActive(false);story.lake.SetActive(true);story.market.SetActive(true);foreach(var g in story.GetComponent<CanhQuanChoV48>().scenery)g.SetActive(true);
  var rows=new JArray();
  foreach(var f in GameObject.Find("R20_Whole_Source_Export").GetComponentsInChildren<MeshFilter>().Where(f=>f.name=="Surface_142"||f.name=="Surface_176"||f.name=="Surface_153"))
  {
   var vs=f.sharedMesh.vertices.Select(p=>f.transform.TransformPoint(p)).ToArray();rows.Add(new JObject{["name"]=f.name,["vertices"]=new JArray(vs.Select(p=>new JArray(p.x,p.y,p.z))),["triangles"]=new JArray(f.sharedMesh.triangles)});
  }
  var points=new JArray();foreach(var i in UnityEngine.Object.FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include).Where(b=>b is IPlayerInteractable))if(i.transform.position.x>450)points.Add(new JObject{["name"]=i.name,["position"]=new JArray(i.transform.position.x,i.transform.position.y,i.transform.position.z)});
  File.WriteAllText(Out+"/envelope-survey.json",new JObject{["surfaces"]=rows,["protected_interactions"]=points}.ToString());
 }
 public static void AuditBoundaries()
 {
  string audit="Docs/Reviews/journey_v48/r21/static-reachability-audit.json";
  if(!File.Exists(Out+"/reachability-before.json"))File.Copy(audit,Out+"/reachability-before.json");
  ConRoiV48R21Reachability.Audit();File.Copy(audit,Out+"/reachability-after.json",true);
 }
}
