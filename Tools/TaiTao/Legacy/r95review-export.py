import bpy,json,sys,numpy as np
from pathlib import Path
from mathutils import kdtree,Vector
R=Path('D:/ConRoi');D=R/'Test/KetQua/journey_v48/r95_visual_review';P=R/'Source/Blender/JourneyV48/R96/Market_Ground_R96_Editable.blend';E=R/'Test/Output/journey_v48/MarketGroundR96';E.mkdir(parents=True,exist_ok=True)
sys.path.insert(0,str(R/'.agents/skills/scenario-blender-expert/scripts'));import bx_audit
sys.path.insert(0,str(R/'.agents/skills/scenario-blender-export/scripts'));import bx_export
bpy.ops.wm.open_mainfile(filepath=str(P));fix=json.loads((D/'winding-source-fix.json').read_text())['fixes'];keys=[r['key'] for r in fix];rows=[];expected={}
assert not bpy.data.libraries and all(i.packed_file or i.source in ['GENERATED','VIEWER'] or Path(bpy.path.abspath(i.filepath)).exists() for i in bpy.data.images)
for o in list(bpy.data.collections['MARKET_GROUND_NATIVE_R95'].all_objects):
 key=o.get('market95_export_key')
 if key not in keys:continue
 o.hide_viewport=False;o.hide_set(False);bpy.context.view_layer.update();mat=np.array(o.matrix_world,dtype=float);v=np.array([x.co for x in o.data.vertices],dtype=float)@mat[:3,:3].T+mat[:3,3];t=np.array([list(p.vertices) for p in o.data.polygons]);q=v[t];n=np.cross(q[:,1]-q[:,0],q[:,2]-q[:,0]);c=q.mean(1);u=np.c_[-c[:,0],c[:,2],-c[:,1]];wrong=(n[:,2]<-.7*np.linalg.norm(n,axis=1))&(u[:,0]>=680)&(u[:,0]<=755)&(u[:,2]>=435)&(u[:,2]<=505)
 print('AUDIT',key,'wrong',int(wrong.sum()),'small',int((np.linalg.norm(n,axis=1)<1e-9).sum()),'minarea',float(np.linalg.norm(n,axis=1).min()),flush=True)
 assert not np.any(wrong) and not np.any(np.linalg.norm(n,axis=1)<1e-9)
 # Snapshot collection iteration: exporter/auditor temporary objects invalidate
 # Blender's live all_objects iterator (Collection_all_objects_next native crash).
 gate=bx_audit.deliverable([o],profile='game')
 assert len(np.unique(np.sort(t,axis=1),axis=0))==len(t)
 assert np.isfinite(v).all() and all(abs(s-1)<1e-7 for s in o.scale)
 assert o.data.uv_layers and all(np.isfinite(np.array([x.uv for x in uvl.data])).all() for uvl in o.data.uv_layers)
 (D/(key+'-delivery-gate.json')).write_text(json.dumps(dict(**gate,zero_area_faces_exact=0,duplicate_faces_exact=0,wrong_scoped_winding=0,positions_changed=0),indent=2,default=str))
 expected[key]=dict(v=v,tri=len(t),bounds=[v.min(0).tolist(),v.max(0).tolist()]);settings=bx_export.export(str(E/(key+'.fbx')),'unity',objects=[o],embed_textures=False,path_mode='AUTO');rows.append(dict(key=key,path=o['unity_path'],triangles=len(t),vertices=len(v),scale=list(o.scale),uv_layers=[x.name for x in o.data.uv_layers],gate_errors=gate['errors'],gate_warnings=gate['warnings'],export=settings))
(D/'export-reopen.json').write_text(json.dumps(dict(version=bpy.app.version_string,source=str(P),rows=rows),indent=2,default=str));out=[]
for key,a in expected.items():
 bpy.ops.wm.read_factory_settings(use_empty=True);bpy.ops.import_scene.fbx(filepath=str(E/(key+'.fbx')));bpy.context.view_layer.update();o=next(o for o in bpy.data.objects if o.type=='MESH');kd=kdtree.KDTree(len(a['v']))
 for i,p in enumerate(a['v']):kd.insert(Vector(p),i)
 kd.balance();error=max(kd.find(o.matrix_world@x.co)[2] for x in o.data.vertices);o.data.calc_loop_triangles();assert error<.001 and len(o.data.loop_triangles)==a['tri'];out.append(dict(key=key,max_world_error_m=error,triangles=len(o.data.loop_triangles),scale=list(o.scale)));print('ROUNDTRIP',key,error,flush=True)
(D/'fbx-roundtrip.json').write_text(json.dumps(out,indent=2));print('RESULT reopened/exported/roundtrip',len(out),flush=True)
