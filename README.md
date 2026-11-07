<!-- checkout-final-a789222-20261107 -->
## Kiểm baseline a789222 — kết quả hiện hành 07/11/2026

**Đã kiểm thực tế `a789222286f1a56db5b6cc21e1ffb5a0e7542be4` tại `C:/ConRoi_CheckoutTest`, Unity 6000.6.2f1.** Git/LFS/import/reference PASS: 878 file LFS materialized, 0 Missing Script/0 reference hỏng, 4.582 dependency. Library C độc lập tạo mới ở lần checkout đầu, import tăng dần baseline sửa; không dùng chung Library D. Entry `Assets/ConRoi/Modules/TichHop/Scenes/ConRoi_Chinh.unity`.

**Tổng thể `BLOCKED_UI_REGRESSION`**, không còn chặn vì dung lượng/quyền điều khiển. Controller Start → dấu sàn → khám/ghi/R/P → điện thoại, 1080p, save/load và skip/revisit đạt bằng input tổng hợp. Navigation mật mã/đối chiếu/kết luận được kiểm với thư cấp API trong profile fixture riêng. Ca **nhập mật mã → Trở lại → P không mở hồ sơ** chưa đạt; Esc → Ghi chép là đường truy cập được, không thay cho PASS. Chưa xác nhận nguyên nhân cuối, không sửa sản phẩm trong đợt này.

Đã bảo toàn diff/state trước Play stop/sync, quarantine 11 file Unity tự sinh, sync từ 8ea91d4 qua Git/LFS tới a789222; không ghi đè snapshot. Editor đã dừng/đóng khi Edit sạch, không save runtime scene. ProjectSettings ghi lúc thoát có snapshot và diff nội dung rỗng; source D không đổi sản phẩm. Lần intro tự đổi ở phiên cũ vẫn **chưa rõ nguồn input**, không gọi là lỗi gameplay.

Ba task branch **giữ a209c0e**, chưa nâng vì còn NAV-01; ownership không đổi, người 2 giữ toàn bộ partial session r107. **4.183 file D chưa xác minh giữ nguyên**. Save gốc/archive/GUID bảo toàn, profile kỹ thuật riêng ở C. Người mới **NOT_RUN**. Không sửa map/gameplay, không remote/push/build/ZIP/runner runtime mới.

Xem [báo cáo cuối](Docs/QuanLy/CheckoutFinal20261107/README.md), [audit](Docs/QuanLy/CheckoutFinal20261107/final-audit.json) và [manifest/index bằng chứng](Docs/QuanLy/CheckoutFinal20261107/evidence-index.json). Commit sau a789222 chỉ là tài liệu/công cụ kiểm; không gọi commit tài liệu là đã chạy sản phẩm. Các phần bên dưới là lịch sử, không thay kết quả hiện hành này.

---

## Checkout độc lập trên C — cập nhật 07/11/2026

Source giữ `D:/ConRoi`, nhánh `chore/prepare-r107`. **Baseline source hiện `a789222286f1a56db5b6cc21e1ffb5a0e7542be4`**; baseline đã mở lần đầu trong checkout là `8ea91d4d64236f5d8507f216112997402d9b46f6`. Commit tài liệu sau hai mốc này không được gọi là đã kiểm sản phẩm.

[Kết quả hiện hành](Docs/QuanLy/CheckoutTest20261107/README.md) thay thế tình trạng BLOCKED_CAPACITY của lần trước: đã tạo checkout thực tại **`C:/ConRoi_CheckoutTest`**, Git objects độc lập, không remote/alternates, không copy file untracked và không dùng chung Library. Trước import C còn 43.972.919.296 byte. LFS đã kiểm hash/materialize **877 file / 795 object / 6.136.024.587 byte**. Library thực được tạo mới bởi Unity **6000.6.2f1**, không copy từ D.

