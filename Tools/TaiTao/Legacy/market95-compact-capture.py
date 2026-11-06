import sys,subprocess,json,hashlib,os
from pathlib import Path
sys.path.insert(0,'Tools/python-deps')
import imageio_ffmpeg as ff
M=Path('Test/BangChung/journey_v48/market_ground_r95');D=Path('Test/KetQua/journey_v48/market_ground_r95');p=M/'market95-controller.mp4';q=M/'market95-controller-compact.mp4'
beforehash=hashlib.sha256(p.read_bytes()).hexdigest();r=ff.read_frames(str(p));before=next(r);r.close()
subprocess.run([ff.get_ffmpeg_exe(),'-v','error','-y','-i',str(p),'-c:v','libx264','-preset','fast','-crf','23','-fps_mode','passthrough',str(q)],check=True)
r=ff.read_frames(str(q));after=next(r);r.close();assert before['size']==after['size'] and abs(before['duration']-after['duration'])<.1
(D/'capture-reencode.json').write_text(json.dumps(dict(original_sha256=beforehash,original_bytes=p.stat().st_size,compact_bytes=q.stat().st_size,before=before,after=after,reason='same continuous capture, reencoded H264 CRF23; source 30fps VFR; no audio; no cuts'),indent=2))
os.replace(q,p);print('Verified same dimensions/duration, replaced generated capture with compact encoding',before['duration'],after['duration'])
