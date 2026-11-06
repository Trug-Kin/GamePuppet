// Quan sát trạng thái hình ảnh; không sửa light/material hay ép readback, F8 giữ ảnh và trạng thái.
// Component trong scene r92: Lake_Market_Story_GDD21.
// Reference public để đối chiếu Inspector (có nhánh tùy chọn): story.
// Code tự tìm/tạo: Camera, Bloom, ColorAdjustments, PhucHoiTaiBienR21, TuTheBaoBenCauV48, CanhQuanChoV48.
// Thứ tự chạy khai báo: 30000; giữ quan hệ khởi tạo/pose với các owner khác.
using System;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using Newtonsoft.Json.Linq;

// Observation only: no render requests, readbacks, lights, materials or gameplay writes.
// Rotating files retain recent state automatically; F8 preserves it and a screen image.
[DefaultExecutionOrder(30000)]
[UnityEngine.Scripting.APIUpdating.MovedFrom(true, null, "Assembly-CSharp", "VisualStateV48")]
public sealed class GhiTrangThaiHinhAnhV48 : MonoBehaviour
{
 public DieuPhoiHanhTrinh story;
 Camera eye; Renderer[] renderers; Light[] lights; Volume[] volumes;
 string folder; StreamWriter writer; float next; int region=-99,loads=-1,chunk,rows;
 bool failed;
 static string PathOf(Transform t)=>t.parent?PathOf(t.parent)+"/"+t.name:t.name;
 void Start()
 {
  try{
   eye=story.player.GetComponentInChildren<Camera>();
   renderers=FindObjectsByType<Renderer>(FindObjectsInactive.Include);
   lights=FindObjectsByType<Light>(FindObjectsInactive.Include);
   volumes=FindObjectsByType<Volume>(FindObjectsInactive.Include);
   folder=Path.Combine(Application.persistentDataPath,"visual-v48",DateTime.UtcNow.ToString("yyyyMMdd_HHmmss_fff"));Directory.CreateDirectory(folder);
   File.WriteAllText(Path.Combine(folder,"session.json"),new JObject{["version"]=Application.version,["gpu"]=SystemInfo.graphicsDeviceName,["api"]=SystemInfo.graphicsDeviceType.ToString(),["os"]=SystemInfo.operatingSystem,["purpose"]="State samples, not proof of absence of whiteout. F8 optional preservation; no gameplay changes."}.ToString());
  }catch(Exception e){Fail(e);}
 }
 void Fail(Exception e){failed=true;writer?.Dispose();Debug.LogWarning("VisualStateV48 logging disabled: "+e);}
 void LateUpdate()
 {
  if(failed||!eye)return;
  try{
   bool mark=Keyboard.current!=null&&Keyboard.current.f8Key.wasPressedThisFrame;
   bool changed=region!=story.data.region||loads!=story.SuccessfulLoads;
   if(!mark&&!changed&&Time.unscaledTime<next)return;
   next=Time.unscaledTime+2;region=story.data.region;loads=story.SuccessfulLoads;
   var sample=Snapshot(mark?"F8":changed?"region_or_load":"periodic");
   if(writer==null||rows>=150){writer?.Dispose();writer=new StreamWriter(Path.Combine(folder,"recent-"+(chunk++%4)+".jsonl"),false){AutoFlush=true};rows=0;}
   writer.WriteLine(sample.ToString(Newtonsoft.Json.Formatting.None));rows++;
   if(mark){var name="marked-"+DateTime.UtcNow.ToString("HHmmss_fff");File.WriteAllText(Path.Combine(folder,name+".json"),sample.ToString());ScreenCapture.CaptureScreenshot(Path.Combine(folder,name+".png"));foreach(var f in Directory.GetFiles(folder,"recent-*.jsonl"))File.Copy(f,Path.Combine(folder,name+"-"+Path.GetFileName(f)),true);}
  }catch(Exception e){Fail(e);}
 }
 JObject Snapshot(string reason)
 {
  // Before the first render (and between reflection cameras) the global stack
  // may not exist. Retain state then, without treating missing data as zero.
  var stack=eye.GetUniversalAdditionalCameraData().volumeStack??VolumeManager.instance.stack;
  var bloom=stack?.GetComponent<Bloom>();var grade=stack?.GetComponent<ColorAdjustments>();
  var planes=GeometryUtility.CalculateFrustumPlanes(eye);
  var emissive=new JArray();var block=new MaterialPropertyBlock();
  foreach(var r in renderers){if(!r||!r.enabled||!r.gameObject.activeInHierarchy||!GeometryUtility.TestPlanesAABB(planes,r.bounds))continue;
   foreach(var m in r.sharedMaterials){if(!m||!m.HasProperty("_EmissionColor"))continue;var color=m.GetColor("_EmissionColor");r.GetPropertyBlock(block);if(block.HasColor("_EmissionColor"))color=block.GetColor("_EmissionColor");if(color.maxColorComponent>1)emissive.Add(new JObject{["path"]=PathOf(r.transform),["material"]=m.name,["emission"]=color.ToString("F4"),["keyword"]=m.IsKeywordEnabled("_EMISSION"),["bounds"]=r.bounds.ToString("F3")});}
  }
  var cameraData=eye.GetUniversalAdditionalCameraData();
  return new JObject{
   ["utc"]=DateTime.UtcNow.ToString("o"),["time"]=Time.realtimeSinceStartupAsDouble,["frame"]=Time.frameCount,["reason"]=reason,["focused"]=Application.isFocused,
   ["region"]=region,["loads"]=loads,["dreamHub"]=story.DreamHub,["loading"]=story.Loading,["paused"]=story.Paused,["menu"]=story.MenuOpen,["phone"]=story.PhoneOpen,
   ["boundaryRecovery"]=story.GetComponent<PhucHoiTaiBienR21>()?new JObject{["recoveries"]=story.GetComponent<PhucHoiTaiBienR21>().Recoveries,["reason"]=story.GetComponent<PhucHoiTaiBienR21>().LastReason}:null,
   ["player"]=story.player.transform.position.ToString("F5"),["cameraPosition"]=eye.transform.position.ToString("F5"),["cameraEuler"]=eye.transform.eulerAngles.ToString("F5"),["cameraEnabled"]=eye.enabled,["cameraTarget"]=eye.targetTexture?eye.targetTexture.name:"screen",["hdr"]=eye.allowHDR,["postProcessing"]=cameraData.renderPostProcessing,["volumeMask"]=cameraData.volumeLayerMask.value,["far"]=eye.farClipPlane,
   ["fog"]=RenderSettings.fog,["fogColor"]=RenderSettings.fogColor.ToString("F4"),["fogDensity"]=RenderSettings.fogDensity,["fogStart"]=RenderSettings.fogStartDistance,["fogEnd"]=RenderSettings.fogEndDistance,["ambientIntensity"]=RenderSettings.ambientIntensity,["skybox"]=RenderSettings.skybox?RenderSettings.skybox.name:"null",
   ["resolvedBloom"]=bloom?new JObject{["active"]=bloom.active,["intensity"]=bloom.intensity.value,["threshold"]=bloom.threshold.value,["scatter"]=bloom.scatter.value,["clamp"]=bloom.clamp.value}:null,["resolvedExposure"]=grade?new JValue(grade.postExposure.value):JValue.CreateNull(),
   ["volumes"]=new JArray(volumes.Where(v=>v).Select(v=>new JObject{["path"]=PathOf(v.transform),["enabled"]=v.isActiveAndEnabled,["weight"]=v.weight,["priority"]=v.priority,["global"]=v.isGlobal,["profile"]=v.sharedProfile?v.sharedProfile.name:"null",["components"]=v.sharedProfile?new JArray(v.sharedProfile.components.Select(c=>JsonUtility.ToJson(c))):new JArray()})),
   ["lights"]=new JArray(lights.Where(l=>l&&l.isActiveAndEnabled&&(l.type==LightType.Directional||Vector3.Distance(l.transform.position,eye.transform.position)<l.range+30)).Select(l=>new JObject{["path"]=PathOf(l.transform),["type"]=l.type.ToString(),["intensity"]=l.intensity,["range"]=l.range,["position"]=l.transform.position.ToString("F3"),["color"]=l.color.ToString("F3")})),
   ["visibleHdrEmission"]=emissive,
   ["baoRenderedContact"]=BaoContactProbeV48.Sample(story.GetComponent<TuTheBaoBenCauV48>()),
   ["restoredMarket"]=new JArray(renderers.Where(r=>r&&r.transform.root.name=="R11_Restored_Source_Market").Select(r=>new JObject{["mesh"]=r.name,["enabled"]=r.enabled,["active"]=r.gameObject.activeInHierarchy,["inFrustum"]=GeometryUtility.TestPlanesAABB(planes,r.bounds),["bounds"]=r.bounds.ToString("F3")})),
   ["baoBridge"]=story.GetComponent<TuTheBaoBenCauV48>()?new JObject{["position"]=story.GetComponent<TuTheBaoBenCauV48>().actor.position.ToString("F4"),["footHeightError"]=story.GetComponent<TuTheBaoBenCauV48>().FootError,["forearmCenterAboveRail"]=story.GetComponent<TuTheBaoBenCauV48>().ForearmRailClearance,["layoutRevision"]=story.data.mapLayoutRevision}:null,
   ["cityRenderers"]=new JObject{["authored"]=renderers.Count(r=>r&&(r.name.StartsWith("Surface_")||r.name.StartsWith("Context_"))),["activeEnabled"]=renderers.Count(r=>r&&r.enabled&&r.gameObject.activeInHierarchy&&(r.name.StartsWith("Surface_")||r.name.StartsWith("Context_"))),["inactivePaths"]=new JArray(renderers.Where(r=>r&&(!r.enabled||!r.gameObject.activeInHierarchy)&&(r.name.StartsWith("Surface_")||r.name.StartsWith("Context_"))).Select(r=>PathOf(r.transform)))},
   ["particles"]=new JArray(FindObjectsByType<ParticleSystem>().Select(p=>new JObject{["path"]=PathOf(p.transform),["playing"]=p.isPlaying,["count"]=p.particleCount})),
   ["ui"]=new JArray(FindObjectsByType<Canvas>().Select(c=>new JObject{["name"]=c.name,["mode"]=c.renderMode.ToString(),["order"]=c.sortingOrder,["images"]=new JArray(c.GetComponentsInChildren<Image>().Select(i=>new JObject{["name"]=i.name,["color"]=i.color.ToString("F4"),["material"]=i.material?i.material.name:"null"}))})),
   ["puppets"]=new JArray(story.puppets.Concat(new[]{story.schoolChapter.puppet,story.slopeChapter.puppet}).Where(p=>p&&p.gameObject.activeInHierarchy).Select(p=>new JObject{["name"]=p.name,["phase"]=p.giaiDoanHienTai.ToString(),["position"]=p.transform.position.ToString("F3"),["headViewport"]=eye.WorldToViewportPoint(p.dauTransform.position).ToString("F4"),["witnessedTelemetry"]=p.HasWitnessedPhase})),
   ["marketScenery"]=new JArray(story.GetComponent<CanhQuanChoV48>().scenery.Select(g=>new JObject{["name"]=g.name,["active"]=g.activeInHierarchy}))
  };
 }
 void OnDestroy(){writer?.Dispose();}
}
