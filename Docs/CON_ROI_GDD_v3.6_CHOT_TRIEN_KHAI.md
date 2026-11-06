# CON RỐI
## Tài liệu thiết kế game v3.6 — Chốt nội dung để triển khai

Ngày: 04/11/2026. Thể loại: điều tra, giải đố. Nền tảng: PC, một người chơi, góc nhìn thứ nhất.

**Trạng thái:** đây là bản chốt thiết kế nội dung, thay v3.5 làm căn cứ cho hướng game điều tra mới. Chưa phải xác nhận đã triển khai hoặc đã playtest. Những điều cần kiểm bằng phần mềm và người chơi được tập trung ở mục 14; không được ghi thành kết quả đã đạt.

**Phạm vi thay đổi:** giữ vụ án, nhân vật, bối cảnh Đà Lạt, mật mã tư thế và các khu đã chọn. Sửa cơ cấu lan can, thay căn cứ thời điểm bằng đoạn quay sửa rối có sẵn tại hiện trường, đồng bộ chuỗi chứng cứ gây ngã, cách chốt hồ sơ và ending. Không dùng lại kết luận sai rằng sợi vải hoặc dấu giày tự chứng minh thời điểm gây án. Không còn cơ cấu lan can mở rồi đứng yên sau khi buông dây của v3.5.

**Không tác động sản phẩm cũ:** tài liệu này không sửa project Unity, scene r98 hoặc các checkpoint trước; không đổi save, source, asset hay ZIP hiện có. Việc tích hợp phải thực hiện ở nhánh/scene dẫn xuất riêng. GDD v2.2 và v3.0–v3.5 là lịch sử tham chiếu, không đồng thời là đặc tả đang có hiệu lực.

---

## 0. Tóm tắt sản phẩm

**Câu giới thiệu:** Mỗi tư thế là một ký tự. Mỗi con rối giữ một phần sự thật.

Khang điều tra cái chết của chủ một gánh rối cũ ở Đà Lạt. Những người từng trong gánh lần lượt nhận các mật thư viết bằng tư thế rối. Giải mã chúng dẫn anh qua hồ, chợ, trường và dốc, tới một vụ chuyển bán rối cổ bị che giấu từ mười hai năm trước. Người từng bị đổ oan nay trở về, dùng chính quy ước của người thầy để ép cả nhóm trả món nợ mà anh cho rằng họ còn thiếu mình.

Người chơi phải phân biệt ba việc: ai gửi thư, ai có mặt tại xưởng, và ai thực hiện thao tác liên quan trực tiếp đến cú ngã. Tìm ra người đáng nghi chưa đủ để hoàn tất hồ sơ.

Chốt bản đầu: **một vụ án lớn, hai mã ending A/C; A có hai diễn biến A-treo và A-sai**. Không thêm chuỗi vụ án, siêu năng lực, kinh dị hoặc giấc mơ trong đợt triển khai này.

## 1. Chủ đề và vai trò con rối

- Ai đang giật dây ai: con người bị chi phối bởi tiếng xấu, sự im lặng và ý nghĩ mình có quyền đòi lại.
- Di sản và lòng tham: gìn giữ một kỷ vật có thể biến thành chiếm hữu nó.
- Sự thật và chứng cứ: quan sát, suy luận và mức chắc chắn là ba điều khác nhau.
- Thông điệp: **Biết sự thật chưa đủ; phải chứng minh đúng điều mình biết.**

Con rối là vật thật: di sản bị tranh chấp, dấu hiệu nhận mật thư, bảng chữ bằng tư thế, đạo cụ dàn dựng và hình nộm dùng để thử diễn biến. Trong vụ đầu, **cơ cấu lan can là phương tiện gây ngã**, không gọi con rối rơi cạnh nạn nhân là hung khí. Rối không có ý thức, không truy đuổi và không tự bắt chước người chơi. Chuyển động trong bàn dựng lại là mô hình do người chơi điều khiển, được phân biệt rõ với sự kiện thật.

Không giữ Minh, Vy, Compliance, bốn giai đoạn bắt chước, mặt Minh trên rối, hiệu ứng nhòe mặt hay trạng thái ngủ của hướng cũ.

## 2. Nhân vật

### 2.1. Khang

Tên làm việc chính thức trong bản triển khai: **Khang**, nam, người điều tra. Không có năng lực đặc biệt. Thông tin đến từ vật chứng, lời khai, tài liệu và thử giả thuyết. Tính cách thể hiện qua cách người chơi kết luận; không đặt sẵn việc bắt người trái với hồ sơ họ đã lập.

Tuổi và cấp bậc không tham gia câu đố, không cần chốt để lập trình prototype. Quy trình và tên nghiệp vụ cần được kiểm nguồn trước khi phát hành; bản này không mô tả các thao tác game như quy trình pháp lý đã xác thực.

### 2.2. Nhóm gánh rối cũ

| Nhân vật | Quá khứ và vai trò hiện tại | Điều họ che giấu |
|---|---|---|
| Ông Tư | Chủ gánh, nạn nhân ở xưởng | Khởi xướng chuyển bán ba rối cổ để trả nợ |
| Ông Bảo | Từng tháo và kéo rối; nay thường tựa lan can cầu Ông Đạo | Đóng thùng rối; im lặng vì nghĩ nói ra không thay đổi được gì |
| Chị Hà | Từng phụ trách hậu cần; nay bán hàng ở chợ | Tổ chức vận chuyển ba gói hàng |
| Cô Mai | Từng dịch thư cho gánh; nay dạy học | Dịch bản thỏa thuận bán rối và giữ bản nháp |
| Anh Tùng | Thành viên trẻ ngày trước; đang chuẩn bị rời Đà Lạt | Đi cùng chuyến giao hàng |
| Bà Lan | Từng giữ kho; sống trong khu vườn có hàng rào kín | Ký xuất kho và giấu phần rối còn lại |
| Khải | Cựu thành viên trở về, ở homestay | Gửi mật thư, thao tác dây nhả lan can và dàn dựng hiện trường |
| Ông Henri | Người thầy đã mất | Để lại bộ rối, sổ nghề và bảng tư thế; không có bí mật siêu nhiên |

**Chỉ Bà Lan biết chỗ cất hiện tại của bộ rối còn lại. Ông Tư không biết.** Ông Tư là người Lan còn chịu nghe nên Khải tìm ông để gây sức ép.

### 2.3. Nhân vật hỗ trợ

