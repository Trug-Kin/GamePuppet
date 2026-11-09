# Nhận SAVE-EVIDENCE-04 trong scene chung — 09/11/2026

UI/UX & Tích hợp đã review/merge đủ `17b1f6e2 → f73631ae → 597369a5` vào `review/runtime-integration-20261109`. **Commit thực chạy Unity6000.6.2f1: `22acc9611aa8c6d0f86741798b2108d68f9b85d0`**, checkout C:/ConRoi_CheckoutTest; scene `Assets/ConRoi/Modules/TichHop/Scenes/ConRoi_Chinh.unity`. Không cần wiring mới, không sửa sản phẩm. Commit báo cáo sau phiên chỉ là tài liệu.

**PASS kỹ thuật ba acceptance trong scene chung:** bốn vật chứng/F6 evidence:null bị từ chối trước thay state, giữ toàn bộ data/tiến trình/TrialRecord và bytes file; thao tác tiếp rồi Load hợp lệ/F5/F6 phục hồi đúng; evidence:[] New Game vẫn tải được. INT-SAVE-NULL-LIST-01 được xác nhận sửa ở phạm vi này. Dùng input Windows tổng hợp và bản sao save UI cũ để chuẩn bị bốn vật chứng; không phải tuyến chơi tự nhiên mới hoặc playtest người mới. Có một lượt mất focus trước Esc được ghi gián đoạn, lượt kiểm lại đạt.

**Chỉ đề xuất nguồn `22acc961` làm ứng viên baseline tích hợp, chưa chốt/cập nhật các nhánh nhóm.** Probe bổ sung bên giao NOT_RUN; UI1080p đầy đủ trên bản sửa/người mới NOT_RUN; Load feedback/P-close OPEN riêng. Các PASS của 78 API/54 hồi quy bên giao không được suy thành PASS scene chung. D:/ConRoi, thay đổi riêng, legacy/save/archive bảo toàn; chưa push/merge main.

[Báo cáo nhận bản sửa](../QA/Tasks/N3/SAVE-EVIDENCE-04-INTEGRATION/README.md), [kết quả scene chung](../QA/Tasks/N3/SAVE-EVIDENCE-04-INTEGRATION/integration-results.json). Báo cáo RUNTIME-INTEGRATION-01 dưới đây giữ nguyên kết quả lỗi lịch sử trước sửa.

---

# Nhận kiểm năm gói runtime — 09/11/2026

UI/UX & Tích hợp đã review đủ22 commit từ baseline0d878e3b đến91835c80. Nhận tại `review/runtime-integration-20261109`, checkout riêng C:/ConRoi_CheckoutTest, merge d765ed92 giữ lịch sử UI riêng. **HEAD chạy Unity6000.6.2f1: c18524a3bc07f74fab16244f2d55c6888afb9284**; commit báo cáo sau đó không được gọi là nguồn chạy lại. Scene `Assets/ConRoi/Modules/TichHop/Scenes/ConRoi_Chinh.unity`. Không cần wiring mới.

Tuyến ghi→mục tiêu→bàn→rời/quay lại→save/load đạt kỹ thuật phạm vi đại diện bằng input Windows tổng hợp. **Tổng thể REVIEW_REQUIRED:** phát hiện mới `evidence:null` được F6 nhận và thay state4→0, đã ghi INT-SAVE-NULL-LIST-01 gửi Gameplay & Dữ liệu; không sửa session partial. UI1080p có kiểm trang đại diện, còn Load feedback/P-close OPEN riêng; người mới NOT_RUN. Chưa chốt lại baseline ba nhánh hoặc merge main; D:/ConRoi cùng deletion/untracked giữ nguyên.

[Báo cáo và yêu cầu tái hiện](../QA/Tasks/N3/RUNTIME-INTEGRATION-01/README.md), [kết quả riêng scene chung](../QA/Tasks/N3/RUNTIME-INTEGRATION-01/integration-results.json). Các trạng thái/baseline cũ dưới đây là lịch sử, không phải kết luận năm gói mới.

---

# Chốt nhận việc 08/11/2026 — fcbb7e19 / biên nhận 55d7f04

Kết quả hiện hành: [bàn giao nhóm](TeamHandoff20261108/README.md), [audit Git/LFS](TeamHandoff20261108/audit.json).
Ba nhánh task đều chứa baseline fcbb7e19, không có commit riêng tại lúc kiểm; không đổi ref trong đợt này. Những thông báo BLOCKED/a209c0e phía dưới thuộc lịch sử, đã được biên nhận NAV-01 thay thế.

