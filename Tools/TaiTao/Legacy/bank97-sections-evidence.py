from pathlib import Path
import json,sys,numpy as np
sys.path.insert(0,'Tools/python-deps')
from PIL import Image,ImageDraw,ImageFont
R=Path('D:/ConRoi');D=R/'Test/KetQua/journey_v48/bank_r97';M=R/'Test/BangChung/journey_v48/bank_r97';src=json.loads((D/'author-result.json').read_text());co=np.array(src['coast']);shore=lambda z:float(np.interp(z,co[:,0],co[:,1]));font=ImageFont.truetype('C:/Windows/Fonts/arial.ttf',18);small=ImageFont.truetype('C:/Windows/Fonts/arial.ttf',14)
before={p.stem.replace('-existing',''):np.load(p) for p in D.glob('*-existing.npz')};after={p.stem.replace('-export-positions',''):np.load(p) for p in D.glob('*-export-positions.npz')};summary=[];raw=[]
def series(data,z,xs):
 v=data['v'];t=data['tri'];q=v[t];q=q[(q[:,:,2].min(1)<=z+1e-7)&(q[:,:,2].max(1)>=z-1e-7)];out=[]
 for x in xs:
  r=q[(q[:,:,0].min(1)<=x+1e-7)&(q[:,:,0].max(1)>=x-1e-7)]
  if not len(r):out.append(None);continue
  a,b,c=r[:,0],r[:,1],r[:,2];den=(b[:,2]-c[:,2])*(a[:,0]-c[:,0])+(c[:,0]-b[:,0])*(a[:,2]-c[:,2]);valid=abs(den)>1e-10;a,b,c,den=a[valid],b[valid],c[valid],den[valid]
  u=((b[:,2]-c[:,2])*(x-c[:,0])+(c[:,0]-b[:,0])*(z-c[:,2]))/den;w=((c[:,2]-a[:,2])*(x-c[:,0])+(a[:,0]-c[:,0])*(z-c[:,2]))/den;ok=(u>=-1e-5)&(w>=-1e-5)&(u+w<=1.00001);h=u*a[:,1]+w*b[:,1]+(1-u-w)*c[:,1];out.append(float(h[ok].max()) if ok.any() else None)
 return out
for z in [422,429,450,471,480]:
 xs=np.linspace(shore(z)-.24,shore(z)+24,244);ds=xs-shore(z);layers={}
 keys=['M95_000','M95_002','M95_019','M95_022','M95_023','M95_016','M95_017','M97_Context125']
 for state,meshes in [('before',before),('after',after)]:
  for k in keys+([r['key'] for r in json.loads((D/'export-reopen.json').read_text())['rows'] if r['key'].startswith('R97_') and 'RAIL' not in r['key']] if state=='after' else []):
   if k not in meshes:continue
   yy=series(meshes[k],z,xs)
   if any(y is not None for y in yy):layers[state+' '+k]=yy
 raw.append(dict(z=z,direction='Unity +X across shore toward upper path',length_m=24.24,start_x=float(xs[0]),end_x=float(xs[-1]),d_m=ds.tolist(),layers=layers))
 im=Image.new('RGB',(1300,760),'white');dr=ImageDraw.Draw(im);dr.text((30,15),f'Cross section Z{z} | +Unity X landward | authoring metres, not survey',fill='black',font=font)
 for panel,state in enumerate(['before','after']):
  top=70+panel*330;dr.text((35,top),state.upper()+' | layers separate, no highest-surface merge',fill='black',font=small)
  def pt(d,y):return (70+int(d/24.24*1150),top+245-int((y+.5)*60))
  for h in [-.5,0,.5,1,1.5,2,2.5]:
   dr.line((70,pt(0,h)[1],1220,pt(0,h)[1]),fill='#dddddd');dr.text((22,pt(0,h)[1]-5),f'{h:g}',fill='black',font=small)
  for d in range(0,25,2):
   x=pt(d,0)[0];dr.line((x,top+40,x,top+280),fill='#eeeeee');dr.text((x,top+285),str(d),fill='black',font=small)
  colors=['#425fa8','#ce9137','#8d4c40','#3a8950','#8e61ac','#d0449e','#16858b','#9e9a37']
  j=0
  for name,yy in layers.items():
   if not name.startswith(state):continue
   color='#111111' if 'R97_' in name else colors[j%len(colors)];j+=1;seg=[]
   for d,h in zip(ds,yy):
    if h is None:
     if len(seg)>1:dr.line(seg,fill=color,width=2)
     seg=[]
    else:seg.append(pt(d,h))
   if len(seg)>1:dr.line(seg,fill=color,width=2)
  dr.text((75,top+305),'0=existing water edge; replacement to16m; winding-only join band16..24m. Vertical exaggeration ~1.25x.',fill='black',font=small)
 im.save(M/f'section-z{z}-before-after.png')
(D/'sections-before-after.json').write_text(json.dumps(raw,indent=2))
for name,a in after.items():
 if not name.startswith('R97_') or ('WALK' not in name):continue
 v=a['v'];t=a['tri'];q=v[t];n=np.cross(q[:,1]-q[:,0],q[:,2]-q[:,0]);c=q.mean(1);ok=(abs(n[:,1])>1e-8)&(c[:,2]>=422)&(c[:,2]<=480);slope=np.linalg.norm(n[ok][:,[0,2]],axis=1)/abs(n[ok][:,1])*100
 summary.append(dict(mesh=name,path_only_z=[422,480],max_triangle_slope_pct=float(slope.max()),p95_triangle_slope_pct=float(np.percentile(slope,95)),note='Surface inclination, not stair riser or route completion evidence'))
(D/'walk-slope-summary.json').write_text(json.dumps(summary,indent=2));print('RESULT sections',len(raw),summary)
