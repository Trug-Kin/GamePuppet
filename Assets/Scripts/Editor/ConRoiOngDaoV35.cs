using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
public static class ConRoiOngDaoV35
{
 public const string Folder="Assets/Art/Environment/OngDaoV35";
 public const string ScenePath="Assets/Scenes/LakeMarketStory_v35.unity";
 public static void Assemble()
 {
  if(EditorApplication.isPlayingOrWillChangePlaymode)throw new Exception("Stop Play first");
  AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
  var importer=(ModelImporter)AssetImporter.GetAtPath(Folder+"/OngDaoDetails.fbx");importer.isReadable=true;importer.addCollider=false;importer.materialImportMode=ModelImporterMaterialImportMode.None;importer.SaveAndReimport();
  EditorSceneManager.OpenScene(ConRoiLakeMarketV34.ScenePath);
  var old=UnityEngine.Object.FindObjectsByType<MeshCollider>(FindObjectsSortMode.None).Where(c=>AssetDatabase.GetAssetPath(c.sharedMesh).Contains("LakeGateMarketV32")).ToArray();
  string Signature()=>string.Join("\n",old.OrderBy(c=>c.name).Select(c=>c.name+":"+AssetDatabase.GetAssetPath(c.sharedMesh)+":"+c.sharedMesh.GetEntityId()+":"+c.transform.localToWorldMatrix.ToString("F6")));
  string before=Signature();var model=(GameObject)PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>(Folder+"/OngDaoDetails.fbx"));model.name="OngDao_V35_Photo_Batch";
  var data=JObject.Parse(File.ReadAllText(Folder+"/bridge_record.json"));
  foreach(var mf in model.GetComponentsInChildren<MeshFilter>())
  {
   if(mf.name.StartsWith("OD35_COLLIDER_")){mf.GetComponent<Renderer>().enabled=false;mf.gameObject.layer=3;var c=mf.gameObject.AddComponent<MeshCollider>();c.sharedMesh=mf.sharedMesh;continue;}
   string index=mf.name.Substring("OD35_Surface_".Length);var info=data["materials"][index];string path=Folder+"/"+(string)info["name"]+".mat";var m=AssetDatabase.LoadAssetAtPath<Material>(path);
   if(!m){m=new Material(Shader.Find("Universal Render Pipeline/Lit"));AssetDatabase.CreateAsset(m,path);}m.SetColor("_BaseColor",new Color((float)info["color"][0],(float)info["color"][1],(float)info["color"][2]));m.SetFloat("_Smoothness",1-(float)info["roughness"]);m.SetFloat("_Metallic",(float)info["metallic"]);mf.GetComponent<Renderer>().sharedMaterial=m;
  }
  if(Signature()!=before||old.Length!=143)throw new Exception("Existing environment physics changed");
  EditorSceneManager.SaveScene(SceneManager.GetActiveScene(),ScenePath);AssetDatabase.SaveAssets();
  File.WriteAllText("Docs/Reviews/lake_gate_market_current/v35_scene_preflight.json",new JObject{["scene"]=ScenePath,["original_143_colliders_unchanged"]=true,["collider_delta"]=model.GetComponentsInChildren<MeshCollider>().Length,["missing_scripts"]=SceneManager.GetActiveScene().GetRootGameObjects().Sum(g=>g.GetComponentsInChildren<Transform>(true).Sum(t=>GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(t.gameObject))),["project_relative_dependencies"]=AssetDatabase.GetDependencies(ScenePath,true).All(p=>p.StartsWith("Assets/")||p.StartsWith("Packages/")),["shader_errors"]=model.GetComponentsInChildren<Renderer>().Where(r=>r.enabled).SelectMany(r=>r.sharedMaterials).Count(m=>!m||ShaderUtil.ShaderHasError(m.shader))}.ToString());
 }
}
