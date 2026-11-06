import json,re,sys
from pathlib import Path
sys.path.insert(0,'Tools/python-deps')
import numpy as np
from PIL import Image,ImageDraw,ImageFont
D=Path('Test/KetQua/journey_v48/market_ground_r95');M=Path('Test/BangChung/journey_v48/market_ground_r95');g=json.loads((D/'grading.json').read_text());font=ImageFont.truetype('C:/Windows/Fonts/arial.ttf',18);small=ImageFont.truetype('C:/Windows/Fonts/arial.ttf',15)
im=Image.new('RGB',(1800,1450),'#f8f8f5');dr=ImageDraw.Draw(im)
def px(p):return (80+(1000-p[0])*2.2,100+(p[1]-55)*2.2)
for row in g['objects']:
 if row['key'].startswith('M95_F') or '/MarketIsland' in row['path']:continue
 a=np.load(D/(row['key']+'-source.npz'));v=a['v'];changed=abs(v[:,1]-a['before'][:,1])>.005;inside=(v[:,0]>445)&(v[:,0]<1005)&(v[:,2]>55)&(v[:,2]<585)
 for p in v[inside][::max(1,int(inside.sum()/10000))]:dr.point(px(p[[0,2]]),fill='#bbc0c2')
 for p in v[changed&inside][::max(1,int((changed&inside).sum()/30000))]:dr.point(px(p[[0,2]]),fill='#d88535' if p[2]>320 else '#5eae82')
for x in range(450,1001,50):dr.line([px([x,55]),px([x,580])],fill='#dddddd');dr.text(px([x,52]),str(x),font=small,fill='black')
for z in range(100,581,50):dr.line([px([450,z]),px([1000,z])],fill='#dddddd');dr.text((15,px([1000,z])[1]),str(z),font=small,fill='black')
dr.line([px(p) for p in [[704,59],[860,59],[860,320],[704,320],[704,59]]],fill='#185a90',width=3)
dr.line([px(p) for p in [[704,320],[860,320],[860,435],[704,435]]],fill='#b86a15',width=3)
dr.rectangle([px([704,390]),px([400,580])],outline='#526777',width=3)
for tag,col in [('controller','#6b6b9a'),('branch-final','#005520'),('north-final','#005520')]:
 p=D/(tag+'-motion.txt')
 if p.exists():
  pts=[]
  for line in p.read_text().splitlines():
   m=re.search(r'^t=([\d.]+).*?p=\(([-\d.]+), ([-\d.]+), ([-\d.]+)\)',line)
   if m and (tag!='controller' or float(m[1])<290):pts.append(px([float(m[2]),float(m[4])]))
  if len(pts)>1:dr.line(pts,fill=col,width=3)
for label,p in [('I2 r94 protected',(714.94,459.48)),('Clock / gate',(746,422)),('M2 SEARCH (not I3)',(700,407)),('Langfarm / TTC apron',(727,300)),('Service split',(781,265)),('I3 statue ring',(783.948,191.167)),('North boundary',(830,90))]:
 x,y=px(p);dr.ellipse((x-4,y-4,x+4,y+4),fill='black');dr.text((x+7,y-5),label,font=small,fill='black',stroke_width=2,stroke_fill='white')
dr.text((20,15),'R95 - SCOPED GAME FLOOR DESIGN / UNITY INTEGRATED - not surveyed elevations',font=font,fill='black')
dr.text((1340,90),'N = -Unity Z\nE = -Unity X\nEqual plan scale\nGrid 50 authored m\n\nGreen changed: north/core\nOrange: approach/transition\nGrey: other source surfaces\nBlue: strict flat-core audit\nBrown: approach envelope\n\nDark green: final local\ncontroller test tracks\nPurple: initial route before\nlocal winding repair (to290s)\n\nProtected bank/dam box\nis outside grading. Only\nobserved support winding\nrepaired, no height change.\n\nM2 photo/object identity OPEN.\nActual branch floor included.\nNo invented island.\n\nCoverage is sampled; not\nwhole-map acceptance.',font=font,fill='black')
dr.line((1430,700,1430,620),fill='black',width=4);dr.polygon([(1430,610),(1420,630),(1440,630)],fill='black');dr.text((1420,585),'N',font=font,fill='black')
dr.text((80,1350),'Core Y5.04; approach Z320..435: delta -4.32 / 115m, eased ends. I2 r94 and shore contour preserved.',font=font,fill='black');im.save(M/'design-scope-and-change-full-extent.png')
defs=json.loads((D/'sections.json').read_text());layers=json.loads((D/'section-layers.json').read_text());summary=[]
for s in defs:
 if not any(x in s['name'] for x in ['north-boundary','north-full-width','clock-','M2-search','Langfarm-cross']):continue
 rows=[r for r in layers if r['section']==s['name']];vals=[y for r in rows for y in r['heights'] if y is not None]
 if not vals:continue
 lo=min(vals)-.2;hi=max(vals)+.2;im=Image.new('RGB',(1450,760),'white');dr=ImageDraw.Draw(im);dr.text((30,15),s['name']+' | length '+str(round(s['length'],3))+' authored m | direction '+str(s['direction']),font=font,fill='black')
 for r in rows:
  pts=[]
  for i,y in enumerate(r['heights']):
   if y is None:
    if len(pts)>1:dr.line(pts,fill='#bd3636' if r['state']=='before' else '#14713c',width=2)
    pts=[];continue
   pts.append((85+1270*i/(len(r['heights'])-1),610-(y-lo)/(hi-lo)*520))
  if len(pts)>1:dr.line(pts,fill='#bd3636' if r['state']=='before' else '#14713c',width=2)
  valid=[(i,y) for i,y in enumerate(r['heights']) if y is not None]
  if valid:summary.append(dict(section=s['name'],layer=r['layer'],state=r['state'],length=s['length'],direction=s['direction'],first=valid[0],last=valid[-1],max_sampled_grade=r['max_sampled_grade']))
 for i in range(6):
  y=lo+(hi-lo)*i/5;yy=610-i/5*520;dr.line((80,yy,1360,yy),fill='#dddddd');dr.text((5,yy),f'{y:.2f}',font=small,fill='black');dr.text((80+1270*i/5,630),f'{s["length"]*i/5:.1f}',font=small,fill='black')
 dr.text((80,680),'Y: authored m. X: distance m. Red before / green after. Each layer sampled independently at <=0.25m.',font=font,fill='black');dr.text((80,715),'Gaps in a curve mean that layer has no surface there; graph is not a merged highest-hit surface.',font=small,fill='black');im.save(M/(s['name']+'-before-after.png'))
(D/'final-section-summary.json').write_text(json.dumps(summary,indent=2));print('Final plan and section figures',len(summary))
