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


## Gameplay & Dữ liệu — nhận OBS-PROGRESS-01

- Xuất phát00c17147, nhánh runtime đúng/tracked sạch; không có gói gameplay mới chưa làm đã được giao trong tiến độ. Người dùng cho phép chọn một khoảng thiếu có căn cứ.
- Mục tiêu: Observe ghi dấu sàn thành công phải đồng bộ firstFloorRecorded; Acquire/khám chưa ghi không tính, ghi trùng không nhân đôi. Source hiện chỉ batch wrapper đặt cờ. Luồng UI nút hiện có không được nhận là lỗi đã tái hiện.
- Acceptance trước sửa tại [OBS-PROGRESS-01](../QA/Tasks/N2/OBS-PROGRESS-01/README.md). File: Session.cs/Observe, phần runtime RecordVisibleObservations trong Onboarding.cs; công cụ Gameplay/Editor, contracts/báo cáo riêng. Không sửa UI/CurrentObjective/scene/map hoặc schema/ID/GUID.
- Thứ tự: baselineAPI → sửa tối thiểu → API thứ tự/lặp/roundtrip tiến trình → UI đại diện khi desktop an toàn. Phụ thuộc UI/UX & Tích hợp review/API cũ, không cần asset mới.


## Gameplay & Dữ liệu — OBS-PROGRESS-01 hoàn tất phạm vi, chờ review

- Baseline00c17147:10 assertions/3 lỗi API được xác nhận: Observe hai ID dấu sàn ghi thật nhưng cờ false; batch chưa thu lại đặt cờtrue khi không ghi. UI nút hợp lệ vốn hoạt động; không nhận lỗi API là lỗi UI đã xảy ra.
- Nguồn1bd2ebd7 đồng bộ firstFloorRecorded tại Observe sau guard; batch wrapper dùng chuyển trạng thái và từ chối vật chưa thu. Chỉ đổi runtime trong Session.cs/Onboarding.cs; không sửa view/CurrentObjective/scene/map, ID/schema/GUID hoặc cốt truyện.
- Công cụ/commit thực sự kiểm45917774:35/35 API và14/14 UI/controller input Windows tổng hợp đạt trong scene Gameplay/profile mới. Kiểm khám-chưa-ghi, thứ tự/lặp, guard, skip, không cấp dữ kiện khác, ba bước F5/F6 và tiếp tục. Không chạy lại các bộ save/trial cũ.
- [Báo cáo, acceptance trước sửa, bằng chứng và giới hạn](../QA/Tasks/N2/OBS-PROGRESS-01/README.md). Scene proxy không lưu, Editor đã stop/quit/dirtyfalse; settings byte bảo toàn rồi phục hồi khi chỉ khác newline; raw/profile ngoài Git, untracked cũ giữ nguyên.
- UI/UX & Tích hợp nhận đủ1bd2ebd7→45917774→commit tài liệu, không wiring mới; review consumer Observe/state và kiểm ghi→P ở scene chung. Scene tích hợp/UI1080p/người mới NOT_RUN, các điểm Load feedback/P-close cũ vẫn OPEN riêng. Chưa push/merge; không nhận nhiệm vụ nội dung khác.

## Gameplay & Dữ liệu — SAVE-EVIDENCE-04 nhận triển khai

- Xuất phát91835c80, tracked sạch, giữ untracked/tiến độ mới. Đọc INT-SAVE-NULL-LIST-01 của UI/UX & Tích hợp từ e30699629; không nhận cả nhánh review.
- Mục tiêu: phân biệt JSON evidence:null/missing/[] trước JsonUtility thay state; giữ schema1 và New Game rỗng. Acceptance trước sửa: [SAVE-EVIDENCE-04](../QA/Tasks/N2/SAVE-EVIDENCE-04/README.md).
- Dự kiến sửa validator và luồng Load trong InvestigationSession.cs nếu baseline xác nhận; công cụ riêng Gameplay/Editor, contracts và báo cáo N2. Không đổi UI/scene/map/ID/GUID/save gốc.
- Kiểm baselineAPI → tương thích → sửa tối thiểu → API hồi quy liên quan → F5/F6 tổng hợp khi desktop an toàn. Giữ Load feedback/P-close OPEN riêng. Chưa push/merge hoặc chốt baseline nhóm.

### SAVE-EVIDENCE-04 — hoàn tất nguồn và kiểm Gameplay, chờ scene chung

- Nguồn17b1f6e2 chặn collection JSON trước JsonUtility/gán state; công cụ/meta f73631ae. evidence:null/thiếu/sai kiểu/null-record bị từ chối; evidence:[] New Game và fixture schema1 cũ vẫn tải/tiếp tục. Rà thêm các collection cùng nguyên nhân, giữ default vắng collection phụ, không sửa schema/ID/GUID/flag/UI hoặc Save writer.
- Baseline74 assertions có14 không đạt do cùng lỗi coercion; baseline F5/F6 input tổng hợp tái hiện slide1→0 với evidence:null, giữ bytes mẫu. Sau sửa78/78 API và54/54 hồi quy validator/Load đạt; không chạy IO/trial. [Báo cáo và hợp đồng chi tiết](../QA/Tasks/N2/SAVE-EVIDENCE-04/README.md).
- Trên f73631ae, UI/controller Windows tổng hợp trong scene Gameplay proxy/profile mới: F6 từ chối null/thiếu/null-record, state/bytes giữ nguyên; sau mỗi ca P/E/Esc/pause/W/recovery F5/F6 đạt. UI New Game[] và tiếp tục ghi/lặp/đổi vị trí/lưu/tải đạt. Không API grant trong lượt UI; sandbox không phải scene chung/tuyến tự nhiên đầy đủ.
- Giữ hai lượt gián đoạn focus riêng: lần đầu chưa gửi input; cuối lượt trước probe W/chuột bổ sung, phần chưa chạy NOT_RUN. Không lỗi runtime mới xác nhận. Edit sạch/Editor đã đóng, settings và untracked cũ bảo toàn; raw/profile/log ngoài Git.
- UI/UX & Tích hợp nhận đủ17b1f6e2→f73631ae→commit tài liệu để kiểm lại INT-SAVE-NULL-LIST-01 ở ConRoi_Chinh; không wiring mới. Scene chung/UI1080p/người mới NOT_RUN với bản sửa; Load feedback/P-close OPEN riêng. Chưa push/merge hoặc chốt baseline nhóm mới.

