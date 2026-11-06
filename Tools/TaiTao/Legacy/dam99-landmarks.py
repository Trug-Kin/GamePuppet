import sys
sys.path.insert(0,'Tools/python-deps')
from PIL import Image,ImageDraw
from pathlib import Path
import sys,json
R=Path('D:/ConRoi');D=R/'Test/KetQua/journey_v48/dam_r99';M=R/'Test/BangChung/journey_v48/dam_r99'
j=json.loads((D/'photo-fit-initial.json').read_text())
for row in j['photos']:
 im=Image.open(R/f'Test/BugModel/Screenshot 2026-11-02 {row["id"]}.png').convert('RGB');dr=ImageDraw.Draw(im)
 for tag,pt in zip(['C0','C1','T0','T1','R0','R1'],row['points']):
  x,y=pt;dr.ellipse((x-5,y-5,x+5,y+5),outline='red',width=2);dr.text((x+7,y-15),tag,fill='red')
 box=(880,420,1780,640) if row['id']=='181347' else (0,460,920,710)
 im.crop(box).resize((1800,int((box[3]-box[1])*2))).save(M/f'landmarks-{row["id"]}.png')

