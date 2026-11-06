exec(open('Test/Tam/market95-grade.py').read().split('data=')[0])
from scipy.sparse import coo_matrix,diags
from scipy.sparse.linalg import cg
from scipy.ndimage import distance_transform_edt
g=json.loads((D/'grading.json').read_text());bi={r['path']:r for r in json.loads((D/'geometry-inventory.json').read_text())};terrain=next(r for r in g['objects'] if r['path'].endswith('/Context_124'));old=np.load(D/(bi[terrain['path']]['cache']+'.npz'));sam=Sampler(old['v'],old['tri'],np.zeros((len(old['tri']),3,2)))
# Single minimum-curvature-in-height (harmonic) transition, rather than blending
# every legacy hump with a flat target. The maximum principle introduces no
# new isolated summit/pit. All render/collider floors use this same field.
xs=np.arange(450.,1000.1,2);zs=np.arange(0.,546.1,2);xx,zz=np.meshgrid(xs,zs,indexing='ij');pp=np.c_[xx.ravel(),zz.ravel()];hh=[];valid=[]
for start in range(0,len(pp),4000):
 h=sam.sample(pp[start:start+4000])[0][:,1];hh.extend(h);valid.extend(sam.last_valid)
raw=np.array(hh).reshape(xx.shape);valid=np.array(valid).reshape(xx.shape);missing=~valid;near=distance_transform_edt(missing,return_distances=False,return_indices=True);raw[missing]=raw[tuple(near[:,missing])]
raw=gaussian_filter(raw,4.,mode='nearest')
water_guard=shapely.from_geojson((D/'protected-actual-water.geojson').read_text()).buffer(30,quad_segs=32)
edit=shapely.box(400,0,1000,545).difference(shapely.Point(714.94,459.48).buffer(18,quad_segs=128)).difference(shapely.box(400,390,704,600)).difference(shapely.box(650,448,756,600)).difference(water_guard)
inside=shapely.contains(edit,shapely.points(pp)).reshape(xx.shape);corepin=inside&(xx>=704)&(xx<=870)&(zz>=0)&(zz<=437)
fixed=~inside|corepin;fixed[[0,-1],:]=True;fixed[:,[0,-1]]=True
# A harmonic C0 join concentrated the derivative on the core boundary.
# Use a C1/C2 authored full-width transition, preserving I2 and actual banks.
w=np.ones(xx.shape) # No unverified legacy hill is a constraint inside the market game floor.
desired=target(pp).reshape(xx.shape)
edge=raw[:,np.argmin(abs(zs-448))][:,None]
fraction=np.clip(np.maximum(raw,0)/np.maximum(edge,.1),0,1)
bank=raw+(.72-edge)*fraction
desired=np.where(zz>448,bank,desired)
values=raw+(desired-raw)*w
# Conform to the actual PROTECTED EDGE height, not remote legacy hills.
# Reusing old height at each distant point retained a new hump around the
# rectangle. This correction is zero at its outer feather and matches the
# measured boundary, over the full width, with smooth end derivatives.
protected=shapely.box(400,390,704,600).union(shapely.box(650,448,756,600)).union(shapely.Point(714.94,459.48).buffer(18,quad_segs=128)).union(water_guard)
nearest=shapely.get_coordinates(shapely.get_point(shapely.shortest_line(shapely.points(pp),protected.boundary),-1))
edgeh=sam.sample(nearest)[0][:,1]
basefield=RegularGridInterpolator((xs,zs),values,bounds_error=False,fill_value=None)
d=shapely.distance(shapely.points(pp),protected)
correction=(edgeh-basefield(nearest))*(1-ss(d/80))
values+=gaussian_filter(correction.reshape(xx.shape),4.,mode='nearest')
solution=np.array([]);info=0;free=np.zeros(xx.shape,bool);field=RegularGridInterpolator((xs,zs),values,bounds_error=False,fill_value=None)
np.savez_compressed(D/'continuous-design-field.npz',x=xs,z=zs,y=values,old=raw,core=corepin,fixed=fixed)
log=[]
design_core=shapely.box(704,0,870,435)
def final_height(xz):
 return field(xz)
