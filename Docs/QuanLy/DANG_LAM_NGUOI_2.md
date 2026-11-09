# Người 2 — Gameplay/dữ liệu/contracts

Trạng thái: **CHUA_NHAN**. Nhánh chính xác: `work/runtime-contracts-baseline`.

Unity **6000.6.2f1**. Baseline nhận việc **fcbb7e19c094a2a52d99237d03587df16679d579**; biên nhận **55d7f04**.
Entry: `Assets/ConRoi/Modules/TichHop/Scenes/ConRoi_Chinh.unity`, chỉ người 3 được sửa.
Checkpoint `Assets/Scenes/JourneyPresentation_v48_r107.unity` giữ nguyên. Scene thử dưới đây là **template rỗng**, không phải bản game độc lập.
Mỗi người dùng checkout/Library riêng; không dùng chung project đang mở. Đọc [quyền sở hữu](OWNERSHIP.md), [tích hợp](TICH_HOP.md) và [hợp đồng hiện hành](HOP_DONG_MODULE.md).
Không đổi GUID/meta, schema/save ID, pipeline/packages; không chạy author/derive/bind một lần. Không dọn 4.183 legacy hoặc archive.

Được sửa theo task: `Assets/ConRoi/Modules/Gameplay/`, `Contracts/`, `Assets/Scripts/` (kể cả toàn bộ partial `InvestigationV36R107`), công cụ compile/Editor/test dưới `Tools/`. Báo cáo riêng `Docs/QA/Tasks/N2/`; hợp đồng `Docs/QuanLy/HOP_DONG_MODULE.md`.
Không sửa scene tích hợp/checkpoint, GDD, map/art, lịch sử chung hoặc profile người dùng. Tool asset mặc định vẫn người 2 giữ file; chỉ giao người 1 sau khi ghi chuyển quyền, không hai người sửa cùng lúc.
Scene thử: `Assets/ConRoi/Modules/Gameplay/Scenes/Gameplay_ThuNghiem.unity`.

## Việc đầu tiên

Đối chiếu API/save/observation hiện hành với [GDD v3.6](../CON_ROI_GDD_v3.6_CHOT_TRIEN_KHAI.md) và HOP_DONG_MODULE. Hoàn thiện bảng producer/consumer, ID, trạng thái modal, dữ liệu lưu và acceptance cho một task gameplay/dữ liệu cụ thể trong tuyến **xưởng → Hà → xưởng**. Phân biệt đã có, thiếu được xác nhận và đề xuất; chưa tự triển khai tính năng mới hoặc refactor partial.

Hoàn tất task đầu khi hợp đồng bám mã thực, có ca kiểm/đầu vào/đầu ra và một nhiệm vụ nhỏ được người 3 review theo GDD. Không đổi schema1/namespace `conroi.investigation.v36.r107`, luật hồi hữu hạn hoặc sáu diễn biến dùng chung cơ cấu; seek không mở quan sát chưa xem. Giữ NAV-01 focus/modal đã đạt. Không chuyển lại 852 script lịch sử hoặc bỏ type còn consumer.

Bàn giao commit code riêng với docs và ca kiểm chịu ảnh hưởng, fixture/API ghi riêng với UI, yêu cầu wiring cho người 3. UI đề xuất chạm session partial do người 3 gửi phải được người 2 tích hợp; không tách module giả bằng đổi namespace/serialized field.
Prompt nhận việc: “Trên work/runtime-contracts-baseline từ fcbb7e19, xác minh và hoàn thiện hợp đồng API/save theo GDD v3.6; đề xuất một task nhỏ có acceptance trong tuyến hiện hành, giữ luật/schema và ownership toàn bộ partial session.”

## Điền khi nhận

- Người nhận / ngày:
- HEAD thực tế / file sẽ chạm:
- Task được review / file chuyển quyền:
- Kiểm và bằng chứng:
- Commit kết quả / giới hạn / người nhận bàn giao:


## Gameplay & Dữ liệu — SAVE-VALIDATION-01, nhận việc 08/11/2026

