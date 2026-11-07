# Kiểm checkout cuối — 07/11/2026

**Baseline thực sự đã kiểm: `a789222286f1a56db5b6cc21e1ffb5a0e7542be4`. Git/LFS/import/reference PASS; tổng thể `BLOCKED_UI_REGRESSION`.** Không nâng ba nhánh task vì một ca navigation chưa đạt. Người mới **NOT_RUN**. Không sửa sản phẩm, map/gameplay, không push/remote/build/ZIP/runner runtime mới.

Source giữ `D:/ConRoi`, nhánh `chore/prepare-r107`; đầu phiên HEAD `137ce0eb396557c77af8bb042ab5f3399cde5487`, không có tracked changes. Commit báo cáo sau này không được gọi là commit sản phẩm đã chạy. Checkout độc lập `C:/ConRoi_CheckoutTest` đã chuyển từ `8ea91d4` tới **a789222** qua Git local fetch; không copy bù untracked. Entry giữ `Assets/ConRoi/Modules/TichHop/Scenes/ConRoi_Chinh.unity`, Unity **6000.6.2f1**.

## Bảo toàn trước thao tác

Người dùng xác nhận không ai điều khiển checkout. Trước input/dừng Play, đã copy/hash **11 file / 117.627 byte**, giữ binary diff và live JSON/context/ảnh tại `BeforeSync_20261107_102541_948f58` và `LiveSnapshot_20261107_102542` dưới `C:/ConRoi_CheckoutTest/Test/Output/CleanCheckout`. Scene dirty=False. Dừng Play, chỉ thoát khi Edit sạch, không save scene. Khi không còn Editor ghi, đối chiếu hash rồi move nguyên file vào `Quarantine_20261107_173159`. Cả snapshot và quarantine vẫn còn; không discard/overwrite chúng. Xem [manifest đầu](preservation-before-sync.json), [kết quả sync](sync-result.json).

Lúc kết thúc kiểm cũng snapshot profile/diff trước dừng; sau Editor thoát snapshot bổ sung các ProjectSettings được ghi khi thoát. Xem [sau kiểm](preservation-after-tests.json), [sau Editor thoát](preservation-after-exit.json). Ba file ProjectSettings hiện có cờ M trong status nhưng **git diff nội dung rỗng**; giữ byte và metadata hiện có, không tự restore. `Assets/Packages/ProjectSettings/Source` ở D không có diff sản phẩm. **11.458 GUID meta** giữa source/checkout khớp. Profile PT001 r106 đã archive từ trước vẫn đúng hash cũ; không thay baseline hash.

Lần intro tự đóng ở phiên trước vẫn ghi **chưa xác định nguồn input**, không quy thành lỗi gameplay. Lần kiểm này Start được click native có guard foreground đúng PID, rồi API chỉ đọc xác nhận introAccepted=true, hồ sơ trống. Các lần sau dùng GameView event và InputSystem tổng hợp, runtime input settings tạm được phục hồi; không sửa Input Actions/controller.

## Checkout và import

- Sau sync: **878 file LFS / 796 object**, từng file materialized khớp SHA256; `git lfs fsck --objects` OK. [LFS](lfs-materialized.json).
- Recovery binary sau import vẫn đúng SHA256 `00eac743343893f614abbb6cbb1ddc286565d95f330e4913ced30345384a7939`, không còn lỗi mất 10 byte ở lần import cũ. HDRP config gốc đến từ Git, không dùng bản auto-generated đã quarantine, không đổi pipeline.
- Library C được tạo mới khi kiểm 8ea91d4; **a789222 được import tăng dần trên Library độc lập đó**. Không tuyên bố đã tạo thêm Library trống cho a789222; không chia sẻ/copy Library D. Đợt này trước sync C trống 35.007.922.176 byte; sau sync 34.552.905.728 byte, đủ cho cập nhật nhỏ và thử tiếp. Không tạo clone thứ hai/import lặp để lấp thời gian.
- Compile C#/Editor và mở entry đạt; audit Edit: **32.958 component, 0 Missing Script, 0 reference serialized hỏng, 4.582 dependency, 0 file dependency thiếu**. [Audit](reference-audit.txt). Bridge snippet từng có CS1503 do ghi Exception vào AppendLine: lỗi công cụ kiểm, đã sửa/rerun, không phải lỗi compile project. Log network/account/telemetry của Unity giữ riêng, không coi là lỗi runtime entry.

## Ca thực hiện trên a789222

