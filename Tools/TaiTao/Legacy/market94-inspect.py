import json,hashlib,sys
from pathlib import Path
sys.path.insert(0,'Tools/python-deps')
import numpy as np
from PIL import Image,ImageDraw
R=Path('D:/ConRoi');D=R/'Test/KetQua/journey_v48/market_ground_r95';M=R/'Test/BangChung/journey_v48/market_ground_r95';D.mkdir(parents=True,exist_ok=True);M.mkdir(parents=True,exist_ok=True)
files=list((R/'Assets/Scenes').glob('*r9[234].unity*'))+list((R/'Source/Blender/JourneyV48/R93').glob('*.blend'))+list((R/'Assets/Art/Environment/JourneyV48/I2GroundR94').rglob('*'))
def sha(p):
 with p.open('rb') as f:return hashlib.file_digest(f,'sha256').hexdigest()
(D/'protected-start.json').write_text(json.dumps({str(p.relative_to(R)):sha(p) for p in files if p.is_file()},indent=2))
photos=[next((R/'Docs/Photo_3D_Model_Source/Photo_renumbered').glob(f'{n}_*')) for n in ['02','03','36','37'] if list((R/'Docs/Photo_3D_Model_Source/Photo_renumbered').glob(f'{n}_*'))]
photos+=list((R/'Test/BugModel').glob('*.png'))
im=Image.new('RGB',(1200,300*((len(photos)+2)//3)),(35,35,35));dr=ImageDraw.Draw(im)
for i,p in enumerate(photos):
 a=Image.open(p);a.thumbnail((400,270));x=i%3*400;y=i//3*300;im.paste(a,(x,y));dr.text((x,y+270),p.name[:42],fill='white')
im.save(M/'source-and-bug-sheet.jpg');print('PROTECTED',len(files),'PHOTOS',len(photos))
