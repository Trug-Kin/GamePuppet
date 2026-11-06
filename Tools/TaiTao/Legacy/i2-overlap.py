import sys,json,math
from pathlib import Path
sys.path.insert(0,'Tools/python-deps')
import numpy as np,shapely
from shapely import STRtree
from PIL import Image,ImageDraw,ImageFont
D=Path('Test/KetQua/journey_v48/i2_r93_complete');M=Path('Test/BangChung/journey_v48/i2_r93_complete');M.mkdir(parents=True,exist_ok=True);report=json.loads((D/'geometry-change.json').read_text());cx,cz=714.94,459.48
class Surf:
 def __init__(self,v,tri):
  self.v,self.tri=v,tri;q=v[tri];a,b,c=q[:,0],q[:,1],q[:,2];den=(b[:,2]-c[:,2])*(a[:,0]-c[:,0])+(c[:,0]-b[:,0])*(a[:,2]-c[:,2]);ok=abs(den)>1e-10;self.q=q[ok];self.den=den[ok];self.tree=STRtree(shapely.polygons(self.q[:,:,[0,2]]));self.ids=np.flatnonzero(ok)
 def query(self,pp):
  pp=np.atleast_2d(pp);pi,ti=self.tree.query(shapely.points(pp));a,b,c=self.q[ti,0],self.q[ti,1],self.q[ti,2];x,z=pp[pi].T;den=self.den[ti];u=((b[:,2]-c[:,2])*(x-c[:,0])+(c[:,0]-b[:,0])*(z-c[:,2]))/den;v=((c[:,2]-a[:,2])*(x-c[:,0])+(a[:,0]-c[:,0])*(z-c[:,2]))/den;ok=(u>=-1e-6)&(v>=-1e-6)&(u+v<=1.000001);pi,ti,u,v=pi[ok],ti[ok],u[ok],v[ok];h=u*a[ok,1]+v*b[ok,1]+(1-u-v)*c[ok,1];top=np.full(len(pp),-np.inf);low=np.full(len(pp),np.inf);np.maximum.at(top,pi,h);np.minimum.at(low,pi,h);top[~np.isfinite(top)]=np.nan;low[~np.isfinite(low)]=np.nan;return top,low

g=np.load(D/'measurement-grid.npz');xx,zz=np.meshgrid(g['xs'],g['zs']);pp=np.c_[xx.ravel(),zz.ravel()];road=g['I2_Surface_128_after'].ravel();a=json.loads((D/'native-input.json').read_text())['objects'];out=[]
for r in a:
 if r['path'].startswith('LakeGateMarket'):continue
 p=D/(('I2_'+r['path'].split('/')[-1]+'-source.npz') if any(t['path']==r['path'] for t in report['objects']) else (r['key']+'.npz'));d=np.load(p);h,_=Surf(d['v'],d['tri']).query(pp);sel=np.isfinite(h)&np.isfinite(road)&(h>road+.005);ids=np.flatnonzero(sel);out.append(dict(path=r['path'],above_samples=len(ids),worst=[dict(xz=pp[i].tolist(),height=float(h[i]),road=float(road[i]),delta=float(h[i]-road[i])) for i in ids[np.argsort((h-road)[ids])[-8:]]]))
(D/'overlap-above-road.json').write_text(json.dumps(out,indent=2));print(json.dumps([r for r in out if r['above_samples']],indent=2))
