from pathlib import Path
import hashlib,json
R=Path('D:/ConRoi');D=R/'Test/KetQua/journey_v48/bank_r97'
reports=[]
for file in [D/'protected-current-start.json',R/'Test/KetQua/journey_v48/market_ground_r95/protected-start.json',R/'Docs/Reviews/journey_v48/restructure-20261102/user-saves-before.json']:
 rows=json.loads(file.read_text(encoding='utf-8-sig'));out=[]
 if isinstance(rows,dict):rows=[dict(path=k,sha256=v) for k,v in rows.items()]
 for row in rows:
  p=Path(row['path']);p=p if p.is_absolute() else R/p
  h=hashlib.file_digest(p.open('rb'),'sha256').hexdigest() if p.exists() else None
  out.append(dict(path=str(p),expected=row['sha256'],actual=h,unchanged=h==row['sha256']))
 reports.append(dict(baseline=str(file.relative_to(R)),count=len(out),unchanged=sum(x['unchanged'] for x in out),files=out))
(D/'preservation-final.json').write_text(json.dumps(reports,indent=2));print([(r['count'],r['unchanged']) for r in reports])
old=D/'private-old-height.json';assert old.read_bytes()==Path(str(old)+'.before').read_bytes()
base=json.loads(old.read_text(encoding='utf-8-sig'));diff=[]
for i in range(3):
 state=json.loads((D/f'old-load-{i}.json').read_text(encoding='utf-8-sig'));diff.append(dict(load=i,changed_fields=[k for k in base if base[k]!=state.get(k)],position=state.get('position')))
(D/'load-state-diff.json').write_text(json.dumps(diff,indent=2));print(diff)