## Gameplay & Dữ liệu — nhận UI-LOAD-FEEDBACK-01

- Xuất phát597369a5, worktree C:/ConRoi_Nguoi2; tracked sạch, untracked cũ giữ nguyên. Đọc trực tiếp phiếu N3 và UI-NAV-REVIEW-02 tại review ef8911965, không merge review hoặc đổi nhánh.
- Mục tiêu: Message Load hiện có phải đọc được trong modal; lỗi file/parse không lộ chi tiết thừa; lỗi cũ được thay ngay khi tải thành công. Giữ6giây hiện có, focus/P-close và guard. Sửa UI partial đang sở hữu, thêm catch an toàn trong Session.cs nếu cần; không sửa validator/schema/ID/GUID/scene chung.
- [Acceptance trước sửa và phạm vi](../QA/Tasks/N2/UI-LOAD-FEEDBACK-01/README.md). Kiểm API targeted rồi GUI/controller trong scene Gameplay/profile riêng; không chạy lại bộ save cũ. UI/UX & Tích hợp nhận nguồn và kiểm scene chung/bố cục sau bàn giao.

### UI-LOAD-FEEDBACK-01 — hoàn tất phần Gameplay & Dữ liệu

- Nguồn25f14632: ModalMessage dùng Message runtime hiện có cho footer modal/header review, có wrap/scroll và giữ6giây; lỗi đọc/parse trả lời nhắn an toàn. Không đổi guard/schema/ID/GUID, world renderer, focus/P-close hoặc save. Hai partial được sửa đúng ownership, không chạm scene chung/UI ngoài quyền.
- Công cụ/meta và commit đã chạy5be056e4:66/66 API targeted đạt valid/null/JSON/read ở world/dossier/pause, state/bytes/recovery. Controller Windows tổng hợp trong scene Gameplay/profile mới tạo progress bằng E/ghi/F5; F6/nút Tải validation/read hiển thị lý do trong dossier/pause1080p, không báo sai thành công; unlock/F6 thay lỗi bằng success, P/E/Esc/modal/pause/di chuyển và F5/F6 tiếp đạt. Không chạy lại bộ save/IO/trial cũ.
- Giữ riêng lượt click Ghi chưa tác động do zoom1,3×; sau chỉnh Editor fit0,86× kiểm lại đạt. Không có foreground-loss exception trong phiên; probe cũ/mất focus trước giữ riêng. Đã đóng Editor không lưu scene, settings/untracked bảo toàn.
- [Báo cáo và chuỗi nhận](../QA/Tasks/N2/UI-LOAD-FEEDBACK-01/README.md). UI/UX & Tích hợp nhận nguồn/công cụ/tài liệu trên tiền đề597369a5, không wiring mới; phải review footer/header và các trang chịu co nội dung trong scene chung. Scene chung/mọi UI1080p/người mới NOT_RUN; P-close OPEN không sửa. Chưa push/merge/chốt baseline.

## Gameplay & Dữ liệu — tuyến prototype, nhận 09/11/2026

Xuất phát thực tế a374a93a; snapshot Test/Output/GameplayScope20261109. Fetch và fast-forward an toàn tới baf7335b5 (baseline kỹ thuật c93cb4f1e, nguồn tích hợp thực kiểm f242b60a), không mất commit riêng; hai thư mục untracked giữ nguyên. Toàn bộ session partial thuộc Gameplay & Dữ liệu; không sửa scene chung/map/GDD/tài liệu điều phối/UI thuộc vai trò khác. Tài liệu N3 nhận qua baseline, không chỉnh tiếp. CLI profile riêng giữ nguyên.

### Checklist chức năng trước sửa

| Phạm vi | Đã có / căn cứ source | Thiếu, trạng thái và phạm vi hiện tại |
| --- | --- | --- |
| Tương tác/khám | Session.TryInteractionHit/Interact/OpenEvidence; GDD6/9: ray gần nhất, E, viewer, khóa input; mẫu áo/giày có samplesAllowed | ĐÃ CÓ trong prototype; chất lượng collider/góc nhìn là phụ thuộc Map & Asset, không tự sửa geometry. Kiểm tuyến đại diện, không audit mọi asset. |
| Ghi quan sát | Observe/RecordVisibleObservations; GDD7.2; OBS-PROGRESS-01: khám chưa ghi, index hợp lệ, lặp không nhân đôi | ĐÃ CÓ; clip bị chặn ghi trước hai vùng exposure. Nhưng transcript clip ở InspectUI/CompareUI vẫn in trực tiếp từ definition trước guard: chọn CLIP-DISCLOSURE-01 kiểm baseline và sửa. |
| Tiến trình/mục tiêu | Onboarding.CurrentObjective/UpdateOnboarding; dấu sàn→P→phone→xem clip→cơ cấu→Hà→quay lại; GDD12.1/13 | ĐÃ CÓ tuyến prototype; skip không cấp chứng cứ, Acquire không tính ghi. Không tự thêm gate kết luận theo trial hoặc ép mọi observation khi GDD không yêu cầu. |
| Mật mã | Assign/ReadCipher/RecordReading + Alphabet/Hà; GDD5.3: tập ký tự prototype, Henri tùy chọn | ĐÃ CÓ bản đọc/ghi cụm rút gọn, không cấp người gửi; không mở rộng26 ký tự/năm thư. UI/asset final là phụ thuộc theo giai đoạn. |
| Đối chiếu | AddRelation/CompareUI; GDD7.2: hai mục, ghi suy luận không tự thành sự thật | ĐÃ CÓ, không chấm đúng/sai/đổi ID. Cùng consumer transcript clip phải dùng guard mới; không sửa hình/căn cứ giấy chưa có nguồn độc lập. |
| Bàn dựng | TrialTimeline/AdvanceTrial/SeekTrial/ClearTrialHolds; GDD3.3/7.1; TRIAL-CONTEXT-01 | ĐÃ CÓ sáu ca, pause/context/seek; không đổi luật hồi/tốc độ. Kiểm một ca tuyến đại diện, không chạy lại103 ca. Gate chứng cứ/hình học ngoài đời chưa được tuyên PASS. |
| Kết luận prototype | ConclusionUI/ConclusionRecord, bốn mục và chưa đủ; GDD7.3/13 | THIẾU save riêng trước chốt; handler đặt finalized trước Save(void), vẫn feedback thành công khi IO fail. Chọn CONCLUSION-CHECKPOINT-01. Giữ nhận định chưa đủ, không tự chấm đủ chuỗi hoặc tạo ending A/C chưa có nội dung. |
| Save/load | SaveFile/SaveValidation/Load; GDD11; các gói save đã tích hợp | ĐÃ CÓ guard/atomic write + feedback. Chỉ hồi quy roundtrip liên quan transcript/kết luận; không chạy lại toàn bộ IO/API. Checkpoint trước chốt mới không được Load tự chọn. |
| Lát cắt/ending/mở khu | GDD12.3–4/8; nguồn draft hiện ghi chưa nối người gửi, chưa có nguồn động cơ đầy đủ | NGOÀI GIAI ĐOẠN prototype đang giao. Không tạo NPC/chương, luật chấm chứng cứ/ending hay schema mới. Nhóm giao nội dung và cơ chế review khi chuyển giai đoạn. |

