# SAVE-EVIDENCE-04 — hợp đồng collection khi Load

Gameplay & Dữ liệu, 09/11/2026. Nhận từ91835c80; đọc báo cáo UI/UX & Tích hợp tại e30699629 trong checkout C, không merge nhánh review. Tracked sạch trước thao tác, untracked cũ giữ nguyên.

## Acceptance ghi trước sửa

1. Trên scene Gameplay/profile mới, tái hiện evidence:null làm mất evidence ở baseline qua Load; lưu state/identity/bytes. Phân biệt API/fixture với F5/F6 Windows tổng hợp.
2. Xác minh writer/schema1 và fixture baseline/cũ: evidence:null, thiếu trường, evidence:[] khác nhau. New Game [] phải tải được. Chỉ chốt chính sách missing sau kiểm tương thích, không tự repair dữ liệu.
3. Guard tại Load trước thay state; dữ liệu lỗi không đổi data/identity/controller/world/gate/page/pause/timeline hoặc bytes. Lỗi trả qua Message hiện có. Sau từ chối vẫn tải/lưu hợp lệ được.
4. API kiểm null, missing, [], hợp lệ, phần tử null; rà các collection cùng đường deserialize trực tiếp để tránh null bị chuẩn hóa trước typed guard. Không refactor toàn validator hoặc thêm luật nội dung.
5. Hồi quy index, duplicate, relation guard và tương thích schema1 có căn cứ; không chạy IO/trial. UI/controller profile mới: F5 → lỗi F6 → giữ tiến trình → P/E/Esc/modal/di chuyển → tải hợp lệ → tiếp tục F5/F6.
6. Không sửa UI, map, scene tích hợp, schema/ID/GUID hoặc save gốc. Load feedback/P-close OPEN riêng; scene chung/UI1080p/người mới không được suy PASS từ sandbox.

Kết quả cuối: nguồn và kiểm Gameplay đạt phạm vi dưới đây; scene chung chờ UI/UX & Tích hợp kiểm lại. Snapshot/output/profile/log ở Test/Output/SaveEvidence04 và Test/PlaytestProfiles/SaveEvidence04_* ngoài Git.

## Baseline và quy tắc đã xác minh trước sửa

- Baseline API:74 assertions,14 lỗi acceptance do JsonUtility nhận null/missing/sai kiểu hoặc tạo default record/index. Giữ [kết quả gốc](baseline-api-results.json).
- Baseline UI/controller Windows tổng hợp: Bắt đầu → E thu slide → F5 → sửa bản sao evidence:null → F6; evidence1→0, identity/state đổi, bytes input không đổi; [kết quả](baseline-ui-results.json). Sau đó restore file hợp lệ, F6, stop/quit Edit sạch. Không cấp evidence API cho lượt UI.
- [Đối chiếu schema1](schema1-compatibility.json): writer ở baseline chung đã luôn phát evidence array; fixture schema1 cũ có evidence/mapping/relations/trials/conclusions array và tải/tiếp tục được. Không tìm thấy yêu cầu tương thích cho evidence thiếu hoặc null. Chốt evidence phải có mảng rõ ràng; [] là hợp lệ, thiếu/null/sai kiểu bị từ chối.
- Collection phụ và observations/grounds bị thiếu vẫn theo default hiện có để giữ tương thích; không có migration mới/repair flag. Nhưng nếu trường hiện diện với null/sai kiểu thì từ chối trước deserialize; root null-record vốn bị typed guard cấm phải bị chặn trước chuyển đổi thành object mặc định. Observation null-item không được thành index0. Không chấm nội dung/ID mới hoặc mở rộng sang mọi trường schema.
- Sử dụng Newtonsoft đã khóa3.2.1 và có consumer runtime trong repo; không nâng/thêm package. Từ chối duplicate JSON properties để hai parser không hiểu khác collection; writer hợp lệ không phát duplicate key.

## Bản sửa và chuỗi commit cần nhận

| Commit | Nội dung |
|---|---|
| `91835c80e040d4331b967b6a676016bcd769c2cb` | Tiền đề đã bàn giao; không reset hoặc nhận nhánh review UI vào Gameplay |
| `17b1f6e25ebdc8c15c13395e2f0eac1ce551c664` | Nguồn: guard collection JSON trong InvestigationSaveValidation và lời gọi trước JsonUtility/data=incoming trong Load |
| `f73631ae99ecec5aadbcf31cfa02dcdce10f8b8a` | Công cụ API/meta; **commit thực sự chạy hồi quy54 và GUI sau sửa** |
| Commit tài liệu sau kiểm | Báo cáo/contract/tiến độ; lấy SHA từ Git log khi nhận, không gọi là nguồn đã chạy lại |

[Hash nguồn đã kiểm](tested-source.json). Bộ78 chạy sau sửa trước commit; source/tool bytes không đổi so với các commit trên và được đối chiếu cuối lượt. Nguồn chỉ đổi hai file `Assets/Scripts/InvestigationV36R107/InvestigationSaveValidation.cs` và `InvestigationSession.cs`; public API/field/schema1/ID/GUID giữ nguyên. Chỉ thêm helper internal đọc token collection bằng Newtonsoft hiện có; typed validator tiếp tục kiểm ID/index/finite như cũ. Không sửa class InvestigationSave, UI/presentation, Save writer hoặc scene.

