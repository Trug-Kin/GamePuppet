from pathlib import Path
root=Path('D:/ConRoi')
updates={
'BAT_DAU_TAI_DAY.md': '''## 05/11/2026 — hiện hành: r105, chuẩn bị playtest điều tra

Mở **[JourneyPresentation_v48_r105.unity](Assets/Scenes/JourneyPresentation_v48_r105.unity)** bằng Unity **6000.6.2f1**, Play từ trước xưởng. WASD/chuột, E, R trả/đóng, P hồ sơ, Esc pause, F5/F6 hồ sơ riêng. Mục tiêu: khám xưởng → hỏi Hà → trở lại bàn dựng. Scene lưu hồ sơ trống, không tự load. Namespace `conroi.investigation.v36.r105`, không dùng save gameplay cũ.

[Bàn giao r105](Test/KetQua/investigation_r105/README.md) · [Video](Test/KetQua/investigation_r105/VIDEO_INDEX.md) · [Checklist](Test/KetQua/investigation_r105/CHECKLIST.md) · [Playtest người mới — NOT_RUN](Test/KetQua/investigation_r105/PLAYTEST_NGUOI_MOI.md).

Sửa tay/dụng cụ/dây sửa Tư, hiện dây treo R-12, làm rõ vai giả thuyết và prompt bàn dựng. Giữ Quaternius, map, luật hồi và checkpoint r101–r104. Tư/diễn xuất còn minh họa; chưa có người mới chơi. Các mốc bên dưới là lịch sử.

''',
'Docs/PROJECT_GUIDE_AND_HISTORY.md': '''## 2026-11-05 — r105: sửa trình bày chứng cứ để người mới thử

Hiện hành `Assets/Scenes/JourneyPresentation_v48_r105.unity`, Unity **6000.6.2f1**. GDD v3.6, tuyến xưởng→Hà→xưởng và map giữ nguyên; checkpoint r101–r104 không đổi. Bàn giao `Test/KetQua/investigation_r105/README.md`, checklist, video index, nguồn và phiếu người mới (**NOT_RUN**).

Review clip r104 xác nhận tay/dụng cụ lệch vùng sửa; đầu dây nhảy tại 4,8 giây; dây treo có trong Blender nhưng layer 0 bị camera layer 30 bỏ qua. Nguồn r105 chỉnh hai tay và dụng cụ weighted, thả dây chuyển tiếp 0,35 giây theo time; giữ root mất tựa, thời điểm sự kiện và camera. Hiện dây treo hiện có trong Unity, không đổi kết cấu/luật hồi để bù hình. Tư vẫn art minh họa, hình nộm trung tính; không xác nhận mọi frame va chạm.

Bàn dựng ghi rõ vai giả thuyết, trường hợp “Người giúp mở”, không tự thêm Khải vào kết luận. Prompt “E · Dùng bàn dựng lại” đã chụp khi controller đi từ spawn tới bàn, E/raycast thật (input tổng hợp), 1/1 đoạn 6,74 giây. Không gán thành đã chạy lại tuyến Hà. Clip/sáu trường hợp quay qua nút UI, im tiếng; slider kéo xuôi/ngược/thả/phát, đổi góc và pause/load phần chịu ảnh hưởng kiểm tại 720p/1080p. Lifecycle API có năm load lặp không trôi/sinh trùng. Mở lại scene sạch: Missing Script 0, broken reference 0. Tư/dụng cụ và chu kỳ hồi đầy đủ ngoài cửa sổ mô hình 10 giây còn giới hạn, không chấm toàn bộ trình bày PASS.

R104 VIDEO_INDEX và đoạn cuối README đã viết lại ký tự tiếng Việt mất, mở lại UTF-8; giữ FAIL/superseded, tên file và giới hạn lịch sử. Hash trước/sau tài liệu riêng. Danh sách bảo vệ thực tế: **1.656 file**, gồm **17 save gốc**, giữ hash; không suy rộng toàn dữ liệu. Script R105 cách ly 10 GUID/namespace/save; art Quaternius R104 chỉ dùng chung đọc. Không build/ZIP/runner sản phẩm, không đổi map/controller/migration Bảo.

''',
'Docs/BLENDER_UNITY_WORKFLOW.md': '''## 2026-11-05 — r105: animation tiếp xúc và binding chứng cứ

Nguồn `Source/Blender/JourneyV48/R105/Repair_R12_R105_Animation.blend` → `Assets/Art/InvestigationV36/R105/RepairEventActor_R105.fbx` → `Repair_clip_illustration/EventActor_R105` trong scene `JourneyPresentation_v48_r105.unity`. Xuất rig/body/tool weighted và Legacy clip `Scene` 12 giây; nguồn đã lưu/mở lại, FBX roundtrip theo `Test/KetQua/investigation_r105/source-final.json`.

Tay trái đặt vùng (.68,4.20,1.42), tay phải (.28,4.15,1.35) trong clip local Unity; dụng cụ handle/shaft weighted vào handL. `InvestigationSession` cập nhật repairEventActor/repairEventMotion/clipTu/clipLeftHand/clipRightHand; bỏ root event cũ chỉ trong r105. `RepairHandBinding` tham chiếu handR mới, attachment R-12 và clipSpace; thả tại 4,8 giây chuyển smoothstep 0,35 giây không cache, tua ngược/load xác định. Ba dây treo sẵn đưa vào layer 30 phù hợp camera; material URP riêng R105. Không đổi camera, luật hồi, mặt sàn hay actor tuyến.

Script dưới `Assets/Scripts/InvestigationV36R105`, 10 GUID/namespace R105; schema1/save `conroi.investigation.v36.r105`, `InvestigationV36/r105`. Art Quaternius R104, cuff và checkpoint không sửa. Reconstruction root operator tên `NguoiGiuDay_GiaThuyet`, UI nói rõ vai chưa định danh; hình nộm giữ trung tính. Không coi chạy mô hình là chứng minh Khải kéo.

Làm lại bằng mở nguồn đã lưu, xuất đúng rig/body/tool animation FBX, reimport và kiểm reference/layer/URP/binding tại các time 0;4,05;4,3;4,8;4,833;5,2. **Không chạy lại derive/author/isolate/bind/integrate một lần**. Recipe nguồn trong Test/Tam là lịch sử, không phải bước cần để mở scene. Nguồn license r104 giữ nguyên; r105 không tải asset mới.

Ảnh trước/sau và video ở `Test/BangChung/investigation_r105`; kết quả ở `Test/KetQua/investigation_r105`. Clip minh họa còn giản lược, dụng cụ nhỏ; chưa kiểm va chạm mọi frame. Video dùng UI tổng hợp, không có âm thanh, không thay playtest người thật (**NOT_RUN**). Kiểm mở lại scene/reference, slider720/1080, pause/load giới hạn đúng phần thay; danh sách bảo vệ1656 file/17 save theo manifest. Không sửa map hoặc kéo dài polish phần chưa ảnh hưởng hiểu sự kiện.

'''}
for name,block in updates.items():
 p=root/name;s=p.read_text(encoding='utf-8-sig')
 if s.startswith(block): continue
 assert 'r105: animation tiếp xúc' not in s and 'r105: sửa trình bày chứng cứ' not in s and 'hiện hành: r105' not in s
 p.write_text(block+s,encoding='utf-8')
print('Prepended current r105 records; historical sections retained.')