### Hai gói triển khai và acceptance trước sửa

1. **CLIP-DISCLOSURE-01 — Không lộ phần ghi nhận clip chưa xem**. GDD7.2/11: chưa xem thì sổ không tiết lộ. Dùng đúng hai cờ exposure hiện có của Observe, không thiết kế luật xem mới. Transcript trên inspect/compare chưa đủ cả hai cờ chỉ có hướng dẫn trung tính; đủ thì giữ bản chép cũ. Không tự ghi observation/cấp dữ kiện khi mở/xem/tua; Acquire clip không đồng nghĩa đã xem. Batch ghi bị từ chối không được báo đã ghi thành công. Kiểm chưa xem/chỉ trước/chỉ sự cố/đủ, lặp/seek/quay lại và save/load; UI/controller đại diện nếu desktop riêng.
2. **CONCLUSION-CHECKPOINT-01 — Chốt bản ghi có checkpoint an toàn**. GDD7.3: xác nhận trung tính và save riêng ngay trước chốt. Tạo snapshot schema1 finalized=false tại đường dẫn riêng cố định trong profile; chỉ đặt live finalized=true/báo thành công sau ghi bản chốt thành công. Save cũ hợp lệ không bị xóa trước commit, lỗi checkpoint hoặc file đích phải trả lỗi qua Message cũ, không chốt giả; khắc phục tiếp tục được. Checkpoint được khôi phục tường minh bằng bản sao, không tự Load nhầm. Xác nhận phải gắn đúng nội dung hiện tại, sửa nội dung/Load không giữ xác nhận cũ. Không chấm đáp án hoặc thêm ending. Kiểm API các bước/IO lỗi chỉ profile mới và khôi phục finally, UI/controller nếu có desktop; hồi quy roundtrip gắn căn cứ và chưa đủ.

File dự kiến: Session.cs, Onboarding.cs, SessionUI.cs (partial đang sở hữu; chỉ consumer guard/hành động runtime, giữ layout), partial runtime mới nếu giúp tách gói; tool/meta trong Gameplay/Editor; báo cáo N2, contracts và tiến độ này. Không sửa presentation/scene thuộc vai trò khác. Mỗi gói có commit nguồn riêng và công cụ/báo cáo; nhận hết chuỗi, không wiring scene mới.

Đang làm: acceptance đã ghi, tiếp theo tái hiện baseline qua API/source rồi sửa tối thiểu; desktop đang chờ xác nhận để kiểm input. P-close OPEN/chưa tái hiện, người mới NOT_RUN; không dùng chúng chặn phần runtime độc lập.


### Hoàn thành phần runtime tuyến prototype — 09/11/2026

- **Đã làm:** CLIP-DISCLOSURE-01 không lộ bản chép trước exposure và phản hồi ghi đúng; CONCLUSION-CHECKPOINT-01 xác nhận gắn record, checkpoint trước chốt, không finalized/báo thành công giả, API restore tường minh. Schema1/ID/GUID/luật và guards cũ giữ nguyên.
- Checklist trước sửa được giữ: tám nhóm runtime hiện có đã đối chiếu; hai thiếu sót chọn đã triển khai/kiểm. Không mở rộng lát cắt/ending/full26 tư thế. Chất lượng asset/media/tuyến scene chung và nút restore UI là phụ thuộc, không tính đã xong toàn tuyến người chơi cuối.
- **Nguồn:** e0544b74 → d7f1c93b. Công cụ71b20f71 → df2a1a83. API cuối trên df2a1a83 **86/86**; controller/UI trên d7f1c93b **37/37**, blob runtime bằng nhau. Không input Windows: IMGUI event đích GameView + keyboard ảo, proxy scene Gameplay/profile mới, không fixture cấp chứng cứ trực tiếp trong routeUI. API có cấp dữ liệu/giải mã/exposure, ghi riêng.
- **Giữ lịch sử:** baseline clip3 lỗi/8, kết luận4 lỗi/6; lượt24/25,80/86,85/86 và click DPI gián đoạn không đổi thành PASS. IO ngoài dự kiến chưa rõ process/nguyên nhân; log tạm chỉ thấy các lỗi chủ động tạo, đã phục hồi nguồn và kiểm commit thật đạt. Không thay writer/retry che lỗi.
- **Bàn giao:** [PROTOTYPE-FLOW-01: chuỗi, acceptance, kiểm, lệnh, phụ thuộc](../QA/Tasks/N2/PROTOTYPE-FLOW-01/README.md). Cập nhật HOP_DONG_MODULE; không cần wiring cho clip/nút chốt cũ. UI/UX & Tích hợp nhận toàn chuỗi, kiểm scene chung; nếu cần nút restore, thống nhất riêng trước nối API, hiện chưa có nút mới.
- **Còn thiếu/phụ thuộc:** scene chung/UI1080p/người mới trên nguồn mới NOT_RUN; P-close OPEN/chưa tái hiện/nguyên nhân chưa rõ; media clip thật và readability cần kiểm tích hợp. Không chốt baseline nhóm. Năm file Unity bảo toàn bytes trước kiểm, không lưu scene runtime; snapshot/log/profile local không commit. Hai thư mục untracked cũ giữ nguyên.
- Không còn việc runtime rõ trong hai gói đã chọn; việc tiếp là tích hợp/kiểm các phụ thuộc trên và nhận quyết định nội dung giai đoạn lát cắt khi nhóm giao. Không tự thêm chương hoặc ending.

