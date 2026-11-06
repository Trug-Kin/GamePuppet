import bpy,sys,json
from pathlib import Path
R=Path('D:/Game/GameHorror/Game');C=Path('C:/Users/kaito/AppData/Local/Temp/ConRoi-r91-authoring-20261101');A=R/'Assets/Art/Environment/JourneyV48/BankR91'
sys.path.insert(0,str(R/'.agents/skills/scenario-blender-export/scripts'));import bx_export
for source in ['north-bank-r91-source-final.blend','north-bank-r91-extra-final.blend']:
 bpy.ops.wm.open_mainfile(filepath=str(C/source));parts=[o for o in bpy.context.scene.objects if o.type=='MESH' and o.name.startswith('BankR91_')]
 for o in parts:
  for p in o.data.polygons:p.use_smooth=True
 bpy.context.preferences.filepaths.save_version=0;bpy.ops.wm.save_as_mainfile(filepath=str(C/source),compress=True)
 for o in parts:print(bx_export.export(str(A/(o.name+'.fbx')),'unity',[o]),flush=True)
print('SOURCE_SMOOTH_FLAGS_RETAINED',flush=True)
