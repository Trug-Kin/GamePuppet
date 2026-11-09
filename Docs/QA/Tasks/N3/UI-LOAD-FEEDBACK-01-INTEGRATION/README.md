# UI-LOAD-FEEDBACK-01 — kiểm tích hợp 1080p

**PASS trong phạm vi acceptance dưới đây.** UI/UX & Tích hợp, 09/11/2026. Nguồn thực chạy: `f242b60a2fb48c45b37caabd8a55836e4d7db53b`, nhánh `review/runtime-integration-20261109`, checkout `C:/ConRoi_CheckoutTest`. Commit báo cáo đi sau chỉ ghi tài liệu, không phải nguồn đã mở lại Unity.

## 1. Nhận đủ chuỗi và review

Nhận toàn bộ `25f14632b64f140bde9a06ecb0c32dbdda33deb2` nguồn → `5be056e4883a7c3cc49ba52e370d75ace2bc1510` công cụ/meta và nguồn đã kiểm phía Gameplay & Dữ liệu → `a374a93aab3f37bc122a26ac7a2a9d81356eee3b` báo cáo. Merge giữ lịch sử trên review branch từ `ef8911965d766507350767c8bc4d491c4618298e`, không conflict, không nhận chỉ commit cuối. Không wiring mới.

Đã review các file của chuỗi: `InvestigationSessionUI.cs` đọc Message hiện có, wrap/scroll; footer chung dành chỗ cho thông báo, header riêng cho clip/bàn dựng giữ vùng điều khiển cố định. Vòng đời vẫn 6 giây unscaledTime. `InvestigationSession.cs` dùng kết quả/catch Load hiện có, lý do validation được giữ, lỗi đọc/JSON trả câu an toàn. Không thêm validator trong view, không thay state apply, guard save, schema, ID, GUID hoặc NAV-01. UI/UX & Tích hợp không sửa session partial; các nguồn được đối chiếu hash trước/sau kiểm, xem [source-comparison.json](source-comparison.json).

## 2. Môi trường và nguồn dữ liệu kiểm

- Unity **6000.6.2f1**, scene thật `Assets/ConRoi/Modules/TichHop/Scenes/ConRoi_Chinh.unity`, Game render **1920×1080**; ảnh Game capture là 1080p, cửa sổ desktop fit 0,86×. Không suy PASS từ scene Gameplay.
- Profile mới `Test/PlaytestProfiles/LoadFeedbackIntegration20261109`; không dùng LocalLow/save gốc. C còn 13.577.515.008 byte trước mở; dùng Library riêng đã có, import tăng dần, không fresh clone/import lại.
- Bấm Bắt đầu bằng UI và F5 tạo save đầu trong profile mới. Sau đó dùng **bản sao navigation fixture** `Test/Output/UIFeedbackPClose20261109/UI/valid-continued-save.json`: bốn vật chứng từ lượt UI tích hợp trước và thư được thêm trong fixture trước đó; mapping đã được nhập bằng UI ở lượt cũ. Đây là hỗ trợ mở các trang, **không phải tuyến điều tra tự nhiên/New Game mới/người mới**. Nguồn fixture giữ nguyên SHA256, ghi trong [results.json](results.json).
- Toàn bộ thao tác kiểm dùng **input Windows tổng hợp**: nút Tải, F5/F6, P/E/Esc, WASD, chuột. Bridge chỉ đọc state/reference/capture và stop/quit; không grant evidence, API Save/Load, chuyển trang hay teleport live.
- Lỗi đọc mô phỏng bằng exclusive handle chỉ file của profile thử, đóng trong `finally`; JSON/evidence:null chỉ sửa bản sao file thử. Không đổi ACL, làm đầy ổ hoặc crash Unity.

## 3. Acceptance và kết quả scene chung

