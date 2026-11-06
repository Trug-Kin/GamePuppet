import hashlib,json,re
from pathlib import Path
d=Path('Test/KetQua/investigation_r107')
rows=[];broken=[]
for p in sorted(d.iterdir()):
 if not p.is_file() or p.name=='results-manifest.json':continue
 if p.suffix=='.md':
  s=p.read_text(encoding='utf-8');assert '\ufffd' not in s,p
  for target in re.findall(r'\]\(([^)]+)\)',s):
   if '://' not in target and not (p.parent/target.split('#')[0]).exists():broken.append(dict(file=p.name,target=target))
 rows.append(dict(file=p.name,bytes=p.stat().st_size,sha256=hashlib.sha256(p.read_bytes()).hexdigest()))
(d/'results-manifest.json').write_text(json.dumps(dict(files=rows,broken_markdown_links=broken),ensure_ascii=False,indent=2),encoding='utf-8')
print(json.dumps(dict(result_files=len(rows),broken_links=broken,protected=json.loads((d/'protected-final.json').read_text(encoding='utf-8'))['unchanged']),ensure_ascii=False))
