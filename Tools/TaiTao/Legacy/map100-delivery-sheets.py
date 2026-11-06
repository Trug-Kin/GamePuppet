from pathlib import Path
import json,re,sys,math,io,subprocess
R=Path('D:/ConRoi');D=R/'Test/KetQua/journey_v48/map_r99_closeout';M=R/'Test/BangChung/journey_v48/map_r99_closeout';sys.path.insert(0,str(R/'Tools/python-deps'))
from PIL import Image,ImageDraw,ImageFont
font=ImageFont.truetype('C:/Windows/Fonts/arial.ttf',20);small=ImageFont.truetype('C:/Windows/Fonts/arial.ttf',16);title=ImageFont.truetype('C:/Windows/Fonts/arial.ttf',27)
im=Image.new('RGB',(1510,1120),'#13232b');dr=ImageDraw.Draw(im);dr.text((25,15),'R100: bridge-dam closeout / finite review scope',font=title,fill='white')
def pt(x,z):return (35+(x-610)*6,65+(z-390)*6)
for x in range(610,756,10):dr.line([pt(x,390),pt(x,554)],fill='#31444f');dr.text((pt(x,390)[0],47),str(x),font=small,fill='#b7c8d2')
for z in range(390,555,10):dr.line([pt(610,z),pt(755,z)],fill='#31444f');dr.text((0,pt(610,z)[1]),str(z),font=small,fill='#b7c8d2')
A=(637.7344,528.2603);T=(.76346655,-.64584737);N=(-.64584737,-.76346655);L=57.2695684
def sd(s,d):return pt(A[0]+T[0]*s+N[0]*d,A[1]+T[1]*s+N[1]*d)
dr.polygon([sd(-3,-14),sd(L+3,-14),sd(L+3,0),sd(-3,0)],fill='#53636b',outline='#a0b2bd')
dr.polygon([sd(0,0),sd(L,0),sd(L,4.5),sd(0,4.5)],fill='#a2a49b',outline='white')
for s in [-3,L]:dr.polygon([sd(s,0),sd(s+3,0),sd(s+3,4.5),sd(s,4.5)],fill='#e4c382',outline='white')
route=[]
for line in (D/'route-motion.txt').read_text(encoding='utf-8').splitlines():
 m=re.search(r'^t=([\d.]+).* p=\(([-\d.]+), ([-\d.]+), ([-\d.]+)\)',line)
 if m:route.append(pt(float(m[2]),float(m[4])))
dr.line(route,fill='#67e6a2',width=3)
dr.rectangle([pt(680,390),pt(720,425)],outline='#d29cff',width=3);dr.text(pt(682,395),'M2 search footprint',font=small,fill='#e2b6ff');dr.text(pt(682,400),'UNVERIFIED',font=small,fill='#e2b6ff')
for x,z,label,color in [(683.53369,490.63019,'Gate lamp: rigid reseat / flowers refreshed','#ffbe63'),(643.5,414,'OPEN terrain tear / no ray support','#ff706e'),(693,507,'Historic mismatch: now ~0.025 unit','#9dcdec'),(714.94,475,'I2 seam test location (historical)','#cdd5da')]:
 p=pt(x,z);dr.ellipse((p[0]-5,p[1]-5,p[0]+5,p[1]+5),fill=color);dr.text((p[0]+8,p[1]-8),label,font=small,fill=color)
dr.text(sd(11,-11),'Bridge deck Y=3 (unchanged)',font=small,fill='white');dr.text(sd(20,6),'Dam / lake side',font=small,fill='white')
dr.line([(935,190),(935,95)],fill='white',width=3);dr.polygon([(935,88),(927,105),(943,105)],fill='white');dr.text((915,65),'N',font=title,fill='white')
lines=['Legend / limits','Green: continuous unchanged-controller route','One initial private bridge fixture; no placements','during route. Not New Game or hardware input.','','Gold stairs: walked up/down both ends.','Orange point: only geometry contact correction.','Six flower meshes: geometry unchanged; fresh','Unity mesh assets removed stale render state.','','Purple: M2 identification search only.','I2: retained r94/r96; not fully replayed this turn.','Red point: confirmed legacy terrain defect','outside this bridge-dam cluster; remains OPEN.','','Coordinates: Unity X/Z, estimated game units.','North = -Unity Z. Schematic ground footprints.','No surveyed absolute dimensions claimed.','Market flat design and lake datum unchanged.']
for i,line in enumerate(lines):dr.text((1000,90+i*31),line,font=small if i else font,fill='#d8e5ec')
im.save(M/'scope-plan.png')
# Same-position source stair samples / known unchanged dam section, with actual lamp datum change.
im=Image.new('RGB',(1500,880),'#f4f6f7');dr=ImageDraw.Draw(im);dr.text((25,18),'Measured source sections / game units; not surveyed metres',font=title,fill='#142b38')
def graph_box(x,y,w,h,titletext):dr.rectangle((x,y,x+w,y+h),outline='#59707b',width=2);dr.text((x+10,y+8),titletext,font=font,fill='#142b38')
graph_box(45,90,900,360,'Far stair, s=-1.5; source render vs Unity collider at same points')
rows=[list(map(float,l.split('\t'))) for l in (D/'source-stair-expected.tsv').read_text().splitlines()];rows=[r for r in rows if r[0]==-1.5]
def gr(d,h):return (100+d*85,410-h*85)
dense=json.loads((D/'source-far-stair-dense-section.json').read_text());dr.line([gr(r['d'],r['h']) for r in dense if r['h'] is not None],fill='#26759f',width=3)
for _,d,h in rows:
 p=gr(d,h);dr.ellipse((p[0]-3,p[1]-3,p[0]+3,p[1]+3),fill='#20895b')
