exec(open('Test/Tam/market95-grade.py').read().split('data=')[0])
f=np.load(D/'continuous-design-field.npz');fi=RegularGridInterpolator((f['x'],f['z']),f['y']);g=json.loads((D/'grading.json').read_text())
for name,pt in [('Surface_176',[545.9899,304.19357]),('Surface_134',[763.12996,427.44387])]:
 r=next(r for r in g['objects'] if r['path'].endswith('/'+name));a=np.load(D/(r['key']+'-graded.npz'));v=a['v'];q=v[a['tri']];ix=np.argmin(np.linalg.norm(q.mean(1)[:,[0,2]]-pt,axis=1));tri=q[ix];print(name,'q',tri.tolist(),'old',a['before'][a['tri'][ix]].tolist(),'field',fi(tri[:,[0,2]]).tolist(),'floor',bool(a['is_floor'][ix]),'edges',np.linalg.norm(np.roll(tri,-1,axis=0)-tri,axis=1).tolist())
