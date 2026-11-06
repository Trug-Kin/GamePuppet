import bpy,json
from pathlib import Path
R=Path('D:/ConRoi');D=R/'Test/KetQua/journey_v48/terrain_r101';bpy.ops.wm.open_mainfile(filepath=str(R/'Source/Blender/JourneyV48/R101/Terrain_R101_Editable.blend'))
rows=[]
for o in bpy.data.objects:
 if o.type!='MESH':continue
 if any('Context_107' in str(o.get(k,'')) for k in o.keys()) or o.name.startswith('R97_'):
  vv=[o.matrix_world@v.co for v in o.data.vertices];rows.append(dict(name=o.name,props={k:o[k] for k in o.keys()},vertices=len(o.data.vertices),aabb=[list(map(min,zip(*vv))),list(map(max,zip(*vv)))],bounds=[list(o.matrix_world@v.co) for v in o.data.vertices if abs((o.matrix_world@v.co).y+418)<.01][:10]))
(D/'join-source-index.json').write_text(json.dumps(rows,indent=2));print('RESULT',json.dumps(rows)[:13000],flush=True)
