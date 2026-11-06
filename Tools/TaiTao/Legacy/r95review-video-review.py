import sys,subprocess,json
from pathlib import Path
sys.path.insert(0,'Tools/python-deps')
import imageio_ffmpeg
from PIL import Image,ImageDraw
D=Path('Test/KetQua/journey_v48/r95_visual_review');M=Path('Test/BangChung/journey_v48/r95_visual_review');v=M/'bridge-market-return-fixed-controller.mp4';ff=imageio_ffmpeg.get_ffmpeg_exe()
g=imageio_ffmpeg.read_frames(str(v));meta=next(g);g.close();frames,secs=imageio_ffmpeg.count_frames_and_secs(str(v));meta.update(frame_count=frames,counted_duration=secs,average_fps=frames/secs,source_fps=30,input='synthetic InputSystem keyboard/mouse; unchanged controller',audio=False,route='one initial bridge fixture, 42 waypoints, no intervening placement; connected outward+return; not story playthrough',lighting_switch_route_seconds=169.4569637)
(D/'video-metadata.json').write_text(json.dumps(meta,indent=2))
times=[4,20,26,40,52,75,90,110,125,145,165,180,200,220,240,258,270,282];sheet=Image.new('RGB',(1500,315*6),'#dddddd')
for i,t in enumerate(times):
 p=M/f'route-frame-{t:03}.jpg';subprocess.run([ff,'-y','-ss',str(t),'-i',str(v),'-frames:v','1','-q:v','3',str(p)],stdout=subprocess.DEVNULL,stderr=subprocess.DEVNULL,check=True);im=Image.open(p).convert('RGB');im.thumbnail((500,285));tile=Image.new('RGB',(500,315),'white');tile.paste(im,(0,27));ImageDraw.Draw(tile).text((8,7),f'Actual capture t={t}s',fill='black');sheet.paste(tile,(i%3*500,i//3*315))
sheet.save(M/'connected-route-filmstrip.jpg')
sheet=Image.new('RGB',(1280,400*5),'#dddddd')
for i,tag in enumerate(['bridge','I2','gate','Langfarm','plaza']):
 for j,state in enumerate(['before','fixed']):
  im=Image.open(M/f'unity-{tag}-{state}.png').convert('RGB');im.thumbnail((640,365));tile=Image.new('RGB',(640,400),'white');tile.paste(im,(0,30));ImageDraw.Draw(tile).text((10,9),f'{tag} | '+('r94 before game grading' if state=='before' else 'r96: r95 grade + winding fix'),fill='black');sheet.paste(tile,(j*640,i*400))
sheet.save(M/'five-sites-before-final.jpg');print(json.dumps(meta))
