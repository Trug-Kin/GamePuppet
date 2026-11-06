from pathlib import Path
import re
D='Test/KetQua/journey_v48/r95_visual_review/'
s=Path('Test/Tam/market95-fixture.cs.txt').read_text().replace('Test/KetQua/journey_v48/market_ground_r95/',D)
Path('Test/Tam/r95review-fixture.cs.txt').write_text(s,encoding='utf-8')
s=Path('Test/Tam/market95-motion.cs.txt').read_text().replace('Test/KetQua/journey_v48/market_ground_r95/',D)
s=s.replace('new Vector3(730,10,438)','new Vector3(714.94f,15,535)').replace('s.Place(new Vector3(730,','s.Place(new Vector3(714.94f,').replace(',438),Quaternion.Euler',',535),Quaternion.Euler')
pts=[(714.94,520),(714.94,500),(714.94,486),(714.94,474),(714.94,457),(730,438),(731,403),(740,370),(750,337),(741,306),(725,302),(737,300),(750,260),(766,226),(784,220),(805,211),(812,191),(804,171),(784,163),(761,173),(756,194),(766,216),(784,220),(766,226),(750,260),(737,300),(725,302),(741,306),(750,337),(740,370),(731,403),(730,438),(714.94,457),(714.94,474),(714.94,486),(714.94,500),(714.94,520),(714.94,535)]
s=re.sub(r'Vector2\[\] path=\{.*?\};','Vector2[] path={'+','.join(f'new Vector2({x}f,{z}f)' for x,z in pts)+'};',s)
s=s.replace('t<480','t<520').replace('max480s','max520s').replace('controller-motion.txt','route-motion.txt')
# Flush evidence during traversal; no runtime component or independent runner.
s=s.replace('if(leg<path.Length&&t<520)return;','if((int)t%5==0)File.WriteAllText("'+D+'route-progress.txt",b.ToString());if(leg<path.Length&&t<520)return;')
Path('Test/Tam/r95review-route-motion.cs.txt').write_text(s,encoding='utf-8')
rec=Path('Test/Tam/market95-record.py').read_text();rec=rec[:rec.index("probe='--probe'")]+'''out=Path('Test/BangChung/journey_v48/r95_visual_review');data=Path('Test/KetQua/journey_v48/r95_visual_review')
subprocess.run([sys.executable,'Test/Tam/r93-pipe.py','Test/Tam/r95review-route-motion.cs.txt',str(data/'route-response.json')],check=True,stdout=subprocess.DEVNULL,creationflags=subprocess.CREATE_NO_WINDOW)
cmd=[imageio_ffmpeg.get_ffmpeg_exe(),'-hide_banner','-y','-f','gdigrab','-framerate','30','-draw_mouse','0','-i','title='+title,'-t','525','-vf','pad=ceil(iw/2)*2:ceil(ih/2)*2','-c:v','libx264','-preset','fast','-crf','24','-pix_fmt','yuv420p','-fps_mode','vfr',str(out/'bridge-market-return-controller.mp4')]
started=time.time()
with open(data/'capture-live.txt','wb') as log:
 p=subprocess.Popen(cmd,stdin=subprocess.PIPE,stdout=subprocess.DEVNULL,stderr=log,creationflags=subprocess.CREATE_NO_WINDOW)
 while p.poll() is None:
  done=data/'route-motion.txt'
  if done.exists() and done.stat().st_mtime>started-5:p.communicate(input=b'q\\n',timeout=30);break
  time.sleep(.5)
(data/'capture.json').write_text(json.dumps(dict(command=cmd,exit=p.returncode,source_fps=30,audio=False,input='synthetic InputSystem keyboard/mouse through unchanged controller',initial='one ground-review fixture on bridge with copied valid market profile; no subsequent placement',lighting='daylight outbound, gameplay return; runtime-only review override'),indent=2))
print(p.returncode)
'''
Path('Test/Tam/r95review-record.py').write_text(rec)