Đã commit đủ **11.326 file A / 3.562.022.051 byte** cùng meta. Sau import phát hiện thêm 5 file D thực sự thuộc closure GUID cấu hình HDRP có sẵn: hai asset, hai meta và folder meta; đã bổ sung có manifest, không đổi pipeline. **D chưa xác minh còn 4.183 file, giữ nguyên tại D**, không archive bừa.

Có lỗi Git thật trong lần checkout đầu: scene legacy nhị phân `Assets/_Recovery/0 (2).unity` bị quy tắc `*.unity text` làm mất 10 byte khi normalize CRLF. Commit `a789222` sửa bằng **LFS đúng đường dẫn**, object hash `00eac743343893f614abbb6cbb1ddc286565d95f330e4913ced30345384a7939`, khớp bản gốc 7.619.172 byte. Không sửa/reserialize checkpoint. Scene entry và prefab text khác vẫn review bình thường. Không rewrite lịch sử; `8ea91d4` giữ làm bằng chứng chưa hoàn chỉnh.

Trên checkout `8ea91d4`: compile C#/Editor đạt; entry **`Assets/ConRoi/Modules/TichHop/Scenes/ConRoi_Chinh.unity`** mở được, **32.958 component, 0 Missing Script, 0 reference hỏng**, AssetDatabase có đủ 4.582 dependency. GUID meta A khớp và entry không đổi sau chuẩn hóa xuống dòng. Có ảnh render thực **1920×1080**. Những kết quả này không thay cho kiểm UI hoặc xác nhận baseline sửa `a789222` đã chạy.

**Tổng thể vẫn BLOCKED — cần xác nhận quyền điều khiển Editor và hoàn tất kiểm baseline sửa.** Công cụ input tổng hợp chưa xác nhận được Start/luồng UI; intro về sau đã đóng nhưng nguồn thao tác chưa phân biệt chắc. Đã hỏi người dùng có đang điều khiển Unity; chưa có câu trả lời tại lúc ghi. Không gửi thêm input, không dừng Play/discard/lưu runtime vào scene. State live và các thay đổi Unity tự sinh đã snapshot, xem `current-status.json`. Hồ sơ CLI riêng tại `C:/ConRoi_CheckoutTest/Test/PlaytestProfiles/Clean_20261107`; không ghi vào save gốc. Chưa có bằng chứng F5/F6 thành công trong checkout.

Các ca **E/khám/ghi/R/P, save/load trước ghi, skip rồi quay lại, điện thoại, cipher/compare/conclusions qua navigation** còn **NOT_RUN/INCOMPLETE**. Người mới **NOT_RUN**. Lỗi fixture/giới hạn bằng chứng được giữ trong `FIXTURE_ATTEMPTS.md`; không chuyển FAIL cũ thành PASS. Không tuyên bố toàn legacy/map PASS.

Tiếp tục theo thứ tự: xác nhận Editor checkout không được người dùng thao tác → bảo toàn và đóng Play có kiểm soát → đưa checkout tới `a789222` bằng Git/LFS, không copy bù untracked và không ghi đè thay đổi tự sinh đã snapshot → mở lại entry/kiểm reference và các ca UI còn thiếu → chỉ sau đạt mới cập nhật ba task branch nếu bảo toàn commit riêng. Ba task branch vẫn ở **`a209c0e`**; chưa giao clone để sửa độc lập. Ownership không đổi theo OWNERSHIP/TICH_HOP; người 2 tạm sở hữu toàn bộ InvestigationSession partial, người 3 không cùng sửa file đó.

Không tạo thêm archive/build/ZIP/runner, không xóa dữ liệu, không tạo remote hoặc push. Log import dài và cache/profile/snapshot giữ ngoài Git trong checkout C; manifest và báo cáo nhỏ ở đây. Dữ liệu archive cũ tại D:/Game giữ nguyên. Chưa cần thông tin remote/login.

