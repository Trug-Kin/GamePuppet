using System;
using System.IO;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using ConRoi.Investigation.V36.R107;

namespace ConRoi.Gameplay.EditorChecks
{
    // Explicit batch/API fixture only. Never attached to the game or a saved scene.
    [InitializeOnLoad]
    public static class SaveLoadContractChecks
    {
        const string Active = "ConRoi.SaveContractChecks.Active";
        const string ExitCode = "ConRoi.SaveContractChecks.Exit";
        const string Output = "ConRoi.SaveContractChecks.Output";
        const string TestScene = "Assets/ConRoi/Modules/Gameplay/Scenes/Gameplay_ThuNghiem.unity";
        [Serializable] class Check { public string name; public bool pass; public string detail; }
        [Serializable] class Report
        {
            public string unity, scene, profile, kind = "PLAYMODE_API_FIXTURE_NOT_NATURAL_ROUTE";
            public int missingScripts, failures;
            public List<Check> checks = new List<Check>();
        }
        static SaveLoadContractChecks() { EditorApplication.update += Tick; }
        static string Arg(string key)
        {
            var args = Environment.GetCommandLineArgs();
            int at = Array.IndexOf(args, key);
            return at >= 0 && at + 1 < args.Length ? args[at + 1] : "";
        }
        public static void RunBatch()
        {
            if (Application.unityVersion != "6000.6.2f1" || !Application.isBatchMode || EditorApplication.isPlayingOrWillChangePlaymode)
                throw new InvalidOperationException("Run explicitly in Unity 6000.6.2f1 batch Editor, outside Play.");
            string output = Arg("--save-contract-output");
            string profile = Arg("--investigation-save-dir");
            if (!Path.IsPathRooted(output) || !Path.IsPathRooted(profile) || Directory.Exists(profile))
                throw new InvalidOperationException("Need an absolute output path and a new, unused absolute test profile.");
            Directory.CreateDirectory(output);
            EditorSceneManager.OpenScene(TestScene, OpenSceneMode.Single);
            SessionState.SetString(Output, output);
            SessionState.SetInt(ExitCode, 2);
            SessionState.SetString(Active + ".Phase", "pending");
            SessionState.SetBool(Active, true);
            EditorApplication.EnterPlaymode();
        }
        static void Tick()
        {
            if (!SessionState.GetBool(Active, false) || EditorApplication.isCompiling || EditorApplication.isUpdating) return;
            string phase = SessionState.GetString(Active + ".Phase", "");
            if (phase == "pending" && EditorApplication.isPlaying)
            {
                SessionState.SetString(Active + ".Phase", "executing");
                Execute();
            }
            else if (phase == "finished" && !EditorApplication.isPlayingOrWillChangePlaymode)
            {
                SessionState.SetBool(Active, false);
                EditorApplication.Exit(SessionState.GetInt(ExitCode, 2));
            }
        }
        static void Add(Report report, string name, bool pass, string detail = "")
        {
            report.checks.Add(new Check { name = name, pass = pass, detail = detail });
            if (!pass) report.failures++;
        }
        static void Execute()
        {
            var report = new Report { unity = Application.unityVersion, scene = SceneManager.GetActiveScene().path, profile = Arg("--investigation-save-dir") };
            GameObject root = null;
            try
            {
                Add(report, "correct_private_test_scene", report.scene == TestScene);
                foreach (var go in SceneManager.GetActiveScene().GetRootGameObjects())
                    foreach (var t in go.GetComponentsInChildren<Transform>(true))
                        report.missingScripts += GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(t.gameObject);
                Add(report, "test_scene_missing_scripts", report.missingScripts == 0, "Empty template; not an integration-scene reference audit.");
                root = new GameObject("SaveContract_API_Fixture");
                root.SetActive(false);
                var player = new GameObject("FixturePlayer"); player.transform.SetParent(root.transform);
                var cc = player.AddComponent<CharacterController>();
                var movement = player.AddComponent<DieuKhienDiChuyen>(); movement.controller = cc;
                var eyeObject = new GameObject("FixtureEye"); eyeObject.transform.SetParent(player.transform);
                var camera = eyeObject.AddComponent<Camera>();
                var look = eyeObject.AddComponent<DieuKhienGocNhin>();
                var gateObject = new GameObject("FixtureGate"); gateObject.transform.SetParent(root.transform);
                var gate = gateObject.AddComponent<ReturnGateMechanism>(); gate.checkPlayerContact = false; gate.enabled = false;
                var modelObject = new GameObject("FixtureModel"); modelObject.transform.SetParent(root.transform);
                var model = modelObject.AddComponent<ReturnGateMechanism>(); model.checkPlayerContact = false; model.enabled = false;
                var session = root.AddComponent<InvestigationSession>();
                session.movement = movement; session.look = look; session.eye = camera; session.gate = gate; session.modelGate = model;
                root.SetActive(true);
                session.enabled = false; movement.enabled = false; look.enabled = false;
                Add(report, "cli_private_profile_applied", session.SaveDirectoryOverride == report.profile);
                Add(report, "initial_empty_state", session.data.evidence.Count == 0 && session.data.mapping.Count == 0 && session.data.relations.Count == 0 && session.data.trials.Count == 0 && !File.Exists(session.SavePath));
                session.ClosePage();
                session.Acquire("slide"); session.Observe("slide", 0);
                session.Acquire("shoeprint"); session.Observe("shoeprint", 0);
                session.Assign("A", "B"); session.AddRelation("slide", "shoeprint", "Fixture relation; not a factual conclusion.");
                player.transform.position = new Vector3(2, 1, 3);
                session.Save();
                Add(report, "save_created_in_private_profile", File.Exists(session.SavePath) && session.SavePath.StartsWith(report.profile + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase));
                byte[] validBytes = File.ReadAllBytes(session.SavePath);
                string validJson = File.ReadAllText(session.SavePath);
                session.data.evidence.Clear(); session.data.mapping.Clear(); player.transform.position = Vector3.zero;
                bool loaded = session.Load();
                Add(report, "save_load_roundtrip", loaded && session.Observation("slide", 0) && !session.Observation("slide", 1) && session.ReadCipher().Contains("_") && session.data.mapping.Count == 1 && session.data.relations.Count == 1 && Vector3.Distance(player.transform.position, new Vector3(2, 1, 3)) < .001f);
                session.Observe("slide", 1); session.Save();
                Add(report, "continue_observe_save_load", session.Load() && session.Observation("slide", 1));
                session.Observe("slide", 999);
                Add(report, "live_api_rejects_invalid_observation", !session.Record("slide").observations.Contains(999));
                session.SetPaused(true); session.Save();
                Add(report, "load_resumes_safe_world", session.Load() && !session.Paused && !session.Modal && movement.enabled && look.enabled && Time.timeScale == 1);

                // API-created out-of-range fixture, independent of JSON whitespace.
                var invalid = JsonUtility.FromJson<InvestigationSave>(validJson);
                invalid.evidence[0].observations.Add(999);
                Reject(report, session, validBytes, "out_of_range_observation", JsonUtility.ToJson(invalid));
                invalid = JsonUtility.FromJson<InvestigationSave>(validJson); invalid.evidence.Add(new EvidenceRecord { id = "slide", collected = true });
                Reject(report, session, validBytes, "duplicate_evidence_record", JsonUtility.ToJson(invalid));
                invalid = JsonUtility.FromJson<InvestigationSave>(validJson); invalid.evidence[0].id = "not_a_game_evidence";
                Reject(report, session, validBytes, "unknown_evidence_id", JsonUtility.ToJson(invalid));
                invalid = JsonUtility.FromJson<InvestigationSave>(validJson); invalid.namespaceId = "not_this_game";
                Reject(report, session, validBytes, "foreign_namespace", JsonUtility.ToJson(invalid));
                invalid = JsonUtility.FromJson<InvestigationSave>(validJson); invalid.schema = 99;
                Reject(report, session, validBytes, "unsupported_schema", JsonUtility.ToJson(invalid));
                Reject(report, session, validBytes, "truncated_json", "{\"schema\":");
                Add(report, "recover_valid_save_after_rejections", session.Load() && session.Observation("slide", 0));
                invalid = JsonUtility.FromJson<InvestigationSave>(validJson); invalid.evidence[0].observations.Add(-1);
                Reject(report, session, validBytes, "negative_observation_index", JsonUtility.ToJson(invalid));
                invalid = JsonUtility.FromJson<InvestigationSave>(validJson); invalid.evidence[0].observations.Add(999);
                session.OpenPage("cipher"); session.SetPaused(true);
                Reject(report, session, validBytes, "rejection_preserves_paused_modal", JsonUtility.ToJson(invalid));
                Add(report, "fixture_required_references", session.movement && session.movement.controller && session.look && session.eye && session.gate && session.modelGate);

                ValidateInvalid(report, validJson, "null_save", null);
                ValidateMutation(report, validJson, "null_evidence_list", x => x.evidence = null);
                ValidateMutation(report, validJson, "null_evidence_record", x => x.evidence.Add(null));
                ValidateMutation(report, validJson, "null_observations", x => x.evidence[0].observations = null);
                ValidateMutation(report, validJson, "null_mapping_list", x => x.mapping = null);
                ValidateMutation(report, validJson, "null_assignment", x => x.mapping.Add(null));
                ValidateMutation(report, validJson, "null_relations_list", x => x.relations = null);
                ValidateMutation(report, validJson, "null_relation", x => x.relations.Add(null));
                ValidateMutation(report, validJson, "null_trials_list", x => x.trials = null);
                ValidateMutation(report, validJson, "null_trial", x => x.trials.Add(null));
                ValidateMutation(report, validJson, "null_conclusions_list", x => x.conclusions = null);
                ValidateMutation(report, validJson, "null_conclusion", x => x.conclusions.Add(null));
                ValidateMutation(report, validJson, "null_grounds", x => x.conclusions[0].grounds = null);
                ValidateMutation(report, validJson, "null_gate", x => x.gate = null);
                ValidateMutation(report, validJson, "nonfinite_position", x => x.position.x = float.NaN);
                ValidateMutation(report, validJson, "nonfinite_yaw", x => x.yaw = float.NaN);
                ValidateMutation(report, validJson, "nonfinite_pitch", x => x.pitch = float.PositiveInfinity);
                ValidateMutation(report, validJson, "nonfinite_gate_angle", x => x.gate.angle = float.NaN);
                ValidateMutation(report, validJson, "nonfinite_gate_velocity", x => x.gate.velocity = float.NegativeInfinity);
                var compatible = JsonUtility.FromJson<InvestigationSave>(validJson);
                compatible.conclusions.Clear();
                string beforeValidation = JsonUtility.ToJson(compatible);
                string error;
                Add(report, "valid_schema_with_unfilled_conclusions", InvestigationSaveValidation.TryValidate(compatible, out error) && beforeValidation == JsonUtility.ToJson(compatible));

                // Relation reference safety: note content and collection membership are not new Load rules.
                foreach (string endpoint in new[] { "first", "second" })
                    foreach (string badId in new[] { "missing_relation_evidence", "", null })
                    {
                        var malformed = JsonUtility.FromJson<InvestigationSave>(validJson);
                        if (endpoint == "first") malformed.relations[0].first = badId;
                        else malformed.relations[0].second = badId;
                        string label = "relation_" + endpoint + "_" + (badId == null ? "null" : badId == "" ? "empty" : "unknown");
                        session.OpenPage("dossier"); session.SetPaused(true);
                        Reject(report, session, validBytes, label, JsonUtility.ToJson(malformed));
                    }
                AcceptRelation(report, session, validBytes, validJson, "no_relations", x => x.relations.Clear());
                AcceptRelation(report, session, validBytes, validJson, "uncertain_note", x => x.relations[0].note = "Chưa đủ chứng minh; cần kiểm thêm.");
                AcceptRelation(report, session, validBytes, validJson, "empty_note", x => x.relations[0].note = "");
                AcceptRelation(report, session, validBytes, validJson, "null_note", x => x.relations[0].note = null);
                AcceptRelation(report, session, validBytes, validJson, "known_uncollected_endpoint", x => x.relations[0].first = "phone");
                AcceptRelation(report, session, validBytes, validJson, "same_known_endpoints", x => x.relations[0].second = x.relations[0].first);

                string compatibilityPath = Arg("--save-contract-compat");
                if (!string.IsNullOrEmpty(compatibilityPath))
                {
                    byte[] oldSave = File.ReadAllBytes(compatibilityPath);
                    File.WriteAllBytes(session.SavePath, oldSave);
                    Add(report, "load_actual_baseline_schema1_file", session.Load() && session.Observation("slide", 0) && !session.Observation("slide", 1) && session.data.mapping.Count == 1 && session.data.relations.Count == 1);
                    session.Observe("slide", 1); session.Save();
                    Add(report, "continue_baseline_file_in_private_profile", session.Load() && session.Observation("slide", 1));
                    Add(report, "baseline_compatibility_file_unchanged", Convert.ToBase64String(oldSave) == Convert.ToBase64String(File.ReadAllBytes(compatibilityPath)));
                }
            }
            catch (Exception ex) { Add(report, "fixture_exception", false, ex.ToString()); }
            finally
            {
                if (root) UnityEngine.Object.DestroyImmediate(root);
                Time.timeScale = 1;
                string output = SessionState.GetString(Output, "");
                File.WriteAllText(Path.Combine(output, "results.json"), JsonUtility.ToJson(report, true));
                SessionState.SetInt(ExitCode, report.failures == 0 ? 0 : 1);
                Debug.Log("Save contract checks: " + report.checks.Count + " checks, " + report.failures + " failures; " + output);
                SessionState.SetString(Active + ".Phase", "finished");
                EditorApplication.ExitPlaymode();
            }
        }
        static void AcceptRelation(Report report, InvestigationSession session, byte[] validBytes, string validJson, string name, Action<InvestigationSave> mutation)
        {
            var incoming = JsonUtility.FromJson<InvestigationSave>(validJson); mutation(incoming);
            string json = JsonUtility.ToJson(incoming); File.WriteAllText(session.SavePath, json);
            bool loaded = session.Load();
            string relationJson = JsonUtility.ToJson(new RelationEnvelope { items = session.data.relations });
            string expected = JsonUtility.ToJson(new RelationEnvelope { items = JsonUtility.FromJson<InvestigationSave>(json).relations });
            bool retained = loaded && relationJson == expected;
            if (loaded) session.Save();
            bool continued = loaded && session.Load() && JsonUtility.ToJson(new RelationEnvelope { items = session.data.relations }) == expected;
            Add(report, "relation_valid_" + name, retained && continued, "Serialized-file Load/Save/Load via API fixture; no UI claim.");
            File.WriteAllBytes(session.SavePath, validBytes);
            if (!session.Load()) throw new InvalidOperationException("Could not recover valid save after " + name);
        }
        [Serializable] class RelationEnvelope { public List<EvidenceRelation> items; }
        static void ValidateMutation(Report report, string validJson, string name, Action<InvestigationSave> mutation)
        {
            var incoming = JsonUtility.FromJson<InvestigationSave>(validJson);
            mutation(incoming);
            ValidateInvalid(report, validJson, name, incoming);
        }
        static void ValidateInvalid(Report report, string validJson, string name, InvestigationSave incoming)
        {
            string error;
            bool accepted = InvestigationSaveValidation.TryValidate(incoming, out error);
            Add(report, "validator_" + name, !accepted && !string.IsNullOrEmpty(error), "Pure API validation; not a serialized-file or UI route test.");
        }
        static void Reject(Report report, InvestigationSession session, byte[] validBytes, string name, string json)
        {
            var before = session.data;
            string beforeData = JsonUtility.ToJson(before);
            Vector3 position = session.movement.transform.position;
            string gate = JsonUtility.ToJson(session.gate.state);
            float yaw = session.movement.transform.eulerAngles.y, pitch = session.eye.transform.localEulerAngles.x, scale = Time.timeScale;
            bool actions = session.movement.GameplayActionsEnabled;
            var cursor = Cursor.lockState;
            string page = session.Page;
            bool paused = session.Paused, movement = session.movement.enabled, look = session.look.enabled;
            File.WriteAllText(session.SavePath, json);
            bool accepted = session.Load();
            bool preserved = ReferenceEquals(session.data, before) && JsonUtility.ToJson(session.data) == beforeData && session.movement.transform.position == position && JsonUtility.ToJson(session.gate.state) == gate && session.Page == page && session.Paused == paused && session.movement.enabled == movement && session.look.enabled == look && session.movement.transform.eulerAngles.y == yaw && session.eye.transform.localEulerAngles.x == pitch && Time.timeScale == scale && session.movement.GameplayActionsEnabled == actions && Cursor.lockState == cursor;
            Add(report, name, !accepted && preserved && File.ReadAllText(session.SavePath) == json && session.Message.StartsWith("Không tải:"), "Load accepted=" + accepted + "; live state preserved=" + preserved + "; disk file unchanged=" + (File.ReadAllText(session.SavePath) == json));
            File.WriteAllBytes(session.SavePath, validBytes);
            if (!session.Load()) throw new InvalidOperationException("Fixture could not recover a valid save after " + name);
        }
    }
}
