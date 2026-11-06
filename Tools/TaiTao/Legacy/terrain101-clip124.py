import bpy,json,numpy as np
from pathlib import Path
R=Path('D:/ConRoi');D=R/'Test/KetQua/journey_v48/terrain_r101';P=R/'Source/Blender/JourneyV48/R101/Terrain_R101_Editable.blend';assert json.loads((D/'context124-delta.json').read_text())==[];bpy.ops.wm.open_mainfile(filepath=str(P))
code=(R/'Test/Tam/bank97-author.py').read_text();exec(code[code.index('def half('):code.index('cut=[]')]);rows=[]
code=(R/'Test/Tam/terrain101-close-joins.py').read_text();start=code.index("o=bpy.data.objects['M95_Service3_NATIVE']");s=code[start:code.index("(D/'join-delta.json')",start)];assert 'rows.append' in s
s=s.replace("M95_Service3_NATIVE","TerrainColliderJoin_R90").replace('Context107','Context124').replace('Context_107','Context_124')
s=s.replace(' pieces=difference(arr,q)',' points=np.array([a[:3] for a in arr]);outside=points[:,0].max()<640 or points[:,0].min()>658 or points[:,2].max()<411 or points[:,2].min()>418\n pieces=[arr] if outside else difference(arr,q)')
exec(s);(D/'context124-delta.json').write_text(json.dumps(rows,indent=2));bpy.ops.wm.save_as_mainfile(filepath=str(P));bpy.ops.wm.read_factory_settings(use_empty=True);bpy.ops.wm.open_mainfile(filepath=str(P));print('RESULT',json.dumps(rows),flush=True)
