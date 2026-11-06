from pathlib import Path
import json,sys
sys.path.insert(0,'Tools/python-deps')
import numpy as np
R=Path('D:/ConRoi');D=R/'Test/KetQua/journey_v48/dam_r98'
A=np.array([637.7344,528.2603]);B=np.array([681.4578,491.2729]);L=np.linalg.norm(B-A);T=(B-A)/L;N=np.array([T[1],-T[0]]);water=np.array(json.loads((R/'Test/KetQua/journey_v48/market_ground_r95/protected-actual-water.geojson').read_text())['coordinates'][0])
def shore(s):
 p=A+T*s;hits=[]
 for a,b in zip(water,water[1:]):
  try:d,u=np.linalg.solve(np.column_stack([N,a-b]),a-p)
  except np.linalg.LinAlgError:continue
  if d>=0 and 0<=u<=1:hits.append(d)
 return min(hits)
def p(s,d):return tuple(A+T*s+N*d)
g=L+1.5;far=-1.5;end=shore(g)
path=[p(L-4,-1.2),p(L+2.5,-1.2),p(L+2.5,.5),p(L+2.5,2.2),p(L+2.5,3.8),p(g,4.3)]
path+=[p(g,d) for d in np.arange(5,end-1.0,1.3)]+[p(g,end-.8),p(L+3.5,shore(L+3.5)-.8)]
coast=np.array(json.loads((R/'Test/KetQua/journey_v48/bank_r97/author-result.json').read_text())['coast'])
path +=[(float(np.interp(z,coast[:,0],coast[:,1]))+1,z) for z in [480.,477.,474.,471.,474.,477.,480.]]
path += [p(L+3.5,shore(L+3.5)-.8),p(g,end-.8)]
path +=[p(g,d) for d in np.arange(end-1.7,4.1,-1.3)]+[p(g,4.3),p(L+2.5,3.8),p(L+2.5,2.2),p(L+2.5,.5),p(L+2.5,-1.2)]
threshold=len(path)
path +=[p(s,-1.2) for s in [L-3,45,35,25,15,5,far]]
path +=[p(far,d) for d in [0.5,2,4,6,8,shore(far)-.8]]
path +=[p(far,d) for d in [8,6,4,2,.5,-1.2]]
path +=[p(s,-1.2) for s in [5,15,25,35,45,L-4]]
path +=[(678,502)]
s=(R/'Test/Tam/bank97-route.cs.txt').read_text(encoding='utf-8-sig');a=s.index('Vector2[] path=');b=s.index(';bool gameplayLight=',a)
s=s[:a]+'Vector2[] path={'+','.join(f'new Vector2({x:.6f}f,{z:.6f}f)' for x,z in path)+'}'+s[b:]
s=s.replace('bank_r97','dam_r98').replace('Bank97Scoped','Dam98Scoped').replace('leg==32',f'leg=={threshold}').replace('t<400','t<240').replace('max400s','max240s')
(R/'Test/Tam/dam98-route.cs.txt').write_text(s)
for name in ['fixture','pause','load']:
 s=(R/f'Test/Tam/bank97-{name}.cs.txt').read_text(encoding='utf-8-sig').replace('bank_r97','dam_r98').replace('R97_Bridgehead_GateSide_Bank','R98_OngDao_Concrete_Dam')
 if name=='load':
  x,z=p(g,shore(g)-.8);s=s.replace('666.79944f,1.62f,450',f'{x:.6f}f,1.62f,{z:.6f}f').replace('low path Z450','new gate lower landing')
 (R/f'Test/Tam/dam98-{name}.cs.txt').write_text(s)
s=(R/'Test/Tam/bank97-record.py').read_text().replace('bank_r97','dam_r98').replace('bank97-route','dam98-route').replace('bridgehead-bank-controller','dam-end-stairs-controller').replace("'405'","'245'")
(R/'Test/Tam/dam98-record.py').write_text(s)
(D/'route-plan.json').write_text(json.dumps(dict(path=path,light_gameplay_from_waypoint=threshold,input='Synthetic keyboard/mouse through unchanged controller',placement='One start fixture only; no placement between end stairs and R97 lower path'),indent=2))
print(len(path),'waypoints; gameplay lighting at',threshold,'Gate low',p(g,end-.8),'shore width',end)
