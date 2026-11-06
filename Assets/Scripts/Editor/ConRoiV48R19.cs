using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.Build.Reporting;

public static class ConRoiV48R19
{
 const string Out="Docs/Reviews/journey_v48/r19",Art="Assets/Art/Environment/JourneyV48/ExportPolicyR19",Scene="Assets/Scenes/JourneyRepair_v48_r19.unity";
 static JArray V(Vector3 p)=>new JArray(p.x,p.y,p.z);
 static string P(Transform t)=>t.parent?P(t.parent)+"/"+t.name:t.name;
 public static void Assemble()
 {
  Directory.CreateDirectory(Out);Directory.CreateDirectory(Art);
  File.Copy("Docs/Reviews/journey_v48/r18/exporter-rebuild/RouteSurfaces.fbx",Art+"/RouteSurfaces.fbx",true);
  File.Copy("Docs/Reviews/journey_v48/r18/exporter-rebuild/surfaces.json",Art+"/surfaces.json",true);
  AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
  var importer=(ModelImporter)AssetImporter.GetAtPath(Art+"/RouteSurfaces.fbx");importer.isReadable=true;importer.addCollider=false;importer.materialImportMode=ModelImporterMaterialImportMode.None;importer.SaveAndReimport();
  EditorSceneManager.OpenScene("Assets/Scenes/JourneyRepair_v48_r17.unity");
  var all=UnityEngine.Object.FindObjectsByType<MeshFilter>(FindObjectsInactive.Include,FindObjectsSortMode.None);
  var materials=new Dictionary<string,Material>();var special=new Dictionary<string,MeshFilter>();
  var old=JObject.Parse(File.ReadAllText(ConRoiRouteV33.Folder+"/surfaces.json"));
  foreach(var row in old["meshes"])
  {
   string mesh=(string)row["mesh"],name=(string)row["material"];
   var f=all.SingleOrDefault(f=>f.name==mesh&&AssetDatabase.GetAssetPath(f.sharedMesh).StartsWith(ConRoiRouteV33.Folder+"/"));
   var mat=AssetDatabase.LoadAssetAtPath<Material>(ConRoiRouteV33.Folder+"/"+mesh+".mat");if(!mat)throw new Exception("Missing existing material "+mesh);materials[name]=mat;
   if(mesh=="Surface_041"||mesh=="Surface_061"||mesh=="Surface_063"){if(!f)throw new Exception("Missing special corrected batch "+mesh);special[name]=f;}
  }
  foreach(string folder in new[]{"MapRestoredR11","ModelsCompletedR15"})
  {
   var manifest=JObject.Parse(File.ReadAllText("Assets/Art/Environment/JourneyV48/"+folder+"/manifest.json"));
   foreach(var row in manifest["meshes"])
   {
    string name=(string)row["material"];if(materials.ContainsKey(name))continue;
    string mesh=(string)row["mesh"],reuse=(string)row["reuse_material"];
    var mat=AssetDatabase.LoadAssetAtPath<Material>(string.IsNullOrEmpty(reuse)?"Assets/Art/Environment/JourneyV48/"+folder+"/"+mesh+".mat":ConRoiRouteV33.Folder+"/"+reuse+".mat");
    if(!mat)throw new Exception("Missing restored material "+name);materials[name]=mat;
   }
  }
  var disabled=new JArray();
  foreach(var f in all)
  {
   string path=AssetDatabase.GetAssetPath(f.sharedMesh);
   if((path==ConRoiRouteV33.Folder+"/RouteSurfaces.fbx"||path.Contains("/MapRestoredR11/")||path.Contains("/ModelsCompletedR15/"))&&!special.Values.Contains(f))
   {var r=f.GetComponent<Renderer>();if(r&&r.enabled){r.enabled=false;disabled.Add(P(f.transform));}}
  }
  var root=(GameObject)PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>(Art+"/RouteSurfaces.fbx"));root.name="R19_Whole_Source_Export";
  var data=JObject.Parse(File.ReadAllText(Art+"/surfaces.json"));var mapping=new JArray();
  foreach(var f in root.GetComponentsInChildren<MeshFilter>())
  {
   var row=data["meshes"].Single(r=>(string)r["mesh"]==f.name);string name=(string)row["material"];
   if(!materials.TryGetValue(name,out var mat))throw new Exception("Unmapped material "+name);
   var r=f.GetComponent<Renderer>();r.sharedMaterial=mat;
   // Keep the previously verified gate UV and cavity/roof batches. These are
   // complete material batches, independent from the geographic clipping bug.
   if(special.ContainsKey(name))r.enabled=false;
   f.gameObject.layer=0;
   mapping.Add(new JObject{["mesh"]=f.name,["source_material"]=name,["material"]=AssetDatabase.GetAssetPath(mat),["sources"]=row["sources"].DeepClone(),["triangles"]=f.sharedMesh.triangles.Length/3,["rendered"]=r.enabled,["retained_special_batch"]=special.ContainsKey(name)?P(special[name].transform):"",["min"]=V(r.bounds.min),["max"]=V(r.bounds.max)});
  }
  var scenery=UnityEngine.Object.FindAnyObjectByType<CanhQuanChoV48>();scenery.scenery=scenery.scenery.Concat(new[]{root}).ToArray();
  RepairShoulder();
  EditorSceneManager.SaveScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene(),Scene);AssetDatabase.SaveAssets();
  File.WriteAllText(Out+"/exporter-runtime-integration.json",new JObject{["scene"]=Scene,["source_export_sha256"]=(string)JObject.Parse(File.ReadAllText("Docs/Reviews/journey_v48/r18/exporter-verification.json"))["fbx_sha256"],["disabled_superseded_renderers"]=disabled,["mapping"]=mapping,["original_materials_retained"]=true,["special_gate_roof_cavity_retained"]=true,["existing_colliders_retained"]=true,["local_supports_and_pine_normal_repairs_retained"]=true}.ToString());
  Audit();
 }
 static void RepairShoulder()
 {
  // V36 sampled the highest of several trimmed ground layers at every interior
  // point. Their discontinuous ownership made ridges inside this 3 m shoulder.
  // Preserve the entire footprint and four boundary curves; interpolate only
  // the interior of the existing two local patches, with physical grid weights.
  var f=GameObject.Find("SE_Graded_Shoulder_Visible_And_Collider").GetComponent<MeshFilter>();var old=f.sharedMesh;var verts=old.vertices;var before=(Vector3[])verts.Clone();
  const int columns=13;int rows=verts.Length/2/columns;if(rows!=25)throw new Exception("Unexpected shoulder topology");
  float maxChange=0;var changes=new JArray();
  for(int patch=0;patch<2;patch++)
  {
   int offset=patch*rows*columns;var y=new float[rows,columns];for(int r=0;r<rows;r++)for(int c=0;c<columns;c++)y[r,c]=verts[offset+r*columns+c].y;
   for(int it=0;it<3000;it++)for(int r=1;r<rows-1;r++)for(int c=1;c<columns-1;c++)
   {
    var p=verts[offset+r*columns+c];float dr=Vector2.Distance(new Vector2(p.x,p.z),new Vector2(verts[offset+(r+1)*columns+c].x,verts[offset+(r+1)*columns+c].z));float dc=Vector2.Distance(new Vector2(p.x,p.z),new Vector2(verts[offset+r*columns+c+1].x,verts[offset+r*columns+c+1].z));
    float wr=1/Mathf.Max(dr*dr,1e-6f),wc=1/Mathf.Max(dc*dc,1e-6f);y[r,c]=(wr*(y[r-1,c]+y[r+1,c])+wc*(y[r,c-1]+y[r,c+1]))/(2*(wr+wc));
   }
   for(int r=1;r<rows-1;r++)for(int c=1;c<columns-1;c++){int i=offset+r*columns+c;verts[i].y=y[r,c];maxChange=Mathf.Max(maxChange,Mathf.Abs(verts[i].y-before[i].y));}
  }
  string path="Assets/Art/Environment/JourneyV48/ExportPolicyR19/SE_Shoulder_Local.asset";var mesh=UnityEngine.Object.Instantiate(old);mesh.name="R19_SE_Shoulder_Continuous_Interior";mesh.vertices=verts;mesh.RecalculateNormals();mesh.RecalculateBounds();AssetDatabase.CreateAsset(mesh,path);f.sharedMesh=mesh;f.GetComponent<MeshCollider>().sharedMesh=mesh;
  for(int i=0;i<verts.Length;i++)if(Mathf.Abs(verts[i].y-before[i].y)>.00001f)changes.Add(new JObject{["index"]=i,["before"]=V(before[i]),["after"]=V(verts[i])});
  File.WriteAllText(Out+"/shoulder-local-repair.json",new JObject{["source"]=AssetDatabase.GetAssetPath(old),["derived"]=path,["footprint_unchanged"]=true,["all_boundary_vertices_unchanged"]=true,["bridge_road_terrain_elevations_unchanged"]=true,["maximum_interior_change_m"]=maxChange,["collider_matches_render"]=true,["changes"]=changes}.ToString());
 }
 public static void Audit()
 {
  var output=new JArray();
  foreach(int rev in new[]{10,17,19})
  {
   EditorSceneManager.OpenScene("Assets/Scenes/JourneyRepair_v48_r"+rev+".unity");Physics.SyncTransforms();
   var meshes=UnityEngine.Object.FindObjectsByType<MeshFilter>(FindObjectsInactive.Include,FindObjectsSortMode.None);
   var selected=meshes.Where(f=>P(f.transform).Contains("R11_Local_Bridge_Supports")||P(f.transform).Contains("R15_Local_Plinth_Foundations")).ToArray();
   var rows=new JArray(selected.Select(f=>new JObject{["path"]=P(f.transform),["mesh"]=AssetDatabase.GetAssetPath(f.sharedMesh),["position"]=V(f.transform.position),["scale"]=V(f.transform.lossyScale),["min"]=V(f.GetComponent<Renderer>().bounds.min),["max"]=V(f.GetComponent<Renderer>().bounds.max),["collider_matches"]=f.GetComponent<MeshCollider>()&&f.GetComponent<MeshCollider>().sharedMesh==f.sharedMesh,["enabled"]=f.GetComponent<Renderer>().enabled}));
   var samples=new JArray();
   // Foot-route centre, both approach edges, island platform and shoreline.
   var points=new List<(string,Vector3)>();
   var nw=new Vector3(685.9787f,1.14f,496.6172f);var se=new Vector3(642.2553f,1.14f,533.6046f);var axis=(se-nw).normalized;var side=Vector3.Cross(Vector3.up,axis);
   foreach(float along in new[]{-18f,-10f,-2f,2f,28f,55f,60f,68f,76f})foreach(float across in new[]{-6f,0f,6f})points.Add(("bridge-route",nw+axis*along+side*across));
   for(int i=0;i<16;i++){float a=i*Mathf.PI*2/16;points.Add(("island-base",new Vector3(714.94f+Mathf.Cos(a)*13.1f,1.2f,459.48f+Mathf.Sin(a)*13.1f)));}
   foreach(float z in new[]{435f,445f,455f,465f,475f,485f,495f})foreach(float x in new[]{675f,680f,685f,690f,695f})points.Add(("photo4-shore",new Vector3(x,0,z)));
   foreach(var p in points)
   {
    var hits=Physics.RaycastAll(new Vector3(p.Item2.x,40,p.Item2.z),Vector3.down,80,~0,QueryTriggerInteraction.Ignore).Where(h=>h.normal.y>.5f).OrderByDescending(h=>h.point.y);
    samples.Add(new JObject{["group"]=p.Item1,["point"]=V(p.Item2),["hits"]=new JArray(hits.Select(h=>new JObject{["path"]=P(h.collider.transform),["y"]=h.point.y,["normal"]=V(h.normal)}))});
   }
   output.Add(new JObject{["revision"]=rev,["structures"]=rows,["samples"]=samples,["scene_dirty"]=UnityEngine.SceneManagement.SceneManager.GetActiveScene().isDirty});
  }
  File.WriteAllText(Out+"/photo-geometry-scene-audit.json",output.ToString());
 }
 public static void Build()
 {
  EditorSceneManager.OpenScene(Scene);var version=PlayerSettings.bundleVersion;
  try{PlayerSettings.bundleVersion="0.48.19";var report=BuildPipeline.BuildPlayer(new[]{Scene},"Builds/Windows_v48_r19/ConRoi.exe",BuildTarget.StandaloneWindows64,BuildOptions.Development);
   File.WriteAllText(Out+"/build.json",new JObject{["result"]=report.summary.result.ToString(),["errors"]=report.summary.totalErrors,["warnings"]=report.summary.totalWarnings}.ToString());
   File.WriteAllText(Out+"/build-assets.json",new JObject{["assets"]=new JArray(report.packedAssets.SelectMany(p=>p.contents).Select(c=>c.sourceAssetPath).Distinct().OrderBy(p=>p))}.ToString());
   if(report.summary.result!=BuildResult.Succeeded)throw new Exception("r19 build failed");
  }finally{PlayerSettings.bundleVersion=version;AssetDatabase.SaveAssets();}
 }
 public static void RayAudit()
 {
  EditorSceneManager.OpenScene("Assets/Scenes/JourneyRepair_v48_r17.unity");
  var fs=UnityEngine.Object.FindObjectsByType<MeshFilter>(FindObjectsInactive.Include,FindObjectsSortMode.None).Where(f=>f.sharedMesh&&f.sharedMesh.isReadable&&f.GetComponent<Renderer>()&&f.GetComponent<Renderer>().enabled&&f.gameObject.activeInHierarchy).ToArray();
  var camera=new GameObject("Temporary_Ray_Audit").AddComponent<Camera>();camera.fieldOfView=60;camera.aspect=1600f/900;
  var rows=new JArray();
  foreach(var view in JArray.Parse(File.ReadAllText("Docs/Reviews/journey_v48/r17/matched-photos-r19/views.json")))
  {
   string name=(string)view["name"];if(!new[]{"photo2-bridge-approach","photo3-park-edge","photo4-shore","photo5-island-base","photo6-market-approach"}.Contains(name))continue;
   Vector3 Vec(JToken t)=>new Vector3((float)t[0],(float)t[1],(float)t[2]);
   camera.transform.SetPositionAndRotation(Vec(view["eye"]),Quaternion.LookRotation(Vec(view["lookAt"])-Vec(view["eye"])));
   foreach(float x in new[]{.15f,.25f,.35f,.45f,.55f,.65f,.75f,.85f})foreach(float y in new[]{.15f,.25f,.35f,.45f})
   {
    var ray=camera.ViewportPointToRay(new Vector3(x,y,0));float best=1000;string path="";Vector3 hit=Vector3.zero;
    foreach(var f in fs)
    {
     if(!f.GetComponent<Renderer>().bounds.IntersectRay(ray,out var distance)||distance>best)continue;
     var m=f.sharedMesh;var verts=m.vertices;var tris=m.triangles;var inv=f.transform.worldToLocalMatrix;var origin=inv.MultiplyPoint(ray.origin);var direction=inv.MultiplyVector(ray.direction);
     for(int i=0;i<tris.Length;i+=3)
     {
      var a=verts[tris[i]];var b=verts[tris[i+1]];var c=verts[tris[i+2]];var e1=b-a;var e2=c-a;var p=Vector3.Cross(direction,e2);float det=Vector3.Dot(e1,p);if(Mathf.Abs(det)<1e-10f)continue;
      var t=origin-a;float u=Vector3.Dot(t,p)/det;if(u<0||u>1)continue;var q=Vector3.Cross(t,e1);float v=Vector3.Dot(direction,q)/det;if(v<0||u+v>1)continue;float d=Vector3.Dot(e2,q)/det;
      if(d>0&&d<best){best=d;path=P(f.transform);hit=ray.GetPoint(d);}
     }
    }
    rows.Add(new JObject{["view"]=name,["viewport"]=new JArray(x,y),["path"]=path,["point"]=V(hit)});
   }
  }
  UnityEngine.Object.DestroyImmediate(camera.gameObject);File.WriteAllText(Out+"/visible-surface-rays-r17.json",rows.ToString());
 }
}
