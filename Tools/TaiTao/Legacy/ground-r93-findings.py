import sys,json,math
from pathlib import Path
sys.path.insert(0,'Tools/python-deps')
import numpy as np,shapely
from shapely import STRtree
from PIL import Image,ImageDraw,ImageFont
D=Path('Test/KetQua/journey_v48/ground_r93');M=Path('Test/BangChung/journey_v48/ground_r93');stats=json.loads((D/'geometry-inventory.json').read_text());sec=json.loads((D/'sections.json').read_text());ls=json.loads((D/'layer-sections.json').read_text());inv={r['path']:r for r in stats};g=np.load(D/'layer-grid.npz');xx,zz=np.meshgrid(g['xs'],g['zs']);F=ImageFont.truetype('C:/Windows/Fonts/arial.ttf',23);S=ImageFont.truetype('C:/Windows/Fonts/arial.ttf',16)
def grid(path):return g[inv[path]['cache']]
terrain=grid('R20_Whole_Source_Export/Surface_176');results=[]
for p in ['R20_Whole_Source_Export/Surface_104','R20_Whole_Source_Export/Surface_127','R20_Whole_Source_Export/Surface_128','R20_Whole_Source_Export/Surface_129','R20_Whole_Source_Export/Surface_134','R20_Whole_Source_Export/Surface_173','R20_Whole_Source_Export/RoadLoop_125314010_R65']:
 road=grid(p);dif=terrain-road;valid=np.isfinite(dif);buried=valid&(dif>.05);bad=valid&(dif>.15);yy,xxi=np.where(bad);coords=np.c_[g['xs'][xxi],g['zs'][yy],dif[bad]];order=np.argsort(coords[:,2])[::-1] if len(coords) else []
 r={'path':p,'overlap_samples':int(valid.sum()),'terrain_above_road_5cm':int(buried.sum()),'terrain_above_road_15cm':int(bad.sum()),'delta_range':list(map(float,[np.nanmin(dif),np.nanmax(dif)])),'worst_xz_delta':coords[order[:10]].tolist()};results.append(r)
(D/'road-terrain-overlap.json').write_text(json.dumps(results,indent=2));print(json.dumps(results,indent=2)[:5500])
# Road-only plan, slope per raster face neighborhood, keep terrain/road deltas independent.
road=np.full(terrain.shape,np.nan)
for r in stats:
 if r.get('cache') and r['active'] and r['path'].startswith('R20_Whole_Source_Export/') and (r['role']=='road' or r['path'].endswith(('Surface_127','Surface_134'))):road=np.fmax(road,g[r['cache']])
im=Image.new('RGB',(1800,1600),'#eef2f4');d=ImageDraw.Draw(im)
def pix(x,z):return (120+(870-x)*3.1,120+(z-40)*2.4)
for iy,z in enumerate(g['zs']):
 for ix,x in enumerate(g['xs']):
  if not np.isfinite(road[iy,ix]):continue
  delta=terrain[iy,ix]-road[iy,ix];c='#d22b2b' if delta>.15 else '#e8ab3e' if delta>.05 else '#6f96b3';p=pix(x,z);d.rectangle((p[0],p[1],p[0]+4,p[1]+3),fill=c)
for label,x,z in [('I1 opposite bridge end',610.67,563.99),('I2 bridge / gate',714.94,459.48),('I3 market',783.948,191.167),('TTC / Langfarm',724,299)]:
 p=pix(x,z);d.text((p[0]+15,p[1]),label,font=F,fill='black',stroke_width=2,stroke_fill='white')
