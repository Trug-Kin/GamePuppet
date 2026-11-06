from pathlib import Path
import hashlib,json
R=Path('D:/ConRoi');D=R/'Test/KetQua/investigation_r104'
readme='''# Bàn giao investigation r104

Mở `Assets/Scenes/JourneyPresentation_v48_r104.unity` trong **Unity 6000.6.2f1**, nhấn Play. Root `Investigation_V36_R104`, player `Khang_FirstPerson_R104`. WASD/chuột, E tương tác, R trả/đóng, P hồ sơ, Esc đóng UI rồi pause, F5/F6 lưu/tải hồ sơ riêng. Bắt đầu trước xưởng, khám xưởng → quầy Hà ngoài cửa → về bàn dựng. Không cần nạp profile kiểm đã giải.

Giữ GDD `Docs/CON_ROI_GDD_v3.6_CHOT_TRIEN_KHAI.md`, map r101 và checkpoint r101–r103. Không sửa map, controller/Input Actions, migration Bảo hoặc save cũ. Không build/ZIP/runner sản phẩm mới. Các helper `Test/Tam/investigation104-*` là thao tác nguồn/kiểm Editor hữu hạn, không runner gameplay.

## Thay đổi thực tế

| Phần | Đã tích hợp / giới hạn |
|---|---|
| Khải/Hà | Model rigged Quaternius Casual Male/Female tải thật từ nguồn chính thức; Blender chỉnh tỷ lệ 1,76/1,64m, idle tay hạ, vật liệu URP, Hà có tạp dề; Khải cổ tay phải rách. Không T/A pose trong lượt kiểm. Phong cách stylized; chưa art final hoặc xác nhận người Việt qua playtest |
| Rối | Quaternius BaseCharacter chỉnh gỗ/đường khớp, 15 pose dùng chung ID và pose sửa. Thay R03/R12 tại xưởng, rối mật mã và R12 trong clip; giữ nhãn khác nhau. Hình nộm bàn dựng và Tư sự kiện còn trung tính prototype, được ghi rõ trên UI |
| Vải/áo | Cổ tay nhân vật và mẫu phẳng dùng nửa bù của đường mép mảnh vải; `CuffComparison.fbx` thay mesh trên root `Evidence_coat`, collider bounds cập nhật. Hình có thể quan sát trong `cuff-inspect-final.png`; ảnh là fixture API mở vật, không bằng chứng thu mẫu qua UI. Vải vẫn giản lược; không suy thời điểm/ý định từ loại vải |
| Clip | Khung tập trung lớn, giữ tỷ lệ 1600×1000, camera liên tục không lộ ĐK/người kéo. Thấy hai tay, R12, cánh/chốt đóng/cài trước t4; nhả/chuyển động từ t4, mất tựa từ t4,8. Giá đỡ sau người và dây treo R12; dây sửa thả sau t4,8 thay vì kéo xuyên người. **Animation minh họa im tiếng, diễn xuất/tiếp xúc dụng cụ còn provisional**, chưa footage hoặc mô phỏng vật lý độc lập |
| Bàn dựng | Góc tổng thể, cận cánh/chốt, cận tay/dây; pause, seek, đổi góc giữ thời điểm và trạng thái. Đã sửa góc tay bị tường che; dịch hình nộm ra khỏi trục bản lề để giảm xuyên tại đầu ngã. Không tuyên bố mọi tiếp xúc ở mọi frame đã sạch |
| Luật | Sáu nút chung `MechanismPresentation.At` → `ReturnGateMechanism.Advance`. Luật namespace-normalized giống r103. Hồi/cài đo 7,4333s; sau6s chưa cài, lực mới mở lại. Không tăng tốc hồi để loại giả thuyết mở trước |
| Tua | MouseDown/Drag/Up riêng, dừng khi giữ/thả; playback exposure mới mở quan sát, seek không tự mở. Load hủy kéo và trả clip về0/dừng, bàn dựng reset an toàn; không giữ nguyên thời điểm tua qua save |

## Kiểm và bằng chứng

- `route-motion.txt`: **21/21 mốc, 60,223s**, controller gốc từ spawn sạch xưởng → Hà → xưởng, InputSystem keyboard/mouse **tổng hợp**, không teleport/dolly. Art mới được E/raycast qua root tương tác giữ nguyên. Lượt này trước chỉnh mẫu cuff/camera bàn dựng cuối; controller/map/collider tuyến không thay sau đó.
- `table-prompt-raycast.png`: chụp trước E ở cuối tuyến tự đi, đọc được “E • Dựng lại bàn dựng lại”; không còn tên kỹ thuật. Chưa polish câu lặp.
- `slider-ui.txt`, `slider-ui-1080.txt`: kéo **trực tiếp bằng chuột UI tổng hợp**, giữ, tới nhiều vị trí, kéo ngược, thả, phát tiếp; không chỉ nút ±1s/API. Camera người chơi không quay. Đổi góc bàn dựng giữ thời gian/góc cánh.
- `presentation-ui.txt`: phát/replay/dừng và sáu nút UI, cận/tổng thể; video cuối `clip-and-six-cases-final.mp4`. API chỉ mở trang đầu; không gán kết quả cánh. Clip bên trong là animation minh họa.
- `ui-functional.txt`, `ui-finish.txt`, `conclusion-ui.txt`: trường UI nhập sai Z/xóa/sửa15 ký tự, ghi đầu mối, đối chiếu, nhập liên hệ/nhận định, lưu **Chưa đủ chứng minh**, tải UI ba lần. Test biết đáp án; API nạp profile route/mở trang, không gán lời giải.
- `lifecycle.txt`: **API state checks** cộng pointer drag thật qua SendEvent: pause/load đang tua, pause/resume mô hình, load reset, năm load lặp không trôi vị trí/nhân đôi8 actor và18 interactable hoặc ID vật chứng. Không chứng minh mọi save lịch sử.
- `final-scene-audit.txt`: mở lại scene đã lưu, compile helper trong6000.6.2f1; Missing Script0, broken serialized reference0, six native rigs/animation/material URP đủ reference. Không thay bằng chứng nhìn/chơi.
- `protected-final.json`: đúng **1.454/1.454 file liệt kê không đổi**, gồm **17/17 save gốc theo dõi**; không suy toàn bộ máy. `delivery-manifest.json`, `evidence-manifest.json`, `results-manifest.json` ghi SHA256 đúng tập file.

Kiểm UI720p/1080p có ảnh layout và ảnh slider. Video quay cửa sổ Game bao gồm toolbar, không nhầm canvas1920×1140 với nội dung1920×1080. Không có âm thanh. Lần tọa độ FAIL và video đen giữ riêng `FAIL-*`; không tính PASS.

## Nguồn và cách làm lại

Nguồn tại `Source/Blender/JourneyV48/R104/`; tải gốc `Source/Downloads/Quaternius_R104/`. Khải/Hà/Puppet editable → `Assets/Art/InvestigationV36/R104/{Khai,Ha,Puppet}.fbx`; pose → `PuppetPoseLibrary104.fbx`; clip source `Repair_R12_R104_Animation.blend`, support → `R12Support.fbx`; cuff source → `CuffComparison.fbx`.

`InvestigationActor.nativeRoot/nativeIdle/poseClips/repairClip` trên6 actor giữ root tương tác cũ; `RepairHandBinding` điều khiển tay/dây sự kiện; `PresentationHands` cho hình nộm; `InvestigationSession` và partial `InvestigationReviewUI` điều khiển time/view/seek. Actor cũ đã gỡ khỏi6 root mới, assets checkpoint giữ nguyên. 62 GUID cách ly theo `isolation.json`; script namespace `ConRoi.Investigation.V36.R104`.

Tái xuất **nguồn cuối đã lưu**, reimport FBX đúng tên rồi kiểm bounds, animation/URP/reference. Không chạy lại derive/bind/author/isolate/integrate một lần: có thể tạo trùng hoặc reset art cuối. Recipe `investigation104-cuff-source.py` sửa cuff; `support-finish.py` là lịch sử author, không cần chạy để reimport. Các báo cáo cuối: cuff-source (supersedes Khải cũ), art-final-export cho Hà, puppet-finish, support-final, clip-context. `art-source-export.json` chứa ứng viên trung gian bị loại, không kết quả art cuối.

Save schema1 namespace `conroi.investigation.v36.r104`, `InvestigationV36/r104/prototype.json` dưới persistentDataPath. Không đọc/migration save cũ. Test dùng `route-profile`, `ui-profile`, `lifecycle-profile` dưới bộ kết quả này qua SaveDirectoryOverride; không dùng profile đó như trải nghiệm sạch.

## Còn mở

**Playtest người thật: NOT_RUN**. Có hướng dẫn/phiếu riêng, không kết luận người mới hiểu từ agent biết GDD. Art/diễn xuất clip, stylized cloth và mọi frame tiếp xúc chưa được kiểm exhaustively vẫn provisional. Không suy danh tính người kéo hoặc ý định từ cơ cấu/giày/vải. Không mở map M2/terrain legacy/mép nước, thư khác hoặc ending.
'''
(D/'README.md').write_text(readme,encoding='utf-8')
asset='''# Nguồn asset r104

Đã kiểm `Assets/Model/Characters`:191 OBJ, chưa thấy rig/animation hoặc hồ sơ license/credit trong Assets/Model. Không dùng nhóm này để tránh mặc định quyền sử dụng.

## Gói đã tải và tích hợp

**Quaternius — Ultimate Animated Characters**. Trang chính thức: https://quaternius.com/packs/ultimatedanimatedcharacter.html ; folder chính thức https://drive.google.com/drive/folders/1sNi1AfenfPRrvRt5yfaj5QMMd6KKcUJ5 ; Blend folder `1X0armnO9EePmnQwP5EJHxpCiuucwG_uQ`.

Đúng file license tải kèm `Source/Downloads/Quaternius_R104/animated-license.txt`: **CC0 1.0 Universal**, https://creativecommons.org/publicdomain/zero/1.0/ . Cho phép dùng/chỉnh trong game thương mại, không bắt buộc attribution; credit tự nguyện “Character bases by Quaternius (CC0), adapted for Con Rối”. Không khẳng định đây là license của mọi asset Quaternius. Upstream không công bố version cho các file đã chọn; nhận diện bằng SHA256/ngày tải05/11/2026.

| File gốc / Drive ID | Thực tế dùng |
|---|---|
| Casual_Male.blend / 1Mig1cMlBWgQ_H8FxlWkfNeXFkNeclyCl | Khải, tỷ lệ, blue outfit, cuff rách, idle/rig |
| Casual_Female.blend / 17O7eSD7xunnN10MQiQcFOQ_EPvRR9_M3 | Hà, tỷ lệ/tạp dề, idle/rig |
| BaseCharacter.blend / 1lSCK-VuY3SPqP-2T-n8b6W6glUbhRStz | R03/R12/cipher/clip puppet, gỗ/khớp/pose |
| animated-license.txt / 1eooYt7zHAMOPEU6d3QHMzR2p0gk9ZJF8 | License đúng gói trên |

Nguồn sửa Blender trong R104; asset URP trong Art/InvestigationV36/R104. Rối dùng base chỉnh được khớp; không tuyên bố đây là puppet final có kết cấu cơ khí thực.

## Ứng viên không tích hợp / art tạm

Casual_Hoodie.blend, Casual_2.blend từ gói Modular Males và license.txt tải riêng: đánh giá nhưng không dùng trong scene; không nhập cả package. Universal Base Characters itch trảHTTP429, chuyển ứng viên khác từ nguồn chính thức. Kenney chỉ đánh giá cũ, không tích hợp.

Tư sự kiện, operator/hình nộm bàn dựng: art tự dựng prototype kế thừa r103, chưa thay Quaternius; Khang góc nhìn thứ nhất dùng controller gốc không có full-body art mới. Map/nhà/cơ cấu/vật chứng tự dựng, không thay map bằng gói mạng. Clip vẫn illustrative silent.

## Hash tải gốc

'''
for p in sorted((R/'Source/Downloads/Quaternius_R104').glob('*')):
 if p.suffix in ['.blend','.txt']:asset+=f"- `{p.name}` · SHA256 `{hashlib.sha256(p.read_bytes()).hexdigest()}`\n"
