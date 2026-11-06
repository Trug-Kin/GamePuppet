import sys,json,struct
from pathlib import Path
sys.path.insert(0,'Tools/python-deps')
import numpy as np
from scipy.spatial import cKDTree
D=Path('Test/KetQua/journey_v48/market_ground_r95');inv={r['path']:r for r in json.loads((D/'props-geometry-inventory.json').read_text())+json.loads((D/'props-extra-geometry-inventory.json').read_text())};rows=json.loads((D/'placement-geometry-bindings.json').read_text());rows.append(json.loads((D/'clock-contact-final.json').read_text()));lines=[];report=[]
for row in rows:
 old=inv[row['path']];a=np.load(D/(row['key']+'-source.npz'));b=np.load(D/(old['cache']+'.npz'));before=a['before'];dist,ix=cKDTree(b['v']).query(before);assert dist.max()<.001,(row['path'],dist.max());n=len(before);ids=np.repeat(ix[:,None],3,axis=1);ww=np.zeros((n,3));ww[:,0]=1
 with open(D/(row['key']+'-mapping.bin'),'wb') as f:f.write(struct.pack('<i',n));f.write(ids.astype('<i4').tobytes());f.write(ww.astype('<f4').tobytes())
 with open(D/(row['key']+'-positions.bin'),'wb') as f:f.write(struct.pack('<i',n));f.write(a['v'].astype('<f4').tobytes())
 lines.append('\t'.join([row['key'],row['path'],old['asset']]));report.append(dict(key=row['key'],path=row['path'],vertices=n,max_source_distance=float(dist.max()),mapping='Unchanged original vertex identity, native clock nearest <=1mm'))
p=D/'integration-manifest.tsv';base=p.read_text().splitlines();keys={r.split('\t')[0] for r in lines};base=[r for r in base if r.split('\t')[0] not in keys];p.write_text('\n'.join(base+lines));(D/'unity-props-mapping.json').write_text(json.dumps(report,indent=2));print('PROPS_MAPPING',len(report),max(r['max_source_distance'] for r in report))
