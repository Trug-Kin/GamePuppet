# Video và phạm vi r106

| File dưới Test/BangChung/investigation_r106 | Loại và phạm vi |
|---|---|
| six-whole-cycles-final.mp4 | **Bản cuối**, 1 phút 44,23 giây, 1280×780 gồm GameView toolbar; nội dung render 720p. Sáu trường hợp từ trạng thái sạch, ba thao tác bỏ giữ, hồi đủ tới trạng thái cuối. Chuột GameView tổng hợp qua SendEvent; API chỉ mở trang/profile sạch ban đầu. Không API seek/gán kết quả/tăng tốc. Animation mô hình minh họa, **không âm thanh**, không phải controller footage hay người mới |
| FAIL-immediate-ui-assertion.mp4 | **FAIL / superseded**: dừng ở assertion sau click quá sớm trong lượt release; log cùng tên trong kết quả |
| FAIL-removal-check-timing.mp4 | **FAIL / superseded**: tới wedge rồi assertion chưa đợi UI cập nhật đủ; không dùng kết luận cả sáu ca. Bản cuối chờ frame cập nhật, không đổi luật sản phẩm |
| before-help-at10.png | Baseline r105 tại 10 giây qua API chẩn đoán; không bằng chứng UI thao tác |
| release/help/hold/hook/wedge/after-initial-final.png | Ảnh dừng đầu mỗi ca: phân biệt đóng/cài và trạng thái đang được giữ |
| release/hook/wedge-after-clear.png | Ảnh sau chủ động bỏ lực/móc/chèn và quan sát tới đóng/cài |
| regression-reset.png | Sau hồi quy UI/lifecycle và load reset an toàn; không đại diện mọi luồng gameplay |
| video-review-*.png | Frame trích từ video cuối để kiểm capture thực tế, không render thay thế |

`cycle-ui-final.txt` ghi trạng thái/time trong lượt UI. `cycle-api.txt` và `cycle-transitions.csv` là đo API riêng, không giả làm thao tác người chơi. `profile-launch-proof.txt` ghi F5/F6 tổng hợp; API thay một mục tạm giữa lưu/tải để xác nhận trạng thái được khôi phục, không điền đáp án.

Video giữ nguyên thời gian mô hình khi chờ người xem ở trạng thái cuối; bấm nút bỏ giữ mới chạy tiếp. Có thể xem bảng SIX_CASES để đối chiếu đồng hồ mô hình. Móc còn căng không bị gọi lỗi vì chốt chưa cài. Không dùng các file FAIL để cộng PASS. Người thật **NOT_RUN**.