## Rà soát bàn giao 07/11/2026 — checkout vẫn BLOCKED

Baseline sản phẩm được rà: **`90147d9382349575c5bc0385f7405525cad727cd`**, nhánh `chore/prepare-r107`. Đầu phiên không có thay đổi tracked; không remote, không push, không đổi sản phẩm. Commit bổ sung sau mốc này chỉ chứa tài liệu/công cụ kiểm. Entry giữ `Assets/ConRoi/Modules/TichHop/Scenes/ConRoi_Chinh.unity`, Unity **6000.6.2f1**. Không tạo r108, build, ZIP hay runner sản phẩm.

[Kết quả chi tiết](Docs/QuanLy/Review20261107/README.md) là cập nhật hiện hành; số liệu phía dưới thuộc lần bàn giao trước. LFS máy gốc: **296 file / 217 object / 4.503.946.222 byte**, hash/size và materialization đều đúng. Chưa chứng minh clone chạy được.

Checkout độc lập hiện cần ước tính **27.709.685.653 byte (27,71 GB)**: working tree 6.223.984.553 + LFS 4.503.946.222 + Git 306.433.037 + Library mới dự trù 12.380.354.545 + dự phòng 4.294.967.296. Library dựa trên cache hiện có, chưa phải số đo import mới. C trống 10.197.442.560; D 2.464.395.264; G 9.687.568.384 byte. C thiếu 17.512.243.093; D thiếu 25.245.290.389; G thiếu 18.022.117.269 byte. G chưa xác nhận vị trí/vai trò, không sử dụng. Không dùng ổ ngoài/cloud.

Legacy 15.514 file đã phân loại tĩnh: **A 11.326 file / 3.562.022.051 byte** có consumer cụ thể (gồm Editor/historical tools); **D 4.188 file / 5.402.343.180 byte** chưa xác minh, giữ nguyên. A vẫn UNTRACKED: chưa được coi baseline đủ cho toàn bộ công cụ. Chuỗi GUID của entry và tập compiled source không thiếu file Git theo kiểm tĩnh. Chưa có AssetDatabase/import từ checkout sạch. Báo cáo refined thay thế phân loại raw vốn nhận quá rộng literal `Assets/`. Không suy không dùng từ việc vắng trong scene.

Nếu giữ toàn bộ A để bàn giao Editor, ngân sách bảo thủ tăng tối đa hai lần 3.562.022.051 byte (working tree + object), thành **34.833.729.755 byte**, chưa khử trùng LFS và nén Git. Cần vị trí local được xác nhận có **khoảng 36 GB trống** để bổ sung A và kiểm; sau chọn tập source sẽ đo lại. Không stage hàng GB khi D chỉ còn khoảng 2,46 GB. Nguồn B: Source/license đã tracked; editable trong Assets chưa xác minh recipe tiếp tục D. Không chuyển bất kỳ Assets/meta/GUID nào trong lần rà này.

852 script Tools: **852 hash giữ nguyên**. Năm cảnh báo lexical đều là từ Python `from`, không phải type C# bị mất. Kiểm tĩnh không thay cho compile mới. Scene/prefab text, source/media game và license giữ Git/LFS; cache, profile cá nhân, capture/build/log không thuộc source clone. Docs/Reviews còn trộn nguồn, giữ UNCLASSIFIED.

Lô C mới `D:/Game/ConRoi_LuuTru/git-review-20261107-001945-00b993`: **8 file / 2.108.862 byte** ảnh kiểm GitPreparation và log Editor đã dừng; staging/hash/move/hash được kiểm lại. Manifest ở bộ review; hoàn nguyên bằng old/new/SHA256, copy về khi old vắng, không ghi đè xung đột. Lô cũ `git-team-20261106-153912-ee41c6` giữ nguyên. Chuyển cùng D **không tăng dung lượng trống**; staging mới còn dùng thêm 2.108.862 byte. Không chuyển cache qua junction, runtime media hoặc save gốc.

