import bpy,json,sys,numpy as np
from pathlib import Path
from mathutils import Vector,kdtree
R=Path('D:/ConRoi');D=R/'Test/KetQua/journey_v48/market_ground_r95';E=R/'Test/Output/journey_v48/MarketGroundR95';E.mkdir(parents=True,exist_ok=True)
assert json.loads((D/'blender-acceptance.json').read_text())['accepted'], 'Blender gate not accepted'
sys.path.insert(0,str(R/'.agents/skills/scenario-blender-export/scripts'));import bx_export as bxe
bpy.ops.wm.open_mainfile(filepath=str(R/'Source/Blender/JourneyV48/R95/Market_Ground_R95_Editable.blend'))
for c in bpy.data.collections:c.hide_viewport=False
bpy.context.view_layer.update();settings=[];parts=[]
for o in list(bpy.data.collections['MARKET_GROUND_NATIVE_R95'].all_objects):
 if not o.get('market95_export_key'):continue
 o.hide_set(False);o.hide_viewport=False
 if '--roundtrip-only' not in sys.argv and ('--service-only' not in sys.argv or o['market95_export_key'].startswith('M95_Service')) and ('--winding-only' not in sys.argv or o['market95_export_key'] in ['M95_016','M95_023']):settings.append(bxe.export(str(E/(o['market95_export_key']+'.fbx')),'unity',objects=[o],embed_textures=False,path_mode='AUTO'))
 parts.append(dict(name=o.name,key=o['market95_export_key'],triangles=len(o.data.polygons)))
(D/'export-settings.json').write_text(json.dumps(settings,indent=2,default=str));out=[]
for part in parts:
 bpy.ops.wm.read_factory_settings(use_empty=True);bpy.ops.import_scene.fbx(filepath=str(E/(part['key']+'.fbx')));bpy.context.view_layer.update();oo=[o for o in bpy.data.objects if o.type=='MESH'];assert len(oo)==1;ob=oo[0];a=np.load(D/(part['key']+'-source.npz'));expected=np.c_[-a['v'][:,0],-a['v'][:,2],a['v'][:,1]];kd=kdtree.KDTree(len(expected))
 for i,v in enumerate(expected):kd.insert(Vector(v),i)
 kd.balance();err=max(kd.find(ob.matrix_world@v.co)[2] for v in ob.data.vertices);ob.data.calc_loop_triangles();nt=len(ob.data.loop_triangles);out.append(dict(key=part['key'],vertices=len(ob.data.vertices),triangles=nt,expected_triangles=len(a['tri']),world_max_error=err,scale=list(ob.scale),uv_layers=[u.name for u in ob.data.uv_layers]));(D/'fbx-roundtrip.json').write_text(json.dumps(out,indent=2));print('ROUNDTRIP',out[-1],flush=True)
(D/'fbx-roundtrip.json').write_text(json.dumps(out,indent=2));assert all(r['world_max_error']<.001 and r['triangles']==r['expected_triangles'] for r in out), 'Roundtrip mismatch; inspect fbx-roundtrip.json';print('FBX_ROUNDTRIP_ACCEPTED',len(out),flush=True)