<!-- nav01-team-receipt-20261108 -->
## Baseline nhóm đã chốt — biên nhận 08/11/2026

- **Nguồn đã chạy kiểm trong checkout C: `074caf3dd3da4107b4fc9ac1c089b20216b4705b`.**
- **Commit báo cáo chính và baseline nhóm: `fcbb7e19c094a2a52d99237d03587df16679d579`.** Commit này chỉ thêm tài liệu/công cụ kiểm sau nguồn; product tree Assets/Packages/ProjectSettings/Source khớp nguồn đã kiểm.
- Ba nhánh `work/map-asset-baseline`, `work/runtime-contracts-baseline`, `work/ui-onboarding-review` **đã fast-forward từ a209c0e tới fcbb7e19** bằng transaction expected-old. Trước cập nhật: không commit riêng, không worktree task; không reset/force hoặc thay đổi stash. [Biên nhận hash đầy đủ](NavFix20261108/team-baseline.json).
- Nhóm đủ điều kiện nhận ba task theo ownership/DANG_LAM trên baseline này trong phạm vi kỹ thuật đã kiểm. Partial session vẫn do người 2 giữ trọn. NAV-01 PASS; người mới NOT_RUN, 4.183 file legacy chưa xác minh giữ nguyên; không suy toàn map PASS.
- Checkout thử vẫn detached ở 074caf3, độc lập với project D. Commit biên nhận sau báo cáo chỉ là metadata, không phải một bản sản phẩm mới được chạy kiểm. Không remote/push; không cần đăng nhập.

---

<!-- nav01-fixed-20261108 -->
## NAV-01 đã kiểm đạt — 08/11/2026

**Commit nguồn đã kiểm: `074caf3dd3da4107b4fc9ac1c089b20216b4705b`.** Sửa tối thiểu vòng đời focus trong hai script r107. Trên checkout độc lập `C:/ConRoi_CheckoutTest`, Unity **6000.6.2f1**, entry `Assets/ConRoi/Modules/TichHop/Scenes/ConRoi_Chinh.unity`: compile/reference đạt, NAV-01 và hồi quy UI/input/save-load **PASS kỹ thuật**. P hoạt động sau nhập đúng/sai/dở, còn được chặn đúng khi ô nhập vẫn focus. Không thay map, luật, schema, GUID hoặc save gốc.

[Báo cáo, fixture và giới hạn](NavFix20261108/README.md): input tổng hợp; letter/slide/phone và 2 quan sát floor được seed API có ghi rõ, không phải người mới. **Người mới NOT_RUN; 4.183 file legacy chưa xác minh giữ nguyên.** 879 file LFS/797 object materialized đủ, 11.458 GUID meta khớp; runtime không phụ thuộc cache/archive ngoài repo. Library C độc lập, import tăng dần commit sửa. State/diff/profile được snapshot, Editor đã đóng ở Edit sạch không lưu scene.

NAV-01 thay thế trạng thái BLOCKED_UI_REGRESSION 07/11; các FAIL cũ bên dưới được giữ như lịch sử. Commit báo cáo và biên nhận fast-forward ba task branch được ghi riêng sau khi có hash thực tế. Ownership vẫn giữ: người 2 sở hữu toàn bộ partial session, người 3 không đồng thời sửa các file đó. Không push/remote/build/ZIP; chưa tuyên bố toàn map/legacy PASS.

---

<!-- checkout-final-a789222-20261107 -->
## Kiểm baseline a789222 — kết quả hiện hành 07/11/2026

**Đã kiểm thực tế `a789222286f1a56db5b6cc21e1ffb5a0e7542be4` tại `C:/ConRoi_CheckoutTest`, Unity 6000.6.2f1.** Git/LFS/import/reference PASS: 878 file LFS materialized, 0 Missing Script/0 reference hỏng, 4.582 dependency. Library C độc lập tạo mới ở lần checkout đầu, import tăng dần baseline sửa; không dùng chung Library D. Entry `Assets/ConRoi/Modules/TichHop/Scenes/ConRoi_Chinh.unity`.

