# Đề xuất đợt Gameplay & Dữ liệu tiếp theo — 10/11/2026

**Chỉ đề xuất, chưa được triển khai.** Đối chiếu source local `74a80c21d903442246ec3f6e5de021166477c83c`, nhánh `work/runtime-contracts-baseline`, worktree riêng. Tracked sạch trước lượt này; hai thư mục untracked cũ giữ nguyên. Không fetch/xác minh push, không chạy Unity/API và không có kết quả PASS mới.

Căn cứ: [GDD v3.6](../../../CON_ROI_GDD_v3.6_CHOT_TRIEN_KHAI.md), [ownership](../../../QuanLy/OWNERSHIP.md), [contracts](../../../QuanLy/HOP_DONG_MODULE.md), [registry](../../../QuanLy/EVIDENCE_REGISTRY.md) và [checklist tiến độ](../../../QuanLy/DANG_LAM_NGUOI_2.md). Chỉ đề xuất **hai gói**; không điền đủ ba bằng chức năng đã có. Những API/metadata mới bên dưới là thiết kế đề xuất, chưa tồn tại trong source.

## 1. Ưu tiên: NOTE-PROVENANCE-01 — Phân biệt nguồn của từng ghi nhận

### Hành vi người chơi và GDD

Người chơi đọc hồ sơ biết mục nào là điều quan sát trực tiếp, mục nào do nhân vật kể, mục nào là giả thuyết mình ghi; có thể xem theo loại nguồn mà không coi một lời kể là sự thật đã chứng minh. GDD7.2 yêu cầu ghi tách ba loại; GDD11 yêu cầu phân biệt sự thật cố định, quan sát đã mở và kết luận người chơi. Không bổ sung chấm đúng/sai hoặc gate kết luận.

### Runtime còn thiếu, khác với chức năng đã có

