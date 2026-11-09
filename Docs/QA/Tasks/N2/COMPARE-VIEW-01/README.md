# COMPARE-VIEW-01 — Hai góc xem vật chứng độc lập

Gameplay & Dữ liệu. **RUNTIME ĐÃ KIỂM → CHỜ UI/TÍCH HỢP.** Đây là runtime preview mới; UI sản phẩm vẫn là CompareUI văn bản cũ, chưa gọi API này. **Cần wiring**, chưa có kết quả scene chung/1080p/người mới.

## 1. Mục đích, căn cứ và acceptance

[GDD9](../../../../CON_ROI_GDD_v3.6_CHOT_TRIEN_KHAI.md) cho phép đổi góc khi so hai vật; GDD6.5 yêu cầu phóng/mô tả khi so mẫu giày/vải, không kéo khớp pixel hoặc chỉ phân biệt bằng màu. GDD7.2 giữ quan hệ là giả thuyết người chơi ghi. Baseline CompareUI chỉ có mô tả hai mục, chưa có hai model preview. Acceptance trước nguồn nằm trong [tiến độ](../../../../QuanLy/DANG_LAM_NGUOI_2.md), mục “Hai gói viewer”.

Đã triển khai chọn/xóa/đổi vật, xoay/phóng/đặt lại từng bên, texture riêng và mô tả qua guard EvidenceTranscript. Không tự Acquire/Observe/AddRelation, chấm đúng/sai, cấp kết luận hoặc sửa luật cơ cấu. Không đổi schema1/ID/GUID hiện có. Mục đã thu nhưng không có model vẫn đối chiếu bằng chữ; không biến thiếu asset thành gate gameplay mới.

Acceptance: hai góc/texture độc lập; reject ID chưa thu/sai side/ngữ cảnh/pause không đổi lựa chọn; số không hữu hạn không đổi pose; reset/lặp idempotent; snapshot không chạy script/physics nguồn; pause giữ góc, rời trang dọn cả hai; Load lỗi giữ state/view/file, Load hợp lệ dọn selection tạm; không mất dữ liệu điều tra hoặc lộ bản chép clip sớm.

## 2. Nguồn và file

- Nguồn đầu `5ef32f002fc94ca168734b8f539e354d037be132`: **InvestigationComparisonViews.cs** + meta mới; hook SetModalState/Load/OnDestroy trong **InvestigationSession.cs**. Không sửa SessionUI, scene, asset, GDD, validator/writer.
- Sửa cô lập camera `0b783b8c9cbd4138ce9efa2ed770cb01f351b941`: far clip10 cho model chuẩn hóa1,2, slot cách30. Nguồn này thay cho bản camera khoảng nhìn mặc định; nhận cả hai commit nguồn.
- Công cụ **Assets/ConRoi/Modules/Gameplay/Editor/EvidenceViewChecks.cs**: `23fc0cb7 →175a248c →49c242a6 →d671a845`; xem kết quả thực chạy và SHA đầy đủ trong [verification.json](verification.json). Nhận toàn chuỗi cùng báo cáo, không chỉ cherry-pick commit tài liệu.
- Phụ thuộc gói INSPECT-CONTROLS-01 (`b8cc108f` và các commit sau): dùng EvidenceViewPose/EvidenceViewMath hiện có; không đổi public API đã bàn giao.

## 3. API thực tế

Namespace `ConRoi.Investigation.V36.R107`; owner toàn session partial là Gameplay & Dữ liệu. Gọi Unity main thread trên session đã khởi tạo đúng binding. Không gọi trong repaint hoặc mỗi frame để ép trạng thái.

| API | Tham số, kết quả và ý nghĩa |
|---|---|
| `GetComparisonView(ComparisonSide side)` | Snapshot chỉ đọc; side `Left` hoặc `Right`. Side sai trả Empty/Diagnostic, không throw |
| `TrySelectComparison(ComparisonSide side, string id, out string error)` | ID phải có trong InvestigationContent và session.Has. Tìm interactable active cùng evidenceId; ưu tiên inspectionPrefab; mechanism dùng gate. Thành công true/error rỗng, gồm text-only |
| `TrySelectComparison(ComparisonSide side, string id, Transform source, out string error)` | Source tùy chọn. UI/asset adapter chịu trách nhiệm model đúng ID; runtime không chứng minh model mỹ thuật khớp ID. Source null dùng tìm tự động như trên |
| `TryAdjustComparison(ComparisonSide side, float yawDegrees, float pitchDegrees, float distanceDelta, out string error)` | Delta độ; distance dương=xa. true khi tiếp nhận, false/lý do khi khóa/thiếu model/số không hữu hạn; không đổi tiến trình |
| `TryResetComparison(ComparisonSide side, out string error)` | Về default ID; không đổi bên kia. Lặp ở default vẫn true, không event thừa |
| `TryClearComparison(ComparisonSide side, out string error)` | Xóa một slot, giải phóng camera/model/RT riêng. Được cleanup cả ngoài trang/khi pause; lặp an toàn. Side sai false |
| `ComparisonViewChanged` | `event Action<ComparisonSide,ComparisonViewState>`; trạng thái sau command có đổi hoặc refresh ngữ cảnh. Không phải event tiến trình hay xác nhận relation |

