using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEditor;
using ConRoi.Investigation.V36.R107;
namespace ConRoi.Gameplay.EditorChecks {
// TEST ONLY. Real action APIs; no progress grants. Controller input remains a manual check.
public static class FullCaseOrderChecks {
 static JArray checks=new JArray(),trace=new JArray(); static string root; static FullInvestigationRuntime s;
 static string Arg(string key){var a=Environment.GetCommandLineArgs();int i=Array.IndexOf(a,key);return i>=0?a[i+1]:null;}
 static void Check(string name,bool ok){checks.Add(new JObject{["name"]=name,["pass"]=ok});}
 static void Act(string name,FullActionResult r){trace.Add(new JObject{["action"]=name,["success"]=r.Success,["code"]=r.Code,["message"]=r.Message});if(!r.Success)throw new Exception(name+": "+r.Code);}
 static void Go(string id){s.Cancel();Act("travel "+id,s.TryTravel(id));}
 static void Ask(string who,string topic){Act("begin "+who,s.TryBeginConversation(who));Act("ask "+topic,s.TryAskTopic(topic,out _));s.Cancel();}
 static void Inspect(string id){s.Cancel();Act("collect "+id,s.TryCollect(id));Act("inspect "+id,s.TryInspect(id));}
 static string State()=>s.ExportStateJson();
 static bool ReceiptHeard()=>!s.HasRecordedObservation("receipt",0)&&s.HasHeardStatement("ha.receipt.facts")&&s.GetHeardStatements().Any(h=>h.topicId=="ha.cargo.receipt");
 static string ComparisonHint()=>typeof(FullCasePlayGuide).GetMethod("ComparisonHint")?.Invoke(null,new object[]{s}) as string;
 static void Affected(){
  s=new FullInvestigationRuntime(FullCaseTestCatalog.CreateApproved(false));s.SetModal(FullCaseModal.Comparison);string before=State();
  Check("empty comparison explains missing sources without mutation",!string.IsNullOrWhiteSpace(ComparisonHint())&&State()==before&&s.GetComparisons().Length==0);
  s.Cancel();Go("workshop");Inspect("slide");Act("record slide",s.TryRecordObservation("slide",0));Inspect("khai");s.Cancel();Go("office");
  Ask("npc.phuong","phuong.direction.homestay");Act("begin access",s.TryBeginConversation("npc.phuong"));Act("access",s.TryRequestAccess("test.samples"));s.Cancel();Go("homestay");
  Inspect("shoe");Act("record shoe",s.TryRecordObservation("shoe",0));s.Cancel();Go("workshop");Inspect("shoeprint");Act("record shoeprint",s.TryRecordObservation("shoeprint",0));s.SetModal(FullCaseModal.Comparison);before=State();
  Check("available comparison removes missing-source message",s.GetComparisons().Any(c=>c.id=="shoe")&&ComparisonHint()==""&&State()==before);
  s.Cancel();Go("market");Inspect("receipt");s.Cancel();Ask("npc.ha","ha.cargo.receipt");Check("receipt heard without record requirement",ReceiptHeard());
  before=State();Ask("npc.ha","ha.cargo.receipt");Check("repeat receipt retains one statement snapshot",State()==before&&s.GetHeardStatements().Count(h=>h.id=="ha.receipt.facts")==1);
  s.Cancel();Check("no station/ending from comparison availability or hearing",!s.CanTravel("station")&&s.GetEnding().Ending==FullEnding.None);
 }
 static void RoundTrip(string name){string p=Path.Combine(root,name);string before=State();Act("save "+name,s.TrySave(p));byte[] bytes=File.ReadAllBytes(FullInvestigationRuntime.SavePathFor(p));Act("load "+name,s.TryLoad(p));Check(name+" roundtrip progress/bytes/context",State()==before&&bytes.SequenceEqual(File.ReadAllBytes(FullInvestigationRuntime.SavePathFor(p)))&&s.Modal==FullCaseModal.None&&!s.Paused);}
 public static void RunBatch(){
  root=Arg("--order-output");if(string.IsNullOrEmpty(root)||Directory.Exists(root))throw new Exception("Fresh output required");Directory.CreateDirectory(root);
  try{
   if(Arg("--order-mode")=="affected"){Affected();}else{
   s=new FullInvestigationRuntime(FullCaseTestCatalog.CreateApproved(false));
   Act("begin early Phuong",s.TryBeginConversation("npc.phuong"));
   string before=State();var locked=s.GetConversationTopics().First(t=>t.Id=="phuong.samples.origin");
   Check("early topic hides question and answer",locked.Question==""&&!s.TryAskTopic(locked.Id,out var hidden).Success&&hidden.Answer==null&&State()==before);s.Cancel();
   Check("cancel before hearing does not grant history",s.GetHeardStatements().Length==0&&!s.TryAskTopic("phuong.direction.homestay",out _).Success);
   before=State();Check("wrong NPC location and unknown actor denied unchanged",!s.TryBeginConversation("npc.ha").Success&&!s.TryBeginConversation("npc.khai").Success&&State()==before);
   s.SetModal(FullCaseModal.Comparison);before=State();Check("early comparison hides tasks and denies result",s.GetComparisons().Length==0&&!s.TryCompare("paper",new[]{"letter","paper.khai"},"test.feature.match").Success&&State()==before);
   var hint=typeof(FullCasePlayGuide).GetMethod("ComparisonHint");Check("early comparison has explanatory empty-state",hint!=null&&!string.IsNullOrWhiteSpace((string)hint.Invoke(null,new object[]{s})));s.Cancel();
   s.SetModal(FullCaseModal.Reconstruction);Act("early trial release",s.TryStartTrial("release"));s.Advance(.25f);float t=s.TrialTime;s.Cancel();s.Advance(20);
   Check("early model observation can cancel without progress leak",s.TrialTime==t&&JObject.Parse(State())["trials"].Count()==0&&!s.Finalized&&!s.CanTravel("station")&&s.GetEnding().Ending==FullEnding.None);
   Go("workshop");Inspect("slide");Check("inspect not recorded",!s.HasRecordedObservation("slide",0));s.Cancel();Go("office");Go("workshop");
   Check("return preserves inspected unfinished objective",s.GetRecords().Single(r=>r.id=="slide").viewed&&!s.HasRecordedObservation("slide",0));
   // Deliberately finish Khai/sample route before recording slide or asking the suggested lead.
   Inspect("khai");s.Cancel();Go("office");Act("begin Phuong for access",s.TryBeginConversation("npc.phuong"));Act("request samples",s.TryRequestAccess("test.samples"));s.Cancel();
   Ask("npc.phuong","phuong.direction.homestay");Go("homestay");Inspect("samples");s.Cancel();before=State();Act("repeat collect samples",s.TryCollect("samples"));Check("collect repeat no duplicate",State()==before&&s.GetRecords().Count(r=>r.id=="samples")==1);
   Go("office");Ask("npc.phuong","phuong.samples.origin");before=State();Ask("npc.phuong","phuong.samples.origin");Check("repeat heard source retains one snapshot",State()==before&&s.GetHeardStatements().Count(h=>h.topicId=="phuong.samples.origin")==1&&s.GetRecordedStatements().Length==0);
   RoundTrip("samples-before-slide-record");Go("workshop");Inspect("slide");Act("record slide",s.TryRecordObservation("slide",0));before=State();Act("repeat record slide",s.TryRecordObservation("slide",0));Check("record repeat is idempotent",before==State());s.Cancel();
   string guide=FullCasePlayGuide.Next(s);Check("objective skips completed Khai/access/sample actions",!guide.Contains("Xin phép")&&!guide.Contains("tìm Bàn giao")&&!guide.Contains("Tìm Gặp Khải")&&s.HasEvidence("samples")&&s.HasHeardStatement("phuong.samples.origin"));
   Go("lake");Act("begin Bao early",s.TryBeginConversation("npc.bao"));before=State();Check("Bao locked before reading no leak",!s.TryAskTopic("bao.direction.lan",out hidden).Success&&hidden.Answer==null&&!s.CanTravel("garden")&&State()==before);s.Cancel();
   Go("market");Act("begin Ha before receipt",s.TryBeginConversation("npc.ha"));Check("receipt locked until inspected",!s.TryAskTopic("ha.cargo.receipt",out hidden).Success&&hidden.Answer==null);s.Cancel();
   Ask("npc.ha","ha.channel.paper");Inspect("receipt");s.Cancel();Ask("npc.ha","ha.cargo.receipt");Check("receipt heard without record requirement",ReceiptHeard());
   Inspect("letter");s.SetModal(FullCaseModal.Cipher);Act("partial mapping",s.TryAssign("pose.01","Z"));s.Cancel();before=State();Check("cancel mapping no reading/reply/history fabrication",s.GetRecordedReadings().Length==0&&!s.HasSentReply&&JObject.Parse(before)["mapping"].Count()==1);
   s.SetModal(FullCaseModal.Cipher);Check("reopen partial cipher not decoded",!s.TryRecordReading("letter").Success&&!s.TrySendReply(FullInvestigationLetters.ReplyPhrases).Success&&s.GetRecordedReadings().Length==0);s.Cancel();RoundTrip("cancelled-cipher");
   Go("school");Ask("npc.mai","mai.direction.theatre");Go("theatre");Go("market");
   before=State();Check("station and ending blocked without finalized case",!s.TryTravel("station").Success&&!s.TryOpenEndingSummary().Success&&State()==before&&s.Location=="market");
   s.SetModal(FullCaseModal.Conversation);before=State();Check("travel inside modal denied unchanged",!s.TryTravel("office").Success&&State()==before);s.Cancel();s.SetPaused(true);before=State();Check("pause action denied unchanged",!s.TryTravel("office").Success&&State()==before);s.SetPaused(false);RoundTrip("out-of-order-final");Go("office");
   Check("return still available after all interruptions",s.CanMove&&s.Location=="office"&&s.HasRecordedObservation("slide",0)&&!s.CanTravel("station"));
   }
  }catch(Exception e){checks.Add(new JObject{["name"]="checker exception",["pass"]=false,["detail"]=e.ToString()});}
  File.WriteAllText(Path.Combine(root,"results.json"),new JObject{["environment"]="isolated Unity headless action API; no desktop/controller input",["checks"]=checks,["trace"]=trace}.ToString());
  if(s!=null)File.WriteAllText(Path.Combine(root,"final-state.json"),State());EditorApplication.Exit(checks.Any(r=>!(bool)r["pass"])?1:0);
 }
}
}