**Tổng thể `BLOCKED_UI_REGRESSION`**, không còn chặn vì dung lượng/quyền điều khiển. Controller Start → dấu sàn → khám/ghi/R/P → điện thoại, 1080p, save/load và skip/revisit đạt bằng input tổng hợp. Navigation mật mã/đối chiếu/kết luận được kiểm với thư cấp API trong profile fixture riêng. Ca **nhập mật mã → Trở lại → P không mở hồ sơ** chưa đạt; Esc → Ghi chép là đường truy cập được, không thay cho PASS. Chưa xác nhận nguyên nhân cuối, không sửa sản phẩm trong đợt này.

Đã bảo toàn diff/state trước Play stop/sync, quarantine 11 file Unity tự sinh, sync từ 8ea91d4 qua Git/LFS tới a789222; không ghi đè snapshot. Editor đã dừng/đóng khi Edit sạch, không save runtime scene. ProjectSettings ghi lúc thoát có snapshot và diff nội dung rỗng; source D không đổi sản phẩm. Lần intro tự đổi ở phiên cũ vẫn **chưa rõ nguồn input**, không gọi là lỗi gameplay.

Ba task branch **giữ a209c0e**, chưa nâng vì còn NAV-01; ownership không đổi, người 2 giữ toàn bộ partial session r107. **4.183 file D chưa xác minh giữ nguyên**. Save gốc/archive/GUID bảo toàn, profile kỹ thuật riêng ở C. Người mới **NOT_RUN**. Không sửa map/gameplay, không remote/push/build/ZIP/runner runtime mới.

Xem [báo cáo cuối](CheckoutFinal20261107/README.md), [audit](CheckoutFinal20261107/final-audit.json) và [manifest/index bằng chứng](CheckoutFinal20261107/evidence-index.json). Commit sau a789222 chỉ là tài liệu/công cụ kiểm; không gọi commit tài liệu là đã chạy sản phẩm. Các phần bên dưới là lịch sử, không thay kết quả hiện hành này.

---

## Checkout độc lập trên C — cập nhật 07/11/2026

Source giữ `D:/ConRoi`, nhánh `chore/prepare-r107`. **Baseline source hiện `a789222286f1a56db5b6cc21e1ffb5a0e7542be4`**; baseline đã mở lần đầu trong checkout là `8ea91d4d64236f5d8507f216112997402d9b46f6`. Commit tài liệu sau hai mốc này không được gọi là đã kiểm sản phẩm.

[Kết quả hiện hành](CheckoutTest20261107/README.md) thay thế tình trạng BLOCKED_CAPACITY của lần trước: đã tạo checkout thực tại **`C:/ConRoi_CheckoutTest`**, Git objects độc lập, không remote/alternates, không copy file untracked và không dùng chung Library. Trước import C còn 43.972.919.296 byte. LFS đã kiểm hash/materialize **877 file / 795 object / 6.136.024.587 byte**. Library thực được tạo mới bởi Unity **6000.6.2f1**, không copy từ D.

Đã commit đủ **11.326 file A / 3.562.022.051 byte** cùng meta. Sau import phát hiện thêm 5 file D thực sự thuộc closure GUID cấu hình HDRP có sẵn: hai asset, hai meta và folder meta; đã bổ sung có manifest, không đổi pipeline. **D chưa xác minh còn 4.183 file, giữ nguyên tại D**, không archive bừa.

Có lỗi Git thật trong lần checkout đầu: scene legacy nhị phân `Assets/_Recovery/0 (2).unity` bị quy tắc `*.unity text` làm mất 10 byte khi normalize CRLF. Commit `a789222` sửa bằng **LFS đúng đường dẫn**, object hash `00eac743343893f614abbb6cbb1ddc286565d95f330e4913ced30345384a7939`, khớp bản gốc 7.619.172 byte. Không sửa/reserialize checkpoint. Scene entry và prefab text khác vẫn review bình thường. Không rewrite lịch sử; `8ea91d4` giữ làm bằng chứng chưa hoàn chỉnh.

Trên checkout `8ea91d4`: compile C#/Editor đạt; entry **`Assets/ConRoi/Modules/TichHop/Scenes/ConRoi_Chinh.unity`** mở được, **32.958 component, 0 Missing Script, 0 reference hỏng**, AssetDatabase có đủ 4.582 dependency. GUID meta A khớp và entry không đổi sau chuẩn hóa xuống dòng. Có ảnh render thực **1920×1080**. Những kết quả này không thay cho kiểm UI hoặc xác nhận baseline sửa `a789222` đã chạy.

