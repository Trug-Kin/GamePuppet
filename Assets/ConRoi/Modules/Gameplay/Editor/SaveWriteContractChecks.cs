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
    public static class SaveWriteContractChecks
    {
        const string Active = "ConRoi.SaveWriteChecks.Active";
        const string ExitCode = "ConRoi.SaveWriteChecks.Exit";
        const string Output = "ConRoi.SaveWriteChecks.Output";
        const string TestScene = "Assets/ConRoi/Modules/Gameplay/Scenes/Gameplay_ThuNghiem.unity";
        [Serializable] class Check { public string name; public bool pass; public string detail; }
        [Serializable] class Report
        {
            public string unity, scene, profile, kind = "PLAYMODE_API_FIXTURE_NOT_NATURAL_ROUTE";
            public int missingScripts, failures;
            public string sourceCommit; public List<Check> checks = new List<Check>();
        }
        static SaveWriteContractChecks() { EditorApplication.update += Tick; }
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
            string output = Arg("--save-write-output");
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
        static string Hash(string path) { using(var h=System.Security.Cryptography.SHA256.Create()) return BitConverter.ToString(h.ComputeHash(File.ReadAllBytes(path))).Replace("-", "").ToLowerInvariant(); }
        static bool Equal(byte[] bytes,string path) => File.Exists(path) && Convert.ToBase64String(bytes)==Convert.ToBase64String(File.ReadAllBytes(path));
        static void Execute()
        {
            var report=new Report { unity=Application.unityVersion,scene=SceneManager.GetActiveScene().path,profile=Arg("--investigation-save-dir"),sourceCommit=Arg("--source-commit") };
            GameObject root=null;
            try {
                root=new GameObject("SaveWrite_API_Fixture");root.SetActive(false);
                var player=new GameObject("Player");player.transform.SetParent(root.transform);
                var movement=player.AddComponent<DieuKhienDiChuyen>();movement.controller=player.AddComponent<CharacterController>();
                var eye=new GameObject("Eye");eye.transform.SetParent(player.transform);var camera=eye.AddComponent<Camera>();var look=eye.AddComponent<DieuKhienGocNhin>();
                var g=new GameObject("Gate");g.transform.SetParent(root.transform);var gate=g.AddComponent<ReturnGateMechanism>();gate.enabled=false;gate.checkPlayerContact=false;
                var m=new GameObject("Model");m.transform.SetParent(root.transform);var model=m.AddComponent<ReturnGateMechanism>();model.enabled=false;model.checkPlayerContact=false;
                var session=root.AddComponent<InvestigationSession>();session.movement=movement;session.look=look;session.eye=camera;session.gate=gate;session.modelGate=model;
                root.SetActive(true);session.enabled=false;movement.enabled=false;look.enabled=false;
                Add(report,"private_profile",session.SaveDirectoryOverride==report.profile);
                Add(report,"scene_and_missing_scripts",report.scene==TestScene && GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(root)==0);
                foreach(string scenario in new[]{"normal","temp_directory","temp_write_locked","destination_locked","temp_delete_locked","stale_temp"})
                    Scenario(report,session,scenario);
                FirstCommitFailure(report,session);
                InjectedWrite(report,session);
            }catch(Exception ex){Add(report,"fixture_exception",false,ex.ToString());}
            finally {
                if(root)UnityEngine.Object.DestroyImmediate(root);Time.timeScale=1;
                File.WriteAllText(Path.Combine(SessionState.GetString(Output,""),"results.json"),JsonUtility.ToJson(report,true));
                SessionState.SetInt(ExitCode,report.failures==0?0:1);SessionState.SetString(Active+".Phase","finished");
                Debug.Log("Save write checks: "+report.checks.Count+" checks, "+report.failures+" failures");EditorApplication.ExitPlaymode();
            }
        }
        static void Scenario(Report report,InvestigationSession session,string scenario)
        {
            session.SaveDirectoryOverride=Path.Combine(report.profile,scenario);
            session.data=new InvestigationSave();session.ClosePage();session.Acquire("slide");session.Observe("slide",0);
            session.Save();string path=session.SavePath,temp=path+".tmp";
            Add(report,scenario+"_initial_save",File.Exists(path)&&session.Message.StartsWith("Đã lưu"));
            byte[] good=File.ReadAllBytes(path);string oldHash=Hash(path);
            session.Observe("slide",1);FileStream held=null;
            try {
                if(scenario=="temp_directory")Directory.CreateDirectory(temp);
                if(scenario=="temp_write_locked") {File.WriteAllText(temp,"partial prior write");held=new FileStream(temp,FileMode.Open,FileAccess.Read,FileShare.Read);}
                if(scenario=="destination_locked")held=new FileStream(path,FileMode.Open,FileAccess.Read,FileShare.Read);
                if(scenario=="temp_delete_locked") {File.WriteAllText(temp,"partial prior write");held=new FileStream(temp,FileMode.Open,FileAccess.Read,FileShare.ReadWrite);}
                if(scenario=="stale_temp") {
                    File.WriteAllText(temp,"{partial stale save");
                    Add(report,"stale_temp_not_loaded",session.Load() && !session.Observation("slide",1) && Equal(good,path));
                    session.Observe("slide",1);
                }
                session.Save();string message=session.Message;
                bool shouldFail=scenario!="normal"&&scenario!="stale_temp";
                Add(report,scenario+"_message",shouldFail?message.StartsWith("Không lưu được:"):message.StartsWith("Đã lưu"),message);
                if(shouldFail)Add(report,scenario+"_old_bytes_preserved",Equal(good,path),"old="+oldHash+" after="+Hash(path));
                if(File.Exists(temp))File.Copy(temp,Path.Combine(Path.GetDirectoryName(path),"attempt-temp-evidence.json"));
            } finally {held?.Dispose();if(Directory.Exists(temp))Directory.Move(temp,temp+".blocker-preserved");}
            bool expectOld=scenario!="normal"&&scenario!="stale_temp";
            Add(report,scenario+"_load_after_attempt",session.Load()&&session.Observation("slide",0)&&(session.Observation("slide",1)!=expectOld));
            session.Observe("slide",1);session.Save();
            Add(report,scenario+"_recovery_save_load",session.Message.StartsWith("Đã lưu")&&session.Load()&&session.Observation("slide",1));
            Add(report,scenario+"_no_temp_after_success",!File.Exists(temp)&&!Directory.Exists(temp));
        }
        static void FirstCommitFailure(Report report,InvestigationSession session)
        {
            session.SaveDirectoryOverride=Path.Combine(report.profile,"first_commit_locked");session.data=new InvestigationSave();session.Acquire("slide");
            string temp=session.SavePath+".tmp";Directory.CreateDirectory(Path.GetDirectoryName(temp));File.WriteAllText(temp,"old unfinished first attempt");
            using(var held=new FileStream(temp,FileMode.Open,FileAccess.Read,FileShare.ReadWrite)) {
                session.Save();Add(report,"first_commit_failure_no_destination",session.Message.StartsWith("Không lưu được:")&&!File.Exists(session.SavePath));
                Add(report,"first_commit_failure_load_does_not_use_temp",!session.Load());
            }
            session.Save();Add(report,"first_commit_recovery",session.Message.StartsWith("Đã lưu")&&session.Load()&&session.Has("slide"));
        }
        static void InjectedWrite(Report report,InvestigationSession session)
        {
            // Baseline has no helper: do not pretend this injection ran before it exists.
            var type=typeof(InvestigationSession).Assembly.GetType("ConRoi.Investigation.V36.R107.InvestigationSaveFile");
            if(type==null) {Add(report,"partial_write_injection_available",false,"NOT_RUN on baseline; helper does not exist yet.");return;}
            Add(report,"partial_write_injection_available",true);
            var method=type.GetMethod("Write",System.Reflection.BindingFlags.Static|System.Reflection.BindingFlags.NonPublic,null,new[]{typeof(string),typeof(string),typeof(Action<FileStream,byte[]>)},null);
            session.SaveDirectoryOverride=Path.Combine(report.profile,"partial_write_injected");session.data=new InvestigationSave();session.Acquire("slide");session.Observe("slide",0);session.Save();
            byte[] good=File.ReadAllBytes(session.SavePath);bool threw=false;
            try {method.Invoke(null,new object[]{session.SavePath,JsonUtility.ToJson(session.data),new Action<FileStream,byte[]>((stream,bytes)=>{stream.Write(bytes,0,Math.Min(12,bytes.Length));throw new IOException("SAVE-WRITE-03 simulated partial write");})});}
            catch(System.Reflection.TargetInvocationException e) {threw=e.InnerException is IOException;}
            Add(report,"partial_write_injected_preserves_old",threw&&Equal(good,session.SavePath)&&session.Load());
            if(File.Exists(session.SavePath+".tmp"))File.Copy(session.SavePath+".tmp",session.SavePath+".partial-evidence");
            session.Observe("slide",1);session.Save();Add(report,"partial_write_recovery",session.Message.StartsWith("Đã lưu")&&session.Load()&&session.Observation("slide",1));
        }
    }
}
