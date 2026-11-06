import bpy,math,sys,json,bmesh
from pathlib import Path
from mathutils import Vector,Matrix
R=Path('D:/ConRoi')
exec(compile((R/'Test/Tam/investigation104-author-art.py').read_text(encoding='utf-8').split('# Khai:')[0],'r104-art-helpers','exec'))
def box(name,loc,scale,material):
 bpy.ops.mesh.primitive_cube_add(size=1,location=loc);o=bpy.context.object;o.name=name;o.scale=scale;bpy.ops.object.transform_apply(location=True,rotation=True,scale=True);o.data.materials.append(material);return o
def bind(o,rig,bone):
 o.parent=rig;vg=o.vertex_groups.new(name=bone);vg.add(list(range(len(o.data.vertices))),1,'REPLACE');o.modifiers.new('Clothing follows rig','ARMATURE').object=rig
def make_idle(rig,name):
 base={b.name:b.matrix_basis.copy() for b in rig.pose.bones};rig.animation_data_create();a=bpy.data.actions.new(name+'_Idle_R104');rig.animation_data.action=a;a.use_fake_user=True
 for f in [1,16,31,46,61]:
  for pb in rig.pose.bones:
   pb.matrix_basis=base[pb.name];pb.rotation_mode='QUATERNION'
   if pb.name=='Head':pb.rotation_quaternion=pb.rotation_quaternion@__import__('mathutils').Quaternion((0,0,1),math.sin((f-1)/60*math.tau)*.025)
   pb.keyframe_insert(data_path='rotation_quaternion',frame=f);pb.keyframe_insert(data_path='location',frame=f)
 bpy.context.scene.frame_start=1;bpy.context.scene.frame_end=61;bpy.context.scene.render.fps=30
 return a
for name,file,height in [('Khai','Casual_Male.blend',1.76),('Ha','Casual_Female.blend',1.64)]:
 bpy.ops.wm.open_mainfile(filepath=str(R/'Source/Downloads/Quaternius_R104'/file),load_ui=False,use_scripts=False);rig,meshes=normalized(name,height)
 colors={'Skin':(.57,.36,.22),'Shirt':(.03,.14,.25) if name=='Khai' else (.28,.15,.07),'Pants':(.045,.055,.06),'Hair':(.025,.018,.013),'Face':(.025,.022,.019),'Belt':(.055,.035,.025)}
 for o in meshes:
  for i,m in enumerate(o.data.materials):o.data.materials[i]=mat(name+'_'+m.name,colors.get(m.name,tuple(m.diffuse_color[:3])))
  # Match long trousers, preserve hands/face while recoloring only lower leg skin surfaces.
  pants=mat(name+'_long_trousers',(.045,.055,.06));o.data.materials.append(pants)
  for p in o.data.polygons:
   if p.center.z<.47 and 'Skin' in o.data.materials[p.material_index].name:p.material_index=len(o.data.materials)-1
 if name=='Khai':
  # Build a fitted jacket sleeve using existing limb surface; the cuff below has a real missing edge.
  for o in meshes:
   cloth=mat('Khai_cuff_denim',(.03,.14,.25));o.data.materials.append(cloth);clothIndex=len(o.data.materials)-1
   bm=bmesh.new();bm.from_mesh(o.data)
   for face in bm.faces:
    c=face.calc_center_median();g={o.vertex_groups[v.group].name for vert in face.verts for v in o.data.vertices[vert.index].groups if v.weight>.4}
    if any('Arm' in n for n in g):face.material_index=clothIndex
   bm.to_mesh(o.data);bm.free()
  # Matched torn cuff: flattened edge points identical to the preserved fragment, no identity claim.
  upper=rig.data.bones['LowerArm.R'];wrist=rig.data.bones.get('Hand.R') or rig.data.bones.get('Wrist.R');center=wrist.head_local if wrist else upper.tail_local;axis=(upper.tail_local-upper.head_local).normalized();u=Vector((0,0,1));normal=Vector((0,-1,0));edge=[(-.06,0),(-.04,.015),(-.02,-.007),(.004,.024),(.024,.006),(.06,.018)]
  outline=edge+[(.06,-.10),(-.06,-.10)];co=[center+u*x+axis*z+normal*.058 for x,z in outline];mesh=bpy.data.meshes.new('MatchedCuffEdge');mesh.from_pydata(co,[],[tuple(range(len(co)))]);mesh.update();cuff=bpy.data.objects.new('Khai_torn_right_cuff',mesh);bpy.context.scene.collection.objects.link(cuff);mesh.materials.append(cloth);bind(cuff,rig,'LowerArm.R');solid=cuff.modifiers.new('Thin cuff cloth','SOLIDIFY');solid.thickness=.002
  bpy.context.view_layer.objects.active=cuff;cuff.select_set(True);bpy.ops.object.modifier_apply(modifier=solid.name);meshes.append(cuff)
  thread=mat('Khai_cuff_thread',(.52,.62,.67))
  for i,(x,z) in enumerate(edge):
   p=center+u*x+axis*z+normal*.060;o=box('Cuff_frayed_thread_'+str(i),p,(.001,.001,.015),thread);bind(o,rig,'LowerArm.R');meshes.append(o)
 else:
  # Simple ochre work apron, weighted to existing spine; source remains editable.
  bone=rig.data.bones.get('Torso') or rig.data.bones.get('Chest');apron=mat('Ha_ochre_work_apron',(.47,.28,.09))
  verts=[(-.14,-.145,1.14),(.14,-.145,1.14),(.21,-.16,.57),(-.21,-.16,.57)];mesh=bpy.data.meshes.new('Ha_apron');mesh.from_pydata(verts,[],[(3,2,1,0)]);mesh.update();o=bpy.data.objects.new('Ha_work_apron',mesh);bpy.context.scene.collection.objects.link(o);mesh.materials.append(apron);solid=o.modifiers.new('Apron cloth thickness','SOLIDIFY');solid.thickness=.002;bpy.context.view_layer.objects.active=o;bpy.ops.object.modifier_apply(modifier=solid.name);bind(o,rig,bone.name);meshes.append(o)
 pose_idle(rig);idle=make_idle(rig,name);save_export(name,rig,meshes)
(D/'art-final-export.json').write_text(json.dumps(report,default=str,indent=2));print('Consistent Quaternius set authored/exported',[(r['name'],r['roundtrip'].get('pass')) for r in report])
