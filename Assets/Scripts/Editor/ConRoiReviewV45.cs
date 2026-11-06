using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.Rendering;
using Newtonsoft.Json.Linq;

public static class ConRoiReviewV45
{
 static bool refining;
 public const string Scene="Assets/Scenes/JourneyExperience_v45.unity",Report="Docs/Reviews/journey_v45";
 static Transform Part(Transform parent,string name,Material mat,PrimitiveType type,Vector3 size)
 {
  var g=GameObject.CreatePrimitive(type);g.name=name;g.layer=3;g.transform.SetParent(parent,false);g.transform.localScale=size;g.GetComponent<Renderer>().sharedMaterial=mat;UnityEngine.Object.DestroyImmediate(g.GetComponent<Collider>());return g.transform;
 }
 static DienXuatHanhTrinhV45.Rig Rig(Transform actor,string role,string speaker,bool arms,List<Transform> driven)
 {
  var all=actor.GetComponentsInChildren<Transform>(true);var frame=all.FirstOrDefault(t=>t.name=="V44_"+role+"_Body");
  var sculpt=all.First(t=>t.name=="V44_"+role+"_SculptedHead");
  if(!frame){var r=new GameObject("V45_"+role+"_Frame").transform;r.SetParent(actor,true);r.SetPositionAndRotation(new Vector3(sculpt.position.x,actor.GetComponentsInChildren<Renderer>(true).Where(x=>x.enabled).Min(x=>x.bounds.min.y),sculpt.position.z),sculpt.rotation);frame=r;}
  var projection=actor.GetComponent<HienGuongMatTheoKhu>();var head=projection?projection.ghostHead:sculpt;
  var rig=new DienXuatHanhTrinhV45.Rig{actor=actor,frame=frame,role=role,speaker=speaker,head=head,torso=all.FirstOrDefault(t=>t.name==role+"_Fitted_Garment"||role=="Bao"&&t.name=="Tailored_Jacket"),customArms=arms,shoulderY=role=="Tung"?1.43f:role=="Lan"?1.24f:1.36f,actorRest=actor.rotation,frameInActor=Quaternion.Inverse(actor.rotation)*frame.rotation};
  rig.eyes=all.Where(t=>t.name==role+"_Eye-1"||t.name==role+"_Eye1").ToArray();rig.eyeRest=rig.eyes.Select(t=>t.localPosition).ToArray();rig.brows=all.Where(t=>t.name==role+"_Brow-1"||t.name==role+"_Brow1").ToArray();rig.browRest=rig.brows.Select(t=>t.localPosition).ToArray();
  driven.Add(actor);driven.Add(head);driven.AddRange(rig.eyes);driven.AddRange(rig.brows);if(rig.torso)driven.Add(rig.torso);
  if(arms)
  {
   var cloth=rig.torso.GetComponent<Renderer>().sharedMaterial;var skin=sculpt.GetComponentsInChildren<Renderer>().First(t=>t.name==role+"_Jaw_Cheek_Forehead").sharedMaterial;
   foreach(var t in all)if(t.name.StartsWith(role+"_UpperSleeve")||t.name.StartsWith(role+"_ForeSleeve")||t.name.StartsWith(role+"_Palm")){var r=t.GetComponent<Renderer>();if(r)r.enabled=false;foreach(var rr in t.GetComponentsInChildren<Renderer>())rr.enabled=false;}
   if(role=="Bao")foreach(var t in all)if(t.name.StartsWith("Tailored_Sleeve")||t.name.StartsWith("Hand_")){var r=t.GetComponent<Renderer>();if(r)r.enabled=false;}
   rig.upper=new Transform[2];rig.fore=new Transform[2];rig.palm=new Transform[2];rig.shoulder=new Vector3[2];
   for(int i=0;i<2;i++)
   {
    int side=i==0?-1:1;float half=role=="Tung"?.255f:.214f;rig.shoulder[i]=new Vector3(side*(half+.015f),rig.shoulderY-.055f,.008f);
    rig.upper[i]=Part(frame,"V45_"+role+"_Upper_"+i,cloth,PrimitiveType.Capsule,new Vector3(.132f,.15f,.13f));rig.fore[i]=Part(frame,"V45_"+role+"_Fore_"+i,cloth,PrimitiveType.Capsule,new Vector3(.105f,.14f,.105f));rig.palm[i]=Part(frame,"V45_"+role+"_Palm_"+i,skin,PrimitiveType.Sphere,new Vector3(.075f,.115f,.07f));
    driven.Add(rig.upper[i]);driven.Add(rig.fore[i]);driven.Add(rig.palm[i]);
   }
  }
  if(role=="Vy")
  {
   rig.thighs=new Transform[2];rig.shins=new Transform[2];rig.shoes=new Transform[2];
   for(int i=0;i<2;i++)
   {
    string side=i==0?"-1":"1";var old=all.First(t=>t.name=="Vy_Tapered_Trouser"+side);var cloth=old.GetComponent<Renderer>().sharedMaterial;old.GetComponent<Renderer>().enabled=false;
    rig.thighs[i]=Part(frame,"V45_Vy_Thigh_"+i,cloth,PrimitiveType.Capsule,new Vector3(.13f,.18f,.14f));rig.shins[i]=Part(frame,"V45_Vy_Shin_"+i,cloth,PrimitiveType.Capsule,new Vector3(.105f,.18f,.115f));rig.shoes[i]=all.First(t=>t.name=="Vy_Leather_Shoe"+side);driven.Add(rig.thighs[i]);driven.Add(rig.shins[i]);driven.Add(rig.shoes[i]);
   }
  }
  return rig;
 }
 public static void AssembleAndBuild()
 {
  Directory.CreateDirectory(Report);
  if(File.Exists(Scene)&&!refining)throw new Exception("v45 scene already authored; use Build");
  EditorSceneManager.OpenScene(ConRoiReviewV44.Scene);var s=UnityEngine.Object.FindAnyObjectByType<DieuPhoiHanhTrinh>();
  var c=s.gameObject.AddComponent<DienXuatHanhTrinhV45>();c.story=s;s.completion.externalPerformance=true;s.GetComponent<TraiNghiemHanhTrinhV42>().externalSorting=true;s.GetComponent<DieuChinhHanhTrinhV43>().externalGaze=true;
  foreach(var old in UnityEngine.Object.FindObjectsByType<DienXuatNhanVatV40>(FindObjectsInactive.Include,FindObjectsSortMode.None))old.enabled=false;
  foreach(var old in UnityEngine.Object.FindObjectsByType<BieuCamNhoNhanVatV41>(FindObjectsInactive.Include,FindObjectsSortMode.None))old.enabled=false;
  var actors=new[]{s.ghosts[0],s.ghosts[1],s.schoolChapter.ghost,s.slopeChapter.ghost,s.completion.lan,s.completion.vy};
  var roles=new[]{"Bao","Ha","Mai","Tung","Lan","Vy"};var speakers=new[]{"\u00d4ng B\u1ea3o:","Ch\u1ecb H\u00e0:","C\u00f4 Mai:","Anh T\u00f9ng:","B\u00e0 Lan:","Vy:"};var driven=new List<Transform>();c.rigs=new DienXuatHanhTrinhV45.Rig[6];
  for(int i=0;i<actors.Length;i++){foreach(var a in actors[i].GetComponentsInChildren<Animation>(true))a.enabled=false;c.rigs[i]=Rig(actors[i],roles[i],speakers[i],i!=1,driven);}
  c.vyOrigin=s.completion.vy.position;c.vyRootGroundOffset=c.vyOrigin.y-c.rigs[5].frame.position.y;c.vyWalkingRotation=Quaternion.LookRotation(Vector3.left)*Quaternion.Inverse(c.rigs[5].frameInActor);c.footClip=s.presentation.step;
  var oldCan=s.completion.lan.GetComponentsInChildren<Transform>(true).First(t=>t.name=="Watering_Can");var mat=oldCan.GetComponent<Renderer>().sharedMaterial;oldCan.GetComponent<Renderer>().enabled=false;
  s.completion.lan.GetComponentsInChildren<Transform>(true).First(t=>t.name=="Watering_Spout").GetComponent<Renderer>().enabled=false;
  c.can=new GameObject("V45_Lan_Handheld_Can").transform;c.can.SetParent(s.completion.lan,false);Part(c.can,"V45_Can_Vessel",mat,PrimitiveType.Cylinder,new Vector3(.24f,.08f,.17f));
  for(int n=0;n<16;n++){float a=n*Mathf.PI*2/16,b=(n+1)*Mathf.PI*2/16;var p=new Vector3(Mathf.Cos(a)*.08f,.035f+Mathf.Sin(a)*.075f,0);var q=new Vector3(Mathf.Cos(b)*.08f,.035f+Mathf.Sin(b)*.075f,0);var t=Part(c.can,"Can_Grip_"+n,mat,PrimitiveType.Capsule,new Vector3(.013f,.04f,.013f));LocalSegment(t,p,q);}
  c.spout=Part(c.can,"V45_Can_Spout",mat,PrimitiveType.Capsule,new Vector3(.022f,.08f,.022f));LocalSegment(c.spout,new Vector3(.10f,.015f,0),new Vector3(.28f,-.02f,0));driven.Add(c.can);
  c.rod=s.lake.GetComponentsInChildren<Transform>(true).First(t=>t.name=="Bamboo_Fishing_Rod");
  var v43=s.GetComponent<DieuChinhHanhTrinhV43>();driven.AddRange(v43.upper);driven.AddRange(v43.lower);driven.AddRange(v43.hands);driven.Add(v43.parcel);driven.Add(s.completion.baoArm);c.driven=driven.Distinct().ToArray();
  s.gameObject.AddComponent<GhiNhanKhungHinhTreV45>().story=s;
  EditorSceneManager.SaveScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene(),Scene);AssetDatabase.SaveAssets();
  File.WriteAllText(Report+"/authoring.json",new JObject{["base"]="v44 r4",["driven_transforms"]=new JArray(c.driven.Select(t=>t.name)),["roles"]=new JArray(c.rigs.Select(r=>new JObject{["role"]=r.role,["head"]=r.head.name,["frame"]=r.frame.name,["position"]=r.frame.position.ToString("F4"),["frame_rotation"]=r.frame.eulerAngles.ToString("F4")})),["puppet_registered"]=false,["lip_sync"]=false,["voice_over"]=false}.ToString());Build();
 }
 public static void Build()
 {
  EditorSceneManager.OpenScene(Scene);EditorBuildSettings.scenes=new[]{new EditorBuildSettingsScene(Scene,true)};PlayerSettings.bundleVersion="0.45.0";PlayerSettings.runInBackground=true;PlayerSettings.enableFrameTimingStats=true;
  PlayerSettings.SetUseDefaultGraphicsAPIs(BuildTarget.StandaloneWindows64,false);PlayerSettings.SetGraphicsAPIs(BuildTarget.StandaloneWindows64,new[]{GraphicsDeviceType.Direct3D11});
  var r=BuildPipeline.BuildPlayer(new[]{Scene},"Builds/Windows_v45_r4/ConRoi.exe",BuildTarget.StandaloneWindows64,BuildOptions.Development);
  File.WriteAllText(Report+"/build.json",new JObject{["result"]=r.summary.result.ToString(),["errors"]=r.summary.totalErrors,["warnings"]=r.summary.totalWarnings,["path"]=r.summary.outputPath,["messages"]=new JArray(r.steps.SelectMany(s=>s.messages).Where(m=>m.type==LogType.Error||m.type==LogType.Warning).Select(m=>m.content))}.ToString());
 }
 static void LocalSegment(Transform t,Vector3 a,Vector3 b){t.localPosition=(a+b)*.5f;t.localRotation=Quaternion.FromToRotation(Vector3.up,b-a);var s=t.localScale;t.localScale=new Vector3(s.x,(b-a).magnitude*.5f,s.z);}
 public static void RefineAndBuild(){File.Copy(Scene,Report+"/scene-r1.unity.snapshot",true);File.Copy(Report+"/build.json",Report+"/build-r1.json",true);refining=true;AssembleAndBuild();}
 public static void FinalBuild(){File.Copy(Report+"/build.json",Report+"/build-r3.json",false);Build();}
}
