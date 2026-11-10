using System;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using Newtonsoft.Json.Linq;
using ConRoi.Gameplay.TestOnly;
namespace ConRoi.Gameplay.EditorChecks {
// Explicit windowed bootstrap observation; no native input, gameplay fixture or save.
[InitializeOnLoad] public static class FullCaseBootstrapChecks {
 const string K="ConRoi.Bootstrap.Check.";
 static FullCaseBootstrapChecks(){EditorApplication.update+=Tick;EditorApplication.playModeStateChanged+=State;Application.logMessageReceived+=Log;Camera.onPostRender+=Rendered;RenderPipelineManager.endCameraRendering+=(ctx,c)=>Rendered(c);}
 static string Arg(string key){var a=Environment.GetCommandLineArgs();int n=Array.IndexOf(a,key);return n>=0&&n+1<a.Length?a[n+1]:"";}
 static string Phase{get=>SessionState.GetString(K+"phase","");set=>SessionState.SetString(K+"phase",value);}
 static string Output=>SessionState.GetString(K+"output","");
 static void Append(string key,JObject row){var rows=JArray.Parse(SessionState.GetString(K+key,"[]"));rows.Add(row);SessionState.SetString(K+key,rows.ToString());}
 public static void RunWindowed(){string output=Arg("--bootstrap-output");if(Application.isBatchMode||!Path.IsPathRooted(output)||Directory.Exists(output))throw new Exception("Fresh output and windowed Editor required");
  Directory.CreateDirectory(output);SessionState.SetString(K+"output",output);SessionState.SetString(K+"checks","[]");SessionState.SetString(K+"callbacks","[]");SessionState.SetString(K+"console","[]");SessionState.SetInt(K+"fail",0);
  EditorSceneManager.OpenScene(FullCasePlayLauncher.Scene);if(EditorSceneManager.GetActiveScene().isDirty)throw new Exception("Dirty template");Phase="direct.pending";EditorApplication.EnterPlaymode();}
 static void Log(string message,string stack,LogType type){if(Phase==""||Phase=="finished")return;Append("console",new JObject{["phase"]=Phase,["type"]=type.ToString(),["message"]=message,["stack"]=stack});}
 static void Rendered(Camera c){var h=FullCasePlayLauncher.Active;if(h&&h.Eye==c)SessionState.SetInt(K+"renders",SessionState.GetInt(K+"renders",0)+1);}
 static void State(PlayModeStateChange state){if(Phase=="")return;Append("callbacks",new JObject{["phase"]=Phase,["state"]=state.ToString(),["activeHost"]=(bool)FullCasePlayLauncher.Active,["scene"]=EditorSceneManager.GetActiveScene().path});
  if(state==PlayModeStateChange.EnteredPlayMode){SessionState.SetFloat(K+"time",(float)EditorApplication.timeSinceStartup);Phase=Phase.StartsWith("direct")?"direct.observe":"menu.observe";}
  if(state==PlayModeStateChange.EnteredEditMode){if(Phase=="direct.stop")Phase="menu.start";else if(Phase=="menu.stop")Phase="cleanup";}}
 static void C(string name,bool pass){Append("checks",new JObject{["phase"]=Phase,["name"]=name,["pass"]=pass});if(!pass)SessionState.SetInt(K+"fail",SessionState.GetInt(K+"fail",0)+1);}
 static bool HelpOpen()=>Resources.FindObjectsOfTypeAll<EditorWindow>().Any(w=>w.GetType().Name=="FullCasePlayLaunchHelp");
 static JObject Snapshot(){var h=FullCasePlayLauncher.Active;var cameras=UnityEngine.Object.FindObjectsByType<Camera>(FindObjectsInactive.Include);return new JObject{["phase"]=Phase,["scene"]=EditorSceneManager.GetActiveScene().path,["hostCount"]=UnityEngine.Object.FindObjectsByType<FullCasePlayHost>(FindObjectsInactive.Include).Length,["activeHost"]=(bool)h,["playerPresent"]=h&&h.Movement,["playerActive"]=h&&h.Movement&&h.Movement.gameObject.activeInHierarchy,["runtimePresent"]=h&&h.Runtime!=null,["targetCount"]=h?h.Targets.Count:0,["profile"]=h?h.Profile:null,["helpOpen"]=HelpOpen(),["renderCallbacks"]=SessionState.GetInt(K+"renders",0),["cameras"]=new JArray(cameras.Select(c=>(object)new JObject{["name"]=c.name,["enabled"]=c.enabled,["active"]=c.gameObject.activeInHierarchy,["display"]=c.targetDisplay,["targetTexture"]=c.targetTexture?c.targetTexture.name:null,["rect"]=c.rect.ToString(),["underPlayer"]=h&&c.transform.IsChildOf(h.Movement.transform)}))};}
 static void Tick(){if(Phase==""||EditorApplication.isCompiling||EditorApplication.isUpdating)return;
 try{if(Phase=="finished"&&!EditorApplication.isPlayingOrWillChangePlaymode){Phase="";EditorApplication.Exit(SessionState.GetInt(K+"fail",1)>0?1:0);return;}
  if(Phase=="direct.observe"&&EditorApplication.timeSinceStartup-SessionState.GetFloat(K+"time",0)>2){File.WriteAllText(Path.Combine(Output,"direct.json"),Snapshot().ToString());C("direct Play does not bypass explicit bootstrap",!FullCasePlayLauncher.Active&&UnityEngine.Object.FindObjectsByType<Camera>(FindObjectsInactive.Include).Length==0);C("direct Play guidance matches expected version",HelpOpen()==(Arg("--expect-guidance")=="yes"));if(Arg("--expect-guidance")=="yes")C("Console explains missing bootstrap and correct menu",JArray.Parse(SessionState.GetString(K+"console","[]")).OfType<JObject>().Any(x=>(string)x["type"]=="Warning"&&((string)x["message"]??"").Contains("Dừng Play")&&((string)x["message"]??"").Contains("Choi the gioi TEST ONLY")));Phase="direct.stop";EditorApplication.ExitPlaymode();}
  else if(Phase=="menu.start"){SessionState.SetInt(K+"renders",0);Phase="menu.pending";C("real menu executes",EditorApplication.ExecuteMenuItem("Con Roi/Gameplay/Choi the gioi TEST ONLY (Play Mode)"));}
  else if(Phase=="menu.observe"&&EditorApplication.timeSinceStartup-SessionState.GetFloat(K+"time",0)>3){var h=FullCasePlayLauncher.Active;var snapshot=Snapshot();File.WriteAllText(Path.Combine(Output,"menu.json"),snapshot.ToString());
   C("menu bootstrap creates exactly one initialized host",h&&h.Runtime!=null&&UnityEngine.Object.FindObjectsByType<FullCasePlayHost>(FindObjectsInactive.Include).Length==1&&h.Targets.Count>0);
   C("player active with actual controller and eye",h&&h.Movement&&h.Look&&h.Movement.gameObject.activeInHierarchy&&h.Movement.controller.enabled&&h.Eye.transform.IsChildOf(h.Movement.transform));
   C("camera enabled active outputs Display 1 directly",h&&h.Eye&&h.Eye.enabled&&h.Eye.gameObject.activeInHierarchy&&h.Eye.targetDisplay==0&&!h.Eye.targetTexture&&h.Eye.rect==new Rect(0,0,1,1));
   C("camera has rendered in windowed Game View",SessionState.GetInt(K+"renders",0)>0);
   C("menu closes direct launch guidance",!HelpOpen());
   C("private clean profile not saved",h&&!Directory.Exists(h.Profile)&&h.Runtime.GetRecords().Length==0);
   Phase="menu.stop";EditorApplication.ExitPlaymode();}
  else if(Phase=="cleanup"){C("stop removes generated host and camera",!FullCasePlayLauncher.Active&&UnityEngine.Object.FindObjectsByType<FullCasePlayHost>(FindObjectsInactive.Include).Length==0&&UnityEngine.Object.FindObjectsByType<Camera>(FindObjectsInactive.Include).Length==0);C("template remains clean",!EditorSceneManager.GetActiveScene().isDirty);C("both routes complete Play lifecycle callbacks",JArray.Parse(SessionState.GetString(K+"callbacks","[]")).OfType<JObject>().Count(x=>(string)x["state"]=="EnteredPlayMode")==2&&JArray.Parse(SessionState.GetString(K+"callbacks","[]")).OfType<JObject>().Count(x=>(string)x["state"]=="EnteredEditMode")==2);
   var console=JArray.Parse(SessionState.GetString(K+"console","[]"));C("no Console error exception or assert in both play routes",!console.OfType<JObject>().Any(x=>new[]{"Error","Exception","Assert"}.Contains((string)x["type"])));
   Finish();}
 }catch(Exception e){Append("console",new JObject{["type"]="Exception",["message"]=e.ToString()});C("checker completed",false);Finish();}}
 static void Finish(){var manifest=new JObject();foreach(string path in new[]{"Assets/ConRoi/Modules/Gameplay/Editor/FullCasePlayLauncher.cs","Assets/ConRoi/Modules/Gameplay/Editor/FullCaseBootstrapChecks.cs",FullCasePlayLauncher.Scene})using(var hash=System.Security.Cryptography.SHA256.Create())manifest[path]=BitConverter.ToString(hash.ComputeHash(File.ReadAllBytes(path))).Replace("-","").ToLowerInvariant();
  File.WriteAllText(Path.Combine(Output,"results.json"),new JObject{["source"]=Arg("--bootstrap-source"),["kind"]="WINDOWED_EDITOR_PLAYMODE_LIFECYCLE_AND_EXECUTE_MENU_ITEM_NOT_WINDOWS_INPUT",["failures"]=SessionState.GetInt(K+"fail",0),["checks"]=JArray.Parse(SessionState.GetString(K+"checks","[]")),["callbacks"]=JArray.Parse(SessionState.GetString(K+"callbacks","[]")),["console"]=JArray.Parse(SessionState.GetString(K+"console","[]")),["sourceFiles"]=manifest}.ToString());Phase="finished";if(EditorApplication.isPlaying)EditorApplication.ExitPlaymode();}
}
}
