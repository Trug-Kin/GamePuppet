using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class ConRoiSchoolV37
{
 public const string ScenePath="Assets/Scenes/SchoolContinuation_v37.unity";
 const string Folder="Assets/Art/Environment/SchoolV37";
 static Vector3 origin=new Vector3(-95,0,-45);
 static Transform parent;
 static GameObject source;
 static Material plaster,wood,stone,metal;
 static Bounds ModelBounds(Transform model)
 {
  var renderers=model.GetComponentsInChildren<Renderer>(true);var bounds=renderers[0].bounds;
  foreach(var renderer in renderers)bounds.Encapsulate(renderer.bounds);return bounds;
 }
 static void FitModel(Transform model,Vector3 point,float height)
 {
  model.rotation=Quaternion.Euler(-90,0,0);var bounds=ModelBounds(model);model.localScale*=height/bounds.size.y;
  bounds=ModelBounds(model);model.position+=point-new Vector3(bounds.center.x,bounds.min.y,bounds.center.z);
 }
 static void BindMai(ChuongTruongHocV37 chapter)
 {
  foreach(var collider in chapter.ghost.GetComponentsInChildren<Collider>(true))UnityEngine.Object.DestroyImmediate(collider);
  foreach(var interaction in chapter.ghost.GetComponentsInChildren<TuongTacTruongV37>(true))UnityEngine.Object.DestroyImmediate(interaction);
  var target=parent.Find("Mai_Interaction");var proxy=target?target.gameObject:new GameObject("Mai_Interaction");proxy.transform.SetParent(parent);proxy.transform.position=origin+new Vector3(2.5f,.875f,10);
  var box=proxy.GetComponent<BoxCollider>();if(!box)box=proxy.AddComponent<BoxCollider>();box.size=new Vector3(.65f,1.75f,.65f);
  var talk=proxy.GetComponent<TuongTacTruongV37>();if(!talk)talk=proxy.AddComponent<TuongTacTruongV37>();talk.chapter=chapter;
 }
 static void Garden()
 {
  if(parent.Find("School_Garden_Ground"))return;
  var green=Mat("Garden_Foliage",new Color(.12f,.2f,.1f));var earth=Mat("Garden_Earth",new Color(.27f,.3f,.2f));
  var facade=Mat("Campus_Facade",new Color(.47f,.46f,.39f));var frame=Mat("Campus_Window",new Color(.09f,.14f,.16f));
  Box("School_Garden_Ground",new Vector3(0,-.35f,0),new Vector3(80,.3f,80),earth);
  Box("Adjacent_Campus_Wing",new Vector3(26,5,7),new Vector3(8,10,52),facade);
  for(int z=-14;z<=28;z+=6)for(int floor=0;floor<3;floor++)Box("Campus_Window",new Vector3(21.98f,1.8f+floor*3,z),new Vector3(.04f,1.4f,2.2f),frame,false);
  for(int z=-10;z<=26;z+=4)
  {
   Box("Garden_Trunk",new Vector3(15,.8f,z),new Vector3(.2f,1.6f,.2f),Mat("Garden_Bark",new Color(.23f,.18f,.12f)),false);
   for(int tier=0;tier<4;tier++)Box("Garden_Cypress_Crown",new Vector3(15,1.8f+tier*.7f,z),new Vector3(2.2f-tier*.4f,1.3f,2.2f-tier*.4f),green,false);
  }
 }
 public static void RepairModels()
 {
  if(UnityEngine.SceneManagement.SceneManager.GetActiveScene().path!=ScenePath)EditorSceneManager.OpenScene(ScenePath);
  var chapter=UnityEngine.Object.FindAnyObjectByType<ChuongTruongHocV37>();parent=chapter.environment.transform;
  FitModel(chapter.ghost,origin+new Vector3(2.5f,0,10),1.75f);
  FitModel(chapter.puppet.transform,origin+new Vector3(0,0,8),3.5f);
  BindMai(chapter);ConRoiSchoolFurnitureV37.Refine(chapter);chapter.chair.rotation=Quaternion.Euler(0,-90,0);Garden();
  var marketMesh=AssetDatabase.LoadAssetAtPath<Mesh>("Assets/Art/Environment/StabilizationV36/MarketPosts_Outward.asset");
  var posts=UnityEngine.Object.FindObjectsByType<MeshCollider>(FindObjectsInactive.Include,FindObjectsSortMode.None).First(c=>c.name=="Context_142");posts.sharedMesh=marketMesh;posts.GetComponent<MeshFilter>().sharedMesh=marketMesh;
  EditorSceneManager.SaveScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene());Inspect();
 }
 public static void Inspect()
 {
  if(UnityEngine.SceneManagement.SceneManager.GetActiveScene().path!=ScenePath)EditorSceneManager.OpenScene(ScenePath);
  var chapter=UnityEngine.Object.FindAnyObjectByType<ChuongTruongHocV37>();
  var nodes=new JArray();
  foreach(var t in new[]{chapter.ghost,chapter.face.ghostHead,chapter.chair,chapter.puppet.transform,chapter.puppet.gesturePart})
  {
   var bounds=new JArray(t.GetComponentsInChildren<Renderer>(true).Select(r=>(JToken)new JObject{["name"]=r.name,["center"]=r.bounds.center.ToString("F4"),["size"]=r.bounds.size.ToString("F4")}));
   nodes.Add(new JObject{["name"]=t.name,["position"]=t.position.ToString("F4"),["rotation"]=t.eulerAngles.ToString("F2"),["bounds"]=bounds});
  }
  File.WriteAllText("Docs/Reviews/school_v37/geometry.json",nodes.ToString());
  var roots=UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
  string postsPath=AssetDatabase.GetAssetPath(UnityEngine.Object.FindObjectsByType<MeshCollider>(FindObjectsInactive.Include,FindObjectsSortMode.None).First(c=>c.name=="Context_142").sharedMesh);
  File.WriteAllText("Docs/Reviews/school_v37/scene-integrity.json",new JObject{["scene"]=ScenePath,["missing_scripts"]=roots.Sum(r=>r.GetComponentsInChildren<Transform>(true).Sum(t=>GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(t.gameObject))),["chapter_attached"]=chapter.story.schoolChapter==chapter,["school_ambience_present"]=chapter.tick!=null,["market_posts_asset"]=postsPath,["market_winding_repair_shared"]=postsPath=="Assets/Art/Environment/StabilizationV36/MarketPosts_Outward.asset"}.ToString());
 }
 static Material Mat(string name,Color color)
 {
  string path=Folder+"/"+name+".mat";var mat=AssetDatabase.LoadAssetAtPath<Material>(path);if(!mat){mat=new Material(Shader.Find("Universal Render Pipeline/Lit"));AssetDatabase.CreateAsset(mat,path);}mat.color=color;mat.SetFloat("_Smoothness",.18f);return mat;
 }
 static GameObject Box(string name,Vector3 p,Vector3 scale,Material material,bool collision=true,Transform owner=null)
 {
  var go=GameObject.CreatePrimitive(PrimitiveType.Cube);go.name=name;go.transform.SetParent(owner?owner:parent,true);go.transform.position=origin+p;go.transform.localScale=scale;go.GetComponent<Renderer>().sharedMaterial=material;go.layer=3;if(!collision)UnityEngine.Object.DestroyImmediate(go.GetComponent<Collider>());return go;
 }
 static void Label(string text,Vector3 p,float size,Color color)
 {
  var go=new GameObject(text);go.transform.SetParent(parent);go.transform.position=origin+p;go.transform.rotation=Quaternion.Euler(0,180,0);var label=go.AddComponent<TextMesh>();label.text=text;label.fontSize=48;label.characterSize=size;label.anchor=TextAnchor.MiddleCenter;label.color=color;
 }
 static GameObject Prop(string sourceName,Vector3 p,float height)
 {
  return ConRoiSchoolFurnitureV37.Create(sourceName+"_V37",origin+p,height,parent);
 }
 public static void Assemble()
 {
  if(File.Exists(ScenePath))throw new Exception("v37 already exists; preserve scene rather than regenerate");
  EditorSceneManager.OpenScene(ConRoiStabilizationV36.ScenePath);Directory.CreateDirectory(Folder);Directory.CreateDirectory("Docs/Reviews/school_v37");
  source=AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Art/Models/Model.blend");
  var story=UnityEngine.Object.FindAnyObjectByType<DieuPhoiHanhTrinh>();var chapter=story.gameObject.AddComponent<ChuongTruongHocV37>();chapter.story=story;story.schoolChapter=chapter;
  chapter.environment=new GameObject("School_V37_Classroom_Courtyard");parent=chapter.environment.transform;
  plaster=Mat("Warm_Plaster",new Color(.53f,.5f,.41f));wood=Mat("Aged_Timber",new Color(.3f,.2f,.11f));stone=Mat("Classroom_Terrazzo",new Color(.39f,.4f,.36f));metal=Mat("Dark_Frame",new Color(.13f,.17f,.15f));
  Box("Continuous_School_Floor",new Vector3(0,-.15f,3),new Vector3(18,.3f,28),stone);
  Box("Back_Wall",new Vector3(0,1.9f,14),new Vector3(16,3.8f,.2f),plaster);
  Box("West_Wall",new Vector3(-8,1.9f,7),new Vector3(.2f,3.8f,14),plaster);
  foreach(int sign in new[]{-1,1})Box("Entrance_Wing",new Vector3(sign*4.8f,1.9f,0),new Vector3(6.4f,3.8f,.2f),plaster);
  Box("Door_Lintel",new Vector3(0,3.35f,0),new Vector3(3.2f,.9f,.22f),wood);
  Box("Window_Sill_Wall",new Vector3(8,.55f,7),new Vector3(.2f,1.1f,14),plaster);
  Box("Window_Header",new Vector3(8,3.3f,7),new Vector3(.2f,1,14),plaster);
  for(int i=0;i<=7;i++)Box("Window_Mullion",new Vector3(8,1.95f,i*2),new Vector3(.16f,1.7f,.1f),metal);
  Box("Window_Horizontal_Frame",new Vector3(8,1.9f,7),new Vector3(.16f,.07f,14),metal);
  Box("Roof",new Vector3(0,3.85f,7),new Vector3(16.3f,.16f,14.3f),plaster);
  for(int x=-8;x<=8;x+=2)Box("Courtyard_Paving_Joint",new Vector3(x,.002f,-6),new Vector3(.015f,.004f,10),metal,false);
  foreach(int sign in new[]{-1,1})
  {
   Box("Courtyard_Planter_Base",new Vector3(sign*6,.2f,-7),new Vector3(2.4f,.4f,2.4f),plaster);
   var green=Mat("Cypress_Green",new Color(.12f,.21f,.11f));
   for(int tier=0;tier<3;tier++)Box("Trimmed_Cypress",new Vector3(sign*6,1+tier*.7f,-7),new Vector3(1.4f-tier*.3f,1,1.4f-tier*.3f),green,false);
   Box("Courtyard_Boundary",new Vector3(sign*8.7f,.55f,-4),new Vector3(.2f,1.1f,12),plaster);
  }
  Box("Courtyard_Rear_Boundary",new Vector3(0,.55f,-10.8f),new Vector3(17.6f,1.1f,.2f),plaster);
  for(int row=0;row<4;row++)foreach(int side in new[]{-1,1})
  {
   Prop("Ban_Sinh_Vien",new Vector3(side*4,0,2+row*2),.74f);Prop("Ghe_Sinh_Vien",new Vector3(side*4,0,1.2f+row*2),.82f);
  }
  Prop("Ban_Giao_Vien",new Vector3(-4,0,11),.78f);
  chapter.chair=Prop("Ghe_Giao_Vien",new Vector3(5.7f,0,11.2f),.9f).transform;
  Box("Chalkboard",new Vector3(0,2.05f,13.82f),new Vector3(6,1.45f,.1f),Mat("Chalkboard_Green",new Color(.055f,.14f,.12f)));
  Box("Chalk_Tray",new Vector3(0,1.28f,13.65f),new Vector3(6,.06f,.2f),wood);
  Label("ĐÁP ÁN MẪU\n1. Quan sát     2. Ghi nhớ\n3. Thực hiện theo trình tự",new Vector3(0,2.1f,13.74f),.055f,new Color(.75f,.76f,.65f));
  var dial=GameObject.CreatePrimitive(PrimitiveType.Cylinder);dial.name="Classroom_Clock";dial.transform.SetParent(parent);dial.transform.SetPositionAndRotation(origin+new Vector3(0,3.2f,13.75f),Quaternion.Euler(90,0,0));dial.transform.localScale=new Vector3(.5f,.025f,.5f);dial.GetComponent<Renderer>().sharedMaterial=Mat("Clock_Paper",new Color(.76f,.75f,.65f));UnityEngine.Object.DestroyImmediate(dial.GetComponent<Collider>());
  chapter.clockHand=Box("Clock_Hand",new Vector3(0,3.25f,13.7f),new Vector3(.015f,.17f,.02f),metal,false).transform;
  var warm=new GameObject("Classroom_Soft_Light").AddComponent<Light>();warm.transform.SetParent(parent);warm.transform.position=origin+new Vector3(0,3,7);warm.type=LightType.Point;warm.range=16;warm.intensity=4;warm.color=new Color(.8f,.83f,.76f);
  chapter.spawn=new GameObject("School_Spawn").transform;chapter.spawn.SetParent(parent);chapter.spawn.SetPositionAndRotation(origin+new Vector3(0,.94f,-7),Quaternion.identity);
  var ghost=UnityEngine.Object.Instantiate(story.ghosts[1].gameObject,parent);ghost.name="CoMai_Former_Teacher";FitModel(ghost.transform,origin+new Vector3(2.5f,0,10),1.75f);
  foreach(var interaction in ghost.GetComponentsInChildren<TuongTacHoCho>(true))UnityEngine.Object.DestroyImmediate(interaction);
  chapter.ghost=ghost.transform;chapter.face=ghost.GetComponent<HienGuongMatTheoKhu>();
  BindMai(chapter);
  var coat=Mat("Mai_Cardigan",new Color(.22f,.27f,.23f));foreach(var r in ghost.GetComponentsInChildren<Renderer>())if(!r.transform.IsChildOf(chapter.face.ghostHead))r.sharedMaterial=coat;
  var ai=UnityEngine.Object.Instantiate(story.puppets[0],parent);ai.name="ConRoi_Minh_GD3";FitModel(ai.transform,origin+new Vector3(0,0,8),3.5f);
  ai.giaiDoanHienTai=DieuKhienConRoi.GiaiDoan.GD3;ai.playerTransform=story.player.transform;ai.gesturePart=ai.GetComponentsInChildren<Transform>().First(t=>t.name=="ConRoi_Arm_R");ai.enabled=false;chapter.puppet=ai;
  var exit=Box("School_Return_Clock",new Vector3(0,1.1f,-9.5f),new Vector3(.6f,2.2f,.3f),wood);exit.AddComponent<TuongTacTruongV37>().chapter=chapter;exit.GetComponent<TuongTacTruongV37>().exit=true;
  chapter.tick=AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/ConRoiGame/Audio/Đồng hồ lớp.wav");
  story.ambience=story.ambience.Concat(new[]{chapter.tick}).ToArray();
  chapter.chair.rotation=Quaternion.Euler(0,-90,0);Garden();
  chapter.environment.SetActive(false);EditorSceneManager.SaveScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene(),ScenePath);AssetDatabase.SaveAssets();
  File.WriteAllText("Docs/Reviews/school_v37/assembly.json",new JObject{["scene"]=ScenePath,["source_v36_preserved"]=true,["model_sources"]=new JArray("Model.blend: student desks/chairs/teacher furniture","Existing Minh puppet and face projection","Scene-native classroom, window wall and courtyard"),["status"]="ASSEMBLED_NOT_YET_VERIFIED"}.ToString());
 }
}
