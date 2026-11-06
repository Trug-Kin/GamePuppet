using System.IO;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

// The inherited approach paving is a single sheet, not a solid slab. Give its
// measured perimeter a 12 cm construction edge, down to the locally graded
// earth. These narrow vertical faces cannot replace missing ground support.
public static class ConRoiV48R21PavingEdges
{
 public static void AuthorDraft()
 {
  EditorSceneManager.OpenScene("Assets/Scenes/JourneyRepair_v48_r21.unity");
  const string folder="Assets/Art/Environment/JourneyV48/LandscapeR21";
  var old=GameObject.Find("R21_Paving_Construction_Edges");if(old)throw new System.Exception("Preserve existing paving edge derivative");
  var root=new GameObject("R21_Paving_Construction_Edges");var rows=new JArray();
  void Add(string name,Vector3[] vertices,int[] triangles)
  {
   var counts=new Dictionary<(int,int),int>();
   for(int i=0;i<triangles.Length;i+=3)for(int j=0;j<3;j++)
   {
    int a=triangles[i+j],b=triangles[i+(j+1)%3];var key=a<b?(a,b):(b,a);counts[key]=counts.TryGetValue(key,out int count)?count+1:1;
   }
   var points=new List<Vector3>();var faces=new List<int>();
   foreach(var edge in counts.Where(e=>e.Value==1))
   {
    var a=vertices[edge.Key.Item1];var b=vertices[edge.Key.Item2];if(Vector3.Distance(a,b)<.001f)continue;
    int n=points.Count;points.AddRange(new[]{a,b,a-Vector3.up*.12f,b-Vector3.up*.12f});
    // Both faces are visible from the accessible shoulder, irrespective of
    // source boundary winding. The collider uses exactly these same faces.
    faces.AddRange(new[]{n,n+1,n+2,n+1,n+3,n+2,n+2,n+1,n,n+2,n+3,n+1});
   }
   if(faces.Count==0)return;
   var mesh=new Mesh{name="R21_PavingEdge_"+name};mesh.SetVertices(points);mesh.SetTriangles(faces,0);mesh.RecalculateNormals();mesh.RecalculateBounds();AssetDatabase.CreateAsset(mesh,folder+"/"+mesh.name+".asset");
   var g=new GameObject(mesh.name);g.layer=3;g.transform.SetParent(root.transform);g.AddComponent<MeshFilter>().sharedMesh=mesh;g.AddComponent<MeshRenderer>().sharedMaterial=AssetDatabase.LoadAssetAtPath<Material>("Assets/Art/Environment/OngDaoV35/OD35_Paving.mat");g.AddComponent<MeshCollider>().sharedMesh=mesh;
   rows.Add(new JObject{["source"]=name,["edge_count"]=points.Count/4,["depth_m"]=.12f,["collider_shared_mesh"]=true});
  }
  var source=JObject.Parse(File.ReadAllText("Docs/Reviews/journey_v48/r21/source-cross-section-geometry.json"));
  foreach(var o in source["objects"])
  {
   string name=(string)o["name"];if(!name.StartsWith("OD35_Approach_")&&!name.StartsWith("OD35_Main_Route_Side_Link"))continue;
   Add(name,o["vertices"].Select(p=>new Vector3((float)p[0],(float)p[1],(float)p[2])).ToArray(),o["triangles"].SelectMany(t=>t.Values<int>()).ToArray());
  }
  foreach(var f in GameObject.Find("R21_Planter_Sidewalk_Connections").GetComponentsInChildren<MeshFilter>())Add(f.name,f.sharedMesh.vertices.Select(f.transform.TransformPoint).ToArray(),f.sharedMesh.triangles);
  var scenery=Object.FindAnyObjectByType<CanhQuanChoV48>();scenery.scenery=scenery.scenery.Concat(new[]{root}).ToArray();
  EditorSceneManager.SaveScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene());AssetDatabase.SaveAssets();
  File.WriteAllText("Docs/Reviews/journey_v48/r21/paving-construction-edges-draft.json",new JObject{["status"]="DRAFT_REQUIRES_BUILD_VISUAL_CHECK",["purpose"]="Construction thickness for source single-sheet paving; not a large foundation patch",["edges"]=rows}.ToString());
 }
 public static void FinishIndependentDraft()
 {
  AuthorDraft();ConRoiV48R21Landscape.RepairAndMeasureDraft();ConRoiV48R21BoundaryAudit.Audit();
 }
}
