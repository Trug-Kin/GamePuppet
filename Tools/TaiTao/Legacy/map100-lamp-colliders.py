import bpy,numpy as np,json
from pathlib import Path
R=Path('D:/ConRoi');D=R/'Test/KetQua/journey_v48/map_r99_closeout';bpy.ops.wm.open_mainfile(filepath=str(R/'Source/Blender/JourneyV48/R99/OngDao_Dam_R99_SectionCandidate.blend'));out=[]
for o in bpy.context.scene.objects:
 if o.type!='MESH' or not o.get('dam99_actual_collider'):continue
 v=np.array([o.matrix_world@p.co for p in o.data.vertices]);mask=(np.linalg.norm(v[:,:2]-np.array([-683.53369,-490.63019]),axis=1)<.8)&(v[:,2]>2.9)
 if mask.any():out.append(dict(object=o.name,path=o.get('dam99_binding_path'),vertices=int(mask.sum()),min=v[mask].min(0).tolist(),max=v[mask].max(0).tolist()))
(D/'gate-lamp-collider-candidates.json').write_text(json.dumps(out,indent=2));print('RESULT',json.dumps(out),flush=True)
