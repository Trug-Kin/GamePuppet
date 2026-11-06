# Checklist r105

| Nội dung | Trạng thái / bằng chứng |
|---|---|
| Nguồn lưu/mở lại, FBX roundtrip | PASS `source-final.json`; sai số đích tay dưới 0,000001 m trong mẫu nguồn. Không phải kiểm va chạm toàn thân mọi frame |
| Clip cánh đóng/chốt cài | Quan sát ảnh `after-unity-0.000.png`, video trước sự cố |
| Chốt nhả/cánh chuyển động | Quan sát 4,05/4,3 giây; không bị tay che chốt |
| Mất tựa nối từ sửa | Quan sát 4,8/4,833/5,2 giây; root motion/timing giữ nguyên |
| R-12 treo và dây sửa | Đã hiện dây treo layer 30; thả dây chuyển tiếp 0,35 giây. Dụng cụ và thao tác còn nhỏ/giản lược |
| Sáu đầu vào cơ cấu | Sáu nút UI trong `presentation-ui.txt`; mã luật và PresentationHands giống r104 sau chuẩn hóa namespace (`shared-law-unchanged.txt`) |
| Giữ/thả dây, lực người, móc, chèn | Review góc tổng/cận trong video và case-*.png; không chứng minh mọi tiếp xúc vật lý |
| Hồi hữu hạn | Giữ nguyên luật. Trong cửa sổ thử 10 giây nhiều trường hợp vẫn chưa cài; không khẳng định video đã thấy toàn chu kỳ tái cài ở từng ca. Trường hợp “chỉ tới sau” cho thấy cánh đóng/chốt cài |
| Slider clip và mô hình | PASS chuột tổng hợp down/hold/drag forward/back/up, phát tiếp; `slider-ui-720.txt`, `slider-ui-1080.txt`; tua không mở quan sát chưa xem |
| Đổi góc khi dừng | PASS giữ time/gate state; ảnh model-slider-ui-*.png |
| UI 720p/1080p | Review ảnh screenshot đầy đủ; không chồng nút hoặc cắt nhãn vai giả thuyết. Capture video cửa sổ có mép trắng và cắt dòng phụ chân trang; ảnh ScreenCapture không bị cắt |
| Prompt raycast | PASS ảnh `table-prompt-raycast.png`, `route-motion.txt`; đi controller 1/1 đoạn spawn → bàn, 6,74 giây, không teleport |
| Pause/load clip và mô hình | PASS `lifecycle.txt`, API trạng thái + chuột bắt đầu kéo; load về dừng/reset an toàn, năm load lặp không trôi/sinh trùng |
| Mở lại scene/reference | PASS `final-scene-audit.txt`; Unity 6000.6.2f1, Missing Script 0, broken serialized reference 0; 7 native actor + 1 neutral |
| Tuyến xưởng → Hà → xưởng | Không chạy lại r105; art/collider/raycast trên tuyến không đổi. Lượt r104 là bằng chứng lịch sử, không gán thành lượt r105 |
| Playtest người mới | **NOT_RUN**; có phiếu riêng, không chấm hiểu bằng agent |
| Bảo toàn | `protected-final.json`: 1.656/1.656 file liệt kê giữ hash, gồm 17/17 save gốc theo dõi; không đại diện toàn dữ liệu |

Tư/hình nộm, thao tác dụng cụ nhỏ, tiếp xúc mọi frame và độ hiểu của người mới còn **provisional**. Không suy danh tính/thời điểm/ý định từ vải hoặc giày. Không có build/ZIP/runner sản phẩm mới.