**Checkout sạch BLOCKED_CAPACITY**; chưa tạo checkout/Library, chưa chạy compile/import/reference/onboarding/save thử trên checkout. Các ca E/ghi/R/P, save trước ghi, skip tutorial, điện thoại, 1080p và mật mã/đối chiếu/kết luận ở checkout đều **NOT_RUN**. Không mượn PASS máy gốc; người mới vẫn NOT_RUN. Không cần remote/login ở bước này.

Ba task branch giữ `a209c0e3b41a8b55e438aeabf1025b5ef6e7d940`, không tiến tới baseline chưa được clean-test. Ownership không đổi: người 1 Map/art/source; người 2 runtime/contracts và toàn bộ InvestigationSession partial tạm thời; người 3 UI/nội dung/scene tích hợp, gửi đề nghị qua người 2 nếu chạm partial dùng chung. Chưa đủ điều kiện tách clone để sửa độc lập. Thứ tự: xác nhận vị trí đủ chỗ → bổ sung A có kiểm → checkout/LFS/import/test → chốt commit đã kiểm → cập nhật task branch nếu không mất việc riêng. Trong lúc chờ chỉ đọc inventory/contracts và nhận task; không polish map/gameplay. CODEOWNERS chờ username thật.

# Con Rối — source r107, chuẩn bị nhóm Git

Entry nhóm: **[Assets/ConRoi/Modules/TichHop/Scenes/ConRoi_Chinh.unity](Assets/ConRoi/Modules/TichHop/Scenes/ConRoi_Chinh.unity)**, Unity **6000.6.2f1**. Copy r107 với GUID scene mới; checkpoint giữ nguyên. Nhánh `chore/prepare-r107`, source commit `40bcb67035b7`; **chưa push, không remote**.

[Bàn giao thực tế và phần chặn](Docs/QuanLy/HANDOFF.md) · [Ownership](Docs/QuanLy/OWNERSHIP.md) · [Clone/LFS/profile sạch](Docs/QuanLy/CLONE_VA_BAT_DAU.md) · [QA](Docs/QA/README.md) · [Mapping/hoàn nguyên](Docs/QuanLy/OLD_NEW_PATHS.md).

Onboarding/khám/ghi/P/save đã kiểm qua input tổng hợp trên scene copy; người mới NOT_RUN. LFS objects local kiểm OK; **checkout sạch BLOCKED vì thiếu dung lượng**, còn Assets/Docs chưa phân loại. Không coi đã sẵn sàng push hoặc toàn bộ archive hoàn tất. Không chạy author/derive/bind một lần hoặc dùng profile kỹ thuật đã giải. Dừng tính năng tại r107; các phần dưới là lịch sử.

# Dữ liệu kiểm thử đã tách

Source sản phẩm tại D:/ConRoi; [Test/README](Test/README.md) chỉ dẫn ZIP/player, ảnh/video, report và output cấu hình được. Không tạo build mới trong đợt này. Docs/Reviews còn nguồn/fixture/checkpoint được giữ, không coi toàn bộ là output kiểm.

# CON RỐI — source r92 tại D:/ConRoi

Đã tổ chức script theo chức năng và Việt hóa 100 file, giữ gameplay/model/save và ZIP r90/r91/r92. Mở ConRoi.code-workspace hoặc Unity Hub ConRoi; scene Assets/Scenes/JourneyPresentation_v48_r92.unity.

[Hướng dẫn, kiểm, đổi tên, hoàn nguyên](Docs/PROJECT_GUIDE_AND_HISTORY.md) · [Workflow Blender/Unity](Docs/BLENDER_UNITY_WORKFLOW.md) · [Builds](Builds/README.md).

Phần dưới là lịch sử bàn giao.

---

# CON RỐI — source r44 tạm chốt

