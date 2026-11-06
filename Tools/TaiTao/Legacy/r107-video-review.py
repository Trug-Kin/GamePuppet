import sys,subprocess,json
from pathlib import Path
sys.path.insert(0,'Tools/python-deps')
import imageio_ffmpeg
from PIL import Image,ImageDraw
R=Path('D:/ConRoi');E=R/'Test/BangChung/investigation_r107';D=R/'Test/KetQua/investigation_r107';exe=imageio_ffmpeg.get_ffmpeg_exe()
v=E/'onboarding-start.mp4';g=imageio_ffmpeg.read_frames(str(v));meta=next(g);g.close();print(meta)
duration=meta['duration'];times=[0,7,14,22,max(25,duration-28),max(28,duration-18),max(30,duration-9),max(31,duration-2)]
sheet=Image.new('RGB',(1280,764),'#161b20');draw=ImageDraw.Draw(sheet)
for i,t in enumerate(times):
 p=E/('onboarding-review-%02d.png'%i)
 subprocess.run([exe,'-v','error','-y','-ss',str(t),'-i',str(v),'-frames:v','1',str(p)],check=True,creationflags=subprocess.CREATE_NO_WINDOW)
 im=Image.open(p);im.thumbnail((320,170));x=(i%4)*320;y=(i//4)*382;sheet.paste(im,(x,y+25));draw.text((x+8,y+6),'%.1fs'%t,fill='white')
sheet=sheet.crop((0,0,1280,580));sheet.save(E/'onboarding-filmstrip.jpg')
(D/'onboarding-video-metadata.json').write_text(json.dumps(dict(metadata=meta,samples=times,audio=False,input='synthetic keyboard/mouse; normal controller route then UI continuation'),indent=2,default=str))
