<!-- nav01-fixed-20261108 -->
## NAV-01 đã kiểm đạt — 08/11/2026

**Commit nguồn đã kiểm: `074caf3dd3da4107b4fc9ac1c089b20216b4705b`.** Sửa tối thiểu vòng đời focus trong hai script r107. Trên checkout độc lập `C:/ConRoi_CheckoutTest`, Unity **6000.6.2f1**, entry `Assets/ConRoi/Modules/TichHop/Scenes/ConRoi_Chinh.unity`: compile/reference đạt, NAV-01 và hồi quy UI/input/save-load **PASS kỹ thuật**. P hoạt động sau nhập đúng/sai/dở, còn được chặn đúng khi ô nhập vẫn focus. Không thay map, luật, schema, GUID hoặc save gốc.

[Báo cáo, fixture và giới hạn](Docs/QuanLy/NavFix20261108/README.md): input tổng hợp; letter/slide/phone và 2 quan sát floor được seed API có ghi rõ, không phải người mới. **Người mới NOT_RUN; 4.183 file legacy chưa xác minh giữ nguyên.** 879 file LFS/797 object materialized đủ, 11.458 GUID meta khớp; runtime không phụ thuộc cache/archive ngoài repo. Library C độc lập, import tăng dần commit sửa. State/diff/profile được snapshot, Editor đã đóng ở Edit sạch không lưu scene.

NAV-01 thay thế trạng thái BLOCKED_UI_REGRESSION 07/11; các FAIL cũ bên dưới được giữ như lịch sử. Commit báo cáo và biên nhận fast-forward ba task branch được ghi riêng sau khi có hash thực tế. Ownership vẫn giữ: người 2 sở hữu toàn bộ partial session, người 3 không đồng thời sửa các file đó. Không push/remote/build/ZIP; chưa tuyên bố toàn map/legacy PASS.

---

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

## Chuẩn bị nhóm Git — r107 đóng băng

Entry nhóm: **[Assets/ConRoi/Modules/TichHop/Scenes/ConRoi_Chinh.unity](Assets/ConRoi/Modules/TichHop/Scenes/ConRoi_Chinh.unity)**, Unity **6000.6.2f1**. Copy r107 với GUID scene mới; checkpoint giữ nguyên. Nhánh `chore/prepare-r107`, source commit `40bcb67035b7`; **chưa push, không remote**.

[Bàn giao thực tế và phần chặn](Docs/QuanLy/HANDOFF.md) · [Ownership](Docs/QuanLy/OWNERSHIP.md) · [Clone/LFS/profile sạch](Docs/QuanLy/CLONE_VA_BAT_DAU.md) · [QA](Docs/QA/README.md) · [Mapping/hoàn nguyên](Docs/QuanLy/OLD_NEW_PATHS.md).

Onboarding/khám/ghi/P/save đã kiểm qua input tổng hợp trên scene copy; người mới NOT_RUN. LFS objects local kiểm OK; **checkout sạch BLOCKED vì thiếu dung lượng**, còn Assets/Docs chưa phân loại. Không coi đã sẵn sàng push hoặc toàn bộ archive hoàn tất. Không chạy author/derive/bind một lần hoặc dùng profile kỹ thuật đã giải. Dừng tính năng tại r107; các phần dưới là lịch sử.

## 06/11/2026 — hiện hành r107: sửa nền và mở đầu từ phản hồi thực

Mở **[JourneyPresentation_v48_r107.unity](Assets/Scenes/JourneyPresentation_v48_r107.unity)** bằng Unity **6000.6.2f1**. Giữ r106 làm checkpoint. [Bàn giao và giới hạn](Test/KetQua/investigation_r107/README.md) · [Người chơi](Test/KetQua/investigation_r107/HUONG_DAN_NGUOI_CHOI.md) · [Người tổ chức/profile sạch](Test/KetQua/investigation_r107/PLAYTEST_NGUOI_MOI.md).

