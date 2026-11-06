using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public static class ConRoiCompletionV41
{
 public const string ScenePath="Assets/Scenes/JourneyExperience_v41.unity",Report="Docs/Reviews/journey_v41";
 const string Art="Assets/Art/Environment/JourneyV41";
 static Material wood,clay,green,ivory,cloth,iron,skin,ink;
 static Material Mat(string name,Color color)
 {
  var m=new Material(Shader.Find("Universal Render Pipeline/Lit")){name=name,color=color};m.SetFloat("_Smoothness",.15f);AssetDatabase.CreateAsset(m,Art+"/"+name+".mat");return m;
 }
 static GameObject Part(Transform parent,string name,Vector3 position,Vector3 size,Material material,PrimitiveType shape=PrimitiveType.Cube)
 {
  var g=GameObject.CreatePrimitive(shape);g.name=name;g.layer=3;g.transform.SetParent(parent,true);g.transform.position=position;
  var scale=parent.lossyScale;g.transform.localScale=new Vector3(size.x/Mathf.Abs(scale.x),size.y/Mathf.Abs(scale.y),size.z/Mathf.Abs(scale.z));
  g.GetComponent<Renderer>().sharedMaterial=material;UnityEngine.Object.DestroyImmediate(g.GetComponent<Collider>());return g;
 }
 static void Rod(Transform parent,string name,Vector3 a,Vector3 b,float width,Material m)
 {var g=Part(parent,name,(a+b)*.5f,new Vector3(width,(b-a).magnitude*.5f,width),m,PrimitiveType.Cylinder);g.transform.up=b-a;}
 static void Text(Transform parent,string text,Vector3 p,float size=.035f)
 {
  var t=new GameObject("Hand_Painted_"+text).AddComponent<TextMesh>();t.transform.SetParent(parent,true);t.transform.position=p;t.text=text;t.characterSize=size;t.fontSize=64;t.anchor=TextAnchor.MiddleCenter;t.color=new Color(.76f,.72f,.57f);
 }
 static Transform Pivot(Transform part,string name)
 {
  var r=part.GetComponent<Renderer>();var b=r.bounds;var p=new GameObject(name).transform;p.position=new Vector3(b.center.x,b.max.y-.02f,b.center.z);p.rotation=Quaternion.identity;p.SetParent(part.parent,true);part.SetParent(p,true);return p;
 }
 static Light Light(Transform parent,Vector3 p,float intensity,float range,Color color)
 {var l=new GameObject("V41_Practical").AddComponent<Light>();l.transform.SetParent(parent,true);l.transform.position=p;l.type=LightType.Point;l.intensity=intensity;l.range=range;l.color=color;return l;}
 static Transform Arm(Transform actor)=>actor.GetComponentsInChildren<Transform>(true).First(t=>t.GetComponent<Renderer>()&&(t.name.EndsWith("_Arm")||t.name=="ConRoi_Arm_L"));
 static void Basket(Transform parent,Vector3 p,int seed)
 {
  Part(parent,"Woven_Rim",p+Vector3.up*.22f,new Vector3(.5f,.025f,.5f),wood,PrimitiveType.Cylinder);
  Part(parent,"Woven_Basket",p+Vector3.up*.12f,new Vector3(.46f,.11f,.46f),clay,PrimitiveType.Cylinder);
  for(int n=0;n<14;n++)
  {
   float a=n*Mathf.PI*2/14;Rod(parent,"Split_Bamboo",p+new Vector3(Mathf.Cos(a)*.2f,.02f,Mathf.Sin(a)*.2f),p+new Vector3(Mathf.Cos(a)*.24f,.23f,Mathf.Sin(a)*.24f),.009f,wood);
  }
  for(int n=0;n<9;n++){float a=n*2.4f;Part(parent,seed%2==0?"DaLat_Artichoke":"Persimmon",p+new Vector3(Mathf.Sin(a)*.15f,.26f+(n%2)*.035f,Mathf.Cos(a)*.15f),new Vector3(.1f,.09f,.1f),seed%2==0?green:clay,PrimitiveType.Sphere);}
 }
 static void RouteMaterials()
 {
  var map=new Dictionary<Material,Material>();var catalog=JObject.Parse(File.ReadAllText("Assets/Art/Environment/LakeGateMarketV33/surfaces.json"))["meshes"].ToDictionary(x=>(string)x["mesh"],x=>(string)x["material"]);
  foreach(var renderer in UnityEngine.Object.FindObjectsByType<MeshRenderer>(FindObjectsInactive.Include,FindObjectsSortMode.None))
  {
   var materials=renderer.sharedMaterials;
   for(int i=0;i<materials.Length;i++)
   {
    var old=materials[i];if(!old||!catalog.TryGetValue(old.name,out string source))continue;
    bool block=source.StartsWith("Building_")||source.Contains("Placeholder")||source.Contains("Unresolved")||source=="Terrain_Flat";
    if(!block)continue;
    if(!map.TryGetValue(old,out var replacement))
    {
     Color c=source.Contains("Terrain")?new Color(.18f,.24f,.16f):source.Contains("Recess")?new Color(.10f,.14f,.135f):source.Contains("BlueGrey")?new Color(.34f,.40f,.39f):source.Contains("Purple")?new Color(.48f,.47f,.37f):new Color(.64f,.56f,.40f);
     replacement=new Material(Shader.Find("ConRoi/V41/WeatheredLime")){name=source+"_V41"};replacement.SetColor("_BaseColor",c);AssetDatabase.CreateAsset(replacement,Art+"/"+old.name+"_Lime.mat");map.Add(old,replacement);
    }
    materials[i]=replacement;
   }
   renderer.sharedMaterials=materials;
  }
  File.WriteAllText(Report+"/material-replacements.json",new JArray(map.Select(x=>new JObject{["from"]=x.Key.name,["to"]=x.Value.name})).ToString());
 }
 static void Lake(DieuPhoiHanhTrinh story,HoanTatHanhTrinhV41 c)
 {
  var root=new GameObject("V41_Lake_Details").transform;root.SetParent(story.lake.transform,true);
  var bao=story.ghosts[0];var baseY=bao.GetComponentsInChildren<Renderer>(true).Min(r=>r.bounds.min.y);
  var p=bao.position;p.y=baseY;
  Part(root,"Bao_Fishing_Bag",p+new Vector3(.7f,.2f,.25f),new Vector3(.42f,.36f,.28f),cloth);
  Rod(root,"Bamboo_Fishing_Rod",p+new Vector3(-.32f,.8f,-.1f),p+new Vector3(-1.2f,1.2f,-2.7f),.018f,wood);
  Rod(root,"Fishing_Line",p+new Vector3(-1.2f,1.2f,-2.7f),p+new Vector3(-1.25f,-.1f,-2.75f),.0025f,ink);
  var thermos=Part(root,"Enamel_Thermos",p+new Vector3(.65f,.23f,-.25f),new Vector3(.15f,.23f,.15f),ivory,PrimitiveType.Cylinder);
  Part(root,"Thermos_Lid",thermos.transform.position+Vector3.up*.24f,new Vector3(.16f,.025f,.16f),iron,PrimitiveType.Cylinder);
  var calendar=story.traces[0];var b=calendar.GetComponent<Renderer>().bounds;
  for(int n=0;n<5;n++)Part(root,"Unturned_Calendar_Edges",b.center+new Vector3(.008f*n,-.008f*n,.015f+n*.003f),new Vector3(b.size.x,b.size.y,.002f),ivory);
  c.lakeReflection=Light(root,p+new Vector3(0,1.5f,-1),.4f,5,new Color(.55f,.69f,.66f));
  // Preserve the authored route/colliders. Plant texture and belongings clarify this stop.
  for(int n=0;n<8;n++)
  {
   Vector3 leaf=p+new Vector3(2.5f+n*.35f,.02f,.9f+Mathf.Sin(n)*.3f);
   for(int j=0;j<3;j++)Rod(root,"Lakeside_Grass",leaf,leaf+new Vector3((j-1)*.14f,.28f+(n%3)*.1f,.1f),.012f,green);
  }
 }
 static void Market(DieuPhoiHanhTrinh story,HoanTatHanhTrinhV41 c)
 {
  var root=new GameObject("V41_Market_Craft").transform;root.SetParent(story.market.transform,true);
  var ha=story.ghosts[1];float floor=ha.GetComponentsInChildren<Renderer>(true).Min(r=>r.bounds.min.y);var p=ha.position;p.y=floor;
  // Repetition belongs to the market theme; varied local goods replace empty counters.
  for(int n=0;n<5;n++)
  {
   Vector3 table=p+new Vector3((n-2)*1.65f,.65f,1.35f);
   Part(root,"Weathered_Stall_Counter",table,new Vector3(1.45f,.09f,.7f),wood);
   for(int k=-1;k<=1;k+=2)Part(root,"Counter_Leg",table+new Vector3(k*.55f,-.32f,0),new Vector3(.07f,.64f,.6f),iron);
   Basket(root,table+new Vector3(-.32f,.05f,0),n);Basket(root,table+new Vector3(.32f,.05f,0),n+1);
   Rod(root,"Awning_Standard",table+new Vector3(-.7f,-.65f,.3f),table+new Vector3(-.7f,2,.3f),.025f,iron);
   var awning=Part(root,"Worn_Ochre_Canvas",table+new Vector3(0,1.9f,0),new Vector3(1.6f,.035f,1.2f),cloth);awning.transform.rotation=Quaternion.Euler(-8,0,0);
   Part(root,"Canvas_Valance",table+new Vector3(0,1.73f,-.58f),new Vector3(1.6f,.18f,.025f),cloth);
   var bulb=Part(root,"Warm_Stall_Bulb",table+new Vector3(0,1.6f,0),Vector3.one*.07f,ivory,PrimitiveType.Sphere);
   Light(root,bulb.transform.position,3,6,new Color(1,.75f,.46f));
  }
  Text(root,"ATISÔ  ·  HỒNG ĐÀ LẠT",p+new Vector3(0,2.12f,.74f),.055f);
  Part(root,"Painted_Sign_Backboard",p+new Vector3(0,2.12f,.77f),new Vector3(3.8f,.40f,.045f),iron);
  var books=story.traces[1];c.bookLight=Light(root,books.position+new Vector3(0,.8f,-.5f),.3f,3,new Color(.7f,.79f,.76f));
  for(int n=0;n<3;n++)
  {
   var pos=books.position+new Vector3((n-1)*.18f,.05f,-.03f);
   Rod(root,"Unbroken_Parcel_Cord",pos+new Vector3(-.05f,0,-.14f),pos+new Vector3(-.05f,0,.14f),.004f,ivory);
  }
  Text(root,"GIÁO TRÌNH",books.position+new Vector3(0,.09f,-.12f),.018f);
  // Ordinary classmate, seen side-on between goods. It never uses face projection.
  var vy=CloneActor(story.ghosts[1],root,"Vy_Ordinary_Classmate");var bounds=vy.GetComponentsInChildren<Renderer>(true).Select(r=>r.bounds).Aggregate((a,b)=>{a.Encapsulate(b);return a;});
  vy.position+=p+new Vector3(-3.1f,0,-3.5f)-new Vector3(bounds.center.x,bounds.min.y,bounds.center.z);vy.rotation=Quaternion.Euler(0,95,0);c.vy=vy;vy.gameObject.SetActive(false);
 }
 static Transform CloneActor(Transform source,Transform parent,string name)
 {
  var t=UnityEngine.Object.Instantiate(source.gameObject,parent,true).transform;t.name=name;
  foreach(var comp in t.GetComponentsInChildren<MonoBehaviour>(true))UnityEngine.Object.DestroyImmediate(comp);
  foreach(var collider in t.GetComponentsInChildren<Collider>(true))UnityEngine.Object.DestroyImmediate(collider);
  var animation=t.GetComponent<Animation>();if(animation)UnityEngine.Object.DestroyImmediate(animation);
  var all=t.GetComponentsInChildren<Renderer>(true);var b=all[0].bounds;foreach(var r in all)b.Encapsulate(r.bounds);
  var wrapper=new GameObject(name+"_World_Rig").transform;wrapper.SetParent(parent,true);wrapper.position=new Vector3(b.center.x,b.min.y,b.center.z);t.SetParent(wrapper,true);
  return wrapper;
 }
 static void Garden(DieuPhoiHanhTrinh story,HoanTatHanhTrinhV41 c)
 {
  var root=new GameObject("V41_Lan_Garden_Performance").transform;root.SetParent(story.hub.transform,true);
  var lan=CloneActor(story.ghosts[1],root,"Lan_Garden");var all=lan.GetComponentsInChildren<Renderer>(true);var b=all[0].bounds;foreach(var r in all)b.Encapsulate(r.bounds);
  lan.position+=new Vector3(-35,0,-40.3f)-new Vector3(b.center.x,b.min.y,b.center.z);lan.rotation=Quaternion.Euler(0,15,0);c.lan=lan;c.lanArm=Pivot(Arm(lan),"Lan_Watering_Shoulder");
  var hand=c.lanArm.GetComponentsInChildren<Renderer>().First().bounds.center+Vector3.down*.24f;
  Part(c.lanArm,"Watering_Can",hand+Vector3.forward*.12f,new Vector3(.26f,.16f,.16f),iron,PrimitiveType.Cylinder);
  Rod(c.lanArm,"Watering_Spout",hand+new Vector3(.1f,.05f,.12f),hand+new Vector3(.35f,0,.12f),.025f,iron);
  var old=story.hub.GetComponentInChildren<TuongTacVuonV41>(true);
  if(old){old.chapter=c;old.transform.position=new Vector3(-35.75f,1.45f,-37.36f);}
  else
  {
   var latch=story.hub.GetComponentsInChildren<DaoCuKeChuyenV40>(true).First(p=>p.heading.Contains("CỬA SỔ"));
   var interaction=latch.gameObject.AddComponent<TuongTacVuonV41>();interaction.chapter=c;UnityEngine.Object.DestroyImmediate(latch);
  }
 }
 static void PuppetRig(DieuKhienConRoi puppet)
 {
  // Move the authored leading arm to an anatomical shoulder. Keep the wood mesh.
  var arm=puppet.gesturePart;if(!arm||!arm.GetComponent<Renderer>())return;
  puppet.gesturePart=Pivot(arm,"V41_Leading_Shoulder");
  var root=puppet.transform;
  foreach(var r in root.GetComponentsInChildren<MeshRenderer>(true).Where(r=>r.name.Contains("ConRoi_Arm")||r.name.Contains("ConRoi_Leg")).ToArray())
  {
   var b=r.bounds;var joint=Part(r.transform,"Carved_Joint_Cap",new Vector3(b.center.x,b.max.y-.02f,b.center.z),Vector3.one*.085f,wood,PrimitiveType.Sphere);
  }
 }
 static void SculptPuppets(DieuPhoiHanhTrinh story)
 {
  string path=Art+"/Carved_Tapered_Torso.asset";var mesh=AssetDatabase.LoadAssetAtPath<Mesh>(path);
  if(!mesh)
  {
   var vertices=new List<Vector3>();var tris=new List<int>();
   Vector2[] corners={new Vector2(-.38f,-.5f),new Vector2(.38f,-.5f),new Vector2(.5f,-.36f),new Vector2(.5f,.36f),new Vector2(.38f,.5f),new Vector2(-.38f,.5f),new Vector2(-.5f,.36f),new Vector2(-.5f,-.36f)};
   float[] heights={-.5f,-.42f,.34f,.5f};float[] widths={.78f,.84f,1,.90f};
   for(int row=0;row<3;row++)for(int side=0;side<8;side++)
   {
    int k=vertices.Count,next=(side+1)%8;
    vertices.Add(new Vector3(corners[side].x*widths[row],heights[row],corners[side].y));vertices.Add(new Vector3(corners[next].x*widths[row],heights[row],corners[next].y));vertices.Add(new Vector3(corners[next].x*widths[row+1],heights[row+1],corners[next].y));vertices.Add(new Vector3(corners[side].x*widths[row+1],heights[row+1],corners[side].y));
    tris.AddRange(new[]{k,k+2,k+1,k,k+3,k+2});
   }
   mesh=new Mesh{name="Carved_Tapered_Torso"};mesh.SetVertices(vertices);mesh.SetTriangles(tris,0);mesh.SetUVs(0,vertices.Select(v=>new Vector2(v.x+.5f,v.y+.5f)).ToList());mesh.RecalculateNormals();mesh.RecalculateBounds();AssetDatabase.CreateAsset(mesh,path);
  }
  foreach(var puppet in story.puppets.Concat(new[]{story.schoolChapter.puppet,story.slopeChapter.puppet}).Distinct())
  {
   if(!puppet.GetComponent<TiepDatConRoiV41>())puppet.gameObject.AddComponent<TiepDatConRoiV41>();
   // Imported elbow/knee meshes were siblings of animated limbs. Bind them to
   // the retained segment so the shoulder lead cannot leave a floating joint.
   var transforms=puppet.GetComponentsInChildren<Transform>(true);
   foreach(string side in new[]{"L","R"})foreach(string limb in new[]{"Arm","Leg"})
   {
    var segment=transforms.FirstOrDefault(t=>t.name=="ConRoi_"+limb+"_"+side);
    var joint=transforms.FirstOrDefault(t=>t.name=="ConRoi_Joint_"+(limb=="Arm"?"Elbow":"Knee")+"_"+side);
    if(segment&&joint)joint.SetParent(segment,true);
   }
   if(puppet.transform.Find("V41_Carved_Torso"))continue;
   var old=puppet.GetComponentsInChildren<MeshRenderer>(true).First(r=>r.name=="ConRoi_Body_Wood");var b=old.bounds;old.enabled=false;
   var g=new GameObject("V41_Carved_Torso");g.transform.SetParent(puppet.transform,true);g.transform.position=b.center;g.transform.rotation=Quaternion.identity;var s=puppet.transform.lossyScale;g.transform.localScale=new Vector3(b.size.x/s.x,b.size.y/s.y,b.size.z/s.z);g.AddComponent<MeshFilter>().sharedMesh=mesh;g.AddComponent<MeshRenderer>().sharedMaterial=wood;
   for(int n=-1;n<=1;n+=2)
   {
    var nail=Part(g.transform,"Dark_Wooden_Pin",b.center+new Vector3(n*.10f,b.size.y*.33f,-b.size.z*.51f),new Vector3(.012f,.006f,.012f),ink,PrimitiveType.Cylinder);nail.transform.rotation=Quaternion.Euler(90,0,0);
   }
  }
 }
 static void Landscape(DieuPhoiHanhTrinh story)
 {
  if(story.lake.transform.Find("V41_Lake_Pine_Grove"))return;
  var root=new GameObject("V41_Lake_Pine_Grove").transform;root.SetParent(story.lake.transform,true);var cone=AssetDatabase.LoadAssetAtPath<Mesh>("Assets/Art/Environment/JourneyV40/Pine_Layer_Conical.asset");
  var random=new System.Random(41);Physics.SyncTransforms();
  var origin=story.ghosts[0].position;
  foreach(var offset in new[]{new Vector3(-14,0,8),new Vector3(-8,0,10),new Vector3(-2,0,9),new Vector3(5,0,12),new Vector3(13,0,11),new Vector3(21,0,14),new Vector3(29,0,15),new Vector3(38,0,17)})
  {
   Vector3 p=origin+offset;if(Physics.Raycast(p+Vector3.up*35,Vector3.down,out var hit,80,~0,QueryTriggerInteraction.Ignore))p.y=hit.point.y;else continue;
   float height=5.8f+(float)random.NextDouble()*2.8f;
   Rod(root,"DaLat_Pine_Trunk",p,p+new Vector3(.2f,height,0),.12f,wood);
   for(int tier=0;tier<4;tier++)for(int branch=0;branch<4;branch++)
   {
    float a=branch*Mathf.PI*.5f+tier*.55f+(float)random.NextDouble()*.3f,radius=1.5f-tier*.24f;
    Vector3 start=p+Vector3.up*(height*.38f+tier*.95f),end=start+new Vector3(Mathf.Cos(a)*radius,.35f,Mathf.Sin(a)*radius);
    Rod(root,"Pine_Bough",start,end,.035f,wood);
    var needles=Part(root,"Pine_Needle_Fan",end-Vector3.up*.12f,new Vector3(1.6f-tier*.18f,.95f,1.45f-tier*.15f),green);needles.GetComponent<MeshFilter>().sharedMesh=cone;
   }
  }
 }
 public static void Assemble()
 {
  if(File.Exists(ScenePath))throw new Exception("v41 exists; checkpoint not overwritten");
  Directory.CreateDirectory(Art);Directory.CreateDirectory(Report);EditorSceneManager.OpenScene(ConRoiFinishV40.ScenePath);
  CaptureViews("before");
  wood=Mat("Aged_Bamboo_Teak",new Color(.28f,.19f,.095f));clay=Mat("Persimmon_Terracotta",new Color(.52f,.25f,.10f));green=Mat("Artichoke_Green",new Color(.22f,.34f,.18f));ivory=Mat("Aged_Paper_Enamel",new Color(.70f,.66f,.48f));cloth=Mat("Ochre_Canvas",new Color(.43f,.35f,.20f));iron=Mat("Worn_Green_Metal",new Color(.10f,.19f,.17f));skin=Mat("Warm_Skin",new Color(.55f,.39f,.26f));ink=Mat("Charcoal",new Color(.045f,.06f,.055f));
  var story=UnityEngine.Object.FindAnyObjectByType<DieuPhoiHanhTrinh>();var c=story.gameObject.AddComponent<HoanTatHanhTrinhV41>();c.story=story;story.completion=c;
  RouteMaterials();Lake(story,c);Market(story,c);Garden(story,c);
  c.baoArm=Pivot(Arm(story.ghosts[0]),"Bao_Rod_Shoulder");c.haArm=Pivot(Arm(story.ghosts[1]),"Ha_Sorting_Shoulder");
  foreach(var puppet in story.puppets.Concat(new[]{story.schoolChapter.puppet,story.slopeChapter.puppet}).Distinct())PuppetRig(puppet);
  c.traceDetails=new[]{story.traces[0],story.traces[1],story.schoolChapter.chair,story.slopeChapter.suitcase};
  c.paperRustle=AssetDatabase.LoadAssetAtPath<AudioClip>(Art+"/PaperRustle.wav");c.cloth=AssetDatabase.LoadAssetAtPath<AudioClip>(Art+"/ParcelCloth.wav");c.watering=AssetDatabase.LoadAssetAtPath<AudioClip>(Art+"/Watering.wav");
  EditorSceneManager.SaveScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene(),ScenePath);AssetDatabase.SaveAssets();CaptureViews("after");
  File.WriteAllText(Report+"/authoring.json",new JObject{["scene"]=ScenePath,["new_renderers"]=UnityEngine.Object.FindObjectsByType<MeshRenderer>(FindObjectsInactive.Include,FindObjectsSortMode.None).Count(r=>r.name.Contains("Woven")||r.name.Contains("Carved")),["source_models_modified"]=false,["new_awareness_traces"]=0,["new_progression_locks"]=0}.ToString());
 }
 public static void CaptureViews(string suffix)
 {
  var story=UnityEngine.Object.FindAnyObjectByType<DieuPhoiHanhTrinh>();var camera=story.player.GetComponentInChildren<Camera>();
  if(story.polish.gardenFenceStages!=null)for(int k=0;k<story.polish.gardenFenceStages.Length;k++)story.polish.gardenFenceStages[k].SetActive(k==0);
  var positions=new[]{story.ghosts[0].position+new Vector3(-2,1,-3),story.ghosts[1].position+new Vector3(-4,1,-5),story.schoolChapter.puppet.dauTransform.position+new Vector3(0,0,-4),new Vector3(-35,1.6f,-36.5f)};
  var targets=new[]{story.ghosts[0].position+Vector3.up,story.ghosts[1].position+Vector3.up,story.schoolChapter.puppet.dauTransform.position,new Vector3(-35,1,-40)};
  var oldPosition=camera.transform.position;var oldRotation=camera.transform.rotation;
  var objects=new[]{story.hub,story.lake,story.market,story.schoolChapter.environment};var active=objects.Select(o=>o.activeSelf).ToArray();
  var rt=RenderTexture.GetTemporary(1280,720,24,RenderTextureFormat.ARGB32);var tex=new Texture2D(1280,720,TextureFormat.RGB24,false);
  Directory.CreateDirectory(Report+"/comparison");
  for(int n=0;n<4;n++)
  {
   for(int j=0;j<objects.Length;j++)objects[j].SetActive(j==(n==0?1:n==1?2:n==2?3:0));
   camera.transform.SetPositionAndRotation(positions[n],Quaternion.LookRotation(targets[n]-positions[n]));
   var arm=story.schoolChapter.puppet.gesturePart;var rest=arm.localRotation;
   if(n==2)arm.localRotation=rest*Quaternion.Euler(story.completion?-62:-30,0,story.completion?-48:-25);
   RenderPipeline.SubmitRenderRequest(camera,new UniversalRenderPipeline.SingleCameraRequest{destination=rt});
   var old=RenderTexture.active;RenderTexture.active=rt;tex.ReadPixels(new Rect(0,0,1280,720),0,0);tex.Apply();RenderTexture.active=old;
   File.WriteAllBytes(Report+"/comparison/"+new[]{"lake","market","GD3","garden"}[n]+"_"+suffix+".png",tex.EncodeToPNG());
   arm.localRotation=rest;
  }
  for(int j=0;j<objects.Length;j++)objects[j].SetActive(active[j]);camera.transform.SetPositionAndRotation(oldPosition,oldRotation);RenderTexture.ReleaseTemporary(rt);UnityEngine.Object.DestroyImmediate(tex);
 }
 public static void Comparison()
 {
  EditorSceneManager.OpenScene(ConRoiFinishV40.ScenePath);CaptureViews("before");
  EditorSceneManager.OpenScene(ScenePath);CaptureViews("after");
 }
 public static void Refine()
 {
  EditorSceneManager.OpenScene(ScenePath);var story=UnityEngine.Object.FindAnyObjectByType<DieuPhoiHanhTrinh>();var c=story.completion;
  wood=AssetDatabase.LoadAssetAtPath<Material>(Art+"/Aged_Bamboo_Teak.mat");clay=AssetDatabase.LoadAssetAtPath<Material>(Art+"/Persimmon_Terracotta.mat");green=AssetDatabase.LoadAssetAtPath<Material>(Art+"/Artichoke_Green.mat");ivory=AssetDatabase.LoadAssetAtPath<Material>(Art+"/Aged_Paper_Enamel.mat");cloth=AssetDatabase.LoadAssetAtPath<Material>(Art+"/Ochre_Canvas.mat");iron=AssetDatabase.LoadAssetAtPath<Material>(Art+"/Worn_Green_Metal.mat");ink=AssetDatabase.LoadAssetAtPath<Material>(Art+"/Charcoal.mat");
  wood.SetTexture("_BaseMap",AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Art/Environment/JourneyV40/TimberGrain.png"));wood.SetTextureScale("_BaseMap",new Vector2(2,3));EditorUtility.SetDirty(wood);
  foreach(var name in new[]{"V41_Lake_Details","V41_Market_Craft","V41_Lan_Garden_Performance"})
  {var t=UnityEngine.Object.FindObjectsByType<Transform>(FindObjectsInactive.Include,FindObjectsSortMode.None).First(x=>x.name==name);UnityEngine.Object.DestroyImmediate(t.gameObject);}
  foreach(var t in story.ghosts[0].GetComponentsInChildren<Transform>(true).Where(t=>t.name=="Bamboo_Fishing_Rod"||t.name=="Fishing_Line").ToArray())UnityEngine.Object.DestroyImmediate(t.gameObject);
  Lake(story,c);Market(story,c);Garden(story,c);
  SculptPuppets(story);
  int row=0;foreach(var puppet in story.puppets.Where(p=>p.transform.IsChildOf(story.market.transform)))
  {
   var spawn=story.marketSpawn.position;puppet.transform.position=new Vector3(spawn.x-3+row*1.4f,puppet.transform.position.y,spawn.z-5-row*.18f);
   if(!puppet.transform.Find("V41_Market_Facing")){puppet.transform.Rotate(Vector3.up,180,Space.World);var marker=new GameObject("V41_Market_Facing");marker.transform.SetParent(puppet.transform,false);}
   row++;
  }
  if(!story.market.transform.Find("V41_Entrance_Lamp"))
  {
   var root=new GameObject("V41_Entrance_Lamp").transform;root.SetParent(story.market.transform,true);var p=story.marketSpawn.position+new Vector3(-5.5f,-.94f,-2);
   Rod(root,"Painted_Iron_Street_Lamp",p,p+Vector3.up*3.5f,.075f,iron);
   Rod(root,"Curved_Lamp_Bracket",p+Vector3.up*3.5f,p+new Vector3(2.5f,3.5f,0),.045f,iron);
   Part(root,"Lamp_Hood",p+new Vector3(2.5f,3.46f,0),new Vector3(.45f,.08f,.45f),iron,PrimitiveType.Sphere);
   Part(root,"Lamp_Glass",p+new Vector3(2.5f,3.35f,0),new Vector3(.16f,.1f,.16f),ivory,PrimitiveType.Sphere);
   Light(root,p+new Vector3(2.5f,3.2f,0),10,12,new Color(1,.76f,.49f));
  }
  Landscape(story);
  foreach(var t in story.lake.GetComponentsInChildren<Transform>(true).Where(t=>t.name=="Bamboo_Fishing_Rod"||t.name=="Fishing_Line").ToArray())t.SetParent(c.baoArm,true);
  foreach(var actor in new[]{story.ghosts[0],story.ghosts[1],story.schoolChapter.ghost,story.slopeChapter.ghost,c.lan})if(!actor.GetComponent<BieuCamNhoNhanVatV41>()){var micro=actor.gameObject.AddComponent<BieuCamNhoNhanVatV41>();micro.story=story;}
  EditorSceneManager.SaveScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene());AssetDatabase.SaveAssets();CaptureViews("after");
 }
 public static void BuildWindows()
 {
  EditorSceneManager.OpenScene(ScenePath);EditorBuildSettings.scenes=new[]{new EditorBuildSettingsScene(ScenePath,true)};PlayerSettings.bundleVersion="0.41.0";PlayerSettings.runInBackground=true;
  PlayerSettings.SetUseDefaultGraphicsAPIs(BuildTarget.StandaloneWindows64,false);PlayerSettings.SetGraphicsAPIs(BuildTarget.StandaloneWindows64,new[]{GraphicsDeviceType.Direct3D11});
  var r=BuildPipeline.BuildPlayer(new[]{ScenePath},"Builds/Windows_v41_r5/ConRoi.exe",BuildTarget.StandaloneWindows64,BuildOptions.Development);
  File.WriteAllText(Report+"/build.json",new JObject{["result"]=r.summary.result.ToString(),["errors"]=r.summary.totalErrors,["warnings"]=r.summary.totalWarnings,["path"]=r.summary.outputPath,["graphics_api"]="Direct3D11",["messages"]=new JArray(r.steps.SelectMany(s=>s.messages).Where(m=>m.type==LogType.Warning||m.type==LogType.Error).Select(m=>m.content))}.ToString());
 }
}
