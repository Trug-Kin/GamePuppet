// Đọc quỹ đạo bước/điểm trụ từ nguồn Blender và fit chân rối; giữ trạng thái plant khi restore.
// Component trong scene r92: ConRoi_Minh_1_2, ConRoi_Minh_1_1, ConRoi_Minh_GD3, ConRoi_Minh_1_0, R35_Roi_tai_quay, ConRoi_Minh_1_3 (các instance khác xem bảng tra cứu).
// Reference public để đối chiếu Inspector (có nhánh tùy chọn): puppet, supportR46, fittedLegsR91.
// Code tự tìm/tạo: Renderer, MeshFilter.
// Thứ tự chạy khai báo: 20900; giữ quan hệ khởi tạo/pose với các owner khác.
using System.Linq;
using UnityEngine;

// Distance-driven, planted-contact articulated wooden legs. The small swing
// trajectory is authored/reviewed in Blender and read from the shipped table.
[DefaultExecutionOrder(20900)]
[UnityEngine.Scripting.APIUpdating.MovedFrom(true, null, "Assembly-CSharp", "PuppetStrideR23")]
public sealed class BuocChanConRoiR23:MonoBehaviour
{
 public DieuKhienConRoi puppet;
 public TextAsset trajectory;
 public Collider supportR46;
 public bool driveArms=true;
 public bool keepFootPlantsOnStop=false;
 public float contactClearance=.012f;
 public bool centeredLegPivotsR89; // Existing GD3 imported segments have center pivots, no extra rig asset.
 public Transform[] fittedLegsR91;
 MeshFilter[] legMeshesR91;
 [System.Serializable] public sealed class PlantState {
  public Vector3[] feet;public Vector3 previous,swingFrom,direction;
  public float phase,stopTime,restY;public int swing;
 }
 [System.Serializable] public class Track {public float[] lift;public float stepLength=.34f;}
 Track track;Transform[] hips=new Transform[2],arms=new Transform[2];
 Vector3[] hipOffsets=new Vector3[2],feet=new Vector3[2];Quaternion[] rests=new Quaternion[2],armRests=new Quaternion[2];Vector3[] scales=new Vector3[2];
 Vector3 previous,swingFrom,direction;float length,phase,restY,stopTime;int swing;bool ready;
 public Vector3 PlanarDelta{get;private set;}
 public float MaxContactSlip{get;private set;}
 public Vector3 Foot(int i)=>feet[i];
 public bool Initialized=>ready;
 void OnEnable(){if(ready&&supportR46)ResetPose();}
 void Start(){
  track=JsonUtility.FromJson<Track>(trajectory.text);
  for(int i=0;i<2;i++){
   string suffix=i==0?"L":"R";
   hips[i]=GetComponentsInChildren<Transform>().First(t=>t.name==(centeredLegPivotsR89?"ConRoi_Leg_":"V40_Wooden_Joint_ConRoi_Leg_")+suffix);
   arms[i]=GetComponentsInChildren<Transform>().First(t=>t.name==(centeredLegPivotsR89?"ConRoi_Arm_":"V40_Wooden_Joint_ConRoi_Arm_")+suffix);
   var r=supportR46?hips[i].GetComponentsInChildren<Renderer>().First(r=>r.name=="ConRoi_Leg_"+suffix):hips[i].GetComponentInChildren<Renderer>();
   var hip=centeredLegPivotsR89?new Vector3(r.bounds.center.x,r.bounds.max.y,r.bounds.center.z):hips[i].position;
   hipOffsets[i]=hip-transform.position;length=hip.y-r.bounds.min.y;
   feet[i]=new Vector3(hips[i].position.x,r.bounds.min.y,hips[i].position.z);
   rests[i]=hips[i].rotation;scales[i]=hips[i].localScale;armRests[i]=arms[i].localRotation;
  }
  if(fittedLegsR91!=null&&fittedLegsR91.Length==2)legMeshesR91=fittedLegsR91.Select(t=>t.GetComponent<MeshFilter>()).ToArray();
  ready=true;ResetPose();
 }
 public void ResetPose(){if(!ready)return;previous=transform.position;restY=feet.Min(f=>f.y);phase=0;stopTime=0;for(int i=0;i<2;i++){hips[i].rotation=rests[i];hips[i].localScale=scales[i];feet[i]=transform.position+hipOffsets[i]-Vector3.up*length;if(keepFootPlantsOnStop&&supportR46)feet[i].y=GroundY(feet[i]);}swingFrom=feet[swing];}
 public string CapturePlantState(){return !ready?"":JsonUtility.ToJson(new PlantState{feet=feet,previous=previous,swingFrom=swingFrom,direction=direction,phase=phase,stopTime=stopTime,restY=restY,swing=swing});}
 static bool Finite(Vector3 p)=>float.IsFinite(p.x)&&float.IsFinite(p.y)&&float.IsFinite(p.z);
 public bool RestorePlantState(string json){
  if(!ready||string.IsNullOrEmpty(json))return false;
  try{var f=JsonUtility.FromJson<PlantState>(json);
   if(f==null||f.feet==null||f.feet.Length!=2||!f.feet.All(Finite)||!Finite(f.previous)||!Finite(f.swingFrom)||!Finite(f.direction)||!float.IsFinite(f.phase)||f.phase<0||f.phase>1||!float.IsFinite(f.stopTime)||f.stopTime<0||!float.IsFinite(f.restY)||f.swing<0||f.swing>1||Vector3.Distance(f.previous,transform.position)>.5f)return false;
   for(int i=0;i<2;i++)if(Vector3.Distance(f.feet[i],transform.position+hipOffsets[i])>length+.5f)return false;
   for(int i=0;i<2;i++)feet[i]=f.feet[i];previous=f.previous;swingFrom=f.swingFrom;direction=f.direction;phase=f.phase;stopTime=f.stopTime;restY=f.restY;swing=f.swing;
   // A paused load must restore rendered legs as well as the cached contact points.
   for(int i=0;i<2;i++){var hip=transform.position+hipOffsets[i];hips[i].position=centeredLegPivotsR89?(hip+feet[i])*.5f:hip;var d=feet[i]-hip;hips[i].rotation=Quaternion.FromToRotation(Vector3.down,d.normalized)*rests[i];var scale=scales[i];if(supportR46)scale.z*=d.magnitude/length;else scale.y*=d.magnitude/length;hips[i].localScale=scale;}
   FitLegsR91();return true;
  }catch(System.ArgumentException){return false;}
 }
 void LateUpdate(){
  if(!ready||Time.timeScale==0)return;
  PlanarDelta=transform.position-previous;PlanarDelta=new Vector3(PlanarDelta.x,0,PlanarDelta.z);
  if(PlanarDelta.magnitude>1.5f){ResetPose();PlanarDelta=Vector3.zero;return;}
  // Region switches and loaded roots can also move vertically.
  if(Mathf.Abs(transform.position.y-previous.y)>.5f){ResetPose();return;}
  previous=transform.position;
  bool moving=PlanarDelta.magnitude>.0005f;
  if(moving){
   stopTime=0;direction=PlanarDelta.normalized;phase+=PlanarDelta.magnitude/track.stepLength;
   if(phase>=1){phase%=1;swing=1-swing;swingFrom=feet[swing];}
   var hip=transform.position+hipOffsets[swing];
   var target=hip+direction*.17f;
   // Only the opt-in preparation gait shortens its swing beside a measured support edge.
   if(keepFootPlantsOnStop&&supportR46&&!supportR46.Raycast(new Ray(target+Vector3.up*2,Vector3.down),out _,4))target=hip;
   target.y=GroundY(target);
   float t=phase*phase*(3-2*phase);feet[swing]=Vector3.Lerp(swingFrom,target,t);
   float sample=phase*(track.lift.Length-1);int k=Mathf.Min((int)sample,track.lift.Length-2);
   feet[swing]+=Vector3.up*Mathf.Lerp(track.lift[k],track.lift[k+1],sample-k);
  }else{
   stopTime+=Time.deltaTime;
   if(supportR46&&!keepFootPlantsOnStop){
    // A short station gesture may stop before completing one whole stride.
    // Finish placing both feet under their hips, instead of keeping a stale
    // swing target from an earlier region/load or the opposite station edge.
    for(int i=0;i<2;i++){
     var target=transform.position+hipOffsets[i];target.y=GroundY(target);
     feet[i]=Vector3.MoveTowards(feet[i],target,Time.deltaTime*.65f);
    }
   }
   feet[swing]=Vector3.MoveTowards(feet[swing],new Vector3(feet[swing].x,GroundY(feet[swing]),feet[swing].z),Time.deltaTime*.8f);
   if(stopTime>.2f){phase=0;swingFrom=feet[swing];}
  }
  for(int i=0;i<2;i++){
   var hip=transform.position+hipOffsets[i];hips[i].position=centeredLegPivotsR89?(hip+feet[i])*.5f:hip;var d=feet[i]-hip;
   hips[i].rotation=Quaternion.FromToRotation(Vector3.down,d.normalized)*rests[i];
   var scale=scales[i];if(supportR46)scale.z*=d.magnitude/length;else scale.y*=d.magnitude/length;hips[i].localScale=scale;
   float arc=moving?Mathf.Sin((phase+i)*Mathf.PI)*12:0;
   if(driveArms)arms[i].localRotation=Quaternion.Slerp(arms[i].localRotation,armRests[i]*Quaternion.Euler(arc*(i==0?1:-1),0,0),1-Mathf.Exp(-Time.deltaTime*18));
  }
  FitLegsR91();
 }
 void FitLegsR91(){
  if(legMeshesR91==null)return;
  for(int i=0;i<2;i++){
   var a=transform.position+hipOffsets[i];var b=feet[i];var t=fittedLegsR91[i];t.up=a-b;
   var bounds=legMeshesR91[i].sharedMesh.bounds;float parentAxis=t.parent.TransformVector(t.localRotation*Vector3.up).magnitude;
   var scale=t.localScale;scale.y=Vector3.Distance(a,b)/(bounds.size.y*parentAxis);t.localScale=scale;
   t.position=(a+b)*.5f-t.TransformVector(bounds.center);
  }
 }
 float GroundY(Vector3 p){
  if(supportR46){
   return supportR46.Raycast(new Ray(p+Vector3.up*2,Vector3.down),out var h,4)?h.point.y+contactClearance:restY;
  }
  foreach(var h in Physics.RaycastAll(p+Vector3.up*2,Vector3.down,4,8,QueryTriggerInteraction.Ignore).OrderBy(h=>h.distance))if(!h.transform.IsChildOf(transform)&&h.normal.y>.7f)return h.point.y+contactClearance;
  return restY;
 }
}
