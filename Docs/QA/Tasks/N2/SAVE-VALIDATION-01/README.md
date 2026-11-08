# SAVE-VALIDATION-01 — Gameplay & Dữ liệu

Ngày kiểm: 08/11/2026. Nhánh `work/runtime-contracts-baseline`.
Commit xuất phát: `0d878e3b7163c4d50e3976cd5c46118403b19240`.
Commit nguồn bàn giao: `546833aace5f59c0c98349ebe0fb7cd33cbe5df0`. Hash nguồn đã chạy kiểm: [tested-source.json](tested-source.json).

## Lỗi đã tái hiện và sửa

`Observe("slide",999)` bị API hiện hành từ chối, nhưng `Load()` trên baseline nhận cùng chỉ số lỗi từ JSON. Save có hai record cùng ID cũng được nhận và thay state đang chơi. [Baseline](baseline-results.json): 16 ca, 2 lỗi; giữ nguyên kết quả FAIL này.

Thay hai điều kiện kiểm cũ trong `InvestigationSession.Load()` bằng `InvestigationSaveValidation.TryValidate(incoming,out error)` trước `data=incoming`. Lớp mới chỉ đọc dữ liệu, chặn schema/namespace sai, giá trị vị trí/góc/cơ cấu không hữu hạn, cấu trúc null, ID lạ/trùng và chỉ số quan sát ngoài definition. Không cấp chứng cứ, sửa schema1, ID, luật cơ cấu hay NAV-01. Không thay serialized field hoặc GUID hiện có.

Không phải bộ chấm nội dung/suy luận. Đảm bảo giữ state khi **validation từ chối**; chưa có rollback tổng quát cho lỗi binding runtime xảy ra sau khi validation đã đạt.

## Kiểm thực tế

Bảng sau ghi đợt fixture/API ban đầu; kết quả UI/controller bổ sung nằm ở mục cuối báo cáo. Giữ nguyên lịch sử NOT_RUN của đợt đầu.

Unity **6000.6.2f1**, worktree `C:/ConRoi_Nguoi2` với Library riêng. Scene thật: `Assets/ConRoi/Modules/Gameplay/Scenes/Gameplay_ThuNghiem.unity`.

| Phạm vi | Kết quả và giới hạn |
|---|---|
| Import/compile | Library mới import và compile thành công; bản sửa compile và chạy PlayMode batch thành công |
| PlayMode/API và validator thuần | [Sau sửa](fixed-results.json): **42 ca, 0 lỗi**; từng ca có tên và kết quả riêng |
| Profile sạch | Bắt đầu fixture với evidence/mapping/relations/trials rỗng, file save chưa tồn tại; chọn đúng CLI profile riêng |
| Save hợp lệ | Acquire/Observe bằng API → Save → Load → tiếp tục Observe/Save/Load; chỉ khôi phục quan sát đã ghi |
| Save lỗi | Từ chối quan sát âm/ngoài khoảng, ID trùng/lạ, schema/namespace sai, JSON đứt; giữ data, vị trí, gate, modal/pause/input flags và nội dung file lỗi |
| Validator thuần | Null collections/record/grounds/gate; số không hữu hạn; nhận định chưa hoàn tất vẫn hợp lệ; không sửa object đầu vào |
| Tương thích baseline | Đọc bản sao save thực do baseline retry tạo, tiếp tục Save/Load đạt; file baseline nguồn không đổi |
| Reference | Template rỗng có 0 Missing Script; sáu binding của fixture khác null. Không suy thành kiểm reference scene tích hợp |
| UI/controller, input tổng hợp, 1080p, tuyến xưởng → Hà → xưởng | **NOT_RUN** trong gói này; không gửi input tổng hợp. Fixture gọi API và cấp đúng dữ liệu thử được ghi rõ |
| Người chơi mới | **NOT_RUN**, không thay bằng fixture |

Lần chạy baseline đầu tiên chưa tạo report: lỗi `UnityEditor.Search.SearchDatabase` trong `EditorApplication.delayCall` chặn lịch chạy fixture. Đã bảo toàn state/diff trước khi dừng đúng batch do tác vụ mở. Chuyển lịch fixture sang `EditorApplication.update`, rồi baseline retry tái hiện được lỗi. Không tính lần thiếu report là PASS hoặc lỗi gameplay. Log kết nối cloud/telemetry không được dùng kết luận runtime lỗi.

