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
probe='--probe' in sys.argv
seam='--seam' in sys.argv
local=next((a[2:] for a in sys.argv if a in ['--branch','--north','--north-return','--branch-final','--north-final']),None)
out=Path('Test/BangChung/journey_v48/market_ground_r95');data=Path('Test/KetQua/journey_v48/market_ground_r95')
if not probe:
 subprocess.run([sys.executable,'Test/Tam/r93-pipe.py','Test/Tam/market95-'+local+'-motion.cs.txt' if local else ('Test/Tam/i2-seam-motion.cs.txt' if seam else 'Test/Tam/market95-motion.cs.txt'),str(data/'seam-motion-response.json' if seam else 'motion-response.json')],check=True,stdout=subprocess.DEVNULL,creationflags=subprocess.CREATE_NO_WINDOW)
cmd=[imageio_ffmpeg.get_ffmpeg_exe(),'-hide_banner','-y','-f','gdigrab','-framerate','30','-draw_mouse','0','-i','title='+title,'-t','3' if probe else ('23' if seam else ('145' if local in ['branch','branch-final'] else '50' if local=='north-return' else '75' if local=='north-final' else '110') if local else '485'),'-vf','pad=ceil(iw/2)*2:ceil(ih/2)*2','-c:v','libx264','-preset','fast','-crf','23','-pix_fmt','yuv420p','-fps_mode','vfr',str(out/('capture-probe.mp4' if probe else ('i2-seam-controller.mp4' if seam else 'market95-'+local+'-controller.mp4' if local else 'market95-controller.mp4')))]
started=time.time()
with open(data/'controller-live-capture.txt','wb') as capturelog:
 p=subprocess.Popen(cmd,stdin=subprocess.PIPE,stdout=subprocess.DEVNULL,stderr=capturelog,creationflags=subprocess.CREATE_NO_WINDOW)
 while p.poll() is None:
  done=data/(local+'-motion.txt' if local else 'controller-motion.txt')
  if not probe and done.exists() and done.stat().st_mtime>started-5:
   p.communicate(input=b'q\n',timeout=30);break
  time.sleep(.5)
tag=local if local else 'probe' if probe else ('seam-controller' if seam else 'controller')
(data/(tag+'-capture.json')).write_text(json.dumps(dict(command=cmd,exit=p.returncode,source_fps=30,audio=False,input='synthetic InputSystem keyboard/mouse; actual unchanged controller; one local start fixture, continuous route',daylight_review_override=True),indent=2))
print(p.returncode)
