# SAVE-VALIDATION-01 — Gameplay & Dữ liệu

Ngày kiểm: 08/11/2026. Nhánh `work/runtime-contracts-baseline`.
Commit xuất phát: `0d878e3b7163c4d50e3976cd5c46118403b19240`.
Commit nguồn bàn giao: `546833aace5f59c0c98349ebe0fb7cd33cbe5df0`. Hash nguồn đã chạy kiểm: [tested-source.json](tested-source.json).

## Lỗi đã tái hiện và sửa

`Observe("slide",999)` bị API hiện hành từ chối, nhưng `Load()` trên baseline nhận cùng chỉ số lỗi từ JSON. Save có hai record cùng ID cũng được nhận và thay state đang chơi. [Baseline](baseline-results.json): 16 ca, 2 lỗi; giữ nguyên kết quả FAIL này.

Thay hai điều kiện kiểm cũ trong `InvestigationSession.Load()` bằng `InvestigationSaveValidation.TryValidate(incoming,out error)` trước `data=incoming`. Lớp mới chỉ đọc dữ liệu, chặn schema/namespace sai, giá trị vị trí/góc/cơ cấu không hữu hạn, cấu trúc null, ID lạ/trùng và chỉ số quan sát ngoài definition. Không cấp chứng cứ, sửa schema1, ID, luật cơ cấu hay NAV-01. Không thay serialized field hoặc GUID hiện có.

Không phải bộ chấm nội dung/suy luận. Đảm bảo giữ state khi **validation từ chối**; chưa có rollback tổng quát cho lỗi binding runtime xảy ra sau khi validation đã đạt.

## Kiểm thực tế

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
