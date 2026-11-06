import sys,json,math
from pathlib import Path
sys.path.insert(0,'Tools/python-deps')
import numpy as np,shapely
from shapely import STRtree
from scipy.spatial import cKDTree
D=Path('Test/KetQua/journey_v48/i2_r93_complete');inv=json.loads((D/'native-input.json').read_text())['objects'];cx,cz=714.94,459.48
class Surface:
 def __init__(self,row):
  d=np.load(D/(row['key']+'.npz'));self.v=d['v'];self.tri=d['tri'];q=self.v[self.tri];a,b,c=q[:,0],q[:,1],q[:,2];den=(b[:,2]-c[:,2])*(a[:,0]-c[:,0])+(c[:,0]-b[:,0])*(a[:,2]-c[:,2]);ok=abs(den)>1e-9;self.q=q[ok];self.den=den[ok];self.tree=STRtree(shapely.polygons(self.q[:,:,[0,2]]))
 def hit(self,pp):
  pp=np.atleast_2d(pp);pi,ti=self.tree.query(shapely.points(pp));a,b,c=self.q[ti,0],self.q[ti,1],self.q[ti,2];x,z=pp[pi].T;den=self.den[ti];u=((b[:,2]-c[:,2])*(x-c[:,0])+(c[:,0]-b[:,0])*(z-c[:,2]))/den;v=((c[:,2]-a[:,2])*(x-c[:,0])+(a[:,0]-c[:,0])*(z-c[:,2]))/den;ok=(u>=-1e-6)&(v>=-1e-6)&(u+v<=1.000001);h=u*a[:,1]+v*b[:,1]+(1-u-v)*c[:,1];out=np.full(len(pp),-np.inf);np.maximum.at(out,pi[ok],h[ok]);out[~np.isfinite(out)]=np.nan;return out
surfs={r['path'].split('/')[-1]:Surface(r) for r in inv if r['path'].split('/')[-1] in ['Surface_128','Surface_129','Surface_142','Surface_153','Surface_176']}
records=[]
for a in range(0,181,15):
 for r in [13.2,14,16,18,21,24,27,30]:
  p=[cx+r*math.cos(math.radians(a)),cz+r*math.sin(math.radians(a))];row=dict(angle_from_unity_x=a,radius=r,xz=p,heights={})
  for name,s in surfs.items():
   h=s.hit([p])[0];row['heights'][name]=float(h) if np.isfinite(h) else None
  records.append(row)
(D/'boundary-samples.json').write_text(json.dumps(records,indent=2));print(json.dumps([r for r in records if r['angle_from_unity_x'] in [30,60,90,120,150] and r['radius'] in [13.2,14,24,27]],indent=2))
# Native layer offsets where both actual layers exist, not nearest extrapolation.
xx,zz=np.meshgrid(np.arange(680,751,.25),np.arange(455,494,.25));p=np.c_[xx.ravel(),zz.ravel()];rr=np.linalg.norm(p-[cx,cz],axis=1);road=surfs['Surface_128'].hit(p)
out=[]
for name,s in surfs.items():
 h=s.hit(p);ok=np.isfinite(h)&np.isfinite(road)&(rr>22)&(rr<32);delta=h[ok]-road[ok];out.append(dict(layer=name,samples=int(ok.sum()),median=float(np.median(delta)) if len(delta) else None,range=[float(delta.min()),float(delta.max())] if len(delta) else None))
(D/'layer-offsets-existing.json').write_text(json.dumps(out,indent=2));print('OFFSETS',out)
