import bpy,json,numpy as np
from pathlib import Path
from mathutils import Vector
from mathutils.bvhtree import BVHTree
R=Path('D:/ConRoi');D=R/'Test/KetQua/journey_v48/dam_r99';M=R/'Test/BangChung/journey_v48/dam_r99'
P=R/'Source/Blender/JourneyV48/R99/OngDao_Dam_R99_SectionCandidate.blend'
bpy.ops.wm.open_mainfile(filepath=str(P));sc=bpy.context.scene
A=np.array([637.7344,528.2603]);B=np.array([681.4578,491.2729]);L=float(np.linalg.norm(B-A));T=(B-A)/L;N=np.array([T[1],-T[0]])
def pos(s,d,h):
 x,z=A+T*s+N*d;return Vector((-x,-z,h))
cam=sc.camera.copy();cam.data=sc.camera.data.copy();sc.collection.objects.link(cam);sc.camera=cam;cam.data.type='PERSP';cam.data.lens=28
for tag,eye,at in [('bridge-landward',(L/2,-30,14),(L/2,-4,2)),('far-approach',(-23,-8,4),(-1,-5,2.8)),('gate-approach',(L+24,-6,5),(L+1,-5,2.8)),('gate-bank-return',(L+9,9,2),(L+1,3,1)),('far-bank-return',(-12,7,2),(-2,3,1))]:
 cam.location=pos(*eye);cam.rotation_euler=(pos(*at)-cam.location).to_track_quat('-Z','Y').to_euler();sc.render.filepath=str(M/('context-'+tag+'.png'));bpy.ops.render.render(write_still=True)
