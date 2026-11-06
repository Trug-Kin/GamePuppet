import bpy,sys,math,json
from pathlib import Path
from mathutils import Vector,Matrix
R=Path('D:/ConRoi');D=R/'Test/KetQua/investigation_r102';E=R/'Test/BangChung/investigation_r102';A=R/'Assets/Art/InvestigationV36/R102';Png=A/'Poses';Png.mkdir(exist_ok=True)
sys.path.extend([str(R/'.agents/skills/scenario-blender-expert/scripts'),str(R/'.agents/skills/scenario-blender-export/scripts')]);import bx_export,bx_audit
bpy.ops.wm.open_mainfile(filepath=str(R/'Source/Blender/JourneyV48/R102/Workshop_Investigation_R102_Editable.blend'))
s=bpy.context.scene;rig=bpy.data.objects['R03_Rig'];body=bpy.data.objects['R03_Body'];rig.location=(0,0,0)
for o in s.objects:o.hide_render=o not in [rig,body,s.camera]
def P(p):return Vector((-p[0],-p[2],p[1]))
camera=s.camera;camera.location=P((0,.92,4));camera.rotation_euler=(P((0,.92,0))-camera.location).to_track_quat('-Z','Y').to_euler();camera.data.type='ORTHO';camera.data.ortho_scale=2.3
s.render.resolution_x=180;s.render.resolution_y=230;s.render.film_transparent=True
s.display.shading.background_type='WORLD';s.world=bpy.data.worlds.new('PoseReviewWorld');s.world.color=(.035,.04,.045)
rig.animation_data_create();letters='ABCDEGHINORSTWY';poses=[]
def direction(bone,d):
 bpy.context.view_layer.update();pb=rig.pose.bones[bone];head=pb.head.copy();q=P(d).to_track_quat('Y','Z');pb.matrix=Matrix.Translation(head)@q.to_matrix().to_4x4();bpy.context.view_layer.update()
for i,c in enumerate(letters):
 for pb in rig.pose.bones:pb.matrix_basis=Matrix.Identity(4);pb.rotation_mode='QUATERNION'
 la=math.radians(-100+(i%5)*48);ra=math.radians(-85+(i//5)*75)
 direction('upperL',(-math.cos(la),-math.sin(la),0));direction('foreL',(-.5,1 if i%2==0 else -1,0))
 direction('upperR',(math.cos(ra),-math.sin(ra),0));direction('foreR',(.5,1 if i%3==0 else -1,0))
 action=bpy.data.actions.new('Pose_'+c);action.use_fake_user=True;rig.animation_data.action=action
 for f in (1,2):
  for pb in rig.pose.bones:pb.keyframe_insert(data_path='rotation_quaternion',frame=f,group=pb.name);pb.keyframe_insert(data_path='location',frame=f,group=pb.name)
 s.frame_set(1);s.render.filepath=str(Png/('Pose_'+c+'.png'));bpy.ops.render.render(write_still=True)
 poses.append({'id':c,'index':i,'action':action.name})
# Add a repair pose with both occupied hands visible in the event illustration.
for pb in rig.pose.bones:pb.matrix_basis=Matrix.Identity(4)
direction('upperL',(-.4,.1,.8));direction('foreL',(.1,.45,.65));direction('upperR',(.4,.1,.8));direction('foreR',(-.1,.45,.65))
action=bpy.data.actions.new('RepairBothHands');action.use_fake_user=True;rig.animation_data.action=action
for f in (1,2):
 for pb in rig.pose.bones:pb.keyframe_insert(data_path='rotation_quaternion',frame=f,group=pb.name);pb.keyframe_insert(data_path='location',frame=f,group=pb.name)
s.frame_set(1);s.render.filepath=str(E/'repair-hands-source.png');bpy.ops.render.render(write_still=True)
bpy.ops.wm.save_as_mainfile(filepath=str(R/'Source/Blender/JourneyV48/R102/Puppet_Poses_R102_Editable.blend'))
result=bx_export.export(str(A/'PuppetPoseLibrary.fbx'),'unity_skeletal',objects=[rig,body],bake_anim=True)
result['roundtrip']=bx_export.round_trip(str(A/'PuppetPoseLibrary.fbx'),[rig,body],preset='unity_skeletal',bake_anim=True)
(D/'pose-library.json').write_text(json.dumps({'poses':poses,'result':result},default=str,indent=2))
print('15 POSE IDS + REPAIR HANDS: source and FBX, real puppet renders')
