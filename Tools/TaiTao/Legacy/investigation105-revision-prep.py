from pathlib import Path
import json,hashlib,re,uuid,shutil
R=Path('D:/ConRoi');D=R/'Test/KetQua/investigation_r105';scene=R/'Assets/Scenes/JourneyPresentation_v48_r105.unity';dest=R/'Assets/Scripts/InvestigationV36R105'
assert not scene.exists() and not dest.exists(),'Existing r105: inspect instead of overwriting'
def sha(p):return hashlib.sha256(p.read_bytes()).hexdigest()
old=json.loads((R/'Test/KetQua/investigation_r104/protected-final.json').read_text())
paths={Path(x['path']) for x in old['files']}
for x in json.loads((R/'Test/KetQua/investigation_r104/delivery-manifest.json').read_text()):
 if x['path'].replace('\\','/').startswith(('Assets/','Source/')):paths.add(R/x['path'])
assert all(p.is_file() for p in paths)
(D/'protected-start.json').write_text(json.dumps([dict(path=str(p),sha256=sha(p)) for p in sorted(paths)],indent=2))
files=list((R/'Assets/Scripts/InvestigationV36R104').glob('*'));maps={}
for p in files:
 if p.suffix=='.meta':maps[re.search(r'^guid: (\w+)',p.read_text(),re.M)[1]]=uuid.uuid4().hex
def remap(t):
 for a,b in maps.items():t=t.replace(a,b)
 return t
for p in files:
 if not p.is_file():continue
 q=dest/p.name;q.parent.mkdir(parents=True,exist_ok=True)
 t=remap(p.read_text(encoding='utf-8-sig')).replace('ConRoi.Investigation.V36.R104','ConRoi.Investigation.V36.R105').replace('conroi.investigation.v36.r104','conroi.investigation.v36.r105').replace('"r104"','"r105"');q.write_text(t,encoding='utf-8')
t=remap((R/'Assets/Scenes/JourneyPresentation_v48_r104.unity').read_text());t=t.replace('Investigation_V36_R104','Investigation_V36_R105').replace('Khang_FirstPerson_R104','Khang_FirstPerson_R105').replace('conroi.investigation.v36.r104','conroi.investigation.v36.r105');scene.write_text(t)
scene.with_suffix('.unity.meta').write_text('fileFormatVersion: 2\nguid: '+uuid.uuid4().hex+'\nDefaultImporter:\n  externalObjects: {}\n  userData: \n  assetBundleName: \n  assetBundleVariant: \n')
S=R/'Source/Blender/JourneyV48/R105';S.mkdir(parents=True);shutil.copy2(R/'Source/Blender/JourneyV48/R104/Repair_R12_R104_Animation.blend',S/'Repair_R12_R105_Animation.blend')
(R/'Assets/Art/InvestigationV36/R105').mkdir(parents=True)
(D/'isolation.json').write_text(json.dumps(dict(scene=str(scene),script_guids=maps,source='r104 final',shared_readonly_art='R104 Quaternius, evidence, mechanism; do not mutate shared assets',protected=len(paths)),indent=2))
print('Guarded r105 prepared:',len(maps),'isolated script GUIDs;',len(paths),'protected files. No author/bind reintegration performed.')
