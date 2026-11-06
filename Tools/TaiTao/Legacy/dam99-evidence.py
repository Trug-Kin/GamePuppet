import sys,json,math,hashlib
from pathlib import Path
sys.path.insert(0,'Tools/python-deps')
from PIL import Image,ImageDraw,ImageFont
import numpy as np
R=Path('D:/ConRoi');D=R/'Test/KetQua/journey_v48/dam_r99';M=R/'Test/BangChung/journey_v48/dam_r99'
fontpath='C:/Windows/Fonts/arial.ttf'
def font(n):return ImageFont.truetype(fontpath,n)
for id in ['181347','181402']:
 canvas=Image.new('RGB',(1920,440),(245,245,245));dr=ImageDraw.Draw(canvas)
 dr.text((20,10),'R99 SECTION REVIEW - ESTIMATED DIMENSIONS, NOT A SURVEY',font=font(24),fill=(180,30,30))
 for i,(tag,path) in enumerate([('BugModel '+id,R/f'Test/BugModel/Screenshot 2026-11-02 {id}.png'),('R98 - same study camera',M/f'r98-matched-{id}.png'),('R99 source - same approximate camera',M/f'candidate-final-{id}.png')]):
  dr.text((i*640+12,45),tag,font=font(19),fill=(20,20,20));canvas.paste(Image.open(path).convert('RGB').resize((640,360)),(i*640,75))
 canvas.save(M/f'photo-r98-candidate-{id}.jpg',quality=92)
# Same-position real BVH section, equal scale on x and y axes.
rows=json.loads((D/'same-position-sections.json').read_text());im=Image.new('RGB',(1260,620),'white');dr=ImageDraw.Draw(im);scale=78;left=85;bottom=475
xy=lambda d,h:(left+d*scale,bottom-h*scale)
dr.text((30,12),'s = 25.00, lakeward section (+N). Equal axis scale, authoring units.',font=font(23),fill='black');dr.text((30,45),'R99 local design: estimated datums and proportions; cameras are approximate, not calibrated.',font=font(19),fill=(170,25,25))
for x in range(15):
 px=xy(x,0)[0];dr.line((px,90,px,520),fill=(225,225,225));dr.text((px-8,525),str(x),font=font(15),fill='black')
for y in [-.5,0,.5,1,1.5,2,2.5,3,3.5,4]:
 py=xy(0,y)[1];dr.line((65,py,1190,py),fill=(220,220,220));dr.text((12,py-9),str(y),font=font(15),fill='black')
summary=[]
for rev,color in [('r98',(30,90,160)),('r99',(200,65,20))]:
 row=next(r for r in rows if r['revision']==rev and r['s']==25);pts=[(p['d'],p['h']) for p in row['samples'] if p['h'] is not None];dr.line([xy(*p) for p in pts],fill=color,width=4);run=max(p[0] for p in pts);H=max(p[1] for p in pts);toe=min(pts,key=lambda p:-p[0])[1];summary.append(dict(rev=rev,station_s=25,sampled_run=run,crest_max=H,toe_last_sample=toe,sample_spacing=.05,warning='Run sampled to .05 grid; dimensions are game units, not a surveyed metre assertion'))
 dr.text((790,90 if rev=='r98' else 120),rev.upper()+': Hmax '+f'{H:.4f}'+', run '+f'{run:.2f}',font=font(18),fill=color)