for h in [0,1,2,3]:p=gr(0,h);dr.text((55,p[1]-9),str(h),font=small,fill='#142b38');dr.line([(90,p[1]),(900,p[1])],fill='#ccd7dc')
dr.text((100,425),'d: 0 ... 4.5 lakeward; X/Y same scale (85 px/unit)',font=small,fill='#142b38');dr.text((540,200),'Blue: dense source profile (0.005 step)',font=small,fill='#142b38');dr.text((540,235),'Green: paired source/collider sample positions',font=small,fill='#142b38')
graph_box(990,90,460,360,'Lamp ground contact / both material meshes')
c=json.loads((D/'source-delta.json').read_text());values=[('R99 base',c['base_before']),('R100 base',c['base_after']),('Paving minimum',c['paving_min']),('Paving maximum',c['paving_max'])]
for i,(name,value) in enumerate(values):dr.text((1010,150+i*45),f'{name}: {value:.6f}',font=font,fill='#142b38')
dr.text((1010,345),'Rigid delta -0.058620; X/Z unchanged',font=small,fill='#142b38');dr.text((1010,378),'Seated 0.002...0.009276 into slope',font=small,fill='#142b38')
graph_box(45,490,1405,310,'Dam section at s=25: r99 -> r100 unchanged (vertex hashes)')
section=next(x for x in json.loads((R/'Test/KetQua/journey_v48/dam_r99/same-position-sections.json').read_text()) if x['revision']=='r99' and x['s']==25)
q=[(110+p['d']*76,770-p['h']*76) for p in section['samples'] if p['h'] is not None and p['d']<=4.51];dr.line(q,fill='#92623a',width=4)
dr.text((1120,550),'Crest 3.00 / toe 0.08',font=font,fill='#142b38');dr.text((1120,590),'Horizontal run 4.50',font=font,fill='#142b38');dr.text((1120,630),'Low walk 0.22 / water ~0',font=small,fill='#142b38');dr.text((1120,675),'13 bays / 14 rises estimated',font=small,fill='#142b38')
dr.text((550,620),'X/Y same scale (76 px/unit)',font=font,fill='#142b38');dr.text((550,660),'d: crest 0 -> toe 4.50, lakeward',font=small,fill='#142b38');dr.text((550,700),'Source section identical r99/r100',font=small,fill='#142b38');dr.text((45,830),'These measurements check integration/contact; photo correspondence and real dimensions remain separately limited.',font=font,fill='#142b38');im.save(M/'sections-and-contact.png')
# Short supplemental video filmstrip.
import imageio_ffmpeg
ff=imageio_ffmpeg.get_ffmpeg_exe();v=M/'bridge-backward-diagonal.mp4';meta=subprocess.run([ff,'-hide_banner','-i',str(v)],capture_output=True,text=True);(D/'back-diagonal-video-probe.txt').write_text(meta.stderr)
sheet=Image.new('RGB',(1440,350),'#14232d');dr=ImageDraw.Draw(sheet)
for i,t in enumerate([1,4,7]):
 p=subprocess.run([ff,'-hide_banner','-loglevel','error','-ss',str(t),'-i',str(v),'-vf','scale=480:300','-frames:v','1','-f','image2pipe','-vcodec','png','-'],capture_output=True,check=True);sheet.paste(Image.open(io.BytesIO(p.stdout)),(i*480,40));dr.text((i*480+10,9),f'Supplement video t={t}s',font=font,fill='white')
sheet.save(M/'back-diagonal-filmstrip.jpg',quality=90)
print('RESULT scope plan, measured sections and supplemental filmstrip')
