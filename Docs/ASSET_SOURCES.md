# CON RỐI — nguồn asset và quy tắc bổ sung

Cập nhật 13/10/2026 theo chỉ dẫn người dùng. Project Unity đích: `D:\Game\GameHorror\Game`; Unity **6000.6.2f1**, **URP** (`PC_RPAsset`/`Mobile_RPAsset` được gán trong Graphics/Quality Settings). Có package HDRP trong project không có nghĩa project đang render bằng HDRP. Tài liệu này được đồng bộ vào `D:\Game\GameHorror\Game\Docs\ASSET_SOURCES.md`.

## Phạm vi và thứ tự ưu tiên

- Tiếp tục hoàn thiện terrain, hồ, đường, cầu, chợ, nhà và các model hiện có theo dữ liệu địa lý, ảnh Đà Lạt và kế hoạch dựng map. Asset tải về chỉ bổ sung chi tiết; không bỏ dở, xóa hoặc thay model đang dựng bằng bộ tải sẵn.
- Ưu tiên tái sử dụng vật thể đã có. Chỉ tìm/tải phần thực sự cần cho bước đang triển khai; không tải bộ lớn, không để tìm asset làm ngắt tiến độ dựng map và tránh trùng vật thể đã dựng.
- Được tìm/tải trên mạng khi cần. Ưu tiên miễn phí và tương thích Unity 6000.6.2f1/URP; không mua, không dùng nguồn không rõ giấy phép. Trước tải, đọc giấy phép gốc để xác nhận được dùng trong game và chỉnh sửa. Ghi bằng chứng tại sổ nguồn bên dưới; chưa xác minh thì chưa tải/import.
- Asset bổ sung vào thư mục riêng, ví dụ `Assets/ConRoiSupplemental/<nguồn hoặc gói>`, và Collection Blender riêng `ConRoi_Supplemental_<nhóm>`. Giữ model địa lý/đặc trưng trong cấu trúc hiện có. Không ghi đè asset hoặc thay Project Settings ngoài phạm vi cần thiết.

## Bản sắc Việt Nam và Đà Lạt

- Giữ bố cục OSM/DEM, hình dạng và quan hệ không gian công trình đặc trưng theo ảnh tham chiếu. Chợ Đà Lạt tiếp tục dựng riêng; không thay bằng model chợ chung hoặc ghép nguyên phố nước ngoài.
- Đối chiếu ảnh đúng khu vực cho nhà phố, mặt tiền, mái, ban công, cửa cuốn, biển hiệu tiếng Việt, quầy hàng và đồ sinh hoạt trước khi chọn/chỉnh asset.
- Cây xanh, bồn hoa, đèn đường, ghế đá, vỉa hè, bờ kè phải khớp ảnh địa điểm cụ thể. Không rải thông hay phủ sương khắp nơi để giả cảm giác Đà Lạt.
- Phòng trọ là không gian sinh viên Việt Nam; tránh đồ vật/nội thất mang dấu hiệu rõ của nước khác. Asset phổ thông chỉ dùng nếu phù hợp hoặc được chỉnh thành đúng bối cảnh.
- Bám GDD v2.1 tại `D:\Game\GameHorror\Game\Docs\Design\conroi.txt`: không thêm biểu tượng nghi lễ, thực thể siêu nhiên hay kiến trúc kinh dị ngoài thiết kế. Hiệu ứng giấc mơ không dùng để che sai bố cục/kiến trúc.
- Nếu chưa có asset đúng, dùng khối tạm có nhãn hoặc dựng riêng từ ảnh. Ghi rõ phần ước lượng; không lấp chỗ trống bằng asset sai văn hóa. Ưu tiên chi tiết tuyến người chơi nhìn gần.

## Mẫu kiểm tra trước khi nhân rộng