Trung úy Phương là đồng nghiệp, yêu cầu tách điều quan sát được khỏi kết luận; không đưa đáp án. Chủ homestay giữ sổ ra vào và làm nhân chứng. Nhân vật phụ không xuất hiện cuối để bất ngờ trở thành hung thủ.

Khải xuất hiện sớm. Người chơi nhìn thấy áo khoác có phần cổ tay sờn và đế giày có chỗ mòn; đây là chi tiết để quay lại đối chiếu, không hiện nhãn “đáng nghi”.

## 3. Cốt truyện và sự thật dành cho đội phát triển

### 3.1. Gánh rối và vụ cũ

Henri dạy nhóm biểu diễn cho khách trong và ngoài nước. Khẩu lệnh hậu đài của ông dùng tiếng Anh. Bảng tư thế A–Z được nhóm dùng để đánh vần ngắn qua màn, làm ký hiệu tập diễn và ghi chú đạo cụ. Mật mã là quy ước cũ hữu ích trong phạm vi gánh, không phải phương pháp bảo mật tuyệt đối.

Mười hai năm trước, gánh nợ nặng. Tư cùng nhóm chuyển bán ba rối cổ dù Henri ghi ý nguyện “không bán, không chia”. Khải phản đối. Nhóm gán cho anh chuyện làm mất rối và đuổi anh khỏi gánh. Sau đó Lan chuyển phần còn lại khỏi Kho Sáu tới một nơi chỉ mình biết.

Khải ra nước ngoài. Nhiều năm mang tiếng khiến anh dần coi bộ rối là khoản bù mà cả nhóm nợ mình. Anh muốn được minh oan, nhưng cũng muốn chiếm bộ rối. Đây là biến chuyển đạo đức của nhân vật, không phải lý do bào chữa cho hành vi hiện tại.

Người chơi phát hiện vụ cũ để hiểu vụ hiện tại; không mở thêm một chiến dịch điều tra độc lập. Người mua và số tiền không là đáp án câu đố; không cần dựng thêm nhân vật hoặc địa điểm cho họ.

### 3.2. Mật thư và cuộc đối đầu

Khải hỏi Lan trực tiếp nhưng bị từ chối. Anh gửi năm thư bằng quy ước Henri: dò chìa kho, đường chuyển hàng, giấy tờ và người còn liên lạc với Lan; thư cuối ép Lan giao bộ rối tại ga. Hình thức gợi quá khứ khiến cả nhóm lo sợ; nội dung cụ thể còn chứa lời thúc ép, không chỉ là một hình rối vô hại.

Tối xảy ra án, Khải tới xưởng yêu cầu Tư thừa nhận vụ cũ và thuyết phục Lan giao rối. Tư từ chối rồi quay lên gác sửa dây một rối sân khấu lớn. Khải thực hiện thao tác nhả lan can khi Tư đang dựa vào. Tư ngã xuống sân khấu. Sau đó Khải dàn dựng thành tai nạn vướng dây và rời đi.

**Sự thật tác giả:** Khải chủ động nhả chốt trong cơn tức giận, biết Tư đang dựa vào lan can. Không thiết kế một kế hoạch giết người chuẩn bị từ trước. **Phạm vi hồ sơ:** chứng minh thao tác gây ngã và việc dàn dựng; không buộc người chơi chứng minh Khải mong muốn nạn nhân chết, không suy ra tội danh chỉ bằng mini game.

### 3.3. Cơ cấu mới: lan can tự hồi

Đây là thay đổi thay thế hoàn toàn cơ cấu không tự đóng của v3.5. Cơ cấu dùng trong game là thiết kế hư cấu cần dựng thử; không phải hướng dẫn xây một thiết bị an toàn thực tế.

| Mã nội bộ | Bộ phận | Hành vi cần thể hiện |
|---|---|---|
| G | Gác thao tác | Có cầu thang, mặt sàn giữ dấu và đoạn lan can L |
| L | Cánh lan can | Có cơ cấu hồi về vị trí đóng sau khi không còn lực giữ mở; không có nấc khóa mở |
| C | Chốt giữ đóng | Bình thường cài; nhả khi dây D căng; tự cài khi L hồi đóng và D đã chùng |
| D | Dây nhả tại hậu đài | Người giữ tay nắm ở ĐK có thể nhả C; không trực tiếp đẩy cánh L |
| M | Móc giữ dây khi thao tác một mình | Là cách giữ D căng không cần người; có lớp bụi còn nguyên ở hiện trường |
| ĐK | Điểm điều khiển | Tách khỏi gác, người đứng đây không đồng thời đẩy L bằng tay |
| P | Vị trí sửa rối | Trên gác, gần L; người chơi nhìn rõ quan hệ P–L–ĐK |

Luật trạng thái cho prototype:

1. D chùng và L đóng: C cài, lực dựa thông thường không mở L.
2. D căng: C nhả. L mở khi có lực từ phía gác thắng lực hồi, chẳng hạn Tư đang dựa vào.
3. D được thả khi L còn mở: L hồi đóng theo chuyển động hữu hạn, C bắt lại khi cánh về đúng vị trí. Cơ cấu này không giữ L mở thụ động.
4. D còn căng nhưng không có lực đẩy L: L vẫn hồi về đóng; C chưa cài cho tới khi D chùng.
5. Nếu muốn để lối mở lâu phải có lực giữ hoặc vật chèn; không mặc định một khe đã mở sẽ tồn tại mãi. Nếu Tư còn đè lên cánh sau khi D được thả, phải mô phỏng lực đó; không bắt cánh hồi xuyên người bằng luật lập trình. Đoạn quay, không riêng lực hồi, là căn cứ cho trạng thái đóng ngay trước sự cố.
6. Trong hiện trường chốt: không có vật chèn; móc M không được dùng; cơ cấu hồi và chốt hoạt động bình thường khi khám trước thử nghiệm. Không dùng lập luận chỉ từ việc “không thấy hỏng”.
7. Trạng thái mở rồi bỏ đi được chạy thử, kể cả khoảng hồi ngắn ngay sau khi thả. **Không bỏ qua thời gian hồi để tuyên bố mở sẵn là bất khả thi.** Đo diễn biến hồi và đường đi thực tế trong prototype; nếu còn một cửa thời gian hợp lý cho kịch bản thay thế thì chưa được duyệt cơ cấu.

Kích thước khởi tạo: gác khoảng 3 m trên sân khấu, lối gác rộng khoảng 1,2 m, cánh L rộng khoảng 1,5 m và cao khoảng 1,1 m, P cách L khoảng 0,5 m, ĐK cách vùng thao tác khoảng 6 m. Đây là tham số dựng thử, không phải kích thước thực địa hoặc thông số đã được xác thực. Không dùng công thức vận tốc làm đáp án.

