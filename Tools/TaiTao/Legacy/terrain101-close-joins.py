import bpy,json,numpy as np,shutil
from pathlib import Path
from mathutils import Vector
from mathutils.bvhtree import BVHTree
R=Path('D:/ConRoi');D=R/'Test/KetQua/journey_v48/terrain_r101';P=R/'Source/Blender/JourneyV48/R101/Terrain_R101_Editable.blend'
assert not (D/'join-delta.json').exists();bpy.ops.wm.open_mainfile(filepath=str(P))
base=bpy.data.objects['BankR91_Grade_017'];vv=[base.matrix_world@v.co for v in base.data.vertices];tree=BVHTree.FromPolygons(vv,[tuple(p.vertices) for p in base.data.polygons]);rows=[]
co=np.array([[411.186473,641.841459],[426.079479,652.028465]])
shore=lambda z:float(np.interp(z,co[:,0],co[:,1]))
for name in ['R97_LOWER_WALK','R97_LOWER_GRASS']:
 o=bpy.data.objects[name];boundary=sorted([( -(o.matrix_world@p.co).x-shore(418),p.index,(o.matrix_world@p.co).z) for p in o.data.vertices if abs((o.matrix_world@p.co).y+418)<.001]);ds=[];delta=[]
 for d,i,h in boundary:
  x=-(o.matrix_world@o.data.vertices[i].co).x;p,n,idx,dist=tree.ray_cast(Vector((-x,-417.999,10)),Vector((0,0,-1)),30);assert p is not None,(name,x)
  ds.append(d);delta.append(p.z-h)
 changed=[]
 for p in o.data.vertices:
  w=o.matrix_world@p.co;z=-w.y
  if z>420.001:continue
  t=max(0,min(1,(z-418)/2));fade=1-t*t*(3-2*t);dy=float(np.interp(-w.x-shore(z),ds,delta))*fade
  if abs(dy)<1e-7:continue
  changed.append(dict(vertex=p.index,before=w.z,after=w.z+dy,x=-w.x,z=z));w.z+=dy;p.co=o.matrix_world.inverted()@w
 o.data.update();o['terrain101_join']='Endpoint matched to visible Surface142, fading out by Z420; XY fixed.';rows.append(dict(object=name,vertices_changed=len(changed),max_abs_dy=max(abs(r['after']-r['before']) for r in changed),boundary_delta=delta,changes=changed))
# Remove only obsolete support polygons inside the repaired terrain rectangle.
# The visible Surface142 now supplies its own identical collider there.
o=bpy.data.objects['M95_Service3_NATIVE'];code=(R/'Test/Tam/bank97-author.py').read_text();exec(code[code.index('def half('):code.index('cut=[]')]);q=np.array([[640,411],[658,411],[658,418],[640,418]])
me=o.data;vs=[o.matrix_world@p.co for p in me.vertices];uvs=list(me.uv_layers);verts=[];faces=[];uvout=[[] for _ in uvs];slots=[];changed=0
for p in me.polygons:
 arr=[np.r_[-vs[me.loops[li].vertex_index].x,vs[me.loops[li].vertex_index].z,-vs[me.loops[li].vertex_index].y,*[list(u.data[li].uv) for u in uvs]] for li in p.loop_indices]
 pieces=difference(arr,q)
 def area(a):return sum(np.linalg.norm(np.cross(a[i][:3]-a[0][:3],a[i+1][:3]-a[0][:3]))*.5 for i in range(1,len(a)-1))
 if abs(area(arr)-sum(area(a) for a in pieces))>1e-8:changed+=1
 else:pieces=[arr]
 for a in pieces:
  ids=[]
  for v in a:
   ids.append(len(verts));verts.append((-v[0],-v[2],v[1]));[uvout[j].append(v[3+j*2:5+j*2]) for j in range(len(uvs))]
  for i in range(1,len(ids)-1):
   if np.linalg.norm(np.cross(np.array(verts[ids[i]])-verts[ids[0]],np.array(verts[ids[i+1]])-verts[ids[0]]))<1e-9:continue
   faces.append((ids[0],ids[i],ids[i+1]));slots.append(p.material_index)
new=bpy.data.meshes.new('Context107_R101_LocalObsoleteSupportCut');new.from_pydata(verts,[],faces);new.update()
for m in me.materials:new.materials.append(m)
for j,u in enumerate(uvs):
 layer=new.uv_layers.new(name=u.name)
 for p in new.polygons:
  for li in p.loop_indices:layer.data[li].uv=uvout[j][new.loops[li].vertex_index]
for p,slot in zip(new.polygons,slots):p.material_index=slot
o.data=new;o.matrix_world.identity();o['terrain101_cut']='Obsolete invisible support removed only X640..658/Z411..418; actual terrain render collider remains, no replacement overlay.'
o['dam99_binding_path']='LakeGateMarketV32/Context_107';o['dam99_actual_renderer']=False;o['dam99_actual_collider']=True
rows.append(dict(object=o.name,clipped_original_faces=changed,triangles=len(faces),scope_unity_xz=[640,658,411,418]));assert changed>0
(D/'join-delta.json').write_text(json.dumps(rows,indent=2));bpy.ops.wm.save_as_mainfile(filepath=str(P));bpy.ops.wm.read_factory_settings(use_empty=True);bpy.ops.wm.open_mainfile(filepath=str(P));print('RESULT',json.dumps([{k:v for k,v in r.items() if k!='changes'} for r in rows]),flush=True)
