import ast,bpy,numpy as np,json
from pathlib import Path
R=Path('D:/ConRoi');D=R/'Test/KetQua/journey_v48/map_r99_closeout'
t=ast.parse((R/'Test/Tam/map100-contact-audit.py').read_text());exec(compile(ast.Module(body=[x for x in t.body if isinstance(x,ast.FunctionDef)],type_ignores=[]),'helpers','exec'))
bpy.ops.wm.open_mainfile(filepath=str(R/'Source/Blender/JourneyV48/R99/OngDao_Dam_R99_SectionCandidate.blend'))
A=np.array([637.7344,528.2603]);T=np.array([.76346655,-.64584737]);N=np.array([-.64584737,-.76346655]);out=[]
for name in ['C95_0041_OD35_Surface_1','C95_0043_OD35_Surface_3','C95_0044_OD35_Surface_4','C95_0045_OD35_Surface_5']:
 o=bpy.data.objects[name];v,groups=components(o)
 for ids in groups:
  q=v[ids];sd=(-q[:,:2]-A)@np.c_[T,N];lo=sd.min(0);hi=sd.max(0)
  if hi[0]<57 or lo[0]>63 or hi[1]<-3 or lo[1]>7:continue
  out.append(dict(object=name,count=len(ids),indices=ids,sd_min=lo.tolist(),sd_max=hi.tolist(),y_min=float(q[:,2].min()),y_max=float(q[:,2].max())))
(D/'gate-components.json').write_text(json.dumps(out,indent=2));print('RESULT',json.dumps([{k:v for k,v in x.items() if k!='indices'} for x in out]),flush=True)
