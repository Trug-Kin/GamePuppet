import bpy,json,numpy as np
from pathlib import Path
R=Path('D:/ConRoi');D=R/'Test/KetQua/journey_v48/terrain_r101'
bpy.ops.wm.open_mainfile(filepath=str(R/'Source/Blender/JourneyV48/R100/OngDao_Dam_R100_Editable.blend'))
o=bpy.data.objects['BankR91_Grade_017'];v=np.array([o.matrix_world@p.co for p in o.data.vertices]);faces=[tuple(p.vertices) for p in o.data.polygons];cent=np.array([np.mean(v[list(f)],axis=0) for f in faces]);normal=np.array([p.normal[:] for p in o.data.polygons]);sel=(cent[:,0]<-640)&(cent[:,0]>-650)&(cent[:,1]<-409)&(cent[:,1]>-418)
out=[]
for z in np.arange(409,418,.5):
 mask=sel&(abs(cent[:,1]+z)<.25);neg=mask&(normal[:,2]<-.01)
 out.append(dict(z=float(z),faces=int(mask.sum()),negative=int(neg.sum()),height_range=[float(cent[mask,2].min()),float(cent[mask,2].max())] if mask.any() else None))
np.savez_compressed(D/'surface142-before.npz',v=v,faces=np.array(faces),normals=normal)
(D/'local-winding-profile.json').write_text(json.dumps(out,indent=2));print('RESULT',json.dumps(out),flush=True)
