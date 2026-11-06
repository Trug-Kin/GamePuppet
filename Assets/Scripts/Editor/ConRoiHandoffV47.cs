using System;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using Newtonsoft.Json.Linq;

public static class ConRoiHandoffV47
{
 const string Scene="Assets/Scenes/JourneyAcceptance_v47_rc3.unity",Report="Docs/Reviews/journey_v47_rc3";
 public static void Build()
 {
  Directory.CreateDirectory(Report);EditorSceneManager.OpenScene(ConRoiReviewV47.Scene);
  var s=UnityEngine.Object.FindAnyObjectByType<DieuPhoiHanhTrinh>();
  s.gameObject.AddComponent<MenuVaoGameV47>().story=s;s.gameObject.AddComponent<GhiBangChungHienThiV47>().story=s;
  var rows=new JArray();
  void Light(Transform parent,string name,Vector3 p,float intensity,float range,Color color)
  {
   var g=new GameObject("RC3_Local_"+name);g.transform.SetParent(parent,true);g.transform.position=p;
   var l=g.AddComponent<Light>();l.type=LightType.Point;l.color=color;l.intensity=intensity;l.range=range;l.shadows=LightShadows.None;
   rows.Add(new JObject{["name"]=name,["position"]=p.ToString("F3"),["intensity"]=intensity,["range"]=range});
  }
  var acting=s.GetComponent<DienXuatHanhTrinhV45>();
  foreach(var r in acting.rigs)
  {
   // Short-range frontal bounce, positioned above ordinary player approach.
   var p=r.frame.position;var forward=r.frame.forward;
   Light(r.frame.parent,"Face_"+r.role,p+Vector3.up*1.9f-forward*1.25f,r.role=="Ha"?2.5f:1.65f,3.8f,r.role=="Ha"?new Color(1,.80f,.60f):new Color(.80f,.87f,1));
  }
  var route=(JArray)JObject.Parse(File.ReadAllText("Assets/Resources/VerificationV36/route.json"))["route"];
  // Existing authored market street route; pools remain separated by darker edges.
  for(int i=165;i<route.Count;i+=5){var q=route[i];var p=new Vector3((float)q[0],(float)q[1]+2.2f,(float)q[2]);Light(s.market.transform,"Market_Path_"+i,p,2.2f,7,new Color(1,.77f,.48f));}
  foreach(var t in new[]{s.traces[1],s.schoolChapter.chair})Light(t.parent,"Interact_"+t.name,t.position+new Vector3(0,1.65f,-.8f),1.7f,3.5f,new Color(.83f,.88f,1));
  Light(s.schoolChapter.environment.transform,"School_Entry",s.schoolChapter.spawn.position+new Vector3(0,2.1f,2),2.8f,8,new Color(.70f,.80f,1));
  File.WriteAllText(Report+"/local-lighting.json",new JObject{["global_lighting_changed"]=false,["fog_changed"]=false,["skin_emission_changed"]=false,["lights"]=rows}.ToString());
  EditorSceneManager.SaveScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene(),Scene);AssetDatabase.SaveAssets();
  EditorBuildSettings.scenes=new[]{new EditorBuildSettingsScene(Scene,true)};PlayerSettings.bundleVersion="0.47.3";
  var build=BuildPipeline.BuildPlayer(new[]{Scene},"Builds/Windows_v47_rc3/ConRoi.exe",BuildTarget.StandaloneWindows64,BuildOptions.Development);
  File.WriteAllText(Report+"/build.json",new JObject{["result"]=build.summary.result.ToString(),["errors"]=build.summary.totalErrors,["warnings"]=build.summary.totalWarnings}.ToString());
  if(build.summary.totalErrors>0)throw new Exception("Build failed");
 }
}
