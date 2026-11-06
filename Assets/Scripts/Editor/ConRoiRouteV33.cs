using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class ConRoiRouteV33
{
 public const string Folder="Assets/Art/Environment/LakeGateMarketV33";
 public const string ScenePath="Assets/Scenes/LakeGateMarket_v33.unity";
 const string Review="Docs/Reviews/lake_gate_market_current";
 static string PhysicsSnapshot(IEnumerable<MeshCollider> colliders)=>string.Join("\n",colliders.OrderBy(c=>c.name).Select(c=>c.name+":"+AssetDatabase.GetAssetPath(c.sharedMesh)+":"+c.sharedMesh.GetEntityId()+":"+c.transform.localToWorldMatrix.ToString("F6")));
 public static void Import()
 {
  if(EditorApplication.isPlayingOrWillChangePlaymode)throw new Exception("Stop Play first");
  AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
  var gateImporter=(ModelImporter)AssetImporter.GetAtPath(Folder+"/GatePanels.fbx");gateImporter.isReadable=true;gateImporter.addCollider=false;gateImporter.materialImportMode=ModelImporterMaterialImportMode.None;gateImporter.SaveAndReimport();
  var cavityImporter=(ModelImporter)AssetImporter.GetAtPath(Folder+"/CavitySurfaces.fbx");cavityImporter.isReadable=true;cavityImporter.addCollider=false;cavityImporter.materialImportMode=ModelImporterMaterialImportMode.None;cavityImporter.SaveAndReimport();
  string fbx=Folder+"/RouteSurfaces.fbx";var importer=(ModelImporter)AssetImporter.GetAtPath(fbx);importer.globalScale=1;importer.addCollider=false;importer.materialImportMode=ModelImporterMaterialImportMode.None;importer.isReadable=true;importer.SaveAndReimport();
  var textureImporter=(TextureImporter)AssetImporter.GetAtPath(Folder+"/GateWayfinding.png");textureImporter.sRGBTexture=true;textureImporter.mipmapEnabled=true;textureImporter.maxTextureSize=2048;textureImporter.textureCompression=TextureImporterCompression.Uncompressed;textureImporter.anisoLevel=8;textureImporter.wrapMode=TextureWrapMode.Clamp;textureImporter.SaveAndReimport();
  var data=JObject.Parse(File.ReadAllText(Folder+"/surfaces.json"));
  var model=AssetDatabase.LoadAssetAtPath<GameObject>(fbx);var instance=(GameObject)PrefabUtility.InstantiatePrefab(model);
  try
  {
   foreach(var mf in instance.GetComponentsInChildren<MeshFilter>())
   {
    if(mf.name=="Surface_041")
    {
     var gate=AssetDatabase.LoadAssetAtPath<GameObject>(Folder+"/GatePanels.fbx");
     if(gate)mf.sharedMesh=gate.GetComponentInChildren<MeshFilter>().sharedMesh;
    }
    if(mf.name=="Surface_061"||mf.name=="Surface_063")mf.sharedMesh=AssetDatabase.LoadAssetAtPath<GameObject>(Folder+"/CavitySurfaces.fbx").GetComponentsInChildren<MeshFilter>().Single(f=>f.name==mf.name).sharedMesh;
    var shader=AssetDatabase.LoadAssetAtPath<Shader>(Folder+"/"+mf.name+".shader");
    if(!shader||ShaderUtil.ShaderHasError(shader))throw new Exception("Shader error "+mf.name);
    string path=Folder+"/"+mf.name+".mat";var mat=AssetDatabase.LoadAssetAtPath<Material>(path);
    if(!mat){mat=new Material(shader);AssetDatabase.CreateAsset(mat,path);}mat.shader=shader;
    var item=data["meshes"].Single(m=>(string)m["mesh"]==mf.name);
    if(item["textures"]["_RouteMap"]!=null)mat.SetTexture("_RouteMap",AssetDatabase.LoadAssetAtPath<Texture2D>(Folder+"/GateWayfinding.png"));
    mat.SetFloat("_Cull",2);EditorUtility.SetDirty(mat);mf.GetComponent<Renderer>().sharedMaterial=mat;
    mf.gameObject.layer=0;GameObjectUtility.SetStaticEditorFlags(mf.gameObject,StaticEditorFlags.OccluderStatic|StaticEditorFlags.OccludeeStatic);
   }
   instance.name="LakeGateMarket_V33_MATERIAL_SURFACES";PrefabUtility.SaveAsPrefabAsset(instance,Folder+"/RouteSurfaces.prefab");
  }
  finally{UnityEngine.Object.DestroyImmediate(instance);}
  EditorSceneManager.OpenScene(ConRoiRouteV32.ScenePath,OpenSceneMode.Single);
  var colliders=UnityEngine.Object.FindObjectsByType<MeshCollider>(FindObjectsSortMode.None);string before=PhysicsSnapshot(colliders);
  foreach(var r in colliders.Select(c=>c.GetComponent<Renderer>()).Where(r=>r))r.enabled=false;
  // Water/foliage have no collider, so hide the complete previous visual layer.
  var old=colliders.Length>0?colliders[0].transform.root.gameObject:null;if(!old)throw new Exception("v32 environment missing");
  foreach(var renderer in old.GetComponentsInChildren<Renderer>())renderer.enabled=false;
  PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>(Folder+"/RouteSurfaces.prefab"));
  new GameObject("Regional_Surface_Lighting").AddComponent<AnhSangBeMatTheoKhu>();
  if(PhysicsSnapshot(colliders)!=before)throw new Exception("Physics references or transforms changed");
  EditorSceneManager.SaveScene(SceneManager.GetActiveScene(),ScenePath);AssetDatabase.SaveAssets();Inspect();
 }
 public static void Inspect()
 {
  var scene=SceneManager.GetActiveScene();if(scene.path!=ScenePath)throw new Exception("Wrong v33 scene");
  var roots=scene.GetRootGameObjects();var all=roots.SelectMany(r=>r.GetComponentsInChildren<Transform>(true)).ToArray();
  var renderers=all.SelectMany(t=>t.GetComponents<MeshRenderer>()).Where(r=>r.enabled).ToArray();
  var colliders=all.SelectMany(t=>t.GetComponents<MeshCollider>()).ToArray();var player=UnityEngine.Object.FindAnyObjectByType<DieuKhienDiChuyen>();var cc=player.controller;
  int missing=all.Sum(t=>GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(t.gameObject));
  var errors=renderers.SelectMany(r=>r.sharedMaterials).Where(m=>!m||!m.shader||ShaderUtil.ShaderHasError(m.shader)).Count();
  var deps=AssetDatabase.GetDependencies(ScenePath,true);var textures=deps.Where(p=>AssetDatabase.LoadAssetAtPath<Texture>(p)).ToArray();
  bool physicsOriginal=colliders.Length==143&&colliders.All(c=>c.enabled&&AssetDatabase.GetAssetPath(c.sharedMesh)==ConRoiRouteV32.Folder+"/LakeGateMarketContext.fbx");
  bool ready=missing==0&&errors==0&&physicsOriginal&&cc.height==1.8f&&cc.radius==.3f&&cc.stepOffset==.3f&&cc.slopeLimit==45&&cc.skinWidth==.03f&&textures.Contains(Folder+"/GateWayfinding.png");
  File.WriteAllText(Review+"/v33_scene_preflight.json",new JObject{["utc"]=DateTime.UtcNow.ToString("o"),["scene"]=ScenePath,["ready"]=ready,["missing_scripts"]=missing,["shader_errors"]=errors,["visible_meshes"]=renderers.Length,["unchanged_original_v32_colliders"]=physicsOriginal,["mesh_colliders"]=colliders.Length,["packaged_textures"]=new JArray(textures),["all_asset_dependencies_project_relative"]=deps.All(p=>p.StartsWith("Assets/")||p.StartsWith("Packages/")),["player_height"]=cc.height,["player_radius"]=cc.radius,["step"]=cc.stepOffset,["slope"]=cc.slopeLimit}.ToString());
  if(!ready)throw new Exception("v33 preflight failed");
 }
}
