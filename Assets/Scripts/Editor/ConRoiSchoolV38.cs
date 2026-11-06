using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

// Additive authoring from v37. Neither the v36 checkpoint nor source models are rewritten.
public static class ConRoiSchoolV38
{
 public const string ScenePath="Assets/Scenes/SchoolIntegration_v38.unity";
 public const string Report="Docs/Reviews/school_v38";
 static Transform root;
 static Vector3 origin=new Vector3(-95,0,-45);
 static Material plaster,timber,metal,paper;
 static Material Mat(string name,Color color)
 {
  string folder="Assets/Art/Environment/SchoolV38";Directory.CreateDirectory(folder);
  string path=folder+"/"+name+".mat";var m=AssetDatabase.LoadAssetAtPath<Material>(path);
  if(!m){m=new Material(Shader.Find("Universal Render Pipeline/Lit"));m.color=color;m.SetFloat("_Smoothness",.16f);AssetDatabase.CreateAsset(m,path);}return m;
 }
 static GameObject Box(string name,Vector3 p,Vector3 size,Material mat,bool collision=true)
 {
  var go=GameObject.CreatePrimitive(PrimitiveType.Cube);go.name=name;go.layer=3;go.transform.SetParent(root);go.transform.position=origin+p;go.transform.localScale=size;go.GetComponent<Renderer>().sharedMaterial=mat;
  if(!collision)UnityEngine.Object.DestroyImmediate(go.GetComponent<Collider>());return go;
 }
 static void Label(string text,Vector3 p,float scale=.05f)
 {
  var go=new GameObject("Lettering_"+text.Split('\n')[0]);go.transform.SetParent(root);go.transform.position=origin+p;
  var t=go.AddComponent<TextMesh>();t.text=text;t.fontSize=48;t.characterSize=scale;t.anchor=TextAnchor.MiddleCenter;t.color=new Color(.77f,.76f,.63f);
 }
 static void Memory(ChuongTruongHocV37 chapter,int index,string name,Vector3 p,string text)
 {
  var board=Box(name,p,new Vector3(1.3f,.8f,.08f),paper);
  var interaction=board.AddComponent<TuongTacTruongV37>();interaction.chapter=chapter;interaction.memoryIndex=index;interaction.memory=text;
  Label(name,p+Vector3.back*.045f,.035f);
 }
 public static void Assemble()
 {
  if(File.Exists(ScenePath))throw new Exception("v38 exists; preserve assembled scene");
  EditorSceneManager.OpenScene(ConRoiSchoolV37.ScenePath);
  var chapter=UnityEngine.Object.FindAnyObjectByType<ChuongTruongHocV37>();root=chapter.environment.transform;
  plaster=Mat("Weathered_ ochre",new Color(.48f,.43f,.32f));timber=Mat("Old_Teak",new Color(.27f,.18f,.10f));metal=Mat("Painted_Green",new Color(.11f,.19f,.16f));paper=Mat("Notice_Paper",new Color(.61f,.59f,.46f));
  // A traversable west wing and covered courtyard link. All changes remain inside the school environment.
  Box("West_Wing_Continuous_Slab",new Vector3(-20,-.15f,3),new Vector3(24,.3f,28),plaster);
  Box("Covered_Link_Roof",new Vector3(-17,3.6f,-5),new Vector3(21,.18f,4),plaster);
  foreach(float x in new[]{-11f,-18f,-25f,-31f})Box("Walkway_Column",new Vector3(x,1.8f,-6.8f),new Vector3(.23f,3.6f,.23f),plaster);
  Box("Archive_Back_Wall",new Vector3(-22,1.9f,14),new Vector3(20,3.8f,.25f),plaster);
  Box("Archive_West_Wall",new Vector3(-32,1.9f,6),new Vector3(.25f,3.8f,16),plaster);
  Box("Archive_East_Wall",new Vector3(-12,1.9f,6),new Vector3(.25f,3.8f,16),plaster);
  foreach(float x in new[]{-28f,-16f})Box("Archive_Entrance_Wing",new Vector3(x,1.9f,-2),new Vector3(8,3.8f,.25f),plaster);
  Box("Archive_Door_Lintel",new Vector3(-22,3.4f,-2),new Vector3(4,.8f,.25f),timber);
  Box("Archive_Roof",new Vector3(-22,3.85f,6),new Vector3(20.3f,.18f,16.3f),plaster);
  Label("PHÒNG TƯ LIỆU",new Vector3(-22,2.9f,-2.16f),.065f);
  Label("GIẢNG ĐƯỜNG 203",new Vector3(0,2.9f,-.16f),.06f);
  // Shelves have backs, uprights, individual shelves and books rather than solid placeholder blocks.
  for(int rack=0;rack<4;rack++)
  {
   float x=-29+rack*4.7f;
   foreach(float side in new[]{-1f,1f})Box("Archive_Shelf_Upright",new Vector3(x+side*.85f,1.1f,12.5f),new Vector3(.08f,2.2f,.5f),timber);
   Box("Archive_Shelf_Back",new Vector3(x,1.1f,12.72f),new Vector3(1.8f,2.2f,.035f),timber);
   for(int row=0;row<4;row++)
   {
    float y=.16f+row*.51f;Box("Archive_Shelf",new Vector3(x,y,12.5f),new Vector3(1.8f,.05f,.55f),timber);
    for(int book=0;book<9;book++)Box("Bound_Course_Register",new Vector3(x-.7f+book*.17f,y+.22f,12.5f),new Vector3(.11f,.39f,.3f),book%2==0?metal:paper,false);
   }
  }
  foreach(float x in new[]{-27f,-17f})foreach(float z in new[]{2f,7f})
  {
   ConRoiSchoolFurnitureV37.Create("Ban_Sinh_Vien_Archive",origin+new Vector3(x,0,z),.74f,root);
   ConRoiSchoolFurnitureV37.Create("Ghe_Sinh_Vien_Archive",origin+new Vector3(x,0,z-.8f),.82f,root);
   Box("Open_Register",new Vector3(x,.79f,z),new Vector3(.6f,.035f,.4f),paper);
  }
  Memory(chapter,0,"LỊCH HỌC",new Vector3(-16,1.65f,-2.17f),"Lịch học cũ: Thứ hai — Kiểm tra theo mẫu. Thứ ba — Chữa bài theo mẫu. Góc giấy đã sờn ở cùng một chỗ.");
  Memory(chapter,1,"SỔ NHẬN XÉT",new Vector3(-29,1.4f,11.9f),"Sổ nhận xét: ‘Em cần mạnh dạn hơn.’ Dòng bên dưới, cùng nét mực: ‘Không làm khác hướng dẫn.’");
  Memory(chapter,2,"TRANG VIẾT DỞ",new Vector3(-22,1.3f,13.8f),"Trang viết dở của Minh: ‘Nếu chỉ có một đáp án đúng, tại sao cô vẫn nhìn ra cửa sổ?’ Phần còn lại bị tẩy đến rách giấy.");
  Memory(chapter,3,"BÀI LÀM CŨ",new Vector3(-17,1.1f,7.35f),"Bài làm cũ: Câu trả lời khác đã bị gạch. Bên lề có một dấu hỏi rất nhỏ, không có điểm.");
  Memory(chapter,4,"THÔNG BÁO",new Vector3(-4.8f,1.7f,-.17f),"Thông báo: Sinh viên thực hiện đúng trình tự. Các tờ giấy của những năm khác nhau có cùng một lỗi đánh máy.");
  Memory(chapter,5,"GIÁO ÁN",new Vector3(-4,1.25f,13.8f),"Giáo án của cô Mai: ‘Dành thời gian cho câu hỏi của học trò.’ Dòng ấy được gạch chân. Không có ghi chép nào phía sau.");
  // Low boundary walls make the authored playable footprint explicit without invisible blockers.
  Box("West_Courtyard_Boundary",new Vector3(-32,.6f,-5),new Vector3(.25f,1.2f,6),plaster);
  Box("West_Courtyard_Rear",new Vector3(-20,.6f,-10.8f),new Vector3(24,1.2f,.25f),plaster);
  // Open the existing west courtyard wall for the link, preserving the classroom wall.
  foreach(var t in root.GetComponentsInChildren<Transform>(true).Where(t=>t.name=="Courtyard_Boundary"&&t.position.x<origin.x).ToArray())UnityEngine.Object.DestroyImmediate(t.gameObject);
  foreach(float z in new[]{1f,8f})
  {
   var lamp=new GameObject("Archive_Soft_Light").AddComponent<Light>();lamp.transform.SetParent(root);lamp.transform.position=origin+new Vector3(-22,3.2f,z);lamp.type=LightType.Point;lamp.range=12;lamp.intensity=3;lamp.color=new Color(.76f,.8f,.68f);
  }
  Label("TRỞ VỀ",new Vector3(0,2.4f,-9.7f),.045f);
  EditorSceneManager.SaveScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene(),ScenePath);AssetDatabase.SaveAssets();Inspect();
 }
 public static void Inspect()
 {
  var scene=UnityEngine.SceneManagement.SceneManager.GetActiveScene();var chapter=UnityEngine.Object.FindAnyObjectByType<ChuongTruongHocV37>();
  var all=scene.GetRootGameObjects().SelectMany(r=>r.GetComponentsInChildren<Transform>(true)).ToArray();
  Directory.CreateDirectory(Report);
  File.WriteAllText(Report+"/scene-integrity.json",new JObject{["scene"]=scene.path,["missing_scripts"]=all.Sum(t=>GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(t.gameObject)),["school_renderers"]=chapter.environment.GetComponentsInChildren<Renderer>(true).Length,["school_colliders"]=chapter.environment.GetComponentsInChildren<Collider>(true).Length,["readable_memories"]=chapter.environment.GetComponentsInChildren<TuongTacTruongV37>(true).Count(i=>i.memoryIndex>=0),["integrated_bed_flow"]=chapter.story.schoolChapter==chapter,["assets"]=new JArray(AssetDatabase.GetDependencies(scene.path,true))}.ToString());
 }
 static GameObject RoundPart(Transform owner,string name,Vector3 center,float[] ys,float[] xs,float[] zs,Material mat)
 {
  const int sides=16;var vertices=new System.Collections.Generic.List<Vector3>();var faces=new System.Collections.Generic.List<int>();
  for(int row=0;row<ys.Length;row++)for(int i=0;i<sides;i++){float a=i*Mathf.PI*2/sides;vertices.Add(new Vector3(Mathf.Cos(a)*xs[row],ys[row],Mathf.Sin(a)*zs[row]));}
  for(int row=0;row<ys.Length-1;row++)for(int i=0;i<sides;i++){int a=row*sides+i,b=row*sides+(i+1)%sides;faces.AddRange(new[]{a,b,a+sides,b,b+sides,a+sides});}
  for(int i=1;i<sides-1;i++){faces.AddRange(new[]{0,i+1,i});int end=(ys.Length-1)*sides;faces.AddRange(new[]{end,end+i,end+i+1});}
  for(int i=0;i<faces.Count;i+=3){int swap=faces[i+1];faces[i+1]=faces[i+2];faces[i+2]=swap;}
  var mesh=new Mesh{name=name};mesh.SetVertices(vertices);mesh.SetTriangles(faces,0);mesh.RecalculateNormals();mesh.RecalculateBounds();
  string path="Assets/Art/Environment/SchoolV38/"+name+".asset";AssetDatabase.CreateAsset(mesh,path);
  var go=new GameObject(name);go.transform.SetParent(owner);go.transform.position=center;go.transform.rotation=Quaternion.identity;go.transform.localScale=new Vector3(1/owner.lossyScale.x,1/owner.lossyScale.y,1/owner.lossyScale.z);
  // Imported character parents are rotated; use world vertices via the inverse parent transform.
  go.transform.SetParent(null,true);go.transform.localScale=Vector3.one;go.transform.SetParent(owner,true);
  go.AddComponent<MeshFilter>().sharedMesh=mesh;go.AddComponent<MeshRenderer>().sharedMaterial=mat;return go;
 }
 public static void FinishVisuals()
 {
  EditorSceneManager.OpenScene(ScenePath);var c=UnityEngine.Object.FindAnyObjectByType<ChuongTruongHocV37>();root=c.environment.transform;
  if(!c.ghost.Find("Mai_Cardigan_Model"))
  {
   var clothing=new GameObject("Mai_Cardigan_Model").transform;clothing.SetParent(c.ghost);clothing.position=origin+new Vector3(2.5f,0,10);clothing.rotation=Quaternion.identity;clothing.localScale=Vector3.one/c.ghost.lossyScale.x;
   // World-space neutral root preserves the imported head, hands, legs and face projection.
   clothing.SetParent(null,true);clothing.localScale=Vector3.one;clothing.SetParent(c.ghost,true);
   foreach(var r in c.ghost.GetComponentsInChildren<Renderer>(true).Where(r=>r.name.Contains("Torso")||r.name.Contains("Arm")))r.enabled=false;
   var fabric=Mat("Mai_Knit",new Color(.25f,.29f,.21f));var skirt=Mat("Mai_Skirt",new Color(.15f,.16f,.13f));var skin=Mat("Mai_Neck",new Color(.43f,.31f,.22f));var collar=Mat("Mai_Collar",new Color(.65f,.61f,.48f));
   var p=clothing.position;
   RoundPart(clothing,"Mai_Cardigan_Tailored",p,new[]{.78f,.9f,1.12f,1.28f,1.32f},new[]{.20f,.17f,.23f,.24f,.12f},new[]{.12f,.11f,.13f,.10f,.075f},fabric);
   RoundPart(clothing,"Mai_Pleated_Skirt",p,new[]{.35f,.4f,.78f,.86f},new[]{.30f,.29f,.20f,.18f},new[]{.17f,.17f,.12f,.11f},skirt);
   RoundPart(clothing,"Mai_Neck",p,new[]{1.27f,1.43f},new[]{.058f,.055f},new[]{.055f,.055f},skin);
   foreach(int side in new[]{-1,1})
   {
    RoundPart(clothing,"Mai_Sleeve_"+side,p+Vector3.right*side*.27f,new[]{.77f,.95f,1.26f},new[]{.06f,.067f,.082f},new[]{.06f,.065f,.075f},fabric);
    RoundPart(clothing,"Mai_Hand_"+side,p+Vector3.right*side*.27f,new[]{.66f,.76f,.79f},new[]{.034f,.044f,.03f},new[]{.033f,.035f,.03f},skin);
   }
   for(int i=0;i<5;i++)
   {
    var b=GameObject.CreatePrimitive(PrimitiveType.Sphere);b.name="Cardigan_Button";UnityEngine.Object.DestroyImmediate(b.GetComponent<Collider>());b.transform.position=p+new Vector3(0,.87f+i*.08f,-.128f);b.transform.localScale=Vector3.one*.017f;b.transform.SetParent(clothing,true);b.GetComponent<Renderer>().sharedMaterial=collar;
   }
   var bun=GameObject.CreatePrimitive(PrimitiveType.Sphere);bun.name="Mai_Hair_Bun";UnityEngine.Object.DestroyImmediate(bun.GetComponent<Collider>());bun.transform.position=p+new Vector3(0,1.64f,.14f);bun.transform.localScale=new Vector3(.15f,.13f,.14f);bun.transform.SetParent(c.face.ghostHead,true);bun.GetComponent<Renderer>().sharedMaterial=skirt;
   // Fine spectacle rims follow the retained imported face during its animation and fade.
   for(int side=-1;side<=1;side+=2)for(int segment=0;segment<12;segment++)
   {
    float a=segment*Mathf.PI/6,b=(segment+1)*Mathf.PI/6;var from=p+new Vector3(side*.056f+Mathf.Cos(a)*.041f,1.571f+Mathf.Sin(a)*.03f,-.14f);var to=p+new Vector3(side*.056f+Mathf.Cos(b)*.041f,1.571f+Mathf.Sin(b)*.03f,-.14f);
    var bar=GameObject.CreatePrimitive(PrimitiveType.Cylinder);bar.name="Mai_Spectacle_Rim";UnityEngine.Object.DestroyImmediate(bar.GetComponent<Collider>());bar.transform.position=(from+to)*.5f;bar.transform.up=to-from;bar.transform.localScale=new Vector3(.006f,Vector3.Distance(from,to)*.5f,.006f);bar.transform.SetParent(c.face.ghostHead,true);bar.GetComponent<Renderer>().sharedMaterial=skirt;
   }
  }
  var texts=root.GetComponentsInChildren<TextMesh>(true);string materialPath="Assets/Art/Environment/SchoolV38/WorldText.mat";
  var textMaterial=AssetDatabase.LoadAssetAtPath<Material>(materialPath);
  if(!textMaterial){textMaterial=new Material(texts[0].font.material);textMaterial.shader=Shader.Find("ConRoi/WorldText");AssetDatabase.CreateAsset(textMaterial,materialPath);}
  foreach(var text in texts)text.GetComponent<Renderer>().sharedMaterial=textMaterial;
  EditorSceneManager.SaveScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene());AssetDatabase.SaveAssets();Inspect();
 }
 public static void BuildWindows()
 {
  EditorSceneManager.OpenScene(ScenePath);Inspect();
  EditorBuildSettings.scenes=new[]{new EditorBuildSettingsScene(ScenePath,true)};
  PlayerSettings.productName="Con Roi";PlayerSettings.bundleVersion="0.38.0";
  var report=BuildPipeline.BuildPlayer(new[]{ScenePath},"Builds/Windows_v38/ConRoi.exe",BuildTarget.StandaloneWindows64,BuildOptions.None);
  File.WriteAllText(Report+"/build.json",new JObject{["result"]=report.summary.result.ToString(),["errors"]=report.summary.totalErrors,["warnings"]=report.summary.totalWarnings,["bytes"]=report.summary.totalSize,["path"]=report.summary.outputPath,["scene"]=ScenePath}.ToString());
 }
}
