# Giao máy r106 — READY, người thật NOT_RUN

Ngày chuẩn bị: **06/11/2026**. Giữ nguyên sản phẩm r106; không tạo revision mới. Chưa có người thật chơi, chưa có phản hồi hay kết quả phiên.

- Unity **6000.6.2f1**, scene `Assets/Scenes/JourneyPresentation_v48_r106.unity`.
- Slot **PT001**, chưa gán cho người thật. Profile: `D:\ConRoi\Test\PlaytestProfiles\PT001_r106`.
- Save thử: `InvestigationV36\r106\prototype.json` trong profile trên; schema 1, namespace `conroi.investigation.v36.r106`.
- Đã mở lại Editor bằng tham số profile thật. Hồ sơ ban đầu trống, không tự tải hồ sơ kỹ thuật. F5/F6 bằng bàn phím tổng hợp đã lưu/tải hồ sơ trống đúng thư mục, không gán đáp án hoặc gọi API Save/Load. Xem [kết quả](start-profile-check.txt) và [kiểm Editor](editor-start-check.txt).
- Sau kiểm, Editor được đặt Play tạm dừng ở điểm bắt đầu; **Esc để bắt đầu**. [Ảnh khởi động trước khi tạm dừng](../../../BangChung/investigation_r106/playtest_handoff/start-screen.png) là bằng chứng kỹ thuật, không phải phiên người thật.

Trước giao máy, xác nhận Editor vẫn ở trạng thái chuẩn bị này. Nếu PT001 đã có tiến trình hoặc không chắc ai đã dùng, giữ profile đó và chọn slot chưa dùng theo hướng dẫn; không xóa save. Không dùng các profile kỹ thuật api/ui/regression/profile-launch-verified làm phiên người mới.

## Tài liệu giao đúng người

- [Người tổ chức](../NGUOI_TO_CHUC.md): mở Editor, chọn profile, kiểm đầu phiên và xử lý lỗi.
- [Chỉ giao trang này cho người chơi](../HUONG_DAN_NGUOI_CHOI.md): mục tiêu và điều khiển, không chứa đáp án.
- [Phiếu trung tính gốc](../PLAYTEST_NGUOI_MOI.md).
- [Mẫu ghi nhận phiên](../MAU_GHI_NHAN_PHIEN.md) và [mẫu tổng hợp vấn đề](../MAU_TONG_HOP_VAN_DE.md).
- [Sổ phiên](SESSIONS.md): hiện chưa có phiên người thật.

Chỉ kiểm nhanh khởi động trong đợt này; không chạy lại sáu ca, toàn UI hoặc tuyến Hà. Không thay sản phẩm, map, art, controller hoặc migration. Không tạo build/ZIP/runner. Log Editor là file đang ghi tiếp; không coi hash của nó là bất biến.

Đối chiếu cuối: **1.732/1.732 file được liệt kê giữ nguyên hash**, gồm **17 save gốc đang theo dõi**; không suy rộng thành toàn bộ dữ liệu. Xem `protected-final.json` và `delivery-manifest.json`. Các trang bàn giao mới đọc lại UTF-8 và liên kết nội bộ có đích; một số liên kết lịch sử cũ trong tài liệu chung không còn đích, không thuộc đường dẫn bắt đầu phiên này và không được sửa trong đợt.

Kết thúc chuẩn bị tại đây. Chờ người thật, ghi trung thực lời nói và thao tác; phân tích vấn đề cụ thể trước khi đề nghị sửa. Không lấy agent biết GDD thay người mới.
