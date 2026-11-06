# r107 — nền hở và mở đầu điều tra

Ngày kiểm: **06/11/2026**. Mở `Assets/Scenes/JourneyPresentation_v48_r107.unity` bằng **Unity6000.6.2f1**. Giữ checkpoint r101–r106. Đây là sửa từ hai phản hồi thực của người dùng, không phải một phiên người mới mới được ghi nhận.

## Bắt đầu

Xem [hướng dẫn người tổ chức](PLAYTEST_NGUOI_MOI.md); chỉ đưa [trang người chơi](HUONG_DAN_NGUOI_CHOI.md) cho người thử. Chọn thư mục profile chưa dùng, không load save kỹ thuật. Scene không tự load; bắt đầu trước cửa xưởng, nhìn thấy lối vào. Lời mở đầu nói ông Tư được phát hiện tử vong; mục tiêu duy nhất là vào xưởng, lên gác, kiểm tra dấu trên sàn gần lan can.

WASD/chuột, E tương tác, R trở lại, P ghi chép, Esc pause; F5/F6 hồ sơ thử riêng. Save namespace `conroi.investigation.v36.r107`, schema1, `<profile>/InvestigationV36/r107/prototype.json`; không đọc/migration r106 hoặc save gameplay cũ. Profile đang chơi giữ vị trí/góc khi load, không bị kéo về spawn mới.

## Đã thay

- Sửa winding nền trong bốn mesh riêng r107, đồng bộ collider liên quan; không đổi vertex/cao độ/nước/nền chợ. [Nguyên nhân, object, nguồn và giới hạn](MAP_REPAIR.md).
- Mở đầu có Bắt đầu/Bỏ qua hướng dẫn; chỉ dẫn cửa, cầu thang và một manh mối đầu. Marker biến mất khi tới nơi/đã ghi. Giữ tutorial flags trong save.
- Dấu sàn có prompt “E · Kiểm tra dấu trên sàn”, góc xem trên, xoay/phóng, Ghi quan sát và Trở lại [R]. Chỉ ghi đặc điểm thấy được; không tự gán danh tính hoặc nguyên nhân.
- Sau ghi lần đầu, nhắc P một lần qua bước xem lại ghi chép; sau đó mới giao điện thoại. HUD giảm còn một việc, prompt theo vật và nhắc thao tác; thông báo tự hết sau6giây.
- Khám vật không còn sidebar toàn bộ công cụ. Hồ sơ ưu tiên ghi chép/việc trước mắt, mật mã khi có thư, đối chiếu khi có hai mục; bàn dựng trong trang riêng. Lưu/tải ở hồ sơ/pause, phím tắt giữ nguyên. Suy nghĩ/kết luận “Chưa đủ chứng minh” vẫn truy cập được, không khóa bằng nhặt đủ đồ.

Không đổi controller/InputActions, art Quaternius, clip, luật hồi hoặc sáu diễn biến r106. Không chạy lại author/derive/bind. Script namespace/GUID riêng trong `Assets/Scripts/InvestigationV36R107`; [nguồn](ASSET_SOURCES.md).

## Kiểm thực tế

Input kỹ thuật **tổng hợp**, qua controller/raycast và UI; API chỉ dùng setup profile/đọc assertion/ảnh chẩn đoán, không mở sẵn trang hoặc ghi đáp án cho video mở đầu.

- Đi từ spawn qua cửa/cầu thang tới dấu sàn; E/raycast thật ở2,24m. Xoay/phóng không quay camera người chơi. Ghi quan sát, R, P xem lại; chỉ có mục đã xem, không thêm mapping/trial. F5/F6 giữ tiến trình và vị trí. `onboarding-route.txt`, `onboarding-ui-complete.txt`.
- Đi liên tục76điểm từ xưởng tới vị trí báo lỗi rồi qua các điểm665/400,650/390,630/370 và trở lại: **113,49giây,0frame mất tiếp đất**, waypoint cuối grounded. `map-controller-complete.txt`; xem video, không dùng A* planner làm bằng chứng controller.
- Đi chéo/dừng/pause và F5/F6 trên vùng sửa; skip giữ sau load, hồ sơ không tự có chứng cứ. `map-lifecycle.txt`. Lượt đầu input không được nhận lưu riêng **FAIL/superseded**; sửa setup fixture nhận input, không sửa controller.
- Mở lại scene sạch; compile thành công,32958component kiểm,0MissingScript/0reference hỏng. `final-live-audit.txt` ghi Edit mode; các cảnh báo face rất nhỏ giải thích trong map report.

[Checklist chi tiết](CHECKLIST.md) · [Video và ảnh, giới hạn bằng chứng](VIDEO_INDEX.md) · [Phản hồi gốc](FEEDBACK.md).

## Chưa được coi là đạt

Phiên người mới có tổ chức trên r107: **NOT_RUN**. Phản hồi r106 là thật, không tự điền số lần gợi ý/thời gian hoặc chấm PASS từ input tổng hợp.

Ba góc đầu của người dùng chưa khớp chính xác pose; chỉ góc vùng hở rộng được tái hiện đúng. Topology kế thừa còn cảnh báo, generic Blender delivery gate **FAIL**; không tuyên bố toàn map kín/sạch. Chưa thêm rào giới hạn ngoài prototype hoặc chứng nhận các khu xa an toàn. Xem checklist cho ca UI chưa chạy; không coi việc hiện mục tiêu điện thoại là đã chứng minh người mới tìm được điện thoại.

Không tạo build, ZIP, runner sản phẩm mới hoặc mở thêm nội dung. Tập bảo vệ thực tế và hash nằm trong `protected-start.json`/`protected-final.json`; không dùng hash lịch sử để khôi phục tiến trình người dùng.

## Đối chiếu bảo vệ và ca còn thiếu

`protected-final.json`: **1733/1733 entry không đổi**, gồm tập checkpoint/asset/nguồn đã theo dõi và17save gameplay gốc cùng1profile PT001r106. Không suy rộng thành toàn bộ mọi save trên máy. Hồ sơ kỹ thuật mới nằm trong thư mục kết quả r107. Bản sao live chưa phân loại được giữ riêng, không tự load vào phiên người dùng hoặc ghi thành kết quả playtest.

Lượt định kiểm tiếp trước lần ghi đầu bị guard fresh-intro từ chối: live Editor đã được thao tác ngoài fixture và có thêm tiến trình. Không tiếp tục reset/input cạnh tranh. Do đó **NOT_RUN**: save/load trước lần ghi đầu, quay lại vật sau skip, controller tới điện thoại theo mục tiêu thứ hai, layout1080p và UI mật mã/đối chiếu/kết luận sau đổi navigation. Những chức năng không đổi giữ kết quả r106; những điều kiện navigation mới chỉ được đọc code, không có PASS UI. Không hẹn rằng người mới chắc chắn hiểu mở đầu.
