from pathlib import Path
import json, hashlib, re, uuid
R=Path('D:/ConRoi');D=R/'Test/KetQua/investigation_r106'
scene=R/'Assets/Scenes/JourneyPresentation_v48_r106.unity'
dest=R/'Assets/Scripts/InvestigationV36R106'
assert not scene.exists() and not dest.exists(), 'Inspect existing r106; never overwrite'
def sha(p):return hashlib.sha256(p.read_bytes()).hexdigest()
old=json.loads((R/'Test/KetQua/investigation_r105/protected-final.json').read_text())
paths={Path(x['path']) for x in old['files']}
for x in json.loads((R/'Test/KetQua/investigation_r105/delivery-manifest.json').read_text()):
 if x['path'].replace('\\','/').startswith(('Assets/','Source/')):paths.add(R/x['path'])
assert all(p.is_file() for p in paths)
(D/'protected-start.json').write_text(json.dumps([dict(path=str(p),sha256=sha(p)) for p in sorted(paths)],indent=2))
files=list((R/'Assets/Scripts/InvestigationV36R105').glob('*'));maps={}
for p in files:
 if p.suffix=='.meta':maps[re.search(r'^guid: (\w+)',p.read_text(),re.M)[1]]=uuid.uuid4().hex
def remap(s):
 for a,b in maps.items():s=s.replace(a,b)
 return s
dest.mkdir()
for p in files:
 if p.is_file():
  s=remap(p.read_text(encoding='utf-8-sig')).replace('ConRoi.Investigation.V36.R105','ConRoi.Investigation.V36.R106').replace('conroi.investigation.v36.r105','conroi.investigation.v36.r106').replace('"r105"','"r106"')
  (dest/p.name).write_text(s,encoding='utf-8')
s=remap((R/'Assets/Scenes/JourneyPresentation_v48_r105.unity').read_text()).replace('Investigation_V36_R105','Investigation_V36_R106').replace('Khang_FirstPerson_R105','Khang_FirstPerson_R106').replace('conroi.investigation.v36.r105','conroi.investigation.v36.r106')
scene.write_text(s,encoding='utf-8')
scene.with_suffix('.unity.meta').write_text('fileFormatVersion: 2\nguid: '+uuid.uuid4().hex+'\nDefaultImporter:\n  externalObjects: {}\n  userData: \n  assetBundleName: \n  assetBundleVariant: \n')
(D/'isolation.json').write_text(json.dumps(dict(scene=str(scene),script_guids=maps,protected=len(paths),source='r105 final',art='r104/r105 shared unchanged; no mesh/animation editing'),indent=2))
print('New r106 timeline isolation:',len(maps),'GUIDs;',len(paths),'protected files. No art author/bind scripts run.')
