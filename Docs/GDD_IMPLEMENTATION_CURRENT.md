<!-- V47_RC3_CURRENT -->
Ứng viên bàn giao hiện tại: **v47 RC3**, giữ ZIP RC2 nguyên checkpoint. [Kết quả 601 PASS trên binary đóng gói](Reviews/journey_v47_rc3/RESULTS.md), [đối chiếu GDD](Reviews/journey_v47_rc3/GDD-v2.1-audit.md), [hướng dẫn](Reviews/journey_v47_rc3/PLAYER-GUIDE.md), [hạn chế](Reviews/journey_v47_rc3/LIMITATIONS.md). Hai lượt hồ foreground không capture và một lượt capture được so cùng đoạn; không chứng nhận 30–40 phút hoặc đã loại mọi spike. Các mục phía dưới là lịch sử.
<!-- /V47_RC3_CURRENT -->

<!-- V47_CURRENT -->
Đợt hiện tại: **v47 RC2**, tiếp tục từ checkpoint v46 r2. Đối chiếu bản đầy đủ với [GDD v2.1](GDDv2.1.txt) trong [audit v47](Reviews/journey_v47/GDD-v2.1-audit.md). RC2 loại thử nghiệm render gate chưa chứng minh lợi ích. Binary đóng gói đã qua 537 ca chức năng + 492 ca lặp hiệu năng PASS; bốn ending đi từ đầu, lưu/tải và chơi lại đã kiểm. Spike hiệu năng và mục tiêu thời lượng còn mở được ghi trong [kết quả v47](Reviews/journey_v47/RESULTS.md). [Hướng dẫn](Reviews/journey_v47/PLAYER-GUIDE.md), [kết thúc](Reviews/journey_v47/ENDINGS.md), [giới hạn](Reviews/journey_v47/LIMITATIONS.md) và [quy tác vụ spike](Reviews/journey_v47/PERFORMANCE.md). Các mốc v36 trở xuống dưới đây là lịch sử, không mô tả trạng thái ứng viên hiện tại.
<!-- /V47_CURRENT -->

<!-- V36_CURRENT -->
Mốc **v36 hồ–chợ**: [kết quả và giới hạn](Reviews/lake_gate_market_current/v36/RESULTS.md). Windows đã tự kiểm và đóng gói; không phải game đầy đủ. Scene trường v37 là bước tiếp nối riêng, chưa nằm trong build v36. Phần v35 trở xuống giữ làm lịch sử.
<!-- /V36_CURRENT -->

<!-- V35_CURRENT -->
Cập nhật đợt **v35**: cầu Ông Đạo tích hợp từ v34; movement/collider: PASS, hồi quy tuyến: EXECUTED_REVIEW_FAILURES; luồng phòng trọ–hồ–chợ: PASS. Hội thoại Hà/nhòe mặt/lưu tải có kết quả chạy riêng trong [đối chiếu đợt](Reviews/lake_gate_market_current/v35_ong_dao.md). Các chương sau chợ, menu/ending/toàn game và Windows build chưa hoàn tất. Dùng GDD v2.1; các thông tin v30–v34 bên dưới là lịch sử triển khai.
<!-- /V35_CURRENT -->

V34 — trạng thái mới nhất 16-10-2026

Scene `Assets/Scenes/LakeMarketStory_v34.unity`; Blender `ntmk_full_map_lake_market_story_v34.blend`. Giữ nguồn v32/v33, 143 collider môi trường và controller 1,80 m / radius 0,30 m. Không dùng asset ngoài mới.

Chức năng `v34_run_20261016_073153/result.json`: 23 ca, FOCUSED_CASES_PASS. E qua PlayerInteraction raycast thật; P/phím số qua Input System. Kiểm điện thoại/chống farm, chuyển hồ–hub–chợ, spawn, lịch/sách tùy chọn, lưu/tải, GĐ1/GĐ2 thực thi và phản chiếu hồ render. Ca chức năng dùng fixture đặt vị trí cạnh tương tác, không phải walkthrough liên tục. Fallback 90 giây chỉ được tính theo kết quả ca thực thi trong JSON. Input OS NOT_VERIFIED.

Movement `unity_run_20261016_070719/result.json`: PASS, đi 279/279 mốc mỗi chiều với đạo cụ hai khu hiện diện, WASD/mouse/dừng/dốc/lan can. Không teleport trong traversal, không tắt collision; 0 xuyên nền/thiếu support. Lượt này chạy trước bổ sung shader phản chiếu/âm nền/kim đồng hồ; các thay đổi sau không sửa geometry/controller.

Còn thiếu kiểm chứng: walkthrough liên tục mọi tương tác; hội thoại Chị Hà và crossfade mặt/điều kiện chứng kiến; chất lượng phản chiếu/âm thanh/hiệu năng. Model nhân vật còn đơn giản. Không gọi đây là game hoàn chỉnh hoặc mời nghiệm thu. Trường/dốc/ending/build toàn game chưa hoàn tất. Các lượt lỗi giữ nguyên evidence.