### 3.4. Diễn biến và dấu vết hiện trường

Khải và Tư nói chuyện tại hậu đài. Tư lên gác và bật điện thoại trên giá để quay cận thao tác sửa R-12, làm tài liệu nghề như các buổi sửa khác; Khải tới ĐK. Điện thoại và các đoạn hướng dẫn cũ được giới thiệu ngay lượt khám xưởng đầu tiên, không xuất hiện bất ngờ lúc chốt hồ sơ. Khải giữ D làm C nhả, sức dựa của Tư làm L mở. Tư trượt khỏi gác. Khải buông D, L hồi đóng và C cài. Khải đi lên gác, quấn dây đạo cụ quanh lan can, rồi đặt rối tập cạnh nạn nhân để gợi hình ảnh vướng dây kéo rối rơi.

Con rối được đặt cạnh nạn nhân là **rối tập R-03 của xưởng**, không thuộc ba rối cổ đã chuyển đi. Tư đang sửa **rối sân khấu lớn R-12**. Thẻ buổi sửa, dây treo còn tại chỗ và nhãn đạo cụ cho phép phân biệt hai con; không để chuỗi sở hữu ở trạng thái chưa quyết định.

Ba phát hiện dành cho người chơi:

| Phát hiện | Chứng cứ chính | Kết luận được phép |
|---|---|---|
| P1 — Lối mở lúc ngã | Vệt trượt từ vị trí thao tác tới mép, tư thế hình nộm và thử cơ cấu | L đã mở trong sự kiện ngã; trạng thái đóng lúc khám không tự chứng minh L đóng lúc ngã |
| P2 — Can thiệp sau ngã | Dấu giày khác đè lên vệt trượt; dây dàn dựng và R-03 không khớp buổi sửa R-12 | Có người tới mép sau cú ngã và có căn cứ về việc dàn dựng; không dùng riêng dấu giày để kết luận họ gây ngã |
| P3 — Thao tác cơ cấu và người liên quan | Đoạn quay sửa R-12; dấu tay nắm D và mảnh vải khớp chỗ thiếu ở áo thật; móc M nguyên bụi; thử cơ cấu | Đoạn quay xác định L còn đóng ngay trước sự cố và mở trong khi Tư đang sửa; mảnh vải nối người thao tác với Khải. Mỗi nguồn trả lời một câu hỏi, không thay thế nhau |

Chi tiết P3:

- **Đoạn quay sửa rối thay căn cứ dầu mới để xác định trạng thái trong sự kiện.** Clip liên tục, không cắt ở khoảng trước–trong sự cố. Góc cận thấy phần C và L cùng hai tay Tư đang giữ dụng cụ/dây R-12: cánh đang đóng và chốt đang cài, rồi chốt nhả, cánh mở bất ngờ dưới sức dựa, Tư mất chỗ tựa. Không thấy ĐK, không thấy mặt hay tay Khải; không có lời thú nhận. Clip không tự chỉ người thao tác và không chứng minh ý định.
- Clip gốc nằm trong điện thoại còn tại xưởng, thu cùng các vật chứng ở lượt khám đầu. Việc liên quan đến đúng sự kiện được đối chiếu qua R-12, dụng cụ, vị trí giá quay và diễn biến hiện trường; không dựa riêng ngày giờ điện thoại. Người chơi được xem, tua và ghi các quan sát ngay, không khóa clip tới cuối và không bắt phục hồi một tệp bí mật.
- Clip không có đủ góc để thay khám hiện trường: P1/P2 vẫn phải kiểm bằng mặt sàn, dấu giày, R-03/R-12 và cơ cấu thật. Quan sát “đang đóng rồi nhả ngay trong sự kiện” là điểm phân biệt với giả thuyết hai người mở trước. Không suy thời điểm này từ mảnh vải hoặc dấu dầu.
- Giữ dấu dầu và vết kéo như dấu tiếp xúc; không gọi chúng là đồng hồ xác định lúc gây án. Ghi chú bảo dưỡng chỉ thu hẹp khoảng tiếp xúc.
- Thay sợi nylon đại trà bằng **mảnh mép vải có đường may bị xé**, mắc vào khe tay nắm. Người chơi đối chiếu hình mép rách, đoạn chỉ và họa tiết dệt với phần thiếu ở cổ tay áo Khải. Màu tương tự không đủ. Cần xem được áo thật trong bước thu vật chứng, không chỉ nhìn áo từ xa rồi coi là đã khớp.
- Thiết kế đây là sự khớp vật thể của mảnh rách với chỗ thiếu, không là kết luận “vân tay”, không là bằng chứng tự xác định thời điểm. Giám định có thể xác nhận thêm; không phải một đáp án bí mật xuất hiện cuối.
- Dấu giày có chỗ mòn và vết khuyết gót được đối chiếu với giày thu ở homestay. Đó là một nguồn hỗ trợ độc lập với mảnh vải, không được mô tả thành nhận diện tuyệt đối chỉ vì cùng loại giày.
- Trước khi thử, người chơi chụp/ghi vật chứng. Bàn mô hình hoặc bản sao cơ cấu dùng cho các lần thử. Nếu cần kiểm trực tiếp, hiển thị dấu thử mới tách biệt; không cho Khang bước lên phá dấu cũ rồi dùng dấu đã nhiễu làm căn cứ.
- Hai chén trà là bối cảnh. Vết xước chốt không còn là chứng cứ ai đó lên đóng lan can: **lan can mới tự đóng**. Bỏ yêu cầu kiểm kê toàn xưởng, lớp 0 và dấu âm tính về người thứ hai như chìa khóa bắt buộc của P3.

### 3.5. Các giả thuyết bắt buộc được thử

