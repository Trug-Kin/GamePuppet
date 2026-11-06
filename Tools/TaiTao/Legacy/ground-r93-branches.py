import sys,json,math
from pathlib import Path
sys.path.insert(0,'Tools/python-deps')
import numpy as np,shapely
from shapely import STRtree
D=Path('Test/KetQua/journey_v48/ground_r93');stats=json.loads((D/'geometry-inventory.json').read_text());roadnames=['Surface_104','Surface_127','Surface_128','Surface_129','Surface_134','Surface_173','RoadLoop_125314010_R65']
road=[]
for r in stats:
 if r['path'].startswith('R20_Whole_Source_Export/') and r['path'].split('/')[-1] in roadnames:
  z=np.load(D/(r['cache']+'.npz'));q=z['v'][z['tri']][:,:,[0,2]];road.append(shapely.union_all(shapely.polygons(q)))
road=shapely.union_all(road)
sections=[];branches=[]
def section(rid,name,p):
 p=np.array(p);dist=np.r_[0,np.cumsum(np.linalg.norm(np.diff(p,axis=0),axis=1))];sections.append(dict(region=rid,name=name,xz=p.tolist(),distance=dist.tolist()))
for rid,cx,cz,rad,inner,outer in [('I1',610.67,563.99,23,7,40),('I2',714.94,459.48,32,13,55),('I3',783.948,191.167,34,20,60)]:
 aa=np.deg2rad(np.arange(0,360,.25));pp=np.c_[cx+rad*np.cos(aa),cz+rad*np.sin(aa)];mask=shapely.covers(road,shapely.points(pp));starts=np.flatnonzero(mask&~np.roll(mask,1));groups=[]
 for start in starts:
  ids=[];k=int(start)
  while mask[k%len(mask)] and len(ids)<len(mask):ids.append(k);k+=1
  mid=(ids[0]+ids[-1])/2*.25;groups.append((mid,len(ids)*.25))
 for j,(angle,span) in enumerate(groups):
  u=np.array([math.cos(math.radians(angle)),math.sin(math.radians(angle))]);n=np.array([-u[1],u[0]]);cen=np.array([cx,cz]);width=min(28,2*rad*math.sin(math.radians(span/2))+6)
  bid=rid+'-B'+str(j+1);branches.append(dict(id=bid,center=[cx,cz],angle_from_unity_positive_x_deg=angle,probe_radius=rad,mouth_arc_deg=span,measured_width=width,inner=inner,outer=outer))
  for off in [-width*.4,0,width*.4]:
   dd=np.arange(inner,outer+.001,.25);section(rid,bid+'-long_'+str(round(off,2)),cen+u*dd[:,None]+n*off)
  for dd in [rad-5,rad,rad+5]:
   tt=np.linspace(-width/2,width/2,int(width*4)+1);section(rid,bid+'-cross_r'+str(dd),cen+u*dd+n*tt[:,None])
print('BRANCH_MOUTHS',json.dumps(branches),flush=True)
# Exact triangle heights per distinct source layer; do not fuse layers.
pts=np.vstack([s['xz'] for s in sections]);points=shapely.points(pts);cuts=np.cumsum([0]+[len(s['xz']) for s in sections]);out=[]
use=[r for r in stats if r.get('cache') and r['active'] and (r['role'] in ['terrain','road','footway','collision'] or r['path'].split('/')[-1] in ['Surface_127','Surface_134','Surface_142'])]
for r in use:
 z=np.load(D/(r['cache']+'.npz'));q=z['v'][z['tri']];a,b,c=q[:,0],q[:,1],q[:,2];den=(b[:,2]-c[:,2])*(a[:,0]-c[:,0])+(c[:,0]-b[:,0])*(a[:,2]-c[:,2]);ok=abs(den)>1e-9;a,b,c,den=a[ok],b[ok],c[ok],den[ok]
 if not len(den):continue
 tree=STRtree(shapely.polygons(np.stack([a,b,c],axis=1)[:,:,[0,2]]));pi,ti=tree.query(points);x,y=pts[pi].T;u=((b[ti,2]-c[ti,2])*(x-c[ti,0])+(c[ti,0]-b[ti,0])*(y-c[ti,2]))/den[ti];v=((c[ti,2]-a[ti,2])*(x-c[ti,0])+(a[ti,0]-c[ti,0])*(y-c[ti,2]))/den[ti];ok=(u>=-1e-7)&(v>=-1e-7)&(u+v<=1.0000001);pi,ti,u,v=pi[ok],ti[ok],u[ok],v[ok];h=u*a[ti,1]+v*b[ti,1]+(1-u-v)*c[ti,1];top=np.full(len(pts),-np.inf);low=np.full(len(pts),np.inf);np.maximum.at(top,pi,h);np.minimum.at(low,pi,h)
 for k,s in enumerate(sections):
  hh=top[cuts[k]:cuts[k+1]];lo=low[cuts[k]:cuts[k+1]];ids=np.flatnonzero(np.isfinite(hh));segs=[]
  if len(ids)<2:continue
  for ix in np.split(ids,np.flatnonzero(np.diff(ids)>1)+1):
   if len(ix)<2:continue
   ds=np.array(s['distance'])[ix];yy=hh[ix];gr=np.diff(yy)/np.diff(ds)*100;X=np.c_[np.ones(len(ix)),ds];fit=np.linalg.lstsq(X,yy,rcond=None)[0];res=yy-X@fit
   segs.append(dict(i0=int(ix[0]),i1=int(ix[-1]),length=float(ds[-1]-ds[0]),delta=float(yy[-1]-yy[0]),grade_percent=float((yy[-1]-yy[0])/(ds[-1]-ds[0])*100),grade_range=gr[[gr.argmin(),gr.argmax()]].tolist(),plane_residual_range=[float(res.min()),float(res.max())],max_grade_kink=float(abs(np.diff(gr)).max(initial=0))))
  out.append(dict(section=k,path=r['path'],top=[None if not np.isfinite(v) else round(float(v),6) for v in hh],bottom=[None if not np.isfinite(v) else round(float(v),6) for v in lo],segments=segs))
(D/'branches.json').write_text(json.dumps(branches,indent=2));(D/'branch-sections.json').write_text(json.dumps(sections));(D/'branch-layers.json').write_text(json.dumps(out));print('DONE',len(branches),len(sections),len(out),flush=True)
