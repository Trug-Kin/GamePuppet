import bpy,json,numpy as np
from pathlib import Path
R=Path('D:/ConRoi');D=R/'Test/KetQua/journey_v48/i2_r93_complete';D.mkdir(parents=True,exist_ok=True)
bpy.ops.wm.open_mainfile(filepath=str(R/'Source/Blender/JourneyV48/R93/GroundSurvey_R93_Editable.blend'));sc=bpy.context.scene;rows=[]
for i,o in enumerate(bpy.data.collections['GROUND_NATIVE_MATCHED_R93'].all_objects):
 if o.type!='MESH':continue
 m=o.data;m.calc_loop_triangles();v=np.array([o.matrix_world@v.co for v in m.vertices],dtype='f8');tri=np.array([t.vertices for t in m.loop_triangles],dtype='i4');loops=np.array([t.loops for t in m.loop_triangles],dtype='i4');uv=np.array([list(t.uv) for t in m.uv_layers[0].data],dtype='f8') if m.uv_layers else np.zeros((len(m.loops),2));norm=np.array([list(t.vector) for t in m.corner_normals]);p=np.c_[-v[:,0],v[:,2],-v[:,1]];key='native-'+str(i).zfill(2)
 np.savez_compressed(D/(key+'.npz'),v=p,tri=tri,uv=uv[loops],normals=norm[loops],slots=np.array([t.material_index for t in m.loop_triangles],dtype='i4'))
 rows.append(dict(key=key,object=o.name,path=o.get('unity_path',''),source=o.get('authoring_source',''),matrix=[list(row) for row in o.matrix_world],scale=list(o.scale),vertices=len(v),triangles=len(tri),materials=[m.name if m else None for m in m.materials],bounds=[p.min(0).tolist(),p.max(0).tolist()]))
report=dict(blender_version=bpy.app.version_string,unit_system=sc.unit_settings.system,scale_length=sc.unit_settings.scale_length,world_mapping='Unity=(-BlenderX, BlenderZ, -BlenderY); authoring units, not surveyed metres',objects=rows)
(D/'native-input.json').write_text(json.dumps(report,indent=2));print('NATIVE_INPUT',len(rows),'objects; unit',report['scale_length'],flush=True)
with bpy.data.libraries.load(str(R/'Docs/Reviews/journey_v48/r48/ong-dao-r48-source.blend'),link=False) as (a,b):names=list(a.objects)
(D/'bridge-native-names.json').write_text(json.dumps(names,indent=2));print('BRIDGE_NATIVE_NAMES',names[:35],flush=True)
