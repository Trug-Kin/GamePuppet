import bpy,json,numpy as np
from pathlib import Path
R=Path('D:/ConRoi');D=R/'Test/KetQua/journey_v48/bank_r97';P=R/'Source/Blender/JourneyV48/R97/Bridgehead_Bank_R97_Editable.blend'
bpy.ops.wm.open_mainfile(filepath=str(P));o=next(o for o in bpy.data.objects if o.get('market95_export_key')=='M95_022');out=[];before=np.array([v.co for v in o.data.vertices],dtype=float)
for p in o.data.polygons:
 c=o.matrix_world@p.center;x,z=-c.x,-c.y
 if 640<=x<=700 and 411<=z<=492 and p.normal.z<-.7:p.flip();out.append(dict(face=p.index,x=float(x),z=float(z),area=float(p.area)))
o.data.update();assert np.array_equal(before,np.array([v.co for v in o.data.vertices],dtype=float));(D/'endpoint-winding-cleanup.json').write_text(json.dumps(dict(object=o.name,scope='M95_022 only, X640..700/Z411..492; downward near-horizontal faces, no position changes',faces=len(out),rows=out),indent=2));bpy.context.preferences.filepaths.save_version=0;bpy.ops.wm.save_as_mainfile(filepath=str(P),compress=True);print('RESULT flipped',len(out),flush=True)