- Trạng thái: HOAN_TAT_PHAN_DOC_LAP_CHO_REVIEW. Ghi nhận mới dùng Gameplay & Dữ liệu, Map & Asset, UI/UX & Tích hợp; không đổi tên file hoặc nhánh chung.
- Nhánh: `work/runtime-contracts-baseline`; commit xuất phát: `0d878e3b7163c4d50e3976cd5c46118403b19240`.
- Workspace: `C:/ConRoi_Nguoi2`, Git worktree với working files và Library riêng, dùng chung object Git/LFS local. Checkout D giữ nguyên commit và thao tác xóa hướng dẫn chưa commit của UI/UX & Tích hợp; snapshot tại `C:/ConRoi_N2_Preservation_20261108`.
- Mục tiêu: xác minh contract save và chặn dữ liệu vật chứng/quan sát hoặc cấu trúc lỗi trước khi thay state phiên hiện tại. Giữ schema1, ID, GUID, luật cơ cấu và NAV-01.
- File dự kiến: `Assets/Scripts/InvestigationV36R107/InvestigationSession.cs`, lớp kiểm hợp lệ save cùng thư mục nếu tái hiện cần thiết; `Assets/ConRoi/Modules/Gameplay/Editor/SaveLoadContractChecks.cs` và meta tương ứng; `HOP_DONG_MODULE.md`, file tiến độ này, báo cáo `Docs/QA/Tasks/N2/SAVE-VALIDATION-01/`.
- Acceptance: tái hiện save lỗi trên baseline; save bình thường roundtrip và tiếp tục Observe/Save/Load; save lỗi bị từ chối trước khi thay data/controller/gate/modal và không bị ghi đè. Mở Gameplay_ThuNghiem bằng Unity 6000.6.2f1, kiểm compile/reference trong phạm vi rõ, profile fixture riêng.
- Kiểm: fixture/API tách khỏi UI/controller và tuyến chơi tự nhiên. Scene thử rỗng, fixture tạm runtime không lưu scene. UI/controller và người mới chưa có bằng chứng giữ NOT_RUN.
- Phụ thuộc: UI/UX & Tích hợp review contract/gói sửa trước tích hợp; không cần sửa UI hay scene chính để hoàn tất phần độc lập. Không cần xác thực mạng, không dùng credential chủ repo để publish.
- Kết quả: tái hiện baseline 16 ca/2 lỗi; sau sửa 42 ca/0 lỗi bằng PlayMode fixture/API và validator thuần. Compile đạt Unity 6000.6.2f1; scene thử rỗng có 0 Missing Script, không phải audit scene tích hợp.
- Commit nguồn: `546833aace5f59c0c98349ebe0fb7cd33cbe5df0`. Báo cáo, hash nguồn và kết quả: [SAVE-VALIDATION-01](../QA/Tasks/N2/SAVE-VALIDATION-01/README.md).
- Thay đổi: Load kiểm cấu trúc/save observations trước nhận state; schema1 và save hợp lệ baseline vẫn đọc/tiếp tục lưu tải được. Không sửa NAV-01, UI, map, scene, ID hoặc GUID cũ.
- Cách dùng: caller Load/F6 hiện có tự dùng guard; API mới tĩnh `InvestigationSaveValidation.TryValidate(InvestigationSave,out string)` chỉ đọc, trả lý do khi false. Không event/wiring mới.
- Chưa kiểm: UI/controller, tuyến tự nhiên, 1080p và người mới NOT_RUN. Fixture dùng profile riêng, không cấp dữ kiện rồi nhận là chơi tự nhiên. Giữ state khi validator từ chối; chưa bảo đảm rollback lỗi binding sau validation.
- Bàn giao UI/UX & Tích hợp: review contract và nhận commit nguồn, kiểm compile/Load tại nhánh tích hợp thống nhất; registry chung còn thiếu hook, cần chủ tài liệu xác nhận. Không tự merge/push. Map & Asset không có phụ thuộc mới.
- Việc tiếp: review gói nhỏ theo GDD, kiểm UI/controller chịu ảnh hưởng trước chốt tích hợp; chưa mở rộng gameplay hay dọn legacy.


