"""Prepare an explicit reviewable staging list; do not stage or change assets."""
import json,hashlib,shutil,re,collections
from pathlib import Path
R=Path(__file__).resolve().parents[2];D=R/'Docs/QuanLy'
closure=json.loads((D/'git-asset-closure.json').read_text(encoding='utf-8'));selected=set()
def add(p):
 p=Path(p)
 if p.is_file():selected.add(p.relative_to(R).as_posix())
def tree(p):
 for f in p.rglob('*'):
  if f.is_file() and '__pycache__' not in f.parts:add(f)
for name in closure['paths']:
 p=R/name
 if not p.is_file():continue
 add(p);add(Path(str(p)+'.meta'))
 parent=p.parent
 while parent!=R and parent!=R/'Assets':add(Path(str(parent)+'.meta'));parent=parent.parent
for name in ['Source','Packages','ProjectSettings','Tools','Docs/QuanLy','Docs/QA']:
 tree(R/name)
for name in ['.gitignore','.gitattributes','README.md','BAT_DAU_TAI_DAY.md','ConRoi.code-workspace','Docs/PROJECT_GUIDE_AND_HISTORY.md','Docs/BLENDER_UNITY_WORKFLOW.md','Docs/CON_ROI_GDD_v3.6_CHOT_TRIEN_KHAI.md']:
 add(R/name)
# Keep original small handoff docs that new QA copies link to, not user profiles/capture outputs.
for p in (R/'Test/KetQua').rglob('*.md'):add(p)
for p in (R/'Docs').rglob('*.md'):
 if 'Reviews' not in p.parts and 'Archives' not in p.parts:add(p)
tree(R/'.agents')
# Explicit private/transient reports are never source deliverables.
selected={x for x in selected if x not in ['Docs/QuanLy/live-state-preserved.json','Docs/QA/GitPreparation/technical-live-state.json'] and not x.startswith('Docs/QuanLy/KiemKe/')}
# Full inventory is retained locally; lightweight summary is useful in Git.
add(D/'KiemKe/summary.json')
attrs=(R/'.gitattributes').read_text(encoding='utf-8')
if '*.blend1 filter=lfs' not in attrs:attrs+='*.blend1 filter=lfs diff=lfs merge=lfs -text\n'
(R/'.gitattributes').write_text(attrs,encoding='utf-8')
ignore=(R/'.gitignore').read_text(encoding='utf-8')
for name in ['/Docs/QA/GitPreparation/technical-live-state.json','/Docs/QuanLy/live-state-preserved.json','/Docs/QuanLy/KiemKe/files.json']:
 if name not in ignore:ignore+='\n'+name+'\n'
(R/'.gitignore').write_text(ignore,encoding='utf-8')
patterns=[re.compile(x) for x in [r'gh[pousr]_[A-Za-z0-9]{25,}',r'github_pat_[A-Za-z0-9_]{40,}',r'sk-[A-Za-z0-9_-]{35,}',r'AKIA[A-Z0-9]{16}']]
suspect=[]
for name in sorted(selected):
 p=R/name
 if p.suffix.lower() in ['.cs','.py','.ps1','.json','.md','.txt','.yaml','.yml','.config'] and p.stat().st_size<3_000_000:
  try:text=p.read_text(encoding='utf-8-sig')
  except (UnicodeError,OSError):continue
  if any(q.search(text) for q in patterns):suspect.append(name)
large_mesh=set(json.loads((D/'file-setup.json').read_text(encoding='utf-8'))['large_asset_lfs'])
lfs_ext={'.blend','.blend1','.fbx','.png','.jpg','.jpeg','.tga','.exr','.hdr','.psd','.wav','.ogg','.mp3','.mp4','.glb','.dll','.zip'}
baseline={r['path']:r for r in json.loads((D/'baseline.json').read_text(encoding='utf-8'))['rows']}
objects={};unknown_lfs=0
for name in selected:
 p=R/name
 if p.suffix.lower() in lfs_ext or name in large_mesh:
  old=baseline.get(name)
  if old:objects[old['sha256']]=old['bytes']
  else:unknown_lfs+=p.stat().st_size
remaining_assets=[]
for p in (R/'Assets').rglob('*'):
 if p.is_file() and p.relative_to(R).as_posix() not in selected:remaining_assets.append(dict(path=p.relative_to(R).as_posix(),bytes=p.stat().st_size,status='UNCLASSIFIED_HELD_NOT_STAGED'))
(D/'git-source-paths.txt').write_text('\n'.join(sorted(selected))+'\n',encoding='utf-8')
(D/'unclassified-assets.json').write_text(json.dumps(remaining_assets,ensure_ascii=False,indent=2),encoding='utf-8')
report=dict(files=len(selected),bytes=sum((R/x).stat().st_size for x in selected),estimated_unique_lfs_bytes=sum(objects.values())+unknown_lfs,free_bytes=shutil.disk_usage(R).free,secret_pattern_files=suspect,unclassified_asset_files=len(remaining_assets),policy='Explicit current/checkpoint + compile/resource/prefab closure; all Source retained. Working tree legacy retained untracked, never discarded. No remote.')
(D/'git-stage-plan.json').write_text(json.dumps(report,ensure_ascii=False,indent=2),encoding='utf-8')
print(json.dumps(report,ensure_ascii=False,indent=2))
