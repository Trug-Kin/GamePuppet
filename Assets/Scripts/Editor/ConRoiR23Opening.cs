using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class ConRoiR23Opening
{
 public const string Scene="Assets/Scenes/JourneyRepair_v48_r23.unity", Out="Docs/Reviews/journey_v48/r23";
 static JArray V(Vector3 v)=>new JArray(v.x,v.y,v.z);
 static string PathOf(Transform t)=>t.parent?PathOf(t.parent)+"/"+t.name:t.name;
 public static void Author(){
  EditorSceneManager.OpenScene(ConRoiR22Scenery.Scene);EditorSceneManager.SaveScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene(),Scene);var s=UnityEngine.Object.FindAnyObjectByType<DieuPhoiHanhTrinh>();
  var old=s.GetComponent<HoSoSceneR22>();var pipeline=old.pipeline;UnityEngine.Object.DestroyImmediate(old);
  var profile=s.gameObject.AddComponent<HoSoSceneR23>();profile.story=s;profile.pipeline=pipeline;
  UnityEngine.Object.DestroyImmediate(s.GetComponent<TraiNghiemHanhTrinhR22>());
  var e=s.gameObject.AddComponent<TraiNghiemHanhTrinhR23>();e.story=s;
  var ui=AssetDatabase.LoadAssetAtPath<Material>("Assets/Art/UI/R23/ReadableUI.mat");if(!ui){ui=new Material(Shader.Find("ConRoi/R23/ReadableUI"));AssetDatabase.CreateAsset(ui,"Assets/Art/UI/R23/ReadableUI.mat");}e.readableUi=ui;
  var encounter=s.gameObject.AddComponent<MoDauBenHoR23>();encounter.story=s;
  s.hub.SetActive(false);s.lake.SetActive(true);Physics.SyncTransforms();
  // Measured deck OD35_COLLIDER_3; one metre from rail, 2.4m from Bao.
  var position=new Vector3(656.05f,2.0521f,514.35f);
  var facing=new Vector3(657.2604f,2.65f,512.21405f)-new Vector3(position.x,position.y+.75f,position.z);facing.y=0;
  s.lakeSpawn.SetPositionAndRotation(position,Quaternion.LookRotation(facing));
  var p=s.puppets[0];var legs=p.GetComponentsInChildren<Renderer>(true).Where(r=>r.name=="ConRoi_Leg_L"||r.name=="ConRoi_Leg_R").ToArray();
  Vector3 foot=(legs[0].bounds.center+legs[1].bounds.center)*.5f;foot.y=legs.Min(r=>r.bounds.min.y);
  encounter.lakeFootAnchor=new Vector3(660.12f,1.1521f,510.72f);p.transform.position+=encounter.lakeFootAnchor-foot;p.enabled=false;
  var toPlayer=position-encounter.lakeFootAnchor;toPlayer.y=0;p.transform.RotateAround(encounter.lakeFootAnchor,Vector3.up,Vector3.SignedAngle(Vector3.back,toPlayer,Vector3.up));
  foreach(var puppet in s.puppets)foreach(string side in new[]{"L","R"}){
   var hip=puppet.GetComponentsInChildren<Transform>(true).First(t=>t.name=="V40_Wooden_Joint_ConRoi_Leg_"+side);
   var bearing=GameObject.CreatePrimitive(PrimitiveType.Sphere);bearing.name="R23_Wooden_HipBearing_"+side;
   UnityEngine.Object.DestroyImmediate(bearing.GetComponent<Collider>());bearing.transform.position=hip.position+Vector3.up*.055f;bearing.transform.localScale=Vector3.one*.19f;
   bearing.GetComponent<Renderer>().sharedMaterial=hip.GetComponentInChildren<Renderer>().sharedMaterial;bearing.transform.SetParent(hip,true);
  }
  // The retained calendar stand was immediately in front of the new arrival.
  // Move the complete assembly (renderers AND collider) along the same rail,
  // leaving the initial Bao/puppet sightline and the walking strip open.
  var calendar=GameObject.Find("R11_Calendar_Complete_Assembly");
  calendar.transform.position=new Vector3(661.239f,1.1420726f,509.177f);
  foreach(var puppet in s.puppets){
   var perf=puppet.GetComponent<DienXuatConRoiV40>();if(perf)perf.enabled=false;
   var stride=puppet.gameObject.AddComponent<BuocChanConRoiR23>();stride.puppet=puppet;stride.trajectory=AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/Art/Characters/R23/WoodenStep.json");
  }
  var ground=p.GetComponent<TiepDatConRoiV41>();if(ground)ground.enabled=false;
  foreach(var i in UnityEngine.Object.FindObjectsByType<TuongTacHoCho>(FindObjectsInactive.Include).Where(i=>i.kind==TuongTacHoCho.Kind.Puppet))UnityEngine.Object.DestroyImmediate(i);
  var lightObject=new GameObject("R23_Rail_ReflectedFill");lightObject.transform.SetParent(s.lake.transform);lightObject.transform.position=encounter.lakeFootAnchor+Vector3.up*2+encounter.landward;
  var light=lightObject.AddComponent<Light>();light.type=LightType.Point;light.range=6;light.intensity=3.5f;light.color=new Color(.82f,.86f,.78f);
  Physics.SyncTransforms();
  var sample=new JArray();foreach(var delta in new[]{Vector3.zero,Vector3.right*.4f,Vector3.left*.4f,Vector3.forward*.4f,Vector3.back*.4f}){
   var at=position+delta;sample.Add(new JObject{["position"]=V(at),["safe"]=s.GetComponent<PhucHoiTaiBienR21>().Supported(at,.5f)});
  }
  File.WriteAllText(Out+"/authoring.json",new JObject{["new_spawn"]=V(position),["rotation"]=V(s.lakeSpawn.eulerAngles),["puppet_feet"]=V(encounter.lakeFootAnchor),["support_samples"]=sample,["old_spawn"]=V(new Vector3(508.9331f,3.14612627f,514.668945f)),["save_policy"]="ApplyRegion(true) lake entry only; Load still ApplyRegion(false) then saved pose; isolated r23 profile"}.ToString());
  File.WriteAllText(Out+"/nearby-renderers.json",new JArray(UnityEngine.Object.FindObjectsByType<MeshRenderer>(FindObjectsInactive.Include).Where(r=>r.gameObject.activeInHierarchy&&Vector3.Distance(r.bounds.center,position)<3).Select(r=>new JObject{["path"]=PathOf(r.transform),["center"]=V(r.bounds.center),["size"]=V(r.bounds.size)})).ToString());
  s.hub.SetActive(true);s.lake.SetActive(false);
  EditorSceneManager.SaveScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene());AssetDatabase.SaveAssets();Debug.Log("R23_AUTHOR_COMPLETE");
 }
 public static void Survey(){
  EditorSceneManager.OpenScene(ConRoiR22Scenery.Scene);
  var s=UnityEngine.Object.FindAnyObjectByType<DieuPhoiHanhTrinh>();s.hub.SetActive(false);s.lake.SetActive(true);Physics.SyncTransforms();
  var cc=s.player.GetComponent<CharacterController>();var b=s.GetComponent<TuTheBaoBenCauV48>();
  var p=s.puppets[0];var rows=new JArray();
  foreach(var t in p.GetComponentsInChildren<Transform>(true))rows.Add(new JObject{["name"]=t.name,["parent"]=t.parent?t.parent.name:"",["local"]=V(t.localPosition),["world"]=V(t.position),["scale"]=V(t.localScale),["rotation"]=V(t.localEulerAngles),["bounds"]=t.GetComponent<Renderer>()?V(t.GetComponent<Renderer>().bounds.size):null});
  var ground=new JArray();
  foreach(float x in new[]{654f,655f,656f,657f,658f,659f,660f,661f})foreach(float z in new[]{510f,511f,512f,513f,514f,515f,516f}){
   var hits=Physics.RaycastAll(new Vector3(x,6,z),Vector3.down,9,~0,QueryTriggerInteraction.Ignore).OrderBy(h=>h.distance);
   ground.Add(new JObject{["x"]=x,["z"]=z,["hits"]=new JArray(hits.Select(h=>new JObject{["y"]=h.point.y,["name"]=h.transform.name,["layer"]=h.transform.gameObject.layer}))});
  }
  var meshes=new JArray();foreach(var f in p.GetComponentsInChildren<MeshFilter>(true))if(f.sharedMesh){var m=f.sharedMesh;meshes.Add(new JObject{["name"]=f.name,["vertices"]=new JArray(m.vertices.Select(v=>V(p.transform.InverseTransformPoint(f.transform.TransformPoint(v))))),["triangles"]=new JArray(m.triangles),["color"]=f.GetComponent<Renderer>().sharedMaterial.color.ToString()});}
  var curves=new JArray();foreach(AnimationClip clip in AnimationUtility.GetAnimationClips(p.gameObject))foreach(var binding in AnimationUtility.GetCurveBindings(clip))curves.Add(new JObject{["clip"]=clip.name,["path"]=binding.path,["property"]=binding.propertyName,["keys"]=new JArray(AnimationUtility.GetEditorCurve(clip,binding).keys.Select(k=>new JArray(k.time,k.value)))});
  File.WriteAllText(Out+"/survey.json",new JObject{["bao"]=V(b.actorPosition),["calendar"]=b.calendar?V(b.calendar.position):null,["traces"]=new JArray(s.traces.Select(t=>new JObject{["name"]=t.name,["position"]=V(t.position),["renderers"]=new JArray(t.GetComponentsInChildren<Renderer>().Select(r=>new JObject{["name"]=r.name,["center"]=V(r.bounds.center),["size"]=V(r.bounds.size)}))})),["player"]=V(s.player.transform.position),["cc_center"]=V(cc.center),["cc_height"]=cc.height,["cc_radius"]=cc.radius,["eye_local"]=V(s.player.GetComponentInChildren<Camera>().transform.localPosition),["puppet"]=rows,["ground"]=ground,["curves"]=curves,["face_renderers"]=new JArray(p.GetComponentsInChildren<Renderer>(true).Where(r=>r.name.Contains("Face")||r.name.Contains("Head")).Select(r=>new JObject{["name"]=r.name,["center"]=V(r.bounds.center),["forward"]=V(r.transform.forward),["enabled"]=r.enabled,["material"]=r.sharedMaterial.name}))}.ToString());
  File.WriteAllText(Out+"/puppet-meshes.json",meshes.ToString(Newtonsoft.Json.Formatting.None));
  Debug.Log("R23_SURVEY_COMPLETE");
 }
}
