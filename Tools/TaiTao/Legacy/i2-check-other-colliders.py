exec(open('Test/Tam/i2-gate.py').read().split('rows=json.loads')[0])
inv=json.loads((D/'native-input.json').read_text())['objects'];rows=json.loads((D/'retopology.json').read_text());xx,zz=np.meshgrid(np.arange(cx-50,cx+51,.2),np.arange(cz-7,cz+49,.2));p=np.c_[xx.ravel(),zz.ravel()];floors={};out=[]
for rn in ['Surface_128','Surface_129','Surface_153']:
 a=np.load(D/('I2_'+rn+'-source.npz'));floors[rn]=Surf(a['v'],a['tri']).query(p)[0]
for row in inv:
 if not row['path'].startswith('LakeGateMarket') or any(r['path']==row['path'] for r in rows):continue
 a=np.load(D/(row['key']+'.npz'));h,_=Surf(a['v'],a['tri']).query(p)
 for rn,f in floors.items():
  ok=np.isfinite(f)&np.isfinite(h)&(h>f+.005);ii=np.flatnonzero(ok);out.append(dict(collider=row['path'],floor=rn,above_points=len(ii),worst=[dict(xz=p[k].tolist(),floor=float(f[k]),old_collider=float(h[k])) for k in ii[np.argsort((h-f)[ii])[-5:]]]))
(D/'other-colliders.json').write_text(json.dumps(out,indent=2));print(json.dumps(out,indent=2))
