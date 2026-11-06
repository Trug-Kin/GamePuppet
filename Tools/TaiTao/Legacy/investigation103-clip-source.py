import bpy,math,sys,json
from pathlib import Path
from mathutils import Vector,Matrix
R=Path('D:/ConRoi');S=R/'Source/Blender/JourneyV48/R103';D=R/'Test/KetQua/investigation_r103';E=R/'Test/BangChung/investigation_r103';A=R/'Assets/Art/InvestigationV36/R103'
bpy.ops.wm.open_mainfile(filepath=str(S/'Workshop_Investigation_R103_Editable.blend'));scene=bpy.context.scene
def P(v):return Vector((-v[0],-v[2],v[1]))
rig=bpy.data.objects['Khai_Rig'];body=bpy.data.objects['Khai_Body'];rig.name='Tu_Event_Rig';body.name='Tu_Event_Body'
with bpy.data.libraries.load(str(S/'Puppet_Poses_R103_Editable.blend'),link=False) as (src,dst):dst.actions=['RepairBothHands']
repair=dst.actions[0];rig.animation_data_create();rig.animation_data.action=repair
if repair.slots:rig.animation_data.action_slot=repair.slots[0]
scene.frame_set(1);bpy.context.view_layer.update();pose={b.name:b.matrix_basis.copy() for b in rig.pose.bones}
skin=bpy.data.materials['Warm skin'];shirt=bpy.data.materials.new('Tu plain brown shirt');shirt.diffuse_color=(.32,.24,.15,1)
for i,m in enumerate(body.data.materials):
 if m and 'Khai blue' in m.name:body.data.materials[i]=shirt
rig.animation_data_clear();rig.rotation_euler=(0,0,math.pi);rig.scale=(1,1,1)
gate=bpy.data.objects['GateLeaf_L'];latch=bpy.data.objects['Latch_C'];gate.location=P((0,3,1.5));latch.location=P((1.51,3.92,1.5))
rope=bpy.data.objects['ReleaseRope_D'];rope.location=P((0,3,1.5))
brass=bpy.data.materials.new('R103 latch brass');brass.diffuse_color=(.74,.53,.13,1);latch.data=latch.data.copy();latch.data.materials.clear();latch.data.materials.append(brass)
tool=bpy.data.objects['RepairTool'];tool.animation_data_clear()
curve=bpy.data.curves.new('RepairStringContinuous','CURVE');curve.dimensions='3D';curve.bevel_depth=.006;line=curve.splines.new('POLY');line.points.add(2);string=bpy.data.objects.new('R12_Hand_RepairString',curve);scene.collection.objects.link(string);curve.materials.append(brass)
r12=bpy.data.objects['R12_Rig'];r12.location=P((.35,3.4,1.1));r12.scale=(.85,)*3;r12.rotation_euler=(0,0,math.pi)
scene.render.fps=30;scene.frame_start=1;scene.frame_end=361
motion=[];state=dict(angle=0.,velocity=0.,latched=True)
def advance(st,t,dt):
 released=4<=t<5.5;force=t<4.8
 if released:st['latched']=False
 if st['latched']:st['angle']=0;st['velocity']=0;return
 v=max(-55,min(55,st['velocity']+((230 if force else 0)-2.5*st['angle']-4*st['velocity'])*dt));n=max(0,min(82,st['angle']+v*dt))
 if n in (0,82):v=0
 st['angle']=n;st['velocity']=v
 if n<.35 and not released and not force:st.update(angle=0,velocity=0,latched=True)
