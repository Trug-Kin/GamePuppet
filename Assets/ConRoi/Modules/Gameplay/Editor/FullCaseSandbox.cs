using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using ConRoi.Investigation.V36.R107;

namespace ConRoi.Gameplay.EditorChecks
{
    public enum FullPlaceholderKind { Location, Evidence, Actor }
    // Editor-only replacement seam. Names, transform positions and button labels are visual;
    // only these IDs enter gameplay commands. Never attach this test component to product.
    public sealed class FullCasePlaceholder
    {
        public string StableId,LocationId;public FullPlaceholderKind Kind;
        public FullActionResult Interact(FullCaseSandboxController adapter)=>adapter.Interact(Kind,StableId,LocationId);
    }
    public sealed class FullCaseSandboxController : IDisposable
    {
        public FullInvestigationRuntime Runtime {get;}
        public InvestigationInterview Interview {get;}
        public string Profile {get;}
        public string Reply {get;private set;}="";
        public FullCaseCatalog TestCatalog {get;}
        object context;
        public FullCaseSandboxController(string profile, bool simulatedRubric=false)
        {
            Profile=profile;TestCatalog=FullCaseTestCatalog.CreateApproved(simulatedRubric);Runtime=new FullInvestigationRuntime(TestCatalog);
            Interview=InvestigationInterview.ForSource(Runtime,FullCaseTestCatalog.Topics());context=Runtime.DataContext;Runtime.Changed+=OnChanged;
        }
        void OnChanged(){if(!ReferenceEquals(context,Runtime.DataContext)){context=Runtime.DataContext;Reply="";Interview.EndInterview();}}
        public FullActionResult Interact(FullPlaceholderKind kind,string id,string location)
        {
            if(kind==FullPlaceholderKind.Location)return Runtime.TryTravel(id);
            if(location!=Runtime.Location)return FullActionResult.Fail("location","TEST ONLY: đối tượng không ở nơi hiện tại.");
            if(kind==FullPlaceholderKind.Evidence)return Runtime.TryCollect(id);
            if(Runtime.Paused||Runtime.Modal!=FullCaseModal.None)return FullActionResult.Fail("context","Đóng thao tác hoặc tiếp tục trước khi hỏi.");
            var result=Runtime.TryBeginConversation(id);Reply="";return result;
        }
        public FullActionResult Ask(string topic)
        {
            Reply="";
            var result=Runtime.TryAskTopic(topic,out var reply);
            if(result.Success)Reply=reply.Answer;return result;
        }
        public void Cancel(){Interview.EndInterview();Reply="";Runtime.Cancel();}
        public void Tick(float visibleDelta,bool visible){if(visible)Runtime.Advance(visibleDelta);}
        public FullActionResult Save()=>Runtime.TrySave(Profile);
        public FullActionResult Load(bool checkpoint=false)=>Runtime.TryLoad(Profile,checkpoint);
        public void Dispose(){Runtime.Changed-=OnChanged;Interview.EndInterview();}
    }
    public sealed class FullCaseSandboxWindow : EditorWindow
    {
        FullCaseSandboxController adapter; PreviewRenderUtility preview;GameObject root;
        internal FullCaseSandboxController ControllerForChecks=>adapter;
        readonly Dictionary<GameObject,FullCasePlaceholder> placeholders=new Dictionary<GameObject,FullCasePlaceholder>();
        readonly Dictionary<string,Vector3> anchors=new Dictionary<string,Vector3>();
        bool simulatedRubric,inScroll;
        readonly HashSet<string> scrollControls=new HashSet<string>();
        readonly Dictionary<string,Rect> controls=new Dictionary<string,Rect>();
        bool Button(string label){bool pressed=GUILayout.Button(label);Remember(label);return pressed;}
        bool Toggle(string label,bool value){bool next=GUILayout.Toggle(value,label);Remember(label);return next;}
        string Field(string label,string value){string next=EditorGUILayout.TextField(label,value);Remember(label);return next;}
        void Remember(string label){if(Event.current.type==EventType.Repaint){var r=GUILayoutUtility.GetLastRect();r.position=GUIUtility.GUIToScreenPoint(r.position)-position.position;controls[label]=r;if(inScroll)scrollControls.Add(label);else scrollControls.Remove(label);}}
        internal string GeometryForChecks(string label)=>"scroll="+scroll+" rect="+(controls.TryGetValue(label,out var r)?r.ToString():"absent")+" window="+position;
        internal bool RevealControl(string label,out Rect rect){if(!controls.TryGetValue(label,out rect))return false;if(!scrollControls.Contains(label))return true;float y=position.height*.55f;if(rect.center.y<260||rect.center.y>position.height-2){scroll.y=Mathf.Max(0,scroll.y+rect.center.y-y);Repaint();return false;}return true;}
        void FreshProfile(){adapter?.Dispose();adapter=new FullCaseSandboxController(Path.Combine(Directory.GetCurrentDirectory(),"Test","PlaytestProfiles","FullCaseSandbox_"+Guid.NewGuid().ToString("N")),simulatedRubric);selected="";letterId="letter";mapping.Clear();choices.Clear();reply=new string[4];message="TEST ONLY: profile mới; profile cũ được giữ trên đĩa.";scroll=Vector2.zero;}
        Vector2 scroll;string selected="",letterId="letter",message="";string[] reply=new string[4];
        readonly Dictionary<string,HashSet<string>> choices=new Dictionary<string,HashSet<string>>();
        readonly Dictionary<string,string> mapping=new Dictionary<string,string>();double previous;
        [MenuItem("Con Roi/Gameplay/Toan tuyen TEST ONLY")]
        public static void Open()=>GetWindow<FullCaseSandboxWindow>("Con Rối • TEST ONLY");
        void OnEnable(){
            var profile=Path.Combine(Directory.GetCurrentDirectory(),"Test","PlaytestProfiles","FullCaseSandbox_"+Guid.NewGuid().ToString("N"));
            adapter=new FullCaseSandboxController(profile);preview=new PreviewRenderUtility();root=new GameObject("TEST ONLY full-case proxies"){hideFlags=HideFlags.HideAndDontSave};
            preview.AddSingleGO(root);int i=0;
            foreach(var location in adapter.Runtime.GetLocations()){
                var go=GameObject.CreatePrimitive(PrimitiveType.Cube);go.name="TEST ONLY region "+location.id;go.hideFlags=HideFlags.HideAndDontSave;go.transform.SetParent(root.transform,false);go.transform.localPosition=new Vector3(i++*5,0,0);go.transform.localScale=new Vector3(2,.1f,2);anchors.Add(location.id,go.transform.position);
                var binding=new FullCasePlaceholder();placeholders.Add(go,binding);binding.StableId=location.id;binding.LocationId=location.id;binding.Kind=FullPlaceholderKind.Location;
            }
            foreach(var evidence in adapter.TestCatalog.evidence){var go=GameObject.CreatePrimitive(PrimitiveType.Cube);go.name="TEST ONLY item "+evidence.id;go.hideFlags=HideFlags.HideAndDontSave;go.transform.SetParent(root.transform,false);go.transform.position=anchors[evidence.location]+new Vector3((evidence.id.GetHashCode()&7)*.16f-.5f,.3f,0);go.transform.localScale=Vector3.one*.2f;var b=new FullCasePlaceholder();placeholders.Add(go,b);b.StableId=evidence.id;b.LocationId=evidence.location;b.Kind=FullPlaceholderKind.Evidence;}
            preview.camera.nearClipPlane=.01f;preview.camera.farClipPlane=12;preview.camera.clearFlags=CameraClearFlags.Color;preview.camera.backgroundColor=new Color(.13f,.15f,.17f);preview.lights[0].intensity=1;
            previous=EditorApplication.timeSinceStartup;EditorApplication.update+=Tick;
        }
        void Tick(){double now=EditorApplication.timeSinceStartup;float dt=Mathf.Min(.1f,(float)(now-previous));previous=now;if(adapter!=null){adapter.Tick(dt,focusedWindow==this);Repaint();}}
        void OnDisable(){EditorApplication.update-=Tick;adapter?.Dispose();adapter=null;preview?.Cleanup();preview=null;anchors.Clear();placeholders.Clear();controls.Clear();scrollControls.Clear();}
        void Do(FullActionResult result){message=result.Message;if(result.Success&&result.Message.StartsWith("Đã tải")){mapping.Clear();foreach(var pair in JsonUtility.FromJson<FullCaseSave>(adapter.Runtime.ExportStateJson()).mapping)mapping[pair.pose]=pair.letter;choices.Clear();selected="";reply=new string[4];}}
        void OnGUI(){
            if(adapter==null)return;var s=adapter.Runtime;var evt=Event.current;
            if(evt.type==EventType.KeyDown){if(evt.keyCode==KeyCode.Escape){if(s.Modal==FullCaseModal.None)s.SetPaused(!s.Paused);else adapter.Cancel();evt.Use();}else if(evt.keyCode==KeyCode.F5){Do(adapter.Save());evt.Use();}else if(evt.keyCode==KeyCode.F6){Do(adapter.Load());evt.Use();}}
            EditorGUILayout.HelpBox("TEST ONLY • Producer/gate lời khai theo quyết định chủ dự án; vật mẫu/feature/rubric còn TEST ONLY, không canon. Không mở hết/hoàn tất. Editor UI/proxy không phải scene chung hoặc controller Windows.",MessageType.Warning);
            simulatedRubric=EditorGUILayout.Toggle("Rubric mô phỏng cho profile MỚI (không là C3)",simulatedRubric);
            if(Button("Bắt đầu sạch • profile mới TEST ONLY")){FreshProfile();s=adapter.Runtime;}
            EditorGUILayout.LabelField("Rubric phiên hiện tại",adapter.TestCatalog.fallChainReviewed?"TEST ONLY được bật (không review sản phẩm)":"Chưa bật; có thể chơi nhánh Chưa đủ chứng minh");
            EditorGUILayout.LabelField("Profile riêng",adapter.Profile);EditorGUILayout.LabelField("Nơi / modal / pause",s.Location+" / "+s.Modal+" / "+s.Paused);
            GUILayout.BeginHorizontal();if(Button("Hủy / trở lại"))adapter.Cancel();if(Button(s.Paused?"Tiếp tục":"Pause"))s.SetPaused(!s.Paused);if(Button("F5 • Lưu"))Do(adapter.Save());if(Button("F6 • Tải"))Do(adapter.Load());if(Button("Khôi phục trước chốt"))Do(adapter.Load(true));GUILayout.EndHorizontal();
            EditorGUILayout.HelpBox(message,MessageType.Info);
            scroll=EditorGUILayout.BeginScrollView(scroll);inScroll=true;
            var rect=GUILayoutUtility.GetRect(300,160);if(Event.current.type==EventType.Repaint&&preview!=null){preview.BeginPreview(rect,GUIStyle.none);var anchor=anchors[s.Location];preview.camera.transform.position=anchor+new Vector3(2,2,-3);preview.camera.transform.LookAt(anchor);preview.Render();GUI.DrawTexture(rect,preview.EndPreview(),ScaleMode.ScaleToFit);}
            GUILayout.Label("Chuyển khu (đóng modal trước)",EditorStyles.boldLabel);foreach(var location in s.GetLocations())if(Button(location.label+(s.CanTravel(location.id)?"":" • khóa")))Do(adapter.Interact(FullPlaceholderKind.Location,location.id,location.id));
            GUILayout.Label("Nguồn tại nơi hiện tại",EditorStyles.boldLabel);foreach(var item in s.GetLocalEvidence())if(Button("Khám/thu • "+item.id)){Do(adapter.Interact(FullPlaceholderKind.Evidence,item.id,s.Location));selected=item.id;}
            GUILayout.Label("NPC thay thế • producer thật",EditorStyles.boldLabel);string actor=s.Location=="lake"?"bao":s.Location=="market"?"ha":s.Location=="school"?"mai":s.Location=="slope"?"tung":s.Location=="garden"?"lan":s.Location=="homestay"?"host":s.Location=="office"?"phuong":"khai";
            if(Button("Nói chuyện • "+actor))Do(adapter.Interact(FullPlaceholderKind.Actor,"npc."+actor,s.Location));
            if(s.Modal==FullCaseModal.Conversation){foreach(var topic in s.GetConversationTopics()){if(Button(topic.CanAsk?topic.Question:"Chủ đề khóa • "+topic.Reason))Do(adapter.Ask(topic.Id));}GUILayout.Label(adapter.Reply);foreach(var permission in s.GetPermissions())if(Button(permission.label))Do(s.TryRequestAccess(permission.id));}
            GUILayout.Label("Tài liệu đã giải • không phải loại ghi nhận",EditorStyles.boldLabel);
            foreach(var reading in s.GetRecordedReadings()){GUILayout.Label(reading.Recipient+" • "+reading.Text,EditorStyles.wordWrappedLabel);GUILayout.Label(reading.Meaning,EditorStyles.wordWrappedLabel);}
            GUILayout.Label("Lời đã nghe • snapshot từ runtime",EditorStyles.boldLabel);
            foreach(var heard in s.GetHeardStatements()){GUILayout.Label(heard.topicId+" • "+heard.answer,EditorStyles.wordWrappedLabel);if(Button("Ghi lời kể • "+heard.id))Do(s.TryRecordStatement(heard.id));}
            GUILayout.Label("Quan sát tách nguồn • chỉ mở khi nguồn đã xem",EditorStyles.boldLabel);
            foreach(var key in new[]{"mechanism.site.clear","hook.site.dust","samples.received","letter.symbols.compared","draft.edge.observed"})if(Button("Ghi • "+key))Do(s.TryRecordSourcedObservation(key));
            foreach(var note in s.GetSourcedNotes())GUILayout.Label(note.Kind+" • "+note.Source+" • "+note.Text+" • "+note.Conditions+" • "+note.Limitations,EditorStyles.wordWrappedLabel);
            GUILayout.Label("Hồ sơ / cầm–đặt",EditorStyles.boldLabel);foreach(var record in s.GetRecords()){GUILayout.BeginHorizontal();if(Button(record.id+" • xem")){selected=record.id;Do(s.TryInspect(record.id));}if(Button("Cầm"))Do(s.TryHold(record.id));GUILayout.EndHorizontal();}
            if(s.HeldEvidenceId!=""){GUILayout.Label("Đang cầm: "+s.HeldEvidenceId);if(Button("Cất"))Do(s.TryStow());if(Button("Đặt bàn thử tại nơi này"))Do(s.TryPlace(s.Location+".table"));}
            if(s.Modal==FullCaseModal.Inspect&&selected!=""){
                GUILayout.Label(s.GetDescription(selected),EditorStyles.wordWrappedLabel);var observationOptions=s.GetObservationOptions(selected);
                for(int j=0;j<observationOptions.Length;j++)if(Button("Ghi mục "+j+" • "+observationOptions[j]))Do(s.TryRecordObservation(selected,j));foreach(var note in s.GetRecordedNotes(selected))GUILayout.Label(note);
            }
            GUILayout.BeginHorizontal();if(Button("Mật mã"))s.SetModal(FullCaseModal.Cipher);if(Button("Đối chiếu"))s.SetModal(FullCaseModal.Comparison);if(Button("Đoạn quay"))s.SetModal(FullCaseModal.Media);if(Button("Bàn dựng"))s.SetModal(FullCaseModal.Reconstruction);if(Button("Kết luận"))s.SetModal(FullCaseModal.Conclusions);GUILayout.EndHorizontal();
            if(s.Modal==FullCaseModal.Media){GUILayout.Label("Clip tạm: "+s.MediaTime.ToString("F1")+" • "+MechanismPresentation.StateWords(MechanismPresentation.At("event",s.MediaTime)));if(Button("Phát"))Do(s.TryPlayMedia());if(Button("Tua về đầu"))Do(s.TrySeekMedia(0));}
            if(s.Modal==FullCaseModal.Reconstruction){foreach(string trial in new[]{"release","help","hold","hook","wedge","after"})if(Button(trial))Do(s.TryStartTrial(trial));GUILayout.Label("Thời gian chỉ chạy khi window có focus, đang xem bàn dựng và không Pause.");GUILayout.Label(s.TrialId+" • "+s.TrialTime.ToString("F1")+" • "+MechanismPresentation.StateWords(s.TrialState));if(Button(s.TrialRunning?"Pause tay":"Chạy tiếp"))s.SetTrialRunning(!s.TrialRunning);if(Button("Bỏ giữ"))Do(s.TryClearHolds());if(Button("Tua hết (không ghi record)"))Do(s.TrySeekTrial(s.TrialDuration));}
            if(s.Modal==FullCaseModal.Cipher){
                foreach(var id in FullInvestigationLetters.Ids)if(s.GetLetter(id).HasValue&&Button("Thư • "+id))letterId=id;
                var view=s.GetLetter(letterId);if(view.HasValue){GUILayout.Label(string.Join(" ",s.GetLetterPoses(letterId)),EditorStyles.wordWrappedLabel);GUILayout.Label(view.Value.Reading,EditorStyles.wordWrappedLabel);for(int j=0;j<26;j++){string key=FullInvestigationLetters.PoseKey((char)('A'+j));mapping.TryGetValue(key,out var prior);string next=Field(key,prior??"");if(next!=(prior??"")){mapping[key]=next;Do(s.TryAssign(key,next));}}if(Button("Ghi bản đọc"))Do(s.TryRecordReading(letterId));for(int level=1;level<=3;level++)if(Button("Tự mở gợi ý "+level))message=string.Join("\n",s.GetHints(letterId,level));}
                foreach(var entry in s.GetHenriLookup())GUILayout.Label(entry.pose+" = "+entry.letter);
                GUILayout.Label(FullInvestigationLetters.Glossary,EditorStyles.wordWrappedLabel);for(int j=0;j<4;j++)reply[j]=Field("Cụm hồi đáp "+(j+1),reply[j]??"");if(Button("Gửi qua kênh đã biết"))Do(s.TrySendReply(reply));
            }
            if(s.Modal==FullCaseModal.Comparison)foreach(var task in s.GetComparisons()){
                GUILayout.Label(task.label+" • feature/rubric TEST ONLY; không chứng nhận C3");if(!choices.TryGetValue(task.id,out var selection))choices[task.id]=selection=new HashSet<string>();
                foreach(var record in s.GetRecords().Where(r=>r.viewed)){bool old=selection.Contains(record.id),next=Toggle(task.id+" • "+record.id,old);if(next)selection.Add(record.id);else selection.Remove(record.id);}
                foreach(var option in task.options)if(Button("Ghi đối chiếu • "+task.id+" • "+option))Do(s.TryCompare(task.id,selection.ToArray(),option));
            }
            if(s.Modal==FullCaseModal.Conclusions){foreach(var draft in s.GetConclusions()){
                var rule=s.GetConclusionChoices().First(r=>r.id==draft.id);GUILayout.Label(rule.label+" • "+(draft.claim==""?"Chưa đủ chứng minh":draft.claim));
                if(Button("Chưa đủ chứng minh • "+rule.id))Do(s.TrySetConclusion(rule.id,"",draft.grounds.ToArray()));foreach(var claim in rule.claims)if(Button(claim+" • "+rule.id))Do(s.TrySetConclusion(rule.id,claim,draft.grounds.ToArray()));
                foreach(var ground in s.GetAvailableGrounds()){bool old=draft.grounds.Any(g=>g.kind==ground.kind&&g.id==ground.id&&g.index==ground.index);bool next=GUILayout.Toggle(old,ground.kind+" • "+ground.id+":"+ground.index);if(next!=old){var grounds=draft.grounds.Where(g=>!(g.kind==ground.kind&&g.id==ground.id&&g.index==ground.index)).ToList();if(next)grounds.Add(ground);Do(s.TrySetConclusion(rule.id,draft.claim,grounds.ToArray()));}}
            }if(Button("Xác nhận trung tính"))Do(s.TryConfirm());if(Button("Chốt hồ sơ"))Do(s.TryFinalize(adapter.Profile));}
            if(s.Location=="station"){
                GUILayout.Label(s.AppointmentRequirement,EditorStyles.wordWrappedLabel);
                if(Button("Tương tác cuộc hẹn • xem tóm tắt"))Do(s.TryOpenEndingSummary());
                if(s.EndingSummaryOpen&&!s.EndingPlaying){foreach(var draft in s.GetConclusions())GUILayout.Label(draft.id+" • "+(draft.claim==""?"Chưa đủ chứng minh":draft.claim));if(Button("Xác nhận bắt đầu trình diễn"))Do(s.TryStartEndingPresentation());}
                if(s.EndingPlaying){foreach(var line in s.GetEndingConsequences())GUILayout.Label(line,EditorStyles.wordWrappedLabel);GUILayout.Label("MÔ PHỎNG TRÌNH DIỄN • chưa có asset/thoại ending sản phẩm. Một chuỗi theo toàn bộ dimension, không phát thưởng.");if(Button("Đã chạy hết trình diễn thử"))Do(s.TryCompleteEndingPresentation(adapter.Profile));}
                if(Button("Chủ động quay lại điều tra • checkpoint trước chốt"))Do(s.TryReturnToInvestigation(adapter.Profile));
                GUILayout.Label("Đã xem trọn • "+s.EndingViewed);
            }
            var ending=s.GetEnding();if(ending.Ending!=FullEnding.None)GUILayout.Label("KẾT QUẢ TEST ONLY • "+ending.Ending+" • mục chưa được chống đỡ: "+string.Join(", ",ending.UnsupportedDimensions)+" • không tự suy tội danh/ý định; Lan và rối vẫn được bảo vệ.",EditorStyles.wordWrappedLabel);
            GUILayout.Space(120);inScroll=false;EditorGUILayout.EndScrollView();
        }
    }
}
