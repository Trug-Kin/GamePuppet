using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
// Diagnostic only, never serialized into the product. Candidate transforms are not walkthrough evidence.
public sealed class R39GeometryProbe:MonoBehaviour {
 const string Out="Docs/Reviews/journey_v48/r39";
 static JArray V(Vector3 v)=>new JArray(v.x,v.y,v.z);
 bool Clear(Vector3 a,Vector3 b,Transform ignore,DieuPhoiHanhTrinh s){return !Physics.RaycastAll(a,(b-a).normalized,Vector3.Distance(a,b)-.04f,~0,QueryTriggerInteraction.Ignore).Any(h=>!h.transform.IsChildOf(s.player.transform)&&!h.transform.IsChildOf(ignore));}
 MeshFilter[] visibleMeshes;
 readonly Dictionary<Mesh,Mesh> readable=new Dictionary<Mesh,Mesh>();
 Mesh ReadMesh(Mesh source){
#if UNITY_EDITOR
  if(!source.isReadable){Mesh copy;if(readable.TryGetValue(source,out copy))return copy;
   using(var data=UnityEditor.MeshUtility.AcquireReadOnlyMeshData(source)){
    var vertices=new Unity.Collections.NativeArray<Vector3>(data[0].vertexCount,Unity.Collections.Allocator.Temp);data[0].GetVertices(vertices);copy=new Mesh();copy.indexFormat=UnityEngine.Rendering.IndexFormat.UInt32;copy.vertices=vertices.ToArray();vertices.Dispose();var indices=new List<int>();
    for(int sub=0;sub<data[0].subMeshCount;sub++){var count=data[0].GetSubMesh(sub).indexCount;var a=new Unity.Collections.NativeArray<int>(count,Unity.Collections.Allocator.Temp);data[0].GetIndices(a,sub);indices.AddRange(a.ToArray());a.Dispose();}copy.triangles=indices.ToArray();copy.RecalculateBounds();readable.Add(source,copy);return copy;
   }
  }
#endif
  return source;
 }
 JArray MeshPath(Vector3 a,Vector3 b,Transform ignore){
  var result=new JArray();
#if UNITY_EDITOR
  if(visibleMeshes==null)visibleMeshes=FindObjectsByType<MeshFilter>(FindObjectsSortMode.None).Where(m=>m.sharedMesh&&m.GetComponent<Renderer>()&&m.GetComponent<Renderer>().enabled).ToArray();
  var ray=new Ray(a,(b-a).normalized);float len=Vector3.Distance(a,b)-.035f;
  foreach(var m in visibleMeshes){if(!m||!m.gameObject.activeInHierarchy||m.transform.IsChildOf(ignore))continue;float entry;if(!m.GetComponent<Renderer>().bounds.IntersectRay(ray,out entry)||entry>len)continue;
   var mesh=ReadMesh(m.sharedMesh);var vertices=mesh.vertices;var indices=mesh.triangles;var inverse=m.transform.worldToLocalMatrix;var origin=inverse.MultiplyPoint3x4(a);var dir=inverse.MultiplyVector(ray.direction);float nearest=float.PositiveInfinity;
   for(int j=0;j<indices.Length;j+=3){var v0=vertices[indices[j]];var e1=vertices[indices[j+1]]-v0;var e2=vertices[indices[j+2]]-v0;var pv=Vector3.Cross(dir,e2);float det=Vector3.Dot(e1,pv);if(Mathf.Abs(det)<1e-12f)continue;float inv=1/det;var tv=origin-v0;float u=Vector3.Dot(tv,pv)*inv;if(u<0||u>1)continue;var qv=Vector3.Cross(tv,e1);float v=Vector3.Dot(dir,qv)*inv;if(v<0||u+v>1)continue;float t=Vector3.Dot(e2,qv)*inv;if(t>.001f&&t<len&&t<nearest)nearest=t;}
   if(float.IsPositiveInfinity(nearest))continue;
   result.Add(new JObject{["renderer"]=m.name,["mesh"]=m.sharedMesh.name,["point"]=V(ray.GetPoint(nearest)),["distance"]=nearest});
  }
#endif
  return result;
 }
 IEnumerator Start(){
  var s=FindAnyObjectByType<DieuPhoiHanhTrinh>();while(s.Loading)yield return null;
  s.SavePathOverride=Out+"/diagnostic-only-save.json";s.SetMenu(false);s.data.region=0;typeof(DieuPhoiHanhTrinh).GetMethod("ApplyRegion",System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.NonPublic).Invoke(s,new object[]{true});yield return null;yield return null;
  var b=s.GetComponent<TuTheBaoBenCauV48>();var r=s.GetComponent<PhanChieuMatHo>();var n=s.GetComponent<TuongTacSoTayHoR37>();var eye=s.player.GetComponentInChildren<Camera>();var p=s.puppets[0];
  File.WriteAllText(Out+"/initial.json",new JObject{["eye"]=V(eye.transform.position),["player"]=V(s.player.transform.position),["bao"]=V(b.actorPosition),["rail"]=V(b.railContact),["waterDirection"]=V(b.waterDirection),["footAnchor"]=V(s.GetComponent<MoDauBenHoR23>().lakeFootAnchor),["puppetRoot"]=V(p.transform.position),["puppetHead"]=V(p.dauTransform.position),["waterBoundsMin"]=V(r.water.bounds.min),["waterBoundsMax"]=V(r.water.bounds.max),["waterMesh"]=r.water.GetComponent<MeshFilter>().sharedMesh.name}.ToString());
  s.player.enabled=false;s.player.GetComponent<DieuKhienGocNhin>().enabled=false;s.GetComponent<MoDauBenHoR23>().enabled=false;p.enabled=false;
  if(Array.IndexOf(Environment.GetCommandLineArgs(),"--r39-visual")>=0){yield return Visual(s,b,r,n,eye,p);yield break;}
  var grounds=new List<Vector3>();var rows=new JArray();
  for(float x=634;x<=707;x+=1.5f)for(float z=478;z<=537;z+=1.5f){
   var hits=Physics.RaycastAll(new Vector3(x,12,z),Vector3.down,14,~0,QueryTriggerInteraction.Ignore).OrderBy(h=>h.distance);
   var h=hits.FirstOrDefault(a=>a.collider.gameObject.layer==3&&a.point.y>.15f&&!a.transform.IsChildOf(s.player.transform)&&!a.transform.IsChildOf(s.hub.transform)&&!a.transform.IsChildOf(s.lake.transform));
   if(!h.collider||h.normal.y<.85f)continue;
   var pt=h.point;bool safe=!Physics.CheckCapsule(pt+Vector3.up*.4f,pt+Vector3.up*1.45f,.31f,~(1<<4),QueryTriggerInteraction.Ignore);
   rows.Add(new JObject{["point"]=V(pt),["support"]=h.collider.name,["safeCapsule"]=safe});if(safe)grounds.Add(pt);
  }
  File.WriteAllText(Out+"/ground-grid.json",rows.ToString());
  float waterY=r.water.bounds.center.y;var candidates=new JArray();
  foreach(var stand in grounds.Where(a=>Vector3.Distance(a,b.actorPosition)<30))foreach(var obj in grounds.Where(a=>Vector3.Distance(a,b.actorPosition)<25)){
   float dist=Vector3.Distance(stand,obj);if(dist<3||dist>16)continue;
   var e=stand+Vector3.up*1.65f;var head=obj+Vector3.up*1.6f;var mirror=head;mirror.y=2*waterY-head.y;float t=(waterY-e.y)/(mirror.y-e.y);var water=Vector3.LerpUnclamped(e,mirror,t);
   // Physical two-leg optical path, with the real deck/rail/colliders retained.
   if(!Clear(e,water,p.transform,s)||!Clear(water+Vector3.up*.03f,head,p.transform,s)||!Clear(e,head,p.transform,s))continue;
   var down=Physics.RaycastAll(water+Vector3.up*.12f,Vector3.down,.5f,~0,QueryTriggerInteraction.Ignore);
   if(down.Any(h=>h.collider.gameObject.layer==3))continue;
   candidates.Add(new JObject{["stand"]=V(stand),["puppetFeet"]=V(obj),["distance"]=dist,["waterPoint"]=V(water),["distanceFromBao"]=Vector3.Distance(stand,b.actorPosition)});
  }
  File.WriteAllText(Out+"/optical-candidates.json",candidates.ToString());
  var meshChecked=new JArray();foreach(var row in candidates){var e=Vec(row["stand"])+Vector3.up*1.65f;var h=Vec(row["puppetFeet"])+Vector3.up*1.6f;var w=Vec(row["waterPoint"]);var eyeHits=MeshPath(e,w,p.transform);var waterHits=MeshPath(w+Vector3.up*.03f,h,p.transform);meshChecked.Add(new JObject{["candidate"]=row.DeepClone(),["eyeToWater"]=eyeHits,["waterToHead"]=waterHits,["clear"]=eyeHits.Count==0&&waterHits.Count==0});}File.WriteAllText(Out+"/rendered-mesh-candidates.json",meshChecked.ToString());
  eye.transform.position=b.actorPosition+Vector3.up*45;eye.transform.rotation=Quaternion.Euler(90,0,0);eye.orthographic=true;eye.orthographicSize=28;eye.farClipPlane=150;yield return null;Capture(eye,Out+"/map-top.png");
  Debug.Log("R39_DIAG_DONE ground="+grounds.Count+" candidates="+candidates.Count);
#if UNITY_EDITOR
  UnityEditor.EditorApplication.Exit(0);
#endif
 }
 static Vector3 Vec(JToken t)=>new Vector3((float)t[0],(float)t[1],(float)t[2]);
 IEnumerator Visual(DieuPhoiHanhTrinh s,TuTheBaoBenCauV48 b,PhanChieuMatHo r,TuongTacSoTayHoR37 n,Camera eye,DieuKhienConRoi puppet){
  var feet=puppet.dauTransform.position-Vector3.up*1.58f;var goal=new Vector3(656.5f,.213f,503.5f);puppet.transform.position+=goal-feet;puppet.GetComponent<BuocChanConRoiR23>().ResetPose();
  var ui=FindObjectsByType<Canvas>(FindObjectsSortMode.None);foreach(var c in ui)c.enabled=false;
  var stands=new[]{new Vector3(659.5f,1.14009f,511),new Vector3(661,1.14009f,509.5f),new Vector3(658.7f,1.14009f,511.6f)};
  for(int i=0;i<stands.Length;i++){
   eye.transform.position=stands[i]+Vector3.up*1.65f;eye.transform.rotation=Quaternion.LookRotation(new Vector3(656.5f,.55f,503.5f)-eye.transform.position);yield return null;yield return null;yield return null;
   yield return new WaitForEndOfFrame();Capture(eye,Out+"/candidate-"+i+".png");File.WriteAllText(Out+"/candidate-"+i+".json",R38ReflectionProbe.Sample(s).ToString());
   var flags=System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.NonPublic;
   var rc=(Camera)typeof(PhanChieuMatHo).GetField("reflection",flags).GetValue(r);var rt=(RenderTexture)typeof(PhanChieuMatHo).GetField("texture",flags).GetValue(r);
   var old=RenderTexture.active;RenderTexture.active=rt;var tex=new Texture2D(rt.width,rt.height,TextureFormat.RGB24,false);tex.ReadPixels(new Rect(0,0,rt.width,rt.height),0,0);tex.Apply();File.WriteAllBytes(Out+"/candidate-"+i+"-raw.png",tex.EncodeToPNG());Destroy(tex);RenderTexture.active=old;
   r.water.material.SetMatrix("_LakeReflectionVP",GL.GetGPUProjectionMatrix(rc.projectionMatrix,false)*rc.worldToCameraMatrix);Capture(eye,Out+"/candidate-"+i+"-unflipped.png");r.water.material.SetFloat("_LakeReflectionReady",0);Capture(eye,Out+"/candidate-"+i+"-water-without-reflection.png");
   var h=puppet.dauTransform.position;var virtualP=h;virtualP.y=2*r.water.bounds.center.y-h.y;var w=Vector3.LerpUnclamped(eye.transform.position,virtualP,(r.water.bounds.center.y-eye.transform.position.y)/(virtualP.y-eye.transform.position.y));
   File.WriteAllText(Out+"/candidate-"+i+"-rendered-triangles.json",new JObject{["eyeToWater"]=MeshPath(eye.transform.position,w,puppet.transform),["waterToHead"]=MeshPath(w+Vector3.up*.03f,h,puppet.transform),["waterSurfaceAtPoint"]=MeshPath(w+Vector3.up*5,w-Vector3.up*.1f,puppet.transform)}.ToString());
  }
#if UNITY_EDITOR
  UnityEditor.EditorApplication.Exit(0);
#endif
 }
 static void Capture(Camera c,string path){var rt=RenderTexture.GetTemporary(1600,1200,24);var old=RenderTexture.active;var tex=new Texture2D(1600,1200,TextureFormat.RGB24,false);try{RenderPipeline.SubmitRenderRequest(c,new UniversalRenderPipeline.SingleCameraRequest{destination=rt});RenderTexture.active=rt;tex.ReadPixels(new Rect(0,0,1600,1200),0,0);tex.Apply();File.WriteAllBytes(path,tex.EncodeToPNG());}finally{RenderTexture.active=old;RenderTexture.ReleaseTemporary(rt);Destroy(tex);}}
}