(D/'ASSET_SOURCES.md').write_text(asset,encoding='utf-8')
(D/'CHECKLIST.md').write_text('''# Checklist r104

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
''',encoding='utf-8')
(D/'PLAYTEST_NGUOI_MOI.md').write_text('''# Bắt đầu và phiếu playtest — NOT_RUN

Chưa có người thật chơi r104. Người kiểm kỹ thuật biết GDD không thay người mới.

Mở scene r104 trong Unity6000.6.2f1, Play từ hồ sơ r104 sạch; giữ save người dùng và dùng thư mục thử riêng. Đi vào xưởng, quan sát vật và điện thoại, gặp người ở xưởng, tìm quầy Hà ngoài cửa rồi quay về bàn dựng. WASD/chuột đi/nhìn; E tương tác, giữ chuột/scroll khi xem vật, R trả/đóng, P hồ sơ, Esc đóng/pause. Có thể dừng/tua clip và đổi góc bàn dựng. Ghi nhận định hoặc chọn chưa đủ chứng minh. Không bắt buộc đoán người có lỗi.

Người hướng dẫn không giải thích GDD, không gán đáp án hoặc chỉ đúng đoạn phim. Ghi thời điểm người chơi cần trợ giúp và câu hỏi nguyên văn.

1. Bạn trực tiếp nhìn thấy những gì? Chỉ vị trí trong vật/clip.
2. Bạn suy ra điều gì từ đó? Có cách giải thích khác không?
3. Bạn nghi ai/vì sao, hoặc chưa nghi ai? Còn thiếu dữ liệu gì?
4. Có đoạn nào không đọc được chốt/tay/dây hoặc không hiểu nút?
5. Bạn có phân biệt điều thấy được với điều mô hình chỉ minh họa không?

Ghi profile, độ phân giải, thời lượng, lỗi thao tác, câu nói trực tiếp, số lần trợ giúp. Chưa điền kết quả hiểu/không hiểu khi chưa chạy người thật.
''',encoding='utf-8')
print('Delivery documents written')
