import json,hashlib
from pathlib import Path
R=Path('D:/ConRoi');D=R/'Test/KetQua/journey_v48/market_ground_r95';out=[]
for rel in ['Test/KetQua/journey_v48/market_ground_r95/protected-start.json','Test/KetQua/journey_v48/r93/product-before-integration.json','Docs/Reviews/journey_v48/restructure-20261102/user-saves-before.json']:
 data=json.loads((R/rel).read_text());rows=data if isinstance(data,list) else [dict(path=p,sha256=h) for p,h in data.items()];checks=[]
 for row in rows:
  p=Path(row['path']);p=p if p.is_absolute() else R/p;h=hashlib.sha256(p.read_bytes()).hexdigest() if p.exists() else None;checks.append(dict(path=str(p),expected=row['sha256'],actual=h,match=h==row['sha256']))
 out.append(dict(manifest=rel,tracked=len(checks),matched=sum(x['match'] for x in checks),files=checks));print(rel,len(checks),'matched',sum(x['match'] for x in checks),'changed',[x['path'] for x in checks if not x['match']])
allowed={'Assets/Scripts/MoiTruong/KhoiPhucCaoDoSauTaiR73.cs','Assets/Scripts/TuongTac/ChuanBiGoiHangR88.cs'}
unexpected=[];authorized=[]
for group in out:
 for row in group['files']:
  if row['match']:continue
  rel=Path(row['path']).relative_to(R).as_posix()
  if rel in allowed and group['manifest'].endswith('product-before-integration.json'):authorized.append(rel)
  else:unexpected.append(row)
(D/'preservation-final.json').write_text(json.dumps(dict(groups=out,authorized_deltas=authorized,unexpected=unexpected,claim='53 protected files and 17 tracked saves; not independent hashing of all 523 saves'),indent=2));assert not unexpected,'Unexpected protected file mismatch; inspect exact report'
