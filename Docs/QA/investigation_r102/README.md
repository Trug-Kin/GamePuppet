# Bàn giao prototype điều tra r102 — 05/11/2026

Scene: [JourneyPresentation_v48_r102.unity](../../../Assets/Scenes/JourneyPresentation_v48_r102.unity), Unity6000.6.2f1. Map r101 đóng băng; checkpoint và GDD v3.6 giữ nguyên. Không có build, ZIP hoặc runner sản phẩm mới.

## Bắt đầu và tuyến chơi

Mở scene, Play. Khang bắt đầu `(774,6.04,286)`, xưởng ở `(774,5.04003,290)`, Hà ở `(785.8,5.04003,272.76)`. WASD/chuột, Space, E raycast; R trả vật/đóng; P hồ sơ; Esc pause; F5/F6 save/load riêng. Đi lên gác khám dấu trượt, dấu giày, điện thoại/R12/cơ cấu; xuống khám móc/vải, gặp Khải rồi tới Hà và quay lại bàn dựng. Khám Henri/R03 để tra tư thế. Sau khi gặp Khải, P → liên hệ Phương cho phép xem mẫu áo/giày, thủ tục được rút gọn và ghi rõ trong prototype.

Tự chọn ghi quan sát; xem vật không tự ghi mọi kết luận. Hai mục có thể đối chiếu bằng ghi chú tự viết, không có đèn đúng/sai. Thư Hà dùng15 ký tự tư thế nhất quán với bảng và rối; bảng nhập tùy ý, sửa/xóa được, có chú giải tiếng Việt và gợi ý tự mở. Đọc được cụm hỏi ba thùng cho đầu mối mới, không định danh người gửi. Bàn dựng có sáu trường hợp thử lại; hồ sơ cho phép ghi “Chưa đủ chứng minh” hoặc nhận định cùng căn cứ. Chưa triển khai ending.

## Nguồn và ánh xạ

- [Workshop_Investigation_R102_Editable.blend](../../../Source/Blender/JourneyV48/R102/Workshop_Investigation_R102_Editable.blend): WORKSHOP/MECHANISM/EVIDENCE/CHARACTERS/PUPPETS/REVIEW.
- [Puppet_Poses_R102_Editable.blend](../../../Source/Blender/JourneyV48/R102/Puppet_Poses_R102_Editable.blend): Pose_<letter>, RepairBothHands.
- FBX/material/pose ảnh: `Assets/Art/InvestigationV36/R102/`; logic `Assets/Scripts/InvestigationV36/`, namespace `ConRoi.Investigation.V36`.
- Scene root `Investigation_V36_R102`; `Workshop_GDD36`, `Reconstruction_stage`, `Repair_clip_illustration`. Player `Khang_FirstPerson_R102`. Session giữ ray/UI/save; ReturnGateMechanism giữ L/C/D; InvestigationActor giữ pose; RepairHandBinding nối tay phải tới dây R12.
- Cách xuất/reimport và checkpoint nguồn: đầu `Docs/BLENDER_UNITY_WORKFLOW.md`. Nguồn hiện tại có sửa khoảng đầu và dụng cụ; không chạy recipe author ban đầu đè lên file mới.
- [Bảng nguồn/giấy phép](ASSET_SOURCES.md). Nhân vật/rối mới tự dựng đã tích hợp; Khang không có mesh thân/tay. Art, diễn xuất và đoạn quay là **prototype**, chưa art final. Kenney chỉ đánh giá, không tích hợp.

## Kết quả kiểm và bằng chứng

