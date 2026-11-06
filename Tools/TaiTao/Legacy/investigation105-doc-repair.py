from pathlib import Path
p = Path('Test/KetQua/investigation_r104/VIDEO_INDEX.md')
p.write_text('''# Video và phạm vi r104

Tất cả input là **tổng hợp**, video **không âm thanh**. Playtest người thật: **NOT_RUN**. Quay GameView Editor, không tạo build/player/runner mới.

| Video trong Test/BangChung/investigation_r104 | Loại / phạm vi |
|---|---|
| controller-route.mp4 | Controller gốc qua InputSystem keyboard/mouse, không teleport/dolly; spawn trước xưởng → Hà → bàn dựng. Log 21/21 và 60,223 giây; capture có độ trễ khởi động nhỏ. E/raycast Khải/Hà/rối/đạo cụ. Trước chỉnh cuff và góc/actor bàn dựng cuối; map/controller không đổi sau lượt này. |
| slider-direct-1080.mp4 | Chuột GameView qua SendEvent: giữ/kéo xuôi/ngược/thả clip và mô hình, phát/dừng/đổi góc. API mở trang/profile fixture ban đầu, không seek hoặc gán quan sát. Kèm slider-ui-1080.txt; 720p có log/ảnh riêng. |
| clip-and-six-cases-final.mp4 | Chuột UI phát clip/sáu nút diễn biến; API chỉ mở trang đầu, không gán kết quả. Bản cuối sau sửa layer/rig/camera/khoảng đứng operator. Clip là animation game minh họa, không phải controller footage. |
| clip-and-six-cases.mp4 | Trước sửa camera tay/layer operator; **superseded**, không dùng làm hình cuối. |
| FAIL-capture-black.mp4 | Capture đen dù UI log chạy; **INVALID** bằng chứng video. |
| FAIL-presentation-coordinate.mp4 | **FAIL** tọa độ cửa sổ; không tính PASS. |

Game render 720p/1080p, video canvas có thêm toolbar (1280×780 hoặc 1920×1140). Xem video-metadata.json và log ffmpeg. Ảnh clip/model layout là fixture render, không chứng minh UI click; ảnh Blender không chứng minh controller. `table-prompt-raycast.png` chụp trực tiếp trong lượt đi trước E. `operator-hands-final.png`/`operator-overview-final.png` kiểm pause/đổi góc qua nút; thời gian/góc giữ nguyên theo operator-ui.txt.

Hình trước sửa được thay thế bằng hậu tố final và bộ source-final render lại từ nguồn cuối. Giữ file FAIL để truy vết, không gom vào PASS. Diễn xuất/Tư/hình nộm và vải còn **provisional**; chưa xác nhận mọi frame tiếp xúc hoặc người mới hiểu.

Khôi phục tiếng Việt trong đợt r105 từ tên file và báo cáo r104; không đổi trạng thái hay phạm vi bằng chứng lịch sử.
''', encoding='utf-8')
p = Path('Test/KetQua/investigation_r104/README.md')
s = p.read_text(encoding='utf-8-sig')
s = s[:s.index('Ch?t operator:')] + '''Chốt operator: `operator-layer-fix.txt` và `operator-ui.txt` bổ sung báo cáo art-integration ban đầu: **7 actor native** (thêm operator), **1 hình nộm trên gác trung tính**, **Tư sự kiện riêng còn prototype**. Gốc raycast tuyến và toàn bộ map không đổi. Khoảng đứng operator minh họa `(1.1,0,-4.25)` so với tay nắm `(1.54,1.2,-4.5)` trong stage, tránh đường dây đi qua đầu. Không suy clip định danh người kéo từ model operator của giả thuyết. Đoạn này được khôi phục tiếng Việt trong đợt r105; không đổi giới hạn bằng chứng r104.
'''
p.write_text(s, encoding='utf-8')
assert '?' not in s[s.index('Chốt operator:'):]
assert '?' not in Path('Test/KetQua/investigation_r104/VIDEO_INDEX.md').read_text(encoding='utf-8')
print('Reopened UTF-8: restored index and final README paragraph; no lost-character question marks.')
