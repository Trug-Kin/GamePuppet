import sys,json,math
from pathlib import Path
sys.path.insert(0,'Tools/python-deps')
import numpy as np,shapely
from shapely import STRtree
from scipy.spatial import Delaunay
from scipy.ndimage import gaussian_filter,distance_transform_edt,map_coordinates
D=Path('Test/KetQua/journey_v48/i2_r93_complete');inv=json.loads((D/'native-input.json').read_text())['objects'];cx,cz=714.94,459.48;plane=[.7439415448549217,-.00900005029080838,-.0429999587064346]
names={'Surface_128':0.,'Context_110':0.,'Surface_129':0.,'Context_111':0.,'Surface_153':0.,'Context_136':0.,'Surface_142':0.,'Surface_176':0.,'Context_124':0.}
def ss(t):t=np.clip(t,0,1);return t*t*t*(10+t*(-15+6*t))
class Sampler:
 def __init__(self,v,t,uv):
  q=v[t];a,b,c=q[:,0],q[:,1],q[:,2];den=(b[:,2]-c[:,2])*(a[:,0]-c[:,0])+(c[:,0]-b[:,0])*(a[:,2]-c[:,2]);ok=abs(den)>1e-10;self.q=q[ok];self.den=den[ok];self.ids=np.flatnonzero(ok);self.uv=uv[ok];self.poly=shapely.polygons(self.q[:,:,[0,2]]);self.tree=STRtree(self.poly)
 def sample(self,pp):
  pp=np.atleast_2d(pp);pi,ti=self.tree.query(shapely.points(pp));a,b,c=self.q[ti,0],self.q[ti,1],self.q[ti,2];x,z=pp[pi].T;den=self.den[ti];u=((b[:,2]-c[:,2])*(x-c[:,0])+(c[:,0]-b[:,0])*(z-c[:,2]))/den;v=((c[:,2]-a[:,2])*(x-c[:,0])+(a[:,0]-c[:,0])*(z-c[:,2]))/den;ok=(u>=-1e-6)&(v>=-1e-6)&(u+v<=1.000001);pi,ti,u,v=pi[ok],ti[ok],u[ok],v[ok];h=u*a[ok,1]+v*b[ok,1]+(1-u-v)*c[ok,1];order=np.lexsort((-h,pi));pi,ti,u,v=pi[order],ti[order],u[order],v[order];first=np.r_[True,np.diff(pi)!=0];idx=np.full(len(pp),-1,dtype=int);weights=np.zeros((len(pp),3));idx[pi[first]]=ti[first];weights[pi[first]]=np.c_[u[first],v[first],1-u[first]-v[first]]
  miss=np.flatnonzero(idx<0)
  self.last_valid=idx>=0
  if len(miss):
   ix=self.tree.nearest(shapely.points(pp[miss]));near=shapely.shortest_line(shapely.points(pp[miss]),self.poly[ix]);snap=shapely.get_coordinates(shapely.get_point(near,-1));a,b,c=self.q[ix,0],self.q[ix,1],self.q[ix,2];x,z=snap.T;den=self.den[ix];u=((b[:,2]-c[:,2])*(x-c[:,0])+(c[:,0]-b[:,0])*(z-c[:,2]))/den;v=((c[:,2]-a[:,2])*(x-c[:,0])+(a[:,0]-c[:,0])*(z-c[:,2]))/den;idx[miss]=ix;weights[miss]=np.c_[u,v,1-u-v]
  p=(self.q[idx]*weights[:,:,None]).sum(1);uv=(self.uv[idx]*weights[:,:,None]).sum(1);return p,uv,self.ids[idx],weights
def floor_target(p,offset):
 dx=p[:,0]-cx;dz=p[:,2]-cz;r=np.hypot(dx,dz);co=dx/np.maximum(r,1e-10);si=dz/np.maximum(r,1e-10);ri,ro=13.35,24.;L=ro-ri;t=np.clip((r-ri)/L,0,1);yi=plane[0]+plane[1]*ri*co+plane[2]*ri*si-.04;yo=.72-.004*ro*co-.004*ro*si;mo=-.004*co-.004*si
 inner=(2*t**3-3*t*t+1)*yi+(-2*t**3+3*t*t)*yo+(t**3-t*t)*L*mo
 return np.where(r>ro,.72-.004*dx-.004*dz,inner)+offset
def pieces(g):
 if g.is_empty:return []
 if g.geom_type=='Polygon':return [g]
 if not hasattr(g,'geoms'):return []
 return [p for child in g.geoms for p in pieces(child)]
