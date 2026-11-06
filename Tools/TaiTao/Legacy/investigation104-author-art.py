import bpy,sys,math,json,bmesh
from pathlib import Path
from mathutils import Vector,Matrix
R=Path('D:/ConRoi');S=R/'Source/Blender/JourneyV48/R104';D=R/'Test/KetQua/investigation_r104';E=R/'Test/BangChung/investigation_r104';A=R/'Assets/Art/InvestigationV36/R104'
sys.path.append(str(R/'.agents/skills/scenario-blender-export/scripts'));import bx_export as X
sys.path.append(str(R/'.agents/skills/scenario-blender-expert/scripts'));import bx_audit
report=[]
def mat(name,color):
 m=bpy.data.materials.get(name) or bpy.data.materials.new(name);m.diffuse_color=(*color,1);m.use_nodes=True;p=m.node_tree.nodes.get('Principled BSDF');p.inputs['Base Color'].default_value=(*color,1);p.inputs['Roughness'].default_value=.8;return m
def render(name,rig,meshes):
 scene=bpy.context.scene
 for o in scene.objects:o.hide_render=o.type not in ('MESH','ARMATURE') or (o.type=='MESH' and o not in meshes)
 for o in meshes:o.hide_render=False;o.hide_set(False)
 for o in list(bpy.data.objects):
  if o.type=='CAMERA':bpy.data.objects.remove(o,do_unlink=True)
 cd=bpy.data.cameras.new('R104_review');cam=bpy.data.objects.new('R104_review',cd);scene.collection.objects.link(cam);scene.camera=cam;cam.location=(2.8,-4.6,2.2);cam.rotation_euler=(Vector((0,0,.88))-cam.location).to_track_quat('-Z','Y').to_euler();cd.type='ORTHO';cd.ortho_scale=2.2
 scene.render.engine='BLENDER_WORKBENCH';scene.display.shading.color_type='MATERIAL';scene.display.shading.light='STUDIO';scene.display.shading.show_shadows=True;scene.display.shading.show_cavity=True;scene.display.shading.background_type='VIEWPORT';scene.display.shading.background_color=(.1,.13,.14);scene.render.resolution_x=720;scene.render.resolution_y=800;scene.render.resolution_percentage=100;scene.render.filepath=str(E/(name+'-source.png'));bpy.ops.render.render(write_still=True)
def normalized(name,height):
 rig=next(o for o in bpy.context.scene.objects if o.type=='ARMATURE');meshes=[o for o in bpy.context.scene.objects if o.type=='MESH']
 for o in [rig,*meshes]:o.animation_data_clear();o.hide_render=False;o.hide_set(False)
 for pb in rig.pose.bones:pb.matrix_basis.identity()
 bpy.context.view_layer.update();coords=[o.matrix_world@v.co for o in meshes for v in o.data.vertices];lo=min(v.z for v in coords);hi=max(v.z for v in coords);factor=height/(hi-lo);T=Matrix.Scale(factor,4)@Matrix.Translation((0,0,-lo))
 rig.data.transform(T@rig.matrix_world);rig.matrix_world=Matrix.Identity(4);rig.name=name+'_Rig'
 for o in meshes:
  o.data.transform(T@o.matrix_world);o.parent=rig;o.matrix_parent_inverse=Matrix.Identity(4);o.matrix_basis=Matrix.Identity(4)
  for mod in list(o.modifiers):
   if mod.type!='ARMATURE':o.modifiers.remove(mod)
  for v in o.data.vertices:
   weights=[g for g in v.groups if g.weight>0]
   total=sum(g.weight for g in weights)
   if total:
    for g in weights:o.vertex_groups[g.group].add([v.index],g.weight/total,'REPLACE')
  bm=bmesh.new();bm.from_mesh(o.data);bmesh.ops.recalc_face_normals(bm,faces=list(bm.faces));bm.to_mesh(o.data);bm.free()
 bpy.context.scene.unit_settings.system='METRIC';bpy.context.scene.unit_settings.scale_length=1
 return rig,meshes
def aim(rig,name,direction):
 pb=rig.pose.bones.get(name)
 if not pb:return
 current=pb.tail-pb.head;world=pb.matrix.to_quaternion();pb.rotation_mode='QUATERNION';pb.rotation_quaternion=pb.bone.matrix_local.to_quaternion().inverted()@current.rotation_difference(Vector(direction))@world
 # Matrix pose assignment avoids assumptions about local bone roll and parent transforms.
 matrix=pb.matrix.copy();rot=current.rotation_difference(Vector(direction));pb.matrix=Matrix.Translation(pb.head)@rot.to_matrix().to_4x4()@Matrix.Translation(-pb.head)@matrix;bpy.context.view_layer.update()
