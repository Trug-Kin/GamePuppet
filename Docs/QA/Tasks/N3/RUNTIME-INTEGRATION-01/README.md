# RUNTIME-INTEGRATION-01 — review và kiểm scene tích hợp

UI/UX & Tích hợp, 09/11/2026. **Đã nhận đủ năm gói đến `91835c80`; tuyến chính được kiểm kỹ thuật trong scene chung. Tổng thể REVIEW_REQUIRED vì phát hiện Load null-list làm thay state; chưa chốt baseline nhóm mới.** Không sửa session partial, UI sản phẩm, map hoặc scene.

## 1. Nguồn thực sự đã kiểm và bảo toàn

- Baseline chung: `0d878e3b7163c4d50e3976cd5c46118403b19240`.
- Runtime nhận: `91835c80e040d4331b967b6a676016bcd769c2cb`. Review **toàn bộ 22 commit / 59 file**, gồm đề xuất, guard nguồn, công cụ và báo cáo của cả năm gói: [chuỗi commit](review-commits.txt), [file thay đổi](review-files.txt).
- Nhận trong nhánh local `review/runtime-integration-20261109` tại `C:/ConRoi_CheckoutTest`: giữ commit UI/UX `ebd92771`, merge trọn runtime bằng `d765ed923bc7984c5c29667275b81d1db2a4e216`. Không cherry-pick riêng báo cáo, không merge main/chore hoặc đổi nhánh thành viên; chưa push.
- **HEAD đã chạy Unity: `c18524a3bc07f74fab16244f2d55c6888afb9284`**. Hai commit sau merge chỉ thêm/sửa công cụ Editor kiểm riêng, không đổi runtime. [Hash nguồn](tested-source.json), [kết quả đo](integration-results.json). Commit tài liệu sau kiểm không phải commit đã chạy lại Unity.
- Scene thật: `Assets/ConRoi/Modules/TichHop/Scenes/ConRoi_Chinh.unity`, Unity **6000.6.2f1**. Dùng Library độc lập hiện có tại C, import tăng dần, không copy Library hoặc file untracked bù cho Git. Đây không phải kiểm clone mạng mới.
- `D:/ConRoi` vẫn ở `work/ui-onboarding-review/ebd92771`, có deletion hướng dẫn nhóm và legacy/untracked của người dùng: giữ nguyên, không switch/reset/restore/stage. C trước nhận detached `074caf3`, có ba settings khác newline và output cũ; đã giữ status/diff/HEAD/bytes trước thao tác. C trước import còn **13.032.443.904 byte**, không tạo Library mới hoặc build/ZIP.
- Snapshot trước/sau, save thử, ảnh/log/input nằm ngoài Git tại `C:/ConRoi_CheckoutTest/Test/Output/RuntimeIntegration20261109`. Profile riêng mới: `Test/PlaytestProfiles/RuntimeIntegration20261109`. Giữ save gốc, checkpoint, GUID, 4.183 legacy chưa xác minh và archive; không dọn.

## 2. Kết luận review toàn chuỗi

| Gói | Phạm vi review / nhận thay đổi |
|---|---|
| [SAVE-VALIDATION-01](../../N2/SAVE-VALIDATION-01/README.md) | Validator chỉ đọc trước `data=incoming`: cấu trúc, finite fields, ID, trùng vật chứng, observation index. Không phải rollback tổng quát cho exception binding sau validation. API null-object không đồng nghĩa mọi JSON null qua parser đều bị chặn; xem phát hiện mới bên dưới. |
| [SAVE-REFERENCE-02](../../N2/SAVE-REFERENCE-02/README.md) | Hai endpoint relation phải có definition registry; null record/endpoint bị chặn. Không thêm luật suy luận, membership hoặc tự chấm quan hệ. Data public vẫn đòi producer giữ hợp đồng. |
| [SAVE-WRITE-03](../../N2/SAVE-WRITE-03/README.md) | Ghi/flush/đóng temp rồi Move lần đầu hoặc Replace khi ghi đè. Không delete/copy save tốt; Load chỉ chọn đích, không tự tải temp. Seam private chỉ phục vụ API test; không suy bảo đảm mất điện/crash. |
| [TRIAL-CONTEXT-01](../../N2/TRIAL-CONTEXT-01/README.md) | Update timeline chỉ trên Page reconstruction, giữ pause tay/seek/luật; Load reset timeline transient như trước. Không thêm hệ đã xem hoặc khóa kết luận. |
| [OBS-PROGRESS-01](../../N2/OBS-PROGRESS-01/README.md) | Observe hợp lệ cập nhật bước dấu sàn; wrapper không mở bước khi chưa có vật. Dùng API/state và presentation cũ; schema/ID giữ nguyên. |

