import bpy,json,numpy as np
from pathlib import Path
from mathutils import Vector
from mathutils.bvhtree import BVHTree
R=Path('D:/ConRoi');D=R/'Test/KetQua/journey_v48/map_r99_closeout'
bpy.ops.wm.open_mainfile(filepath=str(R/'Source/Blender/JourneyV48/R99/OngDao_Dam_R99_SectionCandidate.blend'))
A=np.array([637.7344,528.2603]);T=np.array([.76346655,-.64584737]);N=np.array([-.64584737,-.76346655]);L=57.2695684
def components(o):
 v=np.array([o.matrix_world@p.co for p in o.data.vertices]);parents=np.arange(len(v))
 def find(i):
  while parents[i]!=i:parents[i]=parents[parents[i]];i=parents[i]
  return i
 def union(a,b):
  a=find(a);b=find(b)
  if a!=b:parents[b]=a
 for e in o.data.edges:union(int(e.vertices[0]),int(e.vertices[1]))
 same={}
 for i,q in enumerate(v):
  k=tuple(np.round(q,4))
  if k in same:union(i,same[k])
  else:same[k]=i
 groups={}
 for i in range(len(v)):groups.setdefault(find(i),[]).append(i)
 return v,list(groups.values())
trees={}
for o in bpy.context.scene.objects:
 if o.type!='MESH' or o.hide_render or any(c.hide_render for c in o.users_collection):continue
 if not (o.get('dam99_actual_renderer') or o.name.startswith('R99_')) or o.name.startswith('R99_SUPPORT_'):continue
 if any(x in o.name for x in ['FLOWER','FlowerModule','OD35_Surface_3','OD35_Surface_4','OD35_Surface_6','OD35_Surface_7','OD35_Surface_10','RAIL','GRILLE']):continue
 if not len(o.data.vertices):continue
 trees[o.name]=BVHTree.FromPolygons([o.matrix_world@p.co for p in o.data.vertices],[tuple(p.vertices) for p in o.data.polygons])
o=bpy.data.objects['C95_0043_OD35_Surface_3'];v,groups=components(o);rows=[]
for ids in groups:
 q=v[ids];lo=q.min(0);hi=q.max(0);mid=(lo+hi)/2;sd=(np.array([-mid[0],-mid[1]])-A)@np.c_[T,N]
 if not(-18<sd[0]<L+18 and -16<sd[1]<8):continue
 if hi[2]-lo[2]<1.5 or max(hi[:2]-lo[:2])>2:continue
 hits=[]
 for name,tr in trees.items():
  p,n,fi,dist=tr.ray_cast(Vector((mid[0],mid[1],lo[2]+.02)),Vector((0,0,-1)),8)
  if p and n.z>.7:hits.append(dict(object=name,y=float(p.z),gap=float(lo[2]-p.z)))
 rows.append(dict(object=o.name,indices=ids,s=float(sd[0]),d=float(sd[1]),unity_xz=[float(-mid[0]),float(-mid[1])],bottom=float(lo[2]),top=float(hi[2]),extent=(hi-lo).tolist(),supports=sorted(hits,key=lambda x:abs(x['gap']))[:4]))
(D/'lamp-stem-support-r99.json').write_text(json.dumps(rows,indent=2));print('RESULT',json.dumps([{k:x for k,x in r.items() if k!='indices'} for r in rows],indent=2),flush=True)
