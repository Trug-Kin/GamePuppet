using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public static class ConRoiR22Scenery
{
    public const string Source="Assets/Scenes/JourneyRepair_v48_r21_playtest.unity";
    public const string Scene="Assets/Scenes/JourneyRepair_v48_r22.unity";
    public const string Out="Docs/Reviews/journey_v48/r22";
    const string Art="Assets/Art/Environment/JourneyV48/SceneryR22";
    public static readonly (string name,Vector3 eye,Vector3 target)[] Views={
        ("lake",new Vector3(680,2.8f,503),new Vector3(540,.3f,525)),
        ("bridge",new Vector3(619,2.8f,554),new Vector3(661,1.5f,518)),
        ("roundabout",new Vector3(691,2.8f,480),new Vector3(715,1.7f,459)),
        ("market",new Vector3(789,9.2f,218),new Vector3(791,16,154)),
        ("planter-gate",new Vector3(712,2.2f,450.5f),new Vector3(734,2.5f,410)),
        ("flowers",new Vector3(642.5465f,2.9413f,545.6624f),new Vector3(641.0465f,1.72f,547.6624f)),
        ("shore",new Vector3(649,3.7f,500),new Vector3(654,.3f,506))
    };
    static DieuPhoiHanhTrinh Open(string p){EditorSceneManager.OpenScene(p);return UnityEngine.Object.FindAnyObjectByType<DieuPhoiHanhTrinh>();}
    static void Visible(DieuPhoiHanhTrinh s){s.hub.SetActive(false);s.lake.SetActive(true);s.market.SetActive(true);foreach(var g in s.GetComponent<CanhQuanChoV48>().scenery)if(g)g.SetActive(true);Physics.SyncTransforms();}
    public static void Capture(Camera eye,string path)
    {
        Directory.CreateDirectory(System.IO.Path.GetDirectoryName(path));
        var rt=RenderTexture.GetTemporary(1280,800,24);var prev=RenderTexture.active;var t=new Texture2D(1280,800,TextureFormat.RGB24,false);
        try{for(int i=0;i<3;i++)RenderPipeline.SubmitRenderRequest(eye,new UniversalRenderPipeline.SingleCameraRequest{destination=rt});RenderTexture.active=rt;t.ReadPixels(new Rect(0,0,1280,800),0,0);t.Apply();File.WriteAllBytes(path,t.EncodeToPNG());}
        finally{RenderTexture.active=prev;RenderTexture.ReleaseTemporary(rt);UnityEngine.Object.DestroyImmediate(t);}
    }
    public static void Prototype()
    {
        var original=GraphicsSettings.defaultRenderPipeline;var quality=QualitySettings.renderPipeline;
        var s=Open(Source);RenderViews(s,"before",false);
        s=Open(Scene);
        var old=s.GetComponent<HoSoChoiThuR21>();if(old)UnityEngine.Object.DestroyImmediate(old);
        var profile=s.GetComponent<HoSoSceneR22>()??s.gameObject.AddComponent<HoSoSceneR22>();profile.story=s;
        if(!AssetDatabase.LoadAssetAtPath<UniversalRenderPipelineAsset>(Art+"/Pipeline.asset")){
            var p=(UniversalRenderPipelineAsset)(quality?quality:original);
            AssetDatabase.CopyAsset(AssetDatabase.GetAssetPath(p),Art+"/Pipeline.asset");
            var sp=new SerializedObject(p);var sourceRenderer=sp.FindProperty("m_RendererDataList").GetArrayElementAtIndex(0).objectReferenceValue;
            AssetDatabase.CopyAsset(AssetDatabase.GetAssetPath(sourceRenderer),Art+"/Renderer.asset");
            var renderer=AssetDatabase.LoadAssetAtPath<UniversalRendererData>(Art+"/Renderer.asset");
            var feature=ScriptableObject.CreateInstance<TinhNangSuongKhongGianR22>();feature.name="Regional atmosphere";feature.fogShader=AssetDatabase.LoadAssetAtPath<Shader>(Art+"/WorldMist.shader");
            AssetDatabase.AddObjectToAsset(feature,renderer);renderer.rendererFeatures.Add(feature);EditorUtility.SetDirty(renderer);
            var clone=AssetDatabase.LoadAssetAtPath<UniversalRenderPipelineAsset>(Art+"/Pipeline.asset");var cs=new SerializedObject(clone);cs.FindProperty("m_RendererDataList").GetArrayElementAtIndex(0).objectReferenceValue=renderer;cs.ApplyModifiedPropertiesWithoutUndo();EditorUtility.SetDirty(clone);
        }
        profile.pipeline=AssetDatabase.LoadAssetAtPath<UniversalRenderPipelineAsset>(Art+"/Pipeline.asset");
        s.GetComponent<SuongDaLatV48>().enabled=false;
        var mist=s.GetComponent<SuongKhongGianR22>()??s.gameObject.AddComponent<SuongKhongGianR22>();mist.story=s;
        mist.corridors=new[]{new Vector4(509,515,611,563),new Vector4(611,563,658,516),new Vector4(658,516,720,482),new Vector4(720,482,733,417),new Vector4(733,417,782,275),new Vector4(782,275,791,170)};
        var experience=s.GetComponent<TraiNghiemHanhTrinhR22>()??s.gameObject.AddComponent<TraiNghiemHanhTrinhR22>();experience.story=s;
        EditorSceneManager.SaveScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene());AssetDatabase.SaveAssets();
        GraphicsSettings.defaultRenderPipeline=profile.pipeline;QualitySettings.renderPipeline=profile.pipeline;
        RenderViews(s,"prototype",true);
        GraphicsSettings.defaultRenderPipeline=original;QualitySettings.renderPipeline=quality;Open(Scene);
        Debug.Log("R22_PROTOTYPE_COMPLETE");
    }
    public static void RenderViews(DieuPhoiHanhTrinh s,string stage,bool regional)
    {
        Visible(s);var camera=UnityEngine.Object.Instantiate(s.player.GetComponentInChildren<Camera>());camera.transform.SetParent(null);camera.gameObject.SetActive(true);camera.enabled=false;camera.fieldOfView=60;camera.aspect=1.6f;camera.farClipPlane=1000;
        s.data.region=0;RenderSettings.fog=!regional;RenderSettings.fogMode=FogMode.ExponentialSquared;RenderSettings.fogDensity=.006f;RenderSettings.fogColor=new Color(.36f,.43f,.47f);
        var rows=new JArray();
        foreach(var v in Views){camera.transform.SetPositionAndRotation(v.eye,Quaternion.LookRotation(v.target-v.eye));Capture(camera,Out+"/"+stage+"/"+v.name+".png");rows.Add(new JObject{["name"]=v.name,["eye"]=V(v.eye),["target"]=V(v.target),["fov"]=60});}
        File.WriteAllText(Out+"/"+stage+"/method.json",new JObject{["method"]="Editor URP real scene render requests; fixed internal review cameras, not the missing four user photos. Day profile; runtime/reflection review still required.",["views"]=rows}.ToString());UnityEngine.Object.DestroyImmediate(camera.gameObject);
    }
    static JArray V(Vector3 v)=>new JArray(v.x,v.y,v.z);
    public static void ExportWork()
    {
        var s=Open(Source);Visible(s);var rows=new JArray();
        foreach(var r in UnityEngine.Object.FindObjectsByType<MeshRenderer>(FindObjectsInactive.Include)){
            if(!r.enabled||!r.gameObject.activeInHierarchy)continue;
            var f=r.GetComponent<MeshFilter>();if(!f||!f.sharedMesh)continue;
            if(r.transform.root.name!="R11_Local_Bridge_Supports")continue;
            var m=f.sharedMesh;rows.Add(new JObject{["path"]=PathOf(r.transform),["material"]=r.sharedMaterial.name,["vertices"]=new JArray(m.vertices.Select(v=>V(f.transform.TransformPoint(v)))),["triangles"]=new JArray(m.triangles)});
        }
        File.WriteAllText(Out+"/bridge-world-meshes.json",rows.ToString(Newtonsoft.Json.Formatting.None));
        var ground=new JArray();
        foreach(float x in new[]{701f,705f,715f,725f,729f})foreach(float z in new[]{445f,449f,459f,469f,474f}){
            var hits=Physics.RaycastAll(new Vector3(x,15,z),Vector3.down,30,~0,QueryTriggerInteraction.Ignore);
            ground.Add(new JObject{["x"]=x,["z"]=z,["hits"]=new JArray(hits.Select(h=>new JObject{["y"]=h.point.y,["path"]=PathOf(h.transform)}))});
        }
        File.WriteAllText(Out+"/island-support.json",ground.ToString());Open(Source);Debug.Log("R22_EXPORT_WORK_COMPLETE");
    }
    static string PathOf(Transform t)=>t.parent?PathOf(t.parent)+"/"+t.name:t.name;
    public static void FinalPolish()
    {
        var s=Open(Scene);var root=GameObject.Find("R22_Market_Front_Detail");
        foreach(var t in root.GetComponentsInChildren<Transform>().Where(t=>t.name.StartsWith("Upper_Brown_Louvre_")).ToArray())UnityEngine.Object.DestroyImmediate(t.gameObject);
        // R59: P03 and S1 (published 2020-04-12, capture unverified) show an
        // upper glazed zone and lower metal louvers. The opaque R22 screen
        // concealed the authored Blender facade; retire it instead of adding
        // another facade layer. Retained geometry comes from Blender source.
        var backing=root.transform.Find("Upper_Screen_Recess");if(backing)UnityEngine.Object.DestroyImmediate(backing.gameObject);
        foreach(var t in s.traces){if(t.Find("R22_LocalDetailLight"))continue;var g=new GameObject("R22_LocalDetailLight");g.transform.SetParent(t,false);g.transform.localPosition=new Vector3(0,.35f,.2f);var light=g.AddComponent<Light>();light.type=LightType.Point;light.range=2;light.intensity=.22f;light.color=new Color(.85f,.82f,.65f);}
        EditorSceneManager.SaveScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene());AssetDatabase.SaveAssets();
        var deps=AssetDatabase.GetDependencies(Scene,true);File.WriteAllText(Out+"/scene-asset-dependencies.json",new JArray(deps).ToString());
        Visible(s);var audit=new JObject{["player_controller"]=new JObject{["speed"]=s.player.speed,["height"]=s.player.controller.height,["radius"]=s.player.controller.radius,["step"]=s.player.controller.stepOffset,["slope"]=s.player.controller.slopeLimit,["gravity"]=s.player.gravity,["jump"]=s.player.jumpHeight},["new_flower_renderers"]=new JArray(UnityEngine.Object.FindObjectsByType<MeshRenderer>(FindObjectsInactive.Include).Where(r=>r.transform.parent&&r.transform.parent.name=="R22_Detailed_Route_Flowers").Select(r=>new JObject{["name"]=r.name,["enabled"]=r.enabled,["active"]=r.gameObject.activeInHierarchy,["mesh"]=AssetDatabase.GetAssetPath(r.GetComponent<MeshFilter>().sharedMesh),["materials"]=new JArray(r.sharedMaterials.Select(m=>AssetDatabase.GetAssetPath(m)))})),["rounded_bridge_flower_renderers_still_enabled"]=UnityEngine.Object.FindObjectsByType<MeshRenderer>(FindObjectsInactive.Include).Count(r=>r.transform.root.name=="R11_Local_Bridge_Supports"&&r.enabled&&r.sharedMaterial&&r.sharedMaterial.name.Contains("Flowers"))};
        File.WriteAllText(Out+"/scene-final-audit.json",audit.ToString());RenderViews(s,"polish-preview",false);Open(Scene);Debug.Log("R22_FINAL_POLISH_COMPLETE");
    }
    static Vector3 ReadV(JToken t)=>new Vector3((float)t[0],(float)t[1],(float)t[2]);
    static Material Detail(string name,Color color,float size)
    {
        var m=AssetDatabase.LoadAssetAtPath<Material>(Art+"/"+name+".mat");if(!m){m=new Material(Shader.Find("ConRoi/R22/SurfaceDetail"));AssetDatabase.CreateAsset(m,Art+"/"+name+".mat");}m.SetColor("_BaseColor",color);m.SetFloat("_Tiling",size);EditorUtility.SetDirty(m);return m;
    }
    public static void FinishScenery()
    {
        var s=Open(Scene);Visible(s);var log=new JArray();
        foreach(var c in UnityEngine.Object.FindObjectsByType<MeshCollider>(FindObjectsInactive.Include)){
            if(!c.sharedMesh||AssetDatabase.GetAssetPath(c.sharedMesh).StartsWith(Art))continue;
            if(c.transform.root.name!="OngDao_V35_Photo_Batch"&&c.transform.root.name!="R11_Local_Bridge_Supports")continue;
            var vv=c.sharedMesh.vertices;int count=0;
            for(int i=0;i<vv.Length;i++){var p=c.transform.TransformPoint(vv[i]);if(Vector2.Distance(new Vector2(p.x,p.z),new Vector2(714.94f,459.48f))<13.45f&&p.y>=1.19f&&p.y<4.2f){p.y+=.564f-.043f*(p.z-459)-.009f*(p.x-715)+.20f-1.36f;vv[i]=c.transform.InverseTransformPoint(p);count++;}}
            if(count==0)continue;var path=Art+"/Collider_"+log.Count+".asset";var m=AssetDatabase.LoadAssetAtPath<Mesh>(path);if(!m){m=UnityEngine.Object.Instantiate(c.sharedMesh);AssetDatabase.CreateAsset(m,path);}m.vertices=vv;m.RecalculateBounds();m.RecalculateNormals();c.sharedMesh=m;EditorUtility.SetDirty(m);log.Add(new JObject{["path"]=PathOf(c.transform),["vertices"]=count,["asset"]=path});
        }
        // Give the edited top its exact visible support even when the historic
        // source uses separate, differently named collision parts.
        if(!GameObject.Find("R22_Island_Deck_Collider")){
            var f=UnityEngine.Object.FindObjectsByType<MeshFilter>(FindObjectsInactive.Include).Single(f=>PathOf(f.transform)=="R11_Local_Bridge_Supports/OD35_Surface_0");var old=f.sharedMesh;var vv=old.vertices.Select(f.transform.TransformPoint).ToArray();var tt=old.triangles;var chosen=new System.Collections.Generic.List<int>();
            for(int i=0;i<tt.Length;i+=3)if(Enumerable.Range(0,3).All(j=>Vector2.Distance(new Vector2(vv[tt[i+j]].x,vv[tt[i+j]].z),new Vector2(714.94f,459.48f))<13.45f))chosen.AddRange(new[]{tt[i],tt[i+1],tt[i+2]});
            var mesh=new Mesh{name="R22_Island_Deck_Collider",indexFormat=IndexFormat.UInt32};mesh.vertices=vv;mesh.triangles=chosen.ToArray();mesh.RecalculateBounds();AssetDatabase.CreateAsset(mesh,Art+"/IslandDeckCollider.asset");var g=new GameObject("R22_Island_Deck_Collider");g.layer=3;g.AddComponent<MeshCollider>().sharedMesh=mesh;
        }
        File.WriteAllText(Out+"/collider-delta.json",log.ToString());
        var root=GameObject.Find("R22_Market_Front_Detail");if(root)UnityEngine.Object.DestroyImmediate(root);root=new GameObject("R22_Market_Front_Detail");
        var red=Detail("FadedAwning",new Color(.40f,.075f,.05f),0);var timber=Detail("BrownLouvres",new Color(.19f,.105f,.065f),0);
        var model=AssetDatabase.LoadAssetAtPath<GameObject>(Art+"/MarketAwning.fbx");
        for(int i=0;i<20;i++){var g=UnityEngine.Object.Instantiate(model,root.transform);g.name="MarketAwning_"+i;g.transform.position=new Vector3(773.4f+i*2,12.25f,160.7f);g.transform.localScale=new Vector3(1.99f,1,1);foreach(var r in g.GetComponentsInChildren<Renderer>())r.sharedMaterials=r.sharedMaterials.Select(m=>red).ToArray();}
        // R59: keep the source Blender upper glazing/mullions/lower louvers.
        // Do not regenerate the opaque brown screen retired in FinalPolish.
        foreach(var p in s.puppets.Concat(new[]{s.schoolChapter.puppet,s.slopeChapter.puppet})){
            if(!p||p.transform.Find("R22_Wood_Readability"))continue;var g=new GameObject("R22_Wood_Readability");g.transform.SetParent(p.transform,false);g.transform.localPosition=new Vector3(0,1.7f,-1.2f);var l=g.AddComponent<Light>();l.type=LightType.Point;l.color=new Color(.93f,.80f,.63f);l.intensity=.8f;l.range=5;l.shadows=LightShadows.None;
        }
        var desk=new GameObject("R22_Desk_Practical_Pool");desk.transform.SetParent(s.hub.transform,true);desk.transform.position=new Vector3(-33.8f,1.8f,-34);var light=desk.AddComponent<Light>();light.type=LightType.Point;light.color=new Color(1,.80f,.55f);light.intensity=.65f;light.range=3.5f;
        EditorSceneManager.SaveScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene());AssetDatabase.SaveAssets();RenderViews(s,"scenery",false);Open(Scene);Debug.Log("R22_SCENERY_FINISHED_REQUIRES_PLAY_REVIEW");
    }
    public static void GroundAndRegression()
    {
        var s=Open(Scene);Visible(s);
        var urban=Detail("UrbanGround",new Color(.28f,.29f,.25f),.6f);urban.SetFloat("_Urban",1);EditorUtility.SetDirty(urban);
        var f=UnityEngine.Object.FindObjectsByType<MeshRenderer>(FindObjectsInactive.Include).Single(r=>PathOf(r.transform)=="R20_Whole_Source_Export/Surface_176");
        // Preserve the bounded terrain overrides on derived review scenes. These
        // materials remove the R22 paving multiplier; they do not alter support.
        var terrainShader=f.sharedMaterial?f.sharedMaterial.shader:null;
        bool localTerrain=terrainShader&&(terrainShader.name=="ConRoi/R55/LocalTerrainDetail"||terrainShader.name=="ConRoi/R56/ParkTerrainDetail");
        if(!localTerrain)f.sharedMaterial=urban;
        EditorSceneManager.SaveScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene());AssetDatabase.SaveAssets();
        var samples=new JArray();foreach(float x in new[]{705f,715f,725f})foreach(float z in new[]{450f,459f,469f}){
            var hit=Physics.RaycastAll(new Vector3(x,10,z),Vector3.down,20,~0,QueryTriggerInteraction.Ignore).OrderByDescending(h=>h.point.y).ToArray();
            samples.Add(new JObject{["x"]=x,["z"]=z,["hits"]=new JArray(hit.Select(h=>new JObject{["y"]=h.point.y,["path"]=PathOf(h.transform)}))});}
        File.WriteAllText(Out+"/island-collider-review.json",samples.ToString());
        Directory.CreateDirectory(Out+"/regression");var flags=System.Reflection.BindingFlags.Static|System.Reflection.BindingFlags.NonPublic;typeof(ConRoiR21Preflight).GetField("Out",flags).SetValue(null,Out+"/regression");typeof(ConRoiR21Preflight).GetMethod("RouteRegression",flags).Invoke(null,null);
        RenderViews(s,"finished-editor",false);Open(Scene);Debug.Log("R22_GROUND_REGRESSION_COMPLETE");
    }
    public static void IntegrateGeometry()
    {
        var s=Open(Scene);Visible(s);var changes=new JArray();
        var transforms=UnityEngine.Object.FindObjectsByType<Transform>(FindObjectsInactive.Include).GroupBy(t=>PathOf(t)).ToDictionary(g=>g.Key,g=>g.First());
        foreach(var row in JArray.Parse(File.ReadAllText(Out+"/island-geometry-edits.json"))){
            var t=transforms[(string)row["path"]];var f=t.GetComponent<MeshFilter>();var old=f.sharedMesh;
            var asset=Art+"/Island_"+t.name+".asset";var m=AssetDatabase.LoadAssetAtPath<Mesh>(asset);
            if(!m){m=UnityEngine.Object.Instantiate(old);m.name="R22_"+t.name;AssetDatabase.CreateAsset(m,asset);}
            m.vertices=row["vertices"].Select(v=>t.InverseTransformPoint(ReadV(v))).ToArray();m.RecalculateBounds();m.RecalculateNormals();EditorUtility.SetDirty(m);f.sharedMesh=m;
            int rebound=0;foreach(var c in UnityEngine.Object.FindObjectsByType<MeshCollider>(FindObjectsInactive.Include))if(c.sharedMesh==old){c.sharedMesh=null;c.sharedMesh=m;rebound++;}
            changes.Add(new JObject{["path"]=PathOf(t),["vertices_changed"]=row["changed_vertices"],["colliders_rebound"]=rebound,["asset"]=asset});
        }
        var imported=UnityEngine.Object.Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>(Art+"/FlowerR22.fbx"));var fi=imported.GetComponentInChildren<MeshFilter>();var matrix=fi.transform.localToWorldMatrix;
        var flower=AssetDatabase.LoadAssetAtPath<Mesh>(Art+"/FlowerMetres.asset");if(!flower){flower=UnityEngine.Object.Instantiate(fi.sharedMesh);flower.name="FlowerR22_Metres";flower.vertices=flower.vertices.Select(matrix.MultiplyPoint3x4).ToArray();flower.normals=flower.normals.Select(n=>matrix.inverse.transpose.MultiplyVector(n).normalized).ToArray();flower.RecalculateBounds();AssetDatabase.CreateAsset(flower,Art+"/FlowerMetres.asset");}
        var slots=fi.GetComponent<Renderer>().sharedMaterials.Select(m=>m.name).ToArray();UnityEngine.Object.DestroyImmediate(imported);
        var sourceRoot=UnityEngine.Object.FindObjectsByType<Transform>(FindObjectsInactive.Include).Single(t=>t.name=="R21_RealPlanterFlowerSample");
        var sample=sourceRoot.GetComponentInChildren<MeshRenderer>();var sourceMats=sample.sharedMaterials;
        var foliage=sourceMats.First(m=>m.name.Contains("Foliage"));var centre=sourceMats.First(m=>m.name.Contains("Centre")||m.name.Contains("Center"));
        var red=AssetDatabase.LoadAssetAtPath<Material>("Assets/Art/Environment/JourneyV48/PlaytestR21/PetalRed_Review.mat");var yellow=AssetDatabase.LoadAssetAtPath<Material>("Assets/Art/Environment/JourneyV48/PlaytestR21/PetalYellow_Review.mat");
        var root=GameObject.Find("R22_Detailed_Route_Flowers");if(root)UnityEngine.Object.DestroyImmediate(root);root=new GameObject("R22_Detailed_Route_Flowers");root.transform.SetParent(s.lake.transform.parent,true);
        var flowerRows=JArray.Parse(File.ReadAllText(Out+"/flower-placements.json"));var rand=new System.Random(2201);
        foreach(bool gold in new[]{false,true}){
            var selected=flowerRows.Where(p=>(bool)p["yellow"]==gold).ToArray();var combines=new System.Collections.Generic.List<CombineInstance>();
            foreach(var p in selected){var pos=ReadV(p["position"]);var scale=.90f+(float)rand.NextDouble()*.14f;var rotation=Quaternion.Euler(0,rand.Next(360),0);
                for(int sub=0;sub<flower.subMeshCount;sub++)combines.Add(new CombineInstance{mesh=flower,subMeshIndex=sub,transform=Matrix4x4.TRS(pos,rotation,Vector3.one*scale)});
            }
            // Batch by material instead of one renderer per flower.
            for(int sub=0;sub<flower.subMeshCount;sub++){
                var g=new GameObject((gold?"Yellow":"Red")+"_"+sub);g.transform.SetParent(root.transform,false);var mesh=new Mesh{name=g.name,indexFormat=UnityEngine.Rendering.IndexFormat.UInt32};mesh.CombineMeshes(combines.Where((c,i)=>i%flower.subMeshCount==sub).ToArray(),true,true);
                var path=Art+"/Flowers_"+g.name+".asset";var existing=AssetDatabase.LoadAssetAtPath<Mesh>(path);if(existing){EditorUtility.CopySerialized(mesh,existing);UnityEngine.Object.DestroyImmediate(mesh);mesh=existing;}else AssetDatabase.CreateAsset(mesh,path);
                g.AddComponent<MeshFilter>().sharedMesh=mesh;g.AddComponent<MeshRenderer>().sharedMaterial=sub==0?foliage:sub==1?centre:gold?yellow:red;
            }
        }
        foreach(var r in UnityEngine.Object.FindObjectsByType<MeshRenderer>(FindObjectsInactive.Include))if(r.transform.root.name=="R11_Local_Bridge_Supports"&&r.sharedMaterial&&r.sharedMaterial.name.Contains("Flowers"))r.enabled=false;
        var paving=Detail("GranitePaving",new Color(.39f,.38f,.34f),.4f);var soil=Detail("Soil",new Color(.105f,.075f,.045f),0);var terrain=Detail("Terrain",new Color(.19f,.22f,.16f),0);
        foreach(var r in UnityEngine.Object.FindObjectsByType<MeshRenderer>(FindObjectsInactive.Include)){
            if(r.transform.root.name=="R11_Local_Bridge_Supports"&&r.sharedMaterial){if(r.sharedMaterial.name=="OD35_Paving")r.sharedMaterial=paving;if(r.sharedMaterial.name=="OD35_Soil")r.sharedMaterial=soil;}
            if(PathOf(r.transform)=="R20_Whole_Source_Export/Surface_142")r.sharedMaterial=terrain;
        }
        File.WriteAllText(Out+"/geometry-integration.json",new JObject{["changes"]=changes,["flowers"]=flowerRows.Count,["flower_triangles_each"]=flower.triangles.Length/3,["material_slots"]=new JArray(slots),["method"]="Exact Blender vertex edits imported into derived Unity meshes; original UVs/material binding and xz retained; colliders sharing modified meshes rebound. Replaced active bridge/island rounded flower renderers; retained verified r21 sample bed."}.ToString());
        EditorSceneManager.SaveScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene());AssetDatabase.SaveAssets();
        RenderViews(s,"geometry",false);Open(Scene);Debug.Log("R22_GEOMETRY_INTEGRATED");
    }
    public static void Survey()
    {
        Directory.CreateDirectory(Out);
        EditorSceneManager.OpenScene(Source);
        var s=UnityEngine.Object.FindAnyObjectByType<DieuPhoiHanhTrinh>();
        s.hub.SetActive(false);s.lake.SetActive(true);s.market.SetActive(true);
        foreach(var g in s.GetComponent<CanhQuanChoV48>().scenery)if(g)g.SetActive(true);
        Physics.SyncTransforms();
        var renderers=new JArray(UnityEngine.Object.FindObjectsByType<Renderer>(FindObjectsInactive.Include).Select(r=>new JObject{
            ["path"]=PathOf(r.transform),["enabled"]=r.enabled,["active"]=r.gameObject.activeInHierarchy,
            ["min"]=V(r.bounds.min),["max"]=V(r.bounds.max),
            ["materials"]=new JArray(r.sharedMaterials.Select(m=>m?m.name:"")),
            ["material_assets"]=new JArray(r.sharedMaterials.Select(m=>m?AssetDatabase.GetAssetPath(m):"")),
            ["mesh"]=r.GetComponent<MeshFilter>()?AssetDatabase.GetAssetPath(r.GetComponent<MeshFilter>().sharedMesh):""
        }));
        var gameplay=new JObject{
            ["lake_spawn"]=V(s.lakeSpawn.position),["market_spawn"]=V(s.marketSpawn.position),["hub_spawn"]=V(s.hubSpawn.position),
            ["school_spawn"]=V(s.schoolChapter.spawn.position),["slope_spawn"]=V(s.slopeChapter.spawn.position),
            ["puppets"]=new JArray(s.puppets.Select(p=>new JObject{["path"]=PathOf(p.transform),["position"]=V(p.transform.position)})),
            ["traces"]=new JArray(s.traces.Select(t=>new JObject{["path"]=PathOf(t),["position"]=V(t.position)})),
            ["interactions"]=new JArray(UnityEngine.Object.FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include).Where(m=>m is IPlayerInteractable).Select(m=>new JObject{["path"]=PathOf(m.transform),["position"]=V(m.transform.position),["type"]=m.GetType().Name})),
            ["controller"]=EditorJsonUtility.ToJson(s.player.controller),["movement"]=EditorJsonUtility.ToJson(s.player)
        };
        File.WriteAllText(Out+"/source-scene-survey.json",new JObject{["source"]=Source,["renderers"]=renderers,["gameplay"]=gameplay}.ToString());
        // Reopen from disk: this survey never saves visibility edits into the base.
        EditorSceneManager.OpenScene(Source);
        if(!File.Exists(Scene)&&!AssetDatabase.CopyAsset(Source,Scene))throw new Exception("Derivative copy failed");
        Debug.Log("R22_SOURCE_SURVEY_COMPLETE "+renderers.Count);
    }
}
