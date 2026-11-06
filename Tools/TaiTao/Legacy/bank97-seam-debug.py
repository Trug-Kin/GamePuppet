import bpy,json,numpy as np
from pathlib import Path
from mathutils import Vector
from mathutils.bvhtree import BVHTree
R=Path('D:/ConRoi');D=R/'Test/KetQua/journey_v48/bank_r97';M=R/'Test/BangChung/journey_v48/bank_r97'
bpy.ops.wm.open_mainfile(filepath=str(R/'Source/Blender/JourneyV48/R97/Bridgehead_Bank_R97_Editable.blend'))
out=[]
for o in list(bpy.context.scene.objects):
 if not o.get('bank97_cut') or o.hide_render:continue
 v=[o.matrix_world@v.co for v in o.data.vertices];tree=BVHTree.FromPolygons(v,[tuple(p.vertices) for p in o.data.polygons])
 for x in [680,681,682,683,684,686,690]:
  h,n,idx,d=tree.ray_cast(Vector((-x,-450,30)),Vector((0,0,-1)),60)
  if h:out.append(dict(object=o.name,x=x,h=h.z,normal=list(n)))
(D/'seam-ray-debug.json').write_text(json.dumps(out,indent=2));print('RESULT',out,flush=True)
sc=bpy.context.scene;sc.display.shading.show_shadows=False;sc.display.shading.show_backface_culling=False;sc.render.filepath=str(M/'seam-backface-diagnostic.png');bpy.ops.render.render(write_still=True)
