using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using ConRoi.Investigation.V36.R107;

namespace ConRoi.Gameplay.EditorChecks
{
    // Explicit temporary environment/API fixtures. Never saved into a scene.
    [InitializeOnLoad]
    public static class TrialPlaybackContextChecks
    {
        const string Scene="Assets/ConRoi/Modules/Gameplay/Scenes/Gameplay_ThuNghiem.unity";
        const string Active="ConRoi.TrialContext.Api";
        static readonly Stack<IEnumerator> tasks=new Stack<IEnumerator>();
        static InvestigationSession session; static GameObject root; static int frame=-1;
        static float oldCapture; static string output; static Report report;
        [Serializable] class Row { public string name,detail;public bool pass; }
        [Serializable] class Report { public string unity,scene,profile,kind="PLAYMODE_API_FIXTURE_FIXED_DELTA_NOT_UI",source;public int failures;public List<Row> checks=new List<Row>(); }
        static TrialPlaybackContextChecks(){EditorApplication.update+=Tick;}
        static string Arg(string key){var a=Environment.GetCommandLineArgs();int i=Array.IndexOf(a,key);return i>=0&&i+1<a.Length?a[i+1]:"";}
        static void Add(string name,bool pass,string detail=""){report.checks.Add(new Row{name=name,pass=pass,detail=detail});if(!pass)report.failures++;}
        static string Records()=>JsonUtility.ToJson(new RecordsBox{records=session.data.trials});
        [Serializable] class RecordsBox{public List<TrialRecord> records;}
        static string State()=>JsonUtility.ToJson(session.modelGate.state);
        static void WriteReport(){File.WriteAllText(Path.Combine(output,"results.json"),JsonUtility.ToJson(report,true));}
        public static void RunBatch(){
            if(Application.unityVersion!="6000.6.2f1"||!Application.isBatchMode||EditorApplication.isPlayingOrWillChangePlaymode)throw new InvalidOperationException("Need correct batch Editor outside Play");
            string profile=Arg("--investigation-save-dir");output=Arg("--trial-context-output");
            if(!Path.IsPathRooted(profile)||Directory.Exists(profile)||!Path.IsPathRooted(output))throw new InvalidOperationException("Need a new private absolute profile and output");
            Directory.CreateDirectory(output);EditorSceneManager.OpenScene(Scene,OpenSceneMode.Single);
            SessionState.SetString(Active+".Output",output);SessionState.SetString(Active+".Phase","pending");SessionState.SetBool(Active,true);EditorApplication.EnterPlaymode();
        }
        static void Tick(){
            if(!SessionState.GetBool(Active,false)||EditorApplication.isCompiling||EditorApplication.isUpdating)return;
            output=SessionState.GetString(Active+".Output","");string phase=SessionState.GetString(Active+".Phase","");
            if(phase=="finished"&&!EditorApplication.isPlayingOrWillChangePlaymode){SessionState.SetBool(Active,false);EditorApplication.Exit(SessionState.GetInt(Active+".Exit",2));return;}
            if(!EditorApplication.isPlaying)return;
            try{
                if(phase=="pending"){
                    report=new Report{unity=Application.unityVersion,scene=UnityEngine.SceneManagement.SceneManager.GetActiveScene().path,profile=Arg("--investigation-save-dir"),source=Arg("--trial-source")};
                    session=BuildEnvironment(out root);session.data.introAccepted=true;session.data.tutorialSkipped=true;session.ClosePage();session.movement.enabled=false;session.look.enabled=false;
                    oldCapture=Time.captureDeltaTime;Time.captureDeltaTime=.25f;tasks.Push(Suite());SessionState.SetString(Active+".Phase","running");return;
                }
                if(Time.frameCount==frame)return;frame=Time.frameCount;
                while(tasks.Count>0){var task=tasks.Peek();if(!task.MoveNext()){tasks.Pop();continue;}if(task.Current is IEnumerator nested){tasks.Push(nested);continue;}return;}
                Finish();
            }catch(Exception e){if(report!=null){Add("fixture_exception",false,e.ToString());Finish();}else throw;}
        }
        static void Finish(){Time.captureDeltaTime=oldCapture;Time.timeScale=1;if(root)UnityEngine.Object.DestroyImmediate(root);WriteReport();Debug.Log("Trial context checks "+report.checks.Count+", failures "+report.failures);SessionState.SetInt(Active+".Exit",report.failures==0?0:1);SessionState.SetString(Active+".Phase","finished");EditorApplication.ExitPlaymode();}
        static IEnumerator Frames(int count){for(int i=0;i<count;i++)yield return null;}
        static IEnumerator Complete(){for(int i=0;i<400&&session.TrialRunning;i++)yield return null;}
        static void Manual(bool run){typeof(InvestigationSession).GetProperty("TrialRunning").GetSetMethod(true).Invoke(session,new object[]{run});}
        static void Seek(float time){typeof(InvestigationSession).GetMethod("SeekTrial",BindingFlags.Instance|BindingFlags.NonPublic).Invoke(session,new object[]{time});}
        static IEnumerator Suite(){
            Add("scene_profile",report.scene==Scene&&session.SaveDirectoryOverride==report.profile);
            bool baseline=Arg("--trial-baseline")=="yes";
            foreach(string id in new[]{"release","help","hold","hook","wedge","after"}){
                session.OpenPage("reconstruction");session.StartTrial(id);yield return Frames(2);
                float t=session.TrialTime;string state=State(),records=Records(),text=session.TrialReport;bool intent=session.TrialRunning;
                session.OpenPage("dossier");yield return Frames(Mathf.CeilToInt(session.TrialDuration/.25f)+8);
                Add(id+"_long_absence",session.TrialTime==t&&State()==state&&Records()==records&&session.TrialReport==text&&session.TrialRunning==intent,"beforeTime="+t+" afterTime="+session.TrialTime+" duration="+session.TrialDuration+" recordsBefore="+records+" recordsAfter="+Records()+" gateBefore="+state+" gateAfter="+State());
                if(baseline){WriteReport();continue;}
                session.OpenPage("reconstruction");yield return Frames(1);
                Add(id+"_return_no_catchup",session.TrialTime>t&&session.TrialTime-t<.51f);
                foreach(string page in new[]{"","dossier","compare","inspect"}){
                    if(!session.TrialRunning){session.StartTrial(id);yield return Frames(1);}
                    t=session.TrialTime;state=State();records=Records();text=session.TrialReport;intent=session.TrialRunning;
                    if(page=="")session.ClosePage();else if(page=="inspect"){session.Acquire("slide");session.OpenEvidence("slide");}else session.OpenPage(page);
                    yield return Frames(3);
                    Add(id+"_leave_"+(page==""?"world":page),session.TrialTime==t&&State()==state&&Records()==records&&session.TrialReport==text&&session.TrialRunning==intent);
                    session.OpenPage("reconstruction");yield return Frames(1);
                    Add(id+"_resume_"+(page==""?"world":page),session.TrialTime>t&&session.TrialTime-t<.51f);
                }
                Manual(false);t=session.TrialTime;session.OpenPage("dossier");yield return Frames(2);session.OpenPage("reconstruction");yield return Frames(2);
                Add(id+"_manual_pause_preserved",!session.TrialRunning&&session.TrialTime==t);
                Manual(true);session.SetPaused(true);t=session.TrialTime;yield return Frames(2);Add(id+"_game_pause",session.TrialTime==t&&session.TrialRunning);session.SetPaused(false);yield return Frames(1);Add(id+"_game_resume",session.TrialTime>t);
                string beforeRecord=Records();Seek(session.TrialDuration);Add(id+"_seek_no_record",Records()==beforeRecord&&!session.TrialRunning);
                session.StartTrial(id);int beforeCount=session.data.trials.Count;yield return Complete();
                var expected=TrialTimeline.At(id,session.TrialTime,session.TrialClearAt);
                Add(id+"_visible_complete_one_record",!session.TrialRunning&&session.data.trials.Count==beforeCount+1&&State()==JsonUtility.ToJson(expected)&&TrialTimeline.Settled(session.modelGate.state),session.TrialReport);
                records=Records();state=State();t=session.TrialTime;session.ClosePage();yield return Frames(3);session.OpenPage("reconstruction");yield return Frames(2);Add(id+"_completed_unchanged",Records()==records&&State()==state&&session.TrialTime==t);
                session.StartTrial(id);Manual(false);Seek(TrialTimeline.LastAction(id)+.05f);
                if(session.CanClearTrialHolds){beforeCount=session.data.trials.Count;session.ClearTrialHolds();Manual(true);yield return Complete();expected=TrialTimeline.At(id,session.TrialTime,session.TrialClearAt);Add(id+"_clear_holds",TrialTimeline.Settled(session.modelGate.state)&&State()==JsonUtility.ToJson(expected)&&session.data.trials.Count==beforeCount+1);}
                WriteReport();
            }
            if(baseline)yield break;
            session.OpenPage("reconstruction");session.StartTrial("hold");yield return Frames(2);session.StartTrial("after");yield return Frames(1);Add("switch_trial",session.TrialId=="after"&&session.TrialTime<=.51f);
            // One affected lifecycle roundtrip only, not the Save test suites.
            session.Save();int count=session.data.trials.Count;bool loaded=session.Load();yield return Frames(2);
            Add("existing_save_load_resets_live_trial",loaded&&!session.TrialRunning&&session.TrialId==""&&session.TrialTime==0&&session.data.trials.Count==count);
            session.StartTrial("release");yield return Complete();Add("play_after_load",session.data.trials.Count==count+1);
        }
        static GameObject Cube(Transform parent,string name,Vector3 pos,Vector3 scale){var g=GameObject.CreatePrimitive(PrimitiveType.Cube);g.name=name;g.transform.SetParent(parent);g.transform.localPosition=pos;g.transform.localScale=scale;return g;}
        public static InvestigationSession BuildEnvironment(out GameObject root){
            root=new GameObject("TrialContext_Temporary_Proxy_Environment");root.SetActive(false);
            Cube(root.transform,"Floor",new Vector3(0,-.25f,3),new Vector3(20,.5f,20));
            var player=new GameObject("Player");player.transform.SetParent(root.transform);player.transform.localPosition=new Vector3(0,1.05f,0);var cc=player.AddComponent<CharacterController>();cc.height=2;cc.radius=.3f;
            var move=player.AddComponent<DieuKhienDiChuyen>();move.controller=cc;move.speed=2;
            var eye=new GameObject("Eye");eye.transform.SetParent(player.transform);eye.transform.localPosition=new Vector3(0,.6f,0);var cam=eye.AddComponent<Camera>();var look=eye.AddComponent<DieuKhienGocNhin>();
            var stage=new GameObject("Proxy stage");stage.transform.SetParent(root.transform);stage.transform.localPosition=new Vector3(40,0,0);
            Cube(stage.transform,"Stage floor",new Vector3(0,-.25f,0),new Vector3(12,.5f,12));
            var realGate=new GameObject("Inactive world gate");realGate.transform.SetParent(root.transform);var gate=realGate.AddComponent<ReturnGateMechanism>();gate.enabled=false;gate.checkPlayerContact=false;
            var modelObject=new GameObject("Model gate");modelObject.transform.SetParent(stage.transform);var model=modelObject.AddComponent<ReturnGateMechanism>();model.enabled=false;model.checkPlayerContact=false;
            var leaf=Cube(modelObject.transform,"Leaf proxy",new Vector3(.7f,3,1.4f),new Vector3(1.5f,1,.15f));model.leaf=leaf.transform;
            var kh=Cube(stage.transform,"Khai proxy",Vector3.zero,new Vector3(.4f,1.7f,.4f));var tu=Cube(stage.transform,"Tu proxy",Vector3.zero,new Vector3(.4f,1,.4f));
            var sc=new GameObject("Stage camera");sc.transform.SetParent(stage.transform);sc.transform.localPosition=new Vector3(4.2f,6.2f,-5.4f);sc.transform.LookAt(stage.transform.TransformPoint(new Vector3(0,1.8f,-.8f)));var stageCam=sc.AddComponent<Camera>();stageCam.enabled=false;stageCam.clearFlags=CameraClearFlags.SolidColor;stageCam.backgroundColor=new Color(.12f,.18f,.2f);var texture=new RenderTexture(898,480,16);stageCam.targetTexture=texture;
            foreach(var spec in new[]{new{id="mechanism",action="",x=-2f},new{id="slide",action="",x=2f},new{id="",action="table",x=0f}}){var go=Cube(root.transform,"Interaction proxy "+spec.id+spec.action,new Vector3(spec.x,1.6f,2.7f),new Vector3(.5f,.5f,.1f));var interact=go.AddComponent<InvestigationInteractable>();interact.evidenceId=spec.id;interact.action=spec.action;}
            var sun=new GameObject("Test light");sun.transform.SetParent(root.transform);sun.transform.rotation=Quaternion.Euler(50,-30,0);sun.AddComponent<Light>().type=LightType.Directional;
            var session=root.AddComponent<InvestigationSession>();session.movement=move;session.look=look;session.eye=cam;session.gate=gate;session.modelGate=model;session.modelKhai=kh.transform;session.modelTu=tu.transform;session.stageCamera=stageCam;session.stageTexture=texture;
            root.SetActive(true);Application.runInBackground=true;return session;
        }
    }
}
