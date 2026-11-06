import sys,json,math
from pathlib import Path
sys.path.insert(0,'Tools/python-deps')
import numpy as np
from PIL import Image,ImageDraw,ImageFont
D=Path('Test/KetQua/journey_v48/ground_r93');M=Path('Test/BangChung/journey_v48/ground_r93');a=json.loads((D/'geometry-inventory.json').read_text());inv={r['path']:r for r in a};g=np.load(D/'layer-grid.npz');ss=json.loads((D/'sections.json').read_text());ls=json.loads((D/'layer-sections.json').read_text());branches=json.loads((D/'branches.json').read_text());F=ImageFont.truetype('C:/Windows/Fonts/arial.ttf',25);S=ImageFont.truetype('C:/Windows/Fonts/arial.ttf',18);T=ImageFont.truetype('C:/Windows/Fonts/arial.ttf',15)
roadnames=['Surface_104','Surface_127','Surface_128','Surface_129','Surface_134','Surface_173','RoadLoop_125314010_R65'];footnames=['Surface_125','Surface_153','Surface_174','Surface_175'];W=1900;H=2050;im=Image.new('RGB',(W,H),'#f4f6f7');dr=ImageDraw.Draw(im)
def pix(x,z):return (120+(870-x)*2.95,160+(z-40)*2.95)
for names,col in [(['Surface_176'],'#e2dfd6'),(['Surface_142'],'#d3ddc8'),(footnames,'#c7b993'),(roadnames,'#7197b2')]:
 for r in a:
  if not r['active'] or not r['render'] or not r['path'].startswith('R20_Whole_Source_Export/') or r['path'].split('/')[-1] not in names or r.get('cache') not in g:continue
  iy,ix=np.where(np.isfinite(g[r['cache']]))
  for y,x in zip(iy,ix):
   p=pix(g['xs'][x],g['zs'][y]);dr.rectangle((p[0],p[1],p[0]+3,p[1]+3),fill=col)
# Bridge deck extent is a separate class, never inferred from terrain or rail bounds.
for r in a:
 if r['path'].split('/')[-1] not in ['OD35_COLLIDER_0#COLLIDER','OD35_COLLIDER_1#COLLIDER','OD35_COLLIDER_3#COLLIDER','OD35_COLLIDER_9#COLLIDER'] or r.get('cache') not in g:continue
 iy,ix=np.where(np.isfinite(g[r['cache']]))
 for y,x in zip(iy,ix):
  p=pix(g['xs'][x],g['zs'][y]);dr.rectangle((p[0],p[1],p[0]+3,p[1]+3),fill='#9279a6')
for b in branches:
 x,z=b['center'];aa=math.radians(b['angle_from_unity_positive_x_deg']);u=np.array([math.cos(aa),math.sin(aa)]);p=np.array([x,z]);q=p+u*b['outer'];dr.line((pix(*(p+u*b['inner'])),pix(*q)),fill='#214e37',width=2);dr.text(pix(*q),b['id'],font=T,fill='#0a3323',stroke_width=2,stroke_fill='white')
for k,s in enumerate(ss):
 if s['name'] not in ['X_zoff0','Z_xoff0','deck_parallel0','cross_s10.4']:continue
 if s['region'] in ['BRIDGE','GARDEN_JOIN','LAKE_E']:continue
 dr.line([pix(*p) for p in s['xz']],fill='#686868',width=1)
for tag,x,z,r,txt in [('I1',610.67,563.99,7,'Opposite end / source ESTIMATED'),('I2',714.94,459.48,13.2,'Bridge-gate island / P02'),('I3',783.948,191.167,20.4,'Market statue island / P03')]:
 p=pix(x,z);rr=r*2.95;dr.ellipse((p[0]-rr,p[1]-rr,p[0]+rr,p[1]+rr),outline='#46633b',width=3);dr.text((p[0]+30,p[1]-50),tag+' '+txt,font=S,fill='black',stroke_width=3,stroke_fill='white')
for txt,x,z in [('Ong Dao bridge',665,511),('Gate / NTMK',749,418),('TTC / Langfarm',726,299),('Market forecourt / side yards',778,150),('Local terrain / bank / Garden joins',649,535)]:
 p=pix(x,z);dr.text(p,txt,font=S,fill='#222',stroke_width=3,stroke_fill='white')
