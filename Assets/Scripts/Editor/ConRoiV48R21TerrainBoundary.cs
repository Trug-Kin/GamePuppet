using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class ConRoiV48R21TerrainBoundary
{
 const string Out="Docs/Reviews/journey_v48/r21/asset-pass",Art="Assets/Art/Environment/JourneyV48/LakeMarketBoundaryR21";
 public static void PlaceAndBuild()
 {
  EditorSceneManager.OpenScene("Assets/Scenes/JourneyRepair_v48_r21.unity");
  if(GameObject.Find("R21_Actual_Terrain_Perimeter"))throw new Exception("Preserve previous derivative");
  var story=UnityEngine.Object.FindAnyObjectByType<DieuPhoiHanhTrinh>();bool la=story.lake.activeSelf,ma=story.market.activeSelf;story.lake.SetActive(true);story.market.SetActive(true);
  var old=GameObject.Find("R21_LakeMarket_Measured_Perimeter");int retired=0;
  foreach(Transform t in old.transform)if(t.name.StartsWith("Outer_")){t.gameObject.SetActive(false);retired++;}
  var root=new GameObject("R21_Actual_Terrain_Perimeter");Physics.SyncTransforms();
  var data=JObject.Parse(File.ReadAllText(Out+"/terrain-union-perimeter.json"));
  var temp=GameObject.CreatePrimitive(PrimitiveType.Cube);var cube=temp.GetComponent<MeshFilter>().sharedMesh;UnityEngine.Object.DestroyImmediate(temp);
  var material=AssetDatabase.LoadAssetAtPath<Material>("Assets/Art/Environment/OngDaoV35/OD35_GreenMetal.mat");var placed=new JArray();var unsupported=new JArray();var castFailures=new JArray();int serial=0;
  bool Ground(Vector2 p,out float y)
  {
   var hits=Physics.RaycastAll(new Vector3(p.x,60,p.y),Vector3.down,120,8,QueryTriggerInteraction.Ignore).Where(h=>h.normal.y>.7f&&!h.collider.transform.IsChildOf(root.transform)&&!h.collider.transform.IsChildOf(old.transform)).OrderByDescending(h=>h.point.y).ToArray();y=hits.Length>0?hits[0].point.y:0;return hits.Length>0;
  }
  var colliders=new List<(MeshCollider c,Vector3 midpoint,Vector3 across)>();
  foreach(var row in data["panels"])
  {
   var a=new Vector2((float)row[0][0],(float)row[0][1]);var b=new Vector2((float)row[1][0],(float)row[1][1]);var mid=(a+b)/2;
   if(!Ground(a,out float ya)||!Ground(b,out float yb)||!Ground(mid,out float ym)){unsupported.Add(new JArray(mid.x,mid.y));continue;}
   float top=Mathf.Max(ya,yb,ym)+3.8f;var origin=new Vector3(mid.x,Mathf.Min(ya,yb)-.06f,mid.y);var pieces=new List<CombineInstance>();
   void Box(Vector3 p,Vector3 size,Quaternion rotation){pieces.Add(new CombineInstance{mesh=cube,transform=Matrix4x4.TRS(p-origin,rotation,size)});}
   void Post(Vector2 p,float y,float width){float bottom=y-.06f;Box(new Vector3(p.x,(bottom+top)/2,p.y),new Vector3(width,top-bottom,width),Quaternion.identity);}
   Post(a,ya,.075f);Post(b,yb,.075f);int bars=Mathf.CeilToInt(Vector2.Distance(a,b)/.16f);
   for(int j=1;j<bars;j++){float t=j/(float)bars;Post(Vector2.Lerp(a,b,t),Mathf.Lerp(ya,yb,t),.025f);}
   foreach(float y in new[]{top-.08f,top-1.1f,Mathf.Max(ya,yb,ym)+.16f})
   {var p=new Vector3(a.x,y,a.y);var q=new Vector3(b.x,y,b.y);Box((p+q)/2,new Vector3(.035f,Vector3.Distance(p,q),.035f),Quaternion.FromToRotation(Vector3.up,q-p));}
   var mesh=new Mesh{name="ActualTerrainFence_"+serial};mesh.CombineMeshes(pieces.ToArray(),true,true);mesh.RecalculateBounds();AssetDatabase.CreateAsset(mesh,Art+"/"+mesh.name+".asset");
   var g=new GameObject(mesh.name);g.layer=3;g.transform.SetParent(root.transform);g.transform.position=origin;g.AddComponent<MeshFilter>().sharedMesh=mesh;g.AddComponent<MeshRenderer>().sharedMaterial=material;var collider=g.AddComponent<MeshCollider>();collider.sharedMesh=mesh;
   GameObjectUtility.SetStaticEditorFlags(g,StaticEditorFlags.BatchingStatic);serial++;
   var direction=new Vector3(b.x-a.x,0,b.y-a.y).normalized;colliders.Add((collider,new Vector3(mid.x,ym,mid.y),Vector3.Cross(Vector3.up,direction)));
   placed.Add(new JObject{["a"]=new JArray(a.x,ya,a.y),["b"]=new JArray(b.x,yb,b.y),["top_y"]=top,["same_visible_collision_mesh"]=true});
  }
  Physics.SyncTransforms();
  foreach(var row in colliders)
  {
   var eye=row.midpoint+Vector3.up*.95f;
   // Per-panel collider intercept only: does not certify corners or jumping.
   if(!row.c.Raycast(new Ray(eye-row.across*1.5f,row.across),out _,3))castFailures.Add(new JArray(row.midpoint.x,row.midpoint.z));
  }
  var scenery=UnityEngine.Object.FindAnyObjectByType<CanhQuanChoV48>();scenery.scenery=scenery.scenery.Concat(new[]{root}).ToArray();
  story.lake.SetActive(la);story.market.SetActive(ma);EditorSceneManager.SaveScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene());AssetDatabase.SaveAssets();
  File.WriteAllText(Out+"/actual-terrain-boundary.json",new JObject{["status"]="ACTUAL_OUTLINE_DRAFT_REQUIRES_VISUAL_AND_NATIVE_TEST",["replaces_convex_envelope_panels"]=retired,["height_m"]=3.8,["panels"]=placed,["unsupported_midpoints"]=unsupported,["per_panel_ray_misses"]=castFailures,["shore_excluded_panels"]=data["shore_excluded"].Count(),["limits"]="Panel ray is not a capsule or jump test. Raster outline has 1m simplification tolerance. Shore/bridge low rails remain separate; no invisible walls, story locks or controller changes."}.ToString());
  ConRoiV48R21AssetPass.AuditBoundaries();ConRoiV48R21AssetPass.BuildReview();
 }
}
