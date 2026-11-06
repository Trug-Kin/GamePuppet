#if UNITY_EDITOR || CON_ROI_BUILD_REVIEW
using System;
using System.IO;
using System.Linq;
using System.Collections;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

// Explicit developer capture only. Never runs in an ordinary player launch.
// The normal-input acceptance walk is recorded separately.
public sealed class MapReviewCaptureV48:MonoBehaviour
{
 DieuPhoiHanhTrinh story;string output;
 [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
 static void Boot()
 {
  var args=Environment.GetCommandLineArgs();int i=Array.IndexOf(args,"--map-review-cameras");
  if(i<0||i+1>=args.Length)return;
  var s=FindAnyObjectByType<DieuPhoiHanhTrinh>();if(!s.GetComponent<TuTheBaoBenCauV48>())return;
  var c=s.gameObject.AddComponent<MapReviewCaptureV48>();c.story=s;c.output=Path.GetFullPath(args[i+1]);Directory.CreateDirectory(c.output);s.SavePathOverride=Path.Combine(c.output,"isolated-review-save.json");
 }
 IEnumerator Start()
 {
  yield return new WaitForSecondsRealtime(2);story.SetMenu(false);story.Restart();
  while(story.Loading)yield return null;
  story.Sleep();
  // Sleep first enters the dream-room transition. Keep story.Update alive
  // until it actually enters the lake; a timer alone captured the hub state.
  float deadline=Time.realtimeSinceStartup+30;
  while(story.data.region!=0||story.Loading)
  {
   if(Time.realtimeSinceStartup>deadline){File.WriteAllText(Path.Combine(output,"capture-error.txt"),"Lake transition did not finish; no valid overview.");Application.Quit(1);yield break;}
   yield return null;
  }
  var args=Environment.GetCommandLineArgs();int regionOption=Array.IndexOf(args,"--map-review-region");
  int targetRegion=regionOption>=0&&regionOption+1<args.Length?int.Parse(args[regionOption+1]):0;
  if(targetRegion<0||targetRegion>3)throw new ArgumentOutOfRangeException("map-review-region");
  for(int next=1;next<=targetRegion;next++)
  {
   story.Wake();while(story.Loading)yield return null;story.Sleep();deadline=Time.realtimeSinceStartup+30;
   while(story.data.region!=next||story.Loading)
   {
    if(Time.realtimeSinceStartup>deadline){File.WriteAllText(Path.Combine(output,"capture-error.txt"),"Review chapter transition did not finish.");Application.Quit(1);yield break;}
    yield return null;
   }
  }
  var mist=story.GetComponent<SuongDaLatV48>();bool clearFog=Array.IndexOf(args,"--review-clear-fog")>=0;if(mist&&clearFog)mist.clearGeometryReview=true;
  yield return new WaitForSecondsRealtime(2);
  var eye=story.player.GetComponentInChildren<Camera>();story.player.enabled=false;story.player.GetComponent<DieuKhienGocNhin>().enabled=false;story.enabled=false;
  foreach(var canvas in FindObjectsByType<Canvas>())canvas.enabled=false;
  var views=new[]{(new Vector3(1000,260,680),new Vector3(680,0,360),"whole-lake-bridge-gate-market"),(new Vector3(770,95,570),new Vector3(660,1,510),"bridge-and-approaches"),(new Vector3(870,90,330),new Vector3(760,8,230),"authored-market-cluster")};
  var custom=Path.Combine(output,"views.json");
  JArray options=File.Exists(custom)?JArray.Parse(File.ReadAllText(custom)):null;
  if(options!=null)views=options.Select(v=>(new Vector3((float)v["eye"][0],(float)v["eye"][1],(float)v["eye"][2]),new Vector3((float)v["lookAt"][0],(float)v["lookAt"][1],(float)v["lookAt"][2]),(string)v["name"])).ToArray();
  bool offscreen=Array.IndexOf(args,"--review-offscreen")>=0;
  var rows=new JArray();int viewIndex=0;
  foreach(var view in views)
  {
   bool scriptedDialogue=(bool?)options?[viewIndex]?["dialogue"]??false;
   bool scriptedLoad=(bool?)options?[viewIndex]?["saveLoad"]??false;
   if(scriptedDialogue&&story.data.region==0){var line=story.completion.NextTalk(0);if(!string.IsNullOrEmpty(line))story.Say(line);}
   if(scriptedLoad){story.Save();story.Load();story.player.enabled=false;story.player.GetComponent<DieuKhienGocNhin>().enabled=false;story.enabled=false;}
   eye.transform.SetPositionAndRotation(view.Item1,Quaternion.LookRotation(view.Item2-view.Item1));
   yield return null;yield return new WaitForSecondsRealtime(1);
   // Batch/offscreen review has no presentation frame; do not wait for one.
   if(offscreen)yield return null;else yield return new WaitForEndOfFrame();
   if(offscreen)
   {
    // Hidden D3D11 windows can return black presentation buffers. This opt-in
    // review uses the same build camera/materials through URP's render request;
    // it supplies geometry images only, never normal-window or UI proof.
    var target=RenderTexture.GetTemporary(1600,900,24,RenderTextureFormat.ARGB32);
    var request=new UniversalRenderPipeline.SingleCameraRequest{destination=target};
    if(!RenderPipeline.SupportsRenderRequest(eye,request))throw new InvalidOperationException("URP single-camera request unsupported");
    RenderPipeline.SubmitRenderRequest(eye,request);
    var previous=RenderTexture.active;RenderTexture.active=target;var pixels=new Texture2D(1600,900,TextureFormat.RGB24,false);
    pixels.ReadPixels(new Rect(0,0,1600,900),0,0);pixels.Apply();File.WriteAllBytes(Path.Combine(output,view.Item3+".png"),pixels.EncodeToPNG());
    RenderTexture.active=previous;Destroy(pixels);RenderTexture.ReleaseTemporary(target);
   }
   else ScreenCapture.CaptureScreenshot(Path.Combine(output,view.Item3+".png"));
   rows.Add(new JObject{["image"]=view.Item3+".png",["eye"]=eye.transform.position.ToString("F4"),["lookAt"]=view.Item2.ToString("F4"),["fov"]=eye.fieldOfView,["farClip"]=eye.farClipPlane,["fog"]=RenderSettings.fog,["fogMode"]=RenderSettings.fogMode.ToString(),["fogDensity"]=RenderSettings.fogDensity,["developer_triggered_dialogue"]=scriptedDialogue,["developer_triggered_save_load"]=scriptedLoad,["successful_loads"]=story.SuccessfulLoads,["bao_contact"]=BaoContactProbeV48.Sample(story.GetComponent<TuTheBaoBenCauV48>())});viewIndex++;yield return new WaitForSecondsRealtime(.5f);
  }
  File.WriteAllText(Path.Combine(output,"method.json"),new JObject{["build"]=Application.version,["region"]=story.data.region,["method"]=offscreen?"Supplemental build-camera images via URP SingleCameraRequest to a render texture, not the presentation window. Camera moved; canvas hidden; optional dialogue/load developer-triggered. Not foreground, UI or traversal evidence.":"Supplemental review poses rendered by the player camera to the game window. Camera moved; chapter transitions use story methods. Canvas UI hidden. Optional dialogue/load are developer-triggered, not OS interaction. Not traversal or foreground input evidence.",["offscreen_render_request"]=offscreen,["normal_walk"]="Required separately; these captures do not imply completion",["developer_fog_disable"]=clearFog,["geometry_materials_lights_changed_by_capture"]=false,["views"]=rows}.ToString());
  Application.Quit();
 }
}

#endif
