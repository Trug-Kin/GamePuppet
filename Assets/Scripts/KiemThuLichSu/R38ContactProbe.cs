using System;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;
// Opt-in measurement only. Tests displayed support triangles over a sole footprint, not only colliders.
public static class R38ContactProbe {
 static JArray V(Vector3 v)=>new JArray(v.x,v.y,v.z);
 public static JObject Sample(TuTheBaoBenCauV48 b){
  var rig=b.acting.rigs.First(r=>r.role=="Bao");var shoes=rig.shoes!=null&&rig.shoes.Length>0?rig.shoes:b.actor.GetComponentsInChildren<MeshFilter>().Where(f=>f.name.Contains("Shoe")&&f.GetComponent<Renderer>().enabled).Select(f=>f.transform).ToArray();var rows=new JArray();
  int cameraMask=b.story.player.GetComponentInChildren<Camera>().cullingMask;
  var surfaces=UnityEngine.Object.FindObjectsByType<MeshFilter>().Where(f=>f.sharedMesh&&f.sharedMesh.isReadable&&f.GetComponent<Renderer>() is Renderer r&&r.enabled&&!r.forceRenderingOff&&r.shadowCastingMode!=UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly&&(cameraMask&(1<<f.gameObject.layer))!=0&&r.gameObject.activeInHierarchy&&!f.transform.IsChildOf(b.actor)&&!f.transform.IsChildOf(b.story.player.transform)&&Mathf.Abs(r.bounds.center.y-b.actor.position.y)<2&&Vector3.Distance(r.bounds.ClosestPoint(b.actor.position),b.actor.position)<1).ToArray();
  var world=surfaces.ToDictionary(f=>f,f=>f.sharedMesh.vertices.Select(f.transform.TransformPoint).ToArray());
  var support=new List<(Vector3 a,Vector3 b,Vector3 c,string name)>();foreach(var g in surfaces){var a=world[g];var ind=g.sharedMesh.triangles;for(int j=0;j<ind.Length;j+=3){var aa=a[ind[j]];var bb=a[ind[j+1]];var cc=a[ind[j+2]];if(Mathf.Abs(Vector3.Cross(bb-aa,cc-aa).normalized.y)<.7f)continue;float x=b.actor.position.x,z=b.actor.position.z;if(Mathf.Max(aa.x,bb.x,cc.x)<x-.5f||Mathf.Min(aa.x,bb.x,cc.x)>x+.5f||Mathf.Max(aa.z,bb.z,cc.z)<z-.5f||Mathf.Min(aa.z,bb.z,cc.z)>z+.5f)continue;support.Add((aa,bb,cc,g.name));}}
  foreach(var t in shoes){var f=t.GetComponent<MeshFilter>();var p=f.sharedMesh.vertices.Select(t.TransformPoint).ToArray();var tris=f.sharedMesh.triangles;float bottom=p.Min(v=>v.y);var bounds=f.GetComponent<Renderer>().bounds;var samples=new JArray();
   for(int ix=0;ix<7;ix++)for(int iz=0;iz<11;iz++){
    float x=Mathf.Lerp(bounds.min.x,bounds.max.x,(ix+.5f)/7),z=Mathf.Lerp(bounds.min.z,bounds.max.z,(iz+.5f)/11);float sole=float.PositiveInfinity;
    for(int i=0;i<tris.Length;i+=3)if(Y(x,z,p[tris[i]],p[tris[i+1]],p[tris[i+2]],out float y))sole=Mathf.Min(sole,y);
    if(float.IsInfinity(sole)||sole>bottom+.022f)continue;float ground=float.NegativeInfinity;string name="none";
    foreach(var g in support)if(Y(x,z,g.a,g.b,g.c,out float h)&&h<sole+.3f&&h>ground){ground=h;name=g.name;}
    var hits=Physics.RaycastAll(new Vector3(x,sole+.3f,z),Vector3.down,1,8,QueryTriggerInteraction.Ignore).Where(h=>!h.transform.IsChildOf(b.actor)&&h.normal.y>.7f).OrderBy(h=>h.distance).ToArray();
    samples.Add(new JObject{["x"]=x,["z"]=z,["soleY"]=sole,["displaySupportY"]=float.IsInfinity(ground)?JValue.CreateNull():new JValue(ground),["displayGap"]=float.IsInfinity(ground)?JValue.CreateNull():new JValue(sole-ground),["mesh"]=name,["colliderY"]=hits.Length>0?new JValue(hits[0].point.y):JValue.CreateNull()});
   }
   rows.Add(new JObject{["name"]=t.name,["pivot"]=V(t.position),["scale"]=V(t.lossyScale),["meshMinY"]=bottom,["samples"]=samples});
  }
  return new JObject{["method"]="7x11 vertical footprint grid against displayed shoe triangles (lower sole band 22mm), displayed support mesh triangles and independent collider hits; curved sole edges can have natural clearance. Not a claim of full planar sole contact.",["actor"]=V(b.actor.position),["actorScale"]=V(b.actor.lossyScale),["root"]=V(b.actorPosition),["shoes"]=rows,["visibleSupports"]=new JArray(surfaces.Select(f=>f.name)),["arm"]=BaoContactProbeV48.Sample(b)};
 }
 static bool Y(float x,float z,Vector3 a,Vector3 b,Vector3 c,out float y){a.x-=x;a.z-=z;b.x-=x;b.z-=z;c.x-=x;c.z-=z;double den=(b.z-c.z)*(a.x-c.x)+(c.x-b.x)*(a.z-c.z);if(Math.Abs(den)<1e-10){y=0;return false;}double u=((b.z-c.z)*(-c.x)+(c.x-b.x)*(-c.z))/den,v=((c.z-a.z)*(-c.x)+(a.x-c.x)*(-c.z))/den,w=1-u-v;y=(float)(u*a.y+v*b.y+w*c.y);return u>=-.0001&&v>=-.0001&&w>=-.0001;}
}
