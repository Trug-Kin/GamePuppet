# NOTE-PROVENANCE-01 — Nguồn của từng ghi nhận

## Acceptance trước triển khai — 10/11/2026

Xuất phát `afe461c2`, tracked sạch, hai untracked cũ giữ nguyên. GDD7.2/11 yêu cầu phân biệt cách thu nhận thông tin, **không phải chấm đúng/sai**. Chỉ bổ sung read model/metadata runtime; không đổi Observe, Acquire, AddRelation, lựa chọn kết luận, save schema1/ID/GUID hoặc UI/scene. Bảng này là ánh xạ triển khai cho entry rõ, không tuyên đã được phía nội dung review toàn bộ.

1. Phân loại explicit theo ID/index; không đoán từ từ khóa, tên NPC hay trạng thái finalized. Lời Hà là lời kể; ngoại hình Khải là quan sát. Liên hệ/claim người chơi là giả thuyết cả trước/sau xác nhận/chốt.
2. Query chỉ trả entry đã ghi; Acquire chưa Observe không tạo ghi nhận. Clip giữ cùng guard exposure trước/trong sự cố, không trả text khi chưa đủ. Không lấy raw transcript để lấp entry chưa mở.
3. Entry thiếu metadata trả Chưa phân loại/Thiếu metadata; entry mơ hồ trả Chưa phân loại/CHỜ REVIEW NỘI DUNG. ID/index không có hoặc chưa ghi: thất bại rõ, không text/nguồn và không exception.
4. Snapshot không chứa tham chiếu mutable vào save; query không ghi state/file, đổi điều kiện mở, Message, pause/input hay tiến trình. Ghi lặp không nhân đôi. Đổi câu chữ giữ cùng ID/index không tự đổi loại; thay ý nghĩa nội dung cần review mapping.
5. Save schema1 có trước gói tải được, loại được suy ra mà không thêm field; query không sửa bytes. Kiểm roundtrip liên quan và luồng Observe hiện có, không chạy bộ IO/trial cũ.
6. Kiểm bằng API/fixture trong scene Gameplay/profile mới. UI sản phẩm, scene tích hợp,1080p/người mới chưa được suy PASS. INTERVIEW-REVISIT-01 không triển khai.

## Bảng ánh xạ prototype

Khóa `id:index` dùng index observation 0-based hiện có, **không phải ID save mới**. Chỉ phân loại nội dung đã được ghi; đây là bảng phát triển có thể chứa thông tin chưa mở, không đưa nguyên bảng lên UI. Cột loại theo cách thu nhận, không xác minh sự thật. Transcript tĩnh không tự được phát như một ghi nhận mới.

