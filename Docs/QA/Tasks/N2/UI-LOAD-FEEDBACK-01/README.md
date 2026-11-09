# UI-LOAD-FEEDBACK-01 — Gameplay & Dữ liệu

Nhận yêu cầu09/11/2026, từ597369a5 trên work/runtime-contracts-baseline; tracked sạch, hai nhóm untracked cũ giữ nguyên. Đã đọc phiếu và báo cáo UI-NAV-REVIEW-02 trực tiếp từ checkout review local tại ef8911965, không merge nhánh review. [Hash tài liệu nhận](request-source.json).

## Acceptance trước sửa

- Sửa phần hiển thị trong session partial được giao: mọi modal có vùng Message đọc được, wrap/cuộn nếu dài, không che controls; world giữ vị trí hiện tại. Diagnostics=false vẫn hiện lý do từ Load/F6/nút Tải.
- Giữ bool Load, validator và guard nguyên vẹn; không validate tại UI, không schema/ID/GUID/state save mới. Lỗi validation giữ lý do guard; lỗi đọc/parse dùng lời nhắn an toàn, không stack trace/đường dẫn hoặc nội dung exception thừa.
- Giữ6giây unscaledTime hiện có theo yêu cầu UI; Message thành công thay lỗi ngay sau Load hợp lệ, không giữ riêng lỗi cũ. Không thay focus/nav/P-close, trang, input lock hoặc tiến trình chỉ vì vẽ thông báo.
- API có phạm vi: save hợp lệ, validation lỗi, JSON hỏng, lỗi đọc mô phỏng bằng FileStream exclusive trên file thử; failure không thay state/bytes; khôi phục trong finally rồi Load/Save/Load. Không chạy lại bộ API/IO cũ.
- Scene Gameplay/profile mới: tiến trình bằng controller, F5/F6, lỗi validation và read qua F6/nút UI; nhìn lý do tại dossier/pause ở1080p, không báo thành công sai. Sau thành công không còn lỗi cũ; P/E/Esc/pause/movement/modal hoạt động. Tách input Windows tổng hợp khỏi API fixture, scene chung/người mới chưa kiểm bởi gói này.

## Phạm vi sửa và phối hợp

InvestigationSessionUI.cs thuộc toàn bộ partial Gameplay & Dữ liệu: thêm footer modal riêng có scroll, chừa vùng nội dung ổn định; clip/bàn dựng dùng khoảng trống header để giữ nguyên vùng điều khiển vẽ tọa độ cố định; world không đổi. InvestigationSession.cs cần đổi riêng catch trình bày lỗi đọc/parse để tránh e.Message chứa đường dẫn; không đổi điều kiện guard hoặc gán state. Đây là mở rộng file tối thiểu để đáp ứng yêu cầu không lộ thông tin; phía tích hợp review cả hai. Lifecycle6giây giữ như đã cho phép; không thêm lịch sử lỗi/popup hoặc hệ thống thông báo mới.

Kết quả/API/controller, commit nguồn và công cụ ghi bên dưới. Không sửa UI ngoài ownership/map/scene chung/save gốc; không push/merge/chốt baseline. P-close lịch sử, probe cũ và người mới giữ riêng.

## Nguồn và kết quả thực kiểm

- Tiền đề `597369a566606fb24a549472acfa44291e3da5c3` chứa toàn bộ guard save đã bàn giao.
- **Nguồn sửa riêng: `25f14632b64f140bde9a06ecb0c32dbdda33deb2`.** Hai file Session.cs và SessionUI.cs; không sửa validator, Save writer, schema/ID/GUID, focus/NAV-01 hoặc scene.
- **Công cụ/meta và commit thực chạy cả API/GUI: `5be056e4883a7c3cc49ba52e370d75ace2bc1510`.** Tool LoadFeedbackChecks không gắn vào scene; API chỉ chạy qua RunBatch tường minh. StartGui dùng môi trường controller hiện có của Gameplay và đặt render1080p, không cấp chứng cứ.
- Commit tài liệu sau phiên chỉ ghi nhận kết quả, không phải nguồn được chạy lại. [Hash nguồn thực kiểm](tested-source.json) khớp Git blob theo nội dung chuẩn hóa; không thay hash baseline cũ.

### Hành vi sửa

