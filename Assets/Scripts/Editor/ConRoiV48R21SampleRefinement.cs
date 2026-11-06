using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public static class ConRoiV48R21SampleRefinement
{
 const string Scene="Assets/Scenes/JourneyRepair_v48_r21.unity",Out="Docs/Reviews/journey_v48/r21/asset-pass";
 public static void RefineAndDiagnose()
 {
  EditorSceneManager.OpenScene(Scene);
  var root=GameObject.Find("R21_RealPlanterFlowerSample");
  var original=root.GetComponentsInChildren<MeshFilter>().OrderBy(f=>f.name).ToArray();
  if(original.Length!=68)throw new Exception("Preserve existing refinement: expected original 68 modules");
  var record=JObject.Parse(File.ReadAllText(Out+"/real-planter-sample.json"));
  var centre=new Vector3(641.079163f,1.55758f,547.6977f);var axis=new Vector3(.763470232f,0,-.6458431f);var across=Vector3.Cross(Vector3.up,axis);
  var meshes=original.Select(f=>f.sharedMesh).Distinct().ToArray();if(meshes.Length!=1)throw new Exception("Expected shared module mesh");
  var materials=original.SelectMany(f=>f.GetComponent<Renderer>().sharedMaterials).Distinct().ToArray();
  var red=materials.Single(m=>m.name=="PetalRed");red.SetColor("_BaseColor",new Color(.67f,.065f,.095f));EditorUtility.SetDirty(red);
  var yellow=materials.Single(m=>m.name=="PetalYellow");yellow.SetColor("_BaseColor",new Color(.88f,.57f,.085f));EditorUtility.SetDirty(yellow);
  int n=0;var placements=new JArray();
  for(int row=0;row<3;row++)for(int col=0;col<34;col++)
  {
   var g=n<original.Length?original[n].gameObject:UnityEngine.Object.Instantiate(original[n%original.Length].gameObject,root.transform);
   g.name="FlowerModule_"+n;
   float along=Mathf.Lerp(-5.17f,5.17f,(col+.5f)/34)+Mathf.Sin(col*2.73f+row*1.21f)*.045f;
   float side=(row-1)*.315f+Mathf.Sin(col*1.71f+row*2.35f)*.045f;
   g.transform.position=centre+axis*along+across*side;
   g.transform.rotation=Quaternion.Euler(0,(col*137+row*71)%360,0);
   g.transform.localScale=Vector3.one*(1.08f+.04f*((col+row*3)%5));
   var renderer=g.GetComponent<MeshRenderer>();renderer.sharedMaterials=renderer.sharedMaterials.Select(m=>m.name.StartsWith("Petal")?((col+row*2)%5==0?yellow:red):m).ToArray();
   placements.Add(new JObject{["position"]=new JArray(g.transform.position.x,g.transform.position.y,g.transform.position.z),["scale"]=g.transform.localScale.x});n++;
  }
  record["previous_modules"]=68;record["modules"]=n;record["triangles"]=n*meshes[0].triangles.Length/3;record["shared_mesh"]=AssetDatabase.GetAssetPath(meshes[0]);record["instances"]=placements;
  record["refinement"]="Three staggered rows, bounded deterministic variation, taller blooms and brighter petals; still one real planter, no propagation";
  File.WriteAllText(Out+"/real-planter-sample-refined.json",record.ToString());
  EditorSceneManager.SaveScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene());AssetDatabase.SaveAssets();
  ConRoiV48R21AssetPass.PreviewSample();DiagnoseEdges();ConRoiV48R21AssetPass.AuditBoundaries();
 }
 public static void DiagnoseEdges()
 {
  EditorSceneManager.OpenScene(Scene);var story=UnityEngine.Object.FindAnyObjectByType<DieuPhoiHanhTrinh>();story.hub.SetActive(false);story.lake.SetActive(true);story.market.SetActive(true);foreach(var g in story.GetComponent<CanhQuanChoV48>().scenery)g.SetActive(true);
  var camera=new GameObject("ProbeCamera").AddComponent<Camera>();camera.aspect=1600f/900;camera.fieldOfView=60;camera.transform.SetPositionAndRotation(new Vector3(619,1.7f,554),Quaternion.LookRotation(new Vector3(638,1.05f,542)-new Vector3(619,1.7f,554)));
  var probes=new System.Collections.Generic.List<(MeshCollider c,string name,string asset)>();
  foreach(var f in UnityEngine.Object.FindObjectsByType<MeshFilter>(FindObjectsInactive.Include))
  {
   var r=f.GetComponent<Renderer>();if(!r||!r.enabled||!r.gameObject.activeInHierarchy||!f.sharedMesh||!r.bounds.Intersects(new Bounds(new Vector3(635,1,549),new Vector3(55,15,55))))continue;
   var p=new GameObject("probe");p.transform.SetPositionAndRotation(f.transform.position,f.transform.rotation);p.transform.localScale=f.transform.lossyScale;var c=p.AddComponent<MeshCollider>();c.sharedMesh=f.sharedMesh;
   probes.Add((c,f.name,AssetDatabase.GetAssetPath(f.sharedMesh)));
  }
  Physics.SyncTransforms();bool back=Physics.queriesHitBackfaces;Physics.queriesHitBackfaces=true;var rows=new JArray();
  foreach(int y in new[]{492,500,508,516,524})foreach(int x in new[]{80,140,200,260,320,380,440,500})
  {
   var ray=camera.ViewportPointToRay(new Vector3(x/1600f,1-y/900f,0));var hits=new JArray();
   foreach(var p in probes){if(p.c.Raycast(ray,out var hit,150))hits.Add(new JObject{["name"]=p.name,["mesh"]=p.asset,["distance"]=hit.distance,["point"]=new JArray(hit.point.x,hit.point.y,hit.point.z)});}
   rows.Add(new JObject{["pixel"]=new JArray(x,y),["hits"]=new JArray(hits.OrderBy(h=>(float)h["distance"]).Take(5))});
  }
  Physics.queriesHitBackfaces=back;File.WriteAllText(Out+"/se-slit-ray-diagnosis.json",rows.ToString());
  EditorSceneManager.OpenScene(Scene);
 }
}
