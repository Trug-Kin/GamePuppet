using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Reflection;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using Newtonsoft.Json.Linq;
using ConRoi.Investigation.V36.R107;
using ConRoi.Gameplay.TestOnly;
namespace ConRoi.Gameplay.EditorChecks {
// Opt-in windowed desktop test; never used by batch or launched automatically.
[InitializeOnLoad] public static class FullCaseInputFocusChecks {
 const string K="ConRoi.InputFocus.Check";
 static FullCasePlayHost h;static FullCaseSandboxWindow w;static FullInvestigationRuntime S=>h.Runtime;
 static EditorWindow view;static Mouse mouse;static Vector2 delta,eventOffset;static int mouseFrames,frame=-1,fail;
 static readonly Stack<IEnumerator> tasks=new Stack<IEnumerator>();
 static readonly HashSet<byte> held=new HashSet<byte>();
 static JArray checks,inputs;static string output,step="boot";static double began,next,progress;
 static bool autoFocus=true,calibrated;static IntPtr unityWindow,probeWindow;static System.Diagnostics.Process probe;
 static InputSettings.BackgroundBehavior oldBackground;
 static FullCaseInputFocusChecks(){EditorApplication.update+=Tick;}
 static string Arg(string k){var a=Environment.GetCommandLineArgs();int i=Array.IndexOf(a,k);return i>=0&&i+1<a.Length?a[i+1]:"";}
 public static void RunDesktop(){output=Arg("--input-output");if(Application.isBatchMode||Arg("--exclusive-desktop")!="yes"||!Path.IsPathRooted(output)||Directory.Exists(output))throw new Exception("Explicit exclusive desktop, windowed Unity and fresh output required");
 Directory.CreateDirectory(output);SessionState.SetString(K+".out",output);SessionState.SetString(K,"pending");SessionState.SetString(K,"opening");SessionState.SetFloat(K+".boot",(float)EditorApplication.timeSinceStartup);}
 static JObject Snapshot()=>new JObject{["step"]=step,["ownPid"]=OwnPid,["unityHwnd"]=unityWindow.ToInt64(),["appFocus"]=Application.isFocused,["editorFocus"]=EditorWindow.focusedWindow?.GetType().Name,["cursor"]=Cursor.lockState.ToString(),["cursorVisible"]=Cursor.visible,["gameViewCount"]=Resources.FindObjectsOfTypeAll<EditorWindow>().Count(v=>v.GetType().Name=="GameView"),["nativeCursorShowing"]=NativeCursorShowing(),["nativePoint"]=NativePoint(),["canMove"]=S.CanMove,["moveEnabled"]=h.Movement.enabled,["lookEnabled"]=h.Look.enabled,["location"]=S.Location,["modal"]=S.Modal.ToString(),["paused"]=S.Paused,["yaw"]=Yaw,["pitch"]=Pitch,["position"]=h.Movement.transform.position.ToString(),["worldInteractions"]=h.RayInteractions,["nativeWindowTitle"]=WindowTitle(unityWindow),["foregroundTitle"]=WindowTitle(GetForegroundWindow()),["actionsEnabled"]=h.Movement.GameplayActionsEnabled,["keyboardEnabled"]=Keyboard.current?.enabled,["keyboardName"]=Keyboard.current?.name,["mouseEnabled"]=Mouse.current?.enabled,["mouseName"]=Mouse.current?.name,["foregroundPid"]=ForegroundPid()};
 static void C(string name,bool pass){var row=Snapshot();row["name"]=name;row["pass"]=pass;checks.Add(row);if(!pass)fail++;}
 static void Require(string name,bool pass){C(name,pass);if(!pass)throw new Exception(name);}
 static float Yaw=>Mathf.DeltaAngle(0,h.Movement.transform.eulerAngles.y);
 static float Pitch=>Mathf.DeltaAngle(0,h.Eye.transform.localEulerAngles.x);
 static uint OwnPid=>(uint)System.Diagnostics.Process.GetCurrentProcess().Id;
 static void Tick(){if(EditorApplication.isCompiling||EditorApplication.isUpdating)return;string phase=SessionState.GetString(K,"");
 if(phase=="finished"&&!EditorApplication.isPlayingOrWillChangePlaymode){SessionState.SetString(K,"");EditorApplication.Exit(SessionState.GetInt(K+".exit",2));return;}
 if(phase=="opening"){var own=System.Diagnostics.Process.GetCurrentProcess();own.Refresh();unityWindow=own.MainWindowHandle;if(unityWindow==IntPtr.Zero)return;ShowWindow(unityWindow,9);SetForegroundWindow(unityWindow);EditorSceneManager.OpenScene(FullCasePlayLauncher.Scene);SessionState.SetString(K,"pending");FullCasePlayLauncher.Open();return;}if(!EditorApplication.isPlaying)return;
 try{if(phase=="pending"&&FullCasePlayLauncher.Active){h=FullCasePlayLauncher.Active;w=FullCasePlayLauncher.Panel;output=SessionState.GetString(K+".out","");checks=new JArray();inputs=new JArray();mouse=InputSystem.AddDevice<Mouse>("PrivateFocusProbeMouse");InputSystem.onBeforeUpdate+=Inject;foreach(var previousView in Resources.FindObjectsOfTypeAll<EditorWindow>().Where(v=>v.GetType().Name=="GameView").ToArray())previousView.Close();view=(EditorWindow)ScriptableObject.CreateInstance(typeof(Editor).Assembly.GetType("UnityEditor.GameView"));view.titleContent=new GUIContent("ConRoi TEST ONLY Game View");view.position=new Rect(0,40,1280,900);view.Show();view.Focus();var proc=System.Diagnostics.Process.GetCurrentProcess();proc.Refresh();unityWindow=proc.MainWindowHandle;ShowWindow(unityWindow,9);SetForegroundWindow(unityWindow);oldBackground=InputSystem.settings.backgroundBehavior;began=EditorApplication.timeSinceStartup;tasks.Push(Arg("--input-probe")=="cursor"?CursorProbe():Route());SessionState.SetString(K,"running");next=began+1;return;}
 if(phase!="running"||Time.frameCount==frame||EditorApplication.timeSinceStartup<next)return;frame=Time.frameCount;next=EditorApplication.timeSinceStartup+.02;
 if(autoFocus)view.Focus();view.Repaint();if(EditorApplication.timeSinceStartup-began>240)throw new Exception("Timeout "+step);
 if(EditorApplication.timeSinceStartup-progress>1){progress=EditorApplication.timeSinceStartup;File.WriteAllText(Path.Combine(output,"progress.txt"),step+"\\n"+checks.Count+" checks / "+Snapshot());}
 while(tasks.Count>0){var t=tasks.Peek();if(!t.MoveNext()){tasks.Pop();continue;}if(t.Current is IEnumerator nested){tasks.Push(nested);continue;}return;}Finish();
 }catch(Exception e){fail++;checks?.Add(new JObject{["pass"]=false,["step"]=step,["exception"]=e.ToString()});Finish();}}
 static void Inject(){if(InputState.currentUpdateType==InputUpdateType.Dynamic&&mouse!=null&&delta!=Vector2.zero){InputSystem.QueueDeltaStateEvent(mouse.delta,delta);mouseFrames++;delta=Vector2.zero;}}
 static void Finish(){SessionState.SetString(K,"finishing");foreach(byte key in held.ToArray())SendKey(key,false,false);InputSystem.onBeforeUpdate-=Inject;delta=Vector2.zero;InputSystem.settings.backgroundBehavior=oldBackground;if(probeWindow!=IntPtr.Zero)PostMessage(probeWindow,0x10,IntPtr.Zero,IntPtr.Zero);
 if(h){File.WriteAllText(Path.Combine(output,"state.json"),S.ExportStateJson());try{var tex=ScreenCapture.CaptureScreenshotAsTexture();if(tex){File.WriteAllBytes(Path.Combine(output,"final.png"),tex.EncodeToPNG());UnityEngine.Object.DestroyImmediate(tex);}}catch(Exception capture){checks.Add(new JObject{["kind"]="CAPTURE_LIMIT",["message"]=capture.Message});}}
 File.WriteAllText(Path.Combine(output,"results.json"),new JObject{["source"]=Arg("--input-source"),["kind"]="WINDOWED_PLAY_MODE_WINDOWS_SENDINPUT_KEYS_MOUSE_PLUS_VIRTUAL_MOUSE_AND_TARGETED_GAMEVIEW_IMGUI",["profile"]=h?.Profile,["failures"]=fail,["checks"]=checks,["inputs"]=inputs,["sourceFiles"]=SourceManifest()}.ToString());if(mouse!=null&&mouse.added)InputSystem.RemoveDevice(mouse);tasks.Clear();SessionState.SetInt(K+".exit",fail==0?0:1);SessionState.SetString(K,"finished");EditorApplication.ExitPlaymode();}
 static IEnumerator Delay(double seconds){double until=EditorApplication.timeSinceStartup+seconds;while(EditorApplication.timeSinceStartup<until)yield return null;}
 static IEnumerator Wait(Func<bool> condition,string label,double timeout=8){double until=EditorApplication.timeSinceStartup+timeout;while(!condition()){if(EditorApplication.timeSinceStartup>until)throw new Exception("Timeout "+label+" "+Snapshot());yield return null;}yield return null;}
 static void SendKey(byte key,bool down,bool guard=true){if(guard&&ForegroundPid()!=OwnPid)throw new Exception("Refuse native input outside own Unity window");var input=new INPUT{type=1,u=new UNION{key=new KEYINPUT{scan=(ushort)MapVirtualKey(key,0),flags=8u|(down?0u:2u)}}};if(SendInput(1,new[]{input},Marshal.SizeOf(typeof(INPUT)))!=1)throw new Exception("SendInput key failed");if(down)held.Add(key);else held.Remove(key);}
 static IEnumerator Key(byte key,double duration=.09){step="native key "+key;SendKey(key,true);yield return Delay(duration);inputs.Add(new JObject{["kind"]="WINDOWS_KEY_DOWN_READBACK",["vk"]=key,["unityAnyKey"]=Keyboard.current?.anyKey.isPressed,["nativeDown"]=(GetAsyncKeyState(key)&0x8000)!=0});SendKey(key,false);yield return Delay(.12);inputs.Add(new JObject{["kind"]="WINDOWS_SENDINPUT_KEY",["vk"]=key,["duration"]=duration});}
 static IEnumerator NativeLook(int x,int y){if(ForegroundPid()!=OwnPid)throw new Exception("Native mouse outside Unity");var input=new INPUT{type=0,u=new UNION{mouse=new MOUSEINPUT{dx=x,dy=y,flags=1}}};SendInput(1,new[]{input},Marshal.SizeOf(typeof(INPUT)));yield return Delay(.25);inputs.Add(new JObject{["kind"]="WINDOWS_SENDINPUT_RELATIVE_MOUSE",["dx"]=x,["dy"]=y});}
 static IEnumerator VirtualLook(float x,float y){int n=mouseFrames;delta=new Vector2(x,y);yield return Wait(()=>mouseFrames>n,"virtual mouse");delta=Vector2.zero;yield return Delay(.05);inputs.Add(new JObject{["kind"]="VIRTUAL_INPUT_SYSTEM_MOUSE",["x"]=x,["y"]=y});}
 static IEnumerator Aim(float yaw=0,float pitch=0){for(int i=0;i<10;i++){float dx=Mathf.DeltaAngle(Yaw,yaw),dy=Pitch-pitch;if(Mathf.Abs(dx)<.2f&&Mathf.Abs(dy)<.2f)yield break;yield return VirtualLook(dx/.02f,dy/.02f);}throw new Exception("Cannot aim using input "+Snapshot());}
 static IEnumerator Close(){if(S.Modal!=FullCaseModal.None)yield return Key(0x1B);if(S.Paused)yield return Key(0x1B);}
 static IEnumerator Go(string key){yield return Close();yield return Aim();step="move to "+key;var t=h.Targets[key];double until=EditorApplication.timeSinceStartup+20;
 byte active=0;while(Mathf.Abs(t.transform.position.x-h.Movement.transform.position.x)>.15f){if(EditorApplication.timeSinceStartup>until)throw new Exception("Movement timeout "+Snapshot());byte k=t.transform.position.x>h.Movement.transform.position.x?(byte)0x44:(byte)0x41;if(active!=k){if(active!=0)SendKey(active,false);SendKey(k,true);active=k;}yield return null;}if(active!=0)SendKey(active,false);yield return Delay(.1);Require("raycast target "+key,h.Focused==t);yield return Key(0x45);}
 static IEnumerator Click(string label){step="GameView click "+label;double until=EditorApplication.timeSinceStartup+8;Rect r;while(!w.RevealControl(label,out r)){if(EditorApplication.timeSinceStartup>until)throw new Exception("Missing "+label+" "+w.GeometryForChecks(label));yield return null;}
 if(!calibrated){int n=h.PointerEvents;Vector2 probePoint=new Vector2((Screen.width-40)/EditorGUIUtility.pixelsPerPoint,100);view.SendEvent(new Event{type=EventType.MouseDown,button=1,mousePosition=probePoint});view.SendEvent(new Event{type=EventType.MouseUp,button=1,mousePosition=probePoint});yield return Wait(()=>h.PointerEvents>=n+2,"pointer calibration");eventOffset=h.LastPointer-probePoint*EditorGUIUtility.pixelsPerPoint;calibrated=true;}
 w.RevealControl(label,out r);var pos=(r.center-eventOffset)/EditorGUIUtility.pixelsPerPoint;int before=h.PointerEvents;view.SendEvent(new Event{type=EventType.MouseDown,button=0,mousePosition=pos});view.SendEvent(new Event{type=EventType.MouseUp,button=0,mousePosition=pos});yield return Wait(()=>h.PointerEvents>=before+2,"pointer click");inputs.Add(new JObject{["kind"]="TARGETED_GAMEVIEW_IMGUI_CLICK",["label"]=label});}
 static IEnumerator CursorProbe(){
 yield return Wait(()=>{IntPtr candidate=FindGameWindow();if(candidate!=IntPtr.Zero)unityWindow=candidate;SetForegroundWindow(unityWindow);return ForegroundPid()==OwnPid;},"cursor probe foreground",20);yield return CaptureWindowClick();
 var methods=view.GetType().GetMethods(BindingFlags.Instance|BindingFlags.Static|BindingFlags.Public|BindingFlags.NonPublic|BindingFlags.FlattenHierarchy).Where(m=>m.Name.IndexOf("Cursor",StringComparison.OrdinalIgnoreCase)>=0).ToArray();
 File.WriteAllText(Path.Combine(output,"cursor-methods.txt"),string.Join("\\n",methods.Select(m=>m.ToString())));
 C("initial native cursor hidden",!NativeCursorShowing());yield return Key(0x50);Require("panel cursor visible",NativeCursorShowing());yield return Key(0x50);C("native cursor hidden after P without click",!NativeCursorShowing());
 var allow=methods.FirstOrDefault(m=>m.Name=="AllowCursorLockAndHide"&&m.GetParameters().Length==1&&m.GetParameters()[0].ParameterType==typeof(bool));
 Require("Editor has explicit cursor context API",allow!=null);allow.Invoke(allow.IsStatic?null:view,new object[]{true});yield return Delay(.25);C("Editor cursor allow restores native hide",!NativeCursorShowing());
 yield return Key(0x1B);C("pause native cursor visible",NativeCursorShowing());yield return Key(0x1B);allow.Invoke(allow.IsStatic?null:view,new object[]{true});yield return Delay(.25);C("Editor cursor allow restores after Esc",!NativeCursorShowing());
 }
 static IEnumerator Route(){
 yield return Delay(1);yield return Wait(()=>{IntPtr candidate=FindGameWindow();if(candidate!=IntPtr.Zero)unityWindow=candidate;if(ForegroundPid()!=OwnPid)SetForegroundWindow(unityWindow);return ForegroundPid()==OwnPid;},"Unity foreground",20);yield return CaptureWindowClick();yield return Delay(.4);
 Require("clean profile and real controller",S.GetRecords().Length==0&&!Directory.Exists(h.Profile)&&!h.Movement.TestInputHookActive);
 C("world cursor locked hidden",Cursor.lockState==CursorLockMode.Locked&&!Cursor.visible&&!NativeCursorShowing());
 float yaw=Yaw;yield return NativeLook(100,0);C("Windows mouse changes yaw",Mathf.Abs(Mathf.DeltaAngle(yaw,Yaw))>.2f);yield return Aim();
 C("raycast follows camera initially",h.Focused==h.Targets["actor.office"]);yield return VirtualLook(1500,0);C("turned camera no stale hit",h.Focused!=h.Targets["actor.office"]);int hits=h.RayInteractions;yield return Key(0x45);C("E does not use old ray target",h.RayInteractions==hits);yield return Aim();
 yield return VirtualLook(0,10000);C("upper pitch clamp",Mathf.Abs(Pitch+90)<.2f);yield return VirtualLook(0,10000);C("upper pitch remains clamped",Mathf.Abs(Pitch+90)<.2f);yield return VirtualLook(0,-10000);C("lower pitch clamp",Mathf.Abs(Pitch-90)<.2f);yield return Aim();C("aim restored by input only",Mathf.Abs(Yaw)<.2f&&Mathf.Abs(Pitch)<.2f);
 yield return Key(0x45);Require("world E opens actual NPC modal",S.Modal==FullCaseModal.Conversation);
 C("modal cursor free visible",Cursor.lockState==CursorLockMode.None&&Cursor.visible&&NativeCursorShowing()&&!h.Movement.enabled&&!h.Look.enabled);
 Vector3 point=h.Movement.transform.position;Quaternion rot=h.Eye.transform.rotation;hits=h.RayInteractions;yield return Key(0x57);yield return NativeLook(80,40);yield return Key(0x45);C("modal blocks movement look E",Vector3.Distance(point,h.Movement.transform.position)<.01f&&Quaternion.Angle(rot,h.Eye.transform.rotation)<.01f&&hits==h.RayInteractions);
 yield return Key(0x1B);C("Esc restores world cursor and input",S.CanMove&&h.Movement.enabled&&h.Look.enabled&&Cursor.lockState==CursorLockMode.Locked&&!Cursor.visible&&!NativeCursorShowing());
 yield return Key(0x50);C("P opens panel cursor",S.Modal==FullCaseModal.Inspect&&Cursor.visible&&!h.Movement.enabled);yield return Key(0x50);C("P closes panel and restores cursor",S.CanMove&&Cursor.lockState==CursorLockMode.Locked&&!Cursor.visible&&!NativeCursorShowing());
 yield return Key(0x1B);C("pause frees cursor and disables input",S.Paused&&Cursor.visible&&!h.Movement.enabled&&!h.Look.enabled);point=h.Movement.transform.position;yield return Key(0x57);C("pause blocks Windows movement",Vector3.Distance(point,h.Movement.transform.position)<.01f);yield return Key(0x1B);C("pause resume captures cursor",!S.Paused&&S.CanMove&&Cursor.lockState==CursorLockMode.Locked&&!Cursor.visible&&!NativeCursorShowing());
 yield return Key(0x50);yield return Click("Mật mã");yield return Click("Cụm hồi đáp 1");
 foreach(byte k in new byte[]{0x57,0x41,0x53,0x44,0x45,0x50})yield return Key(k,.05);
 var reply=(string[])typeof(FullCaseSandboxWindow).GetField("reply",BindingFlags.Instance|BindingFlags.NonPublic).GetValue(w);
 inputs.Add(new JObject{["kind"]="TYPED_FIELD_READBACK",["text"]=reply[0]});C("native WASDEP does not trigger gameplay",S.Modal==FullCaseModal.Cipher&&h.RayInteractions==hits&&Vector3.Distance(point,h.Movement.transform.position)<.01f);
 int keyEvents=h.KeyEvents;foreach(char c in "wasdep")view.SendEvent(new Event{type=EventType.KeyDown,character=c});yield return Wait(()=>h.KeyEvents>=keyEvents+6,"GameView text chars");reply=(string[])typeof(FullCaseSandboxWindow).GetField("reply",BindingFlags.Instance|BindingFlags.NonPublic).GetValue(w);inputs.Add(new JObject{["kind"]="TARGETED_GAMEVIEW_TEXT_READBACK",["text"]=reply[0]});Require("GameView text entry works",reply[0].ToLowerInvariant().EndsWith("wasdep"));
 point=h.Movement.transform.position;hits=h.RayInteractions;string state=S.ExportStateJson();yield return Key(0x74);C("typing focus blocks F5 gameplay shortcut",!File.Exists(FullInvestigationRuntime.SavePathFor(h.Profile))&&S.ExportStateJson()==state);
 yield return Key(0x75);C("typing focus blocks F6 gameplay shortcut",S.Modal==FullCaseModal.Cipher&&S.ExportStateJson()==state&&h.RayInteractions==hits&&Vector3.Distance(point,h.Movement.transform.position)<.01f);yield return Close();
 // A small real transition + save/load. No evidence, progress or result setters.
 yield return Go("travel.office.workshop");Require("E travel changes runtime location",S.Location=="workshop");C("travel resets camera and restores control",Mathf.Abs(Yaw)<.2f&&Mathf.Abs(Pitch)<.2f&&Cursor.lockState==CursorLockMode.Locked&&h.Movement.enabled&&!NativeCursorShowing());
 yield return NativeLook(50,30);yield return Key(0x74);Require("world F5 writes private save",File.Exists(FullInvestigationRuntime.SavePathFor(h.Profile))&&S.LastMessage.StartsWith("Đã lưu"));yield return Key(0x50);yield return Key(0x75);Require("Load clears modal at safe camera spawn",S.Modal==FullCaseModal.None&&Mathf.Abs(Yaw)<.2f&&Mathf.Abs(Pitch)<.2f&&Vector3.Distance(h.Movement.transform.position,h.Spawns[S.Location])<.15f&&Cursor.lockState==CursorLockMode.Locked&&!Cursor.visible&&h.Movement.enabled&&!NativeCursorShowing());
 point=h.Movement.transform.position;yield return Key(0x44);C("movement works after load",Vector3.Distance(point,h.Movement.transform.position)>.05f);
 // In-editor focus away and back, followed by real OS application focus loss.
 autoFocus=false;var inspector=EditorWindow.GetWindow<SceneView>();inspector.Focus();yield return Delay(.25);C("other editor view releases input cursor",!h.Movement.enabled&&!h.Look.enabled&&Cursor.visible&&Cursor.lockState==CursorLockMode.None);
 view.Focus();autoFocus=true;yield return Delay(.2);Require("GameView return restores input",h.Movement.enabled&&h.Look.enabled&&Cursor.lockState==CursorLockMode.Locked&&!Cursor.visible&&!NativeCursorShowing());
 yield return Go("evidence.slide");yield return Close();Require("real target in ray before external focus loss",h.Focused==h.Targets["evidence.slide"]);string handleFile=Path.Combine(output,"probe-window.txt");probe=System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo("powershell.exe","-NoProfile -ExecutionPolicy Bypass -File \""+Path.GetFullPath("Tools/Gameplay/FullCaseFocusProbe.ps1")+"\" -HandleFile \""+handleFile+"\""){UseShellExecute=false,CreateNoWindow=true});yield return Wait(()=>File.Exists(handleFile),"external probe starts",12);probeWindow=new IntPtr(long.Parse(File.ReadAllText(handleFile)));autoFocus=false;SetForegroundWindow(probeWindow);yield return Wait(()=>GetForegroundWindow()==probeWindow,"external foreground");
 yield return Delay(.3);point=h.Movement.transform.position;rot=h.Eye.transform.rotation;hits=h.RayInteractions;
 C("OS focus loss disables game input and releases cursor",!h.Movement.enabled&&!h.Look.enabled&&Cursor.lockState==CursorLockMode.None&&Cursor.visible);
 // Diagnostic robustness: windowed editor may keep GameView.selected while OS focus is elsewhere.
 SendKey(0x45,true,false);yield return Delay(.2);C("E held in other application does not interact",h.RayInteractions==hits&&S.Modal==FullCaseModal.None);SetForegroundWindow(unityWindow);view.Focus();autoFocus=true;yield return Wait(()=>ForegroundPid()==OwnPid,"return Unity");yield return Delay(.4);C("held E across focus return does not interact",h.RayInteractions==hits);SendKey(0x45,false);yield return Delay(.2);yield return CaptureWindowClick();
 Require("OS return restores cursor and control",h.Movement.enabled&&h.Look.enabled&&Cursor.lockState==CursorLockMode.Locked&&!Cursor.visible&&!NativeCursorShowing());C("no camera jump while away",Quaternion.Angle(rot,h.Eye.transform.rotation)<.1f);
 yield return NativeLook(60,0);C("mouse works after OS focus return",Quaternion.Angle(rot,h.Eye.transform.rotation)>.2f);
 yield return Key(0x1B);C("pause works after focus return",S.Paused&&Cursor.visible&&!h.Movement.enabled);yield return Key(0x1B);C("resume works after focus return",!S.Paused&&Cursor.lockState==CursorLockMode.Locked&&!NativeCursorShowing());
 }
 static string NativePoint(){GetCursorPos(out POINT p);return p.x+","+p.y;}
 static IEnumerator CaptureWindowClick(){if(ForegroundPid()!=OwnPid||!GetWindowRect(unityWindow,out RECT r)||r.right-r.left<100)throw new Exception("Refuse click without valid own Game View rectangle");SetCursorPos((r.left+r.right)/2,(r.top+r.bottom)/2);var down=new INPUT{type=0,u=new UNION{mouse=new MOUSEINPUT{flags=2}}};var up=new INPUT{type=0,u=new UNION{mouse=new MOUSEINPUT{flags=4}}};SendInput(1,new[]{down},Marshal.SizeOf(typeof(INPUT)));yield return Delay(.1);SendInput(1,new[]{up},Marshal.SizeOf(typeof(INPUT)));yield return Delay(.2);inputs.Add(new JObject{["kind"]="WINDOWS_SENDINPUT_GAMEVIEW_CAPTURE_CLICK",["point"]=NativePoint()});}
 [StructLayout(LayoutKind.Sequential)]struct RECT{public int left,top,right,bottom;}
 [DllImport("user32.dll")]static extern bool GetWindowRect(IntPtr window,out RECT rect);
 [DllImport("user32.dll")]static extern bool GetCursorPos(out POINT point);
 [DllImport("user32.dll")]static extern bool SetCursorPos(int x,int y);
 static JObject SourceManifest(){var result=new JObject();foreach(string path in new[]{"Assets/ConRoi/Modules/Gameplay/Testing/FullCasePlayHost.cs","Assets/ConRoi/Modules/Gameplay/Editor/FullCaseSandbox.cs","Assets/ConRoi/Modules/Gameplay/Editor/FullCasePlayLauncher.cs","Assets/ConRoi/Modules/Gameplay/Editor/FullCaseInputFocusChecks.cs","Tools/Gameplay/FullCaseFocusProbe.ps1"}){using(var hash=System.Security.Cryptography.SHA256.Create())result[path]=BitConverter.ToString(hash.ComputeHash(File.ReadAllBytes(path))).Replace("-","").ToLowerInvariant();}return result;}
 [StructLayout(LayoutKind.Sequential)]struct POINT{public int x,y;}
 [StructLayout(LayoutKind.Sequential)]struct CURSORINFO{public int size,flags;public IntPtr cursor;public POINT point;}
 [StructLayout(LayoutKind.Sequential)]struct MOUSEINPUT{public int dx,dy;public uint data,flags,time;public UIntPtr extra;}
 [StructLayout(LayoutKind.Sequential)]struct KEYINPUT{public ushort vk,scan;public uint flags,time;public UIntPtr extra;}
 [StructLayout(LayoutKind.Explicit)]struct UNION{[FieldOffset(0)]public MOUSEINPUT mouse;[FieldOffset(0)]public KEYINPUT key;}
 [StructLayout(LayoutKind.Sequential)]struct INPUT{public uint type;public UNION u;}
 delegate bool WindowCallback(IntPtr window,IntPtr parameter);
 [DllImport("user32.dll")]static extern bool EnumWindows(WindowCallback callback,IntPtr parameter);
 [DllImport("user32.dll",CharSet=CharSet.Unicode)]static extern int GetWindowText(IntPtr window,System.Text.StringBuilder text,int count);
 [DllImport("user32.dll")]static extern short GetAsyncKeyState(int key);
 [DllImport("user32.dll")]static extern uint MapVirtualKey(uint code,uint map);
 static string WindowTitle(IntPtr window){var b=new System.Text.StringBuilder(512);GetWindowText(window,b,512);return b.ToString();}
 static IntPtr FindGameWindow(){IntPtr found=IntPtr.Zero;EnumWindows((window,p)=>{GetWindowThreadProcessId(window,out uint pid);if(pid==OwnPid){var text=new System.Text.StringBuilder(512);GetWindowText(window,text,512);if(text.ToString().Contains("ConRoi TEST ONLY Game View")||text.ToString()=="Game")found=window;}return true;},IntPtr.Zero);return found;}
 [DllImport("user32.dll")]static extern uint SendInput(uint count,INPUT[] inputs,int size);
 [DllImport("user32.dll")]static extern bool GetCursorInfo(ref CURSORINFO info);
 [DllImport("user32.dll")]static extern IntPtr GetForegroundWindow();
 [DllImport("user32.dll")]static extern bool SetForegroundWindow(IntPtr window);
 [DllImport("user32.dll")]static extern uint GetWindowThreadProcessId(IntPtr window,out uint pid);
 [DllImport("user32.dll")]static extern bool ShowWindow(IntPtr window,int command);
 [DllImport("user32.dll")]static extern bool PostMessage(IntPtr window,uint message,IntPtr wparam,IntPtr lparam);
 static bool NativeCursorShowing(){var info=new CURSORINFO{size=Marshal.SizeOf(typeof(CURSORINFO))};return GetCursorInfo(ref info)&&(info.flags&1)!=0;}
 static uint ForegroundPid(){GetWindowThreadProcessId(GetForegroundWindow(),out uint pid);return pid;}
}}
