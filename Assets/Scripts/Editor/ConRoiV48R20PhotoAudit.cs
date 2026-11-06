using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

// Read-only scene inspection: no transforms, meshes, colliders or scene saves.
public static class ConRoiV48R20PhotoAudit
{
 public static void RunAll(){Run();Profiles();NormalsAndDependencies();}
 public static void NormalsAndDependencies()
 {
  EditorSceneManager.OpenScene("Assets/Scenes/JourneyRepair_v48_r20.unity");
  var rows=new JArray();
  foreach(var f in UnityEngine.Object.FindObjectsByType<MeshFilter>(FindObjectsInactive.Include,FindObjectsSortMode.None).Where(f=>f.sharedMesh&&f.sharedMesh.isReadable&&f.GetComponent<Renderer>()&&f.GetComponent<Renderer>().enabled))
  {
   string asset=AssetDatabase.GetAssetPath(f.sharedMesh);if(!asset.Contains("FinitePineNormals")&&(!f.transform.root.name.Contains("Whole_Source")||!f.gameObject.activeInHierarchy))continue;
   var ns=f.sharedMesh.normals;int invalid=ns.Count(n=>float.IsNaN(n.x)||float.IsNaN(n.y)||float.IsNaN(n.z)||float.IsInfinity(n.x)||float.IsInfinity(n.y)||float.IsInfinity(n.z)||n.sqrMagnitude<1e-10f);
   rows.Add(new JObject{["path"]=Path(f.transform),["asset"]=asset,["vertices"]=f.sharedMesh.vertexCount,["triangles"]=f.sharedMesh.triangles.Length/3,["normal_count"]=ns.Length,["invalid_normals"]=invalid});
  }
  File.WriteAllText("Docs/Reviews/journey_v48/r20/final-mesh-dependencies.json",new JObject{["method"]="Read-only final scene dependencies and enabled corrected-export/pine mesh normals; incremental BuildReport contents were empty and are not used as individual packed-asset proof.",["scene_dependencies"]=new JArray(AssetDatabase.GetDependencies("Assets/Scenes/JourneyRepair_v48_r20.unity",true)),["mesh_normals"]=rows}.ToString());
 }
 public static void Run()
 {
  const string output="Docs/Reviews/journey_v48/r20";Directory.CreateDirectory(output);
  var source=JObject.Parse(File.ReadAllText("Docs/Reviews/journey_v48/r15/planter-local-repair.json"));
  var scenes=new JArray();
  foreach(int revision in new[]{14,20})
  {
   EditorSceneManager.OpenScene("Assets/Scenes/JourneyRepair_v48_r"+revision+".unity");Physics.SyncTransforms();
   var colliders=UnityEngine.Object.FindObjectsByType<Collider>(FindObjectsInactive.Include,FindObjectsSortMode.None);
   var bases=new JArray();
   foreach(var row in source["bases"])
   {
    var footprint=row["footprint_source"].Select(p=>new Vector3(-(float)p[0],(float)row["base_top"],-(float)p[1])).ToArray();
    var center=footprint.Aggregate(Vector3.zero,(sum,p)=>sum+p)/footprint.Length;
    var samples=new JArray();
    // Interior probes avoid edge ambiguity and measure the collider visible in
    // the inherited plinth's exact footprint. Source corner samples stay linked.
    foreach(var p in footprint.Select(p=>Vector3.Lerp(p,center,.1f)).Append(center))
    {
     var hits=Physics.RaycastAll(p+Vector3.up*.01f,Vector3.down,10,~0,QueryTriggerInteraction.Ignore).Where(h=>h.normal.y>.5f).OrderByDescending(h=>h.point.y).ToArray();
     samples.Add(new JObject{["point"]=V(p),["first_support_y"]=hits.Length>0?new JValue(hits[0].point.y):JValue.CreateNull(),["gap_from_original_plinth_bottom"]=hits.Length>0?new JValue(p.y+.002f-hits[0].point.y):JValue.CreateNull(),["support"]=hits.Length>0?Path(hits[0].collider.transform):"none",["all_hits"]=new JArray(hits.Select(h=>new JObject{["path"]=Path(h.collider.transform),["y"]=h.point.y,["normal"]=V(h.normal)}))});
    }
    string meshName=(string)row["mesh"];
    var added=colliders.OfType<MeshCollider>().Where(c=>c.name==meshName).ToArray();
    bases.Add(new JObject{["source_object"]=(string)row["source_planter"],["center"]=V(center),["footprint"]=new JArray(footprint.Select(V)),["source_max_unsupported_height"]=(float)row["max_height"],["samples"]=samples,["matching_foundation_colliders"]=new JArray(added.Select(c=>new JObject{["path"]=Path(c.transform),["enabled"]=c.enabled,["active"]=c.gameObject.activeInHierarchy,["same_render_mesh"]=c.GetComponent<MeshFilter>()&&c.sharedMesh==c.GetComponent<MeshFilter>().sharedMesh,["bounds_min"]=V(c.bounds.min),["bounds_max"]=V(c.bounds.max)}))});
   }
   var roots=UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
   var landmarks=new JArray(roots.SelectMany(r=>r.GetComponentsInChildren<Transform>(true)).Where(t=>t.name.Contains("Outside_Play")||t.name.Contains("Wake")||t.name.Contains("Bao")||t.name.Contains("Direction")||t.name.Contains("Sign")).Select(t=>new JObject{["path"]=Path(t),["position"]=V(t.position),["active"]=t.gameObject.activeInHierarchy}));
   var interactionObjects=roots.SelectMany(r=>r.GetComponentsInChildren<TuongTacHoCho>(true)).Where(i=>i.kind==TuongTacHoCho.Kind.Ha||i.kind==TuongTacHoCho.Kind.Wake).ToArray();
   var interactions=new JArray(interactionObjects.Select(i=>new JObject{["path"]=Path(i.transform),["kind"]=i.kind.ToString(),["position"]=V(i.transform.position),["colliders"]=new JArray(i.GetComponentsInChildren<Collider>(true).Select(c=>new JObject{["path"]=Path(c.transform),["min"]=V(c.bounds.min),["max"]=V(c.bounds.max),["enabled"]=c.enabled}))}));
   var ha=interactionObjects.FirstOrDefault(i=>i.kind==TuongTacHoCho.Kind.Ha);
   var nearHa=ha?new JArray(colliders.Where(c=>Vector3.Distance(c.transform.position,ha.transform.position)<10).Select(c=>new JObject{["path"]=Path(c.transform),["position"]=V(c.transform.position),["min"]=V(c.bounds.min),["max"]=V(c.bounds.max),["enabled"]=c.enabled})):new JArray();
   scenes.Add(new JObject{["revision"]=revision,["bases"]=bases,["landmarks"]=landmarks,["interactions"]=interactions,["near_ha_colliders"]=nearHa,["scene_dirty"]=UnityEngine.SceneManagement.SceneManager.GetActiveScene().isDirty});
  }
  File.WriteAllText(output+"/scene-contact-audit.json",new JObject{["method"]="Read-only imported scene collider raycasts, r14 before/r20 final; original photos now archived and attributed separately by landmarks. Build image evidence recorded separately.",["scenes"]=scenes}.ToString());
 }

