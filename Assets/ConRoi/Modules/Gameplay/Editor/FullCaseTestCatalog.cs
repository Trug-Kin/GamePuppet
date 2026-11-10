using System;
using System.Linq;
using System.Collections.Generic;
using ConRoi.Investigation.V36.R107;

namespace ConRoi.Gameplay.EditorChecks
{
    // TEST ONLY: route gates, specimen keys, equivalence sets and conversations are
    // calibration data. GDD facts/letter texts are referenced, not new official dialogue.
    public static class FullCaseTestCatalog
    {
        public static FullRequirement R(FullRequirementKind kind,string id,int index=-1)=>new FullRequirement(kind,id,index);
        public static FullRequirement Note(string id,int index=0)=>R(FullRequirementKind.Recorded,id,index);
        static FullLocationDefinition Location(string id,string label,params FullRequirement[] needs)=>new FullLocationDefinition{id=id,label="TEST ONLY • "+label,requires=needs};
        static FullEvidenceDefinition Item(string id,string location,string text,bool portable=true)=>new FullEvidenceDefinition{id=id,location=location,label="TEST ONLY • "+id,source="TEST ONLY: nguồn mẫu theo GDD, chưa là bản ghi/thoại sản phẩm",text="TEST ONLY: "+text,observations=new[]{"TEST ONLY: ghi nhận mẫu "+id},portable=portable,placementSlots=portable?new[]{location+".table"}:new string[0]};
        static FullComparisonDefinition Task(string id,string[] evidence,params FullRequirement[] needs)=>new FullComparisonDefinition{id=id,label="TEST ONLY • "+id,evidenceIds=evidence,requires=needs,options=new[]{"test.feature.match","test.feature.near"},expectedOption="test.feature.match"};
        public static FullCaseCatalog Create(bool reviewedTestRubric=true)
        {
            var locations=new[]{Location("office","Văn phòng"),Location("workshop","Xưởng"),Location("lake","Hồ/cầu",Note("slide")),Location("market","Chợ",Note("slide")),Location("school","Trường/lớp 203",Note("slide")),Location("slope","Dốc/nhà Tùng",Note("slide")),Location("theatre","Nhà hát",R(FullRequirementKind.Collected,"lead.theatre")),Location("homestay","Homestay",R(FullRequirementKind.Collected,"lead.homestay")),Location("garden","Vườn Lan",Note("M3")),Location("station","Ga",R(FullRequirementKind.Finalized,""))};
            var evidence=new List<FullEvidenceDefinition>();
            foreach(var old in InvestigationContent.Evidence){
                string location=new[]{"ha","letter","receipt","draft","decoded"}.Contains(old.id)?"market":new[]{"samples","coat","shoe"}.Contains(old.id)?"homestay":"workshop";
                if(old.id=="henri")location="theatre";
                var d=Item(old.id,location,"Mục prototype giữ ID; hình/nguồn chính thức dùng báo cáo gói gốc.",!new[]{"slide","shoeprint","mechanism","hook","khai","ha"}.Contains(old.id));
                d.observations=old.observations.Select((_,i)=>"TEST ONLY: "+old.id+":"+i).ToArray();
                if(new[]{"coat","shoe","samples"}.Contains(old.id))d.requires=new[]{R(FullRequirementKind.Permission,"test.samples")};
                evidence.Add(d);
            }
            evidence.AddRange(new[]{Item("letter.bao","lake","Thư Bảo: dữ liệu tư thế dùng đúng bản rõ GDD5.2."),Item("letter.mai","school","Thư Mai: GDD5.2."),Item("letter.tung","slope","Thư Tùng: GDD5.2."),Item("letter.lan","garden","Thư Lan: GDD5.2, không timer thật."),Item("lead.theatre","school","Đầu mối thử đi nhà hát; gate cần nội dung duyệt."),Item("lead.homestay","slope","Đầu mối thử đi homestay; gate cần nội dung duyệt."),Item("M1","theatre","GDD3.7: sổ Henri/bộ rối/Kho Sáu; không tự nói đã bán."),Item("M2","market","GDD3.7: phiếu ba gói; ngày khớp cần nội dung/asset, không tự nói hàng trong thùng."),Item("M3","school","GDD3.7: biên bản quy lỗi/đuổi; không tự chứng minh oan."),Item("M4","garden","GDD3.7: lời Lan trước chốt; chưa có thoại chính thức."),Item("M5","school","GDD3.7: nháp/bản dịch thỏa thuận; chưa xác nhận thanh toán."),Item("M6","homestay","GDD3.7: sổ phác/ý bù mười hai năm; không chứng minh gây ngã."),Item("paper.khai","homestay","Mẫu nháp/giấy để kiểm bài ghép; không đổi draft Hà thành nháp Khải."),Item("log.homestay","homestay","GDD3.6: mốc giao/nhận chìa, không chứng minh mọi phút.")});
            var tasks=new[]{Task("paper",new[]{"letter.bao","paper.khai"},Note("letter.bao"),Note("paper.khai")),Task("cargo",new[]{"M2","M5","r12"},Note("M2"),Note("M5"),Note("r12")),Task("staging",new[]{"r03","r12","clip"},Note("r03"),Note("r12"),Note("clip")),Task("shoe",new[]{"shoeprint","shoe"},Note("shoeprint"),Note("shoe")),Task("fabric",new[]{"fabric","coat"},Note("fabric"),Note("coat")),Task("counter",new[]{"mechanism","hook","clip"},Note("mechanism"),Note("hook"),Note("clip",1),R(FullRequirementKind.TrialRecorded,"release"),R(FullRequirementKind.TrialRecorded,"help"),R(FullRequirementKind.TrialRecorded,"hook/clear-holds"))};
            var sender=new[]{R(FullRequirementKind.Comparison,"paper"),Note("paper.khai"),R(FullRequirementKind.Decoded,"letter.bao")};
            var presence=new[]{Note("slide"),R(FullRequirementKind.Comparison,"shoe"),R(FullRequirementKind.Comparison,"staging")};
            var fall=new[]{Note("mechanism"),Note("hook"),Note("clip"),Note("clip",1),R(FullRequirementKind.Comparison,"fabric"),R(FullRequirementKind.Comparison,"shoe"),R(FullRequirementKind.Comparison,"staging"),R(FullRequirementKind.Comparison,"counter")};
            var motive=new[]{Note("M1"),Note("M3"),Note("M5"),Note("M6")};
            FullConclusionRule Rule(string id,FullRequirement[] support)=>new FullConclusionRule{id=id,label="TEST ONLY • "+id,claims=new[]{"test.gdd-claim","test.alternative-claim"},correctClaim="test.gdd-claim",reviewed=reviewedTestRubric,alternatives=new[]{new FullSupportSet{requires=support}}};
            var rules=new[]{Rule("sender",sender),Rule("presence",presence),Rule("fall",fall),Rule("motive",motive)};
            rules[0].alternatives=rules[0].alternatives.Concat(new[]{new FullSupportSet{requires=new[]{R(FullRequirementKind.Comparison,"paper"),Note("paper.khai"),R(FullRequirementKind.Decoded,"letter")}}}).ToArray();
            return new FullCaseCatalog{id="test.only.fullcase",revision="1",testOnly=true,startLocation="office",locations=locations,evidence=evidence.ToArray(),comparisons=tasks,conclusions=rules,fallChainReviewed=reviewedTestRubric,permissions=new[]{new FullPermissionDefinition{id="test.samples",location="office",label="TEST ONLY • Phương/quyền truy cập mẫu",requires=new[]{R(FullRequirementKind.Collected,"khai")}}},replyChannelRequirements=new[]{Note("ha",1)}};
        }
        public static InterviewTopicDefinition[] Topics()=>new[]{"bao","ha","mai","tung","lan","khai","phuong","host"}.Select(id=>new InterviewTopicDefinition("test.full.topic."+id,"test.full.actor."+id,"TEST ONLY: hỏi lại nguồn "+id+"?","TEST ONLY: phản hồi cơ chế, không phải thoại chính thức.",new InterviewRequirement(InterviewRequirementKind.RecordedObservation,id=="lan"?"M4":id=="mai"?"M3":id=="host"?"log.homestay":id=="bao"?"letter.bao":id=="tung"?"letter.tung":id=="phuong"?"khai":id,0))).ToArray();
    }
}