Pose: yaw chuẩn hóa0–360, pitch−80…80, distance0,5…4; NaN/Infinity bị từ chối, finite lớn clamp an toàn. Default kế thừa viewer: yaw25; shoe/shoecontrol pitch65, slide/shoeprint/fabric/coat pitch−65, còn lại10; distance slide/shoeprint1,65, còn lại2. Đây là khung nhìn hữu ích ban đầu, không khóa người chơi ở một góc.

Chọn lại cùng ID giữ pose/texture, không rebuild; để làm mới model sau khi asset có sẵn, **Clear rồi Select** theo action/context đã thống nhất. Chọn ID khác dọn ảnh cũ. Khi nhiều interactable trùng ID, truyền source rõ ràng nếu cần xác định model thay vì phụ thuộc thứ tự tìm.

## 4. State/event và dữ liệu mẫu

`ComparisonViewState` gồm `EvidenceId`, `Transcript`, `Diagnostic`, `Pose`, `Preview` (Texture), `Availability`, `CanControl`.

| Availability | Xử lý UI |
|---|---|
| Empty | Chưa chọn; không ảnh, khóa rotate/zoom/reset; cho chọn mục đã thu |
| Ready | Có hình và controls dùng được; mô tả vẫn phải đọc được |
| MissingVisual | Chọn thành công nhưng text-only; Preview=null, Diagnostic giải thích; rotate/reset false/lý do, không giữ ảnh vật cũ |
| Paused | Giữ ID/pose/texture, preview root ngừng render; khóa chọn/điều chỉnh, tiếp tục game giữ góc |
| WrongContext | Không ở Page==compare; không hiện texture, không nhận chọn/điều chỉnh |

Event phát sau chọn/đổi/clear/adjust/reset thật sự thay đổi; SetModalState trong compare có thể phát lại cả hai snapshot (open/pause/resume). Failure/no-op command không phát thay đổi. Event không cam kết đúng một lần cho toàn thao tác Load hay context refresh; adapter phải chấp nhận snapshot lặp. Callback lỗi được cô lập bằng warning, không hủy command; đừng thay session trong callback để tạo vòng lặp. Đọc snapshot ban đầu khi đăng ký, hủy đăng ký khi view tắt/hủy.

Transcript chỉ lấy EvidenceTranscript của ID đã thu. Clip chưa đủ hai vùng quan sát trả hướng dẫn, không bản chép thật. Exposure thay đổi không có event riêng của viewer: refresh getter khi mở trang/đổi mục hoặc theo event media hiện có của adapter; không cache bản chép qua các profile. Không dùng raw definition.transcript để bỏ guard.

Dữ liệu ví dụ: `shoe` và `shoecontrol` là hai ID mẫu đang có; fixture Acquire chúng chỉ để kiểm viewer, **không phải tuyến thu vật tự nhiên**. `letter` không model vẫn text-only; `clip` cần flags quan sát hợp lệ từ media thật. Không tạo ID mới. Source hỗ trợ MeshFilter/MeshRenderer, SkinnedMeshRenderer được BakeMesh thành hình tĩnh, LineRenderer (đổi world points về local). Không clone hành vi, collider, Rigidbody, audio, camera, particle/animation. Shared mesh/material chỉ đọc, baked mesh thuộc preview và được dọn.

## 5. Ví dụ gọi và wiring từng bước

Ví dụ dưới là adapter cần triển khai trong phạm vi UI/UX & Tích hợp, **không phải component đã tồn tại**. session là tham chiếu serialized/binding thật do scene cung cấp, không tìm theo tên Hierarchy.