- [EvidenceDefinition](../../../../Assets/Scripts/InvestigationV36R107/InvestigationEvidence.cs#L7) chỉ có `id/title/source/transcript/observations`; `source` là chuỗi mô tả nơi/người cung cấp, không phải loại nguồn ở từng ghi nhận.
- [EvidenceRecord](../../../../Assets/Scripts/InvestigationV36R107/InvestigationEvidence.cs#L43) lưu mọi chỉ số trong `observations`; [Observe](../../../../Assets/Scripts/InvestigationV36R107/InvestigationSession.cs#L85) không có phân loại nguồn. `ha:0` ghi “Hà từng lo vận chuyển” xuất phát lời Hà; `khai:0` về cổ tay/giày lại là quan sát ngoại hình. Không thể phân loại toàn ID NPC thành lời kể.
- Relations đã tách thành `EvidenceRelation` và không được nâng thành sự thật. Giữ nguyên phần đó. Khẩu hiệu phân biệt nguồn đã có trong UI, nhưng chưa có read model/runtime metadata để consumer phân loại nhất quán. `DossierUI` hiện dùng cùng tiền tố “Quan sát đã ghi” cho mọi chỉ số; **không sửa UI trong gói runtime**.
- Đây là bổ sung ngữ nghĩa dữ liệu/đường truy vấn thiếu, không phải làm lại viewer, thêm bộ lọc UI đơn thuần hoặc xác nhận một lỗi gameplay đã tái hiện bằng Play.

### Phạm vi file, phụ thuộc, save/contracts

Dự kiến sửa `Assets/Scripts/InvestigationV36R107/InvestigationEvidence.cs` cho metadata từng entry; thêm partial/read model trong cùng thư mục nếu cần (tên/chữ ký chốt khi giao task), hook đọc dữ liệu trong `InvestigationSession.cs` nếu cần. Công cụ kiểm tại `Assets/ConRoi/Modules/Gameplay/Editor/`; cập nhật `Docs/QuanLy/HOP_DONG_MODULE.md`, tiến độ và báo cáo riêng.

Metadata gắn **ID + observation index hiện có**, không đổi thứ tự câu, ID hoặc nội dung truyện. Read model chỉ trả entry đã ghi, loại nguồn và nguồn gốc; quan hệ người chơi vẫn là giả thuyết. Với bản chép, dùng accessor có guard hiện có, không đọc raw transcript clip để lộ nội dung. Không diễn giải tự động note tự do thành bằng chứng xác nhận.

Không thêm field vào `InvestigationSave`: suy ra metadata từ definition + record/index đã lưu; save cũ hợp lệ vẫn dùng được. API cũ còn hoạt động; thêm contract đọc có kiểu, không âm thầm đổi kết quả Observe/AddRelation. Entry chưa có mapping hoặc nguồn lai phải báo chưa phân loại, không mặc định thành quan sát/sự thật. Việc phân loại bản đọc mật mã/kết quả mô hình phải được review riêng, không ép vào lời kể hoặc coi là sự kiện thực.

**Chủ phụ thuộc:** UI/UX & Tích hợp review bảng phân loại nội dung đang có và nối nhóm/nhãn nguồn vào hồ sơ; Gameplay & Dữ liệu sở hữu metadata/runtime; không cần model/map mới. Hoàn tất runtime được kiểm độc lập, không cần chờ layout. Đổi câu/nội dung nguồn thuộc phía nội dung, nằm ngoài gói này.

### Điều kiện đạt và cách kiểm độc lập

1. Fixture mới chỉ Acquire, chưa Observe: truy vấn ghi nhận rỗng; không tự cấp quan sát hay hiện bản chép chưa đủ điều kiện.
2. Ghi `slide:0`, `ha:0`, `khai:0`: trả đúng từng entry và loại/nguồn đã được duyệt; hai entry NPC không bị gộp cùng loại chỉ vì cùng nhân vật.
3. Ghi lặp không nhân đôi; AddRelation vẫn là giả thuyết, không đổi các entry nguồn hoặc finalized/tiến trình.
4. ID/index không có hoặc mapping thiếu trả kết quả rõ, không exception, không tự sửa record; thay snapshot consumer không sửa state session.
5. Save/load hợp lệ cũ và profile mới giữ nguyên record/index/relations; truy vấn sau Load phân loại giống trước. JSON/schema/ID không đổi; không chạy lại bộ IO cũ.
6. Kiểm API/proxy trong scene Gameplay/profile riêng; mở/đóng/pause/trở lại hồ sơ không làm query ghi thêm dữ liệu. UI sản phẩm/scene chung chỉ kiểm sau khi phía tích hợp nối consumer, ghi trạng thái riêng.

**Đề nghị giao trước.** Gói nhỏ, dùng nội dung prototype hiện có, không cần khu/nhân vật/ending mới và có thể giữ nguyên schema. Nó tạo phân biệt nguồn ở tầng dữ liệu trước khi thêm hội thoại có điều kiện; không chỉ đổi một dòng nhãn UI. Cần duyệt mapping ngữ nghĩa, không cần quyết định thêm cốt truyện.

## 2. INTERVIEW-REVISIT-01 — Hỏi lại Hà theo tài liệu đã đọc

### Hành vi người chơi và GDD

Sau khi đọc thư/phiếu, người chơi trở lại Hà để hỏi một câu cụ thể hoặc nhận phản hồi mới, thay vì chỉ mở lại nguyên phần giới thiệu. Căn cứ GDD6, mục mini game7: đối chiếu lời khai/tài liệu để hỏi lại NPC; GDD4: mỗi lần trở lại có câu hỏi hoặc phản hồi mới. Giới hạn đề xuất **một lượt hỏi lại tại quầy Hà trong prototype xưởng–Hà–xưởng**, không mở homestay/trường/vườn/ending.

### Runtime còn thiếu

[Interact](../../../../Assets/Scripts/InvestigationV36R107/InvestigationSession.cs#L128) với Hà luôn Acquire các ID cố định, đặt visitedHa và OpenEvidence; không có danh mục câu hỏi, điều kiện mở theo quan sát, chọn/hủy câu hỏi hoặc kết quả trả lời. [InvestigationActor.Greet](../../../../Assets/Scripts/InvestigationV36R107/InvestigationActor.cs#L12) chỉ chạy cử chỉ chào. Chưa có runtime hội thoại theo chứng cứ trong các module Gameplay/Contracts/r107 đã rà. Bản lời Hà tĩnh đang có vẫn dùng được, không đề xuất thay nó bằng một UI mới rồi coi là thêm gameplay.

### Phạm vi file, phụ thuộc, save/contracts

Dự kiến thêm runtime hội thoại/definition có điều kiện trong `Assets/Scripts/InvestigationV36R107/`, nối entry point ở `InvestigationSession.cs`; không sửa cử chỉ/model trừ khi cần và được giao. Công cụ kiểm riêng trong Gameplay/Editor, contract và báo cáo riêng. Không tạo tên API/ID nội dung như thể đã tồn tại.

UI/UX & Tích hợp/nội dung phải duyệt câu hỏi, câu trả lời, loại nguồn và điều kiện mở; UI cung cấp lựa chọn/hủy, hiển thị trạng thái khóa và lỗi. Runtime kiểm điều kiện khi chọn, không tin một nút đang enabled, không tự mở hội thoại hoặc ghi chứng cứ khi chỉ quay lại gần NPC. Không phụ thuộc tên nút/Hierarchy.

**Save là quyết định chưa chốt:** schema1 chưa có lịch sử câu hỏi/nhánh hội thoại. Không dùng visitedHa làm cờ “đã nghe mọi câu”, không lấy collected làm “đã đọc”. Nếu chỉ phản hồi lại thông tin hiện có thì có thể không cần lịch sử mới, nhưng phải được nhóm chốt; nếu phát sinh lời khai mới cần lưu, phải duyệt representation/key bền vững, tương thích save cũ và quan hệ với observation index trước triển khai. Không tự thêm field/schema/ID hoặc dùng một slot relation để giấu state hội thoại.

### Điều kiện đạt và cách kiểm độc lập sau khi nội dung được duyệt

1. Chưa đủ điều kiện đã chốt: query trả khóa/lý do, gọi trực tiếp action cũng không mở đáp án hoặc thêm state.
2. Đạt điều kiện bằng thao tác ghi/đọc đúng contract: đúng câu được mở; hủy/quay lại không đánh dấu đã nghe hoặc lặp cấp vật.
3. Chọn câu hỏi hợp lệ trả đúng phản hồi đã duyệt; gọi lặp có hành vi xác định, không nhân đôi lời khai/tiến trình.
4. Một lời kể mới không tự thành kết luận/quan sát trực tiếp; không tiết lộ người gửi/hàng trong thùng khi prototype chưa có căn cứ độc lập.
5. Pause/modal chặn action không đúng ngữ cảnh; đóng hội thoại trả input đúng; save/load giữ đúng lịch sử nếu nhóm chọn lưu, không tự coi mọi câu đã nghe từ visitedHa.
6. Dùng dữ liệu hội thoại đã duyệt + proxy NPC trong scene Gameplay/profile riêng; API kiểm khóa/lặp/hủy/lỗi và một tuyến UI thử riêng khi được triển khai. Không coi fixture cấp vật là tuyến điều tra tự nhiên. Scene chung chờ báo cáo phía tích hợp.

### Quyết định nhóm cần chốt trước khi giao thực hiện

- Có đưa **một lượt hỏi lại Hà** vào phạm vi prototype hiện tại hay chỉ bắt đầu ở lát cắt GDD12.3? GDD có hướng này nhưng checklist prototype trước chưa giao gói hội thoại.
- Câu hỏi/câu trả lời chính xác; chỉ làm rõ thư/phiếu/kênh nhận giấy đã có hay bổ sung thông tin mới? Nội dung mới phải chỉ rõ căn cứ, không sửa nguồn nháp Hà thành nháp riêng Khải.
- Điều kiện theo entry nào đã ghi/đã đọc; HaPlain đọc thử hay decoded đã ghi có đủ không? Không tự chọn “nhặt đủ vật” làm gate.
- Có lưu lần nghe/lựa chọn không; nếu có dùng representation nào tương thích schema hiện hành, không ghi đè ý nghĩa ID/index cũ?

**Chưa sẵn sàng triển khai** cho đến khi bốn quyết định này được nhóm duyệt. Gameplay & Dữ liệu không tự viết đáp án hoặc luật gate mới để lấp chỗ thiếu nội dung.

## 3. Ranh giới đợt đề xuất

Không chọn thêm các gói gợi ý/chú giải/giải thư Hà, relation ghi tự do, bàn dựng, checkpoint hoặc viewer: các phần runtime đó đã có. Cầm–đặt vật tự do cần định nghĩa slot/đặt hợp lệ và quyết định tái sử dụng hệ cũ; GDD9 nói “nếu có thể tái dùng”, chưa đủ để tự mở một hệ inventory mới. Ghép giấy/đường ba thùng và ending đầy đủ cần dữ liệu lát cắt GDD12.3/12.4, không tự mở từ mẫu nháp ở Hà vốn chưa nối danh tính người gửi.

Đây là kế hoạch công việc mới, không thay kết quả kiểm cũ hoặc tuyên bố code chưa triển khai đã PASS. IO-OPEN-01 theo dõi riêng; không thêm save audit. Đợt này chỉ lưu tài liệu và cập nhật tiến độ; không runtime/Unity/push/merge.
