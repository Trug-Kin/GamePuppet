#if UNITY_EDITOR || CON_ROI_BUILD_REVIEW
using System;
using System.Collections;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
public sealed partial class C3ActualPlayRun
{
 public bool ongDaoRun;
 IEnumerator RunOngDao()
 {
  previousBackground=Application.runInBackground;Application.runInBackground=true;
  player=GetComponent<DieuKhienDiChuyen>();cc=player.controller;player.DisableTestInputHook();
  if(cc.height!=1.8f||cc.radius!=.3f||cc.stepOffset!=.3f||cc.slopeLimit!=45||Time.timeScale!=1)throw new Exception("Frozen controller differs");
  var story=FindAnyObjectByType<DieuPhoiHanhTrinh>();story.SavePathOverride=Path.GetFullPath(string.IsNullOrEmpty(VerificationOutput)?"Docs/Reviews/journey_v39/bridge-fixture-save.json":Path.Combine(VerificationOutput,"bridge-fixture-save.json"));story.Sleep();story.market.SetActive(true);story.enabled=false;
  lookController=player.GetComponent<DieuKhienGocNhin>();previousLookEnabled=lookController.enabled;lookController.enabled=false;
  previousKeyboard=Keyboard.current;previousMouse=Mouse.current;previousInputBackground=InputSystem.settings.backgroundBehavior;previousEditorInputBehavior=InputSystem.settings.editorInputBehaviorInPlayMode;
  InputSystem.settings.backgroundBehavior=InputSettings.BackgroundBehavior.IgnoreFocus;InputSystem.settings.editorInputBehaviorInPlayMode=InputSettings.EditorInputBehaviorInPlayMode.AllDeviceInputAlwaysGoesToGameView;regionalInputSettingsChanged=true;
  regionKeyboard=InputSystem.AddDevice<Keyboard>("OngDaoVirtualKeyboard");regionMouse=InputSystem.AddDevice<Mouse>("OngDaoVirtualMouse");regionKeyboard.MakeCurrent();regionMouse.MakeCurrent();contacts=gameObject.AddComponent<C3PlayContacts>();
  folder=VerificationFolder("bridge");Directory.CreateDirectory(folder);
  frames=new StreamWriter(folder+"/frames.csv"){AutoFlush=true};frames.WriteLine("case,phase,time,dt,x,y,z,foot_y,controller_grounded,ground_check,input_x,input_y,cc_vx,cc_vy,cc_vz,top_support_y,support_names,vertical_speed,move_flags,move_flags_copy,grounded_after_move,frame_contacts");
  var record=JObject.Parse((bridgeRecord ? bridgeRecord.text : File.ReadAllText("Assets/Art/Environment/OngDaoV35/bridge_record.json")));
  var link=record["connections"][0].Select(P).ToArray();
  cc.enabled=false;player.transform.position=link[0]+Vector3.up*.94f;cc.enabled=true;Physics.SyncTransforms();yield return new WaitForSeconds(1);
  yield return CaptureOngDao();
  foreach(var key in new[]{Key.W,Key.S,Key.A,Key.D}){var start=player.transform.position;RegionKey(key);yield return new WaitForSeconds(.08f);float seen=player.LastMovementInput.magnitude;RegionKey(null);yield return new WaitForSeconds(.12f);results.Add(new JObject{["case"]="InputSystem_"+key,["status"]=seen>.9f&&Vector3.Distance(start,player.transform.position)>.1f?"PASS":"FAIL",["input_peak"]=seen});}
  // Return to the initial point by walking after the input probe.
  yield return RegionalGo(link[0],"return_after_probe");
  var sidewalk=record["paths"]["sidewalk_-1"].Select(P).ToArray();var nw=sidewalk[1];var se=sidewalk[sidewalk.Length-2];
  var journeys=new System.Collections.Generic.List<(string,Vector3[])>();
  journeys.Add(("NW_link_slope",link.Take(link.Length/2+1).ToArray()));journeys.Add(("NW_link_forward",link.Skip(link.Length/2).ToArray()));journeys.Add(("NW_approach",new[]{link.Last(),nw}));
  journeys.Add(("sidewalk_minus_forward",new[]{nw,se}));
  var selink=record["connections"][1].Select(P).ToArray();journeys.Add(("SE_link_reverse",selink.Reverse().ToArray()));journeys.Add(("SE_link_forward",selink));
  foreach(var id in new[]{"rail_-1","rail_1"}){var path=record["paths"][id].Select(P).ToArray();if(id=="rail_-1")Array.Reverse(path);else{var inward=(se-nw).normalized;journeys.Add(("cross_NW_inset",new[]{nw+inward*1.5f,path[0]+inward*1.5f}));}journeys.Add((id,path));}
  var plus=record["paths"]["rail_1"].Select(P).ToArray();Array.Reverse(plus);journeys.Add(("rail_plus_reverse",plus));
  journeys.Add(("NW_cross_to_SW_sidewalk",new[]{plus[plus.Length-1]+(se-nw).normalized*1.5f,nw+(se-nw).normalized*1.5f,nw}));journeys.Add(("NW_approach_reverse",new[]{link.Last()}));journeys.Add(("NW_link_reverse",link.Reverse().ToArray()));
  foreach(var job in journeys){caseId=job.Item1;int reached=0;foreach(var target in job.Item2){yield return RegionalGo(target,caseId,15);if(!regionalReached)break;reached++;}results.Add(new JObject{["case"]=caseId,["status"]=reached==job.Item2.Length?"PASS":"FAIL",["reached"]=reached,["total"]=job.Item2.Length,["position"]=player.transform.position.ToString("F4"),["contacts"]=new JArray(contacts.hits)});yield return RegionalHold(caseId+"_stop");if(reached!=job.Item2.Length)break;}
  if(results.All(x=>(string)x["status"]=="PASS"))
  {
   bool approachOk=true;foreach(var target in link){yield return RegionalGo(target,"obstacle_approach_link",15);if(!regionalReached){approachOk=false;break;}}yield return RegionalGo(nw,"obstacle_approach_NW",15);approachOk&=regionalReached;var rail=record["paths"]["rail_-1"].Select(P).ToArray();var middle=rail[rail.Length/2];yield return RegionalGo(middle,"obstacle_approach_middle",15);approachOk&=regionalReached;results.Add(new JObject{["case"]="rail_obstacle_fixture_reached_by_walking",["status"]=approachOk?"PASS":"FAIL"});
   if(approachOk){contacts.hits.Clear();var outside=middle+(middle-(nw+se)*.5f).normalized*2;yield return RegionalGo(outside,"rail_block_attempt",2);bool blocked=!regionalReached&&contacts.hits.Any(x=>x.StartsWith("OD35_COLLIDER_"));results.Add(new JObject{["case"]="bridge_rail_blocks",["status"]=blocked?"PASS":"FAIL",["contacts"]=new JArray(contacts.hits)});yield return RegionalHold("stop_after_rail");}
  }
  RegionKey(null);yield return null;
  File.WriteAllText(folder+"/result.json",new JObject{["status"]=results.All(x=>(string)x["status"]=="PASS")&&regionalPenetrations==0&&regionalMissingSupport==0?"PASS":"EXECUTED_REVIEW_FAILURES",["scene"]=gameObject.scene.path,["input"]="Virtual Input System events, OS NOT_VERIFIED",["fixture_initial_placement"]=1,["teleports_during_traversal"]=0,["floor_penetration_frames"]=regionalPenetrations,["missing_support_frames"]=regionalMissingSupport,["grounded_false_frames_after_bootstrap"]=samples.Count(x=>x.phase!="bootstrap"&&!x.ccGround),["controller"]=Profile(),["results"]=results}.ToString());
  frames.Dispose();frames=null;complete=true;CleanupRegionalInput();
#if UNITY_EDITOR
  UnityEditor.EditorApplication.isPlaying=false;
#else
        Application.Quit(results.All(r=>(string)r["status"]=="PASS")?0:2);
#endif
 }
 IEnumerator CaptureOngDao()
 {
  var cam=player.GetComponentInChildren<Camera>();var pos=cam.transform.position;var rot=cam.transform.rotation;var fov=cam.fieldOfView;
  var views=new[]{(new Vector3(713,3,480),new Vector3(661,3,518)),(new Vector3(650,3,536),new Vector3(692,3,497)),(new Vector3(736,22,439),new Vector3(679,0,504)),(new Vector3(610,15,574),new Vector3(673,1,510))};
  var renderers=GameObject.Find("OngDao_V35_Photo_Batch").GetComponentsInChildren<Renderer>().Where(r=>r.enabled).ToArray();
  for(int i=0;i<views.Length;i++)foreach(bool before in new[]{true,false}){foreach(var r in renderers)r.enabled=!before;cam.transform.position=views[i].Item1;cam.transform.LookAt(views[i].Item2);cam.fieldOfView=65;yield return null;yield return CapturePlayerImage(Path.GetFullPath(folder+"/view_"+i+(before?"_before":"_after")+".png"));}
  foreach(var r in renderers)r.enabled=true;cam.transform.position=pos;cam.transform.rotation=rot;cam.fieldOfView=fov;
 }
}

#endif
