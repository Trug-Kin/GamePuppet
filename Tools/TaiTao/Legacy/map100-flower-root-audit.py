import bpy,numpy as np,json
from pathlib import Path
from mathutils import Vector
from mathutils.bvhtree import BVHTree
R=Path('D:/ConRoi');D=R/'Test/KetQua/journey_v48/map_r99_closeout'
bpy.ops.wm.open_mainfile(filepath=str(R/'Source/Blender/JourneyV48/R99/OngDao_Dam_R99_SectionCandidate.blend'))
A=np.array([637.7344,528.2603]);T=np.array([.76346655,-.64584737]);N=np.array([-.64584737,-.76346655])
soil=bpy.data.objects['C95_0045_OD35_Surface_5'];tr=BVHTree.FromPolygons([soil.matrix_world@p.co for p in soil.data.vertices],[tuple(p.vertices) for p in soil.data.polygons]);out=[]
for color in ['Red','Yellow']:
 o=bpy.data.objects['D99_FLOWERS_'+color+'_0'];v=np.array([o.matrix_world@p.co for p in o.data.vertices]);attr=np.array([a.value for a in o.data.attributes['r22_plant_index'].data])
 for ix in np.unique(attr):
  q=v[attr==ix];bottom=q[:,2].min();root=q[np.abs(q[:,2]-bottom)<.0001].mean(0);sd=(-root[:2]-A)@np.c_[T,N]
  if not(-18<sd[0]<76 and -17<sd[1]<8):continue
  hit,n,_,_=tr.ray_cast(Vector((root[0],root[1],bottom+1)),Vector((0,0,-1)),4)
  out.append(dict(color=color,index=int(ix),s=float(sd[0]),d=float(sd[1]),actual_root_native=root.tolist(),soil=float(hit.z) if hit else None,gap=float(bottom-hit.z) if hit else None))
(D/'actual-flower-root-audit.json').write_text(json.dumps(out,indent=2));bad=[r for r in out if r['gap'] is None or abs(r['gap'])>.025];print('RESULT actual plants',len(out),'unsupported',len(bad),json.dumps(bad[:25]),flush=True)
