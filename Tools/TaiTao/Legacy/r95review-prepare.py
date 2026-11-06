import sys,json,hashlib
from pathlib import Path
sys.path.insert(0,'Tools/python-deps')
from PIL import Image,ImageDraw
R=Path('D:/ConRoi');D=R/'Test/KetQua/journey_v48/r95_visual_review';M=R/'Test/BangChung/journey_v48/r95_visual_review'
D.mkdir(parents=True,exist_ok=True);M.mkdir(parents=True,exist_ok=True)
paths=[R/'Source/Blender/JourneyV48/R95/Market_Ground_R95_Editable.blend',R/'Assets/Scenes/JourneyPresentation_v48_r94.unity',R/'Assets/Scenes/JourneyPresentation_v48_r95.unity',R/'Assets/Scripts/MoiTruong/KhoiPhucCaoDoSauTaiR73.cs',R/'Assets/Scripts/TuongTac/ChuanBiGoiHangR88.cs']
paths+=list((R/'Assets/Art/Environment/JourneyV48/MarketGroundR95').glob('*'))
out=[dict(path=p.relative_to(R).as_posix(),bytes=p.stat().st_size,sha256=hashlib.sha256(p.read_bytes()).hexdigest()) for p in paths if p.is_file()]
(D/'product-before-review.json').write_text(json.dumps(out,indent=2))
sheet=Image.new('RGB',(1800,405*5),'#dddddd');labels=[]
for i,p in enumerate(sorted((R/'Test/BugModel').glob('*.png'))):
 im=Image.open(p).convert('RGB');im.thumbnail((890,365));tile=Image.new('RGB',(900,405),'white');tile.paste(im,(5,35));ImageDraw.Draw(tile).text((5,7),p.name,fill='black');sheet.paste(tile,(i%2*900,i//2*405));labels.append(dict(file=str(p.relative_to(R)),sha256=hashlib.sha256(p.read_bytes()).hexdigest(),size=Image.open(p).size))
sheet.save(M/'BugModel-index.jpg');(D/'BugModel-source-index.json').write_text(json.dumps(labels,indent=2));print('Review baseline',len(out),'files; BugModel originals',len(labels))
