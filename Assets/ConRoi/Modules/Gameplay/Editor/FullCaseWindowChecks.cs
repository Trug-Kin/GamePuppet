using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using ConRoi.Investigation.V36.R107;
namespace ConRoi.Gameplay.EditorChecks {
    public static class FullCaseWindowChecks {
        static FullCaseSandboxWindow window; static string output; static JArray rows=new JArray(); static int failures;static double began;
        static string Arg(string k){var a=Environment.GetCommandLineArgs();int i=Array.IndexOf(a,k);return i>=0&&i+1<a.Length?a[i+1]:"";}
        static void Check(string n,bool pass){rows.Add(new JObject{["name"]=n,["pass"]=pass});if(!pass)failures++;}
        static void Must(FullActionResult r){if(!r.Success)throw new Exception(r.Code+": "+r.Message);}
        static void Key(KeyCode code){window.SendEvent(new Event{type=EventType.KeyDown,keyCode=code});}
        public static void RunBatch(){output=Arg("--full-window-output");if(!Application.isBatchMode||Directory.Exists(output)||string.IsNullOrEmpty(output))throw new Exception("Fresh output required");Directory.CreateDirectory(output);EditorSceneManager.OpenScene("Assets/ConRoi/Modules/Gameplay/Scenes/Gameplay_ThuNghiem.unity");window=EditorWindow.GetWindow<FullCaseSandboxWindow>();window.Show();window.Repaint();began=EditorApplication.timeSinceStartup;EditorApplication.update+=Tick;}
        static void Tick(){if(EditorApplication.timeSinceStartup-began<3)return;EditorApplication.update-=Tick;
            string profile="";
            try{var a=window.ControllerForChecks;profile=a.Profile;var s=a.Runtime;
                Must(a.Interact(FullPlaceholderKind.Location,"workshop","workshop"));Must(a.Interact(FullPlaceholderKind.Evidence,"slide","workshop"));Must(s.TryInspect("slide"));Must(s.TryRecordObservation("slide",0));Key(KeyCode.Escape);Check("escape_closes_inspection_restores_input",s.Modal==FullCaseModal.None&&s.CanMove);
                string before=s.ExportStateJson();Key(KeyCode.F5);Check("F5_editor_event_writes_private_profile",File.Exists(FullInvestigationRuntime.SavePathFor(profile)));byte[] good=File.ReadAllBytes(FullInvestigationRuntime.SavePathFor(profile));
                Must(a.Interact(FullPlaceholderKind.Location,"market","market"));Key(KeyCode.F6);Check("F6_editor_event_restores",s.ExportStateJson()==before&&s.CanMove);
                Key(KeyCode.Escape);Check("escape_opens_pause",s.Paused&&!s.CanMove);Key(KeyCode.Escape);Check("escape_resumes",!s.Paused&&s.CanMove);
                File.WriteAllText(FullInvestigationRuntime.SavePathFor(profile),"{bad json");byte[] bad=File.ReadAllBytes(FullInvestigationRuntime.SavePathFor(profile));Key(KeyCode.F6);Check("F6_bad_JSON_keeps_state_bytes_and_feedback",s.ExportStateJson()==before&&bad.SequenceEqual(File.ReadAllBytes(FullInvestigationRuntime.SavePathFor(profile)))&&s.LastMessage.StartsWith("Không tải")&&s.CanMove);
                s.SetModal(FullCaseModal.Cipher);Check("cipher_locks_move",!s.CanMove);Key(KeyCode.Escape);Check("escape_after_failure_clears_modal",s.CanMove);
                File.WriteAllBytes(FullInvestigationRuntime.SavePathFor(profile),good);Key(KeyCode.F6);Check("F6_valid_after_rejection",s.LastMessage.StartsWith("Đã tải")&&s.ExportStateJson()==before&&s.CanMove);
                Must(a.Interact(FullPlaceholderKind.Location,"market","market"));Key(KeyCode.F5);Must(a.Interact(FullPlaceholderKind.Location,"workshop","workshop"));Key(KeyCode.F6);Check("continue_save_load_new_progress",s.Location=="market"&&s.CanMove);
            }catch(Exception e){rows.Add(new JObject{["pass"]=false,["exception"]=e.ToString()});failures++;}
            finally{if(window)window.Close();File.WriteAllText(Path.Combine(output,"results.json"),new JObject{["source"]=Arg("--full-source"),["unity"]=Application.unityVersion,["profile"]=profile,["kind"]="EDITOR_WINDOW_UNITY_KEY_EVENTS_AND_API_SETUP_NOT_WINDOWS_NOT_NATURAL_PLAY_NOT_SCENE_INTEGRATION",["failures"]=failures,["checks"]=rows}.ToString());EditorApplication.Exit(failures==0?0:1);}
        }
    }
}
