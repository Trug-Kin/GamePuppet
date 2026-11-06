import sys,json
sys.path.insert(0,'Tools/python-deps')
import numpy as np
from shapely.geometry import LineString,Point
A=np.array([637.7344,528.2603]);B=np.array([681.4578,491.2729]);t=(B-A)/np.linalg.norm(B-A);n=np.array([t[1],-t[0]])
w=np.array(json.load(open('Test/KetQua/journey_v48/market_ground_r95/protected-actual-water.geojson'))['coordinates'][0])
print('basis',A,B,t,n,'L',np.linalg.norm(B-A))
for s in [0,3,10,25,40,54,57.27]:
 p=A+t*s;line=LineString([p,p+n*40]);hit=line.intersection(LineString(w));print(s,hit.wkt,hit.distance(Point(p)))
for name in ['C95_0041_OD35_Surface_1','C95_0043_OD35_Surface_3','C95_0045_OD35_Surface_5']:
 a=np.load('Test/KetQua/journey_v48/dam_r98/'+name+'.npz');v=a['v'];q=(v[:,[0,2]]-A);ss=q@t;dd=q@n;mask=(ss>=0)&(ss<=58)&(abs(dd)<2)
 print(name,'sdheight',np.round(np.quantile(np.c_[ss[mask],dd[mask],v[mask,1]],[0,.25,.5,.75,1],axis=0),3))
