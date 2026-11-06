import bpy,json
from pathlib import Path
from mathutils import Vector
from mathutils.bvhtree import BVHTree
R=Path('D:/ConRoi');D=R/'Test/KetQua/journey_v48/map_r99_closeout';bpy.ops.wm.open_mainfile(filepath=str(R/'Source/Blender/JourneyV48/R100/OngDao_Dam_R100_Editable.blend'));out=[]
for x,z in [(693,507),(643.5,414)]:
 hits=[]
 for o in bpy.context.scene.objects:
  if o.type!='MESH' or not o.get('dam99_actual_renderer') or any(t in o.name for t in ['FLOWER','FlowerModule','OD35_Surface_3','OD35_Surface_4','OD35_Surface_6','OD35_Surface_7','OD35_Surface_10']):continue
  v=[o.matrix_world@p.co for p in o.data.vertices]
  if not v or not(min(p.x for p in v)<=-x<=max(p.x for p in v) and min(p.y for p in v)<=-z<=max(p.y for p in v)):continue
  tree=BVHTree.FromPolygons(v,[tuple(p.vertices) for p in o.data.polygons]);p,n,_,_=tree.ray_cast(Vector((-x,-z,15)),Vector((0,0,-1)),40)
  if p:hits.append(dict(source=o.name,path=o.get('dam99_binding_path'),height=p.z,normal=list(n)))
 out.append(dict(unity_xz=[x,z],source_render_hits=sorted(hits,key=lambda r:-r['height'])))
(D/'historic-points-current-source-render.json').write_text(json.dumps(out,indent=2));print('RESULT',json.dumps(out),flush=True)
