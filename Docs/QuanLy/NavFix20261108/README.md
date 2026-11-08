# NAV-01 — sửa vòng đời focus UI, 08/11/2026

**PASS kỹ thuật trên commit nguồn `074caf3dd3da4107b4fc9ac1c089b20216b4705b`.** Người mới **NOT_RUN**. Commit nguồn chỉ sửa hai script; commit báo cáo và biên nhận cập nhật ba nhánh được ghi tại [HANDOFF](../HANDOFF.md). Không push/remote/build/ZIP; không sửa map, luật điều tra, schema save hoặc GUID.

## Mốc và nguyên nhân

Đầu phiên `D:/ConRoi` ở `chore/prepare-r107`, HEAD `b80a78dbaaecfb5b361704e0886c8b8c782b8f32`, không có tracked changes/staged changes. Checkout `C:/ConRoi_CheckoutTest` detached ở `a789222`. Các file cấu hình cờ M có canonical diff rỗng; đã bảo toàn trước thao tác. Không switch/reset source branch.

Trên a789222, UI nhập bản đọc dở, sai và đúng đều tái hiện P không mở hồ sơ sau nút Trở lại. Trong cả ba ca, Page rỗng, Modal/Paused=false và movement/look đã bật; Esc → Ghi chép vẫn hoạt động. Trước nhập, P mở được dossier. [Log tái hiện](repro-suite.txt) ghi kết quả `REPRODUCED`, không phải PASS sản phẩm lỗi.

Nguyên nhân: `textEditing` được lấy từ `GUI.GetNameOfFocusedControl().StartsWith("Entry")` ngay cả khi ô nhập không còn hiển thị. IMGUI vẫn giữ focus tên Entry của trang trước. ClosePage/OpenPage/pause không hủy focus đó; Update tiếp tục chặn P/R bởi cờ textEditing trong khi modal và controller đã trở về trạng thái bình thường. Đây không phải lỗi xác thực mật mã hoặc thay đổi cơ cấu.

Bản sửa **7 dòng thêm / 2 dòng thay** trong `InvestigationSession.cs` và `InvestigationSessionUI.cs`: chuyển trang/pause đặt lại cờ nhập chữ và yêu cầu hủy focus; lần OnGUI kế tiếp mới gọi `GUI.FocusControl(null)` đúng ngữ cảnh; chỉ nhận textEditing khi modal hiện hành không đang chờ xóa focus. Không ép mở dossier mỗi frame, không gỡ chặn phím khi người chơi còn nhập chữ, không đổi Input Actions/controller. Hai cờ là private runtime, không thêm trường save/serialized field.

## Kiểm trên đúng commit sửa

Checkout được fetch local rồi checkout **074caf3**, không copy source untracked. Unity **6000.6.2f1** compile lại thành công (Tundra build success); entry `Assets/ConRoi/Modules/TichHop/Scenes/ConRoi_Chinh.unity` mở ở Edit sạch. [Audit](reference-audit.txt): 32.958 component, **0 Missing Script, 0 reference hỏng**, 4.582 dependency, không thiếu file. Library C độc lập được tạo mới từ đợt checkout đầu; lần này import tăng dần, không copy/chia sẻ Library D.

| Ca | Kết quả |
|---|---|
| Nhập dở rồi Trở lại → P | PASS; UI chỉ gán một ô, bản đọc còn dấu trống |
| Nhập sai rồi Trở lại → P | PASS; UI gán đủ ô, bản đọc khác câu nguồn |
| Nhập đúng rồi Trở lại → P | PASS; UI gán đủ ô, bản đọc khớp câu nguồn |
| P mở/đóng liên tục | PASS, 6 lần nhấn cho mỗi biến thể |
| W/E/chuột khi cipher modal mở | PASS: không dịch vị trí/xoay camera, không đổi số mục chứng cứ |
| Mật mã → đối chiếu → kết luận → hồ sơ | PASS qua nút/phím UI; nhập/ghi liên hệ NAV01 và nhập nhận định N thực sự được kiểm qua dữ liệu |
| Đóng compare/conclusion khi có focus nhập chữ | PASS, P truy cập hồ sơ lại |
| P khi vẫn focus ô nhập | PASS: vẫn ở cipher, không bật dossier; đóng màn xong P hoạt động ngay |
| W/S/A/D và chuột sau đóng UI | PASS: bốn hướng đều dịch chuyển, look xoay được; controller vẫn tiếp đất |
| E theo raycast sau chuỗi UI/load | PASS: controller đi lên gác, raycast slide 2,261 m, mở inspect; không teleport |
| Esc theo ngữ cảnh | PASS: đóng cipher; ở world mở pause; Esc tiếp tục resume |
| Pause không lọt di chuyển/look | PASS |
| F5/F6 ở ô cipher có focus | PASS, giữ mapping/page và không giữ khóa P |
| F5/F6 khi pause hoặc ở world | PASS, load về trạng thái an toàn không pause/khóa input |
| F5/F6 khi đang khám vật | PASS, khôi phục inspect; R/P dùng được |
| 1080p | Render thực 1920×1080; đã xem ảnh cipher/nhận định và trạng thái P/dossier, không đổi bố cục UI |

