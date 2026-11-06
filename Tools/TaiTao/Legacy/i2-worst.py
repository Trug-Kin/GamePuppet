exec(open('Test/Tam/i2-gate.py').read().split('rows=json.loads')[0])
for name in ['Surface_128','Surface_153','Surface_142']:
 a=np.load(D/('I2_'+name+'-source.npz'));v=a['v'];q=v[a['tri']];cen=q.mean(1);n=np.cross(q[:,1]-q[:,0],q[:,2]-q[:,0]);gr=np.linalg.norm(n[:,[0,2]],axis=1)/np.maximum(abs(n[:,1]),1e-30)*100;pa=abs(n[:,1])/2;sel=(np.linalg.norm(cen[:,[0,2]]-[cx,cz],axis=1)<40)&(cen[:,2]>cz-8)&(pa>.01);ix=np.flatnonzero(sel);ix=ix[np.argsort(gr[ix])[-2:]]
 print(name)
 for i in ix:print('grade',gr[i],'q',q[i].tolist(),'before',a['before'][a['tri'][i]].tolist())
