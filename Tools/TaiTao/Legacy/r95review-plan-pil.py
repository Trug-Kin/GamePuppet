from pathlib import Path
import sys,json,re
sys.path.insert(0,'Tools/python-deps')
import numpy as np
from PIL import Image,ImageDraw,ImageFont
D=Path('Test/KetQua/journey_v48/r95_visual_review');M=Path('Test/BangChung/journey_v48/r95_visual_review');B=Path('Test/KetQua/journey_v48/market_ground_r95')
font=ImageFont.truetype('C:/Windows/Fonts/arial.ttf',19)
im=Image.new('RGB',(1200,1650),'#fafaf7');dr=ImageDraw.Draw(im)
def px(p):return (90+(840-p[0])*3.9,90+(p[1]-140)*3.5)
for x in range(620,841,20):dr.line([px([x,140]),px([x,565])],fill='#dddddd');dr.text(px([x,137]),str(x),font=font,fill='black')
for z in range(150,566,25):dr.line([px([610,z]),px([840,z])],fill='#dddddd');dr.text((10,px([840,z])[1]),str(z),font=font,fill='black')
for key in ['M95_002','M95_019','M95_020','M95_021','M95_Service0']:
 a=np.load(B/(key+'-source.npz'));v=a['v'];sel=(v[:,0]>610)&(v[:,0]<840)&(v[:,2]>140)&(v[:,2]<565)
 for p in v[sel][::max(1,int(sel.sum()/15000))]:dr.point(px(p[[0,2]]),fill='#bfc4c2')
text=Path('Test/Tam/r95review-route-fixed.cs.txt').read_text();points=np.array([(float(a),float(b)) for a,b in re.findall(r'new Vector2\(([\d.]+)f,([\d.]+)f\)',text)]);points=np.vstack([[650,528],points]);dr.line([px(p) for p in points],fill='#197c66',width=4)
dr.rectangle([px([755,435]),px([680,505])],outline='#ba542a',width=4);dr.rectangle([px([720,390]),px([680,425])],outline='#b36aa3',width=2)
for name,x,z in [('Bridge fixture',650,528),('Fall / fixed',701.44,480.44),('I2 fountain',714.94,459.48),('Gate / clock',746,422),('Langfarm',724,306),('Market plaza / I3 (NOT M2)',783.948,191.167)]:
 p=px([x,z]);dr.ellipse([p[0]-4,p[1]-4,p[0]+4,p[1]+4],fill='black');dr.text((p[0]+8,p[1]+5),name,font=font,fill='black',stroke_width=2,stroke_fill='white')
dr.text((25,15),'R95 review / R96 winding fix. N=-UnityZ, E=-UnityX. Units: authoring m.',font=font,fill='black');dr.line([(1110,180),(1110,100)],fill='black',width=3);dr.polygon([(1110,90),(1102,108),(1118,108)],fill='black');dr.text((1102,62),'N',font=font,fill='black');dr.text((25,1590),'Green: route; orange: winding fix (height unchanged); purple: M2 UNVERIFIED.',font=font,fill='black');dr.text((25,1620),'Footprint context only; not field accuracy or whole-map approval.',font=font,fill='black');im.save(M/'review-route-and-fix-scope.png')
print('Plan ready')
