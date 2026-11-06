import json
from pathlib import Path
for p in Path('Test/KetQua/journey_v48/dam_r98').glob('*-gate.json'):
 d=json.loads(p.read_text());print(p.stem,d.get('pass'),str(d.get('errors'))[:700],str(d.get('warnings'))[:300])
