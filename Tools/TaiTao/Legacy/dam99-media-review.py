import sys,subprocess,json,re
from pathlib import Path
sys.path.insert(0,'Tools/python-deps')
import imageio_ffmpeg
from PIL import Image,ImageDraw
M=Path('Test/BangChung/journey_v48/dam_r99');D=Path('Test/KetQua/journey_v48/dam_r99');video=M/'dam-end-stairs-controller.mp4';ff=imageio_ffmpeg.get_ffmpeg_exe()
p=subprocess.run([ff,'-hide_banner','-i',str(video)],capture_output=True,text=True);(D/'video-metadata.txt').write_text(p.stderr)
sheet=Image.new('RGB',(1600,1140),'white');dr=ImageDraw.Draw(sheet)
for i,t in enumerate([3,12,18,26,34,45,55,62,70,78,86,91]):
 out=M/('route-frame-%02d.jpg'%t);subprocess.run([ff,'-hide_banner','-loglevel','error','-y','-ss',str(t),'-i',str(video),'-frames:v','1','-vf','scale=400:253',str(out)],check=True)
 im=Image.open(out);x=i%4*400;y=i//4*380;sheet.paste(im,(x,y+35));dr.text((x+10,y+10),'Controller t=%ds'%t,fill='black')
sheet.save(M/'controller-filmstrip.jpg',quality=93)
print(p.stderr[-1000:])
local=M/'lake-motion-final.mp4'
if local.exists():
 p=subprocess.run([ff,'-hide_banner','-i',str(local)],capture_output=True,text=True);(D/'local-video-metadata.txt').write_text(p.stderr)
 sheet=Image.new('RGB',(1602,600),'white');dr=ImageDraw.Draw(sheet)
 for i,t in enumerate([.5,3,6,9,12,15]):
  out=M/('local-final-frame-%s.png'%str(t));subprocess.run([ff,'-hide_banner','-loglevel','error','-y','-ss',str(t),'-i',str(local),'-frames:v','1','-vf','scale=534:328',str(out)],check=True)
  im=Image.open(out);x=i%3*534;y=i//3*300;sheet.paste(im.resize((534,260)),(x,y+30));dr.text((x+10,y+10),'Local fixture video t=%ss'%t,fill='black')
 sheet.save(M/'local-motion-filmstrip.jpg',quality=94);print(p.stderr[-1000:])
