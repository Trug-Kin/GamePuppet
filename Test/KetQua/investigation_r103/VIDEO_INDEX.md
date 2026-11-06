# Video và phạm vi

| File trong `Test/BangChung/investigation_r103` | Input / âm thanh | Phạm vi |
|---|---|---|
| `controller-route-final.mp4` | **InputSystem keyboard/mouse tổng hợp**, không âm thanh | Controller gốc từ spawn trước xưởng, lên gác khám, xuống gặp Hà và quay lại bàn dựng. Không teleport, không camera dolly. Log `route-motion.txt`; không chứng minh mọi góc/nhánh hoặc người mới hiểu |
| `clip-six-cases-ui.mp4` | **Chuột UI GameView tổng hợp**, không âm thanh | Phát clip12s, phát lại/dừng/tua bằng nút, lần lượt sáu trường hợp bằng nút; API mở trang ban đầu, không đặt kết quả cánh. Clip bên trong là animation minh họa game, **không phải controller footage** |
| `controller-route.mp4` | InputSystem tổng hợp, không âm thanh | Lượt đạt sớm trước các sửa UI/preview cuối; dùng tham khảo lịch sử, ưu tiên video final |
| `controller-route-blocked-hook.mp4` | InputSystem tổng hợp, không âm thanh | **FAIL** tại12/21 do ray móc bị vải/collider cũ che, đã sửa và kiểm lại; không tuyến đạt |
| `INVALID-controller-capture-black.mp4` | Capture hỏng | **INVALID**, cửa sổ Game không hiện ra trước; không bằng chứng gameplay |
| `capture-probe.mp4` | Không diễn giải nội dung chơi | 3s kiểm capture sau đưa Game lên trước, không playtest |

Video quay cửa sổ Game gồm thanh công cụ Editor; nội dung Game render1280×720, canvas capture có thêm thanh công cụ (khoảng1280×780). Không tạo player/build. Các PNG `presentation-video-*` và `controller-final-*` là frame trích để xem trực tiếp. Log ffmpeg và metadata cuối trong hồ sơ này.

Các clip/diễn viên/rối đều mang nhãn bản dựng thử. Camera bàn dựng là camera mô hình để quan sát cơ cấu, không dùng làm bằng chứng di chuyển từ tuyến chính. Chưa có người thật playtest.

Nhãn prompt bàn dựng được sửa sau capture: từ tên object kỹ thuật sang “bàn dựng lại” (dây/cánh cũng dùng tên thường). `final-table-prompt.png` là fixture API nạp lại vị trí save tuyến; ảnh không hiện prompt nên **không xác minh trực quan nhãn mới**, cũng không là bằng chứng đi controller. Thay đổi nhãn đã compile và kiểm mã. Video tuyến vẫn ghi đúng lượt di chuyển trước sửa chữ này; geometry/controller không đổi.
