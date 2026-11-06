from pathlib import Path
import sys,json,hashlib,math
sys.path.insert(0,'Tools/python-deps')
import numpy as np
from PIL import Image,ImageDraw,ImageFont
R=Path('D:/ConRoi');D=R/'Test/KetQua/journey_v48/dam_r98';M=R/'Test/BangChung/journey_v48/dam_r98'
def font(n):return ImageFont.truetype('C:/Windows/Fonts/arial.ttf',n)
def base(w,h,title,subtitle):
 im=Image.new('RGB',(w,h),'#f3f5f6');dr=ImageDraw.Draw(im);dr.text((28,18),title,font=font(30),fill='#152c39');dr.text((28,60),subtitle,font=font(18),fill='#425667');return im,dr
info=json.loads((D/'author-result.json').read_text());a=np.array(info['basis']['A']);t=np.array(info['basis']['T']);n=np.array(info['basis']['N']);L=info['basis']['L']
im,dr=base(1400,920,'R98 — Dam / end stairs / R97 tie','Plan: authoring units in metres, estimated adaptation; north = -Unity Z, east = -Unity X')
def p(q):return (int(800+(660-q[0])*10),int(125+(q[1]-475)*10))
water=np.array(json.loads((R/'Test/KetQua/journey_v48/market_ground_r95/protected-actual-water.geojson').read_text())['coordinates'][0]);dr.polygon([p(q) for q in water],fill='#afd5df');dr.rectangle((0,0,1400,95),fill='#f3f5f6');dr.text((28,18),'R98 — Dam / end stairs / R97 tie',font=font(30),fill='#152c39');dr.text((28,60),'Plan: estimated authoring metres; north=-Unity Z, east=-Unity X',font=font(18),fill='#425667')
for q in info['footprint']:dr.polygon([p(v) for v in q],fill='#c8b898',outline='#6f624a',width=2)
for i in range(14):
 s=L*i/13;q=a+t*s;dr.line([p(q),p(q+n*9.6)],fill='#776c58',width=2)
for lo,hi in [(-3,0),(L,L+3)]:
 q=[a+t*lo,a+t*hi,a+t*hi+n*10.5,a+t*lo+n*10.5];dr.polygon([p(v) for v in q],fill='#d8995a',outline='#70441f',width=2)
 for k in range(8):dr.line([p(a+t*lo+n*(1+k)),p(a+t*hi+n*(1+k))],fill='#674a31',width=1)
dr.line([p(a),p(a+t*L)],fill='#2b465e',width=5);dr.text(p(a+np.array([2,4])),'FAR stair',font=font(19),fill='#4c321c');dr.text(p(a+t*L+np.array([1,-3])),'GATE stair',font=font(19),fill='#4c321c')
path=json.loads((D/'route-plan.json').read_text())['path'];dr.line([p(q) for q in path],fill='#b82d52',width=2)
dr.text((35,135),'Scope / ownership',font=font(25),fill='#152c39')
for i,txt in enumerate(['Sand: concrete apron / 13 estimated bays','Orange: two end stairs + low landings','Blue edge: preserved bridge / main rail','Pink: planned controller route; see final log','Gate end: R97 low walk Z480..483 tie','Old sheets cut within the precise footprint','Water / bridge deck / Bao contacts retained','Outside footprint: not certified by this review']):dr.text((35,180+i*35),txt,font=font(19),fill='#334b58')
dr.line([(70,640),(70,560)],fill='#152c39',width=4);dr.polygon([(70,550),(60,569),(80,569)],fill='#152c39');dr.text((55,520),'N',font=font(23),fill='#152c39')
dr.line([(40,730),(140,730)],fill='#152c39',width=4);dr.text((40,745),'10 authoring m (not survey)',font=font(17),fill='#334b58');im.save(M/'scope-plan.png')
sections=json.loads((D/'sections-before-after.json').read_text());im,dr=base(1500,1250,'R98 — identical section locations, before / after','Horizontal 0..13 authoring m; vertical scale exaggerated 3x. Separate layers shown; water Y0 unchanged.')
colors={'BankR91_Grade_017':'#938c77','BankR91_Context125_R91':'#b086a5','C95_0040_OD35_Surface_0':'#516c8f','R98_CONCRETE_DAM_13_BAYS':'#bf7e25','R98_FAR_END_STAIR_AND_LOW_LANDING':'#b44821','R98_GATE_END_STAIR_AND_LOW_LANDING':'#b44821'}
for j,s in enumerate([-1.5,10.,25.,40.,L+1.5]):
 x0=90;y0=140+j*195;w=1280;h=135
 dr.rectangle((x0,y0,x0+w,y0+h),outline='#cad1d5');dr.text((x0,y0-25),f's={s:.3f} along bridge; direction toward lake N=({n[0]:.3f},{n[1]:.3f})',font=font(17),fill='#233b49')
 def xy(d,y):return (int(x0+(d+1)/14*w),int(y0+h-(y+1)*42))
 dr.line([xy(-1,0),xy(13,0)],fill='#32859b',width=2)
 for tick in range(0,14,2):dr.text((xy(tick,0)[0],y0+h+2),str(tick),font=font(13),fill='#465763')
 for rev in ['before','after']:
  row=next(q for q in sections if q['rev']==rev and abs(q['s']-s)<.001)
  for name,color in colors.items():
   if rev=='before' and name.startswith('R98'):continue
   if rev=='after' and not name.startswith('R98'):continue
   segment=[]
   for q in row['samples']:
    if name in q['layers']:
     segment.append(xy(q['d'],q['layers'][name]))
    else:
     if len(segment)>1:dr.line(segment,fill=color,width=3 if rev=='after' else 1)
     segment=[]
   if len(segment)>1:dr.line(segment,fill=color,width=3 if rev=='after' else 1)
dr.text((90,1175),'Thin: original render/collider sheets. Thick orange/red: final apron/stair upper surface. Blue horizontal: water.',font=font(18),fill='#334b58');im.save(M/'sections-before-after.png')
for src,tag in [('181347','across-lake'),('181809','apron-oblique')]:
 im,dr=base(1500,450,'Photo → R97 → R98','Source direction is approximate, not calibrated camera matching. Locked Unity before/after eye, target and FOV60.')
 for j,(file,label) in enumerate([(R/f'Test/BugModel/Screenshot 2026-11-02 {src}.png','Original '+src),(M/f'JourneyPresentation_v48_r97-{tag}.png','Before — r97'),(M/f'JourneyPresentation_v48_r98-{tag}.png','After — r98')]):
  pic=Image.open(file).convert('RGB');pic.thumbnail((480,300));im.paste(pic,(10+j*500,120));dr.text((15+j*500,90),label,font=font(20),fill='#233b49')
 im.save(M/f'photo-before-after-{src}.jpg',quality=94)
rows=[]
for pth in sorted((R/'Test/BugModel').glob('*.png')):rows.append(dict(path=str(pth.relative_to(R)),sha256=hashlib.file_digest(pth.open('rb'),'sha256').hexdigest(),role='181809 close dam;181347/181402 independent across lake;181524 bridge deck; others distinguish grass-bank layers',date='Screenshot filename date only; original recording date UNKNOWN',dimensions='No reliable survey/metre datum'))
(D/'photo-source-index.json').write_text(json.dumps(rows,indent=2));print('RESULT plan, same-section plots, two photo comparisons, ten photo hashes')
