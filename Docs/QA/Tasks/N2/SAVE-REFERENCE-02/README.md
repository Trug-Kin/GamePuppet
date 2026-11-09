# SAVE-REFERENCE-02 — guard đã triển khai, audit gốc được giữ lại

Kết quả triển khai mới nằm ở mục cuối. Phần audit ban đầu bên dưới ghi trạng thái trước khi được giao sửa, không phải trạng thái hiện tại.

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


## Triển khai được cho phép — từ audit 5b88f245

Commit nguồn: **aee7d5592b082a20dc0c849160cd4c4120680011**. Hai file nguồn thay đổi: `Assets/Scripts/InvestigationV36R107/InvestigationSaveValidation.cs` và `Assets/ConRoi/Modules/Gameplay/Editor/SaveLoadContractChecks.cs`. Luồng Load/session, UI, schema, ID, GUID và scene không đổi. Kết quả audit gốc phía trên được giữ nguyên; phần này thay trạng thái “chờ giao triển khai”.

### Quy tắc tham chiếu đã xác minh và guard thực tế

`EvidenceRelation` hiện hành chỉ có `first`, `second`, `note`. Registry trỏ `InvestigationEvidence.cs` làm nguồn ID; mỗi đầu tham chiếu đã lưu phải có definition trong `InvestigationContent`. Không có relation loại khác hoặc một đầu tùy chọn trong cấu trúc này. `relations=[]` hợp lệ. `note` là chuỗi trình bày, không phải ID; null/rỗng không làm lookup thiếu definition và vẫn được Load nhận. API `AddRelation` của thao tác mới vẫn giữ yêu cầu note không trắng và hai mục đã thu; không sửa luật API này.

Guard thêm đúng hai lookup definition trong vòng lặp relation sau kiểm null record; first/second lạ/rỗng/null bị từ chối qua TryValidate hiện có trước `data=incoming`. Lý do qua cơ chế Message hiện có: “Không tải: Liên hệ trong hồ sơ tham chiếu vật chứng không tồn tại.” Không thêm event/API lỗi mới hoặc validation thứ hai trong UI.

Không áp thêm quy tắc hai đầu phải khác nhau, phải nằm trong evidence đã thu, note phải chắc chắn hoặc phải kết luận đúng. Các mẫu có definition nhưng chưa thu/cùng ID vẫn đọc được để không siết schema ngoài gói. `hook` được tra từ source definition thật dù registry văn bản chưa liệt kê; không thay ID hoặc sửa thay registry.

### Kiểm fixture/API và hồi quy chịu ảnh hưởng

Unity6000.6.2f1, Gameplay_ThuNghiem, profile mới `Test/PlaytestProfiles/SaveReference02_API_20261108`. [Kết quả](implemented-api-results.json): **54 ca, 0 lỗi**; đối chiếu tên cho thấy toàn bộ42 ca SAVE-VALIDATION-01 trước đó vẫn có và đạt. Chạy lại có căn cứ vì thay cùng validator.

12 ca mới gồm sáu Load từ JSON với first/second lạ/rỗng/null và sáu Load/Save/Load hợp lệ: không relation, note chưa chắc, note rỗng, note null, ID definition chưa thu, hai đầu cùng ID. Helper từ chối kiểm data identity/toàn data, position/yaw/pitch/gate/page/pause/movement/look/actions/cursor/timeScale cùng bytes input và lý do lỗi; sau mỗi ca phục hồi file hợp lệ và Load được. Save thật từ baseline SAVE-VALIDATION-01 được đọc rồi tiếp tục Observe/Save/Load; source baseline không đổi. Đây là fixture/API có ghi dữ liệu thử, không phải tuyến tự nhiên.

### Kiểm Load/UI qua profile riêng

[Chi tiết UI](implemented-ui-results.json). GUI Editor chạy đúng commit nguồn, công cụ sandbox hiện có dựng môi trường tạm trong Play, controller và IMGUI thật. Không lưu scene. Tiến trình được tạo bằng input Windows tổng hợp: Bắt đầu → E/Ghi quan sát slide → D/E/Ghi quan sát shoeprint → P/Đối chiếu → chọn hai mục → nhập “can kiem them” → Ghi liên hệ → nút Trở lại → P → F5/F6. Không seed tiến trình hoặc mở trang bằng API.

Bản hợp lệ `valid-ui.json` xuất phát từ thao tác trên. Mỗi mẫu lỗi chỉ sửa **bản sao file profile thử**, không sửa state đang chơi:

