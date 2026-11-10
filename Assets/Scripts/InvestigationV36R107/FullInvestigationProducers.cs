using System;
using System.Linq;
using System.Collections.Generic;

namespace ConRoi.Investigation.V36.R107
{
    [Serializable] public sealed class FullHeardStatement
    {
        public string id,topicId,participantId,question,answer; public int version;
    }
    public readonly struct FullSourcedNote
    {
        public readonly string Id,Text,Source,Conditions,Limitations;
        public readonly NoteProvenanceKind Kind;
        internal FullSourcedNote(string id,string text,string source,string conditions,string limits,NoteProvenanceKind kind)
        {Id=id;Text=text;Source=source;Conditions=conditions;Limitations=limits;Kind=kind;}
    }
    // Approved facts/roles from GDD and project-owner decisions, not the Editor test catalog.
    // These five bounded topics are not a replacement for the remaining NPC scripts.
    internal static class FullApprovedStatements
    {
        internal sealed class Definition
        {
            internal string Id,Location; internal InterviewTopicDefinition Topic; internal FullRequirement[] Requires;
            internal Definition(string statement,string actor,string topic,string location,string q,string a,params FullRequirement[] needs)
            {Id=statement;Location=location;Topic=new InterviewTopicDefinition(topic,actor,q,a);Requires=needs;}
        }
        internal static readonly Definition[] All={
            new Definition("ha.receipt.facts","npc.ha","ha.cargo.receipt","market","Phiếu này cho biết gì về ba gói hàng?","Phiếu ghi ba gói và ngày giao, không ghi bên trong.",new FullRequirement(FullRequirementKind.Inspected,"receipt")),
            new Definition("ha.channel.paper","npc.ha","ha.channel.paper","market","Tôi gửi lời trả lại bằng cách nào?","Gửi giấy trả lời qua quầy này."),
            new Definition("mai.direction.theatre","npc.mai","mai.direction.theatre","school","Tôi tìm sổ Henri ở đâu?","Sổ Henri ở hậu đài nhà hát."),
            new Definition("phuong.direction.homestay","npc.phuong","phuong.direction.homestay","office","Tôi kiểm tra nơi Khải lưu trú ở đâu?","Anh kiểm tra đầu mối lưu trú tại homestay."),
            new Definition("phuong.samples.origin","npc.phuong","phuong.samples.origin","office","Nguồn của các mẫu đối chiếu là đâu?","Mẫu áo và giày được thu tại phòng Khải để đối chiếu với dấu vết ở xưởng.",new FullRequirement(FullRequirementKind.Collected,"samples")),
            new Definition("bao.direction.lan","npc.bao","bao.direction.lan","lake","Tôi tìm bà Lan ở đâu?","Bà Lan ở khu vườn có hàng rào kín.",new FullRequirement(FullRequirementKind.Decoded,"letter.bao"))
        };
        internal static Definition Find(string id)=>All.FirstOrDefault(d=>d.Id==id);
    }
    public sealed partial class FullInvestigationRuntime
    {
        string conversation="";
        public bool HasHeardStatement(string id)=>state.heardStatements.Any(h=>h.id==id);
        public FullHeardStatement[] GetHeardStatements()=>state.heardStatements.Select(Copy).ToArray();
        public FullHeardStatement[] GetRecordedStatements()=>state.heardStatements.Where(h=>state.recordedStatements.Contains(h.id)).Select(Copy).ToArray();
        public FullActionResult TryBeginConversation(string participantId)
        {
            if(!Ready(out var e,true))return e;
            if(Modal!=FullCaseModal.None||!FullApprovedStatements.All.Any(d=>d.Topic.ParticipantId==participantId&&d.Location==Location))return Fail("conversation","Không thể bắt đầu trao đổi ở đây.");
            conversation=participantId;Modal=FullCaseModal.Conversation;Publish(false);return Done("Đã mở trao đổi.");
        }
        public InterviewTopicState[] GetConversationTopics()=>FullApprovedStatements.All.Where(d=>d.Topic.ParticipantId==conversation).Select(d=>{
            bool allowed=!Paused&&Modal==FullCaseModal.Conversation&&d.Location==Location&&d.Requires.All(Meets);
            return new InterviewTopicState(d.Topic.Id,allowed?d.Topic.Question:"",allowed?InterviewTopicAvailability.Available:InterviewTopicAvailability.Locked,allowed?"":"Chưa đủ điều kiện hoặc đã rời cuộc trao đổi.");}).ToArray();
        public FullActionResult TryAskTopic(string topicId,out InterviewReply reply)
        {
            reply=default;if(!Ready(out var e,true))return e;
            var d=FullApprovedStatements.All.FirstOrDefault(t=>t.Topic.Id==topicId&&t.Topic.ParticipantId==conversation);
            if(d==null||Modal!=FullCaseModal.Conversation||d.Location!=Location||!d.Requires.All(Meets))return Fail("topic-locked","Chủ đề không tồn tại hoặc chưa mở trong cuộc trao đổi.");
            var heard=state.heardStatements.FirstOrDefault(h=>h.id==d.Id);
            if(heard==null){heard=new FullHeardStatement{id=d.Id,topicId=d.Topic.Id,participantId=d.Topic.ParticipantId,question=d.Topic.Question,answer=d.Topic.Answer,version=1};state.heardStatements.Add(heard);Publish();}
            // The first heard snapshot is stable even if an authored script changes later.
            reply=new InterviewReply(new InterviewTopicDefinition(heard.topicId,heard.participantId,heard.question,heard.answer));
            return Done("Đã nghe lời kể; chưa ghi vào hồ sơ và chưa xác nhận là sự thật.");
        }
        internal static readonly FullEvidenceDefinition ChannelReceipt=new FullEvidenceDefinition{
            id="receipt.channel",location="market",label="Phiếu nhận/trả giấy",source="Quầy Hà • phiếu nhận riêng",
            text="Phiếu nhận/trả giấy tại quầy Hà. Hồ sơ liên kết với phiếu vận chuyển M2; không xác nhận hàng bên trong ba gói.",
            observations=new[]{"Phiếu thuộc điểm nhận/trả giấy ở quầy Hà."},portable=true,placementSlots=new[]{"market.table"},
            requires=new[]{new FullRequirement(FullRequirementKind.Heard,"ha.channel.paper")}};
        public string[] GetRelatedEvidenceIds(string id)=>id=="receipt.channel"&&Record(id)?.viewed==true&&Evidence("M2")!=null?new[]{"M2"}:Array.Empty<string>();
        public FullActionResult TryRecordStatement(string statementId)
        {
            if(!Ready(out var e))return e;
            if(!HasHeardStatement(statementId))return Fail("unheard","Chưa nghe lời kể này.");
            if(!state.recordedStatements.Contains(statementId)){state.recordedStatements.Add(statementId);Publish();}
            return Done("Đã ghi lời kể, không tự xác nhận nội dung.");
        }
        bool LocationOpen(FullLocationDefinition l)
        {
            if(state.finalized&&l.id=="market")return true; // Always permit the return to the introduced reply channel.
            string heard=l.id=="theatre"?"mai.direction.theatre":l.id=="homestay"?"phuong.direction.homestay":l.id=="garden"?"bao.direction.lan":null;
            if(heard!=null&&!HasHeardStatement(heard))return false;
            return l.requires.Where(r=>!(l.id=="theatre"&&r.id=="lead.theatre")&&!(l.id=="homestay"&&r.id=="lead.homestay")&&!(l.id=="garden"&&r.kind==FullRequirementKind.Recorded&&r.id=="M3")).All(Meets);
        }
        public bool CanBeginAppointment=>state.finalized&&state.replySent&&state.replyProducerVerified;
        public bool HasSentReply=>state.replySent;
        public string AppointmentRequirement=>!state.finalized?"Cần chốt hồ sơ trước cuộc hẹn.":!state.replyProducerVerified?"Cần gửi hồi đáp qua quầy Hà; có thể quay lại Hà.":"Có thể bắt đầu cuộc hẹn tại ga.";

