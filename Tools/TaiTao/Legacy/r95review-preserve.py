from pathlib import Path
import hashlib,json
R=Path('D:/ConRoi');D=R/'Test/KetQua/journey_v48/r95_visual_review'
def sha(p):
 h=hashlib.sha256()
 with p.open('rb') as f:
  for b in iter(lambda:f.read(1024*1024),b''):h.update(b)
 return h.hexdigest()
groups=[]
for file in [D/'product-before-review.json',R/'Test/KetQua/journey_v48/market_ground_r95/protected-start.json',R/'Docs/Reviews/journey_v48/restructure-20261102/user-saves-before.json']:
 data=json.loads(file.read_text());rows=data if isinstance(data,list) else [dict(path=k,sha256=v) for k,v in data.items()];out=[]
 for row in rows:
  p=Path(row['path']);p=p if p.is_absolute() else R/p;actual=sha(p) if p.is_file() else None;out.append(dict(path=str(p),expected=row['sha256'],actual=actual,match=actual==row['sha256']))
 groups.append(dict(manifest=str(file.relative_to(R)),tracked=len(out),matched=sum(x['match'] for x in out),files=out))
 print(groups[-1]['manifest'],groups[-1]['tracked'],groups[-1]['matched'])
(D/'preservation-review.json').write_text(json.dumps(dict(groups=groups,claim='Exact listed files only; 17 tracked gameplay saves, not independent verification of all 523 historical saves.'),indent=2))
assert all(g['tracked']==g['matched'] for g in groups)
