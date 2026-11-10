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
    public static class ApprovedProducerChecks
    {
        static readonly JArray rows=new JArray();static FullCaseSandboxWindow window;static FullCaseSandboxController a;
        static FullInvestigationRuntime S=>a.Runtime;static string output;static double start;static int failed;
        static string Arg(string name){var x=Environment.GetCommandLineArgs();int i=Array.IndexOf(x,name);return i>=0&&i+1<x.Length?x[i+1]:"";}
        static void Must(FullActionResult r){if(!r.Success)throw new Exception(r.Code+": "+r.Message);}
        static void C(string name,bool pass,string kind="API_ACTION_ROUTE"){rows.Add(new JObject{["name"]=name,["pass"]=pass,["kind"]=kind});if(!pass)throw new Exception(name);}
        static void Go(string id){a.Cancel();Must(a.Interact(FullPlaceholderKind.Location,id,id));}
        static void View(string id,bool record=false){a.Cancel();if(!S.HasEvidence(id))Must(a.Interact(FullPlaceholderKind.Evidence,id,S.Location));Must(S.TryInspect(id));if(record)Must(S.TryRecordObservation(id,0));a.Cancel();}
        static void Ask(string actor,string topic){a.Cancel();Must(a.Interact(FullPlaceholderKind.Actor,"npc."+actor,S.Location));Must(a.Ask(topic));}
        static void Key(KeyCode code){window.SendEvent(new Event{type=EventType.KeyDown,keyCode=code});}
        public static void RunCompatibilityBatch()
        {
            string target=Arg("--approved-output");if(!Application.isBatchMode||!Path.IsPathRooted(target)||Directory.Exists(target))throw new Exception("Fresh isolated output required");Directory.CreateDirectory(target);
            try{
                foreach(var profile in new[]{Arg("--approved-legacy-profile"),Arg("--approved-old-valid-profile")}){
                    var runtime=new FullInvestigationRuntime(FullCaseTestCatalog.Create());string path=FullInvestigationRuntime.SavePathFor(profile);byte[] bytes=File.ReadAllBytes(path);var old=JObject.Parse(File.ReadAllText(path));Must(runtime.TryLoad(profile));var current=JObject.Parse(runtime.ExportStateJson());
                    C("schema1_preserves_explicit_legacy_progress_"+Path.GetFileName(profile),new[]{"evidence","mapping","decoded","permissions","comparisons","relations","trials","conclusions","ending","finalized","replySent"}.All(k=>JToken.DeepEquals(old[k],current[k])),"API_READ_ONLY_SCHEMA1_COMPATIBILITY");
                    C("schema1_infers_no_new_history_or_viewed_"+Path.GetFileName(profile),runtime.GetHeardStatements().Length==0&&runtime.GetRecordedStatements().Length==0&&!runtime.EndingViewed&&!runtime.CanBeginAppointment&&bytes.SequenceEqual(File.ReadAllBytes(path)),"API_READ_ONLY_SCHEMA1_COMPATIBILITY");
                }
            }catch(Exception e){failed++;rows.Add(new JObject{["pass"]=false,["exception"]=e.ToString()});}
            File.WriteAllText(Path.Combine(target,"results.json"),new JObject{["source"]=Arg("--approved-source"),["failures"]=failed,["checks"]=rows}.ToString());EditorApplication.Exit(failed==0?0:1);
        }
        public static void RunBatch()
        {
            output=Arg("--approved-output");if(!Application.isBatchMode||!Path.IsPathRooted(output)||Directory.Exists(output))throw new Exception("Fresh isolated output required");Directory.CreateDirectory(output);
            EditorSceneManager.OpenScene("Assets/ConRoi/Modules/Gameplay/Scenes/Gameplay_ThuNghiem.unity");EditorApplication.ExecuteMenuItem("Con Roi/Gameplay/Toan tuyen TEST ONLY");
            window=Resources.FindObjectsOfTypeAll<FullCaseSandboxWindow>().Single();a=window.ControllerForChecks;start=EditorApplication.timeSinceStartup;EditorApplication.update+=Tick;
        }
        static void Tick()
        {
            if(EditorApplication.timeSinceStartup-start<3)return;EditorApplication.update-=Tick;
            try{Route();Migration();Validation();EndingFixtures();LegacyTrialProjection();MissingSourceAndLegacyNotes();}catch(Exception e){failed++;rows.Add(new JObject{["pass"]=false,["exception"]=e.ToString()});}
            finally{string profile=a.Profile;if(window)window.Close();File.WriteAllText(Path.Combine(output,"results.json"),new JObject{["source"]=Arg("--approved-source"),["unity"]=Application.unityVersion,["kind"]="API_ACTION_ROUTE_AND_EDITOR_UI_EVENTS; TEST_ONLY_ASSETS_RUBRIC; NOT_WINDOWS_OR_NATURAL_OR_SHARED_SCENE",["profile"]=profile,["failures"]=failed,["checks"]=rows}.ToString());EditorApplication.Exit(failed==0?0:1);}
        }
        static void Route()
        {
            C("cargo_placeholder_not_R12_antique_substitute",!a.TestCatalog.comparisons.Single(t=>t.id=="cargo").evidenceIds.Contains("r12")&&a.TestCatalog.evidence.Any(e=>e.id=="test.antique.labels")&&a.TestCatalog.testOnly);
            C("unknown_unviewed_observation_options_hidden",S.GetObservationOptions("unknown").Length==0&&S.GetObservationOptions("letter").Length==0);
            C("fresh_no_history_no_notes_no_ending",S.GetHeardStatements().Length==0&&S.GetSourcedNotes().Length==0&&!S.EndingViewed&&!S.Finalized);
            C("new_source_gates_closed_without_hearing",!S.CanTravel("theatre")&&!S.CanTravel("homestay")&&!S.CanTravel("garden"));
            Ask("phuong","phuong.direction.homestay");C("hearing_Phuong_unlocks_without_record",S.CanTravel("homestay")&&S.GetRecordedStatements().Length==0&&!S.HasEvidence("lead.homestay"));
            a.Cancel();string snapshot=S.ExportStateJson();C("ask_after_cancel_no_history",!S.TryAskTopic("phuong.direction.homestay",out _).Success&&S.ExportStateJson()==snapshot);
            Go("workshop");View("slide",true);View("mechanism");S.SetModal(FullCaseModal.Inspect);Must(S.TryRecordSourcedObservation("mechanism.site.clear"));C("site_observation_has_new_stable_key",S.GetSourcedNotes().Single().Id=="mechanism.site.clear"&&!S.HasRecordedObservation("mechanism",1));
            snapshot=S.ExportStateJson();Must(S.TryRecordSourcedObservation("mechanism.site.clear"));C("new_site_note_repeat_no_duplicate",S.ExportStateJson()==snapshot);
            C("unknown_or_unseen_sourced_note_rejected",!S.TryRecordSourcedObservation("not.real").Success&&!S.TryRecordSourcedObservation("hook.site.dust").Success);
            View("hook");S.SetModal(FullCaseModal.Inspect);Must(S.TryRecordSourcedObservation("hook.site.dust"));
            Go("market");a.Cancel();Must(S.TryCollect("receipt"));Ask("ha","ha.channel.paper");
            C("hearing_channel_does_not_send_or_grant_evidence",!S.HasSentReply&&S.GetHeardStatements().Any(h=>h.id=="ha.channel.paper")&&!S.HasEvidence("receipt.channel"));
            snapshot=S.ExportStateJson();C("receipt_collected_not_inspected_topic_locked",!a.Ask("ha.cargo.receipt").Success&&S.ExportStateJson()==snapshot&&!S.GetConversationTopics().Single(t=>t.Id=="ha.cargo.receipt").CanAsk);
            C("wrong_topic_safe_no_answer",!S.TryAskTopic("does.not.exist",out var absent).Success&&absent.Answer==null);
            S.SetPaused(true);C("pause_ask_rejected",!a.Ask("ha.channel.paper").Success);S.SetPaused(false);
            a.Cancel();Must(S.TryInspect("receipt"));a.Cancel();Ask("ha","ha.cargo.receipt");
            C("inspection_opens_H1_without_recording",!S.HasRecordedObservation("receipt",0)&&a.Reply=="Phiếu ghi ba gói và ngày giao, không ghi bên trong.");
            int count=S.GetHeardStatements().Length;int events=0;Action changed=()=>events++;S.Changed+=changed;Must(a.Ask("ha.cargo.receipt"));
            C("repeat_ask_no_duplicate_or_progress_event",S.GetHeardStatements().Length==count&&events==0);S.Changed-=changed;
            var history=S.GetHeardStatements();history[0].answer="mutate returned snapshot";C("history_snapshot_isolated",!S.GetHeardStatements().Any(h=>h.answer=="mutate returned snapshot"));
            C("heard_not_recorded_and_not_citable",S.GetRecordedStatements().Length==0&&!S.GetAvailableGrounds().Any(g=>g.kind==FullRequirementKind.Heard));
            Must(S.TryRecordStatement("ha.receipt.facts"));Must(S.TryRecordStatement("ha.receipt.facts"));C("record_statement_separate_idempotent_and_citable",S.GetRecordedStatements().Length==1&&S.GetAvailableGrounds().Any(g=>g.kind==FullRequirementKind.Heard&&g.id=="ha.receipt.facts"));
            C("unheard_statement_cannot_record",!S.TryRecordStatement("bao.direction.lan").Success);
            View("receipt.channel");var options=S.GetObservationOptions("receipt.channel");options[0]="mutated snapshot";C("dynamic_receipt_observation_options_safe_and_isolated",S.GetObservationOptions("receipt.channel").Length==1&&S.GetObservationOptions("receipt.channel")[0]!="mutated snapshot");C("separate_receipt_linked_M2_not_gate",S.GetRelatedEvidenceIds("receipt.channel").SequenceEqual(new[]{"M2"})&&!S.HasRecordedObservation("receipt.channel",0));
            S.SetModal(FullCaseModal.Cipher);C("heard_channel_without_reading_cannot_send",!S.TrySendReply(FullInvestigationLetters.ReplyPhrases).Success&&!S.HasSentReply);
            Go("school");Ask("mai","mai.direction.theatre");C("Mai_heard_opens_theatre_without_record_or_lead",S.CanTravel("theatre")&&!S.HasEvidence("lead.theatre")&&!S.GetRecordedStatements().Any(h=>h.participantId=="npc.mai"));
            Go("theatre");View("henri");var lookup=S.GetHenriLookup();C("real_lookup_available",lookup.Length==26);
            Go("market");View("letter");S.SetModal(FullCaseModal.Cipher);foreach(var pair in lookup)Must(S.TryAssign(pair.pose,pair.letter));
            C("mapping_without_record_no_valid_reading",!S.TrySendReply(FullInvestigationLetters.ReplyPhrases).Success);Must(S.TryRecordReading("letter"));
            C("wrong_reply_not_sent",!S.TrySendReply(new[]{"WRONG"}).Success&&!S.HasSentReply);Must(S.TrySendReply(FullInvestigationLetters.ReplyPhrases));
            C("real_reading_and_hearing_send_without_record_gate",S.HasSentReply&&!S.HasRecordedObservation("ha",1)&&!S.HasRecordedObservation("receipt",0));
            Must(S.TryAssign("pose.01","Z"));Must(S.TryRecordReading("letter"));C("already_recorded_reading_does_not_require_resolve",S.TryGetRecordedReading("letter",out _));
            foreach(var pair in lookup)Must(S.TryAssign(pair.pose,pair.letter));
            Go("lake");a.Cancel();Must(S.TryBeginConversation("npc.bao"));C("Bao_Lan_topic_locked_before_reading",!S.TryAskTopic("bao.direction.lan",out var locked).Success&&locked.Answer==null&&!S.CanTravel("garden"));
            View("letter.bao");S.SetModal(FullCaseModal.Cipher);C("viewed_Bao_without_record_does_not_open",!S.CanTravel("garden"));Must(S.TryRecordReading("letter.bao"));
            C("decoded_Bao_does_not_auto_hear_or_open_garden",!S.CanTravel("garden"));Ask("bao","bao.direction.lan");
            C("hearing_Bao_opens_garden_without_M3_or_extra_record",S.CanTravel("garden")&&!S.HasEvidence("M3")&&!S.GetRecordedStatements().Any(h=>h.participantId=="npc.bao"));Go("garden");
            a.Cancel();snapshot=S.ExportStateJson();Key(KeyCode.F5);C("F5_saves_schema2_history_route",File.Exists(FullInvestigationRuntime.SavePathFor(a.Profile)),"UNITY_EDITOR_KEY_EVENT");Go("workshop");Key(KeyCode.F6);
            C("F6_restores_heard_not_recorded_and_route",S.ExportStateJson()==snapshot&&S.CanTravel("garden")&&!S.EndingPlaying,"UNITY_EDITOR_KEY_EVENT");
            Key(KeyCode.Escape);C("Esc_pauses_after_load",S.Paused&&!S.CanMove,"UNITY_EDITOR_KEY_EVENT");Key(KeyCode.Escape);C("Esc_resumes_after_load",S.CanMove,"UNITY_EDITOR_KEY_EVENT");
            Go("market");View("draft");S.SetModal(FullCaseModal.Inspect);Must(S.TryRecordSourcedObservation("draft.edge.observed"));
            View("letter");S.SetModal(FullCaseModal.Inspect);Must(S.TryRecordSourcedObservation("letter.symbols.compared"));C("observed_note_no_technical_pose_ID",!S.GetSourcedNotes().Any(n=>n.Text.Contains("pose ID")));
            Go("workshop");Must(S.TryCollect("khai"));Go("office");S.SetModal(FullCaseModal.Conversation);Must(S.TryRequestAccess("test.samples"));Go("homestay");View("samples");S.SetModal(FullCaseModal.Inspect);Must(S.TryRecordSourcedObservation("samples.received"));
            C("sample_receipt_observation_not_Phuong_statement",S.GetSourcedNotes().Single(n=>n.Id=="samples.received").Kind==NoteProvenanceKind.Observation);
            Go("office");Ask("phuong","phuong.samples.origin");Must(S.TryRecordStatement("phuong.samples.origin"));
            C("sample_origin_separate_Statement_from_receipt_Observation",S.GetSourcedNotes().Single(n=>n.Id=="phuong.samples.origin").Kind==NoteProvenanceKind.Statement&&S.GetSourcedNotes().Single(n=>n.Id=="samples.received").Kind==NoteProvenanceKind.Observation);
            Go("homestay");View("paper.khai",true);Go("market");View("letter",true);S.SetModal(FullCaseModal.Comparison);
            C("paper_wrong_Bao_pair_rejected",!S.TryCompare("paper",new[]{"letter.bao","paper.khai"},"test.feature.match").Success);Must(S.TryCompare("paper",new[]{"letter","paper.khai"},"test.feature.match"));
            C("paper_does_not_auto_conclude_or_finalize",!S.Finalized&&S.GetConclusions().All(d=>d.claim==""));
            Must(S.TryRecordRelation("letter","paper.khai","Giả thuyết người chơi, chưa xác nhận."));C("player_inference_stays_Hypothesis",S.GetSourcedNotes().Last().Kind==NoteProvenanceKind.Hypothesis);
            Go("workshop");S.SetModal(FullCaseModal.Reconstruction);Must(S.TryStartTrial("release"));S.Advance(S.TrialDuration+1);
            C("trial_report_observation_of_model_with_limits",S.GetSourcedNotes().Any(n=>n.Source=="Mô hình dựng lại"&&n.Kind==NoteProvenanceKind.Observation&&n.Conditions.Contains("release")&&n.Limitations.Contains("không phải sự kiện")));
            a.Cancel();S.SetModal(FullCaseModal.Conclusions);Must(S.TryConfirm());Must(S.TryFinalize(a.Profile));
            C("honest_open_case_finalizes_before_station",S.Finalized&&S.GetEnding().Ending==FullEnding.Open&&S.Location!="station");
            byte[] checkpoint=File.ReadAllBytes(FullInvestigationRuntime.CheckpointPathFor(a.Profile));Go("station");C("arrival_no_auto_summary_or_playback",!S.EndingSummaryOpen&&!S.EndingPlaying&&!S.EndingViewed);
            Must(S.TryOpenEndingSummary());C("summary_modal_locks_movement",S.EndingSummaryOpen&&!S.CanMove);a.Cancel();C("cancel_summary_not_viewed",!S.EndingViewed&&!S.EndingSummaryOpen&&S.CanMove);
            Must(S.TryOpenEndingSummary());Must(S.TryStartEndingPresentation());S.SetPaused(true);C("paused_presentation_cannot_finish",!S.TryCompleteEndingPresentation(a.Profile).Success&&!S.EndingViewed);S.SetPaused(false);Key(KeyCode.Escape);
            C("Esc_aborts_without_marking_viewed",!S.EndingViewed&&!S.EndingPlaying&&S.CanMove,"UNITY_EDITOR_KEY_EVENT");
            Must(S.TryOpenEndingSummary());Must(S.TryStartEndingPresentation());
            string temp=FullInvestigationRuntime.SavePathFor(a.Profile)+".tmp";Directory.CreateDirectory(temp);
            byte[] oldMain=File.ReadAllBytes(FullInvestigationRuntime.SavePathFor(a.Profile));snapshot=S.ExportStateJson();
            try{C("completion_IO_failure_preserves_state_main_and_viewed",!S.TryCompleteEndingPresentation(a.Profile).Success&&snapshot==S.ExportStateJson()&&!S.EndingViewed&&oldMain.SequenceEqual(File.ReadAllBytes(FullInvestigationRuntime.SavePathFor(a.Profile))),"API_IO_SIMULATED_TEMP_PATH_DIRECTORY");}finally{Directory.Delete(temp);}
            Must(S.TryCompleteEndingPresentation(a.Profile));C("complete_persists_viewed",S.EndingViewed&&!S.EndingPlaying);byte[] main=File.ReadAllBytes(FullInvestigationRuntime.SavePathFor(a.Profile));
            Must(S.TryOpenEndingSummary());Must(S.TryStartEndingPresentation());Must(S.TryCompleteEndingPresentation(a.Profile));
            C("replay_no_save_checkpoint_or_reward_mutation",main.SequenceEqual(File.ReadAllBytes(FullInvestigationRuntime.SavePathFor(a.Profile)))&&checkpoint.SequenceEqual(File.ReadAllBytes(FullInvestigationRuntime.CheckpointPathFor(a.Profile))));
            Key(KeyCode.F6);C("load_viewed_not_auto_replayed",S.EndingViewed&&!S.EndingPlaying,"UNITY_EDITOR_KEY_EVENT");
            Must(S.TryReturnToInvestigation(a.Profile));C("explicit_checkpoint_restores_investigation_without_rewriting_main",!S.Finalized&&!S.EndingViewed&&S.HasSentReply&&main.SequenceEqual(File.ReadAllBytes(FullInvestigationRuntime.SavePathFor(a.Profile))));
            // Real actions, no history/progress setters; missing-reply branch is separate fresh run.
            var noReply=new FullInvestigationRuntime(FullCaseTestCatalog.CreateApproved());string separate=Path.Combine(output,"no-reply-profile");noReply.SetModal(FullCaseModal.Conclusions);Must(noReply.TryConfirm());Must(noReply.TryFinalize(separate));noReply.Cancel();Must(noReply.TryTravel("station"));
            C("missing_reply_blocks_appointment_not_travel_back",!noReply.TryOpenEndingSummary().Success&&!noReply.EndingPlaying&&noReply.CanTravel("market"));Must(noReply.TryTravel("market"));
            C("missing_reply_no_auto_send",!noReply.HasSentReply&&noReply.AppointmentRequirement.Contains("Hà"));
        }
        static void Migration()
        {
            string profile=Arg("--approved-legacy-profile");byte[] bytes=File.ReadAllBytes(FullInvestigationRuntime.SavePathFor(profile));var legacy=new FullInvestigationRuntime(FullCaseTestCatalog.Create());Must(legacy.TryLoad(profile));
            var json=JObject.Parse(legacy.ExportStateJson());C("schema1_migration_explicit_empty_new_state",json.Value<int>("schema")==2&&legacy.GetHeardStatements().Length==0&&legacy.GetRecordedStatements().Length==0&&!legacy.EndingViewed&&!legacy.CanBeginAppointment,"API_LEGACY_SAVE");
            C("legacy_raw_alias_not_promoted",legacy.HasEvidence("decoded")&&legacy.GetRecordedReadings().Length==0,"API_LEGACY_SAVE");
            C("legacy_load_read_only",bytes.SequenceEqual(File.ReadAllBytes(FullInvestigationRuntime.SavePathFor(profile))),"API_LEGACY_SAVE");
            string p=Path.Combine(output,"migrated-profile");Must(legacy.TrySave(p));Must(legacy.TryLoad(p));C("migrated_save_round_trip",legacy.HasEvidence("decoded")&&legacy.GetHeardStatements().Length==0,"API_LEGACY_SAVE");
        }
        static void LegacyTrialProjection()
        {
            var go=new GameObject("TEST ONLY legacy report projection");
            try{var session=go.AddComponent<InvestigationSession>();session.data=new InvestigationSave();session.data.trials.Add(new TrialRecord{id="hook",report="TEST ONLY legacy report",returnSeconds=-1});
                string before=JsonUtility.ToJson(session.data);var note=session.GetProvenanceNotes().Single();
                C("r107_report_classification_source_conditions_limits_no_save_mutation",note.Kind==NoteProvenanceKind.Observation&&note.Source=="Mô hình dựng lại"&&note.Conditions.Contains("hook")&&note.Limitations.Contains("không phải sự kiện")&&before==JsonUtility.ToJson(session.data),"API_LEGACY_REPORT_FIXTURE");
            }finally{UnityEngine.Object.DestroyImmediate(go);}
        }
        static void MissingSourceAndLegacyNotes()
        {
            var catalog=FullCaseTestCatalog.CreateApproved();catalog.evidence=catalog.evidence.Where(e=>e.id!="samples").ToArray();
            var runtime=new FullInvestigationRuntime(catalog);runtime.SetModal(FullCaseModal.Inspect);
            C("catalog_missing_sample_source_safe_rejection",!runtime.TryRecordSourcedObservation("samples.received").Success&&runtime.GetSourcedNotes().Length==0,"API_MISSING_DATA");
            runtime.SetModal(FullCaseModal.Conclusions);Must(runtime.TryConfirm());string profile=Path.Combine(output,"viewed-without-reply");Must(runtime.TryFinalize(profile));
            string path=FullInvestigationRuntime.SavePathFor(profile);var json=JObject.Parse(File.ReadAllText(path));json["endingViewed"]=true;File.WriteAllText(path,json.ToString());string before=runtime.ExportStateJson();
            C("viewed_without_reply_even_if_finalized_rejected",!runtime.TryLoad(profile).Success&&before==runtime.ExportStateJson(),"API_MALFORMED_SAVE_FIXTURE");
            var go=new GameObject("TEST ONLY old mixed source notes");
            try{var session=go.AddComponent<InvestigationSession>();session.data=new InvestigationSave();
                foreach(var item in new[]{new[]{"mechanism","1"},new[]{"hook","0"},new[]{"samples","0"},new[]{"letter","1"},new[]{"draft","0"},new[]{"decoded","0"}})session.data.evidence.Add(new EvidenceRecord{id=item[0],observations=new System.Collections.Generic.List<int>{int.Parse(item[1])}});
                before=JsonUtility.ToJson(session.data);var notes=session.GetProvenanceNotes();
                C("six_legacy_entries_not_reinterpreted_as_heard_observed_or_proved",notes.Length==6&&notes.All(n=>n.Kind==NoteProvenanceKind.Unclassified&&n.Status==NoteProvenanceStatus.LegacySourceUnresolved)&&before==JsonUtility.ToJson(session.data),"API_LEGACY_NOTES_FIXTURE");
            }finally{UnityEngine.Object.DestroyImmediate(go);}
        }
        static void EndingFixtures()
        {
            foreach(string dimension in new[]{"sender","motive","fall","sender+motive"}){
                var runtime=new FullInvestigationRuntime(FullCaseTestCatalog.CreateApproved(true));runtime.SetModal(FullCaseModal.Conclusions);
                foreach(string id in dimension.Split('+'))Must(runtime.TrySetConclusion(id,"test.alternative-claim",new FullRequirement[0]));
                Must(runtime.TryConfirm());string profile=Path.Combine(output,"ending-fixture-"+dimension.Replace('+','_'));Must(runtime.TryFinalize(profile));runtime.Cancel();
                var view=runtime.GetEnding();var lines=runtime.GetEndingConsequences();
                C("unsupported_"+dimension+"_combined_consequences",view.Ending==FullEnding.Unsupported&&view.UnsupportedDimensions.Length==dimension.Split('+').Length&&lines.Length==dimension.Split('+').Length+2&&lines.Any(x=>x.Contains("Lan")),"API_TEST_ONLY_RUBRIC_FIXTURE_NOT_OFFICIAL_ENDING");
            }
        }
        static void Validation()
        {
            string p=Path.Combine(output,"validation-profile"),path=FullInvestigationRuntime.SavePathFor(p);Directory.CreateDirectory(Path.GetDirectoryName(path));
            var original=JObject.Parse(S.ExportStateJson());string live=S.ExportStateJson();
            Action<string,Action<JObject>> reject=(name,change)=>{var j=(JObject)original.DeepClone();change(j);File.WriteAllText(path,j.ToString());var bytes=File.ReadAllBytes(path);var result=S.TryLoad(p);C(name,!result.Success&&result.Code=="validation"&&S.ExportStateJson()==live&&bytes.SequenceEqual(File.ReadAllBytes(path)),"API_MALFORMED_SAVE_FIXTURE");};
            reject("existing_evidence_null_guard_retained",j=>j["evidence"]=JValue.CreateNull());
            reject("existing_duplicate_evidence_guard_retained",j=>((JArray)j["evidence"]).Add(j["evidence"][0].DeepClone()));
            reject("existing_relation_reference_guard_retained",j=>((JArray)j["relations"]).Add(new JObject{["first"]="unknown",["second"]="letter",["note"]="invalid"}));
            reject("null_history_rejected",j=>j["heardStatements"]=JValue.CreateNull());reject("missing_history_rejected",j=>j.Remove("heardStatements"));
            reject("null_statement_element_rejected",j=>((JArray)j["heardStatements"]).Add(JValue.CreateNull()));
            reject("duplicate_statement_rejected",j=>((JArray)j["heardStatements"]).Add(j["heardStatements"][0].DeepClone()));
            reject("unknown_statement_rejected",j=>j["heardStatements"][0]["id"]="not.official");reject("snapshot_wrong_field_type_rejected",j=>j["heardStatements"][0]["answer"]=false);reject("wrong_topic_reference_rejected",j=>j["heardStatements"][0]["topicId"]="wrong.topic");
            reject("recorded_unheard_rejected",j=>((JArray)j["recordedStatements"]).Add("unheard.id"));reject("unknown_sourced_note_rejected",j=>((JArray)j["sourcedNotes"]).Add("unknown.note"));
            reject("unfinalized_viewed_rejected",j=>j["endingViewed"]=true);reject("verified_reply_without_history_rejected",j=>j["heardStatements"]=new JArray());
            reject("new_bool_missing_rejected",j=>j.Remove("endingViewed"));reject("new_schema_unknown_rejected",j=>j["schema"]=99);
            File.WriteAllText(path,original.ToString());Must(S.TryLoad(p));C("valid_save_after_rejections_recovers",S.ExportStateJson()==live,"API_MALFORMED_SAVE_FIXTURE");
            // Snapshot revision compatibility fixture: not official dialogue, not a natural route.
            var revised=(JObject)original.DeepClone();revised["heardStatements"][0]["answer"]="TEST ONLY archived snapshot";File.WriteAllText(path,revised.ToString());Must(S.TryLoad(p));C("archived_snapshot_not_rewritten_from_current_script",S.GetHeardStatements()[0].answer=="TEST ONLY archived snapshot","API_HISTORY_SNAPSHOT_FIXTURE");
            Must(S.TrySave(Path.Combine(output,"archived-roundtrip")));File.WriteAllText(path,original.ToString());Must(S.TryLoad(p));
        }
    }
}
