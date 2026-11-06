exec(open('Test/Tam/market95-grade.py').read().split('data=')[0])
# Final footprint audit found service and duplicate-material footway layers not
# represented in the original ground selection. Author those actual r94 assets.
from scipy.spatial import cKDTree
source=Path('Docs/Reviews/journey_v48/r74/mesh_io.py').read_text();exec(source.replace('O=Path(__file__).parent','O=D').replace('r73-source.meshbin','props-baseline.meshbin'));rows=read_source();exec(source.replace('O=Path(__file__).parent','O=D').replace('r73-source.meshbin','props-extra-baseline.meshbin'));rows+=read_source();by={r['path']:r for r in rows}
f=np.load(D/'continuous-design-field.npz');field=RegularGridInterpolator((f['x'],f['z']),f['y'],bounds_error=False,fill_value=None);edit=shapely.from_geojson(json.loads((D/'preserved-r94-boundary.json').read_text())['edit_polygon']);wg=shapely.from_geojson((D/'protected-actual-water.geojson').read_text()).buffer(30)
g=json.loads((D/'grading.json').read_text());log=[]
for k,path in enumerate(['R20_Whole_Source_Export/Surface_130','LakeGateMarketV32/Context_112','R20_Whole_Source_Export/Surface_126','LakeGateMarketV32/Context_107','LakeGateMarketV32/Context_109']):
 r=by[path];ov=world(r);ot=np.vstack(r['sub']);uv=r['uv'][ot] if len(r['uv'])==len(ov) else np.zeros((len(ot),3,2));slots=np.repeat(np.arange(len(r['sub'])),[len(t) for t in r['sub']]);triangles=[];uvs=[];sslot=[]
 def split(q,u,slot,depth=0):
  xz=q[:,[0,2]];poly=shapely.Polygon(xz)
  # Do not subdivide unchanged protected ground, nor structural steps.
  lengths=np.linalg.norm(np.roll(xz,-1,axis=0)-xz,axis=1)
  if depth<8 and lengths.max()>2 and poly.area>1e-9 and edit.intersects(poly):
   i=int(lengths.argmax());j=(i+1)%3;l=(i+2)%3;m=(q[i]+q[j])/2;mu=(u[i]+u[j])/2;split(np.array([q[i],m,q[l]]),np.array([u[i],mu,u[l]]),slot,depth+1);split(np.array([m,q[j],q[l]]),np.array([mu,u[j],u[l]]),slot,depth+1)
  else:triangles.append(q[::-1]);uvs.append(u[::-1]);sslot.append(slot)
 for t,u,slot in zip(ot,uv,slots):split(ov[t],u,slot)
 before=np.asarray(triangles).reshape(-1,3);uv=np.asarray(uvs);slots=np.array(sslot);v=before.copy();xz=v[:,[0,2]];inside=shapely.contains(edit,shapely.points(xz));dist=shapely.distance(shapely.points(xz),edit.boundary);blend=ss(dist/10)*np.maximum(ss(shapely.distance(shapely.points(xz),wg)/80),ss((xz[:,0]-650)/54));v[inside,1]+=blend[inside]*(field(xz[inside])-v[inside,1]);_,first,inv=np.unique(np.round(v,6),axis=0,return_index=True,return_inverse=True);t=inv.reshape(-1,3);v=v[first];before=before[first];q=v[t];valid=np.linalg.norm(np.cross(q[:,1]-q[:,0],q[:,2]-q[:,0]),axis=1)>1e-9;t=t[valid];uv=uv[valid];slots=slots[valid];key='M95_Service'+str(k);np.savez_compressed(D/(key+'-graded.npz'),v=v,before=before,tri=t,uv=uv,slots=slots,is_floor=np.ones(len(t),bool));row=dict(key=key,path=path,object=key+'_NATIVE',source=r['asset'],mode='Actual r94 footprint, shared final grade; no new overlay');g['objects']=[x for x in g['objects'] if x['path']!=path]+[row];log.append(row)
(D/'grading.json').write_text(json.dumps(g,indent=2));(D/'service-layer-closure.json').write_text(json.dumps(log,indent=2))
inv=json.loads((D/'geometry-inventory.json').read_text());extra=json.loads((D/'props-geometry-inventory.json').read_text())+json.loads((D/'props-extra-geometry-inventory.json').read_text())
for r in log:
 if not any(x['path']==r['path'] for x in inv):inv.append(next(x for x in extra if x['path']==r['path']))
(D/'geometry-inventory.json').write_text(json.dumps(inv,indent=2));print('SERVICE_LAYER_CLOSURE',len(log))
