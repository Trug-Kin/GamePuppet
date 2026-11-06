import sys,json
from pathlib import Path
sys.path.insert(0,'Tools/python-deps')
import numpy as np
D=Path('Test/KetQua/journey_v48/market_ground_r95');a=np.load(D/'structural-Core_Road_steps_W99661181.npz');v=a['v'];q=v[a['tri']];n=np.cross(q[:,1]-q[:,0],q[:,2]-q[:,0]);g=np.linalg.norm(n[:,[0,2]],axis=1)/np.maximum(abs(n[:,1]),1e-15)*100
print('vertices by X',v[np.argsort(v[:,0])][::3].round(4).tolist());print('grades',np.quantile(g,[0,.5,.9,.99,1]));
j=json.loads((D/'full-scope-floor-gate.json').read_text())
for r in j['over_six_percent']:
 print(r['path'],r['worst'][-1])
