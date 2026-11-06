// Khôi phục tại biên opt-in của scene recovery; không tự ghi save hoặc đổi tiến trình.
// Component trong scene r92: Lake_Market_Story_GDD21.
// Reference public để đối chiếu Inspector (có nhánh tùy chọn): story.
// Thứ tự chạy khai báo: 31000; giữ quan hệ khởi tạo/pose với các owner khác.
using UnityEngine;

// Opt-in on the recoveryfix derivative only. No progression changes or automatic writes.
[DefaultExecutionOrder(31000)]
[UnityEngine.Scripting.APIUpdating.MovedFrom(true, null, "Assembly-CSharp", "BoundaryRecoveryR21")]
public sealed class PhucHoiTaiBienR21 : MonoBehaviour
{
 public DieuPhoiHanhTrinh story;
 public Vector2[] perimeterEdges; // endpoint pairs of the actual terrain union, world X/Z metres
 public Vector3 verifiedFallback;
 public Quaternion fallbackRotation=Quaternion.identity;
 public float minimumWorldY=-25;
 public int Recoveries {get;private set;}
 public string LastReason {get;private set;}="";
 Vector3 safePosition;Quaternion safeRotation;
 bool haveSafe;
 float stable,invalidFor,unsupportedFor,saveHold;
 bool loadedPending;
 bool Relevant=>story&&story.player&&(story.data.region==0||story.data.region==1);
 public bool Inside(Vector3 p,float margin=0)
 {
  if(float.IsNaN(p.x)||float.IsNaN(p.y)||float.IsNaN(p.z)||float.IsInfinity(p.x)||float.IsInfinity(p.y)||float.IsInfinity(p.z)||p.y<minimumWorldY)return false;
  bool inside=false;var q=new Vector2(p.x,p.z);
  for(int i=0;i+1<perimeterEdges.Length;i+=2){var a=perimeterEdges[i];var b=perimeterEdges[i+1];var ab=b-a;float t=ab.sqrMagnitude>0?Mathf.Clamp01(Vector2.Dot(q-a,ab)/ab.sqrMagnitude):0;if(margin>0&&(q-(a+t*ab)).sqrMagnitude<margin*margin)return false;
   if((a.y>q.y)!=(b.y>q.y)&&q.x<(b.x-a.x)*(q.y-a.y)/(b.y-a.y)+a.x)inside=!inside;
  }
  return inside;
 }
 public bool Supported(Vector3 position,float margin=1)
 {
  if(!Inside(position,margin))return false;
  var c=story.player.controller;var centre=position+story.player.transform.TransformVector(c.center);float foot=centre.y-c.height*.5f;
  bool support=false;
  foreach(var h in Physics.RaycastAll(new Vector3(centre.x,foot+.15f,centre.z),Vector3.down,.4f,8,QueryTriggerInteraction.Ignore))if(!h.collider.transform.IsChildOf(story.player.transform)&&h.normal.y>=Mathf.Cos(c.slopeLimit*Mathf.Deg2Rad)){support=true;break;}
  if(!support)return false;
  var bottom=new Vector3(centre.x,foot+c.radius+.08f,centre.z);var top=new Vector3(centre.x,centre.y+c.height*.5f-c.radius,centre.z);
  foreach(var other in Physics.OverlapCapsule(bottom,top,c.radius,8,QueryTriggerInteraction.Ignore))if(!other.transform.IsChildOf(story.player.transform))return false;
  return true;
 }
 public bool AllowSave()
 {
  if(!Relevant)return true;
  bool ok=saveHold<=0&&Supported(story.player.transform.position,.5f);
  if(!ok){LastReason="save_blocked_unsafe_or_recovery_pending";Debug.Log("R21Recovery "+LastReason+" position="+story.player.transform.position.ToString("F5"));}
  return ok;
 }
 public void AfterLoad()
 {
  stable=invalidFor=unsupportedFor=0;haveSafe=false;loadedPending=Relevant;
 }
 public bool Recover(string reason)
 {
  if(!Relevant)return false;
  Physics.SyncTransforms();var target=haveSafe&&Supported(safePosition,1)?safePosition:verifiedFallback;var rotation=haveSafe&&target==safePosition?safeRotation:fallbackRotation;
  if(!Supported(target,1)){LastReason="no_valid_recovery_anchor";Debug.LogError("R21Recovery "+LastReason);return false;}
  var from=story.player.transform.position;
  story.Place(target,rotation); // Reset falling motion only at this explicit relocation.
  Recoveries++;LastReason=reason;invalidFor=stable=unsupportedFor=0;saveHold=1;
  Debug.Log("R21Recovery reason="+reason+" from="+from.ToString("F5")+" to="+target.ToString("F5")+" progression_retained=true save_written=false");return true;
 }
 void LateUpdate()
 {
  if(!Relevant){stable=invalidFor=unsupportedFor=0;haveSafe=false;loadedPending=false;return;}
  if(story.Loading||story.Paused||story.MenuOpen)return;
  saveHold=Mathf.Max(0,saveHold-Time.deltaTime);
  var p=story.player.transform.position;
  if(loadedPending){loadedPending=false;if(!Inside(p)){Recover("invalid_loaded_position");return;}}
  if(!Inside(p)){stable=0;invalidFor+=Time.deltaTime;if(invalidFor>=.15f)Recover(p.y<minimumWorldY?"below_valid_world":"outside_terrain_union");return;}
  invalidFor=0;
  // Also catch an internal unsupported hole, without interrupting a normal 3m jump.
  if(story.player.VerticalSpeed<0&&!Physics.Raycast(p+Vector3.up*.2f,Vector3.down,12,8,QueryTriggerInteraction.Ignore)){stable=0;unsupportedFor+=Time.deltaTime;if(unsupportedFor>=1.5f)Recover("unsupported_fall");return;}
  unsupportedFor=0;
  if(story.player.controller.isGrounded&&Supported(p,2)){stable+=Time.deltaTime;if(stable>=.6f){safePosition=p;safeRotation=story.player.transform.rotation;haveSafe=true;}}
  else stable=0;
 }
}
