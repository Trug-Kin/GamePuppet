from pathlib import Path
import json,hashlib
R=Path('D:/ConRoi');D=R/'Test/KetQua/journey_v48/market_ground_r95'
paths=[R/'Source/Blender/JourneyV48/R95/Market_Ground_R95_Editable.blend',R/'Assets/Scenes/JourneyPresentation_v48_r95.unity',R/'Assets/Scenes/JourneyPresentation_v48_r95.unity.meta',R/'Assets/Scripts/MoiTruong/KhoiPhucCaoDoSauTaiR73.cs',R/'Assets/Scripts/MoiTruong/KhoiPhucCaoDoSauTaiR73.cs.meta',R/'Assets/Scripts/TuongTac/ChuanBiGoiHangR88.cs',R/'Assets/Scripts/TuongTac/ChuanBiGoiHangR88.cs.meta',R/'Docs/PROJECT_GUIDE_AND_HISTORY.md',R/'Docs/BLENDER_UNITY_WORKFLOW.md']
for folder in ['Assets/Art/Environment/JourneyV48/MarketGroundR95','Test/Output/journey_v48/MarketGroundR95','Test/BangChung/journey_v48/market_ground_r95']:
 paths.extend(p for p in (R/folder).rglob('*') if p.is_file())
paths.extend(p for p in D.iterdir() if p.is_file() and p.suffix in ['.json','.txt','.tsv'] and p.name!='source-assets-final-manifest.json')
paths.extend(p for p in (R/'Test/Tam').glob('market95-*') if p.is_file())
paths=sorted(set(paths));out=[]
for p in paths:
 h=hashlib.sha256()
 with p.open('rb') as f:
  for b in iter(lambda:f.read(1024*1024),b''):h.update(b)
 out.append(dict(path=p.relative_to(R).as_posix(),bytes=p.stat().st_size,sha256=h.hexdigest()))
(D/'source-assets-final-manifest.json').write_text(json.dumps(dict(algorithm='SHA256',files=out,scope='Exact listed working source, Unity assets/meta, derived scene, docs, recipe, evidence and scoped result files; not all original saves'),indent=2))
source=next(x for x in out if x['path'].endswith('Market_Ground_R95_Editable.blend'));assert source['sha256']==json.loads((D/'blender-acceptance.json').read_text())['sha256']
for row in (D/'integration-manifest.tsv').read_text(encoding='utf-8').splitlines():
 key=row.split('\t')[0];a=R/'Test/Output/journey_v48/MarketGroundR95'/(key+'.fbx');b=R/'Assets/Art/Environment/JourneyV48/MarketGroundR95'/(key+'.fbx');assert hashlib.sha256(a.read_bytes()).digest()==hashlib.sha256(b.read_bytes()).digest(),key
print('FINAL_MANIFEST',len(out),'all319 exported/imported FBX hashes match; source',source['sha256'])
