import bpy,json,numpy as np
from pathlib import Path
R=Path('D:/ConRoi');D=R/'Test/KetQua/journey_v48/market_ground_r95';P=R/'Source/Blender/JourneyV48/R95/Market_Ground_R95_Editable.blend'
bpy.ops.wm.open_mainfile(filepath=str(P));out=[]
# Repair only the paired terrain support at the observed western branch.
# No coordinates, shoreline, grading weights or protected-bank heights change.
for o in bpy.data.collections['MARKET_GROUND_NATIVE_R95'].all_objects:
 key=o.get('market95_export_key')
 if o.get('unity_path') not in ['LakeGateMarketV32/Context_124','R20_Whole_Source_Export/Surface_176']:continue
 a=np.load(D/(key+'-source.npz'));v=a['v'];t=a['tri'].copy();q=v[t];nn=np.cross(q[:,1]-q[:,0],q[:,2]-q[:,0]);c=q.mean(1)
 # Native source uses a mirrored mapping: upward Blender faces have negative
 # mathematical Unity-coordinate winding. Repair the wrong orientation only
 # on gentle support faces near this branch, never vertical/bank side walls.
 wrong=(nn[:,1]>0)&(nn[:,1]/np.maximum(np.linalg.norm(nn,axis=1),1e-15)>.7)&(c[:,0]>=680)&(c[:,0]<=720)&(c[:,2]>=380)&(c[:,2]<=435)
 ids=np.flatnonzero(wrong)
 for i in ids:o.data.polygons[int(i)].flip()
 o.data.update();t[wrong]=t[wrong][:,::-1];data={k:a[k] for k in a.files};data['tri']=t
 if 'uv' in data:data['uv']=data['uv'].copy();data['uv'][wrong]=data['uv'][wrong][:,::-1]
 np.savez_compressed(D/(key+'-source.npz'),**data)
 p=D/(key+'-graded.npz');b=np.load(p);data={k:b[k] for k in b.files};tt=data['tri'].copy();qq=data['v'][tt];n=np.cross(qq[:,1]-qq[:,0],qq[:,2]-qq[:,0]);cc=qq.mean(1);bad=(n[:,1]>0)&(n[:,1]/np.maximum(np.linalg.norm(n,axis=1),1e-15)>.7)&(cc[:,0]>=680)&(cc[:,0]<=720)&(cc[:,2]>=380)&(cc[:,2]<=435);tt[bad]=tt[bad][:,::-1];data['tri']=tt
 if 'uv' in data:data['uv']=data['uv'].copy();data['uv'][bad]=data['uv'][bad][:,::-1]
 np.savez_compressed(p,**data);out.append(dict(key=key,path=o['unity_path'],flipped=len(ids),bounds_centres=[c[wrong].min(0).tolist(),c[wrong].max(0).tolist()] if len(ids) else [],position_changes=0))
bpy.context.preferences.filepaths.save_version=0;bpy.ops.wm.save_as_mainfile(filepath=str(P));(D/'winding-repair.json').write_text(json.dumps(out,indent=2));print('WINDING_REPAIR',out,flush=True)
