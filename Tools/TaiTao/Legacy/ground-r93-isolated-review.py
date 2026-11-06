import bpy,json
from pathlib import Path
R=Path('D:/ConRoi');M=R/'Test/BangChung/journey_v48/ground_r93';P=R/'Source/Blender/JourneyV48/R93/Ground_I2_South_Transition_Study_R93.blend'
bpy.ops.wm.open_mainfile(filepath=str(P));sc=bpy.context.scene;col=bpy.data.collections['GROUND_NATIVE_MATCHED_R93'];sc.display.shading.show_shadows=False;sc.display.shading.show_cavity=False;sc.display.shading.light='FLAT';sc.display.shading.show_specular_highlight=False
cam=sc.camera;from mathutils import Vector
cam.data.type='ORTHO';cam.data.ortho_scale=50;cam.location=(-695,-498,18);cam.rotation_euler=(Vector((-714.94,-474,.5))-cam.location).to_track_quat('-Z','Y').to_euler()
for tag,names in [('road',['Surface_128','Surface_129']),('terrain',['Surface_176','Surface_142']),('footway',['Surface_153']),('all',['Surface_128','Surface_129','Surface_176','Surface_142','Surface_153'])]:
 for o in col.objects:o.hide_render=o.get('unity_path','').split('/')[-1] not in names
 sc.render.filepath=str(M/('I2-south-study-'+tag+'-flat.png'));bpy.ops.render.render(write_still=True)
print('ISOLATED_REVIEW_COMPLETE')
