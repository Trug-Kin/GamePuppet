import bpy,bmesh,json,math
from pathlib import Path
from mathutils import Vector
from mathutils.bvhtree import BVHTree
R=Path('D:/Game/GameHorror/Game');O=R/'Docs/Reviews/journey_v48/r91'
bpy.ops.wm.open_mainfile(filepath='C:/Users/kaito/AppData/Local/Temp/ConRoi-r91-authoring-20261101/north-bank-r91-source-conforming.blend')
audit=[]
for o in bpy.data.objects:
 if not o.name.startswith('BankR91_') or o.type!='MESH':continue
 bm=bmesh.new();bm.from_mesh(o.data)
 deg=sum(f.calc_area()<1e-9 for f in bm.faces)
 audit.append(dict(object=o.name,vertices=len(bm.verts),faces=len(bm.faces),degenerate=deg,boundary=sum(e.is_boundary for e in bm.edges),nonmanifold=sum(not e.is_manifold for e in bm.edges)))
 bm.free()
trees={o.name:BVHTree.FromPolygons([o.matrix_world@v.co for v in o.data.vertices],[list(p.vertices) for p in o.data.polygons]) for o in bpy.data.objects if o.name.startswith('BankR91_') and o.type=='MESH'}
rows=[]
for x,z in [(565.15,531.84),(590.44,540.67),(596.05,542.17),(601,542.37),(606.82,539.77),(628,522.71)]:
 samples=[]
 for offset in (-2,-.5,0,.5,1,2,3,4,6,8,10,12):
  hits={}
  for name,t in trees.items():
   p,n,idx,d=t.ray_cast(Vector((-x,-z-offset,50)),Vector((0,0,-1)),100)
   if p is not None:hits[name]=dict(y=p.z,normal_z=n.z)
  samples.append(dict(dry_offset=offset,hits=hits))
 rows.append(dict(shore=[x,z],samples=samples))
(O/'bank-audit-after.json').write_text(json.dumps(audit,indent=2));(O/'bank-sections-after.json').write_text(json.dumps(rows,indent=2))
print('BANK_AUDIT',json.dumps(audit),flush=True)
