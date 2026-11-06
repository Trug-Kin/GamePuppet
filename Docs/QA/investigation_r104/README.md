# Bàn giao investigation r104

Mở `Assets/Scenes/JourneyPresentation_v48_r104.unity` trong **Unity 6000.6.2f1**, nhấn Play. Root `Investigation_V36_R104`, player `Khang_FirstPerson_R104`. WASD/chuột, E tương tác, R trả/đóng, P hồ sơ, Esc đóng UI rồi pause, F5/F6 lưu/tải hồ sơ riêng. Bắt đầu trước xưởng, khám xưởng → quầy Hà ngoài cửa → về bàn dựng. Không cần nạp profile kiểm đã giải.

Giữ GDD `Docs/CON_ROI_GDD_v3.6_CHOT_TRIEN_KHAI.md`, map r101 và checkpoint r101–r103. Không sửa map, controller/Input Actions, migration Bảo hoặc save cũ. Không build/ZIP/runner sản phẩm mới. Các helper `Test/Tam/investigation104-*` là thao tác nguồn/kiểm Editor hữu hạn, không runner gameplay.

## Thay đổi thực tế

| Phần | Đã tích hợp / giới hạn |
|---|---|
| Khải/Hà | Model rigged Quaternius Casual Male/Female tải thật từ nguồn chính thức; Blender chỉnh tỷ lệ 1,76/1,64m, idle tay hạ, vật liệu URP, Hà có tạp dề; Khải cổ tay phải rách. Không T/A pose trong lượt kiểm. Phong cách stylized; chưa art final hoặc xác nhận người Việt qua playtest |
| Rối | Quaternius BaseCharacter chỉnh gỗ/đường khớp, 15 pose dùng chung ID và pose sửa. Thay R03/R12 tại xưởng, rối mật mã và R12 trong clip; giữ nhãn khác nhau. Hình nộm trên gác bàn dựng và Tư sự kiện còn trung tính prototype; operator bàn dựng dùng Khải mới, được ghi rõ trên UI |
| Vải/áo | Cổ tay nhân vật và mẫu phẳng dùng nửa bù của đường mép mảnh vải; `CuffComparison.fbx` thay mesh trên root `Evidence_coat`, collider bounds cập nhật. Hình có thể quan sát trong `cuff-inspect-final.png`; ảnh là fixture API mở vật, không bằng chứng thu mẫu qua UI. Vải vẫn giản lược; không suy thời điểm/ý định từ loại vải |
| Clip | Khung tập trung lớn, giữ tỷ lệ 1600×1000, camera liên tục không lộ ĐK/người kéo. Thấy hai tay, R12, cánh/chốt đóng/cài trước t4; nhả/chuyển động từ t4, mất tựa từ t4,8. Giá đỡ sau người và dây treo R12; dây sửa thả sau t4,8 thay vì kéo xuyên người. **Animation minh họa im tiếng, diễn xuất/tiếp xúc dụng cụ còn provisional**, chưa footage hoặc mô phỏng vật lý độc lập |
| Bàn dựng | Góc tổng thể, cận cánh/chốt, cận tay/dây; pause, seek, đổi góc giữ thời điểm và trạng thái. Đã sửa góc tay bị tường che và layer0 của mesh operator bị camera layer29 bỏ qua; dịch hình nộm ra khỏi trục bản lề để giảm xuyên tại đầu ngã. Không tuyên bố mọi tiếp xúc ở mọi frame đã sạch |
| Luật | Sáu nút chung `MechanismPresentation.At` → `ReturnGateMechanism.Advance`. Luật namespace-normalized giống r103. Hồi/cài đo 7,4333s; sau6s chưa cài, lực mới mở lại. Không tăng tốc hồi để loại giả thuyết mở trước |
| Tua | MouseDown/Drag/Up riêng, dừng khi giữ/thả; playback exposure mới mở quan sát, seek không tự mở. Load hủy kéo và trả clip về0/dừng, bàn dựng reset an toàn; không giữ nguyên thời điểm tua qua save |

## Kiểm và bằng chứng

