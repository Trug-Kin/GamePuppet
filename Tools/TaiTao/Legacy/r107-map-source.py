import bpy,sys,json,hashlib,numpy as np
from pathlib import Path
from mathutils import Vector,kdtree
R=Path('D:/ConRoi');D=R/'Test/KetQua/investigation_r107';S=R/'Source/Blender/JourneyV48/R107';E=D/'export';S.mkdir(exist_ok=True);E.mkdir(exist_ok=True)
P=S/'Terrain_R107_Editable.blend';assert not P.exists()
bpy.ops.wm.open_mainfile(filepath=str(R/'Source/Blender/JourneyV48/R101/Terrain_R101_Editable.blend'))
sys.path[:0]=[str(R/'.agents/skills/scenario-blender-expert/scripts'),str(R/'.agents/skills/scenario-blender-export/scripts')]
import bx_export,bx_audit
names=['TerrainJoin_R90','EastFootwayJoin_R90','TerrainColliderJoin_R90','M95_Service4_NATIVE']
col=bpy.data.collections.new('TERRAIN_R107_WINDING_REPAIR');bpy.context.scene.collection.children.link(col)
rows=[]
for k,name in enumerate(names):
 o=bpy.data.objects[name];v=[o.matrix_world@p.co for p in o.data.vertices];before=np.array([p.co[:] for p in o.data.vertices],dtype=np.float32);changed=[]
 for p in o.data.polygons:
  q=[v[i] for i in p.vertices];c=sum(q,Vector())/len(q)
  if 560<=-c.x<=720 and 320<=-c.y<=418 and c.z>-.05 and p.normal.z<-.2:
   changed.append(p.index);p.flip()
 o.data.update();assert np.array_equal(before,np.array([p.co[:] for p in o.data.vertices],dtype=np.float32))
 o['r107_repair']='Only reverse down-facing terrain top triangles within X560..720 Z320..418 Y>-.05; no coordinate/height edits.'
 col.objects.link(o);rows.append(dict(alias='T107_'+str(k),name=name,path=o.get('unity_path'),flipped=len(changed),faces=changed,vertices_unchanged=True,collider_only=k>=2))
bpy.ops.wm.save_as_mainfile(filepath=str(P));bpy.ops.wm.open_mainfile(filepath=str(P))
parts=[bpy.data.objects[n] for n in names]
# Existing open terrain sheets are audited as sheets, not closed manufactured props.
# General deliverable reports are retained; inward-volume heuristic is not a topology rule for terrain.
gates=[]
for o in parts:
 try:gates.append(dict(name=o.name,gate=bx_audit.deliverable([o.name],profile='game')))
 except Exception as e:gates.append(dict(name=o.name,error=str(e)))
(D/'source-delivery-gate.json').write_text(json.dumps(gates,indent=2,default=str))
expected={}
for row,o in zip(rows,parts):
 o.data.calc_loop_triangles();vv=np.array([o.matrix_world@p.co for p in o.data.vertices]);ff=np.array([tuple(t.vertices) for t in o.data.loop_triangles]);q=vv[ff];area=np.linalg.norm(np.cross(q[:,1]-q[:,0],q[:,2]-q[:,0]),axis=1)/2
 assert np.isfinite(vv).all() and tuple(o.scale)==(1,1,1)
 row['min_area']=float(area.min());row['zero_area_triangles']=int((area<1e-10).sum());row['triangles']=len(ff)
 expected[row['alias']]=(vv,len(ff));o.name=row['alias']
info=bx_export.export(str(E/'TerrainR107.fbx'),'unity',parts,embed_textures=False,path_mode='AUTO')
bpy.ops.wm.read_factory_settings(use_empty=True);bpy.ops.import_scene.fbx(filepath=str(E/'TerrainR107.fbx'));bpy.context.view_layer.update();rt=[]
for o in bpy.context.scene.objects:
 if o.type!='MESH':continue
 vv,n=expected[o.name];kd=kdtree.KDTree(len(vv))
 for i,p in enumerate(vv):kd.insert(Vector(p),i)
 kd.balance();err=max(kd.find(o.matrix_world@p.co)[2] for p in o.data.vertices);o.data.calc_loop_triangles();assert err<.001 and len(o.data.loop_triangles)==n
 rt.append(dict(alias=o.name,max_error=err,triangles=n))
(D/'source-export.json').write_text(json.dumps(dict(source=str(P),scope='X560..720 Z320..418, only downward terrain tops Y>-.05',rows=rows,roundtrip=rt,export=info),indent=2,default=str))
print('RESULT',[(r['name'],r['flipped']) for r in rows],rt)