dr.text((40,25),'r93 GROUND SCOPE / measured geometry; surveyed heights UNKNOWN',font=F,fill='black');dr.text((40,65),'Blue road | tan walking layers | pale core/local terrain | purple bridge deck collider',font=S,fill='black');dr.text((40,95),'9 road-mask mouths are geometric probes, not a complete verified real-world branch inventory.',font=S,fill='black')
dr.line((1640,260,1640,150),fill='black',width=4);dr.polygon([(1640,135),(1629,159),(1651,159)],fill='black');dr.text((1628,105),'N',font=F,fill='black');dr.text((1450,290),'N = -Unity Z; E = -Unity X',font=S,fill='black');dr.text((1450,325),'per native UTM metadata',font=S,fill='black');dr.text((1450,365),'Extent X450..870 / Z40..610',font=S,fill='black');dr.text((1450,405),'Numerical sections: measured',font=S,fill='#214e37');dr.text((1450,440),'Controller coverage: NOT_RUN',font=S,fill='#a43b2d');dr.text((1450,475),'Product terrain correction:',font=S,fill='black');dr.text((1450,505),'not yet accepted/integrated',font=S,fill='#a43b2d')
p=pix(714.94,473.73);dr.ellipse((p[0]-10,p[1]-10,p[0]+10,p[1]+10),outline='red',width=4);dr.line((p,(1400,1100)),fill='red',width=2);dr.text((1400,1080),'I2 local road kink:',font=F,fill='#a42620');dr.text((1400,1120),'0.292 / 0.500 = 58.4%',font=S,fill='#a42620');dr.text((1400,1155),'Same render / collider height',font=S,fill='black')
dr.text((70,1900),'Model scale only: 1 Unity unit authored as 1 m. No local survey validation.',font=S,fill='black');p=pix(850,595);q=pix(800,595);dr.line((p,q),fill='black',width=4);dr.text((p[0],p[1]+10),'50 authoring m',font=S,fill='black');im.save(M/'scope-north-up.png')
prefix='R20_Whole_Source_Export/';bridge='OngDao_V35_Photo_Batch/'
picks=[('I1_SW','X_zoff0',['Surface_128','Surface_142','Surface_176']),('I2_NE','ray_90',['Surface_128','OD35_Surface_0','Context_110#COLLIDER','R22_Island_Deck_Collider#COLLIDER']),('I3_MARKET','X_zoff0',['RoadLoop_125314010_R65','Surface_176','Restored_028','Restored_029','Context_124#COLLIDER']),('I3_MARKET','Z_xoff0',['RoadLoop_125314010_R65','Surface_176','Restored_028','Restored_029','Context_124#COLLIDER']),('BRIDGE','deck_parallel0',['Surface_142','Surface_176','OD35_Surface_2','OD35_COLLIDER_0#COLLIDER','OD35_COLLIDER_1#COLLIDER','OD35_COLLIDER_9#COLLIDER']),('NTMK','X_zoff0',['Surface_173','Surface_176','Surface_174','Surface_175','Context_124#COLLIDER']),('LANGFARM','cross_s10.4',['Surface_176','Context_124#COLLIDER']),('GARDEN_JOIN','X_zoff0',['Surface_142','Surface_176','Surface_125','Surface_153'])]
colors=['#905a25','#195daf','#1c8b5a','#d54988','#6356b0','#444444','#c28318'];out=[]
for rid,name,selected in picks:
 k=next(i for i,s in enumerate(ss) if s['region']==rid and s['name']==name);s=ss[k];lines=[r for r in ls if r['section']==k and r['path'].split('/')[-1] in selected and not r['path'].startswith('RouteSurfaces/')];length=s['distance'][-1]
 if rid=='I2_NE':limit=28
 else:limit=length
 vals=[v for r in lines for d,v in zip(s['distance'],r['top']) if v is not None and d<=limit];lo=min(vals)-.15;hi=max(vals)+.15;im=Image.new('RGB',(1800,1050),'white');dr=ImageDraw.Draw(im)
 def xy(x,y):return (100+x/limit*1580,700-(y-lo)/(hi-lo)*530)
 dr.text((35,22),rid+' / '+name+' : separate ground layers, r93 BEFORE',font=F,fill='black');dr.text((35,65),'Start XZ '+str(np.round(s['xz'][0],3).tolist())+'  end XZ '+str(np.round(s['xz'][-1],3).tolist()),font=S,fill='black');dr.text((35,98),'Horizontal distance / world height in authoring m. Vertical scale exaggerated; not a surveyed section.',font=S,fill='black')
 for j in range(7):
  y=lo+(hi-lo)*j/6;p=xy(0,y);dr.line((100,p[1],1680,p[1]),fill='#ddd');dr.text((10,p[1]-8),f'{y:.3f}',font=T,fill='black')
 for j in range(6):
  x=limit*j/5;p=xy(x,lo);dr.line((p[0],170,p[0],700),fill='#eee');dr.text((p[0]-20,710),f'{x:.1f}',font=T,fill='black')
 for j,r in enumerate(lines):
  c=colors[j%len(colors)];run=[]
  for x,y in zip(s['distance'],r['top']):
   if x>limit:break
   if y is None:
    if len(run)>1:dr.line(run,fill=c,width=3)
    run=[]
   else:run.append(xy(x,y))
  if len(run)>1:dr.line(run,fill=c,width=3)
  seg=max(r['segments'],key=lambda q:q['length'],default=None);summary=''
  if seg:
   i0,i1=seg['i0'],seg['i1'];summary=f"L {seg['length']:.2f}; y {r['top'][i0]:.3f}->{r['top'][i1]:.3f}; dY {seg['delta']:+.3f}; mean {seg['grade_percent']:+.2f}%"
  dr.text((40,770+j*35),r['path'].split('/')[-1]+' | '+summary,font=S,fill=c)
 out.append(dict(region=rid,section=name,start_xz=s['xz'][0],end_xz=s['xz'][-1],layers=[dict(path=r['path'],segments=r['segments']) for r in lines]));im.save(M/(rid+'-'+name+'-ground-only.png'))
(D/'atlas-sections.json').write_text(json.dumps(out,indent=2))
print('ATLAS',len(out),'sections + complete scoped plan')
