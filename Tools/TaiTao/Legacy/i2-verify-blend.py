import bpy,json,numpy as np
from pathlib import Path
R=Path('D:/ConRoi');D=R/'Test/KetQua/journey_v48/i2_r93_complete';P=R/'Source/Blender/JourneyV48/R93/Ground_I2_R93_Editable.blend';bpy.ops.wm.open_mainfile(filepath=str(P));sc=bpy.context.scene;rows=[]
for c in bpy.data.collections:c.hide_viewport=False
bpy.context.view_layer.update()
for o in bpy.data.collections['GROUND_NATIVE_MATCHED_R93'].all_objects:
 if not o.get('i2_export_key'):continue
 m=o.data;m.calc_loop_triangles();local=np.array([p.co for p in m.vertices],dtype=float);mat=np.array(o.matrix_world,dtype=float);v=local@mat[:3,:3].T+mat[:3,3];u=np.c_[-v[:,0],v[:,2],-v[:,1]];t=np.array([f.vertices for f in m.loop_triangles]);a=np.load(D/(o['i2_export_key']+'-source.npz'));q=v[t];nn=np.cross(q[:,1]-q[:,0],q[:,2]-q[:,0]);cent=u[t].mean(1);r=np.linalg.norm(cent[:,[0,2]]-[714.94,459.48],axis=1);sel=(r<51)&(cent[:,2]>452.48)
 ids=np.flatnonzero(sel&(nn[:,2]<-1e-10));ids=ids[np.argsort(abs(nn[ids,2]))[-3:]]
 rows.append(dict(object=o.name,path=o.get('unity_path'),vertices=len(v),triangles=len(t),world_error=float(np.max(abs(u-a['v']))),scale=list(o.scale),pivot=list(o.location),wrong_winding_local=int((nn[sel,2]<-1e-10).sum()),wrong_worst=[dict(area=float(abs(nn[i,2])/2),xyz=u[t[i]].tolist()) for i in ids],zero_area_local=int((np.linalg.norm(nn[sel],axis=1)<1e-10).sum()),uv_layers=[x.name for x in m.uv_layers],missing_materials=[i for i,m in enumerate(m.materials) if m is None]))
result=dict(file=str(P),version=bpy.app.version_string,scene=sc.name,units=sc.unit_settings.scale_length,libraries=[l.filepath for l in bpy.data.libraries],missing_images=[i.filepath for i in bpy.data.images if i.source=='FILE' and not i.packed_file and not Path(bpy.path.abspath(i.filepath)).exists()],objects=rows,status='GEOMETRY_REOPEN_AUDIT_ONLY_NOT_VISUAL_ACCEPTANCE');(D/'blender-reopen.json').write_text(json.dumps(result,indent=2));print(json.dumps(result,indent=2))
