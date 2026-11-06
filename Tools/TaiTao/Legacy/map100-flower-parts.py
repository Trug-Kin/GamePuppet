import bpy,numpy as np,json
from pathlib import Path
R=Path('D:/ConRoi');D=R/'Test/KetQua/journey_v48/map_r99_closeout';bpy.ops.wm.open_mainfile(filepath=str(R/'Source/Blender/JourneyV48/R99/OngDao_Dam_R99_SectionCandidate.blend'))
A=np.array([637.7344,528.2603]);T=np.array([.76346655,-.64584737]);N=np.array([-.64584737,-.76346655]);out=[]
for color in ['Red','Yellow']:
 parts=[]
 for part in range(3):
  o=bpy.data.objects['D99_FLOWERS_'+color+'_'+str(part)];v=np.array([o.matrix_world@p.co for p in o.data.vertices]);attr=np.array([a.value for a in o.data.attributes['r22_plant_index'].data]);parts.append({int(i):v[attr==i] for i in np.unique(attr)})
 for ix,q in parts[0].items():
  sd=(-q[:,:2].mean(0)-A)@np.c_[T,N]
  if not 53<sd[0]<72 or not -3<sd[1]<6:continue
  out.append(dict(color=color,index=ix,parts=[dict(part=j,sd=(( -p[ix][:,:2].mean(0)-A)@np.c_[T,N]).tolist(),min=p[ix].min(0).tolist(),max=p[ix].max(0).tolist()) for j,p in enumerate(parts)]))
(D/'gate-flower-part-bounds.json').write_text(json.dumps(out,indent=2));print('RESULT',json.dumps(out[:8]),flush=True)