Phiên blocker tiếp theo chưa hoàn tất: [kết quả từng mục](Docs/Reviews/journey_v48/r44-content/blocker-session-run-20261025/REVIEW.md). Các blocker còn NOT_RUN; lỗi điều khiển/chọn runner được ghi riêng, không sửa sản phẩm/capture hoặc build. SAC cuối phiên 17:18:42 +07:00 ngày 25/10 vẫn Off/0; đã nhắc bật lại On, chưa xác nhận phục hồi. Mốc r44 và kết quả Editor trước đó giữ nguyên; xác nhận On ở đoạn dưới là lịch sử trước phiên Off mới này.

Giữ r44 làm mốc đã kiểm Editor: `Assets/Scenes/JourneyDevelopment_v48_r44.unity`. Build Settings vẫn bật r42; chưa xuất build r44. Đã compile/mở/lưu–đóng–mở scene và chạy Play Mode bốn khu tới ending A: 157 assertion gameplay đạt; kết quả công cụ full complete=false do capture menu, tách riêng và chưa sửa capture. Edge dốc 1080p PASS 77 assertion. Các ca còn thiếu được phân loại trong UNITY_PENDING; không chạy lại PASS khi không có thay đổi liên quan. **SAC đã xác minh On/1 lúc 14:05:19 +07:00 ngày 25/10/2026.** Không thêm tính năng/công cụ/build.

[Kết quả Editor/Play Mode](Docs/Reviews/journey_v48/r44-content/editor-sac-off-20261025/REVIEW.md) · [Video ngắn từ gameplay thật, có cắt](Test/BangChung/journey_v48/r44-content/editor-sac-off-20261025/r44-editor-gameplay-short.mp4) · [Checklist Unity còn chờ](Docs/Reviews/journey_v48/r44-content/UNITY_PENDING.md) · [Hướng dẫn và lịch sử dự án](Docs/PROJECT_GUIDE_AND_HISTORY.md). C# ngoại tuyến ghi riêng. Giữ r42, map/model, controller, checkpoint/save.

Các phần bên dưới là lịch sử bàn giao.

# CON RỐI — review hồ r38

[Hướng dẫn project và lịch sử xuyên suốt tới r38](Docs/PROJECT_GUIDE_AND_HISTORY.md): đọc trước khi sửa; cập nhật cùng tài liệu này sau mỗi đợt hoàn chỉnh theo checklist ở mục 10. Tài liệu phân biệt nguồn chỉnh sửa, scene/build hiện hành và giới hạn kiểm chứng.

Scene dẫn xuất đang bật trong Build Settings: `Assets/Scenes/JourneyRepair_v48_r38.unity`. Giữ checkpoint r37 và profile save r35-market. Đợt này chỉ sửa đế giày Bảo, thao tác cặp trang sổ và đồng bộ camera/pipeline phản chiếu.

[Báo cáo và giới hạn](Docs/Reviews/journey_v48/r38/REVIEW.md) · [ZIP Windows](Test/BanChoiThu/journey_v48/r38/ConRoi-v48-r38-LakeReview-Windows-x64.zip) · [Video](Test/BangChung/journey_v48/r38/r38-main-CONTINUOUS-OFFSCREEN.mp4). Chi tiết phản chiếu tay/sổ/rối bị mặt cầu che từ footway, chưa chứng minh nhìn rõ trên nước. Hiển thị/input Windows vật lý và vấn đề menu đen vẫn tách riêng.

Các mục dưới đây giữ nguyên làm lịch sử, không phải kết quả kiểm r38.

---

# CON RỐI — bàn giao v47 RC3

Ứng viên nghiệm thu tiếp tục từ checkpoint RC2 giữ nguyên. 601 kiểm tra PASS trên binary đóng gói; có kiểm launch thường, New Game/hướng dẫn/pause/lưu tải, bốn ending và đo hồ foreground không capture. Không tuyên bố đạt 30–40 phút hoặc đã loại mọi spike.

