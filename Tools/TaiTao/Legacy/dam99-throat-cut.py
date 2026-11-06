import bpy,bmesh,json,numpy as np
from pathlib import Path
from mathutils import Vector,Matrix
from mathutils.bvhtree import BVHTree
R=Path('D:/ConRoi');D=R/'Test/KetQua/journey_v48/dam_r99';P=R/'Source/Blender/JourneyV48/R99/OngDao_Dam_R99_SectionCandidate.blend'
bpy.ops.wm.open_mainfile(filepath=str(P));A=np.array([637.7344,528.2603]);B=np.array([681.4578,491.2729]);L=float(np.linalg.norm(B-A));T=(B-A)/L;N=np.array([T[1],-T[0]])
# Remove exact old planter/soil throat from the full width of the authored stair.
# Rebase only for boolean numerical stability, restore map-datum origin before save.
def pos(s,d,h):
 x,z=A+T*s+N*d;return Vector((-x,-z,h))
v=[pos(s,d,h) for h in [-2,12] for d in [-.02,4.6] for s in [L-.04,L+3.04]];f=[(0,2,3,1),(4,5,7,6),(0,1,5,4),(2,6,7,3),(0,4,6,2),(1,3,7,5)];me=bpy.data.meshes.new('R99_TEMP_THROAT_CUTTER');me.from_pydata(v,[],f);me.update();c=bpy.data.objects.new('R99_TEMP_THROAT_CUTTER',me);bpy.context.scene.collection.objects.link(c)
bm=bmesh.new();bm.from_mesh(me);bmesh.ops.recalc_face_normals(bm,faces=list(bm.faces));bm.to_mesh(me);bm.free();pivot=pos(L+1.5,2,0);c.data.transform(Matrix.Translation(-pivot));c.location=pivot
rows=[]
for name in ['C95_0041_OD35_Surface_1','C95_0045_OD35_Surface_5']:
 o=bpy.data.objects[name];count=len(o.data.polygons);samples=[]
 for poly in o.data.polygons:
  if poly.index%5:continue
  q=o.matrix_world@poly.center;ss,dd=(np.array([-q.x,-q.y])-A)@np.c_[T,N]
  if not L-.10<ss<L+3.10 or not -.08<dd<4.66:samples.append(q.copy())
 oldtree=BVHTree.FromPolygons([o.matrix_world@v.co for v in o.data.vertices],[tuple(p.vertices) for p in o.data.polygons]);oldpoints=[o.matrix_world@Vector(q) for q in o.bound_box];lo=np.min(np.array(oldpoints),axis=0);hi=np.max(np.array(oldpoints),axis=0);o.data=o.data.copy();o.data.transform(o.matrix_world);o.matrix_world.identity();o.data.transform(Matrix.Translation(-pivot));o.location=pivot
 mod=o.modifiers.new('R99_AUTHORED_STAIR_THROAT_DIFFERENCE','BOOLEAN');mod.operation='DIFFERENCE';mod.solver='EXACT';mod.object=c
 bpy.context.view_layer.update();bpy.context.view_layer.objects.active=o;o.select_set(True);bpy.ops.object.modifier_apply(modifier=mod.name);o.select_set(False)
 tree=BVHTree.FromPolygons([o.matrix_world@p.co for p in o.data.vertices],[tuple(p.vertices) for p in o.data.polygons]);distances=[float(tree.find_nearest(q)[3]) for q in samples];outside_max=max(distances,default=0);print('DEBUG_SAMPLE',list(samples[0]) if samples else [],'MATRIX',list(o.matrix_world),'BBOX',[list(o.matrix_world@Vector(q)) for q in o.bound_box],'PARENT',str(o.parent),flush=True);print('CANDIDATE_BOOLEAN',name,'faces',count,len(o.data.polygons),'outside max',outside_max,flush=True)
 envelope=[];absent=0
 for q in samples:
  for axis in range(3):
   for sign in [-1,1]:
    start=q.copy();start[axis]=float(hi[axis]+1 if sign<0 else lo[axis]-1);direction=Vector((0,0,0));direction[axis]=sign
    a,an,ai,ad=oldtree.ray_cast(start,direction,400)
    if not a:continue
    ss,dd=(np.array([-a.x,-a.y])-A)@np.c_[T,N]
    if L-.08<ss<L+3.08 and -.06<dd<4.64:continue
    b,bn,bi,bd=tree.ray_cast(start,direction,400)
    if not b:absent+=1
    else:envelope.append(float((a-b).length))
 envelope_max=max(envelope,default=0);print('EXPOSED_ENVELOPE',name,'samples',len(envelope),'missing',absent,'max',envelope_max,flush=True)
 assert not absent and envelope_max<.002,(name,'exposed geometry changed outside cut',absent,envelope_max)
 o.data.transform(o.matrix_world);o.matrix_world.identity();o.data.update();rows.append(dict(object=name,faces_before=count,faces_after=len(o.data.polygons),outside_samples=len(samples),nearest_centroid_distance_including_internal_caps=outside_max,envelope_samples=len(envelope),envelope_missing=absent,envelope_max_distance=envelope_max,scope_s=[L-.04,L+3.04],scope_d=[-.02,4.6]))
bpy.data.objects.remove(c,do_unlink=True);bpy.context.scene['dam99_throat_cut']='Existing Concrete/Soil render cut from authored stair footprint; no overlay; lamp restored rigidly from original geometry onto bridge side.'
(D/'stair-throat-cut.json').write_text(json.dumps(rows,indent=2));bpy.context.preferences.filepaths.save_version=0;bpy.ops.wm.save_as_mainfile(filepath=str(P),compress=True);print('RESULT throat difference',rows,flush=True)



