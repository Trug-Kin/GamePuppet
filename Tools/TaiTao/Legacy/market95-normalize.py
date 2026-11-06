import json
from pathlib import Path
D=Path('Test/KetQua/journey_v48/market_ground_r95');p=D/'native-input.json';d=json.loads(p.read_text());seen=set();rows=[]
for r in d['objects']:
 if r['path'] in seen:continue
 seen.add(r['path']);rows.append(r)
d['objects']=rows;p.write_text(json.dumps(d,indent=2))
p=D/'grading.json';d=json.loads(p.read_text());seen=set();rows=[]
for r in d['objects']:
 if r['path'] in seen:continue
 seen.add(r['path']);rows.append(r)
d['objects']=rows;p.write_text(json.dumps(d,indent=2));print('NORMALIZED_NATIVE_CONTEXT',len(rows))
