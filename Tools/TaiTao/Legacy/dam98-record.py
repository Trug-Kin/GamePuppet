import sys,subprocess,ctypes,json,time
from pathlib import Path
sys.path.insert(0,'Tools/python-deps')
import imageio_ffmpeg
u=ctypes.windll.user32
wins=[]
@ctypes.WINFUNCTYPE(ctypes.c_bool,ctypes.c_void_p,ctypes.c_void_p)
def cb(h,l):
 b=ctypes.create_unicode_buffer(1000);u.GetWindowTextW(h,b,1000)
 if u.IsWindowVisible(h) and (b.value=='Game' or b.value.startswith('Game - ')):wins.append((h,b.value))
 return True
u.EnumWindows(cb,0)
if not wins:raise RuntimeError('Game window required')
h,title=wins[0];u.SetForegroundWindow(h)
out=Path('Test/BangChung/journey_v48/dam_r98');data=Path('Test/KetQua/journey_v48/dam_r98')
subprocess.run([sys.executable,'Test/Tam/r93-pipe.py','Test/Tam/dam98-route.cs.txt',str(data/'route-response.json')],check=True,stdout=subprocess.DEVNULL,creationflags=subprocess.CREATE_NO_WINDOW)
cmd=[imageio_ffmpeg.get_ffmpeg_exe(),'-hide_banner','-y','-f','gdigrab','-framerate','30','-draw_mouse','0','-i','title='+title,'-t','245','-vf','pad=ceil(iw/2)*2:ceil(ih/2)*2','-c:v','libx264','-preset','fast','-crf','24','-pix_fmt','yuv420p','-fps_mode','vfr',str(out/'dam-end-stairs-controller.mp4')]
started=time.time()
with open(data/'capture-fixed-live.txt','wb') as log:
 p=subprocess.Popen(cmd,stdin=subprocess.PIPE,stdout=subprocess.DEVNULL,stderr=log,creationflags=subprocess.CREATE_NO_WINDOW)
 while p.poll() is None:
  done=data/'route-motion.txt'
  if done.exists() and done.stat().st_mtime>started-5:p.communicate(input=b'q\n',timeout=30);break
  time.sleep(.5)
(data/'capture-fixed.json').write_text(json.dumps(dict(command=cmd,exit=p.returncode,source_fps=30,audio=False,input='synthetic InputSystem keyboard/mouse through unchanged controller',initial='one ground-review fixture on bridge with copied valid market profile; no subsequent placement',lighting='daylight outbound, gameplay return; runtime-only review override'),indent=2))
print(p.returncode)
