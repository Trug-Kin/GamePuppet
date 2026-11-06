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

public static class ConRoiReviewV43
{
 public const string Scene="Assets/Scenes/JourneyExperience_v43.unity",Report="Docs/Reviews/journey_v43",Art="Assets/Art/Environment/JourneyV43";
 static Material timber,plaster,stone,tile,cloth,skin,apron;
 static Material Load(string path)=>AssetDatabase.LoadAssetAtPath<Material>(path);
 static Transform Root(Transform p,string name){var t=new GameObject(name).transform;t.SetParent(p,true);return t;}
 static GameObject Box(Transform p,string name,Vector3 pos,Vector3 size,Material mat,PrimitiveType type=PrimitiveType.Cube)
 {
  var g=GameObject.CreatePrimitive(type);g.name=name;g.layer=3;g.transform.position=pos;g.transform.localScale=size;g.transform.SetParent(p,true);g.GetComponent<Renderer>().sharedMaterial=mat;UnityEngine.Object.DestroyImmediate(g.GetComponent<Collider>());return g;
 }
 static void Beam(Transform p,string name,Vector3 a,Vector3 b,float width,Material m)
 {var g=Box(p,name,(a+b)*.5f,new Vector3(width,(b-a).magnitude,width),m);g.transform.up=b-a;}
 static void MeshObject(Transform p,string name,List<Vector3> v,List<int> tris,Material mat)
 {
  var mesh=new Mesh{name=name,indexFormat=IndexFormat.UInt32};mesh.SetVertices(v);mesh.SetTriangles(tris,0);mesh.SetUVs(0,v.Select(q=>new Vector2(q.x*.4f,q.z*.4f)).ToList());mesh.RecalculateNormals();mesh.RecalculateBounds();AssetDatabase.CreateAsset(mesh,Art+"/"+name+".asset");
  var g=new GameObject(name);g.layer=3;g.transform.SetParent(p,true);g.AddComponent<MeshFilter>().sharedMesh=mesh;g.AddComponent<MeshRenderer>().sharedMaterial=mat;
 }
 static void Batch(Transform p,string name)
 {
  var rs=p.GetComponentsInChildren<MeshRenderer>(true);
  int serial=0;foreach(var group in rs.GroupBy(r=>r.sharedMaterial))
  {
   var mesh=new Mesh{name=name+serial,indexFormat=IndexFormat.UInt32};mesh.CombineMeshes(group.Select(r=>new CombineInstance{mesh=r.GetComponent<MeshFilter>().sharedMesh,transform=p.worldToLocalMatrix*r.transform.localToWorldMatrix}).ToArray());AssetDatabase.CreateAsset(mesh,Art+"/"+mesh.name+".asset");
   var g=new GameObject(mesh.name);g.layer=3;g.transform.SetParent(p,false);g.AddComponent<MeshFilter>().sharedMesh=mesh;g.AddComponent<MeshRenderer>().sharedMaterial=group.Key;serial++;
  }
  foreach(var r in rs)UnityEngine.Object.DestroyImmediate(r.gameObject);
 }
 static void Roof(Transform root,Vector3 o,float halfWidth,float length,float height,string name)
 {
  foreach(int side in new[]{-1,1})
  {
   var g=Box(root,name+"_Slope",o+new Vector3(side*halfWidth*.5f,height*.5f,0),new Vector3(Mathf.Sqrt(halfWidth*halfWidth+height*height),.14f,length),tile);g.transform.rotation=Quaternion.Euler(0,0,-side*Mathf.Atan2(height,halfWidth)*Mathf.Rad2Deg);
   Box(root,name+"_Eave",o+new Vector3(side*halfWidth,-.12f,0),new Vector3(.16f,.24f,length+.16f),timber);
   for(float z=-length*.5f;z<=length*.5f;z+=.42f)Beam(root,"Tile_Raised_Course",o+new Vector3(0,height+.08f,z),o+new Vector3(side*halfWidth,.08f,z),.026f,tile);
  }
  Box(root,"Ridge_Cap",o+Vector3.up*(height+.08f),new Vector3(.21f,.17f,length+.2f),tile);
  foreach(int end in new[]{-1,1})
  {
   var z=end*length*.5f;MeshObject(root,name+"_Gable_"+end,new List<Vector3>{o+new Vector3(-halfWidth,0,z),o+new Vector3(halfWidth,0,z),o+new Vector3(0,height,z)},new List<int>{0,1,2,2,1,0},plaster);
   Beam(root,"Gable_Timber",o+new Vector3(-halfWidth,0,z-.025f),o+new Vector3(0,height,z-.025f),.12f,timber);Beam(root,"Gable_Timber",o+new Vector3(halfWidth,0,z-.025f),o+new Vector3(0,height,z-.025f),.12f,timber);
  }
 }
 static void School(DieuPhoiHanhTrinh s)
 {
  var env=s.schoolChapter.environment.transform;var root=Root(env,"V43_School_Architecture");var o=s.schoolChapter.spawn.position-new Vector3(0,.94f,-7);
  // Keep the old structural collider envelope; replace its flat visible roof.
  foreach(var r in env.GetComponentsInChildren<MeshRenderer>(true).Where(r=>r.name=="Roof"||r.name=="Archive_Roof"))r.enabled=false;
  Roof(root,o+new Vector3(0,3.9f,7),8.7f,15.4f,1.6f,"GiangDuong203");Roof(root,o+new Vector3(-22,3.9f,6),10.7f,17.2f,1.4f,"Archive");
  // A continuous veranda fascia connects the covered link to the classroom.
  Box(root,"Veranda_Fascia",o+new Vector3(-16,3.55f,-7.1f),new Vector3(30,.28f,.20f),timber);
  foreach(float x in new[]{-31f,-25f,-18f,-11f})
  {
   Box(root,"Column_Plaster_Base",o+new Vector3(x,.22f,-6.8f),new Vector3(.46f,.44f,.46f),stone);
   Box(root,"Column_Capital",o+new Vector3(x,3.35f,-6.8f),new Vector3(.48f,.22f,.48f),plaster);
   Beam(root,"Veranda_Knee_Brace",o+new Vector3(x,2.8f,-6.8f),o+new Vector3(x+.65f,3.45f,-6.8f),.11f,timber);
  }
  foreach(int side in new[]{-1,1})
  {
   // Recessed panels break the blank entrance wings without covering signage.
   float x=side*6.7f;Box(root,"Entrance_Window_Reveal",o+new Vector3(x,1.92f,-.13f),new Vector3(1.5f,1.65f,.10f),timber);
   Box(root,"Entrance_Dark_Glazing",o+new Vector3(x,1.92f,-.20f),new Vector3(1.29f,1.44f,.04f),Load("Assets/Art/Environment/JourneyV42/Old_BlueGreen_Glass.mat"));
   for(int j=-1;j<=1;j++)Box(root,"Entrance_Window_Mullion",o+new Vector3(x+j*.37f,1.92f,-.25f),new Vector3(.045f,1.5f,.04f),plaster);
   Box(root,"Entrance_Sill",o+new Vector3(x,1.06f,-.24f),new Vector3(1.73f,.13f,.28f),stone);
   Box(root,"Lime_Plaster_Plinth",o+new Vector3(side*5.0f,.24f,-.14f),new Vector3(6,.42f,.09f),stone);
  }
  Batch(root,"SchoolArchitecture");
 }
 static void Slope(DieuPhoiHanhTrinh s)
 {
  var env=s.slopeChapter.environment.transform;var o=s.slopeChapter.spawn.position-new Vector3(0,.94f,1);var root=Root(env,"V43_DaLat_Crest_Join");
  // Landform wraps the existing road, staying outside its curb and final approach.
  var v=new List<Vector3>();var tris=new List<int>();
  for(int z=0;z<=12;z++)for(int x=0;x<=20;x++)
  {float px=-65+x*6.5f,pz=78+z*6;float y=4.0f+z*.23f+3.4f*Mathf.Sin(x*.57f+z*.24f)+2.4f*Mathf.Cos(x*.29f-z*.35f);v.Add(o+new Vector3(px,y,pz));if(x<20&&z<12){int k=z*21+x;tris.AddRange(new[]{k,k+21,k+1,k+1,k+21,k+22});}}
  MeshObject(root,"Crest_Undulating_Landform",v,tris,Load("Assets/Art/Environment/JourneyV42/Pine_Needle_Shadow.mat"));
  // The final door belongs to a plaster garden wall, with short return walls.
  foreach(int side in new[]{-1,1})
  {
   Box(root,"Crest_Garden_Wall",o+new Vector3(side*2.7f,7.05f,77.05f),new Vector3(3.9f,2.1f,.36f),plaster);
   Box(root,"Crest_Wall_Coping",o+new Vector3(side*2.7f,8.18f,77.05f),new Vector3(4.05f,.16f,.50f),tile);
   Box(root,"Crest_Wall_Return",o+new Vector3(side*4.65f,6.65f,73.2f),new Vector3(.35f,1.3f,7.8f),stone);
   for(int bay=0;bay<4;bay++)
   {
    float z=12+bay*13,ground=Mathf.Min(z,60)*.1f;
    // Retaining walls and stepped stoops make each plot meet the sloping lane.
    Box(root,"Stepped_Plot_Retaining_Wall",o+new Vector3(side*4.65f,ground-.32f,z),new Vector3(.48f,1.15f,9.5f),stone);
    for(int step=0;step<3;step++)Box(root,"House_Threshold_Step",o+new Vector3(side*(4.7f+step*.20f),ground+.04f+step*.11f,z-2.8f),new Vector3(.22f,.12f,1.3f),stone);
    if((bay+(side==1?1:0))%2==0)
    {
     // Slender porch roof, braces and recessed timber threshold vary silhouettes.
     Box(root,"Porch_Canopy",o+new Vector3(side*5.2f,ground+2.65f,z-2.8f),new Vector3(1.45f,.12f,2.05f),tile).transform.rotation=Quaternion.Euler(0,0,side*12);
     Beam(root,"Porch_Brace",o+new Vector3(side*5.45f,ground+2.0f,z-3.45f),o+new Vector3(side*4.65f,ground+2.6f,z-3.45f),.075f,timber);
    }
   }
  }
  Box(root,"Final_Door_Lintel_Plaster",o+new Vector3(0,8.32f,76.95f),new Vector3(1.6f,.30f,.42f),plaster);
  // Neighboring roof lines lie behind the crest, not pasted into the walking lane.
  foreach(var p in new[]{new Vector3(-14,7,89),new Vector3(12,8,96),new Vector3(25,6.5f,91)})
  {Box(root,"Crest_Neighbor_Plaster",o+p,new Vector3(5,3,6),plaster);Roof(root,o+p+Vector3.up*1.5f,2.9f,6.6f,.95f,"CrestHouse"+p.x);}
  Batch(root,"SlopeArchitecture");
 }
 static void Ha(DieuPhoiHanhTrinh s,DieuChinhHanhTrinhV43 c)
 {
  var actor=s.ghosts[1];var head=actor.GetComponent<HienGuongMatTheoKhu>().ghostHead;
  var face=head.GetComponentsInChildren<MeshRenderer>(true).First(r=>r.name.EndsWith("_Face"));skin=face.sharedMaterial;
  var sleeve=actor.GetComponentsInChildren<MeshRenderer>(true).First(r=>r.name.StartsWith("Tailored_Sleeve"));cloth=sleeve.sharedMaterial;
  var jacket=actor.GetComponentsInChildren<MeshRenderer>(true).First(r=>r.name=="Tailored_Jacket");var b=jacket.bounds;var basePoint=new Vector3(b.center.x,b.min.y-.76f,b.center.z);
  foreach(var r in actor.GetComponentsInChildren<MeshRenderer>(true).Where(r=>r.name.StartsWith("Tailored_Sleeve")||r.name.StartsWith("Hand_")||r.name.StartsWith("Ha_Apron")||r.name=="Ha_Ochre_Market_Apron"))r.enabled=false;
  // Profiled cloth apron, with volume/hem rather than the former rectangular slab.
  var root=Root(actor,"V43_Ha_Tailored_Apron");var v=new List<Vector3>();var tr=new List<int>();
  for(int row=0;row<5;row++)for(int col=0;col<=12;col++)
  {float t=col/12f,a=(t-.5f)*Mathf.PI*1.1f;float y=.34f+row*.18f,r=.255f-row*.024f;v.Add(basePoint+new Vector3(Mathf.Sin(a)*r,y,-Mathf.Cos(a)*r-.03f-.012f*Mathf.Sin(t*31)));if(row<4&&col<12){int k=row*13+col;tr.AddRange(new[]{k,k+1,k+14,k,k+14,k+13,k+14,k+1,k,k+13,k+14,k});}}
  MeshObject(root,"Ha_Curved_Cloth_Apron",v,tr,apron);
  Box(root,"Apron_Fitted_Bib",basePoint+new Vector3(0,1.13f,-.15f),new Vector3(.25f,.29f,.025f),apron);
  c.shoulders=new Vector3[2];c.upper=new Transform[2];c.lower=new Transform[2];c.hands=new Transform[2];
  for(int i=0;i<2;i++)
  {
   int side=i==0?-1:1;c.shoulders[i]=basePoint+new Vector3(side*.24f,1.25f,0);
   c.upper[i]=Box(actor,"V43_Ha_UpperSleeve_"+i,c.shoulders[i],new Vector3(.115f,.28f,.115f),cloth,PrimitiveType.Capsule).transform;
   c.lower[i]=Box(actor,"V43_Ha_ForeSleeve_"+i,basePoint+new Vector3(side*.25f,.93f,-.11f),new Vector3(.085f,.27f,.085f),cloth,PrimitiveType.Capsule).transform;
   c.hands[i]=Box(actor,"V43_Ha_Palm_"+i,basePoint+new Vector3(side*.13f,.82f,-.30f),new Vector3(.072f,.065f,.10f),skin,PrimitiveType.Sphere).transform;
  }
  c.haOrigin=basePoint;c.haHead=head;c.parcel=s.GetComponent<TraiNghiemHanhTrinhV42>().sortedParcel;
  c.PreviewHa();
 }
 public static void Assemble()
 {
  if(File.Exists(Scene))throw new Exception("v43 exists; preserve it");Directory.CreateDirectory(Report+"/comparison");Directory.CreateDirectory(Art);
  EditorSceneManager.OpenScene(ConRoiExperienceV42.ScenePath);Capture("before");
  timber=Load("Assets/Art/Environment/JourneyV42/Timber_Worn_Warm.mat");plaster=Load("Assets/Art/Environment/SchoolV38/Weathered_ ochre.mat");stone=Load("Assets/Art/Environment/SlopeV39/Retaining_Stone.mat");tile=Load("Assets/Art/Environment/SlopeV39/Old_ Timber.mat");apron=Load("Assets/Art/Environment/JourneyV42/Ha_Terracotta_Apron.mat");
  var s=UnityEngine.Object.FindAnyObjectByType<DieuPhoiHanhTrinh>();var c=s.gameObject.AddComponent<DieuChinhHanhTrinhV43>();c.story=s;School(s);Slope(s);Ha(s,c);
  EditorSceneManager.SaveScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene(),Scene);AssetDatabase.SaveAssets();Capture("after");EditorSceneManager.OpenScene(Scene);
 }
 public static void Capture(string suffix)
 {
  var s=UnityEngine.Object.FindAnyObjectByType<DieuPhoiHanhTrinh>();var eye=s.player.GetComponentInChildren<Camera>();var ha=s.ghosts[1].position;
  var pos=new[]{ha+new Vector3(-2.4f,1.4f,-3.2f),new Vector3(-95,1.7f,-52),new Vector3(-180,7.65f,9)};
  var target=new[]{ha+new Vector3(0,.95f,0),new Vector3(-90,2.8f,-44),new Vector3(-180,7.9f,17)};
  var areas=new[]{s.market,s.schoolChapter.environment,s.slopeChapter.environment};var all=new[]{s.hub,s.lake,s.market,s.schoolChapter.environment,s.slopeChapter.environment};
  var oldPosition=eye.transform.position;var oldRotation=eye.transform.rotation;var active=all.Select(a=>a.activeSelf).ToArray();
  var poses=new JArray();var rt=RenderTexture.GetTemporary(1280,720,24);var tex=new Texture2D(1280,720,TextureFormat.RGB24,false);
  for(int n=0;n<3;n++)
  {
   foreach(var a in all)a.SetActive(a==areas[n]);eye.transform.SetPositionAndRotation(pos[n],Quaternion.LookRotation(target[n]-pos[n]));
   for(int warm=0;warm<3;warm++)RenderPipeline.SubmitRenderRequest(eye,new UniversalRenderPipeline.SingleCameraRequest{destination=rt});var old=RenderTexture.active;RenderTexture.active=rt;tex.ReadPixels(new Rect(0,0,1280,720),0,0);tex.Apply();RenderTexture.active=old;string name=new[]{"Ha","school","crest"}[n];File.WriteAllBytes(Report+"/comparison/"+name+"_"+suffix+".png",tex.EncodeToPNG());poses.Add(new JObject{["scene"]=name,["position"]=new JArray(pos[n].x,pos[n].y,pos[n].z),["target"]=new JArray(target[n].x,target[n].y,target[n].z),["fov"]=eye.fieldOfView,["source"]="Editor render; rest pose, not runtime acting"});
  }
  File.WriteAllText(Report+"/comparison/cameras_"+suffix+".json",poses.ToString());RenderTexture.ReleaseTemporary(rt);UnityEngine.Object.DestroyImmediate(tex);
  for(int n=0;n<all.Length;n++)all[n].SetActive(active[n]);eye.transform.SetPositionAndRotation(oldPosition,oldRotation);
 }
 public static void ReviewFinalImages()
 {
  EditorSceneManager.OpenScene(ConRoiExperienceV42.ScenePath);Capture("before");
  EditorSceneManager.OpenScene(Scene);Capture("after");
 }
 public static void Build()
 {
  RepairTwoSidedNormals();
  EditorSceneManager.OpenScene(Scene);EditorBuildSettings.scenes=new[]{new EditorBuildSettingsScene(Scene,true)};PlayerSettings.bundleVersion="0.43.0";PlayerSettings.runInBackground=true;PlayerSettings.enableFrameTimingStats=true;
  PlayerSettings.SetUseDefaultGraphicsAPIs(BuildTarget.StandaloneWindows64,false);PlayerSettings.SetGraphicsAPIs(BuildTarget.StandaloneWindows64,new[]{GraphicsDeviceType.Direct3D11});
  Capture("after");
  var r=BuildPipeline.BuildPlayer(new[]{Scene},"Builds/Windows_v43_r3/ConRoi.exe",BuildTarget.StandaloneWindows64,BuildOptions.Development);
  File.WriteAllText(Report+"/build.json",new JObject{["result"]=r.summary.result.ToString(),["errors"]=r.summary.totalErrors,["warnings"]=r.summary.totalWarnings,["path"]=r.summary.outputPath,["messages"]=new JArray(r.steps.SelectMany(s=>s.messages).Where(m=>m.type==LogType.Error||m.type==LogType.Warning).Select(m=>m.content))}.ToString());
 }
 static void RepairTwoSidedNormals()
 {
  var results=new JArray();
  foreach(var guid in AssetDatabase.FindAssets("t:Mesh",new[]{Art}))
  {
   var path=AssetDatabase.GUIDToAssetPath(guid);var mesh=AssetDatabase.LoadAssetAtPath<Mesh>(path);
   if(mesh.subMeshCount!=1)throw new Exception("Unexpected submesh count: "+path);
   var vertices=mesh.vertices.ToList();var uv=mesh.uv.ToList();var triangles=mesh.triangles;
   var seen=new HashSet<string>();var back=new Dictionary<int,int>();int split=0;
   for(int i=0;i<triangles.Length;i+=3)
   {
    var key=new[]{triangles[i],triangles[i+1],triangles[i+2]};Array.Sort(key);
    if(seen.Add(string.Join(",",key)))continue;
    for(int j=0;j<3;j++)
    {
     int original=triangles[i+j];
     if(!back.TryGetValue(original,out int index)){index=vertices.Count;back.Add(original,index);vertices.Add(vertices[original]);if(uv.Count>0)uv.Add(uv[original]);}
     triangles[i+j]=index;
    }
    split++;
   }
   if(split==0)continue;
   mesh.Clear();mesh.indexFormat=IndexFormat.UInt32;mesh.SetVertices(vertices);if(uv.Count>0)mesh.SetUVs(0,uv);mesh.SetTriangles(triangles,0);mesh.RecalculateNormals();mesh.RecalculateBounds();
   int zero=mesh.normals.Count(n=>n.sqrMagnitude<.5f);if(zero>0)throw new Exception("Invalid normals remain: "+path);
   EditorUtility.SetDirty(mesh);results.Add(new JObject{["asset"]=path,["back_triangles_split"]=split,["zero_normals"]=zero});
  }
  AssetDatabase.SaveAssets();File.WriteAllText(Report+"/normal-repair.json",results.ToString());
 }
 public static void FinishReview()
 {
  EditorSceneManager.OpenScene(ConRoiExperienceV42.ScenePath);var old=UnityEngine.Object.FindAnyObjectByType<DieuPhoiHanhTrinh>();var cam=old.player.GetComponentInChildren<Camera>();var local=cam.transform.localPosition;var rotation=cam.transform.localRotation;Capture("before");
  EditorSceneManager.OpenScene(Scene);var s=UnityEngine.Object.FindAnyObjectByType<DieuPhoiHanhTrinh>();cam=s.player.GetComponentInChildren<Camera>();cam.transform.localPosition=local;cam.transform.localRotation=rotation;
  var probe=s.GetComponent<ChanDoanDieuKhienV43>();if(!probe)probe=s.gameObject.AddComponent<ChanDoanDieuKhienV43>();probe.story=s;
  timber=Load("Assets/Art/Environment/JourneyV42/Timber_Worn_Warm.mat");plaster=Load("Assets/Art/Environment/SchoolV38/Weathered_ ochre.mat");stone=Load("Assets/Art/Environment/SlopeV39/Retaining_Stone.mat");tile=Load("Assets/Art/Environment/SlopeV39/Old_ Timber.mat");
  var root=s.schoolChapter.environment.transform.Find("V43_School_Architecture");UnityEngine.Object.DestroyImmediate(root.gameObject);School(s);
  s.hub.SetActive(true);s.lake.SetActive(false);s.market.SetActive(false);s.schoolChapter.environment.SetActive(false);s.slopeChapter.environment.SetActive(false);
  EditorSceneManager.SaveScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene());AssetDatabase.SaveAssets();Capture("after");
  File.WriteAllText(Report+"/scene-integrity.json",new JObject{["camera_local_position"]=local.ToString("F4"),["camera_matches_r7"]=true,["school_new_colliders"]=s.schoolChapter.environment.transform.Find("V43_School_Architecture").GetComponentsInChildren<Collider>(true).Length,["crest_new_colliders"]=s.slopeChapter.environment.transform.Find("V43_DaLat_Crest_Join").GetComponentsInChildren<Collider>(true).Length,["missing_scripts"]=UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects().Sum(g=>g.GetComponentsInChildren<Transform>(true).Sum(t=>GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(t.gameObject)))}.ToString());
  EditorSceneManager.OpenScene(Scene);
 }
}
