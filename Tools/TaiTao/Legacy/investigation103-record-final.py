"""Bounded Editor evidence capture using the existing bridge, no player runner/build."""
import sys,subprocess,ctypes,json,time
from pathlib import Path
sys.path.insert(0,'Tools/python-deps')
import imageio_ffmpeg
D=Path('Test/KetQua/investigation_r103');E=Path('Test/BangChung/investigation_r103')
u=ctypes.windll.user32;h=u.FindWindowW(None,'Game')
if not h:raise RuntimeError('Existing Game window required')
started=time.time()
subprocess.run([sys.executable,'Test/Tam/r93-pipe.py','Test/Tam/investigation103-route.cs.txt',str(D/'route-final-response.json')],check=True,stdout=subprocess.DEVNULL,creationflags=subprocess.CREATE_NO_WINDOW)
response=json.loads((D/'route-final-response.json').read_text())
if not response.get('result',{}).get('success'):raise RuntimeError(response)
u.ShowWindow(h,9);u.SetForegroundWindow(h)
cmd=[imageio_ffmpeg.get_ffmpeg_exe(),'-hide_banner','-y','-f','gdigrab','-framerate','30','-draw_mouse','0','-i','title=Game','-t','230','-vf','pad=ceil(iw/2)*2:ceil(ih/2)*2','-c:v','libx264','-preset','fast','-crf','24','-pix_fmt','yuv420p',str(E/'controller-route-final.mp4')]
with (D/'controller-final-capture.log').open('wb') as log:
 p=subprocess.Popen(cmd,stdin=subprocess.PIPE,stdout=subprocess.DEVNULL,stderr=log,creationflags=subprocess.CREATE_NO_WINDOW)
 done=D/'route-motion.txt';observed=None
 while p.poll() is None:
  if done.exists() and done.stat().st_mtime>started:
   text=done.read_text(encoding='utf-8-sig')
   if 'END ' in text:
    observed=text.split('END ')[-1].split(' data=')[0];time.sleep(1.5);p.communicate(input=b'q\n',timeout=30);break
  time.sleep(.3)
(D/'controller-capture-final.json').write_text(json.dumps(dict(exit=p.returncode,completion=observed,stop='route completion plus1.5s; 230s only a timeout',input='synthetic InputSystem keyboard/mouse through unchanged controller',audio=False,teleports=False,initial='normal authored r103 spawn, existing helper3s hold',command=cmd),indent=2))
if p.returncode!=0 or not observed or not observed.startswith('21/21'):raise RuntimeError('Capture/route failed: '+str(observed))
print('Recorded full route through completion:',observed)