        public bool EndingSummaryOpen{get;private set;}
        public bool EndingPlaying{get;private set;}
        public bool EndingViewed=>state.endingViewed;
        public string[] GetEndingConsequences()
        {
            if(!state.finalized)return Array.Empty<string>();
            var lines=new List<string>();
            lines.Add("Lan được bảo vệ và bộ rối được giữ an toàn theo nguy cơ đã biết.");
            if(state.ending==FullEnding.Complete)lines.Add("Ngăn cuộc ép giao rối; hồ sơ ghi chuỗi thao tác gây ngã và dàn dựng để tiếp tục xử lý, không tự tuyên tội danh hoặc bản án.");
            else if(state.ending==FullEnding.Open)lines.Add("Điều tra còn mở: không bắt ai vì điều chưa đủ chứng minh; tiếp tục theo dõi các câu hỏi chưa khép.");
            else foreach(var id in state.unsupportedDimensions){
                if(id=="sender")lines.Add("Khẳng định về người gửi/yêu cầu thư không được chống đỡ: hướng điều tra và niềm tin bị tổn hại.");
                else if(id=="presence")lines.Add("Khẳng định hiện diện/dàn dựng không được chống đỡ; không lấy riêng cơ hội làm chứng minh gây ngã.");
                else if(id=="fall")lines.Add("Khẳng định về vai trò gây ngã không được chống đỡ: cáo buộc dựa trên phần này bị bác/đình lại, gây tổn hại người bị nghi; vụ án chưa khép.");
                else if(id=="motive")lines.Add("Khẳng định động cơ không được chống đỡ; lỗi ở mục này không tự là căn cứ bắt người về cú ngã.");
            }
            lines.Add("Trình diễn không bổ sung chứng cứ hồi tố để hợp thức hóa hồ sơ và không suy ý định gây chết.");
            return lines.ToArray();
        }
        public FullActionResult TryOpenEndingSummary()
        {
            if(!Ready(out var e,true))return e;
            if(Location!="station"||Modal!=FullCaseModal.None||!CanBeginAppointment)return Fail("appointment",AppointmentRequirement);
            EndingSummaryOpen=true;Modal=FullCaseModal.Ending;Publish(false);return Done("Xem tóm tắt hồ sơ đã chốt; xác nhận để bắt đầu trình diễn.");
        }
        public FullActionResult TryStartEndingPresentation()
        {
            if(!Ready(out var e,true))return e;
            if(!EndingSummaryOpen||EndingPlaying||Modal!=FullCaseModal.Ending||Location!="station"||!CanBeginAppointment)return Fail("ending-context","Cần tương tác và xác nhận tóm tắt tại ga.");
            EndingPlaying=true;Publish(false);return Done("Bắt đầu một chuỗi trình diễn theo toàn bộ kết luận, không phát thưởng hoặc tạo checkpoint.");
        }
        public FullActionResult TryCompleteEndingPresentation(string profile)
        {
            if(!Ready(out var e,true))return e;
            if(!EndingPlaying||Modal!=FullCaseModal.Ending||Location!="station")return Fail("ending-context","Chưa có trình diễn đang chạy.");
            try{
                if(!state.endingViewed){var pending=Copy(state);pending.endingViewed=true;ValidateState(pending);InvestigationSaveFile.Write(SavePathFor(profile),UnityEngine.JsonUtility.ToJson(pending,true));state=pending;}
                EndingPlaying=false;EndingSummaryOpen=false;Modal=FullCaseModal.None;Publish(false);return Done("Đã xem hết trình diễn; xem lại không đổi hồ sơ hoặc checkpoint.");
            }catch(Exception){return Fail("write","Chưa lưu được trạng thái đã xem; không đánh dấu hoàn tất.");}
        }
        public FullActionResult TryReturnToInvestigation(string profile)
        {
            if(Paused||EndingPlaying||!state.finalized)return Fail("ending-context","Hãy thoát trình diễn trước khi chủ động khôi phục hồ sơ trước chốt.");
            return TryLoad(profile,true);
        }

