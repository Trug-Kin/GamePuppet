# Ownership — r107, ba người

Không phát triển tính năng trong đợt chuẩn bị này. Module là ranh giới trách nhiệm, **chưa phải ba assembly/game độc lập**. Giữ đường dẫn legacy khi chưa đủ căn cứ move.

| Người | Sở hữu | Không tự sửa |
|---|---|---|
| 1 — Map/asset | `Assets/ConRoi/Modules/Map`, `NgheThuat`; legacy `Assets/Art`, `Assets/Model`, `Assets/Blender`, `Source` | Scene chính; script r107; ID vật chứng |
| 2 — Runtime/contracts | `Gameplay`, `Contracts`; legacy `Assets/Scripts` trừ thư mục Editor; **toàn bộ partial InvestigationSession r107 tạm thời cùng một chủ** | Scene chính; nội dung GDD đã chốt |
| 3 — UI/nội dung/tích hợp | `GiaoDien`, `NoiDung`, `TichHop`; `Assets/Scenes`; GDD, Docs/QA và điều phối tài liệu | Không trực tiếp sửa partial UI r107 đồng thời với người2; gửi đề nghị/patch để người2 tích hợp |

`InvestigationSession.cs`, `InvestigationSessionUI.cs`, `InvestigationReviewUI.cs`, `InvestigationOnboarding.cs` dùng private state chung. Chưa tách adapter trong đợt dọn để tránh đổi hành vi/save. Người3 làm thử UI trong scene/thư mục riêng; mọi thay đổi partial r107 đi qua người2. Đây là ownership tạm, không tuyên bố đã modular hóa runtime.

Legacy `Assets/Scripts/Editor` và `Assets/Scripts/KiemThuLichSu`: người1 giữ tool asset, người2 review compile/API; chỉ một người nhận task bằng DANG_LAM trước sửa. Không archive lớp C# vì không có reference scene: chúng tham gia compilation và còn consumer theo tên.

Chỉ người3 merge scene chính và cập nhật lịch sử chung. Người1/2 viết báo cáo riêng theo task; không cùng commit `Docs/PROJECT_GUIDE_AND_HISTORY.md`, workflow, BAT_DAU hoặc scene chính. File `.meta` đi cùng asset; không sửa GUID. Chưa biết username GitHub của ba người nên **không tạo CODEOWNERS có tên giả**. Tạo khi nhóm xác nhận tài khoản và remote hỗ trợ.