**Tổng thể vẫn BLOCKED — cần xác nhận quyền điều khiển Editor và hoàn tất kiểm baseline sửa.** Công cụ input tổng hợp chưa xác nhận được Start/luồng UI; intro về sau đã đóng nhưng nguồn thao tác chưa phân biệt chắc. Đã hỏi người dùng có đang điều khiển Unity; chưa có câu trả lời tại lúc ghi. Không gửi thêm input, không dừng Play/discard/lưu runtime vào scene. State live và các thay đổi Unity tự sinh đã snapshot, xem `current-status.json`. Hồ sơ CLI riêng tại `C:/ConRoi_CheckoutTest/Test/PlaytestProfiles/Clean_20261107`; không ghi vào save gốc. Chưa có bằng chứng F5/F6 thành công trong checkout.

Các ca **E/khám/ghi/R/P, save/load trước ghi, skip rồi quay lại, điện thoại, cipher/compare/conclusions qua navigation** còn **NOT_RUN/INCOMPLETE**. Người mới **NOT_RUN**. Lỗi fixture/giới hạn bằng chứng được giữ trong `FIXTURE_ATTEMPTS.md`; không chuyển FAIL cũ thành PASS. Không tuyên bố toàn legacy/map PASS.

Tiếp tục theo thứ tự: xác nhận Editor checkout không được người dùng thao tác → bảo toàn và đóng Play có kiểm soát → đưa checkout tới `a789222` bằng Git/LFS, không copy bù untracked và không ghi đè thay đổi tự sinh đã snapshot → mở lại entry/kiểm reference và các ca UI còn thiếu → chỉ sau đạt mới cập nhật ba task branch nếu bảo toàn commit riêng. Ba task branch vẫn ở **`a209c0e`**; chưa giao clone để sửa độc lập. Ownership không đổi theo OWNERSHIP/TICH_HOP; người 2 tạm sở hữu toàn bộ InvestigationSession partial, người 3 không cùng sửa file đó.

Không tạo thêm archive/build/ZIP/runner, không xóa dữ liệu, không tạo remote hoặc push. Log import dài và cache/profile/snapshot giữ ngoài Git trong checkout C; manifest và báo cáo nhỏ ở đây. Dữ liệu archive cũ tại D:/Game giữ nguyên. Chưa cần thông tin remote/login.

## Rà soát bàn giao 07/11/2026 — checkout vẫn BLOCKED

Baseline sản phẩm được rà: **`90147d9382349575c5bc0385f7405525cad727cd`**, nhánh `chore/prepare-r107`. Đầu phiên không có thay đổi tracked; không remote, không push, không đổi sản phẩm. Commit bổ sung sau mốc này chỉ chứa tài liệu/công cụ kiểm. Entry giữ `Assets/ConRoi/Modules/TichHop/Scenes/ConRoi_Chinh.unity`, Unity **6000.6.2f1**. Không tạo r108, build, ZIP hay runner sản phẩm.

[Kết quả chi tiết](Review20261107/README.md) là cập nhật hiện hành; số liệu phía dưới thuộc lần bàn giao trước. LFS máy gốc: **296 file / 217 object / 4.503.946.222 byte**, hash/size và materialization đều đúng. Chưa chứng minh clone chạy được.

Checkout độc lập hiện cần ước tính **27.709.685.653 byte (27,71 GB)**: working tree 6.223.984.553 + LFS 4.503.946.222 + Git 306.433.037 + Library mới dự trù 12.380.354.545 + dự phòng 4.294.967.296. Library dựa trên cache hiện có, chưa phải số đo import mới. C trống 10.197.442.560; D 2.464.395.264; G 9.687.568.384 byte. C thiếu 17.512.243.093; D thiếu 25.245.290.389; G thiếu 18.022.117.269 byte. G chưa xác nhận vị trí/vai trò, không sử dụng. Không dùng ổ ngoài/cloud.

Legacy 15.514 file đã phân loại tĩnh: **A 11.326 file / 3.562.022.051 byte** có consumer cụ thể (gồm Editor/historical tools); **D 4.188 file / 5.402.343.180 byte** chưa xác minh, giữ nguyên. A vẫn UNTRACKED: chưa được coi baseline đủ cho toàn bộ công cụ. Chuỗi GUID của entry và tập compiled source không thiếu file Git theo kiểm tĩnh. Chưa có AssetDatabase/import từ checkout sạch. Báo cáo refined thay thế phân loại raw vốn nhận quá rộng literal `Assets/`. Không suy không dùng từ việc vắng trong scene.

