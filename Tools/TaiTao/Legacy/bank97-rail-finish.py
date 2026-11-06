import bpy,bmesh,numpy as np,math,json,sys
from pathlib import Path
R=Path('D:/ConRoi');D=R/'Test/KetQua/journey_v48/bank_r97';P=R/'Source/Blender/JourneyV48/R97/Bridgehead_Bank_R97_Editable.blend'
bpy.ops.wm.open_mainfile(filepath=str(P));o=bpy.data.objects['R97_PHOTO_SHORT_RAIL'];old=o.data;walk=bpy.data.objects['R97_LOWER_WALK'];from mathutils.bvhtree import BVHTree;from mathutils import Vector
tr=BVHTree.FromPolygons([v.co for v in walk.data.vertices],[tuple(p.vertices) for p in walk.data.polygons]);co=np.array(json.loads((D/'author-result.json').read_text())['coast']);shore=lambda z:float(np.interp(z,co[:,0],co[:,1]));height=lambda z:tr.ray_cast(Vector((-shore(z)-.10,-z,10)),Vector((0,0,-1)),20)[0].z
vv=[];ff=[]
def sweep(points,r):
 k=len(vv);n=len(points)
 for j,p in enumerate(points):
  p=np.array(p);d=np.array(points[min(n-1,j+1)])-np.array(points[max(0,j-1)]);u=np.cross(d,[0,0,1])
  if np.linalg.norm(u)<1e-8:u=np.cross(d,[1,0,0])
  u/=np.linalg.norm(u);w=np.cross(d,u);w/=np.linalg.norm(w)
  for a in np.arange(8)*math.pi/4:vv.append(tuple(p+r*(u*math.cos(a)+w*math.sin(a))))
 ff.extend([tuple(k+i for i in range(7,-1,-1)),tuple(k+(n-1)*8+i for i in range(8))]);ff.extend([(k+j*8+i,k+j*8+(i+1)%8,k+(j+1)*8+(i+1)%8,k+(j+1)*8+i) for j in range(n-1) for i in range(8)])
for h in [.15,.92]:sweep([(-shore(z)-.10,-z,height(z)+h) for z in np.linspace(474,480,25)],.025)
for z in np.arange(474,480.01,.65):sweep([(-shore(z)-.10,-z,height(z)),(-shore(z)-.10,-z,height(z)+.95)],.022)
m=bpy.data.meshes.new('R97_Rail_Continuous_Sweeps');m.from_pydata(vv,[],ff);m.update();bm=bmesh.new();bm.from_mesh(m);bmesh.ops.recalc_face_normals(bm,faces=list(bm.faces));bm.to_mesh(m);bm.free();m.materials.append(old.materials[0]);uv=m.uv_layers.new(name='UVMap')
for p in m.polygons:
 for li in p.loop_indices:
  v=m.vertices[m.loops[li].vertex_index].co;uv.data[li].uv=(v.x,v.y)
o.data=m
# Signed volumes about the per-component centroid avoid catastrophic cancellation
# from ~700m world coordinates on 22mm tubes; verify real orientation in float64.
parts=[];left=set(range(len(m.polygons)));vf={i:[] for i in range(len(m.vertices))}
for p in m.polygons:
 for i in p.vertices:vf[i].append(p.index)
while left:
 stack=[left.pop()];ids=[]
 while stack:
  i=stack.pop();ids.append(i)
  for vi in m.polygons[i].vertices:
   for j in vf[vi]:
    if j in left:left.remove(j);stack.append(j)
 vi=set(i for j in ids for i in m.polygons[j].vertices);center=np.mean([m.vertices[i].co[:] for i in vi],axis=0);vol=0
 for j in ids:
  a=np.array([m.vertices[i].co[:] for i in m.polygons[j].vertices])-center
  vol+=sum(np.dot(a[0],np.cross(a[k],a[k+1]))/6 for k in range(1,len(a)-1))
 if vol<0:
  for j in ids:m.polygons[j].flip()
 parts.append(abs(vol))
m.update()
sys.path.insert(0,str(R/'.agents/skills/scenario-blender-expert/scripts'));import bx_audit
o.location=(float(-np.mean(np.array(vv)[:,0])),float(-np.mean(np.array(vv)[:,1])),float(-np.mean(np.array(vv)[:,2])))
gate=bx_audit.deliverable([o],profile='game');o.location=(0,0,0);gate['check_rebased_to_centroid_only']=True;gate['positive_component_volumes_float64']=parts
(D/'rail-corrected-gate.json').write_text(json.dumps(gate,indent=2,default=str));print('RAIL_GATE',gate['pass'],gate['errors'],flush=True)
assert gate['pass'],gate['errors']
bpy.context.preferences.filepaths.save_version=0;bpy.ops.wm.save_as_mainfile(filepath=str(P),compress=True)
