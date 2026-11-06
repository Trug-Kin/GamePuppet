import bpy,json,math
from pathlib import Path
R=Path('D:/ConRoi');S=R/'Source/Blender/JourneyV48/R93';D=R/'Test/KetQua/journey_v48/ground_r93';out=[]
for entry in json.loads((D/'blender-handoff-state.json').read_text()):
 p=R/entry['file'];bpy.ops.wm.open_mainfile(filepath=str(p));sc=bpy.context.scene;col=bpy.data.collections['GROUND_NATIVE_MATCHED_R93'];rows=[]
 for o in col.all_objects:
  if o.type!='MESH':continue
  rows.append(dict(object=o.name,path=o.get('unity_path',''),vertices=len(o.data.vertices),polygons=len(o.data.polygons),uv_layers=len(o.data.uv_layers),scale=list(o.scale),finite=all(math.isfinite(v) for q in o.data.vertices for v in q.co),missing_material_slots=sum(x is None for x in o.data.materials),render_hidden=o.hide_render))
 missing=[i.filepath for i in bpy.data.images if i.source=='FILE' and not i.packed_file and i.filepath and not Path(bpy.path.abspath(i.filepath)).exists()]
 out.append(dict(file=entry['file'],status=sc.get('handoff_status','MISSING'),linked_libraries=[l.filepath for l in bpy.data.libraries],missing_images=missing,objects=rows,gate='REOPEN_AND_DATA_ONLY; NOT_A_GEOMETRY_ACCEPTANCE'))
(D/'blender-delivery-reopen.json').write_text(json.dumps(out,indent=2));assert all(not r['linked_libraries'] and not r['missing_images'] and all(o['finite'] and o['missing_material_slots']==0 for o in r['objects']) for r in out);print('DELIVERY_REOPEN',len(out),'files; statuses',[r['status'] for r in out],flush=True)
