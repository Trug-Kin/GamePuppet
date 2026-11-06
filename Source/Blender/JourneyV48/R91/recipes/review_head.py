import bpy,math
from pathlib import Path
from mathutils import Vector
R=Path('D:/Game/GameHorror/Game/Docs/Reviews/journey_v48/r91')
bpy.ops.wm.open_mainfile(filepath=str(R/'character-repairs-r91-source.blend'))
o=bpy.data.objects['R91_Portrait']
for ob in bpy.context.scene.objects:ob.hide_render=ob!=o
s=bpy.context.scene;s.render.engine='BLENDER_WORKBENCH';s.display.shading.light='STUDIO';s.display.shading.color_type='MATERIAL';s.display.shading.show_shadows=True;s.display.shading.show_cavity=True;s.display.shading.background_type='WORLD';s.world.color=(.2,.2,.2)
s.render.resolution_x=512;s.render.resolution_y=512;s.render.resolution_percentage=100
bpy.ops.object.camera_add();c=bpy.context.object;s.camera=c;c.data.type='ORTHO';c.data.ortho_scale=.35
for i,p in enumerate([(0,-1,.05),(0,1,.05),(-1,0,.05),(1,0,.05)]):
 c.location=p;c.rotation_euler=(-c.location).to_track_quat('-Z','Y').to_euler();s.render.filepath=str(R/('head-source-'+str(i)+'.png'));bpy.ops.render.render(write_still=True)
print('HEAD_REVIEW_DONE')
