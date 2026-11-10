using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Security.Cryptography;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEditor.SceneManagement;
using ConRoi.Investigation.V36.R107;

namespace ConRoi.Gameplay.EditorChecks
{
    // TEST ONLY. A menu-started, unseeded command route through the delivered adapter.
    // No state replacement, import, prerequisite mutation or evidence fixtures are used.
    public static class FullCaseDeliveryChecks
    {
        const string Menu="Con Roi/Gameplay/Toan tuyen TEST ONLY";
        const string Product="Assets/ConRoi/Modules/TichHop/Scenes/ConRoi_Chinh.unity";
        const string Gameplay="Assets/ConRoi/Modules/Gameplay/Scenes/Gameplay_ThuNghiem.unity";
        static readonly JArray checks=new JArray(), actions=new JArray();
        static FullCaseSandboxWindow window;
        static FullCaseSandboxController a;
        static FullInvestigationRuntime S=>a.Runtime;
        static string output,productHash,gameplayHash;
        static int failures;static double start;
        static string Arg(string key){var args=Environment.GetCommandLineArgs();int i=Array.IndexOf(args,key);return i>=0&&i+1<args.Length?args[i+1]:"";}
        static string Hash(string file){using(var h=SHA256.Create())return BitConverter.ToString(h.ComputeHash(File.ReadAllBytes(file))).Replace("-","").ToLowerInvariant();}
        static void Check(string name,bool pass){checks.Add(new JObject{["name"]=name,["pass"]=pass});if(!pass){failures++;throw new Exception(name);}}
        static void Do(string command,FullActionResult result){actions.Add(new JObject{["command"]=command,["success"]=result.Success,["code"]=result.Code,["location"]=S.Location,["records"]=S.GetRecords().Length});if(!result.Success)throw new Exception(command+": "+result.Code);}
        static void Key(KeyCode code){window.SendEvent(new Event{type=EventType.KeyDown,keyCode=code});}
        static void Go(string id){a.Cancel();Do("travel:"+id,a.Interact(FullPlaceholderKind.Location,id,id));}
        static void Read(string id)
        {
            a.Cancel();Do("collect:"+id,a.Interact(FullPlaceholderKind.Evidence,id,S.Location));Do("inspect:"+id,S.TryInspect(id));
            if(id=="clip"){
                S.SetModal(FullCaseModal.Media);Do("play:clip",S.TryPlayMedia());
                for(int frame=0;frame<130;frame++)a.Tick(.1f,true);
                Do("inspect:clip-after-visible-playback",S.TryInspect(id));
            }
            var definition=a.TestCatalog.evidence.First(e=>e.id==id);
            for(int index=0;index<definition.observations.Length;index++)Do("record:"+id+":"+index,S.TryRecordObservation(id,index));
            a.Cancel();
        }
        static void Local(){foreach(var item in S.GetLocalEvidence())Read(item.id);}
        static void Ask(string actor)
        {
            a.Cancel();Do("interview:"+actor,a.Interact(FullPlaceholderKind.Actor,"test.full.actor."+actor,S.Location));
            string before=S.ExportStateJson();Do("ask:"+actor,a.Ask("test.full.topic."+actor));
            Check("ask_"+actor+"_no_progress_grant",before==S.ExportStateJson());a.Cancel();
        }
        static void Trial(string id,bool clear=false)
        {
            S.SetModal(FullCaseModal.Reconstruction);Do("trial:"+id,S.TryStartTrial(id));
            // Advance the delivered playback through the adapter, never seek/seed its report.
            for(int frame=0;frame<2000&&S.TrialRunning;frame++){
                if(clear&&S.TrialTime>=TrialTimeline.LastAction(id)&&!actions.OfType<JObject>().Any(o=>(string)o["command"]=="clear:"+id))Do("clear:"+id,S.TryClearHolds());
                a.Tick(.1f,true);
            }
            Check("trial_"+id+"_settled",!S.TrialRunning);a.Cancel();
        }
        public static void RunBatch()
        {
            output=Arg("--delivery-output");
            if(!Application.isBatchMode||Application.unityVersion!="6000.6.2f1"||!Path.IsPathRooted(output)||Directory.Exists(output))throw new Exception("Unity 6000.6.2f1 batch and fresh absolute output required");
            Directory.CreateDirectory(output);productHash=Hash(Product);gameplayHash=Hash(Gameplay);
            try{
                var playerFiles=CompilationPipeline.GetAssemblies(AssembliesType.Player).SelectMany(x=>x.sourceFiles).Select(x=>x.Replace('\\','/')).ToArray();
                Check("player_compilation_excludes_fullcase_test_tools",!playerFiles.Any(x=>x.Contains("/Gameplay/Editor/FullCase"))&&playerFiles.Any(x=>x.EndsWith("/FullInvestigationRuntime.cs")));
                Check("product_scene_dependencies_exclude_fullcase_test_tools",!AssetDatabase.GetDependencies(Product,true).Any(x=>x.Contains("/Gameplay/Editor/FullCase")));
                var scene=EditorSceneManager.OpenScene(Product);
                Check("product_editmode_has_no_fullcase_placeholder",!scene.GetRootGameObjects().Any(g=>g.GetComponentsInChildren<FullCasePlaceholder>(true).Length>0));
                EditorSceneManager.OpenScene(Gameplay);
                Check("gameplay_does_not_auto_create_sandbox",Resources.FindObjectsOfTypeAll<FullCaseSandboxWindow>().Length==0);
                Check("documented_menu_opens",EditorApplication.ExecuteMenuItem(Menu));
                window=Resources.FindObjectsOfTypeAll<FullCaseSandboxWindow>().Single();a=window.ControllerForChecks;
                Check("menu_session_clean_and_profile_fresh",S.Location=="office"&&S.GetRecords().Length==0&&!S.Finalized&&!Directory.Exists(a.Profile));
                window.Repaint();start=EditorApplication.timeSinceStartup;EditorApplication.update+=Tick;
            }catch(Exception e){Fail(e);Finish();}
        }
        static void Tick()
        {
            if(EditorApplication.timeSinceStartup-start<3)return;EditorApplication.update-=Tick;
            try{
                Go("workshop");Local();Ask("khai");
                Key(KeyCode.F5);Check("route_F5_milestone_saved",File.Exists(FullInvestigationRuntime.SavePathFor(a.Profile)));string saved=S.ExportStateJson();
                Go("lake");Key(KeyCode.F6);Check("route_F6_returns_to_saved_workshop",S.Location=="workshop"&&S.ExportStateJson()==saved);
                Go("lake");Local();Ask("bao");Go("market");Local();Ask("ha");Go("school");Local();Ask("mai");Go("slope");Local();Ask("tung");
                Go("office");Ask("phuong");S.SetModal(FullCaseModal.Conversation);Do("request:test.samples",S.TryRequestAccess("test.samples"));a.Cancel();
                Go("theatre");Local();Check("route_book_exposes_26_keys",S.GetHenriLookup().Length==26);
                Go("homestay");Local();Ask("host");Go("garden");Local();Ask("lan");
                S.SetModal(FullCaseModal.Cipher);foreach(var pair in S.GetHenriLookup())Do("assign-from-book:"+pair.pose,S.TryAssign(pair.pose,pair.letter));
                foreach(string letter in FullInvestigationLetters.Ids)Do("read-letter:"+letter,S.TryRecordReading(letter));
                Do("send-reply:GDD5.2",S.TrySendReply(FullInvestigationLetters.ReplyPhrases));a.Cancel();
                Trial("release");Trial("help");Trial("hook",true);
                S.SetModal(FullCaseModal.Comparison);foreach(var task in S.GetComparisons())Do("compare-test-feature:"+task.id,S.TryCompare(task.id,task.evidenceIds,"test.feature.match"));a.Cancel();
                Check("route_counter_attempt_present",S.GetAvailableGrounds().Any(g=>g.kind==FullRequirementKind.Comparison&&g.id=="counter"));
                Go("office");S.SetModal(FullCaseModal.Conclusions);
                foreach(var rule in a.TestCatalog.conclusions)Do("draft-test-rubric:"+rule.id,S.TrySetConclusion(rule.id,"test.gdd-claim",rule.alternatives[0].requires));
                Check("route_no_verdict_before_confirm",S.GetEnding().Ending==FullEnding.None);
                Do("confirm",S.TryConfirm());Do("finalize-test-rubric",S.TryFinalize(a.Profile));
                Check("route_simulated_complete_only",S.GetEnding().Ending==FullEnding.Complete);
                Go("station");Key(KeyCode.F5);Key(KeyCode.F6);Check("route_station_persisted",S.Location=="station"&&S.Finalized);
                Do("explicit-checkpoint",a.Load(true));Check("route_checkpoint_is_draft_not_final",!S.Finalized&&S.Location=="office"&&S.Modal==FullCaseModal.None&&!S.Confirmed);
            }catch(Exception e){Fail(e);}finally{Finish();}
        }
        static void Fail(Exception e){failures++;checks.Add(new JObject{["pass"]=false,["exception"]=e.ToString()});}
        static void Finish()
        {
            EditorApplication.update-=Tick;string profile=a?.Profile??"";if(window)window.Close();
            bool intact=Hash(Product)==productHash&&Hash(Gameplay)==gameplayHash;
            checks.Add(new JObject{["name"]="scene_files_unchanged",["pass"]=intact});if(!intact)failures++;
            File.WriteAllText(Path.Combine(output,"results.json"),new JObject{["source"]=Arg("--delivery-source"),["unity"]=Application.unityVersion,["kind"]="MENU_STARTED_EDITOR_ADAPTER_UNSEEDED_COMMAND_ROUTE_PLUS_UNITY_KEY_EVENTS_NOT_WINDOWS_NOT_NATURAL_PLAY_NOT_PRODUCT_SCENE_PLAY",["profile"]=profile,["failures"]=failures,["checks"]=checks,["actions"]=actions}.ToString());EditorApplication.Exit(failures==0?0:1);
        }
    }
}
