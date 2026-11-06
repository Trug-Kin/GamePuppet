import bpy,json,numpy as np,re
from pathlib import Path
R=Path('D:/ConRoi');D=R/'Test/KetQua/journey_v48/market_ground_r95'
catalog=json.loads((R/'Assets/Art/Environment/JourneyV48/ExportPolicyR20/surfaces.json').read_text())['meshes']
inventory={r['path']:r for r in json.loads((D/'inventory.json').read_text())['rows']}
ground={r['path'] for r in json.loads((D/'grading.json').read_text())['objects']}
selected=[r for r in catalog if (p:='R20_Whole_Source_Export/'+r['mesh']) in inventory and p not in ground and inventory[p]['active'] and inventory[p]['render']]
selected += [r for r in catalog if r['mesh']=='Surface_106' and r not in selected]
names=set(n for r in selected for n in r['sources'])
bpy.ops.wm.read_factory_settings(use_empty=True)
with bpy.data.libraries.load(str(R/'Docs/Reviews/lake_gate_market_current/ntmk_full_map_route_materials_v33.blend'),link=False) as (a,b):b.objects=sorted(names.intersection(a.objects))
for o in b.objects:bpy.context.scene.collection.objects.link(o)
bpy.context.view_layer.update();objects={o.name:o for o in b.objects};meta=[]
def group(name,o,centre):
 if name.startswith(('Gate_','GateV11_')):return 'BUILDING_GATE'
 if name.startswith(('Market_','MarketV','PhotoStudy_Market')):return 'BUILDING_MARKET'
 if name.startswith('F01_'):return 'BUILDING_F01'
 if name.startswith('TTC_'):return 'BUILDING_TTC'
 if name.startswith('E1V25_'):return 'BUILDING_E1'
 if name.startswith('EastShop_'):return 'BUILDING_EAST_QUAY'
 m=re.match(r'PROP28_(?:Light_)?(DD|Hien|Len|Suong)',name)
 if m:return 'STALL_'+m[1]
 m=re.match(r'(ParkV\d+_[A-Za-z]+\d*)_',name)
 if m:return m[1]
 for k in ['building_id','osm_building_id','tree_id','osm_id']:
  if k in o:return k+'_'+str(o[k])
 # Parts such as lamp shade, pole and planter in the same local cell use a
 # shared rigid placement, rather than a height warp through their vertices.
 return 'LOCAL_'+str(int(np.floor(centre[0]/5)))+'_'+str(int(np.floor(centre[2]/5)))
info={}
for name,o in objects.items():
 if o.type!='MESH':continue
 v=np.empty((len(o.data.vertices),3));o.data.vertices.foreach_get('co',v.ravel());m=np.array(o.matrix_world);v=v@m[:3,:3].T+m[:3,3];v=np.c_[-v[:,0],v[:,2],-v[:,1]];k=group(name,o,v.mean(0));info[name]=(v,k);meta.append(dict(name=name,group=k,min=v.min(0).tolist(),max=v.max(0).tolist(),collections=[c.name for c in o.users_collection],properties={k:str(o[k])[:120] for k in o.keys()}))
groups=sorted(set(k for v,k in info.values()));gid={k:i for i,k in enumerate(groups)}
for row in selected:
 vs=[];ids=[]
 for name in row['sources']:
  if name not in info:continue
  v,k=info[name];vs.append(v);ids.extend([gid[k]]*len(v))
 if vs:np.savez_compressed(D/('placement-native-'+row['mesh']+'.npz'),v=np.vstack(vs),group=np.array(ids,dtype='i4'))
(D/'placement-native-groups.json').write_text(json.dumps(dict(groups=groups,objects=meta),indent=2));print('NATIVE_PLACEMENT_ATTRIBUTION',len(info),len(groups),len(selected),flush=True)