Nếu giữ toàn bộ A để bàn giao Editor, ngân sách bảo thủ tăng tối đa hai lần 3.562.022.051 byte (working tree + object), thành **34.833.729.755 byte**, chưa khử trùng LFS và nén Git. Cần vị trí local được xác nhận có **khoảng 36 GB trống** để bổ sung A và kiểm; sau chọn tập source sẽ đo lại. Không stage hàng GB khi D chỉ còn khoảng 2,46 GB. Nguồn B: Source/license đã tracked; editable trong Assets chưa xác minh recipe tiếp tục D. Không chuyển bất kỳ Assets/meta/GUID nào trong lần rà này.

852 script Tools: **852 hash giữ nguyên**. Năm cảnh báo lexical đều là từ Python `from`, không phải type C# bị mất. Kiểm tĩnh không thay cho compile mới. Scene/prefab text, source/media game và license giữ Git/LFS; cache, profile cá nhân, capture/build/log không thuộc source clone. Docs/Reviews còn trộn nguồn, giữ UNCLASSIFIED.

Lô C mới `D:/Game/ConRoi_LuuTru/git-review-20261107-001945-00b993`: **8 file / 2.108.862 byte** ảnh kiểm GitPreparation và log Editor đã dừng; staging/hash/move/hash được kiểm lại. Manifest ở bộ review; hoàn nguyên bằng old/new/SHA256, copy về khi old vắng, không ghi đè xung đột. Lô cũ `git-team-20261106-153912-ee41c6` giữ nguyên. Chuyển cùng D **không tăng dung lượng trống**; staging mới còn dùng thêm 2.108.862 byte. Không chuyển cache qua junction, runtime media hoặc save gốc.

**Checkout sạch BLOCKED_CAPACITY**; chưa tạo checkout/Library, chưa chạy compile/import/reference/onboarding/save thử trên checkout. Các ca E/ghi/R/P, save trước ghi, skip tutorial, điện thoại, 1080p và mật mã/đối chiếu/kết luận ở checkout đều **NOT_RUN**. Không mượn PASS máy gốc; người mới vẫn NOT_RUN. Không cần remote/login ở bước này.

Ba task branch giữ `a209c0e3b41a8b55e438aeabf1025b5ef6e7d940`, không tiến tới baseline chưa được clean-test. Ownership không đổi: người 1 Map/art/source; người 2 runtime/contracts và toàn bộ InvestigationSession partial tạm thời; người 3 UI/nội dung/scene tích hợp, gửi đề nghị qua người 2 nếu chạm partial dùng chung. Chưa đủ điều kiện tách clone để sửa độc lập. Thứ tự: xác nhận vị trí đủ chỗ → bổ sung A có kiểm → checkout/LFS/import/test → chốt commit đã kiểm → cập nhật task branch nếu không mất việc riêng. Trong lúc chờ chỉ đọc inventory/contracts và nhận task; không polish map/gameplay. CODEOWNERS chờ username thật.

# Bàn giao chuẩn bị Git — 06/11/2026

**Đã chuẩn bị local; CHƯA SẴN SÀNG PUSH.** Không push, không remote, không repo công khai, không build/ZIP/runner sản phẩm. Dừng tính năng ở r107. Không coi clean checkout đã chạy.

## Root, branch, entry

- Root giữ `D:/ConRoi`. Ban đầu không có `.git` tại root hoặc cha; không có branch/index/remote/lịch sử cũ để reset. Đã tạo repo mới trên `chore/prepare-r107`, LFS local hoạt động.
- Commit tool/policy: `43542326289c1c29fc3d794172603cd9978178ec`.
- Commit source/scene: **`40bcb67035b73776c760b0d84d37dda12ab1fe67`**. Commit license/audit bổ sung tra `commit-supplement.json`; commit bàn giao có thể nằm sau. Lấy HEAD thật bằng `git rev-parse HEAD`, không sửa hash baseline.
- Trước: `Assets/Scenes/JourneyPresentation_v48_r107.unity`. Sau: **`Assets/ConRoi/Modules/TichHop/Scenes/ConRoi_Chinh.unity`**, copy bằng AssetDatabase với GUID mới; **byte scene bằng checkpoint**, component/field không tái tạo. Checkpoint r107 và toàn bộ asset baseline giữ nguyên byte.
- Unity **6000.6.2f1**, ForceText2, VisibleMetaFiles. Build Settings đổi entry sang ConRoi_Chinh, r92 giữ disabled; backup trước đổi có trong `EditorBuildSettings.before.asset`. Không nâng manifest/lock/package.

