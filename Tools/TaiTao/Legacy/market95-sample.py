exec(open('Test/Tam/market95-grade.py').read().split('data=')[0])
rows=json.loads((D/'native-input.json').read_text())['objects'];r=next(r for r in rows if r['path'].endswith('Surface_176'));a=np.load(D/(r['key']+'-native.npz'));s=Sampler(a['v'],a['tri'],a['uv'])
for z in [191,250,280,300,370,430,450,480,510]:
 p=np.c_[np.arange(650,861,10),np.full(22,z)];h=s.sample(p)[0][:,1];print(z,[(int(x),round(y,2),round(t,2),round(w,2)) for x,y,t,w in zip(p[:,0],h,target(p),weight(p))],flush=True)
