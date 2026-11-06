#if UNITY_EDITOR || CON_ROI_BUILD_REVIEW
using System;
using System.Collections;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

// Extension of the existing runner, attached only by Editor during verification.
// All traversal uses virtual keyboard events -> Gameplay actions -> unchanged controller.
public sealed partial class C3ActualPlayRun
{
    public TextAsset regionalRoute;
    public bool regionalViewsOnly;
    Keyboard regionKeyboard,previousKeyboard;
    Mouse regionMouse,previousMouse;
    InputSettings.BackgroundBehavior previousInputBackground;
    InputSettings.EditorInputBehaviorInPlayMode previousEditorInputBehavior;
    bool regionalInputSettingsChanged;
    bool regionalReached;
    float minimumRegionalClearance=float.PositiveInfinity;
    int regionalPenetrations,regionalMissingSupport;
    void RegionKey(Key? key)
    {
        UnityEngine.InputSystem.InputSystem.QueueStateEvent(regionKeyboard,key.HasValue?new KeyboardState(key.Value):new KeyboardState());
    }
    void CleanupRegionalInput()
    {
        if(regionalInputSettingsChanged)
        {
            UnityEngine.InputSystem.InputSystem.settings.backgroundBehavior=previousInputBackground;
            UnityEngine.InputSystem.InputSystem.settings.editorInputBehaviorInPlayMode=previousEditorInputBehavior;
            regionalInputSettingsChanged=false;
        }
        if(regionKeyboard!=null&&regionKeyboard.added)UnityEngine.InputSystem.InputSystem.RemoveDevice(regionKeyboard);
        if(regionMouse!=null&&regionMouse.added)UnityEngine.InputSystem.InputSystem.RemoveDevice(regionMouse);
        if(previousKeyboard!=null&&previousKeyboard.added)previousKeyboard.MakeCurrent();
        if(previousMouse!=null&&previousMouse.added)previousMouse.MakeCurrent();
        regionKeyboard=null;regionMouse=null;
    }
    IEnumerator RegionalGo(Vector3 target,string name,float timeout=6)
    {
        if(player.speed<12)timeout*=12f/player.speed;
        phase=name;regionalReached=false;float deadline=Time.realtimeSinceStartup+timeout;
        while(Time.realtimeSinceStartup<deadline)
        {
            var delta=target-player.transform.position;delta.y=0;
            if(delta.magnitude<.23f){regionalReached=true;break;}
            player.transform.rotation=Quaternion.LookRotation(delta);
            RegionKey(Key.W);yield return null;
            var at=player.transform.position;
            var hits=Physics.RaycastAll(at+Vector3.up*.05f,Vector3.down,2,8,QueryTriggerInteraction.Ignore).Where(h=>h.normal.y>.5f).OrderByDescending(h=>h.point.y).ToArray();
            if(hits.Length==0){regionalMissingSupport++;if(ongDaoRun)File.AppendAllText(folder+"/missing_support.txt",phase+" "+at.ToString("F6")+" all="+string.Join(";",Physics.RaycastAll(at+Vector3.up,Vector3.down,4,8,QueryTriggerInteraction.Ignore).Select(h=>h.collider.name+":"+h.point.ToString("F5")+":"+h.normal.ToString("F5")))+"\n");}
            else{float clearance=at.y-.9f-hits[0].point.y;minimumRegionalClearance=Mathf.Min(minimumRegionalClearance,clearance);if(clearance<-.015f)regionalPenetrations++;}
        }
        RegionKey(null);
    }
    IEnumerator RegionalHold(string name)
    {
        phase=name;RegionKey(null);yield return null;yield return new WaitForSeconds(.25f);
        var start=player.transform.position;float maximum=0,low=start.y,high=start.y;int lost=0,count=0;float end=Time.realtimeSinceStartup+5;
        while(Time.realtimeSinceStartup<end)
        {
            var p=player.transform.position;var d=p-start;d.y=0;maximum=Mathf.Max(maximum,d.magnitude);low=Mathf.Min(low,p.y);high=Mathf.Max(high,p.y);if(!cc.isGrounded)lost++;count++;yield return null;
        }
        float slope=Physics.Raycast(player.transform.position,Vector3.down,out var surface,2,8)?Vector3.Angle(Vector3.up,surface.normal):-1;
        results.Add(new JObject{["case"]=name,["status"]=maximum<=.01f&&high-low<=.005f&&lost==0?"PASS":"FAIL",["hold_seconds"]=5,["surface_slope_deg"]=slope,["position_xyz"]=new JArray(start.x,start.y,start.z),["horizontal_drift_m"]=maximum,["y_peak_to_peak_m"]=high-low,["grounded_false_frames"]=lost,["samples"]=count});
    }
    IEnumerator RunRegional()
    {
        if(regionalViewsOnly){yield return RunMaterialViewsOnly();yield break;}
        previousBackground=Application.runInBackground;Application.runInBackground=true;
        if(gameObject.scene.path!="Assets/Scenes/JourneyExperience_v42.unity"&&gameObject.scene.path!="Assets/Scenes/JourneyExperience_v41.unity"&&gameObject.scene.path!="Assets/Scenes/JourneyExperience_v40.unity"&&gameObject.scene.path!="Assets/Scenes/JourneyIntegration_v39.unity"&&gameObject.scene.path!="Assets/Scenes/SchoolIntegration_v38.unity"&&gameObject.scene.path!="Assets/Scenes/LakeMarketIntegration_v36.unity"&&gameObject.scene.path!="Assets/Scenes/LakeMarketStory_v35.unity"&&gameObject.scene.path!="Assets/Scenes/LakeGateMarket_v32.unity"&&gameObject.scene.path!="Assets/Scenes/LakeGateMarket_v33.unity"&&gameObject.scene.path!="Assets/Scenes/LakeMarketStory_v34.unity")throw new Exception("Wrong regional scene");
        player=GetComponent<DieuKhienDiChuyen>();cc=player.controller;
        if(cc.height!=1.8f||cc.radius!=.3f||cc.stepOffset!=.3f||cc.slopeLimit!=45||cc.skinWidth!=.03f||player.speed!=(gameObject.scene.path=="Assets/Scenes/JourneyExperience_v42.unity"||gameObject.scene.path=="Assets/Scenes/JourneyExperience_v40.unity"||gameObject.scene.path=="Assets/Scenes/JourneyExperience_v41.unity"?3.6f:12)||Time.timeScale!=1)throw new Exception("Frozen profile/timeScale differs");
        player.DisableTestInputHook();
        if(gameObject.scene.path=="Assets/Scenes/LakeGateMarket_v33.unity"||gameObject.scene.path=="Assets/Scenes/LakeMarketStory_v34.unity")yield return WarmMaterialShaders();
        lookController=player.GetComponent<DieuKhienGocNhin>();previousLookEnabled=lookController&&lookController.enabled;
        if(lookController)lookController.enabled=false;
        contacts=player.gameObject.AddComponent<C3PlayContacts>();
        previousKeyboard=Keyboard.current;previousMouse=Mouse.current;
        previousInputBackground=UnityEngine.InputSystem.InputSystem.settings.backgroundBehavior;
        previousEditorInputBehavior=UnityEngine.InputSystem.InputSystem.settings.editorInputBehaviorInPlayMode;
        UnityEngine.InputSystem.InputSystem.settings.backgroundBehavior=InputSettings.BackgroundBehavior.IgnoreFocus;
        UnityEngine.InputSystem.InputSystem.settings.editorInputBehaviorInPlayMode=InputSettings.EditorInputBehaviorInPlayMode.AllDeviceInputAlwaysGoesToGameView;
        regionalInputSettingsChanged=true;
        regionKeyboard=UnityEngine.InputSystem.InputSystem.AddDevice<Keyboard>("RegionalVirtualKeyboard");
        regionMouse=UnityEngine.InputSystem.InputSystem.AddDevice<Mouse>("RegionalVirtualMouse");regionKeyboard.MakeCurrent();regionMouse.MakeCurrent();
        folder=VerificationFolder("route");Directory.CreateDirectory(folder);
        frames=new StreamWriter(folder+"/frames.csv"){AutoFlush=true};frames.WriteLine("case,phase,time,dt,x,y,z,foot_y,controller_grounded,ground_check,input_x,input_y,cc_vx,cc_vy,cc_vz,top_support_y,support_names,vertical_speed,move_flags,move_flags_copy,grounded_after_move,frame_contacts");
        File.WriteAllText(folder+"/started.json",new JObject{["status"]="PLAY_STARTED",["scene"]=gameObject.scene.path,["input"]="Virtual Input System keyboard/mouse -> Gameplay actions; no delegate; no OS claim",["controller"]=Profile(),["process_id"]=System.Diagnostics.Process.GetCurrentProcess().Id}.ToString());
        RegionKey(null);yield return new WaitForSeconds(.8f);
        caseId="input_system_probe";phase="input_probe";bool inputsOk=true;
        foreach(var key in new[]{Key.W,Key.S,Key.A,Key.D})
        {
            phase="input_probe_"+key;
            var start=player.transform.position;RegionKey(key);float end=Time.realtimeSinceStartup+.065f;float seen=0;
            while(Time.realtimeSinceStartup<end){seen=Mathf.Max(seen,player.LastMovementInput.magnitude);yield return null;}
            RegionKey(null);yield return null;yield return new WaitForSeconds(.08f);
            var d=player.transform.position-start;d.y=0;
            var expected=key==Key.W?player.transform.forward:key==Key.S?-player.transform.forward:key==Key.A?-player.transform.right:player.transform.right;
            float along=Vector3.Dot(d,expected);var lateral=d-expected*along;
            bool ok=seen>.9f&&along>.1f&&lateral.magnitude<.1f&&!player.TestInputHookActive&&player.GameplayActionsEnabled;inputsOk&=ok;
            results.Add(new JObject{["case"]="InputSystem_"+key,["status"]=ok?"PASS":"FAIL",["input_peak"]=seen,["displacement_m"]=d.magnitude,["expected_direction_displacement_m"]=along,["lateral_error_m"]=lateral.magnitude,["delegate_hook"]=player.TestInputHookActive});
        }
        yield return RegionalHold("input_release_stop");
        var camera=player.GetComponentInChildren<Camera>();var originalRotation=player.transform.rotation;var originalEye=camera.transform.localRotation;
        if(lookController)lookController.enabled=true;Cursor.lockState=CursorLockMode.Locked;
        float yaw=player.transform.eulerAngles.y,pitch=camera.transform.localEulerAngles.x;
        UnityEngine.InputSystem.InputSystem.QueueStateEvent(regionMouse,new MouseState{delta=new Vector2(120,60)});yield return null;yield return null;
        float actualYaw=Mathf.Abs(Mathf.DeltaAngle(yaw,player.transform.eulerAngles.y)),actualPitch=Mathf.Abs(Mathf.DeltaAngle(pitch,camera.transform.localEulerAngles.x));
        bool lookOk=actualYaw>.1f&&actualPitch>.1f;inputsOk&=lookOk;
        results.Add(new JObject{["case"]="InputSystem_mouse_look",["status"]=lookOk?"PASS":"FAIL",["actual_yaw_deg"]=actualYaw,["actual_pitch_deg"]=actualPitch,["delegate_hook"]=player.TestInputHookActive});
        if(lookController)lookController.enabled=false;player.transform.rotation=originalRotation;camera.transform.localRotation=originalEye;
        var source=JObject.Parse(regionalRoute.text);var route=source["route"].Select(P).ToArray();
        int Nearest(Vector3 p)=>Enumerable.Range(0,route.Length).OrderBy(i=>Vector3.SqrMagnitude(new Vector3(route[i].x-p.x,0,route[i].z-p.z))).First();
        int gate=Nearest(new Vector3(730,0,416)),junction=Nearest(new Vector3(740,0,459));
        int[] screenshots={1,Nearest(new Vector3(610,0,575)),junction,gate,Nearest(new Vector3(750,0,325)),route.Length-2};
        float Horizontal(int i){var delta=route[i]-route[i-1];delta.y=0;return delta.magnitude;}
        int steep=Enumerable.Range(3,Mathf.Max(1,route.Length-6)).Where(i=>Horizontal(i)>1).OrderByDescending(i=>Mathf.Abs(route[i].y-route[i-1].y)/Horizontal(i)).First();
        bool traversalOk=true;
        for(int direction=0;direction<2;direction++)
        {
            caseId=direction==0?"continuous_forward":"continuous_reverse";int reached=0;int first=samples.Count;contacts.hits.Clear();
            foreach(int index in direction==0?Enumerable.Range(0,route.Length):Enumerable.Range(0,route.Length).Reverse())
            {
                yield return RegionalGo(route[index],caseId,index==0?12:6);
                if(!regionalReached){traversalOk=false;results.Add(new JObject{["case"]=caseId,["status"]="FAIL_TARGET_NOT_REACHED",["index"]=index,["target"]=route[index].ToString("F4"),["actual"]=player.transform.position.ToString("F4"),["actual_input"]=player.LastMovementInput.ToString("F4"),["keyboard_enabled"]=regionKeyboard.enabled,["actions_enabled"]=player.GameplayActionsEnabled,["application_focus"]=Application.isFocused,["contacts"]=new JArray(contacts.hits)});break;}
                reached++;
                if(screenshots.Contains(index)){yield return CapturePlayerImage(Path.GetFullPath(folder+"/"+caseId+"_station_"+index+".png"));}
                if(index==steep||index==gate){yield return RegionalHold(caseId+"_slope_stop_"+index);}
                File.WriteAllText(folder+"/progress.json",new JObject{["case"]=caseId,["reached_targets"]=reached,["total_targets"]=route.Length,["index"]=index}.ToString());
            }
            int lost=samples.Skip(first).Count(x=>!x.ccGround);
            results.Add(new JObject{["case"]=caseId,["status"]=reached!=route.Length?"FAIL":lost>0?"REACHED_WITH_GROUNDED_LOSS":"PASS",["reached_targets"]=reached,["total_targets"]=route.Length,["controller_grounded_false_frames"]=lost,["contacts"]=new JArray(contacts.hits)});
            if(!traversalOk)break;
        }
        // Reach the middle of the original shoreline from the final route endpoint;
        // no teleport or collision disabling is used during this entire run.
        if(traversalOk)
        {
            var middle=(route[0]+route[1])*.5f;caseId="shore_rail_obstacle";yield return RegionalGo(middle,"obstacle_approach",8);
            var axis=route[1]-route[0];axis.y=0;axis.Normalize();var towardLake=Vector3.Cross(Vector3.up,axis);var target=middle+towardLake*2.5f;
            contacts.hits.Clear();yield return RegionalGo(target,"rail_block_attempt",2);
            RegionKey(null);yield return null;
            bool railHit=source["rail_collider_names"].Any(n=>contacts.hits.Contains((string)n));
            results.Add(new JObject{["case"]="actual_imported_rail_blocks",["status"]=!regionalReached&&railHit?"PASS":"FAIL",["target_reached"]=regionalReached,["rail_contact"]=railHit,["contacts"]=new JArray(contacts.hits)});
            ScreenCapture.CaptureScreenshot(Path.GetFullPath(folder+"/rail_obstacle.png"),2);yield return RegionalHold("stop_after_obstacle");
        }
        RegionKey(null);yield return null;
        if(gameObject.scene.path=="Assets/Scenes/LakeGateMarket_v33.unity")yield return CaptureMaterialReview();
        bool all=inputsOk&&traversalOk&&regionalPenetrations==0&&regionalMissingSupport==0&&results.All(r=>(string)r["status"]=="PASS");
        File.WriteAllText(folder+"/result.json",new JObject{["status"]=all?"PASS":"EXECUTED_REVIEW_FAILURES",["suite"]="REGIONAL_V32_EXISTING_RUNNER",["scene"]=gameObject.scene.path,["input"]="Virtual Input System keyboard and mouse; no delegate hook; OS NOT_VERIFIED",["controller_after"]=Profile(),["teleports_during_run"]=0,["collision_disabled_during_run"]=false,["minimum_sampled_foot_clearance_m"]=minimumRegionalClearance,["floor_penetration_frames"]=regionalPenetrations,["missing_support_frames"]=regionalMissingSupport,["results"]=results}.ToString());
        frames.Dispose();frames=null;complete=true;CleanupRegionalInput();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying=false;
#else
        Application.Quit(results.All(r=>(string)r["status"]=="PASS")?0:2);
#endif
    }
}

#endif
