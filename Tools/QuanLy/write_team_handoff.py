"""Write documentation only for the verified fcbb7e19 team handoff."""
from pathlib import Path

root = Path(__file__).resolve().parents[2]
docs = root / 'Docs/QuanLy'
if (docs / 'HANDOFF.md').read_text(encoding='utf-8-sig').startswith('# Chốt nhận việc 08/11/2026'):
    raise SystemExit('Already written; review/edit current docs instead of overwriting task records.')

def write(name, text):
    (docs / name).write_text(text.strip()+'\n', encoding='utf-8')

common = '''
Unity **6000.6.2f1**. Baseline nhận việc **fcbb7e19c094a2a52d99237d03587df16679d579**; biên nhận **55d7f04**.
Entry: `Assets/ConRoi/Modules/TichHop/Scenes/ConRoi_Chinh.unity`, chỉ người 3 được sửa.
Checkpoint `Assets/Scenes/JourneyPresentation_v48_r107.unity` giữ nguyên. Scene thử dưới đây là **template rỗng**, không phải bản game độc lập.
Mỗi người dùng checkout/Library riêng; không dùng chung project đang mở. Đọc [quyền sở hữu](OWNERSHIP.md), [tích hợp](TICH_HOP.md) và [hợp đồng hiện hành](HOP_DONG_MODULE.md).
Không đổi GUID/meta, schema/save ID, pipeline/packages; không chạy author/derive/bind một lần. Không dọn 4.183 legacy hoặc archive.
'''

write('DANG_LAM_NGUOI_1.md', '''# Người 1 — Map/asset

Trạng thái: **CHUA_NHAN**. Nhánh chính xác: `work/map-asset-baseline`.
'''+common+'''
Được sửa trong task đã khai báo: `Assets/ConRoi/Modules/Map/`, `Assets/ConRoi/Modules/NgheThuat/`, legacy `Assets/Art/`, `Assets/Model/`, `Assets/Blender/`, `Source/` và meta tương ứng. Báo cáo riêng: `Docs/QA/Tasks/N1/`.
Không sửa scene tích hợp/checkpoint, GDD/lịch sử chung, `Assets/Scripts/`, Packages/ProjectSettings hoặc dữ liệu người dùng. Tool asset cần chuyển quyền từng file từ người 2 trước khi sửa.
Scene thử: `Assets/ConRoi/Modules/Map/Scenes/Map_ThuNghiem.unity`.

## Việc đầu tiên

Đọc [MAP_REPAIR](../QA/investigation_r107/MAP_REPAIR.md). Lập bảng topology còn mở và tái hiện ba góc ảnh **105506/105520/105534**. Góc 105627 đã sửa winding có phạm vi; không gọi ba góc bổ sung là matched trước/sau. Ảnh gốc tại `D:/ConRoi/Test/BugModel` chưa tracked: nhờ người tổ chức cấp đúng bốn ảnh theo index, không giả định có trong clone. Tài liệu/bằng chứng khác tra manifest archive trong HANDOFF.

Nguồn hiện hành `Source/Blender/JourneyV48/R107/Terrain_R107_Editable.blend`; FBX `Assets/Art/Environment/JourneyV48/TerrainR107/TerrainR107_Final.fbx`.
Ưu tiên phân biệt mặt thiếu/winding, topology rời, collider và góc camera. Giữ FAIL source-delivery-gate và 8 tam giác rất nhỏ như vấn đề cần phân tích; không tự weld terrain theo tiêu chí closed-prop. Không san toàn map hoặc đổi nước/nền chợ đã chốt.

Hoàn tất task đầu khi có từng mục: camera/player, object/GUID, nguồn, ảnh, collider, mức ảnh hưởng và đề xuất sửa có giới hạn; ghi chỗ chưa tái hiện được. Chỉ sửa sản phẩm trong task tiếp được duyệt theo bằng chứng.
Bàn giao commit nhỏ + báo cáo riêng + hướng dẫn đặt asset cho người 3; nếu sửa geometry ở task sau phải theo Blender editable → FBX → Unity, giữ GUID và kiểm collider/góc ảnh chịu ảnh hưởng. Không tự commit scene chính hoặc tuyên bố toàn map PASS.

Prompt nhận việc: “Trên work/map-asset-baseline từ fcbb7e19, triage topology và ba góc ảnh chưa khớp theo MAP_REPAIR; ghi đúng object/camera/source/collider, không mở rộng sửa map khi chưa có lỗi xác nhận.”

## Điền khi nhận

- Người nhận / ngày:
- HEAD thực tế / file sẽ chạm:
- Chuyển quyền file giao nhau đã thống nhất:
- Kiểm và bằng chứng:
- Commit kết quả / giới hạn / người nhận bàn giao:
''')

