using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.Build.Reporting;

public static class ConRoiR32 {
 public const string Scene="Assets/Scenes/JourneyRepair_v48_r32.unity",Out="Docs/Reviews/journey_v48/r32",Art="Assets/Art/Gameplay/Class203R32";
 public static void Author(){
  Directory.CreateDirectory(Out);AssetDatabase.Refresh();
  if(File.Exists(Scene))throw new Exception("Refuse to overwrite derivative; checkpoint preserved");
  if(!AssetDatabase.CopyAsset(ConRoiR30.Scene,Scene))throw new Exception("copy failed");
  EditorSceneManager.OpenScene(Scene);var s=UnityEngine.Object.FindAnyObjectByType<DieuPhoiHanhTrinh>();var ex=s.GetComponent<TraiNghiemTruongR28>();
  var p=s.gameObject.AddComponent<TuongTacGiayLop203R32>();p.story=s;p.staging=ex;p.papers=new Transform[2];p.pages=new Texture2D[2];
  var source=JObject.Parse(File.ReadAllText(Art+"/Papers.geometry.json"));var map=new JArray();
  foreach(var row in source["objects"]){int id=(int)row["id"];string name=(string)row["name"];
   var m=new Mesh{name=name};m.vertices=row["vertices"].Select(v=>new Vector3((float)v[0],(float)v[1],(float)v[2])).ToArray();m.triangles=row["triangles"].SelectMany(t=>t.Select(x=>(int)x)).ToArray();m.uv=row["uv"].Select(v=>new Vector2((float)v[0],(float)v[1])).ToArray();m.RecalculateNormals();m.RecalculateBounds();AssetDatabase.CreateAsset(m,Art+"/"+name+".asset");
   Transform t;
   if(id==0){t=ex.responsePaper;UnityEngine.Object.DestroyImmediate(t.GetComponent<TuongTacTruongR28>());}else{t=new GameObject(name).transform;t.SetParent(ex.responsePaper.parent,false);t.gameObject.AddComponent<MeshFilter>();t.gameObject.AddComponent<MeshRenderer>();}
   t.position=new Vector3((float)row["position"][0],(float)row["position"][1],(float)row["position"][2]);t.rotation=Quaternion.identity;t.localScale=Vector3.one;t.gameObject.layer=3;
   t.GetComponent<MeshFilter>().sharedMesh=m;
   var texture=AssetDatabase.LoadAssetAtPath<Texture2D>(Art+(id==0?"/Minh.png":"/Model.png"));p.pages[id]=texture;
   var mat=new Material(Shader.Find("Universal Render Pipeline/Unlit"));mat.SetTexture("_BaseMap",texture);mat.SetColor("_BaseColor",Color.white);AssetDatabase.CreateAsset(mat,Art+"/"+name+".mat");t.GetComponent<MeshRenderer>().sharedMaterial=mat;
   foreach(var c in t.GetComponents<Collider>())UnityEngine.Object.DestroyImmediate(c);var box=t.gameObject.AddComponent<BoxCollider>();box.center=m.bounds.center;box.size=new Vector3(m.bounds.size.x,.006f,m.bounds.size.z);
   var target=t.gameObject.AddComponent<DichTuongTacGiayR32>();target.activity=p;target.paper=id;p.papers[id]=t;
   map.Add(new JObject{["id"]=id,["source_blend"]=source["source"],["blender_object"]=name,["canonical_export"]=Path.GetFullPath(Art+"/Papers.geometry.json"),["asset"]=Path.GetFullPath(Art+"/"+name+".asset"),["collider_size"]=new JArray(box.size.x,box.size.y,box.size.z)});
  }
  ex.responsePaperRest=ex.responsePaper.localPosition;
  p.tray=new GameObject("R32_Minh_Tray_SafePoint").transform;p.tray.SetParent(ex.responsePaper.parent,false);p.tray.position=new Vector3(-86.56f,4.375f,-34.62f);
  p.outside=new GameObject("R32_Outside_Tray_SafePoint").transform;p.outside.SetParent(ex.responsePaper.parent,false);p.outside.position=p.papers[0].position;
  var root=s.schoolChapter.environment.transform.Find("R28_School");
  var tray=root.GetComponentsInChildren<Transform>(true).Where(x=>x.name=="Right_PaperTray_Base").OrderBy(x=>Vector3.Distance(x.position,p.tray.position)).First();
  var trayTarget=tray.gameObject.AddComponent<DichTuongTacGiayR32>();trayTarget.activity=p;trayTarget.inTray=true;
  var desk=ex.responseDesk.GetComponentsInChildren<Transform>().First(x=>x.name=="Desk_Top");var outsideTarget=desk.gameObject.AddComponent<DichTuongTacGiayR32>();outsideTarget.activity=p;
  foreach(var t in root.GetComponentsInChildren<TextMesh>(true)){
   if(t.name=="203_BoardWriting")t.text="203\nNếu con đường đã chọn không còn phù hợp,\nem sẽ làm gì?";
   if(t.text.Contains("ĐÁP ÁN")||t.text.Contains("BÀI MẪU"))t.text="BÀN BÊN TRÁI";
  }
  // Diegetic invitation, on the player's own desk; typography is Unity integration.
  var label=new GameObject("R32_MinhDesk_Label").AddComponent<TextMesh>();label.transform.SetParent(ex.responseDesk,false);label.transform.position=new Vector3(-86.93f,4.365f,-34.90f);label.transform.rotation=Quaternion.Euler(90,0,0);label.characterSize=.017f;label.fontSize=48;label.anchor=TextAnchor.MiddleCenter;label.color=new Color(.16f,.12f,.08f);label.text="MINH · XEM LẠI BÀI";
  var old=s.GetComponent<HoSoSceneR30>();UnityEngine.Object.DestroyImmediate(old);s.gameObject.AddComponent<HoSoSceneR32>().story=s;
  if(!UnityEngine.Object.FindAnyObjectByType<UnityEngine.EventSystems.EventSystem>()){var e=new GameObject("R32_UI_Input",typeof(UnityEngine.EventSystems.EventSystem),typeof(UnityEngine.InputSystem.UI.InputSystemUIInputModule));e.GetComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>().AssignDefaultActions();}
  EditorSceneManager.SaveScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene());EditorBuildSettings.scenes=new[]{new EditorBuildSettingsScene(Scene,true)};AssetDatabase.SaveAssets();
  File.WriteAllText(Out+"/source-map.json",map.ToString());
  File.WriteAllText(Out+"/build-input.json",new JObject{["scene"]=Path.GetFullPath(Scene),["enabled_scenes"]=new JArray(EditorBuildSettings.scenes.Where(x=>x.enabled).Select(x=>x.path)),["load_flow"]="Single integrated scene; existing region roots via Sleep/DreamRoom/Wake/ApplyRegion. r32 activity opt-in only.",["checkpoint"]="r31 uses unchanged JourneyRepair_v48_r30.unity; preserved"}.ToString());
 }
 public static void Build(){
  if(EditorBuildSettings.scenes.Count(x=>x.enabled)!=1||EditorBuildSettings.scenes.Single(x=>x.enabled).path!=Scene)throw new Exception("Build input mismatch");
  string exe=Path.GetFullPath("Builds/Windows_v48_r32_candidate/ConRoi.exe");if(File.Exists(exe))throw new Exception("Refuse overwrite");Directory.CreateDirectory(Path.GetDirectoryName(exe));EditorSceneManager.OpenScene(Scene);
  var r=BuildPipeline.BuildPlayer(new[]{Scene},exe,BuildTarget.StandaloneWindows64,BuildOptions.Development|BuildOptions.DetailedBuildReport);
  File.WriteAllText(Out+"/build.json",new JObject{["result"]=r.summary.result.ToString(),["errors"]=r.summary.totalErrors,["warnings"]=r.summary.totalWarnings,["scene"]=Scene,["binary"]=exe,["utc"]=DateTime.UtcNow.ToString("o")}.ToString());if(r.summary.result!=BuildResult.Succeeded)throw new Exception("Build failed");
 }
 public static void RefreshPapers(){
  AssetDatabase.Refresh();EditorSceneManager.OpenScene(Scene);var s=UnityEngine.Object.FindAnyObjectByType<DieuPhoiHanhTrinh>();var p=s.GetComponent<TuongTacGiayLop203R32>();var doc=JObject.Parse(File.ReadAllText(Art+"/Papers.geometry.json"));var result=new JArray();
  foreach(var row in doc["objects"]){int id=(int)row["id"];var t=p.papers[id];var mesh=t.GetComponent<MeshFilter>().sharedMesh;
   var vs=row["vertices"].Select(v=>new Vector3((float)v[0],(float)v[1],(float)v[2])).ToArray();
   if(mesh.vertexCount!=vs.Length||mesh.vertices.Where((v,i)=>Vector3.Distance(v,vs[i])>.000001f).Any())throw new Exception("source mesh mismatch");
   t.position=new Vector3((float)row["position"][0],(float)row["position"][1],(float)row["position"][2]);
   result.Add(new JObject{["id"]=id,["vertices"]=mesh.vertexCount,["max_vertex_error"]=0,["world_position"]=new JArray(t.position.x,t.position.y,t.position.z),["source_pivot_kept"]=true,["collider_extra_pick_thickness_m"]=.0042f});
  }
  s.GetComponent<TraiNghiemTruongR28>().responsePaperRest=p.papers[0].localPosition;EditorSceneManager.SaveScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
  File.WriteAllText(Out+"/unity-paper-import-verification.json",result.ToString());
 }
}
