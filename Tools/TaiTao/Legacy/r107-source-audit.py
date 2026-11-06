import bpy,json
from pathlib import Path
print('BLENDER_VERSION',bpy.app.version_string)
p=Path('D:/ConRoi/Source/Blender/JourneyV48/R101/Terrain_R101_Editable.blend')
bpy.ops.wm.open_mainfile(filepath=str(p))
rows=[]
for o in bpy.data.objects:
 if o.type=='MESH' and (str(o.get('unity_path','')) in ['R20_Whole_Source_Export/Surface_176','R20_Whole_Source_Export/Surface_104','LakeGateMarketV32/Context_109','LakeGateMarketV32/Context_124']):
  rows.append(dict(name=o.name,props={k:str(o[k]) for k in o.keys()},verts=len(o.data.vertices),faces=len(o.data.polygons),scale=list(o.scale),position=list(o.location),collections=[c.name for c in o.users_collection]))
Path('D:/ConRoi/Test/KetQua/investigation_r107/blender-source-audit.json').write_text(json.dumps(rows,indent=2))
print(rows)

