exec(open('Test/Tam/market95-grade.py').read().split('data=')[0])
g=json.loads((D/'grading.json').read_text())
for row in g['objects']:
 if row['path'].startswith('LakeGate') or '/MarketIsland' in row['path']:continue
 a=np.load(D/(row['key']+'-graded.npz'));q=a['v'][a['tri']];b=a['before'][a['tri']];c=q.mean(1);n=np.cross(q[:,1]-q[:,0],q[:,2]-q[:,0]);nn=np.cross(b[:,1]-b[:,0],b[:,2]-b[:,0]);gr=np.linalg.norm(n[:,[0,2]],axis=1)/np.maximum(abs(n[:,1]),1e-15)*100;old=np.linalg.norm(nn[:,[0,2]],axis=1)/np.maximum(abs(nn[:,1]),1e-15)*100;w=weight(c[:,[0,2]]);valid=(w>.99)&(abs(n[:,1])>1e-7)&a['is_floor'];ix=np.flatnonzero(valid)
 if len(ix):
  i=ix[np.argmax(gr[ix])];print(row['path'].split('/')[-1],c[i],gr[i],old[i],w[i],q[i].tolist(),flush=True)