| Giả thuyết | Cách kiểm và giới hạn |
|---|---|
| Vấp dây qua lan can đóng | So đường trượt và thử tư thế; không bác bằng một ngưỡng vận tốc tự đặt |
| Chốt hỏng hoặc cơ cấu hồi hỏng | Khám cơ cấu trước thử nghiệm, thử nhả–hồi–cài; đối chiếu dấu hỏng giả định với hiện trường |
| Tư tự dùng móc M | Có thể chạy trên mô hình; khác trạng thái móc giữ nguyên bụi ở hiện trường |
| Mở sẵn rồi buông dây, rời đi | Chạy đủ thời gian hồi và quãng đường; không còn trạng thái mở đứng yên của v3.5 |
| Khải giúp mở trước, Tư đẩy, rồi vô tình ngã về sau | Thử đúng hai nhân vật đã có, kể cả Tư giữ cánh bằng sức dựa. Đối chiếu clip thấy chốt vẫn cài và L đóng trước sự cố, loại trạng thái đang mở sẵn. Nếu đã đóng lại rồi mở lần nữa ngay lúc ngã thì đó vẫn là lần nhả chốt trong sự kiện, không còn giải thích bằng riêng lần mở trước. Chưa chứng minh ý định giết người |
| Người chỉ tới sau ngã | Giải thích được một phần P2, chưa giải thích cú ngã và các vật chứng thao tác. Không tự đồng nhất người dàn dựng với người gây ngã nếu thiếu P3 |
| Khải giữ D khi Tư dựa, rồi dàn dựng | Là diễn biến thật; phải phù hợp cả cơ cấu, dấu vết và đối chiếu vật chứng |
| Vật chèn, nút giữ khác hoặc đồng phạm | Chỉ loại bằng khám vật chèn/điểm giữ và đường tiếp cận có thể kiểm tra; không dùng quy tắc “không có trong danh sách thì sai”. Nếu prototype bộc lộ một cách hợp lý chưa giải đáp, ngừng chốt mắt xích gây ngã |

**Ranh giới kết luận:** có người sửa hiện trường không đồng nghĩa cú ngã chắc chắn là một vụ cố ý giết người. Một sự cố khi hai người cùng vận hành vẫn là giả thuyết phải phân biệt với ý định; hồ sơ bản này không yêu cầu kết luận ý định. Mục tiêu là xác định vai trò thao tác và việc dàn dựng bằng chuỗi chứng cứ, không tạo ra chứng cứ tuyệt đối.

### 3.6. Thời gian thật và thông tin người chơi biết

| Thời điểm thật | Sự kiện | Nguồn người chơi có thể thu |
|---|---|---|
| 17:30–20:05 | Khải ở homestay, ăn tối | Chủ homestay, khách cùng bàn; không coi sổ chìa tự chứng minh mọi phút |
| Khoảng 18:00 | Tư ghi bảo dưỡng cơ cấu | Sổ nghề và đồ bảo dưỡng; không tự chứng minh chính xác giờ một vết tiếp xúc |
| 20:05 | Khải giao chìa rồi ra ngoài | Sổ ra vào |
| Khoảng 20:25 | Khải tới xưởng | Sự thật tác giả; người chơi phải ghép chứng cứ, không tự được ghi giờ này |
| Khoảng 20:40 | Cú ngã | Thời điểm tác giả; thông tin khám chỉ cho khoảng 20:00–22:00 |
| Khoảng 21:00 | Lan thấy bóng người rời hẻm về hướng homestay | Bà không thấy mặt; chỉ hỗ trợ |
| 22:15 | Khải nhận lại chìa | Sổ ra vào |
| 06:30 hôm sau | Hàng xóm phát hiện | Lời khai mở đầu |

### 3.7. Ba chuỗi chứng cứ và động cơ

**Chuỗi 1 — gửi mật thư.** Nhóm người biết bảng tư thế chỉ thu hẹp phạm vi. Người nhận không được tự loại khỏi người gửi. Đối chiếu nét vẽ là gợi ý. Tại homestay, người chơi tìm các bản tập vẽ cùng câu thư bị sửa và phần giấy còn lại có mép xé khớp một dải đã thu. Nội dung nháp và khớp mép giấy là chứng cứ chính; giấy cùng loại hoặc phong cách giống nhau chỉ bổ trợ. Vị trí này thay việc buộc phải so nét của mọi thành viên trên tường nhà hát.

**Chuỗi 2 — hiện diện và can thiệp sau ngã.** Dấu giày đè vệt trượt xác định thứ tự, giày thu ở homestay hỗ trợ nhận diện, R-03/R-12 và dây dàn dựng hỗ trợ hành vi. Sổ ra vào, bóng người Lan thấy và lời Khải phủ nhận từng tới xưởng chỉ hỗ trợ; cơ hội và lời nói dối không tự chứng minh gây ngã.

**Chuỗi 3 — thao tác gây ngã.** P1 + đoạn quay sửa rối + kiểm cơ cấu + đối chiếu mảnh vải/áo và giày + P2 hình thành chuỗi suy luận về Khải. Clip cho biết L đóng và C cài trước khi nhả trong chính sự kiện; khám cơ cấu và móc M kiểm nguyên nhân nhả; mảnh vải và giày hỗ trợ xác định người thao tác. Mảnh vải chỉ chứng minh tiếp xúc cụ thể; không phải đồng hồ hay hình ảnh người kéo. Độ nhận diện là kết luận tổng hợp của vật chứng, sự hiện diện và hành vi che giấu, không là định danh tuyệt đối từ một mẩu vải. Không gọi một vật riêng lẻ là “chứng cứ trực tiếp Khải kéo đúng giây nạn nhân ngã”. Kết luận chỉ được chấm đủ khi giả thuyết đối chiếu liên quan đã được giải đáp bằng quan sát người chơi có, không bằng tri thức của đội viết.

**Động cơ và quá khứ:** giữ sáu nguồn sau, mã chỉ dùng trong nội dung/kiểm thử.

| ID | Nguồn | Cho biết và giới hạn |
|---|---|---|
| M1 | Sổ Henri ở nhà hát | Bộ rối nguyên bản, ý nguyện và Kho Sáu; không tự nói có rối bị bán |
| M2 | Phiếu hàng của Hà | Ba gói vận chuyển đúng ngày trong bản thỏa thuận; không tự nói hàng trong thùng |
| M3 | Biên bản gánh do Mai giữ | Khải bị quy tội mất rối và bị đuổi; không tự chứng minh anh oan |
| M4 | Lời Lan trước chốt hồ sơ | Khải hỏi kho và gây sức ép; Lan nói phần cốt lõi của vụ chuyển rối |
| M5 | Bản nháp thỏa thuận và bản dịch | Ba rối được định danh, ngày giao, giá; chưa xác nhận thanh toán |
| M6 | Sổ phác Khải | Tính giá và dòng “bù cho mười hai năm”; động cơ, không chứng minh gây ngã |

Trước chốt, người chơi có đủ căn cứ rằng ba rối đã được chuyển đi theo thỏa thuận mua bán và Khải có sự oán giận. Lời Mai về việc Khải phản đối và lời Lan xác nhận đối chiếu với M3; không để câu “bị đổ oan vì phản đối” chỉ có trong bản dành cho tác giả. Việc thanh toán và vai trò riêng của Lan có thể được nói đầy đủ ở kết thúc, không là chứng cứ thiếu mà ending tự cung cấp để hợp thức hóa hồ sơ.

