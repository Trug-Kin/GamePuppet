# CONCLUSION-CHECKPOINT-01 — Chốt bản ghi có checkpoint an toàn

Gameplay & Dữ liệu. **Đã triển khai và kiểm runtime trong scene Gameplay; chờ tích hợp.**

## Căn cứ, baseline và acceptance

GDD7.3: xác nhận trung tính, save riêng ngay trước chốt; prototype13 cho chọn chưa đủ. Handler cũ đặt finalized trước Save(void), vẫn feedback đã lưu khi IO lỗi; confirmConclusion không gắn nội dung và có thể sống qua Load. API tái hiện handler baseline6 ca,4 lỗi; không gọi đây là thao tác click UI baseline.

Acceptance ghi trước sửa trong [tiến độ](../../../../QuanLy/DANG_LAM_NGUOI_2.md): chỉ báo chốt/đặt live finalized sau ghi hoàn tất; checkpoint chứa draft chưa chốt; lỗi ghi giữ save tốt và có thể thử lại; xác nhận gắn nội dung, không giữ sau sửa/Load; không tự chấm kết luận hoặc tạo ending. Kiểm lỗi tạm/đích chỉ profile riêng, khôi phục finally.

## Thay đổi và hành vi

Nguồn **d7f1c93b7db1c1a878c0763733e6f14c56750cc5**:

- InvestigationConclusionCommit.cs +meta mới: xác nhận bằng chữ ký record, ghi checkpoint schema1 `prototype.before-conclusion.json`, rồi ghi chính và áp dụng live sau thành công. Dùng writer hiện có; không đổi guards.
- InvestigationSession.cs: xóa xác nhận theo ngữ cảnh, Load dùng helper path chung cho Load thường và API restore checkpoint.
- InvestigationSessionUI.cs: handler nút chốt dùng bool thực, sửa record làm hết xác nhận/finalized; không đổi layout/presentation.

`TryFinalizeConclusions()` trả false/Message nếu chưa xác nhận hoặc IO thất bại; không chuyển trang/khóa input. Checkpoint là draft của lần thử gần nhất, có thể đã cập nhật dù ghi chính thất bại. Hai file không là giao dịch nguyên tử chung. Không tự Load checkpoint/.tmp, không xóa bản chính tốt trước ghi thành công. `LoadConclusionCheckpoint()` dùng validator cũ, chỉ restore khi gọi tường minh. **Chưa có nút restore mới**; bàn giao API và yêu cầu UI riêng, không giả vờ người chơi đã dùng được nút đó.

## Kiểm và giới hạn

25/25 API riêng ở lượt kiểm lại;86/86 API cuối gồm25 kết luận +43 clip +18 tuyến. Kiểm file chính khóa, đường tmp bị thư mục chặn, checkpoint khóa, khắc phục, bytes/Load cũ, sửa/xác nhận/pause, chưa đủ, restore tường minh/tiếp tục và checkpoint malformed. Không chạy lại các bộ save/IO cũ.

37/37 controller/UI đại diện có xác nhận/nút chốt, kiểm checkpoint/chính, F6, P/Esc/pause. Restore và lỗi IO của gói này **chỉ kiểm API**, không tuyên controller đã mô phỏng IO. Không chứng minh mất điện/crash. Các lần IO ngoài dự kiến trước lượt cuối và lỗi công cụ được giữ trong [báo cáo gộp](../PROTOTYPE-FLOW-01/README.md); không ghi đè lịch sử24/25 hoặc80/86.

Không cần wiring cho nút chốt hiện có. Nhận đủ chuỗi theo báo cáo gộp, kiểm scene chung và UI1080p. Bốn mục kết luận vẫn bản ghi prototype, không chấm ending A/C hoặc mở nội dung tương lai. P-close OPEN, người mới NOT_RUN.
