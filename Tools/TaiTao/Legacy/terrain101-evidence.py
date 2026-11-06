import sys,json,hashlib,re
from pathlib import Path
sys.path.insert(0,'Tools/python-deps');from PIL import Image,ImageDraw
R=Path('D:/ConRoi');D=R/'Test/KetQua/journey_v48/terrain_r101';E=R/'Test/BangChung/journey_v48/terrain_r101'
im=Image.new('RGB',(1280,890),'white');dr=ImageDraw.Draw(im)
for j,tag in enumerate(['player','top']):
 for i,state in enumerate(['before','after']):
  a=Image.open(E/f'{tag}-{state}.png').resize((640,400));im.paste(a,(i*640,j*440+30));dr.text((i*640+12,j*440+8),f'{tag} {state} | same diagnostic camera, not controller',fill='black')
im.save(E/'before-after.jpg',quality=92)
sections=json.loads((D/'source-sections-before-after.json').read_text());im=Image.new('RGB',(1200,730),'white');dr=ImageDraw.Draw(im)
for j,x in enumerate([648,649,650]):
 y0=50+j*220;dr.text((20,y0),f'North(-Z) <--- section Unity X{x}; Z417..420 | metres: authoring units, not field survey',fill='black')
 dr.line((100,y0+170,1100,y0+170),fill='gray');dr.line((100,y0+20,100,y0+170),fill='gray')
 for state,color in [('before','red'),('after','blue')]:
  q=next(q for q in sections if q['version']==state);points=[]
  for r in q['rows']:
   if r['x']!=x or r['z']<417:continue
   hits=[h['y'] for h in r['layers'] if h['object']!='M95_Service3_NATIVE']
   if hits:points.append((100+(r['z']-417)*320,y0+170-max(hits)*85))
  if len(points)>1:dr.line(points,fill=color,width=3)
 dr.text((1070,y0+35),'red before',fill='red');dr.text((1070,y0+55),'blue after',fill='blue')
 for z in [417,418,419,420]:dr.text((100+(z-417)*320,y0+175),str(z),fill='black')
im.save(E/'sections-before-after.png')
a=(R/'Assets/Scenes/JourneyPresentation_v48_r100.unity').read_text(encoding='utf8').splitlines();b=(R/'Assets/Scenes/JourneyPresentation_v48_r101.unity').read_text(encoding='utf8').splitlines()
if len(a)==len(b):delta=[dict(line=i+1,before=x,after=y) for i,(x,y) in enumerate(zip(a,b)) if x!=y]
else:delta=dict(before_lines=len(a),after_lines=len(b),note='Different line count: inspect per YAML object')
(D/'scene-line-diff.json').write_text(json.dumps(delta,indent=2));print('scene diff',len(delta))
