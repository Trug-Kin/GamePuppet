import bpy,json,numpy as np
from pathlib import Path
from mathutils import Vector
from mathutils.bvhtree import BVHTree
R=Path('D:/ConRoi');D=R/'Test/KetQua/journey_v48/dam_r99';bpy.ops.wm.open_mainfile(filepath=str(R/'Source/Blender/JourneyV48/R99/OngDao_Dam_R99_SectionCandidate.blend'))
A=np.array([637.7344,528.2603]);B=np.array([681.4578,491.2729]);L=float(np.linalg.norm(B-A));T=(B-A)/L;N=np.array([T[1],-T[0]])
def pos(s,d,h):
 x,z=A+T*s+N*d;return Vector((-x,-z,h))
trees={}
for o in bpy.context.scene.objects:
 if o.type!='MESH' or o.hide_render or o.name.startswith('I2_BEFORE') or 'WATER' in o.name or o.name=='C95_0103_Surface_051':continue
 if any(c.hide_render for c in o.users_collection):continue
 v=[o.matrix_world@p.co for p in o.data.vertices]
 if not v:continue
 vv=np.array(v);sd=(np.c_[-vv[:,0],-vv[:,1]]-A)@np.c_[T,N]
 if sd[:,0].max()<-13 or sd[:,0].min()>L+5 or sd[:,1].max()<-.1 or sd[:,1].min()>10:continue
 trees[o.name]=BVHTree.FromPolygons(v,[tuple(p.vertices) for p in o.data.polygons])
rows=[];counts={}
for tag,ss in [('GATE',np.linspace(L+.15,L+2.85,10)),('FAR',np.linspace(-2.85,-.15,10))]:
 for s in ss:
  for d in np.linspace(.1,4.4,30):
   expected=3-2.78/14*max(0,min(14,np.ceil((d-.45)/((4.5-1.1)/14)-1e-7)))
   for name,tree in trees.items():
    if name.startswith('R99_'):continue
    p,n,idx,dist=tree.ray_cast(pos(s,d,5),Vector((0,0,-1)),6)
    if p and p.z>expected+.035 and p.z<expected+1.6:
     counts[name]=counts.get(name,0)+1;rows.append(dict(tag=tag,s=float(s),d=float(d),expected=float(expected),object=name,h=float(p.z),normal=list(n)))
low=[]
for s in [-3.1,-3.5,-4,-5,-7,-10,-12]:
 for d in [2,3,4,5,6,7]:
  hits=[]
  for name,tree in trees.items():
   p,n,i,dist=tree.ray_cast(pos(s,d,6),Vector((0,0,-1)),10)
   if p:hits.append(dict(object=name,h=float(p.z),normal=list(n)))
  low.append(dict(s=s,d=d,hits=hits))
(D/'endpoint-surface-audit.json').write_text(json.dumps(dict(intrusion_counts=counts,intrusions=rows,far_surfaces=low),indent=2));print('RESULT',counts,flush=True)
