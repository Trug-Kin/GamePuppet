from pathlib import Path
import json,hashlib,re
R=Path('D:/ConRoi');D=R/'Test/KetQua/journey_v48/r95_visual_review';M=R/'Test/BangChung/journey_v48/r95_visual_review'
def digest(p):
 h=hashlib.sha256()
 with p.open('rb') as f:
  for chunk in iter(lambda:f.read(1024*1024),b''):h.update(chunk)
 return h.hexdigest()
keys=[r['key'] for r in json.loads((D/'winding-source-fix.json').read_text())['fixes']]
for key in keys:
 a=R/'Test/Output/journey_v48/MarketGroundR96'/(key+'.fbx');b=R/'Assets/Art/Environment/JourneyV48/MarketGroundR96'/(key+'.fbx');assert digest(a)==digest(b)
paths=[R/'Source/Blender/JourneyV48/R96/Market_Ground_R96_Editable.blend',R/'Assets/Scenes/JourneyPresentation_v48_r96.unity',R/'Assets/Scenes/JourneyPresentation_v48_r96.unity.meta',R/'Docs/PROJECT_GUIDE_AND_HISTORY.md',R/'Docs/BLENDER_UNITY_WORKFLOW.md']
for folder in [R/'Assets/Art/Environment/JourneyV48/MarketGroundR96',R/'Test/Output/journey_v48/MarketGroundR96',M]:paths.extend(p for p in folder.rglob('*') if p.is_file())
paths.extend(p for p in (R/'Test/Tam').glob('r95review-*') if p.is_file());paths.extend(p for p in D.iterdir() if p.is_file() and p.name not in ['source-assets-review-manifest.json','Editor-review.log'])
rows=[dict(path=p.relative_to(R).as_posix(),bytes=p.stat().st_size,sha256=digest(p)) for p in sorted(set(paths))]
(D/'source-assets-review-manifest.json').write_text(json.dumps(dict(algorithm='SHA256',scope='Exact listed derivative source, Unity assets/meta/scene, docs, recipe and scoped evidence; original checkpoints/saves separately in preservation-review.json; no build/ZIP.',files=rows),indent=2))
out=dict(source='Source/Blender/JourneyV48/R96/Market_Ground_R96_Editable.blend',scene='Assets/Scenes/JourneyPresentation_v48_r96.unity',preserved=['r94','r95','checkpoints','original gameplay saves'],product_change='scoped winding only; zero vertex changes; no new migration/controller/gameplay code',route=dict(waypoints='42/42',motion_seconds=286.011798099999,capture_seconds=283.93,placement='one initial bridge fixture only',input='synthetic InputSystem',audio=False),migration='30 isolated loads, 5 old copies byte-identical; progression and choice fields unchanged; migrated loads identical; independent Escape pause passed',open=['M2 photo-to-object correspondence UNVERIFIED','bridge-bank structural fidelity not certified','small triangular/shading marks at I2 not classified as grade/hole','hidden contacts and untraversed branches/outer edges not covered','legacy render/collider differences outside scope','URP Inspector OPEN','V32 UNVERIFIED','hardware/headphones/new player NOT_RUN','Garden/r51_trial separate'],next='Model the I2-side bridge head, grass bank, intermediate path and second stair/lower path in Blender against BugModel181952/182010; retain market grade.',no_build_zip=True)
(D/'review-outcome.json').write_text(json.dumps(out,indent=2,ensure_ascii=False));print('MANIFEST',len(rows),'FBX hashes',len(keys),'source',next(r['sha256'] for r in rows if r['path'].endswith('Market_Ground_R96_Editable.blend')))
# Validate newly added document links, not all historical entries.
for doc in ['PROJECT_GUIDE_AND_HISTORY.md','BLENDER_UNITY_WORKFLOW.md']:
 p=R/'Docs'/doc;s=p.read_text(encoding='utf-8');s=s.split('## 2026-11-03 — r95:')[0] if doc.startswith('PROJECT') else s.split('## 2026-11-03 — Nền chợ r95:')[0]
 for target in re.findall(r'\]\(([^)]+)\)',s):assert (p.parent/target).exists(),target
 print('new links exist:',doc)
