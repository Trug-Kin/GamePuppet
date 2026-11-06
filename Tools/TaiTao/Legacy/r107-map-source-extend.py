import bpy,sys,json,numpy as np
from pathlib import Path
from mathutils import Vector,kdtree
R=Path('D:/ConRoi');D=R/'Test/KetQua/investigation_r107';P=R/'Source/Blender/JourneyV48/R107/Terrain_R107_Editable.blend';E=D/'export'
bpy.ops.wm.open_mainfile(filepath=str(P));sys.path.insert(0,str(R/'.agents/skills/scenario-blender-export/scripts'));import bx_export
names=['TerrainJoin_R90','EastFootwayJoin_R90','TerrainColliderJoin_R90','M95_Service4_NATIVE','BankR91_Grade_017'];rows=[]
col=bpy.data.collections['TERRAIN_R107_WINDING_REPAIR']
for k,name in enumerate(names):
 o=bpy.data.objects[name];v=[o.matrix_world@p.co for p in o.data.vertices];before=np.array([p.co[:] for p in o.data.vertices],dtype=np.float32);changes=[]
 for p in o.data.polygons:
  q=[v[i] for i in p.vertices];c=sum(q,Vector())/len(q)
  if 560<=-c.x<=720 and 320<=-c.y<=460 and c.z>-.05 and p.normal.z<-.2:changes.append(p.index);p.flip()
 o.data.update();assert np.array_equal(before,np.array([p.co[:] for p in o.data.vertices],dtype=np.float32))
 o['r107_scope_final']='X560..720 Z320..460, top winding only Y>-.05. Extension justified by adjacent edge pixel rays; no height/water/market change.'
 if o.name not in col.objects:col.objects.link(o)
 rows.append(dict(name=name,alias='T107_'+str(k),path=o.get('unity_path') or o.get('dam99_binding_path'),additional_flips=len(changes),faces=changes))
bpy.ops.wm.save_as_mainfile(filepath=str(P));bpy.ops.wm.open_mainfile(filepath=str(P));expected={};parts=[]
for row in rows:
 o=bpy.data.objects[row['name']];o.data.calc_loop_triangles();vv=[o.matrix_world@p.co for p in o.data.vertices];expected[row['alias']]=(vv,len(o.data.loop_triangles));o.name=row['alias'];parts.append(o)
info=bx_export.export(str(E/'TerrainR107_Final.fbx'),'unity',parts,embed_textures=False,path_mode='AUTO')
bpy.ops.wm.read_factory_settings(use_empty=True);bpy.ops.import_scene.fbx(filepath=str(E/'TerrainR107_Final.fbx'));bpy.context.view_layer.update();rt=[]
for o in bpy.context.scene.objects:
 if o.type!='MESH':continue
 vv,n=expected[o.name];kd=kdtree.KDTree(len(vv))
 for i,p in enumerate(vv):kd.insert(p,i)
 kd.balance();err=max(kd.find(o.matrix_world@p.co)[2] for p in o.data.vertices);o.data.calc_loop_triangles();assert err<.001 and len(o.data.loop_triangles)==n;rt.append(dict(alias=o.name,triangles=n,max_error=err))
(D/'source-export-final.json').write_text(json.dumps(dict(source=str(P),scope='X560..720 Z320..460 Y>-.05 top winding only',rows=rows,roundtrip=rt,export=info),indent=2,default=str));print('RESULT',[(r['name'],r['additional_flips']) for r in rows])
