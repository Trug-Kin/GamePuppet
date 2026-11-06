import bpy,json,numpy as np
from pathlib import Path
from mathutils import Vector
from mathutils.bvhtree import BVHTree
R=Path('D:/ConRoi');D=R/'Test/KetQua/journey_v48/dam_r98';M=R/'Test/BangChung/journey_v48/dam_r98';info=json.loads((D/'author-result.json').read_text());A=np.array(info['basis']['A']);T=np.array(info['basis']['T']);N=np.array(info['basis']['N']);L=info['basis']['L'];rows=[]
for rev,path in [('before','R97/Bridgehead_Bank_R97_Editable.blend'),('after','R98/OngDao_Dam_R98_Editable.blend')]:
 bpy.ops.wm.open_mainfile(filepath=str(R/'Source/Blender/JourneyV48'/path));trees={}
 names=['BankR91_Grade_017','BankR91_Context125_R91','C95_0040_OD35_Surface_0','R97_LOWER_WALK','R97_LOWER_GRASS']
 if rev=='after':names+=['R98_CONCRETE_DAM_13_BAYS','R98_FAR_END_STAIR_AND_LOW_LANDING','R98_GATE_END_STAIR_AND_LOW_LANDING']
 for name in names:
  o=bpy.data.objects[name];mm=np.array(o.matrix_world,dtype=float);v=np.array([p.co for p in o.data.vertices],dtype=float)@mm[:3,:3].T+mm[:3,3];trees[name]=BVHTree.FromPolygons([Vector(p) for p in v],[tuple(p.vertices) for p in o.data.polygons])
 for s in [-1.5,10.,25.,40.,L+1.5]:
  samples=[]
  for d in np.linspace(-1,13,281):
   x,z=A+T*s+N*d;layers={}
   for name,tree in trees.items():
    p,n,i,dist=tree.ray_cast(Vector((-x,-z,20)),Vector((0,0,-1)),40)
    if p:layers[name]=float(p.z)
   samples.append(dict(d=float(d),layers=layers))
  rows.append(dict(rev=rev,s=s,samples=samples))
 if rev=='after':
  sc=bpy.context.scene;sc.render.resolution_x=1280;sc.render.resolution_y=800
  for tag,eye,at in [('wide',[628,8,491],[657,1,515]),('oblique',[663,2.2,489],[649,1,520]),('gate-stair',[678,3,480],[682,.6,488]),('far-stair',[629,3,525],[637,.6,530])]:
   sc.camera.location=(-eye[0],-eye[2],eye[1]);sc.camera.rotation_euler=(Vector((-at[0],-at[2],at[1]))-sc.camera.location).to_track_quat('-Z','Y').to_euler();sc.render.filepath=str(M/('blender-'+tag+'-final.png'));bpy.ops.render.render(write_still=True)
(D/'sections-before-after.json').write_text(json.dumps(rows,indent=2));print('RESULT same-position sections and final reopened source renders',flush=True)
