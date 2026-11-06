using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

// Isolated asset preparation only. Does not open production scenes or change build settings.
public static class Tooling3DTrial
{
    public static void Prepare()
    {
        const string folder = "Assets/Tooling3DTrial";
        const string report = "Docs/Tooling3D/trial";
        AssetDatabase.Refresh();
        var importer = AssetImporter.GetAtPath(folder + "/FlowerClump.fbx") as ModelImporter;
        if (!importer) throw new Exception("Trial FBX not imported");
        importer.globalScale = 1; importer.useFileScale = true;
        importer.importAnimation = false; importer.addCollider = false;
        importer.materialImportMode = ModelImporterMaterialImportMode.ImportStandard;
        importer.SaveAndReimport();
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        var asset = AssetDatabase.LoadAssetAtPath<GameObject>(folder + "/FlowerClump.fbx");
        var flower = (GameObject)PrefabUtility.InstantiatePrefab(asset, scene);
        flower.transform.position = new Vector3(0, .65f, 0);
        var colors = new[] { new Color(.10f,.23f,.065f), new Color(.68f,.62f,.34f), new Color(.42f,.25f,.035f) };
        var names = new[] { "Trial_Leaf", "Trial_Petal", "Trial_Centre" };
        var mats = names.Select((n,i) => {
            var p = folder + "/" + n + ".mat";
            var m = AssetDatabase.LoadAssetAtPath<Material>(p);
            if (!m) { m = new Material(Shader.Find("Universal Render Pipeline/Lit")); AssetDatabase.CreateAsset(m,p); }
            m.SetColor("_BaseColor",colors[i]); m.SetFloat("_Smoothness",.15f); EditorUtility.SetDirty(m); return m;
        }).ToArray();
        foreach (var r in flower.GetComponentsInChildren<Renderer>())
            r.sharedMaterials = r.sharedMaterials.Select(m => { int i=Array.FindIndex(names,n=>m && m.name.StartsWith(n)); return mats[i>=0?i:0]; }).ToArray();
        var bounds = flower.GetComponentsInChildren<Renderer>().First().bounds;
        foreach(var r in flower.GetComponentsInChildren<Renderer>()) bounds.Encapsulate(r.bounds);
        if (bounds.size.y < .15f || bounds.size.y > .30f) throw new Exception("FBX metre scale failed: " + bounds.size);
        var ground = GameObject.CreatePrimitive(PrimitiveType.Cube); ground.name="TrialPlanterScaleProxy";
        ground.transform.position=new Vector3(0,.30f,0);ground.transform.localScale=new Vector3(.7f,.6f,.7f);
        var light = new GameObject("TrialSun").AddComponent<Light>();light.type=LightType.Directional;light.intensity=2;light.transform.rotation=Quaternion.Euler(45,-30,0);
        RenderSettings.ambientMode=AmbientMode.Flat;RenderSettings.ambientLight=new Color(.5f,.5f,.5f);
        var camera=new GameObject("TrialPlayerEye165cm").AddComponent<Camera>();
        camera.transform.position=new Vector3(0,1.65f,-1.5f);
        camera.transform.LookAt(new Vector3(0,.72f,0));camera.fieldOfView=60;camera.nearClipPlane=.03f;
        camera.clearFlags=CameraClearFlags.SolidColor;camera.backgroundColor=new Color(.16f,.19f,.21f);
        camera.gameObject.AddComponent<UniversalAdditionalCameraData>();
        var rt=RenderTexture.GetTemporary(960,720,24);var tex=new Texture2D(960,720,TextureFormat.RGB24,false);
        try {
            for(int warm=0;warm<3;warm++) RenderPipeline.SubmitRenderRequest(camera,new UniversalRenderPipeline.SingleCameraRequest{destination=rt});
            var old=RenderTexture.active;RenderTexture.active=rt;tex.ReadPixels(new Rect(0,0,960,720),0,0);tex.Apply();RenderTexture.active=old;
            File.WriteAllBytes(report+"/unity-player-eye.png",tex.EncodeToPNG());
        } finally {RenderTexture.ReleaseTemporary(rt);UnityEngine.Object.DestroyImmediate(tex);}
        EditorSceneManager.SaveScene(scene,folder+"/FlowerReview.unity");AssetDatabase.SaveAssets();
        File.WriteAllText(report+"/unity-verification.json",new JObject {
            ["status"]="PASS_IMPORT_SCALE_RENDER_SCENE_SAVE",["unity"]=Application.unityVersion,
            ["triangles"]=flower.GetComponentsInChildren<MeshFilter>().Sum(m=>m.sharedMesh.triangles.Length/3),
            ["bounds_m"]=new JArray(bounds.size.x,bounds.size.y,bounds.size.z),
            ["camera_height_m"]=1.65,["colliders_on_flower"]=flower.GetComponentsInChildren<Collider>().Length,
            ["scope"]="Isolated scale proxy; editor player-eye render, not production gameplay acceptance"
        }.ToString());
        Debug.Log("TOOLING3D_UNITY_TRIAL_PASS");
    }
}