## Gameplay & Dữ liệu — SAVE-VALIDATION-01, bổ sung UI/controller 08/11/2026

- Xuất phát `57dcbb03`, runtime `546833aa`; worktree `C:/ConRoi_Nguoi2`, nhánh không đổi. Snapshot status/diff trước phiên và trước stop nằm trong output riêng. Không reset tiến độ mới.
- Đã làm: thêm công cụ Editor dựng môi trường tạm trong Gameplay_ThuNghiem, không lưu scene; kiểm tiến trình qua Windows input tổng hợp/controller/IMGUI, không cấp chứng cứ qua API.
- File thay đổi: `Assets/ConRoi/Modules/Gameplay/Editor/SaveGuardControllerChecks.cs` và meta; báo cáo SAVE-VALIDATION-01, JSON kết quả/manifest nhỏ và tiến độ này. Commit công cụ: `bb88d4cc97ad78427320d03a64f0f49f4050857b`. Runtime/schema/ID/GUID cũ, UI và scene tích hợp không đổi.
- Kết quả: ghi bằng UI → F5 A → thay tiến trình qua D/E/ghi → F6 A → tiếp tục ghi/F5/F6 B đạt. Ba save lỗi (index999, ID trùng, JSON đứt) bị từ chối; state/identity/transform/gate/modal/input flags và bytes file giữ nguyên ở lần kiểm cuối. Sau đó P/E/Esc, khóa W trong modal, di chuyển/chuột, tải A/B hợp lệ và F5/F6 đạt.
- Giới hạn: input tổng hợp, môi trường proxy không phải tuyến xưởng tự nhiên; người mới NOT_RUN. Game captures1536×746, không kiểm lại1080p. Lần đầu input/focus chưa ổn định và một lần P-close sau pause chưa rõ nguyên nhân được giữ trong báo cáo; kiểm lại P ba chu kỳ đạt, chưa coi là lỗi NAV-01 và không sửa runtime. Không chạy lại42 ca API vì hash nguồn không đổi.
- OPEN gửi UI/UX & Tích hợp: UI-LOAD-FEEDBACK-01, lý do Load sai không hiển thị trong dossier/pause dù Message đã có; xem [báo cáo và acceptance](../QA/Tasks/N2/SAVE-VALIDATION-01/README.md). Chưa sửa thay UI; partial chung vẫn thuộc Gameplay & Dữ liệu.
- Bàn giao: nhận guard nguồn546833aa nếu chưa có, source test tại commit trên và tài liệu cập nhật; không wiring mới. Tiếp theo UI/UX & Tích hợp review thông báo lỗi và kiểm ở scene tích hợp theo lịch riêng. Chưa push/merge, không sửa map/save gốc hoặc dọn legacy.


## Gameplay & Dữ liệu — rà contract tiếp theo, nhận 08/11/2026

- Xuất phát `810168d2`, worktree/nhánh giữ nguyên, tracked sạch trước thao tác; hai thư mục untracked giữ nguyên. Không có nhiệm vụ runtime tiếp theo đã được giao trong tiến độ/HANDOFF/tích hợp hiện hành ngoài SAVE-VALIDATION-01.
- Gói độc lập: xác minh reference của EvidenceRelation tại biên validator, hoàn thiện đề xuất SAVE-REFERENCE-02 nếu có bằng chứng. Chưa triển khai guard mới, chưa tự nhận UI-LOAD-FEEDBACK-01 hoặc mở rộng nội dung.
- File sẽ chạm: công cụ kiểm Editor trong Gameplay, báo cáo riêng `Docs/QA/Tasks/N2/SAVE-REFERENCE-02/`, hợp đồng module và tiến độ này. Giữ toàn bộ session partial, schema1/ID/GUID; không sửa runtime, UI, map hoặc scene tích hợp.
- Điều kiện hoàn tất audit: có mẫu schema1 serialize/parse thật, quan sát kết quả validator và lookup consumer với ID relation hợp lệ/không tồn tại; phân biệt kiểm API với Load/UI; đề xuất acceptance và giới hạn cụ thể, source/hash truy lại được.
- Scene/profile: Gameplay_ThuNghiem, profile audit mới riêng; không dùng Play, không chạy lại 42 ca, không đụng profile live. Phụ thuộc sau audit: review phạm vi bởi UI/UX & Tích hợp trước triển khai đề xuất.

