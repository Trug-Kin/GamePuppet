using System;
using System.Linq;
using ConRoi.Investigation.V36.R107;
using ConRoi.Gameplay.TestOnly;
namespace ConRoi.Gameplay.EditorChecks {
// TEST ONLY presentation queries. No progress setters, gate overrides or authored dialogue.
public static class FullCasePlayGuide {
 public static string ActorAt(string location)=>location=="office"?"phuong":location=="market"?"ha":location=="school"?"mai":location=="lake"?"bao":null;
 public static string Person(string id)=>id=="npc.phuong"?"Phương":id=="npc.khai"?"Khải":id=="npc.ha"?"Hà":id=="npc.mai"?"Mai":id=="npc.bao"?"Bảo":"Nhân vật chưa có hội thoại";
 public static string Place(string id)=>id=="office"?"Văn phòng":id=="workshop"?"Xưởng":id=="market"?"Chợ":id=="school"?"Trường":id=="lake"?"Hồ":id=="slope"?"Dốc":id=="theatre"?"Nhà hát":id=="homestay"?"Homestay":id=="garden"?"Khu vườn":id=="station"?"Ga":"Địa điểm";
 public static string Evidence(string id)=>InvestigationContent.Get(id)?.title??(id=="paper.khai"?"Giấy mẫu tại homestay":id=="receipt.channel"?"Phiếu nhận/trả giấy":id.StartsWith("letter.")?"Thư tại đầu mối":id=="M1"?"Sổ tại nhà hát":id=="M2"?"Phiếu ba gói":id=="M3"?"Biên bản tại trường":id=="M4"?"Nguồn lời kể tại vườn":id=="M5"?"Bản thỏa thuận":id=="M6"?"Sổ phác tại homestay":id=="log.homestay"?"Sổ ra vào homestay":"Tài liệu thử (ID trong debug)");
 public static string Target(FullInvestigationRuntime s,FullCasePlayTarget t)=>t.Action==FullPlayAction.Actor?Person(t.StableId):t.Action==FullPlayAction.Evidence?Evidence(t.StableId):t.Action==FullPlayAction.Travel?(s.CanTravel(t.StableId)?"Đi tới "+Place(t.StableId):"Đường chưa mở"):t.Action==FullPlayAction.Appointment?"Cuộc hẹn":"Bàn dựng";
 public static string Next(FullInvestigationRuntime s) {
  string prefix="Việc tiếp theo (gợi ý) • TEST ONLY: ";
  if(s.Finalized)return prefix+s.AppointmentRequirement;
  if(!s.HasHeardStatement("phuong.direction.homestay"))return prefix+(s.Location=="office"?"Tìm Phương gần điểm vào Văn phòng. Nhấn E, chọn câu hỏi về nơi Khải lưu trú. Chủ đề này đang mở; không cần Ghi lời kể.":"Trở về Văn phòng, tìm Phương để hỏi đầu mối nơi lưu trú.");
  if(!s.HasRecordedObservation("slide",0)){
   if(s.Location!="workshop")return prefix+"Tìm bảng Đi tới Xưởng (dọc hàng bảng bên trái; A/D di chuyển, E tương tác).";
   return prefix+(s.GetRecords().Any(e=>e.id=="slide"&&e.viewed)?"Đã khám Vệt trượt trên gác nhưng chưa Ghi. Mở lại vật, chọn Ghi mục 0 trong panel; Esc quay lại.":"Tìm Vệt trượt trên gác ở Xưởng. E khám, rồi chọn Ghi mục 0; chỉ khám chưa cập nhật bước tuyến.");
  }
  if(!s.HasEvidence("khai"))return prefix+(s.Location=="workshop"?"Tìm Gặp Khải trong dãy vật ở Xưởng; E mở cuộc gặp ngắn đã có. Đây là nội dung khám, chưa có hội thoại hỏi lại Khải.":"Trở lại Xưởng để xem cuộc gặp ban đầu Gặp Khải.");
  if(!s.HasPermission("test.samples"))return prefix+(s.Location=="office"?"Gặp Phương, chọn Xin phép nhận mẫu đối chiếu; không bắt buộc Ghi cuộc gặp Khải.":"Quay Văn phòng gặp Phương để xin phép nhận mẫu đối chiếu.");
  if(!s.HasEvidence("samples"))return prefix+"Theo đầu mối Phương đã chỉ tới Homestay; tìm Bàn giao mẫu đối chiếu, E nhận mẫu. Nguồn chưa nhận không được tự coi là đã có.";
  if(!s.HasHeardStatement("phuong.samples.origin"))return prefix+"Quay Văn phòng gặp Phương; chủ đề nguồn mẫu hiện đã mở.";
  return prefix+"Đã hoàn tất bước mở đầu/nhận mẫu. Tiếp tục khám các đầu mối đã mở, ghi quan sát và đọc thư; hồ sơ vẫn cho phép quay lại.";
 }
 public static string LockedTopic(FullInvestigationRuntime s,string topic) {
  if(s.Paused)return "Hãy Tiếp tục trước khi hỏi.";
  if(topic=="phuong.samples.origin")return s.HasEvidence("khai")?"Cần nhận mẫu sau khi được Phương cho phép; rồi quay lại hỏi.":"Chủ đề bổ sung chưa mở. Bạn vẫn có thể chọn câu hỏi đang mở ở trên; hãy khám các nguồn liên quan rồi quay lại.";
  if(topic=="ha.cargo.receipt")return "Cần E khám phiếu tại quầy trước; không bắt buộc Ghi.";
  if(topic=="bao.direction.lan")return "Cần giải và Ghi bản đọc thư Bảo trước khi hỏi chỉ đường; không chỉ xem thư.";
  return "Chủ đề chưa mở trong ngữ cảnh này; đóng panel và gặp đúng người tại nơi hiện tại.";
 }
 public static string TravelHint(FullInvestigationRuntime s,string destination) {
  if(s.CanTravel(destination))return "";
  if(destination=="homestay")return "Hãy nghe Phương chỉ đầu mối tại Văn phòng; không bắt buộc Ghi lời kể.";
  if(destination=="theatre")return "Hãy hỏi Mai tại Trường về đầu mối nhà hát.";
  if(destination=="garden")return "Hãy hoàn tất việc đọc thư tại Hồ và hỏi Bảo chỉ đường.";
  if(destination=="station")return "Cần hoàn tất và xác nhận hồ sơ trước; chỉ đi tới ga không tự chạy ending.";
  return "Cần Ghi quan sát Vệt trượt trên gác ở Xưởng trước khi đi tiếp. Chỉ khám chưa được tính là Ghi.";
 }
 public static FullActionResult Present(FullInvestigationRuntime s,FullActionResult result,FullCasePlayTarget target=null) {
  if(result.Success)return result;
  string message=result.Message;
  if(result.Code=="locked"&&target!=null&&target.Action==FullPlayAction.Travel)message=TravelHint(s,target.StableId);
  if(result.Code=="permission")message=s.HasEvidence("khai")?"Hãy gặp Phương tại Văn phòng, xin phép trước khi nhận mẫu.":"Hãy E khám cuộc gặp Gặp Khải tại Xưởng, rồi quay lại Phương để xin phép nhận mẫu. Chưa cần Ghi cuộc gặp.";
  if(result.Code=="conversation")message="Đối tượng này chưa có hội thoại trong cấu hình thử. Cuộc gặp Khải có sẵn qua E khám Gặp Khải ở Xưởng; không có chủ đề mới để tự mở.";
  return FullActionResult.Fail(result.Code,message); // Same failure/code; no runtime mutation.
 }
}
}
