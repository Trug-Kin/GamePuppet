import bpy,json
from pathlib import Path
R=Path('D:/ConRoi');bpy.ops.wm.open_mainfile(filepath=str(R/'Source/Blender/JourneyV48/R104/Repair_R12_R104_Animation.blend'),load_ui=False,use_scripts=False)
s=bpy.context.scene;rows=[]
for t in [0,3.8,4.3,4.8,4.833333,5.2]:
 s.frame_set(round(t*30)+1);bpy.context.view_layer.update();row={'time':t,'objects':[]}
 for o in s.objects:
  if o.name.startswith(('Tu_Event','RepairTool','R12_Hand','R12_suspension','R104_clip')) or o.type=='ARMATURE':
   x={'name':o.name,'parent':o.parent.name if o.parent else None,'location':list(o.location),'world':list(o.matrix_world.translation),'hidden':o.hide_render,'animation':o.animation_data.action.name if o.animation_data and o.animation_data.action else None}
   if o.type=='ARMATURE':x['bones']={b.name:list(o.matrix_world@b.head) for b in o.pose.bones if b.name in ['handL','handR','head','foreL','foreR']}
   if o.type=='CURVE':x['points']=[list(o.matrix_world@p.co.xyz) for p in o.data.splines[0].points]
   row['objects'].append(x)
 rows.append(row)
 if t in [0,4.8,4.833333]:s.render.filepath=str(R/('Test/BangChung/investigation_r105/before-blender-'+str(t)+'.png'));bpy.ops.render.render(write_still=True)
(R/'Test/KetQua/investigation_r105/baseline-source.json').write_text(json.dumps({'blender':bpy.app.version_string,'samples':rows},indent=2));print('Read-only source audit complete')
