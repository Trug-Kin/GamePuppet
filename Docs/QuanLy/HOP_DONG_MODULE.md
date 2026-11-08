# Hợp đồng module hiện hành — baseline fcbb7e19

Chủ file: **người 2**; người 3 review cách dùng UI, người 1 review asset binding. Đây là tài liệu mô tả mã có sẵn, chưa phải interface/assembly mới. Nguồn chuẩn: `Assets/Scripts/InvestigationV36R107/InvestigationSession.cs`, các partial cùng thư mục, `InvestigationEvidence.cs` và GDD v3.6. Không dùng GDD_IMPLEMENTATION_CURRENT v47 làm yêu cầu mới.

| Biên hiện hành | Producer / chủ sửa | Consumer / trách nhiệm |
|---|---|---|
| Asset, collider, rig, material và GUID | Người 1 | Người 3 wire scene; người 2 kiểm contract tương tác, không đổi ID để khớp tên art |
| `Acquire`, `Observe`, `Has`, `Record`, `Observation` | Session / người 2 | UI và interaction đọc trạng thái; acquire không đồng nghĩa đã nhìn thấy mọi quan sát |
| `OpenEvidence`, `OpenPage`, `ClosePage`, `SetPaused`, `Modal` | Session partial / người 2 | UI gửi ý định qua luồng hiện có; đóng trang phải dọn focus/drag và khôi phục input đúng ngữ cảnh, giữ NAV-01 |
| `ReadCipher`, `Assign`, `AddRelation` | Session / người 2 | UI trình bày dữ kiện và suy nghĩ; không tự gán danh tính/hung thủ vì chạy một ca |
| `StartTrial`, `ClearTrialHolds`, `TrialSettled` | Session + TrialTimeline + cơ cấu / người 2 | Người 3 wire trang bàn dựng; sáu ca cùng luật, giữ hồi hữu hạn, hiển thị đúng lực/móc/chèn; seek không cấp quan sát |
| `Save`, `Load`, `SaveDirectoryOverride`, `SavePath` | Session / người 2 | Người 3 chuẩn bị profile thử riêng và kiểm đường dẫn; không tự load profile kỹ thuật |

Save hiện hành schema1, namespace `conroi.investigation.v36.r107`; đường dẫn `<profile>/InvestigationV36/r107/prototype.json`. ID theo mã hiện hành, không đổi serialized field/schema/GUID trong handoff. CLI `--investigation-save-dir` được Awake đọc; đổi CLI cần mở lại Editor của checkout đó. F5/F6 chỉ dùng profile thử đã xác nhận.

Người 1/3 không gọi API seed dữ kiện để chứng minh người chơi tìm thấy chúng. Bằng chứng phải tách UI/input tổng hợp, API/fixture, render minh họa và người mới. Ca đã PASS kỹ thuật xem NavFix20261108; không suy toàn map PASS hoặc người mới PASS.

Nhiệm vụ đầu của người 2 là bổ sung chi tiết type/consumer/acceptance theo source thực tế trước đề xuất thay đổi. Mọi biên còn dùng chung file phải giữ một chủ; chưa tuyên bố ba module độc lập.


## Gameplay & Dữ liệu — contract đối chiếu source, SAVE-VALIDATION-01

Mốc đối chiếu: `0d878e3b7163c4d50e3976cd5c46118403b19240`, Unity 6000.6.2f1. Phần ghi mới dùng tên vai trò Gameplay & Dữ liệu / Map & Asset / UI/UX & Tích hợp. Giữ nguyên tên file/nhánh, namespace và schema. Đây là contract hiện hành; chưa tạo event bus hoặc assembly module độc lập.

### Producer/consumer và tác dụng thực tế

