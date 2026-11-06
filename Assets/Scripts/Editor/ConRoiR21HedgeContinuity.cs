using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

// Scene-only landscape authoring and supplemental physics audits. Never changes Player.
public static class ConRoiR21HedgeContinuity
{
 const string Scene="Assets/Scenes/JourneyRepair_v48_r21_recoveryprep.unity";
 const string Out="Docs/Reviews/journey_v48/r21/recovery-prep";
 const string Art="Assets/Art/Environment/JourneyV48/RecoveryPrepR21";
 static readonly Vector3 SE=new Vector3(642.2553f,1.14006f,533.6046f),NW=new Vector3(685.9787f,1.14006f,496.6172f);
 static readonly Vector3 Axis=(NW-SE).normalized,Water=new Vector3(-.6456957f,0,-.763594866f);
 static JArray V(Vector3 p)=>new JArray(p.x,p.y,p.z);
 static string P(Transform t)=>t.parent?P(t.parent)+"/"+t.name:t.name;
 static DieuPhoiHanhTrinh Open()
 {
  Directory.CreateDirectory(Out);EditorSceneManager.OpenScene(Scene);
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
   // Fixed XZ sections: never snap independent panels to different ground points.
   var heights=new List<float>();
   for(int k=0;k<24;k++)foreach(float radius in new[]{0f,1f,2f,3.5f,6f}){
    var q=p+Quaternion.Euler(0,k*15,0)*Vector3.forward*radius;
    if(ConRoiV48R21TerrainGround.Sample(terrain,new Vector2(q.x,q.z),out float h))heights.Add(h);
   }
   if(heights.Count==0)throw new Exception("No surrounding terrain for fixed hedge section "+p);
   p.y=ConRoiV48R21TerrainGround.Sample(terrain,new Vector2(p.x,p.z),out float direct)?direct:heights.Min();
   float top=heights.Max()+4.65f+.32f*Mathf.Sin(p.x*1.3f+p.z*.7f)+.18f*Mathf.Sin(p.x*.3f-p.z*.4f);
   var own=GameObject.Find("R21_Boundary_Landscape_Finish").transform;
   for(int k=0;k<24;k++)foreach(float radius in new[]{0f,1.5f,3f,4.5f,6f}){
    var q=p+Quaternion.Euler(0,k*15,0)*Vector3.forward*radius;
    foreach(var h in Physics.RaycastAll(new Vector3(q.x,60,q.z),Vector3.down,120,8,QueryTriggerInteraction.Ignore))if(h.normal.y>.7f&&!h.collider.transform.IsChildOf(own)&&!h.collider.GetComponentInParent<DieuKhienDiChuyen>())top=Mathf.Max(top,h.point.y+4.65f);
   }
   float w=width*(1+.12f*Mathf.Sin(p.x*.6f+p.z*.47f));
   float baseY=heights.Min()-2; // continuous visible foliage side into the lowest surrounding slope
   float height=top-p.y;
   var ring=new[]{new Vector2(-.85f,-.2f),new Vector2(-.85f,height-.35f),new Vector2(-.6f,height-.12f),new Vector2(0,height),new Vector2(.6f,height-.12f),new Vector2(.85f,height-.35f),new Vector2(.85f,-.2f)}.Select(q=>p+normal*(q.x*w)+Vector3.up*q.y).ToArray();
   ring[0].y=baseY;ring[^1].y=baseY;rings.Add(ring);
  }
  for(int i=0;i<sections;i++)for(int j=0;j<7;j++){int k=(j+1)%7;var outwards=j==6?Vector3.down:(j<3?-normal:normal)+Vector3.up*(j>=1&&j<=4?1:.05f);writer.Quad(rings[i][j],rings[i+1][j],rings[i+1][k],rings[i][k],outwards);}
  foreach(int i in new[]{0,sections}){var ring=rings[i];var centre=ring.Aggregate(Vector3.zero,(sum,p)=>sum+p)/ring.Length;for(int j=0;j<7;j++)writer.Triangle(centre,ring[j],ring[(j+1)%7],i==0?-direction:direction);}
  return writer.Finish(name);
 }
 public static void Author()
 {
  var story=Open();var root=GameObject.Find("R21_Boundary_Landscape_Finish");var old=root.transform.Find("Grounded_Cypress_Boundary");UnityEngine.Object.DestroyImmediate(old.gameObject);
  Directory.CreateDirectory(Art);terrain=ConRoiV48R21TerrainGround.CreateProbes();Physics.SyncTransforms();
  // A continuous planted bank covers the accessible steep notch beside the
  // omitted west connector. It joins existing terrain, rather than relying
  // on an arbitrarily deep wall or recovery after walking beneath a hedge.
  GradeBank(root);
  var hedge=AssetDatabase.LoadAssetAtPath<Material>("Assets/Art/Environment/JourneyV48/RecoveryFixR21/Cypress_Hedge.mat");
  var holder=new GameObject("Grounded_Cypress_Boundary");holder.transform.SetParent(root.transform,false);
  var data=JObject.Parse(File.ReadAllText("Docs/Reviews/journey_v48/r21/asset-pass/actual-terrain-boundary.json"));int count=0;
  AssetDatabase.StartAssetEditing();try{foreach(var row in data["panels"]){var a=new Vector3((float)row["a"][0],0,(float)row["a"][2]);var b=new Vector3((float)row["b"][0],0,(float)row["b"][2]);var inset=-Vector3.Cross(Vector3.up,(b-a).normalized)*.8f;a+=inset;b+=inset;var along=(b-a).normalized*.12f;a-=along;b+=along;MeshObject(holder.transform,Hedge(a,b,"CypressBoundary_"+count++),hedge);}}
  finally{AssetDatabase.StopAssetEditing();}
  var joins=new JArray();var panels=data["panels"].ToArray();var edges=new List<Vector2>();
  for(int i=0;i<panels.Length;i++){
   var a=new Vector3((float)panels[i]["b"][0],0,(float)panels[i]["b"][2]);var next=panels[(i+1)%panels.Length];var b=new Vector3((float)next["a"][0],0,(float)next["a"][2]);
   edges.Add(new Vector2((float)panels[i]["a"][0],(float)panels[i]["a"][2]));edges.Add(new Vector2(a.x,a.z));
   if(Vector3.Distance(a,b)>.1f){var axis=(b-a).normalized;var inset=-Vector3.Cross(Vector3.up,axis)*.8f;MeshObject(holder.transform,Hedge(a+inset-axis*.24f,b+inset+axis*.24f,"CypressConnector_after_"+i,1.25f),hedge);joins.Add(new JObject{["after_panel"]=i,["a"]=V(a),["b"]=V(b),["length_m"]=Vector3.Distance(a,b)});edges.Add(new Vector2(a.x,a.z));edges.Add(new Vector2(b.x,b.z));}
  }
  story.GetComponent<PhucHoiTaiBienR21>().perimeterEdges=edges.ToArray();
  ConRoiV48R21TerrainGround.Dispose(terrain);story.lake.SetActive(false);story.market.SetActive(false);EditorSceneManager.SaveScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene());AssetDatabase.SaveAssets();
  File.WriteAllText(Out+"/hedge-authoring.json",new JObject{["panels"]=count,["connectors"]=joins,["design"]="Fixed continuous author XZ; no per-section snapping; upright visible foliage shares collider, buried base below minimum terrain within 6m. Join all four omitted perimeter intervals, including the reachable 5m gap after panel 676. Recovery polygon closed along the same visible outline. Bank routes and controller unchanged."}.ToString());
 }
 static void GradeBank(GameObject root)
 {
  var priorApron=root.transform.Find("PlantedWestBoundaryBank");if(priorApron)UnityEngine.Object.DestroyImmediate(priorApron.gameObject);
  var bank=new MeshWriter();const int nx=36,nz=56;var grid=new Vector3[nx+1,nz+1];
  for(int x=0;x<=nx;x++)for(int z=0;z<=nz;z++){
   var p=new Vector3(462+x*.5f,0,493+z*.5f);float dist=new Vector2(p.x-476.424438f,p.z-509.875519f).magnitude;
   float y=2.9517684f-.32f*dist; // matches supported interior land at the notch
   if(ConRoiV48R21TerrainGround.Sample(terrain,new Vector2(p.x,p.z),out float ground))y=Mathf.Max(y,ground);
   p.y=y+.015f;grid[x,z]=p;
  }
  // Raise neighboring samples to a common 0.32 radial grade envelope. The
  // max-with-source operation alone preserves steep source spikes; this
  // envelope removes those new collision lips without lowering source land.
  var originalGrid=(Vector3[,])grid.Clone();
  for(int x=0;x<=nx;x++)for(int z=0;z<=nz;z++){
   var p=grid[x,z];float y=p.y;
   for(int ax=0;ax<=nx;ax++)for(int az=0;az<=nz;az++){var a=originalGrid[ax,az];float dist=new Vector2(p.x-a.x,p.z-a.z).magnitude;y=Mathf.Max(y,a.y-.32f*dist);}
   p.y=y;grid[x,z]=p;
  }
  float bottom=grid.Cast<Vector3>().Min(p=>p.y)-3;
  for(int x=0;x<nx;x++)for(int z=0;z<nz;z++)bank.Quad(grid[x,z],grid[x+1,z],grid[x+1,z+1],grid[x,z+1],Vector3.up);
  for(int x=0;x<nx;x++)foreach(int z in new[]{0,nz}){var a=grid[x,z];var b=grid[x+1,z];var c=b;c.y=bottom;var d=a;d.y=bottom;bank.Quad(a,b,c,d,z==0?Vector3.back:Vector3.forward);}
  for(int z=0;z<nz;z++)foreach(int x in new[]{0,nx}){var a=grid[x,z];var b=grid[x,z+1];var c=b;c.y=bottom;var d=a;d.y=bottom;bank.Quad(a,b,c,d,x==0?Vector3.left:Vector3.right);}
  bank.Quad(new Vector3(462,bottom,493),new Vector3(480,bottom,493),new Vector3(480,bottom,521),new Vector3(462,bottom,521),Vector3.down);
  MeshObject(root.transform,bank.Finish("PlantedWestBoundaryBank"),AssetDatabase.LoadAssetAtPath<Material>("Assets/Art/Environment/JourneyV48/RecoveryFixR21/Bank_Planted_Shoulders.mat"));Physics.SyncTransforms();
 }
 public static void RegradeOnly()
 {
  var story=Open();var root=GameObject.Find("R21_Boundary_Landscape_Finish");terrain=ConRoiV48R21TerrainGround.CreateProbes();GradeBank(root);ConRoiV48R21TerrainGround.Dispose(terrain);
  story.lake.SetActive(false);story.market.SetActive(false);EditorSceneManager.SaveScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene());AssetDatabase.SaveAssets();ConRoiR21Preflight.FinalAuditAndImages();
 }
}
