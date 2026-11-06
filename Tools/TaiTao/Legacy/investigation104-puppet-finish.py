import bpy,sys,json,math,bmesh
from pathlib import Path
from mathutils import Vector,Matrix,Quaternion
R=Path('D:/ConRoi');S=R/'Source/Blender/JourneyV48/R104';D=R/'Test/KetQua/investigation_r104';E=R/'Test/BangChung/investigation_r104';A=R/'Assets/Art/InvestigationV36/R104'
# Reuse normalized/export/render functions only, not the author loop.
exec(compile((R/'Test/Tam/investigation104-author-art.py').read_text(encoding='utf-8').split('# Khai:')[0],'r104-art-helpers','exec'))
# Target directions from the established shared alphabet, not independent replacement poses.
bpy.ops.wm.open_mainfile(filepath=str(S/'Puppet_Poses_R104_Editable.blend'),load_ui=False,use_scripts=False)
old=next(o for o in bpy.context.scene.objects if o.type=='ARMATURE');targets={}
for action in list(bpy.data.actions):
 if not (action.name.startswith('Pose_') or action.name=='RepairBothHands'):continue
 old.animation_data_create();old.animation_data.action=action
 if action.slots:old.animation_data.action_slot=action.slots[0]
 bpy.context.scene.frame_set(1);bpy.context.view_layer.update();targets[action.name]={n:list((old.pose.bones[n].tail-old.pose.bones[n].head).normalized()) for n in ['upperL','foreL','upperR','foreR']}
bpy.ops.wm.open_mainfile(filepath=str(R/'Source/Downloads/Quaternius_R104/BaseCharacter.blend'),load_ui=False,use_scripts=False)
rig,meshes=normalized('Puppet',1.76)
mapping={'Head':'head','UpperArm.L':'upperL','LowerArm.L':'foreL','UpperArm.R':'upperR','LowerArm.R':'foreR','Hand.L':'handL','Hand.R':'handR','Wrist.L':'handL','Wrist.R':'handR','Fist.L':'handL','Fist.R':'handR'}
for a,b in mapping.items():
 if a in rig.data.bones:rig.data.bones[a].name=b
wood=mat('R104_carved_wood',(.43,.24,.095));ink=mat('R104_puppet_face',(.025,.022,.018));dark=mat('R104_joint_dark',(.09,.055,.022))
for o in meshes:
 for i,m in enumerate(o.data.materials):o.data.materials[i]=ink if any(t in m.name.lower() for t in ['face','eye']) else wood
pose_idle(rig) # Names now changed; set explicitly below.
for n,d in [('upperL',(.12,0,-1)),('foreL',(.05,-.1,-1)),('upperR',(-.12,0,-1)),('foreR',(-.05,-.1,-1))]:aim(rig,n,d)
base={pb.name:pb.matrix_basis.copy() for pb in rig.pose.bones}
# Joint seams are connected rings around the intact base, not detached torso sections.
for n in ['upperL','foreL','handL','upperR','foreR','handR']:
 if n not in rig.data.bones:continue
 b=rig.data.bones[n];center=b.head_local;direction=(b.tail_local-center).normalized();radius=.045 if n.startswith('hand') else .065
 bpy.ops.mesh.primitive_torus_add(major_segments=16,minor_segments=6,location=center,major_radius=radius,minor_radius=.007);o=bpy.context.object;o.name='Joint_seam_'+n;o.rotation_euler=Vector((0,0,1)).rotation_difference(direction).to_euler();bpy.ops.object.transform_apply(location=True,rotation=True,scale=True);o.data.materials.append(dark);o.parent=rig;vg=o.vertex_groups.new(name=n);vg.add(list(range(len(o.data.vertices))),1,'REPLACE');o.modifiers.new('Puppet joint','ARMATURE').object=rig;meshes.append(o)
actions=[]
for name,dirs in {'Puppet_R104_Idle':{},**targets}.items():
 rig.animation_data_clear()
 for pb in rig.pose.bones:pb.matrix_basis=base[pb.name]
 bpy.context.view_layer.update()
 for n,d in dirs.items():aim(rig,n,d)
 action=bpy.data.actions.new(name+'_R104' if name.startswith('Pose_') or name=='RepairBothHands' else name);action.use_fake_user=True;rig.animation_data_create();rig.animation_data.action=action
 for f in [1,61] if not dirs else [1,2]:
  for pb in rig.pose.bones:pb.rotation_mode='QUATERNION';pb.keyframe_insert(data_path='rotation_quaternion',frame=f);pb.keyframe_insert(data_path='location',frame=f)
 actions.append(action)
rig.animation_data.action=actions[0]
if actions[0].slots:rig.animation_data.action_slot=actions[0].slots[0]
bpy.context.scene.frame_set(1);bpy.context.scene.frame_start=1;bpy.context.scene.frame_end=61;bpy.context.scene.render.fps=30
actionNames=[a.name for a in actions]
save_export('Puppet',rig,meshes)
# Export one shared alphabet library with only the 16 intentional actions.
rig=bpy.data.objects['Puppet_Rig'];meshes=[o for o in bpy.context.scene.objects if o.type=='MESH']
for a in list(bpy.data.actions):
 if a.name not in actionNames:bpy.data.actions.remove(a,do_unlink=True)
exp=X.export(str(A/'PuppetPoseLibrary104.fbx'),'unity_skeletal',[rig,*meshes],bake_anim=True,bake_anim_use_all_actions=True)
(D/'puppet-finish.json').write_text(json.dumps(dict(source='BaseCharacter.blend CC0',preserved_silhouette=True,shared_alphabet=list(targets),report=report,pose_export=exp),default=str,indent=2));print('PUPPET finished intact silhouette, retargeted shared pose directions',len(targets))