write('DANG_LAM_NGUOI_2.md', '''# Người 2 — Gameplay/dữ liệu/contracts

Trạng thái: **CHUA_NHAN**. Nhánh chính xác: `work/runtime-contracts-baseline`.
'''+common+'''
Được sửa theo task: `Assets/ConRoi/Modules/Gameplay/`, `Contracts/`, `Assets/Scripts/` (kể cả toàn bộ partial `InvestigationV36R107`), công cụ compile/Editor/test dưới `Tools/`. Báo cáo riêng `Docs/QA/Tasks/N2/`; hợp đồng `Docs/QuanLy/HOP_DONG_MODULE.md`.
Không sửa scene tích hợp/checkpoint, GDD, map/art, lịch sử chung hoặc profile người dùng. Tool asset mặc định vẫn người 2 giữ file; chỉ giao người 1 sau khi ghi chuyển quyền, không hai người sửa cùng lúc.
Scene thử: `Assets/ConRoi/Modules/Gameplay/Scenes/Gameplay_ThuNghiem.unity`.

## Việc đầu tiên

Đối chiếu API/save/observation hiện hành với [GDD v3.6](../CON_ROI_GDD_v3.6_CHOT_TRIEN_KHAI.md) và HOP_DONG_MODULE. Hoàn thiện bảng producer/consumer, ID, trạng thái modal, dữ liệu lưu và acceptance cho một task gameplay/dữ liệu cụ thể trong tuyến **xưởng → Hà → xưởng**. Phân biệt đã có, thiếu được xác nhận và đề xuất; chưa tự triển khai tính năng mới hoặc refactor partial.

Hoàn tất task đầu khi hợp đồng bám mã thực, có ca kiểm/đầu vào/đầu ra và một nhiệm vụ nhỏ được người 3 review theo GDD. Không đổi schema1/namespace `conroi.investigation.v36.r107`, luật hồi hữu hạn hoặc sáu diễn biến dùng chung cơ cấu; seek không mở quan sát chưa xem. Giữ NAV-01 focus/modal đã đạt. Không chuyển lại 852 script lịch sử hoặc bỏ type còn consumer.

Bàn giao commit code riêng với docs và ca kiểm chịu ảnh hưởng, fixture/API ghi riêng với UI, yêu cầu wiring cho người 3. UI đề xuất chạm session partial do người 3 gửi phải được người 2 tích hợp; không tách module giả bằng đổi namespace/serialized field.
Prompt nhận việc: “Trên work/runtime-contracts-baseline từ fcbb7e19, xác minh và hoàn thiện hợp đồng API/save theo GDD v3.6; đề xuất một task nhỏ có acceptance trong tuyến hiện hành, giữ luật/schema và ownership toàn bộ partial session.”

## Điền khi nhận

- Người nhận / ngày:
- HEAD thực tế / file sẽ chạm:
- Task được review / file chuyển quyền:
- Kiểm và bằng chứng:
- Commit kết quả / giới hạn / người nhận bàn giao:
''')