`ModalMessage()` đọc **Message hiện có** ở mọi modal khi diagnostics=false. Hồ sơ/pause/inspect/cipher/compare/conclusions dùng vùng footer y724..794 (hệ tọa độ1200×800), content/sidebar chừa vùng ổn định; thông báo wrap và có scroll nếu vượt viewport. Clip/bàn dựng giữ nguyên vùng điều khiển cố định, dùng khoảng trống header x410..835 để không cắt label cuối trang. World giữ renderer/vị trí thông báo đang có. Chỉ scroll presentation là private runtime, không thêm serialized/save field.

Giữ **6giây theo unscaledTime** như phía tích hợp cho phép. Không lưu một bản lỗi riêng; khi Load thành công đặt Message mới, footer vẽ ngay kết quả mới và reset scroll, lỗi cũ không còn. Hết hạn thì không vẽ nội dung cũ. Không tự đổi page/input hoặc state vì vẽ feedback.

Catch Load giữ nguyên thông báo validation từ guard. JSON parse dùng “Nội dung hồ sơ không đúng định dạng.”; lỗi đọc dùng “Không đọc được tệp hồ sơ. Hãy đóng ứng dụng đang dùng tệp rồi thử lại.”; không quyền đọc có hướng dẫn kiểm quyền; lỗi khác dùng thông báo chung. Không đưa e.Message của IO/parse/exception bất ngờ vào UI, không stack trace/đường dẫn. Bool/các điều kiện validation/gán incoming không đổi, không nhân đôi kiểm tra ở view. Lỗi không có file vẫn giữ lời nhắn hiện hành.

### Kiểm API — PASS66/66

[Kết quả từng assertion](api-results.json). Profile mới, fixture trong scene Gameplay_ThuNghiem. Các ca riêng world/dossier/pause: evidence:null, JSON cắt dở, FileShare.None mô phỏng lỗi đọc; mỗi ca kiểm Load=false, full state/identity/modal/transform/gate/input flags/timeScale không đổi, bytes không đổi, lý do an toàn; nhả lỗi rồi Load hợp lệ thay lỗi cũ, tiếp tục Observe/Save/Load. New Game[] hợp lệ trước/sau vẫn tải được. Các thao tác Acquire/Observe và OpenPage của phần này là **API fixture**, không tuyến chơi tự nhiên; không gộp vào bằng chứng UI.

Không chạy lại42/54/78 hoặc bộ IO/trial cũ; guard validator không đổi.66 assertions này là bộ mới có phạm vi cho feedback/error-result và các hành vi Load chịu ảnh hưởng.

### Kiểm controller/UI — đạt các ca mục tiêu

[Kết quả và giới hạn controller](controller-results.json). Unity6000.6.2f1, đúng commit5be056e4, scene **Gameplay_ThuNghiem**; môi trường proxy floor/slide/shoeprint/gate tạm trong Play, không lưu scene. Các phím/chuột là **input Windows tổng hợp** kiểm foreground PID18928 trước thao tác. Không Acquire/Observe/API Load/Save/OpenPage hoặc teleport để chứng minh UI.

| Ca | Kết quả |
| --- | --- |
| Tạo tiến trình | Click Start → aim/E khám proxy slide: observations=[] → nút Ghi quan sát:[0,1] → R/P → F5. Không fixture cấp chứng cứ trong lượt UI. |
| Hồ sơ/F6 và nút Tải lỗi validation | Footer đọc được ngay ở1920×1080, không diagnostics, không báo đã tải; state/bytes giữ nguyên. Ba chu kỳ P mở/đóng sau từ chối đạt. |
| Pause/F6 validation lỗi | Lý do đọc được ở footer, nút Trở lại/Lưu/Tải và nội dung không bị che; state/bytes giữ nguyên. |
| Pause/F6 và nút Tải lỗi đọc | Dùng CreateFileW giữ handle đọc độc quyền **chỉ file thử**, nhả bằng CloseHandle trong finally. Thật sự gây Windows sharing violation, lý do tiếng Việt xuất hiện, không có đường dẫn/Exception. Không đổi ACL/làm đầy ổ/giả crash. State/bytes giữ nguyên. |
| Load hợp lệ sau nhả lỗi | F6 khôi phục dữ liệu đúng lifecycle cũ và footer hiện “Đã tải…” ngay, không giữ lỗi cũ; ảnh riêng kiểm trực tiếp. |
| Controller sau lỗi | P, di chuyển W/S, aim/E khám thật proxy; W/E trong inspect không đổi vị trí/thoát modal; Esc đóng, Esc pause/W không di chuyển/Esc resume, P mở/đóng đạt. |
| Tiếp tục F5/F6 | Lưu tiến trình UI, A đổi vị trí, F6 khôi phục data và vị trí (sai số<0,002). World vẫn dùng vùng thông báo cũ. |

