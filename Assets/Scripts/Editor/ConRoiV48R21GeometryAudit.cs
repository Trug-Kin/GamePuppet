using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public static class ConRoiV48R21GeometryAudit
{
 public static void Audit()
 {
  EditorSceneManager.OpenScene("Assets/Scenes/JourneyRepair_v48_r21.unity");Physics.SyncTransforms();
  var terrain=GameObject.Find("R20_Whole_Source_Export").GetComponentsInChildren<MeshFilter>().Single(f=>f.name=="Surface_142");var collider=terrain.GetComponent<MeshCollider>();
  var source=JObject.Parse(File.ReadAllText("Docs/Reviews/journey_v48/r21/source-cross-section-geometry.json"));var rows=new JArray();
  foreach(var o in source["objects"])
  {
   string name=(string)o["name"];if(!name.StartsWith("OD35_Approach_")&&!name.StartsWith("OD35_Head_Landscape_Plinth")&&!name.StartsWith("OD35_Main_Route_Side_Link"))continue;
   var vertices=o["vertices"].Select(p=>new Vector3((float)p[0],(float)p[1],(float)p[2])).ToArray();var triangles=o["triangles"].SelectMany(t=>t.Values<int>()).ToArray();
   var samples=new JArray();float inset=name.Contains("Plinth")?.302f:.12f;
   for(int i=0;i<triangles.Length;i+=3)
   {
    var a=vertices[triangles[i]];var b=vertices[triangles[i+1]];var c=vertices[triangles[i+2]];
    if(Vector3.Cross(b-a,c-a).normalized.y<.3f)continue;
    int divisions=Mathf.Max(1,Mathf.CeilToInt(Mathf.Max(Vector3.Distance(a,b),Mathf.Max(Vector3.Distance(b,c),Vector3.Distance(c,a)))));
    for(int u=0;u<=divisions;u++)for(int v=0;v<=divisions-u;v++)
    {
     var p=a*(u/(float)divisions)+b*(v/(float)divisions)+c*((divisions-u-v)/(float)divisions);
     bool support=collider.Raycast(new Ray(new Vector3(p.x,40,p.z),Vector3.down),out var hit,100);
     samples.Add(new JObject{["x"]=p.x,["z"]=p.z,["source_top_y"]=p.y,["support"]=support,["terrain_y"]=support?new JValue(hit.point.y):JValue.CreateNull(),["underside_target_gap_m"]=support?new JValue(p.y-inset-hit.point.y):JValue.CreateNull()});
    }
   }
   var gaps=samples.Where(p=>(bool)p["support"]).Select(p=>(float)p["underside_target_gap_m"]).ToArray();
   rows.Add(new JObject{["name"]=name,["underside_inset_m"]=inset,["samples"]=samples,["missing_support_samples"]=samples.Count(p=>!(bool)p["support"]),["max_positive_gap_m"]=gaps.Length>0?gaps.Max():float.NaN,["most_negative_gap_m"]=gaps.Length>0?gaps.Min():float.NaN});
  }
  var shores=new JArray();var sections=JObject.Parse(File.ReadAllText("Docs/Reviews/journey_v48/r21/shore-water-ground-profiles.json"))["sections"];
  foreach(var edge in sections)
  {
   var a=new Vector3((float)edge["a"][0],0,(float)edge["a"][2]);var b=new Vector3((float)edge["b"][0],0,(float)edge["b"][2]);var n=new Vector3((b-a).z,0,-(b-a).x).normalized;if(n.x<0)n=-n;
   for(int i=0;i<=4;i++)foreach(float offset in new[]{0f,.25f,1f,3f,6f})
   {
    var p=Vector3.Lerp(a,b,i/4f)+n*offset;bool support=collider.Raycast(new Ray(p+Vector3.up*40,Vector3.down),out var hit,100);
    shores.Add(new JObject{["x"]=p.x,["z"]=p.z,["landward_offset_m"]=offset,["water_y"]=0,["ground_y"]=support?new JValue(hit.point.y):JValue.CreateNull()});
   }
  }
  File.WriteAllText("Docs/Reviews/journey_v48/r21/geometry-contact-audit.json",new JObject{["status"]="SUPPLEMENTAL_EDITOR_MEASUREMENTS_NOT_BUILD_VISUAL_ACCEPTANCE",["visible_mesh_matches_collider"]=terrain.sharedMesh==collider.sharedMesh,["mesh"]=AssetDatabase.GetAssetPath(terrain.sharedMesh),["approaches_and_plinths"]=rows,["shore_profiles"]=shores}.ToString());
 }
}
