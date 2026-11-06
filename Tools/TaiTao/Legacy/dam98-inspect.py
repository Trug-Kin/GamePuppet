import bpy,json,numpy as np
from pathlib import Path
R=Path('D:/ConRoi');D=R/'Test/KetQua/journey_v48/dam_r98';D.mkdir(parents=True,exist_ok=True)
bpy.ops.wm.open_mainfile(filepath=str(R/'Source/Blender/JourneyV48/R97/Bridgehead_Bank_R97_Editable.blend'))
out=[]
for o in list(bpy.context.scene.objects):
 if o.type!='MESH':continue
 m=np.array(o.matrix_world,dtype=float);v=np.array([p.co for p in o.data.vertices],dtype=float)
 if not len(v):continue
 v=v@m[:3,:3].T+m[:3,3];u=np.c_[-v[:,0],v[:,2],-v[:,1]];mn=u.min(0);mx=u.max(0)
 if mx[0]<620 or mn[0]>706 or mx[2]<478 or mn[2]>552:continue
 out.append(dict(name=o.name,path=o.get('unity_path',''),key=o.get('market95_export_key',''),min=mn.tolist(),max=mx.tolist(),vertices=len(v),faces=len(o.data.polygons),collections=[c.name for c in o.users_collection],hide_render=o.hide_render,props={k:str(o[k]) for k in o.keys()}))
(D/'blender-inventory.json').write_text(json.dumps(dict(version=bpy.app.version_string,source=bpy.data.filepath,rows=out),indent=2));print('RESULT inventory',len(out),flush=True)
