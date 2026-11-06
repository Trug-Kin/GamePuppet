import bpy,json
from pathlib import Path
from mathutils import Vector
R=Path('D:/ConRoi')
bpy.ops.wm.open_mainfile(filepath=str(R/'Source/Blender/JourneyV48/R93/Langfarm_R93_Editable.blend'))
out={}
for o in bpy.data.collections['R93_BUILDING_EDITABLE'].all_objects:
 if o.type=='MESH' and ('EntryStep' in o.name or 'RecessBack' in o.name):
  vv=[o.matrix_world@v.co for v in o.data.vertices]
  out[o.name]={'min':[min(v[i] for v in vv) for i in range(3)],'max':[max(v[i] for v in vv) for i in range(3)]}
(R/'Test/KetQua/journey_v48/r93/step-source-bounds.json').write_text(json.dumps(out,indent=2))
print('RESULT',json.dumps(out));print(bpy.data.texts.get('READ_ME_R93_STATUS').as_string())
