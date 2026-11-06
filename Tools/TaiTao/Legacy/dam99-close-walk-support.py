import bpy,bmesh,numpy as np,json
from pathlib import Path
from mathutils import Vector
from mathutils.bvhtree import BVHTree
R=Path('D:/ConRoi');D=R/'Test/KetQua/journey_v48/dam_r99';P=R/'Source/Blender/JourneyV48/R99/OngDao_Dam_R99_SectionCandidate.blend'
bpy.ops.wm.open_mainfile(filepath=str(P));A=np.array([637.7344,528.2603]);B=np.array([681.4578,491.2729]);L=float(np.linalg.norm(B-A));T=(B-A)/L;N=np.array([T[1],-T[0]])
trees=[]
for n in ['BankR91_Grade_017','BankR91_Grade_000','C95_0041_OD35_Surface_1']:
 o=bpy.data.objects[n];trees.append(BVHTree.FromPolygons([o.matrix_world@v.co for v in o.data.vertices],[tuple(p.vertices) for p in o.data.polygons]))
report=[]
for name in ['C95_0637_PlanterWalkConnection_2','C95_0638_PlanterWalkConnection_3']:
 o=bpy.data.objects[name];assert not o.get('dam99_closed_support');me=o.data;bm=bmesh.new();bm.from_mesh(me);boundary=[e for e in bm.edges if e.is_boundary];bottom={};added=0
 for e in boundary:
  top=[o.matrix_world@v.co for v in e.verts];sd=(np.array([[-p.x,-p.y] for p in top])-A)@np.c_[T,N]
  if sd[:,0].max()<-18 or sd[:,0].min()>L+18 or sd[:,1].max()<-.15:continue
  depths=[]
  for v,p in zip(e.verts,top):
   if v not in bottom:
    hs=[]
    for tr in trees:
     q,n,i,d=tr.ray_cast(p-Vector((0,0,.045)),Vector((0,0,-1)),5)
     if q and n.z>.2:hs.append(q.z)
    h=max(hs) if hs else p.z-.3
    h=min(h,p.z-.04);co=p.copy();co.z=h-.015;bottom[v]=bm.verts.new(o.matrix_world.inverted()@co)
   depths.append(bottom[v])
  loop=e.link_loops[0];a=loop.vert;b=loop.link_loop_next.vert
  f=bm.faces.new((b,a,bottom[a],bottom[b]));f.material_index=0
  for li in f.loops:
   for uv in bm.loops.layers.uv.values():li[uv].uv=(li.vert.co.x*.3,li.vert.co.z*.3)
  added+=1
 bm.to_mesh(me);bm.free();me.update();o['dam99_closed_support']='Exposed boundary extended down into measured existing foundation/terrain. Existing top sheet retained; no walking overlay or independent wall.';report.append(dict(object=name,added_side_faces=added,top_faces_unchanged=True,absolute_dimensions='estimated design'))
bpy.context.preferences.filepaths.save_version=0;bpy.ops.wm.save_as_mainfile(filepath=str(P),compress=True);(D/'walk-support-closure.json').write_text(json.dumps(report,indent=2));print('RESULT exposed walk boundaries closed',report,flush=True)
