from pathlib import Path
import json,subprocess,hashlib
R=Path(__file__).resolve().parents[2];D=R/'Docs/QuanLy'
commits=json.loads((D/'commits-source.json').read_text(encoding='utf-8'));audit=json.loads((D/'git-audit.json').read_text(encoding='utf-8'));arch=json.loads((D/'archive-result.json').read_text(encoding='utf-8'))
source=next(x['commit'] for x in commits if x['category']=='product');entry='Assets/ConRoi/Modules/TichHop/Scenes/ConRoi_Chinh.unity'
text=f'''# Bàn giao chuẩn bị Git — 06/11/2026

**Đã chuẩn bị local; CHƯA SẴN SÀNG PUSH.** Không push, không remote, không repo công khai, không build/ZIP/runner sản phẩm. Dừng tính năng ở r107. Không coi clean checkout đã chạy.

## Root, branch, entry

- Root giữ `D:/ConRoi`. Ban đầu không có `.git` tại root hoặc cha; không có branch/index/remote/lịch sử cũ để reset. Đã tạo repo mới trên `chore/prepare-r107`, LFS local hoạt động.
- Commit tool/policy: `{commits[0]['commit']}`.
- Commit source/scene: **`{source}`**. Commit license/audit bổ sung tra `commit-supplement.json`; commit bàn giao có thể nằm sau. Lấy HEAD thật bằng `git rev-parse HEAD`, không sửa hash baseline.
- Trước: `Assets/Scenes/JourneyPresentation_v48_r107.unity`. Sau: **`{entry}`**, copy bằng AssetDatabase với GUID mới; **byte scene bằng checkpoint**, component/field không tái tạo. Checkpoint r107 và toàn bộ asset baseline giữ nguyên byte.
- Unity **6000.6.2f1**, ForceText2, VisibleMetaFiles. Build Settings đổi entry sang ConRoi_Chinh, r92 giữ disabled; backup trước đổi có trong `EditorBuildSettings.before.asset`. Không nâng manifest/lock/package.

## Archive và dọn thực tế

Lô: **`{arch['archive']}`**. Không ghi đè D:/Game có sẵn.

| Hoạt động | File | Byte |
|---|---:|---:|
| Chuyển capture/output được xác định sang `DuLieu/<old-relative>` | {arch['archiveFiles']} | {arch['archiveBytes']} |
| Chuyển source kiểm/author Test/Tam → Tools/TaiTao/Legacy | {arch['files']-arch['archiveFiles']} | {arch['bytes']-arch['archiveBytes']} |
| Tổng move có mapping | {arch['files']} | {arch['bytes']} |

Staging của toàn lô giữ thêm **{arch['bytes']}byte**, không xóa. Trình tự: snapshot baseline → kiểm process/path/reparse/free-space → copy toàn lô → SHA256/size → native Move-Item → SHA256 lần nữa. `final-audit.json`: **1445/1445 destination và staging khớp**, old path đã chuyển. Báo cáo kiểm hashcase ban đầu so chuỗi hoa/thường sai được giữ `final-audit-superseded-hashcase.json`; bản sửa chỉ chuẩn hóa case, không đổi manifest/baseline. [Mapping](OLD_NEW_PATHS.md), [hoàn nguyên](HOAN_NGUYEN.md).

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
'''
(D/'HANDOFF.md').write_text(text,encoding='utf-8')
for name,heading in [('README.md','# Con Rối — source r107, chuẩn bị nhóm Git'),('BAT_DAU_TAI_DAY.md','## Chuẩn bị nhóm Git — r107 đóng băng')]:
 p=R/name;old=p.read_text(encoding='utf-8-sig')
 prefix=f'''{heading}

Entry nhóm: **[{entry}]({entry})**, Unity **6000.6.2f1**. Copy r107 với GUID scene mới; checkpoint giữ nguyên. Nhánh `chore/prepare-r107`, source commit `{source[:12]}`; **chưa push, không remote**.

[Bàn giao thực tế và phần chặn](Docs/QuanLy/HANDOFF.md) · [Ownership](Docs/QuanLy/OWNERSHIP.md) · [Clone/LFS/profile sạch](Docs/QuanLy/CLONE_VA_BAT_DAU.md) · [QA](Docs/QA/README.md) · [Mapping/hoàn nguyên](Docs/QuanLy/OLD_NEW_PATHS.md).

Onboarding/khám/ghi/P/save đã kiểm qua input tổng hợp trên scene copy; người mới NOT_RUN. LFS objects local kiểm OK; **checkout sạch BLOCKED vì thiếu dung lượng**, còn Assets/Docs chưa phân loại. Không coi đã sẵn sàng push hoặc toàn bộ archive hoàn tất. Không chạy author/derive/bind một lần hoặc dùng profile kỹ thuật đã giải. Dừng tính năng tại r107; các phần dưới là lịch sử.

'''
 if heading not in old[:100]:p.write_text(prefix+old,encoding='utf-8')
for name in ['Docs/PROJECT_GUIDE_AND_HISTORY.md','Docs/BLENDER_UNITY_WORKFLOW.md']:
 p=R/name;old=p.read_text(encoding='utf-8-sig');prefix=f'''## 06/11/2026 — chuẩn bị Git ba người, r107 giữ checkpoint

Entry `{entry}`, AssetDatabase.CopyAsset từ r107, GUID scene mới và byte component giữ nguyên; legacy artR104/clipR105/terrainR107 giữ path/GUID. Source commit `{source}` trên `chore/prepare-r107`. Bảy folder module và ba scene template rỗng, không tuyên bố module runtime độc lập; partial session/UI giao người2 tạm. Không sửa gameplay/schema/controller/cơ cấu, không xuất model mới.

Lô archive `{arch['archive']}`:593capture/output471857740byte,852script sang Tools/TaiTao/Legacy2053301byte; staging/hash giữ, không xóa. Baseline trước move không overwrite. Lô lớn/Docs mixed và checkout sạch BLOCKED vì dung lượng; còn unclassified giữ local, chưa sẵn sàng push. LFS fsck local OK, không blob thường>90MiB đã kiểm. Không remote/push/build/ZIP/runner. QA scene copy bằng controller/UI tổng hợp,0MissingScript/ref, nguồn Blender mở được; không thay người mới NOT_RUN. Tra `Docs/QuanLy/HANDOFF.md` và manifest để biết phạm vi thật, không chạy lại derive/bind/author/integrate một lần. Các mục dưới là lịch sử.

'''
 if not old.startswith(prefix):p.write_text(prefix+old,encoding='utf-8')
print('Wrote UTF-8 handoff and updated root/start/history/workflow; no baseline overwrite.')
