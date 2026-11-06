import json,sys
from pathlib import Path
sys.path.insert(0,'Tools/python-deps')
import numpy as np
D=Path('Test/KetQua/journey_v48/ground_r93');g=np.load(D/'layer-grid.npz');a=json.loads((D/'geometry-inventory.json').read_text())
out=[]
for x,z in [(715,478),(715,480),(713,482),(711,483),(720,480),(707,499),(640,410),(782,267),(813,200),(728,299)]:
 hits=[]
 for r in a:
  if r.get('cache') not in g:continue
  h=g[r['cache']][z-40,x-450]
  if np.isfinite(h):hits.append(dict(path=r['path'],y=float(h),render=r['render'],active=r['active'],role=r['role']))
 hits.sort(key=lambda r:r['y'],reverse=True);out.append(dict(xz=[x,z],hits=hits))
(D/'critical-point-layers.json').write_text(json.dumps(out,indent=2));print(json.dumps(out,indent=2))