for r in g['objects']:
 if r['key'].startswith('M95_F') or '/MarketIsland_R62/' in r['path']:continue
 a=np.load(D/(r['key']+'-graded.npz'));v=a['v'].copy();before=a['before'].copy();xz=v[:,[0,2]];pin=shapely.contains(edit,shapely.points(xz));h=final_height(xz);distance=shapely.distance(shapely.points(xz),edit.boundary);blend=ss(distance/10)*np.maximum(ss(shapely.distance(shapely.points(xz),water_guard)/80),ss((xz[:,0]-650)/54))
 # Untouched vertices exactly retain r94; native intentional vertical curbs
 # are handled separately from floors, with their thickness retained.
 if r['path'].endswith('/Context_132'):continue
 floor=a['is_floor'].copy() if 'is_floor' in a else np.zeros(len(a['tri']),bool)
 if False: # Only the actual remeshed floor owns grading; retained banks/skirts do not.
  bq=before[a['tri']];bn=np.cross(bq[:,1]-bq[:,0],bq[:,2]-bq[:,0]);cent=bq.mean(1);newfloor=(abs(bn[:,1])>1e-6)&shapely.contains(edit,shapely.points(cent[:,[0,2]]));floor|=newfloor
 touch=np.zeros(len(v),bool);touch[np.unique(a['tri'][floor])]=True;sel=pin&touch
 baseline=np.load(D/(bi[r['path']]['cache']+'.npz'));bs=Sampler(baseline['v'],baseline['tri'],np.zeros((len(baseline['tri']),3,2)));ix=np.flatnonzero(touch)
 for start in range(0,len(ix),4000):
  ids=ix[start:start+4000];oldh=bs.sample(xz[ids])[0][:,1];ok=bs.last_valid;before[ids[ok],1]=oldh[ok]
 # Idempotence: discard every earlier trial height, including vertices on a
 # now-protected boundary. Otherwise clipping interpolates stale trial tips.
 v[:,1]=before[:,1]
 v[sel,1]=before[sel,1]+blend[sel]*(h[sel]-before[sel,1])
 # Retain the actual sloping bank section: scale its positive heights to
 # the new upper contact, with the same water endpoint. Never flatten it.
 bankids=np.flatnonzero(pin&(xz[:,1]>448))
 if len(bankids):
  ep=np.c_[xz[bankids,0],np.full(len(bankids),448.)];oldedge=sam.sample(ep)[0][:,1];newedge=final_height(ep);ratio=np.clip(newedge/np.maximum(oldedge,.1),0,2);bh=before[bankids,1];desired_bank=np.where(bh>0,bh*ratio,bh);v[bankids,1]=bh+blend[bankids]*(desired_bank-bh)
 # Collapse old internal construction skirts only on the game floor.
 if False: # Retained skirts are not an extra floor sheet to collapse.
  skirt=pin&(blend>.999999)&(xz[:,1]<435);v[skirt,1]=final_height(xz[skirt])
 d={k:a[k] for k in a.files};d['v']=v;d['before']=before;d['is_floor']=floor;np.savez_compressed(D/(r['key']+'-graded.npz'),**d);log.append(dict(path=r['path'],vertices_relevelled=int(sel.sum())))
g['transition_design']='Shared analytic flat core with C1 external approach, protected-edge height correction and broad 80m lake-bank conformance; not field reconstruction';g['continuous_field']='continuous-design-field.npz';(D/'grading.json').write_text(json.dumps(g,indent=2));(D/'continuous-field-solve.json').write_text(json.dumps(dict(grid_spacing=2.,unknowns=int(free.sum()),solver_info=int(info),residual=0.,edit=shapely.to_geojson(edit),objects=log),indent=2));print('CONTINUOUS_FIELD',free.sum(),info,flush=True)
