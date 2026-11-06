import bpy,json,math
from pathlib import Path
from mathutils import Vector
import numpy as np
R=Path('D:/ConRoi');D=R/'Test/KetQua/journey_v48/ground_r93';M=R/'Test/BangChung/journey_v48/ground_r93';P=R/'Source/Blender/JourneyV48/R93/Ground_I2_Transition_Trial_R93.blend'
bpy.ops.wm.open_mainfile(filepath=str(P));sc=bpy.context.scene;col=bpy.data.collections['GROUND_NATIVE_MATCHED_R93'];out=[]
for o in col.objects:
 if o.type!='MESH':continue
 m=o.data;m.calc_loop_triangles();v=np.array([o.matrix_world@v.co for v in m.vertices]);tri=np.array([t.vertices for t in m.loop_triangles]);q=v[tri];cross=np.cross(q[:,1]-q[:,0],q[:,2]-q[:,0]);area=np.linalg.norm(cross,axis=1)/2
 r=dict(object=o.name,path=o.get('unity_path',''),scale=list(o.scale),vertices=len(v),triangles=len(tri),degenerate=int((area<1e-10).sum()),uv_channels=[u.name for u in m.uv_layers],missing_material_slots=sum(m is None for m in m.materials),changed=o.get('trial_changed_vertices',0));out.append(r)
 if r['changed']:np.savez_compressed(D/('trial-'+o.name+'.npz'),v=np.c_[-v[:,0],v[:,2],-v[:,1]],tri=tri)
linked=[l.filepath for l in bpy.data.libraries];images=[dict(name=i.name,path=i.filepath,packed=bool(i.packed_file),missing=not i.packed_file and bool(i.filepath) and not Path(bpy.path.abspath(i.filepath)).exists()) for i in bpy.data.images if i.source=='FILE']
sc.display.shading.background_type='VIEWPORT';sc.display.shading.background_color=(.075,.085,.1);sc.display.shading.color_type='OBJECT';sc.render.resolution_x=1400;sc.render.resolution_y=1000;cam=sc.camera;cam.data.type='ORTHO';cam.data.ortho_scale=58;cam.location=(-688,-491,20);target=Vector((-714.94,-459.48,.7));cam.rotation_euler=(target-cam.location).to_track_quat('-Z','Y').to_euler();sc.render.filepath=str(M/'I2-trial-oblique-after.png');bpy.ops.render.render(write_still=True)
# Restore exact before geometry in the temporary in-memory review only; saved editable stays unchanged.
before=bpy.data.collections['BEFORE_R93_DO_NOT_EXPORT'];before.hide_render=False
changed=[o for o in col.objects if o.get('trial_changed_vertices',0)]
for o in changed:o.hide_render=True
for o in before.objects:o.hide_render=o.get('unity_path','').startswith('LakeGateMarketV32/Context_');o.color=(.55,.63,.7,1) if 'Surface_128' in o.get('unity_path','') else (.35,.43,.32,1)
sc.render.filepath=str(M/'I2-trial-oblique-before.png');bpy.ops.render.render(write_still=True)
(D/'trial-reopen-audit.json').write_text(json.dumps(dict(reopened=str(P),linked_libraries=linked,images=images,objects=out,status='Fresh process reopened; diagnostic source only, not a Unity acceptance'),indent=2));print('REOPEN_AUDIT',len(out),'objects; libraries',len(linked),'missing images',sum(i['missing'] for i in images),flush=True)
