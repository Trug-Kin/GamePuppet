import json,hashlib,csv,re,math
from pathlib import Path
R=Path('D:/ConRoi');D=R/'Test/KetQua/journey_v48/ground_r93';M=R/'Test/BangChung/journey_v48/ground_r93'
def read(p):return json.loads(p.read_text(encoding='utf-8-sig'))
def sha(p):
 with p.open('rb') as f:return hashlib.file_digest(f,'sha256').hexdigest()
old=read(R/'Test/KetQua/journey_v48/r93/product-before-integration.json');checks=[]
for rel,h in old.items():
 p=R/rel;checks.append(dict(path=rel,exists=p.exists(),same=p.is_file() and sha(p)==h))
handoff=read(R/'Test/KetQua/journey_v48/r93/handoff-manifest.json')
for rel,item in handoff.items():
 if rel.startswith(('Assets/','Source/Blender/')):
  p=R/rel;checks.append(dict(path=rel,exists=p.exists(),same=p.is_file() and sha(p)==item['sha256']))
(D/'protected-product-final.json').write_text(json.dumps(dict(checked=len(checks),differences=[r for r in checks if not r['same']],scope='Existing r93 handoff product/source and earlier protected scripts, Packages, ProjectSettings, r92 scene/meta; newly created study files are outside product.',files=checks),indent=2))
# Human-readable measured segments. After is explicitly unchanged product, not a study promoted to product.
rows=[]
for item in read(D/'atlas-sections.json'):
 for layer in item['layers']:
  for seg in layer['segments']:
   rows.append(dict(set='atlas',region=item['region'],section=item['section'],layer=layer['path'],start_xz=json.dumps(item['start_xz']),end_xz=json.dumps(item['end_xz']),segment_i0=seg['i0'],segment_i1=seg['i1'],length_authoring_m=seg['length'],delta_authoring_m=seg['delta'],before_grade_percent=seg['grade_percent'],after_product_grade_percent=seg['grade_percent'],product_status='UNCHANGED; not controller-verified in this survey'))
ss=read(D/'supplement-sections.json')
for item in read(D/'supplement-layers.json'):
 s=ss[item['section']]
 for seg in item['segments']:
  if seg['length']<1:continue
  rows.append(dict(set='supplement',region=s['name'].split('-')[0],section=s['name'],layer=item['path'],start_xz=json.dumps(s['start']),end_xz=json.dumps(s['end']),segment_i0=seg['i0'],segment_i1=seg['i1'],length_authoring_m=seg['length'],delta_authoring_m=seg['delta'],before_grade_percent=seg['grade_percent'],after_product_grade_percent=seg['grade_percent'],product_status='UNCHANGED; not controller-verified in this survey'))
with (D/'sections-selected-before-after.csv').open('w',encoding='utf-8-sig',newline='') as f:
 w=csv.DictWriter(f,fieldnames=list(rows[0]));w.writeheader();w.writerows(rows)
newlinks=[]
for name in ['PROJECT_GUIDE_AND_HISTORY.md','BLENDER_UNITY_WORKFLOW.md']:
 p=R/'Docs'/name;t=p.read_text(encoding='utf-8-sig').split('\n---\n',1)[0]
 for link in re.findall(r'\]\(([^)]+)\)',t):
  if link.startswith(('http:','https:','#')):continue
  q=(p.parent/link).resolve();newlinks.append(dict(document=name,link=link,exists=q.exists()))
(D/'document-links.json').write_text(json.dumps(newlinks,indent=2));assert all(r['exists'] for r in newlinks)
report=dict(status='INCOMPLETE_GROUND_REVIEW_NO_PRODUCT_INTEGRATION',scene='Assets/Scenes/JourneyPresentation_v48_r93.unity',source='Source/Blender/JourneyV48/R93/GroundSurvey_R93_Editable.blend',open_collection='GROUND_NATIVE_MATCHED_R93',collider_collection='COLLIDERS_NOT_FOR_RENDER',scope_plan='Test/BangChung/journey_v48/ground_r93/scope-north-up-with-median.png',model_circular_islands=[dict(id='I1',xz=[610.67,563.99],real_world='UNVERIFIED estimated native island'),dict(id='I2',xz=[714.94,459.48],real_world='P02 relation, unsurveyed heights'),dict(id='I3',xz=[783.948,191.167],real_world='P03 relation, unsurveyed heights')],additional_scope=[dict(id='M1',status='native tree attribution and shared terrain measured; footprint UNVERIFIED'),dict(id='M2_P02_splitter',status='object attribution OPEN; no invented coordinate')],sections=dict(primary=106,branch=54,supplement=23,branch_mouths_geometric_probes=9,csv_segments=len(rows)),confirmed_fault=dict(region='I2',start_xz=[714.94,473.48],end_xz=[714.94,473.98],length=.5,delta=.291962,grade_percent=58.3924,render='Surface_128',collider='Context_110',product_fix='NOT_APPLIED'),langfarm_cross_slope=dict(source='native v33 terrain, not new r90/r93 height change',surface_role='bare terrain / apron candidate; road/footway boundary not verified',length=7,drop_range=[.760,.775],percent_range=[10.86,11.08],action='UNCHANGED; real-world appropriateness OPEN'),accepted_product_changes=[],trial_files='blender-handoff-state.json',unity_integration='NOT_RUN',controller='NOT_RUN for new broad ground scope',pause_old_height_save_load='NOT_RUN',producer_lifecycle='NOT_RUN; static owner reading only',video='NOT_PRODUCED; diagnostic Edit renders are not gameplay',fbx='NOT_EXPORTED: Blender correction gate incomplete',build='NOT_RUN / not requested',remaining=['I2 coherent full-width transition and topology gate','Road/terrain/footway/collider contact and overlap classification','Footprint attribution of all traffic separators, including P02 small splitter','Real local grades around Langfarm and market side aprons','Post-integration controller/prop-contact/lifecycle checks'],protected_difference_count=sum(not r['same'] for r in checks))
(D/'final-state.json').write_text(json.dumps(report,indent=2,ensure_ascii=False));assert report['protected_difference_count']==0
files={}
for folder in [D,M,R/'Source/Blender/JourneyV48/R93']:
 for p in folder.iterdir():
  if p.is_file() and p.suffix in ['.json','.csv','.png','.blend'] and p.name!='survey-manifest.json':files[str(p.relative_to(R)).replace('\\','/')]=dict(bytes=p.stat().st_size,sha256=sha(p))
(D/'survey-manifest.json').write_text(json.dumps(files,indent=2));print('Product comparisons',len(checks),'differences',report['protected_difference_count'],'; CSV segments',len(rows),'; links',len(newlinks),'; manifest',len(files))