## 4. Địa điểm và nhịp điều tra

Giữ bối cảnh hiện có: Hồ Xuân Hương/cầu Ông Đạo, chợ đêm, Trường Đà Lạt/lớp 203, dốc lấy cảm hứng Dốc Nhà Bò. Chúng là nơi sống và dấu tích nghề cũ của các nhân vật, không còn là khu mơ.

| Địa điểm | Nội dung cần có | Phạm vi dựng |
|---|---|---|
| Hồ/cầu | Bảo, thư, bệ xem vật | Tái dùng khu đã có; không đổi pose tựa lan can thành câu cá |
| Chợ | Hà, phiếu hàng, thư, nhận/trả thông điệp | Tái dùng quầy và tuyến đi đã có |
| Trường/lớp 203 | Mai, bản dịch, biên bản, thư | Thư lẫn trong hộp đạo cụ cũ Khải gửi lại qua người giao; sổ nhận hộp cung cấp đường kiểm tra |
| Dốc/nhà Tùng | Tùng, hành lý, lời khai chuyến giao cũ | Không dùng việc định rời thành phố làm chứng cứ có tội |
| Xưởng | Cơ cấu, vật chứng, bàn dựng lại | Không gian mới gọn, ưu tiên đầu tiên |
| Vườn Lan | Thư cuối, lời khai và nhu cầu được bảo vệ | Một khoảng sân/lối vào; không cần mở khu vườn lớn |
| Nhà hát | Sổ Henri, nguồn bảng tư thế | Một phòng hậu đài; không xây toàn nhà hát |
| Homestay | Sổ ra vào, nháp thư, áo và giày | Quầy tiếp tân và một phòng |
| Văn phòng điều tra | Mục tiêu, hồ sơ, chốt kết luận | Một phòng, thay chức năng hub cũ |
| Ga | Giao nhận, hành động kết thúc | Một khu vực hẹn; không dựng toàn ga |

Luồng chính: văn phòng → xưởng → các đầu mối hồ/chợ/trường/dốc theo thứ tự linh hoạt → nhà hát/homestay khi có đầu mối → Lan → chốt hồ sơ → cuộc hẹn tại ga theo điều đã kết luận.

Không buộc người chơi nhặt đủ năm thư mới giải. Mỗi lần trở lại một nơi phải có câu hỏi hoặc phản hồi mới. Không kéo thời lượng bằng chờ hoặc đi bộ không có thông tin. Bốn khu cũ được giữ; các nội thất mới giới hạn như bảng trên.

## 5. Mật mã tư thế

### 5.1. Quy tắc chốt

Một tư thế ứng với một chữ A–Z. Dấu ngắt từ là **chấm đặc ở giữa chiều cao dòng**, không là tư thế, không kiêm chữ. Một con rối nhỏ làm dấu nhận biết gánh; dải giấy mang chuỗi ký tự. Không bắt người chơi đoán một tư thế duy nhất là cả từ.

Giao diện tiếng Việt. Từ vựng tiếng Anh có bản chú giải tiếng Việt luôn tra được; thử thách là ánh xạ ký hiệu và ngữ cảnh, không là bài kiểm tra trình độ Anh ngữ.

Mỗi thư mở bằng STAND BY, được giới thiệu qua bảng tập hoặc lời Mai để làm điểm bắt đầu. Tần suất là gợi ý bổ sung, không cam kết chữ E nhiều nhất trên một dải ngắn. Người chơi gán chữ, xem bản đọc thử, được sửa; không có đèn báo đúng từng chữ.

### 5.2. Nội dung thư dùng cho bản đầu

| Người nhận | Bản rõ tiếng Anh | Nghĩa trong game |
|---|---|---|
| Bảo | STAND BY. WHO KEEPS THE STORE KEY? ASK LAN. KEEP QUIET. | Ai giữ chìa kho? Hỏi Lan. Giữ kín. |
| Hà | STAND BY. WHERE DID THE THREE CRATES GO? SEND WORD. | Ba thùng đã đi đâu? Gửi câu trả lời. |
| Mai | STAND BY. FIND THE OLD STORE NOTE. SEND WORD. | Tìm ghi chú kho cũ. Gửi câu trả lời. |
| Tùng | STAND BY. DO YOU STILL SEE LAN? SEND WORD. | Còn gặp Lan không? Gửi câu trả lời. |
| Lan | STAND BY. BRING THE OLD PUPPETS TO THE STATION. NINE AT NIGHT. OR THE OLD SALE COMES OUT. | Mang rối cũ tới ga lúc chín giờ tối, nếu không chuyện bán rối sẽ bị phơi ra. |

Dấu chấm câu trong bảng trên phục vụ người viết. Khi mã hóa, chỉ A–Z và dấu ngắt từ; bản chép dễ đọc khôi phục câu. Không mã hóa chữ số chưa có bảng. “Chín giờ” là giờ của ngày hẹn sau điều tra, không là bộ đếm thời gian thực; nếu đi chậm, game không âm thầm làm lỡ vụ án.

Nhắn lại: **STAND BY. AGREED. STATION. NINE AT NIGHT.** Người chơi ghép các cụm đã hiểu và xếp vài tư thế then chốt trên bàn; không phải kéo chính xác mọi khớp cho cả đoạn. Bản giả lời đồng ý được gửi qua điểm nhận/trả giấy tại quầy Hà đã được giới thiệu bằng lời khai và phiếu nhận. Không để người chơi tự biết một kênh liên lạc chưa từng xuất hiện.

### 5.3. Tư thế và quyền trợ giúp

Bảng đủ 26 tư thế là tài nguyên nội dung phải hoàn thành trước demo đầy đủ. Prototype chỉ cần tập ký tự của thư Hà và đáp án rút gọn. Đầu, hai tay, hai chân có hình bóng rõ; không phân biệt chỉ bằng màu hoặc sai lệch góc rất nhỏ. Dùng cùng pose ID cho giấy, rối và sổ để tránh lệch ánh xạ.

Hai đường giải đều hợp lệ: tự suy ra từng phần; hoặc tìm bảng Henri để hỗ trợ/tra cứu. Tìm sổ không làm mất điểm hay ending. Có ba mức gợi ý tự mở: nhắc chi tiết đã thấy → gợi quan hệ → chỉ dẫn một bước cụ thể. Không tự điền cả vụ án vào hồ sơ.

## 6. Vòng lặp và mini game

Vòng lặp: có câu hỏi → khám/xem → hỏi lời khai → đối chiếu → thử giả thuyết → cập nhật suy luận → quyết định còn thiếu gì.

