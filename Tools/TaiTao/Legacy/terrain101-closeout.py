import json,hashlib,re
from pathlib import Path
R=Path('D:/ConRoi');D=R/'Test/KetQua/journey_v48/terrain_r101'
def sha(p):
 h=hashlib.sha256()
 with p.open('rb') as f:
  for c in iter(lambda:f.read(4194304),b''):h.update(c)
 return h.hexdigest()
def compare(name):
 rows=json.loads((D/name).read_text());out=[]
 for r in rows:
  p=Path(r['path']);a=sha(p) if p.is_file() else None;out.append(dict(path=str(p),before=r['sha256'],after=a,unchanged=a==r['sha256']))
 return dict(count=len(out),unchanged=sum(r['unchanged'] for r in out),changed=[r for r in out if not r['unchanged']],files=out)
report=dict(protected=compare('protected-start.json'),tracked_original_gameplay_saves=compare('original-saves-start.json'),limit='Only enumerated 1140 files and 17 gameplay saves. No independent hash claim about all 523 saves. CrashReporter is separate telemetry.')
(D/'preservation-final.json').write_text(json.dumps(report,indent=2));print('PRESERVATION',[(k,v['count'],v['unchanged']) for k,v in report.items() if isinstance(v,dict)])
if (D/'repeat-load.txt').exists():
 fields=['version','nextRegion','region','awareness','actionPoints','interactions','ignored','traces','faces','witnessed','posted','called','dismissed','schoolTrace','schoolFace','schoolWitnessed','schoolLeading','schoolTalks','schoolReadMask','slopeTrace','slopeFace','slopeWitnessed','finalStarted','finalStarts','slopeTalks','slopeReadMask','compliance','endingBeat','ending','dreamCycles','baoTalks','haTalks','lanVisitMask']
 out=[]
 for i in range(6):
  name='copied-old-height-input.json' if i<3 else 'current-height-input.json';before=json.loads((D/name).read_text());after=json.loads((D/f'load-capture-{i}.json').read_text());out.append(dict(load=i,input=name,position=after['position'],progress_choice_fields_changed=[k for k in fields if before.get(k)!=after.get(k)],all_top_level_fields_changed=[k for k in before if before.get(k)!=after.get(k)]))
 (D/'save-comparison.json').write_text(json.dumps(dict(loads=out,checked_progress_choice_fields=fields,scope='One copied valid profile; edited old-height fixture plus current-height captures. Not every historic save.'),indent=2));print('LOADS',out)
route=(D/'route-motion.txt').read_text();pts=[tuple(map(float,m.groups())) for m in re.finditer(r'p=\(([-\d.]+), ([-\d.]+), ([-\d.]+)\)',route)];m=re.search(r'END reached=(\d+)/(\d+) seconds=([\d.]+)',route)
summary=dict(reached=int(m[1]),waypoints=int(m[2]),motion_seconds=float(m[3]),recorded_samples=len(pts),min_position=[min(p[i] for p in pts) for i in range(3)],max_position=[max(p[i] for p in pts) for i in range(3)],input='synthetic InputSystem keyboard/mouse through unchanged controller',audio=False,initial_placement='One copied profile/adjacent good-ground fixture X654/Z414; no placement during route',route='X654/Z414 -> X652/414 -> X646/414 -> X644.6/414 -> X646/416 -> X648/418 -> X649.4/420 (r97 endpoint) -> reverse via same joins -> X654/414',limits='Not New Game, hardware input, all water edges, full bridge/dam regression or all historic saves. X643.5/Z414 is the steep bank edge, visually/ray checked; controller goes on the dry landward shelf.')
(D/'controller-summary.json').write_text(json.dumps(summary,indent=2));print('ROUTE',summary)
assert report['protected']['unchanged']==report['protected']['count'];assert report['tracked_original_gameplay_saves']['unchanged']==17