| Hạng mục | Kết quả / bằng chứng | Giới hạn |
|---|---|---|
| Controller xưởng → Hà → xưởng | [Video liên tục](../../BangChung/investigation_r102/controller-route-final.mp4), `route-motion.txt`: spawn leg0,21/21 mốc, motion59.7163s; qua thang và vật chứng rồi ra Hà, quay lại bàn | InputSystem keyboard/mouse tổng hợp, im tiếng; không teleport giữa tuyến. Mốc bàn action-only nên acquired=False đúng thiết kế. Chưa kiểm người mới/hardware |
| Raycast E | Vật chứng mục tiêu trên tuyến được mở bằng E/ray; nearest obstruction, bỏ riêng collider player | Không suy mọi góc/vật trong map |
| Mật mã UI | `ui-input-final.txt`, `cipher-ui-solved.txt`, [ảnh](../../BangChung/investigation_r102/cipher-ui-solved.png): gán sai, xóa/gán lại, đủ15 ký tự, ghi lead=True/finalized=False | Synthetic GameView mouse/text, đáp án đã biết; chưa chứng minh độ dễ giải. Nhãn Xóa xuống dòng, vẫn dùng được |
| Kết luận UI | `conclusion-ui-final.txt`, [ảnh lưu](../../BangChung/investigation_r102/conclusion-ui-saved.png): chọn chưa đủ, nhập nhận định, cuộn, xác nhận và bấm lưu; ba load giữ choice0/claim | Mouse/text tổng hợp; API chỉ mở trang, đọc kiểm và reload. Đã sửa thiếu vùng cuộn làm nút lưu bị khuất; không tự gán kết luận bằng API |
| Kết luận UI | `conclusion-ui-final.txt`, [ảnh lưu](../../BangChung/investigation_r102/conclusion-ui-saved.png): chọn chưa đủ, nhập nhận định, cuộn, xác nhận và bấm lưu; ba load giữ choice0/claim | Mouse/text tổng hợp; API chỉ mở trang, đọc kiểm và reload. Đã sửa thiếu vùng cuộn làm nút lưu bị khuất; không tự gán kết luận bằng API |
| Lifecycle | `lifecycle-api.txt`, `lifecycle-final-response.json`: pause/resume, load3 lần mỗi trang cipher/compare/reconstruction/conclusions; inspect restore; active trial reset; gate state restore; sai namespace bị từ chối; không trùng tương tác | API trên profile thử, không giả làm video bấm toàn UI. Lượt nối sau solved-board thiếu reset fixture đã fail; nạp lại route-profile sạch rồi chạy đạt |
| Cơ cấu | `mechanism-check.txt`: finite return7.4333s từ80.162°; mở không lực không tự xảy ra; người đè/móc/chèn xét riêng | Spring/contact model điều khiển, chưa kiểm nguyên mẫu vật lý. Sau6s góc≈.994°, chưa cài; dựa lại mở≈55.097°. Không tự loại “mở trước” bằng spring |
| Clip/sáu diễn biến cuối | [Video](../../BangChung/investigation_r102/clip-and-six-model-cases-final.mp4), `visual-playback.txt`, `visual-capture-final.json`: exit0, watched before/event True, sáu report đủ | API chọn diễn biến, click tổng hợp phát clip; im tiếng. Hình nộm có diễn xuất/force định trước, không controller evidence. Clip9s không thấy người kéo/ĐK, chưa footage final |
| Blender/FBX | `blender-gates.json`, `fbx-roundtrip.json`, `headroom-source.json`, `repair-tool-source.json`, pose roundtrip; lưu–đóng–mở lại; không thiếu library/texture nguồn | Mesh màu phẳng có phần chưa UV; không gọi texture/lightmap-ready. Xem workflow cho cách xuất |
| Scene restart/references | `final-scene-audit.txt`: r102 sạch sau restart, Missing0,17 interaction, oldHeads0; eye/movement/look/cameras/gates/15icons có reference | Reference bắt buộc của prototype, không blanket serialized-reference scan toàn project |
| Bảo toàn | `protected-final.json`:1220/1220 file đã liệt kê bằng hash; gồm17/17 save gameplay gốc | Chỉ tập được theo dõi, không toàn bộ save trên máy |

## Cơ cấu và khoảng trống suy luận

D chùng/cánh đóng thì C cài. Kéo D chỉ nhả C, lực phía gác làm mở; hết lực thì hồi hữu hạn rồi cài, không tự xuyên qua người đè hoặc vật chèn. Có cả móc M giữ dây. “Khải giúp mở rồi Tư còn giữ” là khả năng riêng; người đến sau vẫn có thể để dấu, chưa giải thích cú ngã. Giày/vải hỗ trợ nhận diện, không tự chứng minh thao tác đúng thời điểm; dầu không xác định ý định. Clip minh họa là căn cứ trạng thái trong sự kiện **nếu người chơi đọc được**, chưa được phép gọi bằng chứng forensic final. Không có quy tắc đếm đủ vật để tự chốt vụ án. Kết luận từng phần do người chơi ghi, còn được chọn chưa đủ.

## Lỗi đã sửa và còn mở

Đã sửa: collider control sheet bị nhân scale chặn tuyến; collider áo cũ; ray tự hit player; xà thiếu khoảng đầu (sửa nguồn Blender); binding interaction mất sau restart (tách MonoBehaviour đúng file/GUID); rời mép trước khi hạ người mô hình; freeze report/reset trial; tay/dây/dụng cụ clip; trang kết luận cần vùng cuộn để tới nút lưu. Chi tiết asset/object trong hai tài liệu chung.

Còn mở: art Khải/Hà/rối và đoạn sửa rối còn sơ lược; mô hình bàn dựng chưa mô phỏng rigidbody tự do, khe/tiếp xúc tay và vật chèn vẫn là minh họa. Chưa xác nhận người mới phân biệt tư thế hoặc đọc đủ trạng thái clip; chưa hardware/audio/accessibility ở mọi kích thước cửa sổ. Chưa có toàn26 tư thế/năm thư/ending. Các hạn chế map M2/legacy Z<411/mép nước không sửa trong đợt này. Không kết luận toàn map hay toàn game PASS. Liên hệ hai mục đã kiểm lưu qua API; không coi đó là toàn bộ thao tác UI đã được kiểm bằng controller.

## Save và tái kiểm

Schema1/namespace `conroi.investigation.v36.r102`. Mặc định `<persistentDataPath>/InvestigationV36/r102/prototype.json`; override kiểm nằm trong profile riêng dưới thư mục kết quả này. Không đọc/migration/ghi save gameplay cũ. Đừng copy profile kiểm đã giải vào save mặc định rồi gọi New Game. Các snippet `Test/Tam/investigation102-*` phục vụ kiểm/author có điều kiện; nhiều script author là one-shot, không chạy lại để tránh tạo trùng geometry. Đối chiếu log nested success và kết quả file, không chỉ shell exit0. Manifest file sản phẩm ghi SHA256 riêng trong hồ sơ.
