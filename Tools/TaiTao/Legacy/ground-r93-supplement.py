import sys,json,math
from pathlib import Path
sys.path.insert(0,'Tools/python-deps')
import numpy as np,shapely
from shapely import STRtree
from PIL import Image,ImageDraw,ImageFont
D=Path('Test/KetQua/journey_v48/ground_r93');M=Path('Test/BangChung/journey_v48/ground_r93');a=json.loads((D/'geometry-inventory.json').read_text());sections=[]
def line(name,p,q,step=.25):
 p=np.array(p);q=np.array(q);L=np.linalg.norm(q-p);t=np.linspace(0,1,int(math.ceil(L/step))+1);sections.append(dict(name=name,start=p.tolist(),end=q.tolist(),length=float(L),distance=(t*L).tolist(),xz=(p+(q-p)*t[:,None]).tolist()))
# P02 planted median area; bounds are survey window, not asserted island footprint.
for off in [-12,0,12]:line('M1-long-offset'+str(off),[746+off,370],[746+off,435])
for z in [380,395,410,425]:line('M1-cross-z'+str(z),[720,z],[775,z])
# Broad forecourts and east/west verges beyond road-mask branches.
for x in [731,756,813,836]:line('market-side-X'+str(x),[x,125],[x,270])
for z in [135,160,215,245]:line('market-cross-Z'+str(z),[710,z],[850,z])
# Each bridge end across full envelope, independently of deck rails.
u=np.array([714.94-610.67,459.48-563.99]);u/=np.linalg.norm(u);n=np.array([-u[1],u[0]])
for c,tag in [(np.array([610.67,563.99]),'I1'),(np.array([714.94,459.48]),'I2')]:
 for t in [-20,-8,8,20]:line(tag+'-bridge-end-cross'+str(t),c+u*t-n*24,c+u*t+n*24)
points=np.vstack([s['xz'] for s in sections]);cuts=np.cumsum([0]+[len(s['xz']) for s in sections]);pp=shapely.points(points);out=[];topo=[]
names=['Surface_104','Surface_125','Surface_127','Surface_128','Surface_129','Surface_134','Surface_142','Surface_153','Surface_173','Surface_174','Surface_175','Surface_176','RoadLoop_125314010_R65']
use=[r for r in a if r.get('cache') and r['active'] and (r['role'] in ['collision','terrain','footway','road','island','bridge/island'] or r['path'].split('/')[-1] in names)]
for r in use:
 z=np.load(D/(r['cache']+'.npz'));q=z['v'][z['tri']].astype('f8');aa,bb,cc=q[:,0],q[:,1],q[:,2];nn=np.cross(bb-aa,cc-aa);ar=np.linalg.norm(nn,axis=1)/2;pa=abs(nn[:,1])/2;edge=np.maximum.reduce([np.linalg.norm(bb-aa,axis=1),np.linalg.norm(cc-aa,axis=1),np.linalg.norm(cc-bb,axis=1)]);alt=2*ar/np.maximum(edge,1e-15);cent=q.mean(1);scope=(cent[:,0]>450)&(cent[:,0]<1000)&(cent[:,2]>40)&(cent[:,2]<610)
 topo.append(dict(path=r['path'],role=r['role'],triangles_in_window=int(scope.sum()),zero_area=int((scope&(ar<1e-10)).sum()),thin_altitude_under_0_001_and_edge_over_1=int((scope&(alt<.001)&(edge>1)).sum()),downward_nonvertical=int((scope&(nn[:,1]<-.001)&(abs(nn[:,1])>.5*np.linalg.norm(nn,axis=1))).sum()),note='Thin/downward candidates require face/material review: banks, undersides and curb sides are not classified as faults from counts.'))
 den=(bb[:,2]-cc[:,2])*(aa[:,0]-cc[:,0])+(cc[:,0]-bb[:,0])*(aa[:,2]-cc[:,2]);ok=abs(den)>1e-9;aa,bb,cc,den=aa[ok],bb[ok],cc[ok],den[ok]
 if not len(den):continue
 tree=STRtree(shapely.polygons(np.stack([aa,bb,cc],axis=1)[:,:,[0,2]]));pi,ti=tree.query(pp);x,y=points[pi].T;u=((bb[ti,2]-cc[ti,2])*(x-cc[ti,0])+(cc[ti,0]-bb[ti,0])*(y-cc[ti,2]))/den[ti];v=((cc[ti,2]-aa[ti,2])*(x-cc[ti,0])+(aa[ti,0]-cc[ti,0])*(y-cc[ti,2]))/den[ti];ok=(u>=-1e-7)&(v>=-1e-7)&(u+v<=1.0000001);pi,ti,u,v=pi[ok],ti[ok],u[ok],v[ok];h=u*aa[ti,1]+v*bb[ti,1]+(1-u-v)*cc[ti,1];top=np.full(len(points),-np.inf);low=np.full(len(points),np.inf);np.maximum.at(top,pi,h);np.minimum.at(low,pi,h)
 for k,s in enumerate(sections):
  hh=top[cuts[k]:cuts[k+1]];ll=low[cuts[k]:cuts[k+1]];ids=np.flatnonzero(np.isfinite(hh));runs=np.split(ids,np.flatnonzero(np.diff(ids)>1)+1);seg=[]
  for ii in runs:
   if len(ii)<2:continue
   ds=np.array(s['distance'])[ii];ht=hh[ii];L=ds[-1]-ds[0];gr=np.diff(ht)/np.diff(ds)*100
   seg.append(dict(i0=int(ii[0]),i1=int(ii[-1]),length=float(L),delta=float(ht[-1]-ht[0]),grade_percent=float((ht[-1]-ht[0])/L*100),local_grade_range=[float(gr.min()),float(gr.max())]))
  if not seg:continue
  out.append(dict(section=k,path=r['path'],segments=seg,top=[float(t) if np.isfinite(t) else None for t in hh],bottom=[float(t) if np.isfinite(t) else None for t in ll]))
(D/'supplement-sections.json').write_text(json.dumps(sections));(D/'supplement-layers.json').write_text(json.dumps(out));(D/'topology-candidates.json').write_text(json.dumps(topo,indent=2))
print('SUPPLEMENT',len(sections),'sections;',len(out),'layer sections;',len(topo),'topology layers')
# Update scope with explicit median window; no false assertion that its boundary is known.
im=Image.open(M/'scope-north-up.png');dr=ImageDraw.Draw(im);font=ImageFont.truetype('C:/Windows/Fonts/arial.ttf',18)
def pix(x,z):return (120+(870-x)*2.95,160+(z-40)*2.95)
p=pix(768,370);q=pix(724,435);dr.rectangle((p[0],p[1],q[0],q[1]),outline='#bd6c08',width=4);dr.text((p[0]-220,p[1]-28),'M1 P02 planted median / survey window',font=font,fill='#7b4200',stroke_width=2,stroke_fill='white');dr.text((1410,550),'M1 footprint: UNVERIFIED',font=font,fill='#a43b2d');dr.text((1410,580),'Broad side aprons measured',font=font,fill='black');im.save(M/'scope-north-up-with-median.png')
# Remove non-standard NaN tokens from earlier numerical report, retaining null for absent coverage.
f=D/'road-terrain-overlap.json';old=json.loads(f.read_text())
def clean(v):
 if isinstance(v,float) and not math.isfinite(v):return None
 if isinstance(v,list):return [clean(x) for x in v]
 if isinstance(v,dict):return {k:clean(x) for k,x in v.items()}
 return v
f.write_text(json.dumps(clean(old),indent=2,allow_nan=False))
