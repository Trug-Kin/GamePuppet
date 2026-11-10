using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using ConRoi.Investigation.V36.R107;

namespace ConRoi.Gameplay.EditorChecks
{
    public static class FullCaseChecks
    {
        static JArray rows;static int failures;static string output,profile;
        static FullCaseCatalog c;static FullInvestigationRuntime s;
        static string Arg(string id){var a=Environment.GetCommandLineArgs();int i=Array.IndexOf(a,id);return i>=0&&i+1<a.Length?a[i+1]:"";}
        static void Check(string name,bool pass){rows.Add(new JObject{["name"]=name,["pass"]=pass});if(!pass)failures++;}
        static void Must(FullActionResult r){if(!r.Success)throw new InvalidOperationException("Route action failed: "+r.Code+": "+r.Message);}
        static bool Throws(Action a){try{a();return false;}catch{return true;}}
        static void New(bool reviewed=true){c=FullCaseTestCatalog.Create(reviewed);s=new FullInvestigationRuntime(c);}
        static void Go(string location){s.Cancel();Must(s.TryTravel(location));}
        static void Read(string id){s.Cancel();Must(s.TryCollect(id));Must(s.TryInspect(id));if(id=="clip"){s.SetModal(FullCaseModal.Media);Must(s.TryPlayMedia());for(int i=0;i<130;i++)s.Advance(.1f);Must(s.TryInspect(id));}var d=c.evidence.First(e=>e.id==id);for(int i=0;i<d.observations.Length;i++)Must(s.TryRecordObservation(id,i));s.Cancel();}
        static void ReadLocal(){foreach(var d in s.GetLocalEvidence())Read(d.id);}
        static void SolveLetters(){s.SetModal(FullCaseModal.Cipher);for(int i=0;i<26;i++)Must(s.TryAssign(FullInvestigationLetters.PoseKey((char)('A'+i)),((char)('A'+i)).ToString()));foreach(var id in FullInvestigationLetters.Ids)Must(s.TryRecordReading(id));s.Cancel();}
        static void RunTrial(string id,bool clear=false){s.SetModal(FullCaseModal.Reconstruction);Must(s.TryStartTrial(id));if(clear){Must(s.TrySeekTrial(TrialTimeline.LastAction(id)+.1f));Must(s.TryClearHolds());}for(int i=0;i<300&&s.TrialRunning;i++)s.Advance(.25f);s.Cancel();}
        static void PlayRoute(bool reverse=false){
            New();Go("workshop");ReadLocal();
            foreach(var location in reverse?new[]{"slope","school","market","lake"}:new[]{"lake","market","school","slope"}){Go(location);ReadLocal();}
            Go("office");s.SetModal(FullCaseModal.Conversation);Must(s.TryRequestAccess("test.samples"));Go("theatre");ReadLocal();Go("homestay");ReadLocal();Go("garden");ReadLocal();
            SolveLetters();RunTrial("release");RunTrial("help");RunTrial("hook",true);
            s.SetModal(FullCaseModal.Comparison);foreach(var task in c.comparisons)Must(s.TryCompare(task.id,task.evidenceIds,task.expectedOption));s.Cancel();
        }
        static void SetDrafts(string open=null,string wrong=null,string unsupported=null,bool alternative=false){
            s.SetModal(FullCaseModal.Conclusions);
            foreach(var rule in c.conclusions){var set=alternative&&rule.id=="sender"?rule.alternatives[1]:rule.alternatives[0];Must(s.TrySetConclusion(rule.id,rule.id==open?"":rule.id==wrong?"test.alternative-claim":"test.gdd-claim",rule.id==unsupported?Array.Empty<FullRequirement>():set.requires));}
        }
        public static void RunBatch(){
            output=Arg("--full-output");profile=Arg("--investigation-save-dir");rows=new JArray();failures=0;
            try{
                if(string.IsNullOrWhiteSpace(output)||string.IsNullOrWhiteSpace(profile)||Directory.Exists(output)||Directory.Exists(profile))throw new InvalidOperationException("Use fresh explicit output/profile");
                Directory.CreateDirectory(output);Directory.CreateDirectory(profile);
                EditorSceneManager.OpenScene("Assets/ConRoi/Modules/Gameplay/Scenes/Gameplay_ThuNghiem.unity");
                Boundaries();RouteAndEndings();Persistence();CatalogAndSnapshots();ProxyAndLookup();
            }catch(Exception e){rows.Add(new JObject{["exception"]=e.ToString(),["pass"]=false});failures++;}
            if(!string.IsNullOrWhiteSpace(output)){Directory.CreateDirectory(output);File.WriteAllText(Path.Combine(output,"results.json"),new JObject{["source"]=Arg("--full-source"),["unity"]=Application.unityVersion,["kind"]="EDITMODE_API_TEST_ONLY_CATALOG_ACTION_ROUTE_NOT_WINDOWS_NOT_NATURAL_PLAY_NOT_INTEGRATION",["failures"]=failures,["checks"]=rows}.ToString());}
            Debug.Log("Full case checks: "+rows.Count+", failures "+failures);EditorApplication.Exit(failures==0?0:1);
        }
        static void Boundaries(){
            New();string empty=s.ExportStateJson();
            Check("unknown_travel_and_gated_location_preserve",!s.TryTravel("unknown").Success&&!s.TryTravel("homestay").Success&&s.ExportStateJson()==empty);
            Check("uncollected_letter_notes_hidden",!s.GetLetter("letter.lan").HasValue&&s.GetLetterPoses("letter.lan").Length==0&&s.GetHints("letter.lan",3).Length==0&&s.GetDescription("letter.lan")=="");
            s.SetModal(FullCaseModal.Conversation);Check("sample_permission_before_Khai_denied",!s.TryRequestAccess("test.samples").Success);s.Cancel();
            s.SetPaused(true);Check("paused_actions_blocked",!s.TryTravel("workshop").Success&&!s.TryCollect("slide").Success&&!s.CanMove&&s.ExportStateJson()==empty);s.SetPaused(false);
            Go("workshop");Must(s.TryCollect("slide"));Check("collect_is_not_record",s.GetRecords().Length==1&&s.GetRecordedNotes("slide").Length==0&&!s.CanTravel("lake"));
            Check("record_requires_inspection",!s.TryRecordObservation("slide",0).Success);Must(s.TryInspect("slide"));Check("inspect_locks_movement",!s.CanMove&&s.GetRecordedNotes("slide").Length==0);Must(s.TryRecordObservation("slide",0));Must(s.TryRecordObservation("slide",0));Check("record_repeat_unique_unlocks",s.GetRecords()[0].observations.Count==1&&s.CanTravel("lake"));Check("invalid_observation_denied",!s.TryRecordObservation("slide",-1).Success&&!s.TryRecordObservation("slide",99).Success);s.Cancel();Check("cancel_restores_input",s.CanMove);
            s.SetModal(FullCaseModal.Comparison);empty=s.ExportStateJson();Check("modal_travel_denied",!s.TryTravel("lake").Success&&s.ExportStateJson()==empty);s.Cancel();
            Must(s.TryCollect("clip"));Must(s.TryInspect("clip"));Check("clip_unseen_record_denied",!s.TryRecordObservation("clip",0).Success&&s.GetRecordedNotes("clip").Length==0);s.SetModal(FullCaseModal.Media);Must(s.TrySeekMedia(12));Must(s.TryInspect("clip"));Check("seek_does_not_expose",!s.TryRecordObservation("clip",0).Success);s.SetModal(FullCaseModal.Media);Must(s.TrySeekMedia(0));Must(s.TryPlayMedia());s.Advance(1.2f);s.Cancel();float t=s.MediaTime;s.Advance(20);Check("media_away_does_not_run",s.MediaTime==t);Must(s.TryInspect("clip"));Check("one_window_is_not_enough",!s.TryRecordObservation("clip",0).Success);s.SetModal(FullCaseModal.Media);s.Advance(6);Must(s.TryInspect("clip"));Check("visible_media_unlocks_record",s.TryRecordObservation("clip",0).Success);s.Cancel();
            Must(s.TryCollect("phone"));Must(s.TryHold("phone"));empty=s.ExportStateJson();Check("invalid_place_keeps_item",!s.TryPlace("workshop.outside-map").Success&&s.HeldEvidenceId=="phone"&&s.ExportStateJson()==empty);Must(s.TryStow());Must(s.TryHold("phone"));Must(s.TryPlace("workshop.table"));Check("valid_place_keeps_single_record",s.HeldEvidenceId==""&&s.GetRecords().Count(r=>r.id=="phone")==1);Check("fixed_evidence_cannot_hold",!s.TryHold("slide").Success);
            s.SetModal(FullCaseModal.Cipher);empty=s.ExportStateJson();Check("bad_mapping_preserves_state",!s.TryAssign(null,"A").Success&&!s.TryAssign("pose.01",null).Success&&!s.TryAssign("pose.01","AB").Success&&!s.TryAssign("pose.01","1").Success&&s.ExportStateJson()==empty);Check("unreceived_cannot_read_reply",!s.TryRecordReading("letter").Success&&!s.TrySendReply(FullInvestigationLetters.ReplyPhrases).Success);
            s.Cancel();Go("market");Read("ha");Read("letter");s.SetModal(FullCaseModal.Cipher);Check("blank_reading_not_recorded",!s.TryRecordReading("letter").Success&&!s.GetLetter("letter").Value.Recorded);Must(s.TryAssign("pose.01","Z"));Check("wrong_partial_not_recorded",!s.TryRecordReading("letter").Success);Check("reply_wrong_order_denied",!s.TrySendReply(FullInvestigationLetters.ReplyPhrases.Reverse().ToArray()).Success);Check("known_channel_correct_reply",s.TrySendReply(FullInvestigationLetters.ReplyPhrases).Success);empty=s.ExportStateJson();Must(s.TrySendReply(FullInvestigationLetters.ReplyPhrases));Check("reply_repeat_no_duplicate",s.ExportStateJson()==empty);
            var interview=InvestigationInterview.ForSource(s,FullCaseTestCatalog.Topics());MustBegin(interview,"test.full.actor.mai");Check("locked_topic_hides_question_answer",interview.GetAvailableTopics().Length==0&&!interview.TryAsk("test.full.topic.mai",out var reply,out _)&&reply.Answer==null);interview.EndInterview();
            Check("no_ending_before_finalize",s.GetEnding().Ending==FullEnding.None);
        }
        static void MustBegin(InvestigationInterview i,string participant){if(!i.TryBeginInterview(participant,out var error))throw new InvalidOperationException(error);}
        static void RouteAndEndings(){
            PlayRoute();Check("all_ten_locations_without_bypass",s.GetLocations().Length==10&&!s.CanTravel("station"));Check("five_letters_decoded",FullInvestigationLetters.Ids.All(id=>s.GetLetter(id).Value.Recorded));
            var poses=FullInvestigationLetters.PoseSequence("letter.lan");Check("26_pose_keys_shared_and_word_separator",Enumerable.Range(0,26).Select(i=>FullInvestigationLetters.PoseKey((char)('A'+i))).Distinct().Count()==26&&poses.All(p=>p=="word-break"||Enumerable.Range(0,26).Any(i=>p==FullInvestigationLetters.PoseKey((char)('A'+i)))));
            var i=InvestigationInterview.ForSource(s,FullCaseTestCatalog.Topics());int answers=0;Action<InterviewReply> listener=r=>answers++;i.Answered+=listener;
            foreach(var actor in new[]{"bao","ha","mai","tung","lan","khai","phuong","host"}){MustBegin(i,"test.full.actor."+actor);string before=s.ExportStateJson();Check(actor+"_ask_repeat_no_reward",i.TryAsk("test.full.topic."+actor,out _,out _)&&i.TryAsk("test.full.topic."+actor,out _,out _)&&s.ExportStateJson()==before);i.EndInterview();}
            Check("interview_answer_event_repeat_count",answers==16);i.Answered-=listener;
            string goodProfile=Path.Combine(profile,"route");Must(s.TrySave(goodProfile));
            foreach(var variant in new[]{"complete","open","wrong-sender","wrong-motive","wrong-fall","unsupported","alternative-set"}){
                Must(s.TryLoad(goodProfile));SetDrafts(variant=="open"?"motive":null,variant.StartsWith("wrong-")?variant.Substring(6):null,variant=="unsupported"?"presence":null,variant=="alternative-set");Check(variant+"_neutral_before_confirm",s.GetEnding().Ending==FullEnding.None);Must(s.TryConfirm());Check(variant+"_confirmed",s.Confirmed);Must(s.TryFinalize(Path.Combine(profile,variant)));var ending=s.GetEnding();
                Check(variant+"_priority",ending.Ending==(variant=="open"?FullEnding.Open:variant.StartsWith("wrong-")||variant=="unsupported"?FullEnding.Unsupported:FullEnding.Complete));Check(variant+"_fall_accusation_scope",ending.FallAccusationUnsupported==(variant=="wrong-fall"));
                Check(variant+"_station_available_after_finalize",s.CanTravel("station"));Go("station");Check(variant+"_station_route",s.Location=="station");
                Must(s.TryLoad(Path.Combine(profile,variant),true));Check(variant+"_checkpoint_restores_draft",!s.Finalized&&s.GetEnding().Ending==FullEnding.None&&!s.Confirmed&&s.TrialId=="");
            }
            PlayRoute(true);Check("alternative_lead_order_works",s.GetRecords().Any(r=>r.id=="M6")&&s.Meets(FullCaseTestCatalog.R(FullRequirementKind.Comparison,"cargo")));
            SetDrafts();Must(s.TryConfirm());var first=s.GetConclusions()[0];Must(s.TrySetConclusion(first.id,"",first.grounds.ToArray()));Check("draft_edit_invalidates_confirmation",!s.Confirmed&&!s.TryFinalize(Path.Combine(profile,"edit-denied")).Success);
            New(false);s.SetModal(FullCaseModal.Conclusions);Must(s.TrySetConclusion("sender","test.gdd-claim",Array.Empty<FullRequirement>()));Must(s.TryConfirm());Check("unreviewed_content_fails_closed",!s.TryFinalize(Path.Combine(profile,"pending")).Success&&s.LastMessage.Contains("review")&&!s.Finalized);
            New(false);s.SetModal(FullCaseModal.Conclusions);Must(s.TryConfirm());Must(s.TryFinalize(Path.Combine(profile,"open-with-pending-content")));Check("honest_all_open_does_not_require_positive_proof_rubric",s.GetEnding().Ending==FullEnding.Open);
            New();Must(s.TryLoad(goodProfile));s.SetModal(FullCaseModal.Comparison);var paper=c.comparisons.First(t=>t.id=="paper");Must(s.TryCompare("paper",paper.evidenceIds,"test.feature.near"));s.SetModal(FullCaseModal.Conclusions);var citation=new[]{FullCaseTestCatalog.R(FullRequirementKind.Comparison,"paper")};Check("wrong_comparison_citable_without_premature_verdict",s.TrySetConclusion("sender","test.gdd-claim",citation).Success&&s.GetAvailableGrounds().Any(g=>g.kind==FullRequirementKind.Comparison&&g.id=="paper")&&s.GetEnding().Ending==FullEnding.None);
            New();s.SetModal(FullCaseModal.Reconstruction);foreach(string id in new[]{"release","help","hold","hook","wedge","after"}){
                Must(s.TryStartTrial(id));s.Advance(.5f);float at=s.TrialTime;s.Cancel();s.Advance(100);Check(id+"_away_pauses_context",s.TrialTime==at&&s.TrialRunning);s.SetModal(FullCaseModal.Reconstruction);s.SetTrialRunning(false);s.Cancel();s.SetModal(FullCaseModal.Reconstruction);s.Advance(1);Check(id+"_manual_pause_stays",s.TrialTime==at&&!s.TrialRunning);s.SetTrialRunning(true);s.SetPaused(true);s.Advance(1);Check(id+"_global_pause",s.TrialTime==at);s.SetPaused(false);s.Advance(.1f);Check(id+"_return_no_catchup",s.TrialTime>at&&s.TrialTime<at+.11f);
                int count=JObject.Parse(s.ExportStateJson())["trials"].Count();Must(s.TrySeekTrial(s.TrialDuration));Check(id+"_seek_no_record",JObject.Parse(s.ExportStateJson())["trials"].Count()==count);Must(s.TryStartTrial(id));for(int k=0;k<300&&s.TrialRunning;k++)s.Advance(.25f);s.Advance(.1f);Check(id+"_complete_exactly_one",JObject.Parse(s.ExportStateJson())["trials"].Count()==count+1);
            }
        }
        static void Persistence(){
            PlayRoute();SetDrafts();string p=Path.Combine(profile,"persist");Must(s.TrySave(p));byte[] good=File.ReadAllBytes(FullInvestigationRuntime.SavePathFor(p));string before=s.ExportStateJson();
            foreach(string fault in new[]{"null-evidence","missing-evidence","null-record","bad-index","bad-relation","bad-catalog","null-ground","bad-schema","duplicate","bad-json"}){
                var json=JObject.Parse(System.Text.Encoding.UTF8.GetString(good));string malformed;
                switch(fault){case "null-evidence":json["evidence"]=null;break;case "missing-evidence":json.Remove("evidence");break;case "null-record":((JArray)json["evidence"]).Add(JValue.CreateNull());break;case "bad-index":((JArray)json["evidence"][0]["observations"]).Add(999);break;case "bad-relation":((JArray)json["relations"]).Add(new JObject{["first"]="absent",["second"]="slide",["note"]="test"});break;case "bad-catalog":json["catalogHash"]="wrong";break;case "null-ground":((JArray)json["conclusions"][0]["grounds"]).Add(JValue.CreateNull());break;case "bad-schema":json["schema"]=999;break;}
                malformed=fault=="bad-json"?"{":fault=="duplicate"?"{\"schema\":1,"+json.ToString().Substring(1):json.ToString();string path=FullInvestigationRuntime.SavePathFor(p);File.WriteAllText(path,malformed);byte[] bad=File.ReadAllBytes(path);s.SetModal(FullCaseModal.Conversation);s.SetPaused(true);bool paused=s.Paused;var modal=s.Modal;
                Check(fault+"_load_rejected_full_state_bytes_context",!s.TryLoad(p).Success&&s.ExportStateJson()==before&&bad.SequenceEqual(File.ReadAllBytes(path))&&s.Paused==paused&&s.Modal==modal);
                File.WriteAllBytes(path,good);Must(s.TryLoad(p));Check(fault+"_valid_recovery",s.ExportStateJson()==before&&!s.Paused&&s.Modal==FullCaseModal.None);
            }
            s.SetModal(FullCaseModal.Conclusions);Must(s.TryConfirm());string signature=s.ExportStateJson();
            using(var held=new FileStream(FullInvestigationRuntime.SavePathFor(p),FileMode.Open,FileAccess.Read,FileShare.None)){Check("destination_locked_save_rejected",!s.TrySave(p).Success);Check("finalize_locked_destination_rejected",!s.TryFinalize(p).Success);}
            Check("failed_write_live_main_preserved",s.ExportStateJson()==signature&&!s.Finalized&&good.SequenceEqual(File.ReadAllBytes(FullInvestigationRuntime.SavePathFor(p))));Must(s.TryFinalize(p));Check("write_recovery_finalizes",s.Finalized);Must(s.TryLoad(p,true));Check("recovery_checkpoint_loads",!s.Finalized);Must(s.TrySave(p));Must(s.TryLoad(p));
            var interview=InvestigationInterview.ForSource(s,FullCaseTestCatalog.Topics());MustBegin(interview,"test.full.actor.mai");Must(s.TryLoad(p));Check("full_load_expires_interview_context",!interview.TryAsk("test.full.topic.mai",out _,out _)&&interview.ActiveParticipantId=="");
            string legacy=Path.Combine(profile,"legacy-copy.json");var old=new InvestigationSave();old.evidence.Add(new EvidenceRecord{id="slide",collected=true,observations=new List<int>{0}});old.mapping.Add(new CharacterAssignment{pose="A",letter="B"});File.WriteAllText(legacy,JsonUtility.ToJson(old,true));byte[] oldBytes=File.ReadAllBytes(legacy);New();Check("explicit_schema1_import",s.TryImportPrototype(legacy).Success&&s.HasRecordedObservation("slide",0));Check("import_does_not_guess_new_progress",s.Location=="office"&&!s.HasPermission("test.samples")&&!s.GetLetter("letter").HasValue&&!s.Finalized);Check("import_keeps_old_file_bytes",oldBytes.SequenceEqual(File.ReadAllBytes(legacy)));Check("import_nonempty_denied",!s.TryImportPrototype(legacy).Success);Must(s.TrySave(Path.Combine(profile,"imported")));Must(s.TryLoad(Path.Combine(profile,"imported")));Check("imported_save_continues",s.HasRecordedObservation("slide",0));
        }
        static void ProxyAndLookup(){
            New();Check("book_lookup_not_available_at_start",s.GetHenriLookup().Length==0&&s.GetNoteSource("unknown")=="");
            PlayRoute();string before=s.ExportStateJson();var lookup=s.GetHenriLookup();Check("book_has_shared_26_keys_without_mutation",lookup.Length==26&&lookup.All(a=>a.pose==FullInvestigationLetters.PoseKey(a.letter[0]))&&before==s.ExportStateJson());lookup[0].letter="Z";Check("book_snapshot_isolated",s.GetHenriLookup()[0].letter=="A");
            string proxyProfile=Path.Combine(profile,"proxy");
            using(var a=new FullCaseSandboxController(proxyProfile)){
                var rt=a.Runtime;
                Must(a.Interact(FullPlaceholderKind.Location,"workshop","workshop"));Must(a.Interact(FullPlaceholderKind.Evidence,"slide","workshop"));Must(rt.TryInspect("slide"));Must(rt.TryRecordObservation("slide",0));a.Cancel();
                string snapshot=rt.ExportStateJson();Must(a.Save());Must(a.Interact(FullPlaceholderKind.Location,"market","market"));Must(a.Load());Check("proxy_adapter_save_load_restores_route",rt.ExportStateJson()==snapshot&&rt.Location=="workshop"&&rt.CanMove);
                foreach(FullCaseModal modal in Enum.GetValues(typeof(FullCaseModal))){rt.SetModal(modal);Check("proxy_modal_input_"+modal,rt.CanMove==(modal==FullCaseModal.None));a.Cancel();Check("proxy_cancel_"+modal,rt.CanMove);}
                rt.SetPaused(true);snapshot=rt.ExportStateJson();Check("proxy_paused_rejects_interact",!a.Interact(FullPlaceholderKind.Location,"market","market").Success&&rt.ExportStateJson()==snapshot&&!rt.CanMove);rt.SetPaused(false);
                Check("proxy_foreign_location_item_rejected",!a.Interact(FullPlaceholderKind.Evidence,"letter","market").Success&&!rt.HasEvidence("letter"));
                Must(rt.TryCollect("clip"));rt.SetModal(FullCaseModal.Media);Must(rt.TryPlayMedia());a.Tick(1,true);float t=rt.MediaTime;a.Tick(10,false);Check("proxy_unfocused_tick_not_accumulated",rt.MediaTime==t);a.Tick(.1f,true);Check("proxy_return_tick_only",rt.MediaTime>t&&rt.MediaTime<t+.11f);
                a.Cancel();Must(a.Interact(FullPlaceholderKind.Location,"market","market"));Must(rt.TryCollect("ha"));Must(rt.TryInspect("ha"));Must(rt.TryRecordObservation("ha",0));a.Cancel();Must(a.Interact(FullPlaceholderKind.Actor,"test.full.actor.ha","market"));Must(a.Ask("test.full.topic.ha"));Check("proxy_topic_answer_only",a.Reply.StartsWith("TEST ONLY")&&!rt.HasEvidence("letter"));a.Cancel();Check("proxy_cancel_clears_conversation",a.Reply==""&&a.Interview.ActiveParticipantId==""&&rt.CanMove);Must(a.Load());Check("proxy_load_clears_reply",a.Reply==""&&a.Interview.ActiveParticipantId=="");
            }
            PlayRoute();SetDrafts();Must(s.TryConfirm());Must(s.TryFinalize(Path.Combine(profile,"immutable-final")));before=s.ExportStateJson();s.SetModal(FullCaseModal.Reconstruction);s.SetTrialRunning(true);s.Advance(100);Check("finalized_replay_cannot_mutate_saved_progress",before==s.ExportStateJson()&&!s.TrialRunning);
        }
        static void CatalogAndSnapshots(){
            New();var catalog=FullCaseTestCatalog.Create();var runtime=new FullInvestigationRuntime(catalog);catalog.locations[0].label="mutated";Check("catalog_copy_isolated",runtime.GetLocations()[0].label!="mutated");var locations=runtime.GetLocations();locations[0].label="changed";Check("query_copy_isolated",runtime.GetLocations()[0].label!="changed");
            catalog=FullCaseTestCatalog.Create();catalog.evidence[1].id=catalog.evidence[0].id;Check("duplicate_catalog_id_rejected",Throws(()=>new FullInvestigationRuntime(catalog)));catalog=FullCaseTestCatalog.Create();catalog.locations[0].requires=new[]{FullCaseTestCatalog.Note("unknown")};Check("unknown_requirement_rejected",Throws(()=>new FullInvestigationRuntime(catalog)));catalog=FullCaseTestCatalog.Create();catalog.comparisons[0].requires=new[]{FullCaseTestCatalog.R(FullRequirementKind.Comparison,"paper")};Check("recursive_requirement_rejected",Throws(()=>new FullInvestigationRuntime(catalog)));
            New();bool rejected=false;Action nested=()=>{rejected=!s.TryTravel("workshop").Success;};Action bad=()=>throw new InvalidOperationException("TEST ONLY callback");s.Changed+=nested;s.Changed+=bad;s.SetModal(FullCaseModal.Comparison);Check("callback_isolation_and_no_reentry",rejected&&s.Location=="office");s.Changed-=nested;s.Changed-=bad;
            var before=s.ExportStateJson();s.Advance(float.NaN);s.Advance(float.PositiveInfinity);s.Advance(-1);Check("nonfinite_delta_no_state_change",before==s.ExportStateJson());
        }
    }
}
