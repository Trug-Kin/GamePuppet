#if UNITY_EDITOR || CON_ROI_BUILD_REVIEW
using System.Collections;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public sealed partial class C3ActualPlayRun
{
 void RegionKey(Key a,Key b){InputSystem.QueueStateEvent(regionKeyboard,new UnityEngine.InputSystem.LowLevel.KeyboardState(a,b));}
 IEnumerator RunBoundaryV44()
 {
  yield return PrepareJourneyCheck("boundary");
  var gate=walkingStory.GetComponent<ManHinhTaiV44>();
  WalkCheck("actual_render_gate_completed",gate&&gate.Ready&&!walkingStory.Loading&&gate.GpuCompletionConfirmed,new JObject{["release_frame"]=gate?gate.ReleaseFrame:-1});
  yield return RegionalGo(new Vector3(-35,.94f,-35.4f),"running_jump_start_from_spawn",3);
  player.transform.rotation=Quaternion.Euler(0,180,0);contacts.hits.Clear();
  float runningMinZ=player.transform.position.z,runningMinY=player.transform.position.y,runningApex=runningMinY;
  RegionKey(Key.W,Key.Space);yield return null;yield return null;RegionKey(Key.W);
  float runningUntil=Time.realtimeSinceStartup+2;
  while(Time.realtimeSinceStartup<runningUntil){var p=player.transform.position;runningMinZ=Mathf.Min(runningMinZ,p.z);runningMinY=Mathf.Min(runningMinY,p.y);runningApex=Mathf.Max(runningApex,p.y);yield return null;}
  RegionKey(null);yield return new WaitForSeconds(.3f);
  WalkCheck("running_jump_cannot_clear_window_header",runningMinZ>=-37.25f&&runningMinY>.88f&&cc.isGrounded,new JObject{["minimum_z"]=runningMinZ,["minimum_y"]=runningMinY,["apex_y"]=runningApex,["contacts"]=new JArray(contacts.hits)});
  if(player.transform.position.y<0||player.transform.position.z< -37.5f){yield return FinishWalk();yield break;}
  // Start at the game's spawn. All approaches use live keyboard actions and CC.Move.
  float[] columns={-35f,-36.2f,-35.8f,-35.35f,-34.9f,-34.45f,-34f};
  foreach(float x in columns)
  {
   yield return RegionalGo(new Vector3(x,.94f,-36.60f),"window_approach_"+x,3);
   player.transform.rotation=Quaternion.Euler(0,180,0);contacts.hits.Clear();
   float minZ=player.transform.position.z,minY=player.transform.position.y,apex=minY;int noFloor=0;
   float until=Time.realtimeSinceStartup+2.3f;RegionKey(Key.W);
   yield return new WaitForSeconds(.3f);RegionKey(Key.W,Key.Space);yield return null;yield return null;RegionKey(Key.W);
   while(Time.realtimeSinceStartup<until)
   {
    var p=player.transform.position;minZ=Mathf.Min(minZ,p.z);minY=Mathf.Min(minY,p.y);apex=Mathf.Max(apex,p.y);
    if(!Physics.RaycastAll(p+Vector3.up*.1f,Vector3.down,15,8,QueryTriggerInteraction.Ignore).Any(h=>h.normal.y>.8f&&h.collider.name=="CR_Hub_Floor"))noFloor++;
    yield return null;
   }
   RegionKey(null);yield return new WaitForSeconds(.25f);
   WalkCheck("window_jump_push_"+x,minZ>=-37.25f&&minY>.88f&&noFloor==0&&cc.isGrounded,new JObject{["minimum_z"]=minZ,["minimum_y"]=minY,["apex_y"]=apex,["missing_floor_frames"]=noFloor,["contacts"]=new JArray(contacts.hits)});
   if(player.transform.position.y<0||player.transform.position.z< -37.5f)break;
  }
  var eye=player.GetComponentInChildren<Camera>();
  if(player.transform.position.y>0)
  {
   yield return RegionalGo(new Vector3(-35,.94f,-36),"room_retreat",3);
   foreach(var direction in new[]{new Vector3(-1,0,-1),new Vector3(1,0,-1)})
   {
    player.transform.rotation=Quaternion.LookRotation(direction);RegionKey(Key.W,Key.Space);yield return null;yield return null;RegionKey(Key.W);yield return new WaitForSeconds(1.5f);RegionKey(null);yield return new WaitForSeconds(.3f);
    WalkCheck("corner_jump_"+direction.x,player.transform.position.y>.88f&&player.transform.position.z> -37.25f&&cc.isGrounded);
    yield return RegionalGo(new Vector3(-35,.94f,-36),"corner_retreat",3);
   }
   var latch=walkingStory.hub.GetComponentInChildren<TuongTacVuonV41>();eye.transform.LookAt(latch.transform.position);yield return WalkPress(Key.E);
   WalkCheck("grille_preserves_E_garden",walkingStory.data.lanVisitMask==1);
   yield return CapturePlayerImage(Path.GetFullPath(folder+"/window_runtime.png"));
  }
  foreach(var actor in new[]{walkingStory.slopeChapter.ghost,walkingStory.completion.lan,walkingStory.completion.vy,walkingStory.schoolChapter.ghost})
  {
   var parts=actor.GetComponentsInChildren<MeshRenderer>(true);var authored=parts.Where(r=>r.enabled&&r.name.Contains("_Jaw_Cheek_Forehead")).ToArray();
   WalkCheck("unique_authored_head_"+actor.name,authored.Length==1&&authored[0].GetComponent<MeshFilter>().sharedMesh.vertexCount>150);
   WalkCheck("source_face_disabled_"+actor.name,!parts.Any(r=>r.enabled&&(r.name.StartsWith("CR_")&&r.name.EndsWith("_Face"))));
  }
  yield return FinishWalk();
 }
}

#endif
