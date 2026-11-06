using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

// Scene-only landscape authoring and supplemental physics audits. Never changes Player.
public static class ConRoiV48R21BoundaryFinish
{
 const string Scene="Assets/Scenes/JourneyRepair_v48_r21.unity";
 const string Out="Docs/Reviews/journey_v48/r21/boundary-finish";
 const string Art="Assets/Art/Environment/JourneyV48/BoundaryFinishR21";
 static readonly Vector3 SE=new Vector3(642.2553f,1.14006f,533.6046f),NW=new Vector3(685.9787f,1.14006f,496.6172f);
 static readonly Vector3 Axis=(NW-SE).normalized,Water=new Vector3(-.6456957f,0,-.763594866f);
 static JArray V(Vector3 p)=>new JArray(p.x,p.y,p.z);
 static string P(Transform t)=>t.parent?P(t.parent)+"/"+t.name:t.name;
 static DieuPhoiHanhTrinh Open()
 {
  Directory.CreateDirectory(Out);EditorSceneManager.OpenScene(Scene);
  var story=UnityEngine.Object.FindAnyObjectByType<DieuPhoiHanhTrinh>();story.lake.SetActive(true);story.market.SetActive(true);Physics.SyncTransforms();return story;
 }
 public static void Survey()
 {
  var story=Open();var player=story.player;var probes=ConRoiV48R21TerrainGround.CreateProbes();Physics.SyncTransforms();var grid=new JArray();
  for(float s=-24;s<=Vector3.Distance(SE,NW)+24;s+=2)for(float n=-24;n<=24;n+=2)
  {
   var p=SE+Axis*s+Water*n;bool ground=ConRoiV48R21TerrainGround.Sample(probes,new Vector2(p.x,p.z),out float y);
   var hits=Physics.RaycastAll(new Vector3(p.x,40,p.z),Vector3.down,80,8,QueryTriggerInteraction.Ignore).Where(h=>h.normal.y>.5f).OrderByDescending(h=>h.point.y).Take(4).ToArray();
   grid.Add(new JObject{["s"]=s,["n"]=n,["xz"]=new JArray(p.x,p.z),["terrain_y"]=ground?(JToken)y:JValue.CreateNull(),["hits"]=new JArray(hits.Select(h=>new JObject{["y"]=h.point.y,["path"]=P(h.collider.transform),["normal"]=V(h.normal)}))});
  }
  var start=new Vector3(663.494873f,2.0700598f,506.913635f);var nearby=Physics.OverlapBox(start,new Vector3(4,5,4),Quaternion.identity,8,QueryTriggerInteraction.Ignore).Distinct().Select(c=>new JObject{["path"]=P(c.transform),["type"]=c.GetType().Name,["bounds"]=c.bounds.ToString("F4"),["mesh"]=c is MeshCollider m?AssetDatabase.GetAssetPath(m.sharedMesh):""}).ToArray();
  File.WriteAllText(Out+"/survey.json",new JObject{["method"]="Read-only Editor terrain and collision measurements; no game launched",["player"]=new JObject{["speed"]=player.speed,["jump"]=player.jumpHeight,["gravity"]=player.gravity,["height"]=player.controller.height,["radius"]=player.controller.radius,["step"]=player.controller.stepOffset,["slope"]=player.controller.slopeLimit,["skin"]=player.controller.skinWidth},["recorded_takeoff"]=V(start),["nearby_colliders"]=new JArray(nearby),["grid"]=grid,["replay"]=Replay(player,start,-141.8f,1.5f)}.ToString());
  ConRoiV48R21TerrainGround.Dispose(probes);
 }
 static JObject Replay(DieuKhienDiChuyen player,Vector3 start,float yaw,float moveSeconds)
 {
  var original=player.controller;var g=new GameObject("ReadOnly_Recorded_Jump_Replay");g.layer=original.gameObject.layer;
  var c=g.AddComponent<CharacterController>();c.height=original.height;c.radius=original.radius;c.center=original.center;c.skinWidth=original.skinWidth;c.slopeLimit=original.slopeLimit;c.stepOffset=original.stepOffset;c.minMoveDistance=original.minMoveDistance;g.transform.position=start;Physics.SyncTransforms();
  var direction=Quaternion.Euler(0,yaw,0)*Vector3.forward;float vy=Mathf.Sqrt(player.jumpHeight*-2*player.gravity),peak=start.y;var collisions=new HashSet<string>();var frames=new JArray();
  // Same controller shape and ballistic parameters; 120Hz synthetic displacement,
  // not gameplay input or acceptance of native integration.
  for(int i=0;i<300;i++){
   float t=i/120f;vy+=player.gravity/120;c.Move((direction*(t<moveSeconds?player.speed:0)+Vector3.up*vy)/120);peak=Mathf.Max(peak,g.transform.position.y);
   if(c.isGrounded&&vy<0)vy=-2;
   if(i%12==0)frames.Add(new JObject{["t"]=t,["position"]=V(g.transform.position),["grounded"]=c.isGrounded});
  }
  var end=g.transform.position;UnityEngine.Object.DestroyImmediate(g);
  return new JObject{["method"]="Editor CharacterController.Move, 120Hz ballistic sweep with unchanged scene Player dimensions/parameters; not native gameplay",["peak_center_y"]=peak,["end"]=V(end),["frames"]=frames};
 }
 static MeshCollider[] terrain;
 static float Ground(Vector3 p)
 {
  if(!ConRoiV48R21TerrainGround.Sample(terrain,new Vector2(p.x,p.z),out float y))throw new Exception("No terrain support: "+p);return y;
 }
 static float NearHigh(Vector3 p)
 {
  float y=Ground(p);for(int j=0;j<8;j++){var q=p+Quaternion.Euler(0,j*45,0)*Vector3.forward*3.5f;if(ConRoiV48R21TerrainGround.Sample(terrain,new Vector2(q.x,q.z),out float h))y=Mathf.Max(y,h);}return y;
 }
 sealed class MeshWriter
 {
  readonly List<Vector3> v=new List<Vector3>();readonly List<int> t=new List<int>(),soil=new List<int>();readonly List<Vector2> uv=new List<Vector2>();
  public void Quad(Vector3 a,Vector3 b,Vector3 c,Vector3 d,Vector3 outward,bool planted=false)
  {
   if(Vector3.Dot(Vector3.Cross(b-a,c-a),outward)<0){var q=b;b=d;d=q;}
   int n=v.Count;v.AddRange(new[]{a,b,c,d});uv.AddRange(new[]{Vector2.zero,Vector2.up,Vector2.one,Vector2.right});(planted?soil:t).AddRange(new[]{n,n+1,n+2,n,n+2,n+3});
  }
  public void Triangle(Vector3 a,Vector3 b,Vector3 c,Vector3 outward)
  {
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
   for(int trial=0;trial<24;trial++){var l=p-normal*w;var r=p+normal*w;if(ConRoiV48R21TerrainGround.Sample(terrain,new Vector2(l.x,l.z),out _)&&ConRoiV48R21TerrainGround.Sample(terrain,new Vector2(r.x,r.z),out _))break;w*=.8f;}
   float height=top-p.y;
   var ring=new[]{new Vector2(-.35f,-.2f),new Vector2(-.85f,.3f),new Vector2(-1,height*.35f),new Vector2(-.91f,height*.60f),new Vector2(-.65f,height*.82f),new Vector2(-.32f,height*.95f),new Vector2(0,height),new Vector2(.32f,height*.95f),new Vector2(.65f,height*.82f),new Vector2(.91f,height*.60f),new Vector2(1,height*.35f),new Vector2(.85f,.3f),new Vector2(.35f,-.2f)}.Select(q=>p+normal*(q.x*w)+Vector3.up*q.y).ToArray();
   // Follow real terrain at both toes, avoiding hanging foliage/collision.
   ring[0].y=Ground(ring[0])-.15f;ring[^1].y=Ground(ring[^1])-.15f;ring[1].y=Mathf.Max(ring[0].y+.35f,p.y+.35f);ring[^2].y=Mathf.Max(ring[^1].y+.35f,p.y+.35f);rings.Add(ring);
  }
  for(int i=0;i<sections;i++)for(int j=0;j<13;j++){int k=(j+1)%13;var outwards=j==12?Vector3.down:(j<6?-normal:normal)+Vector3.up*(j>=2&&j<=9?1:.05f);writer.Quad(rings[i][j],rings[i+1][j],rings[i+1][k],rings[i][k],outwards);}
  foreach(int i in new[]{0,sections}){var ring=rings[i];var centre=ring.Aggregate(Vector3.zero,(sum,p)=>sum+p)/ring.Length;for(int j=0;j<13;j++)writer.Triangle(centre,ring[j],ring[(j+1)%13],i==0?-direction:direction);}
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
  for(int i=0;i<points.Count;i++)
  {
   var p=points[i];var tangent=points[Mathf.Min(i+1,points.Count-1)]-points[Mathf.Max(0,i-1)];tangent.y=0;var normal=Vector3.Cross(Vector3.up,tangent.normalized);float top=float.NegativeInfinity;
   foreach(float x in new[]{-.9f,-.45f,0,.45f,.9f})top=Mathf.Max(top,Ground(p+normal*x));
   // Only join existing walking surfaces at the two public approach endpoints.
   if(i<4||i>points.Count-5)foreach(var c in floors)if(c.Raycast(new Ray(p+Vector3.up*20,Vector3.down),out var hit,40)&&hit.normal.y>.7f&&hit.point.y<top+.45f)top=Mathf.Max(top,hit.point.y);
   top=heights[i];
   var topRing=new List<Vector3>();
   float SupportedWidth(float sign){float width=.78f;for(float x=.98f;x<=5.001f;x+=.2f){var q=p+normal*x*sign;if(!ConRoiV48R21TerrainGround.Sample(terrain,new Vector2(q.x,q.z),out _))break;width=x;}return width;}
   float leftWidth=SupportedWidth(-1),rightWidth=SupportedWidth(1);
   foreach(float x in new[]{-5f,-4f,-3f,-2f,-1.2f,-.78f,.78f,1.2f,2f,3f,4f,5f}){
    float actual=Mathf.Abs(x)<=.78f?x:Mathf.Sign(x)*(.78f+(Mathf.Abs(x)-.78f)/4.22f*((x<0?leftWidth:rightWidth)-.78f));var q=p+normal*actual;q.y=Mathf.Abs(x)<=.78f?top:Mathf.Max(Ground(q)+.06f,top-.4f*(Mathf.Abs(actual)-.78f));topRing.Add(q);
   }
   // Broad planted/gravel shoulders provide a traversable side entry from jump landings.
   var toeL=topRing[0];var toeR=topRing[^1];var bottomL=toeL;bottomL.y=Ground(toeL)-.12f;var bottomR=toeR;bottomR.y=Ground(toeR)-.12f;var ring=new[]{bottomL}.Concat(topRing).Concat(new[]{bottomR}).ToArray();rings.Add(ring);points[i]=new Vector3(p.x,top,p.z);
   sections.Add(new JObject{["centre"]=V(points[i]),["terrain_y"]=Ground(p),["fill_m"]=top-Ground(p),["toe_left"]=V(toeL),["toe_right"]=V(toeR)});
  }
  for(int i=0;i<rings.Count-1;i++)for(int j=0;j<rings[i].Length;j++){int k=(j+1)%rings[i].Length;writer.Quad(rings[i][j],rings[i+1][j],rings[i+1][k],rings[i][k],j==rings[i].Length-1?Vector3.down:j>0&&j<rings[i].Length-2?Vector3.up:(rings[i][j]+rings[i][k])/2-points[i]+Vector3.up*.1f,j!=6);}
  foreach(int i in new[]{0,rings.Count-1})for(int j=1;j<rings[i].Length-1;j++)writer.Triangle(rings[i][0],rings[i][j],rings[i][j+1],i==0?points[0]-points[1]:points[^1]-points[^2]);
  return writer.Finish(name);
 }
 public static void Author()
 {
  var story=Open();var previous=GameObject.Find("R21_Boundary_Landscape_Finish");if(previous)UnityEngine.Object.DestroyImmediate(previous);
  if(!File.Exists(Out+"/scene-before-boundary-finish.unity"))File.Copy(Scene,Out+"/scene-before-boundary-finish.unity",false);Directory.CreateDirectory(Art);terrain=ConRoiV48R21TerrainGround.CreateProbes();Physics.SyncTransforms();
  var root=new GameObject("R21_Boundary_Landscape_Finish");var hedge=Material("Cypress_Hedge",new Color(.135f,.205f,.145f));var gravel=Material("Bank_Gravel",new Color(.31f,.29f,.235f));var planted=Material("Bank_Planted_Shoulders",new Color(.13f,.22f,.13f));
  var old=UnityEngine.Object.FindObjectsByType<Transform>(FindObjectsInactive.Include).Single(t=>t.name=="R21_Actual_Terrain_Perimeter").gameObject;old.SetActive(false);var data=JObject.Parse(File.ReadAllText("Docs/Reviews/journey_v48/r21/asset-pass/actual-terrain-boundary.json"));var hedgeRoot=new GameObject("Grounded_Cypress_Boundary");hedgeRoot.transform.SetParent(root.transform,false);
  int count=0;AssetDatabase.StartAssetEditing();try{foreach(var row in data["panels"]){var a=new Vector3((float)row["a"][0],0,(float)row["a"][2]);var b=new Vector3((float)row["b"][0],0,(float)row["b"][2]);var inset=-Vector3.Cross(Vector3.up,(b-a).normalized)*.8f;a+=inset;b+=inset;MeshObject(hedgeRoot.transform,Hedge(a,b,"CypressBoundary_"+count++),hedge);}}finally{AssetDatabase.StopAssetEditing();}
  var gapRows=(JArray)data["unsupported_midpoints"];if(gapRows.Count>0){var centre=gapRows.Select(q=>new Vector3((float)q[0],0,(float)q[1])).Aggregate(Vector3.zero,(sum,p)=>sum+p)/gapRows.Count;MeshObject(hedgeRoot.transform,Hedge(centre-Vector3.right*.8f,centre+Vector3.right*.8f,"Cypress_Corner_Join",1.25f),hedge);}
  var paths=new JArray();foreach(float side in new[]{1f,-1f}){
   var points=TrailPoints(side);var mesh=Trail(points,side>0?"Lake_Bank_Return_Walk":"Garden_Bank_Return_Walk",out var sections);var path=MeshObject(root.transform,mesh,gravel);path.GetComponent<MeshRenderer>().sharedMaterials=new[]{gravel,planted};paths.Add(new JObject{["side"]=side,["sections"]=sections,["route"]=new JArray(points.Select(V))});
  }
  var scenery=story.GetComponent<CanhQuanChoV48>();scenery.scenery=scenery.scenery.Where(g=>g&&g!=old).Concat(new[]{root}).ToArray();
  ConRoiV48R21TerrainGround.Dispose(terrain);story.lake.SetActive(false);story.market.SetActive(false);EditorSceneManager.SaveScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene());AssetDatabase.SaveAssets();
  File.WriteAllText(Out+"/authoring.json",new JObject{["status"]="AUTHORED_REQUIRES_AUDIT_RENDER_NATIVE",["design"]="Retain open 1.02m bridge railing and allow bank exploration; two continuous grounded gravel return walks around both ends. Replace distant thin metal pickets with solid visible faceted evergreen hedge matching collision.",["old_boundary_preserved_inactive"]=true,["hedge_panels"]=count,["hedge_minimum_top_over_nearby_ground_m"]=4.13,["hedge_nearby_ground_radius_m"]=3.5,["trails"]=paths,["player_unchanged"]=true,["limits"]="Editor authoring, no game launched; native jumping, return walking, save/load and final rendering remain pending."}.ToString());
 }
 public static void AuditFinish()
 {
  var story=Open();var root=GameObject.Find("R21_Boundary_Landscape_Finish");var hedge=root.transform.Find("Grounded_Cypress_Boundary");terrain=ConRoiV48R21TerrainGround.CreateProbes();Physics.SyncTransforms();
  var author=JObject.Parse(File.ReadAllText(Out+"/authoring.json"));var obstacles=new JArray();var walkRows=new JArray();
  foreach(var trail in author["trails"])
  {
   var points=trail["route"].Select(q=>new Vector3((float)q[0],(float)q[1],(float)q[2])).ToArray();float maxGrade=0;
   for(int i=0;i<points.Length;i++){
    var p=points[i];if(i>0){var delta=p-points[i-1];maxGrade=Mathf.Max(maxGrade,Mathf.Abs(delta.y)/new Vector2(delta.x,delta.z).magnitude);}
    var hits=Physics.OverlapCapsule(p+Vector3.up*.38f,p+Vector3.up*1.50f,.28f,8,QueryTriggerInteraction.Ignore).Where(c=>!c.transform.IsChildOf(story.player.transform)).ToArray();
    if(hits.Length>0)obstacles.Add(new JObject{["side"]=trail["side"],["index"]=i,["point"]=V(p),["obstacles"]=new JArray(hits.Select(c=>P(c.transform)))});
   }
   // Full loops in both directions; also bank entry from the recorded landing.
   walkRows.Add(Walk(story.player,points[0]+Vector3.up*.94f,points,"trail_forward_"+trail["side"]));walkRows.Add(Walk(story.player,points[^1]+Vector3.up*.94f,points.Reverse().ToArray(),"trail_reverse_"+trail["side"]));
   trail["maximum_centre_grade"]=maxGrade;
  }
  var recordedReplay=Replay(story.player,new Vector3(663.494873f,2.0700598f,506.913635f),-141.8f,1.5f);var e=recordedReplay["end"];var recorded=new Vector3((float)e[0],(float)e[1],(float)e[2]);var lakeRoute=author["trails"][0]["route"].Select(q=>new Vector3((float)q[0],(float)q[1],(float)q[2])).ToArray();int near=Enumerable.Range(0,lakeRoute.Length).OrderBy(i=>Vector2.Distance(new Vector2(lakeRoute[i].x,lakeRoute[i].z),new Vector2(recorded.x,recorded.z))).First();
  walkRows.Add(Walk(story.player,recorded,lakeRoute.Take(near+1).Reverse().ToArray(),"recorded_landing_to_SE_approach"));walkRows.Add(Walk(story.player,recorded,lakeRoute.Skip(near).ToArray(),"recorded_landing_to_NW_approach"));
  var analogues=new JArray();
  foreach(float side in new[]{1f,-1f})foreach(float s in new[]{-10f,0f,12f,28f,44f,57f,66f}){
   var takeoff=SE+Axis*s+Water*side*(s<0||s>58?10f:5.8f);
   var hits=Physics.RaycastAll(takeoff+Vector3.up*15,Vector3.down,40,8,QueryTriggerInteraction.Ignore).Where(h=>h.normal.y>.7f).OrderByDescending(h=>h.point.y).ToArray();if(hits.Length==0)throw new Exception("No analogue takeoff floor "+takeoff);takeoff.y=hits[0].point.y+.94f;
   var direction=Water*side;float yaw=Mathf.Atan2(direction.x,direction.z)*Mathf.Rad2Deg;var replay=Replay(story.player,takeoff,yaw,1.5f);var q=replay["end"];var landing=new Vector3((float)q[0],(float)q[1],(float)q[2]);
   var route=author["trails"][side>0?0:1]["route"].Select(r=>new Vector3((float)r[0],(float)r[1],(float)r[2])).ToArray();int nearest=Enumerable.Range(0,route.Length).OrderBy(i=>(new Vector2(route[i].x-landing.x,route[i].z-landing.z)).sqrMagnitude).First();
   var returns=new JArray(Walk(story.player,landing,route.Take(nearest+1).Reverse().ToArray(),"SE"),Walk(story.player,landing,route.Skip(nearest).ToArray(),"NW"));
   analogues.Add(new JObject{["s"]=s,["side"]=side,["takeoff"]=V(takeoff),["replay"]=replay,["returns"]=returns});
  }
  var data=JObject.Parse(File.ReadAllText("Docs/Reviews/journey_v48/r21/asset-pass/actual-terrain-boundary.json"));var misses=new JArray();int casts=0;
  foreach(var row in data["panels"]){var a=new Vector3((float)row["a"][0],0,(float)row["a"][2]);var b=new Vector3((float)row["b"][0],0,(float)row["b"][2]);var d=(b-a).normalized;var normal=Vector3.Cross(Vector3.up,d);
   foreach(var p in new[]{a,(a+b)/2,b})foreach(float sign in new[]{-1f,1f})foreach(float jump in new[]{0f,3.05f}){
    var origin=p+normal*sign*3.5f;if(!ConRoiV48R21TerrainGround.Sample(terrain,new Vector2(origin.x,origin.z),out float y))y=Ground(p);origin.y=y+jump;
    bool hit=Physics.CapsuleCastAll(origin+Vector3.up*.3f,origin+Vector3.up*1.5f,.3f,-normal*sign,7,8,QueryTriggerInteraction.Ignore).Any(h=>h.collider.transform.IsChildOf(hedge));casts++;if(!hit)misses.Add(new JObject{["point"]=V(p),["jump"]=jump,["direction"]=sign});
   }
  }
  var badMesh=new JArray();int tris=0;foreach(var f in root.GetComponentsInChildren<MeshFilter>()){
   var m=f.sharedMesh;var v=m.vertices;var t=m.triangles;tris+=t.Length/3;int bad=0;for(int i=0;i<t.Length;i+=3)if(Vector3.Cross(v[t[i+1]]-v[t[i]],v[t[i+2]]-v[t[i]]).sqrMagnitude<1e-12f)bad++;
   if(bad>0||f.GetComponent<MeshCollider>().sharedMesh!=m)badMesh.Add(new JObject{["name"]=f.name,["degenerate"]=bad,["collision_matches_visible"]=f.GetComponent<MeshCollider>().sharedMesh==m});
  }
  ConRoiV48R21TerrainGround.Dispose(terrain);
  File.WriteAllText(Out+"/physics-audit.json",new JObject{["method"]="Editor physics only. CharacterController shape/parameters from unchanged Player; synthetic 120Hz walking and capsule sweeps, not native input acceptance.",["recorded_jump_replay"]=recordedReplay,["analogue_jump_returns"]=analogues,["walks"]=walkRows,["headroom_obstacles"]=obstacles,["trails"]=author["trails"],["boundary_capsule_casts"]=casts,["boundary_misses"]=misses,["mesh_issues"]=badMesh,["new_triangles"]=tris,["native_status"]="NOT_RUN_USER_RESERVATION_REQUIRED"}.ToString());
 }
 static JObject Walk(DieuKhienDiChuyen player,Vector3 start,Vector3[] points,string name)
 {
  var g=new GameObject("Temporary_Return_Walk_Controller");g.layer=player.controller.gameObject.layer;var c=g.AddComponent<CharacterController>();var original=player.controller;c.height=original.height;c.radius=original.radius;c.center=original.center;c.skinWidth=original.skinWidth;c.slopeLimit=original.slopeLimit;c.stepOffset=original.stepOffset;c.minMoveDistance=original.minMoveDistance;g.transform.position=start;Physics.SyncTransforms();
  float vy=-2;int reached=0;var failures=new JArray();
  foreach(var target in points){bool arrived=false;for(int i=0;i<480;i++){
   var delta=target-g.transform.position;delta.y=0;if(delta.magnitude<.22f){arrived=true;break;}if(c.isGrounded&&vy<0)vy=-2;vy+=player.gravity/120;c.Move((delta.normalized*player.speed+Vector3.up*vy)/120);
  }if(!arrived){failures.Add(new JObject{["target"]=V(target),["stopped"]=V(g.transform.position)});break;}reached++;}
  var end=g.transform.position;UnityEngine.Object.DestroyImmediate(g);return new JObject{["name"]=name,["start"]=V(start),["end"]=V(end),["targets"]=points.Length,["reached"]=reached,["failures"]=failures};
 }
 public static void RenderFinish()
 {
  var story=Open();story.hub.SetActive(false);foreach(var g in story.GetComponent<CanhQuanChoV48>().scenery)if(g)g.SetActive(true);terrain=ConRoiV48R21TerrainGround.CreateProbes();Physics.SyncTransforms();
  var eye=UnityEngine.Object.Instantiate(story.player.GetComponentInChildren<Camera>());eye.transform.SetParent(null);eye.gameObject.SetActive(true);eye.enabled=false;eye.fieldOfView=60;eye.farClipPlane=1000;
  var views=JArray.Parse(File.ReadAllText("Docs/Reviews/journey_v48/r21/nativefix2-clear/views.json"));
  void View(string name,Vector3 p,Vector3 at){views.Add(new JObject{["name"]=name,["eye"]=V(p),["lookAt"]=V(at)});}
  View("recorded-landing-return",new Vector3(660.198f,.82f,502.842f),SE+Axis*4+Water*13.5f);
  var garden=SE+Axis*28-Water*13.5f;garden.y=Ground(garden)+1.65f;View("garden-bank-return",garden,SE-Axis*15-Water*13.5f);
  View("bank-path-SE-join",SE-Axis*22+Water*4+Vector3.up*1.7f,SE-Axis*15+Water*12);
  View("bank-path-NW-join",NW+Axis*20+Water*5+Vector3.up*1.7f,NW+Axis*10+Water*13);
  View("hedge-from-lake-path",new Vector3(549.7f,4.1f,529.4f),new Vector3(560,5,585));
  var corner=new Vector3(752,0,585);corner.y=Ground(corner)+1.65f;View("hedge-close-corner",corner,new Vector3(758,corner.y,590));
  foreach(bool mist in new[]{false,true}){
   var folder=Out+(mist?"/editor-mist":"/editor-clear");Directory.CreateDirectory(folder);RenderSettings.fog=mist;RenderSettings.fogMode=FogMode.ExponentialSquared;RenderSettings.fogDensity=.006f;RenderSettings.fogColor=new Color(.36f,.43f,.47f);
   foreach(var view in views){var p=view["eye"];var q=view["lookAt"];var position=new Vector3((float)p[0],(float)p[1],(float)p[2]);var at=new Vector3((float)q[0],(float)q[1],(float)q[2]);eye.transform.SetPositionAndRotation(position,Quaternion.LookRotation(at-position));var rt=RenderTexture.GetTemporary(1280,720,24);var tex=new Texture2D(1280,720,TextureFormat.RGB24,false);
    for(int i=0;i<3;i++)UnityEngine.Rendering.RenderPipeline.SubmitRenderRequest(eye,new UnityEngine.Rendering.Universal.UniversalRenderPipeline.SingleCameraRequest{destination=rt});var old=RenderTexture.active;RenderTexture.active=rt;tex.ReadPixels(new Rect(0,0,1280,720),0,0);tex.Apply();RenderTexture.active=old;File.WriteAllBytes(folder+"/"+view["name"]+".png",tex.EncodeToPNG());RenderTexture.ReleaseTemporary(rt);UnityEngine.Object.DestroyImmediate(tex);
   }
   File.WriteAllText(folder+"/method.json",new JObject{["method"]="Unity Editor batch URP render request, no Game/player process, no OS input, no play mode. Static authoring preview; runtime reflection/Bao pose and UI are not certified.",["views"]=views}.ToString());
  }
  ConRoiV48R21TerrainGround.Dispose(terrain);
 }
 // Author must exit before verification: verify the saved meshes in a fresh Editor.
 public static void AuditAndPreview(){AuditFinish();RenderFinish();}
 public static void VerifyAndBuild(){AuditFinish();RenderFinish();BuildFinal();}
 public static void OverlayFootways()
 {
  var story=Open();var root=GameObject.Find("R21_Boundary_Landscape_Finish");
  foreach(var t in root.GetComponentsInChildren<Transform>().Where(t=>t.name.StartsWith("FootwayOverShoulder_")).ToArray())UnityEngine.Object.DestroyImmediate(t.gameObject);
  var paths=root.GetComponentsInChildren<MeshFilter>().Where(f=>f.name.EndsWith("Bank_Return_Walk")).ToArray();var originals=GameObject.Find("R20_Whole_Source_Export").GetComponentsInChildren<MeshFilter>().Where(f=>f.name=="Surface_125"||f.name=="Surface_126"||f.name=="Surface_153").ToArray();var rows=new JArray();
  Physics.SyncTransforms();
  foreach(var path in paths)foreach(var original in originals){
   var split=ConRoiV48R21PathConform.Split(original.sharedMesh,original.transform,path);var vertices=split.vertices;var faces=split.triangles;var writer=new MeshWriter();int count=0;var collider=path.GetComponent<MeshCollider>();
   bool Hit(Vector3 p,out RaycastHit hit)=>collider.Raycast(new Ray(new Vector3(p.x,40,p.z),Vector3.down),out hit,100)&&hit.normal.y>.3f;
   for(int i=0;i<faces.Length;i+=3){
    var triangle=new[]{original.transform.TransformPoint(vertices[faces[i]]),original.transform.TransformPoint(vertices[faces[i+1]]),original.transform.TransformPoint(vertices[faces[i+2]])};var centre=(triangle[0]+triangle[1]+triangle[2])/3;
    if(!Hit(centre,out _))continue;
    for(int j=0;j<3;j++)if(Hit(triangle[j],out var hit))triangle[j].y=Mathf.Max(triangle[j].y,hit.point.y+.025f);
    if(Vector3.Cross(triangle[1]-triangle[0],triangle[2]-triangle[0]).sqrMagnitude<1e-12f)continue;
    writer.Triangle(triangle[0],triangle[1],triangle[2],Vector3.up);count++;
   }
   UnityEngine.Object.DestroyImmediate(split);if(count==0)continue;
   var mesh=writer.Finish("FootwayOverShoulder_"+path.name+"_"+original.name);var overlay=MeshObject(root.transform,mesh,original.GetComponent<MeshRenderer>().sharedMaterial);rows.Add(new JObject{["source"]=P(original.transform),["original_mesh_unchanged"]=true,["new_overlay"]=overlay.name,["triangles"]=count});
  }
  story.lake.SetActive(false);story.market.SetActive(false);EditorSceneManager.SaveScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene());AssetDatabase.SaveAssets();File.WriteAllText(Out+"/footway-overlays.json",rows.ToString());
 }
 public static void FoliageAndPreview()
 {
  var texture=new Texture2D(128,128,TextureFormat.RGB24,false);var pixels=new Color[128*128];
  for(int y=0;y<128;y++)for(int x=0;x<128;x++){float value=.76f+.2f*Mathf.PerlinNoise(x*.15f,y*.15f);pixels[y*128+x]=new Color(value,value,value);}
  var random=new System.Random(21);
  for(int i=0;i<900;i++){
   int x=random.Next(128),y=random.Next(128);float shade=.48f+(float)random.NextDouble()*.28f;
   for(int k=0;k<4;k++){int index=((y+k)%128)*128+(x+k/2)%128;pixels[index]=new Color(shade,shade,shade);}
  }
  texture.SetPixels(pixels);texture.Apply();var path=Art+"/Cypress_Needles.png";File.WriteAllBytes(path,texture.EncodeToPNG());UnityEngine.Object.DestroyImmediate(texture);AssetDatabase.ImportAsset(path,ImportAssetOptions.ForceSynchronousImport);
  var importer=(TextureImporter)AssetImporter.GetAtPath(path);importer.wrapMode=TextureWrapMode.Repeat;importer.mipmapEnabled=true;importer.maxTextureSize=128;importer.SaveAndReimport();
  var material=AssetDatabase.LoadAssetAtPath<Material>(Art+"/Cypress_Hedge.mat");material.SetTexture("_BaseMap",AssetDatabase.LoadAssetAtPath<Texture2D>(path));material.SetColor("_BaseColor",new Color(.18f,.27f,.18f));EditorUtility.SetDirty(material);AssetDatabase.SaveAssets();RenderFinish();
 }
 public static void BuildFinal()
 {
  EditorSceneManager.OpenScene(Scene);
  var audit=JObject.Parse(File.ReadAllText(Out+"/physics-audit.json"));
  if(((JArray)audit["boundary_misses"]).Count>0||((JArray)audit["mesh_issues"]).Count>0||((JArray)audit["headroom_obstacles"]).Count>0||audit["walks"].Any(w=>((JArray)w["failures"]).Count>0)||audit["analogue_jump_returns"].Any(a=>a["returns"].All(w=>((JArray)w["failures"]).Count>0)))throw new Exception("Resolve boundary physics audit before build");
  var reflection=UnityEngine.Object.FindAnyObjectByType<PhanChieuMatHo>();
  if(!reflection.water.enabled||P(reflection.water.transform)!="R20_Whole_Source_Export/Surface_051")throw new Exception("Preserve reflection binding");
  foreach(var light in UnityEngine.Object.FindObjectsByType<Light>(FindObjectsInactive.Include).Where(l=>l.type!=LightType.Directional&&l.shadows!=LightShadows.None)){
   var data=light.GetComponent<UnityEngine.Rendering.Universal.UniversalAdditionalLightData>();if(!data||new SerializedObject(data).FindProperty("m_AdditionalLightsShadowResolutionTier").intValue!=UnityEngine.Rendering.Universal.UniversalAdditionalLightData.AdditionalLightsShadowResolutionTierLow)throw new Exception("Preserve practical shadow tier");
  }
  var version=PlayerSettings.bundleVersion;
  try{
   PlayerSettings.bundleVersion="0.48.21";
   var report=BuildPipeline.BuildPlayer(new[]{Scene},"Builds/Windows_v48_r21_boundaryfinal/ConRoi.exe",BuildTarget.StandaloneWindows64,BuildOptions.Development);
   var messages=new JArray();foreach(var step in report.steps)foreach(var message in step.messages)if(message.type==LogType.Warning||message.type==LogType.Error||message.type==LogType.Exception)messages.Add(new JObject{["step"]=step.name,["type"]=message.type.ToString(),["message"]=message.content});
   File.WriteAllText(Out+"/build.json",new JObject{["result"]=report.summary.result.ToString(),["errors"]=report.summary.totalErrors,["warnings"]=report.summary.totalWarnings,["messages"]=messages,["status"]="INTERNAL_PENDING_FINAL_FOREGROUND_NOT_A_CANDIDATE"}.ToString());
   if(report.summary.result!=UnityEditor.Build.Reporting.BuildResult.Succeeded)throw new Exception("Boundary build failed");
  }finally{PlayerSettings.bundleVersion=version;AssetDatabase.SaveAssets();}
 }
}
