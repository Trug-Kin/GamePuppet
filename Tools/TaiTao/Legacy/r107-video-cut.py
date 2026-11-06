import sys,subprocess,json
from pathlib import Path
sys.path.insert(0,'Tools/python-deps');import imageio_ffmpeg
R=Path('D:/ConRoi');E=R/'Test/BangChung/investigation_r107';D=R/'Test/KetQua/investigation_r107';exe=imageio_ffmpeg.get_ffmpeg_exe()
out=E/'onboarding-short.mp4'
args=[exe,'-v','error','-y','-i',str(E/'onboarding-start.mp4'),'-filter_complex','[0:v]trim=start=0:end=28,setpts=PTS-STARTPTS[a];[0:v]trim=start=142:end=172.83,setpts=PTS-STARTPTS[b];[a][b]concat=n=2:v=1:a=0[v]','-map','[v]','-c:v','libx264','-preset','fast','-crf','23','-pix_fmt','yuv420p',str(out)]
subprocess.run(args,check=True,creationflags=subprocess.CREATE_NO_WINDOW)
(D/'onboarding-short-edit.json').write_text(json.dumps(dict(source='onboarding-start.mp4',output='onboarding-short.mp4',segments_seconds=[[0,28],[142,172.83]],omitted='114 seconds standing still in inspection waiting for tooling; no movement or gameplay state edited',audio=False,input='synthetic controller and mouse/keyboard UI'),indent=2))
