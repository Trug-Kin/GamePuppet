import bpy,bmesh,numpy as np,json,ast,struct
from pathlib import Path
R=Path('D:/ConRoi');D=R/'Test/KetQua/journey_v48/dam_r99';P=R/'Source/Blender/JourneyV48/R99/OngDao_Dam_R99_SectionCandidate.blend';C=D/'flower-sources'
bpy.ops.wm.open_mainfile(filepath=str(P));sc=bpy.context.scene
A=np.array([637.7344,528.2603]);B=np.array([681.4578,491.2729]);L=float(np.linalg.norm(B-A));T=(B-A)/L;N=np.array([T[1],-T[0]])
water=np.array(json.loads((R/'Test/KetQua/journey_v48/market_ground_r95/protected-actual-water.geojson').read_text())['coordinates'][0]);tree=ast.parse((R/'Test/Tam/dam98-author.py').read_text());exec(compile(ast.Module(body=[n for n in tree.body if isinstance(n,ast.FunctionDef) and n.name=='shore'],type_ignores=[]),'shore','exec'))
def smooth(x):x=max(0,min(1,x));return x*x*(3-2*x)
def warp(v):
 s,d=(v[[0,2]]-A)@np.c_[T,N];w=smooth((s+18)/15)*smooth((L+18-s)/15)
 if not w:return v.copy()
 e=shore(s);u=max(0,min(1,d/e)) if d<=e else max(0,1-(d-e)/20);dn=d+(4.5-e)*w*u;ac=smooth((d+20)/6) if d<0 else max(0,1-d/e)
 x,z=A+T*s+N*dn;return np.array([x,v[1]+1.859924*w*ac*max(0,min(1,(v[1]-.22)/.920076)),z])
placements=json.loads((R/'Docs/Reviews/journey_v48/r22/flower-placements.json').read_text());sel=json.loads((D/'actual-context-selection.json').read_text());report=[]
for line in C.joinpath('index.tsv').read_text(encoding='utf-8-sig').splitlines():
 key,path,mat=line.split('\t');name='D99_FLOWERS_'+key;assert name not in bpy.data.objects
 data=(C/(key+'.bin')).read_bytes();nv,ns=struct.unpack_from('<ii',data);v=np.frombuffer(data,dtype='<f4',count=nv*5,offset=8).reshape(-1,5).astype(float);off=8+nv*20;faces=[];slots=[]
 for j in range(ns):
  ni=struct.unpack_from('<i',data,off)[0];off+=4;t=np.frombuffer(data,dtype='<i4',count=ni,offset=off).reshape(-1,3);off+=ni*4;faces.extend(t.tolist());slots.extend([j]*len(t))
 rows=[p for p in placements if bool(p['yellow'])==key.startswith('Yellow')];assert nv%len(rows)==0;step=nv//len(rows);moved=[]
 for i,row in enumerate(rows):
  root=np.array(row['position']);chunk=v[i*step:(i+1)*step,:3];assert np.linalg.norm(chunk.mean(0)[[0,2]]-root[[0,2]])<1,(key,i,'ordering mismatch')
  s,d=(root[[0,2]]-A)@np.c_[T,N]
  if L+.3<s<L+4.5 and 2<d<5.6:
   dxz=N*(-.85-3.000077);shift=np.array([dxz[0],1.859924,dxz[1]]);kind='same rigid shift as relocated gate planter'
  else:shift=warp(root)-root;kind='rigid root follows scoped support displacement'
  if np.linalg.norm(shift)>.0001:chunk+=shift;moved.append(dict(placement=i,s=float(s),d=float(d),shift=shift.tolist(),mode=kind))
 me=bpy.data.meshes.new(name);me.from_pydata([(-x,-z,y) for x,y,z in v[:,:3]],[],faces);me.update();ma=bpy.data.materials.get(mat)
 if not ma:ma=bpy.data.materials.new(mat)
 me.materials.append(ma);u=me.uv_layers.new(name='UVMap')
 for poly in me.polygons:
  for li in poly.loop_indices:u.data[li].uv=v[me.loops[li].vertex_index,3:5]
 bm=bmesh.new();bm.from_mesh(me);bmesh.ops.recalc_face_normals(bm,faces=list(bm.faces));bm.to_mesh(me);bm.free();me.update()
 o=bpy.data.objects.new(name,me);bpy.data.collections['R99_ACTUAL_CONTEXT'].objects.link(o);o['dam99_binding_path']=path;o['dam99_actual_renderer']=True;o['dam99_actual_collider']=False;o['dam99_flowers']='Actual original editable flower geometry; per-flower rigid source delta, other plants preserved.'
 sel.append(dict(object=name,path=path,render=True,collider=False));report.append(dict(object=name,path=path,vertices=nv,changed_flowers=len(moved),moved=moved,source='R22 flower-placements order verified against actual mesh chunks',outside_changed=False))
sc['dam99_supplemental_flowers']='Actual R22 combined plants included; same scoped root/planter relocation, not a runtime overlay.'
bpy.context.preferences.filepaths.save_version=0;bpy.ops.wm.save_as_mainfile(filepath=str(P),compress=True);(D/'actual-context-selection.json').write_text(json.dumps(sel,indent=2));(D/'supplemental-flower-placement.json').write_text(json.dumps(report,indent=2));print('RESULT',[(x['object'],x['changed_flowers']) for x in report],flush=True)
