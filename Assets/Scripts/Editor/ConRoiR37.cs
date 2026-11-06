using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.Build.Reporting;
[InitializeOnLoad] public static class ConRoiR37 {
 public const string Scene="Assets/Scenes/JourneyRepair_v48_r37.unity",Out="Docs/Reviews/journey_v48/r37",Art="Assets/Art/Gameplay/LakeR37";
 static ConRoiR37(){EditorApplication.playModeStateChanged+=Mode;}
 static void Mode(PlayModeStateChange p){if(p==PlayModeStateChange.EnteredPlayMode&&SessionState.GetBool("r37.review",false)){SessionState.SetBool("r37.review",false);new GameObject("R37_OptInReview").AddComponent<R37LakeReview>().output=Out+"/editor-source-final";}}
 public static void Begin(){EditorSceneManager.OpenScene(Scene);SessionState.SetBool("r37.review",true);EditorApplication.EnterPlaymode();}
 static JArray V(Vector3 v)=>new JArray(v.x,v.y,v.z);
 static string P(Transform t)=>t.parent?P(t.parent)+"/"+t.name:t.name;
 public static void Inspect(){
  EditorSceneManager.OpenScene(ConRoiR36.Scene);var s=UnityEngine.Object.FindAnyObjectByType<DieuPhoiHanhTrinh>();var b=s.GetComponent<TuTheBaoBenCauV48>();var reflection=s.GetComponent<PhanChieuMatHo>();
  var rs=new JArray();foreach(var r in UnityEngine.Object.FindObjectsByType<Renderer>(FindObjectsInactive.Include).Where(r=>Vector3.Distance(r.bounds.center,b.actorPosition)<7))rs.Add(new JObject{["path"]=P(r.transform),["min"]=V(r.bounds.min),["max"]=V(r.bounds.max)});
  File.WriteAllText(Out+"/survey.json",new JObject{["bao"]=V(b.actorPosition),["rail"]=V(b.railContact),["waterDirection"]=V(b.waterDirection),["spawn"]=V(s.lakeSpawn.position),["spawnEuler"]=V(s.lakeSpawn.eulerAngles),["calendar"]=V(s.traces[0].position),["puppetHead"]=V(s.puppets[0].dauTransform.position),["puppetRoot"]=V(s.puppets[0].transform.position),["water"]=P(reflection.water.transform),["waterBoundsMin"]=V(reflection.water.bounds.min),["waterBoundsMax"]=V(reflection.water.bounds.max),["near"]=rs}.ToString());
 }
 static GameObject Part(string name,Transform parent,string mesh){var g=new GameObject(name,typeof(MeshFilter),typeof(MeshRenderer));g.transform.SetParent(parent,false);g.GetComponent<MeshFilter>().sharedMesh=AssetDatabase.LoadAssetAtPath<Mesh>(Art+"/"+mesh+".asset");g.GetComponent<Renderer>().sharedMaterial=AssetDatabase.LoadAssetAtPath<Material>(Art+"/"+mesh+".mat");return g;}
 static TextMesh Text(Transform parent,string name,string value,Vector3 pos,float size){var g=new GameObject(name,typeof(TextMesh));g.transform.SetParent(parent,false);g.transform.localPosition=pos;g.transform.localRotation=Quaternion.Euler(90,0,0);var t=g.GetComponent<TextMesh>();t.font=Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");t.fontSize=64;t.characterSize=size;t.anchor=TextAnchor.MiddleCenter;t.alignment=TextAlignment.Center;t.color=new Color(.12f,.105f,.08f);t.text=value;g.GetComponent<Renderer>().sharedMaterial=AssetDatabase.LoadAssetAtPath<Material>("Assets/Art/Gameplay/MarketR36/DepthTestedFont.mat");return t;}
 public static void Author(){
  if(File.Exists(Scene))throw new Exception("Refuse overwrite derivative");
  var geo=JObject.Parse(File.ReadAllText(Art+"/Props.geometry.json"));foreach(var row in geo["objects"]){string n=(string)row["name"];var mesh=new Mesh{name=n};mesh.vertices=row["vertices"].Select(v=>new Vector3((float)v[0],(float)v[1],(float)v[2])).ToArray();mesh.normals=row["normals"].Select(v=>new Vector3((float)v[0],(float)v[1],(float)v[2])).ToArray();mesh.uv=row["uv"].Select(v=>new Vector2((float)v[0],(float)v[1])).ToArray();mesh.triangles=row["triangles"].Select(v=>(int)v).ToArray();mesh.RecalculateBounds();mesh.RecalculateTangents();AssetDatabase.CreateAsset(mesh,Art+"/"+n+".asset");var m=new Material(Shader.Find("Universal Render Pipeline/Lit"));var c=row["color"];m.color=new Color((float)c[0],(float)c[1],(float)c[2]);m.SetFloat("_Smoothness",0);AssetDatabase.CreateAsset(m,Art+"/"+n+".mat");}
  AssetDatabase.CopyAsset(ConRoiR36.Scene,Scene);EditorSceneManager.OpenScene(Scene);var s=UnityEngine.Object.FindAnyObjectByType<DieuPhoiHanhTrinh>();var a=s.gameObject.AddComponent<TuongTacSoTayHoR37>();a.story=s;
  var root=new GameObject("R37_Dieu_minh_dat_xuong");root.transform.SetParent(s.lake.transform,true);a.root=root.transform;
  var b=s.GetComponent<TuTheBaoBenCauV48>();var stand=Part("Mat_ke_an_toan",root.transform,"Safe_Stand");stand.transform.position=new Vector3(658.71f,1.1400611f,512.16f);stand.transform.rotation=Quaternion.LookRotation(b.waterDirection);var bc=stand.AddComponent<BoxCollider>();bc.center=new Vector3(0,.37f,0);bc.size=new Vector3(.46f,.74f,.36f);
  var sock=new GameObject("Book_safe_socket");sock.transform.SetParent(root.transform,false);sock.transform.SetPositionAndRotation(stand.transform.position+Vector3.up*.741f,stand.transform.rotation);a.socket=sock.transform;
  var target=new GameObject("So_cua_Minh");target.transform.SetParent(root.transform,false);target.transform.SetPositionAndRotation(a.socket.position,a.socket.rotation);a.book=target.transform;
  foreach(string part in new[]{"Book_Back","Book_Pages","Book_Spine"})Part(part,target.transform,part);
  a.leaf=Part("Bia_gap_quanh_gay",target.transform,"Book_Leaf").transform;a.leaf.localPosition=Vector3.up*.014f;a.leftPage=Part("Trang_trong",target.transform,"Book_LeftPage").transform;a.leftPage.localPosition=Vector3.up*.014f;
  a.planText=Text(target.transform,"Dong_ke_hoach","KẾ HOẠCH\nTối nay\nviết một trang.\n────────",new Vector3(.078f,.0107f,0),.009f);
  a.blankText=Text(a.leftPage,"Tieu_de_trang_trong","TRANG TRỐNG",new Vector3(.078f,-.0025f,.068f),.008f);a.blankText.transform.localRotation=Quaternion.Inverse(Quaternion.Euler(0,0,-180))*Quaternion.Euler(90,0,0);
  a.coverText=Text(a.leaf,"Ten_Minh","MINH\nSỔ NHÁP",new Vector3(.078f,.0037f,0),.013f);
  a.bookCollider=target.AddComponent<BoxCollider>();a.bookCollider.center=new Vector3(.078f,.008f,0);a.bookCollider.size=new Vector3(.164f,.02f,.224f);target.AddComponent<DichTuongTacSoTayR37>().activity=a;
  stand.AddComponent<DichTuongTacSoTayR37>().activity=a;
  var preview=Part("Preview_dat_so",root.transform,"Book_Back");preview.transform.SetPositionAndRotation(a.socket.position+Vector3.up*.001f,a.socket.rotation);preview.transform.localScale=new Vector3(1,.2f,1);var pm=new Material(Shader.Find("Universal Render Pipeline/Lit"));pm.color=new Color(.26f,.38f,.29f);pm.SetFloat("_Smoothness",0);AssetDatabase.CreateAsset(pm,Art+"/Preview.mat");preview.GetComponent<Renderer>().sharedMaterial=pm;a.preview=preview;preview.SetActive(false);
  a.gesture=s.puppets[0].GetComponentsInChildren<Transform>(true).First(t=>t.name=="V40_Wooden_Joint_ConRoi_Arm_R");
  a.paperSound=AssetDatabase.LoadAssetAtPath<AudioClip>(Art+"/Paper.wav");a.closeSound=AssetDatabase.LoadAssetAtPath<AudioClip>(Art+"/Close.wav");a.placeSound=AssetDatabase.LoadAssetAtPath<AudioClip>(Art+"/Place.wav");
  EditorBuildSettings.scenes=new[]{new EditorBuildSettingsScene(Scene,true)};EditorSceneManager.SaveScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene());AssetDatabase.SaveAssets();
  File.WriteAllText(Out+"/prop-layout.json",new JObject{["stand"]=V(stand.transform.position),["book"]=V(a.socket.position),["euler"]=V(a.socket.eulerAngles),["spawn"]=V(s.lakeSpawn.position),["source"]=(string)geo["source"]}.ToString());
  EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);EditorSceneManager.OpenScene(Scene);Validate();
 }
 public static void Validate(){
  var scene=UnityEngine.SceneManagement.SceneManager.GetActiveScene();var a=UnityEngine.Object.FindAnyObjectByType<TuongTacSoTayHoR37>();var all=scene.GetRootGameObjects().SelectMany(g=>g.GetComponentsInChildren<Transform>(true)).ToArray();
  var missing=new JArray(all.Where(t=>GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(t.gameObject)>0).Select(t=>P(t)));var required=new JArray();Action<string,UnityEngine.Object> need=(n,o)=>required.Add(new JObject{["field"]=n,["valid"]=o!=null,["target"]=o?o.name:"NULL"});
  need("story",a.story);need("root",a.root);need("book",a.book);need("leaf",a.leaf);need("leftPage",a.leftPage);need("socket",a.socket);need("gesture",a.gesture);need("plan",a.planText);need("blank",a.blankText);need("cover",a.coverText);need("preview",a.preview);need("collider",a.bookCollider);need("paperSound",a.paperSound);need("closeSound",a.closeSound);need("placeSound",a.placeSound);
  var prefabs=new JArray(all.Where(t=>PrefabUtility.IsAnyPrefabInstanceRoot(t.gameObject)).Select(t=>new JObject{["path"]=P(t),["source"]=PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(t.gameObject),["status"]=PrefabUtility.GetPrefabInstanceStatus(t.gameObject).ToString()}));
  var meshes=new JArray();var geo=JObject.Parse(File.ReadAllText(Art+"/Props.geometry.json"));foreach(var row in geo["objects"]){string n=(string)row["name"];var m=AssetDatabase.LoadAssetAtPath<Mesh>(Art+"/"+n+".asset");bool same=m.vertexCount==row["vertices"].Count()&&m.triangles.SequenceEqual(row["triangles"].Select(x=>(int)x));for(int i=0;i<m.vertexCount&&same;i++){var p=row["vertices"][i];same&=Vector3.Distance(m.vertices[i],new Vector3((float)p[0],(float)p[1],(float)p[2]))<.000001f;}meshes.Add(new JObject{["mesh"]=n,["matchesBlender"]=same,["uv"]=m.uv.Length==m.vertexCount,["normals"]=m.normals.Length==m.vertexCount});}
  var j=new JObject{["scene"]=scene.path,["missingScripts"]=missing,["required"]=required,["prefabs"]=prefabs,["meshes"]=meshes,["players"]=all.Count(t=>t.GetComponent<DieuKhienDiChuyen>()),["cameras"]=all.Count(t=>t.GetComponent<Camera>()),["listeners"]=all.Count(t=>t.GetComponent<AudioListener>()),["eventSystems"]=all.Count(t=>t.GetComponent<UnityEngine.EventSystems.EventSystem>()),["serializedHarnesses"]=new JArray(all.SelectMany(t=>t.GetComponents<MonoBehaviour>()).Where(c=>c&&c.GetType().Name.Contains("Review")).Select(c=>c.GetType().Name)),["colliders"]=new JArray(a.root.GetComponentsInChildren<BoxCollider>(true).Select(c=>new JObject{["path"]=P(c.transform),["size"]=V(c.size),["scale"]=V(c.transform.lossyScale),["trigger"]=c.isTrigger,["layer"]=c.gameObject.layer,["rigidbody"]=c.attachedRigidbody!=null}))};File.WriteAllText(Out+"/source-reopened.json",j.ToString());
  if(missing.Count>0||required.Any(x=>!(bool)x["valid"])||meshes.Any(x=>!(bool)x["matchesBlender"])||prefabs.Any(x=>(string)x["status"]=="MissingAsset"))throw new Exception("r37 source audit failed");
 }
 public static void Build(){EditorSceneManager.OpenScene(Scene);Validate();EditorBuildSettings.scenes=new[]{new EditorBuildSettingsScene(Scene,true)};string exe=Path.GetFullPath("Builds/Windows_v48_r37_integrated/ConRoi.exe");Directory.CreateDirectory(Path.GetDirectoryName(exe));var r=BuildPipeline.BuildPlayer(new[]{Scene},exe,BuildTarget.StandaloneWindows64,BuildOptions.Development|BuildOptions.DetailedBuildReport);File.WriteAllText(Out+"/build.json",new JObject{["result"]=r.summary.result.ToString(),["errors"]=r.summary.totalErrors,["warnings"]=r.summary.totalWarnings,["scene"]=Scene,["binary"]=exe}.ToString());if(r.summary.result!=BuildResult.Succeeded)throw new Exception("Build failed");}
 public static void Tune(){EditorSceneManager.OpenScene(Scene);var a=UnityEngine.Object.FindAnyObjectByType<TuongTacSoTayHoR37>();a.planText.text="KẾ HOẠCH\nTối nay\nviết một trang.";var t=Text(a.planText.transform,"Dau_gach_ke_hoach","────────────",new Vector3(0,.0002f,0),.009f);t.transform.localRotation=Quaternion.identity;t.transform.localPosition=new Vector3(0,-.011f,0);EditorSceneManager.SaveScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene());EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);EditorSceneManager.OpenScene(Scene);Validate();}

 public static void FitText(){
  EditorSceneManager.OpenScene(Scene);var a=UnityEngine.Object.FindAnyObjectByType<TuongTacSoTayHoR37>();
  a.planText.characterSize=.0023f;a.blankText.characterSize=.0017f;a.coverText.characterSize=.0035f;
  var strike=a.planText.transform.Find("Dau_gach_ke_hoach").GetComponent<TextMesh>();strike.characterSize=.0023f;
  strike.transform.localPosition=new Vector3(0,-.005f,0);
  EditorSceneManager.SaveScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene());AssetDatabase.SaveAssets();
  EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);EditorSceneManager.OpenScene(Scene);Validate();
 }

 public static void Readability(){
  EditorSceneManager.OpenScene(Scene);var a=UnityEngine.Object.FindAnyObjectByType<TuongTacSoTayHoR37>();
  foreach(string name in new[]{"Book_Pages","Book_LeftPage"}){var mat=AssetDatabase.LoadAssetAtPath<Material>(Art+"/"+name+".mat");mat.shader=Shader.Find("Universal Render Pipeline/Unlit");mat.SetColor("_BaseColor",new Color(.68f,.65f,.56f));EditorUtility.SetDirty(mat);}
  a.planText.characterSize=.0026f;a.blankText.characterSize=.0025f;
  foreach(var t in new[]{a.planText,a.blankText,a.coverText,a.planText.transform.Find("Dau_gach_ke_hoach").GetComponent<TextMesh>()})t.color=new Color(.055f,.047f,.035f);
  EditorSceneManager.SaveScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene());AssetDatabase.SaveAssets();
  EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);EditorSceneManager.OpenScene(Scene);Validate();
 }
}
