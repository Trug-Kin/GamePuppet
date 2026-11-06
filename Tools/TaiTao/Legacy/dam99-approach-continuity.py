import bpy,bmesh,json,numpy as np,ast
from pathlib import Path
from mathutils import Vector
R=Path('D:/ConRoi');D=R/'Test/KetQua/journey_v48/dam_r99';P=R/'Source/Blender/JourneyV48/R99/OngDao_Dam_R99_SectionCandidate.blend'
names=['C95_0040_OD35_Surface_0','C95_0042_OD35_Surface_2']+[f'C95_{i:04d}_'+n for i,n in [(635,'PlanterWalkConnection_0'),(636,'PlanterWalkConnection_1'),(637,'PlanterWalkConnection_2'),(638,'PlanterWalkConnection_3')]]
# Only road/sidewalk/edge contact sheets. Preserve the exact prior dam cut.
bpy.ops.wm.open_mainfile(filepath=str(R/'Source/Blender/JourneyV48/R98/OngDao_Dam_R98_Editable.blend'))
names += [o.name for o in bpy.context.scene.objects if o.name.startswith('Grade_085') or o.name.startswith('C95_06') and 'PavingEdge' in o.name]
original={}
for n in names:
 o=bpy.data.objects.get(n)
 if not o:continue
 me=o.data;me.calc_loop_triangles();uv=list(me.uv_layers);original[n]=dict(materials=[m.name for m in me.materials],uv=[u.name for u in uv],tri=[(p.material_index,[np.r_[o.matrix_world@me.vertices[me.loops[i].vertex_index].co,*[u.data[i].uv for u in uv]] for i in p.loops]) for p in me.loop_triangles])
bpy.ops.wm.open_mainfile(filepath=str(P));bpy.context.preferences.filepaths.save_version=0
stage=P.parent/'Stages';stage.mkdir(exist_ok=True);backup=stage/'Before_Approach_Continuity.blend'
if not backup.exists():bpy.ops.wm.save_as_mainfile(filepath=str(backup),copy=True,compress=True)
A=np.array([637.7344,528.2603]);B=np.array([681.4578,491.2729]);L=float(np.linalg.norm(B-A));T=(B-A)/L;N=np.array([T[1],-T[0]])
water=np.array(json.loads((R/'Test/KetQua/journey_v48/market_ground_r95/protected-actual-water.geojson').read_text())['coordinates'][0]);tree=ast.parse((R/'Test/Tam/dam98-author.py').read_text());exec(compile(ast.Module(body=[n for n in tree.body if isinstance(n,ast.FunctionDef) and n.name=='shore'],type_ignores=[]),'shore','exec'))
def smooth(x):x=np.clip(x,0,1);return x*x*(3-2*x)
def clip(poly,axis,cut,above):
 out=[]
 for a,b in zip(poly,poly[1:]+poly[:1]):
  aa=(np.array([-a[0],-a[1]])-A)@axis-cut;bb=(np.array([-b[0],-b[1]])-A)@axis-cut;ia=aa>=-1e-9 if above else aa<=1e-9;ib=bb>=-1e-9 if above else bb<=1e-9
  if ia:out.append(a)
  if ia!=ib and abs(aa-bb)>1e-12:out.append(a+(b-a)*aa/(aa-bb))
 return out
rows=[]
for n,src in original.items():
 o=bpy.data.objects[n];pieces=[];splits=0
 for slot,poly in src['tri']:
  sd=np.array([(np.array([-v[0],-v[1]])-A)@np.c_[T,N] for v in poly]);lo=sd.min(0);hi=sd.max(0);parts=[poly]
  if hi[0]>-19 and lo[0]<L+19 and hi[1]>-21 and lo[1]<.001:
   lines=[(T,float(s)) for s in np.r_[np.arange(-18,-2.99,.5),np.arange(L+3,L+18.01,.5)]]+[(N,float(d)) for d in np.arange(-20,-13.99,.5)]
   for axis,c in lines:
    nxt=[]
    for a in parts:
     vals=[(np.array([-v[0],-v[1]])-A)@axis-c for v in a]
     if min(vals)<-1e-7 and max(vals)>1e-7:nxt.extend([clip(a,axis,c,False),clip(a,axis,c,True)]);splits+=1
     else:nxt.append(a)
    parts=nxt
  pieces.extend((slot,p) for p in parts)
 verts=[];faces=[];slots=[];uu=[[] for _ in src['uv']];dedup={}
 for slot,poly in pieces:
  ids=[]
  for v in poly:
   s,d=(np.array([-v[0],-v[1]])-A)@np.c_[T,N];w=float(smooth((s+18)/15)*smooth((L+18-s)/15));ac=float(smooth((d+20)/6)) if d<0 else max(0,1-d/10)
   # All vertices of these bridge contact sheets move with their own support;
   # no height-based stretching between road and adjacent curb.
   vv=v.copy();vv[2]+=1.859924*w*ac*float(np.clip((v[2]-.22)/.920076,0,1))
   if w>1e-9 and d>0:
    end=shore(s);u=min(1,d/end) if d<=end else max(0,1-(d-end)/20);dn=d+(4.5-end)*w*u;xz=A+T*s+N*dn;vv[0]=-xz[0];vv[1]=-xz[1]
   k=tuple(np.round(vv,8));ix=dedup.get(k)
   if ix is None:
    ix=len(verts);dedup[k]=ix;verts.append(tuple(vv[:3]));[uu[j].append(vv[3+2*j:5+2*j]) for j in range(len(uu))]
   ids.append(ix)
  for j in range(1,len(ids)-1):
   q=np.array([verts[i] for i in [ids[0],ids[j],ids[j+1]]]);
   if np.linalg.norm(np.cross(q[1]-q[0],q[2]-q[0]))>2e-8:faces.append((ids[0],ids[j],ids[j+1]));slots.append(slot)
 me=bpy.data.meshes.new(n+'_R99_Continuous_Approach');me.from_pydata(verts,[],faces);me.update()
 for name in src['materials']:me.materials.append(bpy.data.materials[name])
 for j,name in enumerate(src['uv']):
  u=me.uv_layers.new(name=name)
  for p in me.polygons:
   for li in p.loop_indices:u.data[li].uv=uu[j][me.loops[li].vertex_index]
 for p,slot in zip(me.polygons,slots):p.material_index=slot
 o.data=me;o.matrix_world.identity();o['dam99_approach_continuity']='Original R98 contact sheet, split at shared s/d stations then one support displacement; UV preserved. Not a new overlay.'
 rows.append(dict(object=n,splits=splits,triangles=len(faces),vertices=len(verts)))
bpy.ops.wm.save_as_mainfile(filepath=str(P),compress=True);(D/'approach-continuity.json').write_text(json.dumps(rows,indent=2));print('RESULT rebuilt contact sheets',len(rows),flush=True)
