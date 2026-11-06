import bpy,math
from pathlib import Path
from mathutils import Vector
bpy.ops.wm.open_mainfile(filepath='C:/Users/kaito/AppData/Local/Temp/ConRoi-r91-authoring-20261101/character-repairs-r91-source.blend')
o=bpy.data.objects['R91_Portrait'];o.hide_set(False);o.hide_render=False
for a in bpy.context.scene.objects:
 if a!=o:a.hide_render=True
s=bpy.context.scene;s.render.engine='BLENDER_WORKBENCH';s.display.shading.light='STUDIO';s.display.shading.color_type='MATERIAL';s.display.shading.show_shadows=True;s.display.shading.show_cavity=True;s.render.resolution_x=400;s.render.resolution_y=400;s.render.resolution_percentage=100
cam=bpy.data.cameras.new('audit');c=bpy.data.objects.new('audit',cam);s.collection.objects.link(c);s.camera=c;cam.type='ORTHO';cam.ortho_scale=.30
for n,p in [('front',(0,1,0)),('back',(0,-1,0)),('left',(1,0,0)),('right',(-1,0,0))]:
 c.location=p;c.rotation_euler=(-c.location).to_track_quat('-Z','Y').to_euler();s.render.filepath='D:/Game/GameHorror/Game/Docs/Reviews/journey_v48/r91/head-source-'+n+'.png';bpy.ops.render.render(write_still=True)
