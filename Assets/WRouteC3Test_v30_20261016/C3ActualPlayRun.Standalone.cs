#if UNITY_EDITOR || CON_ROI_BUILD_REVIEW
using System;
using System.IO;
using Newtonsoft.Json.Linq;
using UnityEngine;

public sealed partial class C3ActualPlayRun
{
 public TextAsset bridgeRecord;
 public static string VerificationOutput;
 static string VerificationFolder(string suite)
 {
  string root=string.IsNullOrEmpty(VerificationOutput)?"Docs/Reviews/lake_gate_market_current/v36":VerificationOutput;
  return Path.GetFullPath(Path.Combine(root,suite+"_"+DateTime.UtcNow.ToString("yyyyMMdd_HHmmss")));
 }
 [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
 static void StandaloneVerification()
 {
  if(Application.isEditor)return;
  var args=Environment.GetCommandLineArgs();int i=Array.IndexOf(args,"--verify-suite");if(i<0||i+1>=args.Length)return;
  int output=Array.IndexOf(args,"--verification-output");if(output<0||output+1>=args.Length)throw new Exception("Verification requires an isolated output directory");
  VerificationOutput=Path.GetFullPath(args[output+1]);Directory.CreateDirectory(VerificationOutput);
  var player=UnityEngine.Object.FindAnyObjectByType<DieuKhienDiChuyen>();var story=UnityEngine.Object.FindAnyObjectByType<DieuPhoiHanhTrinh>();story.SavePathOverride=Path.Combine(VerificationOutput,"fixture-save.json");
  string suite=args[i+1];
  if(suite=="os"){player.gameObject.AddComponent<StandaloneOSProbe>();return;}
  if(story.GetComponent<DieuChinhHanhTrinhV43>())player.gameObject.AddComponent<VerificationInputIsolationV43>().Begin(VerificationOutput);
  if(suite=="story"){story.StartCoroutine(VerifyStory(story));return;}
  if(suite=="route"){story.Sleep();story.market.SetActive(true);story.enabled=false;}
  var runner=player.gameObject.AddComponent<C3ActualPlayRun>();runner.walkthroughRun=suite=="walkthrough";runner.ongDaoRun=suite=="bridge";
  if(suite=="feedback-v48"){runner.walkthroughRun=true;runner.journeySuite=suite;}
  if(suite=="branches"||suite=="stability"||suite=="experience"||suite=="boundary"||suite=="acting"||suite=="hitches")runner.journeySuite=suite;
  if(suite.StartsWith("acceptance-"))runner.journeySuite=suite;
  if(suite=="gdd")runner.journeySuite=suite;
  runner.regionalRoute=Resources.Load<TextAsset>("VerificationV36/route");runner.bridgeRecord=Resources.Load<TextAsset>("VerificationV36/bridge");
  File.WriteAllText(Path.Combine(VerificationOutput,"startup.json"),new JObject{["suite"]=suite,["scene"]=player.gameObject.scene.path,["editor"]=Application.isEditor,["data_path"]=Application.dataPath,["route_packaged"]=runner.regionalRoute!=null,["bridge_packaged"]=runner.bridgeRecord!=null}.ToString());
 }
}

public sealed class StandaloneOSProbe : MonoBehaviour
{
 StreamWriter file;DieuKhienDiChuyen player;DieuPhoiHanhTrinh story;float until;bool aimed;
 void Start(){player=GetComponent<DieuKhienDiChuyen>();story=UnityEngine.Object.FindAnyObjectByType<DieuPhoiHanhTrinh>();Application.runInBackground=true;until=Time.realtimeSinceStartup+45;file=new StreamWriter(Path.Combine(C3ActualPlayRun.VerificationOutput,"os-frames.jsonl")){AutoFlush=true};}
 void LateUpdate()
 {
  if(!aimed&&!story.Loading&&story.GetComponent<ManHinhTaiV44>())
  {
   aimed=true;var latch=story.hub.GetComponentInChildren<TuongTacVuonV41>();var eye=GetComponentInChildren<Camera>();var d=latch.transform.position-eye.transform.position;d.y=0;transform.rotation=Quaternion.LookRotation(d);eye.transform.LookAt(latch.transform.position);
   var mouse=GetComponent<DieuKhienGocNhin>();if(!mouse)mouse=GetComponentInChildren<DieuKhienGocNhin>();if(mouse)mouse.RestorePitch(eye.transform.localEulerAngles.x);
   File.WriteAllText(Path.Combine(C3ActualPlayRun.VerificationOutput,"os-fixture.json"),new JObject{["utc"]=DateTime.UtcNow.ToString("o"),["purpose"]="One camera aim at garden latch before native E; no teleport, no virtual device, subsequent input through Windows",["target"]=latch.transform.position.ToString("F5")}.ToString());
  }
  var keyboard=UnityEngine.InputSystem.Keyboard.current;var interaction=GetComponent<TuongTacNguoiChoi>();
  file.WriteLine(new JObject{["kind"]="action_evidence",["utc"]=DateTime.UtcNow.ToString("o"),["t"]=Time.realtimeSinceStartup,["frame"]=Time.frameCount,["jump_count"]=player.JumpCount,["jump_velocity"]=player.VerticalSpeed,["grounded"]=player.controller.isGrounded,["completed_interactions"]=interaction.CompletedInteractions,["target"]=interaction.Target?.GetType().Name,["successful_loads"]=story.SuccessfulLoads,["lan_mask"]=story.data.lanVisitMask,["E_pressed"]=keyboard!=null&&keyboard.eKey.wasPressedThisFrame,["Space_pressed"]=keyboard!=null&&keyboard.spaceKey.wasPressedThisFrame,["F6_pressed"]=keyboard!=null&&keyboard.f6Key.wasPressedThisFrame,["loading"]=story.Loading}.ToString(Newtonsoft.Json.Formatting.None));
  file.WriteLine(new JObject{["t"]=Time.realtimeSinceStartup,["focused"]=Application.isFocused,["input"]=new JArray(player.LastMovementInput.x,player.LastMovementInput.y),["look"]=new JArray(player.LookInput.x,player.LookInput.y),["control_enabled"]=player.enabled&&player.GameplayActionsEnabled,["keyboard_native"]=UnityEngine.InputSystem.Keyboard.current!=null&&UnityEngine.InputSystem.Keyboard.current.native,["keyboard_name"]=UnityEngine.InputSystem.Keyboard.current?.name,["device_count"]=UnityEngine.InputSystem.InputSystem.devices.Count,["pos"]=new JArray(transform.position.x,transform.position.y,transform.position.z),["yaw"]=transform.eulerAngles.y,["pitch"]=GetComponentInChildren<Camera>().transform.localEulerAngles.x,["region"]=story.data.region,["phone"]=story.PhoneOpen,["paused"]=story.Paused,["hook"]=player.TestInputHookActive}.ToString(Newtonsoft.Json.Formatting.None));
  if(Time.realtimeSinceStartup>until){file.Dispose();file=null;Application.Quit();}
 }
 void OnDestroy(){file?.Dispose();}
}

#endif
