import bpy,json,numpy as np
from pathlib import Path
R=Path('D:/ConRoi');src=R/'Docs/Reviews/lake_gate_market_current/ntmk_full_map_route_materials_v33.blend';bpy.ops.wm.read_factory_settings(use_empty=True)
with bpy.data.libraries.load(str(src),link=False) as (a,b):
 names=[n for n in a.objects if any(k in n.lower() for k in ['_steps_','island','median','roundabout'])];b.objects=names
for o in b.objects:
 if o:bpy.context.scene.collection.objects.link(o)
bpy.context.view_layer.update();out=[]
for o in b.objects:
 if not o or o.type!='MESH':continue
 m=np.array(o.matrix_world);v=np.array([p.co for p in o.data.vertices],float)@m[:3,:3].T+m[:3,3];u=np.c_[-v[:,0],v[:,2],-v[:,1]];out.append(dict(name=o.name,min=u.min(0).tolist(),max=u.max(0).tolist(),vertices=len(v),properties={k:str(o[k])[:250] for k in o.keys()}))
 if '_steps_' in o.name:
  o.data.calc_loop_triangles();np.savez_compressed(R/'Test/KetQua/journey_v48/market_ground_r95'/('structural-'+o.name+'.npz'),v=u,tri=np.array([p.vertices for p in o.data.loop_triangles]))
(R/'Test/KetQua/journey_v48/market_ground_r95/native-structural-exclusions.json').write_text(json.dumps(out,indent=2));print(json.dumps(out,indent=2),flush=True)