| Khóa | Loại | Nguồn | Căn cứ / review |
|---|---|---|---|
| `slide:0` | Quan sát | Xưởng • mặt sàn cạnh mép gác | Nhìn dấu bụi và vị trí trên sàn. |
| `slide:1` | Quan sát | Xưởng • mặt sàn cạnh mép gác | Quan sát lớp dấu giao nhau, không kết luận người gây ngã. |
| `shoeprint:0` | Quan sát | Xưởng • dấu đế trên gác | Nhìn phần dấu đè trên bụi. |
| `shoeprint:1` | Quan sát | Xưởng • dấu đế trên gác | Nhìn vân/gót, chưa nhận diện tuyệt đối. |
| `r03:0` | Quan sát | Xưởng • nhãn R-03 và dây quấn | Đọc nhãn rối và đối chiếu vật đang sửa. |
| `r03:1` | Quan sát | Xưởng • nhãn R-03 và dây quấn | Quan sát đầu dây và đường nối vật thể; chưa xác định ai đặt. |
| `r12:0` | Quan sát | Xưởng • thẻ sửa và rối R-12 | Đọc các mã trên thẻ/vật và đối chiếu. |
| `r12:1` | Quan sát | Xưởng • thẻ sửa và rối R-12 | So hai vật có nhãn khác nhau; không suy người dàn dựng. |
| `mechanism:0` | Quan sát | Xưởng • cánh, chốt và dây nhả | Khám/thao tác trực tiếp cơ cấu hiện tại, không suy trạng thái lúc ngã. |
| `mechanism:1` | Chưa phân loại | Xưởng • cánh, chốt và dây nhả | CHỜ REVIEW NỘI DUNG: câu trộn khám khe và kết quả lượt thử, chưa rõ cùng nguồn cơ cấu thật hay bản sao. |
| `mechanism:2` | Quan sát | Xưởng • cánh, chốt và dây nhả | Nhìn vị trí tay nắm so với gác. |
| `hook:0` | Chưa phân loại | Xưởng / bàn mô hình • móc M | CHỜ REVIEW NỘI DUNG: câu trộn bụi hiện trường và so dấu thử trên mô hình. |
| `fabric:0` | Quan sát | Xưởng • mảnh vải tại tay nắm D | Nhìn mép rách và đường chỉ, không suy thời điểm. |
| `fabric:1` | Quan sát | Xưởng • mảnh vải tại tay nắm D | Nhìn sợi chỉ và đường dệt. |
| `phone:0` | Quan sát | Xưởng • điện thoại và giá quay | Quan sát hướng giá quay và trường hình. |
| `phone:1` | Quan sát | Xưởng • điện thoại và giá quay | Đối chiếu đồ trong đoạn quay với vật ở xưởng; không kết luận danh tính người kéo. |
| `clip:0` | Quan sát | Điện thoại • đoạn quay sửa R-12 | Quan sát hình trong đoạn quay sau guard exposure hiện có. |
| `clip:1` | Quan sát | Điện thoại • đoạn quay sửa R-12 | Quan sát diễn biến chốt/cánh và hai tay trong hình. |
| `clip:2` | Quan sát | Điện thoại • đoạn quay sửa R-12 | Ghi giới hạn trường hình; không suy người kéo hoặc ý định. |
| `khai:0` | Quan sát | Cửa xưởng • ngoại hình Khải nhìn thấy | Nhìn áo/giày tại cuộc gặp, khác với lời Khải trong transcript. |
| `samples:0` | Chưa phân loại | Phương • bàn giao mẫu prototype | CHỜ REVIEW NỘI DUNG: câu là trạng thái cho phép truy cập kèm lời bàn giao, chưa rõ loại entry mong muốn. |
| `coat:0` | Quan sát | Bàn mẫu xưởng • cổ tay áo được bàn giao | Nhìn phần thiếu và đường may trên áo thật. |
| `coat:1` | Quan sát | Bàn mẫu xưởng • cổ tay áo được bàn giao | Nhìn mép thiếu và điểm dừng mũi chỉ. |
| `shoe:0` | Quan sát | Bàn mẫu xưởng • đế giày được bàn giao | Nhìn góc khuyết và vân đế. |
| `shoe:1` | Quan sát | Bàn mẫu xưởng • đế giày được bàn giao | Nhìn các dãy vân và vị trí gót. |
| `shoecontrol:0` | Quan sát | Bàn mẫu xưởng • giày mẫu B | Nhìn mẫu đối chứng, không tự xác nhận khớp. |
| `ha:0` | Lời kể | Quầy Hà • lời Hà kể | Cả phần vận chuyển và giới hạn phiếu được tóm tắt từ lời Hà; không tự xác minh nội dung lời kể. |
| `ha:1` | Lời kể | Quầy Hà • lời Hà kể | Kênh nhận giấy được Hà giới thiệu; loại nguồn không chứng nhận lời kể đúng. |
| `letter:0` | Quan sát | Quầy Hà • dải thư tư thế | So hình mở đầu trên dải với bảng tập đã giới thiệu. |
| `letter:1` | Chưa phân loại | Quầy Hà • dải thư tư thế | CHỜ REVIEW NỘI DUNG: câu nêu pose ID/ánh xạ dùng chung, trộn quan sát hình và quy ước dữ liệu. |
| `receipt:0` | Quan sát | Hà giao • nội dung đọc trên phiếu vận chuyển | Đọc điều ghi trên giấy, không chứng nhận chuyến hàng thực tế hoặc hàng trong thùng. |
| `henri:0` | Quan sát | Sổ nghề Henri • nội dung bảng tập | Đọc chú giải trong sổ, không coi là lời Henri đang kể trực tiếp. |
| `draft:0` | Chưa phân loại | Hà giao • giấy mẫu và bản nháp | CHỜ REVIEW NỘI DUNG: câu trộn đặc điểm vật thể và đánh giá thiếu nguồn độc lập. |
| `decoded:0` | Chưa phân loại | Người chơi • bản đọc mật mã đã ghi | CHỜ REVIEW NỘI DUNG: bản đọc suy ra từ mapping, không ép thành quan sát hay lời kể. |
| `relations[index]` | Giả thuyết | Người chơi • liên hệ tự ghi | Note tự do đã ghi, không nâng thành sự thật khi chọn kết luận. |
| `conclusions[index].claim` | Giả thuyết | Người chơi • nhận định tự ghi | Claim không rỗng; choice/confirmation/finalized không đổi loại. |
| `trials[index]` | Chưa phân loại | Bàn dựng • kết quả mô hình | CHỜ REVIEW NỘI DUNG: kết quả thử mô hình không đồng nhất với quan sát sự kiện thật; không tự chọn loại. |

Kết quả kiểm, API thực và hướng dẫn tích hợp sẽ bổ sung sau khi triển khai/kiểm. Hiện chưa có PASS.