### Closure acceptance — tiếp tục sau15d15da2

Đã kiểm HEAD15d15da2 trên work/runtime-contracts-baseline, tracked sạch, hai untracked cũ giữ nguyên; snapshot Test/Output/PrototypeClosure20261109. Không chọn thêm chức năng. Rà GDD7.2/7.3/8/11/12.1/13 và code xác nhận tám nhóm prototype đã có logic; cần đóng bằng chứng/owner cho phần chưa kiểm đầy đủ, không gọi mọi UI/asset đã xong.

Kiểm hẹp trước khi chốt: thay choice/grounds/claim qua thao tác UI làm xác nhận mất hiệu lực, đổi về nội dung cũ không phục hồi xác nhận; lỗi tạo checkpoint/ghi chính qua nút chốt không báo thành công hoặc đổi state/bytes; gỡ điều kiện lỗi rồi chốt/load được. API bổ sung kiểm toàn state/nav/bytes sau lỗi và lặp lại đúng chuỗi resume từng có IO lỗi, không retry tự động che failure. IO chưa giải thích giữ OPEN, không coi một lượt đạt là đã hết. Không chạy lại bộ save/trial hoặc86 ca gộp.

Restore trước chốt: runtime API đã có; GDD8 có nhu cầu người chơi thử lại trước chốt, nhưng đường truy cập UI chưa được thiết kế/giao. UI/UX & Tích hợp là chủ điều phối đường truy cập và acceptance người chơi; không tự yêu cầu/thêm nút mới hoặc coi API là luồng người chơi đã hoàn tất.


### Checklist đóng phạm vi chức năng — đối chiếu sau15d15da2

Đây là trạng thái hiện hành thay cho cột “trước sửa” bên trên. Không chọn thêm task ngoài checklist, không coi có code là toàn bộ sản phẩm đã kiểm. Chủ runtime cả tám hàng là **Gameplay & Dữ liệu**.

| Chức năng người chơi | GDD / code thực tế | Trạng thái runtime và bằng chứng có phạm vi | Phần còn lại, chủ sở hữu và điều kiện nhận |
|---|---|---|---|
| E khám, mở/xem vật, mẫu có điều kiện | GDD6/9; Session.TryInteractionHit/Interact/OpenEvidence; Actor.Greet | **Đã có/đã kiểm tuyến đại diện**: nearest obstruction/self collider theo code, modal chặn đi, E không tự ghi; route37 và closure dùng controller thật trên proxy | **Phụ thuộc Map & Asset:** collider/góc nhìn/inspectionPrefab/model nguồn thực; **UI/UX & Tích hợp:** nhận đúng binding scene chung và kiểm các mục ngoài proxy. Không tuyên mọi asset PASS |
| Ghi quan sát, không tính khám là ghi, lặp không nhân đôi | GDD7.2; Session.Observe, Onboarding.RecordVisibleObservations | **Hoàn tất runtime/đã kiểm** OBS-PROGRESS-01 và clip e0544b74; clip43 API + route37. Trước đủ exposure UI không đưa nút Ghi; lời từ chối batch kiểm API, không giả là đã click nút không tồn tại | **UI/UX & Tích hợp:** inspect/compare clip trước/chỉ một vùng/đủ cả hai trên media thật, xác nhận không lộ chữ và không phản hồi ghi giả |
| Mục tiêu theo tiến trình dấu sàn→P→phone→clip→cơ cấu→Hà→xưởng | GDD12.1/13; Onboarding.CurrentObjective/UpdateOnboarding; Session.OpenPage/Update | **Đã có/đã kiểm tuyến đại diện**: ghi hợp lệ mới tăng, P đánh dấu đọc ghi chép; API18 và controller37, OBS-PROGRESS-01 có kiểm lặp/save-load | **Map & Asset + UI/UX & Tích hợp:** waypoint/toạ độ quay lại xưởng và thứ tự mục tiêu trên map thật. Proxy không chứng minh tuyến đi/toạ độ xưởng thật |
| Tự gán, thử đọc/ghi mật mã Hà | GDD5.3/12.1/13; Session.Assign/ReadCipher/RecordReading; Evidence.Alphabet/HaPlain | **Đã có/đã kiểm API prototype**: trống/sai chưa grant, đúng ghi decoded duy nhất, không cấp người gửi/ending; tuyến18 có roundtrip. Controller route chỉ nhận thư qua E, chưa giải hết bằng UI | **UI/UX & Tích hợp:** thao tác tự gán/đọc/ghi bằng UI scene chung và readability; **Map & Asset:** pose/ảnh theo ID hiện có.26 ký tự/năm thư đầy đủ **ngoài giai đoạn hiện tại** |
| Xem hai mục, ghi quan hệ giả thuyết | GDD7.2; Session.AddRelation, SessionUI.CompareUI/EvidenceTranscript | **Đã có/đã kiểm API**: không mục thiếu/tự nối/ghi trống, ghi hợp lệ không thành verdict; bản chép clip cùng accessor; tuyến18 lưu/tải relation | **UI/UX & Tích hợp:** hai viewer/scroll/nhập note trên scene chung. Quan hệ không tự chấm đúng/sai, không thêm luật căn cứ ngoài dữ liệu đã giao |
| Sáu ca bàn dựng, pause/seek/clear-holds, rời/quay lại | GDD3.3/7.1; Session.StartTrial/AdvanceTrial/Update; ReviewUI.SeekTrial/ClearTrialHolds; TrialTimeline/ReturnGateMechanism | **Đã có/đã kiểm trong gói TRIAL-CONTEXT-01**, không đổi nguồn trong gói hiện tại. Tuyến18 tạo một record; controller37 kiểm rời/quay lại; không chạy lại103 ca | **Map & Asset:** hình học/collider/minh họa vật lý ngoài đời; **UI/UX & Tích hợp:** giữ pause tay, không chạy bù/trùng record trên scene chung. Code mô hình không tự xác nhận vật lý hiện trường |
| Bốn bản ghi kết luận/chưa đủ, xác nhận và chốt | GDD7.3/13; ConclusionUI; ConclusionCommit.SetConclusionConfirmation/TryFinalizeConclusions | **Hoàn tất logic prototype, đã kiểm API/controller**: d7f1c93b checkpoint trước chốt và feedback theo kết quả thực; claim/choice/grounds làm mất xác nhận. Không chấm C/A, không khóa kết luận khi GDD không quy định gate | **UI/UX & Tích hợp:** nhận toàn chuỗi, kiểm feedback/khả năng dùng/nội dung không che ở1080p. Đường truy cập restore của người chơi còn phụ thuộc thiết kế của vai trò này; API restore đã có, không tự yêu cầu nút mới |
| Save/load giữ tiến trình, guard từ chối trước đổi state | GDD11; Session.Save/LoadFromPath, SaveValidation/SaveFile | **Đã có/đã kiểm guard và roundtrip liên quan**, không đổi writer/schema/ID.86 API trước + kiểm hẹp closure; không chạy lại toàn IO. **IO-OPEN-01 vẫn OPEN** sau phiên chẩn đoán hữu hạn7d4a7868; chưa tái hiện tự nhiên, chưa đủ bằng chứng kết luận nguyên nhân/hết lỗi | **Gameplay & Dữ liệu** nhận theo dõi IO-OPEN-01 theo bước tái hiện/ảnh hưởng trong PROTOTYPE-FLOW-01; phụ thuộc tái hiện contention/exception thật ở môi trường. **UI/UX & Tích hợp:** phản hồi ghi/tải và cảnh báo không nhầm tiến trình sau lỗi trên scene chung |
| Đủ thư/khu/chuỗi căn cứ, grading và ending | GDD8/12.3–4 | **Ngoài phạm vi prototype đã chốt**, không có task mới được giao. Không tự đưa chương/ending/đủ26 tư thế vào scope | Nhóm thống nhất đầu vào nội dung và giao scope mới; **UI/UX & Tích hợp** điều phối GDD, **Gameplay & Dữ liệu** chỉ triển khai khi luật/dữ liệu được giao rõ |

