import bpy,json,numpy as np
from pathlib import Path
from mathutils import Vector
from bpy_extras.object_utils import world_to_camera_view
R=Path('D:/ConRoi');P=R/'Source/Blender/JourneyV48/R99/OngDao_Dam_R99_SectionCandidate.blend'
bpy.ops.wm.open_mainfile(filepath=str(P));sc=bpy.context.scene
A=np.array([637.7344,528.2603]);B=np.array([681.4578,491.2729]);L=float(np.linalg.norm(B-A));T=(B-A)/L;N=np.array([T[1],-T[0]])
def pos(s,d,h):
 x,z=A+T*s+N*d;return Vector((-x,-z,h))
cam=sc.camera.copy();cam.data=sc.camera.data.copy();sc.collection.objects.link(cam);sc.camera=cam;cam.data.lens=28
cam.location=pos(-7,8,2);cam.rotation_euler=(pos(-1,2,1.4)-cam.location).to_track_quat('-Z','Y').to_euler();bpy.context.view_layer.update()
rows=[]
for o in sc.objects:
 if o.type!='MESH' or o.hide_render:continue
 verts=[]
 for v in o.data.vertices:
  p=o.matrix_world@v.co;q=world_to_camera_view(sc,cam,p);px=q.x*1280;py=(1-q.y)*720
  if 690<px<735 and 635<py<675:
   xz=np.array([-p.x,-p.y]);sd=(xz-A)@np.c_[T,N];verts.append(dict(index=v.index,p=list(p),pixel=[px,py],sd=sd.tolist()))
 if verts:rows.append(dict(object=o.name,vertices=verts))
(R/'Test/KetQua/journey_v48/dam_r99/visible-seam-inspect.json').write_text(json.dumps(rows,indent=2));print('RESULT',[(x['object'],len(x['vertices'])) for x in rows],flush=True)
