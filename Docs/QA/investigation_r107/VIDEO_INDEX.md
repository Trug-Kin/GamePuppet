# Video và ảnh r107 — UTF-8

Thư mục: `Test/BangChung/investigation_r107/`.

| File | Loại input / nội dung | Giới hạn |
|---|---|---|
| `onboarding-start.mp4` | Controller + chuột/phím **tổng hợp**, mở đầu từ spawn, cửa/cầu thang, E dấu sàn, xoay/phóng, ghi và P, F5/F6 | Im tiếng;172,83s, có114s đứng chờ tooling trong inspect. Không API mở trang/Observe/Save/Load. Không phải người mới |
| `onboarding-short.mp4` | Bản xem ngắn của video trên | Chỉ giữ0–28s và142–172,83s; cắt đoạn đứng chờ114s. Không tăng tốc hoặc ghép lại trạng thái; giữ bản đầy đủ và `onboarding-short-edit.json` |
| `map-controller.mp4` | Controller tổng hợp, từ xưởng đi76điểm tới vùng báo lỗi và qua lại | Im tiếng, không teleport; A* chỉ lập đường, input thực qua controller. Không phải camera fixture chứng minh đường đi |
| `map-final-before-3.png` / `map-final-after-3.png` | Camera chẩn đoán đúng eye/rotation ảnh vùng hở105627 | Render chẩn đoán, không ảnh từ lượt controller |
| `map-final-before-2.png` / `map-final-after-2.png`, `*-top.png`, `*-underside.png` | Các góc bổ sung để kiểm mặt nối/hướng mặt | Không gán là pose ảnh105506/105520/105534 |
| `map-route-69.png`..`map-route-75.png` | Ảnh tại waypoint controller trong video map | Kết hợp log tọa độ/grounded, không tự coi screenshot chứng minh collider |
| `first-inspection-720.png`, `first-floor-recorded.png`, `first-note-dossier.png`, `second-objective.png` | UI thực trong lượt input tổng hợp | Hình vật chứng/nhân vật vẫn giới hạn bản dựng thử theo r106 |
| `map-lifecycle.png` | Sau kiểm bàn phím F5/F6/pause vùng sửa | Dùng log map-lifecycle để xác minh input/trạng thái |

Các ảnh `map-before-*`/`map-after-*` giai đoạn đầu **superseded**: chưa sửa xong vết rách kề. Các request capture/controller trước bị guard `Fresh normal spawn required` hoặc thiếu bridge là **FAIL**, không phải bằng chứng tuyến đã đi. `map-lifecycle-superseded.txt` giữ ca input không được nhận; lần kiểm sau có log nhận input và displacement thực.

Clip sửa R-12 và bàn dựng giữ r106: **animation minh họa**, không quay tai nạn thật hoặc bằng chứng vật lý. Đợt này không xuất lại clip/kiểm sáu diễn biến vì luật/animation không đổi. Video mở đầu/map là capture Editor hiện hành, không build hoặc runner mới.

Playtest người mới r107: **NOT_RUN**. Chỉ phản hồi r106 được người dùng cung cấp là dữ kiện thực, không dựng một phiên mới từ video tổng hợp.

Thông số capture: cửa sổ Game1280×780 (gồm chrome/toolbar), vùng chơi1280×720. Video map110,47s; đồng hồ fixture113,49s vì capture bắt đầu sau request và kết thúc sau marker hoàn tất. Không tăng tốc video. Xem `final-video-metadata.json`; filmstrip map đã đối chiếu hình nền ở các mốc4/45/68/82/95/110giây. Các mốc video không phải timestamp mô phỏng cơ cấu.
