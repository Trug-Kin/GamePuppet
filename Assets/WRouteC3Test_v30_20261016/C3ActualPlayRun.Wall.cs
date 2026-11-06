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
 public bool wallDiagnostic;
 int loggedWallTick=-1;
 void DumpWallGeometry()
 {
  var shapes=new JArray();var position=player.transform.position;
  foreach(var c in FindObjectsByType<Collider>(FindObjectsSortMode.None))
  {
   if(Vector3.Distance(c.ClosestPoint(position),position)>2)continue;
   bool overlap=Physics.ComputePenetration(cc,position,player.transform.rotation,c,c.transform.position,c.transform.rotation,out var direction,out var depth);
   var triangles=new JArray();var meshCollider=c as MeshCollider;
   if(meshCollider&&meshCollider.sharedMesh)
   {
    var mesh=meshCollider.sharedMesh;var vertices=mesh.vertices;var indices=mesh.triangles;
    for(int i=0;i<indices.Length;i+=3)
    {
     var a=c.transform.TransformPoint(vertices[indices[i]]);var b=c.transform.TransformPoint(vertices[indices[i+1]]);var d=c.transform.TransformPoint(vertices[indices[i+2]]);
     var bounds=new Bounds(a,Vector3.zero);bounds.Encapsulate(b);bounds.Encapsulate(d);bounds.Expand(2);
     if(!bounds.Contains(position))continue;
     triangles.Add(new JObject{["a"]=a.ToString("F6"),["b"]=b.ToString("F6"),["c"]=d.ToString("F6"),["normal"]=Vector3.Cross(b-a,d-a).normalized.ToString("F6")});
    }
   }
   shapes.Add(new JObject{["name"]=c.name,["type"]=c.GetType().Name,["bounds"]=c.bounds.ToString("F6"),["overlap"]=overlap,["direction"]=direction.ToString("F6"),["depth"]=depth,["triangles"]=triangles});
  }
  File.WriteAllText(folder+"/nearby-geometry.json",shapes.ToString());
 }
 void LogWallMotion()
 {
  if((!wallDiagnostic&&!walkthroughRun)||!phase.Contains("save")||player.MovementFrames==loggedWallTick)return;
  loggedWallTick=player.MovementFrames;
  File.AppendAllText(folder+"/wall_motion.jsonl",new JObject{["phase"]=phase,["tick"]=loggedWallTick,["position"]=player.transform.position.ToString("F6"),["forward"]=player.transform.forward.ToString("F6"),["input"]=player.LastMovementInput.ToString("F4"),["requested"]=player.RequestedDisplacement.ToString("F6"),["actual"]=player.ActualDisplacement.ToString("F6"),["velocity"]=player.VerticalSpeed,["flags"]=(int)player.VerticalFlags,["controller_enabled"]=cc.enabled,["contacts"]=new JArray(contacts.frameContacts)}.ToString(Newtonsoft.Json.Formatting.None)+"\n");
 }
 IEnumerator RunWallDiagnostic()
 {
  previousBackground=Application.runInBackground;Application.runInBackground=true;player=GetComponent<DieuKhienDiChuyen>();cc=player.controller;player.DisableTestInputHook();walkingStory=FindAnyObjectByType<DieuPhoiHanhTrinh>();
  folder="Docs/Reviews/lake_gate_market_current/v36_wall_"+DateTime.UtcNow.ToString("yyyyMMdd_HHmmss");Directory.CreateDirectory(folder);walkingStory.SavePathOverride=Path.GetFullPath(folder+"/test-save.json");walkingStory.Sleep();walkingStory.Wake();walkingStory.Sleep();
  lookController=player.GetComponent<DieuKhienGocNhin>();previousLookEnabled=lookController.enabled;lookController.enabled=false;contacts=gameObject.AddComponent<C3PlayContacts>();
  previousKeyboard=Keyboard.current;previousMouse=Mouse.current;previousInputBackground=InputSystem.settings.backgroundBehavior;previousEditorInputBehavior=InputSystem.settings.editorInputBehaviorInPlayMode;InputSystem.settings.backgroundBehavior=InputSettings.BackgroundBehavior.IgnoreFocus;InputSystem.settings.editorInputBehaviorInPlayMode=InputSettings.EditorInputBehaviorInPlayMode.AllDeviceInputAlwaysGoesToGameView;regionalInputSettingsChanged=true;regionKeyboard=InputSystem.AddDevice<Keyboard>("WallDiagnosticKeyboard");regionMouse=InputSystem.AddDevice<Mouse>("WallDiagnosticMouse");regionKeyboard.MakeCurrent();regionMouse.MakeCurrent();
  frames=new StreamWriter(folder+"/frames.csv"){AutoFlush=true};frames.WriteLine("case,phase,time,dt,x,y,z,foot_y,controller_grounded,ground_check,input_x,input_y,cc_vx,cc_vy,cc_vz,top_support_y,support_names,vertical_speed,move_flags,move_flags_copy,grounded_after_move,frame_contacts");
  var saved=new Vector3(785.1028f,6.5938f,274.1815f);walkingStory.Place(saved,Quaternion.identity);yield return new WaitForSeconds(.5f);
  yield return RegionalGo(saved+Vector3.back*2,"save_wall_push",4);WalkCheck("post_graze_contacts_and_releases",regionalReached&&contacts.hits.Contains("Context_142"));
  yield return RegionalGo(saved,"save_wall_return",4);WalkCheck("return_from_graze",regionalReached);
  contacts.hits.Clear();yield return RegionalGo(new Vector3(784.8827f,saved.y,273.6947f),"save_wall_direct_push",4);WalkCheck("wall_blocks",!regionalReached&&contacts.hits.Contains("Context_142"));yield return RegionalHold("save_wall_hold");
  yield return RegionalGo(saved+Vector3.forward*.7f,"save_wall_retreat",4);WalkCheck("retreat",regionalReached);
  yield return RegionalGo(saved+Vector3.left,"save_wall_side",4);WalkCheck("side",regionalReached);
  RegionKey(null);File.WriteAllText(folder+"/result.json",new JObject{["status"]=results.All(r=>(string)r["status"]=="PASS")?"PASS":"FAIL",["results"]=results}.ToString());frames.Dispose();frames=null;complete=true;CleanupRegionalInput();
#if UNITY_EDITOR
  UnityEditor.EditorApplication.isPlaying=false;
#endif
 }
}

#endif
