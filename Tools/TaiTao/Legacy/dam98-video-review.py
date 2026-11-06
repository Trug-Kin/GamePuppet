import sys,subprocess,json,re
from pathlib import Path
sys.path.insert(0,'Tools/python-deps')
import imageio_ffmpeg
from PIL import Image,ImageDraw,ImageFont
R=Path('D:/ConRoi');D=R/'Test/KetQua/journey_v48/dam_r98';M=R/'Test/BangChung/journey_v48/dam_r98';video=M/'dam-end-stairs-controller.mp4';exe=Path(imageio_ffmpeg.get_ffmpeg_exe());probe=exe.parent/'ffprobe.exe'
cmd=[str(exe),'-hide_banner','-i',str(video)];r=subprocess.run(cmd,capture_output=True,text=True);(D/'video-probe.txt').write_text(r.stderr)
times=[4,8,13,18,25,30,39,49,57,62,69,77]
im=Image.new('RGB',(1600,1140),'#edf0f2');dr=ImageDraw.Draw(im);f=ImageFont.truetype('C:/Windows/Fonts/arial.ttf',20);dr.text((15,12),'R98 — actual controller video frames / synthetic input / source 30fps VFR / silent',font=f,fill='#183342')
for i,t in enumerate(times):
 p=M/f'controller-frame-{t:02d}.png';subprocess.run([str(exe),'-v','error','-y','-ss',str(t),'-i',str(video),'-frames:v','1',str(p)],check=True,capture_output=True);pic=Image.open(p);pic.thumbnail((390,300));x=8+(i%4)*400;y=55+(i//4)*355;im.paste(pic,(x,y+25));dr.text((x,y),f'video t={t}s',font=f,fill='#183342')
im.save(M/'controller-filmstrip.jpg',quality=94)
log=(D/'route-motion.txt').read_text();rows=re.findall(r'WAYPOINT (\d+) t=([\d.]+) p=\(([^)]+)\)',log);(D/'route-summary.json').write_text(json.dumps(dict(end=log.splitlines()[-1],waypoints=[dict(index=int(i),t=float(t),p=p) for i,t,p in rows],light=re.findall(r'LIGHTING[^\n]+',log),frames=times,scope='One initial local fixture, continuous actual unchanged controller; no subsequent placement; synthetic InputSystem not hardware; no audio'),indent=2));print(r.stderr[-1800:]);print(log.splitlines()[-1])
