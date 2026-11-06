using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.Build.Reporting;
using UnityEngine;

public static class ConRoiV48R15
{
 const string Out="Docs/Reviews/journey_v48/r15",Art="Assets/Art/Environment/JourneyV48/";
 public static void Build()
 {
  Directory.CreateDirectory(Out);AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
  EditorSceneManager.OpenScene("Assets/Scenes/JourneyRepair_v48_r14.unity");
  var story=UnityEngine.Object.FindAnyObjectByType<DieuPhoiHanhTrinh>();var continuity=story.GetComponent<CanhQuanChoV48>();
  var mapping=new JArray();
  GameObject Model(string folder,string file,string rootName)
  {
   var path=Art+folder+"/"+file;var importer=(ModelImporter)AssetImporter.GetAtPath(path);importer.isReadable=true;importer.addCollider=false;importer.materialImportMode=ModelImporterMaterialImportMode.None;importer.SaveAndReimport();
   var root=(GameObject)PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>(path));root.name=rootName;return root;
  }
  var foundations=Model("PlanterRepairR15","PlanterBases.fbx","R15_Local_Plinth_Foundations");
  foreach(var f in foundations.GetComponentsInChildren<MeshFilter>())
  {
   f.GetComponent<Renderer>().sharedMaterial=AssetDatabase.LoadAssetAtPath<Material>("Assets/Art/Environment/OngDaoV35/OD35_Concrete.mat");
   f.gameObject.layer=3;f.gameObject.AddComponent<MeshCollider>().sharedMesh=f.sharedMesh;
   if(f.name.StartsWith("R15_Plinth_Collision"))f.GetComponent<Renderer>().enabled=false;
   mapping.Add(new JObject{["mesh"]=f.name,["asset"]=AssetDatabase.GetAssetPath(f.sharedMesh),["collider_matches"]=f.sharedMesh==f.GetComponent<MeshCollider>().sharedMesh,["bounds"]=f.GetComponent<Renderer>().bounds.ToString("F4")});
  }
  var completed=Model("ModelsCompletedR15","CompletedModels.fbx","R15_Completed_Source_Models");
  var manifest=JObject.Parse(File.ReadAllText(Art+"ModelsCompletedR15/manifest.json"));
  foreach(var f in completed.GetComponentsInChildren<MeshFilter>())
  {
   var row=manifest["meshes"].Single(m=>(string)m["mesh"]==f.name);var reuse=(string)row["reuse_material"];Material material;
   if(!string.IsNullOrEmpty(reuse))material=AssetDatabase.LoadAssetAtPath<Material>(ConRoiRouteV33.Folder+"/"+reuse+".mat");
   else
   {
    var shader=AssetDatabase.LoadAssetAtPath<Shader>(Art+"ModelsCompletedR15/"+f.name+".shader");if(!shader||ShaderUtil.ShaderHasError(shader))throw new Exception("Invalid source material "+f.name);
    string materialPath=Art+"ModelsCompletedR15/"+f.name+".mat";material=AssetDatabase.LoadAssetAtPath<Material>(materialPath);if(!material){material=new Material(shader);material.SetFloat("_Cull",2);AssetDatabase.CreateAsset(material,materialPath);}
   }
   if(!material)throw new Exception("Missing source material "+reuse);f.GetComponent<Renderer>().sharedMaterial=material;
   f.gameObject.layer=3;if((bool)row["collision"])f.gameObject.AddComponent<MeshCollider>().sharedMesh=f.sharedMesh;
   mapping.Add(new JObject{["mesh"]=f.name,["asset"]=AssetDatabase.GetAssetPath(f.sharedMesh),["sources"]=row["sources"].DeepClone(),["collider_matches"]=!(bool)row["collision"]||f.sharedMesh==f.GetComponent<MeshCollider>().sharedMesh,["bounds"]=f.GetComponent<Renderer>().bounds.ToString("F4")});
  }
  continuity.scenery=continuity.scenery.Concat(new[]{foundations,completed}).ToArray();
  File.WriteAllText(Out+"/added-scene-mapping.json",mapping.ToString());
  var bao=story.GetComponent<TuTheBaoBenCauV48>();
  File.WriteAllText(Out+"/bao-shoe-source.json",new JArray(bao.actor.GetComponentsInChildren<MeshFilter>(true).Where(f=>f.name.Contains("Shoe")).Select(f=>new JObject{["name"]=f.name,["mesh"]=AssetDatabase.GetAssetPath(f.sharedMesh),["readable"]=f.sharedMesh.isReadable,["enabled"]=f.GetComponent<Renderer>().enabled})).ToString());
  const string scene="Assets/Scenes/JourneyRepair_v48_r15.unity";EditorSceneManager.SaveScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene(),scene);AssetDatabase.SaveAssets();
  var version=PlayerSettings.bundleVersion;
  try
  {
   PlayerSettings.bundleVersion="0.48.15";
   var report=BuildPipeline.BuildPlayer(new[]{scene},"Builds/Windows_v48_r15/ConRoi.exe",BuildTarget.StandaloneWindows64,BuildOptions.Development);
   File.WriteAllText(Out+"/build.json",new JObject{["result"]=report.summary.result.ToString(),["errors"]=report.summary.totalErrors,["warnings"]=report.summary.totalWarnings}.ToString());
   File.WriteAllText(Out+"/build-assets.json",new JObject{["assets"]=new JArray(report.packedAssets.SelectMany(p=>p.contents).Select(c=>c.sourceAssetPath).Distinct().OrderBy(p=>p))}.ToString());
   if(report.summary.result!=BuildResult.Succeeded)throw new Exception("r15 build failed");
  }
  finally{PlayerSettings.bundleVersion=version;AssetDatabase.SaveAssets();}
 }
}
