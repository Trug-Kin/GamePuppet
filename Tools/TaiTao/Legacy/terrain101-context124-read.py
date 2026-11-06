import bpy,json
from pathlib import Path
R=Path('D:/ConRoi');D=R/'Test/KetQua/journey_v48/terrain_r101';bpy.ops.wm.open_mainfile(filepath=str(R/'Source/Blender/JourneyV48/R101/Terrain_R101_Editable.blend'));rows=[]
for o in bpy.data.objects:
 if o.type=='MESH' and ('Context_124' in str(o.get('unity_path','')) or o.get('market95_export_key')=='M95_016'):
  rows.append(dict(name=o.name,props={k:o[k] for k in o.keys()},vertices=len(o.data.vertices)))
(D/'context124-source.json').write_text(json.dumps(rows,indent=2));print('RESULT',json.dumps(rows),flush=True)
