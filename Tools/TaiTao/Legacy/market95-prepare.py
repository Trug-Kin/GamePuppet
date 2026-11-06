import bpy,json,numpy as np
from pathlib import Path
from mathutils import Matrix
R=Path('D:/ConRoi');D=R/'Test/KetQua/journey_v48/market_ground_r95';S=R/'Source/Blender/JourneyV48/R95';S.mkdir(parents=True,exist_ok=True)
bpy.ops.wm.open_mainfile(filepath=str(R/'Source/Blender/JourneyV48/R93/Ground_I2_R93_Editable.blend'))
sc=bpy.context.scene;root=bpy.data.collections['GROUND_NATIVE_MATCHED_R93'];root.name='MARKET_GROUND_NATIVE_R95'
for c in bpy.data.collections:c.hide_viewport=False
# Extra original native road objects, not Unity review geometry.
surf=json.loads((R/'Assets/Art/Environment/JourneyV48/ExportPolicyR20/surfaces.json').read_text())['meshes']
extra={'Surface_127','Surface_174','Surface_175'}
for info in [m for m in surf if m['mesh'] in extra]:
 with bpy.data.libraries.load(str(R/'Docs/Reviews/lake_gate_market_current/ntmk_full_map_route_materials_v33.blend'),link=False) as (a,b):b.objects=list(info['sources'])
 vv=[];tt=[];uv=[];base=0;mats=[]
 for o in b.objects:
  root.objects.link(o);o.data.calc_loop_triangles();v=np.array([o.matrix_world@p.co for p in o.data.vertices]);tris=np.array([t.vertices for t in o.data.loop_triangles]);vv.extend(v.tolist());tt.extend((tris+base).tolist());base+=len(v);mats.extend(list(o.data.materials));bpy.data.objects.remove(o,do_unlink=True)
 m=bpy.data.meshes.new('M95_'+info['mesh']);m.from_pydata(vv,[],tt);m.update();o=bpy.data.objects.new(info['mesh']+'_Native',m);root.objects.link(o)
 for mat in mats[:1]:m.materials.append(mat)
 o['unity_path']='R20_Whole_Source_Export/'+info['mesh'];o['authoring_source']='Docs/Reviews/lake_gate_market_current/ntmk_full_map_route_materials_v33.blend';o['source_objects']=';'.join(info['sources'])
# Market island/curbs were graded in r88 and remain editable in r91 source.
audit=json.loads((R/'Test/KetQua/journey_v48/ground_r93/editable-source-audit.json').read_text())[0]
pick=[o for o in audit['objects'] if '/MarketIsland_R62/' in o['path'] or o['path'].endswith('/Context_130') or o['path'].endswith('/Context_133')]
with bpy.data.libraries.load(str(R/audit['source']),link=False) as (a,b):b.objects=[o['name'] for o in pick]
island=bpy.data.collections.new('MARKET_ISLAND_NATIVE');root.children.link(island)
for row,o in zip(pick,b.objects):
 if o: island.objects.link(o);o['unity_path']=row['path'];o['authoring_source']=audit['source'];o.hide_render=False;o.hide_set(False)
# Native curb collider counterpart; keep it explicit for source reconciliation.
with bpy.data.libraries.load(str(R/'Source/Blender/JourneyV48/R91/north-bank-r91-extra-final.blend'),link=False) as (a,b):b.objects=['BankR91_Grade_037']
o=b.objects[0];root.objects.link(o);o['unity_path']='LakeGateMarketV32/Context_132';o['authoring_source']='Source/Blender/JourneyV48/R91/north-bank-r91-extra-final.blend'
# Langfarm source, keep all editable parts as context and foundation candidates.
with bpy.data.libraries.load(str(R/'Source/Blender/JourneyV48/R93/Langfarm_R93_Editable.blend'),link=False) as (a,b):b.objects=[n for n in a.objects if not n.startswith('R93_BEFORE')]
lc=bpy.data.collections.new('LANGFARM_NATIVE_FOUNDATIONS');root.children.link(lc)
for o in b.objects:
 if not o:continue
 lc.objects.link(o);o['market95_context_only']=True;o.hide_render=False
# Dump complete world native data. Existing I2 remains baseline and is not regraded.
bpy.context.view_layer.update();rows=[]
for i,o in enumerate(list(root.all_objects)):
 if o.type!='MESH' or not o.get('unity_path'):continue
 me=o.data;me.calc_loop_triangles();v=np.array([x.co for x in me.vertices],dtype=float);mat=np.array(o.matrix_world);v=v@mat[:3,:3].T+mat[:3,3];p=np.c_[-v[:,0],v[:,2],-v[:,1]];t=np.array([f.vertices for f in me.loop_triangles],dtype='i4');loops=np.array([f.loops for f in me.loop_triangles],dtype='i4');uv=np.array([list(v.uv) for v in me.uv_layers[0].data]) if me.uv_layers else np.zeros((len(me.loops),2));key='M95_'+str(i).zfill(3)
 np.savez_compressed(D/(key+'-native.npz'),v=p,tri=t,uv=uv[loops],slots=np.array([f.material_index for f in me.loop_triangles],dtype='i4'))
 rows.append(dict(key=key,object=o.name,path=o['unity_path'],source=o.get('authoring_source',''),bounds=[p.min(0).tolist(),p.max(0).tolist()]))
(D/'native-input.json').write_text(json.dumps(dict(objects=rows,units='1 authoring metre = 1 Blender = 1 Unity unit; game grade design, not field elevations'),indent=2))
sc['market95_requirement']='User-authorized continuous game grade; flatten gate branches, splitter/M2 context, market ring/plaza and TTC-Langfarm apron. Field-elevation uncertainty must not preserve 11 percent cross slope. Preserve r94 I2, bridge, lake banks, dam and structural steps.'
sc['market95_status']='EDITABLE_PREPARATION_NOT_ACCEPTED'
bpy.context.preferences.filepaths.save_version=0;bpy.ops.wm.save_as_mainfile(filepath=str(S/'Market_Ground_R95_Editable.blend'));print('MARKET95_NATIVE_PREPARED',len(rows),flush=True)
