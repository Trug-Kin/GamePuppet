exec(open('Test/Tam/market95-grade.py').read().split('data=')[0])
r=next(r for r in rows if r['path'].endswith('/Island_Paved_Base'));a=np.load(D/(r['key']+'-native.npz'));v=a['v'];q=v[a['tri']];n=np.cross(q[:,1]-q[:,0],q[:,2]-q[:,0]);top=n[:,1]>.001
for mask in [abs(n[:,1])>.001,top,n[:,1]<-.001]:
 p=q[mask].reshape(-1,3);mat=np.c_[np.ones(len(p)),p[:,0]-783.948,p[:,2]-191.167];coef=np.linalg.lstsq(mat,p[:,1],rcond=None)[0];res=p[:,1]-mat@coef;print('FIT',len(p),coef,np.quantile(res,[0,.01,.5,.99,1]))
print('v_y',np.quantile(v[:,1],[0,.01,.1,.5,.9,.99,1]))
for row in json.loads((D/'grading.json').read_text())['objects']:
 if '/MarketIsland_R62/' not in row['path']:continue
 a=np.load(D/(row['key']+'-graded.npz'));q=a['v'][a['tri']];n=np.cross(q[:,1]-q[:,0],q[:,2]-q[:,0]);gr=np.linalg.norm(n[:,[0,2]],axis=1)/np.maximum(abs(n[:,1]),1e-10);ix=np.flatnonzero((abs(n[:,1])>.001)&(gr>2));
 if len(ix):print(row['path'].split('/')[-1],len(ix),'worst',q[ix[np.argmax(gr[ix])]].tolist())