```csharp
// Khi view được bật: đăng ký đúng một lần và vẽ hai snapshot hiện tại.
session.ComparisonViewChanged += OnComparisonChanged;
OnComparisonChanged(ComparisonSide.Left, session.GetComparisonView(ComparisonSide.Left));
OnComparisonChanged(ComparisonSide.Right, session.GetComparisonView(ComparisonSide.Right));

// Trong action chọn vật đã thu, sau khi OpenPage("compare") theo navigation hiện có:
bool accepted = session.TrySelectComparison(ComparisonSide.Left, "shoe", out string error);
// Nếu false: hiển thị error; true vẫn phải đọc Availability/Diagnostic, có thể text-only.
bool adjusted = session.TryAdjustComparison(ComparisonSide.Left, 15f, 0f, -.1f, out error);
bool reset = session.TryResetComparison(ComparisonSide.Left, out error);

// Khi view bị tắt/hủy:
session.ComparisonViewChanged -= OnComparisonChanged;
// OnComparisonChanged(ComparisonSide side, ComparisonViewState state) là hàm adapter tự viết;
// nhận state rồi gán texture/mô tả/diagnostic/enable controls tương ứng với bên đó.
```

1. UI/UX & Tích hợp tạo renderer/adapter trong module GiaoDien theo ownership; nối session bằng reference thật. Nếu cần chạm partial legacy để gọi, giao yêu cầu/patch cho Gameplay & Dữ liệu, không sửa partial song song.
2. Navigation hiện có mở compare; lựa chọn chỉ liệt kê vật đã thu. Nối action chọn từng bên với TrySelectComparison; gán Preview vào phần hiển thị texture của framework UI thực tế. Không đặt tên nút/đường dẫn đối tượng giả trong contract.
3. Hiển thị Transcript và Diagnostic cạnh ảnh; chữ vẫn dùng được khi model thiếu. CanControl điều khiển rotate/zoom/reset; Empty/MissingVisual không chặn đọc/ghi note hợp lệ.
4. Nối điều khiển bên đang focus sang TryAdjustComparison/Reset. Có cách điều khiển không drag chính xác; mức delta và vị trí action do UI thiết kế. Runtime chưa có mouse polling cho cặp viewer, adapter phải route delta **một lần cho đúng bên**.
5. Giữ AddRelation hiện có chỉ khi người chơi chủ động ghi, lấy hai EvidenceId hợp lệ và note; viewer không tự ghi relation/đánh giá đúng sai. Existing selectedA/selectedB của CompareUI văn bản chưa tự đồng bộ với API mới: adapter phải dùng một nguồn lựa chọn rõ ràng, không trộn hai bộ ID.
6. Khi nhận event, đọc snapshot; khi rời trang bỏ texture khỏi renderer để không giữ tham chiếu RT đã hủy. Không tự Destroy texture do session sở hữu. Subscribe/unsubscribe đúng vòng đời, không đăng ký trong OnGUI/repaint.
7. Map & Asset cung cấp inspectionPrefab/model đúng ID và kiểm mặt quan trọng. Snapshot chuẩn hóa kích thước để xem, **không phải tỷ lệ vật lý đo lường**; không suy khớp mẫu từ tỷ lệ ảnh. Kiểm shader/material thật, skinned/line, orientation, ánh sáng và độ đọc được trước khi chốt presentation.

## 6. Input, pause và save/load

Page compare là modal hiện có: chặn movement/look; viewer không đổi khóa input hoặc tự điều hướng. Pause ngừng render nhưng giữ lựa chọn/pose; resume tiếp tục cùng góc. Clear dùng cleanup an toàn cả khi paused. Khi rời compare, hai preview bị dọn, quay lại chọn lại; không có cơ chế “đã xem” mới.

Selection/pose/texture là transient, **không vào schema save**. Save vẫn lưu evidence/observations/relations/trials/conclusions theo cơ chế cũ. Load hợp lệ dọn preview trước áp dụng dữ liệu, mở compare trở lại ở Empty nếu save lưu trang compare; không tự ghi restore selection. JSON/validation từ chối trước apply giữ view và state, không rewrite save. Không thêm checkpoint/fallback hoặc đổi IO guards. UI phải xử lý kết quả Load/Message theo contract hiện có.

## 7. Kiểm độc lập và lịch sử

