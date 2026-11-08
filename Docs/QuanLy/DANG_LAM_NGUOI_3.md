# Người 3 — UI/UX/nội dung/tích hợp

Trạng thái: **CHUA_NHAN**. Nhánh chính xác: `work/ui-onboarding-review`.

Unity **6000.6.2f1**. Baseline nhận việc **fcbb7e19c094a2a52d99237d03587df16679d579**; biên nhận **55d7f04**.
Entry: `Assets/ConRoi/Modules/TichHop/Scenes/ConRoi_Chinh.unity`, chỉ người 3 được sửa.
Checkpoint `Assets/Scenes/JourneyPresentation_v48_r107.unity` giữ nguyên. Scene thử dưới đây là **template rỗng**, không phải bản game độc lập.
Mỗi người dùng checkout/Library riêng; không dùng chung project đang mở. Đọc [quyền sở hữu](OWNERSHIP.md), [tích hợp](TICH_HOP.md) và [hợp đồng hiện hành](HOP_DONG_MODULE.md).
Không đổi GUID/meta, schema/save ID, pipeline/packages; không chạy author/derive/bind một lần. Không dọn 4.183 legacy hoặc archive.

Được sửa theo task: `Assets/ConRoi/Modules/GiaoDien/`, `NoiDung/`, `TichHop/`; duy nhất người 3 sửa scene tích hợp, GDD/tài liệu chung và Docs/QA. GDD chỉ đổi sau quyết định thiết kế có review; task đầu không mở rộng nội dung.
Không sửa map/art, save/profile gốc, runtime/contracts hoặc bất kỳ partial `Assets/Scripts/InvestigationV36R107/` (kể cả UI/onboarding); gửi đề nghị/patch cho người 2. Checkpoint vẫn bất biến dù Assets/Scenes do người 3 quản lý.
Scene thử: `Assets/ConRoi/Modules/GiaoDien/Scenes/GiaoDien_ThuNghiem.unity`.

## Việc đầu tiên

Tổ chức **người chưa đọc GDD** chơi thử trước khi mở rộng onboarding. Dùng [phiếu trung tính](../QA/investigation_r107/PLAYTEST_NGUOI_MOI.md), [hướng dẫn chỉ cho người chơi](../QA/investigation_r107/HUONG_DAN_NGUOI_CHOI.md), mẫu [ghi phiên](../QA/investigation_r106/MAU_GHI_NHAN_PHIEN.md) và [tổng hợp vấn đề](../QA/investigation_r106/MAU_TONG_HOP_VAN_DE.md).

Đóng Editor của checkout mình trước khi mở với `--investigation-save-dir <thu-muc-moi-chua-dung>` theo CLONE_VA_BAT_DAU. Xác nhận intro/hồ sơ trống, path F5/F6 thực tế; không dùng profile đã giải. Người chơi tự đi xưởng → Hà → xưởng, không cho đáp án hoặc thời điểm clip. Chỉ gợi ý thao tác khi họ yêu cầu; ghi nguyên văn lời hỏi/gợi ý.

Hoàn tất khi có biên bản mã người thử (không cần thông tin cá nhân), quan sát trực tiếp tách suy luận, chỗ không hiểu cơ cấu/không đọc hình/không biết thao tác/hiểu nhưng chưa đủ chứng minh, vấn đề theo mức ảnh hưởng và bước tái hiện. Không chấm bằng đoán đúng Khải. Chưa có người thật: giữ **NOT_RUN**, bàn giao lịch/phiếu sẵn sàng rồi dừng, không dùng agent hoặc thêm polish.

Bàn giao báo cáo riêng `Docs/QA/Tasks/N3/`, commit docs; đề xuất UI dựa trên vấn đề cụ thể, gửi người 2 nếu chạm session. Tích hợp thay đổi người 1/2 sau review và kiểm đúng phạm vi; không tự sửa thay cả ba module.
Prompt nhận việc: “Trên work/ui-onboarding-review từ fcbb7e19, chuẩn bị và ghi nhận playtest người mới trung thực bằng phiếu trung tính; ưu tiên phản hồi trước mở rộng onboarding, giữ quyền sửa session cho người 2 và scene chính cho người 3.”

## Điền khi nhận

- Người nhận / ngày:
- HEAD thực tế / file sẽ chạm:
- Mã phiên / trạng thái người thử thực tế:
- Kiểm và bằng chứng / đề nghị gửi người 2:
- Commit kết quả / giới hạn / bàn giao tích hợp:
