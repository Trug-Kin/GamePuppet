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
out=Path('Test/BangChung/journey_v48/map_r99_closeout');data=Path('Test/KetQua/journey_v48/map_r99_closeout')
local='--local' in sys.argv
subprocess.run([sys.executable,'Test/Tam/r93-pipe.py','Test/Tam/dam99-local-motion.cs.txt' if local else 'Test/Tam/map100-route.cs.txt',str(data/('local-motion-response.json' if local else 'route-response.json'))],check=True,stdout=subprocess.DEVNULL,creationflags=subprocess.CREATE_NO_WINDOW)
cmd=[imageio_ffmpeg.get_ffmpeg_exe(),'-hide_banner','-y','-f','gdigrab','-framerate','30','-draw_mouse','0','-i','title='+title,'-t','22' if local else '285','-vf','pad=ceil(iw/2)*2:ceil(ih/2)*2','-c:v','libx264','-preset','fast','-crf','24','-pix_fmt','yuv420p','-fps_mode','vfr',str(out/('lake-motion-final.mp4' if local else 'dam-end-stairs-controller.mp4'))]
started=time.time()
with open(data/('capture-local-live.txt' if local else 'capture-fixed-live.txt'),'wb') as log:
 p=subprocess.Popen(cmd,stdin=subprocess.PIPE,stdout=subprocess.DEVNULL,stderr=log,creationflags=subprocess.CREATE_NO_WINDOW)
 while p.poll() is None:
  done=data/('local-motion.txt' if local else 'route-motion.txt')
  if done.exists() and done.stat().st_mtime>started-5:p.communicate(input=b'q\n',timeout=30);break
  time.sleep(.5)
(data/('capture-local.json' if local else 'capture-fixed.json')).write_text(json.dumps(dict(command=cmd,exit=p.returncode,source_fps=30,audio=False,input='none; natural AI motion with one private fixture' if local else 'synthetic InputSystem keyboard/mouse through unchanged controller',initial='local untouched-notebook fixture; no subsequent placement' if local else 'one ground-review fixture on bridge with copied valid market profile; no subsequent placement',lighting='daylight review' if local else 'daylight outbound, gameplay return; runtime-only review override'),indent=2))
print(p.returncode)