# Unchanged bridge boundary is a measured constraint, never an editable mesh replacement.
G=Path('Test/KetQua/journey_v48/ground_r93');bi=next(r for r in json.loads((G/'geometry-inventory.json').read_text()) if r['path']=='R11_Local_Bridge_Supports/OD35_Surface_2');ba=np.load(G/(bi['cache']+'.npz'));bv=ba['v'].astype(float);bt=ba['tri'];bq=bv[bt];bc=bq.mean(1);bn=np.cross(bq[:,1]-bq[:,0],bq[:,2]-bq[:,0]);bs=(np.linalg.norm(bc[:,[0,2]]-[cx,cz],axis=1)<55)&(abs(bn[:,1])>1e-5);bpoly=shapely.union_all(shapely.polygons(bq[bs][:,:,[0,2]]));bridge=Sampler(bv,bt[bs],np.zeros((int(bs.sum()),3,2)))
# A shared reference across actual adjacent floor footprints prevents per-layer seams.
# Low-pass removes the inherited isolated connector spike, not a flat datum imposed on the banks.
xs=np.arange(cx-55,cx+56,.25);zs=np.arange(cz-16,cz+61,.25);gx,gz=np.meshgrid(xs,zs);gp=np.c_[gx.ravel(),gz.ravel()];gh=np.full(len(gp),np.nan);walk=[];walkshapes={}
for nm in ['Surface_142','Surface_176','Surface_153','Surface_129','Surface_128']:
 nr=next(r for r in inv if r['path'].endswith('/'+nm));a=np.load(D/(nr['key']+'.npz'));sam=Sampler(a['v'],a['tri'],a['uv']);h,_,_,_=sam.sample(gp);gh[sam.last_valid]=h[sam.last_valid,1]
 if nm in ['Surface_153','Surface_129','Surface_128']:
  q=a['v'][a['tri']];cc=q.mean(1);ok=np.linalg.norm(cc[:,[0,2]]-[cx,cz],axis=1)<65;pol=shapely.polygons(q[ok][:,:,[0,2]]);walk.extend(pol);walkshapes[nm]=shapely.union_all(pol)
gh=gh.reshape(gx.shape);missing=~np.isfinite(gh);nearids=distance_transform_edt(missing,return_distances=False,return_indices=True);gh[missing]=gh[tuple(nearids[:,missing])];smooth=gaussian_filter(gh,2/.25);walkpoly=shapely.union_all(walk)
origin=shapely.Point(cx,cz);roi=origin.buffer(48,quad_segs=128).intersection(walkpoly.buffer(4).union(origin.buffer(30,quad_segs=128))).intersection(shapely.box(cx-55,cz-7,cx+55,cz+60)).difference(origin.buffer(11.5,quad_segs=128))
# Unchanged overlapping authored floors are hard outer boundary conditions, not overlays to move through.
protected=[]
for nr in inv:
 nm=nr['path'].split('/')[-1]
 if nm in names or nr['path'].startswith('LakeGateMarket'):continue
 a=np.load(D/(nr['key']+'.npz'));q=a['v'][a['tri']];cc=q.mean(1);n=np.cross(q[:,1]-q[:,0],q[:,2]-q[:,0]);ok=(np.linalg.norm(cc[:,[0,2]]-[cx,cz],axis=1)<60)&(abs(n[:,1])>1e-9)
 protected.extend(shapely.polygons(q[ok][:,:,[0,2]]))
