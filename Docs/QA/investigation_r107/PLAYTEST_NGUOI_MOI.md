# Tổ chức lượt thử r107

Chỉ đưa [hướng dẫn người chơi](HUONG_DAN_NGUOI_CHOI.md) cho người thử. Giữ GDD và báo cáo kỹ thuật cho người tổ chức.

1. Dùng Unity **6000.6.2f1**, scene `Assets/Scenes/JourneyPresentation_v48_r107.unity`. Không chạy script author/derive/bind.
2. Đóng Editor sau khi xử lý công việc chưa lưu của mình. Chọn thư mục thử chưa dùng, không xóa profile cũ. Ví dụ:

```powershell
& 'D:\data_unity\6000.6.2f1\Editor\Unity.exe' -projectPath 'D:\ConRoi' --investigation-save-dir 'D:\ConRoi\Test\PlaytestProfiles\PT_r107_01'
```

3. Mở r107, Play. Màn hình mở đầu có một việc trước mắt và lựa chọn bỏ qua hướng dẫn. Không tự load tiến trình kỹ thuật. Khi kiểm F5 ở hồ sơ trống, đường dẫn phải là `<profile>\InvestigationV36\r107\prototype.json`, namespace `conroi.investigation.v36.r107`, schema1. Không nạp/migration save r106 hoặc save gameplay cũ vào đây.
4. Ghi mã phiên, ngày, độ phân giải, profile thực tế, bản scene và thời gian bắt đầu/kết thúc. Chỉ hỗ trợ thao tác khi được yêu cầu; ghi nguyên văn câu hỏi và lời gợi ý đã đưa. Không chỉ thời điểm clip hoặc diễn biến cần chọn.
5. Dùng mẫu ghi nhận và tổng hợp tại [bộ r106](../investigation_r106/MAU_GHI_NHAN_PHIEN.md), [mẫu vấn đề](../investigation_r106/MAU_TONG_HOP_VAN_DE.md); lưu bản điền thật theo mã phiên trong bộ r107. Không điền thay người chơi.

Giữ năm câu hỏi trung tính:

- Bạn nghĩ đã xảy ra chuyện gì?
- Chi tiết nào khiến bạn nghĩ như vậy?
- Bạn đã biết ai thao tác cơ cấu chưa, hay mới nghi ngờ?
- Bạn còn cần kiểm điều gì?
- Clip và bàn dựng giúp bạn phân biệt những khả năng nào?

Ghi riêng điều nhìn thấy, điều suy ra/căn cứ, không hiểu cơ cấu, hình/chữ khó đọc, thao tác/lỗi chức năng và “hiểu nhưng thấy chứng cứ chưa đủ”. Tách quan sát của người tổ chức khỏi diễn giải. Phân loại: chặn chơi/mất dữ liệu → hiểu sai chứng cứ/cơ cấu → khó thao tác/đọc → góp ý hình ảnh nhỏ. Chỉ đề xuất sửa từ tình huống hoặc nhận xét cụ thể.

Phản hồi r106 đã nhận được tóm tắt riêng trong [FEEDBACK.md](FEEDBACK.md). Phiên người mới có tổ chức trên r107: **NOT_RUN**, không gán PASS bằng kiểm tổng hợp.
