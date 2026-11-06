import json
from pathlib import Path
import sys
sys.path.insert(0,'Tools/python-deps')
import numpy as np
D=Path('Test/KetQua/journey_v48/market_ground_r95');g=json.loads((D/'grading.json').read_text());r=next(r for r in g['objects'] if r['path'].endswith('/Surface_127'));a=np.load(D/(r['key']+'-graded.npz'));assert r['changed_vertices']==0
# Native source127 contains an older, longer bank footprint than actual r94.
# No grading is required here; do not re-introduce those historical faces.
g['objects']=[x for x in g['objects'] if x!=r];(D/'grading.json').write_text(json.dumps(g,indent=2));(D/'unchanged-exclusion.json').write_text(json.dumps(dict(path=r['path'],key=r['key'],reason='Zero height change; keep actual r94 bank footprint, not older native extent'),indent=2))
p=D/'integration-manifest.tsv';p.write_text('\n'.join(x for x in p.read_text().splitlines() if not x.startswith(r['key']+'\t')))
