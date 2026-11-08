# Chốt nhận việc 08/11/2026 — fcbb7e19 / biên nhận 55d7f04

Kết quả hiện hành: [bàn giao nhóm](TeamHandoff20261108/README.md), [audit Git/LFS](TeamHandoff20261108/audit.json).
Ba nhánh task đều chứa baseline fcbb7e19, không có commit riêng tại lúc kiểm; không đổi ref trong đợt này. Những thông báo BLOCKED/a209c0e phía dưới thuộc lịch sử, đã được biên nhận NAV-01 thay thế.

## File giao nhau: một chủ sửa tại một thời điểm

| File / phạm vi | Chủ sửa duy nhất | Phối hợp |
|---|---|---|
| Map/NgheThuat, legacy Art/Model/Blender, Source, meta đi cùng | Người 1 | Người 3 chỉ wire scene; báo cáo topology của N1 ở Docs/QA/Tasks/N1 |
| Toàn bộ Assets/Scripts, gồm InvestigationV36R107 partial UI/onboarding, Editor và KiemThuLichSu | Người 2 | Người 3 gửi đề xuất UI; tool asset chỉ chuyển từng file cho N1 sau khi ghi DANG_LAM |
| Tools (gồm 852 script lịch sử), compile/asmdef và HOP_DONG_MODULE | Người 2 | Tool chia sẻ chỉ một chủ; không chạy lại script một lần |
| GiaoDien/NoiDung/TichHop, scene tích hợp và scene legacy được phép thay | Người 3 | N1/N2 giao prefab/patch + chỉ dẫn; checkpoint r101–r107 không ai sửa |
| GDD, BAT_DAU, README gốc, lịch sử/workflow, HANDOFF, OWNERSHIP, TICH_HOP, registry QA chung | Người 3 | Chỉ cập nhật từ review/kết quả thật; đổi GDD cần quyết định thiết kế, không mở scope |
| Packages, ProjectSettings, .gitignore/.gitattributes, cấu hình Git nhóm | Người 2 | Đóng băng trong task đầu; thay cần review N3, pipeline/package không tự nâng |
| DANG_LAM_NGUOI_1/2/3, báo cáo Docs/QA/Tasks/N1/N2/N3 | Người tương ứng | Người 3 tổng hợp vào lịch sử chung, không ba người cùng commit một lịch sử |
| Scene thử Map / Gameplay / GiaoDien | Người 1 / 2 / 3 tương ứng | Scene rỗng làm sandbox; không tự ghi đè scene tích hợp |
| File chưa có quy tắc, ngoài 4.183 legacy chưa xác minh | Người 3 điều phối | Chưa sửa cho tới khi giao một chủ bằng DANG_LAM; nhóm D và archive giữ nguyên |

Quy tắc bảng này thay phần phân chủ Editor/test mơ hồ ở ghi chép cũ bên dưới. Chuyển quyền phải ghi rõ file, người giao/nhận và commit, tạm dừng sửa ở chủ cũ. Meta thuộc cùng chủ asset. CODEOWNERS chờ username thật; không tạo tên giả.

<!-- nav01-fixed-20261108 -->
> 08/11: [NAV-01 kiểm đạt trên nguồn 074caf3](NavFix20261108/README.md). Lấy baseline nhánh/task và commit báo cáo thực tế từ HANDOFF/biên nhận mới nhất; trạng thái BLOCKED cũ là lịch sử. Ownership/CHUA_NHAN giữ nguyên, người mới NOT_RUN; không push.

<!-- checkout-final-a789222-20261107 -->
> Kết quả mới nhất: [kiểm a789222](CheckoutFinal20261107/README.md), Git/LFS/import/reference đạt; tổng thể BLOCKED_UI_REGRESSION (P sau nhập mật mã). Ba task branch giữ a209c0e, ownership không đổi; không push. Các trạng thái checkout cũ bên dưới là lịch sử.

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
