from pathlib import Path
import json,hashlib,re,uuid,shutil
R=Path('D:/ConRoi');D=R/'Test/KetQua/investigation_r103';D.mkdir(parents=True,exist_ok=True)
assert not (R/'Assets/Scenes/JourneyPresentation_v48_r103.unity').exists()
def sha(p):
 with p.open('rb') as f:return hashlib.file_digest(f,'sha256').hexdigest()
paths={Path(x['path']) for x in json.loads((R/'Test/KetQua/investigation_r102/protected-start.json').read_text())}
paths.update(R/x['path'] for x in json.loads((R/'Test/KetQua/investigation_r102/delivery-manifest.json').read_text())['files'] if x['path'].startswith(('Assets/','Source/')))
saveRoot=Path('C:/Users/kaito/AppData/LocalLow/DefaultCompany/Con Roi/InvestigationV36/r102')
if saveRoot.exists():paths.update(p for p in saveRoot.rglob('*') if p.is_file())
snapshot=[dict(path=str(p),sha256=sha(p)) for p in sorted(paths)]
if (D/'protected-start.json').exists():assert json.loads((D/'protected-start.json').read_text())==snapshot
else:(D/'protected-start.json').write_text(json.dumps(snapshot,indent=2))
pairs=[('Assets/Art/InvestigationV36/R102','Assets/Art/InvestigationV36/R103'),('Assets/Scripts/InvestigationV36','Assets/Scripts/InvestigationV36R103')]
files=[];guids={}
for src,dst in pairs:
 assert not (R/dst).exists()
 files.extend((p,R/dst/p.relative_to(R/src)) for p in (R/src).rglob('*') if p.is_file())
for a,b in files:
 if a.suffix=='.meta':
  m=re.search(r'^guid: (\w+)',a.read_text(),re.M)
  if m:guids[m[1]]=uuid.uuid4().hex
def mapped(s):
 for a,b in guids.items():s=s.replace(a,b)
 return s
for a,b in files:
 b.parent.mkdir(parents=True,exist_ok=True)
 if a.suffix in ('.meta','.cs','.mat','.renderTexture','.prefab','.asset'):
  s=mapped(a.read_text(encoding='utf-8-sig'))
  if a.suffix=='.cs':s=s.replace('ConRoi.Investigation.V36','ConRoi.Investigation.V36.R103').replace('conroi.investigation.v36.r102','conroi.investigation.v36.r103').replace('"r102"','"r103"')
  b.write_text(s,encoding='utf-8')
 else:shutil.copy2(a,b)
scene=R/'Assets/Scenes/JourneyPresentation_v48_r103.unity'
scene.write_text(mapped((R/'Assets/Scenes/JourneyPresentation_v48_r102.unity').read_text()).replace('Investigation_V36_R102','Investigation_V36_R103').replace('Khang_FirstPerson_R102','Khang_FirstPerson_R103'),encoding='utf-8')
scene.with_suffix('.unity.meta').write_text('fileFormatVersion: 2\nguid: '+uuid.uuid4().hex+'\nDefaultImporter:\n  externalObjects: {}\n  userData: \n  assetBundleName: \n  assetBundleVariant: \n')
source=R/'Source/Blender/JourneyV48/R103';source.mkdir(parents=True,exist_ok=True)
for a,b in [('Workshop_Investigation_R102_Editable.blend','Workshop_Investigation_R103_Editable.blend'),('Puppet_Poses_R102_Editable.blend','Puppet_Poses_R103_Editable.blend')]:shutil.copy2(R/'Source/Blender/JourneyV48/R102'/a,source/b)
(R/'Test/BangChung/investigation_r103').mkdir(parents=True,exist_ok=True)
(D/'isolation.json').write_text(json.dumps(dict(protected=len(paths),guid_map=guids,copies=[str(b.relative_to(R)) for a,b in files],scene=str(scene)),indent=2))
print('Isolated',len(files),'files;',len(guids),'GUIDs; protected',len(paths))
