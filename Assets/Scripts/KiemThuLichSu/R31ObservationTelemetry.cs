#if UNITY_EDITOR || CON_ROI_BUILD_REVIEW
using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

// Passive, explicit diagnostic only. Never added to a scene or ordinary play.
[DefaultExecutionOrder(21001)]
public sealed class R31ObservationTelemetry : MonoBehaviour {
 DieuPhoiHanhTrinh s; Camera eye; MoDauBenHoR23 opening; StreamWriter log,input; int renders;
 [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)] static void Boot(){
  var args=Environment.GetCommandLineArgs();int i=Array.IndexOf(args,"--r31-trace");if(i<0)return;
  var story=FindAnyObjectByType<DieuPhoiHanhTrinh>();var t=story.gameObject.AddComponent<R31ObservationTelemetry>();
  Directory.CreateDirectory(args[i+1]);t.log=new StreamWriter(Path.Combine(args[i+1],"observation.jsonl")){AutoFlush=true};
  t.input=new StreamWriter(Path.Combine(args[i+1],"input.jsonl")){AutoFlush=true};
 }
 void Start(){s=GetComponent<DieuPhoiHanhTrinh>();eye=s.player.GetComponentInChildren<Camera>();opening=GetComponent<MoDauBenHoR23>();RenderPipelineManager.beginCameraRendering+=Rendered;InputSystem.onEvent+=InputEvent;InputSystem.onAfterUpdate+=AfterInput;}
 void InputEvent(InputEventPtr e,InputDevice device){if(device is Keyboard)input?.WriteLine(new JObject{["kind"]="event",["frame"]=Time.frameCount,["eventTime"]=e.time,["device"]=device.name,["enabled"]=device.enabled,["focused"]=Application.isFocused}.ToString(Newtonsoft.Json.Formatting.None));}
 void AfterInput(){var k=Keyboard.current;if(k==null)return;var pi=s.player.GetComponent<TuongTacNguoiChoi>();var action=(InputAction)typeof(TuongTacNguoiChoi).GetField("interact",System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.NonPublic).GetValue(pi);input?.WriteLine(new JObject{["kind"]="afterUpdate",["frame"]=Time.frameCount,["type"]=InputState.currentUpdateType.ToString(),["keyboard"]=k.name,["enabled"]=k.enabled,["E"]=k.eKey.isPressed,["E_down"]=k.eKey.wasPressedThisFrame,["interact_down"]=action!=null&&action.WasPressedThisFrame(),["completed"]=pi.CompletedInteractions,["time"]=Time.unscaledTime}.ToString(Newtonsoft.Json.Formatting.None));}
 void Rendered(ScriptableRenderContext c,Camera cam){if(cam==eye)renders++;}
 static JArray V(Vector3 v)=>new JArray(v.x,v.y,v.z);
 static JArray M(Matrix4x4 m)=>new JArray(Enumerable.Range(0,16).Select(i=>m[i]));
 void LateUpdate(){
  if(!s||s.data.region!=1)return;
  var puppets=new JArray();
  foreach(var p in s.puppets){
   if(!p.gameObject.activeInHierarchy)continue;
   var samples=new JArray();var head=p.dauTransform.position;
   foreach(float h in new[]{0f,.85f,1.50f}){
    var target=head-Vector3.up*h;var d=target-eye.transform.position;
    var hits=Physics.RaycastAll(eye.transform.position,d.normalized,d.magnitude-.03f,~0,QueryTriggerInteraction.Ignore).OrderBy(x=>x.distance);
    samples.Add(new JObject{["heightBelowHead"]=h,["world"]=V(target),["viewport"]=V(eye.WorldToViewportPoint(target)),["inView"]=opening.InView(p.transform,target,16,h==0?.30f:h<1?.20f:.10f),["hits"]=new JArray(hits.Select(x=>new JObject{["name"]=x.collider.name,["parent"]=x.transform.parent?x.transform.parent.name:"",["distance"]=x.distance,["point"]=V(x.point),["excluded"]=x.transform.IsChildOf(s.player.transform)||x.transform.IsChildOf(p.transform)}))});
   }
   puppets.Add(new JObject{["name"]=p.name,["phase"]=p.giaiDoanHienTai.ToString(),["delay"]=p.doTreGD2,["root"]=V(p.transform.position),["delta"]=V(p.GetComponent<BuocChanConRoiR23>().PlanarDelta),["samples"]=samples});
  }
  float entered=(float)typeof(MoDauBenHoR23).GetField("entered",System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Instance).GetValue(opening);
  log?.WriteLine(new JObject{["frame"]=Time.frameCount,["time"]=Time.time,["dt"]=Time.deltaTime,["unscaledTime"]=Time.unscaledTime,["unscaledDt"]=Time.unscaledDeltaTime,["entryUnscaledTime"]=entered,["entryAge"]=Time.unscaledTime-entered,["captureDeltaTime"]=Time.captureDeltaTime,["region"]=s.data.region,["loads"]=s.SuccessfulLoads,["loading"]=s.Loading,["menu"]=s.MenuOpen,["paused"]=s.Paused,["phone"]=s.PhoneOpen,["position"]=V(s.player.transform.position),["input"]=new JArray(s.player.LastMovementInput.x,s.player.LastMovementInput.y),["eye"]=V(eye.transform.position),["rotation"]=V(eye.transform.eulerAngles),["aspect"]=eye.aspect,["fov"]=eye.fieldOfView,["pixelWidth"]=eye.pixelWidth,["pixelHeight"]=eye.pixelHeight,["screen"]=new JArray(Screen.width,Screen.height),["view"]=M(eye.worldToCameraMatrix),["projection"]=M(eye.projectionMatrix),["renders"]=renders,["opportunity"]=opening.Opportunity,["noticed"]=opening.VisibleSeconds,["seenTravel"]=opening.SeenTravel,["witnessed"]=s.data.witnessed[1],["flags"]=s.data.experienceR22Flags,["puppets"]=puppets}.ToString(Newtonsoft.Json.Formatting.None));
 }
 void OnDestroy(){RenderPipelineManager.beginCameraRendering-=Rendered;InputSystem.onEvent-=InputEvent;InputSystem.onAfterUpdate-=AfterInput;log?.Dispose();input?.Dispose();}
}

#endif
