import bpy,bmesh,json
from pathlib import Path
R=Path('D:/ConRoi');P=R/'Source/Blender/JourneyV48/R99/OngDao_Dam_R99_SectionCandidate.blend';D=R/'Test/KetQua/journey_v48/dam_r99'
bpy.ops.wm.open_mainfile(filepath=str(P));rows=[]
for name in ['R97_STAIR_450_LOWER','R97_STAIR_471_LOWER','R97_STAIR_471_UPPER']:
 o=bpy.data.objects[name];bm=bmesh.new();bm.from_mesh(o.data);fs=[f for f in bm.faces if len(f.verts)>4];count=len(fs)
 bmesh.ops.triangulate(bm,faces=fs,quad_method='BEAUTY',ngon_method='EAR_CLIP');bm.to_mesh(o.data);bm.free();o.data.update();o.data.calc_loop_triangles()
 rows.append(dict(object=name,ngons_triangulated=count,triangles=len(o.data.loop_triangles),reason='Unity importer discarded warped concave side ngons. Explicit source triangulation retains Blender surface.'))
bpy.context.preferences.filepaths.save_version=0;bpy.ops.wm.save_as_mainfile(filepath=str(P),compress=True);(D/'stair-side-triangulation.json').write_text(json.dumps(rows,indent=2));print('RESULT',rows,flush=True)
