import json,hashlib
from pathlib import Path
R=Path('D:/ConRoi');D=R/'Test/KetQua/journey_v48/terrain_r101';old=R/'Test/KetQua/journey_v48/map_r99_closeout'
def sha(p):
 h=hashlib.sha256()
 with p.open('rb') as f:
  for c in iter(lambda:f.read(4194304),b''):h.update(c)
 return h.hexdigest()
paths={Path(r['path']) for r in json.loads((old/'protected-start.json').read_text())}
for r in json.loads((old/'final-manifest.json').read_text())['files']:
 p=R/r['path']
 if r['path'].startswith(('Assets/','Source/')):paths.add(p)
rows=[dict(path=str(p),sha256=sha(p)) for p in sorted(paths)]
assert not (D/'protected-start.json').exists()
(D/'protected-start.json').write_text(json.dumps(rows,indent=2))
saves=json.loads((R/'Docs/Reviews/journey_v48/restructure-20261102/user-saves-before.json').read_text())
result=[dict(path=r['path'],sha256=sha(Path(r['path'])),matches_historic=sha(Path(r['path']))==r['sha256']) for r in saves]
(D/'original-saves-start.json').write_text(json.dumps(result,indent=2))
print('protected',len(rows),'saves',len(result),'historic equal',sum(r['matches_historic'] for r in result))