- [ZIP nghiệm thu RC3](Test/BanChoiThu/journey_v47/ConRoi-v47-RC3-Windows-x64.zip)
- [Hướng dẫn](Docs/Reviews/journey_v47_rc3/PLAYER-GUIDE.md), [bảng GDD](Docs/Reviews/journey_v47_rc3/GDD-v2.1-audit.md), [hạn chế](Docs/Reviews/journey_v47_rc3/LIMITATIONS.md)
- [Kết quả](Docs/Reviews/journey_v47_rc3/RESULTS.md), [đo foreground](Docs/Reviews/journey_v47_rc3/PERFORMANCE.md), [ánh sáng trước/sau](Docs/Reviews/journey_v47_rc3/VISIBILITY.md)
- [Video tuyến không cắt](Test/BangChung/journey_v47/ConRoi-v47-RC3-player-route.mp4)
- [Hash/kiểm ZIP cuối](Docs/Reviews/journey_v47_rc3/final-delivery-check.json)

Phần dưới giữ làm lịch sử RC2.

# CON RỐI — v47 RC2 (checkpoint)

Ứng viên chơi trọn tuyến, tiếp tục từ checkpoint v46 r2. 537 ca chức năng và 492 ca lặp trong đo hiệu năng PASS trên binary đóng gói. Bốn kết thúc đi từ đầu, lưu/tải và chơi lại đã kiểm. Spike còn lại và mục tiêu thời lượng chưa chứng nhận được ghi rõ trong báo cáo; không tuyên bố đã hết khựng.

- [Build Windows ZIP RC2](Test/BanChoiThu/journey_v47/ConRoi-v47-RC2-Windows-x64.zip)
- [Kết quả và phân loại GDD](Docs/Reviews/journey_v47/RESULTS.md)
- [Hướng dẫn điều khiển, luồng chơi, lưu/tải](Docs/Reviews/journey_v47/PLAYER-GUIDE.md)
- [Bốn kết thúc — có spoiler](Docs/Reviews/journey_v47/ENDINGS.md)
- [Hiệu năng và thử nghiệm bị loại](Docs/Reviews/journey_v47/PERFORMANCE.md)
- [Giới hạn](Docs/Reviews/journey_v47/LIMITATIONS.md)
- [Video toàn tuyến liên tục](Test/BangChung/journey_v47/ConRoi-v47-RC2-full-route.mp4)
- [Video khám phá/đọc nội dung liên tục](Test/BangChung/journey_v47/ConRoi-v47-RC2-full-experience.mp4)

## Checkpoint v46 r2 — giữ nguyên

Tiếp tục v45 r4. Hoàn thiện bề mặt sáu NPC người, ưu tiên Hà/Vy; giữ diễn xuất, tiếp xúc và quy tắc rối.

- [Kết quả và giới hạn](Docs/Reviews/journey_v46/RESULTS.md)
- [Marker và frame time](Docs/Reviews/journey_v46/PERFORMANCE.md)
- [Build Windows ZIP](Test/BanChoiThu/journey_v46/ConRoi-v46-Experience-Windows-x64.zip)
- [Clip trước/sau ở góc người chơi](Test/BangChung/journey_v46/ConRoi-v46-before-after-player.mp4)
- [Toàn tuyến sau — liên tục](Test/BangChung/journey_v46/ConRoi-v46-after-player.mp4)
- [Toàn tuyến trước — liên tục](Test/BangChung/journey_v46/ConRoi-v46-before-player.mp4)
- [Ảnh Hà trên tuyến](Test/BangChung/journey_v46/images/Ha-player-before-after.jpg)
- [Ảnh Vy trên tuyến](Test/BangChung/journey_v46/images/Vy-player-before-after.jpg)
- [Clip kiểm cận bổ sung](Test/BangChung/journey_v46/ConRoi-v46-inspection.mp4)
