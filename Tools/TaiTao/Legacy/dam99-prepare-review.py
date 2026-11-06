from pathlib import Path
import json,sys,math
sys.path.insert(0,'Tools/python-deps')
import numpy as np
R=Path('D:/ConRoi');D=R/'Test/KetQua/journey_v48/dam_r99'
A=np.array([637.7344,528.2603]);B=np.array([681.4578,491.2729]);L=float(np.linalg.norm(B-A));T=(B-A)/L;N=np.array([T[1],-T[0]])
water=np.array(json.loads((R/'Test/KetQua/journey_v48/market_ground_r95/protected-actual-water.geojson').read_text())['coordinates'][0])
def shore(s):
 p=A+T*s;hits=[]
 for a,b in zip(water,water[1:]):
  try:d,u=np.linalg.solve(np.column_stack([N,a-b]),a-p)
  except np.linalg.LinAlgError:continue
  if d>=0 and 0<=u<=1:hits.append(d)
 return min(hits)
def smooth(x):x=max(0,min(1,x));return x*x*(3-2*x)
def warp(p):
 s,d=(np.array(p)-A)@np.c_[T,N];w=smooth((s+18)/15)*smooth((L+18-s)/15);e=shore(s) if w else 1;u=max(0,min(1,d/e)) if d<=e else max(0,1-(d-e)/20);return A+T*s+N*(d+(4.5-e)*w*u)
def p(s,d):return A+T*s+N*d
path=[p(L-4,-1.2),p(L+1.5,-1.2)]+[p(L+1.5,d) for d in [.2,1,2,3,4.05]]+[p(L+4,4.05)]
coast=np.array(json.loads((R/'Test/KetQua/journey_v48/bank_r97/author-result.json').read_text())['coast'])
path +=[warp((float(np.interp(z,coast[:,0],coast[:,1]))+1,z)) for z in [480.,477.,474.,471.,474.,477.,480.]]
path +=[p(L+4,4.05),p(L+1.5,4.05)]+[p(L+1.5,d) for d in [3,2,1,.2,-1.2]]
threshold=len(path)
path +=[p(s,-1.2) for s in [L-3,45,35,25,15,5,-1.5]]
path +=[p(-1.5,d) for d in [.2,1,2,3,4.05]]+[p(-4,4.05),p(-1.5,4.05)]
path +=[p(-1.5,d) for d in [3,2,1,.2,-1.2]]
path +=[p(s,-1.2) for s in [5,15,25,35,45,L-4]]+[(678,502)]
s=(R/'Test/Tam/dam98-route.cs.txt').read_text(encoding='utf-8-sig');a=s.index('Vector2[] path=');b=s.index(';bool gameplayLight=',a)
s=s[:a]+'Vector2[] path={'+','.join(f'new Vector2({x:.6f}f,{z:.6f}f)' for x,z in path)+'}'+s[b:]
s=s.replace('dam_r98','dam_r99').replace('Dam98Scoped','Dam99Scoped').replace('leg==32',f'leg=={threshold}').replace('t<240','t<280').replace('max240s','max280s')
(R/'Test/Tam/dam99-route.cs.txt').write_text(s,encoding='utf-8')
for name in ['fixture','pause','region-return','play','stop']:
 s=(R/f'Test/Tam/dam98-{name}.cs.txt').read_text(encoding='utf-8-sig').replace('dam_r98','dam_r99').replace('r98.unity','r99.unity').replace('R98_OngDao_Concrete_Dam','R99_OngDao_Dam_Section').replace('DamR98/','DamR99/')
 (R/f'Test/Tam/dam99-{name}.cs.txt').write_text(s,encoding='utf-8')
s=(R/'Test/Tam/dam98-record.py').read_text().replace('dam_r98','dam_r99').replace('dam98-route','dam99-route').replace("'245'","'285'")
(R/'Test/Tam/dam99-record.py').write_text(s,encoding='utf-8')
s=(R/'Test/Tam/dam98-views.cs.txt').read_text().replace('dam_r98','dam_r99').replace('Dam98Comparison','Dam99Comparison')
eyes=[(628,8,491),(663,2.2,489),tuple([*p(25,-2)[:1],0])]
def cs(v):return 'new Vector3('+','.join(f'{n:.6f}f' for n in v)+')'
def xyz(s,d,h):x,z=p(s,d);return (x,h,z)
es=[(628,8,491),(663,2.2,489),xyz(25,-2,4.65),xyz(-6,7,1.85),xyz(L+5,7,1.85),xyz(L+2,-1,4.65),xyz(-2,-1,4.65)]
ats=[(657,1,515),(649,1,520),xyz(25,4,.1),xyz(-1.5,2,1.6),xyz(L+1.5,2,1.6),xyz(L+1.5,3,.5),xyz(-1.5,3,.5)]
a=s.index('Vector3[] eyes=');b=s.index('string rev=',a)
s=s[:a]+'Vector3[] eyes={'+','.join(map(cs,es))+'};Vector3[] ats={'+','.join(map(cs,ats))+'};string[] tags={"across-lake","apron-oblique","from-deck","far-from-low","gate-from-low","gate-from-top","far-from-top"};'+s[b:]
(R/'Test/Tam/dam99-views.cs.txt').write_text(s,encoding='utf-8')
(D/'route-plan.json').write_text(json.dumps(dict(path=[list(q) for q in path],lighting_gameplay_from=threshold,run=4.5,risers=14,input='Synthetic InputSystem; unchanged controller; one starting fixture',old_r98_route_not_reused=True),indent=2))
print('Prepared',len(path),'route points, gameplay at',threshold)
