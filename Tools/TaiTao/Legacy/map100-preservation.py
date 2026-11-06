import json,hashlib
from pathlib import Path
R=Path('D:/ConRoi');D=R/'Test/KetQua/journey_v48/map_r99_closeout'
def digest(p):
 h=hashlib.sha256()
 with p.open('rb') as f:
  for chunk in iter(lambda:f.read(4*1024*1024),b''):h.update(chunk)
 return h.hexdigest()
def compare(rows):
 result=[]
 for row in rows:
  p=Path(row['path']);p=p if p.is_absolute() else R/p;actual=digest(p) if p.is_file() else None;result.append(dict(path=str(p),before=row['sha256'],after=actual,unchanged=actual==row['sha256']))
 return dict(count=len(result),unchanged=sum(x['unchanged'] for x in result),changed=[x for x in result if not x['unchanged']],files=result)
report=dict(protected=compare(json.loads((D/'protected-start.json').read_text())),tracked_original_gameplay_saves=compare(json.loads((R/'Docs/Reviews/journey_v48/restructure-20261102/user-saves-before.json').read_text())),scope='Only files enumerated in these baselines. No independent claim about all 523 saves. CrashReporter excluded: telemetry, not gameplay save.')
(D/'preservation-final.json').write_text(json.dumps(report,indent=2));print(json.dumps({k:{q:v for q,v in x.items() if q!='files'} for k,x in report.items() if isinstance(x,dict)},indent=2))
old=json.loads((D/'old-height-private.before.json').read_text());old_after=json.loads((D/'old-height-private.json').read_text());migrated=json.loads((D/'migrated-private.json').read_text());rows=[]
for i in range(6):
 actual=json.loads((D/f'repeat-load-{i}.json').read_text());expected=old if i<3 else migrated;changed=[k for k in set(expected)|set(actual) if expected.get(k)!=actual.get(k)];rows.append(dict(load=i,input='old-height-private' if i<3 else 'migrated-private',position=actual['position'],different_top_level_fields=changed))
(D/'save-comparison.json').write_text(json.dumps(dict(old_input_unchanged=old==old_after,old_input_hash_unchanged=digest(D/'old-height-private.before.json')==digest(D/'old-height-private.json'),loads=rows,limit='One copied valid profile, two elevation variants, six loads. Not all historic saves.'),indent=2));print('SAVE_COMPARISON',json.dumps(rows))
