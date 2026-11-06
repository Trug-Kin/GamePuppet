// Quản lý cầm/đặt/mở khép cùng cặp trang sổ; lưu hinge dở, không dùng page giả thay geometry.
// Component trong scene r92: Lake_Market_Story_GDD21.
// Reference public để đối chiếu Inspector (có nhánh tùy chọn): story, root, book, leaf, leftPage, socket, gesture, planText, blankText, coverText, preview, paperSound, closeSound, placeSound.
// Code tự tìm/tạo: Camera, TuongTacNguoiChoi, MoDauBenHoR23, TuTheBaoBenCauV48, TraiNghiemHanhTrinhR23, TuongTacHoCho, Canvas, CanvasScaler.
// Thứ tự chạy khai báo: 22600; giữ quan hệ khởi tạo/pose với các owner khác.
// Memory giữ tên field/schema; restore phối hợp owner State và bộ đếm SuccessfulLoads.
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

// Optional authored lake activity. Existing trace/ending systems remain authoritative.
[DefaultExecutionOrder(22600)] [UnityEngine.Scripting.APIUpdating.MovedFrom(true, null, "Assembly-CSharp", "LakeNotebookR37")]
public sealed class TuongTacSoTayHoR37:MonoBehaviour {
 [Serializable] public sealed class Memory {public int version=1,place,page,viewed;public bool open,touched,invited,abandoned,handObserved;public int response=-1;public bool hasHingeR38;public float hingeR38;}
 public DieuPhoiHanhTrinh story;
 public Transform root,book,leaf,leftPage,socket,gesture;
 public TextMesh planText,blankText,coverText;
 public GameObject preview;
 public BoxCollider bookCollider;
 public AudioClip paperSound,closeSound,placeSound;
 public Memory memory=new Memory();
 // r38 presents the actual two-page spread. Earlier checkpoint scenes retain their original input sequence.
 public bool pairedPagesR38;
 public TiepCanCoChuDichR89 preparationR89;
 public float gestureLiftDegrees=32; // r39 uses an explicitly authored larger real arm lift over the rail.
 public int VisualRevision{get;private set;}
 public float RestoredHinge{get;private set;}
 public bool HandMotion=>Ready&&Clock>=gestureAt&&Clock-gestureAt<.55f;
 public bool Active=>story.data.region==0&&!story.DreamHub;
 public bool Ready=>!story.UiInputBlocked&&Active&&!story.Loading&&!story.MenuOpen&&!story.Paused&&!story.PhoneOpen&&!story.SupplementInspection&&!story.polish.IsReading;
 public bool Held=>memory.place==1;
 public string Prompt=>Held?"E · Đặt sổ lên mặt kê":"E · Cầm sổ của Minh";
 public string HeldPrompt=>!memory.open?"E · Mở sổ":pairedPagesR38?"E · Khép sổ":memory.page==0?"E · Xem trang trống":"E · Khép sổ";
 public string Notes=>memory.viewed==0?"":((memory.viewed&1)!=0?"Sổ của mình: dòng ‘Tối nay viết một trang’ đã bị gạch.":"")+((memory.viewed&2)!=0?"\nTrang bên kia còn trống.":"");
 public float Clock{get;private set;}
 public float LastInput{get;private set;}=-10;
 public float LastMimic{get;private set;}=-10;
 public float SeenGesture{get;private set;}
 public int SoundRequests{get;private set;}
 public string LastResponse{get;private set;}="";
 Camera eye;TuongTacNguoiChoi interaction;InputAction use,back;Text hint;
 DieuPhoiHanhTrinh.State owner;int loads=-1,lastFrame=-1,region=-99;float angle,gestureAt=-10,seen;bool evidence;Quaternion armRest;
 void Awake(){
  eye=story.player.GetComponentInChildren<Camera>();interaction=story.player.GetComponent<TuongTacNguoiChoi>();armRest=gesture.localRotation;
  use=new InputAction("Notebook context",InputActionType.Button,"<Keyboard>/e");back=new InputAction("Return notebook",InputActionType.Button,"<Keyboard>/r");use.Enable();back.Enable();
  BuildUI();Restore();
 }
 void OnDestroy(){use?.Dispose();back?.Dispose();}
 public void Restore(){
  owner=story.data;memory=new Memory();try{if(!string.IsNullOrEmpty(owner.lakeNotebookR37))memory=JsonUtility.FromJson<Memory>(owner.lakeNotebookR37)??new Memory();}catch(ArgumentException){memory=new Memory();}
  if(memory.version!=1||memory.place<0||memory.place>1)memory=new Memory();memory.page=Mathf.Clamp(memory.page,0,1);memory.viewed&=3;
  if(pairedPagesR38)memory.page=0; // Legacy r37 page=1 was the same open spread, not a different leaf.
  VisualRevision++;
  angle=pairedPagesR38&&memory.hasHingeR38?Mathf.Clamp(memory.hingeR38,-180,0):memory.open?-180:0;RestoredHinge=angle;gestureAt=-10;evidence=false;seen=0;gesture.localRotation=armRest;LastResponse="";lastFrame=Time.frameCount;Visuals();
 }
 public void Capture(){
  if(!eye)return;if(owner!=story.data){Restore();return;}
  if(!Active&&Held){memory.place=0;memory.abandoned=true;Visuals();}
  if(pairedPagesR38){memory.hasHingeR38=true;memory.hingeR38=angle;}
  owner.lakeNotebookR37=JsonUtility.ToJson(memory);
 }
 bool Visible(){return story.GetComponent<MoDauBenHoR23>().InView(story.puppets[0].transform,gesture.position,8,.08f)&&story.GetComponent<MoDauBenHoR23>().InView(book,book.position,3,.03f);}
 void Changed(int kind,AudioClip clip){
  VisualRevision++;
  lastFrame=Time.frameCount;LastInput=Clock;LastMimic=-10;gestureAt=Clock;seen=0;evidence=(kind==1||kind==2||kind==3)&&Visible();
  if(clip){SoundRequests++;story.PlayEffectAt(clip,book.position);}Visuals();story.NotifyInteraction();story.Save();
 }
 public void PickOrPlace(){
  if(!Ready)return;if(Held){Place(true);return;}memory.place=1;memory.touched=true;Changed(0,paperSound);
 }
 public void Cycle(){
  if(!Ready||!Held)return;
  if(pairedPagesR38){memory.open=!memory.open;memory.page=0;if(memory.open)memory.viewed|=3;Changed(memory.open?1:3,memory.open?paperSound:closeSound);return;}
  if(!memory.open){memory.open=true;memory.page=0;memory.viewed|=1;Changed(1,paperSound);}
  else if(memory.page==0){memory.page=1;memory.viewed|=2;Changed(2,paperSound);}
  else {memory.open=false;memory.page=0;Changed(3,closeSound);}
 }
 public void Place(bool feedback){
  if(!Ready||!Held)return;memory.place=0;Changed(4,placeSound);
  if(feedback&&Vector3.Distance(story.player.transform.position,story.GetComponent<TuTheBaoBenCauV48>().actorPosition)<5){memory.response=memory.open?1:0;LastResponse=memory.open?"Ông Bảo: Cháu để mở à. Trang kia vẫn còn chỗ để viết.":"Ông Bảo: Khép lại cũng được. Lúc nào quay lại, cuốn sổ vẫn ở đây.";var journey=GetComponent<TraiNghiemHanhTrinhR23>();if(journey&&journey.journeyR44)LastResponse=journey.LakeReplyR44;story.Say(LastResponse);story.Save();}
 }
 void Update(){
  if(owner!=story.data||loads!=story.SuccessfulLoads){loads=story.SuccessfulLoads;Restore();}
  if(region!=story.data.region){if(region==0){Capture();evidence=false;gestureAt=-10;}region=story.data.region;}
  root.gameObject.SetActive(Active);if(!Active){preview.SetActive(false);hint.transform.parent.gameObject.SetActive(false);return;}
  if(Ready){
   Clock+=Time.unscaledDeltaTime;
   if(!memory.invited&&Vector3.Distance(story.player.transform.position,socket.position)<3.5f&&!story.polish.DialogueVisible){memory.invited=true;story.Say("Ông Bảo: Sổ cháu để trên mặt kê đấy. Cứ xem lại, đường trên cầu vẫn còn rộng.");story.Save();}
   if(Held&&Vector3.Distance(story.player.transform.position,socket.position)>6){memory.place=0;memory.abandoned=true;evidence=false;gestureAt=-10;Visuals();story.Save();}
   if(back.WasPressedThisFrame())Place(true);
   if(use.WasPressedThisFrame()&&lastFrame!=Time.frameCount&&Held&&interaction.Target==null)Cycle();
  }
  Visuals();preview.SetActive(Ready&&Held&&interaction.Target is DichTuongTacSoTayR37 target&&target.activity==this);
  bool show=Ready&&Held&&interaction.Target==null;hint.transform.parent.gameObject.SetActive(show);hint.text=HeldPrompt+"   ·   R · Đặt lại";
 }
 void LateUpdate(){
  if(!Ready)return;float age=Clock-gestureAt;
  if(age>=0&&age<.55f){float wave=Mathf.Sin(age/.55f*Mathf.PI);gesture.localRotation=armRest*Quaternion.Euler(-gestureLiftDegrees*wave,0,-14*wave);
   if(wave>.02f&&LastMimic<LastInput)LastMimic=Clock;
   if(evidence&&wave>.02f&&Visible())seen+=Time.unscaledDeltaTime;
  }else if(evidence){evidence=false;SeenGesture=seen;if(seen>=.25f){bool fresh=!memory.handObserved;memory.handObserved=true;if(!story.data.witnessed[0]){story.data.witnessed[0]=true;story.data.experienceR22Flags|=8;fresh=true;}if(fresh)story.Save();}}
 }
 void Visuals(){
  if(!eye)return;book.gameObject.SetActive(Active);
  if(Held){book.SetParent(eye.transform,false);book.localRotation=Quaternion.Euler(-48,memory.open&&!pairedPagesR38?(memory.page==0?-8:8):0,0);var offset=new Vector3(.30f,-.17f,.72f);float reach=offset.magnitude;
   foreach(var h in Physics.SphereCastAll(eye.transform.position,.19f,eye.transform.TransformDirection(offset.normalized),reach,~0,QueryTriggerInteraction.Ignore))if(h.distance>.001f&&!h.transform.IsChildOf(story.player.transform)&&!h.transform.IsChildOf(book)&&!h.collider.GetComponentInParent<TuongTacHoCho>())reach=Mathf.Min(reach,Mathf.Max(.48f,h.distance-.02f));book.localPosition=offset.normalized*reach;
  }else{book.SetParent(root,true);book.SetPositionAndRotation(socket.position+(preparationR89?preparationR89.NotebookOffset:Vector3.zero),socket.rotation*(preparationR89?preparationR89.NotebookRotation:Quaternion.identity));}
  bookCollider.enabled=!Held;
  bookCollider.center=new Vector3(memory.open?0:.078f,.008f,0);bookCollider.size=new Vector3(memory.open?.332f:.164f,.02f,.224f);
  if(Ready)angle=Mathf.MoveTowards(angle,memory.open?-180:0,480*Time.unscaledDeltaTime);
  leaf.localRotation=Quaternion.Euler(0,0,angle);leftPage.localRotation=leaf.localRotation;
  planText.gameObject.SetActive(angle< -100);blankText.gameObject.SetActive(angle< -100);coverText.gameObject.SetActive(angle> -85);
 }
 void BuildUI(){
  var g=new GameObject("R37_NotebookContext",typeof(Canvas),typeof(CanvasScaler));g.transform.SetParent(transform,false);var c=g.GetComponent<Canvas>();c.renderMode=RenderMode.ScreenSpaceCamera;c.worldCamera=eye;c.planeDistance=.36f;c.sortingOrder=170;
  DieuPhoiGiaoDienR42.ConfigureCanvas(story,c);
  var scaler=g.GetComponent<CanvasScaler>();scaler.uiScaleMode=CanvasScaler.ScaleMode.ScaleWithScreenSize;scaler.referenceResolution=new Vector2(1280,720);
  var box=new GameObject("Notebook_action",typeof(RectTransform),typeof(Image));box.transform.SetParent(g.transform,false);var r=box.GetComponent<RectTransform>();r.anchorMin=r.anchorMax=r.pivot=new Vector2(.5f,0);r.anchoredPosition=new Vector2(0,78);r.sizeDelta=new Vector2(440,34);box.GetComponent<Image>().color=new Color(.025f,.035f,.03f,.9f);
  var t=new GameObject("Text",typeof(RectTransform),typeof(Text));t.transform.SetParent(box.transform,false);r=t.GetComponent<RectTransform>();r.anchorMin=Vector2.zero;r.anchorMax=Vector2.one;r.offsetMin=new Vector2(8,3);r.offsetMax=new Vector2(-8,-3);hint=t.GetComponent<Text>();hint.font=Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");hint.fontSize=18;hint.alignment=TextAnchor.MiddleCenter;hint.color=new Color(.92f,.9f,.8f);hint.raycastTarget=false;
 }
}
