using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEditor;

// Individually authored cross sections; no imported source face/body mesh reused.
public static class CharacterCraftV44
{
 static string Art=>ConRoiReviewV44.Art;
 static Material skin,hair,ink,lips,ivory,cloth,trousers,shoe;
 public static JArray Evidence=new JArray();
 static Material Mat(string name,Color c){var m=new Material(Shader.Find("Universal Render Pipeline/Lit")){name=name,color=c};m.SetFloat("_Smoothness",.19f);AssetDatabase.CreateAsset(m,Art+"/"+name+".mat");return m;}
 static Transform Root(Transform parent,string name,Vector3 point,Quaternion q){var g=new GameObject(name).transform;g.SetPositionAndRotation(point,q);g.SetParent(parent,true);return g;}
 static GameObject MeshPart(Transform parent,string name,List<Vector3> vertices,List<int> indices,Material mat)
 {
  var mesh=new Mesh{name=name};mesh.SetVertices(vertices);mesh.SetTriangles(indices,0);mesh.SetUVs(0,vertices.Select(v=>new Vector2(v.x,v.y)).ToList());mesh.RecalculateNormals();mesh.RecalculateBounds();
  AssetDatabase.CreateAsset(mesh,Art+"/"+name+".asset");var g=new GameObject(name);g.layer=3;g.transform.SetParent(parent,false);g.AddComponent<MeshFilter>().sharedMesh=mesh;g.AddComponent<MeshRenderer>().sharedMaterial=mat;return g;
 }
 // y, half width, half depth, centre z. Closed loft, smooth ring normals.
 static GameObject Loft(Transform p,string name,Vector4[] rings,Material m,int sides=24,float pleat=0)
 {
  var v=new List<Vector3>();var tr=new List<int>();
  for(int row=0;row<rings.Length;row++)for(int n=0;n<sides;n++)
  {float a=n*Mathf.PI*2/sides;var r=rings[row];float fold=1+pleat*Mathf.Cos(a*8);v.Add(new Vector3(Mathf.Sin(a)*r.y*fold,r.x,Mathf.Cos(a)*r.z*fold+r.w));}
  for(int row=0;row<rings.Length-1;row++)for(int n=0;n<sides;n++){int a=row*sides+n,b=row*sides+(n+1)%sides;tr.AddRange(new[]{a,b,b+sides,a,b+sides,a+sides});}
  int bottom=v.Count;v.Add(new Vector3(0,rings[0].x,rings[0].w));int top=v.Count;v.Add(new Vector3(0,rings.Last().x,rings.Last().w));
  for(int n=0;n<sides;n++){tr.AddRange(new[]{bottom,(n+1)%sides,n,top,(rings.Length-1)*sides+n,(rings.Length-1)*sides+(n+1)%sides});}
  return MeshPart(p,name,v,tr,m);
 }
 static GameObject Ellipsoid(Transform p,string name,Vector3 at,Vector3 size,Material mat)
 {var g=GameObject.CreatePrimitive(PrimitiveType.Sphere);g.name=name;g.layer=3;g.transform.SetParent(p,false);g.transform.localPosition=at;g.transform.localScale=size;g.GetComponent<Renderer>().sharedMaterial=mat;UnityEngine.Object.DestroyImmediate(g.GetComponent<Collider>());return g;}
 static void Limb(Transform p,string name,Vector3 a,Vector3 b,float width,float depth,Material mat)
 {var g=Loft(p,name,new[]{new Vector4(0,width*.72f,depth*.72f,0),new Vector4((b-a).magnitude*.18f,width,depth,0),new Vector4((b-a).magnitude*.82f,width*.87f,depth*.87f,0),new Vector4((b-a).magnitude,width*.67f,depth*.67f,0)},mat,16);g.transform.localPosition=a;g.transform.localRotation=Quaternion.FromToRotation(Vector3.up,b-a);}
 static void Head(Transform parent,string role,Vector3 position,Quaternion rotation)
 {
  var root=Root(parent,"V44_"+role+"_SculptedHead",position,rotation);
  float w=role=="Tung"?.130f:role=="Bao"?.148f:role=="Lan"?.127f:role=="Vy"?.118f:role=="Mai"?.122f:.140f;
  float jaw=role=="Tung"?.77f:role=="Lan"?.66f:role=="Vy"?.55f:role=="Mai"?.64f:.81f;
  float h=role=="Tung"?.177f:role=="Lan"?.160f:.168f;
  Loft(root,role+"_Jaw_Cheek_Forehead",new[]{new Vector4(-h,w*.31f,.064f,-.010f),new Vector4(-h*.81f,w*jaw,.088f,-.005f),new Vector4(-h*.37f,w*.87f,.106f,0),new Vector4(.012f,w,.119f,.005f),new Vector4(h*.47f,w*.94f,.119f,.012f),new Vector4(h*.84f,w*.78f,.10f,.018f),new Vector4(h,w*.27f,.045f,.016f)},skin,32);
  // Nose bridge, orbital lids and jaw use role-specific positions and proportions.
  Ellipsoid(root,role+"_NoseBridge",new Vector3(0,.001f,-.114f),new Vector3(role=="Tung"?.038f:.032f,.088f,.040f),skin);
  Ellipsoid(root,role+"_NoseTip",new Vector3(0,-.023f,-.139f),new Vector3(role=="Bao"?.062f:.044f,.038f,.047f),skin);
  foreach(int side in new[]{-1,1})
  {
   Ellipsoid(root,role+"_Ear"+side,new Vector3(side*w,-.005f,.003f),new Vector3(.036f,.065f,.034f),skin);
   float x=side*w*.47f,y=role=="Lan"?.020f:.030f;
   Ellipsoid(root,role+"_OrbitalLid"+side,new Vector3(x,y,-.105f),new Vector3(.059f,.027f,.028f),skin);
   Ellipsoid(root,role+"_Eye"+side,new Vector3(x,y-.002f,-.121f),new Vector3(.037f,role=="Lan"?.009f:.013f,.009f),ink);
   var brow=Ellipsoid(root,role+"_Brow"+side,new Vector3(x,y+.030f,-.113f),new Vector3(role=="Tung"?.065f:.049f,.010f,.012f),hair);brow.transform.localRotation=Quaternion.Euler(0,0,side*(role=="Tung"?-9:5));
   if(role=="Lan"||role=="Bao")for(int n=0;n<2;n++)Ellipsoid(root,role+"_CheekCrease"+side+"_"+n,new Vector3(side*(w*.66f+n*.004f),-.015f-n*.014f,-.100f),new Vector3(.027f,.003f,.004f),lips);
  }
  Ellipsoid(root,role+"_UpperLip",new Vector3(0,-h*.56f,-.102f),new Vector3(role=="Lan"?.055f:.066f,.006f,.012f),lips);
  Ellipsoid(root,role+"_LowerLip",new Vector3(0,-h*.60f,-.100f),new Vector3(.052f,.009f,.012f),skin);
  if(role=="Tung")
  {
   var cap=Loft(root,"Tung_Asymmetric_SidePart",new[]{new Vector4(.078f,w*.97f,.116f,.027f),new Vector4(.154f,w*1.01f,.119f,.021f),new Vector4(.208f,w*.60f,.080f,.015f),new Vector4(.214f,.025f,.030f,.012f)},hair,32);cap.transform.localRotation=Quaternion.Euler(0,0,-9);
   for(int n=0;n<5;n++){var lockPart=Ellipsoid(root,"Tung_Swept_Lock"+n,new Vector3(-.075f+n*.032f,.128f+n*.005f,-.091f),new Vector3(.095f,.062f,.057f),hair);lockPart.transform.localRotation=Quaternion.Euler(0,0,-24);}
  }
  else if(role=="Vy")
  {
   // A short asymmetric bob, distinct from Ha's broad shoulder hair and Lan's bun.
   Loft(root,"Vy_Fitted_Bob_Back",new[]{new Vector4(-.132f,.120f,.068f,.071f),new Vector4(.03f,.135f,.079f,.068f),new Vector4(.132f,.125f,.091f,.035f),new Vector4(.191f,.045f,.044f,.021f)},hair,28);
   for(int side=-1;side<=1;side+=2)Ellipsoid(root,"Vy_Bob_Side"+side,new Vector3(side*.117f,-.009f,.013f),new Vector3(.061f,side==1?.30f:.26f,.17f),hair);
   var fringe=Ellipsoid(root,"Vy_Diagonal_Fringe",new Vector3(.018f,.113f,-.084f),new Vector3(.23f,.082f,.092f),hair);fringe.transform.localRotation=Quaternion.Euler(0,0,-16);
  }
  else
  {
   Ellipsoid(root,role+"_Fitted_Hair_Crown",new Vector3(0,.119f,.023f),new Vector3(w*2.08f,.132f,.249f),hair);
   if(role=="Lan"||role=="Mai")Ellipsoid(root,role+"_Low_Hair_Knot",new Vector3(0,.025f,.127f),new Vector3(.165f,.145f,.105f),hair);
   if(role=="Ha")foreach(int side in new[]{-1,1})Ellipsoid(root,"Ha_Swept_Side"+side,new Vector3(side*.126f,-.038f,.043f),new Vector3(.082f,.276f,.164f),hair);
   if(role=="Bao"){Ellipsoid(root,"Bao_Wool_Cap_Crown",new Vector3(0,.17f,.009f),new Vector3(.35f,.11f,.32f),cloth);Ellipsoid(root,"Bao_Wool_Cap_Visor",new Vector3(0,.135f,-.10f),new Vector3(.32f,.026f,.19f),cloth);}
  }
 }
 static void Body(Transform actor,string role,Vector3 basePoint,Quaternion rotation,Transform movingArm)
 {
  var body=Root(actor,"V44_"+role+"_Body",basePoint,rotation);
  bool tung=role=="Tung",lan=role=="Lan",vy=role=="Vy";float shoulder=tung?1.43f:lan?1.24f:1.36f;float half=tung?.255f:lan?.214f:.212f;
  float hem=lan?.31f:vy?.88f:.78f;
  Loft(body,role+"_Fitted_Garment",new[]{new Vector4(hem,lan?.265f:half*.90f,.14f,lan?.025f:0),new Vector4(hem+.075f,lan?.253f:half*.93f,.145f,.012f),new Vector4(1.02f,half*.83f,.128f,.020f),new Vector4(shoulder-.13f,half,.138f,.021f),new Vector4(shoulder,half*.90f,.116f,.018f),new Vector4(shoulder+.014f,.072f,.072f,.008f)},cloth,32,lan?.023f:0);
  Limb(body,role+"_Neck",new Vector3(0,shoulder,0),new Vector3(0,shoulder+.13f,0),.052f,.052f,skin);
  if(role=="Mai")Loft(body,"Mai_Flared_Pleated_Skirt",new[]{new Vector4(.23f,.29f,.18f,0),new Vector4(.33f,.28f,.17f,0),new Vector4(.75f,.19f,.12f,0),new Vector4(.87f,.18f,.11f,0)},trousers,32,.035f);
  foreach(int side in new[]{-1,1})
  {
   float x=side*(tung?.114f:.101f);Limb(body,role+"_Tapered_Trouser"+side,new Vector3(x,.13f,0),new Vector3(x,.79f,0),tung?.079f:.067f,.071f,trousers);
   Ellipsoid(body,role+"_Leather_Shoe"+side,new Vector3(x,.067f,-.043f),new Vector3(.139f,.13f,.27f),shoe);
   var upper=new Vector3(side*(half+.015f),shoulder-.055f,.008f);var elbow=new Vector3(side*(half+.055f),shoulder-.31f,-.028f);var wrist=new Vector3(side*(half+.015f),shoulder-.54f,-.095f);
   if(lan&&side==-1){elbow=new Vector3(-.29f,.97f,-.05f);wrist=new Vector3(-.29f,.80f,-.12f);}
   if(vy&&side==1){elbow=new Vector3(.27f,1.05f,-.04f);wrist=new Vector3(.14f,1.24f,-.155f);}
   Limb(body,role+"_UpperSleeve"+side,upper,elbow,.074f,.069f,cloth);Limb(body,role+"_ForeSleeve"+side,elbow,wrist,.058f,.055f,cloth);
   var hand=Ellipsoid(body,role+"_Palm"+side,wrist+Vector3.down*.035f,new Vector3(.075f,.117f,.066f),skin);
   Ellipsoid(hand.transform,role+"_Thumb"+side,new Vector3(-side*.30f,.06f,-.20f),new Vector3(.44f,.50f,.49f),skin);
   if(lan&&side==-1&&movingArm)
   {
    foreach(var r in body.GetComponentsInChildren<MeshRenderer>().Where(r=>r.name==role+"_UpperSleeve"+side||r.name==role+"_ForeSleeve"+side||r.name==role+"_Palm"+side).ToArray())r.transform.SetParent(movingArm,true);
   }
  }
  if(tung)
  {
   // Open collar and a slanted jacket hem, not Bao's rounded closed cardigan.
   Loft(body,"Tung_Shirt_Insert",new[]{new Vector4(1.02f,.07f,.015f,-.132f),new Vector4(1.39f,.093f,.017f,-.125f)},ivory,12);
   foreach(int side in new[]{-1,1}){var lapel=Ellipsoid(body,"Tung_Folded_Collar"+side,new Vector3(side*.075f,1.345f,-.128f),new Vector3(.095f,.172f,.026f),cloth);lapel.transform.localRotation=Quaternion.Euler(0,0,side*24);}
  }
  else if(vy)
  {
   Ellipsoid(body,"Vy_Contoured_Backpack",new Vector3(0,1.04f,.18f),new Vector3(.31f,.43f,.18f),trousers);
   foreach(int side in new[]{-1,1})Limb(body,"Vy_Shoulder_Strap"+side,new Vector3(side*.14f,.96f,-.132f),new Vector3(side*.14f,1.37f,-.07f),.015f,.012f,trousers);
  }
 }
 public static void Actor(Transform actor,string role,bool replaceBody,Transform animatedArm=null)
 {
  var all=actor.GetComponentsInChildren<MeshRenderer>(true);var face=all.First(r=>r.name.EndsWith("_Face"));var source=face.bounds;
  var projection=actor.GetComponent<HienGuongMatTheoKhu>();var headParent=projection?projection.ghostHead:face.transform.parent;
  var nose=all.FirstOrDefault(r=>r.name.EndsWith("_Nose"));var front=nose?nose.bounds.center-source.center:Vector3.back;front.y=0;front.Normalize();var rotation=Quaternion.LookRotation(-front,Vector3.up);
  var visible=all.Where(r=>r.enabled).ToArray();float foot=visible.Min(r=>r.bounds.min.y);var origin=new Vector3(source.center.x,foot,source.center.z);
  skin=Mat(role+"_Skin",role=="Lan"?new Color(.49f,.35f,.25f):role=="Tung"?new Color(.61f,.43f,.31f):new Color(.63f,.47f,.35f));
  hair=Mat(role+"_Hair",role=="Lan"?new Color(.40f,.41f,.37f):role=="Bao"?new Color(.23f,.24f,.20f):new Color(.055f,.044f,.037f));
  ink=Mat(role+"_EyeInk",new Color(.025f,.023f,.019f));lips=Mat(role+"_Lips",new Color(.33f,.19f,.14f));ivory=Mat(role+"_Shirt",new Color(.65f,.61f,.49f));
  cloth=Mat(role+"_Cloth",role=="Tung"?new Color(.17f,.22f,.23f):role=="Lan"?new Color(.14f,.20f,.28f):role=="Vy"?new Color(.49f,.43f,.31f):role=="Bao"?new Color(.22f,.27f,.18f):new Color(.25f,.30f,.27f));
  trousers=Mat(role+"_LowerCloth",new Color(.15f,.17f,.17f));shoe=Mat(role+"_Leather",new Color(.10f,.072f,.046f));
  foreach(var r in all)
  {
   bool head=r.transform.IsChildOf(headParent);
   if(head||(replaceBody&&!r.name.Contains("Watering")))r.enabled=false;
  }
  // New anatomical volumes are explicitly placed in the actor's current world pose.
  var newCenter=source.center;if(replaceBody)newCenter.y=foot+(role=="Tung"?1.66f:role=="Lan"?1.48f:1.60f);
  Head(headParent,role,newCenter,rotation);if(replaceBody)Body(actor,role,origin,rotation,animatedArm);
  Evidence.Add(new JObject{["role"]=role,["actor"]=actor.name,["position"]=origin.ToString("F4"),["new_head_center"]=newCenter.ToString("F4"),["body_replaced"]=replaceBody,["new_mesh_assets"]=AssetDatabase.FindAssets("t:Mesh",new[]{Art}).Select(AssetDatabase.GUIDToAssetPath).Count(p=>System.IO.Path.GetFileName(p).StartsWith(role+"_"))});
 }
 public static void All(DieuPhoiHanhTrinh s)
 {
  Actor(s.slopeChapter.ghost,"Tung",true);Actor(s.completion.lan,"Lan",true,s.completion.lanArm);Actor(s.completion.vy,"Vy",true);Actor(s.schoolChapter.ghost,"Mai",true);Actor(s.ghosts[0],"Bao",false);Actor(s.ghosts[1],"Ha",false);
 }
 public static void Refine(DieuPhoiHanhTrinh s)
 {
  var body=s.completion.vy.GetComponentsInChildren<Transform>(true).First(t=>t.name=="V44_Vy_Body");
  var lower=body.GetComponentsInChildren<MeshRenderer>(true).First(r=>r.name.StartsWith("Vy_Tapered_Trouser")).sharedMaterial;
  Loft(body,"Vy_Fitted_Hip_Join",new[]{new Vector4(.72f,.165f,.10f,0),new Vector4(.79f,.182f,.115f,0),new Vector4(.91f,.177f,.115f,0)},lower,28);
 }
}
