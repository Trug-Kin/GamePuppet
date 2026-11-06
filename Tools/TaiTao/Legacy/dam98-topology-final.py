import bpy,bmesh,json,numpy as np
from pathlib import Path
R=Path('D:/ConRoi');P=R/'Source/Blender/JourneyV48/R98/OngDao_Dam_R98_Editable.blend';D=R/'Test/KetQua/journey_v48/dam_r98'
bpy.ops.wm.open_mainfile(filepath=str(P));rows=[]
for o in list(bpy.context.scene.objects):
 if not o.get('dam98_export') or o.get('dam98_cut'):continue
 bm=bmesh.new();bm.from_mesh(o.data);before=len(bm.faces);bmesh.ops.triangulate(bm,faces=list(bm.faces),quad_method='FIXED',ngon_method='EAR_CLIP');bad=[f for f in bm.faces if f.calc_area()<1e-9]
 if bad:bmesh.ops.delete(bm,geom=bad,context='FACES_ONLY');bmesh.ops.dissolve_degenerate(bm,dist=.00002,edges=list(bm.edges))
 bmesh.ops.recalc_face_normals(bm,faces=list(bm.faces));edges=sum(not e.is_manifold for e in bm.edges);volume=bm.calc_volume(signed=True);bm.to_mesh(o.data);bm.free();o.data.update();rows.append(dict(name=o.name,faces_before=before,triangles=len(o.data.polygons),degenerate_removed=len(bad),nonmanifold_edges=edges,signed_volume=volume));print(rows[-1],flush=True)
bpy.context.preferences.filepaths.save_version=0;bpy.ops.wm.save_as_mainfile(filepath=str(P),compress=True);(D/'topology-final.json').write_text(json.dumps(rows,indent=2));print('RESULT triangulated source saved',flush=True)
