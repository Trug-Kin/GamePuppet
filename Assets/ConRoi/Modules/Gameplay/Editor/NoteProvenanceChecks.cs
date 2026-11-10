using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Security.Cryptography;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using ConRoi.Investigation.V36.R107;

namespace ConRoi.Gameplay.EditorChecks
{
    [InitializeOnLoad]
    public static class NoteProvenanceChecks
    {
        const string Key = "ConRoi.NoteProvenanceChecks";
        const string Scene = "Assets/ConRoi/Modules/Gameplay/Scenes/Gameplay_ThuNghiem.unity";
        static InvestigationSession s; static GameObject root; static JArray rows; static string output;
        static int failures;
        static NoteProvenanceChecks() { EditorApplication.update += Tick; }
        static string Arg(string key) { var a = Environment.GetCommandLineArgs(); int i = Array.IndexOf(a,key); return i >= 0 && i+1 < a.Length ? a[i+1] : ""; }
        public static void RunBatch()
        {
            output = Arg("--provenance-output"); string profile = Arg("--investigation-save-dir");
            if (!Application.isBatchMode || Application.unityVersion != "6000.6.2f1" || EditorApplication.isPlayingOrWillChangePlaymode || !Path.IsPathRooted(output) || Directory.Exists(output) || !Path.IsPathRooted(profile) || Directory.Exists(profile) || !File.Exists(Arg("--provenance-old-save")))
                throw new InvalidOperationException("Correct batch Editor, new private profile/output and read-only old fixture required");
            Directory.CreateDirectory(output); EditorSceneManager.OpenScene(Scene,OpenSceneMode.Single);
            SessionState.SetString(Key,"pending"); EditorApplication.EnterPlaymode();
        }
        static void Check(string name,bool pass)
        {
            rows.Add(new JObject { ["name"]=name,["pass"]=pass });
            if (!pass) { failures++; throw new Exception("Assertion failed: "+name); }
        }
        static string State() => JsonUtility.ToJson(s.data);
        static string Notes() => new JArray(s.GetProvenanceNotes().Select(n => new JObject { ["origin"]=n.Origin.ToString(),["id"]=n.EvidenceId,["related"]=n.RelatedEvidenceId,["index"]=n.EntryIndex,["text"]=n.Text,["kind"]=n.Kind.ToString(),["status"]=n.Status.ToString(),["source"]=n.Source })).ToString();
        static InvestigationNote Note(string id,int index) { if (!s.TryGetRecordedNote(id,index,out var n,out var error)) throw new Exception("Fixture record unavailable: "+id+":"+index+" "+error); return n; }
        static void Suite()
        {
            s.data.introAccepted=true;s.ClosePage(); string error; InvestigationNote note;
            Check("fresh_profile_empty",s.GetProvenanceNotes().Length==0);
            Check("null_unknown_id_no_text",!s.TryGetRecordedNote(null,0,out note,out error)&&!string.IsNullOrEmpty(error)&&string.IsNullOrEmpty(note.Text)&&!s.TryGetRecordedNote("unknown",0,out note,out error));
            Check("known_but_unrecorded_no_text_or_source",!s.TryGetRecordedNote("shoe",0,out note,out error)&&string.IsNullOrEmpty(note.Text)&&string.IsNullOrEmpty(note.Source));
            s.Acquire("shoe");string before=State();
            Check("acquire_does_not_create_notes",s.GetProvenanceNotes().Length==0&&!s.TryGetRecordedNote("shoe",0,out note,out error)&&State()==before);
            s.Observe("shoe",-1);s.Observe("shoe",999);
            Check("invalid_observe_unchanged",State()==before&&s.GetProvenanceNotes().Length==0);
            s.Acquire("slide");s.Observe("slide",0);s.Observe("slide",0);
            Check("observe_progress_and_repeat_preserved",s.data.firstFloorRecorded&&s.Record("slide").observations.SequenceEqual(new[]{0})&&s.GetProvenanceNotes().Length==1);
            Check("observed_label",Note("slide",0).Kind==NoteProvenanceKind.Observation&&Note("slide",0).KindLabel=="Quan sát");
            var table=JArray.Parse(File.ReadAllText(Path.Combine(Application.dataPath,"../Docs/QA/Tasks/N2/NOTE-PROVENANCE-01/metadata.json")));
            s.data.clipBeforeSeen=true;s.data.clipEventSeen=true;
            foreach (JObject row in table)
            {
                string id=row.Value<string>("id");int index=row.Value<int>("index");s.Acquire(id);s.Observe(id,index);var n=Note(id,index);
                Check("table_"+id+"_"+index,n.Kind.ToString()==row.Value<string>("kind")&&n.Status.ToString()==row.Value<string>("status")&&n.Source==row.Value<string>("source")&&n.Text==InvestigationContent.Get(id).observations[index]);
            }
            Check("complete_table_34_unique_entries",table.Count==34&&s.GetProvenanceNotes().Length==34);
            Check("same_npc_category_does_not_infer_kind",Note("ha",0).Kind==NoteProvenanceKind.Statement&&Note("ha",0).KindLabel=="Lời kể"&&Note("khai",0).Kind==NoteProvenanceKind.Observation);
            Check("ambiguous_6_entries_pending",s.GetProvenanceNotes().Count(n=>n.Status==NoteProvenanceStatus.ContentReviewPending)==6&&Note("decoded",0).Kind==NoteProvenanceKind.Unclassified&&Note("hook",0).StatusLabel=="CHỜ REVIEW NỘI DUNG");
            s.AddRelation("slide","ha","Fixture: nghi vấn người chơi, không phải sự thật");
            s.data.conclusions[0].claim="Fixture: nhận định của người chơi";s.data.conclusions[0].choice=1;
            Check("relation_and_claim_are_hypotheses",s.GetProvenanceNotes().Where(n=>n.Origin==InvestigationNoteOrigin.PlayerRelation||n.Origin==InvestigationNoteOrigin.PlayerConclusion).Count(n=>n.Kind==NoteProvenanceKind.Hypothesis&&n.KindLabel=="Giả thuyết")==2);
            s.OpenPage("conclusions");s.SetConclusionConfirmation(true);s.data.finalized=true;
            Check("choice_confirmation_finalized_never_promote",s.GetProvenanceNotes().Where(n=>n.Origin==InvestigationNoteOrigin.PlayerRelation||n.Origin==InvestigationNoteOrigin.PlayerConclusion).All(n=>n.Kind==NoteProvenanceKind.Hypothesis)&&Note("ha",0).Kind==NoteProvenanceKind.Statement);
            s.data.finalized=false;s.SetConclusionConfirmation(false);
            s.data.trials.Add(new TrialRecord{id="release",report="Fixture: kết quả mô hình",returnSeconds=-1});
            Check("trial_not_promoted_to_real_event",s.GetProvenanceNotes().Single(n=>n.Origin==InvestigationNoteOrigin.TrialResult).Status==NoteProvenanceStatus.ContentReviewPending);
            var definition=InvestigationContent.Get("ha");string[] text=definition.observations;string source=definition.source;
            try
            {
                definition.observations=new[]{"Fixture: từ ngữ hoàn toàn khác, có chữ Quan sát và Giả thuyết","Fixture: chữ khác"};definition.source="Fixture: tên nguồn hiển thị bị đổi";
                Check("wording_does_not_classify",Note("ha",0).Kind==NoteProvenanceKind.Statement&&Note("ha",0).Source=="Quầy Hà • lời Hà kể"&&Note("ha",0).Text==definition.observations[0]);
            }
            finally { definition.observations=text;definition.source=source; }
            definition=InvestigationContent.Get("shoe");text=definition.observations;
            try
            {
                definition.observations=text.Concat(new[]{"Fixture: entry tương lai chưa có metadata"}).ToArray();s.Observe("shoe",2);
                Check("missing_metadata_is_explicit_unclassified",Note("shoe",2).Kind==NoteProvenanceKind.Unclassified&&Note("shoe",2).Status==NoteProvenanceStatus.MissingMetadata&&Note("shoe",2).StatusLabel=="Thiếu metadata");
            }
            finally { definition.observations=text;s.Record("shoe").observations.Remove(2); }
            s.data.evidence.Add(new EvidenceRecord{id="unknown-fixture",observations=new List<int>{0}});s.Record("shoe").observations.Add(999);
            try { before=State();Check("unknown_and_invalid_record_indices_safe_read_only",s.GetProvenanceNotes().All(n=>n.EvidenceId!="unknown-fixture"&&n.EntryIndex!=999)&&!s.TryGetRecordedNote("shoe",999,out note,out error)&&State()==before); }
            finally {s.data.evidence.RemoveAll(r=>r.id=="unknown-fixture");s.Record("shoe").observations.Remove(999);}
            s.data.clipBeforeSeen=false;s.data.clipEventSeen=false;before=State();
            Check("old_record_does_not_bypass_unseen_clip",!s.TryGetRecordedNote("clip",0,out note,out error)&&string.IsNullOrEmpty(note.Text)&&s.GetProvenanceNotes().All(n=>n.EvidenceId!="clip")&&State()==before);
            s.data.clipBeforeSeen=true;Check("one_exposure_still_hidden",s.GetProvenanceNotes().All(n=>n.EvidenceId!="clip"));s.data.clipEventSeen=true;
            Check("both_exposures_restore_recorded_entries",s.GetProvenanceNotes().Count(n=>n.EvidenceId=="clip")==3);
            var clip=s.Record("clip");var original=new List<int>(clip.observations);clip.observations.Clear();s.data.clipBeforeSeen=false;s.data.clipEventSeen=false;
            s.Observe("clip",0);Check("observe_clip_gate_unchanged",clip.observations.Count==0);
            s.data.clipBeforeSeen=true;s.data.clipEventSeen=true;s.Observe("clip",0);s.Observe("clip",0);
            Check("observe_clip_after_exposure_once",clip.observations.SequenceEqual(new[]{0}));clip.observations=original;
            string expected=Notes();before=State();var snapshot=s.GetProvenanceNotes();snapshot[0]=default;
            Check("snapshot_array_isolated",Notes()==expected&&State()==before);
            string message=s.Message;bool floor=s.data.firstFloorRecorded;
            Check("query_does_not_change_progress_or_message",Notes()==expected&&State()==before&&s.Message==message&&s.data.firstFloorRecorded==floor);
            s.OpenPage("dossier");s.SetPaused(true);before=State();
            Check("paused_queries_do_not_change_input_or_page",Notes()==expected&&State()==before&&s.Paused&&s.Page=="dossier"&&!s.movement.enabled&&!s.look.enabled);
            s.SetPaused(false);s.ClosePage();before=State();
            Check("world_query_does_not_open_modal",Notes()==expected&&State()==before&&s.Page==""&&s.movement.enabled&&s.look.enabled);
            s.Save();Check("normal_save_committed",s.Message.StartsWith("Đã lưu"));
            byte[] saved=File.ReadAllBytes(s.SavePath);expected=Notes();
            Check("new_profile_roundtrip_notes_and_bytes",s.Load()&&Notes()==expected&&File.ReadAllBytes(s.SavePath).SequenceEqual(saved));
            var json=JObject.Parse(System.Text.Encoding.UTF8.GetString(saved));
            Check("no_derived_metadata_in_schema1",json.Value<int>("schema")==1&&!json.Properties().Any(p=>p.Name.IndexOf("provenance",StringComparison.OrdinalIgnoreCase)>=0)&&json["evidence"].All(r=>((JObject)r).Properties().All(p=>new[]{"id","collected","observations"}.Contains(p.Name))));
            string oldPath=Arg("--provenance-old-save");byte[] old=File.ReadAllBytes(oldPath);File.WriteAllBytes(s.SavePath,old);
            Check("old_schema1_load",s.Load()&&s.data.schema==1&&s.Has("shoe")&&s.Has("shoecontrol"));
            before=State();var oldNotes=s.GetProvenanceNotes();Check("old_save_query_is_read_only",State()==before&&File.ReadAllBytes(s.SavePath).SequenceEqual(old));
            s.Observe("shoe",0);Check("old_save_records_new_note_without_migration",Note("shoe",0).Kind==NoteProvenanceKind.Observation);
            s.Save();Check("continue_old_save_roundtrip",s.Message.StartsWith("Đã lưu")&&s.Load()&&Note("shoe",0).Kind==NoteProvenanceKind.Observation);
            Check("old_fixture_source_bytes_unchanged",File.ReadAllBytes(oldPath).SequenceEqual(old));
            Check("api_returns_no_unknown_entry_text",!s.TryGetRecordedNote("not-a-real-id",0,out note,out error)&&string.IsNullOrEmpty(note.Text));
        }
        static void Tick()
        {
            if (EditorApplication.isCompiling||EditorApplication.isUpdating) return;
            string phase=SessionState.GetString(Key,"");
            if (phase=="finished"&&!EditorApplication.isPlayingOrWillChangePlaymode) { SessionState.SetString(Key,"");EditorApplication.Exit(SessionState.GetInt(Key+".exit",2));return; }
            if (!EditorApplication.isPlaying) return;
            try
            {
                if (phase=="pending") { output=Arg("--provenance-output");rows=new JArray();s=TrialPlaybackContextChecks.BuildEnvironment(out root);SessionState.SetString(Key,"running");return; }
                if (phase=="running") { Suite();Finish(); }
            }
            catch(Exception e) { rows.Add(new JObject { ["exception"]=e.ToString(),["pass"]=false });failures++;Finish(); }
        }
        static void Finish()
        {
            if(root)UnityEngine.Object.DestroyImmediate(root);
            File.WriteAllText(Path.Combine(output,"results.json"),new JObject { ["source"]=Arg("--provenance-source"),["kind"]="PLAYMODE_API_PROXY_FIXTURE_NOT_UI_NOT_WINDOWS",["unity"]=Application.unityVersion,["scene"]=Scene,["failures"]=failures,["checks"]=rows }.ToString());
            SessionState.SetInt(Key+".exit",failures==0?0:1);SessionState.SetString(Key,"finished");EditorApplication.ExitPlaymode();
        }
    }
}
