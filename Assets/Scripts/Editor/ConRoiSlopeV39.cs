using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class ConRoiSlopeV39
{
 public const string ScenePath="Assets/Scenes/JourneyIntegration_v39.unity";
 public const string Report="Docs/Reviews/journey_v39";
 const string AssetsFolder="Assets/Art/Environment/SlopeV39";
 static Transform root;
 static Vector3 origin=new Vector3(-180,0,-60);
 static Material road,stone,plaster,wood,metal;
 static float Height(float z)=>Mathf.Clamp(z,0,60)*.1f;
 static Material Mat(string name,Color color)
 {
  string path=AssetsFolder+"/"+name+".mat";var m=AssetDatabase.LoadAssetAtPath<Material>(path);if(!m){m=new Material(Shader.Find("Universal Render Pipeline/Lit"));m.color=color;m.SetFloat("_Smoothness",.17f);AssetDatabase.CreateAsset(m,path);}return m;
 }
 static GameObject Box(string name,Vector3 p,Vector3 size,Material mat,bool collision=true)
 {
  var g=GameObject.CreatePrimitive(PrimitiveType.Cube);g.name=name;g.layer=3;g.transform.SetParent(root);g.transform.position=origin+p;g.transform.localScale=size;g.GetComponent<Renderer>().sharedMaterial=mat;if(!collision)UnityEngine.Object.DestroyImmediate(g.GetComponent<Collider>());return g;
 }
 static void Label(string text,Vector3 p,float scale=.045f)
 {
  var go=new GameObject("Slope_Lettering");go.transform.SetParent(root);go.transform.position=origin+p;var label=go.AddComponent<TextMesh>();label.text=text;label.fontSize=48;label.characterSize=scale;label.anchor=TextAnchor.MiddleCenter;label.color=new Color(.76f,.72f,.57f);go.GetComponent<Renderer>().sharedMaterial=AssetDatabase.LoadAssetAtPath<Material>("Assets/Art/Environment/SchoolV38/WorldText.mat");
 }
 static Bounds Bounds(Transform t){var rs=t.GetComponentsInChildren<Renderer>(true);var b=rs[0].bounds;foreach(var r in rs)b.Encapsulate(r.bounds);return b;}
 static void Fit(Transform t,Vector3 point,float height)
 {
  t.rotation=Quaternion.Euler(-90,0,0);
  var b=Bounds(t);t.localScale*=height/b.size.y;b=Bounds(t);t.position+=point-new Vector3(b.center.x,b.min.y,b.center.z);
 }
 static void Road()
 {
  var vertices=new System.Collections.Generic.List<Vector3>();var triangles=new System.Collections.Generic.List<int>();
  for(int row=0;row<=40;row++){float z=row*2;vertices.Add(new Vector3(-4,Height(z),z));vertices.Add(new Vector3(4,Height(z),z));}
  for(int i=0;i<40;i++){int a=i*2;triangles.AddRange(new[]{a,a+2,a+1,a+1,a+2,a+3});}
  var mesh=new Mesh{name="Slope_Continuous_Road_5_71deg"};mesh.SetVertices(vertices);mesh.SetTriangles(triangles,0);mesh.RecalculateNormals();mesh.RecalculateBounds();AssetDatabase.CreateAsset(mesh,AssetsFolder+"/Road.asset");
  var go=new GameObject(mesh.name);go.layer=3;go.transform.SetParent(root);go.transform.position=origin;go.AddComponent<MeshFilter>().sharedMesh=mesh;go.AddComponent<MeshRenderer>().sharedMaterial=road;go.AddComponent<MeshCollider>().sharedMesh=mesh;
 }
 static void Memory(ChuongDocV39 c,int index,Vector3 p,string title,string text)
 {
  var go=Box(title,p,new Vector3(.75f,.5f,.05f),plaster);var i=go.AddComponent<TuongTacDocV39>();i.chapter=c;i.memoryIndex=index;i.memory=text;Label(title,p+Vector3.back*.03f,.027f);
 }
 public static void Assemble()
 {
  if(File.Exists(ScenePath))throw new Exception("v39 already exists; preserve authored scene");
  EditorSceneManager.OpenScene(ConRoiSchoolV38.ScenePath);Directory.CreateDirectory(AssetsFolder);Directory.CreateDirectory(Report);
  var story=UnityEngine.Object.FindAnyObjectByType<DieuPhoiHanhTrinh>();var chapter=story.gameObject.AddComponent<ChuongDocV39>();story.slopeChapter=chapter;chapter.story=story;
  chapter.environment=new GameObject("DaLat_Slope_V39");root=chapter.environment.transform;
  road=Mat("Worn_Concrete",new Color(.32f,.31f,.27f));stone=Mat("Retaining_Stone",new Color(.25f,.27f,.24f));plaster=Mat("Ochre_Walls",new Color(.49f,.40f,.28f));wood=Mat("Old_ Timber",new Color(.26f,.16f,.085f));metal=Mat("Oxidized_Frame",new Color(.18f,.22f,.19f));Road();
  Box("Slope_Understructure",new Vector3(0,-1,40),new Vector3(8,2,80),stone);
  for(int side=-1;side<=1;side+=2)
  {
   for(int part=0;part<40;part++)
   {
    float z=part*2+1,y=Height(z);
    var curb=Box("Visible_Concrete_Curb",new Vector3(side*4.15f,y+.22f,z),new Vector3(.3f,.44f,2.02f),stone);if(z<60)curb.transform.rotation=Quaternion.Euler(-Mathf.Atan(.1f)*Mathf.Rad2Deg,0,0);
   }
   for(int house=0;house<5;house++)
   {
    float z=8+house*12,y=Height(z);
    Box("House_Retaining_Plinth",new Vector3(side*7,y-1.2f,z),new Vector3(5,2.4f,8),stone);
    Box("DaLat_House_Plaster",new Vector3(side*8,y+2,z),new Vector3(5,4,8),plaster);
    for(int roof=-1;roof<=1;roof+=2){var r=Box("Pitched_Roof",new Vector3(side*8+roof*1.35f,y+4.55f,z),new Vector3(3,.14f,8.7f),wood,false);r.transform.rotation=Quaternion.Euler(0,0,-roof*23);}
    for(int window=0;window<2;window++)
    {
     Box("House_Window_Recess",new Vector3(side*5.46f,y+2,z-2+window*4),new Vector3(.08f,1.35f,1.2f),metal);
     foreach(int slat in new[]{-1,0,1})Box("Window_Shutter_Slat",new Vector3(side*5.40f,y+2+slat*.35f,z-2+window*4),new Vector3(.09f,.05f,1.2f),wood,false);
    }
   }
  }
  // Closed dusty suitcase beside a street-facing door, optional and outside the center walking lane.
  chapter.suitcase=Box("Tung_Packed_Dusty_Suitcase",new Vector3(3.1f,Height(44)+.36f,44),new Vector3(.75f,.72f,.28f),wood).transform;
  var caseDetail=new GameObject("Suitcase_Seams_Latches_Handle").transform;caseDetail.SetParent(chapter.suitcase);caseDetail.localPosition=Vector3.zero;caseDetail.rotation=Quaternion.identity;caseDetail.localScale=Vector3.one;
  foreach(float x in new[]{-.24f,.24f})
  {
   var latch=Box("Suitcase_Brass_Latch",new Vector3(3.1f+x,4.78f,43.846f),new Vector3(.075f,.09f,.025f),metal,false);latch.transform.SetParent(chapter.suitcase,true);
  }
  var handle=Box("Suitcase_Handle",new Vector3(3.1f,5.17f,44),new Vector3(.24f,.045f,.07f),metal,false);handle.transform.SetParent(chapter.suitcase,true);
  Box("Tung_Door_Frame",new Vector3(4.6f,5.5f,44.1f),new Vector3(.18f,2.2f,1.4f),wood);
  Box("Tung_Door_Panel",new Vector3(4.48f,5.5f,44.1f),new Vector3(.09f,2.0f,1.2f),metal);
  Memory(chapter,0,new Vector3(-2.5f,Height(18)+1.2f,18),"VÉ XE CŨ","Một vé xe cũ, ngày khởi hành đã bị sửa nhiều lần. Mặt sau ghi: ‘Để sáng mai.’");
  Memory(chapter,1,new Vector3(-2.5f,Height(54)+1.2f,54),"ĐỊA CHỈ VIẾT TAY","Địa chỉ chỗ làm ở thành phố khác. Tờ giấy được bọc kỹ, không có dấu từng mang theo.");
  var ghost=UnityEngine.Object.Instantiate(story.ghosts[0].gameObject,root);ghost.name="AnhTung_Cousin";Fit(ghost.transform,origin+new Vector3(1.6f,6,73.5f),1.78f);chapter.ghost=ghost.transform;chapter.face=ghost.GetComponent<HienGuongMatTheoKhu>();
  foreach(var old in ghost.GetComponentsInChildren<TuongTacHoCho>(true))UnityEngine.Object.DestroyImmediate(old);
  foreach(var col in ghost.GetComponentsInChildren<Collider>(true))UnityEngine.Object.DestroyImmediate(col);
  var proxy=Box("Tung_Interaction",new Vector3(1.6f,6.89f,73.5f),new Vector3(.65f,1.78f,.65f),metal);UnityEngine.Object.DestroyImmediate(proxy.GetComponent<Renderer>());var talk=proxy.AddComponent<TuongTacDocV39>();talk.chapter=chapter;
  var jacket=Mat("Tung_Faded_Jacket",new Color(.26f,.28f,.33f));foreach(var r in ghost.GetComponentsInChildren<Renderer>(true).Where(r=>!r.transform.IsChildOf(chapter.face.ghostHead)))r.sharedMaterial=jacket;
  var puppet=UnityEngine.Object.Instantiate(story.schoolChapter.puppet,root);puppet.name="ConRoi_Minh_GD4_FinalOnly";Fit(puppet.transform,origin+new Vector3(-1.6f,6,75),2.8f);puppet.giaiDoanHienTai=DieuKhienConRoi.GiaiDoan.GD4;puppet.enabled=false;puppet.gameObject.SetActive(false);chapter.puppet=puppet;
  var door=Box("Last_Door",new Vector3(0,7.1f,76.5f),new Vector3(1.4f,2.2f,.16f),wood);var end=door.AddComponent<TuongTacDocV39>();end.chapter=chapter;end.finalDoor=true;
  Label("",new Vector3(0,8.5f,76.3f));
  chapter.spawn=new GameObject("Slope_Spawn").transform;chapter.spawn.SetParent(root);chapter.spawn.position=origin+new Vector3(0,.94f,1);
  // A modeled spool and visible gutter form the distant moving accent.
  chapter.rollingWood=Box("Distant_Wooden_Spool",new Vector3(-3.4f,5.1f,48),new Vector3(.45f,.45f,.6f),wood,false).transform;
  for(int i=0;i<3;i++){var rim=Box("Spool_Rim",new Vector3(-3.4f,5.1f,47.7f+i*.3f),new Vector3(.6f,.6f,.05f),wood,false);rim.transform.SetParent(chapter.rollingWood,true);}
  chapter.rollingSound=AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/ConRoiGame/Audio/Gỗ va.wav");
  story.ambience=story.ambience.Concat(new[]{AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/ConRoiGame/Audio/Gió trên dốc.wav")}).ToArray();
  var presentation=story.gameObject.AddComponent<TrinhBayHanhTrinhV39>();story.presentation=presentation;presentation.story=story;
  presentation.wire=AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/ConRoiGame/Audio/V39_Wire.wav");presentation.step=AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/ConRoiGame/Audio/V39_Step.wav");
  var all=UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects().SelectMany(r=>r.GetComponentsInChildren<Transform>(true)).ToArray();
  presentation.strings=all.Where(t=>t.name.StartsWith("ConRoi_String_")).Select(t=>t.GetComponent<Renderer>()).Where(r=>r).ToArray();
  presentation.endingPhone=all.First(t=>t.name=="CR_Phone");
  var desk=all.First(t=>t.name=="CR_Desk_Top").GetComponent<Renderer>().bounds;
  var journal=new GameObject("Ending_Open_Journal").transform;journal.SetParent(story.hub.transform);journal.position=new Vector3(desk.center.x,desk.max.y+.025f,desk.center.z);
  var cover=BoxInHub(story,"Journal_Cover",journal.position,new Vector3(.42f,.025f,.28f),wood);cover.transform.SetParent(journal,true);
  foreach(int side in new[]{-1,1}){var page=BoxInHub(story,"Journal_Open_Leaf",journal.position+new Vector3(side*.10f,.022f,0),new Vector3(.20f,.018f,.26f),plaster);page.transform.rotation=Quaternion.Euler(0,0,-side*5);page.transform.SetParent(journal,true);}
  presentation.endingJournal=journal;
  chapter.environment.SetActive(false);PlayerSettings.runInBackground=true;
  EditorSceneManager.SaveScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene(),ScenePath);AssetDatabase.SaveAssets();Inspect();
 }
 static GameObject BoxInHub(DieuPhoiHanhTrinh story,string name,Vector3 p,Vector3 size,Material mat)
 {
  var g=GameObject.CreatePrimitive(PrimitiveType.Cube);g.name=name;g.transform.SetParent(story.hub.transform);g.transform.position=p;g.transform.localScale=size;g.GetComponent<Renderer>().sharedMaterial=mat;UnityEngine.Object.DestroyImmediate(g.GetComponent<Collider>());return g;
 }
 public static void Inspect()
 {
  var scene=UnityEngine.SceneManagement.SceneManager.GetActiveScene();var all=scene.GetRootGameObjects().SelectMany(r=>r.GetComponentsInChildren<Transform>(true)).ToArray();var story=UnityEngine.Object.FindAnyObjectByType<DieuPhoiHanhTrinh>();
  File.WriteAllText(Report+"/scene-integrity.json",new JObject{["scene"]=scene.path,["missing_scripts"]=all.Sum(t=>GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(t.gameObject)),["region_count"]=story.RegionCount,["slope_renderers"]=story.slopeChapter.environment.GetComponentsInChildren<Renderer>(true).Length,["Tung_bounds"]=Bounds(story.slopeChapter.ghost).ToString(),["Tung_head"]=story.slopeChapter.face.ghostHead.position.ToString(),["strings"]=story.presentation.strings.Length,["wire_audio"]=story.presentation.wire!=null,["step_audio"]=story.presentation.step!=null,["dependencies"]=new JArray(AssetDatabase.GetDependencies(scene.path,true))}.ToString());
 }
 public static void FinishVisuals()
 {
  EditorSceneManager.OpenScene(ScenePath);var story=UnityEngine.Object.FindAnyObjectByType<DieuPhoiHanhTrinh>();var chapter=story.slopeChapter;root=chapter.environment.transform;
  Fit(chapter.ghost,origin+new Vector3(1.6f,6,73.5f),1.78f);
  foreach(var ai in UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects().SelectMany(g=>g.GetComponentsInChildren<DieuKhienConRoi>(true)))
  {
   if(!ai.dauTransform||ai.dauTransform.name=="Minh_Head_Pivot")continue;
   var old=ai.dauTransform;var bounds=Bounds(old);float unit=bounds.size.y;
   var pivot=new GameObject("Minh_Head_Pivot").transform;pivot.position=bounds.center;pivot.rotation=Quaternion.Euler(0,180,0);pivot.SetParent(ai.transform,true);old.SetParent(pivot,true);ai.dauTransform=pivot;
   var dark=Mat("Minh_Eyes",new Color(.045f,.035f,.025f));var skin=Mat("Minh_Wood_Features",new Color(.40f,.27f,.16f));
   for(int side=-1;side<=1;side+=2)
   {
    var eye=GameObject.CreatePrimitive(PrimitiveType.Sphere);eye.name="Minh_Eye";UnityEngine.Object.DestroyImmediate(eye.GetComponent<Collider>());eye.transform.position=bounds.center+new Vector3(side*unit*.18f,unit*.1f,-bounds.size.z*.45f);eye.transform.localScale=Vector3.one*unit*.08f;eye.transform.SetParent(pivot,true);eye.GetComponent<Renderer>().sharedMaterial=dark;
   }
   var nose=GameObject.CreatePrimitive(PrimitiveType.Sphere);nose.name="Minh_Nose";UnityEngine.Object.DestroyImmediate(nose.GetComponent<Collider>());nose.transform.position=bounds.center+Vector3.back*bounds.size.z*.49f;nose.transform.localScale=new Vector3(.15f,.18f,.20f)*unit;nose.transform.SetParent(pivot,true);nose.GetComponent<Renderer>().sharedMaterial=skin;
  }
  foreach(var projection in UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects().SelectMany(g=>g.GetComponentsInChildren<HienGuongMatTheoKhu>(true)))projection.minhHeadSource=story.schoolChapter.puppet.dauTransform;
  foreach(var ai in UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects().SelectMany(g=>g.GetComponentsInChildren<DieuKhienConRoi>(true)))
  {
   if(!ai.dauTransform||ai.dauTransform.name!="Minh_Head_Pivot")continue;
   var body=ai.GetComponentsInChildren<Renderer>(true).First(r=>r.name=="ConRoi_Body_Wood").bounds;var head=Bounds(ai.dauTransform);
   // The source head has its height baked into vertices as well as transform.z.
   // Align the retained mesh once during authoring; runtime gaze only rotates its centered pivot.
   ai.dauTransform.position+=new Vector3(body.center.x-head.center.x,body.max.y+.025f-head.min.y,body.center.z-head.center.z);
  }
  if(!root.Find("Tung_Neck"))
  {
   var skin=Mat("Tung_Skin",new Color(.40f,.28f,.18f));var trim=Mat("Tung_Jacket_Trim",new Color(.12f,.14f,.15f));
   var neck=GameObject.CreatePrimitive(PrimitiveType.Cylinder);neck.name="Tung_Neck";UnityEngine.Object.DestroyImmediate(neck.GetComponent<Collider>());neck.transform.SetParent(root);neck.transform.position=origin+new Vector3(1.6f,7.36f,73.5f);neck.transform.localScale=new Vector3(.12f,.09f,.12f);neck.GetComponent<Renderer>().sharedMaterial=skin;
   for(int i=0;i<5;i++)Box("Tung_Jacket_Button",new Vector3(1.6f,6.9f+i*.09f,73.36f),new Vector3(.018f,.018f,.015f),trim,false);
   var spool=chapter.rollingWood;
   foreach(var r in spool.GetComponentsInChildren<Renderer>(true))r.enabled=false;
   for(int part=0;part<3;part++)
   {
    var g=GameObject.CreatePrimitive(PrimitiveType.Cylinder);g.name=part==1?"Spool_Axle":"Spool_Circular_Flange";UnityEngine.Object.DestroyImmediate(g.GetComponent<Collider>());g.transform.position=spool.position+Vector3.right*(part-1)*.27f;g.transform.rotation=Quaternion.Euler(0,0,90);g.transform.localScale=part==1?new Vector3(.35f,.27f,.35f):new Vector3(.6f,.035f,.6f);g.transform.SetParent(spool,true);g.GetComponent<Renderer>().sharedMaterial=Mat("Spool_Wood",new Color(.29f,.19f,.10f));
   }
  }
  var terrain=Mat("Damp_Hillside",new Color(.22f,.26f,.18f));var pine=Mat("Pine_Foliage",new Color(.12f,.19f,.12f));
  if(!root.Find("Surrounding_Hillside"))
  {
   var vs=new System.Collections.Generic.List<Vector3>();var ts=new System.Collections.Generic.List<int>();
   for(int z=0;z<=12;z++)for(int x=0;x<=16;x++)
   {
    float px=-80+x*10,pz=-20+z*10;float y=Height(pz)-.7f+Mathf.Abs(px)*.12f+Mathf.Sin(pz*.12f)*Mathf.Abs(px)*.018f;vs.Add(new Vector3(px,y,pz));
   }
   for(int z=0;z<12;z++)for(int x=0;x<16;x++){int a=z*17+x;ts.AddRange(new[]{a,a+17,a+1,a+1,a+17,a+18});}
   var mesh=new Mesh{name="Surrounding_Hillside"};mesh.SetVertices(vs);mesh.SetTriangles(ts,0);mesh.RecalculateNormals();mesh.RecalculateBounds();AssetDatabase.CreateAsset(mesh,AssetsFolder+"/Hillside.asset");
   var go=new GameObject(mesh.name);go.layer=3;go.transform.SetParent(root);go.transform.position=origin;go.AddComponent<MeshFilter>().sharedMesh=mesh;go.AddComponent<MeshRenderer>().sharedMaterial=terrain;go.AddComponent<MeshCollider>().sharedMesh=mesh;
   for(int side=-1;side<=1;side+=2)for(int tree=0;tree<14;tree++)
   {
    float x=side*(16+tree%3*7),z=-5+tree*7,y=Height(z)-.7f+Mathf.Abs(x)*.12f+Mathf.Sin(z*.12f)*Mathf.Abs(x)*.018f;
    Box("Pine_Trunk",new Vector3(x,y+2,z),new Vector3(.25f,4,.25f),terrain,false);
    for(int tier=0;tier<4;tier++){var crown=Box("Pine_Crown",new Vector3(x,y+3+tier*.8f,z),new Vector3(3-tier*.55f,1.3f,3-tier*.55f),pine,false);crown.transform.rotation=Quaternion.Euler(0,tree*23+tier*17,0);}
   }
   Box("Top_Garden_Wall",new Vector3(0,6.7f,79.5f),new Vector3(8,1.4f,.25f),terrain);
   // Suitcase straps, hinge, dust on top, and two feet preserve the packed closed form.
   foreach(int sign in new[]{-1,1})
   {
    var band=Box("Suitcase_Worn_Strap",new Vector3(3.1f+sign*.24f,4.76f,43.85f),new Vector3(.055f,.70f,.035f),terrain,false);band.transform.SetParent(chapter.suitcase,true);
   }
   var dust=Box("Suitcase_Dust",new Vector3(3.1f,5.124f,44),new Vector3(.72f,.006f,.26f),Mat("Dust",new Color(.45f,.42f,.33f)),false);dust.transform.SetParent(chapter.suitcase,true);
  }
  EditorSceneManager.SaveScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene());AssetDatabase.SaveAssets();Inspect();
 }
 public static void BuildWindows()
 {
  EditorSceneManager.OpenScene(ScenePath);Inspect();EditorBuildSettings.scenes=new[]{new EditorBuildSettingsScene(ScenePath,true)};PlayerSettings.bundleVersion="0.39.0";
  var report=BuildPipeline.BuildPlayer(new[]{ScenePath},"Builds/Windows_v39/ConRoi.exe",BuildTarget.StandaloneWindows64,BuildOptions.None);
  File.WriteAllText(Report+"/build.json",new JObject{["result"]=report.summary.result.ToString(),["errors"]=report.summary.totalErrors,["warnings"]=report.summary.totalWarnings,["bytes"]=report.summary.totalSize,["path"]=report.summary.outputPath,["scene"]=ScenePath}.ToString());
 }
}
