import sys,json
from pathlib import Path
sys.path.insert(0,'Tools/python-deps')
import numpy as np
D=Path('Test/KetQua/journey_v48/ground_r93')
a=json.loads((D/'geometry-inventory.json').read_text()); g=np.load(D/'layer-grid.npz'); inv={r['path']:r for r in a}
for r in a:
 if r.get('cache') and (r['role'] in ['terrain','road','footway'] or r['path'].endswith(('Surface_142','Surface_127','Surface_134'))):
  print(r['cache'],r['path'],r['role'],'bounds',np.round(r['bounds'],2).tolist())
out=[]
for r in a:
 if not r.get('cache') or r['cache'] not in g or '#COLLIDER' in r['path']:continue
 c=inv.get(r['path']+'#COLLIDER')
 if not c or c.get('cache') not in g:continue
 delta=g[c['cache']]-g[r['cache']];v=delta[np.isfinite(delta)]
 if len(v):out.append(dict(render=r['path'],collider=c['path'],samples=len(v),range=[float(v.min()),float(v.max())],over5cm=int((abs(v)>.05).sum())))
(D/'paired-render-collider.json').write_text(json.dumps(out,indent=2))
print('PAIRED DIFFERENCES',json.dumps([r for r in out if r['over5cm']],indent=2))
ls=json.loads((D/'layer-sections.json').read_text());sec=json.loads((D/'sections.json').read_text());bad=[]
for r in ls:
 if r['path'].split('/')[-1] not in ['Surface_176','Surface_142','Surface_104','Surface_127','Surface_128','Surface_129','Surface_134','Surface_173','Surface_174','Surface_175','RoadLoop_125314010_R65']:continue
 for s in r['segments']:
  if s['length']>=4:bad.append(dict(path=r['path'],region=sec[r['section']]['region'],section=sec[r['section']]['name'],**s))
bad.sort(key=lambda r:r['max_adjacent_grade_change_percent'],reverse=True)
(D/'section-anomalies.json').write_text(json.dumps(bad,indent=2))
print('KINK CANDIDATES',json.dumps(bad[:12],indent=2))
print('GROUND COLLIDERS')
for r in a:
 if '#COLLIDER' in r['path'] and r.get('cache') in g and (r['path'].startswith('LakeGateMarketV32/Context_') or any(s in r['path'] for s in ['Terrain','Road','Footway','Bank_Return','Deck'])):
  print(r['cache'],r['path'],r['asset'])
print('NON-LOCAL KINKS',json.dumps([r for r in bad if not r['path'].endswith('Surface_142')][:10],indent=2))
