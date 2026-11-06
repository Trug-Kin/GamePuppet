# r106 — kiểm trọn chu kỳ và chuẩn bị phiên người mới

**Bổ sung bàn giao 06/11/2026:** [Giao máy — READY, người thật NOT_RUN](playtest_handoff/README.md). Đã tách hướng dẫn người tổ chức/người chơi và mẫu ghi nhận; chỉ kiểm khởi động profile PT001 trống bằng F5/F6, không sửa sản phẩm hoặc chạy lại toàn bộ kiểm cũ.

Ngày kiểm: **06/11/2026**. Unity **6000.6.2f1**. Scene hiện hành `Assets/Scenes/JourneyPresentation_v48_r106.unity`, dẫn xuất r105 vì xác nhận sản phẩm ngắt mô hình ở 10 giây. Checkpoint r101–r105, map, art và save gốc giữ nguyên.

## Nguyên nhân và phần sửa

R105 gọi dừng `AdvanceTrial` tại 10 giây, slider và nút phát lại cũng dùng mốc đó. Baseline Unity `baseline-r105.txt` cho thấy help/hold còn hồi; đánh giá luật tại 20 giây cho thấy chúng sẽ tái cài nếu tiếp tục. Hook còn móc nên chưa cài là đúng. Wedge còn chèn và release có người quay lại giữ cánh cũng là đầu vào có chủ đích.

R106 dùng `TrialTimeline` tính điểm cuối theo trạng thái và hành động cuối, không dùng chung mốc 10 giây. Không tăng tốc hồi hoặc đặt cánh về đóng ở cuối. Dừng có lý do: đã đóng/tái cài, người đang giữ mở, chèn còn giữ, hoặc móc còn nhả chốt khi cánh về vị trí đóng. Nếu guard tính toán 60 giây bị chạm mà chưa ổn định thì không ghi hoàn tất.

Ba nút theo ngữ cảnh **Ngừng đè giữ cánh / Nhả dây khỏi móc / Rút vật chèn** chỉ bỏ giữ khi người chơi chủ động dùng. Thời điểm thao tác trở thành đầu vào của timeline, tua ngược khôi phục phần trước thao tác. Khi hình nộm ngừng đè, nó rời cánh trong 0,6 giây; tiếp xúc giữ cánh trong khoảng rời đó, rồi cùng luật hồi hiện có chạy tiếp. Không biến sáu trường hợp thành sáu animation độc lập.

`ReturnGateMechanism`, `MechanismPresentation` và `PresentationHands` giống r105 sau chuẩn hóa namespace (`law-unchanged.txt`). 10 script GUID cách ly; thêm static `TrialTimeline`. Không thay collider, actor, animation nguồn hay tuyến nên không đi lại tuyến Hà và mọi UI cũ.

## Kết quả

- [Bảng sáu trường hợp](SIX_CASES.md), đo 1/120 giây trong mô hình xác định; không phải số đo cơ cấu thực.
- [Checklist và phạm vi](CHECKLIST.md).
- [Video và loại bằng chứng](VIDEO_INDEX.md).
- [Cách mở profile sạch và phiếu trung tính](PLAYTEST_NGUOI_MOI.md).

`cycle-api.txt`/`cycle-transitions.csv` là đo trạng thái qua API. Video UI là thao tác chuột tổng hợp trong GameView với mô hình animation minh họa, không âm thanh; không phải người chơi thật. Các lượt capture FAIL giữ riêng, không tính đạt. Ảnh cuối từng ca gồm trạng thái đang được giữ và sau thao tác bỏ giữ.

## Nguồn và cách làm lại

Scene r105 → scene r106; script dưới `Assets/Scripts/InvestigationV36R106`. `InvestigationSession` giữ timeline/duration/thời điểm bỏ giữ; `InvestigationReviewUI` điều khiển nút và slider theo duration; `TrialTimeline` lấy sáu lịch đầu vào từ `MechanismPresentation`, gọi đúng `ReturnGateMechanism.Advance`, chỉ thêm đầu vào bỏ giữ do UI yêu cầu. Namespace/schema riêng `conroi.investigation.v36.r106`, schema 1, `InvestigationV36/r106/prototype.json`.

Không chỉnh Blender/FBX đợt này. Nguồn art hiện hành vẫn `Source/Blender/JourneyV48/R105/Repair_R12_R105_Animation.blend` → `Assets/Art/InvestigationV36/R105/RepairEventActor_R105.fbx` → `Repair_clip_illustration/EventActor_R105` trong r106; các actor Quaternius giữ asset R104. Bảng nguồn/CC0 tham chiếu [r105](../investigation_r105/ASSET_SOURCES.md) và [r104](../investigation_r104/ASSET_SOURCES.md). Không tải mới, không nhân bản art chỉ để đổi số phiên bản.

Mở scene đã lưu để dùng sản phẩm; không chạy lại author/derive/bind/isolate một lần. Để sửa tiếp đúng phần này, chỉnh ba file timeline/session/UI cách ly, compile, chạy sáu chu kỳ và hồi quy tác động. Không sửa luật hồi để cho kiểm đạt.

## Bảo toàn đã đối chiếu

`protected-final.json` đối chiếu **1.707/1.707 file** giữ nguyên SHA256, gồm **17/17 save gốc được theo dõi**. Chỉ áp dụng danh sách đã liệt kê, không đại diện mọi file/save trên máy. `delivery-manifest.json`, `evidence-manifest.json` và `results-manifest.json` ghi các file bàn giao thực tế. Scene mở lại sạch, Missing Script 0, broken serialized reference 0.

## Điểm dừng

**Người mới: NOT_RUN.** Phần kỹ thuật và hướng dẫn chuẩn bị phiên thử được bàn giao; bước kế tiếp là lấy phản hồi người thật. Không suy việc chốt cơ cấu thành đã chứng minh Khải hay ý định. Art/tiếp xúc mọi frame và thực nghiệm vật lý ngoài đời vẫn ngoài phạm vi; không gọi toàn prototype PASS. Không build, ZIP hoặc runner sản phẩm mới.
