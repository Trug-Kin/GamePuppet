from pathlib import Path
import numpy as np,json,sys
sys.path.insert(0,'Tools/python-deps')
D=Path('Test/KetQua/journey_v48/bank_r97');co=np.array(json.loads((D/'author-result.json').read_text())['coast'])
for f in D.glob('*-existing.npz'):
 d=np.load(f);v=d['v'];delta=v[:,0]-np.interp(v[:,2],co[:,0],co[:,1]);sel=(v[:,2]>418)&(v[:,2]<483)&(delta<-.24)&(delta>-8)&(v[:,1]>.02)
 if sel.any():print(f.name,int(sel.sum()),v[sel].min(0),v[sel].max(0))
