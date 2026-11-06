import sys,json
from pathlib import Path
sys.path.insert(0,'Tools/python-deps')
import numpy as np
D=Path('Test/KetQua/journey_v48/ground_r93');a=json.loads((D/'geometry-inventory.json').read_text());g=np.load(D/'layer-grid.npz')
cols=[r for r in a if '#COLLIDER' in r['path'] and r.get('cache') in g];out=[]
for r in a:
 if not r.get('render') or not r['active'] or r.get('cache') not in g or r['path'].startswith('RouteSurfaces/'):continue
 if not(r['role'] in ['terrain','road','footway'] or r['path'].endswith(('Surface_142','Surface_127','Surface_134'))):continue
 h=g[r['cache']];best=np.full(h.shape,np.inf);which=np.full(h.shape,-1)
 for j,c in enumerate(cols):
  delta=g[c['cache']]-h;dist=abs(delta);sel=np.isfinite(dist)&(dist<best);best[sel]=dist[sel];which[sel]=j
 sel=np.isfinite(h);wrong=sel&(best>.05);yy,xx=np.where(wrong);order=np.argsort(best[wrong])[::-1]
 samples=[]
 for k in order[:12]:
  iy,ix=yy[k],xx[k];j=which[iy,ix];samples.append(dict(xz=[float(g['xs'][ix]),float(g['zs'][iy])],render_y=float(h[iy,ix]),delta=None if j<0 else float(g[cols[j]['cache']][iy,ix]-h[iy,ix]),collider=None if j<0 else cols[j]['path']))
 out.append(dict(path=r['path'],samples=int(sel.sum()),without_match_5cm=int(wrong.sum()),without_any=int((sel&~np.isfinite(best)).sum()),worst=samples))
(D/'all-render-collider-nearest.json').write_text(json.dumps(out,indent=2));print(json.dumps(out,indent=2)[:17000])
