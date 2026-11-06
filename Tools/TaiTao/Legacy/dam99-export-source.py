import bpy,json,numpy as np,sys
from pathlib import Path
from mathutils import Vector,kdtree
R=Path('D:/ConRoi');D=R/'Test/KetQua/journey_v48/dam_r99';E=R/'Test/Output/journey_v48/DamR99';E.mkdir(parents=True,exist_ok=True);P=R/'Source/Blender/JourneyV48/R99/OngDao_Dam_R99_SectionCandidate.blend'
sys.path.insert(0,str(R/'.agents/skills/scenario-blender-export/scripts'));import bx_export
bpy.ops.wm.open_mainfile(filepath=str(P));sc=bpy.context.scene
assert not bpy.data.libraries
missing=[im.filepath for im in bpy.data.images if not im.packed_file and im.source not in ['GENERATED','VIEWER'] and not Path(bpy.path.abspath(im.filepath)).exists()];assert not missing
assert not json.loads((D/'actual-collision-stair-audit.json').read_text())['intrusion_counts']
assert all(r['gate']['pass'] for r in json.loads((D/'candidate-local-gate-final.json').read_text()))
select=json.loads((D/'actual-context-selection.json').read_text());rows=[];parts=[];expected={}
names={x['object']:x for x in select if x['render'] or x['collider']}
for o in sc.objects:
 if o.type!='MESH':continue
 new=o.name.startswith('R99_') and not o.name.startswith('R99_SUPPORT_')
 separate=o.get('dam99_collider_only',False)
 if not (o.name in names or new or separate):continue
 path=o.get('dam99_binding_path','')
 if not new and not path:continue
 o.data.calc_loop_triangles();v=np.array([o.matrix_world@p.co for p in o.data.vertices]);tri=np.array([tuple(p.vertices) for p in o.data.loop_triangles]);q=v[tri];area=np.linalg.norm(np.cross(q[:,1]-q[:,0],q[:,2]-q[:,0]),axis=1)*.5
 assert np.isfinite(v).all(),o.name
 deg=int(np.sum(area<1e-10))
 if deg:raise RuntimeError((o.name,'degenerate triangles',deg,float(area.min())))
 alias='D99_'+str(len(parts)).zfill(3);parts.append(o);expected[alias]=(v,len(tri));rows.append(dict(key=o.name,export_name=alias,path=path,new=new,collider_only=separate or (o.name in names and not names[o.name]['render']),materials=[ma.name if ma else 'MISSING' for ma in o.data.materials],triangles=len(tri),vertices=len(v),bounds_native=[v.min(0).tolist(),v.max(0).tolist()]));o.name=alias
(D/'export-selection.json').write_text(json.dumps(rows,indent=2));print('SELECTION',len(parts),sum(x['triangles'] for x in rows),'triangles',flush=True)
info=bx_export.export(str(E/'OngDaoDamR99.fbx'),'unity',parts,embed_textures=False,path_mode='AUTO')
bpy.ops.wm.read_factory_settings(use_empty=True);bpy.ops.import_scene.fbx(filepath=str(E/'OngDaoDamR99.fbx'));bpy.context.view_layer.update();out=[]
for o in bpy.context.scene.objects:
 if o.type!='MESH':continue
 assert o.name in expected,o.name
 v,n=expected[o.name];o.data.calc_loop_triangles();kd=kdtree.KDTree(len(v))
 for i,p in enumerate(v):kd.insert(Vector(p),i)
 kd.balance();err=max(kd.find(o.matrix_world@p.co)[2] for p in o.data.vertices);assert err<.001 and len(o.data.loop_triangles)==n,(o.name,err,n,len(o.data.loop_triangles))
 out.append(dict(key=o.name,max_world_error=err,triangles=n))
assert len(out)==len(expected)
(D/'fbx-roundtrip.json').write_text(json.dumps(dict(source=str(P),fbx=str(E/'OngDaoDamR99.fbx'),unit_scale=1,export=info,roundtrip=out),indent=2,default=str));print('RESULT FBX roundtrip',len(out),'max',max(x['max_world_error'] for x in out),flush=True)
