import sys,json
from pathlib import Path
sys.path.insert(0,'Tools/python-deps')
import numpy as np
D=Path('Test/KetQua/journey_v48/ground_r93');a=json.loads((D/'geometry-inventory.json').read_text());inv={r['path']:r for r in a};trial=json.loads((D/'transition-trial.json').read_text());out=[];g=np.load(D/'layer-grid.npz');xx,zz=np.meshgrid(g['xs'],g['zs']);rr=np.hypot(xx-714.94,zz-459.48);region=(rr>13.2)&(rr<24)
for item in trial['objects']:
 path=item['path'];orig=inv.get(path,inv.get(path+'#COLLIDER'));before=np.load(D/(orig['cache']+'.npz'));after=np.load(D/('trial-'+item['object']+'.npz'));row=dict(path=path)
 for tag,data in [('before',before),('trial',after)]:
  q=data['v'][data['tri']];cen=q.mean(1);r=np.hypot(cen[:,0]-714.94,cen[:,2]-459.48);nn=np.cross(q[:,1]-q[:,0],q[:,2]-q[:,0]);projected=abs(nn[:,1])/2;sel=(r>13.2)&(r<24)&(projected>1e-8);grade=np.hypot(nn[:,0],nn[:,2])/np.maximum(abs(nn[:,1]),1e-12)*100;row[tag]=dict(triangles=int(sel.sum()),max_face_grade=float(grade[sel].max(initial=0)),area_grade_over20=float(projected[sel&(grade>20)].sum()),area_grade_over10=float(projected[sel&(grade>10)].sum()),projected_area=float(projected[sel].sum()))
 out.append(row)
(D/'trial-face-grades.json').write_text(json.dumps(out,indent=2));print(json.dumps(out,indent=2)[:14000]);print('UNMATCHED ROAD WITHIN PATCH')
for path in ['R20_Whole_Source_Export/Surface_127','R20_Whole_Source_Export/Surface_174','R20_Whole_Source_Export/Surface_175']:
 h=g[inv[path]['cache']];print(path,int((region&np.isfinite(h)).sum()))