Không còn logic runtime thiếu có yêu cầu rõ trong checklist prototype sau hai nguồn e0544b74/d7f1c93b. Các hàng ghi “API” không được đổi thành toàn tuyến UI PASS; các phụ thuộc phía phải chưa hoàn tất và giữ owner, không phải lý do mở chức năng mới. IO-OPEN-01 là rủi ro nghiệm thu độ tin cậy ghi bình thường còn theo dõi, không đóng bằng một lượt đạt. Mức ảnh hưởng/bước nhận cụ thể ở báo cáo cùng task, không mở audit toàn save.


### Kết quả closure và bàn giao hiện hành

- Không thêm/sửa runtime vì checklist prototype không còn logic thiếu có yêu cầu rõ sau e0544b74/d7f1c93b. Đã hoàn tất kiểm còn thiếu của sửa nội dung kết luận và lỗi ghi qua UI.
- API closure36/36 trên45e5a2b4; controller/UI closure55/55 trênc8fc4552, event Unity/virtual keyboard trong proxy Gameplay/profile mới,1600×1099; không input Windows/scene chung/người mới. Lượt UI đầu29/31 dừng do selector chưa làm đổi choice giữ riêng. Không chạy lại86 API hoặc bộ save/trial cũ.
- Chuỗi bổ sung: a53a8512 →45e5a2b4 →c8fc4552 công cụ, cùng báo cáo PROTOTYPE-FLOW-01 cập nhật. Runtime tree nguyên d7f1c93b; các số kiểm mới không được cộng thành tổng ca độc lập với kiểm cũ.
- IO-OPEN-01 vẫn OPEN, chủ Gameplay & Dữ liệu: ghi/chốt bình thường có lần thất bại trên chính runtime bàn giao; thiếu hash/exception gốc tại lỗi ngoài dự kiến, không tuyên bytes/state mọi lượt lịch sử đã được chứng minh giữ nguyên. Sáu lượt resume mới đạt không giải thích nguyên nhân. Đã ghi tái hiện, ảnh hưởng nếu F6 sau ghi lỗi, dữ liệu cần giữ và điều kiện khép đúng phạm vi.
- Bàn giao đủ5 nội dung clip/ghi/xác nhận/IO/restore, contract API và owner/acceptance ở [PROTOTYPE-FLOW-01 mục7–9](../QA/Tasks/N2/PROTOTYPE-FLOW-01/README.md). UI/UX & Tích hợp nhận scene chung,1080p, feedback, đường truy cập restore nếu thiết kế đưa vào tuyến; không tự yêu cầu thêm nút. Map & Asset nhận collider/media/hình học thực.
- Các phần runtime rõ đã đóng triển khai/kiểm có phạm vi; phần chưa khép có owner/phụ thuộc cụ thể, không tự mở nhiệm vụ mới. Không chốt baseline, sửa scene chung/UI ngoài ownership, map/asset/save gốc; năm file Unity đã bảo toàn trước/sau rồi phục hồi đúng bytes trước kiểm; untracked cũ giữ nguyên. Profile/log/capture local không commit.

### IO-OPEN-01 — acceptance chẩn đoán giới hạn

Xuất phát777a59de, tracked sạch, hai untracked cũ giữ nguyên; snapshot Test/Output/IoOpen20261109. Chỉ xử lý lỗi ghi bình thường chưa giải thích: các profile cũ ConclusionFixed20261109, FinalAffectedApi20261109, FinalIoDiagnosis20261109 trong Test/PlaytestProfiles; nguồn thực d7f1c93b/71b20f71. Không suy từ PASS về nguyên nhân hoặc mọi bytes lịch sử.

Một phiên API chẩn đoán hữu hạn:6 chuỗi ghi/chốt/resume không chèn lỗi; chuỗi cũ có ba lỗi được gắn nhãn rõ (đích khóa, thư mục chặn tmp, checkpoint khóa) và các thao tác phục hồi tường minh; một chuỗi writer thực gọi trực tiếp bằng reflection để thu exception/type/HResult/stack mà Session bắt và chuyển thành Message. Direct-writer không được gọi là UI/Load của session. Mỗi bước ghi SHA main/checkpoint/tmp và state trước/sau; lỗi bình thường không tự retry/xóa kết quả. Chỉ thư mục/profile mới. Không chạy bộ save/API/UI cũ, không lặp vô hạn. Nếu không tái hiện lỗi tự nhiên, giữ OPEN và chốt điều kiện phát hiện/giảm rủi ro/mở lại, không sửa runtime theo giả thuyết.


### IO-OPEN-01 — kết luận lượt chẩn đoán hữu hạn