## Archive và dọn thực tế

Lô: **`D:\Game\ConRoi_LuuTru\git-team-20261106-153912-ee41c6`**. Không ghi đè D:/Game có sẵn.

| Hoạt động | File | Byte |
|---|---:|---:|
| Chuyển capture/output được xác định sang `DuLieu/<old-relative>` | 593 | 471857740 |
| Chuyển source kiểm/author Test/Tam → Tools/TaiTao/Legacy | 852 | 2053301 |
| Tổng move có mapping | 1445 | 473911041 |

Staging của toàn lô giữ thêm **473911041byte**, không xóa. Trình tự: snapshot baseline → kiểm process/path/reparse/free-space → copy toàn lô → SHA256/size → native Move-Item → SHA256 lần nữa. `final-audit.json`: **1445/1445 destination và staging khớp**, old path đã chuyển. Báo cáo kiểm hashcase ban đầu so chuỗi hoa/thường sai được giữ `final-audit-superseded-hashcase.json`; bản sửa chỉ chuẩn hóa case, không đổi manifest/baseline. [Mapping](OLD_NEW_PATHS.md), [hoàn nguyên](HOAN_NGUYEN.md).

Chưa chuyển tất cả dữ liệu không push: trước dọn Test~63,3GB, Docs~29,9GB, cache~12,4GB; D chỉ còn~7,9GB lúc đầu, sau LFS~2,47GB. Archive cùng ổ D không giải phóng chỗ; staging cần thêm dung lượng. Các lô lớn **BLOCKED_CAPACITY**. Docs/Reviews trộn source/recipe/output **UNCLASSIFIED**, không bulk archive. Không chuyển checkpoint chỉ vì số cũ. `Library` là junction tới `.build-cache/r92-unity-library`, không follow/move/xóa target; cache giữ local ignored. Profile người dùng còn lại giữ nguyên local/ignored, chưa chuyển do giới hạn lô; không migration.

## Tập Git, LFS, phần giữ local

Stage theo `git-source-paths.txt` và dependency closure: scene tích hợp + checkpoint r101–r107, toàn C#/Editor/test, shader/prefab/Resources/StreamingAssets cần thiết, cấu hình và dependency, **toàn Source**, tool/license/GDD, báo cáo nhỏ và workflow nhóm. Không stage cả repo. `Assets/Art/.../R104`, clipR105 và terrainR107 giữ path/GUID; không move art theo revision.

Binary Blender/FBX/texture/audio/video game, DLL và từng mesh lớn cụ thể dùng LFS; scene/prefab text. **`git lfs fsck --objects`: OK**, object thật tồn tại. Không có blob thường>90MiB trong lịch sử local đã kiểm. `.asset` không bị LFS/ignore hàng loạt; chỉ20mesh lớn liệt kê rõ. Ignore cache/output/profile cá nhân theo root, không ignore Assets/Source/.meta/game media. Tracked bị ignore: **0** tại audit. Không `git rm --cached` vì repo mới không có output tracked.

Khoảng **15.514 file Assets / 8,96GB** legacy chưa được stage, giữ nguyên **UNCLASSIFIED**; Docs/Reviews/input ảnh nguồn chưa phân loại giữ local. Vì vậy commit không được gọi là full project/archive hoàn tất. Compile ở máy gốc chưa chứng minh checkout thiếu phần này vẫn đầy đủ; clean check bắt buộc trước publish. Audit string/path trong `static-dependencies.json` giữ Editor/test C# dù không reference trực tiếp scene; không tạo runtime path trỏ archive. Package local/URL:0. Recipe lịch sử có path tuyệt đối/output cũ giữ **PROVISIONAL**, không chạy author/derive/bind một lần. Thư viện Python đi kèm được giữ, không tự nâng dependency.

## Kiểm và phạm vi

