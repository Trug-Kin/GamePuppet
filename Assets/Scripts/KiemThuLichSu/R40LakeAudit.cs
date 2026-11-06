#if UNITY_EDITOR || CON_ROI_BUILD_REVIEW
using System;using System.IO;using System.Collections;using System.Linq;using Newtonsoft.Json.Linq;using UnityEngine;using UnityEngine.Rendering;using UnityEngine.Rendering.Universal;using UnityEngine.SceneManagement;
// Explicit Editor audit only, never serialized. Diagnostic state/camera setup is not walkthrough evidence.
public sealed class R40LakeAudit:MonoBehaviour {
 const string Out="Docs/Reviews/journey_v48/r40";JArray checks=new JArray();bool finished;
 void Check(string name,bool pass){checks.Add(new JObject{["name"]=name,["pass"]=pass});File.WriteAllText(Out+"/audit-result.json",new JObject{["checks"]=checks,["complete"]=finished&&checks.All(x=>(bool)x["pass"]),["scope"]="Editor diagnostic camera/state setup; not physical display/input or walkthrough"}.ToString());if(!pass)throw new Exception(name);}
 IEnumerator Ready(){var s=FindAnyObjectByType<DieuPhoiHanhTrinh>();while(s.Loading)yield return null;s.SavePathOverride=Out+"/audit-only-save.json";s.SetMenu(false);s.data.region=0;typeof(DieuPhoiHanhTrinh).GetMethod("ApplyRegion",System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.NonPublic).Invoke(s,new object[]{true});yield return null;yield return null;}
 void Capture(Camera c,string path){var rt=RenderTexture.GetTemporary(1280,720,24);var prev=RenderTexture.active;var t=new Texture2D(1280,720,TextureFormat.RGB24,false);try{RenderPipeline.SubmitRenderRequest(c,new UniversalRenderPipeline.SingleCameraRequest{destination=rt});RenderTexture.active=rt;t.ReadPixels(new Rect(0,0,1280,720),0,0);t.Apply();File.WriteAllBytes(Out+"/"+path+".png",t.EncodeToPNG());}finally{RenderTexture.active=prev;RenderTexture.ReleaseTemporary(rt);Destroy(t);}}
 IEnumerator Photos(string label){var s=FindAnyObjectByType<DieuPhoiHanhTrinh>();var eye=s.player.GetComponentInChildren<Camera>();s.player.enabled=false;s.player.GetComponent<DieuKhienGocNhin>().enabled=false;foreach(var canvas in FindObjectsByType<Canvas>(FindObjectsSortMode.None))canvas.enabled=false;
  eye.transform.position=new Vector3(656.05f,2.79006f,514.35f);eye.transform.rotation=Quaternion.LookRotation(new Vector3(650,0,496)-eye.transform.position);yield return null;yield return new WaitForEndOfFrame();Capture(eye,label+"-Bao-to-lake");
  eye.transform.position=new Vector3(659,60,507);eye.transform.rotation=Quaternion.Euler(90,0,0);eye.orthographic=true;eye.orthographicSize=27;yield return null;yield return new WaitForEndOfFrame();Capture(eye,label+"-top");
 }
 static AsyncOperation LoadScene(string path){
#if UNITY_EDITOR
 return UnityEditor.SceneManagement.EditorSceneManager.LoadSceneAsyncInPlayMode(path,new LoadSceneParameters(LoadSceneMode.Single));
#else
 return SceneManager.LoadSceneAsync(path);
#endif
 }
 IEnumerator Start(){
  DontDestroyOnLoad(gameObject);yield return Ready();Check("r40_no_U_after_bootstrap",!FindObjectsByType<Transform>(FindObjectsSortMode.None).Any(t=>t.name.Contains("Protected_Reflection_Overlook")));yield return Photos("after-r40");
  var load=LoadScene("Assets/Scenes/JourneyRepair_v48_r39.unity");while(!load.isDone)yield return null;yield return Ready();yield return Photos("before-r39");
  load=LoadScene("Assets/Scenes/JourneyRepair_v48_r40.unity");while(!load.isDone)yield return null;yield return Ready();var s=FindAnyObjectByType<DieuPhoiHanhTrinh>();
  foreach(string name in new[]{"final-main/open-held.json","final-main/placed.json","final-edge/placed-open.json"}){
   var source="Docs/Reviews/journey_v48/r39/"+name;var raw=File.ReadAllText(source);var original=JsonUtility.FromJson<DieuPhoiHanhTrinh.State>(raw);File.WriteAllText(s.SavePathOverride,raw);s.Load();yield return null;yield return null;
   Check("r39_save_safe_"+name,Vector3.Distance(s.player.transform.position,s.lakeSpawn.position)<.2f&&s.GetComponent<PhucHoiTaiBienR21>().Supported(s.player.transform.position,.5f));
   Check("progress_retained_"+name,s.data.region==original.region&&s.data.nextRegion==original.nextRegion&&s.data.awareness==original.awareness&&s.data.actionPoints==original.actionPoints&&s.data.witnessed.SequenceEqual(original.witnessed)&&s.data.traces.SequenceEqual(original.traces));
   Check("load_no_write_"+name,File.ReadAllText(s.SavePathOverride)==raw);
  }
  string old=File.ReadAllText("Docs/Reviews/journey_v48/r38/native-main/placed.json");File.WriteAllText(s.SavePathOverride,old);var state=JsonUtility.FromJson<DieuPhoiHanhTrinh.State>(old);s.Load();yield return null;Check("r38_position_preserved",Vector3.Distance(s.player.transform.position,state.position)<.2f);
  finished=true;Check("diagnostic_suite_complete",true);
  // New clean initialization, then existing real-controller lake fixture (not the diagnostic photos).
  load=LoadScene("Assets/Scenes/JourneyRepair_v48_r40.unity");while(!load.isDone)yield return null;
  new GameObject("R40_RealController_Review").AddComponent<R38LakeReview>().output=Out+"/"+(Environment.GetCommandLineArgs().Contains("--r40-edge")?"editor-edge":"editor-main");Destroy(gameObject);
 }
}

#endif
