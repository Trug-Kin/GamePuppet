import sys,json
from pathlib import Path
sys.path.insert(0,'Tools/python-deps')
import numpy as np
D=Path('Test/KetQua/journey_v48/ground_r93');inv={r['path']:r for r in json.loads((D/'geometry-inventory.json').read_text())};trial=json.loads((D/'transition-trial.json').read_text());out=[]
for item in trial['objects']:
 if item['path'].split('/')[-1] not in ['Surface_128','Surface_153','Surface_173','Surface_176','Surface_142']:continue
 row={'path':item['path']}
 for tag,fp in [('before',D/(inv[item['path']]['cache']+'.npz')),('after',D/('trial-'+item['object']+'.npz'))]:
  a=np.load(fp);q=a['v'][a['tri']];c=q.mean(1);n=np.cross(q[:,1]-q[:,0],q[:,2]-q[:,0]);area=abs(n[:,1])/2;grade=np.linalg.norm(n[:,[0,2]],axis=1)/np.maximum(abs(n[:,1]),1e-12);rr=np.linalg.norm(c[:,[0,2]]-[714.94,459.48],axis=1);ids=np.flatnonzero((rr>13.2)&(rr<24)&(area>.01)&(grade>.2));ids=ids[np.argsort(area[ids])[::-1]][:12];row[tag]=[dict(centroid=c[i].tolist(),area=float(area[i]),grade=float(grade[i]*100),vertices=q[i].tolist()) for i in ids]
 out.append(row)
(D/'trial-regression-locations.json').write_text(json.dumps(out,indent=2));print(json.dumps(out,indent=2))
