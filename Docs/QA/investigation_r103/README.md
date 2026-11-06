# Bàn giao prototype điều tra r103

Mở `Assets/Scenes/JourneyPresentation_v48_r103.unity` bằng **Unity 6000.6.2f1**, nhấn Play. Root `Investigation_V36_R103`, player `Khang_FirstPerson_R103`. WASD/chuột, E khám, R trả/đóng, P hồ sơ, Esc pause, F5/F6 lưu/tải riêng. Đi vào xưởng, khám điện thoại/dấu sàn/rối, gặp Khải; tới quầy Hà ngoài cửa rồi về bàn dựng. Xin mẫu áo/giày qua mục liên hệ Phương trong hồ sơ sau khi gặp Khải. Không cần nạp profile kiểm đã giải để bắt đầu.

Map r101 đóng băng. R101/r102/checkpoint và 17 save gốc được theo dõi giữ nguyên; mã mới namespace `ConRoi.Investigation.V36.R103`, art trong `Assets/Art/InvestigationV36/R103`. 49 GUID đã cách ly theo `isolation.json`. Save schema1/namespace `conroi.investigation.v36.r103`, thư mục `InvestigationV36/r103`; không đọc/migration save cũ. Không build, ZIP hoặc runner sản phẩm mới.

## Phần đã làm

| Mục | Kết quả / giới hạn |
|---|---|
| Vệt trượt và dấu giày | Hai dải bụi dài và các khối vân đen giao/chồng thật; cùng prefab `TraceCrossing` khi xem gần. Xoay, zoom, mặt trên/dưới hoạt động; dấu là hình stylized, chưa ảnh pháp y |
| Giày đối chiếu | Khuyết gót bằng đường bao lõm, khối vân giữa mòn nhỏ; mẫu B cùng màu/kiểu nhưng khuyết ngắn hơn, vân giữa nguyên. Đã sửa bước xuất lặp làm vân bị đẩy vào trong đế; `sole-final.json` là bản xuất cuối |
| Vải và áo | Mép bất đối xứng, sợi chỉ bám mép, mũi may và đường dệt nhìn được. Hình vải còn giản lược; chưa chứng minh người mới đọc đúng hoặc dùng nó như nhận diện tuyệt đối |
| R-03/R-12 | Nhãn vật thể khác mã, kích thước khác; R-12 có thẻ buổi sửa. Nhãn đã sửa chiều chữ khi nhập Unity. Khác biệt đạo cụ dàn dựng và rối sửa vẫn cần người chơi đối chiếu |
| Cánh/chốt/dây/móc | Chốt đưa về đúng tọa độ nguồn `(1.51,.92,0)`, vật liệu tương phản; móc/trụ tay nắm có hình nguồn mới. Preview giữ cả cơ cấu, chuyển line world-space sang local trước chuẩn hóa; có cận chốt/toàn dây |
| Quan sát và suy luận | Nút ghi giày/vải/áo mô tả đặc điểm nhìn thấy; không tự ghi kết luận thời điểm/ý định. Liên hệ và nhận định do người chơi nhập, không đèn đúng/sai |
| Clip sửa R-12 | Animation editable 12s/361 frame, một góc liên tục. Cánh/chốt đóng trước t4, nhả từ t4, Tư mất tựa từ t4.8. Thấy hai tay và đồ sửa, không quay ĐK/người kéo. Có phát/dừng/phát lại/slider và nút ±1s. **Bản dựng thử im tiếng, chưa final**; diễn xuất thao tác, dây đỡ R-12 và art còn cần hoàn thiện, không là footage thực |
| Sáu diễn biến | Dùng chung `ReturnGateMechanism.Advance` qua `MechanismPresentation.At`; chỉ timeline đầu vào khác. Người giữ/thả dây, lực gác, móc và chèn có trình bày riêng. IK tay tới điểm tiếp xúc; đường ngã bắt đầu liên tục từ vị trí đang đứng. Góc toàn xưởng thấy người giữ dây, thêm cận cánh |
| UI | Namespace/thông số kỹ thuật chuyển vào Inspector `diagnostics`; mục tiêu ngắn, nút × rõ. Gõ trong trường không kích hoạt R/P. Kết luận dùng nút xác nhận lớn, danh sách căn cứ co theo nội dung; lưu lựa chọn chưa đủ bằng UI |
| Nhân vật | Khải/Hà/rối tự dựng của prototype còn dùng tạm. Quaternius CC0 đã đánh giá trang/ảnh chính thức, **chưa tải/tích hợp model hoặc kiểm rig**; không gọi bộ này là art mới đã thay. Xem `ASSET_SOURCES.md` |