- Kết quả audit: compile Unity6000.6.2f1 đạt; bốn mẫu serialize/parse thật, hai đối chứng đạt và hai mẫu ID lạ được validator nhận nhưng lookup consumer phát sinh NullReferenceException được tool bắt. Validator không sửa input. Không chạy Load/UI/controller hoặc Play, không dùng profile game gốc.
- Trạng thái: HOAN_TAT_AUDIT_CHO_REVIEW_DE_XUAT. Đề xuất SAVE-REFERENCE-02 chỉ chặn definition không tồn tại ở first/second của relation; chưa triển khai, chưa đổi contract runtime. [Báo cáo/acceptance/hash](../QA/Tasks/N2/SAVE-REFERENCE-02/README.md).
- File thực đổi: RelationReferenceAudit.cs/meta mới; báo cáo/kết quả/manifest riêng, HOP_DONG_MODULE và tiến độ này. Runtime, save schema, IDs/GUID cũ và scene không đổi; không chạy lại42 ca vì không sửa validator. Giữ các điểm OPEN và bằng chứng SAVE-VALIDATION-01 tại810168d2.
- Bàn giao UI/UX & Tích hợp: review đề xuất nhỏ và giao triển khai nếu thống nhất; không cần wiring hay nhận bản vá runtime trong gói audit. Tiếp theo khi được giao: kiểm đầu vào relation ở biên save và chạy acceptance Load/state/file/UI đã nêu; không mở rộng cốt truyện hoặc sửa UI.


## Gameplay & Dữ liệu — SAVE-REFERENCE-02, triển khai được giao 08/11/2026

- Xuất phát `5b88f245`, tracked sạch; worktree/nhánh không đổi. Người dùng cho phép triển khai acceptance audit, không reset hoặc sửa UI.
- Quy tắc xác minh: relation đã lưu có first/second là ID definition thực từ InvestigationContent (registry dẫn source này); không có đầu tham chiếu tùy chọn. List relations rỗng hợp lệ. Note không phải reference, chấp nhận null/rỗng ở biên Load; AddRelation của thao tác mới vẫn yêu cầu note không trắng. Không thêm luật membership, cùng ID hoặc chấm nội dung.
- File sẽ đổi: InvestigationSaveValidation.cs, kiểm SaveLoadContractChecks.cs trong Gameplay/Editor, báo cáo/contract/tiến độ riêng. Không đổi schema/ID/GUID cũ hoặc runtime UI.
- Acceptance: sáu mẫu first/second lạ/rỗng/null bị Load từ chối trước thay state/file; các mẫu hợp lệ và save baseline tiếp tục đọc/lưu/tải. Chạy hồi quy SAVE-VALIDATION-01 vì đổi validator. Kiểm F6/UI bằng input tổng hợp ở profile sandbox mới, ghi rõ fixture và giới hạn. Các điểm OPEN SAVE-VALIDATION-01 giữ nguyên.
- Trạng thái: DANG_THUC_HIEN; snapshot và log riêng Test/Output/SaveReference02, không ghi save gốc.

