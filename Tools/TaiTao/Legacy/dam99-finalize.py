import sys,json,hashlib,re
from pathlib import Path
sys.path.insert(0,'Tools/python-deps')
from PIL import Image,ImageDraw,ImageFont
import numpy as np
R=Path('D:/ConRoi');D=R/'Test/KetQua/journey_v48/dam_r99';M=R/'Test/BangChung/journey_v48/dam_r99'
for id in ['181347','181402']:
 for rev in ['r98','candidate']:
  src=Image.open(R/f'Test/BugModel/Screenshot 2026-11-02 {id}.png').convert('RGBA').resize((1280,720));mask=Image.open(M/f'{rev}-apron-mask-{id}.png').convert('RGBA');a=np.array(mask.getchannel('A'));assert a.min()==0
  mask.putalpha(mask.getchannel('A').point(lambda p:int(p*.48)));out=Image.alpha_composite(src,mask).convert('RGB');dr=ImageDraw.Draw(out);dr.rectangle((0,0,1280,36),fill='white');dr.text((10,6),f'{id}: {rev.upper()} apron overlay 48%, same approximate camera / NOT VERIFIED',font=ImageFont.truetype('C:/Windows/Fonts/arial.ttf',20),fill=(175,20,20));out.save(M/f'overlay-{rev}-{id}.jpg',quality=94)
# Validate links in just the added sections; publish an exact inventory.
links=[]
for name in ['PROJECT_GUIDE_AND_HISTORY.md','BLENDER_UNITY_WORKFLOW.md']:
 p=R/'Docs'/name;text=p.read_text(encoding='utf-8');section=text.split('\n## ',1)[0]
 for target in re.findall(r'\]\(([^)]+)\)',section):
  if '://' not in target and not (p.parent/target).exists():links.append(dict(doc=name,target=target))
(D/'doc-links.json').write_text(json.dumps(dict(missing=links),indent=2));assert not links
files=[]
for folder in ['Source/Blender/JourneyV48/R99','Assets/Art/Environment/JourneyV48/DamR99','Test/Output/journey_v48/DamR99','Test/KetQua/journey_v48/dam_r99','Test/BangChung/journey_v48/dam_r99']:
 files.extend(p for p in (R/folder).rglob('*') if p.is_file() and p.name!='final-manifest.json')
files.extend((R/'Test/Tam').glob('dam99-*'));files.extend(R/'Docs'/n for n in ['PROJECT_GUIDE_AND_HISTORY.md','BLENDER_UNITY_WORKFLOW.md'])
for name in ['Assets/Scenes/JourneyPresentation_v48_r99.unity','Assets/Scenes/JourneyPresentation_v48_r99.unity.meta','Assets/Scripts/NhanVat/TuTheBaoBenCauV48.cs','Assets/Scripts/NhanVat/TuTheBaoBenCauV48.cs.meta','Assets/Scripts/MoiTruong/KhoiPhucCaoDoSauTaiR73.cs','Assets/Scripts/MoiTruong/KhoiPhucCaoDoSauTaiR73.cs.meta','ProjectSettings/EditorBuildSettings.asset','ProjectSettings/ProjectVersion.txt']:
 if (R/name).exists():files.append(R/name)
rows=[dict(path=str(p.relative_to(R)),bytes=p.stat().st_size,sha256=hashlib.file_digest(p.open('rb'),'sha256').hexdigest()) for p in sorted(set(files))]
(D/'final-manifest.json').write_text(json.dumps(dict(status='SCOPED_R99_INTEGRATED_REVIEWED_WITH_LIMITS_NO_BUILD',files=rows),indent=2));print('manifest',len(rows),'links',len(links))