Kết quả chi tiết và SHA nằm trong verification.json, chỉ assertion đã chạy; không commit raw log, save hoặc capture. Môi trường Unity6000.6.2f1, scene `Assets/ConRoi/Modules/Gameplay/Scenes/Gameplay_ThuNghiem.unity`, dựng môi trường proxy tạm trong Play, profile mới từng lượt. API/fixture, **không UI/controller, không input Windows, không chơi tự nhiên**. Geometry cube/line và flags clip fixture không chứng minh model/media thật hoặc tuyến điều tra scene chung.

- Comparison trên23fc0cb7:40/40 assertion ban đầu đạt, chưa có assertion khoảng nhìn.
- ComparisonIsolationBaseline trên175a248c: kiểm cô lập camera **FAIL**, dừng suite; giữ lịch sử, không gọi toàn gói PASS ở nguồn này.
- ComparisonIsolated trên0b783b8c:41/41 đạt; cùng nguồn sửa camera.
- ComparisonRendered trên49c242a6: pixel hình học **đã đạt**; assertion kế tiếp “0 MonoBehaviour sau render” FAIL, suite dừng. Assertion trước render ở các lượt trước đã đạt; camera.Render qua pipeline có thể thêm component camera, nên không suy rằng runtime đã clone script nguồn. Công cụ d671a845 kiểm snapshot trước render, rồi kiểm riêng không có InvestigationInteractable/Collider/Rigidbody sau render. Không sửa runtime cho lỗi assertion này.
- ComparisonRenderFixtureFixed trên **d671a8458bd9c02c26e746db0858392caad64ad9:43/43 đạt**, có đọc pixel thực từ RenderTexture; source runtime vẫn0b783b8c. Render cube/line thử không chứng minh độ đọc được của model thật. Các fail giữ riêng trong verification.json; số failures bao gồm exception kết thúc suite, không phải số lỗi độc lập.
- InspectionComparisonRegression trên d671a845: **27/27 đạt** do hook modal/Load/destroy dùng chung. Mouse ảo được poll bằng InputSystem.Update rồi reflection gọi Session.Update, **không UI/controller hoặc natural frame input**. Không cộng lượt lặp thành số ca độc lập.
- Warning subscriber exception trong fixture là chủ động; không che exception ngoài dự kiến. Không chạy lại các bộ save IO/trial cũ.

Chạy lại từ worktree riêng đã sạch/thay đổi riêng được bảo toàn, Editor khác đóng; thay $unity thành vị trí Editor6000.6.2f1 thật trên máy. Mỗi output/profile phải **chưa tồn tại**, không dùng save gốc:

```powershell
$project = (Get-Location).Path
$unity = 'D:\data_unity\6000.6.2f1\Editor\Unity.exe'
& $unity -batchmode -projectPath $project -executeMethod ConRoi.Gameplay.EditorChecks.EvidenceViewChecks.RunBatch --view-suite comparison --view-source (git rev-parse HEAD) --view-output "$project/Test/Output/ComparisonNew" --investigation-save-dir "$project/Test/PlaytestProfiles/ComparisonNew" -logFile "$project/Test/Output/ComparisonNew.log"
```

## 8. Acceptance tích hợp và giới hạn

UI/UX & Tích hợp nhận cả chuỗi, wiring rồi kiểm trên commit scene chung thực chạy, ở1080p: thu vật thật → chọn trái/phải → xoay/phóng/reset độc lập/lặp → đổi mẫu/text-only → ghi note quan hệ bằng action cũ → pause/resume → rời/quay lại → Load lỗi giữ view/state → Load hợp lệ dọn view → tiếp tục chọn/ghi/lưu/tải. Kiểm clip chưa xem/chỉ một vùng/đủ cả hai không lộ chữ sớm; không dùng fixture cấp chứng cứ rồi báo tuyến tự nhiên PASS.

Kiểm input P/E/Esc, di chuyển/modal và focus thật; tránh hai consumer cùng feed delta. Kiểm hình không lẫn bên kia ở các góc, model/texture không che mô tả/header/footer, không chỉ dựa màu. Snapshot là hình tĩnh, không mô phỏng cơ cấu hay giữ animation; material phụ thuộc shader/source thật, chưa bảo đảm mọi asset có preview. Model thật/scene chung/1080p/người mới **NOT_RUN**. P-close và IO-OPEN-01 **OPEN riêng**, không lấy kết quả viewer làm căn cứ đóng.

Chỉ chuyển **CHỜ UI/TÍCH HỢP → TÍCH HỢP ĐÃ KIỂM** khi có báo cáo từ phía tích hợp; không merge/chốt baseline nhóm trong gói. Đường chờ và chủ nhận nằm trong tiến độ Gameplay & Dữ liệu.
