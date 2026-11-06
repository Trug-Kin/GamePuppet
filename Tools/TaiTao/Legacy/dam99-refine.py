import bpy,bmesh,json,numpy as np,math,sys,ast
from pathlib import Path
from mathutils import Vector,geometry
from mathutils.bvhtree import BVHTree
R=Path('D:/ConRoi');D=R/'Test/KetQua/journey_v48/dam_r99';M=R/'Test/BangChung/journey_v48/dam_r99';P=R/'Source/Blender/JourneyV48/R99/OngDao_Dam_R99_SectionCandidate.blend'
bpy.ops.wm.open_mainfile(filepath=str(P));sc=bpy.context.scene
A=np.array([637.7344,528.2603]);B=np.array([681.4578,491.2729]);L=float(np.linalg.norm(B-A));T=(B-A)/L;N=np.array([T[1],-T[0]]);H=3.0;W=4.5
water=np.array(json.loads((R/'Test/KetQua/journey_v48/market_ground_r95/protected-actual-water.geojson').read_text())['coordinates'][0]);tree=ast.parse((R/'Test/Tam/dam98-author.py').read_text());exec(compile(ast.Module(body=[n for n in tree.body if isinstance(n,ast.FunctionDef) and n.name in ['shore','pos']],type_ignores=[]),'helpers','exec'))
def smooth(x):x=np.clip(x,0,1);return x*x*(3-2*x)
def warp2(p):
 s,d=(p-A)@np.c_[T,N];w=float(smooth((s+18)/15)*smooth((L+18-s)/15));en=shore(s) if w>1e-12 else 1;u=max(0,min(1,d/en)) if d<=en else max(0,1-(d-en)/20);return A+T*s+N*(d+(W-en)*w*u)
# Rebuild water boundary from protected actual R95 footprint, densify before warp,
# so large historic triangles cannot shortcut the local boundary transition.
# The proposed plan edge remains unverified: this fixes technical discretization only.
pts=[];raw=[]
for a,b in zip(water,water[1:]):
 for t in np.arange(max(1,math.ceil(np.linalg.norm(b-a)/.5)))/max(1,math.ceil(np.linalg.norm(b-a)/.5)):
  q=a+(b-a)*t;raw.append(q);r=warp2(q);pts.append(Vector((-r[0],-r[1],.00005)))
tris=geometry.tessellate_polygon([pts]);lookup={tuple(p):i for i,p in enumerate(pts)};faces=[tuple(int(p) if isinstance(p,int) else lookup[tuple(p)] for p in tri) for tri in tris]
o=bpy.data.objects['C95_0103_Surface_051'];me=bpy.data.meshes.new('R99_CANDIDATE_WATER_LOCAL_EDGE_ONLY');me.from_pydata(pts,[],faces);me.update()
for mat in o.data.materials:me.materials.append(mat)
uv=me.uv_layers.new(name='UVMap')
for poly in me.polygons:
 for li in poly.loop_indices:
  p=raw[me.loops[li].vertex_index];uv.data[li].uv=(float(p[0])*.002,float(p[1])*.002)
bm=bmesh.new();bm.from_mesh(me);bmesh.ops.recalc_face_normals(bm,faces=list(bm.faces));bm.to_mesh(me);bm.free()
# Top surface facing up, retain original material names for review. Not exported.
if sum(p.normal.z*p.area for p in me.polygons)<0:
 bm=bmesh.new();bm.from_mesh(me);bmesh.ops.reverse_faces(bm,faces=list(bm.faces));bm.to_mesh(me);bm.free()
o.data=me;o.matrix_world.identity();o.hide_render=False
if me.materials:
 ma=me.materials[0].copy();ma.name='R99_CANDIDATE_WATER_PREVIEW_ONLY';ma.diffuse_color=(.055,.18,.23,1);me.materials[0]=ma
# Correct two inner handrails: previous straight chord missed clamped-ramp posts.
# A continuous mitered tube follows plateau-ramp-plateau, with closed end caps.
tree=ast.parse((R/'Test/Tam/bank97-author.py').read_text());exec(compile(ast.Module(body=[n for n in tree.body if isinstance(n,ast.FunctionDef) and n.name=='beam'],type_ignores=[]),'beam','exec'))
rail_reports=[]
for tag,s in [('FAR_IN',0.),('GATE_IN',L)]:
 o=bpy.data.objects['R99_'+tag+'_STAIR_RAIL'];vv=[];ff=[];limit=W-.15
 def hh(d):return H-(H-.22)/14*max(0,min(14,math.ceil((d-.45)/((W-1.10)/14)-1e-7)))
 def top(d):return H+1.03-(H-.22)*max(0,min(1,(d-.45)/(W-1.10)))
 ds=[.2,.45,W-.65,limit];centers=[np.array(pos(s,d,top(d))) for d in ds];trans=np.array([T[0],0,T[1]])
 normals=[]
 for a,b in zip(centers,centers[1:]):
  q=b-a;q/=np.linalg.norm(q);n=np.cross(trans,q);n/=np.linalg.norm(n);normals.append(n)
 for i,c in enumerate(centers):
  n=normals[0] if i==0 else normals[-1] if i==len(centers)-1 else normals[i-1]+normals[i]
  n=n/np.linalg.norm(n);mit=.027/(np.dot(n,normals[min(i,len(normals)-1)]))
  vv.extend([tuple(c+trans*.027+n*mit),tuple(c-trans*.027+n*mit),tuple(c-trans*.027-n*mit),tuple(c+trans*.027-n*mit)])
 for i in range(len(centers)-1):
  for j in range(4):ff.append((i*4+j,i*4+(j+1)%4,(i+1)*4+(j+1)%4,(i+1)*4+j))
 ff.extend([(3,2,1,0),tuple((len(centers)-1)*4+j for j in range(4))])
 for d in np.linspace(.2,limit,12):beam(vv,ff,pos(s,d,hh(d)-.012),pos(s,d,top(d)+.004),.023)
 me=bpy.data.meshes.new(o.name+'_ContinuousTopRail');me.from_pydata([(-x,-z,y) for x,y,z in vv],[],ff);me.update();me.materials.append(o.data.materials[0]);uv=me.uv_layers.new(name='UVMap')
 for p in me.polygons:
  for li in p.loop_indices:
   v=me.vertices[me.loops[li].vertex_index].co;uv.data[li].uv=(v.x*.3,v.y*.3)
 bm=bmesh.new();bm.from_mesh(me);bmesh.ops.recalc_face_normals(bm,faces=list(bm.faces));bm.to_mesh(me);bm.free();o.data=me
 rail_reports.append(o.name)
sc['dam99_technical_refinement']='Densified candidate water boundary before warp; two inner top rails now continuous piecewise tubes. These do not validate proposed shoreline or full support/continuity.'
(D/'candidate-technical-refinement.json').write_text(json.dumps(dict(water_boundary_points=len(pts),water_triangles=len(faces),water_y=.00005,water_plan_status='UNVERIFIED proposed local shoreline; not Unity',rails=rail_reports),indent=2));bpy.context.preferences.filepaths.save_version=0;bpy.ops.wm.save_as_mainfile(filepath=str(P),compress=True);print('RESULT technical candidate refinements saved',flush=True)

