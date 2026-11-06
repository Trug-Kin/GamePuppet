using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

// File bridge into the already-running GUI Editor; never launches another process.
[InitializeOnLoad]
public static class C3LiveEditorBridge
{
    const string ScenePath = "Assets/WRouteC3Test_v30_20261016/DaLatMovementTest.unity";
    // Chi la output/protocol Editor; khong la dau vao scene/player.
    static string Folder => Path.Combine(Environment.GetEnvironmentVariable("CONROI_TEST_ROOT") ?? "Test", "KetQua/W_route_Unity_v30_20261016/editor_live");
    static double next;
    static bool? captureBackground;
    static void StoryPlayState(PlayModeStateChange state)
    {
        if(state!=PlayModeStateChange.EnteredPlayMode||!SessionState.GetBool("StoryV34Requested",false))return;
        EditorApplication.playModeStateChanged-=StoryPlayState;
    }
    static C3LiveEditorBridge() { EditorApplication.update += Tick;EditorApplication.playModeStateChanged+=state=>{if(state==PlayModeStateChange.EnteredEditMode)SessionState.SetString("V40Record","");}; }
    static void Write(string file, JObject value) { Directory.CreateDirectory(Folder); File.WriteAllText(Folder + "/" + file, value.ToString()); }
    static JObject State() => new JObject { ["utc"] = DateTime.UtcNow.ToString("o"), ["process_id"] = System.Diagnostics.Process.GetCurrentProcess().Id,
        ["project"] = Directory.GetParent(Application.dataPath).FullName, ["scene_path"] = SceneManager.GetActiveScene().path,
        ["scene_dirty"] = SceneManager.GetActiveScene().isDirty, ["is_compiling"] = EditorApplication.isCompiling,
        ["is_updating"] = EditorApplication.isUpdating, ["is_playing"] = EditorApplication.isPlaying, ["is_paused"] = EditorApplication.isPaused,
        ["editor_version"] = Application.unityVersion, ["batch_mode"] = Application.isBatchMode };
    static void Tick()
    {
        if (EditorApplication.timeSinceStartup < next) return; next = EditorApplication.timeSinceStartup + 1;
        Write("heartbeat.json", State());
        if(EditorApplication.isPlaying&&!EditorApplication.isCompiling&&SessionState.GetBool("V40AudioProbe",false)){SessionState.SetBool("V40AudioProbe",false);UnityEngine.Object.FindAnyObjectByType<DieuKhienDiChuyen>().gameObject.AddComponent<AudioProbeV40>();}
        if(EditorApplication.isPlaying&&!EditorApplication.isCompiling&&!string.IsNullOrEmpty(SessionState.GetString("JourneyCheck", "")))
        {var suite=SessionState.GetString("JourneyCheck", "");SessionState.SetString("JourneyCheck", "");var recording=SessionState.GetString("V40Record","");if(!string.IsNullOrEmpty(recording))C3ActualPlayRun.VerificationOutput=recording;UnityEngine.Object.FindAnyObjectByType<DieuKhienDiChuyen>().gameObject.AddComponent<C3ActualPlayRun>().journeySuite=suite;}
        if(EditorApplication.isPlaying&&!EditorApplication.isCompiling&&SessionState.GetBool("StoryV34Requested",false))
        {
            var story=UnityEngine.Object.FindAnyObjectByType<DieuPhoiHanhTrinh>();
            if(story){SessionState.SetBool("StoryV34Requested",false);story.StartCoroutine(C3ActualPlayRun.VerifyStory(story));}
        }
        if(EditorApplication.isPlaying&&!EditorApplication.isCompiling&&SessionState.GetBool("GroundedDiagnostic",false))
        {
            SessionState.SetBool("GroundedDiagnostic",false);var runner=UnityEngine.Object.FindAnyObjectByType<DieuKhienDiChuyen>().gameObject.AddComponent<C3ActualPlayRun>();runner.groundedDiagnostic=true;runner.regionalRoute=AssetDatabase.LoadAssetAtPath<TextAsset>(ConRoiRouteV32.Folder+"/regional_route.json");
        }
        if(EditorApplication.isPlaying&&!EditorApplication.isCompiling&&(SessionState.GetBool("OngDaoRequested",false)||SessionState.GetBool("WalkthroughRequested",false)))
        {
            bool walk=SessionState.GetBool("WalkthroughRequested",false);SessionState.SetBool("WalkthroughRequested",false);SessionState.SetBool("OngDaoRequested",false);var runner=UnityEngine.Object.FindAnyObjectByType<DieuKhienDiChuyen>().gameObject.AddComponent<C3ActualPlayRun>();runner.ongDaoRun=!walk;runner.walkthroughRun=walk;runner.regionalRoute=AssetDatabase.LoadAssetAtPath<TextAsset>(ConRoiRouteV32.Folder+"/regional_route.json");
        }
        if(EditorApplication.isPlaying&&!EditorApplication.isCompiling&&SessionState.GetBool("SchoolV37Requested",false))
        {
            SessionState.SetBool("SchoolV37Requested",false);
            UnityEngine.Object.FindAnyObjectByType<DieuKhienDiChuyen>().gameObject.AddComponent<C3ActualPlayRun>().schoolRun=true;
        }
        if(EditorApplication.isPlaying&&!EditorApplication.isCompiling&&SessionState.GetBool("WallDiagnosticRequested",false))
        {SessionState.SetBool("WallDiagnosticRequested",false);UnityEngine.Object.FindAnyObjectByType<DieuKhienDiChuyen>().gameObject.AddComponent<C3ActualPlayRun>().wallDiagnostic=true;}
        string command = Folder + "/command.json";
        if (!File.Exists(command) || EditorApplication.isCompiling || EditorApplication.isUpdating) return;
        var c = JObject.Parse(File.ReadAllText(command)); string nonce = (string)c["nonce"];
        // A file left by a previous Editor session is history, not a new request.
        if(File.GetLastWriteTimeUtc(command)<System.Diagnostics.Process.GetCurrentProcess().StartTime.ToUniversalTime())return;
        if (SessionState.GetString("C3LiveNonce", "") == nonce) return;
        SessionState.SetString("C3LiveNonce", nonce);
        try
        {
            if (Application.isBatchMode) throw new Exception("GUI Editor required");
            string action = (string)c["action"];
            if (action == "inspect") Inspect();
            else if (action == "refresh") AssetDatabase.Refresh();
            else if(action=="assemble_v43") ConRoiReviewV43.Assemble();
            else if(action=="build_v43") ConRoiReviewV43.Build();
            else if(action=="review_v43") ConRoiReviewV43.FinishReview();
            else if(action=="assemble_v42") ConRoiExperienceV42.Assemble();
            else if(action=="post_clearance_v42") ConRoiReviewV42.PostClearance();
            else if(action=="compare_v42_extra") ConRoiReviewV42.Compare();
            else if(action=="inspect_v42") ConRoiExperienceV42.Inspect();
            else if(action=="refine_v42") ConRoiExperienceV42.Refine();
            else if(action=="build_v42") ConRoiExperienceV42.BuildWindows();
            else if(action=="assemble_v41") ConRoiCompletionV41.Assemble();
            else if(action=="compare_v41") ConRoiCompletionV41.Comparison();
            else if(action=="refine_v41") ConRoiCompletionV41.Refine();
            else if(action=="build_v41") ConRoiCompletionV41.BuildWindows();
            else if(action=="assemble_v40") ConRoiFinishV40.Assemble();
            else if(action=="review_v40") ConRoiFinishV40.ReviewPass();
            else if(action=="inspect_v40") {EditorSceneManager.OpenScene(ConRoiFinishV40.ScenePath);ConRoiFinishV40.Inspect();}
            else if(action=="build_v40") ConRoiFinishV40.BuildWindows();
            else if(action=="probe_audio_v40"){EditorSceneManager.OpenScene(ConRoiFinishV40.ScenePath);SessionState.SetBool("V40AudioProbe",true);EditorApplication.isPlaying=true;}
            else if(action=="record_v40"){EditorSceneManager.OpenScene(ConRoiFinishV40.ScenePath);SessionState.SetString("V40Record",Path.GetFullPath(ConRoiFinishV40.Report+"/editor-recording_"+DateTime.UtcNow.ToString("yyyyMMdd_HHmmss")));SessionState.SetString("JourneyCheck","experience");EditorApplication.isPlaying=true;}
            else if(action=="assemble_v38") ConRoiSchoolV38.Assemble();
            else if(action=="assemble_v39") ConRoiSlopeV39.Assemble();
            else if(action=="finish_visuals_v39") ConRoiSlopeV39.FinishVisuals();
            else if(action=="run_v39_stability"||action=="run_v39_branches") {EditorSceneManager.OpenScene(ConRoiSlopeV39.ScenePath);SessionState.SetString("JourneyCheck",action=="run_v39_stability"?"stability":"branches");Application.runInBackground=true;EditorApplication.isPaused=false;var view=EditorWindow.GetWindow(Type.GetType("UnityEditor.GameView,UnityEditor"));view.Show();view.Focus();EditorApplication.isPlaying=true;}
            else if(action=="build_v39") ConRoiSlopeV39.BuildWindows();
            else if(action=="run_v39_walkthrough") {EditorSceneManager.OpenScene(ConRoiSlopeV39.ScenePath);SessionState.SetBool("WalkthroughRequested",true);Application.runInBackground=true;EditorApplication.isPaused=false;var view=EditorWindow.GetWindow(Type.GetType("UnityEditor.GameView,UnityEditor"));view.Show();view.Focus();EditorApplication.isPlaying=true;}
            else if(action=="finish_visuals_v38") ConRoiSchoolV38.FinishVisuals();
            else if(action=="build_v38") ConRoiSchoolV38.BuildWindows();
            else if(action=="run_v38_school"||action=="run_v38_walkthrough") {EditorSceneManager.OpenScene(ConRoiSchoolV38.ScenePath);SessionState.SetBool(action=="run_v38_school"?"SchoolV37Requested":"WalkthroughRequested",true);Application.runInBackground=true;EditorApplication.isPaused=false;var view=EditorWindow.GetWindow(Type.GetType("UnityEditor.GameView,UnityEditor"));view.Show();view.Focus();EditorApplication.isPlaying=true;}
            else if(action=="assemble_v37") ConRoiSchoolV37.Assemble();
            else if(action=="inspect_v37") ConRoiSchoolV37.Inspect();
            else if(action=="open_v36") EditorSceneManager.OpenScene(ConRoiStabilizationV36.ScenePath);
            else if(action=="repair_models_v37") ConRoiSchoolV37.RepairModels();
            else if(action=="run_v37_school") {EditorSceneManager.OpenScene(ConRoiSchoolV37.ScenePath);SessionState.SetBool("SchoolV37Requested",true);EditorApplication.isPlaying=true;}
            else if(action=="v36_geometry") ConRoiStabilizationV36.InspectGeometry();
            else if(action=="v36_wall_geometry") ConRoiStabilizationV36.InspectMarketWall();
            else if(action=="repair_v36_market") ConRoiStabilizationV36.RepairMarketWinding();
            else if(action=="assemble_v36") ConRoiStabilizationV36.Assemble();
            else if(action=="blend_v36") ConRoiStabilizationV36.BlendShoulders();
            else if(action=="run_v36_fixed") {EditorSceneManager.OpenScene(ConRoiStabilizationV36.ScenePath);SessionState.SetBool("GroundedDiagnostic",true);EditorApplication.isPlaying=true;}
            else if(action=="run_v36_wall") {EditorSceneManager.OpenScene(ConRoiStabilizationV36.ScenePath);SessionState.SetBool("WallDiagnosticRequested",true);EditorApplication.isPlaying=true;}
            else if(action=="run_v36_bridge"||action=="run_v36_walkthrough") {EditorSceneManager.OpenScene(ConRoiStabilizationV36.ScenePath);SessionState.SetBool(action=="run_v36_bridge"?"OngDaoRequested":"WalkthroughRequested",true);EditorApplication.isPlaying=true;}
            else if(action=="run_v36_route") {EditorSceneManager.OpenScene(ConRoiStabilizationV36.ScenePath);SessionState.SetBool("RegionalV34Route",true);SessionState.SetBool("RegionalV32RunRequested",true);EditorApplication.isPlaying=true;}
            else if(action=="build_v36") ConRoiStabilizationV36.BuildWindows();
            else if(action=="run_v36_grounded") {EditorSceneManager.OpenScene(ConRoiOngDaoV35.ScenePath);SessionState.SetBool("GroundedDiagnostic",true);EditorApplication.isPlaying=true;}
            else if (action == "import_shore") ConRoiShoreImport.Import();
            else if (action == "import_context") ConRoiShoreImport.ImportContext();
            else if (action == "context_palette") ConRoiShoreImport.ApplyContextPalette();
            else if (action == "import_v32") ConRoiRouteV32.ImportAndAssemble();
            else if (action == "import_v33") ConRoiRouteV33.Import();
            else if (action == "assemble_v34") ConRoiLakeMarketV34.Assemble();
            else if(action=="assemble_v35")ConRoiOngDaoV35.Assemble();
            else if(action=="run_v35_route")
            {
                if(EditorApplication.isPlayingOrWillChangePlaymode)throw new Exception("Already playing");EditorSceneManager.OpenScene(ConRoiOngDaoV35.ScenePath);SessionState.SetBool("RegionalV34Route",true);SessionState.SetBool("RegionalV32RunRequested",true);EditorApplication.isPlaying=true;
            }
            else if(action=="run_v35_bridge"||action=="run_v35_walkthrough")
            {
                if(EditorApplication.isPlayingOrWillChangePlaymode)throw new Exception("Already playing");EditorSceneManager.OpenScene(ConRoiOngDaoV35.ScenePath);SessionState.SetBool(action=="run_v35_bridge"?"OngDaoRequested":"WalkthroughRequested",true);var view=EditorWindow.GetWindow(Type.GetType("UnityEditor.GameView,UnityEditor"));view.Show();view.Focus();EditorApplication.isPlaying=true;
            }
            else if (action == "run_v34")
            {
                if(EditorApplication.isPlayingOrWillChangePlaymode)throw new Exception("Already playing");
                EditorSceneManager.OpenScene(ConRoiLakeMarketV34.ScenePath);
                SessionState.SetBool("StoryV34Requested",true);
                var view=EditorWindow.GetWindow(Type.GetType("UnityEditor.GameView,UnityEditor"));view.Show();view.Focus();
                EditorApplication.playModeStateChanged-=StoryPlayState;EditorApplication.playModeStateChanged+=StoryPlayState;
                EditorApplication.isPlaying=true;
            }
            else if(action=="run_v34_route")
            {
                if(EditorApplication.isPlayingOrWillChangePlaymode)throw new Exception("Already playing");
                EditorSceneManager.OpenScene(ConRoiLakeMarketV34.ScenePath);SessionState.SetBool("RegionalV34Route",true);SessionState.SetBool("RegionalV32RunRequested",true);EditorApplication.isPlaying=true;
            }
            else if (action == "run_v33"||action=="views_v33")
            {
                if(EditorApplication.isPlayingOrWillChangePlaymode)throw new Exception("Already playing");
                if(SceneManager.GetActiveScene().path!=ConRoiRouteV33.ScenePath)EditorSceneManager.OpenScene(ConRoiRouteV33.ScenePath,OpenSceneMode.Single);
                ConRoiRouteV33.Inspect();
                SessionState.SetBool("RegionalV32RunRequested",true);
                SessionState.SetBool("RegionalV33ViewsOnly",action=="views_v33");
                EditorApplication.playModeStateChanged-=RegionalPlayState;EditorApplication.playModeStateChanged+=RegionalPlayState;
                var view=EditorWindow.GetWindow(Type.GetType("UnityEditor.GameView,UnityEditor"));view.Show();view.Focus();
                EditorApplication.isPaused=false;EditorApplication.isPlaying=true;
            }
            else if (action == "assemble_v32") ConRoiRouteV32.Assemble();
            else if (action == "v32_static_flags") ConRoiRouteV32.FinalizeStaticFlags();
            else if (action == "run_v32")
            {
                if(EditorApplication.isPlayingOrWillChangePlaymode)throw new Exception("Already playing");
                if(SceneManager.GetActiveScene().path!=ConRoiRouteV32.ScenePath)EditorSceneManager.OpenScene(ConRoiRouteV32.ScenePath,OpenSceneMode.Single);
                ConRoiRouteV32.Inspect();
                SessionState.SetBool("RegionalV32RunRequested",true);
                EditorApplication.playModeStateChanged-=RegionalPlayState;EditorApplication.playModeStateChanged+=RegionalPlayState;
                var view=EditorWindow.GetWindow(Type.GetType("UnityEditor.GameView,UnityEditor"));view.Show();view.Focus();
                EditorApplication.isPaused=false;EditorApplication.isPlaying=true;
            }
            else if (action == "play_shore_view")
            {
                if(EditorApplication.isPlayingOrWillChangePlaymode)throw new Exception("Stop Play first");
                var fixture=UnityEngine.Object.FindAnyObjectByType<C3RouteFixture>();
                if(SceneManager.GetActiveScene().path!=ScenePath||!fixture||!fixture.useLandscapeCases)throw new Exception("Prepare shore first");
                var view=EditorWindow.GetWindow(Type.GetType("UnityEditor.GameView,UnityEditor"));view.Show();view.Focus();
                EditorApplication.isPaused=false;EditorApplication.isPlaying=true;
            }
            else if (action == "capture_shore")
            {
                if(!Application.isPlaying||SceneManager.GetActiveScene().path!=ScenePath)throw new Exception("Shore Play Mode required");
                var fixture=UnityEngine.Object.FindAnyObjectByType<C3RouteFixture>();
                var state=State();var camera=fixture.player.GetComponentInChildren<Camera>();
                if(!captureBackground.HasValue)captureBackground=Application.runInBackground;
                Application.runInBackground=true;
                state["camera_position"]=camera.transform.position.ToString("F6");
                state["camera_is_player_child"]=camera.transform.IsChildOf(fixture.player.transform);
                state["camera_forward"]=camera.transform.forward.ToString("F6");
                state["input_hook"]=fixture.player.TestInputHookActive;
                state["context_instances"]=fixture.GetComponentsInChildren<Transform>(true).Count(t=>t.name=="LakeGateMarket_PRESERVED_IMPORTED_CONTEXT");
                state["shore_instances"]=fixture.GetComponentsInChildren<Transform>(true).Count(t=>t.name=="LakeShoreV31_ACTUAL_IMPORTED_PREFAB");
                string path="Docs/Reviews/lake_gate_market_current/shore_palette_GameView.png";
                state["screenshot"]=path;Write("shore_visual_state.json",state);
                ScreenCapture.CaptureScreenshot(Path.GetFullPath(path));
            }
            else if (action == "open_route")
            {
                if(EditorApplication.isPlayingOrWillChangePlaymode)throw new Exception("Stop Play first");
                EditorSceneManager.OpenScene(ScenePath,OpenSceneMode.Single);
            }
            else if (action == "prepare_shore")
            {
                if(EditorApplication.isPlayingOrWillChangePlaymode)throw new Exception("Stop Play first");
                if(SceneManager.GetActiveScene().path!=ScenePath)EditorSceneManager.OpenScene(ScenePath,OpenSceneMode.Single);
                var f=UnityEngine.Object.FindAnyObjectByType<C3RouteFixture>();
                f.landscape=AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/Art/Environment/LakeShoreV31/landscape.json");
                f.landscapePrefab=AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Art/Environment/LakeShoreV31/LakeShoreV31.prefab");
                f.regionalContextPrefab=AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Art/Environment/LakeGateMarket/LakeGateMarketContext.prefab");
                if(!f.landscapePrefab)throw new Exception("Import and validate shore prefab first");
                f.useLandscapeCases=true;f.manualWalkFromW1=false;f.Build();Inspect();
            }
            else if (action == "assemble_game") ConRoiSceneAssembly.Assemble();
            else if (action == "build_game") ConRoiSceneAssembly.BuildWindows();
            else if (action == "play_game")
            {
                if(SceneManager.GetActiveScene().path!="Assets/ConRoiGame/ConRoi.unity")throw new Exception("Wrong game scene");
                EditorApplication.isPlaying=true;
            }
            else if (action == "asset_inventory")
            {
                var report=new JArray();
                foreach(var path in new[]{"Assets/Art/Models/Model.blend","Assets/Art/Models/Main_character.blend","Assets/Art/Models/main_character/texture_fbx/texture.fbx"})
                {
                    var model=AssetDatabase.LoadAssetAtPath<GameObject>(path);
                    report.Add(new JObject{["path"]=path,["loaded"]=model!=null,["objects"]=model?new JArray(model.GetComponentsInChildren<MeshFilter>(true).Select(m=>new JObject{["name"]=m.name,["position"]=m.transform.position.ToString(),["bounds"]=m.sharedMesh.bounds.size.ToString()})):new JArray()});
                }
                Write("asset_inventory.json",new JObject{["assets"]=report});
            }
            else if (action == "input_probe")
            {
                if (!Application.isPlaying) throw new Exception("Play Mode required");
                EditorWindow.GetWindow(Type.GetType("UnityEditor.GameView,UnityEditor")).Focus();
                var walk = UnityEngine.Object.FindAnyObjectByType<C3ManualWalk>();
                walk.StartCoroutine(walk.ProbeInputSystem());
            }
            else if (action == "runtime_inspect")
            {
                var p = UnityEngine.Object.FindAnyObjectByType<DieuKhienDiChuyen>(FindObjectsInactive.Include);
                var s = State(); s["time_scale"] = Time.timeScale;
                s["focus"] = EditorWindow.focusedWindow ? EditorWindow.focusedWindow.GetType().FullName : "none";
                if (p) {
                    s["player_active"] = p.gameObject.activeInHierarchy; s["movement_enabled"] = p.enabled;
                    s["controller_enabled"] = p.controller.enabled; s["hook"] = p.TestInputHookActive;
                    s["position"] = p.transform.position.ToString("F6");
                    s["input"] = p.ReadMovementInput().ToString("F6");
                    s["actions_enabled"] = p.GameplayActionsEnabled;
                    s["movement_frames"] = p.MovementFrames;
                    s["requested"] = p.RequestedDisplacement.ToString("F6");
                    s["actual"] = p.ActualDisplacement.ToString("F6");
                    s["nearby_colliders"]=new JArray(Physics.OverlapCapsule(p.transform.position-Vector3.up*.6f,p.transform.position+Vector3.up*.6f,.31f,~0,QueryTriggerInteraction.Ignore).Select(col=>new JObject{["name"]=col.name,["type"]=col.GetType().Name,["bounds"]=col.bounds.ToString()}));
                    s["grounded"] = p.controller.isGrounded;
                    s["camera"] = p.GetComponentInChildren<Camera>() != null;
                }
                Write("runtime_state.json", s);
            }
            else if (action == "play_manual")
            {
                PrepareManual();
                var view = EditorWindow.GetWindow(Type.GetType("UnityEditor.GameView,UnityEditor")); view.Show(); view.Focus();
                EditorApplication.isPaused = false; EditorApplication.isPlaying = true;
            }
            else if (action == "prepare_auto")
            {
                if (EditorApplication.isPlayingOrWillChangePlaymode) throw new Exception("Stop Play before preparation");
                var f = UnityEngine.Object.FindAnyObjectByType<C3RouteFixture>();
                f.manualWalkFromW1 = false; f.Build(); Inspect();
            }
            else if (action == "prepare_manual") PrepareManual();
            else if (action == "run" || action == "run_route")
            {
                if (EditorApplication.isPlayingOrWillChangePlaymode) throw new Exception("Already playing; refusing a second run");
                Inspect();
                if (UnityEngine.Object.FindAnyObjectByType<C3RouteFixture>().manualWalkFromW1) throw new Exception("Automatic run refused: scene is in manual mode");
                var pre = JObject.Parse(File.ReadAllText(Folder + "/preflight.json"));
                if (!(bool)pre["ready"]) throw new Exception("Preflight is not ready");
                SessionState.SetBool("C3LiveRunRequested", true);
                SessionState.SetBool("C3RouteOnly", action == "run_route");
                EditorApplication.playModeStateChanged -= PlayState;
                EditorApplication.playModeStateChanged += PlayState;
                var view = EditorWindow.GetWindow(Type.GetType("UnityEditor.GameView,UnityEditor")); view.Show(); view.Focus();
                EditorApplication.isPlaying = true;
            }
            else if (action == "stop") { if(captureBackground.HasValue){Application.runInBackground=captureBackground.Value;captureBackground=null;} EditorApplication.isPlaying = false; }
            else throw new Exception("Unknown command");
            var ack = State(); ack["action"] = action; ack["nonce"] = nonce; ack["status"] = "EXECUTED"; Write("ack.json", ack);
        }
        catch (Exception e) { var state = State(); state["status"] = "ERROR"; state["error"] = e.ToString(); state["nonce"] = nonce; Write("ack.json", state); }
    }
    [MenuItem("C3 v30/Prepare manual W1 spawn")]
    public static void PrepareManual()
    {
        if(EditorApplication.isPlayingOrWillChangePlaymode)throw new Exception("Stop Play before manual preparation");
        if(SceneManager.GetActiveScene().path!=ScenePath)throw new Exception("Wrong scene path for manual preparation");
        var fixture=UnityEngine.Object.FindAnyObjectByType<C3RouteFixture>();
        // TextAsset reimport does not necessarily invoke ExecuteAlways.OnEnable: rebuild cached support copies.
        fixture.useLandscapeCases=false;
        fixture.Build();
        Inspect();var pre=JObject.Parse(File.ReadAllText(Folder+"/preflight.json"));
        if(!(bool)pre["ready"])throw new Exception("Preflight failed");
        SessionState.SetBool("C3LiveRunRequested",false);
        fixture.manualWalkFromW1=true;fixture.PrepareManualSpawn();EditorUtility.SetDirty(fixture);EditorUtility.SetDirty(fixture.player.transform);
        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        if(!EditorSceneManager.SaveScene(SceneManager.GetActiveScene(),ScenePath))throw new Exception("Test scene save failed");
        var state=State();state["status"]="MANUAL_PREPARED_WAITING_FOR_HUMAN_INPUT";state["spawn_s_d"]=new JArray(-7,3.4);
        var t=fixture.player.transform;state["spawn_Unity_xyz"]=new JArray(t.position.x,t.position.y,t.position.z);state["heading_Unity_xyz"]=new JArray(t.forward.x,t.forward.y,t.forward.z);
        state["automatic_input_enabled"]=fixture.player.enableAutomatedTestInput;state["automatic_runner_present"]=fixture.GetComponent<C3ActualPlayRun>()!=null;
        state["manual_results"]="NOT_RUN";state["scene_saved_only_test_scene"]=true;
        Directory.CreateDirectory("Docs/Reviews/W_route_Unity_v30_20261016/manual");
        File.WriteAllText("Docs/Reviews/W_route_Unity_v30_20261016/manual/preparation.json",state.ToString());
        string pending="Docs/Reviews/W_route_Unity_v30_20261016/manual/pending_preparation.json";
        if(File.Exists(pending))File.Delete(pending);
        var view=EditorWindow.GetWindow(Type.GetType("UnityEditor.GameView,UnityEditor"));view.Show();
        Debug.Log("C3 MANUAL READY at W1; press Play yourself, click Game, F9 to arm and WASD to operate. No automatic Play launch.");
    }
    [InitializeOnLoadMethod]
    static void RestorePlayCallback()
    {
        EditorApplication.playModeStateChanged -= PlayState; EditorApplication.playModeStateChanged += PlayState;
        EditorApplication.playModeStateChanged -= RegionalPlayState; EditorApplication.playModeStateChanged += RegionalPlayState;
        EditorApplication.playModeStateChanged -= StoryPlayState; EditorApplication.playModeStateChanged += StoryPlayState;
        EditorApplication.delayCall+=()=>
        {
            string pending="Docs/Reviews/W_route_Unity_v30_20261016/manual/pending_preparation.json";
            if(!File.Exists(pending)||EditorApplication.isPlayingOrWillChangePlaymode)return;
            try{PrepareManual();}
            catch(Exception e){File.WriteAllText("Docs/Reviews/W_route_Unity_v30_20261016/manual/preparation_error.json",new JObject{["status"]="MANUAL_PREPARATION_NOT_COMPLETE",["error"]=e.ToString()}.ToString());Debug.LogException(e);}
        };
    }
    static void RegionalPlayState(PlayModeStateChange state)
    {
        if(state!=PlayModeStateChange.EnteredPlayMode||!SessionState.GetBool("RegionalV32RunRequested",false))return;
        SessionState.SetBool("RegionalV32RunRequested",false);
        try
        {
            if(SceneManager.GetActiveScene().path==ConRoiRouteV33.ScenePath)ConRoiRouteV33.Inspect();else if(SceneManager.GetActiveScene().path!=ConRoiStabilizationV36.ScenePath&&SceneManager.GetActiveScene().path!=ConRoiLakeMarketV34.ScenePath&&SceneManager.GetActiveScene().path!=ConRoiOngDaoV35.ScenePath)ConRoiRouteV32.Inspect();
            if(SessionState.GetBool("RegionalV34Route",false))
            {
                SessionState.SetBool("RegionalV34Route",false);var story=UnityEngine.Object.FindAnyObjectByType<DieuPhoiHanhTrinh>();story.SavePathOverride=Path.GetFullPath("Docs/Reviews/lake_gate_market_current/v34_route_fixture_save.json");story.Sleep();story.market.SetActive(true);story.enabled=false;
            }
            var player=UnityEngine.Object.FindAnyObjectByType<DieuKhienDiChuyen>();
            var runner=player.gameObject.AddComponent<C3ActualPlayRun>();
            runner.regionalRoute=AssetDatabase.LoadAssetAtPath<TextAsset>(ConRoiRouteV32.Folder+"/regional_route.json");
            runner.regionalViewsOnly=SessionState.GetBool("RegionalV33ViewsOnly",false);SessionState.SetBool("RegionalV33ViewsOnly",false);
            if(!runner.regionalRoute)throw new Exception("Missing packaged regional route");
        }
        catch(Exception e){Write("v32_launch_error.json",new JObject{["error"]=e.ToString()});EditorApplication.isPlaying=false;}
    }
    static void PlayState(PlayModeStateChange state)
    {
        if (state != PlayModeStateChange.EnteredPlayMode || !SessionState.GetBool("C3LiveRunRequested", false)) return;
        SessionState.SetBool("C3LiveRunRequested", false);
        if (SceneManager.GetActiveScene().path != ScenePath) { EditorApplication.isPlaying = false; return; }
        var fixture = UnityEngine.Object.FindFirstObjectByType<C3RouteFixture>();
        if (!fixture) { Write("launch_error.json", new JObject { ["error"] = "Missing fixture after domain reload" }); EditorApplication.isPlaying = false; return; }
        try
        {
            Inspect();
            if (!(bool)JObject.Parse(File.ReadAllText(Folder + "/preflight.json"))["ready"]) throw new Exception("Play-mode preflight is not ready");
            fixture.gameObject.AddComponent<C3ActualPlayRun>().routeOnly = SessionState.GetBool("C3RouteOnly",false);
        }
        catch(Exception e) { Write("launch_error.json",new JObject{["error"]=e.ToString()});EditorApplication.isPlaying=false; }
    }
    [MenuItem("C3 v30/Inspect exact scene")]
    public static void Inspect()
    {
        var state = State();
        if (SceneManager.GetActiveScene().path != ScenePath) throw new Exception("Wrong active scene path: " + SceneManager.GetActiveScene().path);
        if (SceneManager.sceneCount != 1) throw new Exception("Other scene loaded; refusing to mix test/game physics");
        var scene = SceneManager.GetActiveScene();
        var roots = scene.GetRootGameObjects();
        var fixture = roots.SelectMany(g => g.GetComponentsInChildren<C3RouteFixture>(true)).Single();
        fixture.ValidateProfile();
        if (!fixture.fixture || !fixture.player || !fixture.player.controller || !fixture.player.groundCheck) throw new Exception("Missing reference");
        var all = roots.SelectMany(g => g.GetComponentsInChildren<Transform>(true)).ToArray();
        int missing = all.Sum(t => GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(t.gameObject));
        var colliders = all.SelectMany(t => t.GetComponents<MeshCollider>()).Where(c => c.enabled && c.gameObject.activeInHierarchy).ToArray();
        int replacements = colliders.Count(c => c.name == "SUP_C3_REPLACEMENT_v30");
        var json = JObject.Parse(fixture.fixture.text); var fr = json["frame"];
        Vector3 P(JToken x) => new Vector3((float)x[0], (float)x[1], (float)x[2]);
        var F=P(fr["F"]); var u=P(fr["u"]); var n=P(fr["n"]); float a=(float)json["edge_a_b"][0], b=(float)json["edge_a_b"][1];
        var samples = new JArray(); int missingSupport = 0, oldHits = 0;
        Physics.SyncTransforms();
        foreach(float off in new[]{-.8f,-.4f,0,.4f,.8f}) foreach(float k in new[]{-3.4f,-2,-1.2f,-.6f,-.01f,0,.01f,.6f,2,3.4f})
        {
            float s=23.4f+off; var q=F+u*s+n*(a*s+b+k); var origin=new Vector3(-q.x,1000,-q.y);
            var hits=Physics.RaycastAll(origin,Vector3.down,2000,8,QueryTriggerInteraction.Ignore).Where(h => h.collider.name.StartsWith("SUP_")).ToArray();
            if(hits.Length==0) missingSupport++; oldHits+=hits.Count(h=>h.collider.name!="SUP_C3_REPLACEMENT_v30");
            samples.Add(new JObject{["offset_m"]=off,["k_m"]=k,["hits"]=new JArray(hits.Select(h=>new JObject{["collider"]=h.collider.name,["height_m"]=h.point.y}))});
        }
        var cc=fixture.player.controller; var player=fixture.player;
        state["ready"]=missing==0&&replacements==1&&missingSupport==0&&oldHits==0;
        state["missing_scripts"]=missing; state["active_mesh_colliders"]=colliders.Length; state["C3_replacement_count"]=replacements;
        state["C3_missing_support_samples"]=missingSupport; state["C3_old_support_hits"]=oldHits;state["C3_samples"]=samples;
        state["player"]=new JObject{["height"]=cc.height,["radius"]=cc.radius,["step"]=cc.stepOffset,["slope"]=cc.slopeLimit,["skin"]=cc.skinWidth,["min_move"]=cc.minMoveDistance,["speed"]=player.speed,["gravity"]=player.gravity,["jump"]=player.jumpHeight,["ground_distance"]=player.groundDistance,["ground_mask"]=player.groundMask.value,["scale"]=cc.transform.lossyScale.ToString("F6")};
        var imports=new JArray();
        foreach(var role in new[]{"VISUAL","COLLIDERS"})
        {
            string path="Assets/WRouteC3Test_v30_20261016/Models/W1_P32_W2_C3_"+role+"_v30.fbx";
            var importer=AssetImporter.GetAtPath(path) as ModelImporter;var asset=AssetDatabase.LoadAssetAtPath<GameObject>(path);
            var info=new JObject{["path"]=path,["asset_loaded"]=asset!=null,["global_scale"]=importer ? importer.globalScale : -1,["file_scale"]=importer ? importer.fileScale : -1,["convert_units"]=importer && importer.useFileScale,["automatic_colliders"]=importer && importer.addCollider};
            if(asset)
            {
                var mf=asset.GetComponentsInChildren<MeshFilter>(true).Single(f=>f.name=="SUP_C3_REPLACEMENT_v30");
                var expected=json["meshes"].Single(m=>(string)m["role"]=="C3_replacement")["vertices"].Select(P).ToArray();
                var actual=mf.sharedMesh.vertices.Select(v=>mf.transform.TransformPoint(v)).ToArray();
                Bounds eb=new Bounds(expected[0],Vector3.zero), ab=new Bounds(actual[0],Vector3.zero);
                foreach(var v in expected)eb.Encapsulate(v);foreach(var v in actual)ab.Encapsulate(v);
                float error=Mathf.Max((eb.min-ab.min).magnitude,(eb.max-ab.max).magnitude);
                info["C3_world_bounds_error_m"]=error;info["C3_import_size_m"]=new JArray(ab.size.x,ab.size.y,ab.size.z);
                if(error>.002f)state["ready"]=false;
            }
            imports.Add(info);
            if(!asset||!importer||importer.globalScale!=1||importer.addCollider) state["ready"]=false;
        }
        state["imports"]=imports;state["fixture_json_path"]=AssetDatabase.GetAssetPath(fixture.fixture);state["references_valid"]=true;
        Write("preflight.json",state); Debug.Log("C3 LIVE PREFLIGHT scene="+scene.path+" ready="+state["ready"]);
    }
}
