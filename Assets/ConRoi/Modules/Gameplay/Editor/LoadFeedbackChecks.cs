using System;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using ConRoi.Investigation.V36.R107;
namespace ConRoi.Gameplay.EditorChecks {
// Explicit serialized-file/API fixture. Does not run unless RunBatch is requested.
[InitializeOnLoad] public static class LoadFeedbackChecks {
 const string Key="ConRoi.LoadFeedback01",Scene="Assets/ConRoi/Modules/Gameplay/Scenes/Gameplay_ThuNghiem.unity";
 static LoadFeedbackChecks(){EditorApplication.update+=Tick;EditorApplication.update+=GuiSize;}
 static string Arg(string key){var a=Environment.GetCommandLineArgs();int i=Array.IndexOf(a,key);return i>=0&&i+1<a.Length?a[i+1]:"";}
 public static void RunBatch(){
  if(Application.unityVersion!="6000.6.2f1"||!Application.isBatchMode||EditorApplication.isPlayingOrWillChangePlaymode)throw new InvalidOperationException("Need explicit batch Unity 6000.6.2f1 outside Play");
  string profile=Arg("--investigation-save-dir"),output=Arg("--load-feedback-output");if(!Path.IsPathRooted(profile)||Directory.Exists(profile)||!Path.IsPathRooted(output))throw new InvalidOperationException("Need NEW absolute test profile and output");
  Directory.CreateDirectory(output);EditorSceneManager.OpenScene(Scene,OpenSceneMode.Single);SessionState.SetString(Key,"pending");EditorApplication.EnterPlaymode();
 }
 public static void StartGui(){SaveGuardControllerChecks.StartGui();SessionState.SetBool(Key+".GuiSize",true);}
 static void GuiSize(){
  if(!SessionState.GetBool(Key+".GuiSize",false)||UnityEngine.Object.FindAnyObjectByType<InvestigationSession>()==null)return;
  var asm=typeof(Editor).Assembly;var vt=asm.GetType("UnityEditor.GameView");var view=EditorWindow.GetWindow(vt);view.Show();view.maximized=true;view.Focus();
  var st=asm.GetType("UnityEditor.GameViewSizes");var single=typeof(ScriptableSingleton<>).MakeGenericType(st);var sizes=single.GetProperty("instance",System.Reflection.BindingFlags.Public|System.Reflection.BindingFlags.Static|System.Reflection.BindingFlags.FlattenHierarchy).GetValue(null);
  var group=st.GetMethod("GetGroup").Invoke(sizes,new object[]{0});var gt=group.GetType();int count=(int)gt.GetMethod("GetTotalCount").Invoke(group,null),index=-1;
  for(int i=0;i<count;i++){var size=gt.GetMethod("GetGameViewSize").Invoke(group,new object[]{i});var t=size.GetType();if((int)t.GetProperty("width").GetValue(size)==1920&&(int)t.GetProperty("height").GetValue(size)==1080)index=i;}
  if(index<0){var t=asm.GetType("UnityEditor.GameViewSize");var e=asm.GetType("UnityEditor.GameViewSizeType");var size=Activator.CreateInstance(t,System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.Public|System.Reflection.BindingFlags.NonPublic,null,new object[]{Enum.ToObject(e,1),1920,1080,"Load feedback 1080p"},null);gt.GetMethod("AddCustomSize").Invoke(group,new[]{size});index=count;}
  vt.GetProperty("selectedSizeIndex",System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.Public|System.Reflection.BindingFlags.NonPublic).SetValue(view,index);SessionState.SetBool(Key+".GuiSize",false);
 }
 static void Tick(){if(EditorApplication.isCompiling||EditorApplication.isUpdating)return;string phase=SessionState.GetString(Key,"");if(phase=="pending"&&EditorApplication.isPlaying){SessionState.SetString(Key,"running");Execute();}else if(phase=="finished"&&!EditorApplication.isPlayingOrWillChangePlaymode){SessionState.SetString(Key,"");EditorApplication.Exit(SessionState.GetInt(Key+".exit",2));}}
 static JObject State(InvestigationSession s)=>new JObject{["data"]=JObject.Parse(JsonUtility.ToJson(s.data)),["identity"]=RuntimeHelpers.GetHashCode(s.data),["position"]=JObject.Parse(JsonUtility.ToJson(s.movement.transform.position)),["rotation"]=JObject.Parse(JsonUtility.ToJson(s.movement.transform.rotation)),["eye"]=JObject.Parse(JsonUtility.ToJson(s.eye.transform.localRotation)),["gate"]=JObject.Parse(JsonUtility.ToJson(s.gate.state)),["modelGate"]=JObject.Parse(JsonUtility.ToJson(s.modelGate.state)),["page"]=s.Page,["paused"]=s.Paused,["modal"]=s.Modal,["movement"]=s.movement.enabled,["look"]=s.look.enabled,["controller"]=s.movement.controller.enabled,["timeScale"]=Time.timeScale,["trialId"]=s.TrialId,["trialTime"]=s.TrialTime,["trialRunning"]=s.TrialRunning};
 static void Execute(){
  var rows=new JArray();GameObject root=null;string output=Arg("--load-feedback-output");int failures=0;
  Action<string,bool> check=(name,pass)=>{rows.Add(new JObject{["name"]=name,["pass"]=pass});if(!pass)failures++;};
  try{
   root=new GameObject("LoadFeedback_API_Fixture");root.SetActive(false);var player=new GameObject("Player");player.transform.SetParent(root.transform);var cc=player.AddComponent<CharacterController>();var movement=player.AddComponent<DieuKhienDiChuyen>();movement.controller=cc;var eye=new GameObject("Eye");eye.transform.SetParent(player.transform);var camera=eye.AddComponent<Camera>();var look=eye.AddComponent<DieuKhienGocNhin>();
   var g=new GameObject("Gate");g.transform.SetParent(root.transform);var gate=g.AddComponent<ReturnGateMechanism>();gate.checkPlayerContact=false;gate.enabled=false;var m=new GameObject("Model");m.transform.SetParent(root.transform);var model=m.AddComponent<ReturnGateMechanism>();model.checkPlayerContact=false;model.enabled=false;
   var s=root.AddComponent<InvestigationSession>();s.movement=movement;s.look=look;s.eye=camera;s.gate=gate;s.modelGate=model;root.SetActive(true);s.enabled=false;movement.enabled=false;look.enabled=false;s.ClosePage();

   check("private_profile",s.SaveDirectoryOverride==Arg("--investigation-save-dir"));
   s.Save();byte[] empty=File.ReadAllBytes(s.SavePath);check("empty_valid_load",s.Load()&&s.data.evidence.Count==0);
   s.Acquire("slide");s.Observe("slide",0);s.Save();byte[] good=File.ReadAllBytes(s.SavePath);var original=JObject.Parse(System.Text.Encoding.UTF8.GetString(good));
   foreach(string page in new[]{"","dossier","pause"}){
    foreach(string kind in new[]{"validation","json","read"}){
     File.WriteAllBytes(s.SavePath,good);if(!s.Load())throw new Exception("Valid fixture setup failed");if(page=="pause")s.SetPaused(true);else if(page!="")s.OpenPage(page);
     byte[] input=good;
     if(kind=="validation"){var bad=(JObject)original.DeepClone();bad["evidence"]=JValue.CreateNull();input=System.Text.Encoding.UTF8.GetBytes(bad.ToString());}
     if(kind=="json")input=System.Text.Encoding.UTF8.GetBytes("{\"evidence\":[");
     File.WriteAllBytes(s.SavePath,input);var before=State(s);bool loaded;
     if(kind=="read"){using(var held=new FileStream(s.SavePath,FileMode.Open,FileAccess.Read,FileShare.None)){loaded=s.Load();}}
     else loaded=s.Load();
     string label=page+"_"+kind;check(label+"_reject",!loaded);check(label+"_state_unchanged",JToken.DeepEquals(before,State(s)));check(label+"_bytes_unchanged",input.SequenceEqual(File.ReadAllBytes(s.SavePath)));
     check(label+"_reason_safe",s.Message.StartsWith("Không tải: ")&&!s.Message.Contains(s.SavePath)&&!s.Message.Contains("Exception")&&!s.Message.Contains(" at "));
     if(kind=="validation")check(label+"_guard_reason_preserved",s.Message.Contains("evidence"));
     if(kind=="read")check(label+"_read_reason",s.Message.Contains("Không đọc được tệp"));
     if(kind=="json")check(label+"_format_reason",s.Message.Contains("không đúng định dạng"));
     File.WriteAllBytes(s.SavePath,good);check(label+"_recovery_clears_error",s.Load()&&s.Message.StartsWith("Đã tải")&&!s.Message.Contains("Không tải"));s.Observe("slide",1);s.Save();check(label+"_continue_save_load",s.Load()&&s.Observation("slide",1));
    }
   }
   File.WriteAllBytes(s.SavePath,empty);check("empty_after_errors",s.Load()&&s.data.evidence.Count==0);
  }catch(Exception e){rows.Add(new JObject{["name"]="fixture_exception",["pass"]=false,["detail"]=e.ToString()});failures++;}
  finally{if(root)UnityEngine.Object.DestroyImmediate(root);Time.timeScale=1;File.WriteAllText(Path.Combine(output,"results.json"),new JObject{["unity"]=Application.unityVersion,["scene"]=Scene,["kind"]="TARGETED_API_FIXTURE_NOT_UI",["checks"]=rows,["failures"]=failures}.ToString());SessionState.SetInt(Key+".exit",failures==0?0:1);SessionState.SetString(Key,"finished");EditorApplication.ExitPlaymode();}
 }
}}
