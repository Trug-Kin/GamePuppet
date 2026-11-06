import bpy
from pathlib import Path
R=Path('D:/ConRoi');bpy.ops.wm.open_mainfile(filepath=str(R/'Source/Blender/JourneyV48/R104/Repair_R12_R104_Animation.blend'),load_ui=False,use_scripts=False)
s=bpy.context.scene
for t in [0,3.8,4.2,4.85,6.0]:
 s.frame_set(round(t*30)+1);s.render.filepath=str(R/('Test/BangChung/investigation_r104/clip-source-final-'+str(t)+'.png'));bpy.ops.render.render(write_still=True)
print('Final saved source rendered at five event times; source not modified')
