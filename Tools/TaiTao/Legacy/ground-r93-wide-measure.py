import sys,json
from pathlib import Path
sys.path.insert(0,'Tools/python-deps')
import numpy as np,shapely
from shapely import STRtree
from PIL import Image,ImageDraw,ImageFont
D=Path('Test/KetQua/journey_v48/ground_r93');M=Path('Test/BangChung/journey_v48/ground_r93');report=json.loads((D/'wide-transition-study.json').read_text());ss=json.loads((D/'sections.json').read_text());old=json.loads((D/'layer-sections.json').read_text());pts=np.vstack([s['xz'] for s in ss]);points=shapely.points(pts);cuts=np.cumsum([0]+[len(s['xz']) for s in ss]);out=[]
for r in report['objects']:
 z=np.load(D/('wide-study-'+r['object']+'.npz'));q=z['v'][z['tri']];a,b,c=q[:,0],q[:,1],q[:,2];den=(b[:,2]-c[:,2])*(a[:,0]-c[:,0])+(c[:,0]-b[:,0])*(a[:,2]-c[:,2]);ok=abs(den)>1e-9;a,b,c,den=a[ok],b[ok],c[ok],den[ok];tree=STRtree(shapely.polygons(np.stack([a,b,c],axis=1)[:,:,[0,2]]));pi,ti=tree.query(points);x,y=pts[pi].T;u=((b[ti,2]-c[ti,2])*(x-c[ti,0])+(c[ti,0]-b[ti,0])*(y-c[ti,2]))/den[ti];v=((c[ti,2]-a[ti,2])*(x-c[ti,0])+(a[ti,0]-c[ti,0])*(y-c[ti,2]))/den[ti];ok=(u>=-1e-7)&(v>=-1e-7)&(u+v<=1.0000001);pi,ti,u,v=pi[ok],ti[ok],u[ok],v[ok];h=u*a[ti,1]+v*b[ti,1]+(1-u-v)*c[ti,1];top=np.full(len(pts),-np.inf);np.maximum.at(top,pi,h)
 for k,s in enumerate(ss):
  h=top[cuts[k]:cuts[k+1]];valid=np.isfinite(h);ids=np.flatnonzero(valid)
  if len(ids)<2:continue
  gr=np.diff(h)/np.diff(s['distance'])*100;gr[~np.isfinite(gr)]=np.nan
  out.append(dict(section=k,path=r['path'],top=[None if not np.isfinite(v) else round(float(v),6) for v in h],grade_range=[float(np.nanmin(gr)),float(np.nanmax(gr))] if np.isfinite(gr).any() else None))
(D/'wide-study-layer-sections.json').write_text(json.dumps(out));checks=[];font=ImageFont.truetype('C:/Windows/Fonts/arial.ttf',22);small=ImageFont.truetype('C:/Windows/Fonts/arial.ttf',17)
for name in ['ray_0','ray_60','ray_90','ray_120','ray_150','ray_180','ray_240','ray_270','ray_300','ring_15','ring_18','ring_22']:
 k=next(i for i,s in enumerate(ss) if s['region']=='I2_NE' and s['name']==name);s=ss[k];bb=next((r for r in old if r['section']==k and r['path']=='R20_Whole_Source_Export/Surface_128'),None);aa=next((r for r in out if r['section']==k and r['path']=='R20_Whole_Source_Export/Surface_128'),None)
 if not aa or not bb:continue
 dist=np.array(s['distance']);yb=np.array([np.nan if v is None else v for v in bb['top']]);ya=np.array([np.nan if v is None else v for v in aa['top']]);sel=(dist<=28) if name.startswith('ray_') else np.ones(len(dist),bool);gb=np.diff(yb)/np.diff(dist)*100;ga=np.diff(ya)/np.diff(dist)*100;valid=np.isfinite(gb)&np.isfinite(ga)&sel[:-1];checks.append(dict(section=name,before_grade_range=[float(np.min(gb[valid])),float(np.max(gb[valid]))],trial_grade_range=[float(np.min(ga[valid])),float(np.max(ga[valid]))],max_height_change=float(np.nanmax(abs(ya-yb)))))
 vals=np.r_[yb[sel],ya[sel]];vals=vals[np.isfinite(vals)];lo=vals.min()-.1;hi=vals.max()+.1;L=dist[sel].max();im=Image.new('RGB',(1500,780),'white');dr=ImageDraw.Draw(im)
 def xy(x,y):return (100+x/L*1300,600-(y-lo)/(hi-lo)*440)
 dr.text((25,20),'I2 / '+name+' | SAME SECTION : r93 before vs Blender trial (NOT integrated)',font=font,fill='black');dr.text((25,60),'Authoring metres; vertical scale exaggerated. Fixed island/bridge; estimated inner-road relation.',font=small,fill='black')
 for j in range(6):
  y=lo+(hi-lo)*j/5;p=xy(0,y);dr.line((100,p[1],1400,p[1]),fill='#ddd');dr.text((15,p[1]-7),f'{y:.3f}',font=small,fill='black')
 for hh,c in [(yb,'#bb4532'),(ya,'#1f8a63')]:
  run=[]
  for x,y in zip(dist[sel],hh[sel]):
   if not np.isfinite(y):
    if len(run)>1:dr.line(run,fill=c,width=3)
    run=[]
   else:run.append(xy(x,y))
  if len(run)>1:dr.line(run,fill=c,width=3)
 for j in range(6):dr.text((100+j*260,610),f'{L*j/5:.1f}',font=small,fill='black')
 dr.text((30,680),'Red r93 before | Green native Blender trial. No claim of surveyed grade.',font=font,fill='black');im.save(M/('I2-'+name+'-wide-study-comparison.png'))
(D/'wide-study-section-comparison.json').write_text(json.dumps(checks,indent=2));print(json.dumps(checks,indent=2))
