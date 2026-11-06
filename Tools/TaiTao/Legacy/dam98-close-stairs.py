import bpy,bmesh,numpy as np,json,math,ast
from pathlib import Path
R=Path('D:/ConRoi');P=R/'Source/Blender/JourneyV48/R98/OngDao_Dam_R98_Editable.blend';D=R/'Test/KetQua/journey_v48/dam_r98'
bpy.ops.wm.open_mainfile(filepath=str(P));A=np.array([637.7344,528.2603]);B=np.array([681.4578,491.2729]);L=float(np.linalg.norm(B-A));T=(B-A)/L;N=np.array([T[1],-T[0]]);water=np.array(json.loads((R/'Test/KetQua/journey_v48/market_ground_r95/protected-actual-water.geojson').read_text())['coordinates'][0])
tree=ast.parse((R/'Test/Tam/dam98-author.py').read_text());exec(compile(ast.Module(body=[n for n in tree.body if isinstance(n,ast.FunctionDef) and n.name in ['shore','pos']],type_ignores=[]),'helpers','exec'))
rows=[]
for tag,a,b in [('FAR',-3.,0.),('GATE',L,L+3.)]:
 o=bpy.data.objects['R98_'+tag+'_END_STAIR_AND_LOW_LANDING'];vs=[];fs=[];sections=[]
 for s in [a,b]:
  end=shore(s);start=4.1 if tag=='GATE' else .7;run=(end-1.6-start)/8;section=[(0,-.35),(end,-.35),(end,.22),(end-1.6,.22)]
  for i in range(7,-1,-1):section.extend([(start+(i+1)*run,1.140076-(i+1)*(.920076/8)),(start+(i+1)*run,1.140076-i*(.920076/8))])
  section.extend([(start,1.140076),(0,1.140076)])
  section=[p for i,p in enumerate(section) if i==0 or abs(p[0]-section[i-1][0])+abs(p[1]-section[i-1][1])>1e-7]
  sections.append(section);vs.extend([pos(s,d,h) for d,h in section])
 assert len(sections[0])==len(sections[1]);k=len(sections[0]);fs=[(j,(j+1)%k,(j+1)%k+k,j+k) for j in range(k)]
 for side,s in enumerate([a,b]):
  center=len(vs);vs.append(pos(s,shore(s)*.5,-.20))
  for j in range(k):fs.append((center,side*k+j,side*k+(j+1)%k))
 me=bpy.data.meshes.new(o.name+'_ClosedFanCaps');me.from_pydata([(-x,-z,y) for x,y,z in vs],[],fs);me.update();me.materials.append(o.data.materials[0]);uv=me.uv_layers.new(name='UVMap')
 for p in me.polygons:
  for li in p.loop_indices:
   v=me.vertices[me.loops[li].vertex_index].co;uv.data[li].uv=(v.x*.3,v.y*.3)
 bm=bmesh.new();bm.from_mesh(me);bmesh.ops.recalc_face_normals(bm,faces=list(bm.faces));bmesh.ops.triangulate(bm,faces=list(bm.faces),ngon_method='EAR_CLIP');rows.append(dict(name=o.name,nonmanifold=sum(not e.is_manifold for e in bm.edges),minimum_area=min(f.calc_area() for f in bm.faces)));bm.to_mesh(me);bm.free();o.data=me
# Keep each manufactured metal tube closed at welded contact points. Do not
# weld the vertices of separate capped tubes into non-manifold multi-face edges.
tree=ast.parse((R/'Test/Tam/bank97-author.py').read_text());exec(compile(ast.Module(body=[n for n in tree.body if isinstance(n,ast.FunctionDef) and n.name=='beam'],type_ignores=[]),'beam','exec'))
for tag,s in [('FAR_OUT',-3.),('FAR_IN',0.),('GATE_IN',L),('GATE_OUT',L+3.)]:
 o=bpy.data.objects['R98_'+tag+'_STAIR_SIDE_RAIL'];end=shore(s);vv=[];ff=[]
 def ramp(d):return 1.140076-.920076*max(0,min(1,(d-.7)/(end-2.3)))
 def hh(d):
  start=4.1 if tag.startswith('GATE') else .7
  return 1.140076-.920076/8*max(0,min(8,math.ceil((d-start)/((end-1.6-start)/8)-1e-7)))
 endRail=end-1.75 if tag.endswith('OUT') else end-.3
 lastTop=2.170076-.920076*(endRail-.3)/(end-.6)
 beam(vv,ff,pos(s,.3,2.170076),pos(s,endRail,lastTop),.027)
 for d in np.linspace(.3,endRail,15):
  top=2.170076-.920076*(d-.3)/(end-.6)
  beam(vv,ff,pos(s,d,hh(d)-.012),pos(s,d,top+.004),.023)
 me=bpy.data.meshes.new(o.name+'_ClosedTubes');me.from_pydata([(-x,-z,y) for x,y,z in vv],[],ff);me.update();me.materials.append(o.data.materials[0]);uv=me.uv_layers.new(name='UVMap')
 for p in me.polygons:
  for li in p.loop_indices:
   v=me.vertices[me.loops[li].vertex_index].co;uv.data[li].uv=(v.x*.3,v.y*.3)
 bm=bmesh.new();bm.from_mesh(me);bmesh.ops.recalc_face_normals(bm,faces=list(bm.faces));bmesh.ops.triangulate(bm,faces=list(bm.faces));rows.append(dict(name=o.name,nonmanifold=sum(not e.is_manifold for e in bm.edges),minimum_area=min(f.calc_area() for f in bm.faces)));bm.to_mesh(me);bm.free();o.data=me
bpy.context.preferences.filepaths.save_version=0;bpy.ops.wm.save_as_mainfile(filepath=str(P),compress=True);(D/'closed-stair-rail-final.json').write_text(json.dumps(rows,indent=2));print('RESULT',rows,flush=True)
