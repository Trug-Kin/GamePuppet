import bpy,json,numpy as np
from pathlib import Path
from mathutils import Matrix,Vector
R=Path('D:/ConRoi');D=R/'Test/KetQua/journey_v48/market_ground_r95';P=R/'Source/Blender/JourneyV48/R95/Market_Ground_R95_Editable.blend'
bpy.ops.wm.open_mainfile(filepath=str(P));root=bpy.data.collections['MARKET_GROUND_NATIVE_R95']
for c in bpy.data.collections:c.hide_viewport=False
bpy.context.view_layer.update();g=json.loads((D/'grading.json').read_text());log=[];origin=np.array([-750.,-300.,0.]);visible={r['path']:r['render'] for r in json.loads((D/'inventory.json').read_text())['rows']}
for row in g['objects']:
 o=bpy.data.objects.get(row['object']);assert o and o.get('unity_path')==row['path'];a=np.load(D/(row['key']+'-graded.npz'));v=a['v'];t=a['tri'];uv=a['uv'];slots=a['slots'];old=o.data;m=bpy.data.meshes.new(row['key']);local=np.c_[-v[:,0],-v[:,2],v[:,1]]-origin
 # Tiny topology fragments: keep source precision but discard zero-area/duplicate faces.
 local=local.astype('f4');q=local[t].astype(float);normal=np.cross(q[:,1]-q[:,0],q[:,2]-q[:,0]);valid=np.linalg.norm(normal,axis=1)>1e-9;t=t[valid];uv=uv[valid];slots=slots[valid];n=normal[valid];flip=(n[:,2]<-1e-9)&(abs(n[:,2])<1e-5);t[flip]=t[flip][:,::-1];uv[flip]=uv[flip][:,::-1]
 m.from_pydata(local.tolist(),[],t.tolist());m.update();o.data=m;o.matrix_world=Matrix.Translation(origin.tolist());o.scale=(1,1,1)
 for mat in old.materials:m.materials.append(mat)
 for f,slot in zip(m.polygons,slots):
  f.material_index=min(int(slot),max(0,len(m.materials)-1))
  if '/MarketIsland' not in row['path'] and not row['key'].startswith('M95_F'):f.use_smooth=abs(f.normal.z)>.6
 u=m.uv_layers.new(name='UVMap')
 for i,co in enumerate(uv.reshape(-1,2)):u.data[i].uv=co
 u=m.uv_layers.new(name='SourceIndex')
 for loop in m.loops:u.data[loop.index].uv=(loop.vertex_index,0)
 o['market95_export_key']=row['key'];o['market95_status']='BLENDER_GATE_PENDING';o['market95_design']='User-authorized game grade, not field elevation reconstruction';o.hide_set(False);o.hide_render=not visible.get(row['path'],False)
 actual=local.astype(float)+origin;vv=np.c_[-actual[:,0],actual[:,2],-actual[:,1]];np.savez_compressed(D/(row['key']+'-source.npz'),v=vv,tri=t,before=a['before'],uv=uv,slots=slots)
 log.append(dict(path=row['path'],key=row['key'],vertices=len(vv),triangles=len(t),removed_degenerate=int((~valid).sum()),micro_winding_repaired=int(flip.sum()),unit_scale=list(o.scale)))
# Prevent inherited context terrain or comparison copies from becoming replacement overlays.
lc=bpy.data.collections['LANGFARM_NATIVE_FOUNDATIONS'];foundation=[]
for o in list(lc.all_objects):
 if o.type!='MESH':o.hide_render=True;continue
 v=np.array([p.co for p in o.data.vertices],float);mat=np.array(o.matrix_world);v=v@mat[:3,:3].T+mat[:3,3];p=np.c_[-v[:,0],v[:,2],-v[:,1]]
 foundation.append(dict(name=o.name,bounds=[p.min(0).tolist(),p.max(0).tolist()],custom={k:str(o[k])[:100] for k in o.keys()}));o.hide_render=not (o.name.startswith('R93_Render') or o.name.startswith('R93_SOLID'))
(D/'foundation-native-info.json').write_text(json.dumps(foundation,indent=2));(D/'blender-saved.json').write_text(json.dumps(log,indent=2))
sc=bpy.context.scene;sc.name='MARKET_GROUND_R95_GAME_DESIGN';sc['status']='EDITABLE_GRADED_PENDING_VISUAL_AND_TOPOLOGY_GATE';sc['design_requirement']='2026-11-02 user explicitly authorizes game-flat continuous gate/branches/M2/market/plaza/Langfarm; no field-height claim; do not restore old 11% slope.'
txt=bpy.data.texts.get('MARKET_R95_README') or bpy.data.texts.new('MARKET_R95_README');txt.clear();txt.write('Open MARKET_GROUND_NATIVE_R95. Editable native source, game grade design explicitly authorized by user. ROAD/WALK/terrain meshes retain unity_path and market95_export_key; independent collider objects are hidden from render. Existing I2 r94/bridge and lake-bank/structural-step exclusions remain. LANGFARM_NATIVE_FOUNDATIONS is context awaiting foundation verification. No export/Unity acceptance yet. Source baseline/checkpoint r92/r93/r94 remain immutable. Test/KetQua/journey_v48/market_ground_r95 records target field, per-layer sections, topology and source lineage.\n')
bpy.context.preferences.filepaths.save_version=0;bpy.ops.wm.save_as_mainfile(filepath=str(P));print('SAVED_MARKET95_EDITABLE',len(log),flush=True)
