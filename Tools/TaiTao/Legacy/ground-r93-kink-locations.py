import sys,json
from pathlib import Path
sys.path.insert(0,'Tools/python-deps')
import numpy as np
D=Path('Test/KetQua/journey_v48/ground_r93');ss=json.loads((D/'sections.json').read_text());ls=json.loads((D/'layer-sections.json').read_text());out=[]
for r in ls:
 if r['path']!='R20_Whole_Source_Export/Surface_128':continue
 s=ss[r['section']];h=np.array([np.nan if v is None else v for v in r['top']]);dist=np.array(s['distance']);gr=np.diff(h)/np.diff(dist)*100
 for i in np.flatnonzero(abs(gr)>20):out.append(dict(section=s['region']+'/'+s['name'],i=int(i),xz0=s['xz'][i],xz1=s['xz'][i+1],y0=float(h[i]),y1=float(h[i+1]),length=float(dist[i+1]-dist[i]),grade=float(gr[i])))
out.sort(key=lambda r:abs(r['grade']),reverse=True);(D/'road128-high-grade-locations.json').write_text(json.dumps(out,indent=2));print(json.dumps(out[:15],indent=2))
