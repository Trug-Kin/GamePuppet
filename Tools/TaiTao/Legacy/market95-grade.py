import sys,json,math
from pathlib import Path
sys.path.insert(0,'Tools/python-deps')
import numpy as np,shapely
from scipy.spatial import Delaunay
import triangle as trmesh
from scipy.interpolate import PchipInterpolator
from scipy.interpolate import RegularGridInterpolator
from scipy.ndimage import gaussian_filter
# Existing barycentric native sampler; no Unity runner or product component.
code=Path('Test/Tam/i2-retopology.py').read_text();exec(code[code.index('class Sampler:'):code.index('def floor_target')].replace('first=np.r_[True,np.diff(pi)!=0]','first=np.r_[True,np.diff(pi)!=0] if len(pi) else np.zeros(0,dtype=bool)'))
from shapely import STRtree
D=Path('Test/KetQua/journey_v48/market_ground_r95');rows=json.loads((D/'native-input.json').read_text())['objects'];report=[]
def ss(t):t=np.clip(t,0,1);return t*t*t*(10+t*(-15+6*t))
# Final authorized game floor: flat core through the north map edge.
# Only Z320..435 is the external I2 approach. C1 easing over 10m at
# either end, constant grade between; 4.32m rise across 115m.
def target(p):
 z=p[:,1];t=np.clip(z-320,0,115);L=115.;e=10.;slope=4.32/(L-e)
 drop=np.where(t<e,slope*t*t/(2*e),np.where(t>L-e,4.32-slope*(L-t)**2/(2*e),slope*(t-e/2)))
 return 5.04-drop
# Full continuous site polygon, not a ribbon around controller tracks.
core=shapely.Polygon([(450,0),(1000,0),(1000,580),(450,580)])
def weight(p):
 pts=shapely.points(p);inside=shapely.contains(core,pts);dist=shapely.distance(pts,core.boundary);w=ss(dist/18)*inside*ss((p[:,0]-450)/254)
 # I2 r94 is a hard checkpoint: new grading fades in beyond its north-side boundary.
 r=np.linalg.norm(p-[714.94,459.48],axis=1);w*=ss((r-18)/4)
 # The dam/lower lakeside walkway west of the bridge approach is structural,
 # not part of the market yard. Preserve its original section and water edge.
 shore=ss((p[:,1]-300)/90)*(1-ss((p[:,0]-704)/14));w*=1-shore
 # Blend into the higher market/stair precinct; do not leave a cut at Z=153.
 w*=1. # no artificial north rim: the source map ends near Z58.76
 # A 15m south boundary generated a new steep wall on the outer branch.
 # Keep the I2 radial guard and use a broad transition over the whole apron.
 w*=1-ss((p[:,1]-405)/40)
 return w
def pieces(g):
 if g.is_empty:return []
 if g.geom_type=='Polygon':return [g]
 return [p for a in getattr(g,'geoms',[]) for p in pieces(a)]

