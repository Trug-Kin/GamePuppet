using System;
using System.Collections.Generic;
using UnityEngine;

namespace ConRoi.Investigation.V36.R103
{
    [Serializable] public class EvidenceDefinition
    {
        public string id, title, source, transcript;
        public string[] observations;
        public EvidenceDefinition(string id,string title,string source,string transcript,params string[] observations)
        { this.id=id;this.title=title;this.source=source;this.transcript=transcript;this.observations=observations; }
    }
    public static class InvestigationContent
    {
        public const string HaPlain = "STAND BY WHERE DID THE THREE CRATES GO SEND WORD";
        // One pose ID is shared by the paper, lookup book and articulated puppet preview.
        public const string Alphabet = "ABCDEGHINORSTWY";
        public static readonly EvidenceDefinition[] Evidence = {
            new EvidenceDefinition("slide","Vệt trượt trên gác","Xưởng • sàn cạnh vị trí sửa R-12","Vệt kéo dài tới mép cánh. Hiện trường được giữ nguyên; các lần thử dùng mô hình riêng.","Vệt trượt đi từ chỗ sửa tới mép cánh.","Dấu đế giày nằm trên một đoạn vệt trượt; thứ tự chồng dấu cần được đối chiếu."),
            new EvidenceDefinition("shoeprint","Dấu đế tại mép gác","Xưởng • gác thao tác","Một phần gót có khoảng khuyết lệch bên và cụm vân mòn. Không thể nhận diện tuyệt đối chỉ bằng loại giày.","Các khối vân đen nằm trên hai dải bụi kéo dài, phần giao không bị bụi phủ lại.","Gót khuyết lệch bên, một hàng vân bị mòn; cần mẫu giày thật."),
            new EvidenceDefinition("r03","Rối tập R-03 và dây quấn","Xưởng • cạnh vị trí nạn nhân","Nhãn R-03: rối tập nhỏ. Dây quấn quanh lan can có đầu tự do, không nối với dây treo R-12.","R-03 là rối tập của xưởng, không phải rối đang sửa.","Dây quấn là đạo cụ khác với dây treo R-12; chưa tự xác định ai đặt."),
            new EvidenceDefinition("r12","Rối sân khấu R-12","Xưởng • vị trí sửa trên gác","Thẻ buổi sửa: R-12, thay dây tay phải. Dụng cụ và giá quay vẫn ở vị trí làm việc.","Thẻ buổi sửa, dây treo và rối lớn cùng mang mã R-12.","R-12 khác rối tập cạnh nạn nhân; cần so với clip và R-03."),
            new EvidenceDefinition("mechanism","Cánh, chốt và dây nhả","Xưởng • khám trước thử","Dây D chỉ nhả chốt C; lực từ phía gác mới mở cánh L. Cánh không có nấc giữ mở. Không phát hiện chèn tại khe đã khám.","Khi dây chùng, cánh đóng và chốt cài; dựa thông thường không mở được.","Khám các khe: không có vật chèn; cơ cấu hồi và chốt hoạt động trong lượt thử.","Tay nắm dây đặt bên dưới gác, tách xa cánh và vị trí sửa."),
            new EvidenceDefinition("hook","Móc M","Xưởng • cạnh tay nắm D","Lớp bụi liền trên bề mặt móc. Trạng thái này được ghi trước khi thử trên bản sao.","Bụi trên móc còn liền; thử móc ở bàn mô hình tạo tiếp xúc khác."),
            new EvidenceDefinition("fabric","Mảnh mép vải","Xưởng • khe tay nắm D","Mảnh vải xanh có mép may rách bất đối xứng, đoạn chỉ và đường dệt. Dầu chỉ là dấu tiếp xúc, không xác định thời điểm.","Mép rách có ba chỗ gãy lớn và đoạn chỉ lệch; cần đối chiếu chỗ thiếu trên áo thật.","Các sợi chỉ còn bám ở mép rách; giữa mảnh có các đường dệt song song."),
            new EvidenceDefinition("phone","Điện thoại quay sửa R-12","Xưởng • giá quay tại chỗ sửa","Tư đặt điện thoại trên giá để quay cách thay dây tay phải R-12 cho người học nghề. Clip có sẵn ngay lúc khám, không cần phục hồi tệp. Bản prototype là minh họa liên tục dựng bằng cơ cấu và hình nộm; chưa là footage final.","Giá quay hướng vào chốt, cánh và hai tay đang thao tác R-12.","Rối, thẻ sửa và dụng cụ trong đoạn quay tương ứng vật tại xưởng."),
            new EvidenceDefinition("clip","Quan sát đoạn quay","Điện thoại tại xưởng","Bản dựng thử 12 giây liên tục: chốt cài/cánh đóng; hai tay giữ đồ sửa; chốt nhả rồi cánh mở dưới sức dựa. Không thấy điểm điều khiển hoặc người kéo. Hãy xem hình trước khi ghi.","Cánh đóng và chốt cài ngay trước biến cố trong đoạn quay.","Chốt nhả trong khi hai tay người sửa vẫn giữ dụng cụ/dây; sau đó mất chỗ tựa.","Góc quay không thấy người ở ĐK, không nhận diện người kéo hoặc ý định."),
            new EvidenceDefinition("khai","Gặp Khải","Cửa xưởng • cuộc gặp ngắn","Khải: Tôi từng trong gánh. Tôi có tới nói chuyện với ông Tư. Đừng biến chuyện cũ thành lý do kết tội tôi.","Khải mặc áo xanh, cổ tay phải sờn; giày có gót mòn. Nhìn từ xa chưa đủ đối chiếu."),
            new EvidenceDefinition("samples","Bàn giao mẫu đối chiếu","Phương • điều tra rút gọn cho prototype","Phương: Khải đã đồng ý giao áo và giày để đối chiếu. Tôi đặt mẫu tại bàn xưởng. Đây là bước rút gọn, chưa mô phỏng chuyến tới homestay hoặc xác thực thủ tục pháp lý.","Đã có quyền xem mẫu áo/giày thật trong phạm vi prototype; không coi quan sát xa là mẫu."),
            new EvidenceDefinition("coat","Áo Khải • cổ tay rách","Mẫu do Phương bàn giao tại xưởng","Xem cổ tay có phần thiếu; đối chiếu mép rách, đường chỉ và dệt với mảnh vải. Màu giống nhau không đủ.","Phần thiếu ở cổ tay có đường gãy và đoạn may để so với mảnh mép vải.","Mép thiếu bất đối xứng; các mũi chỉ dừng lại ở cạnh rách."),
            new EvidenceDefinition("shoe","Giày Khải • mặt đế","Mẫu do Phương bàn giao tại xưởng","Mẫu đế giày có vùng mòn và khuyết gót. Dấu hiện trường chỉ giữ một phần nên mức nhận diện có giới hạn.","Gót có một góc khuyết; một khối vân giữa nhỏ hơn các khối bên cạnh.","Hai dãy vân lệch nhau; phần thiếu ở gót nằm lệch một bên."),
            new EvidenceDefinition("shoecontrol","Giày mẫu B","Bàn mẫu đối chiếu của xưởng","Mẫu cùng màu và kiểu đế. Hãy xem góc gót và từng hàng vân; tên mẫu không xác định người mang.","Góc gót khuyết ngắn hơn; khối vân giữa còn nguyên."),
            new EvidenceDefinition("ha","Lời Hà và điểm nhận giấy","Quầy Hà • chợ","Hà: Trước tôi lo vận chuyển. Phiếu cũ ghi ba gói, không ghi bên trong. Người giao dặn gửi lời lại qua quầy này. Tôi còn giữ dải giấy tư thế; anh xem giúp.","Hà từng lo vận chuyển; phiếu không tự nói hàng trong thùng.","Quầy nhận/trả lời nhắn bằng giấy; không phải một kênh liên lạc tự suy ra."),
            new EvidenceDefinition("letter","Dải tư thế gửi Hà","Hà giao trực tiếp tại quầy","Chuỗi tư thế và dấu chấm đặc giữa dòng. Mỗi tư thế ứng một chữ; dấu chấm chỉ ngắt từ. Mở bảng gán để thử đọc; bản chép chưa chứa lời giải.","Mở đầu cùng bảy ký hiệu với khẩu lệnh tập Henri đã giới thiệu.","Các ký hiệu lặp lại giữ cùng pose ID ở dải giấy, bảng tra và rối."),
            new EvidenceDefinition("receipt","Phiếu vận chuyển ba gói","Hà • phiếu cũ","Ngày 17/8 • Ba gói hàng • giao tại điểm nhận Kho Sáu. Nội dung trong gói không ghi trên phiếu. Ngày là nội dung hư cấu của prototype, chưa nối bản thỏa thuận ở khu chưa dựng.","Phiếu ghi ba gói, ngày và điểm nhận; không ghi rối bên trong."),
            new EvidenceDefinition("henri","Bảng tập Henri • phần thư Hà","Xưởng • sổ nghề","Mỗi tư thế là một chữ. Bảng này cho tra tập ký tự của thư Hà; mở đầu STAND BY nghĩa là chờ lệnh. Tra bảng không làm mất điểm.","STAND BY là khẩu lệnh mở đầu; dấu chấm đặc dùng ngắt từ."),
            new EvidenceDefinition("draft","Giấy mẫu và bản nháp","Hà • giấy mẫu giữ cùng thư","Dải giấy mẫu có mép xé và nét thử. Prototype cung cấp mẫu để so vật thể; chưa có chứng cứ nơi thu bản nháp riêng của Khải, nên không tự nhận diện người gửi.","Mép giấy và nét thử có thể đối chiếu; chưa có nguồn độc lập nối nháp này với người gửi."),
            new EvidenceDefinition("decoded","Cách đọc đã ghi","Bảng gán của người chơi","Bạn đã đọc được cụm hỏi về ba thùng đi đâu. Đối chiếu phiếu Hà; danh tính người gửi và hàng trong thùng còn cần chứng cứ.","Cụm THREE CRATES GO gợi kiểm đường đi ba gói trên phiếu; không tự chứng minh vụ bán rối.")
        };
        public static EvidenceDefinition Get(string id) => Array.Find(Evidence,e=>e.id==id);
    }
    [Serializable] public class EvidenceRecord { public string id; public bool collected; public List<int> observations=new List<int>(); }
    [Serializable] public class CharacterAssignment { public string pose, letter; }
    [Serializable] public class EvidenceRelation { public string first,second,note; }
    [Serializable] public class TrialRecord { public string id,report; public float returnSeconds; }
    [Serializable] public class ConclusionRecord { public int choice; public string claim=""; public List<string> grounds=new List<string>(); }
    [Serializable] public class InvestigationSave
    {
        public int schema=1; public string namespaceId="conroi.investigation.v36.r103";
        public Vector3 position; public float yaw,pitch; public string inspecting="",page="";
        public bool samplesAllowed,returnedToWorkshop,visitedHa;
        public bool clipBeforeSeen,clipEventSeen;
        public List<EvidenceRecord> evidence=new List<EvidenceRecord>();
        public List<CharacterAssignment> mapping=new List<CharacterAssignment>();
        public List<EvidenceRelation> relations=new List<EvidenceRelation>();
        public List<TrialRecord> trials=new List<TrialRecord>();
        public List<ConclusionRecord> conclusions=new List<ConclusionRecord>();
        public GateState gate=new GateState(); public bool finalized;
    }
}
