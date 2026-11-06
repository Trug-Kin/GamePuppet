import json,sys
from pathlib import Path
sys.path.insert(0,'Tools/python-deps')
import numpy as np
D=Path('Test/KetQua/journey_v48/market_ground_r95')
inv=json.loads((D/'inventory.json').read_text())['rows']
rows=[r for r in inv if r['active'] and r['render'] and r['max'][0]>700 and r['min'][0]<875 and r['max'][2]>145 and r['min'][2]<440]
groups={}
for r in rows:
 k=r['path'].split('/')[0];groups.setdefault(k,[]).append(r)
for k,rs in groups.items():
 print(k,len(rs),'Y',round(min(r['min'][1] for r in rs),3),round(max(r['max'][1] for r in rs),3),'XZ',[(round(min(r['min'][j] for r in rs),2),round(max(r['max'][j] for r in rs),2)) for j in [0,2]])
print('R20 structures:')
for r in rows:
 if r['path'].startswith('R20') and r['min'][1]>6.5 and '/Langfarm' not in r['path']: print(r['path'],r['min'],r['max'])
print('Surface104 source')
s=json.loads(Path('Assets/Art/Environment/JourneyV48/ExportPolicyR20/surfaces.json').read_text())['meshes']
print(next(r for r in s if r['mesh']=='Surface_104'))
