import bpy,json,numpy as np
from pathlib import Path
R=Path('D:/ConRoi');D=R/'Test/KetQua/journey_v48/market_ground_r95'
s=json.loads((R/'Assets/Art/Environment/JourneyV48/ExportPolicyR20/surfaces.json').read_text())['meshes']
names=['PhotoStudy_MarketMainHall_RevisedProportions','F01_MainWhiteMass','TTC_MainWhiteMass','ParkV19_B1_Foliage']
names+= [n for r in s if r['mesh'] in ['Surface_064','Surface_070','Surface_138','Surface_084','Surface_008'] for n in r['sources'][:3]]
bpy.ops.wm.read_factory_settings(use_empty=True)
with bpy.data.libraries.load(str(R/'Docs/Reviews/lake_gate_market_current/ntmk_full_map_route_materials_v33.blend'),link=False) as (a,b): b.objects=[n for n in set(names) if n in a.objects]
out=[]
for o in b.objects:
 bpy.context.scene.collection.objects.link(o)
bpy.context.view_layer.update()
for o in b.objects:
 if o.type!='MESH':continue
 m=np.array(o.matrix_world);v=np.array([p.co for p in o.data.vertices])@m[:3,:3].T+m[:3,3];v=np.c_[-v[:,0],v[:,2],-v[:,1]];out.append(dict(name=o.name,min=v.min(0).tolist(),max=v.max(0).tolist()))
(D/'native-contact-read.json').write_text(json.dumps(out,indent=2));print(json.dumps(out),flush=True)
