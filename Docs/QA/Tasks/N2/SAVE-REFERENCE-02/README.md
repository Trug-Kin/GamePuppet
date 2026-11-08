# SAVE-REFERENCE-02 — audit hoàn tất, đề xuất chưa triển khai

Gameplay & Dữ liệu, 08/11/2026. Base thực tế `810168d2`, nhánh `work/runtime-contracts-baseline`, worktree `C:/ConRoi_Nguoi2`. Không có nhiệm vụ runtime tiếp theo đã giao trong tiến độ/HANDOFF/hướng dẫn tích hợp đã đọc. Vì vậy gói lần này là **xác minh thiếu sót và đề xuất cụ thể**, chưa sửa gameplay hoặc guard.

## Thiếu sót đã xác minh

`InvestigationSaveValidation.TryValidate()` kiểm các EvidenceRecord có ID đúng, nhưng với `relations` chỉ từ chối phần tử null. Nó chưa xác minh `EvidenceRelation.first/second` là ID definition tồn tại.

`InvestigationSession.AddRelation()` chỉ thêm khi hai ID khác nhau, đều đã có trong phiên và note không trắng. Tuy nhiên dữ liệu JSON không đi qua API này. `InvestigationSessionUI.DossierUI()` dùng `InvestigationContent.Get(rel.first).title` và tương tự cho second. `InvestigationContent.Get()` trả null nếu ID không tồn tại.

Đã kiểm Unity **6000.6.2f1**, mở `Assets/ConRoi/Modules/Gameplay/Scenes/Gameplay_ThuNghiem.unity` ở EditMode batch, compile đạt. Tool ghi mẫu vào **profile fixture mới** `Test/PlaytestProfiles/RelationReferenceAudit_20261108`, serialize rồi đọc/parse JSON thật và gọi validator. Không dùng Play, không gọi Load hoặc UI.

| Mẫu | Validator thực tế | Biểu thức lookup consumer | Kết luận |
|---|---|---|---|
| `empty` — hai evidence nhưng không có relation | Nhận | Không cần lookup | Đối chứng đạt; không phải New Game rỗng |
| `valid_relation` — slide ↔ shoeprint | Nhận | Lấy đủ hai title | Đối chứng đạt; note “chưa đủ chứng minh” không bị chấm đúng/sai |
| `unknown_first` — ID lạ ở first | Nhận | NullReferenceException, được tool bắt | Khoảng hở được tái hiện |
| `unknown_second` — ID lạ ở second | Nhận | NullReferenceException, được tool bắt | Khoảng hở được tái hiện |

Cả bốn object đầu vào không bị validator sửa. [Kết quả gốc](audit-results.json) ghi `gapReproduced=true`, `runtimeFixImplemented=false`. Exit0 nghĩa là audit tái hiện đúng giả thuyết và có đối chứng; **không có nghĩa save lỗi đã được chặn**.

Biểu thức lookup được thực thi trong công cụ, tương ứng với source DossierUI; chưa gọi OnGUI hay trực tiếp quan sát trang game lỗi. Luồng Load đi qua validator trước gán data là bằng chứng đọc source, chưa có ca Load/UI riêng cho reference lỗi trong gói này. Không tuyên bố đã kiểm UI/controller, 1080p, tuyến chơi tự nhiên hoặc người mới.

## Gói sửa đề xuất để review

**Mục tiêu:** từ chối relation có đầu tham chiếu tới definition không tồn tại trước khi Load nhận state. Đây là an toàn dữ liệu ở biên save, không chấm nội dung liên hệ hoặc kết luận điều tra.

- Chủ sửa dự kiến: Gameplay & Dữ liệu. Phạm vi nhỏ dự kiến: `InvestigationSaveValidation.cs` và kiểm hồi quy trong thư mục Gameplay/Editor. Chưa đổi code runtime trong lần audit.
- Không đổi chữ ký Save/Load, schema1/namespace, ID, GUID, luật cơ cấu, note tự do, sự thật GDD hoặc session partial ownership. Không sửa UI thay UI/UX & Tích hợp, không nhân đôi validation ở màn hình.
- Chỉ đề xuất kiểm hai ID đầu relation có definition. Không tự mở rộng sang membership theo tiến trình, relation cùng ID, độ đúng của note, mapping/trial/conclusion hoặc migration; các ràng buộc đó cần task/review riêng nếu có căn cứ.
- UI/UX & Tích hợp review phạm vi contract và khả năng nhận gói. Không cần Map & Asset thay asset/wiring. Việc hiện thông báo Load lỗi trong modal vẫn là UI-LOAD-FEEDBACK-01 riêng, chưa được giải quyết bởi đề xuất này.

