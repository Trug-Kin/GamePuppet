// Cầm/đặt/đọc hành lý opt-in; memory không ghi điểm, witness hoặc ending.
// Component trong scene r92: Lake_Market_Story_GDD21.
// Reference public để đối chiếu Inspector (có nhánh tùy chọn): story, root, tag, paper, flap, tagRest, tagArranged, paperRest, tagFront, tagBack, paperInside, paperCover, preview, paperSound.
// Code tự tìm/tạo: Camera, TuongTacNguoiChoi, TraiNghiemHanhTrinhR23, TuongTacSoTayHoR37, QuanLyGoiHangChoR35, TuongTacGiayLop203R32, RectTransform, Image.
// Thứ tự chạy khai báo: 22700; giữ quan hệ khởi tạo/pose với các owner khác.
// Memory giữ tên field/schema; restore phối hợp owner State và bộ đếm SuccessfulLoads.
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

// Optional scene-authored props. No trace, witness, progression or ending writes.
[DefaultExecutionOrder(22700)] [UnityEngine.Scripting.APIUpdating.MovedFrom(true, null, "Assembly-CSharp", "SlopePreparationsR41")]
public sealed class ChuanBiHanhLyR41:MonoBehaviour {
 [Serializable] public sealed class Memory {
  public int version=1,held=-1,tagSlot,viewed,response=-1;
  public bool paperOpen,tagBack,invited,abandoned;
  public float tagTurn,paperTurn;
 }
 public const string TagId="slope.tag.r41",PaperId="slope.preparations.r41";
 public DieuPhoiHanhTrinh story;
 public Transform root,tag,paper,flap,tagRest,tagArranged,paperRest;
 public BoxCollider tagCollider,paperCollider;
 public TextMesh tagFront,tagBack,paperInside,paperCover;
 public GameObject preview;
 public Memory memory=new Memory();
 public AudioClip paperSound,placeSound;
 public string LastResponse{get;private set;}="";
 public bool Active=>story.data.region==3&&!story.DreamHub;
 public bool Ready=>!story.UiInputBlocked&&Active&&!story.data.finalStarted&&!story.Loading&&!story.MenuOpen&&!story.Paused&&!story.PhoneOpen&&!story.SupplementInspection&&!story.polish.IsReading&&string.IsNullOrEmpty(story.data.ending);
 public bool Held=>memory.held>=0;
 public string Notes=>((memory.viewed&1)!=0?"Thẻ cũ: ‘TÙNG · ĐI CHUYẾN SỚM’. Ngày đi được gạch rồi ghi lại.\n":"")+((memory.viewed&2)!=0?"Tờ chuẩn bị: đã xếp áo, giữ địa chỉ; dòng ‘mua vé’ dời từ thứ hai sang thứ sáu, rồi ‘để sáng mai’.":"");
 DieuPhoiHanhTrinh.State owner;
 Camera eye;TuongTacNguoiChoi interaction;
 InputAction use,back,turn;Text hint,notes,endingMessage;
 GameObject hubPanel,endingPanel,hubBackdrop;
 int loads=-1,lastFrame=-1;bool enabledActions,endingComparison;
 public bool MemoryOpen=>endingComparison;
 public string RememberedNotes=>AllNotes();
 float hinge;
 public float Hinge=>hinge;
 void Awake(){eye=story.player.GetComponentInChildren<Camera>();interaction=story.player.GetComponent<TuongTacNguoiChoi>();use=new InputAction("Preparation context",InputActionType.Button,"<Keyboard>/e");back=new InputAction("Put preparation back",InputActionType.Button,"<Keyboard>/r");turn=new InputAction("Rotate preparation",InputActionType.Value,"<Mouse>/scroll/y");BuildUI();Restore();}
 void OnDestroy(){use?.Dispose();back?.Dispose();turn?.Dispose();}
 public void Restore(){owner=story.data;memory=new Memory();try{if(!string.IsNullOrEmpty(owner.slopePreparationsR41))memory=JsonUtility.FromJson<Memory>(owner.slopePreparationsR41)??new Memory();}catch(ArgumentException){memory=new Memory();}
  if(memory.version!=1)memory=new Memory();memory.held=Mathf.Clamp(memory.held,-1,1);memory.tagSlot=Mathf.Clamp(memory.tagSlot,0,1);memory.viewed&=3;
  if(float.IsNaN(memory.tagTurn)||float.IsInfinity(memory.tagTurn))memory.tagTurn=0;if(float.IsNaN(memory.paperTurn)||float.IsInfinity(memory.paperTurn))memory.paperTurn=0;
  if(!Active||story.data.finalStarted)memory.held=-1;hinge=memory.paperOpen?-180:0;lastFrame=Time.frameCount;endingComparison=false;LastResponse="";Visuals();
 }
 public void Capture(){if(!eye)return;if(owner!=story.data){Restore();return;}if((!Active||story.data.finalStarted)&&Held){memory.held=-1;memory.abandoned=true;}owner.slopePreparationsR41=JsonUtility.ToJson(memory);}
 void Changed(AudioClip clip){lastFrame=Time.frameCount;Visuals();if(clip)story.PlayEffectAt(clip,(memory.held==0?tag:paper).position);story.NotifyInteraction();story.Save();}
 public string Prompt(int item,bool arrange){
  if(!Ready)return "";
  if(arrange)return memory.held==0?"E · Xếp thẻ cạnh quai vali":memory.tagSlot==1?"E · Cầm thẻ đã xếp":"Chỗ xếp thẻ cạnh quai vali";
  if(memory.held==item)return "E · Đặt lại "+(item==0?"thẻ hành lý":"tờ chuẩn bị");
  return "E · Cầm và xem "+(item==0?"thẻ hành lý cũ":"tờ chuẩn bị");
 }
 public void Interact(int item,bool arrange){if(!Ready)return;
  if(arrange){if(memory.held==0){memory.tagSlot=1;memory.held=-1;Changed(placeSound);}else if(memory.tagSlot==1)Pick(0);return;}
  if(memory.held==item){PutBack();return;}Pick(item);
 }
 void Pick(int item){memory.held=item;if(item==0){memory.tagTurn=0;memory.tagBack=false;}else memory.paperTurn=0;Changed(paperSound);}
 public void PutBack(){if(!Ready||!Held)return;memory.held=-1;Changed(placeSound);}
 public void Cycle(){if(!Ready||!Held)return;
  if(memory.held==0)memory.tagBack=!memory.tagBack;else memory.paperOpen=!memory.paperOpen;Changed(paperSound);
 }
 public bool TryRespond(){if(!Active||story.data.finalStarted||memory.viewed==0)return false;
  var journey=GetComponent<TraiNghiemHanhTrinhR23>();
  int signature=memory.viewed*4+memory.tagSlot+(memory.paperOpen?2:0);
  // r44 dialogue distinguishes held props from the same arrangement at rest.
  if(journey&&journey.journeyR44)signature+=(memory.held+1)*16;
  if(memory.response==signature)return false;
  memory.response=signature;
  LastResponse=memory.paperOpen?(memory.tagSlot==1?"Anh Tùng: Em xếp thẻ cạnh quai rồi à. Còn tờ kia… anh sửa ngày đi mấy lần, vẫn để mở thế.":"Anh Tùng: Tờ ấy vẫn còn mở. Áo thì xếp rồi, dòng mua vé anh cứ để sang hôm sau."):
   memory.tagSlot==1?"Anh Tùng: Thẻ đã nằm ngay cạnh quai. Lần trước anh cũng xếp gọn thế, rồi lại để đấy.":
   (memory.viewed&1)!=0?"Anh Tùng: Chữ trên thẻ phai rồi. Anh vẫn chưa ghi được một ngày khác vào đó.":"Anh Tùng: Anh gấp tờ chuẩn bị lại để khỏi quên. Việc đã làm thì ít khi phải sửa ngày.";
  if(journey&&journey.journeyR44)LastResponse=journey.SlopeReplyR44;
  story.Say(LastResponse);story.Save();return true;
 }
 void Update(){
  if(owner!=story.data||loads!=story.SuccessfulLoads){loads=story.SuccessfulLoads;Restore();}
  bool ready=Ready;if(ready!=enabledActions){enabledActions=ready;if(ready){use.Enable();back.Enable();turn.Enable();}else{use.Disable();back.Disable();turn.Disable();}}
  root.gameObject.SetActive(Active);
  if(Held&&(!ready&&(story.data.finalStarted||!Active)||Active&&Vector3.Distance(story.player.transform.position,tagRest.position)>6)){memory.held=-1;memory.abandoned=true;Capture();story.Save();}
  if(ready){
   if(!memory.invited&&Vector3.Distance(story.player.transform.position,tagRest.position)<3.2f&&!story.polish.DialogueVisible){memory.invited=true;story.Say("Trên lớp bụi còn một thẻ cũ. Tờ giấy bên cạnh đã gấp đi gấp lại.");story.Save();}
   if(back.WasPressedThisFrame())PutBack();
   if(use.WasPressedThisFrame()&&lastFrame!=Time.frameCount&&Held&&interaction.Target==null)Cycle();
   if(Held&&Mathf.Abs(turn.ReadValue<float>())>.01f){if(memory.held==0)memory.tagTurn=Mathf.Repeat(memory.tagTurn+Mathf.Sign(turn.ReadValue<float>())*30,360);else memory.paperTurn=Mathf.Repeat(memory.paperTurn+Mathf.Sign(turn.ReadValue<float>())*30,360);Changed(null);}
  }
  Visuals();RememberVisible();var target=interaction.Target as DichTuongTacHanhLyR41;
  preview.SetActive(ready&&memory.held==0&&target&&target.activity==this&&target.arrange);
  bool show=ready&&Held&&interaction.Target==null;hint.transform.parent.gameObject.SetActive(show);
  hint.text=(memory.held==0?"E · Xem mặt kia":memory.paperOpen?"E · Gấp tờ ghi":"E · Mở tờ ghi")+"    Cuộn · Xoay    R · Đặt lại";
  UpdateHub();
 }
 void RememberVisible(){if(!Ready||!Held||memory.held==1&&hinge>=-125)return;var prop=memory.held==0?tag:paper;Vector3 point=prop.TransformPoint(new Vector3(memory.held==0?0:.073f,.002f,0));var view=eye.WorldToViewportPoint(point);if(view.z<=0||view.z>2||view.x<.05f||view.x>.95f||view.y<.05f||view.y>.95f)return;
  Vector3 normal=prop.up*(memory.held==0&&memory.tagBack?-1:1);if(Vector3.Dot(normal,(eye.transform.position-point).normalized)<.1f)return;var delta=point-eye.transform.position;foreach(var h in Physics.RaycastAll(eye.transform.position,delta.normalized,delta.magnitude-.003f,~0,QueryTriggerInteraction.Ignore))if(!h.transform.IsChildOf(story.player.transform)&&!h.transform.IsChildOf(root))return;
  int bit=1<<memory.held;if((memory.viewed&bit)==0){memory.viewed|=bit;story.Save();}
 }
 void Visuals(){if(!eye)return;
  PlaceVisual(tag,memory.held==0,memory.tagSlot==1?tagArranged:tagRest,memory.tagTurn,0);
  PlaceVisual(paper,memory.held==1,paperRest,memory.paperTurn,1);
  tagCollider.enabled=memory.held!=0;paperCollider.enabled=memory.held!=1;
  if(Ready)hinge=Mathf.MoveTowards(hinge,memory.paperOpen?-180:0,550*Time.unscaledDeltaTime);
  flap.localRotation=Quaternion.Euler(0,0,hinge);paperInside.gameObject.SetActive(hinge< -125);paperCover.gameObject.SetActive(hinge> -80);
  paperCollider.center=new Vector3(memory.paperOpen?0:.073f,.003f,0);paperCollider.size=new Vector3(memory.paperOpen?.29f:.145f,.012f,.202f);
 }
 void PlaceVisual(Transform prop,bool held,Transform socket,float yaw,int id){
  if(held){prop.SetParent(eye.transform,false);prop.localScale=Vector3.one;prop.localRotation=Quaternion.Euler(-65,0,0)*Quaternion.Euler(id==0&&memory.tagBack?180:0,yaw,0);Vector3 offset=id==0?new Vector3(.08f,.06f,.42f):new Vector3(.02f,.07f,.60f);float reach=offset.magnitude;
   foreach(var h in Physics.SphereCastAll(eye.transform.position,.12f,eye.transform.TransformDirection(offset.normalized),reach,~0,QueryTriggerInteraction.Ignore))if(!h.transform.IsChildOf(story.player.transform)&&!h.transform.IsChildOf(root)&&h.distance>.001f)reach=Mathf.Min(reach,Mathf.Max(.14f,h.distance-.02f));prop.localPosition=offset.normalized*reach;
  }else{prop.SetParent(root,true);prop.SetPositionAndRotation(socket.position,socket.rotation);prop.localScale=Vector3.one;}
 }
 string AllNotes(){string value="";var l=GetComponent<TuongTacSoTayHoR37>();if(l&&l.memory.viewed!=0)value+="CUỐN SỔ\n"+l.Notes+"\n\n";var m=GetComponent<QuanLyGoiHangChoR35>();if(m&&m.memory.viewed!=0)value+="QUẦY HÀ\n"+m.Notes+"\n\n";var school=GetComponent<TuongTacGiayLop203R32>();if(school){if((school.memory.viewed&1)!=0)value+="LỚP 203\nBài của Minh có ý riêng bị gạch, yêu cầu viết lại.\n";if((school.memory.viewed&2)!=0)value+="Bài mẫu được nhận xét ‘Đúng dàn ý’, không ghi tên.\n";}if(story.data.traces[0])value+="Tờ lịch ở lan can dừng ở một ngày.\n";if(story.data.traces[1])value+="Chồng sách ở quầy chưa mở.\n";if(story.data.schoolTrace)value+="Ghế giáo viên quay ra cửa sổ.\n";if(story.data.slopeTrace)value+="Vali cạnh đường đã phủ bụi.\n";if(memory.viewed!=0)value+="\nBÊN CHIẾC VALI\n"+Notes;var journey=GetComponent<TraiNghiemHanhTrinhR23>();if(journey&&journey.journeyR44&&!string.IsNullOrEmpty(journey.KnownConnectionsR44))value+="\n\n"+journey.KnownConnectionsR44;return value;}
 void UpdateHub(){bool hub=story.data.region<0&&!story.DreamHub&&!story.MenuOpen&&!story.Loading&&!story.Paused;
  var k=Keyboard.current;bool ending=hub&&!string.IsNullOrEmpty(story.data.ending);
  if(ending&&k!=null&&k.pKey.wasPressedThisFrame)endingComparison=!endingComparison;
  if(endingComparison&&k!=null&&k.tabKey.wasPressedThisFrame)endingComparison=false;
  string value=AllNotes();bool shown=hub&&value.Length>0&&ending&&endingComparison;
  hubPanel.SetActive(shown);notes.text="GHI NHỚ ĐÃ XEM\n\n"+value;
  hubBackdrop.SetActive(shown&&ending);
  endingPanel.SetActive(shown&&ending);
  endingMessage.text="ĐIỆN THOẠI\n\n"+(story.polish.HasMessage?"Vy: "+story.polish.VyMessage:"Không có tin nhắn mới.")+"\n\nP / Tab · Cất ghi nhớ";
  // This is read-only; no PhoneChoice, point, call, post or new hub transition.
  if(ending&&!shown&&value.Length>0){hint.transform.parent.gameObject.SetActive(true);hint.text="P · Xem ghi nhớ cạnh điện thoại";}
 }
 Text Panel(Transform parent,string name,Vector2 pos,Vector2 size,int font){var box=new GameObject(name,typeof(RectTransform),typeof(Image));box.transform.SetParent(parent,false);var r=box.GetComponent<RectTransform>();r.anchorMin=r.anchorMax=r.pivot=new Vector2(0,1);r.anchoredPosition=pos;r.sizeDelta=size;box.GetComponent<Image>().color=new Color(.025f,.035f,.032f,.97f);box.GetComponent<Image>().raycastTarget=false;var g=new GameObject("Text",typeof(RectTransform),typeof(Text));g.transform.SetParent(box.transform,false);r=g.GetComponent<RectTransform>();r.anchorMin=Vector2.zero;r.anchorMax=Vector2.one;r.offsetMin=new Vector2(18,10);r.offsetMax=new Vector2(-18,-10);var t=g.GetComponent<Text>();t.font=Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");t.fontSize=font;t.color=new Color(.92f,.89f,.8f);t.raycastTarget=false;t.supportRichText=false;return t;}
 void BuildUI(){var g=new GameObject("R41_PreparationContext",typeof(Canvas),typeof(CanvasScaler));g.transform.SetParent(transform,false);var c=g.GetComponent<Canvas>();c.renderMode=RenderMode.ScreenSpaceCamera;c.worldCamera=eye;c.planeDistance=.34f;c.sortingOrder=190;DieuPhoiGiaoDienR42.ConfigureCanvas(story,c);var sc=g.GetComponent<CanvasScaler>();sc.uiScaleMode=CanvasScaler.ScaleMode.ScaleWithScreenSize;sc.referenceResolution=new Vector2(1280,720);
  hubBackdrop=new GameObject("Read_only_memory_backdrop",typeof(RectTransform),typeof(Image));hubBackdrop.transform.SetParent(g.transform,false);var bg=hubBackdrop.GetComponent<RectTransform>();bg.anchorMin=Vector2.zero;bg.anchorMax=Vector2.one;bg.offsetMin=bg.offsetMax=Vector2.zero;hubBackdrop.GetComponent<Image>().color=new Color(.025f,.035f,.032f,1);hubBackdrop.GetComponent<Image>().raycastTarget=false;hubBackdrop.SetActive(false);
  hint=Panel(g.transform,"Held_context",new Vector2(260,-650),new Vector2(760,42),18);notes=Panel(g.transform,"Remembered_drawings",new Vector2(25,-90),new Vector2(575,550),19);hubPanel=notes.transform.parent.gameObject;endingMessage=Panel(g.transform,"Existing_phone_readonly",new Vector2(630,-90),new Vector2(610,580),20);endingPanel=endingMessage.transform.parent.gameObject;hubPanel.SetActive(false);endingPanel.SetActive(false);
 }
}
