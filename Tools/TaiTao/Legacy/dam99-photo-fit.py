from pathlib import Path
import sys,json,hashlib
sys.path.insert(0,'Tools/python-deps')
import numpy as np
from scipy.optimize import least_squares
R=Path('D:/ConRoi');D=R/'Test/KetQua/journey_v48/dam_r99';M=R/'Test/BangChung/journey_v48/dam_r99'
D.mkdir(parents=True,exist_ok=True);M.mkdir(parents=True,exist_ok=True)
files=set()
for baseline in ['dam_r98/protected-start.json','market_ground_r95/protected-start.json']:
 data=json.loads((R/'Test/KetQua/journey_v48'/baseline).read_text(encoding='utf-8-sig'))
 if isinstance(data,dict):data=[dict(path=k,sha256=v) for k,v in data.items()]
 for row in data:files.add(Path(row['path']))
for folder in ['Source/Blender/JourneyV48/R98','Assets/Art/Environment/JourneyV48/DamR98']:
 files.update(p for p in (R/folder).rglob('*') if p.is_file())
for x in ['Assets/Scenes/JourneyPresentation_v48_r98.unity','Assets/Scenes/JourneyPresentation_v48_r98.unity.meta']:files.add(R/x)
rows=[]
for p in files:
 p=p if p.is_absolute() else R/p
 if p.is_file():rows.append(dict(path=str(p),sha256=hashlib.file_digest(p.open('rb'),'sha256').hexdigest()))
(D/'protected-start.json').write_text(json.dumps(rows,indent=2))
# Approximate manually identified silhouette landmarks; rail height defines relative scale,
# not a measured metre. Two independent image positions, same original video.
photos=[dict(id='181347',points=[[1060,480],[1665,477],[1045,554],[1516,588],[1060,452],[1665,448]]),dict(id='181402',points=[[141,518],[721,539],[78,611],[552,657],[141,491],[721,510]])]
L=57.2695684

def project(v,params):
 ex,ez,ey,yaw,pitch,roll,f=params
 eye=np.array([ex,ey,ez]);fw=np.array([np.cos(pitch)*np.cos(yaw),np.sin(pitch),np.cos(pitch)*np.sin(yaw)])
 right=np.cross(fw,[0,1,0]);right/=np.linalg.norm(right);up=np.cross(right,fw)
 rr=right*np.cos(roll)+up*np.sin(roll);uu=-right*np.sin(roll)+up*np.cos(roll)
 a=v-eye;dep=a@fw;return np.c_[960+f*(a@rr)/dep,540-f*(a@uu)/dep],dep

def residual(p,H=None,W=None):
 h,w=p[:2] if H is None else (H,W);q=p[2:] if H is None else p
 v=np.array([[0,h,0],[L,h,0],[0,.08,w],[L,.08,w],[0,h+1,0],[L,h+1,0]])
 out=[]
 for i,ph in enumerate(photos):
  a,depth=project(v,q[i*7:i*7+7]);out.extend((a-np.array(ph['points'])).flatten());out.extend(np.minimum(depth,0)*100)
 return out
best=None
rng=np.random.default_rng(98)
lo=[.4,1.0];hi=[7,18]
for i in range(2):lo += [-150,5,.3,-3.14,-.7,-.12,700];hi += [200,220,20,3.14,.3,.12,2200]
for seed in range(8):
 init=[3.,7.]
 for i in range(2):
  ex=100 if i==0 else 130;ez=70 if i==0 else 60
  init += [ex,ez,3.5,np.arctan2(-ez,28-ex),-.025,0,1200]
 init=np.array(init)+rng.normal(0,.03,len(init))
 fit=least_squares(residual,init,bounds=(lo,hi),max_nfev=2200)
 if best is None or np.linalg.norm(fit.fun)<np.linalg.norm(best.fun):best=fit
p=best.x;out=dict(photos=photos,L_assumed=L,rail_unit=1,solution=p.tolist(),rms_px=float(np.sqrt(np.mean(np.array(residual(p))**2))),warning='MANUAL approximate landmarks, assumed level equal-height crest and rail 1 relative unit; span is game footprint, not measured. Degenerate/uncertain focal-depth must be reported, no calibrated survey.')
# Fit fixed R98 proportions with precisely same landmarks/model; camera can adjust.
f=least_squares(lambda q:residual(q,1.140076,10.2),p[2:],bounds=(lo[2:],hi[2:]),max_nfev=2500)
out['r98_camera_fit']=dict(params=f.x.tolist(),rms_px=float(np.sqrt(np.mean(np.array(f.fun)**2))))
(D/'photo-fit-initial.json').write_text(json.dumps(out,indent=2));print(json.dumps(out,indent=2))


