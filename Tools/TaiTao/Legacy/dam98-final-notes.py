from pathlib import Path
import json,hashlib
r=Path('D:/ConRoi');d=r/'Test/KetQua/journey_v48/dam_r98';keys=['OD35','Garden_Bank_Return_Walk','R11_Local_Bridge_Supports','R97_Bridgehead','R98_OngDao'];hits=[]
for p in (r/'Assets/Scripts').rglob('*.cs'):
 if 'Editor' in p.parts:continue
 s=p.read_text(encoding='utf-8-sig')
 if any(k in s for k in keys):hits.append(str(p.relative_to(r)))
(d/'producer-source-review.json').write_text(json.dumps(dict(exact_runtime_geometry_name_references=hits,limitation='No exact-name runtime producer matches in non-Editor Scripts; this alone does not prove absence of generic producers. Runtime root/collider binding through Play/load/region return checked separately.',runtime_evidence='region-return.txt'),indent=2))
(d/'construction-failures-history.json').write_text(json.dumps(dict(scope='Construction checks prior to final export, not final gameplay failures',issues=[dict(step='first refine',result='ERROR',cause='Unused material R98_Ridge not saved because zero users',fix='Use actual referenced source materials; rerun before export'),dict(step='first export',result='FAIL',cause='Gate stair triangulation produced zero-area face',fix='Closed explicit fan caps, source repaired before integration'),dict(step='rail delivery gate',result='FAIL',cause='Segmented top tubes with coincident opposing caps, 39..59 back-to-back faces reported per rail after generic weld',fix='Single continuous top tube; final20 part gates pass'),dict(step='first final-bind compilation',result='FAIL',cause='Mesh namespace ambiguity; no action executed',fix='UnityEngine.Mesh qualified; rerun succeeded')],raw_topology_history='topology-final.json; current gates are <key>-gate.json'),indent=2))
for name in ['Docs/PROJECT_GUIDE_AND_HISTORY.md','Docs/BLENDER_UNITY_WORKFLOW.md']:
 p=r/name;s=p.read_text(encoding='utf-8');first,end=s.split('\n## ',1);first=first.replace('1600×982','1600×984').replace('giữ log thô lần đầu','giữ báo cáo topology ban đầu và lịch sử lỗi đã sửa');p.write_text(first+'\n## '+end,encoding='utf-8')
print('Runtime exact-name producer hits',hits)
