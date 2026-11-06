import bpy,json,numpy as np,sys
from pathlib import Path
from mathutils import Vector,kdtree
R=Path('D:/ConRoi');D=R/'Test/KetQua/journey_v48/map_r99_closeout';E=R/'Test/Output/journey_v48/DamR100';E.mkdir(parents=True,exist_ok=True);P=R/'Source/Blender/JourneyV48/R100/OngDao_Dam_R100_Editable.blend'
sys.path.insert(0,str(R/'.agents/skills/scenario-blender-export/scripts'));import bx_export
bpy.ops.wm.open_mainfile(filepath=str(P));assert bpy.context.scene.unit_settings.scale_length==1
names=['C95_0043_OD35_Surface_3','C95_0044_OD35_Surface_4']+['D99_FLOWERS_'+c+'_'+str(i) for c in ['Red','Yellow'] for i in range(3)];parts=[];expected={};rows=[]
for name in names:
 o=bpy.data.objects[name];assert tuple(o.scale)==(1,1,1);o.data.calc_loop_triangles();v=np.array([o.matrix_world@p.co for p in o.data.vertices]);tri=np.array([tuple(t.vertices) for t in o.data.loop_triangles]);q=v[tri];area=np.linalg.norm(np.cross(q[:,1]-q[:,0],q[:,2]-q[:,0]),axis=1)*.5;assert np.isfinite(v).all() and np.min(area)>1e-10,(name,float(np.min(area)))
 alias='D100_'+str(len(parts)).zfill(3);rows.append(dict(alias=alias,key=name,path=o['dam99_binding_path'],vertices=len(v),triangles=len(tri),min_area=float(np.min(area)),uv_layers=len(o.data.uv_layers),scale=list(o.scale),geometry_delta=name.startswith('C95_')));expected[alias]=(v,len(tri));o.name=alias;parts.append(o)
(D/'r100-export-selection.json').write_text(json.dumps(rows,indent=2));info=bx_export.export(str(E/'OngDaoDamR100_Delta.fbx'),'unity',parts,embed_textures=False,path_mode='AUTO')
bpy.ops.wm.read_factory_settings(use_empty=True);bpy.ops.import_scene.fbx(filepath=str(E/'OngDaoDamR100_Delta.fbx'));bpy.context.view_layer.update();out=[]
for o in bpy.context.scene.objects:
 if o.type!='MESH':continue
 v,n=expected[o.name];kd=kdtree.KDTree(len(v))
 for i,p in enumerate(v):kd.insert(Vector(p),i)
 kd.balance();err=max(kd.find(o.matrix_world@p.co)[2] for p in o.data.vertices);o.data.calc_loop_triangles();assert err<.001 and len(o.data.loop_triangles)==n
 out.append(dict(alias=o.name,max_world_error=err,triangles=n))
assert len(out)==8
(D/'r100-fbx-roundtrip.json').write_text(json.dumps(dict(source=str(P),export=info,roundtrip=out),indent=2,default=str));print('RESULT delta8 max error',max(x['max_world_error'] for x in out),flush=True)
