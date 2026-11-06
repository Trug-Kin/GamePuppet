import hashlib
import json
from pathlib import Path

root = Path('D:/ConRoi')
out = root / 'Test/KetQua/journey_v48/terrain_r101'
evidence = root / 'Test/BangChung/journey_v48/terrain_r101'
scene = root / 'Assets/Scenes/JourneyPresentation_v48_r101.unity'
source = root / 'Source/Blender/JourneyV48/R101/Terrain_R101_Editable.blend'
assets = root / 'Assets/Art/Environment/JourneyV48/TerrainR101'
fbx = assets / 'TerrainR101_Integrated.fbx'
export = out / 'export/TerrainR101_Integrated.fbx'

def sha(path):
    h = hashlib.sha256()
    with path.open('rb') as stream:
        for block in iter(lambda: stream.read(1024 * 1024), b''):
            h.update(block)
    return h.hexdigest()

assert sha(fbx) == sha(export), 'FBX export/Assets mismatch'
result = json.loads((out / 'final-check-response.json').read_text(encoding='utf-8-sig'))
assert result['result']['success'] is True
preserved = json.loads((out / 'preservation-final.json').read_text(encoding='utf-8-sig'))
for key, count in [('protected', 1140), ('tracked_original_gameplay_saves', 17)]:
    assert preserved[key]['count'] == preserved[key]['unchanged'] == count

