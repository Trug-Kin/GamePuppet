from pathlib import Path
R=Path('D:/ConRoi')
history='''## 2026-11-05 — r103: độ rõ vật chứng, clip và bàn dựng điều tra

Scene hiện hành `Assets/Scenes/JourneyPresentation_v48_r103.unity`, Unity6000.6.2f1; dẫn xuất cách ly từ r102, giữ map r101/checkpoint. GDD duy nhất vẫn `Docs/CON_ROI_GDD_v3.6_CHOT_TRIEN_KHAI.md`. Script namespace `ConRoi.Investigation.V36.R103`, art/source R103, 49 GUID cách ly; r102 không đổi hành vi qua reference chung. Save riêng `conroi.investigation.v36.r103`, không migration save cũ.

Đã thay dấu sàn đơn giản bằng vân/gót khuyết, hai vệt bụi và phần giao thật; thêm giày B cùng màu khác khuyết/vân, mép vải/chỉ/may nhìn được và nhãn R03/R12. Lỗi vân bị đẩy vào trong đế do thao tác offset xuất lặp được sửa tọa độ tuyệt đối trong Blender; không vá bằng mô tả. Chốt nhập sai vị trí do helper bỏ transform FBX đã gắn về(1.51,.92,0); móc/collider cập nhật đúng bounds. Preview dây world-space chuyển sang local trước scale để không kéo đường xem ra tọa độ cũ.

Nguồn clip mới editable12s, animation liên tục: đóng/cài trước t4, nhả t4, mất tựa t4.8, hai tay có đồ sửa, camera không thấy ĐK/người kéo. Vẫn **bản dựng thử im tiếng**, chưa diễn xuất/art final hoặc mô phỏng vật lý độc lập; dây đỡ/động tác sửa còn cần review. Sáu nút dùng chung Advance qua MechanismPresentation, đầu vào timeline khác; tay IK, móc/vật chèn có hình; thêm góc toàn xưởng/cận cánh. Giữ spring2.5/damping4 và cửa sổ chưa cài sau6s; không loại giả thuyết mở trước bằng tăng tốc hồi. Không suy giờ kéo/ý định từ vải/giày.

UI chơi dùng lời thường, technical chuyển Inspector diagnostics. Quan sát giày/vải/áo tách khỏi suy luận tự viết. Mật mã ×, trường gõ không kích R/P; tua ±1s, play/pause/replay; kết luận danh sách căn cứ co theo nội dung, nút xác nhận lớn và lưu lựa chọn chưa đủ. Khải/Hà/rối hiện vẫn self-authored provisional. Quaternius CC0 chỉ đánh giá trang/ảnh chính thức, chưa tích hợp model/kiểm rig; bảng giấy phép ghi rõ, không đổi pipeline.

Kiểm controller tuyến xưởng→Hà→xưởng21/21 từ spawn sạch bằng input tổng hợp, không teleport; kiểm UI nhập sai/xóa/sửa, liên hệ tự viết, kết luận chưa đủ và reload; API lifecycle tách riêng. Hash đúng1.325 file bảo vệ liệt kê, gồm17 save gốc được theo dõi; không suy toàn bộ dữ liệu. Video/ảnh có phạm vi và lần INVALID riêng; không dùng compile/MissingScript hay số ca để chứng minh người mới hiểu. **Playtest người mới NOT_RUN.** Bàn giao/checklist/giới hạn và nguồn: `Test/KetQua/investigation_r103/README.md`, `ASSET_SOURCES.md`, `PLAYTEST_NGUOI_MOI.md`; bằng chứng `Test/BangChung/investigation_r103`. Không build/ZIP/runner mới, không mở cốt truyện/địa điểm hoặc map.

'''
workflow='''## 2026-11-05 — r103: nguồn và ánh xạ điều tra cách ly

Mở scene `Assets/Scenes/JourneyPresentation_v48_r103.unity`; giữ r101/r102. Không chọn bản theo số lớn nhất ngoài hồ sơ. Nguồn `Source/Blender/JourneyV48/R103/Workshop_Investigation_R103_Editable.blend`, `Puppet_Poses_R103_Editable.blend`, `Repair_R12_R103_Animation.blend`; các file đã save/reopen. Bộ sưu tập chính WORKSHOP/MECHANISM/EVIDENCE/CHARACTERS/PUPPETS/REVIEW. Unit1, Unity=(-Bx,Bz,-By), FBX Unity -Z forward/Y up; roundtrip EVIDENCE, MECHANISM và actor animation lưu trong hồ sơ r103. Vật liệu URP màu phẳng; không tuyên bố toàn asset đã texture/lightmap-ready.

| Nguồn → asset R103 | Hierarchy / component |
|---|---|
| EVIDENCE: SlideMarks/ShoePrint/TraceCrossing | `Investigation_V36_R103/Workshop_GDD36/Evidence_slide` và `Evidence_shoeprint`; cùng `TraceCrossing.prefab` cho inspection; hai mặt bụi/vân có vị trí lớp thật |
| ShoeSample/ShoeControl/CoatSample/TornFabric → EVIDENCE.fbx | Evidence_shoe/shoecontrol/coat/fabric, `InvestigationInteractable` ID ổn định;18 interaction bindings. Samples Khải giữ điều kiện xin mẫu. Nhãn Label_R03/R12 và RepairCard gắn vào rối/thẻ |
| MECHANISM: L/C/D/Handle_DK/Hook_M/ControlPost_DK/Wedge → MECHANISM.fbx | Cánh thật, clip và bàn dựng; `ReturnGateMechanism` r103 giữ law r102. Chốt local(1.51,.92,0), LineRenderer trạng thái thay rope render tĩnh; collider móc tính lại bounds |
| Repair_R12_R103_Animation.blend → RepairEventActor.fbx | `Repair_clip_illustration/EventActor_R103`, Legacy animation `Scene`12s, `InvestigationSession.repairEventMotion` SampleAnimation; RepairHandBinding nối tay/dây R12. Camera layer30 không thấy ĐK; stage layer29 |
| CHARACTERS/PUPPETS và Pose source sao cách ly → FBX/pose icons | Khải/Hà/R03/R12 và15 pose; art còn tạm, không có bộ Quaternius đã tích hợp |
| `Assets/Scripts/InvestigationV36R103` | namespace R103, session/UI/evidence riêng; `MechanismPresentation.At` dùng Advance chung cho clip/sáu case, `PresentationHands` giữ tay tới điểm lực, `RepairHandBinding` không tranh pose với Animator |

Làm lại từ **ba nguồn hiện hành** và scene r103 đã gắn. Xuất đúng collection bằng bx_export unity; skeletal actor dùng unity_skeletal/bake active action, không all-actions; reimport kiểm scale/bones/action. Không chạy lại isolate hoặc integrate tạo trùng/reset binding. Không chạy script author đầu đè nguồn hoàn chỉnh; bước offset tương đối trong export lịch sử từng đẩy vân vào đế. Bản cuối `investigation103-sole-finish.py` dùng tọa độ tuyệt đối (đáy vân−.001, đỉnh+.0002), nhưng nguồn đã đúng thì chỉ export, không cần sửa lại. Các helper trong Test/Tam là recipe/kiểm Editor hữu hạn, không runner sản phẩm.

Preview LineRenderer phải đổi world→local trước đưa clone về vùng review và scale; tránh bounds dây giữ vị trí thật làm vật xem bị nhỏ/dài sai. Tính collider sau thay mesh; raycast giữ vật cản, không mở rộng hitbox xuyên đạo cụ để né lỗi. Nguồn/chốt nhãn phải đối chiếu Unity, vì gate Blender không phát hiện helper bỏ placement hoặc nhãn bị lật lúc bind.

Đối chiếu `Test/KetQua/investigation_r103/README.md`, các report export/roundtrip, ui/presentation/lifecycle và final scene audit; ảnh/video trong `Test/BangChung/investigation_r103`. Save riêng R103/schema1, không đọc/migration save gameplay cũ. 1.325 protected hashes gồm17 save gốc đối chiếu đúng danh sách; không nói mọi save. Kiểm vật lý thực/hiểu bài người mới và art final còn mở, newcomer NOT_RUN. Nền chợ/map đóng băng, không khôi phục dốc và không build/ZIP/runner mới.

'''
for path,section in [('Docs/PROJECT_GUIDE_AND_HISTORY.md',history),('Docs/BLENDER_UNITY_WORKFLOW.md',workflow)]:
 p=R/path;s=p.read_text(encoding='utf-8-sig');assert '## 2026-11-05 — r103:' not in s;p.write_text(section+s,encoding='utf-8')
