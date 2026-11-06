# Sáu chu kỳ — trạng thái thực tế

Mốc tính từ lúc bắt đầu từng trường hợp. Nguồn: `cycle-transitions.csv` và `cycle-api.txt`; bước đo **1/120 giây**, làm tròn 0,01 giây. Lấy mẫu có sai số một bước tại điều kiện `>`/`<`. Video UI chạy thời gian thực, không tăng tốc. Đây là mô hình minh họa xác định, không phải kết luận vật lý thực địa.

| Trường hợp | Dây và điều kiện còn giữ | Bắt đầu hồi | Đóng / tái cài | Trạng thái dừng có lý do |
|---|---|---|---|---|
| Mở trước → thả → người di chuyển | D giữ 0–2,50; lực đè 0–2,50 rồi người quay lại từ 8,50 | 2,56; lực mới mở lại trước chốt cài | Chưa đóng/cài ở đoạn đầu. Ngừng đè tại 11,74, tiếp xúc khi rời tới 12,34; hồi lại ~12,35; cài ~19,77 | 11,74: người giữ mở ở 82°. Sau nút bỏ giữ: đóng/cài, dừng 19,80 |
| Người giúp mở; Tư còn đè giữ | D giữ 0–2,50; lực đè tới 5,00; tiếp xúc tới 5,10 | 5,10 | ~12,52 đóng/cài | Dừng 12,55: dây chùng, hết lực/tiếp xúc, chốt cài |
| Giữ dây khi Tư đang dựa | Lực đè từ đầu tới 4,50; D giữ sau 2,00 tới 5,00 | ~4,56, khi D vẫn căng | ~11,86 đóng/cài | Dừng 11,89: không còn lực và D chùng |
| Dùng móc giữ dây | Móc từ sau 1,00; lực đè sau 2,00 tới 5,00 | ~5,04 | Vào vùng đóng ~12,42, **chưa cài vì móc giữ D**. Chủ động nhả móc tại 14,65 thì cài trong bước kế tiếp | Dừng đầu 14,65: góc ~0,063°, vận tốc ~−0,049°/s, chốt nhả. Sau nhả móc dừng 14,73: đóng/cài |
| Dùng vật chèn | D/lực đè tới 2,50; chèn từ sau 2,00, giữ nguyên cho tới nút rút | 2,56 rồi dừng tại khe chèn 35° | Rút chèn tại 3,91; hồi tiếp ngay; đóng/cài ~10,22 | Dừng đầu 3,91: chèn giữ. Sau rút dừng 10,25: đóng/cài |
| Chỉ tới sau cú ngã | Không kéo D, không lực đè/chèn/móc; người tới từ 5 tới 9 giây | Không có: cánh chưa mở trong ca này | Đóng/cài từ đầu | Dừng 9,08 sau khi người tới. Ca này không tự giải thích cú ngã trước đó |

Ca móc tiến gần vị trí đóng theo luật hồi có giảm chấn. Tiêu chí đứng yên để dừng trình bày dùng vùng cài sẵn có `<0,35°` và tốc độ `<0,05°/s`; **không snap góc và không cài khi D còn căng**. Góc nhỏ còn lại được giữ nguyên trong mô hình. Khi nhả móc, luật cài hiện hữu mới cho chốt cài.

Thời gian nút bỏ giữ trong bảng là lượt thử bấm ngay sau điểm dừng ban đầu. Video để người xem nhìn trạng thái rồi bấm; đồng hồ mô hình dừng trong khoảng nhìn đó, nên mốc mô hình vẫn khớp. Chỉ khi nhấn bỏ giữ mới tạo phần hồi tiếp theo.

Không đồng nhất các trạng thái cuối. Không lấy việc tự hồi để bác khoảng mở trước; ca đầu vẫn cho người quay lại giữ mở trước tái cài. Clip sự kiện và chứng cứ khác phải được đối chiếu riêng; mô hình không nhận diện người thao tác hay ý định.
