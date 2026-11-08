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
