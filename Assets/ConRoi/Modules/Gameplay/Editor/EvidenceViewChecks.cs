using System;
using System.IO;
using System.Collections;
using System.Reflection;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEditor;
using UnityEditor.SceneManagement;
using ConRoi.Investigation.V36.R107;

namespace ConRoi.Gameplay.EditorChecks
{
    [InitializeOnLoad]
    public static class EvidenceViewChecks
    {
        const string Key = "ConRoi.EvidenceViewChecks";
        const string Scene = "Assets/ConRoi/Modules/Gameplay/Scenes/Gameplay_ThuNghiem.unity";
        static InvestigationSession s; static GameObject root; static Mouse mouse;
        static IEnumerator run; static JArray rows; static int failures, frame = -1; static string output;
        static EvidenceViewChecks() { EditorApplication.update += Tick; }
        static string Arg(string k) { var a = Environment.GetCommandLineArgs(); int i = Array.IndexOf(a, k); return i >= 0 && i + 1 < a.Length ? a[i + 1] : ""; }
        public static void RunBatch()
        {
            output = Arg("--view-output"); string profile = Arg("--investigation-save-dir");
            if (!Application.isBatchMode || Application.unityVersion != "6000.6.2f1" || EditorApplication.isPlayingOrWillChangePlaymode ||
                !Path.IsPathRooted(output) || Directory.Exists(output) || !Path.IsPathRooted(profile) || Directory.Exists(profile))
                throw new InvalidOperationException("Explicit Unity version and new private output/profile required");
            Directory.CreateDirectory(output); EditorSceneManager.OpenScene(Scene, OpenSceneMode.Single);
            SessionState.SetString(Key, "pending"); EditorApplication.EnterPlaymode();
        }
        static void Check(string name, bool pass)
        {
            rows.Add(new JObject { ["name"] = name, ["pass"] = pass });
            if (!pass) { failures++; throw new Exception("Precondition/assertion failed: " + name); }
        }
        static bool Pose(EvidenceViewPose p, float yaw, float pitch, float distance) => Mathf.Abs(p.Yaw - yaw) < .001f && p.Pitch == pitch && p.Distance == distance;
        static GameObject Source(string id)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cube); go.name = "View fixture " + id; go.transform.SetParent(root.transform);
            go.transform.position = new Vector3(5, 2, 5); go.AddComponent<InvestigationInteractable>().evidenceId = id; return go;
        }
        static IEnumerator Inspection()
        {
            s.data.introAccepted = true; s.ClosePage(); string error;
            Check("world_reset_rejected", !s.TryResetInspection(out error) && error.Length > 0 && s.InspectionView.Availability == InspectionAvailability.WrongContext);
            int notifications = 0; Action<InspectionViewState> listener = state => notifications++;
            s.InspectionViewChanged += listener;
            var source = Source("shoe"); Vector3 position = source.transform.position, scale = source.transform.localScale;
            s.Acquire("shoe"); s.OpenEvidence("shoe", source.transform);
            Check("model_ready_and_texture", s.InspectionView.CanControl && s.InspectionView.Preview != null);
            Check("shoe_defaults", Pose(s.InspectionView.Pose, 25, 65, 2));
            string data = JsonUtility.ToJson(s.data); int before = notifications;
            Check("adjust_and_clamp", s.TryAdjustInspection(40, 200, -40, out error) && Pose(s.InspectionView.Pose, 65, 80, .5f));
            Check("event_once_for_command", notifications == before + 1);
            Check("no_evidence_or_progress_grant", JsonUtility.ToJson(s.data) == data);
            Check("zoom_out_clamp", s.TryAdjustInspection(0, -400, 400, out error) && s.InspectionView.Pose.Pitch == -80 && s.InspectionView.Pose.Distance == 4);
            var pose = s.InspectionView.Pose; before = notifications;
            Check("nan_rejected", !s.TryAdjustInspection(float.NaN, 0, 0, out error) && error.Length > 0);
            Check("infinity_rejected", !s.TryAdjustInspection(0, float.PositiveInfinity, 0, out error));
            Check("nonfinite_no_mutation_or_event", Pose(s.InspectionView.Pose, pose.Yaw, pose.Pitch, pose.Distance) && notifications == before && JsonUtility.ToJson(s.data) == data);
            Check("large_finite_safe", s.TryAdjustInspection(float.MaxValue, float.MaxValue, float.MaxValue, out error) && !float.IsNaN(s.InspectionView.Pose.Yaw) && s.InspectionView.Pose.Distance == 4);
            Check("reset_default", s.TryResetInspection(out error) && Pose(s.InspectionView.Pose, 25, 65, 2));
            before = notifications; Check("repeat_reset_idempotent", s.TryResetInspection(out error) && notifications == before);
            s.SetPaused(true); pose = s.InspectionView.Pose; before = notifications;
            Check("paused_adjust_and_reset_rejected", !s.TryAdjustInspection(10, 10, 1, out error) && !s.TryResetInspection(out error));
            Check("paused_state_and_pose", s.InspectionView.Availability == InspectionAvailability.Paused && Pose(s.InspectionView.Pose, pose.Yaw, pose.Pitch, pose.Distance) && notifications == before);
            s.SetPaused(false);
            Action<InspectionViewState> bad = state => { throw new InvalidOperationException("fixture listener"); };
            s.InspectionViewChanged += bad; before = notifications;
            Check("subscriber_exception_isolated", s.TryAdjustInspection(10, 0, 0, out error) && notifications == before + 1);
            s.InspectionViewChanged -= bad;
            s.InspectionViewChanged -= listener; before = notifications; s.TryResetInspection(out error);
            Check("unsubscribe_works", notifications == before);
            Check("source_not_transformed", source.transform.position == position && source.transform.localScale == scale && source.GetComponent<Collider>().enabled);
            s.Acquire("phone"); s.OpenEvidence("phone");
            Check("missing_visual_no_stale_preview", s.InspectionView.Availability == InspectionAvailability.MissingVisual && s.InspectionView.Preview == null && !s.TryResetInspection(out error));
            s.OpenEvidence("shoe", source.transform); s.TryAdjustInspection(60, -20, 1, out error);
            s.Save(); Check("private_save_committed", s.Message.StartsWith("Đã lưu"));
            Check("load_reopens_default_not_saved_pose", s.Load() && Pose(s.InspectionView.Pose, 25, 65, 2) && s.InspectionView.CanControl);
            Check("load_keeps_evidence_no_auto_observation", s.Has("shoe") && s.Record("shoe").observations.Count == 0 && s.data.trials.Count == 0 && !s.data.finalized);
            mouse = InputSystem.AddDevice<Mouse>("PrivateEvidenceViewMouse"); mouse.MakeCurrent(); pose = s.InspectionView.Pose;
            InputSystem.QueueStateEvent(mouse, new MouseState { buttons = 1, delta = new Vector2(24, 0) }); InputSystem.Update(); mouse.MakeCurrent();
            Check("virtual_mouse_fixture_delivered", mouse.leftButton.isPressed && mouse.delta.ReadValue().x != 0);
            typeof(InvestigationSession).GetMethod("Update", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(s, null);
            Check("virtual_mouse_uses_runtime_command", s.InspectionView.Pose.Yaw != pose.Yaw);
            s.SetPaused(true); pose = s.InspectionView.Pose;
            InputSystem.QueueStateEvent(mouse, new MouseState { buttons = 1, delta = new Vector2(24, 12), scroll = new Vector2(0, 120) }); InputSystem.Update(); mouse.MakeCurrent();
            typeof(InvestigationSession).GetMethod("Update", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(s, null);
            Check("virtual_mouse_during_pause_no_change", Pose(s.InspectionView.Pose, pose.Yaw, pose.Pitch, pose.Distance));
            InputSystem.QueueStateEvent(mouse, new MouseState()); s.SetPaused(false); s.ClosePage();
            Check("close_hides_texture_and_restores_world_input", s.InspectionView.Preview == null && !s.InspectionView.CanControl && s.movement.enabled && s.look.enabled);
            s.OpenPage("dossier"); Check("other_modal_rejects_view_commands", !s.TryAdjustInspection(1, 1, 1, out error));
            yield return null;
        }
        static IEnumerator Comparison()
        {
            s.data.introAccepted = true; s.ClosePage(); string error;
            var left = ComparisonSide.Left; var right = ComparisonSide.Right;
            Check("wrong_context_no_selection", !s.TrySelectComparison(left, "shoe", out error) && error.Length > 0 && s.GetComparisonView(left).Availability == ComparisonAvailability.WrongContext);
            s.OpenPage("compare");
            Check("empty_and_modal_input_locked", s.GetComparisonView(left).Availability == ComparisonAvailability.Empty && !s.movement.enabled && !s.look.enabled);
            Check("unacquired_unknown_empty_rejected", !s.TrySelectComparison(left, "shoe", out error) && !s.TrySelectComparison(left, "unknown", out error) && !s.TrySelectComparison(left, null, out error));
            Check("invalid_side_rejected", !s.TrySelectComparison((ComparisonSide)2, "shoe", out error) && !s.TryClearComparison((ComparisonSide)2, out error));
            Check("empty_controls_rejected", !s.TryResetComparison(left, out error) && !s.TryAdjustComparison(left, 1, 1, 1, out error));
            var a = Source("shoe"); var b = Source("shoecontrol");
            a.AddComponent<Rigidbody>().isKinematic = true;
            var position = a.transform.position; var scale = a.transform.localScale; var material = a.GetComponent<Renderer>().sharedMaterial;
            s.Acquire("shoe"); s.Acquire("shoecontrol");
            int events = 0; Action<ComparisonSide, ComparisonViewState> listener = (side,state) => events++;
            s.ComparisonViewChanged += listener;
            Check("select_left_ready", s.TrySelectComparison(left, "shoe", a.transform, out error) && s.GetComparisonView(left).CanControl);
            Check("select_right_auto_source_ready", s.TrySelectComparison(right, "shoecontrol", out error) && s.GetComparisonView(right).CanControl);
            var l = s.GetComparisonView(left); var r = s.GetComparisonView(right);
            Check("independent_created_textures", l.Preview != r.Preview && ((RenderTexture)l.Preview).IsCreated() && ((RenderTexture)r.Preview).IsCreated());
            Check("useful_shoe_defaults", Pose(l.Pose, 25, 65, 2) && Pose(r.Pose, 25, 65, 2));
            var camera = UnityEngine.Object.FindObjectsByType<Camera>(FindObjectsInactive.Include, FindObjectsSortMode.None).First(c => c.targetTexture == l.Preview);
            var visual = camera.transform.parent.gameObject;
            Check("camera_cannot_see_adjacent_preview", camera.farClipPlane < 20 && camera.farClipPlane > 5);
            camera.Render();
            var pixels = new Texture2D(600,600,TextureFormat.RGB24,false);
            var active = RenderTexture.active; bool rendered = false;
            try
            {
                RenderTexture.active = (RenderTexture)l.Preview; pixels.ReadPixels(new Rect(0,0,600,600),0,0); pixels.Apply();
                Color background = pixels.GetPixel(0,0);
                foreach (var pixel in pixels.GetPixels()) if (Mathf.Abs(pixel.r-background.r)+Mathf.Abs(pixel.g-background.g)+Mathf.Abs(pixel.b-background.b) > .1f) { rendered = true; break; }
            }
            finally { RenderTexture.active = active; UnityEngine.Object.Destroy(pixels); }
            Check("synthetic_mesh_renders_pixels", rendered);
            Check("snapshot_has_no_scripts_or_physics", visual.GetComponentsInChildren<MonoBehaviour>(true).Length == 0 && visual.GetComponentsInChildren<Collider>(true).Length == 0 && visual.GetComponentsInChildren<Rigidbody>(true).Length == 0);
            string data = JsonUtility.ToJson(s.data); int before = events;
            Check("left_adjust_independent", s.TryAdjustComparison(left, 40, 200, -40, out error) && Pose(s.GetComparisonView(left).Pose, 65, 80, .5f) && Pose(s.GetComparisonView(right).Pose, 25, 65, 2));
            Check("command_event_and_no_progress_grant", events == before + 1 && JsonUtility.ToJson(s.data) == data);
            before = events; var pose = s.GetComparisonView(left).Pose;
            Check("nonfinite_no_mutation", !s.TryAdjustComparison(left, float.NaN, 0, 0, out error) && !s.TryAdjustComparison(left, 0, 0, float.PositiveInfinity, out error) && events == before && Pose(s.GetComparisonView(left).Pose, pose.Yaw, pose.Pitch, pose.Distance));
            Check("large_finite_and_zoom_clamps", s.TryAdjustComparison(left, float.MaxValue, -float.MaxValue, float.MaxValue, out error) && !float.IsNaN(s.GetComparisonView(left).Pose.Yaw) && s.GetComparisonView(left).Pose.Pitch == -80 && s.GetComparisonView(left).Pose.Distance == 4);
            Check("reset_only_left", s.TryResetComparison(left, out error) && Pose(s.GetComparisonView(left).Pose, 25, 65, 2) && s.GetComparisonView(right).Preview == r.Preview);
            before = events; Check("repeat_reset_no_event", s.TryResetComparison(left, out error) && events == before);
            s.TryAdjustComparison(left, 20, -10, .2f, out error); l = s.GetComparisonView(left); before = events;
            Check("same_selection_retains_pose_texture", s.TrySelectComparison(left, "shoe", out error) && s.GetComparisonView(left).Preview == l.Preview && Pose(s.GetComparisonView(left).Pose, l.Pose.Yaw, l.Pose.Pitch, l.Pose.Distance) && events == before);
            Check("bad_selection_keeps_previous", !s.TrySelectComparison(left, "phone", out error) && s.GetComparisonView(left).EvidenceId == "shoe" && s.GetComparisonView(left).Preview == l.Preview);
            s.SetPaused(true);
            Check("pause_keeps_selection_freezes_render", s.GetComparisonView(left).Availability == ComparisonAvailability.Paused && !visual.activeSelf && !s.TryAdjustComparison(left, 1, 1, 1, out error) && !s.TryResetComparison(left, out error) && !s.TrySelectComparison(right, "shoe", out error));
            s.SetPaused(false);
            Check("resume_preserves_pose", visual.activeSelf && Pose(s.GetComparisonView(left).Pose, l.Pose.Yaw, l.Pose.Pitch, l.Pose.Distance));
            Action<ComparisonSide, ComparisonViewState> bad = (side,state) => { throw new InvalidOperationException("fixture listener"); };
            s.ComparisonViewChanged += bad; before = events;
            Check("listener_exception_isolated", s.TryResetComparison(left, out error) && events == before + 1);
            s.ComparisonViewChanged -= bad; s.ComparisonViewChanged -= listener; before = events;
            s.TryAdjustComparison(left, 10, 0, 0, out error); Check("unsubscribe", events == before);
            Check("original_transform_material_physics_unchanged", a.transform.position == position && a.transform.localScale == scale && a.GetComponent<Renderer>().sharedMaterial == material && a.GetComponent<Collider>().enabled && a.GetComponent<Rigidbody>().isKinematic);
            s.Acquire("letter"); var old = (RenderTexture)s.GetComparisonView(left).Preview;
            Check("text_only_selection_valid_no_stale_preview", s.TrySelectComparison(left, "letter", out error) && s.GetComparisonView(left).Availability == ComparisonAvailability.MissingVisual && s.GetComparisonView(left).Preview == null && s.GetComparisonView(left).Transcript == s.EvidenceTranscript("letter") && s.GetComparisonView(left).Diagnostic.Length > 0 && !old.IsCreated());
            Check("text_only_rotate_reset_reason", !s.TryResetComparison(left, out error) && error.Length > 0 && !s.TryAdjustComparison(left, 1, 1, 1, out error));
            s.Acquire("clip"); s.data.clipBeforeSeen = false; s.data.clipEventSeen = false;
            s.TrySelectComparison(left, "clip", out error);
            Check("clip_transcript_guard", s.GetComparisonView(left).Transcript == s.EvidenceTranscript("clip") && s.GetComparisonView(left).Transcript != InvestigationContent.Get("clip").transcript);
            s.data.clipBeforeSeen = true; Check("one_exposure_still_guarded", s.GetComparisonView(left).Transcript != InvestigationContent.Get("clip").transcript);
            s.data.clipEventSeen = true; Check("both_fixture_flags_reveal_transcript", s.GetComparisonView(left).Transcript == InvestigationContent.Get("clip").transcript && s.Record("clip").observations.Count == 0);
            var lineSource = new GameObject("Line fixture"); lineSource.transform.SetParent(root.transform); lineSource.transform.position = new Vector3(20, 3, 8);
            var line = lineSource.AddComponent<LineRenderer>(); line.useWorldSpace = true; line.positionCount = 2; line.SetPositions(new[] { new Vector3(20,3,8),new Vector3(21,4,9) });
            var point = line.GetPosition(1);
            Check("line_snapshot_and_source_unchanged", s.TrySelectComparison(left, "shoe", lineSource.transform, out error) && s.GetComparisonView(left).CanControl && line.useWorldSpace && line.GetPosition(1) == point);
            s.TryClearComparison(left, out error); r = s.GetComparisonView(right);
            Check("clear_only_one_side", s.GetComparisonView(left).Availability == ComparisonAvailability.Empty && r.EvidenceId == "shoecontrol" && r.CanControl);
            Check("repeat_clear_idempotent", s.TryClearComparison(left, out error));
            s.TrySelectComparison(left, "shoe", a.transform, out error); data = JsonUtility.ToJson(s.data);
            s.Save(); Check("private_save_success", s.Message.StartsWith("Đã lưu"));
            byte[] valid = File.ReadAllBytes(s.SavePath); l = s.GetComparisonView(left); string stateBefore = JsonUtility.ToJson(s.data);
            byte[] rejected = System.Text.Encoding.UTF8.GetBytes("{broken");
            try
            {
                File.WriteAllBytes(s.SavePath, rejected);
                Check("rejected_load_preserves_state_views_bytes", !s.Load() && JsonUtility.ToJson(s.data) == stateBefore && s.GetComparisonView(left).Preview == l.Preview && File.ReadAllBytes(s.SavePath).SequenceEqual(rejected));
                Check("continue_after_rejection", s.TryAdjustComparison(left, 1, 2, .1f, out error));
            }
            finally { File.WriteAllBytes(s.SavePath, valid); }
            Check("valid_load_clears_transient_previews", s.Load() && s.Page == "compare" && s.GetComparisonView(left).Availability == ComparisonAvailability.Empty && s.GetComparisonView(right).Availability == ComparisonAvailability.Empty);
            Check("valid_load_retains_evidence_no_auto_relations", s.Has("shoe") && s.Has("shoecontrol") && s.data.relations.Count == 0 && s.Record("shoe").observations.Count == 0);
            s.TrySelectComparison(left, "shoe", out error); s.Save(); Check("save_load_again", s.Message.StartsWith("Đã lưu") && s.Load());
            s.TrySelectComparison(left, "shoe", out error); old = (RenderTexture)s.GetComparisonView(left).Preview;
            s.ClosePage(); Check("leave_releases_preview_restores_input", !old.IsCreated() && s.GetComparisonView(left).Preview == null && s.movement.enabled && s.look.enabled);
            s.OpenPage("compare"); Check("reopen_empty", s.GetComparisonView(left).Availability == ComparisonAvailability.Empty && s.GetComparisonView(right).Availability == ComparisonAvailability.Empty);
            s.TrySelectComparison(left, "shoe", out error); s.OpenPage("dossier");
            Check("other_modal_clears_and_rejects", s.GetComparisonView(left).Preview == null && !s.TryAdjustComparison(left, 1, 1, 1, out error) && !s.movement.enabled);
            yield return null;
        }
        static void Tick()
        {
            if (EditorApplication.isCompiling || EditorApplication.isUpdating) return;
            string phase = SessionState.GetString(Key, "");
            if (phase == "finished" && !EditorApplication.isPlayingOrWillChangePlaymode) { SessionState.SetString(Key, ""); EditorApplication.Exit(SessionState.GetInt(Key + ".exit", 2)); return; }
            if (!EditorApplication.isPlaying || Time.frameCount == frame) return; frame = Time.frameCount;
            try
            {
                if (phase == "pending") { output = Arg("--view-output"); rows = new JArray(); s = TrialPlaybackContextChecks.BuildEnvironment(out root); run = Arg("--view-suite") == "comparison" ? Comparison() : Inspection(); SessionState.SetString(Key, "running"); }
                if (SessionState.GetString(Key, "") == "running" && !run.MoveNext()) Finish();
            }
            catch (Exception e) { rows.Add(new JObject { ["exception"] = e.ToString(), ["pass"] = false }); failures++; Finish(); }
        }
        static void Finish()
        {
            (run as IDisposable)?.Dispose(); if (mouse != null && mouse.added) InputSystem.RemoveDevice(mouse);
            if (root) UnityEngine.Object.DestroyImmediate(root);
            File.WriteAllText(Path.Combine(output, "results.json"), new JObject { ["source"] = Arg("--view-source"), ["suite"] = Arg("--view-suite"), ["kind"] = Arg("--view-suite") == "comparison" ? "PLAYMODE_API_PROXY_NOT_UI_NOT_WINDOWS" : "API_FIXTURE_DIRECT_UPDATE_VIRTUAL_MOUSE_NOT_UI_NOT_WINDOWS",
                ["unity"] = Application.unityVersion, ["scene"] = Scene, ["failures"] = failures, ["checks"] = rows }.ToString());
            SessionState.SetInt(Key + ".exit", failures == 0 ? 0 : 1); SessionState.SetString(Key, "finished"); EditorApplication.ExitPlaymode();
        }
    }
}
