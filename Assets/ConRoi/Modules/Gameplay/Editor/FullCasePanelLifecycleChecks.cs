using System;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using Newtonsoft.Json.Linq;
namespace ConRoi.Gameplay.EditorChecks {
// TEST ONLY. Runs only on explicit invocation, in a separate Unity project.
[InitializeOnLoad] public static class FullCasePanelLifecycleChecks {
 const string K="ConRoi.PanelLifecycle.Check";
 static string Output=>SessionState.GetString(K+".output","");
 static string Phase=>SessionState.GetString(K,"");
 static double began;
 static FullCaseSandboxWindow window;
 static FullCaseSandboxPanel oldPanel;
 static string Arg(string key){var a=Environment.GetCommandLineArgs();int i=Array.IndexOf(a,key);return i>=0&&i+1<a.Length?a[i+1]:"";}
 static FullCasePanelLifecycleChecks(){Application.logMessageReceived+=Log;EditorApplication.update+=Tick;began=EditorApplication.timeSinceStartup;}
 static void Add(string name,bool pass){var path=Path.Combine(Output,"checks.json");var rows=File.Exists(path)?JArray.Parse(File.ReadAllText(path)):new JArray();rows.Add(new JObject{["name"]=name,["pass"]=pass});File.WriteAllText(path,rows.ToString());}
 static void Log(string message,string stack,LogType type){if(Output==""||Phase==""||Phase=="done")return;
  if(message.Contains("Invalid editor window")||(type==LogType.Exception&&stack.Contains("ConRoi.Gameplay")))File.AppendAllText(Path.Combine(Output,"observed-errors.txt"),message+"\n"+stack+"\n");}
 static void Consistency(){typeof(EditorWindow).Assembly.GetType("UnityEditor.WindowLayout").GetMethod("CheckWindowConsistency",BindingFlags.Static|BindingFlags.Public|BindingFlags.NonPublic).Invoke(null,null);}
 public static void Run(){string output=Arg("--lifecycle-output");if(!Application.isBatchMode||!Path.IsPathRooted(output)||Directory.Exists(output))throw new Exception("Private batch project/fresh absolute output required");Directory.CreateDirectory(output);SessionState.SetString(K+".output",output);
  AssetDatabase.DesiredWorkerCount=0;SessionState.SetString(K,"edit");began=EditorApplication.timeSinceStartup;
  window=EditorWindow.GetWindow<FullCaseSandboxWindow>();window.Show();
  Add("standalone shown window has parent",typeof(EditorWindow).GetField("m_Parent",BindingFlags.Instance|BindingFlags.NonPublic).GetValue(window)!=null&&window.ControllerForChecks!=null);
  oldPanel=(FullCaseSandboxPanel)typeof(FullCaseSandboxWindow).GetField("panel",BindingFlags.Instance|BindingFlags.NonPublic).GetValue(window);window.Close();
  Add("closing standalone disposes panel",oldPanel.Disposed&&Resources.FindObjectsOfTypeAll<FullCaseSandboxWindow>().Length==0);
  window=EditorWindow.GetWindow<FullCaseSandboxWindow>();window.Show();Consistency();window.maximized=true;window.maximized=false;Consistency();
  SessionState.SetString(K,"compile");
  // A compile trigger in this private project only; never installed in the source worktree.
  File.WriteAllText("Assets/ConRoi/Modules/Gameplay/Editor/ConversationCompileSentinel.cs","// TEST ONLY lifecycle compile trigger\nnamespace ConRoi.Gameplay.EditorChecks { internal static class ConversationCompileSentinel {} }\n// "+Guid.NewGuid().ToString("N"));
  AssetDatabase.ImportAsset("Assets/ConRoi/Modules/Gameplay/Editor/ConversationCompileSentinel.cs",ImportAssetOptions.ForceUpdate);
 }
 static void Tick(){if(Phase==""||Phase=="done"||EditorApplication.isCompiling||EditorApplication.isUpdating)return;
  try{
   if(EditorApplication.timeSinceStartup-began>180)throw new Exception("Lifecycle timeout: "+Phase);
   if(Phase=="compile"){
    // After actual script compilation, the runner's static fields are reset.
    if(window!=null)return;
    window=Resources.FindObjectsOfTypeAll<FullCaseSandboxWindow>().Single();
    Add("compile restores shown window without silent new runtime",window.ControllerForChecks==null);
    Consistency();window.Close();
    EditorSceneManager.OpenScene(FullCasePlayLauncher.Scene);SessionState.SetString(K,"play");FullCasePlayLauncher.Open();return;
   }
   if(Phase=="play"&&FullCasePlayLauncher.Active){
    Add("Play uses plain panel no hidden EditorWindow",!typeof(UnityEngine.Object).IsAssignableFrom(FullCasePlayLauncher.Panel.GetType())&&Resources.FindObjectsOfTypeAll<FullCaseSandboxWindow>().Length==0);
    Consistency();SessionState.SetString(K,"reload-play");EditorUtility.RequestScriptReload();return;
   }
   if(Phase=="reload-play"&&!EditorApplication.isPlayingOrWillChangePlaymode){
    Add("Play reload ends interrupted session without bootstrap loop",FullCasePlayLauncher.Active==null&&FullCasePlayLauncher.Panel==null&&Resources.FindObjectsOfTypeAll<ConRoi.Gameplay.TestOnly.FullCasePlayHost>().Length==0);
    SessionState.SetString(K,"normal-play");FullCasePlayLauncher.Open();return;
   }
   if(Phase=="normal-play"&&FullCasePlayLauncher.Active){
    Add("explicit new Play starts one clean runtime",FullCasePlayLauncher.Active.Runtime.GetRecords().Length==0&&FullCasePlayLauncher.Active.Runtime.GetHeardStatements().Length==0&&Resources.FindObjectsOfTypeAll<ConRoi.Gameplay.TestOnly.FullCasePlayHost>().Length==1);
    Consistency();SessionState.SetString(K,"normal-stop");EditorApplication.ExitPlaymode();return;
   }
   if(Phase=="normal-stop"&&!EditorApplication.isPlayingOrWillChangePlaymode){
    Add("normal Play exit cleans host and panel",FullCasePlayLauncher.Panel==null&&FullCasePlayLauncher.Active==null&&Resources.FindObjectsOfTypeAll<FullCaseSandboxWindow>().Length==0);
    Consistency();string path=Path.Combine(Output,"observed-errors.txt");string errors=File.Exists(path)?File.ReadAllText(path):"";
    Add("no invalid sandbox window or gameplay panel exception",!errors.Contains("ConRoi.Gameplay.EditorChecks.FullCaseSandboxWindow")&&!errors.Contains("ConRoi.Gameplay.EditorChecks.FullCaseSandboxPanel"));
    Finish();
   }
  }catch(Exception e){Add("checker exception: "+e,false);Finish();}
 }
 static void Finish(){string output=Output;SessionState.SetString(K,"done");var checks=JArray.Parse(File.ReadAllText(Path.Combine(output,"checks.json")));File.WriteAllText(Path.Combine(output,"results.json"),new JObject{["kind"]="ISOLATED_BATCH_EDITOR_LIFECYCLE_ACTUAL_COMPILE_AND_PLAY_DOMAIN_RELOAD_NOT_WINDOWS_INPUT",["checks"]=checks}.ToString());EditorApplication.Exit(checks.All(c=>(bool)c["pass"])?0:1);}
}}
