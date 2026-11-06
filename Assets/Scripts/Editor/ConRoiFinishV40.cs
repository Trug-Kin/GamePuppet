using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class ConRoiFinishV40
{
 public const string ScenePath="Assets/Scenes/JourneyExperience_v40.unity";
 public const string Report="Docs/Reviews/journey_v40";
 const string Art="Assets/Art/Environment/JourneyV40";
 static Material plaster,wood,green,iron,clay,glass,paper;
 static int serial;
 static Material Mat(string name,Color color,float rough=.8f)
 {
  string path=Art+"/"+name+".mat";var result=AssetDatabase.LoadAssetAtPath<Material>(path);
  if(!result){result=new Material(Shader.Find("Universal Render Pipeline/Lit"));result.color=color;result.SetFloat("_Smoothness",1-rough);AssetDatabase.CreateAsset(result,path);}
  return result;
 }
 static GameObject Part(Transform parent,string name,Vector3 position,Vector3 size,Material mat,PrimitiveType shape=PrimitiveType.Cube)
 {
  var g=GameObject.CreatePrimitive(shape);g.name=name;g.layer=3;g.transform.SetParent(parent,true);g.transform.position=position;WorldScale(g.transform,size);g.GetComponent<Renderer>().sharedMaterial=mat;
  UnityEngine.Object.DestroyImmediate(g.GetComponent<Collider>());return g;
 }
 static void WorldScale(Transform t,Vector3 size)
 {
  var p=t.parent?t.parent.lossyScale:Vector3.one;t.localScale=new Vector3(size.x/Mathf.Abs(p.x),size.y/Mathf.Abs(p.y),size.z/Mathf.Abs(p.z));
 }
 static void Line(Transform parent,string name,Vector3 a,Vector3 b,float width,Material mat)
 {
  var g=Part(parent,name,(a+b)*.5f,new Vector3(width,Vector3.Distance(a,b)*.5f,width),mat,PrimitiveType.Cylinder);g.transform.up=b-a;
 }
 static void Label(Transform parent,string text,Vector3 p,float size=.035f,Quaternion? rotation=null)
 {
  var g=new GameObject("Painted_Lettering");g.transform.SetParent(parent);g.transform.position=p;g.transform.rotation=rotation??Quaternion.identity;
  var t=g.AddComponent<TextMesh>();t.text=text;t.characterSize=size;t.fontSize=56;t.anchor=TextAnchor.MiddleCenter;t.color=new Color(.78f,.74f,.61f);
  g.GetComponent<Renderer>().sharedMaterial=AssetDatabase.LoadAssetAtPath<Material>("Assets/Art/Environment/SchoolV38/WorldText.mat");
 }
 static Mesh Cone(string name,int sides=20)
 {
  var vertices=new List<Vector3>();var triangles=new List<int>();
  for(int row=0;row<4;row++)for(int i=0;i<sides;i++)
  {float a=i*Mathf.PI*2/sides,r=(1-row/3f)*.5f;vertices.Add(new Vector3(Mathf.Cos(a)*r,row/3f,Mathf.Sin(a)*r));}
  for(int row=0;row<3;row++)for(int i=0;i<sides;i++){int a=row*sides+i,b=row*sides+(i+1)%sides;triangles.AddRange(new[]{a,a+sides,b,b,a+sides,b+sides});}
  var mesh=new Mesh{name=name};mesh.SetVertices(vertices);mesh.SetTriangles(triangles,0);mesh.RecalculateNormals();mesh.RecalculateBounds();AssetDatabase.CreateAsset(mesh,Art+"/"+name+".asset");return mesh;
 }
 static Texture2D Grain(string name,int mode)
 {
  var path=Art+"/"+name+".png";
  if(!File.Exists(path))
  {
   var tex=new Texture2D(256,256,TextureFormat.RGB24,false);var values=new Color[256*256];
   for(int y=0;y<256;y++)for(int x=0;x<256;x++)
   {
    float noise=Mathf.PerlinNoise(x*.12f,y*.12f)*.15f+Mathf.PerlinNoise(x*.025f,y*.025f)*.15f;
    float stripe=mode==1?Mathf.Sin(x*.18f+Mathf.PerlinNoise(x*.02f,y*.015f)*8)*.08f:mode==2&&((x%64<2)||(y%64<2))?-.25f:0;
    values[y*256+x]=Color.white*(.7f+noise+stripe);
   }
   tex.SetPixels(values);tex.Apply();File.WriteAllBytes(path,tex.EncodeToPNG());UnityEngine.Object.DestroyImmediate(tex);AssetDatabase.ImportAsset(path);
  }
  return AssetDatabase.LoadAssetAtPath<Texture2D>(path);
 }
 static void SurfacePass(DieuPhoiHanhTrinh story)
 {
  // Clone only the v40 scene's material assignments; no checkpoint material asset is edited.
  var map=new Dictionary<Material,Material>();
  foreach(var r in story.schoolChapter.environment.GetComponentsInChildren<MeshRenderer>(true).Concat(story.slopeChapter.environment.GetComponentsInChildren<MeshRenderer>(true)).Concat(story.hub.GetComponentsInChildren<MeshRenderer>(true)))
  {
   if(r.GetComponent<TextMesh>()||r.name.Contains("String")||r.name.Contains("Eye"))continue;
   var materials=r.sharedMaterials;
   for(int i=0;i<materials.Length;i++)
   {
    var old=materials[i];if(!old||!old.HasProperty("_BaseMap")||old.GetTexture("_BaseMap"))continue;
    if(!map.TryGetValue(old,out var replacement))
    {
     replacement=new Material(old){name=old.name+"_V40"};int mode=old.name.IndexOf("wood",StringComparison.OrdinalIgnoreCase)>=0||old.name.Contains("Teak")||old.name.Contains("Timber")?1:old.name.Contains("Concrete")?2:0;
     replacement.SetTexture("_BaseMap",Grain(mode==1?"TimberGrain":mode==2?"ConcretePavers":"DampPlaster",mode));replacement.SetTextureScale("_BaseMap",new Vector2(3,3));replacement.SetFloat("_Smoothness",.16f);
     AssetDatabase.CreateAsset(replacement,Art+"/Surface_"+(serial++)+".mat");map.Add(old,replacement);
    }
    materials[i]=replacement;
   }
   r.sharedMaterials=materials;
  }
 }
 static void GardenPass(Transform environment,Mesh cone)
 {
  foreach(var crown in environment.GetComponentsInChildren<MeshFilter>(true).Where(f=>f.name=="Pine_Crown"||f.name=="Garden_Cypress_Crown"))
  {crown.sharedMesh=cone;crown.transform.position-=Vector3.up*crown.transform.lossyScale.y*.5f;crown.GetComponent<Renderer>().sharedMaterial=green;}
 }
 static void Pot(Transform parent,Vector3 p,int seed)
 {
  Part(parent,"Terracotta_Pot",p+Vector3.up*.22f,new Vector3(.42f,.22f,.42f),clay,PrimitiveType.Cylinder);
  Part(parent,"Pot_Soil",p+Vector3.up*.44f,new Vector3(.35f,.015f,.35f),wood,PrimitiveType.Cylinder);
  for(int i=0;i<7;i++)
  {
   float a=(i*137+seed)*Mathf.Deg2Rad;Vector3 head=p+new Vector3(Mathf.Cos(a)*.23f,.65f+(i%3)*.12f,Mathf.Sin(a)*.23f);
   Line(parent,"Flower_Stem",p+Vector3.up*.42f,head,.012f,green);
   for(int petal=0;petal<5;petal++)
   {float b=petal*Mathf.PI*2/5;Part(parent,"Hydrangea_Petal",head+new Vector3(Mathf.Cos(b)*.045f,.015f,Mathf.Sin(b)*.045f),new Vector3(.075f,.045f,.075f),paper,PrimitiveType.Sphere);}
  }
 }
 static void SlopePass(DieuPhoiHanhTrinh story)
 {
  var root=story.slopeChapter.environment.transform;var o=new Vector3(-180,0,-60);
  for(int side=-1;side<=1;side+=2)for(int house=0;house<5;house++)
  {
   float z=8+house*12,y=Mathf.Min(z,60)*.1f;
   // The retained massing now has separate painted trim, shutters, tile courses,
   // drainage, meter boxes and numbered entrances typical of a hillside lane.
   Part(root,"Weathered_Skirting",o+new Vector3(side*5.48f,y+.28f,z),new Vector3(.06f,.56f,7.9f),iron);
   Part(root,"Rain_Gutter",o+new Vector3(side*5.4f,y+3.83f,z),new Vector3(.12f,.14f,8.55f),iron);
   Line(root,"Downpipe",o+new Vector3(side*5.3f,y+.15f,z+3.5f),o+new Vector3(side*5.3f,y+3.9f,z+3.5f),.07f,iron);
   for(int window=0;window<2;window++)
   {
    float wz=z-2+window*4;
    Part(root,"Window_Glass",o+new Vector3(side*5.405f,y+2,wz),new Vector3(.025f,1.12f,1.03f),glass);
    foreach(int s in new[]{-1,1})
    {
     Part(root,"Window_Frame",o+new Vector3(side*5.36f,y+2+s*.64f,wz),new Vector3(.12f,.07f,1.35f),wood);
     Part(root,"Window_Frame",o+new Vector3(side*5.36f,y+2,wz+s*.63f),new Vector3(.12f,1.3f,.07f),wood);
    }
    var awning=Part(root,"Window_Rain_Awning",o+new Vector3(side*5.15f,y+2.78f,wz),new Vector3(.85f,.045f,1.6f),iron);awning.transform.rotation=Quaternion.Euler(0,0,side*12);
   }
   for(int tile=0;tile<18;tile++)foreach(int roofSide in new[]{-1,1})
   {
    var seam=Part(root,"Clay_Roof_Course",o+new Vector3(side*8+roofSide*1.35f,y+4.65f,z-4.1f+tile*.48f),new Vector3(3.04f,.055f,.055f),clay);seam.transform.rotation=Quaternion.Euler(0,0,-roofSide*23);
   }
   Part(root,"Enamel_House_Number",o+new Vector3(side*5.35f,y+1.7f,z+3),new Vector3(.06f,.32f,.42f),iron);
   Label(root,(18+house*2+(side>0?1:0)).ToString(),o+new Vector3(side*5.30f,y+1.7f,z+3),.018f,Quaternion.Euler(0,side>0?90:-90,0));
   Pot(root,o+new Vector3(side*4.85f,y,z-3.2f),house);
  }
  for(int side=-1;side<=1;side+=2)
  {
   for(int n=0;n<4;n++)
   {
    float z=5+n*20,y=Mathf.Min(z,60)*.1f;
    Line(root,"Concrete_Power_Pole",o+new Vector3(side*4.55f,y,z),o+new Vector3(side*4.55f,y+6.8f,z),.16f,plaster);
    if(n<3)for(int span=0;span<12;span++)
    {
     float za=z+span*20/12f,zb=z+(span+1)*20/12f;
     float ya=Mathf.Min(za,60)*.1f+6.4f-Mathf.Sin(span*Mathf.PI/12)*.55f,yb=Mathf.Min(zb,60)*.1f+6.4f-Mathf.Sin((span+1)*Mathf.PI/12)*.55f;
     Line(root,"Sagging_Service_Cable",o+new Vector3(side*4.55f,ya,za),o+new Vector3(side*4.55f,yb,zb),.018f,iron);
    }
   }
  }
  // Frame the last door as an actual entrance, rather than a freestanding block.
  foreach(int side in new[]{-1,1})Part(root,"Final_Entrance_Pier",o+new Vector3(side*1.05f,7.25f,76.6f),new Vector3(.45f,2.5f,.5f),plaster);
  Part(root,"Final_Entrance_Lintel",o+new Vector3(0,8.65f,76.6f),new Vector3(2.6f,.3f,.6f),plaster);
  Label(root,"NHÀ SỐ 42",o+new Vector3(0,8.6f,76.24f),.035f);
 }
 static void SchoolPass(DieuPhoiHanhTrinh story)
 {
  var root=story.schoolChapter.environment.transform;var o=new Vector3(-95,0,-45);
  foreach(float x in new[]{-31f,-26f,-21f,-16f})
  {
   Part(root,"Covered_Link_Cornice",o+new Vector3(x,3.45f,-6.7f),new Vector3(4.8f,.12f,.35f),wood);
   Pot(root,o+new Vector3(x,0,-9.7f),(int)x);
  }
  for(int i=0;i<9;i++)Part(root,"Archive_Exposed_Ceiling_Beam",o+new Vector3(-22,3.67f,-1+i*1.8f),new Vector3(20,.16f,.12f),wood);
  foreach(float x in new[]{-30f,-14f})
  {
   Part(root,"Archive_Window_Recess",o+new Vector3(x,2.1f,13.81f),new Vector3(2.5f,1.8f,.055f),glass);
   for(int mullion=0;mullion<4;mullion++)Part(root,"Archive_Window_Mullion",o+new Vector3(x-1.2f+mullion*.8f,2.1f,13.72f),new Vector3(.05f,1.8f,.08f),wood);
  }
  Label(root,"KHOA SƯ PHẠM\nLẮNG NGHE · ĐẶT CÂU HỎI",o+new Vector3(4,2.4f,-.18f),.035f);
  var documentTexts=new[]{
   ("THƯ CHƯA GỬI",new Vector3(-27,.82f,2),"Cô Mai,\n\nEm viết thư này sau buổi thực tập đầu tiên. Có một em hỏi tại sao phải chép lại điều đã in trong sách. Em đã bảo em ấy cứ làm như các bạn. Nói xong em mới nhận ra mình từng ghét câu trả lời ấy.\n\nHôm nay em thử để trống mười phút cuối giờ. Không ai hỏi gì. Em thấy sợ khoảng trống đó, nên lại lấy bài tập ra. Có lẽ cho phép người khác hỏi chưa đủ. Mình còn phải chấp nhận không trả lời được ngay.\n\nEm chưa gửi thư. Em không biết đang muốn xin cô tha thứ, hay muốn cô bảo em đã làm đúng."),
   ("BIÊN BẢN HỌP KHOA",new Vector3(-17,.82f,2),"Mục 4: Kiến nghị bổ sung cột nhận xét cho bài làm có cách giải khác.\n\nÝ kiến: biểu mẫu hiện tại thuận tiện cho thống kê. Nếu mở thêm cột, cần thống nhất tiêu chí giữa các lớp. Quyết định: để học kỳ sau.\n\nBên dưới có một trang viết tay, khác màu mực: ‘Tôi đã đề nghị ba lần. Lần thứ tư tôi không đưa tay nữa. Không ai bảo tôi im. Tôi tự hiểu buổi họp sẽ kết thúc sớm hơn.’\n\nNhững trang năm sau vẫn được đóng cùng một loại ghim. Tên người ghi biên bản thay đổi. Dòng quyết định được chép lại nguyên vẹn."),
   ("NHẬT KÝ THỰC TẬP",new Vector3(-27,.82f,7),"Thứ tư. Em học sinh ngồi cuối lớp chọn một nghề mà cả nhà em phản đối. Mình đã định nói ‘cứ nghe bố mẹ trước đi’. Mình dừng lại và hỏi em đã tìm hiểu công việc ấy đến đâu.\n\nEm kể được cả những việc khó chịu nhất của nghề. Mình không biết em sẽ chọn đúng hay sai. Lần đầu tiên trong tuần, mình không cố quyết định thay em.\n\nCuối trang, Minh ghi nhỏ: ‘Khi mình nói một người chưa đủ lớn để tự chọn, mình đang bảo vệ người đó hay đang tránh trách nhiệm phải lắng nghe?’\n\nMột vệt cà phê cắt ngang câu cuối. Trang tiếp theo còn trắng.")
  };
  foreach(var item in documentTexts)Document(story,root,item.Item1,o+item.Item2,item.Item3);
 }
 static void Document(DieuPhoiHanhTrinh story,Transform parent,string heading,Vector3 p,string text)
 {
  var g=Part(parent,"Document_"+heading,p,new Vector3(.48f,.035f,.34f),paper);
  g.AddComponent<BoxCollider>();var read=g.AddComponent<DaoCuKeChuyenV40>();read.story=story;read.heading=heading;read.text=text;
  Label(parent,heading,p+new Vector3(0,.025f,0),.013f,Quaternion.Euler(90,0,0));
 }
 static Bounds Bounds(Transform t)
 {
  var renderers=t.GetComponentsInChildren<Renderer>(true).Where(r=>r.enabled).ToArray();var b=renderers[0].bounds;foreach(var r in renderers)b.Encapsulate(r.bounds);return b;
 }
 static Transform Pivot(Transform part,string name)
 {
  var p=new GameObject(name).transform;p.SetPositionAndRotation(Bounds(part).center,Quaternion.identity);p.SetParent(part.parent,true);part.SetParent(p,true);return p;
 }
 static void Actor(DieuPhoiHanhTrinh story,Transform actor,HienGuongMatTheoKhu face,string speaker)
 {
  // Preserve the existing model/head. Center rotation at the anatomy rather than imported origin.
  if(face.ghostHead.name!="V40_Head_Gaze_Pivot")face.ghostHead=Pivot(face.ghostHead,"V40_Head_Gaze_Pivot");
  var oldPerformance=actor.GetComponent<DienXuatNhanVatV40>();if(oldPerformance)UnityEngine.Object.DestroyImmediate(oldPerformance);
  var oldAnimation=actor.GetComponent<Animation>();if(oldAnimation)UnityEngine.Object.DestroyImmediate(oldAnimation);
  var performance=actor.gameObject.AddComponent<DienXuatNhanVatV40>();performance.story=story;performance.gaze=face.ghostHead;performance.speaker=speaker;
  var animation=actor.gameObject.AddComponent<Animation>();performance.body=animation;
  var limbs=actor.GetComponentsInChildren<MeshRenderer>(true).Where(r=>r.enabled&&(r.name.Contains("Arm")||r.name.Contains("Sleeve"))).Select(r=>Pivot(r.transform,"V40_Elbow_Pivot")).ToArray();
  foreach(var take in new[]{("idle",4f,1.1f),("speak",6f,7f)})
  {
   var clip=new AnimationClip{name=speaker+"_"+take.Item1,legacy=true,wrapMode=WrapMode.Loop};
   foreach(var limb in limbs)
   {
    var path=AnimationUtility.CalculateTransformPath(limb,actor);
    var c=new AnimationCurve();for(int i=0;i<=12;i++)c.AddKey(i*take.Item2/12,Mathf.Sin(i*Mathf.PI/6)*take.Item3);
    clip.SetCurve(path,typeof(Transform),"localEulerAnglesRaw.z",c);
   }
   AssetDatabase.CreateAsset(clip,Art+"/"+speaker.Replace(" ","_")+"_"+take.Item1+"_"+(serial++)+".anim");animation.AddClip(clip,take.Item1);
   if(take.Item1=="idle")animation.clip=clip;
  }
  animation.playAutomatically=true;
 }
 static GameObject Loft(Transform parent,string name,Vector3 origin,float[] y,float[] rx,float[] rz,Material mat)
 {
  const int n=24;var v=new List<Vector3>();var indices=new List<int>();
  for(int row=0;row<y.Length;row++)for(int j=0;j<n;j++){float a=j*Mathf.PI*2/n;v.Add(new Vector3(Mathf.Cos(a)*rx[row],y[row],Mathf.Sin(a)*rz[row]));}
  for(int row=0;row<y.Length-1;row++)for(int j=0;j<n;j++){int a=row*n+j,b=row*n+(j+1)%n;indices.AddRange(new[]{a,a+n,b,b,a+n,b+n});}
  for(int j=1;j<n-1;j++){indices.AddRange(new[]{0,j,j+1});int end=(y.Length-1)*n;indices.AddRange(new[]{end,end+j+1,end+j});}
  var mesh=new Mesh{name=name};mesh.SetVertices(v);mesh.SetTriangles(indices,0);mesh.RecalculateNormals();mesh.RecalculateBounds();AssetDatabase.CreateAsset(mesh,Art+"/"+name+"_"+(serial++)+".asset");
  var g=new GameObject(name);g.transform.position=origin;g.transform.SetParent(parent,true);g.AddComponent<MeshFilter>().sharedMesh=mesh;g.AddComponent<MeshRenderer>().sharedMaterial=mat;return g;
 }
 static void SculptActor(DieuPhoiHanhTrinh story,Transform actor,HienGuongMatTheoKhu face,string speaker)
 {
  if(actor.GetComponentsInChildren<Transform>(true).Any(t=>t.name=="V40_Tailored_Silhouette"))return;
  var bounds=Bounds(actor);var sourceFace=face.ghostHead.GetComponentsInChildren<Renderer>(true).First(r=>r.name.EndsWith("_Face"));var head=sourceFace.bounds;
  var root=new GameObject("V40_Tailored_Silhouette").transform;root.SetParent(actor,true);root.SetPositionAndRotation(new Vector3(head.center.x,bounds.min.y,head.center.z),Quaternion.identity);root.localScale=Vector3.one/Mathf.Abs(actor.lossyScale.x);
  // Cancel the imported root's scale and basis once. Mesh profiles and joint pivots are authored in metres.
  root.SetParent(null,true);root.localScale=Vector3.one;root.SetParent(actor,true);
  var o=root.position;bool female=speaker=="Chị Hà",mai=speaker=="Cô Mai",tung=speaker=="Anh Tùng";
  var fabric=Mat("Cloth_"+speaker,tung?new Color(.16f,.22f,.26f):female?new Color(.13f,.20f,.23f):new Color(.34f,.36f,.31f));
  var trousers=Mat("Trouser_"+speaker,new Color(.09f,.115f,.12f));var skin=Mat("Skin_"+speaker,female||mai?new Color(.48f,.32f,.23f):new Color(.40f,.28f,.20f));var black=Mat("Soft_Black",new Color(.035f,.04f,.038f));
  foreach(var r in face.ghostHead.GetComponentsInChildren<Renderer>(true))if(r.name.EndsWith("_Face")||r.name.EndsWith("_Nose"))r.sharedMaterial=skin;
  foreach(var r in actor.GetComponentsInChildren<Renderer>(true).Where(r=>r.name.Contains("_Torso")||r.name.Contains("_Arm")||r.name.Contains("_Leg")||r.name.Contains("_Shoe")))r.enabled=false;
  if(!mai)
  {
   Loft(root,"Tailored_Jacket",o,new[]{.76f,.83f,.98f,1.18f,1.27f,1.31f},new[]{.185f,.18f,.17f,.225f,.23f,.10f},new[]{.12f,.12f,.125f,.13f,.11f,.07f},fabric);
   Loft(root,"Trouser_Hips",o,new[]{.65f,.78f,.85f},new[]{.16f,.18f,.17f},new[]{.105f,.11f,.10f},trousers);
  }
  foreach(int side in new[]{-1,1})
  {
   if(!mai)
   {
    Loft(root,"Tailored_Sleeve_"+side,o+Vector3.right*side*.27f,new[]{.75f,.86f,1.10f,1.25f},new[]{.055f,.06f,.075f,.08f},new[]{.055f,.06f,.075f,.075f},fabric);
    Loft(root,"Hand_"+side,o+Vector3.right*side*.27f,new[]{.63f,.67f,.75f,.77f},new[]{.022f,.037f,.037f,.025f},new[]{.025f,.033f,.03f,.025f},skin);
    var lapel=Part(root,"Jacket_Lapel",o+new Vector3(side*.075f,1.23f,-.12f),new Vector3(.075f,.20f,.018f),tung?paper:fabric);lapel.transform.rotation=Quaternion.Euler(0,0,side*18);
    Part(root,"Jacket_Pocket_Welt",o+new Vector3(side*.13f,.92f,-.125f),new Vector3(.09f,.018f,.018f),iron);
    for(int button=0;button<5;button++)Part(root,"Tailored_Button",o+new Vector3(0,.86f+button*.075f,-.133f),Vector3.one*.014f,iron,PrimitiveType.Sphere);
   }
   Loft(root,"Trouser_Leg_"+side,o+Vector3.right*side*.10f,new[]{.10f,.16f,.39f,.63f,.75f},new[]{.064f,.06f,.075f,.088f,.09f},new[]{.07f,.07f,.08f,.085f,.09f},trousers);
   Part(root,"Worn_Leather_Shoe",o+new Vector3(side*.10f,.065f,-.035f),new Vector3(.15f,.12f,.28f),black,PrimitiveType.Sphere);
  }
  // Retain the original heads and identity, soften their imported faceted normals.
  foreach(var f in face.ghostHead.GetComponentsInChildren<MeshFilter>(true).Where(f=>f.sharedMesh&&f.GetComponent<Renderer>().enabled))
  {
   var mesh=UnityEngine.Object.Instantiate(f.sharedMesh);var verts=mesh.vertices;var normals=new Vector3[verts.Length];var sums=new Dictionary<Vector3,Vector3>();var triangles=mesh.triangles;
   for(int j=0;j<triangles.Length;j+=3){int a=triangles[j],b=triangles[j+1],c=triangles[j+2];var normal=Vector3.Cross(verts[b]-verts[a],verts[c]-verts[a]);foreach(int index in new[]{a,b,c}){var p=verts[index];sums[p]=sums.TryGetValue(p,out var sum)?sum+normal:normal;}}
   for(int j=0;j<verts.Length;j++)normals[j]=sums.TryGetValue(verts[j],out var sum)?sum.normalized:Vector3.up;
   mesh.normals=normals;AssetDatabase.CreateAsset(mesh,Art+"/HeadDetail_"+(serial++)+".asset");f.sharedMesh=mesh;
  }
  var h=head.center;
  foreach(int side in new[]{-1,1})
  {
   Part(face.ghostHead,"Ear",h+new Vector3(side*.12f,-.005f,0),new Vector3(.045f,.072f,.035f),skin,PrimitiveType.Sphere);
   Line(face.ghostHead,"Brow",h+new Vector3(side*.035f,.064f,-.11f),h+new Vector3(side*.078f,.062f,-.102f),.006f,black);
  }
  Line(face.ghostHead,"Closed_Mouth",h+new Vector3(-.033f,-.064f,-.102f),h+new Vector3(.033f,-.064f,-.102f),.004f,Mat("Lips_"+speaker,new Color(.23f,.13f,.10f)));
  if(tung)
  {
   foreach(var r in face.ghostHead.GetComponentsInChildren<Renderer>(true).Where(r=>r.name.Contains("Hair")))r.enabled=false;
   Part(face.ghostHead,"Tung_Swept_Hair",h+new Vector3(0,.12f,.025f),new Vector3(.25f,.14f,.24f),black,PrimitiveType.Sphere);
   for(int strand=0;strand<3;strand++){var tuft=Part(face.ghostHead,"Tung_Hair_Forelock",h+new Vector3(-.065f+strand*.05f,.115f,-.085f),new Vector3(.09f,.07f,.07f),black,PrimitiveType.Sphere);tuft.transform.rotation=Quaternion.Euler(0,0,18);}
  }
  if(female)foreach(int side in new[]{-1,1})Part(face.ghostHead,"Ha_Shoulder_Length_Hair",h+new Vector3(side*.115f,-.04f,.045f),new Vector3(.075f,.34f,.17f),black,PrimitiveType.Sphere);
  Actor(story,actor,face,speaker);
 }
 public static void ReviewPass()
 {
  EditorSceneManager.OpenScene(ScenePath);serial=1000;var story=UnityEngine.Object.FindAnyObjectByType<DieuPhoiHanhTrinh>();
  plaster=Mat("Ochre_Limewash",new Color(.52f,.43f,.30f));wood=Mat("Dark_Teak",new Color(.23f,.12f,.055f));green=Mat("DaLat_Pine",new Color(.13f,.23f,.16f));iron=Mat("Oxidized_Green",new Color(.14f,.22f,.21f));paper=Mat("Aged_Ivory",new Color(.72f,.65f,.46f));
  SculptActor(story,story.ghosts[0],story.ghosts[0].GetComponent<HienGuongMatTheoKhu>(),"Ông Bảo");SculptActor(story,story.ghosts[1],story.ghosts[1].GetComponent<HienGuongMatTheoKhu>(),"Chị Hà");SculptActor(story,story.schoolChapter.ghost,story.schoolChapter.face,"Cô Mai");SculptActor(story,story.slopeChapter.ghost,story.slopeChapter.face,"Anh Tùng");
  foreach(var puppet in story.puppets)CorrectPuppetRig(puppet);
  clay=Mat("Fired_Terracotta",new Color(.37f,.18f,.105f));HubGarden(story);
  foreach(var r in story.hub.GetComponentsInChildren<Renderer>(true).Where(r=>r.name=="Terracotta_Pot"&&!r.sharedMaterial))r.sharedMaterial=clay;
  foreach(var t in story.hub.GetComponentsInChildren<Transform>(true))
  {
   if(t.name=="Desk_Lamp_Base")WorldScale(t,new Vector3(.22f,.025f,.22f));
   if(t.name=="Desk_Lamp_Stem")WorldScale(t,new Vector3(.025f,.24f,.025f));
   if(t.name=="Desk_Lamp_Shade")WorldScale(t,new Vector3(.25f,.1f,.25f));
   if(t.name=="Tea_Glass")WorldScale(t,new Vector3(.10f,.085f,.10f));
   if(t.name=="Window_Garden_Latch")WorldScale(t,new Vector3(.16f,.12f,.06f));
   if(t.GetComponent<DaoCuKeChuyenV40>()&&t.name.StartsWith("Document_"))WorldScale(t,new Vector3(.48f,.035f,.34f));
   if(t.name=="Journal_Cover")WorldScale(t,new Vector3(.42f,.025f,.28f));
   if(t.name=="Journal_Open_Leaf")WorldScale(t,new Vector3(.20f,.018f,.26f));
  }
  foreach(var light in story.schoolChapter.environment.GetComponentsInChildren<Light>(true)){light.intensity=6;light.range=14;}
  EditorSceneManager.SaveScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene());AssetDatabase.SaveAssets();Inspect();
 }
 static void HubGarden(DieuPhoiHanhTrinh story)
 {
  if(story.hub.transform.Find("Lan_Outside_Garden"))return;
  var root=new GameObject("Lan_Outside_Garden").transform;root.SetParent(story.hub.transform,true);
  var earth=Mat("Garden_Damp_Earth",new Color(.18f,.22f,.15f));var fence=Mat("Lan_Old_Fence",new Color(.27f,.21f,.13f));
  Part(root,"Garden_Terrace",new Vector3(-35,-.15f,-41.5f),new Vector3(8,.25f,8),earth);
  Part(root,"Garden_Retaining_Base",new Vector3(-35,.1f,-42),new Vector3(7,.2f,.3f),plaster);
  for(int pot=0;pot<5;pot++)Pot(root,new Vector3(-38+pot*1.4f,0,-40-pot%2),pot);
  var stages=new List<GameObject>();
  for(int stage=0;stage<4;stage++)
  {
   var group=new GameObject("Lan_Fence_Added_"+stage);group.transform.SetParent(root,true);stages.Add(group);
   for(int slat=0;slat<8;slat++)
   {
    float x=-38.4f+slat*.85f+stage*.18f;
    Part(group.transform,"Garden_Fence_Picket",new Vector3(x,.75f+stage*.08f,-42),new Vector3(.13f,1.5f+stage*.16f,.09f),fence);
   }
   Part(group.transform,"Garden_Fence_Crossrail",new Vector3(-35,.5f+stage*.25f,-42.03f),new Vector3(7,.07f,.07f),fence);
  }
  story.polish.gardenFenceStages=stages.ToArray();
  var latch=Part(story.hub.transform,"Window_Garden_Latch",new Vector3(-35,1.45f,-37.36f),new Vector3(.16f,.12f,.06f),iron);latch.AddComponent<BoxCollider>();
  var prop=latch.AddComponent<DaoCuKeChuyenV40>();prop.story=story;prop.heading="BÊN KIA CỬA SỔ";
  prop.text="Bà Lan ở nhà sát vách vẫn tưới cây vào giờ này.\n\n‘Hồi trước bác tính mở cửa vườn, cho mấy đứa nhỏ sang ngồi đọc sách. Có người bảo để lối đi như thế khó trông nhà lắm. Bác đóng thêm vài thanh thôi.’\n\nQua mỗi lần trở về, Minh nhận ra một khe nhìn nữa đã bị che. Những chậu hoa vẫn ở nguyên chỗ. Không có ai yêu cầu bà Lan đóng khu vườn lại. Từ cửa sổ phòng trọ, cậu vẫn nghe tiếng nước rơi đều lên lá, còn bàn tay người tưới cây thì khuất sau hàng rào.";
 }
 static void CorrectPuppetRig(DieuKhienConRoi puppet)
 {
  if(puppet.transform.Find("V40_Neutral_Articulation_Rig"))return;
  var rig=new GameObject("V40_Neutral_Articulation_Rig").transform;rig.SetParent(puppet.transform,true);rig.rotation=Quaternion.identity;rig.SetParent(null,true);rig.localScale=Vector3.one;rig.SetParent(puppet.transform,true);
  var clip=new AnimationClip{name=puppet.name+"_Corrected_Walk",legacy=true,wrapMode=WrapMode.Loop};
  foreach(var joint in puppet.GetComponentsInChildren<Transform>(true).Where(t=>t.name.StartsWith("V40_Wooden_Joint_")).ToArray())
  {
   joint.SetParent(rig,true);float side=joint.name.EndsWith("L")?1:-1,amplitude=joint.name.Contains("Leg")?12:6;
   var curve=new AnimationCurve();for(int n=0;n<=16;n++)curve.AddKey(n/16f,Mathf.Sin(n*Mathf.PI/8)*amplitude*side);
   clip.SetCurve(AnimationUtility.CalculateTransformPath(joint,puppet.transform),typeof(Transform),"localEulerAnglesRaw.x",curve);
  }
  AssetDatabase.CreateAsset(clip,Art+"/Corrected_Puppet_"+(serial++)+".anim");var animation=puppet.GetComponent<Animation>();animation.RemoveClip("walk");animation.AddClip(clip,"walk");
 }
 static void HubPass(DieuPhoiHanhTrinh story,HoanThienTrinhBayV40 polish)
 {
  var root=story.hub.transform;var top=root.GetComponentsInChildren<Renderer>(true).First(r=>r.name=="CR_Desk_Top").bounds;
  var p=top.center;p.y=top.max.y;
  var lampBase=Part(root,"Desk_Lamp_Base",p+new Vector3(.65f,.025f,.2f),new Vector3(.22f,.025f,.22f),iron,PrimitiveType.Cylinder);
  Line(root,"Desk_Lamp_Stem",lampBase.transform.position,lampBase.transform.position+new Vector3(0,.48f,0),.025f,iron);
  Part(root,"Desk_Lamp_Shade",lampBase.transform.position+new Vector3(0,.48f,0),new Vector3(.25f,.1f,.25f),green,PrimitiveType.Sphere);
  var light=new GameObject("Warm_Study_Lamp").AddComponent<Light>();light.transform.SetParent(root);light.transform.position=lampBase.transform.position+new Vector3(0,.33f,-.05f);light.type=LightType.Point;light.color=new Color(1,.71f,.4f);light.range=4;light.intensity=1.8f;
  Part(root,"Tea_Glass",p+new Vector3(-.65f,.09f,.15f),new Vector3(.10f,.085f,.10f),glass,PrimitiveType.Cylinder);
  Document(story,root,"TỜ GIẤY CỦA MẸ",p+new Vector3(-.55f,.035f,-.2f),"Mẹ để ít tiền trong túi áo. Không nhiều đâu, tháng này mưa quá. Con cứ ăn uống cho đủ.\n\nNếu có chuyện gì khó nói, cứ gọi về. Mẹ có thể không hiểu ngay, nhưng mẹ vẫn muốn nghe.\n\nPhía sau là danh sách tiền trọ, học phí và giờ làm thêm. Minh đã cộng lại ba lần. Kết quả không thay đổi. Dòng ‘gọi về nhà’ không được ghi vào danh sách.");
  var window=root.GetComponentsInChildren<Transform>(true).First(t=>t.name=="CR_Window_Frame");
  var points=new[]{story.presentation.endingPhone.position,story.presentation.endingJournal.position,p+new Vector3(-.5f,.08f,0),window.position};
  polish.endingFocus=points.Select((point,i)=>{var t=new GameObject("Ending_Composition_"+i).transform;t.SetParent(root);t.position=point;return t;}).ToArray();
 }
 static void Puppet(DieuKhienConRoi puppet)
 {
  var limbs=puppet.GetComponentsInChildren<MeshRenderer>(true).Where(r=>r.name=="ConRoi_Leg_L"||r.name=="ConRoi_Leg_R"||r.name=="ConRoi_Arm_L"||r.name=="ConRoi_Arm_R").ToArray();
  var clip=new AnimationClip{name=puppet.name+"_Articulated_Walk",legacy=true,wrapMode=WrapMode.Loop};
  foreach(var limb in limbs)
  {
   if(limb.transform==puppet.gesturePart)continue;
   var pivot=new GameObject("V40_Wooden_Joint_"+limb.name).transform;
   var b=limb.bounds;pivot.position=new Vector3(b.center.x,b.max.y-.025f,b.center.z);pivot.rotation=Quaternion.identity;pivot.SetParent(limb.transform.parent,true);limb.transform.SetParent(pivot,true);
   float side=limb.name.EndsWith("L")?1:-1,amplitude=limb.name.Contains("Leg")?12:6;
   var curve=new AnimationCurve();for(int n=0;n<=16;n++)curve.AddKey(n/16f,Mathf.Sin(n*Mathf.PI/8)*amplitude*side);
   clip.SetCurve(AnimationUtility.CalculateTransformPath(pivot,puppet.transform),typeof(Transform),"localEulerAnglesRaw.x",curve);
  }
  AssetDatabase.CreateAsset(clip,Art+"/Puppet_"+(serial++)+".anim");var animation=puppet.gameObject.AddComponent<Animation>();animation.playAutomatically=false;animation.AddClip(clip,"walk");
  var performance=puppet.gameObject.AddComponent<DienXuatConRoiV40>();performance.puppet=puppet;performance.articulation=animation;
 }
 public static void Assemble()
 {
  if(File.Exists(ScenePath))throw new Exception("v40 scene already authored; refusing overwrite");
  Directory.CreateDirectory(Art);Directory.CreateDirectory(Report);EditorSceneManager.OpenScene(ConRoiSlopeV39.ScenePath);
  var story=UnityEngine.Object.FindAnyObjectByType<DieuPhoiHanhTrinh>();var polish=story.gameObject.AddComponent<HoanThienTrinhBayV40>();polish.story=story;polish.eye=story.player.GetComponentInChildren<Camera>();story.polish=polish;
  story.player.speed=3.6f; // Normal walking pace; do not extend scenes with forced idle time.
  polish.eye.gameObject.AddComponent<GhiNhanTraiNghiemV40>();
  plaster=Mat("Ochre_Limewash",new Color(.52f,.43f,.30f));wood=Mat("Dark_Teak",new Color(.23f,.12f,.055f));green=Mat("DaLat_Pine",new Color(.13f,.23f,.16f));iron=Mat("Oxidized_Green",new Color(.14f,.22f,.21f));clay=Mat("Fired_Terracotta",new Color(.37f,.18f,.105f));glass=Mat("Smoky_Window_Glass",new Color(.13f,.21f,.23f),.22f);paper=Mat("Aged_Ivory",new Color(.72f,.65f,.46f));
  SurfacePass(story);var cone=Cone("Pine_Layer_Conical");GardenPass(story.schoolChapter.environment.transform,cone);GardenPass(story.slopeChapter.environment.transform,cone);
  SlopePass(story);SchoolPass(story);HubPass(story,polish);
  Actor(story,story.ghosts[0],story.ghosts[0].GetComponent<HienGuongMatTheoKhu>(),"Ông Bảo");
  Actor(story,story.ghosts[1],story.ghosts[1].GetComponent<HienGuongMatTheoKhu>(),"Chị Hà");
  Actor(story,story.schoolChapter.ghost,story.schoolChapter.face,"Cô Mai");
  Actor(story,story.slopeChapter.ghost,story.slopeChapter.face,"Anh Tùng");
  foreach(var puppet in story.puppets)Puppet(puppet);
  polish.pageTurn=AssetDatabase.LoadAssetAtPath<AudioClip>(Art+"/PageTurn.wav");polish.receiver=AssetDatabase.LoadAssetAtPath<AudioClip>(Art+"/Receiver.wav");
  EditorSceneManager.SaveScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene(),ScenePath);AssetDatabase.SaveAssets();Inspect();
 }
 public static void Inspect()
 {
  var scene=UnityEngine.SceneManagement.SceneManager.GetActiveScene();var all=scene.GetRootGameObjects().SelectMany(g=>g.GetComponentsInChildren<Transform>(true)).ToArray();var story=UnityEngine.Object.FindAnyObjectByType<DieuPhoiHanhTrinh>();
  File.WriteAllText(Report+"/render-inventory.json",new JArray(all.Select(t=>t.GetComponent<MeshRenderer>()).Where(r=>r).Select(r=>new JObject{["name"]=r.name,["position"]=r.bounds.center.ToString("F2"),["size"]=r.bounds.size.ToString("F2"),["materials"]=new JArray(r.sharedMaterials.Select(m=>m?m.name:"MISSING"))})).ToString());
  File.WriteAllText(Report+"/interaction-geometry.json",new JObject{["audio_listeners"]=new JArray(all.Where(t=>t.GetComponent<AudioListener>()).Select(t=>t.name)),["documents"]=new JArray(all.Select(t=>t.GetComponent<DaoCuKeChuyenV40>()).Where(t=>t).Select(t=>new JObject{["heading"]=t.heading,["position"]=t.transform.position.ToString("F4"),["bounds"]=t.GetComponent<Collider>().bounds.ToString("F4")}))}.ToString());
  var arm=story.schoolChapter.puppet.gesturePart;var rest=arm.localRotation;var before=Bounds(arm);arm.localRotation=rest*Quaternion.Euler(-30,0,-25);var posed=Bounds(arm);arm.localRotation=rest;
  File.WriteAllText(Report+"/GD3-pose-geometry.json",new JObject{["arm"]=arm.name,["pivot_world"]=arm.position.ToString("F5"),["rest_bounds"]=before.ToString("F5"),["posed_bounds"]=posed.ToString("F5"),["center_displacement_m"]=(posed.center-before.center).magnitude}.ToString());
  File.WriteAllText(Report+"/scene-integrity.json",new JObject{["scene"]=scene.path,["missing_scripts"]=all.Sum(t=>GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(t.gameObject)),["renderers"]=all.Count(t=>t.GetComponent<Renderer>()),["narrative_documents"]=all.Count(t=>t.GetComponent<DaoCuKeChuyenV40>()),["actor_performances"]=all.Count(t=>t.GetComponent<DienXuatNhanVatV40>()),["authored_clips"]=AssetDatabase.FindAssets("t:AnimationClip",new[]{Art}).Length,["source_models_preserved"]=true,["walk_speed"]=story.player.speed,["note"]="Quality requires rendered walkthrough; this inventory is not acceptance."}.ToString());
  File.WriteAllText(Report+"/actor-geometry.json",new JArray(all.Select(t=>t.GetComponent<HienGuongMatTheoKhu>()).Where(f=>f).Select(f=>new JObject{["actor"]=f.name,["head_world"]=f.ghostHead.position.ToString("F5"),["head_rotation"]=f.ghostHead.eulerAngles.ToString("F2"),["parts"]=new JArray(f.ghostHead.GetComponentsInChildren<Renderer>(true).Select(r=>new JObject{["part"]=r.name,["center"]=r.bounds.center.ToString("F5"),["size"]=r.bounds.size.ToString("F5")}))})).ToString());
 }
 public static void BuildWindows()
 {
  EditorSceneManager.OpenScene(ScenePath);Inspect();EditorBuildSettings.scenes=new[]{new EditorBuildSettingsScene(ScenePath,true)};
  PlayerSettings.bundleVersion="0.40.0";PlayerSettings.runInBackground=true;
  // Reproduce the v39 D3D12 warning in a control run, then compare the same
  // runtime on D3D11. Do not claim this setting alone proves a root cause.
  PlayerSettings.SetUseDefaultGraphicsAPIs(BuildTarget.StandaloneWindows64,false);
  PlayerSettings.SetGraphicsAPIs(BuildTarget.StandaloneWindows64,new[]{UnityEngine.Rendering.GraphicsDeviceType.Direct3D11});
  var report=BuildPipeline.BuildPlayer(new[]{ScenePath},"Builds/Windows_v40_r6/ConRoi.exe",BuildTarget.StandaloneWindows64,BuildOptions.Development);
  File.WriteAllText(Report+"/build.json",new JObject{["result"]=report.summary.result.ToString(),["errors"]=report.summary.totalErrors,["warnings"]=report.summary.totalWarnings,["bytes"]=report.summary.totalSize,["path"]=report.summary.outputPath,["scene"]=ScenePath,["development"]=true,["graphics_api"]="Direct3D11"}.ToString());
 }
}
