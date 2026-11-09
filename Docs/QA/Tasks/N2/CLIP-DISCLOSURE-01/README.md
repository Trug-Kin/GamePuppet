# CLIP-DISCLOSURE-01 — Không lộ bản chép clip chưa xem

Gameplay & Dữ liệu. **Đã triển khai và kiểm runtime trong scene Gameplay; chờ tích hợp.**

## Căn cứ, baseline và acceptance

GDD7.2/11: chưa xem nội dung thì sổ không tiết lộ. Source baseline baf7335b5: Observe đã chặn clip theo hai cờ exposure nhưng InspectUI/CompareUI đọc definition.transcript trực tiếp. Acquire clip có thể thấy bản chép đầy đủ; batch ghi bị chặn vẫn có lời báo đã ghi. Baseline kiểm API/source consumer8 ca,3 lỗi; chưa tái hiện baseline bằng click UI.

Acceptance trước sửa nằm trong [tiến độ](../../../../QuanLy/DANG_LAM_NGUOI_2.md): chưa xem/chỉ trước/chỉ sự cố không lộ full transcript; đủ cả hai giữ bản chép cũ; không cấp quan sát bằng mở/xem/tua; ghi lặp không nhân đôi; phản hồi đúng khi ghi bị chặn; save/load giữ cờ. Dùng guard hiện có, không đổi cách tính đã xem.

## Thay đổi

Nguồn **e0544b740fa127897fd6d6c746dc7b15aab68fbd**:

- InvestigationSession.cs: accessor `EvidenceTranscript(id)` chỉ đọc, xử lý ID thiếu/chưa có và guard clip.
- InvestigationSessionUI.cs: hai consumer inspect/compare dùng accessor; không đổi layout.
- InvestigationOnboarding.cs: batch ghi clip bị chặn trả Message dễ hiểu, không báo ghi giả.

Schema/ID/GUID/exposure và save hợp lệ giữ nguyên; không repair observation trong save cũ. Không ẩn bản chép của vật chứng khác đã có.

## Kết quả và cách nhận

43/43 API gói riêng; nằm trong86/86 API cuối trên df2a1a83. Tuyến37/37 controller/UI trên d7f1c93b có dừng clip sớm, bản chép trung tính, xem đủ rồi ghi/lặp; proxy không có footage thật. Compare cùng accessor kiểm API, không tuyên bố mọi đường UI đã chạy.

Nhận đủ chuỗi nguồn/công cụ/tài liệu và kiểm scene chung theo [bàn giao gộp, lệnh chạy, kết quả lịch sử và giới hạn](../PROTOTYPE-FLOW-01/README.md). Không cần wiring mới. Scene chung/UI1080p/người mới chưa kiểm nguồn mới; không suy PASS từ scene Gameplay.
