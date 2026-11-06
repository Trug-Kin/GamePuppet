import bpy,bmesh,numpy as np,json,math,ast
from pathlib import Path
from mathutils import Vector
R=Path('D:/ConRoi');D=R/'Test/KetQua/journey_v48/dam_r99';P=R/'Source/Blender/JourneyV48/R99/OngDao_Dam_R99_SectionCandidate.blend';old=R/'Source/Blender/JourneyV48/R98/OngDao_Dam_R98_Editable.blend'
A=np.array([637.7344,528.2603]);B=np.array([681.4578,491.2729]);L=float(np.linalg.norm(B-A));T=(B-A)/L;N=np.array([T[1],-T[0]])
bpy.ops.wm.open_mainfile(filepath=str(old));original={}
for o in bpy.context.scene.objects:
 if o.type=='MESH' and (o.name in ['C95_0041_OD35_Surface_1','C95_0043_OD35_Surface_3','C95_0044_OD35_Surface_4','C95_0045_OD35_Surface_5'] or 'FlowerModule' in o.name):
  original[o.name]=dict(v=np.array([o.matrix_world@v.co for v in o.data.vertices],dtype=float),edges=np.array([e.vertices[:] for e in o.data.edges]),matrix=np.array(o.matrix_world,dtype=float))
bpy.ops.wm.open_mainfile(filepath=str(P));changes=[]
for name,data in original.items():
 o=bpy.data.objects.get(name)
 if not o or len(o.data.vertices)!=len(data['v']):continue
 v=data['v'];parent=np.arange(len(v))
 def find(a):
  while parent[a]!=a:parent[a]=parent[parent[a]];a=parent[a]
  return a
 def union(a,b):
  a=find(a);b=find(b)
  if a!=b:parent[b]=a
 for a,b in data['edges']:union(int(a),int(b))
 seen={}
 for i,p in enumerate(v):
  key=tuple(np.round(p,4))
  if key in seen:union(i,seen[key])
  else:seen[key]=i
 groups={}
 for i in range(len(v)):groups.setdefault(find(i),[]).append(i)
 for ids in groups.values():
  pts=v[ids];cent=(pts.min(0)+pts.max(0))*.5;s,d=(np.array([-cent[0],-cent[1]])-A)@np.c_[T,N]
  if not L+.3<s<L+4.5 or not 2.0<d<5.6:continue
  # Move the existing planter/lamp assembly rigidly onto bridge-side support.
  target_d=-.85;dxz=N*(target_d-3.000077);shift=np.array([-dxz[0],-dxz[1],3.0-1.140076]);inv=np.linalg.inv(np.array(o.matrix_world,dtype=float));new=pts+shift;local=new@inv[:3,:3].T+inv[:3,3]
  for i,p in zip(ids,local):o.data.vertices[i].co=p
  changes.append(dict(object=name,vertices=len(ids),s=float(s),old_d=float(d),target_d=target_d,vertical_shift=float(shift[2])))
 o.data.update()
# Source terrain conversion had downward surface winding in the near far-head tie.
# Correct only affected exposed surface faces, never an unrelated whole terrain.
flips=[]
for name in ['BankR91_Grade_017','BankR91_Context125_R91']:
 o=bpy.data.objects.get(name)
 if not o:continue
 bm=bmesh.new();bm.from_mesh(o.data);bm.normal_update();selected=[]
 for f in bm.faces:
  c=f.calc_center_median();s,d=(np.array([-c.x,-c.y])-A)@np.c_[T,N]
  if -18.1<s<-2.99 and -2<d<12 and c.z>-.1 and f.normal.z<-.1:selected.append(f)
 bmesh.ops.reverse_faces(bm,faces=selected);bm.to_mesh(o.data);bm.free();o.data.update();flips.append(dict(object=name,faces=len(selected)))
# Rebuild every stair top rail with the same continuous clamped-ramp sweep.
# Reuse proven inner-rail construction, changing end limit for the two outer exits.
s=(R/'Test/Tam/dam99-refine.py').read_text(encoding='utf-8-sig');part=s[s.index("rail_reports=[]"):s.index("sc['dam99_technical_refinement']")];part=part.replace("[('FAR_IN',0.),('GATE_IN',L)]","[('FAR_OUT',-3.),('FAR_IN',0.),('GATE_IN',L),('GATE_OUT',L+3.)]").replace('limit=W-.15','limit=W-.75 if tag.endswith(\'OUT\') else W-.15').replace('ds=[.2,.45,W-.65,limit]','ds=sorted(set([.2,.45,limit]+([W-.65] if W-.65<limit else [])))')
H=3.;W=4.5
# Helpers exactly as the candidate authoring, positive-up closed tubes.
def pos(s,d,h):
 x,z=A+T*s+N*d;return (float(x),float(h),float(z))
tree=ast.parse((R/'Test/Tam/bank97-author.py').read_text());exec(compile(ast.Module(body=[n for n in tree.body if isinstance(n,ast.FunctionDef) and n.name=='beam'],type_ignores=[]),'beam','exec'));exec(part)
sc=bpy.context.scene;sc['dam99_endpoint_revision']='Existing gate planter/lamp relocated rigidly from stair throat to bridge-side support; local far exposed soil winding corrected; all four top handrails follow continuous ramp. Needs fresh visual/support review.'
(D/'endpoint-refinement.json').write_text(json.dumps(dict(existing_prop_components_relocated=changes,source_winding=flips,rails=rail_reports,status='AWAITING_FRESH_REVIEW'),indent=2));bpy.context.preferences.filepaths.save_version=0;bpy.ops.wm.save_as_mainfile(filepath=str(P),compress=True);print('RESULT endpoint refinement',len(changes),flips,flush=True)

