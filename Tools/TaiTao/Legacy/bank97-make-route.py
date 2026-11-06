from pathlib import Path
import json,numpy as np
R=Path('D:/ConRoi');D=R/'Test/KetQua/journey_v48/bank_r97';co=np.array(json.loads((D/'author-result.json').read_text())['coast']);shore=lambda z:float(np.interp(z,co[:,0],co[:,1]));pts=[]
def add(z,d):pts.append((shore(z)+d,z))
pts.extend([(684,496),(690,489)])
for z in [480,476,471]:add(z,14.5)
for d in [12.5,10.5,8.5,6.55,4.8,3.2,.9]:add(471,d)
for z in [465,458,450]:add(z,.9)
for d in [2.2,4.3,6.55,8,10,12.5,14.5]:add(450,d)
for z in [444,437,429]:add(z,14.5)
for d in [12.5,10,8,6.55,4.3,2.2,.9]:add(429,d)
for d in [2.2,4.3,6.55,8,10,12.5,14.5]:add(429,d)
for z in [437,444,450]:add(z,14.5)
for d in [12.5,10,8,6.55,4.3,2.2,.9]:add(450,d)
for z in [458,465,471]:add(z,.9)
for d in [2.2,4.3,6.55,8,10,12.5,14.5]:add(471,d)
for z in [476,480]:add(z,14.5)
pts.extend([(690,489),(684,496),(678,502)])
s=(R/'Test/Tam/r95review-route-fixed.cs.txt').read_text(encoding='utf-8-sig')
s=s.replace('r95_visual_review','bank_r97').replace('Market95Scoped','Bank97Scoped').replace('route-fixed','route').replace('new Vector3(650,15,528)','new Vector3(678,15,502)').replace('new Vector3(650,hit.point.y','new Vector3(678,hit.point.y').replace('*.25f,528)','*.25f,502)')
start=s.index('Vector2[] path=');end=s.index(';bool gameplayLight',start);s=s[:start]+'Vector2[] path={'+','.join(f'new Vector2({x:.5f}f,{z:.5f}f)' for x,z in pts)+'}'+s[end:]
# Tight stair/landing steering uses closer waypoints, preserving the controller.
s=s.replace('delta.magnitude<1.1f','delta.magnitude<.40f').replace('leg==25','leg=='+str(len(pts)-3)).replace('t<520','t<400').replace('max520s','max400s')
(R/'Test/Tam/bank97-route.cs.txt').write_text(s,encoding='utf-8');(D/'route-waypoints.json').write_text(json.dumps(dict(start_fixture=[678,502],waypoints=pts,note='One initial bridge placement, then synthetic keyboard/mouse through unchanged controller; all three stairs both directions, no per-leg placement'),indent=2))
f=(R/'Test/Tam/r95review-fixture.cs.txt').read_text(encoding='utf-8-sig').replace('r95_visual_review','bank_r97');(R/'Test/Tam/bank97-fixture.cs.txt').write_text(f,encoding='utf-8')
rec=(R/'Test/Tam/r95review-record-fixed.py').read_text().replace('r95_visual_review','bank_r97').replace('r95review-route-fixed.cs.txt','bank97-route.cs.txt').replace('route-fixed','route').replace('bridge-market-return-fixed-controller','bridgehead-bank-controller').replace("'525'","'405'")
(R/'Test/Tam/bank97-record.py').write_text(rec)
print('ROUTE',len(pts))
