import bpy,json,numpy as np
from pathlib import Path
R=Path('D:/ConRoi');D=R/'Test/KetQua/journey_v48/dam_r99'
bpy.ops.wm.open_mainfile(filepath=str(R/'Source/Blender/JourneyV48/R98/OngDao_Dam_R98_Editable.blend'))
A=np.array([637.7344,528.2603]);B=np.array([681.4578,491.2729]);L=float(np.linalg.norm(B-A));T=(B-A)/L;N=np.array([T[1],-T[0]])
rows=[]
for o in bpy.context.scene.objects:
 if o.type!='MESH':continue
 mm=np.array(o.matrix_world,dtype=float);v=np.array([p.co for p in o.data.vertices])@mm[:3,:3].T+mm[:3,3]
 sd=(np.c_[-v[:,0],-v[:,1]]-A)@np.c_[T,N]
 if o.get('dam98_export') or o.get('unity_path','').startswith('R11_Local_Bridge_Supports/') or ('Surface_051' in o.name):
  rows.append(dict(name=o.name,path=o.get('unity_path'),key=o.get('dam98_key'),r98_export=bool(o.get('dam98_export')),cut=bool(o.get('dam98_cut')),bounds_sd=[sd.min(0).tolist(),sd.max(0).tolist()],h=[float(v[:,2].min()),float(v[:,2].max())],hidden=o.hide_render,vertices=len(v)))
out=dict(units=bpy.context.scene.unit_settings.system,unit_scale=bpy.context.scene.unit_settings.scale_length,objects=rows,scene_properties=dict(bpy.context.scene.items()),water_provenance='Surface_051 / game datum Y0, historical v21 shoreline rule, not a water level survey',bridge_record='OngDaoV35/bridge_record.json interpretation explicitly estimates all dimensions other than horizontal OSM alignment')
(D/'r98-source-diagnostic.json').write_text(json.dumps(out,indent=2,default=str));print('RESULT',len(rows),flush=True)
