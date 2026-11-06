import bpy,json,numpy as np
from pathlib import Path
from mathutils import Vector
R=Path('D:/ConRoi');D=R/'Test/KetQua/journey_v48/market_ground_r95';M=R/'Test/BangChung/journey_v48/market_ground_r95'
bpy.ops.wm.open_mainfile(filepath=str(R/'Source/Blender/JourneyV48/R95/Market_Ground_R95_Editable.blend'));sc=bpy.context.scene
for c in list(sc.collection.children):c.hide_render=c.name!='MARKET_GROUND_NATIVE_R95'
rows=[];palette={};cols=[(.8,.1,.1,1),(.1,.8,.1,1),(.1,.1,.8,1),(.8,.8,.1,1),(.8,.1,.8,1),(.1,.8,.8,1),(.9,.5,.1,1)]
for o in list(bpy.data.collections['MARKET_GROUND_NATIVE_R95'].all_objects):
 if o.type!='MESH' or o.hide_render:continue
 mat=np.array(o.matrix_world);v=np.array([p.co for p in o.data.vertices])@mat[:3,:3].T+mat[:3,3];v=np.c_[-v[:,0],v[:,2],-v[:,1]]
 if v[:,0].max()<750 or v[:,0].min()>820 or v[:,2].max()<155 or v[:,2].min()>225:continue
 path=o.get('unity_path','');name=path.split('/')[-1];role='ISLAND' if 'MarketIsland_R62' in path else (name or o.name)
 if role not in palette:palette[role]=cols[len(palette)%len(cols)]
 o.color=palette[role];rows.append(dict(object=o.name,path=path,export_key=o.get('market95_export_key',''),role=role,color=palette[role],bounds=[v.min(0).tolist(),v.max(0).tolist()]))
cam=bpy.data.objects.new('ATTRIBUTION',bpy.data.cameras.new('ATTRIBUTION'));sc.collection.objects.link(cam);sc.camera=cam;eye=[798,7.8,231];at=[785,6,188];cam.location=(-eye[0],-eye[2],eye[1]);cam.rotation_euler=(Vector((-at[0],-at[2],at[1]))-cam.location).to_track_quat('-Z','Y').to_euler();cam.data.lens=26
sc.render.engine='BLENDER_WORKBENCH';sc.render.resolution_x=1280;sc.render.resolution_y=800;sc.render.resolution_percentage=100;sc.display.shading.light='STUDIO';sc.display.shading.color_type='OBJECT';sc.display.shading.show_shadows=False;sc.display.shading.show_cavity=False;sc.render.filepath=str(M/'plaza-attribution.png');bpy.ops.render.render(write_still=True)
(D/'plaza-render-attribution.json').write_text(json.dumps(dict(palette=palette,objects=rows),indent=2));print('ATTRIBUTION',palette,flush=True)
