# Con Rối — source r107, chuẩn bị nhóm Git

Entry nhóm: **[Assets/ConRoi/Modules/TichHop/Scenes/ConRoi_Chinh.unity](Assets/ConRoi/Modules/TichHop/Scenes/ConRoi_Chinh.unity)**, Unity **6000.6.2f1**. Copy r107 với GUID scene mới; checkpoint giữ nguyên. Nhánh `chore/prepare-r107`, source commit `40bcb67035b7`; **chưa push, không remote**.

[Bàn giao thực tế và phần chặn](Docs/QuanLy/HANDOFF.md) · [Ownership](Docs/QuanLy/OWNERSHIP.md) · [Clone/LFS/profile sạch](Docs/QuanLy/CLONE_VA_BAT_DAU.md) · [QA](Docs/QA/README.md) · [Mapping/hoàn nguyên](Docs/QuanLy/OLD_NEW_PATHS.md).

Onboarding/khám/ghi/P/save đã kiểm qua input tổng hợp trên scene copy; người mới NOT_RUN. LFS objects local kiểm OK; **checkout sạch BLOCKED vì thiếu dung lượng**, còn Assets/Docs chưa phân loại. Không coi đã sẵn sàng push hoặc toàn bộ archive hoàn tất. Không chạy author/derive/bind một lần hoặc dùng profile kỹ thuật đã giải. Dừng tính năng tại r107; các phần dưới là lịch sử.

# Dữ liệu kiểm thử đã tách

Source sản phẩm tại D:/ConRoi; [Test/README](Test/README.md) chỉ dẫn ZIP/player, ảnh/video, report và output cấu hình được. Không tạo build mới trong đợt này. Docs/Reviews còn nguồn/fixture/checkpoint được giữ, không coi toàn bộ là output kiểm.

# CON RỐI — source r92 tại D:/ConRoi

Đã tổ chức script theo chức năng và Việt hóa 100 file, giữ gameplay/model/save và ZIP r90/r91/r92. Mở ConRoi.code-workspace hoặc Unity Hub ConRoi; scene Assets/Scenes/JourneyPresentation_v48_r92.unity.

[Hướng dẫn, kiểm, đổi tên, hoàn nguyên](Docs/PROJECT_GUIDE_AND_HISTORY.md) · [Workflow Blender/Unity](Docs/BLENDER_UNITY_WORKFLOW.md) · [Builds](Builds/README.md).

Phần dưới là lịch sử bàn giao.

---

# CON RỐI — source r44 tạm chốt

Phiên blocker tiếp theo chưa hoàn tất: [kết quả từng mục](Docs/Reviews/journey_v48/r44-content/blocker-session-run-20261025/REVIEW.md). Các blocker còn NOT_RUN; lỗi điều khiển/chọn runner được ghi riêng, không sửa sản phẩm/capture hoặc build. SAC cuối phiên 17:18:42 +07:00 ngày 25/10 vẫn Off/0; đã nhắc bật lại On, chưa xác nhận phục hồi. Mốc r44 và kết quả Editor trước đó giữ nguyên; xác nhận On ở đoạn dưới là lịch sử trước phiên Off mới này.

Giữ r44 làm mốc đã kiểm Editor: `Assets/Scenes/JourneyDevelopment_v48_r44.unity`. Build Settings vẫn bật r42; chưa xuất build r44. Đã compile/mở/lưu–đóng–mở scene và chạy Play Mode bốn khu tới ending A: 157 assertion gameplay đạt; kết quả công cụ full complete=false do capture menu, tách riêng và chưa sửa capture. Edge dốc 1080p PASS 77 assertion. Các ca còn thiếu được phân loại trong UNITY_PENDING; không chạy lại PASS khi không có thay đổi liên quan. **SAC đã xác minh On/1 lúc 14:05:19 +07:00 ngày 25/10/2026.** Không thêm tính năng/công cụ/build.

[Kết quả Editor/Play Mode](Docs/Reviews/journey_v48/r44-content/editor-sac-off-20261025/REVIEW.md) · [Video ngắn từ gameplay thật, có cắt](Test/BangChung/journey_v48/r44-content/editor-sac-off-20261025/r44-editor-gameplay-short.mp4) · [Checklist Unity còn chờ](Docs/Reviews/journey_v48/r44-content/UNITY_PENDING.md) · [Hướng dẫn và lịch sử dự án](Docs/PROJECT_GUIDE_AND_HISTORY.md). C# ngoại tuyến ghi riêng. Giữ r42, map/model, controller, checkpoint/save.

Các phần bên dưới là lịch sử bàn giao.

# CON RỐI — review hồ r38

[Hướng dẫn project và lịch sử xuyên suốt tới r38](Docs/PROJECT_GUIDE_AND_HISTORY.md): đọc trước khi sửa; cập nhật cùng tài liệu này sau mỗi đợt hoàn chỉnh theo checklist ở mục 10. Tài liệu phân biệt nguồn chỉnh sửa, scene/build hiện hành và giới hạn kiểm chứng.

