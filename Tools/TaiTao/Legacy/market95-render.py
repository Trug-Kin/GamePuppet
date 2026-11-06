import bpy,json,numpy as np
from pathlib import Path
from mathutils import Vector,Matrix
R=Path('D:/ConRoi');D=R/'Test/KetQua/journey_v48/market_ground_r95';M=R/'Test/BangChung/journey_v48/market_ground_r95'
bpy.ops.wm.open_mainfile(filepath=str(R/'Source/Blender/JourneyV48/R95/Market_Ground_R95_Editable.blend'));sc=bpy.context.scene
for c in list(sc.collection.children):c.hide_render=c.name!='MARKET_GROUND_NATIVE_R95'
g=json.loads((D/'grading.json').read_text());parts=[];visible={r['path']:r['render'] for r in json.loads((D/'inventory.json').read_text())['rows']}
colors={'Surface_104':(.46,.46,.43,1),'Surface_176':(.32,.40,.26,1),'Restored_029':(.32,.40,.26,1),'Surface_153':(.65,.60,.48,1),'Surface_125':(.65,.60,.48,1)}
for row in g['objects']:
 if not visible.get(row['path'],False):continue
 a=np.load(D/(row['key']+'-source.npz'));o=bpy.data.objects[row['object']];o.color=colors.get(row['path'].split('/')[-1],(.38,.40,.42,1))
 parts.append((o,a))
for o in list(bpy.data.collections['LANGFARM_NATIVE_FOUNDATIONS'].all_objects):
 if o.type!='MESH':continue
 o.color=(.58,.48,.36,1);o.hide_set(False);o.hide_render=False
context_parts=[];context=bpy.data.collections.get('PLACEMENT_EDITABLE_PENDING_CONTACT')
known={o.name for o,a in parts}
for colname in ['CLOCK_NATIVE_CONTACT_R95','MARKET_NATIVE_FOUNDATION_R95']:
 col=bpy.data.collections.get(colname)
 if col:
  for o in list(col.all_objects):
   key=o.get('market95_export_key')
   if key and not o.hide_render:parts.append((o,np.load(D/(key+'-source.npz'))))
if context:
 for o in list(context.all_objects):
  key=o.get('candidate_key')
  if key and not o.hide_render:context_parts.append((o,np.load(D/(key+'-placement.npz'))))
cam=bpy.data.objects.new('MARKET95_FIXED_REVIEW',bpy.data.cameras.new('MARKET95_FIXED_REVIEW'));sc.collection.objects.link(cam);sc.camera=cam
sc.render.engine='BLENDER_WORKBENCH';sc.render.resolution_x=1280;sc.render.resolution_y=800;sc.render.resolution_percentage=100;sc.display.shading.light='STUDIO';sc.display.shading.color_type='OBJECT';sc.display.shading.show_shadows=False;sc.display.shading.show_cavity=True;sc.display.shading.background_type='VIEWPORT';sc.display.shading.background_color=(.14,.17,.20)
shots=[('whole-ground',[955,280,490],[750,3,300],32),('gate-branches',[748,3.3,435],[746,2,390],28),('apron-low',[750,6.7,315],[724,5.04,300],30),('plaza-low',[798,7.8,231],[785,6,188],26),('connector',[740,5.5,360],[760,4,310],28)]
shots += [('north-player',[830,6.7,165],[827,5.5,125],28),('supplement-gate-clear',[757,4,430],[742,2,408],27),('supplement-north-transition',[830,9,165],[827,9,125],28),('supplement-west-bank-transition',[650,4,395],[637,3,383],28)]
import sys
if '--winding-review' in sys.argv:shots=[('west-support-winding',[710,3.8,431],[696,1.64,413],28)]
for tag,eye,at,lens in shots:
 cam.location=(-eye[0],-eye[2],eye[1]);cam.rotation_euler=(Vector((-at[0],-at[2],at[1]))-cam.location).to_track_quat('-Z','Y').to_euler();cam.data.lens=lens
 for state in ['before','after']:
  for o,a in parts+context_parts:
   v=a['before'] if state=='before' else a['v'];world=np.c_[-v[:,0],-v[:,2],v[:,1]];mat=np.array(o.matrix_world);local=(world-mat[:3,3])@np.linalg.inv(mat[:3,:3]).T;o.data.vertices.foreach_set('co',local.ravel());o.data.update()
  sc.render.filepath=str(M/(tag+'-'+state+'.png'));bpy.ops.render.render(write_still=True)
print('RENDERED_MARKET95_FIXED_CAMERAS',len(shots),flush=True)
