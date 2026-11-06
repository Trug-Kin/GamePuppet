import sys,subprocess,json
from pathlib import Path
sys.path.insert(0,'Tools/python-deps')
import imageio_ffmpeg
from PIL import Image,ImageDraw,ImageFont
M=Path('Test/BangChung/journey_v48/market_ground_r95');D=Path('Test/KetQua/journey_v48/market_ground_r95');ff=imageio_ffmpeg.get_ffmpeg_exe();times=[5,35,70,100,135,165,190,220,250,280,305]
for t in times:
 subprocess.run([ff,'-y','-ss',str(t),'-i',str(M/'market95-controller.mp4'),'-frames:v','1',str(M/('controller-'+str(t)+'.png'))],stdout=subprocess.DEVNULL,stderr=subprocess.DEVNULL)
imgs=[]
for t in times:
 p=M/('controller-'+str(t)+'.png')
 if p.exists():
  im=Image.open(p).convert('RGB');im.thumbnail((500,300));tile=Image.new('RGB',(510,330),'white');tile.paste(im,(5,25));ImageDraw.Draw(tile).text((8,5),'Continuous controller clip t='+str(t)+' s',fill='black');imgs.append(tile)
sheet=Image.new('RGB',(1530,330*((len(imgs)+2)//3)),'#dddddd')
for i,im in enumerate(imgs):sheet.paste(im,((i%3)*510,(i//3)*330))
sheet.save(M/'controller-filmstrip.jpg');print('Decoded video frames',len(imgs))