- Kết quả chốt: nguồn guard `aee7d559`, helper so byte `ea49025a`; 54/54 fixture/API cuối đạt, gồm42 ca SAVE-VALIDATION-01 và12 ca relation. Sáu first/second lạ/rỗng/null bị từ chối có lý do, giữ state/file; save hợp lệ và baseline tiếp tục tải/lưu được. Không đổi schema/IDs/GUID hoặc UI.
- UI/controller: relation hợp lệ tạo từ E/ghi/đối chiếu/nhập/Ghi liên hệ bằng input OS tổng hợp; F5/F6 đạt. Hai mẫu ID lạ nguồn/đích qua F6, state/bytes giữ, ba chu kỳP/lookup hồ sơ/E/R/Esc/modal/movement/mouse và phục hồiF5/F6 đạt. Null-first qua F6 giữ state/file; kiểm tiếp bị gián đoạn focus browser đã xác nhận, chưa PASS trọn phần thao tác tiếp của mẫu bổ sung. Không có NullReferenceException trong log GUI. Các điểm P/1080p/người mới/thông báo modal của SAVE-VALIDATION-01 giữ OPEN/NOT_RUN, không tự nhận UI.
- Đánh giá luồng khác: runtime r107 có AddRelation và Load; producer thao tác giữ ID hợp lệ, Load có guard. data vẫn public mutable; caller mới phải giữ invariant, chưa có producer sản phẩm khác được xác minh cần vá. Không thêm null-check UI che lỗi hoặc refactor session partial.
- Trạng thái: PHAN_SUA_VA_KIEM_CHINH_DAT_CHO_REVIEW; giữ giới hạn UI null bị gián đoạn. [Báo cáo, source/harness commit và manifest](../QA/Tasks/N2/SAVE-REFERENCE-02/README.md). Nhận toàn bộ chuỗi sau SAVE-VALIDATION-01/audit, không wiring/migration mới. Bàn giao UI/UX & Tích hợp review và giữ các yêu cầu UI riêng.
- Phiên Editor đã đóng sau snapshot, scene không dirty; settings tự sinh được bảo toàn trước khôi phục riêng. Log/capture/profile local không stage, runtime chỉ đổi validator. Chưa push/merge, không sửa map/scene tích hợp/save gốc hoặc legacy.


## Gameplay & Dữ liệu — hoàn tất UI null từ abd0ea7e

- Nhánh/worktree và HEAD xác nhận đúng; tracked sạch lúc bắt đầu, hai thư mục untracked giữ nguyên. Hash nguồn khớp, không sửa runtime, không chạy lại54 ca.
- Đã mở GUI Unity6000.6.2f1, Gameplay_ThuNghiem, profile NullCompletion mới riêng. Lượt chưa bắt đầu Load: trước phím/chuột scripted đầu tiên, intro đã rời và có tiến trình ngoài script chưa rõ nguồn. Đánh dấu INTERRUPTED_PRECONDITION_UNKNOWN_INPUT, không PASS null hoặc FAIL gameplay.
- Đã bảo toàn snapshot Game/state/Git diff/status/log copy; không ghi SavePath hoặc dừng Play/đóng Editor/discard state chưa xác định. Đang chờ xác nhận phiên đang được dùng và desktop dành riêng. Không thay thời gian chờ bằng câu trả lời. Các settings tự sinh chưa restore khi Editor còn live.
- Đã hoàn tất bảng bàn giao gộp trong [báo cáo SAVE-REFERENCE-02](../QA/Tasks/N2/SAVE-REFERENCE-02/README.md), đủ baseline0d878e3b, nguồn546833aa/aee7d559, công cụ/meta/kiểmbb88d4cc/5b88f245/ea49025a và tài liệu57dcbb03/810168d2/abd0ea7e; tất cả ancestor đã kiểm. SAVE-VALIDATION-01 có liên kết tới bảng này.
- Còn thiếu: chạy Load null và toàn bộ phần thao tác/phục hồi của lượt mới khi phiên kiểm an toàn. Giữ giới hạn lượt cũ, UI-LOAD-FEEDBACK-01/P-close OPEN; UI1080p và người mới NOT_RUN. Không push/merge, không sửa UI/map/scene tích hợp/save gốc.


## Gameplay & Dữ liệu — chốt UI null có desktop dành riêng