| Ca | Kết quả và bằng chứng |
|---|---|
| Start, hồ sơ trống, không tự load | PASS; native click rồi read-only state; Play lần hai cũng không tự load save kỹ thuật có sẵn |
| Save/load trước ghi đầu | PASS; F5/P/F6, evidence/mapping trống, vị trí giữ nguyên — [log](before-record-calibrated.txt) |
| Cửa xưởng → gác → E dấu sàn | PASS controller tổng hợp, không teleport/API mở; raycast slide 2,239 m — [log](onboarding-route.txt) |
| Xoay/phóng → Ghi → R → P | PASS; ghi đúng 2 quan sát, không thêm chứng cứ chưa xem; camera người chơi không xoay sau UI — [log](onboarding-ui-complete.txt) |
| Mục tiêu thứ hai → điện thoại | PASS controller/E; raycast 1,409 m, hồ sơ có 2 mục — [log](phone-route.txt) |
| Save/load giữa tiến trình | PASS ở sau ghi/P: giữ quan sát, mục tiêu và vị trí — log onboarding |
| Mật mã | PASS mở bằng navigation dossier, gõ một chữ qua UI; **thư được cấp bằng API trong profile fixture riêng**, không chứng minh đã đi tuyến Hà |
| Rời ô nhập mật mã → Trở lại → P | **FAIL**, P không mở hồ sơ; thử lại vẫn không mở. Esc → Ghi chép truy cập được. Xem vấn đề bên dưới |
| Đối chiếu | PASS từ navigation; chọn vệt trượt/điện thoại, nhập và ghi liên hệ; F5/F6 giữ page/liên hệ — [log](navigation-save-ui.txt) |
| Kết luận | PASS từ nút chính dossier; chọn Chưa đủ chứng minh, xác nhận/lưu bằng UI; F6 giữ kết luận, mapping, quan sát. Không tự gán Khải — cùng log |
| Bỏ qua hướng dẫn | PASS nút Skip; case vẫn trống; F5/P/F6 giữ skip, không tự load/ghi quan sát — [log](skip-ui.txt) |
| Skip → đến vật → R/E quay lại → load lúc chưa ghi | PASS controller/E; F5/P/F6 giữ inspect/skip, observation vẫn 0 — [route](skip-route.txt), [UI](skip-revisit-ui.txt) |
| UI 1080p | Render thực 1920×1080; đã xem ảnh khám/ghi/dossier/cipher/compare/conclusions/skip; các trang đã kiểm đọc được và nút không chồng. Không tuyên bố mọi trạng thái/UI khác PASS |

Mật mã/compare/conclusion dùng profile `NavFixture_a789222_20261107`; chỉ Acquire(letter) là API fixture. Không dùng API OpenPage/Observe/Save/Load để chứng minh navigation hoặc thao tác ghi/lưu. `nav-setup` đầu thiếu input settings nên R/P không chạy: giữ [raw state](nav-setup.txt), không coi là PASS; các ca sau bật runtime settings tạm và có assertion. F5 trong setup đầu không được coi là đã lưu điện thoại vì file đã tồn tại; profile route chính giữ nguyên bản save sau ghi/P, live sau điện thoại được snapshot riêng.

Một số nhãn file/log kế thừa công cụ cũ: `first-inspection-720.png` thực tế là 1920×1080; `intro-1080.png` của before-record chụp sau Start, không dùng làm ảnh intro. Dùng `skip-intro-1080.png` cho intro thật. Header skip-ui nhắc letter fixture do kế thừa template; code/guard và assertion xác nhận ca skip dùng profile mới với 0 evidence/mapping, **không seed thư**. Giữ raw log, không sửa FAIL cũ thành PASS.

## Vấn đề cần xử lý riêng

**NAV-01 — P không mở hồ sơ sau nhập mật mã rồi Trở lại** (mức 3: khó thao tác).

Tái hiện tổng hợp: dossier → Đọc thư Hà → click ô đầu → gõ A → nút Trở lại → P. Page đã rỗng nhưng không mở dossier; retry P vẫn page rỗng. Esc mở pause, Ghi chép vẫn hoạt động; P sau compare không nhập chữ hoạt động lại. [Raw FAIL](navigation-continuation-ui.txt), [retry/workaround](navigation-remaining-ui.txt), [state sau FAIL](nav-back-failure-state.json). Kiểm tra code cho thấy P bị chặn khi `textEditing`; **nghi focus ô nhập còn lưu**, chưa xác nhận nguyên nhân cuối và chưa sửa. Không cần thêm dung lượng hoặc remote để xử lý vấn đề này. Chưa kiểm lại bằng người thật/phím vật lý; kết quả hiện là input tổng hợp.

## Dữ liệu, ownership và điểm dừng

Profile riêng tại `C:/ConRoi_CheckoutTest/Test/PlaytestProfiles/`: `Final_a789222_20261107_1740`, `NavFixture_a789222_20261107`, `Skip_a789222_20261107`. F5/F6 đã xác nhận đúng namespace/schema/path. Save người dùng/LocalLow không được load/migration/ghi bởi kiểm này. Snapshot profile thử và byte config giữ ngoài Git tại C; ảnh/capture/log dài/Library cũng ngoài Git. [Index hash](evidence-index.json) chỉ đường tới ảnh và state gốc; log nhỏ được giữ cùng báo cáo. Native screenshot có overlay desktop không đưa vào Git/evidence index.

Archive D:/Game cũ không đổi, không tạo lô chuyển mới. **4.183 file D legacy chưa xác minh giữ tại D**, không chuyển bừa; A + 5 dependency HDRP đã có trong baseline Git/LFS. Không tuyên bố toàn legacy sạch hoặc toàn map PASS. Nguồn Blender/media runtime không chuyển ra archive.

Ba branch `work/map-asset-baseline`, `work/runtime-contracts-baseline`, `work/ui-onboarding-review` giữ **`a209c0e3b41a8b55e438aeabf1025b5ef6e7d940`**, không move ref hoặc làm mất commit riêng. Ownership theo [OWNERSHIP](../OWNERSHIP.md), [TICH_HOP](../TICH_HOP.md): người 1 map/art; người 2 runtime/contracts và mọi partial session r107; người 3 UI/nội dung/scene, đề nghị sửa partial qua người 2. Chưa giao baseline như bản đã đạt mọi ca. Bước tiếp: phân tích NAV-01 trong task được duyệt, rồi kiểm lại đúng ca và chỉ khi đạt mới nâng nhánh an toàn. Dừng polish/map/gameplay.

[Audit cuối](final-audit.json) giữ commit, status, GUID/save, capacity và branch thực tế. Không remote, không push; không cần đăng nhập. Người mới **NOT_RUN**.
