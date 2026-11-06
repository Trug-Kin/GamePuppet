import sys,json
from pathlib import Path
sys.path.insert(0,'Tools/python-deps')
import numpy as np
D=Path('Test/KetQua/journey_v48/market_ground_r95');g=json.loads((D/'grading.json').read_text());points=[];lower=[];raiseby=[]
for r in g['objects']:
 a=np.load(D/(r['key']+'-source.npz'));delta=a['v'][:,1]-a['before'][:,1];changed=abs(delta)>.001;t=a['tri'];faces=changed[t].any(1)
 if faces.any():points.append(a['v'][t[faces]].reshape(-1,3));lower.append(float((-delta).max()));raiseby.append(float(delta.max()))
v=np.vstack(points);b=[v[:,0].min(),v[:,2].min(),v[:,0].max(),v[:,2].max()];(D/'changed-envelope.tsv').write_text('\t'.join(str(float(x)) for x in b));(D/'load-envelope.json').write_text(json.dumps(dict(bounds_xz=b,max_lowered=max(lower),max_raised=max(raiseby),load_only_probe_below=25,reason='22.024m measured maximum lowering plus clearance; no controller change; raising requires separate fixture test'),indent=2));print(b,max(lower),max(raiseby))
