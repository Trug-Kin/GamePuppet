using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using ConRoi.Investigation.V36.R107;
namespace ConRoi.Gameplay.EditorChecks
{
    // Sends mouse/key/text events to existing IMGUI controls. Reads runtime only for assertions.
    // No action API, Advance, state setter, save mutation or progress fixture in this route.
    public static class PlayableSandboxChecks
    {
        static FullCaseSandboxWindow w;static FullInvestigationRuntime S=>w.ControllerForChecks.Runtime;
        static readonly Stack<IEnumerator> stack=new Stack<IEnumerator>();
        static readonly JArray checks=new JArray(),inputs=new JArray(),ioDiagnostics=new JArray();
        static string activeProfile="";
        static void ObserveIO(object sender,System.Runtime.ExceptionServices.FirstChanceExceptionEventArgs e){if(e.Exception is IOException&&!string.IsNullOrEmpty(activeProfile)&&e.Exception.Message.IndexOf(activeProfile,StringComparison.OrdinalIgnoreCase)>=0)lock(ioDiagnostics)ioDiagnostics.Add(new JObject{["step"]=step,["type"]=e.Exception.GetType().Name,["message"]=e.Exception.Message,["hresult"]=e.Exception.HResult});}
        static string output,source,branch;static int failures;static double began,stepAt,lastProgress;static string step="";
        static string Arg(string k){var a=Environment.GetCommandLineArgs();int i=Array.IndexOf(a,k);return i<0?"":a[i+1];}
        static void C(string name,bool pass){checks.Add(new JObject{["name"]=name,["pass"]=pass});if(!pass)throw new Exception(name);}
        static void Paint(){w.SendEvent(new Event{type=EventType.Layout});w.SendEvent(new Event{type=EventType.Repaint});}
        static IEnumerator Click(string label){step="click:"+label;stepAt=EditorApplication.timeSinceStartup;
            Rect r;while(true){Paint();if(w.RevealControl(label,out r))break;if(EditorApplication.timeSinceStartup-stepAt>12)throw new Exception("Control missing/unreachable: "+label+" "+w.GeometryForChecks(label));yield return null;}
            w.SendEvent(new Event{type=EventType.MouseDown,button=0,mousePosition=r.center});w.SendEvent(new Event{type=EventType.MouseUp,button=0,mousePosition=r.center});
            inputs.Add(new JObject{["kind"]="UNITY_IMGUI_MOUSE",["control"]=label,["location"]=S.Location,["message"]=S.LastMessage});yield return null;
        }
        static IEnumerator Text(string label,string value){yield return Click(label);
            w.SendEvent(new Event{type=EventType.KeyDown,keyCode=KeyCode.A,modifiers=EventModifiers.Control});
            w.SendEvent(new Event{type=EventType.KeyDown,keyCode=KeyCode.Backspace});
            foreach(char c in value)w.SendEvent(new Event{type=EventType.KeyDown,character=c});
            w.SendEvent(new Event{type=EventType.KeyDown,keyCode=KeyCode.Tab});
            inputs.Add(new JObject{["kind"]="UNITY_IMGUI_TEXT",["control"]=label,["value"]=value});yield return null;
        }
        static IEnumerator Key(KeyCode key){w.SendEvent(new Event{type=EventType.KeyDown,keyCode=key});inputs.Add(new JObject{["kind"]="UNITY_KEY",["key"]=key.ToString(),["message"]=S.LastMessage,["location"]=S.Location,["finalized"]=S.Finalized,["viewed"]=S.EndingViewed});yield return null;}
        static IEnumerator Go(string id){yield return Click("Hủy / trở lại");yield return Click(w.ControllerForChecks.TestCatalog.locations.Single(l=>l.id==id).label);C("travel_"+id,S.Location==id);}
        static IEnumerator View(string id,bool record=false){yield return Click("Hủy / trở lại");yield return Click("Khám/thu • "+id);yield return Click(id+" • xem");C("inspect_"+id,S.GetRecords().Any(r=>r.id==id&&r.viewed));
            if(record){string label=S.GetObservationOptions(id)[0];yield return Click("Ghi mục 0 • "+label);C("record_"+id,S.HasRecordedObservation(id,0));}yield return Click("Hủy / trở lại");}
        static IEnumerator Ask(string actor,string topic){yield return Click("Hủy / trở lại");yield return Click("Nói chuyện • "+actor);var q=S.GetConversationTopics().Single(t=>t.Id==topic);C("topic_open_"+topic,q.CanAsk);yield return Click(q.Question);C("heard_"+topic,S.GetHeardStatements().Any(h=>h.topicId==topic));yield return Click("Hủy / trở lại");}
        static IEnumerator Decode(string id){yield return Click("Mật mã");yield return Click("Thư • "+id);yield return Click("Ghi bản đọc");C("decoded_"+id,S.GetRecordedReadings().Any(r=>r.Id==id));yield return Click("Hủy / trở lại");}
        static IEnumerator Wait(Func<bool> condition,string label,float timeout=35){step=label;stepAt=EditorApplication.timeSinceStartup;while(!condition()){if(EditorApplication.timeSinceStartup-stepAt>timeout)throw new Exception("Timeout "+label);yield return null;}C(label,true);}
        static IEnumerator Delay(double seconds){double until=EditorApplication.timeSinceStartup+seconds;while(EditorApplication.timeSinceStartup<until)yield return null;}
        public static void RunBatch(){output=Arg("--playable-output");source=Arg("--playable-source");branch=Arg("--playable-branch");if(branch!=""&&branch!="C"&&branch!="A-sai")throw new Exception("Unknown test route");if(!Application.isBatchMode||!Path.IsPathRooted(output)||Directory.Exists(output))throw new Exception("Fresh isolated batch output required");Directory.CreateDirectory(output);
            EditorSceneManager.OpenScene("Assets/ConRoi/Modules/Gameplay/Scenes/Gameplay_ThuNghiem.unity");EditorApplication.ExecuteMenuItem("Con Roi/Gameplay/Toan tuyen TEST ONLY");w=Resources.FindObjectsOfTypeAll<FullCaseSandboxWindow>().Single();w.position=new Rect(40,40,1100,900);w.Show();w.Focus();began=EditorApplication.timeSinceStartup;AppDomain.CurrentDomain.FirstChanceException+=ObserveIO;stack.Push(branch=="C"?CompleteRoute():branch=="A-sai"?UnsupportedRoute():Route());EditorApplication.update+=Update;}
        static void Update(){try{if(EditorApplication.timeSinceStartup-began>(branch==""?240:600))throw new Exception("Route timeout at "+step);w.Focus();Paint();if(EditorApplication.timeSinceStartup-lastProgress>1){lastProgress=EditorApplication.timeSinceStartup;File.WriteAllText(Path.Combine(output,"progress.txt"),step+"\n"+checks.Count+" checks; "+inputs.Count+" inputs");}
            var current=stack.Peek();if(!current.MoveNext()){stack.Pop();if(stack.Count==0)Finish();}else if(current.Current is IEnumerator nested)stack.Push(nested);
        }catch(Exception e){failures++;checks.Add(new JObject{["pass"]=false,["step"]=step,["exception"]=e.ToString()});Finish();}}
        static void Finish(){EditorApplication.update-=Update;AppDomain.CurrentDomain.FirstChanceException-=ObserveIO;string profile=w.ControllerForChecks.Profile;string state=S.ExportStateJson();w.Close();File.WriteAllText(Path.Combine(output,"final-state.json"),state);File.WriteAllText(Path.Combine(output,"results.json"),new JObject{["source"]=source,["branch"]=branch,["configuration"]="EDITOR_TEST_ONLY_NOT_PRODUCT_C3",["kind"]="UNITY_EDITOR_IMGUI_MOUSE_TEXT_KEY_EVENTS_REAL_WINDOW_NO_ACTION_API_NO_WINDOWS_NO_NATURAL_OR_SHARED_SCENE_PLAY",["profile"]=profile,["failures"]=failures,["checks"]=checks,["inputs"]=inputs,["ioDiagnostics"]=ioDiagnostics}.ToString());EditorApplication.Exit(failures==0?0:1);}

