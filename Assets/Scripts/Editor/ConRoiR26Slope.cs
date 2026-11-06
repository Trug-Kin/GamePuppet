using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;

public static class ConRoiR26Slope
{
 public const string Out="Docs/Reviews/journey_v48/r26",Scene="Assets/Scenes/JourneyRepair_v48_r26.unity",Art="Assets/Art/Environment/JourneyV48/SlopeR26";
 static JArray V(Vector3 v)=>new JArray(v.x,v.y,v.z);
 public static void Author(){
  if(File.Exists(Scene))throw new Exception("Keep r26 assembly; use Refine");
  if(!AssetDatabase.CopyAsset(ConRoiR25Slope.Scene,Scene))throw new Exception("Scene copy failed");
  EditorSceneManager.OpenScene(Scene);var s=UnityEngine.Object.FindAnyObjectByType<DieuPhoiHanhTrinh>();var old=s.GetComponent<HoSoSceneR25>();var pipe=old.pipeline;UnityEngine.Object.DestroyImmediate(old);var profile=s.gameObject.AddComponent<HoSoSceneR26>();profile.story=s;profile.pipeline=pipe;
  var exp=s.GetComponent<TraiNghiemDocR25>();exp.story=s;EditorSceneManager.SaveScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene());Refine();
 }
 public static void Refine(){
  EditorSceneManager.OpenScene(Scene);var s=UnityEngine.Object.FindAnyObjectByType<DieuPhoiHanhTrinh>();var root=s.slopeChapter.environment.transform;
  var legacy=s.GetComponent<HoSoSceneR25>();var profile=s.GetComponent<HoSoSceneR26>();
  if(!profile){profile=s.gameObject.AddComponent<HoSoSceneR26>();profile.story=s;profile.pipeline=legacy.pipeline;}
  if(legacy)UnityEngine.Object.DestroyImmediate(legacy);
  var experience=s.GetComponent<TraiNghiemDocR25>()??s.gameObject.AddComponent<TraiNghiemDocR25>();experience.story=s;
  var old=root.Find("R25_ContinuousSlope");if(old)old.gameObject.SetActive(false);
  var owned=root.Find("R26_ContinuousSlope");if(owned)UnityEngine.Object.DestroyImmediate(owned.gameObject);
  var parent=new GameObject("R26_ContinuousSlope");parent.transform.SetParent(root);parent.transform.position=new Vector3(-180,0,-60);
  var rows=JArray.Parse(File.ReadAllText(Art+"/Geometry.json"));var data=new JArray();
  foreach(var group in rows.GroupBy(g=>(string)g["material"]+"_"+(bool)g["collision"])){
   var first=group.First();var vs=new System.Collections.Generic.List<Vector3>();var ts=new System.Collections.Generic.List<int>();var uvs=new System.Collections.Generic.List<Vector2>();
   foreach(var row in group){int start=vs.Count;var points=row["vertices"].Select(p=>new Vector3((float)p[0],(float)p[1],(float)p[2])).ToArray();
    // Split triangle corners so each face gets an undistorted dominant-axis projection.
    var indices=row["triangles"].Select(t=>(int)t).ToArray();
    for(int i=0;i<indices.Length;i+=3){var a=points[indices[i]];var b=points[indices[i+1]];var c=points[indices[i+2]];var n=Vector3.Cross(b-a,c-a).normalized;int axis=Mathf.Abs(n.y)>.65f?1:Mathf.Abs(n.x)>Mathf.Abs(n.z)?0:2;
     foreach(var p in new[]{a,b,c}){ts.Add(vs.Count);vs.Add(p);uvs.Add(axis==1?new Vector2(p.x,p.z)*.5f:axis==0?new Vector2(p.z,p.y)*.5f:new Vector2(p.x,p.y)*.5f);}
    }
   }
   string name=group.Key;var mesh=new Mesh{name=name,indexFormat=UnityEngine.Rendering.IndexFormat.UInt32};mesh.SetVertices(vs);mesh.SetTriangles(ts,0);mesh.SetUVs(0,uvs);mesh.RecalculateNormals();mesh.RecalculateBounds();mesh.RecalculateTangents();string path=Art+"/"+name+".asset";var existing=AssetDatabase.LoadAssetAtPath<Mesh>(path);if(existing){EditorUtility.CopySerialized(mesh,existing);mesh=existing;}else AssetDatabase.CreateAsset(mesh,path);
   string materialPath=Art+"/"+(string)first["material"]+".mat";var m=AssetDatabase.LoadAssetAtPath<Material>(materialPath);
   var lit=Shader.Find("Universal Render Pipeline/Lit");if(!lit)throw new Exception("URP Lit shader missing");
   if(!m)m=new Material(lit);m.shader=lit;
   data.Add(new JObject{["object"]=parent.name+"/"+name,["triangles"]=ts.Count/3,["collider"]=(bool)first["collision"]});
   var col=first["color"];m.color=new Color((float)col[0],(float)col[1],(float)col[2]);m.SetFloat("_Smoothness",name.Contains("Glass")?.35f:.15f);
   string tile=((string)first["material"]).Replace("R25_","").Replace("R26_","");var texture=AssetDatabase.LoadAssetAtPath<Texture2D>(Art+"/Textures/"+tile+".png");if(texture)m.SetTexture("_BaseMap",texture);
   if(!AssetDatabase.Contains(m))AssetDatabase.CreateAsset(m,materialPath);
   var go=new GameObject(name);go.layer=3;go.transform.SetParent(parent.transform,false);go.AddComponent<MeshFilter>().sharedMesh=mesh;go.AddComponent<MeshRenderer>().sharedMaterial=m;if((bool)first["collision"])go.AddComponent<MeshCollider>().sharedMesh=mesh;
  }
  foreach(Transform t in root){if(t.name=="Final_Entrance_Pier"||t.name=="Final_Entrance_Lintel"||t.name=="Top_Garden_Wall"||t.name=="Tung_Door_Frame"||t.name=="Tung_Door_Panel"||t.name=="Slope_Lettering")t.gameObject.SetActive(false);}
  var door=root.Find("Last_Door");foreach(Transform child in door)child.gameObject.SetActive(false);
  // The old door remains the single collision/interact proxy. New planks are its visible skin.
  door.GetComponent<Renderer>().enabled=false;
  foreach(var memory in root.GetComponentsInChildren<TuongTacDocV39>(true).Where(i=>i.memoryIndex>=0)){
   float z=memory.memoryIndex==0?9:27;float x=Mathf.Clamp01((z-18)/12)*2-2.37f;
   memory.transform.position=new Vector3(-180+x,z*.2f+1.05f,-60+z-.005f);memory.transform.localScale=new Vector3(.52f,.36f,.03f);
   memory.GetComponent<Renderer>().sharedMaterial=AssetDatabase.LoadAssetAtPath<Material>(Art+"/R25_Trim.mat");
  }
  foreach(var t in UnityEngine.Object.FindObjectsByType<Transform>(FindObjectsInactive.Include))
   if((t.name.StartsWith("Sign_")||t.name=="Sign_Post")&&t.position.x< -150&&t.position.z< -5)t.gameObject.SetActive(false);
  AddNumber(parent.transform);
  Physics.SyncTransforms();EditorSceneManager.SaveScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene());AssetDatabase.SaveAssets();File.WriteAllText(Out+"/integration.json",new JObject{["scene"]=Scene,["door"]=V(door.position),["spawn"]=V(s.lakeSpawn.position),["batches"]=data,["story"]="Retained GD4, Tung, memories and single final interaction; optional suitcase observation guard only"}.ToString());
  Views();
 }
 static void AddNumber(Transform parent){var g=new GameObject("FictionalHouseNumber_12");g.transform.SetParent(parent);g.transform.position=new Vector3(-176.7f,7.65f,-14.065f);var text=g.AddComponent<TextMesh>();text.text="12";text.fontSize=48;text.characterSize=.035f;text.anchor=TextAnchor.MiddleCenter;text.color=new Color(.18f,.19f,.17f);var light=new GameObject("PorchLamp");light.transform.SetParent(parent);light.transform.position=new Vector3(-178,8.65f,-14.35f);var l=light.AddComponent<Light>();l.type=LightType.Point;l.color=new Color(1,.82f,.58f);l.intensity=.6f;l.range=5;}
 public static void Views(){
  foreach(var version in new[]{("before",ConRoiR25Slope.Scene),("after",Scene)}){
   EditorSceneManager.OpenScene(version.Item2);var s=UnityEngine.Object.FindAnyObjectByType<DieuPhoiHanhTrinh>();s.hub.SetActive(false);s.lake.SetActive(false);s.market.SetActive(false);s.schoolChapter.environment.SetActive(false);s.slopeChapter.environment.SetActive(true);
   var profile=s.GetComponent<HoSoSceneR26>();var pipe=profile?profile.pipeline:s.GetComponent<HoSoSceneR25>().pipeline;GraphicsSettings.defaultRenderPipeline=pipe;QualitySettings.renderPipeline=pipe;
   RenderSettings.fog=false;Shader.SetGlobalVector("_MistProfile",Vector4.zero);RenderSettings.ambientMode=AmbientMode.Flat;RenderSettings.ambientLight=new Color(.65f,.68f,.67f);
   foreach(var l in UnityEngine.Object.FindObjectsByType<Light>(FindObjectsInactive.Include))if(l.type==LightType.Directional){l.intensity=1.1f;l.transform.rotation=Quaternion.Euler(42,-35,0);l.color=new Color(1,.96f,.88f);}
   var cam=new GameObject("R25ReviewCamera").AddComponent<Camera>();cam.fieldOfView=60;cam.farClipPlane=200;cam.nearClipPlane=.1f;cam.clearFlags=CameraClearFlags.SolidColor;cam.backgroundColor=new Color(.48f,.58f,.62f);
   var poses=new[]{("entry",new Vector3(-180,1.69f,-59.5f),new Vector3(-178.5f,5,-31)),("houses",new Vector3(-179.2f,4.5f,-45),new Vector3(-178,7,-34)),("white-side",new Vector3(-189,6.6f,-43),new Vector3(-184.2f,6.7f,-37)),("teal-side",new Vector3(-167,7,-34),new Vector3(-173.5f,7.2f,-32)),("door-approach",new Vector3(-177.55f,7.65f,-20),new Vector3(-178,7.3f,-13.5f)),("door-side",new Vector3(-187,8,-20),new Vector3(-178,7.4f,-12)),("lane-back",new Vector3(-176.5f,7.65f,-26),new Vector3(-180,3,-50))};
   poses=poses.Select(p=>p.Item1=="teal-side"?(p.Item1,new Vector3(-169.3f,7,-38),new Vector3(-174.3f,7.2f,-32)):p).ToArray();
   poses=poses.Concat(new[]{("door-right",new Vector3(-167,7.2f,-11),new Vector3(-173,6,-10)),("court-ground",new Vector3(-179,8,-27),new Vector3(-178,5.9f,-16)),("infill-side",new Vector3(-168,4.5f,-51),new Vector3(-172.8f,3.5f,-45))}).ToArray();
   poses=poses.Concat(new[]{("sample-left",new Vector3(-180.9f,3.65f,-50),new Vector3(-183.0f,4.1f,-45)),("teal-front",new Vector3(-177.8f,6.2f,-40),new Vector3(-173.95f,7.1f,-35.1f)),("source-S02",new Vector3(-181.1f,2.85f,-54),new Vector3(-180.9f,6.2f,-35))}).ToArray();
   var cameras=new JArray();foreach(var p in poses){cam.fieldOfView=p.Item1=="source-S02"?65:60;cam.transform.position=p.Item2;cam.transform.LookAt(p.Item3);if(p.Item1=="source-S02")CapturePortrait(cam,Out+"/"+version.Item1+"/"+p.Item1+".png");else ConRoiR22Scenery.Capture(cam,Out+"/"+version.Item1+"/"+p.Item1+".png");cameras.Add(new JObject{["name"]=p.Item1,["eye"]=V(p.Item2),["target"]=V(p.Item3),["fov"]=cam.fieldOfView});}File.WriteAllText(Out+"/"+version.Item1+"/cameras.json",cameras.ToString());UnityEngine.Object.DestroyImmediate(cam.gameObject);
  }
 }
 static void CapturePortrait(Camera cam,string path){Directory.CreateDirectory(Path.GetDirectoryName(path));var rt=new RenderTexture(700,1400,24);var old=RenderTexture.active;cam.targetTexture=rt;cam.aspect=.5f;cam.Render();RenderTexture.active=rt;var tex=new Texture2D(700,1400,TextureFormat.RGB24,false);tex.ReadPixels(new Rect(0,0,700,1400),0,0);tex.Apply();File.WriteAllBytes(path,tex.EncodeToPNG());cam.targetTexture=null;RenderTexture.active=old;UnityEngine.Object.DestroyImmediate(tex);UnityEngine.Object.DestroyImmediate(rt);cam.ResetAspect();}

}
