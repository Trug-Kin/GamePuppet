exec(open('Test/Tam/market95-grade.py').read().split('data=')[0])
g=json.loads((D/'grading.json').read_text());out=[]
for row in g['objects']:
 a=np.load(D/(row['key']+'-graded.npz'))
 if 'is_floor' not in a:continue
 q=a['v'][a['tri']];c=q.mean(1);n=np.cross(q[:,1]-q[:,0],q[:,2]-q[:,0]);area=abs(n[:,1])/2;grade=np.linalg.norm(n[:,[0,2]],axis=1)/np.maximum(abs(n[:,1]),1e-15)*100
 for name,mask in [('flat_market_core',(q[:,:,0]>=704).all(1)&(q[:,:,0]<=860).all(1)&(q[:,:,2]<=320).all(1)),('external_approach',(c[:,0]>=704)&(c[:,0]<=860)&(c[:,2]>=320)&(c[:,2]<=435)),('north_full_width',(q[:,:,0]>=704).all(1)&(q[:,:,0]<=860).all(1)&(q[:,:,2]>=58).all(1)&(q[:,:,2]<=165).all(1))]:
  mask&=a['is_floor']&(area>.001);ix=np.flatnonzero(mask);worst=ix[np.argmax(grade[ix])] if len(ix) else None
  out.append(dict(path=row['path'],region=name,area=float(area[mask].sum()),max_grade=float(grade[mask].max()) if len(ix) else None,worst=c[worst].tolist() if worst is not None else None,heights=[float(q[mask,:,1].min()),float(q[mask,:,1].max())] if len(ix) else None))
(D/'final-regions.json').write_text(json.dumps(out,indent=2));print([(r['path'].split('/')[-1],r['region'],round(r['max_grade'],4)) for r in out if r['max_grade'] is not None and r['region']=='flat_market_core'])
