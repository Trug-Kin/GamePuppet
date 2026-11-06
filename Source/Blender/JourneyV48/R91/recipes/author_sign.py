import bpy,bmesh,sys,json
from pathlib import Path
R=Path('D:/Game/GameHorror/Game');O=R/'Docs/Reviews/journey_v48/r91';A=R/'Assets/Art/Characters/R91'
sys.path.insert(0,str(R/'.agents/skills/scenario-blender-export/scripts'));import bx_export
bpy.ops.wm.read_factory_settings(use_empty=True)
# Same existing fascia board, reduced to the clear left bay. No new stall or footprint.
bpy.ops.mesh.primitive_cube_add(size=1,location=(-784.25,-273.5344,7.86898));o=bpy.context.object;o.name='R91_ExistingStallSign';o.scale=(1.70,.045,.40);bpy.ops.object.transform_apply(location=True,rotation=True,scale=True)
bm=bmesh.new();bm.from_mesh(o.data);bmesh.ops.triangulate(bm,faces=list(bm.faces));bm.to_mesh(o.data);bm.free();o.data.uv_layers.new(name='UVMap')
o['basis']='Existing V41 Painted_Sign_Backboard, same height/depth; fit clear bay left of fixed canopy post. Inferred game prop, not a surveyed real shop sign.'
bpy.context.preferences.filepaths.save_version=0;bpy.ops.wm.save_as_mainfile(filepath=str(O/'stall-sign-r91-source.blend'),compress=True)
print(bx_export.export(str(A/'R91_ExistingStallSign.fbx'),'unity',[o]),flush=True)
