exec(open('Test/Tam/market95-grade.py').read().split('data=')[0])
g=json.loads((D/'grading.json').read_text());by={r['path'].split('/')[-1]:r for r in g['objects']};out=[]
pairs=[('Surface_104','Context_108'),('Surface_125','Context_106'),('Surface_134','Context_116'),('Surface_153','Context_136'),('Surface_128','Context_110'),('Surface_129','Context_111'),('Surface_173','Restored_028'),('Surface_176','Context_124')]
x,z=np.meshgrid(np.arange(465,920,1.5),np.arange(111,542,1.5));pp=np.c_[x.ravel(),z.ravel()];core=(pp[:,0]>=704)&(pp[:,0]<=860)&(pp[:,1]>=165)&(pp[:,1]<=435)
print('LAYER_COMPARISON_START',len(pp),'grid spacing 1.5 authored m; chunked queries',flush=True)
for a,b in pairs:
 sam=[]
 for name in [a,b]:
  r=by[name];d=np.load(D/(r['key']+'-graded.npz'));s=Sampler(d['v'],d['tri'],d['uv']);heights=[];valids=[]
  for start in range(0,len(pp),5000):
   heights.append(s.sample(pp[start:start+5000])[0][:,1]);valids.append(s.last_valid.copy())
  v=np.concatenate(heights);ok=np.concatenate(valids);sam.append((v,ok));print('SAMPLED',name,flush=True)
 valid=sam[0][1]&sam[1][1];diff=abs(sam[0][0]-sam[1][0]);sel=np.flatnonzero(valid);worst=sel[np.argsort(diff[sel])[-8:]]
 out.append(dict(render=a,collider=b,spacing=1.5,shared_samples=int(valid.sum()),core_shared_samples=int((valid&core).sum()),core_max=float(diff[valid&core].max()) if (valid&core).any() else None,whole_max=float(diff[valid].max()) if valid.any() else None,worst=[dict(xz=pp[i].tolist(),render=float(sam[0][0][i]),collider=float(sam[1][0][i]),difference=float(diff[i])) for i in worst]));(D/'render-collider-layer-comparison.json').write_text(json.dumps(out,indent=2));print('PAIR',a,b,out[-1]['core_max'],out[-1]['whole_max'],flush=True)
(D/'render-collider-layer-comparison.json').write_text(json.dumps(out,indent=2));print([(r['render'],r['collider'],r['core_max'],r['whole_max']) for r in out],flush=True)