- `route-motion.txt`: **21/21 mốc, 60,223s**, controller gốc từ spawn sạch xưởng → Hà → xưởng, InputSystem keyboard/mouse **tổng hợp**, không teleport/dolly. Art mới được E/raycast qua root tương tác giữ nguyên. Lượt này trước chỉnh mẫu cuff/camera bàn dựng cuối; controller/map/collider tuyến không thay sau đó.
- `table-prompt-raycast.png`: chụp trước E ở cuối tuyến tự đi, đọc được “E • Dựng lại bàn dựng lại”; không còn tên kỹ thuật. Chưa polish câu lặp.
- `slider-ui.txt`, `slider-ui-1080.txt`: kéo **trực tiếp bằng chuột UI tổng hợp**, giữ, tới nhiều vị trí, kéo ngược, thả, phát tiếp; không chỉ nút ±1s/API. Camera người chơi không quay. Đổi góc bàn dựng giữ thời gian/góc cánh.
- `presentation-ui.txt`: phát/replay/dừng và sáu nút UI, cận/tổng thể; video cuối `clip-and-six-cases-final.mp4`. API chỉ mở trang đầu; không gán kết quả cánh. Clip bên trong là animation minh họa.
- `ui-functional.txt`, `ui-finish.txt`, `conclusion-ui.txt`: trường UI nhập sai Z/xóa/sửa15 ký tự, ghi đầu mối, đối chiếu, nhập liên hệ/nhận định, lưu **Chưa đủ chứng minh**, tải UI ba lần. Test biết đáp án; API nạp profile route/mở trang, không gán lời giải.
- `lifecycle.txt`: **API state checks** cộng pointer drag thật qua SendEvent: pause/load đang tua, pause/resume mô hình, load reset, năm load lặp không trôi vị trí/nhân đôi8 actor và18 interactable hoặc ID vật chứng. Không chứng minh mọi save lịch sử.
- `final-scene-audit.txt`: mở lại scene đã lưu, compile helper trong6000.6.2f1; Missing Script0, broken serialized reference0, seven native rigs/animation/material URP đủ reference. Không thay bằng chứng nhìn/chơi.
- `protected-final.json`: đúng **1.454/1.454 file liệt kê không đổi**, gồm **17/17 save gốc theo dõi**; không suy toàn bộ máy. `delivery-manifest.json`, `evidence-manifest.json`, `results-manifest.json` ghi SHA256 đúng tập file.

Kiểm UI720p/1080p có ảnh layout và ảnh slider. Video quay cửa sổ Game bao gồm toolbar, không nhầm canvas1920×1140 với nội dung1920×1080. Không có âm thanh. Lần tọa độ FAIL và video đen giữ riêng `FAIL-*`; không tính PASS.

## Nguồn và cách làm lại

Nguồn tại `Source/Blender/JourneyV48/R104/`; tải gốc `Source/Downloads/Quaternius_R104/`. Khải/Hà/Puppet editable → `Assets/Art/InvestigationV36/R104/{Khai,Ha,Puppet}.fbx`; pose → `PuppetPoseLibrary104.fbx`; clip source `Repair_R12_R104_Animation.blend`, support → `R12Support.fbx`; cuff source → `CuffComparison.fbx`.

`InvestigationActor.nativeRoot/nativeIdle/poseClips/repairClip` trên7 actor giữ root tương tác cũ (gồm operator bàn dựng); `RepairHandBinding` điều khiển tay/dây sự kiện; `PresentationHands` cho hình nộm; `InvestigationSession` và partial `InvestigationReviewUI` điều khiển time/view/seek. Actor cũ đã gỡ khỏi6 root tuyến/clip mới; operator tái bind đúng root FBX hiện có, assets checkpoint giữ nguyên. 62 GUID cách ly theo `isolation.json`; script namespace `ConRoi.Investigation.V36.R104`.

Tái xuất **nguồn cuối đã lưu**, reimport FBX đúng tên rồi kiểm bounds, animation/URP/reference. Không chạy lại derive/bind/author/isolate/integrate một lần: có thể tạo trùng hoặc reset art cuối. Recipe `investigation104-cuff-source.py` sửa cuff; `support-finish.py` là lịch sử author, không cần chạy để reimport. Các báo cáo cuối: cuff-source (supersedes Khải cũ), art-final-export cho Hà, puppet-finish, support-final, clip-context. `art-source-export.json` chứa ứng viên trung gian bị loại, không kết quả art cuối.

Save schema1 namespace `conroi.investigation.v36.r104`, `InvestigationV36/r104/prototype.json` dưới persistentDataPath. Không đọc/migration save cũ. Test dùng `route-profile`, `ui-profile`, `lifecycle-profile` dưới bộ kết quả này qua SaveDirectoryOverride; không dùng profile đó như trải nghiệm sạch.

## Còn mở

**Playtest người thật: NOT_RUN**. Có hướng dẫn/phiếu riêng, không kết luận người mới hiểu từ agent biết GDD. Art/diễn xuất clip, stylized cloth và mọi frame tiếp xúc chưa được kiểm exhaustively vẫn provisional. Không suy danh tính người kéo hoặc ý định từ cơ cấu/giày/vải. Không mở map M2/terrain legacy/mép nước, thư khác hoặc ending.

Chốt operator: `operator-layer-fix.txt` và `operator-ui.txt` bổ sung báo cáo art-integration ban đầu: **7 actor native** (thêm operator), **1 hình nộm trên gác trung tính**, **Tư sự kiện riêng còn prototype**. Gốc raycast tuyến và toàn bộ map không đổi. Khoảng đứng operator minh họa `(1.1,0,-4.25)` so với tay nắm `(1.54,1.2,-4.5)` trong stage, tránh đường dây đi qua đầu. Không suy clip định danh người kéo từ model operator của giả thuyết. Đoạn này được khôi phục tiếng Việt trong đợt r105; không đổi giới hạn bằng chứng r104.