 public static void Profiles()
 {
  EditorSceneManager.OpenScene("Assets/Scenes/JourneyRepair_v48_r20.unity");Physics.SyncTransforms();
  var mapping=JObject.Parse(File.ReadAllText("Docs/Reviews/journey_v48/r20/exporter-runtime-integration.json"));
  string terrainName=(string)mapping["mapping"].Single(x=>(string)x["source_material"]=="Terrain_Flat")["mesh"];
  var root=GameObject.Find("R20_Whole_Source_Export");var terrain=root.GetComponentsInChildren<MeshFilter>().Single(f=>f.name==terrainName);
  var collider=UnityEngine.Object.FindObjectsByType<MeshCollider>(FindObjectsSortMode.None).Single(c=>c.name=="Context_125");
  var mesh=terrain.sharedMesh;var points=mesh.vertices.Select(terrain.transform.TransformPoint).ToArray();var tris=mesh.triangles;
  var localPoints=points;var localTris=tris;
  var core=root.GetComponentsInChildren<MeshFilter>().Single(f=>f.name=="Surface_176");var corePoints=core.sharedMesh.vertices.Select(core.transform.TransformPoint).ToArray();var coreTris=core.sharedMesh.triangles;
  var coreCollider=UnityEngine.Object.FindObjectsByType<MeshCollider>(FindObjectsSortMode.None).Single(c=>c.name=="Context_124");
  float Height(float x,float z)
  {
   float best=float.NegativeInfinity;
   for(int i=0;i<tris.Length;i+=3){var a=points[tris[i]];var b=points[tris[i+1]];var c=points[tris[i+2]];
    float det=(b.z-c.z)*(a.x-c.x)+(c.x-b.x)*(a.z-c.z);if(Mathf.Abs(det)<1e-8f)continue;
    float u=((b.z-c.z)*(x-c.x)+(c.x-b.x)*(z-c.z))/det,v=((c.z-a.z)*(x-c.x)+(a.x-c.x)*(z-c.z))/det;
    if(u>=-1e-6f&&v>=-1e-6f&&u+v<=1.000001f)best=Mathf.Max(best,u*a.y+v*b.y+(1-u-v)*c.y);
   }return best;
  }
  var rows=new JArray();
  foreach(float z in new[]{435f,445f,455f,465f,475f,485f,495f})foreach(float x in new[]{675f,680f,685f,690f,695f})
  {
   float y=Height(x,z);string owner=terrainName;var groundCollider=collider;
   if(float.IsInfinity(y)){points=corePoints;tris=coreTris;y=Height(x,z);points=localPoints;tris=localTris;owner=core.name;groundCollider=coreCollider;}
   bool hit=groundCollider.Raycast(new Ray(new Vector3(x,30,z),Vector3.down),out var h,80);
   rows.Add(new JObject{["x"]=x,["z"]=z,["surface"]=owner,["collider"]=groundCollider.name,["visible_terrain_y"]=float.IsInfinity(y)?JValue.CreateNull():new JValue(y),["collider_y"]=hit?new JValue(h.point.y):JValue.CreateNull(),["visual_collider_delta_m"]=hit&&!float.IsInfinity(y)?new JValue(y-h.point.y):JValue.CreateNull()});
  }
  var shoulder=GameObject.Find("SE_Graded_Shoulder_Visible_And_Collider");var sm=shoulder.GetComponent<MeshFilter>().sharedMesh;var cm=shoulder.GetComponent<MeshCollider>();
  var support=new JArray(UnityEngine.Object.FindObjectsByType<MeshFilter>(FindObjectsInactive.Include,FindObjectsSortMode.None).Where(f=>f.name.Contains("Island")||f.name.Contains("Foundation")||f.name.Contains("Fascia")||f.name.Contains("Soffit")).Select(f=>new JObject{["path"]=Path(f.transform),["asset"]=AssetDatabase.GetAssetPath(f.sharedMesh),["visible"]=f.GetComponent<Renderer>()&&f.GetComponent<Renderer>().enabled,["same_mesh_collider"]=f.GetComponent<MeshCollider>()&&f.GetComponent<MeshCollider>().sharedMesh==f.sharedMesh}));
  File.WriteAllText("Docs/Reviews/journey_v48/r20/final-ground-profiles.json",new JObject{["method"]="Read-only final scene; transformed visible terrain triangles vs independent retained collider, 35 landmarks-grid points. Does not claim all unmeasured shoreline vertices or topology.",["terrain_mesh"]=terrainName,["terrain_matrix"]=terrain.transform.localToWorldMatrix.ToString("F6"),["terrain_collider_matrix"]=collider.transform.localToWorldMatrix.ToString("F6"),["photo4_samples"]=rows,["shoulder_render_collider_same_mesh"]=sm==cm.sharedMesh,["shoulder_asset"]=AssetDatabase.GetAssetPath(sm),["support_objects"]=support,["scene_dirty"]=UnityEngine.SceneManagement.SceneManager.GetActiveScene().isDirty}.ToString());
 }

 static JArray V(Vector3 p)=>new JArray(p.x,p.y,p.z);
 static string Path(Transform t)=>t.parent?Path(t.parent)+"/"+t.name:t.name;
}
