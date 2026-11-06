// Bổ sung hoàn tất/trải nghiệm hành trình theo vùng và telemetry hiện có.
// Component trong scene r92: Lake_Market_Story_GDD21.
// Reference public để đối chiếu Inspector (có nhánh tùy chọn): story, vy, lan, lanArm, baoArm, haArm, paperRustle, cloth, watering, lakeReflection, bookLight, traceDetails.
// Code tự tìm/tạo: TraiNghiemHanhTrinhR23, Camera.
// Thứ tự chạy khai báo: 22000; giữ quan hệ khởi tạo/pose với các owner khác.
using System;
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;

// Authored only into v41. Optional scenes do not change the four awareness traces.
[DefaultExecutionOrder(22000)]
[UnityEngine.Scripting.APIUpdating.MovedFrom(true, null, "Assembly-CSharp", "JourneyCompletionV41")]
public sealed class HoanTatHanhTrinhV41 : MonoBehaviour
{
 public DieuPhoiHanhTrinh story;
 public Transform vy,lan,lanArm,baoArm,haArm;
 public AudioClip paperRustle,cloth,watering;
 public Light lakeReflection,bookLight;
 public bool externalPerformance;
 public Transform[] traceDetails;
 Quaternion baoRest,haRest,lanRest;
 Vector3 vyRest,previous;
 int previousRegion=-99,previousNext=-99;
 float areaAt,vyAt=-1,lanAt=-1,interactionUntil;
 bool[] traceSeen=new bool[4];
 StreamWriter activity;
 double lastTick;
 public bool VyVisible=>vy&&vy.gameObject.activeSelf;
 public int LanVisits=>story.data.lanVisitMask;
 void Start()
 {
  vyRest=vy.position;baoRest=baoArm.localRotation;haRest=haArm.localRotation;lanRest=lanArm.localRotation;
  previous=story.player.transform.position;
  var args=Environment.GetCommandLineArgs();int i=Array.IndexOf(args,"--verification-output");
  string dir=i>=0&&i+1<args.Length?args[i+1]:Path.Combine(Application.persistentDataPath,"v41-sessions",DateTime.UtcNow.ToString("yyyyMMdd_HHmmss"));
  Directory.CreateDirectory(dir);activity=new StreamWriter(Path.Combine(dir,"activity-exclusive.csv"));
  activity.WriteLine("time,area,category,seconds,distance_m");
 }
 public string NextTalk(int region)
 {
  string[] lines=region==0?new[]{
   "Ông Bảo: Ngày nào bác cũng ra đây. Chỗ này nhìn được cả mặt hồ.",
   "Ông Bảo: Có lần bác định nghỉ một bữa. Đi được nửa đường rồi lại quay về. Không ai gọi cả.",
   "Ông Bảo: Tờ lịch vẫn để ngày ấy. Bác cứ nghĩ mai rồi xé."
  }:new[]{
   "Chị Hà: Hồi đó ai cũng bảo học ngành ấy sẽ ổn. Chị gấp sách lại rồi ra đây bán hàng.",
   "Chị Hà: Mỗi tối chị xếp lại từng ấy thứ. Khách hỏi cái nào, chị cũng lấy đúng cái người trước đã mua.",
   "Chị Hà: Sách còn nguyên bọc. Chị giữ lại, mà chẳng biết là cho mình hay cho người đã chọn hộ."
  };
  int n=region==0?story.data.baoTalks:story.data.haTalks;
  if(n>=lines.Length)return null;
  if(region==0)story.data.baoTalks++;else story.data.haTalks++;
  var journey=GetComponent<TraiNghiemHanhTrinhR23>();if(region==0&&journey&&journey.journeyR44)return journey.BaoTalkR44(n);
  return lines[n];
 }
 public void VisitLan()
 {
  if(story.data.region>=0||!string.IsNullOrEmpty(story.data.ending))return;
  int stage=Mathf.Clamp(story.data.nextRegion,0,3),mask=1<<stage;
  if((story.data.lanVisitMask&mask)!=0)return;
  story.data.lanVisitMask|=mask;lanAt=Time.time;
  string[] lines={"Bà Lan: Bác từng tính mở cửa vườn cho mấy đứa sang đọc sách. Để bác dọn thêm chút đã.","Bà Lan: Người ta bảo lối mở thế khó trông nhà. Bác đóng thêm một thanh thôi.","Bà Lan: Cây vẫn đủ nắng mà cháu. Qua khe này cũng nhìn được.","Bà Lan: Cổng lâu không mở, bản lề cứng mất rồi. Thôi, ngồi bên này cũng được."};
  story.Say(lines[stage]);story.PlayEffectAt(watering,lan.position);story.polish.Event("Lan_garden",stage.ToString());story.Save();
 }
 void LateUpdate()
 {
  var d=story.data;float now=Time.time;
  if(d.region!=previousRegion||d.nextRegion!=previousNext)
  {previousRegion=d.region;previousNext=d.nextRegion;areaAt=Time.unscaledTime;vyAt=-1;if(!externalPerformance)vy.position=vyRest;}
  bool home=d.region<0&&string.IsNullOrEmpty(d.ending);lan.gameObject.SetActive(home);
  if(!externalPerformance&&home&&!story.Paused)
  {
   // Watering hand follows a finite arc, with no task or awareness reward.
   lanArm.localRotation=lanRest*Quaternion.Euler(-12+Mathf.Sin(now*.7f)*7,0,8);
   bool engaged=lanAt>=0&&now-lanAt<8;
   if(engaged)lanArm.localRotation=lanRest*Quaternion.Euler(-35,0,15);
   lan.rotation=Quaternion.Slerp(lan.rotation,Quaternion.Euler(0,engaged?175:15,0),Time.deltaTime*1.4f);
  }
  if(!externalPerformance){
  bool market=d.region==1&&!story.Paused;
  float near=Vector3.Distance(story.player.transform.position,vyRest);
  if(market&&!d.vyEncountered&&near<12)
  {d.vyEncountered=true;vyAt=now;story.polish.Event("Vy_market_once","ordinary silhouette; no awareness reward");story.Save();}
  if(market&&vyAt>=0&&now-vyAt>=7)
  {
   var eye=story.player.GetComponentInChildren<Camera>();var target=vy.position+Vector3.up*1.5f;var view=eye.WorldToViewportPoint(target);
   // Leave the scene only after the observer has looked away or scenery hides
   // the silhouette. Never make an ordinary classmate vanish in open view.
   bool outside=view.z<0||view.x<-.1f||view.x>1.1f||view.y<-.1f||view.y>1.1f;
   if(outside)vyAt=-1;
  }
  bool visible=market&&vyAt>=0;
  vy.gameObject.SetActive(visible);
  if(visible)
  {
   float t=now-vyAt;int level=Mathf.Clamp(d.ignored,0,2);
   vy.position=vyRest+Vector3.back*(2-level)*2+Vector3.right*Mathf.Clamp(t-2,0,5)*.35f;
   // The retained model faces local -Z. Turn its face away from the observer.
   var toward=story.player.transform.position-vy.position;toward.y=0;
   if(toward.sqrMagnitude>.01f)vy.rotation=Quaternion.LookRotation(toward);
  }
  }
  Measure();
  if(story.Paused)return;
  if(!externalPerformance&&d.region==0)baoArm.localRotation=Quaternion.Slerp(baoArm.localRotation,baoRest*Quaternion.Euler(-18,0,-12),Time.deltaTime*2);
  if(!externalPerformance&&d.region==1)haArm.localRotation=haRest*Quaternion.Euler(-12-Mathf.Sin(now*.9f)*9,0,8);
  bool[] traces={d.traces[0],d.traces[1],d.schoolTrace,d.slopeTrace};
  for(int n=0;n<4;n++)
  {
   if(traces[n]&&!traceSeen[n]&&d.region==n){story.PlayEffectAt(n==1?cloth:paperRustle,traceDetails[n].position);story.polish.Event("trace_environment",n.ToString());}
   traceSeen[n]=traces[n];
  }
  // The calendar and sealed books leave a quiet persistent change, without a popup.
  if(lakeReflection)lakeReflection.intensity=d.traces[0]?.12f:.4f;
  if(bookLight)bookLight.intensity=d.traces[1]?.9f:.3f;
 }
 void Measure()
 {
  double now=Time.realtimeSinceStartupAsDouble,dt=lastTick>0?now-lastTick:0;lastTick=now;
  Vector3 p=story.player.transform.position;float distance=Vector3.Distance(p,previous);previous=p;
  if(distance>2)distance=0;
  var key=Keyboard.current;
  if(key!=null&&key.eKey.wasPressedThisFrame)interactionUntil=Time.unscaledTime+.2f;
  // Mutually exclusive precedence. Observation is an input state, not inferred intent.
  string category=story.Paused?"paused":Time.unscaledTime-areaAt<.65f?"transition":story.polish.IsReading?"document_display":story.PhoneOpen?"phone":!string.IsNullOrEmpty(story.data.ending)?"ending_caption":Time.unscaledTime<interactionUntil?"interaction_input":story.polish.DialogueVisible?"dialogue_display":distance>.001f?"movement":"stationary_observation";
  string area=!string.IsNullOrEmpty(story.data.ending)?"Ending_"+story.data.ending:story.data.region<0?"Hub_"+story.data.nextRegion:new[]{"Lake","Market","School","Slope"}[story.data.region];
  if(dt>0)activity.WriteLine(FormattableString.Invariant($"{now:F4},{area},{category},{dt:F5},{distance:F5}"));
  if(Time.frameCount%120==0)activity.Flush();
 }
 void OnDestroy(){activity?.Dispose();}
}
