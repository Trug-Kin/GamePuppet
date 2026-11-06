import bpy,json,math
from pathlib import Path
from mathutils import Vector
R=Path('D:/ConRoi');D=R/'Test/KetQua/investigation_r104';E=R/'Test/BangChung/investigation_r104';report=[]
for name,folder in [('Casual_Hoodie.blend','Source/Downloads/Quaternius_R104'),('Casual_Female.blend','Source/Downloads/Quaternius_R104'),('Workshop_Investigation_R104_Editable.blend','Source/Blender/JourneyV48/R104')]:
 bpy.ops.wm.open_mainfile(filepath=str(R/folder/name),load_ui=False,use_scripts=False)
 rows=[]
 for o in bpy.context.scene.objects:
  if o.type=='MESH':rows.append(dict(name=o.name,type=o.type,vertices=len(o.data.vertices),dimensions=list(o.dimensions),bounds=[list(o.matrix_world@Vector(c)) for c in o.bound_box],mats=[dict(name=m.name,color=list(m.diffuse_color)) if m else None for m in o.data.materials],parent=o.parent.name if o.parent else None))
  elif o.type=='ARMATURE':rows.append(dict(name=o.name,type=o.type,location=list(o.location),scale=list(o.scale),bones=[dict(name=b.name,head=list(b.head_local),tail=list(b.tail_local),parent=b.parent.name if b.parent else None) for b in o.data.bones]))
 report.append(dict(source=name,objects=rows,actions=[dict(name=a.name,frames=list(a.frame_range)) for a in bpy.data.actions]))
 if 'Casual' in name:
  for o in list(bpy.data.objects):
   if o.type in ['CAMERA','LIGHT']:bpy.data.objects.remove(o,do_unlink=True)
  scene=bpy.context.scene;scene.render.engine='BLENDER_WORKBENCH';scene.display.shading.color_type='MATERIAL';scene.display.shading.light='STUDIO';scene.display.shading.show_shadows=True;scene.display.shading.show_cavity=True;scene.display.shading.background_type='VIEWPORT';scene.display.shading.background_color=(.12,.14,.16)
  rig=next(o for o in scene.objects if o.type=='ARMATURE');rig.animation_data_clear()
  for p in rig.pose.bones:p.matrix_basis.identity()
  bpy.context.view_layer.update()
  camdata=bpy.data.cameras.new('ReviewCamera');cam=bpy.data.objects.new('ReviewCamera',camdata);scene.collection.objects.link(cam);scene.camera=cam;cam.location=(3,-5,2.8);target=Vector((0,0,.95));cam.rotation_euler=(target-cam.location).to_track_quat('-Z','Y').to_euler();camdata.type='ORTHO';camdata.ortho_scale=2.5
  scene.render.resolution_x=700;scene.render.resolution_y=700;scene.render.resolution_percentage=100;scene.render.filepath=str(E/(name+'.png'));bpy.ops.render.render(write_still=True)
(D/'art-inspection.json').write_text(json.dumps(report,indent=2))
for r in report:
 print(r['source'],[(o['name'],o.get('vertices'),[m['name'] if m else '' for m in o.get('mats',[])]) for o in r['objects']],len(r['actions']),'actions')
