exec(open('Test/Tam/i2-gate.py').read().split('rows=json.loads')[0])
G=Path('Test/KetQua/journey_v48/ground_r93');inv=json.loads((G/'geometry-inventory.json').read_text());rows=json.loads((D/'retopology.json').read_text());report=[]
for row in inv:
 if not row.get('cache') or not row['active'] or not row['render']:continue
 if not (row['path'].endswith('OD35_Surface_2') or 'Approach_Road_NW' in row['path'] or 'Side_Link_122' in row['path'] or 'Approach_Sidewalk' in row['path']):continue
 a=np.load(G/(row['cache']+'.npz'));v=a['v'].astype(float);tri=a['tri'];q=v[tri];cen=q.mean(1);sel=np.linalg.norm(cen[:,[0,2]]-[cx,cz],axis=1)<45
 for nr in rows:
  if nr['path'].startswith('LakeGateMarket'):continue
  b=np.load(D/(nr['key']+'-source.npz'));bs=Surf(b['v'].astype(float),b['tri']);os=Surf(b['before'].astype(float),b['tri']);pol=shapely.union_all(shapely.polygons(q[sel][:,:,[0,2]]));bb=pol.boundary
  if bb is None:continue
  pp=shapely.get_coordinates(shapely.line_interpolate_point(bb,np.arange(0,bb.length,.1)));ha,_=bs.query(pp);hb,_=os.query(pp);ok=np.isfinite(ha)&np.isfinite(hb)&(np.linalg.norm(pp-[cx,cz],axis=1)<40)
  if not ok.any():continue
  ids=np.flatnonzero(ok);ix=ids[np.argsort(abs(ha-hb)[ids])[-6:]];report.append(dict(bridge=row['path'],layer=nr['path'],samples=int(ok.sum()),maxchange=float(np.nanmax(abs(ha[ok]-hb[ok]))),worst=[dict(xz=pp[k].tolist(),old=float(hb[k]),new=float(ha[k])) for k in ix]))
(D/'bridge-boundary-check.json').write_text(json.dumps(report,indent=2));print(json.dumps(report,indent=2))
