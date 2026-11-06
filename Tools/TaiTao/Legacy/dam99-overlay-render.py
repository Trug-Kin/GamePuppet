import bpy,json,numpy as np,math,ast
from pathlib import Path
from mathutils import Vector
R=Path('D:/ConRoi');D=R/'Test/KetQua/journey_v48/dam_r99';M=R/'Test/BangChung/journey_v48/dam_r99'
A=np.array([637.7344,528.2603]);B=np.array([681.4578,491.2729]);L=float(np.linalg.norm(B-A));T=(B-A)/L;N=np.array([T[1],-T[0]]);fit=json.loads((D/'photo-fit-initial.json').read_text());params=fit['solution'][2:]
tree=ast.parse((R/'Test/Tam/dam99-review.py').read_text(encoding='utf-8-sig'));exec(compile(ast.Module(body=[n for n in tree.body if isinstance(n,ast.FunctionDef) and n.name=='camera'],type_ignores=[]),'camera','exec'))
for rev,path,name in [('r98','R98/OngDao_Dam_R98_Editable.blend','R98_CONCRETE_DAM_13_BAYS'),('candidate','R99/OngDao_Dam_R99_SectionCandidate.blend','R99_CONCRETE_DAM_13_ESTIMATED_BAYS')]:
 bpy.ops.wm.open_mainfile(filepath=str(R/'Source/Blender/JourneyV48'/path));sc=bpy.context.scene
 for o in sc.objects:
  if o.type=='MESH':o.hide_render=o.name!=name
 sc.render.film_transparent=True;sc.render.image_settings.color_mode='RGBA';sc.render.image_settings.file_format='PNG'
 for i,ph in enumerate(fit['photos']):camera(sc,i);sc.render.filepath=str(M/(rev+'-apron-mask-'+ph['id']+'.png'));bpy.ops.render.render(write_still=True)
print('RESULT photo camera apron masks from protected R98 and persisted candidate; files not saved',flush=True)