### Điều kiện đạt khi nhiệm vụ được nhận triển khai

1. Mẫu hợp lệ schema1 không có relation hoặc có relation hợp lệ (kể cả note chưa chắc/chưa đủ chứng minh) vẫn đọc và tiếp tục lưu/tải được.
2. first/second không tồn tại, rỗng hoặc null bị từ chối có lý do trước gán data; không sửa/loại bỏ âm thầm relation để làm file hợp lệ.
3. Kiểm Load thật bằng fixture riêng: từ chối giữ toàn bộ state/data identity/transform/gate/modal/input flags và bytes file; phục hồi save hợp lệ sau đó. Mẫu và kết quả baseline audit giữ nguyên.
4. Kiểm UI/controller có nhãn input đúng trên scene thử/profile riêng: F6 lỗi → P/E/Esc/di chuyển đúng ngữ cảnh → F6 hợp lệ → tiếp tục chơi/lưu/tải. Không seed fixture rồi gọi là tuyến tự nhiên.
5. Kiểm các ca SAVE-VALIDATION-01 chịu ảnh hưởng khi thay validator; không tự đóng các điểm UI, P-close, 1080p hoặc người mới còn mở. Không mở rộng thành sửa UI.

## Chạy lại audit có chủ đích

Dùng checkout riêng không có Editor khác đang ghi, Unity 6000.6.2f1 và profile mới. Từ root checkout, PowerShell:

```powershell
$editor = 'D:\data_unity\6000.6.2f1\Editor\Unity.exe' # đổi theo cài đặt máy
$project = (Get-Location).Path
$id = [guid]::NewGuid().ToString('N')
$profile = Join-Path $project "Test/PlaytestProfiles/RelationAudit_$id"
$output = Join-Path $project "Test/Output/ContractAudit/$id"
New-Item -ItemType Directory -Path $output | Out-Null
& $editor -batchmode -nographics -projectPath $project -executeMethod ConRoi.Gameplay.EditorChecks.RelationReferenceAudit.RunBatch -logFile (Join-Path $output 'Editor.log') --investigation-save-dir $profile --relation-audit-output $output
```

Tool chỉ audit hiện trạng, không phải bộ acceptance sau sửa. Profile chứa bốn JSON fixture tên riêng, không ghi vào SavePath runtime hoặc profile người dùng. Tool từ chối profile đã tồn tại. Khi guard được sửa sau review, kết quả “gapReproduced” phải thay đổi; khi ấy cần viết bộ acceptance đúng mục trên, không sửa bằng chứng baseline thành PASS sản phẩm.

## Bảo toàn và bàn giao

[Manifest](evidence-manifest.json) ghi hash source đã kiểm cùng log/fixture và snapshot. Báo cáo nhỏ vào Git; log dài, profile và snapshot kỹ thuật giữ local, có thể yêu cầu Gameplay & Dữ liệu cung cấp theo path/hash. Source cần thiết vẫn ở repo, không có dependency runtime vào output/profile.

Hai trạng thái settings tự sinh (EditorBuildSettings/ProjectAuditorSettings, diff không có thay đổi nội dung) được copy/hash vào output ImportedChanges trước khi khôi phục riêng. Scene thử và scene tích hợp không đổi; runtime/guard gốc giữ nguyên hash. Không tác động map, save gốc, archive hoặc legacy chưa xác minh. Hai thư mục untracked có từ trước giữ nguyên, không stage. Không reset/push/merge/build/ZIP.

Điểm dừng: audit có bằng chứng và task nhỏ có acceptance, **chờ review/giao triển khai SAVE-REFERENCE-02**; chưa có bản sửa runtime để tích hợp. Các điểm OPEN của [SAVE-VALIDATION-01](../SAVE-VALIDATION-01/README.md) giữ nguyên.
