import bpy,json,sys,numpy as np
from pathlib import Path
from mathutils import Vector
R=Path('D:/ConRoi');D=R/'Test/KetQua/journey_v48/r95_visual_review';M=R/'Test/BangChung/journey_v48/r95_visual_review'
P=R/'Source/Blender/JourneyV48/R96/Market_Ground_R96_Editable.blend'
# Recompute this one working derivative from immutable r95; never modify r94/r95.
bpy.ops.wm.open_mainfile(filepath=str(R/'Source/Blender/JourneyV48/R95/Market_Ground_R95_Editable.blend'))
print('VERSION',bpy.app.version_string,flush=True)
root=bpy.data.collections['MARKET_GROUND_NATIVE_R95'];parts=[];report=[]
for o in root.all_objects:
 if o.get('market95_export_key') not in ['M95_000','M95_001','M95_002','M95_012','M95_013','M95_014','M95_015','M95_016','M95_017','M95_018','M95_019','M95_020','M95_021','M95_022','M95_023']:continue
 mat=np.array(o.matrix_world,dtype=float);v=np.array([x.co for x in o.data.vertices],dtype=float)@mat[:3,:3].T+mat[:3,3];tri=np.array([list(p.vertices) for p in o.data.polygons]);q=v[tri];n=np.cross(q[:,1]-q[:,0],q[:,2]-q[:,0]);c=q.mean(1);u=np.c_[-c[:,0],c[:,2],-c[:,1]]
 wrong=(n[:,2]<-.7*np.linalg.norm(n,axis=1))&(u[:,0]>=680)&(u[:,0]<=755)&(u[:,2]>=435)&(u[:,2]<=505)
 ids=np.flatnonzero(wrong);parts.append((o,ids,v.copy()))
 report.append(dict(key=o['market95_export_key'],path=o['unity_path'],flipped=len(ids),scope_unity_xz=[680,755,435,505],changed_vertex_positions=0,area=float(np.linalg.norm(n[wrong],axis=1).sum()/2),centres_unity_min=u[wrong].min(0).tolist(),centres_unity_max=u[wrong].max(0).tolist()))
assert len(parts)==15 and all(r['flipped']>0 for r in report)
# Only render the actual native presentation layer; independent colliders stay hidden.
for col in bpy.data.collections:col.hide_viewport=False
sc=bpy.context.scene;sc.render.engine='BLENDER_WORKBENCH';sc.render.resolution_x=1200;sc.render.resolution_y=760;sc.render.resolution_percentage=100
sh=sc.display.shading;sh.light='STUDIO';sh.color_type='OBJECT';sh.show_shadows=False;sh.show_cavity=True;sh.show_backface_culling=True;sh.background_type='VIEWPORT';sh.background_color=(.12,.16,.2)
cam=bpy.data.objects.new('REVIEW_BRIDGE_APPROACH',bpy.data.cameras.new('REVIEW_BRIDGE_APPROACH'));sc.collection.objects.link(cam);sc.camera=cam;cam.data.lens=28
shots=[('bridge-eye',[699,2.7,483],[714,1.2,468]),('I2-wide',[760,38,505],[713,0.7,465])]
for state in ['before','after']:
 if state=='after':
  for o,ids,before in parts:
   for i in ids:o.data.polygons[int(i)].flip()
   o.data.update();mat=np.array(o.matrix_world,dtype=float);assert np.array_equal(before,np.array([x.co for x in o.data.vertices],dtype=float)@mat[:3,:3].T+mat[:3,3])
   o['review96_fix']='Gentle approach support face winding only, no coordinate/grade change; confirmed controller fall and top-down backface-only hit.'
  P.parent.mkdir(parents=True,exist_ok=True);sc['review96_scope']='Bridge-I2 open road, footway and terrain sheets, gentle support face winding only within X680..755 Z435..505; all r95 ground heights and placements retained; no bank flattening.'
  bpy.context.preferences.filepaths.save_version=0;bpy.ops.wm.save_as_mainfile(filepath=str(P))
 for tag,eye,at in shots:
  cam.location=(-eye[0],-eye[2],eye[1]);cam.rotation_euler=(Vector((-at[0],-at[2],at[1]))-cam.location).to_track_quat('-Z','Y').to_euler();sc.render.filepath=str(M/('blender-'+tag+'-'+state+'.png'));bpy.ops.render.render(write_still=True)
(D/'winding-source-fix.json').write_text(json.dumps(dict(source=str(P),unchanged_source='R95/Market_Ground_R95_Editable.blend',unit_scale=sc.unit_settings.scale_length,fixes=report),indent=2))
print('RESULT',json.dumps(report),flush=True)
