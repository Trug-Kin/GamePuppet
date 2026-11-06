using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public static class ConRoiV48AcceptanceRepair
{
 const string Out="Docs/Reviews/journey_v48/r11",Art="Assets/Art/Environment/JourneyV48";
 public const string Scene="Assets/Scenes/JourneyRepair_v48_r11.unity";
 static Vector3 P(JToken t)=>new Vector3((float)t[0],(float)t[1],(float)t[2]);
 static JArray V(Vector3 p)=>new JArray(p.x,p.y,p.z);
 static void Import(string path)
 {
  AssetDatabase.ImportAsset(path,ImportAssetOptions.ForceSynchronousImport);
  var i=(ModelImporter)AssetImporter.GetAtPath(path);i.isReadable=true;i.addCollider=false;i.materialImportMode=ModelImporterMaterialImportMode.None;i.SaveAndReimport();
 }
 public static void Assemble()
 {
  Directory.CreateDirectory(Out);AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
  Import(Art+"/MapRestoredR11/RestoredMarket.fbx");Import(Art+"/BridgeRepairR11/BridgeLocalSupport.fbx");
  EditorSceneManager.OpenScene("Assets/Scenes/JourneyRepair_v48_r10.unity");
  var s=UnityEngine.Object.FindAnyObjectByType<DieuPhoiHanhTrinh>();
  var restorations=JObject.Parse(File.ReadAllText(Art+"/MapRestoredR11/manifest.json"));
  var restored=(GameObject)PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>(Art+"/MapRestoredR11/RestoredMarket.fbx"));restored.name="R11_Restored_Source_Market";
  var map=new JArray();
  foreach(var f in restored.GetComponentsInChildren<MeshFilter>())
  {
   var item=restorations["meshes"].Single(t=>(string)t["mesh"]==f.name);
   string reuse=(string)item["reuse_material"];Material m;
   if(!string.IsNullOrEmpty(reuse))m=AssetDatabase.LoadAssetAtPath<Material>(ConRoiRouteV33.Folder+"/"+reuse+".mat");
   else
   {
    string path=Art+"/MapRestoredR11/"+f.name+".mat";m=AssetDatabase.LoadAssetAtPath<Material>(path);
    var shader=AssetDatabase.LoadAssetAtPath<Shader>(Art+"/MapRestoredR11/"+f.name+".shader");if(!shader||ShaderUtil.ShaderHasError(shader))throw new Exception("Restored source shader "+f.name);
    if(!m){m=new Material(shader);AssetDatabase.CreateAsset(m,path);}m.SetFloat("_Cull",2);
   }
   if(!m)throw new Exception("Missing original material "+reuse);
   f.GetComponent<Renderer>().sharedMaterial=m;f.gameObject.layer=3;
   if((bool)item["collision"])f.gameObject.AddComponent<MeshCollider>().sharedMesh=f.sharedMesh;
   map.Add(new JObject{["mesh"]=f.name,["material"]=AssetDatabase.GetAssetPath(m),["sources"]=item["sources"].DeepClone(),["min"]=V(f.GetComponent<Renderer>().bounds.min),["max"]=V(f.GetComponent<Renderer>().bounds.max),["collider_matches_visual"]=!(bool)item["collision"]||f.GetComponent<MeshCollider>().sharedMesh==f.sharedMesh});
  }
  File.WriteAllText(Out+"/restored-scene-map.json",map.ToString());
  var bridge=(GameObject)PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>(Art+"/BridgeRepairR11/BridgeLocalSupport.fbx"));bridge.name="R11_Local_Bridge_Supports";
  var oldBridge=GameObject.Find("OngDao_V35_Photo_Batch");
  foreach(var f in bridge.GetComponentsInChildren<MeshFilter>())
  {
   if(f.name.StartsWith("OD35_Surface_"))
   {
    var old=oldBridge.GetComponentsInChildren<MeshFilter>(true).Single(t=>t.name==f.name);
    // Batches retain FBX source coordinates and import scale. Replace both mesh
    // and transform as a unit, preserving named original collider deltas.
    f.GetComponent<Renderer>().sharedMaterial=old.GetComponent<Renderer>().sharedMaterial;
    old.GetComponent<Renderer>().enabled=false;
   }
   else
   {
    f.GetComponent<Renderer>().sharedMaterial=AssetDatabase.LoadAssetAtPath<Material>("Assets/Art/Environment/OngDaoV35/OD35_Concrete.mat");
    f.gameObject.layer=3;f.gameObject.AddComponent<MeshCollider>().sharedMesh=f.sharedMesh;
   }
  }
  var act=s.GetComponent<DienXuatHanhTrinhV45>();var rig=act.rigs.Single(r=>r.role=="Bao");
  var b=s.gameObject.AddComponent<TuTheBaoBenCauV48>();b.story=s;b.acting=act;b.actor=rig.actor;b.rod=act.rod;
  b.line=s.lake.GetComponentsInChildren<Transform>(true).Single(t=>t.name=="Fishing_Line");
  b.bag=s.lake.GetComponentsInChildren<Transform>(true).Single(t=>t.name=="Bao_Fishing_Bag");b.calendar=s.traces[0];b.reflectionLight=s.completion.lakeReflection.transform;
  var nw=new Vector3(685.9787f,1.14006f,496.6172f);var se=new Vector3(642.2553f,1.14006f,533.6046f);var axis=(se-nw).normalized;
  b.waterDirection=new Vector3(-.6457f,0,-.7636f).normalized;
  var rail=nw+axis*32+b.waterDirection*6.94f;rail.y+=1.02f;b.railContact=rail;
  b.actorRotation=Quaternion.LookRotation(-b.waterDirection)*Quaternion.Inverse(rig.frameInActor);
  float bottom=rig.shoes!=null&&rig.shoes.Length>0?rig.shoes.Min(t=>t.GetComponent<Renderer>().bounds.min.y):rig.actor.GetComponentsInChildren<Renderer>(true).Where(r=>r.enabled&&r.name.Contains("Shoe")).Min(r=>r.bounds.min.y);
  b.shoeBottomInActor=bottom-rig.actor.position.y;
  b.actorPosition=rail-b.waterDirection*.49f;b.actorPosition.y=1.14006f-b.shoeBottomInActor;
  var oldPosition=rig.actor.position;var oldRotation=rig.actor.rotation;
  rig.actor.SetPositionAndRotation(b.actorPosition,b.actorRotation);rig.actorRest=b.actorRotation;
  // Reposition attached bag/calendar/light as an authored scene, not only a mesh.
  b.bag.position=b.actorPosition-axis*.72f-b.waterDirection*.12f+Vector3.up*.18f;
  b.reflectionLight.position=rail+b.waterDirection*14+Vector3.up*.2f;
  var targets=new GameObject("R11_Bao_Rail_Contact_Targets").transform;targets.SetParent(s.lake.transform,true);
  // FBX actor uses a -90 degree basis and 100 import scale. Props used to be
  // children of a scaled shoulder; solve their cylinders in a unit world frame.
  b.rod.SetParent(targets,true);b.line.SetParent(targets,true);
  b.rod.localScale=new Vector3(.018f,1,.018f);b.line.localScale=new Vector3(.0025f,1,.0025f);
  var rotationDelta=b.actorRotation*Quaternion.Inverse(oldRotation);
  foreach(var t in s.lake.GetComponentsInChildren<Transform>(true).Where(t=>t.name=="Enamel_Thermos"||t.name=="Thermos_Lid"))
  {t.position=b.actorPosition+rotationDelta*(t.position-oldPosition);t.rotation=rotationDelta*t.rotation;}
  var calendarParts=s.lake.GetComponentsInChildren<Transform>(true).Where(t=>t.name=="CR_Calendar"||t.name=="CR_Calendar_Frame"||t.name=="CR_Calendar_Stand"||t.name=="Unturned_Calendar_Edges").ToArray();
  var stand=calendarParts.Single(t=>t.name=="CR_Calendar_Stand").GetComponent<Renderer>();
  var calendarAssembly=new GameObject("R11_Calendar_Complete_Assembly").transform;calendarAssembly.SetParent(targets,true);calendarAssembly.position=new Vector3(stand.bounds.center.x,stand.bounds.min.y,stand.bounds.center.z);
  foreach(var t in calendarParts)t.SetParent(calendarAssembly,true);
  calendarAssembly.rotation=rotationDelta*Quaternion.Euler(0,180,0);calendarAssembly.position=b.actorPosition+axis*1.4f-b.waterDirection*.65f+Vector3.up*.002f;
  Transform Target(string n,Vector3 p){var t=new GameObject(n).transform;t.SetParent(targets,true);t.position=p;return t;}
  // Rail radius .025 m + sleeve radius ~.053 m + .005 m contact clearance.
  var contactRight=Vector3.Cross(Vector3.up,-b.waterDirection).normalized;
  b.leftElbowTarget=Target("LeftForearmOnRail",rail-contactRight*.22f+Vector3.up*.083f-b.waterDirection*.08f);
  b.rightElbowTarget=Target("RightForearmOnRail",rail+contactRight*.20f+Vector3.up*.083f-b.waterDirection*.08f);
  b.leftHandTarget=Target("LeftHandOverWater",rail-contactRight*.22f+b.waterDirection*.23f+Vector3.up*.095f);
  b.rightHandTarget=Target("RightHandRodGrip",rail+contactRight*.20f+b.waterDirection*.23f+Vector3.up*.10f);
  // Main bridge routing starts at the existing lake path spur, crosses the
  // sidewalk and rejoins the north approach, then retains original map alignment.
  var oldRoute=JObject.Parse(File.ReadAllText("Assets/Resources/VerificationV36/route.json"))["route"].Select(P).ToArray();
  var record=JObject.Parse(File.ReadAllText("Assets/Art/Environment/OngDaoV35/bridge_record.json"));
  var route=new List<Vector3>();route.AddRange(oldRoute.Take(63));
  route.AddRange(record["connections"][1].Select(P));
  route.AddRange(record["paths"]["sidewalk_-1"].Select(P).Reverse().Skip(1));
  route.AddRange(record["connections"][0].Select(P).Reverse());
  route.AddRange(oldRoute.Skip(122));
  Physics.SyncTransforms();
  // Follow existing authored road to the restored frontage, no fixture surface.
  foreach(var end in new[]{new Vector3(790,0,255),new Vector3(793,0,225),new Vector3(794,0,195),new Vector3(795,0,170)})
  {
   var start=route.Last();int count=Mathf.CeilToInt(Vector2.Distance(new Vector2(start.x,start.z),new Vector2(end.x,end.z))/2f);
   for(int i=1;i<=count;i++)
   {
    var p=Vector3.Lerp(start,end,i/(float)count);var hits=Physics.RaycastAll(p+Vector3.up*90,Vector3.down,150,8,QueryTriggerInteraction.Ignore).Where(h=>h.normal.y>.7f&&h.point.y<24).OrderByDescending(h=>h.point.y).ToArray();
    if(hits.Length==0)throw new Exception("No road support at "+p);p.y=hits[0].point.y+.04f;route.Add(p);
   }
  }
  // Lake exit after the bridge rather than before the moved encounter.
  var wake=s.lake.GetComponentsInChildren<TuongTacHoCho>(true).First(i=>i.kind==TuongTacHoCho.Kind.Wake&&i.GetComponent<Collider>()).transform.parent;
  var oldWake=wake.position;var targetWake=oldRoute[123]+Vector3.right*1.2f;wake.position=targetWake;
  foreach(var text in s.lake.GetComponentsInChildren<TextMesh>(true).Where(t=>t.text.Contains("TỈNH DẬY")))text.transform.parent.gameObject.SetActive(false);
  var guide=s.GetComponent<HuongDanHanhTrinhV48>();guide.bridgeMarketLayout=true;
  var oldSigns=GameObject.Find("V48_Authored_Wayfinding");if(oldSigns)oldSigns.SetActive(false);
  var signs=new GameObject("R11_Route_Wayfinding").transform;
  void Sign(string label,Vector3 p,Vector3 face)
  {
   var supports=Physics.RaycastAll(p+Vector3.up*6,Vector3.down,16,8,QueryTriggerInteraction.Ignore).Where(h=>h.normal.y>.7f&&h.point.y<=p.y+.5f).OrderByDescending(h=>h.point.y).ToArray();if(supports.Length>0)p.y=supports[0].point.y;
   var t=new GameObject(label).transform;t.SetParent(signs,true);t.position=p;t.rotation=Quaternion.LookRotation(-face.normalized);
   var post=GameObject.CreatePrimitive(PrimitiveType.Cube);post.name="Sign_Post";post.transform.SetParent(t,false);post.transform.localPosition=Vector3.up*.65f;post.transform.localScale=new Vector3(.065f,1.3f,.065f);post.GetComponent<Renderer>().sharedMaterial=AssetDatabase.LoadAssetAtPath<Material>(Art+"/Sign_Painted_Green.mat");UnityEngine.Object.DestroyImmediate(post.GetComponent<Collider>());
   var board=GameObject.CreatePrimitive(PrimitiveType.Cube);board.name="Board";board.transform.SetParent(t,false);board.transform.localPosition=Vector3.up*1.35f;board.transform.localScale=new Vector3(1.9f,.55f,.06f);board.GetComponent<Renderer>().sharedMaterial=AssetDatabase.LoadAssetAtPath<Material>(Art+"/Sign_Painted_Green.mat");UnityEngine.Object.DestroyImmediate(board.GetComponent<Collider>());
   var tm=new GameObject("Lettering").AddComponent<TextMesh>();tm.transform.SetParent(t,false);tm.transform.localPosition=new Vector3(0,1.35f,-.035f);tm.text=label;tm.font=Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");tm.fontSize=56;tm.characterSize=.027f;tm.anchor=TextAnchor.MiddleCenter;tm.alignment=TextAlignment.Center;tm.GetComponent<Renderer>().sharedMaterial=tm.font.material;
   var back=UnityEngine.Object.Instantiate(tm,t);back.name="Lettering_Reverse_Face";back.transform.localPosition=new Vector3(0,1.35f,.035f);back.transform.localRotation=Quaternion.Euler(0,180,0);back.text=label.Replace("←","").Replace("→","").Replace("↑","");
   var faces=t.gameObject.AddComponent<MatBienHieuV48>();faces.eye=s.player.GetComponentInChildren<Camera>();faces.front=tm.GetComponent<Renderer>();faces.back=back.GetComponent<Renderer>();
  }
  Sign("CẦU ÔNG ĐẠO →\nÔng Bảo bên lan can",oldRoute[59]+Vector3.left*1.7f,(oldRoute[57]-oldRoute[59]).normalized);
  Sign("← CẦU ÔNG ĐẠO\nLỐI VEN HỒ",oldRoute[124]+Vector3.right*1.8f,(oldRoute[127]-oldRoute[124]).normalized);
  Sign("VÒNG XOAY / CHỢ →",nw-axis*4-b.waterDirection*5,(se-nw).normalized);
  foreach(int i in new[]{140,165,203,247})Sign("CHỢ ĐÀ LẠT ↑\nTheo phố tới mặt chợ",oldRoute[i]+Vector3.right*1.8f,(oldRoute[Math.Max(0,i-2)]-oldRoute[i]).normalized);
  Sign("MẶT CHỢ ↑\nCó thể đi tiếp rồi quay lại",new Vector3(790,7.3f,267),Vector3.forward);
  Sign("TỈNH DẬY\nE · Về phòng trọ",targetWake+Vector3.left*1.8f,Vector3.back);
  // Close only side-street mouths leading out of the authored playable area.
  // Existing source architecture remains visible behind each modest barrier.
  var continuity=s.GetComponent<CanhQuanChoV48>();continuity.scenery=continuity.scenery.Concat(new[]{restored,bridge,signs.gameObject}).ToArray();
  Directory.CreateDirectory("Assets/Resources/VerificationV48R11");
  File.WriteAllText("Assets/Resources/VerificationV48R11/route.json",new JObject{["route"]=new JArray(route.Select(p=>(JToken)V(p))),["purpose"]="Reference for measured audit only; primary acceptance walk must select branches using in-game signs/objectives.",["bao"]=V(b.actorPosition),["bao_approach"]=V(b.actorPosition-b.waterDirection*1.5f),["lake_wake"]=V(targetWake),["main_market_front"]=V(route.Last())}.ToString());
  File.WriteAllText(Out+"/bao-layout.json",new JObject{["old_position"]=V(oldPosition),["position"]=V(b.actorPosition),["rotation"]=V(b.actorRotation.eulerAngles),["rail_contact"]=V(rail),["water_direction"]=V(b.waterDirection),["shoe_bottom_offset"]=b.shoeBottomInActor,["lake_wake_before"]=V(oldWake),["lake_wake_after"]=V(targetWake),["save_migration"]="Preserve story/player/puppet progress; discard incompatible r10 absolute actor poses, retain new authored bridge layout."}.ToString());
  AssetDatabase.Refresh();EditorSceneManager.SaveScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene(),Scene);AssetDatabase.SaveAssets();
  var renderers=UnityEngine.Object.FindObjectsByType<Renderer>(FindObjectsInactive.Include,FindObjectsSortMode.None);
  File.WriteAllText(Out+"/r11-scene-renderers.json",new JArray(renderers.Select(r=>new JObject{["name"]=r.name,["root"]=r.transform.root.name,["enabled"]=r.enabled,["active"]=r.gameObject.activeInHierarchy,["min"]=V(r.bounds.min),["max"]=V(r.bounds.max),["mesh"]=r.GetComponent<MeshFilter>()?AssetDatabase.GetAssetPath(r.GetComponent<MeshFilter>().sharedMesh):"",["materials"]=new JArray(r.sharedMaterials.Select(m=>m?AssetDatabase.GetAssetPath(m):"MISSING"))})).ToString());
  File.WriteAllText(Out+"/scene-build-dependencies.json",new JArray(AssetDatabase.GetDependencies(Scene,true)).ToString());
  Debug.Log("R11_ASSEMBLED");
 }
 public static void Build()
 {
  Assemble();string version=PlayerSettings.bundleVersion;
  try{PlayerSettings.bundleVersion="0.48.11";var report=BuildPipeline.BuildPlayer(new[]{Scene},"Builds/Windows_v48_r11/ConRoi.exe",BuildTarget.StandaloneWindows64,BuildOptions.Development);File.WriteAllText(Out+"/build.json",new JObject{["result"]=report.summary.result.ToString(),["errors"]=report.summary.totalErrors,["warnings"]=report.summary.totalWarnings}.ToString());if(report.summary.result!=UnityEditor.Build.Reporting.BuildResult.Succeeded)throw new Exception("R11 build failed");}
  finally{PlayerSettings.bundleVersion=version;AssetDatabase.SaveAssets();}
 }
}
