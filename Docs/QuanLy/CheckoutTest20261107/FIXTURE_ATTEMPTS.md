# Phạm vi input và các lần fixture chưa đạt

- Lần đầu: log tool ghi resolution=1536x864, FAIL 1080p render dimensions. Raw file bị lần thử sau ghi đè trước khi copy thành công; transcript này không giả là bản raw đầy đủ.
- Sau resize: ScreenCapture thực 1920x1080. Các file before-record*.txt vẫn FAIL tại nhấn Start, không được đổi thành PASS sản phẩm. File before-record-fixture1536-FAIL.txt có tên nhầm: nội dung thực là lần 1920x1080 không kích hoạt nút, giữ nguyên và đính chính tại đây.
- Các thử EditorWindow event và native mouse là input tổng hợp, không phải người mới. Hai lần native input bị guard từ chối vì foreground PID không phải Editor; không suy thành lỗi game. Các thao tác native đầu chưa có xác nhận state đáng tin cậy.
- Trang intro sau đó chuyển sang page rỗng, introAccepted=true nhưng chưa xác định chắc nguồn thao tác. Đã hỏi người dùng có đang điều khiển Unity, chưa có phản hồi lúc viết báo cáo. Không tính việc đóng intro này thành UI PASS.
- Reflection geometry bị Unity bridge từ chối namespace; đã dùng API public để đọc geometry. Không thay quyền bảo vệ bridge.
- Các ca controller/E/ghi/R/P/điện thoại, save/load trước ghi, skip, cipher/compare/conclusions chưa có bằng chứng UI đạt của checkout. NOT_RUN/INCOMPLETE; người mới NOT_RUN.

Client cũ chỉ giữ cải tiến chọn project rõ ràng và title chung; các thử native focus/click đã bỏ khỏi bản bàn giao. Không thêm runner/component runtime.