- Công cụ/meta7d4a7868, đúng commit đã chạy Unity6000.6.2f1, proxy Gameplay/API, profile IoOpenFinite20261109. Không UI/controller/input Windows. Runtime tree giữ nguyên d7f1c93b; không sửa guard/writer/schema/ID.
- Một phiên36 thao tác:32 ghi thành công,3 từ chối qua session và1 exception direct-writer đều là fault chủ động có nhãn;11 Load hợp lệ/read-only;0 lỗi ngoài dự kiến/0 vi phạm được kiểm. Sáu resume không fault, chuỗi trộn lỗi/recovery giống ca cũ, tám cặp writer trực tiếp; không lặp vô hạn hoặc chạy bộ cũ.
- Direct calibration thu IOException0x80070020/File.Replace do chính fixture giữ FileShare.None; KHÔNG gán đó cho lỗi lịch sử. Fixture roundtrip cũ đã có lỗi tiền điều kiện được df2a1a83 sửa, nhưng nguyên nhân thao tác ghi không hoàn tất vẫn chưa rõ. Message chốt chung không chứng minh exception thật là IO vì catch còn bao trùm clone/capture/apply.
- Bảy file save thử lịch sử hiện parse JSON được, đã hash chỉ đọc; không có snapshot/hash đúng lúc lỗi nên không suy mọi bytes/state ở thời điểm đó đã giữ. Không đụng save gốc hoặc tự sửa các profile cũ.
- **OPEN/chưa tái hiện tự nhiên, không CLOSED.** Chủ Gameplay & Dữ liệu nhận lượt thất bại không injection cùng SHA/state/source/time/exception/profile để xác định bước lỗi; chưa có bằng chứng lỗi nguồn nên không sửa runtime theo giả thuyết. Nếu Save/chốt thất bại, không F6 ngay hoặc coi đã lưu bản mới; giữ phiên/file/tmp và xử lý điều kiện lỗi đã xác minh trước khi thử lại tường minh.
- [PROTOTYPE-FLOW-01 mục10](../QA/Tasks/N2/PROTOTYPE-FLOW-01/README.md): điều kiện đã thử, ảnh hưởng, phát hiện/giảm rủi ro, điều kiện nhận mới và owner. UI/UX & Tích hợp nhận feedback scene chung; các phụ thuộc ở checklist/mục9 giữ nguyên, không phát sinh UI mới. Phần chẩn đoán lượt này hoàn tất có giới hạn; phần nguyên nhân/độ tin cậy IO chưa đóng. Raw log/save/state local, chỉ commit công cụ và bằng chứng đã lược.

## Hai gói viewer — acceptance trước triển khai

Xuất phát b3c1bb06, branch work/runtime-contracts-baseline, tracked sạch; untracked cũ giữ nguyên. GDD9 yêu cầu xoay/phóng/đặt lại khi xem vật và cho đổi góc khi so hai vật; GDD6.5 yêu cầu phóng to/mô tả khi so mẫu giày/vải, không đòi kéo khớp pixel. Source hiện viewer một vật có điều khiển chuột trong Update, không command đặt lại/giao diện kết quả; CompareUI chỉ có text, chưa có hai viewer. Đây là thiếu chức năng viewer prototype trong checklist, không mở chương/ending/luật suy luận.

1. **INSPECT-CONTROLS-01**: API đọc pose/texture/trạng thái và xoay/phóng/đặt lại viewer hiện có, trả lỗi khi không có vật/viewer, sai ngữ cảnh hoặc pause, từ chối số NaN/Infinity; clamp giới hạn cũ, default theo ID cũ. Mouse runtime dùng cùng biên điều khiển, pause không làm đổi view. Event snapshot chỉ đọc, không phụ thuộc nút/Hierarchy UI. Không auto Observe/Acquire hoặc đổi data/schema; Load/reopen dùng default cũ, không tự lưu camera pose. Kiểm repeat/reset/context/pause/error/save-load và input mouse ảo nếu khả thi. Không sửa SessionUI hoặc scene chung; API mới cần UI wiring cho action đặt lại.
2. **COMPARE-VIEW-01**: runtime hai preview độc lập cho các ID đã có, xoay/phóng/reset từng bên bằng cùng pose/range hiện hành, giữ source model không đổi và clone vô hiệu physics/scripts; cung cấp texture/mô tả đã qua EvidenceTranscript guard. Không tự cấp dữ liệu/ghi relation/chấm kết luận. ID thiếu/chưa thu/model thiếu/paused/sai trang trả lỗi có ý nghĩa. Hủy/đổi lựa chọn/rời trang/load phải dọn hoặc làm mất hiệu lực view để không lộ mẫu cũ. Không lưu pose/texture/selection mới vào schema; note/quan hệ vẫn dùng dữ liệu cũ. Kiểm độc lập từng bên, đường lỗi/đổi/hủy/quay lại, lifecycle và roundtrip state. UI sản phẩm chưa gọi: phải bàn giao wiring rõ và trạng thái CHỜ UI/TÍCH HỢP.

Mỗi gói có nguồn, tool/bằng chứng và một README bàn giao riêng trong Docs/QA/Tasks/N2. Sau kiểm chuyển RUNTIME ĐÃ KIỂM → CHỜ UI/TÍCH HỢP; chỉ phía tích hợp có báo cáo mới ghi TÍCH HỢP ĐÃ KIỂM. IO-OPEN-01 theo dõi riêng, không chạy audit save lại. Kích thước/độ sáng/điểm đặt action thuộc UI/UX & Tích hợp; mẫu/render/media thực phụ thuộc Map & Asset, không sửa thay họ.


### Danh sách chờ tích hợp — viewer

| Gói | Nguồn / commit thực kiểm | Runtime | Trạng thái tích hợp / chủ nhận | Phụ thuộc và bằng chứng |
|---|---|---|---|---|
| INSPECT-CONTROLS-01 | b8cc108f /61f26ad0 | RUNTIME ĐÃ KIỂM,27/27 API/fixture | CHỜ UI/TÍCH HỢP — UI/UX & Tích hợp | Action reset/state/errors/event, native mouse/preset/focus và model thật; Map & Asset hỗ trợ source. [Handoff](../QA/Tasks/N2/INSPECT-CONTROLS-01/README.md). Chưa có báo cáo tích hợp, không ghi TÍCH HỢP ĐÃ KIỂM |

