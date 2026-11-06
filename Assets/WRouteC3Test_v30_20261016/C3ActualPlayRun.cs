#if UNITY_EDITOR || CON_ROI_BUILD_REVIEW
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json.Linq;
using UnityEngine;

// Input-only driver for the existing PlayerMovement test hook. All movement/grounding is original runtime code.
[DefaultExecutionOrder(-19000)]
public sealed partial class C3ActualPlayRun : MonoBehaviour
{
    public bool routeOnly;
    C3RouteFixture fixture;
    DieuKhienDiChuyen player;
    CharacterController cc;
    JObject data;
    JArray results = new JArray();
    Vector2 input;
    string folder, caseId = "bootstrap", phase = "bootstrap";
    StreamWriter frames;
    bool complete;
    bool previousBackground;
    DieuKhienGocNhin lookController;
    bool previousLookEnabled;
    List<Sample> samples = new List<Sample>();
    C3PlayContacts contacts;
    static readonly CultureInfo Inv = CultureInfo.InvariantCulture;
    struct Sample { public Vector3 pos; public float t; public bool ccGround, probeGround; public string phase; }
    static Vector3 P(JToken q) => new Vector3((float)q[0], (float)q[1], (float)q[2]);
    IEnumerator Start()
    {
        var bootStory=FindAnyObjectByType<DieuPhoiHanhTrinh>();
        while(bootStory&&bootStory.Loading)yield return null;
        if(journeySuite!=null&&journeySuite.StartsWith("acceptance-")){yield return RunAcceptanceV47();yield break;}
        if(journeySuite=="gdd"){yield return RunGddV47();yield break;}
        if(journeySuite=="acting"){yield return RunActingV45();yield break;}
        if(journeySuite=="hitches"){yield return RunHitchesV45();yield break;}
        if(journeySuite=="boundary"){yield return RunBoundaryV44();yield break;}
        if(journeySuite=="experience"){yield return RunExperienceWalkthrough();yield break;}
        if(journeySuite=="stability"){yield return RunJourneyStability();yield break;}
        if(journeySuite=="branches"){yield return RunEndingBranches();yield break;}
        if(wallDiagnostic){yield return RunWallDiagnostic();yield break;}
        if(schoolRun){yield return RunSchool();yield break;}
        if(groundedDiagnostic){yield return RunGroundedDiagnostic();yield break;}
        if(walkthroughRun){yield return RunStoryWalkthrough();yield break;}
        if(ongDaoRun){yield return RunOngDao();yield break;}
        if(regionalRoute){yield return RunRegional();yield break;}
        previousBackground=Application.runInBackground;
        Application.runInBackground=true;
        if (gameObject.scene.path != "Assets/WRouteC3Test_v30_20261016/DaLatMovementTest.unity") throw new Exception("Wrong scene path");
        fixture = GetComponent<C3RouteFixture>();
        if(fixture.manualWalkFromW1) throw new Exception("Automatic input is disabled for the manual W1 session");
        fixture.ValidateProfile(); player = fixture.player; cc = player.controller;
        lookController=player.GetComponent<DieuKhienGocNhin>();
        if(lookController){previousLookEnabled=lookController.enabled;lookController.enabled=false;}
        if (Time.timeScale != 1) throw new Exception("Unexpected timeScale; not changing it");
        folder = "Docs/Reviews/W_route_Unity_v30_20261016/editor_live/run_" + DateTime.UtcNow.ToString("yyyyMMdd_HHmmss");
        Directory.CreateDirectory(folder); fixture.output = folder + "/fixture_raw";
        data = JObject.Parse(fixture.fixture.text);
        if(fixture.useLandscapeCases)data["cases"]=JObject.Parse(fixture.landscape.text)["cases"].DeepClone();
        var cases = (JArray)data["cases"];
        contacts = player.gameObject.AddComponent<C3PlayContacts>();
        frames = new StreamWriter(folder + "/frames.csv") { AutoFlush = true };
        frames.WriteLine("case,phase,time,dt,x,y,z,foot_y,controller_grounded,ground_check,input_x,input_y,cc_vx,cc_vy,cc_vz,top_support_y,support_names");
        Vector3 forward = P(cases[0]["points"][1]) - P(cases[0]["points"][0]); forward.y = 0; forward.Normalize();
        File.WriteAllText(folder + "/started.json", new JObject { ["status"]="PLAY_MODE_STARTED",["scene_path"]=gameObject.scene.path,["process_id"]=System.Diagnostics.Process.GetCurrentProcess().Id,["input"]="existing PlayerMovement hook; real CharacterController.Move and PhysX; no OS keyboard simulation",["controller"]=Profile() }.ToString());
        foreach (var item in routeOnly ? new JArray() : cases)
        {
            fixture.ValidateProfile(); fixture.selectedCase = cases.IndexOf(item); fixture.BeginCase();
            caseId=(string)item["id"]; samples.Clear(); contacts.hits.Clear();
            bool stop=item["points"].Count()==1;
            Vector3 target=P(item["points"][stop?0:1]);
            if(stop) PlaceAt(target-forward*1.2f);
            player.enableAutomatedTestInput=true; player.movementInputForTesting=()=>input;
            if(!player.TestInputHookActive) throw new Exception("Original movement input hook is inactive");
            input=Vector2.zero; phase="settle"; yield return new WaitForSeconds(.8f);
            Vector3 start=player.transform.position; start.y=target.y;
            Vector3 axis=target-start;axis.y=0;float distance=axis.magnitude;axis.Normalize();
            player.transform.rotation=Quaternion.LookRotation(axis);
            float began=Time.realtimeSinceStartup;phase=stop?"approach_stop":"traverse";bool reached=false;
            bool midRouteCaptured=false;
            while(Time.realtimeSinceStartup-began<8)
            {
                Vector3 delta=player.transform.position-start;delta.y=0;float remaining=distance-Vector3.Dot(delta,axis);
                var targetDelta=target-player.transform.position;targetDelta.y=0;
                if(fixture.useLandscapeCases&&!stop&&!midRouteCaptured&&targetDelta.magnitude<distance*.55f)
                {
                    ScreenCapture.CaptureScreenshot(Path.GetFullPath(folder+"/"+caseId+"_midroute_GameView.png"));
                    midRouteCaptured=true;
                }
                if(targetDelta.magnitude<=.025f){reached=true;break;}
                // Input amplitude only, including a partial final frame. speed/step/slope/skin remain untouched.
                var local=player.transform.InverseTransformDirection(targetDelta.normalized);
                input=new Vector2(local.x,local.z)*Mathf.Clamp01(targetDelta.magnitude/(player.speed*Mathf.Max(Time.deltaTime,Time.fixedDeltaTime)));
                yield return null;
            }
            input=Vector2.zero;float travelDuration=Time.realtimeSinceStartup-began;
            Vector3 actualStop=player.transform.position;phase=stop?"hold":"post_traverse";
            float holdStart=Time.realtimeSinceStartup;yield return new WaitForSeconds(stop?5:.35f);
            var motion=samples.Where(s=>s.phase=="traverse"||s.phase=="approach_stop").ToArray();
            var stable=samples.Where(s=>s.phase==(stop?"hold":"post_traverse")&&s.t>=holdStart+.2f).ToArray();
            var examined=motion.Concat(stable).ToArray();
            float lateral=motion.Length==0?0:motion.Max(s=>{var q=s.pos-start;q.y=0;return(q-axis*Vector3.Dot(q,axis)).magnitude;});
            float verticalRange=stable.Length==0?0:stable.Max(s=>s.pos.y)-stable.Min(s=>s.pos.y);
            float horizontalDrift=stable.Length==0?0:stable.Max(s=>{var q=s.pos-stable[0].pos;q.y=0;return q.magnitude;});
            int ccLost=examined.Count(s=>!s.ccGround), probeLost=examined.Count(s=>!s.probeGround);
            var report=new JObject{["case"]=caseId,["status"]=reached?"EXECUTED_REACHED":"EXECUTED_TIMEOUT",["input_frames"]=motion.Length,["duration_to_target_s"]=travelDuration,["hold_seconds_requested"]=stop?5:.35,["hold_seconds_actual"]=Time.realtimeSinceStartup-holdStart,["examined_frames"]=examined.Length,["controller_not_grounded_frames"]=ccLost,["ground_probe_false_frames"]=probeLost,["max_lateral_deviation_m"]=lateral,["stationary_y_peak_to_peak_m"]=verticalRange,["stationary_horizontal_drift_m"]=horizontalDrift,["actual_stop_xyz"]=new JArray(actualStop.x,actualStop.y,actualStop.z),["target_support_xyz"]=new JArray(target.x,target.y,target.z),["contacts"]=new JArray(contacts.hits),["stuck"]=reached?"NO_TIMEOUT_REACHED_TARGET":"TARGET_NOT_REACHED_INSPECT_CONTACTS",["jitter_observation"]=stable.Length==0?"NO_STABLE_SAMPLES":verticalRange>.005f?"MEASURED_Y_VARIATION_OVER_5MM":"NO_Y_VARIATION_OVER_5MM_IN_SAMPLES",["sliding_observation"]=stable.Length==0?"NO_STABLE_SAMPLES":horizontalDrift>.01f?"MEASURED_DRIFT_OVER_10MM":"NO_DRIFT_OVER_10MM_IN_SAMPLES",["grounded_observation"]=ccLost==0?"CONTROLLER_GROUNDED_ALL_EXAMINED_FRAMES":"CONTROLLER_GROUNDED_FALSE_RECORDED"};
            if((bool?)item["expected_block"]==true)
            {
                bool railContact=contacts.hits.Any(name=>name.StartsWith("V31_Rail"));
                report["status"]=!reached&&railContact&&ccLost==0?"PASS_BLOCKED_AT_RAIL":"FAIL_EXPECTED_RAIL_BLOCK";
                report["expected_block"]=true;
            }
            var endpoint=player.transform.position-target;endpoint.y=0;
            report["endpoint_horizontal_error_m"]=endpoint.magnitude;
            report["look_input_isolated_for_collision_test"]=true;
            results.Add(report); File.WriteAllText(folder+"/progress.json",new JObject{["completed_cases"]=results.Count,["results"]=results.DeepClone()}.ToString());
            if(caseId=="center_forward"||caseId=="center_reverse"||caseId=="stop_transition_-0.6"||caseId=="shore_forward"||caseId=="shore_reverse")
            {ScreenCapture.CaptureScreenshot(Path.GetFullPath(folder+"/"+caseId+"_GameView.png"));yield return new WaitForSeconds(.15f);}
            Debug.Log("C3 ACTUAL PLAY "+caseId+" "+report["status"]+" groundedFalse="+ccLost+" drift="+horizontalDrift+" yRange="+verticalRange);
        }
        // Regression from W1 through the west frontage and across the C3 join,
        // followed by the same route in reverse. One initial placement only.
        if(!fixture.useLandscapeCases)
        {
        var fr=data["frame"];var origin=P(fr["F"]);var u=P(fr["u"]);var n=P(fr["n"]);
        Vector3 Route(float s,float d) {var q=origin+u*s+n*d;return new Vector3(-q.x,0,-q.y);}
        float edge=(float)data["edge_a_b"][0]*23.4f+(float)data["edge_a_b"][1];
        var path=new[]{Route(-7,3.4f),Route(23.4f,3.4f),Route(23.4f,edge+2),Route(23.4f,3.4f),Route(-7,3.4f)};
        if(!Physics.Raycast(path[0]+Vector3.up*1000,Vector3.down,out var routeSupport,2000,8,QueryTriggerInteraction.Ignore))
            throw new Exception("No support at full-route initial spawn; refusing below-ground spawn");
        path[0]=new Vector3(path[0].x,routeSupport.point.y,path[0].z);
        PlaceAt(path[0]);
        player.enableAutomatedTestInput=true; player.movementInputForTesting=()=>input;
        input=Vector2.zero;yield return new WaitForSeconds(.8f);
        for(int leg=1;leg<path.Length;leg++)
        {
            caseId="W_route_leg_"+leg;phase="traverse";samples.Clear();contacts.hits.Clear();
            float deadline=Time.realtimeSinceStartup+12; bool reached=false;
            while(Time.realtimeSinceStartup<deadline)
            {
                Vector3 delta=path[leg]-player.transform.position;delta.y=0;
                if(delta.magnitude<.08f){reached=true;break;}
                player.transform.rotation=Quaternion.LookRotation(delta);
                input=Vector2.up*Mathf.Clamp01(delta.magnitude/(player.speed*Mathf.Max(Time.deltaTime,Time.fixedDeltaTime)));
                yield return null;
            }
            input=Vector2.zero;phase="hold";yield return new WaitForSeconds(.5f);
            int groundedLost=samples.Count(s=>!s.ccGround);
            results.Add(new JObject{["case"]=caseId,["status"]=!reached?"EXECUTED_TIMEOUT":groundedLost>0?"EXECUTED_GROUNDED_LOSS":"PASS",
                ["controller_not_grounded_frames"]=groundedLost,["frames"]=samples.Count,["contacts"]=new JArray(contacts.hits)});
            if(!reached)break;
        }
        }
        fixture.ValidateProfile(); input=Vector2.zero;player.DisableTestInputHook(); frames.Dispose();frames=null;
        var final=new JObject{["status"]="NINE_CASES_EXECUTED_IN_EXISTING_GUI_EDITOR",["scene_path"]=gameObject.scene.path,["batch_mode"]=Application.isBatchMode,["process_id"]=System.Diagnostics.Process.GetCurrentProcess().Id,["utc"]=DateTime.UtcNow.ToString("o"),["controller_after"]=Profile(),["results"]=results,["limitations"]="Automated input in real Play mode; sampled telemetry, not a human visual verdict. Spawn/settle excluded. No source controller settings changed."};
        final["status"]="EXECUTED";
        final["suite"]=fixture.useLandscapeCases?"P3_SHORE_V31":routeOnly?"W_ROUTE_BOTH_DIRECTIONS":"C3_NINE_AND_W_ROUTE";
        File.WriteAllText(folder+"/result.json",final.ToString());complete=true;
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying=false;
#endif
    }
    void PlaceAt(Vector3 point)
    {
        cc.enabled=false;float height=point.y;
        foreach(float x in new[]{-.3f,0,.3f})foreach(float z in new[]{-.3f,0,.3f})
            if(Physics.Raycast(point+new Vector3(x,2,z),Vector3.down,out var h,4,8,QueryTriggerInteraction.Ignore))height=Mathf.Max(height,h.point.y);
        player.transform.position=new Vector3(point.x,height+.96f,point.z);
        player.ResetVerticalMotion();
        cc.enabled=true;Physics.SyncTransforms();
    }
    JObject Profile()=>new JObject{["height"]=cc.height,["radius"]=cc.radius,["step"]=cc.stepOffset,["slope"]=cc.slopeLimit,["skin"]=cc.skinWidth,["min_move"]=cc.minMoveDistance,["speed"]=player.speed,["gravity"]=player.gravity,["jump"]=player.jumpHeight,["ground_distance"]=player.groundDistance,["scale"]=player.transform.lossyScale.ToString("F6")};
    void LateUpdate()
    {
        if(frames==null||!player)return;var pos=player.transform.position;var velocity=cc.velocity;
        if(walkingStory&&walkingStory.presentation)peakEffects=Mathf.Max(peakEffects,walkingStory.presentation.TriggeredLayers);
        bool probe=Physics.CheckSphere(player.groundCheck.position,player.groundDistance,player.groundMask);
        samples.Add(new Sample{pos=pos,t=Time.realtimeSinceStartup,ccGround=cc.isGrounded,probeGround=probe,phase=phase});
        if(regionalRoute||schoolRun)input=player.LastMovementInput;
        var hits=Physics.RaycastAll(pos+Vector3.up,Vector3.down,4,8,QueryTriggerInteraction.Ignore).Where(h=>regionalRoute||schoolRun?h.normal.y>.5f:h.collider.name.StartsWith("SUP_")).OrderByDescending(h=>h.point.y).ToArray();
        string F(float x)=>x.ToString("R",Inv);
        frames.WriteLine(string.Join(",",caseId,phase,F(Time.realtimeSinceStartup),F(Time.deltaTime),F(pos.x),F(pos.y),F(pos.z),F(pos.y-.9f),cc.isGrounded,probe,F(input.x),F(input.y),F(velocity.x),F(velocity.y),F(velocity.z),hits.Length>0?F(hits[0].point.y):"NaN",string.Join("|",hits.Select(h=>h.collider.name)),F(player.VerticalSpeed),(int)player.HorizontalFlags,(int)player.VerticalFlags,player.GroundedAfterHorizontal,contacts?string.Join("|",contacts.frameContacts):""));
        LogWallMotion();
        if(contacts)contacts.frameContacts.Clear();
    }
    void OnDisable()
    {
        CleanupRegionalInput();
        Application.runInBackground=previousBackground;
        if(lookController)lookController.enabled=previousLookEnabled;
        input=Vector2.zero;if(player)player.DisableTestInputHook();frames?.Dispose();frames=null;
        if(!complete&&!string.IsNullOrEmpty(folder))File.WriteAllText(folder+"/interrupted.json",new JObject{["status"]="INTERRUPTED_NOT_NINE_CASE_PASS",["completed"]=results}.ToString());
    }
}
public sealed class C3PlayContacts : MonoBehaviour
{
    public readonly HashSet<string> hits=new HashSet<string>();
    public readonly List<string> frameContacts=new List<string>();
    void OnControllerColliderHit(ControllerColliderHit hit){hits.Add(hit.collider.name);frameContacts.Add(hit.collider.name+":"+hit.point.ToString("F5").Replace(",",";")+":"+hit.normal.ToString("F5").Replace(",",";"));}
}

#endif