Acceptance được ghi trước Play trong bản làm việc của báo cáo: có lý do ở dossier/pause; lỗi đọc/JSON không lộ chi tiết kỹ thuật; thành công thay lỗi cũ; header/footer/navigation/input không bị ảnh hưởng; từ chối không thay state/bytes.

| Ca kiểm | Thao tác và kết quả |
| --- | --- |
| Hồ sơ: validation | Nút Tải với evidence:null hiện đầy đủ lý do, không báo thành công; giữ state/identity/page/tiến trình/input và bytes. PASS. |
| Hồ sơ: JSON / đọc | F6 với JSON hỏng / handle khóa đọc hiện câu tiếng Việt an toàn, không exception/stack/path. State/bytes giữ nguyên. PASS hai ca. |
| Pause: validation | Nút Tải hiện lý do trong footer, vẫn pause đúng. State/bytes giữ nguyên. PASS. |
| Pause: JSON / đọc | F6 hiện lý do an toàn, giữ state/bytes, không thành công giả. PASS hai ca. |
| Hồi phục sau từng lỗi | Khôi phục bản sao hợp lệ rồi F6: thông báo `Đã tải hồ sơ riêng…` thay lỗi ngay. PASS cả sáu cặp từ chối/hồi phục. |
| P và modal | Ba chu kỳ P mở/đóng, W không lọt khi hồ sơ mở. PASS phạm vi đại diện; không đóng P-close lịch sử. |
| Tiếp tục lưu/tải | F5 → A thay vị trí bằng controller → F6 phục hồi đúng vị trí. PASS; không phải chạy lại toàn bộ bộ save. |
| Mật mã | Mở trang, focus ô chữ, Trở lại → P mở hồ sơ. PASS; không nhập lại toàn bộ nghiệm đúng/sai trong lượt này. |
| Đối chiếu / kết luận | Mở và xem các màn ở 1080p, nút/ô nhập không bị footer đè, Trở lại đúng world rồi P mở hồ sơ. PASS phạm vi layout/navigation; không tạo kết luận mới. |
| Bàn dựng / clip | F6 lỗi ngay trong trang: thông báo ở header đọc đủ, không che Trở lại/Hồ sơ; các nút/timeline/nhãn dưới còn hiển thị. Header Hồ sơ và Trở lại dùng được. Clip đã pause bằng UI trước thử lỗi. PASS. |
| E / Esc / pause | Aim chuột/controller tới ray table thật, E mở bàn dựng; Esc ra world, Esc pause, W bị khóa, Esc tiếp tục. PASS. |

**26/26 assertion navigation/input đại diện đạt**, bên cạnh sáu cặp từ chối/hồi phục. So sánh live trước/sau từ chối gồm full `data`, object identity, page/paused/modal/objective, position/yaw/pitch, movement/look, gate/modelGate, trial ID/time/running/report và clipTime; chỉ Message được đổi. Bytes file được đọc/so sánh sau khi nhả handle, trước khôi phục bản hợp lệ.

Ảnh 1080p đã trực tiếp xem: dossier JSON, pause validation, cipher, compare, conclusion, bàn dựng và clip có lỗi header. Không thấy header/footer che nội dung ở các màn/nội dung đại diện này. Chưa kiểm mọi độ dài chuỗi, locale, scale, thanh cuộn cực hạn hoặc mọi biến thể nội dung. Thông báo vẫn tự hết sau 6 giây theo contract hiện có; không đổi sang lỗi sticky.

## 4. Compile/reference, bằng chứng và bảo toàn

