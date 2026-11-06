import sys,json
from pathlib import Path
sys.path.insert(0,'Tools/python-deps')
import numpy as np
from scipy.spatial import cKDTree
code=Path('Test/Tam/market95-grade.py').read_text();exec(code[:code.index('data=')])
code=Path('Test/Tam/i2-mapping.py').read_text();exec(code[code.index('def nearest3d'):code.index("D=Path('Test/")])
row=next(r for r in rows if r['path'].endswith('/Context_132'));a=np.load(D/(row['key']+'-native.npz'));inv=json.loads((D/'geometry-inventory.json').read_text());old=next(r for r in inv if r['path']==row['path']);u=np.load(D/(old['cache']+'.npz'));used=np.unique(a['tri']);remap=np.full(len(a['v']),-1,int);remap[used]=np.arange(len(used));v=a['v'][used];tri=remap[a['tri']];ids,w,e=nearest3d(v,u['v'][u['tri']],k=96)
bad=np.flatnonzero(e>.005)
if len(bad):
 ii,ww,ee=nearest3d(v[bad],u['v'][u['tri']],k=1024);ids[bad]=ii;w[bad]=ww;e[bad]=ee
before=(u['v'][u['tri'][ids]]*w[:,:,None]).sum(1)
print('CURB_RECONCILIATION_DIAGNOSTIC',float(e.max()),v[np.argmax(e)].tolist(),np.percentile(e,[50,90,99,100]).tolist(),flush=True)
assert e.max()<.25,'Native reconciliation outside known historical grade delta'
foot=next(r for r in rows if r['path'].endswith('/Surface_153'));f=np.load(D/(foot['key']+'-native.npz'));sam=Sampler(f['v'],f['tri'],f['uv']);fp,_,_,_=sam.sample(before[:,[0,2]]);fg=np.load(D/(foot['key']+'-graded.npz'));newsam=Sampler(fg['v'],fg['tri'],fg['uv']);newfoot=newsam.sample(fp[:,[0,2]])[0];new=before.copy();new[:,1]+=newfoot[:,1]-fp[:,1]
np.savez_compressed(D/(row['key']+'-graded.npz'),v=new,tri=tri,before=before,uv=a['uv'],slots=a['slots']);g=json.loads((D/'grading.json').read_text());g['objects'].append({**row,'mode':'native curb topology reconciled to current baseline then coherent full-height grade warp','source_reconciliation_max':float(e.max()),'source_reconciliation_xz_max':float(np.linalg.norm((before-v)[:,[0,2]],axis=1).max()),'vertices':len(new),'triangles':len(tri),'delta_range':[float((new[:,1]-before[:,1]).min()),float((new[:,1]-before[:,1]).max())]});(D/'grading.json').write_text(json.dumps(g,indent=2));print('CURB_NATIVE_RECONCILIATION',e.max(), 'xz',np.linalg.norm((before-v)[:,[0,2]],axis=1).max())
