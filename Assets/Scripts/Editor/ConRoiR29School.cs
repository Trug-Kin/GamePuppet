using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.Rendering;

// Import only: all changed vertex geometry is authored and saved in Blender first.
public static class ConRoiR29School {
 public const string Scene="Assets/Scenes/JourneyRepair_v48_r29.unity",Out="Docs/Reviews/journey_v48/r29",Art="Assets/Art/Environment/JourneyV48/SchoolR29";
 static string Hier(Transform t)=>t.parent?Hier(t.parent)+"/"+t.name:t.name;
 static Vector3 V(JToken t)=>new Vector3((float)t[0],(float)t[1],(float)t[2]);
 static JArray Vec(Vector3 v)=>new JArray(v.x,v.y,v.z);
 public static void Author(){
  Directory.CreateDirectory(Out);AssetDatabase.Refresh();
  if(File.Exists(Scene))throw new Exception("r29 already exists; use Import on the derivative, never overwrite checkpoint");
  if(!AssetDatabase.CopyAsset(ConRoiR28School.Scene,Scene))throw new Exception("Scene derivative copy failed");Import();
 }
 public static void Import(){
  EditorSceneManager.OpenScene(Scene);var s=UnityEngine.Object.FindAnyObjectByType<DieuPhoiHanhTrinh>();var root=s.schoolChapter.environment.transform.Find("R28_School");
  var ex=s.GetComponent<TraiNghiemTruongR28>();bool first=!ex.blenderSchoolR29;ex.blenderSchoolR29=true;
  var doc=JObject.Parse(File.ReadAllText(Art+"/SchoolR29.geometry.json"));
  var original=JObject.Parse(File.ReadAllText("Docs/Reviews/model_locations/unity-inventory.json"));
  var used=new HashSet<Transform>();var report=new JArray();
  var glass=AssetDatabase.LoadAssetAtPath<Material>(Art+"/Window_DustyGlass.mat");if(!glass){glass=new Material(Shader.Find("Universal Render Pipeline/Lit"));glass.color=new Color(.21f,.28f,.27f);glass.SetFloat("_Smoothness",.22f);AssetDatabase.CreateAsset(glass,Art+"/Window_DustyGlass.mat");}
  var authored=root.Find("R29_Authored");if(!authored){authored=new GameObject("R29_Authored").transform;authored.SetParent(root,false);}
  AssetDatabase.StartAssetEditing();try{foreach(var row in doc["objects"]){
   bool isNew=!string.IsNullOrEmpty((string)row["new_path"]);Transform t;
   if(isNew){string name=(string)row["name"];t=authored.Find(name);if(!t){t=new GameObject(name).transform;t.SetParent(authored,false);}t.position=new Vector3((float)row["world_matrix"][0][3],(float)row["world_matrix"][1][3],(float)row["world_matrix"][2][3]);t.rotation=Quaternion.identity;t.localScale=Vector3.one;}
   else {
    var src=original["objects"][(int)row["inventory_index"]];var expected=new Vector3((float)src["matrix_unity_world"][0][3],(float)src["matrix_unity_world"][1][3],(float)src["matrix_unity_world"][2][3]);
    if(!first&&!((string)row["hierarchy"]).Contains("/R28_School/"))expected+=new Vector3(-.75f,0,-.25f);
    t=s.schoolChapter.environment.GetComponentsInChildren<Transform>(true).Where(x=>!used.Contains(x)&&Hier(x)==(string)row["hierarchy"]).OrderBy(x=>Vector3.Distance(x.position,expected)).FirstOrDefault();
    if(!t||Vector3.Distance(t.position,expected)>.001f)throw new Exception("Missing exact original object: "+row["hierarchy"]);used.Add(t);
   }
   // Deserialize Blender export into a Unity mesh asset. No procedural geometry edits here.
   var mesh=new Mesh{name="R29_"+(string)row["name"]};mesh.vertices=row["vertices"].Select(V).ToArray();mesh.triangles=row["triangles"].SelectMany(x=>x.Select(v=>(int)v)).ToArray();mesh.uv=row["uv"].Select(x=>new Vector2((float)x[0],(float)x[1])).ToArray();mesh.RecalculateNormals();mesh.RecalculateBounds();
   string path=Art+"/"+(string)row["name"]+".asset";var old=AssetDatabase.LoadAssetAtPath<Mesh>(path);if(old){EditorUtility.CopySerialized(mesh,old);UnityEngine.Object.DestroyImmediate(mesh);mesh=old;}else AssetDatabase.CreateAsset(mesh,path);
   var mf=t.GetComponent<MeshFilter>();if(!mf)mf=t.gameObject.AddComponent<MeshFilter>();mf.sharedMesh=mesh;
   var rr=t.GetComponent<MeshRenderer>();if(!rr)rr=t.gameObject.AddComponent<MeshRenderer>();
   string material=(string)row["material"];
   if(material=="R29_Window_Dusty_Glass")rr.sharedMaterial=glass;
   else if(isNew){string name=material.Substring(material.IndexOf('_')+1);rr.sharedMaterial=AssetDatabase.LoadAssetAtPath<Material>(ConRoiR28School.Art+"/"+name+".mat");if(!rr.sharedMaterial)throw new Exception("Missing material "+material);}
   t.gameObject.layer=3;
   foreach(var mc in t.GetComponents<MeshCollider>())mc.sharedMesh=mesh;
   foreach(var bc in t.GetComponents<BoxCollider>()){bc.center=mesh.bounds.center;bc.size=mesh.bounds.size;}
   report.Add(new JObject{["blender_object"]=row["name"],["unity_hierarchy"]=Hier(t),["mesh_asset"]=path,["same_scene_object"]=!isNew,["vertices"]=mesh.vertexCount});
  }}finally{AssetDatabase.StopAssetEditing();}
  if(first){
   // Placement correction for readability; feet were already on the slab in r28.
   var offset=new Vector3(-.75f,0,-.25f);s.schoolChapter.puppet.transform.position+=offset;ex.leadShoulder.position+=offset;
   foreach(var text in root.GetComponentsInChildren<TextMesh>(true)){
    if(text.name=="Entry_Wayfinding"||text.name=="Stair_Wayfinding")text.transform.localScale*=.62f;
    else if(text.name=="Landing_Wayfinding"||text.name=="Corridor_Wayfinding"||text.name=="203_RoomPlate")text.transform.localScale*=.78f;
    else if(text.name=="203_Return_Text"||text.name=="Gate_Return_Text")text.transform.localScale*=.50f;
   }
   foreach(var l in root.GetComponentsInChildren<Light>(true)){
    if(l.name=="203_SoftCeiling"){l.intensity=1.15f;l.range=8;l.color=new Color(1,.94f,.84f);}
    if(l.name=="203_BoardFill"){l.intensity=.85f;l.range=6;l.color=new Color(1,.94f,.84f);}
   }
  }
  foreach(var text in root.GetComponentsInChildren<TextMesh>(true))if(text.name=="203_Return_Text"||text.name=="Gate_Return_Text")text.characterSize=.01485f;
  var bounce=root.Find("203_WindowBounce");if(!bounce){bounce=new GameObject("203_WindowBounce").transform;bounce.SetParent(root,false);bounce.position=ConRoiR28School.W(5.5f,5.7f,8.5f);var l=bounce.gameObject.AddComponent<Light>();l.type=LightType.Point;l.intensity=1.6f;l.range=5;l.color=new Color(.80f,.88f,1);l.shadows=LightShadows.None;}
  bounce.GetComponent<Light>().intensity=4.5f;
  // Stage the existing arm rig at the same rest pose used by the runtime solver.
  var shoulder=ex.leadShoulder.position;var wrist=shoulder+new Vector3(.04f,-.53f,-.08f);var elbow=Vector3.Lerp(shoulder,wrist,.51f)+new Vector3(.10f,-.06f,.04f);
  Segment(ex.leadUpper,shoulder,elbow);Segment(ex.leadFore,elbow,wrist);ex.leadHand.position=wrist;
  foreach(string matName in new[]{"Warm_Plaster","Terrazzo"}){
   string path=Art+"/"+matName+".mat";var m=AssetDatabase.LoadAssetAtPath<Material>(path);if(!m){m=new Material(AssetDatabase.LoadAssetAtPath<Material>(ConRoiR28School.Art+"/"+matName+".mat"));m.SetTextureScale("_BaseMap",matName=="Terrazzo"?new Vector2(8,8):new Vector2(4,2));AssetDatabase.CreateAsset(m,path);}
   foreach(var rr in root.GetComponentsInChildren<MeshRenderer>(true))if(rr.sharedMaterial&&rr.sharedMaterial.name==matName)rr.sharedMaterial=m;
  }
  Physics.SyncTransforms();EditorSceneManager.SaveScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene());AssetDatabase.SaveAssets();
  File.WriteAllText(Out+"/unity-source-map.json",new JObject{["source_blend"]=doc["source"],["canonical_export"]=Path.GetFullPath(Art+"/SchoolR29.geometry.json"),["interchange_fbx"]=Path.GetFullPath(Art+"/SchoolR29.fbx"),["objects"]=report,["source_note"]="Blender source authored before Unity import; canonical local-space JSON avoids FBX preset rounding and keeps original object references/pivots"}.ToString());
  Views();
 }
 public static void Validate(){
  EditorSceneManager.OpenScene(Scene);var source=JObject.Parse(File.ReadAllText(Art+"/SchoolR29.geometry.json"));var s=UnityEngine.Object.FindAnyObjectByType<DieuPhoiHanhTrinh>();var root=s.schoolChapter.environment.transform.Find("R28_School");float vertexError=0,matrixError=0,colliderError=0;int count=0;
  foreach(var row in source["objects"]){string path=Art+"/"+(string)row["name"]+".asset";var mf=s.schoolChapter.environment.GetComponentsInChildren<MeshFilter>(true).Single(x=>AssetDatabase.GetAssetPath(x.sharedMesh)==path);var mesh=mf.sharedMesh;var vertices=mesh.vertices;
   if(vertices.Length!=row["vertices"].Count())throw new Exception("Vertex count mismatch "+path);
   for(int i=0;i<vertices.Length;i++)vertexError=Mathf.Max(vertexError,Vector3.Distance(vertices[i],V(row["vertices"][i])));
   var world=mf.transform.localToWorldMatrix;for(int a=0;a<4;a++)for(int b=0;b<4;b++){float expected=(float)row["world_matrix"][a][b];if(b==3&&!string.IsNullOrEmpty((string)row["hierarchy"])&&!((string)row["hierarchy"]).Contains("/R28_School/"))expected+=a==0?-.75f:a==2?-.25f:0;matrixError=Mathf.Max(matrixError,Mathf.Abs(world[a,b]-expected));}
   foreach(var box in mf.GetComponents<BoxCollider>())colliderError=Mathf.Max(colliderError,Vector3.Distance(box.center,mesh.bounds.center),Vector3.Distance(box.size,mesh.bounds.size));
   foreach(var mc in mf.GetComponents<MeshCollider>())if(mc.sharedMesh!=mesh)throw new Exception("Stale collider "+path);count++;
  }
  bool pass=vertexError<.00001f&&matrixError<.0001f&&colliderError<.0001f;File.WriteAllText(Out+"/unity-import-verification.json",new JObject{["pass"]=pass,["objects"]=count,["max_local_vertex_error"]=vertexError,["max_world_matrix_error"]=matrixError,["max_box_collider_bounds_error"]=colliderError,["mesh_colliders_synchronized"]=true,["method"]="Reopen saved r29 scene; compare Blender canonical export with actual referenced mesh assets, object matrices and collider bounds"}.ToString());if(!pass)throw new Exception("Import validation failed");
 }
 static void Segment(Transform t,Vector3 a,Vector3 b){t.position=(a+b)*.5f;t.up=b-a;var scale=t.localScale;t.localScale=new Vector3(scale.x,(a-b).magnitude,scale.z);}
 public static void Views(){
  var poses=new[]{("exterior",new Vector3(-95,1.7f,-78),new Vector3(-95,5.5f,-43)),("courtyard",new Vector3(-106,1.7f,-68),new Vector3(-94,5.3f,-41)),("porch",new Vector3(-94,1.7f,-54),new Vector3(-95.5f,2.7f,-46)),("stairs",new Vector3(-101.5f,1.8f,-50.2f),new Vector3(-101.5f,4.5f,-40.5f)),("gallery",new Vector3(-98,5.25f,-42.9f),new Vector3(-89,5.25f,-39.4f)),("class203",new Vector3(-90.35f,5.3f,-38.8f),new Vector3(-87.1f,5,-32.5f)),("gd3",new Vector3(-87,5.25f,-36),new Vector3(-90,4.7f,-34.4f))};
  foreach(var version in new[]{("r28",ConRoiR28School.Scene),("r29",Scene)}){
   EditorSceneManager.OpenScene(version.Item2);var s=UnityEngine.Object.FindAnyObjectByType<DieuPhoiHanhTrinh>();s.hub.SetActive(false);s.lake.SetActive(false);s.market.SetActive(false);s.slopeChapter.environment.SetActive(false);s.schoolChapter.environment.SetActive(true);
   var pipe=s.GetComponent<HoSoSceneR26>().pipeline;GraphicsSettings.defaultRenderPipeline=pipe;QualitySettings.renderPipeline=pipe;RenderSettings.fog=false;Shader.SetGlobalVector("_MistProfile",Vector4.zero);RenderSettings.ambientMode=AmbientMode.Flat;RenderSettings.ambientLight=new Color(.57f,.60f,.61f);
   foreach(var l in UnityEngine.Object.FindObjectsByType<Light>(FindObjectsInactive.Include))if(l.type==LightType.Directional){l.intensity=1.15f;l.transform.rotation=Quaternion.Euler(48,-32,0);l.color=new Color(1,.94f,.84f);}
   // Identical comparison lighting, including fixtures, irrespective of production adjustments.
   foreach(var l in s.schoolChapter.environment.GetComponentsInChildren<Light>())if(l.type==LightType.Point){l.intensity=.75f;l.range=7;l.color=new Color(1,.94f,.84f);if(l.name=="203_WindowBounce")l.enabled=false;}
   var cam=new GameObject("R29_ComparisonCamera").AddComponent<Camera>();cam.fieldOfView=65;cam.farClipPlane=180;cam.nearClipPlane=.1f;cam.clearFlags=CameraClearFlags.SolidColor;cam.backgroundColor=new Color(.44f,.54f,.61f);var data=new JArray();
   foreach(var p in poses){cam.transform.position=p.Item2;cam.transform.LookAt(p.Item3);ConRoiR22Scenery.Capture(cam,Out+"/"+version.Item1+"/"+p.Item1+".png");data.Add(new JObject{["name"]=p.Item1,["eye"]=Vec(p.Item2),["target"]=Vec(p.Item3),["fov"]=65});}
   File.WriteAllText(Out+"/"+version.Item1+"/comparison-cameras.json",data.ToString());UnityEngine.Object.DestroyImmediate(cam.gameObject);
  }
 }
}
