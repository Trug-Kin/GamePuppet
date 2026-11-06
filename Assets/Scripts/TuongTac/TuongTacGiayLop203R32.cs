// Quản lý hai bài giấy lớp 203, cầm/đặt/đối chiếu và memory; không cộng hai trục ending.
// Component trong scene r92: Lake_Market_Story_GDD21.
// Reference public để đối chiếu Inspector (có nhánh tùy chọn): story, staging, papers, tray, outside.
// Code tự tìm/tạo: Camera, MeshFilter, MeshRenderer.
// Thứ tự chạy khai báo: 22000; giữ quan hệ khởi tạo/pose với các owner khác.
// Memory giữ tên field/schema; restore phối hợp owner State và bộ đếm SuccessfulLoads.
using System;
using UnityEngine;
using UnityEngine.InputSystem;

// Scene opt-in. Dream-paper memory is deliberately separate from both ending axes.
[DefaultExecutionOrder(22000)]
[UnityEngine.Scripting.APIUpdating.MovedFrom(true, null, "Assembly-CSharp", "Class203PapersR32")]
public sealed partial class TuongTacGiayLop203R32 : MonoBehaviour
{
 [Serializable] public sealed class Memory {
  public int version=1;
  // Stable paper IDs: 0=Minh, 1=model. 0=desk, 1=stowed, 2=tray, 3=outside tray.
  public int[] places=new int[2];
  public int viewed,choice,viewing=-1;
  public bool related,early,abandoned,puppetSeen;
 }
 public DieuPhoiHanhTrinh story;
 public bool completeR34;
 public TraiNghiemTruongR28 staging;
 public Transform[] papers;
 public Texture2D[] pages;
 public Transform tray,outside;
 public Memory memory=new Memory();
 public bool Inspecting=>view>=0;
 public int Viewing=>view;
 public int Selected=>selected;
 public bool Comparing=>story.PhoneOpen&&compare;
 public Vector2 InspectionRotation=>rotation;
 public float InspectionZoom=>magnify;
 public bool Active=>story.data.region==2;
 public string HeldNames=>memory.places[0]==1?(memory.places[1]==1?"Bài của Minh; bài mẫu":"Bài của Minh"):memory.places[1]==1?"Bài mẫu":"Không giữ giấy";
 public string Objective=>story.player.transform.position.y<2.8f?"Theo cầu thang trái tới lớp 203, gặp Cô Mai.":"Gặp Cô Mai cạnh bảng; lối TỈNH DẬY ở cửa lớp. Có thể xem bài trên bàn của Minh.";
 public static readonly string[] Names={"BÀI CỦA MINH","BÀI MẪU"};
 public static readonly string[] Contents={
  "Minh · Lựa chọn hướng học\n\nNếu con đường đã chọn không còn phù hợp, em sẽ làm gì?\n\nEm muốn thử làm một công việc khác trong một tháng, rồi tự quyết định có tiếp tục ngành học. Em chưa biết kết quả.\n\n[Gạch ngang: ‘tự quyết định’]\nNhận xét bằng mực đỏ: Ý riêng chưa theo dàn ý. Viết lại theo bài mẫu.",
  "Lựa chọn hướng học · Bài được chấp nhận\n\nNếu con đường đã chọn không còn phù hợp, em sẽ làm gì?\n\nEm sẽ tiếp tục kế hoạch đã được chọn. Mọi người đều có lý do khi khuyên em như vậy. Em sẽ cố gắng để không phụ lòng mong đợi.\n\nNhận xét: Đúng dàn ý.\n\nKhông có tên người viết."
 };
 public string ChoiceText=>memory.choice==1?"Bài mẫu nằm trong khay của Minh.":memory.choice==2?(memory.places[0]==1?"Minh đang giữ bài riêng.":"Bài riêng nằm ngoài khay của Minh."):memory.choice==3?"Đã để bài lại và tiếp tục.":"Chưa chọn vị trí nộp bài.";
 public string MaiResponse=>completeR34?ResponseR34:memory.choice==1?"Cô Mai: Em đã đặt bài mẫu vào khay. Cô nhận được rồi.":memory.choice==2?"Cô Mai: Bài của em vẫn ở ngoài khay. Cô thấy rồi; em cứ để đó.":"Cô Mai: Bài vẫn ở trên bàn. Em có thể xem lại, hoặc để lần khác.";
 Vector3[] home;Quaternion[] rotations;Vector3[] scales;Collider[][] colliders;
 InputAction close,returnItem,notebook,zoom,drag,delta,cycle;
 int view=-1,selected=-1,loads=-1,region=-99;float opened,readTime;Vector2 rotation;float magnify=1;
 bool waitForRelease,phoneNotes;Camera eye;GameObject preview;Camera previewCamera;RenderTexture previewImage;
 DieuPhoiHanhTrinh.State owner;
 public void Capture(){
  if(home==null)return;
  if(owner!=story.data){Restore();return;}
  if(completeR34&&!Active&&region==2){region=story.data.region;LeaveR34();}
  if(!Active){if(region==2)memory.abandoned=memory.choice==0;for(int i=0;i<2;i++)if(memory.places[i]==1)memory.places[i]=0;CloseVisual();}
  if(completeR34)ResolveR34();else memory.choice=memory.places[1]==2?1:memory.places[0]==3?2:0;
  memory.viewing=view;story.data.class203R32=JsonUtility.ToJson(memory);
 }
 public void Restore(){
  quickCompare=false;CloseVisual();owner=story.data;
  try{memory=string.IsNullOrEmpty(story.data.class203R32)?new Memory():JsonUtility.FromJson<Memory>(story.data.class203R32);}catch(ArgumentException){memory=new Memory();}
  if(memory==null||memory.version!=1||memory.places==null||memory.places.Length!=2)memory=new Memory();
  for(int i=0;i<2;i++)memory.places[i]=Mathf.Clamp(memory.places[i],0,3);
  memory.viewed&=3;memory.choice=Mathf.Clamp(memory.choice,0,completeR34?3:2);memory.viewing=-1;
  if(!Active)for(int i=0;i<2;i++)if(memory.places[i]==1)memory.places[i]=0;
  selected=memory.places[0]==1?0:memory.places[1]==1?1:-1;loads=story.SuccessfulLoads;region=story.data.region;
  ApplyPapers();Capture();
 }
 InputAction Key(string name,string path,InputActionType type=InputActionType.Button){var a=new InputAction(name,type,path);a.Enable();return a;}
 void Awake(){
  eye=story.player.GetComponentInChildren<Camera>();home=new Vector3[2];rotations=new Quaternion[2];scales=new Vector3[2];colliders=new Collider[2][];
  for(int i=0;i<2;i++){home[i]=papers[i].position;rotations[i]=papers[i].rotation;scales[i]=papers[i].localScale;colliders[i]=papers[i].GetComponentsInChildren<Collider>(true);}
  close=Key("Stow paper","<Keyboard>/tab");returnItem=Key("Return paper","<Keyboard>/r");notebook=Key("Notebook","<Keyboard>/n");cycle=Key("Held paper","<Keyboard>/q");
  zoom=Key("Paper zoom","<Mouse>/scroll",InputActionType.Value);drag=Key("Rotate paper","<Mouse>/leftButton");delta=Key("Paper rotation","<Mouse>/delta",InputActionType.Value);
  if(completeR34)InitR34();
 }
 void OnDestroy(){DisposeR34();foreach(var a in new[]{close,returnItem,notebook,cycle,zoom,drag,delta})a?.Dispose();if(preview)Destroy(preview);if(previewCamera)Destroy(previewCamera.gameObject);if(previewImage){previewImage.Release();Destroy(previewImage);}}
 void Update(){
  if(owner!=story.data||loads!=story.SuccessfulLoads||region!=story.data.region){
   if(region==2&&story.data.region!=2&&loads==story.SuccessfulLoads){if(completeR34)LeaveR34();else{memory.abandoned=memory.choice==0;for(int i=0;i<2;i++)if(memory.places[i]==1)memory.places[i]=0;}Capture();}
   Restore();
  }
  if(story.MenuOpen||story.Loading||story.UiInputBlocked)return;
  if(completeR34)UpdateR34();
  if(waitForRelease&&!Inspecting){
   var k=Keyboard.current;
   if(k==null||!(k.wKey.isPressed||k.aKey.isPressed||k.sKey.isPressed||k.dKey.isPressed||k.upArrowKey.isPressed||k.downArrowKey.isPressed||k.leftArrowKey.isPressed||k.rightArrowKey.isPressed||k.spaceKey.isPressed)){waitForRelease=false;story.SetSupplementInspection(false);}
  }
  if(Inspecting){
   if(!story.Paused){
    readTime+=Time.unscaledDeltaTime;if(readTime>=.4f)memory.viewed|=1<<view;
    if(drag.IsPressed()){rotation+=delta.ReadValue<Vector2>()*.45f;rotation.y=Mathf.Clamp(rotation.y,-75,75);}
    magnify=Mathf.Clamp(magnify+zoom.ReadValue<Vector2>().y*.0008f,.85f,1.35f);
    if(Time.unscaledTime-opened>.18f&&close.WasPressedThisFrame()){if(completeR34&&quickCompare)quickCompare=false;else Close();}
    if(returnItem.WasPressedThisFrame())Return();
   }
  }else if(Active&&!story.Paused&&!story.PhoneOpen){
   if(cycle.WasPressedThisFrame()){int other=selected==0?1:0;if(memory.places[other]==1)selected=other;}
   if(close.WasPressedThisFrame()&&selected>=0&&memory.places[selected]==1)Inspect(selected);
  }
  if(story.PhoneOpen&&notebook.WasPressedThisFrame())phoneNotes=!phoneNotes;
  if(Active&&staging.InitialSeen&&staging.ObservedSeconds>=1.1f&&(staging.ActionFinished||staging.Leading&&staging.LeadSeconds>=1.8f))memory.puppetSeen=true;
  if(Inspecting)story.NotifyInteraction();
  Capture();
 }
 void LateUpdate(){
  ApplyPapers();
  if(completeR34)PlacementPreviewR34();
  if(Inspecting&&preview){preview.transform.rotation=Quaternion.Euler(-75+rotation.y,rotation.x,0);preview.transform.localScale=Vector3.one*magnify;
   UnityEngine.Rendering.RenderPipeline.SubmitRenderRequest(previewCamera,new UnityEngine.Rendering.Universal.UniversalRenderPipeline.SingleCameraRequest{destination=previewImage});}
  RefreshUI();
 }
 void ApplyPapers(){if(home==null)return;
  for(int i=0;i<2;i++){
   var t=papers[i];bool show=Active&&memory.places[i]!=1;t.gameObject.SetActive(show);
   foreach(var c in colliders[i])c.enabled=show;
   t.rotation=rotations[i];t.localScale=scales[i];
   t.position=memory.places[i]==2?tray.position:memory.places[i]==3?outside.position+Vector3.right*(i*.24f):home[i];
  }
 }
 public void Inspect(int id){
  if(!Active||id<0||id>1||story.Paused||story.PhoneOpen||story.MenuOpen||story.Loading)return;
  if(Inspecting)Close();
  if(!completeR34){memory.places[id]=1;selected=id;}view=id;readTime=0;opened=Time.unscaledTime;rotation=Vector2.zero;magnify=1;
  story.polish.DismissReading();story.SetSupplementInspection(true);MakePreview(id);Capture();ApplyPapers();
 }
 void MakePreview(int id){
  if(preview)Destroy(preview);
  preview=new GameObject("R32_inspection_existing_mesh");preview.layer=30;preview.transform.position=new Vector3(0,-900,0);
  preview.AddComponent<MeshFilter>().sharedMesh=papers[id].GetComponent<MeshFilter>().sharedMesh;
  preview.AddComponent<MeshRenderer>().sharedMaterials=papers[id].GetComponent<MeshRenderer>().sharedMaterials;
  // Display instance only; source vertex data is unchanged. Dimensions match the actual paper.
  var b=preview.GetComponent<MeshFilter>().sharedMesh.bounds;preview.transform.localScale=scales[id];
  var holder=new GameObject("R32_inspection_pivot");holder.layer=30;holder.transform.position=preview.transform.position;preview.transform.SetParent(holder.transform,true);preview.transform.localPosition=-Vector3.Scale(b.center,scales[id]);preview=holder;
  if(!previewCamera){previewCamera=new GameObject("R32_inspection_camera").AddComponent<Camera>();previewCamera.enabled=false;previewCamera.cullingMask=1<<30;previewCamera.clearFlags=CameraClearFlags.SolidColor;previewCamera.backgroundColor=new Color(.14f,.15f,.15f);previewCamera.orthographic=true;previewCamera.orthographicSize=.26f;previewCamera.nearClipPlane=.01f;previewCamera.farClipPlane=3;
   previewImage=new RenderTexture(512,640,24);previewCamera.targetTexture=previewImage;
  }
  previewCamera.transform.position=new Vector3(0,-900,-1);previewCamera.transform.rotation=Quaternion.identity;
  eye.cullingMask&=~(1<<30);
 }
 void CloseVisual(){if(view<0)return;view=-1;if(preview){Destroy(preview);preview=null;}waitForRelease=true;story.NotifyInteraction();}
 public void Close(){quickCompare=false;CloseVisual();memory.viewing=-1;Capture();}
 public void Return(){if(completeR34){ReturnR34();return;}if(view<0)return;int id=view;memory.places[id]=0;Close();selected=memory.places[1-id]==1?1-id:-1;ApplyPapers();Capture();}
 public bool Place(bool inTray){
  if(!Active||Inspecting||story.Paused||story.PhoneOpen||completeR34&&(story.MenuOpen||story.Loading)||selected<0||memory.places[selected]!=1)return false;
  int id=selected;if(inTray&&id!=1){story.Say("Bạn đang giữ bài riêng; có thể đặt nó bên ngoài khay hoặc cất lại.");return false;}
  if(!staging.ActionFinished&&!(staging.Leading&&staging.LeadSeconds>=1.8f))memory.early=true;
  memory.places[id]=inTray?2:3;
  if(inTray||id==0){memory.choice=inTray?1:2;if(!completeR34||inTray)staging.RegisterPaperChoiceR32(memory.early);}
  if(!completeR34)memory.puppetSeen|=story.data.schoolWitnessed;
  selected=memory.places[1-id]==1?1-id:-1;ApplyPapers();Capture();
  story.PlayEffectAt(story.polish.pageTurn,inTray?tray.position:outside.position);story.Say(MaiResponse);story.NotifyInteraction();return true;
 }
 public string ItemPrompt(int id)=>completeR34?"E · Xem "+(id==0?"bài của Minh":"bài mẫu"):"E · Cầm / xem "+(id==0?"bài của Minh":"bài mẫu");
 public string PlacePrompt(bool inTray)=>selected<0?(inTray?"Khay nộp bài của Minh":"Bàn của Minh · E xem bài"):inTray&&selected==0?"Đang giữ bài riêng · Nhìn ngoài khay để đặt":inTray?"E · Đặt bài mẫu vào khay của Minh":"E · Đặt giấy bên ngoài khay của Minh";
 public void Relate(){if(memory.viewed==3){memory.related=!memory.related;Capture();}}

 bool compare;
}
