using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using ConRoi.Investigation.V36.R107;

namespace ConRoi.Gameplay.EditorChecks
{
    // Explicit producer/API fixtures, never claimed as a natural controller route.
    [InitializeOnLoad]
    public static class ObservationProgressChecks
    {
        const string Active="ConRoi.ObservationProgress.Api";
        const string Scene="Assets/ConRoi/Modules/Gameplay/Scenes/Gameplay_ThuNghiem.unity";
        [Serializable] class Row { public string name,detail;public bool pass; }
        [Serializable] class Report { public string source,unity,scene,profile,kind="PLAYMODE_API_FIXTURE_NOT_UI";public int failures;public List<Row> checks=new List<Row>(); }
        static InvestigationSession session;static Report report;
        static ObservationProgressChecks(){EditorApplication.update+=Tick;}
        static string Arg(string key){var a=Environment.GetCommandLineArgs();int i=Array.IndexOf(a,key);return i>=0&&i+1<a.Length?a[i+1]:"";}
        public static void RunBatch(){
            if(Application.unityVersion!="6000.6.2f1"||!Application.isBatchMode||EditorApplication.isPlayingOrWillChangePlaymode)throw new InvalidOperationException("Need Unity 6000.6.2f1 batch outside Play");
            if(!Path.IsPathRooted(Arg("--investigation-save-dir"))||Directory.Exists(Arg("--investigation-save-dir"))||!Path.IsPathRooted(Arg("--observation-output")))throw new InvalidOperationException("Need NEW absolute private profile and absolute output");
            Directory.CreateDirectory(Arg("--observation-output"));EditorSceneManager.OpenScene(Scene,OpenSceneMode.Single);SessionState.SetString(Active,"pending");EditorApplication.EnterPlaymode();
        }
        static void Tick(){
            if(EditorApplication.isCompiling||EditorApplication.isUpdating)return;
            string phase=SessionState.GetString(Active,"");
            if(phase=="finished"&&!EditorApplication.isPlayingOrWillChangePlaymode){SessionState.SetString(Active,"");EditorApplication.Exit(SessionState.GetInt(Active+".exit",2));return;}
            if(phase!="pending"||!EditorApplication.isPlaying)return;
            SessionState.SetString(Active,"running");
            report=new Report{source=Arg("--observation-source"),unity=Application.unityVersion,scene=UnityEngine.SceneManagement.SceneManager.GetActiveScene().path,profile=Arg("--investigation-save-dir")};
            GameObject root=null;
            try{session=TrialPlaybackContextChecks.BuildEnvironment(out root);Add("private_scene_profile",report.scene==Scene&&session.SaveDirectoryOverride==report.profile);Suite();}
            catch(Exception e){Add("fixture_exception",false,e.ToString());}
            finally{if(root)UnityEngine.Object.DestroyImmediate(root);File.WriteAllText(Path.Combine(Arg("--observation-output"),"results.json"),JsonUtility.ToJson(report,true));Debug.Log("Observation progress checks "+report.checks.Count+", failures "+report.failures);SessionState.SetInt(Active+".exit",report.failures==0?0:1);SessionState.SetString(Active,"finished");EditorApplication.ExitPlaymode();}
        }
        static void Add(string name,bool pass,string detail=""){report.checks.Add(new Row{name=name,pass=pass,detail=detail});if(!pass)report.failures++;}
        static string State()=>"recorded="+session.data.firstFloorRecorded+" reviewed="+session.data.firstNotesReviewed+" objective="+session.CurrentObjective+" evidence="+JsonUtility.ToJson(session.data);
        static void Fresh(){session.ClosePage();session.data=new InvestigationSave{introAccepted=true};for(int i=0;i<4;i++)session.data.conclusions.Add(new ConclusionRecord());}
        static void Batch(string id)=>typeof(InvestigationSession).GetMethod("RecordVisibleObservations",BindingFlags.Instance|BindingFlags.NonPublic).Invoke(session,new object[]{id});
        static void Suite(){
            bool baseline=Arg("--observation-baseline")=="yes";
            foreach(string id in new[]{"slide","shoeprint"}){
                Fresh();session.Acquire(id);session.OpenEvidence(id);
                Add(id+"_inspect_not_record",!session.data.firstFloorRecorded&&session.Record(id).observations.Count==0&&!session.data.firstNotesReviewed,State());
                session.OpenPage("dossier");Add(id+"_early_dossier_not_review",!session.data.firstNotesReviewed);session.OpenEvidence(id);
                session.Observe(id,0);Add(id+"_observe_updates_progress",session.Observation(id,0)&&session.data.firstFloorRecorded&&session.CurrentObjective=="P · Xem ghi chép vừa tạo.",State());
                session.Observe(id,0);Add(id+"_repeat_not_duplicate",session.Record(id).observations.Count==1&&session.data.evidence.Count==1);
                if(baseline)continue;
                session.OpenPage("dossier");Add(id+"_review_advances_to_phone",session.data.firstNotesReviewed&&session.CurrentObjective=="Kiểm tra điện thoại trên giá quay cạnh chỗ sửa rối.");
                session.Observe(id,0);session.Observe(id,1);Batch(id);Batch(id);Add(id+"_batch_repeat_keeps_progress",session.Record(id).observations.Count==2&&session.data.firstNotesReviewed&&session.data.evidence.Count==1);
                Add(id+"_no_unearned_evidence",!session.Has("phone")&&!session.Has("decoded")&&session.data.trials.Count==0&&!session.data.finalized);
            }
            Fresh();Batch("slide");Add("unacquired_batch_no_progress",!session.data.firstFloorRecorded&&session.data.evidence.Count==0,State());
            if(baseline)return;
            foreach(string id in new[]{"slide","shoeprint"}){
                Fresh();session.Observe(id,0);Add(id+"_unacquired_observe",!session.data.firstFloorRecorded&&session.data.evidence.Count==0);
                session.Acquire(id);session.Observe(id,-1);session.Observe(id,999);session.Observe("missing",0);Add(id+"_invalid_index_no_progress",!session.data.firstFloorRecorded&&session.Record(id).observations.Count==0);
                Batch(id);Add(id+"_ui_batch_contract",session.data.firstFloorRecorded&&!session.data.firstNotesReviewed&&session.Record(id).observations.Count==2&&session.Message=="Đã ghi quan sát. P · Xem ghi chép");
                Batch(id);Add(id+"_batch_duplicate_no_reset",session.data.firstFloorRecorded&&session.Record(id).observations.Count==2&&session.Message=="Đã ghi quan sát vào hồ sơ.");
            }
            foreach(string first in new[]{"slide","shoeprint"}){
                Fresh();string second=first=="slide"?"shoeprint":"slide";session.Acquire(first);session.Acquire(second);session.Observe(first,1);session.OpenPage("dossier");session.Observe(second,0);Add(first+"_then_"+second,session.data.firstFloorRecorded&&session.data.firstNotesReviewed&&session.data.evidence.Count==2&&session.Record(first).observations.Count==1&&session.Record(second).observations.Count==1);
            }
            Fresh();session.Acquire("mechanism");session.Observe("mechanism",0);Add("other_observation_no_floor_progress",!session.data.firstFloorRecorded&&session.Observation("mechanism",0));
            Fresh();session.Acquire("clip");session.Observe("clip",0);Add("clip_guard_unchanged",!session.Observation("clip",0)&&!session.data.firstFloorRecorded);
            Fresh();session.data.tutorialSkipped=true;session.Acquire("slide");Batch("slide");Add("skip_remains_skipped",session.data.tutorialSkipped&&session.data.firstFloorRecorded&&!session.data.firstNotesReviewed&&session.CurrentObjective=="Kiểm tra điện thoại trên giá quay cạnh chỗ sửa rối.");
            // Three affected progress states only; not a rerun of the save suites.
            for(int phase=0;phase<3;phase++){
                Fresh();session.Acquire("slide");session.OpenEvidence("slide");if(phase>0)session.Observe("slide",0);if(phase>1)session.OpenPage("dossier");
                string objective=session.CurrentObjective;bool recorded=session.data.firstFloorRecorded,reviewed=session.data.firstNotesReviewed;int count=session.Record("slide").observations.Count;
                session.Save();string saved=File.ReadAllText(session.SavePath);session.Observe("slide",1);session.OpenPage("dossier");bool loaded=session.Load();
                Add("roundtrip_phase_"+phase,loaded&&session.CurrentObjective==objective&&session.data.firstFloorRecorded==recorded&&session.data.firstNotesReviewed==reviewed&&session.Record("slide").observations.Count==count&&File.ReadAllText(session.SavePath)==saved,State());
                session.Observe("slide",0);session.Observe("slide",0);session.OpenPage("dossier");session.Save();loaded=session.Load();Add("continue_after_load_"+phase,loaded&&session.data.firstFloorRecorded&&session.data.firstNotesReviewed&&session.Record("slide").observations.Count==1);
            }
        }
    }
}
