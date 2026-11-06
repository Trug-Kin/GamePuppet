from pathlib import Path
import sys,subprocess,json,re,io
R=Path('D:/ConRoi');sys.path.insert(0,str(R/'Tools/python-deps'));import imageio_ffmpeg
from PIL import Image,ImageDraw,ImageFont
D=R/'Test/KetQua/journey_v48/map_r99_closeout';M=R/'Test/BangChung/journey_v48/map_r99_closeout';ff=imageio_ffmpeg.get_ffmpeg_exe();video=M/'dam-end-stairs-controller.mp4'
p=subprocess.run([ff,'-hide_banner','-i',str(video)],capture_output=True,text=True);(D/'video-probe.txt').write_text(p.stderr)
font=ImageFont.truetype('C:/Windows/Fonts/arial.ttf',22)
sheet=Image.new('RGB',(1440,990),'#17232d');dr=ImageDraw.Draw(sheet)
for i,t in enumerate([4,15,26,37,48,59,70,81,92]):
 p=subprocess.run([ff,'-hide_banner','-loglevel','error','-ss',str(t),'-i',str(video),'-vf','scale=480:300','-frames:v','1','-f','image2pipe','-vcodec','png','-'],capture_output=True,check=True);im=Image.open(io.BytesIO(p.stdout));x=i%3*480;y=i//3*330;sheet.paste(im,(x,y+30));dr.text((x+8,y+3),f'Video t={t}s',font=font,fill='white')
sheet.save(M/'controller-filmstrip.jpg',quality=92)
for tag in ['gate-overhead','gate-lamp-side','gate-bed']:
 a=Image.open(M/(tag+'-r99.png'));b=Image.open(M/(tag+'-r100.png'));out=Image.new('RGB',(1600,540),'#17232d');dr=ImageDraw.Draw(out)
 for i,(im,title) in enumerate([(a,'R99 - before'),(b,'R100 - reopened scene')]):out.paste(im.resize((800,500)),(i*800,40));dr.text((i*800+15,8),title,font=font,fill='white')
 out.save(M/(tag+'-before-after.jpg'),quality=93)
print('RESULT video probe, filmstrip and three same-camera comparisons')
