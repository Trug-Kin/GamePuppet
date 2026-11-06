import json,sys
from pathlib import Path
sys.path.insert(0,'Tools/python-deps')
import numpy as np
from PIL import Image,ImageDraw,ImageFont
D=Path('Test/KetQua/journey_v48/market_ground_r95');M=Path('Test/BangChung/journey_v48/market_ground_r95');g=json.loads((D/'grading.json').read_text());im=Image.new('RGB',(1850,1500),'#f6f6f3');dr=ImageDraw.Draw(im);font=ImageFont.truetype('C:/Windows/Fonts/arial.ttf',19);small=ImageFont.truetype('C:/Windows/Fonts/arial.ttf',15)
def px(p):return (100+(1000-p[0])*2.2,90+(p[1]-70)*2.2)
allchanged=[];bounds=[]
for row in g['objects']:
 if row['key'].startswith('M95_F') or '/MarketIsland' in row['path']:continue
 a=np.load(D/(row['key']+'-graded.npz'));v=a['v'];changed=abs(v[:,1]-a['before'][:,1])>.005;inplan=(v[:,0]>445)&(v[:,0]<1005)&(v[:,2]>65)&(v[:,2]<585)
 for p in v[inplan][::max(1,int(inplan.sum()/12000))]:dr.point(px(p[[0,2]]),fill='#b4babd')
 for p in v[changed&inplan][::max(1,int((changed&inplan).sum()/50000))]:dr.point(px(p[[0,2]]),fill='#c84230')
 if changed.any():
  allchanged.append(v[changed]);bounds.append(dict(path=row['path'],min=v[changed].min(0).tolist(),max=v[changed].max(0).tolist(),changed_vertices=int(changed.sum())))
box=np.vstack(allchanged);(D/'actual-changed-bounds.json').write_text(json.dumps(dict(units='authored metre / Unity unit, not surveyed',min=box.min(0).tolist(),max=box.max(0).tolist(),layers=bounds),indent=2))
for x in range(450,1001,50):
 dr.line([px([x,70]),px([x,580])],fill='#cccccc',width=1);dr.text(px([x,65]),str(x),font=small,fill='black')
for z in range(100,581,50):
 dr.line([px([450,z]),px([1000,z])],fill='#cccccc',width=1);dr.text((35,px([1000,z])[1]),str(z),font=small,fill='black')
core=[[704,165],[860,165],[860,435],[704,435],[704,165]];dr.line([px(p) for p in core],fill='#145787',width=3)
labels=[('I2 r94 / bridge checkpoint',(714.94,459.48)),('Gate: BOTH branches',(745,425)),('Median / yard full width',(745,390)),('L angfarm / TTC apron',(727,300)),('Split / service branches',(781,260)),('Market ring + plaza',(783.948,191.167))]
for text,p in labels:
 x,y=px(p);dr.ellipse((x-4,y-4,x+4,y+4),fill='black');dr.text((x+7,y-5),text,font=small,fill='black',stroke_width=2,stroke_fill='white')
dr.text((25,15),'R95 - ACTUAL SOURCE CHANGE EXTENT (red); game grade design, NOT field elevations',font=font,fill='black')
dr.text((1370,85),'N = -Unity Z\nE = -Unity X\n\nGrid: 50 authored m\nPlan has equal X/Z scale.\n\nRed: height changed >5mm\nGrey: source vertices\nBlue: full-face core audit\n\nUnity/controller coverage:\nNOT_RUN / not integrated\n\nBanks / dam / constructed\nstairs are separate.\n\nM2 label-to-object mapping\nnot yet independently verified;\nsplit and yard floor included.',font=font,fill='black')
dr.line((1440,580,1440,470),fill='black',width=4);dr.polygon([(1440,460),(1430,480),(1450,480)],fill='black');dr.text((1432,435),'N',font=font,fill='black')
dr.text((100,1300),'Changed bounds X/Y/Z: '+str(np.round(box.min(0),3).tolist())+' to '+str(np.round(box.max(0),3).tolist()),font=font,fill='black');dr.text((100,1340),'Whole-envelope transition/contact audit still pending; red is NOT a PASS coverage map.',font=font,fill='#9b201c');im.save(M/'design-scope-and-change-full-extent.png');print('WIDE_PLAN',box.min(0),box.max(0),flush=True)
