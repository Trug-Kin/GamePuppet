# Kết quả theo loại bằng chứng

| Nội dung | Trạng thái | Căn cứ / giới hạn |
|---|---|---|
| Dấu sàn, vân/khuyết giày A–B, mép vải/chỉ, nhãn R03/R12 | ĐÃ XEM HÌNH | Source renders và `ui-720-*`/`ui-1080-*`; còn hình stylized, chưa kiểm người mới nhận ra |
| Nguồn Blender và FBX | ĐÃ LƯU/MỞ LẠI, ROUNDTRIP | `sole-final.json`, `mechanism-source.json`, `clip-animation-source.json`. Không suy mọi chi tiết đã final từ audit topology |
| Controller xưởng → Hà → xưởng | ĐÃ ĐI | `route-motion.txt`, `controller-route-final.mp4`; synthetic InputSystem, không teleport, im tiếng |
| Mật mã nhập sai/xóa/sửa và liên hệ tự viết | ĐÃ KIỂM UI | `ui-functional.txt`, `ui-finish.txt`; dùng đáp án đã biết, không đánh giá khả năng suy luận mới |
| Nhận định “Chưa đủ chứng minh” | ĐÃ KIỂM UI | `conclusion-ui.txt`: xác nhận, lưu, ba lần Tải; không API gán finalized |
| Clip phát/dừng/phát lại/tua +1s | ĐÃ KIỂM UI | `clip-controls-ui.txt`; nút tua và video. Kéo slider chưa có lượt đạt, không gộp vào kết quả này |
| Clip quay sự kiện | BẢN DỰNG THỬ | 12s liên tục, hai tay có đồ sửa, chốt/cánh và mất tựa đọc được; không thấy người kéo/ĐK. Dây đỡ rối, diễn xuất sửa và art còn cần hoàn thiện |
| Sáu diễn biến | ĐÃ CHẠY QUA NÚT UI | `presentation-ui.txt`, `clip-six-cases-ui.mp4`; ID thực tế khớp từng nút, cùng Advance; không là mô phỏng vật lý tự do hoặc chứng minh ngoài đời |
| Khoảng hồi sau6s | ĐÃ ĐO MÔ HÌNH | Chưa cài tại0.994°; khoảng hồi/cài7.4333s. Không loại mở trước bằng đổi spring |
| R/P khi gõ; Esc đóng/pause/resume; Esc+E cùng frame; xoay/zoom | ĐÃ KIỂM INPUTSYSTEM | `input-edges.txt`. Fixture dùng runtime input settings nhận mọi thiết bị rồi phục hồi; lượt phụ thuộc focus trước đó FAIL giữ riêng, không sửa controller/input asset |
| Pause/load giữa hoạt động, namespace cũ, không trùng vật | ĐÃ KIỂM API RIÊNG | `lifecycle-api.txt`; không thay kiểm UI/controller. Lần dựng đang chạy reset an toàn khi load |
| Hai độ phân giải | ĐÃ XEM BỐ CỤC | `ui-layout.txt`:1280×720 và1920×1080, fixture API. Không kiểm tất cả tỷ lệ cửa sổ; phần clip kiểm thêm tại720p |
| Scene reopen / Missing Script / references | XEM LOG CUỐI | `final-scene-audit.txt`, `reopen-references.txt`; phạm vi component đã ghi, không mọi ref toàn project |
| Checkpoint/save | ĐỐI CHIẾU ĐÚNG DANH SÁCH | `protected-final.json`:1.325 file,17 save gốc; không tất cả dữ liệu trên máy |
| Bộ art bên ngoài phù hợp | OPEN | Quaternius CC0 mới đánh giá trang/ảnh, chưa tích hợp hoặc kiểm rig. Khải/Hà/rối tự dựng vẫn tạm |
| Người mới chưa đọc GDD | **NOT_RUN** | `PLAYTEST_NGUOI_MOI.md`; agent biết đáp án không thay được người chơi mới |

Các lượt lỗi tọa độ click, capture đen, fixture focus và tuyến bị hook che ray được giữ để truy nguyên; không dùng làm bằng chứng đạt. Không mở map, cốt truyện, năm thư hoặc ending. Không build/ZIP/runner sản phẩm mới.