| Mẫu/ngữ cảnh | Guard qua F6 | Kiểm tiếp và phục hồi |
|---|---|---|
| ID lạ ở first / world | Từ chối, mọi trường state so sánh và bytes file giữ nguyên | Ba chu kỳ P, lookup hồ sơ, E/R/Esc, W bị chặn trong modal, di chuyển/chuột, F6 hợp lệ và F5/F6 tiếp đạt |
| ID lạ ở second / dossier | Như trên; hồ sơ tiếp tục dùng relation hợp lệ đang chơi | Cùng các kiểm điều khiển/phục hồi trên đạt |
| first=null / pause | Từ chối, giữ cả pause/timeScale và bytes file | Phần điều khiển tiếp bị gián đoạn focus, chưa gộp thành PASS trọn ca; guard API null và phục hồi đã đạt riêng trong54 ca |

Không có NullReferenceException/MissingReferenceException trong log GUI của đợt kiểm. Sau từ chối unknown endpoints, mở hồ sơ hiển thị title hai vật và note cũ, không đưa relation lỗi vào phiên. Các giá trị rỗng/null đủ sáu tổ hợp được kiểm Load/API; không tuyên bố cả sáu đều chạy UI.

Một snapshot ngay sau nút Ghi liên hệ chưa có record nhưng readback tiếp có record và UI báo đã ghi; giữ cả hai thay vì gọi đó là lỗi gameplay. Ở ca null-first sau pause, P có lần không chuyển trong timeout. Khi kiểm lại, P mở hồ sơ được, nhưng trước phím tiếp theo script xác nhận foreground đổi từ Unity PID380 sang browser PID26204 và dừng gửi input. Focus mất đã xác nhận cho **lần kiểm lại này**; chưa đủ căn cứ gán cùng nguyên nhân cho lần P trước hoặc đóng điểm P-close cũ. UI-LOAD-FEEDBACK-01 vẫn OPEN: Message có lý do nhưng modal không hiện; không sửa UI ở gói này. 1080p và người mới NOT_RUN; Game captures thực1536×746.

### Đánh giá lookup từ luồng khác

Rà writer của dữ liệu r107: thao tác sản phẩm đi qua AddRelation, chỉ nhận ID đã có; Acquire chỉ nhận definition biết trước. Load được guard bảo vệ trước nhận data. Các ghi trực tiếp vào relations tìm được trong Gameplay là fixture/validator/audit, không phải producer gameplay mới. Chưa xác minh một luồng sản phẩm khác tạo relation thiếu đầu tham chiếu ngoài Load.

`session.data` vẫn public/mutable: caller tương lai hoặc tool có thể bỏ qua AddRelation và làm data thiếu. Không biến data thành private, không refactor toàn hệ thống hoặc thêm null-check che lỗi ở DossierUI trong gói này. Contracts yêu cầu producer mới giữ invariant definition tồn tại, hoặc gửi task biên dữ liệu riêng trước sử dụng. Nếu UI/UX & Tích hợp muốn fallback khi dữ liệu bị sửa ngoài contract, review riêng cách hiện lỗi có nguồn; không thay guard bằng việc bỏ record âm thầm. Không có thay đổi UI để nhận trong commit nguồn.

### Nhận gói

Nhận chuỗi từ audit5b88f245 qua nguồn aee7d559 rồi commit báo cáo của đợt này; không chỉ nhận tài liệu hoặc tool. Các caller Load/F6 hiện có tự dùng guard, không wiring mới/migration. Không tuyên bố đã sửa toàn bộ dữ liệu save hoặc rollback exception binding sau validation. Các điểm OPEN SAVE-VALIDATION-01 giữ nguyên.


### Chốt kiểm và bảo toàn

Commit kiểm byte chính xác: **ea49025a** (chỉ đổi helper test). Kiểm cuối chạy đúng commit này, Unity6000.6.2f1/profile `SaveReference02_API_Final_20261108`: **54/54 đạt**. Guard hash không đổi so với commit aee7d559 đã chạy GUI; không chạy lại UI chỉ vì thay helper test. Trong lần cuối helper đọc bytes ngay trước/sau Load và so byte chính xác, bổ sung kiểm cursor/timeScale/actions; không chỉ so chuỗi JSON. Hai ca GUI unknown endpoints và bước từ chối null-first cũng so bytes trực tiếp.

