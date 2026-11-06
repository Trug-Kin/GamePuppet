import bpy,bmesh,json,numpy as np,ast,math
from pathlib import Path
R=Path('D:/ConRoi');D=R/'Test/KetQua/journey_v48/dam_r99';P=R/'Source/Blender/JourneyV48/R99/OngDao_Dam_R99_SectionCandidate.blend';C=D/'collider-sources'
bpy.ops.wm.open_mainfile(filepath=str(P));sc=bpy.context.scene
A=np.array([637.7344,528.2603]);B=np.array([681.4578,491.2729]);L=float(np.linalg.norm(B-A));T=(B-A)/L;N=np.array([T[1],-T[0]])
water=np.array(json.loads((R/'Test/KetQua/journey_v48/market_ground_r95/protected-actual-water.geojson').read_text())['coordinates'][0]);tree=ast.parse((R/'Test/Tam/dam98-author.py').read_text());exec(compile(ast.Module(body=[n for n in tree.body if isinstance(n,ast.FunctionDef) and n.name=='shore'],type_ignores=[]),'shore','exec'))
def smooth(x):x=np.clip(x,0,1);return x*x*(3-2*x)
sg=np.linspace(-18,L+18,1001);eg=np.array([shore(float(s)) for s in sg])
def warp(v):
 sd=(v[:,[0,2]]-A)@np.c_[T,N];s,d=sd.T;w=smooth((s+18)/15)*smooth((L+18-s)/15);en=np.interp(s,sg,eg);u=np.where(d<=en,np.clip(d/en,0,1),np.clip(1-(d-en)/20,0,1));dn=d+(4.5-en)*w*u;ac=np.where(d<0,smooth((d+20)/6),np.clip(1-d/en,0,1));h=v[:,1]+1.859924*w*ac*np.clip((v[:,1]-.22)/.920076,0,1);xz=A+s[:,None]*T+dn[:,None]*N;return np.c_[xz[:,0],h,xz[:,1]]
collection=bpy.data.collections.get('R99_ACTUAL_COLLIDERS_EDITABLE') or bpy.data.collections.new('R99_ACTUAL_COLLIDERS_EDITABLE')
if collection.name not in sc.collection.children:sc.collection.children.link(collection)
mapped={x['path']:bpy.data.objects[x['object']] for x in json.loads((D/'actual-context-selection.json').read_text())};rows=[]
for line in C.joinpath('index.tsv').read_text(encoding='utf-8-sig').splitlines():
 key,path,asset,shared=line.split('\t');raw=(C/(key+'.bin')).read_bytes();nv,ni=np.frombuffer(raw[:8],dtype='<i4');v=np.frombuffer(raw,dtype='<f4',count=int(nv)*3,offset=8).reshape(-1,3).astype(float);tri=np.frombuffer(raw,dtype='<i4',count=int(ni),offset=8+int(nv)*12).reshape(-1,3)
 if np.max(np.linalg.norm(warp(v)-v,axis=1),initial=0)<.0001:continue
 if path in mapped and shared=='True':
  o=mapped[path];o['dam99_actual_collider']=True;rows.append(dict(key=key,path=path,object=o.name,mode='shared_final_render_mesh',source_asset=asset));continue
 name='R99_SUPPORT_'+key
 if name in bpy.data.objects:raise RuntimeError('Already imported '+name)
 me=bpy.data.meshes.new(name+'_Original');me.from_pydata([(-x,-z,y) for x,y,z in v],[],tri.tolist());me.update();bm=bmesh.new();bm.from_mesh(me)
 # Refine just faces whose edges enter the finite deformation field. Subdivide
 # before deformation, preserving the old plane away from its support field.
 for repeat in range(9):
  selected=[]
  for e in bm.edges:
   if e.calc_length()<=1.25:continue
   q=np.array([[-p.co.x,-p.co.y] for p in e.verts]);sd=(q-A)@np.c_[T,N]
   if sd[:,0].max()>=-19 and sd[:,0].min()<=L+19 and sd[:,1].max()>=-21 and sd[:,1].min()<=35:selected.append(e)
  if not selected:break
  bmesh.ops.subdivide_edges(bm,edges=selected,cuts=1,use_grid_fill=True)
 bm.to_mesh(me);bm.free();v=np.array([[-p.co.x,p.co.z,-p.co.y] for p in me.vertices]);out=warp(v)
 for p,(x,y,z) in zip(me.vertices,out):p.co=(-x,-z,y)
 me.update();o=bpy.data.objects.new(name,me);collection.objects.link(o);o.hide_render=True;o['dam99_binding_path']=path;o['dam99_collider_only']=True;o['dam99_export']=True;o['dam99_captured_old_asset']=asset;o['dam99_actual_collider']=True
 rows.append(dict(key=key,path=path,object=o.name,mode='separate_collider_source',vertices=len(me.vertices),triangles=len(me.polygons),source_asset=asset))
(D/'collider-source-mapping.json').write_text(json.dumps(rows,indent=2));sc['dam99_collision_gate']='Actual affected supports imported/mapped; stair intrusion/continuity gate pending, NOT exported.';bpy.context.preferences.filepaths.save_version=0;bpy.ops.wm.save_as_mainfile(filepath=str(P),compress=True);print('RESULT affected old colliders',len(rows),'new editable',sum(x['mode']=='separate_collider_source' for x in rows),flush=True)