## Cách chạy lại có kiểm soát

Đóng Editor của **checkout thử riêng**, kiểm không có thay đổi chưa bảo toàn. Tại root checkout, dùng PowerShell với đường dẫn Editor thật trên máy; không chạy song song Editor cùng checkout. Mỗi lần dùng profile mới; không chỉ lại profile live hoặc profile đã dùng. Tool từ chối profile đã tồn tại.

```powershell
$editor = 'D:\data_unity\6000.6.2f1\Editor\Unity.exe' # đổi theo cài đặt máy
$project = (Get-Location).Path
$run = [guid]::NewGuid().ToString('N')
$profile = Join-Path $project "Test/PlaytestProfiles/SaveContract_$run"
$output = Join-Path $project "Test/Output/SaveContract/$run"
New-Item -ItemType Directory -Path $output | Out-Null
& $editor -batchmode -nographics -projectPath $project -executeMethod ConRoi.Gameplay.EditorChecks.SaveLoadContractChecks.RunBatch -logFile (Join-Path $output 'Editor.log') --investigation-save-dir $profile --save-contract-output $output
```

Tool tự mở scene thử, tạo fixture tạm, thoát Play rồi thoát batch; không lưu scene. Đọc `results.json`, `failures` và log. Có thể thêm `--save-contract-compat` với đường dẫn tới **bản sao** save baseline đã bảo toàn; không dùng save người dùng. Không có tham số này thì các ca tương thích tùy chọn không chạy, không tự gọi 42 ca là đã đạt.

## Tích hợp và quyền sở hữu

UI/UX & Tích hợp nhận commit nguồn trên sau review contract, tích hợp một gói vào nhánh tích hợp do nhóm thống nhất. Không chép scene hoặc sửa wiring: các caller Load/F6 hiện có tự dùng guard mới. Nhận cả `.cs/.meta` mới và folder Editor/meta; fixture chỉ chạy khi gọi batch tường minh, không gắn vào game. Kiểm compile và luồng Load chịu ảnh hưởng trên checkout tích hợp; tuyến UI/controller còn chờ kiểm. Gameplay & Dữ liệu giữ toàn bộ session partial.

Không yêu cầu Map & Asset thay mesh/collider. Registry chung đang thiếu ID `hook` so với source 20 ID: gửi UI/UX & Tích hợp review/cập nhật index; gói này không sửa thay registry hoặc ID runtime. Không cần migration save hay callback/event mới.

## Bảo toàn và bằng chứng

[Manifest bằng chứng local](evidence-manifest.json) ghi bytes/SHA256 log, report và snapshot. JSON kết quả nhỏ được giữ cùng báo cáo; log dài và profile ở `Test/Output/SaveContract/` và `Test/PlaytestProfiles/`, không commit, không phải runtime dependency. Có thể yêu cầu Gameplay & Dữ liệu cung cấp theo manifest.

Unity tự sinh thay đổi ở URP settings, `_Recovery/0 (2).unity` và ba file ProjectSettings. Đã copy nguyên bytes, kiểm hash vào `Test/Output/SaveContract/ImportedChanges/manifest.json`, rồi khôi phục riêng năm file đó về HEAD; không đưa vào commit. Không khôi phục thay đổi người dùng.

Project gốc `D:/ConRoi` giữ nhánh UI và deletion hướng dẫn chưa commit, untracked/legacy/profile/archive nguyên trạng. Snapshot trước tác vụ: `C:/ConRoi_N2_Preservation_20261108`. Không push/merge, không sửa scene tích hợp, checkpoint, GDD, map hoặc save gốc. Worktree thử dùng chung Git/LFS object với repo local, không tuyên bố clone mạng độc lập hoặc kiểm trên máy thành viên khác.


## Bổ sung kiểm UI/controller — 08/11/2026

**Guard Load và phục hồi đạt trong sandbox; còn giới hạn input và yêu cầu hiển thị lỗi bên dưới.** Không đổi kết quả 42 ca cũ hoặc chạy lại toàn bộ chúng. Commit runtime vẫn `546833aa`; bắt đầu phiên từ `57dcbb03`, công cụ controller được commit tại `bb88d4cc97ad78427320d03a64f0f49f4050857b`. Hash ba file trong `tested-source.json` đối chiếu cuối phiên vẫn khớp. Xem [kết quả controller](controller-results.json) và [manifest local](controller-evidence-manifest.json).