**Không cần wiring mới:** scene giữ binding hợp lệ, caller E/P/F5/F6 hiện có đi qua nguồn mới. Không sửa prefab/component/GUID hoặc contracts. Các kết quả 42/54/43/103/35 API và UI tại sandbox trong báo cáo bên giao được giữ nguyên như bằng chứng lịch sử; không chạy lại hoặc đổi chúng thành PASS scene tích hợp.

## 3. Phương pháp và acceptance

Công cụ `ConRoi.Integration.EditorChecks.IntegratedRuntimeReviewChecks.StartGui` mở scene thật, audit reference, lấy state/raycast/snapshot và quản lý Play/quit; không dựng proxy, cấp evidence, teleport, gọi Save/Load/OpenPage hoặc đổi state gameplay. Việc đi lại, E, ghi, R, P, clip, bàn dựng, F5/F6 đều qua **phím/chuột Windows tổng hợp**, kiểm đúng PID/foreground. Không có người mới tham gia.

Tiến trình đầu được tạo từ profile trống: Bắt đầu → đi cầu thang → E dấu sàn → ghi → R/P → điện thoại → xem clip → khám cơ cấu → đi xuống bàn dựng. Các ca hỏng save/IO sau đó dùng **bản sao save đã tạo bằng UI**, sửa file/handle có phạm vi trong profile riêng; đó là fixture hỗ trợ lỗi + input UI tổng hợp, không phải tuyến chơi tự nhiên của save hỏng. Backup được giữ, khôi phục trong finally; không sửa ACL, làm đầy ổ hoặc kill Unity.

## 4. Kết quả riêng trong scene chung

| Acceptance | Kết quả và bằng chứng thực tế |
|---|---|
| Import/compile/reference | **PASS phạm vi kiểm** trên HEAD c18524a3b: 32.958 component, 0 Missing Script, không phát hiện serialized reference hỏng; 1 session, movement/look/eye/gate/modelGate/stageCamera/stageTexture đều bound. Không có error CS/NullReferenceException/MissingReferenceException của lượt chạy sau sửa công cụ. Cảnh báo Burst/WDAC và licensing/cloud giữ trong log; không nhận là một lần import sạch mọi warning. |
| Khám chưa ghi | E chỉ acquire slide; observations rỗng, firstFloorRecorded=false. P trước ghi không đặt firstNotesReviewed và không nhảy sang mục tiêu điện thoại. F5/F6 trước ghi phục hồi đúng trạng thái chưa ghi. |
| Ghi → mục tiêu → lặp → lưu/tải | **PASS**: ghi slide có [0,1], firstFloorRecorded=true, mục tiêu P xem ghi chép; ghi lặp không thêm observation. P đặt firstNotesReviewed và mục tiêu điện thoại; không cấp thông tin/kết luận mới. Save/Load trước và sau review giữ đúng bước. File `observation-ui-results.json` có 6 kiểm cuối đạt; các kiểm/lượt lỗi công cụ trước được giữ riêng, không gộp thành một tổng PASS mới. |
| Điện thoại/clip/cơ cấu/bàn | **PASS tuyến đại diện** bằng controller: E phone, nút phát clip, xem đủ trước/sự kiện, E cơ cấu rồi E bàn dựng vật lý. Chưa đi toàn tuyến Hà/mật mã/kết án trong đợt này. |
| Rời/quay lại bàn dựng | **PASS ca release đại diện**: R ra world, chờ 12 giây: TrialTime 1,50986969 giữ nguyên, 0 record; E trở lại tiếp tục. Rời bằng P khi đang chạy cũng giữ time/record, trở lại bằng nút Bàn dựng lại không chạy bù. Esc/E sau pause tay vẫn giữ TrialRunning=false và time 3,303325. |
| Record / save-load / pause | **PASS đại diện**: trở lại hoàn tất release tạo đúng 1 record; R/E mở lại không đổi record. F5 lưu, bắt đầu help, F6 giữ record cũ và reset live trial đúng thiết kế; tiếp tục đi/lưu/tải được. Pause game khi đã pause tay giữ time, resume/E không tự bật timeline. Chưa chạy lại cả sáu diễn biến, seek/clear-holds trong scene chung; các ca đó chỉ có bằng chứng API/UI sandbox trước. |
| Load lỗi được giao | **PASS 7 mẫu file**: index999, duplicate evidence, JSON bị cắt, relation first/second lạ, first=null, relations=[null]. F6 từ chối, data và identity giữ nguyên, bytes/SHA file lỗi giữ nguyên; sau mỗi mẫu F6 hợp lệ tải lại được. Thử thêm trong dossier/pause: context, state, bytes giữ nguyên; đóng bằng P/Esc và di chuyển được. Không exception lookup tiêu đề trong các ca này. |
| JSON evidence=null ngoài tập cũ | **OPEN INT-SAVE-NULL-LIST-01**: hai lượt F6 tải thành công và thay state, 4 vật chứng thành 0. Đây là phát hiện mới, không đổi PASS của các mẫu đã nêu thành FAIL hoặc tuyên bố mọi cấu trúc null đều an toàn. Xem yêu cầu tái hiện bên dưới. |
| Ghi save và lỗi IO | **PASS phạm vi mô phỏng**: F5 lần đầu/ghi đè, F6 phục hồi và tiếp tục; 4 lỗi IO qua F5: temp là directory, khóa mở temp, khóa replace đích, khóa replace temp nguồn. Mỗi ca Message báo lỗi, save đích cũ giữ bytes, F6 đọc được; bỏ blocker/đóng handle, F5/F6 lưu/tải tiến trình mới đúng. Temp tồn dư được giữ ngoài Git và lần lưu sau thành công. Partial-write chỉ có API trước, không chạy lại; không kiểm mất điện/crash thật. |
| Navigation sau lỗi | **PASS lượt này**: 3 chu kỳ P mở/đóng, WASD không lọt qua modal; pause chặn di chuyển, resume đi được. E mở bàn sau toàn bộ lỗi; bàn pause/resume theo ngữ cảnh giữ cờ pause tay. Không dùng các lượt này để đóng bất thường P-close lịch sử chưa rõ nguồn. |

