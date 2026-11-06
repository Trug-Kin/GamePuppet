import bpy,json,numpy as np
from pathlib import Path
R=Path('D:/ConRoi');D=R/'Test/KetQua/journey_v48/bank_r97';D.mkdir(parents=True,exist_ok=True)
bpy.ops.wm.open_mainfile(filepath=str(R/'Source/Blender/JourneyV48/R96/Market_Ground_R96_Editable.blend'))
out=[]
for o in list(bpy.context.scene.objects):
 if o.type!='MESH':continue
 a=np.array([v.co for v in o.data.vertices],dtype=float);m=np.array(o.matrix_world);v=a@m[:3,:3].T+m[:3,3];u=np.c_[-v[:,0],v[:,2],-v[:,1]];lo=u.min(0);hi=u.max(0)
 if lo[0]<760 and hi[0]>620 and lo[2]<545 and hi[2]>380:
  out.append(dict(name=o.name,collections=[c.name for c in o.users_collection],lo=lo.tolist(),hi=hi.tolist(),faces=len(o.data.polygons),props={k:str(o[k]) for k in o.keys()},hidden=o.hide_render))
(D/'blender-existing-inventory.json').write_text(json.dumps(out,indent=2))
print('RESULT',len(out),flush=True)
