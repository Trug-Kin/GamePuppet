# BỘ NGUỒN THAM CHIẾU DỰNG MÔ HÌNH 3D – KHU VỰC CHỢ/ĐƯỜNG PHỐ ĐÀ LẠT

## 1. Mục tiêu
Dùng 41 ảnh trong thư mục `Photo/` làm nguồn tham chiếu trực quan để dựng một cảnh 3D đô thị Việt Nam có vòng xoay/nút giao, đường phố, công trình thương mại, khu quầy hàng có mái che, quầy hoa/cây cảnh và quầy quần áo. Đây là bản tái tạo gần đúng từ ảnh, không phải bản đo đạc kỹ thuật chính xác.

## 2. Những gì quan sát được trong ảnh
- Ảnh 01: công trình/tượng trang trí trên bệ, chụp dọc; dùng tham chiếu cho chi tiết tượng/công trình nếu phù hợp với khu vực cần dựng.
- Ảnh 02–03: góc nhìn toàn cảnh đô thị và nút giao dạng vòng xoay/đảo trung tâm; tham chiếu bố cục tổng thể, đường dẫn, công trình xung quanh và mảng cây xanh.
- Ảnh 04–06: đường phố có cây xanh, biển/cổng trang trí, mặt tiền cửa hàng và nhà nhiều tầng.
- Ảnh 07–12: dãy công trình/quầy hàng mặt tiền, mái che, bậc thềm, cột, bảng hiệu và lối đi phía trước.
- Ảnh 13–18: tuyến đường rộng hơn, phương tiện giao thông, cây cao, dãy quầy hàng sát đường và mái che kéo dài.
- Ảnh 19–23: quầy hoa/cây cảnh với nhiều chậu cây, kệ, khung mái và cột đỡ.
- Ảnh 24–28: quầy quần áo/hàng hóa treo dưới mái che; có bảng giá/biển nhỏ.
- Ảnh 29–34: nhiều góc nhìn dọc theo dãy quầy có mái, khung kim loại, bạt che, cây cảnh và khoảng trống phía trước.
- Ảnh 35–41: ảnh chụp màn hình Google Street View, cho thấy các góc đường, mặt tiền cửa hàng, phương tiện và không gian giao thông. Khi dựng cảnh, chỉ tái tạo cảnh thực, không đưa giao diện trình duyệt/Street View vào mô hình.

## 3. Bố cục cảnh đề xuất
Dựng theo thứ tự từ lớn đến nhỏ:
1. **Mặt bằng**: đường chính, nhánh đường/nút giao, vỉa hè, mép đường và đảo giao thông nếu thấy trong góc cảnh đã chọn.
2. **Khối công trình**: nhà nhiều tầng ở nền xa, cửa hàng mặt phố và dãy quầy thấp hơn ở tiền cảnh.
3. **Kết cấu quầy hàng**: cột thép, khung mái, mái tôn/mái che, bậc thềm, nền quầy, bạt che và vách ngăn.
4. **Các cụm hàng hóa**: chậu cây/hoa, kệ, quần áo treo, thùng và vật dụng bán hàng.
5. **Cảnh quan**: cây đường phố, cây cao dạng lá kim ở một số góc, cây tán rộng, bồn cây và bóng đổ.
6. **Giao thông**: xe máy, ô tô/xe tải nhỏ, vạch đường, biển báo và các chi tiết đường phố nếu cần.
7. **Hoàn thiện**: vật liệu bê tông/asphalt, tường sơn, kính, kim loại, mái tôn, bạt, màu sắc biển hiệu và ánh sáng ban ngày.

## 4. Phong cách và mức độ chi tiết
- Phong cách: hiện thực (realistic), đô thị Việt Nam, cảnh ban ngày.
- Bắt đầu bằng low-poly blockout để khóa bố cục; sau đó tăng chi tiết ở khu vực camera sẽ nhìn thấy.
- Dùng mô-đun lặp lại cho cột, vì kèo, tấm mái, gian hàng, chậu cây và cửa hàng.
- Ưu tiên tỷ lệ nhất quán và silhouette đúng trước texture/đồ trang trí.
- Không thêm yếu tố giả tưởng, kiến trúc hiện đại không có trong ảnh, biển hiệu bịa đặt dễ đọc, hoặc chi tiết không được ảnh hỗ trợ.

## 5. Prompt tổng hợp để dùng với AI tạo model/cảnh 3D

**PROMPT (có thể sao chép):**

