# Bàn giao trình bày chứng cứ r105

Scene: `Assets/Scenes/JourneyPresentation_v48_r105.unity`, Unity **6000.6.2f1**. Mở scene, Play từ trước xưởng. Map và checkpoint r101–r104 được giữ; không mở thêm nội dung. Đây là bản đủ để bắt đầu playtest, không phải xác nhận người mới đã hiểu vụ việc.

## Lỗi đã xác nhận và sửa

| Thời điểm clip | Trước r104 | Sau r105 |
|---|---|---|
| 0–4,8 giây | Tay phải ở đầu R-12 trong khi điểm sửa dây ở thấp; dụng cụ trái rời vùng thao tác | Chỉnh hai tay trên rig hiện có tới vùng vai/dây; cán dụng cụ gắn trọng số cứng vào bàn tay trái |
| 4,8 → 4,833 giây | Đầu dây đổi từ tay sang vị trí buông trong một bước | Chuyển tiếp xác định theo thời gian trong 0,35 giây, tua ngược/load không dựa cache frame trước |
| Trước/trong sự cố | Dây treo có trong Blender nhưng layer 0 bị camera layer 30 loại | Đưa ba dây treo hiện có vào layer 30; R-12 đọc được điểm treo |
| Bàn dựng | Model operator và tên trường hợp dễ gợi danh tính | Nhãn vai giả thuyết; trường hợp “Người giúp mở”; tên root `NguoiGiuDay_GiaThuyet`, giảm dấu hiệu cuff trong minh họa |
| Raycast bàn dựng | “Dựng lại bàn dựng lại” | **“E · Dùng bàn dựng lại”**, đã chụp khi controller đi từ spawn tới bàn |

Ảnh trước/sau cùng camera tại `Test/BangChung/investigation_r105/{before,after}-unity-*.png`; 0; 3,8; 4,05; 4,3; 4,8; 4,833; 5,2; 6 giây. Đây là lấy mẫu thời gian qua API để review hình, không phải bằng chứng chơi controller. Ảnh Blender có tiền tố tương ứng. Camera, thời điểm chốt nhả, chuyển động gốc mất tựa và luật hồi không đổi.

## Nguồn và cách tái tạo

`Source/Blender/JourneyV48/R105/Repair_R12_R105_Animation.blend` → `Assets/Art/InvestigationV36/R105/RepairEventActor_R105.fbx` → `Repair_clip_illustration/EventActor_R105` trong scene. FBX gồm rig, body, dụng cụ weighted và animation Legacy `Scene` 12 giây. `InvestigationSession.repairEventActor/repairEventMotion/clipTu/clipLeftHand/clipRightHand` trỏ bản mới. `RepairHandBinding` gắn vào actor, tham chiếu handR, điểm sửa trên R-12 và clipSpace.

Blender đã lưu, mở lại, kiểm khoảng tay và FBX roundtrip (`source-final.json`). Xuất lại các object rig/body/tool từ nguồn đã lưu, giữ transform và animation, reimport đúng FBX rồi kiểm reference, vật liệu URP, dây layer 30. Không chạy lại script derive/author/bind/integrate một lần. Recipe sửa nguồn trong `Test/Tam/investigation105-source-contact.py` chỉ là lịch sử thao tác, không cần chạy để mở sản phẩm.

10 GUID script cách ly trong `Assets/Scripts/InvestigationV36R105`, namespace `ConRoi.Investigation.V36.R105`; các art Quaternius r104 không sửa dùng chung. Không sửa controller, map, migration Bảo hoặc cơ cấu để bù hình. Tư vẫn dùng rig minh họa hiện có; hình nộm bàn dựng trung tính, không phải art nhân vật cuối.

## Bằng chứng và giới hạn

Xem [CHECKLIST.md](CHECKLIST.md), [VIDEO_INDEX.md](VIDEO_INDEX.md), [ASSET_SOURCES.md](ASSET_SOURCES.md), [phiếu playtest](PLAYTEST_NGUOI_MOI.md). Phân biệt chuột/phím tổng hợp, API trạng thái, render animation và người thật. Lượt controller r105 chỉ từ spawn tới bàn dựng; không tuyên bố đã đi lại toàn tuyến Hà. Không thay rig/collider/interaction trên tuyến nên không chạy lại mọi ca r104.

Clip cho thấy cánh đóng/chốt cài trước sự cố, chốt nhả/cánh chuyển động và Tư mất tựa trong một góc liên tục. Không thấy ĐK/người kéo. Dụng cụ nhỏ và diễn xuất còn giản lược; chưa kiểm mọi frame va chạm hoặc tính chân thực vật lý. Không suy thời điểm kéo/danh tính/ý định từ vải, giày hay việc chọn một trường hợp mô hình. Bàn dựng giữ sáu đầu vào chung luật; hồi hữu hạn, có thể chưa cài sau sáu giây.

Save schema 1, namespace `conroi.investigation.v36.r105`, thư mục `InvestigationV36/r105`. Test dùng thư mục riêng dưới bộ kết quả, không đọc/migration save cũ. Số file và save bảo vệ thực sự đối chiếu nằm trong `protected-final.json`; không suy rộng toàn máy. r104 VIDEO_INDEX và đoạn cuối README được viết lại tiếng Việt UTF-8, giữ tên file, FAIL/superseded và phạm vi lịch sử; hash sửa tài liệu ghi riêng.

**Playtest người thật: NOT_RUN.** Đợt này chuẩn bị để người mới thử; chưa có kết quả về mức hiểu, danh tính nghi ngờ hay số lần cần gợi ý.
