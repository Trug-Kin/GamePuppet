#if UNITY_EDITOR || CON_ROI_BUILD_REVIEW
using System.Collections;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEngine;

public sealed partial class C3ActualPlayRun
{
 IEnumerator RunMaterialViewsOnly()
 {
  player=GetComponent<DieuKhienDiChuyen>();player.DisableTestInputHook();previousBackground=Application.runInBackground;Application.runInBackground=true;
  folder="Docs/Reviews/lake_gate_market_current/v33_views_"+System.DateTime.UtcNow.ToString("yyyyMMdd_HHmmss");Directory.CreateDirectory(folder);
  yield return CaptureMaterialReview();
  var route=JObject.Parse(regionalRoute.text)["route"].Select(P).ToArray();
  var go=new GameObject("Temporary_Representative_Material_Camera");var camera=go.AddComponent<Camera>();camera.CopyFrom(player.GetComponentInChildren<Camera>());camera.enabled=false;var rt=new RenderTexture(1440,900,24);camera.targetTexture=rt;
  foreach(int index in new[]{1,47,141,165,213,277})
  {
   var p=route[index];if(Physics.Raycast(new Vector3(p.x,100,p.z),Vector3.down,out var hit,200,8))p.y=hit.point.y+1.65f;
   camera.transform.SetPositionAndRotation(p,Quaternion.LookRotation(route[Mathf.Min(index+2,route.Length-1)]-route[index]));camera.Render();yield return null;camera.Render();
   var old=RenderTexture.active;RenderTexture.active=rt;var image=new Texture2D(rt.width,rt.height,TextureFormat.RGB24,false);image.ReadPixels(new Rect(0,0,rt.width,rt.height),0,0);image.Apply();File.WriteAllBytes(Path.Combine(folder,"continuous_forward_station_"+index+".png"),image.EncodeToPNG());Destroy(image);RenderTexture.active=old;
  }
  camera.targetTexture=null;rt.Release();Destroy(rt);Destroy(go);
  File.WriteAllText(folder+"/result.json",new JObject{["status"]="VISUAL_CAPTURES_EXECUTED",["movement"]="NOT_RUN_IN_THIS_CAPTURE_ONLY_MODE",["scene"]=gameObject.scene.path,["OS_input"]="NOT_VERIFIED"}.ToString());complete=true;
#if UNITY_EDITOR
  UnityEditor.EditorApplication.isPlaying=false;
#endif
 }
 IEnumerator WarmMaterialShaders()
 {
  // First-draw shader compilation must finish before timed input/movement cases.
  var route=JObject.Parse(regionalRoute.text)["route"].Select(P).ToArray();
  var go=new GameObject("Temporary_ShaderWarmup_Camera");var camera=go.AddComponent<Camera>();camera.CopyFrom(player.GetComponentInChildren<Camera>());camera.enabled=false;
  var rt=new RenderTexture(1440,900,24);camera.targetTexture=rt;
  foreach(int index in new[]{1,47,141,165,213,277})
  {
   var p=route[index];if(Physics.Raycast(new Vector3(p.x,100,p.z),Vector3.down,out var h,200,8))p.y=h.point.y+1.65f;
   camera.transform.SetPositionAndRotation(p,Quaternion.LookRotation(route[Mathf.Min(index+2,route.Length-1)]-route[index]));camera.Render();yield return null;yield return null;
   camera.transform.Rotate(0,180,0);camera.Render();yield return null;
  }
  camera.targetTexture=null;rt.Release();Destroy(rt);Destroy(go);
 }
 IEnumerator CaptureMaterialReview()
 {
  var original=player.GetComponentInChildren<Camera>();var go=new GameObject("Temporary_MaterialReview_Camera");var camera=go.AddComponent<Camera>();camera.CopyFrom(original);camera.enabled=false;camera.fieldOfView=65;
  var texture=new RenderTexture(1440,900,24);camera.targetTexture=texture;var captures=new JArray();
  var panel=GameObject.Find("LakeGateMarket_V33_MATERIAL_SURFACES/Surface_041");
  if(!panel)panel=FindObjectsByType<MeshRenderer>(FindObjectsSortMode.None).First(r=>r.enabled&&r.name=="Surface_041").gameObject;
  var bounds=panel.GetComponent<Renderer>().bounds;var normal=Vector3.forward;
  // Derive actual long horizontal direction from imported vertices, never repair a face by emission.
  var mesh=panel.GetComponent<MeshFilter>().sharedMesh;var points=mesh.vertices.Select(p=>panel.transform.TransformPoint(p)).ToArray();
  var mean=bounds.center;var far=points.OrderByDescending(p=>(new Vector2(p.x-mean.x,p.z-mean.z)).sqrMagnitude).First()-mean;far.y=0;far.Normalize();normal=Vector3.Cross(Vector3.up,far).normalized;
  for(int side=0;side<2;side++)foreach(float distance in new[]{12f,25f,40f})
  {
   var position=mean+normal*(side==0?distance:-distance);position.y=mean.y;
   if(Physics.Raycast(new Vector3(position.x,100,position.z),Vector3.down,out var ground,200,8))position.y=ground.point.y+1.65f;
   camera.transform.SetPositionAndRotation(position,Quaternion.LookRotation(mean-position));
   yield return null;camera.Render();var previous=RenderTexture.active;RenderTexture.active=texture;var image=new Texture2D(texture.width,texture.height,TextureFormat.RGB24,false);image.ReadPixels(new Rect(0,0,texture.width,texture.height),0,0);image.Apply();
   string name="gate_side_"+side+"_"+distance.ToString("F0")+"m.png";File.WriteAllBytes(Path.Combine(folder,name),image.EncodeToPNG());Destroy(image);RenderTexture.active=previous;
   captures.Add(new JObject{["image"]=name,["horizontal_distance_m"]=distance,["position"]=position.ToString("F4"),["gate_center"]=mean.ToString("F4"),["fov_deg"]=camera.fieldOfView});
  }
  camera.targetTexture=null;texture.Release();Destroy(texture);Destroy(go);
  File.WriteAllText(Path.Combine(folder,"material_views.json"),new JObject{["scene"]=gameObject.scene.path,["mode"]="Executed Play Mode Camera.Render, temporary review camera, no player relocation",["captures"]=captures,["gate_emission"]=0,["OS_input"]="NOT_VERIFIED"}.ToString());
 }
}

#endif
