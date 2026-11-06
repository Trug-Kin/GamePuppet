using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

// Geometry-derived trial boundaries. No movement, save, quest or progression changes.
public static class ConRoiV48R21LakeMarketBoundary
{
 const string Out="Docs/Reviews/journey_v48/r21/asset-pass",Art="Assets/Art/Environment/JourneyV48/LakeMarketBoundaryR21";
 static float Cross(Vector2 a,Vector2 b,Vector2 c)=>(b.x-a.x)*(c.y-a.y)-(b.y-a.y)*(c.x-a.x);
 public static void PlaceDraft()
 {
  EditorSceneManager.OpenScene("Assets/Scenes/JourneyRepair_v48_r21.unity");
  if(GameObject.Find("R21_LakeMarket_Measured_Perimeter"))throw new Exception("Preserve prior perimeter trial");
  var story=UnityEngine.Object.FindAnyObjectByType<DieuPhoiHanhTrinh>();var lakeActive=story.lake.activeSelf;var marketActive=story.market.activeSelf;
  story.lake.SetActive(true);story.market.SetActive(true);Physics.SyncTransforms();
  Directory.CreateDirectory(Art);var root=new GameObject("R21_LakeMarket_Measured_Perimeter");
  var material=AssetDatabase.LoadAssetAtPath<Material>("Assets/Art/Environment/OngDaoV35/OD35_GreenMetal.mat");
  var temp=GameObject.CreatePrimitive(PrimitiveType.Cube);var cube=temp.GetComponent<MeshFilter>().sharedMesh;UnityEngine.Object.DestroyImmediate(temp);
  var survey=JObject.Parse(File.ReadAllText(Out+"/envelope-survey.json"));
  var points=survey["surfaces"].Where(s=>(string)s["name"]!="Surface_153").SelectMany(s=>s["vertices"]).Select(v=>new Vector2((float)v[0],(float)v[2])).Distinct().OrderBy(p=>p.x).ThenBy(p=>p.y).ToArray();
  var hull=new List<Vector2>();foreach(var p in points){while(hull.Count>=2&&Cross(hull[^2],hull[^1],p)<=0)hull.RemoveAt(hull.Count-1);hull.Add(p);}int lower=hull.Count;
  foreach(var p in points.Reverse()){while(hull.Count>lower&&Cross(hull[^2],hull[^1],p)<=0)hull.RemoveAt(hull.Count-1);hull.Add(p);}hull.RemoveAt(hull.Count-1);
  var centre=hull.Aggregate(Vector2.zero,(a,b)=>a+b)/hull.Count;
  bool Ground(Vector2 p,out float y)
  {
   var hits=Physics.RaycastAll(new Vector3(p.x,60,p.y),Vector3.down,120,8,QueryTriggerInteraction.Ignore).Where(h=>h.normal.y>.7f&&!(h.collider.transform.IsChildOf(root.transform))).OrderByDescending(h=>h.point.y).ToArray();
   y=hits.Length>0?hits[0].point.y:0;return hits.Length>0;
  }
  var rail=new JArray();var skipped=new JArray();int serial=0;
  void Panel(Vector2 a,Vector2 b,string kind,float height)
  {
   var midpoint=(a+b)/2;
   if(!Ground(a,out float ya)||!Ground(b,out float yb)||!Ground(midpoint,out float ym)||Mathf.Min(ya,yb,ym)<-.05f){skipped.Add(new JObject{["kind"]=kind,["at"]=new JArray(midpoint.x,midpoint.y),["reason"]="No measured dry support; no floating barrier"});return;}
   var start=new Vector3(a.x,ya-.04f,a.y);var end=new Vector3(b.x,yb-.04f,b.y);var origin=(start+end)/2;var pieces=new List<CombineInstance>();
   void Box(Vector3 p,Vector3 size,Quaternion rotation){pieces.Add(new CombineInstance{mesh=cube,transform=Matrix4x4.TRS(p-origin,rotation,size)});}
   void Bar(Vector3 p,Vector3 q,float width){Box((p+q)/2,new Vector3(width,Vector3.Distance(p,q),width),Quaternion.FromToRotation(Vector3.up,q-p));}
   float baseRaise=Mathf.Max(0,ym-(ya+yb)/2);
   Box(start+Vector3.up*(height/2+baseRaise),new Vector3(.075f,height,.075f),Quaternion.identity);Box(end+Vector3.up*(height/2+baseRaise),new Vector3(.075f,height,.075f),Quaternion.identity);
   foreach(float h in new[]{.16f,1.05f,height-.08f})Bar(start+Vector3.up*(h+baseRaise),end+Vector3.up*(h+baseRaise),.035f);
   int count=Mathf.CeilToInt(Vector2.Distance(a,b)/.16f);for(int i=1;i<count;i++)Box(Vector3.Lerp(start,end,i/(float)count)+Vector3.up*(height/2+baseRaise),new Vector3(.025f,height,.025f),Quaternion.identity);
   var mesh=new Mesh{name="MeasuredPerimeter_"+serial++};mesh.CombineMeshes(pieces.ToArray(),true,true);mesh.RecalculateBounds();AssetDatabase.CreateAsset(mesh,Art+"/"+mesh.name+".asset");
   var g=new GameObject(kind+"_"+serial);g.layer=3;g.transform.SetParent(root.transform,true);g.transform.position=origin;g.AddComponent<MeshFilter>().sharedMesh=mesh;g.AddComponent<MeshRenderer>().sharedMaterial=material;g.AddComponent<MeshCollider>().sharedMesh=mesh;
   GameObjectUtility.SetStaticEditorFlags(g,StaticEditorFlags.BatchingStatic);
   rail.Add(new JObject{["kind"]=kind,["a"]=new JArray(start.x,start.y,start.z),["b"]=new JArray(end.x,end.y,end.z),["height_m"]=height,["same_visible_collision_mesh"]=true});
  }
  // The outer footprint is measured from the existing terrain, preserving the
  // entire interior exploration area. Small inset keeps posts on real terrain.
  for(int k=0;k<hull.Count;k++)
  {
   var a=Vector2.MoveTowards(hull[k],centre,1.5f);var b=Vector2.MoveTowards(hull[(k+1)%hull.Count],centre,1.5f);int n=Mathf.CeilToInt(Vector2.Distance(a,b)/2);
   for(int i=0;i<n;i++)Panel(Vector2.Lerp(a,b,i/(float)n),Vector2.Lerp(a,b,(i+1)/(float)n),"Outer_Park_Service_Fence",3.8f);
  }
  // Only the actual lake edge that meets dry terrain. Bridge deck/rail and Bao
  // are retained intact; the bridge itself still needs native jump verification.
  var lake=JObject.Parse(File.ReadAllText(Out+"/lake-actual-boundary.json"));
  var nw=new Vector2(685.9787f,496.6172f);var se=new Vector2(642.2553f,533.6046f);var axis=(nw-se).normalized;float length=Vector2.Distance(nw,se);
  // r91: rejected invented lakeshore fence. Keep the historical bridge rails
  // and outer perimeter; this producer must never recreate the shore panels.
  foreach(var e in Array.Empty<JToken>())
  {
   var a=new Vector2((float)e["a"][0],(float)e["a"][2]);var b=new Vector2((float)e["b"][0],(float)e["b"][2]);var normal=new Vector2(-(b-a).y,(b-a).x).normalized;var midpoint=(a+b)/2;
   bool ga=Ground(midpoint+normal*.8f,out float ya),gb=Ground(midpoint-normal*.8f,out float yb);if(!ga&&!gb)continue;if(!ga||gb&&yb>ya)normal=-normal;
   a+=normal*.8f;b+=normal*.8f;int n=Mathf.CeilToInt(Vector2.Distance(a,b)/2);
   for(int i=0;i<n;i++)
   {
    var p=Vector2.Lerp(a,b,i/(float)n);var q=Vector2.Lerp(a,b,(i+1)/(float)n);var m=(p+q)/2;var delta=m-se;float along=Vector2.Dot(delta,axis);float side=Mathf.Abs(delta.x*axis.y-delta.y*axis.x);
    if(along>-4&&along<length+4&&side<10){skipped.Add(new JObject{["kind"]="Shore",["at"]=new JArray(m.x,m.y),["reason"]="Preserve bridge footprint and existing leaning rail"});continue;}
    if(m.x<465||m.x>745||m.y<255||m.y>590)continue;
    Panel(p,q,"Lake_Bank_Maintenance_Fence",3.8f);
   }
  }
  story.lake.SetActive(lakeActive);story.market.SetActive(marketActive);
  EditorSceneManager.SaveScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene());AssetDatabase.SaveAssets();
  File.WriteAllText(Out+"/lake-market-boundary-draft.json",new JObject{["status"]="MEASURED_PHYSICAL_DRAFT_REQUIRES_VISUAL_AND_NATIVE_ESCAPE_TEST",["outer_basis"]="Convex envelope of existing terrain vertices, inset 1.5m",["shore_basis"]="Actual Blender lake boundary, dry bank offset 0.8m",["player_jump_unchanged_m"]=story.player.jumpHeight,["bridge_jump_escape"]="OPEN_NATIVE_TEST_REQUIRED; existing rail and Bao contact unchanged",["panels"]=rail,["skipped"]=skipped,["new_quest_locks"]=false}.ToString());
 }
 public static void PlaceAndBuild(){PlaceDraft();ConRoiV48R21AssetPass.BuildReview();}
 public static void RejectLakeFenceDraft()
 {
  EditorSceneManager.OpenScene("Assets/Scenes/JourneyRepair_v48_r21.unity");var root=GameObject.Find("R21_LakeMarket_Measured_Perimeter");
  if(root)root.SetActive(false);
  EditorSceneManager.SaveScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene());AssetDatabase.SaveAssets();
  File.WriteAllText(Out+"/boundary-visual-rejection.json",new JObject{["status"]="REJECTED_DISABLED",["reason"]="3.8m shore fence dominates lake views and crosses visual bridge context; envelope derivation does not establish suitable scenery or jump-safe boundary",["preserved_trial_root"]=true,["final_boundaries"]="OPEN_REQUIRES_NEW_SCENERY_DESIGN_AND_NATIVE_TEST"}.ToString());
  ConRoiV48R21AssetPass.BuildReview();
 }
 public static void LowerShoreRailDraft()
 {
  EditorSceneManager.OpenScene("Assets/Scenes/JourneyRepair_v48_r21.unity");var root=UnityEngine.Object.FindObjectsByType<Transform>(FindObjectsInactive.Include).Single(t=>t.name=="R21_LakeMarket_Measured_Perimeter");
  var record=JObject.Parse(File.ReadAllText(Out+"/lake-market-boundary-draft.json"));var panels=record["panels"].ToArray();
  var nw=new Vector2(685.9787f,496.6172f);var se=new Vector2(642.2553f,533.6046f);var axis=(nw-se).normalized;float length=Vector2.Distance(nw,se);int disabled=0,lowered=0;
  for(int i=0;i<panels.Length;i++)
  {
   var row=panels[i];string kind=(string)row["kind"];var g=root.Find(kind+"_"+(i+1));if(!g)throw new Exception("Missing measured panel "+i);
   if(kind!="Lake_Bank_Maintenance_Fence")continue;
   var a=new Vector3((float)row["a"][0],(float)row["a"][1],(float)row["a"][2]);var b=new Vector3((float)row["b"][0],(float)row["b"][1],(float)row["b"][2]);var mid=(a+b)/2;var delta=new Vector2(mid.x,mid.z)-se;float along=Vector2.Dot(delta,axis),side=Mathf.Abs(delta.x*axis.y-delta.y*axis.x);
   if(along>-18&&along<length+18&&side<20){g.gameObject.SetActive(false);disabled++;continue;}
   var filter=g.GetComponent<MeshFilter>();var mesh=UnityEngine.Object.Instantiate(filter.sharedMesh);var vertices=mesh.vertices;var horizontal=new Vector2(b.x-a.x,b.z-a.z);
   for(int j=0;j<vertices.Length;j++)
   {
    var w=g.TransformPoint(vertices[j]);float t=Mathf.Clamp01(Vector2.Dot(new Vector2(w.x-a.x,w.z-a.z),horizontal)/horizontal.sqrMagnitude);float baseY=Mathf.Lerp(a.y,b.y,t);
    w.y=baseY+(w.y-baseY)*(1.15f/3.8f);vertices[j]=g.InverseTransformPoint(w);
   }
   mesh.name="LowShoreRail_"+i;mesh.vertices=vertices;mesh.RecalculateNormals();mesh.RecalculateBounds();AssetDatabase.CreateAsset(mesh,Art+"/"+mesh.name+".asset");filter.sharedMesh=mesh;g.GetComponent<MeshCollider>().sharedMesh=mesh;lowered++;
  }
  root.gameObject.SetActive(true);EditorSceneManager.SaveScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene());AssetDatabase.SaveAssets();
  File.WriteAllText(Out+"/low-shore-rail-draft.json",new JObject{["status"]="VISUAL_BOUNDARY_DRAFT_NOT_JUMP_PROOF",["height_m"]=1.15,["lowered_panels"]=lowered,["bridge_context_panels_disabled"]=disabled,["outer_service_fences_retained"]=true,["known_limit"]="Low shore rails and existing bridge rails may be cleared by unchanged 3m jump; must not certify containment without native test or compatible visible scenery"}.ToString());
  ConRoiV48R21AssetPass.BuildReview();
 }
 public static void CompleteOuterSupports()
 {
  EditorSceneManager.OpenScene("Assets/Scenes/JourneyRepair_v48_r21.unity");
  var root=GameObject.Find("R21_LakeMarket_Measured_Perimeter");
  if(root.transform.Cast<Transform>().Any(t=>t.name.StartsWith("Outer_Infill_")))throw new Exception("Preserve previous outer infill");
  var story=UnityEngine.Object.FindAnyObjectByType<DieuPhoiHanhTrinh>();bool la=story.lake.activeSelf,ma=story.market.activeSelf;story.lake.SetActive(true);story.market.SetActive(true);Physics.SyncTransforms();
  var survey=JObject.Parse(File.ReadAllText(Out+"/envelope-survey.json"));
  var points=survey["surfaces"].Where(s=>(string)s["name"]!="Surface_153").SelectMany(s=>s["vertices"]).Select(v=>new Vector2((float)v[0],(float)v[2])).Distinct().OrderBy(p=>p.x).ThenBy(p=>p.y).ToArray();
  var hull=new List<Vector2>();foreach(var p in points){while(hull.Count>=2&&Cross(hull[^2],hull[^1],p)<=0)hull.RemoveAt(hull.Count-1);hull.Add(p);}int lower=hull.Count;
  foreach(var p in points.Reverse()){while(hull.Count>lower&&Cross(hull[^2],hull[^1],p)<=0)hull.RemoveAt(hull.Count-1);hull.Add(p);}hull.RemoveAt(hull.Count-1);var centre=hull.Aggregate(Vector2.zero,(a,b)=>a+b)/hull.Count;
  var existing=JObject.Parse(File.ReadAllText(Out+"/lake-market-boundary-draft.json"))["panels"].Where(p=>(string)p["kind"]=="Outer_Park_Service_Fence").Select(p=>new Vector2(((float)p["a"][0]+(float)p["b"][0])/2,((float)p["a"][2]+(float)p["b"][2])/2)).ToArray();
  var temp=GameObject.CreatePrimitive(PrimitiveType.Cube);var cube=temp.GetComponent<MeshFilter>().sharedMesh;UnityEngine.Object.DestroyImmediate(temp);
  var material=AssetDatabase.LoadAssetAtPath<Material>("Assets/Art/Environment/OngDaoV35/OD35_GreenMetal.mat");var added=new JArray();var unsupported=new JArray();int serial=0;
  bool Ground(Vector2 p,out float y)
  {
   var hits=Physics.RaycastAll(new Vector3(p.x,60,p.y),Vector3.down,120,8,QueryTriggerInteraction.Ignore).Where(h=>h.normal.y>.7f&&!h.collider.transform.IsChildOf(root.transform)).OrderByDescending(h=>h.point.y).ToArray();y=hits.Length>0?hits[0].point.y:0;return hits.Length>0;
  }
  for(int k=0;k<hull.Count;k++)
  {
   var from=Vector2.MoveTowards(hull[k],centre,1.5f);var to=Vector2.MoveTowards(hull[(k+1)%hull.Count],centre,1.5f);int count=Mathf.CeilToInt(Vector2.Distance(from,to)/2);
   for(int i=0;i<count;i++)
   {
    var a=Vector2.Lerp(from,to,i/(float)count);var b=Vector2.Lerp(from,to,(i+1)/(float)count);var mid=(a+b)/2;if(existing.Any(p=>Vector2.Distance(p,mid)<.02f))continue;
    if(!Ground(a,out float ya)||!Ground(b,out float yb)||!Ground(mid,out float ym)){unsupported.Add(new JArray(mid.x,mid.y));continue;}
    // Negative authored terrain is still real terrain at the outer envelope.
    // Do not confuse its world elevation with missing support. The shore rail
    // remains separate, and nothing is added at the bridge or story route.
    float top=Mathf.Max(ya,yb,ym)+3.8f;var origin=new Vector3(mid.x,Mathf.Min(ya,yb)-.06f,mid.y);var pieces=new List<CombineInstance>();
    void Box(Vector3 p,Vector3 size,Quaternion rotation){pieces.Add(new CombineInstance{mesh=cube,transform=Matrix4x4.TRS(p-origin,rotation,size)});}
    void Upright(Vector2 p,float y,float width){float bottom=y-.06f;Box(new Vector3(p.x,(bottom+top)/2,p.y),new Vector3(width,top-bottom,width),Quaternion.identity);}
    Upright(a,ya,.075f);Upright(b,yb,.075f);
    int bars=Mathf.CeilToInt(Vector2.Distance(a,b)/.16f);for(int j=1;j<bars;j++){float t=j/(float)bars;Upright(Vector2.Lerp(a,b,t),Mathf.Lerp(ya,yb,t),.025f);}
    foreach(float h in new[]{top-.08f,top-1.1f,Mathf.Max(ya,yb,ym)+.16f})
    {var p=new Vector3(a.x,h,a.y);var q=new Vector3(b.x,h,b.y);Box((p+q)/2,new Vector3(.035f,Vector3.Distance(p,q),.035f),Quaternion.FromToRotation(Vector3.up,q-p));}
    var mesh=new Mesh{name="OuterInfill_"+serial};mesh.CombineMeshes(pieces.ToArray(),true,true);mesh.RecalculateBounds();AssetDatabase.CreateAsset(mesh,Art+"/"+mesh.name+".asset");
    var g=new GameObject("Outer_Infill_"+serial++);g.layer=3;g.transform.SetParent(root.transform);g.transform.position=origin;g.AddComponent<MeshFilter>().sharedMesh=mesh;g.AddComponent<MeshRenderer>().sharedMaterial=material;g.AddComponent<MeshCollider>().sharedMesh=mesh;
    GameObjectUtility.SetStaticEditorFlags(g,StaticEditorFlags.BatchingStatic);
    added.Add(new JObject{["a"]=new JArray(a.x,ya,a.y),["b"]=new JArray(b.x,yb,b.y),["top_y"]=top,["same_visible_collision_mesh"]=true});
   }
  }
  var scenery=UnityEngine.Object.FindAnyObjectByType<CanhQuanChoV48>();var sample=GameObject.Find("R21_RealPlanterFlowerSample");
  foreach(var g in new[]{root,sample})if(g&&!scenery.scenery.Contains(g))scenery.scenery=scenery.scenery.Concat(new[]{g}).ToArray();
  story.lake.SetActive(la);story.market.SetActive(ma);EditorSceneManager.SaveScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene());AssetDatabase.SaveAssets();
  File.WriteAllText(Out+"/outer-boundary-infill.json",new JObject{["status"]="GROUNDED_OUTER_DRAFT_REQUIRES_VISUAL_AND_NATIVE_TEST",["reason"]="Original placement incorrectly rejected supported outer terrain with negative world Y",["height_above_highest_support_m"]=3.8,["new_panels"]=added,["unsupported_midpoints"]=unsupported,["limits"]="Convex terrain envelope is not proof of scenic fit. Low shore/bridge rails remain jumpable; no controller or jump changes.",["new_story_locks"]=false}.ToString());
  ConRoiV48R21AssetPass.AuditBoundaries();ConRoiV48R21AssetPass.BuildReview();
 }
}
