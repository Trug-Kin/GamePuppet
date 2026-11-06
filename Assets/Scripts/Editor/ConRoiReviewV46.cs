using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using Newtonsoft.Json.Linq;

public static class ConRoiReviewV46
{
 public const string Scene="Assets/Scenes/JourneyExperience_v46.unity",Report="Docs/Reviews/journey_v46";
 static Mesh Palm()
 {
  const string path="Assets/Art/Environment/HumanPalmV46.asset";
  var existing=AssetDatabase.LoadAssetAtPath<Mesh>(path);if(existing)return existing;
  var v=new List<Vector3>();var triangles=new List<int>();
  float[] y={-.5f,-.43f,.17f,.38f,.5f},w={.26f,.45f,.49f,.42f,.26f};
  for(int r=0;r<5;r++)for(int n=0;n<16;n++){float a=n*Mathf.PI*2/16;float x=Mathf.Sin(a),z=Mathf.Cos(a);v.Add(new Vector3(Mathf.Sign(x)*Mathf.Pow(Mathf.Abs(x),.55f)*w[r],y[r],Mathf.Sign(z)*Mathf.Pow(Mathf.Abs(z),.55f)*w[r]*.7f));}
  for(int r=0;r<4;r++)for(int n=0;n<16;n++){int a=r*16+n,b=r*16+(n+1)%16;triangles.AddRange(new[]{a,b,b+16,a,b+16,a+16});}
  for(int n=1;n<15;n++)triangles.AddRange(new[]{0,n+1,n,64,64+n,65+n});
  var mesh=new Mesh{name="V46_Bevelled_Palm_Knuckles"};mesh.SetVertices(v);mesh.SetTriangles(triangles,0);mesh.RecalculateNormals();mesh.RecalculateBounds();AssetDatabase.CreateAsset(mesh,path);return mesh;
 }
 public static void AssembleAndBuild()
 {
  Directory.CreateDirectory(Report);EditorSceneManager.OpenScene(ConRoiReviewV45.Scene);
  var acting=UnityEngine.Object.FindAnyObjectByType<DienXuatHanhTrinhV45>();var sorting=acting.GetComponent<DieuChinhHanhTrinhV43>();
  var surface=acting.gameObject.AddComponent<BeMatNhanVatV46>();surface.acting=acting;acting.gameObject.AddComponent<DoChiPhiTacVuV46>();
  surface.bodies=acting.rigs.Select(r=>
  {
   var all=r.actor.GetComponentsInChildren<Transform>(true);
   var skin=all.First(t=>t.name==r.role+"_Jaw_Cheek_Forehead").GetComponent<Renderer>().sharedMaterial;
   var upper=r.role=="Ha"?sorting.upper:r.upper;var lower=r.role=="Ha"?sorting.lower:r.fore;var hands=r.role=="Ha"?sorting.hands:r.palm;
   var cloth=upper[0].GetComponent<Renderer>().sharedMaterial;
   foreach(var hand in hands){hand.GetComponent<MeshFilter>().sharedMesh=Palm();hand.GetComponent<Renderer>().sharedMaterial=skin;}
   foreach(var t in upper.Concat(lower))t.GetComponent<Renderer>().enabled=false;
   foreach(var t in all)if(t.name==r.role+"_Neck"||t.name==r.role+"_Fitted_Garment"||t.name=="Tailored_Jacket") {var ren=t.GetComponent<Renderer>();if(ren)ren.enabled=false;}
   Material trousers=cloth;
   if(r.role=="Vy")foreach(var t in r.thighs.Concat(r.shins)){var ren=t.GetComponent<Renderer>();trousers=ren.sharedMaterial;ren.enabled=false;}
   return new BeMatNhanVatV46.Body{role=r.role,frame=r.frame,head=all.First(t=>t.name=="V44_"+r.role+"_SculptedHead"),torso=r.torso,upper=upper,lower=lower,hands=hands,cloth=cloth,skin=skin,trousers=trousers,thighs=r.role=="Vy"?r.thighs:null,shins=r.role=="Vy"?r.shins:null,shoulder=r.role=="Ha"?r.frame.InverseTransformPoint(sorting.shoulders[0]).y+.055f:r.shoulderY,width=r.role=="Tung"?.255f:r.role=="Ha"?.242f:r.role=="Vy"?.193f:r.role=="Bao"?.235f:.212f,hem=r.role=="Lan"?.31f:r.role=="Vy"?.86f:.77f};
  }).ToArray();
  // A shaded warm stall practical and a restrained cool bounce, both local lights.
  var ha=acting.rigs.First(r=>r.role=="Ha");
  Light Make(string name,Vector3 local,Color color,float intensity,float range)
  {var go=new GameObject(name);go.transform.position=ha.frame.TransformPoint(local);var l=go.AddComponent<Light>();l.type=LightType.Point;l.color=color;l.intensity=intensity;l.range=range;l.shadows=name.Contains("Bounce")?LightShadows.None:LightShadows.Soft;l.shadowResolution=UnityEngine.Rendering.LightShadowResolution.Low;return l;}
  Make("V46_Ha_Stall_Warm_Practical",new Vector3(-.45f,2.1f,-.65f),new Color(1,.76f,.49f),2.1f,3.1f);
  Make("V46_Ha_Stall_Soft_Bounce",new Vector3(.6f,1.35f,-.8f),new Color(.63f,.74f,1),.55f,2.1f);
  File.WriteAllText(Report+"/authoring.json",new JArray(surface.bodies.Select(b=>new JObject{["role"]=b.role,["shoulder"]=b.shoulder,["head_height"]=b.frame.InverseTransformPoint(b.head.position).y,["width"]=b.width,["visible_original_parts"]=new JArray(b.frame.parent.GetComponentsInChildren<Renderer>(true).Where(x=>x.enabled).Select(x=>x.name))})).ToString());
  EditorSceneManager.SaveScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene(),Scene);AssetDatabase.SaveAssets();Build();
 }
 public static void Build()
 {
  EditorSceneManager.OpenScene(Scene);EditorBuildSettings.scenes=new[]{new EditorBuildSettingsScene(Scene,true)};PlayerSettings.bundleVersion="0.46.0";
  var r=BuildPipeline.BuildPlayer(new[]{Scene},"Builds/Windows_v46_r2/ConRoi.exe",BuildTarget.StandaloneWindows64,BuildOptions.Development);
  File.WriteAllText(Report+"/build.json",new JObject{["result"]=r.summary.result.ToString(),["errors"]=r.summary.totalErrors,["warnings"]=r.summary.totalWarnings,["path"]=r.summary.outputPath,["messages"]=new JArray(r.steps.SelectMany(s=>s.messages).Where(m=>m.type==LogType.Error||m.type==LogType.Warning).Select(m=>m.content))}.ToString());
  if(r.summary.totalErrors>0)throw new Exception("Build failed");
 }
}
