using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

// Construction faces for the retained promenade footprint, down to the
// adjoining earth. No foundation box, additional walking floor or terrain cap.
public static class ConRoiV48R21PromenadeEdges
{
 public static void AuthorAndBuildDraft()
 {
  Author(false);
 }
 public static void RefinePerimeterAndBuildDraft()
 {
  Author(true);
 }
 static void Author(bool replace)
 {
  EditorSceneManager.OpenScene("Assets/Scenes/JourneyRepair_v48_r21.unity");
  var old=UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects().SingleOrDefault(g=>g.name=="R21_SE_Promenade_Construction_Edges");
  if(old)
  {
   if(!replace)throw new Exception("Do not stack promenade edge geometry");
   var continuity=UnityEngine.Object.FindAnyObjectByType<CanhQuanChoV48>();continuity.scenery=continuity.scenery.Where(g=>g!=old).ToArray();UnityEngine.Object.DestroyImmediate(old);
  }
  var root=GameObject.Find("R20_Whole_Source_Export");var path=root.GetComponentsInChildren<MeshFilter>().Single(f=>f.name=="Surface_153");var ground=root.GetComponentsInChildren<MeshCollider>().Single(c=>c.name=="Surface_142");Physics.SyncTransforms();
  var probe=new GameObject("Temporary_Promenade_Perimeter_Probe");probe.transform.SetPositionAndRotation(path.transform.position,path.transform.rotation);probe.transform.localScale=path.transform.lossyScale;var pathCollider=probe.AddComponent<MeshCollider>();pathCollider.sharedMesh=path.sharedMesh;Physics.SyncTransforms();
  var vertices=path.sharedMesh.vertices.Select(path.transform.TransformPoint).ToArray();var triangles=path.sharedMesh.triangles;var counts=new Dictionary<(int,int),int>();
  for(int i=0;i<triangles.Length;i+=3)for(int j=0;j<3;j++){int a=triangles[i+j],b=triangles[i+(j+1)%3];var key=a<b?(a,b):(b,a);counts[key]=counts.TryGetValue(key,out int count)?count+1:1;}
  var points=new List<Vector3>();var faces=new List<int>();var exceptions=new JArray();float greatestDepth=0;int interiorEdges=0;
  foreach(var edge in counts.Where(p=>p.Value==1))
  {
   var a=vertices[edge.Key.Item1];var b=vertices[edge.Key.Item2];var centre=(a+b)*.5f;
   if(ConRoiV48R21JunctionGrade.Weight(centre)<=0||ConRoiV48R21JunctionGrade.InBridgeVoid(centre))continue;
   var tangent=b-a;tangent.y=0;if(tangent.sqrMagnitude<.000001f)continue;var normal=Vector3.Cross(tangent.normalized,Vector3.up)*.02f;
   bool left=pathCollider.Raycast(new Ray(new Vector3(centre.x+normal.x,30,centre.z+normal.z),Vector3.down),out _,70),right=pathCollider.Raycast(new Ray(new Vector3(centre.x-normal.x,30,centre.z-normal.z),Vector3.down),out _,70);
   // Terrain clipping can leave T junctions in the path triangulation. A
   // single triangle-index edge is not sufficient evidence of an outer edge.
   if(left==right){interiorEdges++;continue;}
   bool ha=ground.Raycast(new Ray(new Vector3(a.x,30,a.z),Vector3.down),out var ga,70),hb=ground.Raycast(new Ray(new Vector3(b.x,30,b.z),Vector3.down),out var gb,70);
   float da=ha?a.y-ga.point.y:float.PositiveInfinity,db=hb?b.y-gb.point.y:float.PositiveInfinity;
   if(da<=.001f&&db<=.001f)continue; // An overlapping higher platform already hides this edge.
   if(!ha||!hb||Mathf.Max(da,db)>.4f){exceptions.Add(new JObject{["x"]=centre.x,["z"]=centre.z,["depth_a"]=ha?new JValue(da):JValue.CreateNull(),["depth_b"]=hb?new JValue(db):JValue.CreateNull()});continue;}
   greatestDepth=Mathf.Max(greatestDepth,Mathf.Max(da,db));int n=points.Count;
   points.AddRange(new[]{a,b,new Vector3(a.x,Mathf.Min(a.y,ga.point.y)-.005f,a.z),new Vector3(b.x,Mathf.Min(b.y,gb.point.y)-.005f,b.z)});
   faces.AddRange(new[]{n,n+1,n+2,n+1,n+3,n+2,n+2,n+1,n,n+2,n+3,n+1});
  }
  UnityEngine.Object.DestroyImmediate(probe);
  var mesh=new Mesh{name="R21_SE_Promenade_Construction_Edges",indexFormat=UnityEngine.Rendering.IndexFormat.UInt32};mesh.SetVertices(points);mesh.SetTriangles(faces,0);mesh.RecalculateNormals();mesh.RecalculateBounds();AssetDatabase.CreateAsset(mesh,"Assets/Art/Environment/JourneyV48/LandscapeR21/"+mesh.name+".asset");
  var g=new GameObject(mesh.name);g.layer=3;g.AddComponent<MeshFilter>().sharedMesh=mesh;g.AddComponent<MeshRenderer>().sharedMaterial=path.GetComponent<Renderer>().sharedMaterial;g.AddComponent<MeshCollider>().sharedMesh=mesh;
  var scenery=UnityEngine.Object.FindAnyObjectByType<CanhQuanChoV48>();scenery.scenery=scenery.scenery.Concat(new[]{g}).ToArray();
  EditorSceneManager.SaveScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene());AssetDatabase.SaveAssets();
  File.WriteAllText("Docs/Reviews/journey_v48/r21/se-promenade-construction-edges.json",new JObject{["status"]="DRAFT_REQUIRES_BUILD_IMAGE_AND_NATIVE_WALK",["edge_segments"]=points.Count/4,["rejected_non_perimeter_triangle_edges"]=interiorEdges,["maximum_depth_m"]=greatestDepth,["collider_same_mesh"]=true,["unhandled_edges"]=exceptions,["unhandled_policy"]="Do not conceal missing support with a large wall; inspect these edges separately"}.ToString());
  ConRoiV48R21.BuildWorking();
 }
}