def designed_heights(sam,p,old):
 w=weight(p);base=old.copy();ix=np.flatnonzero((w>1e-6)&(w<.999999))
 if len(ix):
  lo=np.floor(p[ix].min(0))-4;hi=np.ceil(p[ix].max(0))+4
  xs=np.arange(lo[0],hi[0]+1);zs=np.arange(lo[1],hi[1]+1);xx,zz=np.meshgrid(xs,zs,indexing='ij');grid=np.c_[xx.ravel(),zz.ravel()]
  reference_sampler=globals().get('BANK_SAMPLER',sam)
  hh=reference_sampler.sample(grid)[0][:,1].reshape(xx.shape)
  # Remeshing must not interpolate overlapping original slivers into new
  # millimetre-wide spikes. A shared 1m transition field smooths their height
  # signal; full-weight game floor remains the analytic design exactly.
  smooth=gaussian_filter(hh,1.25,mode='nearest');ref=RegularGridInterpolator((xs,zs),smooth)(p[ix]);s=ss(w[ix]/.1);base[ix]+=(ref-base[ix])*s
 result=base+w*(target(p)-base)
 # South of the approach edge is a descending bank, not a new flat yard.
 # Preserve its section toward water: move its top with the adjoining road,
 # taper that displacement with original height, and pin water/below-water.
 ix=np.flatnonzero(p[:,1]>448)
 if len(ix) and 'BANK_SAMPLER' in globals():
  # One shared smooth bank reference for render/collider/road. A raw nearest
  # query over intersecting legacy triangles produced discontinuous edge tops.
  px=np.arange(np.floor(p[ix,0].min())-4,np.ceil(p[ix,0].max())+5)
  edge=np.c_[px,np.full(len(px),452.)];raw=BANK_SAMPLER.sample(edge)[0][:,1]
  from scipy.ndimage import gaussian_filter1d
  top=np.interp(p[ix,0],px,gaussian_filter1d(raw,1.25,mode='nearest'))
  edgepoint=np.c_[p[ix,0],np.full(len(ix),452.)];edge_new=top+weight(edgepoint)*(target(edgepoint)-top)
  # Smooth in XZ while preserving the sloping bank profile and the water pin.
  bx=np.arange(np.floor(p[ix,0].min())-4,np.ceil(p[ix,0].max())+5);bz=np.arange(np.floor(p[ix,1].min())-4,np.ceil(p[ix,1].max())+5)
  xx,zz=np.meshgrid(bx,bz,indexing='ij');hh=BANK_SAMPLER.sample(np.c_[xx.ravel(),zz.ravel()])[0][:,1].reshape(xx.shape);ref=RegularGridInterpolator((bx,bz),gaussian_filter(hh,1.25,mode='nearest'))(p[ix])
  fraction=np.clip(ref/np.maximum(top,.1),0,1);bank=ref+(edge_new-top)*fraction;blend=ss((p[ix,1]-448)/4)*weight(p[ix]);result[ix]+=(bank-result[ix])*blend
 return result
data={r['path'].split('/')[-1]:np.load(D/(r['key']+'-native.npz')) for r in rows if '/MarketIsland_R62/' not in r['path']}
BANK_SAMPLER=Sampler(data['Surface_176']['v'],data['Surface_176']['tri'],data['Surface_176']['uv'])
ground=['Surface_125','Surface_142','Surface_134','Surface_176','Surface_104','Restored_028','Surface_128','Surface_153','Surface_129','Restored_029','Surface_173','RoadLoop_125314010_R65','Surface_127','Surface_174','Surface_175','Context_111','Context_136','Context_110','Context_108','Context_124','Context_106','Context_116','Context_133','Context_130']
# Existing footprint ownership: remove hidden terrain beneath actual walking meshes.
walk=[]
for name in ['Surface_104','Surface_173','Surface_125','Surface_153','Surface_174','Surface_175','Surface_127','Surface_128','Surface_129','Surface_134','RoadLoop_125314010_R65']:
 if name not in data:continue
 a=data[name];q=a['v'][a['tri']];c=q.mean(1);sel=(c[:,0]>650)&(c[:,0]<870)&(c[:,2]>130)&(c[:,2]<452);walk.extend(shapely.polygons(q[sel][:,:,[0,2]]))
walkpoly=shapely.set_precision(shapely.union_all(walk),.0005).intersection(core)
# Island foundation plane is inherited mesh grade, not a surveyed elevation.
a=data['RoadLoop_125314010_R65'];q=a['v'][a['tri']];p=q.reshape(-1,3);islandplane=np.linalg.lstsq(np.c_[np.ones(len(p)),p[:,0]-783.948,p[:,2]-191.167],p[:,1],rcond=None)[0]
resume=int(sys.argv[sys.argv.index('--resume-index')+1]) if '--resume-index' in sys.argv else 0
if resume:
 prior=json.loads((D/'grading.json').read_text())['objects'];skip={r['path'] for r in rows[:resume]};report=[r for r in prior if r['path'] in skip and r['path'].split('/')[-1]!='Context_132']
