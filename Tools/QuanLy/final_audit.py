from pathlib import Path
import json,hashlib,os,shutil,collections,subprocess
R=Path(__file__).resolve().parents[2];D=R/'Docs/QuanLy'
def sha(p):
 h=hashlib.sha256()
 with p.open('rb') as f:
  for b in iter(lambda:f.read(256*1024),b''):h.update(b)
 return h.hexdigest()
base=json.loads((D/'baseline.json').read_text(encoding='utf-8'))['rows'];changed=[];missing=[]
for row in base:
 p=R/row['path']
 if not p.exists():missing.append(row['path'])
 elif sha(p)!=row['sha256']:changed.append(row['path'])
allowed={'.gitignore','README.md','BAT_DAU_TAI_DAY.md','ProjectSettings/EditorBuildSettings.asset','Docs/PROJECT_GUIDE_AND_HISTORY.md','Docs/BLENDER_UNITY_WORKFLOW.md'}
archive=[]
for line in (D/'archive-manifest.jsonl').read_text(encoding='utf-8-sig').splitlines():
 row=json.loads(line);row['destinationOK']=Path(row['new']).is_file() and sha(Path(row['new'])).lower()==row['sha256'].lower();row['stagingOK']=Path(row['staging']).is_file() and sha(Path(row['staging'])).lower()==row['sha256'].lower();row['sourceAbsent']=not (R/row['old']).exists();archive.append(row)
# GUID/meta checks in the prepared source set, not a claim all historical assets are classified.
paths=[x for x in (D/'git-source-paths.txt').read_text(encoding='utf-8').splitlines() if (R/x).is_file()]
metaMissing=[x for x in paths if x.startswith('Assets/') and not x.endswith('.meta') and not Path(str(R/x)+'.meta').exists()]
sceneSame=sha(R/'Assets/Scenes/JourneyPresentation_v48_r107.unity')==sha(R/'Assets/ConRoi/Modules/TichHop/Scenes/ConRoi_Chinh.unity')
legacy=D/'Before'
result=dict(baseline_files=len(base),missing=missing,changed=changed,unexpected_changes=[x for x in changed if x not in allowed],archive_verified=sum(x['destinationOK'] and x['stagingOK'] and x['sourceAbsent'] for x in archive),archive_entries=len(archive),archive_failures=[{k:x[k] for k in ['old','destinationOK','stagingOK','sourceAbsent']} for x in archive if not(x['destinationOK'] and x['stagingOK'] and x['sourceAbsent'])],prepared_assets_missing_meta=metaMissing,integration_scene_bytes_equal_checkpoint=sceneSame,free_bytes=shutil.disk_usage(R).free)
previous=D/'final-audit.json'
if previous.exists() and not (D/'final-audit-superseded-hashcase.json').exists():shutil.copy2(previous,D/'final-audit-superseded-hashcase.json')
previous.write_text(json.dumps(result,ensure_ascii=False,indent=2),encoding='utf-8');print(json.dumps(result,ensure_ascii=False,indent=2))
