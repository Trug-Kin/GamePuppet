// Quản lý gói hàng, ô giao và phản hồi Hà; đọc khăn thật thay vì nhãn hoặc ID nút.
// Component trong scene r92: Lake_Market_Story_GDD21.
// Reference public để đối chiếu Inspector (có nhánh tùy chọn): story, root, delivery, slip, puppet, gesture, mimicPackage, mimicLid, parcels, placementPreview.
// Code tự tìm/tạo: ChuanBiGoiHangR88, Camera, TuongTacNguoiChoi, KhanTrongGoiHangR35, TraiNghiemHanhTrinhR23, MoDauBenHoR23, TuongTacHoCho, Canvas.
// Thứ tự chạy khai báo: 22500; giữ quan hệ khởi tạo/pose với các owner khác.
// Memory giữ tên field/schema; restore phối hợp owner State và bộ đếm SuccessfulLoads.
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

// Optional scene opt-in. Ending axes and original market discoveries remain owned by story.
[DefaultExecutionOrder(22500)] [UnityEngine.Scripting.APIUpdating.MovedFrom(true, null, "Assembly-CSharp", "MarketPackagesR35")]
public sealed class QuanLyGoiHangChoR35:MonoBehaviour {
 [Serializable] public sealed class Memory {
  public int version=1;
  // 0=home, 1=held, 2=delivery mat. Only authored safe sockets are persisted.
  public int[] places=new int[2];public bool[] opened=new bool[2];
  public int confirmed=-1,viewed;
  public bool invited,notChosen,abandoned;
 }
 public DieuPhoiHanhTrinh story;
 public Transform root,delivery,slip,puppet,gesture,mimicPackage,mimicLid;
 public GoiHangChoR35[] parcels;
 public GameObject placementPreview;
 public Memory memory=new Memory();
 public int Held=>Array.IndexOf(memory.places,1);
 public int Candidate=>Array.IndexOf(memory.places,2);
 public bool Available=>story.data.region==1&&!story.DreamHub;
 public bool Ready=>!story.UiInputBlocked&&Available&&!story.MenuOpen&&!story.Loading&&!story.Paused&&!story.PhoneOpen&&!story.SupplementInspection;
 public bool PreviewVisible=>placementPreview&&placementPreview.activeSelf;
 public bool PuppetVisible=>Visible();
 public float Clock{get;private set;}
 public float LastAction{get;private set;}=-10;
 public float LastMimic{get;private set;}=-10;
 public float SeenMotion{get;private set;}
 public string LastResponse{get;private set;}="";
 public string Notes=>"PHIẾU: "+((memory.viewed&1)!=0?"1 khăn XANH RÊU · gói mang nhãn KHĂN LEN":"chưa xem")+"\nGói cạnh phiếu: "+((memory.viewed&2)!=0?parcels[0].scarf.Description:"chưa mở")+"\nGói khoang bên: "+((memory.viewed&4)!=0?parcels[1].scarf.Description:"chưa mở");
 Vector3[] homes;Quaternion[] rotations;Camera eye;TuongTacNguoiChoi interaction;
 ChuanBiGoiHangR88 preparation;
 DieuPhoiHanhTrinh.State owner;int lastFrame=-1,region=-99,loads=-1;
 InputAction action,back;Text status;Quaternion gestureRest;float seen,animStart=-10;bool evidence;
 Vector3 playerAtAction;bool aimAtAction;int mimicKind;
 bool mimicOpen,mimicHeld;float mimicLidAngle,mimicHeight;float[] lidAngles=new float[2];
 struct Delayed {public float at;public int kind;public Vector3 player;public bool visible;}
 readonly Queue<Delayed> queue=new Queue<Delayed>();
 void Awake(){
  preparation=GetComponent<ChuanBiGoiHangR88>();
  eye=story.player.GetComponentInChildren<Camera>();interaction=story.player.GetComponent<TuongTacNguoiChoi>();
  homes=new Vector3[2];rotations=new Quaternion[2];for(int i=0;i<2;i++){homes[i]=parcels[i].transform.position;rotations[i]=parcels[i].transform.rotation;}
  if(gesture)gestureRest=gesture.localRotation;
  action=new InputAction("Parcel context",InputActionType.Button,"<Keyboard>/e");back=new InputAction("Return parcel",InputActionType.Button,"<Keyboard>/r");action.Enable();back.Enable();BuildUI();Restore();
 }
 void OnDestroy(){action?.Dispose();back?.Dispose();}
 public void Restore(){
  owner=story.data;memory=new Memory();
  try{if(!string.IsNullOrEmpty(owner.marketPackagesR35))memory=JsonUtility.FromJson<Memory>(owner.marketPackagesR35)??new Memory();}catch(ArgumentException){memory=new Memory();}
  if(memory.version!=1||memory.places?.Length!=2||memory.opened?.Length!=2)memory=new Memory();
  bool held=false,staged=false;for(int i=0;i<2;i++){
   int p=memory.places[i];if(p<0||p>2||(p==1&&held)||(p==2&&staged))p=0;
   if(p==1)held=true;if(p==2)staged=true;memory.places[i]=p;
  }
  if(memory.confirmed< -1||memory.confirmed>1||memory.confirmed!=Candidate)memory.confirmed=-1;
  queue.Clear();seen=0;animStart=-10;evidence=false;LastResponse="";lastFrame=Time.frameCount;
  for(int i=0;i<2;i++)lidAngles[i]=memory.opened[i]?108:0;
  mimicHeld=Held>=0;int showing=Held>=0?Held:Candidate;mimicOpen=showing>=0&&memory.opened[showing];mimicLidAngle=mimicOpen?108:0;mimicHeight=mimicHeld?.15f:0;Visuals();
 }
 public void Capture(){
  if(homes==null)return;if(owner!=story.data){Restore();return;}
  if(!Available){for(int i=0;i<2;i++)if(memory.places[i]==1)memory.places[i]=0;memory.abandoned=memory.confirmed<0&&!memory.notChosen&&memory.invited;Visuals();}
  owner.marketPackagesR35=JsonUtility.ToJson(memory);
 }
 void Changed(int kind){
  lastFrame=Time.frameCount;LastAction=Clock;
  queue.Enqueue(new Delayed{at=Clock+.5f,kind=kind,player=story.player.transform.position,visible=Visible()});
  Visuals();story.NotifyInteraction();story.Save();
 }
 public void Pick(int id){
  if(!Ready||id<0||id>=2)return;
  if(Candidate==id&&Held>=0){Stage();return;}
  int old=Held;if(old>=0)memory.places[old]=0;
  memory.places[id]=1;if(memory.confirmed==id)memory.confirmed=-1;memory.notChosen=false;Changed(0);
 }
 public void Toggle(){
  if(!Ready||Held<0)return;int i=Held;memory.opened[i]=!memory.opened[i];if(memory.opened[i])memory.viewed|=2<<i;Changed(memory.opened[i]?1:2);
 }
 public void Return(){
  if(!Ready)return;if(story.polish.IsReading){story.polish.DismissReading();return;}
  int i=Held;if(i<0)return;memory.places[i]=0;Changed(3);
 }
 public void Stage(){
  if(!Ready||Held<0)return;
  int i=Held,old=Candidate;if(old>=0)memory.places[old]=0;memory.places[i]=2;memory.confirmed=-1;memory.notChosen=false;Changed(3);
  story.Say("Gói đã đặt thử. Có thể lấy lại để xem, đổi gói, hoặc gặp Hà để xác nhận giao.");
 }
 public void ReadSlip(){
  if(!Ready)return;memory.viewed|=1;lastFrame=Time.frameCount;
  story.polish.Read("PHIẾU GHI HÀNG", "Một khăn len · XANH RÊU\nVỏ gói: KHĂN LEN\n\nHai gói cùng nhãn. Kiểm bên trong trước khi giao, hoặc để lại nếu chưa chọn.\nE / R · Cất phiếu");story.Save();
 }
 public string HaPrompt=>Candidate>=0?"E · Xác nhận giao gói đang trên ô với Hà":"E · Nói với Hà: tôi chưa chọn gói";
 public void Ha(){
  if(!Ready)return;
  // Keep the original finite dialogue/performance/face state machine.
  story.Talk(1);
  int i=Candidate;
  if(i<0){if(Held>=0)memory.places[Held]=0;memory.notChosen=true;LastResponse="Chị Hà: Em cứ để hai gói ở đó. Chị vẫn giữ phiếu này; lúc khác mình xem tiếp.";}
  else {
   memory.confirmed=i;memory.notChosen=false;
   var actual=parcels[i].GetComponentInChildren<KhanTrongGoiHangR35>(true);
   LastResponse=actual.colour==KhanTrongGoiHangR35.Colour.MossGreen?"Chị Hà: Xanh rêu, như trên phiếu. Hai vỏ gói giống nhau quá; chị để gói em mở ở đây.":"Chị Hà: Trong này là đỏ gạch à… Phiếu lại ghi xanh rêu. Chị đã xếp chúng cùng một nhãn. Chị để riêng gói này nhé.";
   memory.opened[i]=true;memory.viewed|=2<<i;
  }
  var journey=GetComponent<TraiNghiemHanhTrinhR23>();if(journey&&journey.journeyR44)LastResponse=journey.MarketReplyR44(i>=0&&parcels[i].GetComponentInChildren<KhanTrongGoiHangR35>(true).colour==KhanTrongGoiHangR35.Colour.MossGreen);
  lastFrame=Time.frameCount;story.Say(LastResponse);Visuals();story.Save();
 }
 bool Visible(){
  if(!puppet||!gesture)return false;
  var opening=story.GetComponent<MoDauBenHoR23>();
  return opening&&opening.InView(puppet,gesture.position,7,.1f)&&opening.InView(puppet,mimicPackage.position+Vector3.up*.1f,7,.1f);
 }
 void Update(){
  if(owner!=story.data||loads!=story.SuccessfulLoads){loads=story.SuccessfulLoads;Restore();}
  if(region!=story.data.region){if(region==1){Capture();queue.Clear();seen=0;}region=story.data.region;}
  root.gameObject.SetActive(Available);if(puppet)puppet.gameObject.SetActive(Available);
  if(!Available){placementPreview.SetActive(false);status.transform.parent.gameObject.SetActive(false);return;}
  if(!memory.invited&&Ready&&Vector3.Distance(story.player.transform.position,delivery.position)<4){memory.invited=true;story.Say("Chị Hà: Hai gói chị để cạnh phiếu và ở khoang bên. Em xem giúp chị bên trong nhé; cùng nhãn mà chị cứ thấy lẫn.");story.Save();}
  if(Ready){
   Clock+=Time.unscaledDeltaTime;
   if(Vector3.Distance(story.player.transform.position,delivery.position)>7&&Held>=0){memory.places[Held]=0;memory.abandoned=true;queue.Clear();Visuals();story.Save();}
   if(back.WasPressedThisFrame())Return();
   if(action.WasPressedThisFrame()&&Time.frameCount!=lastFrame&&Held>=0&&interaction.Target==null&&!story.polish.IsReading)Toggle();
   while(queue.Count>0&&queue.Peek().at<=Clock){var m=queue.Dequeue();LastMimic=Clock;animStart=Clock;mimicKind=m.kind;playerAtAction=m.player;aimAtAction=m.visible;seen=0;evidence=true;
    if(m.kind==0)mimicHeld=true;if(m.kind==1)mimicOpen=true;if(m.kind==2)mimicOpen=false;if(m.kind==3)mimicHeld=false;
   }
   float t=Clock-animStart;float wave=t>=0&&t<.65f?Mathf.Sin(t/.65f*Mathf.PI):0;
   if(gesture)gesture.localRotation=gestureRest*Quaternion.Euler((mimicHeld?-20:0)-18*wave,0,12*wave);
   mimicLidAngle=Mathf.MoveTowards(mimicLidAngle,mimicOpen?108:0,260*Time.unscaledDeltaTime);mimicHeight=Mathf.MoveTowards(mimicHeight,mimicHeld?.15f:0,.35f*Time.unscaledDeltaTime);
   if(mimicLid)mimicLid.localRotation=Quaternion.Euler(mimicLidAngle,0,0);
   if(mimicPackage)mimicPackage.localPosition=new Vector3(0,mimicHeight,0);
   bool still=Vector3.Distance(story.player.transform.position,playerAtAction)<.12f;
   if(evidence&&t>=0&&t<.65f&&aimAtAction&&still&&Visible())seen+=Time.unscaledDeltaTime;
   if(t>=.65f&&evidence){evidence=false;SeenMotion=seen;
    if(seen>=.30f&&!story.data.witnessed[1]){story.data.witnessed[1]=true;story.data.experienceR22Flags|=16;story.Save();}
   }
  }
  Visuals();var target=interaction.Target as DichTuongTacChoR35;
  placementPreview.SetActive(Ready&&Held>=0&&((target&&!target.slip)||(interaction.Target is GoiHangChoR35 occupied&&occupied.id==Candidate)));
  status.transform.parent.gameObject.SetActive(Ready&&!story.polish.IsReading&&Held>=0&&interaction.Target==null);
  string held=Held>=0?"ĐANG GIỮ · Gói "+(Held==0?"cạnh phiếu":"khoang bên")+"\n"+(memory.opened[Held]?"Đã mở · KHĂN "+parcels[Held].scarf.Description:"Đang đóng · nhãn KHĂN LEN")+"\n"+(interaction.Target==null?"E · "+(memory.opened[Held]?"Khép gói":"Mở gói")+"    ":"")+"R · Đặt về chỗ cũ":"";
  status.text=held+"\n"+(memory.confirmed>=0?"ĐÃ GIAO · Gói trên ô ở lại quầy.":Candidate>=0?"ĐẶT THỬ · Có thể lấy lại; gặp Hà để giao.":memory.notChosen?"CHƯA CHỌN · Có thể tiếp tục hoặc xem lại.":"Có thể kiểm hàng, hoặc tiếp tục tới lối TỈNH DẬY.");
 }
 void Visuals(){
  if(homes==null)return;
  for(int i=0;i<2;i++){
   var p=parcels[i];p.gameObject.SetActive(Available);int place=memory.places[i];
   if(place==1){
    p.transform.SetParent(eye.transform,false);p.transform.localRotation=Quaternion.Euler(-20,0,0);
    var offset=new Vector3(.20f,-.16f,.80f);var direction=eye.transform.TransformDirection(offset.normalized);float reach=offset.magnitude;
    foreach(var hit in Physics.SphereCastAll(eye.transform.position,.22f,direction,reach,~0,QueryTriggerInteraction.Ignore))
     if(hit.distance>.001f&&!hit.transform.IsChildOf(story.player.transform)&&!hit.transform.IsChildOf(p.transform)&&!hit.collider.GetComponentInParent<TuongTacHoCho>())reach=Mathf.Min(reach,Mathf.Max(.50f,hit.distance-.01f));
    p.transform.localPosition=offset.normalized*reach;
   }
   else {p.transform.SetParent(root,true);p.transform.position=place==2?delivery.position+Vector3.up*.006f:homes[i]+(i==0&&preparation?preparation.PackageOffset:Vector3.zero);p.transform.rotation=place==2?delivery.rotation:rotations[i];}
   foreach(var c in p.GetComponentsInChildren<Collider>(true))c.enabled=place!=1;
   if(Ready)lidAngles[i]=Mathf.MoveTowards(lidAngles[i],memory.opened[i]?108:0,300*Time.unscaledDeltaTime);
   p.lid.localRotation=Quaternion.Euler(lidAngles[i],0,0);
   p.inside.gameObject.SetActive(memory.opened[i]);p.label.gameObject.SetActive(!memory.opened[i]);
  }
 }
 void BuildUI(){
  var go=new GameObject("R35_ParcelContext",typeof(Canvas),typeof(CanvasScaler));go.transform.SetParent(transform,false);var cv=go.GetComponent<Canvas>();cv.renderMode=RenderMode.ScreenSpaceCamera;cv.worldCamera=eye;cv.planeDistance=.36f;cv.sortingOrder=165;
  DieuPhoiGiaoDienR42.ConfigureCanvas(story,cv);
  var sc=go.GetComponent<CanvasScaler>();sc.uiScaleMode=CanvasScaler.ScaleMode.ScaleWithScreenSize;sc.referenceResolution=new Vector2(1280,720);
  var box=new GameObject("Package_notes",typeof(RectTransform),typeof(Image));box.transform.SetParent(go.transform,false);var r=box.GetComponent<RectTransform>();r.anchorMin=r.anchorMax=r.pivot=new Vector2(1,1);r.anchoredPosition=new Vector2(-22,-35);r.sizeDelta=new Vector2(325,252);box.GetComponent<Image>().color=new Color(.025f,.035f,.03f,.92f);
  var txt=new GameObject("Text",typeof(RectTransform),typeof(Text));txt.transform.SetParent(box.transform,false);r=txt.GetComponent<RectTransform>();r.anchorMin=Vector2.zero;r.anchorMax=Vector2.one;r.offsetMin=new Vector2(12,8);r.offsetMax=new Vector2(-12,-8);status=txt.GetComponent<Text>();status.font=Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");status.fontSize=17;status.color=new Color(.94f,.90f,.78f);status.raycastTarget=false;
 }
}
