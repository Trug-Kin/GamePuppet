#if UNITY_EDITOR || CON_ROI_BUILD_REVIEW
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Collections;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using Newtonsoft.Json.Linq;
using UnityEngine;

// Human input only. Recording starts after F9 AND real movement keys, never on automatic results.
[DefaultExecutionOrder(-19000)]
public sealed class C3ManualWalk : MonoBehaviour
{
    C3RouteFixture fixture;
    DieuKhienDiChuyen player;
    Camera eye;
    bool armed, recorded, finished;
    float pitch, sensitivity, began;
    int frames, movementFrames;
    string folder;
    StreamWriter csv;
    static readonly CultureInfo Inv=CultureInfo.InvariantCulture;
    public bool ProbeRunning { get; private set; }
    public IEnumerator ProbeInputSystem()
    {
        if(ProbeRunning)yield break;
        ProbeRunning=true;
        fixture.PrepareManualSpawn();
        var originalKeyboard = Keyboard.current;
        var originalMouse = Mouse.current;
        var keyboard = InputSystem.AddDevice<Keyboard>("ConRoiAutomatedProbe");
        var mouse = InputSystem.AddDevice<Mouse>("ConRoiAutomatedProbe");
        bool previousBackground=Application.runInBackground;
        Application.runInBackground=true;
        var results = new JArray();
        try
        {
            yield return new WaitForSeconds(.8f);
            foreach (var key in new[] { Key.W, Key.S, Key.A, Key.D })
            {
                Vector3 before = player.transform.position;
                int frame = player.MovementFrames;
                float until = Time.realtimeSinceStartup + .2f;
                Vector2 observed = Vector2.zero;
                float requested=0,actual=0,elapsed=0;
                while (Time.realtimeSinceStartup < until)
                {
                    InputSystem.QueueStateEvent(keyboard, new KeyboardState(key));
                    yield return null;
                    requested+=player.RequestedDisplacement.magnitude;
                    actual+=player.ActualDisplacement.magnitude;
                    elapsed+=Time.deltaTime;
                    if (player.LastMovementInput.sqrMagnitude > observed.sqrMagnitude) observed = player.LastMovementInput;
                }
                InputSystem.QueueStateEvent(keyboard, new KeyboardState());
                yield return null; yield return null;
                var delta = player.transform.position - before;
                var horizontal = new Vector2(delta.x, delta.z).magnitude;
                results.Add(new JObject { ["key"] = key.ToString(), ["action_value"] = observed.ToString(),
                    ["movement_frames"] = player.MovementFrames-frame, ["horizontal_m"] = horizontal,
                    ["requested_sum_m"]=requested,["actual_sum_m"]=actual,["simulated_seconds"]=elapsed,
                    ["position"]=player.transform.position.ToString("F6"),["cc_enabled"]=player.controller.enabled,
                    ["collision_flags"]=player.controller.collisionFlags.ToString(),
                    ["grounded"] = player.controller.isGrounded, ["hook"] = player.TestInputHookActive,
                    ["status"] = observed.sqrMagnitude > .5f && horizontal > .1f && player.controller.isGrounded ? "PASS" : "FAIL" });
            }
            Vector3 stopped = player.transform.position;
            yield return new WaitForSeconds(.5f);
            Vector3 drift = player.transform.position-stopped; drift.y=0;
            results.Add(new JObject { ["case"]="stop",["drift_m"]=drift.magnitude,["status"]=drift.magnitude<.01f?"PASS":"FAIL" });
            var yawBefore=player.transform.rotation;
            var pitchBefore=eye.transform.localRotation;
            Cursor.lockState=CursorLockMode.Locked;
            InputSystem.QueueStateEvent(mouse, new MouseState { delta = new Vector2(24, -12) });
            yield return null;
            yield return null;
            float yawChange=Quaternion.Angle(yawBefore,player.transform.rotation);
            float pitchChange=Quaternion.Angle(pitchBefore,eye.transform.localRotation);
            results.Add(new JObject { ["case"]="mouse_action",["action_value"]=player.LookInput.ToString(),
                ["yaw_degrees"]=yawChange,["pitch_degrees"]=pitchChange,
                ["status"]=yawChange>.1f&&pitchChange>.1f?"PASS":"FAIL" });
            Directory.CreateDirectory("Docs/Reviews/W_route_Unity_v30_20261016/editor_live");
            File.WriteAllText("Docs/Reviews/W_route_Unity_v30_20261016/editor_live/input_system_probe.json",
                new JObject { ["utc"]=DateTime.UtcNow.ToString("o"), ["source"]="Virtual Keyboard/Mouse -> queued Input System events -> Gameplay actions -> existing controller; no delegate input; not OS or human input",
                    ["actions_enabled"]=player.GameplayActionsEnabled, ["results"]=results }.ToString());
        }
        finally
        {
            InputSystem.RemoveDevice(keyboard); InputSystem.RemoveDevice(mouse);
            Application.runInBackground=previousBackground;
            ProbeRunning=false;
            originalKeyboard?.MakeCurrent(); originalMouse?.MakeCurrent();
        }
    }
    void Start()
    {
        fixture=GetComponent<C3RouteFixture>();
        if(gameObject.scene.path!="Assets/WRouteC3Test_v30_20261016/DaLatMovementTest.unity"||!fixture.manualWalkFromW1)throw new InvalidOperationException("Exact manual scene required");
        fixture.ValidateProfile();player=fixture.player;player.DisableTestInputHook();
        if(GetComponent<C3ActualPlayRun>())throw new InvalidOperationException("Automatic runner present");
        eye=player.GetComponentInChildren<Camera>();if(!eye)throw new InvalidOperationException("Missing player camera");
        var oldLook=player.GetComponent<DieuKhienGocNhin>();sensitivity=oldLook?oldLook.mouseSensitivity:500;
        // Gameplay look is owned by MouseMovement, independently of recording.
        Debug.Log("MANUAL W1 ready. Click Game to capture mouse. F9 arms recorder; only real WASD/Space starts a separate session. F10 ends recording. No manual verdict is generated automatically.");
    }
    void Update()
    {
        if(!player)return;
        if(player.TestInputHookActive||player.enableAutomatedTestInput)throw new InvalidOperationException("Automatic input must stay disabled in manual mode");
        if(Input.GetKeyDown(KeyCode.F9)&&!finished)armed=true;
        bool moved=Input.GetKey(KeyCode.W)||Input.GetKey(KeyCode.A)||Input.GetKey(KeyCode.S)||Input.GetKey(KeyCode.D)||Input.GetKey(KeyCode.Space);
        if(armed&&!recorded&&!finished&&moved)StartRecording();
        if(recorded&&moved)movementFrames++;
        if(Input.GetKeyDown(KeyCode.F10))Finish("USER_FINISHED_RECORDING");
    }
    void StartRecording()
    {
        fixture.ValidateProfile();recorded=true;began=Time.realtimeSinceStartup;
        folder="Docs/Reviews/W_route_Unity_v30_20261016/manual/session_"+DateTime.UtcNow.ToString("yyyyMMdd_HHmmss_fffffff");
        Directory.CreateDirectory(folder);csv=new StreamWriter(folder+"/manual_frames.csv"){AutoFlush=true};
        csv.WriteLine("time,dt,x,y,z,grounded,ground_probe,input_x,input_y,W,A,S,D,jump,mouse_x,mouse_y,auto_hook,cc_vx,cc_vy,cc_vz");
        File.WriteAllText(folder+"/started.json",new JObject{["status"]="HUMAN_MOVEMENT_KEYS_OBSERVED_RECORDING",["scene_path"]=gameObject.scene.path,["source"]="real keyboard/mouse only; no automated case inference",["utc"]=DateTime.UtcNow.ToString("o")}.ToString());
    }
    void LateUpdate()
    {
        if(csv==null)return;var p=player.transform.position;var v=player.controller.velocity;var input=player.ReadMovementInput();
        string F(float f)=>f.ToString("R",Inv);
        csv.WriteLine(string.Join(",",F(Time.realtimeSinceStartup),F(Time.deltaTime),F(p.x),F(p.y),F(p.z),player.controller.isGrounded,Physics.CheckSphere(player.groundCheck.position,player.groundDistance,player.groundMask),F(input.x),F(input.y),Input.GetKey(KeyCode.W),Input.GetKey(KeyCode.A),Input.GetKey(KeyCode.S),Input.GetKey(KeyCode.D),Input.GetKey(KeyCode.Space),F(Input.GetAxis("Mouse X")),F(Input.GetAxis("Mouse Y")),player.TestInputHookActive,F(v.x),F(v.y),F(v.z)));frames++;
    }
    void Finish(string why)
    {
        armed=false;if(!recorded||finished)return;finished=true;csv?.Dispose();csv=null;
        File.WriteAllText(folder+"/session.json",new JObject{["status"]="MANUAL_RAW_SESSION_RECORDED_NO_VERDICT",["reason"]=why,["duration_s"]=Time.realtimeSinceStartup-began,["recorded_frames"]=frames,["movement_key_frames"]=movementFrames,["automatic_input"]=false,["assessment"]="PENDING_USER_OBSERVATIONS_AND_TELEMETRY_REVIEW",["route_completed"]="NOT_ASSERTED",["stuck"]="NOT_ASSESSED",["jitter"]="NOT_ASSESSED",["slide"]="NOT_ASSESSED",["grounded_loss"]="NOT_ASSESSED"}.ToString());
        Debug.Log("MANUAL raw session saved: "+folder+". No traversal PASS or observation inferred.");
    }
    void OnGUI()
    {
        GUI.Box(new Rect(10,10,590,48),"MANUAL W1 → P32 → W2 → C3 | WASD + chuột | Esc thả chuột\nF9 bật ghi • F10 kết thúc | "+(finished?"Đã lưu raw, chờ đánh giá":recorded?"ĐANG GHI MANUAL":armed?"Chờ bạn bấm phím di chuyển":"Chưa ghi"));
    }
    void OnDisable(){Finish("PLAY_ENDED");Cursor.lockState=CursorLockMode.None;Cursor.visible=true;}
}

#endif
