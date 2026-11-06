using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;

public static class ConRoiR24Reference
{
 public const string Out="Docs/Reviews/journey_v48/r24", Scene="Assets/Scenes/JourneyRepair_v48_r24.unity";
 const string Art="Assets/Art/Environment/JourneyV48/ReferenceR24";
 static JArray V(Vector3 p)=>new JArray(p.x,p.y,p.z);
 static string PathOf(Transform t)=>t.parent?PathOf(t.parent)+"/"+t.name:t.name;
 public static void Author(){Survey();Assemble();}
 public static void Survey(){
  Directory.CreateDirectory(Out);
  EditorSceneManager.OpenScene(ConRoiR23Opening.Scene);
  var s=UnityEngine.Object.FindAnyObjectByType<DieuPhoiHanhTrinh>();
  var roots=UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
  File.WriteAllText(Out+"/survey.json",new JObject{
   ["spawn"]=V(s.lakeSpawn.position),["school_spawn"]=V(s.schoolChapter.spawn.position),["slope_spawn"]=V(s.slopeChapter.spawn.position),
   ["renderers"]=new JArray(roots.SelectMany(g=>g.GetComponentsInChildren<MeshRenderer>(true)).Select(r=>new JObject{["path"]=PathOf(r.transform),["min"]=V(r.bounds.min),["max"]=V(r.bounds.max),["material"]=r.sharedMaterial?r.sharedMaterial.name:"",["mesh"]=r.GetComponent<MeshFilter>()?AssetDatabase.GetAssetPath(r.GetComponent<MeshFilter>().sharedMesh):""}))}.ToString());
  Render(s,"before");
 }
 static void Render(DieuPhoiHanhTrinh s,string folder){
  s.hub.SetActive(false);s.lake.SetActive(true);s.market.SetActive(true);s.schoolChapter.environment.SetActive(true);s.slopeChapter.environment.SetActive(true);
  var pipeline=s.GetComponent<HoSoSceneR24>()?s.GetComponent<HoSoSceneR24>().pipeline:s.GetComponent<HoSoSceneR23>().pipeline;
  GraphicsSettings.defaultRenderPipeline=pipeline;QualitySettings.renderPipeline=pipeline;
  Shader.SetGlobalVector("_MistProfile",Vector4.zero);RenderSettings.fog=false;RenderSettings.ambientMode=AmbientMode.Flat;RenderSettings.ambientLight=new Color(.7f,.7f,.7f);RenderSettings.ambientIntensity=1;
  foreach(var light in UnityEngine.Object.FindObjectsByType<Light>(FindObjectsInactive.Include))if(light.type==LightType.Directional){light.intensity=1.2f;light.transform.rotation=Quaternion.Euler(45,-30,0);light.color=Color.white;}
  var go=new GameObject("R24_Review_Camera");var cam=go.AddComponent<Camera>();cam.fieldOfView=60;cam.farClipPlane=1300;cam.nearClipPlane=.1f;cam.clearFlags=CameraClearFlags.SolidColor;cam.backgroundColor=new Color(.55f,.7f,.85f);
  var views=new[]{
   ("bridge",new Vector3(619,2.8f,554),new Vector3(661,1.5f,518)),
   ("roundabout",new Vector3(691,2.8f,480),new Vector3(715,1.7f,459)),
   ("shore",new Vector3(649,3.7f,500),new Vector3(654,.3f,506)),
   ("market-route",new Vector3(749,5.8f,365),new Vector3(791,16,154)),
   ("school",new Vector3(-105,1.65f,-52),new Vector3(-95,5,-40)),
   ("school-context",new Vector3(-95,1.65f,-85),new Vector3(-95,6,-40)),
   ("slope",s.slopeChapter.spawn.position+Vector3.up*.75f,new Vector3(-180,5,-12))};
  var record=new JArray();foreach(var v in views){go.transform.position=v.Item2;go.transform.LookAt(v.Item3);ConRoiR22Scenery.Capture(cam,Out+"/"+folder+"/"+v.Item1+".png");record.Add(new JObject{["name"]=v.Item1,["eye"]=V(v.Item2),["target"]=V(v.Item3),["fov"]=60});}
  File.WriteAllText(Out+"/"+folder+"/cameras.json",record.ToString());UnityEngine.Object.DestroyImmediate(go);
 }
 public static void Assemble(){
  if(File.Exists(Scene))throw new Exception("Retain first r24 assembly; use RefreshViews for read-only review");
  if(!AssetDatabase.CopyAsset(ConRoiR23Opening.Scene,Scene))throw new Exception("Copy failed");
  EditorSceneManager.OpenScene(Scene);var s=UnityEngine.Object.FindAnyObjectByType<DieuPhoiHanhTrinh>();
  var old=s.GetComponent<HoSoSceneR23>();var pipeline=old.pipeline;UnityEngine.Object.DestroyImmediate(old);
  var profile=s.gameObject.AddComponent<HoSoSceneR24>();profile.story=s;profile.pipeline=pipeline;
  var sr=s.schoolChapter.environment.transform;var sl=s.slopeChapter.environment.transform;
  var changes=new JArray();
  // Ground floor and all story objects retain their original poses. Remove only exterior proxy blocks replaced by the source module.
  foreach(Transform t in sr){if(new[]{"Adjacent_Campus_Wing","Campus_Window"}.Contains(t.name)){t.gameObject.SetActive(false);changes.Add("school proxy hidden: "+t.name);}}
  var hidden=new[]{"Road","Slope_Understructure","Visible_Concrete_Curb","DaLat_House_Plaster","House_Retaining_Plinth","House_Window_Recess","Pitched_Roof","Clay_Roof_Course","Window_Shutter_Slat","Window_Frame","Window_Glass","Window_Rain_Awning","Weathered_Skirting","Rain_Gutter","Downpipe","Concrete_Power_Pole","Sagging_Service_Cable","Hillside","R21_Slope_House_Frontage_Fences"};
  foreach(Transform t in sl){
   var before=t.position;t.position=HoSoSceneR24.MapSlope(before);
   if(hidden.Contains(t.name))t.gameObject.SetActive(false);
   // Non-interactive crest architecture follows the same horizontal compression.
   if(t.name=="V43_DaLat_Crest_Join")t.localScale=new Vector3(t.localScale.x*.45f,t.localScale.y,t.localScale.z);
   changes.Add(new JObject{["slope_object"]=t.name,["before"]=V(before),["after"]=V(t.position),["active"]=t.gameObject.activeSelf});
  }
  var created=BuildModules(sr,sl);
  var sign=new GameObject("R24_MainSchoolName");sign.transform.SetParent(sr);sign.transform.position=new Vector3(-95,3.15f,-45.44f);sign.transform.rotation=Quaternion.identity;
  var text=sign.AddComponent<TextMesh>();text.text="TRƯỜNG ĐÀ LẠT";text.fontSize=64;text.characterSize=.052f;text.anchor=TextAnchor.MiddleCenter;text.color=new Color(.9f,.87f,.71f);sign.GetComponent<Renderer>().sharedMaterial=AssetDatabase.LoadAssetAtPath<Material>("Assets/Art/Environment/SchoolV38/WorldText.mat");
  // Keep bridge walking geometry and all rail-contact objects. Tone the exposed return-bank sides as a planted bank, as seen in B01/B02.
  var bankChanges=new JArray();
  foreach(var r in UnityEngine.Object.FindObjectsByType<MeshRenderer>(FindObjectsInactive.Include).Where(r=>r.name=="Lake_Bank_Return_Walk"||r.name=="Garden_Bank_Return_Walk")){
   var material=new Material(r.sharedMaterial);material.name="R24_PlantedBank_Return";material.color=new Color(.30f,.37f,.20f);string path=Art+"/"+r.name+"_Bank.mat";AssetDatabase.CreateAsset(material,path);r.sharedMaterial=material;bankChanges.Add(PathOf(r.transform));
  }
  Physics.SyncTransforms();s.hub.SetActive(true);s.lake.SetActive(false);s.market.SetActive(false);s.schoolChapter.environment.SetActive(false);s.slopeChapter.environment.SetActive(false);
  EditorSceneManager.SaveScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene());AssetDatabase.SaveAssets();
  File.WriteAllText(Out+"/assembly.json",new JObject{["scene"]=Scene,["changes"]=changes,["meshes"]=created,["bank_material_changes"]=bankChanges,["spawn"]=V(s.lakeSpawn.position),["slope_grade_game_only"]=.20f,["school_interior"]="retained; no photographic interior claim"}.ToString());
  Render(s,"after");
 }
 static JArray BuildModules(Transform sr,Transform sl){
  var rows=JArray.Parse(File.ReadAllText(Art+"/Geometry.json"));var created=new JArray();
  foreach(string module in new[]{"school","slope"}){
   var root=new GameObject("R24_Reference_"+module);root.transform.SetParent(module=="school"?sr:sl);root.transform.position=module=="school"?new Vector3(-95,0,-45):new Vector3(-180,0,-60);
   var selected=rows.Where(r=>(string)r["module"]==module);
   foreach(var batch in selected.GroupBy(r=>(string)r["material"]+"_"+(bool)r["collision"])){
    var verts=new System.Collections.Generic.List<Vector3>();var triangles=new System.Collections.Generic.List<int>();var first=batch.First();
    foreach(var row in batch){int offset=verts.Count;verts.AddRange(row["vertices"].Select(v=>new Vector3((float)v[0],(float)v[1],(float)v[2])));triangles.AddRange(row["triangles"].Select(t=>(int)t+offset));}
    var mesh=new Mesh{name=module+"_"+batch.Key};mesh.indexFormat=UnityEngine.Rendering.IndexFormat.UInt32;mesh.SetVertices(verts);mesh.SetTriangles(triangles,0);mesh.RecalculateNormals();mesh.RecalculateBounds();mesh.uv=verts.Select(p=>new Vector2(p.x*.25f,(p.y+p.z)*.25f)).ToArray();
    string path=Art+"/"+mesh.name+".asset";var existing=AssetDatabase.LoadAssetAtPath<Mesh>(path);if(existing){EditorUtility.CopySerialized(mesh,existing);mesh=existing;}else AssetDatabase.CreateAsset(mesh,path);
    var g=new GameObject(mesh.name);g.layer=3;g.transform.SetParent(root.transform,false);g.AddComponent<MeshFilter>().sharedMesh=mesh;
    string mp=Art+"/"+(string)first["material"]+".mat";var material=AssetDatabase.LoadAssetAtPath<Material>(mp);
    if(!material){material=new Material(Shader.Find("Universal Render Pipeline/Lit"));var c=first["color"];material.color=new Color((float)c[0],(float)c[1],(float)c[2]);material.SetFloat("_Smoothness",.12f);AssetDatabase.CreateAsset(material,mp);}
    g.AddComponent<MeshRenderer>().sharedMaterial=material;if((bool)first["collision"])g.AddComponent<MeshCollider>().sharedMesh=mesh;
    created.Add(new JObject{["mesh"]=path,["vertices"]=verts.Count,["triangles"]=triangles.Count/3,["collider_same_mesh"]=(bool)first["collision"]});
   }
  }
  return created;
 }
 public static void Refine(){
  EditorSceneManager.OpenScene(Scene);var s=UnityEngine.Object.FindAnyObjectByType<DieuPhoiHanhTrinh>();var sr=s.schoolChapter.environment.transform;var sl=s.slopeChapter.environment.transform;
  foreach(var root in new[]{sr,sl})foreach(var t in root.GetComponentsInChildren<Transform>(true).Where(t=>t.name.StartsWith("R24_Reference_")).ToArray())UnityEngine.Object.DestroyImmediate(t.gameObject);
  BuildModules(sr,sl);
  var keep=new[]{"AnhTung_Cousin","ConRoi_Minh_GD4_FinalOnly","Last_Door","Distant_Wooden_Spool","Tung_Packed_Dusty_Suitcase","Tung_Door_Frame","Tung_Door_Panel","Tung_Jacket_Button","Tung_Neck","Top_Garden_Wall","Final_Entrance_Lintel","Final_Entrance_Pier","VÉ XE CŨ","ĐỊA CHỈ VIẾT TAY","Slope_Lettering"};
  foreach(Transform t in sl){
   if(t.name.StartsWith("R24_")||t==s.slopeChapter.spawn||keep.Contains(t.name))continue;
   t.gameObject.SetActive(false);
  }
  foreach(var r in sr.GetComponentsInChildren<MeshRenderer>(true).Where(r=>r.name=="Covered_Link_Roof"||r.name=="Covered_Link_Cornice"||r.name.StartsWith("SchoolArchitecture_")))r.enabled=false;
  // The old batched roof is decorative; retain all gameplay structure and colliders.
  var architecture=sr.Find("V43_School_Architecture");if(architecture)foreach(var r in architecture.GetComponentsInChildren<MeshRenderer>(true))r.enabled=false;
  var spool=s.GetComponent<CuonChiDocR24>()??s.gameObject.AddComponent<CuonChiDocR24>();spool.chapter=s.slopeChapter;
  // Keep pavement tops; only slope/vertical faces use the planted-bank material.
  var bankAudit=new JArray();
  foreach(var r in UnityEngine.Object.FindObjectsByType<MeshRenderer>(FindObjectsInactive.Include).Where(r=>r.name=="Lake_Bank_Return_Walk"||r.name=="Garden_Bank_Return_Walk")){
   var f=r.GetComponent<MeshFilter>();var mesh=UnityEngine.Object.Instantiate(f.sharedMesh);mesh.name=r.name+"_R24_SideOnly";
   var top=new System.Collections.Generic.List<int>();var side=new System.Collections.Generic.List<int>();var v=mesh.vertices;var tt=mesh.triangles;
   for(int i=0;i<tt.Length;i+=3){var normal=Vector3.Cross(f.transform.TransformVector(v[tt[i+1]]-v[tt[i]]),f.transform.TransformVector(v[tt[i+2]]-v[tt[i]])).normalized;var dst=normal.y>.7f?top:side;dst.AddRange(new[]{tt[i],tt[i+1],tt[i+2]});}
   mesh.subMeshCount=2;mesh.SetTriangles(top,0);mesh.SetTriangles(side,1);string path=Art+"/"+mesh.name+".asset";var old=AssetDatabase.LoadAssetAtPath<Mesh>(path);if(old){EditorUtility.CopySerialized(mesh,old);mesh=old;}else AssetDatabase.CreateAsset(mesh,path);f.sharedMesh=mesh;
   var paving=AssetDatabase.LoadAssetAtPath<Material>("Assets/Art/Environment/JourneyV48/RecoveryFixR21/Bank_Gravel.mat")??r.sharedMaterial;
   var planted=AssetDatabase.LoadAssetAtPath<Material>(Art+"/"+r.name+"_Bank.mat");r.sharedMaterials=new[]{paving,planted};bankAudit.Add(new JObject{["path"]=PathOf(r.transform),["pavement_triangles"]=top.Count/3,["bank_triangles"]=side.Count/3,["geometry_positions_unchanged"]=true});
  }
  File.WriteAllText(Out+"/bank-refinement.json",bankAudit.ToString());
  EditorSceneManager.SaveScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene());AssetDatabase.SaveAssets();
  Render(s,"after");
  EditorSceneManager.OpenScene(ConRoiR23Opening.Scene);Render(UnityEngine.Object.FindAnyObjectByType<DieuPhoiHanhTrinh>(),"before");
 }
 public static void RefreshViews(){EditorSceneManager.OpenScene(Scene);Render(UnityEngine.Object.FindAnyObjectByType<DieuPhoiHanhTrinh>(),"after");}
 public static void CheckGeometry(DieuPhoiHanhTrinh s){
  var sample=new JArray();bool pass=true;
  for(int row=0;row<=90;row++){
   float z=row*.5f,c=Mathf.Clamp01((z-18)/12)*2;
   foreach(float dx in new[]{-.5f,.4f,1.2f}){
    var at=new Vector3(-180+c+dx,12,-60+z);var hits=Physics.RaycastAll(at,Vector3.down,16,8,QueryTriggerInteraction.Ignore).Where(h=>h.transform.name.StartsWith("slope_")).OrderBy(h=>h.distance).ToArray();
    bool found=hits.Length>0;float expected=Mathf.Min(z,30)*.2f;bool good=found&&Mathf.Abs(hits[0].point.y-expected)<.025f&&hits[0].normal.y>.9f;pass&=good;
    sample.Add(new JObject{["z"]=z,["x"]=dx,["height"]=found?(JToken)hits[0].point.y:null,["pass"]=good});
   }
  }
  var name=s.schoolChapter.environment.GetComponentsInChildren<TextMesh>(true).Where(t=>t.name=="R24_MainSchoolName").Single().text;
  var meshes=s.schoolChapter.environment.GetComponentsInChildren<MeshFilter>(true).Concat(s.slopeChapter.environment.GetComponentsInChildren<MeshFilter>(true)).Where(f=>AssetDatabase.GetAssetPath(f.sharedMesh).Contains("ReferenceR24"));
  bool meshPass=meshes.All(f=>f.sharedMesh.vertexCount>0&&f.sharedMesh.uv.Length==f.sharedMesh.vertexCount&&f.sharedMesh.normals.All(n=>n.sqrMagnitude>.9f));
  File.WriteAllText(Out+"/geometry-validation.json",new JObject{["pass"]=pass&&meshPass&&name=="TRƯỜNG ĐÀ LẠT",["ramp_samples"]=sample,["all_new_meshes_have_uv_normals"]=meshPass,["main_school_sign"]=name,["school_story_preserved"]=s.schoolChapter.ghost.name.Contains("CoMai")&&s.schoolChapter.puppet.giaiDoanHienTai==DieuKhienConRoi.GiaiDoan.GD3,["lake_spawn_unchanged"]=Vector3.Distance(s.lakeSpawn.position,new Vector3(656.05f,2.0521f,514.35f))<.001f,["scope"]="Dense vertical ramp support samples; does not certify native input or whole scene collision."}.ToString());
  if(!pass||!meshPass)throw new Exception("R24 geometry checks failed");
 }
}
