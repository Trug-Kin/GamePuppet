import sys,json,hashlib,re
from pathlib import Path
sys.path.insert(0,'Tools/python-deps')
from PIL import Image,ImageDraw,ImageFont,ImageOps
R=Path('D:/ConRoi');M=R/'Test/BangChung/journey_v48/r93';K=R/'Test/KetQua/journey_v48/r93'
F=ImageFont.truetype('C:/Windows/Fonts/arial.ttf',23);S=ImageFont.truetype('C:/Windows/Fonts/arial.ttf',18)
def panel(path,size):return ImageOps.contain(Image.open(path).convert('RGB'),size)
def sheet(rows,out):
 im=Image.new('RGB',(1800,len(rows)*470+70),'#151c25');dr=ImageDraw.Draw(im);dr.text((20,15),'R93 | PHOTO / BEFORE r92 / AFTER r93 | approximate matched camera; model dimensions unverified',font=F,fill='white')
 for j,row in enumerate(rows):
  for i,(path,label) in enumerate(row):
   p=panel(path,(590,410));im.paste(p,(i*600+(600-p.width)//2,100+j*470));dr.text((i*600+10,75+j*470),label,font=S,fill='white')
 im.save(M/out,quality=94)
p36=next((R/'Docs/Photo_3D_Model_Source/Photo_renumbered').glob('36_*'))
sheet([[(M/'P37-facade-crop.png','P37 / Oct 2020'),(M/'before-unity-day-R93_P37_MATCH.png','Unity r92 / same light and camera'),(M/'after-unity-day-R93_P37_MATCH.png','Unity r93 / source-relative proportions')],[(p36,'P36 independent direction / Oct 2020'),(M/'before-unity-day-R93_P36_INDEPENDENT.png','Unity r92 / independent direction'),(M/'after-unity-day-R93_P36_INDEPENDENT.png','Unity r93 / geometry held fixed')]],'comparison-final.jpg')
a=Image.open(M/'P37-facade-crop.png').convert('RGB');b=Image.open(M/'final-P37-materials.png').convert('RGB').resize(a.size);Image.blend(a,b,.5).save(M/'overlay-final-P37.jpg',quality=95)
mark=a.copy();d=ImageDraw.Draw(mark)
for label,x,y in [('A fascia north',52,225),('B fascia south',940,15),('C opaque panel',99,414),('D glazing',521,358),('E green reveal',226,583),('F entry',575,590),('G step / grade',605,700)]:
 d.ellipse((x-6,y-6,x+6,y+6),fill='#ff4040');d.text((min(x+9,900),max(y-23,0)),label,font=S,fill='#ffdb70',stroke_width=1,stroke_fill='black')
mark.save(M/'P37-landmarks.jpg',quality=95)
grid=json.loads((K/'render-collider-grid.json').read_text())['samples']
im=Image.new('RGB',(1800,1250),'white');d=ImageDraw.Draw(im)
d.text((35,20),'R93 ground sections | BEFORE = AFTER: unchanged native r90 grade',font=F,fill='black')
d.text((35,55),'Coordinates are Unity/model units, not surveyed metres. Render line and collider samples overlap (<1 mm at 121 points).',font=S,fill='black')
stats=[]
def plot(box,series,title):
 x0,y0,x1,y1=box;ys=[p[1] for name,pp in series for p in pp];xs=[p[0] for name,pp in series for p in pp];xmin=min(xs);xmax=max(xs);ymin=min(ys)-.08;ymax=max(ys)+.08
 def xy(x,y):return (x0+(x-xmin)/(xmax-xmin)*(x1-x0),y1-(y-ymin)/(ymax-ymin)*(y1-y0))
 d.text((x0,y0-36),title,font=F,fill='black')
 for k in range(6):
  y=ymin+(ymax-ymin)*k/5;p=xy(xmin,y);d.line((x0,p[1],x1,p[1]),fill='#dddddd');d.text((x0-65,p[1]-10),f'{y:.2f}',font=S,fill='black')
 colors=['#1768ac','#d34c21','#1e8a65']
 for idx,(name,pp) in enumerate(series):
  d.line([xy(*p) for p in pp],fill=colors[idx],width=4);d.text((x0+idx*270,y1+32),name,font=S,fill=colors[idx]);slope=(pp[-1][1]-pp[0][1])/(pp[-1][0]-pp[0][0])*100;stats.append({'section':title,'series':name,'slope_percent':slope,'drop_model_units':pp[-1][1]-pp[0][1]})
  for x,y in pp:d.ellipse((*[v-3 for v in xy(x,y)],*[v+3 for v in xy(x,y)]),fill=colors[idx])
 d.text((x0,y1+5),f'{xmin:.2f}',font=S,fill='black');d.text((x1-30,y1+5),f'{xmax:.2f}',font=S,fill='black')
plot((100,155,1670,460),[(f'd={dd:.1f}',[(e['s'],e['render'][0]) for e in grid if abs(e['d']-dd)<.01]) for dd in [8,11.5,15]],'LONGITUDINAL | x=s, facade northward; y=world height')
plot((100,660,1670,990),[(f's={ss:.2f}',[(e['d'],e['render'][0]) for e in grid if abs(e['s']-ss)<.01]) for ss in [0,10.4,20.8]],'CROSS | x=d, outward from facade; y=world height')
d.text((80,1090),'Observed continuous model grade: ~1.8–2.2% longitudinal; ~10.8–11.1% cross over 7 units.',font=F,fill='black')
d.text((80,1130),'No flattening target inferred from DSM30m or photographs. Real local grade and road/footway boundary remain UNVERIFIED.',font=S,fill='black')
im.save(M/'ground-sections.png')
(K/'section-statistics.json').write_text(json.dumps(stats,indent=2))
im=Image.new('RGB',(1700,1100),'white');d=ImageDraw.Draw(im)
d.text((30,20),'R93 PLAN | facade + adjoining native ground sample scope',font=F,fill='black')
def xy(s,v):return (100+s*65,170+(v-7)*90)
for e in grid:
 x,y=xy(e['s'],e['d']);d.ellipse((x-4,y-4,x+4,y+4),fill='#777777')
d.line([xy(3.23,8.4),xy(17.57,8.4)],fill='#ba331d',width=12)
d.text((300,110),'Facade width ~14.33 model units; centre retained; depth not surveyed',font=F,fill='#ba331d')
for idx,(s,v) in enumerate([(5,11.5),(14,11.5),(5,11.5),(4.88,8.56),(4.77,13.81)]):
 x,y=xy(s,v);d.ellipse((x-8,y-8,x+8,y+8),fill='#1778aa');d.text((x+10,y+10),str(idx+1),font=F,fill='#1778aa')
d.line([xy(*p) for p in [(5,11.5),(14,11.5),(5,11.5),(4.88,8.56),(4.77,13.81)]],fill='#1778aa',width=3)
d.text((100,960),'Grey:121 measured render/collider points. Blue:controller sample path, two-way + step/diagonal stops.',font=S,fill='black')
d.text((100,1000),'s increases north; d increases away from facade. This is a local model plan, not a surveyed street map.',font=S,fill='black')
im.save(M/'scope-plan.png')
steps=json.loads((K/'step-source-bounds.json').read_text());im=Image.new('RGB',(1700,700),'white');d=ImageDraw.Draw(im)
d.text((25,20),'R93 ENTRY SECTION | new closed steps + unchanged ground at s=5',font=F,fill='black')
def xy2(v,z):return (120+(v-7.7)*290,520-(z-4.7)*330)
pp=[(e['d'],e['render'][0]) for e in grid if abs(e['s']-4.16)<.01 and e['d']<12.3];d.line([xy2(*p) for p in pp],fill='#229658',width=5)
for name,e in steps.items():
 if 'EntryStep' not in name:continue
 lo,hi=e['min'],e['max'];d.rectangle((*xy2(lo[1],hi[2]+5.62),*xy2(hi[1],lo[2]+5.62)),outline='#1466b0',width=3)
 d.text((xy2(lo[1],hi[2]+5.62)[0],xy2(lo[1],hi[2]+5.62)[1]-50),f"z={hi[2]+5.62:.2f}",font=S,fill='#1466b0')
d.text((30,590),'3 closed solids; rise 0.16 model units; projection ~0.18 (lower two). Source shows exposed step edges.',font=F,fill='black')
d.text((30,635),'Absolute dimensions/projection estimated. Green: nearest grid ground section s=4.16. No terrain overlay or flattening.',font=S,fill='black')
im.save(M/'entry-section.png')
before=json.loads((K/'product-before-integration.json').read_text());diff=[]
for path,old in before.items():
 p=R/path;new=hashlib.sha256(p.read_bytes()).hexdigest() if p.exists() else None
 if old!=new:diff.append({'path':path,'before':old,'after':new})
(K/'protected-product-comparison.json').write_text(json.dumps({'checked':len(before),'differences':diff},indent=2));print('Protected product differences:',diff)
official=json.loads((K/'official-downloads.json').read_text());prov=[]
for idx,dest in [(2,'Langfarm_official_front.jpg'),(6,'Langfarm_official_glazing_inside.jpg')]:
 e=official[idx].copy();e['durable_file']='Docs/Photo_3D_Model_Source/Supplement_R93/'+dest;e['publication_date']='UNKNOWN';e['role']='front proportions' if idx==2 else 'glazing continuous; masonry columns behind glass';e['sha256']=hashlib.sha256((R/e['durable_file']).read_bytes()).hexdigest();prov.append(e)
(K/'source-provenance-final.json').write_text(json.dumps(prov,indent=2))
print('Evidence generated')