Dựng đoạn mẫu ngắn từ model hiện có và phần bổ sung cần thiết. Xuất ảnh **ban ngày, ánh sáng trung tính**, kiểm tra bản sắc, tỷ lệ, kiến trúc và so với ảnh địa điểm. Lưu ảnh mẫu, ảnh đối chiếu, nhận xét và phần còn ước lượng. Chỉ sau khi bố cục phù hợp mới áp dụng hàng loạt, thêm ánh sáng đêm và hiệu ứng giấc mơ. Chưa tạo mẫu trong lượt cập nhật quy tắc/controller này; không coi kết quả kiểm collision là duyệt kiến trúc.

## Controller và thử di chuyển

Project đã có nhân vật góc nhìn thứ nhất `SampleScene :: Player`, dùng `PlayerMovement`, `CharacterController`, `MouseMovement` và camera con. Tái sử dụng, chưa cần tải controller.

Trong scene thử di chuyển: tắt `ComplianceSystem` cùng coroutine nhích camera; dùng input movement raw để bỏ smoothing/tăng giảm axis. Mã hiện tại chưa triển khai delay Compliance riêng 150–250 ms theo GDD; khi bổ sung sau này, chế độ thử cũng phải bypass nó. Không đổi speed, capsule, gravity, step offset, slope limit, collider hoặc global InputManager để che lỗi. Scene game giữ hành vi mặc định; các lần thử có profile khác nhau phải có log riêng, không kế thừa PASS từ profile cũ.

## Sổ nguồn asset tải mới

**Chưa tải asset ngoài trong lượt này.** Không có mục license đã duyệt để import mới. Các asset có sẵn trong project không được tự gán giấy phép hoặc tác giả từ suy đoán.

| ID / công dụng đang cần | URL trang nguồn + URL tải | Tác giả / chủ quyền | Tên/phiên bản + URL giấy phép | Dùng trong game / sửa đổi | Ghi công chính xác và nơi hiển thị | Unity/URP | Thư mục/Collection | Chỉnh sửa + phần ước lượng | Trạng thái/ngày/SHA-256 |
|---|---|---|---|---|---|---|---|---|---|

Mỗi lần tải điền đủ thông tin trên, dung lượng, file thực tế, ngày kiểm giấy phép và ngày tải. Giữ bản giấy phép đi kèm và nguyên văn yêu cầu credit; ghi cả trường hợp giấy phép nói không bắt buộc credit. Phân biệt giấy phép model, texture và phụ thuộc. Không suy ra quyền sửa đổi chỉ từ nhãn “free”. Nếu điều kiện không đáp ứng, ghi loại bỏ và không import.

## Sổ ảnh tham chiếu

| Địa điểm / vật thể | URL ảnh/trang gốc | Tác giả/đơn vị, ngày ảnh nếu biết | Quyền sử dụng / cách dùng ảnh | Model/asset được đối chiếu | Chi tiết đã xác minh | Phần ước lượng | Ảnh mẫu ban ngày |
|---|---|---|---|---|---|---|---|

Chưa thêm ảnh mới ở bước controller này. Giữ nguyên hồ sơ dữ liệu địa lý hiện có trong `D:\DaLatMap\docs\SOURCES.json` và `SOURCES_AND_LICENSES.json`; các giấy phép dữ liệu đó không tự áp dụng cho asset tải thêm. Không ghi nguồn/tác giả/ngày ảnh chưa xác minh.




## PHOTO_REFERENCES_20261013 — ảnh địa điểm từ D:\Photo

Đã mở 41 JPG, xem bốn contact sheet và mở riêng các ảnh chính. Chỉ mục P01–P41, đường dẫn gốc, SHA-256, kích thước và credit hiển thị: `data/reference_photos_index_20261013/index.json`. Ảnh gốc giữ nguyên trong D:\Photo. Dùng ảnh làm tham chiếu để dựng geometry riêng; không nhập ảnh làm texture hoặc pack vào blend. Chưa có đầy đủ giấy phép/tác giả gốc để coi ảnh là asset phát hành. Không tải asset mạng trong bước này.