## Kiểm và bằng chứng

- `route-motion.txt`: controller gốc, input InputSystem tổng hợp, từ spawn sạch qua xưởng → Hà → xưởng, 21/21 mốc; không đặt lại vị trí giữa tuyến. Video controller không thay bằng dolly. Dữ liệu kết thúc chưa tự gán mapping, quan sát, liên hệ hoặc kết luận.
- `ui-functional.txt`, `ui-finish.txt`: chuột/text GameView tổng hợp, nhập sai Z → × → gán lại 15 ký tự, ghi cách đọc, chọn hai mục và ghi liên hệ. Đây là kiểm biết đáp án; API chỉ nạp profile/mở trang ban đầu, không gán đáp án để chứng minh UI.
- `conclusion-ui.txt`: xác nhận/lưu “Chưa đủ chứng minh” bằng UI và ba lần tải lại từ nút; giữ nhận định, trang và vị trí. Các thử tọa độ/checkbox trước đó thất bại không được tính là đạt.
- `lifecycle-api.txt`: kiểm API riêng về pause/resume, save/load các trang, vật đang xem, reset lần dựng đang chạy, giữ cơ cấu, từ chối namespace cũ và không sinh trùng. Không thay bằng chứng thao tác UI/controller.
- `mechanism-check.txt`: hồi/cài khoảng **7.4333s** từ 80.162°; sau6s còn0.994°, chưa cài, lực mới mở lại55.097°. Không tăng tốc hồi để loại giả thuyết mở trước. Đây là kiểm mô hình xác định, **không chứng minh thiết kế vật lý thực tế**.
- `presentation-ui.txt`: lựa chọn sáu trường hợp bằng nút, log ID thực tế và trạng thái. Kiểm “Khải giúp mở, Tư đè giữ” phải so với cánh đóng/chốt cài ngay trước sự cố trong clip; vải/giày không xác định giờ kéo hay ý định. Nếu còn diễn biến khớp toàn bộ chứng cứ, hồ sơ cho giữ chưa đủ.
- `ui-layout.txt` và `ui-720-*`/`ui-1080-*`: fixture bố cục mở trang bằng API, không là bằng chứng người chơi đã thu mẫu/giải mã. 1280×720 và1920×1080. Ảnh sau sửa kết luận/clip có tên riêng; xem bảng trạng thái cuối, không suy toàn UI ở mọi tỷ lệ cửa sổ.
- `final-scene-audit.txt`: scene đã lưu–đóng–mở lại, Missing Script và reference bắt buộc kiểm riêng. Không gọi đây là mọi reference trong toàn project.
- `protected-final.json`: đối chiếu đúng **1.325 file đã liệt kê**, gồm **17 save gốc**; không suy thành tất cả dữ liệu/save trên máy. `delivery-manifest.json`/`evidence-manifest.json` ánh xạ hash cuối.

## Nguồn và tái tạo

Ba nguồn mở được: `Source/Blender/JourneyV48/R103/Workshop_Investigation_R103_Editable.blend`, `Puppet_Poses_R103_Editable.blend`, `Repair_R12_R103_Animation.blend`. Collections chính WORKSHOP/MECHANISM/EVIDENCE/CHARACTERS/PUPPETS/REVIEW; animation sửa ở source riêng. Các nguồn đã lưu và mở lại; unit1, chuyển Unity=(-Bx,Bz,-By). Roundtrip cuối EVIDENCE/MECHANISM/RepairEventActor trong JSON tương ứng.

