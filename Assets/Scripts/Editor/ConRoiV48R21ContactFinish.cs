using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class ConRoiV48R21ContactFinish
{
 public static void Repair()
 {
  const string outdir="Docs/Reviews/journey_v48/r21/asset-pass",art="Assets/Art/Environment/JourneyV48/ContactFinishR21";
  EditorSceneManager.OpenScene("Assets/Scenes/JourneyRepair_v48_r21.unity");Directory.CreateDirectory(art);
  var terrain=GameObject.Find("R20_Whole_Source_Export").GetComponentsInChildren<MeshFilter>().Single(f=>f.name=="Surface_142");
  var old=terrain.sharedMesh;var points=old.vertices;var audit=JObject.Parse(File.ReadAllText(outdir+"/geometry-contact-audit.json"));
  var target=audit["approaches_and_plinths"].Single(g=>(string)g["name"]=="OD35_Approach_Sidewalk.001")["samples"].Where(s=>(bool)s["support"]&&(float)s["underside_target_gap_m"]>.015f).ToArray();
  var changes=new JArray();
  for(int i=0;i<points.Length;i++)
  {
   var w=terrain.transform.TransformPoint(points[i]);if(w.x<610||w.x>650||w.z<537||w.z>566)continue;float y=w.y;
   foreach(var s in target)
   {
    float distance=Vector2.Distance(new Vector2(w.x,w.z),new Vector2((float)s["x"],(float)s["z"]));if(distance>1.5f)continue;
    float desired=(float)s["source_top_y"]-.125f;float weight=1-Mathf.SmoothStep(0,1,Mathf.InverseLerp(.6f,1.5f,distance));y=Mathf.Max(y,Mathf.Lerp(w.y,desired,weight));
   }
   if(y>w.y+.00001f){points[i]=terrain.transform.InverseTransformPoint(new Vector3(w.x,y,w.z));changes.Add(new JObject{["x"]=w.x,["z"]=w.z,["before"]=w.y,["after"]=y});}
  }
  if(changes.Count>0)
  {
   var mesh=UnityEngine.Object.Instantiate(old);mesh.name="R21_FinalLocalSidewalkSupport_Refined";mesh.vertices=points;mesh.RecalculateNormals();mesh.RecalculateBounds();AssetDatabase.CreateAsset(mesh,art+"/"+mesh.name+".asset");terrain.sharedMesh=mesh;var collider=terrain.GetComponent<MeshCollider>();collider.sharedMesh=mesh;
  }
  EditorSceneManager.SaveScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene());AssetDatabase.SaveAssets();
  File.WriteAllText(outdir+"/sidewalk-final-support-refined.json",new JObject{["status"]="LOCAL_REPAIR_REQUIRES_MEASURE_AND_VISUAL_CHECK",["source_mesh"]=AssetDatabase.GetAssetPath(old),["radius_m"]=1.5,["water_level_changed"]=false,["changes"]=changes}.ToString());
  ConRoiV48R21AssetPass.BuildReview();
 }
 public static void ExtendBankBlend()
 {
  const string outdir="Docs/Reviews/journey_v48/r21/asset-pass",art="Assets/Art/Environment/JourneyV48/ContactFinishR21";
  EditorSceneManager.OpenScene("Assets/Scenes/JourneyRepair_v48_r21.unity");var edges=JObject.Parse(File.ReadAllText("Docs/Reviews/journey_v48/r21/shore-water-ground-profiles.json"))["sections"].Select(e=>(a:new Vector2((float)e["a"][0],(float)e["a"][2]),b:new Vector2((float)e["b"][0],(float)e["b"][2]))).ToArray();
  var rows=new JArray();var se=new Vector2(642.2553f,533.6046f);var nw=new Vector2(685.9787f,496.6172f);var axis=(nw-se).normalized;float span=Vector2.Distance(se,nw);
  foreach(var f in GameObject.Find("R20_Whole_Source_Export").GetComponentsInChildren<MeshFilter>().Where(f=>f.name=="Surface_142"||f.name=="Surface_176"))
  {
   var source=f.sharedMesh;var working=f.name=="Surface_142"?ConRoiV48R21TerrainPatch.Remesh(source,f.transform,new[]{new Rect(630,398,28,32)},outdir+"/southern-bank-remesh-closed.json"):UnityEngine.Object.Instantiate(source);var points=working.vertices;int changed=0;
   for(int i=0;i<points.Length;i++)
   {
    var p=f.transform.TransformPoint(points[i]);if(p.z<398||p.z>430)continue;var q=new Vector2(p.x,p.z);var delta=q-se;float along=Vector2.Dot(delta,axis),side=Mathf.Abs(delta.x*axis.y-delta.y*axis.x);if(along>0&&along<span&&side<9)continue;
    float best=float.PositiveInfinity;Vector2 nearest=Vector2.zero,normal=Vector2.zero;
    foreach(var e in edges){var v=e.b-e.a;var n=e.a+v*Mathf.Clamp01(Vector2.Dot(q-e.a,v)/v.sqrMagnitude);float d=Vector2.Distance(q,n);if(d>=best)continue;best=d;nearest=n;normal=new Vector2(v.y,-v.x).normalized;if(normal.x<0)normal=-normal;}
    float signed=Vector2.Dot(q-nearest,normal);if(best>6||signed<-.75f)continue;
    float weight=(1-Mathf.SmoothStep(0,1,Mathf.InverseLerp(.8f,6,best)))*Mathf.SmoothStep(0,1,Mathf.InverseLerp(398,403,p.z))*Mathf.SmoothStep(0,1,Mathf.InverseLerp(-.75f,-.25f,signed));float y=Mathf.Lerp(p.y,Mathf.Max(p.y,.26f),weight);if(y-p.y<.00001f)continue;
    points[i]=f.transform.InverseTransformPoint(new Vector3(p.x,y,p.z));changed++;
   }
   if(changed==0){UnityEngine.Object.DestroyImmediate(working);continue;}var mesh=working;mesh.name="R21_ShoreBlendClosed_"+f.name;mesh.vertices=points;mesh.RecalculateNormals();mesh.RecalculateBounds();AssetDatabase.CreateAsset(mesh,art+"/"+mesh.name+".asset");f.sharedMesh=mesh;var collider=f.GetComponent<MeshCollider>();if(collider)collider.sharedMesh=mesh;
   rows.Add(new JObject{["object"]=f.name,["changed_vertices"]=changed,["source"]=AssetDatabase.GetAssetPath(source),["mesh"]=AssetDatabase.GetAssetPath(mesh)});
  }
  EditorSceneManager.SaveScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene());AssetDatabase.SaveAssets();File.WriteAllText(outdir+"/shore-blend-closed.json",new JObject{["status"]="LOCAL_SHORE_EXTENT_REPAIR_REQUIRES_VISUAL_VERIFICATION",["z"]=new JArray(398,430),["bank_overlap_m"]=.75,["reason"]="Shoreline cannot end between a raised dry vertex and an unchanged submerged vertex; grade the narrow contact strip as part of terrain",["water_unchanged"]=true,["meshes"]=rows}.ToString());ConRoiV48R21AssetPass.BuildReview();
 }
}
