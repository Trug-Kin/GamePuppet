using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using ConRoi.Investigation.V36.R107;
namespace ConRoi.Gameplay.EditorChecks
{
    public static class ReadingProducerChecks
    {
        static string Arg(string key){var args=Environment.GetCommandLineArgs();int i=Array.IndexOf(args,key);return i>=0&&i+1<args.Length?args[i+1]:"";}
        static void Must(FullActionResult r){if(!r.Success)throw new Exception(r.Code+": "+r.Message);}
        static FullCaseSandboxWindow window;static FullCaseSandboxController a;
        static FullInvestigationRuntime S=>a.Runtime;
        static readonly JArray rows=new JArray();static int failures;static string output;static double start;
        static void Check(string name,bool pass,string kind="API_ACTION_ROUTE"){rows.Add(new JObject{["name"]=name,["pass"]=pass,["kind"]=kind});if(!pass)throw new Exception(name);}
        static void Go(string location){a.Cancel();Must(a.Interact(FullPlaceholderKind.Location,location,location));}
        static void Read(string id){a.Cancel();Must(a.Interact(FullPlaceholderKind.Evidence,id,S.Location));Must(S.TryInspect(id));Must(S.TryRecordObservation(id,0));a.Cancel();}
        static string[][] GddReadings()=>File.ReadAllLines("Docs/CON_ROI_GDD_v3.6_CHOT_TRIEN_KHAI.md").Where(l=>l.StartsWith("| ")&&l.Contains("| STAND BY.")).Select(l=>l.Split('|').Select(x=>x.Trim()).ToArray()).ToArray();
        static bool MatchesGdd(FullRecordedReading reading){var line=GddReadings().Single(x=>x[1]==reading.Recipient);return reading.Text==line[2]&&reading.Meaning==line[3];}
        static void Key(KeyCode key)=>window.SendEvent(new Event{type=EventType.KeyDown,keyCode=key});
        public static void RunBatch()
        {
            output=Arg("--reading-output");if(!Application.isBatchMode||!Path.IsPathRooted(output)||Directory.Exists(output))throw new Exception("Fresh isolated output required");Directory.CreateDirectory(output);
            EditorSceneManager.OpenScene("Assets/ConRoi/Modules/Gameplay/Scenes/Gameplay_ThuNghiem.unity");
            if(!EditorApplication.ExecuteMenuItem("Con Roi/Gameplay/Toan tuyen TEST ONLY"))throw new Exception("Menu missing");
            window=Resources.FindObjectsOfTypeAll<FullCaseSandboxWindow>().Single();a=window.ControllerForChecks;window.Repaint();start=EditorApplication.timeSinceStartup;EditorApplication.update+=Tick;
        }
        static void Tick()
        {
            if(EditorApplication.timeSinceStartup-start<3)return;EditorApplication.update-=Tick;
            try{Verify();}catch(Exception e){failures++;rows.Add(new JObject{["pass"]=false,["exception"]=e.ToString()});}
            finally{string profile=a.Profile;if(window)window.Close();File.WriteAllText(Path.Combine(output,"results.json"),new JObject{["source"]=Arg("--reading-source"),["unity"]=Application.unityVersion,["kind"]="MENU_API_ACTION_ROUTE_AND_UNITY_KEY_EVENTS_TEST_ONLY_WORLD_NOT_WINDOWS_NOT_NATURAL_PLAY_NOT_PRODUCT_SCENE",["profile"]=profile,["failures"]=failures,["checks"]=rows}.ToString());EditorApplication.Exit(failures==0?0:1);}
        }
        static void Verify()
        {
            Check("fresh_menu_has_no_recorded_transcripts",S.Location=="office"&&S.GetRecords().Length==0&&S.GetRecordedReadings().Length==0);
            Check("unknown_and_unopened_queries_return_empty",!S.TryGetRecordedReading("unknown",out var unknown)&&unknown.Text==null&&!S.TryGetRecordedReading("letter.lan",out var closed)&&closed.Meaning==null);
            Check("unopened_cannot_record_reading",!S.TryRecordReading("letter").Success&&S.GetRecordedReadings().Length==0);
            Go("workshop");Read("slide");Go("market");string before=S.ExportStateJson();
            Check("derived_alias_not_offered_or_collectable",!S.GetLocalEvidence().Any(x=>x.id=="decoded")&&!S.TryCollect("decoded").Success&&S.LastMessage.Contains("bản đọc")&&S.ExportStateJson()==before);
            Read("letter");S.SetModal(FullCaseModal.Cipher);Check("view_without_mapping_cannot_produce_transcript",!S.TryRecordReading("letter").Success&&S.GetRecordedReadings().Length==0);
            Must(S.TryAssign("pose.01","Z"));before=S.ExportStateJson();Check("wrong_mapping_does_not_produce_or_change_progress",!S.TryRecordReading("letter").Success&&before==S.ExportStateJson()&&!S.Meets(new FullRequirement(FullRequirementKind.Decoded,"letter")));
            Go("school");Read("M3");Read("lead.theatre");Go("theatre");Read("henri");S.SetModal(FullCaseModal.Cipher);
            foreach(var pair in S.GetHenriLookup())Must(S.TryAssign(pair.pose,pair.letter));
            Check("correct_mapping_without_record_does_not_publish",S.GetRecordedReadings().Length==0&&!S.TryGetRecordedReading("letter",out _));
            int events=0;Action changed=()=>events++;S.Changed+=changed;int count=events;Must(S.TryRecordReading("letter"));
            Check("record_action_publishes_one_progress_event",events==count+1&&S.Meets(new FullRequirement(FullRequirementKind.Decoded,"letter")));
            Check("Ha_canonical_transcript_and_meaning_match_GDD",S.TryGetRecordedReading("letter",out var ha)&&MatchesGdd(ha));
            Check("one_record_does_not_reveal_other_letters",S.GetRecordedReadings().Length==1&&!S.TryGetRecordedReading("letter.bao",out _));
            before=S.ExportStateJson();count=events;Must(S.TryRecordReading("letter"));Check("repeat_no_duplicate_no_progress_event",S.GetRecordedReadings().Length==1&&S.ExportStateJson()==before&&events==count);
            var snapshot=S.GetRecordedReadings();snapshot[0]=default;S.TryGetRecordedReading("letter",out ha);Check("snapshot_queries_are_pure_and_isolated",MatchesGdd(ha)&&S.ExportStateJson()==before&&events==count);
            Key(KeyCode.F5);Check("F5_saves_recorded_reading",File.Exists(FullInvestigationRuntime.SavePathFor(a.Profile)),"UNITY_KEY_EVENT_WITH_API_SETUP");
            Must(S.TryAssign("pose.01","Z"));Check("editing_trial_mapping_keeps_recorded_transcript",S.TryGetRecordedReading("letter",out ha)&&MatchesGdd(ha));
            Key(KeyCode.F6);Check("F6_restores_mapping_and_recorded_reading",S.ExportStateJson()==before&&S.TryGetRecordedReading("letter",out ha)&&MatchesGdd(ha),"UNITY_KEY_EVENT_WITH_API_SETUP");
            foreach(var step in new[]{new[]{"lake","letter.bao"},new[]{"school","letter.mai"},new[]{"slope","letter.tung"},new[]{"garden","letter.lan"}}){
                Check(step[1]+"_hidden_before_record",!S.TryGetRecordedReading(step[1],out _));Go(step[0]);Read(step[1]);S.SetModal(FullCaseModal.Cipher);Must(S.TryRecordReading(step[1]));
                Check(step[1]+"_producer_matches_GDD_and_unlocks_only_its_flag",S.TryGetRecordedReading(step[1],out var reading)&&MatchesGdd(reading)&&S.Meets(new FullRequirement(FullRequirementKind.Decoded,step[1])));
            }
            Check("all_five_recorded_from_actions_without_alias",S.GetRecordedReadings().Length==5&&!S.HasEvidence("decoded"));
            S.SetPaused(true);before=S.ExportStateJson();Check("paused_record_denied_without_mutation",!S.TryRecordReading("letter").Success&&S.ExportStateJson()==before&&S.GetRecordedReadings().Length==5);S.SetPaused(false);a.Cancel();
            Key(KeyCode.Escape);Check("escape_pause_after_recording",S.Paused&&!S.CanMove,"UNITY_KEY_EVENT_WITH_API_SETUP");Key(KeyCode.Escape);Check("escape_resume_after_recording",!S.Paused&&S.CanMove,"UNITY_KEY_EVENT_WITH_API_SETUP");
            before=S.ExportStateJson();Key(KeyCode.F5);Go("workshop");Key(KeyCode.F6);Check("F5_F6_five_readings_continue_without_duplicates",S.ExportStateJson()==before&&S.GetRecordedReadings().Length==5&&S.GetRecordedReadings().All(MatchesGdd),"UNITY_KEY_EVENT_WITH_API_SETUP");
            S.Changed-=changed;
            foreach(var path in new[]{Arg("--reading-legacy-profile"),Arg("--reading-old-valid-profile")}){
                byte[] bytes=File.ReadAllBytes(FullInvestigationRuntime.SavePathFor(path));var old=new FullInvestigationRuntime(FullCaseTestCatalog.Create());Must(old.TryLoad(path));
                bool isRaw=path==Arg("--reading-legacy-profile");
                Check(isRaw?"legacy_alias_save_loads_without_promoting_to_reading":"old_fullcase_decoded_flags_load_as_canonical_readings",isRaw?old.HasEvidence("decoded")&&!old.Meets(new FullRequirement(FullRequirementKind.Decoded,"letter"))&&old.GetRecordedReadings().Length==0:old.GetRecordedReadings().Length==5&&old.GetRecordedReadings().All(MatchesGdd));
                Check(isRaw?"legacy_alias_source_bytes_unchanged":"old_valid_source_bytes_unchanged",bytes.SequenceEqual(File.ReadAllBytes(FullInvestigationRuntime.SavePathFor(path))));
            }
            var json=JObject.Parse(S.ExportStateJson());Check("no_transcript_text_or_new_schema_saved",json.Value<int>("schema")==1&&json.Value<string>("namespaceId")=="conroi.investigation.v36.fullcase"&&!json.Properties().Any(x=>x.Name.Contains("Transcript")||x.Name.Contains("Reading")));
        }
        public static void RunBaseline()
        {
            string output=Arg("--reading-output"),profile=Arg("--reading-profile");
            if(!Application.isBatchMode||!Path.IsPathRooted(output)||Directory.Exists(output)||!Path.IsPathRooted(profile)||Directory.Exists(profile))throw new Exception("Fresh isolated output/profile required");
            Directory.CreateDirectory(output);EditorSceneManager.OpenScene("Assets/ConRoi/Modules/Gameplay/Scenes/Gameplay_ThuNghiem.unity");
            try{var s=new FullInvestigationRuntime(FullCaseTestCatalog.Create());Must(s.TryTravel("workshop"));Must(s.TryCollect("slide"));Must(s.TryInspect("slide"));Must(s.TryRecordObservation("slide",0));s.Cancel();Must(s.TryTravel("market"));
                bool offered=s.GetLocalEvidence().Any(e=>e.id=="decoded");Must(s.TryCollect("decoded"));Must(s.TryInspect("decoded"));Must(s.TryRecordObservation("decoded",0));s.Cancel();
                bool reproduced=offered&&s.HasRecordedObservation("decoded",0)&&!s.Meets(new FullRequirement(FullRequirementKind.Decoded,"letter"));Must(s.TrySave(profile));
                File.WriteAllText(Path.Combine(output,"baseline.json"),new JObject{["source"]="69e9ca57",["kind"]="BASELINE_API_ACTION_ROUTE_NOT_GAMEPLAY_PASS",["reproduced"]=reproduced,["profile"]=profile,["state"]=JObject.Parse(s.ExportStateJson())}.ToString());EditorApplication.Exit(reproduced?0:1);
            }catch(Exception e){File.WriteAllText(Path.Combine(output,"error.txt"),e.ToString());EditorApplication.Exit(1);}
        }
    }
}
