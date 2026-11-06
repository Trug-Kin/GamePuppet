import bpy,numpy as np,json
from pathlib import Path
R=Path('D:/ConRoi');D=R/'Test/KetQua/journey_v48/dam_r99';bpy.ops.wm.open_mainfile(filepath=str(R/'Source/Blender/JourneyV48/R99/OngDao_Dam_R99_SectionCandidate.blend'));bad=[]
for r in json.loads((D/'export-selection.json').read_text()):
 o=bpy.data.objects[r['key']]
 for i,u in enumerate(o.data.uv_layers):
  a=np.empty(len(u.data)*2,dtype=np.float32);u.data.foreach_get('uv',a);a=a.reshape(-1,2);n=int(np.sum(~np.isfinite(a).all(1)))
  if n:bad.append(dict(object=o.name,path=r['path'],channel=i,name=u.name,bad_loops=n,loops=len(a)))
(D/'uv-nonfinite.json').write_text(json.dumps(bad,indent=2));(D/'uv-bad-paths.tsv').write_text('\n'.join(x['object']+'\t'+x['path'] for x in bad),encoding='utf-8');print('RESULT',bad,flush=True)