> Tạo một cảnh 3D môi trường đô thị Việt Nam dựa trên toàn bộ bộ ảnh tham chiếu được cung cấp. Cảnh gồm một tuyến đường đô thị và khu vực nút giao/vòng xoay ở nền hoặc trong góc tổng thể; các tòa nhà thương mại nhiều tầng ở phía xa; cửa hàng mặt phố; dãy quầy bán hàng thấp, liên tục sát vỉa hè với mái che kéo dài bằng khung kim loại và mái tôn/bạt; quầy hoa và cây cảnh với nhiều chậu cây; quầy quần áo với hàng treo; cây xanh đường phố; vỉa hè, bậc thềm, mặt đường và một số phương tiện giao thông Việt Nam. Ánh sáng ban ngày, vật liệu đời thực, tỷ lệ kiến trúc hợp lý, phong cách realistic. Dựng bố cục và khối lớn trước, sau đó thêm các mô-đun quầy hàng, cột, mái, cây, chậu hoa, quần áo và phương tiện. Giữ các chi tiết theo ảnh tham chiếu, không tự bịa địa danh hoặc biển hiệu. Nếu các ảnh không cho biết kích thước, hãy dùng tỷ lệ đô thị hợp lý và đánh dấu các kích thước là ước lượng. Không dựng giao diện Google Maps/Street View trong cảnh 3D; chỉ dùng ảnh màn hình đó làm tham chiếu cho cảnh phố. Tổ chức scene thành các nhóm riêng: Ground_Roads, Buildings, Market_Stalls, Roof_Structure, Plants_Flowers, Clothing_Stalls, Street_Trees, Vehicles, Street_Furniture. Tạo mô hình dễ chỉnh sửa, vật liệu riêng biệt, pivot hợp lý và mesh có cấu trúc rõ ràng.

## 6. Prompt cho quy trình dựng trong Unity

> Tạo environment scene cho Unity theo ảnh tham chiếu. Dùng đơn vị mét (1 Unity unit = 1 mét), mặt đất ở Y=0, trục X/Z làm mặt bằng và Y là chiều cao. Tách mesh theo đối tượng để dễ thay thế và tối ưu. Dùng prefab cho cột mái, đoạn mái, quầy hàng, chậu cây, cây đường phố, xe và biển báo. Bắt đầu bằng blockout không texture; cung cấp các kích thước dưới dạng ước lượng, không tuyên bố là số đo thực tế. Đảm bảo lối đi, mặt đường và các quầy hàng có thể điều chỉnh độc lập. Giữ polygon count vừa phải; dùng LOD hoặc mesh đơn giản cho công trình nền xa. Không tạo logic gameplay trừ khi được yêu cầu.

## 7. Các điều chưa thể xác minh từ bộ ảnh
- Chưa xác định bằng chứng chắc chắn cho tọa độ, tên đường, ranh giới địa lý chính xác và kích thước đo đạc.
- Không thể suy ra mặt sau/mặt khuất của công trình từ ảnh hiện có.
- Góc chụp, phối cảnh và ảnh Street View có thể làm sai lệch cảm nhận khoảng cách/kích thước.
- Ảnh chụp vào các thời điểm khác nhau có thể có khác biệt về xe cộ, hàng hóa, cây xanh hoặc trạng thái mái che.
- Không dùng số lượng xe/người trong ảnh làm mật độ giao thông cố định của cảnh.

## 8. Cách sử dụng bộ nguồn
1. Mở `photo_contact_sheet_full.jpg` để xem nhanh toàn bộ 41 ảnh và chọn ảnh phù hợp với góc cảnh muốn dựng.
2. Dùng ảnh 02–03 để khóa bố cục lớn; ảnh 13–18 và 35–41 để tham chiếu đường phố; ảnh 07–12 và 19–34 để dựng chi tiết quầy hàng.
3. Không cố dựng toàn bộ khu vực trong một lần nếu công cụ giới hạn. Chia thành các asset/đoạn cảnh, sau đó ghép lại.
4. Khi cần độ chính xác địa lý, bổ sung bản đồ có tỷ lệ, tọa độ hoặc số đo thực tế.

## 9. Ghi chú
Đây là bản brief dựng mô hình dựa trên quan sát trực quan. Các hình thu nhỏ trong contact sheet chỉ để lập chỉ mục; luôn mở ảnh gốc trong `Photo/` để xem chi tiết trước khi tạo mesh/texture.