- Xuất phát/HEAD đã kiểm: `490f7a021f6218308da407f45388a152ab635bb9`; nhánh `work/runtime-contracts-baseline`, worktree riêng không đổi. Người dùng xác nhận desktop/Unity không có ai dùng và cho phép tiếp tục; snapshot/state/diff/log cũ giữ nguyên.
- **HOAN_TAT_PHAM_VI_SAVE_REFERENCE_02_CHO_REVIEW:** hai ca `first=null` và `relations=[null]` qua F6 đều từ chối, không đổi toàn state/data identity/cờ controller-modal/pause và bytes save. Sau mỗi ca, P mở/đóng, E/R, Esc, di chuyển/chuột, chặn di chuyển trong modal, nạp hợp lệ/chơi tiếp/F5/F6 đạt. Không có exception tham chiếu trong log.
- Kiểm bằng input Windows tổng hợp trên Gameplay_ThuNghiem/profile riêng, có dùng bản sao save hợp lệ đã tạo bằng UI trước đó. Không phải New Game sạch, tuyến tự nhiên hoặc playtest người mới. Không dùng API cấp tiến trình/mở trang thay input. Không sửa runtime và không chạy lại54 ca API; hash source khớp manifest.
- Bàn giao gộp đầy đủ baseline, nguồn, công cụ/meta và tài liệu tại [SAVE-REFERENCE-02](../QA/Tasks/N2/SAVE-REFERENCE-02/README.md), kèm null-authorized-results/manifest. Nhận cả chuỗi SAVE-VALIDATION-01 → SAVE-REFERENCE-02 theo bảng, không chỉ commit kiểm/tài liệu cuối; không wiring mới.
- Lượt gián đoạn intro chưa rõ nguồn vẫn giữ trạng thái cũ; không đổi FAIL/PASS. UI-LOAD-FEEDBACK-01 và P-close vẫn OPEN riêng cho UI/UX & Tích hợp; UI1080p/người mới NOT_RUN (Game1536×746).
- Đã snapshot cuối, dừng Play/đóng Editor thử không lưu scene, sceneDirty=false. Settings tự sinh line-ending-only được bảo toàn rồi khôi phục đúng file. Chỉ cập nhật báo cáo/manifest/tiến độ, không chạm UI/map/scene tích hợp/save gốc hoặc legacy; hai thư mục untracked cũ giữ nguyên. Chưa push/merge.


## Gameplay & Dữ liệu — SAVE-WRITE-03 nhận triển khai

- Xuất phát `148440b7a851f9578701377b81367b61b46e7e16`, nhánh runtime đúng, tracked sạch; hai thư mục untracked cũ giữ nguyên. Người dùng cho phép task, chưa push/merge trong đợt này.
- Mục tiêu: lỗi ghi báo đúng, save tốt cũ giữ bytes/Load được, khắc phục rồi Save/Load tiếp. Acceptance và cách mô phỏng giới hạn tại [SAVE-WRITE-03](../QA/Tasks/N2/SAVE-WRITE-03/README.md).
- File dự kiến: công cụ `Gameplay/Editor/SaveWriteContractChecks.cs`/meta; nếu lỗi được xác minh, helper ghi save và lời gọi Save trong session. Contracts/báo cáo/tiến độ cập nhật theo kết quả; không chạm UI/map/scene tích hợp/save gốc.
- Thứ tự: baseline physical IO faults → sửa tối thiểu có căn cứ → API/SaveLoad hồi quy → F5/F6 nếu desktop khả thi. Giữ điểm OPEN/NOT_RUN hai gói trước.


- SAVE-WRITE-03 kết quả: nguồn `b76828a9`, công cụ `b26e8404`, chế độ kiểm tương thích `b1858a78`. Baseline có hai assertion xác nhận Save báo lỗi sau copy nhưng bytes đích đã đổi; entry helper chưa có ghi NOT_RUN riêng. Đã sửa helper ghi/flush temp rồi Move/Replace, không cleanup sau commit hoặc chọn temp làm save, không đổi schema/IDs/GUID cũ.
- Kiểm mới43/43 fixture/API đạt: lỗi tạo/mở ghi temp, khóa đích/rename temp, temp dở, lỗi commit lần đầu, partial-write IOException và phục hồi Save/Load. Hồi quy đầu53/54; một ca dùng nhầm fixture UI, đã giữ FAIL cấu hình. Chỉ chạy lại tương thích với đúng fixture baseline: ba ca + hai tiền điều kiện đều đạt, source fixture không đổi. Không lặp các ca đã PASS.
- Compile và Missing Script phạm vi template/root fixture đạt, không audit scene tích hợp/render. F5/F6 UI/controller SAVE-WRITE-03 NOT_RUN: đã hỏi desktop dành riêng, chưa có xác nhận trong lượt mới; không gửi input. Không gọi fixture là tuyến tự nhiên hoặc người mới.
- Trạng thái: NGUON_VA_API_DAT_CHO_REVIEW_CON_THIEU_UI. [Báo cáo/acceptance/bằng chứng](../QA/Tasks/N2/SAVE-WRITE-03/README.md). UI/UX & Tích hợp nhận cả chuỗi sau148440b7; không wiring mới. Việc tiếp: kiểm F5/F6 lỗi và recovery trong phiên desktop dành riêng/profile mới; giữ UI-LOAD-FEEDBACK-01/P-close OPEN, UI1080p/người mới NOT_RUN.
- Không sửa UI/map/scene tích hợp/save gốc/legacy. Settings tự sinh line-ending-only được copy/hash trước khôi phục sau Editor thoát. Không push/merge/build/ZIP; output/profile/log dài và untracked cũ không stage.