[Manifest triển khai](implementation-evidence-manifest.json) ghi hash source, log, snapshot/capture và fixture/profile tương ứng; kết quả gốc của các lần đầu được giữ local. Nhật ký xác nhận browser lấy focus được bảo toàn, không gửi thêm input sau phát hiện. Không có desktop dành riêng được xác nhận trong thời gian chốt nên dừng phần UI null bổ sung ở giới hạn đã nêu; không suy thành lỗi gameplay hoặc PASS trọn ca.

Đã snapshot state/diff trước stop, sceneDirty=false, thoát đúng Unity phiên thử. Profile UI thử đang giữ bản null-first lỗi đã bị từ chối; bản hợp lệ tạo qua UI nằm ở `Test/Output/SaveReference02/UI_20261108/valid-ui.json`, không ghi đè tự động trong cleanup. Không dùng profile này làm New Game. State/ảnh đã giữ local; save gốc không bị chạm. Các settings tự sinh/line-ending EditorBuildSettings, ProjectAuditorSettings, QualitySettings đã copy/so hash trước khôi phục từng file; không commit chúng hoặc Library/profile/capture.

Chuỗi bàn giao: gói SAVE-VALIDATION-01 đến810168d2 là tiền đề → audit5b88f245 → guard aee7d559 → helper kiểm byte ea49025a → commit báo cáo. Nhận cả chuỗi theo nhánh sau review, không chỉ cherry-pick tài liệu/test vào baseline thiếu lớp validator. Chưa push/merge.


## Bàn giao gộp SAVE-VALIDATION-01 và SAVE-REFERENCE-02

Nhánh `work/runtime-contracts-baseline`; thứ tự phụ thuộc dưới đây đã kiểm bằng Git, các commit 1–9 đều là ancestor của abd0ea7e; commit 10 là tài liệu kế tiếp và là HEAD thực sự của lượt UI bổ sung. Chưa push/merge. Nhận chuỗi sau review từ baseline chung, không chỉ nhận commit báo cáo hoặc hai commit test cuối. Không cần wiring scene mới; Load/F6 hiện có đi qua validator.

| Thứ tự | Commit đầy đủ | Vai trò trong gói |
|---|---|---|
| 1 | `0d878e3b7163c4d50e3976cd5c46118403b19240` | Baseline nhóm, runtime/scene/contracts tiền đề; không reset nhánh có tiến độ mới |
| 2 | `546833aace5f59c0c98349ebe0fb7cd33cbe5df0` | SAVE-VALIDATION-01: guard cấu trúc/observation + fixture/meta ban đầu |
| 3 | `57dcbb038223b7d9b7fa59548f43cd77a1a96a13` | SAVE-VALIDATION-01: contracts/tiến độ và42 ca fixture/API |
| 4 | `bb88d4cc97ad78427320d03a64f0f49f4050857b` | Sandbox Editor controller/UI và meta, không seed tiến trình |
| 5 | `810168d2cc3dea11434b20055226476f19120ae0` | SAVE-VALIDATION-01: kiểm UI/controller và yêu cầu UI còn OPEN |
| 6 | `5b88f2457beaadb404b7a2f6701988ad5f17b43e` | SAVE-REFERENCE-02: audit tái hiện reference thiếu + tool/meta/acceptance |
| 7 | `aee7d5592b082a20dc0c849160cd4c4120680011` | SAVE-REFERENCE-02: guard definition first/second +12 ca acceptance bổ sung |
| 8 | `ea49025a06b9e5497c545aa3193562a1758c13f0` | Helper kiểm bytes chính xác;54 ca gồm42 cũ và12 mới |
| 9 | `abd0ea7ef10ff0ac226eb3f3ed2b29c4456d19ac` | SAVE-REFERENCE-02: kết quả/giới hạn UI, contracts và manifest |
| 10 | `490f7a021f6218308da407f45388a152ab635bb9` | Bàn giao gộp và bảo toàn lượt null gián đoạn; HEAD đã kiểm UI bổ sung, source không đổi |

Bản vá runtime chỉ ở InvestigationSaveValidation.cs (luồng Load trong Session.cs đã dùng từ SAVE-VALIDATION-01). Bộ kiểm tại Gameplay/Editor: SaveLoadContractChecks.cs, SaveGuardControllerChecks.cs, RelationReferenceAudit.cs và meta hiện có. Các source/test commit và docs đều cần được review; không tự cập nhật các nhánh khác hoặc scene tích hợp trong đợt kiểm này.

