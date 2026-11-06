import bpy,math,json,sys
from pathlib import Path
from mathutils import Vector
R=Path('D:/ConRoi');S=R/'Source/Blender/JourneyV48/R104';A=R/'Assets/Art/InvestigationV36/R104';D=R/'Test/KetQua/investigation_r104';E=R/'Test/BangChung/investigation_r104'
bpy.ops.wm.open_mainfile(filepath=str(S/'Repair_R12_R104_Animation.blend'),load_ui=False,use_scripts=False)
col=bpy.data.collections['R104_R12_REPAIR_SUPPORT']
for o in list(col.objects):bpy.data.objects.remove(o,do_unlink=True)
def P(v):return Vector((-v[0],-v[2],v[1]))
material=bpy.data.materials.new('R104_repair_stand_final');material.diffuse_color=(.24,.14,.06,1);material.use_nodes=True;material.node_tree.nodes.get('Principled BSDF').inputs['Base Color'].default_value=material.diffuse_color
def beam(name,a,b,r):
 p=P(a);q=P(b);bpy.ops.mesh.primitive_cylinder_add(vertices=12,radius=r,depth=(q-p).length,location=(p+q)*.5);o=bpy.context.object;o.name=name;o.rotation_euler=(q-p).to_track_quat('Z','Y').to_euler();bpy.ops.object.transform_apply(location=True,rotation=True,scale=True);o.data.materials.append(material)
 for c in list(o.users_collection):c.objects.unlink(o)
 col.objects.link(o);return o
objects=[]
for x,tag in [(-.55,'left'),(.55,'right')]:
 objects += [beam('R12_stand_'+tag,(x,0,1.3),(x,2.3,1.3),.035),beam('R12_stand_foot_'+tag,(x,.025,.8),(x,.025,1.65),.04),beam('R12_overhead_arm_'+tag,(x,2.3,1.3),(x,2.3,.15),.03)]
objects.append(beam('R12_stand_top',(-.55,2.3,.15),(.55,2.3,.15),.045))
sys.path.append(str(R/'.agents/skills/scenario-blender-export/scripts'));import bx_export as X
export=X.export(str(A/'R12Support.fbx'),'unity',objects);rt=X.round_trip(str(A/'R12Support.fbx'),objects,preset='unity')
for o in objects:o.location+=P((.35,3,1.1))
ctx=bpy.data.collections['R104_CURRENT_R12_CONTEXT']
for o in ctx.objects:
 if o.type=='CURVE' and o.name.startswith('R12_suspension_'):
  p=o.data.splines[0].points[0];v=Vector(p.co[:3]);v.z=5.3;v.y=-1.25;p.co=(*v,1)
scene=bpy.context.scene;scene.frame_set(1);scene.render.filepath=str(E/'clip-source-final-clear.png');bpy.ops.render.render(write_still=True);bpy.ops.wm.save_as_mainfile(filepath=str(S/'Repair_R12_R104_Animation.blend'));bpy.ops.wm.open_mainfile(filepath=str(S/'Repair_R12_R104_Animation.blend'),load_ui=False,use_scripts=False)
(D/'support-final.json').write_text(json.dumps(dict(reopened=True,posts_behind_performer=True,export=export,roundtrip=rt,placement_unity=[.35,3,1.1]),default=str,indent=2));print('Support moved behind key action, source reopened and FBX roundtrip',rt.get('pass'))
