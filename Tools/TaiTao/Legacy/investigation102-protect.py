import json,hashlib
from pathlib import Path
root=Path('D:/ConRoi'); out=root/'Test/KetQua/investigation_r102';out.mkdir(parents=True,exist_ok=True)
paths=set()
old=json.loads((root/'Test/KetQua/journey_v48/terrain_r101/preservation-final.json').read_text(encoding='utf8'))
for key in ('protected','tracked_original_gameplay_saves'):
 for row in old[key]['files']: paths.add(Path(row['path']))
for folder in ('Assets/Art/Environment/JourneyV48/TerrainR101','Source/Blender/JourneyV48/R101'):
 paths.update(p for p in (root/folder).rglob('*') if p.is_file())
paths.update([root/'Assets/Scenes/JourneyPresentation_v48_r101.unity',root/'Assets/Scenes/JourneyPresentation_v48_r101.unity.meta',root/'Docs/CON_ROI_GDD_v3.6_CHOT_TRIEN_KHAI.md'])
def sha(p):return hashlib.file_digest(p.open('rb'),'sha256').hexdigest()
p=out/'protected-start.json'
assert not p.exists()
p.write_text(json.dumps([{'path':str(f),'sha256':sha(f)} for f in sorted(paths)],indent=2),encoding='utf8')
print('Protected files',len(paths),'including 17 explicitly tracked original saves')
