import sys,json,importlib.util
from pathlib import Path
sys.path.insert(0,'Tools/python-deps')
import numpy as np
D=Path('Test/KetQua/journey_v48/ground_r93');p=Path('Docs/Reviews/journey_v48/r88/ground-data.py');spec=importlib.util.spec_from_file_location('r88ground',p);g=importlib.util.module_from_spec(spec);spec.loader.exec_module(g);f=np.load(p.parent/'bridge-field.npz');out=[]
def field(x,z):
 u=(x-f['xs'][0])*2;v=(z-f['zs'][0])*2;i=int(u);j=int(v);a=u-i;b=v-j
 return float(f['delta'][j,i]*(1-a)*(1-b)+f['delta'][j,i+1]*a*(1-b)+f['delta'][j+1,i]*(1-a)*b+f['delta'][j+1,i+1]*a*b)
for x,z in [(714.94,473.48),(714.94,473.98),(714.94,474.48),(714.94,475.48)]:
 hit=g.hits(g.row('Surface_128'),x,z);h=hit[0][0] if hit else None;delta=field(x,z);out.append(dict(xz=[x,z],r87_road_y=h,r88_field_delta=delta,estimated_vertex_field_sum=None if h is None else h+delta,note='r88 subdivided triangles then displaced vertices; point sum is a lineage approximation, not exact tessellated reconstruction'))
(D/'i2-r88-field-lineage.json').write_text(json.dumps(out,indent=2));print(json.dumps(out,indent=2))
