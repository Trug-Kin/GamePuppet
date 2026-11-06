using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public static class ConRoiV48R21Footpaths
{
 public static void ConformAndBuildDraft()
 {
  MatchChangedLocalGradeDraft();ConRoiV48R21BoundaryAudit.Audit();ConRoiV48R21GeometryAudit.Audit();ConRoiV48R21.EnableAtmosphereDraftAndBuild();
 }
 public static Mesh DensifyLocal(Mesh source,Transform owner)
 {
  var original=source.vertices;var originalUV=source.uv;var vertices=new List<Vector3>();var uvs=new List<Vector2>();var triangles=new List<int>();var index=new Dictionary<(int,int,int),int>();
  int Vertex(Vector3 world,Vector2 uv)
  {
   var key=(Mathf.RoundToInt(world.x*10000),Mathf.RoundToInt(world.y*10000),Mathf.RoundToInt(world.z*10000));
   if(index.TryGetValue(key,out var existing))return existing;
   int n=vertices.Count;index.Add(key,n);vertices.Add(owner.InverseTransformPoint(world));uvs.Add(uv);return n;
  }
  void Triangle(Vector3 a,Vector3 b,Vector3 c,Vector2 ua,Vector2 ub,Vector2 uc,int depth)
  {
   float minX=Mathf.Min(a.x,Mathf.Min(b.x,c.x)),maxX=Mathf.Max(a.x,Mathf.Max(b.x,c.x)),minZ=Mathf.Min(a.z,Mathf.Min(b.z,c.z)),maxZ=Mathf.Max(a.z,Mathf.Max(b.z,c.z));
   bool local=!(maxX<610||minX>725||maxZ<470||minZ>565);
   float Dist(Vector3 x,Vector3 y)=>new Vector2(x.x-y.x,x.z-y.z).sqrMagnitude;
   float ab=Dist(a,b),bc=Dist(b,c),ca=Dist(c,a);
   if(local&&depth<20&&Mathf.Max(ab,Mathf.Max(bc,ca))>.25f)
   {
    if(ab>=bc&&ab>=ca){var m=(a+b)*.5f;var uv=(ua+ub)*.5f;Triangle(a,m,c,ua,uv,uc,depth+1);Triangle(m,b,c,uv,ub,uc,depth+1);}
    else if(bc>=ca){var m=(b+c)*.5f;var uv=(ub+uc)*.5f;Triangle(a,b,m,ua,ub,uv,depth+1);Triangle(a,m,c,ua,uv,uc,depth+1);}
    else{var m=(c+a)*.5f;var uv=(uc+ua)*.5f;Triangle(a,b,m,ua,ub,uv,depth+1);Triangle(m,b,c,uv,ub,uc,depth+1);}
    return;
   }
   triangles.Add(Vertex(a,ua));triangles.Add(Vertex(b,ub));triangles.Add(Vertex(c,uc));
  }
  var sourceTriangles=source.triangles;
  for(int i=0;i<sourceTriangles.Length;i+=3)
  {
   int a=sourceTriangles[i],b=sourceTriangles[i+1],c=sourceTriangles[i+2];
   Triangle(owner.TransformPoint(original[a]),owner.TransformPoint(original[b]),owner.TransformPoint(original[c]),originalUV.Length==original.Length?originalUV[a]:Vector2.zero,originalUV.Length==original.Length?originalUV[b]:Vector2.zero,originalUV.Length==original.Length?originalUV[c]:Vector2.zero,0);
  }
  var result=new Mesh{name="Dense_Local_Footway",indexFormat=UnityEngine.Rendering.IndexFormat.UInt32};result.SetVertices(vertices);result.SetUVs(0,uvs);result.SetTriangles(triangles,0);result.RecalculateNormals();result.RecalculateBounds();return result;
 }
 public static void MatchChangedLocalGradeDraft()
 {
  const string art="Assets/Art/Environment/JourneyV48/LandscapeR21";
  EditorSceneManager.OpenScene("Assets/Scenes/JourneyRepair_v48_r21.unity");
  var root=GameObject.Find("R20_Whole_Source_Export");var terrain=root.GetComponentsInChildren<MeshFilter>().Single(f=>f.name=="Surface_142");
  var rawMesh=AssetDatabase.LoadAllAssetsAtPath("Assets/Art/Environment/JourneyV48/ExportPolicyR20/RouteSurfaces.fbx").OfType<Mesh>().Single(m=>m.name=="Surface_142");
  var temporary=new GameObject("Temporary_Original_Terrain_For_Projection");temporary.transform.SetParent(terrain.transform,false);var originalCollider=temporary.AddComponent<MeshCollider>();originalCollider.sharedMesh=rawMesh;
  var current=terrain.GetComponent<MeshCollider>();Physics.SyncTransforms();
  var rows=new JArray();
  Mesh Project(Mesh before,Transform owner,string name)
  {
   string beforePath=AssetDatabase.GetAssetPath(before);int sourceTriangles=before.triangles.Length/3;
   before=DensifyLocal(before,owner);
   var split=ConRoiV48R21PathConform.Split(before,owner,terrain);UnityEngine.Object.DestroyImmediate(before);before=split;
   var points=before.vertices;var changes=new JArray();var projectedNormals=new Dictionary<int,Vector3>();
   var groundTriangles=terrain.sharedMesh.triangles;var groundNormals=terrain.sharedMesh.normals;
   for(int i=0;i<points.Length;i++)
   {
    var p=owner.TransformPoint(points[i]);if(p.x<610||p.x>725||p.z<470||p.z>565)continue;
    if(name=="Surface_153"||name=="Context_136")
    {
     var graded=ConRoiV48R21JunctionGrade.Paving(p);
     if(Mathf.Abs(graded.y-p.y)>.0001f)
     {
      changes.Add(new JObject{["vertex"]=i,["kind"]="SE junction grade matching authored approach road",["before_y"]=p.y,["after_y"]=graded.y});points[i]=owner.InverseTransformPoint(graded);p=graded;
     }
    }
    var ray=new Ray(new Vector3(p.x,40,p.z),Vector3.down);
    bool hadGround=originalCollider.Raycast(ray,out var oldHit,100);if(!current.Raycast(ray,out var newHit,100))continue;
    if((name=="Surface_153"||name=="Context_136")&&newHit.point.y<p.y-.02f)continue;
    float delta=newHit.point.y-(hadGround?oldHit.point.y:p.y);if(delta<=.03f&&newHit.point.y<p.y-.02f)continue;
    float weight=newHit.point.y>=p.y-.02f?1:Mathf.SmoothStep(0,1,Mathf.InverseLerp(.03f,.2f,delta));
    var after=new Vector3(p.x,Mathf.Lerp(p.y,newHit.point.y+.025f,weight),p.z);if(Mathf.Abs(after.y-p.y)<.0001f)continue;
    points[i]=owner.InverseTransformPoint(after);
    int triangle=newHit.triangleIndex*3;var bary=newHit.barycentricCoordinate;
    var normal=groundNormals[groundTriangles[triangle]]*bary.x+groundNormals[groundTriangles[triangle+1]]*bary.y+groundNormals[groundTriangles[triangle+2]]*bary.z;
    projectedNormals[i]=owner.InverseTransformDirection(terrain.transform.TransformDirection(normal)).normalized;
    changes.Add(new JObject{["vertex"]=i,["before_y"]=p.y,["after_y"]=after.y,["grade_change_m"]=delta});
   }
   if(changes.Count==0){UnityEngine.Object.DestroyImmediate(before);return AssetDatabase.LoadAssetAtPath<Mesh>(beforePath);}
   var mesh=before;mesh.name="R21_ProjectedDense_"+name;mesh.vertices=points;ConRoiV48R21PathConform.RemoveInvalidLocalFaces(mesh,owner);mesh.RecalculateNormals();var normals=mesh.normals;foreach(var pair in projectedNormals)normals[pair.Key]=pair.Value;mesh.normals=normals;mesh.RecalculateBounds();string path=art+"/"+mesh.name+".asset";AssetDatabase.CreateAsset(mesh,path);
   rows.Add(new JObject{["name"]=name,["original"]=beforePath,["derived"]=path,["source_triangles"]=sourceTriangles,["triangles"]=mesh.triangles.Length/3,["local_max_edge_m"]=.5f,["split_at_terrain_triangle_boundaries"]=true,["changes"]=changes});return mesh;
  }
  var sourceAssets=AssetDatabase.LoadAllAssetsAtPath("Assets/Art/Environment/JourneyV48/ExportPolicyR20/RouteSurfaces.fbx").OfType<Mesh>().ToArray();
  foreach(var f in root.GetComponentsInChildren<MeshFilter>().Where(f=>f.name=="Surface_125"||f.name=="Surface_126"||f.name=="Surface_153"))f.sharedMesh=Project(sourceAssets.Single(m=>m.name==f.name),f.transform,f.name);
  foreach(var c in UnityEngine.Object.FindObjectsByType<MeshCollider>(FindObjectsInactive.Include).Where(c=>c.name=="Context_106"||c.name=="Context_107"||c.name=="Context_136"))
  {
   var path=AssetDatabase.GetAssetPath(c.sharedMesh);
   // Collision exports contain extra road geometry; start from their own
   // original meshes rather than replacing them with the render footway.
   var first=JObject.Parse(File.ReadAllText("Docs/Reviews/journey_v48/r21/local-path-grade-draft.json"))["meshes"].FirstOrDefault(r=>(string)r["name"]==c.name);
   string originalPath=first==null?path:(string)first["original"];
   var original=AssetDatabase.LoadAllAssetsAtPath(originalPath).OfType<Mesh>().SingleOrDefault(m=>m.name==c.name);
   if(!original)throw new Exception("Original footway collider missing: "+originalPath);
   c.sharedMesh=Project(original,c.transform,c.name);
  }
  var shoulder=UnityEngine.Object.FindObjectsByType<MeshFilter>(FindObjectsInactive.Include).Single(f=>f.name=="SE_Graded_Shoulder_Visible_And_Collider");
  // The old V36 shoulder can be retired after the terrain patch closes its
  // underlying support. Do not recreate or reactivate that obsolete overlay.
  if(shoulder.gameObject.activeSelf)
  {
   shoulder.sharedMesh=Project(AssetDatabase.LoadAssetAtPath<Mesh>("Assets/Art/Environment/JourneyV48/ExportPolicyR20/SE_Shoulder_Local.asset"),shoulder.transform,"SE_Shoulder_Local");
   shoulder.GetComponent<MeshCollider>().sharedMesh=shoulder.sharedMesh;
  }
  UnityEngine.Object.DestroyImmediate(temporary);
  EditorSceneManager.SaveScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene());AssetDatabase.SaveAssets();
  File.WriteAllText("Docs/Reviews/journey_v48/r21/local-path-grade-draft.json",new JObject{["status"]="DRAFT_REQUIRES_VISUAL_AND_COLLIDER_PROFILE_CHECK",["scope"]="Footways follow changed local earthworks. Main promenade matches the authored SE approach grade locally, retaining its footprint and blending back to the original grade.",["junction_centre_xz"]=new JArray(635,548),["junction_paving_y"]=1.105f,["junction_inner_outer_radius_m"]=new JArray(14,19),["visible_and_collision_same_projection"]=true,["water_modified"]=false,["meshes"]=rows}.ToString());
 }
}