protected=shapely.union_all(protected)
rows=[]
for row in inv:
 name=row['path'].split('/')[-1]
 if name not in names:continue
 data=np.load(D/(row['key']+'.npz'));v=data['v'];tri=data['tri'];uv=data['uv'];slots=data['slots'];q=v[tri];xz=q[:,:,[0,2]];cen=xz.mean(1);rad=np.linalg.norm(xz-cen[:,None,:],axis=2).max(1);rr=np.linalg.norm(cen-[cx,cz],axis=1);foot=name in ['Surface_153','Context_136'];radius=50
 nn=np.cross(q[:,1]-q[:,0],q[:,2]-q[:,0]);pa=abs(nn[:,1])/2;select=(rr<radius+rad)&(xz[:,:,1].max(1)>cz-10)&(pa>1e-9);candidates=np.flatnonzero(select);select[candidates]=shapely.intersects(shapely.polygons(xz[candidates]),roi);ids=np.flatnonzero(select);shape=shapely.union_all(shapely.set_precision(shapely.polygons(xz[select]),.00001));shape=shapely.make_valid(shape);sourcearea=shape.area
 # Remove actual hidden overlapping floor faces, instead of stacking coincident layers.
 # Footway owns its walking footprint; road owns the remainder; terrain meets those edges.
 if name in ['Surface_142','Surface_176','Context_124']:shape=shape.difference(roi.intersection(walkpoly))
 elif name in ['Surface_128','Context_110']:shape=shape.difference(roi.intersection(walkshapes['Surface_153'].union(walkshapes['Surface_129'])))
 elif name in ['Surface_129','Context_111']:shape=shape.difference(roi.intersection(walkshapes['Surface_153']))
 cutarea=sourcearea-shape.area;shape=shapely.simplify(shape,.00008,preserve_topology=True);polys=[p for p in pieces(shape) if p.area>1e-8];sam=Sampler(v,tri,uv);new=[]
 # Independent from old tessellation: uniform interior points, all footprint boundaries as constraints via clipping.
 for poly in polys:
  mnx,mnz,mxx,mxz=poly.bounds;boundary=[]
  for ring in [poly.exterior]+list(poly.interiors):
   c=np.array(ring.coords)
   for aa,bb in zip(c[:-1],c[1:]):
    count=max(1,int(np.ceil(np.linalg.norm(bb-aa)/.6)));boundary.extend(aa+(bb-aa)*np.arange(count)[:,None]/count)
  xx,zz=np.meshgrid(np.arange(mnx,mxx+.1,.6),np.arange(mnz,mxz+.1,.52));pp=np.c_[xx.ravel(),zz.ravel()];pp=pp[shapely.contains(poly,shapely.points(pp))];pts=np.unique(np.round(np.vstack([np.array(boundary),pp]),7),axis=0)
  if len(pts)<3:continue
  dt=Delaunay(pts);tris=shapely.polygons(pts[dt.simplices]);inside=shapely.covers(poly,tris);new.extend(pts[dt.simplices[inside]].tolist());border=shapely.intersection(tris[~inside],poly)
  for g in border:
   for part in pieces(g):
    if part.area<1e-10:continue
    tt=shapely.constrained_delaunay_triangles(part)
    new.extend([np.asarray(t.exterior.coords)[:3].tolist() for t in tt.geoms if t.area>1e-10])
 # Quantize sub-0.1 mm clipping fragments before sampling: one XZ must have one height.
 # This removes numerical slivers without moving an architectural footprint at visible scale.
 nq=np.round(np.array(new,dtype=float),4);flat=nq.reshape(-1,2);pp,nu,sourceface,weights=sam.sample(flat);r=np.linalg.norm(flat-[cx,cz],axis=1);dz=flat[:,1]-cz;target=floor_target(pp,names[name]);angular=ss((dz+7)/9)
 bp=shapely.points(flat);near=shapely.shortest_line(bp,bpoly);nearest=shapely.get_coordinates(shapely.get_point(near,-1));bh,_,_,_=bridge.sample(nearest);dist=shapely.distance(bp,bpoly)
 reference=map_coordinates(smooth,[(flat[:,1]-zs[0])/.25,(flat[:,0]-xs[0])/.25],order=1,mode='nearest');core=1-ss((r-24)/6);desired=reference+(target-reference)*core;bw=1-ss(dist/6);desired=desired+(bh[:,1]-desired)*bw
 # Terrain remote from the walking footprint / ring is not regraded as a road.
 walkdistance=shapely.distance(bp,walkpoly);corridor=np.maximum(1-ss((walkdistance-1)/3),1-ss((r-26)/4));w=ss((r-11.5)/1.6)*(1-ss((r-40)/8))*angular*corridor
 guard=ss(shapely.distance(bp,protected)/4) if not protected.is_empty else np.ones(len(bp));w*=guard
 height=pp[:,1]+(desired-pp[:,1])*w
 npnew=np.c_[flat[:,0],height,flat[:,1]];before=pp.copy();before[:,0]=flat[:,0];before[:,2]=flat[:,1]
 # Native input winds downward in Unity coordinate convention; retain this, Blender will face up.
 area2=np.cross(nq[:,1]-nq[:,0],nq[:,2]-nq[:,0]);swap=area2<0
 # In XZ CCW = Unity normal -Y. Force CCW projected winding.
 idx=np.arange(len(npnew)).reshape(-1,3);idx[swap]=idx[swap][:,::-1]
 oq=q[~select].reshape(-1,3);ot=np.arange(len(oq)).reshape(-1,3);ouv=uv[~select].reshape(-1,2);allv=np.vstack([oq,npnew]);allb=np.vstack([oq,before]);allt=np.vstack([ot,idx+len(oq)]);alluv=np.vstack([ouv,nu]);allslots=np.r_[slots[~select],slots[sourceface.reshape(-1,3)[:,0]]]
 # Re-weld positions for smooth normals and shared borders; keep per-corner UVs separately.
 quant=np.round(allv,6);_,ix,back=np.unique(quant,axis=0,return_index=True,return_inverse=True);vv=allv[ix];bb=allb[ix];tt=back[allt];uu=alluv[allt];qn=vv[tt];nn=np.cross(qn[:,1]-qn[:,0],qn[:,2]-qn[:,0]);ok=np.linalg.norm(nn,axis=1)>1e-10;tt,uu,allslots=tt[ok],uu[ok],allslots[ok];coverage=float(sum(p.area for p in polys));newarea=float(abs(np.cross(nq[:,1]-nq[:,0],nq[:,2]-nq[:,0])).sum()/2);key='I2_'+name
 np.savez_compressed(D/(key+'-retopo.npz'),v=vv,tri=tt,before=bb,uv=uu,slots=allslots)
 rows.append(dict(path=row['path'],object=row['object'],key=key,native_key=row['key'],selected_source_triangles=len(ids),retopology_triangles=len(nq),vertices=len(vv),triangles=len(tt),footprint_area=coverage,removed_hidden_overlap_area=float(cutarea),retopology_area=newarea,coverage_error=abs(coverage-newarea),source_offset=names[name],radius=radius,bounds=[flat.min(0).tolist(),flat.max(0).tolist()]));print('RETOPO',key,len(vv),len(tt),'coverage',abs(coverage-newarea),'hidden overlap cut',cutarea,flush=True)
(D/'retopology.json').write_text(json.dumps(rows,indent=2))
