"""Archive existing evidence and draw measurements; no game control or state changes."""
from pathlib import Path
import sys,json,hashlib,shutil,zipfile
R=Path.cwd();D=Path(__file__).parent;sys.path.insert(0,str(R/'Tools/python-deps'))
from PIL import Image,ImageDraw,ImageFont
C=Path('C:/Users/kaito/AppData/Local/Temp/ConRoi-r91-authoring-20261101')
V=Path('C:/Users/kaito/AppData/Local/Temp/ConRoi-r91-review-20261101')
def sha(p):
 with p.open('rb') as f:return hashlib.file_digest(f,'sha256').hexdigest()
for mode in ['lake-clock','market-clock','school-clock','bank-final-clock']:
 for suffix in ['-controller.mp4','-controller.txt','-video-metadata.json','-capture.log','-audio-result.txt']:
  p=V/(mode+suffix)
  if p.exists():shutil.copy2(p,D/p.name)
for p in V.glob('*-listener.f32'):shutil.copy2(p,D/p.name)
sources=['character-repairs-r91-source.blend','stall-sign-r91-source.blend','north-bank-r91-source-final.blend','north-bank-r91-extra-final.blend']
zpath=D/'editable-sources-r91.zip'
if not zpath.exists():
 with zipfile.ZipFile(zpath,'x',compression=zipfile.ZIP_DEFLATED,compresslevel=1) as z:
  for n in sources:z.write(C/n,n)
  for p in C.glob('*mapping.bin'):z.write(p,'mapping/'+p.name)
  for p in D.glob('*.py'):z.write(p,'recipes/'+p.name)
(D/'editable-source-manifest.json').write_text(json.dumps({'archive':zpath.name,'sha256':sha(zpath),'sources':{n:{'bytes':(C/n).stat().st_size,'sha256':sha(C/n)} for n in sources},'scope_correction':'Final executable target() mask X545..630, despite stale X643 descriptions in source custom properties/provenance. No Grade033/077 integrated.'},indent=2))
old=json.loads((D/'bank-sections-before.json').read_text());new=json.loads((D/'bank-sections-final.json').read_text())
im=Image.new('RGB',(1500,1000),'white');dr=ImageDraw.Draw(im);font=ImageFont.truetype('C:/Windows/Fonts/arial.ttf',18)
dr.text((20,10),'R91 bank sections: solid blue FINAL / orange BEFORE. Heights metres; signed sampling along Unity +Z.',fill='black',font=font)
dr.text((20,38),'Technical continuity repair; local shore section estimated, not a field survey. X545..630; shoreline XY fixed.',fill='black',font=font)
for i,(a,b) in enumerate(zip(old,new)):
 ox=50+(i%3)*490;oy=100+(i//3)*440;w=420;h=320
 dr.text((ox,oy-24),'X/Z '+str(a['shore']),fill='black',font=font)
 for y in [-1,0,1,2,3,4]:
  yy=oy+h-(y+1)*h/5;dr.line((ox,yy,ox+w,yy),fill='#dddddd');dr.text((ox-25,yy-10),str(y),fill='black',font=font)
 for x in [-2,0,2,4,6,8,10,12]:
  xx=ox+(x+2)*w/14;dr.text((xx-6,oy+h+5),str(x),fill='black',font=font)
 for row,name,col in [(a,'Grade_017','#d67713'),(b,'BankR91_Grade_017','#1762b1')]:
  pts=[]
  for s in row['samples']:
   hit=s['hits'].get(name)
   if hit:pts.append((ox+(s['dry_offset']+2)*w/14,oy+h-(hit['y']+1)*h/5))
  if len(pts)>1:dr.line(pts,fill=col,width=3)
  for x,y in pts:dr.ellipse((x-3,y-3,x+3,y+3),fill=col)
im.save(D/'bank-sections-final.png')
# Real source, before, after; captions keep location uncertainty explicit.
items=[('bvp-shore.jpg','SOURCE BVPL: relation grass/revetment/water; exact segment unproven'),('before-bank.png','BEFORE r90: invented technical bank fence'),('final-bank.png','AFTER r91 Editor: fence removed; continuous estimated bank')]
sheet=Image.new('RGB',(1200,1050),'#181818');draw=ImageDraw.Draw(sheet)
for i,(name,label) in enumerate(items):
 p=Image.open(D/name).convert('RGB');p.thumbnail((1180,300));sheet.paste(p,((1200-p.width)//2,i*350+36));draw.text((12,i*350+8),label,fill='white',font=font)
sheet.save(D/'bank-source-before-after.jpg',quality=90)
baseline=json.loads((R/'Docs/Reviews/journey_v48/r90_windows_playtest/saves-before.json').read_text(encoding='utf-8-sig'))
changed=[];missing=[]
for n,h in baseline.items():
 p=Path(n)
 if not p.exists():missing.append(n)
 elif sha(p)!=h:changed.append(n)
(D/'preservation-final.json').write_text(json.dumps({'original_save_count':len(baseline),'unchanged':len(baseline)-len(changed)-len(missing),'changed':changed,'missing':missing,'r90_zip_sha256':sha(R/'Docs/Reviews/journey_v48/r90_windows_playtest/ConRoi-r90-Windows-playtest.zip'),'r90_scene_sha256':sha(R/'Assets/Scenes/JourneyPresentation_v48_r90.unity')},indent=2))
print('Evidence archived; sections and preservation written.')
