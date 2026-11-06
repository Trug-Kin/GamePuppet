import bpy,json,numpy as np
from pathlib import Path
from mathutils import Vector
R=Path('D:/ConRoi');D=R/'Test/KetQua/journey_v48/ground_r93'
p=R/'Docs/Reviews/lake_gate_market_current/ntmk_full_map_route_materials_v33.blend'
bpy.ops.wm.open_mainfile(filepath=str(p));sc=bpy.context.scene;out={'source':str(p),'scene_custom':{k:str(sc[k]) for k in sc.keys()},'world_custom':{k:str(sc.world[k]) for k in sc.world.keys()} if sc.world else {},'objects':[]}
for o in list(sc.objects):
 if o.type!='MESH' or not(any(x in o.name for x in ['Core_Terrain_UTM','Local_Terrain_UTM','Island_Manual','Core_Road_secondary_W1226367858','Local_Road_secondary_W1226367858'])):continue
 v=np.array([o.matrix_world@v.co for v in o.data.vertices]);r={'name':o.name,'min':v.min(0).tolist(),'max':v.max(0).tolist(),'scale':list(o.scale),'custom':{k:str(o[k]) for k in o.keys()}};out['objects'].append(r)
 if o.name=='Core_Terrain_UTM49N_Graded40m':
  o.data.calc_loop_triangles();tri=np.array([t.vertices for t in o.data.loop_triangles]);np.savez_compressed(D/'native-v33-terrain.npz',v=np.c_[-v[:,0],v[:,2],-v[:,1]],tri=tri)
 for k in list(o.keys()):
  if any(w in k.lower() for w in ['origin','utm','north','lon','lat']):print(o.name,k,str(o[k])[:400])
(D/'native-trace.json').write_text(json.dumps(out,indent=2));print('RESULT',json.dumps(out)[:3500])
