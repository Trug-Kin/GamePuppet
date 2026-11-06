using System;using System.IO;using System.Collections;using Newtonsoft.Json.Linq;using UnityEngine;using UnityEngine.Rendering;using UnityEngine.Rendering.Universal;using UnityEditor;using UnityEditor.SceneManagement;
[InitializeOnLoad] public static class R38SavedViews {
 static R38SavedViews(){EditorApplication.playModeStateChanged+=p=>{if(p==PlayModeStateChange.EnteredPlayMode&&SessionState.GetBool("r38.views",false)){SessionState.SetBool("r38.views",false);new GameObject("EditorOnly_SavedViews").AddComponent<R38SavedViewsRunner>();}};}
 public static void Begin(){EditorSceneManager.OpenScene(ConRoiR38.Scene);SessionState.SetBool("r38.views",true);EditorApplication.EnterPlaymode();}
}
public sealed class R38SavedViewsRunner:MonoBehaviour {
 string Out=>ConRoiR38.Out+"/editor-saved-views";JArray checks=new JArray();DieuPhoiHanhTrinh s;TuongTacSoTayHoR37 n;Camera eye;
 void Check(string name,bool pass){checks.Add(new JObject{["name"]=name,["pass"]=pass});File.WriteAllText(Out+"/progress.json",checks.ToString());}
 IEnumerator Load(string path){File.Copy(path,s.SavePathOverride,true);s.GetComponent<MenuVaoGameV47>().Continue();yield return null;yield return new WaitForSecondsRealtime(.4f);s.player.MovementFilter=r=>Vector2.zero;}
 void Shot(string name){var rt=RenderTexture.GetTemporary(1280,720,24);var old=RenderTexture.active;var im=new Texture2D(1280,720,TextureFormat.RGB24,false);try{RenderPipeline.SubmitRenderRequest(eye,new UniversalRenderPipeline.SingleCameraRequest{destination=rt});RenderTexture.active=rt;im.ReadPixels(new Rect(0,0,1280,720),0,0);im.Apply();File.WriteAllBytes(Out+"/"+name+".png",im.EncodeToPNG());}finally{RenderTexture.active=old;RenderTexture.ReleaseTemporary(rt);Destroy(im);}}
 IEnumerator Start(){
  Directory.CreateDirectory(Out);s=FindAnyObjectByType<DieuPhoiHanhTrinh>();n=s.GetComponent<TuongTacSoTayHoR37>();eye=s.player.GetComponentInChildren<Camera>();s.SavePathOverride=Out+"/isolated-save.json";Application.runInBackground=true;while(s.Loading)yield return null;
  yield return Load(ConRoiR38.Out+"/native-main/placed.json");s.player.GetComponentInChildren<DieuKhienGocNhin>().enabled=false;var pos=eye.transform.position;var rot=Quaternion.LookRotation(n.socket.position-pos);eye.transform.SetPositionAndRotation(pos,rot);Shot("placed-open-same-camera");Check("open_loaded",!n.Held&&n.memory.open);
  yield return Load(ConRoiR38.Out+"/native-closed/placed.json");eye.transform.SetPositionAndRotation(pos,rot);Shot("placed-closed-same-camera");Check("closed_loaded",!n.Held&&!n.memory.open);
  File.WriteAllText(Out+"/camera.json",new JObject{["position"]=new JArray(pos.x,pos.y,pos.z),["rotation"]=new JArray(rot.x,rot.y,rot.z,rot.w),["scope"]="Editor diagnostic only, exact same camera after real binary saves loaded; not captured walkthrough."}.ToString());
  yield return Load("Docs/Reviews/journey_v48/r37/native-main/open-held.json");Check("r37_open_held_no_hinge_fields",n.Held&&n.memory.open&&n.memory.page==0&&!n.memory.hasHingeR38&&n.RestoredHinge==-180);Check("r37_progress_retained",n.memory.viewed!=0&&s.data.region==0);
  var old=JObject.Parse(File.ReadAllText("Docs/Reviews/journey_v48/r36/native-main/confirmed.json"));yield return Load("Docs/Reviews/journey_v48/r36/native-main/confirmed.json");Check("r36_missing_notebook_keeps_market_region",s.data.region==(int)old["region"]&&!n.Active&&!n.Held);Check("r36_progress_and_position",s.data.awareness==(int)old["awareness"]&&Vector3.Distance(s.player.transform.position,JsonUtility.FromJson<DieuPhoiHanhTrinh.State>(old.ToString()).position)<.1f);
  bool pass=true;foreach(var c in checks)pass&=(bool)c["pass"];File.WriteAllText(Out+"/result.json",new JObject{["complete"]=pass,["checks"]=checks,["scope"]="Editor diagnostic saved-state views and unmodified historical r37/r36 saves; isolated save override. No product dependencies."}.ToString());EditorApplication.Exit(pass?0:1);
 }
}
