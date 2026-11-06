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
def clean(a):return [None if not np.isfinite(v) else float(v) for v in a]
sections=[]
def line(tag,p,q):
 p=np.array(p);q=np.array(q);L=np.linalg.norm(q-p);t=np.linspace(0,1,int(math.ceil(L/.10))+1);sections.append(dict(name=tag,xz=(p+(q-p)*t[:,None]).tolist(),distance=(t*L).tolist()))
for a in range(-15,196,15):
 u=np.array([math.cos(math.radians(a)),math.sin(math.radians(a))]);n=np.array([-u[1],u[0]])
 for off in [-2,0,2]:line('radial'+str(a)+'-width'+str(off),[cx,cz]+u*12+n*off,[cx,cz]+u*52+n*off)
for z in [472,474,476,480,484,488,494,500,506]:line('transverse-z'+str(z),[665,z],[765,z])
line('diagonal-east',[700,469],[735,487]);line('diagonal-west',[732,469],[695,489])
for rr in [14,16,18,21,24,27,30,32,36,40,44,48]:
 aa=np.linspace(0,math.pi,541);p=np.c_[cx+rr*np.cos(aa),cz+rr*np.sin(aa)];sections.append(dict(name='ring'+str(rr),xz=p.tolist(),distance=(aa*rr).tolist()))
allp=np.vstack([s['xz'] for s in sections]);cuts=np.cumsum([0]+[len(s['xz']) for s in sections]);xx,zz=np.meshgrid(np.arange(665,765.01,.2),np.arange(449.48,511.51,.2));grid=np.c_[xx.ravel(),zz.ravel()];pts=np.vstack([allp,grid]);rows=[];grids={};surfs={}
for item in report['objects']:
 a=np.load(D/(item['key']+'-source.npz'));b=np.load(D/(item['native_key']+'.npz'));v,tri=a['v'].astype(float),a['tri'];row=dict(path=item['path'],key=item['key']);after=Surf(v,tri);before=Surf(b['v'].astype(float),b['tri']);surfs[item['path']]=after
 for tag,surf in [('before',before),('after',after)]:
  top,low=surf.query(pts);row[tag+'_self_multiple_heights']=int((np.isfinite(top)&np.isfinite(low)&(abs(top-low)>.001)).sum());ss=[]
  for k,s in enumerate(sections):
   h=top[cuts[k]:cuts[k+1]];ds=np.array(s['distance']);gr=np.diff(h)/np.diff(ds)*100;valid=np.isfinite(gr);ss.append(dict(section=k,top=clean(h),max_abs_grade=float(np.max(abs(gr[valid]),initial=0))))
  row[tag+'_sections']=ss;grids[item['key']+'_'+tag]=top[len(allp):].reshape(xx.shape)
 q=v[tri];n=np.cross(q[:,1]-q[:,0],q[:,2]-q[:,0]);area=np.linalg.norm(n,axis=1)/2;pa=abs(n[:,1])/2;cen=q.mean(1);rr=np.linalg.norm(cen[:,[0,2]]-[cx,cz],axis=1);sel=(rr<32)&(cen[:,2]>cz)&(pa>1e-8);grade=np.linalg.norm(n[:,[0,2]],axis=1)/np.maximum(abs(n[:,1]),1e-15)*100
 row['after_geometry']=dict(degenerate_local=int(((area<1e-10)&(rr<32)&(cen[:,2]>cz)).sum()),max_face_grade_area_over_0_01=float(grade[sel&(pa>.01)].max(initial=0)),max_face_grade_any=float(grade[sel].max(initial=0)),area_over_10=float(pa[sel&(grade>10)].sum()),area_over_20=float(pa[sel&(grade>20)].sum()),sliver_locations=[dict(center=cen[i].tolist(),grade=float(grade[i]),area=float(pa[i])) for i in np.flatnonzero(sel&(grade>20))[:20]])
 rows.append(row);print(item['key'],row['after_geometry'],flush=True)
np.savez_compressed(D/'measurement-grid.npz',xs=xx[0],zs=zz[:,0],**grids);(D/'sections.json').write_text(json.dumps(sections));(D/'layer-measurements.json').write_text(json.dumps(rows));pairs=[]
for rn,cn in [('Surface_128','Context_110'),('Surface_129','Context_111'),('Surface_153','Context_136'),('Surface_176','Context_124')]:
 r=next(x for x in report['objects'] if x['path'].endswith('/'+rn));c=next(x for x in report['objects'] if x['path'].endswith('/'+cn));rh=grids[r['key']+'_after'];ch=grids[c['key']+'_after'];ok=np.isfinite(rh)&np.isfinite(ch);delta=abs(rh[ok]-ch[ok]);pairs.append(dict(render=rn,collider=cn,samples=int(ok.sum()),max_error=float(delta.max(initial=0))))
(D/'render-collider-measured.json').write_text(json.dumps(pairs,indent=2));print('COLLIDER',pairs)
F=ImageFont.truetype('C:/Windows/Fonts/arial.ttf',22);S=ImageFont.truetype('C:/Windows/Fonts/arial.ttf',16)
for name in ['radial90-width0','radial60-width0','radial120-width0','transverse-z474','transverse-z480','diagonal-east']:
 k=next(i for i,s in enumerate(sections) if s['name']==name);s=sections[k];vals=[v for row in rows if not row['path'].startswith('LakeGateMarket') for t in ['before','after'] for v in row[t+'_sections'][k]['top'] if v is not None];lo=min(vals)-.1;hi=max(vals)+.1;L=s['distance'][-1];im=Image.new('RGB',(1700,1000),'white');dr=ImageDraw.Draw(im)
 def pix(d,h):return (100+d/L*1480,650-(h-lo)/(hi-lo)*470)
 dr.text((30,20),'I2 FULL-WIDTH / '+name+' | same XZ before / after Blender',font=F,fill='black');dr.text((30,58),'Authoring metres; vertical exaggeration. Existing island/bridge unchanged. Not surveyed grade.',font=S,fill='black')
 for j in range(6):
  h=lo+(hi-lo)*j/5;dr.line((100,pix(0,h)[1],1580,pix(0,h)[1]),fill='#ddd');dr.text((15,pix(0,h)[1]-8),f'{h:.3f}',font=S,fill='black')
 palette={'Surface_128':'#1565ad','Surface_129':'#8d32a6','Surface_153':'#198545','Surface_142':'#a66c25','Surface_176':'#c24a40'}
 for i,row in enumerate([r for r in rows if not r['path'].startswith('LakeGateMarket')]):
  n=row['path'].split('/')[-1];col=palette[n]
  for tag in ['before','after']:
   hh=row[tag+'_sections'][k]['top'];run=[]
   for d,h in zip(s['distance'],hh):
    if h is None:
     if len(run)>1:dr.line(run,fill=col if tag=='after' else '#bbb',width=3 if tag=='after' else 1)
     run=[]
    else:run.append(pix(d,h))
   if len(run)>1:dr.line(run,fill=col if tag=='after' else '#bbb',width=3 if tag=='after' else 1)
  dr.text((30+i%3*540,730+i//3*32),n+' colored after; gray before',font=S,fill=col)
 dr.text((100,670),'0',font=S,fill='black');dr.text((1500,670),f'{L:.2f} m',font=S,fill='black');dr.text((30,850),'From '+str(s['xz'][0])+' to '+str(s['xz'][-1]),font=S,fill='black');im.save(M/(name+'-before-after.png'))