| Hạng mục độc lập | Trạng thái giữ riêng |
|---|---|
| Guard/API và hồi quy có căn cứ |42 ca SAVE-VALIDATION-01;54 ca sau SAVE-REFERENCE-02 đã đạt theo báo cáo trước, không chạy lại trong đợt chỉ kiểm UI này |
| UI hiển thị lý do Load trong hồ sơ/pause | OPEN, UI-LOAD-FEEDBACK-01; Message đã có, không nhân đôi validation UI |
| P-close sau mật mã/pause | OPEN cho kiểm tích hợp; các chu kỳ lặp đạt không tự xác định nguyên nhân lần bất thường trước |
| UI1080p | NOT_RUN trong các đợt này; Game captures1536×746 không thay thế kiểm1080p |
| Người chơi mới | NOT_RUN; fixture và input tổng hợp không thay người chơi |

Kết quả lượt hoàn tất UI null mới phải đọc cùng các trạng thái này; không dùng một ca guard đạt để đóng tất cả vấn đề. Báo cáo bổ sung sẽ giữ rõ lượt gián đoạn và phần thực sự chạy.


## Lượt hoàn tất UI null từ abd0ea7e — gián đoạn trước kiểm

Checkout đã xác nhận `abd0ea7ef10ff0ac226eb3f3ed2b29c4456d19ac`; hash source guard/công cụ khớp manifest trước. Không sửa runtime và không chạy lại54 ca API. Mở đúng Gameplay_ThuNghiem bằng Unity6000.6.2f1, profile mới `Test/PlaytestProfiles/SaveReference02_NullCompletion_20261108`, output `Test/Output/SaveReference02/NullCompletion_20261108`.

**INTERRUPTED_PRECONDITION_UNKNOWN_INPUT; ca Load null mới NOT_RUN.** Khi script mới lấy focus và chờ intro, state đã rời intro, có shoeprint/slide và quan sát slide, đang ở dossier; chưa có scripted key/click/mouse-relative nào. Nguồn input/thay đổi chưa xác định. Không suy intro tự đổi là lỗi gameplay, không tính kết quả null đã đạt hoặc kết luận FAIL gameplay. Lượt kiểm trước tại abd0ea7e và giới hạn còn thiếu của nó giữ nguyên.

Đã lưu snapshot Game/state, Git status/diff và log copy ở output trước mọi bước tiếp. Chưa gọi F6, chưa ghi file SavePath, chưa dừng Play hoặc đóng Editor có live state chưa rõ nguồn. Phiên PID23308 được để nguyên chờ xác nhận đang có ai dùng và dành riêng desktop; không ghi đè/discard runtime progress. Các trạng thái ProjectSettings tự sinh hiện có cũng giữ nguyên, không khôi phục trong lúc Editor còn chạy. Không lấy thời gian chờ làm câu trả lời cho phép.

[Kết quả lượt gián đoạn](null-completion-results.json) và [manifest snapshot/log copy](null-completion-evidence-manifest.json). Profile dự kiến nhận bản sao save tạo qua UI ở đợt trước, không phải New Game tự nhiên; chưa nạp fixture vì điều kiện trước kiểm không đạt. Người mới/1080p vẫn NOT_RUN, UI-LOAD-FEEDBACK-01 và P-close vẫn OPEN riêng.

Việc còn thiếu không đổi: với phiên được dành cho kiểm, F6 từ chối `first=null` (và kiểm riêng record relation=null nếu dùng mẫu đó), so state/bytes → P/E/Esc/modal/movement → F6 hợp lệ → tiếp tục chơi/F5/F6. Chỉ ghi PASS khi cả đoạn đã thực hiện; các lượt mất focus/tiến trình ngoài script phải được giữ như lượt gián đoạn. Bảng bàn giao gộp ở trên đã hoàn tất độc lập với ca UI đang chờ.


## Hoàn tất lượt null sau xác nhận desktop dành riêng — 08/11/2026

**PASS trọn hai ca UI/controller bổ sung:** `first=null` và phần tử `relations=[null]`. Gameplay & Dữ liệu kiểm trên HEAD **490f7a021f6218308da407f45388a152ab635bb9**, Unity **6000.6.2f1**, scene thử `Assets/ConRoi/Modules/Gameplay/Scenes/Gameplay_ThuNghiem.unity`. Hash sáu source guard/session/UI/definition/harness khớp manifest triển khai. Không sửa runtime, không chạy lại54 ca API.

