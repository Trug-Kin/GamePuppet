# Video, ảnh và phạm vi r105

| File trong Test/BangChung/investigation_r105 | Nội dung thực tế |
|---|---|
| clip-and-six-cases-final.mp4 | Animation game minh họa im tiếng; chuột tổng hợp SendEvent bấm phát clip và sáu nút bàn dựng, đổi tổng/cận. API chỉ mở trang clip ban đầu và chuyển sang trang bàn dựng. Không gán kết quả, không phải controller footage hay người mới |
| FAIL-presentation-transition.mp4 | **FAIL / superseded**: helper kẹt sau case đầu khi sửa thứ tự screenshot; không dùng làm video cuối. Log `FAIL-presentation-transition.txt`. Callback đã được xóa qua stop/reload script Editor, không sửa luật game |
| before-unity-*.png / after-unity-*.png | Cùng camera và thời điểm clip. Lấy mẫu bằng API để review render, không chứng minh thao tác người chơi |
| before-blender-*.png / after-blender-*.png | Render source editable, không bằng chứng controller |
| case-*-wide.png / case-*-close.png | Góc tổng khoảng 3,4 giây và cận khoảng 5,4 giây trong lượt UI cuối. Không phải cùng thời điểm; kiểm giữ thời điểm đổi góc riêng bằng slider log |
| table-prompt-raycast.png | Controller gốc nhận input tổng hợp, đi liên tục từ spawn tới bàn; prompt thật trước E. Không fixture placement |
| clip-slider-ui-720.png / clip-slider-ui-1080.png | Chụp sau thao tác kéo/phát/dừng bằng chuột tổng hợp |
| model-slider-ui-720.png / model-slider-ui-1080.png | Tua/dừng/đổi góc UI; log phân biệt với API |
| video-review-*.png | Các frame trích từ video cuối để xác nhận capture có nội dung |

Video GameView có toolbar/mép trắng và cắt dòng hướng dẫn phụ chân trang; các nút và hình sự kiện chính vẫn đọc được. Dùng ảnh ScreenCapture 720p/1080p để rà đầy đủ layout. Không đổi cơ cấu hoặc camera clip để che sai tiếp xúc. Tư/dụng cụ/diễn xuất và mọi frame tiếp xúc vẫn provisional. Playtest người thật **NOT_RUN**.
