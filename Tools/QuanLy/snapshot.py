"""Hash baseline before archive/moves; no overwrite. Does not traverse Library."""
from pathlib import Path
import hashlib,json,datetime,os
ROOT=Path(__file__).resolve().parents[2];D=ROOT/'Docs/QuanLy';P=D/'baseline.json'
if P.exists():raise SystemExit('Baseline already exists; preserve it.')
inventory=[]
for top in ['Assets','Source','Packages','ProjectSettings']:
 for folder,dirs,files in os.walk(ROOT/top,followlinks=False):
  for name in files:inventory.append(dict(path=(Path(folder)/name).relative_to(ROOT).as_posix()))
for name in ['.gitignore','README.md','BAT_DAU_TAI_DAY.md','Docs/PROJECT_GUIDE_AND_HISTORY.md','Docs/BLENDER_UNITY_WORKFLOW.md','Docs/CON_ROI_GDD_v3.6_CHOT_TRIEN_KHAI.md']:inventory.append(dict(path=name))
rows=[]
for x in inventory:
 p=ROOT/x['path']
 if x['path'].split('/')[0] in ['Assets','Source','Packages','ProjectSettings'] or x['path'] in ['.gitignore','README.md','BAT_DAU_TAI_DAY.md','Docs/PROJECT_GUIDE_AND_HISTORY.md','Docs/BLENDER_UNITY_WORKFLOW.md','Docs/CON_ROI_GDD_v3.6_CHOT_TRIEN_KHAI.md']:
  h=hashlib.sha256()
  with p.open('rb') as f:
   for block in iter(lambda:f.read(256*1024),b''):h.update(block)
  rows.append(dict(path=x['path'],bytes=p.stat().st_size,sha256=h.hexdigest()))
P.write_text(json.dumps(dict(utc=datetime.datetime.now(datetime.timezone.utc).isoformat(),rows=rows),ensure_ascii=False,indent=2),encoding='utf-8')
print('Baseline saved:',len(rows),'files',sum(x['bytes'] for x in rows),'bytes')