Scene dẫn xuất đang bật trong Build Settings: `Assets/Scenes/JourneyRepair_v48_r38.unity`. Giữ checkpoint r37 và profile save r35-market. Đợt này chỉ sửa đế giày Bảo, thao tác cặp trang sổ và đồng bộ camera/pipeline phản chiếu.

[Báo cáo và giới hạn](Docs/Reviews/journey_v48/r38/REVIEW.md) · [ZIP Windows](Test/BanChoiThu/journey_v48/r38/ConRoi-v48-r38-LakeReview-Windows-x64.zip) · [Video](Test/BangChung/journey_v48/r38/r38-main-CONTINUOUS-OFFSCREEN.mp4). Chi tiết phản chiếu tay/sổ/rối bị mặt cầu che từ footway, chưa chứng minh nhìn rõ trên nước. Hiển thị/input Windows vật lý và vấn đề menu đen vẫn tách riêng.

Các mục dưới đây giữ nguyên làm lịch sử, không phải kết quả kiểm r38.

---

# CON RỐI — bàn giao v47 RC3

Ứng viên nghiệm thu tiếp tục từ checkpoint RC2 giữ nguyên. 601 kiểm tra PASS trên binary đóng gói; có kiểm launch thường, New Game/hướng dẫn/pause/lưu tải, bốn ending và đo hồ foreground không capture. Không tuyên bố đạt 30–40 phút hoặc đã loại mọi spike.

- [ZIP nghiệm thu RC3](Test/BanChoiThu/journey_v47/ConRoi-v47-RC3-Windows-x64.zip)
- [Hướng dẫn](Docs/Reviews/journey_v47_rc3/PLAYER-GUIDE.md), [bảng GDD](Docs/Reviews/journey_v47_rc3/GDD-v2.1-audit.md), [hạn chế](Docs/Reviews/journey_v47_rc3/LIMITATIONS.md)
- [Kết quả](Docs/Reviews/journey_v47_rc3/RESULTS.md), [đo foreground](Docs/Reviews/journey_v47_rc3/PERFORMANCE.md), [ánh sáng trước/sau](Docs/Reviews/journey_v47_rc3/VISIBILITY.md)
- [Video tuyến không cắt](Test/BangChung/journey_v47/ConRoi-v47-RC3-player-route.mp4)
- [Hash/kiểm ZIP cuối](Docs/Reviews/journey_v47_rc3/final-delivery-check.json)

Phần dưới giữ làm lịch sử RC2.

# CON RỐI — v47 RC2 (checkpoint)

Ứng viên chơi trọn tuyến, tiếp tục từ checkpoint v46 r2. 537 ca chức năng và 492 ca lặp trong đo hiệu năng PASS trên binary đóng gói. Bốn kết thúc đi từ đầu, lưu/tải và chơi lại đã kiểm. Spike còn lại và mục tiêu thời lượng chưa chứng nhận được ghi rõ trong báo cáo; không tuyên bố đã hết khựng.

- [Build Windows ZIP RC2](Test/BanChoiThu/journey_v47/ConRoi-v47-RC2-Windows-x64.zip)
- [Kết quả và phân loại GDD](Docs/Reviews/journey_v47/RESULTS.md)
- [Hướng dẫn điều khiển, luồng chơi, lưu/tải](Docs/Reviews/journey_v47/PLAYER-GUIDE.md)
- [Bốn kết thúc — có spoiler](Docs/Reviews/journey_v47/ENDINGS.md)
- [Hiệu năng và thử nghiệm bị loại](Docs/Reviews/journey_v47/PERFORMANCE.md)
- [Giới hạn](Docs/Reviews/journey_v47/LIMITATIONS.md)
- [Video toàn tuyến liên tục](Test/BangChung/journey_v47/ConRoi-v47-RC2-full-route.mp4)
- [Video khám phá/đọc nội dung liên tục](Test/BangChung/journey_v47/ConRoi-v47-RC2-full-experience.mp4)

## Checkpoint v46 r2 — giữ nguyên

Tiếp tục v45 r4. Hoàn thiện bề mặt sáu NPC người, ưu tiên Hà/Vy; giữ diễn xuất, tiếp xúc và quy tắc rối.

- [Kết quả và giới hạn](Docs/Reviews/journey_v46/RESULTS.md)
- [Marker và frame time](Docs/Reviews/journey_v46/PERFORMANCE.md)
- [Build Windows ZIP](Test/BanChoiThu/journey_v46/ConRoi-v46-Experience-Windows-x64.zip)
- [Clip trước/sau ở góc người chơi](Test/BangChung/journey_v46/ConRoi-v46-before-after-player.mp4)
- [Toàn tuyến sau — liên tục](Test/BangChung/journey_v46/ConRoi-v46-after-player.mp4)
- [Toàn tuyến trước — liên tục](Test/BangChung/journey_v46/ConRoi-v46-before-player.mp4)
- [Ảnh Hà trên tuyến](Test/BangChung/journey_v46/images/Ha-player-before-after.jpg)
- [Ảnh Vy trên tuyến](Test/BangChung/journey_v46/images/Vy-player-before-after.jpg)
- [Clip kiểm cận bổ sung](Test/BangChung/journey_v46/ConRoi-v46-inspection.mp4)
