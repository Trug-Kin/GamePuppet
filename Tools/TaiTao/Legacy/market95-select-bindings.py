import bpy,json,numpy as np
from pathlib import Path
R=Path('D:/ConRoi');D=R/'Test/KetQua/journey_v48/market_ground_r95';P=R/'Source/Blender/JourneyV48/R95/Market_Ground_R95_Editable.blend';bpy.ops.wm.open_mainfile(filepath=str(P))
c=json.loads((D/'placement-candidates.json').read_text());by={r['path']:r for r in c['objects']};groups={r['group']:r for r in c['groups']};geometry=[];rigid={}
for o in list(bpy.data.collections['PLACEMENT_EDITABLE_PENDING_CONTACT'].all_objects):
 r=by.get(o.get('unity_path'))
 if not r:continue
 p=r['path'];delta=r['delta_range'];aggregate=p.count('/')==1 and (p.startswith('R20_Whole_Source_Export/Surface_') or p.startswith('LakeGateMarketV32/Context_'))
 variable=abs(delta[1]-delta[0])>.0001
 if aggregate or variable:
  geometry.append(dict(**r,object=o.name));continue
 o['market95_export_key']='';dy=(delta[0]+delta[1])*.5
 if p.startswith('R21_'):
  depth=3 if '/Grounded_Cypress_Boundary/' in p else 2;root='/'.join(p.split('/')[:depth])
 elif p.startswith('R22_'):root=p.split('/')[0]
 elif p.startswith('R20_'):root='/'.join(p.split('/')[:2])
 else:root=p
 if root in rigid:assert abs(rigid[root]['delta_y']-dy)<.001,(root,rigid[root]['delta_y'],dy)
 else:rigid[root]=dict(path=root,delta_y=dy,authoring_collection='PLACEMENT_EDITABLE_PENDING_CONTACT',mesh_shape_unchanged=True)
 o['market95_rigid_root']=root;o['market95_rigid_delta_y']=dy
# A variable lowest foundation in a root is already authored in world space;
# bind it after moving that rigid root, avoiding a second translation.
(D/'placement-geometry-bindings.json').write_text(json.dumps(geometry,indent=2));(D/'rigid-placement-bindings.json').write_text(json.dumps(list(rigid.values()),indent=2));bpy.context.preferences.filepaths.save_version=0;bpy.ops.wm.save_as_mainfile(filepath=str(P));print('BINDING_SELECTION',len(geometry),'geometry',len(rigid),'rigid assemblies',flush=True)