| API/state thật | Producer, dữ liệu vào | Consumer và tác dụng | Điều kiện/giới hạn |
|---|---|---|---|
| `Acquire(string id)` | Interaction/UI → session | Thêm `EvidenceRecord {id,collected=true}` nếu definition có thật và chưa có | Không tự mở observations, không chứng minh người chơi đã đọc vật |
| `Observe(string id,int index)` | Màn khám hoặc fixture được ghi rõ | Thêm chỉ số observation duy nhất vào record đã có | ID phải được thu, index trong definition; clip chỉ ghi sau khi đã có cả exposure trước/trong sự cố |
| `OpenEvidence(string id,Transform visual=null)` | Interaction hoặc hồ sơ | Ghi inspecting/page, tìm visual/inspectionPrefab và tạo viewer | Phải `Has(id)`; Map & Asset giao visual/binding, không thay ID theo tên mesh |
| `OpenPage(string)`, `ClosePage()`, `SetPaused(bool)` | UI/UX & Tích hợp qua session partial | Page/data.page, viewer, modal, focus/drag, movement/look/cursor; pause điều khiển timeScale | Giữ NAV-01; đóng UI dọn focus, khi đang nhập chữ P/R không được dùng như phím world |
| `Interact(InvestigationInteractable)` | Controller/raycast hoặc fixture | Tương tác Hà cấp ha/letter/receipt/draft và visitedHa; table đặt tableDiscovered; dây/lực đổi gate input | Đây là API hiện có; gọi fixture không thay bằng chứng đi tuyến controller |
| `Assign(string pose,string letter)` / `ReadCipher()` | UI mật mã → mapping | Pose thuộc Alphabet, chữ A–Z hoặc rỗng; ReadCipher trả chuỗi với `_` và dấu ngắt từ | Không tự cấp decoded chỉ vì có mapping; `RecordReading()` kiểm bản đọc và letter theo luật hiện có |
| `AddRelation(string first,string second,string note)` | UI đối chiếu | Thêm relation có hai ID đã thu, khác nhau và note không trắng | Ghi suy luận của người chơi, không nâng thành sự thật hay tự chọn hung thủ |
| `StartTrial(string)` / `ClearTrialHolds()` | Bàn dựng → timeline/cơ cấu | Sáu đầu vào release/help/hold/hook/wedge/after, cùng `ReturnGateMechanism.Advance` | Giữ hồi hữu hạn; clear chỉ khi điều kiện cho phép; không sửa luật trong gói này |
| `Save()` | F5 hoặc nút UI | Chụp position/yaw/pitch/gate/page/inspecting và data, ghi `.tmp`, copy sang SavePath | API trả void; kiểm Message và file thực khi test. Không đổi cơ chế ghi file trong gói này |
| `Load()` | F6 hoặc nút UI | Parse save, kiểm hợp lệ trước nhận, khôi phục position/look/gate; reset trial/clip, bỏ pause, khôi phục trang được phép | Trả bool; lỗi validation trước gán phải giữ state phiên đang chơi và giữ nguyên file input. Chưa đảm bảo rollback lỗi binding sau validation. Không migration namespace/schema khác |
| `SaveDirectoryOverride` / `SavePath` | Awake đọc `--investigation-save-dir` | `<profile>/InvestigationV36/r107/prototype.json`, fallback persistentDataPath nếu không có override | Fixture phải dùng profile mới; không dùng profile live hoặc kết quả đã giải |

`Page`, `Paused`, `TrialId/TrialTime`, `ClipTime`, `Modal` có setter private hoặc chỉ đọc; không có event module độc lập đã công bố. UI/UX & Tích hợp không cần thêm handler mới để dùng bản sửa Load. Thay đổi API/event tiếp theo phải được thống nhất và mô tả riêng trước khi caller dùng.

### Dữ liệu thật và ranh giới schema1

- `InvestigationSave`: schema/namespaceId; position/yaw/pitch; inspecting/page; các cờ tiến trình/onboarding/exposure; evidence/mapping/relations/trials/conclusions; gate; finalized. Không thêm field mới vào schema trong gói này.
- `EvidenceRecord`: id, collected, observations (chỉ số 0-based theo `EvidenceDefinition.observations`). Thu vật không đồng nghĩa đã ghi mọi chỉ số. `Observe()` hiện chặn index ngoài khoảng; contract tương tự cần áp dụng ở biên đọc save.
- `CharacterAssignment`: pose/letter. `EvidenceRelation`: first/second/note. `TrialRecord`: id/report/returnSeconds. `ConclusionRecord`: choice/claim/grounds; grounds là **ID vật chứng**, không phải `id:index`.
- Definition source hiện có 20 ID: slide, shoeprint, r03, r12, mechanism, **hook**, fabric, phone, clip, khai, samples, coat, shoe, shoecontrol, ha, letter, receipt, henri, draft, decoded. Registry chung hiện chưa liệt kê hook; đề nghị UI/UX & Tích hợp cập nhật index khi tổng hợp, không đổi code ID hoặc sửa thay registry ở gói này.
- Giữ schema1 và namespace `conroi.investigation.v36.r107`; không đổi chữ ký Save/Load, serialized fields, GUID hoặc sự thật vụ án. Những checkpoint/save namespace khác không được migration tự động.

### Gói kiểm nhỏ đã thực hiện

SAVE-VALIDATION-01 đối chiếu biên Observe/Load và tính toàn vẹn dữ liệu trước gán `data=incoming`. Acceptance: save hợp lệ roundtrip rồi tiếp tục quan sát/lưu/tải; save hỏng phải bị từ chối trước thay data/controller/gate/modal, giữ nguyên file lỗi. Tái hiện baseline và kết quả sau sửa được ghi riêng tại `Docs/QA/Tasks/N2/SAVE-VALIDATION-01/README.md` ; không lấy fixture/API làm PASS tuyến tự nhiên.

Gameplay & Dữ liệu giữ toàn bộ partial; không sửa UI/presentation, scene tích hợp, GDD hoặc lịch sử chung. UI/UX & Tích hợp review contract và nhận gói theo commit, Map & Asset chỉ cần tham gia nếu có thay binding/asset (gói này chưa yêu cầu).

