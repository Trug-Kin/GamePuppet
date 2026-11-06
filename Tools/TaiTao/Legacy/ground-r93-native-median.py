import bpy,json,numpy as np
from pathlib import Path
R=Path('D:/ConRoi');D=R/'Test/KetQua/journey_v48/ground_r93'
bpy.ops.wm.open_mainfile(filepath=str(R/'Docs/Reviews/lake_gate_market_current/ntmk_full_map_route_materials_v33.blend'))
rows=[]
for o in bpy.context.scene.objects:
 if o.type!='MESH':continue
 if not any(k in o.name.lower() for k in ['park','island','median','divider','planter','kerb','terrain','curb']):continue
 pts=np.array([o.matrix_world@v.co for v in o.data.vertices]);p=np.c_[-pts[:,0],pts[:,2],-pts[:,1]];lo=p.min(0);hi=p.max(0)
 if hi[0]<680 or lo[0]>820 or hi[2]<230 or lo[2]>450:continue
 rows.append(dict(name=o.name,bounds=[lo.tolist(),hi.tolist()],custom={k:str(o[k])[:500] for k in o.keys()}))
(D/'native-median-attribution.json').write_text(json.dumps(rows,indent=2));print('MEDIAN_CANDIDATES',len(rows));print(json.dumps([dict(name=r['name'],bounds=r['bounds']) for r in rows],indent=2))
