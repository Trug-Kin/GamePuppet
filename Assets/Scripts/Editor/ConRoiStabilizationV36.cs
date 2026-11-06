using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
public static class ConRoiStabilizationV36
{
 public const string ScenePath="Assets/Scenes/LakeMarketIntegration_v36.unity";
 public static void InspectMarketWall()
 {
  EditorSceneManager.OpenScene(ScenePath);var output=new JArray();var point=new Vector3(785.1019f,6.6025f,273.9449f);
  foreach(var c in Object.FindObjectsByType<MeshCollider>(FindObjectsInactive.Include,FindObjectsSortMode.None).Where(c=>c.name=="Context_142"||c.name=="Context_124"))
  {
   var vertices=c.sharedMesh.vertices;var indices=c.sharedMesh.triangles;var faces=new JArray();
   for(int i=0;i<indices.Length;i+=3)
   {
    var a=c.transform.TransformPoint(vertices[indices[i]]);var b=c.transform.TransformPoint(vertices[indices[i+1]]);var d=c.transform.TransformPoint(vertices[indices[i+2]]);
    var bounds=new Bounds(a,Vector3.zero);bounds.Encapsulate(b);bounds.Encapsulate(d);bounds.Expand(2);
    if(!bounds.Contains(point))continue;
    JArray P(Vector3 p)=>new JArray(p.x,p.y,p.z);
    faces.Add(new JObject{["a"]=P(a),["b"]=P(b),["c"]=P(d),["normal"]=P(Vector3.Cross(b-a,d-a).normalized),["triangle"]=i/3});
   }
   output.Add(new JObject{["name"]=c.name,["vertices"]=vertices.Length,["scale"]=c.transform.lossyScale.ToString("F6"),["faces"]=faces});
  }
  File.WriteAllText("Docs/Reviews/lake_gate_market_current/v36/market-wall-geometry.json",output.ToString());
 }
 public static void RepairMarketWinding()
 {
  EditorSceneManager.OpenScene(ScenePath);
  var collider=Object.FindObjectsByType<MeshCollider>(FindObjectsInactive.Include,FindObjectsSortMode.None).First(c=>c.name=="Context_142");
  var original=collider.sharedMesh;var vertices=original.vertices;var triangles=original.triangles;
  var canonical=new System.Collections.Generic.Dictionary<Vector3,int>();var ids=new int[vertices.Length];
  for(int i=0;i<vertices.Length;i++){if(!canonical.TryGetValue(vertices[i],out int id)){id=canonical.Count;canonical.Add(vertices[i],id);}ids[i]=id;}
  int count=triangles.Length/3;var parent=Enumerable.Range(0,count).ToArray();var first=new System.Collections.Generic.Dictionary<int,int>();
  int Root(int i){while(parent[i]!=i){parent[i]=parent[parent[i]];i=parent[i];}return i;}
  for(int i=0;i<count;i++)for(int j=0;j<3;j++){int id=ids[triangles[i*3+j]];if(first.TryGetValue(id,out int prior))parent[Root(i)]=Root(prior);else first[id]=i;}
  var report=new JArray();int changed=0;
  foreach(var group in Enumerable.Range(0,count).GroupBy(Root))
  {
   var faces=group.ToArray();var points=faces.SelectMany(i=>new[]{vertices[triangles[i*3]],vertices[triangles[i*3+1]],vertices[triangles[i*3+2]]}).ToArray();
   var center=points.Aggregate(Vector3.zero,(a,b)=>a+b)/points.Length;double volume=0;
   var edges=new System.Collections.Generic.Dictionary<(int,int),int>();
   foreach(int i in faces)
   {
    var a=vertices[triangles[i*3]]-center;var b=vertices[triangles[i*3+1]]-center;var c=vertices[triangles[i*3+2]]-center;
    volume+=Vector3.Dot(a,Vector3.Cross(b,c))/6.0;
    for(int j=0;j<3;j++){int u=ids[triangles[i*3+j]],v=ids[triangles[i*3+(j+1)%3]];var edge=u<v?(u,v):(v,u);edges[edge]=edges.TryGetValue(edge,out int n)?n+1:1;}
   }
   bool closed=edges.Values.All(n=>n==2);bool reverse=closed&&volume<0;
   if(reverse)foreach(int i in faces){int temp=triangles[i*3+1];triangles[i*3+1]=triangles[i*3+2];triangles[i*3+2]=temp;changed++;}
   report.Add(new JObject{["faces"]=faces.Length,["closed"]=closed,["signed_local_volume_before"]=volume,["winding_reversed"]=reverse});
  }
  if(changed==0)throw new System.Exception("No closed inward component found; inspect before mutation");
  string path="Assets/Art/Environment/StabilizationV36/MarketPosts_Outward.asset";
  if(File.Exists(path))throw new System.Exception("Preserve existing repair asset");
  var mesh=Object.Instantiate(original);mesh.name="MarketPosts_Outward_SameVertices";mesh.triangles=triangles;mesh.RecalculateNormals();mesh.RecalculateBounds();AssetDatabase.CreateAsset(mesh,path);
  collider.sharedMesh=mesh;collider.GetComponent<MeshFilter>().sharedMesh=mesh;
  EditorSceneManager.SaveScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene(),ScenePath);AssetDatabase.SaveAssets();
  File.WriteAllText("Docs/Reviews/lake_gate_market_current/v36/market-winding-fix.json",new JObject{["collider"]="Context_142",["vertices_unchanged"]=vertices.SequenceEqual(mesh.vertices),["reversed_triangles"]=changed,["components"]=report,["asset"]=path}.ToString());
 }
 public static void BlendShoulders()
 {
  EditorSceneManager.OpenScene(ScenePath);Physics.SyncTransforms();
  var col=Object.FindObjectsByType<MeshCollider>(FindObjectsSortMode.None).First(c=>c.name=="OD35_COLLIDER_12");
  var points=col.sharedMesh.vertices.Select(v=>col.transform.TransformPoint(v)).Distinct().ToArray();
  var path=JObject.Parse(File.ReadAllText(ConRoiOngDaoV35.Folder+"/bridge_record.json"))["connections"][1];
  Vector3 P(JToken p)=>new Vector3((float)p[0],(float)p[1],(float)p[2]);
  var a=P(path.First);var axis=P(path.Last)-a;axis.y=0;axis.Normalize();var side=Vector3.Cross(Vector3.up,axis);
  float Ground(Vector3 p)
  {
   var hits=Physics.RaycastAll(p+Vector3.up*4,Vector3.down,10,8,QueryTriggerInteraction.Ignore).Where(h=>h.collider.name.StartsWith("Context_")&&h.normal.y>.5f).ToArray();
   if(hits.Length==0)throw new System.Exception("No original support for SE shoulder at "+p);return hits.Max(h=>h.point.y);
  }
  var vertices=new System.Collections.Generic.List<Vector3>();var triangles=new System.Collections.Generic.List<int>();
  foreach(int sign in new[]{-1,1})
  {
   var edge=points.Where(p=>Vector3.Dot(p-a,side)*sign>.8f).OrderBy(p=>Vector3.Dot(p-a,axis)).ToArray();int first=vertices.Count;
   for(int row=0;row<edge.Length;row++)for(int column=0;column<=12;column++)
   {
    float u=column/12f;var p=edge[row]+side*sign*(3*u);float baseY=Ground(p);float lift=Mathf.Max(0,edge[row].y-Ground(edge[row]));
    p.y=column==0?Mathf.Max(edge[row].y,baseY):baseY+lift*(1-u*u*(3-2*u));vertices.Add(p);
   }
   for(int row=0;row<edge.Length-1;row++)for(int column=0;column<12;column++)
   {
    int i=first+row*13+column;int[] face={i,i+13,i+1,i+1,i+13,i+14};
    if(Vector3.Cross(vertices[face[1]]-vertices[face[0]],vertices[face[2]]-vertices[face[0]]).y<0)for(int k=0;k<6;k+=3){int temp=face[k+1];face[k+1]=face[k+2];face[k+2]=temp;}
    triangles.AddRange(face);
   }
  }
  var mesh=new Mesh{name="SE_Visible_Graded_Shoulders"};mesh.SetVertices(vertices);mesh.SetTriangles(triangles,0);mesh.RecalculateNormals();mesh.RecalculateBounds();AssetDatabase.CreateAsset(mesh,"Assets/Art/Environment/StabilizationV36/SE_Shoulders.asset");
  var go=new GameObject("SE_Graded_Shoulder_Visible_And_Collider");go.layer=3;go.AddComponent<MeshFilter>().sharedMesh=mesh;go.AddComponent<MeshRenderer>().sharedMaterial=Object.FindObjectsByType<MeshRenderer>(FindObjectsSortMode.None).First(r=>r.name=="OD35_Surface_0").sharedMaterial;go.AddComponent<MeshCollider>().sharedMesh=mesh;
  EditorSceneManager.SaveScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene(),ScenePath);AssetDatabase.SaveAssets();
  File.WriteAllText("Docs/Reviews/lake_gate_market_current/v36/shoulders.json",new JObject{["width_each_side_m"]=3,["vertices"]=vertices.Count,["triangles"]=triangles.Count/3,["visual_mesh_is_collision_mesh"]=true,["original_143_meshes_unchanged"]=true}.ToString());
 }
 public static void BuildWindows()
 {
  Directory.CreateDirectory("Assets/Resources/VerificationV36");
  File.Copy(ConRoiRouteV32.Folder+"/regional_route.json","Assets/Resources/VerificationV36/route.json",true);
  File.Copy(ConRoiOngDaoV35.Folder+"/bridge_record.json","Assets/Resources/VerificationV36/bridge.json",true);AssetDatabase.Refresh();
  EditorSceneManager.OpenScene(ScenePath);
  var roots=UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
  int missing=roots.Sum(r=>r.GetComponentsInChildren<Transform>(true).Sum(t=>GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(t.gameObject)));
  if(missing>0)throw new System.Exception("Missing scripts: "+missing);
  var dependencies=AssetDatabase.GetDependencies(ScenePath,true);
  File.WriteAllText("Docs/Reviews/lake_gate_market_current/v36/dependencies.json",new JObject{["missing_scripts"]=missing,["assets"]=new JArray(dependencies),["all_project_relative"]=dependencies.All(p=>p.StartsWith("Assets/")||p.StartsWith("Packages/"))}.ToString());
  EditorBuildSettings.scenes=new[]{new EditorBuildSettingsScene(ScenePath,true)};
  PlayerSettings.productName="Con Roi - Ho Cho";PlayerSettings.bundleVersion="0.36.0";
  var report=BuildPipeline.BuildPlayer(new[]{ScenePath},"Builds/Windows_v36/ConRoi-HoCho.exe",BuildTarget.StandaloneWindows64,BuildOptions.None);
  File.WriteAllText("Docs/Reviews/lake_gate_market_current/v36/build.json",new JObject{["result"]=report.summary.result.ToString(),["errors"]=report.summary.totalErrors,["warnings"]=report.summary.totalWarnings,["bytes"]=report.summary.totalSize,["path"]=report.summary.outputPath,["scene"]=ScenePath}.ToString());
 }
 public static void Assemble()
 {
  EditorSceneManager.OpenScene(ConRoiOngDaoV35.ScenePath);
  string folder="Assets/Art/Environment/StabilizationV36";Directory.CreateDirectory(folder);
  var col=Object.FindObjectsByType<MeshCollider>(FindObjectsSortMode.None).First(c=>c.name=="OD35_COLLIDER_12");
  var original=col.sharedMesh;var old=original.vertices;var changed=(Vector3[])old.Clone();
  var record=JObject.Parse(File.ReadAllText(ConRoiOngDaoV35.Folder+"/bridge_record.json"));var path=record["connections"][1];
  Vector3 Point(JToken p)=>new Vector3((float)p[0],(float)p[1],(float)p[2]);
  var start=Point(path.First);var end=Point(path.Last);var axis=end-start;axis.y=0;float length=axis.magnitude;axis.Normalize();var side=Vector3.Cross(Vector3.up,axis);
  // Preserve the authored endpoints and width. Replace per-vertex clamp at the
  // footprint boundary with one continuous cubic grade on each edge.
  var ends=old.Select(v=>col.transform.TransformPoint(v)).OrderBy(v=>Vector3.Dot(v-start,axis)).Take(2).OrderBy(v=>Vector3.Dot(v-start,side)).ToArray();
  for(int i=0;i<old.Length;i++)
  {
   var w=col.transform.TransformPoint(old[i]);float t=Mathf.Clamp01(Vector3.Dot(w-start,axis)/length);float lateral=Mathf.Clamp01((Vector3.Dot(w-start,side)+1.2f)/2.4f);float initial=Mathf.Lerp(ends[0].y,ends[1].y,lateral);
   w.y=Mathf.Lerp(initial,end.y,t*t*(3-2*t));changed[i]=col.transform.InverseTransformPoint(w);
  }
  var mesh=Object.Instantiate(original);mesh.name="SE_Link_Continuous_Grade_v36";mesh.vertices=changed;mesh.RecalculateNormals();mesh.RecalculateBounds();AssetDatabase.CreateAsset(mesh,folder+"/SE_Link.asset");col.sharedMesh=mesh;
  int matched=0;
  foreach(var mf in Object.FindObjectsByType<MeshFilter>(FindObjectsSortMode.None).Where(m=>m.name=="OD35_Surface_0"))
  {
   var visual=Object.Instantiate(mf.sharedMesh);var vertices=visual.vertices;
   for(int i=0;i<vertices.Length;i++){var w=mf.transform.TransformPoint(vertices[i]);for(int j=0;j<old.Length;j++)if((w-col.transform.TransformPoint(old[j])).sqrMagnitude<.0000001f){vertices[i]=mf.transform.InverseTransformPoint(col.transform.TransformPoint(changed[j]));matched++;break;}}
   visual.vertices=vertices;visual.RecalculateNormals();visual.RecalculateBounds();AssetDatabase.CreateAsset(visual,folder+"/Paving.asset");mf.sharedMesh=visual;
  }
  if(matched<old.Length)throw new System.Exception("Visual/collision vertex mapping incomplete: "+matched);
  EditorSceneManager.SaveScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene(),ScenePath);AssetDatabase.SaveAssets();
  File.WriteAllText("Docs/Reviews/lake_gate_market_current/v36/geometry_fix.json",new JObject{["source"]=ConRoiOngDaoV35.ScenePath,["scene"]=ScenePath,["collider_vertices"]=old.Length,["matched_visual_vertices"]=matched,["old_vertices"]=new JArray(old.Select(v=>new JArray(v.x,v.y,v.z))),["new_vertices"]=new JArray(changed.Select(v=>new JArray(v.x,v.y,v.z))),["controller_parameters_changed"]=false}.ToString());
 }
 public static void InspectGeometry()
 {
  var col=Object.FindObjectsByType<MeshCollider>(FindObjectsSortMode.None).First(c=>c.name=="OD35_COLLIDER_12");
  Directory.CreateDirectory("Docs/Reviews/lake_gate_market_current/v36");
  File.WriteAllText("Docs/Reviews/lake_gate_market_current/v36/link_mesh.json",new JObject{["name"]=col.name,["vertices"]=new JArray(col.sharedMesh.vertices.Select(v=>{var p=col.transform.TransformPoint(v);return new JArray(p.x,p.y,p.z);})),["triangles"]=new JArray(col.sharedMesh.triangles)}.ToString());
 }
}