Người dùng đã báo nền hở và UI quá nhiều công cụ. R107 sửa winding trong bốn mesh riêng từ Blender→FBX→Unity, không đổi cao độ/nước/nền chợ. Mở đầu chỉ một việc: vào xưởng, lên gác, kiểm dấu sàn; E khám, xoay/phóng, ghi, R trở lại, P xem lại. Công cụ hiện theo ngữ cảnh; save riêng `InvestigationV36/r107`, không migration save gốc. Đã có video input tổng hợp và đường controller qua vùng sửa. Chưa coi toàn map sạch topology hoặc người mới đã hiểu: phiên r107 **NOT_RUN**, báo cáo ghi các kiểm còn thiếu. Không chạy lại author/derive/bind, không build/ZIP/runner. Các mục dưới là lịch sử.

## 06/11/2026 — bàn giao r106 sẵn sàng, người thật NOT_RUN

[Giao máy](Test/KetQua/investigation_r106/playtest_handoff/README.md) · [Người tổ chức](Test/KetQua/investigation_r106/NGUOI_TO_CHUC.md) · [Chỉ giao cho người chơi](Test/KetQua/investigation_r106/HUONG_DAN_NGUOI_CHOI.md). Slot PT001 chưa gán cho người thật; đã mở Editor 6000.6.2f1 với profile mới và kiểm F5/F6 hồ sơ trống đúng thư mục. Sản phẩm r106 giữ nguyên, không chạy lại cơ cấu/tuyến hoặc polish. Chờ phiên người thật; ghi nhận trung thực theo mẫu trước khi đề nghị sửa.

## 06/11/2026 — hiện hành r106: chu kỳ cơ cấu đã kiểm, chờ người mới

Mở **[JourneyPresentation_v48_r106.unity](Assets/Scenes/JourneyPresentation_v48_r106.unity)** bằng Unity **6000.6.2f1**. Giữ map/art/checkpoint r101–r105. R106 sửa timeline dừng sớm 10 giây, giữ luật hồi; sáu ca chạy đủ tới đóng/cài hoặc trạng thái giữ có lý do. Có nút ngừng đè/nhả móc/rút chèn để xem phần hồi tiếp theo.

[Bàn giao](Test/KetQua/investigation_r106/README.md) · [Bảng sáu ca](Test/KetQua/investigation_r106/SIX_CASES.md) · [Video](Test/KetQua/investigation_r106/VIDEO_INDEX.md) · [Chuẩn bị profile và phiếu người mới](Test/KetQua/investigation_r106/PLAYTEST_NGUOI_MOI.md).

Khởi động phiên người thử bằng Editor đã đóng rồi mở lại với `--investigation-save-dir` tới thư mục mới chưa dùng; xem lệnh chính xác trong phiếu. Đã kiểm tham số khởi động thật và F5/F6 vào thư mục thử, không tự load. **Người thật NOT_RUN**. Bước kế tiếp là lấy phản hồi; không tiếp tục polish chung khi chưa có lỗi cụ thể. Các mục dưới là lịch sử.

## 05/11/2026 — hiện hành: r105, chuẩn bị playtest điều tra

Mở **[JourneyPresentation_v48_r105.unity](Assets/Scenes/JourneyPresentation_v48_r105.unity)** bằng Unity **6000.6.2f1**, Play từ trước xưởng. WASD/chuột, E, R trả/đóng, P hồ sơ, Esc pause, F5/F6 hồ sơ riêng. Mục tiêu: khám xưởng → hỏi Hà → trở lại bàn dựng. Scene lưu hồ sơ trống, không tự load. Namespace `conroi.investigation.v36.r105`, không dùng save gameplay cũ.

[Bàn giao r105](Test/KetQua/investigation_r105/README.md) · [Video](Test/KetQua/investigation_r105/VIDEO_INDEX.md) · [Checklist](Test/KetQua/investigation_r105/CHECKLIST.md) · [Playtest người mới — NOT_RUN](Test/KetQua/investigation_r105/PLAYTEST_NGUOI_MOI.md).

Sửa tay/dụng cụ/dây sửa Tư, hiện dây treo R-12, làm rõ vai giả thuyết và prompt bàn dựng. Giữ Quaternius, map, luật hồi và checkpoint r101–r104. Tư/diễn xuất còn minh họa; chưa có người mới chơi. Các mốc bên dưới là lịch sử.

