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
        static readonly JArray checks=new JArray(),inputs=new JArray();
        static string output,source;static int failures;static double began,stepAt,lastProgress;static string step="";
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
        static IEnumerator Key(KeyCode key){w.SendEvent(new Event{type=EventType.KeyDown,keyCode=key});inputs.Add(new JObject{["kind"]="UNITY_KEY",["key"]=key.ToString()});yield return null;}
        static IEnumerator Go(string id){yield return Click("Hủy / trở lại");yield return Click(w.ControllerForChecks.TestCatalog.locations.Single(l=>l.id==id).label);C("travel_"+id,S.Location==id);}
        static IEnumerator View(string id,bool record=false){yield return Click("Hủy / trở lại");yield return Click("Khám/thu • "+id);yield return Click(id+" • xem");C("inspect_"+id,S.GetRecords().Any(r=>r.id==id&&r.viewed));
            if(record){string label=S.GetObservationOptions(id)[0];yield return Click("Ghi mục 0 • "+label);C("record_"+id,S.HasRecordedObservation(id,0));}yield return Click("Hủy / trở lại");}
        static IEnumerator Ask(string actor,string topic){yield return Click("Hủy / trở lại");yield return Click("Nói chuyện • "+actor);var q=S.GetConversationTopics().Single(t=>t.Id==topic);C("topic_open_"+topic,q.CanAsk);yield return Click(q.Question);C("heard_"+topic,S.GetHeardStatements().Any(h=>h.topicId==topic));yield return Click("Hủy / trở lại");}
        static IEnumerator Decode(string id){yield return Click("Mật mã");yield return Click("Thư • "+id);yield return Click("Ghi bản đọc");C("decoded_"+id,S.GetRecordedReadings().Any(r=>r.Id==id));yield return Click("Hủy / trở lại");}
        static IEnumerator Wait(Func<bool> condition,string label,float timeout=35){step=label;stepAt=EditorApplication.timeSinceStartup;while(!condition()){if(EditorApplication.timeSinceStartup-stepAt>timeout)throw new Exception("Timeout "+label);yield return null;}C(label,true);}
        static IEnumerator Delay(double seconds){double until=EditorApplication.timeSinceStartup+seconds;while(EditorApplication.timeSinceStartup<until)yield return null;}
        public static void RunBatch(){output=Arg("--playable-output");source=Arg("--playable-source");if(!Application.isBatchMode||!Path.IsPathRooted(output)||Directory.Exists(output))throw new Exception("Fresh isolated batch output required");Directory.CreateDirectory(output);
            EditorSceneManager.OpenScene("Assets/ConRoi/Modules/Gameplay/Scenes/Gameplay_ThuNghiem.unity");EditorApplication.ExecuteMenuItem("Con Roi/Gameplay/Toan tuyen TEST ONLY");w=Resources.FindObjectsOfTypeAll<FullCaseSandboxWindow>().Single();w.position=new Rect(40,40,1100,900);w.Show();w.Focus();began=EditorApplication.timeSinceStartup;stack.Push(Route());EditorApplication.update+=Update;}
        static void Update(){try{if(EditorApplication.timeSinceStartup-began>240)throw new Exception("Route timeout at "+step);w.Focus();Paint();if(EditorApplication.timeSinceStartup-lastProgress>1){lastProgress=EditorApplication.timeSinceStartup;File.WriteAllText(Path.Combine(output,"progress.txt"),step+"\n"+checks.Count+" checks; "+inputs.Count+" inputs");}
            var current=stack.Peek();if(!current.MoveNext()){stack.Pop();if(stack.Count==0)Finish();}else if(current.Current is IEnumerator nested)stack.Push(nested);
        }catch(Exception e){failures++;checks.Add(new JObject{["pass"]=false,["step"]=step,["exception"]=e.ToString()});Finish();}}
        static void Finish(){EditorApplication.update-=Update;string profile=w.ControllerForChecks.Profile;string state=S.ExportStateJson();w.Close();File.WriteAllText(Path.Combine(output,"final-state.json"),state);File.WriteAllText(Path.Combine(output,"results.json"),new JObject{["source"]=source,["kind"]="UNITY_EDITOR_IMGUI_MOUSE_TEXT_KEY_EVENTS_REAL_WINDOW_NO_ACTION_API_NO_WINDOWS_NO_NATURAL_OR_SHARED_SCENE_PLAY",["profile"]=profile,["failures"]=failures,["checks"]=checks,["inputs"]=inputs}.ToString());EditorApplication.Exit(failures==0?0:1);}
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
