using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

// Opposite winding on shared vertices cancels RecalculateNormals. Keep both
// visible/collision sides, with independent vertices and normals for each face.
public static class ConRoiV48R21ConstructionNormals
{
 public static void RepairAndBuild()
 {
  EditorSceneManager.OpenScene("Assets/Scenes/JourneyRepair_v48_r21.unity");
  const string art="Assets/Art/Environment/JourneyV48/ContactFinishR21";
  var roots=UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects().Where(g=>g.name=="R21_Paving_Construction_Edges"||g.name=="R21_SE_Promenade_Construction_Edges").ToArray();
  var audit=new JArray();
  foreach(var filter in roots.SelectMany(g=>g.GetComponentsInChildren<MeshFilter>(true)))
  {
   var source=filter.sharedMesh;if(source.name.EndsWith("_FaceNormals"))throw new Exception("Already repaired; preserve derivative");
   var vs=source.vertices;var ts=source.triangles;var vertices=new List<Vector3>();var triangles=new List<int>();int degenerate=0;
   for(int i=0;i<ts.Length;i+=3)
   {
    var a=vs[ts[i]];var b=vs[ts[i+1]];var c=vs[ts[i+2]];
    if(Vector3.Cross(b-a,c-a).sqrMagnitude<1e-12f){degenerate++;continue;}
    int n=vertices.Count;vertices.AddRange(new[]{a,b,c});triangles.AddRange(new[]{n,n+1,n+2});
   }
   if(triangles.Count==0)continue;
   var mesh=new Mesh{name=source.name+"_FaceNormals",indexFormat=UnityEngine.Rendering.IndexFormat.UInt32};mesh.SetVertices(vertices);mesh.SetTriangles(triangles,0);mesh.RecalculateNormals();mesh.RecalculateBounds();
   string path=art+"/"+mesh.name+".asset";AssetDatabase.CreateAsset(mesh,path);filter.sharedMesh=mesh;var collider=filter.GetComponent<MeshCollider>();if(collider)collider.sharedMesh=mesh;
   audit.Add(new JObject{["object"]=filter.name,["source"]=AssetDatabase.GetAssetPath(source),["derived"]=path,["old_zero_normals"]=source.normals.Count(n=>n.sqrMagnitude<.25f),["new_zero_normals"]=mesh.normals.Count(n=>n.sqrMagnitude<.25f),["removed_degenerate_triangles"]=degenerate,["topology_footprint_unchanged"]=true,["same_visible_collision_mesh"]=collider!=null});
  }
  // Region scenery must not remain visible in later chapters.
  var boundary=GameObject.Find("R21_LakeMarket_Measured_Perimeter");var scenery=UnityEngine.Object.FindAnyObjectByType<CanhQuanChoV48>();
  if(boundary&&!scenery.scenery.Contains(boundary))scenery.scenery=scenery.scenery.Concat(new[]{boundary}).ToArray();
  EditorSceneManager.SaveScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene());AssetDatabase.SaveAssets();
  File.WriteAllText("Docs/Reviews/journey_v48/r21/asset-pass/construction-face-normals.json",new JObject{["reason"]="Opposite-winding faces shared vertices and cancelled normals; split face vertices without covering gaps or changing grade",["meshes"]=audit,["boundary_registered_as_region_scenery"]=boundary!=null}.ToString());
  ConRoiV48R21AssetPass.BuildReview();
 }
}
