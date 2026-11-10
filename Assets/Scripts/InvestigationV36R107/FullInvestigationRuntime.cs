using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace ConRoi.Investigation.V36.R107
{
    // A bounded case controller. Catalog, save and UI are separate; no scene-name lookup,
    // automatic component startup or authored placeholder data lives in this class.
    public sealed partial class FullInvestigationRuntime
    {
        readonly FullCaseCatalog catalog; readonly string catalogHash;
        FullCaseSave state; string confirmation; bool notifying;
        public event Action Changed;
        public FullCaseModal Modal {get;private set;}
        public bool Paused {get;private set;}
        public string Location=>state.location;
        public bool TestOnly=>catalog.testOnly;
        public string CatalogId=>catalog.id;
        public string HeldEvidenceId=>state.held;
        public object DataContext=>state;
        public bool Finalized=>state.finalized;
        public string LastMessage {get;private set;}="";
        public bool CanMove=>!Paused&&Modal==FullCaseModal.None;
        public string TrialId {get;private set;}="";
        public float TrialTime {get;private set;}
        public float TrialDuration {get;private set;}
        public bool TrialRunning {get;private set;}
        float clearAt=float.PositiveInfinity; bool trialRecorded;
        public GateState TrialState=>TrialTimeline.At(TrialId,TrialTime,clearAt);
        public float MediaTime {get;private set;}
        public bool MediaPlaying {get;private set;}
        public FullInvestigationRuntime(FullCaseCatalog definitions)
        {
            if(definitions==null)throw new ArgumentNullException(nameof(definitions));
            catalog=Copy(definitions);ValidateCatalog();catalogHash=Hash(JsonUtility.ToJson(catalog));
            state=FreshState();
        }
        static T Copy<T>(T value)=>JsonUtility.FromJson<T>(JsonUtility.ToJson(value));
        FullCaseSave FreshState()=>new FullCaseSave{catalogHash=catalogHash,location=catalog.startLocation,
            conclusions=catalog.conclusions.Select(c=>new FullConclusionDraft{id=c.id}).ToList()};
        FullActionResult Result(FullActionResult value){LastMessage=value.Message;return value;}
        FullActionResult Fail(string code,string message)=>Result(FullActionResult.Fail(code,message));
        FullActionResult Done(string message="Đã thực hiện.")=>Result(FullActionResult.Ok(message));
        bool Ready(out FullActionResult error,bool allowFinalized=false)
        {
            error=default;
            if(notifying){error=Fail("busy","Đang xử lý thay đổi trước.");return false;}
            if(Paused){error=Fail("paused","Hãy tiếp tục trò chơi trước khi thao tác.");return false;}
            if(state.finalized&&!allowFinalized){error=Fail("finalized","Hồ sơ đã chốt; khôi phục bản trước chốt để sửa.");return false;}
            return true;
        }
        void Publish(bool progress=true)
        {
            if(progress)confirmation=null;
            if(notifying||Changed==null)return;
            notifying=true;
            try{foreach(Action callback in Changed.GetInvocationList())try{callback();}catch(Exception e){Debug.LogWarning("Full case listener failed: "+e.GetType().Name);}}
            finally{notifying=false;}
        }
        public void SetPaused(bool value){if(notifying)return;Paused=value;confirmation=null;Publish(false);}
        public void SetModal(FullCaseModal value){if(notifying||!Enum.IsDefined(typeof(FullCaseModal),value))return;Modal=value;confirmation=null;Publish(false);}
        public void Cancel(){SetModal(FullCaseModal.None);}
        public FullLocationDefinition[] GetLocations()=>catalog.locations.Select(l=>new FullLocationDefinition{id=l.id,label=l.label}).ToArray();
        public bool CanTravel(string id)=>catalog.locations.Any(l=>l.id==id&&MeetsAll(l.requires));
        public FullActionResult TryTravel(string id)
        {
            if(!Ready(out var e,true))return e;
            if(Modal!=FullCaseModal.None)return Fail("modal","Hãy đóng thao tác hiện tại trước khi chuyển nơi.");
            var location=catalog.locations.FirstOrDefault(l=>l.id==id);
            if(location==null)return Fail("unknown-location","Không có địa điểm này trong tuyến.");
            if(!MeetsAll(location.requires))return Fail("locked","Chưa có đầu mối hoặc điều kiện để tới nơi này.");
            if(state.location==id)return Done();
            state.location=id;Publish();return Done("Đã chuyển địa điểm.");
        }
        FullEvidenceDefinition Evidence(string id)=>catalog.evidence.FirstOrDefault(e=>e.id==id);
        FullEvidenceRecord Record(string id)=>state.evidence.FirstOrDefault(e=>e.id==id);
        public bool HasEvidence(string id)=>Record(id)!=null;
        bool Readable(string id)=>id!="clip"||(state.clipBeforeSeen&&state.clipEventSeen);
        public bool HasRecordedObservation(string id,int index)=>Readable(id)&&Record(id)?.observations.Contains(index)==true;
        public bool IsKnownObservation(string id,int index)=>Evidence(id)!=null&&index>=0&&index<Evidence(id).observations.Length;
        public FullEvidenceDefinition[] GetLocalEvidence()=>catalog.evidence.Where(e=>e.location==Location&&MeetsAll(e.requires)).Select(e=>new FullEvidenceDefinition{id=e.id,label=e.label,location=e.location,portable=e.portable}).ToArray();
        public FullEvidenceRecord[] GetRecords()=>state.evidence.Select(Copy).ToArray();
        public FullActionResult TryCollect(string id)
        {
            if(!Ready(out var e))return e;
            if(Modal!=FullCaseModal.None)return Fail("modal","Hãy đóng thao tác hiện tại trước khi khám vật mới.");
            var definition=Evidence(id);
            if(definition==null)return Fail("unknown-evidence","Không có vật chứng này.");
            if(definition.location!=Location||!MeetsAll(definition.requires))return Fail("locked","Chưa tiếp cận được nguồn này.");
            if(!HasEvidence(id)){state.evidence.Add(new FullEvidenceRecord{id=id,placement=Location,slot="source"});Publish();}
            return Done("Đã thêm mục vào hồ sơ; chưa tự ghi quan sát.");
        }
        public FullActionResult TryInspect(string id)
        {
            if(!Ready(out var e))return e;
            var record=Record(id);if(record==null)return Fail("not-collected","Chưa có mục này trong hồ sơ.");
            record.viewed=true;Modal=FullCaseModal.Inspect;Publish();return Done("Đã mở xem.");
        }
        public string GetDescription(string id)=>Record(id)?.viewed==true?(Readable(id)?Evidence(id).text:"Hãy xem cả trước và trong sự cố trước khi đọc ghi nhận clip."):"";
        public string[] GetRecordedNotes(string id)=>!HasEvidence(id)||!Readable(id)?Array.Empty<string>():Record(id).observations.Select(i=>Evidence(id).observations[i]).ToArray();
        public string GetNoteKind(string id)=>HasEvidence(id)?Evidence(id).kind:"Unclassified";
        public string GetNoteSource(string id)=>HasEvidence(id)?Evidence(id).source:"";
        // Finding the book permits lookup, never fills the player's assignments or scores.
        public CharacterAssignment[] GetHenriLookup()=>Record("henri")?.viewed==true?
            Enumerable.Range(0,26).Select(i=>new CharacterAssignment{pose=FullInvestigationLetters.PoseKey((char)('A'+i)),letter=((char)('A'+i)).ToString()}).ToArray():Array.Empty<CharacterAssignment>();
        public FullActionResult TryRecordObservation(string id,int index)
        {
            if(!Ready(out var e))return e;
            var record=Record(id);
            if(Modal!=FullCaseModal.Inspect||record==null||!record.viewed||!IsKnownObservation(id,index))return Fail("context","Hãy mở mục hợp lệ trước khi ghi.");
            if(!Readable(id))return Fail("unseen","Chưa xem đủ trước và trong sự cố.");
            if(!record.observations.Contains(index)){record.observations.Add(index);Publish();}
            return Done("Đã ghi nhận; không tự xác nhận kết luận.");
        }
        public FullActionResult TryRecordRelation(string first,string second,string note)
        {
            if(!Ready(out var e))return e;
            if(Modal!=FullCaseModal.Comparison||first==second||!HasEvidence(first)||!HasEvidence(second)||string.IsNullOrWhiteSpace(note))return Fail("relation","Cần hai mục đã có và nội dung liên hệ.");
            state.relations.Add(new EvidenceRelation{first=first,second=second,note=note.Trim()});Publish();return Done("Đã ghi giả thuyết; không tự xác nhận sự thật.");
        }
        public FullActionResult TryHold(string id)
        {
            if(!Ready(out var e))return e;
            var r=Record(id);var d=Evidence(id);
            if(Modal!=FullCaseModal.None||r==null||!d.portable||state.held!=""||(r.placement!=Location&&r.placement!="inventory"))return Fail("placement","Không thể cầm mục này trong ngữ cảnh hiện tại.");
            state.held=id;r.placement="held";r.slot="";Publish();return Done("Đang cầm vật.");
        }
        public FullActionResult TryStow()
        {
            if(!Ready(out var e))return e;
            if(state.held=="")return Fail("empty-hand","Không cầm vật nào.");
            var r=Record(state.held);r.placement="inventory";r.slot="";state.held="";Publish();return Done("Đã cất; vật chứng vẫn trong hồ sơ.");
        }
        public FullActionResult TryPlace(string slot)
        {
            if(!Ready(out var e))return e;
            if(Modal!=FullCaseModal.None||state.held==""||string.IsNullOrEmpty(slot)||!slot.StartsWith(Location+".",StringComparison.Ordinal)||!Evidence(state.held).placementSlots.Contains(slot))return Fail("placement","Vị trí đặt chưa hợp lệ; vật vẫn được giữ.");
            var r=Record(state.held);r.placement=Location;r.slot=slot;state.held="";Publish();return Done("Đã đặt ở vị trí hợp lệ.");
        }
        public FullPermissionDefinition[] GetPermissions()=>catalog.permissions.Select(p=>new FullPermissionDefinition{id=p.id,label=p.label}).ToArray();
        public bool HasPermission(string id)=>state.permissions.Contains(id);
        public FullActionResult TryRequestAccess(string id)
        {
            if(!Ready(out var e))return e;
            var definition=catalog.permissions.FirstOrDefault(p=>p.id==id);
            if(definition==null||definition.location!=Location||Modal!=FullCaseModal.Conversation||!MeetsAll(definition.requires))return Fail("permission","Chưa đủ điều kiện nhận quyền truy cập.");
            if(!state.permissions.Contains(id)){state.permissions.Add(id);Publish();}
            return Done("Đã nhận quyền truy cập; chưa tự thu hoặc ghi mẫu.");
        }
        public bool Meets(FullRequirement requirement)=>MeetsIn(state,requirement);
        bool MeetsAll(IEnumerable<FullRequirement> requirements)=>requirements!=null&&requirements.All(Meets);
        public FullLetterView? GetLetter(string id)=>FullInvestigationLetters.Ids.Contains(id)&&Record(id)?.viewed==true?new FullLetterView(id,FullInvestigationLetters.Recipient(id),FullInvestigationLetters.Read(id,state.mapping),state.decoded.Contains(id)):(FullLetterView?)null;
        public string[] GetLetterPoses(string id)=>GetLetter(id).HasValue?FullInvestigationLetters.PoseSequence(id):Array.Empty<string>();
        public FullActionResult TryAssign(string poseKey,string letter)
        {
            if(!Ready(out var e))return e;
            if(Modal!=FullCaseModal.Cipher||!Enumerable.Range(0,26).Select(i=>FullInvestigationLetters.PoseKey((char)('A'+i))).Contains(poseKey)||letter==null)return Fail("mapping","Tư thế hoặc ngữ cảnh chưa hợp lệ.");
            letter=letter.ToUpperInvariant();if(letter.Length>1||letter.Any(c=>c<'A'||c>'Z'))return Fail("mapping","Gán một chữ A–Z hoặc xóa.");
            var item=state.mapping.FirstOrDefault(m=>m.pose==poseKey);if(item==null)state.mapping.Add(new CharacterAssignment{pose=poseKey,letter=letter});else item.letter=letter;
            Publish();return Done("Đã giữ bảng gán; không báo đúng từng chữ.");
        }
        public FullActionResult TryRecordReading(string id)
        {
            if(!Ready(out var e))return e;
            var letter=GetLetter(id);
            if(Modal!=FullCaseModal.Cipher||!letter.HasValue)return Fail("letter","Chưa mở thư này.");
            if(letter.Value.Reading.Replace(" • "," ")!=FullInvestigationLetters.Plain(id))return Fail("reading","Đã giữ bản đọc thử; chưa ghi được nội dung hoàn chỉnh.");
            if(!state.decoded.Contains(id)){state.decoded.Add(id);Publish();}
            return Done("Đã ghi bản đọc; không tự xác nhận người gửi hoặc kết luận.");
        }
        public string[] GetHints(string letterId,int level)
        {
            if(!GetLetter(letterId).HasValue||level<1||level>3)return Array.Empty<string>();
            return new[]{"So các tư thế lặp và dấu ngắt từ trên thư đã mở.","Một tư thế giữ cùng chữ; so khẩu lệnh STAND BY đã giới thiệu trong bảng tập.","Tra bảng Henri nếu đã tìm được, rồi tự sửa bảng gán; không có đèn đúng từng chữ."}.Take(level).ToArray();
        }
        public FullActionResult TrySendReply(string[] orderedPhrases)
        {
            if(!Ready(out var e))return e;
            if(Modal!=FullCaseModal.Cipher||!MeetsAll(catalog.replyChannelRequirements)||catalog.replyChannelRequirements.Length==0)return Fail("channel","Chưa có căn cứ về kênh nhận/trả giấy.");
            if(orderedPhrases==null||!orderedPhrases.SequenceEqual(FullInvestigationLetters.ReplyPhrases))return Fail("reply","Các cụm hồi đáp chưa đúng thứ tự; có thể sửa lại.");
            if(!state.replySent){state.replySent=true;Publish();}
            return Done("Đã gửi hồi đáp qua kênh đã biết; không có đồng hồ đếm ngược.");
        }
        public FullComparisonDefinition[] GetComparisons()=>catalog.comparisons.Where(c=>MeetsAll(c.requires)).Select(c=>new FullComparisonDefinition{id=c.id,label=c.label,evidenceIds=(string[])c.evidenceIds.Clone(),options=(string[])c.options.Clone()}).ToArray();
        bool ComparisonAccepted(FullComparisonAttempt a)
        {
            var d=catalog.comparisons.FirstOrDefault(c=>c.id==a.id);
            return d!=null&&a.option==d.expectedOption&&a.evidenceIds.OrderBy(x=>x,StringComparer.Ordinal).SequenceEqual(d.evidenceIds.OrderBy(x=>x,StringComparer.Ordinal))&&MeetsAll(d.requires);
        }
        public FullActionResult TryCompare(string taskId,string[] evidenceIds,string option)
        {
            if(!Ready(out var e))return e;
            var d=catalog.comparisons.FirstOrDefault(c=>c.id==taskId);
            if(Modal!=FullCaseModal.Comparison||d==null||!MeetsAll(d.requires)||evidenceIds==null||evidenceIds.Length!=d.evidenceIds.Length||evidenceIds.Distinct().Count()!=evidenceIds.Length||evidenceIds.Any(id=>Record(id)?.viewed!=true)||!d.options.Contains(option))return Fail("comparison","Chưa đủ nguồn đã xem hoặc lựa chọn đối chiếu chưa hợp lệ.");
            var attempt=new FullComparisonAttempt{id=taskId,evidenceIds=(string[])evidenceIds.Clone(),option=option};
            var old=state.comparisons.FirstOrDefault(a=>a.id==taskId);if(old!=null)state.comparisons.Remove(old);state.comparisons.Add(attempt);
            Publish();return Done("Đã ghi kết quả đối chiếu để tiếp tục xem xét; không tự nhận diện hung thủ.");
        }
        // Citations expose player records, not the grading outcome of an attempted comparison.
        public FullRequirement[] GetAvailableGrounds()
        {
            var result=new List<FullRequirement>();
            foreach(var record in state.evidence)if(Readable(record.id))foreach(int index in record.observations)result.Add(new FullRequirement(FullRequirementKind.Recorded,record.id,index));
            foreach(var attempt in state.comparisons)result.Add(new FullRequirement(FullRequirementKind.Comparison,attempt.id));
            foreach(var letter in state.decoded)result.Add(new FullRequirement(FullRequirementKind.Decoded,letter));
            foreach(var trial in state.trials.GroupBy(t=>t.id).Select(g=>g.First()))result.Add(new FullRequirement(FullRequirementKind.TrialRecorded,trial.id));
            return result.ToArray();
        }
        public FullConclusionDraft[] GetConclusions()=>state.conclusions.Select(Copy).ToArray();
        public FullConclusionRule[] GetConclusionChoices()=>catalog.conclusions.Select(c=>new FullConclusionRule{id=c.id,label=c.label,claims=(string[])c.claims.Clone()}).ToArray();
        public FullActionResult TrySetConclusion(string id,string claim,FullRequirement[] grounds)
        {
            if(!Ready(out var e))return e;
            var rule=catalog.conclusions.FirstOrDefault(c=>c.id==id);
            if(Modal!=FullCaseModal.Conclusions||rule==null||claim==null||(claim!=""&&!rule.claims.Contains(claim))||grounds==null||grounds.Any(g=>!ValidRequirement(g)||!Citable(state,g)))return Fail("conclusion","Lựa chọn hoặc căn cứ chưa hợp lệ/được ghi.");
            var draft=state.conclusions.First(c=>c.id==id);draft.claim=claim;draft.grounds=grounds.Select(Copy).ToList();Publish();return Done("Đã giữ lựa chọn và căn cứ; chưa đánh giá đúng/sai.");
        }
        string Signature()=>JsonUtility.ToJson(state);
        public bool Confirmed=>Modal==FullCaseModal.Conclusions&&!Paused&&confirmation!=null&&confirmation==Signature();
        public FullActionResult TryConfirm()
        {
            if(!Ready(out var e))return e;
            if(Modal!=FullCaseModal.Conclusions)return Fail("context","Hãy mở hồ sơ kết luận.");
            confirmation=Signature();return Done("Đã xác nhận bản ghi hiện tại, kể cả chưa đủ chứng minh.");
        }
        internal bool Assess(FullCaseSave candidate,out FullEnding ending,out List<string> unsupported)
        {
            ending=FullEnding.None;unsupported=new List<string>();
            if(catalog.conclusions.Any(r=>candidate.conclusions.Any(d=>d.id==r.id&&d.claim!="")&&(!r.reviewed||r.alternatives.Length==0)))return false;
            if(candidate.conclusions.Any(d=>d.id=="fall"&&d.claim!="")&&!catalog.fallChainReviewed)return false;
            bool open=false;
            foreach(var rule in catalog.conclusions){
                var draft=candidate.conclusions.First(c=>c.id==rule.id);
                if(draft.claim==""){open=true;continue;}
                bool supported=draft.claim==rule.correctClaim&&rule.alternatives.Any(set=>set.requires.All(need=>draft.grounds.Any(g=>Same(g,need))&&MeetsIn(candidate,need)));
                if(!supported)unsupported.Add(rule.id);
            }
            ending=unsupported.Count>0?FullEnding.Unsupported:open?FullEnding.Open:FullEnding.Complete;return true;
        }
        static bool Same(FullRequirement a,FullRequirement b)=>a.kind==b.kind&&a.id==b.id&&a.index==b.index;
        public FullEndingView GetEnding()=>state.finalized?new FullEndingView(state.ending,state.unsupportedDimensions.ToArray()):new FullEndingView(FullEnding.None,Array.Empty<string>());
        public FullActionResult TryStartTrial(string id)
        {
            if(!Ready(out var e))return e;
            if(Modal!=FullCaseModal.Reconstruction||!new[]{"release","help","hold","hook","wedge","after"}.Contains(id))return Fail("trial","Ca thử hoặc ngữ cảnh chưa hợp lệ.");
            TrialId=id;TrialTime=0;clearAt=float.PositiveInfinity;TrialDuration=TrialTimeline.End(id,clearAt);TrialRunning=true;trialRecorded=false;return Done("Đang thử trên mô hình, không phải tua lại sự thật.");
        }
        public void SetTrialRunning(bool running){if(!notifying&&!Finalized&&!Paused&&Modal==FullCaseModal.Reconstruction&&TrialId!=""&&TrialTime<TrialDuration)TrialRunning=running;}
        public FullActionResult TrySeekTrial(float time)
        {
            if(!Ready(out var e))return e;
            if(Modal!=FullCaseModal.Reconstruction||TrialId==""||!Finite(time))return Fail("trial","Chưa có ca thử/thời điểm hợp lệ.");
            TrialTime=Mathf.Clamp(time,0,TrialDuration);TrialRunning=false;return Done("Đã tua; chưa tạo bản ghi thử mới.");
        }
        public FullActionResult TryClearHolds()
        {
            if(!Ready(out var e))return e;var s=TrialState;
            if(Modal!=FullCaseModal.Reconstruction||TrialId==""||!float.IsPositiveInfinity(clearAt)||TrialTime<TrialTimeline.LastAction(TrialId)||!(s.bodyForce||s.hook||s.wedge))return Fail("trial","Chưa thể bỏ điều kiện giữ.");
            clearAt=TrialTime;TrialDuration=TrialTimeline.End(TrialId,clearAt);trialRecorded=false;TrialRunning=true;return Done("Đã bỏ giữ; tiếp tục xem cơ cấu hồi.");
        }
        public FullActionResult TryPlayMedia()
        {
            if(!Ready(out var e))return e;
            if(Modal!=FullCaseModal.Media||!HasEvidence("clip"))return Fail("media","Chưa mở đoạn quay.");
            MediaPlaying=true;return Done();
        }
        public FullActionResult TrySeekMedia(float time)
        {
            if(!Ready(out var e))return e;
            if(Modal!=FullCaseModal.Media||!HasEvidence("clip")||!Finite(time))return Fail("media","Thời điểm/ngữ cảnh chưa hợp lệ.");
            MediaTime=Mathf.Clamp(time,0,MechanismPresentation.ClipLength);return Done("Đã tua; không tự ghi quan sát.");
        }
        static bool Finite(float f)=>!float.IsNaN(f)&&!float.IsInfinity(f);
        // The adapter supplies visible-playback delta only; never elapsed time while away.
        public void Advance(float dt)
        {
            if(Finalized||Paused||!Finite(dt)||dt<=0||notifying)return;
            if(Modal==FullCaseModal.Media&&MediaPlaying){
                float a=MediaTime,b=Mathf.Min(MechanismPresentation.ClipLength,a+dt);MediaTime=b;
                state.clipBeforeExposure+=Mathf.Max(0,Mathf.Min(b,3.8f)-Mathf.Max(a,.5f));state.clipEventExposure+=Mathf.Max(0,Mathf.Min(b,6.2f)-Mathf.Max(a,4));
                state.clipBeforeSeen|=state.clipBeforeExposure>=.6f;state.clipEventSeen|=state.clipEventExposure>=.8f;
                if(b>=MechanismPresentation.ClipLength)MediaPlaying=false;confirmation=null;
            }
            if(Modal==FullCaseModal.Reconstruction&&TrialRunning){
                TrialTime=Mathf.Min(TrialDuration,TrialTime+dt);
                if(TrialTime>=TrialDuration){TrialRunning=false;if(!trialRecorded&&TrialTimeline.Settled(TrialState)){
                    state.trials.Add(new TrialRecord{id=TrialId+(float.IsPositiveInfinity(clearAt)?"":"/clear-holds"),report=MechanismPresentation.StateWords(TrialState)+". Mô hình không xác định người thao tác hay ý định.",returnSeconds=-1});trialRecorded=true;Publish();
                }}
            }
        }
        void ResetTransient(){Modal=FullCaseModal.None;Paused=false;confirmation=null;TrialId="";TrialTime=0;TrialDuration=0;TrialRunning=false;trialRecorded=false;clearAt=float.PositiveInfinity;MediaTime=0;MediaPlaying=false;}
        public string ExportStateJson()=>JsonUtility.ToJson(state,true);
    }
}
