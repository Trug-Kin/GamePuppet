from pathlib import Path
import hashlib,json,re
R=Path('D:/ConRoi');D=R/'Test/KetQua/journey_v48/bank_r97'
files=[]
for folder in ['Source/Blender/JourneyV48/R97','Assets/Art/Environment/JourneyV48/BankR97','Test/BangChung/journey_v48/bank_r97','Test/KetQua/journey_v48/bank_r97']:
 files.extend(p for p in (R/folder).rglob('*') if p.is_file() and p.name!='final-manifest.json')
files.extend((R/'Test/Tam').glob('bank97-*'))
files.extend(R/p for p in ['Assets/Scenes/JourneyPresentation_v48_r97.unity','Assets/Scenes/JourneyPresentation_v48_r97.unity.meta','Docs/PROJECT_GUIDE_AND_HISTORY.md','Docs/BLENDER_UNITY_WORKFLOW.md'])
rows=[dict(path=str(p.relative_to(R)),bytes=p.stat().st_size,sha256=hashlib.file_digest(p.open('rb'),'sha256').hexdigest()) for p in sorted(set(files))]
(D/'final-manifest.json').write_text(json.dumps(dict(files=rows,scope='New r97 working products and evidence only; existing checkpoint/save comparison in preservation-final.json; no build or package'),indent=2))
bad=[]
for file in ['Docs/PROJECT_GUIDE_AND_HISTORY.md','Docs/BLENDER_UNITY_WORKFLOW.md']:
 p=R/file;s=p.read_text(encoding='utf-8');first=s[:s.index('\n## ',10)]
 for link in re.findall(r'\]\(([^)]+)\)',first):
  if '://' not in link and not (p.parent/link.split('#')[0]).exists():bad.append(dict(doc=file,link=link))
(D/'doc-links.json').write_text(json.dumps(dict(new_section_missing_links=bad),indent=2));print('manifest',len(rows),'new doc missing links',bad)
