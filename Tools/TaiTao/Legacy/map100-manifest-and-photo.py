from pathlib import Path
import json,hashlib,sys
R=Path('D:/ConRoi');D=R/'Test/KetQua/journey_v48/map_r99_closeout';M=R/'Test/BangChung/journey_v48/map_r99_closeout';sys.path.insert(0,str(R/'Tools/python-deps'));from PIL import Image,ImageDraw,ImageFont,ImageOps
photo=next(p for p in (R/'Test/BugModel').iterdir() if '181347' in p.name);a=R/'Test/BangChung/journey_v48/dam_r99/across-lake-final-daylight.png';b=M/'across-lake-final-daylight.png';out=Image.new('RGB',(1800,480),'#14232d');dr=ImageDraw.Draw(out);font=ImageFont.truetype('C:/Windows/Fonts/arial.ttf',19)
for i,(p,label) in enumerate([(photo,'BugModel181347: original reference'),(a,'R99: historical same Unity camera'),(b,'R100: current same Unity camera')]):
 im=Image.open(p).convert('RGB');im=ImageOps.contain(im,(600,380));out.paste(im,(i*600+(600-im.width)//2,40+(380-im.height)//2));dr.text((i*600+12,10),label,font=font,fill='white')
dr.text((14,442),'Approximate reference direction; photo camera not calibrated. Main dam section unchanged r99/r100; no survey accuracy claimed.',font=font,fill='#dce6eb');out.save(M/'photo-r99-r100-across-lake.jpg',quality=92)
def digest(p):
 h=hashlib.sha256()
 with p.open('rb') as f:
  for chunk in iter(lambda:f.read(4*1024*1024),b''):h.update(chunk)
 return h.hexdigest()
paths=[R/'Source/Blender/JourneyV48/R100/OngDao_Dam_R100_Editable.blend',R/'Assets/Scenes/JourneyPresentation_v48_r100.unity',R/'Assets/Scenes/JourneyPresentation_v48_r100.unity.meta',R/'Assets/Art/Environment/JourneyV48/DamR100.meta',R/'Docs/PROJECT_GUIDE_AND_HISTORY.md',R/'Docs/BLENDER_UNITY_WORKFLOW.md',R/'BAT_DAU_TAI_DAY.md',R/'Test/Output/journey_v48/DamR100/OngDaoDamR100_Delta.fbx']
for folder in [R/'Assets/Art/Environment/JourneyV48/DamR100',M,D,R/'Test/BugModel']:
 paths.extend(p for p in folder.rglob('*') if p.is_file() and p.name!='final-manifest.json')
paths.extend(p for p in (R/'Test/Tam').glob('map100-*') if p.is_file());paths=sorted(set(paths),key=lambda p:str(p));rows=[dict(path=str(p.relative_to(R)).replace('\\','/'),bytes=p.stat().st_size,sha256=digest(p)) for p in paths]
assert digest(R/'Test/Output/journey_v48/DamR100/OngDaoDamR100_Delta.fbx')==digest(R/'Assets/Art/Environment/JourneyV48/DamR100/OngDaoDamR100_Delta.fbx')
baseline=json.loads((D/'preservation-final.json').read_text());assert baseline['protected']['unchanged']==1118 and baseline['tracked_original_gameplay_saves']['unchanged']==17
manifest=dict(status='R100_SCOPED_MAP_HANDOFF_WITH_OPEN_ITEMS_NO_BUILD_ZIP_RUNNER',source=rows[0]['path'] if False else 'Source/Blender/JourneyV48/R100/OngDao_Dam_R100_Editable.blend',scene='Assets/Scenes/JourneyPresentation_v48_r100.unity',fbx_output_vs_asset_hash_equal=True,protected_baseline_count=1118,tracked_original_gameplay_saves=17,files=rows)
(D/'final-manifest.json').write_text(json.dumps(manifest,ensure_ascii=False,indent=2),encoding='utf-8');print('RESULT manifest',len(rows),'listed files; copied FBX identical; protected1118/saves17 unchanged')
