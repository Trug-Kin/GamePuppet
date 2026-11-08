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