Người dùng xác nhận không ai dùng desktop/Unity và cho phép lượt kiểm tiếp. Snapshot state/diff được giữ trước input. Profile riêng `Test/PlaytestProfiles/SaveReference02_NullCompletion_20261108`; bản save có sẵn trong profile cũng được copy bảo toàn trước ghi bản thử. Dùng lại **bản sao save hợp lệ tạo bằng UI của lượt trước** (`UI_20261108/valid-ui.json`, SHA256 `0c8cf171c99dc3dda7c3dab0664c43788d1712985f5f4ddafb86c03dbba73813`), sau đó nạp bằng F6. Đây là **fixture hỗ trợ + input Windows tổng hợp**, không phải bắt đầu sạch hoặc tuyến chơi tự nhiên. Fixture nguồn không đổi. Bridge chỉ đọc state/chụp ảnh và quản lý lifecycle; không gọi Load/OpenPage hay cấp tiến trình thay cho input trong ca kiểm này.

Mỗi ca thực hiện: F6 nạp bản hợp lệ → Esc/pause → sửa bản sao JSON trong profile → F6 từ chối → Esc → P mở hồ sơ/W bị chặn/P đóng → E khám/W bị chặn/R trở lại → Esc/pause/W bị chặn/Esc tiếp tục → S/W và chuột → F6 nạp lại bản hợp lệ → E/R, di chuyển → F5 → di chuyển tiếp → F6 phục hồi.

| Kiểm | first=null | relations=[null] |
|---|---|---|
| F6 từ chối có lý do qua Message hiện có | PASS | PASS |
| Toàn data/data identity, transform, gate, page/pause/modal và cờ input/timeScale giữ nguyên | PASS | PASS |
| Bytes save lỗi giữ nguyên trước/sau Load | PASS | PASS |
| P mở/đóng, E/R, Esc pause/resume, di chuyển và chuột | PASS | PASS |
| Không di chuyển khi hồ sơ, khám vật hoặc pause đang mở | PASS | PASS |
| F6 hợp lệ, chơi tiếp và F5/F6 phục hồi vị trí/relations | PASS | PASS |

[Kết quả có cấu trúc](null-authorized-results.json), [manifest ảnh/state/input/log và hash nguồn](null-authorized-evidence-manifest.json). Ảnh hồ sơ sau từ chối vẫn hiển thị title hai vật và note hợp lệ đang chơi. Log phiên thử không có NullReferenceException/MissingReferenceException hoặc error CS. Không có mất focus được phát hiện trong hai ca hoàn tất; mọi bước input kiểm foreground đúng PID phiên Unity thử.

Lượt gián đoạn trước và nguồn chuyển intro chưa xác định được giữ nguyên trong `null-completion-results.json`, manifest/log/snapshot cũ; không sửa kết quả đó thành PASS. Chỉ phần kiểm null mới nêu trên hoàn tất. Sau kiểm đã snapshot state/diff, dừng Play (`sceneDirty=false`), đóng đúng Editor phiên thử, không lưu scene. Ba settings tự sinh (ProjectAuditorSettings, EditorBuildSettings, QualitySettings) chỉ khác line ending được copy/hash rồi khôi phục riêng sau khi Editor thoát. Profile/capture/log dài còn local theo manifest; không stage hoặc tác động save gốc, map, scene tích hợp, legacy hay archive.

**Bàn giao UI/UX & Tích hợp:** nhận cả chuỗi trong bảng bàn giao gộp, cùng commit tài liệu kết quả lượt này (SHA trong biên nhận Git khi nhận). Runtime hai gói đã đủ bằng chứng phạm vi giao; caller Load/F6 hiện có tự đi qua guard, không wiring mới, không migration/schema/ID/GUID mới. Chưa push/merge. Không chỉ cherry-pick commit tài liệu/test cuối vào baseline thiếu validator.

Giữ trạng thái riêng: **UI-LOAD-FEEDBACK-01 OPEN** (cần trình bày Message/lý do Load trong hồ sơ/pause, không nhân đôi validation); **P-close OPEN** cho kiểm tích hợp do lần bất thường trước chưa rõ nguyên nhân; **UI1080p NOT_RUN**, capture thực1536×746; **người mới NOT_RUN**. Hai ca P bổ sung đạt không đóng lỗi P cũ hoặc thay playtest người thật.
