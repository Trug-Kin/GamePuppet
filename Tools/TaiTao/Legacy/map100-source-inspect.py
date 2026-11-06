import bpy,json,numpy as np
from pathlib import Path
R=Path('D:/ConRoi');D=R/'Test/KetQua/journey_v48/map_r99_closeout';D.mkdir(exist_ok=True)
bpy.ops.wm.open_mainfile(filepath=str(R/'Source/Blender/JourneyV48/R99/OngDao_Dam_R99_SectionCandidate.blend'))
rows=[]
for o in bpy.context.scene.objects:
 if o.type!='MESH' or not o.get('dam99_binding_path'):continue
 if not any(x in o.name for x in ['OD35_Surface','FlowerModule','Light','Lamp','Planter']):continue
 v=np.array([o.matrix_world@p.co for p in o.data.vertices]);rows.append(dict(object=o.name,path=o['dam99_binding_path'],vertices=len(v),min=v.min(0).tolist(),max=v.max(0).tolist(),render=o.get('dam99_actual_renderer'),collider=o.get('dam99_actual_collider'),materials=[m.name if m else None for m in o.data.materials]))
(D/'source-object-index.json').write_text(json.dumps(rows,indent=2));print('RESULT',json.dumps(rows,indent=2),flush=True)
