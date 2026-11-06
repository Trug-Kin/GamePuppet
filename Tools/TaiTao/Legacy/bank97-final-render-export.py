import bpy,json,numpy as np
from pathlib import Path
from mathutils import Vector
R=Path('D:/ConRoi');M=R/'Test/BangChung/journey_v48/bank_r97'
bpy.ops.wm.open_mainfile(filepath=str(R/'Source/Blender/JourneyV48/R97/Bridgehead_Bank_R97_Editable.blend'));sc=bpy.context.scene
s=(R/'Test/Tam/bank97-review-source.py').read_text();exec(s[s.index('sc.render.resolution_x='):s.index('bpy.context.preferences.filepaths.save_version=')])
exec((R/'Test/Tam/bank97-export.py').read_text())
