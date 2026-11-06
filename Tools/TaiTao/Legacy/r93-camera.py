import json,numpy as np
from scipy.optimize import least_squares
from scipy.spatial.transform import Rotation
from pathlib import Path
# Manually marked P37 crop (2x source pixels); points are architecture, never tree/wire.
P=[];Q=[];names=[]
for label,y,z,left,right in [('roof',9.23,7.08,(35,205),(929,18)),('sign-bottom',8.65,5.46,(35,362),(911,252)),('balcony-base',8.5,3.05,(35,476),(913,437)),('ground',8.4,None,(35,649),(910,694))]:
 for s,q in [(20.8,left),(0,right)]:
  P.append([s,y,z if z is not None else (-.60+.020*s)]);Q.append(q);names.append(label+('-north' if s else '-south'))
P=np.array(P);Q=np.array(Q)
# Camera computer-vision coordinates x right, y down, z forward.
def proj(a):
 c=a[:3];rot=Rotation.from_rotvec(a[3:6]).as_matrix();v=(P-c)@rot.T;f=np.exp(a[6]);return v[:,:2]/v[:,2:]*f+np.array([115,534]) # principal point of ORIGINAL viewport in cropped 2x coords
best=None
for c in [[-5,40,2],[0,40,2],[-15,45,3]]:
 target=np.array([10,8.4,3]);forward=(target-c);forward/=np.linalg.norm(forward);right=np.cross(forward,[0,0,1]);right/=np.linalg.norm(right);down=np.cross(forward,right);rot=np.array([right,down,forward]);a=np.r_[c,Rotation.from_matrix(rot).as_rotvec(),np.log(2000)]
 fit=least_squares(lambda a:(proj(a)-Q).ravel(),a,max_nfev=10000)
 if best is None or np.linalg.norm(fit.fun)<np.linalg.norm(best.fun):best=fit
r={'a':best.x.tolist(),'rotation_cv':Rotation.from_rotvec(best.x[3:6]).as_matrix().tolist(),'focal_pixels_crop':float(np.exp(best.x[6])),'principal_crop':[115,534],'points':P.tolist(),'pixels':Q.tolist(),'labels':names,'projected':proj(best.x).tolist(),'rms_crop_px':float(np.sqrt(np.mean(best.fun**2))),'note':'Approximate manual architecture marks P37 crop; not surveyed; left end partially obscured, no exact fSpy calibration claim.'}
Path('Test/KetQua/journey_v48/r93/camera-fit.json').write_text(json.dumps(r,indent=2));print(r['a'],r['rms_crop_px'])
