// Điều phối vùng, tương tác, điện thoại, pause và save/load; các chương trường/dốc sở hữu tiến trình cục bộ.
// Component trong scene r92: Lake_Market_Story_GDD21.
// Reference public để đối chiếu Inspector (có nhánh tùy chọn): player, hub, lake, market, schoolChapter, slopeChapter, presentation, polish, completion, hubSpawn, lakeSpawn, marketSpawn, traces, ghosts.
// Code tự tìm/tạo: HuongDanHanhTrinhV48, PhongTrongMoV47, DienXuatHanhTrinhV45, ManHinhTaiV44, DieuChinhHanhTrinhV43, TraiNghiemHanhTrinhV42, AudioSource, Volume.
// Update bỏ qua khi Loading/MenuOpen; xử lý save/load trước nhánh return khi pause.
// State JSON version 1 và tên field giữ tương thích; profile chọn trước Awake, test dùng --r30-review-output riêng.
using System;
using System.IO;
using System.Linq;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

// Regional continuation. Later school/slope chapters are deliberately not endings here.
[UnityEngine.Scripting.APIUpdating.MovedFrom(true, null, "Assembly-CSharp", "LakeMarketStory")]
public sealed class DieuPhoiHanhTrinh : MonoBehaviour
{
    [Serializable] public class State
    {
        public int version=1, nextRegion, region=-1, awareness, actionPoints, interactions, ignored;
        public int mapLayoutRevision;
        // Optional presentation memory; older version-1 saves default to zero.
        // This never participates in gates, awareness or ending selection.
        public int experienceR22Flags;
        public bool[] traces=new bool[2], faces=new bool[2], witnessed=new bool[2], posted=new bool[3], called=new bool[3], dismissed=new bool[3];
        public bool schoolTrace, schoolFace, schoolWitnessed;
        public bool schoolLeading, cameraSaved;
        public float schoolLeadElapsed;
        // Optional r30 staging snapshot; absent in older version-1 saves.
        public string schoolStagingR30="";
        public string class203R32="";
        public string marketPackagesR35="";
        public string marketPreparationR88="";
        public string lakeApproachR89="",schoolApproachR89="";
        public string lakeNotebookR37="";
        public string slopePreparationsR41="";
        public int schoolTalks, schoolReadMask;
        public bool slopeTrace, slopeFace, slopeWitnessed, finalStarted;
        public float finalElapsed,slopeRollTime;
        public int finalStarts, slopeTalks, slopeReadMask, compliance, endingBeat;
        public string ending="";
        public int dreamCycles,vyLastCycle,vyMessages;
        public int baoTalks,haTalks,lanVisitMask;
        public bool vyEncountered;
        public string npcPerformance="";
        public string dreamRoom="";
        public bool vyMessageAvailable=true;
        public string vyMessage="";
        public Quaternion cameraRotation=Quaternion.identity;
        public Vector3[] puppetPositions;
        public Vector3 position; public Quaternion rotation=Quaternion.identity;
    }
    public DieuKhienDiChuyen player;
    public GameObject hub, lake, market;
    public ChuongTruongHocV37 schoolChapter;
    public ChuongDocV39 slopeChapter;
    public TrinhBayHanhTrinhV39 presentation;
    public HoanThienTrinhBayV40 polish;
    public HoanTatHanhTrinhV41 completion;
    public bool nightReadabilityR91;
    public int RegionCount=>slopeChapter?4:schoolChapter?3:2;
    public Transform hubSpawn, lakeSpawn, marketSpawn;
    public Transform[] traces, ghosts;
    public DieuKhienConRoi[] puppets;
    public AudioClip[] ambience;
    public AudioClip wood, notification;
    public AudioClip drone;
    public Transform clockHand;
    public State data=new State();
    public string SavePathOverride {get;set;}
    public bool PhoneOpen {get;private set;}
    public bool Paused {get;private set;}
    public bool MenuOpen {get;private set;}
    public bool SupplementInspection {get;private set;}
    public void SetSupplementInspection(bool value){SupplementInspection=value;NotifyInteraction();Controls();}
    public void RefreshControls(){Controls();}
    public void ClosePhone(){PhoneOpen=false;NotifyInteraction();Controls();}
    public void SetMenu(bool value){MenuOpen=value;NotifyInteraction();Controls();}
    public string Dialogue {get;private set;}="";
    public int Interactions {get;private set;}
    public int WoodEffectsPlayed {get;private set;}
    Vector3[] traceRest;
    Vector3[] puppetRest;
    int regionEpoch;
    InputAction pause, phone, post, call, ignore, save, load;
    AudioSource background;
    AudioSource droneSource;
    AudioSource[] effects;
    float awakeAt, dialogueUntil;
    float pauseStarted;
    float idleTime;
    PhongTrongMoV47 dreamRoom;
    public bool DreamHub=>dreamRoom&&dreamRoom.DreamHub;
    public void NotifyInteraction(){awakeAt=Time.realtimeSinceStartup;}
    Light sun; float daylight; Material sky;
    Color ambientSky,ambientEquator,ambientGround;
    Volume dreamVolume;
    bool originalFog;Color originalFogColor;FogMode originalFogMode;float originalFogStart,originalFogEnd;
    string PathName=>string.IsNullOrEmpty(SavePathOverride)?Path.Combine(Application.persistentDataPath,GetComponent<HuongDanHanhTrinhV48>()?"journey-v48.json":GetComponent<PhongTrongMoV47>()?"journey-v47.json":GetComponent<DienXuatHanhTrinhV45>()?"journey-v45.json":GetComponent<ManHinhTaiV44>()?"journey-v44.json":GetComponent<DieuChinhHanhTrinhV43>()?"journey-v43.json":GetComponent<TraiNghiemHanhTrinhV42>()?"journey-v42.json":completion?"journey-v41.json":polish?"journey-v40.json":"lake-market-v34.json"):SavePathOverride;
    void Awake()
    {
        dreamRoom=GetComponent<PhongTrongMoV47>();
        traceRest=new Vector3[traces.Length];for(int i=0;i<traces.Length;i++)traceRest[i]=traces[i].localPosition;
        puppetRest=new Vector3[puppets.Length];for(int i=0;i<puppets.Length;i++)puppetRest[i]=puppets[i].transform.position;
        if(slopeChapter){Array.Resize(ref data.posted,4);Array.Resize(ref data.called,4);Array.Resize(ref data.dismissed,4);}
        background=gameObject.AddComponent<AudioSource>();background.loop=true;background.volume=.28f;
        droneSource=gameObject.AddComponent<AudioSource>();droneSource.clip=drone;droneSource.loop=true;droneSource.volume=.12f;
        sun=RenderSettings.sun;daylight=sun?sun.intensity:1;sky=RenderSettings.skybox;ambientSky=RenderSettings.ambientSkyColor;ambientEquator=RenderSettings.ambientEquatorColor;ambientGround=RenderSettings.ambientGroundColor;
        originalFog=RenderSettings.fog;originalFogColor=RenderSettings.fogColor;originalFogMode=RenderSettings.fogMode;originalFogStart=RenderSettings.fogStartDistance;originalFogEnd=RenderSettings.fogEndDistance;
        dreamVolume=gameObject.AddComponent<Volume>();dreamVolume.isGlobal=true;dreamVolume.priority=10;dreamVolume.profile=ScriptableObject.CreateInstance<VolumeProfile>();var grade=dreamVolume.profile.Add<ColorAdjustments>();grade.saturation.Override(-30);
        var cameraData=player.GetComponentInChildren<Camera>().GetUniversalAdditionalCameraData();cameraData.renderPostProcessing=true;
        effects=new AudioSource[2];for(int i=0;i<2;i++){var voice=new GameObject("NarrativeVoice_"+i);voice.transform.SetParent(transform,false);effects[i]=voice.AddComponent<AudioSource>();}
        pause=Action("Pause","escape");phone=Action("Phone","p");post=Action("Post","digit1");call=Action("Call","digit2");ignore=Action("Ignore","digit3");save=Action("Save","f5");load=Action("Load","f6");
        ApplyRegion(true);
    }
    InputAction Action(string name,string key){if(key.StartsWith("digit"))key=key.Substring(5);var a=new InputAction(name,InputActionType.Button,"<Keyboard>/"+key);a.Enable();return a;}
    void OnDestroy(){foreach(var a in new[]{pause,phone,post,call,ignore,save,load})a?.Dispose();Time.timeScale=1;AudioListener.pause=false;}
    void Update()
    {
        if(Loading||MenuOpen)return;
        if(pause.WasPressedThisFrame()){if(Paused)awakeAt+=Time.realtimeSinceStartup-pauseStarted;else pauseStarted=Time.realtimeSinceStartup;Paused=!Paused;Controls();}
        if(save.WasPressedThisFrame())Save();if(load.WasPressedThisFrame())Load();
        if(Paused)return;
        if(polish&&polish.IsReading){NotifyInteraction();return;}
        if(clockHand&&data.region<0&&!DreamHub)clockHand.Rotate(0,0,-Time.deltaTime*6,Space.Self);
        if(DreamHub){if(dreamRoom.Advance()){dreamRoom.End();if(polish)polish.Event("dream_room_end","automatic continuation; no extra task");EnterDreamRegionV47();}return;}
        if(!SupplementInspection&&(data.region<0||GetComponent<HuongDanHanhTrinhV48>())&&string.IsNullOrEmpty(data.ending)&&phone.WasPressedThisFrame()){PhoneOpen=!PhoneOpen;NotifyInteraction();Controls();}
        if(PhoneOpen){if(post.WasPressedThisFrame())PhoneChoice(0);if(call.WasPressedThisFrame())PhoneChoice(1);if(ignore.WasPressedThisFrame())PhoneChoice(2);return;}
        if(!string.IsNullOrEmpty(data.ending))return;
        if(data.region<0){if(!SupplementInspection&&data.nextRegion<RegionCount&&Time.realtimeSinceStartup-awakeAt>=90)Sleep();return;}
        int r=data.region;
        if(r>=2)return; // Optional later chapters own their local progression.
        var camera=player.GetComponentInChildren<Camera>();
        foreach(var ai in puppets)
        {
            if(GetComponent<MoDauBenHoR23>())break;
            if(!ai.gameObject.activeInHierarchy||!ai.HasWitnessedPhase)continue;
            var target=ai.dauTransform?ai.dauTransform.position:ai.transform.position+Vector3.up;
            var view=camera.WorldToViewportPoint(target);var delta=target-camera.transform.position;
            if(view.z>0&&view.x>0&&view.x<1&&view.y>0&&view.y<1&&!Physics.Raycast(camera.transform.position,delta.normalized,delta.magnitude-.2f,8))data.witnessed[r]=true;
        }
        idleTime=player.LastMovementInput.sqrMagnitude<.01f?idleTime+Time.deltaTime:0;
        if(!presentation&&data.nextRegion>0&&idleTime>4&&idleTime<4.2f)player.transform.Rotate(0,Time.deltaTime*3,0);
        var r23=GetComponent<MoDauBenHoR23>();
        if(!data.traces[r]&&(!r23||r23.TraceReady(traces[r]))&&Vector3.Distance(player.transform.position,traces[r].position)<2.4f)
        {data.traces[r]=true;data.awareness++;ApplyTraces();if(!r23)StartCoroutine(TraceNudge(traces[r]));}
    }
    IEnumerator TraceNudge(Transform target)
    {
        int epoch=regionEpoch;
        // Small additive nudge; never disable or replace the live input actions.
        var eye=player.GetComponentInChildren<Camera>().transform;
        for(float t=0;t<.18f;t+=Time.deltaTime){if(epoch!=regionEpoch)yield break;var direction=target.position-eye.position;if(direction.sqrMagnitude>.01f)eye.rotation=Quaternion.RotateTowards(eye.rotation,Quaternion.LookRotation(direction),Time.deltaTime*9);yield return null;}
    }
    public bool UiInputBlocked=>Time.frameCount==uiChangedFrame;
    int uiChangedFrame=-1;
    void Controls()
    {
        uiChangedFrame=Time.frameCount;
        bool free=!MenuOpen&&!Loading&&!Paused&&!PhoneOpen&&!SupplementInspection&&! (polish&&polish.IsReading)&&string.IsNullOrEmpty(data.ending);player.enabled=free;
        var mouse=player.GetComponentInChildren<DieuKhienGocNhin>();if(!mouse)mouse=player.GetComponent<DieuKhienGocNhin>();if(mouse)mouse.enabled=free;
        var interaction=player.GetComponent<TuongTacNguoiChoi>();if(interaction)interaction.enabled=free;
        AudioListener.pause=MenuOpen||Paused||Loading;
        Time.timeScale=MenuOpen||Paused||Loading?0:1;Cursor.lockState=free?CursorLockMode.Locked:CursorLockMode.None;Cursor.visible=!free;
    }
    public bool Loading {get;private set;}
    public int SuccessfulLoads {get;private set;}
    public void SetLoading(bool value){Loading=value;Controls();}
    public void OpenPhone(){if((data.region<0||GetComponent<HuongDanHanhTrinhV48>())&&!DreamHub&&string.IsNullOrEmpty(data.ending)){PhoneOpen=true;NotifyInteraction();Controls();}}
    public void Sleep()
    {
        if(Paused||DreamHub||data.region>=0||data.nextRegion>=RegionCount||!string.IsNullOrEmpty(data.ending))return;
        bool message=polish&&polish.PrepareSleep();PhoneOpen=false;
        if(dreamRoom){var acting=GetComponent<DienXuatHanhTrinhV45>();if(acting)data.npcPerformance=acting.Capture();dreamRoom.Begin();if(polish)polish.Event("dream_room_begin","same room, repaired strings, stopped clock, dream audio/grade; automatic 3 second transition");ApplyRegion(false);if(message)PlayEffect(notification);Save();return;}
        EnterDreamRegionV47();if(message)PlayEffect(notification);
    }
    void EnterDreamRegionV47(){data.region=data.nextRegion;ApplyRegion(true);Save();}
    public void Wake(){if(Paused||data.region<0)return;if(data.region==3&&slopeChapter){slopeChapter.BeginFinal();return;}data.nextRegion=Math.Max(data.nextRegion,data.region+1);if(presentation)data.compliance=Mathf.Clamp(data.nextRegion-data.awareness,0,2);data.region=-1;ApplyRegion(true);Save();}
    public static string ResolveEnding(int awareness,int interactions)=>awareness>=3?(interactions>=2?"C":"B"):(interactions>=2?"A2":"A");
    public void Restart(){if(dreamRoom)dreamRoom.End();data=new State();if(slopeChapter){Array.Resize(ref data.posted,4);Array.Resize(ref data.called,4);Array.Resize(ref data.dismissed,4);}ApplyRegion(true);Save();}
    public void FinishJourney(){if(data.region!=3||!data.finalStarted)return;data.ending=ResolveEnding(data.awareness,data.interactions);data.nextRegion=4;data.region=-1;ApplyRegion(true);Save();}
    static readonly Unity.Profiling.ProfilerMarker applyMarkerV47=new Unity.Profiling.ProfilerMarker("V47.Story.ApplyRegion");
    void ApplyRegion(bool relocate){using(applyMarkerV47.Auto())ApplyRegionMeasuredV47(relocate);}
    void ApplyRegionMeasuredV47(bool relocate)
    {
        if(polish)polish.DismissReading();
        regionEpoch++;Dialogue="";dialogueUntil=0;SupplementInspection=false;
        foreach(var effect in effects)effect.Stop();
        hub.SetActive(data.region<0);lake.SetActive(data.region==0);market.SetActive(data.region==1);
        if(schoolChapter)schoolChapter.Apply(data.region==2);
        if(slopeChapter)slopeChapter.Apply(data.region==3);
        for(int i=0;i<puppets.Length;i++){var ai=puppets[i];bool active=ai.transform.IsChildOf(data.region==0?lake.transform:market.transform)&&(data.region==0||data.region==1);ai.gameObject.SetActive(active);ai.transform.position=!relocate&&data.puppetPositions?.Length==puppets.Length?data.puppetPositions[i]:puppetRest[i];ai.SetPhase(data.region==0?DieuKhienConRoi.GiaiDoan.GD1:DieuKhienConRoi.GiaiDoan.GD2);}
        if(relocate){var spawn=data.region==0?lakeSpawn:data.region==1?marketSpawn:data.region==2&&schoolChapter?schoolChapter.spawn:data.region==3&&slopeChapter?slopeChapter.spawn:hubSpawn;Place(spawn.position,spawn.rotation);if(data.region==0)GetComponent<MoDauBenHoR23>()?.BeginLakeEntry();}
        awakeAt=Time.realtimeSinceStartup;PhoneOpen=false;Paused=false;Controls();ApplyTraces();
        dreamVolume.weight=data.region>=0||DreamHub?1:0;idleTime=0;
        if(data.region>=0||DreamHub){if(!droneSource.isPlaying)droneSource.Play();}else droneSource.Stop();
        if(dreamRoom)dreamRoom.Apply();
        bool night=data.region==1;RenderSettings.skybox=night?null:sky;if(sun)sun.intensity=night?(nightReadabilityR91?.22f:.08f):daylight;
        RenderSettings.ambientSkyColor=night?(nightReadabilityR91?new Color(.22f,.25f,.32f):new Color(.045f,.06f,.095f)):ambientSky;RenderSettings.ambientEquatorColor=night?(nightReadabilityR91?new Color(.14f,.16f,.20f):new Color(.035f,.035f,.045f)):ambientEquator;RenderSettings.ambientGroundColor=night?(nightReadabilityR91?new Color(.07f,.08f,.10f):new Color(.018f,.02f,.025f)):ambientGround;
        var camera=player.GetComponentInChildren<Camera>();camera.backgroundColor=new Color(.012f,.018f,.03f);camera.clearFlags=night?CameraClearFlags.SolidColor:CameraClearFlags.Skybox;
        var dalatMist=GetComponent<SuongDaLatV48>();
        if(dalatMist)dalatMist.ApplyRegion();
        else if(slopeChapter){bool mist=data.region>=2;camera.farClipPlane=mist?90:1000;RenderSettings.fog=mist||originalFog;RenderSettings.fogColor=mist?new Color(.37f,.40f,.40f):originalFogColor;RenderSettings.fogMode=mist?FogMode.Linear:originalFogMode;RenderSettings.fogStartDistance=mist?25:originalFogStart;RenderSettings.fogEndDistance=mist?80:originalFogEnd;}
        for(int i=0;i<ghosts.Length;i++){var face=ghosts[i].GetComponent<HienGuongMatTheoKhu>();if(face)face.Restore(data.faces[i]);}
        int index=data.region+1;if(ambience!=null&&index<ambience.Length){background.clip=ambience[index];background.Play();}
        if(presentation)presentation.Apply();
        var acting=GetComponent<DienXuatHanhTrinhV45>();if(acting)acting.Applied(relocate);
    }
    public void Place(Vector3 position,Quaternion rotation)
    {
        // One transition/load placement, never a per-frame movement correction.
        player.controller.enabled=false;player.transform.SetPositionAndRotation(position,rotation);player.ResetVerticalMotion();player.controller.enabled=true;Physics.SyncTransforms();
    }
    void ApplyTraces(){for(int i=0;i<traces.Length;i++)traces[i].localPosition=traceRest[i]+(data.traces[i]?traces[i].parent.InverseTransformVector(new Vector3(.1f,-.12f,.08f)):Vector3.zero);}
    public void PhoneChoice(int choice)
    {
        if(!PhoneOpen||data.region>=0||!string.IsNullOrEmpty(data.ending))return;int n=Mathf.Clamp(data.nextRegion,0,data.posted.Length-1);
        NotifyInteraction();
        if(choice==0&&!data.posted[n]){data.posted[n]=true;data.actionPoints++;data.interactions++;Say(polish?"Đã ghi lại giấc mơ.":"Đã ghi lại giấc mơ. Vy: Cậu lại nhớ được gì à?");}
        if(choice==1&&!data.called[n]){data.called[n]=true;data.actionPoints+=3;data.interactions++;Say("Mẹ: Ở trên đó nhớ ăn uống cho đủ nhé con.");}
        if(choice==2&&!data.dismissed[n]&&(!polish||polish.HasMessage)){data.dismissed[n]=true;data.actionPoints--;data.ignored++;data.vyMessageAvailable=false;Say("Điện thoại im lặng.");}Save();
    }
    public void Talk(int region)
    {
        Interactions++;if(completion){var line=completion.NextTalk(region);if(line!=null)Say(line);Save();}
        else Say(region==0?"Ông Bảo: Ngày nào bác cũng ra đây. Thành quen rồi cháu.":"Chị Hà: Hồi đó ai cũng bảo học ngành ấy sẽ ổn.");
        // Actual on-screen witnessing is checked separately from AI movement telemetry.
        if(data.witnessed[region]&&!data.faces[region]){data.faces[region]=true;var face=ghosts[region].GetComponent<HienGuongMatTheoKhu>();if(face)face.Reveal();PlayEffect(wood);}
    }
    public void Say(string text){Dialogue=text;dialogueUntil=Time.unscaledTime+5;}
    public void PlayEffectAt(AudioClip clip,Vector3 position) { PlayEffectInternal(clip,position,true); }
    public void PlayEffect(AudioClip clip) { PlayEffectInternal(clip,Vector3.zero,false); }
    public AudioSource PlayContactFootstep(AudioClip clip,Vector3 position)
    {
        if(!clip)return null;foreach(var voice in effects)if(!voice.isPlaying){voice.transform.position=position;voice.spatialBlend=1;voice.minDistance=1;voice.maxDistance=12;voice.rolloffMode=AudioRolloffMode.Linear;voice.clip=clip;voice.volume=.18f;voice.Play();return voice;}return null;
    }
    void PlayEffectInternal(AudioClip clip,Vector3 position,bool spatial)
    {
        if(!clip)return;
        AudioSource target=null;foreach(var source in effects)if(!source.isPlaying){target=source;break;}
        // Narrative sync cues take precedence over a footstep/wire accent, still using two voices.
        if(!target&&(clip==wood||clip==notification)){target=effects[0];target.Stop();}
        if(target){target.clip=clip;target.volume=1;target.transform.position=position;target.spatialBlend=spatial?1:0;target.minDistance=2;target.maxDistance=18;target.rolloffMode=AudioRolloffMode.Linear;target.PlayOneShot(clip,.4f);if(clip==wood)WoodEffectsPlayed++;}
    }
    static readonly Unity.Profiling.ProfilerMarker saveMarkerV47=new Unity.Profiling.ProfilerMarker("V47.Story.Save");
    public void Save(){using(saveMarkerV47.Auto())SaveMeasuredV47();}
    void SaveMeasuredV47()
    {
        var recovery=GetComponent<PhucHoiTaiBienR21>();if(recovery&&!recovery.AllowSave())return;
        var bridgeLayout=GetComponent<TuTheBaoBenCauV48>();if(bridgeLayout)bridgeLayout.PrepareSave();
        if(dreamRoom)data.dreamRoom=dreamRoom.Capture();
        var acting=GetComponent<DienXuatHanhTrinhV45>();if(acting)data.npcPerformance=acting.Capture();
        if(schoolChapter)schoolChapter.Capture();
        GetComponent<TuongTacGiayLop203R32>()?.Capture();
        GetComponent<QuanLyGoiHangChoR35>()?.Capture();
        GetComponent<ChuanBiGoiHangR88>()?.Capture();
        GetComponent<TuongTacSoTayHoR37>()?.Capture();
        foreach(var approach in GetComponents<TiepCanCoChuDichR89>())approach.Capture();
        GetComponent<ChuanBiHanhLyR41>()?.Capture();
        if(slopeChapter)slopeChapter.Capture();
        data.position=player.transform.position;data.rotation=player.transform.rotation;data.cameraRotation=player.GetComponentInChildren<Camera>().transform.localRotation;data.cameraSaved=true;
        data.puppetPositions=new Vector3[puppets.Length];for(int i=0;i<puppets.Length;i++)data.puppetPositions[i]=puppets[i].transform.position;
        try{Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(PathName)));string temp=PathName+".tmp";File.WriteAllText(temp,JsonUtility.ToJson(data,true));File.Copy(temp,PathName,true);File.Delete(temp);}
        catch(Exception e)when(e is IOException||e is UnauthorizedAccessException){Say("Không thể lưu trò chơi. "+e.Message);}
    }
    static bool Finite(float v)=>!float.IsNaN(v)&&!float.IsInfinity(v);
    static bool Valid(Vector3 v)=>Finite(v.x)&&Finite(v.y)&&Finite(v.z);
    static bool Valid(Quaternion q)=>Finite(q.x)&&Finite(q.y)&&Finite(q.z)&&Finite(q.w)&&Quaternion.Dot(q,q)>.01f;
    static readonly Unity.Profiling.ProfilerMarker loadMarkerV47=new Unity.Profiling.ProfilerMarker("V47.Story.Load");
    public void Load(){using(loadMarkerV47.Auto())LoadMeasuredV47();}
    void LoadMeasuredV47()
    {
        if(!File.Exists(PathName))return;
        try
        {
            var loaded=JsonUtility.FromJson<State>(File.ReadAllText(PathName));
            if(loaded==null||loaded.version!=1||loaded.region< -1||loaded.region>=RegionCount||loaded.nextRegion<0||loaded.nextRegion>RegionCount||loaded.traces?.Length!=2||loaded.faces?.Length!=2||loaded.witnessed?.Length!=2||loaded.posted==null||loaded.called==null||loaded.dismissed==null||loaded.posted.Length<3||loaded.posted.Length>4||loaded.called.Length!=loaded.posted.Length||loaded.dismissed.Length!=loaded.posted.Length||!Valid(loaded.position)||!Valid(loaded.rotation)||!Finite(loaded.schoolLeadElapsed)||!Finite(loaded.finalElapsed)||loaded.finalElapsed<0||loaded.finalStarts<0||loaded.finalStarts>1||(!string.IsNullOrEmpty(loaded.ending)&&(loaded.region!=-1||loaded.nextRegion!=4||!new[]{"A","A2","B","C"}.Contains(loaded.ending))))throw new InvalidDataException("Trạng thái lưu không hợp lệ.");
            if(slopeChapter){Array.Resize(ref loaded.posted,4);Array.Resize(ref loaded.called,4);Array.Resize(ref loaded.dismissed,4);}
            if(loaded.puppetPositions!=null)foreach(var p in loaded.puppetPositions)if(!Valid(p))throw new InvalidDataException("Vị trí con rối không hợp lệ.");
            if(!PhongTrongMoV47.Valid(loaded.dreamRoom)||(PhongTrongMoV47.HasDream(loaded.dreamRoom)&&(loaded.region!=-1||loaded.nextRegion>=RegionCount||!string.IsNullOrEmpty(loaded.ending))))throw new InvalidDataException("Trạng thái phòng trọ Mơ không hợp lệ.");
            if(dreamRoom)dreamRoom.Restore(loaded.dreamRoom);
            var bridgeLayout=GetComponent<TuTheBaoBenCauV48>();if(bridgeLayout)bridgeLayout.MigrateLoaded(loaded);
            data=loaded;ApplyRegion(false);Place(data.position,data.rotation);SuccessfulLoads++;
            var routeGrade=GetComponent<KhoiPhucCaoDoSauTaiR73>();if(routeGrade)routeGrade.ReseatAfterLoad();
            var recovery=GetComponent<PhucHoiTaiBienR21>();if(recovery)recovery.AfterLoad();
            if(data.cameraSaved&&Valid(data.cameraRotation)){player.GetComponentInChildren<Camera>().transform.localRotation=data.cameraRotation;var look=player.GetComponentInChildren<DieuKhienGocNhin>();if(look)look.RestorePitch(data.cameraRotation.eulerAngles.x);}
        }
        catch(Exception e)when(e is IOException||e is InvalidDataException||e is ArgumentException||e is UnauthorizedAccessException){Say("Không thể tải trò chơi. "+e.Message);}
    }
    void OnGUI()
    {
        if(polish)return;
        var helpStyle=new GUIStyle(GUI.skin.box){fontSize=Screen.width<650?11:14,wordWrap=true,alignment=TextAnchor.UpperLeft};
        GUI.Box(new Rect(15,15,Mathf.Min(570,Screen.width-30),60),"WASD: di chuyển · Chuột: nhìn · E: tương tác\nP: điện thoại ở phòng · Esc: tạm dừng · F5/F6: lưu/tải",helpStyle);
        if(Time.unscaledTime<dialogueUntil)GUI.Box(new Rect(Screen.width*.2f,Screen.height-110,Screen.width*.6f,70),Dialogue);
        if(PhoneOpen)GUI.Box(new Rect(Screen.width/2-240,Screen.height/2-100,480,200),"ĐẰNG SAU GIẤC NGỦ\nVy: Nếu nhớ được thì kể mình nghe nhé.\n\n1 · Ghi và đăng giấc mơ\n2 · Gọi về nhà\n3 · Bỏ qua tin nhắn\nP · Cất điện thoại");
        if(Paused)GUI.Box(new Rect(Screen.width/2-160,Screen.height/2-50,320,100),"TẠM DỪNG\nEsc · Tiếp tục\nF5 · Lưu / F6 · Tải");
        if(data.region<0&&data.nextRegion>=2&&string.IsNullOrEmpty(data.ending))GUI.Box(new Rect(15,80,490,45),schoolChapter?(data.nextRegion>=3?(slopeChapter?"Đã trở về từ trường. Ngủ để đến dốc Đà Lạt.":"Đã trở về từ trường. Đợt tiếp theo: dốc Đà Lạt."):"Đã trở về từ chợ. Ngủ để đến trường đại học."):"Đã trở về từ chợ. Đợt tiếp theo: khu trường đại học.");
    }
}