### Phương pháp và phạm vi thật

- GUI Unity **6000.6.2f1**, scene Gameplay_ThuNghiem thuộc Gameplay & Dữ liệu. Công cụ `SaveGuardControllerChecks.StartGui` mở template, tạo floor/proxy targets/controller/camera/session và gate tạm **trong Play**; không lưu scene, không mở/sửa scene tích hợp. Gate sandbox không chạy physics; đây không phải hồi quy cơ cấu động.
- Controller và IMGUI của game chạy thật. Tiến trình được tạo bằng phím/chuột **tổng hợp qua Windows**, có scan code, kiểm PID/foreground và nhật ký `os-input.jsonl`. Không gọi Acquire/Observe/Save/Load/OpenPage để tạo hoặc thao tác tiến trình. Bridge chỉ dựng môi trường, đọc state/chụp Game view và điều khiển vòng đời Editor; không seed chứng cứ.
- Profile mới: `Test/PlaytestProfiles/SaveController_20261108`. Intro rỗng → nhấp Bắt đầu → ngắm proxy → E → nhấp Ghi quan sát → R/P. Proxy chỉ dùng kiểm tương tác; hình xem là cube, không chứng minh đọc hình vật chứng/art thật hoặc tìm đường xưởng tự nhiên. Người mới **NOT_RUN**.
- Game capture khi kiểm các lỗi là **1536×746**, không tuyên bố đã kiểm lại UI 1080p. Hình desktop có thể lẫn cửa sổ khác chỉ giữ local; bằng chứng giao nhóm dùng ảnh Game view `*-final-*.png` trong manifest.

### Kết quả thao tác

| Ca | Kết quả thực tế |
|---|---|
| Tiến trình bằng chơi | E thu slide với observations rỗng; nút Ghi quan sát ghi [0,1]; R và P xem lại, W khi khám không di chuyển |
| Save A → đổi tiến trình B → Load A | F5 lưu A có một vật; D di chuyển tới shoeprint, E và nút ghi tạo B có hai vật; F6 trở về một vật và vị trí A |
| Tiếp tục sau tải | Di chuyển/E/ghi lại vật thứ hai; F5/F6 lưu và tải B thành công |
| Quan sát ngoài phạm vi | Bản sao A có observations=[999]; F6 từ world từ chối; giữ nguyên 13 nhóm state so sánh và file input |
| Vật chứng trùng | Bản sao A thêm record slide trùng; F6 từ hồ sơ từ chối; state/identity/transform/gate/modal/input flags và file input giữ nguyên |
| Cấu trúc lỗi | Bản sao JSON bị cắt giữa mảng evidence; F6 từ pause từ chối, giữ cả pause/timeScale cùng state và file input. Không suy rộng thành kiểm UI mọi dạng null/schema |
| Sau từng lần từ chối | P mở/đóng hồ sơ; E mở khám đúng vật, E/W trong modal không thoát hoặc di chuyển; Esc đóng khám rồi pause/resume, W bị chặn khi pause; S/W di chuyển, chuột đổi nhìn; tải A hợp lệ rồi B hợp lệ, tiếp tục F5/F6 |

Bản sao lỗi có bytes/SHA256 trong manifest; đối chiếu trước/sau F6 byte-for-byte, không bị Load ghi đè. Trạng thái so sánh gồm toàn bộ data và identity, position/yaw/pitch, gate, page/pause/modal, movement/look/actions và timeScale. Sau mỗi ca, **chủ động** thay riêng file profile thử bằng bản hợp lệ đã tạo qua chơi để F6 phục hồi; đây là bước của bài kiểm, không phải runtime tự sửa file lỗi.

### Các lần kiểm chưa xác định nguồn input

Giữ nguyên bằng chứng các lần script dừng; không biến chúng thành PASS hoặc quy lỗi gameplay:

1. Chuẩn bị Editor có lỗi command/lịch import của công cụ kiểm, focus nhầm main window thay vì Game tách riêng và scan code thiếu; đã dừng trước khi coi là kiểm sản phẩm. Log cũ còn trong manifest. Không sửa runtime để bù.
2. Lần world rejection đầu có yaw/pitch đổi khoảng 1 độ, các trường data/identity/gate/vị trí/modal giữ nguyên; chưa xác định input gây đổi góc. Một lần sau vị trí đã khác precondition cố định nên E không mở vật. Giữ snapshots `attempt-focus-*` và các ca chưa đủ bước. Sau ổn định focus, giải phóng phím điều hướng thử và tải lại save B bằng F6, lần kiểm cuối giữ toàn bộ các trường trước/sau tải.
3. Trong ca JSON lỗi, một assert đóng hồ sơ bằng P chưa chuyển như script dự kiến. Không có bằng chứng xác định focus, timing hoặc runtime là nguyên nhân. Lần kiểm lại với quan sát dài hơn đóng được và lặp **ba chu kỳ P mở/đóng** đạt; không sửa NAV-01. Nếu UI/UX & Tích hợp/người chơi tái hiện lại, cần ghi input/focus và trạng thái tại lúc xảy ra.

### Yêu cầu UI-LOAD-FEEDBACK-01 — OPEN, gửi UI/UX & Tích hợp

- Tái hiện: mở hồ sơ (hoặc pause), đặt bản sao save lỗi ở profile thử, bấm F6. Session Message có `Không tải: Vật chứng trong hồ sơ không hợp lệ hoặc bị lặp.` hoặc `Không tải: JSON parse error: Invalid value.`, nhưng ảnh modal `duplicate-evidence-final-rejected.png` / `broken-json-final-rejected.png` không có thông báo này.
- Nguyên nhân hiển thị đã đối chiếu: `InvestigationSessionUI.OnGUI()` vẽ Message ở nhánh `!Modal`; diagnostics mặc định tắt. Không phải guard mất lý do hoặc UI kẹt. Thông báo world về index đọc được; lỗi parser còn thuật ngữ tiếng Anh.
- Đề nghị: UI/UX & Tích hợp review cách hiện lỗi tải ngay tại hồ sơ/pause, giữ rõ việc file và phiên hiện tại không bị thay; thống nhất bản vá với Gameplay & Dữ liệu nếu cần chạm session partial. **Chưa sửa file UI hoặc đổi ownership.** Acceptance đề nghị: F6 sai trong cả ba ngữ cảnh có lý do đọc được, không tự đóng trang/ghi lại save, vẫn tải hợp lệ được.

### Bàn giao và chạy lại có chủ đích

Không cần thay runtime để tích hợp phần kiểm này. UI/UX & Tích hợp nhận bản guard `546833aa` và báo cáo; công cụ controller/meta tại `bb88d4cc97ad78427320d03a64f0f49f4050857b` là source test riêng, không cần gắn component vào scene hay copy scene. Gói hiện tại không chốt tuyến tự nhiên, hình/art, mọi partial save khác hoặc rollback lỗi reference sau khi guard đã nhận dữ liệu.

Muốn kiểm lại: dùng checkout thử không có Editor khác đang ghi, profile **mới chưa tồn tại**, output riêng và phiên desktop được dành cho kiểm. Gọi Editor GUI với `-executeMethod ConRoi.Gameplay.EditorChecks.SaveGuardControllerChecks.StartGui --investigation-save-dir <profile-tuyệt-đối-mới> --save-controller-output <output-tuyệt-đối>`. Công cụ không tự thực hiện thao tác chơi. Tự dùng controller/UI theo bảng; không chạy script input local với PID/profile của phiên cũ. Capture/snapshot/stop/quit qua `command.json` trong output là lệnh công cụ Editor, không phải API gameplay. Giữ trạng thái trước stop/quit; không lưu thay đổi runtime vào scene.

Cuối phiên đã snapshot state/diff trước stop, scene dirty=false, thoát đúng Editor do tác vụ mở. ProjectAuditorSettings tự sinh và hai trạng thái line-ending EditorBuildSettings/QualitySettings sau quit đã được copy/hash vào output ImportedChanges rồi khôi phục riêng; không commit settings/cache/profile/capture. Dữ liệu gốc, legacy và thay đổi ngoài phạm vi giữ nguyên. Không push/merge/build/ZIP.

Compile công cụ controller đạt (Tundra build success trong log); không có error CS, NullReferenceException hoặc MissingReferenceException trong log phiên này. Template và fixture không có bằng chứng thay thế việc audit reference scene tích hợp.
