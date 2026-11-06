from pathlib import Path
R=Path('D:/ConRoi')
blocks={
'BAT_DAU_TAI_DAY.md':'''## 06/11/2026 — hiện hành r106: chu kỳ cơ cấu đã kiểm, chờ người mới

Mở **[JourneyPresentation_v48_r106.unity](Assets/Scenes/JourneyPresentation_v48_r106.unity)** bằng Unity **6000.6.2f1**. Giữ map/art/checkpoint r101–r105. R106 sửa timeline dừng sớm 10 giây, giữ luật hồi; sáu ca chạy đủ tới đóng/cài hoặc trạng thái giữ có lý do. Có nút ngừng đè/nhả móc/rút chèn để xem phần hồi tiếp theo.

[Bàn giao](Test/KetQua/investigation_r106/README.md) · [Bảng sáu ca](Test/KetQua/investigation_r106/SIX_CASES.md) · [Video](Test/KetQua/investigation_r106/VIDEO_INDEX.md) · [Chuẩn bị profile và phiếu người mới](Test/KetQua/investigation_r106/PLAYTEST_NGUOI_MOI.md).

Khởi động phiên người thử bằng Editor đã đóng rồi mở lại với `--investigation-save-dir` tới thư mục mới chưa dùng; xem lệnh chính xác trong phiếu. Đã kiểm tham số khởi động thật và F5/F6 vào thư mục thử, không tự load. **Người thật NOT_RUN**. Bước kế tiếp là lấy phản hồi; không tiếp tục polish chung khi chưa có lỗi cụ thể. Các mục dưới là lịch sử.

''',
'Docs/PROJECT_GUIDE_AND_HISTORY.md':'''## 2026-11-06 — r106: trọn chu kỳ sáu diễn biến và profile phiên thử

Hiện hành `Assets/Scenes/JourneyPresentation_v48_r106.unity`, Unity **6000.6.2f1**, GDD v3.6 giữ nguyên. R105 baseline sạch xác nhận `AdvanceTrial`, slider và nút phát lại đều chặn ở 10 giây. Help/hold còn hồi đúng luật, không phải cơ cấu cần tăng tốc. Hook còn móc nên chưa cài; wedge và người quay lại giữ mở cũng là trạng thái giữ có chủ đích.

R106 cách ly 10 GUID/namespace script, thêm `TrialTimeline`: kết thúc theo trạng thái sau hành động cuối. Không snap đóng, không tự xóa lực/chèn/móc; ba nút bỏ giữ do người chơi yêu cầu ghi thời điểm vào timeline. Khi ngừng đè, hình nộm rời trong 0,6 giây với tiếp xúc giữ cánh tới khi rời, rồi hồi. ReturnGateMechanism/MechanismPresentation/PresentationHands giống r105 sau chuẩn hóa namespace; không sửa map/art/controller/collider/migration Bảo. Blender/FBX vẫn nguồn R105 và Quaternius R104, không nhân bản hoặc tái author.

Đã đo sáu ca và ba phần hồi sau bỏ giữ, có video UI thời gian thực và ảnh chốt/cánh cuối; bảng `Test/KetQua/investigation_r106/SIX_CASES.md` ghi đầy đủ mốc. Help cài ~12,52s; hold ~11,86s. Release còn người giữ ở 11,74s rồi sau chủ động ngừng đè cài ~19,77s. Hook vào vùng đóng ~12,42s nhưng giữ chốt nhả; nhả móc lúc14,65s mới cài. Wedge giữ35°, rút lúc3,91s rồi cài ~10,22s. After đóng/cài từ đầu, chờ người tới. Đó là thời gian mô hình, không thực nghiệm ngoài đời hoặc bằng chứng danh tính/ý định.

Hồi quy đúng phần thay: UI đổi ca đang chạy, kéo tua trước/sau thả và tái cài, đổi góc giữ time/state, pause/resume, API save/load reset an toàn và năm load lặp không trôi; seek không mở quan sát hoặc ghi thử nghiệm. Không đi lại Hà vì tuyến không đổi. Mở lại scene sạch: Missing Script0, broken reference0, binding clip R105 giữ nguyên. Video dùng input UI tổng hợp, animation minh họa im tiếng; capture FAIL giữ riêng, không gán PASS.

Đã đóng/mở Editor thực tế với `--investigation-save-dir` và kiểm Awake dùng đúng thư mục mới; F5/F6 bàn phím tổng hợp ghi/tải hồ sơ r106 trống. Schema1 namespace `conroi.investigation.v36.r106`, đường dẫn `<profile>/InvestigationV36/r106/prototype.json`; không tự load/migration save gameplay cũ. Hướng dẫn phải đóng Editor trước khi truyền tham số, chọn profile chưa dùng và xác nhận file F5; không dùng profile kỹ thuật đã kiểm làm hồ sơ người mới.

Bàn giao `Test/KetQua/investigation_r106/README.md`, checklist, video index và phiếu có bốn nhóm: không hiểu cơ cấu/không đọc được hình/không biết thao tác/hiểu nhưng thấy chưa đủ chứng cứ. **Playtest người thật NOT_RUN**. Dừng kỹ thuật để lấy phản hồi, không thêm art/khu/thư/ending. Hash bảo vệ chỉ theo `protected-final.json`, không suy toàn bộ dữ liệu; không build/ZIP/runner sản phẩm mới.

''',
'Docs/BLENDER_UNITY_WORKFLOW.md':'''## 2026-11-06 — r106: timeline hoàn tất theo trạng thái, không đổi nguồn art

Scene dẫn xuất `Assets/Scenes/JourneyPresentation_v48_r106.unity`; script `Assets/Scripts/InvestigationV36R106`, 10 GUID cách ly và thêm static `TrialTimeline`. Không chỉnh Blender/FBX/collider/material trong đợt này. Nguồn vẫn `Source/Blender/JourneyV48/R105/Repair_R12_R105_Animation.blend` → `Assets/Art/InvestigationV36/R105/RepairEventActor_R105.fbx` → `Repair_clip_illustration/EventActor_R105`; Quaternius R104 dùng chung đọc. Root event giữ tên R105 đúng asset, component session/binding dùng namespace R106.

`InvestigationSession` quản lý TrialDuration/TrialClearAt và dừng theo kết quả ổn định; `InvestigationReviewUI` dùng duration thực cho tua/phát lại, nút bỏ điều kiện giữ; `TrialTimeline` lấy cùng sáu lịch gốc từ MechanismPresentation và gọi ReturnGateMechanism.Advance. Người chơi chủ động bỏ lực/móc/chèn mới thêm đầu vào; không tự đóng khi hết đoạn. Chuyển ca hoặc load reset lịch bỏ giữ; seek tái tính đầy đủ, không phụ thuộc cache frame trước. Ngừng đè minh họa rời cánh0,6s và giữ tiếp xúc tới khi rời; không đổi luật hồi.

Làm lại bằng mở scene/source đã lưu, chỉnh đúng các script r106 và chạy kiểm chu kỳ/tua/lifecycle tác động. **Không chạy lại author/derive/bind/isolate một lần** và không tái xuất art chỉ để đổi revision. Bảng đo và video trong `Test/KetQua/investigation_r106`/`Test/BangChung/investigation_r106`; tách UI tổng hợp, API đo trạng thái và animation minh họa. R105 và nguồn/checkpoint giữ hash theo danh sách bảo vệ; xem manifest thực tế.

Scene đã mở lại, compile/reference và binding kiểm; người thật **NOT_RUN**. Hướng dẫn profile xác nhận khởi động Editor với `--investigation-save-dir` thật, F5/F6 đúng thư mục thử; không đổi save gốc. Dừng polish chung để lấy phản hồi sau bàn giao.

'''}
for name,block in blocks.items():
 p=R/name;s=p.read_text(encoding='utf-8-sig')
 if s.startswith(block):continue
 assert not s.startswith(block.split('\n')[0]),'Review partial existing r106 documentation first'
 p.write_text(block+s,encoding='utf-8')
print('r106 current records added; old sections retained, UTF-8.')
