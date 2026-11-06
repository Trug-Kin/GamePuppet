import bpy,json,hashlib
from pathlib import Path
from mathutils import Vector
from mathutils.bvhtree import BVHTree
R=Path('D:/ConRoi');D=R/'Test/KetQua/journey_v48/terrain_r101'
bpy.ops.wm.open_mainfile(filepath=str(R/'Source/Blender/JourneyV48/R100/OngDao_Dam_R100_Editable.blend'))
rows=[]
for o in bpy.context.scene.objects:
 if o.type!='MESH':continue
 v=[o.matrix_world@p.co for p in o.data.vertices]
 if not v or not(min(p.x for p in v)<-640 and max(p.x for p in v)>-647 and min(p.y for p in v)<-411 and max(p.y for p in v)>-417):continue
 if not o.get('dam99_binding_path'):continue
 tree=BVHTree.FromPolygons(v,[tuple(p.vertices) for p in o.data.polygons]);hits=[]
 for x,z in [(643.5,414),(642,414),(645,414),(643.5,412),(643.5,416)]:
  p,n,idx,dist=tree.ray_cast(Vector((-x,-z,20)),Vector((0,0,-1)),50)
  if p:hits.append(dict(x=x,z=z,y=p.z,normal=list(n),face=idx))
 local=[]
 for p in o.data.polygons:
  q=[v[i] for i in p.vertices]
  if min(a.x for a in q)<=-640 and max(a.x for a in q)>=-647 and min(a.y for a in q)<=-411 and max(a.y for a in q)>=-417:
   local.append(dict(index=p.index,vertices=list(p.vertices),coordinates=[list(a) for a in q],normal=list(p.normal)))
 rows.append(dict(name=o.name,props={k:o[k] for k in o.keys()},vertices=len(v),polygons=len(o.data.polygons),hits=hits,local_faces=local))
(D/'source-local-geometry.json').write_text(json.dumps(rows,indent=2),encoding='utf8')
print('RESULT',json.dumps([dict(name=r['name'],vertices=r['vertices'],faces=len(r['local_faces']),hits=r['hits'],props=r['props']) for r in rows]),flush=True)
