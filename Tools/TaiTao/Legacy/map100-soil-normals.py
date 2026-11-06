import bpy,json,numpy as np
from pathlib import Path
R=Path('D:/ConRoi');D=R/'Test/KetQua/journey_v48/map_r99_closeout'
bpy.ops.wm.open_mainfile(filepath=str(R/'Source/Blender/JourneyV48/R99/OngDao_Dam_R99_SectionCandidate.blend'))
A=np.array([637.7344,528.2603]);T=np.array([.76346655,-.64584737]);N=np.array([-.64584737,-.76346655]);out=[]
for name in ['C95_0041_OD35_Surface_1','C95_0045_OD35_Surface_5']:
 o=bpy.data.objects[name];o.data.calc_loop_triangles();faces=[]
 for p in o.data.polygons:
  q=o.matrix_world@p.center;s,d=(-np.array(q[:2])-A)@np.c_[T,N]
  if 57<s<70 and -3<d<7:faces.append(dict(face=p.index,sd=[s,d],height=q.z,normal=list(p.normal),verts=len(p.vertices),triangles=sum(t.polygon_index==p.index for t in o.data.loop_triangles)))
 out.append(dict(object=name,faces=faces))
(D/'gate-soil-normals.json').write_text(json.dumps(out,indent=2));print('RESULT',json.dumps(out),flush=True)
