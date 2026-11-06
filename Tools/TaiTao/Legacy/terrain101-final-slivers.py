import bpy,bmesh,json
from pathlib import Path
R=Path('D:/ConRoi');D=R/'Test/KetQua/journey_v48/terrain_r101';P=R/'Source/Blender/JourneyV48/R101/Terrain_R101_Editable.blend';assert not (D/'final-seam-slivers.json').exists();bpy.ops.wm.open_mainfile(filepath=str(P));o=bpy.data.objects['BankR91_Grade_017'];bm=bmesh.new();bm.from_mesh(o.data);bad=[]
for f in bm.faces:
 q=[o.matrix_world@p.co for p in f.verts]
 if len(q)!=3 or not all(abs(p.y+418)<.001 for p in q):continue
 if not 646.50< -sum(p.x for p in q)/3 <652.31:continue
 projected=abs((q[1]-q[0]).cross(q[2]-q[0]).z)*.5
 if projected<1e-8:bad.append(f)
count=len(bad);bmesh.ops.delete(bm,geom=bad,context='FACES_ONLY');bm.to_mesh(o.data);bm.free();o.data.update();bpy.ops.wm.save_as_mainfile(filepath=str(P));bpy.ops.wm.read_factory_settings(use_empty=True);bpy.ops.wm.open_mainfile(filepath=str(P));(D/'final-seam-slivers.json').write_text(json.dumps(dict(removed=count,scope='Source142 only, all vertices Z418 +/- .001, X646.50..652.31, projected horizontal area <1e-8. Obsolete vertical fins on shared edge; top surface retained.'),indent=2));print('RESULT removed',count,flush=True)
