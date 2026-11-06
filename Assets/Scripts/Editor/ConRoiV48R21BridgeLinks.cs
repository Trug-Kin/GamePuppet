using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

// Retain the authored two bridge-to-path connectors, matching their low
// portions to the closed earthwork rather than leaving them half buried.
public static class ConRoiV48R21BridgeLinks
{
 const string Art="Assets/Art/Environment/JourneyV48/LandscapeR21",Out="Docs/Reviews/journey_v48/r21";
 public static void RepairAndBuildDraft()
 {
  EditorSceneManager.OpenScene("Assets/Scenes/JourneyRepair_v48_r21.unity");
  if(GameObject.Find("R21_Conformed_Authored_Bridge_Links"))throw new Exception("Preserve the existing link derivative; do not stack repairs");
  var source=JObject.Parse(File.ReadAllText(Out+"/source-cross-section-geometry.json"))["objects"].Where(o=>((string)o["name"]).StartsWith("OD35_Main_Route_Side_Link")).ToArray();
  var sourcePoints=source.SelectMany(o=>o["vertices"]).Select(p=>new Vector3((float)p[0],(float)p[1],(float)p[2])).ToArray();
  bool Match(Vector3 p)=>sourcePoints.Any(q=>Mathf.Abs(p.x-q.x)<.002f&&Mathf.Abs(p.z-q.z)<.002f&&Mathf.Abs(p.y-q.y)<.003f);
  bool MatchXZ(Vector3 p)=>sourcePoints.Any(q=>Mathf.Abs(p.x-q.x)<.002f&&Mathf.Abs(p.z-q.z)<.002f);
  bool Selected(Vector3[] points,int[] triangles,int start)=>Match(points[triangles[start]])&&Match(points[triangles[start+1]])&&Match(points[triangles[start+2]]);
  var owner=GameObject.Find("R11_Local_Bridge_Supports").GetComponentsInChildren<MeshFilter>(true).Single(f=>f.name=="OD35_Surface_0");
  var original=owner.sharedMesh;var world=original.vertices.Select(owner.transform.TransformPoint).ToArray();var triangles=original.triangles;var selected=new List<int>();var retained=new List<int>();
  for(int i=0;i<triangles.Length;i+=3)(Selected(world,triangles,i)?selected:retained).AddRange(new[]{triangles[i],triangles[i+1],triangles[i+2]});
  int expected=source.Sum(o=>o["triangles"].Count());
  if(selected.Count/3!=expected)throw new Exception("Authored bridge link face attribution mismatch: "+selected.Count/3+" versus "+expected);
  var remainder=UnityEngine.Object.Instantiate(original);remainder.name="R21_OD35_Paving_Without_Link_Duplicates";remainder.triangles=retained.ToArray();remainder.RecalculateBounds();AssetDatabase.CreateAsset(remainder,Art+"/"+remainder.name+".asset");owner.sharedMesh=remainder;
  // V36 already smoothed the SE collision grade. Later visual re-exports kept
  // the V35 heights while retaining that V36 collider. Preserve the verified
  // collision grade as the baseline for the now shared visible/collision mesh.
  var baseline=UnityEngine.Object.FindObjectsByType<MeshCollider>(FindObjectsInactive.Include).Where(c=>c.name=="OD35_COLLIDER_11"||c.name=="OD35_COLLIDER_12").SelectMany(c=>c.sharedMesh.vertices.Select(c.transform.TransformPoint)).ToArray();
  var patchWorld=(Vector3[])world.Clone();int historicalGradeMatches=0;
  foreach(int index in selected.Distinct())
  {
   var matches=baseline.Where(q=>Mathf.Abs(q.x-world[index].x)<.002f&&Mathf.Abs(q.z-world[index].z)<.002f).ToArray();
   if(matches.Length==0)throw new Exception("Missing historical link collision grade");
   patchWorld[index]=new Vector3(world[index].x,matches[0].y,world[index].z);historicalGradeMatches++;
  }
  var patch=new Mesh{indexFormat=UnityEngine.Rendering.IndexFormat.UInt32,name="Authored_Bridge_Links_Source"};patch.vertices=patchWorld;patch.uv=original.uv;patch.triangles=selected.ToArray();patch.RecalculateNormals();patch.RecalculateBounds();
  var root=new GameObject("R21_Conformed_Authored_Bridge_Links");root.layer=3;
  var terrain=GameObject.Find("R20_Whole_Source_Export").GetComponentsInChildren<MeshFilter>().Single(f=>f.name=="Surface_142");var ground=terrain.GetComponent<MeshCollider>();Physics.SyncTransforms();
  var dense=ConRoiV48R21Footpaths.DensifyLocal(patch,root.transform);UnityEngine.Object.DestroyImmediate(patch);
  var mesh=ConRoiV48R21PathConform.Split(dense,root.transform,terrain);UnityEngine.Object.DestroyImmediate(dense);
  var vertices=mesh.vertices;var changes=new JArray();int missing=0;
  for(int i=0;i<vertices.Length;i++)
  {
   var p=vertices[i];if(!ground.Raycast(new Ray(new Vector3(p.x,30,p.z),Vector3.down),out var hit,70)){missing++;continue;}
   if(p.y>=hit.point.y+.025f)continue;
   vertices[i]=new Vector3(p.x,hit.point.y+.025f,p.z);changes.Add(new JObject{["x"]=p.x,["z"]=p.z,["before_y"]=p.y,["after_y"]=vertices[i].y});
  }
  if(missing!=0)throw new Exception("Bridge link projection has missing earth support; do not save an incomplete repair");
  mesh.name="R21_Conformed_Authored_Bridge_Links";mesh.vertices=vertices;mesh.RecalculateNormals();mesh.RecalculateBounds();AssetDatabase.CreateAsset(mesh,Art+"/"+mesh.name+".asset");
  root.AddComponent<MeshFilter>().sharedMesh=mesh;root.AddComponent<MeshRenderer>().sharedMaterial=owner.GetComponent<Renderer>().sharedMaterial;root.AddComponent<MeshCollider>().sharedMesh=mesh;
  var collision=new JArray();
  foreach(var collider in UnityEngine.Object.FindObjectsByType<MeshCollider>(FindObjectsInactive.Include).Where(c=>c.name=="OD35_COLLIDER_11"||c.name=="OD35_COLLIDER_12"))
  {
   var before=collider.sharedMesh;if(!before)continue;var points=before.vertices.Select(collider.transform.TransformPoint).ToArray();var faces=before.triangles;var keep=new List<int>();int removed=0;
   for(int i=0;i<faces.Length;i+=3){if(MatchXZ(points[faces[i]])&&MatchXZ(points[faces[i+1]])&&MatchXZ(points[faces[i+2]])){removed++;continue;}keep.AddRange(new[]{faces[i],faces[i+1],faces[i+2]});}
   if(removed==0)continue;
   if(keep.Count==0){collider.enabled=false;collider.sharedMesh=null;}
   else{var after=UnityEngine.Object.Instantiate(before);after.name="R21_"+collider.name+"_Without_Old_Link_Faces";after.triangles=keep.ToArray();after.RecalculateBounds();AssetDatabase.CreateAsset(after,Art+"/"+after.name+".asset");collider.sharedMesh=after;}
   collision.Add(new JObject{["name"]=collider.name,["removed_old_link_faces"]=removed,["remaining_faces"]=keep.Count/3});
  }
  File.WriteAllText(Out+"/bridge-link-collision-attribution.json",new JObject{["expected_visible_faces"]=expected,["colliders"]=collision}.ToString());
  if(collision.Sum(o=>(int)o["removed_old_link_faces"])!=expected)throw new Exception("Old bridge link collision attribution does not match visible faces; see bridge-link-collision-attribution.json");
  var scenery=UnityEngine.Object.FindAnyObjectByType<CanhQuanChoV48>();scenery.scenery=scenery.scenery.Concat(new[]{root}).ToArray();
  EditorSceneManager.SaveScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene());AssetDatabase.SaveAssets();
  File.WriteAllText(Out+"/authored-bridge-links-draft.json",new JObject{["status"]="DRAFT_REQUIRES_BUILD_VISUAL_AND_NATIVE_WALK",["source_faces"]=expected,["historical_grade_matched_vertices"]=historicalGradeMatches,["baseline"]="Existing NW/V36 smoothed SE colliders, preserving previous continuous grade before terrain clearance",["preserved_xz_footprint_and_source_uv"]=true,["terrain_triangle_splitting"]=true,["render_and_collider_same_mesh"]=true,["old_link_collision_removed"]=collision,["changes"]=changes}.ToString());
  ConRoiV48R21Boundaries.SchoolAndSlopeDraft();ConRoiV48R21.BuildWorking();
 }
 public static void DisableEmptyRetiredCollidersAndBuild()
 {
  EditorSceneManager.OpenScene("Assets/Scenes/JourneyRepair_v48_r21.unity");
  var replacement=GameObject.Find("R21_Conformed_Authored_Bridge_Links");
  if(!replacement||replacement.GetComponent<MeshCollider>().sharedMesh!=replacement.GetComponent<MeshFilter>().sharedMesh)throw new Exception("Shared visible/collision replacement is required before retiring the old colliders");
  var rows=new JArray();
  foreach(var collider in UnityEngine.Object.FindObjectsByType<MeshCollider>(FindObjectsInactive.Include).Where(c=>c.name=="OD35_COLLIDER_11"||c.name=="OD35_COLLIDER_12"))
  {
   if(collider.sharedMesh&&collider.sharedMesh.triangles.Length!=0)throw new Exception("Refuse to disable nonempty old collision geometry");
   collider.enabled=false;collider.sharedMesh=null;rows.Add(collider.name);
  }
  if(rows.Count!=2)throw new Exception("Expected exactly two retired link colliders");
  EditorSceneManager.SaveScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene());AssetDatabase.SaveAssets();
  File.WriteAllText(Out+"/retired-link-collider-cleanup.json",new JObject{["reason"]="Build log reported empty old link meshes as invalid collision meshes; disable the retired components rather than assigning zero-face meshes",["retired_components"]=rows,["replacement_shared_render_collider_mesh"]=AssetDatabase.GetAssetPath(replacement.GetComponent<MeshFilter>().sharedMesh)}.ToString());
  ConRoiV48R21.BuildWorking();
 }
}
