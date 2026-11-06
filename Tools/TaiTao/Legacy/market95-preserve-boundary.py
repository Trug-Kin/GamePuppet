exec(open('Test/Tam/market95-grade.py').read().split('data=')[0])
sampler_code=Path('Test/Tam/i2-retopology.py').read_text();sampler_code=sampler_code[sampler_code.index('class Sampler:'):sampler_code.index('def floor_target')].replace('class Sampler:','class LowerSeamSampler:').replace('np.lexsort((-h,pi))','np.lexsort((h,pi))').replace('first=np.r_[True,np.diff(pi)!=0]','first=np.r_[True,np.diff(pi)!=0] if len(pi) else np.zeros(0,dtype=bool)');exec(sampler_code)
code=Path('Docs/Reviews/journey_v48/r74/mesh_io.py').read_text();exec(code.replace('O=Path(__file__).parent','O=D').replace('r73-source.meshbin','scene-ground.meshbin'));baseline={r['path']:r for r in read_source()}
water_guard=shapely.from_geojson((D/'protected-actual-water.geojson').read_text()).buffer(30,quad_segs=32)
edit=shapely.box(400,0,1000,545).difference(shapely.Point(714.94,459.48).buffer(18,quad_segs=128)).difference(shapely.box(400,390,704,600)).difference(shapely.box(650,448,756,600)).difference(water_guard);shapely.prepare(edit)
g=json.loads((D/'grading.json').read_text());report=[]
def parts_of(g):
 if g.is_empty:return []
 if g.geom_type=='Polygon':return [g]
 return [p for a in getattr(g,'geoms',[]) for p in parts_of(a)]
def line_parts(g):
 if g.is_empty:return []
 if g.geom_type=='LineString':return [g]
 return [p for a in getattr(g,'geoms',[]) for p in line_parts(a)]
def clip_vertical(q,b,u,inside):
 xz=q[:,[0,2]];edges=np.linalg.norm(xz[:,None,:]-xz[None,:,:],axis=2);i,j=np.unravel_index(edges.argmax(),edges.shape);base=xz[i];direction=xz[j]-base;length2=direction@direction
 if length2<1e-16:return []
 t=(xz-base)@direction/length2;line=shapely.LineString([base,base+direction]);piece=line.intersection(edit) if inside else line.difference(edit);out=[]
 for seg in line_parts(piece):
  ends=(np.asarray(seg.coords)[[0,-1]]-base)@direction/length2;lo,hi=sorted(ends);poly=[(q[k],b[k],u[k],t[k]) for k in range(3)]
  for value,greater in [(lo,True),(hi,False)]:
   clipped=[]
   for a,bb in zip(poly,poly[1:]+poly[:1]):
    ia=(a[3]>=value-1e-10) if greater else (a[3]<=value+1e-10);ib=(bb[3]>=value-1e-10) if greater else (bb[3]<=value+1e-10)
    if ia:clipped.append(a)
    if ia!=ib:
     w=(value-a[3])/(bb[3]-a[3]);clipped.append(tuple(a[k]+(bb[k]-a[k])*w for k in range(4)))
   poly=clipped
   if len(poly)<3:break
  for k in range(1,len(poly)-1):out.append(([poly[j][0] for j in [0,k,k+1]],[poly[j][1] for j in [0,k,k+1]],[poly[j][2] for j in [0,k,k+1]]))
 return out
def clipped_blocks(v,t,before,uv,slots,flags,inside):
 q=v[t];b=before[t];xz=q[:,:,[0,2]];n=np.cross(q[:,1]-q[:,0],q[:,2]-q[:,0]);horizontal=abs(n[:,1])>1e-8;poly=shapely.polygons(xz);covered=np.zeros(len(t),bool);intersects=np.zeros(len(t),bool);ix=np.flatnonzero(horizontal);covered[ix]=shapely.covers(edit,poly[ix]);intersects[ix]=shapely.intersects(edit,poly[ix])
 # Classify projected lines separately: polygon area is zero for a vertical face.
 for i in np.flatnonzero(~horizontal):
  line=shapely.LineString(xz[i]);covered[i]=shapely.covers(edit,line);intersects[i]=shapely.intersects(edit,line)
 keep=covered if inside else ~intersects;partial=intersects&~covered;vs=[q[keep].reshape(-1,3)];bs=[b[keep].reshape(-1,3)];us=[uv[keep].reshape(-1,2)];sslot=[slots[keep]];ff=[flags[keep]]
 for i in np.flatnonzero(partial):
  if not horizontal[i]:triples=clip_vertical(q[i],b[i],uv[i],inside)
  else:
   shape=poly[i].intersection(edit) if inside else poly[i].difference(edit);co=[]
   for part in parts_of(shape):
    if part.area<1e-12:continue
    co.extend(np.asarray(tri.exterior.coords)[:3] for tri in shapely.constrained_delaunay_triangles(part).geoms if tri.area>1e-12)
   triples=[]
   if co:
    pp=np.asarray(co).reshape(-1,2);aa,bb,cc=xz[i];den=np.cross(bb-aa,cc-aa);w0=np.cross(bb-pp,cc-pp)/den;w1=np.cross(cc-pp,aa-pp)/den;weights=np.c_[w0,w1,1-w0-w1];vv=weights@q[i];vv[:,0]=pp[:,0];vv[:,2]=pp[:,1];bbb=weights@b[i];uu=weights@uv[i];triples=list(zip(vv.reshape(-1,3,3),bbb.reshape(-1,3,3),uu.reshape(-1,3,2)))
  for vv,bb,uu in triples:
   vv,bb,uu=np.array(vv),np.array(bb),np.array(uu);nn=np.cross(vv[1]-vv[0],vv[2]-vv[0])
   if np.linalg.norm(nn)<1e-10:continue
   if nn@n[i]<0:vv=vv[::-1];bb=bb[::-1];uu=uu[::-1]
   vs.append(vv);bs.append(bb);us.append(uu);sslot.append(np.array([slots[i]]));ff.append(np.array([flags[i]]))
 return np.vstack(vs),np.vstack(bs),np.vstack(us),np.concatenate(sslot),np.concatenate(ff),int(partial.sum())
