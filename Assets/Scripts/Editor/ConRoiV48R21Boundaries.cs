using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public static class ConRoiV48R21Boundaries
{
 const string Out="Docs/Reviews/journey_v48/r21",Art="Assets/Art/Environment/JourneyV48/BoundaryR21";
 static JArray V(Vector3 p)=>new JArray(p.x,p.y,p.z);
 public static void SchoolAndSlopeDraft()
 {
  EditorSceneManager.OpenScene("Assets/Scenes/JourneyRepair_v48_r21.unity");Directory.CreateDirectory(Art);
  // Chapter roots are normally inactive while authoring. GameObject.Find
  // misses their old fences, leaving the previous layout on top of the new one.
  var generatedNames=new[]{"R21_School_Garden_Perimeter","R21_Slope_House_Frontage_Fences"};
  var previousRoots=UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects()
   .SelectMany(g=>g.GetComponentsInChildren<Transform>(true)).Where(t=>generatedNames.Contains(t.name)).ToArray();
  foreach(var previous in previousRoots)UnityEngine.Object.DestroyImmediate(previous.gameObject);
  var story=UnityEngine.Object.FindAnyObjectByType<DieuPhoiHanhTrinh>();
  var material=AssetDatabase.LoadAssetAtPath<Material>("Assets/Art/Environment/OngDaoV35/OD35_GreenMetal.mat");
  var temporary=GameObject.CreatePrimitive(PrimitiveType.Cube);var cube=temporary.GetComponent<MeshFilter>().sharedMesh;UnityEngine.Object.DestroyImmediate(temporary);
  var rows=new JArray();int serial=0;
  void Fence(Transform parent,string name,Vector3 a,Vector3 b,float height)
  {
   int count=Mathf.CeilToInt(Vector3.Distance(a,b)/2);
   for(int part=0;part<count;part++)
   {
    var start=Vector3.Lerp(a,b,part/(float)count);var end=Vector3.Lerp(a,b,(part+1)/(float)count);var centre=(start+end)*.5f;
    var pieces=new List<CombineInstance>();
    void Box(Vector3 position,Vector3 scale,Quaternion rotation){pieces.Add(new CombineInstance{mesh=cube,transform=Matrix4x4.TRS(position-centre,rotation,scale)});}
    void Rail(Vector3 x,Vector3 y,float width){var delta=y-x;Box((x+y)*.5f,new Vector3(width,delta.magnitude,width),Quaternion.FromToRotation(Vector3.up,delta));}
    Box(start+Vector3.up*height*.5f,new Vector3(.085f,height,.085f),Quaternion.identity);
    Box(end+Vector3.up*height*.5f,new Vector3(.085f,height,.085f),Quaternion.identity);
    foreach(float h in new[]{.18f,height*.46f,height-.05f})Rail(start+Vector3.up*h,end+Vector3.up*h,.045f);
    int bars=Mathf.CeilToInt(Vector3.Distance(start,end)/.18f);
    for(int j=1;j<bars;j++)Box(Vector3.Lerp(start,end,j/(float)bars)+Vector3.up*height*.5f,new Vector3(.025f,height,.025f),Quaternion.identity);
    var mesh=new Mesh{name="R21_Fence_"+(serial++)};mesh.CombineMeshes(pieces.ToArray(),true,true);mesh.RecalculateBounds();AssetDatabase.CreateAsset(mesh,Art+"/"+mesh.name+".asset");
    var g=new GameObject(name+"_"+part);g.layer=3;g.transform.SetParent(parent,true);g.transform.position=centre;g.AddComponent<MeshFilter>().sharedMesh=mesh;g.AddComponent<MeshRenderer>().sharedMaterial=material;g.AddComponent<MeshCollider>().sharedMesh=mesh;
   }
   rows.Add(new JObject{["name"]=name,["kind"]="fixed_map_boundary",["start"]=V(a),["end"]=V(b),["height_m"]=height,["panels"]=count,["collider"]="exact visible fence mesh; no invisible wall",["progression_dependency"]=false});
  }
  void Sign(Transform parent,string text,Vector3 position,Vector3 facing)
  {
   var g=new GameObject("Boundary_Reason_"+text);g.transform.SetParent(parent,true);g.transform.position=position;g.transform.rotation=Quaternion.LookRotation(-facing);
   var panel=GameObject.CreatePrimitive(PrimitiveType.Cube);panel.transform.SetParent(g.transform,false);panel.transform.localScale=new Vector3(2.3f,.55f,.06f);panel.GetComponent<Renderer>().sharedMaterial=material;UnityEngine.Object.DestroyImmediate(panel.GetComponent<Collider>());
   var label=new GameObject("Lettering").AddComponent<TextMesh>();label.transform.SetParent(g.transform,false);label.transform.localPosition=new Vector3(0,0,-.036f);label.text=text;label.font=Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");label.fontSize=56;label.characterSize=.028f;label.anchor=TextAnchor.MiddleCenter;label.alignment=TextAlignment.Center;label.color=new Color(.94f,.91f,.75f);label.GetComponent<Renderer>().sharedMaterial=label.font.material;
  }
  var school=new GameObject("R21_School_Garden_Perimeter").transform;school.SetParent(story.schoolChapter.environment.transform,true);
  var origin=new Vector3(-95,0,-45);
  // Set fences on the inner faces of the low walls so their top surfaces do
  // not become intermediate stepping ledges over the outer perimeter.
  Fence(school,"West_Garden_Fence",origin+new Vector3(-31.8f,0,-10.6f),origin+new Vector3(-31.8f,0,-2),3.5f);
  Fence(school,"Rear_Garden_Fence",origin+new Vector3(-31.8f,0,-10.6f),origin+new Vector3(8.5f,0,-10.6f),3.5f);
  Fence(school,"East_Garden_Fence",origin+new Vector3(8.5f,0,-10.6f),origin+new Vector3(8.5f,0,0),3.5f);
  Sign(school,"VƯỜN ĐANG ĐÓNG\n← Phòng tư liệu",origin+new Vector3(5,1.7f,-10.74f),Vector3.forward);
  var slope=new GameObject("R21_Slope_House_Frontage_Fences").transform;slope.SetParent(story.slopeChapter.environment.transform,true);origin=new Vector3(-180,0,-60);
  float Height(float z)=>Mathf.Clamp(z,0,60)*.1f;
  // Existing houses occupy z=4..12,16..24,...52..60, with their street
  // facades at |x|=5.5. Close only the garden gaps; do not fence in front of
  // every window and door or turn the street into a barred corridor.
  foreach(int side in new[]{-1,1})foreach(var interval in new[]{(0,4),(12,16),(24,28),(36,40),(48,52),(60,78)})
   for(int z=interval.Item1;z<interval.Item2;z+=2)
    Fence(slope,"Private_Garden_Gap_"+side+"_"+z,origin+new Vector3(side*5.45f,Height(z)+.03f,z),origin+new Vector3(side*5.45f,Height(z+2)+.03f,z+2),3.4f);
  Fence(slope,"Lower_Closed_Service_Gate",origin+new Vector3(-5.45f,.03f,.1f),origin+new Vector3(5.45f,.03f,.1f),3.4f);
  Fence(slope,"Upper_Closed_Courtyard_Gate",origin+new Vector3(-5.45f,6.03f,78),origin+new Vector3(5.45f,6.03f,78),3.4f);
  Sign(slope,"SÂN NHÀ DÂN ĐÓNG\nTheo dốc tới cửa cuối",origin+new Vector3(-5.40f,Height(26)+1.65f,26),Vector3.right);
  Sign(slope,"SÂN SAU ĐANG ĐÓNG",origin+new Vector3(3.1f,7.65f,77.9f),Vector3.back);
  story.GetComponent<HuongDanHanhTrinhV48>().regionalBoundaryHints=true;
  EditorSceneManager.SaveScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene());AssetDatabase.SaveAssets();
  var rootCounts=generatedNames.ToDictionary(n=>n,n=>UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects().SelectMany(g=>g.GetComponentsInChildren<Transform>(true)).Count(t=>t.name==n));
  if(rootCounts.Values.Any(count=>count!=1))throw new Exception("Generated chapter boundary roots must be unique, including inactive chapters");
  File.WriteAllText(Out+"/regional-boundaries-draft.json",new JObject{["status"]="DRAFT_REQUIRES_NATIVE_EXPLORATION_AND_SAVE_LOAD",["removed_previous_roots_including_inactive"]=previousRoots.Length,["generated_root_counts"]=JObject.FromObject(rootCounts),["school_and_slope"]=rows,["lake_market_and_hub_full_boundary_audit"]="PENDING",["optional_exploration_retained"]="Classroom, archive and courtyard; slope documents, suitcase and NPC remain on accessible side",["story_state_or_ending_rules_changed"]=false,["new_progression_locks_added"]=false}.ToString());
 }
 public static void RefineAndBuildDraft()
 {
  SchoolAndSlopeDraft();ConRoiV48R21Footpaths.ConformAndBuildDraft();
 }
 public static void RemoveDuplicateLayoutsAndBuildDraft()
 {
  SchoolAndSlopeDraft();ConRoiV48R21BoundaryAudit.Audit();ConRoiV48R21.BuildWorking();
 }
}