readme = '''# Bàn giao terrain r101 — 04/11/2026

Vết rách tại X643.5/Z414 đã sửa cục bộ. Source và Unity CPU mesh cùng có winding quay xuống; không quy lỗi cho buffer render. Đã sửa 108 mặt, gợn ramp sát bên, hai collider cũ chắn đường và đầu nối lối thấp r97. Không thay nước, nền chợ, cụm đập, controller hoặc gameplay.

| Mục | Kết luận | Bằng chứng |
|---|---|---|
| Điểm báo rách | Đã sửa mặt bị cull; đây là mép mái bờ dốc, không công bố là đường đi | before-after.jpg; top/underside/section-before/after.png; final-ray-response.json |
| Ramp và nối r97 | Đã sửa nguồn, chia chung seam, đồng bộ render/collider | sections-before-after.png; source-sections-before-after.json; final-integration.tsv |
| Vật cản vô hình | Cắt cục bộ collider Context_107/124; giữ ngoài phạm vi | join-delta.json; context124-delta.json |
| Controller | Tuyến liên tục từ nền tốt X654/Z414 qua thềm khô X644.6/Z414, nối r97 X649.4/Z420 rồi quay lại; không teleport giữa tuyến | terrain-controller.mp4; scope-and-controller-plan.png; controller-summary.json |
| Lùi/ch éo | Đã kiểm bổ sung tiếp nối lượt controller, không đặt lại | terrain-backward-diagonal.mp4; back-diagonal-motion.txt |
| Lifecycle | Pause/resume; 3 load cao độ cũ + 3 load hiện hành không nâng cộng dồn; phòng → load hồ giữ geometry đúng | pause.txt; repeat-load.txt; save-comparison.json; region-return.txt |
| Mở lại scene | r101 sạch, Missing Script 0; giữ 8 reference đèn/hoa r100 | scene-final-reopen.txt |
| M2 | UNVERIFIED ánh xạ ảnh–object, giữ hình học | Không suy từ tên object |
| Ngoài cửa sổ sửa | Còn terrain legacy/cull, đặc biệt Z<411; chưa kiểm toàn bộ | winding-extent.json; controller-filmstrip.jpg |

## Mở đúng nguồn

- Scene: `Assets/Scenes/JourneyPresentation_v48_r101.unity` (Unity 6000.6.2f1, DX11).
- Blender: `Source/Blender/JourneyV48/R101/Terrain_R101_Editable.blend`, collection `TERRAIN_R101_EDITABLE_REPAIR`. Đã lưu–đóng–mở lại, giữ context đầy đủ.
- FBX: `Assets/Art/Environment/JourneyV48/TerrainR101/TerrainR101_Integrated.fbx`.
- Năm bound asset dùng suffix `_IntegratedR101.asset`. Các file stage khác không được scene cuối reference. Scene chỉ đổi 8 reference mesh/collider so với r100; không sửa prefab checkpoint hay producer dùng chung.
- Ánh xạ chi tiết trong `final-integration.tsv` và mục r101 của hai tài liệu chung.

## Xem bằng chứng

Ảnh/video nằm tại `Test/BangChung/journey_v48/terrain_r101/`. Video là cửa sổ Game với **input tổng hợp InputSystem**, **không âm thanh**; fixture riêng đặt một lần ở đầu. Camera chẩn đoán trong ảnh trước–sau không thay bằng chứng controller. Không chứng nhận input phần cứng, New Game liên tục, mọi mép nước hay toàn tuyến cầu–đập.

Các lượt bị chặn đầu tiên giữ riêng trong thư mục lịch sử. Kiểm runtime ban đầu so identity MeshFilter/Collider đã báo nhầm khi Unity static batch hai mesh r97. Kiểm cuối đối chiếu đúng submesh/world geometry, không đổi sản phẩm để làm test qua (`static-batch-proof.txt`).

## Bảo toàn và giới hạn

Đúng danh sách theo dõi: **1140/1140 file bảo vệ và 17/17 save gameplay giữ SHA256** (`preservation-final.json`). Không suy rộng thành 523 save. Sáu load giữ trường tiến trình/lựa chọn đã đối chiếu; trường pose/memory runtime có thay đổi được liệt kê riêng. CrashReporter là telemetry riêng.

Không build/ZIP/runner mới; không triển khai GDD v3.6 trong đợt này. Giữ checkpoint r94–r100 và các ZIP cũ. Kích thước là đơn vị authoring, chưa xác minh kích thước thực địa. Không tuyên bố toàn map PASS. Bước sau bàn giao là prototype xưởng → quầy Hà → quay lại xưởng.

`handoff-manifest.json` liệt kê hash đúng nguồn cuối, asset/meta, scene, tài liệu và bằng chứng/report hiện có tại thời điểm bàn giao; không bao gồm chính manifest. Không dùng các stage để ghi đè nguồn cuối.
'''.replace('Lùi/ch éo', 'Lùi/chéo')
(out / 'README.md').write_text(readme, encoding='utf8')
outcome = {
    'baseline': 'r100', 'current': 'r101', 'reported_tear': 'FIXED_LOCALLY',
    'scene': scene.relative_to(root).as_posix(),
    'source': source.relative_to(root).as_posix(),
    'collection': 'TERRAIN_R101_EDITABLE_REPAIR',
    'fbx_export_matches_unity_asset': True,
    'protected_files_unchanged': 1140, 'tracked_original_saves_unchanged': 17,
    'controller': json.loads((out / 'controller-summary.json').read_text(encoding='utf8')),
    'open': ['M2 photo/object mapping', 'legacy terrain outside repair, notably Z<411', 'all historic saves and all water edges not tested'],
    'not_run': ['hardware input', 'headphones', 'new player', 'full bridge/dam regression'],
    'new_build_zip_runner': False,
}
(out / 'handoff-outcome.json').write_text(json.dumps(outcome, ensure_ascii=False, indent=2), encoding='utf8')
paths = {source, scene, Path(str(scene) + '.meta'), fbx, Path(str(fbx) + '.meta')}
paths.update(assets.glob('*_IntegratedR101.asset*'))
paths.update(root / p for p in ['BAT_DAU_TAI_DAY.md', 'Docs/PROJECT_GUIDE_AND_HISTORY.md', 'Docs/BLENDER_UNITY_WORKFLOW.md'])
paths.update(p for p in evidence.rglob('*') if p.is_file())
paths.update(p for p in out.rglob('*') if p.is_file() and p.name != 'handoff-manifest.json')
manifest = {'scope': 'Exact final files listed; stage reports retained as history, final product uses Integrated assets.',
            'files': [{'path': p.relative_to(root).as_posix(), 'bytes': p.stat().st_size, 'sha256': sha(p)} for p in sorted(paths)]}
(out / 'handoff-manifest.json').write_text(json.dumps(manifest, ensure_ascii=False, indent=2), encoding='utf8')
print('Handoff written; manifest files:', len(paths), '; FBX hashes equal; protected 1140/1140; saves 17/17.')
