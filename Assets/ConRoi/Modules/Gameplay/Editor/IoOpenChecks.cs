using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using ConRoi.Investigation.V36.R107;

namespace ConRoi.Gameplay.EditorChecks
{
    // Finite IO-OPEN-01 diagnosis. No desktop input, runtime hook or automatic retry.
    [InitializeOnLoad]
    public static class IoOpenChecks
    {
        const string Key = "ConRoi.IoOpen01";
        const string Scene = "Assets/ConRoi/Modules/Gameplay/Scenes/Gameplay_ThuNghiem.unity";
        static InvestigationSession session;
        static JArray attempts, loads;
        static int unexpected, violations;
        static string output, profile;
        static IoOpenChecks() { EditorApplication.update += Tick; }
        static string Arg(string key)
        {
            var args = Environment.GetCommandLineArgs(); int i = Array.IndexOf(args, key);
            return i >= 0 && i + 1 < args.Length ? args[i + 1] : "";
        }
        public static void RunBatch()
        {
            output = Arg("--io-output"); profile = Arg("--investigation-save-dir");
            if (!Application.isBatchMode || Application.unityVersion != "6000.6.2f1" || EditorApplication.isPlayingOrWillChangePlaymode)
                throw new InvalidOperationException("Explicit batch Unity 6000.6.2f1 required");
            if (!Path.IsPathRooted(profile) || Directory.Exists(profile) || !Path.IsPathRooted(output) || Directory.Exists(output))
                throw new InvalidOperationException("New private output and profile required");
            Directory.CreateDirectory(output);
            EditorSceneManager.OpenScene(Scene, OpenSceneMode.Single);
            SessionState.SetString(Key, "pending"); EditorApplication.EnterPlaymode();
        }
        static string Hash(string path)
        {
            if (!File.Exists(path)) return "MISSING";
            // Capture only outside injected exclusive handles; dispose before any write.
            using (var sha = SHA256.Create()) return BitConverter.ToString(sha.ComputeHash(File.ReadAllBytes(path))).Replace("-", "").ToLowerInvariant();
        }
        static JObject Files() => new JObject {
            ["main"] = Hash(session.SavePath), ["checkpoint"] = Hash(session.BeforeConclusionSavePath),
            ["main_tmp"] = Hash(session.SavePath + ".tmp"), ["checkpoint_tmp"] = Hash(session.BeforeConclusionSavePath + ".tmp")
        };
        static string Live() => JsonUtility.ToJson(session.data) + "|" + session.Page + "|" + session.Paused + "|" + session.ConclusionsConfirmed + "|" + JsonUtility.ToJson(session.movement.transform.position) + "|" + JsonUtility.ToJson(session.gate.state) + "|" + session.TrialTime + "|" + session.TrialRunning;
        static void Draft(string label)
        {
            session.ClosePage(); session.SetPaused(false);
            session.data = new InvestigationSave { introAccepted = true };
            for (int i = 0; i < 4; i++) session.data.conclusions.Add(new ConclusionRecord());
            session.Acquire("slide"); session.Observe("slide", 0); session.OpenPage("conclusions");
            session.data.conclusions[0].claim = label;
            session.data.conclusions[0].grounds.Add("slide");
        }
        static bool Attempt(string name, string injection, Func<bool> action, bool checkLive = true)
        {
            bool expectedFailure = injection != "none";
            var row = new JObject { ["name"] = name, ["injection"] = injection, ["expected_failure"] = expectedFailure,
                ["utc"] = DateTime.UtcNow.ToString("o"), ["main_path"] = session.SavePath, ["before_files"] = Files(), ["before_live"] = Live() };
            bool ok = false;
            try { ok = action(); }
            catch (Exception e)
            {
                while (e is TargetInvocationException && e.InnerException != null) e = e.InnerException;
                row["exception_type"] = e.GetType().FullName; row["hresult"] = "0x" + e.HResult.ToString("X8");
                row["exception"] = e.ToString();
            }
            row["result"] = ok; row["message"] = session.Message;
            row["after_files"] = Files(); row["after_live"] = Live();
            row["live_unchanged"] = (string)row["before_live"] == (string)row["after_live"];
            row["main_unchanged"] = (string)row["before_files"]["main"] == (string)row["after_files"]["main"];
            bool safe = ok || ((bool)row["main_unchanged"] && (!checkLive || (bool)row["live_unchanged"]));
            row["failure_safeguard"] = safe;
            if (ok == expectedFailure || !safe) violations++;
            if (!ok && !expectedFailure) unexpected++;
            attempts.Add(row);
            File.WriteAllText(Path.Combine(output, "attempts.json"), attempts.ToString());
            return ok;
        }
        static void Seed(string name)
        {
            if (!Attempt(name, "none", () => { session.Save(); return session.Message.StartsWith("Đã lưu"); }, false))
                throw new IOException("Seed did not commit; no later Load may assume a new save");
        }
        static void LoadMain(string name)
        {
            var before = Files(); bool result = session.Load(); var after = Files();
            bool readOnly = JToken.DeepEquals(before, after);
            loads.Add(new JObject { ["name"] = name, ["result"] = result, ["files_unchanged"] = readOnly, ["message"] = session.Message });
            if (!result || !readOnly) violations++;
        }
        static void Commit(string name)
        {
            session.SetConclusionConfirmation(true); Attempt(name, "none", () => session.TryFinalizeConclusions());
            LoadMain(name + "_load");
        }
        static void RunSessionSequences()
        {
            session.SaveDirectoryOverride = Path.Combine(profile, "no-injection");
            Draft("seed"); Seed("normal_seed");
            for (int i = 0; i < 6; i++)
            {
                Draft("normal " + i); session.data.conclusions[1].claim = "Điều vừa sửa";
                session.SetConclusionConfirmation(true); session.ClosePage(); session.OpenPage("conclusions");
                session.SetConclusionConfirmation(true); session.SetPaused(true); session.SetPaused(false);
                Commit("normal_resume_" + i);
            }
            session.SaveDirectoryOverride = Path.Combine(profile, "mixed-original-sequence");
            Draft("mixed target"); Seed("mixed_seed"); session.SetConclusionConfirmation(true);
            Attempt("mixed_target_locked", "exclusive-main-handle", () => {
                using (var held = new FileStream(session.SavePath, FileMode.Open, FileAccess.Read, FileShare.None)) return session.TryFinalizeConclusions();
            });
            LoadMain("after_target_failure"); Commit("target_explicit_recovery");
            Draft("mixed tmp"); Seed("tmp_seed"); session.SetConclusionConfirmation(true);
            string block = session.BeforeConclusionSavePath + ".tmp";
            Attempt("mixed_tmp_directory", "directory-at-checkpoint-tmp", () => {
                Directory.CreateDirectory(block);
                try { return session.TryFinalizeConclusions(); }
                finally { Directory.Move(block, Path.Combine(output, "blocked-tmp-preserved")); }
            });
            Commit("tmp_explicit_recovery");
            Draft("mixed checkpoint"); Seed("checkpoint_seed"); session.SetConclusionConfirmation(true);
            Attempt("mixed_checkpoint_locked", "exclusive-checkpoint-handle", () => {
                using (var held = new FileStream(session.BeforeConclusionSavePath, FileMode.Open, FileAccess.Read, FileShare.None)) return session.TryFinalizeConclusions();
            });
            Commit("checkpoint_explicit_recovery");
            Draft("mixed resume"); session.data.conclusions[1].claim = "Điều vừa sửa";
            session.SetConclusionConfirmation(true); session.ClosePage(); session.OpenPage("conclusions");
            session.SetConclusionConfirmation(true); session.SetPaused(true); session.SetPaused(false);
            Commit("mixed_resume_after_faults");
        }
        static void Writer(string path, string json)
        {
            var type = typeof(InvestigationSession).Assembly.GetType("ConRoi.Investigation.V36.R107.InvestigationSaveFile", true);
            var write = type.GetMethod("Write", BindingFlags.Static | BindingFlags.NonPublic, null, new[] { typeof(string), typeof(string) }, null);
            write.Invoke(null, new object[] { path, json });
        }
        static void RunDirectWriter()
        {
            session.SaveDirectoryOverride = Path.Combine(profile, "direct-writer"); Draft("direct writer fixture");
            string json = JsonUtility.ToJson(session.data, true);
            Attempt("direct_seed", "none", () => { Writer(session.SavePath, json); return true; });
            for (int i = 0; i < 8; i++)
                foreach (bool checkpoint in new[] { true, false })
                {
                    string path = checkpoint ? session.BeforeConclusionSavePath : session.SavePath;
                    Attempt("direct_" + i + (checkpoint ? "_checkpoint" : "_main"), "none", () => { Writer(path, json); return File.ReadAllText(path) == json; });
                }
            Attempt("direct_lock_calibration", "exclusive-main-handle", () => {
                using (var held = new FileStream(session.SavePath, FileMode.Open, FileAccess.Read, FileShare.None)) Writer(session.SavePath, json);
                return true;
            });
            Attempt("direct_explicit_recovery", "none", () => { Writer(session.SavePath, json); return File.ReadAllText(session.SavePath) == json; });
        }
        static void Tick()
        {
            if (EditorApplication.isCompiling || EditorApplication.isUpdating) return;
            string phase = SessionState.GetString(Key, "");
            if (phase == "finished" && !EditorApplication.isPlayingOrWillChangePlaymode) { SessionState.SetString(Key, ""); EditorApplication.Exit(SessionState.GetInt(Key + ".exit", 2)); return; }
            if (phase != "pending" || !EditorApplication.isPlaying) return;
            SessionState.SetString(Key, "running"); output = Arg("--io-output"); profile = Arg("--investigation-save-dir");
            attempts = new JArray(); loads = new JArray(); GameObject root = null; string exception = "";
            try
            {
                session = TrialPlaybackContextChecks.BuildEnvironment(out root);
                session.movement.enabled = false; session.look.enabled = false;
                RunSessionSequences(); RunDirectWriter();
            }
            catch (Exception e) { violations++; exception = e.ToString(); }
            finally
            {
                if (root) UnityEngine.Object.DestroyImmediate(root);
                var result = new JObject { ["source"] = Arg("--io-source"), ["kind"] = "FINITE_PLAYMODE_API_PROXY_NOT_UI", ["unity"] = Application.unityVersion,
                    ["scene"] = Scene, ["profile"] = profile, ["process"] = System.Diagnostics.Process.GetCurrentProcess().Id,
                    ["os"] = Environment.OSVersion.ToString(), ["free_bytes"] = new DriveInfo(Path.GetPathRoot(profile)).AvailableFreeSpace,
                    ["unexpected_failures"] = unexpected, ["violations"] = violations, ["fixture_exception"] = exception, ["attempts"] = attempts, ["loads"] = loads };
                File.WriteAllText(Path.Combine(output, "results.json"), result.ToString());
                SessionState.SetInt(Key + ".exit", violations == 0 ? 0 : 1); SessionState.SetString(Key, "finished"); EditorApplication.ExitPlaymode();
            }
        }
    }
}