## 5. UI 1080p và người mới — ghi riêng

- **UI 1920×1080: đã kiểm kỹ thuật có giới hạn.** Screen.width/height đo thật và ảnh Game capture PNG 1920×1080; desktop/GameView scale 0,86 không được dùng thay kích thước render. Đã xem màn intro, inspect dấu sàn/phone/cơ cấu, dossier, clip, reconstruction, compare, conclusions và pause; chữ/nút của tuyến đã thao tác đọc được, truy cập được. Không phát hiện clipping chặn tuyến này.
- **UI-LOAD-FEEDBACK-01 OPEN, tái xác nhận:** lỗi có Message nhưng dossier/pause diagnostics=false không vẽ Message. F6 lỗi không kẹt nhưng người chơi không thấy lý do ngay tại modal. Giữ yêu cầu hiển thị qua kết quả hiện có, không nhân đôi validator. Toàn bộ session partial kể cả UI vẫn do Gameplay & Dữ liệu giữ; cần thống nhất chủ sửa trước bản vá, đợt này không sửa.
- **P-close OPEN riêng:** ba chu kỳ lượt này đạt, không tái hiện bất thường cũ. Chưa xác định nguồn lỗi cũ, không tuyên bố đã sửa.
- Mật mã nhập đúng/sai/dở, nhập liệu dài, mọi mức scroll/độ phân giải và toàn tuyến kết luận: **NOT_RUN trong lượt tích hợp này**. Trang compare/conclusions chỉ kiểm hiển thị/navigation với vật chứng đã thu, không nhận là đã giải hoặc kết án.
- **Playtest người mới NOT_RUN**: không có phiên người thật; input tổng hợp và fixture không thay thế.

## 6. Yêu cầu gửi Gameplay & Dữ liệu: INT-SAVE-NULL-LIST-01 — OPEN

**Phạm vi:** luồng deserialize → validation → Load, không phải UI. Không sửa session partial song song.

