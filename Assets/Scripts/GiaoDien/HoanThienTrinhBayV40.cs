// Quản lý trình bày thoại/đọc và phản hồi trải nghiệm; dùng chung owner hành trình.
// Component trong scene r92: Lake_Market_Story_GDD21.
// Reference public để đối chiếu Inspector (có nhánh tùy chọn): story, eye, endingFocus, gardenFenceStages, pageTurn, receiver.
// Code tự tìm/tạo: Canvas, CanvasScaler, RectTransform, Image, Text, HuongDanHanhTrinhV48, TuongTacNguoiChoi, TuongTacGiayLop203R32.
// Thứ tự chạy khai báo: 21000; giữ quan hệ khởi tạo/pose với các owner khác.
using System;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

// Authored only into v40. The shipped v36/v39 scenes and builds remain checkpoints.
[DefaultExecutionOrder(21000)]
[UnityEngine.Scripting.APIUpdating.MovedFrom(true, null, "Assembly-CSharp", "JourneyPolishV40")]
public sealed class HoanThienTrinhBayV40 : MonoBehaviour
{
 public DieuPhoiHanhTrinh story;
 public Camera eye;
 public Transform[] endingFocus;
 public GameObject[] gardenFenceStages;
 public AudioClip pageTurn,receiver;
 public bool DialogueVisible=>!string.IsNullOrEmpty(story.Dialogue)&&Time.unscaledTime<dialogueUntil;
 public bool IsReading=>!string.IsNullOrEmpty(reading);
 public void DismissReading(){reading="";story.RefreshControls();}
 Canvas canvas;
 Text subtitle,help,phone,reader,title;
 Image fade;
 string reading="",lastDialogue="",lastEnding="";
 float regionAt,dialogueUntil,transition,readAt;
 int previousRegion=-99,previousBeat=-1;
 string output;
 StreamWriter timing,events;
 double entered,active,moving,readingSeconds,dialogueSeconds,distance;
 double previousTick;
 int previousNext;
 Vector3 previousPosition;
 Vector3 readOrigin;
 bool closed;
 public int MessageCount=>story.data.vyMessages;
 public string VyMessage=>string.IsNullOrEmpty(story.data.vyMessage)?"Cậu không cần nhớ hết. Có một đoạn nào cứ quay lại không?":story.data.vyMessage;
 public bool HasMessage=>story.data.vyMessageAvailable;
 public bool PrepareSleep()
 {
  var d=story.data;d.dreamCycles=Mathf.Max(d.dreamCycles,d.nextRegion)+1;
  int posts=d.posted.Count(x=>x),interval=posts>=2?1:posts>0?2+d.dreamCycles%2:3+d.dreamCycles%2;
  bool deliver=d.dreamCycles==1||d.dreamCycles-d.vyLastCycle>=interval;
  if(!deliver)return false;
  d.vyLastCycle=d.dreamCycles;d.vyMessages++;d.vyMessageAvailable=true;
  d.vyMessage=d.dreamCycles==1?"Tớ thử ghi lại một giấc mơ. Nếu cậu muốn thử, đừng cố điều khiển mọi thứ nhé.":posts>=2?"Tớ đọc bài mới rồi. Cậu có muốn kể tiếp không?":"Mấy hôm rồi không thấy cậu kể. Cậu vẫn ổn chứ? Không muốn đăng cũng được.";
  Event("Vy_message",d.vyMessage);return true;
 }
 void Awake()
 {
  var args=Environment.GetCommandLineArgs();int i=Array.IndexOf(args,"--verification-output");
  output=i>=0&&i+1<args.Length?args[i+1]:Path.Combine(Application.persistentDataPath,"v40-sessions",DateTime.UtcNow.ToString("yyyyMMdd_HHmmss"));
#if UNITY_EDITOR
  var recording=UnityEditor.SessionState.GetString("V40Record","");if(!string.IsNullOrEmpty(recording))output=recording;
#endif
  Directory.CreateDirectory(output);
  timing=new StreamWriter(Path.Combine(output,"area-timings.csv")){AutoFlush=true};
  timing.WriteLine("area,entry_realtime_s,exit_realtime_s,wall_seconds,active_seconds,moving_seconds,reading_seconds,dialogue_seconds,distance_m,reason");
  events=new StreamWriter(Path.Combine(output,"experience-events.jsonl")){AutoFlush=true};
  previousPosition=story.player.transform.position;BuildUI();
 }
 void BuildUI()
 {
  var root=new GameObject("V40_Presentation",typeof(RectTransform),typeof(Canvas),typeof(CanvasScaler));
  root.transform.SetParent(transform,false);canvas=root.GetComponent<Canvas>();canvas.renderMode=RenderMode.ScreenSpaceCamera;canvas.worldCamera=eye;canvas.planeDistance=.4f;canvas.sortingOrder=100;
  DieuPhoiGiaoDienR42.ConfigureCanvas(story,canvas);
  var scaler=root.GetComponent<CanvasScaler>();scaler.uiScaleMode=CanvasScaler.ScaleMode.ScaleWithScreenSize;scaler.referenceResolution=new Vector2(1280,720);
  help=Label("Controls",new Vector2(24,24),new Vector2(820,40),15,TextAnchor.UpperLeft);
  title=Label("Chapter",new Vector2(30,86),new Vector2(900,80),29,TextAnchor.UpperLeft);
  subtitle=Label("Dialogue",new Vector2(150,540),new Vector2(980,145),23,TextAnchor.MiddleCenter);
  phone=Label("Phone",new Vector2(760,130),new Vector2(440,450),23,TextAnchor.UpperLeft);
  reader=Label("Document",new Vector2(180,120),new Vector2(920,470),22,TextAnchor.UpperLeft);
  var black=new GameObject("Wake_Fade",typeof(RectTransform),typeof(Image));black.transform.SetParent(canvas.transform,false);
  var rect=black.GetComponent<RectTransform>();rect.anchorMin=Vector2.zero;rect.anchorMax=Vector2.one;rect.offsetMin=rect.offsetMax=Vector2.zero;
  fade=black.GetComponent<Image>();fade.color=Color.clear;fade.raycastTarget=false;
 }
 Text Label(string name,Vector2 position,Vector2 size,int fontSize,TextAnchor alignment)
 {
  var panel=new GameObject(name,typeof(RectTransform),typeof(Image));panel.transform.SetParent(canvas.transform,false);
  var rect=panel.GetComponent<RectTransform>();rect.anchorMin=rect.anchorMax=new Vector2(0,1);rect.pivot=new Vector2(0,1);rect.anchoredPosition=new Vector2(position.x,-position.y);rect.sizeDelta=size;
  var bg=panel.GetComponent<Image>();bg.color=new Color(.025f,.035f,.032f,.86f);bg.raycastTarget=false;
  var go=new GameObject("Text",typeof(RectTransform),typeof(Text),typeof(Shadow));go.transform.SetParent(panel.transform,false);
  var textRect=go.GetComponent<RectTransform>();textRect.anchorMin=Vector2.zero;textRect.anchorMax=Vector2.one;textRect.offsetMin=new Vector2(16,8);textRect.offsetMax=new Vector2(-16,-8);
  var text=go.GetComponent<Text>();text.font=Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");text.fontSize=fontSize;text.alignment=alignment;text.color=new Color(.92f,.89f,.79f);text.supportRichText=false;text.raycastTarget=false;
  return text;
 }
 static void Show(Text text,bool visible,string value=""){text.transform.parent.gameObject.SetActive(visible);if(visible)text.text=value;}
 public void Read(string heading,string text)
 {
  readAt=Time.unscaledTime;readOrigin=story.player.transform.position;reading=heading+"\n\n"+text;story.RefreshControls();story.PlayEffect(pageTurn);Event("read",heading);
 }
 public void Event(string kind,string detail)
 {
  if(events==null)return;
  events.WriteLine("{\"t\":"+Time.realtimeSinceStartup.ToString("F3",System.Globalization.CultureInfo.InvariantCulture)+",\"region\":"+story.data.region+",\"kind\":\""+kind+"\",\"detail\":\""+detail.Replace("\\","\\\\").Replace("\"","\\\"").Replace("\n","\\n").Replace("\r","")+"\"}");
 }
 string Area()=>!string.IsNullOrEmpty(lastEnding)?"Ending_"+lastEnding:previousRegion<0?"Hub_before_"+previousNext:new[]{"Lake","Market","School","Slope"}[Mathf.Clamp(previousRegion,0,3)];
 void FlushArea(string reason)
 {
  if(previousRegion==-99||timing==null)return;
  timing.WriteLine(string.Join(",",Area(),entered.ToString("F3",System.Globalization.CultureInfo.InvariantCulture),Time.realtimeSinceStartup.ToString("F3",System.Globalization.CultureInfo.InvariantCulture),(Time.realtimeSinceStartup-entered).ToString("F3",System.Globalization.CultureInfo.InvariantCulture),active.ToString("F3",System.Globalization.CultureInfo.InvariantCulture),moving.ToString("F3",System.Globalization.CultureInfo.InvariantCulture),readingSeconds.ToString("F3",System.Globalization.CultureInfo.InvariantCulture),dialogueSeconds.ToString("F3",System.Globalization.CultureInfo.InvariantCulture),distance.ToString("F3",System.Globalization.CultureInfo.InvariantCulture),reason));
 }
 void Update()
 {
  double now=Time.realtimeSinceStartupAsDouble,elapsed=previousTick>0?now-previousTick:0;previousTick=now;
  var d=story.data;var key=Keyboard.current;
  if(d.region!=previousRegion||d.ending!=lastEnding)
  {
   FlushArea("transition");entered=Time.realtimeSinceStartup;active=moving=readingSeconds=dialogueSeconds=distance=0;
   previousRegion=d.region;previousNext=d.nextRegion;lastEnding=d.ending;previousPosition=story.player.transform.position;regionAt=Time.unscaledTime;transition=.65f;reading="";elapsed=0;
   Event("area_enter",Area());
  }
  if(!story.Paused)
  {
   active+=elapsed;var delta=story.player.transform.position-previousPosition;
   if(delta.magnitude<2){distance+=delta.magnitude;if(delta.magnitude>.001f)moving+=elapsed;}
   if(IsReading)readingSeconds+=elapsed;
   if(Time.unscaledTime<dialogueUntil)dialogueSeconds+=elapsed;
  }
  previousPosition=story.player.transform.position;
  if(gardenFenceStages!=null)for(int stage=0;stage<gardenFenceStages.Length;stage++)if(gardenFenceStages[stage])gardenFenceStages[stage].SetActive(stage<=Mathf.Clamp(d.nextRegion,0,3));
  if(IsReading&&Vector3.Distance(readOrigin,previousPosition)>.75f)reading="";
  if(!story.Paused&&IsReading&&Time.unscaledTime-readAt>.15f&&key!=null&&(key.tabKey.wasPressedThisFrame||key.eKey.wasPressedThisFrame||key.rKey.wasPressedThisFrame))DismissReading();
  if(story.Dialogue==lastDialogue&&(story.Paused||story.PhoneOpen||story.SupplementInspection||IsReading))dialogueUntil+=Time.unscaledDeltaTime;
  if(story.Dialogue!=lastDialogue)
  {
   lastDialogue=story.Dialogue;dialogueUntil=string.IsNullOrEmpty(story.Dialogue)?0:Time.unscaledTime+Mathf.Max(5,story.Dialogue.Length/14f);
   if(!string.IsNullOrEmpty(lastDialogue))Event("dialogue",lastDialogue);
  }
  if(!story.Paused&&!story.MenuOpen)transition=Mathf.Max(0,transition-Time.unscaledDeltaTime);fade.color=new Color(.025f,.03f,.035f,transition/.65f);
  Show(help,true,story.Paused?"TẠM DỪNG · Esc tiếp tục · F5 lưu / F6 tải":"WASD · Đi bộ     Chuột · Nhìn     E · Tương tác     P · Điện thoại     Esc · Tạm dừng");
  Show(title,Time.unscaledTime-regionAt<4,!string.IsNullOrEmpty(d.ending)?"SÁNG HÔM SAU":d.region<0?"PHÒNG TRỌ · ĐÀ LẠT":new[]{"HỒ XUÂN HƯƠNG","CHỢ ĐÊM ĐÀ LẠT","GIẢNG ĐƯỜNG CŨ","CON DỐC QUEN"}[d.region]);
  var guide=story.GetComponent<HuongDanHanhTrinhV48>();
  var target=story.player.GetComponent<TuongTacNguoiChoi>().Target;
  var subtitleRect=(RectTransform)subtitle.transform.parent;bool ending=!string.IsNullOrEmpty(d.ending);subtitleRect.anchoredPosition=new Vector2(150,ending?-425:-540);subtitleRect.sizeDelta=new Vector2(980,ending?270:145);subtitle.fontSize=ending?21:23;
  Show(subtitle,!IsReading&&(Time.unscaledTime<dialogueUntil||ending||target!=null),ending?EndingText(d.ending,d.endingBeat)+"\n\n"+(d.endingBeat<3?"E · Tiếp tục khi bạn sẵn sàng":"CON RỐI · Hết     R · Chơi lại"):Time.unscaledTime<dialogueUntil?story.Dialogue:guide?HuongDanHanhTrinhV48.Prompt(target):"E · Quan sát / trò chuyện");
  Show(reader,IsReading,reading+"\n\nE / Tab · Cất trang giấy");
  if(guide){var pr=(RectTransform)phone.transform.parent;pr.anchoredPosition=new Vector2(630,-90);pr.sizeDelta=new Vector2(610,580);phone.fontSize=20;}
  Show(phone,story.PhoneOpen&&!(GetComponent<TuongTacGiayLop203R32>()&&GetComponent<TuongTacGiayLop203R32>().Comparing),guide?guide.PhoneText:"ĐẰNG SAU GIẤC NGỦ\n\n"+(HasMessage?"Vy: "+VyMessage:"Chưa có tin nhắn mới.")+"\n\n1 · Ghi và đăng giấc mơ\n2 · Gọi về nhà\n3 · Bỏ qua tin nhắn\n\nP · Cất điện thoại");
  if(!string.IsNullOrEmpty(d.ending))
  {
   if(d.endingBeat!=previousBeat){previousBeat=d.endingBeat;story.PlayEffect(d.endingBeat==0?receiver:pageTurn);Event("ending_beat",d.ending+":"+d.endingBeat);}
   int focus=d.ending=="B"?Mathf.Min(d.endingBeat+1,3):d.endingBeat;
   if(endingFocus!=null&&endingFocus.Length>focus&&endingFocus[focus])eye.transform.rotation=Quaternion.Slerp(eye.transform.rotation,Quaternion.LookRotation(endingFocus[focus].position-eye.transform.position),1-Mathf.Exp(-Time.unscaledDeltaTime*1.5f));
   if(!story.Paused&&!(GetComponent<ChuanBiHanhLyR41>()?.MemoryOpen??false)&&d.endingBeat==3&&key!=null&&key.rKey.wasPressedThisFrame)story.Restart();
  }
 }
 void OnApplicationQuit(){Close();}
 public static string EndingText(string ending,int beat)
 {
  string[] lines=ending=="C"?new[]{
   "Điện thoại nằm cạnh trang nhật ký. Minh đã viết được một câu rồi gạch đi: ‘Con vẫn ổn.’ Ngoài cửa sổ, tiếng xe đầu ngày đi qua con dốc. Cậu mở số điện thoại của mẹ, nhìn một lúc, rồi bấm gọi. Không có dòng hướng dẫn nào cho câu đầu tiên. Minh đặt bàn tay còn lại lên tờ giấy mẹ để trong túi áo. Lần này, cậu không tìm một giấc mơ để kể thay cho chuyện của mình.",
   "Mẹ: Con dậy sớm thế?\nMinh: Con chưa ngủ được. Con vẫn bảo là con ổn, nhưng thật ra con đang sợ. Con sợ tiền trọ, sợ học không xong, sợ bố mẹ nghĩ con không cố gắng.\nĐầu dây bên kia im một lúc. Minh định cười, định đổi sang chuyện thời tiết. Cậu nhìn câu vừa gạch và để khoảng im lặng ở nguyên đó.\nMẹ: Mẹ đang nghe. Con nói tiếp đi.",
   "Minh: Con không muốn học tiếp chỉ vì sợ làm bố mẹ buồn. Con chưa biết mình sẽ làm gì. Con cần tìm hiểu việc chuyển ngành, tiền học và công việc làm thêm. Con không muốn bố mẹ quyết định hộ. Nhưng con muốn nói thật với bố mẹ, kể cả khi mọi người chưa đồng ý.\nMẹ không trả lời ngay. Ngoài cửa, một người gọi nhau xuống chợ. Cuộc sống vẫn tiếp tục trong khi câu hỏi của Minh còn chưa có đáp án.",
   "Mẹ: Mẹ chưa hiểu hết. Tối nay bố về, cả nhà mình nói chuyện nhé.\nMinh: Vâng. Tối nay con sẽ gọi.\nCậu ghi giờ gọi vào trang giấy, cạnh những khoản tiền vẫn chưa trả xong. Cuộc gọi không làm chúng biến mất. Dây con rối nằm đứt trên bàn từ lâu; buổi sáng nay cũng không có ai đến tháo thêm một sợi nào. Minh để trang nhật ký mở, đứng dậy và nhìn ra cửa sổ. Hôm nay, cậu còn một việc phải tự làm."
  }:ending=="B"?new[]{
   "Minh thức dậy. Cậu nhớ chiếc ghế hướng ra cửa sổ, những cuốn sách chưa mở, chiếc vali chờ một buổi sáng khác. Cậu kéo trang nhật ký lại và viết: ‘Không ai giữ tay mình.’ Ngòi bút dừng ở cuối câu. Ngoài cửa sổ, tiếng người đi làm đã bắt đầu. Điện thoại ở ngay trong tầm tay, không có một chướng ngại nào giữa cậu và màn hình tối.",
   "Cậu viết tên những người đã bảo mình nên chọn gì. Dưới mỗi cái tên, Minh nhớ ra một lần chính mình đã lặp lại lời khuyên đó với người khác. Cậu không gạch tên nào. Ở cuối trang, có chỗ để viết việc đầu tiên mình sẽ làm. Cậu xoay cây bút giữa hai ngón tay. Đồng hồ vẫn chạy. Căn phòng không đòi cậu phải quyết định ngay, và cũng không quyết định thay cậu.",
   "Điện thoại sáng lên rồi tối đi. Minh nhớ số của mẹ mà không cần mở danh bạ. Cậu thử nghĩ câu đầu tiên: ‘Con có chuyện muốn nói.’ Chỉ nghĩ thôi đã đủ để nghe thấy những câu hỏi tiếp theo. Minh đặt bút nằm ngang trang giấy. Cậu biết mình đang tránh điều gì. Sự nhận biết ấy không tự nhấc điện thoại lên, cũng không biến thành một cuộc gọi.",
   "Trang nhật ký vẫn thiếu dòng tiếp theo. Minh kéo rèm ra một chút. Thành phố ở bên ngoài cửa sổ đã thức, bình thường như mọi ngày. Cậu nhìn bàn tay mình rồi nhìn con rối. Những sợi dây trên bàn đã đứt từ lâu. Không có tiếng bước chân đến ép cậu ngồi xuống. Minh vẫn ngồi. Điện thoại vẫn nằm im. Buổi sáng tiếp tục mà không chờ cậu viết xong."
  }:ending=="A2"?new[]{
   "Minh mở danh bạ và gọi về nhà. Cậu đã ghi lại giấc mơ, đã trả lời tin nhắn, đã làm những việc mà mình nghĩ một người chủ động nên làm. Màn hình hiện số quen thuộc. Cậu chỉnh lại cổ áo dù đầu dây bên kia không nhìn thấy. Khi tiếng chuông dừng, câu trả lời đầu tiên đã sẵn ở trên môi, trước cả khi mẹ hỏi.",
   "Mẹ: Dạo này con thế nào?\nMinh: Con ổn mà.\nMẹ hỏi chuyện ăn uống, chuyện tiền trọ, chuyện học. Minh trả lời từng câu, ngắn và tròn trịa như những ô đã được kẻ sẵn. Cậu nhìn tờ giấy trên bàn, nơi mình đã định ghi một điều khó nói. Bàn tay giữ điện thoại không buông ra. Cuộc trò chuyện vẫn đang diễn ra, nhưng câu cần nói vẫn nằm ngoài nó.",
   "Mẹ: Con có chuyện gì muốn nói không?\nMinh: Không ạ. Mọi thứ vẫn đúng kế hoạch.\nCậu nghe giọng mình rõ ràng đến mức thấy yên tâm. Ở đầu dây bên kia, mẹ nói tối nhớ ăn. Minh hứa. Cậu còn định nói thêm một câu, nhưng lại hỏi nhà có mưa không. Cuộc gọi tìm được một lối rẽ quen thuộc. Không ai tranh cãi, không ai thất vọng, không ai nghe thấy điều Minh chưa nói.",
   "Cuộc gọi kết thúc. Màn hình tối lại và phản chiếu một phần khuôn mặt Minh. Cậu đặt điện thoại đúng chỗ cũ, cạnh trang nhật ký. Cậu đã gọi về; trong danh sách hôm nay, có một việc được gạch đi. Câu trên tờ giấy vẫn chưa viết xong. Ngoài cửa sổ, tiếng xe khuất dần xuống dốc. Minh nhìn đồng hồ, rồi chuẩn bị trả lời tin nhắn tiếp theo."
  }:new[]{
   "Minh thức dậy và mở điện thoại như mọi sáng. Cậu nhớ được vài hình ảnh rời rạc: mặt nước, một quầy sách, tiếng đồng hồ. Ngón tay mở trang nhật ký trước khi cậu nghĩ xem mình có muốn viết không. Ngoài cửa, một chiếc xe dừng rồi chạy tiếp. Màn hình sáng đủ để căn phòng có vẻ đã bắt đầu một ngày mới. Minh không nhìn con rối trên bàn.",
   "Cậu ghép các hình ảnh thành một bài đăng. Những câu quen thuộc tự tìm được thứ tự của chúng. Cậu chọn một tiêu đề, thêm một dòng hỏi người khác có từng mơ giống mình không. Bài viết được đăng lên. Minh không đọc lại đoạn cuối. Cậu kéo màn hình xuống chờ một thông báo mới, như thể giấc mơ chỉ thật sự kết thúc khi có ai đó xác nhận đã nhìn thấy nó.",
   "Một thông báo hiện lên. Minh lướt qua, mở khung trả lời rồi đóng lại. Cậu chỉnh giờ báo thức cho ngày mai. Tờ giấy cạnh điện thoại nhắc một cuộc gọi về nhà; cậu đẩy nó sang bên để có chỗ đặt cốc. Không có tiếng dây kéo trong căn phòng. Chỉ có tiếng ngón tay chạm màn hình và đồng hồ đếm một phút nữa trôi qua.",
   "Con rối nằm trên bàn. Dây đã đứt từ lâu, đầu dây cuộn lại trong bụi. Minh khoác áo và kiểm tra điện thoại lần cuối trước khi ra khỏi phòng. Cậu không thấy có điều gì khác thường. Ngày mai, cậu vẫn đặt báo thức cùng một giờ. Màn hình tắt đi, để lại trang nhật ký đã đăng và một căn phòng yên lặng chờ đến lần thức dậy tiếp theo."
  };
  return lines[Mathf.Clamp(beat,0,3)];
 }
 void OnDestroy(){Close();}
 void Close(){if(closed)return;closed=true;FlushArea("session_end");timing?.Dispose();events?.Dispose();timing=null;events=null;}
}