dr.line((65,xy(0,0)[1],1190,xy(0,0)[1]),fill=(30,140,180),width=2)
dr.text((700,440),'Water datum unchanged (~Y0).',font=font(17),fill=(20,90,130))
dr.text((30,558),'R98: crest 1.140076 / toe .08 / run 9.51..11.01. Candidate: crest 3.00 / toe .08 / run 4.50.',font=font(18),fill='black')
dr.text((30,585),'Low walk Y.22 is at the ends; there is no path across the dam face. Endpoint connection and Unity results are reported separately from photo fidelity.',font=font(18),fill=(170,25,25));im.save(M/'same-position-section.png')
(D/'section-dimensions.json').write_text(json.dumps(summary,indent=2))
# Plan in bridge-local coordinates, preserving the explicit test-field extent.
im=Image.new('RGB',(1260,900),'white');dr=ImageDraw.Draw(im);p=lambda s,d:(260+s*10,360+d*10)
# Actual bridge coordinates provide a north arrow without claiming surveyed heights.
# Basis Unity Z north is negative; in local s,d plane its components are (-Tz,-Nz).
dr.text((25,15),'R99 local section field / Unity derivative',font=font(25),fill=(170,25,25))
dr.rectangle((*p(-18,-20),*p(75.2695684,32)),outline=(210,130,50),width=2)
dr.text((25,68),'Orange: enclosing parameter-field bounds (s -18..75.27, d -20..32); not every face changes.',font=font(18),fill=(180,90,25))
dr.text((25,98),'Equal plan axis scale, authoring units. Blue toe outline is schematic; exact changed meshes: integration.tsv.',font=font(18),fill='black')
dr.rectangle((*p(0,-14),*p(57.2695684,0)),fill=(210,210,210),outline='black',width=2)
dr.rectangle((*p(0,0),*p(57.2695684,10.5)),outline=(40,100,180),width=3)
dr.rectangle((*p(0,0),*p(57.2695684,4.5)),fill=(215,190,165),outline=(180,65,20),width=3)
for s in [-3,57.2695684]:dr.rectangle((*p(s,0),*p(s+3,4.5)),fill=(190,190,190),outline='black',width=2)
dr.text(p(20,-8),'Bridge: local design uplift +1.859924',font=font(17),fill='black')
dr.text(p(5,6.2),'Blue: R98 toe envelope (approx outline)',font=font(17),fill=(40,100,180))
dr.text(p(5,2),'Candidate run 4.5',font=font(17),fill=(140,45,10))
dr.text(p(37,12),'Gate / R97 tie: local rebuild',font=font(17),fill=(180,30,30));dr.text(p(-8,12),'Far tie: local rebuild',font=font(17),fill=(180,30,30))
n0=(1070,135);n1=(1070+55*.64584737,135+55*.76346655);dr.line((*n0,*n1),fill='black',width=4);dr.ellipse((n1[0]-4,n1[1]-4,n1[0]+4,n1[1]+4),fill='black');dr.text((n1[0]+8,n1[1]-10),'N',font=font(22),fill='black')
dr.text((25,750),'s: A(637.7344,528.2603) -> B(681.4578,491.2729); d: lakeward N=(-.645847,-.763467) in Unity XZ.',font=font(17),fill='black')
dr.text((25,790),'R98 checkpoint preserved. R99 is a local derived design; water datum unchanged; shoreline plan compressed locally.',font=font(18),fill=(170,25,25));im.save(M/'candidate-scope-plan.png')
# Hash precisely the enumerated protected files and original gameplay saves.
reports=[]
for file in [D/'protected-start.json',R/'Docs/Reviews/journey_v48/restructure-20261102/user-saves-before.json']:
 entries=json.loads(file.read_text(encoding='utf-8-sig'));entries=[dict(path=k,sha256=v) for k,v in entries.items()] if isinstance(entries,dict) else entries;out=[]
 for row in entries:
  pp=Path(row['path']);pp=pp if pp.is_absolute() else R/pp;actual=hashlib.file_digest(pp.open('rb'),'sha256').hexdigest() if pp.is_file() else None;out.append(dict(path=str(pp),expected=row['sha256'],actual=actual,unchanged=actual==row['sha256']))
 reports.append(dict(baseline=str(file.relative_to(R)),count=len(out),unchanged=sum(x['unchanged'] for x in out),files=out))
(D/'preservation-final.json').write_text(json.dumps(reports,indent=2));print('Protection:',[(r['count'],r['unchanged']) for r in reports]);print('Dimension samples:',summary)

