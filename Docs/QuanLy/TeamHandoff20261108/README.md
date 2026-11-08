# Chốt bàn giao nhóm — 08/11/2026

Baseline nhóm **fcbb7e19c094a2a52d99237d03587df16679d579**, biên nhận **55d7f04cc62089ec576efbd2b55d4192aa70f2e5**. Đầu phiên: `D:/ConRoi`, `chore/prepare-r107`, HEAD 55d7f04, tracked tree sạch. [Audit thực tế](audit.json) xác nhận ba nhánh đều ở fcbb7e19, thiếu baseline 0/commit riêng 0, một worktree D, stash rỗng; không chuyển branch/ref hoặc thay việc riêng. Commit chốt đợt này chỉ là docs/công cụ audit; không phải nguồn Unity mới được chạy kiểm.

| Người | Nhánh chính xác | Hướng dẫn/công việc đầu |
|---|---|---|
| 1 | `work/map-asset-baseline` | [Map/asset](../DANG_LAM_NGUOI_1.md): topology và ba góc ảnh còn mở |
| 2 | `work/runtime-contracts-baseline` | [Gameplay/dữ liệu](../DANG_LAM_NGUOI_2.md): API/save/contract theo GDD, một task nhỏ có review |
| 3 | `work/ui-onboarding-review` | [UI/UX/tích hợp](../DANG_LAM_NGUOI_3.md): người mới chơi trước mở rộng onboarding |

[Ownership](../OWNERSHIP.md) ghi một chủ cho file giao nhau: N2 giữ toàn bộ partial session kể cả UI/onboarding, Editor/test/Tools; N3 giữ scene tích hợp/GDD/lịch sử chung. N1/N2 không commit scene chính. Ba task branch giữ baseline, nhận docs mới qua merge có bảo toàn theo [TICH_HOP](../TICH_HOP.md), không reset/force. Trạng thái nhận việc vẫn CHUA_NHAN.

Unity **6000.6.2f1**. Entry giữ `Assets/ConRoi/Modules/TichHop/Scenes/ConRoi_Chinh.unity`. Ba scene thử riêng Map_ThuNghiem/Gameplay_ThuNghiem/GiaoDien_ThuNghiem tồn tại, là template rỗng. Không thay asset/scene/GUID/map/gameplay/save hoặc tạo build/ZIP. [Hợp đồng mô tả source](../HOP_DONG_MODULE.md) chưa phải một refactor runtime.

## Căn cứ kỹ thuật và giới hạn

Tái dùng [kiểm checkout độc lập NAV-01](../NavFix20261108/README.md): nguồn 074caf3 đã kiểm Unity/import/compile/reference và hồi quy input/UI/save-load; product tree từ nguồn đó tới HEAD 55d7f04 không đổi. Không chạy lại Unity trong đợt docs. Input tổng hợp và seed API được báo cáo riêng ở đợt kiểm, không phải người mới. Người mới vẫn **NOT_RUN**, không suy toàn map PASS.

LFS hiện tại **879 file / 797 object / 6.143.855.590 byte**: object tồn tại/đúng size, working file đúng size và không còn pointer. Kiểm hash đầy đủ/fsck tái dùng audit NAV-01 vì không thay binary. Lịch sử mọi ref: blob Git thường lớn nhất **46.173.387 byte**, không blob >100 MiB; không file ignored vẫn tracked. Chưa kiểm upload phía remote.

**Đủ điều kiện local để ba người nhận việc theo ownership và phạm vi kỹ thuật đã kiểm. Chưa hoàn tất bàn giao qua remote.** Chưa có remote/URL đã xác nhận; cần URL repository, quyền tài khoản nhóm và Git LFS/quota đủ cho ít nhất tập object hiện tại (~6,14 GB, chưa tính bandwidth/lịch sử dư). CODEOWNERS chờ username thật. Không tự tạo remote, repo công khai hoặc push.

Danh sách nhánh dự kiến publish sau khi được cung cấp URL và duyệt: `chore/prepare-r107` cùng đúng ba task branch trong bảng. Không có main/master local; không dùng push --all/--mirror, không đưa temp refs lên. Nhánh chore chứa tài liệu điều phối mới; ba nhánh task vẫn fcbb7e19.

## Dữ liệu giữ ngoài baseline Git

4.183 file legacy D chưa xác minh vẫn tại Assets trong project D, không add/move/xóa. Cache/Library, profile/save cá nhân, capture/log dài và snapshot không thuộc source clone. Không phân loại/dọn tiếp trong đợt này.

Archive giữ nguyên: `D:/Game/ConRoi_LuuTru/git-team-20261106-153912-ee41c6` và `D:/Game/ConRoi_LuuTru/git-review-20261107-001945-00b993`. Tra [manifest cũ](../archive-manifest.jsonl), [kết quả](../archive-result.json), [profile](../profiles-final.json) và [review](../Review20261107/README.md). Hoàn nguyên theo old/new/SHA256, chỉ copy khi đích cũ chưa có và hash khớp; không overwrite xung đột. Không thay lô archive hay save gốc.

Riêng bốn ảnh BugModel gốc chưa tracked: cần người tổ chức giao đúng ảnh cho N1 trước so góc; không tuyên bố ảnh có sẵn trong clone. Không coi đây là runtime dependency.
