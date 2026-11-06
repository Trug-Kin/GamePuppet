import json
from pathlib import Path
D=Path('Test/KetQua/journey_v48/ground_r93');ss=json.loads((D/'sections.json').read_text());ls=json.loads((D/'layer-sections.json').read_text());k=next(i for i,s in enumerate(ss) if s['region']=='I2_NE' and s['name']=='ray_90')
for r in ls:
 if r['section']!=k:continue
 if any(v is not None for v in r['top'][:36]):print(r['path'],[(ss[k]['distance'][i],v) for i,v in enumerate(r['top'][:36]) if v is not None])