Gói đã triển khai reset/bounded controls, không chỉ audit/API giả; caller UI sản phẩm chưa nối nên không tuyên người chơi scene chung đã dùng được. Giữ lượt mouse-frame chưa tới và lỗi compile công cụ0720006e riêng, không sửa lịch sử để đổi PASS. Save/profile/log local không commit. Tiếp theo thực hiện COMPARE-VIEW-01 độc lập đã ghi acceptance, không chờ UI nhận gói trước và không thay public API gói đã giao.

COMPARE-VIEW-01 — làm rõ acceptance trước source: để giữ chức năng đối chiếu ghi chú của GDD7.2, ID đã có nhưng chưa có model vẫn chọn được ở trạng thái MissingVisual/text-only; không dùng ảnh vật trước. Lệnh xoay/reset bị từ chối có lý do. ID chưa thu/sai/side sai hoặc chọn khi pause/sai trang bị từ chối và giữ lựa chọn cũ. Không biến thiếu asset thành luật khóa điều tra mới. Snapshot visual chỉ sao mesh/line/baked skin và transform, không Instantiate script/physics nguồn rồi để Awake chạy.


### Kết quả hai gói viewer — 10/11/2026

Chức năng người chơi được hỗ trợ: đặt lại vật đang xem và xoay/phóng/đặt lại riêng từng vật trong đối chiếu. Đã triển khai runtime thật; phần UI mới **chưa nối**, không gọi là hoàn tất toàn tuyến người chơi.

| Gói | Nguồn / commit thực kiểm | Runtime | Trạng thái / chủ nhận | Phụ thuộc và bàn giao |
|---|---|---|---|---|
| INSPECT-CONTROLS-01 | b8cc108f /61f26ad0; hồi quy d671a845 | RUNTIME ĐÃ KIỂM,27/27 mỗi lượt cuối có phạm vi | CHỜ UI/TÍCH HỢP — UI/UX & Tích hợp | [API/reset và input lifecycle](../QA/Tasks/N2/INSPECT-CONTROLS-01/README.md); native input/preset/focus và action reset chưa kiểm scene chung |
| COMPARE-VIEW-01 |5ef32f00 +0b783b8c /d671a845 | RUNTIME ĐÃ KIỂM,43/43 API/fixture render; inspection27/27 hồi quy | CHỜ UI/TÍCH HỢP — UI/UX & Tích hợp | [API, model hai bên, event, ví dụ và wiring](../QA/Tasks/N2/COMPARE-VIEW-01/README.md); Map & Asset hỗ trợ model/shader/source đúng ID |

Trạng thái theo quy ước: **RUNTIME ĐÃ KIỂM → CHỜ UI/TÍCH HỢP → TÍCH HỢP ĐÃ KIỂM**. Chưa có báo cáo phía tích hợp của hai gói nên không ghi trạng thái cuối. Scene chung/1080p/người mới NOT_RUN với nguồn mới; P-close OPEN/chưa tái hiện và IO-OPEN-01 OPEN giữ riêng.

Chuỗi nguồn/công cụ comparison:5ef32f00 →23fc0cb7 →175a248c →0b783b8c →49c242a6 →d671a845. Nguồn camera chưa cô lập bị assertion FAIL ở175a248c đã sửa0b783b8c; lượt49c242a6 pixel render đạt nhưng assertion0MonoBehaviour sau render quá rộng làm dừng, công cụ sửa d671a845 và kiểm lại43/43. Giữ lịch sử từng lượt, không gộp thành số ca độc lập hoặc suy mọi asset/UI PASS. Thử bằng API/proxy, fixture cấp vật/flags clip; inspection mouse là thiết bị ảo + gọi Update trực tiếp, không input Windows. Không sửa UI/scene/map/save gốc, không chạy lại bộ save/trial cũ.

Checklist trước đó mô tả tuyến luật prototype có sẵn; hai khoảng viewer rõ từ GDD9/6.5 nay đã đóng triển khai runtime. Tương tác/ghi/mục tiêu/mật mã/quan hệ/bàn dựng/kết luận/save giữ hiện trạng trong checklist, chưa mở chương/ending/grading/26 tư thế. Phần còn thiếu để người chơi sử dụng hai viewer là adapter/render/input UI, model thật và kiểm scene chung, có owner ở bảng trên. Khôi phục checkpoint vẫn chờ UI/UX & Tích hợp chốt đường truy cập theo GDD, không tự yêu cầu nút mới. Chưa có gói runtime độc lập tiếp theo có yêu cầu rõ trong checklist hiện tại; không tạo chức năng ngoài GDD để kéo dài việc. Tiếp tục phần phối hợp khi có API/asset/UI request cụ thể; không dùng trạng thái chờ này để tuyên toàn game hoàn tất.


### Đợt Gameplay & Dữ liệu kế tiếp — đề xuất 10/11/2026

Đối chiếu GDD/code tại74a80c21, chỉ đọc source, không chạy Unity/kiểm lại save hoặc xác minh push. Kết luận trước “chưa có gói độc lập tiếp theo” được bổ sung bằng hai khoảng thiếu dưới đây; **chưa có runtime mới**. [Đề xuất và acceptance đầy đủ](../QA/Tasks/N2/DE_XUAT_DOT_GAMEPLAY_TIEP_THEO.md).

| Gói mới | Khoảng thiếu runtime | Trạng thái / chủ phụ thuộc |
|---|---|---|
| **NOTE-PROVENANCE-01 — ưu tiên** | Metadata từng entry và truy vấn có kiểu để phân biệt quan sát/lời kể/giả thuyết theo GDD7.2/11; hiện có source text, observations chung và relations tách riêng | ĐỀ XUẤT, chưa triển khai/kiểm. Gameplay & Dữ liệu nhận runtime; UI/UX & Tích hợp review mapping nội dung và nối hồ sơ. Dự kiến không đổi schema/ID/index |
| INTERVIEW-REVISIT-01 | Hỏi lại Hà theo tài liệu đã ghi/đọc (GDD4/6.7), thay vì chỉ lặp Interact/OpenEvidence tĩnh | ĐỀ XUẤT, chờ nhóm chốt scope prototype hay lát cắt, câu hỏi/đáp án, điều kiện mở và lịch sử cần lưu. UI/UX & Tích hợp điều phối nội dung; Gameplay & Dữ liệu nhận runtime sau khi rõ |