write('DANG_LAM_NGUOI_3.md', '''# Người 3 — UI/UX/nội dung/tích hợp

Trạng thái: **CHUA_NHAN**. Nhánh chính xác: `work/ui-onboarding-review`.
'''+common+'''
Được sửa theo task: `Assets/ConRoi/Modules/GiaoDien/`, `NoiDung/`, `TichHop/`; duy nhất người 3 sửa scene tích hợp, GDD/tài liệu chung và Docs/QA. GDD chỉ đổi sau quyết định thiết kế có review; task đầu không mở rộng nội dung.
Không sửa map/art, save/profile gốc, runtime/contracts hoặc bất kỳ partial `Assets/Scripts/InvestigationV36R107/` (kể cả UI/onboarding); gửi đề nghị/patch cho người 2. Checkpoint vẫn bất biến dù Assets/Scenes do người 3 quản lý.
Scene thử: `Assets/ConRoi/Modules/GiaoDien/Scenes/GiaoDien_ThuNghiem.unity`.

## Việc đầu tiên

Tổ chức **người chưa đọc GDD** chơi thử trước khi mở rộng onboarding. Dùng [phiếu trung tính](../QA/investigation_r107/PLAYTEST_NGUOI_MOI.md), [hướng dẫn chỉ cho người chơi](../QA/investigation_r107/HUONG_DAN_NGUOI_CHOI.md), mẫu [ghi phiên](../QA/investigation_r106/MAU_GHI_NHAN_PHIEN.md) và [tổng hợp vấn đề](../QA/investigation_r106/MAU_TONG_HOP_VAN_DE.md).

Đóng Editor của checkout mình trước khi mở với `--investigation-save-dir <thu-muc-moi-chua-dung>` theo CLONE_VA_BAT_DAU. Xác nhận intro/hồ sơ trống, path F5/F6 thực tế; không dùng profile đã giải. Người chơi tự đi xưởng → Hà → xưởng, không cho đáp án hoặc thời điểm clip. Chỉ gợi ý thao tác khi họ yêu cầu; ghi nguyên văn lời hỏi/gợi ý.

Hoàn tất khi có biên bản mã người thử (không cần thông tin cá nhân), quan sát trực tiếp tách suy luận, chỗ không hiểu cơ cấu/không đọc hình/không biết thao tác/hiểu nhưng chưa đủ chứng minh, vấn đề theo mức ảnh hưởng và bước tái hiện. Không chấm bằng đoán đúng Khải. Chưa có người thật: giữ **NOT_RUN**, bàn giao lịch/phiếu sẵn sàng rồi dừng, không dùng agent hoặc thêm polish.

Bàn giao báo cáo riêng `Docs/QA/Tasks/N3/`, commit docs; đề xuất UI dựa trên vấn đề cụ thể, gửi người 2 nếu chạm session. Tích hợp thay đổi người 1/2 sau review và kiểm đúng phạm vi; không tự sửa thay cả ba module.
Prompt nhận việc: “Trên work/ui-onboarding-review từ fcbb7e19, chuẩn bị và ghi nhận playtest người mới trung thực bằng phiếu trung tính; ưu tiên phản hồi trước mở rộng onboarding, giữ quyền sửa session cho người 2 và scene chính cho người 3.”

## Điền khi nhận

- Người nhận / ngày:
- HEAD thực tế / file sẽ chạm:
- Mã phiên / trạng thái người thử thực tế:
- Kiểm và bằng chứng / đề nghị gửi người 2:
- Commit kết quả / giới hạn / bàn giao tích hợp:
''')

