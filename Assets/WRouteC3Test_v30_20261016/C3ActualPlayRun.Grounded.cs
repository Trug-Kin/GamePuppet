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
 public bool groundedDiagnostic;
 IEnumerator RunGroundedDiagnostic()
 {
  previousBackground=Application.runInBackground;Application.runInBackground=true;
  player=GetComponent<DieuKhienDiChuyen>();cc=player.controller;player.DisableTestInputHook();
  var story=FindAnyObjectByType<DieuPhoiHanhTrinh>();story.SavePathOverride=Path.GetFullPath("Docs/Reviews/lake_gate_market_current/v36_fixture_save.json");story.Sleep();story.market.SetActive(true);story.enabled=false;
  lookController=player.GetComponent<DieuKhienGocNhin>();previousLookEnabled=lookController.enabled;lookController.enabled=false;
  previousKeyboard=Keyboard.current;previousMouse=Mouse.current;previousInputBackground=InputSystem.settings.backgroundBehavior;previousEditorInputBehavior=InputSystem.settings.editorInputBehaviorInPlayMode;
  InputSystem.settings.backgroundBehavior=InputSettings.BackgroundBehavior.IgnoreFocus;InputSystem.settings.editorInputBehaviorInPlayMode=InputSettings.EditorInputBehaviorInPlayMode.AllDeviceInputAlwaysGoesToGameView;regionalInputSettingsChanged=true;
  regionKeyboard=InputSystem.AddDevice<Keyboard>("GroundedDiagnosticKeyboard");regionMouse=InputSystem.AddDevice<Mouse>("GroundedDiagnosticMouse");regionKeyboard.MakeCurrent();regionMouse.MakeCurrent();contacts=gameObject.AddComponent<C3PlayContacts>();
  folder="Docs/Reviews/lake_gate_market_current/v36_grounded_"+DateTime.UtcNow.ToString("yyyyMMdd_HHmmss");Directory.CreateDirectory(folder);
  frames=new StreamWriter(folder+"/frames.csv"){AutoFlush=true};frames.WriteLine("case,phase,time,dt,x,y,z,foot_y,controller_grounded,ground_check,input_x,input_y,cc_vx,cc_vy,cc_vz,top_support_y,support_names,vertical_speed,horizontal_flags,vertical_flags,grounded_after_horizontal");
  var geometry=new JArray();
  var segments=new[]{(new Vector3(627.4f,0,551.8f),new Vector3(627.24f,0,554)),(new Vector3(738.6f,0,446.4f),new Vector3(737.3f,0,443.4f)),(new Vector3(627.6f,0,555),new Vector3(630,0,553))};
  foreach(var segment in segments)
  {
   for(int j=0;j<=200;j++){var p=Vector3.Lerp(segment.Item1,segment.Item2,j/200f);var hits=Physics.RaycastAll(p+Vector3.up*6,Vector3.down,12,8,QueryTriggerInteraction.Ignore).OrderByDescending(h=>h.point.y);geometry.Add(new JObject{["x"]=p.x,["z"]=p.z,["hits"]=new JArray(hits.Select(h=>new JObject{["name"]=h.collider.name,["y"]=h.point.y,["normal"]=new JArray(h.normal.x,h.normal.y,h.normal.z),["triangle"]=h.triangleIndex}))});}
  }
  File.WriteAllText(folder+"/geometry.json",geometry.ToString());
  int previousRate=Application.targetFrameRate;int previousVsync=QualitySettings.vSyncCount;QualitySettings.vSyncCount=0;
  foreach(int rate in new[]{60,100})foreach(int direction in new[]{0,1})for(int segmentIndex=0;segmentIndex<segments.Length;segmentIndex++)
  {
   Application.targetFrameRate=rate;var segment=segments[segmentIndex];var a=direction==0?segment.Item1:segment.Item2;var b=direction==0?segment.Item2:segment.Item1;
   caseId=$"segment_{segmentIndex}_{rate}fps_direction_{direction}";phase="fixture_settle";PlaceAt(a);yield return new WaitForSeconds(.5f);int first=samples.Count;contacts.hits.Clear();
   yield return RegionalGo(b,caseId,4);RegionKey(null);yield return new WaitForSeconds(.2f);
   results.Add(new JObject{["case"]=caseId,["status"]=regionalReached?"REACHED":"FAIL",["grounded_false"]=samples.Skip(first).Count(s=>!s.ccGround),["contacts"]=new JArray(contacts.hits)});
  }
  Application.targetFrameRate=previousRate;QualitySettings.vSyncCount=previousVsync;
  phase="jump_fixture_settle";PlaceAt(segments[1].Item1);yield return new WaitForSeconds(.5f);
  caseId="real_jump_and_landing";phase=caseId;float startY=player.transform.position.y;float peak=startY;int air=0;float jumpEnd=Time.realtimeSinceStartup+2;
  RegionKey(Key.Space);yield return null;yield return null;RegionKey(null);
  while(Time.realtimeSinceStartup<jumpEnd){peak=Mathf.Max(peak,player.transform.position.y);if(!cc.isGrounded)air++;yield return null;}
  results.Add(new JObject{["case"]=caseId,["status"]=air>0&&peak-startY>2.4f&&peak-startY<3.2f&&cc.isGrounded?"PASS":"FAIL",["intentional_airborne_frames"]=air,["jump_height_m"]=peak-startY,["landed"]=cc.isGrounded});
  File.WriteAllText(folder+"/result.json",new JObject{["controller"]=Profile(),["results"]=results}.ToString());frames.Dispose();frames=null;complete=true;CleanupRegionalInput();
#if UNITY_EDITOR
  UnityEditor.EditorApplication.isPlaying=false;
#else
        Application.Quit(results.All(r=>(string)r["status"]=="PASS")?0:2);
#endif
 }
}

#endif
