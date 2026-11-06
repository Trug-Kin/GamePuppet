import bpy,json,numpy as np,ast
from pathlib import Path
R=Path('D:/ConRoi');D=R/'Test/KetQua/journey_v48/dam_r99';P=R/'Source/Blender/JourneyV48/R99/OngDao_Dam_R99_SectionCandidate.blend'
bpy.ops.wm.open_mainfile(filepath=str(P));A=np.array([637.7344,528.2603]);B=np.array([681.4578,491.2729]);L=float(np.linalg.norm(B-A));T=(B-A)/L;N=np.array([T[1],-T[0]])
water=np.array(json.loads((R/'Test/KetQua/journey_v48/market_ground_r95/protected-actual-water.geojson').read_text())['coordinates'][0]);tree=ast.parse((R/'Test/Tam/dam98-author.py').read_text());exec(compile(ast.Module(body=[n for n in tree.body if isinstance(n,ast.FunctionDef) and n.name=='shore'],type_ignores=[]),'shore','exec'))
def smooth(x):x=np.clip(x,0,1);return x*x*(3-2*x)
sg=np.linspace(-18,L+18,1001);eg=np.array([shore(float(s)) for s in sg]);rows=[]
assert not bpy.context.scene.get('dam99_common_support_field_applied',False),'Already applied; no repeated displacement'
for o in bpy.context.scene.objects:
 if o.type!='MESH' or not o.get('dam99_export') or o.name.startswith('R99_') or o.get('dam99_approach_continuity') or o.name=='C95_0103_Surface_051':continue
 m=np.array(o.matrix_world);v=np.array([p.co for p in o.data.vertices])@m[:3,:3].T+m[:3,3];world=np.c_[-v[:,0],v[:,2],-v[:,1]];sd=(world[:,[0,2]]-A)@np.c_[T,N];s,dn=sd.T;w=smooth((s+18)/15)*smooth((L+18-s)/15);en=np.interp(s,sg,eg)
 # Invert the original monotone plan field (no horizontal change here).
 ratio=1+(4.5/en-1)*w;edge=en*ratio;d=np.where(dn<=edge,dn/ratio,(dn-(4.5-en)*w*(1+en/20))/(1-(4.5-en)*w/20))
 across=np.where(d<0,smooth((d+20)/6),np.clip(1-d/en,0,1));off=1.859924*w*across;h=world[:,1]
 old=np.where(h<=.22,h,np.where(h>=1.140076+off,h-off,(h+off*.22/.920076)/(1+off/.920076)))
 f=np.clip((old-.22)/.920076,0,1);blend=1-smooth(np.maximum(d,0));new=old+off*(f+(1-f)*blend)
 # Do not lift underwater/low-walk surfaces or regenerate already-authored rails.
 new=np.where(old<=.22001,h,new);dd=new-h
 if np.max(np.abs(dd),initial=0)<1e-5:continue
 v[:,2]=new;inv=np.linalg.inv(m);local=v@inv[:3,:3].T+inv[:3,3]
 for p,q in zip(o.data.vertices,local):p.co=q
 o.data.update();rows.append(dict(object=o.name,changed=int(np.sum(np.abs(dd)>.0001)),max_additional=float(np.max(np.abs(dd)))))
bpy.context.scene['dam99_common_support_field_applied']=True;bpy.context.scene['dam99_support_field']='Landward support displacement is independent of original height above low walk, to keep render/curb support together; blended to original bank field over d0..1. Low walk/water unchanged.'
bpy.context.preferences.filepaths.save_version=0;bpy.ops.wm.save_as_mainfile(filepath=str(P),compress=True);(D/'common-support-field.json').write_text(json.dumps(rows,indent=2));print('RESULT support field corrected',len(rows),flush=True)
