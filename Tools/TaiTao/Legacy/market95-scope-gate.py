exec(open('Test/Tam/market95-grade.py').read().split('data=')[0])
g=json.loads((D/'grading.json').read_text());rows=[];bad=[]
for row in g['objects']:
 if row['path'].startswith('LakeGate') or '/MarketIsland' in row['path'] or row['key'].startswith('M95_F'):continue
 a=np.load(D/(row['key']+'-graded.npz'));v=a['v'];t=a['tri'];q=v[t];c=q.mean(1);n=np.cross(q[:,1]-q[:,0],q[:,2]-q[:,0]);gr=np.linalg.norm(n[:,[0,2]],axis=1)/np.maximum(abs(n[:,1]),1e-15)*100
 # Full width of the game entrance, both branches, ring and foreground plaza.
 inside=(c[:,0]>=704)&(c[:,0]<=860)&(c[:,2]>=165)&(c[:,2]<=435)&a['is_floor']&(abs(n[:,1])>1e-7)
 ix=np.flatnonzero(inside);worst=ix[np.argsort(gr[ix])[-10:]]
 r=dict(path=row['path'],faces=int(inside.sum()),max_grade=float(gr[inside].max()) if inside.any() else None,worst=[dict(position=c[i].tolist(),grade=float(gr[i]),weight=float(weight(c[i:i+1][:,[0,2]])[0])) for i in worst]);rows.append(r)
 if r['max_grade'] and r['max_grade']>6:bad.append(r)
(D/'full-scope-floor-gate.json').write_text(json.dumps(dict(accepted=False,pending='Source reopen, topology, layer/seam checks and visual gate still required',region='Unity X704..860 Z165..435; complete surface triangles, not controller tracks',rows=rows,over_six_percent=bad),indent=2));print([(r['path'].split('/')[-1],r['max_grade']) for r in rows],flush=True)