## Kết quả API/fixture — không phải tuyến chơi tự nhiên

| Lượt | Kết quả | Phạm vi |
|---|---|---|
| Baseline |74 assertions,14 không đạt acceptance | Cùng nguyên nhân null/default/coercion, không gọi là14 lỗi độc lập. Chưa có hai mẫu duplicate-key được bổ sung sau baseline; giữ file baseline nguyên trạng |
| Sau sửa |**78/78 đạt** — [kết quả](fixed-api-results.json) |35 mẫu file × kiểm Load + phục hồi/tiếp tục Save/Load, cùng New Game/profile/fixture cũ. Null/missing/[]/valid/null-record/object/string; observations null/null-item; các list cùng luồng null/thiếu/rỗng/null-record; grounds; hai thứ tự duplicate evidence key; index/ID/duplicate/relation guard |
| Hồi quy chịu ảnh hưởng |**54/54 đạt** — [kết quả](regression-results.json) | Dùng SaveLoadContractChecks hiện có và **đúng fixture baseline**; typed validation, file Load, pause/modal, recovery và tương thích cũ. Không chạy bộ IO/trial |

Các ca từ chối so data đầy đủ và identity, transform/rotation người chơi/eye, gate/modelGate, Page/Paused/Modal, enabled controller/movement/look, timeScale và transient trial tại thời điểm kiểm; bytes input không đổi. Message lỗi là kênh được phép thay đổi. Mỗi mẫu khôi phục file hợp lệ rồi tiếp tục Observe/Save/Load. Fixture cấp slide/shoeprint/relation bằng API được ghi đúng là fixture; không coi đó là đã đi tuyến hiện trường.

Schema1 cũ đã kiểm: baseline fixture từ SaveContractBaselineRetry_20261108, có hai evidence chỉ observation0, mapping1/relation1; nguồn fixture chỉ đọc và hash không đổi. New Game do Save tạo có evidence:[], Load được cả trước và sau một phiên có tiến trình. Giữ default vắng collection phụ/observations/grounds như trước; không có migration mới hoặc tự sửa cờ tiến trình. Không suy tương thích mọi file từng tồn tại khi chưa có mẫu cụ thể.

## UI/controller — input Windows tổng hợp, scene Gameplay

[Chi tiết](ui-results.json). Unity6000.6.2f1, HEAD f73631ae; scene `Assets/ConRoi/Modules/Gameplay/Scenes/Gameplay_ThuNghiem.unity` là template rỗng. Công cụ SaveGuardControllerChecks hiện có chỉ dựng floor/proxy targets/controller/session/gate tạm trong Play, đọc state/chụp ảnh/stop/quit. Không lưu scene, không cấp evidence/observation, không gọi Save/Load/OpenPage bằng API trong lượt UI. Gate sandbox tĩnh; đây không phải kiểm cơ cấu động hoặc scene tích hợp.

1. Bắt đầu từ intro/hồ sơ rỗng → F5 save New Game → E thu slide → click Ghi quan sát → P → F5. Tiến trình tạo bằng UI: slide observations[0,1], firstFloorRecorded=true, firstNotesReviewed=true.
2. Trên bản sao save đó, lần lượt evidence:null, thiếu evidence, evidence:[null], F6 từ dossier: **cả3 bị từ chối**, Message có lý do tương ứng. Full data/identity, state/controller/gate/page/pause/transform và bytes giữ nguyên. File mẫu hỏng lưu riêng/hash trong manifest; không F5 ghi đè file lỗi.
3. Sau **mỗi** mẫu: P đóng → E khám → R → Esc pause → W không di chuyển, movement/look disabled → Esc resume → W đi được → restore bytes save hợp lệ → F6 → F5/F6 tiếp. Cả3 giữ observations và hai cờ tiến trình đúng, input không kẹt. Đây là các kiểm navigation mới của gói, không đóng bất thường P-close lịch sử.
4. F6 với chính save New Game [] tạo bằng UI: nhận hợp lệ, evidence rỗng và hai cờ đầu false đúng file; sau đó F6 save có tiến trình phục hồi được. Ghi lặp không nhân đôi; đi tới vị trí mới → F5 → đi chỗ khác → F6 phục hồi vị trí/tiến trình, tiếp tục chơi được.

### Gián đoạn được giữ riêng

- [Lượt đầu](ui-focus-interruption.json): helper chưa lấy được foreground PID11924, dừng trước key/click; profile vẫn intro/evidence0. Lượt retry tạo Windows message queue và kiểm đúng cửa sổ/PID rồi mới gửi input; không thay runtime hoặc sửa kết quả lượt đầu thành PASS.
- [Lượt phụ cuối](ui-extra-probe-interruption.json): sau khi đã hoàn tất các ca bắt buộc và F5/F6 tiếp tục, foreground đổi sang Terminal PID7904 trước W/chuột trong dossier. Helper dừng trước input; probe bổ sung **NOT_RUN**, không nhận PASS/FAIL gameplay. Data/identity tại snapshot không đổi kể từ trước probe; chưa xác định nguồn chuyển focus là tool hay người. Ba ca pause/W đã hoàn tất trước đó giữ kết quả riêng. Không gửi thêm input sau lượt này; chỉ snapshot rồi stop/quit phiên do mình tạo.

