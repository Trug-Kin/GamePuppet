import bpy,json,numpy as np,ast
from pathlib import Path
R=Path('D:/ConRoi');D=R/'Test/KetQua/journey_v48/dam_r99';P=R/'Source/Blender/JourneyV48/R99/OngDao_Dam_R99_SectionCandidate.blend'
bpy.ops.wm.open_mainfile(filepath=str(P));A=np.array([637.7344,528.2603]);B=np.array([681.4578,491.2729]);L=float(np.linalg.norm(B-A));T=(B-A)/L;N=np.array([T[1],-T[0]])
water=np.array(json.loads((R/'Test/KetQua/journey_v48/market_ground_r95/protected-actual-water.geojson').read_text())['coordinates'][0]);tree=ast.parse((R/'Test/Tam/dam98-author.py').read_text());exec(compile(ast.Module(body=[n for n in tree.body if isinstance(n,ast.FunctionDef) and n.name=='shore'],type_ignores=[]),'shore','exec'))
def smooth(x):x=np.clip(x,0,1);return x*x*(3-2*x)
sg=np.linspace(-18,L+18,1001);eg=np.array([shore(float(s)) for s in sg]);names={r['object'] for r in json.loads((D/'common-support-field.json').read_text())}
assert bpy.context.scene.get('dam99_common_support_field_applied')
for o in bpy.context.scene.objects:
 if o.name not in names:continue
 m=np.array(o.matrix_world);v=np.array([p.co for p in o.data.vertices])@m[:3,:3].T+m[:3,3];world=np.c_[-v[:,0],v[:,2],-v[:,1]];sd=(world[:,[0,2]]-A)@np.c_[T,N];s,dn=sd.T;w=smooth((s+18)/15)*smooth((L+18-s)/15);en=np.interp(s,sg,eg);ratio=1+(4.5/en-1)*w;edge=en*ratio;d=np.where(dn<=edge,dn/ratio,(dn-(4.5-en)*w*(1+en/20))/(1-(4.5-en)*w/20))
 across=np.where(d<0,smooth((d+20)/6),np.clip(1-d/en,0,1));off=1.859924*w*across;h=world[:,1];blend=1-smooth(np.maximum(d,0))
 old=np.where(h<=.22001+off*blend,h,np.where(h>=1.140076+off,h-off,(h-off*blend+off*(1-blend)*.22/.920076)/(1+off*(1-blend)/.920076)))
 f=np.clip((old-.22)/.920076,0,1);new=old+off*f;v[:,2]=new;inv=np.linalg.inv(m);local=v@inv[:3,:3].T+inv[:3,3]
 for p,q in zip(o.data.vertices,local):p.co=q
 o.data.update()
bpy.context.scene['dam99_common_support_field_applied']=False;bpy.context.scene['dam99_support_field']='Height-independent support experiment reverted analytically; continuous original field retained. Contact-sheet topology is subdivided before the same field as adjacent ground.'
bpy.context.preferences.filepaths.save_version=0;bpy.ops.wm.save_as_mainfile(filepath=str(P),compress=True);print('RESULT reverted support experiment',len(names),flush=True)
