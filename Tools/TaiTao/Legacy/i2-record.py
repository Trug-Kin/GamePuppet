import sys,subprocess,ctypes,json
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
out=Path('Test/BangChung/journey_v48/i2_r93_complete');data=Path('Test/KetQua/journey_v48/i2_r93_complete')
if not probe:
 subprocess.run([sys.executable,'Test/Tam/r93-pipe.py','Test/Tam/i2-seam-motion.cs.txt' if seam else 'Test/Tam/i2-motion.cs.txt',str(data/'seam-motion-response.json' if seam else 'motion-response.json')],check=True,stdout=subprocess.DEVNULL,creationflags=subprocess.CREATE_NO_WINDOW)
cmd=[imageio_ffmpeg.get_ffmpeg_exe(),'-hide_banner','-y','-f','gdigrab','-framerate','30','-draw_mouse','0','-i','title='+title,'-t','3' if probe else ('23' if seam else '53'),'-vf','pad=ceil(iw/2)*2:ceil(ih/2)*2','-c:v','libx264','-preset','ultrafast','-crf','19','-pix_fmt','yuv420p','-fps_mode','vfr',str(out/('capture-probe.mp4' if probe else ('i2-seam-controller.mp4' if seam else 'i2-controller.mp4')))]
p=subprocess.run(cmd,capture_output=True,creationflags=subprocess.CREATE_NO_WINDOW)
tag='probe' if probe else ('seam-controller' if seam else 'controller')
(data/(tag+'-capture.txt')).write_bytes(p.stderr)
(data/(tag+'-capture.json')).write_text(json.dumps(dict(command=cmd,exit=p.returncode,source_fps=30,audio=False,input='synthetic InputSystem keyboard; actual unchanged controller; local fixture',daylight_review_override=True),indent=2))
print(p.returncode)
