import bpy,json,sys,numpy as np
from pathlib import Path
from mathutils import Vector,kdtree
from mathutils.bvhtree import BVHTree
R=Path('D:/ConRoi');D=R/'Test/KetQua/journey_v48/bank_r97';E=R/'Test/Output/journey_v48/BankR97';E.mkdir(parents=True,exist_ok=True)
sys.path.insert(0,str(R/'.agents/skills/scenario-blender-expert/scripts'));import bx_audit
sys.path.insert(0,str(R/'.agents/skills/scenario-blender-export/scripts'));import bx_export
bpy.ops.wm.open_mainfile(filepath=str(R/'Source/Blender/JourneyV48/R97/Bridgehead_Bank_R97_Editable.blend'))
assert not bpy.data.libraries
missing=[i.filepath for i in bpy.data.images if not i.packed_file and i.source not in ['GENERATED','VIEWER'] and not Path(bpy.path.abspath(i.filepath)).exists()];assert not missing,missing
rows=[];expected={};section=[]
coast=np.array([[411.186473,641.841459],[426.079479,652.028465],[439.996352,661.236382],[460.297645,670.699549],[471.846449,675.05722],[476.974594,675.900316],[481.787168,674.963951],[486.249314,672.422123]])
old=np.load(D/'M95_022-existing.npz');ov=old['v'];ot=old['tri'];tree=BVHTree.FromPolygons([Vector(p) for p in ov],ot.tolist())
for o in list(bpy.context.scene.objects):
 if o.type!='MESH' or not o.get('bank97_export'):continue
 key=o.get('market95_export_key',o.name);o.data.calc_loop_triangles();m=np.array(o.matrix_world,dtype=float);v=np.array([x.co for x in o.data.vertices],dtype=float)@m[:3,:3].T+m[:3,3];tri=np.array([tuple(p.vertices) for p in o.data.loop_triangles]);q=v[tri];area=np.linalg.norm(np.cross(q[:,1]-q[:,0],q[:,2]-q[:,0]),axis=1)*.5
 assert np.isfinite(v).all() and float(area.min())>1e-10,(key,float(area.min()))
 assert all(abs(s-1)<1e-7 for s in o.scale) and o.data.uv_layers
 if key=='R97_PHOTO_SHORT_RAIL':
  prev=o.location.copy();o.location-=Vector(v.mean(0));bpy.context.view_layer.update();gate=bx_audit.deliverable([o],profile='game');o.location=prev;bpy.context.view_layer.update();gate['rebased_for_float32_audit_only']=True
 else:gate=bx_audit.deliverable([o],profile='game') if key.startswith('R97_') else {'pass':None,'errors':[],'warnings':['Legacy context outside the cut is not globally certified; exact area/finite/UV and scoped replacement checked.']}
 (D/(key+'-gate.json')).write_text(json.dumps(gate,indent=2,default=str))
 expected[key]=(v,tri.shape[0]);info=bx_export.export(str(E/(key+'.fbx')),'unity',[o],embed_textures=False,path_mode='AUTO')
 rows.append(dict(key=key,name=o.name,path=o.get('unity_path',''),legacy=bool(o.get('bank97_cut')),triangles=len(tri),vertices=len(v),min_triangle_area=float(area.min()),bounds_native=[v.min(0).tolist(),v.max(0).tolist()],material_names=[ma.name if ma else 'MISSING' for ma in o.data.materials],gate=gate,export=info))
 if key.startswith('R97_'):
  u=np.c_[-v[:,0],v[:,2],-v[:,1]];np.savez_compressed(D/(key+'-final.npz'),v=u,tri=tri)
 # Serialized expected Unity coordinates used only to verify FBX integration.
 np.savez_compressed(D/(key+'-export-positions.npz'),v=np.c_[-v[:,0],v[:,2],-v[:,1]],tri=tri)
print('EXPORT_COUNT',len(rows),flush=True)
(D/'export-reopen.json').write_text(json.dumps(dict(source=bpy.data.filepath,version=bpy.app.version_string,unit_scale=bpy.context.scene.unit_settings.scale_length,missing_images=missing,libraries=0,rows=rows),indent=2,default=str))
out=[]
for key,(v,ntri) in expected.items():
 bpy.ops.wm.read_factory_settings(use_empty=True);bpy.ops.import_scene.fbx(filepath=str(E/(key+'.fbx')));bpy.context.view_layer.update();o=next(o for o in bpy.data.objects if o.type=='MESH');o.data.calc_loop_triangles();kd=kdtree.KDTree(len(v))
 for i,p in enumerate(v):kd.insert(Vector(p),i)
 kd.balance();error=max(kd.find(o.matrix_world@x.co)[2] for x in o.data.vertices);assert error<.001 and len(o.data.loop_triangles)==ntri,(key,error)
 out.append(dict(key=key,max_world_error_m=error,triangles=ntri));print('ROUNDTRIP',key,error,flush=True)
(D/'fbx-roundtrip.json').write_text(json.dumps(out,indent=2));print('RESULT export/reopen/roundtrip',len(out),flush=True)