Không dùng các lượt gián đoạn để tuyên bố mọi thao tác chuột/modal đã kiểm. Game capture thực1536×746, **UI1080p NOT_RUN** trong gói; ảnh desktop1920×1080 không thay render. **Người mới NOT_RUN**, không có playtest người thật. **Scene chung NOT_RUN với bản sửa này**; kết quả RUNTIME-INTEGRATION-01 trước đó là baseline còn lỗi, không tự đổi thành PASS.

## Bàn giao UI/UX & Tích hợp

Nhận đủ `17b1f6e2 → f73631ae → commit tài liệu` trên tiền đề91835c80; không merge toàn nhánh review/runtime-integration-20261109 vào Gameplay và không chỉ lấy báo cáo. Không cần wiring mới: các caller UI/F6 hiện có tự qua guard mới; lý do trả qua Load/Message cũ, không thêm validator UI.

Kiểm lại INT-SAVE-NULL-LIST-01 trong ConRoi_Chinh với profile riêng: thu/ghi bằng controller → F5 → giữ backup → bản sao evidence:null → F6 phải trả lỗi, giữ toàn data/identity/tiến trình/controller/modal và bytes → P/E/Esc/di chuyển → restore save hợp lệ → F6 → chơi/F5/F6 tiếp. Kiểm riêng thiếu field bị từ chối, [] hợp lệ, null-record; không dùng save gốc. Tách kết quả scene chung, UI1080p và người mới. Nếu có file schema1 cũ thiếu evidence thực sự cần hỗ trợ, giữ mẫu/hash và thống nhất tương thích trước thay luật; chưa có normalization như vậy.

**UI-LOAD-FEEDBACK-01 và P-close vẫn OPEN riêng.** Message đã chứa lý do nhưng modal diagnostics=false chưa hiển thị; không sửa session UI/presentation trong task này. Không đóng hai mục đó bằng kết quả guard hoặc các chu kỳ P đạt. Không suy khả năng rollback binding sau nhận dữ liệu hoặc khả năng chịu mất điện/crash từ gói này.

## Chạy lại có kiểm soát

Công cụ mới: `ConRoi.Gameplay.EditorChecks.SaveEvidenceCollectionChecks.RunBatch`; chỉ chạy khi gọi tường minh, không gắn vào game. Dùng Unity6000.6.2f1 batch, project/Library riêng không có Editor khác đang ghi. `--investigation-save-dir` phải là thư mục tuyệt đối chưa tồn tại; output riêng. Không dùng save người chơi làm fixture.

```powershell
& '<Unity6000.6.2f1>/Editor/Unity.exe' -batchmode -nographics `
  -projectPath '<clone-gameplay>' `
  -executeMethod ConRoi.Gameplay.EditorChecks.SaveEvidenceCollectionChecks.RunBatch `
  -logFile '<output-moi>/Editor.log' `
  --investigation-save-dir '<profile-moi-chua-ton-tai>' `
  --evidence-check-output '<output-moi>'
```

Không tham số tương thích:74 assertions sau sửa; muốn kiểm đủ78 của lượt bàn giao, thêm `--evidence-check-compat '<fixture-schema1-baseline>'` đúng hash trong [schema1-compatibility.json](schema1-compatibility.json), yêu cầu nguồn fixture theo manifest nếu clone mới chưa có. Không copy bù asset/runtime từ archive để chạy project; fixture chỉ dùng kiểm tương thích. GUI dùng SaveGuardControllerChecks.StartGui với profile/output mới và --save-controller-output; chỉ thao tác khi phiên desktop được kiểm soát, dừng khi focus/PID không đúng.

## Bảo toàn và giới hạn bàn giao

[Biên nhận](preservation-results.json), [manifest local](evidence-manifest.json). Snapshot trước/sau/status/diff, tất cả save thử, bản lỗi, screenshot, input/log dài nằm trong Test/Output/SaveEvidence04 và Test/PlaytestProfiles/SaveEvidence04_* ngoài Git. Manifest chỉ index/bytes/SHA256; không stage raw save/cache/capture/log. Old fixture hash và các file nguồn đã kiểm khớp cuối lượt.

Editor đã stop/quit bình thường, Edit/sceneDirty=false, không lưu scene/runtime. Ba settings tự sinh được copy sau kiểm và chỉ trả bytes trước tác vụ khi normalized text khớp; không discard nội dung semantic khác. Untracked cũ giữ nguyên. Source/scene/meta checkpoint/GDD/map/save gốc và4.183 legacy chưa xác minh không dọn/đổi. Không push/merge/build/ZIP hoặc chốt baseline nhóm mới.
