using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;

// Native scene models; the source Blender assets remain untouched.
public static class ConRoiSchoolFurnitureV37
{
 const string Folder="Assets/Art/Environment/SchoolV37";
 static Material Material(string name,Color color)
 {
  string path=Folder+"/"+name+".mat";var material=AssetDatabase.LoadAssetAtPath<Material>(path);
  if(!material){material=new Material(Shader.Find("Universal Render Pipeline/Lit"));AssetDatabase.CreateAsset(material,path);}
  material.color=color;material.SetFloat("_Smoothness",.23f);return material;
 }
 static void Part(Transform owner,string name,Vector3 p,Vector3 size,Material material)
 {
  var part=GameObject.CreatePrimitive(PrimitiveType.Cube);part.name=name;part.layer=3;part.transform.SetParent(owner,false);part.transform.localPosition=p;part.transform.localScale=size;part.GetComponent<Renderer>().sharedMaterial=material;
 }
 public static GameObject Create(string name,Vector3 position,float height,Transform owner)
 {
  var root=new GameObject(name);root.transform.SetParent(owner);root.transform.position=position;
  var wood=Material("Furniture_Aged_Wood",new Color(.36f,.245f,.125f));var top=Material("Furniture_Worn_Top",new Color(.46f,.34f,.20f));var steel=Material("Furniture_Steel",new Color(.105f,.125f,.12f));
  bool chair=name.StartsWith("Ghe_");
  if(chair)
  {
   float width=name.Contains("Giao_Vien")?.5f:.44f;float depth=.46f;float seat=height*.52f;
   Part(root.transform,"Chair_Seat",new Vector3(0,seat,0),new Vector3(width,.055f,depth),top);
   foreach(int side in new[]{-1,1})
   {
    Part(root.transform,"Front_Leg",new Vector3(side*width*.42f,(seat-.03f)*.5f,depth*.38f),new Vector3(.045f,seat-.03f,.045f),steel);
    Part(root.transform,"Back_Leg_And_Upright",new Vector3(side*width*.42f,height*.5f,-depth*.4f),new Vector3(.045f,height-.04f,.045f),steel);
    Part(root.transform,"Side_Stretcher",new Vector3(side*width*.42f,.17f,0),new Vector3(.035f,.035f,depth*.8f),steel);
   }
   foreach(float y in new[]{height*.75f,height*.89f})Part(root.transform,"Wooden_Back_Slat",new Vector3(0,y,-depth*.43f),new Vector3(width,.085f,.04f),wood);
  }
  else
  {
   float width=name.Contains("Giao_Vien")?1.65f:1.45f;float depth=.68f;float leg=height-.065f;
   Part(root.transform,"Desk_Top",new Vector3(0,height-.0325f,0),new Vector3(width,.065f,depth),top);
   foreach(int x in new[]{-1,1})foreach(int z in new[]{-1,1})Part(root.transform,"Desk_Leg",new Vector3(x*width*.43f,leg*.5f,z*depth*.39f),new Vector3(.055f,leg,.055f),steel);
   Part(root.transform,"Rear_Stretcher",new Vector3(0,.22f,depth*.39f),new Vector3(width*.86f,.055f,.055f),steel);
   foreach(int side in new[]{-1,1})Part(root.transform,"Side_Stretcher",new Vector3(side*width*.43f,.22f,0),new Vector3(.055f,.055f,depth*.78f),steel);
   if(name.Contains("Giao_Vien"))Part(root.transform,"Teacher_Desk_Front_Panel",new Vector3(0,height*.62f,depth*.4f),new Vector3(width*.86f,.3f,.035f),wood);
  }
  return root;
 }
 public static void Refine(ChuongTruongHocV37 chapter)
 {
  var report=new JArray();
  foreach(var old in chapter.environment.GetComponentsInChildren<MeshFilter>(true).Where(m=>m.name.EndsWith("_V37")&&(m.name.StartsWith("Ban_")||m.name.StartsWith("Ghe_"))).ToArray())
  {
   var bounds=old.GetComponent<Renderer>().bounds;var point=new Vector3(bounds.center.x,bounds.min.y,bounds.center.z);
   bool teacher=old.name.Contains("Giao_Vien"),chair=old.name.StartsWith("Ghe_");float height=chair?(teacher?.9f:.82f):(teacher?.78f:.74f);
   var replacement=Create(old.name,point,height,chapter.environment.transform);
   if(old.transform==chapter.chair)chapter.chair=replacement.transform;
   UnityEngine.Object.DestroyImmediate(old.gameObject);
  }
  foreach(var text in chapter.environment.GetComponentsInChildren<TextMesh>(true))text.transform.rotation=Quaternion.identity;
  foreach(var model in chapter.environment.transform.Cast<Transform>().Where(t=>t.name.EndsWith("_V37")))report.Add(new JObject{["model"]=model.name,["parts"]=model.GetComponentsInChildren<Renderer>(true).Length});
  File.WriteAllText("Docs/Reviews/school_v37/furniture-models.json",report.ToString());
 }
}
