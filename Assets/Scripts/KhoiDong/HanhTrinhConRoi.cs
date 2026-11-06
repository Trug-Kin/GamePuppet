// Luồng hành trình legacy dùng reference scene serialized, không sử dụng fixture di chuyển.
// Không gắn trực tiếp trong r92; checkpoint ConRoi.unity: "CON R\u1ED0I \u2014 GDD v2.1".
// Reference public để đối chiếu Inspector (có nhánh tùy chọn): player, hub, regions, hubSpawn, regionSpawns, puppets, traces, traceMoved, sun, ambience, drone, effectA, effectB, regionAudio.
// Code tự tìm/tạo: TienTrinhTroChoi, QuanLyTrangThaiGame, Camera, DieuKhienGocNhin, TuongTacNguoiChoi.
using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;

// Production flow: serialized scene references only; no movement fixture or recorder.
[UnityEngine.Scripting.APIUpdating.MovedFrom(true, null, "Assembly-CSharp", "ConRoiJourney")]
public sealed class HanhTrinhConRoi : MonoBehaviour
{
    public DieuKhienDiChuyen player;
    public GameObject hub;
    public GameObject[] regions;
    public Transform hubSpawn;
    public Transform[] regionSpawns;
    public DieuKhienConRoi[] puppets;
    public Transform[] traces;
    public Transform[] traceMoved;
    public Light sun;
    public AudioSource ambience, drone, effectA, effectB;
    public AudioClip[] regionAudio;
    public AudioClip phoneTone, woodTone;
    public bool InMenu {get; private set;}=true;
    public bool Paused {get; private set;}
    public bool PhoneOpen {get; private set;}
    public bool[] Found=new bool[4];
    public bool[] FaceSeen=new bool[4];
    public int cycle;
    public bool dreaming;
    public string ending;
    bool[] posted=new bool[5], called=new bool[5], ignored=new bool[5];
    bool vySeen, finalStarted;
    int lastMessageCycle=-4;
    float idle, finalTimer;
    string caption="", notice="";
    float captionUntil;
    string SavePath=>Path.Combine(Application.persistentDataPath,"conroi-save-v1.json");
    [Serializable] public class SaveData
    {
        public int version=1,cycle,lastMessageCycle;
        public bool dreaming,vySeen,finalStarted;
        public bool[] found,faceSeen,posted,called,ignored;
        public string ending;
        public Vector3 position;
        public Quaternion rotation;
        public TienTrinhTroChoi.Snapshot progress;
    }
    void Awake()
    {
        if(!TienTrinhTroChoi.Instance)new GameObject("GameProgress").AddComponent<TienTrinhTroChoi>();
        if(!QuanLyTrangThaiGame.Instance)new GameObject("ConRoiGameManager").AddComponent<QuanLyTrangThaiGame>();
    }
    void Start(){ShowWorld(); Controls(false);}
    void Update()
    {
        if(Keyboard.current!=null && Keyboard.current.escapeKey.wasPressedThisFrame && !InMenu)
        { if(PhoneOpen){PhoneOpen=false;Controls(!Paused);}else TogglePause(); }
        if(InMenu||Paused||!string.IsNullOrEmpty(ending))return;
        idle+=Time.deltaTime;
        if(player.LastMovementInput.sqrMagnitude>.01f || player.LookInput.sqrMagnitude>.01f)idle=0;
        if(!dreaming && idle>=90)Sleep();
        if(dreaming && cycle<4)
        {
            if(!Found[cycle] && traces[cycle] && Vector3.Distance(player.transform.position,traces[cycle].position)<2.4f)
            {
                Found[cycle]=true;TienTrinhTroChoi.Instance.PhatHienDauVet();
                ApplyTraces();Sfx(woodTone);
                StartCoroutine(Glance(traces[cycle].position));
            }
            if(puppets[cycle] && puppets[cycle].HasWitnessedPhase && !FaceSeen[cycle])
            {
                var eye=player.GetComponentInChildren<Camera>();
                var d=puppets[cycle].transform.position-eye.transform.position;
                if(d.magnitude<12 && Vector3.Dot(eye.transform.forward,d.normalized)>.6f)
                {FaceSeen[cycle]=true;Sfx(woodTone);}
            }
        }
        if(finalStarted && dreaming)
        {finalTimer+=Time.deltaTime;if(finalTimer>=8)Finish();}
    }
    IEnumerator Glance(Vector3 point)
    {
        var camera=player.GetComponentInChildren<Camera>().transform;
        var rest=camera.localRotation;
        for(float t=0;t<.35f;t+=Time.deltaTime)
        {
            if(!dreaming||Paused)break;
            var world=Quaternion.LookRotation(point-camera.position);
            camera.rotation=Quaternion.Slerp(player.transform.rotation*rest,world,Mathf.Sin(t/.35f*Mathf.PI)*.12f);
            yield return null;
        }
        camera.localRotation=rest;
    }
    public void NewGame()
    {
        TienTrinhTroChoi.Instance.ResetProgress();Found=new bool[4];FaceSeen=new bool[4];
        posted=new bool[5];called=new bool[5];ignored=new bool[5];
        cycle=0;dreaming=false;ending=null;vySeen=false;finalStarted=false;finalTimer=0;lastMessageCycle=-4;
        InMenu=false;Paused=false;PhoneOpen=false;Time.timeScale=1;
        QuanLyTrangThaiGame.Instance.RestoreState(0,QuanLyTrangThaiGame.TrangThai.Thuc);
        ShowWorld();Place(hubSpawn.position,hubSpawn.rotation);Controls(true);Say("Vy: Thử ghi lại giấc mơ đi Minh. Tớ gửi cách nhận biết mình đang mơ rồi đó.",10);
    }
    public void Sleep()
    {
        if(dreaming||InMenu||Paused||cycle>=4)return;
        int posts=0;foreach(bool p in posted)if(p)posts++;
        int frequency=posts>=2?1:posts>0?2:3;
        if(cycle-lastMessageCycle>=frequency)
        {Say("Vy: Cậu còn ghi nhật ký giấc mơ chứ? Có gì kể tớ nghe nhé.",8);lastMessageCycle=cycle;Sfx(phoneTone);}
        dreaming=true;PhoneOpen=false;idle=0;
        QuanLyTrangThaiGame.Instance.RestoreState(cycle,QuanLyTrangThaiGame.TrangThai.Mo);
        ShowWorld();Place(regionSpawns[cycle].position,regionSpawns[cycle].rotation);
        if(puppets[cycle])puppets[cycle].SetPhase((DieuKhienConRoi.GiaiDoan)Mathf.Min(cycle,2));
        Controls(true);Save();
    }
    public void Wake()
    {
        if(!dreaming||finalStarted)return;
        if(cycle==3)
        {
            finalStarted=true;finalTimer=0;
            if(puppets[3])puppets[3].ChuyenSangGD4();
            return;
        }
        cycle++;dreaming=false;idle=0;
        QuanLyTrangThaiGame.Instance.RestoreState(cycle,QuanLyTrangThaiGame.TrangThai.Thuc);
        ShowWorld();Place(hubSpawn.position,hubSpawn.rotation);Controls(true);Save();
    }
    void Finish()
    {
        dreaming=false;cycle=4;finalStarted=false;ShowWorld();Place(hubSpawn.position,hubSpawn.rotation);
        ending=TienTrinhTroChoi.Instance.LayEnding(4);
        QuanLyTrangThaiGame.Instance.KichHoatEnding();Controls(false);Save();
    }
    public void OpenPhone()
    {
        if(dreaming||InMenu||Paused)return;
        PhoneOpen=true;idle=0;Controls(false);Sfx(phoneTone);
    }
    public void PhoneAction(int action)
    {
        if(!PhoneOpen||dreaming)return;
        int c=Mathf.Clamp(cycle,0,4);
        if(action==0&&!posted[c]){posted[c]=true;TienTrinhTroChoi.Instance.DangBaiTrend();TienTrinhTroChoi.Instance.GhiNhanTuongTacDoiThuc();Say("Nhật ký giấc mơ đã đăng.",4);}
        if(action==1&&!called[c]){called[c]=true;TienTrinhTroChoi.Instance.NgheDienThoai();TienTrinhTroChoi.Instance.GhiNhanTuongTacDoiThuc();Say("Mẹ: Dạo này con ngủ được không? Có chuyện gì thì cứ nói với bố mẹ nhé.\nMinh: Dạ… con nghe đây.",10);}
        if(action==2&&!ignored[c]){ignored[c]=true;TienTrinhTroChoi.Instance.BoQuaTinNhan();Say("Điện thoại im lặng.",3);}
        idle=0;PhoneOpen=false;Controls(true);Save();
    }
    public void Speak(string text){if(InMenu||Paused)return;Say(text,8);idle=0;}
    void Say(string text,float seconds){caption=text;captionUntil=Time.unscaledTime+seconds;}
    void Sfx(AudioClip clip){if(!clip)return;var source=!effectA.isPlaying?effectA:!effectB.isPlaying?effectB:null;if(source)source.PlayOneShot(clip);}
    void Place(Vector3 position,Quaternion rotation)
    {
        player.controller.enabled=false;player.transform.SetPositionAndRotation(position,rotation);
        player.ResetVerticalMotion();player.controller.enabled=true;Physics.SyncTransforms();
    }
    void Controls(bool enabled)
    {
        player.enabled=enabled;
        var look=player.GetComponent<DieuKhienGocNhin>();if(look)look.enabled=enabled;
        var interaction=player.GetComponent<TuongTacNguoiChoi>();if(interaction)interaction.enabled=enabled;
        Cursor.lockState=enabled?CursorLockMode.Locked:CursorLockMode.None;Cursor.visible=!enabled;
    }
    void ShowWorld()
    {
        hub.SetActive(!dreaming);
        for(int i=0;i<regions.Length;i++)regions[i].SetActive(dreaming&&cycle==i);
        if(sun){sun.color=dreaming?new Color(.6f,.68f,.8f):new Color(1,.83f,.61f);sun.intensity=dreaming?.65f:1;}
        if(drone){if(dreaming)drone.Play();else drone.Stop();}
        if(ambience&&regionAudio.Length>0){ambience.clip=regionAudio[Mathf.Clamp(dreaming?cycle+1:0,0,regionAudio.Length-1)];ambience.Play();}
        ApplyTraces();
    }
    void ApplyTraces()
    {
        for(int i=0;i<traceMoved.Length;i++)if(traceMoved[i])
            traceMoved[i].localRotation=Found[i]?Quaternion.Euler(0,35,8):Quaternion.identity;
    }
    public void TogglePause()
    {
        Paused=!Paused;Time.timeScale=Paused?0:1;AudioListener.pause=Paused;Controls(!Paused);
    }
    public void Save()
    {
        try
        {
            var data=new SaveData{cycle=cycle,dreaming=dreaming,found=Found,faceSeen=FaceSeen,posted=posted,called=called,ignored=ignored,
                ending=ending,vySeen=vySeen,finalStarted=finalStarted,lastMessageCycle=lastMessageCycle,position=player.transform.position,rotation=player.transform.rotation,progress=TienTrinhTroChoi.Instance.Capture()};
            string temp=SavePath+".tmp";File.WriteAllText(temp,JsonUtility.ToJson(data,true));
            if(File.Exists(SavePath))File.Replace(temp,SavePath,SavePath+".bak");else File.Move(temp,SavePath);
            notice="Đã lưu";
        }
        catch(Exception ex){notice="Không thể lưu: "+ex.Message;Debug.LogException(ex);}
    }
    static bool Valid(bool[] a,int n)=>a!=null&&a.Length==n;
    public bool Load()
    {
        try
        {
            var data=JsonUtility.FromJson<SaveData>(File.ReadAllText(SavePath));
            if(data==null||data.version!=1||data.cycle<0||data.cycle>4||data.progress==null||!Valid(data.found,4)||!Valid(data.faceSeen,4)||!Valid(data.posted,5)||!Valid(data.called,5)||!Valid(data.ignored,5)||float.IsNaN(data.position.x)||float.IsInfinity(data.position.y)||(data.dreaming&&data.cycle>=4))throw new InvalidDataException("Dữ liệu lưu không hợp lệ");
            cycle=data.cycle;dreaming=data.dreaming;Found=data.found;FaceSeen=data.faceSeen;posted=data.posted;called=data.called;ignored=data.ignored;
            ending=data.ending;vySeen=data.vySeen;lastMessageCycle=data.lastMessageCycle;finalStarted=data.finalStarted;finalTimer=0;
            TienTrinhTroChoi.Instance.Restore(data.progress);
            QuanLyTrangThaiGame.Instance.RestoreState(cycle,string.IsNullOrEmpty(ending)?dreaming?QuanLyTrangThaiGame.TrangThai.Mo:QuanLyTrangThaiGame.TrangThai.Thuc:QuanLyTrangThaiGame.TrangThai.KetThuc);
            InMenu=false;Paused=false;PhoneOpen=false;Time.timeScale=1;AudioListener.pause=false;ShowWorld();
            Place(data.position,data.rotation);Controls(string.IsNullOrEmpty(ending));return true;
        }
        catch(Exception ex){notice="Không thể tải: "+ex.Message;return false;}
    }
    void OnGUI()
    {
        GUI.skin.label.wordWrap=true;
        if(Time.unscaledTime<captionUntil&&!InMenu)GUI.Box(new Rect(Screen.width*.15f,Screen.height*.83f,Screen.width*.7f,80),caption);
        if(InMenu||Paused||PhoneOpen||!string.IsNullOrEmpty(ending))
        {
            float x=Screen.width/2-230,y=Screen.height/2-190;
            GUI.Box(new Rect(x,y,460,390),"CON RỐI");
            GUILayout.BeginArea(new Rect(x+25,y+35,410,340));
            if(!string.IsNullOrEmpty(ending))
            {
                string text=ending=="C"?"Minh: Bố mẹ ơi… con không muốn tiếp tục học ngành này chỉ vì sợ làm bố mẹ buồn. Con chưa biết phải làm gì, nhưng con muốn tự chịu trách nhiệm.\n\nMẹ: Mẹ đang nghe.":ending=="B"?"Điện thoại nằm im. Minh nhìn trang nhật ký viết dở. Cậu đã nhận ra những sợi dây, nhưng vẫn chưa gọi.":ending=="A2"?"Mẹ: Dạo này con thế nào?\nMinh: Con ổn mà.\nMẹ: Có chắc không?\nMinh: Con ổn mà.":"Minh mở điện thoại, đăng thêm một trang nhật ký giấc mơ. Ngón tay lướt qua màn hình như mọi sáng.";
                GUILayout.Label(text);if(GUILayout.Button("Về màn hình chính")){ending=null;InMenu=true;Controls(false);}
            }
            else if(PhoneOpen)
            {
                GUILayout.Label("Vy · Đằng Sau Giấc Ngủ\nCậu còn ghi nhật ký chứ?\n\nCuộc gọi nhỡ · Mẹ");
                if(GUILayout.Button("Đăng nhật ký"))PhoneAction(0);
                if(GUILayout.Button("Gọi về nhà"))PhoneAction(1);
                if(GUILayout.Button("Để tin nhắn lại"))PhoneAction(2);
                if(GUILayout.Button("Đóng")){PhoneOpen=false;Controls(true);}
            }
            else
            {
                if(Paused&&GUILayout.Button("Tiếp tục"))TogglePause();
                if(InMenu&&GUILayout.Button("Bắt đầu"))NewGame();
                if(File.Exists(SavePath)&&GUILayout.Button("Tải trò chơi"))Load();
                if(Paused&&GUILayout.Button("Lưu trò chơi"))Save();
                GUILayout.Label("Âm lượng");AudioListener.volume=GUILayout.HorizontalSlider(PlayerPrefs.GetFloat("volume",.7f),0,1);PlayerPrefs.SetFloat("volume",AudioListener.volume);
                var look=player.GetComponent<DieuKhienGocNhin>();GUILayout.Label("Độ nhạy chuột");look.mouseSensitivity=GUILayout.HorizontalSlider(PlayerPrefs.GetFloat("sensitivity",500),100,1000);PlayerPrefs.SetFloat("sensitivity",look.mouseSensitivity);
                GUILayout.Label("WASD: đi · Chuột: nhìn · E: tương tác · Esc: tạm dừng\nKhám phá theo nhịp của bạn. Có thể bỏ qua các dấu vết.");
                GUILayout.Label(notice);
                if(GUILayout.Button("Thoát")){PlayerPrefs.Save();Application.Quit();}
            }
            GUILayout.EndArea();
        }
    }
    void OnDestroy(){Time.timeScale=1;AudioListener.pause=false;}
}
