from pathlib import Path
import json,hashlib,re,uuid
R=Path('D:/ConRoi');D=R/'Test/KetQua/investigation_r107'
assert not (R/'Assets/Scenes/JourneyPresentation_v48_r107.unity').exists()
assert not (R/'Assets/Scripts/InvestigationV36R107').exists()
old=json.loads((R/'Test/KetQua/investigation_r106/playtest_handoff/protected-final.json').read_text())
paths={Path(x['path']) for x in old['files']}
paths.update((R/'Test/PlaytestProfiles/PT001_r106').rglob('*.json'))
(D/'protected-start.json').write_text(json.dumps([dict(path=str(p),sha256=hashlib.sha256(p.read_bytes()).hexdigest()) for p in sorted(paths)],indent=2),encoding='utf-8')
dest=R/'Assets/Scripts/InvestigationV36R107';dest.mkdir()
src=R/'Assets/Scripts/InvestigationV36R106';mapping={}
for p in src.glob('*.meta'):mapping[re.search(r'^guid: (\w+)',p.read_text(),re.M)[1]]=uuid.uuid4().hex
def remap(s):
 for a,b in mapping.items():s=s.replace(a,b)
 return s.replace('ConRoi.Investigation.V36.R106','ConRoi.Investigation.V36.R107').replace('conroi.investigation.v36.r106','conroi.investigation.v36.r107').replace('"r106"','"r107"')
for p in src.iterdir():
 if p.is_file():(dest/p.name).write_text(remap(p.read_text(encoding='utf-8-sig')),encoding='utf-8')
scene=R/'Assets/Scenes/JourneyPresentation_v48_r107.unity'
scene.write_text(remap((R/'Assets/Scenes/JourneyPresentation_v48_r106.unity').read_text()).replace('Investigation_V36_R106','Investigation_V36_R107').replace('Khang_FirstPerson_R106','Khang_FirstPerson_R107'),encoding='utf-8')
scene.with_suffix('.unity.meta').write_text('fileFormatVersion: 2\nguid: '+uuid.uuid4().hex+'\nDefaultImporter:\n  externalObjects: {}\n  userData: \n  assetBundleName: \n  assetBundleVariant: \n')
(D/'isolation.json').write_text(json.dumps({'source':'r106 saved checkpoint, not runtime state','scene':str(scene),'script_guids':mapping,'protected_files':len(paths)},indent=2))
print('Isolated r107; protected',len(paths),'files. No existing author/derive/bind scripts run.')
