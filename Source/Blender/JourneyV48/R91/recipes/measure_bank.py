import bpy,json
from pathlib import Path
from mathutils import Vector
from mathutils.bvhtree import BVHTree
R=Path('D:/Game/GameHorror/Game');O=R/'Docs/Reviews/journey_v48/r91'
bpy.ops.wm.open_mainfile(filepath=str(R/'Docs/Reviews/journey_v48/r90_review/market-branches-r90-source.blend'))
trees={}
for name in ('Grade_017','Grade_015','Grade_000','Grade_012','Grade_076','Grade_057','TerrainJoin_R90'):
 o=bpy.data.objects.get(name)
 if o and o.type=='MESH':trees[name]=BVHTree.FromPolygons([o.matrix_world@v.co for v in o.data.vertices],[list(p.vertices) for p in o.data.polygons])
points=[(565.15,531.84),(590.44,540.67),(596.05,542.17),(601,542.37),(606.82,539.77),(628,522.71)]
rows=[]
for x,z in points:
 samples=[]
 for offset in (-2,-.5,0,.5,1,2,3,4,6,8,10,12):
  hits={}
  for name,tree in trees.items():
   p,n,idx,d=tree.ray_cast(Vector((-x,-z-offset,50)),Vector((0,0,-1)),100)
   if p is not None:hits[name]={'y':p.z,'normal_z':n.z}
  samples.append({'dry_offset':offset,'hits':hits})
 rows.append({'shore':[x,z],'samples':samples})
(O/'bank-sections-before.json').write_text(json.dumps(rows,indent=2));print('BANK_SECTIONS',json.dumps(rows),flush=True)