**Lượt chưa hoàn tất giữ riêng:** lần click Ghi đầu không ghi vì Editor zoom1,3× làm tọa độ helper lệch, snapshot/log giữ nguyên; đã chỉnh riêng zoom Game view về fit0,86× rồi chạy lại từ state chưa ghi. Không sửa runtime để bù, không gọi lượt chưa chạy đủ là PASS/FAIL gameplay. Trong phiên này không có foreground-loss exception; các lượt mất focus/probe bổ sung trước đây vẫn theo báo cáo cũ, không đóng hồi tố.

### Giới hạn và nhận tích hợp

- PASS trong **scene Gameplay proxy và API**, không phải scene chung hoặc người mới. Đã xem ảnh1920×1080 cho world/inspect/dossier/pause và thao tác mục tiêu. **Scene chung, mọi trang UI1080p, cipher/compare/conclusion/clip/bàn dựng và scroll thông báo dài qua controller chưa kiểm với bản vá**; đặc biệt header review phải được phía tích hợp review. Không suy mọi màn PASS từ ảnh mục tiêu.
- P-close lịch sử vẫn **OPEN**, chưa tái hiện hoặc sửa; không đổi NAV-01. Người mới và probe bổ sung trước vẫn NOT_RUN. Chưa chốt baseline nhóm hoặc sửa báo cáo N3 cũ thành PASS.
- UI/UX & Tích hợp nhận cả `25f14632 → 5be056e4 → commit báo cáo` trên tiền đề597369a5, review hai partial cùng một chủ sửa; **không cần wiring mới**, không merge cả nhánh review vào Gameplay. Trong scene chung, kiểm F6/nút Tải sai từ dossier/pause có lý do và không báo thành công, full state/bytes giữ, sau hợp lệ lỗi cũ mất; P/E/Esc/pause và layout1080p/footer/header các trang bị co nội dung. Không tự đóng Load feedback tổng thể trước khi kiểm scene chung đạt.
- Muốn lặp API: Unity batchmode/nographics, `-executeMethod ConRoi.Gameplay.EditorChecks.LoadFeedbackChecks.RunBatch --investigation-save-dir <profile-moi-tuyet-doi> --load-feedback-output <output-tuyet-doi>`; không reuse profile.
- Muốn lặp GUI: Unity GUI, `-executeMethod ConRoi.Gameplay.EditorChecks.LoadFeedbackChecks.StartGui --investigation-save-dir <profile-moi-tuyet-doi> --save-controller-output <output-tuyet-doi>`; Game view render1920×1080, xác nhận zoom fit trước tọa độ click. Đây là tool khởi tạo sandbox/quan sát, không tự nhập toàn tuyến; input/snapshot/stop/quit dùng quy trình controller hiện có.

### Bảo toàn

Đã snapshot state/diff/settings trước thao tác, Play dừng ở sceneDirty=false và Editor đóng bình thường không lưu scene. [Audit bảo toàn](preservation.json): settings phát sinh được lưu bytes/hash After, chỉ phục hồi exact Before sau khi so normalized bằng nhau. Product working-tree diff so commit kiểm rỗng. Log không có error CS/NullReferenceException/MissingReferenceException hoặc GUI/GUILayout error; còn warning môi trường Unity/Burst/licensing/cloud.

Raw log/capture/save/profile và snapshot nằm ở Test/Output/LoadFeedback20261109 cùng Test/PlaytestProfiles/LoadFeedbackAPI20261109 và LoadFeedbackUI20261109, ngoài Git. [Index SHA256](evidence-index.json) để yêu cầu đúng bằng chứng; không phải dependency runtime. C còn13.598.076.928byte trước kiểm, dùng Library Gameplay riêng hiện có. Project gốc, save gốc, legacy/untracked và dữ liệu archive không sửa. Chưa push/merge/tạo build/ZIP.
