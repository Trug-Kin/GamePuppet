import hashlib,json
from pathlib import Path
d=Path('Test/KetQua/investigation_r107')
rows=json.loads((d/'protected-start.json').read_text(encoding='utf-8-sig'))
result=[]
for row in rows:
 p=Path(row['path']);current=hashlib.sha256(p.read_bytes()).hexdigest() if p.exists() else None
 result.append(dict(path=str(p),expected=row['sha256'],current=current,unchanged=current==row['sha256']))
out=dict(total=len(result),unchanged=sum(r['unchanged'] for r in result),changed=[r for r in result if not r['unchanged']],rows=result)
(d/'protected-final.json').write_text(json.dumps(out,ensure_ascii=False,indent=2),encoding='utf-8')
print(json.dumps({k:v for k,v in out.items() if k!='rows'},ensure_ascii=False,indent=2))