write('HOP_DONG_MODULE.md', '''# Hợp đồng module hiện hành — baseline fcbb7e19

Chủ file: **người 2**; người 3 review cách dùng UI, người 1 review asset binding. Đây là tài liệu mô tả mã có sẵn, chưa phải interface/assembly mới. Nguồn chuẩn: `Assets/Scripts/InvestigationV36R107/InvestigationSession.cs`, các partial cùng thư mục, `InvestigationEvidence.cs` và GDD v3.6. Không dùng GDD_IMPLEMENTATION_CURRENT v47 làm yêu cầu mới.

| Biên hiện hành | Producer / chủ sửa | Consumer / trách nhiệm |
|---|---|---|
| Asset, collider, rig, material và GUID | Người 1 | Người 3 wire scene; người 2 kiểm contract tương tác, không đổi ID để khớp tên art |
| `Acquire`, `Observe`, `Has`, `Record`, `Observation` | Session / người 2 | UI và interaction đọc trạng thái; acquire không đồng nghĩa đã nhìn thấy mọi quan sát |
| `OpenEvidence`, `OpenPage`, `ClosePage`, `SetPaused`, `Modal` | Session partial / người 2 | UI gửi ý định qua luồng hiện có; đóng trang phải dọn focus/drag và khôi phục input đúng ngữ cảnh, giữ NAV-01 |
| `ReadCipher`, `Assign`, `AddRelation` | Session / người 2 | UI trình bày dữ kiện và suy nghĩ; không tự gán danh tính/hung thủ vì chạy một ca |
| `StartTrial`, `ClearTrialHolds`, `TrialSettled` | Session + TrialTimeline + cơ cấu / người 2 | Người 3 wire trang bàn dựng; sáu ca cùng luật, giữ hồi hữu hạn, hiển thị đúng lực/móc/chèn; seek không cấp quan sát |
| `Save`, `Load`, `SaveDirectoryOverride`, `SavePath` | Session / người 2 | Người 3 chuẩn bị profile thử riêng và kiểm đường dẫn; không tự load profile kỹ thuật |

Save hiện hành schema1, namespace `conroi.investigation.v36.r107`; đường dẫn `<profile>/InvestigationV36/r107/prototype.json`. ID theo mã hiện hành, không đổi serialized field/schema/GUID trong handoff. CLI `--investigation-save-dir` được Awake đọc; đổi CLI cần mở lại Editor của checkout đó. F5/F6 chỉ dùng profile thử đã xác nhận.

Người 1/3 không gọi API seed dữ kiện để chứng minh người chơi tìm thấy chúng. Bằng chứng phải tách UI/input tổng hợp, API/fixture, render minh họa và người mới. Ca đã PASS kỹ thuật xem NavFix20261108; không suy toàn map PASS hoặc người mới PASS.

Nhiệm vụ đầu của người 2 là bổ sung chi tiết type/consumer/acceptance theo source thực tế trước đề xuất thay đổi. Mọi biên còn dùng chung file phải giữ một chủ; chưa tuyên bố ba module độc lập.
''')

