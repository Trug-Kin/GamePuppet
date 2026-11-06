// Công cụ ablation hình ảnh cho developer, chỉ bật khi có --visual-isolation; không thuộc lượt chơi thường.
// Không thấy attachment trực tiếp trong YAML scene; xem consumer AddComponent và prefab trong bảng tra cứu.
// Reference public để đối chiếu Inspector (có nhánh tùy chọn): story.
// Code tự tìm/tạo: Camera, DieuKhienGocNhin, Volume.
using System;
using System.IO;
using System.Linq;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Newtonsoft.Json.Linq;

// Developer-only ablation, explicitly opt-in. Never runs on an ordinary launch.
[UnityEngine.Scripting.APIUpdating.MovedFrom(true, null, "Assembly-CSharp", "VisualIsolationV48")]
public sealed class CoLapHinhAnhV48:MonoBehaviour
{
 public DieuPhoiHanhTrinh story;string folder;bool running;
 void Start(){var args=Environment.GetCommandLineArgs();int i=Array.IndexOf(args,"--visual-isolation");if(i<0){enabled=false;return;}folder=args[i+1];Directory.CreateDirectory(folder);}
 void Update(){if(!running&&story.data.region==0&&Keyboard.current!=null&&Keyboard.current.f9Key.wasPressedThisFrame){running=true;StartCoroutine(Isolate());}}
 IEnumerator Isolate()
 {
  var eye=story.player.GetComponentInChildren<Camera>();var data=eye.GetUniversalAdditionalCameraData();
  var lights=FindObjectsByType<Light>().Where(l=>l.enabled).ToArray();
  var volumes=FindObjectsByType<Volume>().Where(v=>v.enabled).ToArray();
  var all=FindObjectsByType<Renderer>().Where(r=>r.enabled&&r.gameObject.activeInHierarchy).ToArray();
  var mouse=story.player.GetComponent<DieuKhienGocNhin>();bool movement=story.player.enabled,looking=mouse.enabled;float scale=Time.timeScale;bool post=data.renderPostProcessing;bool fog=RenderSettings.fog;
  var rows=new JArray();GameObject bloomOverride=null;
  IEnumerator Shot(string name)
  {
   yield return new WaitForSecondsRealtime(.35f);yield return new WaitForEndOfFrame();
   var tex=ScreenCapture.CaptureScreenshotAsTexture();var colors=tex.GetPixels32();int white=colors.Count(c=>c.r>=250&&c.g>=250&&c.b>=250);File.WriteAllBytes(Path.Combine(folder,name+".png"),tex.EncodeToPNG());Destroy(tex);
   rows.Add(new JObject{["stage"]=name,["nearWhiteFraction"]=(double)white/colors.Length});File.WriteAllText(Path.Combine(folder,"stages.json"),rows.ToString());
  }
  try{
   story.player.enabled=false;mouse.enabled=false;Time.timeScale=0;
   yield return Shot("00-baseline");
   data.renderPostProcessing=false;yield return Shot("01-post-off");data.renderPostProcessing=post;
   bloomOverride=new GameObject("Developer_Bloom_Only_Override");var v=bloomOverride.AddComponent<Volume>();v.isGlobal=true;v.priority=100000;v.profile=ScriptableObject.CreateInstance<VolumeProfile>();v.profile.Add<Bloom>().intensity.Override(0);
   yield return Shot("02-bloom-off");v.enabled=false;
   foreach(var l in lights)l.enabled=false;yield return Shot("03-lights-off");foreach(var l in lights)l.enabled=true;
   foreach(var a in volumes)a.enabled=false;yield return Shot("04-volumes-off");foreach(var a in volumes)a.enabled=true;
   RenderSettings.fog=false;yield return Shot("05-fog-off");RenderSettings.fog=fog;
   var city=all.Where(r=>r.name.StartsWith("Surface_")).ToArray();foreach(var r in city)r.enabled=false;yield return Shot("06-city-surfaces-off");foreach(var r in city)r.enabled=true;
   var actors=all.Except(city).ToArray();foreach(var r in actors)r.enabled=false;yield return Shot("07-other-renderers-off");foreach(var r in actors)r.enabled=true;
   string[] patterns={"/ConRoi_Minh_0_0/","/V42_DaLat_Irregular_Pine_Grove/","/CR_Bao/","/V41_Lake_Details/","/V42_Route_Facades/","ConRoi_String_"};
   for(int k=0;k<patterns.Length;k++){
    var group=all.Where(r=>PathOf(r.transform).Contains(patterns[k])).ToArray();foreach(var r in group)r.enabled=false;yield return Shot("group-"+k);rows.Last["pattern"]=patterns[k];rows.Last["count"]=group.Length;foreach(var r in group)r.enabled=true;
    yield return Shot("restore-group-"+k);
   }
   var materialGroups=all.SelectMany(r=>r.sharedMaterials).Where(m=>m).Distinct().Where(m=>m.name.Contains("Pine")||m.name.Contains("Day_Roi")||m.name.Contains("Timber")||m.name.Contains("Leaf")||m.name.Contains("Needle")).ToArray();
   for(int k=0;k<materialGroups.Length;k++){
    var mat=materialGroups[k];var group=all.Where(r=>r.sharedMaterials.Contains(mat)).ToArray();foreach(var r in group)r.enabled=false;yield return Shot("material-"+k);rows.Last["material"]=mat.name;rows.Last["count"]=group.Length;foreach(var r in group)r.enabled=true;
    yield return Shot("restore-material-"+k);
   }
   // Bisection removes one half at a time, restores it, then follows the half
   // whose removal removes the symptom. This is diagnostic evidence only.
   var candidates=all.Where(r=>GeometryUtility.TestPlanesAABB(GeometryUtility.CalculateFrustumPlanes(eye),r.bounds)).ToArray();
   int pass=0;
   while(candidates.Length>1&&pass<12){
    var half=candidates.Take((candidates.Length+1)/2).ToArray();foreach(var r in half)r.enabled=false;
    string name="bisect-"+pass.ToString("D2");yield return Shot(name);foreach(var r in half)r.enabled=true;
    var fraction=(double)rows.Last["nearWhiteFraction"];bool removed=fraction<(double)rows[0]["nearWhiteFraction"]-.015;
    rows.Last["hidden"]=new JArray(half.Select(r=>PathOf(r.transform)));rows.Last["symptomRemoved"]=removed;
    candidates=removed?half:candidates.Except(half).ToArray();pass++;
   }
   File.WriteAllText(Path.Combine(folder,"result.json"),new JObject{["method"]="Opt-in developer ablation on frozen live foreground view, all changes restored; not a gameplay verification",["candidate"]=new JArray(candidates.Select(r=>new JObject{["path"]=PathOf(r.transform),["materials"]=new JArray(r.sharedMaterials.Select(m=>m.name)),["shaders"]=new JArray(r.sharedMaterials.Select(m=>m.shader.name)),["bounds"]=r.bounds.ToString("F4")})),["stages"]=rows}.ToString());
  }finally{
   foreach(var r in all)if(r)r.enabled=true;foreach(var l in lights)if(l)l.enabled=true;foreach(var v in volumes)if(v)v.enabled=true;
   if(bloomOverride){Destroy(bloomOverride.GetComponent<Volume>().profile);Destroy(bloomOverride);}data.renderPostProcessing=post;RenderSettings.fog=fog;Time.timeScale=scale;story.player.enabled=movement;mouse.enabled=looking;
  }
 }
 static string PathOf(Transform t)=>t.parent?PathOf(t.parent)+"/"+t.name:t.name;
}
