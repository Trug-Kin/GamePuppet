"""One guarded derivation of finalized r103; never author or reintegrate checkpoints."""
from pathlib import Path
import json,hashlib,re,uuid,shutil
R=Path('D:/ConRoi');D=R/'Test/KetQua/investigation_r104';D.mkdir(parents=True,exist_ok=True)
scene=R/'Assets/Scenes/JourneyPresentation_v48_r104.unity'
assert not scene.exists(),'r104 exists: inspect it instead of overwriting'
assert 'dirty=False playing=False' in (R/'Test/Tam/investigation104-editor-before.txt').read_text()
def sha(p):
 with p.open('rb') as f:return hashlib.file_digest(f,'sha256').hexdigest()
baseline=json.loads((R/'Test/KetQua/investigation_r103/protected-final.json').read_text())
paths={Path(row['path']) for row in baseline['files']}
paths.update(R/row['path'] for row in json.loads((R/'Test/KetQua/investigation_r103/delivery-manifest.json').read_text()) if row['path'].startswith(('Assets/','Source/')))
saves=Path('C:/Users/kaito/AppData/LocalLow/DefaultCompany/Con Roi/InvestigationV36/r103')
if saves.exists():paths.update(p for p in saves.rglob('*') if p.is_file())
snapshot=[dict(path=str(p),sha256=sha(p)) for p in sorted(paths)]
if (D/'protected-start.json').exists():assert json.loads((D/'protected-start.json').read_text())==snapshot,'Existing snapshot differs'
else:(D/'protected-start.json').write_text(json.dumps(snapshot,indent=2))
files=[];mapping={}
for src,dst in [('Assets/Art/InvestigationV36/R103','Assets/Art/InvestigationV36/R104'),('Assets/Scripts/InvestigationV36R103','Assets/Scripts/InvestigationV36R104')]:
 assert not (R/dst).exists()
 files.extend((p,R/dst/p.relative_to(R/src)) for p in (R/src).rglob('*') if p.is_file())
for src,dst in files:
 if src.suffix=='.meta':
  m=re.search(r'^guid: (\w+)',src.read_text(),re.M)
  if m:mapping[m[1]]=uuid.uuid4().hex
def remap(text):
 for a,b in mapping.items():text=text.replace(a,b)
 return text
for src,dst in files:
 dst.parent.mkdir(parents=True,exist_ok=True)
 if src.suffix in ('.meta','.cs','.mat','.renderTexture','.prefab','.asset'):
  text=remap(src.read_text(encoding='utf-8-sig'))
  if src.suffix=='.cs':text=text.replace('ConRoi.Investigation.V36.R103','ConRoi.Investigation.V36.R104').replace('conroi.investigation.v36.r103','conroi.investigation.v36.r104').replace('"r103"','"r104"')
  dst.write_text(text,encoding='utf-8')
 else:shutil.copy2(src,dst)
text=remap((R/'Assets/Scenes/JourneyPresentation_v48_r103.unity').read_text())
for a,b in [('Investigation_V36_R103','Investigation_V36_R104'),('Khang_FirstPerson_R103','Khang_FirstPerson_R104'),('conroi.investigation.v36.r103','conroi.investigation.v36.r104')]:text=text.replace(a,b)
scene.write_text(text,encoding='utf-8');scene.with_suffix('.unity.meta').write_text('fileFormatVersion: 2\nguid: '+uuid.uuid4().hex+'\nDefaultImporter:\n  externalObjects: {}\n  userData: \n  assetBundleName: \n  assetBundleVariant: \n')
S=R/'Source/Blender/JourneyV48/R104';S.mkdir(parents=True,exist_ok=True)
for name in ['Workshop_Investigation_R103_Editable.blend','Puppet_Poses_R103_Editable.blend','Repair_R12_R103_Animation.blend']:shutil.copy2(R/'Source/Blender/JourneyV48/R103'/name,S/name.replace('R103','R104'))
(R/'Test/BangChung/investigation_r104').mkdir(parents=True,exist_ok=True)
(D/'isolation.json').write_text(json.dumps(dict(source='r103 finalized',scene=str(scene),guid_map=mapping,copied=[str(b.relative_to(R)) for a,b in files],protected=len(paths)),indent=2))
print('Derived r104 once:',len(mapping),'GUIDs;',len(paths),'protected files')
