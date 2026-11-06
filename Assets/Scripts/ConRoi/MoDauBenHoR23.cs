// Điều phối rối tại dải hồ, sight/LOS và witness theo cơ hội đi/dừng.
// Component trong scene r92: Lake_Market_Story_GDD21.
// Reference public để đối chiếu Inspector (có nhánh tùy chọn): story, preparationR89.
// Code tự tìm/tạo: Camera, DieuKhienGocNhin, BuocChanConRoiR23.
// Thứ tự chạy khai báo: 21000; giữ quan hệ khởi tạo/pose với các owner khác.
using UnityEngine;

// Opt-in encounter presentation for the r23 scene. No controller, score or gate.
[DefaultExecutionOrder(21000)]
[UnityEngine.Scripting.APIUpdating.MovedFrom(true, null, "Assembly-CSharp", "LakeOpeningR23")]
public sealed class MoDauBenHoR23 : MonoBehaviour
{
 public DieuPhoiHanhTrinh story;
 public Vector3 lakeFootAnchor;
 public TiepCanCoChuDichR89 preparationR89;
 public float stripHalfWidth=.85f; // Serialized r39 edge margin; old scenes retain the original strip.
 public Vector3 tangent=new Vector3(.763595f,0,-.645696f);
 public Vector3 landward=new Vector3(.645696f,0,.763595f);
 Camera eye;Vector3 previousPlayer,arrival,frameDelta;
 int region=-99,loads=-1;float entered,seenTravel,still,noticed;
 public bool Opportunity {get;private set;}
 public bool Observed=>story.data.region>=0&&story.data.region<2&&(story.data.experienceR22Flags&(story.data.region==0?8:16))!=0;
 public float SeenTravel=>seenTravel;
 public float VisibleSeconds=>noticed;
 public bool TraceReady(Transform trace){return Time.unscaledTime-entered>9&&Vector3.Distance(arrival,story.player.transform.position)>1.5f&&InView(trace,trace.position,5,.05f);}
 void Awake(){eye=story.player.GetComponentInChildren<Camera>();}
 public void BeginLakeEntry(){
  var camera=story.player.GetComponentInChildren<Camera>();camera.transform.localRotation=Quaternion.Euler(3,0,0);
  var mouse=story.player.GetComponentInChildren<DieuKhienGocNhin>();if(mouse)mouse.RestorePitch(3);
 }
 public bool InView(Transform root,Vector3 target,float far=18,float border=.08f){
  var v=eye.WorldToViewportPoint(target);if(v.z<.2f||v.z>far||v.x<border||v.x>1-border||v.y<.05f||v.y>.95f)return false;
  var d=target-eye.transform.position;
  foreach(var hit in Physics.RaycastAll(eye.transform.position,d.normalized,d.magnitude-.03f,~0,QueryTriggerInteraction.Ignore))
   if(!hit.transform.IsChildOf(story.player.transform)&&!hit.transform.IsChildOf(root))return false;
  return true;
 }
 void Update(){
  var d=story.data;var player=story.player.transform.position;
  if(region!=d.region||loads!=story.SuccessfulLoads){region=d.region;loads=story.SuccessfulLoads;entered=Time.unscaledTime;seenTravel=still=noticed=0;Opportunity=false;previousPlayer=arrival=player;return;}
  Vector3 delta=player-previousPlayer;previousPlayer=player;delta.y=0;frameDelta=delta;
  if(story.Loading||story.MenuOpen||story.Paused||story.PhoneOpen||region<0||region>1)return;
  var lake=story.puppets[0];
  if(region==0&&(!preparationR89||!preparationR89.OwnsMotion)){
   // A small supported strip beside the rail: immediate motion remains local
   // to the encounter rather than trailing the player through the map.
   if(delta.magnitude<1.5f){
    var p=lake.transform.position+delta;
    var feet=lake.dauTransform.position-Vector3.up*1.58f+delta;
    var rel=feet-lakeFootAnchor;
    float along=Mathf.Clamp(Vector3.Dot(rel,tangent),-stripHalfWidth,stripHalfWidth),across=Mathf.Clamp(Vector3.Dot(rel,landward),-.20f,.48f);
    var corrected=lakeFootAnchor+tangent*along+landward*across;
    p.x+=corrected.x-feet.x;p.z+=corrected.z-feet.z;if(!preparationR89||preparationR89.ClearStep(p))lake.transform.position=p;
   }
  }
 }
 void LateUpdate(){
  var d=story.data;var delta=frameDelta;
  if(region==0&&preparationR89&&preparationR89.OwnsMotion)return; // Preparation is not evidence of imitation.
  if(story.Loading||story.MenuOpen||story.Paused||story.PhoneOpen||region<0||region>1)return;
  bool visible=false;float motion=0;
  foreach(var puppet in story.puppets){
   if(!puppet.gameObject.activeInHierarchy)continue;
   var gait=puppet.GetComponent<BuocChanConRoiR23>();
   if(!gait||!puppet.dauTransform)continue;
   var head=puppet.dauTransform.position;
   var viewportHead=eye.WorldToViewportPoint(head);var viewportFoot=eye.WorldToViewportPoint(head-Vector3.up*1.50f);
   bool readable=InView(puppet.transform,head,16,.30f)&&InView(puppet.transform,head-Vector3.up*.85f,16,.20f)&&InView(puppet.transform,head-Vector3.up*1.50f,16,.10f)&&viewportHead.y-viewportFoot.y>.15f;
   if(readable){visible=true;motion=Mathf.Max(motion,gait.PlanarDelta.magnitude);}
  }
  Opportunity=visible&&Time.unscaledTime-entered>2;
  if(!Opportunity){still=0;if(!visible)seenTravel=0;return;}
  noticed+=Time.deltaTime;
  if(noticed<.7f)return;
  if(delta.magnitude>1.5f){seenTravel=still=0;return;}
  if(motion>.002f){
   // GD2 evidence must include delayed output after Minh has stopped.
   if(region==0||delta.magnitude<.008f)seenTravel+=motion;
   still=0;
  }else if(delta.magnitude<.008f&&seenTravel>(region==0?.22f:.10f))still+=Time.deltaTime;
  if(still>.45f&&!Observed){d.experienceR22Flags|=region==0?8:16;d.witnessed[region]=true;story.Save();}
 }
}
