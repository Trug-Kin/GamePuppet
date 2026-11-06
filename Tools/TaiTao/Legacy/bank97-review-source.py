import bpy,json,numpy as np
from pathlib import Path
from mathutils import Vector
R=Path('D:/ConRoi');D=R/'Test/KetQua/journey_v48/bank_r97';M=R/'Test/BangChung/journey_v48/bank_r97';P=R/'Source/Blender/JourneyV48/R97/Bridgehead_Bank_R97_Editable.blend'
bpy.ops.wm.open_mainfile(filepath=str(P));sc=bpy.context.scene;sc.display.shading.show_shadows=False;sc.display.shading.show_cavity=True
coast=np.array(json.loads((D/'author-result.json').read_text())['coast']);fixed=[]
for o in list(sc.objects):
 if o.type!='MESH' or not o.get('bank97_cut'):continue
 n=0
 for p in o.data.polygons:
  c=o.matrix_world@p.center;z=-c.y;x=-c.x
  if 418<=z<=483 and 16-.001<=x-float(np.interp(z,coast[:,0],coast[:,1]))<=24 and p.normal.z<-.70 and p.area<10:
   p.flip();n+=1
 if n:o.data.update();fixed.append(dict(object=o.name,faces=n,positions_changed=0,reason='Confirmed backface-only visible inherited bank join, narrow landward seam band; not I2 grading'))
(D/'boundary-winding.json').write_text(json.dumps(fixed,indent=2))
grass=bpy.data.materials['R97_Grass']
for name in ['R97_LOWER_WALK','R97_INTERMEDIATE_WALK']:
 o=bpy.data.objects[name];o.data.materials.append(grass)
 for p in o.data.polygons:
  z=-p.center.y
  if z<422 or z>480:p.material_index=1
sc['bank97_open_paths']='Paved lower/middle paths Z422..480, return via stairs429/450/471; endpoint blend Z418..422/480..483 is grass, not an opened path. All route directions require Unity check.'
for o in list(sc.objects):
 if o.type!='MESH' or o.get('bank97_export') or o.name=='CONTEXT_WATER_Y0_UNCHANGED':continue
 # R96 includes historical placement/before collections as editable provenance.
 # They must never be presented as a second rendered environment.
 if any('BEFORE' in c.name or 'PENDING' in c.name or 'COLLIDER' in c.name for c in o.users_collection):o.hide_render=True
sc.render.resolution_x=1280;sc.render.resolution_y=800
shots=[('wide',[645,14,482],[670,.7,447]),('upper',[682,2.7,475],[660,.9,431]),('lower',[668,1.85,453],[655,.6,430]),('stairs',[666,2.4,449],[674,.9,450])]
for name,eye,at in shots:
 sc.camera.location=(-eye[0],-eye[2],eye[1]);sc.camera.rotation_euler=(Vector((-at[0],-at[2],at[1]))-sc.camera.location).to_track_quat('-Z','Y').to_euler();sc.render.filepath=str(M/('blender-'+name+'-review.png'));bpy.ops.render.render(write_still=True)
bpy.context.preferences.filepaths.save_version=0;bpy.ops.wm.save_as_mainfile(filepath=str(P),compress=True)
print('RESULT reviewed source saved',flush=True)
