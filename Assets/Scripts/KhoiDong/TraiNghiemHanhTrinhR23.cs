// Bổ sung trải nghiệm R23; các phần partial dùng chung một component/owner.
// Component trong scene r92: Lake_Market_Story_GDD21.
// Reference public để đối chiếu Inspector (có nhánh tùy chọn): story, readableUi.
// Code tự tìm/tạo: TuongTacSoTayHoR37, MoDauBenHoR23, HuongDanHanhTrinhV48, Camera, TuongTacNguoiChoi, Canvas, CanvasScaler, RectTransform.
// Thứ tự chạy khai báo: 21600; giữ quan hệ khởi tạo/pose với các owner khác.
using UnityEngine;
using UnityEngine.UI;

// Situated, optional learning. Never moves the camera/player, advances chapters
// or awards hidden points. Ordinary story saves retain the presentation memory.
[DefaultExecutionOrder(21600)]
[UnityEngine.Scripting.APIUpdating.MovedFrom(true, null, "Assembly-CSharp", "JourneyExperienceR23")]
public sealed partial class TraiNghiemHanhTrinhR23 : MonoBehaviour
{
    public DieuPhoiHanhTrinh story;
    public Material readableUi;
    Text hint;
    Vector3 openingPosition;
    Quaternion openingLook;
    int previousInteractions,previousRegion=-99,previousLoads=-1;
    int previousNext,previousAwareness,previousChoice;
    float entered,changed,observedUntil;
    string lastHint="",feedback="";
    Camera eye;
    TuongTacNguoiChoi interaction;
    float noticedAt=-1,lastWood=-30;
    MoDauBenHoR23 encounter;
    int observedRegion=-99;
    string observation="";
    public string Observation {
        get {
            var notebook=GetComponent<TuongTacSoTayHoR37>();if(story&&story.data.region==0&&notebook&&notebook.memory.handObserved)return "Mình mở, khép sổ; tay hình người gỗ cũng cử động ngay cùng nhịp.";
            if(!string.IsNullOrEmpty(observation)||!story||story.data==null)return observation;
            int region=story.data.region;
            return region==0&&Flag(8)?"Khi mình bước, con rối gỗ cũng dịch chuyển.":region==1&&Flag(16)?"Mình đã dừng, nhưng nó còn chuyển động thêm một nhịp.":region==2&&Flag(32)?"Mình đang dừng. Tay nó vừa giơ lên trước khi mình làm gì.":"";
        }
        private set {observation=value;}
    }
    public bool HintVisible=>hint&&hint.transform.parent.gameObject.activeSelf;
    bool Flag(int bit)=>(story.data.experienceR22Flags&bit)!=0;
    void Remember(int bit){story.data.experienceR22Flags|=bit;}
    void Awake()
    {
        encounter=GetComponent<MoDauBenHoR23>();
        var old=GetComponent<HuongDanHanhTrinhV48>();if(old)old.enabled=false;
        eye=story.player.GetComponentInChildren<Camera>();interaction=story.player.GetComponent<TuongTacNguoiChoi>();
    }
    void Start()
    {
        var canvasObject=new GameObject("R23_ContextHint",typeof(Canvas),typeof(CanvasScaler));canvasObject.transform.SetParent(transform,false);
        var canvas=canvasObject.GetComponent<Canvas>();canvas.renderMode=RenderMode.ScreenSpaceCamera;canvas.worldCamera=eye;canvas.planeDistance=.4f;canvas.sortingOrder=155;
        DieuPhoiGiaoDienR42.ConfigureCanvas(story,canvas);
        var scaler=canvasObject.GetComponent<CanvasScaler>();scaler.uiScaleMode=CanvasScaler.ScaleMode.ScaleWithScreenSize;scaler.referenceResolution=new Vector2(1280,720);
        var panel=new GameObject("Hint",typeof(RectTransform),typeof(Image));panel.transform.SetParent(canvasObject.transform,false);
        var rect=panel.GetComponent<RectTransform>();rect.anchorMin=rect.anchorMax=rect.pivot=Vector2.zero;rect.anchoredPosition=new Vector2(24,90);rect.sizeDelta=new Vector2(760,100);panel.GetComponent<Image>().color=new Color(.025f,.04f,.04f,.82f);
        var textObject=new GameObject("Text",typeof(RectTransform),typeof(Text));textObject.transform.SetParent(panel.transform,false);
        rect=textObject.GetComponent<RectTransform>();rect.anchorMin=Vector2.zero;rect.anchorMax=Vector2.one;rect.offsetMin=new Vector2(14,8);rect.offsetMax=new Vector2(-14,-8);
        hint=textObject.GetComponent<Text>();hint.font=Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");hint.fontSize=18;hint.color=new Color(.94f,.94f,.87f);hint.raycastTarget=false;
        // The world fog uses forward depth. UI writes its own near depth so
        // water/sky behind the phone cannot fog away its letters or panels.
        if(readableUi&&!DieuPhoiGiaoDienR42.OverlayFor(story))foreach(var graphic in GetComponentsInChildren<Graphic>(true)){
            graphic.material=readableUi;
            if(graphic is Image&&graphic.color.a>.1f){var c=graphic.color;c.a=1;graphic.color=c;}
        }
    }
    string RegionGoal
    {
        get
        {
            var d=story.data;
            if(journeyR44)return JourneyGoalR44;
            if(d.region==0)return !Flag(8)?"Ông Bảo ở lan can phía trước. Có thể ghé trò chuyện rồi nhìn hình người gỗ bên mặt nước. Lối TỈNH DẬY ở đầu cầu phía vòng xoay dẫn về phòng.":"Theo vỉa cầu về phía vòng xoay tới biển TỈNH DẬY: nhìn gần và E để về phòng. Giấc ngủ tiếp theo dẫn tới chợ. Có thể nhìn lại hình người gỗ nếu muốn.";
            if(d.region==1)return GetComponent<QuanLyGoiHangChoR35>()?"Tới quầy Hà: có thể xem phiếu và kiểm hai gói trên bàn. Chồng sách vẫn ở sau quầy. Lối TỈNH DẬY dẫn về; không cần chọn gói để tiếp tục.":"Theo cổng CHỢ ĐÊM vào phố vắng. Quầy chị Hà và chồng sách ở bên lối đi; biển TỈNH DẬY chỉ đường về.";
            if(d.region==2){var school=GetComponent<TraiNghiemTruongR28>();return school?school.Objective:"Theo biển GIẢNG ĐƯỜNG vào lớp cũ. Thử nhìn quanh ghế giáo viên và cửa sổ; lối về vẫn ở cửa vào.";}
            return d.finalStarted?"":"Theo con dốc tới cánh cửa cuối đường. Vali và giấy tờ bên đường có thể xem nếu muốn.";
        }
    }
    public string Objective
    {
        get
        {
            var d=story.data;
            if(!string.IsNullOrEmpty(d.ending))return "E · Đọc tiếp khi bạn sẵn sàng.";
            if(story.DreamHub)return "Những sợi dây đã trở lại. Căn phòng đang chìm vào giấc mơ…";
            if(d.region>=0)return RegionGoal;
            if(DieuPhoiGiaoDienR42.OverlayFor(story)&&d.nextRegion==0)return "Trên bàn có điện thoại và một con rối nhỏ bằng gỗ. Có thể lại gần nhìn; khi sẵn sàng, nhìn giường và E để ngủ.";
            if(d.nextRegion==0&&!Flag(4)&&!story.PhoneOpen)return !Flag(1)?"Chuột · Nhìn quanh căn phòng của Minh.":!Flag(2)?"WASD · Thử bước tới bàn học.":"Nhìn điện thoại hoặc giấy trên bàn. Khi tên vật hiện ra, E để tương tác.";
            return d.nextRegion==0?"Vy đã nhắc chuyện ghi lại giấc mơ. Khi sẵn sàng, nhìn chiếc giường và E để ngủ.":"Đã về phòng. Có thể ghi lại giấc mơ, gọi về nhà hoặc để tin nhắn lại. Giường dẫn sang "+new[]{"hồ","chợ đêm","trường cũ","con dốc","buổi sáng"}[Mathf.Clamp(d.nextRegion,0,4)]+".";
        }
    }
    public string PhoneText
    {
        get
        {
            var d=story.data;
            string context=d.region<0?"Minh đang thử ghi nhận những giấc mơ theo lời Vy. Không cần đăng bài hay gọi điện để được ngủ.":d.region==0?"Hồ và cây cầu quen thuộc. Có thể quan sát mặt nước và con rối gỗ khi bạn đi rồi dừng.":d.region==1?"Phố chợ quen nhưng vắng tiếng người. Có thể dừng lại và nhìn xem chuyển động quanh mình kết thúc lúc nào.":d.region==2?"Lớp học cũ. Để ý cử chỉ và hướng nhìn trong lúc bạn tự chọn đi hoặc dừng.":"Vali và giấy tờ nằm bên đường tới nhà Tùng.";
            if(journeyR44)context=d.region<0?ReturnNoteR44:"Trang Ghi nhớ giữ những nội dung đã xem. Vật cầm tạm ở lại khu khi tỉnh.";
            string choices=d.region<0&&!story.DreamHub?"\n\nTIN NHẮN · Vy\n"+(story.polish.HasMessage?story.polish.VyMessage:"Chưa có tin nhắn mới.")+"\n\n1 · Ghi và đăng giấc mơ"+(d.posted[Mathf.Clamp(d.nextRegion,0,d.posted.Length-1)]?" · đã ghi":"")+"\n2 · Gọi về nhà"+(d.called[Mathf.Clamp(d.nextRegion,0,d.called.Length-1)]?" · đã gọi":"")+"\n3 · Để tin nhắn lại":"";
            return (journeyR44?"GIẤC MƠ / PHÒNG TRỌ":"VIỆC TRƯỚC MẮT\n"+Objective)+"\n\n"+context+(string.IsNullOrEmpty(Observation)?"":"\n\nĐIỀU VỪA NHẬN THẤY\n"+Observation)+choices+(journeyR44?"":"\n\nTrò chuyện, dấu vết và giấy tờ là tùy chọn. Không cần xem hết để đi tiếp.")+"\nP · Cất điện thoại";
        }
    }
    bool Visible(Transform target,float far,float height=.8f)
    {
        var p=eye.WorldToViewportPoint(target.position+Vector3.up*height);
        if(p.z<0||p.z>far||p.x<.12f||p.x>.88f||p.y<.08f||p.y>.92f)return false;
        var delta=target.position+Vector3.up*height-eye.transform.position;
        foreach(var hit in Physics.RaycastAll(eye.transform.position,delta.normalized,delta.magnitude,~0,QueryTriggerInteraction.Ignore))
            if(!hit.transform.IsChildOf(story.player.transform)&&!hit.transform.IsChildOf(target))return false;
        return true;
    }
    void Update()
    {
        if(!hint||!story||story.data==null)return;
        var d=story.data;
        if(previousRegion!=d.region||previousLoads!=story.SuccessfulLoads)
        {
            previousRegion=d.region;previousLoads=story.SuccessfulLoads;entered=Time.unscaledTime;
            openingPosition=story.player.transform.position;openingLook=eye.transform.rotation;
            previousInteractions=interaction.CompletedInteractions;previousNext=d.nextRegion;previousAwareness=d.awareness;previousChoice=d.interactions+d.ignored;
            Observation="";feedback="";observedUntil=0;noticedAt=-1;observedRegion=-99;
            if(d.nextRegion>0||d.region>=0)Remember(7);
        }
        if(story.MenuOpen||story.Loading||story.Paused){hint.transform.parent.gameObject.SetActive(false);return;}
        if(d.region<0&&d.nextRegion==0&&!story.DreamHub)
        {
            if(Quaternion.Angle(openingLook,eye.transform.rotation)>12)Remember(1);
            var delta=story.player.transform.position-openingPosition;delta.y=0;if(delta.magnitude>1)Remember(2);
            if(!Flag(4)&&(interaction.CompletedInteractions>previousInteractions||story.PhoneOpen)){Remember(4);feedback="Bạn vừa tương tác với căn phòng. Khi muốn ngủ, chiếc giường ở cạnh bàn học.";observedUntil=Time.unscaledTime+5;}
        }
        if(d.nextRegion!=previousNext){feedback="Đã trở về phòng. Điều bạn vừa thấy có thể ghi lại hoặc kể với người nhà.";observedUntil=Time.unscaledTime+6;previousNext=d.nextRegion;}
        if(d.awareness>previousAwareness){feedback=journeyR44?"Chi tiết vừa xem đã có trong Ghi nhớ.":"Một chi tiết quen thuộc đã thay đổi. Bạn có thể nhìn lại; không cần tìm đủ mọi dấu vết.";observedUntil=Time.unscaledTime+5;previousAwareness=d.awareness;}
        if(d.interactions+d.ignored!=previousChoice){feedback="Lựa chọn đã được giữ lại. Bạn vẫn có thể tiếp tục ngủ khi sẵn sàng.";observedUntil=Time.unscaledTime+4;previousChoice=d.interactions+d.ignored;}
        if(d.region>=0&&d.region<2&&encounter.Opportunity){
            if(noticedAt<0)noticedAt=Time.unscaledTime;
            if(encounter.Observed)Observation=d.region==0?"Mình vừa bước rồi dừng. Hình người gỗ cũng bước và dừng cùng mình.":"Có nhiều hình người gỗ. Mình đã dừng, chân chúng vẫn bước nốt một nhịp.";
        }
        else if(d.region>=0&&d.region<2&&!encounter.Observed)noticedAt=-1;
        if(d.region==2&&!GetComponent<TraiNghiemTruongR28>()&&story.schoolChapter.Leading&&story.schoolChapter.LeadSeconds>.35f&&Visible(story.schoolChapter.puppet.transform,18)){
            Observation="Mình đang dừng. Tay nó vừa giơ lên trước khi mình làm gì.";Remember(32);
        }
        
        if(!journeyR44&&d.region>=0&&d.region<2&&encounter.Opportunity&&noticedAt>=0&&Time.unscaledTime-noticedAt>1&&Time.unscaledTime-noticedAt<5&&string.IsNullOrEmpty(Observation)){
            feedback=d.region==0?"Hình người gỗ đứng bên mặt nước. Thử nhìn chân nó khi bạn bước ngang rồi dừng. Chỉ cần quan sát; không cần nhấn E.":"Ở chợ có nhiều hình người gỗ hơn. Thử dừng lại, nhìn chân chúng thêm một nhịp. Không cần nhấn E.";observedUntil=Time.unscaledTime+.1f;
        }
        if(!string.IsNullOrEmpty(Observation)&&observedRegion!=d.region){
            observedRegion=d.region;
            if(!encounter.Observed&&Time.unscaledTime-lastWood>10){story.PlayEffect(story.wood);lastWood=Time.unscaledTime;}
        }
        if(!journeyR44&&d.region>=0&&d.region<2&&!d.traces[d.region]&&!Flag(d.region==0?128:256)&&Time.unscaledTime-entered>6&&Visible(story.traces[d.region],7,0)){
            Remember(d.region==0?128:256);feedback=d.region==0?"Tờ lịch ở lan can có một chi tiết không khớp. Có thể bước lại gần để nhìn kỹ.":"Có một chồng sách chưa mở cạnh quầy. Có thể ghé lại quan sát.";observedUntil=Time.unscaledTime+5;
        }
        string line=Time.unscaledTime<observedUntil?feedback:Objective;
        if(line!=lastHint){lastHint=line;changed=Time.unscaledTime;}
        bool early=d.region<0&&d.nextRegion==0&&!Flag(4);
        bool show=string.IsNullOrEmpty(d.ending)&&!d.finalStarted&&!story.SupplementInspection&&!story.PhoneOpen&&!story.polish.IsReading&&!story.polish.DialogueVisible&&(early||Time.unscaledTime-entered<6||Time.unscaledTime<observedUntil||Time.unscaledTime-changed<4);
        hint.transform.parent.gameObject.SetActive(show);hint.text=line+"\nP · Xem lại việc trước mắt";
    }
}
