import sys,json,math
from pathlib import Path
sys.path.insert(0,'Tools/python-deps')
import numpy as np,shapely
from shapely import STRtree
from shapely.geometry import Polygon
D=Path('Test/KetQua/journey_v48/ground_r93');stats=json.loads((D/'geometry-inventory.json').read_text())
REGIONS=[{'id':'I1_SW','title':'SW bridge-end traffic island','center':[610.67,563.99],'half':[32,27],'rings':[9,11,14],'rays':30},
{'id':'I2_NE','title':'NE bridge / gate roundabout','center':[714.94,459.48],'half':[48,44],'rings':[15,18,22],'rays':50},
{'id':'I3_MARKET','title':'Market statue roundabout + all aprons','center':[783.948,191.167],'half':[70,55],'rings':[21,24,27],'rays':65},
{'id':'BRIDGE','title':'Ong Dao deck + both abutments','center':[661.4,512.0],'half':[66,64],'rings':[],'rays':0},
{'id':'NTMK','title':'Gate to TTC to market road + side yards','center':[752,329],'half':[52,109],'rings':[],'rays':0},
{'id':'LAKE_E','title':'East shore road/footway; bank distinguished','center':[813,355],'half':[47,185],'rings':[],'rays':0},
{'id':'GARDEN_JOIN','title':'SW bridge-end adjoining garden walking layers','center':[656,531],'half':[52,51],'rings':[],'rays':0},
{'id':'LANGFARM','title':'TTC Langfarm adjoining surface','center':[728,299],'half':[16,21],'rings':[],'rays':0}]
sections=[]
def section(region,name,pp):
 p=np.array(pp);ds=np.r_[0,np.cumsum(np.linalg.norm(np.diff(p,axis=0),axis=1))];sections.append({'region':region,'name':name,'xz':p.tolist(),'distance':ds.tolist()})
for reg in REGIONS:
 cx,cz=reg['center'];hx,hz=reg['half'];rid=reg['id']
 for offset in [-.6,0,.6]:
  section(rid,f'X_zoff{offset}',np.c_[np.linspace(cx-hx,cx+hx,int(hx*4)+1),np.full(int(hx*4)+1,cz+offset*hz)])
  section(rid,f'Z_xoff{offset}',np.c_[np.full(int(hz*4)+1,cx+offset*hx),np.linspace(cz-hz,cz+hz,int(hz*4)+1)])
 for rad in reg['rings']:
  aa=np.deg2rad(np.arange(0,360.01,2));section(rid,f'ring_{rad}',np.c_[cx+rad*np.cos(aa),cz+rad*np.sin(aa)])
 if reg['rays']:
  L=reg['rays']
  for a in range(0,360,30):
   t=np.linspace(0,L,int(L*2)+1);section(rid,f'ray_{a}',np.c_[cx+t*np.cos(np.deg2rad(a)),cz+t*np.sin(np.deg2rad(a))])
# Exact Langfarm local directions; d outward, s north along facade.
origin=np.array([713.656372,306.396576]);u=np.array([.22935149,-.97334367]);n=np.array([.97334367,.22935149])
for s in [0,5,10.4,15,20.8]:
 dd=np.linspace(7.5,23,63);section('LANGFARM',f'cross_s{s}',origin+u*s+n*dd[:,None])
for dd in [8,11.5,15]:
 ss=np.linspace(0,20.8,85);section('LANGFARM',f'along_d{dd}',origin+n*dd+u*ss[:,None])
v=np.array([714.94-610.67,459.48-563.99]);L=np.linalg.norm(v);u=v/L;n=np.array([-u[1],u[0]]);center=np.array([610.67,563.99])
for off in [-8,-5,0,5,8]:
 tt=np.linspace(-25,L+25,int((L+50)*2)+1);section('BRIDGE',f'deck_parallel{off}',center+u*tt[:,None]+n*off)