        static string Profile=>w.ControllerForChecks.Profile;
        static string MainPath=>FullInvestigationRuntime.SavePathFor(Profile);
        static string Checkpoint=>FullInvestigationRuntime.CheckpointPathFor(Profile);
        static string GroundLabel(string dimension,FullRequirement g)=>dimension+" • "+g.kind+" • "+g.id+":"+g.index;
        static IEnumerator StartReviewedTestProfile(){
            C("initial_profile_not_reviewed",!w.ControllerForChecks.TestCatalog.fallChainReviewed);
            yield return Click("Rubric mô phỏng cho profile MỚI (không là C3)");
            C("checkbox_does_not_change_running_catalog",!w.ControllerForChecks.TestCatalog.fallChainReviewed);
            yield return Click("Bắt đầu sạch • profile mới TEST ONLY");
            activeProfile=Profile;C("profile_outside_Unity_project",!Path.GetFullPath(Profile).StartsWith(Path.GetFullPath(Directory.GetCurrentDirectory())+Path.DirectorySeparatorChar,StringComparison.OrdinalIgnoreCase));C("explicit_clean_TEST_ONLY_rubric",S.TestOnly&&w.ControllerForChecks.TestCatalog.fallChainReviewed&&S.GetRecords().Length==0&&!S.Finalized&&!S.EndingViewed&&!Directory.Exists(Profile));
        }
        static IEnumerator ReadingAndReply(){
            yield return Go("workshop");yield return View("slide",true);
            yield return Go("market");yield return Ask("ha","ha.channel.paper");yield return View("letter",true);
            yield return Go("school");yield return Ask("mai","mai.direction.theatre");yield return Go("theatre");yield return View("henri");yield return Click("Mật mã");
            foreach(var pair in S.GetHenriLookup())yield return Text(pair.pose,pair.letter);
            yield return Click("Hủy / trở lại");yield return Decode("letter");yield return Go("market");yield return Click("Mật mã");
            var phrases=FullInvestigationLetters.ReplyPhrases;for(int i=0;i<phrases.Length;i++)yield return Text("Cụm hồi đáp "+(i+1),phrases[i]);
            yield return Click("Gửi qua kênh đã biết");C("reply_through_real_UI_actions",S.HasSentReply);yield return Click("Hủy / trở lại");
        }
        static IEnumerator TrialThroughUI(string id,bool clear=false){
            yield return Click("Bàn dựng");yield return Click(id);
            if(clear){yield return Wait(()=>S.TrialTime>=TrialTimeline.LastAction(id),id+"_last_action_visible",60);yield return Click("Bỏ giữ");}
            yield return Wait(()=>!S.TrialRunning,id+"_settled_visible_clock",60);
            string recordId=id+(clear?"/clear-holds":"");C("trial_record_"+recordId,S.GetAvailableGrounds().Any(g=>g.kind==FullRequirementKind.TrialRecorded&&g.id==recordId));yield return Click("Hủy / trở lại");
        }
        static IEnumerator CompareThroughUI(string id){
            yield return Click("Đối chiếu");var task=w.ControllerForChecks.TestCatalog.comparisons.Single(t=>t.id==id);
            C("comparison_prerequisites_"+id,S.GetComparisons().Any(t=>t.id==id));
            foreach(var evidence in task.evidenceIds)yield return Click(id+" • "+evidence);
            yield return Click("Ghi đối chiếu • "+id+" • test.feature.match");
            C("comparison_UI_"+id,S.GetAvailableGrounds().Any(g=>g.kind==FullRequirementKind.Comparison&&g.id==id));yield return Click("Hủy / trở lại");
        }
        static IEnumerator CompleteRoute(){
            yield return StartReviewedTestProfile();yield return Ask("phuong","phuong.direction.homestay");yield return ReadingAndReply();
            yield return Go("workshop");foreach(string id in new[]{"khai","mechanism","hook","shoeprint","r03","r12","fabric"})yield return View(id,true);
            yield return View("clip");yield return Click("Đoạn quay");yield return Click("Phát");yield return Wait(()=>!S.MediaPlaying,"C_clip_playback_finished",60);yield return Click("Hủy / trở lại");yield return Click("clip • xem");
            for(int i=0;i<2;i++){yield return Click("Ghi mục "+i+" • "+S.GetObservationOptions("clip")[i]);C("C_clip_record_"+i,S.HasRecordedObservation("clip",i));}yield return Click("Hủy / trở lại");
            yield return TrialThroughUI("release");yield return TrialThroughUI("help");yield return TrialThroughUI("hook",true);
            yield return Go("office");yield return Click("Nói chuyện • phuong");yield return Click("TEST ONLY • Phương/quyền truy cập mẫu");yield return Go("homestay");foreach(string id in new[]{"paper.khai","shoe","coat","M6"})yield return View(id,true);
            yield return Go("theatre");yield return View("M1",true);yield return Go("school");foreach(string id in new[]{"M3","M5","test.antique.labels"})yield return View(id,true);yield return Go("market");yield return View("M2",true);
            foreach(string id in new[]{"paper","cargo","staging","shoe","fabric","counter"})yield return CompareThroughUI(id);
            yield return Key(KeyCode.F5);string before=S.ExportStateJson();yield return Go("workshop");yield return Key(KeyCode.F6);C("C_producer_milestone_roundtrip",before==S.ExportStateJson());
            yield return Click("Kết luận");var rules=w.ControllerForChecks.TestCatalog.conclusions;
            foreach(var rule in rules){yield return Click("test.gdd-claim • "+rule.id);var set=rule.id=="sender"?rule.alternatives.Last():rule.alternatives.First();foreach(var g in set.requires){if(rule.id=="motive"&&g.id=="M6")continue;yield return Click(GroundLabel(rule.id,g));}}
            C("no_verdict_before_confirmation",S.GetEnding().Ending==FullEnding.None&&!S.Finalized);byte[] main=File.ReadAllBytes(MainPath);
            yield return Click("Chốt hồ sơ");C("C_without_confirmation_no_write",!S.Finalized&&!File.Exists(Checkpoint)&&main.SequenceEqual(File.ReadAllBytes(MainPath)));
            yield return Click("Xác nhận trung tính");yield return Click("Chốt hồ sơ");C("C_missing_M6_is_unsupported_not_complete",S.GetEnding().Ending==FullEnding.Unsupported&&S.GetEnding().UnsupportedDimensions.SequenceEqual(new[]{"motive"}));
            yield return Click("Khôi phục trước chốt");C("C_explicit_checkpoint_is_draft",!S.Finalized&&!S.Confirmed&&!S.EndingViewed);
            yield return Click("Kết luận");var last=rules.Single(r=>r.id=="motive").alternatives[0].requires.Single(g=>g.id=="M6");yield return Click(GroundLabel("motive",last));
            yield return Click("Xác nhận trung tính");C("C_confirmed",S.Confirmed);yield return Click("test.alternative-claim • motive");yield return Click("test.gdd-claim • motive");C("C_edit_invalidates_confirmation",!S.Confirmed);
            main=File.ReadAllBytes(MainPath);byte[] checkpoint=File.ReadAllBytes(Checkpoint);yield return Click("Chốt hồ sơ");C("C_stale_confirmation_no_file_change",!S.Finalized&&main.SequenceEqual(File.ReadAllBytes(MainPath))&&checkpoint.SequenceEqual(File.ReadAllBytes(Checkpoint)));
            yield return Key(KeyCode.F5);yield return Key(KeyCode.F6);yield return Click("Kết luận");yield return Click("Chốt hồ sơ");C("C_Load_requires_confirmation_again",!S.Finalized&&!S.Confirmed);
            yield return Click("Xác nhận trung tính");yield return Click("Chốt hồ sơ");C("C_complete_four_supported_dimensions",S.Finalized&&S.GetEnding().Ending==FullEnding.Complete&&S.GetEnding().UnsupportedDimensions.Length==0);
            yield return EndingLifecycle("C",Array.Empty<string>());
        }
        static readonly Dictionary<string,string> Consequence=new Dictionary<string,string>{{"sender","hướng điều tra và niềm tin"},{"presence","không lấy riêng cơ hội"},{"fall","bác/đình lại"},{"motive","không tự là căn cứ bắt"}};
        static void CheckConsequences(string label,string[] dimensions){var lines=S.GetEndingConsequences();C(label+"_single_aggregate_consequence_list",lines.Distinct().Count()==lines.Length&&lines.Length==(dimensions.Length==0?3:dimensions.Length+2));
            foreach(var pair in Consequence)C(label+"_consequence_scope_"+pair.Key,lines.Any(l=>l.Contains(pair.Value))==dimensions.Contains(pair.Key));
            C(label+"_no_overreach",lines.Any(l=>l.Contains("Lan được bảo vệ"))&&lines.Any(l=>l.Contains("không suy ý định"))&&!lines.Any(l=>l.Contains("kết án")||l.Contains("bắt Khải")||l.Contains("rối chắc chắn bị mất")));
        }
        static IEnumerator UnsupportedRoute(){
            yield return StartReviewedTestProfile();yield return ReadingAndReply();
            foreach(string variant in new[]{"sender","presence","motive","fall","sender+motive+fall"}){
                var dimensions=variant.Split('+');yield return Click("Kết luận");foreach(var rule in S.GetConclusions())yield return Click("Chưa đủ chứng minh • "+rule.id);
                foreach(string dimension in dimensions)yield return Click("test.alternative-claim • "+dimension);
                yield return Key(KeyCode.F5);string draft=S.ExportStateJson();yield return Key(KeyCode.F6);C("A_"+variant+"_draft_roundtrip",draft==S.ExportStateJson()&&!S.Finalized&&!S.Confirmed);yield return Click("Kết luận");
                yield return Click("Chốt hồ sơ");C("A_"+variant+"_requires_confirmation",!S.Finalized);yield return Click("Xác nhận trung tính");yield return Click("Chốt hồ sơ");
                C("A_"+variant+"_only_selected_dimensions_unsupported",S.GetEnding().Ending==FullEnding.Unsupported&&S.GetEnding().UnsupportedDimensions.OrderBy(x=>x).SequenceEqual(dimensions.OrderBy(x=>x)));
                yield return EndingLifecycle("A_"+variant,dimensions);
            }
        }
        static IEnumerator EndingLifecycle(string label,string[] dimensions){
            var expected=S.GetEnding().Ending;
            var cp=JsonUtility.FromJson<FullCaseSave>(File.ReadAllText(Checkpoint));C(label+"_checkpoint_not_finalized",!cp.finalized&&cp.ending==FullEnding.None&&!cp.endingViewed);
            byte[] checkpoint=File.ReadAllBytes(Checkpoint);CheckConsequences(label,dimensions);string[] lines=S.GetEndingConsequences();
            yield return Go("station");C(label+"_arrival_does_not_start",!S.EndingSummaryOpen&&!S.EndingPlaying&&!S.EndingViewed);yield return Key(KeyCode.F5);C(label+"_station_save_reports_success",S.LastMessage.StartsWith("Đã lưu"));yield return Key(KeyCode.F6);
            C(label+"_finalized_station_roundtrip",S.Finalized&&S.GetEnding().Ending==expected&&S.Location=="station"&&!S.EndingPlaying&&!S.EndingViewed);byte[] main=File.ReadAllBytes(MainPath);
            yield return Click("Tương tác cuộc hẹn • xem tóm tắt");yield return Click("Hủy / trở lại");C(label+"_summary_cancel_not_viewed",!S.EndingViewed&&!S.EndingPlaying&&main.SequenceEqual(File.ReadAllBytes(MainPath)));
            yield return Click("Tương tác cuộc hẹn • xem tóm tắt");yield return Click("Xác nhận bắt đầu trình diễn");C(label+"_explicit_start",S.EndingPlaying);yield return Click("Pause");yield return Click("Đã chạy hết trình diễn thử");C(label+"_paused_completion_blocked",S.Paused&&!S.EndingViewed&&main.SequenceEqual(File.ReadAllBytes(MainPath)));
            yield return Click("Tiếp tục");yield return Click("Hủy / trở lại");C(label+"_midplay_cancel_no_viewed",!S.EndingPlaying&&!S.EndingViewed);
            yield return Click("Tương tác cuộc hẹn • xem tóm tắt");yield return Click("Xác nhận bắt đầu trình diễn");yield return Key(KeyCode.F6);C(label+"_load_during_playback_safe",!S.EndingPlaying&&!S.EndingSummaryOpen&&!S.EndingViewed&&S.GetEnding().Ending==expected&&main.SequenceEqual(File.ReadAllBytes(MainPath)));
            yield return Click("Tương tác cuộc hẹn • xem tóm tắt");yield return Click("Xác nhận bắt đầu trình diễn");yield return Click("Đã chạy hết trình diễn thử");C(label+"_complete_persists_viewed_once",S.EndingViewed&&!S.EndingPlaying&&checkpoint.SequenceEqual(File.ReadAllBytes(Checkpoint)));
            main=File.ReadAllBytes(MainPath);yield return Key(KeyCode.F6);C(label+"_viewed_roundtrip",S.EndingViewed&&S.GetEnding().Ending==expected);
            yield return Click("Tương tác cuộc hẹn • xem tóm tắt");yield return Click("Xác nhận bắt đầu trình diễn");yield return Click("Đã chạy hết trình diễn thử");
            C(label+"_replay_keeps_bytes_consequences_and_ending",main.SequenceEqual(File.ReadAllBytes(MainPath))&&checkpoint.SequenceEqual(File.ReadAllBytes(Checkpoint))&&lines.SequenceEqual(S.GetEndingConsequences())&&S.GetEnding().Ending==expected&&S.EndingViewed);
            C(label+"_one_main_one_checkpoint",Directory.GetFiles(Path.GetDirectoryName(MainPath),"case*.json").Length==2);
            yield return Click("Chủ động quay lại điều tra • checkpoint trước chốt");C(label+"_return_to_draft_keeps_main_and_checkpoint",!S.Finalized&&!S.EndingViewed&&!S.Confirmed&&main.SequenceEqual(File.ReadAllBytes(MainPath))&&checkpoint.SequenceEqual(File.ReadAllBytes(Checkpoint)));
        }

