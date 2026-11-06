using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;

public static class ConRoiSceneAssembly
{
    const string Folder="Assets/ConRoiGame";
    static GameObject source;
    static Material plaster, stone, wood;
    static HanhTrinhConRoi journey;
    [MenuItem("CON ROI/Assemble playable journey")]
    public static void Assemble()
    {
        if(EditorApplication.isPlayingOrWillChangePlaymode)throw new Exception("Stop Play before assembling");
        string scenePath=Folder+"/ConRoi.unity";
        if(File.Exists(scenePath))throw new Exception("Scene exists; refusing destructive regeneration");
        Directory.CreateDirectory(Folder+"/Meshes");Directory.CreateDirectory(Folder+"/Materials");
        source=AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Art/Models/Model.blend");
        if(!source)throw new Exception("Source model not imported");
        var scene=EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);
        plaster=Mat("Tường vôi",new Color(.53f,.49f,.4f));stone=Mat("Đá lát",new Color(.29f,.3f,.28f));wood=Mat("Gỗ",new Color(.28f,.16f,.08f));
        var root=new GameObject("CON RỐI — GDD v2.1");journey=root.AddComponent<HanhTrinhConRoi>();
        var p=new GameObject("Minh");p.tag="Player";
        var cc=p.AddComponent<CharacterController>();cc.height=1.8f;cc.radius=.3f;cc.center=Vector3.zero;cc.skinWidth=.03f;cc.stepOffset=.3f;cc.slopeLimit=45;cc.minMoveDistance=.001f;
        journey.player=p.AddComponent<DieuKhienDiChuyen>();journey.player.controller=cc;journey.player.speed=3.5f;journey.player.groundDistance=.15f;journey.player.groundMask=8;
        var ground=new GameObject("GroundCheck");ground.transform.SetParent(p.transform,false);ground.transform.localPosition=new Vector3(0,-.8f,0);journey.player.groundCheck=ground.transform;
        var camera=new GameObject("Main Camera");camera.tag="MainCamera";camera.transform.SetParent(p.transform,false);camera.transform.localPosition=new Vector3(0,.75f,0);
        var cam=camera.AddComponent<Camera>();cam.nearClipPlane=.05f;cam.farClipPlane=1000;cam.fieldOfView=72;camera.AddComponent<AudioListener>();
        p.AddComponent<DieuKhienGocNhin>().mouseSensitivity=500;p.AddComponent<TuongTacNguoiChoi>().playerCamera=cam;
        var light=new GameObject("Ánh sáng Đà Lạt").AddComponent<Light>();light.type=LightType.Directional;light.shadows=LightShadows.Soft;light.transform.rotation=Quaternion.Euler(40,-25,0);journey.sun=light;
        RenderSettings.ambientMode=AmbientMode.Flat;RenderSettings.ambientLight=new Color(.35f,.38f,.42f);RenderSettings.fog=true;RenderSettings.fogColor=new Color(.26f,.3f,.34f);RenderSettings.fogDensity=.009f;
        journey.hub=new GameObject("Phòng trọ");var hub=journey.hub.transform;
        Box("Sàn phòng trọ",hub,new Vector3(-2,-.15f,13),new Vector3(8,.3f,10),stone);
        Box("Tường sau",hub,new Vector3(-2,1.5f,18),new Vector3(8,3,.2f),plaster);
        Box("Tường trái",hub,new Vector3(-6,1.5f,13),new Vector3(.2f,3,10),plaster);
        Box("Tường phải",hub,new Vector3(2,1.5f,13),new Vector3(.2f,3,10),plaster);
        SelectSource(hub,m=>{var b=m.GetComponent<Renderer>().bounds;return b.center.z>8&&b.center.z<19&&b.center.x>-6&&b.center.x<2&&!m.name.StartsWith("Minh")&&!m.name.StartsWith("ConRoi");});
        var bed=Box("Giường",hub,new Vector3(-4,.35f,15),new Vector3(1.4f,.7f,2),wood);Interact(bed,DoiTuongHanhTrinh.Kind.Bed);
        var desk=Box("Bàn học",hub,new Vector3(0,.7f,15),new Vector3(1.8f,.12f,.8f),wood);
        var phone=Box("Điện thoại",hub,new Vector3(0,.81f,15),new Vector3(.16f,.03f,.28f),Mat("Điện thoại",Color.black));Interact(phone,DoiTuongHanhTrinh.Kind.Phone);
        journey.hubSpawn=Point("Hub Spawn",hub,new Vector3(-2,.94f,11),Quaternion.identity);
        journey.regions=new GameObject[4];journey.regionSpawns=new Transform[4];journey.puppets=new DieuKhienConRoi[4];journey.traces=new Transform[4];journey.traceMoved=new Transform[4];
        for(int i=0;i<4;i++)journey.regions[i]=new GameObject(new[]{"Hồ Xuân Hương","Chợ đêm Đà Lạt v30","Trường đại học","Dốc Đà Lạt"}[i]);
        var lake=journey.regions[0].transform;
        Box("Đường ven hồ",lake,new Vector3(100,-.15f,0),new Vector3(12,.3f,42),stone);
        Box("Mặt hồ tĩnh",lake,new Vector3(120,-.35f,0),new Vector3(28,.1f,90),Mat("Mặt nước",new Color(.13f,.21f,.23f),.95f),false);
        SelectSource(lake,m=>m.name.StartsWith("Pine_Tier"),new Vector3(100,0,0),8);
        Box("Lan can ven hồ",lake,new Vector3(106,.45f,0),new Vector3(.15f,.9f,42),wood);
        journey.regionSpawns[0]=Point("Lake Spawn",lake,new Vector3(100,.94f,-16),Quaternion.identity);
        var calendar=Box("Lịch treo Ông Bảo",lake,new Vector3(102,1.2f,-4),new Vector3(.35f,.5f,.04f),Mat("Giấy lịch",new Color(.74f,.69f,.55f)));
        Label("Lịch",calendar.transform,"17",new Vector3(0,0,-.04f));Trace(0,calendar.transform);
        PopulateMarket(journey.regions[1].transform);
        var school=journey.regions[2].transform;
        SelectSource(school,m=>m.name.Contains("Lop_Hoc")||m.name.StartsWith("Ban_Sinh_Vien")||m.name.StartsWith("Ban_Giao")||m.name.StartsWith("Ghe_")||m.name.StartsWith("Giao_Trinh"));
        Box("Sàn trường",school,new Vector3(-12,-.15f,0),new Vector3(8,.3f,12),stone);
        Box("Tường lớp",school,new Vector3(-12,1.5f,-5),new Vector3(8,3,.2f),plaster);
        journey.regionSpawns[2]=Point("School Spawn",school,new Vector3(-12,.94f,4),Quaternion.Euler(0,180,0));
        var chair=Box("Ghế cạnh cửa sổ",school,new Vector3(-14,.5f,-2),new Vector3(.5f,1,.5f),wood);Trace(2,chair.transform);
        var slope=journey.regions[3].transform;
        SelectSource(slope,m=>m.name.StartsWith("House_")||m.name.StartsWith("Fence")||m.name.StartsWith("PowerPole")||m.name=="Road"||m.name=="GroundSlope"||m.name.StartsWith("Log_")||m.name.StartsWith("Suitcase"));
        journey.regionSpawns[3]=Point("Slope Spawn",slope,Supported(new Vector3(-5,0,-30)),Quaternion.Euler(0,-90,0));
        var suitcase=Box("Vali đóng phủ bụi",slope,Supported(new Vector3(-15,0,-29))+Vector3.up*.05f,new Vector3(.65f,.6f,.22f),wood);Trace(3,suitcase.transform);
        var ghostNames=new[]{"Ông Bảo","Chị Hà","Cô Mai","Anh Tùng"};
        var lines=new[]{"Ông Bảo: Hôm nào cũng thế. Ngày mai rồi tính.","Chị Hà: Hồi đó ai cũng bảo ngành này ổn định. Chị cứ học thôi.","Cô Mai: Cứ theo đáp án mẫu. Đừng làm khác.","Anh Tùng: Vali anh xếp lâu rồi. Chắc để hôm khác đi."};
        for(int i=0;i<4;i++)
        {
            var spawn=journey.regionSpawns[i];var pos=spawn.position+spawn.forward*7;
            var puppet=Puppet(journey.regions[i].transform,pos,1);journey.puppets[i]=puppet.AddComponent<DieuKhienConRoi>();journey.puppets[i].playerTransform=p.transform;
            journey.puppets[i].dauTransform=puppet.transform.Find("Đầu Minh");journey.puppets[i].gesturePart=puppet.transform.Find("Tay trái");
            var ghost=Puppet(journey.regions[i].transform,pos+spawn.right*3,1);ghost.name=ghostNames[i];Interact(ghost,DoiTuongHanhTrinh.Kind.Dialogue,lines[i]);
            var exitPosition=spawn.position+spawn.forward*16;
            if(i==3)exitPosition=Supported(exitPosition);
            var exit=Box("Lối trở về",journey.regions[i].transform,exitPosition,new Vector3(1.5f,2,.2f),wood);
            Interact(exit,DoiTuongHanhTrinh.Kind.Wake);Label("Biển",exit.transform,"Trở về",new Vector3(0,.2f,-.15f));
        }
        Puppet(hub,new Vector3(.2f,.8f,15),.25f);
        for(int i=0;i<5;i++)Box("Hàng rào vườn Bà Lan",hub,new Vector3(-5.5f+i*.3f,.8f,8.5f),new Vector3(.15f,1.5f,.15f),wood);
        var lan=Box("Cửa Bà Lan",hub,new Vector3(-5,1,8.2f),new Vector3(1,2,.2f),wood);Interact(lan,DoiTuongHanhTrinh.Kind.Dialogue,"Bà Lan: Ở trong vườn này quen rồi cháu. Ngoài kia… thôi, đóng cổng lại cho yên.");
        var audioNames=new[]{"Phòng trọ","Gió mặt hồ","Chợ vắng","Đồng hồ lớp","Gió trên dốc"};journey.regionAudio=new AudioClip[5];
        for(int i=0;i<5;i++)journey.regionAudio[i]=Tone(audioNames[i],i==3?2:45+i*8,.025f,4,i);
        journey.ambience=Audio(root,"Ambience",true);journey.drone=Audio(root,"Drone",true);journey.drone.clip=Tone("Drone",48,.04f,4,0);
        journey.effectA=Audio(root,"SFX A",false);journey.effectB=Audio(root,"SFX B",false);journey.phoneTone=Tone("Thông báo",880,.12f,.16f,0);journey.woodTone=Tone("Gỗ va",160,.15f,.08f,1);
        cam.gameObject.AddComponent<UnityEngine.Rendering.Universal.UniversalAdditionalCameraData>().renderPostProcessing=true;
        root.AddComponent<HinhAnhTrangThaiMo>().droneMo=journey.drone;
        p.transform.SetPositionAndRotation(journey.hubSpawn.position,journey.hubSpawn.rotation);
        foreach(var region in journey.regions)region.SetActive(false);
        EditorSceneManager.MarkSceneDirty(scene);EditorSceneManager.SaveScene(scene,scenePath);
        // Unverified integration scene stays out of the build list. Current work
        // prioritizes the existing lake/gate/market map before broader gameplay.
        AssetDatabase.SaveAssets();
        File.WriteAllText("Docs/scene_assembly_current.json",new JObject{["scene"]=scenePath,["utc"]=DateTime.UtcNow.ToString("o"),["source_v30_preserved"]=true,["status"]="ASSEMBLED_NOT_PLAYTESTED"}.ToString());
    }
    static void PopulateMarket(Transform root)
    {
        var json=JObject.Parse(File.ReadAllText("Assets/WRouteC3Test_v30_20261016/route_fixture.json"));
        foreach(var item in json["meshes"])
        {
            var mesh=new Mesh{name=(string)item["name"],indexFormat=IndexFormat.UInt32};
            mesh.vertices=item["vertices"].Select(v=>new Vector3((float)v[0],(float)v[1],(float)v[2])).ToArray();mesh.triangles=item["triangles"].Select(v=>(int)v).ToArray();mesh.RecalculateNormals();mesh.RecalculateBounds();
            AssetDatabase.CreateAsset(mesh,Folder+"/Meshes/"+mesh.name.Replace('/','_')+".asset");
            var go=new GameObject(mesh.name);go.transform.SetParent(root,false);go.layer=3;go.AddComponent<MeshFilter>().sharedMesh=mesh;
            var c=item["color"];go.AddComponent<MeshRenderer>().sharedMaterial=Mat(mesh.name,new Color((float)c[0],(float)c[1],(float)c[2]));
            if((string)item["role"]!="text_visual_only")go.AddComponent<MeshCollider>().sharedMesh=mesh;
        }
        journey.regionSpawns[1]=Point("Market Spawn",root,new Vector3(783.37933f,6.55f,275.15897f),Quaternion.LookRotation(new Vector3(.2738386f,0,-.9617757f)));
        var books=Box("Giáo trình chưa mở",root,new Vector3(784.5f,6.05f,270),new Vector3(.6f,.15f,.4f),wood);Trace(1,books.transform);
    }
    static Material Mat(string name,Color color,float smooth=.15f)
    {
        string path=Folder+"/Materials/"+name.Replace('/','_')+".mat";
        var existing=AssetDatabase.LoadAssetAtPath<Material>(path);if(existing)return existing;
        var m=new Material(Shader.Find("Universal Render Pipeline/Lit")){name=name,color=color};m.SetFloat("_Smoothness",smooth);AssetDatabase.CreateAsset(m,path);return m;
    }
    static GameObject Box(string name,Transform parent,Vector3 position,Vector3 scale,Material mat,bool collider=true)
    {
        var go=GameObject.CreatePrimitive(PrimitiveType.Cube);go.name=name;go.transform.SetParent(parent,false);go.transform.position=position;go.transform.localScale=scale;go.layer=3;go.GetComponent<Renderer>().sharedMaterial=mat;if(!collider)UnityEngine.Object.DestroyImmediate(go.GetComponent<Collider>());return go;
    }
    static void SelectSource(Transform parent,Func<MeshFilter,bool> filter,Vector3 offset=default,int limit=10000)
    {
        foreach(var m in source.GetComponentsInChildren<MeshFilter>(true).Where(filter).Take(limit))
        {
            var go=UnityEngine.Object.Instantiate(m.gameObject);go.name=m.name;go.transform.SetParent(parent,false);go.transform.position=m.transform.position+offset;go.transform.rotation=m.transform.rotation;go.transform.localScale=m.transform.lossyScale;go.layer=3;
            foreach(var child in go.transform.Cast<Transform>().ToArray())UnityEngine.Object.DestroyImmediate(child.gameObject);
            foreach(var c in go.GetComponents<Collider>())UnityEngine.Object.DestroyImmediate(c);
            go.AddComponent<MeshCollider>().sharedMesh=m.sharedMesh;
            var renderer=go.GetComponent<Renderer>();if(renderer)renderer.sharedMaterials=renderer.sharedMaterials.Select(material=>material&&material.shader.name.StartsWith("Universal")?material:Mat(material?material.name:"Nguồn",material&&material.HasProperty("_Color")?material.color:new Color(.45f,.42f,.35f))).ToArray();
        }
    }
    static Transform Point(string name,Transform parent,Vector3 pos,Quaternion rot){var t=new GameObject(name).transform;t.SetParent(parent,false);t.SetPositionAndRotation(pos,rot);return t;}
    static Vector3 Supported(Vector3 p)
    {
        Physics.SyncTransforms();
        var hits=Physics.RaycastAll(p+Vector3.up*500,Vector3.down,1000,8,QueryTriggerInteraction.Ignore);
        var road=hits.Where(h=>h.collider.name=="Road"||h.collider.name=="GroundSlope").OrderByDescending(h=>h.point.y).ToArray();
        if(road.Length>0)p.y=road[0].point.y+.94f;
        else if(hits.Length>0)p.y=hits.Max(h=>h.point.y)+.94f;
        else throw new Exception("No physical support for authored spawn at "+p);
        return p;
    }
    static void Trace(int i,Transform t){journey.traces[i]=t;journey.traceMoved[i]=t;}
    static void Interact(GameObject go,DoiTuongHanhTrinh.Kind kind,string text=""){var c=go.AddComponent<DoiTuongHanhTrinh>();c.journey=journey;c.kind=kind;c.dialogue=text;}
    static void Label(string name,Transform parent,string text,Vector3 offset){var t=new GameObject(name);t.transform.SetParent(parent,false);t.transform.localPosition=offset;t.transform.localRotation=Quaternion.Euler(0,180,0);var label=t.AddComponent<TextMesh>();label.text=text;label.characterSize=.12f;label.anchor=TextAnchor.MiddleCenter;label.color=new Color(.85f,.8f,.65f);}
    static GameObject Puppet(Transform parent,Vector3 pos,float scale)
    {
        var root=new GameObject("Con Rối — khuôn mặt Minh");root.transform.SetParent(parent,false);root.transform.position=pos;root.transform.localScale=Vector3.one*scale;
        Box("Thân gỗ",root.transform,pos+Vector3.up*.85f*scale,new Vector3(.35f,.55f,.2f)*scale,wood,false);
        var head=Box("Đầu Minh",root.transform,pos+Vector3.up*1.3f*scale,new Vector3(.23f,.28f,.22f)*scale,plaster,false);
        // Use the same source face across all projections, never the ghost's identity.
        var sourceHead=source.GetComponentsInChildren<MeshFilter>(true).FirstOrDefault(m=>m.name=="ConRoi_Head_SameAsMinh");
        if(sourceHead){head.GetComponent<MeshFilter>().sharedMesh=sourceHead.sharedMesh;head.transform.rotation=sourceHead.transform.rotation;head.transform.localScale=Vector3.one*scale;}
        Box("Tay trái",root.transform,pos+new Vector3(-.25f,.85f,0)*scale,new Vector3(.08f,.6f,.08f)*scale,wood,false);
        Box("Tay phải",root.transform,pos+new Vector3(.25f,.85f,0)*scale,new Vector3(.08f,.6f,.08f)*scale,wood,false);
        for(int i=0;i<2;i++)Box("Chân",root.transform,pos+new Vector3(i==0?-.1f:.1f,.3f,0)*scale,new Vector3(.09f,.6f,.09f)*scale,wood,false);
        var hit=root.AddComponent<BoxCollider>();hit.center=new Vector3(0,.8f,0);hit.size=new Vector3(.6f,1.6f,.3f);
        return root;
    }
    static AudioSource Audio(GameObject root,string name,bool loop){var go=new GameObject(name);go.transform.SetParent(root.transform);var s=go.AddComponent<AudioSource>();s.loop=loop;s.playOnAwake=false;s.volume=.4f;return s;}
    static AudioClip Tone(string name,float hz,float volume,float duration,int noise)
    {
        // Original generated audio; no external licensed recordings or runtime disk dependency.
        int rate=22050,count=(int)(rate*duration);var clip=AudioClip.Create(name,count,1,rate,false);var data=new float[count];var rng=new System.Random(7+noise);
        for(int i=0;i<count;i++){float envelope=duration<1?Mathf.Exp(-i/(float)rate*35):1;data[i]=volume*envelope*(Mathf.Sin(2*Mathf.PI*hz*i/rate)*.6f+(float)(rng.NextDouble()*2-1)*(noise>0?.4f:.08f));}
        clip.SetData(data,0);
        // AudioClip native assets do not persist sample data reliably: write PCM WAV and import.
        string folder=Folder+"/Audio";Directory.CreateDirectory(folder);string path=folder+"/"+name+".wav";
        using(var writer=new BinaryWriter(File.Create(path))){writer.Write(System.Text.Encoding.ASCII.GetBytes("RIFF"));writer.Write(36+count*2);writer.Write(System.Text.Encoding.ASCII.GetBytes("WAVEfmt "));writer.Write(16);writer.Write((short)1);writer.Write((short)1);writer.Write(rate);writer.Write(rate*2);writer.Write((short)2);writer.Write((short)16);writer.Write(System.Text.Encoding.ASCII.GetBytes("data"));writer.Write(count*2);foreach(float v in data)writer.Write((short)(Mathf.Clamp(v,-1,1)*32767));}
        UnityEngine.Object.DestroyImmediate(clip);AssetDatabase.ImportAsset(path,ImportAssetOptions.ForceSynchronousImport);return AssetDatabase.LoadAssetAtPath<AudioClip>(path);
    }
    public static void BuildWindows()
    {
        var result=UnityEditor.BuildPipeline.BuildPlayer(new[]{Folder+"/ConRoi.unity"},"Builds/Windows/ConRoi.exe",BuildTarget.StandaloneWindows64,BuildOptions.None);
        File.WriteAllText("Docs/build_current.json",new JObject{["result"]=result.summary.result.ToString(),["errors"]=result.summary.totalErrors,["warnings"]=result.summary.totalWarnings,["path"]=result.summary.outputPath}.ToString());
    }
}
