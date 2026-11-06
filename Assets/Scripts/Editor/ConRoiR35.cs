using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.Build.Reporting;
[InitializeOnLoad] public static class ConRoiR35 {
 public const string Scene="Assets/Scenes/JourneyRepair_v48_r35.unity",Out="Docs/Reviews/journey_v48/r35";
 static ConRoiR35(){EditorApplication.playModeStateChanged+=p=>{if(p==PlayModeStateChange.EnteredPlayMode&&SessionState.GetBool("r35.walk",false)){SessionState.SetBool("r35.walk",false);UnityEngine.Object.FindAnyObjectByType<DieuPhoiHanhTrinh>().gameObject.AddComponent<R35WalkReview>().output=Out+"/editor-main";}};}
 public static void Begin(){EditorSceneManager.OpenScene(Scene);SessionState.SetBool("r35.walk",true);EditorApplication.EnterPlaymode();}
 static JArray V(Vector3 p)=>new JArray(p.x,p.y,p.z);
 public static void Inspect(){
  EditorSceneManager.OpenScene(ConRoiR34.Scene);
  var s=UnityEngine.Object.FindAnyObjectByType<DieuPhoiHanhTrinh>();
  var ha=UnityEngine.Object.FindObjectsByType<TuongTacHoCho>(FindObjectsInactive.Include,FindObjectsSortMode.None).First(x=>x.kind==TuongTacHoCho.Kind.Ha);
  var near=new JArray();foreach(var r in UnityEngine.Object.FindObjectsByType<Renderer>(FindObjectsInactive.Include,FindObjectsSortMode.None).Where(r=>Vector3.Distance(r.bounds.center,ha.transform.position)<5)) near.Add(new JObject{["name"]=r.name,["path"]=PathOf(r.transform),["min"]=V(r.bounds.min),["max"]=V(r.bounds.max),["position"]=V(r.transform.position)});
  var ps=new JArray();foreach(var p in s.puppets)ps.Add(new JObject{["name"]=p.name,["pos"]=V(p.transform.position),["scale"]=V(p.transform.lossyScale),["head"]=V(p.dauTransform.position),["gesture"]=p.gesturePart?PathOf(p.gesturePart):"",["hierarchy"]=new JArray(p.GetComponentsInChildren<Transform>(true).Select(t=>PathOf(t)))});
  File.WriteAllText(Out+"/scene-inspect.json",new JObject{["ha"]=V(ha.transform.position),["ha_path"]=PathOf(ha.transform),["near"]=near,["puppets"]=ps,["market_spawn"]=V(s.marketSpawn.position),["books"]=V(s.traces[1].position)}.ToString());
 }
 static string PathOf(Transform t)=>t.parent?PathOf(t.parent)+"/"+t.name:t.name;
 public static void InspectRoute(){
  EditorSceneManager.OpenScene(Scene);var rows=new JArray();var at=new Vector3(770,5.5f,272.4f);
  foreach(var c in UnityEngine.Object.FindObjectsByType<Collider>(FindObjectsInactive.Include,FindObjectsSortMode.None).Where(c=>Vector3.Distance(c.ClosestPoint(at),at)<3))rows.Add(new JObject{["name"]=PathOf(c.transform),["min"]=V(c.bounds.min),["max"]=V(c.bounds.max),["layer"]=c.gameObject.layer,["enabled"]=c.enabled});
  File.WriteAllText(Out+"/route-colliders.json",rows.ToString());
 }
 static GameObject MeshPart(string name,Transform parent,string part){
  var g=new GameObject(name,typeof(MeshFilter),typeof(MeshRenderer));g.transform.SetParent(parent,false);
  g.GetComponent<MeshFilter>().sharedMesh=AssetDatabase.LoadAssetAtPath<Mesh>("Assets/Art/Gameplay/MarketR35/"+part+".asset");g.GetComponent<MeshRenderer>().sharedMaterial=AssetDatabase.LoadAssetAtPath<Material>("Assets/Art/Gameplay/MarketR35/"+part+".mat");return g;
 }
 static TextMesh Label(Transform parent,string name,string text,Vector3 position,float size=.018f){
  var g=new GameObject(name);g.transform.SetParent(parent,false);g.transform.localPosition=position;g.transform.localRotation=Quaternion.Euler(90,0,0);var t=g.AddComponent<TextMesh>();t.text=text;t.anchor=TextAnchor.MiddleCenter;t.characterSize=size;t.fontSize=48;t.color=new Color(.12f,.09f,.05f);return t;
 }
 static GoiHangChoR35 Parcel(Transform parent,QuanLyGoiHangChoR35 activity,int id,Vector3 pos){
  var g=new GameObject(id==0?"Goi_canh_phieu":"Goi_khoang_ben");g.transform.SetParent(parent,false);g.transform.position=pos;
  var p=g.AddComponent<GoiHangChoR35>();p.activity=activity;p.id=id;
  MeshPart("Kraft_base",g.transform,"Package_Base");var lid=MeshPart("Hinged_lid",g.transform,"Package_Lid");lid.transform.localPosition=new Vector3(0,.105f,.12f);p.lid=lid.transform;
  var scarf=MeshPart("Khan_that",g.transform,id==0?"Scarf_Red":"Scarf_Green");p.scarf=scarf.AddComponent<KhanTrongGoiHangR35>();p.scarf.colour=id==0?KhanTrongGoiHangR35.Colour.BrickRed:KhanTrongGoiHangR35.Colour.MossGreen;
  var c=g.AddComponent<BoxCollider>();c.center=new Vector3(0,.055f,0);c.size=new Vector3(.35f,.11f,.25f);
  p.label=Label(lid.transform,"Cung_nhan","KHĂN LEN",new Vector3(0,.005f,-.12f),.014f);
  p.inside=Label(scarf.transform,"Mau_khan",p.scarf.Description,new Vector3(0,.065f,0),.011f);return p;
 }
 public static void Author(){
  if(File.Exists(Scene))throw new Exception("Refuse overwrite derivative");
  var geo=JObject.Parse(File.ReadAllText("Assets/Art/Gameplay/MarketR35/Props.geometry.json"));
  foreach(var row in geo["objects"]){
   string n=(string)row["name"];var mesh=new Mesh{name=n};mesh.vertices=row["vertices"].Select(v=>new Vector3((float)v[0],(float)v[1],(float)v[2])).ToArray();mesh.normals=row["normals"].Select(v=>new Vector3((float)v[0],(float)v[1],(float)v[2])).ToArray();mesh.uv=row["uv"].Select(v=>new Vector2((float)v[0],(float)v[1])).ToArray();mesh.triangles=row["triangles"].Select(v=>(int)v).ToArray();mesh.RecalculateBounds();mesh.RecalculateTangents();AssetDatabase.CreateAsset(mesh,"Assets/Art/Gameplay/MarketR35/"+n+".asset");
   var mat=new Material(Shader.Find("Universal Render Pipeline/Lit")){name=n};var c=row["color"];mat.color=new Color((float)c[0],(float)c[1],(float)c[2]);mat.SetFloat("_Smoothness",.08f);AssetDatabase.CreateAsset(mat,"Assets/Art/Gameplay/MarketR35/"+n+".mat");
  }
  if(!AssetDatabase.CopyAsset(ConRoiR34.Scene,Scene))throw new Exception("Scene copy failed");EditorSceneManager.OpenScene(Scene);
  var s=UnityEngine.Object.FindAnyObjectByType<DieuPhoiHanhTrinh>();var activity=s.gameObject.AddComponent<QuanLyGoiHangChoR35>();activity.story=s;
  var root=new GameObject("R35_Nhung_goi_hang_giong_nhau");root.transform.SetParent(s.market.transform,true);activity.root=root.transform;
  // Layout coordinates are measured on the existing stall; no environment geometry is altered.
  var layout=JObject.Parse(File.ReadAllText(Out+"/prop-layout.json"));Func<string,Vector3> point=n=>new Vector3((float)layout[n][0],(float)layout[n][1],(float)layout[n][2]);
  foreach(var spec in new[]{("Work_Counter","workCounter",1.8f),("Side_Counter","sideCounter",.9f)}){var table=MeshPart(spec.Item1,root.transform,spec.Item1);table.transform.position=point(spec.Item2);var collider=table.AddComponent<BoxCollider>();collider.center=new Vector3(0,.375f,0);collider.size=new Vector3(spec.Item3,.75f,.55f);}
  activity.parcels=new[]{Parcel(root.transform,activity,0,point("red")),Parcel(root.transform,activity,1,point("green"))};
  var matGo=MeshPart("O_giao_hang",root.transform,"Delivery_Mat");matGo.transform.position=point("delivery");activity.delivery=matGo.transform;
  var mc=matGo.AddComponent<BoxCollider>();mc.center=new Vector3(0,.008f,0);mc.size=new Vector3(.44f,.016f,.33f);var mt=matGo.AddComponent<DichTuongTacChoR35>();mt.activity=activity;
  Label(matGo.transform,"Giao_label","ĐẶT THỬ · GẶP HÀ ĐỂ GIAO",new Vector3(0,.007f,.13f),.012f);
  var slip=MeshPart("Phieu_ghi_hang",root.transform,"Order_Slip");slip.transform.position=point("slip");activity.slip=slip.transform;
  var sc=slip.AddComponent<BoxCollider>();sc.center=new Vector3(0,.004f,0);sc.size=new Vector3(.18f,.008f,.24f);var st=slip.AddComponent<DichTuongTacChoR35>();st.activity=activity;st.slip=true;
  Label(slip.transform,"Phieu_chu","PHIẾU HÀNG\n1 KHĂN LEN\nXANH RÊU",new Vector3(0,.0035f,0),.015f);
  var preview=MeshPart("Placement_preview",root.transform,"Package_Base");preview.transform.position=activity.delivery.position+Vector3.up*.006f;
  var pm=new Material(Shader.Find("Universal Render Pipeline/Lit"));pm.color=new Color(.36f,.48f,.38f);pm.SetFloat("_Smoothness",0);AssetDatabase.CreateAsset(pm,"Assets/Art/Gameplay/MarketR35/Preview.mat");preview.GetComponent<Renderer>().sharedMaterial=pm;preview.transform.localScale=new Vector3(1,.08f,1);activity.placementPreview=preview;preview.SetActive(false);
  var original=s.puppets.First(p=>p.transform.IsChildOf(s.market.transform));var clone=UnityEngine.Object.Instantiate(original.gameObject,s.market.transform);clone.name="R35_Roi_tai_quay";
  var ai=clone.GetComponent<DieuKhienConRoi>();activity.gesture=clone.GetComponentsInChildren<Transform>(true).First(t=>t.name=="V40_Wooden_Joint_ConRoi_Arm_R");activity.puppet=clone.transform;var delta=point("puppetHead")-ai.dauTransform.position;clone.transform.position+=delta;
  foreach(var b in clone.GetComponentsInChildren<MonoBehaviour>(true))UnityEngine.Object.DestroyImmediate(b);
  foreach(var c in clone.GetComponentsInChildren<Collider>(true))UnityEngine.Object.DestroyImmediate(c);
  var sock=new GameObject("Mimic_package_socket");sock.transform.SetParent(s.market.transform,true);sock.transform.position=point("mimic");sock.transform.SetParent(clone.transform,true);
  var mp=Parcel(sock.transform,activity,0,point("mimic"));activity.mimicPackage=mp.transform;activity.mimicLid=mp.lid;UnityEngine.Object.DestroyImmediate(mp);foreach(var c in sock.GetComponentsInChildren<Collider>())UnityEngine.Object.DestroyImmediate(c);foreach(var t in sock.GetComponentsInChildren<TextMesh>())UnityEngine.Object.DestroyImmediate(t.gameObject);
  UnityEngine.Object.DestroyImmediate(s.GetComponent<HoSoSceneR34>());s.gameObject.AddComponent<HoSoLuuTaiR35>().story=s;
  EditorBuildSettings.scenes=new[]{new EditorBuildSettingsScene(Scene,true)};EditorSceneManager.SaveScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene());AssetDatabase.SaveAssets();
  File.WriteAllText(Out+"/integrated.json",new JObject{["scene"]=Scene,["geometry_origin"]=(string)geo["source"],["layout"]=layout,["clone_source"]=PathOf(original.transform),["school_complete_r34"]=s.GetComponent<TuongTacGiayLop203R32>().completeR34,["new_awareness_events"]=0,["new_action_events"]=0}.ToString());
 }
 public static void Build(){
  RefreshMeshes();FixPlacement();Validate();
  EditorSceneManager.OpenScene(Scene);EditorBuildSettings.scenes=new[]{new EditorBuildSettingsScene(Scene,true)};
  string exe=Path.GetFullPath("Builds/Windows_v48_r35_integrated/ConRoi.exe");Directory.CreateDirectory(Path.GetDirectoryName(exe));
  var r=BuildPipeline.BuildPlayer(new[]{Scene},exe,BuildTarget.StandaloneWindows64,BuildOptions.Development|BuildOptions.DetailedBuildReport);
  File.WriteAllText(Out+"/build.json",new JObject{["result"]=r.summary.result.ToString(),["errors"]=r.summary.totalErrors,["warnings"]=r.summary.totalWarnings,["scene"]=Scene,["binary"]=exe}.ToString());if(r.summary.result!=BuildResult.Succeeded)throw new Exception("Build failed");
 }
 public static void RefreshMeshes(){
  var geo=JObject.Parse(File.ReadAllText("Assets/Art/Gameplay/MarketR35/Props.geometry.json"));foreach(var row in geo["objects"]){
   var m=AssetDatabase.LoadAssetAtPath<Mesh>("Assets/Art/Gameplay/MarketR35/"+(string)row["name"]+".asset");m.Clear();m.vertices=row["vertices"].Select(v=>new Vector3((float)v[0],(float)v[1],(float)v[2])).ToArray();m.normals=row["normals"].Select(v=>new Vector3((float)v[0],(float)v[1],(float)v[2])).ToArray();m.uv=row["uv"].Select(v=>new Vector2((float)v[0],(float)v[1])).ToArray();m.triangles=row["triangles"].Select(v=>(int)v).ToArray();m.RecalculateBounds();m.RecalculateTangents();EditorUtility.SetDirty(m);
  }AssetDatabase.SaveAssets();
 }
 public static void Validate(){
  EditorSceneManager.OpenScene(Scene);var a=UnityEngine.Object.FindAnyObjectByType<QuanLyGoiHangChoR35>();var rows=new JArray();
  var geo=JObject.Parse(File.ReadAllText("Assets/Art/Gameplay/MarketR35/Props.geometry.json"));
  foreach(var row in geo["objects"]){string n=(string)row["name"];var m=AssetDatabase.LoadAssetAtPath<Mesh>("Assets/Art/Gameplay/MarketR35/"+n+".asset");bool same=m.vertexCount==row["vertices"].Count()&&m.triangles.SequenceEqual(row["triangles"].Select(v=>(int)v));for(int i=0;i<m.vertexCount&&same;i++){var p=row["vertices"][i];same&=Vector3.Distance(m.vertices[i],new Vector3((float)p[0],(float)p[1],(float)p[2]))<.000001f;}
   rows.Add(new JObject{["part"]=n,["exact_blender_vertices_and_winding"]=same,["normals"]=m.normals.Length==m.vertexCount,["uv"]=m.uv.Length==m.vertexCount,["bounds_m"]=V(m.bounds.size)});if(!same)throw new Exception("Source mesh mismatch "+n);
  }
  foreach(var c in a.root.GetComponentsInChildren<BoxCollider>(true))if(Vector3.Distance(c.transform.lossyScale,Vector3.one)>.001f)throw new Exception("Collider scale "+c.name);
  File.WriteAllText(Out+"/unity-mesh-validation.json",rows.ToString());
  var endings=new JArray();foreach(var t in new[]{(0,0,"A"),(0,2,"A2"),(3,0,"B"),(3,2,"C"),(2,1,"A"),(2,2,"A2"),(3,1,"B"),(4,2,"C")}){var got=DieuPhoiHanhTrinh.ResolveEnding(t.Item1,t.Item2);endings.Add(new JObject{["awareness"]=t.Item1,["interactions"]=t.Item2,["expected"]=t.Item3,["actual"]=got,["pass"]=got==t.Item3});if(got!=t.Item3)throw new Exception("Ending fixture changed");}
  File.WriteAllText(Out+"/ending-fixtures-SEPARATE.json",endings.ToString());
 }
 public static void FixPlacement(){
  EditorSceneManager.OpenScene(Scene);var a=UnityEngine.Object.FindAnyObjectByType<QuanLyGoiHangChoR35>();var j=JObject.Parse(File.ReadAllText(Out+"/prop-layout.json"));Func<string,Vector3> pos=n=>new Vector3((float)j[n][0],(float)j[n][1],(float)j[n][2]);
  a.root.SetParent(null,true);a.root.position=Vector3.zero;a.root.rotation=Quaternion.identity;a.root.localScale=Vector3.one;a.root.SetParent(a.story.market.transform,true);
  a.parcels[0].transform.position=pos("red");a.parcels[1].transform.position=pos("green");a.delivery.position=pos("delivery");a.slip.position=pos("slip");a.placementPreview.transform.position=a.delivery.position+Vector3.up*.006f;
  a.root.Find("Work_Counter").position=pos("workCounter");a.root.Find("Side_Counter").position=pos("sideCounter");
  foreach(var p in a.parcels){p.label.characterSize=.014f;p.inside.characterSize=.011f;}
  var sock=a.mimicPackage.parent;sock.SetParent(null,true);sock.position=pos("mimic");sock.rotation=Quaternion.identity;sock.localScale=Vector3.one;sock.SetParent(a.puppet,true);a.mimicPackage.localPosition=Vector3.zero;a.mimicPackage.localRotation=Quaternion.identity;a.mimicPackage.localScale=Vector3.one;
  var rows=new JArray();foreach(var c in a.root.GetComponentsInChildren<BoxCollider>(true))rows.Add(new JObject{["name"]=c.name,["localSize"]=V(c.size),["worldScale"]=V(c.transform.lossyScale)});
  File.WriteAllText(Out+"/metric-colliders.json",rows.ToString());EditorSceneManager.SaveScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
 }
}
