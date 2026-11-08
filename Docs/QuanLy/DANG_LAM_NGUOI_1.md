# Người 1 — Map/asset

Trạng thái: **CHUA_NHAN**. Nhánh chính xác: `work/map-asset-baseline`.

Unity **6000.6.2f1**. Baseline nhận việc **fcbb7e19c094a2a52d99237d03587df16679d579**; biên nhận **55d7f04**.
Entry: `Assets/ConRoi/Modules/TichHop/Scenes/ConRoi_Chinh.unity`, chỉ người 3 được sửa.
Checkpoint `Assets/Scenes/JourneyPresentation_v48_r107.unity` giữ nguyên. Scene thử dưới đây là **template rỗng**, không phải bản game độc lập.
Mỗi người dùng checkout/Library riêng; không dùng chung project đang mở. Đọc [quyền sở hữu](OWNERSHIP.md), [tích hợp](TICH_HOP.md) và [hợp đồng hiện hành](HOP_DONG_MODULE.md).
Không đổi GUID/meta, schema/save ID, pipeline/packages; không chạy author/derive/bind một lần. Không dọn 4.183 legacy hoặc archive.

Được sửa trong task đã khai báo: `Assets/ConRoi/Modules/Map/`, `Assets/ConRoi/Modules/NgheThuat/`, legacy `Assets/Art/`, `Assets/Model/`, `Assets/Blender/`, `Source/` và meta tương ứng. Báo cáo riêng: `Docs/QA/Tasks/N1/`.
Không sửa scene tích hợp/checkpoint, GDD/lịch sử chung, `Assets/Scripts/`, Packages/ProjectSettings hoặc dữ liệu người dùng. Tool asset cần chuyển quyền từng file từ người 2 trước khi sửa.
Scene thử: `Assets/ConRoi/Modules/Map/Scenes/Map_ThuNghiem.unity`.

## Việc đầu tiên

Đọc [MAP_REPAIR](../QA/investigation_r107/MAP_REPAIR.md). Lập bảng topology còn mở và tái hiện ba góc ảnh **105506/105520/105534**. Góc 105627 đã sửa winding có phạm vi; không gọi ba góc bổ sung là matched trước/sau. Ảnh gốc tại `D:/ConRoi/Test/BugModel` chưa tracked: nhờ người tổ chức cấp đúng bốn ảnh theo index, không giả định có trong clone. Tài liệu/bằng chứng khác tra manifest archive trong HANDOFF.

Nguồn hiện hành `Source/Blender/JourneyV48/R107/Terrain_R107_Editable.blend`; FBX `Assets/Art/Environment/JourneyV48/TerrainR107/TerrainR107_Final.fbx`.
Ưu tiên phân biệt mặt thiếu/winding, topology rời, collider và góc camera. Giữ FAIL source-delivery-gate và 8 tam giác rất nhỏ như vấn đề cần phân tích; không tự weld terrain theo tiêu chí closed-prop. Không san toàn map hoặc đổi nước/nền chợ đã chốt.

Hoàn tất task đầu khi có từng mục: camera/player, object/GUID, nguồn, ảnh, collider, mức ảnh hưởng và đề xuất sửa có giới hạn; ghi chỗ chưa tái hiện được. Chỉ sửa sản phẩm trong task tiếp được duyệt theo bằng chứng.
Bàn giao commit nhỏ + báo cáo riêng + hướng dẫn đặt asset cho người 3; nếu sửa geometry ở task sau phải theo Blender editable → FBX → Unity, giữ GUID và kiểm collider/góc ảnh chịu ảnh hưởng. Không tự commit scene chính hoặc tuyên bố toàn map PASS.

Prompt nhận việc: “Trên work/map-asset-baseline từ fcbb7e19, triage topology và ba góc ảnh chưa khớp theo MAP_REPAIR; ghi đúng object/camera/source/collider, không mở rộng sửa map khi chưa có lỗi xác nhận.”

## Điền khi nhận

- Người nhận / ngày:
- HEAD thực tế / file sẽ chạm:
- Chuyển quyền file giao nhau đã thống nhất:
- Kiểm và bằng chứng:
- Commit kết quả / giới hạn / người nhận bàn giao:
