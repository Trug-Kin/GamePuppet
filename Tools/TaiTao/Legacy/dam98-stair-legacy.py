import sys
sys.path.insert(0,'Tools/python-deps')
import numpy as np
A=np.array([637.7344,528.2603]);B=np.array([681.4578,491.2729]);L=np.linalg.norm(B-A);T=(B-A)/L;N=np.array([T[1],-T[0]])
for name in ['C95_0041_OD35_Surface_1','C95_0043_OD35_Surface_3']:
 q=np.load('Test/KetQua/journey_v48/dam_r98/'+name+'.npz');v=q['v'];tri=q['tri'];sd=(v[:,[0,2]]-A)@np.c_[T,N]
 for lo,hi in [(-3,0),(L,L+3)]:
  mask=(sd[:,0]>=lo)&(sd[:,0]<=hi)&(sd[:,1]>=0)&(sd[:,1]<13)
  print(name,lo,'bounds',np.round(np.c_[sd,v[:,1]][mask].min(0),3) if mask.any() else None,np.round(np.c_[sd,v[:,1]][mask].max(0),3) if mask.any() else None)