# Bắt đầu tại đây — Con Rối

## 05/11/2026 — prototype hiện hành r104

Mở **[JourneyPresentation_v48_r104.unity](Assets/Scenes/JourneyPresentation_v48_r104.unity)** bằng Unity **6000.6.2f1**, Play từ trước xưởng. WASD/chuột, E, R trả/đóng, P hồ sơ, Esc pause, F5/F6 hồ sơ riêng. Đi xưởng → quầy Hà → quay lại bàn dựng. GDD đúng: Docs/CON_ROI_GDD_v3.6_CHOT_TRIEN_KHAI.md.

Khải/Hà/rối Quaternius đã nhập thật; clip/bàn dựng có khung lớn, góc cận và slider kéo được. Clip/diễn xuất vẫn bản dựng thử im tiếng; người mới **NOT_RUN**. Save namespace conroi.investigation.v36.r104 riêng, không đọc save cũ. Giữ map r101 và checkpoint r101–r103; không build/ZIP/runner mới.

[Bàn giao r104](Test/KetQua/investigation_r104/README.md) · [Nguồn/giấy phép](Test/KetQua/investigation_r104/ASSET_SOURCES.md) · [Video](Test/KetQua/investigation_r104/VIDEO_INDEX.md) · [Hướng dẫn/phiếu playtest](Test/KetQua/investigation_r104/PLAYTEST_NGUOI_MOI.md). Các mục phía dưới là lịch sử, không dùng save/menu gameplay cũ để bắt đầu prototype mới.


## 05/11/2026 — prototype điều tra hiện hành r103

**Mở [JourneyPresentation_v48_r103.unity](Assets/Scenes/JourneyPresentation_v48_r103.unity) bằng Unity6000.6.2f1, nhấn Play.** Root `Investigation_V36_R103`, player `Khang_FirstPerson_R103`, bắt đầu trước xưởng. Đi khám xưởng → quầy Hà ngoài cửa → quay lại bàn dựng. WASD/chuột, Space, E, R trả/đóng, P hồ sơ, Esc pause, F5/F6 save riêng. Xin mẫu Khải qua mục liên hệ Phương sau khi gặp Khải. Đoạn quay R12 có ngay khi khám điện thoại.

Save riêng namespace `conroi.investigation.v36.r103`, schema1, `InvestigationV36/r103` dưới persistentDataPath; không nạp/migration save cũ. **R101 là map đóng băng; r102 là checkpoint prototype**, không ghi đè. Không build/ZIP/runner mới. GDD đúng: `Docs/CON_ROI_GDD_v3.6_CHOT_TRIEN_KHAI.md`.

[Bàn giao r103 và giới hạn](Test/KetQua/investigation_r103/README.md) · [Nguồn/giấy phép](Test/KetQua/investigation_r103/ASSET_SOURCES.md) · [Playtest người mới — NOT_RUN](Test/KetQua/investigation_r103/PLAYTEST_NGUOI_MOI.md). Vật chứng có geometry để so; clip12s và art nhân vật vẫn bản dựng thử, chưa final. Không suy giờ kéo/ý định từ vải/giày. Hướng dẫn bên dưới là **lịch sử map/gameplay cũ**, không áp dụng profile/menu đó cho r103. Lịch sử r102 giữ trong hai tài liệu chung.

**Project: `D:\ConRoi`. Scene phát triển map hiện hành: [JourneyPresentation_v48_r101.unity](Assets/Scenes/JourneyPresentation_v48_r101.unity), dẫn xuất từ r100: khép terrain X643.5/Z414 và đầu nối r97 ngày 04/11/2026. Giữ checkpoint r94–r100.** Build Settings vẫn bật r92 lịch sử; chưa có build/ZIP r101. Không chọn baseline chỉ theo số lớn nhất; xem mục r101 trong hai tài liệu chung. Các khu gameplay cũ vẫn trong cùng scene, GDDv3.6 xưởng→quầy Hà→xưởng là hướng prototype tiếp theo, chưa triển khai.

