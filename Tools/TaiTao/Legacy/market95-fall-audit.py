import json,sys
from pathlib import Path
sys.path.insert(0,'Tools/python-deps')
import numpy as np
D=Path('Test/KetQua/journey_v48/market_ground_r95')
g=json.loads((D/'grading.json').read_text())['objects'];base={r['path']:r for r in json.loads((D/'geometry-inventory.json').read_text())}
def hits(v,t,p):
 q=v[t];x,z=p;a=q[:,0];b=q[:,1];c=q[:,2];den=(b[:,2]-c[:,2])*(a[:,0]-c[:,0])+(c[:,0]-b[:,0])*(a[:,2]-c[:,2]);ok=abs(den)>1e-10;den=np.where(ok,den,1)
 u=((b[:,2]-c[:,2])*(x-c[:,0])+(c[:,0]-b[:,0])*(z-c[:,2]))/den;w=((c[:,2]-a[:,2])*(x-c[:,0])+(a[:,0]-c[:,0])*(z-c[:,2]))/den
 return (u*a[:,1]+w*b[:,1]+(1-u-w)*c[:,1])[ok&(u>=-1e-6)&(w>=-1e-6)&(u+w<=1.000001)].tolist()
out=[]
for p in [(704.03,422.82),(702,420),(700.5511,418.559),(696,413),(705,385)]:
 row={'xz':p,'layers':[]}
 for r in g:
  a=np.load(D/(r['key']+'-graded.npz'));v=a['v'];t=a['tri']
  if not ((v[:,0].min()<=p[0]<=v[:,0].max()) and (v[:,2].min()<=p[1]<=v[:,2].max())):continue
  after=hits(v,t,p);old=base.get(r['path']);before=[]
  if old:
   a=np.load(D/(old['cache']+'.npz'));before=hits(a['v'],a['tri'],p)
  if after or before:row['layers'].append(dict(path=r['path'],before=before,after=after))
 out.append(row)
(D/'controller-fall-layer-audit.json').write_text(json.dumps(out,indent=2));print(json.dumps(out,indent=2))
