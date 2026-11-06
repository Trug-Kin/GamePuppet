#if UNITY_EDITOR || CON_ROI_BUILD_REVIEW
using System;
using System.IO;
using System.Linq;
using System.Collections;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

// Explicit test component only; absent from saved scenes and ordinary launches.
public sealed class R30NativeLifecycleReview:MonoBehaviour {
 DieuPhoiHanhTrinh s;MenuVaoGameV47 entry;string output;JArray checks=new JArray(),errors=new JArray();bool done;
 [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]static void Boot(){
  var args=Environment.GetCommandLineArgs();int i=Array.IndexOf(args,"--r30-native-lifecycle");
  if(Application.isEditor||i<0)return;
  var s=FindAnyObjectByType<DieuPhoiHanhTrinh>();var c=s.gameObject.AddComponent<R30NativeLifecycleReview>();c.output=Path.GetFullPath(args[i+1]);
 }
 void Start(){
  Directory.CreateDirectory(output);s=GetComponent<DieuPhoiHanhTrinh>();entry=GetComponent<MenuVaoGameV47>();
  s.SavePathOverride=Path.Combine(output,"lifecycle-save.json");Application.runInBackground=true;
  InputSystem.settings=Instantiate(InputSystem.settings);InputSystem.settings.updateMode=InputSettings.UpdateMode.ProcessEventsInDynamicUpdate;InputSystem.settings.backgroundBehavior=InputSettings.BackgroundBehavior.IgnoreFocus;
  if(Keyboard.current==null)InputSystem.AddDevice<Keyboard>();
  if(Array.IndexOf(Environment.GetCommandLineArgs(),"--r31-input-device")>=0){var k=InputSystem.AddDevice<Keyboard>("R31ReviewKeyboard");k.MakeCurrent();QualitySettings.vSyncCount=0;Application.targetFrameRate=30;}
  Application.logMessageReceived+=(m,t,k)=>{if(k==LogType.Error||k==LogType.Exception)errors.Add(new JObject{["message"]=m,["trace"]=t});};
  Application.quitting+=()=>File.WriteAllText(Path.Combine(output,"quit-observed.txt"),"Application.quitting callback reached on native test binary.");
  StartCoroutine(Run());StartCoroutine(Timeout());
 }
 IEnumerator Timeout(){yield return new WaitForSecondsRealtime(120);if(!done){Check("timeout",false);Finish();}}
 void Check(string n,bool ok,JToken detail=null){checks.Add(new JObject{["name"]=n,["pass"]=ok,["detail"]=detail});File.WriteAllText(Path.Combine(output,"progress.json"),checks.ToString());Debug.Log("R30_LIFECYCLE "+n+" "+ok);}
 IEnumerator Press(Key key){
  InputSystem.QueueStateEvent(Keyboard.current,new KeyboardState(key));yield return null;yield return null;
  InputSystem.QueueStateEvent(Keyboard.current,new KeyboardState());yield return null;yield return null;
 }
 IEnumerator Shot(string name){yield return new WaitForEndOfFrame();var t=ScreenCapture.CaptureScreenshotAsTexture();File.WriteAllBytes(Path.Combine(output,name+".png"),t.EncodeToPNG());Destroy(t);}
 IEnumerator Run(){
  while(s.Loading)yield return null;yield return null;
  Check("real_loading_gate",GetComponent<ManHinhTaiV44>().Ready);Check("menu_before_NewGame",s.MenuOpen&&Time.timeScale==0);yield return Shot("01-menu");
  yield return Press(UnityEngine.InputSystem.Key.Enter);Check("Enter_NewGame",!s.MenuOpen&&s.data.region==-1&&s.player.enabled);yield return Shot("02-new-game");
  var p=s.player.transform.position;
  yield return Press(UnityEngine.InputSystem.Key.Escape);Check("Escape_pause",s.Paused&&!s.player.enabled&&Time.timeScale==0);yield return Shot("03-paused");
  yield return new WaitForSecondsRealtime(.4f);Check("paused_position_held",Vector3.Distance(p,s.player.transform.position)<.02f);
  yield return Press(UnityEngine.InputSystem.Key.Escape);Check("Escape_resume",!s.Paused&&s.player.enabled&&Time.timeScale==1);
  yield return Press(UnityEngine.InputSystem.Key.F5);Check("F5_file_written",File.Exists(s.SavePathOverride));p=s.player.transform.position;
  s.player.MovementFilter=r=>new Vector2(.35f,0);yield return new WaitForSeconds(.5f);s.player.MovementFilter=r=>Vector2.zero;yield return null;
  Check("controller_moved_after_save",Vector3.Distance(p,s.player.transform.position)>.1f);
  int loads=s.SuccessfulLoads;yield return Press(UnityEngine.InputSystem.Key.F6);
  Check("F6_restored_saved_position",s.SuccessfulLoads==loads+1&&Vector3.Distance(p,s.player.transform.position)<.05f);yield return Shot("04-loaded");
  Check("r30_staging_enabled",GetComponent<TraiNghiemTruongR28>().stagedSchoolR30);
  var roots=new[]{s.schoolChapter.environment,s.slopeChapter.environment};
  foreach(var root in roots){var all=root.GetComponentsInChildren<MeshFilter>(true);var meshes=all.Where(m=>IntendedVisible(m.transform,root.transform)).ToArray();Check(root.name+"_meshes_present",meshes.Length>0&&meshes.All(m=>m.sharedMesh&&m.sharedMesh.vertexCount>0),new JObject{["intended_visible"]=meshes.Length,["excluded_disabled_children"]=all.Length-meshes.Length});}
  Check("save_isolated",Path.GetFullPath(s.SavePathOverride).StartsWith(output,StringComparison.OrdinalIgnoreCase));
  Finish();
 }
 static bool IntendedVisible(Transform t,Transform root){while(t!=root){if(!t.gameObject.activeSelf)return false;t=t.parent;}return true;}
 void Finish(){if(done)return;done=true;bool pass=errors.Count==0&&checks.All(c=>(bool)c["pass"]);
  File.WriteAllText(Path.Combine(output,"result.json"),new JObject{["complete"]=pass,["scene"]=gameObject.scene.path,["checks"]=checks,["errors"]=errors,["scope"]="Native lifecycle automation: actual menu, queued keyboard Enter/Esc/F5/F6 consumed by normal Dynamic InputSystem and original Update/controller; exit via Application.Quit test API. No physical OS input or quit-button click claim."}.ToString());Application.Quit(pass?0:1);
 }
}

#endif