Ảnh: `v33_images/v33_unity_materials.jpg`, `v33_images/v33_gate_readability.jpg`, `v33_images/v33_blender_materials.jpg`, `v33_images/v34_story_overview.jpg`. Tổng hợp số liệu: `v34_verification.json`.

# CON RỐI — đối chiếu GDD v2.1

Ưu tiên hiện tại theo chỉ dẫn mới: movement → hoàn thiện model/map và gameplay theo từng khu, bắt đầu hồ–cổng–chợ. Dừng mở rộng hệ gameplay toàn game. Scene Assets/ConRoiGame/ConRoi.unity là phần tích hợp thử CHƯA KIỂM CHỨNG, không nằm trong build list và không dùng làm bàn giao.

Căn cứ duy nhất về thiết kế: `Docs/GDDv2.1.txt`. Không dùng phạm vi demo mục 10 để cắt yêu cầu bản đầy đủ. Các tài liệu audit cũ chỉ là bằng chứng lịch sử.

| Phần | Đã có | Thiếu / cần sửa |
|---|---|---|
| Điều khiển | Capsule 1,80 m / 0,30 m; probe Input System WASD/dừng đạt; 9 ca C3 chạy tới đích | Input OS bị chặn foreground; W-route tới đích nhưng có 4 frame mất grounded ở một đoạn xuống dốc |
| Camera | MouseMovement yaw thân/pitch camera; virtual mouse đã kiểm góc quay thực | Chưa kiểm bàn phím/chuột OS hoặc nghiệm thu sensitivity |
| Bản đồ | v32 từ v31: tuyến bờ hồ–cổng–phố/C3–W2/P32/W1 đã tích hợp; Play Mode 042656 đi đủ 279/279 mốc mỗi chiều, 0 mất grounded trên tuyến; 143 collider chung mesh hiển thị, controller giữ nguyên. Chi tiết: Docs/Reviews/lake_gate_market_current/README.md | Ngoài tuyến còn placeholder/chưa kiểm; shader nguồn chưa bake đầy đủ, bảng cổng nhìn xa tối; 1 frame mất grounded khi ép lan can (không xuyên nền); OS input chưa xác minh. Chưa phải scene gameplay đầy đủ |
| Tương tác | Raycast E, giường, điện thoại, trigger dấu vết | Prompt, hội thoại, điện thoại thống nhất, tương tác sau load |
| Tiến trình | Manager Thức/Mơ, vùng, scoring | Bốn vùng nối tiếp và quay về hub; dấu vết phải luôn tùy chọn |
| Con Rối | GĐ1/GĐ2, prototype GĐ3 | GĐ3 đi trước 1–2 s; GĐ4 duy nhất trước ending; khuôn mặt Minh, animation thực |
| Compliance | Camera nhích prototype | Ba mức, delay 150–250 ms, cập nhật cuối vùng, âm thanh |
| Chỉ số/ending | Hai điểm ẩn, bốn nhánh logic | Ngưỡng Hành động dùng số lần tương tác >=2 theo bảng baseline; điểm có trọng số vẫn lưu riêng |
| Hình/âm | Asset môi trường và puppet nguồn | Năm dây, nhòe mặt 0,08 s, dấu vết bền vững, ambience và giới hạn hai SFX |
| Vignette | Chưa tích hợp | Bà Lan; Vy tại chợ tối đa một lần và không xác nhận danh tính |
| Luồng sản phẩm | Scene prototype | Menu, pause, settings, save/load, New Game → ending, hướng dẫn điều khiển |
| Bàn giao | Project Unity 6000.6.2f1 | Build Windows độc lập, thử build và mọi ending; chưa đủ điều kiện nghiệm thu |

Quyết định diễn giải nhỏ: mục 5.3 có điểm Hành động +1/+3/−1 nhưng ngưỡng ghi rõ “>=2 lần tương tác”. Lưu cả điểm và số lần; xét trục cao bằng số lần tương tác đời thực chủ động, bỏ qua tin nhắn không được tính là một lần hành động tích cực. Nhận thức ending chỉ tính bốn dấu vết chính; Bà Lan là vignette tùy chọn, không nâng mẫu số.

Kiểm chứng hiện tại: Input System virtual-device → actions → controller: WASD, dừng và mouse action đã chạy đạt ở Editor, không có delegate hook. Không phải kiểm thử bàn phím vật lý. Windows từ chối foreground nên lớp input OS bị chặn. Chín ca C3 đã chạy; W-route chạy hai chiều tới đích, còn 4 frame mất grounded thoáng qua. Bờ hồ P3 có bốn ca đạt, xem `Reviews/lake_gate_market_current/README.md` và dữ liệu gốc. Chưa chạy New Game → ending hoặc build.

