using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class ConRoiRouteV32
{
    public const string Folder="Assets/Art/Environment/LakeGateMarketV32";
    public const string ScenePath="Assets/Scenes/LakeGateMarket_v32.unity";
    const string Review="Docs/Reviews/lake_gate_market_current";
    public static void ImportAndAssemble()
    {
        if(EditorApplication.isPlayingOrWillChangePlaymode)throw new Exception("Stop Play before regional import");
        string path=Folder+"/LakeGateMarketContext.fbx";
        AssetDatabase.ImportAsset(path,ImportAssetOptions.ForceSynchronousImport);
        var importer=(ModelImporter)AssetImporter.GetAtPath(path);
        importer.globalScale=1;importer.addCollider=false;importer.materialImportMode=ModelImporterMaterialImportMode.None;
        importer.SaveAndReimport();
        var root=(GameObject)PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>(path));
        var data=JObject.Parse(File.ReadAllText(Folder+"/context.json"));var checks=new JArray();float maxError=0;int colliderCount=0;
        if(!data["meshes"].Any(m=>m["sources"].Any(n=>(string)n=="V32_Walk_Continuous_2p4m_VisibleAndCollider")))throw new Exception("Authored V32 paving missing from export");
        try
        {
            AssetDatabase.StartAssetEditing();
            try
            {
                foreach(var mf in root.GetComponentsInChildren<MeshFilter>(true))
                {
                    var item=data["meshes"].Single(m=>(string)m["name"]==mf.name);
                    Vector3 P(JToken p)=>new Vector3((float)p[0],(float)p[1],(float)p[2]);
                    var vertices=mf.sharedMesh.vertices.Select(v=>mf.transform.TransformPoint(v)).ToArray();
                    var bounds=new Bounds(vertices[0],Vector3.zero);foreach(var p in vertices)bounds.Encapsulate(p);
                    float error=Mathf.Max((bounds.min-P(item["unity_bounds"][0])).magnitude,(bounds.max-P(item["unity_bounds"][1])).magnitude);maxError=Mathf.Max(maxError,error);
                    string materialPath=Folder+"/"+mf.name+".mat";
                    var mat=AssetDatabase.LoadAssetAtPath<Material>(materialPath);
                    if(!mat){mat=new Material(Shader.Find("Universal Render Pipeline/Lit"));AssetDatabase.CreateAsset(mat,materialPath);}
                    var c=item["color"];mat.color=new Color((float)c[0],(float)c[1],(float)c[2],1);mat.SetFloat("_Smoothness",.12f);
                    string source=((string)item["source_material"]).ToLowerInvariant();
                    if(source=="lake_flat")mat.SetFloat("_Smoothness",.8f);
                    if(source.Contains("warmglass")){mat.EnableKeyword("_EMISSION");mat.SetColor("_EmissionColor",new Color(.35f,.2f,.07f));}
                    EditorUtility.SetDirty(mat);mf.GetComponent<Renderer>().sharedMaterial=mat;
                    mf.gameObject.layer=3;
                    // Geometry already merged by material: static batching adds no
                    // useful material batching and duplicates these large meshes.
                    GameObjectUtility.SetStaticEditorFlags(mf.gameObject,StaticEditorFlags.OccluderStatic|StaticEditorFlags.OccludeeStatic);
                    if((bool)item["collision"]){mf.gameObject.AddComponent<MeshCollider>().sharedMesh=mf.sharedMesh;colliderCount++;}
                    checks.Add(new JObject{["mesh"]=mf.name,["bounds_error_m"]=error,["collision"]=item["collision"],["same_visible_and_collider_mesh"]=!(bool)item["collision"]||mf.GetComponent<MeshCollider>().sharedMesh==mf.sharedMesh});
                }
            }
            finally{AssetDatabase.StopAssetEditing();}
            if(maxError>.003f)throw new Exception("V32 FBX coordinate mismatch "+maxError);
            root.name="LakeGateMarket_V32_FULL_ENVIRONMENT";
            PrefabUtility.SaveAsPrefabAsset(root,Folder+"/LakeGateMarketV32.prefab");AssetDatabase.SaveAssets();
            File.WriteAllText(Review+"/v32_unity_import.json",new JObject{["utc"]=DateTime.UtcNow.ToString("o"),["status"]="IMPORTED_ACTUAL_MESHES_NOT_TRAVERSAL_VERDICT",["mesh_count"]=checks.Count,["static_mesh_colliders"]=colliderCount,["max_bounds_error_m"]=maxError,["checks"]=checks}.ToString());
        }
        finally{UnityEngine.Object.DestroyImmediate(root);}
        Assemble();
    }
    public static void Assemble()
    {
        if(EditorApplication.isPlayingOrWillChangePlaymode)throw new Exception("Stop Play first");
        EditorSceneManager.OpenScene("Assets/WRouteC3Test_v30_20261016/DaLatMovementTest.unity",OpenSceneMode.Single);
        var source=UnityEngine.Object.FindAnyObjectByType<DieuKhienDiChuyen>(FindObjectsInactive.Include);
        var player=UnityEngine.Object.Instantiate(source.gameObject);player.name="Minh_RegionalWalk_180cm";
        // The shared controller is reusable; dedicated test-only components are not.
        foreach(var profile in player.GetComponentsInChildren<DaLatMovementTestProfile>(true))UnityEngine.Object.DestroyImmediate(profile);
        foreach(var recorder in player.GetComponentsInChildren<DaLatManualMovementRecorder>(true))UnityEngine.Object.DestroyImmediate(recorder);
        var previous=SceneManager.GetActiveScene();
        var scene=EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Additive);
        SceneManager.MoveGameObjectToScene(player,scene);SceneManager.SetActiveScene(scene);EditorSceneManager.CloseScene(previous,true);
        PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>(Folder+"/LakeGateMarketV32.prefab"),scene);
        var movement=player.GetComponent<DieuKhienDiChuyen>();movement.enabled=true;movement.controller.enabled=true;movement.DisableTestInputHook();player.SetActive(true);
        var cc=movement.controller;
        if(cc.height!=1.8f||cc.radius!=.3f||cc.stepOffset!=.3f||cc.slopeLimit!=45||cc.skinWidth!=.03f)throw new Exception("Frozen capsule changed");
        var route=JObject.Parse(File.ReadAllText(Folder+"/regional_route.json"))["route"];
        Vector3 P(JToken p)=>new Vector3((float)p[0],(float)p[1],(float)p[2]);
        var initial=P(route[0]);Physics.SyncTransforms();
        if(!Physics.Raycast(initial+Vector3.up*10,Vector3.down,out var hit,20,8))throw new Exception("No imported spawn support");
        cc.enabled=false;player.transform.position=new Vector3(initial.x,hit.point.y+.94f,initial.z);movement.ResetVerticalMotion();cc.enabled=true;
        var heading=P(route[1])-initial;heading.y=0;player.transform.rotation=Quaternion.LookRotation(heading);
        var look=player.GetComponent<DieuKhienGocNhin>();if(look)look.enabled=true;
        var camera=player.GetComponentInChildren<Camera>();camera.enabled=true;camera.farClipPlane=1800;
        var sun=new GameObject("DaLat_Daylight").AddComponent<Light>();sun.type=LightType.Directional;sun.intensity=1.15f;sun.shadows=LightShadows.Soft;sun.transform.rotation=Quaternion.Euler(44,-30,0);
        RenderSettings.ambientMode=UnityEngine.Rendering.AmbientMode.Trilight;RenderSettings.ambientSkyColor=new Color(.55f,.62f,.68f);RenderSettings.ambientEquatorColor=new Color(.35f,.38f,.4f);RenderSettings.ambientGroundColor=new Color(.17f,.18f,.16f);
        player.AddComponent<HoTroDiChuyenTheoKhu>();
        Directory.CreateDirectory("Assets/Scenes");EditorSceneManager.SaveScene(scene,ScenePath);AssetDatabase.SaveAssets();
        Inspect();
    }
    public static void FinalizeStaticFlags()
    {
        if(EditorApplication.isPlayingOrWillChangePlaymode)throw new Exception("Stop Play first");
        string path=Folder+"/LakeGateMarketV32.prefab";
        var root=PrefabUtility.LoadPrefabContents(path);
        try
        {
            foreach(var mf in root.GetComponentsInChildren<MeshFilter>(true))GameObjectUtility.SetStaticEditorFlags(mf.gameObject,StaticEditorFlags.OccluderStatic|StaticEditorFlags.OccludeeStatic);
            PrefabUtility.SaveAsPrefabAsset(root,path);
        }
        finally{PrefabUtility.UnloadPrefabContents(root);}
        AssetDatabase.SaveAssets();Inspect();
    }
    public static void Inspect()
    {
        var scene=SceneManager.GetActiveScene();if(scene.path!=ScenePath)throw new Exception("Wrong regional scene");
        var roots=scene.GetRootGameObjects();var transforms=roots.SelectMany(r=>r.GetComponentsInChildren<Transform>(true)).ToArray();
        int missing=transforms.Sum(t=>GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(t.gameObject));
        var p=UnityEngine.Object.FindAnyObjectByType<DieuKhienDiChuyen>();var cc=p.controller;
        var colliders=transforms.SelectMany(t=>t.GetComponents<MeshCollider>()).ToArray();
        bool matched=colliders.All(c=>c.sharedMesh&&c.GetComponent<MeshFilter>()&&c.sharedMesh==c.GetComponent<MeshFilter>().sharedMesh);
        var manifest=JObject.Parse(File.ReadAllText(Folder+"/context.json"));
        string pavingName=(string)manifest["meshes"].Single(m=>m["sources"].Any(n=>(string)n=="V32_Walk_Continuous_2p4m_VisibleAndCollider"))["name"];
        var paving=colliders.SingleOrDefault(c=>c.name==pavingName);
        bool pavingPresent=paving&&paving.sharedMesh&&paving.GetComponent<Renderer>().enabled;
        bool ready=missing==0&&matched&&pavingPresent&&p.enabled&&cc.enabled&&cc.height==1.8f&&cc.radius==.3f&&cc.stepOffset==.3f&&cc.slopeLimit==45;
        bool testDependency=transforms.Any(t=>t.GetComponent<C3RouteFixture>()||t.GetComponent<DaLatMovementTestProfile>()||t.GetComponent<DaLatManualMovementRecorder>());
        ready&=!testDependency;
        File.WriteAllText(Review+"/v32_scene_preflight.json",new JObject{["utc"]=DateTime.UtcNow.ToString("o"),["scene"]=scene.path,["ready"]=ready,["missing_scripts"]=missing,["mesh_colliders"]=colliders.Length,["authored_paving_actual_instance_present"]=pavingPresent,["paving_mesh"]=paving?paving.sharedMesh.name:"MISSING",["all_colliders_share_renderer_mesh"]=matched,["test_fixture_or_profile_in_saved_scene"]=testDependency,["player_height"]=cc.height,["player_radius"]=cc.radius,["step"]=cc.stepOffset,["slope"]=cc.slopeLimit,["skin"]=cc.skinWidth}.ToString());
        if(!ready)throw new Exception("Regional preflight failed");
    }
}
