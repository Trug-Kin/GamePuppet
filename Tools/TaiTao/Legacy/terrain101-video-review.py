import sys,subprocess,json,re
from pathlib import Path
sys.path.insert(0,'Tools/python-deps');import imageio_ffmpeg
from PIL import Image,ImageDraw
R=Path('D:/ConRoi');D=R/'Test/KetQua/journey_v48/terrain_r101';E=R/'Test/BangChung/journey_v48/terrain_r101';ff=imageio_ffmpeg.get_ffmpeg_exe();metadata=[]
for name,out in [('terrain-controller.mp4','controller-filmstrip.jpg'),('terrain-backward-diagonal.mp4','back-diagonal-filmstrip.jpg')]:
 path=E/name
 if not path.exists():continue
 reader=imageio_ffmpeg.read_frames(str(path));meta=next(reader);reader.close();metadata.append(dict(file=name,metadata=meta,input='synthetic InputSystem keyboard/mouse via unchanged controller',audio=False))
 duration=meta['duration'];times=[duration*i/8 for i in range(8)];sheet=Image.new('RGB',(1440,960),'white');dr=ImageDraw.Draw(sheet)
 for j,t in enumerate(times):
  frame=E/(name.replace('.mp4','')+'-review-'+str(j)+'.png');subprocess.run([ff,'-hide_banner','-loglevel','error','-y','-ss',str(t),'-i',str(path),'-frames:v','1',str(frame)],check=True,creationflags=subprocess.CREATE_NO_WINDOW)
  a=Image.open(frame).convert('RGB').resize((480,285));x=(j%3)*480;y=(j//3)*320;sheet.paste(a,(x,y+25));dr.text((x+7,y+6),f'{name} t={t:.2f}s | synthetic input; silent',fill='black')
 sheet.save(E/out,quality=92)
(D/'video-review-metadata.json').write_text(json.dumps(metadata,indent=2))
# Spatial scope and actual recorded path, north is -UnityZ.
im=Image.new('RGB',(1200,960),'white');dr=ImageDraw.Draw(im)
def p(x,z):return (75+(x-638)*37,50+(z-407)*43)
dr.rectangle((*p(638,407),*p(665,425)),outline='black',width=2)
dr.rectangle((*p(640,411),*p(658,418)),fill=(245,226,185),outline=(190,110,0),width=3)
dr.rectangle((*p(649,411),*p(651,418)),fill=(225,165,165),outline='red',width=2)
co=[(641.841459,411.186473),(652.028465,426.079479)];dr.line([p(x,z) for x,z in co],fill=(40,110,160),width=4)
dr.line([p(646.502,418),p(652.302,418)],fill=(0,145,60),width=6)
trace=[]
for m in re.finditer(r'p=\(([-\d.]+), ([-\d.]+), ([-\d.]+)\)',(D/'route-motion.txt').read_text()):trace.append(p(float(m[1]),float(m[3])))
if trace:dr.line(trace,fill=(120,40,160),width=3)
x,y=p(643.5,414);dr.ellipse((x-7,y-7,x+7,y+7),fill='black');dr.text((x-90,y+13),'reported X643.5/Z414',fill='black')
for x in range(640,666,5):dr.text(p(x,425.2),f'X{x}',fill='black')
for z in range(410,426,5):dr.text((14,p(638,z)[1]),f'Z{z}',fill='black')
dr.text((1040,45),'N (-Z)',fill='black');dr.line((1070,110,1070,65),fill='black',width=3);dr.polygon([(1070,60),(1063,73),(1077,73)],fill='black')
for i,line in enumerate(['Orange: repair/collider-cut window X640..658 Z411..418.','Red: ramp fan centres corrected X649..651; corners held.','Green: shared topology at r97 join Z418; endpoint height fades by Z420.','Blue: inherited shoreline reference; elevation/water outline unchanged.','Purple: actual continuous controller trace; ONE start fixture at (654,414).','M2 outside this map remains UNVERIFIED. Units are game-authoring units.']):dr.text((55,830+i*20),line,fill='black')
im.save(E/'scope-and-controller-plan.png');print(json.dumps(metadata,indent=2))
