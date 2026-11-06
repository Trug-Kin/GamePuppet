import bpy,bmesh,json
from pathlib import Path
from mathutils import Vector
from mathutils.bvhtree import BVHTree
R=Path('D:/ConRoi');D=R/'Test/KetQua/journey_v48/terrain_r101';P=R/'Source/Blender/JourneyV48/R101/Terrain_R101_Editable.blend';assert not (D/'seam-knots.json').exists();bpy.ops.wm.open_mainfile(filepath=str(P))
names=['BankR91_Grade_017','R97_LOWER_WALK','R97_LOWER_GRASS'];base=bpy.data.objects[names[0]];v=[base.matrix_world@p.co for p in base.data.vertices];tree=BVHTree.FromPolygons(v,[tuple(p.vertices) for p in base.data.polygons]);knots=[]
for name in names:
 o=bpy.data.objects[name]
 for p in o.data.vertices:
  w=o.matrix_world@p.co
  if abs(w.y+418)<.001 and 646.501<-w.x<652.303:knots.append(-w.x)
knots=sorted(set(round(x,5) for x in knots));rows=[]
for name in names:
 o=bpy.data.objects[name];bm=bmesh.new();bm.from_mesh(o.data);inv=o.matrix_world.inverted();added=0
 for x in knots:
  for e in list(bm.edges):
   a,b=[o.matrix_world@p.co for p in e.verts]
   if abs(a.y+418)>.001 or abs(b.y+418)>.001 or abs(a.x-b.x)<1e-6:continue
   lo,hi=sorted([-a.x,-b.x])
   if not lo+1e-4<x<hi-1e-4:continue
   fraction=(-x-a.x)/(b.x-a.x);_,new=bmesh.utils.edge_split(e,e.verts[0],fraction);added+=1
 for p in bm.verts:
  w=o.matrix_world@p.co;x=-w.x
  if abs(w.y+418)>.001 or not 646.501<x<652.303:continue
  hit,n,i,d=tree.ray_cast(Vector((-x,-417.9999,10)),Vector((0,0,-1)),30);assert hit is not None
  w.y=-418;w.z=hit.z;p.co=inv@w
 bmesh.ops.triangulate(bm,faces=[f for f in bm.faces if len(f.verts)>3]);bm.to_mesh(o.data);bm.free();o.data.update();rows.append(dict(object=name,edge_vertices_added=added))
bpy.ops.wm.save_as_mainfile(filepath=str(P));bpy.ops.wm.read_factory_settings(use_empty=True);bpy.ops.wm.open_mainfile(filepath=str(P));(D/'seam-knots.json').write_text(json.dumps(dict(shared_unity_z=418,shared_x_knots=knots,objects=rows,reason='Different tessellation at visible seam left interpolation gaps up to ~.04 despite endpoint matches; shared edge knots preserve reference profile and close seam.'),indent=2));print('RESULT',json.dumps(rows),flush=True)
