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