1. Trên nguồn runtime91835c80, Unity6000.6.2f1, scene ConRoi_Chinh, profile thử có save hợp lệ đã thu slide/phone/clip/mechanism bằng UI. Sao lưu bytes save; ghi nhận live data identity và số evidence=4.
2. Chỉ trên bản sao save profile thử, thay toàn bộ giá trị trường `evidence` bằng JSON `null`, giữ schema/namespace và các trường khác.
3. Nhấn F6 trong world. **Thực tế hai lần:** Message “Đã tải hồ sơ riêng; thử nghiệm đang chạy trở về trạng thái an toàn.”; dataIdentity đổi, evidence từ4→0; file input giữ bytes. Sau đó restore bản hợp lệ và F6 phục hồi được.
4. **Kỳ vọng cần xử lý theo guard hiện có:** cấu trúc danh sách bắt buộc bị hỏng không được silently nhận và thay tiến trình đang chơi. `InvestigationSaveValidation.TryValidate` có kiểm incoming.evidence==null, nhưng đường JSON null đã đi tới state có danh sách rỗng. Đề nghị xác minh semantics JsonUtility/default initializer trước validation; không suy quy tắc null cho mọi trường hoặc tự đổi schema.
5. Bằng chứng local: `reject-structure-null-before/after.*`, `specimen-structure-null.json`, `null-list-repro2-before/after.*`, `specimen-null-list-repro2.json`, `modal-and-null-list-results.json`. SHA mẫu lần2: `0d0821ce14d5a6a6ac77ce8472ba97362e9df787ae6799c2b85d715633fbbce8`.

Không F5 ghi đè mẫu lỗi sau khi load, không mất save người dùng; save thử hợp lệ cuối được khôi phục. Chưa vá hoặc tự mở rộng thành validator JSON tổng quát. Gameplay & Dữ liệu review và bàn giao bản sửa/acceptance nếu xác nhận; UI/UX & Tích hợp kiểm lại trên scene chung sau đó.

## 7. Lịch sử lượt kiểm và dữ liệu ngoài Git

- Công cụ đầu `edaf5d805` dùng API reference cũ gây CS0619/SafeMode; `c18524a3b` sửa công cụ sang API EntityId. Giữ log lần lỗi, không gọi là lỗi runtime hoặc giấu bằng PASS cuối.
- Các thử aim/route đầu chưa hiệu chỉnh sensitivity và va cầu thang; giữ input/snapshot như thử hiệu chỉnh, không suy lỗi map hoặc bỏ qua để gọi tuyến đầu tiên hoàn hảo.
- Hai assertion đầu về mục tiêu ghi thất bại do decode cp1252 và PowerShell pipe sai Unicode. Snapshot thực đã có cờ/mục tiêu đúng; đã dùng UTF-8 và kiểm lại bằng UI. Giữ `observation-ui-attempt1/2-results.json` cùng kết quả cuối, không sửa FAIL công cụ cũ thành PASS gameplay.
- `load-guard-ui-results.json` dừng ở exploratory evidence=null do assertion từ chối không đạt; finally giữ/restore bytes hợp lệ. Lượt continuation kiểm riêng JSON cắt và relation, không đổi mẫu null thành mẫu khác để nhận PASS.
- [Manifest bằng chứng](evidence-manifest.json) chỉ index/hash đường dẫn local. Save/profile, capture, desktop overlay, log dài và os-input không commit vào Git. Ảnh Game riêng không có cửa sổ video/IME desktop lẫn trong một số ảnh OS.
- Đã snapshot trước dừng Play; sceneDirty=false, thoát Edit không lưu scene. Ba settings newline trước/sau được giữ bytes, chỉ phục hồi byte gốc khi normalized text khớp; diff semantic nếu có phải giữ và báo.

## 8. Bàn giao và bước tiếp

Nhận **toàn bộ lịch sử 0d878e3b..91835c80** cùng merge review d765ed92, công cụ c18524a3b và commit báo cáo sau kiểm; không chỉ lấy tài liệu cuối. Review branch C là nơi nhận kiểm cục bộ, chưa thay baseline các nhánh task hoặc main. Project D và công việc riêng giữ nguyên.

Core flow đại diện trong scene chung đã có bằng chứng độc lập; vẫn cần Gameplay & Dữ liệu nhận INT-SAVE-NULL-LIST-01, phối hợp UI-LOAD-FEEDBACK-01 và duy trì P-close riêng. Chưa chốt toàn bộ regression scene chung, UI1080p mọi màn, toàn map hoặc người mới PASS. Không cần wiring mới cho năm gói đã nhận; không tự tích hợp bản vá chưa có. Không push/merge remote, không tạo build/ZIP.

### Đối chiếu sau thoát

[Biên nhận bảo toàn](preservation-results.json): ba settings có normalized text khớp snapshot trước, đã trả đúng bytes gốc. Hai RenderTexture có bytes khớp blob HEAD; không stage file sản phẩm. Trạng thái D so trước: **True**, HEAD `ebd92771047435e53975205b35bb8213c25cc779`; branch `work/ui-onboarding-review`. Giữ toàn bộ thay đổi riêng.
