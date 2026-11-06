using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;

// Source-led school envelope. All dimensions are authored game metres, not surveys.
public static class ConRoiR28School
{
 public const string Scene="Assets/Scenes/JourneyRepair_v48_r28.unity",Out="Docs/Reviews/journey_v48/r28",Art="Assets/Art/Environment/JourneyV48/SchoolR28";
 static Transform root;static Vector3 origin=new Vector3(-95,0,-45);
 static Material peach,cream,wall,floor,roof,dark,steel,wood,paper,green;
 static JArray objects;
 public static Vector3 W(float x,float y,float z)=>origin+new Vector3(x,y,z);
 static JArray V(Vector3 v)=>new JArray(v.x,v.y,v.z);
 static Material Mat(string n,Color c,string texture=null){string p=Art+"/"+n+".mat";var m=AssetDatabase.LoadAssetAtPath<Material>(p);if(!m){m=new Material(Shader.Find("Universal Render Pipeline/Lit"));AssetDatabase.CreateAsset(m,p);}m.color=c;m.SetFloat("_Smoothness",.12f);if(texture!=null)m.SetTexture("_BaseMap",AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Art/Environment/JourneyV48/SlopeR27/Textures/"+texture+".png"));return m;}
 static GameObject Box(string n,Vector3 p,Vector3 size,Material m,bool collision=true){var g=GameObject.CreatePrimitive(PrimitiveType.Cube);g.name=n;g.layer=3;g.transform.SetParent(root,false);g.transform.localPosition=p;g.transform.localScale=size;g.GetComponent<Renderer>().sharedMaterial=m;if(!collision)UnityEngine.Object.DestroyImmediate(g.GetComponent<Collider>());objects.Add(new JObject{["object"]=n,["position_local"]=V(p),["size_game_metres"]=V(size),["collision"]=collision});return g;}
 static GameObject B(string n,float x,float y,float z,float sx,float sy,float sz,Material m,bool col=true)=>Box(n,new Vector3(x,y,z),new Vector3(sx,sy,sz),m,col);
 static Transform Label(string n,string text,Vector3 p,float size=.08f,float yaw=0){var g=new GameObject(n);g.transform.SetParent(root,false);g.transform.localPosition=p;g.transform.localRotation=Quaternion.Euler(0,yaw,0);var t=g.AddComponent<TextMesh>();t.text=text;t.fontSize=64;t.characterSize=size*.33f;t.anchor=TextAnchor.MiddleCenter;t.color=new Color(.90f,.89f,.78f);g.GetComponent<Renderer>().sharedMaterial=AssetDatabase.LoadAssetAtPath<Material>("Assets/Art/Environment/SchoolV38/WorldText.mat");return g.transform;}
 static Transform Sign(string n,string text,float x,float y,float z,float width,float height,float size=.09f){B(n+"_Back",x,y,z,width,height,.075f,dark);var t=Label(n,text,new Vector3(x,y,z-.045f),size);Fit(t,width*.88f,height*.82f);return t;}
 static void Fit(Transform t,float w,float h){var b=t.GetComponent<Renderer>().bounds;float f=Mathf.Min(1,w/b.size.x,h/b.size.y);t.localScale*=f;}
 static void Rod(string n,Vector3 a,Vector3 b,float radius,Material m,bool collision=false){var g=GameObject.CreatePrimitive(PrimitiveType.Cylinder);g.name=n;g.layer=3;g.transform.SetParent(root,false);g.transform.localPosition=(a+b)*.5f;g.transform.up=b-a;g.transform.localScale=new Vector3(radius*2,(b-a).magnitude*.5f,radius*2);g.GetComponent<Renderer>().sharedMaterial=m;if(!collision)UnityEngine.Object.DestroyImmediate(g.GetComponent<Collider>());}
 static void Mesh(string n,Vector3[] v,int[] t,Material m){var mesh=new Mesh{name=n};mesh.vertices=v;mesh.triangles=t;mesh.uv=v.Select(p=>new Vector2(p.x*.3f,p.z*.3f)).ToArray();mesh.RecalculateNormals();mesh.RecalculateBounds();string p=Art+"/"+n+".asset";var old=AssetDatabase.LoadAssetAtPath<Mesh>(p);if(old){EditorUtility.CopySerialized(mesh,old);mesh=old;}else AssetDatabase.CreateAsset(mesh,p);var g=new GameObject(n);g.layer=3;g.transform.SetParent(root,false);g.AddComponent<MeshFilter>().sharedMesh=mesh;g.AddComponent<MeshRenderer>().sharedMaterial=m;g.AddComponent<MeshCollider>().sharedMesh=mesh;}
 static void Gable(string n,float x,float z,float y,float width,float rise,float depth,Material m){float a=x-width*.5f,b=x+width*.5f;Mesh(n,new[]{new Vector3(a,y,z-depth*.5f),new Vector3(b,y,z-depth*.5f),new Vector3(x,y+rise,z-depth*.5f),new Vector3(a,y,z+depth*.5f),new Vector3(b,y,z+depth*.5f),new Vector3(x,y+rise,z+depth*.5f)},new[]{0,2,1,3,4,5,0,1,4,0,4,3,1,2,5,1,5,4,2,0,3,2,3,5},m);}
 static void Roof(string n,float x,float z,float eave,float width,float depth,float rise){
  // Two sloped sheets with closed fascia: ridge runs along the facade.
  float angle=Mathf.Atan2(rise,depth*.5f)*Mathf.Rad2Deg;float half=Mathf.Sqrt(depth*depth*.25f+rise*rise);
  foreach(int side in new[]{-1,1}){var g=B(n+"_Slope",x,eave+rise*.5f,z+side*depth*.25f,width,.14f,half,roof);g.transform.localRotation=Quaternion.Euler(side*angle,0,0);}
  B(n+"_Ridge",x,eave+rise+.04f,z,width,.16f,.24f,roof);
  foreach(int side in new[]{-1,1})B(n+"_Eave",x,eave,z+side*depth*.5f,width,.21f,.17f,cream);
 }
 static void Light(string n,Vector3 p,float power,float range){var g=new GameObject(n);g.transform.SetParent(root,false);g.transform.localPosition=p;var l=g.AddComponent<Light>();l.type=LightType.Point;l.color=new Color(1,.89f,.69f);l.intensity=power;l.range=range;l.shadows=LightShadows.Soft;B(n+"_Shade",p.x,p.y+.06f,p.z,.45f,.12f,.32f,cream,false);}
 static void Window(float x,float y,float z,float width=2.1f){
  B("Recessed_Window",x,y,z,width,1.5f,.07f,dark);
  foreach(float yy in new[]{y-.79f,y+.79f})B("Window_Horizontal_Frame",x,yy,z-.07f,width+.17f,.10f,.18f,cream);
  foreach(float xx in new[]{x-width*.5f,x,x+width*.5f})B("Window_Mullion",xx,y,z-.09f,.065f,1.58f,.16f,wood,false);
 }
 static void Ground(){
  B("School_Ground_Support",0,-.28f,-9,64,.56f,60,floor);
  B("Court_Main_Paving",0,-.05f,-16,45,.10f,28,floor);
  for(int x=-20;x<=20;x+=4)B("Court_ExpansionJoint",x,.002f,-16,.018f,.004f,27,wall,false);
  // Main entrance path stays visibly open through the gate and porch.
  B("Porch_Stage",0,.12f,-1.6f,10,.24f,5.6f,floor);
  for(int j=0;j<2;j++)B("Porch_ApproachStep",0,.04f+j*.08f,-4.8f+j*.4f,9.4f,.08f+j*.16f,.4f,floor);
  foreach(int side in new[]{-1,1}){
   B("Court_SideBoundary",side*24,1.0f,-10,.3f,2,41,wall);
   B("Gate_LowBoundary",side*15,.45f,-30,18,.9f,.4f,wall);
   for(int i=0;i<28;i++)B("Gate_FenceUpright",side*(6.5f+i*.62f),1.5f,-30,.055f,1.25f,.055f,steel);
   B("Gate_StonePier",side*5.3f,1.9f,-30,.78f,3.8f,.85f,wall);
   B("Gate_PierCap",side*5.3f,3.85f,-30,1,.18f,1.08f,cream);
   B("Gate_LampBase",side*5.3f,4.03f,-30,.34f,.18f,.34f,cream);
  }
  Sign("R28_MainSchoolName","TRƯỜNG ĐÀ LẠT",0,4.45f,-30,11.5f,.85f,.19f);
  B("Gate_HeaderSupport",0,4.02f,-30,11.4f,.16f,.45f,steel);
  // Gate leaves are open alongside the piers; no bars block the player's route.
  foreach(int side in new[]{-1,1})for(int i=0;i<9;i++)B("Open_GateLeaf",side*5.4f,1.4f,-28.7f+i*.3f,.07f,2.6f,.045f,steel);
  Sign("Entry_Wayfinding","SẢNH  ↑\nTẦNG 2 · LỚP 203",-3.0f,1.35f,-25.6f,3.5f,.9f,.10f);
  B("Entry_SignPost",-3,.47f,-25.6f,.12f,.94f,.12f,steel);
 }
 static void Building(){
  // Three readable storeys; open galleries in front of rear classrooms.
  for(int level=0;level<3;level++){
   float y=level*3.6f;
   if(level==0)B("Ground_WingFloor",0,.02f,7.5f,47,.24f,15,floor);
   else {
    B("Gallery_West_Slab",-16,y-.11f,2,15,.22f,4,floor);
    B("Gallery_East_Slab",9.4f,y-.11f,2,28.2f,.22f,4,floor);
    B("Rear_Classrooms_Slab",0,y-.11f,9.7f,47,.22f,11.4f,floor);
    // Landing bridges the stair to the gallery at the head, keeping its opening clear.
    B("Stair_HeadLanding",-6.5f,y-.11f,4.7f,3.6f,.22f,1.4f,floor);
   }
   B("Rear_Wall",0,y+1.75f,15,47,3.5f,.24f,wall);
   foreach(int side in new[]{-1,1})B("End_Wall",side*23.5f,y+1.75f,9.8f,.28f,3.5f,10.4f,wall);
   B("Classroom_FrontLower",0,y+.5f,5.4f,47,1,.23f,wall);
   B("Classroom_FrontHeader",0,y+3.12f,5.4f,47,.96f,.23f,wall);
   // Door openings replace wall from floor to lintel at the authored room bays.
   // Remove lower strip at room 203 by splitting that one continuous strip below.
   for(int bay=0;bay<12;bay++){
    float x=-22+bay*4;
    if(x!=-6)B("Gallery_BearingColumn",x,y+1.75f,.05f,.34f,3.5f,.40f,peach);
    B("Classroom_WindowPier",x,y+1.7f,5.4f,.3f,1.45f,.24f,wall);
    if(!(level==1&&x>=2&&x<=10))Window(x+1.8f,y+1.8f,5.48f,2.9f);
    if(level>0&&x!=-10&&x!=-6){
     B("Gallery_RailBase",x+1.83f,y+.28f,.08f,3.35f,.35f,.22f,cream);
     for(float h=.52f;h<=1.13f;h+=.30f)B("Gallery_OpenRail",x+1.83f,y+h,.08f,3.35f,.085f,.12f,cream);
    }
   }
   if(level>0){
    foreach(float h in new[]{.45f,.78f,1.1f}){
     B("StairBay_WestGalleryGuard",-9.2f,y+h,.08f,1.3f,.10f,.16f,cream);
     B("StairBay_EastGalleryGuard",-3.55f,y+h,.08f,2.2f,.10f,.16f,cream);
     foreach(float edge in new[]{-8.5f,-4.7f})B("Stairwell_SideGuard",edge,y+h,2,.12f,.10f,4,cream);
    }
    foreach(float edge in new[]{-8.5f,-4.7f})foreach(float z in new[]{0f,2f,3.9f})B("Stairwell_GuardPost",edge,y+.58f,z,.10f,1.16f,.10f,cream);
   }
   B("Floor_Cornice_West",-16,y+3.45f,-.06f,15.6f,.19f,.48f,cream);B("Floor_Cornice_East",9.6f,y+3.45f,-.06f,28.4f,.19f,.48f,cream);
  }
  // Delete the front strips where a real door is needed; rebuild around the opening.
  foreach(var t in root.GetComponentsInChildren<Transform>().Where(t=>t.name=="Classroom_FrontLower"&&t.localPosition.y>3&&t.localPosition.y<5).ToArray())UnityEngine.Object.DestroyImmediate(t.gameObject);
  B("203_FrontLower_West",-10.25f,4.1f,5.4f,26.5f,1,.23f,wall);
  B("203_FrontLower_East",14.75f,4.1f,5.4f,17.5f,1,.23f,wall);
  // Door at x4.6, clear width 2.6m. Other part of wall has windows with reveals.
  for(float x=1.1f;x<14;x+=3.2f){if(x>2.5f&&x<6.3f)continue;Window(x,5.4f,5.48f,2.5f);}
  foreach(float x in new[]{3.25f,6.0f})B("203_DoorJamb",x,4.9f,5.28f,.15f,2.6f,.3f,wood);
  B("203_DoorLintel",4.625f,6.25f,5.28f,2.9f,.18f,.3f,wood);
  var leaf=B("203_OpenDoor",6.05f,4.86f,6.08f,.12f,2.5f,1.55f,wood); // open along wall, not across entry
  Sign("203_RoomPlate","203",6.75f,5.65f,5.24f,1.0f,.48f,.15f);
  // Central projecting volume and two tall arched recesses under the pediment.
  foreach(int side in new[]{-1,1}){
   B("Central_BearingPilaster",side*4.8f,5.2f,-.35f,.45f,10.4f,.60f,peach);
   B("Central_TallRecess",side*3.45f,7.85f,-.32f,1.55f,3.9f,.14f,dark);
   for(int j=0;j<13;j++){float a=j*Mathf.PI/12;var g=B("Central_ArchMoulding",side*3.45f+Mathf.Cos(a)*.82f,9.25f+Mathf.Sin(a)*.82f,-.48f,.15f,.23f,.18f,cream,false);g.transform.localRotation=Quaternion.Euler(0,0,a*Mathf.Rad2Deg-90);}
  }
  B("Central_Header",0,10.42f,-.32f,10.2f,.22f,.6f,cream);
  Gable("Central_Pediment",0,-.24f,10.55f,11,1.65f,.50f,peach);
  foreach(int side in new[]{-1,1})Rod("Pediment_Trim",new Vector3(side*5.55f,10.53f,-.52f),new Vector3(0,12.22f,-.52f),.07f,cream);
  Roof("Main_TiledRoof",0,7.1f,10.65f,49,16.5f,1.6f);
  // Roof ribs read as tile courses instead of a flat thin slab.
  for(int x=-24;x<=24;x++)Rod("Roof_TileRib",new Vector3(x,10.78f,-1.15f),new Vector3(x,12.35f,7.1f),.025f,roof);
  // Right wing and covered link are inferred from the visible connected wing in T01.
  B("Right_Wing_Core",29,3.45f,8,10,6.9f,12,peach);
  for(int j=0;j<3;j++)Window(26+j*3,4.85f,1.94f,2.1f);
  B("Right_Wing_Link",24,3.5f,3.3f,2,.22f,3.8f,floor);
  Roof("Right_Wing_Roof",29,8,7.05f,11,13.5f,1.65f);
  // S02-style court planting is not reused; preserve a clean school court silhouette.
  foreach(float x in new[]{-16,17}){
   B("Court_TreeBed",x,.20f,-14,3.7f,.4f,3.7f,wall);
   Rod("Court_TreeTrunk",new Vector3(x,.4f,-14),new Vector3(x,5,-14),.16f,wood,true);
   for(int j=0;j<7;j++){float a=j*2.4f;var end=new Vector3(x+Mathf.Cos(a)*2.2f,4.7f+j*.24f,-14+Mathf.Sin(a)*2.2f);Rod("Court_TreeBranch",new Vector3(x,3.8f,-14),end,.055f,wood);var g=GameObject.CreatePrimitive(PrimitiveType.Sphere);g.name="Court_Foliage";g.transform.SetParent(root,false);g.transform.localPosition=end;g.transform.localScale=new Vector3(1.9f,2.1f,1.8f);g.GetComponent<Renderer>().sharedMaterial=green;UnityEngine.Object.DestroyImmediate(g.GetComponent<Collider>());}
  }
 }
 static void StairsAndPorch(){
  B("Stair_Foundation",-6.5f,.12f,.57f,3,.24f,7.14f,floor);
  B("Stair_ApproachLow",-6.5f,.06f,-3.5f,3,.12f,.4f,floor);
  B("Stair_ApproachHigh",-6.5f,.12f,-3.15f,3,.24f,.3f,floor);
  for(int i=0;i<21;i++){float top=.24f+(i+1)*.16f;B("Stair_203_Tread_"+i,-6.5f,(top+.24f)*.5f,-3.0f+(i+.5f)*.34f,3.0f,top-.24f,.34f,floor);B("Stair_Nosing",-6.5f,top-.016f,-3+i*.34f+.025f,3,.032f,.06f,cream,false);}
  // top = 3.6, head z4.14. Rails follow slope, landing is fully supported.
  foreach(float x in new[]{-8.10f,-4.90f}){
   Rod("Stair_Handrail",new Vector3(x,1.16f,-3),new Vector3(x,4.52f,4.14f),.045f,steel,true);
   for(int j=0;j<8;j++){float z=-3+j*1.02f;float h=.24f+(z+3)/7.14f*3.36f;Rod("Stair_RailPost",new Vector3(x,h,z),new Vector3(x,h+.9f,z),.025f,steel);}
  }
  // Ground porch columns are round and bear into the raised stage like T02.
  foreach(float x in new[]{-4.2f,4.2f}){
   Rod("Porch_RoundColumn",new Vector3(x,.24f,-3.5f),new Vector3(x,3.32f,-3.5f),.23f,peach,true);
   foreach(float y in new[]{.34f,3.22f})B("Porch_ColumnCapital",x,y,-3.5f,.62f,.17f,.62f,cream);
  }
  Gable("Porch_Canopy",0,-2.45f,3.32f,10.5f,.86f,4.9f,cream);
  Roof("Porch_TiledCap",0,-2.45f,3.38f,10.8f,5.2f,.88f);
  Sign("Stair_Wayfinding","← CẦU THANG\nTẦNG 2 · 203",-2.35f,1.85f,-.5f,3.0f,1.0f,.105f);
  Sign("Landing_Wayfinding","203 →",-4.0f,5.05f,5.23f,1.5f,.55f,.12f);
  Sign("Corridor_Wayfinding","201    202    203 →",0,5.9f,5.21f,3.6f,.42f,.09f);
  Light("Stair_LandingLamp",new Vector3(-6.5f,6.45f,4.6f),1.2f,6);
  Light("203_EntryLamp",new Vector3(4.6f,6.35f,4.75f),1.3f,5);
 }
 static void Furniture(DieuPhoiHanhTrinh s,TraiNghiemTruongR28 ex){
  // Classroom 203 is a designed dream interior; no source claims for its hidden plan.
  B("203_WestWall",.6f,5.25f,10,.22f,3.3f,9.4f,wall);
  B("203_East_Sill",14.3f,4.15f,10,.22f,1.1f,9.4f,wall);
  B("203_East_Header",14.3f,6.65f,10,.22f,.5f,9.4f,wall);
  for(int i=0;i<5;i++){
   float z=6+i*1.9f;B("203_EastWindow_Mullion",14.3f,5.45f,z,.18f,1.5f,.08f,wood);
   B("203_EastWindow_Sill",14.3f,4.75f,z,.4f,.10f,1.8f,cream);
  }
  B("203_WallSkirting",.75f,3.68f,10,.08f,.16f,9.3f,wood,false);
  B("203_Chalkboard_Frame",7.5f,5.3f,14.73f,6.3f,1.65f,.13f,wood);
  B("203_Chalkboard",7.5f,5.3f,14.64f,6.05f,1.43f,.06f,dark);
  B("203_ChalkTray",7.5f,4.53f,14.45f,6.2f,.07f,.28f,wood);
  Fit(Label("203_BoardWriting","203\nĐÁP ÁN MẪU\nĐặt bài vào khay bên phải",new Vector3(7.5f,5.35f,14.59f),.10f),5.4f,1.18f);
  for(int row=0;row<3;row++)foreach(float x in new[]{2.6f,11.5f}){
   float z=7.4f+row*1.9f;
   var d=ConRoiSchoolFurnitureV37.Create("Ban_Sinh_Vien_R28",W(x,3.6f,z),.76f,root);d.transform.rotation=Quaternion.Euler(0,180,0);
   var c=ConRoiSchoolFurnitureV37.Create("Ghe_Sinh_Vien_R28",W(x,3.6f,z-.85f),.86f,root);c.transform.rotation=Quaternion.Euler(0,180,0);
  }
  var teacher=ConRoiSchoolFurnitureV37.Create("Ban_Giao_Vien_R28",W(11,3.6f,13.0f),.80f,root);
  var chair=ConRoiSchoolFurnitureV37.Create("Ghe_Giao_Vien_R28",W(12.8f,3.6f,13.2f),.94f,root);chair.transform.rotation=Quaternion.Euler(0,90,0);
  s.schoolChapter.chair=chair.transform;ex.traceChair=chair.transform;ex.chairWindow=chair.transform.rotation;ex.chairBoard=Quaternion.Euler(0,180,0);
  s.completion.traceDetails[2]=chair.transform;
  var pd=ConRoiSchoolFurnitureV37.Create("Ban_Sinh_Vien_Puppet_R28",W(5.5f,3.6f,10.4f),.76f,root);ex.puppetDesk=pd.transform;
  var rd=ConRoiSchoolFurnitureV37.Create("Ban_Sinh_Vien_Response_R28",W(8.3f,3.6f,10.4f),.76f,root);ex.responseDesk=rd.transform;
  var pp=B("Puppet_Paper",5.18f,4.371f,10.38f,.30f,.022f,.39f,paper,false);ex.puppetPaper=pp.transform;ex.puppetPaperRest=pp.transform.localPosition;
  var rp=B("Player_Paper",7.98f,4.371f,10.38f,.30f,.022f,.39f,paper);ex.responsePaper=rp.transform;ex.responsePaperRest=rp.transform.localPosition;
  var interaction=rp.AddComponent<TuongTacTruongR28>();interaction.school=ex;
  foreach(float x in new[]{5.64f,8.44f}){
   B("Right_PaperTray_Base",x,4.367f,10.38f,.35f,.014f,.44f,wood);
   foreach(float dx in new[]{-.19f,.19f})B("PaperTray_Edge",x+dx,4.40f,10.38f,.025f,.065f,.47f,wood);
  }
  Light("203_SoftCeiling",new Vector3(7.4f,6.25f,10.4f),.75f,7);
  Light("203_BoardFill",new Vector3(9.8f,6.2f,13.3f),.55f,5);
  var clock=B("203_Clock",7.5f,6.6f,14.68f,.5f,.5f,.06f,cream,false);
  s.schoolChapter.clockHand=B("203_ClockHand",7.5f,6.65f,14.62f,.025f,.2f,.025f,dark,false).transform;
  // Same actor identities and face bindings; plant feet on the authored classroom slab.
  var maiBody=s.schoolChapter.ghost.GetComponentsInChildren<Transform>(true).First(t=>t.name=="V44_Mai_Body");
  foreach(var r in maiBody.GetComponentsInChildren<Renderer>(true))if(r.name=="Mai_Fitted_Garment"||r.name=="Mai_Neck"||r.name.StartsWith("V45_Mai_"))r.enabled=true;
  MoveActor(s.schoolChapter.ghost,W(10.4f,3.6f,12.1f));
  var maiProxy=new GameObject("Mai_R28_Interaction");maiProxy.layer=3;maiProxy.transform.SetParent(root,false);maiProxy.transform.position=W(10.4f,4.45f,12.1f);var mc=maiProxy.AddComponent<BoxCollider>();mc.size=new Vector3(.65f,1.7f,.5f);var mi=maiProxy.AddComponent<TuongTacTruongV37>();mi.chapter=s.schoolChapter;
  File.WriteAllText(Out+"/mai-renderers.json",new JArray(s.schoolChapter.ghost.GetComponentsInChildren<Renderer>(true).Select(r=>new JObject{["name"]=r.name,["enabled"]=r.enabled,["active"]=LocallyActive(r.transform,s.schoolChapter.ghost),["min"]=V(r.bounds.min),["max"]=V(r.bounds.max)})).ToString());
  MoveActor(s.schoolChapter.puppet.transform,W(5.5f,3.6f,11.1f));
  foreach(var r in s.schoolChapter.puppet.gesturePart.GetComponentsInChildren<Renderer>(true))r.enabled=false;
  var shoulder=new GameObject("GD3_RightShoulder");shoulder.transform.SetParent(root,false);shoulder.transform.position=W(5.7f,4.96f,11.06f);ex.leadShoulder=shoulder.transform;
  var upper=B("GD3_UpperArm",5.7f,4.72f,11,.11f,.25f,.11f,wood,false);ex.leadUpper=upper.transform;
  var fore=B("GD3_ForeArm",5.7f,4.47f,11,.09f,.25f,.09f,wood,false);ex.leadFore=fore.transform;
  ex.leadHand=B("GD3_Hand",5.7f,4.43f,11,.13f,.06f,.16f,wood,false).transform;
  // Retain optional archive texts, now grounded on a classroom shelf.
  var docs=s.schoolChapter.environment.GetComponentsInChildren<DaoCuKeChuyenV40>(true).Where(d=>!d.transform.IsChildOf(root)).Take(3).ToArray();
  B("203_ArchiveShelfTop",1.4f,4.32f,13.1f,1.25f,.12f,2.9f,wood);
  foreach(float z in new[]{12.1f,14.1f})foreach(float x in new[]{.92f,1.87f})B("203_ArchiveShelfLeg",x,3.96f,z,.07f,.72f,.07f,wood);
  for(int i=0;i<docs.Length;i++){var copy=UnityEngine.Object.Instantiate(docs[i].gameObject,root);copy.SetActive(true);copy.transform.position=W(1.4f,4.4f,12.3f+i*.78f);}
 }
 static bool LocallyActive(Transform t,Transform actor){while(t&&t!=actor){if(!t.gameObject.activeSelf)return false;t=t.parent;}return true;}
 static void MoveActor(Transform actor,Vector3 foot){actor.gameObject.SetActive(true);var rr=actor.GetComponentsInChildren<Renderer>().Where(r=>r.enabled&&LocallyActive(r.transform,actor)&&!(r is ParticleSystemRenderer)&&!r.GetComponent<TextMesh>()).ToArray();float bottom=rr.Min(r=>r.bounds.min.y);var b=rr[0].bounds;foreach(var r in rr)b.Encapsulate(r.bounds);actor.position+=new Vector3(foot.x-b.center.x,foot.y-bottom,foot.z-b.center.z);}
 static void Exit(DieuPhoiHanhTrinh s,float x,float y,float z,string name){var g=B(name,x,y+1.15f,z,1.35f,1.65f,.14f,dark);var it=g.AddComponent<TuongTacTruongV37>();it.chapter=s.schoolChapter;it.exit=true;Fit(Label(name+"_Text","TỈNH DẬY\nE · Về phòng trọ",new Vector3(x,y+1.15f,z-.09f),.09f),1.2f,1.35f);B(name+"_Post",x,y+.22f,z,.14f,.44f,.14f,steel);}
 public static void Author(){if(File.Exists(Scene))throw new Exception("Keep existing r28; use Refine");if(!AssetDatabase.CopyAsset(ConRoiR27Slope.Scene,Scene))throw new Exception("Copy failed");Refine();}
 public static void Refine(){
  Directory.CreateDirectory(Out);Directory.CreateDirectory(Art);EditorSceneManager.OpenScene(Scene);var s=UnityEngine.Object.FindAnyObjectByType<DieuPhoiHanhTrinh>();var ch=s.schoolChapter;
  // Every prior school object remains serialized for provenance; only replacement school is active.
  var owned=ch.environment.transform.Find("R28_School");if(owned)UnityEngine.Object.DestroyImmediate(owned.gameObject);
  foreach(Transform t in ch.environment.transform)if(t!=ch.ghost&&t!=ch.puppet.transform&&t!=ch.spawn)t.gameObject.SetActive(false);
  var parent=new GameObject("R28_School");parent.transform.SetParent(ch.environment.transform);parent.transform.position=origin;root=parent.transform;objects=new JArray();
  peach=Mat("Peach_Render",new Color(.69f,.43f,.35f),"Limewash");cream=Mat("Ivory_Trim",new Color(.84f,.80f,.67f));wall=Mat("Warm_Plaster",new Color(.68f,.67f,.58f),"Limewash");floor=Mat("Terrazzo",new Color(.49f,.50f,.45f),"Concrete");roof=Mat("Terracotta",new Color(.48f,.23f,.14f));dark=Mat("Blackboard",new Color(.055f,.105f,.095f));steel=Mat("Gate_Steel",new Color(.14f,.22f,.22f));wood=Mat("Aged_Wood",new Color(.34f,.25f,.14f));paper=Mat("Paper",new Color(.79f,.77f,.64f));green=Mat("School_Foliage",new Color(.17f,.28f,.12f));
  var ex=s.GetComponent<TraiNghiemTruongR28>();if(!ex)ex=s.gameObject.AddComponent<TraiNghiemTruongR28>();ex.story=s;
  Ground();Building();StairsAndPorch();Furniture(s,ex);Exit(s,-3,0,-28.5f,"Gate_Return");Exit(s,2.1f,3.6f,5.12f,"203_Return");
  ch.spawn.SetPositionAndRotation(W(0,1.0f,-33),Quaternion.identity);
  ch.environment.SetActive(false);Physics.SyncTransforms();EditorSceneManager.SaveScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene());AssetDatabase.SaveAssets();
  File.WriteAllText(Out+"/assembly.json",new JObject{["scene"]=Scene,["scope"]="School replacement only; hidden plan/interior/stair dimensions designed for gameplay, not surveyed",["objects"]=objects,["spawn"]=V(ch.spawn.position),["mai"]=V(ch.ghost.position),["puppet"]=V(ch.puppet.transform.position),["class203_floor"]=3.6f}.ToString());Views();
 }
 public static void Views(){
  var poses=new[]{("exterior",W(0,1.7f,-33),W(0,5.5f,2)),("courtyard",W(-11,1.7f,-23),W(1,5.3f,4)),("porch",W(1,1.7f,-9),W(-.5f,2.7f,-1)),("stairs",W(-6.5f,1.8f,-5.2f),W(-6.5f,4.5f,4.5f)),("gallery",W(-3,5.25f,2.1f),W(6,5.25f,5.6f)),("class203",W(4.65f,5.3f,6.2f),W(7.9f,5.0f,12.5f)),("mai-board",W(8.4f,5.3f,10),W(10.5f,5.2f,13.5f))};
  foreach(var version in new[]{("r27",ConRoiR27Slope.Scene),("r28",Scene)}){
   EditorSceneManager.OpenScene(version.Item2);var s=UnityEngine.Object.FindAnyObjectByType<DieuPhoiHanhTrinh>();s.hub.SetActive(false);s.lake.SetActive(false);s.market.SetActive(false);s.slopeChapter.environment.SetActive(false);s.schoolChapter.environment.SetActive(true);
   var pipe=s.GetComponent<HoSoSceneR26>().pipeline;GraphicsSettings.defaultRenderPipeline=pipe;QualitySettings.renderPipeline=pipe;RenderSettings.fog=false;Shader.SetGlobalVector("_MistProfile",Vector4.zero);RenderSettings.ambientMode=AmbientMode.Flat;RenderSettings.ambientLight=new Color(.57f,.60f,.61f);
   foreach(var l in UnityEngine.Object.FindObjectsByType<Light>(FindObjectsInactive.Include))if(l.type==LightType.Directional){l.intensity=1.15f;l.transform.rotation=Quaternion.Euler(48,-32,0);l.color=new Color(1,.94f,.84f);}
   var cam=new GameObject("R28ReviewCamera").AddComponent<Camera>();cam.fieldOfView=65;cam.farClipPlane=180;cam.nearClipPlane=.1f;cam.clearFlags=CameraClearFlags.SolidColor;cam.backgroundColor=new Color(.44f,.54f,.61f);var data=new JArray();
   foreach(var p in poses){cam.transform.position=p.Item2;cam.transform.LookAt(p.Item3);ConRoiR22Scenery.Capture(cam,Out+"/"+version.Item1+"/"+p.Item1+".png");data.Add(new JObject{["name"]=p.Item1,["eye"]=V(p.Item2),["target"]=V(p.Item3),["fov"]=65});}
   var ep=version.Item1=="r27"?W(0,1.65f,3.5f):W(4.65f,5.3f,6.2f);var et=version.Item1=="r27"?W(3,1.1f,10):W(7.9f,5,12.5f);cam.transform.position=ep;cam.transform.LookAt(et);ConRoiR22Scenery.Capture(cam,Out+"/"+version.Item1+"/class203-functional.png");File.WriteAllText(Out+"/"+version.Item1+"/functional-camera.json",new JObject{["eye"]=V(ep),["target"]=V(et),["method"]="Equivalent classroom function; different cameras, because r28 moves room upstairs"}.ToString());
   File.WriteAllText(Out+"/"+version.Item1+"/cameras.json",data.ToString());UnityEngine.Object.DestroyImmediate(cam.gameObject);
  }
 }
}
