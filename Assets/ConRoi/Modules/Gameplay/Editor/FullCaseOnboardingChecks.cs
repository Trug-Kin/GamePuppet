using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using Newtonsoft.Json.Linq;
using ConRoi.Investigation.V36.R107;
using ConRoi.Gameplay.TestOnly;
namespace ConRoi.Gameplay.EditorChecks {
// Real movement input, E/nearest raycast and Game View controls. State reads only.
[InitializeOnLoad] public static class FullCaseOnboardingChecks {
 const string K="ConRoi.Onboarding.Check";
 static FullCasePlayHost h;static FullCaseSandboxPanel w;static FullInvestigationRuntime S=>h.Runtime;
 static Vector2 eventOffset;static bool calibrated;static Vector2 mouseDelta;static int mouseFrames;static Keyboard keyboard;static Mouse mouse;static EditorWindow view;
 static readonly Stack<IEnumerator> tasks=new Stack<IEnumerator>();
 static JArray rows,inputs;static string output,step="boot";static int frame=-1,fail;static double began,next,progress;
 static FullCaseOnboardingChecks(){EditorApplication.update+=Tick;}
 static string Arg(string k){var a=Environment.GetCommandLineArgs();int i=Array.IndexOf(a,k);return i>=0&&i+1<a.Length?a[i+1]:"";}
 public static void RunBatch(){output=Arg("--world-output");if(!Path.IsPathRooted(output)||Directory.Exists(output))throw new Exception("Fresh private output; batch Unity only");Directory.CreateDirectory(output);SessionState.SetString(K+".out",output);SessionState.SetString(K,"pending");EditorSceneManager.OpenScene(FullCasePlayLauncher.Scene);FullCasePlayLauncher.Open();}
 static void C(string name,bool pass){rows.Add(new JObject{["name"]=name,["pass"]=pass,["location"]=S.Location,["modal"]=S.Modal.ToString(),["message"]=S.LastMessage});if(!pass)throw new Exception(name);}
 static void Tick(){if(EditorApplication.isCompiling||EditorApplication.isUpdating)return;var phase=SessionState.GetString(K,"");if(phase=="finished"&&!EditorApplication.isPlayingOrWillChangePlaymode){SessionState.SetString(K,"");EditorApplication.Exit(SessionState.GetInt(K+".exit",2));return;}if(!EditorApplication.isPlaying)return;
 try{if(phase=="pending"&&FullCasePlayLauncher.Active){h=FullCasePlayLauncher.Active;w=FullCasePlayLauncher.Panel;output=SessionState.GetString(K+".out","");rows=new JArray();inputs=new JArray();keyboard=InputSystem.AddDevice<Keyboard>("PrivateWorldKeyboard");mouse=InputSystem.AddDevice<Mouse>("PrivateWorldMouse");InputSystem.onBeforeUpdate+=InjectMouse;InputSystem.onAfterUpdate+=TraceMouse;view=EditorWindow.GetWindow(typeof(Editor).Assembly.GetType("UnityEditor.GameView"));view.position=new Rect(0,0,1280,940);view.Focus();began=EditorApplication.timeSinceStartup;tasks.Push(Route());SessionState.SetString(K,"running");next=began+.6;return;}
 if(phase!="running"||Time.frameCount==frame||EditorApplication.timeSinceStartup<next)return;frame=Time.frameCount;next=EditorApplication.timeSinceStartup+.025;view.Focus();view.Repaint();
 if(EditorApplication.timeSinceStartup-began>400)throw new Exception("Route timeout "+step);
 if(EditorApplication.timeSinceStartup-progress>1){progress=EditorApplication.timeSinceStartup;File.WriteAllText(Path.Combine(output,"progress.txt"),step+"\n"+rows.Count+" checks / "+inputs.Count+" inputs / "+h.Movement.transform.position);}
 while(tasks.Count>0){var t=tasks.Peek();if(!t.MoveNext()){tasks.Pop();continue;}if(t.Current is IEnumerator nested){tasks.Push(nested);continue;}return;}Finish();
 }catch(Exception e){fail++;rows?.Add(new JObject{["pass"]=false,["step"]=step,["exception"]=e.ToString()});Finish();}}
 static void TraceMouse(){if(h!=null&&mouse!=null&&mouseDelta!=Vector2.zero)inputs.Add(new JObject{["kind"]="MOUSE_DIAGNOSTIC",["phase"]=InputState.currentUpdateType.ToString(),["delta"]=mouse.delta.ReadValue().ToString(),["look"]=h.Movement.LookInput.ToString(),["cursor"]=Cursor.lockState.ToString(),["lookEnabled"]=h.Look.enabled,["yaw"]=h.Movement.transform.eulerAngles.y,["focused"]=EditorWindow.focusedWindow?.GetType().Name});}
 static void InjectMouse(){if(InputState.currentUpdateType==InputUpdateType.Dynamic&&mouse!=null&&mouseDelta!=Vector2.zero){InputSystem.QueueDeltaStateEvent(mouse.delta,mouseDelta);mouseFrames++;}}
 static void Finish(){InputSystem.onBeforeUpdate-=InjectMouse;InputSystem.onAfterUpdate-=TraceMouse;if(h){File.WriteAllText(Path.Combine(output,"final-state.json"),S.ExportStateJson());try{var tex=ScreenCapture.CaptureScreenshotAsTexture();if(tex){File.WriteAllBytes(Path.Combine(output,fail==0?"final.png":"failure.png"),tex.EncodeToPNG());UnityEngine.Object.DestroyImmediate(tex);}}catch(Exception e){inputs.Add(new JObject{["kind"]="CAPTURE_LIMIT",["message"]=e.Message});}}File.WriteAllText(Path.Combine(output,"results.json"),new JObject{["source"]=Arg("--world-source"),["kind"]="PLAY_MODE_VIRTUAL_INPUT_SYSTEM_KEYBOARD_MOUSE_AND_TARGETED_GAMEVIEW_IMGUI_NOT_WINDOWS_NOT_NATURAL",["scene"]=FullCasePlayLauncher.Scene,["profile"]=h?.Profile,["sourceFiles"]=SourceManifest(),["failures"]=fail,["checks"]=rows,["inputs"]=inputs}.ToString());if(keyboard!=null&&keyboard.added)InputSystem.RemoveDevice(keyboard);if(mouse!=null&&mouse.added)InputSystem.RemoveDevice(mouse);tasks.Clear();SessionState.SetInt(K+".exit",fail==0?0:1);SessionState.SetString(K,"finished");EditorApplication.ExitPlaymode();}
 static IEnumerator Delay(double seconds){var until=EditorApplication.timeSinceStartup+seconds;while(EditorApplication.timeSinceStartup<until)yield return null;}
 static IEnumerator Key(Key key,int frames=2){InputSystem.QueueStateEvent(keyboard,new KeyboardState(key));for(int i=0;i<frames;i++)yield return null;InputSystem.QueueStateEvent(keyboard,new KeyboardState());yield return null;inputs.Add(new JObject{["key"]=key.ToString(),["kind"]="INPUT_SYSTEM_KEY",["location"]=S.Location});}
 static IEnumerator Look(float x,float y){int before=mouseFrames;mouseDelta=new Vector2(x,y);while(mouseFrames==before)yield return null;mouseDelta=Vector2.zero;yield return null;}
 static IEnumerator Close(){if(S.Modal!=FullCaseModal.None)yield return Key(UnityEngine.InputSystem.Key.Escape);C("world_return",S.CanMove);}
 static IEnumerator AimForward(){for(int i=0;i<12;i++){float yaw=Mathf.DeltaAngle(0,h.Movement.transform.eulerAngles.y),pitch=Mathf.DeltaAngle(0,h.Eye.transform.localEulerAngles.x);if(Mathf.Abs(yaw)<.1f&&Mathf.Abs(pitch)<.1f)yield break;yield return Look(-yaw/.02f,pitch/.02f);yield return Delay(.05);}throw new Exception("Could not aim using input");}
 static IEnumerator World(string key){yield return Close();yield return AimForward();step="world:"+key;var target=h.Targets[key];C("target_active_"+key,target.gameObject.activeInHierarchy);float x=target.transform.position.x,z=target.transform.position.z-2.7f;var until=EditorApplication.timeSinceStartup+30;
 while(Mathf.Abs(x-h.Movement.transform.position.x)>.12f||Mathf.Abs(z-h.Movement.transform.position.z)>.12f){if(EditorApplication.timeSinceStartup>until)throw new Exception("Movement timeout "+key);float dx=x-h.Movement.transform.position.x,dz=z-h.Movement.transform.position.z;var keys=new List<UnityEngine.InputSystem.Key>();if(Mathf.Abs(dx)>.12f)keys.Add(dx>0?UnityEngine.InputSystem.Key.D:UnityEngine.InputSystem.Key.A);if(Mathf.Abs(dz)>.12f)keys.Add(dz>0?UnityEngine.InputSystem.Key.W:UnityEngine.InputSystem.Key.S);InputSystem.QueueStateEvent(keyboard,new KeyboardState(keys.ToArray()));yield return null;}
 InputSystem.QueueStateEvent(keyboard,new KeyboardState());yield return Delay(.1);yield return AimForward();yield return Delay(.1);
 C("nearest_raycast_"+key,h.Focused==target);int before=h.RayInteractions;yield return Key(UnityEngine.InputSystem.Key.E);C("E_interaction_"+key,h.LastWorldKey==key&&h.RayInteractions==before+1);}
 static IEnumerator Go(string id){string from=S.Location;yield return World("travel."+from+"."+id);C("travel_"+id,S.Location==id);}
 static IEnumerator Delivered(Func<bool> condition){double until=EditorApplication.timeSinceStartup+8;while(!condition()){if(EditorApplication.timeSinceStartup>until)throw new Exception("Game View event delivery timeout "+step);yield return null;}yield return null;}
 static IEnumerator Click(string label){step="click:"+label;var until=EditorApplication.timeSinceStartup+12;Rect r;
 while(!w.RevealControl(label,out r)){if(EditorApplication.timeSinceStartup>until)throw new Exception("Control unreachable "+label+" "+w.GeometryForChecks(label));yield return null;}
 if(!calibrated){int n=h.PointerEvents;var probe=new Vector2((Screen.width-40)/EditorGUIUtility.pixelsPerPoint,100);view.SendEvent(new Event{type=EventType.MouseDown,button=1,mousePosition=probe});view.SendEvent(new Event{type=EventType.MouseUp,button=1,mousePosition=probe});yield return Delivered(()=>h.PointerEvents>=n+2);eventOffset=h.LastPointer-probe*EditorGUIUtility.pixelsPerPoint;calibrated=true;inputs.Add(new JObject{["kind"]="SAFE_BLANK_AREA_EVENT_CALIBRATION",["offsetPixels"]=eventOffset.ToString()});}
 w.RevealControl(label,out r);var pos=(r.center-eventOffset)/EditorGUIUtility.pixelsPerPoint;int before=h.PointerEvents;
 view.SendEvent(new Event{type=EventType.MouseDown,button=0,mousePosition=pos});view.SendEvent(new Event{type=EventType.MouseUp,button=0,mousePosition=pos});yield return Delivered(()=>h.PointerEvents>=before+2);inputs.Add(new JObject{["kind"]="GAMEVIEW_IMGUI_MOUSE",["control"]=label,["point"]=pos.ToString(),["rect"]=r.ToString(),["received"]=h.LastPointer.ToString(),["screen"]=Screen.width+"x"+Screen.height,["dpi"]=EditorGUIUtility.pixelsPerPoint});}
 static IEnumerator Text(string label,string value){yield return Click(label);yield return Key(UnityEngine.InputSystem.Key.P);C("P_does_not_close_focused_text_"+label,S.Modal==FullCaseModal.Cipher);int before=h.KeyEvents;view.SendEvent(new Event{type=EventType.KeyDown,keyCode=KeyCode.A,modifiers=EventModifiers.Control});view.SendEvent(new Event{type=EventType.KeyDown,keyCode=KeyCode.Backspace});foreach(char c in value)view.SendEvent(new Event{type=EventType.KeyDown,character=c});view.SendEvent(new Event{type=EventType.KeyDown,keyCode=KeyCode.Tab});yield return Delivered(()=>h.KeyEvents>=before+value.Length+3);}
 static IEnumerator View(string id,bool record=false){yield return World("evidence."+id);C("inspect_"+id,S.GetRecords().Any(r=>r.id==id&&r.viewed));if(record){yield return Click("Ghi mục 0 • "+S.GetObservationOptions(id)[0]);C("record_"+id,S.HasRecordedObservation(id,0));}yield return Close();}
 static IEnumerator Ask(string topic){yield return World("actor."+S.Location);var q=S.GetConversationTopics().Single(t=>t.Id==topic);C("topic_open_"+topic,q.CanAsk);yield return Click(q.Question);C("heard_"+topic,S.GetHeardStatements().Any(t=>t.topicId==topic));yield return Close();}
 static IEnumerator Page(string label){if(S.Modal==FullCaseModal.None)yield return Key(UnityEngine.InputSystem.Key.P);yield return Click(label);}
 static IEnumerator Decode(string id){yield return Page("Mật mã");yield return Click("Thư • "+id);yield return Click("Ghi bản đọc");C("decoded_"+id,S.GetRecordedReadings().Any(r=>r.Id==id));yield return Close();}
 static IEnumerator Wait(Func<bool> condition,string label){step=label;var until=EditorApplication.timeSinceStartup+60;while(!condition()){if(EditorApplication.timeSinceStartup>until)throw new Exception("Timeout "+label);yield return null;}C(label,true);}
 static IEnumerator Shot(string name){ScreenCapture.CaptureScreenshot(Path.Combine(output,name+".png"));yield return null;yield return null;}
 static JObject SourceManifest(){var result=new JObject();foreach(string file in new[]{"Assets/ConRoi/Modules/Gameplay/Editor/FullCasePlayLauncher.cs","Assets/ConRoi/Modules/Gameplay/Editor/FullCaseSandbox.cs","Assets/ConRoi/Modules/Gameplay/Editor/FullCasePlayGuide.cs","Assets/ConRoi/Modules/Gameplay/Testing/FullCasePlayHost.cs","Assets/ConRoi/Modules/Gameplay/Testing/FullCasePlayTarget.cs","Assets/ConRoi/Modules/Gameplay/Editor/FullCaseOnboardingChecks.cs"})using(var hash=System.Security.Cryptography.SHA256.Create())result[file]=BitConverter.ToString(hash.ComputeHash(File.ReadAllBytes(file))).Replace("-","").ToLowerInvariant();return result;}
 static string Guide()=>h.GetType().GetField("Guide")?.GetValue(h) is Func<string> g?g():"";
 static IEnumerator Route(){yield return Delay(.5);bool fixedVersion=Arg("--onboarding-fixed")=="yes";
 C("clean profile real controller",S.GetRecords().Length==0&&S.GetHeardStatements().Length==0&&!Directory.Exists(h.Profile)&&!h.Movement.TestInputHookActive);
 C("initial guide expected",fixedVersion?Guide().Contains("Phương"):Guide()=="");
 C("Phuong binding office",h.Targets["actor.office"].StableId=="npc.phuong");
 yield return Shot("first-step");
 yield return World("actor.office");C("Phuong conversation opens",S.Modal==FullCaseModal.Conversation);
 var first=S.GetConversationTopics().Single(q=>q.Id=="phuong.direction.homestay");var locked=S.GetConversationTopics().Single(q=>q.Id=="phuong.samples.origin");
 C("direction open sample topic locked without answer",first.CanAsk&&!locked.CanAsk&&locked.Question=="");
 string before=S.ExportStateJson();yield return Click(fixedVersion?"Chủ đề chưa mở":"Chủ đề khóa • "+locked.Reason);
 C("locked ask does not grant anything",S.ExportStateJson()==before&&w.ControllerForChecks.Reply=="");
 string permissionLabel=fixedVersion?"Xin phép nhận mẫu đối chiếu":S.GetPermissions().Single().label;
 yield return Click(permissionLabel);C("permission before Khai stays locked",!S.HasPermission("test.samples"));if(fixedVersion)C("permission UI points to Khai encounter",((string)typeof(FullCaseSandboxPanel).GetField("message",System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.NonPublic).GetValue(w)).Contains("Gặp Khải"));
 yield return Click(first.Question);C("direction heard opens homestay without Ghi",S.HasHeardStatement("phuong.direction.homestay")&&S.CanTravel("homestay")&&S.GetRecordedStatements().Length==0);
 yield return Close();C("guide points to workshop",!fixedVersion||Guide().Contains("Xưởng"));
 yield return Go("workshop");
 C("phantom actor baseline or fixed",fixedVersion?!h.Targets.ContainsKey("actor.workshop"):h.Targets["actor.workshop"].StableId=="npc.khai");
 if(!fixedVersion){yield return World("actor.workshop");C("baseline phantom Khai rejected",S.Modal==FullCaseModal.None&&S.LastMessage=="Không thể bắt đầu trao đổi ở đây.");}
 else C("only supported actors are spawned",h.Targets.Values.Where(t=>t.Action==FullPlayAction.Actor).All(t=>new[]{"npc.phuong","npc.ha","npc.mai","npc.bao"}.Contains(t.StableId)));
 yield return View("slide");C("view without record does not open market",!S.HasRecordedObservation("slide",0)&&!S.CanTravel("market"));
 yield return World("travel.workshop.market");C("locked travel holds workshop",S.Location=="workshop");if(fixedVersion)C("blocked travel explains missing Ghi without opening gate",h.LastFeedback.Contains("Ghi")&&h.LastFeedback.Contains("Vệt trượt")&&!S.CanTravel("market"));
 yield return View("slide",true);C("record opens existing gate",S.CanTravel("market"));
 C("guide names Khai",!fixedVersion||Guide().Contains("Khải"));yield return View("khai");
 C("real Khai encounter inspects existing record no new statement",S.GetRecords().Any(e=>e.id=="khai"&&e.viewed)&&!S.HasRecordedObservation("khai",0)&&!S.GetHeardStatements().Any(x=>x.participantId=="npc.khai"));
 C("guide returns to Phuong for access",!fixedVersion||Guide().Contains("Phương"));
 yield return Go("office");yield return World("actor.office");yield return Click(permissionLabel);C("access after encounter without extra Ghi",S.HasPermission("test.samples"));
 C("source topic still locked until samples received",!S.GetConversationTopics().Single(q=>q.Id=="phuong.samples.origin").CanAsk);yield return Close();
 yield return Go("homestay");yield return View("samples");C("samples received through real E",S.HasEvidence("samples"));
 yield return Go("office");yield return World("actor.office");
 var origin=S.GetConversationTopics().Single(q=>q.Id=="phuong.samples.origin");C("source topic now available",origin.CanAsk);
 yield return Click(origin.Question);int count=S.GetHeardStatements().Length;yield return Click(origin.Question);C("repeat does not duplicate statements",S.GetHeardStatements().Length==count&&S.HasHeardStatement("phuong.samples.origin"));yield return Close();
 string objective=Guide();int records=S.GetRecords().Length;yield return Key(UnityEngine.InputSystem.Key.F5);C("private save created",File.Exists(FullInvestigationRuntime.SavePathFor(h.Profile)));yield return Key(UnityEngine.InputSystem.Key.P);yield return Key(UnityEngine.InputSystem.Key.F6);
 C("load retains real progress and derived objective",S.Modal==FullCaseModal.None&&S.HasPermission("test.samples")&&S.HasEvidence("samples")&&S.HasRecordedObservation("slide",0)&&S.GetRecords().Length==records&&S.GetHeardStatements().Length==count&&Guide()==objective);
 if(fixedVersion){C("human actor and encounter labels",h.Targets["actor.office"].Label=="Phương"&&h.Targets["evidence.khai"].Label.Contains("Khải")&&!h.Targets["actor.office"].Label.Contains("npc."));
 string state=S.ExportStateJson();for(int i=0;i<3;i++)Guide();C("guide query does not mutate state",state==S.ExportStateJson());}
 yield return Key(UnityEngine.InputSystem.Key.Escape);C("pause still blocks controller",S.Paused&&!h.Movement.enabled&&!h.Look.enabled);yield return Key(UnityEngine.InputSystem.Key.Escape);C("resume restores controller",!S.Paused&&h.Movement.enabled&&h.Look.enabled);yield return Shot("after-return");
 }
 static string LockedText(string topic){var type=typeof(FullCasePlayLauncher).Assembly.GetType("ConRoi.Gameplay.EditorChecks.FullCasePlayGuide");return (string)type.GetMethod("LockedTopic").Invoke(null,new object[]{S,topic});}
}}
