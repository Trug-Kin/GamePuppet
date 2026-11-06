using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public static class ConRoiV48R21Shore
{
 const string Out="Docs/Reviews/journey_v48/r21";
 const string Art="Assets/Art/Environment/JourneyV48/ShoreR21";
 static JArray V(Vector3 p)=>new JArray(p.x,p.y,p.z);
 static Vector2 XZ(JToken p)=>new Vector2((float)p[0],(float)p[2]);
 public static void RepairLocalBankDraft()
 {
  EditorSceneManager.OpenScene("Assets/Scenes/JourneyRepair_v48_r21.unity");
  Directory.CreateDirectory(Art);
  var sections=JObject.Parse(File.ReadAllText(Out+"/shore-water-ground-profiles.json"))["sections"];
  var edges=sections.Select(s=>(a:XZ(s["a"]),b:XZ(s["b"]))).ToArray();
  var root=GameObject.Find("R20_Whole_Source_Export");var rows=new JArray();
  foreach(var pair in new[]{("Surface_142","Context_125"),("Surface_176","Context_124")})
  {
   var f=root.GetComponentsInChildren<MeshFilter>().Single(t=>t.name==pair.Item1);
   var original=f.sharedMesh;var points=original.vertices;var changes=new JArray();
   for(int i=0;i<points.Length;i++)
   {
    var world=f.transform.TransformPoint(points[i]);if(world.z<420||world.z>550)continue;
    var p=new Vector2(world.x,world.z);float best=float.PositiveInfinity;Vector2 nearest=Vector2.zero,landward=Vector2.zero;
    foreach(var edge in edges)
    {
     var e=edge.b-edge.a;float t=Mathf.Clamp01(Vector2.Dot(p-edge.a,e)/e.sqrMagnitude);var q=edge.a+e*t;
     float distance=(p-q).sqrMagnitude;if(distance>=best)continue;
     var n=new Vector2(e.y,-e.x).normalized;if(n.x<0)n=-n;
     best=distance;nearest=q;landward=n;
    }
    float signed=Vector2.Dot(p-nearest,landward),distanceToEdge=Mathf.Sqrt(best);
    if(signed<-.025f||distanceToEdge>6)continue;
    // A narrow landscaped bank crest prevents the fixed y=0 lake polygon from
    // ending against dry-side terrain as low as -1 m. Blend into the measured
    // terrain within six metres; never alter water or the broad terrain grade.
    float spatial=Mathf.SmoothStep(0,1,Mathf.InverseLerp(420,430,world.z))*Mathf.SmoothStep(0,1,Mathf.InverseLerp(550,540,world.z));
    float weight=(1-Mathf.SmoothStep(0,1,Mathf.InverseLerp(.6f,6,distanceToEdge)))*spatial;
    float y=Mathf.Lerp(world.y,Mathf.Max(world.y,.26f),weight);
    if(y-world.y<.00001f)continue;
    var after=new Vector3(world.x,y,world.z);points[i]=f.transform.InverseTransformPoint(after);
    changes.Add(new JObject{["vertex"]=i,["before"]=V(world),["after"]=V(after),["distance_to_shore_m"]=distanceToEdge});
   }
   if(changes.Count==0)continue;
   var mesh=UnityEngine.Object.Instantiate(original);mesh.name="R21_LocalBank_"+pair.Item1;mesh.vertices=points;mesh.RecalculateNormals();mesh.RecalculateBounds();
   string path=Art+"/"+mesh.name+".asset";if(AssetDatabase.LoadAssetAtPath<Mesh>(path))throw new Exception("Preserve previous derivative; mesh already exists: "+path);
   AssetDatabase.CreateAsset(mesh,path);f.sharedMesh=mesh;
   var oldCollider=UnityEngine.Object.FindObjectsByType<MeshCollider>(FindObjectsInactive.Include).Single(c=>c.name==pair.Item2);oldCollider.enabled=false;
   f.gameObject.layer=3;f.gameObject.AddComponent<MeshCollider>().sharedMesh=mesh;
   rows.Add(new JObject{["renderer"]=f.name,["original_mesh"]=AssetDatabase.GetAssetPath(original),["derived_mesh"]=path,["superseded_collider"]=pair.Item2,["visible_and_collider_shared_mesh"]=true,["changes"]=changes});
  }
  var mist=UnityEngine.Object.FindAnyObjectByType<SuongDaLatV48>();mist.clearGeometryReview=true;
  EditorSceneManager.SaveScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene());AssetDatabase.SaveAssets();
  File.WriteAllText(Out+"/shore-bank-draft-repair.json",new JObject{["status"]="DRAFT_REQUIRES_VISUAL_AND_TRAVERSAL_VERIFICATION",["water_level_unchanged"]=0,["water_collider_added"]=false,["maximum_landward_width_m"]=6,["z_extent_m"]=new JArray(420,550),["crest_minimum_m"]=.26f,["fog_disabled_for_geometry_review"]=true,["meshes"]=rows}.ToString());
 }
}