# Keep historical evidence intact; place current instructions before older receipts.
prefix = '''# Chốt nhận việc 08/11/2026 — fcbb7e19 / biên nhận 55d7f04

Kết quả hiện hành: [bàn giao nhóm](TeamHandoff20261108/README.md), [audit Git/LFS](TeamHandoff20261108/audit.json).
Ba nhánh task đều chứa baseline fcbb7e19, không có commit riêng tại lúc kiểm; không đổi ref trong đợt này. Những thông báo BLOCKED/a209c0e phía dưới thuộc lịch sử, đã được biên nhận NAV-01 thay thế.

'''
for name in ['HANDOFF.md', 'OWNERSHIP.md', 'TICH_HOP.md', 'CLONE_VA_BAT_DAU.md']:
    old = (docs/name).read_text(encoding='utf-8-sig')
    extra = ''
    if name == 'OWNERSHIP.md':
        extra = '''## File giao nhau: một chủ sửa tại một thời điểm

| File / phạm vi | Chủ sửa duy nhất | Phối hợp |
|---|---|---|
| Map/NgheThuat, legacy Art/Model/Blender, Source, meta đi cùng | Người 1 | Người 3 chỉ wire scene; báo cáo topology của N1 ở Docs/QA/Tasks/N1 |
| Toàn bộ Assets/Scripts, gồm InvestigationV36R107 partial UI/onboarding, Editor và KiemThuLichSu | Người 2 | Người 3 gửi đề xuất UI; tool asset chỉ chuyển từng file cho N1 sau khi ghi DANG_LAM |
| Tools (gồm 852 script lịch sử), compile/asmdef và HOP_DONG_MODULE | Người 2 | Tool chia sẻ chỉ một chủ; không chạy lại script một lần |
| GiaoDien/NoiDung/TichHop, scene tích hợp và scene legacy được phép thay | Người 3 | N1/N2 giao prefab/patch + chỉ dẫn; checkpoint r101–r107 không ai sửa |
| GDD, BAT_DAU, README gốc, lịch sử/workflow, HANDOFF, OWNERSHIP, TICH_HOP, registry QA chung | Người 3 | Chỉ cập nhật từ review/kết quả thật; đổi GDD cần quyết định thiết kế, không mở scope |
| Packages, ProjectSettings, .gitignore/.gitattributes, cấu hình Git nhóm | Người 2 | Đóng băng trong task đầu; thay cần review N3, pipeline/package không tự nâng |
| DANG_LAM_NGUOI_1/2/3, báo cáo Docs/QA/Tasks/N1/N2/N3 | Người tương ứng | Người 3 tổng hợp vào lịch sử chung, không ba người cùng commit một lịch sử |
| Scene thử Map / Gameplay / GiaoDien | Người 1 / 2 / 3 tương ứng | Scene rỗng làm sandbox; không tự ghi đè scene tích hợp |
| File chưa có quy tắc, ngoài 4.183 legacy chưa xác minh | Người 3 điều phối | Chưa sửa cho tới khi giao một chủ bằng DANG_LAM; nhóm D và archive giữ nguyên |

Quy tắc bảng này thay phần phân chủ Editor/test mơ hồ ở ghi chép cũ bên dưới. Chuyển quyền phải ghi rõ file, người giao/nhận và commit, tạm dừng sửa ở chủ cũ. Meta thuộc cùng chủ asset. CODEOWNERS chờ username thật; không tạo tên giả.

'''
    if name == 'TICH_HOP.md':
        extra = '''## Quy trình hiện hành

1. Nhận đúng nhánh trong DANG_LAM_NGUOI_1/2/3; ghi HEAD/file dự định chạm trước sửa. Nhóm bắt đầu từ fcbb7e19, không phải a209c0e.
2. Mỗi người dùng checkout riêng. N1/N2 dùng scene thử và đề nghị wiring; chỉ N3 sửa `Assets/ConRoi/Modules/TichHop/Scenes/ConRoi_Chinh.unity`.
3. Thứ tự đầu: N1 triage topology/góc ảnh; N2 xác minh hợp đồng/GDD và chọn task nhỏ có review; N3 tổ chức người mới chơi trước mở rộng onboarding. Có thể làm song song ở file đã giao riêng.
4. Bàn giao commit + diff/file/GUID + ca kiểm chịu ảnh hưởng + giới hạn. N2 review runtime/contract; N3 review luồng/nội dung và tích hợp lên chore/prepare-r107. Không tự cherry-pick scene khác đè scene chính hoặc force/reset nhánh có việc riêng.
5. Docs điều phối mới nằm trên nhánh chore; task branch vẫn fcbb7e19. Trước bắt đầu sửa, từng người bảo toàn working tree rồi merge commit tài liệu chốt từ chore bằng Git (sau khi nhận ref đã publish hoặc từ repo local được phép). Không copy bù asset untracked. Không force cập nhật ba nhánh chỉ để nhận docs.
6. Không lặp toàn bộ kiểm Unity đã PASS nếu không thay sản phẩm. Sau thay đổi, compile và kiểm đúng luồng chịu ảnh hưởng; ghi fixture/API riêng. Người mới chưa chạy giữ NOT_RUN.

'''
    if name == 'CLONE_VA_BAT_DAU.md':
        extra = '''## Chọn đúng nhánh sau khi remote được duyệt và publish

Trong clone riêng dùng `git switch --track origin/work/map-asset-baseline` (N1), `origin/work/runtime-contracts-baseline` (N2), hoặc `origin/work/ui-onboarding-review` (N3); nếu nhánh local đã có thì switch nhánh đó và kiểm HEAD, không tạo đè. Nhận commit tài liệu từ `origin/chore/prepare-r107` bằng merge sau khi bảo toàn việc riêng. Hướng dẫn task: [N1](DANG_LAM_NGUOI_1.md), [N2](DANG_LAM_NGUOI_2.md), [N3](DANG_LAM_NGUOI_3.md).

Chưa có remote nên các lệnh clone/pull bên dưới là hướng dẫn cho giai đoạn được duyệt, chưa được thực hiện ở đợt này. Máy nhận cần Git LFS; kiểm `git lfs pull`, `git lfs fsck` và asset materialized trước mở Unity. Không lấy 4.183 legacy chưa xác minh hoặc profile/cache từ máy chủ để “bù” checkout.

'''
    write(name, prefix+extra+old)

write('TeamHandoff20261108/README.md', '''# Chốt bàn giao nhóm — 08/11/2026

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
''')
print('Wrote handoff documentation only.')
