// Sở hữu pose tiếp xúc lan can của Bảo ở LateUpdate; thoại/điểm/chương vẫn do owner cũ quản lý.
// Component trong scene r92: Lake_Market_Story_GDD21.
// Reference public để đối chiếu Inspector (có nhánh tùy chọn): story, acting, actor, rod, line, bag, calendar, reflectionLight, leftElbowTarget, leftHandTarget, rightElbowTarget, rightHandTarget.
// Code tự tìm/tạo: TraiNghiemHanhTrinhV42.
// Thứ tự chạy khai báo: 25500; giữ quan hệ khởi tạo/pose với các owner khác.
using System;
using UnityEngine;

// Authored only in the r11 derivative. Late pose owns Bao's rail contact;
// the existing dialogue, face reveal, awareness and chapter rules remain authoritative.
[DefaultExecutionOrder(25500)]
[UnityEngine.Scripting.APIUpdating.MovedFrom(true, null, "Assembly-CSharp", "BaoBridgeV48")]
public sealed class TuTheBaoBenCauV48:MonoBehaviour
{
 public DieuPhoiHanhTrinh story;
 public DienXuatHanhTrinhV45 acting;
 public Transform actor,rod,line,bag,calendar,reflectionLight;
 public Vector3 actorPosition,waterDirection,railContact;
 public Quaternion actorRotation;
 public float shoeBottomInActor;
 // Derived dam section only; old scenes retain their original bridge datum.
 public float bridgeDeckElevation=1.14006f;
 public bool migrateRaisedDamSection;
 public float previousLakePuppetRootY;
 public bool leaningOnly;
 // Only r40 opts in. Restore saves on the removed, non-source-backed r39 overlook.
 public bool restoreSourceLayoutR40;
 public int LayoutRevision=>leaningOnly?21:11;
 public Transform leftElbowTarget,leftHandTarget,rightElbowTarget,rightHandTarget;
 Quaternion torsoRest;
 DienXuatHanhTrinhV45.Rig rig;
 public float FootError {get;private set;}
 public float ForearmRailClearance {get;private set;}
 void Start()
 {
  foreach(var r in acting.rigs)if(r.role=="Bao")rig=r;
  torsoRest=rig.torso.localRotation;
 }
 public void PrepareSave(){story.data.mapLayoutRevision=LayoutRevision;}
 public void MigrateLoaded(DieuPhoiHanhTrinh.State loaded)
 {
  if(migrateRaisedDamSection&&loaded.region==0){
   float dy=bridgeDeckElevation-1.14006f;
   bool OldRoot(Vector3 p)=>float.IsFinite(p.y)&&Mathf.Abs(p.y-previousLakePuppetRootY)<.25f&&
    Vector2.Distance(new Vector2(p.x,p.z),new Vector2(story.puppets[0].transform.position.x,story.puppets[0].transform.position.z))<6;
   if(loaded.puppetPositions!=null&&loaded.puppetPositions.Length>0&&OldRoot(loaded.puppetPositions[0]))loaded.puppetPositions[0]+=Vector3.up*dy;
   if(!string.IsNullOrEmpty(loaded.lakeApproachR89))try{
    var m=JsonUtility.FromJson<TiepCanCoChuDichR89.Memory>(loaded.lakeApproachR89);
    if(m!=null&&m.version==1&&m.phase>=0&&m.phase<=5&&OldRoot(m.position)){
     m.position+=Vector3.up*dy;
     if(!string.IsNullOrEmpty(m.plants)){
      var p=JsonUtility.FromJson<BuocChanConRoiR23.PlantState>(m.plants);
      if(p!=null&&p.feet!=null&&p.feet.Length==2){for(int i=0;i<2;i++)p.feet[i]+=Vector3.up*dy;p.previous+=Vector3.up*dy;p.swingFrom+=Vector3.up*dy;p.restY+=dy;m.plants=JsonUtility.ToJson(p);}
     }
     loaded.lakeApproachR89=JsonUtility.ToJson(m);
    }
   }catch(ArgumentException){}
  }
  if(restoreSourceLayoutR40&&loaded.region==0){
   var entrance=new Vector3(660.8829346f,1.14f,508.7565918f);
   var outward=new Vector3(-.64584774f,0,-.76346624f);
   var along=new Vector3(-.76346624f,0,.64584774f);
   bool Removed(Vector3 p){var v=p-entrance;float x=Vector3.Dot(v,along),z=Vector3.Dot(v,outward);return p.y>.5f&&p.y<4.5f&&z>.25f&&z<16.8f&&((z<9.5f&&Mathf.Abs(x)<2.25f)||(z>=8.8f&&z<12&&Mathf.Abs(x)<5)||(z>=11.2f&&Mathf.Abs(x)>1.8f&&Mathf.Abs(x)<5));}
   bool relocated=Removed(loaded.position);
   if(relocated){loaded.position=story.lakeSpawn.position;loaded.rotation=story.lakeSpawn.rotation;}
   // A current-layout puppet origin can overlap this legacy recovery mask even
   // while its visible counterpart is safely authored at the railing. Preserve
   // current saved motion unless the player actually needs overlook recovery.
   // Older layouts still retain the original NPC migration policy.
   if((relocated||loaded.mapLayoutRevision<LayoutRevision)&&loaded.puppetPositions!=null&&loaded.puppetPositions.Length>0){
    // Puppet root has a legacy offset from its visible feet; detect either footprint.
    var p=loaded.puppetPositions[0];if(Removed(p)||Removed(p+new Vector3(2.03f,0,2.26f)))loaded.puppetPositions[0]=story.puppets[0].transform.position;
   }
   if(relocated)Debug.Log("R40 migrated removed r39 overlook position; progress retained; no save write");
  }
  if(loaded.mapLayoutRevision>=11){loaded.mapLayoutRevision=LayoutRevision;return;}
  // r10 stored absolute actor/driven poses. Retain story progress and the player's
  // position, but never restore the old land-fishing pose over the new authoring.
  loaded.npcPerformance="";loaded.mapLayoutRevision=LayoutRevision;
 }
 void LateUpdate()
 {
  if(rig==null||!actor.gameObject.activeInHierarchy)return;
  actor.SetPositionAndRotation(actorPosition,actorRotation);
  var forward=waterDirection.normalized;var side=Vector3.Cross(Vector3.up,forward).normalized;
  // Rest the underside of each sleeve just above the top rail; palms stay on
  // the lake side. The finite rod lift is small enough to preserve rail clearance.
  bool talking=story.polish.DialogueVisible&&story.Dialogue.StartsWith(rig.speaker,StringComparison.Ordinal);
  float lift=!leaningOnly&&talking?Mathf.Sin(Mathf.Clamp01(rig.phrase/3f)*Mathf.PI)*.035f:0;
  // V46 rounds the elbow above its control segment. Measured visible cloth in
  // r15 left an 11-12 mm idle gap; lower the contact chain by 9 mm so the cloth
  // nearly touches the top rail with a small numerical clearance.
  // Bent elbows move the rail contact farther along the tapered sleeve than
  // the old extended fishing arm. The r21 build measured a 17 mm air gap;
  // lower the leaning chain a further 15 mm while preserving the old scene.
  var contactOffset=Vector3.down*(leaningOnly?.024f:.009f);
  var elbows=new[]{leftElbowTarget.position+contactOffset,rightElbowTarget.position+contactOffset};
  var hands=new[]{leftHandTarget.position+contactOffset,rightHandTarget.position+contactOffset+Vector3.up*lift};
  for(int i=0;i<2;i++)
  {
   var shoulder=rig.frame.TransformPoint(rig.shoulder[i]);
   DieuChinhHanhTrinhV43.Segment(rig.upper[i],shoulder,elbows[i]);
   DieuChinhHanhTrinhV43.Segment(rig.fore[i],elbows[i],hands[i]);
   rig.palm[i].SetPositionAndRotation(hands[i],Quaternion.LookRotation(-forward));
  }
  if(rig.torso)rig.torso.localRotation=torsoRest*Quaternion.Euler(-6,0,0);
  if(leaningOnly)
  {
   // Pose restoration cannot reintroduce fishing props. Head attention is still
   // owned by JourneyActing; the two forearms retain their rail contacts.
   if(rod)rod.gameObject.SetActive(false);
   if(line)line.gameObject.SetActive(false);
   if(bag)bag.gameObject.SetActive(false);
  }
  else
  {
  var habit=story.GetComponent<TraiNghiemHanhTrinhV42>().BaoHabitLift;
  var grip=hands[1];var tip=grip+forward*2.5f+Vector3.up*(.35f+lift+habit*.75f);
  DieuChinhHanhTrinhV43.Segment(rod,grip-forward*.15f,tip);
  // Fishing line terminates over the water, not in the approach soil.
  var end=railContact+forward*14;end.y=.03f;
  DieuChinhHanhTrinhV43.Segment(line,tip,end);
  }
  FootError=Mathf.Abs(actor.position.y+shoeBottomInActor-bridgeDeckElevation);
  ForearmRailClearance=Mathf.Min(elbows[0].y,elbows[1].y)-railContact.y;
 }
}
