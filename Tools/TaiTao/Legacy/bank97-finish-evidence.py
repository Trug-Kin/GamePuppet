from pathlib import Path
import sys,json,subprocess
sys.path.insert(0,'Tools/python-deps')
import imageio_ffmpeg
from PIL import Image,ImageDraw
D=Path('Test/KetQua/journey_v48/bank_r97');M=Path('Test/BangChung/journey_v48/bank_r97')
p=Path('Test/Tam/bank97-reopen.cs.txt');s=p.read_text();s=s.replace("var names=e[1].Split",'if(e[1]=="")e[1]="R97_Bridgehead_GateSide_Bank/"+e[0];var names=e[1].Split');p.write_text(s)
v=M/'bridgehead-bank-controller.mp4';ff=imageio_ffmpeg.get_ffmpeg_exe();g=imageio_ffmpeg.read_frames(str(v));meta=next(g);g.close();frames,secs=imageio_ffmpeg.count_frames_and_secs(str(v));meta.update(frame_count=frames,counted_duration=secs,average_fps=frames/secs,source_fps=30,audio=False,input='synthetic InputSystem keyboard/mouse through unchanged controller; not hardware certification',route='68/68 waypoints including endpoint bank transition, single initial bridge fixture, no intervening placement',lighting='review sun/fog outbound with gameplay sky; actual gameplay sun/fog restored at waypoint32 for return')
(D/'video-metadata.json').write_text(json.dumps(meta,indent=2))
times=[1,8,15,22,30,38,45,51,58,66,74,81,88,94,100];sheet=Image.new('RGB',(1500,315*5),'#ddd')
for i,t in enumerate(times):
 p=M/f'route-frame-{t:03}.jpg';subprocess.run([ff,'-y','-ss',str(t),'-i',str(v),'-frames:v','1','-q:v','3',str(p)],stdout=subprocess.DEVNULL,stderr=subprocess.DEVNULL,check=True);im=Image.open(p).convert('RGB');im.thumbnail((500,285));tile=Image.new('RGB',(500,315),'white');tile.paste(im,(0,27));ImageDraw.Draw(tile).text((8,7),f'Actual capture {t}s',fill='black');sheet.paste(tile,(i%3*500,i//3*315))
sheet.save(M/'controller-filmstrip.jpg')
for photo,tag in [('181952','wide'),('182010','upper'),('181826','stairs')]:
 sheet=Image.new('RGB',(1500,460),'#fff');files=[next(Path('Test/BugModel').glob('*'+photo+'.png')),M/f'JourneyPresentation_v48_r96-{tag}.png',M/f'JourneyPresentation_v48_r97-{tag}.png']
 for i,(f,label) in enumerate(zip(files,[f'Photo {photo}: landmark-relative direction','r96 before | fixed eye/target/FOV60','r97 after | same eye/target/FOV60'])):
  im=Image.open(f).convert('RGB');im.thumbnail((500,390));sheet.paste(im,(i*500,40));ImageDraw.Draw(sheet).text((i*500+8,10),label,fill='black')
 ImageDraw.Draw(sheet).text((10,435),'Not calibrated camera matching. Game dimensions/stair counts estimated; source date and metric datum unknown.',fill='black');sheet.save(M/f'photo-before-after-{photo}.jpg')
print(json.dumps(meta))
