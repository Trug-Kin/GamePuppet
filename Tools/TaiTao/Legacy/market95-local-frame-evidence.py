import sys,json,subprocess
from pathlib import Path
sys.path.insert(0,'Tools/python-deps')
import imageio_ffmpeg as ff
from PIL import Image,ImageDraw
M=Path('Test/BangChung/journey_v48/market_ground_r95');D=Path('Test/KetQua/journey_v48/market_ground_r95');metadata=[];tiles=[]
for tag,times in [('branch-final',[7,20,40,65]),('north-final',[5,20,40,60])]:
 p=M/('market95-'+tag+'-controller.mp4')
 if not p.exists():continue
 r=ff.read_frames(str(p));meta=next(r);r.close();metadata.append(dict(clip=str(p),metadata=meta,source_fps=30,audio=False,input='synthetic InputSystem keyboard/mouse through unchanged controller; separate single-start local fixture'))
 for t in times:
  if t>=meta['duration']:continue
  q=M/(tag+'-controller-'+str(t)+'.png');subprocess.run([ff.get_ffmpeg_exe(),'-v','error','-y','-ss',str(t),'-i',str(p),'-frames:v','1',str(q)],check=True)
  im=Image.open(q).convert('RGB');im.thumbnail((600,380));tile=Image.new('RGB',(610,410),'white');tile.paste(im,(5,25));ImageDraw.Draw(tile).text((5,5),tag+' FINAL controller t='+str(t)+'s',fill='black');tiles.append(tile)
sheet=Image.new('RGB',(1220,410*((len(tiles)+1)//2)),'#dddddd')
for i,t in enumerate(tiles):sheet.paste(t,(i%2*610,i//2*410))
sheet.save(M/'final-controller-filmstrip.jpg');(D/'final-controller-metadata.json').write_text(json.dumps(metadata,indent=2));print('Local videos decoded',[(r['clip'],r['metadata']['duration'],r['metadata']['size'],r['metadata']['fps']) for r in metadata])
