# Bắt đầu phiên thử và phiếu người mới — NOT_RUN

**Tài liệu này dành cho người tổ chức.** Bản tách để giao máy: [người tổ chức](NGUOI_TO_CHUC.md) · [chỉ giao người chơi trang này](HUONG_DAN_NGUOI_CHOI.md) · [mẫu ghi phiên](MAU_GHI_NHAN_PHIEN.md) · [mẫu phân loại vấn đề](MAU_TONG_HOP_VAN_DE.md) · [trạng thái chuẩn bị hiện tại](playtest_handoff/README.md). Các câu hỏi và phiếu trung tính bên dưới được giữ nguyên.

Chưa có người thật chơi r106. Người kiểm kỹ thuật biết GDD không thay người mới. Không chấm bằng việc đoán đúng Khải, nhớ mã vật chứng hoặc nói đúng câu GDD.

## Chuẩn bị profile — dành cho người tổ chức

1. Lưu công việc của mình rồi đóng **Editor Con Rối** trước khi truyền tham số. Mở qua Hub hoặc gửi tham số khi Editor dự án đã mở không bảo đảm đó là một phiên khởi động mới.
2. Chọn một tên thư mục **chưa dùng**, ví dụ `D:\ConRoi\Test\PlaytestProfiles\nguoi_moi_01`. Không dùng `profile-launch-verified`, `api-profile`, `ui-cycle-profile` hoặc `regression-profile` trong bộ kết quả kỹ thuật.
3. Từ PowerShell mở đúng Editor (thay tên thư mục cho mỗi người):

```powershell
& 'D:\data_unity\6000.6.2f1\Editor\Unity.exe' -projectPath 'D:\ConRoi' --investigation-save-dir 'D:\ConRoi\Test\PlaytestProfiles\nguoi_moi_01'
```

4. Mở `Assets/Scenes/JourneyPresentation_v48_r106.unity`, nhấn Play. Scene lưu hồ sơ trống và **không tự load**. Chưa nhấn F6 từ hồ sơ cũ. Người tổ chức kiểm hồ sơ không có vật đã xem, gán ký tự hay nhận định đã điền; bốn ô kết luận trống là bố cục, không phải đã kết luận.
5. Có thể nhấn F5 một lần khi còn hồ sơ trống để xác nhận file được ghi đúng **`D:\ConRoi\Test\PlaytestProfiles\nguoi_moi_01\InvestigationV36\r106\prototype.json`**. Ghi đường dẫn thực vào phiếu. JSON phải có namespace `conroi.investigation.v36.r106`, evidence/mapping/relations/trials rỗng và claim/grounds trống. Nếu file không xuất hiện đúng nơi, dừng chuẩn bị để kiểm tham số; không lấy một profile kỹ thuật thay thế.

Tham số trên đã được xác nhận bằng mở lại Editor thực tế: `editor-actual-args.txt`, `profile-launch-proof.txt`. F5/F6 qua input bàn phím tổng hợp đã ghi/tải đúng thư mục thử. API chỉ tạo một thay đổi tạm sau lưu để phân biệt trạng thái trước/sau tải; không gán đáp án. Đây không phải kết quả playtest người thật. Không xóa hoặc migration save người dùng.

## Chỉ đưa phần này cho người chơi

**Khám xưởng, hỏi Hà tại quầy rồi quay lại thử dựng diễn biến. Ghi điều bạn có thể kết luận, hoặc chọn “Chưa đủ chứng minh”.**

WASD/chuột đi và nhìn; E tương tác, R trả vật/đóng, P hồ sơ, Esc đóng hoặc pause. F5 lưu, F6 tải hồ sơ phiên thử. Xem vật có xoay/phóng. Clip có phát/dừng/tua; bàn dựng có góc tổng/cận, dừng/tua và các thao tác hiển thị. Không cần đọc GDD hoặc biết trước lời giải.

1. Bạn nghĩ đã xảy ra chuyện gì?
2. Chi tiết nào khiến bạn nghĩ như vậy? Hãy chỉ vào vật hoặc hình bạn đã thấy.
3. Bạn đã biết ai thao tác cơ cấu chưa, hay mới nghi ngờ?
4. Bạn còn cần kiểm điều gì?
5. Clip và bàn dựng giúp bạn phân biệt những khả năng nào?

## Phiếu quan sát — người tổ chức ghi

Giữ nguyên văn nhận xét, thời điểm và số lần cần gợi ý. Không chỉ sẵn thời điểm then chốt hoặc giải thích đáp án.

| Mục | Ghi nhận thực tế |
|---|---|
| Người thử / ngày / độ phân giải / đường dẫn profile | NOT_RUN |
| Điều trực tiếp nhìn thấy / lời nói nguyên văn | NOT_RUN |
| Điều suy ra / điều còn thiếu | NOT_RUN |
| **Không hiểu cơ cấu** — mô tả chỗ chưa hiểu | NOT_RUN |
| **Không đọc được hình** — bộ phận/góc bị che hoặc quá nhỏ | NOT_RUN |
| **Không biết thao tác** — nút/phím hoặc hành động chưa rõ | NOT_RUN |
| **Hiểu nhưng cho rằng chứng cứ chưa đủ** — lý do người thử nêu | NOT_RUN |
| Nhầm vai giả thuyết với người đã được chứng minh | NOT_RUN |
| Lần cần gợi ý / câu hỏi / gợi ý đã đưa / lỗi thao tác | NOT_RUN |

Có thể đánh dấu nhiều nhóm cùng lúc; không quy mọi phản biện thành “người chơi chưa hiểu”. Sau phiên mới tổng hợp vấn đề cụ thể. Chưa có phản hồi thì không tự thêm art/nội dung để lấp thời gian chờ.
