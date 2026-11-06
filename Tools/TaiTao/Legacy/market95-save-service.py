import bpy,json
from pathlib import Path
R=Path('D:/ConRoi');D=R/'Test/KetQua/journey_v48/market_ground_r95';P=R/'Source/Blender/JourneyV48/R95/Market_Ground_R95_Editable.blend';bpy.ops.wm.open_mainfile(filepath=str(P));rows=json.loads((D/'service-layer-closure.json').read_text());paths={r['path'] for r in rows};root=bpy.data.collections['MARKET_GROUND_NATIVE_R95']
for r in rows:
 old=next(o for o in bpy.data.objects if o.get('unity_path')==r['path']);m=bpy.data.meshes.new(r['key']);o=bpy.data.objects.new(r['object'],m);root.objects.link(o);o['unity_path']=r['path']
 for mat in old.data.materials:m.materials.append(mat)
 for copy in list(bpy.data.objects):
  if copy!=o and copy.get('unity_path')==r['path']:copy.hide_render=True;copy['market95_export_key']='';copy['candidate_key']=''
b=json.loads((D/'placement-geometry-bindings.json').read_text());(D/'placement-geometry-bindings.json').write_text(json.dumps([x for x in b if x['path'] not in paths],indent=2));bpy.context.preferences.filepaths.save_version=0;bpy.ops.wm.save_as_mainfile(filepath=str(P));exec((R/'Test/Tam/market95-save.py').read_text())
