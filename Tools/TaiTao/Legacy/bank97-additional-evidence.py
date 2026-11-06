from pathlib import Path
import sys,json,subprocess,re
sys.path.insert(0,'Tools/python-deps')
import imageio_ffmpeg
from PIL import Image,ImageDraw
D=Path('Test/KetQua/journey_v48/bank_r97');M=Path('Test/BangChung/journey_v48/bank_r97');v=M/'bank-gameplay-stairs-controller.mp4';ff=imageio_ffmpeg.get_ffmpeg_exe();g=imageio_ffmpeg.read_frames(str(v));meta=next(g);g.close();n,t=imageio_ffmpeg.count_frames_and_secs(str(v));meta.update(frame_count=n,counted_duration=t,average_fps=n/t,source_fps=30,audio=False,input='synthetic keyboard/mouse through unchanged controller',fixture='Separate low-path Z450 fixture, 15/15 waypoints, up both flights then back down; actual gameplay lighting throughout');(D/'gameplay-video-metadata.json').write_text(json.dumps(meta,indent=2))
sheet=Image.new('RGB',(1500,630),'#ddd')
for i,t in enumerate([1,3,6,9,12,14]):
 p=M/f'gameplay-frame-{t:03}.jpg';subprocess.run([ff,'-y','-ss',str(t),'-i',str(v),'-frames:v','1','-q:v','3',str(p)],stdout=subprocess.DEVNULL,stderr=subprocess.DEVNULL,check=True);im=Image.open(p).convert('RGB');im.thumbnail((500,285));tile=Image.new('RGB',(500,315),'white');tile.paste(im,(0,27));ImageDraw.Draw(tile).text((8,7),f'Gameplay fixture capture {t}s',fill='black');sheet.paste(tile,(i%3*500,i//3*315))
sheet.save(M/'gameplay-stairs-filmstrip.jpg');print(meta)
s=Path('Test/Tam/bank97-fixedviews.cs.txt').read_text();s=s.replace('new Vector3(645,14,482),new Vector3(690,2.7f,480),new Vector3(666.8f,1.9f,450),new Vector3(666,2.4f,449)','new Vector3(681,2,480),new Vector3(654,1.8f,423),new Vector3(681.5f,2,450),new Vector3(677.5f,1.8f,479)');s=s.replace('new Vector3(670,.7f,447),new Vector3(659,1,429),new Vector3(655,.5f,429),new Vector3(674,.9f,450)','new Vector3(679,1,485),new Vector3(650,.6f,418),new Vector3(688,1,450),new Vector3(675,.5f,475)');s=s.replace('"wide","upper","lower","stairs"','"endpoint-north","endpoint-south","upper-join","rail-foot"');Path('Test/Tam/bank97-endpoints.cs.txt').write_text(s)
