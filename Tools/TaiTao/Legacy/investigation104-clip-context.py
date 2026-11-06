import bpy,math,json,sys
from pathlib import Path
from mathutils import Vector
R=Path('D:/ConRoi');S=R/'Source/Blender/JourneyV48/R104';D=R/'Test/KetQua/investigation_r104';E=R/'Test/BangChung/investigation_r104'
bpy.ops.wm.open_mainfile(filepath=str(S/'Repair_R12_R104_Animation.blend'),load_ui=False,use_scripts=False)
def P(v):return Vector((-v[0],-v[2],v[1]))
for n in ['R12_Rig','R12_Body']:
 if n in bpy.data.objects:bpy.data.objects.remove(bpy.data.objects[n],do_unlink=True)
with bpy.data.libraries.load(str(S/'Puppet_R104_Editable.blend'),link=False) as (src,dst):dst.objects=[n for n in src.objects if n=='Puppet_Rig' or n=='Body' or n.startswith('Joint_seam_')]
col=bpy.data.collections.new('R104_CURRENT_R12_CONTEXT');bpy.context.scene.collection.children.link(col)
for o in dst.objects:
 if o:col.objects.link(o);o.hide_render=False;o.hide_set(False)
rig=next(o for o in dst.objects if o and o.type=='ARMATURE');rig.animation_data_clear();rig.location=P((.35,3.4,1.1));rig.scale=(.7,)*3;rig.rotation_euler=(0,0,math.pi)
action=bpy.data.actions.get('RepairBothHands_R104')
if action:
 rig.animation_data_create();rig.animation_data.action=action
 if action.slots:rig.animation_data.action_slot=action.slots[0]
support=bpy.data.collections['R104_R12_REPAIR_SUPPORT']
for o in support.objects:o.location+=P((.35,3,1.1));o.hide_render=False
material=bpy.data.materials.new('R104_suspension_cord');material.diffuse_color=(.68,.59,.39,1)
def line(name,points):
 curve=bpy.data.curves.new(name,'CURVE');curve.dimensions='3D';curve.bevel_depth=.004;poly=curve.splines.new('POLY');poly.points.add(len(points)-1)
 for p,v in zip(poly.points,points):p.co=(*v,1)
 o=bpy.data.objects.new(name,curve);col.objects.link(o);curve.materials.append(material);return o
bpy.context.scene.frame_set(1);bpy.context.view_layer.update()
for name,x in [('head',0),('upperL',.35),('upperR',-.35)]:
 pb=rig.pose.bones[name];line('R12_suspension_'+name,[P((.35+x,5.1,1.45)),rig.matrix_world@pb.head])
thread=bpy.data.objects.get('R12_Hand_RepairString');tu=bpy.data.objects['Tu_Event_Rig']
if thread:
 thread.animation_data_clear()
 for f in range(1,362):
  bpy.context.scene.frame_set(f);bpy.context.view_layer.update();end=rig.matrix_world@rig.pose.bones['handR'].head;start=tu.matrix_world@tu.pose.bones['handR'].head if f<145 else end-P((0,.25,0))
  for i,p in enumerate(thread.data.splines[0].points):p.co=(*start.lerp(end,i/2),1);p.keyframe_insert(data_path='co',frame=f)
scene=bpy.context.scene;camera=scene.camera;camera.location=P((2.65,4.75,-1.10));camera.rotation_euler=(P((.82,4.10,1.52))-camera.location).to_track_quat('-Z','Y').to_euler();camera.data.lens=43
scene.render.engine='BLENDER_WORKBENCH';scene.render.resolution_x=1280;scene.render.resolution_y=800;scene.render.resolution_percentage=100
for t in [0,3.8,4.2,4.85,6.0]:scene.frame_set(round(t*30)+1);scene.render.filepath=str(E/('clip-source-final-'+str(t)+'.png'));bpy.ops.render.render(write_still=True)
scene.frame_set(1);bpy.ops.wm.save_as_mainfile(filepath=str(S/'Repair_R12_R104_Animation.blend'));bpy.ops.wm.open_mainfile(filepath=str(S/'Repair_R12_R104_Animation.blend'),load_ui=False,use_scripts=False)
(D/'clip-context.json').write_text(json.dumps(dict(source=str(S/'Repair_R12_R104_Animation.blend'),reopened=True,puppet_source='Puppet_R104_Editable.blend',support_export_local_offset_unity=[.35,3,1.1],continuous_camera=True,angles=[0,3.8,4.2,4.85,6.0],limits='Editable illustrative animation, not final acting or independent physics proof'),indent=2));print('Clip source synchronized to R104 puppet/support; saved/reopened')
