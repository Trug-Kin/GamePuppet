import bpy,json,hashlib,sys,numpy as np
from pathlib import Path
from mathutils import Vector
R=Path('D:/ConRoi');D=R/'Test/KetQua/journey_v48/terrain_r101';E=R/'Test/BangChung/journey_v48/terrain_r101';S=R/'Source/Blender/JourneyV48/R101';P=S/'Terrain_R101_Editable.blend'
assert not P.exists();S.mkdir(parents=True,exist_ok=True)
bpy.ops.wm.open_mainfile(filepath=str(R/'Source/Blender/JourneyV48/R100/OngDao_Dam_R100_Editable.blend'))
def digest(o):
 return hashlib.sha256(np.array([p.co[:] for p in o.data.vertices],dtype=np.float32).tobytes()+str([tuple(p.vertices) for p in o.data.polygons]).encode()).hexdigest()
before={o.name:digest(o) for o in bpy.context.scene.objects if o.type=='MESH'}
o=bpy.data.objects['BankR91_Grade_017'];v=[o.matrix_world@p.co for p in o.data.vertices];changes=[]
for p in o.data.polygons:
 q=[v[i] for i in p.vertices];c=sum(q,Vector())/len(q)
 if not (640-1e-3<=-c.x<=658.001 and 411-1e-3<=-c.y<=418.001 and max(a.z for a in q)>-.05 and p.normal.z<-.001):continue
 changes.append(dict(face=p.index,vertices=list(p.vertices),center_unity=[-c.x,c.z,-c.y],normal_before=list(p.normal)));p.flip()
assert changes and any(r['face']==226063 for r in changes)
o.data.update();o['terrain101_fix']='Bounded top-sheet winding repair only; X640..658/Z411..418, triangles reaching Y>-.05. No vertices, shore, water, market or dam heights changed.'
col=bpy.data.collections.new('TERRAIN_R101_EDITABLE_REPAIR');bpy.context.scene.collection.children.link(col);col.objects.link(o)
changed=[n for n,h in before.items() if digest(bpy.data.objects[n])!=h];assert changed==[o.name],changed
o.select_set(True);bpy.context.view_layer.objects.active=o
report=dict(source=str(P),changed_objects=changed,unchanged_objects=len(before)-1,flipped_faces=len(changes),changes=changes,vertex_coordinates_unchanged=True,scope='Top sheet at reported tear; no terrain regrade, no overlay. Underwater faces and unrelated areas excluded.')
(D/'source-delta.json').write_text(json.dumps(report,indent=2))
bpy.ops.wm.save_as_mainfile(filepath=str(P));bpy.ops.wm.read_factory_settings(use_empty=True);bpy.ops.wm.open_mainfile(filepath=str(P))
o=bpy.data.objects['BankR91_Grade_017'];assert all(o.data.polygons[r['face']].normal.z>0 for r in changes)
missing=[im.filepath for im in bpy.data.images if im.source=='FILE' and not im.packed_file and im.filepath and not Path(bpy.path.abspath(im.filepath)).exists()]
(D/'source-reopen.json').write_text(json.dumps(dict(file=bpy.data.filepath,unit_scale=bpy.context.scene.unit_settings.scale_length,libraries=[l.filepath for l in bpy.data.libraries],missing_images=missing,faces_verified=len(changes)),indent=2))
assert not missing and not bpy.data.libraries
# Local source render, same camera before/after; geometry is actual source, culling enabled.
sc=bpy.context.scene;sc.render.engine='BLENDER_WORKBENCH';sc.render.resolution_x=1100;sc.render.resolution_y=750;sc.render.resolution_percentage=100;sc.display.shading.light='STUDIO';sc.display.shading.color_type='MATERIAL';sc.display.shading.show_backface_culling=True
for ob in sc.objects:
 if ob.type=='MESH':ob.hide_render=not bool(ob.get('dam99_actual_renderer'))
cam=bpy.data.objects.new('R101_LOCAL_SOURCE_REVIEW',bpy.data.cameras.new('R101Review'));sc.collection.objects.link(cam);cam.location=(-641.5,-412,3.2);target=Vector((-644.5,-415,.2));cam.rotation_euler=(target-cam.location).to_track_quat('-Z','Y').to_euler();cam.data.lens=28;cam.data.clip_start=.01;sc.camera=cam
sc.render.filepath=str(E/'blender-after.png');bpy.ops.render.render(write_still=True)
for r in changes:o.data.polygons[r['face']].flip()
o.data.update();sc.render.filepath=str(E/'blender-before.png');bpy.ops.render.render(write_still=True)
print('RESULT',json.dumps({k:v for k,v in report.items() if k!='changes'}),flush=True)
