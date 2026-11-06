from pathlib import Path
R=Path('D:/ConRoi');D=R/'Test/KetQua/investigation_r104'
p=D/'README.md';t=p.read_text();t=t.replace('Hình nộm bàn dựng và Tư sự kiện còn trung tính prototype','Hình nộm trên gác bàn dựng và Tư sự kiện còn trung tính prototype; operator bàn dựng dùng Khải mới').replace('six native rigs','seven native rigs').replace('trên6 actor giữ root tương tác cũ','trên7 actor giữ root tương tác cũ (gồm operator bàn dựng)').replace('Actor cũ đã gỡ khỏi6 root mới','Actor cũ đã gỡ khỏi6 root tuyến/clip mới; operator tái bind đúng root FBX hiện có').replace('Đã sửa góc tay bị tường che;','Đã sửa góc tay bị tường che và layer0 của mesh operator bị camera layer29 bỏ qua;');p.write_text(t,encoding='utf-8')
p=D/'ASSET_SOURCES.md';t=p.read_text().replace('Tư sự kiện, operator/hình nộm bàn dựng: art tự dựng prototype kế thừa r103, chưa thay Quaternius','Tư sự kiện và hình nộm trên gác bàn dựng: art tự dựng prototype kế thừa r103; operator dưới gác đã dùng Khải Quaternius, sửa layer29 và native rig/idle');p.write_text(t,encoding='utf-8')
intro='''## 2026-11-05 — r104: art thật và tập trung quan sát điều tra

Scene hiện hành `Assets/Scenes/JourneyPresentation_v48_r104.unity`, Unity **6000.6.2f1**; GDD v3.6 và phạm vi xưởng→Hà→xưởng giữ nguyên. Map r101 đóng băng, checkpoint r101–r103 giữ nguyên. Bàn giao chi tiết: `Test/KetQua/investigation_r104/README.md`, `ASSET_SOURCES.md`, `CHECKLIST.md`, `VIDEO_INDEX.md` và `PLAYTEST_NGUOI_MOI.md` (**NOT_RUN người thật**).

Khải/Hà/rối đã tải/import/tích hợp thật từ đúng gói Quaternius Ultimate Animated Characters CC0; nguồn gốc tải trong Source/Downloads/Quaternius_R104. Assets/Model có191 OBJ nhưng thiếu rig/license được tìm thấy nên không chọn. Khải/Hà idle có rig; R03/R12/cipher/clip có pose chỉnh được. Cuff nhân vật và mẫu đối chiếu đồng bộ đường mép bù của mảnh vải; loại/màu vải không tự chứng minh người kéo/thời điểm/ý định. Operator bàn dựng dùng Khải mới, hình nộm trên gác và Tư sự kiện vẫn art tạm trung tính.

Clip lớn giữ tỷ lệ, một camera liên tục thấy chốt/cánh đóng trước t4, nhả/chuyển động và mất tựa t4,8, không thấy ĐK/người kéo. Sửa giá đỡ che tay bằng nguồn Blender, dây sửa thả thay vì kéo xuyên lúc ngã; animation im tiếng/diễn xuất còn provisional, không gọi final. Bàn dựng có tổng thể/cận cánh/cận tay, dừng/tua/đổi góc giữ thời điểm. Sửa góc tay nhìn vào tường, dịch hình nộm khỏi trục bản lề, sửa operator layer0 bị camera29 bỏ qua và chuyển đúng rig/idle/IK. Không sửa ReturnGateMechanism: code normalized giống r103, hồi7,4333s, sau6s chưa cài và lực mới còn mở lại.

Controller gốc đi21/21 mốc từ spawn sạch xưởng→Hà→xưởng bằng input tổng hợp không teleport. Prompt bàn dựng đã chụp trực quan trước E bằng raycast trên tuyến. Slider kiểm MouseDown/hold/drag xuôi/ngược/release tại720/1080p; tua không tự mở quan sát, UI không xoay camera người chơi. Mật mã sai/xóa/sửa, liên hệ, nhận định và lưu chưa đủ chứng minh đã thao tác UI biết đáp án. API lifecycle riêng kiểm pause/load đang tua/chạy mô hình, tải lặp không trôi/sinh trùng; clip load về0/dừng, mô hình reset an toàn. Không tự nhận giữ nguyên thời điểm tua sau load. Xem log để phân biệt UI/API/render/NOT_RUN; capture lỗi tọa độ/đen giữ FAIL riêng.

Hash đúng1.454 file bảo vệ liệt kê không đổi, gồm17 save gốc theo dõi; không suy toàn bộ dữ liệu. Namespace/save r104 riêng, không migration save cũ; không sửa controller/map/Bảo, không build/ZIP/runner mới. Không chạy lại recipe author/bind/isolate một lần lên bản cuối.

'''
p=R/'Docs/PROJECT_GUIDE_AND_HISTORY.md';p.write_text(intro+p.read_text(encoding='utf-8'),encoding='utf-8')
flow='''## 2026-11-05 — r104: nguồn art, clip và lớp quan sát

Hiện hành `JourneyPresentation_v48_r104.unity`; r101–r103/checkpoint không sửa. Xem bộ bàn giao `Test/KetQua/investigation_r104`. Script `Assets/Scripts/InvestigationV36R104`, namespace R104; art `Assets/Art/InvestigationV36/R104`,62 GUID cách ly theo isolation.json. Save riêng schema1/namespace conroi.investigation.v36.r104, thư mục InvestigationV36/r104; không nạp save cũ.

| Nguồn editable trong Source/Blender/JourneyV48/R104 | FBX dưới Art/InvestigationV36/R104 | Hierarchy/component |
|---|---|---|
| Khai_R104_Editable.blend | Khai.fbx | Workshop_GDD36/Khai_Investigation và Reconstruction_stage/Khai_Investigation; InvestigationActor nativeRoot/nativeIdle, PresentationHands operator |
| Ha_R104_Editable.blend | Ha.fbx | Ha_Investigation, giữ root/collider/raycast lời khai |
| Puppet_R104_Editable.blend | Puppet.fbx, PuppetPoseLibrary104.fbx | R03/R12/cipher/clip; InvestigationActor poseClips/repairClip giữ15 ID |
| Cuff_Comparison_R104_Editable.blend | CuffComparison.fbx | Evidence_coat MeshFilter/Renderer/collider bounds, mép bù so mảnh vải |
| Repair_R12_R104_Animation.blend, collections R104_R12_REPAIR_SUPPORT/R104_CURRENT_R12_CONTEXT | R12Support.fbx, animation event kế thừa cách ly | Repair_clip_illustration, clipRepair/repairEventActor, RepairHandBinding; support local(.35,3,1.1) |

Nguồn tải gốc và license CC0 đúng gói giữ trong Source/Downloads/Quaternius_R104; bảng ASSET_SOURCES ghi URL/Drive ID/hash, không dùng191 OBJ thiếu hồ sơ quyền sử dụng ở Assets/Model. Nguồn cuối lưu–mở lại, export/reimport roundtrip PASS theo cuff-source/art-final-export/puppet-finish/support-final; Khải trong cuff-source supersedes art-final-export cũ, art-source-export là ứng viên trung gian. Hình đã xem: Khai/Ha/Puppet-source, cuff-inspect-final, source clip final frames và UI.

Tái tạo: mở nguồn cuối, chọn đúng rig+mesh rồi export unity_skeletal (static cuff/support dùng unity), reimport đúng FBX/path, giữ rig Legacy/native sampling, vật liệu URP và root tương tác. Kiểm bounds/scale/pose/collider/reference và chân tiếp đất qua tuyến. Không chạy lại derive/bind/author/isolate/integrate một lần: có thể tạo trùng/reset nguồn. Không xóa art checkpoint. Operator nhập lại FBX từng có mesh layer0 trong khi camera chỉ29: mọi descendant operator đặt29, nativeRoot chính actor root chứa Khai_Rig, tay Fist.L/R cho IK; không chỉ sửa layer root.

UI tập trung là partial InvestigationReviewUI: ScaleToFit, clip/model textures1600×1000; camera tổng/cận cánh/cận tay trong stage, góc tay đặt phía trong tường. Thay góc/seek không đổi luật Advance. Mouse lifecycle giữ/thả và CancelReviewDrag cho pause/load; exposure chỉ tích lũy playback, không seek. Save/load reset review về trạng thái an toàn, không persist thời điểm animation. Luật hồi giữ nguyên r103 và khả năng chưa cài sau6s; không dùng script/animation minh họa làm chứng minh vật lý thực.

Map/nền chợ và các hạn chế M2/terrain legacy/mép nước không mở lại. Clip/Tư/hình nộm vẫn provisional, người mới NOT_RUN. Bằng chứng UI/controller/API tách trong README/video index; hash1454 file liệt kê gồm17 save gốc. Không build/ZIP/runner mới.

'''
p=R/'Docs/BLENDER_UNITY_WORKFLOW.md';p.write_text(flow+p.read_text(encoding='utf-8'),encoding='utf-8')
p=R/'BAT_DAU_TAI_DAY.md';t=p.read_text(encoding='utf-8');first=t.find('\n');start='''

## 05/11/2026 — prototype hiện hành r104

Mở **[JourneyPresentation_v48_r104.unity](Assets/Scenes/JourneyPresentation_v48_r104.unity)** bằng Unity **6000.6.2f1**, Play từ trước xưởng. WASD/chuột, E, R trả/đóng, P hồ sơ, Esc pause, F5/F6 hồ sơ riêng. Đi xưởng → quầy Hà → quay lại bàn dựng. GDD đúng: Docs/CON_ROI_GDD_v3.6_CHOT_TRIEN_KHAI.md.

Khải/Hà/rối Quaternius đã nhập thật; clip/bàn dựng có khung lớn, góc cận và slider kéo được. Clip/diễn xuất vẫn bản dựng thử im tiếng; người mới **NOT_RUN**. Save namespace conroi.investigation.v36.r104 riêng, không đọc save cũ. Giữ map r101 và checkpoint r101–r103; không build/ZIP/runner mới.

[Bàn giao r104](Test/KetQua/investigation_r104/README.md) · [Nguồn/giấy phép](Test/KetQua/investigation_r104/ASSET_SOURCES.md) · [Video](Test/KetQua/investigation_r104/VIDEO_INDEX.md) · [Hướng dẫn/phiếu playtest](Test/KetQua/investigation_r104/PLAYTEST_NGUOI_MOI.md). Các mục phía dưới là lịch sử, không dùng save/menu gameplay cũ để bắt đầu prototype mới.
''';p.write_text(t[:first]+start+'\n'+t[first+1:],encoding='utf-8')
print('Common docs updated; old history retained')