| Ref | Đường dẫn gốc | Credit / URL nguồn | Chi tiết được đối chiếu | Phần chưa xác nhận |
|---|---|---|---|---|
| P03 | `D:\Photo\1791866154410_208213154647874597_6355541241509484277_f8458d6f49a32ffe858c8f3cbf5d06db.jpg` | not provided; URL gốc chưa được cung cấp | Mặt chợ; khối chính cao hơn cánh thấp; kính, ban công và tên chợ | Kích thước/nhịp chính xác, thời điểm chụp; định vị từng quầy còn ước lượng |
| P34 | `D:\Photo\1791868105748_208213154647874597_6355541241509484277_c195ee8379b73cbd8528d1bbaed26831.jpg` | MIN STUDIO ĐÀ LẠT watermark; original photographer/date unverified; URL gốc chưa được cung cấp | Mặt chợ nhìn từ Nguyễn Thị Minh Khai; nhịp kính và lan can | Kích thước/nhịp chính xác, thời điểm chụp; định vị từng quầy còn ước lượng |
| P29 | `D:\Photo\1791868026062_208213154647874597_6355541241509484277_88deb6034b18e171e83a5eb86432ee8f.jpg` | MIN STUDIO ĐÀ LẠT watermark; original photographer/date unverified; URL gốc chưa được cung cấp | Mái che dốc có gân, khung thép và cửa cuốn xanh | Kích thước/nhịp chính xác, thời điểm chụp; định vị từng quầy còn ước lượng |
| P30 | `D:\Photo\1791868046207_208213154647874597_6355541241509484277_20feed4fc4f39a32db17627ffd0e3bc0.jpg` | MIN STUDIO ĐÀ LẠT watermark; original photographer/date unverified; URL gốc chưa được cung cấp | Khung đỡ và nhịp mái che | Kích thước/nhịp chính xác, thời điểm chụp; định vị từng quầy còn ước lượng |
| P31 | `D:\Photo\1791868059988_208213154647874597_6355541241509484277_37d1f22872bb65b1aebcee5f61028b2a.jpg` | MIN STUDIO ĐÀ LẠT watermark; original photographer/date unverified; URL gốc chưa được cung cấp | Giằng kim loại, lối dưới mái và ô quầy | Kích thước/nhịp chính xác, thời điểm chụp; định vị từng quầy còn ước lượng |
| P33 | `D:\Photo\1791868089282_208213154647874597_6355541241509484277_d33fd9649b0ee59060c9f4841920061c.jpg` | MIN STUDIO ĐÀ LẠT watermark; original photographer/date unverified; URL gốc chưa được cung cấp | Quan hệ hàng mái che với hướng vào chợ | Kích thước/nhịp chính xác, thời điểm chụp; định vị từng quầy còn ước lượng |

P35–P41 là screenshot Google Street View ghi **Oct 2020**; chỉ dùng bổ sung ngữ cảnh, không coi ngày screenshot năm 2026 là ngày chụp ảnh. Các prefix URL vị trí đọc được từ thanh địa chỉ nằm trong index.json; chưa khôi phục panorama URL đầy đủ. P05 cho thấy mặt tiền L’angfarm khác hình thức Street View cũ: chưa ghép mặt tiền khác thời kỳ vào mẫu. Không đưa banner quảng cáo, người/xe hoặc trang trí nghi lễ vào mẫu.

Mẫu tiếp tục model chợ hiện có và bổ sung khoảng 15 m mái che. Chiều cao tổng chợ 8 m giữ từ model cũ vẫn là ước lượng; cánh thấp 3,1 m, kích thước tầng/nhịp kính và vị trí mái che là ước lượng. Nền, đường, hồ, cầu, dữ liệu OSM/DEM không thay. Font Arial hệ thống chỉ dùng tạo chữ biển đã chuyển sang mesh, không pack/copy file font. Không có ảnh gốc làm texture game.