# Dense plan grid covers roads/side yards, not just pre-existing circular masks.
xs=np.arange(450,871,1.);zs=np.arange(40,611,1.);xx,zz=np.meshgrid(xs,zs);grid=np.c_[xx.ravel(),zz.ravel()];allpts=np.vstack([grid]+[np.array(s['xz']) for s in sections]);lens=[len(grid)]+[len(s['xz']) for s in sections];cuts=np.cumsum([0]+lens);points=shapely.points(allpts)
use=[r for r in stats if r.get('cache') and r['active'] and (r['role']!='other' or r['path'].split('/')[-1] in ['Surface_127','Surface_134','Surface_149'] or r['path'].endswith(('Surface_045','Surface_046','Surface_048','Surface_049')) or r['path'].startswith('R11_Local_Bridge_Supports/')) and 'Construction_Edges/' not in r['path']]
summ=[];layersec=[];grids={}
for idx,r in enumerate(use):
 z=np.load(D/(r['cache']+'.npz'));vv=z['v'];tri=z['tri'];q=vv[tri];a,b,c=q[:,0],q[:,1],q[:,2];den=(b[:,2]-c[:,2])*(a[:,0]-c[:,0])+(c[:,0]-b[:,0])*(a[:,2]-c[:,2]);ok=abs(den)>1e-9;q=q[ok];a,b,c=q[:,0],q[:,1],q[:,2];den=den[ok]
 if not len(q):continue
 poly=shapely.polygons(q[:,:,[0,2]]);tree=STRtree(poly);pair=tree.query(points);pi,ti=pair;x,y=allpts[pi].T;u=((b[ti,2]-c[ti,2])*(x-c[ti,0])+(c[ti,0]-b[ti,0])*(y-c[ti,2]))/den[ti];v=((c[ti,2]-a[ti,2])*(x-c[ti,0])+(a[ti,0]-c[ti,0])*(y-c[ti,2]))/den[ti];valid=(u>=-1e-7)&(v>=-1e-7)&(u+v<=1.0000001);pi,ti,u,v=pi[valid],ti[valid],u[valid],v[valid];h=u*a[ti,1]+v*b[ti,1]+(1-u-v)*c[ti,1]
 top=np.full(len(allpts),-np.inf);low=np.full(len(allpts),np.inf);np.maximum.at(top,pi,h);np.minimum.at(low,pi,h);top[~np.isfinite(top)]=np.nan;low[~np.isfinite(low)]=np.nan
 grids[r['cache']]=top[:len(grid)].astype('f4').reshape(xx.shape)
 for reg in REGIONS:
  cx,cz=reg['center'];hx,hz=reg['half'];sel=np.flatnonzero((abs(grid[:,0]-cx)<hx)&(abs(grid[:,1]-cz)<hz)&np.isfinite(top[:len(grid)]));h1=top[sel]
  if len(sel)<4:continue
  X=np.c_[np.ones(len(sel)),grid[sel]-[cx,cz]];coef=np.linalg.lstsq(X,h1,rcond=None)[0];res=h1-X@coef
  summ.append({'region':reg['id'],'path':r['path'],'role':r['role'],'samples':len(sel),'y_minmax':[float(h1.min()),float(h1.max())],'plane_y_dx_dz':coef.tolist(),'tilt_percent':float(np.linalg.norm(coef[1:])*100),'residual_range':list(map(float,[res.min(),res.max()])),'same_mesh_multiple_height_samples':int(np.sum(top[sel]-low[sel]>.02))})
 for k,s in enumerate(sections):
  lo,hi=cuts[k+1],cuts[k+2];h1=top[lo:hi];valid=np.isfinite(h1)
  if valid.sum()<2:continue
  loc=np.flatnonzero(valid);runs=np.split(loc,np.flatnonzero(np.diff(loc)>1)+1);segments=[]
  for ids in runs:
   if len(ids)<2:continue
   dist=np.array(s['distance'])[ids];hh=h1[ids];length=float(dist[-1]-dist[0]);delta=float(hh[-1]-hh[0]);g=np.diff(hh)/np.maximum(np.diff(dist),1e-9)*100
   segments.append({'i0':int(ids[0]),'i1':int(ids[-1]),'length':length,'delta':delta,'grade_percent':100*delta/max(length,1e-9),'local_grade_range':list(map(float,[g.min(),g.max()])),'max_adjacent_grade_change_percent':float(np.abs(np.diff(g)).max(initial=0))})
  vals=[None if not np.isfinite(x) else round(float(x),6) for x in h1];lowvals=[None if not np.isfinite(x) else round(float(x),6) for x in low[lo:hi]];layersec.append({'section':k,'path':r['path'],'role':r['role'],'top':vals,'bottom':lowvals,'segments':segments})
 print(idx+1,len(use),r['path'],flush=True)
(D/'regions.json').write_text(json.dumps(REGIONS,indent=2));(D/'sections.json').write_text(json.dumps(sections));(D/'layer-sections.json').write_text(json.dumps(layersec));(D/'region-layer-summary.json').write_text(json.dumps(summ,indent=2));np.savez_compressed(D/'layer-grid.npz',xs=xs,zs=zs,**grids)
print('MEASURED',len(use),'layers',len(sections),'sections;',len(layersec),'layer sections')
