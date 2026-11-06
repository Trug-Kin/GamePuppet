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
