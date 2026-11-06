import bpy,json
from pathlib import Path
R=Path('D:/ConRoi');D=R/'Test/KetQua/journey_v48/dam_r99';P=R/'Source/Blender/JourneyV48/R99/OngDao_Dam_R99_SectionCandidate.blend'
bpy.ops.wm.open_mainfile(filepath=str(P));rows=[s.split('\t') for s in (D/'unity-binding-audit.tsv').read_text(encoding='utf-8-sig').splitlines()];by={}
for e in rows:
 if len(e)<9:continue
 if e[1] not in by or by[e[1]][2]=='True' and e[2]=='False':by[e[1]]=e
c=bpy.data.collections.get('R99_ACTUAL_CONTEXT') or bpy.data.collections.new('R99_ACTUAL_CONTEXT')
if c.name not in bpy.context.scene.collection.children:bpy.context.scene.collection.children.link(c)
report=[]
for path,e in by.items():
 o=bpy.data.objects[e[0]];o.hide_render=e[3]!='True' or e[4]!='True'
 if o.name not in c.objects:c.objects.link(o)
 o['dam99_binding_path']=path;o['dam99_actual_renderer']=e[4]=='True';o['dam99_actual_collider']=':True:False:' in e[6]
 report.append(dict(object=o.name,path=path,render=not o.hide_render,collider=o['dam99_actual_collider']))
# Suppress stale alternate source for the same current Unity path.
selected={e[0] for e in by.values()}
for e in rows:
 if len(e)>8 and e[0] not in selected:bpy.data.objects[e[0]].hide_render=True
bpy.context.preferences.filepaths.save_version=0;bpy.ops.wm.save_as_mainfile(filepath=str(P),compress=True);(D/'actual-context-selection.json').write_text(json.dumps(report,indent=2));print('RESULT mapped',len(report),'current Unity contexts',flush=True)
