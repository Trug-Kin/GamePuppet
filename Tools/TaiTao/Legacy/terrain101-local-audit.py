import bpy,json,sys,numpy as np
from pathlib import Path
from mathutils import Vector
R=Path('D:/ConRoi');D=R/'Test/KetQua/journey_v48/terrain_r101'
sys.path.insert(0,str(R/'.agents/skills/scenario-blender-expert/scripts'));import bx_audit
bpy.ops.wm.open_mainfile(filepath=str(R/'Source/Blender/JourneyV48/R101/Terrain_R101_Editable.blend'))
o=bpy.data.objects['BankR91_Grade_017'];v=[o.matrix_world@p.co for p in o.data.vertices];selected=[]
for p in o.data.polygons:
 c=sum((v[i] for i in p.vertices),Vector())/len(p.vertices)
 if 640<=-c.x<=658 and 411<=-c.y<=418:selected.append(p)
ids=sorted({i for p in selected for i in p.vertices});lookup={i:j for j,i in enumerate(ids)};center=sum((v[i] for i in ids),Vector())/len(ids)
m=bpy.data.meshes.new('LocalActualContext');m.from_pydata([v[i]-center for i in ids],[],[[lookup[i] for i in p.vertices] for p in selected]);m.update();obj=bpy.data.objects.new('R101_Actual_Continuous_Context',m);bpy.context.scene.collection.objects.link(obj);obj.location=center
gate=bx_audit.deliverable([obj],profile='game');(D/'context-delivery-gate.json').write_text(json.dumps(gate,indent=2,default=str));print('RESULT',json.dumps(gate),flush=True)
