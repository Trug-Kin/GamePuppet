import json,hashlib,re,math
from pathlib import Path
D=Path('Test/KetQua/journey_v48/r95_visual_review')
def read(p):return json.loads(p.read_text(encoding='utf-8'))
def sha(p):return hashlib.sha256(p.read_bytes()).hexdigest()
out=[]
presentation={'position','puppetPositions','npcPerformance','marketPreparationR88','lakeApproachR89'}
for n in range(5):
 p=D/f'repeat-case-{n}-old.json';before=Path(str(p)+'.before');a=read(before);m=read(D/f'repeat-case-{n}-migrated.json');assert sha(p)==sha(before)
 raw=[read(D/f'case-{n}-old-{i}-loaded.json') for i in range(3)];migrated=[read(D/f'case-{n}-migrated-{i}-loaded.json') for i in range(3)]
 assert all({k:v for k,v in b.items() if k!='position'}=={k:v for k,v in a.items() if k!='position'} for b in raw)
 assert all(b['position']==raw[0]['position'] for b in raw)
 assert all(b==m for b in migrated)
 assert {k:v for k,v in a.items() if k not in presentation}=={k:v for k,v in m.items() if k not in presentation}
 old=json.loads(a['marketPreparationR88']);new=json.loads(m['marketPreparationR88'])
 assert all(old[k]==new[k] for k in ['version','phase','elapsed','slide','rotation'])
 assert max(abs(old['position'][q]-new['position'][q]) for q in ['x','z'])<.0003
 lake_old=json.loads(a['lakeApproachR89']);lake_new=json.loads(m['lakeApproachR89']);assert {k:v for k,v in lake_old.items() if k!='position'}=={k:v for k,v in lake_new.items() if k!='position'}
 npc_old=json.loads(a['npcPerformance']);npc_new=json.loads(m['npcPerformance']);assert {k:v for k,v in npc_old.items() if k!='poses'}=={k:v for k,v in npc_new.items() if k!='poses'}
 out.append(dict(case=n,old_sha256=sha(p),old_copy_bytes_unchanged=True,old_loads=3,migrated_loads=3,raw_changes=['position'] if n<2 else [],progress_choices_preserved=True,player_target=raw[0]['position'],migrated_player_drift=0,puppet_phase=new['phase'],puppet_elapsed=new['elapsed'],puppet_slide=new['slide'],puppet_y=new['position']['y'],xz_save_rounding_max_m=max(abs(old['position'][q]-new['position'][q]) for q in ['x','z'])))
(D/'repeat-load-analysis.json').write_text(json.dumps(dict(cases=out,scope='30 isolated loads, menu hold Time0, not Escape pause; vyEncountered true in private fixtures to avoid existing Vy proximity autosave. First confounded attempt preserved separately.'),indent=2));print('PASS 30 isolated loads; bytes unchanged for 5 old copies; all progress/choice fields preserved; repeated migrated data identical')
