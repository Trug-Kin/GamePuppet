from pathlib import Path
import json,hashlib,math,sys
sys.path.insert(0,'Tools/python-deps')
import numpy as np
from PIL import Image,ImageDraw,ImageFont
R=Path('D:/ConRoi');D=R/'Test/KetQua/journey_v48/bank_r97';M=R/'Test/BangChung/journey_v48/bank_r97';a=json.loads((D/'author-result.json').read_text());co=np.array(a['coast']);shore=lambda z:float(np.interp(z,co[:,0],co[:,1]));font=ImageFont.truetype('C:/Windows/Fonts/arial.ttf',18);small=ImageFont.truetype('C:/Windows/Fonts/arial.ttf',15)
im=Image.new('RGB',(1200,1030),'#f3f3eb');dr=ImageDraw.Draw(im)
def p(x,z):return (round(80+(755-x)*7.5),round(55+(z-400)*7.5))
water=json.load(open(R/'Test/KetQua/journey_v48/market_ground_r95/protected-actual-water.geojson'))['coordinates'][0]
dr.polygon([p(*q) for q in water],fill='#8ab5c0');dr.line([p(650,528),p(690,491),p(714,474),p(731,455),p(731,425)],fill='#b4b6b7',width=30)
colors=['#d3c6ae','#95ad68','#d3c6ae','#95ad68','#d3c6ae'];ds=[(0,1.8),(1.8,5.8),(5.8,7.3),(7.3,13),(13,16)]
zs=np.arange(418,483.01,.25)
for (lo,hi),color in zip(ds,colors):dr.polygon([p(shore(z)+lo,z) for z in zs]+[p(shore(z)+hi,z) for z in zs[::-1]],fill=color)
for z in [429,450,471]:
 for lo,hi in [(1.8,5.8),(7.3,13)]:
  dr.polygon([p(shore(z)+lo,z-1.1),p(shore(z)+hi,z-1.1),p(shore(z)+hi,z+1.1),p(shore(z)+lo,z+1.1)],fill='#626566')
 dr.text(p(shore(z)+18,z),f'S{z}',fill='black',font=small)
for x,z,label in [(714.94,459.48,'I2 unchanged'),(746,422,'Gate/clock unchanged'),(657.26,512.21,'Bao unchanged'),(680,501,'Ong Dao bridge')]:
 pt=p(x,z);dr.ellipse((pt[0]-4,pt[1]-4,pt[0]+4,pt[1]+4),fill='#a44235');dr.text((pt[0]+8,pt[1]),label,fill='black',font=small)
dr.text((25,14),'R97 gate-side grass bank | plan in Unity authoring metres',fill='black',font=font)
dr.line((1100,180,1100,70),fill='black',width=3);dr.polygon([(1100,60),(1094,78),(1106,78)],fill='black');dr.text((1089,37),'N',fill='black',font=font);dr.text((965,203),'East = -Unity X',fill='black',font=small)
dr.rectangle((*p(700,411),*p(640,492)),outline='#c97524',width=2)
dr.text((25,850),'Orange envelope: Surface142 winding-only repair at joins; no height displacement.',fill='#a45b14',font=small)
dr.text((25,880),'Green: new grass banks. Tan: upper / intermediate / lower paths. Grey: real stepped flights.',fill='black',font=font)
dr.text((25,910),'Scope: Z418..483, original shoreline to +16m inland. Water, dam, bridge and market entry retained.',fill='black',font=small)
dr.text((25,940),'Photo-backed topology; dimensions and exact stair/rail stations estimated for the existing game datum.',fill='black',font=small)
dr.text((25,970),'M2 footprint UNVERIFIED. This plan does not certify the entire shore or survey accuracy.',fill='black',font=small)
im.save(M/'scope-plan.png')
rows=[]
for f in sorted((R/'Test/BugModel').glob('*.png')):
 name=f.name;id=name[-10:-4];purpose={'181826':'Gate-side grass, two-tier stairs + intermediate walk','181952':'Same gate-side camera, bridge-head curve and upper walk','182010':'Independent view, repeated two-tier stairs and gate/hotel landmarks','181809':'Concrete dam apron distinct from grass; short rail return context','181347':'Opposite overview: dam + shoreline, do not copy one section everywhere','181402':'Opposite curved bank; not evidence for every grass-bank railing','181524':'Bridge upper sidewalk + true balustrade; preserve Bao contact','181852':'Across-water context, bank + upper road','181919':'Opposite/wider bank, lower path/quay without universal rail','181938':'Hotel/pottery-side rail and single slope; not universal two-tier bank'}[id]
 rows.append(dict(file=str(f.relative_to(R)),sha256=hashlib.file_digest(f.open('rb'),'sha256').hexdigest(),purpose=purpose,original_capture_date='UNKNOWN; filename is screenshot date, not original capture',metric_datum='NONE',orientation='Landmark-relative; exact camera calibration UNVERIFIED'))
(D/'photo-scope-index.json').write_text(json.dumps(rows,indent=2),encoding='utf-8')
print('RESULT plan/photo index')
