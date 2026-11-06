using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEngine;

// Read-only rendered mesh measurements, after V46 deformation, at diagnostic cadence.
public static class BaoContactProbeV48
{
 static JArray V(Vector3 v)=>new JArray(v.x,v.y,v.z);
 public static JObject Sample(TuTheBaoBenCauV48 bridge)
 {
  if(!bridge||!bridge.actor.gameObject.activeInHierarchy)return null;
  var rig=bridge.acting.rigs.FirstOrDefault(r=>r.role=="Bao");if(rig==null)return null;
  var shoes=new JArray();
  var shoeParts=rig.shoes!=null&&rig.shoes.Length>0?rig.shoes:bridge.actor.GetComponentsInChildren<MeshFilter>(true).Where(f=>f.name.Contains("Shoe")&&f.GetComponent<Renderer>().enabled).Select(f=>f.transform).ToArray();
  foreach(var t in shoeParts)
  {
   var f=t.GetComponent<MeshFilter>();if(!f||!f.sharedMesh||!f.sharedMesh.isReadable)continue;
   var bottom=f.sharedMesh.vertices.Select(p=>t.TransformPoint(p)).OrderBy(p=>p.y).First();
   var hits=Physics.RaycastAll(bottom+Vector3.up*.3f,Vector3.down,1,8,QueryTriggerInteraction.Ignore).Where(h=>h.normal.y>.7f&&!h.collider.transform.IsChildOf(bridge.actor)).OrderByDescending(h=>h.point.y).ToArray();
   shoes.Add(new JObject{["name"]=t.name,["meshBottom"]=V(bottom),["supportY"]=hits.Length>0?new JValue(hits[0].point.y):JValue.CreateNull(),["gap"]=hits.Length>0?new JValue(bottom.y-hits[0].point.y):JValue.CreateNull(),["support"]=hits.Length>0?hits[0].collider.name:"none"});
  }
  var surface=bridge.acting.GetComponent<BeMatNhanVatV46>();var body=surface?surface.bodies.FirstOrDefault(b=>b.role=="Bao"):null;
  var sleeves=new JArray();var axis=Vector3.Cross(Vector3.up,bridge.waterDirection).normalized;
  if(body?.arms!=null)foreach(var arm in body.arms)
  {
   float min=float.PositiveInfinity;Vector3 nearest=Vector3.zero;
   foreach(var p in arm.vertices)
   {
    var w=arm.root.TransformPoint(p);var q=bridge.railContact+axis*Mathf.Clamp(Vector3.Dot(w-bridge.railContact,axis),-.6f,.6f);
    var d=Vector3.Distance(w,q)-.025f;if(d<min){min=d;nearest=w;}
   }
   var points=arm.vertices.Select(p=>arm.root.TransformPoint(p)).ToArray();var triangles=arm.mesh.triangles;
   var start=bridge.railContact-axis*.6f;var end=bridge.railContact+axis*.6f;float exact=float.PositiveInfinity;
   for(int i=0;i<triangles.Length;i+=3)exact=Mathf.Min(exact,SegmentTriangle(start,end,points[triangles[i]],points[triangles[i+1]],points[triangles[i+2]]));
   sleeves.Add(new JObject{["name"]=arm.root.name,["vertexToTopRailSurfaceMin"]=min,["triangleToTopRailSurfaceMin"]=Mathf.Sqrt(exact)-.025f,["nearestVertex"]=V(nearest)});
  }
  return new JObject{["method"]="Rendered shoe vertices and ground raycasts; exact segment-to-triangle distance from visible sleeve mesh to local top-rail capsule proxy (radius .025m, half length .6m). Does not certify every other railing part.",["shoes"]=shoes,["sleeves"]=sleeves,["dialogue"]=bridge.story.Dialogue,["habitLift"]=bridge.story.GetComponent<TraiNghiemHanhTrinhV42>().BaoHabitLift};
 }
 static Vector3 TrianglePoint(Vector3 p,Vector3 a,Vector3 b,Vector3 c)
 {
  var ab=b-a;var ac=c-a;var ap=p-a;float d1=Vector3.Dot(ab,ap),d2=Vector3.Dot(ac,ap);if(d1<=0&&d2<=0)return a;
  var bp=p-b;float d3=Vector3.Dot(ab,bp),d4=Vector3.Dot(ac,bp);if(d3>=0&&d4<=d3)return b;
  float vc=d1*d4-d3*d2;if(vc<=0&&d1>=0&&d3<=0)return a+ab*(d1/(d1-d3));
  var cp=p-c;float d5=Vector3.Dot(ab,cp),d6=Vector3.Dot(ac,cp);if(d6>=0&&d5<=d6)return c;
  float vb=d5*d2-d1*d6;if(vb<=0&&d2>=0&&d6<=0)return a+ac*(d2/(d2-d6));
  float va=d3*d6-d5*d4;if(va<=0&&(d4-d3)>=0&&(d5-d6)>=0)return b+(c-b)*((d4-d3)/((d4-d3)+(d5-d6)));
  float sum=va+vb+vc;if(Mathf.Abs(sum)<1e-15f)return a;return a+ab*(vb/sum)+ac*(vc/sum);
 }
 static float Segments(Vector3 a,Vector3 b,Vector3 c,Vector3 d)
 {
  var u=b-a;var v=d-c;var w=a-c;float aa=Vector3.Dot(u,u),bb=Vector3.Dot(u,v),cc=Vector3.Dot(v,v),dd=Vector3.Dot(u,w),ee=Vector3.Dot(v,w);
  if(aa<1e-12f||cc<1e-12f)return Mathf.Min((a-c).sqrMagnitude,(b-d).sqrMagnitude);
  float det=aa*cc-bb*bb;float s=det>1e-12f?Mathf.Clamp01((bb*ee-cc*dd)/det):0;float t=(bb*s+ee)/cc;
  if(t<0){t=0;s=Mathf.Clamp01(-dd/aa);}else if(t>1){t=1;s=Mathf.Clamp01((bb-dd)/aa);}
  return (a+u*s-c-v*t).sqrMagnitude;
 }
 static float SegmentTriangle(Vector3 a,Vector3 b,Vector3 p,Vector3 q,Vector3 r)
 {
  var n=Vector3.Cross(q-p,r-p);float denom=Vector3.Dot(n,b-a);
  if(Mathf.Abs(denom)>1e-12f){float t=Vector3.Dot(n,p-a)/denom;if(t>=0&&t<=1){var at=Vector3.Lerp(a,b,t);if((TrianglePoint(at,p,q,r)-at).sqrMagnitude<1e-10f)return 0;}}
  float best=Mathf.Min((TrianglePoint(a,p,q,r)-a).sqrMagnitude,(TrianglePoint(b,p,q,r)-b).sqrMagnitude);
  best=Mathf.Min(best,Segments(a,b,p,q));best=Mathf.Min(best,Segments(a,b,q,r));return Mathf.Min(best,Segments(a,b,r,p));
 }
}
