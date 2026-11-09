using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using ConRoi.Investigation.V36.R107;
namespace ConRoi.Integration.EditorChecks {
// Read-only gameplay inspection. No grants, teleport, API page changes, Save or Load commands.
[InitializeOnLoad] public static class IntegratedRuntimeReviewChecks {
const string Key="ConRoi.IntegrationReview";
const string Scene="Assets/ConRoi/Modules/TichHop/Scenes/ConRoi_Chinh.unity";
static double next;
static IntegratedRuntimeReviewChecks(){EditorApplication.update+=Tick;}
static bool IsDefault<T>(T value)=>System.Collections.Generic.EqualityComparer<T>.Default.Equals(value,default(T));
static string Arg(string key){var a=Environment.GetCommandLineArgs();int i=Array.IndexOf(a,key);return i>=0&&i+1<a.Length?a[i+1]:"";}
public static void StartGui(){
 if(Application.unityVersion!="6000.6.2f1"||Application.isBatchMode||EditorApplication.isPlayingOrWillChangePlaymode||SceneManager.GetActiveScene().isDirty)throw new InvalidOperationException("Need correct GUI Editor, no Play or dirty scene");
 string profile=Arg("--investigation-save-dir"),output=Arg("--integration-review-output");if(!Path.IsPathRooted(profile)||Directory.Exists(profile)||!Path.IsPathRooted(output))throw new InvalidOperationException("Need NEW absolute private profile and output");
 Directory.CreateDirectory(output);EditorSceneManager.OpenScene(Scene,OpenSceneMode.Single);
 int missing=0,components=0;var broken=new JArray();
 foreach(var root in SceneManager.GetActiveScene().GetRootGameObjects())foreach(var t in root.GetComponentsInChildren<Transform>(true)){
 missing+=GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(t.gameObject);
 foreach(var c in t.GetComponents<Component>())if(c){components++;using(var so=new SerializedObject(c)){var prop=so.GetIterator();while(prop.Next(true))if(prop.propertyType==SerializedPropertyType.ObjectReference&&prop.objectReferenceValue==null&&!IsDefault(prop.objectReferenceEntityIdValue))broken.Add(c.name+"/"+c.GetType().Name+"/"+prop.propertyPath);}}
 }
 var session=UnityEngine.Object.FindAnyObjectByType<InvestigationSession>();
 File.WriteAllText(Path.Combine(output,"reference-audit.json"),new JObject{["scene"]=Scene,["unity"]=Application.unityVersion,["components"]=components,["missingScripts"]=missing,["brokenReferences"]=broken,["sessionCount"]=UnityEngine.Object.FindObjectsByType<InvestigationSession>(FindObjectsSortMode.None).Length,["bindings"]=new JObject{["movement"]=session&&session.movement,["look"]=session&&session.look,["eye"]=session&&session.eye,["gate"]=session&&session.gate,["modelGate"]=session&&session.modelGate,["stageCamera"]=session&&session.stageCamera,["stageTexture"]=session&&session.stageTexture}}.ToString());
 SessionState.SetString(Key+".output",output);SessionState.SetString(Key+".phase","pending");EditorApplication.EnterPlaymode();
}
static void SetupView(){
 var asm=typeof(Editor).Assembly;var viewType=asm.GetType("UnityEditor.GameView");var view=EditorWindow.GetWindow(viewType);view.Show();view.maximized=true;view.Focus();
 var sizesType=asm.GetType("UnityEditor.GameViewSizes");var singleton=typeof(ScriptableSingleton<>).MakeGenericType(sizesType);var sizes=singleton.GetProperty("instance",BindingFlags.Public|BindingFlags.Static|BindingFlags.FlattenHierarchy).GetValue(null);
 var group=sizesType.GetMethod("GetGroup").Invoke(sizes,new object[]{0});var g=group.GetType();int count=(int)g.GetMethod("GetTotalCount").Invoke(group,null),index=-1;
 for(int i=0;i<count;i++){var size=g.GetMethod("GetGameViewSize").Invoke(group,new object[]{i});var t=size.GetType();if((int)t.GetProperty("width").GetValue(size)==1920&&(int)t.GetProperty("height").GetValue(size)==1080)index=i;}
 if(index<0){var st=asm.GetType("UnityEditor.GameViewSize");var et=asm.GetType("UnityEditor.GameViewSizeType");var size=Activator.CreateInstance(st,BindingFlags.Instance|BindingFlags.Public|BindingFlags.NonPublic,null,new object[]{Enum.ToObject(et,1),1920,1080,"Integration review 1080p"},null);g.GetMethod("AddCustomSize").Invoke(group,new[]{size});index=count;}
 viewType.GetProperty("selectedSizeIndex",BindingFlags.Instance|BindingFlags.Public|BindingFlags.NonPublic).SetValue(view,index);Application.runInBackground=true;
}
static JObject State(){
 var s=UnityEngine.Object.FindAnyObjectByType<InvestigationSession>();var o=new JObject{["utc"]=DateTime.UtcNow.ToString("o"),["scene"]=SceneManager.GetActiveScene().path,["sceneDirty"]=SceneManager.GetActiveScene().isDirty,["unity"]=Application.unityVersion,["play"]=EditorApplication.isPlaying,["width"]=Screen.width,["height"]=Screen.height};
 if(s){o["data"]=JObject.Parse(JsonUtility.ToJson(s.data));o["dataIdentity"]=RuntimeHelpers.GetHashCode(s.data);o["page"]=s.Page;o["paused"]=s.Paused;o["modal"]=s.Modal;o["objective"]=s.CurrentObjective;o["message"]=s.Message;o["savePath"]=s.SavePath;o["position"]=JObject.Parse(JsonUtility.ToJson(s.movement.transform.position));o["yaw"]=s.movement.transform.eulerAngles.y;o["pitch"]=s.eye.transform.localEulerAngles.x;o["movement"]=s.movement.enabled;o["look"]=s.look.enabled;o["gate"]=JObject.Parse(JsonUtility.ToJson(s.gate.state));o["trialId"]=s.TrialId;o["trialTime"]=s.TrialTime;o["trialRunning"]=s.TrialRunning;o["trialReport"]=s.TrialReport;o["modelGate"]=JObject.Parse(JsonUtility.ToJson(s.modelGate.state));o["clipTime"]=s.ClipTime;
 o["interactables"]=new JArray(UnityEngine.Object.FindObjectsByType<InvestigationInteractable>(FindObjectsSortMode.None).Select(x=>new JObject{["id"]=x.evidenceId,["action"]=x.action,["name"]=x.name,["position"]=JObject.Parse(JsonUtility.ToJson(x.transform.position)),["distance"]=Vector3.Distance(s.eye.transform.position,x.transform.position)}));
 if(s.TryInteractionHit(out var hit)){var item=hit.collider.GetComponentInParent<InvestigationInteractable>();o["ray"]=new JObject{["name"]=hit.collider.name,["distance"]=hit.distance,["id"]=item?item.evidenceId:"",["action"]=item?item.action:""};}}
 return o;
}
static void Tick(){
 string phase=SessionState.GetString(Key+".phase","");if(phase==""||EditorApplication.isCompiling||EditorApplication.isUpdating)return;string output=SessionState.GetString(Key+".output","");
 try{if(phase=="pending"&&EditorApplication.isPlaying){SessionState.SetString(Key+".phase","running");SetupView();}
 if(EditorApplication.timeSinceStartup<next)return;next=EditorApplication.timeSinceStartup+.2;var state=State();File.WriteAllText(Path.Combine(output,"state.json"),state.ToString());
 string cp=Path.Combine(output,"command.json");if(!File.Exists(cp))return;var command=JObject.Parse(File.ReadAllText(cp));string nonce=(string)command["nonce"];if(nonce==SessionState.GetString(Key+".nonce",""))return;SessionState.SetString(Key+".nonce",nonce);string action=(string)command["action"];
 if(action=="snapshot"){string stem="snapshot_"+Guid.Parse(nonce).ToString("N");File.WriteAllText(Path.Combine(output,stem+".json"),state.ToString());if(EditorApplication.isPlaying)ScreenCapture.CaptureScreenshot(Path.Combine(output,stem+".png"));}
 else if(action=="stop"){File.WriteAllText(Path.Combine(output,"before-stop.json"),state.ToString());EditorApplication.ExitPlaymode();SessionState.SetString(Key+".phase","stopped");}
 else if(action=="quit"&&!EditorApplication.isPlayingOrWillChangePlaymode){SessionState.SetString(Key+".phase","");EditorApplication.Exit(0);}else throw new InvalidOperationException("Unknown or unsafe command");
 }catch(Exception e){File.WriteAllText(Path.Combine(output,"bridge-error.txt"),e.ToString());Debug.LogException(e);}
}
}}