for f in range(1,362):
 t=(f-1)/30
 if f>1:
  for sub in range(4):advance(state,t-1/30+sub/120,1/120)
 e=max(0,t-4.8);root=(.8,max(.2,3-max(0,e-.35)*2),1.65+(.0 if t<=4.8 else (1.05-1.65)*min(1,e/.35)))
 rig.location=P(root);rig.rotation_euler=(0,0,math.pi)
 if t>4.8:rig.rotation_euler.x=math.radians(min(75,e*80))
 rig.keyframe_insert(data_path='location',frame=f);rig.keyframe_insert(data_path='rotation_euler',frame=f)
 for pb in rig.pose.bones:
  pb.matrix_basis=pose[pb.name];pb.rotation_mode='QUATERNION';pb.keyframe_insert(data_path='rotation_quaternion',frame=f);pb.keyframe_insert(data_path='location',frame=f)
 bpy.context.view_layer.update();tool.matrix_world=rig.matrix_world@rig.pose.bones['handL'].matrix;tool.keyframe_insert(data_path='location',frame=f);tool.keyframe_insert(data_path='rotation_euler',frame=f)
 hand=rig.matrix_world@rig.pose.bones['handR'].head;attachment=r12.matrix_world@r12.pose.bones['foreR'].tail
 for i,point in enumerate(line.points):
  p=hand.lerp(attachment,i/2);point.co=(*p,1);point.keyframe_insert(data_path='co',frame=f)
 gate.rotation_euler=(0,0,math.radians(state['angle']));gate.keyframe_insert(data_path='rotation_euler',frame=f)
 latch.rotation_euler=(0,math.radians(0 if state['latched'] else 65),0);latch.keyframe_insert(data_path='rotation_euler',frame=f)
 motion.append(dict(frame=f,time=t,angle=state['angle'],latched=state['latched'],root=root))
rig.animation_data.action.name='RepairEventContinuous_R103';rig.animation_data.action.use_fake_user=True
from bpy_extras.anim_utils import action_get_channelbag_for_slot
for obj in [rig,gate,latch]:
 ad=obj.animation_data
 if ad and ad.action:
  bag=action_get_channelbag_for_slot(ad.action,ad.action_slot)
  if bag:
   for fc in bag.fcurves:
    for k in fc.keyframe_points:k.interpolation='LINEAR'
allowed={rig.name,body.name,r12.name,'R12_Body','GateLeaf_L','Latch_C','ReleaseRope_D','WorkshopShell','RepairTool'}
for o in scene.objects:o.hide_render=o.type in ('MESH','ARMATURE') and o.name not in allowed
camera=scene.camera;camera.location=P((2.7,4.7,-1.0));camera.rotation_euler=(P((.8,4.1,1.55))-camera.location).to_track_quat('-Z','Y').to_euler();camera.data.type='PERSP';camera.data.lens=43
scene.render.engine='BLENDER_WORKBENCH';scene.display.shading.color_type='MATERIAL';scene.display.shading.background_type='VIEWPORT';scene.display.shading.background_color=(.13,.15,.16);scene.render.resolution_x=1100;scene.render.resolution_y=650;scene.render.resolution_percentage=100
for t in [0,3.8,4.15,4.8,6.1,6.7]:
 scene.frame_set(round(t*30)+1);scene.render.filepath=str(E/('clip-source-'+str(t)+'.png'));bpy.ops.render.render(write_still=True)
scene.frame_set(1);bpy.ops.wm.save_as_mainfile(filepath=str(S/'Repair_R12_R103_Animation.blend'));bpy.ops.wm.open_mainfile(filepath=str(S/'Repair_R12_R103_Animation.blend'))
rig=bpy.data.objects['Tu_Event_Rig'];body=bpy.data.objects['Tu_Event_Body'];sys.path.append(str(R/'.agents/skills/scenario-blender-export/scripts'));import bx_export
export=bx_export.export(str(A/'RepairEventActor.fbx'),'unity_skeletal',objects=[rig,body],bake_anim=True,bake_anim_use_all_actions=False)
rt=bx_export.round_trip(str(A/'RepairEventActor.fbx'),[rig,body],preset='unity_skeletal',bake_anim=True,bake_anim_use_all_actions=False)
(D/'clip-animation-source.json').write_text(json.dumps(dict(source=str(S/'Repair_R12_R103_Animation.blend'),reopened=True,silent=True,fps=30,frames=361,continuous=True,export=export,roundtrip=rt,samples=motion),default=str,indent=2));print('CLIP source animated, reopened, exported, roundtrip checked')
