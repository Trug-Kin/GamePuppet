exec(open('Test/Tam/market95-grade.py').read().split('data=')[0])
baseline={r['path']:r for r in json.loads((D/'geometry-inventory.json').read_text())};g=json.loads((D/'grading.json').read_text());log=[]
apply='--apply' in sys.argv
for r in g['objects']:
 a=np.load(D/(r['key']+'-graded.npz'))
 if 'is_floor' not in a or not a['is_floor'].any():continue
 v=a['v'].copy();b=a['before'].copy();t=a['tri'];w=weight(v[:,[0,2]]);sel=(w>1e-8)&(w<.999999)
 if not sel.any():continue
 old=np.load(D/(baseline[r['path']]['cache']+'.npz'));s=Sampler(old['v'],old['tri'],np.zeros((len(old['tri']),3,2)));h=s.sample(v[sel][:,[0,2]])[0][:,1];ok=s.last_valid;ix=np.flatnonzero(sel)[ok];h=h[ok]
 # Reconcile to the current bound asset, not a shared terrain sampler which
 # selected unrelated/below-water sheets on transition roads. One blend only.
 v[ix,1]=h+w[ix]*(target(v[ix][:,[0,2]])-h);b[ix,1]=h
 q=v[t];n=np.cross(q[:,1]-q[:,0],q[:,2]-q[:,0]);area=abs(n[:,1])/2;grade=np.linalg.norm(n[:,[0,2]],axis=1)/np.maximum(abs(n[:,1]),1e-15)*100;valid=a['is_floor']&(area>.01)&(weight(q.mean(1)[:,[0,2]])>1e-6)
 worst=np.flatnonzero(valid);worst=worst[np.argsort(grade[worst])[-8:]]
 log.append(dict(path=r['path'],transition_reconciled_vertices=len(ix),worst=[dict(xyz=q[i].mean(0).tolist(),grade=float(grade[i]),area=float(area[i])) for i in worst]))
 if apply:
  data={k:a[k] for k in a.files};data.update(v=v,before=b);np.savez_compressed(D/(r['key']+'-graded.npz'),**data)
(D/'transition-reconciliation.json').write_text(json.dumps(dict(applied=apply,rows=log),indent=2));print(json.dumps(log,indent=2),flush=True)
