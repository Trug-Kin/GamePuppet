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

public static class ConRoiExperienceV42
{
 public const string ScenePath="Assets/Scenes/JourneyExperience_v42.unity",Report="Docs/Reviews/journey_v42";
 const string Art="Assets/Art/Environment/JourneyV42";
 static Material wood,cream,green,dark,glass,red,blue,grey;
 static Material Mat(string name,Color c)
 {var m=new Material(Shader.Find("Universal Render Pipeline/Lit")){name=name,color=c,enableInstancing=true};m.SetFloat("_Smoothness",.17f);AssetDatabase.CreateAsset(m,Art+"/"+name+".mat");return m;}
 static Transform Root(Transform parent,string name){var t=new GameObject(name).transform;t.SetParent(parent,true);return t;}
 static GameObject Part(Transform parent,string name,Vector3 p,Vector3 size,Material m,PrimitiveType shape=PrimitiveType.Cube)
 {
  var g=GameObject.CreatePrimitive(shape);g.name=name;g.layer=3;g.transform.SetParent(parent,true);g.transform.position=p;g.transform.rotation=Quaternion.identity;
  var s=parent.lossyScale;g.transform.localScale=new Vector3(size.x/Mathf.Abs(s.x),size.y/Mathf.Abs(s.y),size.z/Mathf.Abs(s.z));
  g.GetComponent<Renderer>().sharedMaterial=m;UnityEngine.Object.DestroyImmediate(g.GetComponent<Collider>());return g;
 }
 static void Rod(Transform root,string name,Vector3 a,Vector3 b,float width,Material m)
 {var g=Part(root,name,(a+b)*.5f,new Vector3(width,(b-a).magnitude*.5f,width),m,PrimitiveType.Cylinder);g.transform.up=b-a;}
 static void Panel(Transform root,string name,Vector3 p,Vector3 size,Material m,Vector3 normal)
 {Part(root,name,p,size,m).transform.rotation=Quaternion.LookRotation(normal,Vector3.up);}
 static Mesh Rounded(string name,int rings=7,int sides=10)
 {
  // Rounded, irregular foliage/garment sections with closed caps, not cone fans.
  var v=new List<Vector3>();var tr=new List<int>();var uv=new List<Vector2>();
  for(int y=0;y<=rings;y++)for(int a=0;a<sides;a++)
  {
   float t=(float)y/rings,angle=a*Mathf.PI*2/sides;
   float r=Mathf.Max(.015f,Mathf.Sin(t*Mathf.PI)*.5f)*(1+.08f*Mathf.Sin(a*7+y*3));
   v.Add(new Vector3(Mathf.Cos(angle)*r,t-.5f,Mathf.Sin(angle)*r));uv.Add(new Vector2((float)a/sides,t));
   if(y<rings){int k=y*sides+a,j=y*sides+(a+1)%sides;tr.AddRange(new[]{k,j,j+sides,k,j+sides,k+sides});}
  }
  var mesh=new Mesh{name=name};mesh.SetVertices(v);mesh.SetTriangles(tr,0);mesh.SetUVs(0,uv);mesh.RecalculateNormals();mesh.RecalculateBounds();AssetDatabase.CreateAsset(mesh,Art+"/"+name+".asset");return mesh;
 }
 static Mesh NeedleSpray()
 {
  var existing=AssetDatabase.LoadAssetAtPath<Mesh>(Art+"/Pine_Needle_Sprays.asset");if(existing)return existing;
  var v=new List<Vector3>();var tr=new List<int>();var rng=new System.Random(42);
  for(int twig=0;twig<36;twig++)
  {
   float a=twig*2.39996f,r=Mathf.Sqrt((twig+.5f)/36)*.48f;
   var start=new Vector3(Mathf.Cos(a)*r,(float)rng.NextDouble()*.20f,Mathf.Sin(a)*r);
   for(int needle=0;needle<6;needle++)
   {
    float angle=a+needle*1.047f;var dir=new Vector3(Mathf.Cos(angle),.7f+(float)rng.NextDouble(),Mathf.Sin(angle)).normalized;
    var side=Vector3.Cross(dir,Vector3.up).normalized*.012f;var end=start+dir*(.18f+(float)rng.NextDouble()*.16f);int k=v.Count;
    v.Add(start-side);v.Add(start+side);v.Add(end);tr.AddRange(new[]{k,k+1,k+2,k+2,k+1,k});
   }
  }
  var mesh=new Mesh{name="Pine_Needle_Sprays"};mesh.SetVertices(v);mesh.SetTriangles(tr,0);mesh.RecalculateNormals();mesh.RecalculateBounds();AssetDatabase.CreateAsset(mesh,Art+"/Pine_Needle_Sprays.asset");return mesh;
 }
 static Mesh TailoredBody()
 {
  var mesh=AssetDatabase.LoadAssetAtPath<Mesh>(Art+"/Tailored_Cardigan.asset");if(mesh)return mesh;
  mesh=UnityEngine.Object.Instantiate(AssetDatabase.LoadAssetAtPath<Mesh>("Assets/Art/Environment/JourneyV41/Carved_Tapered_Torso.asset"));mesh.name="Tailored_Cardigan";
  var tr=mesh.triangles;for(int n=0;n<tr.Length;n+=3){int swap=tr[n+1];tr[n+1]=tr[n+2];tr[n+2]=swap;}mesh.triangles=tr;mesh.RecalculateNormals();AssetDatabase.CreateAsset(mesh,Art+"/Tailored_Cardigan.asset");return mesh;
 }
 static void Facades(DieuPhoiHanhTrinh story)
 {
  var route=JObject.Parse(File.ReadAllText(ConRoiRouteV32.Folder+"/regional_route.json"))["route"].Select(x=>new Vector3((float)x[0],(float)x[1],(float)x[2])).ToArray();
  var root=Root(story.transform,"V42_Route_Facades");var rows=new JArray();var seen=new HashSet<string>();int count=0;
  // Existing model surfaces carry the building envelope. Add fenestration to
  // their actual outward planes, rather than placing new boxes over the street.
  foreach(var renderer in UnityEngine.Object.FindObjectsByType<MeshRenderer>(FindObjectsInactive.Include,FindObjectsSortMode.None))
  {
   if(!new[]{"Surface_000","Surface_001","Surface_002","Surface_003","Surface_093","Surface_165","Surface_167"}.Contains(renderer.name))continue;
   var filter=renderer.GetComponent<MeshFilter>();if(!filter||!filter.sharedMesh)continue;var mesh=filter.sharedMesh;var vertices=mesh.vertices;var triangles=mesh.triangles;
   for(int n=0;n+5<triangles.Length;n+=6)
   {
    var points=Enumerable.Range(0,6).Select(i=>filter.transform.TransformPoint(vertices[triangles[n+i]])).ToArray();
    var normal=Vector3.Cross(points[1]-points[0],points[2]-points[0]).normalized;
    if(Mathf.Abs(normal.y)>.08f)continue;normal.y=0;normal.Normalize();var side=Vector3.Cross(Vector3.up,normal);
    float low=points.Min(p=>p.y),high=points.Max(p=>p.y),left=points.Min(p=>Vector3.Dot(p,side)),right=points.Max(p=>Vector3.Dot(p,side));
    float width=right-left,height=high-low;if(width<3||height<4||width>65||height>35)continue;
    var center=points.Aggregate(Vector3.zero,(a,b)=>a+b)/6;center+=side*((left+right)*.5f-Vector3.Dot(center,side));center.y=low;
    float nearest=route.Min(p=>Vector2.Distance(new Vector2(p.x,p.z),new Vector2(center.x,center.z)));if(nearest>38)continue;
    string key=$"{center.x:F1}_{center.z:F1}_{normal.x:F1}_{normal.z:F1}";if(!seen.Add(key))continue;
    if(count++>=70)break;
    for(int floor=0;floor<Mathf.Min(4,Mathf.FloorToInt((height-1)/2.9f));floor++)
    {
     float y=low+3.4f+floor*2.8f;if(y+1>high)break;
     for(float x=-width*.5f+1.3f;x<width*.5f-1;x+=2.8f)
     {
      var p=center+side*x+Vector3.up*(y-low)+normal*.055f;
      Panel(root,"Recessed_Timber_Window_Frame",p,new Vector3(1.14f,1.55f,.09f),wood,normal);
      Panel(root,"BlueGreen_Window_Glass",p+normal*.06f,new Vector3(.96f,1.36f,.025f),glass,normal);
      Panel(root,"Window_Mullion",p+normal*.085f,new Vector3(.045f,1.4f,.04f),cream,normal);
      Panel(root,"Window_Transom",p+Vector3.up*.25f+normal*.085f,new Vector3(1.02f,.045f,.04f),cream,normal);
      Panel(root,"Weathered_Window_Sill",p-Vector3.up*.83f+normal*.11f,new Vector3(1.30f,.1f,.28f),cream,normal);
      if((floor+count)%3==0)for(int s=-1;s<=1;s+=2)Panel(root,"Narrow_Timber_Shutter",p+side*s*.69f,new Vector3(.22f,1.5f,.06f),green,normal);
     }
    }
    Panel(root,"Roof_Cornice",center+Vector3.up*(height-.15f)+normal*.10f,new Vector3(width,.23f,.34f),cream,normal);
    Panel(root,"Rendered_Lower_Plinth",center+Vector3.up*.3f+normal*.035f,new Vector3(width,.6f,.07f),grey,normal);
    if(width>6)for(int i=0;i<=Mathf.FloorToInt(width/4);i++)Panel(root,"Facade_Pilaster",center+side*(-width*.5f+i*4)+Vector3.up*height*.5f+normal*.02f,new Vector3(.13f,height,.09f),cream,normal);
    rows.Add(new JObject{["source"]=renderer.name,["center"]=center.ToString("F2"),["width"]=width,["height"]=height,["route_distance"]=nearest});
   }
  }
  File.WriteAllText(Report+"/facades.json",rows.ToString());Batch(root,"Facades");
 }
 public static void Inspect()
 {
  EditorSceneManager.OpenScene(ScenePath);
  var story=UnityEngine.Object.FindAnyObjectByType<DieuPhoiHanhTrinh>();var ha=story.ghosts[1].position;
  File.WriteAllText(Report+"/market-nearby.json",new JArray(UnityEngine.Object.FindObjectsByType<MeshRenderer>(FindObjectsInactive.Include).Where(r=>Vector3.Distance(r.bounds.ClosestPoint(ha),ha)<20&&r.bounds.size.y>3&&r.bounds.size.x+r.bounds.size.z>8).Select(r=>new JObject{["name"]=r.name,["materials"]=new JArray(r.sharedMaterials.Select(m=>m?m.name:"null")),["bounds"]=r.bounds.ToString()})).ToString());
  File.WriteAllText(Report+"/geometry.json",new JArray(UnityEngine.Object.FindObjectsByType<MeshRenderer>(FindObjectsInactive.Include,FindObjectsSortMode.None).Where(r=>r.sharedMaterials.Any(m=>m&&(m.name.Contains("Building")||m.name.StartsWith("Surface_00")))).Select(r=>new JObject{["name"]=r.name,["parent"]=r.transform.parent.name,["materials"]=new JArray(r.sharedMaterials.Select(m=>m?m.name:"null")),["bounds"]=r.bounds.ToString(),["vertices"]=r.GetComponent<MeshFilter>().sharedMesh.vertexCount})).ToString());
 }
 public static void Refine()
 {
  EditorSceneManager.OpenScene(ScenePath);var story=UnityEngine.Object.FindAnyObjectByType<DieuPhoiHanhTrinh>();
  wood=AssetDatabase.LoadAssetAtPath<Material>(Art+"/Timber_Worn_Warm.mat");cream=AssetDatabase.LoadAssetAtPath<Material>(Art+"/Ivory_Lime_Trim.mat");green=AssetDatabase.LoadAssetAtPath<Material>(Art+"/Pine_Needle_Mid.mat");dark=AssetDatabase.LoadAssetAtPath<Material>(Art+"/Pine_Needle_Shadow.mat");glass=AssetDatabase.LoadAssetAtPath<Material>(Art+"/Old_BlueGreen_Glass.mat");grey=AssetDatabase.LoadAssetAtPath<Material>(Art+"/Bao_Grey_Wool.mat");
  red=AssetDatabase.LoadAssetAtPath<Material>(Art+"/Ha_Terracotta_Apron.mat");blue=AssetDatabase.LoadAssetAtPath<Material>(Art+"/Lan_Indigo_Vy_Satchel.mat");
  var rounded=AssetDatabase.LoadAssetAtPath<Mesh>(Art+"/Rounded_Organic_Section.asset");
  var verts=rounded.vertices;var normals=rounded.normals;int sample=10;
  if(Vector3.Dot(new Vector3(verts[sample].x,0,verts[sample].z),normals[sample])<0)
  {var tris=rounded.triangles;for(int n=0;n<tris.Length;n+=3){int swap=tris[n+1];tris[n+1]=tris[n+2];tris[n+2]=swap;}rounded.triangles=tris;rounded.RecalculateNormals();EditorUtility.SetDirty(rounded);}
  var old=UnityEngine.Object.FindObjectsByType<Transform>(FindObjectsInactive.Include).FirstOrDefault(t=>t.name=="V42_Route_Facades");if(old)UnityEngine.Object.DestroyImmediate(old.gameObject);
  Facades(story);
  var dressParts=new HashSet<string>{"Bao_Wool_Flat_Cap","Bao_Cap_Brim","Bao_Broad_Wool_Cardigan","Cardigan_Horn_Button","Cardigan_Pocket","Ha_Ochre_Market_Apron","Ha_Apron_Bib","Ha_Apron_Pocket","Ha_Tied_Hair_Bun","Ha_Hair_Tie","Lan_Long_Indigo_Coat","Lan_Silver_Bun","Vy_Student_Backpack","Vy_Backpack_Strap"};
  foreach(var actor in new[]{story.ghosts[0],story.ghosts[1],story.completion.lan,story.completion.vy})
  foreach(var t in actor.GetComponentsInChildren<Transform>(true).Where(t=>dressParts.Contains(t.name)&&t.GetComponent<Renderer>()).ToArray())UnityEngine.Object.DestroyImmediate(t.gameObject);
  Dress(story.ghosts[0],"Bao",rounded);Dress(story.ghosts[1],"Ha",rounded);Dress(story.completion.lan,"Lan",rounded);Dress(story.completion.vy,"Vy",rounded);
  BindVisibleHand(story.ghosts[0],story.completion.baoArm);BindVisibleHand(story.ghosts[1],story.completion.haArm);
  var sortingHand=story.completion.haArm.GetComponentsInChildren<MeshRenderer>(true).First(r=>r.name.StartsWith("Hand_"));
  story.GetComponent<TraiNghiemHanhTrinhV42>().sortedParcel.position=sortingHand.bounds.center+new Vector3(0,.035f,-.10f);
  var grove=story.lake.transform.Find("V42_DaLat_Irregular_Pine_Grove");if(grove)UnityEngine.Object.DestroyImmediate(grove.gameObject);Pines(story,NeedleSpray());
  foreach(var name in new[]{"V42_School_Joinery","V42_Suitcase_Stitchwork","V42_Hub_Worn_Joinery"}){var detail=UnityEngine.Object.FindObjectsByType<Transform>(FindObjectsInactive.Include).FirstOrDefault(t=>t.name==name);if(detail)UnityEngine.Object.DestroyImmediate(detail.gameObject);}
  OtherAreas(story);ReshapeTrees(story);
  if(!story.schoolChapter.puppet.GetComponentsInChildren<Transform>(true).Any(t=>t.name=="V42_Articulated_Carved_Hand"))PuppetHands(story);
  EditorSceneManager.SaveScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene());AssetDatabase.SaveAssets();CaptureViews("after");
 }
 static void Batch(Transform root,string name)
 {
  // Combine only static cosmetic children. Runtime actors/trace props remain separate.
  var renderers=root.GetComponentsInChildren<MeshRenderer>();int batch=0;
  foreach(var group in renderers.GroupBy(r=>r.sharedMaterial))
  {
   var mesh=new Mesh{name=name+"_"+batch,indexFormat=IndexFormat.UInt32};
   mesh.CombineMeshes(group.Select(r=>new CombineInstance{mesh=r.GetComponent<MeshFilter>().sharedMesh,transform=root.worldToLocalMatrix*r.transform.localToWorldMatrix}).ToArray());
   AssetDatabase.CreateAsset(mesh,Art+"/"+name+"_"+batch+++".asset");
   var g=new GameObject(mesh.name);g.layer=3;g.transform.SetParent(root,false);g.AddComponent<MeshFilter>().sharedMesh=mesh;g.AddComponent<MeshRenderer>().sharedMaterial=group.Key;
  }
  foreach(var r in renderers)UnityEngine.Object.DestroyImmediate(r.gameObject);
 }
 static void Pines(DieuPhoiHanhTrinh story,Mesh crown)
 {
  var old=story.lake.transform.Find("V41_Lake_Pine_Grove");if(!old)return;
  var trunks=old.GetComponentsInChildren<Transform>().Where(t=>t.name=="DaLat_Pine_Trunk").Select(t=>t.GetComponent<Renderer>().bounds).ToArray();old.gameObject.SetActive(false);
  var root=Root(story.lake.transform,"V42_DaLat_Irregular_Pine_Grove");int seed=0;
  foreach(var b in trunks)
  {
   Vector3 p=new Vector3(b.center.x,b.min.y,b.center.z);float height=b.size.y;Rod(root,"Scots_Pine_Rough_Trunk",p,p+Vector3.up*height,.13f,wood);
   for(int level=0;level<5;level++)for(int arm=0;arm<5;arm++)
   {
    float a=arm*2.399f+level*.71f+seed;float radius=(1.8f-level*.24f)*(1+.18f*Mathf.Sin(seed+arm*3));
    var start=p+Vector3.up*(height*.42f+level*height*.10f);var end=start+new Vector3(Mathf.Cos(a)*radius,.24f,Mathf.Sin(a)*radius);
    Rod(root,"Crooked_Pine_Bough",start,end,.028f,wood);
    for(int spray=0;spray<3;spray++)
    {
     var tuft=Part(root,"Fine_Pine_Needle_Spray",Vector3.Lerp(start,end,.5f+spray*.25f),new Vector3(radius*.85f,1.05f,radius*.85f),level%2==0?green:dark);tuft.GetComponent<MeshFilter>().sharedMesh=crown;tuft.transform.Rotate(0,a*Mathf.Rad2Deg,12*Mathf.Sin(arm+spray));
    }
   }
   seed++;
  }
  Batch(root,"Pines");
 }
 static void Dress(Transform actor,string role,Mesh rounded)
 {
  var renderers=actor.GetComponentsInChildren<Renderer>(true).Where(r=>r.name.StartsWith("CR_")).ToArray();if(renderers.Length==0)renderers=actor.GetComponentsInChildren<Renderer>(true);
  var p=new Vector3(actor.position.x,renderers.Min(r=>r.bounds.min.y),actor.position.z);var head=actor.GetComponent<HienGuongMatTheoKhu>();var headRoot=head?head.ghostHead:actor;
  if(role=="Ha"||role=="Vy"||role=="Lan")
  {
   var face=actor.GetComponentsInChildren<MeshRenderer>(true).First(r=>r.name.EndsWith("_Face"));var h=face.bounds.center;
   var sourceHair=actor.GetComponentsInChildren<MeshRenderer>(true).FirstOrDefault(r=>r.name.EndsWith("_Hair"));var hair=role=="Lan"?cream:sourceHair?sourceHair.sharedMaterial:wood;
   // Preserve/restore the v40 shoulder hair; costume cleanup must never match it.
   if(!headRoot.GetComponentsInChildren<Transform>(true).Any(t=>t.name=="Ha_Shoulder_Length_Hair"))
   foreach(int side in new[]{-1,1})Part(headRoot,"Ha_Shoulder_Length_Hair",h+new Vector3(side*.115f,-.04f,.045f),new Vector3(.075f,.34f,.17f),hair,PrimitiveType.Sphere);
   if(!headRoot.GetComponentsInChildren<Transform>(true).Any(t=>t.name=="V42_Original_Hair_Cap"))Part(headRoot,"V42_Original_Hair_Cap",h+new Vector3(0,.12f,.025f),new Vector3(.265f,.15f,.255f),hair,PrimitiveType.Sphere);
  }
  // Retain faces and limbs. Distinct tailoring/props are fitted around source meshes.
  if(role=="Bao")
  {
   foreach(var r in renderers.Where(r=>r.name.Contains("Torso")))r.enabled=false;
   var cap=Part(headRoot,"Bao_Wool_Flat_Cap",headRoot.position+Vector3.up*.15f,new Vector3(.34f,.12f,.33f),grey,PrimitiveType.Sphere);
   Part(headRoot,"Bao_Cap_Brim",cap.transform.position+new Vector3(0,-.025f,-.11f),new Vector3(.31f,.025f,.20f),grey,PrimitiveType.Sphere);
   foreach(var r in actor.GetComponentsInChildren<MeshRenderer>(true).Where(r=>r.name=="Tailored_Jacket"||r.name.StartsWith("Tailored_Sleeve")))r.sharedMaterial=green;
   for(int n=0;n<4;n++)Part(actor,"Cardigan_Horn_Button",p+new Vector3(0,.78f+n*.105f,-.16f),Vector3.one*.018f,cream,PrimitiveType.Sphere);
   for(int s=-1;s<=1;s+=2)Part(actor,"Cardigan_Pocket",p+new Vector3(s*.14f,.83f,-.15f),new Vector3(.13f,.12f,.015f),grey);
  }
  if(role=="Ha")
  {
   var apron=Part(actor,"Ha_Ochre_Market_Apron",p+new Vector3(0,.64f,-.16f),new Vector3(.40f,.72f,.045f),red);apron.transform.Rotate(4,0,0);
   Part(actor,"Ha_Apron_Bib",p+new Vector3(0,1.06f,-.16f),new Vector3(.28f,.25f,.035f),red);
   Part(actor,"Ha_Apron_Pocket",p+new Vector3(.07f,.70f,-.19f),new Vector3(.17f,.14f,.015f),cream);
   var bun=Part(headRoot,"Ha_Tied_Hair_Bun",headRoot.position+new Vector3(0,.06f,.15f),new Vector3(.18f,.19f,.15f),wood,PrimitiveType.Sphere);
   Part(headRoot,"Ha_Hair_Tie",bun.transform.position+Vector3.back*.03f,new Vector3(.19f,.035f,.15f),red,PrimitiveType.Sphere);
  }
  if(role=="Lan")
  {
   var skirt=Part(actor,"Lan_Long_Indigo_Coat",p+Vector3.up*.56f,new Vector3(.52f,.92f,.33f),blue);skirt.GetComponent<MeshFilter>().sharedMesh=rounded;
   Part(headRoot,"Lan_Silver_Bun",headRoot.position+new Vector3(0,.10f,.12f),new Vector3(.26f,.22f,.20f),cream,PrimitiveType.Sphere);
  }
  if(role=="Vy")
  {
   Part(actor,"Vy_Student_Backpack",p+new Vector3(0,.96f,.21f),new Vector3(.34f,.45f,.18f),blue,PrimitiveType.Capsule);
   for(int s=-1;s<=1;s+=2)Part(actor,"Vy_Backpack_Strap",p+new Vector3(s*.14f,1.09f,-.15f),new Vector3(.045f,.39f,.025f),blue);
  }
 }
 static void BindVisibleHand(Transform actor,Transform pivot)
 {
  var sleeves=actor.GetComponentsInChildren<MeshRenderer>(true).Where(r=>r.name.StartsWith("Tailored_Sleeve")).ToArray();
  if(sleeves.Length==0)return;var sleeve=sleeves.OrderBy(r=>Vector3.Distance(r.bounds.center,pivot.position)).First();
  string side=sleeve.name.Substring("Tailored_Sleeve_".Length);var hand=actor.GetComponentsInChildren<Transform>(true).FirstOrDefault(t=>t.name=="Hand_"+side);
  sleeve.transform.SetParent(pivot,true);if(hand)hand.SetParent(pivot,true);
 }
 static void ChapterCraft(DieuPhoiHanhTrinh story,TraiNghiemHanhTrinhV42 c)
 {
  var root=Root(story.market.transform,"V42_Market_Detail");var p=story.ghosts[1].position;float floor=story.ghosts[1].GetComponentsInChildren<Renderer>().Min(r=>r.bounds.min.y);p.y=floor;
  // A concrete parcel is sorted by Ha's hand; this is visible scene action.
  c.sortedParcel=Part(root,"Ha_Same_Wrapped_Parcel",p+new Vector3(-.32f,.77f,-.30f),new Vector3(.23f,.14f,.18f),cream).transform;
  for(int n=0;n<4;n++)
  {
   var shelf=p+new Vector3(.65f,.32f+n*.22f,.05f);Part(root,"Stacked_Wooden_Produce_Crate",shelf,new Vector3(.55f,.18f,.42f),wood);
   for(int j=0;j<4;j++)Part(root,"Crate_Slat_Reveal",shelf+new Vector3(0,-.065f+j*.04f,-.215f),new Vector3(.50f,.012f,.012f),dark);
  }
  Batch(root,"MarketStatic"); // Parcel must remain independent of this static batch.
 }
 static void OtherAreas(DieuPhoiHanhTrinh story)
 {
  var root=Root(story.schoolChapter.environment.transform,"V42_School_Joinery");
  var origin=story.schoolChapter.spawn.position-new Vector3(0,.94f,-7);
  for(int n=0;n<3;n++)
  {
   var w=origin+new Vector3(7.92f,1.95f,3+n*4);
   Panel(root,"School_Timber_Window",w,new Vector3(3.85f,1.70f,.09f),wood,Vector3.left);
   Panel(root,"School_Old_Window_Glass",w+Vector3.left*.06f,new Vector3(3.70f,1.53f,.025f),glass,Vector3.left);
   Panel(root,"School_Window_Crossbar",w+Vector3.left*.08f,new Vector3(3.72f,.045f,.03f),cream,Vector3.left);
   Panel(root,"School_Window_Mullion",w+Vector3.left*.08f,new Vector3(.045f,1.55f,.03f),cream,Vector3.left);
  }
  Batch(root,"SchoolJoinery");
  var suitcase=story.slopeChapter.suitcase;var bounds=suitcase.GetComponentsInChildren<Renderer>().First().bounds;var handle=Root(suitcase,"V42_Suitcase_Stitchwork");
  for(int s=-1;s<=1;s+=2)Part(handle,"Packed_Luggage_Leather_Strap",bounds.center+new Vector3(s*bounds.size.x*.3f,0,-bounds.size.z*.51f),new Vector3(.035f,bounds.size.y*.9f,.018f),wood);
  var hub=Root(story.hub.transform,"V42_Hub_Worn_Joinery");
  for(int n=0;n<5;n++){var q=new Vector3(-37+n*.75f,.02f,-36.8f);Part(hub,"Worn_Timber_Floor_Edge",q,new Vector3(.70f,.025f,.11f),wood);}
  Batch(hub,"HubJoinery");
 }
 static void PuppetHands(DieuPhoiHanhTrinh story)
 {
  foreach(var puppet in story.puppets.Concat(new[]{story.schoolChapter.puppet,story.slopeChapter.puppet}).Distinct())
  foreach(var arm in puppet.GetComponentsInChildren<MeshRenderer>(true).Where(r=>r.name=="ConRoi_Arm_L"||r.name=="ConRoi_Arm_R").ToArray())
  {
   var b=arm.bounds;var root=Root(arm.transform,"V42_Articulated_Carved_Hand");var p=new Vector3(b.center.x,b.min.y-.015f,b.center.z);
   Part(root,"Wooden_Wrist_Pin",p,new Vector3(.065f,.055f,.065f),dark,PrimitiveType.Sphere);
   Part(root,"Carved_Palm",p+Vector3.down*.055f,new Vector3(.085f,.095f,.045f),wood,PrimitiveType.Capsule);
   for(int n=0;n<4;n++)
   {
    var start=p+new Vector3((n-1.5f)*.022f,-.09f,-.005f);var end=start+new Vector3((n-1.5f)*.005f,-.055f+(n==0||n==3?.013f:0),-.012f);
    Rod(root,"Separate_Wooden_Finger",start,end,.009f,wood);Part(root,"Finger_Hinge",start,Vector3.one*.014f,dark,PrimitiveType.Sphere);
   }
   Rod(root,"Carved_Thumb",p+new Vector3(.04f,-.04f,0),p+new Vector3(.065f,-.095f,-.01f),.014f,wood);
   Batch(root,"Hand_"+Guid.NewGuid().ToString("N").Substring(0,8)+"_"+arm.name);
  }
 }
 static void ReshapeTrees(DieuPhoiHanhTrinh story)
 {
  foreach(var area in new[]{story.schoolChapter.environment.transform,story.slopeChapter.environment.transform})
  {
   if(area.Find("V42_Branch_Foliage"))continue;var root=Root(area,"V42_Branch_Foliage");
   foreach(var old in area.GetComponentsInChildren<MeshRenderer>(true).Where(r=>r.name=="Pine_Crown"||r.name=="Garden_Cypress_Crown").ToArray())
   {
    var b=old.bounds;old.enabled=false;
    for(int n=0;n<7;n++)
    {
     float angle=n*2.399f;var p=b.center+new Vector3(Mathf.Cos(angle)*b.size.x*.23f,Mathf.Sin(n*1.7f)*b.size.y*.18f,Mathf.Sin(angle)*b.size.z*.23f);
     Rod(root,"Visible_Tree_Twig",b.center-Vector3.up*b.size.y*.15f,p,.018f,wood);
     var spray=Part(root,"Branch_Needles",p,new Vector3(b.size.x*.58f,b.size.y,b.size.z*.58f),n%2==0?green:dark);spray.GetComponent<MeshFilter>().sharedMesh=NeedleSpray();spray.transform.Rotate(0,n*41,0);
    }
   }
   Batch(root,area==story.schoolChapter.environment.transform?"SchoolFoliage":"SlopeFoliage");
  }
 }
 public static void Assemble()
 {
  if(File.Exists(ScenePath))throw new Exception("v42 checkpoint already exists");Directory.CreateDirectory(Art);Directory.CreateDirectory(Report);
  EditorSceneManager.OpenScene(ConRoiCompletionV41.ScenePath);CaptureViews("before");
  wood=Mat("Timber_Worn_Warm",new Color(.24f,.16f,.085f));cream=Mat("Ivory_Lime_Trim",new Color(.66f,.62f,.48f));green=Mat("Pine_Needle_Mid",new Color(.15f,.25f,.13f));dark=Mat("Pine_Needle_Shadow",new Color(.08f,.15f,.10f));glass=Mat("Old_BlueGreen_Glass",new Color(.09f,.20f,.22f));red=Mat("Ha_Terracotta_Apron",new Color(.43f,.20f,.10f));blue=Mat("Lan_Indigo_Vy_Satchel",new Color(.14f,.19f,.29f));grey=Mat("Bao_Grey_Wool",new Color(.29f,.31f,.28f));
  wood.SetTexture("_BaseMap",AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Art/Environment/JourneyV40/TimberGrain.png"));
  var story=UnityEngine.Object.FindAnyObjectByType<DieuPhoiHanhTrinh>();var c=story.gameObject.AddComponent<TraiNghiemHanhTrinhV42>();c.story=story;
  var rounded=Rounded("Rounded_Organic_Section");Facades(story);Pines(story,rounded);
  Dress(story.ghosts[0],"Bao",rounded);Dress(story.ghosts[1],"Ha",rounded);Dress(story.completion.lan,"Lan",rounded);Dress(story.completion.vy,"Vy",rounded);
  // Movable parcel lives outside combined static scenery.
  var ha=story.ghosts[1];float y=ha.GetComponentsInChildren<Renderer>().Min(r=>r.bounds.min.y);
  c.sortedParcel=Part(story.market.transform,"Ha_Same_Wrapped_Parcel",new Vector3(ha.position.x-.32f,y+.77f,ha.position.z-.30f),new Vector3(.23f,.14f,.18f),cream).transform;
  OtherAreas(story);PuppetHands(story);ReshapeTrees(story);
  EditorSceneManager.SaveScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene(),ScenePath);AssetDatabase.SaveAssets();CaptureViews("after");
 }
 public static void BuildWindows()
 {
  EditorSceneManager.OpenScene(ScenePath);EditorBuildSettings.scenes=new[]{new EditorBuildSettingsScene(ScenePath,true)};PlayerSettings.bundleVersion="0.42.0";PlayerSettings.runInBackground=true;PlayerSettings.enableFrameTimingStats=true;
  PlayerSettings.SetUseDefaultGraphicsAPIs(BuildTarget.StandaloneWindows64,false);PlayerSettings.SetGraphicsAPIs(BuildTarget.StandaloneWindows64,new[]{GraphicsDeviceType.Direct3D11});
  var r=BuildPipeline.BuildPlayer(new[]{ScenePath},"Builds/Windows_v42_r7/ConRoi.exe",BuildTarget.StandaloneWindows64,BuildOptions.Development);
  File.WriteAllText(Report+"/build.json",new JObject{["result"]=r.summary.result.ToString(),["errors"]=r.summary.totalErrors,["warnings"]=r.summary.totalWarnings,["path"]=r.summary.outputPath,["graphics_api"]="Direct3D11",["messages"]=new JArray(r.steps.SelectMany(s=>s.messages).Where(m=>m.type==LogType.Warning||m.type==LogType.Error).Select(m=>m.content))}.ToString());
 }
 public static void CaptureViews(string suffix)
 {
  var story=UnityEngine.Object.FindAnyObjectByType<DieuPhoiHanhTrinh>();var camera=story.player.GetComponentInChildren<Camera>();
  var positions=new[]{story.ghosts[0].position+new Vector3(-2,1,-3),story.ghosts[1].position+new Vector3(-4,1,-5),story.schoolChapter.puppet.dauTransform.position+new Vector3(0,0,-4),new Vector3(-35,1.6f,-36.5f)};
  var targets=new[]{story.ghosts[0].position+Vector3.up,story.ghosts[1].position+Vector3.up,story.schoolChapter.puppet.dauTransform.position,new Vector3(-35,1,-40)};
  var oldPosition=camera.transform.position;var oldRotation=camera.transform.rotation;
  var objects=new[]{story.hub,story.lake,story.market,story.schoolChapter.environment};var active=objects.Select(o=>o.activeSelf).ToArray();
  var rt=RenderTexture.GetTemporary(1280,720,24,RenderTextureFormat.ARGB32);var tex=new Texture2D(1280,720,TextureFormat.RGB24,false);Directory.CreateDirectory(Report+"/comparison");
  for(int n=0;n<4;n++)
  {
   for(int j=0;j<objects.Length;j++)objects[j].SetActive(j==(n==0?1:n==1?2:n==2?3:0));
   camera.transform.SetPositionAndRotation(positions[n],Quaternion.LookRotation(targets[n]-positions[n]));
   RenderPipeline.SubmitRenderRequest(camera,new UniversalRenderPipeline.SingleCameraRequest{destination=rt});
   var old=RenderTexture.active;RenderTexture.active=rt;tex.ReadPixels(new Rect(0,0,1280,720),0,0);tex.Apply();RenderTexture.active=old;File.WriteAllBytes(Report+"/comparison/"+new[]{"lake","market","GD3","garden"}[n]+"_"+suffix+".png",tex.EncodeToPNG());
  }
  for(int j=0;j<objects.Length;j++)objects[j].SetActive(active[j]);camera.transform.SetPositionAndRotation(oldPosition,oldRotation);RenderTexture.ReleaseTemporary(rt);UnityEngine.Object.DestroyImmediate(tex);
 }
}
