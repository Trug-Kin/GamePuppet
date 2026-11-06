import bpy,bmesh,json,numpy as np
from pathlib import Path
R=Path('D:/ConRoi');D=R/'Test/KetQua/journey_v48/dam_r99';P=R/'Source/Blender/JourneyV48/R99/OngDao_Dam_R99_SectionCandidate.blend'
bpy.ops.wm.open_mainfile(filepath=str(P));rows=[]
for o in bpy.context.scene.objects:
 if not o.name.startswith('R99_SUPPORT_') or o.get('dam99_native_winding_fixed'):continue
 # Actual Unity world positions were converted through the reflected native
 # basis (-X,-Z,Y). Winding must reverse with that negative determinant.
 bm=bmesh.new();bm.from_mesh(o.data);before=sum(f.normal.z*f.calc_area() for f in bm.faces);bmesh.ops.reverse_faces(bm,faces=list(bm.faces));bm.normal_update();after=sum(f.normal.z*f.calc_area() for f in bm.faces);bm.to_mesh(o.data);bm.free();o.data.update();o['dam99_native_winding_fixed']=True;rows.append(dict(object=o.name,oriented_z_area_before=before,oriented_z_area_after=after,geometry_positions_unchanged=True))
bpy.context.preferences.filepaths.save_version=0;bpy.ops.wm.save_as_mainfile(filepath=str(P),compress=True);(D/'collider-native-winding.json').write_text(json.dumps(rows,indent=2));print('RESULT',len(rows),'independent collider sheets reflected winding corrected',flush=True)
