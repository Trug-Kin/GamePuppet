# Registry vật chứng ban đầu

Nguồn dữ liệu giữ nguyên: `Assets/Scripts/InvestigationV36R107/InvestigationEvidence.cs`. Registry phục vụ phát triển, không đưa lời giải cho người chơi. Thay ID phải được người2/3 duyệt; đợt này **không đổi ID/schema**.

| ID | Loại / nguồn | Chủ nội dung |
|---|---|---|
| slide, shoeprint | Dấu trên sàn xưởng | Người3; hình do người1 |
| r03, r12 | Hai rối khác nhau | Người1/3 |
| mechanism | Cánh/chốt/dây | Người2 giữ luật; người1 hình |
| phone, clip | Điện thoại/đoạn sửa rối | Người3; nguồn animation người1 |
| khai, coat, shoe, shoecontrol, fabric | Người gặp/mẫu đối chiếu | Người3; art người1 |
| ha, letter, receipt, henri, draft, samples, decoded | Lời khai/tài liệu/mẫu/cách đọc đã ghi | Người3; state người2 |

Không suy thời điểm/ý định từ giày hoặc vải; không tự thêm quan sát khi seek. Cột registry không thay source code: kiểm ID thực trong code trước khi chỉnh. Clip/hình nộm vẫn animation minh họa; người mới sau r107 NOT_RUN theo bộ QA, không đổi thành PASS khi dọn.
