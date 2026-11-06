import bpy,json,numpy as np,sys
from pathlib import Path
R=Path('D:/ConRoi');D=R/'Test/KetQua/journey_v48/market_ground_r95';P=R/'Source/Blender/JourneyV48/R95/Market_Ground_R95_Editable.blend';bpy.ops.wm.open_mainfile(filepath=str(P))
for c in list(bpy.data.collections):c.hide_viewport=False
bpy.context.view_layer.update();rows=[];sys.path.insert(0,str(R/'.agents/skills/scenario-blender-expert/scripts'));import bx_audit
for o in list(bpy.data.collections['MARKET_GROUND_NATIVE_R95'].all_objects):
 key=o.get('market95_export_key')
 if not key:continue
 m=o.data;m.calc_loop_triangles();vv=np.array([v.co for v in m.vertices],float);mat=np.array(o.matrix_world);vv=vv@mat[:3,:3].T+mat[:3,3];world=np.c_[-vv[:,0],vv[:,2],-vv[:,1]];tri=np.array([f.vertices for f in m.loop_triangles]);a=np.load(D/(key+'-source.npz'));q=vv[tri];nn=np.cross(q[:,1]-q[:,0],q[:,2]-q[:,0]);zero=int((np.linalg.norm(nn,axis=1)<1e-10).sum());dups=len(tri)-len(np.unique(np.sort(tri,axis=1),axis=0));err=float(np.max(abs(world-a['v'])))
 row=dict(key=key,object=o.name,path=o.get('unity_path'),triangles=len(tri),vertices=len(vv),world_error=err,scale=list(o.scale),degenerate=zero,duplicate_faces=dups,uv_layers=[u.name for u in m.uv_layers],missing_material_slots=sum(x is None for x in m.materials),render_visible=not o.hide_render)
 if key.startswith('M95_F'):row['closed_foundation_gate']=bx_audit.deliverable([o],profile='game')
 rows.append(row)
out=dict(file=str(P),version=bpy.app.version_string,units=bpy.context.scene.unit_settings.scale_length,libraries=[l.filepath for l in bpy.data.libraries],missing_images=[i.filepath for i in bpy.data.images if i.source=='FILE' and not i.packed_file and not Path(bpy.path.abspath(i.filepath)).exists()],objects=rows,status='REOPEN_AUDIT_NOT_VISUAL_ACCEPTANCE');(D/'blender-reopen.json').write_text(json.dumps(out,indent=2,default=str));print('REOPEN_RESULT',len(rows),'degenerate',sum(r['degenerate'] for r in rows),'duplicates',sum(r['duplicate_faces'] for r in rows),'worldError',max(r['world_error'] for r in rows),'foundationErrors',[(r['object'],r['closed_foundation_gate']['errors']) for r in rows if 'closed_foundation_gate' in r and not r['closed_foundation_gate']['pass']],flush=True)
