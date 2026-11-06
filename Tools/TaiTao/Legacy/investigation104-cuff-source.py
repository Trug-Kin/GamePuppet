import bpy,sys,json,math
from pathlib import Path
from mathutils import Vector
R=Path('D:/ConRoi')
exec(compile((R/'Test/Tam/investigation104-author-art.py').read_text(encoding='utf-8').split('# Khai:')[0],'r104-art-helpers','exec'))
bpy.ops.wm.open_mainfile(filepath=str(S/'Khai_R104_Editable.blend'),load_ui=False,use_scripts=False)
rig=bpy.data.objects['Khai_Rig'];idle=rig.animation_data.action
for o in list(bpy.context.scene.objects):
 if o.name=='Khai_torn_right_cuff' or o.name.startswith('Cuff_frayed_thread_'):bpy.data.objects.remove(o,do_unlink=True)
edge=[(-.06,0),(-.04,.015),(-.02,-.007),(.004,.024),(.024,.006),(.06,.018)]
outline=edge+[(.06,.10),(-.06,.10)]
fore=rig.data.bones['LowerArm.R'];center=fore.tail_local;axis=-(fore.tail_local-fore.head_local).normalized();u=Vector((0,0,1));normal=Vector((0,-1,0));cloth=mat('Khai_cuff_denim',(.03,.14,.25));thread=mat('Khai_cuff_thread',(.52,.62,.67))
def slab(name,coords,material):
 mesh=bpy.data.meshes.new(name);mesh.from_pydata(coords,[],[tuple(range(len(coords)-1,-1,-1))]);mesh.update();o=bpy.data.objects.new(name,mesh);bpy.context.scene.collection.objects.link(o);mesh.materials.append(material);sol=o.modifiers.new('Cloth thickness','SOLIDIFY');sol.thickness=.0015;bpy.context.view_layer.objects.active=o;bpy.ops.object.modifier_apply(modifier=sol.name);return o
def bind(o):
 o.parent=rig;g=o.vertex_groups.new(name='LowerArm.R');g.add(list(range(len(o.data.vertices))),1,'REPLACE');o.modifiers.new('Cuff follows forearm','ARMATURE').object=rig
cuff=slab('Khai_torn_right_cuff',[center+u*x+axis*v+normal*.058 for x,v in outline],cloth);bind(cuff)
for o in bpy.context.scene.objects:
 if o.type!='MESH' or o==cuff:continue
 skin=next((i for i,m in enumerate(o.data.materials) if m and m.name=='Khai_Skin'),None)
 if skin is not None:
  for p in o.data.polygons:
   if (p.center-center).length<.09 and p.center.y<center.y:p.material_index=skin
meshes=[o for o in bpy.context.scene.objects if o.type=='MESH'];save_export('Khai',rig,meshes)
# Flat sample uses the complementary half of the very same edge used by the fragment.
rig=bpy.data.objects['Khai_Rig'];cloth=bpy.data.materials['Khai_cuff_denim'];thread=bpy.data.materials['Khai_cuff_thread']
sample=slab('CuffSample',[(x,-v,0) for x,v in outline],cloth)
for i,(x,v) in enumerate(edge):
 bpy.ops.mesh.primitive_cube_add(size=1,location=(x,-v,.002));o=bpy.context.object;o.name='Cuff_stitch_'+str(i);o.scale=(.004,.010,.001);bpy.ops.object.transform_apply(location=True,rotation=True,scale=True);o.data.materials.append(thread)
parts=[sample]+[o for o in bpy.context.scene.objects if o.name.startswith('Cuff_stitch_')]
bpy.ops.object.select_all(action='DESELECT')
for o in parts:o.select_set(True)
bpy.context.view_layer.objects.active=sample;bpy.ops.object.join();sample['matching_edge']=json.dumps(edge);sample['observation_limit']='Complementary torn edge is an observation; cloth type/color alone does not identify a person or time.'
exp=X.export(str(A/'CuffComparison.fbx'),'unity',[sample]);rt=X.round_trip(str(A/'CuffComparison.fbx'),[sample],preset='unity')
bpy.ops.wm.save_as_mainfile(filepath=str(S/'Cuff_Comparison_R104_Editable.blend'));bpy.ops.wm.open_mainfile(filepath=str(S/'Cuff_Comparison_R104_Editable.blend'),load_ui=False,use_scripts=False)
(D/'cuff-source.json').write_text(json.dumps(dict(edge=edge,fragment_side='negative v',remaining_cuff_side='positive v',complementary=True,source='Cuff_Comparison_R104_Editable.blend',reopened=True,export=exp,roundtrip=rt,khai_export=report),default=str,indent=2));print('Complementary cuff and fragment edge exported; source reopened')
