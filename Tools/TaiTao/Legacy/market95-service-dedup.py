import bpy,json,numpy as np
from pathlib import Path
R=Path('D:/ConRoi');D=R/'Test/KetQua/journey_v48/market_ground_r95';P=R/'Source/Blender/JourneyV48/R95/Market_Ground_R95_Editable.blend';bpy.ops.wm.open_mainfile(filepath=str(P));log=[]
for o in list(bpy.data.collections['MARKET_GROUND_NATIVE_R95'].all_objects):
 key=o.get('market95_export_key','')
 if not key.startswith('M95_Service'):continue
 a=np.load(D/(key+'-source.npz'));data={k:a[k] for k in a.files};t=a['tri'];_,ix=np.unique(np.sort(t,axis=1),axis=0,return_index=True);ix.sort();old=o.data;local=np.array([v.co[:] for v in old.vertices]);m=bpy.data.meshes.new(key+'_Final');m.from_pydata(local.tolist(),[],t[ix].tolist());m.update()
 for mat in old.materials:m.materials.append(mat)
 for p,s in zip(m.polygons,a['slots'][ix]):p.material_index=int(s);p.use_smooth=abs(p.normal.z)>.6
 u=m.uv_layers.new(name='UVMap')
 for i,uv in enumerate(a['uv'][ix].reshape(-1,2)):u.data[i].uv=uv
 u=m.uv_layers.new(name='SourceIndex')
 for l in m.loops:u.data[l.index].uv=(l.vertex_index,0)
 o.data=m;data['tri']=t[ix];data['uv']=a['uv'][ix];data['slots']=a['slots'][ix];np.savez_compressed(D/(key+'-source.npz'),**data);log.append(dict(key=key,removed_duplicate=len(t)-len(ix)))
(D/'service-duplicate-closure.json').write_text(json.dumps(log,indent=2));bpy.context.preferences.filepaths.save_version=0;bpy.ops.wm.save_as_mainfile(filepath=str(P));print('SERVICE_DEDUP',log)
