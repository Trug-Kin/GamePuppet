using System;
using System.IO;
using System.Collections;
using System.Reflection;
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
        static void Tick()
        {
            if (EditorApplication.isCompiling || EditorApplication.isUpdating) return;
            string phase = SessionState.GetString(Key, "");
            if (phase == "finished" && !EditorApplication.isPlayingOrWillChangePlaymode) { SessionState.SetString(Key, ""); EditorApplication.Exit(SessionState.GetInt(Key + ".exit", 2)); return; }
            if (!EditorApplication.isPlaying || Time.frameCount == frame) return; frame = Time.frameCount;
            try
            {
                if (phase == "pending") { output = Arg("--view-output"); rows = new JArray(); s = TrialPlaybackContextChecks.BuildEnvironment(out root); run = Inspection(); SessionState.SetString(Key, "running"); }
                if (SessionState.GetString(Key, "") == "running" && !run.MoveNext()) Finish();
            }
            catch (Exception e) { rows.Add(new JObject { ["exception"] = e.ToString(), ["pass"] = false }); failures++; Finish(); }
        }
        static void Finish()
        {
            (run as IDisposable)?.Dispose(); if (mouse != null && mouse.added) InputSystem.RemoveDevice(mouse);
            if (root) UnityEngine.Object.DestroyImmediate(root);
            File.WriteAllText(Path.Combine(output, "results.json"), new JObject { ["source"] = Arg("--view-source"), ["kind"] = "API_FIXTURE_DIRECT_UPDATE_VIRTUAL_MOUSE_NOT_UI_NOT_WINDOWS",
                ["unity"] = Application.unityVersion, ["scene"] = Scene, ["failures"] = failures, ["checks"] = rows }.ToString());
            SessionState.SetInt(Key + ".exit", failures == 0 ? 0 : 1); SessionState.SetString(Key, "finished"); EditorApplication.ExitPlaymode();
        }
    }
}
