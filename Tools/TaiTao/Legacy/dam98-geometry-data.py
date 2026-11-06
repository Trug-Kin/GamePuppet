import bpy,numpy as np,json
from pathlib import Path
R=Path('D:/ConRoi');D=R/'Test/KetQua/journey_v48/dam_r98';bpy.ops.wm.open_mainfile(filepath=str(R/'Source/Blender/JourneyV48/R97/Bridgehead_Bank_R97_Editable.blend'))
out=[]
for name in ['C95_0040_OD35_Surface_0','C95_0041_OD35_Surface_1','C95_0042_OD35_Surface_2','C95_0043_OD35_Surface_3','C95_0045_OD35_Surface_5','C95_0051_R11_Bridge_Deck_Fascia','Grade_012']:
 o=bpy.data.objects[name];o.data.calc_loop_triangles();m=np.array(o.matrix_world);v=np.array([x.co for x in o.data.vertices],dtype=float)@m[:3,:3].T+m[:3,3];u=np.c_[-v[:,0],v[:,2],-v[:,1]];t=np.array([tuple(p.vertices) for p in o.data.loop_triangles]);np.savez_compressed(D/(name+'.npz'),v=u,tri=t);out.append(dict(name=name,unique_y=sorted(set(np.round(u[:,1],4))),verts=u.tolist() if len(u)<30 else None,faces=[list(p.vertices) for p in o.data.polygons] if len(u)<30 else None))
(D/'bridge-datums.json').write_text(json.dumps(out,indent=2));print('RESULT data exported')
