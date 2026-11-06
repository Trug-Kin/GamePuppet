import bpy,sys,json,shutil,bmesh
from pathlib import Path
R=Path('D:/ConRoi');p=R/'Source/Blender/JourneyV48/R102/Workshop_Investigation_R102_Editable.blend';backup=p.with_name('Workshop_R102_Before_Headroom.blend');assert not backup.exists();shutil.copy2(p,backup)
bpy.ops.wm.open_mainfile(filepath=str(p));room=bpy.data.objects['WorkshopShell'];assert room.type=='MESH';points=[room.matrix_world@v.co for v in room.data.vertices];assert 4.9<max(v.z for v in points)<5.1
changed=[]
for v in room.data.vertices:
 w=room.matrix_world@v.co
 if w.z>=4.78:
  w.z+=.5;v.co=room.matrix_world.inverted()@w;changed.append(v.index)
bm=bmesh.new();bm.from_mesh(room.data);bmesh.ops.recalc_face_normals(bm,faces=list(bm.faces));bm.to_mesh(room.data);bm.free();room.data.update();room['headroom_fix']='Top wall/lintel vertices and full beams +0.5 Unity/Blender vertical; loft/rail/mechanism unchanged';bpy.ops.wm.save_as_mainfile(filepath=str(p));bpy.ops.wm.open_mainfile(filepath=str(p));room=bpy.data.objects['WorkshopShell'];assert not bpy.data.libraries
sys.path.append(str(R/'.agents/skills/scenario-blender-export/scripts'));import bx_export
result=bx_export.export(str(R/'Assets/Art/InvestigationV36/R102/WORKSHOP.fbx'),'unity',objects=list(bpy.data.collections['WORKSHOP'].objects));rt=bx_export.round_trip(str(R/'Assets/Art/InvestigationV36/R102/WORKSHOP.fbx'),list(bpy.data.collections['WORKSHOP'].objects),preset='unity');scene=bpy.context.scene;cam=scene.camera;from mathutils import Vector
cam.location=(-13,16,11);cam.rotation_euler=(Vector((0,0,2))-cam.location).to_track_quat('-Z','Y').to_euler();scene.render.filepath=str(R/'Test/BangChung/investigation_r102/blender-headroom-final.png');bpy.ops.render.render(write_still=True)
(R/'Test/KetQua/investigation_r102/headroom-source.json').write_text(json.dumps(dict(changed_vertex_count=len(changed),delta_y=.5,source=str(p),backup=str(backup),reopened=True,roundtrip=rt,export=result),default=str,indent=2));print('HEADROOM SOURCE REOPEN/ROUNDTRIP DONE')
