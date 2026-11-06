import sys,subprocess,json,ctypes,time
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
print(wins)
if not wins:raise RuntimeError('Game view window required')
h,title=wins[0];u.SetForegroundWindow(h)
out=Path('Test/BangChung/journey_v48/r93');exe=imageio_ffmpeg.get_ffmpeg_exe()
probe='--probe' in sys.argv
if not probe:
 subprocess.run([sys.executable,'Test/Tam/r93-pipe.py','Test/Tam/r93-motion.cs.txt','Test/KetQua/journey_v48/r93/motion-response.json'],check=True,stdout=subprocess.DEVNULL)
cmd=[exe,'-hide_banner','-y','-f','gdigrab','-framerate','30','-draw_mouse','0','-i','title='+title,'-t','3' if probe else '23','-vf','pad=ceil(iw/2)*2:ceil(ih/2)*2','-c:v','libx264','-preset','ultrafast','-crf','19','-pix_fmt','yuv420p','-fps_mode','vfr',str(out/('capture-probe.mp4' if probe else 'langfarm-r93-controller.mp4'))]
p=subprocess.run(cmd,capture_output=True)
tag='probe' if probe else 'controller'
Path('Test/KetQua/journey_v48/r93/'+tag+'-capture.txt').write_bytes(p.stderr)
Path('Test/KetQua/journey_v48/r93/'+tag+'-capture.json').write_text(json.dumps(dict(command=cmd,exit=p.returncode,source_fps=30,input='synthetic InputSystem keyboard through actual CharacterController; one initial local fixture placement',audio=False),indent=2))
print(p.returncode)
