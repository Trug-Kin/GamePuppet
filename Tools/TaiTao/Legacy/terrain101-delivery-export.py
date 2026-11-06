import bpy,json,sys,numpy as np
from pathlib import Path
from mathutils import Vector,kdtree
R=Path('D:/ConRoi');D=R/'Test/KetQua/journey_v48/terrain_r101';E=D/'export';sys.path.insert(0,str(R/'.agents/skills/scenario-blender-export/scripts'));import bx_export
bpy.ops.wm.open_mainfile(filepath=str(R/'Source/Blender/JourneyV48/R101/Terrain_R101_Editable.blend'))
names=['BankR91_Grade_017','R97_LOWER_WALK','R97_LOWER_GRASS','M95_Service3_NATIVE','TerrainColliderJoin_R90'];parts=[];expected={};rows=[]
for i,name in enumerate(names):
 o=bpy.data.objects[name];v=np.array([o.matrix_world@p.co for p in o.data.vertices]);o.data.calc_loop_triangles();f=np.array([tuple(t.vertices) for t in o.data.loop_triangles]);a=v[f];area=np.linalg.norm(np.cross(a[:,1]-a[:,0],a[:,2]-a[:,0]),axis=1)*.5;assert np.isfinite(v).all() and area.min()>1e-10,(name,area.min());assert tuple(o.scale)==(1,1,1)
 for uv in o.data.uv_layers:assert np.isfinite(np.array([p.uv[:] for p in uv.data])).all()
 alias='T101_'+str(i);rows.append(dict(alias=alias,key=name,path=o['dam99_binding_path'],collider_only=name in ['M95_Service3_NATIVE','TerrainColliderJoin_R90'],triangles=len(f),min_area=float(area.min()),uv_layers=len(o.data.uv_layers)));expected[alias]=(v,len(f));o.name=alias;parts.append(o)
info=bx_export.export(str(E/'TerrainR101_Delivery.fbx'),'unity',parts,embed_textures=False,path_mode='AUTO');bpy.ops.wm.read_factory_settings(use_empty=True);bpy.ops.import_scene.fbx(filepath=str(E/'TerrainR101_Delivery.fbx'));bpy.context.view_layer.update();out=[]
for o in bpy.context.scene.objects:
 if o.type!='MESH':continue
 v,n=expected[o.name];kd=kdtree.KDTree(len(v))
 for i,p in enumerate(v):kd.insert(Vector(p),i)
 kd.balance();err=max(kd.find(o.matrix_world@p.co)[2] for p in o.data.vertices);o.data.calc_loop_triangles();assert err<.001 and len(o.data.loop_triangles)==n;out.append(dict(alias=o.name,max_error=err,triangles=n))
assert len(out)==5;(D/'final-fbx-roundtrip.json').write_text(json.dumps(dict(rows=rows,roundtrip=out,export=info),indent=2,default=str));(D/'final-integration.tsv').write_text('\n'.join('\t'.join([r['alias'],r['key'],r['path'],'1' if r['collider_only'] else '0']) for r in rows));print('RESULT',json.dumps(out),flush=True)

