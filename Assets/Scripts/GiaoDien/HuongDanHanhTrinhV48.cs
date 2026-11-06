// Hiện gợi ý hành trình theo trạng thái và vùng hiện tại.
// Component trong scene r92: Lake_Market_Story_GDD21.
// Reference public để đối chiếu Inspector (có nhánh tùy chọn): story.
// Code tự tìm/tạo: TraiNghiemHanhTrinhR23, TraiNghiemHanhTrinhR22, QuanLyGoiHangChoR35, HuongDanHanhTrinhV48, Canvas, Camera, CanvasScaler, RectTransform.
// Thứ tự chạy khai báo: 21500; giữ quan hệ khởi tạo/pose với các owner khác.
using UnityEngine;
using UnityEngine.UI;

// Guidance is derived from the loaded chapter, never an extra progression gate.
[DefaultExecutionOrder(21500)]
[UnityEngine.Scripting.APIUpdating.MovedFrom(true, null, "Assembly-CSharp", "JourneyGuideV48")]
public sealed class HuongDanHanhTrinhV48:MonoBehaviour
{
 public DieuPhoiHanhTrinh story;
 public bool bridgeMarketLayout;
 public bool regionalBoundaryHints;
 public string BoundaryHint=>!regionalBoundaryHints?"":story.data.region==2?"Sân và phòng tư liệu bên trái có thể khám phá. Vườn ngoài hàng rào đang đóng.":story.data.region==3?"Giấy tờ và vali ở hai bên dốc có thể xem. Các lối nhà dân đóng cửa.":"";
 Text objective;string previous="";float changed;bool previousMenu;int previousLoads=-1;
 public bool ReminderVisible=>objective&&objective.transform.parent.gameObject.activeSelf;
 public string ObservationHint=>story.data.region==0?"Có thể xem sổ ở mặt kê cạnh lan can.":story.data.region==1?"Có thể xem phiếu và mở gói tại quầy Hà.":story.data.region==2?"Có thể xem bài trên bàn trong lớp 203.":story.data.region==3&&!story.data.finalStarted?"Giấy tờ và vali có thể xem bên đường tới nhà Tùng.":"";
 public string Objective
 {
  get
  {
   var r23=GetComponent<TraiNghiemHanhTrinhR23>();if(r23)return r23.Objective;
   var experience=GetComponent<TraiNghiemHanhTrinhR22>();if(experience)return experience.Objective;
   var d=story.data;
   if(!string.IsNullOrEmpty(d.ending))return "E · Đọc tiếp câu chuyện. Khi trang cuối khép lại, R để chơi lại.";
   if(story.DreamHub)return "Căn phòng đã khác. Giấc mơ đang tiếp diễn…";
   if(d.region<0)return (d.nextRegion>0?"Đã trở về phòng. ":"")+"Nhìn giường cạnh bàn học, nhấn E để ngủ"+(d.nextRegion==0?" và đến hồ Xuân Hương.":d.nextRegion==1?" và đến chợ đêm.":d.nextRegion==2?" và đến trường cũ.":" và đến con dốc.")+"\nĐiện thoại và giấy tờ là lựa chọn; không cần dùng để đi tiếp.";
   if(d.region==0)return bridgeMarketLayout?"Theo biển CẦU ÔNG ĐẠO dọc hồ, qua vỉa cầu nơi ông Bảo đứng bên lan can. Sau cầu, theo biển VÒNG XOAY / CHỢ.\nLối TỈNH DẬY bên đường sau cầu: nhìn gần và nhấn E để về phòng. Có thể đi tiếp xem chợ; trò chuyện và lịch là tùy chọn.":"Đi theo lối lát ven hồ đến biển TỈNH DẬY ở cuối lan can; đến gần và nhấn E để về phòng.\nÔng Bảo ở chỗ câu cá. Có thể ghé trò chuyện; lịch và dấu vết không bắt buộc.";
   if(d.region==1)return bridgeMarketLayout?"Qua cổng CHỢ ĐÊM, theo phố tới quầy chị Hà và biển MẶT CHỢ phía trước. Có thể xem chợ rồi quay lại.\nLối TỈNH DẬY ở cuối đoạn quầy Hà: nhấn E để về phòng. Trò chuyện và chồng sách là tùy chọn.":"Theo biển CHỢ ĐÊM vào phố chợ, tìm quầy chị Hà. Cuối phố có lối TỈNH DẬY: nhấn E để về phòng.\nTrò chuyện và xem chồng sách là tùy chọn; không cần thu thập đủ.";
   if(d.region==2)return "Theo biển GIẢNG ĐƯỜNG vào lớp cũ, nơi cô Mai đang đứng. Khi muốn về phòng, quay lại lối vào có biển TỈNH DẬY và nhấn E.\nGhế, sách và giấy tờ là tùy chọn. Bạn có thể dừng lại bất cứ lúc nào.";
   return d.finalStarted?"Có ai đó đang nhìn. Bạn có thể quan sát, đứng yên hoặc nhìn quanh; không cần nhấn phím để qua chặng này.":"Đi dọc con dốc qua chỗ anh Tùng và chiếc vali đến cửa cuối đường. Nhìn cửa, nhấn E để tiếp tục.\nTrò chuyện và xem giấy tờ là tùy chọn.";
  }
 }
 public string PhoneText=>GetComponent<TraiNghiemHanhTrinhR23>()?GetComponent<TraiNghiemHanhTrinhR23>().PhoneText:LegacyPhoneText;
 string LegacyPhoneText=>"VIỆC TIẾP THEO\n"+Objective+"\n\n"+ObservationHint+(string.IsNullOrEmpty(BoundaryHint)?"":"\n"+BoundaryHint)+"\n\n"+(story.data.region<0&&!story.DreamHub?"ĐẰNG SAU GIẤC NGỦ\n"+(story.polish.HasMessage?"Vy: "+story.polish.VyMessage:"Chưa có tin nhắn mới.")+"\n\n1 · Ghi và đăng giấc mơ\n2 · Gọi về nhà\n3 · Để tin nhắn lại\nCác lựa chọn này không khóa khu tiếp theo.":"Ghi chú đang có sẵn trong giấc mơ. Về phòng trọ để nhắn tin hoặc gọi điện.")+"\n\nP · Cất điện thoại";
 public static string Prompt(IPlayerInteractable target)
 {
  if(target is DichTuongTacHanhLyR41 preparation)return preparation.Prompt;
  if(target is DichTuongTacSoTayR37 notebook)return notebook.Prompt;
  if(target is GoiHangChoR35 parcel)return parcel.Prompt;
  if(target is DichTuongTacChoR35 marketTarget)return marketTarget.Prompt;
  if(target is TuongTacHoCho ha&&ha.kind==TuongTacHoCho.Kind.Ha&&ha.story.TryGetComponent<QuanLyGoiHangChoR35>(out var packages))return packages.HaPrompt;
  if(target is DichTuongTacGiayR32 paper)return paper.Prompt;
  if(target is TuongTacTruongR28 r28)return r28.Prompt;
  if(target is TuongTacHoCho i)switch(i.kind)
  {
   case TuongTacHoCho.Kind.Bed:return "E · Ngủ — tiếp tục hành trình";
   case TuongTacHoCho.Kind.Phone:return "E · Điện thoại — mục tiêu / tin nhắn";
   case TuongTacHoCho.Kind.Wake:return "E · Tỉnh dậy — trở về phòng trọ";
   case TuongTacHoCho.Kind.Bao:return "E · Trò chuyện với ông Bảo (tùy chọn)";
   case TuongTacHoCho.Kind.Ha:return "E · Trò chuyện với chị Hà (tùy chọn)";
   case TuongTacHoCho.Kind.Puppet:var guide=i.story.GetComponent<HuongDanHanhTrinhV48>();return guide?guide.ObservationHint:"Con rối trong giấc mơ · Quan sát khi bạn đi và dừng";
  }
  if(target is TuongTacTruongV37 school)return school.exit?"E · Tỉnh dậy — trở về phòng trọ":school.memoryIndex>=0?"E · Đọc tư liệu (tùy chọn)":"E · Trò chuyện với cô Mai (tùy chọn)";
  if(target is TuongTacDocV39 slope)return slope.finalDoor?"E · Mở cửa — tiếp tục hành trình":slope.memoryIndex>=0?"E · Đọc giấy tờ (tùy chọn)":"E · Trò chuyện với anh Tùng (tùy chọn)";
  if(target is DaoCuKeChuyenV40 prop)return "E · Đọc "+prop.heading+" (tùy chọn)";
  return "E · Quan sát (tùy chọn)";
 }
 void Start()
 {
  var g=new GameObject("V48_Objective",typeof(Canvas),typeof(CanvasScaler));g.transform.SetParent(transform,false);
  var canvas=g.GetComponent<Canvas>();canvas.renderMode=RenderMode.ScreenSpaceCamera;canvas.worldCamera=story.player.GetComponentInChildren<Camera>();canvas.planeDistance=.35f;canvas.sortingOrder=160;DieuPhoiGiaoDienR42.ConfigureCanvas(story,canvas);
  var scaler=g.GetComponent<CanvasScaler>();scaler.uiScaleMode=CanvasScaler.ScaleMode.ScaleWithScreenSize;scaler.referenceResolution=new Vector2(1280,720);
  var panel=new GameObject("Objective",typeof(RectTransform),typeof(Image));panel.transform.SetParent(g.transform,false);
  var r=panel.GetComponent<RectTransform>();r.anchorMin=r.anchorMax=r.pivot=new Vector2(0,1);r.anchoredPosition=new Vector2(24,-142);r.sizeDelta=new Vector2(740,205);panel.GetComponent<Image>().color=new Color(.02f,.035f,.04f,.92f);
  var label=new GameObject("Text",typeof(RectTransform),typeof(Text));label.transform.SetParent(panel.transform,false);r=label.GetComponent<RectTransform>();r.anchorMin=Vector2.zero;r.anchorMax=Vector2.one;r.offsetMin=new Vector2(15,8);r.offsetMax=new Vector2(-15,-8);
  objective=label.GetComponent<Text>();objective.font=Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");objective.fontSize=20;objective.color=new Color(.96f,.94f,.84f);objective.raycastTarget=false;
 }
 void Update()
 {
  if(previous!=Objective||(previousMenu&&!story.MenuOpen)||previousLoads!=story.SuccessfulLoads){previous=Objective;changed=Time.unscaledTime;story.polish.Event("objective",previous);}
  previousMenu=story.MenuOpen;previousLoads=story.SuccessfulLoads;
  bool opening=story.data.region==0&&!story.data.witnessed[0];
  objective.transform.parent.gameObject.SetActive(!story.MenuOpen&&!story.Loading&&!story.PhoneOpen&&!story.Paused&&!story.polish.IsReading&&(Time.unscaledTime-changed<12||opening));
  objective.text=opening?"Trước mặt có một con rối bằng gỗ. Thử đi ngang vài bước rồi dừng, nếu bạn muốn quan sát nó.\nP · Xem việc tiếp theo. Không cần làm theo con rối để tiếp tục."+"\n"+Objective:"VIỆC TIẾP THEO · P để xem lại\n"+Objective+"\n"+ObservationHint;
 }
}