Các mini game đều phải cung cấp hoặc kiểm một thông tin cho vụ án:

1. Giải mật mã tư thế, mở đầu mối.
2. Ghép mép giấy và bản nháp, phân biệt nguồn thư.
3. Đối chiếu phiếu hàng, ký hiệu rối và bản dịch, dựng đường đi ba thùng.
4. So R-03/R-12 với buổi sửa, phát hiện dàn dựng.
5. So dấu giày và mảnh rách áo với vật chứng thật; cho phóng to và mô tả, không yêu cầu thị lực tinh hoặc kéo khớp từng pixel.
6. Dựng lại cơ cấu bằng hình nộm, kiểm các diễn biến đối chiếu ở mục 3.5.
7. Đối chiếu lời khai và tài liệu để hỏi lại NPC.

Không thêm khóa số, trò phản xạ hoặc trò nối hình chỉ để đủ số mini game. Sai trong thử nghiệm không game over, không trừ điểm ẩn và không làm mất vật chứng.

## 7. Dựng lại và hồ sơ

### 7.1. Thử giả thuyết

Người chơi chọn nhân vật, vị trí, thao tác và trạng thái cơ cấu từ một bộ tùy chọn rõ ràng. Thử mở trước, thả dây, giữ dây, dùng móc, đóng/mở, ngã, đến sau. Prototype dùng bộ tình huống hữu hạn; không hứa mô phỏng mọi vật lý tự do.

Phản hồi là cánh hồi đóng, dây chùng/căng, hình nộm ngã ở đâu và dấu kết quả khác hiện trường ở đâu. Không hiện chữ “hung thủ đúng” hoặc chấm xanh ngay khi chọn Khải. Mô hình là công cụ kiểm giả thuyết, không là camera tua lại sự thật.

Kết quả không được phụ thuộc ngẫu nhiên ragdoll. Dùng trạng thái xác định để tái lập cơ cấu; chuyển động hình nộm có thể minh họa có kiểm soát. Kiểm vật lý riêng để bảo đảm minh họa không mâu thuẫn hình học. Mô hình đúng theo code không tự xác nhận vật lý hoặc suy luận đúng ngoài đời.

### 7.2. Hồ sơ vật chứng và ghi chú

Ghi tách: quan sát; lời người khác kể; giả thuyết người chơi. Mã P/V/R chỉ là nội bộ. Chưa xem nội dung thì sổ không tiết lộ nó. Vật chứng có ID bền vững, ảnh/mô tả, nơi thu và quan hệ trước–sau khi có căn cứ.

Bảng liên hệ cho xem hai mục cạnh nhau và ghi nhận quan hệ. Đánh dấu một quan hệ không tự biến nó thành sự thật đã xác nhận.

### 7.3. Bốn kết luận khi chốt

| Kết luận | Đáp án nội dung | Nhóm chứng cứ chống đỡ |
|---|---|---|
| Người gửi và yêu cầu thư | Khải dò bộ rối và ép Lan giao nhận | Bản rõ liên quan + nháp và mép giấy khớp; kiến thức bảng chỉ hỗ trợ |
| Có mặt và dàn dựng | Khải tới xưởng, lên sau ngã và bố trí lại | P2 + đối chiếu giày + R-03/R-12; lời khai và sổ ra vào bổ trợ |
| Vai trò trong cú ngã | Khải thao tác nhả cơ cấu khi Tư ở mép; sau đó che giấu | P1 + clip liên tục thể hiện trạng thái chốt/cánh + khám/thử cơ cấu và móc + P3 đối chiếu áo thật + hiện diện/P2 và kiểm giả thuyết đối chiếu; không kết luận ý định từ đây |
| Động cơ | Oán giận vì bị đổ oan, tự coi rối là khoản bù | Nguồn quá khứ M1/M3/M5 và lời đối chiếu + M6; M2/M4 nối vận chuyển và sức ép |

Người chơi chọn “Kết luận” và gắn căn cứ, hoặc “Chưa đủ chứng minh”. Không yêu cầu gắn đúng thứ tự hay mọi chứng cứ bổ trợ. Chấp nhận các bộ chứng cứ tương đương được đặc tả trong dữ liệu, không chấm theo số vật nhặt.

Trước chốt chỉ báo đã gắn/chưa gắn và nội dung lựa chọn, không báo đáp án. Có xác nhận trung tính và save riêng ngay trước chốt. Sau kết thúc, giải thích được quan hệ nào chưa đủ. Không trừ điểm cho đi chậm, tra bảng, đọc nhiều lần hoặc chọn chưa đủ.

**Điều kiện duyệt chuỗi gây ngã là gate nội dung:** nếu prototype còn một diễn biến hợp lý dùng chính Khải/Tư và vật đã có mà phù hợp toàn bộ chứng cứ, đội phát triển chưa được gán bộ chứng cứ đó thành đủ. Đây là kiểm trước phát hành, không đưa thành thông báo kỹ thuật cho người chơi.

### 7.4. Quy trình và hành động

Thu áo, giày và giấy trong phòng Khải qua một bước cho phép truy cập được thể hiện rõ trong truyện. Phương xử lý liên lạc và yêu cầu thu vật chứng. Chưa kiểm nguồn nghiệp vụ nên không đóng cứng tên văn bản, thẩm quyền hay thời hạn như quy trình thật. Không dùng một thao tác lỡ bấm để vô hiệu toàn bộ chứng cứ mà không báo trước.

## 8. Ending và quyền quyết định

Chốt quy tắc ưu tiên: có kết luận khẳng định sai hoặc không đủ căn cứ → A-sai; nếu không có khẳng định sai/thiếu căn cứ nhưng còn mục “Chưa đủ chứng minh” → A-treo; nếu cả bốn kết luận đúng và được chống đỡ → C.

**C — Hồ sơ được hoàn tất.** Khang và đồng nghiệp ngăn cuộc ép giao rối tại ga, bảo vệ Lan và bộ rối. Chuỗi thao tác gây ngã và dàn dựng được đưa vào hồ sơ để tiếp tục xử lý. Lan nói đầy đủ vai trò của mình. Không dùng màn kết thúc để tuyên một tội danh hoặc bản án mà gameplay chưa chứng minh.

**A-treo — Điều tra còn mở, kết luận trung thực.** Khang không bắt ai vì điều mình ghi chưa đủ chứng minh. Lan được bảo vệ, bộ rối được giữ an toàn; những đầu mối đã có được tiếp tục theo dõi. Hồ sơ chỉ ra câu hỏi chưa khép. Kết này không gọi người chơi thất bại đạo đức và không làm mất rối để phạt sự thận trọng.