- Editor ban đầu Con Rối không chạy; sau mở xác nhận Edit/dirtyFalse. Không tranh input người dùng. Kiểm bằng profile mới `GitPreparation_20261106`; sau xong giữ state kỹ thuật riêng, dừng Play và chỉ exit khi scene không dirty; project Unity khác không bị đóng. Không save runtime vào scene.
- Scene copy: controller từ spawn → cửa → gác → E/raycast dấu sàn (~2,247m), UI xoay/phóng, Ghi quan sát, R/P, objective điện thoại, F5/F6 đúng namespace/profile: **PASS kỹ thuật tổng hợp**. Không dùng API mở trang/Observe/Save/Load. `Docs/QA/GitPreparation` có log/ảnh output local. Người thật sau r107 vẫn **NOT_RUN**.
- Compile trong Editor thành công; **0MissingScript/0broken reference**, 32962component trong scene đang Play; log `reference-audit.txt`. Không tuyên bố kiểm mọi checkpoint/tool historical.
- Blender5.2.0LTS mở terrainR107 và clipR105: không sửa/lưu/export, không thiếu external texture. Unity vẫn dùng FBX/clip shared; không copy media sang archive để runtime tải ngoài repo.
- Baseline27101file: không mất file, chỉ `.gitignore` và BuildSettings đổi trước tài liệu cuối; audit cuối bổ sung tài liệu có chủ ý. Không ghi đè baseline để biến thay đổi ngoài phạm vi thành bảo toàn. Đối chiếu asset/source/schema không thay.
- **Checkout sạch: BLOCKED**, chưa tạo/mở/import. Working tree chuẩn bị~6,22GB chưa tính Library; D còn~2,47GB. Không copy Library hoặc dùng cache cũ giả làm clean checkout; không tạo bộ ZIP/build để thay. Cần ổ đủ chỗ hoặc người dùng cung cấp thêm dung lượng trước kiểm clone/LFS/NewGame/UI/save.

## Ba người và bước đầu

[OWNERSHIP](OWNERSHIP.md), [TICH_HOP](TICH_HOP.md), [CLONE_VA_BAT_DAU](CLONE_VA_BAT_DAU.md). Bảy module mới có folder/meta, ba scene thử rỗng cho Map/Gameplay/GiaoDien; chưa move legacy risky. Partial session/UI r107 dùng chung state nên **người2 giữ trọn bộ tạm**; người3 gửi đề nghị/patch, không cùng sửa file. Không refactor adapter/gameplay/save trong dọn.

Ba nhánh/task/prompt trong DANG_LAM_NGUOI_1/2/3, trạng thái CHUA_NHAN; nhánh sẽ tạo local từ source commit đã chốt và ghi kết quả riêng. Chỉ người3 chạm scene tích hợp/lịch sử chung. Username cộng tác viên chưa có: CODEOWNERS **PENDING_IDENTITY**, không ghi tài khoản giả hoặc tự thêm collaborator.

## Chặn trước push

1. Có dung lượng để hoàn tất archive các lô lớn và mở checkout sạch bằng Unity đúng version, LFS object thực.
2. Phân loại15.514Assets và nguồn/recipe Docs mixed; bổ sung dependency/tool/source texture cần thiết, không xóa để làm status sạch.
3. Clean checkout compile/NewGame/UI/save riêng; hiện **NOT_RUN**, không kế thừa PASS máy gốc.
4. Nhóm duyệt tài khoản/remote/quota LFS rồi mới authorize publish. Đợt này không remote/push hoặc đăng nhập người khác.

Không tiếp tục polish map/art/gameplay để lấp phần bị chặn. Đã làm các bước có căn cứ và bảo toàn; giữ phần còn lại nguyên trạng để tiếp tục sau khi có dung lượng/kiểm checkout.

## Đính chính phạm vi profile và checkpoint task

Manifest thực tế có **1 profile PT001r106 đã chuyển** trong lô593file, không phải mọi profile vẫn ở đường cũ. Profile đó được copy/hash rồi move nguyên byte sang archive, có staging cùng SHA256 và không migration/ghi lại; tra `profiles-final.json` cùng manifest để hoàn nguyên khi cần. Profile mới GitPreparation chỉ phục vụ kiểm kỹ thuật, không dùng cho người mới. Báo cáo profile đầu kiểm path cũ thấy absent là do move có chủ ý, không phải mất save; audit đã được sửa để resolve mapping, baseline không đổi.

Commit bàn giao ownership/index: `a209c0e3b41a8b55e438aeabf1025b5ef6e7d940`. Ba nhánh task đã tạo local từ đúng commit này, chưa checkout/worktree hoặc push. Commit sau chỉ bổ sung đính chính/audit, không đổi sản phẩm. Các username cộng tác viên vẫn chưa được cung cấp.