Ưu tiên NOTE-PROVENANCE-01 vì có thể làm với dữ liệu prototype, không thêm cốt truyện/asset/khu/ending và có đường kiểm độc lập không đổi save; đây là phần ngữ nghĩa runtime còn thiếu, không chỉ đổi nhãn UI. Không tự coi phân loại nguồn là chứng minh lời kể đúng hoặc thêm gate chốt. Các entry lai phải review, không phân loại toàn NPC theo một loại nguồn. Trạng thái hai gói viewer trước giữ CHỜ UI/TÍCH HỢP tại74a80c21, không báo lại hoặc suy thêm PASS. Lượt này chưa sửa runtime, chạy Unity, push/merge hoặc mở lại audit IO.


### NOTE-PROVENANCE-01 — nhận triển khai

Được giao10/11/2026, base afe461c2; tracked sạch, untracked cũ bảo toàn. Mục tiêu: metadata theo ID/index và read model cho ghi nhận đã mở, loại Quan sát/Lời kể/Giả thuyết; mơ hồ giữ CHỜ REVIEW NỘI DUNG, thiếu mapping không mặc định Quan sát. [Acceptance và bảng từng entry trước source](../QA/Tasks/N2/NOTE-PROVENANCE-01/README.md). Chỉ thêm runtime partial mới/test Gameplay/Editor/contracts riêng, không sửa UI/scene/EvidenceDefinition/save types hoặc luật mở chứng cứ. Kiểm API/schema1/Observe liên quan, không bộ save đầy đủ. Phụ thuộc UI/UX & Tích hợp: review mục mơ hồ và nối labels/nhóm; không triển khai hội thoại.


### NOTE-PROVENANCE-01 — kết quả và chờ nhận

Đã cung cấp metadata34 entry (28 rõ/6 CHỜ REVIEW NỘI DUNG), nhãn Quan sát/Lời kể/Giả thuyết/Chưa phân loại và snapshot query chỉ cho dữ liệu đã ghi. Source386fdfdc →tool/meta e0b9e58a →bảng/acceptance57c6ad35; thực chạy Unity6000.6.2f1 tại57c6ad35, **68/68 API/fixture đạt**, không UI/controller/Windows/natural play. Ghi lặp/đổi text/guard clip/query không mutate, relation/claim không nâng thành sự thật, save thử schema1 trước gói và roundtrip hiện hành đạt; không sửa schema/types/ID/GUID cũ hoặc producer/validator. Không chạy các bộ IO/trial cũ.

| Gói | Commit nguồn / đã kiểm | Runtime | Chờ tích hợp / chủ nhận |
|---|---|---|---|
| NOTE-PROVENANCE-01 |386fdfdc /57c6ad35 | RUNTIME ĐÃ KIỂM,68/68 API/fixture | CHỜ UI/TÍCH HỢP — UI/UX & Tích hợp nối metadata/nhãn hồ sơ; review mechanism:1,hook:0,samples:0,letter:1,draft:0,decoded:0 và trial report |

[Bàn giao đầy đủ và bảng metadata](../QA/Tasks/N2/NOTE-PROVENANCE-01/README.md); UI cần wiring mới qua reference session thật, không lấy raw definition để lộ ghi nhận chưa mở, không sửa partial song song. Không có event mới; refresh query theo lifecycle/action hiện có. Trạng thái TÍCH HỢP ĐÃ KIỂM chỉ cập nhật khi có báo cáo nhận. Mục mơ hồ vẫn Unclassified/pending, không chặn UI dùng28 entry rõ và không giả nhận đã review xong. Scene chung/UI1080p/người mới NOT_RUN; IO-OPEN-01/P-close OPEN riêng. INTERVIEW-REVISIT-01 chưa triển khai.


### INTERVIEW-REVISIT-01 — nhận cơ chế độc lập

Được giao từfc781aaa, tracked sạch, untracked cũ giữ nguyên. [Acceptance trước nguồn và trích GDD](../QA/Tasks/N2/INTERVIEW-REVISIT-01/README.md). Chỉ catalog/chủ đề/query/select/error/repeat/context/event, điều kiện dữ liệu tường minh trong fixture TEST ONLY ở Editor; không thoại/gate chính thức Hà, thưởng, state lịch sử/save field, UI/map/scene chung. Nội dung và history chờ quyết định; tiếp tục kiểm cơ chế độc lập. Sáu entry provenance và việc review kết quả bàn dựng vẫn do UI/UX & Tích hợp/nội dung nhận, không chốt lại nhãn hoặc kiểm/push lại gói đó.


### INTERVIEW-REVISIT-01 — kết quả cơ chế và chờ nội dung/UI

Nguồn97b8048e →e92a2ea6; công cụ/meta d5b67c9a là commit thực chạy, **48/48 API/fixture đạt**, Unity6000.6.2f1/scene Gameplay/proxy/profile mới. Cơ chế topic immutable, query/select recheck, lỗi khóa/sai, repeat không grant/state/history, event và context hết hiệu lực sau Load đã kiểm; không UI/controller/Windows/natural play. Chỉ một Save/Load liên quan context, không bộ save/IO/trial/provenance cũ. Không sửa UI/scene/map/EvidenceDefinition/session producer/save types và không có catalog thoại thử trong source sản phẩm.

| Gói | Nguồn / đã kiểm | Trạng thái | Chủ nhận và phụ thuộc |
|---|---|---|---|
| INTERVIEW-REVISIT-01 |97b8048e +e92a2ea6 /d5b67c9a | **CƠ CHẾ ĐÃ KIỂM / CHỜ NỘI DUNG & UI** | UI/UX & Tích hợp duyệt Question/Answer/participant-topic ID/gate/history, nối consumer/modal; Gameplay & Dữ liệu nhận yêu cầu partial/contract cụ thể |

[Bàn giao đầy đủ](../QA/Tasks/N2/INTERVIEW-REVISIT-01/README.md). Chưa có hội thoại Hà hoàn chỉnh, chưa lưu lịch sử hỏi, không tự thêm phần thưởng/ghi nhận khi hỏi. Cần review6 entry provenance và **kết quả bàn dựng/trial report** bởi phía nội dung như đầu việc riêng; giữ nhãn pending, không chốt thay hoặc kiểm/push lại gói cũ. Scene chung/UI1080p/người mới NOT_RUN, P-close/IO-OPEN-01 OPEN riêng. Không mở thêm hệ thống/ending/nhánh và không merge/chốt baseline nhóm.
