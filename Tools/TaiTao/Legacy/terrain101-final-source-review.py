import bpy,json
from pathlib import Path
from mathutils import Vector
from mathutils.bvhtree import BVHTree
R=Path('D:/ConRoi');D=R/'Test/KetQua/journey_v48/terrain_r101';E=R/'Test/BangChung/journey_v48/terrain_r101';data=[]
for version,src in [('before','R100/OngDao_Dam_R100_Editable.blend'),('after','R101/Terrain_R101_Editable.blend')]:
 bpy.ops.wm.open_mainfile(filepath=str(R/'Source/Blender/JourneyV48'/src));trees={}
 for name in ['BankR91_Grade_017','R97_LOWER_WALK','R97_LOWER_GRASS','M95_Service3_NATIVE']:
  o=bpy.data.objects[name];v=[o.matrix_world@p.co for p in o.data.vertices];trees[name]=BVHTree.FromPolygons(v,[tuple(p.vertices) for p in o.data.polygons])
 rows=[]
 for x in [643.5,644.6,646,647,648,649,650,651,652]:
  for z in [414,417,417.9,417.999,418.001,418.1,418.5,419,420]:
   hits=[]
   for n,t in trees.items():
    p,no,idx,dist=t.ray_cast(Vector((-x,-z,15)),Vector((0,0,-1)),30)
    if p:hits.append(dict(object=n,y=p.z,normalZ=no.z))
   rows.append(dict(x=x,z=z,layers=hits))
 data.append(dict(version=version,rows=rows))
(D/'source-sections-before-after.json').write_text(json.dumps(data,indent=2))
sc=bpy.context.scene;sc.render.engine='BLENDER_WORKBENCH';sc.render.resolution_x=1100;sc.render.resolution_y=750;sc.render.resolution_percentage=100;sc.display.shading.light='STUDIO';sc.display.shading.color_type='MATERIAL';sc.display.shading.show_backface_culling=True
for ob in sc.objects:
 if ob.type=='MESH':ob.hide_render=not bool(ob.get('dam99_actual_renderer'))
cam=bpy.data.objects.new('R101_FINAL_SOURCE_REVIEW',bpy.data.cameras.new('R101FinalReview'));sc.collection.objects.link(cam);cam.data.lens=28;cam.data.clip_start=.01;sc.camera=cam
for name,pos,target in [('blender-final',(-641.5,-412,3.2),(-644.5,-415,.2)),('blender-join-final',(-645,-415,2.8),(-649,-418,.5))]:
 cam.location=pos;cam.rotation_euler=(Vector(target)-cam.location).to_track_quat('-Z','Y').to_euler();sc.render.filepath=str(E/(name+'.png'));bpy.ops.render.render(write_still=True)
print('RESULT source sections and two final renders; no save mutation',flush=True)