**A-sai — Khẳng định vượt chứng cứ hoặc sai người.** Khang hành động theo khẳng định cụ thể đã lập. Nếu chỉ sai người gửi thư, hậu quả trước hết là hướng điều tra và niềm tin bị tổn hại; **không tự bắt người về gây chết vì một lỗi ở mục động cơ**. Nếu người chơi khẳng định sai người gây ngã và đề nghị xử lý trên hồ sơ ấy, kết thúc thể hiện việc cáo buộc bị bác/đình lại, tổn hại người bị nghi và vụ án chưa khép. Lan vẫn được bảo vệ theo nguy cơ đã biết; không mặc định Khải được thả là bộ rối chắc chắn bị mất.

Hành động phải theo đúng loại kết luận đã chọn, không gom mọi sai sót thành cùng một vụ bắt oan. Không có Yes/No “chọn kết đẹp”; diễn biến đến từ hồ sơ. Có thể load save trước chốt để thử lại, không ghi đè save cũ của hướng game trước.

## 9. Vật phẩm, UI và điều khiển

Giữ hệ cầm–xem–đặt–cất và điện thoại nếu có thể tái dùng mà không kéo theo logic mơ. E hiển thị động từ theo vật; R trả/cất theo ngữ cảnh đã thống nhất. Nhãn pause/save/load phải đọc từ cấu hình đang dùng, không chép phím cũ không còn đúng.

Khi xem vật: xoay, phóng, đặt lại, cất; ngừng input đi/nhìn. Khi so hai vật: khóa góc xem hữu ích nhưng cho đổi góc. Có bản chép chữ và mô tả hình; không chỉ phân biệt bằng màu. Không yêu cầu drag chính xác, thao tác nhanh hoặc timer.

Vật chứng được ghi vào hồ sơ và đặt ở vị trí hợp lệ; không cho quăng mất ra ngoài map. Không áp giới hạn ba vật mỗi khu của hướng cũ. Điện thoại chứa liên lạc, việc trước mắt và hồ sơ, không tự phát đáp án chưa khám.

## 10. Hình ảnh, âm thanh và tài nguyên

Đà Lạt đời thường, ánh sáng đọc được; chợ có âm đám đông phù hợp. Tiếng gỗ, dây, giấy, vải và cơ cấu hỗ trợ việc hiểu thao tác. Không dùng thiếu âm thanh hoặc nhòe mặt như dấu hiệu siêu nhiên cũ.

Rối có khớp và điểm gắn dây rõ, không bộ phận lơ lửng. Có thể tái dùng rig/dây nếu kiểm đáp ứng tư thế; không giữ mặt Minh như bắt buộc. Danh tính vật chứng ổn định giữa Blender, prefab và scene.

Asset mới theo trình tự: nguồn Blender editable → lưu/mở lại → FBX → Unity prefab/material/collider → scene dẫn xuất → kiểm hiển thị và controller. Không gọi model ước lượng là tái dựng thực địa chính xác.

Không tiếp tục sửa terrain, nhà hoặc bridge chỉ vì chuyển GDD. Giữ các vùng map đã chốt và checkpoint. Bản dựng kỹ thuật chỉ dùng nội thất cần cho prototype; không dựng cả ga hoặc nhà hát trước khi vòng chơi xưởng đạt.

## 11. Lưu/tải và cách ly hướng cũ

Dùng schema/save namespace riêng, không migration tự động save v2.2. Lưu: ID vật chứng, trạng thái xem/thu/đặt, mục đang cầm, lời khai đã nghe, ánh xạ mật mã, quan hệ ghi chú, giả thuyết đã thử, căn cứ gắn và trạng thái kết luận. Load giữa mini game trở lại trạng thái an toàn không nhân đôi hoặc mất đồ.

Phân biệt sự thật vụ án cố định, quan sát đã mở và kết luận người chơi; không dùng cờ đã xem thay cho cờ đã chứng minh. Ending không làm thay đổi dữ liệu sự thật. Pause dừng mô phỏng và input phù hợp; load cơ cấu phải tái lập được trạng thái.

Không tuyên bố kế thừa PASS của r90–r98 cho gameplay mới. Không sửa save gốc, migration Bảo hoặc ZIP cũ trong công việc làm tài liệu này.

## 12. Phạm vi chốt và thứ tự thực hiện

1. Prototype tách biệt: xưởng → quầy Hà → quay lại xưởng; có một cảnh gặp Khải và vật mẫu để đối chiếu.
2. Kiểm cơ cấu và suy luận trước; sau đó kiểm người mới hiểu được vòng chơi.
3. Làm lát cắt chơi xuyên suốt: văn phòng, xưởng, hồ/chợ, tiếp tân/phòng homestay tối thiểu, khu hẹn gọn; có hồ sơ và ba diễn biến ending. Các chứng cứ của lát cắt phải nằm trong những nơi thật sự được dựng; không tham chiếu tài liệu chỉ có ở khu đã cắt.
4. Mở đầy đủ trường, dốc, vườn và hậu đài nhà hát theo luồng chính; bổ sung tất cả thư và lời khai.
5. Sau hồi quy và review mới quyết định build chơi thử. Không tự tạo runner/build mới chỉ vì chốt GDD.

Không đặt mục tiêu số giờ bằng cách kéo dài đường đi. Số phút cần đo sau prototype. Không thêm vụ thứ hai hoặc kết thúc mới trước khi vụ đầu hoạt động trọn vẹn.

## 13. Prototype đầu tiên — đầu vào và tiêu chí

Đầu vào nội dung: một câu chuyện mở đầu; cảnh Khải để nhớ áo/giày; một thư Hà; bản nháp/giấy mẫu; R-03 và R-12; điện thoại với clip sửa rối được giới thiệu từ đầu; cơ cấu mới; chứng cứ P1–P3; một bảng hồ sơ nhỏ có lựa chọn chưa đủ.

Những nơi chưa dựng không xuất hiện như yêu cầu ẩn. Mẫu áo/giày trong prototype được thu qua bước điều tra rút gọn có giải thích; không giả vờ đã kiểm homestay của game đầy đủ.

Người chơi cần tự nói bằng lời thường:

- Vì sao lan can đóng lúc khám không có nghĩa nó đóng lúc ngã.
- Vì sao clip loại trạng thái đã mở sẵn; cơ cấu hồi giải thích được những lần mở–thả, nhưng không tự nhận diện người kéo.
- Vì sao có căn cứ dàn dựng sau ngã.
- Mảnh vải và giày nối với Khải thế nào, và chúng chưa chứng minh điều gì.
- Vì sao kết luận vai trò thao tác không đồng nghĩa tự chứng minh ý định giết người.

