# Bổ sung tài liệu chung từ kết quả thực tế, không sửa manifest/log lịch sử.
from pathlib import Path
from DuongDanKiem import dau_vao
import json
R=Path(__file__).resolve().parents[1]
O=R/'Docs/Reviews/journey_v48/restructure-20261102'
def read(name,default=None):
    p=dau_vao(O/name)
    return json.loads(p.read_text(encoding='utf-8-sig')) if p.exists() else default

def main():
    if (R/"Docs/PROJECT_GUIDE_AND_HISTORY.md").read_text(encoding="utf-8").startswith("## Tổ chức lại r92"):
        raise SystemExit("Ho so da bo sung; khong chay lap. Chinh tai lieu truc tiep neu can cap nhat ket qua.")
    build=read('full-build-reorganized.json',{}); audit=read('static-preservation.json',{}); smoke=read('smoke-tests.json',{}); package=read('package-reorganized.json',{})
    intro='''## Tổ chức lại r92 — 02/11/2026

**Nguồn hiện hành: D:/ConRoi**, Unity 6000.6.2f1, scene `Assets/Scenes/JourneyPresentation_v48_r92.unity`. Unity Hub đã đổi đúng mục project; mở `ConRoi.code-workspace`. Root cũ D:/Game/GameHorror/Game chỉ còn vỏ rỗng do host giữ cwd. Không thay gameplay/model/controller/điểm/ending, tên sản phẩm/company, schema save hoặc bảo mật Windows.

### Cây thư mục và phân loại

| Mục | Vai trò / quyết định |
|---|---|
| Assets | Tài nguyên Unity và script. .meta/GUID script, prefab, model được giữ. |
| Assets/Scripts | KhoiDong, NguoiChoi, TuongTac, NhanVat, ConRoi, GiaoDien, LuuTai, MoiTruong, ChanDoan, Editor; KiemThuLichSu và Prototypes giữ phụ thuộc checkpoint. Âm thanh nằm trong owner hành trình/diễn xuất nên không tạo AmThanh rỗng. |
| Docs | GDD, hướng dẫn, workflow, lịch sử, Reviews và báo cáo. |
| Docs/SourceImages/Bug | Giữ nguyên 24 ảnh nguồn người dùng từ Bug/. |
| Docs/Archives | Công cụ lịch sử cùng module/phụ thuộc; 145 thư mục chỉ có .thumbnails rỗng được lưu nguyên trạng vì chưa chứng minh nguồn tạo, không xóa. |
| Docs/Fixtures/BuildExcluded | Giữ R92Resources/.meta ngoài Assets, không nhập ordinary player. |
| Builds | ZIP giao r90/r91/r92 giữ nguyên hash; bản mới có chữ Reorganized. Output cũ còn dùng đối chiếu/debug, chưa xóa suy đoán. |
| Source/Blender | 36 file editable/mapping/recipe R91/R92 ngoài Assets; không chạy Blender/export hoặc sửa model trong đợt này. |
| Tools | Công cụ tổ chức/kiểm/build, gửi lệnh GUI/input đang dùng; python-deps bên thứ ba giữ cấu trúc. |
| .agents | Giữ skill, không dọn vì tên cũ. |
| Library → .build-cache/r92-unity-library | Cache đang dùng giữ nguyên, junction cập nhật D:/ConRoi; package được đăng ký từ D:/ConRoi/Library/PackageCache. |
| .build-temp, .tmp, Logs, UserSettings | Giữ log/crash/chẩn đoán/thiết lập; .tmp còn protocol/nguồn thử chưa rõ phụ thuộc nên không xóa. |

### Chốt nguồn và checkpoint

Source chính khớp source-r92-manifest; hai tài liệu chung có addendum mới hơn. Đối chiếu toàn bộ Assets/Packages/ProjectSettings/BuildExcluded trong work copy: sản phẩm r92 đã tích hợp. Helper riêng và hai scene lịch sử khác bản chính đã archive ở `work-unique-before-cleanup.zip`, không ghi đè checkpoint chính. Fixture trong work có hash trùng fixture ở đường khác. Git trả lỗi không có repository; `git-status.json` giữ trạng thái thực tế, không coi file chưa commit/untracked là rác. Source/ảnh/Blender/build chưa được Git quản lý được di chuyển nguyên trạng.

Checkpoint `Reviews/journey_v48/restructure-20261102/checkpoint-before-restructure.zip`: 19.206 file có thể sửa (C#, .meta, scene, prefab, config, công cụ và hai tài liệu). Hash/phạm vi ở `checkpoint.json`; không nhân đôi cache/build/mesh/ảnh để sao lưu. Các báo cáo máy đọc trong cùng thư mục là bảng di chuyển có thể hoàn nguyên.

| Cũ | Mới |
|---|---|
| D:/Game/GameHorror/Game | D:/ConRoi; root-move-plan.json / root-move-result.json. |
| Assets/Editor | Assets/Scripts/Editor, giữ .meta folder/script. |
| Assets/Scripts/Gameplay (fixture còn lại) | Assets/Scripts/KiemThuLichSu. |
| BuildExcluded | Docs/Fixtures/BuildExcluded. |
| Bug | Docs/SourceImages/Bug. |
| Tools lịch sử | Docs/Archives/ToolsBeforeRestructure; tool-moves.json ghi từng mục. |
| Tools/unity_live_sequence.py | Tools/GuiLenhUnity.py, root tính từ __file__. |
| Tools/Unity/SendMovementInput.ps1 | Tools/Unity/GuiNhapLieuUnity.ps1. |
| ZIP player r90/r91/r92 trong Reviews | Builds/ConRoi-r90/r91/r92-Windows-playtest.zip; delivered-zip-moves.json ghi tên/hash chính xác. |

Log/manifest/recipe lịch sử giữ nguyên đường dẫn gốc làm bằng chứng: ánh xạ root rồi áp bảng move khi tra cứu. Liên kết source trong hai tài liệu chung trỏ file hiện tại; bản trước có trong checkpoint. Công cụ lịch sử cần checkpoint/môi trường tương ứng, không chạy author/repair lịch sử trực tiếp lên r92.

| native-input.py r90 (bản gốc giữ ở Reviews) | Tools/DieuKhienNhapLieu.py; kế thừa công cụ OS, root tương đối, sửa xử lý cửa sổ; input-tool-origin.json. |

### Mở, chạy và build

- Unity Hub → ConRoi, hoặc `powershell -File Tools/KiemUnity.ps1 -Mode Open -Tag <tên-log-mới>` tại D:/ConRoi. Scene r92, Play; Enter/New Game bắt đầu, E tương tác, P điện thoại, Esc pause, F5/F6 lưu/tải. Kiểm dùng `--r30-review-output <thư-mục-thử-riêng>`.
- Compile/reference/save-reopen: `powershell -File Tools/KiemUnity.ps1 -Mode Probe -Tag <tên-mới>`. Helper hiện có `KiemTraNguonVaBuild` giữ idle gate 5 giây trước OpenScene, workaround crash batch r92 đã quan sát; nguyên nhân native cũ còn OPEN.
- Full build: `powershell -File Tools/KiemUnity.ps1 -Mode Build -Tag <tên-mới>`. Output mặc định `Builds/ConRoi-r92-Reorganized-20261102`; chọn output khác trong launcher trước lần build sau vì helper từ chối ghi đè. Build Settings chỉ r92. TEMP/TMP riêng tiến trình; không đổi Windows policy/pagefile/package.
- `GuiLenhUnity.py` gửi lệnh bridge có sẵn, chờ ACK, không mở Unity. Không tạo runner gameplay mới hoặc chạy lại toàn bộ ma trận lịch sử.

### Kết quả và giới hạn

'''
    intro+=f"Năm nhóm đã compile và mở/reopen r92. Kiểm cuối lưu scene thành công; r92 có 10.473 object, r90/r91 không Missing Script/broken reference; 32 prefab không Missing Script. Full build **{build.get('result')}**, {build.get('errors')} error / {build.get('warnings')} warning.\n\n"
    intro+='So checkpoint: '+json.dumps(audit,ensure_ascii=False)+'\n\nKiểm player riêng: '+json.dumps(smoke,ensure_ascii=False)+'\n\nZIP/startup bản giải nén cuối: '+json.dumps(package,ensure_ascii=False)+'\n\n'
    intro+='''Bảo toàn cuối: 17 save người dùng, ba ZIP gốc, 36 editable Blender và checkpoint khớp hash (`protected-data-final.json`). Kiểm source/serialized reference không thay playtest đầy đủ. Player dùng input OS tổng hợp và fixture/profile riêng, không phải input phần cứng hoặc người mới. Không kiểm lại toàn bộ ending, chất lượng model/âm thanh hoặc đóng các hạn chế r92 cũ. Chi tiết log/ảnh/hành động trong hồ sơ đợt này.

### Xóa và giữ

Không xóa script trong đợt này: chưa đủ bằng chứng không còn cần cho scene/checkpoint/tooling. RegionRenderGateV47 còn consumer GetComponent theo chuỗi ở Editor lịch sử; AudioProbeV40 được bridge tạo; Review/Probe opt-in cần để tái lập. Không tạo Missing Script trong checkpoint để giảm số file. Assets không có asmdef/asmref tự viết hoặc Addressables cần đổi assembly; package/plugin giữ nguyên. Method UnityEvent/lifecycle/API Unity và field/schema JSON giữ nguyên. MovedFrom hỗ trợ tên kiểu cũ; .meta/GUID là liên kết chính.

Đường dẫn thực sự đã xóa (target junction và file count trong deleted-paths.json):

'''
    for e in read('deleted-paths.json',[]):intro+='- `'+e['path']+'`: '+e['reason']+'\n'
    intro+='''
.build-work chỉ dọn sau đối chiếu/archive delta. Cache cũ có bốn junction lịch sử trên C được gỡ liên kết, không đệ quy target. Cache đang dùng, Reviews, checkpoint, Blender, ảnh và save người dùng giữ lại. Không dọn các bản giải nén ngoài project trên C.

### Hoàn nguyên

1. Đóng Unity/player; giữ hồ sơ/ZIP mới riêng. Không mở hai Editor dùng chung cache.
2. Theo script-moves.json và directory-moves.json chuyển script mới cùng .meta về đường cũ theo thứ tự ngược, không ghi đè file trùng; khôi phục C#/scene/prefab/config và tài liệu đã sửa từ checkpoint. Không chép checkpoint đè toàn bộ Assets/mesh đang giữ.
3. Khôi phục ảnh/fixture/Tools/ZIP theo outer-moves.json, tool-moves.json, delivered-zip-moves.json. Unity Hub có backup unityhub-projects-before.json; nếu người dùng đã sửa danh sách sau đó, chỉ đổi lại mục ConRoi thay vì ghi đè toàn bộ.
4. Muốn trả root cũ: đóng host giữ cwd rồi chuyển nguyên root về D:/Game/GameHorror/Game (hoặc chuyển từng mục vào vỏ rỗng). Chỉ gỡ junction Library bằng thao tác không đệ quy, tạo lại target cache theo root cũ. Không xóa target qua junction.
5. Cache cũ/work source trùng đã xóa là dữ liệu tái tạo, không cần khôi phục để có source r92 trước tổ chức. Muốn tái lập work thử cũ: dựng bản riêng từ chuỗi ZIP nguồn r90/r91/r92 và work-unique-before-cleanup.zip; không ghi đè checkpoint chính. Compile/kiểm scene trước sử dụng.

### File/lớp cũ → mới và object/component

Tên field serialized/method giữ nguyên. File partial theo cùng lớp. Các attachment checkpoint và Inspector reference đầy đủ ở [script-guide.json](Reviews/journey_v48/restructure-20261102/script-guide.json).

| File/lớp cũ | File/lớp mới | Object r92 |
|---|---|---|
'''
    for e in read('script-guide.json',[]):
        names=', '.join(e['r92_objects'][:3]) or 'Checkpoint/consumer; xem JSON'
        intro+=f"| `{e['old']}` / `{e['old_type']}` | [{e['new_type']}]({('../'+e['new'])}) | {names.replace('|','/')} |\n"
    intro+='\nCác phần dưới là lịch sử, giữ tên/đường dẫn tại thời điểm đó.\n\n---\n\n'
    p=R/'Docs/PROJECT_GUIDE_AND_HISTORY.md';old=p.read_text(encoding='utf-8')
    for e in read('script-moves.json',[]):old=old.replace('('+ '../'+e['old']+')','('+ '../'+e['new']+')')
    for a,b in read('directory-moves.json',[]):old=old.replace('('+ '../'+a+'/', '('+'../'+b+'/')
    for e in read('delivered-zip-moves.json',[]):old=old.replace('('+e['old'][5:]+')','(../'+e['new']+')')
    p.write_text(intro+old,encoding='utf-8')
    p=R/'Docs/BLENDER_UNITY_WORKFLOW.md';old=p.read_text(encoding='utf-8')
    for e in read('script-moves.json',[]):old=old.replace('('+ '../'+e['old']+')','('+ '../'+e['new']+')')
    note='''## Tổ chức lại r92 — 02/11/2026

Nguồn Unity hiện tại **D:/ConRoi**, scene `Assets/Scenes/JourneyPresentation_v48_r92.unity`. Root cũ D:/Game/GameHorror/Game ánh xạ sang root mới; giữ nguyên đường dẫn trong log/manifest/recipe lịch sử. Không chạy Blender/author/export trong đợt này; không đổi mesh/material/pose/rig/controller/reference model.

Editable ở `Source/Blender/JourneyV48/R91` (bốn .blend, mapping/recipe) và `Source/Blender/JourneyV48/R92/character-repairs-r92-source.blend`, ngoài Assets. Asset vẫn Art/Characters/R91,R92 và BankR91, GUID giữ nguyên. Ảnh Bug → Docs/SourceImages/Bug; ZIP player r90/r91/r92 → Builds nguyên hash. Skill/Reviews/checkpoint/save giữ lại; chỉ cache cũ và work trùng đã dọn sau đối chiếu, delta riêng có archive.

Script đổi tên không dấu theo trách nhiệm, giữ .meta và MovedFrom; xem bảng file/lớp/object/Inspector, compile–save–reopen–prefab, full build, player/ZIP, danh sách xóa và hoàn nguyên trong [PROJECT_GUIDE_AND_HISTORY](PROJECT_GUIDE_AND_HISTORY.md) và Reviews/journey_v48/restructure-20261102/. Quy trình gán Blender/mesh/collider bên dưới giữ nguyên, chỉ tên lớp/đường script đổi. Không đổi field/schema save. Tool/recipe lịch sử ở Docs/Archives/ToolsBeforeRestructure với tool-moves.json, không tự chạy lên r92.

Mở Unity Hub ConRoi hoặc Tools/KiemUnity.ps1; -Mode Probe kiểm source, -Mode Build full build với tag/output mới. Một Editor dùng cache .build-cache/r92-unity-library; Library trỏ root mới. Giữ workaround idle gate; không sửa Windows security. Phần dưới là lịch sử từng đợt.

---

'''
    p.write_text(note+old,encoding='utf-8')
    p=R/'README.md';p.write_text('# CON RỐI — source r92 tại D:/ConRoi\n\nĐã tổ chức script theo chức năng và Việt hóa 100 file, giữ gameplay/model/save và ZIP r90/r91/r92. Mở ConRoi.code-workspace hoặc Unity Hub ConRoi; scene Assets/Scenes/JourneyPresentation_v48_r92.unity.\n\n[Hướng dẫn, kiểm, đổi tên, hoàn nguyên](Docs/PROJECT_GUIDE_AND_HISTORY.md) · [Workflow Blender/Unity](Docs/BLENDER_UNITY_WORKFLOW.md) · [Builds](Builds/README.md).\n\nPhần dưới là lịch sử bàn giao.\n\n---\n\n'+p.read_text(encoding='utf-8'),encoding='utf-8')
    (R/'ConRoi.code-workspace').write_text(json.dumps({'folders':[{'path':'.'}],'settings':{'dotnet.defaultSolution':'ConRoi.slnx'}},indent=2),encoding='utf-8')
    (R/'Builds/README.md').write_text('# Gói Windows\n\nZIP r90/r91/r92 chuyển nguyên hash từ Reviews; delivered-zip-moves.json ghi vị trí cũ. Bản sau tổ chức có chữ Reorganized.\n\n'+''.join('- ['+e['new'].split('/')[-1]+']('+e['new'].split('/')[-1]+')\n' for e in read('delivered-zip-moves.json',[]))+'\nXem kết quả/giới hạn ở ../Docs/PROJECT_GUIDE_AND_HISTORY.md.\n',encoding='utf-8')
    (R/'Tools/README.md').write_text('# Công cụ hiện hành\n\n- KiemUnity.ps1: mở/kiểm/build bằng helper Editor cũ; root tính từ PSScriptRoot, tag/output mới không ghi đè.\n- GuiLenhUnity.py và Unity/GuiNhapLieuUnity.ps1: bridge có sẵn/input OS theo PID.\n- SapXepDuAn.py, ChuThichScript.py: bảng đổi tên/chú thích đợt này, không áp lại nhóm đã move.\n- KiemTraBaoToan.py: đối chiếu token C#, GUID, YAML với checkpoint.\n- KiemKeSapXep.py: audit trước chuyển đã chạy, không ghi đè checkpoint.\n- python-deps: bên thứ ba, giữ cấu trúc.\n\nCông cụ lịch sử/module giữ trong Docs/Archives/ToolsBeforeRestructure; xem tool-moves.json, không chạy author/repair lịch sử lên r92 mới.\n',encoding='utf-8')
    with (R/'Tools/README.md').open('a',encoding='utf-8') as f:f.write('\n- DieuKhienNhapLieu.py: kế thừa helper OS r90; --boot/--folder profile thử riêng, tự F5 sau mỗi capture; khi kiểm F6 phải phục hồi save thử gốc trước tải.\n- DongGoiBanSapXep.py: ZIP và hash/extract, từ chối ghi đè.\n- KiemDuLieuCanGiu.py: hash ZIP gốc, 17 save, Blender, checkpoint.\n- CapNhatHoSo.py: bổ sung hồ sơ một lần, không chạy lặp.\n')
    print('Updated guide/workflow, README, workspace, Builds/Tools entry points')
if __name__=='__main__':main()
