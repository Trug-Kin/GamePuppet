import sys,json
from pathlib import Path
sys.path.insert(0,'Tools/python-deps')
import numpy as np
R=Path('D:/ConRoi');D=R/'Test/KetQua/journey_v48/market_ground_r95'
code=(R/'Docs/Reviews/journey_v48/r74/mesh_io.py').read_text();exec(code.replace('O=Path(__file__).parent','O=D').replace('r73-source.meshbin','foundation-baseline.meshbin'))
out=[]
for i,r in enumerate(read_source()):
 key='foundation-unity-'+str(i).zfill(3);v=world(r);t=np.vstack(r['sub']);np.savez_compressed(D/(key+'.npz'),v=v,tri=t);out.append(dict(path=r['path'],asset=r['asset'],cache=key))
(D/'foundation-geometry-inventory.json').write_text(json.dumps(out,indent=2));print('BASELINE_CACHE',len(out))
