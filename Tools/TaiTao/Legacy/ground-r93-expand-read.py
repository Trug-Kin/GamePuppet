exec(open('Test/Tam/ground-r93-survey.py').read().split('stats=compact()')[0].replace("scene-ground.meshbin'","expanded-ground.meshbin'").replace("rows=read_source();exec", "rows=read_source();unused=read_source;exec").replace("rows+=read_source()","discard=read_source()").replace("'inventory.json'","'expanded-inventory.json'"))
stats=json.loads((D/'geometry-inventory.json').read_text())
new=[]
for r in rows:
 meta=inv[r['path']];v=world(r);t=np.vstack(r['sub']);key='E'+str(len(new)).zfill(3)
 rr=dict(path=r['path'],asset=r['asset'],role=role(r),active=meta['active'],render=meta['render'],collider=meta['collider'],vertices=len(v),triangles=len(t),bounds=[v.min(0).tolist(),v.max(0).tolist()],cache=key)
 new.append(rr);np.savez_compressed(D/(key+'.npz'),v=v,tri=t,uv=r['uv'])
(D/'expanded-geometry.json').write_text(json.dumps(new,indent=2))
print(json.dumps(new,indent=2))
