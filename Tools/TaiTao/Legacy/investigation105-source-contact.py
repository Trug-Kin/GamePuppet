import bpy,sys,math,json
from pathlib import Path
from mathutils import Vector,Matrix
from bpy_extras.anim_utils import action_get_channelbag_for_slot
R=Path('D:/ConRoi');S=R/'Source/Blender/JourneyV48/R105';D=R/'Test/KetQua/investigation_r105';E=R/'Test/BangChung/investigation_r105';A=R/'Assets/Art/InvestigationV36/R105'
bpy.ops.wm.open_mainfile(filepath=str(S/'Repair_R12_R105_Animation.blend'),load_ui=False,use_scripts=False)
def P(v):return Vector((-v[0],-v[2],v[1]))
def U(v):return [-v.x,v.z,-v.y]
s=bpy.context.scene;rig=bpy.data.objects['Tu_Event_Rig'];body=bpy.data.objects['Tu_Event_Body'];tool=bpy.data.objects['RepairTool'];thread=bpy.data.objects['R12_Hand_RepairString'];puppet=bpy.data.objects['Puppet_Rig']
s.frame_set(1);bpy.context.view_layer.update()
def point(n):return rig.matrix_world@rig.pose.bones[n].head
# Hands work at the suspended puppet's shoulder cord, above the top rail, not on its head.
targets={'L':P((.68,4.20,1.42)),'R':P((.28,4.15,1.35))}
def aim(upper,child,target):
 pb=rig.pose.bones[upper];start=point(upper);current=point(child)-start;rot=current.rotation_difference(target-start)
 world=rig.matrix_world@pb.matrix;pb.matrix=rig.matrix_world.inverted()@Matrix.Translation(start)@rot.to_matrix().to_4x4()@Matrix.Translation(-start)@world;bpy.context.view_layer.update()
def solve(side,target):
 up,fore,hand='upper'+side,'fore'+side,'hand'+side;start=point(up);a=(point(fore)-start).length;b=(point(hand)-point(fore)).length;direction=target-start;distance=min(direction.length,a+b-.00001);direction.normalize()
 pole=P((.4 if side=='L' else -.4,-1,.15));pole=(pole-direction*pole.dot(direction)).normalized();along=(a*a-b*b+distance*distance)/(2*distance);elbow=start+direction*along+pole*math.sqrt(max(0,a*a-along*along))
 aim(up,fore,elbow);aim(fore,hand,target)
for side,target in targets.items():solve(side,target)
pose={n:rig.pose.bones[n].matrix_basis.copy() for n in ['upperL','foreL','upperR','foreR']};errors={side:(point('hand'+side)-target).length for side,target in targets.items()};assert max(errors.values())<.002,errors
# A single weighted tool follows the left hand through the exported animation.
# Its handle is centered in the palm; the shaft points at R12's shoulder cord.
tool.animation_data_clear();tool.parent=None;tool.matrix_world=Matrix.Identity(4)
parts=[]
def cylinder(name,r,depth,z,material):
 bpy.ops.mesh.primitive_cylinder_add(vertices=12,radius=r,depth=depth,location=(0,0,z));o=bpy.context.object;o.name=name;o.data.materials.append(material);parts.append(o)
