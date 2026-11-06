using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

// Scene-only landscape authoring and supplemental physics audits. Never changes Player.
public static class ConRoiV48R21RecoveryGeometry
{
 const string Scene="Assets/Scenes/JourneyRepair_v48_r21_recoveryfix.unity";
 const string Out="Docs/Reviews/journey_v48/r21/recovery-fix";
 const string Art="Assets/Art/Environment/JourneyV48/RecoveryFixR21";
 static readonly Vector3 SE=new Vector3(642.2553f,1.14006f,533.6046f),NW=new Vector3(685.9787f,1.14006f,496.6172f);
 static readonly Vector3 Axis=(NW-SE).normalized,Water=new Vector3(-.6456957f,0,-.763594866f);
 static JArray V(Vector3 p)=>new JArray(p.x,p.y,p.z);
 static string P(Transform t)=>t.parent?P(t.parent)+"/"+t.name:t.name;
 static DieuPhoiHanhTrinh Open(string scenePath=Scene)
 {
  Directory.CreateDirectory(Out);EditorSceneManager.OpenScene(scenePath);
  var story=UnityEngine.Object.FindAnyObjectByType<DieuPhoiHanhTrinh>();story.lake.SetActive(true);story.market.SetActive(true);Physics.SyncTransforms();return story;
 }
 static MeshCollider[] terrain;
 static float Ground(Vector3 p)
 {
  if(!ConRoiV48R21TerrainGround.Sample(terrain,new Vector2(p.x,p.z),out float y))throw new Exception("No terrain support: "+p);return y;
 }
 static float NearHigh(Vector3 p)
 {
  float y=Ground(p);for(int j=0;j<8;j++){var q=p+Quaternion.Euler(0,j*45,0)*Vector3.forward*3.5f;if(ConRoiV48R21TerrainGround.Sample(terrain,new Vector2(q.x,q.z),out float h))y=Mathf.Max(y,h);}
  // Include actual walkable object tops, not only the terrain beneath them.
  // A roof, planter or plinth next to the hedge must not become a second takeoff.
  var own=GameObject.Find("R21_Boundary_Landscape_Finish").transform;
  for(int j=0;j<24;j++)foreach(float radius in new[]{0f,1.5f,3f,4.5f,6f}){var q=p+Quaternion.Euler(0,j*15,0)*Vector3.forward*radius;
   foreach(var h in Physics.RaycastAll(new Vector3(q.x,50,q.z),Vector3.down,100,8,QueryTriggerInteraction.Ignore))if(h.normal.y>.7f&&!h.collider.GetComponentInParent<DieuKhienDiChuyen>()&&!h.collider.transform.IsChildOf(own))y=Mathf.Max(y,h.point.y);
  }
  return y;
 }
 sealed class MeshWriter
 {
  readonly List<Vector3> v=new List<Vector3>();readonly List<int> t=new List<int>(),soil=new List<int>();readonly List<Vector2> uv=new List<Vector2>();
  public void Quad(Vector3 a,Vector3 b,Vector3 c,Vector3 d,Vector3 outward,bool planted=false)
  {
   if(Vector3.Dot(Vector3.Cross(b-a,c-a),outward)<0){var q=b;b=d;d=q;}
   int n=v.Count;v.AddRange(new[]{a,b,c,d});uv.AddRange(new[]{Vector2.zero,Vector2.up,Vector2.one,Vector2.right});var target=planted?soil:t;if(Vector3.Cross(b-a,c-a).sqrMagnitude>=1e-12f)target.AddRange(new[]{n,n+1,n+2});if(Vector3.Cross(c-a,d-a).sqrMagnitude>=1e-12f)target.AddRange(new[]{n,n+2,n+3});
  }
  public void Triangle(Vector3 a,Vector3 b,Vector3 c,Vector3 outward)
  {
   if(Vector3.Cross(b-a,c-a).sqrMagnitude<1e-12f)return;
   if(Vector3.Dot(Vector3.Cross(b-a,c-a),outward)<0){var q=b;b=c;c=q;}int n=v.Count;v.AddRange(new[]{a,b,c});uv.AddRange(new[]{Vector2.zero,Vector2.up,Vector2.one});t.AddRange(new[]{n,n+1,n+2});
  }
  public Mesh Finish(string name)
  {
   var mesh=new Mesh{name=name};if(v.Count>65000)mesh.indexFormat=UnityEngine.Rendering.IndexFormat.UInt32;
   mesh.SetVertices(v);mesh.SetUVs(0,uv);mesh.subMeshCount=soil.Count>0?2:1;mesh.SetTriangles(t,0);if(soil.Count>0)mesh.SetTriangles(soil,1);mesh.RecalculateNormals();mesh.RecalculateBounds();mesh.SetPreBakeCollisionMesh(false,true);return mesh;
  }
 }
 static Material Material(string name,Color color)
 {
  var path=Art+"/"+name+".mat";var m=AssetDatabase.LoadAssetAtPath<Material>(path);if(!m){m=new Material(Shader.Find("Universal Render Pipeline/Lit")){name=name,enableInstancing=true};AssetDatabase.CreateAsset(m,path);}m.SetColor("_BaseColor",color);m.SetFloat("_Smoothness",.05f);EditorUtility.SetDirty(m);return m;
 }
 static GameObject MeshObject(Transform root,Mesh mesh,Material material)
 {
  var path=Art+"/"+mesh.name+".asset";var existing=AssetDatabase.LoadAssetAtPath<Mesh>(path);if(existing){EditorUtility.CopySerialized(mesh,existing);EditorUtility.SetDirty(existing);AssetDatabase.SaveAssetIfDirty(existing);UnityEngine.Object.DestroyImmediate(mesh);mesh=existing;}else AssetDatabase.CreateAsset(mesh,path);var g=new GameObject(mesh.name);g.layer=3;g.transform.SetParent(root,false);g.AddComponent<MeshFilter>().sharedMesh=mesh;g.AddComponent<MeshRenderer>().sharedMaterial=material;g.AddComponent<MeshCollider>().sharedMesh=mesh;GameObjectUtility.SetStaticEditorFlags(g,StaticEditorFlags.BatchingStatic);return g;
 }
 static Mesh Hedge(Vector3 a,Vector3 b,string name,float width=1.05f)
 {
  var writer=new MeshWriter();var direction=(b-a).normalized;direction.y=0;direction.Normalize();var normal=Vector3.Cross(Vector3.up,direction);var rings=new List<Vector3[]>();int sections=Mathf.Max(2,Mathf.CeilToInt(Vector3.Distance(a,b)/.7f));
  for(int i=0;i<=sections;i++)
  {
   var p=Vector3.Lerp(a,b,i/(float)sections);
   if(!ConRoiV48R21TerrainGround.Sample(terrain,new Vector2(p.x,p.z),out _)){
    var original=p;bool supported=false;
    for(float radius=.1f;radius<=2.5f&&!supported;radius+=.1f)for(int k=0;k<24;k++){
     var candidate=original+Quaternion.Euler(0,k*15,0)*Vector3.forward*radius;
     if(ConRoiV48R21TerrainGround.Sample(terrain,new Vector2(candidate.x,candidate.z),out _)){p=candidate;supported=true;break;}
    }
    if(!supported)throw new Exception("Unsupported hedge centre "+original);
   }
   p.y=Ground(p);float top=NearHigh(p)+4.65f+.32f*Mathf.Sin(p.x*1.3f+p.z*.7f)+.18f*Mathf.Sin(p.x*.3f-p.z*.4f);float w=width*(1+.12f*Mathf.Sin(p.x*.6f+p.z*.47f));

   float height=top-p.y;
   var ring=new[]{new Vector2(-.85f,-.2f),new Vector2(-.85f,height-.35f),new Vector2(-.6f,height-.12f),new Vector2(0,height),new Vector2(.6f,height-.12f),new Vector2(.85f,height-.35f),new Vector2(.85f,-.2f)}.Select(q=>p+normal*(q.x*w)+Vector3.up*q.y).ToArray();
   ring[0].y=ConRoiV48R21TerrainGround.Sample(terrain,new Vector2(ring[0].x,ring[0].z),out float ly)?ly-.2f:p.y-.5f;ring[^1].y=ConRoiV48R21TerrainGround.Sample(terrain,new Vector2(ring[^1].x,ring[^1].z),out float ry)?ry-.2f:p.y-.5f;rings.Add(ring);
  }
  for(int i=0;i<sections;i++)for(int j=0;j<7;j++){int k=(j+1)%7;var outwards=j==6?Vector3.down:(j<3?-normal:normal)+Vector3.up*(j>=1&&j<=4?1:.05f);writer.Quad(rings[i][j],rings[i+1][j],rings[i+1][k],rings[i][k],outwards);}
  foreach(int i in new[]{0,sections}){var ring=rings[i];var centre=ring.Aggregate(Vector3.zero,(sum,p)=>sum+p)/ring.Length;for(int j=0;j<7;j++)writer.Triangle(centre,ring[j],ring[(j+1)%7],i==0?-direction:direction);}
  return writer.Finish(name);
 }
 static List<Vector3> TrailPoints(float side)
 {
  float length=Vector3.Distance(SE,NW);var local=new[]{new Vector2(-19,0),new Vector2(-19,side*13.5f),new Vector2(length+17,side*13.5f),new Vector2(length+17,0)};var points=new List<Vector3>();
  var smooth=new List<Vector2>{local[0]};
  for(int j=1;j<local.Length-1;j++){
   var before=local[j]+(local[j-1]-local[j]).normalized*2;var after=local[j]+(local[j+1]-local[j]).normalized*2;smooth.Add(before);
   for(int k=1;k<=16;k++){float t=k/16f;smooth.Add((1-t)*(1-t)*before+2*(1-t)*t*local[j]+t*t*after);}
  }smooth.Add(local[^1]);
  for(int j=0;j<smooth.Count-1;j++){int count=Mathf.CeilToInt(Vector2.Distance(smooth[j],smooth[j+1])/.4f);for(int i=0;i<count;i++){var q=Vector2.Lerp(smooth[j],smooth[j+1],i/(float)count);points.Add(SE+Axis*q.x+Water*q.y);}}
  points.Add(SE+Axis*local[^1].x);return points;
 }
 static Mesh Trail(List<Vector3> points,string name,out JArray sections)
 {
  var writer=new MeshWriter();var rings=new List<Vector3[]>();sections=new JArray();
  var floors=UnityEngine.Object.FindObjectsByType<Collider>().Where(c=>c.enabled&&!c.isTrigger&&c.gameObject.layer==3&&c.bounds.size.y<.8f&&!c.name.Contains("Fence")&&!c.name.Contains("Rail")).ToArray();
  var heights=new float[points.Count];
  for(int i=0;i<points.Count;i++)
  {
   var tangent=points[Mathf.Min(i+1,points.Count-1)]-points[Mathf.Max(0,i-1)];tangent.y=0;var normal=Vector3.Cross(Vector3.up,tangent.normalized);float y=float.NegativeInfinity;
   foreach(float x in new[]{-.9f,-.45f,0,.45f,.9f})y=Mathf.Max(y,Ground(points[i]+normal*x));
   if(i<4||i>points.Count-5)foreach(var c in floors)if(c.Raycast(new Ray(points[i]+Vector3.up*20,Vector3.down),out var hit,40)&&hit.normal.y>.7f&&hit.point.y<y+.45f)y=Mathf.Max(y,hit.point.y);
   heights[i]=y+.035f;
  }
  // Grade the visible gravel fill rather than changing the controller's slope limit.
  for(int i=1;i<points.Count;i++)heights[i]=Mathf.Max(heights[i],heights[i-1]-.32f*Vector3.Distance(points[i],points[i-1]));
  for(int i=points.Count-2;i>=0;i--)heights[i]=Mathf.Max(heights[i],heights[i+1]-.32f*Vector3.Distance(points[i],points[i+1]));
  const int columns=97; // 24m full transition, 25cm transverse samples
  var grid=new Vector3[points.Count,columns];var widths=Enumerable.Repeat(12f,points.Count).ToArray();widths[0]=widths[^1]=1.2f;
  for(int i=1;i<points.Count-1;i++){var a=points[i]-points[i-1];var b=points[i+1]-points[i];a.y=b.y=0;float angle=Vector3.Angle(a,b)*Mathf.Deg2Rad;if(angle>.001f)widths[i]=Mathf.Clamp(.5f*Mathf.Min(a.magnitude,b.magnitude)/angle,1.2f,12f);}
  for(int i=1;i<points.Count;i++)widths[i]=Mathf.Min(widths[i],widths[i-1]+.5f*Vector3.Distance(points[i],points[i-1]));
  for(int i=points.Count-2;i>=0;i--)widths[i]=Mathf.Min(widths[i],widths[i+1]+.5f*Vector3.Distance(points[i],points[i+1]));
  for(int i=0;i<points.Count;i++){
   var p=points[i];var tangent=points[Mathf.Min(i+1,points.Count-1)]-points[Mathf.Max(0,i-1)];tangent.y=0;var normal=Vector3.Cross(Vector3.up,tangent.normalized);
   float Width(float sign){float result=.78f;for(float x=.8f;x<=widths[i];x+=.2f){var q=p+normal*x*sign;if(!ConRoiV48R21TerrainGround.Sample(terrain,new Vector2(q.x,q.z),out _))break;result=x;}return result;}
   float left=Width(-1),right=Width(1);
   for(int j=0;j<columns;j++){float x=j<45?Mathf.Lerp(-left,-.78f,j/45f):j<=51?Mathf.Lerp(-.78f,.78f,(j-45)/6f):Mathf.Lerp(.78f,right,(j-51)/45f);var q=p+normal*x;float gy=Ground(q);q.y=Mathf.Max(gy+.025f,heights[i]-.32f*Mathf.Max(0,Mathf.Abs(x)-.78f));grid[i,j]=q;}
  }
  // Upper envelope under a grade constraint, in both axes. This fills steep
  // terrain protrusions too, rather than leaving them above a cosmetic ramp.
  for(int pass=0;pass<6;pass++){
   void Raise(int i,int j,int k,int l){var a=grid[i,j];var b=grid[k,l];float distance=new Vector2(a.x-b.x,a.z-b.z).magnitude;a.y=Mathf.Max(a.y,b.y-.32f*distance);grid[i,j]=a;}
   for(int i=0;i<points.Count;i++){for(int j=1;j<columns;j++)Raise(i,j,i,j-1);for(int j=columns-2;j>=0;j--)Raise(i,j,i,j+1);}
   for(int j=0;j<columns;j++){for(int i=1;i<points.Count;i++)Raise(i,j,i-1,j);for(int i=points.Count-2;i>=0;i--)Raise(i,j,i+1,j);}
  }
  for(int i=0;i<points.Count;i++){
   var topRing=Enumerable.Range(0,columns).Select(j=>grid[i,j]).ToList();var toeL=topRing[0];var toeR=topRing[^1];float underside=Mathf.Min(topRing.Min(q=>q.y)-.3f,Mathf.Min(Ground(toeL),Ground(toeR))-.12f);var bottomL=toeL;bottomL.y=underside;var bottomR=toeR;bottomR.y=underside;var ring=new[]{bottomL}.Concat(topRing).Concat(new[]{bottomR}).ToArray();rings.Add(ring);points[i]=grid[i,48];
   sections.Add(new JObject{["centre"]=V(points[i]),["terrain_y"]=Ground(points[i]),["toe_left"]=V(toeL),["toe_right"]=V(toeR),["toe_left_lip"]=toeL.y-Ground(toeL),["toe_right_lip"]=toeR.y-Ground(toeR)});
  }
  for(int i=0;i<rings.Count-1;i++)for(int j=0;j<rings[i].Length;j++){int k=(j+1)%rings[i].Length;writer.Quad(rings[i][j],rings[i+1][j],rings[i+1][k],rings[i][k],j==rings[i].Length-1?Vector3.down:j>0&&j<rings[i].Length-2?Vector3.up:(rings[i][j]+rings[i][k])/2-points[i]+Vector3.up*.1f,j<46||j>51);}
  foreach(int i in new[]{0,rings.Count-1})for(int j=1;j<rings[i].Length-1;j++)writer.Triangle(rings[i][0],rings[i][j],rings[i][j+1],i==0?points[0]-points[1]:points[^1]-points[^2]);
  return writer.Finish(name);
 }
 // Preserve a retired recovery surface as an authoring decision. A scenery root
 // being active must never resurrect its disabled child, render or collision.
 public static bool IsRetiredRecoverySurface(Transform surface)
 {
  if(!surface)return false;
  var renderer=surface.GetComponent<Renderer>();var collider=surface.GetComponent<Collider>();
  return !surface.gameObject.activeSelf||(renderer&&!renderer.enabled&&collider&&!collider.enabled);
 }
 public static void Author(string scenePath=Scene)
 {
  var story=Open(scenePath);var previous=GameObject.Find("R21_Boundary_Landscape_Finish");
  var retired=new HashSet<string>();
  if(previous)foreach(var name in new[]{"Lake_Bank_Return_Walk","Garden_Bank_Return_Walk"})
   if(IsRetiredRecoverySurface(previous.transform.Find(name)))retired.Add(name);
  if(previous)UnityEngine.Object.DestroyImmediate(previous);
  if(!File.Exists(Out+"/scene-before-boundary-finish.unity"))File.Copy(Scene,Out+"/scene-before-boundary-finish.unity",false);Directory.CreateDirectory(Art);terrain=ConRoiV48R21TerrainGround.CreateProbes();Physics.SyncTransforms();
  var root=new GameObject("R21_Boundary_Landscape_Finish");var hedge=Material("Cypress_Hedge",new Color(.135f,.205f,.145f));var gravel=Material("Bank_Gravel",new Color(.31f,.29f,.235f));var planted=Material("Bank_Planted_Shoulders",new Color(.13f,.22f,.13f));
  var old=UnityEngine.Object.FindObjectsByType<Transform>(FindObjectsInactive.Include).Single(t=>t.name=="R21_Actual_Terrain_Perimeter").gameObject;old.SetActive(false);var data=JObject.Parse(File.ReadAllText("Docs/Reviews/journey_v48/r21/asset-pass/actual-terrain-boundary.json"));var hedgeRoot=new GameObject("Grounded_Cypress_Boundary");hedgeRoot.transform.SetParent(root.transform,false);
  int count=0;AssetDatabase.StartAssetEditing();try{foreach(var row in data["panels"]){var a=new Vector3((float)row["a"][0],0,(float)row["a"][2]);var b=new Vector3((float)row["b"][0],0,(float)row["b"][2]);var inset=-Vector3.Cross(Vector3.up,(b-a).normalized)*.8f;a+=inset;b+=inset;var along=(b-a).normalized*.12f;a-=along;b+=along;MeshObject(hedgeRoot.transform,Hedge(a,b,"CypressBoundary_"+count++),hedge);}}finally{AssetDatabase.StopAssetEditing();}
  var gapRows=(JArray)data["unsupported_midpoints"];if(gapRows.Count>0){var centre=gapRows.Select(q=>new Vector3((float)q[0],0,(float)q[1])).Aggregate(Vector3.zero,(sum,p)=>sum+p)/gapRows.Count;MeshObject(hedgeRoot.transform,Hedge(centre-Vector3.right*.8f,centre+Vector3.right*.8f,"Cypress_Corner_Join",1.25f),hedge);}
  var paths=new JArray();foreach(float side in new[]{1f,-1f}){
   string name=side>0?"Lake_Bank_Return_Walk":"Garden_Bank_Return_Walk";
   // Do not generate geometry, overlays or toe grading for a retired surface.
   if(retired.Contains(name)){
    // Keep the retirement decision across subsequent authoring passes too.
    var marker=new GameObject(name);marker.transform.SetParent(root.transform,false);marker.SetActive(false);continue;
   }
   var points=TrailPoints(side);var mesh=Trail(points,name,out var sections);var path=MeshObject(root.transform,mesh,gravel);path.GetComponent<MeshRenderer>().sharedMaterials=new[]{gravel,planted};paths.Add(new JObject{["side"]=side,["sections"]=sections,["route"]=new JArray(points.Select(V))});
  }
  var baseHedge=AssetDatabase.LoadAssetAtPath<Material>("Assets/Art/Environment/JourneyV48/BoundaryFinishR21/Cypress_Hedge.mat");hedge.CopyPropertiesFromMaterial(baseHedge);EditorUtility.SetDirty(hedge);
  var scenery=story.GetComponent<CanhQuanChoV48>();scenery.scenery=scenery.scenery.Where(g=>g&&g!=old).Concat(new[]{root}).ToArray();
  ConRoiV48R21TerrainGround.Dispose(terrain);story.lake.SetActive(false);story.market.SetActive(false);EditorSceneManager.SaveScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene());AssetDatabase.SaveAssets();
  File.WriteAllText(Out+"/authoring.json",new JObject{["status"]="AUTHORED_REQUIRES_AUDIT_RENDER_NATIVE",["design"]="Retain open 1.02m bridge railing and allow bank exploration; two continuous grounded gravel return walks around both ends. Replace distant thin metal pickets with solid visible faceted evergreen hedge matching collision.",["old_boundary_preserved_inactive"]=true,["hedge_panels"]=count,["hedge_minimum_top_over_nearby_ground_m"]=4.13,["hedge_nearby_ground_radius_m"]=3.5,["trails"]=paths,["player_unchanged"]=true,["limits"]="Editor authoring, no game launched; native jumping, return walking, save/load and final rendering remain pending."}.ToString());
 }
 public static void OverlayFootways(string scenePath=Scene)
 {
  var story=Open(scenePath);var root=GameObject.Find("R21_Boundary_Landscape_Finish");
  foreach(var t in root.GetComponentsInChildren<Transform>().Where(t=>t.name.StartsWith("FootwayOverShoulder_")).ToArray())UnityEngine.Object.DestroyImmediate(t.gameObject);
  var paths=root.GetComponentsInChildren<MeshFilter>().Where(f=>f.name.EndsWith("Bank_Return_Walk")&&!IsRetiredRecoverySurface(f.transform)).ToArray();var originals=GameObject.Find("R20_Whole_Source_Export").GetComponentsInChildren<MeshFilter>().Where(f=>f.name=="Surface_125"||f.name=="Surface_126"||f.name=="Surface_153").ToArray();var rows=new JArray();
  Physics.SyncTransforms();
  foreach(var path in paths)foreach(var original in originals){
   var split=ConRoiV48R21PathConform.Split(original.sharedMesh,original.transform,path);var vertices=split.vertices;var faces=split.triangles;var writer=new MeshWriter();int count=0;var collider=path.GetComponent<MeshCollider>();
   bool Hit(Vector3 p,out RaycastHit hit)=>collider.Raycast(new Ray(new Vector3(p.x,40,p.z),Vector3.down),out hit,100)&&hit.normal.y>.3f;
   for(int i=0;i<faces.Length;i+=3){
    var triangle=new[]{original.transform.TransformPoint(vertices[faces[i]]),original.transform.TransformPoint(vertices[faces[i+1]]),original.transform.TransformPoint(vertices[faces[i+2]])};var centre=(triangle[0]+triangle[1]+triangle[2])/3;
    if(!Hit(centre,out _))continue;
    // R50: a centroid hit does not support the triangle's corners. Keeping a
    // corner outside the trail footprint joins the raised overlay to an
    // unprojected vertex, creating an exposed white connecting triangle.
    // Allow only a 2mm boundary tolerance; do not extend the support mesh.
    bool Supported(Vector3 p){
     if(Hit(p,out _))return true;
     foreach(var offset in new[]{Vector3.left,Vector3.right,Vector3.forward,Vector3.back})
      if(Hit(p+offset*.002f,out _))return true;
     return false;
    }
    if(triangle.Any(p=>!Supported(p)))continue;
    for(int j=0;j<3;j++)if(Hit(triangle[j],out var hit))triangle[j].y=Mathf.Max(triangle[j].y,hit.point.y+.025f);
    if(Vector3.Cross(triangle[1]-triangle[0],triangle[2]-triangle[0]).sqrMagnitude<1e-12f)continue;
    writer.Triangle(triangle[0],triangle[1],triangle[2],Vector3.up);count++;
   }
   UnityEngine.Object.DestroyImmediate(split);if(count==0)continue;
   var mesh=writer.Finish("FootwayOverShoulder_"+path.name+"_"+original.name);var overlay=MeshObject(root.transform,mesh,original.GetComponent<MeshRenderer>().sharedMaterial);rows.Add(new JObject{["source"]=P(original.transform),["original_mesh_unchanged"]=true,["new_overlay"]=overlay.name,["triangles"]=count});
  }
  story.lake.SetActive(false);story.market.SetActive(false);EditorSceneManager.SaveScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene());AssetDatabase.SaveAssets();File.WriteAllText(Out+"/footway-overlays.json",rows.ToString());
 }
}
