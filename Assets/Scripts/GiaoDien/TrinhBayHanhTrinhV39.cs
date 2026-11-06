// Trình bày hành trình/chương cuối và các reference rối hiện có.
// Component trong scene r92: Lake_Market_Story_GDD21.
// Reference public để đối chiếu Inspector (có nhánh tùy chọn): story, wire, step, strings, endingPhone, endingJournal, renderer.
// Code tự tìm/tạo: Camera, TraiNghiemTruongR28, ChuanBiHanhLyR41.
// Thứ tự chạy khai báo: 20000; giữ quan hệ khởi tạo/pose với các owner khác.
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

// Optional presentation for the complete journey; checkpoint builds do not contain this component.
[DefaultExecutionOrder(20000)]
[UnityEngine.Scripting.APIUpdating.MovedFrom(true, null, "Assembly-CSharp", "JourneyPresentationV39")]
public sealed class TrinhBayHanhTrinhV39 : MonoBehaviour
{
 public DieuPhoiHanhTrinh story;
 public AudioClip wire,step;
 public Renderer[] strings;
 public Transform endingPhone,endingJournal;
 Vector3 eyeRest;
 float idle,stepTravel;
 int signature=-1;
 Quaternion phoneRest,journalRest;
 readonly Queue<(float time,Vector2 input)> pending=new Queue<(float,Vector2)>();
 Vector2 delayed;
 struct WireState {public Renderer renderer;public Vector3 scale;public int index;}
 WireState[] wires;
 bool initialized;
 public float LastDelay {get;private set;}
 public int TriggeredLayers=>GetComponentsInChildren<AudioSource>().Count(s=>s.isPlaying&&!s.loop);
 void Awake()
 {
  Initialize();
 }
 void Initialize()
 {
  if(initialized)return;initialized=true;
  eyeRest=story.player.GetComponentInChildren<Camera>().transform.localPosition;
  phoneRest=endingPhone.localRotation;journalRest=endingJournal.localRotation;
  wires=strings.Select(r=>new WireState{renderer=r,scale=r.transform.localScale,index=Index(r.name)}).ToArray();
  story.player.MovementFilter=Filter;
 }
 static int Index(string name){for(int i=1;i<=5;i++)if(name.Contains("String_"+i+"_"))return i-1;return 4;}
 Vector2 Filter(Vector2 raw)
 {
  if(story.data.region==2&&GetComponent<TraiNghiemTruongR28>()){pending.Clear();delayed=raw;LastDelay=0;return raw;}
  if(story.data.compliance<2||story.data.region<0||story.Paused){pending.Clear();delayed=raw;LastDelay=0;return raw;}
  pending.Enqueue((Time.time,raw));
  while(pending.Count>0&&Time.time-pending.Peek().time>=.18f){var item=pending.Dequeue();delayed=item.input;LastDelay=Time.time-item.time;}
  return new Vector2(delayed.x,raw.y); // Only lateral responses are delayed at high compliance.
 }
 public void Apply()
 {
  Initialize();
  pending.Clear();delayed=Vector2.zero;idle=stepTravel=0;signature=-1;
  if(story.player){var eye=story.player.GetComponentInChildren<Camera>();eye.transform.localPosition=eyeRest;}
  endingPhone.localRotation=phoneRest;endingJournal.localRotation=journalRest;
  if(!string.IsNullOrEmpty(story.data.ending))story.player.GetComponentInChildren<Camera>().transform.LookAt((endingPhone.position+endingJournal.position)*.5f);
 }
 void LateUpdate()
 {
  var d=story.data;var eye=story.player.GetComponentInChildren<Camera>().transform;
  if(!story.Paused&&story.player.enabled)
  {
   bool still=story.player.LastMovementInput.sqrMagnitude<.01f&&story.player.LookInput.sqrMagnitude<.01f;
   idle=still?idle+Time.deltaTime:0;
   bool freeSchool=d.region==2&&GetComponent<TraiNghiemTruongR28>();
   float nudge=!freeSchool&&d.compliance>=1&&idle>4?Mathf.Sin(Mathf.Clamp01((idle-4)/1.2f)*Mathf.PI)*.025f:0;
   eye.localPosition=eyeRest+new Vector3(nudge,nudge*.3f,0);
   if(!freeSchool&&idle>4&&idle-Time.deltaTime<=4&&d.compliance>=1)story.PlayEffect(wire);
   if(story.player.controller.isGrounded){stepTravel+=new Vector2(story.player.ActualDisplacement.x,story.player.ActualDisplacement.z).magnitude;if(stepTravel>1.5f+d.compliance*.12f){stepTravel=0;story.PlayEffect(step);}}
  }
  bool endingMoment=d.region==3&&d.finalStarted&&d.finalElapsed>=9;
  int key=((d.traces[0]?1:0)|(d.traces[1]?2:0)|(d.schoolTrace?4:0)|(d.slopeTrace?8:0)|(d.finalStarted?16:0)|((d.ending??"").GetHashCode()<<5))^(endingMoment?0x40000000:0);
  if(signature!=key&&wires!=null)
  {
   signature=key;bool[] traces={d.traces[0],d.traces[1],d.schoolTrace,d.slopeTrace,false};
   foreach(var item in wires)
   {
    Color color=traces[item.index]?new Color(.18f,.16f,.13f):new Color(.62f,.61f,.56f);
    string ending=endingMoment?DieuPhoiHanhTrinh.ResolveEnding(d.awareness,d.interactions):d.ending;
    if(item.index==4&&!string.IsNullOrEmpty(ending))color=ending=="C"?new Color(.85f,.76f,.4f):ending=="B"?new Color(.88f,.88f,.83f):ending=="A2"?Color.gray:new Color(.18f,.18f,.18f);
    var block=new MaterialPropertyBlock();item.renderer.GetPropertyBlock(block);block.SetColor("_BaseColor",color);item.renderer.SetPropertyBlock(block);
    item.renderer.transform.localScale=item.scale*(d.finalStarted&&d.region==3?1.015f:1);
   }
  }
  if(!string.IsNullOrEmpty(d.ending))
  {
   bool phone=d.ending=="A"||d.ending=="A2"||d.ending=="C";
   endingPhone.localRotation=phoneRest*Quaternion.Euler(phone?-12:0,0,0);
   endingJournal.localRotation=journalRest*Quaternion.Euler(d.ending=="B"?-8:0,0,0);
   if(!story.Paused&&!(GetComponent<ChuanBiHanhLyR41>()?.MemoryOpen??false)&&Keyboard.current!=null&&Keyboard.current.eKey.wasPressedThisFrame){d.endingBeat=Mathf.Min(d.endingBeat+1,3);story.Save();}
  }
 }
 public static string EndingText(string ending,int beat)
 {
  string[] lines=ending=="C"?new[]{"Điện thoại đổ chuông. Minh nhìn trang giấy trước mặt, rồi gọi về nhà.","Minh: Bố mẹ ơi… con vẫn nói là con ổn. Thật ra con đang sợ.","Minh: Con không muốn học tiếp chỉ vì sợ làm bố mẹ buồn. Con chưa biết bước sau, nhưng con muốn tự chịu trách nhiệm.","Mẹ: Mẹ đang nghe.\nMinh để trang nhật ký mở. Ngoài cửa, một ngày mới vẫn chưa có đáp án."}:ending=="B"?new[]{"Minh thức dậy. Những đường dây vẫn nằm trong trí nhớ.","Cậu viết: ‘Không ai giữ tay mình.’ Rồi dừng bút.","Điện thoại nằm im. Trang nhật ký vẫn thiếu dòng tiếp theo.","Minh đã nhận ra. Cuộc gọi vẫn chưa được thực hiện."}:ending=="A2"?new[]{"Minh gọi về nhà. Số quen thuộc hiện trên màn hình.","Mẹ: Dạo này con thế nào?\nMinh: Con ổn mà.","Mẹ: Con có chuyện gì muốn nói không?\nMinh: Con ổn mà. Mọi thứ vẫn đúng kế hoạch.","Cuộc gọi kết thúc. Minh nhìn bản thân phản chiếu trên màn hình tối."}:new[]{"Minh thức dậy, mở điện thoại như mọi sáng.","Một trang nhật ký giấc mơ được đăng lên. Cậu không đọc lại.","Thông báo mới sáng lên. Ngón tay lướt qua màn hình, theo đúng trình tự.","Con rối nằm trên bàn. Dây đã đứt từ lâu.\nNgày mai, Minh vẫn đặt báo thức cùng một giờ."};
  return lines[Mathf.Clamp(beat,0,3)];
 }
 void OnGUI()
 {
  if(story.polish)return;
  if(string.IsNullOrEmpty(story.data.ending))return;
  var style=new GUIStyle(GUI.skin.box){fontSize=20,wordWrap=true,alignment=TextAnchor.MiddleCenter};
  GUI.Box(new Rect(Screen.width*.15f,Screen.height*.55f,Screen.width*.7f,200),EndingText(story.data.ending,story.data.endingBeat)+"\n\n"+(story.data.endingBeat<3?"E · Tiếp tục":"CON RỐI · Hết\nF5 · Lưu / F6 · Tải"),style);
  if(story.data.endingBeat==3&&GUI.Button(new Rect(Screen.width/2-80,Screen.height*.55f+210,160,40),"Chơi lại"))story.Restart();
 }
 void OnDestroy(){if(story&&story.player)story.player.MovementFilter=null;}
}