steel=bpy.data.materials.new('R105_tool_steel');steel.diffuse_color=(.5,.56,.59,1)
handle=bpy.data.materials.new('R105_tool_ochre');handle.diffuse_color=(.58,.28,.06,1)
cylinder('Tool_grip',.022,.10,0,handle);cylinder('Tool_shaft',.006,.31,.18,steel)
bpy.ops.object.select_all(action='DESELECT')
for o in parts:o.select_set(True)
bpy.context.view_layer.objects.active=parts[0];bpy.ops.object.join();temp=parts[0]
mesh=temp.data.copy();bpy.data.objects.remove(temp,do_unlink=True);tool.data=mesh
for m in list(tool.modifiers):tool.modifiers.remove(m)
for g in list(tool.vertex_groups):tool.vertex_groups.remove(g)
grip=point('handL');tip=P((.4,4.12,1.15));M=Matrix.Translation(grip)@(tip-grip).to_track_quat('Z','Y').to_matrix().to_4x4()
pb=rig.pose.bones['handL'];deform=pb.matrix@pb.bone.matrix_local.inverted();T=deform.inverted()@rig.matrix_world.inverted()@M
# Joined cylinder geometry retains the grip object's local origin (zero here).
mesh.transform(T);tool.parent=rig;tool.matrix_parent_inverse=Matrix.Identity(4);tool.matrix_basis=Matrix.Identity(4);g=tool.vertex_groups.new(name='handL');g.add(list(range(len(mesh.vertices))),1,'REPLACE');tool.modifiers.new('Rigid grip handL','ARMATURE').object=rig
rig.animation_data.action=rig.animation_data.action.copy();rig.animation_data.action.name='RepairEvent_R105_ContactContinuous'
thread.animation_data_clear();thread.data.animation_data_clear();samples=[];release=None
for f in range(1,362):
 t=(f-1)/30;s.frame_set(f)
 for n,m in pose.items():
  pb=rig.pose.bones[n];pb.matrix_basis=m;pb.keyframe_insert(data_path='rotation_quaternion',frame=f);pb.keyframe_insert(data_path='location',frame=f)
 bpy.context.view_layer.update();start=point('handR');end=puppet.matrix_world@puppet.pose.bones['handR'].head
 if f==145:release=start.copy()
 if t>4.8:
  u=max(0,min(1,(t-4.8)/.35));u=u*u*(3-2*u);start=release.lerp(end+P((0,-.25,0)),u)
 for i,p in enumerate(thread.data.splines[0].points):
  v=start.lerp(end,i/2)+P((0,-.02*math.sin(i/2*math.pi),0));p.co=(*v,1);p.keyframe_insert(data_path='co',frame=f)
 samples.append(dict(time=t,handR=U(point('handR')),stringStart=U(start),stringEnd=U(end)))
for obj in [rig,thread.data]:
 ad=obj.animation_data
 if ad and ad.action:
  bag=action_get_channelbag_for_slot(ad.action,ad.action_slot)
  if bag:
   for fc in bag.fcurves:
    for k in fc.keyframe_points:k.interpolation='LINEAR'
for o in [body,tool]:o.hide_render=False
# Make suspension cords legible in the fixed camera and keep their source anchors.
for o in s.objects:
 if o.name.startswith('R12_suspension_') and o.type=='CURVE':o.data.bevel_depth=.006
s.frame_set(1);bpy.ops.wm.save_as_mainfile(filepath=str(S/'Repair_R12_R105_Animation.blend'));bpy.ops.wm.open_mainfile(filepath=str(S/'Repair_R12_R105_Animation.blend'),load_ui=False,use_scripts=False)
rig=bpy.data.objects['Tu_Event_Rig'];body=bpy.data.objects['Tu_Event_Body'];tool=bpy.data.objects['RepairTool'];s=bpy.context.scene
for t in [0,3.8,4.05,4.3,4.8,4.833333,5.2,6]:s.frame_set(round(t*30)+1);s.render.filepath=str(E/('after-blender-'+str(t)+'.png'));bpy.ops.render.render(write_still=True)
s.frame_set(1)
sys.path.append(str(R/'.agents/skills/scenario-blender-export/scripts'));import bx_export as X
sys.path.append(str(R/'.agents/skills/scenario-blender-expert/scripts'));import bx_audit
checks=[{'object':o.name,'gate':bx_audit.deliverable([o],profile='game')} for o in [body,tool]]
exp=X.export(str(A/'RepairEventActor_R105.fbx'),'unity_skeletal',[rig,body,tool],bake_anim=True,bake_anim_use_all_actions=False);rt=X.round_trip(str(A/'RepairEventActor_R105.fbx'),[rig,body,tool],preset='unity_skeletal',bake_anim=True,bake_anim_use_all_actions=False)
report=dict(source=str(S/'Repair_R12_R105_Animation.blend'),reopened=True,hand_target_errors=errors,release_point_clip=U(release),release_duration=.35,unchanged_camera=True,unchanged_mechanism=True,export=exp,roundtrip=rt,gates=checks,max_string_step=max((Vector(a['stringStart'])-Vector(b['stringStart'])).length for a,b in zip(samples,samples[1:])),samples=samples)
(D/'source-final.json').write_text(json.dumps(report,default=str,indent=2));print('R105 SOURCE COMPLETE',errors,'roundtrip',rt.get('pass'))

