using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class ConRoiV48R21RegroundBoundary
{
 public static void RepairAndBuild()
 {
  const string Out="Docs/Reviews/journey_v48/r21/asset-pass",Art="Assets/Art/Environment/JourneyV48/LakeMarketBoundaryR21";
  EditorSceneManager.OpenScene("Assets/Scenes/JourneyRepair_v48_r21.unity");var root=GameObject.Find("R21_Actual_Terrain_Perimeter");
  var data=JObject.Parse(File.ReadAllText(Out+"/actual-terrain-boundary.json"));File.Copy(Out+"/actual-terrain-boundary.json",Out+"/actual-terrain-boundary-before-reground.json",false);
  var probes=ConRoiV48R21TerrainGround.CreateProbes();Physics.SyncTransforms();
  var temp=GameObject.CreatePrimitive(PrimitiveType.Cube);var cube=temp.GetComponent<MeshFilter>().sharedMesh;UnityEngine.Object.DestroyImmediate(temp);
  var changed=new JArray();var missing=new JArray();int index=0;
  AssetDatabase.StartAssetEditing();
  try {
   foreach(var row in data["panels"])
   {
    var a=new Vector2((float)row["a"][0],(float)row["a"][2]);var b=new Vector2((float)row["b"][0],(float)row["b"][2]);var mid=(a+b)/2;
    if(!ConRoiV48R21TerrainGround.Sample(probes,a,out float ya)||!ConRoiV48R21TerrainGround.Sample(probes,b,out float yb)||!ConRoiV48R21TerrainGround.Sample(probes,mid,out float ym))throw new Exception("Actual terrain support missing at panel "+index);
    var filter=root.transform.Find("ActualTerrainFence_"+index).GetComponent<MeshFilter>();float top=Mathf.Max(ya,yb,ym)+3.8f;
    if(Mathf.Abs((float)row["a"][1]-ya)>.05f||Mathf.Abs((float)row["b"][1]-yb)>.05f||Mathf.Abs((float)row["top_y"]-top)>.05f)changed.Add(new JObject{["panel"]=index,["old_a_y"]=row["a"][1],["old_b_y"]=row["b"][1],["actual_a_y"]=ya,["actual_b_y"]=yb,["old_top_y"]=row["top_y"],["actual_top_y"]=top});
    var origin=new Vector3(mid.x,Mathf.Min(ya,yb,ym)-.06f,mid.y);var pieces=new List<CombineInstance>();
    void Box(Vector3 p,Vector3 size,Quaternion rotation){pieces.Add(new CombineInstance{mesh=cube,transform=Matrix4x4.TRS(p-origin,rotation,size)});}
    void Post(Vector2 p,float y,float width){float bottom=y-.06f;Box(new Vector3(p.x,(bottom+top)/2,p.y),new Vector3(width,top-bottom,width),Quaternion.identity);}
    Post(a,ya,.075f);Post(b,yb,.075f);int bars=Mathf.CeilToInt(Vector2.Distance(a,b)/.16f);
    for(int j=1;j<bars;j++){float t=j/(float)bars;var p=Vector2.Lerp(a,b,t);if(!ConRoiV48R21TerrainGround.Sample(probes,p,out float y)){y=Mathf.Min(ya,yb,ym);missing.Add(new JObject{["panel"]=index,["at"]=new JArray(p.x,p.y)});}Post(p,y,.025f);}
    foreach(float y in new[]{top-.08f,top-1.1f,Mathf.Max(ya,yb,ym)+.16f}){var p=new Vector3(a.x,y,a.y);var q=new Vector3(b.x,y,b.y);Box((p+q)/2,new Vector3(.035f,Vector3.Distance(p,q),.035f),Quaternion.FromToRotation(Vector3.up,q-p));}
    var mesh=new Mesh{name="ActualTerrainFenceGrounded_"+index};mesh.CombineMeshes(pieces.ToArray(),true,true);mesh.RecalculateBounds();AssetDatabase.CreateAsset(mesh,Art+"/"+mesh.name+".asset");filter.transform.position=origin;filter.sharedMesh=mesh;filter.GetComponent<MeshCollider>().sharedMesh=mesh;
    row["a"][1]=ya;row["b"][1]=yb;row["top_y"]=top;index++;
   }
  } finally {AssetDatabase.StopAssetEditing();ConRoiV48R21TerrainGround.Dispose(probes);}
  data["ground_basis"]="Direct raycasts against actual Surface_142 and Surface_176 meshes only, excluding tree canopies/roofs; every picket sampled separately";
  File.WriteAllText(Out+"/actual-terrain-boundary.json",data.ToString());
  File.WriteAllText(Out+"/reground-boundary-to-terrain.json",new JObject{["panels"]=index,["endpoint_or_top_changes_over_5cm"]=changed,["missing_interior_picket_support"]=missing,["source_terrain_unchanged"]=true}.ToString());
  EditorSceneManager.SaveScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene());AssetDatabase.SaveAssets();
  File.Copy(Out+"/boundary-capsule-intercepts.json",Out+"/boundary-capsule-before-reground.json",false);
  ConRoiV48R21BoundaryCapsule.FinishAndAudit();ConRoiV48R21AssetPass.BuildReview();
 }
}