for row in rows[resume:]:
 name=row['path'].split('/')[-1];a=np.load(D/(row['key']+'-native.npz'));v=a['v'];tri=a['tri'];uv=a['uv'];slots=a['slots'];q=v[tri]
 if '/MarketIsland_R62/' in row['path']:
  before=v.copy();new=v.copy();old=islandplane[0]+islandplane[1]*(v[:,0]-783.948)+islandplane[2]*(v[:,2]-191.167);new[:,1]+=target(v[:,[0,2]])-old
  np.savez_compressed(D/(row['key']+'-graded.npz'),v=new,tri=tri,before=before,uv=uv,slots=slots)
  report.append({**row,'mode':'island foundation de-tilt; XZ fixed','vertices':len(v),'triangles':len(tri),'delta_range':[float((new[:,1]-v[:,1]).min()),float((new[:,1]-v[:,1]).max())]});continue
 if name not in ground:continue
 c=q.mean(1);normal=np.cross(q[:,1]-q[:,0],q[:,2]-q[:,0]);projected=abs(normal[:,1]);bounds=(q[:,:,0].max(1)>450)&(q[:,:,0].min(1)<1000)&(q[:,:,2].max(1)>0)&(q[:,:,2].min(1)<560)
 intersects=np.zeros(len(tri),bool);ix=np.flatnonzero(bounds);intersects[ix]=shapely.intersects(shapely.polygons(q[ix][:,:,[0,2]]),core)
 # Structural vertical walls/steps and lake bank surfaces are excluded from floor remesh.
 select=intersects&(projected>1e-8)
 if name in ['Surface_142']:
  select&=c[:,1]>.3
 if name in ['Surface_142','Surface_176','Context_124']:
  select&=~((c[:,0]<704)&(c[:,2]>390))
  select&=c[:,1]>.3
 if not select.any():continue
 shape=shapely.set_precision(shapely.union_all(shapely.polygons(q[select][:,:,[0,2]])),.0005);area0=shape.area
 if name in ['Surface_142','Surface_176','Context_124','Restored_029']:shape=shape.difference(walkpoly)
 sam=Sampler(v,tri,uv);newtris=[]
 for poly in pieces(shape):
  if poly.area<1e-8:continue
  boundary=[]
  for ring in [poly.exterior]+list(poly.interiors):
   co=np.asarray(ring.coords)
   for aa,bb in zip(co[:-1],co[1:]):
    n=max(1,int(np.ceil(np.linalg.norm(bb-aa)/1.25)));boundary.extend(aa+(bb-aa)*np.arange(n)[:,None]/n)
  lo=np.array(poly.bounds[:2]);hi=np.array(poly.bounds[2:]);gx,gz=np.meshgrid(np.arange(lo[0],hi[0]+.1,1.5),np.arange(lo[1],hi[1]+.1,1.3));pp=np.c_[gx.ravel(),gz.ravel()];pp=pp[shapely.contains(poly,shapely.points(pp))];points=np.unique(np.round(np.vstack([boundary,pp]),6),axis=0)
  if len(points)<3:continue
  # Constrained quality triangulation avoids the clipped long slivers exposed by full-face audit.
  vertices=[];segments=[];holes=[]
  for ring in [poly.exterior]+list(poly.interiors):
   co=np.asarray(ring.coords)[:-1];start=len(vertices);vertices.extend(co.tolist());segments.extend([(start+j,start+(j+1)%len(co)) for j in range(len(co))])
  for ring in poly.interiors:
   holepoint=shapely.Polygon(ring).representative_point();holes.append([holepoint.x,holepoint.y])
  vv0,remap=np.unique(np.round(np.array(vertices),7),axis=0,return_inverse=True);seg=remap[np.array(segments,dtype='i4')];seg=seg[seg[:,0]!=seg[:,1]];seg=np.unique(np.sort(seg,axis=1),axis=0)
  inp={'vertices':vv0,'segments':seg.astype('i4')}
  if holes:inp['holes']=np.array(holes)
  print('TRIANGULATE',name,round(poly.area,3),len(vv0),flush=True)
  result=trmesh.triangulate(inp,'pq15a1.5S50000Q')
  if 'triangles' in result:newtris.extend(result['vertices'][result['triangles']].tolist())
 if not newtris:continue
 nq=np.round(np.array(newtris),4);flat=nq.reshape(-1,2);old,nu,sid,bw=sam.sample(flat);old[:,0]=flat[:,0];old[:,2]=flat[:,1];height=designed_heights(sam,flat,old[:,1]);nv=np.c_[flat[:,0],height,flat[:,1]];nt=np.arange(len(flat)).reshape(-1,3);flip=np.cross(nq[:,1]-nq[:,0],nq[:,2]-nq[:,0])<0;nt[flip]=nt[flip][:,::-1]
 oq=q[~select].reshape(-1,3).copy();adj=np.isin(tri[~select].ravel(),np.unique(tri[select]));op=oq[:,[0,2]];oq[adj,1]=designed_heights(sam,op[adj],oq[adj,1]) if adj.any() else oq[adj,1];allv=np.vstack([oq,nv]);allb=np.vstack([q[~select].reshape(-1,3),old]);allt=np.vstack([np.arange(len(oq)).reshape(-1,3),nt+len(oq)]);alluv=np.vstack([uv[~select].reshape(-1,2),nu]);allslots=np.r_[slots[~select],slots[sid.reshape(-1,3)[:,0]]]
 _,keep,inv=np.unique(np.round(allv,6),axis=0,return_index=True,return_inverse=True);vv=allv[keep];bb=allb[keep];tt=inv[allt];uu=alluv[allt];nn=np.cross(vv[tt[:,1]]-vv[tt[:,0]],vv[tt[:,2]]-vv[tt[:,0]]);good=np.linalg.norm(nn,axis=1)>1e-10;tt,uu,sl=tt[good],uu[good],allslots[good];_,keep=np.unique(np.sort(tt,axis=1),axis=0,return_index=True);keep=np.sort(keep);floor_flags=np.r_[np.zeros(int((~select).sum()),bool),np.ones(len(nt),bool)][good][keep];tt,uu,sl=tt[keep],uu[keep],sl[keep]
 np.savez_compressed(D/(row['key']+'-graded.npz'),v=vv,tri=tt,before=bb,uv=uu,slots=sl,is_floor=floor_flags)
 dy=vv[:,1]-bb[:,1];changed=abs(dy)>1e-5;report.append({**row,'mode':'full-footprint remesh and shared design field','vertices':len(vv),'triangles':len(tt),'cut_hidden_area':area0-shape.area,'selected_faces':int(select.sum()),'changed_vertices':int(changed.sum()),'delta_range':[float(dy.min()),float(dy.max())],'changed_bounds':[vv[changed].min(0).tolist(),vv[changed].max(0).tolist()] if changed.any() else None});print('GRADED',name,len(vv),int(changed.sum()),flush=True)
(D/'grading.json').write_text(json.dumps(dict(status='BLENDER_GATE_PENDING',design='User authorized game grade, not field reconstruction',profile_nodes=[[120,6.10],[160,6.02],[220,5.90],[275,5.48],[300,5.04],[320,4.78],[435,1.12],[452,.88]],polygon=list(core.exterior.coords),crossfall=.002,langfarm_apron=5.04,island_old_plane=islandplane.tolist(),objects=report),indent=2));print('GRADED_COMPLETE',len(report),flush=True)
