# Checklist r107

Ngày06/11/2026. PASS dưới đây là ca kỹ thuật được nêu, không phải mức hiểu của người mới.

| Phần | Trạng thái | Bằng chứng / phạm vi |
|---|---|---|
| Tái hiện vùng nền hở rộng ảnh105627 | PASS | Camera eye681.10/3.64/413.07, yaw230; ảnh before/after3 |
| Xác định nguyên nhân mesh/collider | PASS | Down-facing triangles, collider còn đỡ; triangle-probe/map-detail |
| Blender→FBX→Unity, vertex/UV/collider | PASS có giới hạn | map-integration-final/source-export-final/final-live-audit; face cực mảnh còn cảnh báo |
| Generic delivery gate terrain | FAIL, giữ nguyên | source-delivery-gate.json; kế thừa topology, xem MAP_REPAIR |
| Khớp chính xác ba góc người dùng đầu | NOT_RUN | Đã xem ảnh; view0–2 chỉ hướng tham khảo |
| Đi controller từ xưởng qua vùng hở và trở lại | PASS | map-controller-complete,76waypoint, video tổng hợp |
| Đi chéo/dừng/pause/F5/F6 vùng sửa | PASS | map-lifecycle; bản setup đầu FAIL/superseded |
| Giới hạn truy cập vùng xa chưa hoàn thiện | NOT_IMPLEMENTED | Không dùng hàng rào để thay sửa nền; toàn map xa chưa chứng nhận |
| Spawn nhìn cửa, intro, một mục tiêu | PASS kỹ thuật | intro720, route/video mở đầu |
| E khám/xoay/phóng/ghi/R/P/việc thứ hai | PASS | onboarding-ui-complete; không API mở trang/ghi quan sát |
| Profile đang chơi F5/F6, không dời về spawn | PASS | onboarding-ui-complete, map-lifecycle |
| Bỏ qua hướng dẫn và load giữ lựa chọn | PASS | UI Skip trên lượt map, F5/F6 giữ flag |
| Save/load giữa tutorial trước ghi; quay lại vật sau skip | NOT_RUN | Guard fresh intro thất bại vì live Editor đã có tiến trình ngoài fixture; không reset/xóa tiến trình để ép kiểm |
| Tới điện thoại bằng controller/E sau mục tiêu thứ hai | NOT_RUN | Chỉ mục tiêu đã xác nhận; không gọi là tìm được |
| Layout thay đổi720p | PASS quan sát | intro/inspect/dossier và video |
| Layout thay đổi1080p | NOT_RUN | Không suy từ720p |
| Hồi quy toàn tuyến Hà/sáu ca/clip slider | NOT_RERUN | Không sửa các phần này; kết quả r106 giữ lịch sử |
| Menu mật mã/đối chiếu/kết luận sau đổi navigation qua UI | NOT_RUN | Kiểm code điều kiện, chưa gán PASS UI |
| Compile/reference/mở lại scene | PASS | final-live-audit,32958component,0MissingScript/0brokenref |
| Người mới hiểu bước đầu và chứng cứ | NOT_RUN | Chờ phiên thật, không thay bằng agent |

Không đổi trạng thái lịch sử r106. Các ca chưa chạy ghi rõ, không dùng assertion API thay kiểm người chơi.
