import bpy,bmesh,json
from pathlib import Path
R=Path('D:/ConRoi');D=R/'Test/KetQua/journey_v48/terrain_r101';P=R/'Source/Blender/JourneyV48/R101/Terrain_R101_Editable.blend';bpy.ops.wm.open_mainfile(filepath=str(P));rows=[]
for name in ['BankR91_Grade_017','R97_LOWER_WALK','R97_LOWER_GRASS']:
 o=bpy.data.objects[name];bm=bmesh.new();bm.from_mesh(o.data);bad=[f for f in bm.faces if len(f.verts)==3 and (f.verts[1].co-f.verts[0].co).cross(f.verts[2].co-f.verts[0].co).length*.5<=1e-10]
 assert all(any(abs((o.matrix_world@v.co).y+418)<.001 for v in f.verts) for f in bad),(name,len(bad))
 bmesh.ops.delete(bm,geom=bad,context='FACES_ONLY');loose=[v for v in bm.verts if not v.link_faces];bmesh.ops.delete(bm,geom=loose,context='VERTS');bm.to_mesh(o.data);bm.free();o.data.update();rows.append(dict(object=name,zero_area_seam_faces_removed=len(bad),unused_vertices_removed=len(loose)))
(D/'seam-null-face-cleanup.json').write_text(json.dumps(rows,indent=2));bpy.ops.wm.save_as_mainfile(filepath=str(P));bpy.ops.wm.read_factory_settings(use_empty=True);bpy.ops.wm.open_mainfile(filepath=str(P));print('RESULT',json.dumps(rows),flush=True)
