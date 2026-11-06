import bpy,json,math,numpy as np
from pathlib import Path
from mathutils import Vector
R=Path('D:/ConRoi');D=R/'Test/KetQua/journey_v48/i2_r93_complete';M=R/'Test/BangChung/journey_v48/i2_r93_complete';P=R/'Source/Blender/JourneyV48/R93/Ground_I2_R93_Editable.blend'
bpy.ops.wm.open_mainfile(filepath=str(P));sc=bpy.context.scene;col=bpy.data.collections['GROUND_NATIVE_MATCHED_R93'];before=bpy.data.collections['I2_BEFORE_DO_NOT_EXPORT'];context=bpy.data.collections.new('UNITY_DIAGNOSTIC_CONTEXT_DO_NOT_EXPORT');sc.collection.children.link(context)
inv=json.loads((R/'Test/KetQua/journey_v48/ground_r93/geometry-inventory.json').read_text())
for r in inv:
 if not r.get('cache') or not r['active'] or not r['render'] or not r['path'].startswith('R11_Local_Bridge_Supports/OD35_Surface_'):continue
 z=np.load(R/'Test/KetQua/journey_v48/ground_r93'/(r['cache']+'.npz'));v=z['v'];tri=z['tri'];cen=v[tri].mean(1);sel=np.linalg.norm(cen[:,[0,2]]-[714.94,459.48],axis=1)<45
 if not sel.any():continue
 mm=bpy.data.meshes.new('CONTEXT_'+r['path'].split('/')[-1]);mm.from_pydata(np.c_[-v[:,0],-v[:,2],v[:,1]],[],tri[sel][:,::-1].tolist());mm.update();o=bpy.data.objects.new(mm.name,mm);context.objects.link(o);o.color=(.66,.57,.42,1);o['context_only']='Unity diagnostic geometry; not editable authoring replacement; never export'
sc.render.engine='BLENDER_WORKBENCH';sc.render.resolution_x=1280;sc.render.resolution_y=720;sc.render.resolution_percentage=100;sc.display.shading.light='STUDIO';sc.display.shading.color_type='OBJECT';sc.display.shading.show_shadows=False;sc.display.shading.show_cavity=True;sc.display.shading.background_type='VIEWPORT';sc.display.shading.background_color=(.16,.19,.23);cam=sc.camera;cam.data.type='PERSP';cam.data.lens=32
colors={'Surface_128':(.39,.44,.48,1),'Surface_129':(.42,.46,.5,1),'Surface_142':(.28,.35,.21,1),'Surface_176':(.29,.36,.23,1),'Surface_153':(.60,.58,.52,1)}
for o in list(col.all_objects)+list(before.objects):o.color=colors.get(o.get('unity_path','').split('/')[-1],(.45,.45,.4,1))
shots=[('overview',[-685,-496,22],[-715,-470,.6]),('player-low',[-715,-484,2.3],[-715,-471,.3]),('bridge-join',[-696,-480,2.4],[-708,-472,.5]),('east-join',[-738,-477,2.5],[-725,-472,.5]),('connector-low',[-697,-491,2.7],[-705,-481,.8]),('branch-low',[-744,-497,1.8],[-731,-482,.3])]
for tag,eye,at in shots:
 cam.location=eye;cam.rotation_euler=(Vector(at)-cam.location).to_track_quat('-Z','Y').to_euler()
 for state in ['before','after']:
  before.hide_render=state!='before'
  for o in col.all_objects:
   if o.type=='MESH':o.hide_render=o.get('unity_path','').startswith('LakeGateMarket') or (state=='before' and bool(o.get('i2_export_key')))
  for o in before.objects:o.hide_render=o.get('unity_path','').startswith('LakeGateMarket')
  sc.render.filepath=str(M/(tag+'-'+state+'.png'));bpy.ops.render.render(write_still=True)
print('RENDERED_FIXED_CAMERAS',len(shots),flush=True)
