import bpy,json,numpy as np
from pathlib import Path
bpy.ops.wm.open_mainfile(filepath='D:/ConRoi/Docs/Reviews/journey_v48/r87/gate-clock-r87-source.blend')
for o in bpy.data.objects:
 if 'ContactFoot' in o.name or 'UpperCap' in o.name:
  v=np.array([o.matrix_world@p.co for p in o.data.vertices]);print(o.name,'matrix',list(o.location),'bounds',v.min(0),v.max(0),'verts',v.tolist(),flush=True)
