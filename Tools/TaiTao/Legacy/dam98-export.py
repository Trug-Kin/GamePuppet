import bpy,json,sys,numpy as np
from pathlib import Path
from mathutils import Vector,kdtree
R=Path('D:/ConRoi');D=R/'Test/KetQua/journey_v48/dam_r98';E=R/'Test/Output/journey_v48/DamR98';E.mkdir(parents=True,exist_ok=True)
sys.path.insert(0,str(R/'.agents/skills/scenario-blender-expert/scripts'));import bx_audit
sys.path.insert(0,str(R/'.agents/skills/scenario-blender-export/scripts'));import bx_export
bpy.ops.wm.open_mainfile(filepath=str(R/'Source/Blender/JourneyV48/R98/OngDao_Dam_R98_Editable.blend'))
assert not bpy.data.libraries
missing=[i.filepath for i in bpy.data.images if not i.packed_file and i.source not in ['GENERATED','VIEWER'] and not Path(bpy.path.abspath(i.filepath)).exists()];assert not missing,missing
selected=sys.argv[sys.argv.index('--')+1:] if '--' in sys.argv else []
prior=json.loads((D/'export-reopen.json').read_text())['rows'] if selected else []
prior_round=json.loads((D/'fbx-roundtrip.json').read_text()) if selected else []
rows=[];expected={}
for o in list(bpy.context.scene.objects):
 if o.type!='MESH' or not o.get('dam98_export'):continue
 if selected and o.name not in selected:continue
 key=o.get('dam98_key',o.name);o.data.calc_loop_triangles();m=np.array(o.matrix_world,dtype=float);v=np.array([x.co for x in o.data.vertices],dtype=float)@m[:3,:3].T+m[:3,3];tri=np.array([tuple(p.vertices) for p in o.data.loop_triangles]);q=v[tri];area=np.linalg.norm(np.cross(q[:,1]-q[:,0],q[:,2]-q[:,0]),axis=1)*.5
 assert np.isfinite(v).all() and float(area.min())>1e-10,(key,float(area.min()))
 assert all(abs(s-1)<1e-7 for s in o.scale)
 if not o.get('dam98_cut'):
  prev=o.location.copy();o.location-=Vector(v.mean(0));bpy.context.view_layer.update();gate=bx_audit.deliverable([o],profile='game');o.location=prev;bpy.context.view_layer.update();gate['audit_only_rebased_for_float32']=True
 else:gate={'pass':None,'errors':[],'warnings':['Legacy context outside exact scoped cut is not globally certified.']}
 (D/(key+'-gate.json')).write_text(json.dumps(gate,indent=2,default=str))
 expected[key]=(v,len(tri));info=bx_export.export(str(E/(key+'.fbx')),'unity',[o],embed_textures=False,path_mode='AUTO')
 path=o.get('unity_path','')
 if o.get('dam98_cut') and not path and o.name.startswith('R97_'):path='R97_Bridgehead_GateSide_Bank/'+o.name
 rows.append(dict(key=key,name=o.name,path=path,legacy=bool(o.get('dam98_cut')),collider_only=bool(o.get('dam98_collider_only')),triangles=len(tri),vertices=len(v),min_triangle_area=float(area.min()),bounds_native=[v.min(0).tolist(),v.max(0).tolist()],material_names=[ma.name if ma else 'MISSING' for ma in o.data.materials],gate=gate,export=info))
 np.savez_compressed(D/(key+'-export-positions.npz'),v=np.c_[-v[:,0],v[:,2],-v[:,1]],tri=tri)
 print('EXPORTED',key,len(tri),flush=True)
rows=[r for r in prior if r['name'] not in selected]+rows if selected else rows
(D/'export-reopen.json').write_text(json.dumps(dict(source=bpy.data.filepath,version=bpy.app.version_string,unit_scale=bpy.context.scene.unit_settings.scale_length,missing_images=missing,libraries=0,rows=rows),indent=2,default=str))
out=[]
for key,(v,ntri) in expected.items():
 bpy.ops.wm.read_factory_settings(use_empty=True);bpy.ops.import_scene.fbx(filepath=str(E/(key+'.fbx')));bpy.context.view_layer.update();o=next(o for o in bpy.data.objects if o.type=='MESH');o.data.calc_loop_triangles();kd=kdtree.KDTree(len(v))
 for i,p in enumerate(v):kd.insert(Vector(p),i)
 kd.balance();error=max(kd.find(o.matrix_world@x.co)[2] for x in o.data.vertices);assert error<.001 and len(o.data.loop_triangles)==ntri,(key,error)
 out.append(dict(key=key,max_world_error_m=error,triangles=ntri));print('ROUNDTRIP',key,error,flush=True)
out=[r for r in prior_round if r['key'] not in selected]+out if selected else out
(D/'fbx-roundtrip.json').write_text(json.dumps(out,indent=2));print('RESULT export/reopen/roundtrip',len(out),flush=True)