## Gameplay & Dữ liệu — SAVE-WRITE-03 hoàn tất UI/controller 09/11/2026

- HEAD kiểm `b4bf64846df349cddfe6ace48c9a0f063acdaf5d`, nhánh runtime đúng, tracked sạch trước mở; chuỗi/hash nguồn khớp. Người dùng dành riêng desktop. Scene Gameplay/profile mới, snapshot/status/diff trước input, không nạp fixture hoặc cấp tiến trình qua API.
- PASS input Windows tổng hợp: Bắt đầu/E → F5 → ghi quan sát → F6 phục hồi → chơi tiếp/ghi đè/F5/F6. Bốn lỗi qua F5: directory temp, khóa đích, khóa ghi temp, khóa rename/delete temp; mỗi ca báo lỗi đúng, giữ bytes/hash save cũ, F6 tải được khi lỗi còn giữ, P ba chu kỳ/E/R/Esc/movement/mouse/modal đạt, gỡ lỗi rồi F5/F6 lưu-phục hồi tiến trình mới.
- Giữ riêng lượt click tọa độ shoeprint sai và lượt cuối mất focus sang WindowsTerminal trước F6; chỉ ca cuối được kiểm lại riêng và đạt, không gọi gián đoạn là FAIL gameplay hoặc PASS phần chưa chạy. Raw kết quả/ảnh/log theo [báo cáo](../QA/Tasks/N2/SAVE-WRITE-03/README.md).
- Trạng thái: HOAN_TAT_PHAM_VI_CHO_REVIEW. Không runtime/test Unity thay đổi, không chạy lại43/54 API; partial-write delegate vẫn chỉ API. Không chứng minh mất điện/crash thật. UI-LOAD-FEEDBACK-01/P-close OPEN, UI1080p/người mới NOT_RUN; không đồng nhất desktop1080p với Game1536×746.
- Bàn giao UI/UX & Tích hợp: nhận đủ b76828a9 → b26e8404 → b1858a78 → b4bf6484 và commit báo cáo UI mới sau baseline148440b7, không wiring mới. Lượt nhầm fixture và kiểm tương thích riêng giữ nguyên. Đã stop/quit phiên thử không lưu scene, sceneDirty=false; settings copy/hash rồi khôi phục bytes trước phiên. Chưa push/merge, giữ save gốc/map/scene tích hợp/legacy/untracked cũ.


## Gameplay & Dữ liệu — đề xuất TRIAL-CONTEXT-01, 09/11/2026

