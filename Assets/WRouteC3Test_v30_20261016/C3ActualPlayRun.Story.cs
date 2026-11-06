#if UNITY_EDITOR || CON_ROI_BUILD_REVIEW
using System.Collections;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
public sealed partial class C3ActualPlayRun
{
 public static IEnumerator VerifyStory(DieuPhoiHanhTrinh story)
 {
  bool oldBackground=Application.runInBackground;Application.runInBackground=true;
  var previousInputBackground=InputSystem.settings.backgroundBehavior;var previousEditorInput=InputSystem.settings.editorInputBehaviorInPlayMode;
  InputSystem.settings.backgroundBehavior=InputSettings.BackgroundBehavior.IgnoreFocus;InputSystem.settings.editorInputBehaviorInPlayMode=InputSettings.EditorInputBehaviorInPlayMode.AllDeviceInputAlwaysGoesToGameView;
  string folder=VerificationFolder("functional");Directory.CreateDirectory(folder);story.SavePathOverride=Path.GetFullPath(folder+"/test-save.json");
  var cases=new JArray();var keyboard=InputSystem.AddDevice<Keyboard>("StoryVerificationKeyboard");
  void Check(string name,bool pass){cases.Add(new JObject{["case"]=name,["status"]=pass?"PASS":"FAIL",["region"]=story.data.region,["position"]=story.player.transform.position.ToString("F4")});File.WriteAllText(folder+"/cases_in_progress.json",cases.ToString());}
  IEnumerator KeyPress(Key key){InputSystem.QueueStateEvent(keyboard,new KeyboardState(key));yield return null;yield return null;if(key==Key.W)yield return new WaitForSeconds(Time.fixedDeltaTime*2);InputSystem.QueueStateEvent(keyboard,new KeyboardState());yield return null;yield return null;}
  IEnumerator Use(TuongTacHoCho.Kind kind)
  {
   var target=Object.FindObjectsByType<TuongTacHoCho>(FindObjectsInactive.Exclude).FirstOrDefault(i=>i.kind==kind&&i.GetComponent<Collider>());if(!target){Check("missing_active_interaction_"+kind,false);yield break;}var collider=target.GetComponent<Collider>();
   var eye=story.player.GetComponentInChildren<Camera>();var p=collider.bounds.center+Vector3.back*1.7f;p.y=kind==TuongTacHoCho.Kind.Wake?collider.bounds.center.y-.16f:story.player.transform.position.y;
   var mouse=story.player.GetComponentInChildren<DieuKhienGocNhin>();if(!mouse)mouse=story.player.GetComponent<DieuKhienGocNhin>();bool was=mouse&&mouse.enabled;if(mouse)mouse.enabled=false;
   story.Place(p,Quaternion.identity);eye.transform.rotation=Quaternion.LookRotation(collider.bounds.center-eye.transform.position);yield return null;yield return null;
   var ray=new Ray(eye.transform.position,eye.transform.forward);string blocker=Physics.Raycast(ray,out var hit,3,~0,QueryTriggerInteraction.Ignore)?hit.collider.name:"none";
   File.AppendAllText(folder+"/interaction_rays.txt",kind+" -> "+blocker+" target="+collider.bounds+"\n");
   yield return KeyPress(Key.E);if(mouse)mouse.enabled=was;
  }
  yield return new WaitForSeconds(.5f);
  IEnumerator Capture(string name,Vector3 p,Vector3 target)
  {
    var go=new GameObject("Temporary_Story_Review_Camera");var camera=go.AddComponent<Camera>();camera.CopyFrom(story.player.GetComponentInChildren<Camera>());camera.enabled=false;camera.transform.SetPositionAndRotation(p,Quaternion.LookRotation(target-p));var rt=new RenderTexture(1280,800,24);camera.targetTexture=rt;camera.Render();yield return null;camera.Render();var previous=RenderTexture.active;RenderTexture.active=rt;var image=new Texture2D(1280,800,TextureFormat.RGB24,false);image.ReadPixels(new Rect(0,0,1280,800),0,0);image.Apply();File.WriteAllBytes(folder+"/"+name+".png",image.EncodeToPNG());Object.Destroy(image);RenderTexture.active=previous;camera.targetTexture=null;rt.Release();Object.Destroy(rt);Object.Destroy(go);
  }
  yield return Capture("hub",new Vector3(-35,1.65f,-36.8f),new Vector3(-35,.8f,-33.4f));
  Check("hub_controller_dimensions",story.player.controller.height==1.8f&&story.player.controller.radius==.3f);
  yield return KeyPress(Key.P);Check("phone_input_system_open",story.PhoneOpen);yield return KeyPress(Key.Digit2);Check("phone_call_points",story.data.actionPoints==3&&story.data.interactions==1);yield return KeyPress(Key.Digit2);Check("phone_call_no_farm",story.data.interactions==1);yield return KeyPress(Key.P);
  yield return Use(TuongTacHoCho.Kind.Bed);Check("E_bed_enters_lake",story.data.region==0);
  yield return Capture("lake",story.lakeSpawn.position+Vector3.up*.7f,story.traces[0].position);
  yield return new WaitForSeconds(.5f);Check("lake_spawn_supported",story.player.controller.isGrounded&&Mathf.Abs(story.player.transform.position.y-story.lakeSpawn.position.y)<.1f);
  var reflection=story.GetComponent<PhanChieuMatHo>();Check("lake_reflection_rendered",reflection&&reflection.HasRendered);
  var initial=story.player.transform.position;yield return KeyPress(Key.W);Check("W_real_actions_move",(story.player.transform.position-initial).sqrMagnitude>.0001f&&!story.player.TestInputHookActive&&Mathf.Abs(story.player.transform.position.y-initial.y)<.2f);
  Check("GD1_mimic_executed",story.puppets.Any(ai=>ai.gameObject.activeInHierarchy&&ai.HasWitnessedPhase));
  yield return Use(TuongTacHoCho.Kind.Wake);Check("optional_lake_trace_bypass",story.data.region== -1&&story.data.nextRegion==1&&!story.data.traces[0]);
  yield return Use(TuongTacHoCho.Kind.Bed);Check("market_unlocked_without_phone",story.data.region==1);var activePuppets=story.puppets.Where(p=>p.gameObject.activeInHierarchy).ToArray();Check("GD2_delay_configured",activePuppets.Length==4&&activePuppets.All(p=>Mathf.Abs(p.doTreGD2-.5f)<.001f));
  yield return new WaitForSeconds(.5f);Check("market_spawn_road_height",story.player.controller.isGrounded&&story.player.transform.position.y<3&&Mathf.Abs(story.player.transform.position.y-story.marketSpawn.position.y)<.12f);
  if(activePuppets.Length>0)
  {
   var puppet=activePuppets[0];var original=puppet.transform.position;InputSystem.QueueStateEvent(keyboard,new KeyboardState(Key.W));yield return new WaitForSeconds(.12f);var early=Vector3.Distance(original,puppet.transform.position);InputSystem.QueueStateEvent(keyboard,new KeyboardState());yield return new WaitForSeconds(.65f);Check("GD2_delayed_motion_executed",early<.05f&&Vector3.Distance(original,puppet.transform.position)>.05f&&puppet.HasWitnessedPhase);
  }
  var trace=story.traces[1];var q=trace.position+Vector3.back;q.y=story.ghosts[1].position.y+.94f;story.Place(q,Quaternion.identity);yield return null;yield return null;Check("market_trace_optional_discovery",story.data.traces[1]&&story.data.awareness==1);
  yield return Capture("market",story.ghosts[1].position+new Vector3(1.8f,1.6f,-3),story.ghosts[1].position+Vector3.up);
  story.Save();var saved=story.player.transform.position;story.data.awareness=0;story.Load();yield return null;Check("save_load_state_and_pose",story.data.awareness==1&&Vector3.Distance(saved,story.player.transform.position)<.1f&&story.data.traces[1]);
  yield return Use(TuongTacHoCho.Kind.Wake);Check("market_returns_hub_no_invented_ending",story.data.region== -1&&story.data.nextRegion==2);
  yield return KeyPress(Key.Escape);Check("pause_input",story.Paused&&Time.timeScale==0);yield return KeyPress(Key.Escape);Check("resume_input",!story.Paused&&Time.timeScale==1&&story.player.enabled);
  // A second branch uses the same regional runtime, preserving the bypass branch above.
  story.data.nextRegion=0;story.Sleep();yield return new WaitForSeconds(.3f);
  var lakeTrace=story.traces[0];var nearTrace=lakeTrace.position+Vector3.back;nearTrace.y=story.ghosts[0].position.y+.94f;story.Place(nearTrace,Quaternion.identity);yield return new WaitForSeconds(.3f);int awareness=story.data.awareness;
  Check("calendar_discovery",story.data.traces[0]&&awareness==2);yield return new WaitForSeconds(.3f);Check("calendar_not_counted_twice",story.data.awareness==awareness);
  yield return Use(TuongTacHoCho.Kind.Bao);Check("Bao_E_dialogue",story.Dialogue.Contains("Ông Bảo"));
  story.Wake();story.data.nextRegion=0;float fallbackStart=Time.realtimeSinceStartup;
  while(story.data.region<0&&Time.realtimeSinceStartup-fallbackStart<92)yield return null;
  Check("90_second_sleep_fallback",story.data.region==0&&Time.realtimeSinceStartup-fallbackStart>=89);
  File.WriteAllText(folder+"/result.json",new JObject{["scene"]=story.gameObject.scene.path,["status"]=cases.Any(c=>(string)c["status"]=="FAIL")?"EXECUTED_FAILURES":"FOCUSED_CASES_PASS",["input"]="Virtual Input System Keyboard -> actions; E uses PlayerInteraction raycast",["OS_input"]="NOT_VERIFIED",["traversal"]="NOT_RUN_STORY_FIXTURE_PLACEMENT",["cases"]=cases}.ToString());
  InputSystem.RemoveDevice(keyboard);InputSystem.settings.backgroundBehavior=previousInputBackground;InputSystem.settings.editorInputBehaviorInPlayMode=previousEditorInput;Application.runInBackground=oldBackground;
#if UNITY_EDITOR
  UnityEditor.EditorApplication.isPlaying=false;
#else
  Application.Quit(cases.Any(c=>(string)c["status"]=="FAIL")?2:0);
#endif
 }
}

#endif
