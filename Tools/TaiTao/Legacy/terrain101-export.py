import bpy,json,numpy as np,sys
from pathlib import Path
from mathutils import Vector,kdtree
R=Path('D:/ConRoi');D=R/'Test/KetQua/journey_v48/terrain_r101';E=D/'export';E.mkdir(exist_ok=True)
sys.path.insert(0,str(R/'.agents/skills/scenario-blender-export/scripts'));sys.path.insert(0,str(R/'.agents/skills/scenario-blender-expert/scripts'));import bx_export,bx_audit
bpy.ops.wm.open_mainfile(filepath=str(R/'Source/Blender/JourneyV48/R101/Terrain_R101_Editable.blend'))
o=bpy.data.objects['BankR91_Grade_017'];o.data.calc_loop_triangles();v=np.array([o.matrix_world@p.co for p in o.data.vertices]);f=np.array([tuple(t.vertices) for t in o.data.loop_triangles]);q=v[f];areas=np.linalg.norm(np.cross(q[:,1]-q[:,0],q[:,2]-q[:,0]),axis=1)*.5
assert np.isfinite(v).all() and areas.min()>1e-10 and tuple(o.scale)==(1,1,1)
assert all(np.isfinite(np.array([u.uv[:] for u in layer.data])).all() for layer in o.data.uv_layers)
# The legacy sheet has intentional open borders and submerged faces; gate the edited surface separately.
changes=json.loads((D/'source-delta.json').read_text())['changes'];local=bpy.data.meshes.new('R101LocalGate');ids=sorted({i for r in changes for i in r['vertices']});lookup={i:j for j,i in enumerate(ids)}
local.from_pydata([v[i] for i in ids],[],[[lookup[i] for i in o.data.polygons[r['face']].vertices] for r in changes]);local.update();part=bpy.data.objects.new('R101LocalGate',local);bpy.context.scene.collection.objects.link(part)
gate=bx_audit.deliverable([part],profile='game');(D/'local-delivery-gate.json').write_text(json.dumps(gate,indent=2,default=str));bpy.data.objects.remove(part,do_unlink=True)
o.name='T101_Surface142';info=bx_export.export(str(E/'TerrainR101.fbx'),'unity',[o],embed_textures=False,path_mode='AUTO');n=len(f)
bpy.ops.wm.read_factory_settings(use_empty=True);bpy.ops.import_scene.fbx(filepath=str(E/'TerrainR101.fbx'));bpy.context.view_layer.update();ob=next(o for o in bpy.context.scene.objects if o.type=='MESH');kd=kdtree.KDTree(len(v))
for i,p in enumerate(v):kd.insert(Vector(p),i)
kd.balance();error=max(kd.find(ob.matrix_world@p.co)[2] for p in ob.data.vertices);ob.data.calc_loop_triangles();assert error<.001 and len(ob.data.loop_triangles)==n
(D/'fbx-roundtrip.json').write_text(json.dumps(dict(source_vertices=len(v),triangles=n,min_area=float(areas.min()),error_world_max=error,export=info,local_gate=gate),indent=2,default=str));print('RESULT',len(v),n,error,'gate',gate.get('pass'),flush=True)
