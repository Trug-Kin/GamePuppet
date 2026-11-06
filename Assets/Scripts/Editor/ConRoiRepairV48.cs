using System;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.Rendering;
using Newtonsoft.Json.Linq;

public static class ConRoiRepairV48
{
 public const string Report="Docs/Reviews/journey_v48",Scene="Assets/Scenes/JourneyRepair_v48.unity";
 static string PathOf(Transform t)=>t.parent?PathOf(t.parent)+"/"+t.name:t.name;
 public static void Assemble()
 {
  EditorSceneManager.OpenScene("Assets/Scenes/JourneyAcceptance_v47_rc3.unity");
  var s=UnityEngine.Object.FindAnyObjectByType<DieuPhoiHanhTrinh>();s.gameObject.AddComponent<HuongDanHanhTrinhV48>().story=s;
  var continuity=s.gameObject.AddComponent<CanhQuanChoV48>();continuity.story=s;
  var staticNames=new[]{"V41_Market_Craft","V41_Entrance_Lamp"};
  continuity.scenery=staticNames.Select(n=>s.market.transform.Find(n).gameObject).ToArray();
  foreach(var g in continuity.scenery)g.transform.SetParent(null,true);
  var desk=s.hub.GetComponentsInChildren<Transform>(true).First(t=>t.name=="Desk_Puppet_Broken_Strings");
  var head=desk.GetComponentsInChildren<Renderer>(true).First(r=>r.name=="ConRoi_Head_SameAsMinh");var body=desk.GetComponentsInChildren<Renderer>(true).First(r=>r.name=="ConRoi_Body_Wood");
  float gap=head.bounds.min.y-body.bounds.max.y;head.transform.position+=Vector3.up*(.003f-gap);
  var deskTop=s.hub.GetComponentsInChildren<Renderer>(true).First(r=>r.name=="CR_Desk_Top");
  var room=s.GetComponent<PhongTrongMoV47>();
  for(int i=0;i<room.brokenStrings.Length;i++)
  {var r=room.brokenStrings[i];r.transform.rotation=Quaternion.AngleAxis(90,Vector3.right)*r.transform.rotation;var center=body.bounds.center+new Vector3((i-2)*.024f,0,.04f+(i%2)*.025f);center.y=deskTop.bounds.max.y+.004f+r.bounds.extents.y;r.transform.position+=center-r.bounds.center;}
  File.WriteAllText(Report+"/desk-repair.json",new JObject{["original_head_body_gap_m"]=gap,["new_gap_m"]=head.bounds.min.y-body.bounds.max.y,["head_mesh_changed"]=false,["broken_strings"]="Five existing wire meshes laid on tabletop in waking room; repaired dream wires remain separate."}.ToString());
  File.WriteAllText(Report+"/market-continuity.json",new JObject{["moved_existing_groups"]=new JArray(staticNames),["reason"]="RC3 chapter activation hid market goods/entrance scenery when approaching on foot during the lake chapter. Static city architecture was already active and is retained.",["geometry_replaced"]=false,["chapter_actors_changed"]=false}.ToString());
  var root=new GameObject("V48_Authored_Wayfinding").transform;
  Directory.CreateDirectory("Assets/Art/Environment/JourneyV48");
  var timber=AssetDatabase.LoadAssetAtPath<Material>("Assets/Art/Environment/JourneyV48/Sign_Painted_Green.mat");
  if(!timber){timber=new Material(Shader.Find("Universal Render Pipeline/Lit"));timber.color=new Color(.07f,.15f,.12f);timber.SetFloat("_Smoothness",.12f);AssetDatabase.CreateAsset(timber,"Assets/Art/Environment/JourneyV48/Sign_Painted_Green.mat");}
  var rows=new JArray();
  void Part(Transform parent,string name,Vector3 pos,Vector3 size)
  {var g=GameObject.CreatePrimitive(PrimitiveType.Cube);g.name=name;g.transform.SetParent(parent,false);g.transform.localPosition=pos;g.transform.localScale=size;g.layer=3;g.GetComponent<Renderer>().sharedMaterial=timber;UnityEngine.Object.DestroyImmediate(g.GetComponent<Collider>());}
  void Sign(Transform parent,string label,Vector3 p,Vector3 toward,float width=2.6f)
  {
   var t=new GameObject("Sign_"+label.Replace('\n',' ')).transform;t.SetParent(parent,true);t.position=p;
   var d=toward-p;d.y=0;t.rotation=Quaternion.LookRotation(-d);
   Part(t,"Painted_board",new Vector3(0,1.65f,0),new Vector3(width,.70f,.10f));Part(t,"Post",new Vector3(0,.75f,.05f),new Vector3(.12f,1.5f,.12f));
   var text=new GameObject("Lettering",typeof(TextMesh)).GetComponent<TextMesh>();text.transform.SetParent(t,false);text.transform.localPosition=new Vector3(0,1.65f,-.061f);text.font=Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");text.fontSize=72;text.characterSize=.035f;text.anchor=TextAnchor.MiddleCenter;text.alignment=TextAlignment.Center;text.text=label;text.color=new Color(.94f,.91f,.74f);text.GetComponent<MeshRenderer>().sharedMaterial=text.font.material;
   rows.Add(new JObject{["label"]=label,["position"]=p.ToString("F3")});
  }
  var route=JObject.Parse(File.ReadAllText("Assets/Resources/VerificationV36/route.json"))["route"];
  Vector3 P(int i)=>new Vector3((float)route[i][0],(float)route[i][1],(float)route[i][2]);
  foreach(int i in new[]{31,45,61,124,140,165,203,247})
  {
   var p=P(i);var forward=(P(i+1)-p).normalized;var side=Vector3.Cross(Vector3.up,forward).normalized;
   // Stand beside the existing paving; the board faces the ordinary approach.
   Sign(root,"↑  CHỢ ĐÀ LẠT\nTheo lối lát",p+side*1.8f,p-forward*5);
   Sign(root,"↑  HỒ XUÂN HƯƠNG",p-side*1.8f,p+forward*5);
  }
  var boundary=new GameObject("V48_Closed_Outer_Streets").transform;
  void Gate(string name,Vector3 position,float yaw,float width)
  {
   if(Physics.Raycast(position+Vector3.up*40,Vector3.down,out var support,80,8))position.y=support.point.y;
   var t=new GameObject(name).transform;t.SetParent(boundary,false);t.position=position;t.rotation=Quaternion.Euler(0,yaw,0);
   Part(t,"Top_Rail",new Vector3(0,3.3f,0),new Vector3(width,.09f,.09f));Part(t,"Lower_Rail",new Vector3(0,.25f,0),new Vector3(width,.09f,.09f));
   for(float x=-width/2;x<=width/2;x+=.35f)Part(t,"Closed_Vertical_Bar",new Vector3(x,1.7f,0),new Vector3(.06f,3.4f,.06f));
   var box=t.gameObject.AddComponent<BoxCollider>();box.center=Vector3.up*1.7f;box.size=new Vector3(width,3.4f,.12f);t.gameObject.layer=3;
   Sign(t,"LỐI ĐANG ĐÓNG\nĐi theo biển chợ",position+t.right*2.5f,position-t.forward*5,2.8f);
   rows.Add(new JObject{["closed_branch"]=name,["position"]=position.ToString("F3"),["width"]=width});
  }
  Gate("North_Roundabout_Outside_Play",new Vector3(607,0,590),0,18);
  Gate("West_Roundabout_Outside_Play",new Vector3(588,0,580),90,18);
  Gate("East_Market_Junction_Outside_Play",new Vector3(760,0,463),90,18);
  continuity.scenery=continuity.scenery.Concat(new[]{boundary.gameObject}).ToArray();
  foreach(var exit in UnityEngine.Object.FindObjectsByType<TuongTacHoCho>(FindObjectsInactive.Include).Where(i=>i.kind==TuongTacHoCho.Kind.Wake&&i.GetComponent<Collider>()))
  {var p=exit.GetComponent<Collider>().bounds.center;p.y=Physics.Raycast(p+Vector3.up*2,Vector3.down,out var hit,10,8)?hit.point.y:p.y-1;Sign(exit.transform.parent,"TỈNH DẬY\nE · Về phòng trọ",p+Vector3.left*2,p+Vector3.back*4,2.4f);}
  var school=s.schoolChapter;Sign(school.environment.transform,"GIẢNG ĐƯỜNG  →",school.spawn.position+new Vector3(-2,-.94f,3),school.spawn.position);
  foreach(var exit in school.environment.GetComponentsInChildren<TuongTacTruongV37>(true).Where(i=>i.exit&&i.GetComponent<Collider>())){var p=exit.GetComponent<Collider>().bounds.center;Sign(school.environment.transform,"TỈNH DẬY\nE · Về phòng trọ",p+new Vector3(-1.6f,-1,0),p+Vector3.back*4);}
  foreach(var exit in s.slopeChapter.environment.GetComponentsInChildren<TuongTacDocV39>(true).Where(i=>i.finalDoor&&i.GetComponent<Collider>())){var p=exit.GetComponent<Collider>().bounds.center;Sign(exit.transform,"CỬA CUỐI DỐC\nE · Mở cửa",p+new Vector3(-3.8f,-1,-1),p+Vector3.back*4);}
  File.WriteAllText(Report+"/wayfinding.json",rows.ToString());
  EditorSceneManager.SaveScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene(),Scene);AssetDatabase.SaveAssets();
 }
 public static void Build()
 {
  EditorSceneManager.OpenScene(Scene);PlayerSettings.bundleVersion="0.48.4";EditorBuildSettings.scenes=new[]{new EditorBuildSettingsScene(Scene,true)};
  var b=BuildPipeline.BuildPlayer(new[]{Scene},"Builds/Windows_v48_r4/ConRoi.exe",BuildTarget.StandaloneWindows64,BuildOptions.Development);
  File.WriteAllText(Report+"/build.json",new JObject{["result"]=b.summary.result.ToString(),["errors"]=b.summary.totalErrors,["warnings"]=b.summary.totalWarnings}.ToString());if(b.summary.totalErrors>0)throw new Exception("Build failed");
 }
 public static void AssembleAndBuild(){Assemble();Build();}
 public static void AuditMore()
 {
  string MeshIdentity(Mesh mesh){if(!mesh)return "none";AssetDatabase.TryGetGUIDAndLocalFileIdentifier(mesh,out string guid,out long file);return guid+":"+file+":"+mesh.name;}
  System.Collections.Generic.Dictionary<string,string> PuppetMeshes(DieuPhoiHanhTrinh story)
  {
   var groups=story.puppets.Concat(new[]{story.schoolChapter.puppet,story.slopeChapter.puppet}).Select(p=>p.transform).Concat(new[]{story.hub.GetComponentsInChildren<Transform>(true).First(t=>t.name=="Desk_Puppet_Broken_Strings")});
   return groups.SelectMany(t=>t.GetComponentsInChildren<MeshFilter>(true)).GroupBy(f=>PathOf(f.transform)).ToDictionary(g=>g.Key,g=>string.Join("|",g.Select(f=>MeshIdentity(f.sharedMesh)).OrderBy(v=>v)));
  }
  EditorSceneManager.OpenScene("Assets/Scenes/JourneyAcceptance_v47_rc3.unity");var s=UnityEngine.Object.FindAnyObjectByType<DieuPhoiHanhTrinh>();
  var oldPuppet=PuppetMeshes(s);
  var desk=s.hub.GetComponentsInChildren<Transform>(true).First(t=>t.name=="Desk_Puppet_Broken_Strings");
  File.WriteAllText(Report+"/desk-rc3.json",new JArray(desk.GetComponentsInChildren<Renderer>(true).Select(r=>new JObject{["name"]=r.name,["enabled"]=r.enabled,["bounds"]=r.bounds.ToString("F5"),["scale"]=r.transform.lossyScale.ToString("F5"),["position"]=r.transform.position.ToString("F5")})).ToString());
  var map=UnityEngine.Object.FindObjectsByType<MeshRenderer>(FindObjectsInactive.Include).Where(r=>r.name.StartsWith("Surface_")||r.name.StartsWith("Context_")).ToDictionary(r=>PathOf(r.transform),r=>new{mesh=AssetDatabase.GetAssetPath(r.GetComponent<MeshFilter>().sharedMesh),matrix=r.transform.localToWorldMatrix.ToString("F5"),r.enabled});
  EditorSceneManager.OpenScene(Scene);var after=UnityEngine.Object.FindObjectsByType<MeshRenderer>(FindObjectsInactive.Include).Where(r=>r.name.StartsWith("Surface_")||r.name.StartsWith("Context_")).ToDictionary(r=>PathOf(r.transform),r=>new{mesh=AssetDatabase.GetAssetPath(r.GetComponent<MeshFilter>().sharedMesh),matrix=r.transform.localToWorldMatrix.ToString("F5"),r.enabled});
  var rows=new JArray(map.Select(k=>new JObject{["renderer"]=k.Key,["same_mesh_transform_and_visibility"]=after.ContainsKey(k.Key)&&k.Value.Equals(after[k.Key])}));
  File.WriteAllText(Report+"/architecture-rc3-v48.json",rows.ToString());
  var newPuppet=PuppetMeshes(UnityEngine.Object.FindAnyObjectByType<DieuPhoiHanhTrinh>());
  File.WriteAllText(Report+"/puppet-meshes-rc3-v48.json",new JArray(oldPuppet.Select(k=>new JObject{["renderer_path"]=k.Key,["source_mesh_identity"]=k.Value,["same_mesh"]=newPuppet.ContainsKey(k.Key)&&newPuppet[k.Key]==k.Value})).ToString());
 }
 public static void BuildBaseline()
 {
  EditorSceneManager.OpenScene("Assets/Scenes/JourneyAcceptance_v47_rc3.unity");
  var b=BuildPipeline.BuildPlayer(new[]{"Assets/Scenes/JourneyAcceptance_v47_rc3.unity"},"Builds/Windows_v48_rc3_diagnostic/ConRoi.exe",BuildTarget.StandaloneWindows64,BuildOptions.Development);
  File.WriteAllText(Report+"/baseline-build.json",new JObject{["result"]=b.summary.result.ToString(),["errors"]=b.summary.totalErrors,["purpose"]="RC3 scene and production behavior; additional opt-in New Game route observer only. Original RC3 ZIP remains unchanged."}.ToString());
 }
 public static void Audit()
 {
  Directory.CreateDirectory(Report);EditorSceneManager.OpenScene("Assets/Scenes/JourneyAcceptance_v47_rc3.unity");
  var all=UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects().SelectMany(g=>g.GetComponentsInChildren<Transform>(true)).ToArray();
  var s=UnityEngine.Object.FindAnyObjectByType<DieuPhoiHanhTrinh>();
  var rows=new JArray(all.SelectMany(t=>t.GetComponents<Renderer>()).Where(r=>r.name.StartsWith("Surface_")||r.name.StartsWith("Context_")||r.name.Contains("TTC")||r.name.Contains("P32")).Select(r=>new JObject{["path"]=PathOf(r.transform),["enabled"]=r.enabled,["active"]=r.gameObject.activeInHierarchy,["position"]=r.transform.position.ToString("F3"),["bounds"]=r.bounds.ToString("F3"),["mesh"]=r.GetComponent<MeshFilter>()?AssetDatabase.GetAssetPath(r.GetComponent<MeshFilter>().sharedMesh):"",["materials"]=new JArray(r.sharedMaterials.Select(m=>m?AssetDatabase.GetAssetPath(m):"null"))}));
  File.WriteAllText(Report+"/rc3-scene-audit.json",new JObject{["roots"]=new JArray(all.Where(t=>!t.parent).Select(t=>t.name)),["renderers"]=rows,["volumes"]=new JArray(all.SelectMany(t=>t.GetComponents<Volume>()).Select(v=>new JObject{["path"]=PathOf(v.transform),["weight"]=v.weight,["global"]=v.isGlobal,["profile"]=v.sharedProfile?AssetDatabase.GetAssetPath(v.sharedProfile):""})),["lights"]=new JArray(all.SelectMany(t=>t.GetComponents<Light>()).Select(l=>new JObject{["path"]=PathOf(l.transform),["intensity"]=l.intensity,["range"]=l.range,["type"]=l.type.ToString(),["position"]=l.transform.position.ToString("F3")})),["puppets"]=new JArray(s.puppets.Concat(new[]{s.schoolChapter.puppet,s.slopeChapter.puppet}).Select(p=>new JObject{["path"]=PathOf(p.transform),["position"]=p.transform.position.ToString("F3"),["head"]=p.dauTransform.position.ToString("F3"),["scale"]=p.transform.lossyScale.ToString("F3")}))}.ToString());
 }
}