Không kiểm bằng việc nhớ mã P/V/R. Không yêu cầu người chơi nói lại đúng câu đội viết.

## 14. Gate kiểm chứng trước khi triển khai rộng

Tất cả các mục dưới đây đang ở trạng thái **CHƯA CHẠY** trong tài liệu này.

| Nhóm | Ca kiểm | Điều kiện đạt |
|---|---|---|
| Cơ cấu | Cài, kéo, thả, hồi, dùng móc; lặp nhiều lần | Trạng thái tái lập, không có mở thụ động ngoài luật |
| Thời gian hồi | Khải/Tư mở trước, thả, Tư đổi vị trí rồi ngã | Kiểm thời gian hữu hạn; không còn cửa hợp lý bị bỏ sót để giữ kết luận sai |
| Giả thuyết hai người | Khải giúp mở và Tư đẩy trước; sau đó tai nạn | Đối chiếu được với L đóng/C cài trong clip; kiểm cả lực Tư giữ cánh; không dùng thiếu vật chứng để suy ra cố ý |
| Clip sự kiện | Đọc chốt, cánh, hai tay Tư và tính liên tục | Góc quay thật sự cho thấy các quan sát được ghi; clip không lộ người kéo, không đưa đáp án danh tính; không coi chữ mô tả là thay thế khung hình |
| Vật chèn/điểm giữ | Thử các vật có thật và các điểm buộc trong phạm vi cơ cấu | Có câu trả lời quan sát được; nếu tồn tại cách phù hợp chưa giải đáp thì chuỗi gây ngã chưa đạt |
| Hiện trường | L hồi đóng nhưng người đến sau dàn dựng | P2 vẫn đọc được; không còn dựa vào vết cài chốt để nói có người đóng L |
| Đối chiếu | Áo cùng màu, vải cùng loại nhưng không khớp mép; giày gần giống | Không coi tương tự chung là cùng một vật |
| Suy luận | Chỉ P2; chỉ P3; đủ chuỗi; suy ý định | Không đủ thì cho chưa đủ; không tự suy chủ ý gây chết |
| Kết thúc | Đủ bốn mục; bỏ ngỏ; sai thư; sai động cơ; sai người gây ngã | Hành động đúng loại kết luận, không tự bắt người cho mọi lỗi |
| Cầm/xem | Mở UI, trả vật, đặt sai, đổi khu | Không mất hoặc nhân đôi chứng cứ, input rõ |
| Lifecycle | Pause, save/load giữa cơ cấu, ghép giấy, chốt hồ sơ | Tái lập trạng thái, không lộ nội dung chưa xem, không đụng save cũ |
| Người mới | Chơi mà chưa đọc GDD, được phép phản biện | Hiểu ba phát hiện và nêu được căn cứ; ghi lỗi thực thay vì mặc định đạt |
| Nguồn quy trình | Tên thủ tục và cách xử lý cuối | Chỉ dùng nghiệp vụ đã kiểm nguồn; nếu chưa thì giữ mô tả trung tính |

Nếu gate cơ cấu/suy luận không đạt, sửa đúng cơ cấu hoặc vật chứng liên quan và kiểm lại. Không tự thêm hàng loạt đồ, NPC hay nhánh để che mâu thuẫn. Thay đổi ảnh hưởng lời giải phải cập nhật cùng lúc cơ cấu, vật chứng, hồ sơ và ending.

## 15. Khác biệt quan trọng với v3.5

- Thay cánh không tự đóng bằng cơ cấu tự hồi, xử lý trực tiếp giả thuyết mở trước rồi bỏ đó.
- Giữ thời gian hồi hữu hạn là yêu cầu kiểm, không biến nó thành phép đóng tức thời thuận tiện cho tác giả.
- Không còn chứng cứ “lên gác đóng L”; lan can tự đóng. P2 dựa dấu sau ngã và đạo cụ dàn dựng.
- P3 là chuỗi suy luận từ clip, vật chứng và cơ cấu. Đoạn quay sửa rối là nguồn xác định trạng thái ngay trong sự kiện; không gọi mảnh vải hay dầu là bằng chứng trực tiếp của đúng giây gây ngã. Clip không nhận diện người kéo.
- Thay sợi vải phổ biến bằng đối chiếu mảnh rách; thêm mép giấy và nháp thư thay việc chỉ nhận diện nét vẽ.
- Tách vai trò gây ngã, dàn dựng và ý định. Ý định không là điều kiện ngầm của ending C.
- Chốt bốn kết luận và quy tắc C/A-treo/A-sai; lỗi động cơ không tự gây một vụ bắt oan về cái chết.
- R-03/R-12, đoạn quay sửa rối được giới thiệu ngay đầu, năm bản rõ mật thư, kênh hồi đáp, phạm vi nội thất và prototype đã có đáp án làm việc cụ thể.
- Bỏ lịch sử tranh luận và công thức vận tốc khỏi đặc tả hiện hành. Bản cũ được giữ riêng để tra cứu.

## 16. Quyết định cuối cùng và việc còn lại

**Đã chốt để làm:** hướng điều tra thuần thực; một vụ án; Khang; gánh rối và vụ chuyển bán; Khải; năm khu đầu mối; bảng tư thế; cơ cấu tự hồi và clip sự kiện; P1–P3; hồ sơ bốn mục; C/A-treo/A-sai; namespace save mới; prototype xưởng trước khi mở rộng.

**Cần sản xuất, không còn là lựa chọn cốt truyện bỏ ngỏ:** hình 26 tư thế, lời thoại theo từng NPC, bản ghi vật chứng, scene xưởng, asset cơ cấu và UI. Các tài nguyên này phải theo đáp án đã chốt, được kiểm khả năng đọc và tính nhất quán trước tích hợp.

**Cần kiểm chứng trước phát hành:** kích thước và hồi cánh trong engine; giả thuyết thay thế; độ đọc dấu vết; hiểu của người mới; nghiệp vụ và trình bày cuối; hiệu năng và lifecycle. Không gọi chúng là PASS bằng việc tài liệu đã hoàn thành.

Chốt nội dung có nghĩa ngừng mở rộng hoặc đổi hướng theo từng vòng góp ý chung. Từ đây chỉ sửa khi prototype, kiểm nguồn hoặc playtest chỉ ra vấn đề cụ thể có thể mô tả và kiểm lại.
