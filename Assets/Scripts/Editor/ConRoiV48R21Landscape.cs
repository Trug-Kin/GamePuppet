using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

// Local earthworks beneath the existing approach and landscape composition.
// This replaces tall rectangular support patches, not the bridge span.
public static class ConRoiV48R21Landscape
{
 const string Out="Docs/Reviews/journey_v48/r21",Art="Assets/Art/Environment/JourneyV48/LandscapeR21";
 static JArray V(Vector3 p)=>new JArray(p.x,p.y,p.z);
 sealed class Shape
 {
  public string name;public Vector3[] vertices;public int[] triangles;public float inset;
  public Bounds bounds;
 }
 static float Target(Shape s,Vector3 p)
 {
  float best=float.NegativeInfinity;var b=s.bounds;b.Expand(new Vector3(10,100,10));if(!b.Contains(p))return best;
  for(int i=0;i<s.triangles.Length;i+=3)
  {
   var a=s.vertices[s.triangles[i]];var c=s.vertices[s.triangles[i+1]];var d=s.vertices[s.triangles[i+2]];
   float det=(c.z-d.z)*(a.x-d.x)+(d.x-c.x)*(a.z-d.z);if(Mathf.Abs(det)<1e-7f)continue;
   float u=((c.z-d.z)*(p.x-d.x)+(d.x-c.x)*(p.z-d.z))/det;
   float v=((d.z-a.z)*(p.x-d.x)+(a.x-d.x)*(p.z-d.z))/det;
   Vector3 q;
   if(u>=0&&v>=0&&u+v<=1)q=a*u+c*v+d*(1-u-v);
   else
   {
    Vector3 Edge(Vector3 x,Vector3 y){var e=y-x;e.y=0;var delta=p-x;delta.y=0;return Vector3.Lerp(x,y,Mathf.Clamp01(Vector3.Dot(delta,e)/Mathf.Max(e.sqrMagnitude,1e-8f)));}
    var choices=new[]{Edge(a,c),Edge(c,d),Edge(d,a)};
    q=choices.OrderBy(x=>new Vector2(x.x-p.x,x.z-p.z).sqrMagnitude).First();
   }
   float distance=Vector2.Distance(new Vector2(p.x,p.z),new Vector2(q.x,q.z));if(distance>=5)continue;
   float weight=1-Mathf.SmoothStep(0,1,Mathf.InverseLerp(.7f,5,distance));
   best=Mathf.Max(best,Mathf.Lerp(p.y,Mathf.Max(p.y,q.y-s.inset),weight));
  }
  return best;
 }
 static float FootprintCeiling(Shape s,Vector3 p)
 {
  if(p.x<s.bounds.min.x||p.x>s.bounds.max.x||p.z<s.bounds.min.z||p.z>s.bounds.max.z)return float.PositiveInfinity;
  float top=float.NegativeInfinity;
  for(int i=0;i<s.triangles.Length;i+=3)
  {
   var a=s.vertices[s.triangles[i]];var b=s.vertices[s.triangles[i+1]];var c=s.vertices[s.triangles[i+2]];
   if(Vector3.Cross(b-a,c-a).normalized.y<.3f)continue;
   float det=(b.z-c.z)*(a.x-c.x)+(c.x-b.x)*(a.z-c.z);if(Mathf.Abs(det)<1e-7f)continue;
   float u=((b.z-c.z)*(p.x-c.x)+(c.x-b.x)*(p.z-c.z))/det;
   float v=((c.z-a.z)*(p.x-c.x)+(a.x-c.x)*(p.z-c.z))/det;
   if(u>=-.00001f&&v>=-.00001f&&u+v<=1.00001f)top=Mathf.Max(top,a.y*u+b.y*v+c.y*(1-u-v));
  }
  return float.IsNegativeInfinity(top)?float.PositiveInfinity:top-s.inset;
 }
 public static void RegradeDenseDraft()
 {
  EditorSceneManager.OpenScene("Assets/Scenes/JourneyRepair_v48_r21.unity");
  var source=JObject.Parse(File.ReadAllText(Out+"/source-cross-section-geometry.json"));var shapes=new List<Shape>();
  foreach(var o in source["objects"])
  {
   string name=(string)o["name"];
   if(!name.StartsWith("OD35_Approach_")&&!name.StartsWith("OD35_Head_Landscape_Plinth")&&!name.StartsWith("OD35_Main_Route_Side_Link"))continue;
   var s=new Shape{name=name,vertices=o["vertices"].Select(p=>new Vector3((float)p[0],(float)p[1],(float)p[2])).ToArray(),triangles=o["triangles"].SelectMany(t=>t.Values<int>()).ToArray(),inset=name.Contains("Plinth")?.302f:.12f};
   s.bounds=new Bounds(s.vertices[0],Vector3.zero);foreach(var p in s.vertices)s.bounds.Encapsulate(p);shapes.Add(s);
  }
  foreach(var f in GameObject.Find("R21_Planter_Sidewalk_Connections").GetComponentsInChildren<MeshFilter>())
  {
   var vertices=f.sharedMesh.vertices.Select(f.transform.TransformPoint).ToArray();var b=new Bounds(vertices[0],Vector3.zero);foreach(var p in vertices)b.Encapsulate(p);
   shapes.Add(new Shape{name=f.name,vertices=vertices,triangles=f.sharedMesh.triangles,inset=.12f,bounds=b});
  }
  var terrain=GameObject.Find("R20_Whole_Source_Export").GetComponentsInChildren<MeshFilter>().Single(f=>f.name=="Surface_142");
  // Restart from the shore-only derivative, so grading is applied once and
  // source terrain outside the local earthworks retains its original height.
  var original=AssetDatabase.LoadAssetAtPath<Mesh>("Assets/Art/Environment/JourneyV48/ShoreR21/R21_LocalBank_Surface_142.asset");
  var shoreSections=JObject.Parse(File.ReadAllText(Out+"/shore-water-ground-profiles.json"))["sections"];
  var shoreEdges=shoreSections.Select(s=>(a:new Vector2((float)s["a"][0],(float)s["a"][2]),b:new Vector2((float)s["b"][0],(float)s["b"][2]))).ToArray();
  var dense=ConRoiV48R21TerrainPatch.Remesh(original,terrain.transform);var points=dense.vertices;int changed=0;float maxRaise=0;int capped=0,shoreChanged=0;
  // The SE junction contains the already authored V32 promenade. Keep its
  // smooth grade and shape the earth beneath it, rather than projecting that
  // pavement onto unrelated road/plinth grading peaks.
  var promenade=GameObject.Find("R20_Whole_Source_Export").GetComponentsInChildren<MeshFilter>().Single(f=>f.name=="Surface_153");
  var sampler=new GameObject("Temporary_Original_Promenade_Grade");sampler.transform.SetPositionAndRotation(promenade.transform.position,promenade.transform.rotation);sampler.transform.localScale=promenade.transform.lossyScale;
  var pathCollider=sampler.AddComponent<MeshCollider>();pathCollider.sharedMesh=AssetDatabase.LoadAllAssetsAtPath("Assets/Art/Environment/JourneyV48/ExportPolicyR20/RouteSurfaces.fbx").OfType<Mesh>().Single(m=>m.name=="Surface_153");Physics.SyncTransforms();int promenadeCaps=0;
  for(int i=0;i<points.Length;i++)
  {
   var p=terrain.transform.TransformPoint(points[i]);float target=p.y;
   if(p.z>=420&&p.z<=550)
   {
    var xz=new Vector2(p.x,p.z);float best=float.PositiveInfinity,signed=0;
    foreach(var edge in shoreEdges)
    {
     var e=edge.b-edge.a;var q=edge.a+e*Mathf.Clamp01(Vector2.Dot(xz-edge.a,e)/e.sqrMagnitude);float distance=(xz-q).sqrMagnitude;if(distance>=best)continue;
     var n=new Vector2(e.y,-e.x).normalized;if(n.x<0)n=-n;best=distance;signed=Vector2.Dot(xz-q,n);
    }
    float distanceToEdge=Mathf.Sqrt(best);
    if(signed>=-.6f&&distanceToEdge<=6)
    {
     float minimum=signed<0?.08f+signed*.8f:Mathf.Lerp(.08f,.26f,Mathf.Clamp01(signed/.6f));
     float fade=(1-Mathf.SmoothStep(0,1,Mathf.InverseLerp(.6f,6,distanceToEdge)))*Mathf.SmoothStep(0,1,Mathf.InverseLerp(420,430,p.z))*Mathf.SmoothStep(0,1,Mathf.InverseLerp(550,540,p.z));
     target=Mathf.Lerp(p.y,Mathf.Max(p.y,minimum),fade);if(target>p.y+.00001f)shoreChanged++;
    }
   }
   if(p.x>=610&&p.x<=725&&p.z>=470&&p.z<=565)
   {
    float ceiling=float.NegativeInfinity,lowestTop=float.PositiveInfinity;
    foreach(var s in shapes)
    {
     target=Mathf.Max(target,Target(s,p));var underside=FootprintCeiling(s,p);
     if(float.IsPositiveInfinity(underside))continue;
     ceiling=Mathf.Max(ceiling,underside);lowestTop=Mathf.Min(lowestTop,underside+s.inset-.02f);
    }
    // At overlapping road/plinth footprints, earth supports the upper paving
    // while staying below every visible top. A lower plinth underside must not
    // carve a hole underneath the adjoining sidewalk.
    ceiling=Mathf.Min(ceiling,lowestTop);
    if(!float.IsNegativeInfinity(ceiling)&&target>ceiling){target=ceiling;capped++;}
   }
   if(ConRoiV48R21JunctionGrade.Weight(p)>0&&!ConRoiV48R21JunctionGrade.InBridgeVoid(p))
   {
    bool found=false;float nearest=float.PositiveInfinity;Vector3 support=Vector3.zero;
    foreach(var offset in new[]{Vector2.zero,new Vector2(.75f,0),new Vector2(-.75f,0),new Vector2(0,.75f),new Vector2(0,-.75f),new Vector2(.75f,.75f),new Vector2(-.75f,.75f),new Vector2(.75f,-.75f),new Vector2(-.75f,-.75f)})
    {
     if(offset.sqrMagnitude>=nearest)continue;
     if(!pathCollider.Raycast(new Ray(new Vector3(p.x+offset.x,40,p.z+offset.y),Vector3.down),out var pathHit,100))continue;
     nearest=offset.sqrMagnitude;support=ConRoiV48R21JunctionGrade.Paving(pathHit.point);found=true;
    }
    if(found)
    {
     float blend=(1-Mathf.SmoothStep(0,1,Mathf.Sqrt(nearest)/1.5f))*ConRoiV48R21JunctionGrade.Weight(p);
     target=Mathf.Lerp(target,support.y-.08f,blend);promenadeCaps++;
    }
   }
   if(Mathf.Abs(target-p.y)<.00001f)continue;changed++;maxRaise=Mathf.Max(maxRaise,target-p.y);points[i]=terrain.transform.InverseTransformPoint(new Vector3(p.x,target,p.z));
  }
  UnityEngine.Object.DestroyImmediate(sampler);
  dense.name="R21_DenseGradedApproach_Surface_142";dense.vertices=points;dense.RecalculateNormals();dense.RecalculateBounds();
  AssetDatabase.CreateAsset(dense,Art+"/"+dense.name+".asset");terrain.sharedMesh=dense;terrain.GetComponent<MeshCollider>().sharedMesh=dense;
  EditorSceneManager.SaveScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene());AssetDatabase.SaveAssets();
  File.WriteAllText(Out+"/dense-local-grade-draft.json",new JObject{["status"]="DRAFT_REQUIRES_BUILD_VISUAL_CHECK",["source_triangles"]=original.triangles.Length/3,["triangles"]=dense.triangles.Length/3,["changed_vertices"]=changed,["shore_contact_vertices"]=shoreChanged,["ground_capped_under_authored_surfaces"]=capped,["se_promenade_grade_caps"]=promenadeCaps,["maximum_raise_m"]=maxRaise,["water_changed"]=false,["bridge_changed"]=false,["collider_shared_mesh"]=true}.ToString());
  ConRoiV48R21Footpaths.MatchChangedLocalGradeDraft();
  ConRoiV48R21.EnableAtmosphereDraftAndBuild();
 }
 public static void RepairAndMeasureDraft()
 {
  RegradeDenseDraft();
  ConRoiV48R21GeometryAudit.Audit();
 }
 public static void GradeApproachesDraft()
 {
  Directory.CreateDirectory(Art);EditorSceneManager.OpenScene("Assets/Scenes/JourneyRepair_v48_r21.unity");
  var source=JObject.Parse(File.ReadAllText(Out+"/source-cross-section-geometry.json"));var shapes=new List<Shape>();
  foreach(var o in source["objects"])
  {
   string name=(string)o["name"];
   if(!name.StartsWith("OD35_Approach_")&&!name.StartsWith("OD35_Head_Landscape_Plinth")&&!name.StartsWith("OD35_Main_Route_Side_Link"))continue;
   var s=new Shape{name=name,vertices=o["vertices"].Select(p=>new Vector3((float)p[0],(float)p[1],(float)p[2])).ToArray(),triangles=o["triangles"].SelectMany(t=>t.Values<int>()).ToArray(),inset=name.Contains("Plinth")?.302f:.12f};
   s.bounds=new Bounds(s.vertices[0],Vector3.zero);foreach(var p in s.vertices)s.bounds.Encapsulate(p);shapes.Add(s);
  }
  var links=new GameObject("R21_Planter_Sidewalk_Connections");
  var nw=new Vector3(685.9787f,1.14f,496.6172f);var axis=new Vector3(-.7636f,0,.6457f).normalized;var side=Vector3.Cross(Vector3.up,axis);
  var centres=new[]{new Vector3(697.781f,1.14f,499.731f),new Vector3(641.079f,1.14f,547.698f),new Vector3(684.864f,1.14f,484.462f),new Vector3(628.162f,1.14f,532.428f)};
  for(int j=0;j<centres.Length;j++)
  {
   float along=Vector3.Dot(centres[j]-nw,axis);float sign=Mathf.Sign(Vector3.Dot(centres[j]-nw,side));
   // Fill the measured two-metre disconnect between the existing sidewalk
   // outside edge and the long planting platform, at the same paving level.
   var vertices=new[]{nw+axis*(along-8)+side*(sign*7),nw+axis*(along+8)+side*(sign*7),nw+axis*(along+8)+side*(sign*9.05f),nw+axis*(along-8)+side*(sign*9.05f)};
   int[] tri={0,1,2,0,2,3};if(Vector3.Cross(vertices[1]-vertices[0],vertices[2]-vertices[0]).y<0)tri=new[]{2,1,0,3,2,0};
   var mesh=new Mesh{name="PlanterWalkConnection_"+j};mesh.vertices=vertices;mesh.triangles=tri;mesh.RecalculateNormals();mesh.RecalculateBounds();AssetDatabase.CreateAsset(mesh,Art+"/"+mesh.name+".asset");
   var g=new GameObject(mesh.name);g.layer=3;g.transform.SetParent(links.transform);g.AddComponent<MeshFilter>().sharedMesh=mesh;g.AddComponent<MeshRenderer>().sharedMaterial=AssetDatabase.LoadAssetAtPath<Material>("Assets/Art/Environment/OngDaoV35/OD35_Paving.mat");g.AddComponent<MeshCollider>().sharedMesh=mesh;
   shapes.Add(new Shape{name=mesh.name,vertices=vertices,triangles=tri,inset=.12f,bounds=mesh.bounds});
  }
  var rows=new JArray();var root=GameObject.Find("R20_Whole_Source_Export");
  foreach(var name in new[]{"Surface_142","Surface_176"})
  {
   var f=root.GetComponentsInChildren<MeshFilter>().Single(t=>t.name==name);var original=f.sharedMesh;var vertices=original.vertices;var changes=new JArray();
   for(int i=0;i<vertices.Length;i++)
   {
    var world=f.transform.TransformPoint(vertices[i]);if(world.x<610||world.x>725||world.z<470||world.z>565)continue;
    float target=world.y;foreach(var s in shapes)target=Mathf.Max(target,Target(s,world));
    if(target-world.y<.00001f)continue;
    var after=new Vector3(world.x,target,world.z);vertices[i]=f.transform.InverseTransformPoint(after);
    changes.Add(new JObject{["vertex"]=i,["before"]=V(world),["after"]=V(after)});
   }
   if(changes.Count==0)continue;
   var mesh=UnityEngine.Object.Instantiate(original);mesh.name="R21_GradedApproach_"+name;mesh.vertices=vertices;mesh.RecalculateNormals();mesh.RecalculateBounds();string path=Art+"/"+mesh.name+".asset";AssetDatabase.CreateAsset(mesh,path);f.sharedMesh=mesh;
   var collider=f.GetComponent<MeshCollider>();if(!collider){collider=f.gameObject.AddComponent<MeshCollider>();f.gameObject.layer=3;var oldName=name=="Surface_142"?"Context_125":"Context_124";foreach(var old in UnityEngine.Object.FindObjectsByType<MeshCollider>(FindObjectsInactive.Include).Where(c=>c.name==oldName))old.enabled=false;}collider.sharedMesh=mesh;
   rows.Add(new JObject{["renderer"]=name,["original"]=AssetDatabase.GetAssetPath(original),["derived"]=path,["changes"]=changes});
  }
  var disabled=new JArray();
  foreach(var f in UnityEngine.Object.FindObjectsByType<MeshFilter>(FindObjectsInactive.Include))
  {
   bool replace=f.name.StartsWith("R11_Foundation_OD35_Approach_")||f.name.StartsWith("R11_Foundation_OD35_Main_Route_Side_Link_")||f.name.StartsWith("R11_V32_Approach_Edge_Foundation_")||Enumerable.Range(0,4).Any(i=>f.name=="R15_Planter_Base_"+i);
   if(!replace)continue;f.gameObject.SetActive(false);disabled.Add(f.name);
  }
  var scenery=UnityEngine.Object.FindAnyObjectByType<CanhQuanChoV48>();scenery.scenery=scenery.scenery.Concat(new[]{links}).ToArray();
  EditorSceneManager.SaveScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene());AssetDatabase.SaveAssets();
  File.WriteAllText(Out+"/approach-landscape-draft.json",new JObject{["status"]="DRAFT_REQUIRES_VISUAL_CONTACT_AND_TRAVERSAL_VERIFICATION",["bridge_deck_unchanged"]=true,["water_unchanged"]=true,["earthworks_landward_feather_m"]=5,["replaced_vertical_patches"]=disabled,["shapes"]=new JArray(shapes.Select(s=>new JObject{["name"]=s.name,["min"]=V(s.bounds.min),["max"]=V(s.bounds.max),["underside_inset_m"]=s.inset})),["terrain_changes"]=rows}.ToString());
 }
}
