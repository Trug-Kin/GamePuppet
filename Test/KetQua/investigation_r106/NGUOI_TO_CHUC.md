# Chuẩn bị và điều hành phiên r106 — chỉ cho người tổ chức

Chỉ giao [HUONG_DAN_NGUOI_CHOI.md](HUONG_DAN_NGUOI_CHOI.md) cho người chơi. Không đưa GDD, bảng đáp án, báo cáo sáu ca, video kiểm kỹ thuật hoặc tài liệu nguồn trong lúc họ chơi.

## Bản dùng để thử

- Unity **6000.6.2f1**.
- Scene **`D:\ConRoi\Assets\Scenes\JourneyPresentation_v48_r106.unity`**.
- Không đổi scene/asset, chạy script author/derive/bind hoặc tạo bản mới khi tổ chức phiên.
- Đợt chuẩn bị hiện tại dùng profile **`D:\ConRoi\Test\PlaytestProfiles\PT001_r106`**. Xem [trạng thái giao máy](playtest_handoff/README.md) trước khi dùng. Đây là mã slot, chưa phải kết quả của một người thật.

## Mở lại hoặc chuẩn bị người tiếp theo

1. Lưu công việc khác của bạn và đóng Editor Con Rối. Nếu có scene đang sửa chưa lưu, giải quyết trước; không bỏ thay đổi của người khác.
2. Với người tiếp theo, chọn mã không có thông tin cá nhân, ví dụ PT002, và thư mục chưa tồn tại `D:\ConRoi\Test\PlaytestProfiles\PT002_r106`. Không dùng profile kỹ thuật đã giải hoặc profile của người trước; không xóa chúng để lấy chỗ.
3. Mở Editor bằng PowerShell, thay PT002 đúng mã đã chọn:

```powershell
& 'D:\data_unity\6000.6.2f1\Editor\Unity.exe' -projectPath 'D:\ConRoi' --investigation-save-dir 'D:\ConRoi\Test\PlaytestProfiles\PT002_r106'
```

4. Mở đúng scene r106 rồi Play. Hồ sơ không tự load. Kiểm chưa có vật đã ghi, quan sát clip, gán ký tự hoặc nhận định điền sẵn. Bốn ô kết luận trống là bố cục, không phải kết luận đã có.
5. Khi còn trống, nhấn F5, kiểm file `<profile>\InvestigationV36\r106\prototype.json` mới xuất hiện đúng nơi. Namespace phải là `conroi.investigation.v36.r106`, evidence/mapping/relations/trials rỗng; claim/grounds trống, finalized=false. Nhấn F6 và xác nhận vẫn trống. Ghi đường dẫn thực và kết quả vào phiếu. Không kiểm bằng một profile kỹ thuật thay thế.
6. Đưa về vị trí bắt đầu, đóng hồ sơ nếu đang mở, nhấn Esc để tạm dừng. Giao trang hướng dẫn người chơi. Nhờ họ nhấn Esc khi sẵn sàng; đó mới là thời điểm bắt đầu phiên người thật.

PT001 có thể dùng cho người đầu tiên nếu vẫn ở trạng thái chuẩn bị trống, chưa có người chơi. Nếu đã có tiến trình hoặc không chắc ai đã dùng, giữ lại và chọn mã mới. Không coi dữ liệu trống đã thử F5/F6 là profile đã giải.

## Trong phiên

Để người thử tự khám xưởng → hỏi Hà → quay lại bàn dựng và ghi nhận định. Chỉ hỗ trợ thao tác khi họ yêu cầu. Ghi nguyên văn câu hỏi, thời điểm và chính xác câu gợi ý đã đưa; không chỉ sẵn thời điểm clip, diễn biến cần chọn hay người cần kết luận. Không ép nói thành lời nếu việc đó gây khó chịu; có thể hỏi các câu trung tính sau phiên.

Giữ nguyên năm câu hỏi và phiếu trung tính trong [PLAYTEST_NGUOI_MOI.md](PLAYTEST_NGUOI_MOI.md). Ghi riêng điều nhìn thấy, điều suy ra/căn cứ, cơ cấu chưa hiểu, hình/chữ khó đọc, thao tác/lỗi chức năng, và phản biện “đã hiểu nhưng chưa đủ chứng cứ”. Một nhận xét có thể thuộc nhiều nhóm. Không quy mọi phản biện thành không hiểu.

## Lỗi kỹ thuật

- Khi chặn chơi hoặc có nguy cơ mất dữ liệu: dừng thao tác, ghi màn hình/trang/vị trí và hành động ngay trước lỗi. Ghi nguyên văn thông báo nếu có; không đoán nguyên nhân rồi ghi thành sự thật.
- Không F6 hoặc thoát Play ngay nếu việc đó làm mất trạng thái cần ghi. Sau khi ghi đủ, hỏi người chơi trước khi tải lại save thử của chính phiên đó. Ghi thời điểm can thiệp và đoạn tiến trình mất/phải làm lại.
- Nếu F5 không tạo file đúng profile, chưa giao máy. Kiểm cách khởi động Editor và tham số; không đổi sang save người dùng hay migration.
- Nếu tiếp tục được sau hỗ trợ, đánh dấu đoạn đã được hỗ trợ. Nếu không, kết thúc với trạng thái bị chặn; vẫn giữ dữ liệu thực đã thu. Không tự sửa sản phẩm giữa lượt để rồi coi là cùng một bản thử.

## Sau phiên

Dùng [MAU_GHI_NHAN_PHIEN.md](MAU_GHI_NHAN_PHIEN.md) và [MAU_TONG_HOP_VAN_DE.md](MAU_TONG_HOP_VAN_DE.md). Lưu tại `playtest_handoff/sessions/<mã>/`, chỉ mã/ngày/thời lượng/thông tin kỹ thuật cần thiết. Không cần tên thật hoặc dữ liệu cá nhân không liên quan. Tách lời người chơi, điều người tổ chức trực tiếp thấy và diễn giải của người tổ chức.

Ưu tiên vấn đề có chứng cứ cụ thể: **1 chặn chơi/mất dữ liệu → 2 hiểu sai chứng cứ/cơ cấu → 3 khó thao tác/đọc → 4 góp ý hình ảnh nhỏ**. Mỗi đề xuất cần dẫn vị trí, tình huống, câu nói hoặc ảnh/log tương ứng; phân tích trước khi đề nghị sửa. Không tạo r107 chỉ vì một ý kiến chưa phân tích. Chưa có người thật thì giữ NOT_RUN và dừng chuẩn bị.
