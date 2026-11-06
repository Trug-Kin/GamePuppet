import bpy,json
from pathlib import Path
from mathutils import Vector
from mathutils.bvhtree import BVHTree
R=Path('D:/ConRoi');D=R/'Test/KetQua/journey_v48/map_r99_closeout';bpy.ops.wm.open_mainfile(filepath=str(R/'Source/Blender/JourneyV48/R100/OngDao_Dam_R100_Editable.blend'));o=bpy.data.objects['R99_FAR_STAIR_AND_LOW_LANDING'];tr=BVHTree.FromPolygons([o.matrix_world@p.co for p in o.data.vertices],[tuple(p.vertices) for p in o.data.polygons]);rows=[]
for j in range(901):
 d=j*.005;s=-1.5;x=637.7344+.76346655*s-.64584737*d;z=528.2603-.64584737*s-.76346655*d;p,n,_,_=tr.ray_cast(Vector((-x,-z,8)),Vector((0,0,-1)),12);rows.append(dict(d=d,h=p.z if p else None))
(D/'source-far-stair-dense-section.json').write_text(json.dumps(rows,indent=2));print('RESULT source section901, no source changes')