p=R/'BAT_DAU_TAI_DAY.md';s=p.read_text(encoding='utf-8-sig')
start=s.index('## 05/11/2026 — bắt đầu prototype điều tra r102');end=s.index('**Project: `D:\\ConRoi`. Scene phát triển map hiện hành:')
intro='''## 05/11/2026 — prototype điều tra hiện hành r103

**Mở [JourneyPresentation_v48_r103.unity](Assets/Scenes/JourneyPresentation_v48_r103.unity) bằng Unity6000.6.2f1, nhấn Play.** Root `Investigation_V36_R103`, player `Khang_FirstPerson_R103`, bắt đầu trước xưởng. Đi khám xưởng → quầy Hà ngoài cửa → quay lại bàn dựng. WASD/chuột, Space, E, R trả/đóng, P hồ sơ, Esc pause, F5/F6 save riêng. Xin mẫu Khải qua mục liên hệ Phương sau khi gặp Khải. Đoạn quay R12 có ngay khi khám điện thoại.

Save riêng namespace `conroi.investigation.v36.r103`, schema1, `InvestigationV36/r103` dưới persistentDataPath; không nạp/migration save cũ. **R101 là map đóng băng; r102 là checkpoint prototype**, không ghi đè. Không build/ZIP/runner mới. GDD đúng: `Docs/CON_ROI_GDD_v3.6_CHOT_TRIEN_KHAI.md`.

[Bàn giao r103 và giới hạn](Test/KetQua/investigation_r103/README.md) · [Nguồn/giấy phép](Test/KetQua/investigation_r103/ASSET_SOURCES.md) · [Playtest người mới — NOT_RUN](Test/KetQua/investigation_r103/PLAYTEST_NGUOI_MOI.md). Vật chứng có geometry để so; clip12s và art nhân vật vẫn bản dựng thử, chưa final. Không suy giờ kéo/ý định từ vải/giày. Hướng dẫn bên dưới là **lịch sử map/gameplay cũ**, không áp dụng profile/menu đó cho r103. Lịch sử r102 giữ trong hai tài liệu chung.

'''
p.write_text(s[:start]+intro+s[end:],encoding='utf-8')
print('Updated shared history/workflow and removed duplicate r102 start paragraphs; history retained in Docs.')
