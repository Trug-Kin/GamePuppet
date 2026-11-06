using System.Linq;
using System.Reflection;
using Newtonsoft.Json.Linq;
using UnityEngine;
public static class R38ReflectionProbe {
 static JArray V(Vector3 p)=>new JArray(p.x,p.y,p.z);
 public static JObject Sample(DieuPhoiHanhTrinh s){
  var r=s.GetComponent<PhanChieuMatHo>();var n=s.GetComponent<TuongTacSoTayHoR37>();var eye=r.playerCamera;var mirror=typeof(PhanChieuMatHo).GetField("reflection",BindingFlags.Instance|BindingFlags.NonPublic).GetValue(r) as Camera;
  float y=r.water.bounds.center.y;var rows=new JArray();
  foreach(var t in new[]{n.book,n.gesture,s.puppets[0].dauTransform}){
   var p=t.position;var reflected=p;reflected.y=2*y-p.y;float at=(y-eye.transform.position.y)/(reflected.y-eye.transform.position.y);var hit=Vector3.LerpUnclamped(eye.transform.position,reflected,at);var d=hit-eye.transform.position;
   var obstacles=Physics.RaycastAll(eye.transform.position,d.normalized,d.magnitude,~0,QueryTriggerInteraction.Ignore).Where(h=>!h.transform.IsChildOf(s.player.transform)&&!h.transform.IsChildOf(n.book)).OrderBy(h=>h.distance);
   rows.Add(new JObject{["object"]=t.name,["position"]=V(p),["virtualMirrorPoint"]=V(reflected),["intersectionAtWater"]=V(hit),["mainView"]=V(eye.WorldToViewportPoint(p)),["reflectionView"]=mirror?V(mirror.WorldToViewportPoint(p)):null,["blockersEyeToWater"]=new JArray(obstacles.Select(h=>new JObject{["name"]=h.collider.name,["point"]=V(h.point)})),["renderers"]=new JArray(t.GetComponentsInChildren<Renderer>(true).Select(a=>new JObject{["name"]=a.name,["enabled"]=a.enabled&&a.gameObject.activeInHierarchy,["layer"]=a.gameObject.layer,["includedByMask"]=mirror&&(mirror.cullingMask&(1<<a.gameObject.layer))!=0}))});
  }
  return new JObject{["technique"]="Planar world reflection; same world renderers, matrix mirrored at horizontal lake surface, oblique clip; no reflected clones. Camera frustum inclusion alone does not imply visible water pixels: bridge/rail can occlude.",["urpRequest"]=r.urpCaptureR38,["mainMask"]=eye.cullingMask,["reflectionMask"]=mirror?mirror.cullingMask:0,["reflectionEnabled"]=mirror&&mirror.enabled,["cameraForward"]=mirror?V(mirror.transform.forward):null,["expectedForward"]=V(Vector3.Reflect(eye.transform.forward,Vector3.up)),["cameraOrientationErrorDegrees"]=mirror?Vector3.Angle(mirror.transform.forward,Vector3.Reflect(eye.transform.forward,Vector3.up)):0,["waterY"]=y,["states"]=rows,["bookOpen"]=n.memory.open,["bookHeld"]=n.Held};
 }
}
