from pathlib import Path
updates={
'BAT_DAU_TAI_DAY.md':'''## 06/11/2026 — hiện hành r107: sửa nền và mở đầu từ phản hồi thực

Mở **[JourneyPresentation_v48_r107.unity](Assets/Scenes/JourneyPresentation_v48_r107.unity)** bằng Unity **6000.6.2f1**. Giữ r106 làm checkpoint. [Bàn giao và giới hạn](Test/KetQua/investigation_r107/README.md) · [Người chơi](Test/KetQua/investigation_r107/HUONG_DAN_NGUOI_CHOI.md) · [Người tổ chức/profile sạch](Test/KetQua/investigation_r107/PLAYTEST_NGUOI_MOI.md).

Người dùng đã báo nền hở và UI quá nhiều công cụ. R107 sửa winding trong bốn mesh riêng từ Blender→FBX→Unity, không đổi cao độ/nước/nền chợ. Mở đầu chỉ một việc: vào xưởng, lên gác, kiểm dấu sàn; E khám, xoay/phóng, ghi, R trở lại, P xem lại. Công cụ hiện theo ngữ cảnh; save riêng `InvestigationV36/r107`, không migration save gốc. Đã có video input tổng hợp và đường controller qua vùng sửa. Chưa coi toàn map sạch topology hoặc người mới đã hiểu: phiên r107 **NOT_RUN**, báo cáo ghi các kiểm còn thiếu. Không chạy lại author/derive/bind, không build/ZIP/runner. Các mục dưới là lịch sử.

''',
'Docs/PROJECT_GUIDE_AND_HISTORY.md':'''## 2026-11-06 — r107: sửa theo ảnh nền hở và onboarding

Checkpoint r106 giữ nguyên; scene `Assets/Scenes/JourneyPresentation_v48_r107.unity`, Unity6000.6.2f1. Phản hồi trực tiếp r106 đã nhận: nền mất rộng trong ảnh105627 và quá nhiều công cụ UI. Không tiếp tục ghi “chưa có phản hồi”; vẫn phân biệt phiên người mới r107 có tổ chức **NOT_RUN**. Chi tiết tại `Test/KetQua/investigation_r107/README.md`, `FEEDBACK.md`, `CHECKLIST.md`, `VIDEO_INDEX.md`.

Vị trí ảnh nền rộng eye681.10/3.64/413.07, yaw230. Triangle test xác nhận mặt quay xuống/culling, collider còn đỡ; không gán M2/legacy hoặc đổ lỗi runtime khi chưa có bằng chứng. Surface176/104, Context124 và Surface142 chuyển sang mesh riêng sửa winding; vertex/UV/cao độ giữ nguyên. Ba góc ảnh đầu chưa khớp pose chính xác; không gọi các ảnh chẩn đoán bổ sung là đủ bốn góc trước/sau. Toàn map xa/topology kế thừa chưa được chứng nhận; generic source gate FAIL còn nguyên.

Onboarding bắt đầu trước cửa, intro ngắn, một mục tiêu dấu sàn, marker đường tới gác rồi biến mất. E khám, xoay/phóng, ghi quan sát trực tiếp, R, P xem lại rồi mục tiêu điện thoại; skip/persistence riêng. HUD ngắn, thông báo6s; công cụ theo ngữ cảnh, save/load trong hồ sơ/pause. Luật/animation/Quaternius/controller/InputActions giữ nguyên. Script r107 remap11GUID, namespace/save riêng, không migration. Không sửa hoặc khôi phục save người dùng.

Video mở đầu và đường map dùng input tổng hợp; không API mở trang/ghi lời giải để giả chứng minh người chơi tự tìm. Đường map76điểm113,49s,0frame mất tiếp đất; pause/F5/F6 kiểm riêng. Không lặp sáu ca/Hà khi không chịu ảnh hưởng. Tra checklist cho phạm vi chưa chạy; không tự chấm người mới PASS.

''',
'Docs/BLENDER_UNITY_WORKFLOW.md':'''## 2026-11-06 — r107: winding terrain, không san lại map

Nguồn `Source/Blender/JourneyV48/R107/Terrain_R107_Editable.blend`, collection `TERRAIN_R107_WINDING_REPAIR`, Blender5.2.0LTS; derived editable contextR101. Cửa sổ X560..720/Z320..460, top centroidY>−0,05. Chỉ đảo hướng face quay xuống, không dời vertex/nước/nền chợ. `TerrainR107_Final.fbx` → `Assets/Art/Environment/JourneyV48/TerrainR107/`; bốn reference scene riêng tại Surface176/104/142 và Context124. Context109 giữ nguyên. Xem `Test/KetQua/investigation_r107/MAP_REPAIR.md` và `ASSET_SOURCES.md`.

FBX reimport5object sai số<0,001m. Tích hợp dùng hướng face xuất từ FBX trên topology bound cũ (nearest centroid<0,02m), vertexDelta0/UVequal, unmatched0; không đổi topology ngoài đảo thứ tự. Source mapping và số face ở `source-export-final.json`, `map-integration-final.txt`. **Không chạy lại các script source-extend/integrate/derive một lần**; tiếp tục từ nguồn/scene đã lưu, chỉnh đúng mục tiêu. Không dùng Two Sided/tấm che/fog để giấu lỗi.

Gate prop tổng quát `source-delivery-gate.json` vẫn FAIL do topology terrain kế thừa; audit diện tích thực riêng và ảnh/controller ghi phạm vi cụ thể, không chuyển tất cả sang PASS. Bound Unity còn vài tam giác cực mảnh/normal nhạy float, xem `residual-faces.txt`. Không weld/san cả map để đổi kết quả gate. Quaternius và nguồn clipR105/luậtR106 giữ nguyên, không tải art hoặc xuất clip mới. Onboarding chỉ thêm script/UI/marker code-native và spawn trong r107. Bằng chứng `Test/BangChung/investigation_r107`, kết quả `Test/KetQua/investigation_r107`; giữ nguồn/checkpoint/save theo1733entry bảo vệ.

'''
}
for name,prefix in updates.items():
 p=Path(name);old=p.read_text(encoding='utf-8-sig')
 if not old.startswith(prefix):p.write_text(prefix+old,encoding='utf-8')
print('Updated start and two common documents as UTF-8; historical sections retained.')