Xuất **từ nguồn hiện hành** bằng preset Unity của `bx_export`, đúng collection; skeletal actor có bake action 12s, Legacy clip `Scene`. Không chạy lại script author đầu hoặc export có cộng offset vào đỉnh: chúng là lịch sử, có thể tái đưa lỗi vân vào đế. Script `investigation103-sole-finish.py` đặt tọa độ tuyệt đối; không cần chạy lại nếu xuất nguồn đã đúng. Không chạy lại isolate/integrate lên scene hiện hành vì có thể tạo trùng/reset binding. Xem mapping trong hai tài liệu chung và `isolation.json`.

## Còn mở / chưa kiểm

**Art nhân vật và clip còn provisional.** Chưa đủ26 tư thế, năm thư, ending hoặc khu mới; không hứa các chức năng đó trên UI. Chưa xác minh đầy đủ tính cơ học ngoài đời, mọi tiếp xúc hình học từng frame, mọi tổ hợp input/tỷ lệ màn hình, mọi save lịch sử. Slider kéo chuột phải phân biệt với nút tua đã kiểm; không gộp chúng thành một kết quả.

**Người mới: NOT_RUN.** Hướng dẫn tại `PLAYTEST_NGUOI_MOI.md`; chưa kết luận prototype được người chưa đọc GDD hiểu. M2/terrain legacy/mép nước ngoài phạm vi, không mở lại san nền.

## Chốt lượt cuối

Đã mở lại scene sạch: **347 component** trong root prototype không có reference hiển thị bị dangling với ID khác0;18 interaction, Missing Script0, pose refs8. Initial records0, finalized=false, namespace R103, diagnostics=false, clip `Scene`12s. Đây là phạm vi kiểm cụ thể, không mọi serialized field toàn project.

`input-edges.txt` đã qua raw R/P, Esc đóng/pause/resume, chặn E cùng frame đóng UI, xoay/zoom bằng chuột tổng hợp. Fixture tạm cấu hình runtime InputSettings để nhận thiết bị ngoài focus GameView rồi khôi phục; lượt phụ thuộc focus bị lỗi giữ ở `input-edges-focus-failed.txt`. Không sửa input asset/controller để làm test đạt. Native phím thử trước đó không kích hoạt vào GameView, không tính là lượt pause đạt.

Clip tua +1s đã kiểm qua nút; slider click-track trước đó không có lượt đạt, không gọi slider đã kiểm. Bản ghi kết luận sau đổi checkbox nhỏ thành nút lớn đã lưu và ba lần Tải qua UI. Bố cục kết luận/mật mã/đối chiếu/inspection/bàn dựng được chụp lại ở cả720p/1080p.

Video ưu tiên: `controller-route-final.mp4` dừng theo END21/21 +1.5s (xem `controller-capture-final.json`), và `clip-six-cases-ui.mp4`87s. Các lượt capture cắt sớm/đen, click sai tọa độ và hook chắn ray không được dùng kết luận đạt. Xem `VIDEO_INDEX.md`, `CHECKLIST.md`. Art/clip còn provisional; người mới NOT_RUN.

Nhãn prompt bàn dựng được sửa sau capture: từ tên object kỹ thuật sang “bàn dựng lại” (dây/cánh cũng dùng tên thường). `final-table-prompt.png` là fixture API nạp lại vị trí save tuyến; ảnh không hiện prompt nên **không xác minh trực quan nhãn mới**, cũng không là bằng chứng đi controller. Thay đổi nhãn đã compile và kiểm mã. Video tuyến vẫn ghi đúng lượt di chuyển trước sửa chữ này; geometry/controller không đổi.
