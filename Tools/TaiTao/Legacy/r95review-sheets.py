from pathlib import Path
import sys,json
sys.path.insert(0,'Tools/python-deps')
from PIL import Image,ImageDraw
M=Path('Test/BangChung/journey_v48/r95_visual_review')
sheet=Image.new('RGB',(1280,400*5),'#dddddd')
for i,tag in enumerate(['bridge','I2','gate','Langfarm','plaza']):
 for j,state in enumerate(['before','after']):
  im=Image.open(M/f'unity-{tag}-{state}.png').convert('RGB');im.thumbnail((640,365));tile=Image.new('RGB',(640,400),'white');tile.paste(im,(0,30));ImageDraw.Draw(tile).text((10,9),f'{tag} | r94 before' if state=='before' else f'{tag} | r95 after (before winding fix)',fill='black');sheet.paste(tile,(j*640,i*400))
sheet.save(M/'five-sites-r94-r95.jpg')
print('fixed-camera sheet ready')