### Kết quả và API bổ sung SAVE-VALIDATION-01

Commit nguồn `546833aace5f59c0c98349ebe0fb7cd33cbe5df0`. `InvestigationSaveValidation.TryValidate(InvestigationSave incoming,out string error)` thuộc namespace `ConRoi.Investigation.V36.R107`: chỉ đọc, true khi cấu trúc hợp lệ trong phạm vi kiểm; false kèm lý do. Không repair/grant evidence, không đánh giá kết luận đúng/sai. Load gọi trước gán state; từ chối không đổi file input hoặc phiên hiện tại. Không phải giao dịch rollback cho exception ở binding sau khi đã nhận dữ liệu.

Baseline tái hiện 2 lỗi/16 ca; sau sửa 42 ca/0 lỗi bằng fixture/API. Save thực từ baseline được tải và tiếp tục lưu/tải; giữ schema1/namespace cũ. Chi tiết, giới hạn và cách chạy: [báo cáo](../QA/Tasks/N2/SAVE-VALIDATION-01/README.md). Gói độc lập đã kiểm; review UI/UX & Tích hợp và tuyến controller/UI còn chờ, không tuyên bố playtest người mới hoặc tuyến tự nhiên PASS.


### SAVE-REFERENCE-02 — đề xuất từ audit, chưa đổi contract runtime

Audit trên base810168d2 xác minh validator hiện hành nhận relation có first/second không tồn tại, dù lookup definition `.title` tương ứng phát sinh NullReferenceException. Hai mẫu đối chứng không có relation/relation hợp lệ đạt. Đây là kiểm serialize/parse + validator/lookup API, không phải Load/UI đã chạy. [Bằng chứng, phạm vi và acceptance đề xuất](../QA/Tasks/N2/SAVE-REFERENCE-02/README.md).

Đề xuất Gameplay & Dữ liệu bổ sung guard definition tồn tại cho hai đầu relation trước nhận save, giữ schema1/namespace/ID và note tự do. Chờ UI/UX & Tích hợp review/giao thực hiện; chưa có API hoặc hành vi runtime mới. Không tự thêm luật membership/đánh giá suy luận, không thay phần hiển thị lỗi UI-LOAD-FEEDBACK-01; toàn bộ session partial tiếp tục cùng chủ Gameplay & Dữ liệu.


### SAVE-REFERENCE-02 — contract Load đã triển khai tại aee7d559

Theo yêu cầu triển khai sau audit5b88f245, TryValidate nay yêu cầu **cả first và second của mỗi EvidenceRelation có definition trong InvestigationContent**. Rỗng/null/ID lạ không có definition nên bị từ chối trước gán state. List relations rỗng hợp lệ; note không là reference, Load vẫn nhận null/rỗng. Không thêm luật membership, hai ID phải khác hoặc chấm nội dung. AddRelation của thao tác mới không đổi điều kiện; schema1/namespace/ID/GUID và chữ ký API không đổi. Message trả lý do theo cơ chế hiện hành, không sự kiện/UI validation mới.

Runtime producer hiện có AddRelation/Acquire giữ invariant; Load được guard. data vẫn public/mutable, producer/tool mới không được tự đưa đầu relation thiếu definition vào phiên rồi dựa UI null-check. Chưa phát hiện producer sản phẩm khác cần vá trong phạm vi rà, không đổi UI. [Kết quả và giới hạn UI/API](../QA/Tasks/N2/SAVE-REFERENCE-02/README.md). Phần đề xuất/audit ở trên giữ để tra lịch sử, đã được yêu cầu triển khai này thay trạng thái.


### SAVE-WRITE-03 — ranh giới ghi save

Nguồn `b76828a9`: Save giữ chữ ký void/Message cũ, ghi `.tmp` cùng folder bằng UTF8/Flush(true), đóng rồi Move cho file mới hoặc Replace cho file có sẵn. Chỉ đặt Message thành công sau commit; lỗi IO truyền vào catch hiện có. Không copy đè rồi delete temp, không xóa save tốt trước commit, không fallback phá hủy nếu Replace không hỗ trợ. Một writer/profile. Không `.bak` hoặc tự nạp tiến trình dự phòng; Load chỉ đọc SavePath và validator như trước. `.tmp` sau lỗi không được Load chọn; Save chủ động tiếp theo ghi lại từ state live. Schema/ID/GUID cũ không đổi. Flush/replace đã kiểm lỗi mô phỏng trên Windows ổ C, không bảo đảm crash/mất điện thật hay filesystem khác. [Acceptance, bằng chứng và UI NOT_RUN](../QA/Tasks/N2/SAVE-WRITE-03/README.md). Giữ UI-LOAD-FEEDBACK-01/P-close OPEN; không wiring/UI validation mới.
