using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class ConRoiLakeMarketV34
{
    public const string ScenePath="Assets/Scenes/LakeMarketStory_v34.unity";
    const string Folder="Assets/Art/Gameplay/LakeMarketV34";
    public static void Assemble()
    {
        if(EditorApplication.isPlayingOrWillChangePlaymode)throw new Exception("Stop Play first");
        AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
        var importer=(ModelImporter)AssetImporter.GetAtPath(Folder+"/StoryProps.fbx");importer.addCollider=false;importer.materialImportMode=ModelImporterMaterialImportMode.ImportStandard;importer.SaveAndReimport();
        EditorSceneManager.OpenScene(ConRoiRouteV33.ScenePath);
        var props=(GameObject)PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>(Folder+"/StoryProps.fbx"));
        PrefabUtility.UnpackPrefabInstance(props,PrefabUnpackMode.Completely,InteractionMode.AutomatedAction);
        var record=JObject.Parse(File.ReadAllText(Folder+"/story_models.json"));
        var transforms=props.GetComponentsInChildren<Transform>(true);
        Transform Find(string n)=>transforms.First(t=>t.name==n);
        foreach(var entry in (JObject)record["materials"])
        {
            string path=Folder+"/"+entry.Key+".mat";var mat=AssetDatabase.LoadAssetAtPath<Material>(path);if(!mat){mat=new Material(Shader.Find("Universal Render Pipeline/Lit"));AssetDatabase.CreateAsset(mat,path);}var a=(JArray)entry.Value;mat.SetColor("_BaseColor",new Color((float)a[0],(float)a[1],(float)a[2]));mat.SetFloat("_Smoothness",.15f);
            foreach(var r in props.GetComponentsInChildren<Renderer>())if(r.sharedMaterials.Any(m=>m&&m.name.StartsWith(entry.Key)))r.sharedMaterial=mat;
        }
        foreach(var name in record["collision_names"]){var go=Find((string)name).gameObject;go.layer=3;go.AddComponent<MeshCollider>();}
        var story=new GameObject("Lake_Market_Story_GDD21").AddComponent<DieuPhoiHanhTrinh>();story.player=UnityEngine.Object.FindAnyObjectByType<DieuKhienDiChuyen>();story.player.DisableTestInputHook();
        // Production scene never depends on the review recorder or its Start coroutine.
        foreach(var component in story.player.GetComponents<MonoBehaviour>())if(component&&component.GetType().Name.StartsWith("C3"))UnityEngine.Object.DestroyImmediate(component);
        foreach(var renderer in story.player.GetComponentsInChildren<Renderer>())renderer.enabled=false;
        story.hub=Find("CR_Hub").gameObject;story.lake=Find("CR_Lake").gameObject;story.market=Find("CR_Market").gameObject;
        Vector3 V(JToken t)=>new Vector3((float)t[0],(float)t[1],(float)t[2]);
        Transform Spawn(string name,Vector3 p){var t=new GameObject(name).transform;t.position=p;return t;}
        Vector3 SupportedSpawn(Vector3 p){Physics.SyncTransforms();if(!Physics.Raycast(p+Vector3.up*.5f,Vector3.down,out var hit,3,8))throw new Exception("Missing story spawn support");return new Vector3(p.x,hit.point.y+.94f,p.z);}
        story.hubSpawn=Spawn("Hub_Story_Spawn",V(record["hub_spawn"]));story.lakeSpawn=Spawn("Lake_Story_Spawn",SupportedSpawn(V(record["lake_spawn"])));story.marketSpawn=Spawn("Market_Story_Spawn",SupportedSpawn(V(record["market_spawn"])));
        story.traces=new[]{Find("CR_Calendar_Page"),Find("CR_Books_Unopened")};story.ghosts=new[]{Find("CR_Bao"),Find("CR_Ha")};
        void Bind(string name,TuongTacHoCho.Kind kind,Vector3 size)
        {
            var t=Find(name);var go=new GameObject(name+"_Interaction");go.transform.position=t.position;go.transform.SetParent(t,true);
            var c=go.AddComponent<BoxCollider>();c.size=size;
            if(kind==TuongTacHoCho.Kind.Wake)c.center=Vector3.up*1.1f;
            if(kind==TuongTacHoCho.Kind.Bao||kind==TuongTacHoCho.Kind.Ha){c.size=new Vector3(.65f,1.7f,.65f);c.center=Vector3.up*.85f;}
            var i=go.AddComponent<TuongTacHoCho>();i.kind=kind;i.story=story;
            var parentInteraction=t.gameObject.AddComponent<TuongTacHoCho>();parentInteraction.kind=kind;parentInteraction.story=story;
        }
        Bind("CR_Bed",TuongTacHoCho.Kind.Bed,new Vector3(1.4f,.6f,2));Bind("CR_Phone",TuongTacHoCho.Kind.Phone,new Vector3(.25f,.12f,.4f));
        Bind("CR_Lake_Wake",TuongTacHoCho.Kind.Wake,new Vector3(.65f,2.6f,.5f));Bind("CR_Market_Wake",TuongTacHoCho.Kind.Wake,new Vector3(.65f,2.6f,.5f));
        Bind("CR_Bao",TuongTacHoCho.Kind.Bao,new Vector3(.65f,3.2f,.65f));Bind("CR_Ha",TuongTacHoCho.Kind.Ha,new Vector3(.65f,3.2f,.65f));
        var interaction=story.player.GetComponent<TuongTacNguoiChoi>();if(!interaction)interaction=story.player.gameObject.AddComponent<TuongTacNguoiChoi>();interaction.playerCamera=story.player.GetComponentInChildren<Camera>();
        var source=AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Art/Models/Model.blend");
        var puppetSource=source.GetComponentsInChildren<Transform>(true).First(t=>t.name=="ConRoi_Puppet_Root");
        var sourceFace=source.GetComponentsInChildren<MeshFilter>(true).First(t=>t.name=="ConRoi_Head_SameAsMinh");
        var faceMat=AssetDatabase.LoadAssetAtPath<Material>(Folder+"/MinhFace.mat");if(!faceMat){faceMat=new Material(Shader.Find("Universal Render Pipeline/Lit"));AssetDatabase.CreateAsset(faceMat,Folder+"/MinhFace.mat");}faceMat.SetColor("_BaseColor",new Color(.5f,.36f,.23f));
        for(int i=0;i<2;i++){var face=story.ghosts[i].gameObject.AddComponent<HienGuongMatTheoKhu>();face.ghostHead=story.ghosts[i].GetComponentsInChildren<Transform>().First(t=>t.name.EndsWith("_Head"));face.minhFace=sourceFace.sharedMesh;face.minhMaterial=faceMat;}
        var aiList=new System.Collections.Generic.List<DieuKhienConRoi>();
        for(int region=0;region<2;region++)for(int n=0;n<(region==0?1:4);n++)
        {
            var go=UnityEngine.Object.Instantiate(puppetSource.gameObject);go.name="ConRoi_Minh_"+region+"_"+n;go.transform.SetParent(region==0?story.lake.transform:story.market.transform,true);
            var renderers=go.GetComponentsInChildren<Renderer>();var bounds=renderers[0].bounds;foreach(var r in renderers)bounds.Encapsulate(r.bounds);
            foreach(var renderer in renderers)
            {
                renderer.sharedMaterials=renderer.sharedMaterials.Select(m=>
                {
                    string path=Folder+"/Puppet_"+(m?m.name:"wood")+".mat";var mat=AssetDatabase.LoadAssetAtPath<Material>(path);if(!mat){mat=new Material(Shader.Find("Universal Render Pipeline/Lit"));mat.SetColor("_BaseColor",m&&m.HasProperty("_Color")?m.color:new Color(.3f,.18f,.09f));mat.SetFloat("_Smoothness",.2f);AssetDatabase.CreateAsset(mat,path);}return mat;
                }).ToArray();
            }
            go.transform.rotation=Quaternion.Euler(-90,0,0);bounds=renderers[0].bounds;foreach(var r in renderers)bounds.Encapsulate(r.bounds);
            float scale=3.5f/bounds.size.y;go.transform.localScale*=scale;bounds=renderers[0].bounds;foreach(var r in renderers)bounds.Encapsulate(r.bounds);
            var spawn=region==0?story.lakeSpawn.position:story.marketSpawn.position;var p=spawn+new Vector3(2.5f+n*1.4f,0,5+n*2);
            if(Physics.Raycast(p+Vector3.up,Vector3.down,out var hit,5,8))p.y=hit.point.y;
            go.transform.position+=p-new Vector3(bounds.center.x,bounds.min.y,bounds.center.z);
            var ai=go.AddComponent<DieuKhienConRoi>();ai.playerTransform=story.player.transform;ai.dauTransform=go.GetComponentsInChildren<Transform>().FirstOrDefault(t=>t.name=="ConRoi_Head_SameAsMinh");ai.doTreGD2=.5f;aiList.Add(ai);
        }
        story.puppets=aiList.ToArray();
        var water=UnityEngine.Object.FindObjectsByType<MeshRenderer>(FindObjectsSortMode.None).First(r=>r.name=="Surface_051"&&r.enabled);
        string waterShader=File.ReadAllText(ConRoiRouteV33.Folder+"/Surface_051.shader").Replace("ConRoi/RouteV33/Surface_051","ConRoi/LakeStoryV34/Water");
        waterShader=waterShader.Replace("TEXTURE2D(_RouteMap);","TEXTURE2D(_LakeReflection); SAMPLER(sampler_LakeReflection); float4x4 _LakeReflectionVP; float _LakeReflectionReady; TEXTURE2D(_RouteMap);");
        waterShader=waterShader.Replace("return color;","float4 projected=mul(_LakeReflectionVP,float4(i.positionWS,1));float2 uv=projected.xy/max(projected.w,.0001)*.5+.5;if(_LakeReflectionReady>.5&&all(uv>0)&&all(uv<1)){float fresnel=.22+.5*pow(1-saturate(dot(n,d.viewDirectionWS)),5);color.rgb=lerp(color.rgb,SAMPLE_TEXTURE2D(_LakeReflection,sampler_LakeReflection,uv).rgb,fresnel);}return color;");
        File.WriteAllText(Folder+"/LakeReflection.shader",waterShader);AssetDatabase.ImportAsset(Folder+"/LakeReflection.shader",ImportAssetOptions.ForceSynchronousImport);
        var waterMaterial=AssetDatabase.LoadAssetAtPath<Material>(Folder+"/LakeReflection.mat");if(!waterMaterial){waterMaterial=new Material(water.sharedMaterial);AssetDatabase.CreateAsset(waterMaterial,Folder+"/LakeReflection.mat");}waterMaterial.shader=AssetDatabase.LoadAssetAtPath<Shader>(Folder+"/LakeReflection.shader");water.sharedMaterial=waterMaterial;
        var reflection=story.gameObject.AddComponent<PhanChieuMatHo>();reflection.playerCamera=story.player.GetComponentInChildren<Camera>();reflection.water=water;
        var deskPuppet=UnityEngine.Object.Instantiate(puppetSource.gameObject,story.hub.transform);deskPuppet.name="Desk_Puppet_Broken_Strings";
        deskPuppet.transform.rotation=Quaternion.Euler(-90,0,0);
        var deskRenderers=deskPuppet.GetComponentsInChildren<Renderer>();var deskBounds=deskRenderers[0].bounds;foreach(var r in deskRenderers)deskBounds.Encapsulate(r.bounds);deskPuppet.transform.localScale*=.38f/deskBounds.size.y;deskBounds=deskRenderers[0].bounds;foreach(var r in deskRenderers)deskBounds.Encapsulate(r.bounds);deskPuppet.transform.position+=new Vector3(-33,.81f,-33.4f)-new Vector3(deskBounds.center.x,deskBounds.min.y,deskBounds.center.z);
        foreach(var r in deskRenderers){string path=Folder+"/Puppet_"+(r.sharedMaterial?r.sharedMaterial.name:"wood")+".mat";var mat=AssetDatabase.LoadAssetAtPath<Material>(path);if(mat)r.sharedMaterial=mat;}
        foreach(var t in deskPuppet.GetComponentsInChildren<Transform>())if(t.name.StartsWith("ConRoi_String")){t.localScale=new Vector3(t.localScale.x,t.localScale.y*.14f,t.localScale.z);t.Rotate(0,0,55);}
        story.ambience=new[]{"Phòng trọ","Gió mặt hồ","Chợ vắng"}.Select(n=>AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/ConRoiGame/Audio/"+n+".wav")).ToArray();story.wood=AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/ConRoiGame/Audio/Gỗ va.wav");
        story.drone=AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/ConRoiGame/Audio/Drone.wav");story.clockHand=Find("CR_Clock_Hand");
        var warm=new GameObject("Room_Warm_Light").AddComponent<Light>();warm.transform.SetParent(story.hub.transform);warm.transform.position=new Vector3(-35,2.6f,-35);warm.type=LightType.Point;warm.range=8;warm.intensity=3;warm.color=new Color(1,.75f,.46f);
        // New props get local night lamps; original surface/collision meshes are untouched.
        for(int n=0;n<3;n++){var light=new GameObject("Market_Practical_"+n).AddComponent<Light>();light.transform.SetParent(story.market.transform);light.transform.position=story.ghosts[1].position+new Vector3(n*4-4,3,2);light.type=LightType.Point;light.range=12;light.intensity=5;light.color=new Color(1,.72f,.4f);}
        var route=JObject.Parse(File.ReadAllText(ConRoiRouteV32.Folder+"/regional_route.json"))["route"];Vector3 previousLamp=Vector3.positiveInfinity;
        for(int n=165;n<route.Count();n++)
        {
            var p=V(route[n]);if(Vector3.Distance(p,previousLamp)<15)continue;previousLamp=p;
            var light=new GameObject("Market_Route_Lamp_"+n).AddComponent<Light>();light.transform.SetParent(story.market.transform);light.transform.position=p+new Vector3(1.5f,3.3f,0);light.type=LightType.Point;light.range=15;light.intensity=3.5f;light.color=new Color(1,.74f,.5f);
        }
        AssetDatabase.SaveAssets();EditorSceneManager.SaveScene(SceneManager.GetActiveScene(),ScenePath);
        File.WriteAllText("Docs/Reviews/lake_gate_market_current/v34_assembly.json",new JObject{["scene"]=ScenePath,["props"]=transforms.Length,["puppets"]=story.puppets.Length,["original_colliders"]=UnityEngine.Object.FindObjectsByType<MeshCollider>(FindObjectsSortMode.None).Count(c=>AssetDatabase.GetAssetPath(c.sharedMesh).Contains("LakeGateMarketV32")),["runtime_tests"]="NOT_RUN"}.ToString());
    }
}
