using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class ConRoiV48R21BoundaryCapsule
{
 const string Out="Docs/Reviews/journey_v48/r21/asset-pass";
 public static void InspectCastMisses()
 {
  EditorSceneManager.OpenScene("Assets/Scenes/JourneyRepair_v48_r21.unity");
  var story=UnityEngine.Object.FindAnyObjectByType<DieuPhoiHanhTrinh>();story.lake.SetActive(true);story.market.SetActive(true);Physics.SyncTransforms();
  var root=GameObject.Find("R21_Actual_Terrain_Perimeter");var source=JObject.Parse(File.ReadAllText(Out+"/boundary-capsule-intercepts.json"));var rows=new JArray();
  foreach(var miss in source["misses"])
  {
   var q=miss["point"];var p=new Vector3((float)q[0],(float)q[1],(float)q[2]);
   var hits=Physics.OverlapCapsule(p+Vector3.up*.34f,p+Vector3.up*1.52f,.28f,8,QueryTriggerInteraction.Ignore).Where(c=>c.transform.IsChildOf(root.transform)).ToArray();
   rows.Add(new JObject{["point"]=q,["midpoint_capsule_overlaps_visible_boundary"]=hits.Length>0,["colliders"]=new JArray(hits.Select(c=>c.name))});
  }
  File.WriteAllText(Out+"/boundary-capsule-miss-inspection.json",new JObject{["status"]="SUPPLEMENTAL_OVERLAP_AT_CAST_MISS_LOCATIONS_NOT_NATIVE_ACCEPTANCE",["locations"]=rows,["limits"]="Overlap at barrier centre diagnoses the local missed cast; it does not establish a traversable approach, jumping or containment."}.ToString());
 }
 public static void FinishAndAudit()
 {
  EditorSceneManager.OpenScene("Assets/Scenes/JourneyRepair_v48_r21.unity");
  var story=UnityEngine.Object.FindAnyObjectByType<DieuPhoiHanhTrinh>();bool la=story.lake.activeSelf,ma=story.market.activeSelf;story.lake.SetActive(true);story.market.SetActive(true);
  var root=GameObject.Find("R21_Actual_Terrain_Perimeter");var old=GameObject.Find("R21_LakeMarket_Measured_Perimeter");Physics.SyncTransforms();
  var audit=JObject.Parse(File.ReadAllText(Out+"/actual-terrain-boundary.json"));var gaps=audit["unsupported_midpoints"].Select(p=>new Vector2((float)p[0],(float)p[1])).ToArray();
  var terrainProbes=ConRoiV48R21TerrainGround.CreateProbes();Physics.SyncTransforms();
  bool Ground(Vector2 p,out float y)
  {
   return ConRoiV48R21TerrainGround.Sample(terrainProbes,p,out y);
  }
  var pierReport=new JObject{["created"]=false};bool changed=false;
  if(gaps.Length==2&&Vector2.Distance(gaps[0],gaps[1])<1&&!root.transform.Find("Grounded_Corner_Service_Pier"))
  {
   var centre=(gaps[0]+gaps[1])/2;var candidates=new List<Vector2>();for(int x=-5;x<=5;x++)for(int z=-5;z<=5;z++)candidates.Add(centre+new Vector2(x,z)*.2f);
   foreach(float width in new[]{1.4f,1.8f})
   {
    foreach(var p in candidates.OrderBy(p=>Vector2.Distance(p,centre)))
    {
     if(gaps.Any(q=>Mathf.Abs(q.x-p.x)>width/2+.15f||Mathf.Abs(q.y-p.y)>width/2+.15f))continue;
     var samples=new[]{p,p+new Vector2(-width/2,-width/2),p+new Vector2(-width/2,width/2),p+new Vector2(width/2,-width/2),p+new Vector2(width/2,width/2)};var heights=new List<float>();
     foreach(var q in samples)if(Ground(q,out float y))heights.Add(y);
     if(heights.Count!=5||heights.Max()-heights.Min()>.8f)continue;
     float bottom=heights.Min()-.04f,top=heights.Max()+3.8f;
     var g=GameObject.CreatePrimitive(PrimitiveType.Cube);g.name="Grounded_Corner_Service_Pier";g.layer=3;g.transform.SetParent(root.transform);g.transform.position=new Vector3(p.x,(bottom+top)/2,p.y);g.transform.localScale=new Vector3(width,top-bottom,width);
     g.GetComponent<Renderer>().sharedMaterial=AssetDatabase.LoadAssetAtPath<Material>("Assets/Art/Environment/OngDaoV35/OD35_Concrete.mat");
     pierReport=new JObject{["created"]=true,["position"]=new JArray(p.x,(bottom+top)/2,p.y),["size"]=new JArray(width,top-bottom,width),["five_ground_heights"]=new JArray(heights),["collider_matches_visible_cube"]=true};changed=true;break;
    }
    if(changed)break;
   }
  }
  Physics.SyncTransforms();var misses=new JArray();int tested=0;var samplesAudit=new JArray();
  var allRows=audit["panels"].Select(p=>(a:new Vector3((float)p["a"][0],(float)p["a"][1],(float)p["a"][2]),b:new Vector3((float)p["b"][0],(float)p["b"][1],(float)p["b"][2]))).ToArray();
  void Test(Vector3 mid,Vector3 normal,string kind)
  {
   bool success=true;
   foreach(float sign in new[]{-1f,1f})
   {
    var start=mid+normal*(sign*1.5f);var direction=normal*(-sign);
    bool hit=Physics.CapsuleCastAll(start+Vector3.up*.34f,start+Vector3.up*1.52f,.28f,direction,3,8,QueryTriggerInteraction.Ignore).Any(h=>h.collider.transform.IsChildOf(root.transform));success&=hit;
   }
   tested++;if(!success)misses.Add(new JObject{["kind"]=kind,["point"]=new JArray(mid.x,mid.y,mid.z)});
  }
  foreach(var p in allRows)
  {
   var mid=(p.a+p.b)/2;var direction=p.b-p.a;direction.y=0;var normal=Vector3.Cross(Vector3.up,direction.normalized);
   if(Ground(new Vector2(mid.x,mid.z),out float y))mid.y=y;Test(mid,normal,"panel_midpoint");
   Test(p.a,normal,"panel_endpoint");
  }
  foreach(var gap in gaps)
  {
   var nearest=allRows.OrderBy(p=>Vector2.Distance(new Vector2(p.a.x,p.a.z),gap)).First();var direction=nearest.b-nearest.a;direction.y=0;
   var p=new Vector3(gap.x,Mathf.Max(nearest.a.y,nearest.b.y),gap.y);if(Ground(gap,out float y))p.y=y;Test(p,Vector3.Cross(Vector3.up,direction.normalized),"previously_unsupported_outline_sample");
  }
  var protectedPoints=JObject.Parse(File.ReadAllText(Out+"/envelope-survey.json"))["protected_interactions"];
  foreach(var row in protectedPoints)
  {
   var p=new Vector2((float)row["position"][0],(float)row["position"][2]);float min=float.PositiveInfinity;
   foreach(var edge in allRows){var a=new Vector2(edge.a.x,edge.a.z);var b=new Vector2(edge.b.x,edge.b.z);var v=b-a;var q=a+v*Mathf.Clamp01(Vector2.Dot(p-a,v)/v.sqrMagnitude);min=Mathf.Min(min,Vector2.Distance(p,q));}
   samplesAudit.Add(new JObject{["name"]=row["name"],["nearest_outer_fence_m"]=min});
  }
  ConRoiV48R21TerrainGround.Dispose(terrainProbes);story.lake.SetActive(la);story.market.SetActive(ma);
  if(changed){EditorSceneManager.SaveScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene());AssetDatabase.SaveAssets();}
  File.WriteAllText(Out+"/boundary-capsule-intercepts.json",new JObject{["status"]="SUPPLEMENTAL_STATIC_CAPSULE_CASTS_NOT_NATIVE_ACCEPTANCE",["radius_m"]=.28,["capsule_sphere_centres_above_ground_m"]=new JArray(.34,1.52),["tested_locations"]=tested,["tested_both_directions"]=true,["misses"]=misses,["corner_pier"]=pierReport,["protected_interaction_clearances"]=samplesAudit,["limits"]="Local casts do not establish connected reachability or native controller behaviour, jumping, falling, views or story progression. Shore and bridge low rail jumping remains OPEN."}.ToString());
  if(changed)ConRoiV48R21AssetPass.BuildReview();
 }
}