        static IEnumerator Route(){
            yield return Click("Bắt đầu sạch • profile mới TEST ONLY");C("clean_no_progress_no_fake_review",S.GetRecords().Length==0&&S.GetHeardStatements().Length==0&&!S.EndingViewed&&!w.ControllerForChecks.TestCatalog.fallChainReviewed);
            yield return Ask("phuong","phuong.direction.homestay");yield return Go("workshop");yield return View("slide",true);yield return View("khai");
            yield return Key(KeyCode.F5);string saved=S.ExportStateJson();yield return Go("market");yield return Key(KeyCode.F6);C("F6_workshop_milestone",saved==S.ExportStateJson());
            yield return Key(KeyCode.Escape);C("pause_blocks_movement",S.Paused&&!S.CanMove);yield return Key(KeyCode.Escape);C("resume_after_pause",S.CanMove);
            yield return Go("market");yield return View("receipt");yield return Ask("ha","ha.cargo.receipt");yield return Ask("ha","ha.channel.paper");int heard=S.GetHeardStatements().Length;yield return Ask("ha","ha.channel.paper");C("repeat_no_duplicate",heard==S.GetHeardStatements().Length);yield return View("letter",true);
            yield return Go("school");yield return Ask("mai","mai.direction.theatre");yield return View("letter.mai");yield return Go("slope");yield return View("letter.tung");yield return Go("theatre");yield return View("henri");yield return Click("Mật mã");
            foreach(var pair in S.GetHenriLookup())yield return Text(pair.pose,pair.letter);C("mapping_entered_via_text_fields",S.GetLetter("letter").Value.Reading.IndexOf('_')<0);yield return Click("Hủy / trở lại");
            yield return Decode("letter");yield return Decode("letter.mai");yield return Decode("letter.tung");yield return Go("lake");yield return View("letter.bao");yield return Decode("letter.bao");yield return Ask("bao","bao.direction.lan");yield return Go("garden");yield return View("letter.lan");yield return Decode("letter.lan");C("five_readings_no_grants",S.GetRecordedReadings().Length==5);
            yield return Go("market");yield return Click("Mật mã");var phrases=FullInvestigationLetters.ReplyPhrases;for(int i=0;i<4;i++)yield return Text("Cụm hồi đáp "+(i+1),phrases[i]);yield return Click("Gửi qua kênh đã biết");C("reply_real_validation",S.HasSentReply);yield return Click("Hủy / trở lại");
            yield return Go("office");yield return Click("Nói chuyện • phuong");yield return Click("TEST ONLY • Phương/quyền truy cập mẫu");yield return Go("homestay");yield return View("paper.khai",true);
            yield return Click("Đối chiếu");yield return Click("paper • letter");yield return Click("paper • paper.khai");yield return Click("Ghi đối chiếu • paper • test.feature.match");C("comparison_recorded_via_selection",S.GetAvailableGrounds().Any(g=>g.kind==FullRequirementKind.Comparison&&g.id=="paper"));yield return Click("Hủy / trở lại");
            yield return Go("workshop");yield return View("clip");yield return Click("Đoạn quay");yield return Click("Phát");yield return Wait(()=>!S.MediaPlaying,"clip_finished_visible_clock");yield return Click("Hủy / trở lại");yield return Click("clip • xem");C("clip_readable_after_real_playback",S.GetObservationOptions("clip").Length>0);yield return Click("Hủy / trở lại");
            yield return Click("Bàn dựng");yield return Click("release");yield return Wait(()=>S.TrialTime>1,"trial_started");yield return Click("Pause tay");float time=S.TrialTime;yield return Click("Hủy / trở lại");yield return Delay(.6);yield return Click("Bàn dựng");C("return_preserves_manual_pause",!S.TrialRunning&&S.TrialTime==time);yield return Click("Chạy tiếp");yield return Wait(()=>!S.TrialRunning,"trial_finishes_through_clock");C("trial_model_note_created",S.GetSourcedNotes().Any(n=>n.Source=="Mô hình dựng lại"));yield return Click("Hủy / trở lại");
            yield return Key(KeyCode.F5);saved=S.ExportStateJson();yield return Go("office");yield return Key(KeyCode.F6);C("F6_readings_reply_comparison_trial_retained",S.ExportStateJson()==saved&&S.GetRecordedReadings().Length==5&&S.HasSentReply);yield return Click("Kết luận");yield return Click("Xác nhận trung tính");yield return Click("Chốt hồ sơ");C("honest_open_no_trial_rubric_needed",S.Finalized&&S.GetEnding().Ending==FullEnding.Open);
            yield return Go("station");C("arrival_no_auto_ending",!S.EndingPlaying&&!S.EndingViewed);yield return Click("Tương tác cuộc hẹn • xem tóm tắt");yield return Click("Hủy / trở lại");C("cancel_summary_no_viewed",!S.EndingViewed);yield return Click("Tương tác cuộc hẹn • xem tóm tắt");yield return Click("Xác nhận bắt đầu trình diễn");C("explicit_start",S.EndingPlaying);yield return Click("Pause");C("ending_pause",S.Paused);yield return Click("Tiếp tục");yield return Click("Đã chạy hết trình diễn thử");C("explicit_test_presenter_completion",S.EndingViewed&&!S.EndingPlaying);
            yield return Key(KeyCode.F6);C("ending_viewed_persisted",S.EndingViewed&&S.Location=="station");yield return Click("Chủ động quay lại điều tra • checkpoint trước chốt");C("checkpoint_returns_without_inventing_history",!S.Finalized&&!S.EndingViewed&&S.GetRecordedReadings().Length==5&&S.HasSentReply);yield return Go("market");
        }
    }
}