- Chưa có nhiệm vụ gameplay tiếp theo được giao trong tiến độ. Đã đọc GDD v3.6, ownership/contracts và rà tuyến khám → ghi → mục tiêu → mật mã → đối chiếu → dựng lại → kết luận bằng source. Không mở lại các lượt xác nhận/kiểm save.
- Chọn một khoảng thiếu: Update tiếp tục AdvanceTrial khi Page không còn reconstruction, còn ClosePage/OpenPage không dừng trial; khi hết timeline, tự cập nhật TrialReport/thêm TrialRecord dù stageCamera đã tắt. SeekTrial đã có guard không record khi tua, không coi đó là lỗi mới.
- Đề xuất **TRIAL-CONTEXT-01 — Tạm ngưng diễn biến khi rời bàn dựng**: ngừng bước mô phỏng ngoài view, quay lại tiếp tục từ thời điểm cũ nếu trước đó đang phát; pause tay vẫn giữ. Đây là phương án đề xuất theo GDD7.1/7.2, chưa phải quy tắc đã duyệt hoặc lỗi đã chạy UI tái hiện.
- [Phạm vi, căn cứ source, phụ thuộc và acceptance](../QA/Tasks/N2/TRIAL-CONTEXT-01/README.md). File nguồn dự kiến chỉ InvestigationSession.cs; tool kiểm mới chỉ tạo khi được giao. Không đổi luật cơ cấu, schema/ID/GUID, UI/presentation, map, scene tích hợp hay GDD.
- Trạng thái: DE_XUAT_CHO_GIAO, SOURCE_AUDIT_ONLY; API/UI_RUNTIME_NOT_RUN. UI/UX & Tích hợp review nhịp quay lại và binding fixture thử; không cần asset mới. Chưa sửa runtime/công cụ, không chạy Unity, push hoặc merge.


## Gameplay & Dữ liệu — nhận triển khai TRIAL-CONTEXT-01

- Người dùng giao triển khai từ đề xuất7cfcd7ca; nhánh đúng, tracked sạch trước khi làm; untracked cũ giữ nguyên. Acceptance được giao: playback chỉ tiến ở bàn dựng, giữ manual pause, không catch-up/record ngoài trang, không sửa record cũ; sáu ca và lifecycle chịu ảnh hưởng.
- Bắt đầu bằng fixture/API baseline, rồi đường rời trang UI thật qua input tổng hợp khi desktop dành riêng. Nếu xác nhận mới sửa nguồn một điều kiện trong InvestigationSession.cs. Công cụ/fixtures tạm tại Gameplay/Editor, không lưu scene hoặc đổi presentation sản phẩm. Không chạy bộ save; chỉ một roundtrip lifecycle có trial.


## Gameplay & Dữ liệu — TRIAL-CONTEXT-01 hoàn tất phạm vi, chờ review

- Đã tái hiện baseline7cfcd7ca bằng API cả sáu ca và input UI Windows tổng hợp qua R/Esc/P/header Trở lại/Hồ sơ; timeline tiến ngoài trang, P vắng lâu làm record0→1. Không suy lỗi từ code đơn thuần.
- Nguồn8c37b866 sửa duy nhất điều kiện Page trong Update; toolb09c7790 là commit thực sự kiểm. Giữ manual pause, timeline/luật/tốc độ/seek/clear-holds, schema/ID/GUID, record cũ. Không sửa UI/map/scene tích hợp hoặc save gốc.
- PASS103/103 API và26/26 UI/controller tổng hợp trong scene Gameplay với môi trường proxy/profile mới. Sáu ca API, nhiều đường thoát/quay lại, no catch-up/record ngoài trang, pause tay/game, seek/đổi góc/clear-holds/đổi ca, hoàn tất1record/mở lại và1roundtrip save/load theo lifecycle hiện có. Không chạy bộ save. Tool compile attempt đầu và baseline fail được giữ riêng trong báo cáo.
- [Acceptance, bằng chứng, cách chạy và giới hạn](../QA/Tasks/N2/TRIAL-CONTEXT-01/README.md). Raw/profile chỉ local. Unity đã stop/quit không save scene, dirty=false; settings newline-only được bảo toàn/đối chiếu trước phục hồi. Untracked cũ giữ nguyên.
- UI/UX & Tích hợp nhận đủ7cfcd7ca→8c37b866→b09c7790→commit tài liệu, không wiring mới; review lifecycle rồi kiểm gói chịu ảnh hưởng ở scene tích hợp thuộc quyền mình. Không cần asset mới. UI1080p/người mới/scene tích hợp NOT_RUN; drag-đổi trang controller chưa kiểm riêng. Các điểm UI Load feedback/P-close cũ giữ OPEN riêng. Chưa push/merge.
