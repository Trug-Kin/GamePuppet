# Checklist r106 — 06/11/2026

| Kiểm | Kết quả / bằng chứng |
|---|---|
| Baseline đúng scene/version | r105, Unity 6000.6.2f1, Edit sạch trước sửa; `baseline-r105.txt`, ảnh before-help-at10 |
| Nguyên nhân ngắt | Xác nhận cả AdvanceTrial/seek/play dùng 10 giây; cánh chưa xong hồi ở help/hold |
| Sáu chu kỳ API | PASS `cycle-api.txt`, `cycle-transitions.csv`; đo mốc chuyển đầu vào, hồi, đóng/cài và trạng thái cuối |
| Luật/đầu vào cũ | PASS `law-unchanged.txt`; cùng mã cơ cấu và sáu lịch gốc. So trạng thái từng ca tại 0/2,5/6/10 giây với r105 khớp chính xác |
| Sáu chu kỳ UI thời gian thực | PASS `cycle-ui-final.txt`, `six-whole-cycles-final.mp4`; cả ba nút bỏ giữ đi tiếp tới cài |
| Đối chiếu hình | Review ảnh cuối sáu ca/ba ảnh sau bỏ giữ và frame trích video: chốt đổi góc, cánh hồi/đóng, hình nộm rời cánh sau ngừng đè. Không suy mọi frame tiếp xúc đã được kiểm |
| Đang giữ có chủ đích | PASS ca người giữ mở 82°, chèn 35°; không ép về đóng. Móc về vùng đóng vẫn chốt nhả cho tới nút nhả móc |
| Tua trước/sau thả và tái cài | PASS chuột MouseDown/Drag/Up thực trong GameView, `regression-ui.txt`; trạng thái tính lại từ đầu, không mở quan sát clip hoặc tự ghi thử nghiệm chỉ từ seek |
| Chuyển ca đang chạy | PASS UI; timeline và lịch bỏ giữ cũ được reset |
| Đổi góc / UI không quay player | PASS UI giữ time/state và hướng player; không đổi camera clip |
| Pause/resume | PASS API pause + nút UI tiếp tục sau bỏ giữ, đúng timeline hiện hành |
| Save/load | PASS API giữa mô hình đang chạy: reset an toàn về 0/dừng/không ca, xóa lịch bỏ giữ; năm load lặp không trôi player. Không tuyên bố giữ nguyên thời điểm sau load |
| Profile khởi động thật | PASS mở Editor với --investigation-save-dir, `editor-actual-args.txt`, `profile-launch-proof.txt`; F5/F6 bàn phím tổng hợp ghi/tải profile r106 trống đúng đường dẫn |
| Không tự load profile đã giải | PASS hồ sơ sạch trước F5; bản lưu thử có namespace r106, evidence/mapping/claim/grounds trống; không đọc save gameplay cũ |
| Compile / scene mở lại | PASS `final-scene-audit.txt`, `event-binding-audit.txt`; Missing Script 0, broken serialized reference 0, Edit sạch, diagnostics tắt |
| Tuyến Hà / UI không liên quan | Không chạy lại; không sửa collider/tương tác/luồng tuyến, không gán kiểm lịch sử r104 thành kiểm mới |
| Profile người thật / mức hiểu | **NOT_RUN**; hướng dẫn/phiếu có bốn nhóm khó khăn và lời nói nguyên văn |
| Bảo toàn | `protected-final.json`: **1.707/1.707 file**, gồm **17/17 save gốc theo dõi**, giữ SHA256; không suy toàn máy. Không build/ZIP/runner sản phẩm mới |

Video là animation minh họa điều khiển bằng UI tổng hợp. Các script dưới Test/Tam là kiểm có giới hạn trong Editor, không phải runner sản phẩm. Hai capture FAIL giữ riêng; sự cố assertion đọc UI quá sớm không gán thành kết quả sản phẩm PASS.

Giới hạn: chưa kiểm người mới; không thẩm định vật lý ngoài đời, không rà mọi frame rig/va chạm hay toàn map. Art/chi tiết nhỏ vẫn provisional theo r105, không mở đợt polish chung.