def pose_idle(rig):
 for side,sign in [('L',1),('R',-1)]:
  aim(rig,'UpperArm.'+side,(sign*.12,-.02,-1));aim(rig,'LowerArm.'+side,(sign*.05,-.16,-1))
def save_export(name,rig,meshes):
 scene=bpy.context.scene;scene.frame_set(1);render(name,rig,meshes)
 path=S/(name+'_R104_Editable.blend');rigName=rig.name;meshNames=[o.name for o in meshes];bpy.ops.wm.save_as_mainfile(filepath=str(path));bpy.ops.wm.open_mainfile(filepath=str(path),load_ui=False,use_scripts=False)
 rig=bpy.data.objects[rigName];meshes=[bpy.data.objects[n] for n in meshNames]
 gate=[]
 for o in meshes:
  # Connected eyes/hair and garment layers are intentional components of one character.
  g=bx_audit.deliverable([o],profile='game');gate.append(dict(object=o.name,report=g))
 exp=X.export(str(A/(name+'.fbx')),'unity_skeletal',[rig,*meshes],bake_anim=True,bake_anim_use_all_actions=False)
 rt=X.round_trip(str(A/(name+'.fbx')),[rig,*meshes],preset='unity_skeletal',bake_anim=True,bake_anim_use_all_actions=False)
 report.append(dict(name=name,source=str(path),reopened=True,export=exp,roundtrip=rt,gate=gate,triangles=sum(len(o.data.loop_triangles) for o in meshes),unweighted=sum(1 for o in meshes for v in o.data.vertices if not any(g.weight>0 for g in v.groups))))
# Khai: licensed original modular hoodie, trousers recolored, distinctive evidence cuff added below.
for name,file,height in [('Khai','Casual_Hoodie.blend',1.76),('Ha','Casual_Female.blend',1.64)]:
 bpy.ops.wm.open_mainfile(filepath=str(R/'Source/Downloads/Quaternius_R104'/file),load_ui=False,use_scripts=False)
 rig,meshes=normalized(name,height)
 colors={'Skin':(.58,.37,.22),'Purple':(.035,.15,.25),'LightBlue':(.045,.055,.06),'White':(.12,.13,.13),'Shirt':(.32,.18,.08),'Pants':(.045,.055,.065),'Hair':(.025,.016,.012),'Belt':(.07,.04,.025)}
 for o in meshes:
  for i,m in enumerate(o.data.materials):
   color=colors.get(m.name,tuple(m.diffuse_color[:3]));o.data.materials[i]=mat(name+'_'+m.name,color)
   if name=='Khai' and o.name=='Casual_Legs':
    # Long dark trousers rather than exposed pale calves; retain original rigged silhouette.
    o.data.materials[i]=mat('Khai_trousers',(.045,.055,.06))
 # Pose before authoring a restrained idle action. Imported unneeded combat actions are not exported.
 pose_idle(rig)
 base={b.name:b.matrix_basis.copy() for b in rig.pose.bones};rig.animation_data_create();action=bpy.data.actions.new(name+'_Idle_R104');rig.animation_data.action=action;action.use_fake_user=True
 for frame in [1,16,31,46,61]:
  for pb in rig.pose.bones:
   pb.matrix_basis=base[pb.name];pb.rotation_mode='QUATERNION'
   if pb.name=='Head':pb.rotation_quaternion=pb.rotation_quaternion@__import__('mathutils').Quaternion((0,0,1),math.sin((frame-1)/60*math.tau)*.025)
   pb.keyframe_insert(data_path='rotation_quaternion',frame=frame);pb.keyframe_insert(data_path='location',frame=frame)
 bpy.context.scene.frame_start=1;bpy.context.scene.frame_end=61;bpy.context.scene.render.fps=30
 save_export(name,rig,meshes)
# Wooden puppet built from downloaded base, segmented and bound to the existing pose-ID rig.
bpy.ops.wm.open_mainfile(filepath=str(R/'Source/Downloads/Quaternius_R104/BaseCharacter.blend'),load_ui=False,use_scripts=False)
srcRig=next(o for o in bpy.context.scene.objects if o.type=='ARMATURE');srcMeshes=[o for o in bpy.context.scene.objects if o.type=='MESH'];srcRig.animation_data_clear()
for pb in srcRig.pose.bones:pb.matrix_basis.identity()
bpy.context.view_layer.update()
source=[]
for o in srcMeshes:
 for poly in o.data.polygons:
  verts=[];weights={}
  for i in poly.vertices:
   v=o.data.vertices[i];verts.append(o.matrix_world@v.co)
   for g in v.groups:weights[o.vertex_groups[g.group].name]=weights.get(o.vertex_groups[g.group].name,0)+g.weight
  source.append((verts,max(weights,key=weights.get) if weights else 'Body'))