d.text((35,25),'Road layers: blue | terrain > road +5cm amber / +15cm red (model units)',font=F,fill='black');d.text((35,65),'North up (-Unity Z); east right (-Unity X), per native UTM offset convention. Not a surveyed layout.',font=S,fill='black')
d.line((1650,230,1650,130),fill='black',width=4);d.polygon([(1650,120),(1641,140),(1659,140)],fill='black');d.text((1640,90),'N',font=F,fill='black');im.save(M/'road-terrain-overlap-plan.png')
# Compact critical close-sections for all 3 junctions, both bridge ends, side aprons.
pick=[('I1_SW','X_zoff0'),('I2_NE','X_zoff0'),('I3_MARKET','X_zoff0'),('I3_MARKET','Z_xoff0'),('BRIDGE','deck_parallel0'),('NTMK','X_zoff0'),('LANGFARM','cross_s10.4'),('GARDEN_JOIN','X_zoff0')]
palette={'terrain':'#a67842','road':'#1555aa','footway':'#1c9e63','collision':'#cc3c8c','bridge/island':'#7259a5','ground-candidate':'#888888','island':'#555555'}
for rid,name in pick:
 k=next(i for i,s in enumerate(sec) if s['region']==rid and s['name']==name);s=sec[k];lines=[r for r in ls if r['section']==k and (r['role'] in ['terrain','road','footway','bridge/island'] or (r['role']=='collision' and any(n in r['path'] for n in ['Context_124','Context_110','Context_108','OD35_COLLIDER_0','OD35_COLLIDER_1','OD35_COLLIDER_9','Context_111'])))];vals=[v for r in lines for v in r['top'] if v is not None];ymin=min(vals)-.2;ymax=max(vals)+.2;L=s['distance'][-1];im=Image.new('RGB',(1700,900),'white');d=ImageDraw.Draw(im)
 def xy(t,y):return (100+t/L*1500,650-(y-ymin)/(ymax-ymin)*520)
 d.text((30,20),rid+' / '+name+' | actual independent layers BEFORE r93',font=F,fill='black');d.text((30,55),'x=distance (Unity/model units); y=world height. Absolute surveyed grade UNKNOWN.',font=S,fill='black')
 for j in range(6):
  v=ymin+j*(ymax-ymin)/5;p=xy(0,v);d.line((100,p[1],1600,p[1]),fill='#ddd');d.text((15,p[1]-8),f'{v:.2f}',font=S,fill='black')
 for i,r in enumerate(lines):
  pp=[];c=palette.get(r['role'],'black')
  for x,y in zip(s['distance'],r['top']):
   if y is None:
    if len(pp)>1:d.line(pp,fill=c,width=2)
    pp=[]
   else:pp.append(xy(x,y))
  if len(pp)>1:d.line(pp,fill=c,width=2)
  if i<15:d.text((30+(i%3)*550,710+(i//3)*29),r['path'].split('/')[-1],font=S,fill=c)
 d.text((100,665),'0',font=S,fill='black');d.text((1510,665),f'{L:.2f}',font=S,fill='black');im.save(M/(rid+'-'+name+'-before.png'))
# Exact native-v33 comparison at Langfarm 121 current sample locations.
z=np.load(D/'native-v33-terrain.npz');q=z['v'][z['tri']];a,b,c=q[:,0],q[:,1],q[:,2];den=(b[:,2]-c[:,2])*(a[:,0]-c[:,0])+(c[:,0]-b[:,0])*(a[:,2]-c[:,2]);ok=abs(den)>1e-9;q=q[ok];a,b,c=q[:,0],q[:,1],q[:,2];den=den[ok];tree=STRtree(shapely.polygons(q[:,:,[0,2]]));old=json.loads(Path('Test/KetQua/journey_v48/r93/render-collider-grid.json').read_text())['samples'];origin=np.array([713.656372,306.396576]);u=np.array([.22935149,-.97334367]);n=np.array([.97334367,.22935149]);pp=np.array([origin+u*r['s']+n*r['d'] for r in old]);pi,ti=tree.query(shapely.points(pp));x,y=pp[pi].T;u=((b[ti,2]-c[ti,2])*(x-c[ti,0])+(c[ti,0]-b[ti,0])*(y-c[ti,2]))/den[ti];v=((c[ti,2]-a[ti,2])*(x-c[ti,0])+(a[ti,0]-c[ti,0])*(y-c[ti,2]))/den[ti];valid=(u>=-1e-7)&(v>=-1e-7)&(u+v<=1.0000001);pi,ti,u,v=pi[valid],ti[valid],u[valid],v[valid];h=u*a[ti,1]+v*b[ti,1]+(1-u-v)*c[ti,1];top=np.full(len(pp),-np.inf);np.maximum.at(top,pi,h);out=[{'s':r['s'],'d':r['d'],'xz':pp[i].tolist(),'native_v33_y':float(top[i]),'r93_y':r['render'][0],'delta':float(r['render'][0]-top[i])} for i,r in enumerate(old)];(D/'langfarm-native-v33-comparison.json').write_text(json.dumps(out,indent=2));print('Langfarm native v33 maximum difference',max(abs(r['delta']) for r in out))
