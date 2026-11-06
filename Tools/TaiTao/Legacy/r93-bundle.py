import json,numpy as np
from pathlib import Path
from scipy.optimize import least_squares
from scipy.spatial.transform import Rotation
D=Path('Test/KetQua/journey_v48/r93');p37=json.load(open(D/'camera-fit.json'));a0=np.array(p37['a']);
Q1=np.array(p37['pixels']);Q2=np.array([[65,175],[1158,167],[-15,313],[1208,309],[-35,530],[1210,532],[1275,960]])
# Canonical front lines; retain model width, solve relative vertical profiles / overhang.
def pts(g):
 hb,hs,hr,dr,ds=g
 return np.array([[s,d,z if z is not None else -.60+.020*s] for d,z in [(dr,hr),(ds,hs),(8.5,hb),(8.4,None)] for s in [20.8,0]])
def caminit(c,target,f):
 c=np.array(c,dtype=float);fw=np.array(target)-c;fw/=np.linalg.norm(fw);ri=np.cross(fw,[0,0,1]);ri/=np.linalg.norm(ri);do=np.cross(fw,ri);return np.r_[c,Rotation.from_matrix([ri,do,fw]).as_rotvec(),np.log(f)]
def proj(P,c,center):
 v=(P-c[:3])@Rotation.from_rotvec(c[3:6]).as_matrix().T;return v[:,:2]/v[:,2:]*np.exp(c[6])+center
best=None
for f37 in [1191,1778,2430]:
 g=np.array([3.5,6.3,9.5,9.23,8.65]);c1=caminit([-12,38,1.1],[10,8.5,3],f37);c2=caminit([9,20,1.2],[10,8.5,4],559);x=np.r_[g,c1,c2]
 def resid(x):
  g=x[:5];P=pts(g);q1=proj(P,x[5:12],[115,534]);q2=proj(P[[0,1,2,3,4,5,7]],x[12:19],[750,500]);
  return np.r_[(q1-Q1).ravel()/15,(q2-Q2).ravel()/20,(x[7]-1.1)/.7,(x[14]-1.2)/.7,(x[11]-np.log(f37))/.4,(x[18]-np.log(559))/.3,max(0,g[0]+1.2-g[1])*10,max(0,g[1]+1.2-g[2])*10,(g[3]-9.23)/1.,(g[4]-8.65)/1.]
 lower=[2.6,4.6,6.8,8.1,8.1,-70,12,-.2,-6,-6,-6,np.log(700),-30,10,-.2,-6,-6,-6,np.log(350)]
 upper=[5.2,8.5,12.5,10.5,10.5,40,90,3.5,6,6,6,np.log(5000),40,70,3.5,6,6,6,np.log(1500)]
 r=least_squares(resid,x,bounds=(lower,upper),max_nfev=20000)
 print('prior',f37,'geometry',np.round(r.x[:5],3),'cam37',np.round(r.x[5:8],3),'camOF',np.round(r.x[12:15],3),'cost',round(np.linalg.norm(r.fun),3))
 if best is None or np.linalg.norm(r.fun)<np.linalg.norm(best.fun):best=r
x=best.x;P=pts(x[:5]);out={'status':'UNACCEPTED diagnostic bundle fit; approximate/cropped landmarks and focal assumptions','geometry':x[:5].tolist(),'P':P.tolist(),'q37':Q1.tolist(),'qofficial':Q2.tolist(),'camera37':x[5:12].tolist(),'cameraOfficial':x[12:19].tolist(),'project37':proj(P,x[5:12],[115,534]).tolist(),'projectOfficial':proj(P[[0,1,2,3,4,5,7]],x[12:19],[750,500]).tolist(),'norm':float(np.linalg.norm(best.fun))}
(D/'bundle-diagnostic.json').write_text(json.dumps(out,indent=2));