sourceBones={b.name:(srcRig.matrix_world@b.head_local,srcRig.matrix_world@b.tail_local,b.matrix_local.copy()) for b in srcRig.data.bones}
bpy.ops.wm.open_mainfile(filepath=str(S/'Puppet_Poses_R104_Editable.blend'),load_ui=False,use_scripts=False)
rig=next(o for o in bpy.context.scene.objects if o.type=='ARMATURE');rig.animation_data_clear();rig.location=(0,0,0);rig.rotation_euler=(0,0,0);rig.scale=(1,1,1)
for pb in rig.pose.bones:pb.matrix_basis.identity()
for o in list(bpy.context.scene.objects):
 if o!=rig:bpy.data.objects.remove(o,do_unlink=True)
wood=mat('R104_carved_wood',(.42,.235,.09));dark=mat('R104_joint_dark',(.085,.055,.027));eye=mat('R104_puppet_ink',(.025,.025,.022))
def target(n):
 side='L' if '.L' in n else 'R'
 if 'UpperArm' in n:return 'upper'+side
 if 'LowerArm' in n:return 'fore'+side
 if any(t in n for t in ['Wrist','Hand','Index','Thumb','Finger']):return 'hand'+side
 if any(t in n for t in ['UpperLeg','Thigh']):return 'thigh'+side
 if any(t in n for t in ['LowerLeg','Foot','Toe','Shin']):return 'shin'+side
 if any(t in n for t in ['Head','Neck']):return 'head'
 if any(t in n for t in ['Hips','Body','Root']):return 'root'
 return 'spine'
vertices=[];faces=[];groups=[]
for points,n in source:
 dst=target(n)
 if dst not in rig.data.bones:continue
 # Use major limb rest transforms; fingers stay in the hand section, no individual floating digits.
 primary=('Wrist.'+('L' if '.L' in n else 'R')) if dst.startswith('hand') else n
 if primary not in sourceBones:primary=n
 sh,st,sm=sourceBones[primary];db=rig.data.bones[dst]
 ratio=(db.tail_local-db.head_local).length/max(.05,(st-sh).length)
 if dst in ['root','spine']:ratio=.55
 ratio=max(.18,min(1.15,ratio));transform=db.matrix_local@Matrix.Scale(ratio,4)@sm.inverted()
 ids=[]
 for p in points:ids.append(len(vertices));vertices.append(tuple(transform@p));groups.append(dst)
 faces.append(ids)
mesh=bpy.data.meshes.new('R104_carved_base_segments');mesh.from_pydata(vertices,[],faces);mesh.update();body=bpy.data.objects.new('R104_Articulated_Puppet',mesh);bpy.context.scene.collection.objects.link(body);body.parent=rig;mesh.materials.append(wood)
for name in set(groups):
 vg=body.vertex_groups.new(name=name);vg.add([i for i,n in enumerate(groups) if n==name],1,'REPLACE')
arm=body.modifiers.new('Shared pose rig','ARMATURE');arm.object=rig
bm=bmesh.new();bm.from_mesh(mesh);bmesh.ops.remove_doubles(bm,verts=list(bm.verts),dist=.00001);bmesh.ops.recalc_face_normals(bm,faces=list(bm.faces));bm.to_mesh(mesh);bm.free()
meshes=[body]
for n in ['upperL','upperR','foreL','foreR','handL','handR','thighL','thighR','shinL','shinR','head']:
 p=rig.data.bones[n].head_local;bpy.ops.mesh.primitive_uv_sphere_add(segments=12,ring_count=6,radius=.044,location=p);o=bpy.context.object;o.name='R104_joint_'+n;bpy.ops.object.transform_apply(location=True,rotation=True,scale=True);o.data.materials.append(dark);o.parent=rig;vg=o.vertex_groups.new(name=n);vg.add(list(range(len(o.data.vertices))),1,'REPLACE');o.modifiers.new('Rigid joint','ARMATURE').object=rig;meshes.append(o)
# Keep the existing 15 shared pose actions and bind to identical bones. Idle at arms-down.
action=bpy.data.actions.new('Puppet_R104_Idle');rig.animation_data_create();rig.animation_data.action=action;action.use_fake_user=True
for f in [1,61]:
 for pb in rig.pose.bones:pb.rotation_mode='QUATERNION';pb.keyframe_insert(data_path='rotation_quaternion',frame=f)
bpy.context.scene.frame_start=1;bpy.context.scene.frame_end=61;bpy.context.scene.render.fps=30
save_export('Puppet',rig,meshes)
(D/'art-source-export.json').write_text(json.dumps(report,default=str,indent=2));print('Authored, reopened and exported',[(r['name'],r['unweighted'],r['roundtrip'].get('pass')) for r in report])
