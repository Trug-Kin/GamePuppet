"""Bounded evidence recording of the existing Editor GameView, no product runner/build."""
import subprocess,sys,ctypes,time,json
from pathlib import Path
sys.path.insert(0,'Tools/python-deps');import imageio_ffmpeg
snippet,output,video,done=sys.argv[1:];D=Path('Test/KetQua/investigation_r105');E=Path('Test/BangChung/investigation_r105');start=time.time()
u=ctypes.windll.user32;h=u.FindWindowW(None,'Game')
if not h:raise RuntimeError('Existing Game window required')
u.ShowWindow(h,9);u.SetForegroundWindow(h)
res=subprocess.run([sys.executable,'Test/Tam/investigation104-pipe.py',snippet,output],capture_output=True,creationflags=subprocess.CREATE_NO_WINDOW)
if res.returncode:raise RuntimeError(res.stdout.decode(errors='replace')+res.stderr.decode(errors='replace'))
cmd=[imageio_ffmpeg.get_ffmpeg_exe(),'-hide_banner','-y','-f','gdigrab','-framerate','30','-draw_mouse','1','-i','title=Game','-t','230','-vf','pad=ceil(iw/2)*2:ceil(ih/2)*2','-c:v','libx264','-preset','fast','-crf','24','-pix_fmt','yuv420p',str(E/video)]
with (D/(video+'.log')).open('wb') as log:
 p=subprocess.Popen(cmd,stdin=subprocess.PIPE,stdout=subprocess.DEVNULL,stderr=log,creationflags=subprocess.CREATE_NO_WINDOW)
 found=False
 while p.poll() is None:
  path=Path(done)
  if path.exists() and path.stat().st_mtime>start:
   found=True;time.sleep(1.5);p.communicate(input=b'q\n',timeout=30);break
  time.sleep(.3)
(D/(video+'.json')).write_text(json.dumps(dict(exit=p.returncode,completion_file=done,completed=found,input='synthetic UI or InputSystem as specified in paired log',audio=False,no_build=True),indent=2))
if p.returncode or not found:raise RuntimeError('Capture did not reach completion file')
print('Recorded',video,'completion',done)