- Import/compile cho phép Play; scan log mục tiêu không có error CS, NullReferenceException, MissingReferenceException hay GUI control error. Audit scene: **32.958 component, 0 Missing Script, 0 broken reference**, một session, bindings movement/look/eye/gate/modelGate/stageCamera/stageTexture đủ. Không tuyên bố mọi checkpoint legacy đã kiểm.
- Có cảnh báo Burst/Windows app-control, licensing/cloud và toast Windows Security trong môi trường; không bypass cài đặt bảo mật. Không có căn cứ coi chúng là lỗi Load; không chứng minh mọi tính năng Burst/plugin hoạt động.
- Hai script kiểm đã dừng ở giả định sai về pause (`paused=true,page=""`) và nút Trở lại về world, thêm một lượt chạy tiếp còn precondition cũ; ghi riêng trong log, chỉnh **công cụ local**, không sửa sản phẩm hoặc kết luận FAIL gameplay. Đợi stop 4 giây còn state cũ trong import: lượt xác nhận sau thấy `play=false,sceneDirty=false`, rồi mới quit; không lưu scene.
- Không có foreground-loss exception trong lượt này; những lượt gián đoạn cũ vẫn giữ nguyên. Không dùng các lượt assertion chưa chạy làm PASS.
- [results.json](results.json): ma trận, 26 kiểm navigation, reference audit, preservation và trạng thái worktree khác. [evidence-index.json](evidence-index.json): đường dẫn/bytes/SHA256 của 59 ảnh local, không commit capture/profile/log thô. Local log, snapshots, os-input.jsonl, script, save và Before tại `Test/Output/LoadFeedbackIntegration20261109/`; profile tại đường dẫn riêng trên. Đây là dữ liệu máy kiểm, không bắt buộc có trên máy clone.
- Năm file Unity modified sẵn đã snapshot Before/After, nội dung chuẩn hóa không đổi và phục hồi đúng bytes trước kiểm. HEAD/branch/status/diff D:/ConRoi và C:/ConRoi_Nguoi2 giữ nguyên; deletion/untracked, legacy 4.183 file, archive và save gốc không bị xử lý.
- Không chạy lại **66 API bên giao** hoặc toàn bộ API/IO cũ. Kết quả API bên giao không được tính thành kết quả scene chung.

## 5. Bàn giao và baseline đề xuất

Đề xuất **`f242b60a2fb48c45b37caabd8a55836e4d7db53b`** làm **ứng viên baseline kỹ thuật** sau review nhóm: acceptance UI-LOAD-FEEDBACK-01 ở scene chung/1080p nêu trên đạt, không còn lỗi chặn được xác nhận trong phạm vi lượt này. Nhận đầy đủ lịch sử review và chuỗi nguồn/công cụ/báo cáo, không cherry-pick riêng tài liệu cuối; không cần wiring mới. Commit ghi báo cáo sau nguồn này chỉ tài liệu, xem Git history để lấy biên nhận.

**Chưa chốt baseline hoặc cập nhật nhánh nhóm/chore/main; chưa push.**

Giới hạn giữ riêng:

- **P-close OPEN / chưa tái hiện / nguyên nhân chưa xác định**. Ba chu kỳ và đường mật mã/pause/Load lỗi đạt trong lượt hiện tại không chứng minh lỗi cũ đã hết; không sửa phỏng đoán.
- **Người mới NOT_RUN**. Input tổng hợp và fixture không thay playtest người thật; toàn bộ UI1080p/mọi trường hợp nội dung không được gọi PASS.
- Probe bổ sung chưa chạy của gói trước vẫn **NOT_RUN**; không gộp với acceptance đã đạt. Các FAIL/lượt mất focus/lượt nhầm fixture lịch sử giữ nguyên trong báo cáo gốc.
- UI-LOAD-FEEDBACK-01 đã được xác nhận sửa ở scope acceptance này; báo cáo cũ dưới đây vẫn giữ đúng kết quả trước vá.

Liên quan: [phiếu gốc](../UI-LOAD-FEEDBACK-01/README.md), [UI-NAV-REVIEW-02](../UI-NAV-REVIEW-02/README.md), [báo cáo nguồn Gameplay & Dữ liệu](../../N2/UI-LOAD-FEEDBACK-01/README.md), [HANDOFF](../../../../QuanLy/HANDOFF.md).