1. Mở Unity Hub → Add project from disk → `D:\ConRoi`, dùng **Unity 6000.6.2f1 (770e33f6875c)**. Máy này có Editor tại `D:\data_unity\6000.6.2f1\Editor\Unity.exe`. Mở scene r101 ở trên để tiếp tục map; chờ import/compile xong. Workspace code: [ConRoi.code-workspace](ConRoi.code-workspace).
2. Tìm các root: **`Lake_Market_Story_GDD21`** (điều phối, UI, save/chuyển khu), **`Minh_RegionalWalk_180cm`** → `Main Camera`/`GroundCheck` (Player), **`StoryProps`** → `CR_Hub`, `CR_Lake`, `CR_Market`; **`School_V37_Classroom_Courtyard`**, **`DaLat_Slope_V39`**. Map dùng thêm `LakeGateMarketV32`, `RouteSurfaces`, `R20_Whole_Source_Export`; ánh sáng tại `DaLat_Daylight`. UI phần lớn chỉ xuất hiện khi Play.
3. Play mở menu: Enter/**New Game** bắt đầu ở phòng; L/**Load Game** là Continue nếu profile có save hợp lệ. H mở trợ giúp; WASD/chuột, Space nhảy, E tương tác, P điện thoại, Esc pause, F5/F6 lưu/tải. Xem mô tả đầy đủ trong menu H và [MenuVaoGameV47.cs](Assets/Scripts/GiaoDien/MenuVaoGameV47.cs). New Game ghi lại profile hiện tại: **không dùng profile người dùng để thử**.
4. Khi kiểm riêng, đóng Editor rồi mở với `--r30-review-output D:\ConRoi\Test\KetQua\<phien-moi>\save-rieng` và `--verification-output D:\ConRoi\Test\KetQua\<phien-moi>\telemetry` (tạo thư mục telemetry trước). Đối số đầu đã kiểm thực tế với r92; đối số thứ hai được xác nhận từ code V40/V41. Một số telemetry V42/V48 vẫn mặc định ghi LocalLow, xem giới hạn trong tài liệu chung. `Tools/KiemUnity.ps1 -Mode Open` chưa tự truyền profile save riêng. Mở chỉ để đọc scene thì không cần Play.
5. Nguồn Blender phát triển: [Source/Blender/JourneyV48](Source/Blender/JourneyV48/) và các editable còn dùng trong `Docs/Reviews`; tra đúng model theo workflow, không chọn bản REVIEW thay nguồn gốc. Bộ báo cáo mở [ConRoi_TongHop_TrinhBay.blend](TrinhBay/Blender/ConRoi_TongHop_TrinhBay.blend), xem [HUONG_DAN](TrinhBay/Blender/HUONG_DAN.md). Bộ này là bản sao trình bày, không tự thay nguồn Unity.
6. Dữ liệu kiểm theo đợt: [Test/BanChoiThu](Test/BanChoiThu/) ZIP/player; [Test/BangChung](Test/BangChung/) ảnh/video; [Test/KetQua](Test/KetQua/) log/telemetry/report; [Test/Tam](Test/Tam/) dữ liệu tái tạo. [Test/README](Test/README.md) chỉ cách tra đường cũ. ZIP hiện hành: `Test/BanChoiThu/journey_v48/restructure-20261102/ConRoi-r92-Reorganized-20261102-Windows-playtest.zip`.

[Hướng dẫn và lịch sử — scene/Hierarchy/kết quả kiểm](Docs/PROJECT_GUIDE_AND_HISTORY.md) · [Workflow Blender–Unity — nguồn model/reference](Docs/BLENDER_UNITY_WORKFLOW.md) · [Danh mục đủ 117 scene](Docs/Inventory/20261102/SCENES.md).

Đợt này đã đọc trực tiếp Hierarchy/serialized Inspector trong Editor, kiểm 0 Missing Script/0 reference hỏng và Play khởi tạo + handler điện thoại trên save riêng. Chưa kiểm lại toàn tuyến, mọi scene lịch sử, phím E/raycast hay toàn bộ hình ảnh; không tạo build mới.