for row in g['objects']:
 if '/MarketIsland_R62/' in row['path'] or row['key'].startswith('M95_F'):continue
 old=baseline[row['path']];ov=world(old);ot=np.vstack(old['sub']);ou=old['uv'][ot] if len(old['uv'])==len(ov) else np.zeros((len(ot),3,2));os=np.repeat(np.arange(len(old['sub'])),[len(t) for t in old['sub']]);a=np.load(D/(row['key']+'-graded.npz'));af=a['is_floor'] if 'is_floor' in a else np.zeros(len(a['tri']),bool)
 new=clipped_blocks(a['v'],a['tri'],a['before'],a['uv'],a['slots'],af,True);saved=clipped_blocks(ov,ot,ov,ou,os,np.zeros(len(ot),bool),False)
 v=np.vstack([new[0],saved[0]]);b=np.vstack([new[1],saved[1]]);uv=np.vstack([new[2],saved[2]]).reshape(-1,3,2);slots=np.r_[new[3],saved[3]];flags=np.r_[new[4],saved[4]];t=np.arange(len(v)).reshape(-1,3)
 # A clipped native edge is not the approved r94 boundary. Reconcile floor
 # intersections to the actual current asset, avoiding inherited tall tips.
 newfloor=np.ones(len(new[0]),bool);ix=np.flatnonzero(newfloor&(shapely.distance(shapely.points(v[:len(newfloor),:][:,[0,2]]),edit.boundary)<.00011));boundary_adjustment=0.
 if len(ix):
  # The OUTER face identifies the actual seam sheet. Highest-surface sampling
  # over all baseline faces can pick a different inside sheet at an edge tip.
  sv=saved[0];st=np.arange(len(sv)).reshape(-1,3);sampler=LowerSeamSampler(sv,st,saved[2].reshape(-1,3,2));sampled=sampler.sample(v[ix][:,[0,2]])[0];h=sampled[:,1];ok=sampler.last_valid|(np.linalg.norm(sampled[:,[0,2]]-v[ix][:,[0,2]],axis=1)<.001);ix=ix[ok];h=h[ok]
  if len(ix):boundary_adjustment=float(np.max(abs(v[ix,1]-h)));v[ix,1]=h;b[ix,1]=h
 # Preserve exact baseline world points outside. A 1 micrometre vertex key is
 # only for identity/welding; coordinates themselves are never rounded here.
 order=np.argsort(-b[:,1],kind='stable');_,first,inv=np.unique(np.round(v[order],6),axis=0,return_index=True,return_inverse=True);remap=np.empty(len(v),int);remap[order]=inv;v=v[order[first]];b=b[order[first]];t=remap[t]
 np.savez_compressed(D/(row['key']+'-graded.npz'),v=v,tri=t,before=b,uv=uv,slots=slots,is_floor=flags)
 report.append(dict(path=row['path'],new_boundary_triangles=new[5],baseline_boundary_triangles=saved[5],preserved_outside_triangles=len(saved[3]),inside_triangles=len(new[3]),boundary_floor_vertices_pinned=len(ix),boundary_pin_max=boundary_adjustment,source_asset=old['asset']));print('PRESERVED_R94_BOUNDARY',row['path'],len(saved[3]),flush=True)
(D/'preserved-r94-boundary.json').write_text(json.dumps(dict(edit_polygon=shapely.to_geojson(edit),objects=report,reason='Keep current approved I2/banks/outside geometry; editable native remesh only inside authorized grading envelope'),indent=2))
