using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

// Read-only attribution of the visible layers at the SE approach. Temporary
// colliders are removed without saving the scene; player data is never loaded.
public static class ConRoiV48R21ApproachLayers
{
 public static void Audit()
 {
  EditorSceneManager.OpenScene("Assets/Scenes/JourneyRepair_v48_r21.unity");
  var filters=UnityEngine.Object.FindObjectsByType<MeshFilter>(FindObjectsInactive.Include).Where(f=>f.gameObject.activeInHierarchy).ToArray();
  var rows=new JArray();var objects=new JArray();
  foreach(var f in filters)
  {
   var renderer=f.GetComponent<Renderer>();if(!renderer||!renderer.enabled||!f.sharedMesh)continue;
   var b=f.sharedMesh.bounds;var corners=new[]{new Vector3(b.min.x,b.min.y,b.min.z),new Vector3(b.max.x,b.max.y,b.max.z),new Vector3(b.min.x,b.max.y,b.max.z),new Vector3(b.max.x,b.min.y,b.min.z),new Vector3(b.min.x,b.min.y,b.max.z),new Vector3(b.max.x,b.max.y,b.min.z),new Vector3(b.min.x,b.max.y,b.min.z),new Vector3(b.max.x,b.min.y,b.max.z)}.Select(f.transform.TransformPoint).ToArray();
   if(corners.Max(p=>p.x)<614||corners.Min(p=>p.x)>648||corners.Max(p=>p.z)<535||corners.Min(p=>p.z)>565)continue;
   var temp=new GameObject("ReadOnly_Layer_Probe");temp.transform.SetPositionAndRotation(f.transform.position,f.transform.rotation);temp.transform.localScale=f.transform.lossyScale;
   var collider=temp.AddComponent<MeshCollider>();collider.sharedMesh=f.sharedMesh;Physics.SyncTransforms();
   int count=0;
   for(int x=614;x<=648;x++)for(int z=535;z<=565;z++)
   {
    if(!collider.Raycast(new Ray(new Vector3(x,30,z),Vector3.down),out var hit,70)||hit.normal.y<.3f)continue;
    rows.Add(new JObject{["x"]=x,["z"]=z,["y"]=hit.point.y,["name"]=f.name,["triangle"]=hit.triangleIndex});count++;
   }
   if(count>0)objects.Add(new JObject{["name"]=f.name,["mesh"]=AssetDatabase.GetAssetPath(f.sharedMesh),["material"]=renderer.sharedMaterial?renderer.sharedMaterial.name:"",["sample_hits"]=count});
   UnityEngine.Object.DestroyImmediate(temp);
  }
  File.WriteAllText("Docs/Reviews/journey_v48/r21/se-visible-layer-audit.json",new JObject{["status"]="READ_ONLY_EDITOR_ATTRIBUTION_NOT_BUILD_ACCEPTANCE",["objects"]=objects,["hits"]=rows}.ToString());
 }
 public static void CleanBoundariesAndAudit()
 {
  ConRoiV48R21Boundaries.SchoolAndSlopeDraft();Audit();
 }
 public static void RetireSupersededShoulderAndBuild()
 {
  EditorSceneManager.OpenScene("Assets/Scenes/JourneyRepair_v48_r21.unity");
  var shoulder=UnityEngine.Object.FindObjectsByType<MeshFilter>(FindObjectsInactive.Include).Single(f=>f.name=="SE_Graded_Shoulder_Visible_And_Collider");
  var terrain=GameObject.Find("R20_Whole_Source_Export").GetComponentsInChildren<MeshFilter>().Single(f=>f.name=="Surface_142");
  var ground=terrain.GetComponent<MeshCollider>();Physics.SyncTransforms();
  var rows=new JArray();int missing=0;float minY=float.PositiveInfinity,maxY=float.NegativeInfinity;
  // Sample the original patch, not just its corner vertices. Replacing its
  // support with a closed terrain mesh must precede retiring its collider.
  var verts=shoulder.sharedMesh.vertices;var tris=shoulder.sharedMesh.triangles;
  for(int i=0;i<tris.Length;i+=3)
  {
   var a=shoulder.transform.TransformPoint(verts[tris[i]]);var b=shoulder.transform.TransformPoint(verts[tris[i+1]]);var c=shoulder.transform.TransformPoint(verts[tris[i+2]]);
   foreach(var p in new[]{a,b,c,(a+b+c)/3})
   {
    bool hit=ground.Raycast(new Ray(new Vector3(p.x,30,p.z),Vector3.down),out var h,70);
    if(!hit)missing++;else{minY=Mathf.Min(minY,h.point.y);maxY=Mathf.Max(maxY,h.point.y);}
    rows.Add(new JObject{["x"]=p.x,["z"]=p.z,["ground_present"]=hit,["terrain_y"]=hit?new JValue(h.point.y):JValue.CreateNull()});
   }
  }
  if(missing!=0)throw new Exception("Do not retire the old shoulder while its underlying terrain has missing support");
  shoulder.gameObject.SetActive(false); // Retained for provenance, render and collider disabled together.
  EditorSceneManager.SaveScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene());AssetDatabase.SaveAssets();
  File.WriteAllText("Docs/Reviews/journey_v48/r21/retired-shoulder-draft.json",new JObject{["status"]="DRAFT_REQUIRES_BUILD_IMAGES_AND_NATIVE_WALK",["object"]=shoulder.name,["reason"]="V36 raised shoulder overlaps current approach roads and the locally closed r21 terrain, leaving disjoint paving wedges",["original_mesh_preserved"]=AssetDatabase.GetAssetPath(shoulder.sharedMesh),["render_and_collider_disabled_together"]=true,["underlying_terrain_matches_collider"]=terrain.sharedMesh==ground.sharedMesh,["missing_ground_samples"]=missing,["ground_min_y"]=minY,["ground_max_y"]=maxY,["samples"]=rows}.ToString());
  ConRoiV48R21.BuildWorking();
 }
}
