# Checklist r104

| Mục | Kết quả | Căn cứ |
|---|---|---|
| Checkpoint/map/save | PASS trong tập liệt kê | protected-final.json:1454/1454,17 save |
| Art thật Khải/Hà/rối | INTEGRATED | art-integration, final-scene-audit, source/FBX roundtrip |
| Idle/rig/URP/E raycast | PASS phạm vi tuyến | route-motion + audit; không mọi animation |
| Cuff đồng bộ | INTEGRATED, stylized | cuff-source.json, cuff-inspect-final.png (API fixture) |
| Clip đóng/cài→nhả→mất tựa | VISIBLE, PROVISIONAL ACTING | clip-closed-ui/release-ui, source final frames |
| Clip slider giữ/kéo/ngược/thả | PASS UI720/1080 | slider-ui*.txt, slider-direct-1080.mp4 |
| Model sáu trường hợp, pause/view | PASS UI/state riêng | presentation-ui, slider-ui; chung luật |
| Camera tay từng bị tường che | SỬA + kiểm lại hình cuối | final video/close hook; không chỉ log |
| Prompt bàn dựng | PASS trực quan controller | table-prompt-raycast.png |
| Mật mã/đối chiếu/nhận định chưa đủ | PASS UI biết đáp án | ui-functional/ui-finish/conclusion-ui |
| Pause/load giữa review/model | PASS API lifecycle | lifecycle.txt; timing reset an toàn |
| Reopen/Missing Script/reference | PASS | final-scene-audit.txt |
| Mọi tiếp xúc/tất cả save lịch sử | NOT_RUN | chỉ phạm vi log, không suy rộng |
| Người mới tự hiểu suy luận | NOT_RUN | PLAYTEST_NGUOI_MOI.md |

Không lấy thời gian hồi hoặc test code làm xác nhận vật lý thực địa; không chấm hung thủ bằng dầu/vải/giày.
