exec(open('Test/Tam/market95-grade.py').read().split('data=')[0])
g=json.loads((D/'grading.json').read_text());out=[]
for r in g['objects']:
 a=np.load(D/(r['key']+'-graded.npz'))
 if 'is_floor' not in a or not a['is_floor'].any():continue
 q=a['v'][a['tri']];old=a['before'][a['tri']];c=q.mean(1);n=np.cross(q[:,1]-q[:,0],q[:,2]-q[:,0]);no=np.cross(old[:,1]-old[:,0],old[:,2]-old[:,0]);area=abs(n[:,1])/2
 grade=np.linalg.norm(n[:,[0,2]],axis=1)/np.maximum(abs(n[:,1]),1e-15)*100;before=np.linalg.norm(no[:,[0,2]],axis=1)/np.maximum(abs(no[:,1]),1e-15)*100
 changed=np.max(abs(q[:,:,1]-old[:,:,1]),axis=1)>.005;valid=a['is_floor']&changed&(area>.01)
 bad=valid&(grade>8)&(grade>before+2)
 ix=np.flatnonzero(bad);ix=ix[np.argsort(grade[ix])[-20:]]
 out.append(dict(path=r['path'],changed_floor_area=float(area[valid].sum()),new_over8_area=float(area[bad].sum()),new_over8_faces=int(bad.sum()),worst=[dict(xyz=c[i].tolist(),before=float(before[i]),after=float(grade[i]),area=float(area[i]),weight=float(weight(c[i:i+1][:,[0,2]])[0])) for i in ix]))
(D/'whole-envelope-transition-audit.json').write_text(json.dumps(out,indent=2))
print(json.dumps([r for r in out if r['new_over8_faces']],indent=2),flush=True)
