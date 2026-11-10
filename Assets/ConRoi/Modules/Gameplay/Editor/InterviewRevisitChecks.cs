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
    [InitializeOnLoad]
    public static class InterviewRevisitChecks
    {
        const string Key="ConRoi.InterviewRevisitChecks", Scene="Assets/ConRoi/Modules/Gameplay/Scenes/Gameplay_ThuNghiem.unity";
        public const string TestOnlyParticipantA="test.only.interview.a", TestOnlyParticipantB="test.only.interview.b";
        // TEST ONLY. This Editor fixture is not shipped as an NPC/story catalog.
        public static InterviewTopicDefinition[] CreateTestOnlyCatalog() => new[] {
            new InterviewTopicDefinition("test.open",TestOnlyParticipantA,"TEST ONLY: câu không gate?","TEST ONLY: đáp án A"),
            new InterviewTopicDefinition("test.acquired",TestOnlyParticipantA,"TEST ONLY: câu có ID?","TEST ONLY: đáp án B",new InterviewRequirement(InterviewRequirementKind.AcquiredEvidence,"shoe")),
            new InterviewTopicDefinition("test.recorded",TestOnlyParticipantA,"TEST ONLY: câu cần ghi?","TEST ONLY: đáp án C",new InterviewRequirement(InterviewRequirementKind.RecordedObservation,"slide",0)),
            new InterviewTopicDefinition("test.and",TestOnlyParticipantA,"TEST ONLY: hai điều kiện?","TEST ONLY: đáp án D",new InterviewRequirement(InterviewRequirementKind.AcquiredEvidence,"shoe"),new InterviewRequirement(InterviewRequirementKind.RecordedObservation,"slide",1)),
            new InterviewTopicDefinition("test.clip",TestOnlyParticipantA,"TEST ONLY: clip đã đọc?","TEST ONLY: đáp án E",new InterviewRequirement(InterviewRequirementKind.RecordedObservation,"clip",0)),
            new InterviewTopicDefinition("test.invalid-id",TestOnlyParticipantA,"TEST ONLY: dữ liệu lỗi?","TEST ONLY: không được lộ",new InterviewRequirement(InterviewRequirementKind.AcquiredEvidence,"unknown-fixture")),
            new InterviewTopicDefinition("test.invalid-index",TestOnlyParticipantA,"TEST ONLY: index lỗi?","TEST ONLY: không được lộ",new InterviewRequirement(InterviewRequirementKind.RecordedObservation,"shoe",999)),
            new InterviewTopicDefinition("test.invalid-kind",TestOnlyParticipantA,"TEST ONLY: kind lỗi?","TEST ONLY: không được lộ",new InterviewRequirement((InterviewRequirementKind)99,"shoe")),
            new InterviewTopicDefinition("test.invalid-acquired-index",TestOnlyParticipantA,"TEST ONLY: cấu trúc lỗi?","TEST ONLY: không được lộ",new InterviewRequirement(InterviewRequirementKind.AcquiredEvidence,"shoe",0)),
            new InterviewTopicDefinition("test.other",TestOnlyParticipantB,"TEST ONLY: người B?","TEST ONLY: đáp án người B")
        };
        static InvestigationSession s;static GameObject root;static string output;static JArray rows;static int failures;
        static InterviewRevisitChecks(){EditorApplication.update+=Tick;}
        static string Arg(string k){var a=Environment.GetCommandLineArgs();int i=Array.IndexOf(a,k);return i>=0&&i+1<a.Length?a[i+1]:"";}
        public static void RunBatch()
        {
            output=Arg("--interview-output");string profile=Arg("--investigation-save-dir");
            if(!Application.isBatchMode||Application.unityVersion!="6000.6.2f1"||EditorApplication.isPlayingOrWillChangePlaymode||!Path.IsPathRooted(output)||Directory.Exists(output)||!Path.IsPathRooted(profile)||Directory.Exists(profile))throw new InvalidOperationException("Correct batch Editor, new private profile/output required");
            Directory.CreateDirectory(output);EditorSceneManager.OpenScene(Scene,OpenSceneMode.Single);SessionState.SetString(Key,"pending");EditorApplication.EnterPlaymode();
        }
        static void Check(string name,bool pass){rows.Add(new JObject{["name"]=name,["pass"]=pass});if(!pass){failures++;throw new Exception("Assertion failed: "+name);}}
        static bool Throws<T>(Action action) where T:Exception{try{action();return false;}catch(T){return true;}}
        static string State()=>JsonUtility.ToJson(s.data);
        static bool Empty(InterviewReply reply)=>string.IsNullOrEmpty(reply.Answer)&&string.IsNullOrEmpty(reply.Question)&&string.IsNullOrEmpty(reply.ParticipantId);
        static void Suite()
        {
            s.data.introAccepted=true;s.ClosePage();string error;InterviewReply reply;
            Check("default_snapshot_is_unavailable",!default(InterviewTopicState).CanAsk);
            Check("missing_source_rejected",Throws<ArgumentNullException>(()=>new InvestigationInterview(null,CreateTestOnlyCatalog())));
            Check("missing_catalog_rejected",Throws<ArgumentNullException>(()=>new InvestigationInterview(s,null)));
            var catalog=CreateTestOnlyCatalog();
            Check("blank_ids_and_missing_text_rejected",Throws<ArgumentException>(()=>new InterviewTopicDefinition(" ",TestOnlyParticipantA,"TEST ONLY","TEST ONLY"))&&Throws<ArgumentException>(()=>new InterviewTopicDefinition("test.x",TestOnlyParticipantA,"TEST ONLY","")));
            Check("duplicate_and_null_topic_rejected",Throws<ArgumentException>(()=>new InvestigationInterview(s,new[]{catalog[0],catalog[0]}))&&Throws<ArgumentException>(()=>new InvestigationInterview(s,new InterviewTopicDefinition[]{null})));
            var noTopics=new InvestigationInterview(s,Array.Empty<InterviewTopicDefinition>());
            Check("no_product_catalog_or_topics_by_default",!noTopics.TryBeginInterview(TestOnlyParticipantA,out error)&&noTopics.GetTopics().Length==0);
            var mutable=new List<InterviewTopicDefinition>(catalog);var interview=new InvestigationInterview(s,mutable);mutable.Clear();
            string before=State();
            Check("closed_query_and_ask_no_answer",interview.GetTopics().Length==0&&interview.GetTopicState("test.open").Availability==InterviewTopicAvailability.NoConversation&&!interview.TryAsk("test.open",out reply,out error)&&Empty(reply)&&error.Length>0&&State()==before);
            Check("unknown_participant_rejected",!interview.TryBeginInterview("unknown",out error)&&interview.ActiveParticipantId=="");
            Check("begin_no_world_or_save_mutation",interview.TryBeginInterview(TestOnlyParticipantA,out error)&&interview.ActiveParticipantId==TestOnlyParticipantA&&State()==before&&s.Page==""&&s.movement.enabled&&s.look.enabled);
            Check("catalog_input_list_copied",interview.GetTopics().Length==9);
            Check("only_open_topic_initially",interview.GetAvailableTopics().Length==1&&interview.GetAvailableTopics()[0].Id=="test.open");
            Check("query_never_exposes_answer_field",typeof(InterviewTopicState).GetField("Answer")==null);
            Check("locked_question_withheld",interview.GetTopicState("test.recorded").Availability==InterviewTopicAvailability.Locked&&interview.GetTopicState("test.recorded").Question=="");
            int answered=0;Action<InterviewReply> listener=r=>answered++;interview.Answered+=listener;
            Check("locked_ask_no_answer_or_event",!interview.TryAsk("test.recorded",out reply,out error)&&Empty(reply)&&answered==0&&State()==before);
            Check("unknown_null_case_and_cross_participant_fail",!interview.TryAsk(null,out reply,out error)&&Empty(reply)&&!interview.TryAsk("TEST.OPEN",out reply,out error)&&!interview.TryAsk("test.other",out reply,out error)&&interview.GetTopicState("test.other").Availability==InterviewTopicAvailability.UnknownTopic&&answered==0);
            Check("available_answer_and_single_event",interview.TryAsk("test.open",out reply,out error)&&reply.Answer=="TEST ONLY: đáp án A"&&reply.ParticipantId==TestOnlyParticipantA&&answered==1&&error=="");
            Check("repeat_same_answer_no_rewards_or_history",interview.TryAsk("test.open",out var repeat,out error)&&repeat.Answer==reply.Answer&&answered==2&&State()==before);
            s.Acquire("slide");s.Acquire("shoe");
            Check("acquired_and_recorded_are_distinct",interview.GetTopicState("test.acquired").CanAsk&&!interview.GetTopicState("test.recorded").CanAsk);
            Check("explicit_acquired_condition_asks",interview.TryAsk("test.acquired",out reply,out error)&&reply.Answer=="TEST ONLY: đáp án B");
            s.Observe("slide",0);Check("recorded_condition_opens_only_after_record",interview.GetTopicState("test.recorded").CanAsk&&interview.TryAsk("test.recorded",out reply,out error)&&reply.Answer=="TEST ONLY: đáp án C");
            Check("and_condition_requires_second_record",!interview.GetTopicState("test.and").CanAsk);
            s.Observe("slide",1);Check("and_conditions_all_satisfied",interview.GetTopicState("test.and").CanAsk);
            var stale=interview.GetTopicState("test.and");var shoe=s.Record("shoe");s.data.evidence.Remove(shoe);before=State();int count=answered;
            Check("selection_rechecks_stale_unlocked_snapshot",stale.CanAsk&&!interview.TryAsk("test.and",out reply,out error)&&Empty(reply)&&answered==count&&State()==before);s.data.evidence.Add(shoe);
            foreach(string id in new[]{"test.invalid-id","test.invalid-index","test.invalid-kind","test.invalid-acquired-index"})
                Check(id+"_fails_closed",interview.GetTopicState(id).Availability==InterviewTopicAvailability.InvalidCondition&&!interview.TryAsk(id,out reply,out error)&&Empty(reply)&&answered==count);
            s.Acquire("clip");s.Record("clip").observations.Add(0);s.data.clipBeforeSeen=false;s.data.clipEventSeen=false;
            Check("injected_clip_record_does_not_bypass_guard",!interview.TryAsk("test.clip",out reply,out error)&&Empty(reply));
            s.data.clipBeforeSeen=true;Check("one_clip_exposure_not_enough",!interview.GetTopicState("test.clip").CanAsk);s.data.clipEventSeen=true;
            Check("readable_recorded_clip_condition",interview.TryAsk("test.clip",out reply,out error)&&reply.Answer=="TEST ONLY: đáp án E");
            var requirements=new[]{new InterviewRequirement(InterviewRequirementKind.AcquiredEvidence,"shoe")};
            var immutable=new InterviewTopicDefinition("test.immutable",TestOnlyParticipantA,"TEST ONLY: immutable?","TEST ONLY: immutable",requirements);
            var privateEngine=new InvestigationInterview(s,new[]{immutable});privateEngine.TryBeginInterview(TestOnlyParticipantA,out error);
            requirements[0]=new InterviewRequirement(InterviewRequirementKind.AcquiredEvidence,"unknown");var exposed=immutable.Requirements;exposed[0]=requirements[0];
            Check("requirement_input_and_getter_arrays_copied",privateEngine.GetTopicState("test.immutable").CanAsk);
            var states=interview.GetTopics();states[0]=default;Check("query_snapshot_array_isolated",interview.GetTopicState("test.open").CanAsk);
            s.SetPaused(true);before=State();count=answered;
            Check("pause_blocks_query_ask_and_begin",interview.GetAvailableTopics().Length==0&&interview.GetTopicState("test.open").Availability==InterviewTopicAvailability.Paused&&!interview.TryAsk("test.open",out reply,out error)&&Empty(reply)&&!interview.TryBeginInterview(TestOnlyParticipantB,out error)&&interview.ActiveParticipantId==TestOnlyParticipantA&&answered==count&&State()==before);
            s.SetPaused(false);Check("resume_keeps_transient_context",interview.TryAsk("test.open",out reply,out error));
            before=State();interview.EndInterview();
            Check("end_cancel_no_rewards_or_save_mutation",interview.ActiveParticipantId==""&&interview.GetTopics().Length==0&&!interview.TryAsk("test.open",out reply,out error)&&Empty(reply)&&State()==before);
            interview.TryBeginInterview(TestOnlyParticipantB,out error);
            Check("participant_switch_isolated",interview.GetAvailableTopics().Length==1&&interview.GetAvailableTopics()[0].Id=="test.other"&&!interview.TryAsk("test.open",out reply,out error)&&interview.TryAsk("test.other",out reply,out error)&&reply.Answer=="TEST ONLY: đáp án người B");
            interview.TryBeginInterview(TestOnlyParticipantA,out error);count=answered;bool reentryRejected=false;
            Action<InterviewReply> recursive=r=>{reentryRejected=!interview.TryAsk("test.open",out var nested,out var nestedError)&&Empty(nested)&&nestedError.Length>0;};
            Action<InterviewReply> bad=r=>{throw new InvalidOperationException("TEST ONLY listener");};interview.Answered+=recursive;interview.Answered+=bad;before=State();
            Check("listener_exception_isolated_and_reentry_rejected",interview.TryAsk("test.open",out reply,out error)&&reentryRejected&&answered==count+1&&State()==before);
            interview.Answered-=recursive;interview.Answered-=bad;interview.Answered-=listener;count=answered;interview.TryAsk("test.open",out reply,out error);
            Check("unsubscribe_works",answered==count);
            string message=s.Message;before=State();
            Check("queries_and_repeated_asks_do_not_change_progress",interview.GetTopics().Length==9&&interview.TryAsk("test.open",out reply,out error)&&State()==before&&s.Message==message&&s.data.relations.Count==0&&s.data.trials.Count==0&&!s.data.finalized);
            s.Save();Check("private_save_for_context_test",s.Message.StartsWith("Đã lưu"));byte[] bytes=File.ReadAllBytes(s.SavePath);before=State();
            interview.TryAsk("test.open",out reply,out error);interview.EndInterview();interview.TryBeginInterview(TestOnlyParticipantA,out error);interview.GetTopics();
            Check("ask_begin_end_do_not_write_save",State()==before&&File.ReadAllBytes(s.SavePath).SequenceEqual(bytes));
            Check("valid_load_expires_old_context",s.Load()&&interview.ActiveParticipantId==""&&interview.GetTopics().Length==0&&interview.GetTopicState("test.open").Availability==InterviewTopicAvailability.ContextExpired&&!interview.TryAsk("test.open",out reply,out error)&&Empty(reply));
            Check("load_bytes_unchanged",File.ReadAllBytes(s.SavePath).SequenceEqual(bytes));
            Check("begin_after_load_rechecks_saved_conditions",interview.TryBeginInterview(TestOnlyParticipantA,out error)&&interview.GetTopicState("test.and").CanAsk&&interview.TryAsk("test.and",out reply,out error));
            var rebuilt=new InvestigationInterview(s,CreateTestOnlyCatalog());rebuilt.TryBeginInterview(TestOnlyParticipantA,out error);
            Check("reconstructed_engine_has_no_asked_history",rebuilt.GetAvailableTopics().Select(t=>t.Id).SequenceEqual(interview.GetAvailableTopics().Select(t=>t.Id))&&rebuilt.TryAsk("test.open",out reply,out error));
            var json=JObject.Parse(System.Text.Encoding.UTF8.GetString(bytes));Check("schema1_no_new_interview_history",json.Value<int>("schema")==1&&!json.Properties().Any(p=>p.Name.IndexOf("interview",StringComparison.OrdinalIgnoreCase)>=0||p.Name.IndexOf("topic",StringComparison.OrdinalIgnoreCase)>=0));
            var data=s.data;s.data=new InvestigationSave();
            Check("profile_data_replacement_invalidates_context",!interview.TryAsk("test.open",out reply,out error)&&Empty(reply)&&interview.GetAvailableTopics().Length==0);s.data=data;
            interview.EndInterview();before=State();Check("end_is_idempotent",interview.ActiveParticipantId==""&&State()==before);
        }
        static void Tick(){if(EditorApplication.isCompiling||EditorApplication.isUpdating)return;string phase=SessionState.GetString(Key,"");if(phase=="finished"&&!EditorApplication.isPlayingOrWillChangePlaymode){SessionState.SetString(Key,"");EditorApplication.Exit(SessionState.GetInt(Key+".exit",2));return;}if(!EditorApplication.isPlaying)return;try{if(phase=="pending"){output=Arg("--interview-output");rows=new JArray();s=TrialPlaybackContextChecks.BuildEnvironment(out root);SessionState.SetString(Key,"running");return;}if(phase=="running"){Suite();Finish();}}catch(Exception e){rows.Add(new JObject{["exception"]=e.ToString(),["pass"]=false});failures++;Finish();}}
        static void Finish(){if(root)UnityEngine.Object.DestroyImmediate(root);File.WriteAllText(Path.Combine(output,"results.json"),new JObject{["source"]=Arg("--interview-source"),["kind"]="PLAYMODE_API_PROXY_TEST_ONLY_CATALOG_NOT_UI_NOT_WINDOWS",["unity"]=Application.unityVersion,["scene"]=Scene,["failures"]=failures,["checks"]=rows}.ToString());SessionState.SetInt(Key+".exit",failures==0?0:1);SessionState.SetString(Key,"finished");EditorApplication.ExitPlaymode();}
    }
}
