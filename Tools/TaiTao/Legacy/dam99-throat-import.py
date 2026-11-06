import bpy,bmesh,json,numpy as np
from pathlib import Path
from mathutils import Vector
from mathutils.bvhtree import BVHTree
R=Path('D:/ConRoi');D=R/'Test/KetQua/journey_v48/dam_r99';P=R/'Source/Blender/JourneyV48/R99/OngDao_Dam_R99_SectionCandidate.blend';bpy.ops.wm.open_mainfile(filepath=str(P));A=np.array([637.7344,528.2603]);B=np.array([681.4578,491.2729]);L=float(np.linalg.norm(B-A));T=(B-A)/L;N=np.array([T[1],-T[0]]);report=[]
for row in json.loads((D/'throat-clipped-geometry.json').read_text()):
 o=bpy.data.objects[row['name']];old=o.data;ov=[o.matrix_world@v.co for v in old.vertices];pivot=Vector(np.mean(np.array(ov),axis=0));old.calc_loop_triangles();tree=BVHTree.FromPolygons([p-pivot for p in ov],[tuple(p.vertices) for p in old.loop_triangles]);samples=[]
 for p in old.polygons:
  q=o.matrix_world@p.center;hit,n,i,d=tree.find_nearest(q-pivot)
  if not hit:continue
  glob=hit+pivot;s,dd=(np.array([-glob.x,-glob.y])-A)@np.c_[T,N]
  if not L-.002<s<L+3.002 or not -.002<dd<4.502:samples.append(hit)
 verts=[];faces=[];lookup={};uv=[]
 for poly in row['polygons']:
  ids=[]
  for a in poly['a']:
   key=tuple(np.round(a,8));ix=lookup.get(key)
   if ix is None:ix=len(verts);lookup[key]=ix;verts.append((-a[0],-a[2],a[1]));uv.append(a[3:])
   ids.append(ix)
  faces.append(ids)
 me=bpy.data.meshes.new(row['name']+'_R99_SCOPED_STAIR_CUT');me.from_pydata(verts,[],faces);me.update()
 for mat in old.materials:me.materials.append(mat)
 for ch,name in enumerate(row['uvs']):
  layer=me.uv_layers.new(name=name)
  for p in me.polygons:
   for li in p.loop_indices:layer.data[li].uv=uv[me.loops[li].vertex_index][ch*2:ch*2+2]
 for p,rec in zip(me.polygons,row['polygons']):p.material_index=rec['slot'];p.use_smooth=rec['smooth']
 me.update()
 after=BVHTree.FromPolygons([v.co-pivot for v in me.vertices],[tuple(p.vertices) for p in me.polygons]);errors=[after.find_nearest(q)[3] for q in samples];mx=max(errors,default=0);bad=int(np.argmax(errors)) if errors else 0;print('WORST',list(samples[bad]+pivot),list(after.find_nearest(samples[bad])[0]+pivot), 'sd',(np.array([-(samples[bad]+pivot).x,-(samples[bad]+pivot).y])-A)@np.c_[T,N],flush=True);assert mx<.001,(o.name,'outside surface changed',mx)
 o.data=me;o.matrix_world.identity();report.append(dict(object=o.name,old_polygons=len(old.polygons),new_polygons=len(me.polygons),outside_surface_samples=len(samples),outside_max_distance=float(mx),method='Exact convex footprint subtraction + planar material caps; old world matrix applied, original UV retained'))
bpy.context.scene['dam99_throat_refinement']='Scoped subtraction from concrete/soil (20/10 source faces), caps only on cut planes. Outside surface deviation checked, no global boolean retained.'
(D/'throat-integration-blender.json').write_text(json.dumps(report,indent=2));bpy.context.preferences.filepaths.save_version=0;bpy.ops.wm.save_as_mainfile(filepath=str(P),compress=True);print('RESULT',report,flush=True)