[Suite sau sửa](fixed-suite.txt), [controller/E](fixed-controller-E.txt), [typing guard/WASD/inspection load](fixed-tail.txt). Các biến thể đúng/sai là bản đọc thử nhập trực tiếp qua các ô UI, không API gán mapping; ca này không lấy việc bấm ghi lời giải làm điều kiện cho P. Đây là kiểm kỹ thuật bằng **GameView mouse event/InputSystem tổng hợp**, không phải người mới chơi.

## Fixture và giới hạn bằng chứng

Profile tái hiện `C:/ConRoi_CheckoutTest/Test/PlaytestProfiles/NavRepro_20261108`; profile sau sửa `.../Nav_fixed_20261108`, ban đầu chưa có save. API fixture chỉ cấp letter/slide/phone để mở navigation; sau ba ca cipher, thêm 2 quan sát slide qua API để có nội dung kiểm trang đối chiếu/kết luận. API chỉ đọc được dùng để assert trạng thái. Không API OpenPage, Assign, Save/Load hay teleport để chứng minh thao tác UI. Không coi fixture là người chơi đã khám/ghi vật chứng. Route E là di chuyển/raycast thật bằng controller tổng hợp nhưng đối tượng đã được fixture cấp trước đó. Input settings/devices/action-map device binding chỉ thay tạm lúc kiểm và phục hồi; không sửa asset Input Actions.

F5/F6 chỉ ghi profile thử schema1/namespace `conroi.investigation.v36.r107`. Save người dùng/LocalLow không được load, migration hoặc ghi. Profile r106 trong archive vẫn đúng hash cũ. Snapshot state/diff/profile trước dừng và sau Editor thoát: [repro](preserve-repro.json), [fixed](preserve-fixed.json), [sau thoát](preserve-afterexit.json). Editor đã đóng khi Edit sạch, không lưu runtime scene. Dữ liệu/ảnh/log dài còn ở C; [index SHA256](evidence-index.json) để tìm lại. Các file báo cáo FAIL 07/11 giữ nguyên, chỉ được thay thế về trạng thái hiện hành bằng báo cáo này.

Kiểm cuối phát hiện một LFS pointer của **ảnh QA trong Docs** vừa đến qua lịch sử báo cáo; không có pointer asset runtime. Đã snapshot pointer rồi copy đúng object đã kiểm hash từ kho Git LFS D sang kho C độc lập và materialize bằng `git lfs checkout`. [Manifest](lfs-doc-correction.json). Sau đó **879 file / 797 object** đều khớp SHA256, fsck OK. Không copy bù working-tree untracked; ảnh QA ngoài Assets không đổi kết quả import/UI đã kiểm.

## Baseline nhóm và phạm vi còn lại

[Audit cuối](audit.json) xác nhận **11.458 GUID meta khớp**, chỉ đúng hai script sản phẩm thay đổi so với a789222; map/scene/prefab/nguồn art/luật/schema giữ nguyên. **4.183 file legacy D chưa xác minh giữ nguyên**, không dọn thêm. Không suy toàn map/legacy PASS. Log cloud/account của Unity vẫn là giới hạn dịch vụ online, không chặn compile/UI local. Người mới tiếp tục **NOT_RUN**.

Ba task branch trước cập nhật đều ở a209c0e, là tổ tiên của commit đã kiểm, không có commit riêng; worktree chỉ có D:/ConRoi và không có stash. Chỉ fast-forward bằng expected-old ref sau commit báo cáo; biên nhận actual old/new ở HANDOFF. Không reset/force nhánh có công việc riêng.

Ownership giữ theo [OWNERSHIP](../OWNERSHIP.md): người 1 map/art, người 2 runtime/contracts và **toàn bộ partial session r107**, người 3 UI/nội dung/scene chính; người 3 gửi đề nghị sửa các partial qua người 2. Chưa phải ba module độc lập. Có thể nhận ba task review theo DANG_LAM trên baseline đã chốt; không mở thêm phát triển/map trong đợt này. Chưa cấu hình remote hoặc push; không cần thông tin đăng nhập để hoàn tất kiểm local.