        internal static readonly string[] SourcedKeys={"mechanism.site.clear","hook.site.dust","samples.received","letter.symbols.compared","draft.edge.observed"};
        public FullActionResult TryRecordSourcedObservation(string noteId)
        {
            if(!Ready(out var e))return e;
            string source=noteId==SourcedKeys[0]?"mechanism":noteId==SourcedKeys[1]?"hook":noteId==SourcedKeys[2]?"samples":noteId==SourcedKeys[3]?"letter":noteId==SourcedKeys[4]?"draft":null;
            var definition=source==null?null:Evidence(source);
            if(definition==null||(noteId==SourcedKeys[2]&&!MeetsAll(definition.requires))||Modal!=FullCaseModal.Inspect||Record(source)?.viewed!=true||(noteId==SourcedKeys[3]&&Record("henri")?.viewed!=true))return Fail("source-unseen","Cần mở xem các nguồn tương ứng trước khi ghi.");
            if(!state.sourcedNotes.Contains(noteId)){state.sourcedNotes.Add(noteId);Publish();}
            return Done("Đã ghi quan sát có nguồn riêng; dữ liệu legacy không được đổi nghĩa.");
        }
        public FullSourcedNote[] GetSourcedNotes()
        {
            var notes=new List<FullSourcedNote>();
            foreach(var id in state.sourcedNotes){
                string text=id==SourcedKeys[0]?"Không thấy vật chèn trong các khe đã khám.":id==SourcedKeys[1]?"Lớp bụi trên móc còn liền trước khi thử.":id==SourcedKeys[2]?"Đã trực tiếp nhận mẫu để đối chiếu.":id==SourcedKeys[3]?"Hình tư thế trên dải thư tương ứng hình đã xem trong sổ.":"Mép giấy và các nét thử nhìn được trên mẫu.";
                string src=id==SourcedKeys[0]?"Cơ cấu thật • khe đã khám":id==SourcedKeys[1]?"Hiện trường • móc M":id==SourcedKeys[2]?"Thao tác nhận mẫu":id==SourcedKeys[3]?"Dải thư Hà và sổ Henri":"Giấy mẫu Hà • mép giấy";
                notes.Add(new FullSourcedNote(id,text,src,"Đã mở xem nguồn tương ứng","Không xác nhận danh tính, thời điểm hoặc sự kiện vượt điều đã thấy.",NoteProvenanceKind.Observation));
            }
            foreach(var h in GetRecordedStatements())notes.Add(new FullSourcedNote(h.id,h.answer,"Lời kể • "+h.participantId,"Đã nghe và chủ động ghi • bản "+h.version,"Lời kể không tự là sự thật đã chứng minh.",NoteProvenanceKind.Statement));
            for(int i=0;i<state.relations.Count;i++){var rel=state.relations[i];notes.Add(new FullSourcedNote("relation:"+i,rel.note,"Người chơi","Liên hệ "+rel.first+" / "+rel.second,"Giả thuyết không tự xác nhận quan hệ.",NoteProvenanceKind.Hypothesis));}
            for(int i=0;i<state.trials.Count;i++){var t=state.trials[i];notes.Add(new FullSourcedNote("trial:"+t.id+":"+i,t.report,"Mô hình dựng lại","Ca "+t.id+(t.returnSeconds<0?" • thời gian hồi chưa được đo trong record":" • thời gian hồi "+t.returnSeconds),"Quan sát mô hình, không phải sự kiện hiện trường đã chứng minh hoặc xác nhận vật lý thật.",NoteProvenanceKind.Observation));}
            return notes.ToArray();
        }
    }
}
